using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace DnTeamData.Models
{
    /// <summary>
    /// Enum class representing Enums collection
    /// </summary> 
    public class Enums
    {
        /// <summary>
        /// Enum name - is used as Id
        /// </summary>
        [BsonId]
        public string Name { get; set; }
        
        /// <summary>
        /// The list of values
        /// </summary>
        public List<string> Values { get; set; }
        
        /// <summary>
        /// Enums constructor  creates empty values list
        /// </summary>
        public Enums()
        {
            Values = new List<string>();
        }
    }

    /// <summary>
    /// Enum containing name of the Enums collections' document
    /// </summary>
    public enum EnumName
    {
        /// <summary>
        /// Locations
        /// </summary>
        Locations,
        /// <summary>
        /// Project Roles
        /// </summary>
        ProjectRoles,
        /// <summary>
        /// Project Statuses
        /// </summary>
        ProjectStatuses,
        /// <summary>
        /// Technology Specialty Names
        /// </summary>
        TechnologySpecialtyNames,
        /// <summary>
        /// Project Types
        /// </summary>
        ProjectTypes,
        /// <summary>
        /// Project Milestones
        /// </summary>
        ProjectMilestones,
        /// <summary>
        /// Project Noise Types
        /// </summary>
        ProjectNoiseTypes,
        /// <summary>
        /// Project Priority Types
        /// </summary>
        ProjectPriorityTypes,
        /// <summary>
        /// Technology Specialty Levels
        /// </summary>
        TechnologySpecialtyLevels,
        /// <summary>
        /// Undefined enum value
        /// </summary>
        Undefined
    }
}
