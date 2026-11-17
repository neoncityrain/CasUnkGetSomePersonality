namespace ncrGetSomePersonality;

internal class TooltipHovers
{
    [HarmonyPatch(typeof(PlayerCamera), "HandlePlayerHover")]
    public class playerHovers
    {
        internal static void Postfix(PlayerCamera __instance)
        {
            if (__instance != null && !playingMultiplayer)
            {
                Collider2D collider2D = Physics2D.OverlapPoint(
                        Camera.main.ScreenToWorldPoint(Input.mousePosition),
                        LayerMask.GetMask("Body"));
                if ((bool)collider2D && collider2D.TryGetComponent<Body>(
                    out Body body) && (body.PlayerSelf().npc ||
                    body != __instance.body))
                {
                    string name = body.PlayerSelf().displayName ??
                        GetBaseGSP("stranger");
                    string desc = body.PlayerSelf().desc ??
                        GetBaseGSP("strangerdsc");

                    GlobalDark.main.SetTooltip((name, desc));
                }
            }
        }
    }

    [HarmonyPatch(typeof(Locale), "GetBuilding")]
    public class buildingText
    {
        internal static void Prefix(ref string str)
        {
            if (PlayerCamera.main != null && PlayerCamera.main.body != null &&
                str != null)
            {
                var body = thisPlayer(PlayerCamera.main.body);
                if (body != null)
                {
                    if (personaText != null && personaText.Count > 0 &&
                        body.PlayerSelf().personalityType != null &&
                        personaText.TryGetValue(body.PlayerSelf().personalityType, out var mySona) && mySona.buildings != null)
                    {

                        if (mySona.buildings.Count > 0 && mySona.buildings.TryGetValue(str, out var build))
                        {
                            str = build;
                        }
                    }
                    if (body.PlayerSelf().bodyTraits != null && body.PlayerSelf().bodyTraits.Count > 0)
                    {
                        foreach (string bodytype in body.PlayerSelf().bodyTraits)
                        {
                            if (traitText.TryGetValue(bodytype, out gspJsonLang lan) &&
                                lan.buildings != null && lan.buildings.Count > 0 &&
                                lan.buildings.TryGetValue(str, out var build))
                            {
                                str = build;
                            }
                        }
                    }
                    if (characterText != null && characterText.Count > 0 &&
                        body.PlayerSelf().codeName != null &&
                        characterText.TryGetValue(body.PlayerSelf().codeName, out var myChar) && myChar.buildings != null)
                    {

                        if (myChar.buildings.Count > 0 &&
                            myChar.buildings.TryGetValue(str, out var build))
                        {
                            str = build;
                        }
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(Locale), "GetItem")]
    public class itemText
    {
        internal static void Prefix(ref string str)
        {
            if (PlayerCamera.main != null && PlayerCamera.main.body != null &&
                str != null)
            {
                var body = thisPlayer(PlayerCamera.main.body);
                if (body != null)
                {
                    if (characterText != null && characterText.Count > 0 &&
                        body.PlayerSelf().codeName != null &&
                        characterText.TryGetValue(body.PlayerSelf().codeName, out var myChar) && myChar.items != null)
                    {
                        if (myChar.items.Count > 0 &&
                            myChar.items.TryGetValue(str, out var build))
                        {
                            str = build;
                        }
                    }
                    else if (body.PlayerSelf().bodyTraits != null && body.PlayerSelf().bodyTraits.Count > 0)
                        {
                            foreach (string bodytype in body.PlayerSelf().bodyTraits)
                            {
                                if (traitText.TryGetValue(bodytype, out gspJsonLang lan) &&
                                    lan.items != null && lan.items.Count > 0 &&
                                    lan.items.TryGetValue(str, out var build))
                                {
                                    str = build;
                                }
                            }
                        }
                    else if (personaText != null && personaText.Count > 0 &&
                        body.PlayerSelf().personalityType != null &&
                        personaText.TryGetValue(body.PlayerSelf().personalityType, out var mySona) && mySona.items != null)
                    {

                        if (mySona.items.Count > 0 && mySona.items.TryGetValue(str, out var build))
                        {
                            str = build;
                        }
                    }
                }
            }
        }
    }
}
