using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Sql;
using System.Data.SqlClient;

namespace ApplicationMaintanceTool.DAL
{
    public class SQLConnectionManger
    {

        string connectionString = string.Format
            (@"data source={0};database={1};User Id={2};Password={3}",
                "DESKTOP-1M6CA3A", "ApplicationMaintance", "sa","sys123sa#"
            );


        private static SQLConnectionManger connectionManager;

        private static SqlConnection dbConnection;

        private SQLConnectionManger()
        {
            if (dbConnection == null )
            {
                dbConnection = new SqlConnection(connectionString);
                dbConnection.Open();
            }
        }


        public static SQLConnectionManger GetSQLConnectionManger()
        {
            if (dbConnection == null)
                connectionManager = new SQLConnectionManger();
            return connectionManager;
        }

        public void CloseConnection()
        {
            if (dbConnection != null && dbConnection.State == System.Data.ConnectionState.Open)
            {
                dbConnection.Close();
                dbConnection = null;
            }
        }

        public int ExecuteInsertQuery(string query)
        {
            SqlCommand sqlCommand = dbConnection.CreateCommand();
            sqlCommand.CommandText = query;
            return sqlCommand.ExecuteNonQuery();
        }

        public SqlDataReader ExecuteSelectQuery(string query)
        {
            SqlCommand sqlCommand = dbConnection.CreateCommand();
            sqlCommand.CommandText = query;
            return sqlCommand.ExecuteReader();
        }

    }
}
