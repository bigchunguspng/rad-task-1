using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace RadencyTaskETL.OutputModel
{
    public class ServiceData
    {
        [JsonProperty("name")]   public string Name;
        [JsonProperty("payers")] public List<PayerData> Payers;
        [JsonProperty("total")]  public decimal Total;

        public decimal GetTotal() => Payers.Sum(x => x.Payment);
    }
}