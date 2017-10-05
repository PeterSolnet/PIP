using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using log4net;
using Provisioning.Commands.Model;
using Provisioning.Expressions;
using Provisioning.Receivers;
using SmartFormat;

namespace Provisioning.Commands
{
    public class SqlQueryCommand : CommandBase
    {
        static readonly ILog log = LogManager.GetLogger(typeof(SqlQueryCommand));

        public bool IsSelectStatement { get; set; }
        public string QueryManager { get; set; }

        public string CONNSTRING;
        public string CONNPROVIDER;

        public SqlQueryCommand(IReceiver receiver, string extra)
            : base(receiver, extra)
        {; }

        public override CommandStatus Execute(ExpressionContext context)
        {
            var conndetails = GetArguments(context); // Expected: connectionstring, connection type

            CONNSTRING = (string)conndetails[0];
            CONNPROVIDER = (string)conndetails[1];

            DbProviderFactory factory =
                DbProviderFactories.GetFactory(CONNPROVIDER);
            DbConnection connection = factory.CreateConnection();
            connection.ConnectionString = CONNSTRING;

            var contextDictionary = context.AsDictionary();

            var commandText = Smart.Format(QueryManager, contextDictionary);

            if (IsSelectStatement == true)
            {
                using (connection)
                {
                    // Create the DbCommand.            
                    DbCommand command = factory.CreateCommand();
                    command.CommandText = commandText;
                    command.Connection = connection;

                    // Create the DbDataAdapter.          
                    DbDataAdapter adapter = factory.CreateDataAdapter();
                    adapter.SelectCommand = command;

                    DataTable table = new DataTable();
                    adapter.Fill(table);

                    return new CommandStatus
                    {
                        Data = table.Rows,
                        StatusCode = CommandCode.Ok
                    };
                }
            }
            else
            {
                using (connection)
                {
                    DbCommand command = factory.CreateCommand();
                    command.CommandText = commandText;
                    command.Connection = connection;
                    if (connection.State == ConnectionState.Closed)
                    {
                        connection.Open();
                    }
                    var rowCount = (int)command.ExecuteNonQuery();
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                    return new CommandStatus
                    {
                        Data = rowCount,
                        StatusCode = CommandCode.Ok
                    };
                }
            }
        }
    }
}