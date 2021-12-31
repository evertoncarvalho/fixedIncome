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
        private IPersistencyController<FixedIncomeData> _persistency = new JsonPersistencyController();
        
        public Manager()
        {
            _fixedIncome.AddRange(_persistency.Get());
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

        public void Save()
        {
            _persistency.Save(_fixedIncome);
        }

        protected void Initialize()
        {
            Requester requester = new Requester();
            CDIData cdi = requester.GetCDI();
            IPCAData ipca = requester.GetIPCALast12Months();
            List<DateTime> holidays = GetHolidays();
            foreach (FixedIncomeData item in _fixedIncome)
            {
                item.UpdateBondsValueProjections(
                    item.Indexer == FixedIncomeIndexer.CDI
                    || item.TaxType == FixedIncomeTaxType.PRE
                        ? cdi
                        : ipca,
                    holidays);
            }
        }

        public List<DateTime> GetHolidays(string path = "holidays.csv")
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
