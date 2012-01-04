using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DnTeamData.Models
{
    /// <summary>
    /// A class describing Client
    /// </summary>
    public class Client
    {
        /// <summary>
        /// Client Id
        /// </summary>
        [BsonId]
        public ObjectId Id { get; set; }
        
        /// <summary>
        /// Client Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Overrides ToString() to return Client Id
        /// </summary>
        /// <returns>String value of Client Id</returns>
        public override string ToString()
        {
            return Id.ToString();
        }

        /// <summary>
        /// Generates Client Id while creation a new client
        /// </summary>
        public Client()
        {
            Id = ObjectId.GenerateNewId();
        }
    }

    /// <summary>
    /// Enum contains ClientEditStatus
    /// </summary>
    public enum ClientEditStatus
    {
        /// <summary>
        /// Transaction copleted successfuly
        /// </summary>
        /// 
        Ok,
        /// <summary>
        /// Error occured: item with such index already exist
        /// </summary>
        DuplicateItem,
        /// <summary>
        /// Error occured: name value is empty
        /// </summary>
        NameIsEmpty
    }
}
