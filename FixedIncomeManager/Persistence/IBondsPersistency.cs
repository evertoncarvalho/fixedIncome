using System;
using System.Collections.Generic;

namespace FixedIncomeManager.Persistence
{
    interface IBondsPersistency<Bond, Tax>
    {
        public string SourceString { get; }
        public bool SaveBonds(ICollection<Bond> items);
        public ICollection<Bond> GetBonds();
        public bool SaveTax(Tax tax);
        public Tax GetTax(Type taxType);
    }
}
