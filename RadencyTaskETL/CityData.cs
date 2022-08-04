using System.Collections.Generic;
using System.Linq;

namespace RadencyTaskETL
{
    public class CityData
    {
        public string City;
        public List<ServiceData> Services;
        public decimal Total;
        
        public decimal GetTotal() => Services.Sum(x => x.GetTotal());
    }
}