namespace DB.Client.Core.UpdateDefinitions
{
    internal class UnsetUpdateDefinition : IUpdateDefinition
    {
        private readonly string field;

        public UnsetUpdateDefinition(string field)
            => this.field = field;

        public string Render()
            => $@"{{""unset"":""{field}""}}";
    }
}