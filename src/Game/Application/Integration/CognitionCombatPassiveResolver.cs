using System.Collections.Generic;
using Game.Domain.Combat.Passives;
using Game.Domain.Player;

namespace Game.Application.Integration
{
    public class CognitionCombatPassiveResolver
    {
        public IReadOnlyList<ICombatPassiveEffect> Resolve(PlayerProfile profile)
        {
            var passives = new List<ICombatPassiveEffect>();

            if (profile.Cognition.HasActiveTeaching("lyra_detect_weakness"))
            {
                passives.Add(new TrackerInstinctPassiveEffect());
            }

            if (profile.Cognition.HasActiveTeaching("atlas_stable_cast"))
            {
                passives.Add(new TechnicalResonancePassiveEffect());
            }

            return passives;
        }
    }
}
