using FixedIncomeManager.Models;
using FixedIncomeManager.Persistence;

namespace TaxRequesterTest.FixedIncomeManager.Persistence
{
    internal class PersistencyStub
        : IBondsPersistency<FixedIncomeModel, RateModel>
    {
        public string SourceString => throw new NotImplementedException();

        public ICollection<FixedIncomeModel> GetBonds()
        {
            return new HashSet<FixedIncomeModel>();
        }

        public ICollection<RateModel> GetRates()
        {
            return new HashSet<RateModel>();
        }

        public bool SaveBonds(ICollection<FixedIncomeModel> items)
        {
            return true;
        }

        public bool SaveRates(ICollection<RateModel> rates)
        {
            return true;
        }
    }
}
