namespace ncrGetSomePersonality;

public class traitFile
{
    public string name;
    public string desc;
    public string[] impliesTraits;
    public bool isDisability;
    public Dictionary<string, string> items;
    public Dictionary<string, string> moodles;
    public Dictionary<string, string> replacements;
    public Dictionary<string, List<string>> dialogue;
    public Dictionary<string, string> buildings;
    public Dictionary<string, List<string>> pauseQuotes;
    public Dictionary<string, List<string>> pauseThoughts;

    public traitFile()
    {
        moodles = [];
        replacements = [];
        items = [];
        dialogue = [];
        isDisability = false;
        buildings = [];
        pauseQuotes = [];
        pauseThoughts = [];
    }
}