using DB.Core.State;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace DB.Core.Commands.Update
{
    public class SetCommandExecutor : IUpdateCommandExecutor
    {
        public bool CanExecute(JToken parameters) =>
            parameters is JObject { Count: 1 } jObjectCommand &&
            jObjectCommand.Properties().Single().Name == "set" &&
            jObjectCommand.Properties().Single().Value is JObject { Count: 1 } jObject &&
            jObject.Properties().Single().Value.Type == JTokenType.String;

        public void Execute(IDbState state, string collectionName, string id, ConcurrentDictionary<string, string> document, JToken parameters)
        {
            var commandProperty = ((JObject)parameters).Properties().Single();
            var bodyProperty = ((JObject)commandProperty.Value).Properties().Single();

            var field = bodyProperty.Name;
            var value = bodyProperty.Value.ToObject<string>();

            document[field] = value;
        }
    }
}
