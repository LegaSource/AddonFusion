using GameNetcodeStuff;
using HarmonyLib;

namespace AddonFusion.Patches
{
    internal class NutcrackerEnemyAIPatch
    {
        [HarmonyPatch(typeof(NutcrackerEnemyAI), "FireGun")]
        [HarmonyPrefix]
        private static void NutcrackerFireGun(ref NutcrackerEnemyAI __instance)
        {
            PlayerControllerB player = StartOfRound.Instance.allPlayerObjects[__instance.lastPlayerSeenMoving].GetComponent<PlayerControllerB>();
            if (player != null)
            {
                PlayerControllerBPatch.AddParriedEnemy(player, __instance);
            }
        }
    }
}
