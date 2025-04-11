using UnityEngine;
using UnityEngine.UI;


namespace Honeylab.Gameplay.Ui
{
    public class ResourcePopup : PopupBase
    {
       [field: SerializeField] 
       public Image Image { get; private set; }
    }
}
