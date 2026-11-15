namespace ncrGetSomePersonality;

internal class worldGenSprites
{
    public static Sprite currentLoadChar;


    [HarmonyPatch(typeof(WorldGeneration), "Update")]
    internal class fixTheLoadImageIFuckinGuess
    {
        internal static void Postfix(WorldGeneration __instance)
        {
            if (__instance.generatingWorld &&
                __instance.biomeOverride == WorldGeneration.OverrideSceneType.Tutorial &&
                currentLoadChar != null)
            {
                Image genSprite = __instance.genRects[2].GetComponent<Image>();
                if (genSprite != null && genSprite.sprite != null)
                {
                    try
                    {
                        Vector2 ogsize = genSprite.rectTransform.sizeDelta;
                        genSprite.sprite = currentLoadChar;
                        genSprite.rectTransform.sizeDelta = ogsize;
                    }
                    catch (Exception ex)
                    {
                        Log.LogError("Error applying character sprite to loadscreen: " +
                            ex);
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(WorldGeneration), "InstantiateWorld")]
    internal class chooseWorldgenSprite
    {
        internal static void Prefix(WorldGeneration __instance)
        {
            if (__instance == null) { return; }

            if (__instance.biomeOverride ==
                WorldGeneration.OverrideSceneType.Tutorial)
            {
                currentLoadChar = null;
                if (Directory.Exists(gspLocation))
                {
                    try
                    {
                        IEnumerable<string> dir = Directory.EnumerateFiles(
                            gspLocation, "gensit-*.png",
                            searchOption: SearchOption.AllDirectories);

                        if ((int)Random.Range(0, dir.Count()) > (1 / dir.Count()) &&
                            dir != null &&
                            dir.Count() != 0
                            )
                        {
                            currentLoadChar = LoadCustomSprite(dir.ElementAt((
                                int)Random.Range(0, dir.Count())));
                        }
                        else
                        {
                            currentLoadChar = null;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.LogError("Error applying character sprite to loadscreen: " +
                            ex);
                    }
                }
            }
        }
    }
}