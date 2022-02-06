using Newtonsoft.Json;
using System;

namespace TaxRequester
{
    //{ "taxa":"7,65","dataTaxa":"04/11/2021","indice":"33.284,31","dataIndice":"05/11/2021"}
    public class CDIData
        : BaseRateData
    {
        [JsonIgnore]
        public virtual string AsString
        {
            get
            {
                return "cdi " + Rate
                    + " cdi date " + RateDate
                    + " index " + Index
                    + " index date" + IndexDate;
            }
        }

        [JsonProperty("taxa")]
        [JsonConverter(typeof(RateConverter))]
        public override double Rate { get; set; } = 0f;

        [JsonProperty("dataTaxa")]
        [JsonConverter(typeof(RateDateConverter))]
        public override DateTime RateDate { get; set; } = DateTime.MinValue;

        [JsonProperty("indice")]
        [JsonConverter(typeof(RateConverter))]
        public double Index { get; set; } = 0f;

        [JsonProperty("dataIndice")]
        [JsonConverter(typeof(RateDateConverter))]
        public DateTime IndexDate { get; set; } = DateTime.MinValue;

        public override double GetRateDaily(double inputRate = 0)
        {
            if (inputRate == 0f)
            {
                inputRate = Rate;
            }
            return Math.Pow(1 + inputRate / 100, 1f / 252) - 1;
        }
    }
}
