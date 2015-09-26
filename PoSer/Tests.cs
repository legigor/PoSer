﻿using System;
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

        [TestCase("",              Result = 0.00)]
        [TestCase("B",             Result = 4.25)]
        [TestCase("BB",            Result = 8.50)]
        [TestCase("ABCDABA",       Result = 13.25)]
        [TestCase("CCCCCCC",       Result = 6.00)]
        [TestCase("ABCD",          Result = 7.25)]
        [TestCase("EEEEEEEEEEE",   Result = 10.50)]
        public decimal When_scanning(string codes)
        {
            pos.Scan(codes.Select(c => c.ToString()).ToArray());
            return pos.CalculateTotal();
        }
    }
}
