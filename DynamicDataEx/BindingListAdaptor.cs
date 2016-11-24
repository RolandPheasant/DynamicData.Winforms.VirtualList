using DynamicData;
using DynamicData.Internal;
using DynamicData.Annotations;
using System;
using System.ComponentModel;


namespace DynamicDataEx
{
    public class BindingListAdaptor<T> : IChangeSetAdaptor<T>
    {

        private readonly BindingList<T> _list;
        private readonly int _refreshThreshold;
        private bool _loaded;

        public BindingListAdaptor([NotNull] BindingList<T> list, int refreshThreshold = 25)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            _list = list;
            _refreshThreshold = refreshThreshold;
        }

        public void Adapt(IChangeSet<T> changes)
        {
            if (changes.Count > _refreshThreshold || !_loaded)
            {

                _list.RaiseListChangedEvents = false;
                _list.Clone(changes);
                _loaded = true;

                _list.RaiseListChangedEvents = true;
                _list.ResetBindings();
            }
            else
            {
                _list.Clone(changes);
            }
        }
    }



}