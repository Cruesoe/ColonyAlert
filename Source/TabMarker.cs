using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace ColonyAlert
{
    /// <summary>Marks pawn table tabs (Work, Schedule, Assign, Animals, Mechs) that show settings switched by alert status while it is on.</summary>
    public static class TabMarker
    {
        private const float IconSize = 24f;
        private const float MarkerHeight = 32f;
        private const float BottomSpace = 53f;
        private const float Gap = 6f;

        private static readonly Color MarkerRed = new Color(1f, 0.35f, 0.35f);

        // Switched setting kinds shown by each table; defs are fixed for the session
        private static readonly Dictionary<PawnTableDef, Kinds> KindsByTable = new Dictionary<PawnTableDef, Kinds>();

        private static string? label;
        private static float labelWidth;

        [Flags]
        private enum Kinds
        {
            None = 0,
            Areas = 1,
            AnimalAreas = 2,
            MechanoidAreas = 4,
            Schedules = 8,
            Work = 16,
            Apparel = 32,
            Food = 64,
            Drugs = 128,
            Reading = 256,
            Hostility = 512
        }

        public static void Draw(Rect rect, PawnTableDef? table)
        {
            if (table == null || Event.current.type != EventType.Repaint || !AlertMod.Settings.tabMarker || Current.Game?.GetComponent<GameComponent_Alert>()?.Active != true)
            {
                return;
            }
            if (!KindsByTable.TryGetValue(table, out Kinds kinds))
            {
                kinds = KindsOf(table);
                KindsByTable[table] = kinds;
            }
            if (!AnySwitched(kinds))
            {
                return;
            }

            if (label == null)
            {
                label = "ColonyAlert_TabMarker".Translate();
                Text.Font = GameFont.Small;
                labelWidth = Text.CalcSize(label).x;
            }

            // Sits in the empty space every pawn table tab leaves below the table
            float y = rect.yMax - BottomSpace + (BottomSpace - MarkerHeight) / 2f;
            GUI.DrawTexture(new Rect(rect.x, y + (MarkerHeight - IconSize) / 2f, IconSize, IconSize), AlertTextures.Warning);
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleLeft;
            GUI.color = MarkerRed;
            Widgets.Label(new Rect(rect.x + IconSize + Gap, y, labelWidth + 2f, MarkerHeight), label);
            GUI.color = Color.white;
            Text.Anchor = TextAnchor.UpperLeft;
        }

        private static bool AnySwitched(Kinds kinds)
        {
            AlertSettings s = AlertMod.Settings;
            return ((kinds & Kinds.Areas) != 0 && s.areas)
                || ((kinds & Kinds.AnimalAreas) != 0 && s.animals)
                || ((kinds & Kinds.MechanoidAreas) != 0 && s.mechanoids)
                || ((kinds & Kinds.Schedules) != 0 && s.schedules)
                || ((kinds & Kinds.Work) != 0 && s.work)
                || ((kinds & Kinds.Apparel) != 0 && s.apparel)
                || ((kinds & Kinds.Food) != 0 && s.food)
                || ((kinds & Kinds.Drugs) != 0 && s.drugs)
                || ((kinds & Kinds.Reading) != 0 && s.reading)
                || ((kinds & Kinds.Hostility) != 0 && s.hostility);
        }

        // Read from the table's column workers, so tables added or changed by other mods are covered too
        private static Kinds KindsOf(PawnTableDef table)
        {
            Kinds kinds = Kinds.None;
            Kinds areaKind = table == PawnTableDefOf.Animals ? Kinds.AnimalAreas : table == PawnTableDefOf.Mechs ? Kinds.MechanoidAreas : Kinds.Areas;
            foreach (PawnColumnDef column in table.columns)
            {
                Type? worker = column.workerClass;
                if (worker == null)
                {
                    continue;
                }
                if (typeof(PawnColumnWorker_AllowedArea).IsAssignableFrom(worker) || typeof(PawnColumnWorker_AllowedAreaWide).IsAssignableFrom(worker))
                {
                    kinds |= areaKind;
                }
                else if (typeof(PawnColumnWorker_Timetable).IsAssignableFrom(worker))
                {
                    kinds |= Kinds.Schedules;
                }
                else if (typeof(PawnColumnWorker_WorkPriority).IsAssignableFrom(worker))
                {
                    kinds |= Kinds.Work;
                }
                else if (typeof(PawnColumnWorker_Outfit).IsAssignableFrom(worker))
                {
                    kinds |= Kinds.Apparel;
                }
                else if (typeof(PawnColumnWorker_FoodRestriction).IsAssignableFrom(worker))
                {
                    kinds |= Kinds.Food;
                }
                else if (typeof(PawnColumnWorker_DrugPolicy).IsAssignableFrom(worker))
                {
                    kinds |= Kinds.Drugs;
                }
                else if (typeof(PawnColumnWorker_Reading).IsAssignableFrom(worker))
                {
                    kinds |= Kinds.Reading;
                }
                else if (typeof(PawnColumnWorker_HostilityResponse).IsAssignableFrom(worker))
                {
                    kinds |= Kinds.Hostility;
                }
            }
            return kinds;
        }
    }
}
