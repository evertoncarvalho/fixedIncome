using FixedIncomeManager.Persistence;
using System;
using System.Collections.Generic;
using System.IO;
using TaxRequester;

namespace FixedIncomeManager
{
    public class Manager
    {
        private List<FixedIncomeData> _fixedIncome = new List<FixedIncomeData>();
        private IBondsPersistency<FixedIncomeData, CDIData> _persistency = new JsonPersistencyController();

        public CDIData CDIData { get; private set; } = null;

        public IPCAData IPCAData { get; private set; } = null;

        public Manager()
        {
            _fixedIncome.AddRange(_persistency.GetBonds());
            Initialize();
        }

        public bool Add(
            string name,
            string broker,
            double capital,
            double remuneration,
            double lastBondValue,
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
                            lastBondValue,
                            remuneration,
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

        public FixedIncomeSummaryData GetSummary()
        {
            FixedIncomeSummaryData data = new FixedIncomeSummaryData();
            foreach (FixedIncomeData item in _fixedIncome)
            {
                data.Capital += item.Capital;
                data.CurrentValue += item.CurrentBondValue;
                data.ValueAtExpiration += item.BondValueAtExpiration;
            }
            return data;
        }

        public void Save()
        {
            _persistency.SaveBonds(_fixedIncome);
        }

        protected void Initialize()
        {
            Requester requester = new Requester();
            CDIData = requester.GetCDI();
            IPCAData = requester.GetIPCALast12Months();
            List<DateTime> holidays = GetHolidays();
            foreach (FixedIncomeData item in _fixedIncome)
            {
                item.UpdateBondsValueProjections(
                    item.Indexer == FixedIncomeIndexer.CDI
                    || item.TaxType == FixedIncomeTaxType.PRE
                        ? CDIData
                        : IPCAData,
                    holidays);
            }
        }

        protected List<DateTime> GetHolidays(string path = "holidays.csv")
        {
            List<DateTime> holidays = new List<DateTime>(0);
            try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    reader.ReadLine(); //header
                    string line = null;
                    while ((line = reader.ReadLine()) != null)
                    {
                        holidays.Add(DateTime.Parse(line.Substring(0, line.IndexOf(';'))));
                    }
                }
            }
            catch (Exception ex)
            {
                //TODO log
                Console.WriteLine(ex.Message);
            }
            return holidays;
        }
    }
}
