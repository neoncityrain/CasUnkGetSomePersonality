namespace ncrGetSomePersonality;

internal class gspRunSettings
{
    public static List<RunSetting> settingTypes = new List<RunSetting>
    {

    };

    //[HarmonyPatch(typeof(RunSettingDisplay), "UpdateSetting")]
    public class updateAndApply
    {
        public static bool isTxt;
        internal static void Prefix(RunSettingDisplay __instance, ref
            Dictionary<string, object> settings)
        {
            isTxt = __instance.settingType.Equals(typeof(RunSettingTextInput));
            if (isTxt)
            {
                settings[__instance.associated.name] =
                    __instance.transform.GetChild(1).GetComponent
                    <TMP_InputField>().name;
            }
        }
    }

    [HarmonyPatch(typeof(RunSettingDisplay), "UpdateSettingDisplay")]
    public class displayUpdate
    {
        private static bool didInitialize;
        internal static void Prefix(RunSettingDisplay __instance,
            Dictionary<string, object> settings)
        {
            if (!didInitialize)
            {
                updateAndApply.isTxt = __instance.settingType.Equals(typeof(
                    RunSettingTextInput));
                didInitialize = true;
                if (updateAndApply.isTxt && __instance.associated is RunSettingTextInput)
                {
                    __instance.transform.GetChild(1).GetComponent<TMP_InputField>().
                        isRichTextEditingAllowed = false;
                    __instance.transform.GetChild(1).GetComponent<TMP_InputField>().
                        onValueChanged.AddListener(delegate
                        {
                            __instance.UpdateSetting(settings);
                        });
                }
            }

            if (updateAndApply.isTxt && __instance.associated is RunSettingTextInput)
            {
                __instance.transform.GetChild(1).GetComponent<TMP_InputField>().
                    SetTextWithoutNotify((string)settings[__instance.associated.name]);
            }
        }
    }
}

public class RunSettingTextInput : RunSetting
{

    public RunSettingTextInput(string name)
    {
        base.name = name;
    }
}