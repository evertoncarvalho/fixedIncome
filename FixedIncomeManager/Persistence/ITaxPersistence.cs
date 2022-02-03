
namespace FixedIncomeManager.Persistence
{
    public interface ITaxPersistence<TaxType>
    {
        public bool Save(TaxType tax);
        public TaxType GetTax();
    }
}
