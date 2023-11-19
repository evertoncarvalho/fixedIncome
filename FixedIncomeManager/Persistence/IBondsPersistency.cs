
namespace FixedIncomeManager.Persistence
{
    public interface IBondsPersistency<Bond, Rate>
    {
        public string SourceString { get; }
        public bool SaveBonds(ICollection<Bond> items);
        public ICollection<Bond> GetBonds();
        public bool SaveRates(ICollection<Rate> rates);
        public ICollection<Rate> GetRates();
    }
}
