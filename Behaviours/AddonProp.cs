using System;
using Unity.Netcode;
using UnityEngine;

namespace AddonFusion.Behaviours
{
    internal class AddonProp : PhysicsProp
    {
        public virtual Type AddonType { get; }
        public virtual string ToolTip { get; }

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
                if (item != null && CheckSpecificItem(networkObject.gameObject))
                {
                    Addon addon = item.GetComponent<Addon>() ?? item.gameObject.AddComponent<Addon>();
                    if (!addon.hasAddon)
                    {
                        addon.hasAddon = true;
                        addon.addonName = itemProperties.itemName;
                        addon.toolTip = ToolTip;
                        DestroyObjectInHand(playerHeldBy);
                    }
                    else if (IsOwner)
                    {
                        HUDManager.Instance.DisplayTip("Impossible action", "This addon is already installed on this item");
                    }
                }
            }
        }

        public bool CheckSpecificItem(GameObject gameObject)
        {
            SprayPaintItem sprayPaintItem = gameObject.GetComponent<SprayPaintItem>();
            if (sprayPaintItem != null)
            {
                PyrethrinTank pyrethrinTank = base.gameObject.GetComponent<PyrethrinTank>();
                SaltTank saltTank = base.gameObject.GetComponent<SaltTank>();
                if (pyrethrinTank != null && sprayPaintItem.isWeedKillerSprayBottle)
                {
                    return true;
                }
                else if (saltTank != null && !sprayPaintItem.isWeedKillerSprayBottle)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
    }
}
