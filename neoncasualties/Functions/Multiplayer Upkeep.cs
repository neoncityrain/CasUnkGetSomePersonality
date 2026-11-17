using Together;

namespace ncrGetSomePersonality;

internal class multiplayer
{
    public static Body multiplayerBody(Body body)
    {
        if (!Net.IsRunning) return null;

        body.TryGetNetBody(out var player);
        return player.body;
    }

    public static void handlePersonaForAllPlayers()
    {
        foreach (ScavPlayer player in ScavPlayer.AllLivingPlayers)
        {
            var self = player.body.PlayerSelf();
            if (string.IsNullOrEmpty(self.personalityType)) // player not initialized
            {
                self.reviseStats();
                foreach (ScavPlayer others in ScavPlayer.AllLivingPlayers)
                {
                    if (others.body.PlayerSelf().personalityType == self.personalityType)
                    {
                        self.personalityType = null;
                        self.reviseStats();
                        // give it a higher chance of being different
                    }
                }
            }
        }
    }
}
