using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using Provisioning.Commands.Model;
using Provisioning.Expressions;
using Provisioning.Receivers;
using SmartFormat;

namespace Provisioning.Commands
{
    public class QueryCommand : CommandBase
    {
        public List<ColumnDetails> Details { get; set; }

        public string CONNSTRING;
        public string CONNPROVIDER;
        public string TABLENAME;

        private enum QueryType
        {
            UPDATE, INSERT, SELECT, COUNT
        }
        private static QueryType GetQueryType(string querytype)
        {
            switch (querytype)
            {
                case "select":
                case "0":
                    return QueryType.SELECT;
                case "count":
                case "1":
                    return QueryType.COUNT;
                case "insert":
                case "2":
                    return QueryType.INSERT;
                case "update":
                case "3":
                    return QueryType.UPDATE;

            }
            return QueryType.SELECT;
        }
        public QueryCommand(IReceiver receiver, string extra)
            : base(receiver, extra)
        {; }
        /// <summary>
        /// helper method to the WHERE field in a columndetails collection
        /// </summary>
        /// <param name="details"></param>
        /// <returns></returns>
        private static ColumnDetails GetIsKeyField(List<ColumnDetails> details)
        {
            foreach (var x in details)
            {
                if (x.IsKey == true)
                {
                    return x;
                }
            }
            return null;
        }
        /// <summary>
        /// expected arguements - table name, connection string, query type
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override  CommandStatus Execute(Expressions.ExpressionContext context)
        {
            CommandStatus commandStatus = new CommandStatus();

            var conndetails = GetArguments(context);

            TABLENAME = conndetails[0].ToString();

            var connstring = Smart.Format(conndetails[1].ToString(), context.AsDictionary());

            CONNSTRING = ConfigurationManager.ConnectionStrings[connstring].ConnectionString;
            CONNPROVIDER = ConfigurationManager.ConnectionStrings[connstring].ProviderName;

            var _querytpe = conndetails[2].ToString();

            var querytype = GetQueryType(_querytpe);
            switch (querytype)
            {
                case QueryType.COUNT:
                    commandStatus = CreateSelectCountCommand(CONNSTRING, CONNPROVIDER, TABLENAME, Details, context);
                    break;
                case QueryType.SELECT:
                    commandStatus = CreateSelectCommand(CONNSTRING, CONNPROVIDER, TABLENAME, Details, context);
                    break;
                case QueryType.UPDATE:
                    commandStatus = CreateUpdateQueryCommand(Details, TABLENAME, CONNPROVIDER, CONNSTRING, context);
                    break;
                case QueryType.INSERT:
                    commandStatus = CreateInsertQueryCommand(Details, TABLENAME, CONNPROVIDER, CONNSTRING, context);
                    break;
            }
            return commandStatus;
        }
        public static string BuildSelectQueryString(string table, string column, string param, string queryoperator = "=")
        {
            string selectQuery = string.Format("SELECT * FROM {0} where {1} {2} {3}", table, column, queryoperator, param);

            return selectQuery;
        }
        public static string BuildCountQueryString(string table, string column, string param, string queryoperator = "=")
        {
            string countQuery = string.Format("SELECT Count(*) FROM {0} where {1} {2} {3}", table, column, queryoperator, param);

            return countQuery;
        }
        public static CommandStatus CreateInsertQueryCommand(List<ColumnDetails> details, string table, string providername, string connectionstring, ExpressionContext context)
        {
            string commandText = "INSERT into " + table + " ({0}) VALUES ";

            DbProviderFactory dbFactory = DbProviderFactories.GetFactory(providername);
            DbConnection dbConnection = dbFactory.CreateConnection();

            dbConnection.ConnectionString = connectionstring;

            StringBuilder insertBuilder = new StringBuilder();
            int lastma = 0;
            var lastmacnt = details.Count;

            foreach (var c in details)
            {
                if (++lastma == lastmacnt)
                {
                    insertBuilder.Append("" + c.Name + "");
                }
                else
                {
                    insertBuilder.Append("" + c.Name + ",");
                }
            }
            commandText = string.Format(commandText, insertBuilder.ToString());
            StringBuilder insertParameter = new StringBuilder();

            lastma = 0;

            foreach (var x in details)
            {
                if (++lastma == lastmacnt)
                {
                    insertParameter.Append(x.ParameterName + "");
                }
                else
                {
                    insertParameter.Append(x.ParameterName + ",");
                }
            }
            commandText += "(" + insertParameter.ToString() + ")";

            DbCommand dbCommand = dbFactory.CreateCommand();
            dbCommand.CommandText = commandText;
            dbCommand.Connection = dbConnection;
            //dbCommand.CommandText = "UPDATE [Student] SET Name = @Name Where Id = @Id";

            // build a parameter list
            DbParameter dbParam = null;
            foreach (var param in details)
            {
                dbParam = dbCommand.CreateParameter();
                dbParam.ParameterName = param.ParameterName;
                dbParam.Value = Smart.Format(param.Value, context.AsDictionary());// param.Value;
                if (param.DbParameterType != null)
                {
                    if (param.DbParameterType != "")
                    {
                        dbParam.DbType = (DbType)Enum.Parse(typeof(DbType), param.DbParameterType);
                    }
                }
                dbCommand.Parameters.Add(dbParam);
            }
            if (dbConnection.State != ConnectionState.Open)
            {
                dbConnection.Open();
            }
            var rowcount = dbCommand.ExecuteNonQuery();
            dbConnection.Close();

            return new CommandStatus
            {
                Data = rowcount,
                StatusCode = CommandCode.Ok
            };
        }
        public static CommandStatus CreateUpdateQueryCommand(List<ColumnDetails> details, string table, string providername, string connectionstring, ExpressionContext context)
        {
            string commandText = "UPDATE " + table + " SET ";

            DbProviderFactory dbFactory = DbProviderFactories.GetFactory(providername);
            DbConnection dbConnection = dbFactory.CreateConnection();

            dbConnection.ConnectionString = connectionstring;

            string setString = "";

            int lastma = 0;
            var lastmacnt = details.Count;
            foreach (var c in details)
            {
                if (++lastma == lastmacnt)
                {
                    setString += "" + c.Name + "=" + c.ParameterName;
                }
                else
                {
                    setString += "" + c.Name + "=" + c.ParameterName + ",";
                }
            }
            commandText = commandText + setString;

            var keyfield = GetIsKeyField(details);

            commandText += " WHERE " + keyfield.Name + "=" + keyfield.ParameterName;

            DbCommand dbCommand = dbFactory.CreateCommand();
            dbCommand.CommandText = commandText;
            dbCommand.Connection = dbConnection;

            // build a parameter list
            DbParameter dbParam = null;
            foreach (var param in details)
            {
                dbParam = dbCommand.CreateParameter();
                dbParam.ParameterName = param.ParameterName;
                dbParam.Value = Smart.Format(param.Value, context.AsDictionary());// param.Value;
                if (param.DbParameterType != null)
                {
                    if (param.DbParameterType != "")
                    {
                        dbParam.DbType = (DbType)Enum.Parse(typeof(DbType), param.DbParameterType);
                    }
                }
                dbCommand.Parameters.Add(dbParam);
            }
            if (dbConnection.State != ConnectionState.Open)
            {
                dbConnection.Open();
            }
            var rowcount = dbCommand.ExecuteNonQuery();
            dbConnection.Close();

            return new CommandStatus
            {
                Data = rowcount,
                StatusCode = CommandCode.Ok
            };
        }
        public static CommandStatus CreateSelectCountCommand(string connectionString, string providerName, string tableName, List<ColumnDetails> details, ExpressionContext context)
        {
            int rowCount = 0;

            DbProviderFactory factory =
                DbProviderFactories.GetFactory(providerName);
            DbConnection connection = factory.CreateConnection();
            connection.ConnectionString = connectionString;

            using (connection)
            {
                var iskeyfield = GetIsKeyField(details);

                string queryString = BuildCountQueryString(tableName, iskeyfield.Name,
                    Smart.Format(iskeyfield.ParameterName, context.AsDictionary()));

                // Create the DbCommand.            
                DbCommand command = factory.CreateCommand();
                command.CommandText = queryString;
                command.Connection = connection;

                // build a parameter list
                DbParameter dbParam = null;

                dbParam = command.CreateParameter();
                dbParam.ParameterName = Smart.Format(iskeyfield.ParameterName, context.AsDictionary());// iskeyfield.ParameterName;
                dbParam.Value = Smart.Format(iskeyfield.Value, context.AsDictionary());// iskeyfield.Value;
                if (iskeyfield.DbParameterType != null)
                {
                    if (iskeyfield.DbParameterType != "")
                    {
                        dbParam.DbType = (DbType)Enum.Parse(typeof(DbType), iskeyfield.DbParameterType);
                    }
                }
                command.Parameters.Add(dbParam);
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                try
                {
                    rowCount = (int)command.ExecuteScalar();
                }
                catch
                {
                    rowCount = 0;
                }
                connection.Close();
            }
            return new CommandStatus
            {
                StatusCode = CommandCode.Ok,
                Data = rowCount
            };
        }
        public static CommandStatus CreateSelectCommand(string connectionString, string providerName, string tableName, List<ColumnDetails> details, ExpressionContext context)
        {
            List<string> resultSet = new List<string>();

            DbProviderFactory factory =
                DbProviderFactories.GetFactory(providerName);
            DbConnection connection = factory.CreateConnection();
            connection.ConnectionString = connectionString;
            using (connection)
            {
                var iskeyfield = GetIsKeyField(details);

                string queryString = BuildSelectQueryString(tableName, iskeyfield.Name, Smart.Format(iskeyfield.ParameterName, context.AsDictionary()));

                // Create the DbCommand.            
                DbCommand command = factory.CreateCommand();
                command.CommandText = queryString;
                command.Connection = connection;

                // build a parameter list
                DbParameter dbParam = null;

                dbParam = command.CreateParameter();
                dbParam.ParameterName = Smart.Format(iskeyfield.ParameterName, context.AsDictionary());//iskeyfield.ParameterName;
                dbParam.Value = Smart.Format(iskeyfield.Value, context.AsDictionary());// iskeyfield.Value;
                if (iskeyfield.DbParameterType != null)
                {
                    if (iskeyfield.DbParameterType != "")
                    {
                        dbParam.DbType = (DbType)Enum.Parse(typeof(DbType), iskeyfield.DbParameterType);
                    }
                }
                command.Parameters.Add(dbParam);
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                var reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        foreach (var x in details)
                        {
                            resultSet.Add(reader[x.Name].ToString());
                        }
                        break;// one row at a time (?)
                    }
                }
            }
            return new CommandStatus
            {
                StatusCode = CommandCode.Ok,
                Data = resultSet.ToArray()
            };
        }
    }
    
}