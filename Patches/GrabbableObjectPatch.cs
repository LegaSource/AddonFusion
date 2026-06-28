using AddonFusion.Behaviours.AddonComponents;
using HarmonyLib;
using LegaFusionCore.Utilities;

namespace AddonFusion.Patches;

public class GrabbableObjectPatch
{
    [HarmonyPatch(typeof(GrabbableObject), nameof(GrabbableObject.SetScrapValue))]
    [HarmonyPostfix]
    public static void SetAddonScanNode(GrabbableObject __instance)
    {
        if (AFUtilities.TryGetAddonComponent(__instance, out AddonComponent addonComponent))
        {
            string addonText = "Addon: " + addonComponent.AddonName;
            if (__instance.gameObject.TryGetComponentInChildren(out ScanNodeProperties scanNode) && (scanNode.subText == null || !scanNode.subText.Contains(addonText)))
                scanNode.subText += (scanNode.subText != null ? "\n" : "") + addonText;
        }
    }

    [HarmonyPatch(typeof(GrabbableObject), nameof(GrabbableObject.SetControlTipsForItem))]
    [HarmonyPostfix]
    public static void SetControlTipsForAddon(GrabbableObject __instance) => AFUtilities.SetControlTipsForAddon(__instance);
}
