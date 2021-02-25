using Newtonsoft.Json.Linq;

namespace DB.Core.Helpers
{
    public static class Result
    {
        public static class Error
        {
            public static readonly JObject AlreadyExists = WithMessage("already exists");
            public static readonly JObject NotFound = WithMessage("not found");
            public static readonly JObject CommandNotFound = WithMessage("invalid command");
            public static readonly JObject InvalidRequest = WithMessage("invalid request");
        
            public static JObject WithMessage(string message)
                => JObject.FromObject(new {error = new {message}});
        }
        
        public static class Ok
        {
            public static readonly JObject Empty = WithContent(new JObject());

            public static JObject WithContent(object content) => new(new JProperty("ok", content));
        }
    }
}