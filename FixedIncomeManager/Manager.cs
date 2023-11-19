using FixedIncomeManager.Models;
using FixedIncomeManager.Persistence;
using TaxRequester;

using Holidays = System.Collections.Generic.List<System.DateTime>;

namespace FixedIncomeManager
{
    public class Manager
    {
        private List<FixedIncomeModel> _fixedIncome = new List<FixedIncomeModel>();
        private readonly IBondsPersistency<FixedIncomeModel, RateModel> _persistency;
        public RateModel CDIRate { get; private set; } = new RateModel(RateType.CDI);
        public RateModel IPCA12Rate { get; private set; } = new RateModel(RateType.IPCA12);
        public Manager(IBondsPersistency<FixedIncomeModel, RateModel> persistency)
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
            RateModel? previousCDI, previousIPCA12;
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
        private bool IsRateChanged(out RateModel? previousCDI, out RateModel? previousIPCA)
        {
            previousCDI = null;
            previousIPCA = null;
            var rateList = _persistency.GetRates();
            if(rateList.Count == 0)
            {
                return false;
            }
            previousCDI = _persistency.GetRates().Last(rate => rate.Type == RateType.CDI);
            previousIPCA = _persistency.GetRates().Last(rate => rate.Type == RateType.IPCA12);
            return !(previousCDI.Equals(CDIRate)
                && previousIPCA.Equals(IPCA12Rate));
        }
        private void UpdateBondsProjections(
            RateModel cdi, RateModel ipca12, Holidays holidays, bool setLastUpdateDate = false)
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
    }
}
