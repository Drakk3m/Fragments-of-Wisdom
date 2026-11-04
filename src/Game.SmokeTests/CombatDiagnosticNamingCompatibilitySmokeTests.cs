using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Game.Domain.Classes;
using Game.Domain.Combat;
using Game.Domain.Common;
using Game.Domain.Skills;

namespace Game.SmokeTests
{
    public static partial class Program
    {
        private static void RunCombatDiagnosticNamingCompatibilityTests()
        {
            AssertCombatDiagnosticEnumCompatibility();
            AssertCombatDiagnosticNamedConstructors();
            AssertCombatDiagnosticRealValidationFlow();
            AssertCombatDiagnosticPrecedence();
            AssertCombatDiagnosticEngineMessages();
        }

        private static void AssertCombatDiagnosticEnumCompatibility()
        {
            string[] names = { "None", "RequestMissing", "NotSkillAction", "SkillMissing",
                "SkillProgressMissing", "SkillDefinitionMissing", "SkillIdMissing", "SkillProgressIdMismatch",
                "ValidationContextMissing", "PlayerContextIncomplete", "V3UnlockProfileMissing", "V3SkillLocked",
                "V4ContextIncomplete", "V4SkillBlocked", "SkillDefinitionUnregistered", "SkillDefinitionConflict",
                "SkillDefinitionInvalid", "UnsupportedResonanceOwnership" };
            Assert(Enum.GetNames<CombatSkillUseBlockReason>().SequenceEqual(names)
                && Enum.GetValues<CombatSkillUseBlockReason>().Select(value => (int)value)
                    .SequenceEqual(Enumerable.Range(0, names.Length)),
                "P8B: enum names/values/order remain exact, with no duplicate aliases.");
            var strings = new JsonSerializerOptions();
            strings.Converters.Add(new JsonStringEnumConverter());
            for (int index = 0; index < names.Length; index++)
            {
                var reason = (CombatSkillUseBlockReason)index;
                Assert(Enum.Parse<CombatSkillUseBlockReason>(names[index]) == reason && reason.ToString() == names[index],
                    "P8B: legacy parsing and ToString remain stable for " + names[index]);
                Assert(JsonSerializer.Serialize(reason) == index.ToString()
                    && JsonSerializer.Deserialize<CombatSkillUseBlockReason>(index.ToString()) == reason,
                    "P8B: numeric JSON round trips the original diagnostic value.");
                Assert(JsonSerializer.Serialize(reason, strings) == "\"" + names[index] + "\""
                    && JsonSerializer.Deserialize<CombatSkillUseBlockReason>("\"" + names[index] + "\"", strings) == reason,
                    "P8B: opt-in string-enum JSON preserves the original diagnostic name.");
            }
            var aliases = new[] { CombatSkillUseBlockReasons.UnlockProfileMissing, CombatSkillUseBlockReasons.UnlockSkillLocked,
                CombatSkillUseBlockReasons.ClassMasteryContextIncomplete, CombatSkillUseBlockReasons.ClassMasterySkillBlocked };
            Assert(aliases.Select(reason => (int)reason).SequenceEqual(new[] { 10, 11, 12, 13 })
                && !Enum.TryParse<CombatSkillUseBlockReason>("UnlockSkillLocked", out _),
                "P8B: canonical constants use old enum values, not new parsable enum members.");
            foreach (var reason in aliases)
            {
                int code;
                switch (reason)
                {
                    case CombatSkillUseBlockReasons.UnlockProfileMissing: code = 10; break;
                    case CombatSkillUseBlockReasons.UnlockSkillLocked: code = 11; break;
                    case CombatSkillUseBlockReasons.ClassMasteryContextIncomplete: code = 12; break;
                    case CombatSkillUseBlockReasons.ClassMasterySkillBlocked: code = 13; break;
                    default: code = -1; break;
                }
                Assert(code == (int)reason && reason is CombatSkillUseBlockReasons.UnlockProfileMissing
                    or CombatSkillUseBlockReasons.UnlockSkillLocked or CombatSkillUseBlockReasons.ClassMasteryContextIncomplete
                    or CombatSkillUseBlockReasons.ClassMasterySkillBlocked,
                    "P8B: typed constants remain usable in switch and constant patterns.");
            }
        }

        private static void AssertCombatDiagnosticNamedConstructors()
        {
            var use = new CombatSkillUseValidationResult(isAllowed: false, skillId: null, isPlayerAction: true,
                v3AccessChecked: true, v4AccessChecked: false, blockReason: CombatSkillUseBlockReason.V3UnlockProfileMissing,
                v3BlockReason: null, v4BlockReason: MasteredSkillRetentionBlockReason.None);
            Assert(use.SkillId == "" && use.UnlockBlockReason == "" && use.UnlockAccessChecked
                && !use.ClassMasteryAccessChecked, "P8B: original named arguments and null normalization remain callable.");
            AssertCombatUseDiagnosticWireShape(use);
            var turn = new CombatSkillTurnValidationResult(isAllowed: false, validationMode: null,
                failureStage: CombatSkillValidationOrchestrationFailureStage.Validation,
                classificationFailureReason: CombatActorValidationClassificationFailureReason.None,
                blockReason: CombatSkillUseBlockReason.V4SkillBlocked, v3BlockReason: null,
                v4BlockReason: MasteredSkillRetentionBlockReason.SkillNotAllowed);
            Assert(turn.UnlockBlockReason == "" && turn.ClassMasteryBlockReason == MasteredSkillRetentionBlockReason.SkillNotAllowed,
                "P8B: turn result retains named arguments and enum facts.");
            AssertCombatTurnDiagnosticWireShape(turn);
            Assert(typeof(CombatSkillUseValidationResult).GetConstructors().Single().GetParameters()
                .Select(p => p.Name).SequenceEqual(new[] { "isAllowed", "skillId", "isPlayerAction", "v3AccessChecked",
                    "v4AccessChecked", "blockReason", "v3BlockReason", "v4BlockReason" })
                && typeof(CombatSkillTurnValidationResult).GetConstructors().Single().GetParameters()
                .Select(p => p.Name).SequenceEqual(new[] { "isAllowed", "validationMode", "failureStage",
                    "classificationFailureReason", "blockReason", "v3BlockReason", "v4BlockReason" }),
                "P8B: public constructor parameter names/order remain unchanged; no ambiguous new overload.");
        }

        private static void AssertCombatDiagnosticRealValidationFlow()
        {
            var f = new CanonicalSkillFixture();
            var request = f.Request(new SkillProgress(f.Canonical));
            var validator = new CombatSkillUseValidator();
            var state = f.State;
            var context = CombatSkillUseValidationContext.Player(state.UnlockProfile, state.ClassProfile,
                state.MasteryProfile, state.ClassRegistry, state.SkillRegistry);
            var missingUnlock = validator.Validate(request, true, null, null, null, null, null);
            AssertCombatUseDiagnosticFacts(missingUnlock, CombatSkillUseBlockReasons.UnlockProfileMissing, true, false);
            Assert(missingUnlock.UnlockBlockReason == "" && !missingUnlock.IsAllowed,
                "P8B: checked means the unlock gate was reached, not policy completion or permission.");
            var locked = validator.Validate(request, context);
            AssertCombatUseDiagnosticFacts(locked, CombatSkillUseBlockReasons.UnlockSkillLocked, true, false);
            Assert(locked.UnlockBlockReason == "skill.block.missing_unlock",
                "P8B: locked-skill diagnostic text remains exact.");
            AssertCombatDiagnosticGatewayProjection(f, request);

            f.Unlock();
            AssertCombatUseDiagnosticFacts(validator.Validate(request, context), CombatSkillUseBlockReason.None, true, true);
            AssertCombatDiagnosticGatewayProjection(f, request);
            var partial = validator.Validate(request, true, state.UnlockProfile, state.ClassProfile,
                state.ClassRegistry, null, state.SkillRegistry);
            AssertCombatUseDiagnosticFacts(partial, CombatSkillUseBlockReasons.ClassMasteryContextIncomplete, true, false);
            var strictPartial = validator.Validate(request, CombatSkillUseValidationContext.Player(state.UnlockProfile,
                state.ClassProfile, null, state.ClassRegistry, state.SkillRegistry));
            AssertCombatUseDiagnosticFacts(strictPartial, CombatSkillUseBlockReason.PlayerContextIncomplete, false, false);

            state.ClassRegistry.Register(new ClassDefinition("p8b.other", "Other", Array.Empty<string>()));
            state.ClassProfile.ChangeClassId("p8b.other", state.ClassRegistry);
            var rejected = validator.Validate(request, context);
            AssertCombatUseDiagnosticFacts(rejected, CombatSkillUseBlockReasons.ClassMasterySkillBlocked, true, true);
            Assert(rejected.ClassMasteryBlockReason == MasteredSkillRetentionBlockReason.SkillNotAllowed,
                "P8B: evaluated class/mastery access can still be blocked.");
            AssertCombatDiagnosticGatewayProjection(f, request);
            state.MasteryProfile.IncreaseProgress(CanonicalSkillFixture.SkillId, 100, state.SkillRegistry);
            AssertCombatUseDiagnosticFacts(validator.Validate(request, context), CombatSkillUseBlockReason.None, true, true);
            AssertCombatDiagnosticGatewayProjection(f, request);

            var enemyRequest = TurnRequest.SkillAction(f.Enemy, f.Player, f.Skill, new SkillProgress(f.Canonical));
            var enemy = validator.Validate(enemyRequest, CombatSkillUseValidationContext.Enemy());
            var legacy = validator.Validate(request, CombatSkillUseValidationContext.LegacyBypass());
            AssertCombatUseDiagnosticFacts(enemy, CombatSkillUseBlockReason.None, false, false);
            AssertCombatUseDiagnosticFacts(legacy, CombatSkillUseBlockReason.None, false, false);
            Assert(!enemy.IsPlayerAction && legacy.IsPlayerAction,
                "P8B: Enemy and explicit LegacyBypass preserve their distinct actor facts.");
            AssertCombatDiagnosticGatewayProjection(f, enemyRequest);
            var incompleteGateway = new CombatSkillTurnValidatorGateway(false).Validate(request, f.Runtime);
            Assert(!incompleteGateway.IsAllowed && incompleteGateway.FailureStage == CombatSkillValidationOrchestrationFailureStage.Classification
                && incompleteGateway.BlockReason == CombatSkillUseBlockReason.None && incompleteGateway.UnlockBlockReason == ""
                && incompleteGateway.ClassMasteryBlockReason == MasteredSkillRetentionBlockReason.None,
                "P8B: gateway classification failure does not fabricate unlock or retention results.");
            AssertCombatTurnDiagnosticWireShape(incompleteGateway);
        }

        private static void AssertCombatDiagnosticPrecedence()
        {
            var f = new CanonicalSkillFixture();
            var s = f.State;
            var validator = new CombatSkillUseValidator();
            var context = CombatSkillUseValidationContext.Player(s.UnlockProfile, s.ClassProfile, s.MasteryProfile,
                s.ClassRegistry, s.SkillRegistry);
            AssertCombatUseDiagnosticFacts(validator.Validate(null, (CombatSkillUseValidationContext)null),
                CombatSkillUseBlockReason.ValidationContextMissing, false, false);
            AssertCombatUseDiagnosticFacts(validator.Validate(null, context), CombatSkillUseBlockReason.RequestMissing, false, false);
            var mismatch = TurnRequest.SkillAction(f.Player, f.Enemy, new TrackingCombatSkill("mismatch"), new SkillProgress(f.Canonical));
            AssertCombatUseDiagnosticFacts(validator.Validate(mismatch, CombatSkillUseValidationContext.Player(null, null, null, null, null)),
                CombatSkillUseBlockReason.SkillProgressIdMismatch, false, false);
            AssertCombatUseDiagnosticFacts(validator.Validate(f.Request(new SkillProgress(CanonicalSkillFixture.Definition(requiredUnlock: ""))), context),
                CombatSkillUseBlockReason.SkillDefinitionConflict, false, false);
            AssertCombatUseDiagnosticFacts(validator.Validate(f.Request(new SkillProgress(new SkillDefinition(
                CanonicalSkillFixture.SkillId, "invalid", 3, null))), context), CombatSkillUseBlockReason.SkillDefinitionInvalid, false, false);
            AssertCombatUseDiagnosticFacts(validator.Validate(f.Request(new SkillProgress(f.Canonical)), true,
                s.UnlockProfile, s.ClassProfile, null, null, null), CombatSkillUseBlockReasons.UnlockSkillLocked, true, false);
        }

        private static void AssertCombatDiagnosticEngineMessages()
        {
            foreach (bool retention in new[] { false, true })
            {
                var f = new CanonicalSkillFixture();
                if (retention)
                {
                    f.Unlock();
                    f.State.ClassRegistry.Register(new ClassDefinition("p8b.blocked", "Blocked", Array.Empty<string>()));
                    f.State.ClassProfile.ChangeClassId("p8b.blocked", f.State.ClassRegistry);
                }
                var progress = new SkillProgress(f.Canonical);
                f.Engine.ExecuteTurn(f.Runtime, f.Request(progress));
                Assert(FindEventMessage(f.Runtime.EventLog, GameEventType.ActionBlocked) == f.Player.Name
                    + " cannot use the skill. Validation reason: " + (retention ? "V4SkillBlocked" : "V3SkillLocked") + ".",
                    "P8B: engine-emitted diagnostic strings remain exactly historical.");
                Assert(!f.Skill.Applied && progress.Experience == 0 && f.Player.Resource.Current == 20
                    && f.Resonance.Active.Current == 6 && f.Enemy.Stats.CurrentHealth == 100 && f.Runtime.TurnNumber == 2,
                    "P8B: diagnostic naming preserves rejected-turn cost/effect/XP boundaries.");
            }
        }

        private static void AssertCombatDiagnosticGatewayProjection(CanonicalSkillFixture f, TurnRequest request)
        {
            var s = f.State;
            var source = new CombatSkillValidationOrchestrator().Validate(request, request.Attacker, f.Runtime,
                false, s.UnlockProfile, s.ClassProfile, s.MasteryProfile, s.ClassRegistry, s.SkillRegistry);
            var projected = f.Gateway.Validate(request, f.Runtime);
            Assert(projected.IsAllowed == source.IsAllowed && projected.ValidationMode == source.ValidationMode
                && projected.FailureStage == source.FailureStage && projected.ClassificationFailureReason == source.Classification.FailureReason
                && projected.BlockReason == source.Validation.BlockReason && projected.UnlockBlockReason == source.Validation.UnlockBlockReason
                && projected.ClassMasteryBlockReason == source.Validation.ClassMasteryBlockReason,
                "P8B: gateway retains every projected orchestration/validation diagnostic.");
            AssertCombatTurnDiagnosticWireShape(projected);
        }

        private static void AssertCombatUseDiagnosticFacts(CombatSkillUseValidationResult result,
            CombatSkillUseBlockReason reason, bool unlockChecked, bool classMasteryChecked)
        {
            Assert(result.BlockReason == reason && result.IsAllowed == (reason == CombatSkillUseBlockReason.None)
                && result.UnlockAccessChecked == unlockChecked && result.ClassMasteryAccessChecked == classMasteryChecked,
                "P8B: real validation retains expected decision, checked facts and precedence: " + reason);
            AssertCombatUseDiagnosticWireShape(result);
        }

        private static void AssertCombatUseDiagnosticWireShape(CombatSkillUseValidationResult result)
        {
            Assert(result.V3AccessChecked == result.UnlockAccessChecked && result.V4AccessChecked == result.ClassMasteryAccessChecked
                && result.V3BlockReason == result.UnlockBlockReason && result.V4BlockReason == result.ClassMasteryBlockReason,
                "P8B: canonical use properties expose the same legacy facts.");
            string json = JsonSerializer.Serialize(result);
            Assert(json == JsonSerializer.Serialize(new { result.IsAllowed, result.SkillId, result.IsPlayerAction,
                    result.V3AccessChecked, result.V4AccessChecked, result.BlockReason, result.V3BlockReason, result.V4BlockReason })
                && JsonSerializer.Serialize(JsonSerializer.Deserialize<CombatSkillUseValidationResult>(json)) == json,
                "P8B: use-result JSON preserves exactly the eight legacy fields and constructor round trip.");
        }

        private static void AssertCombatTurnDiagnosticWireShape(CombatSkillTurnValidationResult result)
        {
            Assert(result.V3BlockReason == result.UnlockBlockReason && result.V4BlockReason == result.ClassMasteryBlockReason,
                "P8B: canonical turn properties expose the same legacy facts.");
            string json = JsonSerializer.Serialize(result);
            Assert(json == JsonSerializer.Serialize(new { result.IsAllowed, result.ValidationMode, result.FailureStage,
                    result.ClassificationFailureReason, result.BlockReason, result.V3BlockReason, result.V4BlockReason })
                && JsonSerializer.Serialize(JsonSerializer.Deserialize<CombatSkillTurnValidationResult>(json)) == json,
                "P8B: turn-result JSON preserves exactly the seven legacy fields and constructor round trip.");
        }
    }
}
