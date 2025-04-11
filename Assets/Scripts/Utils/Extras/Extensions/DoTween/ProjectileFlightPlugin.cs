using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Core.Easing;
using DG.Tweening.Core.Enums;
using DG.Tweening.Plugins.Core;
using DG.Tweening.Plugins.Options;
using Honeylab.Utils.Maths;
using UnityEngine;


namespace Honeylab.Utils.Extensions
{
    public class ProjectileFlightPlugin : ABSTweenPlugin<Vector3, Vector3, ProjectileFlightOptions>
    {
        public static readonly ProjectileFlightPlugin Instance = new ProjectileFlightPlugin();


        public override void Reset(TweenerCore<Vector3, Vector3, ProjectileFlightOptions> t) { }


        public override void SetFrom(TweenerCore<Vector3, Vector3, ProjectileFlightOptions> t, bool isRelative)
        {
            Vector3 prevEndVal = t.endValue;
            t.endValue = t.getter();
            t.startValue = isRelative ? t.endValue + prevEndVal : prevEndVal;
            t.setter(t.startValue);
        }


        public override void SetFrom(TweenerCore<Vector3, Vector3, ProjectileFlightOptions> t,
            Vector3 fromValue,
            bool setImmediately,
            bool isRelative)
        {
            t.startValue = isRelative ? t.startValue + fromValue : fromValue;
            if (setImmediately)
            {
                t.setter(fromValue);
            }
        }


        public override Vector3 ConvertToStartValue(TweenerCore<Vector3, Vector3, ProjectileFlightOptions> t,
            Vector3 value) => value;


        public override void SetRelativeEndValue(TweenerCore<Vector3, Vector3, ProjectileFlightOptions> t) =>
            t.endValue = t.startValue + t.changeValue;


        public override void SetChangeValue(TweenerCore<Vector3, Vector3, ProjectileFlightOptions> t)
        {
            t.changeValue = t.endValue - t.startValue;
        }


        public override float GetSpeedBasedDuration(ProjectileFlightOptions options,
            float unitsXSecond,
            Vector3 changeValue) => unitsXSecond;


        public override void EvaluateAndApply(ProjectileFlightOptions options,
            Tween t,
            bool isRelative,
            DOGetter<Vector3> getter,
            DOSetter<Vector3> setter,
            float elapsed,
            Vector3 startValue,
            Vector3 changeValue,
            float duration,
            bool usingInversePosition,
            UpdateNotice updateNotice)
        {
            float easeVal = EaseManager.Evaluate(t, elapsed, duration, t.easeOvershootOrAmplitude, t.easePeriod);

            ProjectileFlight flight = new ProjectileFlight(startValue,
                startValue + changeValue,
                options.VertexRaise);
            Vector3 position = flight.Evaluate(easeVal);

            setter(position);
        }
    }


    public struct ProjectileFlightOptions : IPlugOptions
    {
        internal float VertexRaise;


        public void Reset()
        {
            VertexRaise = default;
        }
    }
}
