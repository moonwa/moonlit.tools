using System.Data.SqlClient;
using System.Data.SqlServerCe;
using System.IO;

namespace Moonlit.Tools.Sql
{
    internal abstract class SqlCommandBase
    {
        [Parameter("conn", Description = "存储文件名", Prefixs = "n", Required = true)]
        public string ConnectionString { get; set; }
        public SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }
    }
}