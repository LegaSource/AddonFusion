using BepInEx.Configuration;

namespace AddonFusion.Managers;

public class ConfigManager
{
    // BLADE SHARPENER
    public static ConfigEntry<int> bladeSharpenerRarity;
    public static ConfigEntry<int> bladeSharpenerMinValue;
    public static ConfigEntry<int> bladeSharpenerMaxValue;
    public static ConfigEntry<bool> isBladeSharpenerPurchasable;
    public static ConfigEntry<int> bladeSharpenerPrice;
    // ECHO SCANNER
    public static ConfigEntry<int> echoScannerRarity;
    public static ConfigEntry<int> echoScannerMinValue;
    public static ConfigEntry<int> echoScannerMaxValue;
    public static ConfigEntry<bool> isEchoScannerPurchasable;
    public static ConfigEntry<int> echoScannerPrice;
    // FLASHLIGHT LENS
    public static ConfigEntry<int> flashlightLensRarity;
    public static ConfigEntry<int> flashlightLensMinValue;
    public static ConfigEntry<int> flashlightLensMaxValue;
    public static ConfigEntry<bool> isFlashlightLensPurchasable;
    public static ConfigEntry<int> flashlightLensPrice;
    public static ConfigEntry<int> flashlightLensCooldown;
    // MARKER COMPOUND
    public static ConfigEntry<int> markerCompoundRarity;
    public static ConfigEntry<int> markerCompoundMinValue;
    public static ConfigEntry<int> markerCompoundMaxValue;
    public static ConfigEntry<bool> isMarkerCompoundPurchasable;
    public static ConfigEntry<int> markerCompoundPrice;
    // PROTECTIVE CORD
    public static ConfigEntry<int> protectiveCordRarity;
    public static ConfigEntry<int> protectiveCordMinValue;
    public static ConfigEntry<int> protectiveCordMaxValue;
    public static ConfigEntry<bool> isProtectiveCordPurchasable;
    public static ConfigEntry<int> protectiveCordPrice;
    public static ConfigEntry<float> protectiveCordWindowDuration;
    public static ConfigEntry<int> protectiveCordCooldown;
    // SALT TANK
    public static ConfigEntry<int> saltTankRarity;
    public static ConfigEntry<int> saltTankMinValue;
    public static ConfigEntry<int> saltTankMaxValue;
    public static ConfigEntry<bool> isSaltTankPurchasable;
    public static ConfigEntry<int> saltTankPrice;
    // SPECTRAL FREQUENCY
    public static ConfigEntry<int> spectralFrequencyRarity;
    public static ConfigEntry<int> spectralFrequencyMinValue;
    public static ConfigEntry<int> spectralFrequencyMaxValue;
    public static ConfigEntry<bool> isSpectralFrequencyPurchasable;
    public static ConfigEntry<int> spectralFrequencyPrice;

    internal static void Load()
    {
        // BLADE SHARPENER
        bladeSharpenerRarity = AddonFusion.configFile.Bind(Constants.BLADE_SHARPENER, "Rarity", 20, $"{Constants.BLADE_SHARPENER} spawn rarity");
        bladeSharpenerMinValue = AddonFusion.configFile.Bind(Constants.BLADE_SHARPENER, "Min value", 15, $"{Constants.BLADE_SHARPENER} min value");
        bladeSharpenerMaxValue = AddonFusion.configFile.Bind(Constants.BLADE_SHARPENER, "Max value", 25, $"{Constants.BLADE_SHARPENER} max value");
        isBladeSharpenerPurchasable = AddonFusion.configFile.Bind(Constants.BLADE_SHARPENER, "Purchasable", false, $"Is the {Constants.BLADE_SHARPENER} purchasable?");
        bladeSharpenerPrice = AddonFusion.configFile.Bind(Constants.BLADE_SHARPENER, "Price", 30, $"{Constants.BLADE_SHARPENER} price");
        // ECHO SCANNER
        echoScannerRarity = AddonFusion.configFile.Bind(Constants.ECHO_SCANNER, "Rarity", 10, $"{Constants.ECHO_SCANNER} spawn rarity");
        echoScannerMinValue = AddonFusion.configFile.Bind(Constants.ECHO_SCANNER, "Min value", 60, $"{Constants.ECHO_SCANNER} min value");
        echoScannerMaxValue = AddonFusion.configFile.Bind(Constants.ECHO_SCANNER, "Max value", 70, $"{Constants.ECHO_SCANNER} max value");
        isEchoScannerPurchasable = AddonFusion.configFile.Bind(Constants.ECHO_SCANNER, "Purchasable", false, $"Is the {Constants.ECHO_SCANNER} purchasable?");
        echoScannerPrice = AddonFusion.configFile.Bind(Constants.ECHO_SCANNER, "Price", 75, $"{Constants.ECHO_SCANNER} price");
        // FLASHLIGHT LENS
        flashlightLensRarity = AddonFusion.configFile.Bind(Constants.FLASHLIGHT_LENS, "Rarity", 10, $"{Constants.FLASHLIGHT_LENS} spawn rarity");
        flashlightLensMinValue = AddonFusion.configFile.Bind(Constants.FLASHLIGHT_LENS, "Min value", 65, $"{Constants.FLASHLIGHT_LENS} min value");
        flashlightLensMaxValue = AddonFusion.configFile.Bind(Constants.FLASHLIGHT_LENS, "Max value", 75, $"{Constants.FLASHLIGHT_LENS} max value");
        isFlashlightLensPurchasable = AddonFusion.configFile.Bind(Constants.FLASHLIGHT_LENS, "Purchasable", false, $"Is the {Constants.FLASHLIGHT_LENS} purchasable?");
        flashlightLensPrice = AddonFusion.configFile.Bind(Constants.FLASHLIGHT_LENS, "Price", 80, $"{Constants.FLASHLIGHT_LENS} price");
        flashlightLensCooldown = AddonFusion.configFile.Bind(Constants.FLASHLIGHT_LENS, "Spam cooldown", 10, "Cooldown duration per use");
        // MARKER COMPOUND
        markerCompoundRarity = AddonFusion.configFile.Bind(Constants.MARKER_COMPOUND, "Rarity", 5, $"{Constants.MARKER_COMPOUND} spawn rarity");
        markerCompoundMinValue = AddonFusion.configFile.Bind(Constants.MARKER_COMPOUND, "Min value", 75, $"{Constants.MARKER_COMPOUND} min value");
        markerCompoundMaxValue = AddonFusion.configFile.Bind(Constants.MARKER_COMPOUND, "Max value", 85, $"{Constants.MARKER_COMPOUND} max value");
        isMarkerCompoundPurchasable = AddonFusion.configFile.Bind(Constants.MARKER_COMPOUND, "Purchasable", false, $"Is the {Constants.MARKER_COMPOUND} purchasable?");
        markerCompoundPrice = AddonFusion.configFile.Bind(Constants.MARKER_COMPOUND, "Price", 90, $"{Constants.MARKER_COMPOUND} price");
        // PROTECTIVE CORD
        protectiveCordRarity = AddonFusion.configFile.Bind(Constants.PROTECTIVE_CORD, "Rarity", 15, $"{Constants.PROTECTIVE_CORD} spawn rarity");
        protectiveCordMinValue = AddonFusion.configFile.Bind(Constants.PROTECTIVE_CORD, "Min value", 40, $"{Constants.PROTECTIVE_CORD} min value");
        protectiveCordMaxValue = AddonFusion.configFile.Bind(Constants.PROTECTIVE_CORD, "Max value", 50, $"{Constants.PROTECTIVE_CORD} max value");
        isProtectiveCordPurchasable = AddonFusion.configFile.Bind(Constants.PROTECTIVE_CORD, "Purchasable", false, $"Is the {Constants.PROTECTIVE_CORD} purchasable?");
        protectiveCordPrice = AddonFusion.configFile.Bind(Constants.PROTECTIVE_CORD, "Price", 55, $"{Constants.PROTECTIVE_CORD} price");
        protectiveCordWindowDuration = AddonFusion.configFile.Bind(Constants.PROTECTIVE_CORD, "Window duration", 0.35f, "Parrying window duration");
        protectiveCordCooldown = AddonFusion.configFile.Bind(Constants.PROTECTIVE_CORD, "Spam cooldown", 2, "Cooldown duration per use");
        // SALT TANK
        saltTankRarity = AddonFusion.configFile.Bind(Constants.SALT_TANK, "Rarity", 20, $"{Constants.SALT_TANK} spawn rarity");
        saltTankMinValue = AddonFusion.configFile.Bind(Constants.SALT_TANK, "Min value", 10, $"{Constants.SALT_TANK} min value");
        saltTankMaxValue = AddonFusion.configFile.Bind(Constants.SALT_TANK, "Max value", 20, $"{Constants.SALT_TANK} max value");
        isSaltTankPurchasable = AddonFusion.configFile.Bind(Constants.SALT_TANK, "Purchasable", false, $"Is the {Constants.SALT_TANK} purchasable?");
        saltTankPrice = AddonFusion.configFile.Bind(Constants.SALT_TANK, "Price", 25, $"{Constants.SALT_TANK} price");
        // SPECTRAL FREQUENCY
        spectralFrequencyRarity = AddonFusion.configFile.Bind(Constants.SPECTRAL_FREQUENCY, "Rarity", 5, $"{Constants.SPECTRAL_FREQUENCY} spawn rarity");
        spectralFrequencyMinValue = AddonFusion.configFile.Bind(Constants.SPECTRAL_FREQUENCY, "Min value", 80, $"{Constants.SPECTRAL_FREQUENCY} min value");
        spectralFrequencyMaxValue = AddonFusion.configFile.Bind(Constants.SPECTRAL_FREQUENCY, "Max value", 90, $"{Constants.SPECTRAL_FREQUENCY} max value");
        isSpectralFrequencyPurchasable = AddonFusion.configFile.Bind(Constants.SPECTRAL_FREQUENCY, "Purchasable", false, $"Is the {Constants.SPECTRAL_FREQUENCY} purchasable?");
        spectralFrequencyPrice = AddonFusion.configFile.Bind(Constants.SPECTRAL_FREQUENCY, "Price", 95, $"{Constants.SPECTRAL_FREQUENCY} price");
    }
}
