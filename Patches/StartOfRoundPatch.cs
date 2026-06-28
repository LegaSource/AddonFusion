using AddonFusion.Behaviours.AddonComponents;
using AddonFusion.Behaviours.Scripts;
using AddonFusion.Managers;
using AddonFusion.Registries;
using GameNetcodeStuff;
using HarmonyLib;
using LegaFusionCore.Registries;
using LegaFusionCore.Utilities;
using System;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace AddonFusion.Patches;

public class StartOfRoundPatch
{
    [HarmonyPatch(typeof(StartOfRound), "Start")]
    [HarmonyBefore(["evaisa.lethallib"])]
    [HarmonyPostfix]
    public static void StartRound(StartOfRound __instance)
    {
        AddonInput.Instance.EnableInput();

        if (NetworkManager.Singleton.IsHost)
        {
            if (AddonFusionNetworkManager.Instance == null)
            {
                GameObject gameObject = UnityEngine.Object.Instantiate(AddonFusion.managerPrefab, __instance.transform.parent);
                gameObject.GetComponent<NetworkObject>().Spawn();
                AddonFusion.mls.LogInfo("Spawning AddonFusionNetworkManager");
            }
            LoadAddons(__instance);
        }
    }

    [HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.UpdatePlayerVoiceEffects))]
    [HarmonyPostfix]
    public static void SpectralFrequencyVoiceEffects()
    {
        WalkieTalkie walkieTalkie = LFCUtilities.LocalPlayer?.currentlyHeldObjectServer as WalkieTalkie;
        if (walkieTalkie != null && LFCUtilities.LocalPlayer.holdingWalkieTalkie && walkieTalkie.isBeingUsed && AFUtilities.TryGetAddonComponent(walkieTalkie, out SpectralFrequency spectralFrequency) && spectralFrequency.isEnabled)
        {
            foreach (PlayerControllerB sourcePlayer in StartOfRound.Instance.allPlayerScripts)
            {
                if (sourcePlayer.isPlayerDead && sourcePlayer != LFCUtilities.LocalPlayer)
                {
                    AudioSource currentVoiceChatAudioSource = sourcePlayer.currentVoiceChatAudioSource;
                    currentVoiceChatAudioSource.GetComponent<AudioLowPassFilter>().enabled = true;
                    currentVoiceChatAudioSource.GetComponent<AudioHighPassFilter>().enabled = true;
                    currentVoiceChatAudioSource.panStereo = 0.4f;
                    currentVoiceChatAudioSource.spatialBlend = 0f;
                    sourcePlayer.currentVoiceChatIngameSettings.set2D = true;
                    sourcePlayer.voicePlayerState.Volume = 1f;
                }
            }
        }
    }

    [HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.OnClientConnect))]
    [HarmonyPostfix]
    public static void SyncItems()
    {
        if (LFCUtilities.IsServer)
        {
            foreach (GrabbableObject grabbableObject in LFCSpawnRegistry.GetAllAs<GrabbableObject>())
            {
                if (AFUtilities.TryGetAddonComponent(grabbableObject, out AddonComponent addonComponent))
                    AddonFusionNetworkManager.Instance.SetAddonComponentEveryoneRpc(addonComponent.AddonName, grabbableObject.GetComponent<NetworkObject>());
            }
        }
    }

    public static void LoadAddons(StartOfRound startOfRound)
    {
        if (!ES3.KeyExists("shipAddonIDs", GameNetworkManager.Instance.currentSaveFileName))
        {
            AddonFusion.mls.LogWarning("Key 'shipAddonIDs' does not exist");
            return;
        }
        string[] shipAddonIDs = ES3.Load<string[]>("shipAddonIDs", GameNetworkManager.Instance.currentSaveFileName);
        if (shipAddonIDs == null || shipAddonIDs == null)
        {
            AddonFusion.mls.LogError("Ship addon list loaded from file returns a null value!");
            return;
        }
        foreach (string shipAddonID in shipAddonIDs)
        {
            string[] values = shipAddonID.Split(',');
            int itemID = int.Parse(values[0]);
            string addonName = values[1];

            if (startOfRound.allItemsList.itemsList[itemID] != null)
            {
                GrabbableObject grabbableObject = LFCSpawnRegistry.GetAllAs<GrabbableObject>().FirstOrDefault(g => g.itemProperties == startOfRound.allItemsList.itemsList[itemID]);
                if (grabbableObject != null && AddonObjectRegistry.TryGetAddonType(addonName, out Type addonType))
                    AFUtilities.SetAddonComponent(addonType, grabbableObject);
            }
        }
    }

    [HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.ShipLeave))]
    [HarmonyPostfix]
    public static void EndRound()
    {
        foreach (GrabbableObject grabbableObject in LFCSpawnRegistry.GetAllAs<GrabbableObject>())
        {
            if (AFUtilities.TryGetAddonComponent(grabbableObject, out AddonComponent addonComponent))
                addonComponent.StopCooldown();
        }
    }

    [HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.OnDisable))]
    [HarmonyPostfix]
    public static void OnDisable()
    {
        AddonInput.Instance.DisableInput();
        AddonFusionNetworkManager.Instance = null;
    }
}
