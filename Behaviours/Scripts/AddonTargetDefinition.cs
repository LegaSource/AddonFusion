using System;
using System.Collections.Generic;

namespace AddonFusion.Behaviours.Scripts;

public class AddonTargetDefinition
{
    public Type TargetType;
    public HashSet<string> ItemNames = [];

    public bool IsValid(GrabbableObject item) => item != null && ((TargetType != null && TargetType.IsAssignableFrom(item.GetType())) || ItemNames.Contains(item.itemProperties?.itemName));
}
