using System;

namespace FixedIncomeManager
{
    public enum FixedIncomeIndexer
    {
        CDI,
        IPCA
    }

    public enum FixedIncomeTaxType
    {
        PRE,
        POST
    }

    public enum FixedIncomeType
    {
        LC,
        LCA,
        LCI,
        CDB
    }

    public class FixedIncomeData
    {
        public string Name { get; private set; }
        public string Broker { get; private set; }
        public float Capital { get; private set; }
        public FixedIncomeType Type { get; private set; }
        public FixedIncomeTaxType TaxType { get; private set; }
        public FixedIncomeIndexer Segment { get; private set; }
        public DateTime Hiring { get; private set; }
        public DateTime Expiration { get; private set; }

        public FixedIncomeData(
            string name,
            string broker,
            float capital,
            FixedIncomeType type,
            FixedIncomeTaxType taxType,
            FixedIncomeIndexer segment,
            DateTime hiring,
            DateTime expiration)
        {
            Name = name;
            Broker = broker;
            Capital = capital;
            Type = type;
            TaxType = taxType;
            Segment = segment;
            Hiring = hiring;
            Expiration = expiration;
        }
    }
}
