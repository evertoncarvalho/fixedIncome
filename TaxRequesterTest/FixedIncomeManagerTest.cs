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
                DateTime.Now,
                DateTime.Now,
                FixedIncomeType.CDB,
                FixedIncomeIndexer.CDI,
                FixedIncomeTaxType.POST);
            Assert.IsTrue(lastCount < manager.Get().Count);
        }

        [TestMethod]
        public void TryLoadCsv()
        {
            FixedIncomeManager.Manager manager = new FixedIncomeManager.Manager();
            Assert.IsTrue(manager.LoadFromCsv("D:/projects/fixedIncome/fixedIncomeSample.csv"));
        }

        [TestMethod]
        public void LoadCsv()
        {
            FixedIncomeManager.Manager manager = new FixedIncomeManager.Manager();
            manager.LoadFromCsv("D:/projects/fixedIncome/fixedIncomeSample.csv");
            Assert.IsTrue(manager.Get().Count == 42);
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
                FixedIncomeType.CDB,
                FixedIncomeTaxType.POST,
                FixedIncomeIndexer.CDI,
                DateTime.Now,
                DateTime.Now);
        }
    }
}
