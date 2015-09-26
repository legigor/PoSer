using System;
using System.Collections.Generic;
using System.Linq;

namespace PoSer
{
    public class PointOfSaleTerminal
    {
        readonly IDictionary<string, ItemTotalCalculator> calculators = new Dictionary<string, ItemTotalCalculator>();

        readonly IList<string> items = new List<string>();

        public void SetPricing(string code, decimal price, params VolumePrice[] volumePrices)
        {
            calculators[code] = new ItemTotalCalculator(price, volumePrices);
        }

        public void Scan(params string[] codes)
        {
            foreach (var code in codes)
            {
                if(calculators.ContainsKey(code) == false)
                    throw new InvalidOperationException($"Product '{code}' not found");
            }

            Array.ForEach(codes, items.Add);
        }

        public decimal CalculateTotal()
        {
            var total = items.Distinct().Sum(code =>
            {
                var calculator = calculators[code];
                var volume = items.Count(c => c == code);

                return calculator.CalculateTotal(volume);
            });
            return total;
        }

        class ItemTotalCalculator
        {
            readonly decimal price;
            readonly VolumePrice[] volumePrices;

            public ItemTotalCalculator(decimal price, params VolumePrice[] volumePrices)
            {
                this.price = price;
                this.volumePrices = volumePrices;
            }

            public decimal CalculateTotal(int volume)
            {
                var weightedPrices = volumePrices
                    .Union(new[] { new VolumePrice(price, 1) })
                    .OrderBy(price => price.PriceForUnit);

                decimal total = 0;
                var v = volume;

                foreach (var wp in weightedPrices)
                {
                    var groups = v / wp.Volume;

                    total += wp.PriceForVolume * groups;
                    v = v - wp.Volume * groups;
                }

                return total;
            }
        }

    }
}