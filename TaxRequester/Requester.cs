using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;

namespace TaxRequester
{
    public class Requester
    {
        //"https://www2.cetip.com.br/ConsultarTaxaDi/ConsultarTaxaDICetip.aspx"
        //"https://servicodados.ibge.gov.br/api/v3/agregados/portal?view=object"
        private float GetIPCA()
        {
            return 0f;
        }

        public CDIData GetCDI()
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
                using (StreamReader reader = new StreamReader(client.OpenRead(app.Default.CDIEndpoint)))
                {
                    string cdi = reader.ReadToEnd();
                    return JsonSerializer.Deserialize<CDIData>(cdi);
                }
            }
        }

        private bool AllowCertificate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors error)
        {
            return true;
        }
    }
}
