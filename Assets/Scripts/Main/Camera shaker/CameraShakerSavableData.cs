using System;
using UnityEngine;

namespace SpaceAce.Main
{
    [Serializable]
    public sealed class CameraShakerSavableData
    {
        [SerializeField] private bool _shakingEnabled;

        public bool ShakingEnabled => _shakingEnabled;

        public CameraShakerSavableData(bool shakingEnabled)
        {
            _shakingEnabled = shakingEnabled;
        }
    }
}