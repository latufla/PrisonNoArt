using Cysharp.Threading.Tasks;
using Honeylab.Gameplay.Tutorial;
using System.Threading;
using UnityEngine;
using Zenject;


namespace Honeylab.Utils.Tutorial
{
    public class TutorialStepContext : MonoBehaviour
    {
        [SerializeField] private GameObjectContext _context;
        private TutorialStepBase _step;

        public TutorialStepBase Step => _step;


        [Inject]
        public void Construct(TutorialStepBase step)
        {
            _step = step;
        }


        public void Init()
        {
            _context.Run();
            _step.Init();
        }


        public async UniTask RunAsync(CancellationToken ct) => await _step.RunAsync(ct);


        public void Clear()
        {
            _step.Clear();
        }
    }
}
