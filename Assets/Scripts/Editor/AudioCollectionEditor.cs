using SpaceAce.Main.Audio;
using UnityEditor;
using UnityEngine;

namespace SpaceAce.Editors
{
    [CustomEditor(typeof(AudioCollection))]
    public sealed class AudioCollectionEditor : Editor
    {
        private SerializedProperty _audioClips;

        private SerializedProperty _outputAudioGroup;

        private SerializedProperty _volume;
        private SerializedProperty _volumeDeviation;

        private SerializedProperty _priority;

        private SerializedProperty _spatialBlend;
        private SerializedProperty _spatialBlendDeviation;

        private SerializedProperty _pitch;
        private SerializedProperty _pitchDeviation;

        private AudioCollection _target = null;
        private AudioSource _audioPreviewer = null;

        private void OnEnable()
        {
            _audioClips = serializedObject.FindProperty("_audioClips");

            _outputAudioGroup = serializedObject.FindProperty("_outputAudioGroup");

            _volume = serializedObject.FindProperty("_volume");
            _volumeDeviation = serializedObject.FindProperty("_volumeDeviation");

            _priority = serializedObject.FindProperty("_priority");

            _spatialBlend = serializedObject.FindProperty("_spatialBlend");
            _spatialBlendDeviation = serializedObject.FindProperty("_spatialBlendDeviation");

            _pitch = serializedObject.FindProperty("_pitch");
            _pitchDeviation = serializedObject.FindProperty("_pitchDeviation");

            _target = (AudioCollection)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_audioClips, new GUIContent("Audio clips"));

            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(_outputAudioGroup, new GUIContent("Output audio mixer group"));

            EditorGUILayout.Separator();
            EditorGUILayout.Slider(_volume, 0f, 1f, "Volume 1 unit away");
            EditorGUILayout.Slider(_volumeDeviation, 0f, _volume.floatValue, "Random deviation");

            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(_priority, new GUIContent("Audio priority"));

            EditorGUILayout.Separator();
            EditorGUILayout.Slider(_spatialBlend, 0f, 1f, "Spatial blend");
            EditorGUILayout.Slider(_spatialBlendDeviation, 0f, _spatialBlend.floatValue, "Random deviation");

            EditorGUILayout.Separator();
            EditorGUILayout.Slider(_pitch, AudioCollection.MinPitch, AudioCollection.MaxPitch, "Pitch");
            EditorGUILayout.Slider(_pitchDeviation, 0f, _pitch.floatValue, "Random deviation");

            EditorGUILayout.Separator();

            if (GUILayout.Button("Preview audio"))
            {
                if (_target.ClipsAmount == 0)
                {
                    return;
                }

                if (_audioPreviewer != null)
                {
                    _audioPreviewer.Stop();
                    DestroyImmediate(_audioPreviewer.gameObject);
                    _audioPreviewer = null;
                }

                GameObject audioPreviewObject = EditorUtility.CreateGameObjectWithHideFlags("Audio preview", hideFlags);
                _audioPreviewer = audioPreviewObject.AddComponent<AudioSource>();

                _audioPreviewer.clip = _target.RandomAudioClip;
                _audioPreviewer.spatialBlend = 0f;
                _audioPreviewer.pitch = _target.Pitch;
                _audioPreviewer.volume = _target.Volume;
                _audioPreviewer.outputAudioMixerGroup = _target.OutputAudioGroup;

                _audioPreviewer.Play();
            }

            if (GUILayout.Button("Stop audio preview"))
            {
                if (_audioPreviewer != null && _audioPreviewer.isPlaying)
                {
                    _audioPreviewer.Stop();
                    DestroyImmediate(_audioPreviewer.gameObject);
                    _audioPreviewer = null;
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}