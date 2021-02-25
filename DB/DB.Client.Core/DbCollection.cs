using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DB.Client.Core.Helpers;
using DB.Client.Core.UpdateDefinitions;
using Newtonsoft.Json.Linq;

namespace DB.Client.Core
{
    internal class DbCollection : IDbCollection
    {
        private readonly string collectionName;
        private readonly IDbRequestSender sender;

        public DbCollection(string collectionName, IDbRequestSender sender)
        {
            this.collectionName = collectionName;
            this.sender = sender;
            Indexes = new DbIndexManager(collectionName, sender);
        }

        public IDbIndexManager Indexes { get; }

        public async Task InsertAsync(string id, Dictionary<string, string> document)
        {
            var command = CommandBuilder.BuildInsert(collectionName, id, document);
            var rawResult = await sender.SendAsync(command).ConfigureAwait(false);
            ResultParser.Parse(rawResult);
        }

        public async Task<Dictionary<string, string>> FindAsync(string id)
        {
            var command = CommandBuilder.BuildFind(collectionName, id);
            var rawResult = await sender.SendAsync(command).ConfigureAwait(false);
            var jContainer = ResultParser.Parse(rawResult);
            return jContainer[id].ToObject<Dictionary<string, string>>();
        }

        public async Task<List<(string Id, Dictionary<string, string> Document)>> FindAsync(string field, string value)
        {
            var command = CommandBuilder.BuildFind(collectionName, field, value);
            var rawResult = await sender.SendAsync(command).ConfigureAwait(false);
            var jContainer = ResultParser.Parse(rawResult);
            return jContainer.Values<JObject>()
                             .Select(jObject => jObject.Properties().Single())
                             .Select(x => (x.Name, x.Value.ToObject<Dictionary<string, string>>()))
                             .ToList();
        }

        public async Task ReplaceAsync(string id, Dictionary<string, string> document, bool upsert = false)
        {
            var command = CommandBuilder.BuildReplace(collectionName, id, document, upsert);
            var rawResult = await sender.SendAsync(command).ConfigureAwait(false);
            ResultParser.Parse(rawResult);
        }

        public async Task UpdateAsync(string id, IUpdateDefinition updateDefinition)
        {
            var command = CommandBuilder.BuildUpdate(collectionName, id, updateDefinition.Render());
            var rawResult = await sender.SendAsync(command).ConfigureAwait(false);
            ResultParser.Parse(rawResult);
        }

        public async Task DeleteAsync(string id)
        {
            var command = CommandBuilder.BuildDelete(collectionName, id);
            var rawResult = await sender.SendAsync(command).ConfigureAwait(false);
            ResultParser.Parse(rawResult);
        }
    }
}