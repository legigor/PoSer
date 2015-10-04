using System;
using System.Linq;

namespace PoSer
{
    public class Total
    {
        public decimal Net   { get; }
        public decimal Gross { get; }
        
        public Total(decimal net, decimal gross)
        {
            Net = net;
            Gross = gross;
        }
    }
}