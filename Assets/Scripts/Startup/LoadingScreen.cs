using Honeylab.MoneyGarden.Ui;
using UnityEngine;


namespace Honeylab.Startup
{
    public class LoadingScreen : MonoBehaviour
    {
        [SerializeField] private LoadingSpinnerView _spinner;


        public void Start()
        {
            DontDestroyOnLoad(this);

            _spinner.SetTweening(true);
        }
    }
}
