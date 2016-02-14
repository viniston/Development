using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Xml.Linq;
using Development.Core.Interface;
using Development.Dal;
using Development.Core;
using Development.Core.User.Interface;
using System.Globalization;


namespace Development.Web
{
    public class Global : System.Web.HttpApplication
    {
        

        void Application_Start(object sender, EventArgs e)
        {


            Development.Core.Metadata.LogHandler.LogInfo("global asax file hit happen at" + DateTime.Now, Development.Core.Metadata.LogHandler.LogType.General);

            //register MVC Web API Attribute routing
            System.Web.Http.GlobalConfiguration.Configure(WebApiConfig.Register);

            //intialize all Development tenants
            DevelopmentManagerFactory.InitializeSystem();

            Development.Core.Metadata.LogHandler.LogInfo("global asax file hit finished  at" + DateTime.Now, Development.Core.Metadata.LogHandler.LogType.General);

        }

        void Application_End(object sender, EventArgs e)
        {
            //  Code that runs on application shutdown

        }

        void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs

        }

        void Application_BeginRequest(object sender, EventArgs e)
        {
        }

        void Session_Start(object sender, EventArgs e)
        {

        }

        void Session_End(object sender, EventArgs e)
        {
            // Code that runs when a session ends. 
            // Note: The Session_End event is raised only when the sessionstate mode
            // is set to InProc in the Web.config file. If session mode is set to StateServer 
            // or SQLServer, the event is not raised.

        }


    }
}
