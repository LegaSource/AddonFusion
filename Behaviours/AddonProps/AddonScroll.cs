using AddonFusion.Behaviours.AddonComponents;
using AddonFusion.Behaviours.Scripts;
using AddonFusion.Managers;
using AddonFusion.Registries;
using LegaFusionCore.Managers.NetworkManagers;
using System;
using System.Reflection;
using Unity.Netcode;
using UnityEngine;
using static AddonFusion.Behaviours.Scripts.AddonTargetDatabase;

namespace AddonFusion.Behaviours.AddonProps;

public class AddonScroll : PhysicsProp
{
    public virtual Type AddonType { get; }

    public override void ItemActivate(bool used, bool buttonDown = true)
    {
        base.ItemActivate(used, buttonDown);

        if (buttonDown
            && playerHeldBy != null
            && typeof(AddonComponent).IsAssignableFrom(AddonType)
            && Physics.Raycast(new Ray(playerHeldBy.gameplayCamera.transform.position, playerHeldBy.gameplayCamera.transform.forward), out RaycastHit hit, 3f, 832)
            && hit.transform.TryGetComponent(out GrabbableObject grabbableObject))
        {
            if (!IsValidForItem(grabbableObject))
            {
                HUDManager.Instance.DisplayTip("Impossible action", "This addon cannot be assigned to this item");
                return;
            }
            if (grabbableObject.TryGetComponent(out AddonComponent _))
            {
                HUDManager.Instance.DisplayTip("Impossible action", "This item already has an addon");
                return;
            }
            if (AddonObjectRegistry.TryGetAddonName(AddonType, out string addonName))
            {
                AddonFusionNetworkManager.Instance.SetAddonComponentEveryoneRpc(addonName, grabbableObject.GetComponent<NetworkObject>());
                LFCNetworkManager.Instance.DestroyObjectEveryoneRpc(GetComponent<NetworkObject>());
            }
        }
    }

    public bool IsValidForItem(GrabbableObject item)
    {
        if (item != null)
        {
            AddonInfoAttribute addonInfoAttribute = AddonType.GetCustomAttribute<AddonInfoAttribute>();
            AddonTargetType targetType = addonInfoAttribute?.TargetType ?? AddonTargetType.ALL;
            return targetType == AddonTargetType.ALL || (Targets.TryGetValue(targetType, out AddonTargetDefinition targetDefinition) && targetDefinition.IsValid(item));
        }
        return false;
    }
}
