namespace ncrGetSomePersonality;
/*
internal class gspRunSettings : MonoBehaviour
{
    public List<gspInput> settings = [
        new gspInput("playername")
        ];
    internal GameObject GSPSettings;
    public gspInput currentSetting;
    internal TMP_InputField input;
    internal TMP_ColorGradient colour;


    internal void Start()
    {
        input = ConsoleScript.instance?.input;
    }

    internal void Set(gspInput playSetting)
    {
        currentSetting = playSetting;
    }

    internal void initInput()
    {
        if (input == null) { return; }
        input.text = "";
        input.onValueChanged.AddListener(str =>
        {
            currentSetting.inputValue = str;
        });
    }
}
public class gspInput
{
    internal string name;
    internal bool boolValue;
    internal float floatValue;
    internal int intValue;
    internal string inputValue;
    internal Color colorValue;

    public gspInput(string name)
    {
        this.name = name;
    }
}
*/