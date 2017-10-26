using System;
using System.Collections.Generic;
using System.Text;

namespace NugetVisualizer.Core.Repositories
{
    using System.Data.Common;
    using System.Threading.Tasks;

    public static class SqlHelper
    {
        public delegate void ProcessReader<TReturnType>(DbDataReader reader, TReturnType result);

        public static async Task<TReturnType> GetFromSql<TReturnType>(this INugetVisualizerContext context, string query, ProcessReader<TReturnType> readerProcess) where TReturnType : new()
        {
            var result = new TReturnType();
            var conn = context.GetDbConnection();
            try
            {
                await conn.OpenAsync();
                using (var command = conn.CreateCommand())
                {
                    command.CommandText = query;
                    var reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            readerProcess(reader, result);
                        }
                    }
                    reader.Dispose();
                }
            }
            finally
            {
                conn.Close();
            }
            return result;
        }
    }
}
