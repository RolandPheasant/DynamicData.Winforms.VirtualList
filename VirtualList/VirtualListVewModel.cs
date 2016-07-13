using DynamicData;
using DynamicData.Controllers;
using System;
using System.ComponentModel;
using System.Threading;
using System.Reactive.Linq;
using DynamicDataEx;
using System.Reactive.Disposables;
using System.Diagnostics;
using DynamicData.Binding;
using DynamicData.Aggregation;//this using is importantant, otherwise it uses standard observable.Count()

namespace VirtualList
{
   public class VirtualListVewModel
    {
        private SynchronizationContext _bindingContext;
        private DataService _service;
        private VirtualisingController _virtualisingController;
        private FilterController<Poco> _filterController;

        public IObservable<int> Count { get; }
        public BindingList<Poco> Items { get; }

        private IDisposable _disposables;


        public VirtualListVewModel(SynchronizationContext bindingContext, DataService service)
        {
            _bindingContext = bindingContext;
            _service = service;
            _virtualisingController = new VirtualisingController(new VirtualRequest(0,10));
            _filterController = new FilterController<Poco>();
            Items = new BindingList<Poco>();

            var sharedDataSource = _service
                .DataStream
                .Do(x => Trace.WriteLine($"Service -> {x}"))
                .ToObservableChangeSet()
                .Publish();

            var binding = sharedDataSource
                          .Virtualise(_virtualisingController)
                          .ObserveOn(_bindingContext)
                          .Bind(Items)
                          .Subscribe();
            
            //the problem was because Virtualise should fire a noticiation if count changes, but it does not [BUG]
            //Therefore take the total count from the underlying data NB: Count is DD.Count() not Observable.Count()
            Count = sharedDataSource.Count().DistinctUntilChanged();

            Count.Subscribe(x => Trace.WriteLine($"Count = {x}"));

            var connection = sharedDataSource.Connect();
            _disposables = new CompositeDisposable(binding, connection);
        }



        public void Virtualise(int startIndex, int size)
        {
            _virtualisingController.Virtualise(new VirtualRequest(startIndex, size));
        }

    }
}
