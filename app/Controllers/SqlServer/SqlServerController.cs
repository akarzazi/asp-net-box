using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;

using Microsoft.AspNetCore.Mvc;

namespace AspNetBox.Controllers
{
    /// <summary>
    /// Test connection to SQL Server
    /// </summary>
    [Route("[controller]")]
    [Produces("text/plain")]
    public class SqlServerController : ControllerBase
    {
        private const string DEFAULT_CNX_STR = "Server=host.docker.internal,1434;Database=AdventureWorksLT2017;User Id=sa;Password=your_123strOng_password;";
        private const string SQL = "Select SYSUTCDATETIME()";

        /// <summary>
        /// Executes a query on SQL Server using System.Data.SqlClient 
        /// </summary>
        /// <param name="connectionString">The connection string</param>
        /// <param name="sql">SQL query</param>
        /// <returns>Returns the query results</returns>
        [HttpGet("/System")]
        public string ExecuteQuerySystem(string connectionString= DEFAULT_CNX_STR, string sql = SQL)
        {
            using (var conn = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                var cmd = new System.Data.SqlClient.SqlCommand(sql, conn);
                conn.Open();

                using var reader = cmd.ExecuteReader();

                return JsonSerializer.Serialize(ToKeyValues(reader), new JsonSerializerOptions() { WriteIndented = true });
            }
        }

        /// <summary>
        /// Executes a query on SQL Server using Microsoft.Data.SqlClient 
        /// </summary>
        /// <param name="connectionString">The connection string</param>
        /// <param name="sql">SQL query</param>
        /// <returns>Returns the query results</returns>
        [HttpGet("/Microsoft")]
        public string ExecuteQueryMicrosoft(string connectionString = DEFAULT_CNX_STR, string sql = SQL)
        {
            using (var conn = new Microsoft.Data.SqlClient.SqlConnection(connectionString))
            {
                var cmd = new Microsoft.Data.SqlClient.SqlCommand(sql, conn);
                conn.Open();

                using var reader = cmd.ExecuteReader();

                return JsonSerializer.Serialize(ToKeyValues(reader), new JsonSerializerOptions() { WriteIndented = true });
            }
        }

        private List<IEnumerable<KeyValuePair<string, object>>> ToKeyValues(System.Data.SqlClient.SqlDataReader reader)
        {
            var results = new List<IEnumerable<KeyValuePair<string, object>>>();
            var cols = new List<string>();
            for (var i = 0; i < reader.FieldCount; i++)
                cols.Add(reader.GetName(i));

            while (reader.Read())
            {
                results.Add(cols.Select((p, i) => KeyValuePair.Create(p, reader[i])).ToArray());
            }

            return results;
        }

        private List<IEnumerable<KeyValuePair<string, object>>> ToKeyValues(Microsoft.Data.SqlClient.SqlDataReader reader)
        {
            var results = new List<IEnumerable<KeyValuePair<string, object>>>();
            var cols = new List<string>();
            for (var i = 0; i < reader.FieldCount; i++)
                cols.Add(reader.GetName(i));

            while (reader.Read())
            {
                results.Add(cols.Select((p, i) => KeyValuePair.Create(p, reader[i])).ToArray());
            }

            return results;
        }
    }
}
