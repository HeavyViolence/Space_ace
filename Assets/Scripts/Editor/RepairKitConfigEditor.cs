using SpaceAce.Editors;
using SpaceAce.Gameplay.Inventories;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RepairKitConfig))]
public sealed class RepairKitConfigEditor : InventoryItemConfigEditor
{
    private SerializedProperty _regenPerSecond;
    private SerializedProperty _regenPerSecondRandomDeviation;

    protected override void OnEnable()
    {
        base.OnEnable();

        _regenPerSecond = serializedObject.FindProperty("_regenPerSecond");
        _regenPerSecondRandomDeviation = serializedObject.FindProperty("_regenPerSecondRandomDeviation");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        EditorGUILayout.Separator();
        EditorGUILayout.Slider(_regenPerSecond, RepairKit.MinRegenPerSecond, RepairKit.MaxRegenPerSecond, "Regen per second");
        EditorGUILayout.Slider(_regenPerSecondRandomDeviation, 0f, _regenPerSecond.floatValue, "Random deviation");

        _regenPerSecondRandomDeviation.floatValue = Mathf.Clamp(_regenPerSecondRandomDeviation.floatValue, 0f, _regenPerSecond.floatValue);

        EditorGUILayout.Separator();

        if (GUILayout.Button("Apply settings"))
        {
            var config = target as RepairKitConfig;
            config.ApplySettings();
        }

        serializedObject.ApplyModifiedProperties();
    }
}
