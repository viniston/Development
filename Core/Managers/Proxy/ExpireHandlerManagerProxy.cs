using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using BrandSystems.Marcom.Core.Interface;
using BrandSystems.Marcom.Core.Interface.Managers;
using BrandSystems.Marcom.Core.Metadata.Interface;
using BrandSystems.Marcom.Dal.Metadata.Model;
using System.Xml;
using BrandSystems.Marcom.Core.Metadata;
using BrandSystems.Marcom.Metadata.Interface;
using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;
using BrandSystems.Marcom.Metadata;
using BrandSystems.Marcom.Core.Planning.Interface;
using BrandSystems.Marcom.Core.Common.Interface;
using BrandSystems.Marcom.Core.Access.Interface;
using BrandSystems.Marcom.Core.Managers;
using BrandSystems.Marcom.Core.Dam;
using BrandSystems.Marcom.Core.Dam.Interface;
using BrandSystems.Marcom.Core.DAM;
using BrandSystems.Marcom.Core.DAM.Interface;
using BrandSystems.Marcom.Dal.DAM.Model;
using BrandSystems.Marcom.Core.ExpireHandler.Interface;


namespace BrandSystems.Marcom.Core.Managers.Proxy
{
    internal partial class ExpireHandlerManagerProxy : IExpireHandlerManager, IManagerProxy
    {
        // Reference to the MarcomManager
        /// <summary>
        /// The _marcom manager      
        /// </summary>
        private MarcomManager _marcomManager = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataManagerProxy" /> class.
        /// </summary>
        /// <param name="marcomManager">The marcom manager.</param>
        internal ExpireHandlerManagerProxy(MarcomManager marcomManager)
        {
            _marcomManager = marcomManager;

            // Do some initialization.... 
            // i.e. cache logged in user specific things (or maybe use lazy loading for that)
        }


        // Reference to the MarcomManager (only internal)
        /// <summary>
        /// Gets the marcom manager.
        /// </summary>
        /// <value>
        /// The marcom manager.
        /// </value>
        internal MarcomManager MarcomManager
        {
            get { return _marcomManager; }
        }


        public IList<IExpireActionSources> GetExpireActions(int SourceID , int ActionID)
        {
            return ExpireHandlerManager.Instance.GetExpireActions(this, SourceID, ActionID);
        }
        public int CreateExpireAction(int ActionID, int SourceID, int SourceEnityID, int SourceFrom, string Actionexutedays, string DateActionexpiredate, bool Actionexute, bool ispublish, int ActionsourceId, IList<IAttributeData> listattributevalues)
        {
            return ExpireHandlerManager.Instance.CreateExpireAction(this, ActionID, SourceID, SourceEnityID, SourceFrom, Actionexutedays, DateActionexpiredate, Actionexute, ispublish,ActionsourceId, listattributevalues);
        }

        public bool UpdateExpireActionDate(int SourceID, string DateActionexpiredate, int SourcetypeID, int ActionID, string Actionexutedays)
        {
            return ExpireHandlerManager.Instance.UpdateExpireActionDate(this, SourceID, DateActionexpiredate, SourcetypeID, ActionID, Actionexutedays);
        }
        public bool DeleteExpireAction(int ActionsourceId)
        {
            return ExpireHandlerManager.Instance.DeleteExpireAction(this, ActionsourceId);
        }
    }
}
