using UnityEditor;
using UnityEngine;

public static partial class EtraGUIUtility
{
    public static Color BorderColor => EditorGUIUtility.isProSkin ? new Color(0.1372549019607843f, 0.1372549019607843f, 0.1372549019607843f) : new Color(0.6f, 0.6f, 0.6f);
    public static Texture2D BorderTexture => GenerateColorTexture(BorderColor);

    public static Texture2D GenerateColorTexture(Color color)
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, color);
        texture.Apply();
        return texture;
    }

    public static void HorizontalLineLayout()
    {
        GUIStyle style = new GUIStyle()
        {
            fixedHeight = 1f,
            stretchWidth = true,
        };

        style.normal.background = BorderTexture;
        GUILayout.Box(GUIContent.none, style);
    }
}