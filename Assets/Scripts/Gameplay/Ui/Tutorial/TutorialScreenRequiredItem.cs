using Honeylab.Consumables;
using Honeylab.Gameplay.Ui;
using Honeylab.Utils.Tutorial;
using UnityEngine;
using UnityEngine.UI;

namespace Honeylab.Gameplay.Tutorial
{
    public abstract class TutorialScreenRequiredItem : MonoBehaviour
    {
        public GameObject Root;
        public Button Button;

        public abstract void Init(TutorialFlow flow,
            TutorialScreenFocus screenFocus);
        public abstract void Run(TutorialInfo tutorialInfo);
        public abstract void Hide();
        public abstract bool IsCompleted();

        protected class ItemInfo
        {
            public UIItemInfoWithCheckMark UIItem;
            public ConsumablePersistenceId Id;
            public bool Completed;
        }
    }
}
