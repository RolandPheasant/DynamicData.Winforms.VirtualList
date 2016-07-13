using System;
using System.Reactive.Linq;

namespace VirtualList
{
   public class DataService
    {

        public IObservable<Poco> DataStream { get; }

        public  DataService()
        {

            DataStream = Observable.Create<Poco>(o => 
                         {
                            var rnd = new Random();
                            var source = Observable.Interval(TimeSpan.FromMilliseconds(100))
                                            .Select((x,i) => new Poco(i,
                                                                      rnd.Next(0, 100),
                                                                      rnd.Next(0, 20),
                                                                      rnd.Next(0, 8)))
                                            .Take(1000);
                            return source.Subscribe(o);
                         });
        }


    }
}
