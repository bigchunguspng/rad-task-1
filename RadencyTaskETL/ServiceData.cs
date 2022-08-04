using System.Collections.Generic;
using System.Linq;

namespace RadencyTaskETL
{
    public class ServiceData
    {
        public string Name;
        public List<PayerData> Payers;
        public decimal Total;

        public decimal GetTotal() => Payers.Sum(x => x.Payment);
    }
}