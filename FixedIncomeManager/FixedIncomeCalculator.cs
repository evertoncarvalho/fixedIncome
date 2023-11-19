
namespace FixedIncomeManager
{
    public abstract class FixedIncomeCalculator
    {
        protected const int _rateDigits = 9;

        /// <summary>
        /// Calculate the daily rate considering bond indexer and remuneration
        /// </summary>
        /// <param name="remuneration">Bond remuneration</param>
        /// <param name="indexerValue">Indexer value</param>
        /// <returns>Equivalent daily rate</returns>
        public abstract double GetDailyRate(double remuneration, double indexerValue);

        /// <summary>
        /// Converts anual rates in daily rates considering only working days
        /// </summary>
        /// <param name="rate">Anual rate</param>
        /// <returns>Equivalent daily rate</returns>
        public double ConvertToDailyRate(double rate)
        {
            return Math.Round(
                    Math.Pow(1 + rate / 100, 1f / 252) - 1,
                    _rateDigits,
                    MidpointRounding.ToZero);
        }
    }

    /// <summary>
    /// Used for any bond calculated with it proportional index value (something like 115% CDI)
    /// </summary>
    public class ProportionalCalculator
        : FixedIncomeCalculator
    {
        public override double GetDailyRate(double remuneration, double indexerValue)
        {
            return Math.Round(
                    remuneration * ConvertToDailyRate(indexerValue),
                    _rateDigits,
                    MidpointRounding.AwayFromZero);
        }
    }

    /// <summary>
    /// Used for any bond calculated with fixed rate part plus some index value (something like IPCA+5% or CDI+2.5%
    /// </summary>
    public class FixedRateCalculator
        : FixedIncomeCalculator
    {
        public override double GetDailyRate(double remuneration, double indexerValue)
        {
            if(remuneration == 0)
            {
                return ConvertToDailyRate(indexerValue);
            }
            return Math.Round(
                    ConvertToDailyRate(((1 + remuneration / 100) * (1 + indexerValue / 100) -1) * 100),
                    _rateDigits,
                    MidpointRounding.AwayFromZero);
        }
    }
}
