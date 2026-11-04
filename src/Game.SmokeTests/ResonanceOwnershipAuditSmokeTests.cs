using Game.Domain.Classes;
using Game.Domain.Codex;
using Game.Domain.Common;
using Game.Domain.Combat;
using Game.Domain.Combat.Passives;
using Game.Domain.Combat.Resolvers;
using Game.Domain.Combat.Resources;
using Game.Domain.Combat.StatusEffects;
using Game.Domain.Skills;

namespace Game.SmokeTests
{
    public static partial class Program
    {
        private static void RunResonanceOwnershipSmokeTests()
        {
            foreach (int engineMode in new[] { 0, 1, 2 })
            {
                foreach (int? active in new int?[] { 30, 0, null })
                {
                    AssertEnemyCannotSpendPlayerResonance(engineMode, active);
                    AssertZeroResonanceEnemySkillExecutes(engineMode, active);
                }
            }

            AssertPlayerResonanceCostsRemainOwned();
            AssertEnemyEssenceStillRequired();
            AssertOwnershipBlockPreservesEndTurn();
            AssertOwnershipBasicAttacksAndForeignActor();
            AssertResonanceOwnershipIntegratedWorkflow();
        }

        private static void AssertEnemyCannotSpendPlayerResonance(int engineMode, int? active)
        {
            var fixture = new ResonanceOwnershipFixture(10, active, engineMode);
            var request = fixture.SkillRequest(false);
            if (engineMode == 2)
            {
                var validation = fixture.Gateway.Validate(request, fixture.Runtime);
                Assert(validation.IsAllowed && validation.ValidationMode == CombatSkillUseValidationMode.Enemy,
                    "AUD-05 resource ownership is enforced by the engine after enemy access validation.");
            }

            fixture.Engine.ExecuteTurn(fixture.Runtime, request);

            AssertOwnershipRejected(fixture);
            Assert(fixture.Enemy.Resource.Current == 20,
                "AUD-05 enemy rejection must not spend skill Essence.");
            AssertPlayerPoolUnchanged(fixture, active);
        }

        private static void AssertOwnershipRejected(ResonanceOwnershipFixture fixture)
        {
            Assert(CountEvents(fixture.Runtime.EventLog, GameEventType.ActionBlocked) == 1
                && FindEventMessage(fixture.Runtime.EventLog, GameEventType.ActionBlocked).Contains(
                    CombatSkillUseBlockReason.UnsupportedResonanceOwnership.ToString()),
                "AUD-05 unsupported ownership must emit a distinct stable ActionBlocked reason.");
            Assert(CountEvents(fixture.Runtime.EventLog, GameEventType.ResourceSpent) == 0
                && CountEvents(fixture.Runtime.EventLog, GameEventType.ResourceNotEnough) == 0,
                "AUD-05 ownership rejection must precede resource spending and insufficiency checks.");
            Assert(fixture.CostObserver.Calls == 0,
                "AUD-05 rejection must occur before skill cost passive hooks.");
            Assert(!fixture.Skill.Applied && fixture.Player.Stats.CurrentHealth == 100
                && CountEvents(fixture.Runtime.EventLog, GameEventType.DamageDealt) == 0
                && CountEvents(fixture.Runtime.EventLog, GameEventType.SkillUsed) == 0,
                "AUD-05 rejected enemy skill must not apply effects or damage.");
            Assert(fixture.Progress.Experience == 0 && fixture.Progress.Level == 1,
                "AUD-05 rejected enemy skill must not award XP.");
            Assert(fixture.Runtime.TurnNumber == 2
                && CountEvents(fixture.Runtime.EventLog, GameEventType.TurnEnded) == 1,
                "AUD-05 rejection must preserve established turn consumption.");
            Assert(fixture.Player.Resource.Current == 20,
                "AUD-05 enemy request must not spend player Essence.");
        }

        private static void AssertZeroResonanceEnemySkillExecutes(int engineMode, int? active)
        {
            var fixture = new ResonanceOwnershipFixture(0, active, engineMode);
            fixture.Engine.ExecuteTurn(fixture.Runtime, fixture.SkillRequest(false));

            Assert(fixture.Skill.Applied && fixture.Player.Stats.CurrentHealth == 94,
                "AUD-05 zero-Resonance enemy skill must execute existing effects.");
            Assert(fixture.Enemy.Resource.Current == 15 && fixture.Player.Resource.Current == 20
                && CountEvents(fixture.Runtime.EventLog, GameEventType.ResourceSpent) == 1
                && FindEventMessage(fixture.Runtime.EventLog, GameEventType.ResourceSpent).Contains("10 Essence"),
                "AUD-05 enemy pays its own 10 Essence, then regenerates 5; it is not made free.");
            Assert(fixture.Progress.Experience == 10 && fixture.Runtime.TurnNumber == 2
                && fixture.CostObserver.Calls == 1
                && CountEvents(fixture.Runtime.EventLog, GameEventType.ActionBlocked) == 0,
                "AUD-05 zero-Resonance enemy skill preserves cost hooks, XP and turn behavior.");
            AssertPlayerPoolUnchanged(fixture, active);
        }

        private static void AssertPlayerPoolUnchanged(ResonanceOwnershipFixture fixture, int? active)
        {
            Assert(ReferenceEquals(fixture.Runtime.ResonanceProfile, fixture.Resonance)
                && (active.HasValue
                    ? fixture.Resonance.Active.Current == active.Value && fixture.Resonance.Reserve.Current == 50
                    : fixture.Runtime.ResonanceProfile == null),
                "AUD-05 enemy actions must not mutate, replace or implicitly create player Resonance.");
        }

        private static void AssertPlayerResonanceCostsRemainOwned()
        {
            foreach (int? active in new int?[] { 30, 5, null })
            {
                var fixture = new ResonanceOwnershipFixture(10, active, 2);
                fixture.Engine.ExecuteTurn(fixture.Runtime, fixture.SkillRequest(true));
                bool enough = active == 30;
                Assert(fixture.Skill.Applied == enough
                    && fixture.Enemy.Stats.CurrentHealth == (enough ? 94 : 100)
                    && fixture.Progress.Experience == (enough ? 10 : 0),
                    "AUD-05 player skill still requires sufficient player Resonance before effects or XP.");
                Assert(fixture.Player.Resource.Current == (enough ? 15 : 20)
                    && fixture.Enemy.Resource.Current == 20,
                    "AUD-05 only the player's own Essence pays a successful player skill.");
                Assert(fixture.Runtime.TurnNumber == 2
                    && CountEvents(fixture.Runtime.EventLog, GameEventType.ResourceNotEnough) == (enough ? 0 : 1)
                    && CountEvents(fixture.Runtime.EventLog, GameEventType.ResourceSpent) == (enough ? 2 : 0)
                    && CountEvents(fixture.Runtime.EventLog, GameEventType.ActionBlocked) == 0,
                    "AUD-05 player cost success and insufficient/missing resource events remain compatible.");
                Assert(active.HasValue
                    ? fixture.Resonance.Active.Current == (enough ? 20 : 5)
                        && fixture.Resonance.Reserve.Current == 50
                    : fixture.Runtime.ResonanceProfile == null,
                    "AUD-05 player skills spend active only, never reserve or an implicit profile.");
            }

            var insufficientEssence = new ResonanceOwnershipFixture(10, 30, 2);
            insufficientEssence.Player.Resource.TrySpend(15);
            insufficientEssence.Engine.ExecuteTurn(insufficientEssence.Runtime,
                insufficientEssence.SkillRequest(true));
            Assert(!insufficientEssence.Skill.Applied && insufficientEssence.Progress.Experience == 0
                && insufficientEssence.Player.Resource.Current == 10
                && insufficientEssence.Resonance.Active.Current == 30,
                "AUD-05 insufficient player Essence must not spend Resonance; normal regeneration remains.");
        }

        private static void AssertEnemyEssenceStillRequired()
        {
            foreach (int resonanceCost in new[] { 0, 10 })
            {
                var fixture = new ResonanceOwnershipFixture(resonanceCost, 30, 2);
                fixture.Enemy.Resource.TrySpend(20);
                fixture.Engine.ExecuteTurn(fixture.Runtime, fixture.SkillRequest(false));
                Assert(!fixture.Skill.Applied && fixture.Progress.Experience == 0
                    && fixture.Player.Stats.CurrentHealth == 100 && fixture.Enemy.Resource.Current == 5,
                    "AUD-05 neither insufficient Essence nor unsupported ownership may execute the skill.");
                Assert(CountEvents(fixture.Runtime.EventLog, GameEventType.ResourceNotEnough)
                    == (resonanceCost == 0 ? 1 : 0),
                    "AUD-05 ownership must take precedence over the enemy Essence check.");
                AssertPlayerPoolUnchanged(fixture, 30);
                if (resonanceCost > 0)
                {
                    AssertOwnershipRejected(fixture);
                }
            }
        }

        private static void AssertOwnershipBlockPreservesEndTurn()
        {
            var fixture = new ResonanceOwnershipFixture(10, 30, 2);
            fixture.Enemy.Resource.TrySpend(10);
            var poison = new StatusEffectInstance(StatusEffectType.Poison, 2, false, 3);
            fixture.Enemy.StatusEffects.Apply(poison);
            fixture.Engine.ExecuteTurn(fixture.Runtime, fixture.SkillRequest(false));
            AssertOwnershipRejected(fixture);
            Assert(fixture.Enemy.Resource.Current == 15 && fixture.Enemy.Stats.CurrentHealth == 97
                && poison.RemainingDuration == 1,
                "AUD-05 blocked skill still regenerates Essence and ticks existing end-turn status effects.");
            AssertPlayerPoolUnchanged(fixture, 30);
        }

        private static void AssertOwnershipBasicAttacksAndForeignActor()
        {
            foreach (bool playerActs in new[] { true, false })
            {
                var fixture = new ResonanceOwnershipFixture(10, 30, 2);
                var attacker = playerActs ? fixture.Player : fixture.Enemy;
                var defender = playerActs ? fixture.Enemy : fixture.Player;
                fixture.Engine.ExecuteTurn(fixture.Runtime, TurnRequest.BasicAttack(attacker, defender));
                Assert(defender.Stats.CurrentHealth == 94 && fixture.Runtime.TurnNumber == 2
                    && CountEvents(fixture.Runtime.EventLog, GameEventType.BasicAttackUsed) == 1
                    && CountEvents(fixture.Runtime.EventLog, GameEventType.ActionBlocked) == 0,
                    "AUD-05 basic attacks remain unaffected for either actor.");
                Assert(fixture.Player.Resource.Current == 20 && fixture.Enemy.Resource.Current == 20
                    && fixture.Progress.Experience == 0 && fixture.CostObserver.Calls == 0,
                    "AUD-05 basic attacks do not incur skill costs or XP.");
                AssertPlayerPoolUnchanged(fixture, 30);
            }

            var legacy = new ResonanceOwnershipFixture(10, 30, 0);
            var foreign = BuildCombatEntity("aud05_foreign", 10, 5, 100, 0, 0, 0, 1f, 20);
            legacy.Engine.ExecuteTurn(legacy.Runtime,
                TurnRequest.SkillAction(foreign, legacy.Player, legacy.Skill, legacy.Progress));
            AssertOwnershipRejected(legacy);
            Assert(foreign.Resource.Current == 20,
                "AUD-05 unvalidated foreign actors cannot access the player's pool either.");
            AssertPlayerPoolUnchanged(legacy, 30);
        }

        private static void AssertResonanceOwnershipIntegratedWorkflow()
        {
            var fixture = new ResonanceOwnershipFixture(10, 30, 2);
            fixture.Engine.ExecuteTurn(fixture.Runtime, fixture.SkillRequest(true));
            Assert(fixture.Resonance.Active.Current == 20 && fixture.Progress.Experience == 10
                && fixture.Runtime.TurnNumber == 2,
                "AUD-05 workflow starts with a canonically validated player skill spending its pool.");

            var enemySkill = new TrackingCombatSkill(fixture.Skill.Id);
            var enemyProgress = new SkillProgress(fixture.Progress.Definition);
            fixture.Engine.ExecuteTurn(fixture.Runtime, TurnRequest.SkillAction(
                fixture.Enemy, fixture.Player, enemySkill, enemyProgress));
            Assert(!enemySkill.Applied && enemyProgress.Experience == 0
                && fixture.Resonance.Active.Current == 20 && fixture.Enemy.Resource.Current == 20
                && fixture.Runtime.TurnNumber == 3,
                "AUD-05 workflow rejects enemy spending without consuming the remaining player pool.");

            var zeroProgress = new SkillProgress(new SkillDefinition(enemySkill.Id, "Enemy Skill", 3,
                new SkillCost(10, 0)));
            fixture.Engine.ExecuteTurn(fixture.Runtime, TurnRequest.SkillAction(
                fixture.Enemy, fixture.Player, enemySkill, zeroProgress));
            Assert(enemySkill.Applied && zeroProgress.Experience == 10
                && fixture.Enemy.Resource.Current == 15 && fixture.Player.Stats.CurrentHealth == 94
                && fixture.Resonance.Active.Current == 20 && fixture.Resonance.Reserve.Current == 50
                && fixture.Runtime.TurnNumber == 4,
                "AUD-05 workflow allows a zero-Resonance enemy skill to pay only enemy Essence.");

            bool refilled = new ResonanceRefillAction().Execute(fixture.Resonance, fixture.Runtime, 5);
            Assert(refilled && fixture.Resonance.Active.Current == 25
                && fixture.Resonance.Reserve.Current == 45 && fixture.Runtime.TurnNumber == 5,
                "AUD-05 workflow preserves the player's existing explicit reserve-to-active refill.");
            Assert(fixture.Progress.Experience == 10 && enemyProgress.Experience == 0
                && fixture.Player.Resource.Current == 20 && fixture.Enemy.Resource.Current == 15,
                "AUD-05 ownership and XP remain unchanged; AUD-06 refill regenerates only the acting player's Essence.");
        }

        private sealed class ResonanceOwnershipFixture
        {
            public readonly CombatEntity Player = BuildCombatEntity("aud05_player", 10, 5, 100, 0, 0, 0, 1f, 20);
            public readonly CombatEntity Enemy = BuildCombatEntity("aud05_enemy", 10, 5, 100, 0, 0, 0, 1f, 20);
            public readonly ResonanceProfile Resonance;
            public readonly CombatRuntimeContext Runtime;
            public readonly SkillProgress Progress;
            public readonly TrackingCombatSkill Skill = new TrackingCombatSkill("skill_aud05");
            public readonly ResonanceCostObserver CostObserver = new ResonanceCostObserver();
            public readonly CombatSkillTurnValidatorGateway Gateway;
            public readonly TurnBasedCombatEngine Engine;

            // Modes exercise no validator, explicit legacy bypass, and progression validation.
            public ResonanceOwnershipFixture(int resonanceCost, int? active, int engineMode)
            {
                Resonance = active.HasValue
                    ? new ResonanceProfile(new ActiveResonance(active.Value), new ResonanceReserve(50))
                    : null;
                Runtime = BuildCombatRuntime(Player, Enemy, Resonance);
                var definition = new SkillDefinition(Skill.Id, "Audit Skill", 3, new SkillCost(10, 0),
                    SkillCategory.Combat, SkillTargeting.SingleEnemy, SkillEffectType.Damage, "", resonanceCost);
                Progress = new SkillProgress(definition);
                var skills = new SkillDefinitionRegistry();
                skills.Register(definition);
                var classes = new ClassDefinitionRegistry();
                classes.Register(new ClassDefinition("class_aud05", "Audit Class", new[] { Skill.Id }));
                var profile = new ClassProfile();
                profile.AssignClassId("class_aud05", classes);
                Gateway = new CombatSkillTurnValidatorGateway(new UnlockProfile(), profile,
                    new MasteryProfile(), classes, skills);
                var pipeline = new CombatPassivePipeline(new ICombatPassiveEffect[] { CostObserver });
                var random = new FixedRandomSource(0.0, 0.99, 0.0, 0.99);
                Engine = engineMode == 0
                    ? new TurnBasedCombatEngine(random, pipeline)
                    : new TurnBasedCombatEngine(random, pipeline,
                        engineMode == 1 ? new CombatSkillTurnValidatorGateway() : Gateway);
            }

            public TurnRequest SkillRequest(bool playerActs)
            {
                return TurnRequest.SkillAction(playerActs ? Player : Enemy, playerActs ? Enemy : Player,
                    Skill, Progress);
            }
        }

        private sealed class ResonanceCostObserver : ICombatPassiveEffect
        {
            public int Calls { get; private set; }
            public void OnCombatStart(CombatContext context) { }
            public void OnBeforeDebuffApplied(DebuffApplicationContext context) { }
            public void OnBeforeSkillCostCalculated(CombatContext context) { Calls++; }
        }
    }
}
