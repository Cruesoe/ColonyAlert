using UnityEngine;
using Verse;

namespace ColonyAlert
{
    public class AlertMod : Mod
    {
        public static AlertSettings Settings = null!;

        // Settings page text, translated on first draw and kept for the session
        private static string? switchHeader, areas, schedules, work, apparel, food, drugs, reading, hostility, animals, mechanoids, mechanoidsBiotech,
            newArrivals, newArrivalsTip, displayHeader, border, banner, tabMarker, tabMarkerTip, sound, message;

        public AlertMod(ModContentPack content) : base(content)
        {
            Settings = GetSettings<AlertSettings>();
        }

        public override string SettingsCategory() => "Colony Alert";

        public override void DoSettingsWindowContents(Rect inRect)
        {
            if (switchHeader == null)
            {
                TranslateLabels();
            }
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(inRect);
            listing.Label(switchHeader);
            listing.CheckboxLabeled(areas, ref Settings.areas);
            listing.CheckboxLabeled(schedules, ref Settings.schedules);
            listing.CheckboxLabeled(work, ref Settings.work);
            listing.CheckboxLabeled(apparel, ref Settings.apparel);
            listing.CheckboxLabeled(food, ref Settings.food);
            listing.CheckboxLabeled(drugs, ref Settings.drugs);
            listing.CheckboxLabeled(reading, ref Settings.reading);
            listing.CheckboxLabeled(hostility, ref Settings.hostility);
            listing.CheckboxLabeled(animals, ref Settings.animals);
            // Colony mechanoids need Biotech, so the option is greyed out without it
            Rect mechRow = listing.GetRect(Text.LineHeight);
            listing.Gap(listing.verticalSpacing);
            Widgets.CheckboxLabeled(mechRow, mechanoids, ref Settings.mechanoids, !ModsConfig.BiotechActive);
            if (!ModsConfig.BiotechActive)
            {
                TooltipHandler.TipRegion(mechRow, mechanoidsBiotech);
            }
            listing.CheckboxLabeled(newArrivals, ref Settings.newArrivals, newArrivalsTip);
            listing.GapLine();
            listing.Label(displayHeader);
            listing.CheckboxLabeled(border, ref Settings.border);
            listing.CheckboxLabeled(banner, ref Settings.banner);
            listing.CheckboxLabeled(tabMarker, ref Settings.tabMarker, tabMarkerTip);
            listing.CheckboxLabeled(sound, ref Settings.sound);
            listing.CheckboxLabeled(message, ref Settings.message);
            listing.End();
        }

        private static void TranslateLabels()
        {
            switchHeader = "ColonyAlert_SettingsSwitch".Translate();
            areas = "ColonyAlert_SettingAreas".Translate();
            schedules = "ColonyAlert_SettingSchedules".Translate();
            work = "ColonyAlert_SettingWork".Translate();
            apparel = "ColonyAlert_SettingApparel".Translate();
            food = "ColonyAlert_SettingFood".Translate();
            drugs = "ColonyAlert_SettingDrugs".Translate();
            reading = "ColonyAlert_SettingReading".Translate();
            hostility = "ColonyAlert_SettingHostility".Translate();
            animals = "ColonyAlert_SettingAnimals".Translate();
            mechanoids = "ColonyAlert_SettingMechanoids".Translate();
            mechanoidsBiotech = "ColonyAlert_SettingMechanoidsBiotech".Translate();
            newArrivals = "ColonyAlert_SettingNewArrivals".Translate();
            newArrivalsTip = "ColonyAlert_SettingNewArrivalsTip".Translate();
            displayHeader = "ColonyAlert_SettingsDisplay".Translate();
            border = "ColonyAlert_SettingBorder".Translate();
            banner = "ColonyAlert_SettingBanner".Translate();
            tabMarker = "ColonyAlert_SettingTabMarker".Translate();
            tabMarkerTip = "ColonyAlert_SettingTabMarkerTip".Translate();
            sound = "ColonyAlert_SettingSound".Translate();
            message = "ColonyAlert_SettingMessage".Translate();
        }
    }
}
