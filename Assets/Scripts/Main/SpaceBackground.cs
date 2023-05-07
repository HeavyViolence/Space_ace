using SpaceAce.Architecture;
using System.Collections.Generic;
using UnityEngine;
using System;
using SpaceAce.Auxiliary;

namespace SpaceAce.Main
{
    public sealed class SpaceBackground : IGameService, IUpdatable
    {
        public const float MinScrollSpeed = 0.001f;
        public const float MaxScrollSpeed = 0.01f;
        public const float DefaultScrollSpeed = 0.002f;

        private readonly Material _mainMenuSpaceBackground;
        private readonly List<Material> _spaceBackgrounds;
        private readonly MeshRenderer _spaceBackgroundMeshRenderer;
        private readonly ParticleSystem _dustfield;

        private float _scrollSpeed = MinScrollSpeed;

        public GameObject SpaceBackgroundAnchor { get; }
        public float ScrollSpeed { get => _scrollSpeed; set => _scrollSpeed = Mathf.Clamp(value, MinScrollSpeed, MaxScrollSpeed); }

        public SpaceBackground(Vector2 viewportLowerLeftPoint,
                               Vector2 viewportLowerRightPoint,
                               float aspectRatio,
                               float widthDelta,
                               Material mainMenuSpaceBackground,
                               IEnumerable<Material> spaceBackgrounds,
                               GameObject dustfieldPrefab)
        {
            if (mainMenuSpaceBackground == null)
            {
                throw new ArgumentNullException(nameof(mainMenuSpaceBackground),
                                                "An empty main menu space background material has been passed!");
            }

            _mainMenuSpaceBackground = mainMenuSpaceBackground;

            if (spaceBackgrounds is null)
            {
                throw new ArgumentNullException(nameof(spaceBackgrounds),
                                                "An empty collection of space background materials has been passed!");
            }

            _spaceBackgrounds = new(spaceBackgrounds);

            var (anchor, renderer) = ConstructSpaceBackground(viewportLowerLeftPoint, viewportLowerRightPoint, aspectRatio, widthDelta);

            SpaceBackgroundAnchor = anchor;
            _spaceBackgroundMeshRenderer = renderer;

            _spaceBackgroundMeshRenderer.sharedMaterial = mainMenuSpaceBackground;
            _spaceBackgroundMeshRenderer.sharedMaterial.mainTextureOffset = new Vector2(0f, AuxMath.RandomNormal);

            if (dustfieldPrefab == null)
            {
                throw new ArgumentNullException(nameof(dustfieldPrefab), $"Attempted to pass an empty dustfield {nameof(ParticleSystem)} prefab!");
            }

            var dustfieldAhchor = UnityEngine.Object.Instantiate(dustfieldPrefab, Vector3.zero, Quaternion.identity);
            dustfieldAhchor.transform.parent = SpaceBackgroundAnchor.transform;

            if (dustfieldAhchor.TryGetComponent(out ParticleSystem system))
            {
                _dustfield = system;
            }
            else
            {
                throw new MissingComponentException($"Passed dustfield prefab doesn't contain a {nameof(ParticleSystem)} component!");
            }
        }

        private (GameObject anchor, MeshRenderer renderer) ConstructSpaceBackground(Vector2 viewportLowerLeftPoint,
                                                                                    Vector2 viewportLowerRightPoint,
                                                                                    float aspectRatio,
                                                                                    float widthDelta)
        {
            GameObject spaceBackgroundAnchor = new("Space background");

            var meshFilter = spaceBackgroundAnchor.AddComponent<MeshFilter>();
            var meshRenderer = spaceBackgroundAnchor.AddComponent<MeshRenderer>();

            float meshWidth = (viewportLowerRightPoint - viewportLowerLeftPoint).magnitude * (1f + widthDelta);
            float meshHeight = meshWidth * aspectRatio;

            Vector3[] vertices = new Vector3[]
            {
                    new Vector3(-1f * meshWidth / 2f, -1f * meshHeight / 2f, 0f),
                    new Vector3(meshWidth / 2f, -1f * meshHeight / 2f, 0f),
                    new Vector3(meshWidth / 2f, meshHeight / 2f, 0f),
                    new Vector3(-1f * meshWidth / 2f, meshHeight / 2f, 0f)
            };

            meshFilter.mesh.vertices = vertices;

            int[] triangles = new int[] { 0, 3, 1, 1, 3, 2 };
            meshFilter.mesh.triangles = triangles;

            meshFilter.mesh.RecalculateNormals();

            Vector2[] uv = new Vector2[]
            {
                    new Vector2(0f, 0f),
                    new Vector2(1f, 0f),
                    new Vector2(1f, 1f),
                    new Vector2(0f, 1f)
            };

            meshFilter.mesh.uv = uv;
            meshFilter.mesh.RecalculateTangents();

            return (spaceBackgroundAnchor, meshRenderer);
        }

        #region interfaces

        public void OnInitialize()
        {
            GameServices.Register(this);
        }

        public void OnSubscribe()
        {
            if (GameServices.TryGetService(out GameModeLoader loader) == true)
            {
                loader.LevelLoaded += LevelLoadedEventHandler;
                loader.MainMenuLoaded += MainMenuLoadedEventHandler;
            }
            else
            {
                throw new UnregisteredGameServiceAccessAttemptException(typeof(GameModeLoader));
            }
        }

        public void OnUnsubscribe()
        {
            if (GameServices.TryGetService(out GameModeLoader loader) == true)
            {
                loader.LevelLoaded -= LevelLoadedEventHandler;
                loader.MainMenuLoaded -= MainMenuLoadedEventHandler;
            }
            else
            {
                throw new UnregisteredGameServiceAccessAttemptException(typeof(GameModeLoader));
            }
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

        #region event handlers

        private void LevelLoadedEventHandler(object sender, LevelLoadedEventArgs e)
        {
            ScrollSpeed = DefaultScrollSpeed;
            _dustfield.Play();

            int materialIndex = UnityEngine.Random.Range(0, _spaceBackgrounds.Count);
            _spaceBackgroundMeshRenderer.sharedMaterial = _spaceBackgrounds[materialIndex];
        }

        private void MainMenuLoadedEventHandler(object sender, EventArgs e)
        {
            ScrollSpeed = MinScrollSpeed;
            _dustfield.Stop();
            _spaceBackgroundMeshRenderer.sharedMaterial = _mainMenuSpaceBackground;
        }

        #endregion
    }
}