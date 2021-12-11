using System;
using System.Collections.Generic;

namespace FixedIncomeManager
{
    public class Manager
    {
        private List<FixedIncomeData> _fixedIncome = new List<FixedIncomeData>();

        public bool Add(
            string name,
            float capital,
            DateTime hiring,
            DateTime expiration,
            FixedIncomeSegment segment,
            FixedIncomeTaxType taxType)
        {
            return Add(new FixedIncomeData(
                            name,
                            capital,
                            taxType,
                            segment,
                            hiring,
                            expiration));
        }

        private bool Add(FixedIncomeData fixedIncomeData)
        {
            _fixedIncome.Add(fixedIncomeData);
            return true;
        }

        public List<FixedIncomeData> Get()
        {
            return _fixedIncome;
        }
    }
}
