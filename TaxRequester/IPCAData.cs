
namespace TaxRequester
{
    /*"agregado":"1737",
     * "id":"2265",
     * "variavel":"IPCA - Variação acumulada em 12 meses",
     * "unidade":"%",
     * "resultados":[
     * { "classificacoes":[ ],
     *  "series":[
     *  {
     *  "localidade":{
     *  "id":"1",
     *  "nivel":{
     *  "id":"N1",
     *  "nome":"Brasil"
     *  },
     *  "nome":"Brasil"
     *  },
     *  "serie":{
     *  "202109":"10.25",
     *  "202110":"10.67"
     *  }
     */
    public class IPCAData
        : CDIData
    {
        public override string AsString
        {
            get
            {
                return "ipca acumulado em 12 meses " + Tax
                    + " data " + TaxDate;
            }
        }
    }
}
