using DG.Tweening;
using Honeylab.Consumables;
using System;
using UnityEngine;


namespace Honeylab.Utils
{
    public static class ItemTweenUtil
    {
        public static Tween ItemFlight(Transform itemTransform,
            Vector3 startPosition,
            Quaternion startRotation,
            Vector3 endPosition,
            Quaternion targetRotation,
            Vector3 endScale,
            float productFlightDuration,
            float productFlightHeight,
            Ease ease = Ease.Linear,
            Action onKill = null)
        {
            itemTransform.position = startPosition;
            itemTransform.rotation = startRotation;

            return DOTween.Sequence()
                .SetLink(itemTransform.gameObject)
                .Append(DOTween.To(value =>
                            itemTransform.position = Parabola(startPosition, endPosition, productFlightHeight, value),
                        0,
                        1,
                        productFlightDuration)
                    .SetEase(ease))
                .Insert(0, itemTransform.DORotateQuaternion(targetRotation, productFlightDuration))
                .Insert(0, itemTransform.DOScale(endScale, productFlightDuration))
                .OnKill(() =>
                {
                    itemTransform.localScale = Vector3.one;

                    onKill?.Invoke();
                });
        }


        public static Tween ItemFlight(ConsumablePersistenceId id,
            ConsumablesPool pool,
            Vector3 startPosition,
            Quaternion startRotation,
            Vector3 endPosition,
            Quaternion targetRotation,
            float productFlightDuration,
            float productFlightHeight,
            Ease ease = Ease.Linear,
            Action onKill = null)
        {
            GameObject productGo = pool.Pop(id);
            Transform productTransform = productGo.transform;
            productTransform.position = startPosition;
            productTransform.rotation = startRotation;

            return DOTween.Sequence()
                .SetLink(productGo)
                .Append(DOTween.To(value =>
                            productTransform.position =
                                Parabola(startPosition, endPosition, productFlightHeight, value),
                        0,
                        1,
                        productFlightDuration)
                    .SetEase(ease))
                .Insert(0, productTransform.DORotateQuaternion(targetRotation, productFlightDuration))
                .OnKill(() =>
                {
                    productTransform.localScale = Vector3.one;
                    pool.Push(id, productGo);

                    onKill?.Invoke();
                });
        }


        public static Tween ItemMove(ConsumablePersistenceId id,
            ConsumablesPool pool,
            Vector3 startPosition,
            Quaternion startRotation,
            Vector3 endPosition,
            Quaternion targetRotation,
            float productFlightDuration,
            Ease ease,
            Action onKill = null)
        {
            GameObject productGo = pool.Pop(id);
            Transform productTransform = productGo.transform;
            productTransform.position = startPosition;
            productTransform.rotation = startRotation;

            return DOTween.Sequence()
                .SetLink(productGo)
                .Append(productTransform.DOMove(endPosition, productFlightDuration).SetEase(ease))
                .Insert(0, productTransform.DORotateQuaternion(targetRotation, productFlightDuration))
                .SetEase(ease)
                .OnKill(() =>
                {
                    productTransform.localScale = Vector3.one;
                    pool.Push(id, productGo);

                    onKill?.Invoke();
                });
        }


        public static Tween ItemFlightLocal(ConsumablePersistenceId id,
            ConsumablesPool pool,
            Transform parent,
            Vector3 startPosition,
            Quaternion startRotation,
            Vector3 endPosition,
            float productFlightDuration,
            float productFlightHeight,
            bool scaleDownInTheEnd = false,
            Action onKill = null)
        {
            GameObject productGo = pool.Pop(id);
            Transform productTransform = productGo.transform;
            productTransform.parent = parent;
            productTransform.position = startPosition;
            productTransform.rotation = startRotation;

            Vector3 localStartPosition = productTransform.localPosition;
            Vector3 localEndPosition = parent.InverseTransformPoint(endPosition);
            Sequence sequence = DOTween.Sequence()
                .SetLink(productGo)
                .Append(DOTween.To(value =>
                    {
                        productTransform.localPosition = Parabola(localStartPosition,
                            localEndPosition,
                            productFlightHeight,
                            value);

                        if (scaleDownInTheEnd && value > 0.75f)
                        {
                            productTransform.localScale = Vector3.one * (1.0f - value);
                        }
                    },
                    0,
                    1,
                    productFlightDuration));

            sequence.OnKill(() =>
            {
                productTransform.localScale = Vector3.one;
                pool.Push(id, productGo);

                onKill?.Invoke();
            });

            return sequence;
        }


        public static Vector3 Parabola(Vector3 start, Vector3 end, float height, float t)
        {
            Vector3 mid = Vector3.Lerp(start, end, t);
            return new Vector3(mid.x, ParabolaFunc(t, height) + mid.y, mid.z);
        }


        private static float ParabolaFunc(float t, float height) => 4 * (-height * t * t + height * t);
    }
}
