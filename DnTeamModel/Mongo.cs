using MongoDB.Driver;

namespace DnTeamData
{
    /// <summary>
    /// Decribes MongoDb object
    /// </summary>
    public static class Mongo
    {
        /// <summary>
        /// Returns MongoDatabase object with default settings
        /// </summary>
        /// <returns>MongoDatabase object</returns>
        static public MongoDatabase Init()
        {
            return Init(Properties.Settings.Default.DatabaseName, 
                Properties.Settings.Default.ConnectionString);
        }

        /// <summary>
        /// Returns MongoDatabase object with defined database and settings
        /// </summary>
        /// <param name="databaseName">Database name</param>
        /// <param name="connectionString">Connection string</param>
        /// <returns></returns>
        static public MongoDatabase Init(string databaseName, string connectionString)
        {
            MongoServer server = MongoServer.Create(connectionString);
            MongoDatabase db = server.GetDatabase(databaseName);

            return db;
        }
    }
}
