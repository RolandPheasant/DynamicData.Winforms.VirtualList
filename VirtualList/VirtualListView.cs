using System.Threading;
using System.Windows.Forms;
using System;
using System.Reactive.Linq;


namespace VirtualList
{
    public partial class VirtualListView : Form
    {
       public VirtualListVewModel ViewModel { get; }

        public VirtualListView()
        {
            InitializeComponent();

            ViewModel = new VirtualListVewModel(SynchronizationContext.Current, new DataService());

            var listMaxItems = 30;
            listBox1.DataSource = ViewModel.Items;

            ViewModel.Count
            .Sample(TimeSpan.FromMilliseconds(100))                               // every tenth of a second
            .DistinctUntilChanged()                                               // if count has changed
            .Select(x=> Math.Max(0, x- listMaxItems))
            .ObserveOn(SynchronizationContext.Current)
            .Subscribe(x => {
                            vScrollBar1.Maximum = x;                              // update the scrollbar
                            //  vScrollBar1.Value = vScrollBar1.Maximum;            // auto-scroll the list
            });                                                   

            Observable.FromEventPattern<EventHandler, EventArgs>(h => vScrollBar1.ValueChanged += h,
                                                                h => vScrollBar1.ValueChanged -= h)
            .Select(_ => vScrollBar1.Value)
            .StartWith(0)
            .Subscribe(x => ViewModel.Virtualise(x, listMaxItems));

        }
    }
}
