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
                    new Command("gsp-egg", "Changes the player to a specific codename.",
                    args =>
                    {
                        var body = thisPlayer(PlayerCamera.main?.body);
                        if (args[1] != null && body != null)
                        {
                            body.PlayerSelf().resetCWTs();
                            body.PlayerSelf().returnToVanillaSprites();
                            body.PlayerSelf().codeName = args[1].ToString();
                            body.PlayerSelf().reviseStats();

                            LogToConsole(__instance, "Player's egg is now: " +
                                body.PlayerSelf().codeName);
                        }
                        else
                        {
                            LogToConsole(__instance, "Can't apply egg.");
                        }

                    }, null, ("string egg", "Egg value to apply."))); // eggs
                ConsoleScript.Commands.Add(
                    new Command("gsp-getpersona", "Returns the player's current personality.",
                    args =>
                    {
                        var body = thisPlayer(PlayerCamera.main?.body);
                        if (body != null)
                        {
                            LogToConsole(__instance, "Player's personality string is: " +
                                body.PlayerSelf().personalityType);
                        }
                        else
                        {
                            LogToConsole(__instance,
                                "Player was null, so the personality cannot be read.");
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
                            LogToConsole(__instance, "Player's personality string has" +
                                "changed into: " + body.PlayerSelf().personalityType);
                        }
                        else
                        {
                            LogToConsole(__instance,
                                "Player was null, so the personality cannot be read.");
                        }
                    }, null, ("string text", "The name of the personality to set."))); // swap persona
                ConsoleScript.Commands.Add(
                    new Command("gsp-textReload", "Clears and re-initiates all text. " +
                    "May be good to fix errors within json files, but may cause lag.", args =>
                    {
                        reloadAllJsonFiles();
                        LogToConsole(__instance, "Successfully reloaded jsons.");
                    }, null)); // reloadText
                ConsoleScript.Commands.Add(
                    new Command("gsp-rename", "Renames the player.", args =>
                    {
                        var body = thisPlayer(PlayerCamera.main?.body);
                        if (args[1] != null && body != null)
                        {
                            body.PlayerSelf().personalName = args[1].ToString();
                            body.PlayerSelf().reviseStats();
                            LogToConsole(__instance, "Player has been renamed to: " +
                                body.PlayerSelf().personalName);
                        }
                        else
                        {
                            LogToConsole(__instance, "Can't rename player.");
                        }

                    }, null)); // rename
                ConsoleScript.Commands.Add(
                    new Command("gsp-addtrait", "Gives the player a specific trait.",
                    args =>
                    {
                        var body = thisPlayer(PlayerCamera.main?.body);
                        if (args[1] != null && body != null)
                        {
                            if (body.PlayerSelf().bodyTraits.Contains(args[1]))
                            {
                                LogToConsole(__instance, "Player already has this trait!");
                            }
                            else
                            {
                                body.PlayerSelf().addTrait(args[1].ToString());
                                body.PlayerSelf().reviseStats();

                                LogToConsole(__instance, "Added the trait: " + args[1].ToString());
                            }
                        }
                        else
                        {
                            LogToConsole(__instance, "Can't apply trait to player.");
                        }

                    }, null, ("string trait", "Trait to add."))); // add trait
                ConsoleScript.Commands.Add(
                    new Command("gsp-deletetrait", "Removes a specific trait from the player.",
                    args =>
                    {
                        var body = thisPlayer(PlayerCamera.main?.body);
                        if (args[1] != null && body != null)
                        {
                            if (body.PlayerSelf().bodyTraits.Remove(args[1].ToString()))
                            {
                                body.PlayerSelf().reviseStats();
                                LogToConsole(__instance, "Removed the trait: " + args[1].ToString());
                            }
                            else
                            {
                                LogToConsole(__instance, "Can't remove trait " + args[1].ToString() +
                                    " because it wasn't found on the player.");
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
                        if (args[1] != null && body != null)
                        {
                            if (int.TryParse(args[1], out var result))
                            {
                                body.PlayerSelf().myWV.cInfo[2] = result;
                                body.PlayerSelf().reviseStats();
                                LogToConsole(__instance, "Player's ID is now: " +
                                    body.PlayerSelf().idTag);
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
                        if (args[1] != null && body != null)
                        {
                            if (int.TryParse(args[1], out var result))
                            {
                                body.PlayerSelf().myWV.cInfo[1] = result;
                                body.PlayerSelf().reviseStats();
                                LogToConsole(__instance, "Player's age is now: " +
                                    body.PlayerSelf().ownerAge);
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
                                body.PlayerSelf().myWV.cInfo[0] = result;
                                body.PlayerSelf().reviseStats();
                                LogToConsole(__instance, "Player's height is now: " +
                                    body.PlayerSelf().ownerHeight);
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
                        if (args[1] != null && body != null)
                        {
                            if (int.TryParse(args[1], out var result))
                            {
                                body.PlayerSelf().myWV.cInfo[3] = result;
                                body.PlayerSelf().reviseStats();
                                LogToConsole(__instance, "Player's firmware is now: " +
                                    body.PlayerSelf().ownerVersion);
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
                    "Toggles the option for pause quotes to be thoughts instead.",
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