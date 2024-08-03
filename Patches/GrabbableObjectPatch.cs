using HarmonyLib;

namespace AddonFusion.Patches
{
    internal class GrabbableObjectPatch
    {
        [HarmonyPatch(typeof(GrabbableObject), nameof(GrabbableObject.Start))]
        [HarmonyPostfix]
        private static void StartPatch(ref GrabbableObject __instance)
        {
            if (!__instance.itemProperties.isScrap)
            {
                __instance.gameObject.AddComponent<Addon>();
            }
        }

        [HarmonyPatch(typeof(GrabbableObject), nameof(GrabbableObject.LateUpdate))]
        [HarmonyAfter(new string[] { "TestAccount666.ScannableTools" })]
        [HarmonyPostfix]
        private static void AddonScan(ref GrabbableObject __instance)
        {
            Addon addon = __instance.gameObject.GetComponent<Addon>();
            if (addon != null && addon.hasAddon)
            {
                ScanNodeProperties scanNode = __instance.gameObject.GetComponentInChildren<ScanNodeProperties>();
                if (scanNode != null)
                {
                    if (scanNode.subText == null) scanNode.subText = "Addon: " + addon.addonName;
                    else if (scanNode.subText != null && !scanNode.subText.Contains("Addon:")) scanNode.subText += "\nAddon: " + addon.addonName;
                }
            }
        }
    }
}
