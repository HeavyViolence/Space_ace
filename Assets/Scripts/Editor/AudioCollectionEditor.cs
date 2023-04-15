using SpaceAce.Main.Audio;
using System.Collections;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;

namespace SpaceAce.Editors
{
    [CustomEditor(typeof(AudioCollection))]
    public sealed class AudioCollectionEditor : Editor
    {
        private const int MinConsecutiveAudioPreviewAmount = 2;
        private const int MaxConsecutiveAudioPreviewAmount = 10;
        private const int DefaultConsecutiveAudioPreviewAmount = 4;

        private const float MinConsecutiveAudioPreviewDelay = 0.1f;
        private const float MaxConsecutiveAudioPreviewDelay = 1f;
        private const float DefaultConsecutiveAudioPreviewDelay = 0.5f;

        private SerializedProperty _audioClips;

        private SerializedProperty _outputAudioGroup;

        private SerializedProperty _volume;
        private SerializedProperty _volumeRandomDeviation;

        private SerializedProperty _priority;

        private SerializedProperty _spatialBlend;
        private SerializedProperty _spatialBlendRandomDeviation;

        private SerializedProperty _pitch;
        private SerializedProperty _pitchRandomDeviation;

        private AudioCollection _target = null;
        private AudioSource _audioPreviewer = null;
        private int _lastAppliedSettingsHashSum = 0;

        private bool _consecutiveAudioPreviewEnabled = false;
        private int _consecutiveAudioPreviewAmount = DefaultConsecutiveAudioPreviewAmount;
        private float _consecutiveAudioPreviewDelay = DefaultConsecutiveAudioPreviewDelay;
        private EditorCoroutine _consecutiveAudioPreview = null;

        private void OnEnable()
        {
            _audioClips = serializedObject.FindProperty("_audioClips");

            _outputAudioGroup = serializedObject.FindProperty("_outputAudioGroup");

            _volume = serializedObject.FindProperty("_volume");
            _volumeRandomDeviation = serializedObject.FindProperty("_volumeRandomDeviation");

            _priority = serializedObject.FindProperty("_priority");

            _spatialBlend = serializedObject.FindProperty("_spatialBlend");
            _spatialBlendRandomDeviation = serializedObject.FindProperty("_spatialBlendRandomDeviation");

            _pitch = serializedObject.FindProperty("_pitch");
            _pitchRandomDeviation = serializedObject.FindProperty("_pitchRandomDeviation");

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
            EditorGUILayout.Slider(_volumeRandomDeviation, 0f, _volume.floatValue, "Max random deviation");

            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(_priority, new GUIContent("Audio priority"));

            EditorGUILayout.Separator();
            EditorGUILayout.Slider(_spatialBlend, AudioCollection.MinSpatialBlend, AudioCollection.MaxSpatialBlend, "Spatial blend");
            EditorGUILayout.Slider(_spatialBlendRandomDeviation, 0f, _spatialBlend.floatValue, "Max random deviation");

            EditorGUILayout.Separator();
            EditorGUILayout.Slider(_pitch, AudioCollection.MinPitch, AudioCollection.MaxPitch, "Pitch");
            EditorGUILayout.Slider(_pitchRandomDeviation, 0f, _pitch.floatValue, "Max random deviation");

            EditorGUILayout.Separator();
            _consecutiveAudioPreviewEnabled = EditorGUILayout.Toggle("Consecutive audio preview", _consecutiveAudioPreviewEnabled);

            if (_consecutiveAudioPreviewEnabled)
            {
                _consecutiveAudioPreviewAmount = EditorGUILayout.IntSlider("Amount",
                                                                           _consecutiveAudioPreviewAmount,
                                                                           MinConsecutiveAudioPreviewAmount,
                                                                           MaxConsecutiveAudioPreviewAmount);

                _consecutiveAudioPreviewDelay = EditorGUILayout.Slider("Delay",
                                                                       _consecutiveAudioPreviewDelay,
                                                                       MinConsecutiveAudioPreviewDelay,
                                                                       MaxConsecutiveAudioPreviewDelay);
            }

            EditorGUILayout.Separator();

            if (GUILayout.Button("Preview audio"))
            {
                if (_target.AudioClipsAmount == 0)
                {
                    return;
                }

                int currentSettingsHashSum = GetCurrentSettingsHashSum();

                if (_lastAppliedSettingsHashSum != currentSettingsHashSum)
                {
                    _lastAppliedSettingsHashSum = currentSettingsHashSum;
                    _target.ApplySettings();
                }

                if (_audioPreviewer != null)
                {
                    _audioPreviewer.Stop();
                    DestroyImmediate(_audioPreviewer.gameObject);
                    _audioPreviewer = null;
                }

                if (_consecutiveAudioPreviewEnabled)
                {
                    _consecutiveAudioPreview = EditorCoroutineUtility.StartCoroutineOwnerless(ConsecutiveAudioPreview(_consecutiveAudioPreviewAmount,
                                                                                                                      _consecutiveAudioPreviewDelay));
                }
                else
                {
                    AudioPreview();
                }
            }

            if (GUILayout.Button("Stop audio preview"))
            {
                if (_consecutiveAudioPreviewEnabled)
                {
                    if (_consecutiveAudioPreview != null)
                    {
                        EditorCoroutineUtility.StopCoroutine(_consecutiveAudioPreview);
                        _consecutiveAudioPreview = null;
                    }
                }
                else
                {
                    if (_audioPreviewer != null && _audioPreviewer.isPlaying)
                    {
                        _audioPreviewer.Stop();
                        DestroyImmediate(_audioPreviewer.gameObject);
                        _audioPreviewer = null;
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void AudioPreview()
        {
            GameObject audioPreviewObject = EditorUtility.CreateGameObjectWithHideFlags("Audio preview", hideFlags);
            _audioPreviewer = audioPreviewObject.AddComponent<AudioSource>();

            _audioPreviewer.clip = _target.RandomAudioClip;
            _audioPreviewer.spatialBlend = _target.SpatialBlend.RandomValue;
            _audioPreviewer.pitch = _target.Pitch.RandomValue;
            _audioPreviewer.volume = _target.Volume.RandomValue;
            _audioPreviewer.outputAudioMixerGroup = _target.OutputAudioGroup;

            _audioPreviewer.Play();
        }

        private IEnumerator ConsecutiveAudioPreview(int amount, float delay)
        {
            GameObject consecutiveAudioPreviewObject = EditorUtility.CreateGameObjectWithHideFlags("Audio preview", hideFlags);
            _audioPreviewer = consecutiveAudioPreviewObject.AddComponent<AudioSource>();

            for (int i = 0; i < amount; i++)
            {
                _audioPreviewer.clip = _target.RandomAudioClip;
                _audioPreviewer.spatialBlend = _target.SpatialBlend.RandomValue;
                _audioPreviewer.pitch = _target.Pitch.RandomValue;
                _audioPreviewer.volume = _target.Volume.RandomValue;
                _audioPreviewer.outputAudioMixerGroup = _target.OutputAudioGroup;

                _audioPreviewer.Play();

                yield return new EditorWaitForSeconds(delay);
            }
        }

        private int GetCurrentSettingsHashSum()
        {
            int sum = 0;

            sum += _volume.floatValue.GetHashCode();
            sum += _volumeRandomDeviation.floatValue.GetHashCode();

            sum += _spatialBlend.floatValue.GetHashCode();
            sum += _spatialBlendRandomDeviation.floatValue.GetHashCode();

            sum += _pitch.floatValue.GetHashCode();
            sum += _pitchRandomDeviation.floatValue.GetHashCode();

            return sum;
        }
    }
}