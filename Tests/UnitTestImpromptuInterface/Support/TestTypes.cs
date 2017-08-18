using System;
using ImpromptuInterface;

namespace UnitTestImpromptuInterface
{


    public interface ITestAlias
    {
        [Alias("events_are_ridiculous")]
        event EventHandler EventsAreRidiculous;

        [Alias("手伝ってくれますか？")]
        string CanYouHelpMe(string arg);

        [Alias("★✪The Best Named Property in World!!☮")]
        string URTrippin
        {
            get;
            set;
        }
    }


}
