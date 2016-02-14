using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Development.Core.Interface;
using Development.Core.Interface.Managers;
using Development.Core.User.Interface;
using System.Threading;
using Development.Core.Managers.Proxy;
using System.Collections;

namespace Development.Core
{
    public class DevelopmentManager : IDevelopmentManager
    {

        // Reference to the managers
        private CommonManagerProxy _commonManagerProxy = null;
        private IEventManager _eventManager = null;
        private IPluginManager _pluginManager = null;
        private HashSet<string> powerThreadIds = new HashSet<string>();

        public DevelopmentManager(IEventManager eventManager, IPluginManager pluginManager)
        {

            // Create the logged in user's specific instance of the user manager proxy.
            // The actual implementation class could be changed in configuration
            // or using dependency injection (if we want to implement that)...
            _eventManager = eventManager;
            _pluginManager = pluginManager;
            _commonManagerProxy = new CommonManagerProxy(this);

        }

        // Reference to the logged in user
        public IUser User { get; set; }

        //Reference filterd Entity Id for the login user
        public int EntitySortorderIdColleHash { get; set; }
        public string EntityMainQuery { get; set; }
        public Tuple<ArrayList, ArrayList> GeneralColumnDefs { get; set; }



        // Retrieve the CommonManager
        public ICommonManager CommonManager
        {
            get { return _commonManagerProxy; }
        }
        
        // Retrieve the Event manager
        public IEventManager EventManager
        {
            get { return _eventManager; }
        }

        // Retrieve the Plugin manager
        public IPluginManager PluginManager
        {
            get { return _pluginManager; }
        }


        public ITransaction GetTransaction()
        {
            return GetTransaction(true);
        }

        public ITransaction GetTransaction(bool create)
        {
            return DevelopmentManagerFactory.GetTransaction(create);
        }


        /// <summary>
        /// Gets or Sets this thread in powermode. This means that this thread has unlimited rights!
        /// </summary>
        /// <param name="powerMode"></param>
        public bool PowerMode
        {
            get { return powerThreadIds.Contains(GetThreadKey()); }
            set
            {
                if (value)
                    powerThreadIds.Add(GetThreadKey());
                else
                    powerThreadIds.Remove(GetThreadKey());
            }
        }

        private static string GetThreadKey()
        {
            return Thread.CurrentThread.GetHashCode().ToString();
        }

    }
}
