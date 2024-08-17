using GameNetcodeStuff;
using HarmonyLib;

namespace AddonFusion.Patches
{
    internal class ForestGiantAIPatch
    {
        [HarmonyPatch(typeof(ForestGiantAI), "BeginEatPlayer")]
        [HarmonyPrefix]
        private static bool ForestGiantGrabPlayer(ref ForestGiantAI __instance, PlayerControllerB playerBeingEaten)
        {
            PlayerControllerBPatch.AddParriedEnemy(playerBeingEaten, __instance);
            return !PlayerControllerBPatch.ParryEntity(ref playerBeingEaten, true);
        }
    }
}
