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
        public float Capital { get; private set; }
        /// <summary>
        /// Tax remuneration
        /// </summary>
        public float Remuneration { get; private set; }
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
        public float LastBondValue { get; private set; }
        /// <summary>
        /// Current bond value projection
        /// </summary>
        public float CurrentBondValue { get; set; }
        /// <summary>
        /// Bond value projection at the expiration
        /// </summary>
        public float BondValueAtExpiration { get; set; }
        /// <summary>
        /// Bond value projection at the expiration without fees
        /// </summary>
        public float NetBondValueAtExpiration { get; set; }
        /// <summary>
        /// Net Profit at bonds the expiration
        /// </summary>
        public float Profit
        {
            get
            {
                return NetBondValueAtExpiration - Capital;
            }
        }
        /// <summary>
        /// Brazilian taxation for fixed income
        /// </summary>
        public virtual float Taxation
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
            float capital,
            float lastBondValue,
            float remuneration,
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

        public float GetDailyTaxRemuneration(BaseTaxData taxData, bool netTax = false)
        {
            float taxation = netTax
                ? 1f - Taxation
                : 1f;
            if (Indexer == FixedIncomeIndexer.CDI)
            {
                return 1 + Remuneration * taxation / 100 * taxData.GetTaxDaily();
            }
            return 1 + taxData.GetTaxDaily(Remuneration) * taxation;
        }

        public void UpdateBondsValueProjections(
            BaseTaxData taxData,
            List<DateTime> holidays)
        {
            CurrentBondValue =
                LastBondValue *
                (float)Math.Pow(
                    GetDailyTaxRemuneration(taxData),
                    GetWorkingDaysBetween(
                        LastBondValueUpdate,
                        DateTime.Now.Date,
                        holidays) - 1);
            BondValueAtExpiration =
                LastBondValue *
                (float)Math.Pow(
                    GetDailyTaxRemuneration(taxData),
                    GetWorkingDaysBetween(
                        LastBondValueUpdate,
                        Expiration,
                        holidays) - 1);
            NetBondValueAtExpiration =
                CurrentBondValue *
                (float)Math.Pow(
                    GetDailyTaxRemuneration(taxData, true),
                    GetWorkingDaysBetween(
                        DateTime.Now.Date,
                        Expiration,
                        holidays) - 1);
        }

        public int GetWorkingDaysBetween(DateTime begin, DateTime end, List<DateTime> holidays)
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
