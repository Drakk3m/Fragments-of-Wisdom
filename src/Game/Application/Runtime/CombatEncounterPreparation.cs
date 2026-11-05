using System;
using System.Collections.Generic;
using Game.Domain.Common;
using Game.Domain.Combat;
using Game.Domain.Combat.Passives;
using Game.Domain.Combat.Resources;

namespace Game.Application.Runtime
{
    /// <summary>
    /// Single-use prepared dependencies. The caller transfers exclusive encounter use of
    /// mutable instances; copying collections does not clone their elements.
    /// </summary>
    public sealed class CombatEncounterPreparation
    {
        private bool claimed;

        internal CombatEntity Player { get; }
        internal CombatEntity Enemy { get; }
        internal GameEventLog EventLog { get; }
        internal IRandomSource Random { get; }
        internal IReadOnlyList<ICombatPassiveEffect> Passives { get; }
        internal IReadOnlyList<PreparedCombatSkillBinding> Skills { get; }
        internal ResonanceProfile Resonance { get; }

        public CombatEncounterPreparation(
            CombatEntity player,
            CombatEntity enemy,
            GameEventLog eventLog,
            IRandomSource random,
            IReadOnlyList<ICombatPassiveEffect> passives,
            IReadOnlyList<PreparedCombatSkillBinding> skills,
            ResonanceProfile resonance = null)
        {
            Player = player ?? throw new ArgumentNullException(nameof(player));
            Enemy = enemy ?? throw new ArgumentNullException(nameof(enemy));
            EventLog = eventLog ?? throw new ArgumentNullException(nameof(eventLog));
            Random = random ?? throw new ArgumentNullException(nameof(random));
            if (passives == null) throw new ArgumentNullException(nameof(passives));
            if (skills == null) throw new ArgumentNullException(nameof(skills));

            Passives = new List<ICombatPassiveEffect>(passives).AsReadOnly();
            Skills = new List<PreparedCombatSkillBinding>(skills).AsReadOnly();
            Resonance = resonance;
            Validate();
        }

        internal void Validate()
        {
            if (ReferenceEquals(Player, Enemy)
                || string.IsNullOrWhiteSpace(Player.Id)
                || string.IsNullOrWhiteSpace(Enemy.Id)
                || Player.Id == Enemy.Id
                || ReferenceEquals(Player.Stats, Enemy.Stats))
            {
                throw new ArgumentException("Prepared combatants must have distinct identities and stats.");
            }

            if (!Player.IsAlive || !Enemy.IsAlive)
            {
                throw new ArgumentException("Prepared combatants must be alive.");
            }

            if (EventLog.Events.Count != 0)
            {
                throw new ArgumentException("A prepared encounter requires a fresh event log.");
            }

            foreach (var passive in Passives)
            {
                if (passive == null) throw new ArgumentException("Prepared passives cannot contain null.");
            }

            var ids = new HashSet<string>(StringComparer.Ordinal);
            foreach (var binding in Skills)
            {
                if (binding == null || !ids.Add(binding.SkillId))
                {
                    throw new ArgumentException("Prepared skill bindings must be nonnull with unique IDs.");
                }
            }
        }

        internal void Claim()
        {
            if (claimed)
            {
                throw new InvalidOperationException("Encounter preparation already belongs to an owner.");
            }

            claimed = true;
        }
    }
}
