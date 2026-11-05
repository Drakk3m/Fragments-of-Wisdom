using System.Collections.Generic;
using Game.Domain.Combat;

namespace Game.Application.Runtime
{
    public sealed class CombatantSnapshot
    {
        public string Id { get; }
        public string Name { get; }
        public int CurrentHealth { get; }
        public int MaxHealth { get; }
        public bool IsAlive { get; }
        public CombatResourceSnapshot Essence { get; }
        public IReadOnlyList<CombatDebuffSnapshot> Debuffs { get; }
        public IReadOnlyList<CombatStatusEffectSnapshot> StatusEffects { get; }

        internal CombatantSnapshot(CombatEntity actor)
        {
            Id = actor.Id;
            Name = actor.Name;
            CurrentHealth = actor.Stats.CurrentHealth;
            MaxHealth = actor.Stats.MaxHealth;
            IsAlive = actor.IsAlive;
            Essence = new CombatResourceSnapshot(actor.Resource.Current, actor.Resource.Max);

            var debuffs = new List<CombatDebuffSnapshot>();
            foreach (var debuff in actor.Debuffs.Active) debuffs.Add(new CombatDebuffSnapshot(debuff));
            Debuffs = debuffs.AsReadOnly();

            var statusEffects = new List<CombatStatusEffectSnapshot>();
            foreach (var effect in actor.StatusEffects.Active) statusEffects.Add(new CombatStatusEffectSnapshot(effect));
            StatusEffects = statusEffects.AsReadOnly();
        }
    }
}
