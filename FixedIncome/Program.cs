using System;

namespace FixedIncome
{
    class Program
    {
        static void Main(string[] args)
        {
            TaxRequester.CDIData cdi = new TaxRequester.Requester().GetCDI();
            Console.WriteLine("cdi " + cdi.Tax
                + " cdi date " + cdi.TaxDate
                + " index " + cdi.Index
                + " index date" + cdi.IndexDate);
        }
    }
}
