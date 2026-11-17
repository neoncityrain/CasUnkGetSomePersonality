namespace ncrGetSomePersonality;

internal static class characterAgency
{
    internal static GameObject spawnAgent()
    {
        GameObject gameObject =
            Utils.Create("Special/Characters/Milky", Vector2.zero, 0f);
        var body = gameObject.GetComponentInChildren<Body>();
        var wv = new WoundView();
        wv.SetCharDetails((int)((float)Random.Range(130, 161) * body.heightMultiplier),
            Random.Range(10, 26), Random.Range(1, 100000), Random.Range(0, 10));
        body.PlayerSelf().npc = true;
        body.PlayerSelf().owner = body;
        body.PlayerSelf().myWV = wv;
        wv.CustomCall().owner = body;
        wv.CustomCall().thisView = wv;
        body.PlayerSelf().reviseStats();
        return gameObject;
    }
}
