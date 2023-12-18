
namespace FixedIncomeManager.Persistence
{
    public interface IBondsPersistency<Bond, Rate>
    {
        public string SourceString { get; }
        public void Initialize();
        public bool Save(Bond item);
        public bool SaveBonds(ICollection<Bond> items);
        public ICollection<Bond> GetBonds();
        public bool SaveRate(Rate rate);
        public bool SaveRates(ICollection<Rate> rates);
        public ICollection<Rate> GetRates();
    }
}
