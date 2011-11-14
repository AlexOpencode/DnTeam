using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DnTeamData.Models
{

    public class Value
    {
        public string Name { get; set; }
    }

    public class Enums
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public List<string> Values { get; set; }

        public Enums()
        {
            Id = ObjectId.GenerateNewId();
        }
    }

    public enum EnumName
    {
        Locations,
        ProjectRoles,
        ProjectStatuses,
        TechnologySpecialties,
        FunctionalSpecialties,
        ProjectTypes,
        ProjectMilestones,
        //ProjectNoiseTypes,
        Undefined
    }
}
