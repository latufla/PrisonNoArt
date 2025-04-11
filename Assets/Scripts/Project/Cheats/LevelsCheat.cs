using Cysharp.Threading.Tasks;
using Honeylab.Project.Levels;
using System;
using Zenject;


namespace Honeylab.Project.Cheats
{
    public class LevelsCheat : IInitializable, IDisposable
    {
        private readonly LevelsLoadService _levelsLoadService;

        private SROptions _srOptions;


        public LevelsCheat(LevelsLoadService levelsLoadService)
        {
            _levelsLoadService = levelsLoadService;
        }


        public void Initialize()
        {
            _srOptions = SROptions.Current;
            _srOptions.Level1Requested += LoadLevel1;
            _srOptions.Level2Requested += LoadLevel2;
        }


        public void Dispose()
        {
            _srOptions.Level1Requested -= LoadLevel1;
            _srOptions.Level2Requested -= LoadLevel2;
        }


        private void LoadLevel1()
        {
            _levelsLoadService.LeaveLevel();
            _levelsLoadService.LoadLevelInOrderAsync(0).Forget();
        }

        private void LoadLevel2()
        {
            _levelsLoadService.LeaveLevel();
            _levelsLoadService.LoadLevelInOrderAsync(1).Forget();
        }
    }
}