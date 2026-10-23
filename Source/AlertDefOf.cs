using RimWorld;
using Verse;

namespace ColonyAlert
{
    [DefOf]
    public static class AlertDefOf
    {
        public static KeyBindingDef ColonyAlert_Toggle = null!;
        public static SoundDef ColonyAlert_Klaxon = null!;
        public static SoundDef ColonyAlert_StandDown = null!;

        static AlertDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(AlertDefOf));
        }
    }
}
