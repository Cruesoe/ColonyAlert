using HarmonyLib;
using RimWorld;
using UnityEngine;

namespace ColonyAlert
{
    /// <summary>Draws the alert marker on pawn table tabs; tabs that override DoWindowContents call this base method too.</summary>
    [HarmonyPatch(typeof(MainTabWindow_PawnTable), nameof(MainTabWindow_PawnTable.DoWindowContents))]
    public static class Patch_MainTabWindow_PawnTable_DoWindowContents
    {
        private static readonly AccessTools.FieldRef<PawnTable, PawnTableDef> TableDef = AccessTools.FieldRefAccess<PawnTable, PawnTableDef>("def");

        public static void Postfix(Rect __0, PawnTable? ___table)
        {
            if (___table != null)
            {
                TabMarker.Draw(__0, TableDef(___table));
            }
        }
    }
}
