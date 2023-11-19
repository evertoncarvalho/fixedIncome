using FixedIncomeManager;
using FixedIncomeManager.Models;
using FixedIncomeManager.Persistence;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaxRequester;
using TaxRequesterTest.FixedIncomeManager.Persistence;

namespace TaxRequesterTest.FixedIncomeManager
{
    [TestClass]
    public class FixedIncomeManagerTest
        : Manager
    {
        public FixedIncomeManagerTest()
            : base(new PersistencyStub())
        {
            //NOOP
        }

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
            List<FixedIncomeModel> items = new List<FixedIncomeModel>();
            items.Add(GetFixedIncomeDataSample());
            Assert.IsTrue(persistency.SaveBonds(items));
        }

        [TestMethod]
        public void JsonPersistencyControllerSaveRate()
        {
            JsonPersistencyController persistency = GetAndSetupJsonPersistencyController();
            List<RateModel> items = new List<RateModel>();
            items.Add(GetRateDataSample(RateType.CDI, 9, DateTime.Now.Date));
            Assert.IsTrue(persistency.SaveRates(items));
        }

        [TestMethod]
        public void JsonPersistencyControllerGetRates()
        {
            JsonPersistencyController persistency = GetAndSetupJsonPersistencyController();
            List<RateModel> items = new List<RateModel>();
            items.Add(GetRateDataSample(RateType.CDI, 9, DateTime.Now.Date));
            items.Add(GetRateDataSample(RateType.IPCA12, 9, DateTime.Now.Date));
            Assert.IsTrue(persistency.SaveRates(items));
            Assert.AreEqual(persistency.GetRates().Count, 2);
        }

        [TestMethod]
        public void JsonPersistencyControllerSaveCreatePersistencyFile()
        {
            JsonPersistencyController persistency = GetAndSetupJsonPersistencyController();
            List<FixedIncomeModel> items = new List<FixedIncomeModel>();
            items.Add(GetFixedIncomeDataSample());
            persistency.SaveBonds(items);
            Assert.IsTrue(File.Exists(persistency.SourceString));
        }

        [TestMethod]
        public void JsonPersistencyControllerGet()
        {
            JsonPersistencyController persistency = GetAndSetupJsonPersistencyController();
            List<FixedIncomeModel> items = new List<FixedIncomeModel>();
            items.Add(GetFixedIncomeDataSample());
            persistency.SaveBonds(items);
            Assert.IsTrue(persistency.GetBonds().Count == 1);
        }

        [TestMethod]
        public void WorkingDaysCountOverTheYear()
        {
            int workingDays = GetFixedIncomeDataSample().GetWorkingDaysBetween(
                new DateTime(2021, 12, 30),
                new DateTime(2024, 12, 31),
                GetHolidays("D:/projects/fixedIncome/holidays.csv"));
            Assert.IsTrue(workingDays == 756);
        }

        [TestMethod]
        public void WorkingDaysCountWeekend()
        {
            int wokingDays = GetFixedIncomeDataSample().GetWorkingDaysBetween(
                new DateTime(2021, 12, 25),
                new DateTime(2021, 12, 26),
                GetHolidays("D:/projects/fixedIncome/holidays.csv"));
            Assert.IsTrue(wokingDays == 0);
        }

        [TestMethod]
        public void WorkingDaysCountFridayToMonday()
        {
            int wokingDays = GetFixedIncomeDataSample().GetWorkingDaysBetween(
                new DateTime(2021, 12, 31),
                new DateTime(2022, 1, 3),
                GetHolidays("D:/projects/fixedIncome/holidays.csv"));
            Assert.IsTrue(wokingDays == 2);
        }
        [TestMethod]
        public void DailyCDI()
        {
            Assert.AreEqual(0.000455131, new ProportionalCalculator().GetDailyRate(1d, 12.15));
            Assert.AreEqual(0.000537055, new ProportionalCalculator().GetDailyRate(1.18, 12.15));
        }
        [TestMethod]
        public void DailyIPCA()
        {
            Assert.AreEqual(0.000200806, new FixedRateCalculator().GetDailyRate(0d, 5.19));
            Assert.AreEqual(0.000484566, new FixedRateCalculator().GetDailyRate(7.41, 5.19));
        }
        private JsonPersistencyController GetAndSetupJsonPersistencyController()
        {
            string persistencyFileName = "persistencyTest";
            JsonPersistencyController jsonPersistency = new JsonPersistencyController(".", persistencyFileName);
            File.Delete(jsonPersistency.SourceString);
            File.Delete(jsonPersistency.RateSourceString);
            return jsonPersistency;
        }

        private FixedIncomeModel GetFixedIncomeDataSample()
        {
            return new FixedIncomeModel(
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

        private RateModel GetRateDataSample(
            RateType type,
            double rate,
            DateTime date)
        {
            return new RateModelTest(type, rate, date);
        }

        class RateModelTest
            : RateModel
        {
            public RateModelTest(RateType type, double rate, DateTime date)
                : base(type)
            {
                UpdateRate(rate, date);
            }
        }
    }
}
