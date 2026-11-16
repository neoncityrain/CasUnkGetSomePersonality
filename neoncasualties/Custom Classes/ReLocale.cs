namespace ncrGetSomePersonality;

public static class ReLocale
{
    #region texts
    public static Dictionary<string, baseFile> baseText;
    public static Dictionary<string, personalityFile> personaText;
    public static Dictionary<string, characterFile> characterText;
    public static Dictionary<string, traitFile> traitText;
    public static Dictionary<string, tormentFile> tormentText;
    public static Dictionary<string, worldFile> worldText;
    public static Dictionary<string, systemFile> sysText;
    #endregion

    public static void LoadGSPJsons()
    {
        #region baseLang
        IEnumerable<string> dir = Directory.EnumerateFiles(gspLocation, "*.json",
            searchOption: SearchOption.AllDirectories);

        if (dir.Count() == 0 || dir == null)
        {
            Log.LogError("Could not find the necessary json files.");
            return;
        }

        baseText = [];
        personaText = [];
        sysText = [];
        characterText = [];
        tormentText = [];
        traitText = [];

        foreach (string path in dir)
        {
            string filename = Path.GetFileNameWithoutExtension(path).ToLower();

            if (filename == null ||
                (!path.ToLower().Contains("gspersona") &&
                !path.ToLower().Contains("neoncasualties")) ||
                path.ToLower().Contains("template"))
            {
                // do NOT do a SINGLE thang.
            }
            else if (filename.ToLower() == "base")
            {
                baseFile gsp = JsonConvert.DeserializeObject<baseFile>(
                    File.ReadAllText(path));

                if (gsp != null)
                {
                    string codeName = gsp.name ?? filename;
                    // the name cant be null, so if it IS,
                    // we assign a name to it.
                    baseText.Add(codeName, gsp);
                }
            }
            else if (path.ToLower().Contains("\\personalities\\") ||
                path.ToLower().Contains("\\personality\\") ||
                path.ToLower().Contains("/personalities/") ||
                path.ToLower().Contains("/personality/"))
            {
                personalityFile gsp = JsonConvert.DeserializeObject<
                    personalityFile>(File.ReadAllText(path));
                if (gsp != null)
                {
                    string codeName = gsp.name ?? filename;
                    personaText.Add(codeName, gsp);
                }
            }
            else if (path.ToLower().Contains("\\character\\") ||
                    path.ToLower().Contains("\\characters\\") ||
                    path.ToLower().Contains("/character/") ||
                    path.ToLower().Contains("/characters/"))
            {
                characterFile gsp = JsonConvert.DeserializeObject<
                    characterFile>(File.ReadAllText(path));
                if (gsp != null)
                {
                    if (string.IsNullOrEmpty(gsp.codeName))
                    {
                        Log.LogWarning("No codename is applied for the file at path \"" +
                            path.ToString() +
                            "\". While the json can be read, it may not be properly called." +
                            " Please fix this when you are able.");
                    }
                    string codeName = gsp.codeName ?? filename;
                    characterText.Add(codeName, gsp);
                }
            }
            else if (path.ToLower().Contains("\\plural\\") ||
                path.ToLower().Contains("\\plurality\\") ||
                    path.ToLower().Contains("/plural/") ||
                    path.ToLower().Contains("/plurality/"))
            {
                systemFile gsp = JsonConvert.DeserializeObject<
                    systemFile>(File.ReadAllText(path));
                if (gsp != null)
                {
                    string codeName = gsp.systemName ?? filename;
                    sysText.Add(codeName, gsp);
                }
            }
            else if (path.ToLower().Contains("\\worldstate\\") ||
                path.ToLower().Contains("\\worldstates\\") ||
                    path.ToLower().Contains("/worldstate/") ||
                    path.ToLower().Contains("/worldstates/"))
            {
                worldFile gsp = JsonConvert.DeserializeObject<
                    worldFile>(File.ReadAllText(path));

                if (gsp != null)
                {
                    string codeName = gsp.name ?? filename;
                    worldText.Add(codeName, gsp);
                }
            }
            else if (path.ToLower().Contains("\\trait\\") ||
                path.ToLower().Contains("\\traits\\") ||
                path.ToLower().Contains("/trait/") ||
                path.ToLower().Contains("/traits/"))
            {
                traitFile gsp = JsonConvert.DeserializeObject<
                    traitFile>(File.ReadAllText(path));

                if (gsp != null)
                {
                    string codeName = gsp.name ?? filename;
                    traitText.Add(codeName, gsp);
                }
            }
            else if (path.ToLower().Contains("\\torment\\") ||
                path.ToLower().Contains("\\torments\\") ||
                path.ToLower().Contains("/torment/") ||
                path.ToLower().Contains("/torments/"))
            {
                tormentFile gsp = JsonConvert.DeserializeObject<
                    tormentFile>(File.ReadAllText(path));

                if (gsp != null)
                {
                    string codeName = gsp.name ?? filename;
                    tormentText.Add(codeName, gsp);
                }
            }
            else
            {
                Log.LogWarning("Found a file, but unsure what to do with it: " +
                    path);
            }
        }
        #endregion
    }

    public static string GetBaseGSP(string str)
    {
        if (baseText.Count <= 0)
        {
            LoadGSPJsons();
        }

        if (baseText.TryGetValue("base", out baseFile baselang))
        {
            if (baselang.main.TryGetValue(str, out var main))
            {
                return main;
            }
            if (baselang.moodles.TryGetValue(str, out var moodle))
            {
                return moodle;
            }
        }
        
        return str;
    }

    public static List<string> GetChar(string str)
    {
        if (baseText.Count <= 0 || characterText.Count <= 0)
        {
            LoadGSPJsons();
        }

        if (baseText.Count > 0)
        {
            foreach (baseFile lang in baseText.Values)
            {
                for (int i = 0; i < lang.basicAddedDialogue.Count; i++)
                {
                    if (lang.basicAddedDialogue.TryGetValue(
                        str, out var gspLang))
                    {
                        return gspLang;
                    }
                }
            }
        }

        return Locale.GetCharacter(str);
    }

    #region Moodles
    [HarmonyPatch(typeof(MoodleManager), "AddMoodle")]
    internal class moodleFetch
    {
        internal static void Prefix(MoodleManager __instance, ref int intensity,
            ref string icon, ref string name, ref string desc)
        {
            string path = Path.Combine(gspLocation, "Assets", "moodles");
            if (!__instance.icons.ContainsKey("gspdeclawed"))
            {
                Sprite declawsprite = LoadCustomSprite(
                    Path.Combine(path, "declawed.png"), 32f);
                __instance.icons.Add("gspdeclawed", declawsprite);
            }
            if (!__instance.icons.ContainsKey("gspdeaf"))
            {
                Sprite deafsprite = LoadCustomSprite(
                    Path.Combine(path, "deaf.png"), 32f);
                __instance.icons.Add("gspdeaf", deafsprite);
            }

            var player = thisPlayer(PlayerCamera.main?.body);
            if (player != null && player.PlayerSelf().personalityType != null &&
                !string.IsNullOrEmpty(icon))
            {
                if (baseText.Count < 0 || traitText.Count <= 0 || personaText.Count <= 0)
                {
                    LoadGSPJsons();
                }

                string trueMoodleName = icon;
                string inten = (intensity + 1).ToString();


                #region Get moodle's true / code name
                if (icon == "lastleg")
                {
                    trueMoodleName = "laststand";
                }
                if (icon == "arrythmia")
                {
                    trueMoodleName = "fibrillation";
                    inten = intensity.ToString();
                }
                if (icon == "sad" || icon == "gloomy" || icon == "miserable")
                {
                    trueMoodleName = "depression";
                }
                else if (icon == "happy")
                {
                    inten = (intensity - 3).ToString();
                }
                if (icon == "hearingloss")
                {
                    inten = (intensity + 2).ToString();
                    trueMoodleName = "deaf";
                }
                if (intensity == 3 && icon == "rippedeye")
                {
                    trueMoodleName = "rippedeyes";
                }
                if (icon == "fightorflight")
                {
                    trueMoodleName = "adrenaline";
                }
                if (icon == "stimulants")
                {
                    trueMoodleName = "stimulated";
                }
                if (icon == "confused" && intensity == 3)
                {
                    trueMoodleName = "incapacitated";
                }
                #endregion


                if (player.PlayerSelf().bodyTraits != null &&
                    player.PlayerSelf().bodyTraits?.Count > 0 &&
                    icon == "hearingloss" && trueMoodleName == "deaf" &&
                    player.PlayerSelf().bodyTraits.Contains(
                        "deaf"))
                {
                    intensity = 0;
                    icon = "gspdeaf";
                    name = GetBaseGSP("permanentlydeaf");
                    desc = GetBaseGSP("permanentlydeafdsc");
                }
                if (player.PlayerSelf().bodyTraits != null &&
                    player.PlayerSelf().bodyTraits?.Count > 0 &&
                    icon == "clawdamage" && trueMoodleName == "clawdamage" &&
                    player.PlayerSelf().bodyTraits.Contains(
                        "declawed"))
                {
                    intensity = 0;
                    icon = "gspdeclawed";
                    name = GetBaseGSP("declawed");
                    desc = GetBaseGSP("declaweddsc");
                }

                #region Moodle Grouping and setting
                personalityFile perslang = new();
                characterFile charlang = new();
                if (player.PlayerSelf().personalityType != null)
                {
                    personaText.TryGetValue(player.PlayerSelf().personalityType,
                    out perslang);
                }
                if (player.PlayerSelf().codeName != null)
                {
                    characterText.TryGetValue(player.PlayerSelf().codeName,
                    out charlang);
                }
                string rename = GetRelocaledMoodle(trueMoodleName + inten,
                    perslang ?? null,
                    charlang ?? null,
                    player.PlayerSelf().bodyTraits ?? null,
                    IDnumber: player.PlayerSelf().idTag
                    );
                rename ??= GetRelocaledMoodle(trueMoodleName,
                    perslang ?? null,
                    charlang ?? null,
                    player.PlayerSelf().bodyTraits ?? null,
                    IDnumber: player.PlayerSelf().idTag
                    );
                string redesc = GetRelocaledMoodle(
                    trueMoodleName + inten + "dsc",
                    perslang ?? null,
                    charlang ?? null,
                    player.PlayerSelf().bodyTraits ?? null,
                    IDnumber: player.PlayerSelf().idTag
                    );
                redesc ??= GetRelocaledMoodle(
                    trueMoodleName + "dsc",
                    perslang ?? null,
                    charlang ?? null,
                    player.PlayerSelf().bodyTraits ?? null,
                    IDnumber: player.PlayerSelf().idTag
                    );

                if (!string.IsNullOrEmpty(rename))
                {
                    name = rename;
                }
                if (!string.IsNullOrEmpty(redesc))
                {
                    desc = redesc;
                }
                

                #region my son <3
                if (player.PlayerSelf().codeName != null &&
                    player.PlayerSelf().codeName == "polarischandran" &&
                    characterText.TryGetValue("polarischandran",
                    out characterFile pollang)
                    )
                {
                    if (icon == "hunger" && intensity < 2)
                    {
                        icon = "confused";
                    }
                    if (icon == "rippedeye" &&
                        player.traumaAmount <= 25f && player.eyeGone &&
                        !player.bothEyesGone)
                    {
                        intensity = 0;
                        icon = "trauma";
                        name = Locale.GetMoodle("trauma1");
                        desc = GetRelocaledMoodle("trauma1dsc", character: pollang);
                    }
                }
                #endregion
                #endregion
            }
        }
    }

    public static string GetRelocaledMoodle(string str, personalityFile persona = null,
        characterFile character = null, List<string> traits = null,
        tormentFile torment = null, int? IDnumber = null)
    {
        if (baseText?.Count <= 0 ||
            (characterText?.Count <= 0 && character != null) ||
            (traitText?.Count <= 0 && traits != null) ||
            (personaText?.Count <= 0 && persona != null) ||
            (tormentText?.Count <= 0 && torment != null)
            )
        {
            LoadGSPJsons();
        }

        List<string> potentialMoodles = [];
        if (baseText.TryGetValue("base", out baseFile lan) && baseText != null &&
            lan.moodles != null && lan.moodles.TryGetValue(str, out var baseLang) &&
            baseLang != str && shouldUseGeneralStrings.Value == true)
        {
            potentialMoodles.Add(baseLang);
        }

        if (character != null && character.moodles != null && characterText != null && 
            character.moodles.TryGetValue(str, out var charLang) && charLang != str)
        {
            return charLang; // this should always (or almost always?) override the others.
        }
        if (persona != null && persona?.moodles != null && personaText != null &&
            persona.moodles.TryGetValue(str, out var perLang) && perLang != str)
        {
            potentialMoodles.Add(perLang);
        }
        if (torment != null && torment?.moodles != null && tormentText != null &&
            torment.moodles.TryGetValue(str, out var torLang) && torLang != str)
        {
            potentialMoodles.Add(torLang);
        }
        if (traits != null && traits?.Count > 0 && traitText != null)
        {
            foreach (string trait in traits)
            {
                if (traitText.TryGetValue(trait, out var myTrait) &&
                    myTrait?.moodles != null && myTrait?.moodles?.Count > 0)
                {
                    if (myTrait.moodles.TryGetValue(str, out var traLang) &&
                        traLang != str)
                    {
                        potentialMoodles.Add(traLang);
                    }
                }
            }
        }

        if (potentialMoodles != null && potentialMoodles?.Count > 0)
        {
            if (IDnumber != null)
            {
                Random.State state = Random.state;
                Random.InitState((int)IDnumber);
                string choice = potentialMoodles[Random.Range(0, potentialMoodles.Count)];
                Random.state = state;
                // due to the way random states work, any change to the moodle count
                // can (and quite possibly will!) change the random choice.
                return choice;
            }
            else
            {
                return potentialMoodles.First();
            }
        }
        else if (shouldUseGeneralStrings.Value == false && lan != null &&
            lan.moodles.TryGetValue(str, out var genLang) && genLang != str
            )
        {
                return genLang;
        }

        return null; // return vanilla
    }
    #endregion

    #region Talk
    //[HarmonyPatch(typeof(Talker), "Update")]
    public class soundEffectSwap
    {
        internal static void Prefix(Talker __instance)
        {
            if (__instance.body != null && PlayerCamera.main != null &&
                PlayerCamera.main.body != null &&
                PlayerCamera.main.body == __instance.body &&
                PlayerCamera.main.body.conscious
                )
            {
                var chat = Resources.Load<AudioClip>("Sounds/watchtalk");
                if (__instance.body.PlayerSelf().lastDialogueKey != "" //&&
                    //__instance.body.PlayerSelf().sys != null
                    )
                {
                    var sound = __instance.transform.GetComponent<Sound>;
                    var method = sound.GetMethodInfo();
                    if (method.Attributes.ToString() != null)
                    {
                        Log.LogDebug("Found talktext yayyyyyy! " + method.Attributes.ToString());

                    }
                }
                __instance.body.PlayerSelf().lastDialogueKey = "";
            }
        }
    }


    [HarmonyPatch(typeof(Talker), "Talk", [typeof(List<string>),typeof(Limb),
        typeof(bool),typeof(bool)])]
    public class TalkerTalk
    {
        internal static void Prefix(Talker __instance, ref List<string> lines,
            ref Limb limb)
        {
            try
            {
                if (lines != null && __instance != null && PlayerCamera.main != null &&
                    PlayerCamera.main.body != null && __instance.body != null
                    )
                {
                    if (!__instance.body)
                    {
                        return;
                    }
                    if (baseText?.Count <= 0 || personaText?.Count <= 0 ||
                        traitText?.Count <= 0)
                    {
                        LoadGSPJsons();
                    }

                    #region Initiating Variables
                    List<string> dialoguePool = [];
                    List<string> randomnoises = [];
                    bool isTrader = __instance.trader != null;
                    TraderScript trader = __instance.trader;
                    Body body = thisPlayer(__instance.body);
                    if (body == null)
                    {
                        Log.LogError("Player was null for some reason!");
                        return;
                    }
                    string dialogue = "";
                    // we initiate the dialogue pool. this allows us to pull from
                    // a number of dialogues at once, rather than from only one json file.

                    if (isTrader)
                    {
                        dialogue = trader.TraderSelf().lastDialogueKey;
                    }
                    else
                    {
                        dialogue = body.PlayerSelf().lastDialogueKey;
                    }
                    string persona = isTrader ? trader.TraderSelf().personalityType?.ToLower() :
                        body.PlayerSelf().personalityType?.ToLower();
                    List<string> bodyTraits = isTrader ? trader.TraderSelf().bodyTraits :
                        body.PlayerSelf().bodyTraits;
                    string myName = isTrader ? trader.TraderSelf().ownName :
                        body.PlayerSelf().personalName;

                    if ((isTrader && !string.IsNullOrEmpty(trader.TraderSelf().easterEggType)) ||
                        (!isTrader && !string.IsNullOrEmpty(body.PlayerSelf().codeName)))
                    {
                        myName = isTrader ? trader.TraderSelf().easterEggType :
                        body.PlayerSelf().codeName;
                        // for those with names different than their easter egg types,
                        // since the eggstring is whats recorded in the key of charactertext
                        // with priority over character name as the player sees it.
                    }

                    // string yourName = isTrader ? body.PlayerSelf().ownName : null;
                    #endregion


                    if (string.IsNullOrEmpty(persona) || string.IsNullOrEmpty(dialogue))
                    {
                        return;
                    }

                    if (dialogue == "shouldnotspeak")
                    {
                        lines = [""];
                        return;
                    }

                    if (bodyTraits.Contains("deaf") &&
                        dialogue == "loud")
                    {
                        dialogue = "deafloud";
                    }

                    try
                    {
                        #region Body Trait Pool
                        if (bodyTraits != null && bodyTraits.Count > 0)
                        {
                            foreach (string bodytype in bodyTraits)
                            {
                                if (traitText.TryGetValue(bodytype, out traitFile lan) &&
                                    lan.dialogue != null && lan.dialogue.Count > 0)
                                {
                                    if (lan.dialogue.TryGetValue(
                                        dialogue, out var bodyPool))
                                    {
                                        dialoguePool.AddRange(bodyPool);
                                    }
                                    if (lan.dialogue.TryGetValue(
                                        "randomnoises", out var bodysoundPool))
                                    {
                                        randomnoises.AddRange(bodysoundPool);
                                    }
                                }
                            }
                        }
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        Log.LogError("Error in body trait pool: " + ex);
                    }

                    try
                    {
                        #region Persona Lines
                        if (personaText != null &&
                            personaText.Count > 0 && persona != null &&
                            personaText.TryGetValue(persona, out var mySona) &&
                            mySona.dialogue != null)
                        {
                            if (mySona.dialogue.TryGetValue(dialogue, out var personaPool))
                            {
                                dialoguePool.AddRange(personaPool);
                            }
                            if (mySona.dialogue.TryGetValue(
                                "randomnoises", out var personasoundPool))
                            {
                                randomnoises.AddRange(personasoundPool);
                            }
                        }
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        Log.LogError("Error in personality pool: " + ex);
                    }

                    try
                    {
                        #region Name Pool
                        if (characterText != null &&
                            characterText.Count > 0 && myName != null &&
                            characterText.TryGetValue(myName, out var myCharacter) &&
                            myCharacter.dialogue != null)
                        {
                            if (myCharacter.dialogue.TryGetValue(
                                    dialogue, out var namedPool))
                            {
                                dialoguePool.AddRange(namedPool);
                            }
                            if (myCharacter.dialogue.TryGetValue(
                                "randomnoises", out var namedsoundPool))
                            {
                                randomnoises.AddRange(namedsoundPool);
                            }
                        }
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        Log.LogError("Error in name pool: " + ex);
                    }

                    try
                    {
                        // we only want the vanilla and general lines if there arent
                        // enough OTHER lines in the dialogue pool.
                        // we check if theres enough on both, so it adds general lines
                        // BEFORE vanilla (as vanilla dialogue has been seen a lot)
                        #region General Lines
                        if (baseText != null &&
                            shouldUseGeneralStrings.Value == true &&
                            dialoguePool.Count < 100 &&
                            baseText.TryGetValue(
                            "base", out baseFile lang) &&
                            lang?.basicAddedDialogue != null &&
                            lang?.basicAddedDialogue?.Count > 0)
                        {
                                if (lang.basicAddedDialogue.TryGetValue(
                                    dialogue, out var genPool))
                                {
                                    dialoguePool.AddRange(genPool);
                                }
                                if (lang.basicAddedDialogue.TryGetValue(
                                    "randomnoises", out var soundPool))
                                {
                                    randomnoises.AddRange(soundPool);
                                }
                        }
                        #endregion

                        #region Vanilla Lines
                        if (((dialoguePool.Count < 100 &&
                            shouldUseVanillaStrings.Value == true) ||
                            (dialoguePool.Count == 0 &&
                            shouldUseVanillaStrings.Value == false)) &&
                            Locale.currentLang.character[0].TryGetValue(
                            dialogue, out var vanillaDialogue))
                        {
                            foreach (string text in vanillaDialogue)
                            {
                                dialoguePool.Add(text);
                            }
                        }
                        #endregion

                        #region General Lines Backup
                        if (baseText.TryGetValue(
                            "base", out baseFile gen) &&
                            gen?.basicAddedDialogue != null &&
                            gen?.basicAddedDialogue?.Count > 0 &&
                            shouldUseGeneralStrings.Value == false &&
                            dialoguePool.Count <= 0)
                        {
                            if (gen.basicAddedDialogue.TryGetValue(
                                    dialogue, out var genPool))
                            {
                                dialoguePool.AddRange(genPool);
                            }
                            if (gen.basicAddedDialogue.TryGetValue(
                                "randomnoises", out var soundPool))
                            {
                                randomnoises.AddRange(soundPool);
                            }
                        }
#endregion
                    }
                    catch (Exception ex)
                    {
                        Log.LogError("Error in general/vanilla pool: " + ex);
                    }

                    try
                    {
                        dialoguePool ??= tweakMe(dialoguePool,
                            randomnoises ?? null,
                            limb ?? null,
                            body ?? null,
                            trader ?? null
                            );
                    }
                    catch (Exception ex)
                    {
                        Log.LogError("Error tweaking strings: " + ex);
                    }

                    if (dialoguePool != null && lines != dialoguePool)
                    {
                        // Log.LogDebug((isTrader ? "Trader " : "Player ") + "reading dialogue: " + dialogue);
                        lines = dialoguePool; // transfer to the vanilla lines.
                    }
                }
            }
            catch (Exception ex)
            {
                Log.LogError("Issue found within talk interception: " + ex);
            }
        }
    }
    #endregion

    #region Pause Quotes
    [HarmonyPatch(typeof(PauseHandler), "Update")]
    public class additionalPauseQuotes()
    {
        internal static string lastpausequote;
        private static bool pauseProcessing;

        internal static void Postfix(PauseHandler __instance)
        {
            #region verifyShouldReplace
            try
            {
                if (__instance.pauseContainer.activeSelf &&
                (__instance == null || PlayerCamera.main == null || baseText == null ||
                PlayerCamera.main.body == null || (__instance.pauseProgress <= 0f &&
                lastpausequote != __instance.pauseText.text) ||
                pauseProcessing ||
                (__instance.pauseProgress > 0f &&
                lastpausequote == __instance.pauseText.text)))
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                Log.LogError("Issue with pausetext return: " + ex);
                return;
            }
            pauseProcessing = true;
            #endregion

            if (baseText?.Count <= 0 || personaText?.Count <= 0 || traitText?.Count <= 0)
            {
                LoadGSPJsons();
            }

            var player = thisPlayer(PlayerCamera.main.body);
            if (player == null)
            {
                Log.LogError("Player was null.");
                return;
            }
            else if ((bool)player.mindWipe && player.mindWipe.active &&
                pauseQuotesAreThoughts != null &&
                pauseQuotesAreThoughts.Value == true)
            {
                if (lastpausequote != "...")
                {
                    lastpausequote = "...";
                }
                __instance.pauseText.text = "...";
                return;
            }
            personalityFile perslang = new();
            characterFile charlang = new();

            var self = player.PlayerSelf();

            baseText.TryGetValue("base", out baseFile baselang);
            if (self.personalityType != null)
            {
                personaText.TryGetValue(self.personalityType, out perslang);
            }
            if (self.codeName != null)
            {
                characterText.TryGetValue(self.codeName, out charlang);
            }

            #region pauseQuoteType
            string type = "normal";
            if (WorldGeneration.world.biomeOverride ==
                WorldGeneration.OverrideSceneType.Tutorial)
            {
                type = "normal";
            }
            else if ((bool)EndLabScript.instance &&
                EndLabScript.instance.doingEscape)
            {
                type = "labescape";
            }
            else if (PlayerCamera.main.body.hasCargo)
            {
                type = "cargoembedded";
            }
            else if (PlayerCamera.main.threatMusicTime > 0f)
            {
                type = ((PlayerCamera.main.currentThreatTheme != 10 &&
                    PlayerCamera.main.currentThreatTheme != 15) ? "combat" :
                    "combatelder");
            }
            else if (PlayerCamera.main.body.isDying)
            {
                type = "dying";
            }
            else if (PlayerCamera.main.body.totalHappiness > 50f)
            {
                type = "happy";
            }
            else if (PlayerCamera.main.body.totalHappiness <= -50f)
            {
                type = "depressed";
            }
            #endregion

            string newpause;
            if (pauseQuotesAreThoughts != null &&
                pauseQuotesAreThoughts.Value == true)
            {
                newpause = GetPauseThoughts(type,
                    baselang,
                    perslang ?? null,
                    charlang ?? null,
                    self.bodyTraits ?? null
                    );

                if (!string.IsNullOrEmpty(newpause))
                {
                    __instance.pauseText.text = newpause;

                    if (self.playerColours != null &&
                        self.playerColours.Count > 0 &&
                        self.thoughtColour != null &&
                        self.thoughtColour != new Color() &&
                        self.thoughtColour != __instance.pauseText.color)
                    {
                        __instance.pauseText.color = self.thoughtColour;
                    }
                    else if (self.playerColours != null &&
                        self.playerColours.Count > 0 &&
                        self.playerColours.TryGetValue(
                        "text", out Color pausecol) &&
                        pausecol != __instance.pauseText.color)
                    {
                        if (pausecol.r < 0.1f || pausecol.b < 0.1f ||
                        pausecol.g < 0.1f)
                        {
                            pausecol.r += 0.1f;
                            pausecol.b += 0.1f;
                            pausecol.g += 0.1f;
                        }
                        if (pausecol.r > 1f) { pausecol.r = 1f; }
                        if (pausecol.g > 1f) { pausecol.g = 1f; }
                        if (pausecol.b > 1f) { pausecol.b = 1f; }

                        self.thoughtColour = pausecol;
                        __instance.pauseText.color = pausecol;
                    }
                }
            }
            else
            {
                newpause = GetRelocaledPause(type,
                    baselang,
                    perslang ?? null,
                    charlang ?? null,
                    self.bodyTraits ?? null
                    );
            }


            if (!string.IsNullOrEmpty(newpause))
            {
                lastpausequote = newpause;
                __instance.pauseText.text = newpause;
                pauseProcessing = false;
            }
        }
    }


    public static string GetPauseThoughts(string type,
        baseFile baselang = null,
        personalityFile persona = null, characterFile character = null,
        List<string> traits = null, tormentFile torment = null)
    {
        if ((baseText?.Count <= 0 && baselang != null) ||
        (characterText?.Count <= 0 && character != null) ||
        (traitText?.Count <= 0 && traits != null) ||
        (personaText?.Count <= 0 && persona != null) ||
        (tormentText?.Count <= 0 && torment != null)
        )
        {
            LoadGSPJsons();
        }

        List<string> potentialPauseThought = [];

        try
        {
            #region Base thoughts
            if (baselang != null && baselang?.pauseThoughts != null &&
                baselang?.pauseThoughts?.Count > 0 &&
                shouldUseGeneralStrings.Value == true &&
                baselang.pauseThoughts.TryGetValue(type, out List<string> basepause))
            {
                potentialPauseThought.AddRange(basepause);
            }
            if (character != null && character?.pauseThoughts != null &&
                character?.pauseThoughts?.Count > 0 &&
                character.pauseThoughts.TryGetValue(type, out List<string> charpause))
            {
                potentialPauseThought.AddRange(charpause);
            }
            if (persona != null && persona?.pauseThoughts != null &&
                persona?.pauseThoughts?.Count > 0 &&
                persona.pauseThoughts.TryGetValue(type, out List<string> sonapause))
            {
                potentialPauseThought.AddRange(sonapause);
            }
            if (traits != null && traits?.Count > 0)
            {
                foreach (string trait in traits)
                {
                    if (traitText.TryGetValue(trait, out traitFile file) &&
                        file.pauseThoughts != null && file.pauseThoughts.Count > 0 &&
                        file.pauseThoughts.TryGetValue(type, out List<string> traitpause))
                    {
                        potentialPauseThought.AddRange(traitpause);
                    }
                }
            }
            if (torment != null && torment?.pauseThoughts != null &&
                torment?.pauseThoughts?.Count > 0 &&
                torment.pauseThoughts.TryGetValue(type, out List<string> torpause))
            {
                potentialPauseThought.AddRange(torpause);
            }
            #endregion
        }
        catch (Exception ex)
        {
            Log.LogError("Error collecting pause thoughts: " + ex);
        }

        #region Not enough texts
        if (potentialPauseThought != null && potentialPauseThought.Count < 20)
        {
            potentialPauseThought.AddRange(
                fetchDialogueForAllVariables("seeGravel",
                baselang, persona, character, traits, torment));
            potentialPauseThought.AddRange(
                fetchDialogueForAllVariables("sadresponse",
                baselang, persona, character, traits, torment));
            potentialPauseThought.AddRange(
                fetchDialogueForAllVariables("depressedresponse",
                baselang, persona, character, traits, torment));
        }
        if (potentialPauseThought.Count <= 1 &&
            baselang != null && baselang?.pauseThoughts != null &&
            baselang?.pauseThoughts?.Count > 0 &&
            shouldUseGeneralStrings.Value == false &&
            baselang.pauseThoughts.TryGetValue(type, out List<string> genpause))
        {
            potentialPauseThought.AddRange(genpause);
        }
        #endregion
        if (potentialPauseThought != null && potentialPauseThought.Count > 1)
        {
            loop:
            string newquote = potentialPauseThought.PickRandom();
            if (additionalPauseQuotes.lastpausequote == newquote)
            {
                goto loop;
            }
            return newquote;
        }
        else
        {
            Log.LogWarning("Could not find pause thoughts!");
        }

        return "No thoughts. Head empty.";
    }

    public static string GetRelocaledPause(string type,
        baseFile baselang = null,
        personalityFile persona = null, characterFile character = null,
        List<string> traits = null, tormentFile torment = null)
    {
        if ((baseText?.Count <= 0 && baselang != null) ||
            (characterText?.Count <= 0 && character != null) ||
            (traitText?.Count <= 0 && traits != null) ||
            (personaText?.Count <= 0 && persona != null) ||
            (tormentText?.Count <= 0 && torment != null)
            )
        {
            LoadGSPJsons();
        }

        List<string> potentialPauseText = [];
        if (baselang != null && baselang?.pauseQuotes != null &&
            baselang?.pauseQuotes?.Count > 0 &&
            shouldUseGeneralStrings.Value == true &&
            baselang.pauseQuotes.TryGetValue(type, out List<string> basepause))
        {
            potentialPauseText.AddRange(basepause);
        }
        if (character != null && character?.pauseQuotes != null &&
            character?.pauseQuotes.Count > 0 &&
            character.pauseQuotes.TryGetValue(type, out List<string> charpause))
        {
            potentialPauseText.AddRange(charpause);
        }
        if (persona != null && persona?.pauseQuotes != null &&
            persona?.pauseQuotes?.Count > 0 &&
            persona.pauseQuotes.TryGetValue(type, out List<string> sonapause))
        {
            potentialPauseText.AddRange(sonapause);
        }
        if (traits != null && traits?.Count > 0)
        {
            foreach (string trait in traits)
            {
                if (traitText.TryGetValue(trait, out traitFile file) &&
                    file.pauseQuotes != null && file.pauseQuotes.Count > 0 &&
                    file.pauseQuotes.TryGetValue(type, out List<string> traitpause))
                {
                    potentialPauseText.AddRange(traitpause);
                }
            }
        }
        if (torment != null && torment?.pauseQuotes != null &&
            torment?.pauseQuotes?.Count > 0 &&
            torment.pauseQuotes.TryGetValue(type, out List<string> torpause))
        {
            potentialPauseText.AddRange(torpause);
        }
        if (Locale.currentLang != null && Locale.currentLang.pauseQuotes != null &&
            (shouldUseVanillaStrings.Value == true || potentialPauseText.Count < 15) &&
            Locale.currentLang.pauseQuotes.TryGetValue(type, out List<string> normpause))
        {
            potentialPauseText.AddRange(normpause);
        }

        if (potentialPauseText != null && potentialPauseText?.Count > 1)
        {
            loop:
            string newquote = potentialPauseText.PickRandom();
            if (additionalPauseQuotes.lastpausequote == newquote)
            {
                goto loop;
            }
            return newquote;
        }

        return "Ya done goofed."; // return vanilla
    }
    #endregion

    #region FetchText
    public static List<string> fetchDialogueForAllVariables(string text,
        baseFile baselang = null,
        personalityFile persona = null,
        characterFile character = null,
        List<string> traits = null,
        tormentFile torment = null)
    {
        List<string> dialogueCollected = [];
        if (baselang != null && baselang?.basicAddedDialogue != null &&
            shouldUseGeneralStrings.Value == true &&
            baselang.basicAddedDialogue.TryGetValue(
                text, out var baseT))
        {
            dialogueCollected.AddRange(baseT);
        }
        if (character != null && character?.dialogue != null &&
            character.dialogue.TryGetValue(text, out var characterT))
        {
            dialogueCollected.AddRange(characterT);
        }
        if (persona != null && persona?.dialogue != null &&
            persona.dialogue.TryGetValue(text, out var personaT))
        {
            dialogueCollected.AddRange(personaT);
        }
        if (traits != null && traits?.Count > 0)
        {
            foreach (string trait in traits)
            {
                if (traitText.TryGetValue(trait, out traitFile file) &&
                    file.dialogue != null && file.dialogue.TryGetValue(
                        text, out var traitT))
                {
                    dialogueCollected.AddRange(traitT);
                }
            }
        }
        if (torment != null && torment?.dialogue != null &&
            torment.dialogue.TryGetValue(text, out var tormentT))
        {
            dialogueCollected.AddRange(tormentT);
        }

        return dialogueCollected;
    }
    #endregion

    #region String Tweaker
    public static List<string> tweakMe(List<string> dialoguePool,
        List<string> randomnoises = null,
        Limb limb = null, Body body = null, TraderScript trader = null)
    {
        Log.LogDebug("Attempting to tweak dialogue pool. Pool is: " + dialoguePool);
        List<string> tweakedPool = [];
        foreach (string potentialDialogue in dialoguePool)
        {
            string tweakString = potentialDialogue;

            if ((bool)limb && !(bool)trader)
            {
                tweakString = tweakString.Replace("<limb>",
                    limb.shortName.ToLower());
                tweakString = tweakString.Replace("<Limb>",
                    limb.shortName);
                tweakString = tweakString.Replace("<LIMB>",
                    limb.shortName.ToUpper());
            }

            var myName = (bool)trader ? trader.TraderSelf().ownName :
                body.PlayerSelf().personalName;
            if (myName != null)
            {
                tweakString = tweakString.Replace("<name>",
                    myName);
                tweakString = tweakString.Replace("<NAME>",
                    myName.ToUpper());
            }

            var bodyTraits = (bool)trader ? trader.TraderSelf().bodyTraits :
                body.PlayerSelf().bodyTraits;

            if (randomnoises != null && randomnoises.Count > 0)
            {
                if (!bodyTraits.Contains("mute"))
                {
                    tweakString = tweakString.Replace("<noise>",
                    randomnoises.PickRandom());
                }
                else
                {
                    tweakString = tweakString.Replace("<noise>",
                        "...");
                }
            }
                
            if (bodyTraits.Count > 0)
            {
                if (bodyTraits.Contains("mute") &&
                    !tweakString.Contains("["))
                {
                    tweakString = "[" + tweakString + "]";
                }
                else if (bodyTraits.Contains("mute") &&
                    !tweakString.Contains("("))
                {
                    tweakString = "(" + tweakString + ")";
                }

                if (bodyTraits.Contains("casecapslock"))
                {
                    tweakString = tweakString.ToUpper();
                }
                else if (bodyTraits.Contains("casenoshift"))
                {
                    tweakString = tweakString.ToLower();
                }

                foreach (string currentTrait in bodyTraits)
                {
                    if (traitText.TryGetValue(currentTrait, out traitFile trait))
                    {
                        for (int i = 0; trait.replacements.Count > i; i++)
                        {
                            if (tweakString.Contains(
                                trait.replacements.Keys.ElementAt(i)))
                            {
                                tweakString.Replace(trait.replacements.Keys.ElementAt(i),
                                    trait.replacements.Values.ElementAt(i));
                                if (!tweakedPool.Contains(tweakString))
                                {
                                    tweakedPool.Add(tweakString.ToString());
                                }
                            }
                        }
                            
                    }
                }
            }


            Log.LogDebug("Attempting to tweak dialogue pool. Dialogue after tweaks is: " + tweakString);
            if (!tweakedPool.Contains(tweakString))
            {
                tweakedPool.Add(tweakString.ToString());
            }
        }
        return tweakedPool;
    }
    #endregion

    #region Ending Cutscenes
    [HarmonyPatch(typeof(Locale), "GetOther")]
    public class endingSceneText()
    {
        internal static void Prefix(ref string str)
        {
            if (PlayerCamera.main?.body == null)
            {
                return;
            }
            var body = thisPlayer(PlayerCamera.main.body);

            if ((!WorldGeneration.world.cutsceneActive &&
                !str.Contains("endscreen")) ||
                (characterText == null && personaText == null))
            {
                return;
            }

            try
            {
                var title = body.PlayerSelf().playerTitle;
                if (characterText != null && title == body.PlayerSelf().codeName && characterText.Keys.Contains(title) && characterText.TryGetValue(title, out var charfile))
                {
                    if (charfile.cutsceneTexts != null &&
                        charfile.cutsceneTexts.TryGetValue(str, out var newScene))
                    {
                        str = newScene;
                    }
                }
                if (personaText != null && title == body.PlayerSelf().personalityType && personaText.Keys.Contains(title) && personaText.TryGetValue(title, out var persfile))
                {
                    if (persfile.cutsceneTexts != null &&
                        persfile.cutsceneTexts.TryGetValue(str, out var newScene))
                    {
                        str = newScene;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.LogError("Issue applying cutscene text: " + ex);
            }
        }
    }
    #endregion
}