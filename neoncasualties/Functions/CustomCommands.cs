using static UnityEngine.Experimental.Rendering.RayTracingAccelerationStructure;

namespace ncrGetSomePersonality;

[HarmonyPatch(typeof(ConsoleScript), "RegisterAllCommands")]
internal class PreRunFix
{
    internal static void Postfix(ConsoleScript __instance)
    {
        try
        {
            if (ConsoleScript.instance != null)
            {
                ConsoleScript.Commands.Add(
                    new Command("gsp-codename", "Changes the player to the state provided by a specific, fixed codename.",
                    args =>
                    {
                        if (!string.IsNullOrEmpty(args[1]))
                        {
                            var body = thisPlayer(PlayerCamera.main?.body);
                            if (body != null)
                            {
                                body.PlayerSelf().resetCWTs();
                                body.PlayerSelf().returnToVanillaSprites();
                                body.PlayerSelf().codeName = args[1].ToLower();
                                body.PlayerSelf().reviseStats();

                                LogToConsole(__instance, "Player's egg is now: " +
                                    body.PlayerSelf().codeName);
                            }
                            else
                            {
                                gspRunSettings.FetchSetting("codename").inputValue = args[1].ToLower();
                                LogToConsole(__instance, "Upon loading a game, the player's egg string will be: " +
                                    args[1].ToLower());
                            }
                        }
                        else
                        {
                            LogToConsole(__instance, "Can't apply a non-existant string.");
                        }

                    }, null, ("string egg", "Egg value to apply."))); // codename
                ConsoleScript.Commands.Add(
                    new Command("gsp-getpersona", "Returns the player's current or future personality.",
                    args =>
                    {
                        var body = thisPlayer(PlayerCamera.main?.body);
                        if (body != null)
                        {
                            LogToConsole(__instance, "Player's personality string is: " +
                                body.PlayerSelf().personalityType);
                        }
                        else if (gspRunSettings.FetchSetting("forcepersona").inputValue != null)
                        {
                            LogToConsole(__instance, "Player's personality string will be: " + gspRunSettings.FetchSetting("forcepersona").inputValue);
                        }
                        else
                        {
                            LogToConsole(__instance,
                                "Player and run settings were both null.");
                        }
                    }, null)); // get persona
                ConsoleScript.Commands.Add(
                    new Command("gsp-personaswap", "Replaces the player's personality.", (
                        args) =>
                    {
                        var body = thisPlayer(PlayerCamera.main?.body);
                        if (body != null)
                        {
                            body.PlayerSelf().personalityType = args[1].ToLower();
                            LogToConsole(__instance, "Player's personality string has been changed to: " + body.PlayerSelf().personalityType);
                        }
                        else if (gspRunSettings.FetchSetting("forcepersona") != null)
                        {
                            gspRunSettings.FetchSetting("forcepersona").inputValue = args[1].ToLower();
                            LogToConsole(__instance, "When the game is loaded, the player's personality will be: " + args[1]);
                        }
                        else
                        {
                            LogToConsole(__instance,
                                "GSP could not write to player OR to run settings for some reason. Please report this!");
                        }
                    }, null, ("string text", "The name of the personality to set."))); // swap persona
                ConsoleScript.Commands.Add(
                    new Command("gsp-textreload", "Clears and re-initiates all text. " +
                    "May be good to fix errors within json files, but may cause lag.", args =>
                    {
                        reloadAllJsonFiles();
                        LogToConsole(__instance, "Successfully reloaded jsons.");
                    }, null)); // reload text
                ConsoleScript.Commands.Add(
                    new Command("gsp-rename", "Changes what the player character calls themself.", args =>
                    {
                        var body = thisPlayer(PlayerCamera.main?.body);
                        if (args[1] != null)
                        { 
                            if (body != null)
                            {
                                body.PlayerSelf().personalName = args[1];
                                body.PlayerSelf().reviseStats();
                                LogToConsole(__instance, "Player has been renamed to: " + body.PlayerSelf().personalName);
                            }
                            else
                            {
                                gspRunSettings.FetchSetting("personalname")?.inputValue = args[1];
                                LogToConsole(__instance, "Player's personal name will be: " + args[1]);
                            }
                        }
                        else
                        {
                            LogToConsole(__instance, "Can't rename player.");
                        }

                    }, null)); // rename
                ConsoleScript.Commands.Add(
                    new Command("gsp-display", "Changes the name that the WoundView screen displays.", args =>
                    {
                        var body = thisPlayer(PlayerCamera.main?.body);
                        if (args[1] != null)
                        {
                            if (body != null)
                            {
                                body.PlayerSelf().displayName = args[1];
                                body.PlayerSelf().reviseStats();
                                LogToConsole(__instance, "Player's displayed name is now: " + body.PlayerSelf().displayName);
                            }
                            else
                            {
                                gspRunSettings.FetchSetting("displayname")?.inputValue = args[1];
                                LogToConsole(__instance, "Player's display name will be: " + args[1]);
                            }
                        }
                        else
                        {
                            LogToConsole(__instance, "Can't rename player.");
                        }

                    }, null)); // display
                ConsoleScript.Commands.Add(
                    new Command("gsp-addtrait", "Gives the player a specific trait.",
                    args =>
                    {
                        var body = thisPlayer(PlayerCamera.main?.body);
                        if (args[1] != null)
                        {
                            if (body != null)
                            {
                                if (body.PlayerSelf().bodyTraits.Contains(args[1].ToLower()))
                                {
                                    LogToConsole(__instance, "Player already has this trait!");
                                }
                                else
                                {
                                    body.PlayerSelf().addTrait(args[1].ToLower());
                                    body.PlayerSelf().reviseStats();

                                    LogToConsole(__instance, "Added the trait: " + args[1].ToString());
                                }
                            }
                            else
                            {
                                if (gspRunSettings.FetchSetting("addedtraits").stringListValue.Contains(args[1].ToLower()))
                                {
                                    LogToConsole(__instance, args[1] + " is already set to apply to the player upon load.");
                                }
                                else
                                {
                                    gspRunSettings.FetchSetting("addedtraits").stringListValue.AddItem(args[1].ToLower());
                                    LogToConsole(__instance, "When the game is loaded, the player will have this trait added: " + args[1]);
                                }
                            }
                        }
                        else
                        {
                            LogToConsole(__instance, "Can't apply a nonexistant trait to player.");
                        }

                    }, null, ("string trait", "Trait to add."))); // add trait
                ConsoleScript.Commands.Add(
                    new Command("gsp-deletetrait", "Removes a specific trait from the player.",
                    args =>
                    {
                        var body = thisPlayer(PlayerCamera.main?.body);
                        if (args[1] != null)
                        {
                            if (body != null)
                            {
                                if (body.PlayerSelf().bodyTraits.Remove(args[1].ToLower()))
                                {
                                    body.PlayerSelf().reviseStats();
                                    LogToConsole(__instance, "Removed the trait: " + args[1]);
                                }
                                else
                                {
                                    LogToConsole(__instance, "Can't remove trait " + args[1] + " because it wasn't found on the player.");
                                }
                            }
                            else
                            {
                                if (gspRunSettings.FetchSetting("addedtraits").stringListValue.Remove(args[1].ToLower()))
                                {
                                    LogToConsole(__instance, "Removed the pending trait: " + args[1]);
                                }
                                else
                                {
                                    LogToConsole(__instance, "Can't remove trait " + args[1] + " because it wasn't found in the pending run settings.");
                                }
                            }
                        }
                        else
                        {
                            LogToConsole(__instance, "Can't apply trait to player.");
                        }

                    }, null, ("string trait", "Trait to remove."))); // remove trait
                ConsoleScript.Commands.Add(
                    new Command("gsp-changeid", "Changes the player's ID number.", args =>
                    {
                        var body = thisPlayer(PlayerCamera.main?.body);
                        if (args[1] != null)
                        {
                            if (int.TryParse(args[1], out var result))
                            {
                                if (body != null)
                                {
                                    body.PlayerSelf().myWV.cInfo[2] = result;
                                    body.PlayerSelf().reviseStats();
                                    LogToConsole(__instance, "Player's ID is now: " + body.PlayerSelf().idTag);
                                }
                                else
                                {
                                    gspRunSettings.FetchSetting("forceid")?.intValue = result;
                                    LogToConsole(__instance, "Player's ID will be: " + result.ToString());
                                }
                            }
                            else
                            {
                                LogToConsole(__instance, "Invalid integer.");
                            }
                        }
                        else
                        {
                            LogToConsole(__instance, "Can't give player new ID.");
                        }

                    }, null, ("int id", "The ID to give the player."))); // id
                ConsoleScript.Commands.Add(
                    new Command("gsp-changeage", "Changes the player's age.", args =>
                    {
                        var body = thisPlayer(PlayerCamera.main?.body);
                        if (args[1] != null)
                        {
                            if (int.TryParse(args[1], out var result))
                            {
                                if (body != null)
                                {
                                    body.PlayerSelf().myWV.cInfo[1] = result;
                                    body.PlayerSelf().reviseStats();
                                    LogToConsole(__instance, "Player's age is now: " +
                                        body.PlayerSelf().ownerAge);
                                }
                                else
                                {
                                    gspRunSettings.FetchSetting("forceage")?.intValue = result;
                                    LogToConsole(__instance, "Player's age will be: " + result.ToString());
                                }
                            }
                            else
                            {
                                LogToConsole(__instance, "Invalid integer.");
                            }
                        }
                        else
                        {
                            LogToConsole(__instance, "Can't give player new age.");
                        }

                    }, null, ("int age", "The age to make the player."))); // age
                ConsoleScript.Commands.Add(
                    new Command("gsp-changeheight", "Changes the player's height.", args =>
                    {
                        var body = thisPlayer(PlayerCamera.main?.body);
                        if (args[1] != null && body != null)
                        {
                            if (int.TryParse(args[1], out var result))
                            {
                                if (body != null)
                                {
                                    body.PlayerSelf().myWV.cInfo[0] = result;
                                    body.PlayerSelf().reviseStats();
                                    LogToConsole(__instance, "Player's height is now: " + body.PlayerSelf().ownerHeight);
                                }
                                else
                                {
                                    gspRunSettings.FetchSetting("forceheight")?.intValue = result;
                                    LogToConsole(__instance, "Player's height will be: " + result.ToString());
                                }
                            }
                            else
                            {
                                LogToConsole(__instance, "Invalid integer.");
                            }
                        }
                        else
                        {
                            LogToConsole(__instance, "Can't give player new height.");
                        }

                    }, null, ("int height", "The height to make the player."))); // height
                ConsoleScript.Commands.Add(
                    new Command("gsp-changevers", "Changes the player's firmware version.", args =>
                    {
                        var body = thisPlayer(PlayerCamera.main?.body);
                        if (args[1] != null)
                        {
                            if (int.TryParse(args[1], out var result))
                            {
                                if (body != null)
                                {
                                    body.PlayerSelf().myWV.cInfo[3] = result;
                                    body.PlayerSelf().reviseStats();
                                    LogToConsole(__instance, "Player's firmware version is now: " + body.PlayerSelf().ownerVersion);
                                }
                                else
                                {
                                    gspRunSettings.FetchSetting("forcevers")?.intValue = result;
                                    LogToConsole(__instance, "Player's firmware version will be: " + result.ToString());
                                }
                            }
                            else
                            {
                                LogToConsole(__instance, "Invalid integer.");
                            }
                        }
                        else
                        {
                            LogToConsole(__instance, "Can't give player new firmware.");
                        }

                    }, null, ("int vers", "The firmware version to make the player."))); // version
                ConsoleScript.Commands.Add(
                    new Command("gsp-pausethoughts",
                    "Toggles the option for pause quotes to be thoughts instead, or vice versa.",
                    args =>
                    {
                        try
                        {
                            if (pauseQuotesAreThoughts.Value == false)
                            {
                                pauseQuotesAreThoughts.Value = true;
                                LogToConsole(__instance, "Pause quotes are now thoughts.");
                            }
                            else
                            {
                                pauseQuotesAreThoughts.Value = false;
                                LogToConsole(__instance, "Pause thoughts are now quotes.");
                            }
                            gspConfig.Save();
                        }
                        catch (Exception ex)
                        {
                            LogToConsole(__instance, "An error occured: " + ex);
                        }

                    }, null)); // pause thoughts
            }
        }
        catch (Exception ex)
        {
            Log.LogError("Error within consolescript command additions: " + ex);
        }
    }
}