using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DnTeamData.Models
{
    public class Department
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public List<Subsidiary> Subsidiaries { get; set; }
        public ObjectId DepartmentOf { get; set; }
        public bool IsDeleted { get; set; }

        public Department()
        {
            Id = ObjectId.GenerateNewId();
            Subsidiaries = new List<Subsidiary>();
            IsDeleted = false;
        }
    }

    public class TDepartment
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<Subsidiary> Subsidiaries { get; set; }
        public List<TDepartment> SubDepartments { get; set; }
        
        public TDepartment()
        {
            SubDepartments = new List<TDepartment>();
        }
    }

    public class Subsidiary
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string SubsidiaryId { get { return Id.ToString(); } }
        public string Location { get; set; }
        public float BaseCost { get; set; }
        public float BaseRate { get; set; }
        public bool IsDeleted { get; set; }

        public Subsidiary()
        {
            Id = ObjectId.GenerateNewId();
            IsDeleted = false;
        }
    }
}
