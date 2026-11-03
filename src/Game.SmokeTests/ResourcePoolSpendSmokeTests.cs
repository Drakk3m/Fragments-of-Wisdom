using System;
using Game.Domain.Classes;
using Game.Domain.Codex;
using Game.Domain.Common;
using Game.Domain.Combat;
using Game.Domain.Combat.Resources;
using Game.Domain.Skills;

namespace Game.SmokeTests
{
    public static partial class Program
    {
        private static void RunResourcePoolSpendSmokeTests()
        {
            AssertNegativeResourceSpendRejected();
            AssertResourceRegenerationCannotOverflow();
            AssertResourceConstructionAndSpendingMatrix();
            AssertResourceRepeatedSpending();
            AssertResourceRegenerationBounds();
            AssertResonanceSpendDelegation();
            AssertResourceSkillCostCompatibility();
            AssertResourceSpendRefillWorkflow();
        }

        private static void AssertNegativeResourceSpendRejected()
        {
            var pool = new ResourcePool(100);
            bool accepted = pool.TrySpend(-10);
            Console.WriteLine("  AUD-11 TrySpend(-10): accepted={0}, Current={1}, Max={2}",
                accepted, pool.Current, pool.Max);
            Assert(!accepted && pool.Current == 100 && pool.Max == 100,
                "AUD-11 negative spending must fail without increasing Current above Max.");
        }

        private static void AssertResourceRegenerationCannotOverflow()
        {
            var pool = new ResourcePool(100);
            pool.TrySpend(50);
            pool.Regenerate(int.MaxValue);
            Console.WriteLine("  AUD-11 invariant probe Regenerate(int.MaxValue) at 50: Current={0}, Max={1}",
                pool.Current, pool.Max);
            Assert(pool.Current == 100 && pool.Max == 100,
                "AUD-11 pool invariant requires regeneration to cap safely before narrowing.");
        }

        private static void AssertResourceConstructionAndSpendingMatrix()
        {
            foreach (int maximum in new[] { int.MinValue, -100, -1, 0, 1, 100, int.MaxValue })
            {
                var pool = new ResourcePool(maximum);
                int expectedMax = maximum < 0 ? 0 : maximum;
                Assert(pool.Max == expectedMax && pool.Current == expectedMax,
                    "AUD-11 constructor retains nonnegative maximum and starts full.");
                AssertResourcePoolBounds(pool);
                AssertAud11Spend(pool, 0, true, expectedMax);
                foreach (int negative in new[] { -1, -10, int.MinValue })
                    AssertAud11Spend(pool, negative, false, expectedMax);
            }

            // Initial current, request, expected acceptance (0/1), final current.
            int[][] cases =
            {
                new[] { 100, 20, 1, 80 },
                new[] { 100, 100, 1, 0 },
                new[] { 100, 101, 0, 100 },
                new[] { 20, 21, 0, 20 },
                new[] { 20, 20, 1, 0 },
                new[] { 0, 1, 0, 0 },
                new[] { 100, int.MaxValue, 0, 100 },
                new[] { 20, -1, 0, 20 },
                new[] { 20, -10, 0, 20 },
                new[] { 20, int.MinValue, 0, 20 },
                new[] { 0, -1, 0, 0 },
                new[] { 0, int.MinValue, 0, 0 },
                new[] { 20, 0, 1, 20 },
                new[] { 0, 0, 1, 0 }
            };
            foreach (var row in cases)
            {
                var pool = new ResourcePool(100);
                pool.TrySpend(100 - row[0]);
                AssertAud11Spend(pool, row[1], row[2] == 1, row[3]);
            }
            var extreme = new ResourcePool(int.MaxValue);
            AssertAud11Spend(extreme, int.MaxValue, true, 0);
            AssertAud11Spend(extreme, 1, false, 0);
            var nearExtreme = new ResourcePool(int.MaxValue);
            AssertAud11Spend(nearExtreme, int.MaxValue - 1, true, 1);
            AssertAud11Spend(nearExtreme, 1, true, 0);
        }

        private static void AssertAud11Spend(ResourcePool pool, int amount, bool accepted, int expectedCurrent)
        {
            int before = pool.Current;
            int max = pool.Max;
            bool actual = pool.TrySpend(amount);
            Assert(actual == accepted && pool.Current == expectedCurrent && pool.Max == max,
                "AUD-11 spend result/balance/maximum: " + before + " spend " + amount);
            Assert(actual ? pool.Current <= before : pool.Current == before,
                "AUD-11 successful spending never creates resource; rejection never mutates it.");
            AssertResourcePoolBounds(pool);
        }

        private static void AssertResourcePoolBounds(ResourcePool pool)
        {
            Assert(pool.Max >= 0 && pool.Current >= 0 && pool.Current <= pool.Max,
                "AUD-11 canonical pool invariant is 0 <= Current <= Max, Max >= 0.");
        }

        private static void AssertResourceRepeatedSpending()
        {
            var pool = new ResourcePool(100);
            AssertAud11Spend(pool, 20, true, 80);
            AssertAud11Spend(pool, -10, false, 80);
            AssertAud11Spend(pool, 30, true, 50);
            AssertAud11Spend(pool, 51, false, 50);
            AssertAud11Spend(pool, 50, true, 0);
            AssertAud11Spend(pool, 1, false, 0);
            AssertAud11Spend(pool, 0, true, 0);
            pool.Regenerate(25);
            Assert(pool.Current == 25 && pool.Max == 100, "AUD-11 ordinary regeneration still restores spent resource.");
            AssertAud11Spend(pool, 25, true, 0);
        }

        private static void AssertResourceRegenerationBounds()
        {
            foreach (int maximum in new[] { 0, 100, int.MaxValue })
            {
                foreach (int initial in new[] { 0, maximum / 2, maximum })
                {
                    foreach (int amount in new[] { int.MinValue, -1, 0, 5, int.MaxValue })
                    {
                        var pool = new ResourcePool(maximum);
                        pool.TrySpend(maximum - initial);
                        // Remaining-capacity oracle avoids repeating the production widened addition.
                        int expected = amount <= 0 ? initial
                            : amount >= maximum - initial ? maximum : initial + amount;
                        pool.Regenerate(amount);
                        Assert(pool.Current == expected && pool.Max == maximum && pool.Current >= initial,
                            "AUD-11 regeneration preserves legitimate/no-op behavior and caps extreme sums safely.");
                        AssertResourcePoolBounds(pool);
                    }
                }
            }
        }

        private static void AssertResonanceSpendDelegation()
        {
            var profile = new ResonanceProfile(new ActiveResonance(100), new ResonanceReserve(200));
            foreach (int amount in new[] { -1, -10, int.MinValue })
            {
                Assert(!profile.Active.TrySpend(amount) && !profile.TrySpendActive(amount)
                    && profile.Active.Current == 100 && profile.Active.Max == 100,
                    "AUD-11 active Resonance spending inherits invalid-input rejection.");
                Assert(!profile.Reserve.TryConsume(amount) && profile.Reserve.Current == 200 && profile.Reserve.Max == 200,
                    "AUD-11 reserve consumption inherits invalid-input rejection.");
            }
            Assert(profile.TrySpendActive(0) && profile.Reserve.TryConsume(0)
                && profile.Active.Current == 100 && profile.Reserve.Current == 200,
                "AUD-11 active/reserve zero spending remains a successful no-op.");
            Assert(profile.TrySpendActive(40) && profile.Active.Current == 60,
                "AUD-11 active Resonance still spends valid amounts.");
            Assert(profile.TryRefillFromReserve(50) && profile.Active.Current == 100 && profile.Reserve.Current == 160,
                "AUD-11 refill transfers only missing active capacity from reserve.");
            Assert(profile.TrySpendActive(100) && profile.Reserve.TryConsume(160),
                "AUD-11 active and reserve pools can be depleted exactly.");
            Assert(!profile.TryRefillFromReserve(1) && profile.Active.Current == 0 && profile.Reserve.Current == 0,
                "AUD-11 insufficient reserve rejection leaves both pools unchanged.");
            profile.Active.Refill(int.MaxValue);
            profile.Active.Refill(int.MaxValue);
            Assert(profile.Active.Current == 100 && profile.Active.Max == 100,
                "AUD-11 direct active refill delegates to overflow-safe capped regeneration.");
        }

        private static void AssertResourceSkillCostCompatibility()
        {
            // Essence cost, available Essence, expected applied (0/1), final Essence after end-turn regen.
            int[][] cases =
            {
                new[] { 10, 20, 1, 15 },
                new[] { 0, 0, 1, 5 },
                new[] { 21, 20, 0, 20 },
                new[] { 10, 5, 0, 10 },
                new[] { -10, 10, 0, 15 }
            };
            foreach (var row in cases)
            {
                var player = BuildCombatEntity("aud11_player", 10, 5, 100, 0, 0, 0, 1f, 20);
                var enemy = BuildCombatEntity("aud11_enemy", 10, 5, 100, 0, 0, 0, 1f, 20);
                player.Resource.TrySpend(20 - row[1]);
                var resonance = new ResonanceProfile(new ActiveResonance(10), new ResonanceReserve(50));
                var runtime = BuildCombatRuntime(player, enemy, resonance);
                var skill = new TrackingCombatSkill("skill_aud11");
                var definition = new SkillDefinition(skill.Id, "Resource audit skill", 3, new SkillCost(row[0], 0),
                    SkillCategory.Combat, SkillTargeting.SingleEnemy, SkillEffectType.Damage, "", 2);
                var progress = new SkillProgress(definition);
                var skills = new SkillDefinitionRegistry();
                skills.Register(definition);
                var classes = new ClassDefinitionRegistry();
                classes.Register(new ClassDefinition("class_aud11", "Resource audit class", new[] { skill.Id }));
                var classProfile = new ClassProfile();
                classProfile.AssignClassId("class_aud11", classes);
                var gateway = new CombatSkillTurnValidatorGateway(new UnlockProfile(), classProfile,
                    new MasteryProfile(), classes, skills);
                var engine = new TurnBasedCombatEngine(new FixedRandomSource(0.0, 0.99), BuildEmptyPassivePipeline(), gateway);
                var request = TurnRequest.SkillAction(player, enemy, skill, progress);
                Assert(gateway.Validate(request, runtime).IsAllowed,
                    "AUD-11 resource validation remains distinct from canonical skill authorization.");
                engine.ExecuteTurn(runtime, request);
                bool applied = row[2] == 1;
                Assert(skill.Applied == applied && progress.Experience == (applied ? 10 : 0)
                    && enemy.Stats.CurrentHealth == (applied ? 94 : 100),
                    "AUD-11 only paid valid/zero-cost skills execute effects, damage and XP.");
                Assert(player.Resource.Current == row[3] && player.Resource.Max == 20
                    && enemy.Resource.Current == 20 && resonance.Active.Current == (applied ? 8 : 10)
                    && resonance.Reserve.Current == 50,
                    "AUD-11 skill spending preserves exact balances including existing end-turn regeneration.");
                Assert(CountEvents(runtime.EventLog, GameEventType.ResourceSpent) == (applied ? 2 : 0)
                    && CountEvents(runtime.EventLog, GameEventType.ResourceNotEnough) == (applied ? 0 : 1)
                    && CountEvents(runtime.EventLog, GameEventType.ActionBlocked) == 0,
                    "AUD-11 retains existing spending/failure event contracts without new event types.");
                Assert(runtime.TurnNumber == 2 && CountEvents(runtime.EventLog, GameEventType.TurnEnded) == 1,
                    "AUD-11 skill rejection preserves blocked-turn behavior.");
                AssertResourcePoolBounds(player.Resource);
            }
        }

        private static void AssertResourceSpendRefillWorkflow()
        {
            var fixture = new ResonanceOwnershipFixture(10, 30, 2);
            fixture.Engine.ExecuteTurn(fixture.Runtime, fixture.SkillRequest(true));
            Assert(fixture.Player.Resource.Current == 15 && fixture.Resonance.Active.Current == 20
                && fixture.Progress.Experience == 10 && fixture.Runtime.TurnNumber == 2,
                "AUD-11 workflow begins with valid player Essence/Resonance spending.");
            Assert(!fixture.Player.Resource.TrySpend(-10) && !fixture.Resonance.TrySpendActive(int.MinValue)
                && !fixture.Resonance.Reserve.TryConsume(-1) && fixture.Player.Resource.Current == 15
                && fixture.Resonance.Active.Current == 20 && fixture.Resonance.Reserve.Current == 50,
                "AUD-11 rejected spends cannot manufacture either actor Essence or player Resonance.");
            var enemySkill = new TrackingCombatSkill(fixture.Skill.Id);
            var enemyProgress = new SkillProgress(fixture.Progress.Definition);
            fixture.Engine.ExecuteTurn(fixture.Runtime, TurnRequest.SkillAction(
                fixture.Enemy, fixture.Player, enemySkill, enemyProgress));
            Assert(!enemySkill.Applied && enemyProgress.Experience == 0 && fixture.Resonance.Active.Current == 20
                && fixture.Enemy.Resource.Current == 20 && fixture.Runtime.TurnNumber == 3
                && FindEventMessage(fixture.Runtime.EventLog, GameEventType.ActionBlocked).Contains(
                    CombatSkillUseBlockReason.UnsupportedResonanceOwnership.ToString()),
                "AUD-11 preserves AUD-05 player-only Resonance ownership.");
            var freeEnemyProgress = new SkillProgress(new SkillDefinition(enemySkill.Id, "Free enemy skill", 3,
                new SkillCost(0, 0)));
            fixture.Engine.ExecuteTurn(fixture.Runtime, TurnRequest.SkillAction(
                fixture.Enemy, fixture.Player, enemySkill, freeEnemyProgress));
            Assert(enemySkill.Applied && freeEnemyProgress.Experience == 10 && fixture.Player.Stats.CurrentHealth == 94
                && fixture.Enemy.Resource.Current == 20 && fixture.Resonance.Active.Current == 20
                && fixture.Runtime.TurnNumber == 4,
                "AUD-11 zero Essence and zero Resonance skill remains executable without player pool spending.");
            bool refilled = new ResonanceRefillAction().Execute(fixture.Resonance, fixture.Runtime, 7);
            Assert(refilled && fixture.Resonance.Active.Current == 27 && fixture.Resonance.Reserve.Current == 43
                && fixture.Runtime.TurnNumber == 5 && fixture.Player.Resource.Current == 15,
                "AUD-11 existing explicit refill moves seven reserve points and consumes its usual turn.");
        }
    }
}
