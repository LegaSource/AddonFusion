using HarmonyLib;
using System.Linq;

namespace AddonFusion.Patches
{
    internal class GrabbableObjectPatch
    {
        [HarmonyPatch(typeof(GrabbableObject), nameof(GrabbableObject.SetScrapValue))]
        [HarmonyPostfix]
        private static void FixItemNodeSubText(ref GrabbableObject __instance)
        {
            GrabbableObject item = __instance;
            string subText = AddonFusion.customItems.Where(i => i.Item == item.itemProperties).Select(i => i.Item.spawnPrefab.GetComponentInChildren<ScanNodeProperties>()?.subText).FirstOrDefault();
            if (!string.IsNullOrEmpty(subText))
            {
                ScanNodeProperties componentInChildren = __instance.gameObject.GetComponentInChildren<ScanNodeProperties>();
                if (componentInChildren != null)
                {
                    componentInChildren.subText = subText;
                }
            }
        }
    }
}
