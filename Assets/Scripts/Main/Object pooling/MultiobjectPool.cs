using SpaceAce.Architecture;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace SpaceAce.Main.ObjectPooling
{
    public sealed class MultiobjectPool : IGameService
    {
        private static readonly GameServiceFastAccess<GamePauser> s_gamePauser = new();

        private readonly Dictionary<string, ObjectPool<GameObject>> _multiobjectPool = new();
        private readonly Dictionary<string, GameObject> _poolAnchors = new();

        private readonly GameObject _masterAnchor;

        public MultiobjectPool()
        {
            _masterAnchor = new GameObject("Multiobject pool");
        }

        public void EnsureObjectPoolExistence(string anchorName, GameObject sample)
        {
            if (string.IsNullOrEmpty(anchorName) || string.IsNullOrWhiteSpace(anchorName))
                throw new ArgumentNullException(nameof(anchorName));

            if (sample == null) throw new ArgumentNullException(nameof(sample));

            if (_multiobjectPool.ContainsKey(anchorName) == false)
            {
                GameObject poolAnchor = new($"Object pool of: {anchorName}");
                poolAnchor.transform.parent = _masterAnchor.transform;
                _poolAnchors.Add(anchorName, poolAnchor);

                ObjectPool<GameObject> newPool = new(OnCreate(sample), OnGet, OnRelease, OnDestroy);
                _multiobjectPool.Add(anchorName, newPool);
            }
        }

        public void ClearObjectPool(string anchorName)
        {
            if (_multiobjectPool.TryGetValue(anchorName, out var pool) == true)
            {
                pool.Clear();
                _multiobjectPool.Remove(anchorName);

                if (_poolAnchors.TryGetValue(anchorName, out var anchor) == true)
                {
                    _poolAnchors.Remove(anchorName);
                    UnityEngine.Object.Destroy(anchor);
                }
            }
            else
            {
                throw new MissingObjectPoolException(anchorName);
            }
        }

        public GameObject GetObject(string anchorName)
        {
            if (_multiobjectPool.TryGetValue(anchorName, out var pool) == true)
            {
                GameObject obj = pool.Get();

                if (obj.transform.parent == null &&
                    _poolAnchors.TryGetValue(anchorName, out var anchor) == true)
                {
                    obj.transform.parent = anchor.transform;
                }

                return obj;
            }
            else
            {
                throw new MissingObjectPoolException(anchorName);
            }
        }

        public void ReleaseObject(string anchorName, GameObject member, Func<bool> condition, float delay = 0f)
        {
            if (member == null) throw new ArgumentNullException(nameof(member));

            if (member.activeInHierarchy == false) return;

            CoroutineRunner.RunRoutine(ReleaseObjectConditionally());

            IEnumerator ReleaseObjectConditionally()
            {
                while (condition() == false || s_gamePauser.Access.Paused == true) yield return null;

                if (delay > 0f)
                {
                    float timer = 0f;

                    while (timer < delay)
                    {
                        timer += Time.deltaTime;

                        yield return null;
                        while (s_gamePauser.Access.Paused == true) yield return null;
                    }
                }

                yield return null;

                if (member.activeInHierarchy == true)
                {
                    if (_multiobjectPool.TryGetValue(anchorName, out var pool) == true) pool.Release(member);
                    else throw new MissingObjectPoolException(anchorName);
                }
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

            foreach (var element in _multiobjectPool) element.Value.Dispose();
            _multiobjectPool.Clear();

            foreach (var element in _poolAnchors) UnityEngine.Object.Destroy(element.Value);
            _poolAnchors.Clear();
        }

        #endregion

        private Func<GameObject> OnCreate(GameObject prefab) => delegate ()
        {
            return UnityEngine.Object.Instantiate(prefab, Vector3.zero, Quaternion.identity);
        };

        private Action<GameObject> OnGet => delegate (GameObject obj)
        {
            obj.SetActive(true);
        };

        private Action<GameObject> OnRelease => delegate (GameObject obj)
        {
            obj.SetActive(false);
            obj.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        };

        private Action<GameObject> OnDestroy => delegate (GameObject obj)
        {
            UnityEngine.Object.Destroy(obj);
        };
    }
}