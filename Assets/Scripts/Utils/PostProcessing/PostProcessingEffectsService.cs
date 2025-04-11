using System;


namespace Honeylab.Utils
{
    [Serializable]
    public class PostProcessingEffectsServiceArgs
    {
        public DamagePostProcessingEffect DamageEffect;
    }


    public class PostProcessingEffectsService : IDisposable
    {
        private readonly PostProcessingEffectsServiceArgs _args;


        public PostProcessingEffectsService(PostProcessingEffectsServiceArgs args)
        {
            _args = args;
        }


        public void Dispose() { }


        public void PlayDamage()
        {
            if (_args.DamageEffect != null)
            {
                _args.DamageEffect.Play();
            }
        }
    }
}
