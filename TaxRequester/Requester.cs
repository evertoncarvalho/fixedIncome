using Newtonsoft.Json;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace TaxRequester
{
    public class Requester
    {
        public CDIData GetCDI()
        {
            string responseContent = RequestData(app.Default.CDIEndpoint);
            return GetCDI(responseContent);
        }

        protected CDIData GetCDI(string cdiAsJson)
        {
            return JsonConvert.DeserializeObject<CDIData>(cdiAsJson);
        }

        public IPCAData? GetIPCALast12Months()
        {
            string responseContent = RequestData(app.Default.IPCAEndpoint);
            return !string.IsNullOrWhiteSpace(responseContent)
                ? GetIPCALast12Months(responseContent)
                : null;
        }

        protected IPCAData GetIPCALast12Months(string ipcaAsJson)
        {
            string[] ipcaSerie = ParseIPCASerie(ipcaAsJson);
            return GetIPCADataFromSerie(ipcaSerie);
        }

        protected IPCAData GetIPCADataFromSerie(string[] ipcaSerie)
        {
            IPCAData ipcaData = new IPCAData();
            foreach (string item in ipcaSerie)
            {
                string[] parts = item.Split(':');
                DateTime aux = DateTime.ParseExact(parts[0], "yyyyMM", null);
                if (ipcaData.RateDate < aux)
                {
                    ipcaData.RateDate = aux;
                    ipcaData.Rate = double.Parse(parts[1].Replace(".", ","));
                }
            }
            return ipcaData;
        }

        protected string[] ParseIPCASerie(string resposeFromIBGE)
        {
            string ipca12Id = "1737";
            string ipcaSerieId = "serie\":{";
            string ipca12 = resposeFromIBGE.Substring(resposeFromIBGE.IndexOf(ipca12Id));
            ipca12 = ipca12.Substring(ipca12.IndexOf(ipcaSerieId));
            ipca12 = ipca12.Substring(ipcaSerieId.Length, ipca12.IndexOf("}") - ipcaSerieId.Length);
            ipca12 = ipca12.Replace("\"", "");
            return ipca12.Split(',');
        }

        private string RequestData(string endpoint)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback += AllowCertificate;
            using (WebClient client = new WebClient())
            {
                client.Headers.Add(
                    "user-agent",
                    "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                client.Headers.Add("Content-type:text/html; charset=utf-8");
                try
                {
                    using (StreamReader reader = new StreamReader(client.OpenRead(endpoint)))
                    {
                        return reader.ReadToEnd();
                    }
                }
                catch (WebException)
                {
                    //TODO log
                    return string.Empty;
                }
            }
        }

        private bool AllowCertificate(
            object sender,
            X509Certificate cert,
            X509Chain chain,
            SslPolicyErrors error)
        {
            return true;
        }
    }
}
