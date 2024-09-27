using AddonFusion.AddonValues;
using BepInEx.Configuration;
using System.Collections.Generic;
using System.Globalization;

namespace AddonFusion
{
    internal class ConfigManager
    {
        // GLOBAL
        public static ConfigEntry<bool> isEphemeralEnabled;
        public static ConfigEntry<bool> isEphemeralDestroyEnabled;
        public static ConfigEntry<int> minEphemeralItem;
        public static ConfigEntry<int> maxEphemeralItem;
        // CAPSULE HOI-POI
        public static ConfigEntry<bool> isCapsuleEnabled;
        public static ConfigEntry<bool> isCapsuleSpawnable;
        public static ConfigEntry<int> capsuleRarity;
        public static ConfigEntry<bool> isCapsulePurchasable;
        public static ConfigEntry<int> capsulePrice;
        public static ConfigEntry<bool> isCapsuleCharge;
        public static ConfigEntry<string> capsuleItemValues;
        public static ConfigEntry<int> maxCapsule;
        // SALT TANK
        public static ConfigEntry<bool> isSaltTankEnabled;
        public static ConfigEntry<bool> isSaltTankSpawnable;
        public static ConfigEntry<int> saltTankRarity;
        public static ConfigEntry<bool> isSaltTankPurchasable;
        public static ConfigEntry<int> saltTankPrice;
        public static ConfigEntry<string> saltTankEntityValues;
        public static ConfigEntry<int> saltTankSpawnPerItem;
        // PROTECTIVE CORD
        public static ConfigEntry<bool> isCordEnabled;
        public static ConfigEntry<bool> isCordSpawnable;
        public static ConfigEntry<int> cordRarity;
        public static ConfigEntry<bool> isCordPurchasable;
        public static ConfigEntry<int> cordPrice;
        public static ConfigEntry<float> cordWindowDuration;
        public static ConfigEntry<float> cordSpamCooldown;
        public static ConfigEntry<bool> canCordStunDropItem;
        public static ConfigEntry<bool> canCordStunImmobilize;
        public static ConfigEntry<string> cordEntityValues;
        public static ConfigEntry<string> cordExclusions;
        public static ConfigEntry<int> cordSpawnPerItem;
        // FLASHLIGHT LENS
        public static ConfigEntry<bool> isLensEnabled;
        public static ConfigEntry<bool> isLensSpawnable;
        public static ConfigEntry<int> lensRarity;
        public static ConfigEntry<bool> isLensPurchasable;
        public static ConfigEntry<int> lensPrice;
        public static ConfigEntry<string> lensEntityValues;
        public static ConfigEntry<string> lensExclusions;
        public static ConfigEntry<int> lensSpawnPerItem;
        // BLADE SHARPENER
        public static ConfigEntry<bool> isSharpenerEnabled;
        public static ConfigEntry<bool> isSharpenerSpawnable;
        public static ConfigEntry<int> sharpenerRarity;
        public static ConfigEntry<bool> isSharpenerPurchasable;
        public static ConfigEntry<int> sharpenerPrice;
        public static ConfigEntry<string> sharpenerEntityValues;
        public static ConfigEntry<int> sharpenerSpawnPerItem;
        // SENZU
        public static ConfigEntry<bool> isSenzuEnabled;
        public static ConfigEntry<bool> isSenzuSpawnable;
        public static ConfigEntry<int> senzuRarity;
        public static ConfigEntry<bool> isSenzuPurchasable;
        public static ConfigEntry<int> senzuPrice;
        public static ConfigEntry<float> senzuReviveDuration;
        public static ConfigEntry<float> senzuHealthRegenDuration;
        public static ConfigEntry<float> senzuStaminaRegenDuration;
        public static ConfigEntry<int> senzuSpawnPerItem;
        // PYRETHRIN TANK
        public static ConfigEntry<bool> isPyrethrinTankEnabled;
        public static ConfigEntry<bool> isPyrethrinTankSpawnable;
        public static ConfigEntry<int> pyrethrinTankRarity;
        public static ConfigEntry<bool> isPyrethrinTankPurchasable;
        public static ConfigEntry<int> pyrethrinTankPrice;
        public static ConfigEntry<string> pyrethrinTankEntityValues;
        public static ConfigEntry<int> pyrethrinTankSpawnPerItem;
        // REPAIR MODULE
        public static ConfigEntry<bool> isRepairModuleEnabled;
        public static ConfigEntry<bool> isRepairModuleSpawnable;
        public static ConfigEntry<int> repairModuleRarity;
        public static ConfigEntry<bool> isRepairModulePurchasable;
        public static ConfigEntry<int> repairModulePrice;
        public static ConfigEntry<int> repairModuleDuration;
        public static ConfigEntry<int> repairModuleProfit;
        public static ConfigEntry<int> repairModuleSpawnPerItem;
        // EPHEMERAL FLASHLIGHT
        public static ConfigEntry<bool> isEphemeralFlashEnabled;
        public static ConfigEntry<int> ephemeralFlashRarity;
        public static ConfigEntry<int> ephemeralFlashAddonRarity;
        public static ConfigEntry<int> ephemeralFlashMinUse;
        public static ConfigEntry<int> ephemeralFlashMaxUse;
        // EPHEMERAL SHOVEL
        public static ConfigEntry<bool> isEphemeralShovelEnabled;
        public static ConfigEntry<int> ephemeralShovelRarity;
        public static ConfigEntry<int> ephemeralShovelAddonRarity;
        public static ConfigEntry<int> ephemeralShovelMinUse;
        public static ConfigEntry<int> ephemeralShovelMaxUse;
        // EPHEMERAL SPRAY PAINT
        public static ConfigEntry<bool> isEphemeralSprayPaintEnabled;
        public static ConfigEntry<int> ephemeralSprayPaintRarity;
        public static ConfigEntry<int> ephemeralSprayPaintAddonRarity;
        public static ConfigEntry<int> ephemeralSprayPaintMinUse;
        public static ConfigEntry<int> ephemeralSprayPaintMaxUse;
        // EPHEMERAL KNIFE
        public static ConfigEntry<bool> isEphemeralKnifeEnabled;
        public static ConfigEntry<int> ephemeralKnifeRarity;
        public static ConfigEntry<int> ephemeralKnifeAddonRarity;
        public static ConfigEntry<int> ephemeralKnifeMinUse;
        public static ConfigEntry<int> ephemeralKnifeMaxUse;
        // EPHEMERAL WEED KILLER
        public static ConfigEntry<bool> isEphemeralWeedKillerEnabled;
        public static ConfigEntry<int> ephemeralWeedKillerRarity;
        public static ConfigEntry<int> ephemeralWeedKillerAddonRarity;
        public static ConfigEntry<int> ephemeralWeedKillerMinUse;
        public static ConfigEntry<int> ephemeralWeedKillerMaxUse;
        // EPHEMERAL TZP-INHALANT
        public static ConfigEntry<bool> isEphemeralTZPEnabled;
        public static ConfigEntry<int> ephemeralTZPRarity;
        public static ConfigEntry<int> ephemeralTZPAddonRarity;
        public static ConfigEntry<int> ephemeralTZPMinUse;
        public static ConfigEntry<int> ephemeralTZPMaxUse;

        internal static void Load()
        {
            // GLOBAL
            isEphemeralEnabled = AddonFusion.configFile.Bind<bool>("_Global_", "Enable ephemeral items", true, "Is the ephemeral items enabled?");
            isEphemeralDestroyEnabled = AddonFusion.configFile.Bind<bool>("_Global_", "Enable ephemeral destruction", true, "Are ephemeral items destroyed when the ship takes off?");
            minEphemeralItem = AddonFusion.configFile.Bind<int>("_Global_", "Min ephemeral item", 4, "Minimum number of ephemeral items that will spawn in the dungeon.");
            maxEphemeralItem = AddonFusion.configFile.Bind<int>("_Global_", "Max ephemeral item", 8, "Maximum number of ephemeral items that will spawn in the dungeon.");
            // CAPSULE HOI-POI
            isCapsuleEnabled = AddonFusion.configFile.Bind<bool>("Capsule Hoi-Poi", "Enable", true, "Is the capsule Hoi-Poi enabled?");
            isCapsuleSpawnable = AddonFusion.configFile.Bind<bool>("Capsule Hoi-Poi", "Spawnable", false, "Is the capsule Hoi-Poi spawnable?");
            capsuleRarity = AddonFusion.configFile.Bind<int>("Capsule Hoi-Poi", "Rarity", 5, "Capsule Hoi-Poi spawn rarity");
            isCapsulePurchasable = AddonFusion.configFile.Bind<bool>("Capsule Hoi-Poi", "Purchasable", true, "Is the capsule Hoi-Poi purchasable?");
            capsulePrice = AddonFusion.configFile.Bind<int>("Capsule Hoi-Poi", "Price", 30, "Capsule Hoi-Poi price");
            isCapsuleCharge = AddonFusion.configFile.Bind<bool>("Capsule Hoi-Poi", "Charge", true, "Can the capsule Hoi-Poi charge items?");
            capsuleItemValues = AddonFusion.configFile.Bind<string>("Capsule Hoi-Poi", "Values", "default:600", "Values per item, the format is ItemName:ChargeDuration." +
                "\nChargeDuration: Time required to charge the item from empty to full capacity when it's in the capsule, in seconds.");
            maxCapsule = AddonFusion.configFile.Bind<int>("Capsule Hoi-Poi", "Max spawn", 2, "Max capsules to spawn");
            // SALT TANK
            isSaltTankEnabled = AddonFusion.configFile.Bind<bool>("Salt Tank", "Enable", true, "Is the salt tank enabled?");
            isSaltTankSpawnable = AddonFusion.configFile.Bind<bool>("Salt Tank", "Spawnable", true, "Is the salt tank spawnable?");
            saltTankRarity = AddonFusion.configFile.Bind<int>("Salt Tank", "Rarity", 5, "Salt tank spawn rarity");
            isSaltTankPurchasable = AddonFusion.configFile.Bind<bool>("Salt Tank", "Purchasable", false, "Is the salt tank purchasable?");
            saltTankPrice = AddonFusion.configFile.Bind<int>("Salt Tank", "Price", 30, "Salt tank price");
            saltTankEntityValues = AddonFusion.configFile.Bind<string>("Salt Tank", "Values", "default:20:10", "Values per entity, the format is EntityName:BaseChance:AdditionalChance." +
                "\nBaseChance: Base chance for the entity to stop chasing when hit by a salted graffiti." +
                "\nAdditionalChance: Additional chance for the entity to stop chasing when hit by a salted graffiti.");
            saltTankSpawnPerItem = AddonFusion.configFile.Bind<int>("Salt Tank", "Spawn addon per item", 1, "If this option is enabled, it limits the number of spawnable addons based on the associated item + configured value." +
                "\nIf there is :" +
                "\n- 1 spray paint without an addon -> 1 salt tank + 1 (default value) can spawn" +
                "\n- 2 sprays -> 2 salt tank + 1 (default value) can spawn" +
                "\n- 0 sprays -> 1 (default value) salt tank can spawn");
            // PROTECTIVE CORD
            isCordEnabled = AddonFusion.configFile.Bind<bool>("Protective Cord", "Enable", true, "Is the protective cord enabled?");
            isCordSpawnable = AddonFusion.configFile.Bind<bool>("Protective Cord", "Spawnable", true, "Is the protective cord spawnable?");
            cordRarity = AddonFusion.configFile.Bind<int>("Protective Cord", "Rarity", 100, "Protective cord spawn rarity");
            isCordPurchasable = AddonFusion.configFile.Bind<bool>("Protective Cord", "Purchasable", false, "Is the protective cord purchasable?");
            cordPrice = AddonFusion.configFile.Bind<int>("Protective Cord", "Price", 30, "Protective cord price");
            cordWindowDuration = AddonFusion.configFile.Bind<float>("Protective Cord", "Window duration", 0.35f, "Parrying window duration");
            cordSpamCooldown = AddonFusion.configFile.Bind<float>("Protective Cord", "Spam cooldown", 1f, "Cooldown duration per use");
            canCordStunDropItem = AddonFusion.configFile.Bind<bool>("Protective Cord", "Player item drop", true, "Does the stun cause the player to drop their weapon?");
            canCordStunImmobilize = AddonFusion.configFile.Bind<bool>("Protective Cord", "Player immobilization", true, "Does the stun immobilize the player?");
            cordEntityValues = AddonFusion.configFile.Bind<string>("Protective Cord", "Values", "default:10:1.5:5:100:10,Player:5:4:0:0:0,Flowerman:10:5:5:100:10,MouthDog:10:2.5:5:100:10", "Values per entity, the format is EntityName:CooldownDuration:StunDuration:SpeedBoostDuration:SpeedMultiplier:StaminaRegen." +
                "\nSpeedMultiplier: Speed multiplier percentage." +
                "\nStaminaRegen: Stamina regen percentage.");
            cordExclusions = AddonFusion.configFile.Bind<string>("Protective Cord", "Exclusion list", "Flowerman,MouthDog,ForestGiant", "List of creatures that will not be affected by the stun.");
            cordSpawnPerItem = AddonFusion.configFile.Bind<int>("Protective Cord", "Spawn addon per item", 1, "If this option is enabled, it limits the number of spawnable addons based on the associated item + configured value." +
                "\nIf there is :" +
                "\n- 1 shovel without an addon -> 1 protective cord + 1 (default value) can spawn" +
                "\n- 2 shovels -> 2 protective cord + 1 (default value) can spawn" +
                "\n- 0 shovels -> 1 (default value) protective cord can spawn");
            // FLASHLIGHT LENS
            isLensEnabled = AddonFusion.configFile.Bind<bool>("Flashlight Lens", "Enable", true, "Is the flashlight lens enabled?");
            isLensSpawnable = AddonFusion.configFile.Bind<bool>("Flashlight Lens", "Spawnable", true, "Is the flashlight lens spawnable?");
            lensRarity = AddonFusion.configFile.Bind<int>("Flashlight Lens", "Rarity", 100, "Flashlight lens spawn rarity");
            isLensPurchasable = AddonFusion.configFile.Bind<bool>("Flashlight Lens", "Purchasable", false, "Is the flashlight lens purchasable?");
            lensPrice = AddonFusion.configFile.Bind<int>("Flashlight Lens", "Price", 30, "Flashlight lens price");
            lensEntityValues = AddonFusion.configFile.Bind<string>("Flashlight Lens", "Values", "default:2:3:15:120:10:10:0,Player:2:0:10:60:10:0:0,ForestGiant:2:4:30:120:20:20:0", "Values per entity, the format is EntityName:FlashDuration:StunDuration:LightAngle:EntityAngle:EntityDistance:ImmunityDuration:BatteryConsumption." +
                "\nFlashDuration: Time required to blind an enemy." +
                "\nLightAngle: Angle of the light in relation to the enemy's eyes - increasing this value makes aiming easier." +
                "\nEntityAngle: Angle of the enemy in relation to the flashlight - increasing this value makes blinding from an angle easier.");
            lensExclusions = AddonFusion.configFile.Bind<string>("Flashlight Lens", "Exclusion list", "MouthDog", "List of creatures that will not be affected by the stun.");
            lensSpawnPerItem = AddonFusion.configFile.Bind<int>("Flashlight Lens", "Spawn addon per item", 1, "If this option is enabled, it limits the number of spawnable addons based on the associated item + configured value." +
                "\nIf there is :" +
                "\n- 1 flashlight without an addon -> 1 flashlight lens + 1 (default value) can spawn" +
                "\n- 2 flashlights -> 2 flashlight lens + 1 (default value) can spawn" +
                "\n- 0 flashlights -> 1 (default value) flashlight lens can spawn");
            // BLADE SHARPENER
            isSharpenerEnabled = AddonFusion.configFile.Bind<bool>("Blade Sharpener", "Enable", true, "Is the blade sharpener enabled?");
            isSharpenerSpawnable = AddonFusion.configFile.Bind<bool>("Blade Sharpener", "Spawnable", true, "Is the blade sharpener spawnable?");
            sharpenerRarity = AddonFusion.configFile.Bind<int>("Blade Sharpener", "Rarity", 5, "Blade sharpener spawn rarity");
            isSharpenerPurchasable = AddonFusion.configFile.Bind<bool>("Blade Sharpener", "Purchasable", false, "Is the blade sharpener purchasable?");
            sharpenerPrice = AddonFusion.configFile.Bind<int>("Blade Sharpener", "Price", 30, "Blade sharpener price");
            sharpenerEntityValues = AddonFusion.configFile.Bind<string>("Blade Sharpener", "Values", "default:15:10", "Values per entity, the format is EntityName:CriticalSuccessChance:CriticalFailChance." +
                "\nCriticalSuccessChance: Chance for the player to score a critical hit and kill the enemy in one strike." +
                "\nCriticalFailChance: Chance for the player to perform a critical failure, causing the player's weapon to drop on the ground without dealing damage.");
            sharpenerSpawnPerItem = AddonFusion.configFile.Bind<int>("Blade Sharpener", "Spawn addon per item", 1, "If this option is enabled, it limits the number of spawnable addons based on the associated item + configured value." +
                "\nIf there is :" +
                "\n- 1 knife without an addon -> 1 blade sharpener + 1 (default value) can spawn" +
                "\n- 2 knives -> 2 blade sharpener + 1 (default value) can spawn" +
                "\n- 0 knives -> 1 (default value) blade sharpener can spawn");
            // SENZU
            isSenzuEnabled = AddonFusion.configFile.Bind<bool>("Senzu", "Enable", true, "Is the senzu enabled?");
            isSenzuSpawnable = AddonFusion.configFile.Bind<bool>("Senzu", "Spawnable", false, "Is the senzu spawnable?");
            senzuRarity = AddonFusion.configFile.Bind<int>("Senzu", "Rarity", 5, "Senzu spawn rarity");
            isSenzuPurchasable = AddonFusion.configFile.Bind<bool>("Senzu", "Purchasable", true, "Is the senzu purchasable?");
            senzuPrice = AddonFusion.configFile.Bind<int>("Senzu", "Price", 30, "Senzu price");
            senzuReviveDuration = AddonFusion.configFile.Bind<float>("Senzu", "Revive duration", 60f, "Duration during which a player can be revived.");
            senzuHealthRegenDuration = AddonFusion.configFile.Bind<float>("Senzu", "Health regen duration", 25f, "Time required to regen the health from 0 to max, in seconds.");
            senzuStaminaRegenDuration = AddonFusion.configFile.Bind<float>("Senzu", "Stamina regen duration", 35f, "Time required to regen the stamina from 0 to max, in seconds.");
            senzuSpawnPerItem = AddonFusion.configFile.Bind<int>("Senzu", "Spawn addon per item", 1, "If this option is enabled, it limits the number of spawnable addons based on the associated item + configured value." +
                "\nIf there is :" +
                "\n- 1 TZP chemical an addon -> 1 senzu + 1 (default value) can spawn" +
                "\n- 2 TZP -> 2 senzu + 1 (default value) can spawn" +
                "\n- 0 TZP -> 1 (default value) senzu can spawn");
            // PYRETHRIN TANK
            isPyrethrinTankEnabled = AddonFusion.configFile.Bind<bool>("Pyrethrin Tank", "Enable", true, "Is the pyrethrin tank enabled?");
            isPyrethrinTankSpawnable = AddonFusion.configFile.Bind<bool>("Pyrethrin Tank", "Spawnable", true, "Is the pyrethrin tank spawnable?");
            pyrethrinTankRarity = AddonFusion.configFile.Bind<int>("Pyrethrin Tank", "Rarity", 5, "Pyrethrin tank spawn rarity");
            isPyrethrinTankPurchasable = AddonFusion.configFile.Bind<bool>("Pyrethrin Tank", "Purchasable", false, "Is the pyrethrin tank purchasable?");
            pyrethrinTankPrice = AddonFusion.configFile.Bind<int>("Pyrethrin Tank", "Price", 30, "Pyrethrin tank price");
            pyrethrinTankEntityValues = AddonFusion.configFile.Bind<string>("Pyrethrin Tank", "Values", "Red Locust Bees:3,Butler Bees:5,Bunker Spider:3,Hoarding bug:3,Centipede:3", "Values per entity, the format is EntityName:FleeDuration." +
                "\nFleeDuration: Duration for which the entity runs away from the player.");
            pyrethrinTankSpawnPerItem = AddonFusion.configFile.Bind<int>("Pyrethrin Tank", "Spawn addon per item", 1, "If this option is enabled, it limits the number of spawnable addons based on the associated item + configured value." +
                "\nIf there is :" +
                "\n- 1 weedkiller an addon -> 1 pyrethrin tank + 1 (default value) can spawn" +
                "\n- 2 weedkillers -> 2 pyrethrin tank + 1 (default value) can spawn" +
                "\n- 0 weedkillers -> 1 (default value) pyrethrin tank can spawn");
            // REPAIR MODULE
            isRepairModuleEnabled = AddonFusion.configFile.Bind<bool>("Repair Module", "Enable", true, "Is the repair module enabled?");
            isRepairModuleSpawnable = AddonFusion.configFile.Bind<bool>("Repair Module", "Spawnable", true, "Is the repair module spawnable?");
            repairModuleRarity = AddonFusion.configFile.Bind<int>("Repair Module", "Rarity", 5, "Repair module spawn rarity");
            isRepairModulePurchasable = AddonFusion.configFile.Bind<bool>("Repair Module", "Purchasable", false, "Is the repair module purchasable?");
            repairModulePrice = AddonFusion.configFile.Bind<int>("Repair Module", "Price", 30, "Repair module price");
            repairModuleDuration = AddonFusion.configFile.Bind<int>("Repair Module", "Repair duration", 600, "Time required to repair the item and add its profit value.");
            repairModuleProfit = AddonFusion.configFile.Bind<int>("Repair Module", "Profit", 50, "Profit percentage relative to the item's initial value.");
            repairModuleSpawnPerItem = AddonFusion.configFile.Bind<int>("Repair Module", "Spawn addon per item", 1, "If this option is enabled, it limits the number of spawnable addons based on the associated item + configured value." +
                "\nIf there is :" +
                "\n- 1 capsule Hoi-Poi an addon -> 1 repair module + 1 (default value) can spawn" +
                "\n- 2 capsules -> 2 repair module + 1 (default value) can spawn" +
                "\n- 0 capsules -> 1 (default value) repair module can spawn");
            // EPHEMERAL FLASHLIGHT
            isEphemeralFlashEnabled = AddonFusion.configFile.Bind<bool>("Ephemeral Flashlight", "Enable", true, "Is the ephemeral flashlight enabled?");
            ephemeralFlashRarity = AddonFusion.configFile.Bind<int>("Ephemeral Flashlight", "Rarity", 15, "Ephemeral flashlight spawn rarity");
            ephemeralFlashAddonRarity = AddonFusion.configFile.Bind<int>("Ephemeral Flashlight", "Addon rarity", 20, "Chance for the ephemeral flashlight to spawn with an addon.");
            ephemeralFlashMinUse = AddonFusion.configFile.Bind<int>("Ephemeral Flashlight", "Min battery", 10, "Minimum battery before the ephemeral flashlight breaks (1 - 100).");
            ephemeralFlashMaxUse = AddonFusion.configFile.Bind<int>("Ephemeral Flashlight", "Max battery", 60, "Maximum battery before the ephemeral flashlight breaks (1 - 100).");
            // EPHEMERAL SHOVEL
            isEphemeralShovelEnabled = AddonFusion.configFile.Bind<bool>("Ephemeral Shovel", "Enable", true, "Is the ephemeral shovel enabled?");
            ephemeralShovelRarity = AddonFusion.configFile.Bind<int>("Ephemeral Shovel", "Rarity", 15, "Ephemeral shovel spawn rarity");
            ephemeralShovelAddonRarity = AddonFusion.configFile.Bind<int>("Ephemeral Shovel", "Addon rarity", 20, "Chance for the ephemeral shovel to spawn with an addon.");
            ephemeralShovelMinUse = AddonFusion.configFile.Bind<int>("Ephemeral Shovel", "Min use", 4, "Minimum number of uses before the ephemeral shovel breaks.");
            ephemeralShovelMaxUse = AddonFusion.configFile.Bind<int>("Ephemeral Shovel", "Max use", 10, "Maximum number of uses before the ephemeral shovel breaks.");
            // EPHEMERAL SPRAY PAINT
            isEphemeralSprayPaintEnabled = AddonFusion.configFile.Bind<bool>("Ephemeral Spray Paint", "Enable", true, "Is the ephemeral spray paint enabled?");
            ephemeralSprayPaintRarity = AddonFusion.configFile.Bind<int>("Ephemeral Spray Paint", "Rarity", 10, "Ephemeral spray paint spawn rarity");
            ephemeralSprayPaintAddonRarity = AddonFusion.configFile.Bind<int>("Ephemeral Spray Paint", "Addon rarity", 20, "Chance for the ephemeral spray paint to spawn with an addon.");
            ephemeralSprayPaintMinUse = AddonFusion.configFile.Bind<int>("Ephemeral Spray Paint", "Min tank", 10, "Minimum tank value before the ephemeral spray paint breaks (1 - 100).");
            ephemeralSprayPaintMaxUse = AddonFusion.configFile.Bind<int>("Ephemeral Spray Paint", "Max tank", 60, "Maximum tank value before the ephemeral spray paint breaks (1 - 100).");
            // EPHEMERAL KNIFE
            isEphemeralKnifeEnabled = AddonFusion.configFile.Bind<bool>("Ephemeral Knife", "Enable", true, "Is the ephemeral knife enabled?");
            ephemeralKnifeRarity = AddonFusion.configFile.Bind<int>("Ephemeral Knife", "Rarity", 10, "Ephemeral knife spawn rarity");
            ephemeralKnifeAddonRarity = AddonFusion.configFile.Bind<int>("Ephemeral Knife", "Addon rarity", 20, "Chance for the ephemeral knife to spawn with an addon.");
            ephemeralKnifeMinUse = AddonFusion.configFile.Bind<int>("Ephemeral Shovel", "Min use", 3, "Minimum number of uses before the ephemeral knife breaks.");
            ephemeralKnifeMaxUse = AddonFusion.configFile.Bind<int>("Ephemeral Shovel", "Max use", 6, "Maximum number of uses before the ephemeral knife breaks.");
            // EPHEMERAL WEED KILLER
            isEphemeralWeedKillerEnabled = AddonFusion.configFile.Bind<bool>("Ephemeral Weed Killer", "Enable", true, "Is the ephemeral weed killer enabled?");
            ephemeralWeedKillerRarity = AddonFusion.configFile.Bind<int>("Ephemeral Weed Killer", "Rarity", 10, "Ephemeral weed killer spawn rarity");
            ephemeralWeedKillerAddonRarity = AddonFusion.configFile.Bind<int>("Ephemeral Weed Killer", "Addon rarity", 20, "Chance for the ephemeral weed killer to spawn with an addon.");
            ephemeralWeedKillerMinUse = AddonFusion.configFile.Bind<int>("Ephemeral Weed Killer", "Min fuel", 10, "Minimum fuel before the ephemeral weed killer breaks (1 - 100).");
            ephemeralWeedKillerMaxUse = AddonFusion.configFile.Bind<int>("Ephemeral Weed Killer", "Max fuel", 60, "Maximum fuel before the ephemeral weed killer breaks (1 - 100).");
            // EPHEMERAL TZP-INHALANT
            isEphemeralTZPEnabled = AddonFusion.configFile.Bind<bool>("Ephemeral TZP-Inhalant", "Enable", true, "Is the ephemeral TZP-Inhalant enabled?");
            ephemeralTZPRarity = AddonFusion.configFile.Bind<int>("Ephemeral TZP-Inhalant", "Rarity", 10, "Ephemeral TZP-Inhalant spawn rarity");
            ephemeralTZPAddonRarity = AddonFusion.configFile.Bind<int>("Ephemeral TZP-Inhalant", "Addon rarity", 20, "Chance for the ephemeral TZP-Inhalant to spawn with an addon.");
            ephemeralTZPMinUse = AddonFusion.configFile.Bind<int>("Ephemeral TZP-Inhalant", "Min fuel", 10, "Minimum fuel before the ephemeral TZP-Inhalant breaks (1 - 100).");
            ephemeralTZPMaxUse = AddonFusion.configFile.Bind<int>("Ephemeral TZP-Inhalant", "Max fuel", 60, "Maximum fuel before the ephemeral TZP-Inhalant breaks (1 - 100).");
        }

        internal static List<CapsuleHoiPoiValue> GetCapsuleValuesFromConfig()
        {
            List<CapsuleHoiPoiValue> capsuleValues = new List<CapsuleHoiPoiValue>();
            string[] items = capsuleItemValues.Value.Split(',');
            foreach (string itemValue in items)
            {
                string[] values = itemValue.Split(':');
                if (values.Length == 2)
                {
                    capsuleValues.Add(new CapsuleHoiPoiValue(values[0],
                        int.Parse(values[1])));
                }
            }
            return capsuleValues;
        }

        internal static List<SaltTankValue> GetSaltTankValuesFromConfig()
        {
            List<SaltTankValue> saltTankValues = new List<SaltTankValue>();
            string[] entities = saltTankEntityValues.Value.Split(',');
            foreach (string entityValue in entities)
            {
                string[] values = entityValue.Split(':');
                if (values.Length == 3)
                {
                    saltTankValues.Add(new SaltTankValue(values[0],
                        int.Parse(values[1]),
                        int.Parse(values[2])));
                }
            }
            return saltTankValues;
        }

        internal static List<ProtectiveCordValue> GetProtectiveCordValuesFromConfig()
        {
            List<ProtectiveCordValue> protectiveCordValues = new List<ProtectiveCordValue>();
            string[] entities = cordEntityValues.Value.Split(',');
            foreach (string entityValue in entities)
            {
                string[] values = entityValue.Split(':');
                if (values.Length == 6)
                {
                    protectiveCordValues.Add(new ProtectiveCordValue(values[0],
                        float.Parse(values[1], CultureInfo.InvariantCulture),
                        float.Parse(values[2], CultureInfo.InvariantCulture),
                        float.Parse(values[3], CultureInfo.InvariantCulture),
                        int.Parse(values[4]),
                        int.Parse(values[5])));
                }
            }
            return protectiveCordValues;
        }

        internal static List<LensValue> GetLensValuesFromConfig()
        {
            List<LensValue> lensValues = new List<LensValue>();
            string[] entities = lensEntityValues.Value.Split(',');
            foreach (string entityValue in entities)
            {
                string[] values = entityValue.Split(':');
                if (values.Length == 8)
                {
                    lensValues.Add(new LensValue(values[0],
                        float.Parse(values[1], CultureInfo.InvariantCulture),
                        float.Parse(values[2], CultureInfo.InvariantCulture),
                        float.Parse(values[3], CultureInfo.InvariantCulture),
                        float.Parse(values[4], CultureInfo.InvariantCulture),
                        int.Parse(values[5]),
                        float.Parse(values[6], CultureInfo.InvariantCulture),
                        float.Parse(values[7], CultureInfo.InvariantCulture)));
                }
            }
            return lensValues;
        }

        internal static List<BladeSharpenerValue> GetBladeSharpenerValuesFromConfig()
        {
            List<BladeSharpenerValue> bladeSharpenerValues = new List<BladeSharpenerValue>();
            string[] entities = sharpenerEntityValues.Value.Split(',');
            foreach (string entityValue in entities)
            {
                string[] values = entityValue.Split(':');
                if (values.Length == 3)
                {
                    bladeSharpenerValues.Add(new BladeSharpenerValue(values[0],
                        int.Parse(values[1]),
                        int.Parse(values[2])));
                }
            }
            return bladeSharpenerValues;
        }

        internal static List<PyrethrinTankValue> GetPyrethrinTankValuesFromConfig()
        {
            List<PyrethrinTankValue> pyrethrinTankValues = new List<PyrethrinTankValue>();
            string[] entities = pyrethrinTankEntityValues.Value.Split(',');
            foreach (string entityValue in entities)
            {
                string[] values = entityValue.Split(':');
                if (values.Length == 2)
                {
                    pyrethrinTankValues.Add(new PyrethrinTankValue(values[0],
                        float.Parse(values[1])));
                }
            }
            return pyrethrinTankValues;
        }
    }
}
