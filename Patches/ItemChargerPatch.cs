using HarmonyLib;
using Unity.Netcode;

namespace AddonFusion.Patches
{
    internal class ItemChargerPatch
    {
        [HarmonyPatch(typeof(ItemCharger), nameof(ItemCharger.ChargeItem))]
        [HarmonyPrefix]
        private static bool ExplosionChargeItem(ref ItemCharger __instance)
        {
            GrabbableObject currentlyHeldObjectServer = GameNetworkManager.Instance.localPlayerController.currentlyHeldObjectServer;
            if (currentlyHeldObjectServer != null
                && currentlyHeldObjectServer.itemProperties.requiresBattery
                && AFUtilities.GetEphemeralItem(currentlyHeldObjectServer) != null)
            {
                AddonFusionNetworkManager.Instance.ItemExplodeServerRpc(currentlyHeldObjectServer.GetComponent<NetworkObject>(), __instance.transform.position);
                return false;
            }
            return true;
        }
    }
}
