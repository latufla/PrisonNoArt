using Honeylab.Utils.Persistence;
using System;
using UnityEngine;
using Zenject;


namespace Honeylab.Project.Cheats
{
    public class SaveCheat : IInitializable, IDisposable
    {
        private readonly SharedPersistenceService _sharedPersistenceService;
        private readonly LevelPersistenceService _levelPersistenceService;

        private SROptions _srOptions;


        public SaveCheat(SharedPersistenceService sharedPersistenceService, LevelPersistenceService levelPersistenceService)
        {
            _sharedPersistenceService = sharedPersistenceService;
            _levelPersistenceService = levelPersistenceService;
        }


        public void Initialize()
        {
            _srOptions = SROptions.Current;
            _srOptions.AutoRequested += SetAuto;
            _srOptions.ManualRequested += SetManual;
            _srOptions.SaveRequested += Save;
            _srOptions.CopySaveFileRequested += CopySaveFile;
            _srOptions.CopySaveFileClipboardRequested += CopySaveFileClipboard;
        }


        public void Dispose()
        {
            _srOptions.AutoRequested -= SetAuto;
            _srOptions.ManualRequested -= SetManual;
            _srOptions.SaveRequested -= Save;
            _srOptions.CopySaveFileRequested -= CopySaveFile;
            _srOptions.CopySaveFileClipboardRequested -= CopySaveFileClipboard;
        }


        private void CopySaveFile()
        {
            _sharedPersistenceService.SaveJson("save_shared.json");
            _levelPersistenceService.SaveJson("save_level.json");
        }


        private void CopySaveFileClipboard()
        {
            string saveStr = _sharedPersistenceService.GetSave();
            string levelSaveStr = _levelPersistenceService.GetSave();
            saveStr = string.Concat(saveStr, ",", levelSaveStr);
            GUIUtility.systemCopyBuffer = saveStr;
        }


        private void SetAuto()
        {
            _sharedPersistenceService.SetPause(false);
            _levelPersistenceService.SetPause(false);
            _isManual = false;
        }


        private bool _isManual;
        private void SetManual()
        {
            _sharedPersistenceService.SetPause(true);
            _levelPersistenceService.SetPause(true);
            _isManual = true;
        }


        private void Save()
        {
            _sharedPersistenceService.SetPause(false);
            _sharedPersistenceService.Save();
            _sharedPersistenceService.SetPause(_isManual);

            _levelPersistenceService.SetPause(false);
            _levelPersistenceService.Save();
            _levelPersistenceService.SetPause(_isManual);
        }
    }
}
