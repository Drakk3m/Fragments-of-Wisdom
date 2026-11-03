using System;
using Game.Domain.Classes;
using Game.Domain.Skills;

namespace Game.Domain.Progress
{
    public class V4ProgressRestoreCoordinator
    {
        private readonly V2ProgressSnapshotService snapshotService;
        private readonly V2ProgressSnapshotValidationService validationService =
            new V2ProgressSnapshotValidationService();

        public V4ProgressRestoreCoordinator()
            : this(new V2ProgressSnapshotService())
        {
        }

        public V4ProgressRestoreCoordinator(V2ProgressSnapshotService snapshotService)
        {
            if (snapshotService == null)
            {
                throw new ArgumentNullException(nameof(snapshotService));
            }

            this.snapshotService = snapshotService;
        }

        public V4ProgressRestoreResult Restore(
            V2ProgressSnapshot snapshot,
            ClassProfile classProfile,
            MasteryProfile masteryProfile,
            MentorRelationshipProfile mentorRelationshipProfile,
            SkillTreeProgressProfile skillTreeProgressProfile,
            ClassDefinitionRegistry classRegistry,
            SkillDefinitionRegistry skillRegistry,
            MentorDefinitionRegistry mentorRegistry,
            SkillTreeDefinition skillTree)
        {
            return RestoreInternal(
                snapshot,
                classProfile,
                masteryProfile,
                mentorRelationshipProfile,
                skillTreeProgressProfile,
                classRegistry,
                skillRegistry,
                mentorRegistry,
                skillTree,
                null,
                false);
        }

        public V4ProgressRestoreResult Restore(
            V2ProgressSnapshot snapshot,
            ClassProfile classProfile,
            MasteryProfile masteryProfile,
            MentorRelationshipProfile mentorRelationshipProfile,
            SkillTreeProgressProfile skillTreeProgressProfile,
            ClassDefinitionRegistry classRegistry,
            SkillDefinitionRegistry skillRegistry,
            MentorDefinitionRegistry mentorRegistry,
            SkillTreeDefinition skillTree,
            ClassSkillTreeBindingRegistry classSkillTreeBindingRegistry)
        {
            if (classSkillTreeBindingRegistry == null)
            {
                throw new ArgumentNullException(nameof(classSkillTreeBindingRegistry));
            }

            return RestoreInternal(
                snapshot,
                classProfile,
                masteryProfile,
                mentorRelationshipProfile,
                skillTreeProgressProfile,
                classRegistry,
                skillRegistry,
                mentorRegistry,
                skillTree,
                classSkillTreeBindingRegistry,
                true);
        }

        private V4ProgressRestoreResult RestoreInternal(
            V2ProgressSnapshot snapshot,
            ClassProfile classProfile,
            MasteryProfile masteryProfile,
            MentorRelationshipProfile mentorRelationshipProfile,
            SkillTreeProgressProfile skillTreeProgressProfile,
            ClassDefinitionRegistry classRegistry,
            SkillDefinitionRegistry skillRegistry,
            MentorDefinitionRegistry mentorRegistry,
            SkillTreeDefinition skillTree,
            ClassSkillTreeBindingRegistry classSkillTreeBindingRegistry,
            bool restoreClassSkillTreeBindings)
        {
            // Temporary-profile preflight cannot validate the real destination references.
            if (classProfile == null)
            {
                throw new ArgumentNullException(nameof(classProfile));
            }

            if (masteryProfile == null)
            {
                throw new ArgumentNullException(nameof(masteryProfile));
            }

            if (mentorRelationshipProfile == null)
            {
                throw new ArgumentNullException(nameof(mentorRelationshipProfile));
            }

            if (skillTreeProgressProfile == null)
            {
                throw new ArgumentNullException(nameof(skillTreeProgressProfile));
            }

            var preflightBindingRegistry = restoreClassSkillTreeBindings
                ? BuildPreflightBindingRegistry(
                    classSkillTreeBindingRegistry,
                    classRegistry)
                : null;
            bool canRestoreClass = snapshotService.RestoreClassProfile(
                snapshot,
                new ClassProfile(),
                classRegistry);
            bool canRestoreMastery = snapshotService.RestoreMasteryProfile(
                snapshot,
                new MasteryProfile(),
                skillRegistry);
            bool canRestoreMentors = snapshotService.RestoreMentorRelationshipProfile(
                snapshot,
                new MentorRelationshipProfile(),
                mentorRegistry);
            bool skillTreeProgressValid = validationService.IsValidSkillTreeProgressForRestore(
                snapshot.SkillTreeProgress,
                skillTree,
                restoreClassSkillTreeBindings ? classSkillTreeBindingRegistry : null);
            bool canRestoreTreeProgress = skillTreeProgressValid && (restoreClassSkillTreeBindings
                && snapshot.SkillTreeProgress.Scopes.Count > 0
                    ? snapshotService.RestoreSkillTreeProgressProfile(
                        snapshot,
                        new SkillTreeProgressProfile(),
                        classSkillTreeBindingRegistry)
                    : snapshotService.RestoreSkillTreeProgressProfile(
                        snapshot,
                        new SkillTreeProgressProfile(),
                        skillTree));
            bool canRestoreBindings = !restoreClassSkillTreeBindings
                || snapshotService.RestoreClassSkillTreeBindings(
                    snapshot,
                    preflightBindingRegistry,
                    classRegistry);

            if (!canRestoreClass || !canRestoreMastery || !canRestoreMentors
                || !canRestoreTreeProgress || !canRestoreBindings)
            {
                return BuildResult(canRestoreClass, canRestoreMastery, canRestoreMentors,
                    canRestoreTreeProgress, canRestoreBindings, false, restoreClassSkillTreeBindings);
            }

            bool classRestored = snapshotService.RestoreClassProfile(snapshot, classProfile, classRegistry);
            bool masteryRestored = snapshotService.RestoreMasteryProfile(snapshot, masteryProfile, skillRegistry);
            bool mentorRelationshipsRestored = snapshotService.RestoreMentorRelationshipProfile(
                snapshot,
                mentorRelationshipProfile,
                mentorRegistry);

            bool skillTreeProgressRestored;
            if (restoreClassSkillTreeBindings
                && snapshot.SkillTreeProgress.Scopes.Count > 0)
            {
                skillTreeProgressRestored = snapshotService.RestoreSkillTreeProgressProfile(
                    snapshot,
                    skillTreeProgressProfile,
                    classSkillTreeBindingRegistry);
            }
            else
            {
                skillTreeProgressRestored = snapshotService.RestoreSkillTreeProgressProfile(
                    snapshot,
                    skillTreeProgressProfile,
                    skillTree);
            }

            bool classSkillTreeBindingsRestored = false;
            if (restoreClassSkillTreeBindings)
            {
                classSkillTreeBindingsRestored = snapshotService.RestoreClassSkillTreeBindings(
                    snapshot,
                    classSkillTreeBindingRegistry,
                    classRegistry);
            }

            return BuildResult(classRestored, masteryRestored, mentorRelationshipsRestored,
                skillTreeProgressRestored, classSkillTreeBindingsRestored, true, restoreClassSkillTreeBindings);
        }

        private static ClassSkillTreeBindingRegistry BuildPreflightBindingRegistry(
            ClassSkillTreeBindingRegistry sourceRegistry,
            ClassDefinitionRegistry classRegistry)
        {
            var preflightRegistry = new ClassSkillTreeBindingRegistry();

            foreach (var skillTree in sourceRegistry.SkillTrees.Values)
            {
                preflightRegistry.RegisterSkillTree(skillTree);
            }

            foreach (var binding in sourceRegistry.Bindings.Values)
            {
                preflightRegistry.RegisterBinding(binding, classRegistry);
            }

            return preflightRegistry;
        }

        private static V4ProgressRestoreResult BuildResult(
            bool classRestored,
            bool masteryRestored,
            bool mentorRelationshipsRestored,
            bool skillTreeProgressRestored,
            bool classSkillTreeBindingsRestored,
            bool applicationAttempted,
            bool bindingsRequested)
        {
            // Preflight outcomes classify feasibility only; none are applied-state evidence.
            return new V4ProgressRestoreResult(
                applicationAttempted && classRestored,
                applicationAttempted && masteryRestored,
                applicationAttempted && mentorRelationshipsRestored,
                applicationAttempted && skillTreeProgressRestored,
                applicationAttempted && bindingsRequested && classSkillTreeBindingsRestored,
                BuildReason(classRestored, applicationAttempted),
                BuildReason(masteryRestored, applicationAttempted),
                BuildReason(mentorRelationshipsRestored, applicationAttempted),
                BuildReason(skillTreeProgressRestored, applicationAttempted),
                bindingsRequested ? BuildReason(classSkillTreeBindingsRestored, applicationAttempted)
                    : V4ProgressRestoreFailureReason.None,
                BuildMessage(classRestored, applicationAttempted, "ClassProfile"),
                BuildMessage(masteryRestored, applicationAttempted, "MasteryProfile"),
                BuildMessage(mentorRelationshipsRestored, applicationAttempted, "MentorRelationshipProfile"),
                BuildMessage(skillTreeProgressRestored, applicationAttempted, "SkillTreeProgressProfile"),
                bindingsRequested ? BuildMessage(classSkillTreeBindingsRestored, applicationAttempted,
                    "ClassSkillTreeBindings") : "",
                bindingsRequested);
        }

        private static V4ProgressRestoreFailureReason BuildReason(bool succeeded, bool applicationAttempted)
        {
            if (!applicationAttempted)
            {
                return succeeded ? V4ProgressRestoreFailureReason.NotApplied
                    : V4ProgressRestoreFailureReason.ValidationFailed;
            }

            return succeeded ? V4ProgressRestoreFailureReason.None
                : V4ProgressRestoreFailureReason.ApplicationFailed;
        }

        private static string BuildMessage(
            bool succeeded,
            bool applicationAttempted,
            string subsystemName)
        {
            if (!applicationAttempted)
            {
                return succeeded
                    ? subsystemName + " was not applied because another subsystem failed preflight."
                    : subsystemName + " failed registry or snapshot validation during preflight.";
            }

            return succeeded ? "" : subsystemName + " failed during application to the real destination; partial changes may remain.";
        }
    }
}
