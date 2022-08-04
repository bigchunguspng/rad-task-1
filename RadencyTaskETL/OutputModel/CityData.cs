using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace RadencyTaskETL.OutputModel
{
    public class CityData
    {
        [JsonProperty("city")]     public string City;
        [JsonProperty("services")] public List<ServiceData> Services;
        [JsonProperty("total")]    public decimal Total;
        
        public decimal GetTotal() => Services.Sum(x => x.GetTotal());
    }
}