using System;
using System.Collections.Generic;
using System.IO;

namespace FixedIncomeManager
{
    public class Manager
    {
        private List<FixedIncomeData> _fixedIncome = new List<FixedIncomeData>();

        public bool Add(
            string name,
            string broker,
            float capital,
            FixedIncomeType type,
            FixedIncomeTaxType taxType,
            FixedIncomeIndexer indexer,
            DateTime hiring,
            DateTime expiration)
        {
            return Add(new FixedIncomeData(
                            name,
                            broker,
                            capital,
                            type,
                            taxType,
                            indexer,
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
