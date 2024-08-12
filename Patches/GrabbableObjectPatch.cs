using AddonFusion.Behaviours;
using HarmonyLib;
using System.Linq;
using Unity.Netcode;

namespace AddonFusion.Patches
{
    internal class GrabbableObjectPatch
    {
        [HarmonyPatch(typeof(GrabbableObject), nameof(GrabbableObject.Start))]
        [HarmonyPostfix]
        private static void StartPatch(ref GrabbableObject __instance)
        {
            if (!__instance.itemProperties.isScrap && __instance.gameObject.GetComponent<Addon>() == null)
            {
                __instance.gameObject.AddComponent<Addon>();
            }
        }

        [HarmonyPatch(typeof(GrabbableObject), nameof(GrabbableObject.Update))]
        [HarmonyPostfix]
        private static void DestroyEphemeralItems(ref GrabbableObject __instance)
        {
            EphemeralItem ephemeralItem;
            if (__instance.IsOwner
                && (ephemeralItem = AFUtilities.GetEphemeralItem(__instance)) != null)
            {
                if ((__instance.itemProperties.requiresBattery && __instance.insertedBattery.empty)
                    || (ephemeralItem.maxUse > 0 && ephemeralItem.use >= ephemeralItem.maxUse))
                {
                    AddonFusionNetworkManager.Instance.DestroyObjectServerRpc(__instance.GetComponent<NetworkObject>());
                }
            }
        }

        [HarmonyPatch(typeof(GrabbableObject), nameof(GrabbableObject.LateUpdate))]
        [HarmonyAfter(["TestAccount666.ScannableTools"])]
        [HarmonyPostfix]
        private static void AddonScan(ref GrabbableObject __instance)
        {
            Addon addon = __instance.gameObject.GetComponent<Addon>();
            if (addon != null && addon.hasAddon)
            {
                ScanNodeProperties scanNode = __instance.gameObject.GetComponentInChildren<ScanNodeProperties>();
                if (scanNode != null)
                {
                    if (string.IsNullOrEmpty(scanNode.subText)) scanNode.subText = "Addon: " + addon.addonName;
                    else if (!string.IsNullOrEmpty(scanNode.subText) && !scanNode.subText.Contains("Addon:")) scanNode.subText += "\nAddon: " + addon.addonName;
                }
            }
        }

        [HarmonyPatch(typeof(GrabbableObject), nameof(GrabbableObject.SetControlTipsForItem))]
        [HarmonyPostfix]
        private static void SetAddonToolTip(ref GrabbableObject __instance)
        {
            if (__instance.gameObject.GetComponent<Addon>() != null && __instance.gameObject.GetComponent<Addon>().hasAddon && __instance.gameObject.GetComponent<Addon>().toolTip != null)
            {
                HUDManager.Instance.ChangeControlTipMultiple(__instance.itemProperties.toolTips.Concat([__instance.gameObject.GetComponent<Addon>().toolTip]).ToArray(), holdingItem: true, __instance.itemProperties);
            }
        }
    }
}
