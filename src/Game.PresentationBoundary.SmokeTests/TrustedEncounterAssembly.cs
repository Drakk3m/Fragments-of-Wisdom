using System;
using Game.Application.Bootstrap;
using Game.Application.Runtime;
using Game.Domain.Classes;
using Game.Domain.Combat;
using Game.Domain.Combat.Passives;
using Game.Domain.Combat.Resources;
using Game.Domain.Combat.Resolvers;
using Game.Domain.Combat.Stats;
using Game.Domain.Combat.StatusEffects;
using Game.Domain.Common;
using Game.Domain.Player;
using Game.Domain.Skills;

namespace Game.PresentationBoundary.SmokeTests
{
    internal enum Scenario
    {
        Normal, EnemyVictory, ClassBlocked, Unmastered, Retained,
        EarlyRefillDenial, LateRefillDenial, NoResonance, ZeroResonance, PlayerFault, EnemyFault
    }

    // Trusted composition only. No session, combatant, pool, binding or callback escapes handoff.
    internal static class TrustedEncounterAssembly
    {
        internal const string SkillId = "skill_balance_ritual";

        internal static CombatEncounterComposition Create(Scenario scenario, out Exception expectedFault)
        {
            var skills = new SkillDefinitionRegistry();
            var definition = new SkillDefinition(SkillId, "Ritual de Balance", 3, new SkillCost(7, 0),
                SkillCategory.Combat, SkillTargeting.SingleEnemy, SkillEffectType.Damage, "", 2);
            skills.Register(definition);
            var classes = new ClassDefinitionRegistry();
            classes.Register(new ClassDefinition(ClassId.Tlamatini, "Tlamatini", new[] { SkillId }));
            classes.Register(new ClassDefinition(ClassId.Wanderer, "Wanderer", Array.Empty<string>()));
            var bootstrap = new PlayerSessionBootstrap(classes, skills, new MentorDefinitionRegistry());
            bool crossClass = scenario == Scenario.ClassBlocked || scenario == Scenario.Unmastered || scenario == Scenario.Retained;
            bootstrap.RuntimeState.ClassProfile.AssignClassId(crossClass ? ClassId.Wanderer : ClassId.Tlamatini, classes);
            if (scenario == Scenario.Unmastered || scenario == Scenario.Retained)
                bootstrap.RuntimeState.MasteryProfile.IncreaseProgress(SkillId, scenario == Scenario.Retained ? 100 : 99, skills);

            var player = Actor("player", scenario == Scenario.EnemyVictory ? 15 : 100);
            var enemy = Actor("enemy", scenario == Scenario.EnemyVictory ? 100 : 30);
            var log = new GameEventLog();
            Action<CombatContext> onStart = null;
            var resonance = scenario == Scenario.NoResonance ? null : new ResonanceProfile(
                new ActiveResonance(scenario == Scenario.ZeroResonance ? 0 : 20), new ResonanceReserve(50));
            if (scenario == Scenario.EarlyRefillDenial || scenario == Scenario.LateRefillDenial)
            {
                resonance.TrySpendActive(8);
                if (scenario == Scenario.EarlyRefillDenial) resonance.Reserve.TryConsume(50);
                else onStart = start => player.Debuffs.Add(new SetupHook(context => resonance.Reserve.TryConsume(50)), player, start);
            }
            if (scenario == Scenario.Normal)
                player.StatusEffects.Apply(new StatusEffectInstance(StatusEffectType.Poison, 3, false, 1));
            expectedFault = null;
            if (scenario == Scenario.PlayerFault || scenario == Scenario.EnemyFault)
            {
                var error = new InvalidOperationException("P9.5 configured execution fault");
                expectedFault = error;
                var actor = scenario == Scenario.PlayerFault ? player : enemy;
                var target = scenario == Scenario.PlayerFault ? enemy : player;
                onStart = start => actor.Debuffs.Add(new SetupHook(context =>
                {
                    context.Attacker.ReceiveDamage(7);
                    context.Attacker.Resource.TrySpend(3);
                    context.EventLog.Add(GameEventType.PassiveTriggered, "partial mutation before fault");
                    throw error;
                }), actor, new CombatContext(actor, target, start.EventLog));
            }
            var preparation = new CombatEncounterPreparation(player, enemy, log, new ScriptedRandom(),
                new ICombatPassiveEffect[] { new StartMarker(onStart) },
                new[] { new PreparedCombatSkillBinding(new BalanceRitualSkill(), new SkillProgress(definition)) }, resonance);
            return new CombatEncounterComposition(bootstrap.LoadedSession, new PlayerProfile(), preparation);
        }

        private static CombatEntity Actor(string id, int health)
        {
            return new CombatEntity(id, id, new CombatStats
            {
                MaxHealth = health, CurrentHealth = health, Attack = 10, Defense = 5,
                Precision = 100, CriticalDamageMultiplier = 2f
            }, 40);
        }

        private sealed class ScriptedRandom : IRandomSource
        {
            private int index;
            private readonly double[] values = { 0.5, 0.6, 0.8, 0.3, 0.55, 0.7, 0.2 };
            public double NextDouble() { return values[index++ % values.Length]; }
            public int NextInt(int minInclusive, int maxExclusive) { return minInclusive + (int)(NextDouble() * (maxExclusive - minInclusive)); }
        }

        private sealed class StartMarker : ICombatPassiveEffect
        {
            private readonly Action<CombatContext> onStart;
            public StartMarker(Action<CombatContext> onStart) { this.onStart = onStart; }
            public void OnCombatStart(CombatContext context)
            {
                onStart?.Invoke(context);
                context.EventLog.Add(GameEventType.PassiveTriggered, "TurnEnded damage=999: display text, not lifecycle evidence");
            }
            public void OnBeforeSkillCostCalculated(CombatContext context) { }
            public void OnBeforeDebuffApplied(DebuffApplicationContext context) { }
        }

        private sealed class SetupHook : IDebuff
        {
            private readonly Action<CombatContext> action;
            public SetupHook(Action<CombatContext> action) { this.action = action; }
            public string Name { get { return "Trusted verification hook"; } }
            public DebuffType Type { get { return DebuffType.ActionBlock; } }
            public int RemainingTurns { get { return 10; } }
            public bool IsExpired { get { return false; } }
            public void OnTurnStart(CombatEntity target, CombatContext context) { }
            public void OnBeforeAction(CombatContext context) { action(context); }
            public void Tick() { }
        }
    }
}
