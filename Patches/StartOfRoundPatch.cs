using HarmonyLib;
using Unity.Netcode;
using UnityEngine;

namespace AddonFusion.Patches
{
    internal class StartOfRoundPatch
    {
        [HarmonyPatch(typeof(StartOfRound), "Start")]
        [HarmonyPostfix]
        private static void StartRound(StartOfRound __instance)
        {
            if (AddonFusionNetworkManager.Instance == null && NetworkManager.Singleton.IsHost)
            {
                GameObject gameObject = Object.Instantiate(AddonFusion.managerPrefab, __instance.transform.parent);
                gameObject.GetComponent<NetworkObject>().Spawn();
                AddonFusion.mls.LogInfo("Spawning AddonFusionNetworkManager");
            }
        }

        /*[HarmonyPatch(typeof(PlayerControllerB), "Update")]
        [HarmonyPostfix]
        private static void UpdatePlayer(PlayerControllerB __instance)
        {
            if (__instance != null && __instance.isPlayerControlled && __instance.playerUsername.Equals("Player #1"))
            {
                Debug.Log("Player inSpecialAnimation");
                __instance.inSpecialInteractAnimation = true;
            }
        }*/
    }
}
