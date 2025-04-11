using System;
using UnityEngine;


namespace Honeylab.Utils
{
    public class TimeService
    {
        public TimeService() { }

        public double GetUtcTime() => DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        public float GetRealtime() => Time.realtimeSinceStartup;
        public float GetDeltaRealtime(float time) => (GetRealtime() - time) * Time.timeScale;

        public void Pause() => Time.timeScale = 0;
        public void Resume() => Time.timeScale = 1;

        public float GetDeltaTime() => Time.deltaTime * Time.timeScale;
    }
}
