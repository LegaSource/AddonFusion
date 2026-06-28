using LegaFusionCore.Utilities;
using UnityEngine;

namespace AddonFusion.Behaviours.Scripts;

public class AoEProjector : MonoBehaviour
{
    public float surfaceOffset = 0.05f;
    public LayerMask groundMask;

    public Vector3 CurrentPosition { get; private set; }
    public bool HasValidTarget { get; private set; }

    private void Update()
    {
        if (LFCUtilities.LocalPlayer == null) return;
        if (Physics.Raycast(new Ray(LFCUtilities.LocalPlayer.gameplayCamera.transform.position, LFCUtilities.LocalPlayer.gameplayCamera.transform.forward), out RaycastHit hit, 50f, groundMask, QueryTriggerInteraction.Ignore))
        {
            HasValidTarget = true;
            CurrentPosition = hit.point + (Vector3.up * surfaceOffset);
            transform.position = CurrentPosition;
            transform.up = hit.normal;
            return;
        }
        HasValidTarget = false;
    }

    public bool TryConfirm(out Vector3 position) => (position = HasValidTarget ? CurrentPosition : default) != default;
}