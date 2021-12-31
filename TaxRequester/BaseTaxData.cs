using System;

namespace TaxRequester
{
    public abstract class BaseTaxData
    {
        public abstract double Tax { get; set; }
        public virtual double GetTaxDaily(double inputTax = 0f)
        {
            if (inputTax == 0f)
            {
                inputTax = Tax;
            }
            return Math.Pow(1 + inputTax / 100, 1f / 252) - 1;
        }
    }
}
