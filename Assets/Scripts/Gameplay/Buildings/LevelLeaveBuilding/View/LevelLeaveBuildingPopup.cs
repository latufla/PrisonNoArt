using Honeylab.Gameplay.Ui;
using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;


namespace Honeylab.Gameplay.Buildings
{
    public class LevelLeaveBuildingPopup : PopupBase
    {
        [SerializeField] private Button _levelLeaveButton;

        public IObservable<Unit> OnLevelLeaveButtonClickAsObservable() => _levelLeaveButton.OnClickAsObservable();
    }
}
