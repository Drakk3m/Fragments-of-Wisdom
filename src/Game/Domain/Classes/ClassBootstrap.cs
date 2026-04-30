using System.Collections.Generic;

namespace Game.Domain.Classes
{
    public static class ClassBootstrap
    {
        public static ClassDefinition BuildTlamatini()
        {
            return new ClassDefinition(
                "class_tlamatini",
                "Tlamatini",
                new[]
                {
                    "skill_deep_sight",
                    "skill_balance_ritual",
                    "skill_destiny_correction"
                });
        }

        public static IReadOnlyDictionary<string, SkillTreeNodeDefinition> BuildTlamatiniSkillTree()
        {
            return new Dictionary<string, SkillTreeNodeDefinition>
            {
                {
                    "node_deep_sight",
                    new SkillTreeNodeDefinition("node_deep_sight", "skill_deep_sight", new string[0])
                },
                {
                    "node_balance_ritual",
                    new SkillTreeNodeDefinition("node_balance_ritual", "skill_balance_ritual", new[] { "node_deep_sight" })
                },
                {
                    "node_destiny_correction",
                    new SkillTreeNodeDefinition("node_destiny_correction", "skill_destiny_correction", new[] { "node_balance_ritual" })
                }
            };
        }
    }
}
