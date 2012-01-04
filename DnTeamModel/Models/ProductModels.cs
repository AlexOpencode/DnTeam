using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DnTeamData.Models
{
    /// <summary>
    /// A class describing Product
    /// </summary>
    public class Product
    {
        /// <summary>
        /// Product Id
        /// </summary>
        [BsonId]
        public ObjectId Id { get; set; }

        /// <summary>
        /// Product Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Linked Client
        /// </summary>
        public ObjectId ClientId { get; set; }

        /// <summary>
        /// Client Name
        /// </summary>
        /// <returns></returns>
        public string ClientName ()
        {
            return ClientRepository.GetName(ClientId);
        }

        /// <summary>
        /// Overrides ToString() to return Product Id
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Id.ToString(); 
        }

        /// <summary>
        /// Generates Product Id while creation a new client
        /// </summary>
        public Product()
        {
            Id = ObjectId.GenerateNewId();
        }
    }

    /// <summary>
    /// Enum contains ProductEditstatuses
    /// </summary>
    public enum ProductEditStatus
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
        /// Error occured: client value is invalid
        /// </summary>
        ClientIsInvalid,
        /// <summary>
        /// Error occured: name value is empty
        /// </summary>
        NameIsEmpty
    }
}
