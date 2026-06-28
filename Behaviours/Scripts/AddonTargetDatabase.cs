using System.Collections.Generic;

namespace AddonFusion.Behaviours.Scripts;

public static class AddonTargetDatabase
{
    public enum AddonTargetType
    {
        ALL,
        FLASHLIGHT,
        KNIFE,
        SHOVEL,
        SPRAY_PAINT,
        WALKIE_TALKIE,
        BOOMBOX,
        SHOTGUN
    }

    public static readonly Dictionary<AddonTargetType, AddonTargetDefinition> Targets = new()
    {
        {
            AddonTargetType.FLASHLIGHT,
            new AddonTargetDefinition
            {
                TargetType = typeof(FlashlightItem),
                ItemNames =
                [
                    "Pro-flashlight",
                    "Flashlight",
                    "Laser pointer",
                    "Elite-flashlight",
                    "Gummy flashlight",
                    "Heavy flashlight",
                    "Jacobs Ladder",
                    "ProMaxFlashlig",
                    "ProMaxPlusFlashlig",
                    "ProMaxPlusUltraFlashlig",
                    "Spotlight",
                    "Abandoned Flashlight",
                    "Abandoned Pro-Flashlight",
                    "Golden Flashlight",
                    "Industrial Flashlight"
                ]
            }
        },
        {
            AddonTargetType.KNIFE,
            new AddonTargetDefinition
            {
                TargetType = typeof(KnifeItem),
                ItemNames =
                [
                    "Poison Dagger"
                ]
            }
        },
        {
            AddonTargetType.SHOVEL,
            new AddonTargetDefinition
            {
                TargetType = typeof(Shovel),
                ItemNames =
                [
                    "Shovel"
                ]
            }
        },
        {
            AddonTargetType.SPRAY_PAINT,
            new AddonTargetDefinition
            {
                TargetType = typeof(SprayPaintItem),
                ItemNames =
                [
                    "Spray paint"
                ]
            }
        },
        {
            AddonTargetType.WALKIE_TALKIE,
            new AddonTargetDefinition
            {
                TargetType = typeof(WalkieTalkie),
                ItemNames =
                [
                    "Walkie-talkie"
                ]
            }
        },
        {
            AddonTargetType.BOOMBOX,
            new AddonTargetDefinition
            {
                TargetType = typeof(BoomboxItem),
                ItemNames =
                [
                    "Boombox"
                ]
            }
        },
        {
            AddonTargetType.SHOTGUN,
            new AddonTargetDefinition
            {
                TargetType = typeof(ShotgunItem),
                ItemNames =
                [
                    "Shotgun"
                ]
            }
        }
    };
}
