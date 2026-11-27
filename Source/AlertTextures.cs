using System;
using UnityEngine;
using Verse;

namespace ColonyAlert
{
    [StaticConstructorOnStartup]
    public static class AlertTextures
    {
        // Glow ramp resolution; the textures stretch to any border width
        private const int GlowSize = 32;

        public static readonly Texture2D Banner = ContentFinder<Texture2D>.Get("ColonyAlert/Banner");
        public static readonly Texture2D Warning = ContentFinder<Texture2D>.Get("ColonyAlert/Warning");

        // White glow textures, tinted when drawn: opaque at the screen edge, clear on the inside. Rows count up from the bottom.
        public static readonly Texture2D GlowLeft = Glow(GlowSize, 1, (x, y) => x);
        public static readonly Texture2D GlowRight = Glow(GlowSize, 1, (x, y) => GlowSize - 1 - x);
        public static readonly Texture2D GlowTop = Glow(1, GlowSize, (x, y) => GlowSize - 1 - y);
        public static readonly Texture2D GlowBottom = Glow(1, GlowSize, (x, y) => y);
        public static readonly Texture2D GlowTopLeft = Glow(GlowSize, GlowSize, (x, y) => Math.Min(x, GlowSize - 1 - y));
        public static readonly Texture2D GlowTopRight = Glow(GlowSize, GlowSize, (x, y) => Math.Min(GlowSize - 1 - x, GlowSize - 1 - y));
        public static readonly Texture2D GlowBottomLeft = Glow(GlowSize, GlowSize, (x, y) => Math.Min(x, y));
        public static readonly Texture2D GlowBottomRight = Glow(GlowSize, GlowSize, (x, y) => Math.Min(GlowSize - 1 - x, y));

        // distance gives each pixel's distance in pixels from the edge it fades away from
        private static Texture2D Glow(int width, int height, Func<int, int, int> distance)
        {
            Color[] pixels = new Color[width * height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float fade = 1f - (distance(x, y) + 0.5f) / GlowSize;
                    pixels[y * width + x] = new Color(1f, 1f, 1f, fade * fade);
                }
            }
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false)
            {
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear
            };
            texture.SetPixels(pixels);
            texture.Apply();
            return texture;
        }
    }
}
