using System;
using DynamicData;
using DynamicData.Annotations;
using System.ComponentModel;

namespace DynamicDataEx
{
    public static class ObservableListEx
    {

        public static IObservable<IChangeSet<T>> Bind<T>([NotNull] this IObservable<IChangeSet<T>> source,
                                                         [NotNull] BindingList<T> targetList, int resetThreshold = 25)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (targetList == null) throw new ArgumentNullException(nameof(targetList));

            var adaptor = new BindingListAdaptor<T>(targetList, resetThreshold);
            return source.Adapt(adaptor);
        }

  
    }
}
