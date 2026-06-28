using AddonFusion.Behaviours.AddonComponents;
using AddonFusion.Registries;
using LegaFusionCore.Managers;
using LegaFusionCore.Utilities;
using Unity.Netcode;
using UnityEngine;

namespace AddonFusion.Behaviours.Furnitures;

public class AddonCapsule : NetworkBehaviour
{
    public void RemoveAddon()
    {
        GrabbableObject grabbableObject = LFCUtilities.LocalPlayer?.currentlyHeldObjectServer;
        if (grabbableObject != null && grabbableObject.TryGetComponent(out NetworkObject networkObject) && AFUtilities.TryGetAddonComponent(grabbableObject, out AddonComponent addonComponent))
        {
            if (!addonComponent.onCooldown)
            {
                RemoveAddonEveryoneRpc(networkObject);
                SpawnAddonItemServerRpc(networkObject, LFCUtilities.LocalPlayer.transform.position);
                return;
            }
            HUDManager.Instance.DisplayTip("Information", "The addon cannot be removed while on cooldown");
        }
    }

    [Rpc(SendTo.Everyone, RequireOwnership = false)]
    public void RemoveAddonEveryoneRpc(NetworkObjectReference obj)
    {
        if (obj.TryGet(out NetworkObject networkObject) && AFUtilities.TryGetAddonComponent(networkObject.gameObject.GetComponentInChildren<GrabbableObject>(), out AddonComponent addonComponent))
            addonComponent.RemoveAddon();
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void SpawnAddonItemServerRpc(NetworkObjectReference obj, Vector3 position)
    {
        if (obj.TryGet(out NetworkObject networkObject) && AFUtilities.TryGetAddonComponent(networkObject.gameObject.GetComponentInChildren<GrabbableObject>(), out AddonComponent addonComponent) && AddonObjectRegistry.TryGetAddonObject(addonComponent.GetType(), out GameObject addonObj))
            _ = LFCObjectsManager.SpawnObjectForServer(addonObj, position);
    }
}
