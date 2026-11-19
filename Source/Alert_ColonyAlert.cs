using RimWorld;
using UnityEngine;
using Verse;

namespace ColonyAlert
{
    /// <summary>Permanent entry in the alerts list while alert status is on, when the indicator is set to notification.</summary>
    public class Alert_ColonyAlert : Alert
    {
        // Explanation text, rebuilt only when the toggle key changes
        private KeyCode explanationKey = KeyCode.None;
        private TaggedString explanation;

        public Alert_ColonyAlert()
        {
            defaultLabel = "ColonyAlert_Notification".Translate();
            defaultPriority = AlertPriority.Critical;
        }

        // Same pulsing red as critical alerts such as major break risk, without their repeated messages
        protected override Color BGColor => Alert_Critical.BgColor();

        public override AlertReport GetReport()
        {
            if (AlertMod.Settings.indicator != IndicatorStyle.Notification)
            {
                return false;
            }
            return Current.Game?.GetComponent<GameComponent_Alert>()?.Active == true;
        }

        public override TaggedString GetExplanation()
        {
            KeyCode key = AlertDefOf.ColonyAlert_Toggle.MainKey;
            if (key != explanationKey || explanation.NullOrEmpty())
            {
                explanationKey = key;
                explanation = key == KeyCode.None ? "ColonyAlert_NotificationExplanationUnbound".Translate() : "ColonyAlert_NotificationExplanation".Translate(key.ToStringReadable());
            }
            return explanation;
        }
    }
}
