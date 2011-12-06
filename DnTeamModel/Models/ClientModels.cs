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
}
