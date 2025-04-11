using Cysharp.Threading.Tasks;
using Honeylab.Utils.Extensions;
using System;
using System.Threading;
using UniRx;


namespace Honeylab.Gameplay.Ui
{
    public class ScreenActionInfo
    {
        public string ActionName;
        public ScreenBase Screen;

        public ScreenActionInfo(string actionName, ScreenBase screen)
        {
            ActionName = actionName;
            Screen = screen;
        }
    }


    public abstract class ScreenPresenterBase<T> where T : ScreenBase
    {
        private readonly ScreenFactory _factory;

        private T _screen;
        private readonly ISubject<ScreenActionInfo> _actionSubject = new Subject<ScreenActionInfo>();
        private readonly ISubject<T> _showSubject = new Subject<T>();
        private readonly ISubject<Unit> _stopSubject = new Subject<Unit>();
        private CompositeDisposable _disposable;

        public IObservable<ScreenActionInfo> OnActionAsObservable() => _actionSubject.AsObservable();
        public IObservable<T> OnShownAsObservable() => _showSubject.AsObservable();
        public IObservable<Unit> OnStopAsObservable() => _stopSubject.AsObservable();

        public T Screen => _screen;


        public bool IsRunning => _screen != null;



        protected ScreenPresenterBase(ScreenFactory factory)
        {
            _factory = factory;
        }


        public async UniTask RunAsync(ScreenOpenType openType, CancellationToken ct)
        {
            if (IsRunning)
            {
                return;
            }

            try
            {
                _screen = _factory.Create<T>();
                _screen.Show(openType);

                OnRun(ct);

                _showSubject.OnNext(_screen);

                RunObservables();
                await RunCloseButtonAsync(ct);
            }
            catch (OperationCanceledException)
            {
                Stop();
            }
        }


        private async UniTask RunCloseButtonAsync(CancellationToken ct)
        {
            while (IsRunning)
            {
                await _screen.OnCloseButtonClickAsObservable().ToUniTask(true, cancellationToken: ct);
                _actionSubject.OnNext(new ScreenActionInfo(ScreenParameters.Close, _screen));
                Stop();
            }
        }


        private void RunObservables()
        {
            _disposable = new CompositeDisposable();

            IDisposable onAction = _screen.OnScreenActionAsObservable()
                .Subscribe(info =>
                {
                    _actionSubject.OnNext(info);
                });
            _disposable.Add(onAction);
        }


        public void Stop()
        {
            if (!IsRunning)
            {
                return;
            }

            OnStop();

            _screen.Hide();

            _factory.Destroy(_screen);

            _screen = null;

            _stopSubject.OnNext();

            _disposable?.Dispose();
            _disposable = null;
        }


        protected virtual void OnRun(CancellationToken ct) { }
        protected virtual void OnStop() { }
    }
}
