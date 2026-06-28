using AddonFusion.Behaviours.AddonComponents;
using AddonFusion.Registries;
using GameNetcodeStuff;
using LegaFusionCore.Utilities;
using System;
using Unity.Netcode;

namespace AddonFusion.Managers;

public class AddonFusionNetworkManager : NetworkBehaviour
{
    public static AddonFusionNetworkManager Instance;

    public void Awake() => Instance = this;

    [Rpc(SendTo.Everyone, RequireOwnership = false)]
    public void SetAddonComponentEveryoneRpc(string addonName, NetworkObjectReference obj)
    {
        if (obj.TryGet(out NetworkObject networkObject)
            && networkObject.gameObject.TryGetComponentInChildren(out GrabbableObject grabbableObject)
            && AddonObjectRegistry.TryGetAddonType(addonName, out Type addonType))
        {
            AFUtilities.SetAddonComponent(addonType, grabbableObject);
        }
    }

    [Rpc(SendTo.Everyone, RequireOwnership = false)]
    public void EnableAddonEveryoneRpc(NetworkObjectReference obj, bool enable)
    {
        if (obj.TryGet(out NetworkObject networkObject) && networkObject.gameObject.TryGetComponentInChildren(out GrabbableObject grabbableObject) && AFUtilities.TryGetAddonComponent(grabbableObject, out AddonComponent addonComponent))
        {
            addonComponent.isEnabled = enable;
            if (LFCUtilities.ShouldBeLocalPlayer(grabbableObject.playerHeldBy))
                HUDManager.Instance.DisplayTip("Information", enable ? "The addon ability is enabled" : "The addon ability is disabled");
        }
    }

    [Rpc(SendTo.Everyone, RequireOwnership = false)]
    public void StartAddonCooldownEveryoneRpc(NetworkObjectReference obj, int cooldown)
    {
        if (obj.TryGet(out NetworkObject networkObject) && networkObject.gameObject.TryGetComponentInChildren(out GrabbableObject grabbableObject) && AFUtilities.TryGetAddonComponent(grabbableObject, out AddonComponent addonComponent))
        {
            addonComponent.onCooldown = true;
            addonComponent.cooldownCoroutine = StartCoroutine(addonComponent.StartCooldownCoroutine(cooldown));
        }
    }

    [Rpc(SendTo.Everyone, RequireOwnership = false)]
    public void BlindPlayerEveryoneRpc(int playerId, float flashFilter)
    {
        if (LFCUtilities.ShouldBeLocalPlayer(StartOfRound.Instance.allPlayerObjects[playerId].GetComponent<PlayerControllerB>()))
            HUDManager.Instance.flashFilter = Math.Max(HUDManager.Instance.flashFilter, flashFilter);
    }
}
