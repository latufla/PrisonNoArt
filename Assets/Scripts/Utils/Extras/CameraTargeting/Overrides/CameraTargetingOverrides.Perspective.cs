namespace Honeylab.Utils.CameraTargeting
{
    public partial class CameraTargetingOverrides
    {
        private float? _fieldOfView;


        public CameraTargetingOverrides WithFieldOfView(float fieldOfView)
        {
            _fieldOfView = fieldOfView;
            return this;
        }


        public bool TryGetFieldOfView(out float fieldOfView)
        {
            fieldOfView = _fieldOfView.GetValueOrDefault();
            return _fieldOfView.HasValue;
        }
    }
}
