using System;

namespace TaxRequester
{
    public abstract class BaseRateData
    {
        public abstract double Rate { get; set; }

        public abstract DateTime RateDate { get; set; }

        public abstract double GetRateDaily(double inputRate = 0f);
    }
}
