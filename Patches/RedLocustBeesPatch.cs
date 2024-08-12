using HarmonyLib;

namespace AddonFusion.Patches
{
    internal class RedLocustBeesPatch
    {
        [HarmonyPatch(typeof(RedLocustBees), nameof(RedLocustBees.DoAIInterval))]
        [HarmonyPrefix]
        private static void DoAIInterval(ref RedLocustBees __instance)
        {
            EnemyAIPatch.PyrethrinTankBehaviour(__instance);
        }
    }
}
