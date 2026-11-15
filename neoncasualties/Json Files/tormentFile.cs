namespace ncrGetSomePersonality;

public class tormentFile
{
    public string name;
    public Dictionary<string, string> items;
    public Dictionary<string, string> moodles;
    public Dictionary<string, string> buildings;
    public Dictionary<string, List<string>> dialogue;
    public Dictionary<string, List<string>> pauseQuotes;
    public Dictionary<string, List<string>> pauseThoughts;

    public tormentFile()
    {
        moodles = [];
        buildings = [];
        items = [];
        dialogue = [];
        pauseQuotes = [];
        pauseThoughts = [];
    }
}