namespace Game.Domain.Combat.StatusEffects
{
    public interface IDebuff
    {
        string Name { get; }
        DebuffType Type { get; }
        int RemainingTurns { get; }
        bool IsExpired { get; }

        void OnTurnStart(
            CombatEntity target,
            CombatContext context);

        void OnBeforeAction(CombatContext context);

        void Tick();
    }
}
