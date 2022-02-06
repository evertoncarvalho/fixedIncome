using FixedIncomeManager;
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
            Manager manager = new Manager();
            Print(manager);
            manager.Save();
            Console.ReadKey();
        }

        static void CreatePersistencyDirectory()
        {
            string directory = "persistency";
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        static void Print(Manager manager)
        {
            PrintTax(manager);
            PrintBonds(manager);
            PrintSummary(manager);
        }

        static void PrintTax(Manager manager)
        {
            Console.WriteLine("cdi em {0}: {1}%\t\tipca em {2}: {3}%\n",
                manager.CDIData.RateDate.ToString("dd/MM/yy"),
                manager.CDIData.Rate,
                manager.IPCAData.RateDate.ToString("dd/MM/yy"),
                manager.IPCAData.Rate);
        }

        static void PrintBonds(Manager manager)
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

        static void PrintSummary(Manager manager)
        {
            FixedIncomeSummaryData summary = manager.GetSummary();
            Console.WriteLine("\nCapital:\t\t{0:C}\nCurrent Value:\t\t{1:C}\nAt Expiration:\t\t{2:C}",
                summary.Capital,
                summary.CurrentValue,
                summary.ValueAtExpiration);
        }
    }
}
