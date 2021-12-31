using System;
using System.Collections.Generic;
using System.IO;

namespace FixedIncomeManager.Persistence
{
    public class SpreadsheetCsvParser
        : IPersistencyController<FixedIncomeData>
    {
        private string _sourceString = null;
        public string SourceString
        {
            get
            {
                return _sourceString;
            }
        }

        public SpreadsheetCsvParser(string sourceString)
        {
            _sourceString = string.IsNullOrWhiteSpace(sourceString)
                ? "fixedIncomeSample.csv"
                : sourceString;
        }

        public ICollection<FixedIncomeData> Get()
        {
            List<FixedIncomeData> items = new List<FixedIncomeData>(0);
            try
            {
                if (File.Exists(SourceString))
                {
                    using (StreamReader reader = new StreamReader(SourceString))
                    {
                        reader.ReadLine(); //skip header
                        string line = null;
                        while ((line = reader.ReadLine()) != null)
                        {
                            string[] parts = line.Split(';');
                            items.Add(new FixedIncomeData(
                                parts[6],
                                parts[17],
                                float.Parse(parts[0], System.Globalization.NumberStyles.Currency),
                                float.Parse(parts[1], System.Globalization.NumberStyles.Currency),
                                float.Parse(parts[3].Trim('%')),
                                (FixedIncomeType)Enum.Parse(typeof(FixedIncomeType), parts[18]),
                                GetFixedIncomeTaxType(parts[19]),
                                (FixedIncomeIndexer)Enum.Parse(typeof(FixedIncomeIndexer), parts[24]),
                                DateTime.Parse(parts[2]),
                                DateTime.Parse(parts[16])));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //TODO log
                Console.WriteLine(ex.Message);
            }
            return items;
        }

        public bool Save(ICollection<FixedIncomeData> items)
        {
            return true;
        }

        private FixedIncomeTaxType GetFixedIncomeTaxType(string taxType)
        {
            return taxType.Equals("Pós")
                || taxType.Equals("P�s")
                ? FixedIncomeTaxType.POST
                : FixedIncomeTaxType.PRE;
        }
    }
}
