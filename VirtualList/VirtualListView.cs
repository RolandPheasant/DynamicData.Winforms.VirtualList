using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using System;

namespace VirtualList
{
    public partial class VirtualListView : Form
    {
       public VirtualListVewModel ViewModel { get; }

        public VirtualListView()
        {
            InitializeComponent();

            ViewModel = new VirtualListVewModel(SynchronizationContext.Current, new DataService());

        }
    }
}
