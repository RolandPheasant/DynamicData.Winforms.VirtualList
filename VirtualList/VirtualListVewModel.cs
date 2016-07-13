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


            /// this does not update Count, if no virtualisation request is issued
            var sharedDataSource = _service
                                   .DataStream
                                   .Do(x => Trace.WriteLine($"Service -> {x}"))
                                   .ToObservableChangeSet()
                                   .Virtualise(_virtualisingController)
                                   .Publish();

            var binding = sharedDataSource
                          .ObserveOn(_bindingContext)
                          .Bind(Items)
                          .Subscribe();


            /// this works, but items are reversed in the BindingList
            //var sharedDataSource = _service
            //                       .DataStream
            //                       .Do(x => Trace.WriteLine($"Service -> {x}"))
            //                       .ToObservableChangeSet()
            //                       .Reverse()
            //                       .Virtualise(_virtualisingController)
            //                       .Publish();

            //var binding = sharedDataSource
            //              .ObserveOn(_bindingContext)
            //              .Bind(Items)
            //              .Subscribe();


            /// this works, but requires  transitioning from keyed Changeset then back to un-keyed ChangeSet
            /// it also requires to sort before virtualise
            //var sharedDataSource = _service
            //                       .DataStream
            //                       .Do(x => Trace.WriteLine($"Service -> {x}"))
            //                       .ToObservableChangeSet()
            //                       .AddKey(x => x.Id)
            //                       .Sort(SortExpressionComparer<Poco>.Ascending(x => x.Id))
            //                       .Virtualise(_virtualisingController)
            //                       .Publish();

            //var binding = sharedDataSource
            //              .RemoveKey()
            //              .ObserveOn(_bindingContext)
            //              .Bind(Items)
            //              .Subscribe();


            Count = sharedDataSource.Select(x => {
                                            return x.Response.TotalSize; });

            Count.Subscribe(x => Trace.WriteLine($"Count = {x}"));

            var connection = sharedDataSource.Connect();
            _disposables = new CompositeDisposable(binding, connection);

            //////////
    
        }



        public void Virtualise(int startIndex, int size)
        {
            _virtualisingController.Virtualise(new VirtualRequest(startIndex, size));
        }

    }
}
