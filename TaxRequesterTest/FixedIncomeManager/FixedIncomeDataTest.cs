using FixedIncomeManager.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TaxRequesterTest.FixedIncomeManager
{
    [TestClass]
    public class FixedIncomeDataTest
        : FixedIncomeModel
    {
        public FixedIncomeDataTest()
            : base("name",
                  "broker",
                  0f,
                  0f,
                  0f,
                  FixedIncomeType.CDB,
                  FixedIncomeTaxType.POST,
                  FixedIncomeIndexer.CDI,
                  DateTime.Now,
                  DateTime.Now)
        {
            //NOOP
        }
        [TestMethod]
        public void LCATaxation()
        {
            Assert.IsTrue(CheckNoTaxBonds(
                FixedIncomeType.LCA,
                DateTime.Now.Date));
        }
        [TestMethod]
        public void LCITaxation()
        {
            Assert.IsTrue(CheckNoTaxBonds(
                FixedIncomeType.LCI,
                DateTime.Now.Date));
        }
        [TestMethod]
        public void CDBTaxation()
        {
            Assert.IsTrue(CheckTaxBonds(
                FixedIncomeType.CDB,
                DateTime.Now.Date));
        }
        [TestMethod]
        public void CRATaxation()
        {
            Assert.IsTrue(CheckTaxBonds(
                FixedIncomeType.CRA,
                DateTime.Now.Date));
        }
        [TestMethod]
        public void CRITaxation()
        {
            Assert.IsTrue(CheckTaxBonds(
                FixedIncomeType.CRI,
                DateTime.Now.Date));
        }
        [TestMethod]
        public void LCTaxation()
        {
            Assert.IsTrue(CheckTaxBonds(
                   FixedIncomeType.LC,
                   DateTime.Now.Date));
        }
        private bool CheckNoTaxBonds(
            FixedIncomeType type,
            DateTime hiringDate)
        {
            Type = type;
            Hiring = hiringDate;
            Expiration = Hiring.AddDays(179);
            bool ok = Taxation == 0f;
            Expiration = Hiring.AddDays(359);
            ok &= Taxation == 0f;
            Expiration = Hiring.AddDays(719);
            ok &= Taxation == 0f;
            Expiration = Hiring.AddDays(720);
            ok &= Taxation == 0f;
            Expiration = Hiring.AddDays(1096);
            ok &= Taxation == 0f;
            return ok;
        }
        private bool CheckTaxBonds(
            FixedIncomeType type,
            DateTime hiringDate)
        {
            Type = type;
            Hiring = hiringDate;
            Expiration = Hiring.AddDays(179);
            bool ok = Taxation == 0.225f;
            Expiration = Hiring.AddDays(359);
            ok &= Taxation == 0.2f;
            Expiration = Hiring.AddDays(719);
            ok &= Taxation == 0.175f;
            Expiration = Hiring.AddDays(720);
            ok &= Taxation == 0.15f;
            Expiration = Hiring.AddDays(1096);
            ok &= Taxation == 0.15f;
            return ok;
        }
    }
}
