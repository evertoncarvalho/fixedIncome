using Newtonsoft.Json;
using System;

namespace TaxRequester
{
    //{ "taxa":"7,65","dataTaxa":"04/11/2021","indice":"33.284,31","dataIndice":"05/11/2021"}
    public class CDIData
        : BaseTaxData
    {
        [JsonIgnore]
        public virtual string AsString
        {
            get
            {
                return "cdi " + Tax
                    + " cdi date " + TaxDate
                    + " index " + Index
                    + " index date" + IndexDate;
            }
        }

        [JsonProperty("taxa")]
        [JsonConverter(typeof(TaxConverter))]
        public override double Tax { get; set; } = 0f;

        [JsonProperty("indice")]
        [JsonConverter(typeof(TaxConverter))]
        public double Index { get; set; } = 0f;

        [JsonProperty("dataTaxa")]
        [JsonConverter(typeof(TaxDateConverter))]
        public DateTime TaxDate { get; set; } = DateTime.MinValue;

        [JsonProperty("dataIndice")]
        [JsonConverter(typeof(TaxDateConverter))]
        public DateTime IndexDate { get; set; } = DateTime.MinValue;
    }
}
