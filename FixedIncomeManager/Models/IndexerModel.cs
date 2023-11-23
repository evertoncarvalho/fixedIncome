
namespace FixedIncomeManager.Models
{
    public enum IndexerType
    {
        CDI,
        IPCA12
    }
    public class IndexerModel
    {
        public double Value { get; private set; } = 1d;
        public DateTime UpdatedAt { get; private set; } = DateTime.Today;
        public IndexerType Type { get; private set; }
        public IndexerModel(IndexerType type)
        {
            Type = type;
        }
        internal protected void UpdateRate(double value, DateTime updatedAt)
        {
            Value = value;
            UpdatedAt = updatedAt;
        }
        public override bool Equals(object? obj)
        {
            var other = obj as IndexerModel;
            if (other == null)
            {
                return false;
            }
            return Value == other.Value
                && UpdatedAt == other.UpdatedAt
                && Type == other.Type;
        }
        public override int GetHashCode()
        {
            return GetHashCode();
        }
    }
}
