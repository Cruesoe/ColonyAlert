using UnityEngine;
using Verse;

namespace ColonyAlert
{
    public class AlertMod : Mod
    {
        public static AlertSettings Settings = null!;

        private const float SubIndent = 24f;
        private const float SliderRowHeight = 30f;
        private const float SliderLabelPct = 0.45f;

        // Settings page text, translated on first draw and kept for the session
        private static string? switchHeader, areas, schedules, work, apparel, food, drugs, reading, hostility, animals, mechanoids, mechanoidsBiotech,
            newArrivals, newArrivalsTip, displayHeader, border, borderStyle, borderSolid, borderGlow, borderPulse, borderNeedsBorder, borderPulseNeedsGlow,
            indicator, indicatorBanner, indicatorNotification, indicatorOff, tabMarker, tabMarkerTip, sound, message;

        // Slider labels, rebuilt only when their value changes
        private static string widthLabel = "", opacityLabel = "";
        private static int widthLabelValue = -1;
        private static float opacityLabelValue = -1f;

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
            listing.ColumnWidth = (inRect.width - Listing.ColumnSpacing) / 2f;
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
            CheckboxRow(listing, 0f, mechanoids, ref Settings.mechanoids, !ModsConfig.BiotechActive, mechanoidsBiotech);
            listing.CheckboxLabeled(newArrivals, ref Settings.newArrivals, newArrivalsTip);

            listing.NewColumn();
            listing.Label(displayHeader);
            listing.CheckboxLabeled(border, ref Settings.border);
            // Border options are greyed out while the border is off; pulse applies to the glow only
            bool noBorder = !Settings.border;
            if (Settings.borderWidth != widthLabelValue)
            {
                widthLabelValue = Settings.borderWidth;
                widthLabel = "ColonyAlert_SettingBorderWidth".Translate(widthLabelValue);
            }
            Settings.borderWidth = Mathf.RoundToInt(SliderRow(listing, widthLabel, Settings.borderWidth, AlertBorder.MinWidth, AlertBorder.MaxWidth, 1f, noBorder));
            if (Settings.borderOpacity != opacityLabelValue)
            {
                opacityLabelValue = Settings.borderOpacity;
                opacityLabel = "ColonyAlert_SettingBorderOpacity".Translate(opacityLabelValue.ToStringPercent());
            }
            Settings.borderOpacity = SliderRow(listing, opacityLabel, Settings.borderOpacity, AlertBorder.MinOpacity, 1f, 0.05f, noBorder);
            LabelRow(listing, borderStyle, noBorder);
            if (listing.RadioButton(borderSolid, Settings.borderStyle == BorderStyle.Solid, SubIndent * 2f, noBorder ? borderNeedsBorder : null, null, noBorder))
            {
                Settings.borderStyle = BorderStyle.Solid;
            }
            if (listing.RadioButton(borderGlow, Settings.borderStyle == BorderStyle.Glow, SubIndent * 2f, noBorder ? borderNeedsBorder : null, null, noBorder))
            {
                Settings.borderStyle = BorderStyle.Glow;
            }
            CheckboxRow(listing, SubIndent, borderPulse, ref Settings.borderPulse, noBorder || Settings.borderStyle != BorderStyle.Glow, noBorder ? borderNeedsBorder : borderPulseNeedsGlow);

            listing.Gap();
            listing.Label(indicator);
            if (listing.RadioButton(indicatorBanner, Settings.indicator == IndicatorStyle.Banner, SubIndent))
            {
                Settings.indicator = IndicatorStyle.Banner;
            }
            if (listing.RadioButton(indicatorNotification, Settings.indicator == IndicatorStyle.Notification, SubIndent))
            {
                Settings.indicator = IndicatorStyle.Notification;
            }
            if (listing.RadioButton(indicatorOff, Settings.indicator == IndicatorStyle.Off, SubIndent))
            {
                Settings.indicator = IndicatorStyle.Off;
            }
            listing.CheckboxLabeled(tabMarker, ref Settings.tabMarker, tabMarkerTip);
            listing.CheckboxLabeled(sound, ref Settings.sound);
            listing.CheckboxLabeled(message, ref Settings.message);
            listing.End();
        }

        // Checkbox that is greyed out and inert with a tooltip when disabled
        private static void CheckboxRow(Listing_Standard listing, float indent, string? label, ref bool value, bool disabled, string? disabledTip)
        {
            Rect row = listing.GetRect(Text.LineHeight);
            listing.Gap(listing.verticalSpacing);
            row.xMin += indent;
            Widgets.CheckboxLabeled(row, label, ref value, disabled);
            if (disabled)
            {
                TooltipHandler.TipRegion(row, disabledTip);
            }
        }

        private static void LabelRow(Listing_Standard listing, string? label, bool disabled)
        {
            Rect row = listing.GetRect(Text.LineHeight);
            listing.Gap(listing.verticalSpacing);
            row.xMin += SubIndent;
            GUI.color = disabled ? Widgets.InactiveColor : Color.white;
            Widgets.Label(row, label);
            GUI.color = Color.white;
            if (disabled)
            {
                TooltipHandler.TipRegion(row, borderNeedsBorder);
            }
        }

        // Labeled slider, indented under the border option; greyed out and inert when disabled
        private static float SliderRow(Listing_Standard listing, string label, float value, float min, float max, float roundTo, bool disabled)
        {
            Rect row = listing.GetRect(SliderRowHeight);
            listing.Gap(listing.verticalSpacing);
            row.xMin += SubIndent;
            GUI.color = disabled ? Widgets.InactiveColor : Color.white;
            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(row.LeftPart(SliderLabelPct), label);
            Text.Anchor = TextAnchor.UpperLeft;
            float result = Widgets.HorizontalSlider(row.RightPart(1f - SliderLabelPct), value, min, max, middleAlignment: true, roundTo: roundTo);
            GUI.color = Color.white;
            if (disabled)
            {
                TooltipHandler.TipRegion(row, borderNeedsBorder);
                return value;
            }
            return result;
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
            borderStyle = "ColonyAlert_SettingBorderStyle".Translate();
            borderSolid = "ColonyAlert_SettingBorderStyleSolid".Translate();
            borderGlow = "ColonyAlert_SettingBorderStyleGlow".Translate();
            borderPulse = "ColonyAlert_SettingBorderPulse".Translate();
            borderNeedsBorder = "ColonyAlert_SettingBorderNeedsBorder".Translate();
            borderPulseNeedsGlow = "ColonyAlert_SettingBorderPulseNeedsGlow".Translate();
            indicator = "ColonyAlert_SettingIndicator".Translate();
            indicatorBanner = "ColonyAlert_SettingIndicatorBanner".Translate();
            indicatorNotification = "ColonyAlert_SettingIndicatorNotification".Translate();
            indicatorOff = "ColonyAlert_SettingIndicatorOff".Translate();
            tabMarker = "ColonyAlert_SettingTabMarker".Translate();
            tabMarkerTip = "ColonyAlert_SettingTabMarkerTip".Translate();
            sound = "ColonyAlert_SettingSound".Translate();
            message = "ColonyAlert_SettingMessage".Translate();
        }
    }
}
