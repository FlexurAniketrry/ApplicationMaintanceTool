using ApplicationMaintanceTool.DAL;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace ApplicationMaintanceTool.Models
{
    public class SessionState
    {
        public const string Start = "Start";
        public const string Complete = "Complete";
        public const string InProgress = "InProgress";
    }


    public class AnalysisSession
    {
        public string ArasUrl { get; set; }

        public string Database { get; set; }

        public string UserName { get; set; }

        public string Description { get; set; }

        public string SessionId { get; set; }

        private string State { get; set; }

        private AnalysisSession()
        { 
            SessionId = Guid.NewGuid().ToString();
            State = SessionState.Start;
        }

        public static AnalysisSession NewSession(AnalysisSession session)
        {
            AnalysisSession _session = new AnalysisSession();
            _session.ArasUrl = session.ArasUrl;
            _session.Database = session.Database;
            _session.Description = session.Description;
            _session.UserName = session.UserName;
            return _session;
        }

        public static HashSet<AnalysisSession> GetAll()
        {
            SQLConnectionManger dbManager = null;
            try
            {
                dbManager = SQLConnectionManger.GetSQLConnectionManger();
                StringBuilder selectQuery = new StringBuilder
                    ("SELECT TOP (1000) [id],[aras_url],[aras_db] FROM[ApplicationMaintance].[dbo].[SessionHistory]");

                SqlDataReader reader = dbManager.ExecuteSelectQuery(selectQuery.ToString());
                HashSet<AnalysisSession> sessions = new HashSet<AnalysisSession>();
                while (reader.Read())
                {
                    AnalysisSession session = new AnalysisSession();
                    session.SessionId = (String)reader["id"];
                    if (reader["aras_url"] != null && reader["aras_url"] != DBNull.Value)
                        session.ArasUrl = (String)reader["aras_url"];
                    sessions.Add(session);
                }
                reader.Close();

                return sessions;
            }
            finally
            {
                if (dbManager != null)
                    dbManager.CloseConnection();
            }
        }



        public static bool SaveSessionToDB(AnalysisSession session)
        {
            SQLConnectionManger dbManager = null;
            try
            {
                dbManager = SQLConnectionManger.GetSQLConnectionManger();
                StringBuilder insertQuery = new StringBuilder
                    ("INSERT INTO DBO.SessionHistory (ID,ARAS_URL,ARAS_DB) VALUES " +
                    "('" + session.SessionId + "','"+ session.ArasUrl+"','"+session.Database+"')");
                int result = dbManager.ExecuteInsertQuery(insertQuery.ToString());
                if (result > 0)
                    return true;
            }
            finally
            {
                if (dbManager != null)
                    dbManager.CloseConnection();
            }
            return false;
        }
    }
}