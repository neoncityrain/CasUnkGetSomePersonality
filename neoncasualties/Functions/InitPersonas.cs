using static WorldGeneration;

namespace ncrGetSomePersonality;

internal class InitPersonas
{
    [HarmonyPatch(typeof(PlayerCamera), "Start")]
    internal static class grabPlacePlayer
    {
        internal static void Postfix(PlayerCamera __instance)
        {
            var body = thisPlayer(__instance.body);
            if (body != null && WoundView.view.cInfo != null &&
                (WorldGeneration.world.totalTraveled <= 0 ||
                string.IsNullOrEmpty(body.PlayerSelf().personalityType))
                )
            {
                body.PlayerSelf().resetCWTs();
                body.PlayerSelf().reviseStats(body);

                #region Traits
                if (body.PlayerSelf().bodyTraits != null &&
                    body.PlayerSelf().bodyTraits.Count > 0)
                {
                    if (body.PlayerSelf().bodyTraits.Contains("selfdestructive"))
                    {
                        body.harmer.cooldownTime = 500f;
                    }
                    if (body.PlayerSelf().bodyTraits.Contains("missingeyeleft") &&
                        !body.eyeGone)
                    {
                        body.eyeGone = true;
                    }
                    if (body.PlayerSelf().bodyTraits.Contains("missingeyeright"))
                    {
                        //body.bothEyesGone = true;
                    }
                }
                #endregion

                string coden = body.PlayerSelf().codeName;
                if (characterText == null || characterText.Count < 1 |
                    body.PlayerSelf().codeName == null || string.IsNullOrEmpty(coden) ||
                    !characterText.ContainsKey(coden))
                {
                    return;
                }
                characterText.TryGetValue(coden, out characterFile myFile);

                body.skills.INT += myFile.startINToffset;
                body.skills.STR += myFile.startSTRoffset;
                body.skills.RES += myFile.startRESoffset;
                body.skills.UpdateExpBoundaries();

                if (coden == ("polarischandran"))
                {
                    body.traumaAmount = 25f;
                    body.happiness = -20f;
                    body.lastHappiness = [-20f];
                }
            }
        }
    }

    [HarmonyPatch(typeof(PlayerCamera), "HandleDeathScreen")]
    internal static class TweakDeathScreen
    {
        internal static void Postfix(PlayerCamera __instance)
        {
            if (__instance.didDeathScreen && __instance.deathStats != null &&
                __instance.body != null
                )
            {
                var body = __instance.body;
                string myNumber = "01";
                if (body.PlayerSelf().bodyTraits != null &&
                    body.PlayerSelf().bodyTraits.Count > 0)
                {
                    if (body.PlayerSelf().bodyTraits.Contains("orange"))
                    { myNumber = "02"; }
                    if (body.PlayerSelf().bodyTraits.Contains("milky"))
                    { myNumber = "03"; }
                    if (body.PlayerSelf().bodyTraits.Contains("leapy"))
                    { myNumber = "04"; }
                    if (body.PlayerSelf().bodyTraits.Contains("chompy"))
                    { myNumber = "05"; }
                    if (body.PlayerSelf().bodyTraits.Contains("roza"))
                    { myNumber = "06"; }
                    if (body.PlayerSelf().bodyTraits.Contains("voyager"))
                    { myNumber = "07"; }
                    if (body.PlayerSelf().bodyTraits.Contains("baron"))
                    { myNumber = "08"; }
                    if (body.PlayerSelf().bodyTraits.Contains("chik"))
                    { myNumber = "09"; }
                    if (body.PlayerSelf().bodyTraits.Contains("crystal"))
                    { myNumber = "10"; }
                    if (body.PlayerSelf().bodyTraits.Contains("velvet"))
                    { myNumber = "11"; }
                    if (body.PlayerSelf().bodyTraits.Contains("dune"))
                    { myNumber = "12"; }
                    if (body.PlayerSelf().bodyTraits.Contains("shelly"))
                    { myNumber = "13"; }
                    if (body.PlayerSelf().bodyTraits.Contains("icke"))
                    { myNumber = "<corrupt:0xAFDED0>"; }
                }

                var id = __instance.deathStats.GetChild(3).GetComponent<
                    TextMeshProUGUI>();
                id.text = $"#{WoundView.specimenId}-SAW-{myNumber}";

                if (!string.IsNullOrEmpty(body.PlayerSelf().displayName))
                {
                    id.text += $" ({body.PlayerSelf().displayName.ToUpper()})";
                }

                if (characterText != null &&
                    !string.IsNullOrEmpty(body.PlayerSelf().codeName) &&
                    characterText.TryGetValue(body.PlayerSelf().codeName, out var file))
                {
                    if (file.endStatsID != null &&
                        !string.IsNullOrEmpty(file.endStatsID))
                    {
                        id.text = file.endStatsID.ToUpper();
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(TraderScript), "OnWillRenderObject")]
    internal class InitTraderPersona
    {
        internal static void Prefix(TraderScript __instance)
        {
            try
            {
                if (Application.isPlaying && __instance != null &&
                    __instance.character == 0 &&
                    __instance.TraderSelf().personalityType == null
                    )
                {
                    Random.State state = Random.state;
                    int rand = (int)(__instance.transform.position.x +
                        __instance.transform.position.y * __instance.transform.position.z);
                    Random.InitState(rand);

                    if (personaText != null && personaText.Keys != null)
                    {
                        __instance.TraderSelf().personalityType =
                        personaText.Keys.ElementAt(Random.Range(0,
                        personaText.Count));
                        Log.LogDebug("Trader personality is: " +
                            __instance.TraderSelf().personalityType);
                    }

                    Random.state = state;
                }
            }
            catch (Exception ex)
            {
                Log.LogError("Error within trader initiation: " + ex);
            }
        }
    }
}
