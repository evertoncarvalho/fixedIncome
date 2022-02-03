using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using TaxRequester;

namespace FixedIncomeManager.Persistence
{
    public class JsonPersistencyController
        : IBondsPersistency<FixedIncomeData, CDIData>
    {
        private string _sourceString = null;
        public string SourceString
        {
            get
            {
                return _sourceString;
            }
        }

        public JsonPersistencyController(string sourceString = null)
        {
            _sourceString = string.IsNullOrWhiteSpace(sourceString)
                ? "persistency/fixedIncome.json"
                : sourceString;
        }

        public ICollection<FixedIncomeData> GetBonds()
        {
            List<FixedIncomeData> items = new List<FixedIncomeData>(0);
            if (File.Exists(SourceString))
            {
                using (StreamReader reader = new StreamReader(SourceString))
                {
                    string line = null;
                    while ((line = reader.ReadLine()) != null)
                    {
                        items.Add(JsonConvert.DeserializeObject<FixedIncomeData>(line));
                    }
                }
            }
            return items;
        }

        public bool SaveBonds(ICollection<FixedIncomeData> items)
        {
            try
            {
                using(StreamWriter writer = new StreamWriter(SourceString))
                {
                    foreach(FixedIncomeData item in items)
                    {
                        writer.WriteLine(JsonConvert.SerializeObject(item));
                    }
                }
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }
        public bool SaveTax(CDIData tax)
        {
            return false;
        }
        public CDIData GetTax(Type taxType)
        {
            return null;
        }
    }
}
