using System;
using static AddonFusion.Behaviours.Scripts.AddonTargetDatabase;

namespace AddonFusion.Behaviours.Scripts;

[AttributeUsage(AttributeTargets.Class)]
public class AddonInfoAttribute : Attribute
{
    public AddonTargetType TargetType;

    public AddonInfoAttribute(AddonTargetType targetType) => TargetType = targetType;
}
