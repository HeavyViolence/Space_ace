using SpaceAce.Architecture;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SpaceAce
{
    namespace Main
    {
        public sealed class SpaceBackground : IInitializable, IUpdatable
        {
            public const float MinScrollSpeed = 0.001f;
            public const float MaxScrollSpeed = 0.01f;
            public const float DefaultScrollSpeed = 0.002f;

            private readonly List<Material> _spaceBackgroundMaterials;
            private readonly MeshRenderer _spaceBackgroundMeshRenderer;

            private float _scrollSpeed = DefaultScrollSpeed;

            public GameObject SpaceBackgroundAnchor { get; }
            public float ScrollSpeed { get => _scrollSpeed; set => _scrollSpeed = Mathf.Clamp(value, MinScrollSpeed, MaxScrollSpeed); }

            public SpaceBackground(GameObject spaceBackgroundPrefab, IEnumerable<Material> spaceBackgroundMaterials)
            {
                if (spaceBackgroundPrefab == null)
                {
                    throw new ArgumentNullException(nameof(spaceBackgroundPrefab),
                                                    "An empty space background prefab has been passed!");
                }

                SpaceBackgroundAnchor = UnityEngine.Object.Instantiate(spaceBackgroundPrefab, Vector3.zero, Quaternion.identity);

                if (spaceBackgroundMaterials is null)
                {
                    throw new ArgumentNullException(nameof(spaceBackgroundMaterials),
                                                    "An empty collection of space background materials has been passed!");
                }

                _spaceBackgroundMaterials = new(spaceBackgroundMaterials);

                if (SpaceBackgroundAnchor.TryGetComponent(out MeshRenderer renderer))
                {
                    _spaceBackgroundMeshRenderer = renderer;

                    int materialIndex = UnityEngine.Random.Range(0, _spaceBackgroundMaterials.Count);
                    _spaceBackgroundMeshRenderer.sharedMaterial = _spaceBackgroundMaterials[materialIndex];
                    _spaceBackgroundMeshRenderer.sharedMaterial.mainTextureOffset = new Vector2(0f, UnityEngine.Random.Range(0f, 1f));
                }
                else
                {
                    throw new MissingComponentException($"Passed {nameof(spaceBackgroundPrefab)} is missing component of type {typeof(MeshRenderer)}!");
                }
            }

            #region interfaces

            public void OnInitialize()
            {
                GameServices.Register(this);
            }

            public void OnSubscribe()
            {

            }

            public void OnUnsubscribe()
            {

            }

            public void OnClear()
            {
                GameServices.Deregister(this);
            }

            public void OnUpdate()
            {
                ScrollSpaceBackground();
            }

            #endregion

            private void ScrollSpaceBackground()
            {
                _spaceBackgroundMeshRenderer.sharedMaterial.mainTextureOffset += new Vector2(0f, ScrollSpeed * Time.deltaTime);
            }
        }
    }
}