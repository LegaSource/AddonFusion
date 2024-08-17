using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace AddonFusion.Patches
{
    internal class FlowermanAIPatch
    {
        [HarmonyPatch(typeof(FlowermanAI), nameof(FlowermanAI.OnCollideWithPlayer))]
        [HarmonyPrefix]
        private static bool FlowermanKillPlayer(ref FlowermanAI __instance, Collider other)
        {
            PlayerControllerB player = __instance.MeetsStandardPlayerCollisionConditions(other, __instance.inKillAnimation || __instance.startingKillAnimationLocalClient || __instance.carryingPlayerBody);
            if (player != null)
            {
                PlayerControllerBPatch.AddParriedEnemy(player, __instance);
                return !PlayerControllerBPatch.ParryEntity(ref player, true);
            }
            return true;
        }
    }
}
