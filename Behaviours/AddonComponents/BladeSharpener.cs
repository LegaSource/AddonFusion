using AddonFusion.Behaviours.Scripts;
using static AddonFusion.Behaviours.Scripts.AddonTargetDatabase;

namespace AddonFusion.Behaviours.AddonComponents;

[AddonInfo(AddonTargetType.KNIFE)]
public class BladeSharpener : AddonComponent
{
    public override string AddonName => Constants.BLADE_SHARPENER;
    public override bool IsPassive => true;
}