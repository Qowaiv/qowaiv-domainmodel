using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EventStorage.SqlServer
{
    internal static class Sql
    {
        internal static string CreateTableEvents => FromEmbedded("CreateTableEvents.sql");
        internal static string SelectCurrentVersion => FromEmbedded("SelectCurrentVersion.sql");
        internal static string SelectEvents => FromEmbedded("SelectEvents.sql");

        private static string FromEmbedded(string path)
        {
            using var stream = typeof(SqlServerEventStore<>).Assembly.GetManifestResourceStream("EventStorage.SqlServer.Sql." + path);
            var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}
