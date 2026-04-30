using Game.Domain.Combat.StatusEffects;

namespace Game.Domain.Combat
{
    public class StatusApplicationRequest
    {
        public IDebuff Debuff { get; private set; }
        public float BaseChance { get; private set; }

        public StatusApplicationRequest(
            IDebuff debuff,
            float baseChance)
        {
            Debuff = debuff;
            BaseChance = baseChance;
        }
    }
}
