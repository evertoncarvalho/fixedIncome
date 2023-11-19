using FixedIncomeManager.Models;
using Newtonsoft.Json;

namespace FixedIncomeManager.Persistence
{
    public class JsonPersistencyController
        : IBondsPersistency<FixedIncomeModel, RateModel>
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

        public ICollection<FixedIncomeModel> GetBonds()
        {
            return ReadDatabase<FixedIncomeModel>(_sourceString);
        }

        public bool SaveBonds(ICollection<FixedIncomeModel> items)
        {
            return Save(items, _sourceString);
        }

        public bool SaveRates(ICollection<RateModel> rates)
        {
            return Save(rates, RateSourceString);
        }

        public ICollection<RateModel> GetRates()
        {
            return ReadDatabase<RateModel>(RateSourceString);
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
