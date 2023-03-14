using SpaceAce.Gameplay.Shooting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GunConfig))]
public abstract class GunConfigEditor : Editor
{
    private SerializedProperty _gunIcon;

    private SerializedProperty _fireDuration;
    private SerializedProperty _fireDurationRandomDeviation;

    private SerializedProperty _cooldown;
    private SerializedProperty _cooldownRandomDeviation;

    private SerializedProperty _fireAudio;

    protected virtual void OnEnable()
    {
        _gunIcon = serializedObject.FindProperty("_gunIcon");

        _fireDuration = serializedObject.FindProperty("_fireDuration");
        _fireDurationRandomDeviation = serializedObject.FindProperty("_fireDurationRandomDeviation");

        _cooldown = serializedObject.FindProperty("_cooldown");
        _cooldownRandomDeviation = serializedObject.FindProperty("_cooldownRandomDeviation");

        _fireAudio = serializedObject.FindProperty("_fireAudio");
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(_gunIcon, new GUIContent("Gun icon"));

        EditorGUILayout.Separator();
        EditorGUILayout.Slider(_fireDuration, GunConfig.MinFireDuration, GunConfig.MaxFireDuration, "Fire duration");
        EditorGUILayout.Slider(_fireDurationRandomDeviation, 0f, _fireDuration.floatValue, "Max random deviation");

        EditorGUILayout.Separator();
        EditorGUILayout.Slider(_cooldown, GunConfig.MinCooldown, GunConfig.MaxCooldown, "Cooldown");
        EditorGUILayout.Slider(_cooldownRandomDeviation, 0f, _cooldown.floatValue, "Max random deviation");

        EditorGUILayout.Separator();
        EditorGUILayout.PropertyField(_fireAudio, new GUIContent("Fire audio"));
    }
}
