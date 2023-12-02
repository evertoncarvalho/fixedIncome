﻿using FixedIncomeManager.Models;
using FixedIncomeManager.Persistence;
using TaxRequester;

using Holidays = System.Collections.Generic.List<System.DateTime>;

namespace FixedIncomeManager
{
    public class Manager
    {
        private HashSet<DateTime> _holidays;
        private List<FixedIncomeModel> _fixedIncome = new();
        private readonly IBondsPersistency<FixedIncomeModel, IndexerModel> _persistency;
        public IndexerModel CDIRate { get; private set; } = new IndexerModel(IndexerType.CDI);
        public IndexerModel IPCA12Rate { get; private set; } = new IndexerModel(IndexerType.IPCA12);
        private HashSet<DateTime> Holidays
        {
            get
            {
                if(_holidays == null)
                {
                    _holidays = new (GetHolidays());
                }
                return _holidays;
            }
        }
        public Manager(IBondsPersistency<FixedIncomeModel, IndexerModel> persistency)
        {
            _persistency = persistency;
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
            return Add(new FixedIncomeModel(
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

        private bool Add(FixedIncomeModel fixedIncomeData)
        {
            _fixedIncome.Add(fixedIncomeData);
            return true;
        }

        public List<FixedIncomeModel> Get()
        {
            return _fixedIncome;
        }

        public FixedIncomeSummaryData GetSummary()
        {
            FixedIncomeSummaryData data = new FixedIncomeSummaryData();
            foreach (FixedIncomeModel item in _fixedIncome)
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
            Holidays holidays = GetHolidays();
            GetCurrentRates();
            IndexerModel? previousCDI, previousIPCA12;
            if(IsRateChanged(out previousCDI, out previousIPCA12))
            {
                UpdateBondsProjections(
                    previousCDI,
                    previousIPCA12,
                    holidays,
                    true);
            }
            UpdateBondsProjections(
                CDIRate,
                IPCA12Rate,
                holidays);
        }
        /// <summary>
        /// Request and set the current CDI and IPCA12 rates.
        /// </summary>
        private void GetCurrentRates()
        {
            Requester requester = new Requester();
            var cdi = requester.GetCDI();
            if(cdi != null )
            {
                CDIRate.UpdateRate(cdi.Rate, cdi.RateDate);
            }
            var ipca = requester.GetIPCALast12Months();
            if(ipca != null )
            {
                IPCA12Rate.UpdateRate(ipca.Rate, ipca.RateDate);
            }
        }
        private bool IsRateChanged(out IndexerModel? previousCDI, out IndexerModel? previousIPCA)
        {
            previousCDI = null;
            previousIPCA = null;
            var rateList = _persistency.GetRates();
            if(rateList.Count == 0)
            {
                return false;
            }
            previousCDI = _persistency.GetRates().Last(rate => rate.Type == IndexerType.CDI);
            previousIPCA = _persistency.GetRates().Last(rate => rate.Type == IndexerType.IPCA12);
            return !(previousCDI.Equals(CDIRate)
                && previousIPCA.Equals(IPCA12Rate));
        }
        private void UpdateBondsProjections(
            IndexerModel cdi, IndexerModel ipca12, Holidays holidays, bool setLastUpdateDate = false)
        {
            foreach(FixedIncomeModel item in _fixedIncome)
            {
                item.UpdateBondsValueProjections(
                    item.Indexer == FixedIncomeIndexer.IPCA
                        ? ipca12
                        : cdi,
                    holidays,
                    setLastUpdateDate);
            }
        }
        protected Holidays GetHolidays(string path = "holidays.csv")
        {
            Holidays holidays = new Holidays(0);
            try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    reader.ReadLine(); //header
                    string? line = null;
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
        public ICollection<DateTime> GetInputDates(int inputYear, int maturityYear)
        {
            int netDays = (maturityYear - inputYear) * -365;
            List<DateTime> inputDates = new List<DateTime>();
            foreach(var matutiry in GetMaturities(maturityYear))
            {
                inputDates.Add(
                    GetNearWorkingDay(
                        Holidays,
                        matutiry.AddDays(netDays),
                        x => -1)); // always moves the days backwards
            }
            return inputDates;
        }
        public ICollection<DateTime> GetMaturities(int year)
        {
            DateTime maturity = new DateTime(year, 1, 1);
            List<DateTime> maturities = new (1);
            int auxMultiplier = 1;
            while (maturity.Year == year)
            {
                maturities.Add(
                    GetNearWorkingDay(
                        Holidays,
                        maturity,
                        x => x == 1 ? 1 : -1)); // if it is at the beginning of the month, the days move forward,
                                                // otherwise, they move backwards

                if (maturity.Day == 1)
                {
                    auxMultiplier = 1;
                }
                else
                {
                    auxMultiplier = -1;
                    maturity = maturity.AddMonths(1);
                }
                maturity = maturity.AddDays(14 * auxMultiplier);
            }
                        
            return maturities;
        }
        private DateTime GetNearWorkingDay(HashSet<DateTime> holidays, DateTime date, Func<int, int> incrementRule)
        {
            int increment = incrementRule(date.Day);
            while(!IsWorkingDay(holidays, date))
            {
                date = date.AddDays(increment);
            }
            return date;
        }
        private bool IsWorkingDay(HashSet<DateTime> holidays, DateTime date)
        {
            return !holidays.Contains(date)
                && date.DayOfWeek != DayOfWeek.Saturday
                && date.DayOfWeek != DayOfWeek.Sunday;
        }
    }
}
