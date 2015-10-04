using System;
using System.Collections.Generic;
using System.Linq;

namespace PoSer
{
    public class PointOfSaleTerminal
    {
        readonly IDictionary<string, ItemTotalCalculator> calculators = new Dictionary<string, ItemTotalCalculator>();
        readonly IList<string> items = new List<string>();
        double discount;

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

        public Total CalculateTotal()
        {
            var totals = items.Distinct().Select(code =>
            {
                var calculator = calculators[code];
                var volume = items.Count(c => c == code);

                var applyDiscount = calculator
                    .CalculateTotal(volume);
                return ApplyDiscountIfNothingAppliedYet(applyDiscount, discount);
            });
            return totals.Aggregate(new Total(0, 0), (acc, total) => new Total(acc.Net + total.Net, acc.Gross + total.Gross));
        }

        static Total ApplyDiscountIfNothingAppliedYet(Total total, double discount)
        {
            var adjustedGross = total.Gross != total.Net
                                ? total.Gross
                                : total.Net - total.Net * (decimal)discount;

            return new Total(total.Net, Math.Round(adjustedGross, 2));
        }

        public void SetTotalDiscount(double discount)
        {
            this.discount = discount;
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

            public Total CalculateTotal(int volume)
            {
                var weightedPrices = volumePrices
                    .Union(new[] { new VolumePrice(price, 1) })
                    .OrderBy(price => price.PriceForUnit);

                var seed = new AccumulatorRecord { Total = 0m, Volume = volume };

                var gross = weightedPrices.Aggregate(
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

                return new Total(volume * price, gross); ;
            }

            class AccumulatorRecord // Kind of a tuple like record type
            {
                public decimal Total;
                public int Volume;
            }
        }

    }
}