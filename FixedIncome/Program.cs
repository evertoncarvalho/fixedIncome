using FixedIncomeManager.Persistence;
using System;
using System.IO;

namespace FixedIncome
{
    class Program
    {
        static void Main(string[] args)
        {
            CreatePersistencyDirectory();
            FixedIncomeManager.Manager manager = new FixedIncomeManager.Manager();
            manager.Save();
            //new JsonPersistencyController().Save(new SpreadsheetCsvParser("fixedIncomeSample.csv").Get());
            //TaxRequester.Requester requester = new TaxRequester.Requester();
            //TaxRequester.CDIData cdi = requester.GetCDI();
            //Console.WriteLine(cdi.AsString);
            //Console.WriteLine(requester.GetIPCALast12Months().AsString);
        }

        static void CreatePersistencyDirectory()
        {
            string directory = "persistency";
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }
    }
}
