using AddonFusion.Behaviours.AddonComponents;
using LegaFusionCore.Utilities;
using LethalCompanyInputUtils.Api;
using LethalCompanyInputUtils.BindingPathEnums;
using System.Linq;
using UnityEngine.InputSystem;

namespace AddonFusion.Behaviours.Scripts;

public class AddonInput : LcInputActions
{
    private static AddonInput instance;

    public static AddonInput Instance
    {
        get
        {
            instance ??= new AddonInput();
            return instance;
        }
        private set => instance = value;
    }

    [InputAction(KeyboardControl.Y, GamepadControl = GamepadControl.ButtonNorth, Name = "Addon Ability")]
    public InputAction AddonKey { get; set; }

    public void EnableInput() => AddonKey.performed += ActivateAddonAbility;
    public void DisableInput() => AddonKey.performed -= ActivateAddonAbility;

    public void ActivateAddonAbility(InputAction.CallbackContext context)
    {
        if (context.performed && AFUtilities.TryGetAddonComponent(LFCUtilities.LocalPlayer?.currentlyHeldObjectServer, out AddonComponent addonComponent) && !addonComponent.IsPassive)
            addonComponent.ActivateAddonAbility();
    }

    public string GetAddonToolTip() => $"Addon Ability : [{AddonKey.GetBindingDisplayString().First()}]";
}
