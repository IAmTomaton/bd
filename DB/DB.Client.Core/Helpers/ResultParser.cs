using System;
using System.Runtime.ExceptionServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DB.Client.Core.Helpers
{
    internal static class ResultParser
    {
        private static readonly JsonSerializerSettings Settings = new()
        {
            MissingMemberHandling = MissingMemberHandling.Error
        };
        
        public static JContainer Parse(string rawResult)
        {
            try
            {
                var ok = JsonConvert.DeserializeObject<OkResult>(rawResult, Settings);
                return ok.Ok;
            }
            catch (Exception e)
            {
                var exceptionDispatchInfo = ExceptionDispatchInfo.Capture(e);
                string message = null;
                try
                {
                    var error = JsonConvert.DeserializeObject<ErrorResult>(rawResult, Settings);
                    message = error.Error.Message;
                }
                catch
                {
                    exceptionDispatchInfo.Throw();
                }

                throw new DbCommandException(message, e);
            }
        }
        
        private class OkResult
        {
            [JsonProperty("ok")]
            public JContainer Ok { get; set; }
        }
        
        private class ErrorResult
        {
            [JsonProperty("error")]
            public MessageInfo Error { get; set; }
            
            public class MessageInfo
            {
                [JsonProperty("message")]
                public string Message { get; set; }
            }
        }
    }
}