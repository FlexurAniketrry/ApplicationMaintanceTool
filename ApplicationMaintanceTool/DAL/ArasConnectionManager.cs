using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Web;
using System.Web.UI.WebControls;
using Aras.IOM;

namespace ApplicationMaintanceTool.DAL
{
    public class ArasConnectionManager
    {
        public Innovator innovator { get;  }

        /// <summary>
        /// ArasConnection Manager - Constructor 
        /// </summary>
        /// <param name="arasUrl"></param>
        /// <param name="database"></param>
        /// <param name="userid"></param>
        /// <param name="password"></param>
        public ArasConnectionManager(string arasUrl, string database, string userid, string password)
        {
            HttpServerConnection connection 
                = IomFactory.CreateHttpServerConnection(arasUrl, database, userid, password);
            Item loginItem = connection.Login();
            if (loginItem.isError())
                throw new InvalidCredentialException("Unable To Login To Aras Database");

            innovator = loginItem.getInnovator();
        }

        public void LogoutArasConnection() => ((HttpServerConnection)(innovator.getConnection())).Logout();

    }
}