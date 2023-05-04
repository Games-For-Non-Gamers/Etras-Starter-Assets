using UnityEngine;

namespace Etra.StarterAssets.Source.Editor
{
    public static partial class GUIStyleExtensions
    {
        public static GUIStyle WithBackgroundColor(this GUIStyle style, Color color) =>
            WithBackground(style, EtraGUIUtility.GenerateColorTexture(color));

        public static GUIStyle WithBackground(this GUIStyle style, Texture2D background)
        {
            style.normal.background = background;
            return style;
        }
    }
}
