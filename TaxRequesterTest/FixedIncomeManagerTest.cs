using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                0f,
                DateTime.Now,
                DateTime.Now,
                FixedIncomeManager.FixedIncomeSegment.CDI,
                FixedIncomeManager.FixedIncomeTaxType.POST);
            Assert.IsTrue(lastCount < manager.Get().Count);
        }
    }
}
