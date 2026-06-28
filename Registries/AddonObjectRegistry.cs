using System;
using System.Collections.Generic;
using UnityEngine;

namespace AddonFusion.Registries;

public class AddonObjectRegistry
{
    private static readonly Dictionary<Type, (string, GameObject)> addonObjectByType = [];
    private static readonly Dictionary<string, (Type, GameObject)> addonObjectByName = [];

    public static void Add(Type addonType, string addonName, GameObject addonObj)
    {
        addonObjectByType[addonType] = (addonName, addonObj);
        addonObjectByName[addonName] = (addonType, addonObj);
    }

    public static bool TryGetAddonObject(Type addonType, out GameObject addonObj)
    {
        addonObj = null;
        return addonObjectByType.TryGetValue(addonType, out (string, GameObject) value) && (addonObj = value.Item2) != null;
    }

    public static bool TryGetAddonName(Type addonType, out string addonName)
    {
        addonName = null;
        return addonObjectByType.TryGetValue(addonType, out (string, GameObject) value) && (addonName = value.Item1) != null;
    }

    public static bool TryGetAddonType(string addonName, out Type addonType)
    {
        addonType = null;
        return addonObjectByName.TryGetValue(addonName, out (Type, GameObject) value) && (addonType = value.Item1) != null;
    }
}
