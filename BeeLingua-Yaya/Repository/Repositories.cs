using BeeLingua_Yaya.Models;
using Microsoft.Azure.Documents.Client;
using Nexus.Base.CosmosDBRepository;
using System;
using System.Collections.Generic;
using System.Text;

namespace BeeLingua_Yaya.Repository
{
    public class Repositories
    {
        public class ClassRepository : DocumentDBRepository<Lesson>
        {
            public ClassRepository(DocumentClient client) : base("Course", client, partitionProperties: "LessonCode") { }
        }
    }
}
