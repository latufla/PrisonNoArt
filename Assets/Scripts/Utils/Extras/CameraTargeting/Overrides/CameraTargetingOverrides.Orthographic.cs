namespace Honeylab.Utils.CameraTargeting
{
    public partial class CameraTargetingOverrides
    {
        private float? _orthoSize;


        public CameraTargetingOverrides WithOrthoSize(float size)
        {
            _orthoSize = size;
            return this;
        }


        public bool TryGetOrthoSize(out float size)
        {
            size = _orthoSize.GetValueOrDefault();
            return _orthoSize.HasValue;
        }
    }
}
