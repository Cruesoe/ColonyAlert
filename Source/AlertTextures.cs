using UnityEngine;
using Verse;

namespace Alert
{
    [StaticConstructorOnStartup]
    public static class AlertTextures
    {
        public static readonly Texture2D Banner = ContentFinder<Texture2D>.Get("Alert/Banner");
        public static readonly Texture2D Warning = ContentFinder<Texture2D>.Get("Alert/Warning");
    }
}
