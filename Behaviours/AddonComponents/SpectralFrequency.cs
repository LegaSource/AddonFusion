using AddonFusion.Behaviours.Scripts;
using AddonFusion.Managers;
using Unity.Netcode;
using UnityEngine;
using static AddonFusion.Behaviours.Scripts.AddonTargetDatabase;

namespace AddonFusion.Behaviours.AddonComponents;

[AddonInfo(AddonTargetType.WALKIE_TALKIE)]
public class SpectralFrequency : AddonComponent
{
    public override string AddonName => Constants.SPECTRAL_FREQUENCY;
    public override bool IsPassive => false;

    public override void ActivateAddonAbility()
    {
        if (grabbableObject is WalkieTalkie && grabbableObject.TryGetComponent(out NetworkObject networkObject))
            AddonFusionNetworkManager.Instance.EnableAddonEveryoneRpc(networkObject, !isEnabled);
    }

    public void Update()
    {
        if (isEnabled && grabbableObject is WalkieTalkie walkieTalkie && walkieTalkie.isBeingUsed && walkieTalkie.insertedBattery.charge > 0f)
            walkieTalkie.insertedBattery.charge -= Time.deltaTime / walkieTalkie.itemProperties.batteryUsage;
    }
}
