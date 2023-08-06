using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TaxRequester;

namespace TaxRequesterTest.TaxRequester
{
    [TestClass]
    public class TaxRequestTest
        : Requester
    {
        private const string _cdiDataAsJson = "{\"taxa\":\"7,65\",\"dataTaxa\":\"03/12/2021\","
            + "\"indice\":\"33.479,60\",\"dataIndice\":\"06/12/2021\"}";
        private const string _ipcaDataAsJson = "\"1737\":{\"id\":\"2265\","
            + "\"variavel\":\"IPCA - Variação acumulada em 12 meses\","
            + "\"unidade\":\"%\",\"resultados\":[{\"classificacoes\":[],"
            + "\"series\":[{\"localidade\":{\"id\":\"1\",\"nivel\":{\"id\":\"N1\",\"nome\":\"Brasil\"},"
            + "\"nome\":\"Brasil\"},\"serie\":{\"202111\":\"10.74\",\"202112\":\"10.06\"}}]}]}";

        [TestMethod]
        public void ParseCDIDataFromJson()
        {
            BaseRateData cdiData = GetCDI(_cdiDataAsJson);
            Assert.IsTrue(cdiData is CDIData);
        }

        [TestMethod]
        public void ParseCDIRateFromJson()
        {
            CDIData cdiData = GetCDI(_cdiDataAsJson);
            Assert.IsTrue((cdiData.Rate - 7.65) < 0.001);
        }

        [TestMethod]
        public void ParseCDIIndexFromJson()
        {
            CDIData cdiData = GetCDI(_cdiDataAsJson);
            Assert.AreEqual(cdiData.Index, 33479, 60);
        }

        [TestMethod]
        public void ParseCDIRateDate()
        {
            CDIData cdiData = GetCDI(_cdiDataAsJson);
            Assert.AreEqual(cdiData.RateDate, DateTime.Parse("03/12/2021"));
        }

        [TestMethod]
        public void ParseCDIIndexDate()
        {
            CDIData cdiData = GetCDI(_cdiDataAsJson);
            Assert.AreEqual(cdiData.IndexDate, DateTime.Parse("06/12/2021"));
        }

        [TestMethod]
        public void ParseIPCARateSerieFromJson()
        {
            string[] series = ParseIPCASerie(_ipcaDataAsJson);
            Assert.AreEqual(series.Length, 2);
        }

        [TestMethod]
        public void GetIPCADataFromSerieArray()
        {
            string[] series = ParseIPCASerie(_ipcaDataAsJson);
            BaseRateData ipcaData = GetIPCADataFromSerie(series);
            Assert.IsTrue(ipcaData is IPCAData);
        }

        [TestMethod]
        public void GetCurrenteIPCARateFromJson()
        {
            BaseRateData ipcaData = GetIPCALast12Months(_ipcaDataAsJson);
            Assert.AreEqual(ipcaData.Rate, 10.06);
        }
        [TestMethod]
        public void GetCurrenteIPCADateFromJson()
        {
            BaseRateData ipcaData = GetIPCALast12Months(_ipcaDataAsJson);
            Assert.AreEqual(ipcaData.RateDate, new DateTime(2021, 12, 1));
        }
    }
}
