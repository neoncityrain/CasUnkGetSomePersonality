namespace ncrGetSomePersonality;
public class GSPUtils
{
    public static ManualLogSource Log;
    internal static Dictionary<string, Sprite> currentSpriteCache = [];
    internal static string gspLocation = Path.GetDirectoryName(
        typeof(PluginSetup).Assembly.Location) ?? Path.Combine(Paths.PluginPath,
            "neoncasualties");
    public static string saveFileName = "saveGSP.sv";
    internal static bool QOLEnabled = false;
    internal static bool playingMultiplayer = false;
    public static Dictionary<string, Sprite[]> savedVanillaSpriteSets = [];
    public static Dictionary<string, Image[]> savedVanillaImageSets = [];
    public static Dictionary<string, List<Eye>> savedVanillaEyeList = [];


    public static void SetHue(Texture2D texture, float hue)
    {
        var colors = texture.GetPixels32();

        for (var i = 0; i < colors.Length; i++)
        {
            Color.RGBToHSV(colors[i], out float _, out float s, out float v);
            if (v > 0f && v < 1f)
            {
                var a = colors[i].a;
                colors[i] = Color.HSVToRGB(hue, s, v);
                colors[i].a = a;
            }
        }

        try
        {
            texture.SetPixels32(colors);
            texture.Apply();
        }
        catch (Exception ex)
        {
            Log.LogDebug("Error setting texture to sprite: " + ex);
        }
    }

    public static void RecolourHue(Texture2D texture, float hue)
    {
        var colors = texture.GetPixels32();

        for (var i = 0; i < colors.Length; i++)
        {
            Color.RGBToHSV(colors[i], out float h, out float s, out float v);
            if (h != hue && s != 0f && v != 0f && v != 1f)
            {
                var a = colors[i].a;
                var newhue = hue + h;
                if (newhue > 1f) { newhue -= 1f; }
                colors[i] = Color.HSVToRGB(newhue, s, v);
                colors[i].a = a;
            }
        }

        try
        {
            texture.SetPixels32(colors);
            texture.Apply();
        }
        catch (Exception ex)
        {
            Log.LogDebug("Error setting texture to sprite: " + ex);
        }
    }

    public static Body thisPlayer(Body og)
    {
        if (playingMultiplayer)
        {
            return multiplayer.multiplayerBody(og) ?? og;
        }
        return og;
    }

    public static void RecolourSprites(Texture2D texture, Color from, Color to)
    {
        var colors = texture.GetPixels32();
        for (var i = 0; i < colors.Length; i++)
        {
            if (colors[i] == from)
            {
                colors[i] = to;
            }
        }
        texture.SetPixels32(colors);
        texture.Apply();
    }
    
    public static void loadAllPersonalSprites(string targetLocation, Body body)
    {
        IEnumerable<string> dir = Directory.EnumerateFiles(targetLocation,
                $"*.png", searchOption: SearchOption.AllDirectories);
        if (dir != null && dir.Count() > 0
            )
        {
            foreach (string path in dir)
            {
                if (path.ToLower().Contains(body.PlayerSelf().codeName.ToLower()))
                {
                    string fileAndFolder = path.Replace(targetLocation +
                    Path.DirectorySeparatorChar.ToString(), " ").ToLower().TrimStart();
                    // Log.LogDebug("Found sprite filename: " + fileAndFolder);

                    body.PlayerSelf().personalSprites[fileAndFolder] =
                        LoadCustomSprite(path);
                }
            }
        }
    }

    public static void reloadAllJsonFiles()
    {
        baseText = [];
        personaText = [];
        characterText = [];
        traitText = [];
        worldText = [];
        LoadGSPJsons();
    }

    internal static void LogToConsole(ConsoleScript cs, string text)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(Time.realtimeSinceStartup);
        cs.logs.Add($"[<alpha=#55>{timeSpan:mm\\:ss}<alpha=#FF>] " + text);
        Log.LogDebug($"CONSOLE LOG RETURNED [{timeSpan:mm\\:ss}] " + text);
        if (cs.logs.Count > 100)
        {
            cs.logs.RemoveAt(0);
        }
        if (cs.active)
        {
            cs.logText.text = string.Join("\n", cs.logs);
        }
    }

    internal static bool assemblyExists(string ass)
    {
        try
        {
            Assembly[] asses = AppDomain.CurrentDomain?.GetAssemblies();

            foreach (Assembly sem in asses)
            {
                if (sem != null && sem.ManifestModule?.ScopeName?.ToString() == ass + ".dll")
                {
                    Log.LogDebug("Successfully found assembly! Was seeking " + ass);
                    return true;
                }
            }
        }
        catch (Exception ex)
        {
            Log.LogDebug("Could not find assembly \"" + ass + "\" and returned an error: "
                + ex);
        }

        return false;
    }

    internal static Sprite LoadCustomSprite(string fileName,
        float pixelsperunit = 0f)
    {
        string file = Path.GetFileNameWithoutExtension(fileName);
        if (currentSpriteCache.ContainsKey(file))
        {
            return currentSpriteCache[file];
        }

        if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(file))
        {
            return null;
        }

        if (!File.Exists(fileName))
        {
            return null;
        }

        byte[] data = File.ReadAllBytes(fileName);
        Texture2D img = new(2, 2, TextureFormat.RGBA32, mipChain: false);
        img.LoadImage(data);
        img.filterMode = FilterMode.Point;
        img.name = file;
        Sprite sprite = Sprite.Create(img, new Rect(
            0f, 0f, img.width, img.height),
            new Vector2(0.5f, 0.5f),
            pixelsperunit == 0f ? 8f : pixelsperunit,
            0u,
            SpriteMeshType.FullRect);
        sprite.name = file.ToLower();
        currentSpriteCache.Add(file, sprite);
        return sprite;
    }

    internal static string getStringFromLimbInt(int limb)
    {
        return limb switch
        {
            0 => "head",
            1 => "chest",
            2 => "hips",
            3 => "upperarm",
            4 => "forearm",
            5 => "hand",
            6 => "upperarm",
            7 => "forearm",
            8 => "hand",
            9 => "thigh",
            10 => "crus",
            11 => "foot",
            12 => "thigh",
            13 => "crus",
            14 => "foot",
            _ => null
        };
    }

    internal static string getSpeciesNumberString(string sawianSpecies)
    {
        if (string.IsNullOrEmpty(sawianSpecies))
        {
            return "01";
        }

        return sawianSpecies.ToLower() switch
        {
            "sawian-orange" => "02",
            "sawian-milky" => "03",
            "sawian-leapy" => "04",
            "sawian-chompy" => "05",
            "sawian-roza" => "06",
            "sawian-voyager" => "07",
            "sawian-baron" => "08",
            "sawian-chik" => "09",
            "sawian-crystal" => "10",
            "sawian-velvet" => "11",
            "sawian-dune" => "12",
            "sawian-shelly" => "13",
            "sawian-icke" => "<corrupt:0xAFDED0>",
            _ => "",
        };
    }
}