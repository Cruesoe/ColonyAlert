using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Alert
{
    /// <summary>Holds the current status and each pawn's remembered settings for both statuses, and draws the alert indicator.</summary>
    public class GameComponent_Alert : GameComponent
    {
        private const float BannerWidth = 140f;
        private const float BannerHeight = 34f;
        private const float MainButtonsHeight = 35f;
        private const float BorderThickness = 3f;
        private const int SeedInterval = 250;

        private static readonly Color AlertRed = new Color(0.8f, 0.1f, 0.1f, 0.85f);

        private bool alert;
        private Dictionary<string, PawnSettings> normal = new Dictionary<string, PawnSettings>();
        private Dictionary<string, PawnSettings> alerted = new Dictionary<string, PawnSettings>();

        public GameComponent_Alert(Game game)
        {
        }

        public void Toggle()
        {
            Dictionary<string, PawnSettings> leaving = alert ? alerted : normal;
            Dictionary<string, PawnSettings> entering = alert ? normal : alerted;
            List<Pawn> pawns = TrackedPawns();

            // Save every pawn's current settings to the status being left before applying any
            foreach (Pawn pawn in pawns)
            {
                leaving[pawn.GetUniqueLoadID()] = PawnSettings.Capture(pawn);
            }
            // Pawns with nothing saved for the new status keep their current settings
            foreach (Pawn pawn in pawns)
            {
                if (entering.TryGetValue(pawn.GetUniqueLoadID(), out PawnSettings? settings) && settings != null)
                {
                    settings.Apply(pawn);
                }
            }

            alert = !alert;
            Prune(pawns);

            if (AlertMod.Settings.sound)
            {
                (alert ? SoundDefOf.Tick_High : SoundDefOf.Tick_Low).PlayOneShotOnCamera();
            }
            if (AlertMod.Settings.message)
            {
                Messages.Message((alert ? "Alert_On" : "Alert_Off").Translate(), MessageTypeDefOf.SilentInput, false);
            }
        }

        public override void GameComponentTick()
        {
            // Pawns who join during alert get their current settings as their normal ones
            if (!alert || Find.TickManager.TicksGame % SeedInterval != 0)
            {
                return;
            }
            foreach (Pawn pawn in TrackedPawns())
            {
                string id = pawn.GetUniqueLoadID();
                if (!normal.ContainsKey(id))
                {
                    normal[id] = PawnSettings.Capture(pawn);
                }
            }
        }

        public override void GameComponentOnGUI()
        {
            if (Event.current.type == EventType.KeyDown && AlertDefOf.Alert_Toggle.KeyDownEvent)
            {
                Toggle();
                Event.current.Use();
                return;
            }
            if (alert && Find.ScreenshotModeHandler?.Active != true)
            {
                DrawIndicator();
            }
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref alert, "alert");
            Scribe_Collections.Look(ref normal, "normal", LookMode.Value, LookMode.Deep);
            Scribe_Collections.Look(ref alerted, "alerted", LookMode.Value, LookMode.Deep);
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                normal ??= new Dictionary<string, PawnSettings>();
                alerted ??= new Dictionary<string, PawnSettings>();
            }
        }

        private static List<Pawn> TrackedPawns()
        {
            return PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_OfPlayerFaction.Where(PawnSettings.IsTracked).ToList();
        }

        // Forget pawns that are dead or gone from the world; kidnapped and away pawns are kept
        private void Prune(List<Pawn> tracked)
        {
            HashSet<string> alive = new HashSet<string>(PawnsFinder.AllMapsWorldAndTemporary_Alive.Select(p => p.GetUniqueLoadID()));
            alive.UnionWith(tracked.Select(p => p.GetUniqueLoadID()));
            foreach (Dictionary<string, PawnSettings> states in new[] { normal, alerted })
            {
                foreach (string id in states.Keys.Where(id => !alive.Contains(id)).ToList())
                {
                    states.Remove(id);
                }
            }
        }

        private void DrawIndicator()
        {
            if (AlertMod.Settings.border && Event.current.type == EventType.Repaint)
            {
                float width = UI.screenWidth;
                float height = UI.screenHeight;
                Widgets.DrawBoxSolid(new Rect(0f, 0f, width, BorderThickness), AlertRed);
                Widgets.DrawBoxSolid(new Rect(0f, height - BorderThickness, width, BorderThickness), AlertRed);
                Widgets.DrawBoxSolid(new Rect(0f, 0f, BorderThickness, height), AlertRed);
                Widgets.DrawBoxSolid(new Rect(width - BorderThickness, 0f, BorderThickness, height), AlertRed);
            }

            Rect banner = new Rect((UI.screenWidth - BannerWidth) / 2f, UI.screenHeight - MainButtonsHeight - BannerHeight - 6f, BannerWidth, BannerHeight);
            Widgets.DrawBoxSolidWithOutline(banner, AlertRed, Color.white);
            Widgets.DrawHighlightIfMouseover(banner);
            Text.Font = GameFont.Medium;
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(banner, "Alert_Banner".Translate());
            Text.Anchor = TextAnchor.UpperLeft;
            Text.Font = GameFont.Small;
            TooltipHandler.TipRegion(banner, "Alert_BannerTip".Translate(AlertDefOf.Alert_Toggle.MainKeyLabel));

            // The banner draws beneath windows, so ignore clicks meant for a window over it
            if (Find.WindowStack.GetWindowAt(UI.MousePositionOnUIInverted) == null && Widgets.ButtonInvisible(banner))
            {
                Toggle();
            }
        }
    }
}
