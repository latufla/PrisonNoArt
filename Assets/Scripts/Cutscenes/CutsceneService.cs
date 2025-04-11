using Cysharp.Threading.Tasks;
using Honeylab.Gameplay.World;
using Honeylab.Persistence;
using Honeylab.Utils.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Playables;


namespace Honeylab.Cutscene
{
    [Serializable]
    public class CutsceneArgs
    {
        [SerializeField] private CutsceneInfo _startCutsceneInfo;
        [SerializeField] private WorldObjectId _startCutsceneWeaponCollectId;
        [SerializeField] private List<CutsceneInfo> _cutsceneInfos;

        public CutsceneInfo StartCutsceneInfo => _startCutsceneInfo;
        public List<CutsceneInfo> CutsceneInfos => _cutsceneInfos;
        public WorldObjectId StartCutsceneWeaponCollectId => _startCutsceneWeaponCollectId;


        public CutsceneInfo GetCutsceneInfo(PersistenceId id)
        {
            return StartCutsceneInfo.Id != null && StartCutsceneInfo.Id.Equals(id) ?
                StartCutsceneInfo :
                CutsceneInfos.FirstOrDefault(it => it.Id.Equals(id));
        }


        public CutsceneInfo GetCutsceneInfoByIndex(int index) => _cutsceneInfos[index];


        [Serializable]
        public class CutsceneInfo
        {
            public PersistenceId Id;
            public PlayableDirector Cutscene;
            public bool DontSave;
            public List<GameObject> ObjectsDeactivate;
        }
    }


    public class CutsceneService
    {
        private readonly CutsceneArgs _args;
        private readonly LevelPersistenceService _levelPersistenceService;
        private readonly WorldObjectsService _world;

        private CutscenePersistentComponent _isFinishedProp;
        private WorldObjectFlow _startCutsceneWeaponCollect;
        [Obsolete("Make better state handling")]
        private bool _cutSceneInProgress;


        private CutsceneService(CutsceneArgs args,
            LevelPersistenceService levelPersistenceService,
            WorldObjectsService world)
        {
            _args = args;
            _levelPersistenceService = levelPersistenceService;
            _world = world;
        }


        public void Init() { }


        public async UniTask RunAsync(CancellationToken ct)
        {
            var startCutscene = _args.StartCutsceneInfo;
            if (startCutscene.Id == null)
            {
                return;
            }

            _startCutsceneWeaponCollect = _world.GetObject<WorldObjectFlow>(_args.StartCutsceneWeaponCollectId);
            if (_startCutsceneWeaponCollect)
                _startCutsceneWeaponCollect.gameObject.SetActive(false);

            await PlayCutsceneAsync(startCutscene.Id, ct, startCutscene.DontSave);

            if (_startCutsceneWeaponCollect)
                _startCutsceneWeaponCollect.gameObject.SetActive(true);
        }


        public async UniTask PlayCutsceneAsync(PersistenceId cutsceneId,
            CancellationToken ct,
            bool dontSave = false,
            bool inPause = false)
        {
            _cutSceneInProgress = true;
            _levelPersistenceService.TryGet(cutsceneId, out PersistentObject po);

            if (po != null)
            {
                _isFinishedProp = po.GetOrAdd<CutscenePersistentComponent>();
                _cutSceneInProgress = false;
                return;
            }


            await PlayCutscene(cutsceneId, ct, inPause);

            _cutSceneInProgress = false;
            if (!dontSave)
            {
                Save(cutsceneId);
            }
        }


        public bool HasCutScene() => _args.StartCutsceneInfo.Id != null;
        public bool IsCutSceneFinished() => HasCutScene() && _cutSceneInProgress is false;


        private async UniTask PlayCutscene(PersistenceId cutsceneId, CancellationToken ct, bool inPause)
        {
            var cutsceneInfo = _args.GetCutsceneInfo(cutsceneId);
            var cutscene = cutsceneInfo.Cutscene;

            if (inPause)
            {
                Time.timeScale = 0;
                cutscene.timeUpdateMode = DirectorUpdateMode.UnscaledGameTime;
            }

            foreach (var obj in cutsceneInfo.ObjectsDeactivate)
            {
                obj.SetActive(false);
            }

            cutscene.gameObject.SetActive(true);
            cutscene.Play();

            await UniTask.Delay(TimeSpan.FromSeconds(cutscene.duration),
                ignoreTimeScale: inPause,
                cancellationToken: ct);

            cutscene.Stop();
            cutscene.gameObject.SetActive(false);

            foreach (var obj in cutsceneInfo.ObjectsDeactivate)
            {
                obj.SetActive(true);
            }

            if (inPause)
            {
                Time.timeScale = 1;
                cutscene.timeUpdateMode = DirectorUpdateMode.GameTime;
            }
        }


        private void Save(PersistenceId cutsceneId)
        {
            PersistentObject po = _levelPersistenceService.Create(cutsceneId);
            _isFinishedProp = po.Add<CutscenePersistentComponent>();
            _isFinishedProp.Value = true;
        }
    }
}
