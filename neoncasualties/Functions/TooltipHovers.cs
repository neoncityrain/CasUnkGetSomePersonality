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
}
