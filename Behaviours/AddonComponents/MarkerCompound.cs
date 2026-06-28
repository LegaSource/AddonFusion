using AddonFusion.Behaviours.Scripts;
using AddonFusion.Managers;
using LegaFusionCore.Managers;
using LegaFusionCore.Utilities;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static AddonFusion.Behaviours.Scripts.AddonTargetDatabase;

namespace AddonFusion.Behaviours.AddonComponents;

[AddonInfo(AddonTargetType.SPRAY_PAINT)]
public class MarkerCompound : AddonComponent
{
    public override string AddonName => Constants.MARKER_COMPOUND;
    public override bool IsPassive => false;

    public Dictionary<GameObject, Color> targetEntities = [];

    public void AddTargetEntity(GameObject entity, Color color)
    {
        if (!targetEntities.ContainsKey(entity))
            targetEntities.Add(entity, color);
    }

    public override void ActivateAddonAbility()
    {
        if (grabbableObject is SprayPaintItem && grabbableObject.TryGetComponent(out NetworkObject networkObject))
            AddonFusionNetworkManager.Instance.EnableAddonEveryoneRpc(networkObject, !isEnabled);
    }

    public void Update()
    {
        if (isEnabled && grabbableObject is SprayPaintItem sprayPaint)
        {
            if (sprayPaint.isSpraying && sprayPaint.isHeld && sprayPaint.sprayCanTank > 0f)
            {
                if (LFCUtilities.ShouldBeLocalPlayer(sprayPaint.playerHeldBy))
                {
                    foreach (KeyValuePair<GameObject, Color> targetEntity in targetEntities)
                        LFCCustomPassManager.SetupAuraForObject(targetEntity.Key, LegaFusionCore.LegaFusionCore.wallhackShader, $"{AddonFusion.modName}{gameObject}", targetEntity.Value);
                }
                sprayPaint.sprayCanTank = Mathf.Max(sprayPaint.sprayCanTank - (Time.deltaTime / 30f), 0f);
                return;
            }
            if (LFCUtilities.ShouldBeLocalPlayer(sprayPaint.playerHeldBy))
                LFCCustomPassManager.RemoveAuraByTag($"{AddonFusion.modName}{gameObject}");
        }
    }
}