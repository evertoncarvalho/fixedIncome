using System.Collections.Generic;

namespace FixedIncomeManager.Persistence
{
    interface IPersistencyController<T>
    {
        public string SourceString { get; }
        public bool Save(ICollection<T> items);
        public ICollection<T> Get();
    }
}
