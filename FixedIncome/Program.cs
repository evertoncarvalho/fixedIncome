using System;
using System.IO;

namespace FixedIncome
{
    class Program
    {
        static void Main(string[] args)
        {
            CreatePersistencyDirectory();
            //new FixedIncomeManager.Persistence.JsonPersistencyController().Save(new FixedIncomeManager.Persistence.SpreadsheetCsvParser("fixedIncomeSample.csv").Get());
            FixedIncomeManager.Manager manager = new FixedIncomeManager.Manager();
            Print(manager);
            manager.Save();
        }

        static void CreatePersistencyDirectory()
        {
            string directory = "persistency";
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        static void Print(FixedIncomeManager.Manager manager)
        {
            int count = 0;
            foreach (var item in manager.Get())
            {
                Console.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}",
                    ++count,
                    item.Capital.ToString("C"),
                    item.Hiring.ToString("dd/MM/yy"),
                    item.CurrentBondValue.ToString("C"),
                    item.BondValueAtExpiration.ToString("C"),
                    item.Expiration.ToString("dd/MM/yy"),
                    item.Type,
                    item.Indexer,
                    item.Name);
            }
        }
    }
}
