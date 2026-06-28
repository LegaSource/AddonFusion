using AddonFusion.Behaviours.AddonComponents;
using AddonFusion.Behaviours.Scripts;
using GameNetcodeStuff;
using LegaFusionCore.Utilities;
using System;

namespace AddonFusion;

public class AFUtilities
{
    public static void SetAddonComponent(Type addonType, GrabbableObject grabbableObject)
    {
        if (addonType != null && typeof(AddonComponent).IsAssignableFrom(addonType) && !TryGetAddonComponent(grabbableObject, out AddonComponent _))
        {
            AddonComponent addonComponent = grabbableObject.gameObject.AddComponent(addonType) as AddonComponent;
            addonComponent.grabbableObject = grabbableObject;

            if (grabbableObject.gameObject.TryGetComponentInChildren(out ScanNodeProperties scanNode))
                scanNode.subText += (scanNode.subText != null ? "\n" : "") + "Addon: " + addonComponent.AddonName;
        }
    }

    public static void SetAddonComponent<T>(GrabbableObject grabbableObject) where T : AddonComponent => SetAddonComponent(typeof(T), grabbableObject);

    public static bool TryGetAddonComponent<T>(PlayerControllerB player, out T addonComponent) where T : AddonComponent
    {
        addonComponent = null;
        if (player != null)
        {
            for (int i = 0; i < player.ItemSlots.Length; i++)
            {
                if (TryGetAddonComponent(player.ItemSlots[i], out addonComponent))
                    return true;
            }
        }
        return false;
    }

    public static bool TryGetAddonComponent<T>(GrabbableObject grabbableObject, out T addonComponent) where T : AddonComponent
    {
        addonComponent = grabbableObject?.GetComponent<T>();
        return addonComponent != null;
    }

    public static void SetControlTipsForAddon(GrabbableObject grabbableObject)
    {
        if (TryGetAddonComponent(grabbableObject, out AddonComponent addonComponent) && !addonComponent.IsPassive)
            addonComponent.SetTipsForItem([AddonInput.Instance.GetAddonToolTip()]);
    }
}
