namespace Test.Integration.TS
{
    public class TimeseriesTtbTests : TimeseriesTests
    {
        /*
         * TODO NB: timeseries does not work with security yet
         */
        public TimeseriesTtbTests()
            : base(useTtb: true, auth: false)
        {
        }
    }
}
