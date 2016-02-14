using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrandSystems.Marcom.Core.Interface;
using BrandSystems.Marcom.Core.Interface.Managers;
using BrandSystems.Marcom.Core.Access.Interface;
using System.Collections;
using Newtonsoft.Json.Linq;

namespace BrandSystems.Marcom.Core.Managers.Proxy
{
    internal class AccessManagerProxy : IAccessManager, IManagerProxy
    {
        // Reference to the MarcomManager
        private IMarcomManager _marcomManager = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessManagerProxy"/> class.
        /// </summary>
        /// <param name="marcomManager">The marcom manager.</param>
        internal AccessManagerProxy(IMarcomManager marcomManager)
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
        internal IMarcomManager MarcomManager
        {
            get { return _marcomManager; }
        }

        #region Instance of Classes In ServiceLayer reference
        /// <summary>
        /// Returns TaskMember class.
        /// </summary>
        public ITaskMember TaskMemberservice()
        {
            return AccessManager.Instance.TaskMemberservice();
        }

        #endregion

        /// <summary>
        /// Creates the entity user role.
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <param name="Roleid">The roleid.</param>
        /// <param name="Userid">The userid.</param>
        /// <param name="IsInherited">if set to <c>true</c> [is inherited].</param>
        /// <param name="InheritedFromEntityid">The inherited from entityid.</param>
        /// <returns>
        /// IEntityRoleUser
        /// </returns>
        public int CreateEntityUserRole(int entityId, int Roleid, int Userid, bool IsInherited, int InheritedFromEntityid)
        {
            return AccessManager.Instance.CreateEntityUserRole(this, entityId, Roleid, Userid, IsInherited, InheritedFromEntityid);
        }

        /// <summary>
        /// Deletes the entity user role.
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <param name="Roleid">The roleid.</param>
        /// <param name="Userid">The userid.</param>
        /// <returns>
        /// IEntityRoleUser
        /// </returns>
        public bool DeleteEntityUserRole(int ID)
        {
            return AccessManager.Instance.DeleteEntityUserRole(this, ID);
        }


        /// <summary>
        /// get the role.
        /// </summary>
        /// <returns>Ilist of IRole</returns>
        public IList<IRole> GetRole()
        {
            return AccessManager.Instance.GetRole(this);
        }

        /// <summary>
        /// Saves the update role.
        /// </summary>
        /// <param name="Caption">The caption.</param>
        /// <param name="Description">The description.</param>
        /// <param name="Id">The id.</param>
        /// <returns>
        /// String
        /// </returns>
        public int SaveUpdateRole(String Caption, String Description, int Id = 0)
        {
            return AccessManager.Instance.SaveUpdateRole(this, Caption, Description, Id);
        }

        /// <summary>
        /// Deletes the role.
        /// </summary>
        /// <param name="Id">The id.</param>
        /// <returns>
        /// String
        /// </returns>
        public bool DeleteRole(int Id)
        {
            return AccessManager.Instance.DeleteRole(this, Id);
        }

        /// <summary>
        /// Saves the update entity ACL.
        /// </summary>
        /// <param name="RoleId">The role id.</param>
        /// <param name="ModuleId">The module id.</param>
        /// <param name="EntityTypeId">The entity type id.</param>
        /// <param name="FeatureId">The feature id.</param>
        /// <param name="AccessRights">The access rights.</param>
        /// <returns>
        /// int
        /// </returns>
        public int SaveUpdateEntityACL(int RoleId, int ModuleId, int EntityTypeId, int FeatureId, string AccessRights)
        {
            return AccessManager.Instance.SaveUpdateEntityACL(this, RoleId, ModuleId, EntityTypeId, FeatureId, AccessRights);
        }


        /// <summary>
        /// get the global access.
        /// </summary>
        /// <param name="caption">The GlobalRoleid.</param>
        /// <param name="description">The Moduleid.</param>
        /// <param name="roleid">The EntityTypeid.</param>
        /// /// <param name="description">The Featureid.</param>
        /// <param name="roleid">The AccessPermission.</param>
        /// <returns>Ilist of IGlobalAcl</returns>
        public IList<object> GetGlobalAcl(int roleID)
        {
            return AccessManager.Instance.GetGlobalAcl(this, roleID);
        }


        /// <summary>
        /// Saves the update global ACL.
        /// </summary>
        /// <param name="GlobalRoleId">The global role id.</param>
        /// <param name="ModuleId">The module id.</param>
        /// <param name="EntityTypeId">The entity type id.</param>
        /// <param name="FeatureId">The feature id.</param>
        /// <param name="AccessRights">The access rights.</param>
        /// <returns>
        /// int
        /// </returns>
        /// 
        public bool SaveUpdateGlobalACL(int GlobalRoleId, int ModuleId, int EntityTypeId, int FeatureId, string AccessRights, int ID=0)
        {
            return AccessManager.Instance.SaveUpdateGlobalACL(this, GlobalRoleId, ModuleId, EntityTypeId, FeatureId, AccessRights, ID);
        }

        /// <summary>
        /// Saves the delete global ACL.
        /// </summary>
        /// <param name="ID">ID</param>
        /// <returns>
        /// int
        /// </returns>
        /// 
        public bool DeleteGlobalACL(int ID)
        {
            return AccessManager.Instance.DeleteGlobalACL(this, ID);
        }

        //GetGlobalRole
        /// <summary>
        /// get the global role.
        /// </summary>
        /// <param name="caption">The caption.</param>
        /// <param name="description">The description.</param>
        /// <param name="roleid">The roleid.</param>
        /// <returns>Ilist of GlobalRole</returns>
        public IList<IGlobalRole> GetGlobalRole()
        {
            return AccessManager.Instance.GetGlobalRole(this);
        }

        //GetGlobalRoleByID
        /// <summary>
        /// get the global role by id
        /// </summary>
        /// <param name="caption">The caption.</param>
        /// <param name="description">The description.</param>
        /// <param name="roleid">The roleid.</param>
        /// <returns>Ilist of GlobalRole</returns>
        public IList<IGlobalRole> GetGlobalRoleByID(int ID)
        {
            return AccessManager.Instance.GetGlobalRoleByID(this,ID);
        }
        /// <summary>
        /// InsertUpdates the global role.
        /// </summary>
        /// <param name="caption">The caption.</param>
        /// <param name="description">The description.</param>
        /// <param name="roleid">The roleid.</param>
        /// <returns>
        /// int
        /// </returns>
        public bool InsertUpdateGlobalRole(string caption, string description, int roleid=0)
        {
            return AccessManager.Instance.InsertUpdateGlobalRole(this, caption, description, roleid);
        }

        /// <summary>
        /// Deletes the global role.
        /// </summary>
        /// <param name="roleid">The roleid.</param>
        /// <returns>
        /// string
        /// </returns>
        public int DeleteGlobalRole(int roleid)
        {
            return AccessManager.Instance.DeleteGlobalRole(this, roleid);
        }

        /// <summary>
        /// Inserts the global role user.
        /// </summary>
        /// <param name="globalroleid">The globalroleid.</param>
        /// <param name="userid">The userid.</param>
        /// <returns>
        /// int
        /// </returns>
        public int InsertGlobalRoleUser(int globalroleid, int userid)
        {
            return AccessManager.Instance.InsertGlobalRoleUser(this, globalroleid, userid);
        }

        /// <summary>
        /// Checks the access.
        /// </summary>
        /// <param name="moduleId">The module id.</param>
        /// <param name="EntitytypeID">The entitytype ID.</param>
        /// <param name="featureId">The feature id.</param>
        /// <param name="AccessPermissionId">The access permission id.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool CheckAccess(Modules moduleId, int EntitytypeID, FeatureID featureId, OperationId AccessPermissionId)
        {
            return AccessManager.Instance.CheckAccess(this, moduleId,EntitytypeID, featureId ,AccessPermissionId);
        }
        public bool CheckUserAccess(int moduleId, int featureId, int AccessPermissionId)
        {
            return AccessManager.Instance.CheckUserAccess(this, (Modules)moduleId, (FeatureID)featureId, (OperationId)AccessPermissionId);
        }
        public bool CheckUserAccess(int moduleId, int featureId)
        {
            return AccessManager.Instance.CheckUserAccess(this, (Modules)moduleId, (FeatureID)featureId);
        }
        /// <summary>
        /// Tries the access.
        /// </summary>
        /// <param name="moduleId">The module id.</param>
        /// <param name="EntitytypeID">The entitytype ID.</param>
        /// <param name="featureId">The feature id.</param>
        /// <param name="AccessPermissionId">The access permission id.</param>
        public void TryAccess(Modules moduleId, int EntitytypeID, FeatureID featureId, OperationId AccessPermissionId)
        {
             AccessManager.Instance.TryAccess(this, moduleId, EntitytypeID, featureId, AccessPermissionId);
        }

        public void TryAccess(Modules moduleId, FeatureID featureId, OperationId AccessPermissionId)
        {
            AccessManager.Instance.TryAccess(this, moduleId, featureId, AccessPermissionId);
        }

        public void TryAccess(Modules moduleId, FeatureID featureId)
        {
            AccessManager.Instance.TryAccess(this, moduleId, featureId);
        }
        /// <summary>
        /// Checks the access.
        /// </summary>
        /// <param name="moduleId">The module id.</param>
        /// <param name="EntitytypeID">The entitytype ID.</param>
        /// <param name="featureId">The feature id.</param>
        /// <param name="AccessPermissionId">The access permission id.</param>
        /// <param name="EntityId">The entity id.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool CheckAccess(Modules moduleId, int EntitytypeID, FeatureID featureId, OperationId AccessPermissionId, int EntityId)
        {
            return AccessManager.Instance.CheckAccess(this, moduleId, EntitytypeID, featureId, AccessPermissionId,EntityId);
        }

        /// <summary>
        /// Tries the access.
        /// </summary>
        /// <param name="moduleId">The module id.</param>
        /// <param name="EntitytypeID">The entitytype ID.</param>
        /// <param name="featureId">The feature id.</param>
        /// <param name="AccessPermissionId">The access permission id.</param>
        /// <param name="EntityId">The entity id.</param>
        public void TryAccess(Modules moduleId, int EntitytypeID, FeatureID featureId, OperationId AccessPermissionId, int EntityId)
        {
            AccessManager.Instance.TryAccess(this, moduleId, EntitytypeID, featureId, AccessPermissionId, EntityId);
        }

           /// <summary>
        /// Delete the global role user.
        /// </summary>
        /// <param name="userid">The userid.</param>
        /// <returns>int</returns>
        public bool DeleteGlobalRoleUser(int userid)
        {
          return  AccessManager.Instance.DeleteGlobalRoleUser(this, userid);
        }

        /// <summary>
        /// Get the global role user By UserID.
        /// </summary>
        /// <param name="userid">The userid.</param>
        /// <returns>int[] </returns>
        public int[] GetGlobalRoleUserByID(int userid)
        {
            return AccessManager.Instance.GetGlobalRoleUserByID(this, userid);
        }

        public bool CheckUserIsAdmin()
        {
            return AccessManager.Instance.CheckUserIsAdmin(this);
        }

        public bool CheckGlobalAccessAvaiilability(int GlobalRoleID, int ModuleID, int FeatureID)
        {
            return AccessManager.Instance.CheckGlobalAccessAvaiilability(this, GlobalRoleID, ModuleID, FeatureID);
        }

        public bool DeleteOldGlobalAccess(int GlobalRoleID, int ModuleID, int FeatureID)
        {
            return AccessManager.Instance.DeleteOldGlobalAccess(this, GlobalRoleID, ModuleID, FeatureID);
        }

        public void GetApplicationLevelSettings()
        {
             AccessManager.Instance.GetApplicationLevelSettings(this);
        }

        public bool InsertUpdateGlobalEntitTypeACL( JArray entitytypeAccessObj)
        {
            return AccessManager.Instance.InsertUpdateGlobalEntitTypeACL(this, entitytypeAccessObj);
        }

        public IList<object> GetGlobalEntityTypeAcl(int roleID, int moduleID)
        {
            return AccessManager.Instance.GetGlobalEntityTypeAcl(this, roleID, moduleID);
        }

        public bool DeleteGlobalEntityTypeACL(int ID)
        {
            return AccessManager.Instance.DeleteGlobalEntityTypeACL(this, ID);
        }

        public IList<IRole> GetAllEntityRole()
        {
            return AccessManager.Instance.GetAllEntityRole(this);
        }

        public IList<IEntityTypeRoleAcl> GetEntityTypeRoleAccess(int EntityTypeID)
        {
            return AccessManager.Instance.GetEntityTypeRoleAccess(this, EntityTypeID);
        }

        public void TryEntityTypeAccess(Modules moduleID, int EntityTypeID)
        {
            AccessManager.Instance.TryEntityTypeAccess(this, moduleID, EntityTypeID);
        }

        public void TryEntityTypeAccess(Modules moduleID, int parententityID, int EntityTypeID)
        {
            AccessManager.Instance.TryEntityTypeAccess(this, moduleID, parententityID, EntityTypeID);
        }

        public void TryEntityTypeAccess(int currententityID, Modules moduleID)
        {
            AccessManager.Instance.TryEntityTypeAccess(this, currententityID, moduleID);
        }

        public bool CheckEntityTypeAccessForRootEntityType(Modules moduleId, int EntitytypeID)
        {
            return AccessManager.Instance.CheckEntityTypeAccessForRootEntityType(this, moduleId, EntitytypeID);
        }

        public bool CheckEntityTypeAndEntityRoleAccess(Modules moduleId,int parententityID, int EntitytypeID)
        {
            return AccessManager.Instance.CheckEntityTypeAndEntityRoleAccess(this, moduleId, parententityID, EntitytypeID);
        }

        public bool CheckEntityRoleAccess(Modules moduleId, int currententityID)
        {
            return AccessManager.Instance.CheckEntityRoleAccess(this, moduleId, currententityID);
        }

    }
}
