using FixedIncomeManager;
using FixedIncomeManager.Persistence;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using TaxRequester;

namespace TaxRequesterTest.FixedIncomeManager
{
    [TestClass]
    public class FixedIncomeManagerTest
        : Manager
    {
        [TestMethod]
        public void AddFixedIncome()
        {
            int lastCount = Get().Count;
            Add(
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
            Assert.IsTrue(lastCount < Get().Count);
        }

        [TestMethod]
        public void JsonPersistencyControllerSaveBonds()
        {
            JsonPersistencyController persistency = GetAndSetupJsonPersistencyController();
            List<FixedIncomeData> items = new List<FixedIncomeData>();
            items.Add(GetFixedIncomeDataSample());
            Assert.IsTrue(persistency.SaveBonds(items));
        }

        [TestMethod]
        public void JsonPersistencyControllerSaveRate()
        {
            JsonPersistencyController persistency = GetAndSetupJsonPersistencyController();
            List<CDIData> items = new List<CDIData>();
            items.Add(GetRateDataSample(9, DateTime.Now.Date));
            Assert.IsTrue(persistency.SaveRates(items));
        }

        [TestMethod]
        public void JsonPersistencyControllerGetRates()
        {
            JsonPersistencyController persistency = GetAndSetupJsonPersistencyController();
            List<CDIData> items = new List<CDIData>();
            items.Add(GetRateDataSample(9, DateTime.Now.Date));
            items.Add(GetRateDataSample(9, DateTime.Now.Date));
            Assert.IsTrue(persistency.SaveRates(items));
            Assert.AreEqual(persistency.GetRates().Count, 2);
        }

        [TestMethod]
        public void JsonPersistencyControllerSaveCreatePersistencyFile()
        {
            JsonPersistencyController persistency = GetAndSetupJsonPersistencyController();
            List<FixedIncomeData> items = new List<FixedIncomeData>();
            items.Add(GetFixedIncomeDataSample());
            persistency.SaveBonds(items);
            Assert.IsTrue(File.Exists(persistency.SourceString));
        }

        [TestMethod]
        public void JsonPersistencyControllerGet()
        {
            JsonPersistencyController persistency = GetAndSetupJsonPersistencyController();
            List<FixedIncomeData> items = new List<FixedIncomeData>();
            items.Add(GetFixedIncomeDataSample());
            persistency.SaveBonds(items);
            Assert.IsTrue(persistency.GetBonds().Count == 1);
        }

        [TestMethod]
        public void WorkingDaysCountOverTheYear()
        {
            int wokingDays = GetFixedIncomeDataSample().GetWorkingDaysBetween(
                new DateTime(2021, 12, 30),
                new DateTime(2024, 12, 31),
                GetHolidays("D:/projects/fixedIncome/holidays.csv"));
            Assert.IsTrue(wokingDays == 756);
        }

        [TestMethod]
        public void WorkingDaysCountWeekend()
        {
            Manager manager = new Manager();
            int wokingDays = GetFixedIncomeDataSample().GetWorkingDaysBetween(
                new DateTime(2021, 12, 25),
                new DateTime(2021, 12, 26),
                GetHolidays("D:/projects/fixedIncome/holidays.csv"));
            Assert.IsTrue(wokingDays == 0);
        }

        [TestMethod]
        public void WorkingDaysCountFridayToMonday()
        {
            Manager manager = new Manager();
            int wokingDays = GetFixedIncomeDataSample().GetWorkingDaysBetween(
                new DateTime(2021, 12, 31),
                new DateTime(2022, 1, 3),
                GetHolidays("D:/projects/fixedIncome/holidays.csv"));
            Assert.IsTrue(wokingDays == 2);
        }

        private JsonPersistencyController GetAndSetupJsonPersistencyController()
        {
            string persistencyFileName = "persistencyTest";
            JsonPersistencyController jsonPersistency = new JsonPersistencyController(".", persistencyFileName);
            File.Delete(jsonPersistency.SourceString);
            File.Delete(jsonPersistency.RateSourceString);
            return jsonPersistency;
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

        private CDIData GetRateDataSample(
            double rate,
            DateTime date)
        {
            return new CDIData()
            {
                Rate = rate,
                RateDate = date
            };
        }
    }
}
