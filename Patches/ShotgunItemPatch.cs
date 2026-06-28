using HarmonyLib;

namespace AddonFusion.Patches;

public class ShotgunItemPatch
{

    [HarmonyPatch(typeof(ShotgunItem), nameof(ShotgunItem.SetControlTipsForItem))]
    [HarmonyPostfix]
    public static void SetControlTipsForAddon(ShotgunItem __instance) => AFUtilities.SetControlTipsForAddon(__instance);
}
