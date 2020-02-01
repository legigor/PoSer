using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

namespace PoSer
{
    public class PoSTests
    {
        PointOfSaleTerminal pos;

        [SetUp]
        public void SetUp()
        {
            pos = new PointOfSaleTerminal();

            pos.SetPricing("A", 1.25m, new VolumePrice(3m, 3));
            pos.SetPricing("B", 4.25m);
            pos.SetPricing("C", 1.00m, new VolumePrice(5m, 6));
            pos.SetPricing("D", 0.75m);
            pos.SetPricing("E", 1.25m, new VolumePrice(5m, 6), new VolumePrice(3m, 3));
        }

        [Test]
        public void When_scaning_not_existing_code()
        {
            Assert.Throws<InvalidOperationException>(() => pos.Scan("ABCDEZ"));
        }

        [TestCase("",              ExpectedResult = 0.00)]
        [TestCase("B",             ExpectedResult = 4.25)]
        [TestCase("BB",            ExpectedResult = 8.50)]
        [TestCase("ABCDABA",       ExpectedResult = 13.25)]
        [TestCase("CCCCCCC",       ExpectedResult = 6.00)]
        [TestCase("ABCD",          ExpectedResult = 7.25)]
        [TestCase("EEEEEEEEEEE",   ExpectedResult = 10.50)]
        public decimal When_calculating_voume_discount(string codes)
        {
            pos.Scan(codes.Select(c => c.ToString()).ToArray());
            return pos.CalculateTotal().Gross;
        }

        [TestCase("",              ExpectedResult = 0.00)]
        [TestCase("B",             ExpectedResult = 4.25)]
        [TestCase("BB",            ExpectedResult = 8.50)]
        [TestCase("ABCDABA",       ExpectedResult = 14.00)]
        [TestCase("CCCCCCC",       ExpectedResult = 7.00)]
        [TestCase("ABCD",          ExpectedResult = 7.25)]
        [TestCase("EEEEEEEEEEE",   ExpectedResult = 13.75)]
        public decimal When_calculating_net_for_saving_to_the_customers_ballance(string codes)
        {
            pos.Scan(codes.Select(c => c.ToString()).ToArray());
            return pos.CalculateTotal().Net;
        }

        [TestCase("",              ExpectedResult = 0.00)]
        [TestCase("B",             ExpectedResult = 4.04)]
        [TestCase("BB",            ExpectedResult = 8.08)]
        [TestCase("ABCDABA",       ExpectedResult = 12.74)]
        [TestCase("CCCCCCC",       ExpectedResult = 6.00)]
        [TestCase("ABCD",          ExpectedResult = 6.89)]
        [TestCase("EEEEEEEEEEE",   ExpectedResult = 10.50)]
        public decimal When_calculating_total_discount(string codes)
        {
            pos.Scan(codes.Select(c => c.ToString()).ToArray());
            pos.SetTotalDiscount(0.05);

            return pos.CalculateTotal().Gross;
        }
    }
}
