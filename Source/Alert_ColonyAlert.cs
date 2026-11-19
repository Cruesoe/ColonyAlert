using RimWorld;
using UnityEngine;
using Verse;

namespace ColonyAlert
{
    /// <summary>Permanent entry in the alerts list while alert status is on, when the indicator is set to notification.</summary>
    public class Alert_ColonyAlert : Alert
    {
        private static readonly Color Background = new Color(0.5f, 0.05f, 0.05f, 0.6f);

        // Explanation text, rebuilt only when the toggle key changes
        private KeyCode explanationKey = KeyCode.None;
        private TaggedString explanation;

        public Alert_ColonyAlert()
        {
            defaultLabel = "ColonyAlert_Notification".Translate();
            defaultPriority = AlertPriority.High;
        }

        protected override Color BGColor => Background;

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
                explanation = "ColonyAlert_NotificationExplanation".Translate(key.ToStringReadable());
            }
            return explanation;
        }
    }
}
