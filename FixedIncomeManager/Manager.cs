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
            DateTime hiring,
            DateTime expiration,
            FixedIncomeType type,
            FixedIncomeIndexer segment,
            FixedIncomeTaxType taxType)
        {
            return Add(new FixedIncomeData(
                            name,
                            broker,
                            capital,
                            type,
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

        public bool LoadFromCsv(string path)
        {
            try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    reader.ReadLine(); //skip header
                    string line = null;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] parts = line.Split(';');
                        Add(
                            parts[6],
                            parts[17],
                            float.Parse(parts[0], System.Globalization.NumberStyles.Currency),
                            DateTime.Parse(parts[2]),
                            DateTime.Parse(parts[16]),
                            (FixedIncomeType)Enum.Parse(typeof(FixedIncomeType), parts[18]),
                            (FixedIncomeIndexer)Enum.Parse(typeof(FixedIncomeIndexer), parts[24]),
                            GetFixedIncomeTaxType(parts[19]));
                    }
                }
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        private FixedIncomeTaxType GetFixedIncomeTaxType(string taxType)
        {
            return taxType.Equals("Pós")
                ? FixedIncomeTaxType.POST
                : FixedIncomeTaxType.PRE;
        }
    }
}
