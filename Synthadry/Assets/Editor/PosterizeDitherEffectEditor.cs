using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PosterizeDitherEffect))]
public class PosterizeDitherEffectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var script = (PosterizeDitherEffect)target;

        GUILayout.Space(10);
        GUILayout.Label("Настройки постеризации и дезеринга", EditorStyles.boldLabel);

        if (script.settings != null)
        {
            script.settings.posterizeLevels = EditorGUILayout.Slider("Posterize Levels", script.settings.posterizeLevels, 2, 32);
            script.settings.ditherAmount = EditorGUILayout.Slider("Dither Amount", script.settings.ditherAmount, 0f, 1f);

            if (GUI.changed)
            {
                EditorUtility.SetDirty(script);
            }
        }
    }
}
