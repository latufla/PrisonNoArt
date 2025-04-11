using System;
using System.Collections.Generic;
using UniRx;


namespace Honeylab.Utils.Arrows
{
    public class ArrowsService
    {
        public IObservable<ArrowView> OnArrowViewRegisteredAsObservable() =>
            _arrowViewRegisteredSubject.AsObservable();
        public IObservable<ArrowView> OnArrowViewUnregisteredAsObservable() =>
            _arrowViewUnregisteredSubject.AsObservable();


        public IReadOnlyList<ArrowView> RegisteredArrowViews => _registeredArrowViews;

        private readonly Subject<ArrowView> _arrowViewRegisteredSubject = new Subject<ArrowView>();
        private readonly Subject<ArrowView> _arrowViewUnregisteredSubject = new Subject<ArrowView>();
        private readonly List<ArrowView> _registeredArrowViews = new List<ArrowView>();

        public void RegisterArrowView(ArrowView arrowView)
        {
            _registeredArrowViews.Add(arrowView);
            _arrowViewRegisteredSubject.OnNext(arrowView);
        }


        public void UnregisterArrowView(ArrowView arrowView)
        {
            _registeredArrowViews.Remove(arrowView);
            _arrowViewUnregisteredSubject.OnNext(arrowView);
        }
    }
}
