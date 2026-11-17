namespace ncrGetSomePersonality;

public class gspJsonLang : Language
{
    #region Universal
    public string codeName;
    public string displayName;
    public string fileType;
    #endregion

    #region To Add
    public bool poolsWithVanilla = true;
    public bool poolsWithBase = true;
    #endregion
    #region Mostly Universal
    public Dictionary<string, string> items;
    public Dictionary<string, List<string>> dialogue;
    public Dictionary<string, List<string>> pauseThoughts;
    public Dictionary<string, string> replacements;
    public Dictionary<string, string> cutsceneTexts;
    #endregion

    #region Characters
    public string personality;
    public Dictionary<string, string[]> personalitySubtypes;
    public string[] bodyTraits;
    public int id;
    public int age;
    public int versionNumber;
    public int heightCm;
    public string woundviewVersionText;
    public string woundviewBodystatText;
    public string woundviewFirmwareText;
    public string endStatsID;
    public int startSTRoffset = 0;
    public int startRESoffset = 0;
    public int startINToffset = 0;
    public Dictionary<string, List<int>> colours;
    #endregion
    #region Traits
    public string[] impliesTraits;
    public bool isDisability;
    #endregion

    #region Fallbacks
    // these are to prevent the json from failing to read if these keys
    // are accidentally left in the files.
    internal string DELETE;
    internal string IGNORE;
    internal string DELETEME;
    internal string COMMENT;
    internal string ABOUTCOLOURS;
    #endregion

    public gspJsonLang()
    {
        personalitySubtypes = [];
        bodyTraits = [];
        moodles = [];
        dialogue = [];
        id = -1;
        age = -1;
        versionNumber = -1;
        heightCm = -1;
        colours = [];
        items = [];
        buildings = [];
        pauseQuotes = [];
        pauseThoughts = [];
        cutsceneTexts = [];
    }

    public void Merge(gspJsonLang merging)
    {
        if (displayName == null && merging.displayName != null)
        { displayName = merging.displayName; }
        if (name == null && merging.name != null) { name = merging.name; }

        if (!merging.poolsWithBase && poolsWithBase)
        { poolsWithBase = false; }
        if (!merging.poolsWithVanilla && poolsWithVanilla)
        { poolsWithVanilla = false; }

        #region universal
        if (items != null && merging.items != null &&
            merging.items.Count != 0)
        {
            foreach (string item in merging.items.Keys)
            {
                if (!items.ContainsKey(item))
                {
                    merging.items.TryGetValue(item, out string itemCalled);
                    items.Add(item, itemCalled);
                }
            }
        }
        if (dialogue != null && merging.dialogue != null &&
            merging.dialogue.Count != 0)
        {
            foreach (string key in merging.dialogue.Keys)
            {
                merging.dialogue.TryGetValue(key, out var dialNew);
                if (!dialogue.ContainsKey(key))
                {
                    dialogue.Add(key, dialNew);
                }
                else
                {
                    dialogue.TryGetValue(key, out var dial);
                    dial.AddRange(dialNew);
                }
            }
        }
        if (pauseQuotes != null && merging.pauseQuotes != null &&
            merging.pauseQuotes.Count != 0)
        {
            foreach (string key in merging.pauseQuotes.Keys)
            {
                merging.pauseQuotes.TryGetValue(key, out var pauseNew);
                if (!pauseQuotes.ContainsKey(key))
                {
                    pauseQuotes.Add(key, pauseNew);
                }
                else
                {
                    pauseQuotes.TryGetValue(key, out var pause);
                    pause.AddRange(pauseNew);
                }
            }
        }
        if (pauseThoughts != null && merging.pauseThoughts != null &&
            merging.pauseThoughts.Count != 0)
        {
            foreach (string key in merging.pauseThoughts.Keys)
            {
                merging.pauseThoughts.TryGetValue(key, out var pauseNew);
                if (!pauseThoughts.ContainsKey(key))
                {
                    pauseThoughts.Add(key, pauseNew);
                }
                else
                {
                    pauseThoughts.TryGetValue(key, out var pause);
                    pause.AddRange(pauseNew);
                }
            }
        }
        if (replacements != null && merging.replacements != null &&
            merging.replacements.Count != 0)
        {
            foreach (string key in merging.replacements.Keys)
            {
                if (!replacements.ContainsKey(key))
                {
                    merging.replacements.TryGetValue(key, out string replacement);
                    replacements.Add(key, replacement);
                }
            }
        }
        #endregion
        #region character
        if (personality == null && merging.personality != null)
        { personality = merging.personality; }
        if (personalitySubtypes != null && merging.personalitySubtypes != null &&
            merging.personalitySubtypes.Count != 0)
        {
            foreach (string key in merging.personalitySubtypes.Keys)
            {
                merging.personalitySubtypes.TryGetValue(key, out var sub);
                if (!personalitySubtypes.ContainsKey(key))
                {
                    personalitySubtypes.Add(key, sub);
                }
                else
                {
                    personalitySubtypes.TryGetValue(key, out var pause);
                    pause.AddRangeToArray(sub);
                }
            }
        }
        if (bodyTraits != null && merging.bodyTraits != null &&
            merging.bodyTraits.Length != 0)
        {
            foreach (string trait in merging.bodyTraits)
            {
                if (!bodyTraits.Contains(trait))
                {
                    bodyTraits.AddToArray(trait);
                }
            }
        }
        if (id == -1 && merging.id != -1) { id = merging.id; }
        if (age == -1 && merging.age != -1) { age = merging.age; }
        if (versionNumber == -1 && merging.versionNumber != -1)
        { versionNumber = merging.versionNumber; }
        if (heightCm == -1 && merging.heightCm != -1)
        { heightCm = merging.heightCm; }

        if (startSTRoffset == 0 && merging.startSTRoffset != 0)
        { startSTRoffset = merging.startSTRoffset; }
        if (startRESoffset == 0 && merging.startRESoffset != 0)
        { startRESoffset = merging.startRESoffset; }
        if (startINToffset == 0 && merging.startINToffset != 0)
        { startINToffset = merging.startINToffset; }

        if (woundviewVersionText == null && merging.woundviewVersionText != null)
        { woundviewVersionText = merging.woundviewVersionText; }
        if (woundviewBodystatText == null && merging.woundviewBodystatText != null)
        { woundviewBodystatText = merging.woundviewBodystatText; }
        if (endStatsID == null && merging.endStatsID != null)
        { endStatsID = merging.endStatsID; }

        if (colours != null && merging.colours != null)
        { 
            foreach (string key in merging.colours.Keys)
            {
                merging.colours.TryGetValue(key, out var intlist);
                if (!colours.Keys.Contains(key))
                {
                    colours.Add(key, intlist);
                }
            }
        }
        #endregion

        #region Traits
        if (impliesTraits != null && merging.impliesTraits != null &&
            merging.impliesTraits.Length != 0)
        {
            foreach (string trait in merging.impliesTraits)
            {
                if (!impliesTraits.Contains(trait))
                {
                    impliesTraits.AddToArray(trait);
                }
            }
        }
        if (!isDisability && merging.isDisability)
        { isDisability = true; }
        #endregion
    }
}