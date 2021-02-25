using System.Collections.Generic;
using Newtonsoft.Json;

namespace DB.Client.Core.Helpers
{
    internal static class CommandBuilder
    {
        public static string Build(string commandName, string parameters = null)
            => $@"{{""{commandName}"":{parameters ?? "{}"}}}";

        public static string BuildInsert(string collection, string id, Dictionary<string, string> document)
            => Build("insert", @$"{{""{collection}"":{{""{id}"":{JsonConvert.SerializeObject(document)}}}}}");

        public static string BuildReplace(string collection, string id, Dictionary<string, string> document, bool upsert)
            => Build("replace", @$"{{""{collection}"":{{""{id}"":{JsonConvert.SerializeObject(document)}}},""upsert"":""{upsert.ToString().ToLower()}""}}");

        public static string BuildFind(string collection, string id)
            => Build("find", @$"{{""{collection}"":""{id}""}}");

        public static string BuildFind(string collection, string field, string value)
            => Build("find", @$"{{""{collection}"":{{""{field}"":""{value}""}}}}");

        public static string BuildDelete(string collection, string id)
            => Build("delete", @$"{{""{collection}"":""{id}""}}");

        public static string BuildAddIndex(string collection, string field)
            => Build("addIndex", @$"{{""{collection}"":""{field}""}}");

        public static string BuildDropIndex(string collection, string field)
            => Build("dropIndex", @$"{{""{collection}"":""{field}""}}");
        
        public static string BuildUpdate(string collection, string id, string updateDefinition)
            => Build("update", @$"{{""{collection}"":{{""{id}"":{updateDefinition}}}}}");
    }
}