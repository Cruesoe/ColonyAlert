using RimWorld.Planet;
using Verse;

namespace ColonyAlert
{
    /// <summary>Draws the alert indicator on the current map, after thing overlays such as pawn labels so it sits on top of them.</summary>
    public class MapComponent_AlertIndicator : MapComponent
    {
        public MapComponent_AlertIndicator(Map map) : base(map)
        {
        }

        public override void MapComponentOnGUI()
        {
            if (WorldRendererUtility.DrawingMap)
            {
                Current.Game.GetComponent<GameComponent_Alert>()?.DrawIndicatorIfActive();
            }
        }
    }
}
