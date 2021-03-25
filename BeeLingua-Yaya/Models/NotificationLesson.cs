using Newtonsoft.Json;
using Nexus.Base.CosmosDBRepository;
using System;
using System.Collections.Generic;
using System.Text;

namespace BeeLingua_Yaya
{
    public class NotificationLesson : ModelBase
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("subject")]
        public string Subject { get; set; }

        [JsonProperty("data")]
        //public Dictionary<string, string> Data { get; set; }
        //public string Data { get; set; }
        public object Data { get; set; }
        [JsonProperty("eventType")]
        public string EventType { get; set; }

        [JsonProperty("eventTime")]
        public DateTime EventTime { get; set; }

        [JsonProperty("topic")]
        public string Topic { get; set; }

        [JsonProperty("messageBodyEvent")]
        public object MessageBodyEvent { get; internal set; }
    }
}
