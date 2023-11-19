
namespace FixedIncomeManager.Models
{
    public enum RateType
    {
        CDI,
        IPCA12
    }
    public class RateModel
    {
        public double Rate { get; private set; } = 1d;
        public DateTime RateDate { get; private set; } = DateTime.Today;
        public RateType Type { get; private set; }
        public RateModel(RateType type)
        {
            Type = type;
        }
        internal protected void UpdateRate(double rate, DateTime rateDate)
        {
            Rate = rate;
            RateDate = rateDate;
        }
        public override bool Equals(object? obj)
        {
            var other = obj as RateModel;
            if (other == null)
            {
                return false;
            }
            return Rate == other.Rate
                && RateDate == other.RateDate
                && Type == other.Type;
        }
        public override int GetHashCode()
        {
            return GetHashCode();
        }
    }
}
