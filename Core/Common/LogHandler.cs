using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Security.Principal;

namespace Development.Core.Metadata
{
    public class LogHandler
    {
        const string _defaultApplicationLogger = "Application";
        public enum LogType
        {

            General,

            Notify

        }


        public static void LogError(string message, Exception error)
        {
            ILog logger = LogManager.GetLogger(_defaultApplicationLogger);


            if ((error.InnerException != null))
            {
                error = error.InnerException;

            }


            if (logger.IsErrorEnabled)
            {
                logger.Error(message, error);

            }

        }


        public static void LogError(string message, IPrincipal user, Uri url, Exception error)
        {
            SetOptionalParametersOnLogger(user, url);

            LogError(message, error);

        }


        public static void LogInfo(string message, LogType type)
        {
            ILog logger = null;


            if (type == LogType.Notify)
            {
                logger = LogManager.GetLogger(LogType.Notify.ToString());


            }
            else
            {
                logger = LogManager.GetLogger(_defaultApplicationLogger);

            }


            if (logger.IsInfoEnabled)
            {
                logger.Info(message);

            }

        }


        public static void LogWarning(string message, Exception error)
        {
            ILog logger = LogManager.GetLogger(_defaultApplicationLogger);


            if ((error.InnerException != null))
            {
                error = error.InnerException;

            }


            if (logger.IsWarnEnabled)
            {
                logger.Warn(message, error);

            }

        }


        public static void LogWarning(string message, IPrincipal user, Uri url, Exception error)
        {
            SetOptionalParametersOnLogger(user, url);

            LogWarning(message, error);

        }


        private static void SetOptionalParametersOnLogger(IPrincipal user, Uri url)
        {
            //set user to log4net context, so we can use %X{user} in the appenders


            if ((user != null) && user.Identity.IsAuthenticated)
            {
                MDC.Set("user", user.Identity.Name);

            }

            //set url to log4net context, so we can use %X{url} in the appenders

            MDC.Set("url", url.ToString());

        }
    }
}
