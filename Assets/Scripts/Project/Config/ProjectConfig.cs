using System;
using UnityEngine;


namespace Honeylab.Project
{
    [Serializable]
    public class ProjectConfig
    {
        [SerializeField] [Min(30)] private int _targetFps;
        [SerializeField] private bool _isLogEnabled;
        [SerializeField] private bool _debugAreasEnabled;

        public int TargetFps => _targetFps;
        public bool IsLogEnabled => _isLogEnabled;
        public bool DebugAreasEnabled => _debugAreasEnabled;
    }
}
