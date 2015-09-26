using System;
using System.Linq;

namespace PoSer
{
    public class VolumePrice
    {
        public decimal PriceForVolume { get; }
        public int Volume { get; }

        public decimal PriceForUnit => PriceForVolume / Volume;

        public VolumePrice(decimal priceForVolume, int volume)
        {
            PriceForVolume = priceForVolume;
            Volume = volume;
        }
    }
}