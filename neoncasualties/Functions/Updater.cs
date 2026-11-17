namespace ncrGetSomePersonality;
internal class Updater
{
    [HarmonyPatch(typeof(Body), "HandleBody")]
    public class updater
    {
        internal static void Prefix(Body __instance)
        {
            var body = thisPlayer(__instance);
            if (body != null && body.alive)
            {
                #region Eggs
                if (body.PlayerSelf().codeName == "polarischandran")
                {
                    if (body.hunger > 120f)
                    {
                        body.happiness -= body.hunger * 0.0005f *
                            Time.deltaTime * WorldGeneration.GetRunSettingFloat(
                                "metabolismrate");
                    }
                }
                #endregion

                #region Dirty WoundView
                if (WoundView.view != null && WoundView.view.enabled &&
                    WoundView.view.CustomCall().isDirty)
                {
                    WoundView.view.CustomCall().isDirty = false;
                    WoundView.view.CustomCall().reviseStats();
                }
                else if (WoundView.view != null && !WoundView.view.enabled &&
                    !WoundView.view.CustomCall().isDirty)
                {
                    WoundView.view.CustomCall().isDirty = true;
                }
                #endregion

                #region Autospriter
                try
                {
                    var cam = PlayerCamera.main;
                    if ((Input.GetKey(KeyBinds.GetBind("pause")) && !PauseHandler.main.isPaused &&
                        body.PlayerSelf().personalSprites != null &&
                        currentSpriteCache != null &&
                        !ScrollableText.textExists && !cam.radialOpen &&
                        !cam.tradeMenu.activeSelf && !cam.itemCustomizer.activeSelf &&
                        !cam.craftingPanel.activeSelf && !cam.woundView.activeSelf &&
                        !(bool)MinigameBase.main.currentMinigame) && reloadSpritesOnPause.Value == true
                        )
                    {
                        // if player is CLOSING the pause screen, reload sprites.
                        currentSpriteCache.Clear();
                        body.PlayerSelf().personalSprites.Clear();
                    }
                }
                catch (Exception ex)
                {
                    Log.LogError("Error reloading sprites on pause: " + ex);
                }

                try
                {
                    if (allowAutospriting.Value == true &&
                        body.PlayerSelf().personalSprites.Count <= 0 &&
                        body.PlayerSelf().playerTitle != null)
                    {
                        body.PlayerSelf().fullLoadSprites();
                        //autoSpritePlayer.recolourMe(body);
                    }
                    else if (allowRandomColours.Value == true &&
                        body.PlayerSelf().personalSprites.Count <= 0 &&
                        body.PlayerSelf().playerColours != null &&
                        body.PlayerSelf().playerColours.Count > 0)
                    {
                        //autoSpritePlayer.recolourMe(body);
                    }
                }
                catch (Exception ex)
                {
                    Log.LogError("Autospriter error: " + ex);
                }
                #endregion

                if (playingMultiplayer)
                {
                    multiplayer.handlePersonaForAllPlayers();
                }
                #region Upkeep Traits
                if (body != null && body.PlayerSelf().bodyTraits.Count > 0)
                {

                    #region Disabilities
                    if (!body.PlayerSelf().bodyTraits.Contains("deaf") &&
                        body.hearingLoss >= 99f)
                    {
                        body.PlayerSelf().bodyTraits.Add("deaf");
                        body.traumaAmount += 10f;
                    }
                    else if (body.PlayerSelf().bodyTraits.Contains("deaf") &&
                        body.hearingLoss < 95f)
                    {
                        body.hearingLoss = 100f; // give it some time to reset itself again
                    }

                    if (body.PlayerSelf().bodyTraits.Contains("declawed") &&
                        (body.clawHealth > 0f || body.clawRegrowTime > 0f))
                    {
                        body.clawHealth = 0f;
                        body.clawRegrowTime = 0f;
                    }
                    #endregion


                    if (body.PlayerSelf().bodyTraits.Contains("selfdestructive"))
                    {
                        if (body.harmer.cooldownTime <= 0f &&
                            body.opiateHappiness <= 0f)
                        {
                            body.happiness -= Time.deltaTime * 0.01f;
                            // this is about 3 times lower than the heat/cold unhappiness.
                            // however, it will proc a lot more often and longer than that.
                        }


                        if (body.happiness < -20f && body.totalHappiness >= -75f &&
                                body.harmer.cooldownTime <= 0f)
                        {
                            WoundView.view.CustomCall().setCasualHarmer();
                        }
                        else if (body.totalHappiness < -75f)
                        {
                            WoundView.view.CustomCall().setCasualKiller();
                        }
                        else
                        {
                            WoundView.view.CustomCall().setNormalExercise();
                        }
                    }
                    else if (WoundView.view.CustomCall().exerciseReplaced ||
                        WoundView.view.CustomCall().exerciseReplacedDeath)
                    {
                        // this should never be the case- BUT, if it IS, fix that.
                        WoundView.view.CustomCall().setNormalExercise();
                    }
                }
                #endregion
            }
        }
    }
        

    [HarmonyPatch(typeof(WoundView), "UpdateView")]
    public class viewTweak
    {
        internal static void Postfix(WoundView __instance)
        {
            var body = thisPlayer(__instance.body);
            if (body != null && body.PlayerSelf().codeName != null)
            {
                if (body.PlayerSelf().codeName == "aydcsleet" &&
                body.brainHealth > 20f)
                {
                    int brainScrambler = -(int)body.happiness;
                    if (brainScrambler > 75) { brainScrambler = 75; }
                    if (body.brainHealth > 80)
                    {
                        brainScrambler -= (int)(body.brainHealth * 0.1f);
                    }
                    if (brainScrambler < 7) { brainScrambler = 7; }

                    __instance.brainHealthText.text = (Mathf.RoundToInt(
                        __instance.body.brainHealth - brainScrambler)).ToString() + "%";
                    __instance.brainFill.GetComponent<FilledImagePP>().fillAmount =
                        (__instance.body.brainHealth - brainScrambler) * 0.01f;

                    UITooltip component = __instance.brainHealthText.GetComponentInParent<
                        UITooltip>();
                    if (component != null && component.skipLocale != true)
                    {
                        component.skipLocale = true;
                        component.localeDesc = GetBaseGSP("aydcsleethpbrainhealth");
                        component.tipDesc = GetBaseGSP("aydcsleethpbrainhealth");
                    }
                }
            }

            if (body != null &&
                body.PlayerSelf().personalityType != null)
            {
                body.PlayerSelf().reviseStats();
            }
        }
    }


    [HarmonyPatch(typeof(Body), "WakeUp")]
    public class narcolepticSleepQualityFix
    {
        internal static void Postfix(Body __instance)
        {
            var body = thisPlayer(__instance);
            if (body != null && body.PlayerSelf().bodyTraits != null &&
                body.PlayerSelf().bodyTraits.Count > 0
                )
            {
                if (body.PlayerSelf().bodyTraits.Contains("narcoleptic") &&
                    body.caffeinated <= 0f
                    )
                {
                    body.badSleepAmount = 0f;
                    body.goodSleepTime = Time.time;
                }
            }
        }
    }


    [HarmonyPatch(typeof(PlayerCamera), "HandleScreenShaders")]
    public class shaders
    {
        internal static void Postfix(PlayerCamera __instance)
        {
            if (__instance != null && __instance.body != null)
            {
                var body = thisPlayer(__instance.body);
                if (body.PlayerSelf().codeName == "polarischandran" &&
                    !body.bothEyesGone &&
                    !(bool)body.GetWearable("blindfold"))
                {
                    __instance.damagePass.SetFloat("_EyeDamage",
                        0.1f + body.strokeAmount * 0.5f / 100f);
                }

                if (body.PlayerSelf().bodyTraits != null &&
                    body.PlayerSelf().bodyTraits.Count > 0)
                {
                    if (body.PlayerSelf().bodyTraits.Contains("blind") &&
                        !body.bothEyesGone)
                    {
                        PlayerCamera.main.damagePass.SetFloat("_EyeDamage", 2 +
                        body.strokeAmount * 0.7f / 100f);
                    }

                    if (body.PlayerSelf().bodyTraits.Contains("colorblind") &&
                        body.PlayerSelf().colorblindness > 0f)
                    {
                        PlayerCamera.main.mainScreenPass.SetFloat("_SaturationMult",
                        (PlayerCamera.main.satSmooth * 0.5f) -
                        (float)body.PlayerSelf().colorblindness);
                    }
                }
            }
        }
    }
}