namespace DnTeamData.Models
{
    /// <summary>
    /// Enum contains DnTeamData transaction statuses
    /// </summary>
    public enum TransactionStatus
    {
        /// <summary>
        /// Transaction copleted successfuly
        /// </summary>
        /// 
        Ok,
        /// <summary>
        /// Error occured: item with such index already exist
        /// </summary>
        DuplicateItem
    }
}
