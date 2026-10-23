using UnityEngine;
using Verse;

namespace ColonyAlert
{
    public class AlertMod : Mod
    {
        public static AlertSettings Settings = null!;

        public AlertMod(ModContentPack content) : base(content)
        {
            Settings = GetSettings<AlertSettings>();
        }

        public override string SettingsCategory() => "Colony Alert";

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(inRect);
            listing.Label("ColonyAlert_SettingsSwitch".Translate());
            listing.CheckboxLabeled("ColonyAlert_SettingAreas".Translate(), ref Settings.areas);
            listing.CheckboxLabeled("ColonyAlert_SettingSchedules".Translate(), ref Settings.schedules);
            listing.CheckboxLabeled("ColonyAlert_SettingWork".Translate(), ref Settings.work);
            listing.CheckboxLabeled("ColonyAlert_SettingApparel".Translate(), ref Settings.apparel);
            listing.CheckboxLabeled("ColonyAlert_SettingFood".Translate(), ref Settings.food);
            listing.CheckboxLabeled("ColonyAlert_SettingDrugs".Translate(), ref Settings.drugs);
            listing.CheckboxLabeled("ColonyAlert_SettingReading".Translate(), ref Settings.reading);
            listing.CheckboxLabeled("ColonyAlert_SettingHostility".Translate(), ref Settings.hostility);
            listing.CheckboxLabeled("ColonyAlert_SettingAnimals".Translate(), ref Settings.animals);
            listing.GapLine();
            listing.Label("ColonyAlert_SettingsDisplay".Translate());
            listing.CheckboxLabeled("ColonyAlert_SettingBorder".Translate(), ref Settings.border);
            listing.CheckboxLabeled("ColonyAlert_SettingSound".Translate(), ref Settings.sound);
            listing.CheckboxLabeled("ColonyAlert_SettingMessage".Translate(), ref Settings.message);
            listing.End();
        }
    }
}
