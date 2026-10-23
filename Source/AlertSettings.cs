using Verse;

namespace Alert
{
    public class AlertSettings : ModSettings
    {
        public bool areas = true;
        public bool schedules = true;
        public bool work = true;
        public bool apparel = true;
        public bool food = true;
        public bool drugs = true;
        public bool reading = true;
        public bool hostility = true;
        public bool animals = true;
        public bool border = true;
        public bool sound = true;
        public bool message = true;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref areas, "areas", true);
            Scribe_Values.Look(ref schedules, "schedules", true);
            Scribe_Values.Look(ref work, "work", true);
            Scribe_Values.Look(ref apparel, "apparel", true);
            Scribe_Values.Look(ref food, "food", true);
            Scribe_Values.Look(ref drugs, "drugs", true);
            Scribe_Values.Look(ref reading, "reading", true);
            Scribe_Values.Look(ref hostility, "hostility", true);
            Scribe_Values.Look(ref animals, "animals", true);
            Scribe_Values.Look(ref border, "border", true);
            Scribe_Values.Look(ref sound, "sound", true);
            Scribe_Values.Look(ref message, "message", true);
        }
    }
}
