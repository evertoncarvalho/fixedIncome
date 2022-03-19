using FixedIncomeManager;
using FixedIncomeManager.Persistence;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TaxRequesterTest.FixedIncomeManager.Persistence
{
    [TestClass]
    public class SpreadsheetCsvParserTest
        : SpreadsheetCsvParser
    {
        public SpreadsheetCsvParserTest()
            : base(string.Empty)
        {
            //NOOP
        }

        [TestMethod]
        public void ParseCsv()
        {
            Assert.IsNotNull(
                GetFixedIncomeDataFromJson(
                    $"R$ 15.000,00;R$ 17.115,81;02/04/19;100,00%;CDI;0,00%;100,00%;Original;1,69%;" +
                    $"1,69%;1,69%;R$ 17.123,30;R$ 17.130,79;R$ 17.205,86;R$ 2.205,86;R$ 7,49;" +
                    $"R$ 7,49;04/04/22;XPI;LCA;Pós;1098;757;16;10;0,000437392423016;" +
                    $"0,000437392423016"));
        }
        [TestMethod]
        public void GetFixedIncomeTaxTypeTest()
        {
            Assert.AreEqual(GetFixedIncomeTaxType("Pós"), FixedIncomeTaxType.POST);
            Assert.AreEqual(GetFixedIncomeTaxType("Pré"), FixedIncomeTaxType.PRE);
        }
        [TestMethod]
        public void GetFixedIncomeTaxTypeWithInvalidCharacter()
        {
            Assert.AreEqual(GetFixedIncomeTaxType("P�s"), FixedIncomeTaxType.POST);
        }
    }
}
