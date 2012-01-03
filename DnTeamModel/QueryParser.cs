using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System.Collections.Generic;

namespace DnTeamData
{
    public static class QueryParser
    {
        public static QueryComplete Parse(string name, string query)
        {
            return Query.And(
                query.Replace('$', ' ').Replace('.', ' ').Replace(',', ' ').Split(' ')
                .Select(o => Query.Matches(name, new BsonRegularExpression(string.Format("/^{0}/i", o)))).Cast<IMongoQuery>().ToArray());
        }

        public static void ParseList(IEnumerable<string> filterQuery, QueryComplete andQuery, out QueryComplete totalQuery)
        {
            if (filterQuery == null || filterQuery.Count() <= 0)
            {
                totalQuery = andQuery;
                return;
            }

            var andQueryList = new List<IMongoQuery>();
            foreach (var filter in filterQuery)
            {
                var v = filter.Split('~');
                andQueryList.Add(Query.Or(Parse(v[0], v[1])));
            }
            andQueryList.Add(andQuery);
            totalQuery = Query.And(andQueryList.ToArray());
        }
    }
}
