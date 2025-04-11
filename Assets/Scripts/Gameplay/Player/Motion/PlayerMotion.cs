using Cysharp.Threading.Tasks;
using Honeylab.Gameplay.Cameras;
using Honeylab.Gameplay.Creatures;
using Honeylab.Gameplay.World;
using Honeylab.Utils.Extensions;
using System.Threading;
using UniRx;
using UnityEngine;


namespace Honeylab.Gameplay.Player
{
    public class PlayerMotion : WorldObjectComponentBase
    {
        [SerializeField] private CharacterController _controller;

        private float _gravitationalVelocityPart;

        private readonly ReactiveProperty<bool> _isMoving = new();
        public IReadOnlyReactiveProperty<bool> IsMoving => _isMoving;

        private Vector3 _moveDirection;
        private float _speed;
        private bool _isFinght;

        private PlayerInputService _playerInput;
        private CameraProvider _cameras;
        [SerializeField]
        private PlayerAnimations _animations;
        private Vector3 _gunDirection;

        private Knockback _knockback;
        private CancellationTokenSource _cts;


        public void SetSpeed(float speed)
        {
            _speed = speed;
        }


        public float GetSpeed() => _speed;


        protected override void OnRun()
        {
            WorldObjectFlow flow = GetFlow();
            _playerInput = flow.Resolve<PlayerInputService>();
            _cameras = flow.Resolve<CameraProvider>();
            _knockback = GetFlow().Get<Knockback>();
            _cts = new CancellationTokenSource();
            RunLoopAsync(_cts.Token).Forget();
        }


        protected override void OnClear()
        {
            _cts?.CancelThenDispose();
            _cts = null;
        }


        private async UniTaskVoid RunLoopAsync(CancellationToken ct)
        {
            _controller.Move(Vector3.forward * 0.02f);

            while (true)
            {
                await UniTask.Yield(PlayerLoopTiming.FixedUpdate, ct);

                float deltaTime = Time.fixedDeltaTime;
                if (_controller.isGrounded)
                {
                    _gravitationalVelocityPart = 0.0f;
                }
                else
                {
                    _gravitationalVelocityPart += -9.81f * deltaTime;
                }

                float inputPower = _playerInput.GetPower();
                Vector2 inputDirection = _playerInput.GetDirection();

                bool isMoving = inputPower > 0.0f;
                if (isMoving && !_knockback.IsKnockback)
                {
                    _moveDirection = CalculateMoveDirection(inputDirection);
                    if (_isFinght is false)
                    {
                        _controller.transform.rotation = Quaternion.LookRotation(_moveDirection);
                        _animations.PlayWalk();
                    }
                    else
                    {
                        _animations.PlayGunWalk(_gunDirection.z, _gunDirection.x);
                    }
                }
                else
                {
                    _moveDirection = Vector3.zero;
                }

                UpdateMove(inputPower, _moveDirection, deltaTime);

                _isMoving.Value = isMoving;
            }
        }


        public void SetFight(bool value)
        {
            _isFinght = value;
        }


        public void Teleport(Vector3 position, Quaternion rotation)
        {
            SetControllerEnabled(false);
            Transform controllerTransform = _controller.transform;
            controllerTransform.SetPositionAndRotation(position, rotation);
            SetControllerEnabled(true);
        }


        public void SetControllerEnabled(bool isEnabled)
        {
            _controller.enabled = isEnabled;
        }


        public Vector3 GetMoveDirection() => _moveDirection;


        private Vector3 CalculateMoveDirection(Vector2 inputDirection)
        {
            float cameraY = _cameras.GameCamera.transform.rotation.eulerAngles.y;

            Vector3 moveDirection = Quaternion.Euler(0.0f, cameraY, 0.0f) *
                new Vector3(inputDirection.x, 0.0f, inputDirection.y);
            moveDirection.Normalize();
            return moveDirection;
        }


        public void UpdateMove(float inputPower, Vector3 moveDirection, float deltaTime)
        {
            Vector3 planarVelocity = _speed *
                inputPower *
                moveDirection;
            Vector3 planarDeltaMove = deltaTime * planarVelocity;

            Vector3 gravitationalVelocity = _gravitationalVelocityPart * Vector3.up;
            Vector3 gravitationalDeltaMove = deltaTime * gravitationalVelocity;
            Vector3 deltaMove = planarDeltaMove + gravitationalDeltaMove;

            if (deltaMove.magnitude > 0.00001f)
            {
                _controller.Move(deltaMove);
            }
        }


        public void SetTargetDirrection(Vector3 directionNormalized)
        {
            _gunDirection = directionNormalized;
        }
    }
}
