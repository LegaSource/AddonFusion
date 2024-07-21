using GameNetcodeStuff;
using HarmonyLib;

namespace AddonFusion.Patches
{
    internal class PlayerControllerBPatch
    {
        [HarmonyPatch(typeof(PlayerControllerB), "BeginGrabObject")]
        [HarmonyPostfix]
        private static void FixVehicleParent(ref GrabbableObject ___currentlyGrabbingObject)
        {
            // Limiter le fix au véhicule au cas où le parent serait nécessaire ailleur
            if (___currentlyGrabbingObject?.transform.parent?.GetComponent<VehicleController>() != null)
            {
                ___currentlyGrabbingObject.transform.SetParent(null);
            }
        }
    }
}
