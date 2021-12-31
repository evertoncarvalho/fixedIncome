using System;

namespace TaxRequester
{
    public abstract class BaseTaxData
    {
        public abstract float Tax { get; set; }
        public virtual float GetTaxDaily(float inputTax = 0f)
        {
            if (inputTax == 0f)
            {
                inputTax = Tax;
            }
            return (float)Math.Pow(1 + inputTax / 100, 1f / 252) - 1;
        }
    }
}
