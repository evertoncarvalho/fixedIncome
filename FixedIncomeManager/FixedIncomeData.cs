using System;
using System.Collections.Generic;
using TaxRequester;

namespace FixedIncomeManager
{
    public enum FixedIncomeIndexer
    {
        CDI,
        IPCA
    }

    public enum FixedIncomeTaxType
    {
        PRE,
        POST
    }

    public enum FixedIncomeType
    {
        LC,
        LCA,
        LCI,
        CDB
    }

    public class FixedIncomeData
    {
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
        public FixedIncomeType Type { get; private set; }
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
        public DateTime Hiring { get; private set; }
        /// <summary>
        /// Expiration Data
        /// </summary>
        public DateTime Expiration { get; private set; }
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

        public FixedIncomeData(
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
        }

        public double GetDailyPostTaxRemuneration(
            BaseRateData rateData,
            bool netTax = false)
        {
            double taxation = netTax
                ? 1f - Taxation
                : 1f;
            if (Indexer == FixedIncomeIndexer.CDI)
            {
                return 1 + Remuneration * taxation / 100 * rateData.GetRateDaily();
            }
            return 1 + rateData.GetRateDaily(Remuneration) * taxation;
        }

        public double GetDailyPreTaxRemuneration(
            BaseRateData rateData,
            bool netTax = false)
        {
            double taxation = netTax
                ? 1f - Taxation
                : 1f;
            return 1 + taxation * rateData.GetRateDaily(Remuneration);
        }

        public void UpdateBondsValueProjections(
            BaseRateData rateData,
            List<DateTime> holidays)
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
                    holidays);
            }
        }

        public void PostFixedProjection(
            BaseRateData rateData,
            List<DateTime> holidays)
        {
            CurrentBondValue = GetFutureValue(
                LastBondValue,
                LastBondValueUpdate,
                DateTime.Now.Date,
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
                DateTime.Now.Date,
                Expiration,
                holidays,
                rateData,
                FixedIncomeTaxType.POST,
                true);
        }

        public void PreFixedProjection(
            BaseRateData rateData,
            List<DateTime> holidays)
        {
            CurrentBondValue = GetFutureValue(
                Capital,
                Hiring,
                DateTime.Now,
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
            NetBondValueAtExpiration = GetFutureValue(
                Capital,
                Hiring,
                Expiration,
                holidays,
                rateData,
                FixedIncomeTaxType.PRE,
                true);
        }

        public double GetFutureValue(
            double initialValue,
            DateTime begin,
            DateTime end,
            List<DateTime> holidays,
            BaseRateData rateData,
            FixedIncomeTaxType taxType,
            bool netRate = false)
        {
            return initialValue *
                Math.Pow(
                    taxType == FixedIncomeTaxType.POST
                        ? GetDailyPostTaxRemuneration(rateData, netRate)
                        : GetDailyPreTaxRemuneration(rateData, netRate),
                    GetWorkingDaysBetween(
                        begin,
                        end,
                        holidays) - 1);
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
