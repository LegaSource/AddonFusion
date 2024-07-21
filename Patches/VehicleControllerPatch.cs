using HarmonyLib;

namespace AddonFusion.Patches
{
    internal class VehicleControllerPatch
    {
        public static bool isImmune = false;

        [HarmonyPatch(typeof(VehicleController), "DealPermanentDamage")]
        [HarmonyPrefix]
        private static bool ImmuneVehicle()
        {
            if (isImmune) return false;
            return true;
        }
    }
}
