using AddonFusion.Behaviours.Scripts;
using static AddonFusion.Behaviours.Scripts.AddonTargetDatabase;

namespace AddonFusion.Behaviours.AddonComponents;

[AddonInfo(AddonTargetType.SHOTGUN)]
public class SaltTank : AddonComponent
{
    public override string AddonName => Constants.PROTECTIVE_CORD;
    public override bool IsPassive => true;
}
