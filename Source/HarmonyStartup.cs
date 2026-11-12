using HarmonyLib;
using Verse;

namespace ColonyAlert
{
    [StaticConstructorOnStartup]
    public static class HarmonyStartup
    {
        static HarmonyStartup()
        {
            new Harmony("cruesoe.colonyalert").PatchAll();
        }
    }
}
