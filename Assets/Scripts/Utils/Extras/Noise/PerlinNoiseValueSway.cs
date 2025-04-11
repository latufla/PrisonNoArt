using System;
using UnityEngine;


namespace Honeylab.Utils.Noise
{
    public class PerlinNoiseValueSway
    {
        private readonly Settings _settings;
        private readonly float _seed;
        private float _walkValue;


        public PerlinNoiseValueSway(Settings settings, float seed)
        {
            _settings = settings;
            _seed = seed;
        }


        public void Update(float deltaTime) => _walkValue += _settings.WalkSpeed * deltaTime;


        public float Evaluate()
        {
            // Clamping due to note for return value at https://docs.unity3d.com/ScriptReference/Mathf.PerlinNoise.html
            float rawNoise = Mathf.Clamp01(Mathf.PerlinNoise(_walkValue, _seed));
            float normalizedSway = (rawNoise - 0.5f) * 2.0f;
            float poweredSway = _settings.ValuePower * normalizedSway;
            return poweredSway;
        }


        [Serializable]
        public class Settings
        {
            [SerializeField] private float _valuePower;
            [SerializeField] private float _walkSpeed;


            public float ValuePower => _valuePower;
            public float WalkSpeed => _walkSpeed;


            // For serialization support
            public Settings() { }


            public Settings(float valuePower, float walkSpeed)
            {
                _valuePower = valuePower;
                _walkSpeed = walkSpeed;
            }
        }
    }
}
