using Honeylab.Gameplay.World;
using System.Collections.Generic;
using UniRx;
using UnityEngine;


namespace Honeylab.Gameplay.Ui
{
    public class Billboard : WorldObjectComponentBase
    {
        [SerializeField] private Transform[] _roots;

        private BillboardPresenterFactory _factory;
        private readonly List<BillboardPresenter> _billboards = new();


        protected override void OnInit()
        {
            WorldObjectFlow flow = GetFlow();
            _factory = flow.Resolve<BillboardPresenterFactory>();

            foreach (Transform root in _roots)
            {
                BillboardPresenter billboard = _factory.Create(root).AddTo(this);
                billboard.Run();

                _billboards.Add(billboard);
            }
        }

        protected override void OnClear()
        {
            foreach (BillboardPresenter presenter in _billboards)
            {
                presenter.Dispose();
            }

            _billboards.Clear();
        }
    }
}
