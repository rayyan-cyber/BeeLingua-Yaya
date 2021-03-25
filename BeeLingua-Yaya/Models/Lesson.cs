﻿using Newtonsoft.Json;
using Nexus.Base.CosmosDBRepository;

namespace BeeLingua_Yaya
{
    public class Lesson : ModelBase
    {
        [JsonProperty("lessonCode")]
        public string LessonCode { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }
}
