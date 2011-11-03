using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DnTeamData.Models
{
    public class Departament
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public List<Subsidary> Subsidaries { get; set; }
        public ObjectId DepartamentOf { get; set; }
        public bool IsDeleted { get; set; }

        public Departament()
        {
            Id = ObjectId.GenerateNewId();
            Subsidaries = new List<Subsidary>();
            IsDeleted = false;
        }
    }

    public class TDepartament
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<Subsidary> Subsidaries { get; set; }
        public List<TDepartament> SubDepartaments { get; set; }
        
        public TDepartament()
        {
            SubDepartaments = new List<TDepartament>();
        }
    }

    public class Subsidary
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string SubsidaryId { get { return Id.ToString(); } }
        public string Location { get; set; }
        public float BaseCost { get; set; }
        public float BaseRate { get; set; }
        public bool IsDeleted { get; set; }

        public Subsidary()
        {
            Id = ObjectId.GenerateNewId();
            IsDeleted = false;
        }
    }
}
