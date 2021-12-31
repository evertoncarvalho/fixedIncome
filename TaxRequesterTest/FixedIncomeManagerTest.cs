using FixedIncomeManager;
using FixedIncomeManager.Persistence;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

namespace TaxRequesterTest
{
    [TestClass]
    public class FixedIncomeManagerTest
    {
        [TestMethod]
        public void AddFixedIncome()
        {
            FixedIncomeManager.Manager manager = new FixedIncomeManager.Manager();
            int lastCount = manager.Get().Count;
            manager.Add(
                "name",
                "broker",
                0f,
                0f,
                0f,
                FixedIncomeType.CDB,
                FixedIncomeTaxType.POST,
                FixedIncomeIndexer.CDI,
                DateTime.Now,
                DateTime.Now);
            Assert.IsTrue(lastCount < manager.Get().Count);
        }

        [TestMethod]
        public void TryLoadCsv()
        {
            SpreadsheetCsvParser parser = new SpreadsheetCsvParser("D:/projects/fixedIncome/fixedIncomeSample.csv");
            Assert.IsTrue(parser.Get().Count == 42);
        }

        [TestMethod]
        public void JsonPersistencyControllerSave()
        {
            JsonPersistencyController persistency = GetAndSetupJsonPersistencyController();
            List<FixedIncomeData> items = new List<FixedIncomeData>();
            items.Add(GetFixedIncomeDataSample());
            Assert.IsTrue(persistency.Save(items));
        }

        [TestMethod]
        public void JsonPersistencyControllerSaveCreatePersistencyFile()
        {
            JsonPersistencyController persistency = GetAndSetupJsonPersistencyController();
            List<FixedIncomeData> items = new List<FixedIncomeData>();
            items.Add(GetFixedIncomeDataSample());
            persistency.Save(items);
            Assert.IsTrue(File.Exists(persistency.SourceString));
        }

        [TestMethod]
        public void JsonPersistencyControllerGet()
        {
            JsonPersistencyController persistency = GetAndSetupJsonPersistencyController();
            List<FixedIncomeData> items = new List<FixedIncomeData>();
            items.Add(GetFixedIncomeDataSample());
            persistency.Save(items);
            Assert.IsTrue(persistency.Get().Count == 1);
        }

        [TestMethod]
        public void WorkingDaysCountOverTheYear()
        {
            Manager manager = new Manager();
            int wokingDays = GetFixedIncomeDataSample().GetWorkingDaysBetween(
                new DateTime(2021, 12, 30),
                new DateTime(2024, 12, 31),
                manager.GetHolidays("D:/projects/fixedIncome/holidays.csv"));
            Assert.IsTrue(wokingDays == 756);
        }

        [TestMethod]
        public void WorkingDaysCountWeekend()
        {
            Manager manager = new Manager();
            int wokingDays = GetFixedIncomeDataSample().GetWorkingDaysBetween(
                new DateTime(2021, 12, 25),
                new DateTime(2021, 12, 26),
                manager.GetHolidays("D:/projects/fixedIncome/holidays.csv"));
            Assert.IsTrue(wokingDays == 0);
        }

        [TestMethod]
        public void WorkingDaysCountFridayToMonday()
        {
            Manager manager = new Manager();
            int wokingDays = GetFixedIncomeDataSample().GetWorkingDaysBetween(
                new DateTime(2021, 12, 31),
                new DateTime(2022, 1, 3),
                manager.GetHolidays("D:/projects/fixedIncome/holidays.csv"));
            Assert.IsTrue(wokingDays == 2);
        }

        private JsonPersistencyController GetAndSetupJsonPersistencyController()
        {
            string persistencyFileName = "persistencyTest.json";
            File.Delete(persistencyFileName);
            return new JsonPersistencyController(persistencyFileName);
        }

        private FixedIncomeData GetFixedIncomeDataSample()
        {
            return new FixedIncomeData(
                "name",
                "broker",
                0f,
                0f,
                0f,
                FixedIncomeType.CDB,
                FixedIncomeTaxType.POST,
                FixedIncomeIndexer.CDI,
                DateTime.Now,
                DateTime.Now);
        }
    }
}
