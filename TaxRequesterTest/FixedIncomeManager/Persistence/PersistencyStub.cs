using FixedIncomeManager.Models;
using FixedIncomeManager.Persistence;

namespace TaxRequesterTest.FixedIncomeManager.Persistence
{
    internal class PersistencyStub
        : IBondsPersistency<FixedIncomeModel, IndexerModel>
    {
        public string SourceString => throw new NotImplementedException();
        public void Initialize()
        {
            //NOOP
        }
        public ICollection<FixedIncomeModel> GetBonds()
        {
            return new HashSet<FixedIncomeModel>();
        }

        public ICollection<IndexerModel> GetRates()
        {
            return new HashSet<IndexerModel>();
        }

        public bool SaveBonds(ICollection<FixedIncomeModel> items)
        {
            return true;
        }

        public bool SaveRates(ICollection<IndexerModel> rates)
        {
            return true;
        }

        public bool Save(FixedIncomeModel item)
        {
            return true;
        }

        public bool SaveRate(IndexerModel rate)
        {
            return true;
        }
    }
}
