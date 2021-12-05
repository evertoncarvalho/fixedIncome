using System;

namespace FixedIncome
{
    class Program
    {
        static void Main(string[] args)
        {
            TaxRequester.Requester requester = new TaxRequester.Requester();
            TaxRequester.CDIData cdi = requester.GetCDI();
            Console.WriteLine(cdi.AsString);
            Console.WriteLine(requester.GetIPCALast12Months().AsString);
        }
    }
}
