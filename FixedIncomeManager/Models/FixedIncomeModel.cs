namespace FixedIncomeManager.Models
{
    public enum FixedIncomeIndexer
        : short
    {
        // Certificado de Depósito Interbancário
        CDI,
        CDI_PLUS, // (CDI+) same as above but it contains a fixed rate part
        // Índice nacional de Preços ao Consumidor Amplo (12 meses)
        IPCA
    }

    public enum FixedIncomeTaxType
        : short
    {
        PRE,
        POST
    }

    public enum FixedIncomeType
        : short
    {
        // Letra de Câmbio
        LC,
        // Letra de Crédito de Agronegócio
        LCA,
        // Letra de Crédito Imobiliário
        LCI,
        // Certificado de Depósito Bancário
        CDB,
        // Certificado de Recebíveis de Agronegócio (Não garantido pelo FGC - Fundo Garantidor de Crédito)
        CRA,
        // Certificado de Recebívies Imobiliário (Não garantido pelo FGC - Fundo Garantidor de Crédito) 
        CRI
    }

    public class FixedIncomeModel
    {
        /// <summary>
        /// Used for future values projections
        /// </summary>
        private readonly FixedIncomeCalculator _calculator;

        /// <summary>
        /// Name of the company
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Name of the broker
        /// </summary>
        public string Broker { get; private set; }
        /// <summary>
        /// The value pay for the bonds
        /// </summary>
        public double Capital { get; private set; }
        /// <summary>
        /// Tax remuneration
        /// </summary>
        public double Remuneration { get; private set; }
        /// <summary>
        /// Flavor of the fixed income.
        /// </summary>
        public FixedIncomeType Type { get; protected set; }
        /// <summary>
        /// Indicates when the remuneration can float.
        /// </summary>
        public FixedIncomeTaxType TaxType { get; private set; }
        /// <summary>
        /// Rate used for bonds remuneration
        /// </summary>
        public FixedIncomeIndexer Indexer { get; private set; }
        /// <summary>
        /// Hiring date
        /// </summary>
        public DateTime Hiring { get; protected set; }
        /// <summary>
        /// Expiration Data
        /// </summary>
        public DateTime Expiration { get; protected set; }
        /// <summary>
        /// Update date of bond value. It changes when the remuneration tax change.
        /// </summary>
        public DateTime LastBondValueUpdate { get; set; }
        /// <summary>
        /// Bond value at registration or before rate update
        /// </summary>
        public double LastBondValue { get; private set; }
        /// <summary>
        /// Current bond value projection
        /// </summary>
        public double CurrentBondValue { get; set; }
        /// <summary>
        /// Bond value projection at the expiration
        /// </summary>
        public double BondValueAtExpiration { get; set; }
        /// <summary>
        /// Bond value projection at the expiration without fees
        /// </summary>
        public double NetBondValueAtExpiration { get; set; }
        /// <summary>
        /// Net Profit at bonds the expiration
        /// </summary>
        public double Profit
        {
            get
            {
                return NetBondValueAtExpiration - Capital;
            }
        }
        /// <summary>
        /// Brazilian taxation for fixed income
        /// </summary>
        public virtual double Taxation
        {
            get
            {
                if (Type == FixedIncomeType.LCA
                    || Type == FixedIncomeType.LCI)
                {
                    return 0f;
                }

                int daysUntilDeadline = (Expiration - Hiring).Days;
                if (daysUntilDeadline < 180)
                {
                    return 0.225f;
                }
                if (daysUntilDeadline < 360)
                {
                    return 0.20f;
                }
                if (daysUntilDeadline < 720)
                {
                    return 0.175f;
                }
                return 0.15f;
            }
        }

        public FixedIncomeModel(
            string name,
            string broker,
            double capital,
            double lastBondValue,
            double remuneration,
            FixedIncomeType type,
            FixedIncomeTaxType taxType,
            FixedIncomeIndexer indexer,
            DateTime hiring,
            DateTime expiration)
        {
            Name = name;
            Broker = broker;
            Capital = capital;
            LastBondValue = lastBondValue;
            Remuneration = remuneration;
            Type = type;
            TaxType = taxType;
            Indexer = indexer;
            Hiring = hiring;
            Expiration = expiration;
            _calculator = indexer == FixedIncomeIndexer.CDI
                ? new ProportionalCalculator()
                : new FixedRateCalculator();
        }

        public double GetDailyPostTaxRemuneration(double indexerValue)
        {
            return _calculator.GetDailyRate(Remuneration, indexerValue);
        }

        public double GetDailyPreTaxRemuneration()
        {
            return _calculator.GetDailyRate(Remuneration, 1d);
        }

        public void UpdateBondsValueProjections(
            RateModel rateData,
            List<DateTime> holidays,
            bool setLastUpdateDate)
        {
            if (TaxType == FixedIncomeTaxType.PRE)
            {
                PreFixedProjection(
                    rateData,
                    holidays);
            }
            else
            {
                PostFixedProjection(
                    rateData,
                    holidays,
                    setLastUpdateDate);
            }
            if (setLastUpdateDate)
            {
                /* The tax rate is updated in D+1, therefore we have to use the previous tax to update bond value
                 * until previous day, and then use the new tax to do the current value projection.
                 */
                LastBondValueUpdate = DateTime.Today.AddDays(-1);
            }
        }

        public void PostFixedProjection(
            RateModel rateData,
            List<DateTime> holidays,
            bool setLastUpdateDate)
        {
            if (rateData == null)
            {
                return;
            }
            DateTime today = setLastUpdateDate
                ? DateTime.Today.AddDays(-1)
                : DateTime.Today;
            CurrentBondValue = GetFutureValue(
                LastBondValue,
                LastBondValueUpdate,
                today,
                holidays,
                rateData,
                FixedIncomeTaxType.POST);
            BondValueAtExpiration = GetFutureValue(
                LastBondValue,
                LastBondValueUpdate,
                Expiration,
                holidays,
                rateData,
                FixedIncomeTaxType.POST);
            NetBondValueAtExpiration = GetFutureValue(
                CurrentBondValue,
                today,
                Expiration,
                holidays,
                rateData,
                FixedIncomeTaxType.POST);
        }

        public void PreFixedProjection(
            RateModel rateData,
            List<DateTime> holidays)
        {
            if (rateData == null)
            {
                return;
            }
            CurrentBondValue = GetFutureValue(
                Capital,
                Hiring,
                DateTime.Today,
                holidays,
                rateData,
                FixedIncomeTaxType.PRE);
            BondValueAtExpiration = GetFutureValue(
                Capital,
                Hiring,
                Expiration,
                holidays,
                rateData,
                FixedIncomeTaxType.PRE);
            //TODO corrigir
            NetBondValueAtExpiration = GetFutureValue(
                Capital,
                Hiring,
                Expiration,
                holidays,
                rateData,
                FixedIncomeTaxType.PRE);
        }

        public double GetFutureValue(
            double initialValue,
            DateTime begin,
            DateTime end,
            List<DateTime> holidays,
            RateModel rateData,
            FixedIncomeTaxType taxType)
        {
            return initialValue *
                Math.Pow(
                    taxType == FixedIncomeTaxType.POST
                        ? GetDailyPostTaxRemuneration(rateData.Rate)
                        : GetDailyPreTaxRemuneration(),
                    GetWorkingDaysBetween(
                        begin,
                        end,
                        holidays));
        }

        public int GetWorkingDaysBetween(
            DateTime begin,
            DateTime end,
            List<DateTime> holidays)
        {
            TimeSpan between = end - begin;
            int workingDays = between.Days + 1;
            int weeks = workingDays / 7;
            if (workingDays > weeks * 7)
            {
                int beginDayOfWeek = GetDayOfWeekAsInt(begin.Date);
                int endDayOfWeek = GetDayOfWeekAsInt(end.Date);
                if (endDayOfWeek < beginDayOfWeek)
                {
                    endDayOfWeek += 7;
                }
                if (beginDayOfWeek <= 6)
                {
                    if (endDayOfWeek >= 7)
                    {
                        workingDays -= 2;
                    }
                    else if (endDayOfWeek >= 6)
                    {
                        workingDays -= 1;
                    }
                }
                else if (beginDayOfWeek <= 7
                    && endDayOfWeek >= 7)
                {
                    workingDays -= 1;
                }
            }
            workingDays -= weeks * 2;
            foreach (DateTime holiday in holidays)
            {
                if (holiday.DayOfWeek != DayOfWeek.Saturday
                    && holiday.DayOfWeek != DayOfWeek.Sunday
                    && begin.Date <= holiday
                    && holiday <= end.Date)
                {
                    --workingDays;
                }
            }
            return workingDays;
        }

        private int GetDayOfWeekAsInt(DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Sunday
                ? 7
                : (int)date.DayOfWeek;
        }
    }
}
