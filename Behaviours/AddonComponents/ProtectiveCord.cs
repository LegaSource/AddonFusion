using AddonFusion.Behaviours.Scripts;
using AddonFusion.Managers;
using GameNetcodeStuff;
using System.Collections;
using UnityEngine;
using static AddonFusion.Behaviours.Scripts.AddonTargetDatabase;

namespace AddonFusion.Behaviours.AddonComponents;

[AddonInfo(AddonTargetType.SHOVEL)]
public class ProtectiveCord : AddonComponent
{
    public override string AddonName => Constants.PROTECTIVE_CORD;
    public override bool IsPassive => false;

    public bool isParrying = false;

    public override void ActivateAddonAbility()
    {
        if (onCooldown)
        {
            HUDManager.Instance.DisplayTip("Information", "On cooldown!");
            return;
        }
        PlayerControllerB player = grabbableObject?.playerHeldBy;
        if (player != null && grabbableObject is Shovel shovel && !shovel.reelingUp)
        {
            StartCooldown(ConfigManager.protectiveCordCooldown.Value);
            shovel.previousPlayerHeldBy = player;
            _ = StartCoroutine(ParryCoroutine(shovel));
        }
    }

    private IEnumerator ParryCoroutine(Shovel shovel)
    {
        isParrying = true;
        ReelingUpShovel(true, ref shovel);
        yield return new WaitForSeconds(ConfigManager.protectiveCordWindowDuration.Value);
        ReelingUpShovel(false, ref shovel);
        isParrying = false;
    }

    public void ReelingUpShovel(bool enable, ref Shovel shovel)
    {
        shovel.reelingUp = enable;
        shovel.previousPlayerHeldBy.activatingItem = enable;
        shovel.previousPlayerHeldBy.twoHanded = enable;
        shovel.previousPlayerHeldBy.playerBodyAnimator.SetBool("reelingUp", value: enable);
        if (enable)
        {
            if (shovel.reelingUpCoroutine != null)
            {
                shovel.StopCoroutine(shovel.reelingUpCoroutine);
            }
            shovel.previousPlayerHeldBy.playerBodyAnimator.ResetTrigger("shovelHit");
            shovel.shovelAudio.PlayOneShot(shovel.reelUp);
            shovel.ReelUpSFXServerRpc();
        }
    }
}