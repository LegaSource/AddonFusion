using System;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace AddonFusion.Behaviours
{
    internal class AddonProp : PhysicsProp
    {
        protected virtual Type AddonType { get; }
        protected virtual string ToolTip { get; }

        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            base.ItemActivate(used, buttonDown);
            if (buttonDown && playerHeldBy != null && Physics.Raycast(new Ray(playerHeldBy.gameplayCamera.transform.position, playerHeldBy.gameplayCamera.transform.forward), out RaycastHit hit, 3f, 832))
            {
                var item = hit.transform.GetComponent(AddonType);
                if (item != null)
                {
                    SetAddonServerRpc(item.GetComponent<NetworkObject>());
                }
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetAddonServerRpc(NetworkObjectReference itemObject)
        {
            SetAddonClientRpc(itemObject);
        }

        [ClientRpc]
        private void SetAddonClientRpc(NetworkObjectReference itemObject)
        {
            if (itemObject.TryGet(out NetworkObject networkObject))
            {
                var item = networkObject.gameObject.GetComponentInChildren(AddonType);
                if (item != null)
                {
                    Addon addon = item.GetComponent<Addon>() ?? item.gameObject.AddComponent<Addon>();
                    if (!addon.hasAddon)
                    {
                        addon.hasAddon = true;
                        addon.addonName = itemProperties.itemName;
                        GrabbableObject grabbableObject = item.GetComponent<GrabbableObject>();
                        if (grabbableObject != null && !string.IsNullOrEmpty(ToolTip))
                        {
                            grabbableObject.itemProperties.toolTips = grabbableObject.itemProperties.toolTips.Concat(new[] { ToolTip }).ToArray();
                        }
                        DestroyObjectInHand(playerHeldBy);
                    }
                    else if (IsOwner)
                    {
                        HUDManager.Instance.DisplayTip("Impossible action", "This addon is already installed on this item");
                    }
                }
            }
        }
    }
}
