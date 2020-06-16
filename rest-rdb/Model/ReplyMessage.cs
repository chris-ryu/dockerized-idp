using System.Text.Json.Serialization;
namespace webapi.Model
{
   public class ReplyMessage
   {
       [JsonPropertyName("Reply-Message")]
        public string Message{get;set;}
   } 
}