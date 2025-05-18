using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PixelationEffect))]
public class PixelationEffectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var script = (PixelationEffect)target;

        GUILayout.Space(10);
        GUILayout.Label("Настройки пикселизации", EditorStyles.boldLabel);

        if (script.settings != null)
        {
            script.settings.pixelSize = EditorGUILayout.Slider("Pixel Size", script.settings.pixelSize, 1f, 128f);

            if (GUI.changed)
            {
                EditorUtility.SetDirty(script);
            }
        }
    }
}
