using RimWorld;
using Verse;

namespace Alert
{
    [DefOf]
    public static class AlertDefOf
    {
        public static KeyBindingDef Alert_Toggle = null!;
        public static SoundDef Alert_Klaxon = null!;
        public static SoundDef Alert_StandDown = null!;

        static AlertDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(AlertDefOf));
        }
    }
}
