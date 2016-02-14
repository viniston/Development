using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrandSystems.Marcom.Core.Interface.Managers;
using BrandSystems.Marcom.Core.Interface;
using BrandSystems.Marcom.Core.Managers.Proxy;
using BrandSystems.Marcom.Core.Access.Interface;
using BrandSystems.Marcom.Dal.Access.Model;
using BrandSystems.Marcom.Core.Access;
using BrandSystems.Marcom.Dal.Base;
using System.Collections;
using BrandSystems.Marcom.Core.Metadata;
using Newtonsoft.Json.Linq;
using BrandSystems.Marcom.Core.Metadata.Interface;
using BrandSystems.Marcom.Dal.Planning.Model;

namespace BrandSystems.Marcom.Core.Managers
{
    internal partial class AccessManager : IManager
    {
        /// <summary>
        /// The instance
        /// </summary>
        private static AccessManager instance = new AccessManager();

        /// <summary>
        /// Initializes the specified marcom manager.
        /// </summary>
        /// <param name="marcomManager">The marcom manager.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void Initialize(IMarcomManager marcomManager)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        internal static AccessManager Instance
        {
            get { return instance; }
        }

        /// <summary>
        /// Commits the caches.
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public void CommitCaches()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Rollbacks the caches.
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public void RollbackCaches()
        {
            throw new NotImplementedException();
        }

        #region Instance of Classes In ServiceLayer reference
        /// <summary>
        /// Returns TaskMember class.
        /// </summary>
        public ITaskMember TaskMemberservice()
        {
            return new TaskMember();
        }


        #endregion


        /// <summary>
        /// Creates the entity user role.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="entityId">The entity id.</param>
        /// <param name="Roleid">The roleid.</param>
        /// <param name="Userid">The userid.</param>
        /// <param name="IsInherited">if set to <c>true</c> [is inherited].</param>
        /// <param name="InheritedFromEntityid">The inherited from entityid.</param>
        /// <returns>IEntityRoleUser</returns>
        public int CreateEntityUserRole(AccessManagerProxy proxy, int entityId, int Roleid, int Userid, bool IsInherited, int InheritedFromEntityid)
        {
            //Check access 
            //Start Transaction

            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    // Business logic of EntityRoleUser
                    EntityRoleUserDao dao = new EntityRoleUserDao();
                    dao.Entityid = entityId;
                    dao.Roleid = Roleid;
                    dao.Userid = Userid;
                    dao.IsInherited = IsInherited;
                    dao.InheritedFromEntityid = InheritedFromEntityid;
                    tx.PersistenceManager.AccessRepository.Save<EntityRoleUserDao>(dao);
                    tx.Commit();
                    IEntityRoleUser EntityRoleUser = new EntityRoleUser();
                    EntityRoleUser.Id = dao.Id;
                    EntityRoleUser.Entityid = dao.Entityid;
                    EntityRoleUser.Roleid = dao.Roleid;
                    EntityRoleUser.Userid = dao.Userid;
                    EntityRoleUser.IsInherited = dao.IsInherited;
                    EntityRoleUser.InheritedFromEntityid = dao.InheritedFromEntityid;

                    return EntityRoleUser.Id;
                }

            }
            catch (Exception ex)
            {
                return 0;
            }

        }

        /// <summary>
        /// Deletes the entity user role.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="entityId">The entity id.</param>
        /// <param name="Roleid">The roleid.</param>
        /// <param name="Userid">The userid.</param>
        /// <returns>IEntityRoleUser</returns>
        public bool DeleteEntityUserRole(AccessManagerProxy proxy, int ID)
        {
            try
            {
                IEntityRoleUser _DeleteEntityUserRole = new EntityRoleUser();
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    EntityRoleUserDao dao = new EntityRoleUserDao();
                    dao.Id = ID;
                    tx.PersistenceManager.AccessRepository.Delete<EntityRoleUserDao>(dao);
                    tx.Commit();
                    _DeleteEntityUserRole.Entityid = dao.Entityid;
                    _DeleteEntityUserRole.Roleid = dao.Roleid;
                    _DeleteEntityUserRole.Userid = dao.Userid;
                }
                return true;
            }
            catch (Exception ex)
            {

            }
            return false;

        }

        /// <summary>
        /// get the role.
        /// </summary>
        /// <returns>Ilist of IRole</returns>
        public IList<IRole> GetRole(AccessManagerProxy proxy)
        {
            try
            {
                IList<IRole> irole = new List<IRole>();
                IList<RoleDao> roledao = new List<RoleDao>();
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    roledao = tx.PersistenceManager.PlanningRepository.GetAll<RoleDao>();
                    int[] EntityRoleIds = { 2, 3, 5 };
                    var result = roledao.Where(at => EntityRoleIds.Contains(at.Id));
                    foreach (var item in result)
                    {
                        IRole role = new Role();
                        role.Id = item.Id;
                        role.Caption = item.Caption;
                        role.Description = item.Description;
                        irole.Add(role);
                    }
                }
                return irole;
            }
            catch
            {

            }
            return null;
        }

        /// <summary>
        /// Saves/Update role.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="Caption">The caption.</param>
        /// <param name="Description">The description.</param>
        /// <param name="Id">The id.</param>
        /// <returns>String</returns>
        public int SaveUpdateRole(AccessManagerProxy proxy, String Caption, String Description, int Id)
        {
            try
            {
                RoleDao Role = new RoleDao();
                IList<RoleDao> RoleList;
                if (Id == 0)
                {
                    Role.Caption = Caption;
                    Role.Description = Description;
                    using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                    {
                        RoleList = tx.PersistenceManager.AccessRepository.GetAll<RoleDao>();
                        tx.Commit();
                    }
                    var x = from t in RoleList where t.Caption == Caption && t.Description == Description select t;
                    if (x.Count() > 0)
                    {
                        Role.Id = x.ElementAt(0).Id;
                    }
                }
                else
                {
                    Role.Id = Id;
                    Role.Caption = Caption;
                    Role.Description = Description;
                }
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    tx.PersistenceManager.AccessRepository.Save<RoleDao>(Role);
                    tx.Commit();
                }
                return Role.Id;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        /// <summary>
        /// Deletes the role.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="Id">The id.</param>
        /// <returns>String</returns>
        public bool DeleteRole(AccessManagerProxy proxy, int Id)
        {
            try
            {
                RoleDao Role = new RoleDao();
                Role.Id = Id;
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    tx.PersistenceManager.AccessRepository.DeleteByID<RoleDao>(Role.Id);
                    tx.Commit();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Entities enum val.
        /// </summary>
        /// <param name="Access">The access.</param>
        /// <returns>int</returns>
        public int EntityEnumVal(string Access)
        {
            int sum = 0;
            string[] words = Access.Split(',');
            foreach (string word in words)
            {
                if (word == "All")
                    sum = 0;
                else if (word == "View")
                    sum = sum + (int)OperationId.View;
                else if (word == "Edit")
                    sum = sum + (int)OperationId.Edit;
                else if (word == "Delete")
                    sum = sum + (int)OperationId.Delete;
                else if (word == "Create")
                    sum = sum + (int)OperationId.Create;
                else if (word == "Self")
                    sum = sum + (int)OperationId.Self;
                else if (word == "Simulate")
                    sum = sum + (int)OperationId.Simulate;
                else if (word == "Allow")
                    sum = sum + (int)OperationId.Allow;
            }
            return sum;
        }

        /// <summary>
        /// Entities enum val.
        /// </summary>
        /// <param name="Access">The access.</param>
        /// <returns>int</returns>
        public string EntityEnumCaption(int Access)
        {
            string sum = "";
            if (Access == 0)
                sum = "All";
            else if (Access == 1)
                sum = "View";
            else if (Access == 2)
                sum = "Edit";
            else if (Access == 4)
                sum = "Delete";
            else if (Access == 8)
                sum = "Create";
            else if (Access == 16)
                sum = "Self";
            else if (Access == 32)
                sum = "Simulate";
            else if (Access == 64)
                sum = "Allow";

            return sum;
        }

        /// <summary>
        /// Saves the update entity ACL.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="RoleId">The role id.</param>
        /// <param name="ModuleId">The module id.</param>
        /// <param name="EntityTypeId">The entity type id.</param>
        /// <param name="FeatureId">The feature id.</param>
        /// <param name="AccessRights">The access rights.</param>
        /// <returns>int</returns>
        public int SaveUpdateEntityACL(AccessManagerProxy proxy, int RoleId, int ModuleId, int EntityTypeId, int FeatureId, string AccessRights)
        {
            IEntityAcl EntityAcl = new EntityAcl();
            try
            {
                int AccessPermission = 0;
                AccessPermission = EntityEnumVal(AccessRights);
                EntityAclDao EntityAclDao = new EntityAclDao();
                //EntityAclDao.Id = EntityAclDao.Id;
                EntityAclDao.Roleid = RoleId;
                EntityAclDao.EntityTypeid = EntityTypeId;
                EntityAclDao.Moduleid = ModuleId;
                EntityAclDao.Featureid = FeatureId;
                EntityAclDao.AccessPermission = AccessPermission;
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    tx.PersistenceManager.AccessRepository.Save<EntityAclDao>(EntityAclDao);
                    tx.Commit();
                }
                EntityAcl.Roleid = RoleId;
                EntityAclDao.Id = EntityAclDao.Id;
                return EntityAcl.Id;
            }
            catch
            {
                return 0;
            }
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
        public IList<object> GetGlobalAcl(AccessManagerProxy proxy, int roleID)
        {
            try
            {
                IList<object> result = new List<object>();
                //IList<IGlobalAcl> _iiGlobalAcl = new List<IGlobalAcl>();
                //IList<GlobalAclDao> _GlobalAclDao = new List<GlobalAclDao>();
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    var distinctroles = tx.PersistenceManager.AccessRepository.ExecuteQuery("SELECT aga.GlobalRoleID,aga.ModuleID,aga.FeatureID FROM AM_GlobalAcl aga WHERE aga.GlobalRoleID = " + roleID + " GROUP BY aga.GlobalRoleID ,aga.ModuleID,aga.FeatureID");
                    foreach (var item in distinctroles)
                    {
                        IList roles = null;
                        IList roleaccess = null;
                        StringBuilder getGlobalAcl = new StringBuilder();
                        //getGlobalAcl.Append(" SELECT agr.Caption AS GlobalRoleCaption,mm.Caption AS ModuleCaption,met.Caption AS EntityTypeCaption,mf.Caption AS FeatureCaption, aga.AccessPermission As AccessPermission,aga.GlobalRoleID,aga.ModuleID,aga.EntityTypeID,aga.FeatureID, aga.ID FROM   AM_GlobalAcl aga INNER JOIN AM_GlobalRole agr ON  agr.ID = aga.GlobalRoleID INNER JOIN MM_Module mm ON  mm.ID = aga.ModuleID INNER JOIN MM_EntityType met ON  met.ID = aga.EntityTypeID INNER JOIN MM_Feature mf ON  mf.ID = aga.FeatureID ");
                        getGlobalAcl.Append("SELECT ");
                        getGlobalAcl.Append("ISNULL(mm.Caption,'All')            AS ModuleCaption, ");
                        getGlobalAcl.Append("ISNULL(met.Caption ,'All')          AS EntityTypeCaption, ");
                        getGlobalAcl.Append("ISNULL(mf.Caption ,'All')           AS FeatureCaption, ");
                        getGlobalAcl.Append("AccessPermission   AS AccessPermission, ");
                        getGlobalAcl.Append("aga.GlobalRoleID, ");
                        getGlobalAcl.Append("aga.ModuleID, ");
                        getGlobalAcl.Append("aga.EntityTypeID, ");
                        getGlobalAcl.Append("aga.FeatureID, ");
                        getGlobalAcl.Append("aga.ID ");
                        getGlobalAcl.Append("FROM   AM_GlobalAcl aga ");
                        getGlobalAcl.Append("LEFT OUTER JOIN AM_GlobalRole agr ");
                        getGlobalAcl.Append("ON  agr.ID = aga.GlobalRoleID ");
                        getGlobalAcl.Append("LEFT OUTER JOIN MM_Module mm ");
                        getGlobalAcl.Append("ON  mm.ID = aga.ModuleID ");
                        getGlobalAcl.Append("LEFT OUTER JOIN MM_EntityType met ");
                        getGlobalAcl.Append("ON  met.ID = aga.EntityTypeID ");
                        getGlobalAcl.Append("LEFT OUTER JOIN MM_Feature mf ");
                        getGlobalAcl.Append("ON  mf.ID = aga.FeatureID");
                        getGlobalAcl.Append(" WHERE aga.GlobalRoleID= ? AND aga.ModuleID= ? AND aga.FeatureID= ?");

                        roles = tx.PersistenceManager.AccessRepository.ExecuteQuerywithMinParam(getGlobalAcl.ToString(), (int)((System.Collections.Hashtable)(item))["GlobalRoleID"], (int)((System.Collections.Hashtable)(item))["ModuleID"], (int)((System.Collections.Hashtable)(item))["FeatureID"]);
                        StringBuilder access = new StringBuilder();
                        StringBuilder ids = new StringBuilder();
                        int i = 1;
                        string AccessPermission = "";

                        for (int m = 0; m < roles.Count; m++)
                        {
                            var item2 = roles[m];
                            AccessPermission = EntityEnumCaption((int)((System.Collections.Hashtable)(item2))["AccessPermission"]);
                            ids.Append((int)((System.Collections.Hashtable)(item2))["ID"]);
                            access.Append(AccessPermission);
                            if (roles.Count > i)
                            {
                                access.Append(", ");
                                ids.Append(",");
                            }
                            i++;
                        }

                        int l = 1;
                        int rolescount = roles.Count;
                        for (int m = 0; m < rolescount; m++)
                        {
                            if (rolescount == l)
                            {

                            }
                            else
                            {
                                roles.RemoveAt(roles.Count - 1);
                                l++;
                            }
                        }


                        //int rolescount = roles.Count;
                        //if (roles.Count > 1)
                        //{
                        //    for (int j = 0; j < rolescount; j++)
                        //    {
                        //        if (j < roles.Count)
                        //        {
                        //            roles.RemoveAt(j);
                        //        }
                        //    }

                        //}
                        foreach (var itemss in roles)
                        {
                            ((System.Collections.Hashtable)(itemss))["AccessPermission"] = access.ToString();
                            ((System.Collections.Hashtable)(itemss))["ID"] = ids.ToString();
                        }
                        foreach (var item123 in roles)
                        {
                            result.Add(item123);
                        }
                        //result.Add(roles);
                    }

                    return result;
                }

            }
            catch { return null; }
        }

        public bool CheckGlobalAccessAvaiilability(AccessManagerProxy proxy, int GlobalRoleID, int ModuleID, int FeatureID)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {

                    IList<GlobalAclDao> globalrole;
                    IList<MultiProperty> prplst = new List<MultiProperty>();
                    prplst.Add(new MultiProperty { propertyName = GlobalAclDao.PropertyNames.GlobalRoleid, propertyValue = GlobalRoleID });
                    prplst.Add(new MultiProperty { propertyName = GlobalAclDao.PropertyNames.Moduleid, propertyValue = ModuleID });
                    prplst.Add(new MultiProperty { propertyName = GlobalAclDao.PropertyNames.Featureid, propertyValue = FeatureID });
                    globalrole = tx.PersistenceManager.AccessRepository.GetEquals<GlobalAclDao>(prplst);
                    if (globalrole.Count() > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch
            {

                return true;
            }

        }

        /// <summary>
        /// Saves/Update global ACL.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="GlobalRoleId">The global role id.</param>
        /// <param name="ModuleId">The module id.</param>
        /// <param name="EntityTypeId">The entity type id.</param>
        /// <param name="FeatureId">The feature id.</param>
        /// <param name="AccessRights">The access rights.</param>
        /// <returns>IGlobalAcl</returns>
        public bool SaveUpdateGlobalACL(AccessManagerProxy proxy, int GlobalRoleId, int ModuleId, int EntityTypeId, int FeatureId, string AccessRights, int ID = 0)
        {
            IGlobalAcl GlobalAcl = new GlobalAcl();
            try
            {

                int AccessPermission = 0;
                AccessPermission = EntityEnumVal(AccessRights);
                GlobalAclDao GlobalAclDao = new GlobalAclDao();
                GlobalAclDao.Id = ID;
                GlobalAclDao.GlobalRoleid = GlobalRoleId;
                GlobalAclDao.Moduleid = ModuleId;
                GlobalAclDao.EntityTypeid = EntityTypeId;
                GlobalAclDao.Featureid = FeatureId;
                GlobalAclDao.AccessPermission = true;
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    tx.PersistenceManager.AccessRepository.Save<GlobalAclDao>(GlobalAclDao);
                    tx.Commit();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteOldGlobalAccess(AccessManagerProxy proxy, int GlobalRoleID, int ModuleID, int FeatureID)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    tx.PersistenceManager.AccessRepository.ExecuteQuerywithMinParam("delete from AM_GlobalAcl where GlobalRoleID = ? and ModuleID=  ? and FeatureID =  ? ", GlobalRoleID, ModuleID, FeatureID);
                    tx.Commit();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// Delete global ACL.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="ID">ID</param>
        /// <returns>IGlobalAcl</returns>
        public bool DeleteGlobalACL(AccessManagerProxy proxy, int ID)
        {
            IGlobalAcl GlobalAcl = new GlobalAcl();
            try
            {

                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    tx.PersistenceManager.AccessRepository.DeleteByID<GlobalAclDao>(GlobalAclDao.PropertyNames.Id, ID);
                    tx.Commit();
                    return true;
                }
            }
            catch
            {
            }
            return false;
        }

        //GetGlobalRole
        /// <summary>
        /// get the global role.
        /// </summary>
        /// <param name="caption">The caption.</param>
        /// <param name="description">The description.</param>
        /// <param name="roleid">The roleid.</param>
        /// <returns>Ilist of GlobalRole</returns>
        public IList<IGlobalRole> GetGlobalRole(AccessManagerProxy proxy)
        {
            try
            {
                IList<IGlobalRole> _iiGlobalRole = new List<IGlobalRole>();
                IList<GlobalRoleDao> _GlobalRoleDao = new List<GlobalRoleDao>();
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    _GlobalRoleDao = tx.PersistenceManager.AccessRepository.GetAll<GlobalRoleDao>();

                }
                foreach (var item in _GlobalRoleDao)
                {
                    IGlobalRole _iglobalrole = new GlobalRole();
                    _iglobalrole.Id = item.Id;
                    _iglobalrole.Caption = item.Caption;
                    _iglobalrole.Description = item.Description;
                    _iglobalrole.IsAssetAccess = item.IsAssetAccess;
                    _iiGlobalRole.Add(_iglobalrole);
                }
                return _iiGlobalRole;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //GetGlobalRole By ID
        /// <summary>
        /// get the global role by id.
        /// </summary>
        /// <param name="caption">The caption.</param>
        /// <param name="description">The description.</param>
        /// <param name="roleid">The roleid.</param>
        /// <returns>Ilist of GlobalRole</returns>
        public IList<IGlobalRole> GetGlobalRoleByID(AccessManagerProxy proxy, int ID)
        {
            try
            {
                GlobalRoleDao GlobalRoleDao = new GlobalRoleDao();
                GlobalRoleDao.Id = ID;
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    GlobalRoleDao = tx.PersistenceManager.AccessRepository.Get<GlobalRoleDao>(GlobalRoleDao);
                }
                IList<IGlobalRole> _iiglobalrole = new List<IGlobalRole>();
                IGlobalRole _iGlobalRole = new GlobalRole();
                _iGlobalRole.Id = GlobalRoleDao.Id;
                _iGlobalRole.Caption = GlobalRoleDao.Caption;
                _iGlobalRole.Description = GlobalRoleDao.Description;
                _iiglobalrole.Add(_iGlobalRole);
                return _iiglobalrole;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        //insert/update global role 
        /// <summary>
        /// Inserts/Update global role.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="description">The description.</param>
        /// <param name="roleid">The roleid.</param>
        /// <returns>int</returns>
        public bool InsertUpdateGlobalRole(AccessManagerProxy proxy, string caption, string description, int ID = 0)
        {
            try
            {
                GlobalRoleDao dao = new GlobalRoleDao();
                dao.Caption = caption;
                dao.Description = description;
                if (ID != 0)
                {
                    dao.Id = ID;
                }
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    tx.PersistenceManager.AccessRepository.Save<GlobalRoleDao>(dao);
                    tx.Commit();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Deletes the global role.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="roleid">The roleid.</param>
        /// <returns>string</returns>
        public int DeleteGlobalRole(AccessManagerProxy proxy, int roleid)
        {
            try
            {
                GlobalRoleDao dao = new GlobalRoleDao();
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    var ischeckGlobalAccess = (from item in tx.PersistenceManager.AccessRepository.Query<GlobalAclDao>() where item.GlobalRoleid == roleid select item);
                    if (ischeckGlobalAccess.Count() > 0)
                        return 2;

                    //dao = tx.PersistenceManager.AccessRepository.Get<GlobalRoleDao>(roleid);
                    //tx.Commit();
                }
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    tx.PersistenceManager.AccessRepository.DeleteByID<GlobalRoleDao>(GlobalRoleDao.PropertyNames.Id, roleid);
                    tx.Commit();
                }
                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        /// <summary>
        /// Inserts the global role user.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="globalroleid">The globalroleid.</param>
        /// <param name="userid">The userid.</param>
        /// <returns>int</returns>
        public int InsertGlobalRoleUser(AccessManagerProxy proxy, int globalroleid, int userid)
        {
            //return null;
            try
            {
                if ((CheckAccess(proxy, Modules.Admin, 4, FeatureID.ListView, OperationId.Self)) == true)
                {
                    GlobalRoleUserDao dao = new GlobalRoleUserDao();
                    dao.GlobalRoleId = globalroleid;
                    dao.Userid = userid;
                    using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                    {
                        tx.PersistenceManager.AccessRepository.Save<GlobalRoleUserDao>(dao);
                        tx.Commit();
                    }

                    return dao.Id;
                }
                return 0;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        /// <summary>
        /// Delete the global role user.
        /// </summary>
        /// <param name="userid">The userid.</param>
        /// <returns>bool</returns>
        public bool DeleteGlobalRoleUser(AccessManagerProxy proxy, int userid)
        {
            //return null;
            try
            {
                if ((CheckAccess(proxy, Modules.Admin, 4, FeatureID.ListView, OperationId.Self)) == true)
                {

                    using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                    {
                        tx.PersistenceManager.AccessRepository.DeleteByID<GlobalRoleUserDao>(GlobalRoleUserDao.PropertyNames.Userid, userid);
                        tx.Commit();
                        return true;
                    }


                }

            }
            catch (Exception ex)
            {
                return false;
            }
            return false;
        }



        /// <summary>
        /// Get the global role user By UserID.
        /// </summary>
        /// <param name="userid">The userid.</param>
        /// <returns>List<int></returns>
        public int[] GetGlobalRoleUserByID(AccessManagerProxy proxy, int userid)
        {
            //return null;
            try
            {
                if ((CheckAccess(proxy, Modules.Admin, 4, FeatureID.ListView, OperationId.Self)) == true)
                {

                    using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                    {
                        var GlobalRolelist = tx.PersistenceManager.AccessRepository.Query<GlobalRoleUserDao>();
                        var roleList = from t in GlobalRolelist where t.Userid == userid select t;
                        int[] GlobalRoleId = new int[roleList.ToList().Count()];
                        for (var i = 0; i < roleList.ToList().Count(); i++)
                        {
                            GlobalRoleId[i] = roleList.ToList().ElementAt(i).GlobalRoleId;
                        }
                        //foreach (var val in roleList)
                        //{
                        //    GlobalRoleId.Add( val.Userid);
                        //}

                        tx.Commit();
                        return GlobalRoleId;

                    }


                }

            }
            catch (Exception ex)
            {
                return null;
            }
            return null;
        }



        public bool CheckUserIsAdmin(AccessManagerProxy proxy)
        {
            //return null;
            try
            {

                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    IList<MultiProperty> prpList = new List<MultiProperty>();
                    prpList.Add(new MultiProperty { propertyName = GlobalRoleUserDao.PropertyNames.Userid, propertyValue = proxy.MarcomManager.User.Id });
                    prpList.Add(new MultiProperty { propertyName = GlobalRoleUserDao.PropertyNames.GlobalRoleId, propertyValue = 1 });
                    var GlobalRolelist = tx.PersistenceManager.AccessRepository.GetEquals<GlobalRoleUserDao>(prpList);



                    tx.Commit();
                    if (GlobalRolelist.ToList().Count() > 0)
                        return true;

                }




            }
            catch (Exception ex)
            {
                return false;
            }
            return false;
        }


        // Check access for a global type
        /// <summary>
        /// Checks the access.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="moduleId">The module id.</param>
        /// <param name="EntitytypeID">The entitytype ID.</param>
        /// <param name="featureId">The feature id.</param>
        /// <param name="AccessPermissionId">The access permission id.</param>
        /// <returns>bool</returns>
        public bool CheckAccess(AccessManagerProxy proxy, Modules moduleId, int EntitytypeID, FeatureID featureId, OperationId AccessPermissionId)
        {
            try
            {
                IList<GlobalRoleUserDao> globalRoleUser;
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    globalRoleUser = tx.PersistenceManager.AccessRepository.GetEquals<GlobalRoleUserDao>(GlobalRoleUserDao.PropertyNames.Userid, proxy.MarcomManager.User.Id);
                    IList<GlobalAclDao> globalAcl;
                    globalAcl = tx.PersistenceManager.AccessRepository.GetAll<GlobalAclDao>();
                    var newx = from newt in globalRoleUser.ToList()
                               join newz in globalAcl on newt.GlobalRoleId equals newz.GlobalRoleid into leftjoin
                               from fnresult in leftjoin.DefaultIfEmpty()
                               select fnresult;
                    //foreach (var sval in newx.ToList())
                    //{
                    //    if (sval.Moduleid == (int)moduleId && sval.EntityTypeid == EntitytypeID && (sval.Featureid == (int)featureId) && ((sval.AccessPermission & (int)AccessPermissionId) != 0))
                    //    {
                    //        return true;
                    //    }
                    //}

                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {

                return true;

            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="proxy"></param>
        /// <param name="moduleId"></param>
        /// <param name="featureId"></param>
        /// <param name="AccessPermissionId"></param>
        /// <returns></returns>
        public bool CheckUserAccess(AccessManagerProxy proxy, Modules moduleId, FeatureID featureId, OperationId AccessPermissionId)
        {
            try
            {
                bool stat = proxy.MarcomManager.User.ListOfUserGlobalRoles.Any(tt => tt.Moduleid == (int)moduleId && tt.Featureid == (int)featureId && (tt.AccessPermission == true));
                return stat;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="proxy"></param>
        /// <param name="moduleId"></param>
        /// <param name="featureId"></param>
        /// <returns></returns>
        public bool CheckUserAccess(AccessManagerProxy proxy, Modules moduleId, FeatureID featureId)
        {
            try
            {
                bool stat = proxy.MarcomManager.User.ListOfUserGlobalRoles.Any(tt => tt.Moduleid == (int)moduleId && tt.Featureid == (int)featureId && tt.AccessPermission == true);
                return stat;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public void GetApplicationLevelSettings(AccessManagerProxy metadataManagerProxy)
        {
            //IMarcomManager metadataManagerProxy = MarcomManagerFactory.GetMarcomManager(GetSystemSession());
            using (ITransaction tx = metadataManagerProxy.MarcomManager.GetTransaction())
            {
                string qry = "select mm.ID as 'moduleid',mm.Caption as 'modulecaption',mf.Caption as 'featurecaption',mf.ID as 'featureid' from MM_Module mm inner join MM_Feature mf on  mm.ID=mf.ModuleID where mm.IsEnable=1 and mf.IsEnable=1";
                var result = tx.PersistenceManager.MetadataRepository.ExecuteQuery(qry).Cast<Hashtable>().ToList();
                MarcomManagerFactory.ListOfModuleFeature = new List<ModuleFeature>();
                foreach (var c in result)
                {
                    ModuleFeature mf = new ModuleFeature();
                    mf.Featureid = Convert.ToInt32(c["featureid"]);
                    mf.ModuleId = Convert.ToInt32(c["moduleid"]);
                    mf.ModuleCaption = Convert.ToString(c["modulecaption"]);
                    mf.FeatureCaption = Convert.ToString(c["featurecaption"]);
                    MarcomManagerFactory.ListOfModuleFeature.Add(mf);
                }
            }
        }

        // Try access for a global type
        /// <summary>
        /// Tries the access.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="moduleId">The module id.</param>
        /// <param name="EntitytypeID">The entitytype ID.</param>
        /// <param name="featureId">The feature id.</param>
        /// <param name="AccessPermissionId">The access permission id.</param>
        /// <exception cref="System.Exception">Invalid operation</exception>
        public void TryAccess(AccessManagerProxy proxy, Modules moduleId, int EntitytypeID, FeatureID featureId, OperationId AccessPermissionId)
        {
            if (CheckAccess(proxy, moduleId, EntitytypeID, featureId, AccessPermissionId) == false)
            {
                throw new Exception("Invalid operation");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="proxy"></param>
        /// <param name="moduleId"></param>
        /// <param name="featureId"></param>
        /// <param name="AccessPermissionId"></param>
        public void TryAccess(AccessManagerProxy proxy, Modules moduleId, FeatureID featureId, OperationId AccessPermissionId)
        {
            if (CheckUserAccess(proxy, moduleId, featureId, AccessPermissionId) == false)
            {
                throw new MarcomAccessDeniedException("Access denied");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="proxy"></param>
        /// <param name="moduleId"></param>
        /// <param name="featureId"></param>
        public void TryAccess(AccessManagerProxy proxy, Modules moduleId, FeatureID featureId)
        {
            if (CheckUserAccess(proxy, moduleId, featureId) == false)
            {
                throw new MarcomAccessDeniedException("Access denied");
            }
        }

        // Check access for a local type
        /// <summary>
        /// Checks the access.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="moduleId">The module id.</param>
        /// <param name="EntitytypeID">The entitytype ID.</param>
        /// <param name="featureId">The feature id.</param>
        /// <param name="AccessPermissionId">The access permission id.</param>
        /// <param name="EntityId">The entity id.</param>
        /// <returns>bool</returns>
        public bool CheckAccess(AccessManagerProxy proxy, Modules moduleId, int EntitytypeID, FeatureID featureId, OperationId AccessPermissionId, int EntityId)
        {
            try
            {
                IList<EntityRoleUserDao> entityRoleUser;
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    IList<MultiProperty> prplst = new List<MultiProperty>();
                    prplst.Add(new MultiProperty { propertyName = EntityRoleUserDao.PropertyNames.Entityid, propertyValue = EntityId });
                    prplst.Add(new MultiProperty { propertyName = EntityRoleUserDao.PropertyNames.Userid, propertyValue = proxy.MarcomManager.User.Id });
                    entityRoleUser = tx.PersistenceManager.AccessRepository.GetEquals<EntityRoleUserDao>(prplst);
                    IList<EntityAclDao> entityAcl;
                    entityAcl = tx.PersistenceManager.AccessRepository.GetAll<EntityAclDao>();
                    var newx = from newt in entityRoleUser.ToList()
                               join newz in entityAcl on newt.Roleid equals newz.Roleid into leftjoin
                               from fnresult in leftjoin.DefaultIfEmpty()
                               select fnresult;
                    foreach (var sval in newx.ToList())
                    {
                        if (sval.Moduleid == (int)moduleId && sval.EntityTypeid == EntitytypeID && ((sval.Featureid & (int)featureId) != 0) && ((sval.AccessPermission & (int)AccessPermissionId) != 0))
                        {
                            return true;
                        }
                    }
                }
            }
            catch
            {
                return false;
            }

            return false;
        }

        // Try access for a local type
        /// <summary>
        /// Tries the access.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="moduleId">The module id.</param>
        /// <param name="EntitytypeID">The entitytype ID.</param>
        /// <param name="featureId">The feature id.</param>
        /// <param name="AccessPermissionId">The access permission id.</param>
        /// <param name="EntityId">The entity id.</param>
        /// <exception cref="System.Exception">Invalid operation</exception>
        public void TryAccess(AccessManagerProxy proxy, Modules moduleId, int EntitytypeID, FeatureID featureId, OperationId AccessPermissionId, int EntityId)
        {
            if (CheckAccess(proxy, moduleId, EntitytypeID, featureId, AccessPermissionId, EntityId) == false)
            {
                throw new Exception("Invalid operation");
            }
        }

        public bool InsertUpdateGlobalEntitTypeACL(AccessManagerProxy proxy, JArray entitytypeAccessObj)
        {
            try
            {
                bool Isdeleted = false;
                IList<GlobalEntityTypeAccessDao> iiglobalentitytypeaccess = new List<GlobalEntityTypeAccessDao>();
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    int globalroleid = 0, moduleid = 0;
                    foreach (var obj in entitytypeAccessObj)
                    {
                        if (!Isdeleted)
                        {
                            globalroleid = (int)obj["GlobalRoleID"];
                            moduleid = (int)obj["ModuleID"];
                            tx.PersistenceManager.AccessRepository.ExecuteQuerywithMinParam("delete from AM_GlobalEntityTypeAcl where GlobalRoleID = ? and ModuleID=  ? ", globalroleid, moduleid);
                            Isdeleted = true;
                        }
                        if ((bool)obj["AccessPermission"])
                        {
                            GlobalEntityTypeAccessDao GlobalAclDao = new GlobalEntityTypeAccessDao();
                            GlobalAclDao.Id = 0;
                            GlobalAclDao.GlobalRoleid = (int)obj["GlobalRoleID"];
                            GlobalAclDao.Moduleid = (int)obj["ModuleID"];
                            GlobalAclDao.EntityTypeid = (int)obj["EntityTypeID"];
                            GlobalAclDao.AccessPermission = (bool)obj["AccessPermission"];
                            iiglobalentitytypeaccess.Add(GlobalAclDao);
                        }
                    }

                    tx.PersistenceManager.AccessRepository.Save<GlobalEntityTypeAccessDao>(iiglobalentitytypeaccess);
                    tx.Commit();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public IList<object> GetGlobalEntityTypeAcl(AccessManagerProxy proxy, int roleID, int moduleID = 3)
        {
            try
            {
                IList<object> result = new List<object>();
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {

                    IList<IEntityType> ientitytype = new List<IEntityType>();
                    ientitytype = proxy.MarcomManager.MetadataManager.GetEntityTypeIsAssociate();

                    string entitytypeIDs = "";

                    foreach (var obj in ientitytype)
                    {
                        entitytypeIDs += obj.Id.ToString() + ",";
                    }

                    IList roles = null;
                    StringBuilder getGlobalAcl = new StringBuilder();

                    getGlobalAcl.Append("SELECT tempentitytype.Caption, ISNULL(ageta.AccessPermission,0)AccessPermission, ");
                    getGlobalAcl.Append("ISNULL(ageta.GlobalRoleID, " + roleID + ")GlobalRoleID, ISNULL(ageta.ModuleID, " + moduleID + ")ModuleID, tempentitytype.ID AS EntityTypeID, ");
                    getGlobalAcl.Append("tempentitytype.ShortDescription, tempentitytype.ColorCode, ISNULL(ageta.ID,0)ID ");
                    getGlobalAcl.Append("FROM ( ");
                    getGlobalAcl.Append("SELECT * FROM MM_EntityType met ");
                    getGlobalAcl.Append("WHERE met.ID IN (" + entitytypeIDs.TrimEnd(',') + "))tempentitytype ");
                    getGlobalAcl.Append("LEFT JOIN  AM_GlobalEntityTypeAcl ageta ");
                    getGlobalAcl.Append("ON ageta.EntityTypeID = tempentitytype.ID ");

                    getGlobalAcl.Append("AND ageta.ModuleID = " + moduleID + " AND ageta.GlobalRoleID = " + roleID + " ");
                    getGlobalAcl.Append("LEFT JOIN AM_GlobalRole agr ON ageta.GlobalRoleID = agr.ID ");
                    getGlobalAcl.Append("LEFT JOIN MM_Module mm ON ageta.ModuleID = mm.ID ");

                    roles = tx.PersistenceManager.AccessRepository.ExecuteQuerywithMinParam(getGlobalAcl.ToString());

                    foreach (var item123 in roles)
                    {
                        result.Add(item123);
                    }

                    return result;
                }

            }
            catch { return null; }
        }

        public bool DeleteGlobalEntityTypeACL(AccessManagerProxy proxy, int ID)
        {
            IGlobalAcl GlobalAcl = new GlobalAcl();
            try
            {

                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    tx.PersistenceManager.AccessRepository.DeleteByID<GlobalEntityTypeAccessDao>(GlobalEntityTypeAccessDao.PropertyNames.Id, ID);
                    tx.Commit();
                    return true;
                }
            }
            catch
            {
            }
            return false;
        }

        /// <summary>
        /// get the role.
        /// </summary>
        /// <returns>Ilist of IRole</returns>
        public IList<IRole> GetAllEntityRole(AccessManagerProxy proxy)
        {
            try
            {
                IList<IRole> irole = new List<IRole>();
                IList<RoleDao> roledao = new List<RoleDao>();
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    roledao = tx.PersistenceManager.PlanningRepository.Query<RoleDao>().ToList();
                    int[] EntityRoleIds = { (int)EntityRoles.Owner, (int)EntityRoles.Editer, (int)EntityRoles.Viewer, (int)EntityRoles.ExternalParticipate, (int)EntityRoles.BudgerApprover };
                    var result = roledao.Where(at => EntityRoleIds.Contains(at.Id));
                    foreach (var item in result)
                    {
                        IRole role = new Role();
                        role.Id = item.Id;
                        role.Caption = item.Caption;
                        role.Description = item.Description;
                        irole.Add(role);
                    }
                }
                return irole;
            }
            catch
            {

            }
            return null;
        }

        public IList<IEntityTypeRoleAcl> GetEntityTypeRoleAccess(AccessManagerProxy proxy, int EntityTypeID)
        {
            try
            {

                IList<IEntityTypeRoleAcl> _iientityTyperoleobj = new List<IEntityTypeRoleAcl>();
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    IList<EntityTypeRoleAclDao> dao = new List<EntityTypeRoleAclDao>();
                    int[] FilterIds = { (int)EntityRoles.Owner, (int)EntityRoles.BudgerApprover };
                    var entityroleobj = tx.PersistenceManager.AccessRepository.Query<EntityTypeRoleAclDao>().Where(a => a.EntityTypeID == EntityTypeID && !FilterIds.Contains(a.EntityRoleID)).OrderBy(a => a.Sortorder);
                    foreach (var item in entityroleobj)
                    {
                        EntityTypeRoleAcl entitytypeRoleAclObj = new EntityTypeRoleAcl();
                        entitytypeRoleAclObj.ID = item.ID;
                        entitytypeRoleAclObj.EntityTypeID = item.EntityTypeID;
                        entitytypeRoleAclObj.EntityRoleID = item.EntityRoleID;
                        entitytypeRoleAclObj.ModuleID = item.ModuleID;
                        entitytypeRoleAclObj.Sortorder = item.Sortorder;
                        entitytypeRoleAclObj.Caption = item.Caption;
                        _iientityTyperoleobj.Add(entitytypeRoleAclObj);
                    }
                }
                return _iientityTyperoleobj;

            }
            catch
            {


            }
            return null;
        }

        /// <summary>
        /// CHECK THE ENTITY TYPE ACCESS
        /// </summary>
        /// <param name="moduleID"></param>
        /// <param name="EntityTypeID"></param>
        public void TryEntityTypeAccess(AccessManagerProxy proxy, Modules moduleID, int EntityTypeID)
        {
            if (!CheckEntityTypeAccessForRootEntityType(proxy, moduleID, EntityTypeID))
            {
                throw new MarcomAccessDeniedException("Access denied");
            }
        }

        /// <summary>
        /// ENTITY TYPE ACCESS
        /// </summary>
        /// <param name="moduleId"></param>
        /// <param name="EntitytypeID"></param>
        /// <returns></returns>
        public bool CheckEntityTypeAccessForRootEntityType(AccessManagerProxy proxy, Modules moduleId, int EntitytypeID)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    StringBuilder str = new StringBuilder();

                    str.Append("SELECT COUNT(*) AS AccessExist ");
                    str.Append("FROM   AM_GlobalEntityTypeAcl ageta INNER JOIN MM_Module mm ON  mm.ID = ageta.ModuleID  ");
                    str.Append("AND mm.IsEnable = 1 WHERE mm.ID = " + (int)moduleId + " AND ageta.EntityTypeID = " + EntitytypeID + "  AND  ageta.GlobalRoleID IN (SELECT GlobalRoleID FROM  ");
                    str.Append("AM_GlobalRole_User WHERE  UserId = " + proxy.MarcomManager.User.Id + ") ");

                    var result = tx.PersistenceManager.MetadataRepository.ExecuteQuery(str.ToString()).Cast<Hashtable>().ToList().FirstOrDefault();
                    tx.Commit();

                    if (result["AccessExist"] != null && (int)result["AccessExist"] > 0)
                        return true;


                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// ENTITY TYPE AND ENTITY ACCESS
        /// </summary>
        /// <param name="moduleID"></param>
        /// <param name="parententityID"></param>
        /// <param name="EntityTypeID"></param>
        public void TryEntityTypeAccess(AccessManagerProxy proxy, Modules moduleID, int parententityID, int EntityTypeID)
        {
            if (!CheckEntityTypeAndEntityRoleAccess(proxy, moduleID, parententityID, EntityTypeID))
            {
                throw new MarcomAccessDeniedException("Access denied");
            }
        }

        public bool CheckEntityTypeAndEntityRoleAccess(AccessManagerProxy proxy, Modules moduleId, int parententityID, int EntitytypeID)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    StringBuilder str = new StringBuilder();

                    str.Append(" SELECT MIN(aetra.EntityRoleID) as Permission  FROM AM_Entity_Role_User aeru ");
                    str.Append("INNER JOIN AM_EntityTypeRoleAcl aetra ON aeru.RoleID=aetra.ID ");
                    str.Append("WHERE aeru.EntityID= " + parententityID + " AND aeru.UserID = " + proxy.MarcomManager.User.Id + " ");

                    var res = tx.PersistenceManager.MetadataRepository.ExecuteQuery(str.ToString()).Cast<Hashtable>().ToList();
                    if (res[0]["Permission"] != null && Convert.ToInt32(res[0]["Permission"]) <= 2)
                        return true;
                    else
                    {
                        str.Clear();
                        str.Append("SELECT COUNT(*) AS AccessExist ");
                        str.Append("FROM   AM_GlobalEntityTypeAcl ageta INNER JOIN MM_Module mm ON  mm.ID = ageta.ModuleID ");
                        str.Append("AND mm.IsEnable = 1 INNER JOIN MM_EntityType met ON  met.ID = ageta.EntityTypeID ");
                        str.Append("WHERE ageta.EntityTypeID = " + EntitytypeID + " AND mm.ID = " + (int)moduleId + "  AND  ageta.GlobalRoleID IN (SELECT GlobalRoleID FROM   AM_GlobalRole_User WHERE  UserId = " + proxy.MarcomManager.User.Id + ") ");

                        var result = tx.PersistenceManager.MetadataRepository.ExecuteQuery(str.ToString()).Cast<Hashtable>().ToList().FirstOrDefault();

                        if ((int)result["AccessExist"] > 0)
                            return true;
                    }
                    tx.Commit();
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// ENTITY ACCESSs
        /// </summary>
        /// <param name="currententityID"></param>
        /// <param name="moduleID"></param>
        public void TryEntityTypeAccess(AccessManagerProxy proxy, int currententityID, Modules moduleID)
        {


            if (!CheckEntityRoleAccess(proxy, moduleID, currententityID))
            {
                throw new MarcomAccessDeniedException("Access denied");
            }

        }

        public bool CheckEntityRoleAccess(AccessManagerProxy proxy, Modules moduleId, int currententityID)
        {
            try
            {
                int isGlobalAdmin = proxy.MarcomManager.User.ListOfUserGlobalRoles.Where(role => role.Id == 1).Count();
                if (isGlobalAdmin > 0)
                    return true;
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    StringBuilder str = new StringBuilder();
                    str.Append(" DECLARE @TestVal int  ");
                    str.Append(" SET @TestVal = (SELECT pe.TypeID FROM PM_Entity pe WHERE pe.ID = ?)  ");
                    str.Append(" SELECT   ");
                    str.Append(" CASE @TestVal  ");
                    str.Append(" WHEN 7 THEN  ");
                    str.Append(" (SELECT MIN(ptm.RoleID) as Permission    ");
                    str.Append(" FROM PM_Task_Members ptm WHERE ptm.TaskID = ? AND ptm.UserID = ?)  ");
                    str.Append(" WHEN 30 THEN  ");
                    str.Append(" (SELECT MIN(ttm.RoleID) as Permission   ");
                    str.Append(" FROM TM_Task_Members ttm WHERE ttm.TaskID = ? AND ttm.UserID = ?)  ");
                    str.Append(" ELSE        ");
                    str.Append(" (SELECT MIN(aetra.EntityRoleID) as Permission   FROM AM_Entity_Role_User aeru  ");
                    str.Append(" INNER JOIN AM_EntityTypeRoleAcl aetra ON aeru.RoleID=aetra.ID   ");
                    str.Append(" WHERE aeru.EntityID= ? AND aeru.UserID = ?)  ");
                    str.Append(" END   AS 'Permission' , @TestVal etype  ");
                    var res = tx.PersistenceManager.MetadataRepository.ExecuteQuerywithMinParam(str.ToString(), currententityID, currententityID, proxy.MarcomManager.User.Id, currententityID, proxy.MarcomManager.User.Id, currententityID, proxy.MarcomManager.User.Id).Cast<Hashtable>().ToList();
                    tx.Commit();
                    if ((int)res[0]["etype"] != 7)
                    {
                        if (res[0]["Permission"] != null && Convert.ToInt32(res[0]["Permission"]) <= 2)
                            return true;
                    }
                    else 
                    {
                        if (res[0]["Permission"] != null && Convert.ToInt32(res[0]["Permission"]) <= 4)
                            return true;
                    }
                        

                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}
