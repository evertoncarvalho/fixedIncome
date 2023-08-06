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
                            items.Add(GetFixedIncomeDataFromCSV(
                                line,
                                lastBondValueUpdate));
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

        protected FixedIncomeTaxType GetFixedIncomeTaxType(string taxType)
        {
            return taxType.Equals("Pós")
                || taxType.Equals("P�s")
                ? FixedIncomeTaxType.POST
                : FixedIncomeTaxType.PRE;
        }

        protected FixedIncomeData GetFixedIncomeDataFromCSV(
            string csv,
            DateTime? lastBondValueUpdate = null)
        {
            char separator = ';';
            string[] parts = null;
            if (string.IsNullOrEmpty(csv)
                || csv.IndexOf(separator) == 0)
            {
                throw new ArgumentException("invalid csv");
            }
            parts = csv.Split(';');
            FixedIncomeData fixedIncome = new FixedIncomeData(
                parts[7],
                parts[19],
                double.Parse(parts[0], System.Globalization.NumberStyles.Currency),
                double.Parse(parts[1], System.Globalization.NumberStyles.Currency),
                double.Parse(parts[3].Trim('%')),
                (FixedIncomeType)Enum.Parse(typeof(FixedIncomeType), parts[20]),
                GetFixedIncomeTaxType(parts[21]),
                (FixedIncomeIndexer)Enum.Parse(typeof(FixedIncomeIndexer), parts[4]),
                DateTime.Parse(parts[2]),
                DateTime.Parse(parts[18]));
            if (lastBondValueUpdate != null)
            {
                fixedIncome.LastBondValueUpdate = lastBondValueUpdate.Value;
            }
            return fixedIncome;
        }
    }
}
