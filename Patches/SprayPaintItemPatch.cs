using AddonFusion.Behaviours.AddonComponents;
using GameNetcodeStuff;
using HarmonyLib;
using LegaFusionCore.Utilities;
using UnityEngine;

namespace AddonFusion.Patches;

public class SprayPaintItemPatch
{
    [HarmonyPatch(typeof(SprayPaintItem), nameof(SprayPaintItem.AddSprayPaintLocal))]
    [HarmonyPostfix]
    public static void SprayPaintDecal(SprayPaintItem __instance, bool __result)
    {
        PlayerControllerB playerHeldBy = __instance.playerHeldBy;
        if (__result
            && playerHeldBy != null
            && AFUtilities.TryGetAddonComponent(__instance, out MarkerCompound markerCompound)
            && markerCompound.isEnabled)
        {
            int mask = 832 | 1084754248;
            foreach (RaycastHit hit in Physics.RaycastAll(new Ray(playerHeldBy.gameplayCamera.transform.position, playerHeldBy.gameplayCamera.transform.forward), 5f, mask))
            {
                if (hit.transform.TryGetComponent(out GrabbableObject grabbableObject))
                    markerCompound.AddTargetEntity(grabbableObject.gameObject, Color.yellow);
                else if (hit.transform.TryGetComponent(out PlayerControllerB player) && player != playerHeldBy && LFCUtilities.ShouldNotBeLocalPlayer(player))
                    markerCompound.AddTargetEntity(player.gameObject, Color.yellow);
                else if (hit.transform.TryGetComponent(out EnemyAICollisionDetect collision) && collision.mainScript != null)
                    markerCompound.AddTargetEntity(collision.mainScript.gameObject, Color.red);
            }
        }
    }
}
