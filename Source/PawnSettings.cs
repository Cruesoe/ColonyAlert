using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RimWorld;
using Verse;

namespace Alert
{
    /// <summary>One pawn's remembered settings for one status. Policies and areas are stored by ID so deleted ones are skipped on restore.</summary>
    public class PawnSettings : IExposable
    {
        // GetPriority reports 3 for every enabled work type when manual priorities are off, so read the stored values directly
        private static readonly FieldInfo? PrioritiesField = typeof(Pawn_WorkSettings).GetField("priorities", BindingFlags.Instance | BindingFlags.NonPublic);

        private bool hasArea;
        private int mapId = -1;
        private int areaId = -1;
        private List<string>? times;
        private Dictionary<string, int>? work;
        private int apparelId = -1;
        private int foodId = -1;
        private int drugId = -1;
        private int readingId = -1;
        private bool hasHostility;
        private HostilityResponseMode hostility;

        public static bool IsTracked(Pawn pawn)
        {
            return pawn.RaceProps.Humanlike || (pawn.RaceProps.Animal && AlertMod.Settings.animals);
        }

        public static PawnSettings Capture(Pawn pawn)
        {
            AlertSettings s = AlertMod.Settings;
            PawnSettings result = new PawnSettings();
            bool humanlike = pawn.RaceProps.Humanlike;

            if ((humanlike ? s.areas : s.animals) && pawn.playerSettings != null && pawn.playerSettings.SupportsAllowedAreas && pawn.MapHeld != null)
            {
                result.hasArea = true;
                result.mapId = pawn.MapHeld.uniqueID;
                result.areaId = pawn.playerSettings.AreaRestrictionInPawnCurrentMap?.ID ?? -1;
            }
            if (!humanlike)
            {
                return result;
            }

            if (s.schedules && pawn.timetable?.times != null)
            {
                result.times = pawn.timetable.times.Select(t => t.defName).ToList();
            }
            if (s.work && pawn.workSettings != null && pawn.workSettings.EverWork)
            {
                DefMap<WorkTypeDef, int>? priorities = PrioritiesField?.GetValue(pawn.workSettings) as DefMap<WorkTypeDef, int>;
                result.work = new Dictionary<string, int>();
                foreach (WorkTypeDef def in DefDatabase<WorkTypeDef>.AllDefsListForReading)
                {
                    if (!pawn.WorkTypeIsDisabled(def))
                    {
                        result.work[def.defName] = priorities != null ? priorities[def] : pawn.workSettings.GetPriority(def);
                    }
                }
            }
            if (s.apparel && pawn.outfits != null)
            {
                result.apparelId = pawn.outfits.CurrentApparelPolicy?.id ?? -1;
            }
            if (s.food && pawn.foodRestriction != null && pawn.foodRestriction.Configurable)
            {
                result.foodId = pawn.foodRestriction.CurrentFoodPolicy?.id ?? -1;
            }
            if (s.drugs && pawn.drugs != null)
            {
                result.drugId = pawn.drugs.CurrentPolicy?.id ?? -1;
            }
            if (s.reading && pawn.reading != null)
            {
                result.readingId = pawn.reading.CurrentPolicy?.id ?? -1;
            }
            if (s.hostility && pawn.playerSettings != null && pawn.playerSettings.UsesConfigurableHostilityResponse)
            {
                result.hasHostility = true;
                result.hostility = pawn.playerSettings.hostilityResponse;
            }
            return result;
        }

        public void Apply(Pawn pawn)
        {
            if (hasArea && pawn.playerSettings != null && pawn.MapHeld is Map map && map.uniqueID == mapId)
            {
                if (areaId < 0)
                {
                    pawn.playerSettings.AreaRestrictionInPawnCurrentMap = null;
                }
                else
                {
                    Area? area = map.areaManager.AllAreas.Find(a => a.ID == areaId && a.AssignableAsAllowed());
                    if (area != null)
                    {
                        pawn.playerSettings.AreaRestrictionInPawnCurrentMap = area;
                    }
                }
            }

            if (times != null && pawn.timetable?.times != null)
            {
                for (int hour = 0; hour < times.Count && hour < pawn.timetable.times.Count; hour++)
                {
                    TimeAssignmentDef? def = DefDatabase<TimeAssignmentDef>.GetNamedSilentFail(times[hour]);
                    if (def != null)
                    {
                        pawn.timetable.SetAssignment(hour, def);
                    }
                }
            }

            if (work != null && pawn.workSettings != null && pawn.workSettings.EverWork)
            {
                foreach (KeyValuePair<string, int> entry in work)
                {
                    WorkTypeDef? def = DefDatabase<WorkTypeDef>.GetNamedSilentFail(entry.Key);
                    if (def != null && !pawn.WorkTypeIsDisabled(def))
                    {
                        pawn.workSettings.SetPriority(def, entry.Value);
                    }
                }
            }

            if (apparelId >= 0 && pawn.outfits != null)
            {
                ApparelPolicy? policy = Current.Game.outfitDatabase.AllOutfits.Find(p => p.id == apparelId);
                if (policy != null)
                {
                    pawn.outfits.CurrentApparelPolicy = policy;
                }
            }
            if (foodId >= 0 && pawn.foodRestriction != null && pawn.foodRestriction.Configurable)
            {
                FoodPolicy? policy = Current.Game.foodRestrictionDatabase.AllFoodRestrictions.Find(p => p.id == foodId);
                if (policy != null)
                {
                    pawn.foodRestriction.CurrentFoodPolicy = policy;
                }
            }
            if (drugId >= 0 && pawn.drugs != null)
            {
                DrugPolicy? policy = Current.Game.drugPolicyDatabase.AllPolicies.Find(p => p.id == drugId);
                if (policy != null)
                {
                    pawn.drugs.CurrentPolicy = policy;
                }
            }
            if (readingId >= 0 && pawn.reading != null)
            {
                ReadingPolicy? policy = Current.Game.readingPolicyDatabase.AllReadingPolicies.Find(p => p.id == readingId);
                if (policy != null)
                {
                    pawn.reading.CurrentPolicy = policy;
                }
            }

            // Pawns incapable of violence can't be set to attack
            if (hasHostility && pawn.playerSettings != null && pawn.playerSettings.UsesConfigurableHostilityResponse
                && (hostility != HostilityResponseMode.Attack || !pawn.WorkTagIsDisabled(WorkTags.Violent)))
            {
                pawn.playerSettings.hostilityResponse = hostility;
            }
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref hasArea, "hasArea");
            Scribe_Values.Look(ref mapId, "mapId", -1);
            Scribe_Values.Look(ref areaId, "areaId", -1);
            Scribe_Collections.Look(ref times, "times", LookMode.Value);
            Scribe_Collections.Look(ref work, "work", LookMode.Value, LookMode.Value);
            Scribe_Values.Look(ref apparelId, "apparelId", -1);
            Scribe_Values.Look(ref foodId, "foodId", -1);
            Scribe_Values.Look(ref drugId, "drugId", -1);
            Scribe_Values.Look(ref readingId, "readingId", -1);
            Scribe_Values.Look(ref hasHostility, "hasHostility");
            Scribe_Values.Look(ref hostility, "hostility");
        }
    }
}
