namespace ncrGetSomePersonality;
[BepInPlugin("neoncityrain.getsomepersonality", "Get Some Personality!", "0.0.1")]
[BepInDependency("Together", BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency("org.bepinex.plugins.qol_unknown", BepInDependency.DependencyFlags.SoftDependency)]

public class PluginSetup : BaseUnityPlugin
{
    Harmony harmony;
    internal void Awake()
    {
        Log = Logger;
        try
        {
            harmony = new("neoncityrain.getsomepersonality");
            harmony.PatchAll();
            foreach (MethodBase method in harmony.GetPatchedMethods())
            {
                Log.LogDebug("Harmony has patched: " +
                method.ToString());
            }

            if (assemblyExists("QoL Unknown"))
            {
                QOLEnabled = true;
            }
            if (assemblyExists("CasualtiesMP"))
            {
                playingMultiplayer = true;
            }
            initConfigs();
        }
        catch (Exception ex)
        {
            Log.LogError("GSP has failed to patch correctly. Error is: " + ex);
            return;
        }

        try
        {
            LoadGSPJsons();
        }
        catch (Exception ex)
        {
            Log.LogError("Json file collector is being a shithead again. Error is: " + ex);
            return;
        }

        Log.LogInfo("GSP has loaded correctly!");
    }

    private void OnDestroy()
    {
        harmony.UnpatchSelf();
        if (currentSpriteCache != null)
        {
            currentSpriteCache.Clear();
        }
    }
}