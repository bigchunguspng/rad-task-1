using System;
using Newtonsoft.Json;

namespace RadencyTaskETL.OutputModel
{
    public class PayerData
    {
        [JsonProperty("name")]           public string Name;
        [JsonProperty("payment")]        public decimal Payment;
        [JsonProperty("date")]           public DateOnly Date;
        [JsonProperty("account_number")] public long AccountNumber;
    }
}