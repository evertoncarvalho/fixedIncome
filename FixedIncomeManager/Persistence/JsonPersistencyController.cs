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
        public string RateSourceString { get; private set; }

        public JsonPersistencyController(string sourceString = "persistency")
        {
            _sourceString = sourceString + "/fixedIncome.json";
            RateSourceString = sourceString + "/rates.json";
        }

        public JsonPersistencyController(string directory, string fileName)
        {
            _sourceString = $"{directory}/{fileName}.json";
            RateSourceString = $"{directory}/{fileName}Rates.json";
        }

        public ICollection<FixedIncomeData> GetBonds()
        {
            return ReadDatabase<FixedIncomeData>(_sourceString);
        }

        public bool SaveBonds(ICollection<FixedIncomeData> items)
        {
            return Save(items, _sourceString);
        }

        public bool SaveRates(ICollection<CDIData> rates)
        {
            return Save(rates, RateSourceString);
        }

        public ICollection<CDIData> GetRates()
        {
            return ReadDatabase<CDIData>(RateSourceString);
        }

        private bool Save<T>(ICollection<T> content,
            string sourceString)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(sourceString))
                {
                    foreach (T item in content)
                    {
                        writer.WriteLine(JsonConvert.SerializeObject(item));
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        private ICollection<T> ReadDatabase<T>(string sourceString)
        {
            List<T> items = new List<T>(0);
            if (File.Exists(sourceString))
            {
                using (StreamReader reader = new StreamReader(sourceString))
                {
                    string line = null;
                    while ((line = reader.ReadLine()) != null)
                    {
                        items.Add(JsonConvert.DeserializeObject<T>(line));
                    }
                }
            }
            return items;
        }
    }
}
