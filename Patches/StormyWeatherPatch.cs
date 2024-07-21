using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace AddonFusion.Patches
{
    internal class StormyWeatherPatch
    {
        public static List<GrabbableObject> conductiveObjectsToAdd = new List<GrabbableObject>();
        public static List<GrabbableObject> conductiveObjectsToRemove = new List<GrabbableObject>();

        [HarmonyPatch(typeof(StormyWeather), "Update")]
        [HarmonyPostfix]
        private static void RemoveConductiveObjects(ref List<GrabbableObject> ___metalObjects, ref GrabbableObject ___targetingMetalObject, ref ParticleSystem ___staticElectricityParticle)
        {
            ___metalObjects.RemoveAll(obj => conductiveObjectsToRemove.Contains(obj));
            if (conductiveObjectsToRemove.Contains(___targetingMetalObject))
            {
                ___staticElectricityParticle.Stop();
                ___staticElectricityParticle.gameObject.GetComponent<AudioSource>().Stop();
                ___targetingMetalObject = null;
            }
            foreach (GrabbableObject grabbableObject in conductiveObjectsToAdd)
            {
                if (!___metalObjects.Contains(grabbableObject)) ___metalObjects.Add(grabbableObject);
            }
            conductiveObjectsToAdd.Clear();
            conductiveObjectsToRemove.Clear();
        }
    }
}
