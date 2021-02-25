namespace DB.Client.Core.UpdateDefinitions
{
    internal class SetUpdateDefinition : IUpdateDefinition
    {
        private readonly string field;
        private readonly string value;

        public SetUpdateDefinition(string field, string value)
        {
            this.field = field;
            this.value = value;
        }
        
        public string Render()
            => $@"{{""set"":{{""{field}"":""{value}""}}}}";
    }
}