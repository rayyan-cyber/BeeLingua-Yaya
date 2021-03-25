
using Microsoft.Azure.Documents.Client;
using Nexus.Base.CosmosDBRepository;
using System;
using System.Collections.Generic;
using System.Text;

namespace BeeLingua_Yaya.Repository
{
    public class Repositories
    {
        private static readonly string _eventGridEndPoint = Environment.GetEnvironmentVariable("eventGridEndPoint");
        private static readonly string _eventGridKey = Environment.GetEnvironmentVariable("eventGridEndKey");

        public class LessonRepository : DocumentDBRepository<Lesson>
        {
            public LessonRepository(DocumentClient client) : base("Course", client, partitionProperties: "LessonCode",
                eventGridEndPoint: _eventGridEndPoint, eventGridKey: _eventGridKey) { }
        }

        public class NotificationLessonRepository : DocumentDBRepository<NotificationLesson>
        {
            public NotificationLessonRepository(DocumentClient client) : base("Course", client)
            { }
        }
    }
}
