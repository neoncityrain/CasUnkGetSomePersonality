namespace ncrGetSomePersonality;

public class TalkStringCollectors
{
    [HarmonyPatch(typeof(Locale), "GetCharacter", [typeof(string),typeof(int)])]
    public class TraderTalkStringCollector
    {
        internal static void Prefix(string str, int chr)
        {
            if ((str.StartsWith("trader") ||
                (str.EndsWith("response") && str != "swingresponse")) &&
                chr < 2)
            {
                var body = thisPlayer(PlayerCamera.main?.body);
                PlayerCamera.main?.lastTrader?.TraderSelf().lastDialogueKey = str;
            }
        }
    }

    [HarmonyPatch(typeof(Locale), "GetCharacter", [typeof(string)])]
    public class PlayerTalkStringCollector
    {
        internal static void Prefix(string str)
        {
            // as this is only used for the player, it should be LOGGED for the player.
            if (PlayerCamera.main?.body != null)
            {
                var body = thisPlayer(PlayerCamera.main?.body);
                body.PlayerSelf().lastDialogueKey = str;
            }
        }
    }
}