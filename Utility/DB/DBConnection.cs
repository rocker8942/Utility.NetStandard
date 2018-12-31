using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Utility.DB
{
    public class DBConnection : IDisposable
    {
        #region Fields

        private SqlConnection _connection;

        #endregion

        #region Properties

        public string ConnectionString { set; get; }

        #endregion

        internal delegate void Callback(SqlDataReader r);

        #region Constructors

        /// <summary>
        ///     Create a database connection with the default connection string
        /// </summary>
        public DBConnection()
        {
        }

        /// <summary>
        ///     Create a database connection using the given connection string
        /// </summary>
        public DBConnection(string connectionString)
        {
            ConnectionString = connectionString;
            OpenConnection();
        }

        #endregion

        #region Methods

        #region Disposing Methods

        /// <summary>
        ///     Implement Disposable and call Finalize using garbage collector
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Free other state (managed objects).
            }
            // Free your own state (unmanaged objects).
            CloseConnection();
        }

        ~DBConnection()
        {
            // Simply call Dispose(false).
            Dispose(false);
        }

        #endregion

        /// <summary>
        ///     Attempts to open the connection if it is not already open. Throws an exception if it cannot.
        /// </summary>
        public void OpenConnection()
        {
            if (_connection == null)
                _connection = new SqlConnection(ConnectionString);

            if (_connection.State == ConnectionState.Broken)
            {
                _connection.Close();

                var ex = new Exception("Database Connection is broken.");
                throw ex;
            }
            if (_connection.State != ConnectionState.Open)
                _connection.Open();
        }

        /// <summary>
        ///     Closes the connection if it is in any state except closed.
        /// </summary>
        public void CloseConnection()
        {
            if (_connection != null)
            {
                if (_connection.State != ConnectionState.Closed)
                    _connection.Close();

                _connection = null;
            }
        }

        /// <summary>
        ///     Returns a SqlDataReader object which contains the rows returned by the given query.
        /// </summary>
        /// <param name="query">The SQL query to be run</param>
        /// <returns>A SqlDataReader object</returns>
        public SqlDataReader GetDataReader(string query)
        {
            OpenConnection();

            var sqlCommand = new SqlCommand(query, _connection);

            return sqlCommand.ExecuteReader();
        }

        /// <summary>
        ///     Runs the given query in the database. Usually for INSERT and UPDATE queries.
        /// </summary>
        /// <param name="query">The SQL query to be run</param>
        /// <returns>True if the query was run successfully, False otherwise.</returns>
        public void RunQuery(string query)
        {
            OpenConnection();

            var sqlCommand = new SqlCommand(query, _connection);

            sqlCommand.ExecuteNonQuery();
        }

        /// <summary>
        ///     Runs a stored procedure from the database.
        /// </summary>
        /// <param name="procName"></param>
        /// <param name="parameters">The list of parameter names</param>
        /// <param name="useIdentity"></param>
        /// <returns>True if successful, False otherwise</returns>
        public int RunStoredProcedure(string procName, Dictionary<string, string> parameters, bool useIdentity = false)
        {
            var identity = SystemConstants.Null;

            OpenConnection();

            // Create a SQL command object, and set it up to run a stored procedure.
            var sqlCommand = new SqlCommand(procName, _connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            // Now add the procedure parameters and values
            for (var i = 0; i < parameters.Count; i++)
            {
                var param = new SqlParameter(parameters.ElementAt(i).Key, parameters.ElementAt(i).Value);
                sqlCommand.Parameters.Add(param);
            }

            if (useIdentity)
            {
                // Add the output parameter which must exist in the stored procedure for this function to succeed.
                sqlCommand.Parameters.Add("@ident", SqlDbType.Int);
                sqlCommand.Parameters["@ident"].Direction = ParameterDirection.Output;

                sqlCommand.ExecuteNonQuery();

                // Gets the new id for the item entered into the database (or updated in the case of update queries).
                // This relies on the output variable existing in the stored procedure, otherwise an exception is thrown.
                identity = (int) sqlCommand.Parameters["@ident"].Value;

                // Sql server returns a negative integer to indicate an error result.
                if (identity < 0)
                    throw new Exception(string.Format("Running stored procedure:{0} didn't return the expected identity.", procName));
            }
            else
            {
                sqlCommand.ExecuteNonQuery();
            }

            return identity;
        }

        /// <summary>
        ///     Runs the given stored procedure in the database.
        /// </summary>
        public void RunStoredProcedure(string procName)
        {
            OpenConnection();

            var sqlCommand = new SqlCommand(procName, _connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            sqlCommand.ExecuteNonQuery();
        }

        /// <summary>
        ///     Runs the stored procedure, taking the parameters and values given.
        /// </summary>
        /// <param name="procName"></param>
        /// <param name="parameters">The list of parameters</param>
        /// <returns>The rows returned by the query, null if an error occurs</returns>
        public SqlDataReader GetDataReaderFromStoredProcedure(string procName, Dictionary<string, string> parameters)
        {
            OpenConnection();

            var sqlCommand = new SqlCommand(procName, _connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            // Now add the procedure parameters and values
            for (var i = 0; i < parameters.Count; i++)
                if (parameters.ElementAt(i).Value != null)
                {
                    var param = new SqlParameter(parameters.ElementAt(i).Key, parameters.ElementAt(i).Value);
                    sqlCommand.Parameters.Add(param);
                }
                else
                {
                    var param = new SqlParameter(parameters.ElementAt(i).Key, DBNull.Value);
                    sqlCommand.Parameters.Add(param);
                }

            var reader = sqlCommand.ExecuteReader();

            return reader;
        }

        /// <summary>
        ///     Runs the given stored procedure, returning results usually for a SELECT statement
        /// </summary>
        /// <returns>The rows returned by the query, null if an error occurs</returns>
        public SqlDataReader GetDataReaderFromStoredProcedure(string procName)
        {
            return GetDataReaderFromStoredProcedure(procName, new Dictionary<string, string>());
        }

        /// <summary>
        ///     Runs the stored procedure, which returns an integer count, taking the parameters and values given.
        /// </summary>
        /// <param name="procName"></param>
        /// <param name="parameters">The list of parameters</param>
        /// <returns>count</returns>
        public int GetCountFromStoredProcedure(string procName, Dictionary<string, string> parameters)
        {
            var result = 0;

            var reader = GetDataReaderFromStoredProcedure(procName, parameters);

            if (reader.Read())
                result = (int) reader[0];

            return result;
        }

        /// <summary>
        ///     Excute stored procedure and excute the call back method with the returned data.
        /// </summary>
        /// <param name="procName">stored procedure name</param>
        /// <param name="parameterList">stored procedure parameter list</param>
        /// <param name="callback">callback method</param>
        internal static void CallbackFromStoredProcedure(string procName, Dictionary<string, string> parameterList,
            Callback callback)
        {
            using (var cn = new DBConnection())
            {
                SqlDataReader reader = null;

                try
                {
                    reader = cn.GetDataReaderFromStoredProcedure(procName, parameterList);

                    callback?.Invoke(reader);
                }
                finally
                {
                    reader?.Close();

                    cn.CloseConnection();
                }
            }
        }

        #endregion
    }
}