using UnityEngine;
using Verse;

namespace Alert
{
    public class AlertMod : Mod
    {
        public static AlertSettings Settings = null!;

        public AlertMod(ModContentPack content) : base(content)
        {
            Settings = GetSettings<AlertSettings>();
        }

        public override string SettingsCategory() => "Alert!";

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(inRect);
            listing.Label("Alert_SettingsSwitch".Translate());
            listing.CheckboxLabeled("Alert_SettingAreas".Translate(), ref Settings.areas);
            listing.CheckboxLabeled("Alert_SettingSchedules".Translate(), ref Settings.schedules);
            listing.CheckboxLabeled("Alert_SettingWork".Translate(), ref Settings.work);
            listing.CheckboxLabeled("Alert_SettingApparel".Translate(), ref Settings.apparel);
            listing.CheckboxLabeled("Alert_SettingFood".Translate(), ref Settings.food);
            listing.CheckboxLabeled("Alert_SettingDrugs".Translate(), ref Settings.drugs);
            listing.CheckboxLabeled("Alert_SettingReading".Translate(), ref Settings.reading);
            listing.CheckboxLabeled("Alert_SettingHostility".Translate(), ref Settings.hostility);
            listing.CheckboxLabeled("Alert_SettingAnimals".Translate(), ref Settings.animals);
            listing.GapLine();
            listing.Label("Alert_SettingsDisplay".Translate());
            listing.CheckboxLabeled("Alert_SettingBorder".Translate(), ref Settings.border);
            listing.CheckboxLabeled("Alert_SettingSound".Translate(), ref Settings.sound);
            listing.CheckboxLabeled("Alert_SettingMessage".Translate(), ref Settings.message);
            listing.End();
        }
    }
}
