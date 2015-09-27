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

                var seed = new AccumulatorRecord { Total = 0m, Volume = volume };

                var res = weightedPrices.Aggregate(
                    seed, 
                    (acc, wp) =>
                    {
                        var groups = acc.Volume / wp.Volume;

                        var total = acc.Total + wp.PriceForVolume * groups;
                        var newVolume = acc.Volume - wp.Volume * groups;
                    
                        return new AccumulatorRecord { Total = total, Volume = newVolume };
                    }, 
                    x => x.Total
                );

                return res;
            }

            class AccumulatorRecord // Kind of a tuple like record type
            {
                public decimal Total;
                public int Volume;
            }
        }

    }
}