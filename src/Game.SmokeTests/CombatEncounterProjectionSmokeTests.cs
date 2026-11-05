using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using Game.Application.Runtime;
using Game.Domain.Classes;
using Game.Domain.Common;
using Game.Domain.Combat;
using Game.Domain.Combat.Resources;
using Game.Domain.Combat.StatusEffects;
using Game.Domain.Skills;

namespace Game.SmokeTests
{
    public static partial class Program
    {
        private static void RunCombatEncounterProjectionSmokeTests()
        {
            AssertProjectionFieldMappingAndAbsence();
            AssertProjectionFreshnessAndQuietReads();
            AssertProjectionEffectsAndCollections();
            AssertProjectionLifecycle();
            AssertProjectionIntegratedGameplay();
            AssertProjectionPublicValueGraph();
        }

        private static void AssertProjectionFieldMappingAndAbsence()
        {
            var skills = new SkillDefinitionRegistry();
            var canonical = new SkillDefinition("skill_balance_ritual", "Canonical Ritual", 5, new SkillCost(9, 4),
                SkillCategory.Support, SkillTargeting.Self, SkillEffectType.Buff, "locked", 3);
            skills.Register(canonical);
            var session = new LoadedPlayerSession(new ClassDefinitionRegistry(), skills,
                new MentorDefinitionRegistry(), new ClassSkillTreeBindingRegistry());
            var f = new EncounterFixture(session);
            var player = new CombatEntity("projection.player", "Player display", f.Player.Stats.Clone(), 33);
            var enemy = new CombatEntity("projection.enemy", "Enemy display", f.Enemy.Stats.Clone(), 17);
            player.ReceiveDamage(12);
            enemy.ReceiveDamage(7);
            player.Resource.TrySpend(5);
            enemy.Resource.TrySpend(2);
            f.Resonance.TrySpendActive(6);
            f.Resonance.Reserve.TryConsume(8);
            f.Progress.GainExperience(35);
            var owner = new CombatEncounterComposition(session, f.Profile,
                new CombatEncounterPreparation(player, enemy, f.Log, f.Random, f.Passives, f.Bindings, f.Resonance));
            var before = CaptureProjectionSource(owner, f);
            var snapshot = owner.GetSnapshot();
            Assert(snapshot.State == CombatEncounterState.Prepared && snapshot.TurnNumber == 1 && !snapshot.CombatEnded,
                "P9.2 reads prepared lifecycle without starting.");
            Assert(snapshot.Player.Id == player.Id && snapshot.Player.Name == "Player display",
                "P9.2 copies player ID and name independently.");
            Assert(snapshot.Enemy.Id == enemy.Id && snapshot.Enemy.Name == "Enemy display",
                "P9.2 copies enemy ID and name independently.");
            Assert(snapshot.Player.CurrentHealth == 88 && snapshot.Player.MaxHealth == 100 && snapshot.Player.IsAlive,
                "P9.2 copies player HP and alive fact.");
            Assert(snapshot.Enemy.CurrentHealth == 93 && snapshot.Enemy.MaxHealth == 100 && snapshot.Enemy.IsAlive,
                "P9.2 copies enemy HP and alive fact.");
            Assert(snapshot.Player.Essence.Current == 28 && snapshot.Player.Essence.Max == 33,
                "P9.2 copies player Essence separately from HP.");
            Assert(snapshot.Enemy.Essence.Current == 15 && snapshot.Enemy.Essence.Max == 17,
                "P9.2 copies enemy Essence independently.");
            Assert(snapshot.HasPlayerResonance && snapshot.PlayerResonance != null,
                "P9.2 reports the actual player Resonance pool.");
            Assert(snapshot.PlayerResonance.Active.Current == 14 && snapshot.PlayerResonance.Active.Max == 20,
                "P9.2 copies active Resonance capacity and balance.");
            Assert(snapshot.PlayerResonance.Reserve.Current == 42 && snapshot.PlayerResonance.Reserve.Max == 50,
                "P9.2 copies reserve separately, not as usable active resource.");
            Assert(snapshot.Player.Debuffs.Count == 0 && snapshot.Player.StatusEffects.Count == 0
                && snapshot.Enemy.Debuffs.Count == 0 && snapshot.Enemy.StatusEffects.Count == 0,
                "P9.2 empty supported effect collections are readable.");
            Assert(snapshot.PreparedSkills.Count == 1, "P9.2 exposes only prepared skill membership.");
            var skill = snapshot.PreparedSkills[0];
            Assert(skill.SkillId == canonical.Id && skill.Name == canonical.Name && skill.Name != f.Skill.Name,
                "P9.2 uses canonical metadata, not executable display text.");
            Assert(skill.Category == SkillCategory.Support && skill.Targeting == SkillTargeting.Self,
                "P9.2 category and targeting are copied without interpreting access.");
            Assert(skill.Level == 1 && skill.Experience == 35 && skill.MaxLevel == 5 && !skill.IsSkillProgressMastered,
                "P9.2 copies retained skill progress and its distinct mastery fact.");
            Assert(skill.BaseEssenceCost == 9 && skill.EssenceCostPerLevel == 4 && skill.ResonanceCost == 3,
                "P9.2 copies canonical cost metadata, not level-adjusted or payable costs.");
            Assert(CaptureProjectionSource(owner, f) == before && f.Passive.Starts == 0,
                "P9.2 prepared read has no gameplay effects, even for locked skills.");

            var absent = new EncounterFixture();
            var absentOwner = new CombatEncounterComposition(absent.Session, absent.Profile,
                new CombatEncounterPreparation(absent.Player, absent.Enemy, absent.Log, absent.Random,
                    absent.Passives, Array.Empty<PreparedCombatSkillBinding>()));
            var absentSnapshot = absentOwner.GetSnapshot();
            Assert(!absentSnapshot.HasPlayerResonance && absentSnapshot.PlayerResonance == null,
                "P9.2 absent Resonance remains absent, not a zero-capacity resource.");
            Assert(absentSnapshot.PreparedSkills.Count == 0, "P9.2 basic-only encounter has no fabricated skills.");
            AssertProjectionCollectionImmutable(absentSnapshot.PreparedSkills);
            AssertProjectionCollectionImmutable(absentSnapshot.Player.Debuffs);
            AssertProjectionCollectionImmutable(absentSnapshot.Enemy.StatusEffects);

            var zero = new EncounterFixture();
            var zeroOwner = new CombatEncounterComposition(zero.Session, zero.Profile,
                new CombatEncounterPreparation(zero.Player, zero.Enemy, zero.Log, zero.Random,
                    zero.Passives, zero.Bindings, new ResonanceProfile(new ActiveResonance(0), new ResonanceReserve(0))));
            var zeroSnapshot = zeroOwner.GetSnapshot();
            Assert(zeroSnapshot.HasPlayerResonance && zeroSnapshot.PlayerResonance.Active.Max == 0
                && zeroSnapshot.PlayerResonance.Reserve.Max == 0,
                "P9.2 an existing zero-capacity pool is distinguishable from absence.");

            var ordered = new EncounterFixture();
            var extra = new SkillDefinition("skill_deep_sight", "Canonical Sight", 3, new SkillCost(2, 0));
            ordered.Session.RuntimeState.SkillRegistry.Register(extra);
            ordered.Bindings.Insert(0, new PreparedCombatSkillBinding(new DeepSightSkill(), new SkillProgress(extra)));
            var orderedOwner = CreateProjectionOwner(ordered);
            ordered.Bindings.Clear();
            ordered.Session.RuntimeState.SkillRegistry.Register(new SkillDefinition("unprepared", "Not bound", 1, new SkillCost(0, 0)));
            var orderedSnapshot = orderedOwner.GetSnapshot();
            Assert(orderedSnapshot.PreparedSkills.Select(s => s.SkillId).SequenceEqual(new[] { extra.Id, ordered.Skill.Id }),
                "P9.2 preserves prepared order, ignores caller-list changes and does not enumerate a full skill catalog.");
        }

        private static void AssertProjectionFreshnessAndQuietReads()
        {
            var f = new EncounterFixture();
            var owner = CreateProjectionOwner(f);
            var session = owner.Session;
            var runtime = owner.Runtime;
            var engine = owner.Engine;
            var original = owner.GetSnapshot();
            string originalValues = JsonSerializer.Serialize(original);
            string source = CaptureProjectionSource(owner, f);
            for (int i = 0; i < 3; i++)
            {
                var again = owner.GetSnapshot();
                Assert(!ReferenceEquals(original, again) && !ReferenceEquals(original.Player, again.Player)
                    && !ReferenceEquals(original.PreparedSkills[0], again.PreparedSkills[0]),
                    "P9.2 each read allocates detached values, not a cached view.");
                Assert(JsonSerializer.Serialize(again) == originalValues, "P9.2 repeated reads observe equal values.");
                Assert(CaptureProjectionSource(owner, f) == source,
                    "P9.2 repeated reads preserve RNG, XP, resources, hooks, events, profiles and turn number.");
            }
            Assert(ReferenceEquals(session, owner.Session) && ReferenceEquals(runtime, owner.Runtime)
                && ReferenceEquals(engine, owner.Engine), "P9.2 reads do not reconstruct session, runtime or engine.");

            f.Player.ReceiveDamage(13);
            f.Enemy.ReceiveDamage(8);
            f.Player.Stats.MaxHealth = 110;
            f.Player.Resource.TrySpend(9);
            f.Enemy.Resource.TrySpend(3);
            f.Resonance.TrySpendActive(7);
            f.Resonance.TryRefillFromReserve(4);
            f.Progress.GainExperience(35);
            var changed = owner.GetSnapshot();
            Assert(changed.Player.CurrentHealth == 87 && changed.Player.MaxHealth == 110
                && changed.Enemy.CurrentHealth == 92, "P9.2 fresh read reflects damage and changed HP capacity.");
            Assert(changed.Player.Essence.Current == 31 && changed.Enemy.Essence.Current == 37,
                "P9.2 fresh read reflects actual Essence spending.");
            Assert(changed.PlayerResonance.Active.Current == 17 && changed.PlayerResonance.Reserve.Current == 46,
                "P9.2 fresh read reflects real spend/refill transfer.");
            Assert(changed.PreparedSkills[0].Experience == 35 && changed.PreparedSkills[0].Level == 1,
                "P9.2 fresh read reflects retained XP.");
            Assert(JsonSerializer.Serialize(original) == originalValues, "P9.2 old entire nested snapshot stays unchanged.");
            f.Progress.GainExperience(65);
            var levelled = owner.GetSnapshot();
            Assert(levelled.PreparedSkills[0].Level == 2 && levelled.PreparedSkills[0].Experience == 0,
                "P9.2 reads existing level/XP transition without recalculating it.");
            f.Session.RuntimeState.MasteryProfile.IncreaseProgress(f.Skill.Id, 100, f.Session.RuntimeState.SkillRegistry);
            Assert(!owner.GetSnapshot().PreparedSkills[0].IsSkillProgressMastered,
                "P9.2 retention mastery is not mislabeled as XP mastery.");
            f.Progress.GainExperience(100);
            var mastered = owner.GetSnapshot();
            Assert(mastered.PreparedSkills[0].IsSkillProgressMastered && mastered.PreparedSkills[0].Level == 3,
                "P9.2 XP mastery reflects SkillProgress's established threshold.");
            Assert(mastered.PreparedSkills[0].BaseEssenceCost == 7
                && mastered.PreparedSkills[0].EssenceCostPerLevel == 0 && mastered.PreparedSkills[0].ResonanceCost == 2,
                "P9.2 no mastery discount or payable cost is calculated.");
            Assert(changed.PreparedSkills[0].Experience == 35 && !levelled.PreparedSkills[0].IsSkillProgressMastered,
                "P9.2 earlier skill snapshots retain their original values.");
        }

        private static void AssertProjectionEffectsAndCollections()
        {
            var f = new EncounterFixture();
            var owner = CreateProjectionOwner(f);
            owner.Start();
            IDebuff[] debuffs = { new PoisonDebuff(3, 4), new BurnDebuff(2, 5, 0.9f),
                new BlindDebuff(4, 0.2f), new SilenceDebuff(0), new ResourceBlockDebuff(5) };
            var context = new CombatContext(f.Player, f.Enemy, f.Log);
            foreach (var debuff in debuffs) f.Player.Debuffs.Add(debuff, f.Player, context);
            var enemyDebuff = new BlindDebuff(2, 0.1f);
            f.Enemy.Debuffs.Add(enemyDebuff, f.Enemy, context);
            var statuses = new[]
            {
                new StatusEffectInstance(StatusEffectType.Poison, 2, true, 7),
                new StatusEffectInstance(StatusEffectType.Poison, 3, true, 9),
                new StatusEffectInstance(StatusEffectType.Regen, 4, false, 5),
                new StatusEffectInstance(StatusEffectType.SkillSeal, 0, false),
                new StatusEffectInstance(StatusEffectType.Burn, 1, true, 3),
                new StatusEffectInstance(StatusEffectType.RegenBlock, 1, false)
            };
            foreach (var status in statuses) f.Player.StatusEffects.Apply(status);
            var enemyStatus = new StatusEffectInstance(StatusEffectType.Regen, 4, false, 6);
            f.Enemy.StatusEffects.Apply(enemyStatus);
            var source = CaptureProjectionSource(owner, f);
            var snapshot = owner.GetSnapshot();
            var values = JsonSerializer.Serialize(snapshot);
            Assert(snapshot.Player.Debuffs.Count == 5 && snapshot.Player.StatusEffects.Count == 6,
                "P9.2 copies separate effect collections without dropping expired entries or merging stacks.");
            for (int i = 0; i < debuffs.Length; i++)
            {
                var item = snapshot.Player.Debuffs[i];
                Assert(item.Name == debuffs[i].Name && item.Type == debuffs[i].Type
                    && item.RemainingTurns == debuffs[i].RemainingTurns && item.IsExpired == debuffs[i].IsExpired,
                    "P9.2 copies each legacy debuff's supported values in original order.");
            }
            for (int i = 0; i < statuses.Length; i++)
            {
                var item = snapshot.Player.StatusEffects[i];
                Assert(item.Type == statuses[i].Type && item.RemainingDuration == statuses[i].RemainingDuration
                    && item.Potency == statuses[i].Potency && item.AllowsStacking == statuses[i].AllowsStacking
                    && item.IsExpired == statuses[i].IsExpired,
                    "P9.2 copies each status's values, not its mutable instance.");
            }
            Assert(snapshot.Enemy.Debuffs.Single().Name == "Blind" && snapshot.Enemy.Debuffs[0].RemainingTurns == 2,
                "P9.2 enemy debuffs are not copied from the player.");
            Assert(snapshot.Enemy.StatusEffects.Single().Potency == 6 && snapshot.Enemy.StatusEffects[0].RemainingDuration == 4,
                "P9.2 enemy statuses have independent values.");
            Assert(snapshot.Player.Debuffs[3].IsExpired && snapshot.Player.StatusEffects[3].IsExpired,
                "P9.2 reads do not silently clean expired effects from source collections.");
            Assert(CaptureProjectionSource(owner, f) == source, "P9.2 effect reads never execute hooks or ticks.");

            AssertProjectionCollectionImmutable(snapshot.PreparedSkills);
            AssertProjectionCollectionImmutable(snapshot.Player.Debuffs);
            AssertProjectionCollectionImmutable(snapshot.Player.StatusEffects);
            AssertProjectionCollectionImmutable(snapshot.Enemy.Debuffs);
            AssertProjectionCollectionImmutable(snapshot.Enemy.StatusEffects);
            Assert(JsonSerializer.Serialize(snapshot) == values && CaptureProjectionSource(owner, f) == source,
                "P9.2 attempted collection mutation changes neither snapshot nor domain.");

            debuffs[0].Tick();
            f.Player.Debuffs.OnAfterAction(f.Player, context);
            statuses[0].TickDuration();
            f.Player.StatusEffects.TickDurations();
            enemyDebuff.Tick();
            enemyStatus.TickDuration();
            var fresh = owner.GetSnapshot();
            Assert(fresh.Player.Debuffs.Count == 4 && fresh.Player.Debuffs[0].RemainingTurns == 1,
                "P9.2 fresh legacy collection reflects actual tick/removal semantics.");
            Assert(fresh.Player.StatusEffects.Count == 2 && fresh.Player.StatusEffects[0].Type == StatusEffectType.Poison
                && fresh.Player.StatusEffects[0].RemainingDuration == 2 && fresh.Player.StatusEffects[0].Potency == 9,
                "P9.2 fresh status membership follows domain expiry in retained order.");
            Assert(fresh.Enemy.Debuffs[0].RemainingTurns == 1 && fresh.Enemy.StatusEffects[0].RemainingDuration == 3,
                "P9.2 fresh enemy nested values track their own mutations.");
            Assert(JsonSerializer.Serialize(snapshot) == values, "P9.2 old nested effects and membership remain detached.");
        }

        private static void AssertProjectionLifecycle()
        {
            var f = new EncounterFixture();
            var owner = CreateProjectionOwner(f);
            var prepared = owner.GetSnapshot();
            f.Passive.StartAction = context =>
            {
                var before = CaptureProjectionSource(owner, f);
                ExpectEncounterException<InvalidOperationException>(() => owner.GetSnapshot(),
                    "P9.2 rejects reentrant observation during Starting.");
                Assert(owner.State == CombatEncounterState.Starting && CaptureProjectionSource(owner, f) == before,
                    "P9.2 rejected read leaves the in-flight start untouched.");
            };
            Assert(owner.Start(), "P9.2 caught read rejection does not prevent a successful start.");
            var started = owner.GetSnapshot();
            Assert(started.State == CombatEncounterState.Started && !started.CombatEnded && f.Passive.Starts == 1,
                "P9.2 reads Started without starting again.");
            Assert(prepared.State == CombatEncounterState.Prepared, "P9.2 earlier lifecycle is detached.");
            f.Enemy.ReceiveDamage(1000);
            var dead = owner.GetSnapshot();
            Assert(dead.State == CombatEncounterState.Terminal && !dead.Enemy.IsAlive
                && dead.Enemy.CurrentHealth == 0 && !dead.CombatEnded,
                "P9.2 reports observed terminal and raw CombatEnded separately, without repairing the flag.");
            owner.Runtime.EndCombat();
            Assert(owner.GetSnapshot().CombatEnded && !dead.CombatEnded,
                "P9.2 later terminal flag changes do not alter previous observations.");

            var endedAlive = new EncounterFixture();
            var endedOwner = CreateProjectionOwner(endedAlive);
            endedOwner.Start();
            endedOwner.Runtime.EndCombat();
            var ended = endedOwner.GetSnapshot();
            Assert(ended.State == CombatEncounterState.Terminal && ended.CombatEnded
                && ended.Player.IsAlive && ended.Enemy.IsAlive,
                "P9.2 ended-alive state is copied without inventing a winner.");

            var fault = new EncounterFixture();
            var faultOwner = CreateProjectionOwner(fault);
            var initial = faultOwner.GetSnapshot();
            var error = new InvalidOperationException("projection partial-start fixture");
            fault.Passive.StartAction = context =>
            {
                context.Attacker.ReceiveDamage(11);
                context.Attacker.Resource.TrySpend(3);
                context.EventLog.Add(GameEventType.PassiveTriggered, "partial");
                throw error;
            };
            ExpectEncounterException<InvalidOperationException>(() => faultOwner.Start(), "P9.2 fixture faults through existing start contract.");
            var faultSource = CaptureProjectionSource(faultOwner, fault);
            var failed = faultOwner.GetSnapshot();
            Assert(failed.State == CombatEncounterState.Faulted && !failed.CombatEnded,
                "P9.2 partial state is explicitly Faulted, never a successful completion.");
            Assert(failed.Player.CurrentHealth == 89 && failed.Player.Essence.Current == 37,
                "P9.2 faulted snapshot reports actual partial mutations without rollback.");
            Assert(initial.Player.CurrentHealth == 100 && initial.Player.Essence.Current == 40,
                "P9.2 pre-failure snapshot stays unchanged.");
            Assert(JsonSerializer.Serialize(faultOwner.GetSnapshot()) == JsonSerializer.Serialize(failed)
                && CaptureProjectionSource(faultOwner, fault) == faultSource && ReferenceEquals(faultOwner.StartFailure, error),
                "P9.2 faulted reads neither retry hooks nor replace the original diagnostic.");
        }

        private static void AssertProjectionIntegratedGameplay()
        {
            var f = new EncounterFixture();
            var owner = CreateProjectionOwner(f);
            owner.Start();
            var original = owner.GetSnapshot();
            var request = TurnRequest.SkillAction(f.Player, f.Enemy, f.Skill, f.Progress);
            owner.Engine.ExecuteTurn(owner.Runtime, request);
            var allowed = owner.GetSnapshot();
            Assert(allowed.TurnNumber == 2 && allowed.PreparedSkills[0].Experience == 10
                && allowed.Enemy.CurrentHealth < original.Enemy.CurrentHealth,
                "P9.2 reflects the existing class-allowed engine action, not an action executed by the read API.");
            Assert(allowed.Player.Essence.Current == f.Player.Resource.Current && allowed.PlayerResonance.Active.Current == 18,
                "P9.2 observes actual post-turn costs/regeneration, never estimates them.");
            var state = f.Session.RuntimeState;
            state.ClassProfile.ChangeClassId(ClassId.Wanderer, state.ClassRegistry);
            var beforeBlocked = owner.GetSnapshot();
            owner.Engine.ExecuteTurn(owner.Runtime, request);
            var blocked = owner.GetSnapshot();
            Assert(blocked.PreparedSkills.Count == 1 && blocked.PreparedSkills[0].Experience == 10
                && blocked.Enemy.CurrentHealth == beforeBlocked.Enemy.CurrentHealth
                && blocked.PlayerResonance.Active.Current == beforeBlocked.PlayerResonance.Active.Current,
                "P9.2 prepared membership is not an availability claim; class blocking remains authoritative.");
            Assert(blocked.TurnNumber == 3 && f.Log.Events.Any(e => e.Type == GameEventType.ActionBlocked),
                "P9.2 reads preserve blocked-turn event/lifecycle semantics.");
            state.MasteryProfile.IncreaseProgress(f.Skill.Id, 100, state.SkillRegistry);
            owner.Engine.ExecuteTurn(owner.Runtime, request);
            var retained = owner.GetSnapshot();
            Assert(retained.PreparedSkills[0].Experience == 20 && !retained.PreparedSkills[0].IsSkillProgressMastered
                && retained.PlayerResonance.Active.Current == 16,
                "P9.2 retained mastery access stays in the domain, distinct from XP mastery.");
            owner.Engine.ExecuteTurn(owner.Runtime, TurnRequest.SkillAction(f.Enemy, f.Player, f.Skill, f.Progress));
            var enemyBlocked = owner.GetSnapshot();
            Assert(enemyBlocked.PlayerResonance.Active.Current == 16 && enemyBlocked.PreparedSkills[0].Experience == 20,
                "P9.2 existing unsupported enemy Resonance ownership still blocks without player spending.");
            var beforeBasic = enemyBlocked.Player.CurrentHealth;
            owner.Engine.ExecuteTurn(owner.Runtime, TurnRequest.BasicAttack(f.Enemy, f.Player));
            var afterBasic = owner.GetSnapshot();
            Assert(afterBasic.Player.CurrentHealth < beforeBasic && afterBasic.PlayerResonance.Active.Current == 16,
                "P9.2 existing enemy basic attack remains executable without player Resonance mutation.");
            var source = CaptureProjectionSource(owner, f);
            owner.GetSnapshot();
            owner.GetSnapshot();
            Assert(CaptureProjectionSource(owner, f) == source, "P9.2 post-workflow reads do not consume log events or RNG.");
            Assert(original.TurnNumber == 1 && original.PreparedSkills[0].Experience == 0
                && original.PlayerResonance.Active.Current == 20,
                "P9.2 initial projection remains detached across the integrated workflow.");
        }

        private static void AssertProjectionPublicValueGraph()
        {
            Type[] snapshots = { typeof(CombatEncounterSnapshot), typeof(CombatantSnapshot), typeof(CombatResourceSnapshot),
                typeof(CombatResonanceSnapshot), typeof(CombatDebuffSnapshot), typeof(CombatStatusEffectSnapshot), typeof(PreparedSkillSnapshot) };
            foreach (var type in snapshots)
            {
                Assert(type.IsSealed && type.GetConstructors().Length == 0, "P9.2 value types cannot be subclassed or externally constructed.");
                Assert(type.GetProperties().All(p => p.SetMethod == null && IsProjectionValueType(p.PropertyType, snapshots)),
                    "P9.2 public graph contains only getter-only scalars, enums, detached values and readonly lists.");
                Assert(type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                    .All(field => field.IsInitOnly && IsProjectionValueType(field.FieldType, snapshots)),
                    "P9.2 snapshots retain no hidden live domain references or mutable backing fields.");
                Assert(type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                    .All(method => method.IsSpecialName && method.Name.StartsWith("get_", StringComparison.Ordinal)),
                    "P9.2 snapshots expose no callbacks or gameplay methods.");
            }
        }

        private static bool IsProjectionValueType(Type type, Type[] snapshots)
        {
            return type == typeof(string) || type == typeof(int) || type == typeof(bool) || type.IsEnum
                || snapshots.Contains(type)
                || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IReadOnlyList<>)
                    && snapshots.Contains(type.GetGenericArguments()[0]));
        }

        private static void AssertProjectionCollectionImmutable<T>(IReadOnlyList<T> values)
        {
            Assert(!(values is T[]) && !(values is List<T>), "P9.2 does not expose an array or mutable list.");
            var list = values as IList<T>;
            Assert(list != null && list.IsReadOnly, "P9.2 collection rejects mutation even through IList casts.");
            ExpectEncounterException<NotSupportedException>(() => list.Add(default(T)), "P9.2 collection Add is rejected.");
            ExpectEncounterException<NotSupportedException>(() => ((IList)values).Clear(), "P9.2 nongeneric Clear is rejected.");
            if (values.Count > 0)
            {
                ExpectEncounterException<NotSupportedException>(() => list[0] = default(T), "P9.2 element replacement is rejected.");
            }
        }

        private static CombatEncounterComposition CreateProjectionOwner(EncounterFixture f)
        {
            return new CombatEncounterComposition(f.Session, f.Profile, f.Prepare());
        }

        private static string CaptureProjectionSource(CombatEncounterComposition owner, EncounterFixture f)
        {
            return JsonSerializer.Serialize(new { owner.State, Runtime = owner.Runtime, Fixture = CaptureEncounterFixture(f) });
        }
    }
}
