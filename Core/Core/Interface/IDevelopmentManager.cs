using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Development.Core.Interface.Managers;
using Development.Core.User.Interface;

using System.Collections;

namespace Development.Core.Interface
{
    public interface IDevelopmentManager
    {
        IUser User { get; set; }

        ICommonManager CommonManager { get; }
       
        IEventManager EventManager { get; }
        IPluginManager PluginManager { get; }

        ITransaction GetTransaction();
        ITransaction GetTransaction(bool create);

        bool PowerMode { get; set; }
    }
}
