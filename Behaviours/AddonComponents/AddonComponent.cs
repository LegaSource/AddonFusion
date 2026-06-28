using AddonFusion.Behaviours.Scripts;
using AddonFusion.Managers;
using LegaFusionCore.Utilities;
using System.Collections;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace AddonFusion.Behaviours.AddonComponents;

public abstract class AddonComponent : MonoBehaviour
{
    public GrabbableObject grabbableObject;

    public abstract string AddonName { get; }
    public abstract bool IsPassive { get; }

    public bool isEnabled = false;
    public bool onCooldown = false;
    public Coroutine cooldownCoroutine;

    public virtual void ActivateAddonAbility() { }

    public void StartCooldown(int cooldown)
    {
        if (!onCooldown && !IsPassive && grabbableObject != null && grabbableObject.TryGetComponent(out NetworkObject networkObject))
            AddonFusionNetworkManager.Instance.StartAddonCooldownEveryoneRpc(networkObject, cooldown);
    }

    public void StopCooldown()
    {
        if (onCooldown && cooldownCoroutine != null)
        {
            StopCoroutine(cooldownCoroutine);
            cooldownCoroutine = null;

            onCooldown = false;
            SetCooldownTipsForItem(timeLeft: 0);
        }
    }

    public IEnumerator StartCooldownCoroutine(int cooldown)
    {
        while (cooldown > 0)
        {
            yield return new WaitForSecondsRealtime(1f);

            cooldown--;
            SetCooldownTipsForItem(cooldown);
        }
        onCooldown = false;
        cooldownCoroutine = null;
    }

    public void SetCooldownTipsForItem(int timeLeft)
    {
        if (grabbableObject != null && grabbableObject.isHeld && !grabbableObject.isPocketed)
        {
            string toolTip = timeLeft > 0 ? $"[On cooldown: {timeLeft}]" : "";
            SetTipsForItem(IsPassive ? [toolTip] : [AddonInput.Instance.GetAddonToolTip(), toolTip]);
        }
    }

    public void SetTipsForItem(string[] toolTips)
    {
        if (LFCUtilities.ShouldBeLocalPlayer(grabbableObject?.playerHeldBy))
            HUDManager.Instance.ChangeControlTipMultiple(grabbableObject.itemProperties.toolTips.Concat(toolTips).ToArray(), holdingItem: true, grabbableObject.itemProperties);
    }

    public void RemoveAddon()
    {
        if (grabbableObject != null)
        {
            if (grabbableObject.gameObject.TryGetComponentInChildren(out ScanNodeProperties scanNode))
            {
                string[] textsToRemove = ["\nAddon: " + AddonName, "Addon: " + AddonName];
                foreach (string textToRemove in textsToRemove)
                {
                    int index = scanNode.subText.IndexOf(textToRemove);
                    if (index >= 0)
                    {
                        scanNode.subText = scanNode.subText.Remove(index, textToRemove.Length);
                        break;
                    }
                }
            }
            if (LFCUtilities.ShouldBeLocalPlayer(grabbableObject.playerHeldBy))
            {
                HUDManager.Instance.ClearControlTips();
                HUDManager.Instance.ChangeControlTipMultiple(grabbableObject.itemProperties.toolTips, holdingItem: true, grabbableObject.itemProperties);
            }
            Destroy(this);
        }
    }
}
