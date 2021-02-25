using System.Linq;

namespace DB.Client.Core.UpdateDefinitions
{
    internal class CombineUpdateDefinition : IUpdateDefinition
    {
        private readonly IUpdateDefinition[] updateDefinitions;

        public CombineUpdateDefinition(params IUpdateDefinition[] updateDefinitions)
            => this.updateDefinitions = updateDefinitions
                .SelectMany(d => d is CombineUpdateDefinition c ? c.updateDefinitions : new[] {d})
                .ToArray();

        public string Render()
            => $"[{string.Join(",", updateDefinitions.Select(d => d.Render()))}]";
    }
}