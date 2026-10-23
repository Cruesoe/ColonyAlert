using RimWorld;
using Verse;

namespace Alert
{
    [DefOf]
    public static class AlertDefOf
    {
        public static KeyBindingDef Alert_Toggle = null!;

        static AlertDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(AlertDefOf));
        }
    }
}
