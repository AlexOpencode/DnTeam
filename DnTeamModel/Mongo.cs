using MongoDB.Driver;

namespace DnTeamData
{
    public static class Mongo
    {

        static public MongoDatabase Init()
        {
            return Init(Properties.Settings.Default.DatabaseName, 
                Properties.Settings.Default.ConnectionString);
        }

        static public MongoDatabase Init(string databaseName, string connectionString)
        {
            MongoServer server = MongoServer.Create(connectionString);
            MongoDatabase db = server.GetDatabase(databaseName);

            return db;
        }
    }
}
