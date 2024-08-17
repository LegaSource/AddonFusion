using AddonFusion.Behaviours;
using UnityEngine;

namespace AddonFusion
{
    internal class AFUtilities
    {
        public static Addon GetAddonInstalled(Component component, string addonName)
        {
            Addon addon = component.gameObject.GetComponent<Addon>();
            if (addon != null
                && addon.hasAddon
                && !string.IsNullOrEmpty(addon.addonName)
                && addon.addonName.Equals(addonName))
            {
                return addon;
            }
            return null;
        }

        public static Addon GetAddonInstalled(Component component)
        {
            Addon addon = component.gameObject.GetComponent<Addon>();
            if (addon != null
                && addon.hasAddon)
            {
                return addon;
            }
            return null;
        }

        public static EphemeralItem GetEphemeralItem(Component component)
        {
            EphemeralItem ephemeralItem = component.gameObject.GetComponent<EphemeralItem>();
            if (ephemeralItem != null && ephemeralItem.isEphemeral)
            {
                return ephemeralItem;
            }
            return null;
        }

        public static void EnablePlayerActions(bool enable)
        {
            string[] actionNames = { "Move", "Jump", "Crouch", "Interact", "ItemSecondaryUse", "ItemTertiaryUse", "ActivateItem", "SwitchItem", "InspectItem", "Emote1", "Emote2" };

            foreach (string actionName in actionNames)
            {
                if (enable) IngamePlayerSettings.Instance.playerInput.actions.FindAction(actionName, false).Enable();
                else IngamePlayerSettings.Instance.playerInput.actions.FindAction(actionName, false).Disable();
            }
        }
    }
}
