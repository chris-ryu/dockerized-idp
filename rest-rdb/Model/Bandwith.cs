
using System.Text.Json.Serialization;
namespace webapi.Model
{
   public class WISPrBandwidth
   {
       [JsonPropertyName("WISPr-Bandwidth-Max-Down")]
        public int WISPrBandwidthMaxDown{get;set;}
       [JsonPropertyName("WISPr-Bandwidth-Max-Up")]
        public int WISPrBandwidthMaxUp{get;set;}
   } 
}