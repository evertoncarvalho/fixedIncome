using FixedIncomeManager;
using System.Text;

namespace FixedIncome
{
    class Program
    {
        static void Main(string[] args)
        {
            CreatePersistencyDirectory();
            var persistency = new FixedIncomeManager.Persistence.JsonPersistencyController();
            persistency.SaveBonds(
                new FixedIncomeManager.Persistence.SpreadsheetCsvParser().GetBonds());
            Manager manager = new Manager(persistency);
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
            StringBuilder builder = new StringBuilder("cdi ");
            string unnavailable = "indisponível\n";
            if (manager.CDIRate != null)
            {
                builder.Append($"em {manager.CDIRate.RateDate.ToString("dd/MM/yy")}: {manager.CDIRate.RateDate}\n");
            }
            else
            {
                builder.Append(unnavailable);
            }
            builder.Append("ipca ");
            if(manager.IPCA12Rate != null)
            {
                builder.Append($"em {manager.IPCA12Rate.RateDate.ToString("dd/MM/yy")}: {manager.IPCA12Rate.Rate}\n");
            }
            else
            {
                builder.Append("indisponível\n");
            }
            Console.WriteLine(builder);
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
