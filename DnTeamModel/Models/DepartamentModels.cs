using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DnTeamData.Models
{
    /// <summary>
    /// A class describing Department
    /// </summary>
    public class Department : IEquatable<Department>
    {
        /// <summary>
        /// Department id
        /// </summary>
        [BsonId]
        public ObjectId Id { get; set; }
        
        /// <summary>
        /// Department name
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Parent department id
        /// </summary>
        public ObjectId DepartmentOf { get; set; }
        
        /// <summary>
        /// Parent department description (Location, Name)
        /// </summary>
        public string ParentDepartment { get; set; }
        
        /// <summary>
        /// Department location
        /// </summary>
        public string Location { get; set; }
        
        /// <summary>
        /// Base cost
        /// </summary>
        public decimal Cost { get; set; }
        
        /// <summary>
        /// Base rate
        /// </summary>
        public decimal Rate { get; set; }

        /// <summary>
        /// Override of ToSting method, returning department id
        /// </summary>
        /// <returns>String of department id</returns>
        public override string ToString()
        {
            return Id.ToString();
        }

        public bool Equals(Department other)
        {
            return Id == other.Id && Name == other.Name && DepartmentOf == other.DepartmentOf && Location == other.Location && Cost == other.Cost && Rate == other.Rate;
        }
    }

    /// <summary>
    /// Decribes options of departments editing
    /// </summary>
    public enum DepartmentEditStatus
    {
        /// <summary>
        /// Department was edited successfuly
        /// </summary>
        Ok,
        /// <summary>
        /// Department is duplicate
        /// </summary>
        ErrorDuplicate,
        /// <summary>
        /// Parent department was defined incorectly
        /// </summary>
        ErrorParentUndefined,
        /// <summary>
        /// Department name was empty
        /// </summary>
        ErrorNameIsEmpty,
    }
}
