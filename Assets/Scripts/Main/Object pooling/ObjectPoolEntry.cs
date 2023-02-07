using SpaceAce.Architecture;
using System;
using UnityEngine;

namespace SpaceAce.Main.ObjectPooling
{
    [CreateAssetMenu(fileName = "Object pool entry", menuName = "Space ace/Configs/Object pool entry")]
    public sealed class ObjectPoolEntry : ScriptableObject, IEquatable<ObjectPoolEntry>
    {
        [SerializeField] private GameObject _prefab = null;
        [SerializeField] private string _anchorName = string.Empty;

        private static readonly GameServiceFastAccess<MultiobjectPool> s_multiobjectPool = new();

        public GameObject Prefab => _prefab;
        public string AnchorName => _anchorName;

        public void EnsureObjectPoolExistence() => s_multiobjectPool.Access.EnsureObjectPoolExistence(AnchorName, Prefab);

        #region interfaces

        public override bool Equals(object obj) => Equals(obj as ObjectPoolEntry);

        public bool Equals(ObjectPoolEntry other) => other != null && other.AnchorName.Equals(AnchorName);

        public override int GetHashCode() => AnchorName.GetHashCode();

        public override string ToString() => $"{nameof(ObjectPoolEntry)} of {Prefab.name} as {AnchorName}";

        #endregion
    }
}