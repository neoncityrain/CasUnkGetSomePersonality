namespace ncrGetSomePersonality;
internal class GSPConfig
{
    internal static ConfigFile gspConfig;
    internal static ConfigEntry<bool> pauseQuotesAreThoughts;
    internal static ConfigEntry<bool> shouldUseVanillaStrings;
    internal static ConfigEntry<bool> shouldUseGeneralStrings;
    internal static ConfigEntry<bool> allowAutospriting;
    internal static ConfigEntry<bool> allowRandomColours;

    public static void initConfigs()
    {
        gspConfig = new ConfigFile(Paths.ConfigPath + "\\neoncasualties.cfg", true);
        pauseQuotesAreThoughts = gspConfig.Bind(
            "Function", "pauseQuotesAreThoughts", false,
            "When enabled, causes pause quotes to instead pull from the 'pauseThoughts' " +
            "section of applicable json files.");
        shouldUseVanillaStrings = gspConfig.Bind(
            "Function", "shouldUseVanillaStrings", true,
            "Should vanilla dialogue be enabled?\nVanilla lines will still be pulled " +
            "if no other dialogue, including general, is present.");
        shouldUseGeneralStrings = gspConfig.Bind(
            "Function", "shouldUseGeneralStrings", true,
            "Should general GSP dialogue be enabled?\nGeneral lines will still be pulled " +
            "if no other dialogue, including vanilla, is present.");
        allowAutospriting = gspConfig.Bind(
            "Function", "allowAutospriting", true,
            "If sprites should be automatically applied to the player when applicable.");
        allowRandomColours = gspConfig.Bind(
            "Function", "allowRandomColours", true,
            "If new Expies should have randomized eye colour tints.");
    }
}
