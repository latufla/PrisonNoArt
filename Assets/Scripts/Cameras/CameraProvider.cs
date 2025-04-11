using Cinemachine;
using Honeylab.Utils.Bounds;
using System;
using UnityEngine;


namespace Honeylab.Gameplay.Cameras
{
    [Serializable]
    public class CameraProvider
    {
        [SerializeField] private Camera _gameCamera;
        [SerializeField] private CinemachineVirtualCamera _playerCamera;
        [SerializeField] private Camera _minimapCamera;
        [SerializeField] private CompoundBounds _minimapCameraBounds;

        public Camera GameCamera => _gameCamera;
        public CinemachineVirtualCamera PlayerCamera => _playerCamera;
        public Camera MinimapCamera => _minimapCamera;
        public CompoundBounds MinimapCameraBounds => _minimapCameraBounds;
    }
}
