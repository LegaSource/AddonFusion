using HarmonyLib;

namespace AddonFusion.Patches
{
    internal class KnifeItemPatch
    {
        [HarmonyPatch(typeof(KnifeItem), nameof(KnifeItem.Start))]
        [HarmonyPostfix]
        private static void StartPatch(ref KnifeItem __instance)
        {
            __instance.gameObject.AddComponent<Addon>();
        }
    }
}
