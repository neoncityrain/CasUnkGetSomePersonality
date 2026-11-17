namespace ncrGetSomePersonality;

internal class gspRunSettings : MonoBehaviour
{
    public static List<gspInput> settings = [
        new gspInput("displayname"),
        new gspInput("codename"),
        new gspInput("personalname"),

        new gspInput("forceage"),
        new gspInput("forceheight"),
        new gspInput("forceid"),
        new gspInput("forcevers"),

        new gspInput("addedtraits")
        ];
    internal GameObject GSPSettings;
    public gspInput inputField;


    internal TMP_InputField input;
    internal TMP_ColorGradient colour;


    internal void Start()
    {
        input = ConsoleScript.instance?.input;
    }

    internal void Set(gspInput playSetting)
    {
        inputField = playSetting;
    }

    internal void initInput()
    {
        if (input == null) { return; }
        input.text = "";
        input.onValueChanged.AddListener(str =>
        {
            inputField.inputValue = str;
        });
    }
    public static gspInput FetchSetting(string name)
    {
        return settings.First(x => x.name == name);
    }
}
public class gspInput
{
    internal string name;
    internal bool? boolValue;
    internal float? floatValue;
    internal int? intValue;
    internal string inputValue;
    internal Color? colorValue;
    internal List<string> stringListValue;

    public gspInput(string name)
    {
        this.name = name;
    }
}