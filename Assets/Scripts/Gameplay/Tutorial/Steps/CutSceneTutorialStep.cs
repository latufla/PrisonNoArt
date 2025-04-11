using Cysharp.Threading.Tasks;
using Honeylab.Gameplay.Player;
using Honeylab.Utils.CameraTargeting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Playables;


namespace Honeylab.Gameplay.Tutorial
{
    public class CutSceneTutorialStep : TutorialStepBase
    {
        [SerializeField] private PlayableDirector _director;
        [SerializeField] private Transform _character;


        protected override List<Func<UniTask>> CreateSubSteps(CancellationToken ct) => new()
        {
            async () => await PlayCutSceneAsync(ct)
        };


        private async UniTask PlayCutSceneAsync(CancellationToken ct)
        {
            using (BlockPlayerInput())
            {
                PlayerFlow player = await GetObjectFirstAsync<PlayerFlow>(ct);
                CharacterController controller = player.GetComponent<CharacterController>();

                if (_character != null)
                {
                    Vector3 destination = _character.position;
                    PlayerMotion motion = player.Motion;

                    player.View.Animations.PlayWalk();
                    await MoveTo(motion, destination, ct);
                    player.View.Animations.PlayIdle();

                    var interaction = player.Get<PlayerInteraction>();
                    interaction.ExitInteract();

                    player.gameObject.SetActive(false);
                }

                _director.gameObject.SetActive(true);

                ICameraTargetingHandle cameraHandler = ChangeCameraTarget(_character);

                double duration = _director.playableAsset.duration;
                _director.Play();

                await UniTask.Delay(TimeSpan.FromSeconds(duration), cancellationToken: ct);

                _director.Stop();

                if (_character != null)
                {
                    Vector3 endPosition = _character.transform.position;
                    controller.enabled = false;
                    controller.transform.position = endPosition;
                    controller.enabled = true;

                    player.gameObject.SetActive(true);
                }
                _director.gameObject.SetActive(false);
                cameraHandler.Finish();
            }
        }


        private async UniTask MoveTo(PlayerMotion motion, Vector3 destination, CancellationToken ct)
        {
            float distance = CalcDistance(motion, destination);
            while (distance > 0.1f)
            {
                await UniTask.Yield(ct);

                Vector3 direction = destination - motion.transform.position;
                motion.UpdateMove(1.0f, direction, Time.deltaTime);
                distance = CalcDistance(motion, destination);
            }
        }


        private float CalcDistance(PlayerMotion motion, Vector3 destination) =>
            Vector3.Distance(motion.transform.position, destination);


        public override TutorialInfo GetTutorialInfo() => null;


        protected override void OnSubStepComplete()
        {

        }
    }
}
