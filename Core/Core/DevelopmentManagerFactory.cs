using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Development.Core;
using Development.Core.Interface;
using Development.Core.Managers;
using Development.Dal.Base;
using System.Threading;
using System.Configuration;
using Development.Core.Interface.Managers;
using Development.Core.Metadata;
using System.Collections;

namespace Development.Core
{
    public class DevelopmentManagerFactory
    {
        // Dictionary to keep control over all current sessions
        private static Dictionary<Guid, IDevelopmentManager> _sessions = new Dictionary<Guid, IDevelopmentManager>();

        // Dictionary to keep control over all current API sessions with tokens
        private static Dictionary<Guid, Guid> _APITokenSessions = new Dictionary<Guid, Guid>();

        // Dictionary to keep control over all ongoing transactions
        private static Dictionary<string, Transaction> _transactions = new Dictionary<string, Transaction>();

        // List of the Managers
        private static List<IManager> _managers = new List<IManager>();

        // Reference to the only EventManager
        private static IEventManager _eventManager = null;

        //Reference to the only PluginManager
        private static IPluginManager _pluginManager = null;

        private static bool _isInitialized = false;

        private static readonly object _locker = new object();

        internal static Guid _systemSessionId;



        /// <summary>
        /// Gets a DevelopmentManager with a certain sessionId
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public static IDevelopmentManager GetDevelopmentManager(Guid sessionId)
        {
            LogHandler.LogInfo("************************ user session created " + DateTime.Now + " ************************", LogHandler.LogType.General);
            // Return a user specific DevelopmentManager
            return _sessions[sessionId];
        }

        #region "Initialization"

        /// <summary>
        /// Check if Development platform is initialized, if not it is initialized
        /// </summary>
        internal static void EnsureInitialized()
        {
            lock (_managers)
            {
                if (!_isInitialized)
                    Initialize();
            }
        }

        private static void Reinitialize()
        {
            _isInitialized = false;
            Initialize();
        }




        /// <summary>
        /// Initialize Development Platform
        /// </summary>
        private static void Initialize()
        {
            Development.Core.Metadata.LogHandler.LogInfo("initialization block hit happen at" + DateTime.Now, Development.Core.Metadata.LogHandler.LogType.General);


            if (_isInitialized)
                ShutDown();

            // Initialize the event manager
            _eventManager = EventManager.Instance;
            _eventManager.Initialize();

            IDevelopmentManager initializingDevelopmentManager = new DevelopmentManager(_eventManager, null);

            // Initialize log4net
            log4net.Config.XmlConfigurator.Configure();

            // Initialize DataAccess Layer
            PersistenceManager.Instance.Initialize();
            _managers = new List<IManager>();
            // Create the managers (this could be done by configuration)

            _managers.Add(CommonManager.Instance);

            // Todo: add the rest of the managers

            // Initialize all managers
            foreach (IManager manager in _managers)
            {
                manager.Initialize(initializingDevelopmentManager);
            }

            GetSystemSession();
            _isInitialized = true;

            Development.Core.Metadata.LogHandler.LogInfo("initialization block hit done at" + DateTime.Now, Development.Core.Metadata.LogHandler.LogType.General);


        }

        /// <summary>
        /// Shutdown and cleanup Development Platform
        /// </summary>
        private static void ShutDown()
        {
            _managers.Clear();
            _sessions.Clear();
            _transactions.Clear();
            _isInitialized = false;
        }

        #endregion

        #region "Sessions"

     

        /// <summary>
        /// Authenticates the user and returns a sessionId that can be used 
        /// in GetDevelopmentManger(). If the user cannot be authenticated -1 is returned...
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static void InitializeSystem()
        {
            Development.Core.Metadata.LogHandler.LogInfo("initialization from global asax hit happen at" + DateTime.Now, Development.Core.Metadata.LogHandler.LogType.General);

            EnsureInitialized();

        }



        public static Guid GetSystemSession()
        {
            if (_systemSessionId == Guid.Empty)
            {
                Development.Core.User.Interface.IUser user = new User.User();
                user.UserName = "system";
                _systemSessionId = Guid.NewGuid();// /* A generated session key */ 1;
                _sessions[_systemSessionId] = new DevelopmentManager(_eventManager, _pluginManager);
                _sessions[_systemSessionId].User = user;
                return _systemSessionId;
            }
            else
                return _systemSessionId;
            throw new Exception("Invalid Opeartion");

        }


        /// <summary>
        /// Logout the current user
        /// </summary>
        /// <param name="sessionId"></param>
        public static void EndSession(Guid sessionId)
        {
            _sessions.Remove(sessionId);

            //remove session details for API users
            var APITokenToRemove = _APITokenSessions.Where(kvp => kvp.Value == sessionId).Select(kvp => kvp.Key);
            foreach (var APIsession in APITokenToRemove)
            {
                _APITokenSessions.Remove(APIsession);
            }
            //

            // Some other cleanup
        }

        #endregion

        #region "Transactions"

        internal static ITransaction GetTransaction(bool create)
        {
            string key = GetTransactionKey();
            lock (_transactions)
            {
                if (!_transactions.ContainsKey(key))
                {

                    if (create)
                    {
                        _transactions.Add(key, new Transaction(key, false));
                    }
                    else
                    {
                        return null;
                    }
                }

                Transaction t = _transactions[key];
                t.IncrementCommitCount();
                return t;
            }
        }

        private static string GetTransactionKey()
        {
            return Thread.CurrentThread.GetHashCode().ToString();
        }

        internal static void TransactionCommited(ITransaction tx)
        {
            try
            {
                // Todo: Add synchronization here
                // Call all managers
                foreach (IManager manager in _managers)
                {
                    manager.CommitCaches();
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        internal static void TransactionRollbacked(ITransaction tx)
        {
            // Todo: Add synchronization here
            // Call all managers
            foreach (IManager manager in _managers)
            {
                manager.RollbackCaches();
            }
        }

        internal static void RemoveTransaction(string key)
        {
            lock (_transactions)
            {
                _transactions.Remove(key);
            }
        }

        #endregion
    }
}
