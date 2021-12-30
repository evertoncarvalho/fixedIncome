using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

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
                FixedIncomeManager.FixedIncomeType.CDB,
                FixedIncomeManager.FixedIncomeIndexer.CDI,
                FixedIncomeManager.FixedIncomeTaxType.POST);
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
    }
}
