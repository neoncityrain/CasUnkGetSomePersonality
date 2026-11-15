namespace ncrGetSomePersonality;

public class baseFile : Language
{
    public Dictionary<string, List<string>> basicAddedDialogue;
    public Dictionary<string, List<string>> pauseThoughts;

    public baseFile()
    {
        basicAddedDialogue = [];
        pauseThoughts = [];
    }
}
