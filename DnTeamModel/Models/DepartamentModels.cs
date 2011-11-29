using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DnTeamData.Models
{
    //public class Department
    //{
    //    [BsonId]
    //    public ObjectId Id { get; set; }
    //    public string DepartmentId { get { return Id.ToString(); } }
    //    public string Name { get; set; }
    //    public List<Subsidiary> Subsidiaries { get; set; }
    //    public ObjectId DepartmentOf { get; set; }
    //    public string DepartmentOfId { get { return (DepartmentOf == ObjectId.Empty) ? string.Empty : DepartmentOf.ToString(); } }

    //    public Department()
    //    {
    //        Id = ObjectId.GenerateNewId();
    //        Subsidiaries = new List<Subsidiary>();
    //    }
        
    //}

    //public class Subsidiary
    //{
    //    [BsonId]
    //    public ObjectId Id { get; set; }
    //    public string SubsidiaryId { get { return Id.ToString(); } }
    //    public string Location { get; set; }
    //    public decimal BaseCost { get; set; }
    //    public decimal BaseRate { get; set; }

    //    public Subsidiary()
    //    {
    //        Id = ObjectId.GenerateNewId();
    //    }
    //} 
    
    //public class TDepartment
    //{
    //    public string Id { get; set; }
    //    public string Name { get; set; }
    //    public List<Subsidiary> Subsidiaries { get; set; }
    //    public List<TDepartment> SubDepartments { get; set; }
        
    //    public TDepartment()
    //    {
    //        SubDepartments = new List<TDepartment>();
    //    }
    //}

    //public class DepartmentId
    //{
    //    public string Name { get; set; }
    //    public string Location { get; set; }
    //}


    //public class Department
    //{
    //    [BsonId]
    //    public DepartmentId Id { get; set; }
    //    public string DepartmentId { get { return Id.ToString(); } }
    //    public DepartmentId DepartmentOf { get; set; }
    //    public string DepartmentOfId { get { return DepartmentOf.ToString(); } }
    //    public decimal Cost { get; set; }
    //    public decimal Rate { get; set; }

    //    public Department()
    //    {
    //        Id = ObjectId.GenerateNewId();

    //    }
    //}

    public class Department
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string DepartmentId { get { return Id.ToString(); } }
        public string Name { get; set; }
        public ObjectId DepartmentOf { get; set; }
        public string DepartmentOfId { get { return (DepartmentOf == ObjectId.Empty) ? string.Empty : DepartmentOf.ToString(); } }
        public string ParentDepartment { get; set; }
        public string Location { get; set; }
        public decimal Cost { get; set; }
        public decimal Rate { get; set; }

        //public Department()
        //{
        //    Id = ObjectId.GenerateNewId();

        //}
    }

    public enum DepartmentEditStatus
    {
        Created,
        ErrorDuplicate,
        ErrorUndefined,
        
    }
}
