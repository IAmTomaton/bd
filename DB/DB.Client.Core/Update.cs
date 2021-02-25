using DB.Client.Core.UpdateDefinitions;

namespace DB.Client.Core
{
    public static class Update
    {
        public static IUpdateDefinition Set(string field, string value)
            => new CombineUpdateDefinition(new SetUpdateDefinition(field, value));

        public static IUpdateDefinition Unset(string field)
            => new CombineUpdateDefinition(new UnsetUpdateDefinition(field));
        
        public static IUpdateDefinition Set(this IUpdateDefinition source, string field, string value)
            => new CombineUpdateDefinition(source, new SetUpdateDefinition(field, value));

        public static IUpdateDefinition Unset(this IUpdateDefinition source, string field)
            => new CombineUpdateDefinition(source, new UnsetUpdateDefinition(field));
    }
}