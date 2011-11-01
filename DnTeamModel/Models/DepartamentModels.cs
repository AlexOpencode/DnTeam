using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DnTeamData.Models
{
    class Departament
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public List<Subsidary> Subsidaries { get; set; }
        public ObjectId DepartamentOf { get; set; }

        Departament()
        {
            Id = ObjectId.GenerateNewId();
            Subsidaries = new List<Subsidary>();
        }
    }

    class Subsidary
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string Location { get; set; }
        public float BaseCost { get; set; }
        public float BaseRate { get; set; }

        Subsidary()
        {
            Id = ObjectId.GenerateNewId();
        }
    }
}
