using System;
using System.Collections.Generic;
using System.IO;
using TaxRequester;

namespace FixedIncomeManager.Persistence
{
    public class SpreadsheetCsvParser
        : IBondsPersistency<FixedIncomeData, CDIData>
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

        public ICollection<FixedIncomeData> GetBonds()
        {
            List<FixedIncomeData> items = new List<FixedIncomeData>(0);
            try
            {
                if (File.Exists(SourceString))
                {
                    using (StreamReader reader = new StreamReader(SourceString))
                    {
                        string line = reader.ReadLine();
                        string[] parts = line.Split(';');
                        DateTime lastBondValueUpdate = DateTime.Parse(parts[1]);
                        while ((line = reader.ReadLine()) != null)
                        {
                            parts = line.Split(';');
                            FixedIncomeData fixedIncome = new FixedIncomeData(
                                parts[6],
                                parts[17],
                                double.Parse(parts[0], System.Globalization.NumberStyles.Currency),
                                double.Parse(parts[1], System.Globalization.NumberStyles.Currency),
                                double.Parse(parts[3].Trim('%')),
                                (FixedIncomeType)Enum.Parse(typeof(FixedIncomeType), parts[18]),
                                GetFixedIncomeTaxType(parts[19]),
                                (FixedIncomeIndexer)Enum.Parse(typeof(FixedIncomeIndexer), parts[24]),
                                DateTime.Parse(parts[2]),
                                DateTime.Parse(parts[16]));
                            fixedIncome.LastBondValueUpdate = lastBondValueUpdate;
                            items.Add(fixedIncome);
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

        public bool SaveBonds(ICollection<FixedIncomeData> items)
        {
            return true;
        }
        
        public bool SaveRates(ICollection<CDIData> rates)
        {
            return false;
        }

        public ICollection<CDIData> GetRates()
        {
            return null;
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
