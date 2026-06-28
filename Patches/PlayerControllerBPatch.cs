using AddonFusion.Behaviours.AddonComponents;
using GameNetcodeStuff;
using HarmonyLib;
using LegaFusionCore.Managers.NetworkManagers;

namespace AddonFusion.Patches;

public class PlayerControllerBPatch
{
    [HarmonyPatch(typeof(PlayerControllerB), "IHittable.Hit")]
    [HarmonyPrefix]
    public static bool HitPlayer(PlayerControllerB __instance, PlayerControllerB playerWhoHit)
    {
        if (__instance.currentlyHeldObjectServer != null
            && AFUtilities.TryGetAddonComponent(__instance.currentlyHeldObjectServer, out ProtectiveCord protectiveCord)
            && protectiveCord.isParrying)
        {
            __instance.DiscardHeldObject();
            LFCNetworkManager.Instance.PlayAudioEveryoneRpc($"{AddonFusion.modName}{AddonFusion.parryAudio.name}", __instance.currentlyHeldObjectServer.transform.position);
            return false;
        }
        return true;
    }
}
