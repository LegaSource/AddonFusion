using HarmonyLib;

namespace AddonFusion.Patches
{
    internal class RoundManagerPatch
    {
        [HarmonyPatch(typeof(RoundManager), nameof(RoundManager.DetectElevatorIsRunning))]
        [HarmonyPostfix]
        private static void EndGame()
        {
            FlashlightItemPatch.blindableEnemies.Clear();
        }
    }
}
