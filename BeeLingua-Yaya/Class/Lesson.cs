using Newtonsoft.Json;
using Nexus.Base.CosmosDBRepository;
using System;
using System.Collections.Generic;
using System.Text;

namespace BeeLingua_Yaya.Class
{
    public class Lesson : ModelBase
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("lessonCode")]
        public string LessonCode { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }
}
