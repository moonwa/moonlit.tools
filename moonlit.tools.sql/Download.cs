using System.Data;
using System.Data.SqlClient;
using System.Data.SqlServerCe;

namespace Moonlit.Tools.Sql
{
    /// <summary>
    /// 命令 : Repair
    /// </summary>
    [Function("Download data")]
    [Version(0, 1, Feature = "实现基本功能")]
    [CommandUsage("Repair")]
    [Command("Download")]
    internal class Download : SqlCommandBase, ICommand
    {
        public int Execute()
        {
            using (var conn = this.GetConnection())
            {
                conn.Open();
                DataTable dt = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(this.Command, conn);
                adapter.SelectCommand.CommandType = this.CommandType;
                adapter.Fill(dt);
                dt.TableName = "tmp";
                dt.WriteXml(this.SaveTo);
                dt.WriteXmlSchema(this.SaveTo + ".schema");
            }
            return 0;
        }
        [Parameter("cmd", Description = "执行命令 sql / sp", Prefixs = "c", Required = true)]
        public string Command { get; set; }
        [Target(0, Name = "保存到", Required = true)]
        public string SaveTo { get; set; }

        [Parameter("cmdType", Description = "执行命令类型", Prefixs = "c", Required = true)]
        public CommandType CommandType { get; set; }
        #region ITitleCommand 成员

        public string CommandTitle
        {
            get { return ""; }
        }

        #endregion

        public Download()
        {
            CommandType = CommandType.Text;
        }
    }
}