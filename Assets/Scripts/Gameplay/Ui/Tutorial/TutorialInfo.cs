using Honeylab.Consumables;
using Honeylab.Gameplay.World;
using System;
using UnityEngine;


namespace Honeylab.Gameplay.Tutorial
{
    [Serializable]
    public class TutorialInfo
    {
        [SerializeField] private string _taskText;
        [SerializeField] private float _completeDuration = 2.0f;
        [SerializeField] private int _maxAmountToCollect;
        [SerializeField] private int _arrowPositionY = 5;
        [SerializeField] private bool _focusOnStart = true;
        [SerializeField] private bool _focusOnClick = true;
        [SerializeField] private Sprite _icon;
        [SerializeField] private bool _isClickHerePanelActive;
        [SerializeField] private int _requiredWeaponLevel = -1;
        [SerializeField] private RequiredItemType _requiredItemType;
        public string TaskText => _taskText;
        public float CompleteDuration => _completeDuration;
        public int MaxAmountToCollect => _maxAmountToCollect;
        public int CurrentAmountToCollect { get; set; }
        public int ArrowPositionY => _arrowPositionY;
        public bool FocusOnStart => _focusOnStart;
        public bool FocusOnClick => _focusOnClick;
        public Sprite Icon => _icon;
        public bool IsClickHerePanelActive => _isClickHerePanelActive;
        public int RequiredWeaponLevel => _requiredWeaponLevel;
        public RequiredItemType RequiredItemType => _requiredItemType;
        public WorldObjectFlow WorldObject { get; set; }
        public ConsumablePersistenceId ConsumableId { get; set; }

        public TutorialInfo(string taskText = null)
        {
            _taskText = taskText ?? string.Empty;
        }
    }
}
