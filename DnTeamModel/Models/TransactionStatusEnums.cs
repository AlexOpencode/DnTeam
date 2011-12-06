namespace DnTeamData.Models
{
    /// <summary>
    /// Enum contains DnTeamData transaction statuses
    /// </summary>
    public enum TransactionStatus
    {
        /// <summary>
        /// MongoSafeModeException occured and it is updefined
        /// </summary>
        UndefinedMongoSafeModeException,

        /// <summary>
        /// Transaction copleted successfuly
        /// </summary>
        /// 
        Ok,
        /// <summary>
        /// Error occured: duplicate Name
        /// </summary>
        DuplicateName
    }
}
