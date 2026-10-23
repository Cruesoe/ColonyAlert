using UnityEngine;
using Verse;

namespace ColonyAlert
{
    [StaticConstructorOnStartup]
    public static class AlertTextures
    {
        public static readonly Texture2D Banner = ContentFinder<Texture2D>.Get("ColonyAlert/Banner");
        public static readonly Texture2D Warning = ContentFinder<Texture2D>.Get("ColonyAlert/Warning");
    }
}
