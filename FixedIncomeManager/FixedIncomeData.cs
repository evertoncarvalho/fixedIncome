using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixedIncomeManager
{
    public enum FixedIncomeSegment
    {
        CDI,
        IPCA
    }

    public enum FixedIncomeTaxType
    {
        PRE,
        POST
    }

    public class FixedIncomeData
    {
        public string Name { get; private set; }
        public float Capital { get; private set; }
        public FixedIncomeTaxType TaxType { get; private set; }
        public FixedIncomeSegment Segment { get; private set; }
        public DateTime Hiring { get; private set; }
        public DateTime Expiration { get; private set; }

        public FixedIncomeData(
            string name,
            float capital,
            FixedIncomeTaxType taxType,
            FixedIncomeSegment segment,
            DateTime hiring,
            DateTime expiration)
        {
            Name = name;
            Capital = capital;
            TaxType = taxType;
            Segment = segment;
            Hiring = hiring;
            Expiration = expiration;
        }
    }
}
