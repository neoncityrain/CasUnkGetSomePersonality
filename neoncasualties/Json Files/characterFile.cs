namespace ncrGetSomePersonality;

public class characterFile
{
    public string codeName;
    public string name;
    public string displayName;
    public string description;
    public string personality;
    public string calloutQuote;
    public Dictionary<string, string> personalitySubtypes;
    public string[] bodyTraits;
    public int id;
    public int age;
    public int versionNumber;
    public int heightCm;
    public Dictionary<string, string> moodles;
    public Dictionary<string, string> items;
    public Dictionary<string, List<string>> dialogue;
    public Dictionary<string, List<int>> colours;
    public Dictionary<string, string> buildings;
    public Dictionary<string, List<string>> pauseQuotes;
    public Dictionary<string, List<string>> pauseThoughts;

    public string woundviewVersionText;
    public string woundviewBodystatText;
    public string woundviewFirmwareText;
    public int startSTRoffset = 0;
    public int startRESoffset = 0;
    public int startINToffset = 0;

    public Dictionary<string, string> cutsceneTexts;

    public string endStatsID;


    public characterFile()
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
}