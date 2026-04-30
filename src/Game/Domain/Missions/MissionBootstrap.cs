namespace Game.Domain.Missions
{
    public static class MissionBootstrap
    {
        public static MissionDefinitionRegistry BuildDefinitionRegistry()
        {
            var registry = new MissionDefinitionRegistry();

            registry.Register(new MissionDefinition(
                LyraJudgementCrackEvaluator.Id,
                MissionType.MasterTraining,
                "La Grieta del Juicio",
                "Descubre la contradicciÃ³n real antes de intentar controlar al enemigo.",
                new[]
                {
                    new MissionObjectiveDefinition(
                        "objective_analyze_before_debuff",
                        MissionObjectiveType.PerformAction,
                        "AnalyzeBeforeDebuff",
                        1)
                },
                new[]
                {
                    new MissionRewardDefinition(MissionRewardType.FixTeaching, "lyra_detect_weakness")
                }));

            registry.Register(new MissionDefinition(
                AtlasThreeCertaintiesEvaluator.Id,
                MissionType.MasterTraining,
                "El Problema de las Tres Certezas",
                "Elige la opciÃ³n estable, no la mÃ¡s vistosa ni la mÃ¡s caÃ³tica.",
                new[]
                {
                    new MissionObjectiveDefinition(
                        "objective_choose_stable_action",
                        MissionObjectiveType.PerformAction,
                        "ChooseStableAction",
                        1)
                },
                new[]
                {
                    new MissionRewardDefinition(MissionRewardType.FixTeaching, "atlas_stable_cast")
                }));

            registry.Register(new MissionDefinition(
                FirstRelicSideQuestEvaluator.Id,
                MissionType.SideQuest,
                "El Relicario Partido",
                "Recupera una reliquia daÃ±ada y descubre su primera propiedad.",
                new[]
                {
                    new MissionObjectiveDefinition(
                        "objective_collect_broken_relic",
                        MissionObjectiveType.CollectItem,
                        "item_broken_reliquary",
                        1)
                },
                new[]
                {
                    new MissionRewardDefinition(MissionRewardType.GrantAccessory, "item_broken_reliquary"),
                    new MissionRewardDefinition(MissionRewardType.GrantCurrency, "gold", 100),
                    new MissionRewardDefinition(MissionRewardType.UnlockCodexEntry, "item_broken_reliquary")
                }));

            return registry;
        }

        public static MissionEvaluatorRegistry BuildEvaluatorRegistry()
        {
            var registry = new MissionEvaluatorRegistry();

            registry.Register(new LyraJudgementCrackEvaluator());
            registry.Register(new AtlasThreeCertaintiesEvaluator());
            registry.Register(new FirstRelicSideQuestEvaluator());

            return registry;
        }
    }
}
