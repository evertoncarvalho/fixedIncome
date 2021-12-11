using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using TaxRequester;

namespace TaxRequesterTest
{
    [TestClass]
    public class TaxRequestTest
    {
        private const string _cdiDataAsJson = "{\"taxa\":\"7,65\",\"dataTaxa\":\"03/12/2021\","
                + "\"indice\":\"33.479,60\",\"dataIndice\":\"06/12/2021\"}";

        [TestMethod]
        public void ParseCDITax()
        {
            CDIData cdiData = JsonConvert.DeserializeObject<CDIData>(_cdiDataAsJson);
            Assert.IsTrue((cdiData.Tax - 7.65) < 0.001);
        }

        [TestMethod]
        public void ParseCDIIndex()
        {
            CDIData cdiData = JsonConvert.DeserializeObject<CDIData>(_cdiDataAsJson);
            Assert.AreEqual(cdiData.Index, 33479,60);
        }

        [TestMethod]
        public void ParseCDITaxDate()
        {
            CDIData cdiData = JsonConvert.DeserializeObject<CDIData>(_cdiDataAsJson);
            Assert.AreEqual(cdiData.TaxDate, DateTime.Parse("03/12/2021"));
        }

        [TestMethod]
        public void ParseCDIIndexDate()
        {
            CDIData cdiData = JsonConvert.DeserializeObject<CDIData>(_cdiDataAsJson);
            Assert.AreEqual(cdiData.IndexDate, DateTime.Parse("06/12/2021"));
        }
    }
}
