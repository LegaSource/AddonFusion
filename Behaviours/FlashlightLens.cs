using Unity.Netcode;
using UnityEngine;

namespace AddonFusion.Behaviours
{
    internal class FlashlightLens : PhysicsProp
    {
        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            base.ItemActivate(used, buttonDown);
            if (buttonDown && playerHeldBy != null && Physics.Raycast(new Ray(playerHeldBy.gameplayCamera.transform.position, playerHeldBy.gameplayCamera.transform.forward), out var hit, 3f, 832))
            {
                FlashlightItem flashlight = hit.transform.GetComponent<FlashlightItem>();
                if (flashlight != null)
                {
                    SetAddonServerRpc(flashlight.NetworkObject);
                }
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetAddonServerRpc(NetworkObjectReference flashlightObject)
        {
            SetAddonClientRpc(flashlightObject);
        }

        [ClientRpc]
        private void SetAddonClientRpc(NetworkObjectReference flashlightObject)
        {
            if (flashlightObject.TryGet(out var networkObject))
            {
                FlashlightItem flashlight = networkObject.gameObject.GetComponentInChildren<FlashlightItem>();
                if (flashlight != null)
                {
                    Addon addon = flashlight.GetComponent<Addon>() ?? flashlight.gameObject.AddComponent<Addon>();
                    if (!addon.hasAddon)
                    {
                        addon.hasAddon = true;
                        addon.addonName = itemProperties.itemName;
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
