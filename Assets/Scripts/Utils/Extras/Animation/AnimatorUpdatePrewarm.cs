using UnityEngine;


namespace Honeylab.Utils.Animation
{
    [RequireComponent(typeof(Animator))]
    public class AnimatorUpdatePrewarm : MonoBehaviour
    {
        private void Awake()
        {
            GetComponent<Animator>().Update(0.0f);
        }
    }
}
