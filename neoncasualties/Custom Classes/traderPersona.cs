namespace ncrGetSomePersonality;

internal static class traderPersona
{
    internal class TraderKind
    {
        internal string personalityType;
        internal List<string> bodyTraits = [];
        internal string ownName = "";
        internal string codeName = "";
        internal string lastDialogueKey = "";
        internal BuildingEntity myBody;

        internal TraderKind()
        {

        }
    }
    internal static readonly ConditionalWeakTable<TraderScript, TraderKind> TraderCWT = new();
    internal static TraderKind TraderSelf(this TraderScript trader) => TraderCWT.GetValue(trader, _ => new());
}
