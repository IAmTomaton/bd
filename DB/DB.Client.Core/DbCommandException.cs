using System;

namespace DB.Client.Core
{
    public class DbCommandException : Exception
    {
        public string DbError { get; }
        public Exception SerializationException { get; }

        public DbCommandException(string dbError, Exception serializationException, Exception innerException = null)
            : base($"Error: {dbError}", innerException)
        {
            DbError = dbError;
            SerializationException = serializationException;
        }
    }
}