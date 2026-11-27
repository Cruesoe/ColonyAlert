using UnityEngine;
using Verse;

namespace ColonyAlert
{
    /// <summary>Draws the red screen border during alert status, as solid lines or a glow that fades inwards.</summary>
    public static class AlertBorder
    {
        public const int DefaultWidth = 3;
        public const int MinWidth = 1;
        public const int MaxWidth = 40;
        public const float DefaultOpacity = 0.85f;
        public const float MinOpacity = 0.1f;

        // Solid borders up to this width also outline the bottom button bar; wider ones would cover its labels
        private const int ThinWidth = 4;
        private const float MainButtonsHeight = 35f;

        private static readonly Color AlertRed = new Color(0.8f, 0.1f, 0.1f);

        // Slow pulse between full and 55% strength, shared with the banner's warning icon
        public static float Pulse => 0.775f + 0.225f * Mathf.Sin(Time.realtimeSinceStartup * 3f);

        public static void Draw()
        {
            AlertSettings s = AlertMod.Settings;
            bool glow = s.borderStyle == BorderStyle.Glow;
            float alpha = s.borderOpacity * (glow && s.borderPulse ? Pulse : 1f);
            float width = Mathf.Clamp(s.borderWidth, MinWidth, MaxWidth);
            float screenWidth = UI.screenWidth;
            float mapBottom = UI.screenHeight - MainButtonsHeight;

            GUI.color = new Color(AlertRed.r, AlertRed.g, AlertRed.b, alpha);
            if (glow)
            {
                DrawGlow(width, screenWidth, mapBottom);
            }
            else
            {
                DrawSolid(width, screenWidth, mapBottom);
            }
            GUI.color = Color.white;
        }

        private static void DrawSolid(float width, float screenWidth, float mapBottom)
        {
            float screenHeight = UI.screenHeight;
            bool thin = width <= ThinWidth;
            float sideHeight = thin ? screenHeight : mapBottom;
            GUI.DrawTexture(new Rect(0f, 0f, screenWidth, width), BaseContent.WhiteTex);
            GUI.DrawTexture(new Rect(0f, mapBottom - width, screenWidth, width), BaseContent.WhiteTex);
            GUI.DrawTexture(new Rect(0f, 0f, width, sideHeight), BaseContent.WhiteTex);
            GUI.DrawTexture(new Rect(screenWidth - width, 0f, width, sideHeight), BaseContent.WhiteTex);
            if (thin)
            {
                GUI.DrawTexture(new Rect(0f, screenHeight - width, screenWidth, width), BaseContent.WhiteTex);
            }
        }

        // Outlines the map view only, with corner pieces so the edges meet without doubling up
        private static void DrawGlow(float width, float screenWidth, float mapBottom)
        {
            float right = screenWidth - width;
            float bottom = mapBottom - width;
            float edgeLength = screenWidth - 2f * width;
            float sideLength = mapBottom - 2f * width;
            GUI.DrawTexture(new Rect(0f, 0f, width, width), AlertTextures.GlowTopLeft);
            GUI.DrawTexture(new Rect(right, 0f, width, width), AlertTextures.GlowTopRight);
            GUI.DrawTexture(new Rect(0f, bottom, width, width), AlertTextures.GlowBottomLeft);
            GUI.DrawTexture(new Rect(right, bottom, width, width), AlertTextures.GlowBottomRight);
            GUI.DrawTexture(new Rect(width, 0f, edgeLength, width), AlertTextures.GlowTop);
            GUI.DrawTexture(new Rect(width, bottom, edgeLength, width), AlertTextures.GlowBottom);
            GUI.DrawTexture(new Rect(0f, width, width, sideLength), AlertTextures.GlowLeft);
            GUI.DrawTexture(new Rect(right, width, width, sideLength), AlertTextures.GlowRight);
        }
    }
}
