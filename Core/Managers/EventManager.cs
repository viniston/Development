using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrandSystems.Marcom.Core.Interface.Managers;

namespace BrandSystems.Marcom.Core.Managers
{
    internal class EventManager : IEventManager
    {
        private static EventManager instance = new EventManager();


        internal static EventManager Instance
        {
            get { return instance; }
        }

        void IEventManager.Initialize()
        {

        }
    }
}
