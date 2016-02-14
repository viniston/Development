using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrandSystems.Marcom.Core.Interface;
using BrandSystems.Marcom.Core.Common;
using BrandSystems.Marcom.Core.Common.Interface;
using BrandSystems.Marcom.Core.Managers.Proxy;
using BrandSystems.Marcom.Dal.Common.Model;
using Newtonsoft.Json.Linq;
using BrandSystems.Marcom.Dal.Access.Model;
using BrandSystems.Marcom.Dal.Base;
using Newtonsoft.Json;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Dynamic;
using System.Text.RegularExpressions;
using System.IO;
using System.Drawing;
using System.Web;
using System.Configuration;
using BrandSystems.Marcom.Dal.Planning.Model;
using System.Collections;
using BrandSystems.Marcom.Core.Metadata.Interface;
using BrandSystems.Marcom.Dal.Metadata.Model;
using BrandSystems.Marcom.Dal.User.Model;
using Mail;
using BrandSystems.Marcom.Core.Planning.Interface;
using BrandSystems.Marcom.Core.Planning;
using BrandSystems.Marcom.Core.Task.Interface;
using BrandSystems.Marcom.Core.Task;
using BrandSystems.Marcom.Dal.Task.Model;
using BrandSystems.Marcom.Core.Utility;
using BrandSystems.Marcom.Core.Access;
using System.Threading.Tasks;
using BrandSystems.Marcom.Core.Report.Interface;
using BrandSystems.Marcom.Core.Metadata;
using MSUtil;
using UpgradeTool;
using System.Xml.Serialization;
using System.Net;
using BrandSystems.Marcom.Dal.DAM.Model;
using System.Data;

namespace BrandSystems.Marcom.Core.Managers
{
    internal partial class CommonManager : IManager
    {

        /// <summary>
        /// The instance
        /// </summary>
        private static CommonManager instance = new CommonManager();

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        internal static CommonManager Instance
        {
            get { return instance; }
        }

        /// <summary>
        /// Initializes the specified marcom manager.
        /// </summary>
        /// <param name="marcomManager">The marcom manager.</param>
        void IManager.Initialize(IMarcomManager marcomManager)
        {
            // Cache and initialize things here...
        }

        /// <summary>
        /// Commit all caches since the transaction has been commited.
        /// </summary>
        void IManager.CommitCaches()
        {
        }

        /// <summary>
        /// Rollback all caches since the transaction has been rollbacked.
        /// </summary>
        void IManager.RollbackCaches()
        {
        }

        #region Instance of Classes In ServiceLayer reference
        /// <summary>
        /// Returns TaskMember class.
        /// </summary>
        public IFile Fileservice()
        {
            return new BrandSystems.Marcom.Core.Common.File();
        }


        #endregion

        /// <summary>
        /// Initializes the type of the isubscription.
        /// </summary>
        /// <param name="strBody">The STR body.</param>
        /// <returns></returns>
        public ISubscriptionType initializeIsubscriptionType(string strBody)
        {
            JObject jobj = JObject.Parse(strBody.ToUpper());
            ISubscriptionType subscriptionType = new SubscriptionType();
            subscriptionType.Caption = jobj["CAPTION"] == null ? "" : (string)jobj["CAPTION"];
            subscriptionType.Id = jobj["ID"] == null ? 0 : (int)jobj["ID"];
            return subscriptionType;
        }

        /// <summary>
        /// Initializes the inavigation.
        /// </summary>
        /// <param name="strBody">The STR body.</param>
        /// <returns>INavigation</returns>
        public INavigation initializeInavigation(string strBody)
        {
            JObject jobj = JObject.Parse(strBody.ToUpper());
            INavigation navigation = new Navigation();
            navigation.AddUserEmail = jobj["ADDUSEREMAIL"] == null ? false : (bool)jobj["ADDUSEREMAIL"];
            navigation.AddUserName = jobj["ADDUSERNAME"] == null ? false : (bool)jobj["ADDUSERNAME"];
            navigation.Caption = jobj["CAPTION"] == null ? "" : (string)jobj["CAPTION"];
            navigation.Description = jobj["DESCRIPTION"] == null ? "" : (string)jobj["DESCRIPTION"];
            navigation.Featureid = jobj["FEATUREID"] == null ? 0 : (int)jobj["FEATUREID"];
            navigation.Id = jobj["ID"] == null ? 0 : (int)jobj["ID"];
            navigation.Imageurl = jobj["IMAGEURL"] == null ? "" : (string)jobj["IMAGEURL"];
            navigation.IsActive = jobj["ISACTIVE"] == null ? false : (bool)jobj["ISACTIVE"];
            navigation.IsDynamicPage = jobj["ISDYNAMICPAGE"] == null ? false : (bool)jobj["ISDYNAMICPAGE"];
            navigation.IsExternal = jobj["ISEXTERNAL"] == null ? false : (bool)jobj["ISEXTERNAL"];
            navigation.IsIframe = jobj["CAPTION"] == null ? false : (bool)jobj["ISIFRAME"];
            navigation.IsPopup = jobj["ISPOPUP"] == null ? false : (bool)jobj["ISPOPUP"];
            navigation.JavaScript = jobj["JAVASCRIPT"] == null ? "" : (string)jobj["JAVASCRIPT"];
            navigation.Moduleid = jobj["MODULEID"] == null ? 0 : (int)jobj["MODULEID"];
            navigation.Parentid = jobj["PARENTID"] == null ? 0 : (int)jobj["PARENTID"];
            navigation.Url = jobj["URL"] == null ? "" : (string)jobj["URL"];
            return navigation;
        }

        /// <summary>
        /// Initializes the I user mail subscription.
        /// </summary>
        /// <param name="strBody">The STR body.</param>
        /// <returns>IUserMailSubscription</returns>
        public IUserMailSubscription initializeIUserMailSubscription(string strBody)
        {

            try
            {
                JObject jobj = JObject.Parse(strBody.ToUpper());
                IUserMailSubscription userMailSubscription = new UserMailSubscription();
                userMailSubscription.DayName = jobj["DAYNAME"] == null ? "" : (string)jobj["DAYNAME"];
                userMailSubscription.RecapReport = jobj["RECAPREPORT"] == null ? false : (bool)jobj["RECAPREPORT"];
                userMailSubscription.IsEmailEnable = jobj["ISEMAILENABLE"] == null ? false : (bool)jobj["ISEMAILENABLE"];
                userMailSubscription.LastSentOn = jobj["LASTSENTON"] == null ? DateTime.Now : (DateTimeOffset)jobj["LASTSENTON"];
                userMailSubscription.LastUpdatedOn = jobj["LASTUPDATEDON"] == null ? DateTime.Now : (DateTimeOffset)jobj["LASTUPDATEDON"];
                userMailSubscription.Id = jobj["ID"] == null ? 0 : (int)jobj["ID"];
                userMailSubscription.Userid = jobj["USERID"] == null ? 0 : (int)jobj["USERID"];
                return userMailSubscription;
            }
            catch (Exception Ex)
            {
                return null;
            }

        }

        /// <summary>
        /// Gets the type of all subscription.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <returns>IList</returns>
        public IList<ISubscriptionType> GetAllSubscriptionType(CommonManagerProxy proxy)
        {

            try
            {
                if (proxy.MarcomManager.AccessManager.CheckAccess(Modules.Admin, 4, FeatureID.Report, OperationId.Self) == true)
                {
                    IList<ISubscriptionType> subscriptiontype = new List<ISubscriptionType>();
                    using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                    {

                        var dao = tx.PersistenceManager.CommonRepository.GetAll<SubscriptionTypeDao>();
                        foreach (var item in dao)
                        {
                            ISubscriptionType subscription = new SubscriptionType();
                            subscription.Id = item.Id;
                            subscription.isAppDefault = item.isAppDefault;
                            subscription.isAppMandatory = item.isAppMandatory;
                            subscription.isMailDefault = item.isMailDefault;
                            subscription.isMailMandatory = item.isMailMandatory;
                            subscription.Caption = item.Caption;
                            subscriptiontype.Add(subscription);
                        }

                        tx.Commit();
                        return subscriptiontype;
                    }
                }

            }
            catch (Exception ex)
            {

            }
            return null;
        }

        /// <summary>
        /// Gets the user subscription settings.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="SubscribtionTypeID">The subscribtion type ID.</param>
        /// <returns>IUserSubscription</returns>
        public IUserSubscription GetUserSubscriptionSettings(CommonManagerProxy proxy, string SubscribtionTypeID)
        {

            try
            {
                UserSubscriptionDao UserSubscription = new UserSubscriptionDao();
                IUserSubscription newGetUserSubscriptionSettings = null;
                if (proxy.MarcomManager.AccessManager.CheckAccess(Modules.Admin, 4, FeatureID.Report, OperationId.Self) == true)
                {
                    using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                    {
                        UserSubscription = tx.PersistenceManager.CommonRepository.Get<UserSubscriptionDao>(SubscribtionTypeID);
                        tx.Commit();
                        newGetUserSubscriptionSettings.EntityTypeid = UserSubscription.EntityTypeid;
                        newGetUserSubscriptionSettings.Entityid = UserSubscription.Entityid;
                        newGetUserSubscriptionSettings.IsComplex = UserSubscription.IsComplex;
                        newGetUserSubscriptionSettings.IsMultiLevel = UserSubscription.IsMultiLevel;
                        newGetUserSubscriptionSettings.LastUpdatedOn = UserSubscription.LastUpdatedOn;
                        newGetUserSubscriptionSettings.SubscribedOn = UserSubscription.SubscribedOn;
                        newGetUserSubscriptionSettings.Userid = UserSubscription.Userid;
                        newGetUserSubscriptionSettings.Id = UserSubscription.Id;
                    }
                    return newGetUserSubscriptionSettings;
                }
            }
            catch (Exception ex)
            {

            }

            return null;
        }

        /// <summary>
        /// Updates the user subscription settings.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="UserId">The user id.</param>
        /// <param name="subscription">The subscription.</param>
        /// <returns>bool</returns>
        public bool UpdateUserSubscriptionSettings(CommonManagerProxy proxy, int UserId, ISubscriptionType subscription)
        {

            try
            {
                UserDefaultSubscriptionDao updateuserdefaultset = new UserDefaultSubscriptionDao();
                SubscriptionTypeDao subscriptiontypeDao = new SubscriptionTypeDao();
                if (proxy.MarcomManager.AccessManager.CheckAccess(Modules.Admin, 4, FeatureID.Report, OperationId.Self) == true)
                {
                    using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                    {
                        updateuserdefaultset.UserID = UserId;
                        subscriptiontypeDao.Id = subscription.Id;
                        //updateuserdefaultset.SubscriptionTypeid = subscriptiontypeDao.su;
                        tx.PersistenceManager.CommonRepository.Delete<UserDefaultSubscriptionDao>(updateuserdefaultset);
                        tx.Commit();
                    }
                    using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                    {
                        updateuserdefaultset.UserID = UserId;
                        subscriptiontypeDao.Id = subscription.Id;
                        //  updateuserdefaultset.SubscriptionTypeid = subscriptiontypeDao;
                        tx.PersistenceManager.CommonRepository.Save<UserDefaultSubscriptionDao>(updateuserdefaultset);
                        tx.Commit();
                    }
                    IUserDefaultSubscription usedefsub = new UserDefaultSubscription();
                    usedefsub.Userid = updateuserdefaultset.UserID;
                    // usedefsub.SubscriptionTypeid = subscription;
                    return true;
                }
            }
            catch (Exception ex)
            {

            }

            return false;
        }


        /// <summary>
        /// Updates the user subscription settings.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="UserId">The user id.</param>
        /// <param name="Id">The id.</param>
        /// <returns>bool</returns>
        public bool UpdateUserSubscriptionSettings(CommonManagerProxy proxy, int UserId, int Id)
        {

            try
            {
                UserDefaultSubscriptionDao updateuserdefaultset = new UserDefaultSubscriptionDao();
                SubscriptionTypeDao subscriptiontypeDao = new SubscriptionTypeDao();
                ISubscriptionType subscription = new SubscriptionType();
                subscription.Id = Id;
                if (proxy.MarcomManager.AccessManager.CheckAccess(Modules.Admin, 4, FeatureID.Report, OperationId.Self) == true)
                {
                    using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                    {
                        updateuserdefaultset.UserID = UserId;
                        subscriptiontypeDao.Id = subscription.Id;
                        //  updateuserdefaultset.SubscriptionTypeid = subscriptiontypeDao;
                        tx.PersistenceManager.CommonRepository.Delete<UserDefaultSubscriptionDao>(updateuserdefaultset);
                        updateuserdefaultset.UserID = UserId;
                        subscriptiontypeDao.Id = subscription.Id;
                        //   updateuserdefaultset.SubscriptionTypeid = subscriptiontypeDao;
                        tx.PersistenceManager.CommonRepository.Save<UserDefaultSubscriptionDao>(updateuserdefaultset);
                        tx.Commit();
                    }
                    IUserDefaultSubscription usedefsub = new UserDefaultSubscription();
                    usedefsub.Userid = updateuserdefaultset.UserID;
                    //   usedefsub.SubscriptionTypeid = subscription;
                    return true;
                }
            }
            catch (Exception ex)
            {

            }

            return false;
        }



        /// <summary>
        /// Inserts Navigation.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="navigation">The navigation.</param>
        /// <returns>int</returns>
        public int Navigation_Insert(CommonManagerProxy proxy, INavigation navigation)
        {

            try
            {
                NavigationDao navigationdao = new NavigationDao();
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    navigationdao.AddUserEmail = navigation.AddUserEmail;
                    navigationdao.AddUserName = navigation.AddUserName;
                    navigationdao.Caption = navigation.Caption;
                    navigationdao.Description = navigation.Description;
                    navigationdao.Id = navigation.Id;
                    navigationdao.Featureid = navigation.Featureid;
                    navigationdao.Imageurl = navigation.Imageurl;
                    navigationdao.IsActive = navigation.IsActive;
                    navigationdao.IsDynamicPage = navigation.IsDynamicPage;
                    navigationdao.IsExternal = navigation.IsExternal;
                    navigationdao.IsIframe = navigation.IsIframe;
                    navigationdao.IsPopup = navigation.IsPopup;
                    navigationdao.JavaScript = navigation.JavaScript;
                    navigationdao.Moduleid = navigation.Moduleid;
                    navigationdao.Parentid = navigation.Parentid;
                    navigationdao.Url = navigation.Url;
                    tx.PersistenceManager.CommonRepository.Save<NavigationDao>(navigationdao);
                    NavigationAccessDao navigationaccess = new NavigationAccessDao();
                    navigationaccess.Navigationid = navigationdao.Id;
                    navigationaccess.GlobalRoleid = 1;
                    tx.PersistenceManager.CommonRepository.Save<NavigationAccessDao>(navigationaccess);
                    tx.Commit();
                    return navigationdao.Id;
                }
            }
            catch (Exception ex)
            {

            }
            return 0;
        }

        /// <summary>
        /// Inserts Navigation.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="Id">The id.</param>
        /// <param name="Parentid">The parentid.</param>
        /// <param name="Moduleid">The moduleid.</param>
        /// <param name="Featureid">The featureid.</param>
        /// <param name="Caption">The caption.</param>
        /// <param name="Description">The description.</param>
        /// <param name="Url">The URL.</param>
        /// <param name="JavaScript">The java script.</param>
        /// <param name="IsActive">if set to <c>true</c> [is active].</param>
        /// <param name="IsPopup">if set to <c>true</c> [is popup].</param>
        /// <param name="IsIframe">if set to <c>true</c> [is iframe].</param>
        /// <param name="IsDynamicPage">if set to <c>true</c> [is dynamic page].</param>
        /// <param name="IsExternal">if set to <c>true</c> [is external].</param>
        /// <param name="AddUserName">if set to <c>true</c> [add user name].</param>
        /// <param name="AddUserEmail">if set to <c>true</c> [add user email].</param>
        /// <param name="Imageurl">The imageurl.</param>
        /// <param name="GlobalRoleid">The global roleid.</param>
        /// <returns>last inserted id</returns>
        public int Navigation_Insert(CommonManagerProxy proxy, int Id, int Typeid, int Parentid, int Moduleid, int Featureid, string Caption, string Description, string Url, string JavaScript, bool IsActive, bool IsPopup, bool IsIframe, bool IsDynamicPage, bool IsExternal, bool AddUserName, bool AddUserEmail, bool IsDefault, string ExternalUrl, string Imageurl, int GlobalRoleid, bool AddUserID, bool AddLanguageCode)
        {
            try
            {
                int SortOrder = 0;
                INavigation navigation = new Navigation();
                navigation.AddUserEmail = AddUserEmail;
                navigation.AddUserName = AddUserName;
                navigation.Caption = Caption;
                navigation.Description = Description;
                navigation.Featureid = Featureid;
                navigation.Typeid = Typeid;
                navigation.Imageurl = Imageurl;
                navigation.IsActive = IsActive;
                navigation.IsDynamicPage = IsDynamicPage;
                navigation.IsExternal = IsExternal;
                navigation.IsIframe = IsIframe;
                navigation.IsPopup = IsPopup;
                if (Featureid == 17)
                {
                    navigation.IsActive = true;
                    navigation.IsDynamicPage = true;
                    navigation.IsExternal = true;
                    navigation.IsIframe = true;
                }
                navigation.JavaScript = JavaScript;
                navigation.Moduleid = Moduleid;
                navigation.Parentid = Parentid;
                navigation.Url = Url;
                navigation.IsDefault = IsDefault;
                navigation.ExternalUrl = ExternalUrl;
                navigation.AddUserID = AddUserID;
                navigation.AddLanguageCode = AddLanguageCode;
                NavigationDao navigationdao = new NavigationDao();

                using (ITransaction tx1 = proxy.MarcomManager.GetTransaction())
                {

                    var mainnavs = tx1.PersistenceManager.CommonRepository.Get<NavigationDao>(Parentid);
                    if (mainnavs != null)
                    {
                        mainnavs.Featureid = 0;

                        //tx1.PersistenceManager.CommonRepository.Save<NavigationDao>(mainnavs);
                        //tx1.Commit();

                        var maxSortOrder = tx1.PersistenceManager.CommonRepository.ExecuteQuery("SELECT MAX(SortOrder) + 1 AS SortOrder FROM CM_Navigation cn WHERE cn.ParentID != 0");

                        SortOrder = (int)((System.Collections.Hashtable)(maxSortOrder)[0])["SortOrder"];
                    }
                    else
                    {
                        var maxSortOrder = tx1.PersistenceManager.CommonRepository.ExecuteQuery("SELECT MAX(SortOrder) + 1 AS SortOrder FROM CM_Navigation cn WHERE cn.ParentID = 0");

                        SortOrder = (int)((System.Collections.Hashtable)(maxSortOrder)[0])["SortOrder"];

                    }
                }

                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {


                    navigationdao.AddUserEmail = navigation.AddUserEmail;
                    navigationdao.AddUserName = navigation.AddUserName;
                    navigationdao.Caption = navigation.Caption;
                    navigationdao.Description = navigation.Description;
                    navigationdao.Id = navigation.Id;
                    navigationdao.Featureid = navigation.Featureid;
                    navigationdao.Imageurl = navigation.Imageurl;
                    navigationdao.IsActive = navigation.IsActive;
                    navigationdao.IsDynamicPage = navigation.IsDynamicPage;
                    navigationdao.IsExternal = navigation.IsExternal;
                    navigationdao.IsIframe = navigation.IsIframe;
                    navigationdao.IsPopup = navigation.IsPopup;
                    navigationdao.JavaScript = navigation.JavaScript;
                    navigationdao.Moduleid = navigation.Moduleid;
                    navigationdao.Parentid = navigation.Parentid;
                    navigationdao.Url = navigation.Url;
                    navigationdao.Typeid = navigation.Typeid;
                    navigationdao.IsDefault = IsDefault;
                    navigationdao.ExternalUrl = ExternalUrl;
                    navigationdao.AddUserID = AddUserID;
                    navigationdao.AddLanguageCode = AddLanguageCode;
                    navigationdao.SortOrder = SortOrder;
                    tx.PersistenceManager.CommonRepository.Save<NavigationDao>(navigationdao);

                    NavigationAccessDao navigationaccess = new NavigationAccessDao();
                    navigationaccess.Navigationid = navigationdao.Id;
                    navigationaccess.GlobalRoleid = GlobalRoleid;
                    tx.PersistenceManager.CommonRepository.Save<NavigationAccessDao>(navigationaccess);
                    tx.Commit();

                    return navigationdao.Id;
                }
            }
            catch (Exception ex)
            {

            }

            return 0;
        }

        public bool Navigation_Update(CommonManagerProxy proxy, int ID, int typeID, int parentId, int moduleid, int featureid, string Caption, string Description, string URL, string externalurl, bool IsExternal, bool IsDefault, bool IsEnable, bool AddUserID, bool AddLanguageCode, bool AddUserEmail, bool AddUserName, int SortOrder)
        {
            try
            {
                INavigation _UpdateFundingRequest = new Navigation();
                //     if (proxy.MarcomManager.AccessManager.CheckAccess(Modules.Admin, 4, FeatureID.Report, OperationId.Self, 1) == true)
                //{
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    NavigationDao dao = new NavigationDao();
                    dao.Id = ID;
                    dao.Typeid = typeID;
                    dao.Moduleid = moduleid;
                    dao.Description = Description;
                    dao.Url = URL;
                    dao.ExternalUrl = externalurl;
                    if (featureid == 17)
                    {
                        dao.IsExternal = true;
                        dao.IsDefault = true;
                    }
                    dao.AddUserEmail = AddUserEmail;
                    dao.AddUserName = AddUserName;
                    dao.Caption = Caption;
                    dao.Featureid = featureid;
                    dao.Imageurl = "";
                    dao.IsActive = true;
                    dao.JavaScript = "";
                    dao.Parentid = parentId;
                    dao.IsPopup = true;
                    dao.IsIframe = false;
                    dao.AddUserID = AddUserID;
                    dao.AddLanguageCode = AddLanguageCode;
                    dao.IsDynamicPage = true;
                    dao.SortOrder = SortOrder;
                    tx.PersistenceManager.CommonRepository.Save<NavigationDao>(dao);
                    tx.Commit();

                }

                return true;
            }
            //        return false; 
            //}

            catch (Exception ex)
            {

            }
            return false;
        }


        /// <summary>
        /// Deletes Navigation.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="navigation">The navigation.</param>
        /// <returns>bool</returns>
        public bool Navigation_Delete(CommonManagerProxy proxy, INavigation navigation)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    NavigationDao navigationdao = new NavigationDao();
                    navigationdao.AddUserEmail = navigation.AddUserEmail;
                    navigationdao.AddUserName = navigation.AddUserName;
                    navigationdao.Caption = navigation.Caption;
                    navigationdao.Description = navigation.Description;
                    navigationdao.Featureid = navigation.Featureid;
                    navigationdao.Id = navigation.Id;
                    navigationdao.Imageurl = navigation.Imageurl;
                    navigationdao.IsActive = navigation.IsActive;
                    navigationdao.IsDynamicPage = navigation.IsDynamicPage;
                    navigationdao.IsExternal = navigation.IsExternal;
                    navigationdao.IsIframe = navigation.IsIframe;
                    navigationdao.IsPopup = navigation.IsPopup;
                    navigationdao.JavaScript = navigation.JavaScript;
                    navigationdao.Moduleid = navigation.Moduleid;
                    navigationdao.Parentid = navigation.Parentid;
                    navigationdao.Url = navigation.Url;
                    tx.PersistenceManager.CommonRepository.Delete<NavigationDao>(navigationdao);
                    tx.Commit();
                    return true;
                }
            }
            catch (Exception ex)
            {

            }
            return false;

        }

        /// <summary>
        /// Deletes Navigation.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="Id">The id.</param>
        /// <returns>bool</returns>
        public bool Navigation_Delete(CommonManagerProxy proxy, int Id)
        {

            try
            {
                NavigationDao dao = new NavigationDao();
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    dao = tx.PersistenceManager.PlanningRepository.Get<NavigationDao>(Id);
                    tx.PersistenceManager.CommonRepository.Delete<NavigationDao>(dao);
                    tx.Commit();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// selects Navigation.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="NavigationID">The navigation ID.</param>
        /// <param name="ParentID">The parent ID.</param>
        /// <returns>IList</returns>
        public IList<INavigation> Navigation_Select(CommonManagerProxy proxy, int NavigationID, int ParentID)
        {

            try
            {
                IList<INavigation> navigationlist = new List<INavigation>();
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    IList<NavigationDao> navigationdao;
                    navigationdao = tx.PersistenceManager.CommonRepository.GetAll<NavigationDao>();
                    tx.Commit();
                    // var linqnavigationdao = from t in navigationdao where t.Parentid == ParentID && t.Typeid == NavigationID select t;

                    var linqnavigationdao = (from t in navigationdao select t).OrderBy(s => s.SortOrder);

                    foreach (var temp in linqnavigationdao.ToList())
                    {
                        INavigation navigation = new Navigation();
                        navigation.AddUserEmail = temp.AddUserEmail;
                        navigation.AddUserName = temp.AddUserName;
                        navigation.Caption = temp.Caption;
                        navigation.Description = temp.Description;
                        navigation.Featureid = temp.Featureid;
                        navigation.Id = temp.Id;
                        navigation.Imageurl = temp.Imageurl;
                        navigation.IsActive = temp.IsActive;
                        navigation.IsDynamicPage = temp.IsDynamicPage;
                        navigation.ExternalUrl = temp.ExternalUrl;
                        navigation.IsExternal = temp.IsExternal;
                        navigation.IsDefault = temp.IsDefault;
                        navigation.IsExternal = temp.IsExternal;
                        navigation.Typeid = temp.Typeid;
                        navigation.IsIframe = temp.IsIframe;
                        navigation.IsPopup = temp.IsPopup;
                        navigation.JavaScript = temp.JavaScript;
                        navigation.Moduleid = temp.Moduleid;
                        navigation.Parentid = temp.Parentid;
                        navigation.Url = temp.Url;
                        navigation.SortOrder = temp.SortOrder;
                        navigationlist.Add(navigation);
                    }
                }
                return navigationlist;
            }
            catch (Exception ex)
            {


            }
            return null;
        }

        public bool UpdateNavigationSortOrder(CommonManagerProxy proxy, int Id, int SortOrder)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    tx.PersistenceManager.CommonRepository.ExecuteQuerywithMinParam("Update CM_Navigation set SortOrder = ?  where ID = ? ", SortOrder, Id);
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
        /// selects Navigation.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="NavigationID">The navigation ID.</param>
        /// <param name="ParentID">The parent ID.</param>
        /// <returns>IList</returns>
        public IList<INavigation> Navigation_Select(CommonManagerProxy proxy, bool IsParentID, int UserID, int flag)
        {
            // int flag=0;
            try
            {
                IList<INavigation> navigationlist = new List<INavigation>();
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    IList<NavigationDao> navigationdao;
                    //IList<MultiProperty> prpList = new List<MultiProperty>();
                    // prpList.Add(new MultiProperty { propertyName = GlobalRoleUserDao.PropertyNames.Userid, propertyValue = UserID });
                    // prpList.Add(new MultiProperty { propertyName = GlobalRoleUserDao.PropertyNames.GlobalRoleId, propertyValue = 1 });
                    //var RoleId = (from rr in tx.PersistenceManager.AccessRepository.Query<GlobalRoleUserDao>().ToList() 
                    //              join tt in proxy.MarcomManager.User.ListOfUserGlobalRoles 
                    //                  on rr.GlobalRoleId equals tt.GlobalRoleid  
                    //                  where rr.Userid==UserID
                    //                 select rr).ToList();

                    //if (RoleId.Count() > 0)
                    navigationdao = tx.PersistenceManager.CommonRepository.GetAll<NavigationDao>();
                    // else
                    // navigationdao = tx.PersistenceManager.CommonRepository.GetAll<NavigationDao>().Where(a => a.Typeid != 9).Cast<NavigationDao>().ToList();
                    // tx.Commit();

                    List<NavigationDao> applicableNavigation = null;
                    if (flag == 0)
                    {
                        applicableNavigation = (from uu in navigationdao select uu).ToList();
                    }
                    else
                    {
                        if (IsParentID == true)
                        {
                            var haschildren = (from uu in navigationdao
                                               join tt in navigationdao on uu.Id equals tt.Parentid
                                               select tt).ToList();
                            if (haschildren.Count() >= 0)
                            {
                                applicableNavigation = (from access in proxy.MarcomManager.User.ListOfUserGlobalRoles
                                                        join navs in navigationdao
                                                            on access.Moduleid equals navs.Moduleid
                                                        //on new { access.Moduleid, access.Featureid } equals new { navs.Moduleid, navs.Featureid }
                                                        select navs).Distinct().OrderBy(p => p.Id).ToList();
                            }
                            else
                            {
                                applicableNavigation = (from access in proxy.MarcomManager.User.ListOfUserGlobalRoles
                                                        join navs in navigationdao
                                                            on access.Moduleid equals navs.Moduleid
                                                        //on new { access.Moduleid, access.Featureid } equals new { navs.Moduleid, navs.Featureid }
                                                        where access.Featureid == navs.Featureid
                                                        select navs).Distinct().OrderBy(p => p.Id).ToList();
                            }
                        }
                        else
                        {
                            applicableNavigation = (from access in proxy.MarcomManager.User.ListOfUserGlobalRoles
                                                    join navs in navigationdao
                                                        on access.Moduleid equals navs.Moduleid
                                                    //on new { access.Moduleid, access.Featureid } equals new { navs.Moduleid, navs.Featureid }
                                                    where access.Featureid == navs.Featureid
                                                    select navs).Distinct().OrderBy(p => p.Id).ToList();
                        }

                    }


                    var supNav = (from nav in navigationdao where nav.Featureid == 23 select nav).FirstOrDefault();      //for support page 23=support

                    if (supNav != null)
                    {

                        NavigationDao dao = new NavigationDao();
                        dao.AddLanguageCode = supNav.AddLanguageCode;
                        dao.AddUserEmail = supNav.AddUserEmail;
                        dao.AddUserID = supNav.AddUserID;
                        dao.AddUserName = supNav.AddUserName;
                        dao.Caption = supNav.Caption;

                        dao.Description = supNav.Description;
                        dao.ExternalUrl = supNav.ExternalUrl;
                        dao.Featureid = supNav.Featureid;
                        dao.Id = supNav.Id;
                        applicableNavigation.Add(supNav);
                    }


                    var linqnavigationdao = (from t in applicableNavigation select t).OrderBy(x => x.SortOrder);

                    if (IsParentID == true)
                    {
                        linqnavigationdao = (from t in linqnavigationdao where t.Parentid == 0 select t).OrderBy(x => x.SortOrder);
                    }
                    else
                    {
                        linqnavigationdao = (from t in linqnavigationdao where t.Parentid != 0 select t).OrderBy(x => x.SortOrder);
                    }

                    //var linqnavigationdao = from t in navigationdao select t;

                    foreach (var temp in linqnavigationdao.ToList())
                    {
                        INavigation navigation = new Navigation();
                        navigation.AddUserEmail = temp.AddUserEmail;
                        navigation.AddUserName = temp.AddUserName;
                        navigation.Caption = temp.Caption;
                        navigation.Description = temp.Description;
                        navigation.Featureid = temp.Featureid;
                        navigation.Id = temp.Id;
                        navigation.Imageurl = temp.Imageurl;
                        navigation.IsActive = temp.IsActive;
                        navigation.IsDynamicPage = temp.IsDynamicPage;
                        navigation.ExternalUrl = temp.ExternalUrl;
                        navigation.IsExternal = temp.IsExternal;
                        navigation.IsDefault = temp.IsDefault;
                        navigation.IsExternal = temp.IsExternal;
                        navigation.Typeid = temp.Typeid;
                        navigation.IsIframe = temp.IsIframe;
                        navigation.IsPopup = temp.IsPopup;
                        navigation.JavaScript = temp.JavaScript;
                        navigation.Moduleid = temp.Moduleid;
                        navigation.Parentid = temp.Parentid;
                        navigation.Url = temp.Url;
                        navigation.AddUserID = temp.AddUserID;
                        navigation.AddLanguageCode = temp.AddLanguageCode;
                        navigation.SortOrder = temp.SortOrder;
                        navigationlist.Add(navigation);
                    }
                }
                navigationlist = (from navlist1 in navigationlist select navlist1).Distinct().ToList();

                return navigationlist;
            }
            catch (Exception ex)
            {


            }
            return null;
        }

        public TopNavigation GetTopNavigation(CommonManagerProxy proxy)
        {
            using (ITransaction tx = proxy.MarcomManager.GetTransaction())
            {
                //List<NavigationDao> applicableNavigation = null;

                //List<NavigationDao> childnavigationavailability = null;

                IList<NavigationDao> navigationdao;

                //NavigationDao dao = new NavigationDao();

                //IList<INavigation> _iinavigation = new List<INavigation>();
                navigationdao = tx.PersistenceManager.CommonRepository.GetAll<NavigationDao>();

                var allParentIDs = navigationdao.Select(a => a.Parentid);

                var topMenuWithOutChild = from navig in navigationdao
                                          //join parentNavig in navigationdao on navig.Id  parentNavig.Parentid
                                          //join navgAccess in proxy.MarcomManager.User.ListOfUserGlobalRoles on new { navig.Moduleid, navig.Featureid } equals new { navgAccess.Moduleid, navgAccess.Featureid }
                                          where !allParentIDs.Contains(navig.Id) && navig.Parentid == 0
                                          select navig;
                var topMenuWithOutChildWithAccess = from navig in topMenuWithOutChild
                                                    join navgAccess in proxy.MarcomManager.User.ListOfUserGlobalRoles on new { navig.Moduleid, navig.Featureid } equals new { navgAccess.Moduleid, navgAccess.Featureid }
                                                    select navig;

                var topMenuWithChild = (from navig in navigationdao
                                        join parentNavig in navigationdao on navig.Id equals parentNavig.Parentid
                                        select navig).Distinct();

                var childerMenu = (from navig in navigationdao
                                   join parentNavig in topMenuWithChild on navig.Parentid equals parentNavig.Id
                                   join navgAccess in proxy.MarcomManager.User.ListOfUserGlobalRoles on new { navig.Moduleid, navig.Featureid } equals new { navgAccess.Moduleid, navgAccess.Featureid }
                                   select navig).OrderBy(a => a.SortOrder).Distinct();

                var parentIDshavingChildWithAccess = childerMenu.Select(e => e.Parentid).Distinct();

                var topMenu = ((from navig in topMenuWithOutChildWithAccess select navig)
                                        .Union(from navigWithChild in navigationdao where parentIDshavingChildWithAccess.Contains(navigWithChild.Id) select navigWithChild)).OrderBy(a => a.SortOrder);

                IList<INavigation> TopNavigationMenu = new List<INavigation>();



                TopNavigation topnavigationlist = new TopNavigation();

                topnavigationlist.TopMenu = new List<INavigation>();
                topnavigationlist.ChildMenu = new List<INavigation>();

                int supportcount = 0;
                var supNav = (from nav in navigationdao where nav.Featureid == 23 select nav).FirstOrDefault();
                INavigation support = new Navigation();
                if (supNav != null)
                {
                    support.Id = supNav.Id;
                    support.Typeid = supNav.Typeid;
                    support.Parentid = supNav.Parentid;
                    support.Moduleid = supNav.Moduleid;
                    support.Featureid = supNav.Featureid;
                    support.Caption = supNav.Caption;
                    support.Description = supNav.Description;
                    support.Url = supNav.Url;
                    support.ExternalUrl = supNav.ExternalUrl;
                    support.JavaScript = supNav.JavaScript;
                    support.IsActive = supNav.IsActive;
                    support.IsDefault = supNav.IsDefault;
                    support.IsPopup = supNav.IsPopup;
                    support.IsIframe = supNav.IsIframe;
                    support.IsDynamicPage = supNav.IsDynamicPage;
                    support.IsExternal = supNav.IsExternal;
                    support.AddUserName = supNav.AddUserName;
                    support.AddUserEmail = supNav.AddUserEmail;
                    support.Imageurl = supNav.Imageurl;
                    support.AddUserID = supNav.AddUserID;
                    support.AddLanguageCode = supNav.AddLanguageCode;
                    support.SortOrder = supNav.SortOrder;

                    supportcount = topMenu.Where(a => a.Id == supNav.Id).ToList().Count();
                }


                foreach (var top in topMenu)
                {
                    INavigation topnavig = new Navigation();
                    topnavig.Id = top.Id;
                    topnavig.Typeid = top.Typeid;
                    topnavig.Parentid = top.Parentid;
                    topnavig.Moduleid = top.Moduleid;
                    topnavig.Featureid = top.Featureid;
                    topnavig.Caption = top.Caption;
                    topnavig.Description = top.Description;
                    topnavig.Url = top.Url;
                    topnavig.ExternalUrl = top.ExternalUrl;
                    topnavig.JavaScript = top.JavaScript;
                    topnavig.IsActive = top.IsActive;
                    topnavig.IsDefault = top.IsDefault;
                    topnavig.IsPopup = top.IsPopup;
                    topnavig.IsIframe = top.IsIframe;
                    topnavig.IsDynamicPage = top.IsDynamicPage;
                    topnavig.IsExternal = top.IsExternal;
                    topnavig.AddUserName = top.AddUserName;
                    topnavig.AddUserEmail = top.AddUserEmail;
                    topnavig.Imageurl = top.Imageurl;
                    topnavig.AddUserID = top.AddUserID;
                    topnavig.AddLanguageCode = top.AddLanguageCode;
                    topnavig.SortOrder = top.SortOrder;
                    topnavigationlist.TopMenu.Add(topnavig);
                }

                if (supNav != null)
                {
                    if (supportcount == 0)
                    {
                        //topnavigationlist.TopMenu.Add(support);
                    }
                }

                foreach (var child in childerMenu)
                {
                    INavigation childnavig = new Navigation();
                    childnavig.Id = child.Id;
                    childnavig.Typeid = child.Typeid;
                    childnavig.Parentid = child.Parentid;
                    childnavig.Moduleid = child.Moduleid;
                    childnavig.Featureid = child.Featureid;
                    childnavig.Caption = child.Caption;
                    childnavig.Description = child.Description;
                    childnavig.Url = child.Url;
                    childnavig.ExternalUrl = child.ExternalUrl;
                    childnavig.JavaScript = child.JavaScript;
                    childnavig.IsActive = child.IsActive;
                    childnavig.IsDefault = child.IsDefault;
                    childnavig.IsPopup = child.IsPopup;
                    childnavig.IsIframe = child.IsIframe;
                    childnavig.IsDynamicPage = child.IsDynamicPage;
                    childnavig.IsExternal = child.IsExternal;
                    childnavig.AddUserName = child.AddUserName;
                    childnavig.AddUserEmail = child.AddUserEmail;
                    childnavig.Imageurl = child.Imageurl;
                    childnavig.AddUserID = child.AddUserID;
                    childnavig.AddLanguageCode = child.AddLanguageCode;
                    childnavig.SortOrder = child.SortOrder;

                    topnavigationlist.ChildMenu.Add(childnavig);

                }

                return topnavigationlist;

            }
        }
        /// <summary>
        /// Gets the group ID for navigation.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="NavID">The nav ID.</param>
        /// <param name="UserID">The user ID.</param>
        /// <returns>INavigationAccess</returns>
        public INavigationAccess GetGroupIDForNavigation(CommonManagerProxy proxy, int NavID, int UserID)
        {

            try
            {
                INavigationAccess navaccess = new NavigationAccess();
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    NavigationAccessDao navidationaccessdao = new NavigationAccessDao();
                    navidationaccessdao = tx.PersistenceManager.CommonRepository.Get<NavigationAccessDao>(NavID);
                    navaccess.GlobalRoleid = navidationaccessdao.GlobalRoleid;
                    navaccess.Navigationid = navidationaccessdao.Navigationid;
                }
                return navaccess;
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        /// <summary>
        /// Inserts the subscription notificationsettings.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="userid">The userid.</param>
        /// <param name="lastsenton">The lastsenton.</param>
        /// <param name="lastupdatedon">The lastupdatedon.</param>
        /// <param name="Timing">The timing.</param>
        /// <param name="IsEmailEnable">if set to <c>true</c> [is email enable].</param>
        /// <param name="DayName">Name of the day.</param>
        /// <param name="RecapReport">if set to <c>true</c> [recap report].</param>
        /// <returns>int</returns>
        public int InsertSubscriptionNotificationsettings(CommonManagerProxy proxy, int userid, DateTimeOffset lastsenton, DateTimeOffset lastupdatedon, TimeSpan Timing, bool IsEmailEnable, string DayName, bool RecapReport)
        {

            try
            {
                if (proxy.MarcomManager.AccessManager.CheckAccess(Modules.Admin, 4, FeatureID.Report, OperationId.Self) == true)
                {
                    using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                    {
                        // insert notification/subscription logic
                        UserMailSubscriptionDao dao = new UserMailSubscriptionDao();
                        dao.Userid = userid;
                        dao.LastSentOn = lastsenton;
                        dao.LastUpdatedOn = lastupdatedon;
                        dao.IsEmailEnable = IsEmailEnable;
                        dao.DayName = DayName;
                        dao.Timing = Timing;
                        dao.RecapReport = RecapReport;
                        tx.PersistenceManager.PlanningRepository.Save<UserMailSubscriptionDao>(dao);
                        tx.Commit();
                        IUserMailSubscription ms = new UserMailSubscription();
                        ms.Id = dao.Id;
                        ms.Userid = userid;
                        ms.LastSentOn = lastsenton;
                        ms.LastUpdatedOn = lastupdatedon;
                        ms.IsEmailEnable = IsEmailEnable;
                        ms.DayName = DayName;
                        ms.Timing = Timing;
                        ms.RecapReport = RecapReport;
                        return ms.Id;
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return 0;
        }

        /// <summary>
        /// Updates the recap notificationsettings.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="id">The id.</param>
        /// <param name="tempUserMailSubscription">The temp user mail subscription.</param>
        /// <returns>bool</returns>
        public bool UpdateRecapNotificationsettings(CommonManagerProxy proxy, int id, IUserMailSubscription tempUserMailSubscription)
        {

            try
            {
                IUserMailSubscription _usrmailsunscrptn = new UserMailSubscription();
                UserMailSubscriptionDao dao = new UserMailSubscriptionDao();
                if (proxy.MarcomManager.AccessManager.CheckAccess(Modules.Admin, 4, FeatureID.Report, OperationId.Self) == true)
                {
                    using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                    {
                        dao = tx.PersistenceManager.PlanningRepository.Get<UserMailSubscriptionDao>(id);
                        tx.Commit();
                    }
                    using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                    {
                        // insert notification/subscription logic
                        dao.Id = id;
                        dao.Userid = tempUserMailSubscription.Userid;
                        dao.LastUpdatedOn = tempUserMailSubscription.LastUpdatedOn;
                        dao.RecapReport = tempUserMailSubscription.RecapReport;
                        dao.LastSentOn = tempUserMailSubscription.LastSentOn;
                        dao.IsEmailEnable = tempUserMailSubscription.IsEmailEnable;
                        dao.DayName = tempUserMailSubscription.DayName;
                        dao.Timing = tempUserMailSubscription.Timing;
                        tx.PersistenceManager.PlanningRepository.Save<UserMailSubscriptionDao>(dao);
                        tx.Commit();
                    }
                    _usrmailsunscrptn.Id = dao.Id;
                    _usrmailsunscrptn.Userid = dao.Userid;
                    _usrmailsunscrptn.LastUpdatedOn = dao.LastUpdatedOn;
                    _usrmailsunscrptn.RecapReport = dao.RecapReport;
                    _usrmailsunscrptn.LastSentOn = dao.LastSentOn;
                    _usrmailsunscrptn.IsEmailEnable = dao.IsEmailEnable;
                    _usrmailsunscrptn.DayName = dao.DayName;
                    _usrmailsunscrptn.Timing = dao.Timing;
                    return true;
                }
            }
            catch (Exception ex)
            {

            }

            return false;
        }

        /// <summary>
        /// Updates the recap notificationsettings.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="id">The id.</param>
        /// <param name="Userid">The userid.</param>
        /// <param name="LastSentOn">The last sent on.</param>
        /// <param name="LastUpdatedOn">The last updated on.</param>
        /// <param name="IsEmailEnable">if set to <c>true</c> [is email enable].</param>
        /// <param name="DayName">Name of the day.</param>
        /// <param name="Timing">The timing.</param>
        /// <param name="RecapReport">if set to <c>true</c> [recap report].</param>
        /// <returns>bool</returns>
        public bool UpdateRecapNotificationsettings(CommonManagerProxy proxy, int id, int Userid, DateTimeOffset LastSentOn, DateTimeOffset LastUpdatedOn, bool IsEmailEnable, string DayName, TimeSpan Timing, bool RecapReport)
        {

            try
            {
                IUserMailSubscription _usrmailsunscrptn = new UserMailSubscription();
                UserMailSubscriptionDao dao = new UserMailSubscriptionDao();
                IUserMailSubscription tempUserMailSubscription = new UserMailSubscription();
                tempUserMailSubscription.Userid = Userid;
                tempUserMailSubscription.LastUpdatedOn = LastUpdatedOn;
                tempUserMailSubscription.RecapReport = RecapReport;
                tempUserMailSubscription.LastSentOn = LastSentOn;
                tempUserMailSubscription.IsEmailEnable = IsEmailEnable;
                tempUserMailSubscription.DayName = DayName;
                tempUserMailSubscription.Timing = Timing;
                if (proxy.MarcomManager.AccessManager.CheckAccess(Modules.Admin, 4, FeatureID.Report, OperationId.Self) == true)
                {
                    using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                    {
                        dao = tx.PersistenceManager.PlanningRepository.Get<UserMailSubscriptionDao>(id);
                        // insert notification/subscription logic
                        dao.Id = id;
                        dao.Userid = tempUserMailSubscription.Userid;
                        dao.LastUpdatedOn = tempUserMailSubscription.LastUpdatedOn;
                        dao.RecapReport = tempUserMailSubscription.RecapReport;
                        dao.LastSentOn = tempUserMailSubscription.LastSentOn;
                        dao.IsEmailEnable = tempUserMailSubscription.IsEmailEnable;
                        dao.DayName = tempUserMailSubscription.DayName;
                        dao.Timing = tempUserMailSubscription.Timing;
                        tx.PersistenceManager.PlanningRepository.Save<UserMailSubscriptionDao>(dao);
                        tx.Commit();
                    }
                    _usrmailsunscrptn.Id = dao.Id;
                    _usrmailsunscrptn.Userid = dao.Userid;
                    _usrmailsunscrptn.LastUpdatedOn = dao.LastUpdatedOn;
                    _usrmailsunscrptn.RecapReport = dao.RecapReport;
                    _usrmailsunscrptn.LastSentOn = dao.LastSentOn;
                    _usrmailsunscrptn.IsEmailEnable = dao.IsEmailEnable;
                    _usrmailsunscrptn.DayName = dao.DayName;
                    _usrmailsunscrptn.Timing = dao.Timing;
                    return true;
                }
            }
            catch (Exception ex)
            {

            }
            return false;
        }

        /// <summary>
        ///feed
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="entityid">The entityid.</param>
        /// <returns>IList</returns>
        public IList<IFeed> GetFeedByID(CommonManagerProxy proxy, int entityid)
        {
            try
            {
                IList<IFeed> getfeedlist = new List<IFeed>();
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    IList<FeedDao> feeddao = new List<FeedDao>();
                    feeddao = tx.PersistenceManager.PlanningRepository.GetAll<FeedDao>();
                    tx.Commit();
                    var linqfeed = from t in feeddao where t.Entityid == entityid select t;
                    foreach (var temp in linqfeed.ToList())
                    {
                        IFeed feed = new Feed();
                        feed.Id = temp.Id;
                        feed.Actor = temp.Actor;
                        feed.Entityid = temp.Entityid;
                        getfeedlist.Add(feed);
                    }
                }
                return getfeedlist;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        /// <summary>
        /// Inserts the feed comment.
        /// </summary>
        /// <param name="commonManagerProxy">The common manager proxy.</param>
        /// <param name="feedid">The feedid.</param>
        /// <param name="actor">The actor.</param>
        /// <param name="comment">The comment.</param>
        /// <param name="commentupdatedon">The commentupdatedon.</param>
        /// <returns>int</returns>
        internal string InsertFeedComment(CommonManagerProxy commonManagerProxy, int feedid, int actor, string comment)
        {
            try
            {
                using (ITransaction tx = commonManagerProxy.MarcomManager.GetTransaction())
                {
                    // insert notification/subscription logic
                    FeedCommentDao dao = new FeedCommentDao();
                    dao.Id = dao.Id;
                    dao.Feedid = feedid;
                    dao.Actor = actor;
                    dao.Comment = HttpUtility.HtmlEncode(comment);
                    dao.CommentedOn = System.DateTime.UtcNow;
                    tx.PersistenceManager.PlanningRepository.Save<FeedCommentDao>(dao);
                    tx.Commit();
                    FeedNotificationServer fs = new FeedNotificationServer();
                    NotificationFeedObjects obj = new NotificationFeedObjects();
                    obj.Actorid = actor;
                    obj.action = "comment added on feed";
                    var fetchentityidforfeed = (from item in tx.PersistenceManager.CommonRepository.Query<FeedDao>() where item.Id == (int)feedid select item).FirstOrDefault();
                    obj.EntityId = (int)fetchentityidforfeed.Entityid;
                    obj.ToValue = HttpUtility.HtmlEncode(comment.ToString());
                    obj.Userid = fetchentityidforfeed.Actor;
                    obj.FromValue = feedid.ToString();
                    obj.AssociatedEntityId = (int)fetchentityidforfeed.AssocitedEntityID;
                    fs.AsynchronousNotify(obj);
                    return dao.Comment;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Inserts the content of the update mail.
        /// </summary>
        /// <param name="commonManagerProxy">The common manager proxy.</param>
        /// <param name="Subject">The subject.</param>
        /// <param name="Body">The body.</param>
        /// <param name="description">The description.</param>
        /// <param name="id">The id.</param>
        /// <returns>int</returns>
        public int InsertUpdateMailContent(CommonManagerProxy commonManagerProxy, string Subject, string Body, string description, int id)
        {
            try
            {
                MailContentDao mDao = new MailContentDao();
                IList<MailContentDao> treevalue;
                using (ITransaction tx = commonManagerProxy.MarcomManager.GetTransaction())
                {
                    treevalue = tx.PersistenceManager.PlanningRepository.GetAll<MailContentDao>();
                    tx.Commit();
                    var tid = from table in treevalue where table.Id == id select table;
                    if (id != 0)
                    {
                        mDao.Id = id;
                    }
                    mDao.Body = Body;
                    mDao.description = description;
                    mDao.Subject = Subject;
                    tx.PersistenceManager.CommonRepository.Save<MailContentDao>(mDao);
                    tx.Commit();
                }
                IMailContent mailContent = new MailContent();
                mailContent.Id = mDao.Id;
                mailContent.Body = mDao.Body;
                mailContent.description = mDao.description;
                mailContent.Subject = mDao.Subject;
                return mailContent.Id;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Deletes the content of the mail.
        /// </summary>
        /// <param name="commonManagerProxy">The common manager proxy.</param>
        /// <param name="id">The id.</param>
        /// <returns>bool</returns>
        public bool DeleteMailContent(CommonManagerProxy commonManagerProxy, int id)
        {
            try
            {
                MailContentDao dao = new MailContentDao();
                using (ITransaction tx = commonManagerProxy.MarcomManager.GetTransaction())
                {
                    dao = tx.PersistenceManager.PlanningRepository.Get<MailContentDao>(id);
                    tx.PersistenceManager.PlanningRepository.Delete<MailContentDao>(dao);
                    tx.Commit();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Inserts the update mail footer.
        /// </summary>
        /// <param name="commonManagerProxy">The common manager proxy.</param>
        /// <param name="Body">The body.</param>
        /// <param name="description">The description.</param>
        /// <param name="id">The id.</param>
        /// <returns>int</returns>
        public int InsertUpdateMailFooter(CommonManagerProxy commonManagerProxy, string Body, string description, int id)
        {
            try
            {
                MailFooterDao mDao = new MailFooterDao();
                IList<MailFooterDao> treevalue;
                using (ITransaction tx = commonManagerProxy.MarcomManager.GetTransaction())
                {
                    treevalue = tx.PersistenceManager.PlanningRepository.GetAll<MailFooterDao>();
                }
                var tid = from table in treevalue where table.Id == id select table;
                if (id != 0)
                {
                    mDao.Id = id;
                }
                mDao.Body = Body;
                mDao.description = description;
                using (ITransaction tx = commonManagerProxy.MarcomManager.GetTransaction())
                {
                    tx.PersistenceManager.CommonRepository.Save<MailFooterDao>(mDao);
                    tx.Commit();
                }
                IMailFooter mailfoot = new MailFooter();
                mailfoot.Body = mDao.Body;
                mailfoot.description = mDao.description;
                mailfoot.Id = mDao.Id;
                return mailfoot.Id;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Deletes the mail footer.
        /// </summary>
        /// <param name="commonManagerProxy">The common manager proxy.</param>
        /// <param name="id">The id.</param>
        /// <returns>bool</returns>
        public bool DeleteMailFooter(CommonManagerProxy commonManagerProxy, int id)
        {
            try
            {
                MailFooterDao dao = new MailFooterDao();
                using (ITransaction tx = commonManagerProxy.MarcomManager.GetTransaction())
                {
                    dao = tx.PersistenceManager.PlanningRepository.Get<MailFooterDao>(id);

                    tx.PersistenceManager.PlanningRepository.Delete<MailFooterDao>(dao);
                    tx.Commit();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Updates the user single entity subscription.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="EntityId">The entity id.</param>
        /// <param name="SubscriptionTypeIDs">The subscription type I ds.</param>
        /// <param name="Userid">The userid.</param>
        /// <returns>bool</returns>
        public bool UpdateUserSingleEntitySubscription(CommonManagerProxy proxy, int EntityId, SubscriptionTypeDao SubscriptionTypeIDs, int Userid)
        {

            try
            {
                if (proxy.MarcomManager.AccessManager.CheckAccess(Modules.Admin, 4, FeatureID.Report, OperationId.Self) == true)
                {
                    if (proxy.MarcomManager.AccessManager.CheckAccess(Modules.Admin, 4, FeatureID.Report, OperationId.Self, 1) == true)
                    {
                        try
                        {
                            UserAutoSubscriptionDao AutoSubscription = new UserAutoSubscriptionDao();
                            IUserAutoSubscription userautoSubscription = new UserAutoSubscription();
                            UserSubscriptionDao UserSubscription = new UserSubscriptionDao();
                            IList<UserSubscriptionDao> UserList;
                            using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                            {
                                UserList = tx.PersistenceManager.CommonRepository.GetAll<UserSubscriptionDao>();
                                tx.Commit();
                                var x = from t in UserList where t.Userid == Userid && t.Entityid == EntityId select t;
                                int y = x.Count();
                                if (y > 0)
                                {
                                    SubscriptionTypeDao subsDao = new SubscriptionTypeDao();
                                    subsDao.Id = x.ElementAt(0).Id;
                                    AutoSubscription.Subscriptionid = subsDao;
                                    AutoSubscription.Entityid = EntityId;
                                    AutoSubscription.IsValid = true;
                                    AutoSubscription.Userid = Userid;
                                    AutoSubscription.EntityTypeid = 1;
                                    tx.PersistenceManager.CommonRepository.Delete<UserAutoSubscriptionDao>(AutoSubscription);
                                    UserSubscription.IsComplex = true;
                                    UserSubscription.IsMultiLevel = false;
                                    UserSubscription.Id = x.ElementAt(0).Id;
                                    UserSubscription.Entityid = EntityId;
                                    UserSubscription.EntityTypeid = 12;
                                    userautoSubscription.Userid = Userid;
                                    tx.PersistenceManager.CommonRepository.Save<UserSubscriptionDao>(UserSubscription);
                                }
                                else
                                {
                                    UserSubscription.Userid = Userid;
                                    UserSubscription.Entityid = EntityId;
                                    UserSubscription.IsComplex = true;
                                    tx.PersistenceManager.CommonRepository.Save<UserSubscriptionDao>(UserSubscription);
                                }
                                tx.Commit();
                                return true;
                            }
                        }
                        catch (Exception ex)
                        {
                            return false;
                        }
                    } return false;
                }
            }
            catch (Exception ex)
            {

            }

            return false;
        }

        /// <summary>
        /// Updates the user single entity subscription.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="EntityId">The entity id.</param>
        /// <param name="Id">The id.</param>
        /// <param name="Caption">The caption.</param>
        /// <param name="IsAutomated">if set to <c>true</c> [is automated].</param>
        /// <param name="Userid">The userid.</param>
        /// <returns>bool</returns>
        public bool UpdateUserSingleEntitySubscription(CommonManagerProxy proxy, int EntityId, int Id, string Caption, bool IsAutomated, int Userid)
        {

            try
            {
                SubscriptionTypeDao SubscriptionTypeIDs = new SubscriptionTypeDao();
                SubscriptionTypeIDs.Id = Id;
                SubscriptionTypeIDs.Caption = Caption;
                //SubscriptionTypeIDs.IsAutomated = IsAutomated;
                if (proxy.MarcomManager.AccessManager.CheckAccess(Modules.Admin, 4, FeatureID.Report, OperationId.Self) == true)
                {
                    if (proxy.MarcomManager.AccessManager.CheckAccess(Modules.Admin, 4, FeatureID.Report, OperationId.Self, 1) == true)
                    {
                        try
                        {
                            UserAutoSubscriptionDao AutoSubscription = new UserAutoSubscriptionDao();
                            IUserAutoSubscription userautoSubscription = new UserAutoSubscription();
                            UserSubscriptionDao UserSubscription = new UserSubscriptionDao();
                            IList<UserSubscriptionDao> UserList;
                            using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                            {
                                UserList = tx.PersistenceManager.CommonRepository.GetAll<UserSubscriptionDao>();
                                tx.Commit();
                                var x = from t in UserList where t.Userid == Userid && t.Entityid == EntityId select t;
                                int y = x.Count();
                                if (y > 0)
                                {
                                    SubscriptionTypeDao subsDao = new SubscriptionTypeDao();
                                    subsDao.Id = x.ElementAt(0).Id;
                                    AutoSubscription.Subscriptionid = subsDao;
                                    AutoSubscription.Entityid = EntityId;
                                    AutoSubscription.IsValid = true;
                                    AutoSubscription.Userid = Userid;
                                    AutoSubscription.EntityTypeid = 1;
                                    tx.PersistenceManager.CommonRepository.Delete<UserAutoSubscriptionDao>(AutoSubscription);
                                    UserSubscription.IsComplex = true;
                                    UserSubscription.IsMultiLevel = false;
                                    UserSubscription.Id = x.ElementAt(0).Id;
                                    UserSubscription.Entityid = EntityId;
                                    UserSubscription.EntityTypeid = 12;
                                    userautoSubscription.Userid = Userid;
                                    tx.PersistenceManager.CommonRepository.Save<UserSubscriptionDao>(UserSubscription);
                                }
                                else
                                {
                                    UserSubscription.Userid = Userid;
                                    UserSubscription.Entityid = EntityId;
                                    UserSubscription.IsComplex = true;
                                    tx.PersistenceManager.CommonRepository.Save<UserSubscriptionDao>(UserSubscription);
                                }
                                tx.Commit();
                            }
                            return true;
                        }
                        catch (Exception)
                        {
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return false;
        }

        /// <summary>
        /// Updates the user single entity subscription.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="EntityId">The entity id.</param>
        /// <param name="Id">The id.</param>
        /// <param name="Caption">The caption.</param>
        /// <param name="IsAutomated">if set to <c>true</c> [is automated].</param>
        /// <param name="Userid">The userid.</param>
        /// <returns>bool</returns>
        public bool InsertUserSingleEntitySubscription(CommonManagerProxy proxy, int UserId, int EntityId, int EntitytypeId, DateTimeOffset SubscribedOn, DateTimeOffset LastUpdatedOn, string issubscribe, int filteroption)
        {

            try
            {

                // if (proxy.MarcomManager.AccessManager.CheckAccess(Modules.Admin, 4, FeatureID.Report, OperationId.Self) == true)
                // {
                // if (proxy.MarcomManager.AccessManager.CheckAccess(Modules.Admin, 4, FeatureID.Report, OperationId.Self, 1) == true)
                // {
                try
                {

                    UserSubscriptionDao UserSubscription = new UserSubscriptionDao();
                    IList<UserSubscriptionDao> UserList;
                    using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                    {
                        UserList = tx.PersistenceManager.CommonRepository.GetAll<UserSubscriptionDao>();

                        UserMultiSubscriptionDao multiSubscriptionDaoGet = new UserMultiSubscriptionDao();
                        UserAutoSubscriptionDao autoSubscriptionDaoGet = new UserAutoSubscriptionDao();

                        IList<MultiProperty> prplst = new List<MultiProperty>();
                        prplst.Add(new MultiProperty { propertyName = UserSubscriptionDao.PropertyNames.Entityid, propertyValue = EntityId });
                        prplst.Add(new MultiProperty { propertyName = UserSubscriptionDao.PropertyNames.Userid, propertyValue = UserId });

                        tx.PersistenceManager.CommonRepository.DeleteByID<UserSubscriptionDao>(prplst);

                        if (UserList.Count() > 0)
                        {
                            IList<MultiProperty> prplstauto = new List<MultiProperty>();
                            prplstauto.Add(new MultiProperty { propertyName = UserAutoSubscriptionDao.PropertyNames.Subscriptionid, propertyValue = UserList[0].Id });

                            tx.PersistenceManager.CommonRepository.DeleteByID<UserAutoSubscriptionDao>(prplstauto);

                            IList<MultiProperty> prplstmulti = new List<MultiProperty>();
                            prplstmulti.Add(new MultiProperty { propertyName = UserMultiSubscriptionDao.PropertyNames.Subscriptionid, propertyValue = UserList[0].Id });

                            tx.PersistenceManager.CommonRepository.DeleteByID<UserMultiSubscriptionDao>(prplstmulti);


                        }
                        if (issubscribe == "true")
                        {
                            UserSubscription.Userid = UserId;
                            UserSubscription.SubscribedOn = SubscribedOn;
                            UserSubscription.LastUpdatedOn = LastUpdatedOn;
                            UserSubscription.IsComplex = true;
                            UserSubscription.IsMultiLevel = false;
                            UserSubscription.Entityid = EntityId;
                            UserSubscription.EntityTypeid = EntitytypeId;
                            UserSubscription.FilterRoleOption = filteroption;
                            tx.PersistenceManager.CommonRepository.Save<UserSubscriptionDao>(UserSubscription);
                        }

                        tx.Commit();
                    }
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
                // }
                //}
            }
            catch (Exception ex)
            {

            }
            return false;
        }

        //Used to get all the selected multilevel subscription types
        //This method will return 2nd array list
        //1st array contains all different types of subscriptions choosen
        //2nd array contans type of level, Entitytype
        /// <summary>
        ///  loads User multi subscription.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="EntityId">The entity id.</param>
        /// <param name="Userid">The userid.</param>
        /// <returns>int</returns>
        public Tuple<int[], int, int> UserMultiSubscriptionLoad(CommonManagerProxy proxy, int EntityId, int Userid)
        {

            try
            {
                // if (proxy.MarcomManager.AccessManager.CheckAccess(Modules.Admin, 4, FeatureID.Report, OperationId.Self) == true)
                // {
                // if (proxy.MarcomManager.AccessManager.CheckAccess(Modules.Admin, 4, FeatureID.Report, OperationId.Self, 1) == true)
                // {
                UserSubscriptionDao userSubscriptionDao = new UserSubscriptionDao();
                IList<UserSubscriptionDao> UserSubsList;
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    UserSubsList = tx.PersistenceManager.CommonRepository.GetAll<UserSubscriptionDao>();
                    tx.Commit();
                    var z = from t in UserSubsList where t.Userid == Userid && t.Entityid == EntityId select t;
                    if (z.Count() > 0)
                    {
                        userSubscriptionDao.Id = z.ElementAt(0).Id;
                        userSubscriptionDao.FilterRoleOption = z.ElementAt(0).FilterRoleOption;
                        userSubscriptionDao.EntityTypeid = z.ElementAt(0).EntityTypeid;

                        UserMultiSubscriptionDao multiSubscriptionDao = new UserMultiSubscriptionDao();
                        IList<UserMultiSubscriptionDao> MultiSubs;
                        MultiSubs = tx.PersistenceManager.CommonRepository.GetAll<UserMultiSubscriptionDao>();
                        tx.Commit();
                        var x = from j in MultiSubs where j.Subscriptionid.Id == userSubscriptionDao.Id select j;
                        int[][] scores = new int[2][];
                        int[,] matrix = new int[2, x.Count()];
                        int[] arr = new int[x.Count() + 1];
                        int k = 0;

                        for (int i = 0; i < 1; i++)
                        {
                            arr[k] = Convert.ToInt32(z.ElementAt(0).EntityTypeid);
                            k = 1;
                            for (int j = 0; j < x.Count(); j++)
                            {
                                //matrix[i, j] = x.ElementAt(j).EntityTypeid;
                                arr[k] = Convert.ToInt32(x.ElementAt(j).EntityTypeid);
                                k = k + 1;
                            }
                            /// k = k + 1;

                        }

                        //matrix[1, 0] = Convert.ToInt32(z.ElementAt(0).IsMultiLevel);
                        //matrix[1, 1] = z.ElementAt(0).EntityTypeid;                             
                        var tuple = Tuple.Create(arr, Convert.ToInt32(z.ElementAt(0).IsMultiLevel), Convert.ToInt32(z.ElementAt(0).FilterRoleOption));
                        return tuple;
                    }
                    else
                        return null;
                }
                //}
                //}
            }
            catch (Exception ex)
            {

            }

            return null;
        }

        //if user choosen multiple subscription, we need to save in multisucbscription table and usersubscription table.
        //if user want to modify subscription types, delete from the multisubscription table and from the usersubscription tbale, then 
        //enter the values in both the tables.
        /// <summary>
        /// Saves the update multi subscription.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="levels">The levels.</param>
        /// <param name="EntityId">The entity id.</param>
        /// <param name="Userid">The userid.</param>
        /// <param name="IsMultiLevel">if set to <c>true</c> [is multi level].</param>
        /// <param name="EntityTypeId">The entity type id.</param>
        /// <returns>String</returns>
        public String SaveUpdateMultiSubscription(CommonManagerProxy proxy, int[] levels, int EntityId, int Userid, bool IsMultiLevel, int EntityTypeId, DateTimeOffset SubscribedOn, DateTimeOffset LastUpdatedOn, int filteroption)
        {
            //if (proxy.MarcomManager.AccessManager.CheckAccess(Modules.Admin, 4, FeatureID.Report, OperationId.Self) == true)
            //{
            //  if (proxy.MarcomManager.AccessManager.CheckAccess(Modules.Admin, 4, FeatureID.Report, OperationId.Self, 1) == true)
            // {
            try
            {
                UserMultiSubscriptionDao multiSubscriptionDaoDel = new UserMultiSubscriptionDao();
                UserMultiSubscriptionDao multiSubscriptionDaoGet = new UserMultiSubscriptionDao();
                UserMultiSubscriptionDao multiSubscriptionDaoSave = new UserMultiSubscriptionDao();

                UserAutoSubscriptionDao autoSubscriptionDaoGet = new UserAutoSubscriptionDao();
                UserAutoSubscriptionDao autoSubscriptionDaoSave = new UserAutoSubscriptionDao();
                UserAutoSubscriptionDao autoSubscriptionDaoDel = new UserAutoSubscriptionDao();

                SubscriptionTypeDao STD = new SubscriptionTypeDao();
                UserSubscriptionDao userSubscriptionDao1 = new UserSubscriptionDao();
                userSubscriptionDao1.Entityid = EntityId;
                userSubscriptionDao1.Userid = Userid;
                IList<UserSubscriptionDao> UserSubsList;
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    //get details from the UserSubscription table
                    UserSubsList = tx.PersistenceManager.CommonRepository.GetAll<UserSubscriptionDao>();
                    tx.Commit();
                }
                //get details for the logged in userid and selected entity
                var z = from t in UserSubsList where t.Userid == Userid && t.Entityid == EntityId select t;
                //if userid and entityid is available in the UserSubscription table, delete and save else save
                UserSubscriptionDao userSubscriptionDao = new UserSubscriptionDao();
                UserSubscriptionDao userSubscriptionDaoDel = new UserSubscriptionDao();
                if (z.Count() > 0)
                {
                    STD.Id = z.ElementAt(0).Id;
                    multiSubscriptionDaoGet.Subscriptionid = STD;
                    IList<UserMultiSubscriptionDao> Multisubs;
                    IList<UserAutoSubscriptionDao> Autosubs;
                    using (ITransaction tx1 = proxy.MarcomManager.GetTransaction())
                    {
                        Multisubs = tx1.PersistenceManager.CommonRepository.GetAll<UserMultiSubscriptionDao>();
                        Autosubs = tx1.PersistenceManager.CommonRepository.GetAll<UserAutoSubscriptionDao>();
                        tx1.Commit();
                    }
                    var multi = from table in Multisubs where table.Subscriptionid.Id == STD.Id select table;
                    var auto = from table in Autosubs where table.Subscriptionid.Id == STD.Id select table;

                    if (multi.Count() > 0)
                    {
                        for (int k = 0; k < multi.Count(); k++)
                        {
                            using (ITransaction tx6 = proxy.MarcomManager.GetTransaction())
                            {
                                multiSubscriptionDaoGet.EntityTypeid = multi.ElementAt(k).EntityTypeid;

                                tx6.PersistenceManager.CommonRepository.Delete<UserMultiSubscriptionDao>(multiSubscriptionDaoGet);
                                tx6.Commit();
                            }
                        }
                    }
                    if (auto.Count() > 0)
                    {
                        for (int k = 0; k < auto.Count(); k++)
                        {
                            using (ITransaction tx6 = proxy.MarcomManager.GetTransaction())
                            {
                                autoSubscriptionDaoGet.EntityTypeid = auto.ElementAt(k).EntityTypeid;
                                autoSubscriptionDaoGet.Entityid = auto.ElementAt(k).Entityid;
                                autoSubscriptionDaoGet.Userid = auto.ElementAt(k).Userid;
                                autoSubscriptionDaoGet.Subscriptionid = auto.ElementAt(k).Subscriptionid;
                                tx6.PersistenceManager.CommonRepository.Delete<UserAutoSubscriptionDao>(autoSubscriptionDaoGet);
                                tx6.Commit();
                            }
                        }
                    }
                    using (ITransaction tx2 = proxy.MarcomManager.GetTransaction())
                    {
                        userSubscriptionDaoDel.Id = z.ElementAt(0).Id;
                        userSubscriptionDaoDel.Entityid = EntityId;
                        userSubscriptionDaoDel.EntityTypeid = EntityTypeId;
                        userSubscriptionDaoDel.IsComplex = true;
                        userSubscriptionDaoDel.IsMultiLevel = IsMultiLevel;
                        userSubscriptionDaoDel.SubscribedOn = z.ElementAt(0).SubscribedOn;
                        userSubscriptionDaoDel.LastUpdatedOn = DateTimeOffset.MinValue;
                        userSubscriptionDaoDel.Userid = Userid;

                        tx2.PersistenceManager.CommonRepository.Delete<UserSubscriptionDao>(userSubscriptionDaoDel);
                        tx2.Commit();
                    }
                    using (ITransaction tx3 = proxy.MarcomManager.GetTransaction())
                    {
                        userSubscriptionDao.Entityid = EntityId;
                        userSubscriptionDao.EntityTypeid = EntityTypeId;
                        userSubscriptionDao.IsComplex = true;
                        userSubscriptionDao.IsMultiLevel = IsMultiLevel;
                        userSubscriptionDao.SubscribedOn = z.ElementAt(0).SubscribedOn;
                        userSubscriptionDao.LastUpdatedOn = DateTimeOffset.Now;
                        userSubscriptionDao.Userid = Userid;
                        userSubscriptionDao.FilterRoleOption = filteroption;
                        tx3.PersistenceManager.CommonRepository.Save<UserSubscriptionDao>(userSubscriptionDao);
                        tx3.Commit();
                    }

                }
                else
                {
                    userSubscriptionDao.Entityid = EntityId;
                    userSubscriptionDao.EntityTypeid = EntityTypeId;
                    userSubscriptionDao.IsComplex = true;
                    userSubscriptionDao.IsMultiLevel = IsMultiLevel;
                    userSubscriptionDao.SubscribedOn = DateTimeOffset.Now;
                    userSubscriptionDao.LastUpdatedOn = DateTimeOffset.MinValue;
                    userSubscriptionDao.Userid = Userid;
                    userSubscriptionDao.FilterRoleOption = filteroption;

                    using (ITransaction tx4 = proxy.MarcomManager.GetTransaction())
                    {
                        tx4.PersistenceManager.CommonRepository.Save<UserSubscriptionDao>(userSubscriptionDao);
                        tx4.Commit();
                    }
                }
                int ID = userSubscriptionDao.Id;
                STD.Id = ID;
                multiSubscriptionDaoSave.Subscriptionid = STD;
                for (int i = 0; i < levels.Count(); i++)
                {
                    if (i != 0)
                    {
                        multiSubscriptionDaoSave.EntityTypeid = levels[i];

                        using (ITransaction tx5 = proxy.MarcomManager.GetTransaction())
                        {
                            tx5.PersistenceManager.CommonRepository.Save<UserMultiSubscriptionDao>(multiSubscriptionDaoSave);
                            tx5.Commit();
                        }
                    }
                }
                STD.Id = ID;


                var typeids = string.Join(", ", levels);

                StringBuilder str = new StringBuilder();
                str.AppendLine("SELECT ID,typeid");
                str.AppendLine("FROM PM_Entity");
                str.AppendLine("WHERE UniqueKey");
                str.AppendLine("LIKE CONCAT('%',(SELECT UniqueKey FROM PM_Entity where ID=" + EntityId + " ),'%') and TypeID in (" + typeids + ")");
                using (ITransaction tx6 = proxy.MarcomManager.GetTransaction())
                {
                    var ListofChildEntities = ((tx6.PersistenceManager.CommonRepository.ExecuteQuery(str.ToString())).Cast<Hashtable>().ToList());

                    IList<UserAutoSubscriptionDao> autoSubscriptionDaoSavelist = new List<UserAutoSubscriptionDao>();

                    for (int i = 0; i < ListofChildEntities.Count(); i++)
                    {
                        if (Convert.ToInt32(ListofChildEntities[i]["typeid"]) != levels[0])
                        {
                            UserAutoSubscriptionDao autoSubscriptionDaoSave1 = new UserAutoSubscriptionDao();

                            autoSubscriptionDaoSave1.Subscriptionid = STD;
                            autoSubscriptionDaoSave1.Userid = Userid;
                            autoSubscriptionDaoSave1.Entityid = Convert.ToInt32(ListofChildEntities[i]["ID"]);
                            autoSubscriptionDaoSave1.EntityTypeid = Convert.ToInt32(ListofChildEntities[i]["typeid"]);

                            autoSubscriptionDaoSavelist.Add(autoSubscriptionDaoSave1);
                        }
                    }
                    tx6.PersistenceManager.CommonRepository.Save<UserAutoSubscriptionDao>(autoSubscriptionDaoSavelist);
                    tx6.Commit();

                }
                return "Success";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            //  }
            //     return null;
            // }
            return null;
        }

        //If user want to unsubscribe the multilevel subscription, delete records from the usersubscription table and from the multisubscription table.
        /// <summary>
        /// Unsubscribes the multi subscription.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="EntityId">The entity id.</param>
        /// <param name="Userid">The userid.</param>
        /// <returns>bool</returns>
        public bool UnsubscribeMultiSubscription(CommonManagerProxy proxy, int EntityId, int Userid)
        {
            //if (proxy.MarcomManager.AccessManager.CheckAccess(Modules.Admin, 4, FeatureID.Report, OperationId.Self) == true)
            //{
            //  if (proxy.MarcomManager.AccessManager.CheckAccess(Modules.Admin, 4, FeatureID.Report, OperationId.Self, 1) == true)
            //{
            try
            {
                UserSubscriptionDao userSubscriptionDao1 = new UserSubscriptionDao();
                UserSubscriptionDao userSubscriptionDaoDel = new UserSubscriptionDao();

                userSubscriptionDao1.Entityid = EntityId;
                userSubscriptionDao1.Userid = Userid;
                IList<UserSubscriptionDao> UserSubsList;
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    //get details from the UserSubscription table
                    UserSubsList = tx.PersistenceManager.CommonRepository.GetAll<UserSubscriptionDao>();
                    tx.Commit();
                }
                var z = from t in UserSubsList where t.Userid == Userid && t.Entityid == EntityId select t;
                SubscriptionTypeDao STD = new SubscriptionTypeDao();
                UserMultiSubscriptionDao multiSubscriptionDaoGet = new UserMultiSubscriptionDao();
                UserAutoSubscriptionDao autoSubscriptionDaoGet = new UserAutoSubscriptionDao();
                STD.Id = z.ElementAt(0).Id;
                multiSubscriptionDaoGet.Subscriptionid = STD;
                IList<UserMultiSubscriptionDao> Multisubs;
                IList<UserAutoSubscriptionDao> Autosubs;

                using (ITransaction tx1 = proxy.MarcomManager.GetTransaction())
                {
                    Multisubs = tx1.PersistenceManager.CommonRepository.GetAll<UserMultiSubscriptionDao>();
                    Autosubs = tx1.PersistenceManager.CommonRepository.GetAll<UserAutoSubscriptionDao>();
                    tx1.Commit();
                }
                var multi = from table in Multisubs where table.Subscriptionid.Id == STD.Id select table;
                var auto = from table1 in Autosubs where table1.Subscriptionid.Id == STD.Id select table1;
                if (multi.Count() > 0)
                {
                    for (int k = 0; k < multi.Count(); k++)
                    {
                        using (ITransaction tx6 = proxy.MarcomManager.GetTransaction())
                        {
                            multiSubscriptionDaoGet.EntityTypeid = multi.ElementAt(k).EntityTypeid;
                            tx6.PersistenceManager.CommonRepository.Delete<UserMultiSubscriptionDao>(multiSubscriptionDaoGet);
                            tx6.Commit();
                        }
                    }
                }
                if (auto.Count() > 0)
                {
                    for (int k = 0; k < auto.Count(); k++)
                    {
                        using (ITransaction tx6 = proxy.MarcomManager.GetTransaction())
                        {
                            autoSubscriptionDaoGet.EntityTypeid = auto.ElementAt(k).EntityTypeid;
                            autoSubscriptionDaoGet.Entityid = auto.ElementAt(k).Entityid;
                            autoSubscriptionDaoGet.Userid = auto.ElementAt(k).Userid;
                            autoSubscriptionDaoGet.Subscriptionid = auto.ElementAt(k).Subscriptionid;
                            tx6.PersistenceManager.CommonRepository.Delete<UserAutoSubscriptionDao>(autoSubscriptionDaoGet);
                            tx6.Commit();
                        }
                    }
                }
                using (ITransaction tx2 = proxy.MarcomManager.GetTransaction())
                {
                    userSubscriptionDaoDel.Id = z.ElementAt(0).Id;
                    userSubscriptionDaoDel.Entityid = EntityId;
                    userSubscriptionDaoDel.EntityTypeid = z.ElementAt(0).EntityTypeid;
                    userSubscriptionDaoDel.IsComplex = true;
                    userSubscriptionDaoDel.IsMultiLevel = z.ElementAt(0).IsMultiLevel;
                    userSubscriptionDaoDel.SubscribedOn = z.ElementAt(0).SubscribedOn;
                    userSubscriptionDaoDel.LastUpdatedOn = DateTimeOffset.MinValue;
                    userSubscriptionDaoDel.Userid = Userid;
                    tx2.PersistenceManager.CommonRepository.Delete<UserSubscriptionDao>(userSubscriptionDaoDel);
                    tx2.Commit();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            //}
            // return false;
            //}
            return false;
        }

        //Save or update the CM_Feed_Template table
        /// <summary>
        /// Saves/update feed template.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="ModuleId">The module id.</param>
        /// <param name="FeatureId">The feature id.</param>
        /// <param name="Template">The template.</param>
        /// <returns>int</returns>
        public int SaveUpdateFeedTemplate(CommonManagerProxy proxy, int ModuleId, int FeatureId, String Template)
        {
            if (proxy.MarcomManager.AccessManager.CheckAccess(Modules.Admin, 4, FeatureID.Report, OperationId.Self) == true)
            {
                if (proxy.MarcomManager.AccessManager.CheckAccess(Modules.Admin, 4, FeatureID.Report, OperationId.Self, 1) == true)
                {
                    try
                    {
                        FeedTemplateDao feedTemplateDao = new FeedTemplateDao();
                        feedTemplateDao.Moduleid = ModuleId;
                        feedTemplateDao.Featureid = FeatureId;
                        feedTemplateDao.Template = Template;
                        IList<FeedTemplateDao> FeedTemplate;
                        using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                        {
                            FeedTemplate = tx.PersistenceManager.CommonRepository.GetAll<FeedTemplateDao>();
                            var multi = from table in FeedTemplate where table.Moduleid == ModuleId && table.Featureid == FeatureId select table;
                            if (multi.Count() > 0)
                            {
                                feedTemplateDao.Id = multi.ElementAt(0).Id;
                            }
                            tx.PersistenceManager.CommonRepository.Save<FeedTemplateDao>(feedTemplateDao);
                            tx.Commit();
                        }
                        IFeedTemplate feedtemp = new FeedTemplate();
                        feedtemp.Featureid = feedTemplateDao.Featureid;
                        feedtemp.Id = feedTemplateDao.Id;
                        feedtemp.Moduleid = feedTemplateDao.Moduleid;
                        feedtemp.Template = feedTemplateDao.Template;
                        return feedtemp.Id;
                    }
                    catch (Exception)
                    {
                        return 0;
                    }
                }
            } return 0;
        }

        //Save or Update the CM_Feed table
        /// <summary>
        /// Saves/Update update feed.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="Actor">The actor.</param>
        /// <param name="TemplateId">The template id.</param>
        /// <param name="EntityId">The entity id.</param>
        /// <param name="TypeName">Name of the type.</param>
        /// <param name="AttributeName">Name of the attribute.</param>
        /// <param name="FromValue">From value.</param>
        /// <param name="ToValue">To value.</param>
        /// <returns>Last Inserted Feed ID</returns>
        public int SaveUpdateFeed(CommonManagerProxy proxy, int Actor, int TemplateId, int EntityId, String TypeName, String AttributeName, String FromValue, String ToValue, int Userid, int associatedentityid, string attributeGroupRecordName, int Version)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    if ((TemplateId == 2 || TemplateId == 40 || TemplateId == 104))
                    {
                        if ((ToValue.Trim().Length == 0 || ToValue.Trim().Length == -1))
                        {
                            return -1;
                        }
                        else
                        {
                            IFeed feedList = new Feed();
                            FeedDao feedObj = new FeedDao();
                            feedObj.Actor = Actor;
                            feedObj.Templateid = TemplateId;
                            feedObj.Entityid = EntityId;
                            feedObj.TypeName = TypeName;
                            feedObj.HappenedOn = DateTimeOffset.UtcNow;
                            feedObj.CommentedUpdatedOn = DateTimeOffset.MinValue;
                            feedObj.AttributeName = AttributeName;
                            feedObj.FromValue = (FromValue == "" ? FromValue = " - " : FromValue);
                            ToValue = (ToValue == "" ? ToValue = " - " : ToValue);
                            feedObj.ToValue = HttpUtility.HtmlEncode(ToValue);
                            feedObj.UserID = Userid;
                            feedObj.AssocitedEntityID = associatedentityid;
                            feedObj.AttributeGroupRecordName = attributeGroupRecordName;
                            feedObj.Version = Version;
                            tx.PersistenceManager.CommonRepository.Save<FeedDao>(feedObj);
                            //tx.Commit();

                            if (TemplateId == 2 || TemplateId == 40 || TemplateId == 104)
                            {
                                FeedNotificationServer fs = new FeedNotificationServer();
                                NotificationFeedObjects obj = new NotificationFeedObjects();
                                obj.Actorid = Actor;
                                obj.action = "comment added";
                                obj.EntityId = EntityId;
                                if (TemplateId == 104)
                                {
                                    obj.AssociatedEntityId = associatedentityid;
                                    obj.Version = Version;
                                }
                                obj.ToValue = HttpUtility.HtmlEncode(ToValue.ToString());
                                fs.AsynchronousNotify(obj);
                            }
                            tx.Commit();
                            return feedObj.Id;
                        }
                    }
                    else
                    {
                        IFeed feedList = new Feed();
                        FeedDao feedObj = new FeedDao();
                        feedObj.Actor = Actor;
                        feedObj.Templateid = TemplateId;
                        feedObj.Entityid = EntityId;
                        feedObj.TypeName = TypeName;
                        feedObj.HappenedOn = DateTimeOffset.UtcNow;
                        feedObj.CommentedUpdatedOn = DateTimeOffset.MinValue;
                        feedObj.AttributeName = AttributeName;
                        feedObj.FromValue = (FromValue == "" ? FromValue = " - " : FromValue);
                        ToValue = (ToValue == "" ? ToValue = " - " : ToValue);
                        feedObj.ToValue = HttpUtility.HtmlEncode(ToValue);
                        feedObj.UserID = Userid;
                        feedObj.AssocitedEntityID = associatedentityid;
                        feedObj.AttributeGroupRecordName = attributeGroupRecordName;
                        feedObj.Version = Version;
                        tx.PersistenceManager.CommonRepository.Save<FeedDao>(feedObj);
                        //tx.Commit();

                        //if (TemplateId == 2 || TemplateId == 40 || TemplateId == 104)
                        //{
                        //    FeedNotificationServer fs = new FeedNotificationServer();
                        //    NotificationFeedObjects obj = new NotificationFeedObjects();
                        //    obj.Actorid = Actor;
                        //    obj.action = "comment added";
                        //    obj.EntityId = EntityId;
                        //    if (TemplateId == 104)
                        //    {
                        //        obj.AssociatedEntityId = associatedentityid;
                        //        obj.Version = Version;
                        //    }
                        //    obj.ToValue = HttpUtility.HtmlEncode(ToValue.ToString());
                        //    fs.AsynchronousNotify(obj);
                        //}
                        tx.Commit();
                        return feedObj.Id;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets the type of the notification BY.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="pCaption">The p caption.</param>
        /// <returns></returns>
        public INotificationType GetNotificationBYType(CommonManagerProxy proxy, string pCaption)
        {

            try
            {

                if (proxy.MarcomManager.AccessManager.CheckAccess(Modules.Admin, 4, FeatureID.Report, OperationId.Self) == true)
                {
                    using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                    {
                        var notificationtionRow = tx.PersistenceManager.CommonRepository.Get<NotificationTypeDao>(NotificationTypeDao.PropertyNames.Caption, pCaption);
                        tx.Commit();
                        INotificationType notify = new NotificationType();
                        notify.Id = notificationtionRow.Id;
                        notify.Caption = notificationtionRow.Caption;
                        ISubscriptionType subScrip = new SubscriptionType();
                        SubscriptionTypeDao subdao = new SubscriptionTypeDao();
                        subdao = notificationtionRow.SubscriptionTypeid;
                        subScrip.Id = subdao.Id;
                        subScrip.Caption = subdao.Caption;
                        //  subScrip.IsAutomated = subdao.IsAutomated;
                        notify.SubscriptionTypeid = subScrip;
                        notify.Template = notificationtionRow.Template;
                        return notify;
                    }

                }
            }
            catch (Exception ex)
            {

            }

            return null;
        }

        /// <summary>
        /// Gets the user default subscription.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="SubScriptionTypeId">The sub scription type id.</param>
        /// <returns>IList</returns>
        public IList<IUserDefaultSubscription> GetUserDefaultSubscription(CommonManagerProxy proxy, ISubscriptionType SubScriptionTypeId)
        {

            try
            {
                IList<IUserDefaultSubscription> UserDefsubscriptiontype = new List<IUserDefaultSubscription>();
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    SubscriptionTypeDao subdao = new SubscriptionTypeDao();
                    subdao.Id = SubScriptionTypeId.Id;
                    subdao.Caption = SubScriptionTypeId.Caption;
                    //  subdao.IsAutomated = SubScriptionTypeId.IsAutomated;
                    var dao = tx.PersistenceManager.CommonRepository.GetAll<UserDefaultSubscriptionDao>();
                    tx.Commit();
                    foreach (var userDefSubscriptionRow in dao.ToList())
                    {
                        IUserDefaultSubscription sub = new UserDefaultSubscription();
                        sub.Userid = userDefSubscriptionRow.UserID;
                        ISubscriptionType subscription = new SubscriptionType();
                        // subscription.Id = (userDefSubscriptionRow.SubscriptionTypeid).Id;
                        //  subscription.Caption = (userDefSubscriptionRow.SubscriptionTypeid).Caption;
                        //subscription.IsAutomated = (userDefSubscriptionRow.SubscriptionTypeid).IsAutomated;
                        // sub.SubscriptionTypeid = subscription;
                        UserDefsubscriptiontype.Add(sub);
                    }
                }
                return UserDefsubscriptiontype;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        /// <summary>
        /// Gets the user default subscription.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="Id">The id.</param>
        /// <param name="Caption">The caption.</param>
        /// <param name="IsAutomated">if set to <c>true</c> [is automated].</param>
        /// <returns>IList</returns>
        public IList<IUserDefaultSubscription> GetUserDefaultSubscription(CommonManagerProxy proxy, int Id, string Caption, bool IsAutomated)
        {
            try
            {
                ISubscriptionType SubScriptionTypeId = new SubscriptionType();
                SubScriptionTypeId.Id = Id;
                SubScriptionTypeId.Caption = Caption;
                // SubScriptionTypeId.IsAutomated = IsAutomated;
                IList<IUserDefaultSubscription> UserDefsubscriptiontype = new List<IUserDefaultSubscription>();
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    SubscriptionTypeDao subdao = new SubscriptionTypeDao();
                    subdao.Id = SubScriptionTypeId.Id;
                    subdao.Caption = SubScriptionTypeId.Caption;
                    //  subdao.IsAutomated = SubScriptionTypeId.IsAutomated;
                    var dao = tx.PersistenceManager.CommonRepository.GetAll<UserDefaultSubscriptionDao>();
                    tx.Commit();
                    foreach (var userDefSubscriptionRow in dao.ToList())
                    {
                        IUserDefaultSubscription sub = new UserDefaultSubscription();
                        sub.Userid = userDefSubscriptionRow.UserID;
                        ISubscriptionType subscription = new SubscriptionType();
                        //  subscription.Id = (userDefSubscriptionRow.SubscriptionTypeid).Id;
                        //  subscription.Caption = (userDefSubscriptionRow.SubscriptionTypeid).Caption;
                        // subscription.IsAutomated = (userDefSubscriptionRow.SubscriptionTypeid).IsAutomated;
                        //sub.SubscriptionTypeid = subscription;
                        UserDefsubscriptiontype.Add(sub);
                    }
                }
                return UserDefsubscriptiontype;
            }
            catch (Exception ex)
            {
                return null;
            }

        }



        /// <summary>
        /// inserts User notifications.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="Id">The id.</param>
        /// <param name="Userid">The userid.</param>
        /// <param name="Entityid">The entityid.</param>
        /// <param name="Actorid">The actorid.</param>
        /// <param name="CreatedOn">The created on.</param>
        /// <param name="Typeid">The typeid.</param>
        /// <param name="IsViewed">if set to <c>true</c> [is viewed].</param>
        /// <param name="IsSentInMail">if set to <c>true</c> [is sent in mail].</param>
        /// <param name="NotificationText">The notification text.</param>
        /// <returns>bool</returns>
        public bool UserNotification_Insert(CommonManagerProxy proxy, int Entityid, int Actorid, DateTimeOffset CreatedOn, int Typeid, bool IsViewed, bool IsSentInMail, string Typename, string Attributename, string Fromvalue, string Tovalue, int mailtemplateid, int userid)
        {
            using (ITransaction tx = proxy.MarcomManager.GetTransaction())
            {

                IList<MultiProperty> parLIST = new List<MultiProperty>();
                try
                {

                    IEnumerable<Hashtable> listresult;
                    StringBuilder strqry = new StringBuilder();
                    //logic to fetch all userids who have subscribed for the subscription type from cm_subscriptiontype table for each notification action.
                    //or fetch the user ids if the subscription type is mandatory.
                    strqry.AppendLine("declare @isMandatory bit");
                    parLIST.Add(new MultiProperty { propertyName = "Typeid", propertyValue = Typeid });
                    parLIST.Add(new MultiProperty { propertyName = "Entityid", propertyValue = Entityid });

                    strqry.AppendLine("select @isMandatory = isAppMandatory from CM_SubscriptionType where id in(select SubscriptionTypeID from CM_NotificationType where id= :Typeid)");
                    if (userid == 0)
                    {
                        parLIST.Add(new MultiProperty { propertyName = "Actorid", propertyValue = Actorid });
                        strqry.AppendLine("if (@isMandatory=0)");
                        strqry.AppendLine("begin");

                        strqry.AppendLine("IF EXISTS (select UserID from TM_Task_Members where TaskID= :Entityid and UserID in");
                        strqry.AppendLine("(select distinct UserID");
                        strqry.AppendLine("from   CM_UserDefaultSubscription a cross apply");
                        strqry.AppendLine("dbo.tf_SplitString(a.SubscriptionTypeID,',') b");
                        strqry.AppendLine("where  b.Value =(select SubscriptionTypeID from");
                        strqry.AppendLine("CM_NotificationType where id= :Typeid))) ");
                        strqry.AppendLine("begin");

                        strqry.AppendLine("select distinct UserID from TM_Task_Members where TaskID= :Entityid and  UserID <> :Actorid and UserID in");
                        strqry.AppendLine("(select distinct UserID");
                        strqry.AppendLine("from   CM_UserDefaultSubscription a cross apply");
                        strqry.AppendLine("dbo.tf_SplitString(a.SubscriptionTypeID,',') b");
                        strqry.AppendLine("where  b.Value =(select SubscriptionTypeID from");
                        strqry.AppendLine("CM_NotificationType where id= :Typeid))");
                        strqry.AppendLine("end");

                        strqry.AppendLine("else if EXISTS(select UserID from AM_Entity_Role_User where EntityID= :Entityid and UserID in");
                        strqry.AppendLine("(select distinct UserID");
                        strqry.AppendLine("from   CM_UserDefaultSubscription a cross apply");
                        strqry.AppendLine("dbo.tf_SplitString(a.SubscriptionTypeID,',') b");
                        strqry.AppendLine("where  b.Value =(select SubscriptionTypeID from");
                        strqry.AppendLine("CM_NotificationType where id=:Typeid))) ");
                        strqry.AppendLine("begin");

                        strqry.AppendLine("select distinct UserID from AM_Entity_Role_User where EntityID=:Entityid and UserID <> :Actorid and UserID in ");
                        strqry.AppendLine("(select distinct UserID");
                        strqry.AppendLine("from   CM_UserDefaultSubscription a cross apply");
                        strqry.AppendLine("dbo.tf_SplitString(a.SubscriptionTypeID,',') b");
                        strqry.AppendLine("where  b.Value =(select SubscriptionTypeID from");
                        strqry.AppendLine("CM_NotificationType where id=:Typeid))");

                        strqry.AppendLine("end");

                        strqry.AppendLine("else if EXISTS(select UserID from PM_Task_Members where TaskID= :Entityid  and UserID<> :Actorid and UserID in");
                        strqry.AppendLine("(select distinct UserID");
                        strqry.AppendLine("from   CM_UserDefaultSubscription a cross apply");
                        strqry.AppendLine("dbo.tf_SplitString(a.SubscriptionTypeID,',') b");
                        strqry.AppendLine("where  b.Value =(select SubscriptionTypeID from");
                        strqry.AppendLine("CM_NotificationType where id= :Typeid))) ");
                        strqry.AppendLine("begin");

                        strqry.AppendLine("select distinct UserID from PM_Task_Members where TaskID=:Entityid and UserID<> :Actorid  and UserID in");
                        strqry.AppendLine("(select distinct UserID");
                        strqry.AppendLine("from   CM_UserDefaultSubscription a cross apply");
                        strqry.AppendLine("dbo.tf_SplitString(a.SubscriptionTypeID,',') b");
                        strqry.AppendLine("where  b.Value =(select SubscriptionTypeID from");
                        strqry.AppendLine("CM_NotificationType where id= :Typeid)) ");

                        strqry.AppendLine("end");

                        strqry.AppendLine("end");

                        strqry.AppendLine("else");
                        strqry.AppendLine("begin");

                        strqry.AppendLine("declare @subscriptionType bit");
                        strqry.AppendLine("select @subscriptionType = SubscriptionTypeID from CM_NotificationType where id= :Typeid");
                        strqry.AppendLine("if(@subscriptionType!=0) ");
                        strqry.AppendLine("begin ");

                        strqry.AppendLine("if EXISTS(select  distinct UserID from AM_Entity_Role_User where EntityID= :Entityid)");
                        strqry.AppendLine("begin");
                        strqry.AppendLine("select distinct UserID from AM_Entity_Role_User where EntityID= :Entityid");
                        strqry.AppendLine("end");
                        strqry.AppendLine("else IF EXISTS (select UserID from TM_Task_Members where TaskID= :Entityid )");
                        strqry.AppendLine("begin");
                        strqry.AppendLine("select distinct UserID from TM_Task_Members where TaskID= :Entityid");
                        strqry.AppendLine("end");
                        strqry.AppendLine("else if EXISTS(select UserID from PM_Task_Members where TaskID= :Entityid)");
                        strqry.AppendLine("begin");
                        strqry.AppendLine("select distinct UserID from PM_Task_Members where TaskID= :Entityid");      //role id 1 => user of person who has reaised fund request
                        strqry.AppendLine("end");
                        strqry.AppendLine("end");

                        strqry.AppendLine("end");


                    }
                    else
                    {
                        parLIST.Add(new MultiProperty { propertyName = "Userid", propertyValue = userid });

                        strqry.AppendLine("if (@isMandatory=0)");
                        strqry.AppendLine("begin");

                        strqry.AppendLine("if EXISTS(select UserID from AM_Entity_Role_User where EntityID= :Entityid  and UserID in");
                        strqry.AppendLine("(select distinct UserID");
                        strqry.AppendLine("from   CM_UserDefaultSubscription a cross apply");
                        strqry.AppendLine("dbo.tf_SplitString(a.SubscriptionTypeID,',') b");
                        strqry.AppendLine("where  b.Value =(select SubscriptionTypeID from");
                        strqry.AppendLine("CM_NotificationType where id= :Typeid)and UserID= :Userid )) ");
                        strqry.AppendLine("begin");

                        strqry.AppendLine("select distinct UserID from AM_Entity_Role_User where EntityID= :Entityid and UserID in");
                        strqry.AppendLine("(select distinct UserID");
                        strqry.AppendLine("from   CM_UserDefaultSubscription a cross apply");
                        strqry.AppendLine("dbo.tf_SplitString(a.SubscriptionTypeID,',') b");
                        strqry.AppendLine("where  b.Value =(select SubscriptionTypeID from");
                        strqry.AppendLine("CM_NotificationType where id= :Typeid)and UserID= :Userid )");
                        strqry.AppendLine("end");

                        strqry.AppendLine("else IF EXISTS (select UserID from TM_Task_Members where TaskID= :Entityid and UserID in");
                        strqry.AppendLine("(select distinct UserID");
                        strqry.AppendLine("from   CM_UserDefaultSubscription a cross apply");
                        strqry.AppendLine("dbo.tf_SplitString(a.SubscriptionTypeID,',') b");
                        strqry.AppendLine("where  b.Value =(select SubscriptionTypeID from");
                        strqry.AppendLine("CM_NotificationType where id= :Typeid)and UserID= :Userid )) ");
                        strqry.AppendLine("begin");

                        strqry.AppendLine("select distinct UserID from TM_Task_Members where TaskID= :Entityid and UserID in");
                        strqry.AppendLine("(select distinct UserID");
                        strqry.AppendLine("from   CM_UserDefaultSubscription a cross apply");
                        strqry.AppendLine("dbo.tf_SplitString(a.SubscriptionTypeID,',') b");
                        strqry.AppendLine("where  b.Value =(select SubscriptionTypeID from");
                        strqry.AppendLine("CM_NotificationType where id= :Typeid)and UserID= :Userid )");

                        strqry.AppendLine("end");

                        strqry.AppendLine("else if EXISTS(select UserID from PM_Task_Members where TaskID= :Entityid and UserID in");
                        strqry.AppendLine("(select distinct UserID");
                        strqry.AppendLine("from   CM_UserDefaultSubscription a cross apply");
                        strqry.AppendLine("dbo.tf_SplitString(a.SubscriptionTypeID,',') b");
                        strqry.AppendLine("where  b.Value =(select SubscriptionTypeID from");
                        strqry.AppendLine("CM_NotificationType where id= :Typeid)and UserID= :Userid  )) ");
                        strqry.AppendLine("begin");

                        strqry.AppendLine("select distinct UserID from PM_Task_Members where TaskID= :Entityid and UserID in");    //if suppose fund is del then pass user id for tht template
                        strqry.AppendLine("(select distinct UserID");
                        strqry.AppendLine("from   CM_UserDefaultSubscription a cross apply");
                        strqry.AppendLine("dbo.tf_SplitString(a.SubscriptionTypeID,',') b");
                        strqry.AppendLine("where  b.Value =(select SubscriptionTypeID from");
                        strqry.AppendLine("CM_NotificationType where id= :Typeid)and UserID= :Userid)");

                        strqry.AppendLine("end");

                        strqry.AppendLine("else if EXISTS");
                        strqry.AppendLine("(select distinct UserID");
                        strqry.AppendLine("from   CM_UserDefaultSubscription a cross apply");
                        strqry.AppendLine("dbo.tf_SplitString(a.SubscriptionTypeID,',') b");
                        strqry.AppendLine("where  b.Value =(select SubscriptionTypeID from");
                        strqry.AppendLine("CM_NotificationType where id= :Typeid)and UserID= :Userid )");

                        strqry.AppendLine("begin");

                        strqry.AppendLine("select distinct UserID");
                        strqry.AppendLine("from   CM_UserDefaultSubscription a cross apply");
                        strqry.AppendLine("dbo.tf_SplitString(a.SubscriptionTypeID,',') b");
                        strqry.AppendLine("where  b.Value =(select SubscriptionTypeID from");
                        strqry.AppendLine("CM_NotificationType where id= :Typeid)and UserID= :Userid");

                        strqry.AppendLine("end ");

                        strqry.AppendLine("end");
                        strqry.AppendLine("else");

                        strqry.AppendLine("begin");

                        strqry.AppendLine("if EXISTS(select  distinct UserID from AM_Entity_Role_User where EntityID= :Entityid and UserID = :Userid )");
                        strqry.AppendLine("begin");
                        strqry.AppendLine("select distinct UserID from AM_Entity_Role_User where EntityID= :Entityid and UserID = :Userid");
                        strqry.AppendLine("end");
                        strqry.AppendLine("else IF EXISTS (select UserID from TM_Task_Members where TaskID= :Entityid and UserID = :Userid)");
                        strqry.AppendLine("begin");
                        strqry.AppendLine("select distinct UserID from TM_Task_Members where TaskID= :Entityid and UserID = :Userid ");
                        strqry.AppendLine("end");
                        strqry.AppendLine("else if EXISTS(select UserID from PM_Task_Members where TaskID= :Entityid and UserID = :Userid) ");
                        strqry.AppendLine("begin");
                        strqry.AppendLine("select distinct UserID from PM_Task_Members where TaskID=:Entityid  and UserID = :Userid");
                        strqry.AppendLine("end");
                        strqry.AppendLine("else if EXISTS(select UserID from CM_UserDefaultSubscription where UserID = :Userid )");
                        strqry.AppendLine("begin");
                        strqry.AppendLine("select distinct UserID from CM_UserDefaultSubscription where UserID = :Userid");
                        strqry.AppendLine("end");

                        strqry.AppendLine("else");
                        strqry.AppendLine("begin");
                        strqry.AppendLine("select  :Userid ");
                        strqry.AppendLine("end");
                        strqry.AppendLine("end");

                    }


                    listresult = tx.PersistenceManager.CommonRepository.ExecuteQuerywithParam(strqry.ToString(), parLIST).Cast<Hashtable>();
                    IList<UserNotificationDao> listuserdao = new List<UserNotificationDao>();

                    if (listresult.Count() > 0)
                    {
                        foreach (var val in listresult)
                        {
                            if (Actorid != Convert.ToInt32(val["UserID"]))
                            {
                                UserNotificationDao userNotify = new UserNotificationDao();
                                userNotify.Entityid = Convert.ToInt32(Entityid);
                                userNotify.Actorid = Actorid;
                                userNotify.CreatedOn = CreatedOn;
                                userNotify.Typeid = Typeid; // notification templateId
                                userNotify.IsViewed = false;
                                userNotify.IsSentInMail = false;
                                userNotify.TypeName = Typename;
                                userNotify.AttributeName = Attributename;
                                userNotify.FromValue = Fromvalue;
                                userNotify.ToValue = Tovalue;
                                userNotify.Userid = Convert.ToInt32(val["UserID"]);
                                listuserdao.Add(userNotify);
                            }
                        }
                        tx.PersistenceManager.CommonRepository.Save<UserNotificationDao>(listuserdao);

                    }
                    tx.Commit();
                }

                catch (Exception ex)
                {
                    tx.Rollback();
                    return false;
                }
            }
            //logic to fetch all userids who have subscribed for the mailsubscription type from cm_subscriptiontype table for each notification action.
            //or fetch the user ids if the mailsubscription type is mandatory.
            using (ITransaction txmail = proxy.MarcomManager.GetTransaction())
            {
                IList<MultiProperty> parLIST = new List<MultiProperty>();
                parLIST.Add(new MultiProperty { propertyName = "mailtemplateid", propertyValue = mailtemplateid });
                parLIST.Add(new MultiProperty { propertyName = "Entityid", propertyValue = Entityid });
                parLIST.Add(new MultiProperty { propertyName = "Actorid", propertyValue = Actorid });
                parLIST.Add(new MultiProperty { propertyName = "Userid", propertyValue = userid });

                try
                {
                    bool mailStatus = false;
                    IEnumerable<Hashtable> listOfUsersToSendMail;
                    StringBuilder strMailQuery = new StringBuilder();
                    strMailQuery.AppendLine("declare @isMailMandatory bit");
                    strMailQuery.AppendLine("select @isMailMandatory= isMailMandatory from CM_SubscriptionType where id in(select mailSubscriptionTypeID from CM_MailContent where id= :mailtemplateid)");
                    if (userid == 0)
                    {

                        strMailQuery.AppendLine("if (@isMailMandatory=0)");
                        strMailQuery.AppendLine("begin");


                        strMailQuery.AppendLine("IF EXISTS (select UserID from TM_Task_Members where TaskID= :Entityid and UserID in");
                        strMailQuery.AppendLine("(select distinct UserID ");
                        strMailQuery.AppendLine("from   CM_UserDefaultSubscription a cross apply");
                        strMailQuery.AppendLine("dbo.tf_SplitString(a.MailSubscriptionTypeID,',') b  ");
                        strMailQuery.AppendLine("where  b.Value =(select MailSubscriptionTypeID from ");
                        strMailQuery.AppendLine("CM_MailContent where id= :mailtemplateid)))");
                        strMailQuery.AppendLine("begin");
                        strMailQuery.AppendLine("select distinct UserID from TM_Task_Members where TaskID= :Entityid and  UserID<> :Actorid and UserID in");
                        strMailQuery.AppendLine("(select distinct UserID ");
                        strMailQuery.AppendLine("from   CM_UserDefaultSubscription a cross apply");
                        strMailQuery.AppendLine("dbo.tf_SplitString(a.MailSubscriptionTypeID,',') b  ");
                        strMailQuery.AppendLine("where  b.Value =(select MailSubscriptionTypeID from ");
                        strMailQuery.AppendLine("CM_MailContent where id= :mailtemplateid))");
                        strMailQuery.AppendLine("end");

                        strMailQuery.AppendLine("else if EXISTS(select UserID from AM_Entity_Role_User where EntityID= :Entityid and UserID in");
                        strMailQuery.AppendLine("(select distinct UserID ");
                        strMailQuery.AppendLine("from   CM_UserDefaultSubscription a cross apply");
                        strMailQuery.AppendLine("dbo.tf_SplitString(a.MailSubscriptionTypeID,',') b  ");
                        strMailQuery.AppendLine("where  b.Value =(select MailSubscriptionTypeID from ");
                        strMailQuery.AppendLine("CM_MailContent where id= :mailtemplateid)))");
                        strMailQuery.AppendLine("begin");
                        strMailQuery.AppendLine("select distinct UserID from AM_Entity_Role_User where EntityID= :Entityid and UserID<> :Actorid and UserID in ");
                        strMailQuery.AppendLine("(select distinct UserID ");
                        strMailQuery.AppendLine("from   CM_UserDefaultSubscription a cross apply");
                        strMailQuery.AppendLine("dbo.tf_SplitString(a.MailSubscriptionTypeID,',') b  ");
                        strMailQuery.AppendLine("where  b.Value =(select MailSubscriptionTypeID from ");
                        strMailQuery.AppendLine("CM_MailContent where id= :mailtemplateid))");
                        strMailQuery.AppendLine("end");

                        strMailQuery.AppendLine("else if EXISTS(select UserID from PM_Task_Members where TaskID= :Entityid and UserID<>:Actorid and UserID in");
                        strMailQuery.AppendLine("(select distinct UserID ");
                        strMailQuery.AppendLine("from   CM_UserDefaultSubscription a cross apply");
                        strMailQuery.AppendLine("dbo.tf_SplitString(a.MailSubscriptionTypeID,',') b  ");
                        strMailQuery.AppendLine("where  b.Value =(select MailSubscriptionTypeID from ");
                        strMailQuery.AppendLine("CM_MailContent where id= :mailtemplateid)))");
                        strMailQuery.AppendLine("begin");
                        strMailQuery.AppendLine("select distinct UserID from PM_Task_Members where TaskID= :Entityid and UserID<> :Actorid and UserID in");
                        strMailQuery.AppendLine("(select distinct UserID ");
                        strMailQuery.AppendLine("from   CM_UserDefaultSubscription a cross apply");
                        strMailQuery.AppendLine("dbo.tf_SplitString(a.MailSubscriptionTypeID,',') b  ");
                        strMailQuery.AppendLine("where  b.Value =(select MailSubscriptionTypeID from ");
                        strMailQuery.AppendLine("CM_MailContent where id=:mailtemplateid))");
                        strMailQuery.AppendLine("end");
                        strMailQuery.AppendLine("end");
                        strMailQuery.AppendLine("else");
                        strMailQuery.AppendLine("begin");


                        strMailQuery.AppendLine("declare @mailsubscriptionType bit");
                        strMailQuery.AppendLine("select @mailsubscriptionType = mailSubscriptionTypeID from CM_MailContent where id= :mailtemplateid");
                        strMailQuery.AppendLine("if(@mailsubscriptionType!=0) ");
                        strMailQuery.AppendLine("begin ");
                        strMailQuery.AppendLine("if EXISTS(select  distinct UserID from AM_Entity_Role_User where EntityID= :Entityid)");
                        strMailQuery.AppendLine("begin");
                        strMailQuery.AppendLine("select distinct UserID from AM_Entity_Role_User where EntityID= :Entityid");
                        strMailQuery.AppendLine("end");
                        strMailQuery.AppendLine("else IF EXISTS (select UserID from TM_Task_Members where TaskID= :Entityid )");
                        strMailQuery.AppendLine("begin");
                        strMailQuery.AppendLine("select distinct UserID from TM_Task_Members where TaskID= :Entityid");
                        strMailQuery.AppendLine("end");
                        strMailQuery.AppendLine("else if EXISTS(select UserID from PM_Task_Members where TaskID= :Entityid)");
                        strMailQuery.AppendLine("begin");
                        strMailQuery.AppendLine("select distinct UserID from PM_Task_Members where TaskID= :Entityid");
                        strMailQuery.AppendLine("end");

                        strMailQuery.AppendLine("else");
                        strMailQuery.AppendLine("begin");
                        strMailQuery.AppendLine("select  :Userid ");
                        strMailQuery.AppendLine("end");
                        strMailQuery.AppendLine("end");
                        strMailQuery.AppendLine("end");

                    }
                    else
                    {


                        strMailQuery.AppendLine("if (@isMailMandatory=0) ");
                        strMailQuery.AppendLine("begin ");


                        strMailQuery.AppendLine("IF EXISTS (select UserID from TM_Task_Members where TaskID= :Entityid and UserID in");
                        strMailQuery.AppendLine("(select distinct UserID ");
                        strMailQuery.AppendLine("from   CM_UserDefaultSubscription a cross apply");
                        strMailQuery.AppendLine("dbo.tf_SplitString(a.MailSubscriptionTypeID,',') b  ");
                        strMailQuery.AppendLine("where  b.Value =(select MailSubscriptionTypeID from ");
                        strMailQuery.AppendLine("CM_MailContent where id= :mailtemplateid) and UserID = :Userid ))");
                        strMailQuery.AppendLine("begin");
                        strMailQuery.AppendLine("select distinct UserID from TM_Task_Members where TaskID= :Entityid and  UserID<> :Actorid and UserID in");
                        strMailQuery.AppendLine("(select distinct UserID ");
                        strMailQuery.AppendLine("from   CM_UserDefaultSubscription a cross apply");
                        strMailQuery.AppendLine("dbo.tf_SplitString(a.MailSubscriptionTypeID,',') b  ");
                        strMailQuery.AppendLine("where  b.Value =(select MailSubscriptionTypeID from ");
                        strMailQuery.AppendLine("CM_MailContent where id= :mailtemplateid) and UserID = :Userid )");
                        strMailQuery.AppendLine("end");

                        strMailQuery.AppendLine("else if EXISTS(select UserID from AM_Entity_Role_User where EntityID= :Entityid and UserID in");
                        strMailQuery.AppendLine("(select distinct UserID ");
                        strMailQuery.AppendLine("from   CM_UserDefaultSubscription a cross apply");
                        strMailQuery.AppendLine("dbo.tf_SplitString(a.MailSubscriptionTypeID,',') b  ");
                        strMailQuery.AppendLine("where  b.Value =(select MailSubscriptionTypeID from ");
                        strMailQuery.AppendLine("CM_MailContent where id= :mailtemplateid) and UserID = :Userid ))");
                        strMailQuery.AppendLine("begin");
                        strMailQuery.AppendLine("select distinct UserID from AM_Entity_Role_User where EntityID= :Entityid and UserID<> :Actorid and UserID in ");
                        strMailQuery.AppendLine("(select distinct UserID ");
                        strMailQuery.AppendLine("from   CM_UserDefaultSubscription a cross apply");
                        strMailQuery.AppendLine("dbo.tf_SplitString(a.MailSubscriptionTypeID,',') b  ");
                        strMailQuery.AppendLine("where  b.Value =(select MailSubscriptionTypeID from ");
                        strMailQuery.AppendLine("CM_MailContent where id= :mailtemplateid) and UserID = :Userid )");
                        strMailQuery.AppendLine("end");

                        strMailQuery.AppendLine("else if EXISTS(select UserID from PM_Task_Members where TaskID= :Entityid and UserID<> :Actorid and UserID in");
                        strMailQuery.AppendLine("(select distinct UserID ");
                        strMailQuery.AppendLine("from   CM_UserDefaultSubscription a cross apply");
                        strMailQuery.AppendLine("dbo.tf_SplitString(a.MailSubscriptionTypeID,',') b  ");
                        strMailQuery.AppendLine("where  b.Value =(select MailSubscriptionTypeID from ");
                        strMailQuery.AppendLine("CM_MailContent where id= :mailtemplateid) and UserID = :Userid ))");
                        strMailQuery.AppendLine("begin");
                        strMailQuery.AppendLine("select distinct UserID from PM_Task_Members where TaskID= :Entityid and UserID<> :Actorid  and UserID in");
                        strMailQuery.AppendLine("(select distinct UserID ");
                        strMailQuery.AppendLine("from   CM_UserDefaultSubscription a cross apply");
                        strMailQuery.AppendLine("dbo.tf_SplitString(a.MailSubscriptionTypeID,',') b  ");
                        strMailQuery.AppendLine("where  b.Value =(select MailSubscriptionTypeID from ");
                        strMailQuery.AppendLine("CM_MailContent where id= :mailtemplateid) and UserID = :Userid )");
                        strMailQuery.AppendLine("end");
                        strMailQuery.AppendLine("else if EXISTS");
                        strMailQuery.AppendLine("(select distinct UserID ");
                        strMailQuery.AppendLine("from   CM_UserDefaultSubscription a cross apply");
                        strMailQuery.AppendLine("dbo.tf_SplitString(a.MailSubscriptionTypeID,',') b  ");
                        strMailQuery.AppendLine("where  b.Value =(select MailSubscriptionTypeID from ");
                        strMailQuery.AppendLine("CM_MailContent where id=:mailtemplateid) and UserID = :Userid )");

                        strMailQuery.AppendLine("begin");

                        strMailQuery.AppendLine("select distinct UserID ");
                        strMailQuery.AppendLine("from   CM_UserDefaultSubscription a cross apply");
                        strMailQuery.AppendLine("dbo.tf_SplitString(a.MailSubscriptionTypeID,',') b  ");
                        strMailQuery.AppendLine("where  b.Value =(select MailSubscriptionTypeID from ");
                        strMailQuery.AppendLine("CM_MailContent where id= :mailtemplateid) and UserID = :Userid");

                        strMailQuery.AppendLine("end ");

                        strMailQuery.AppendLine("end ");
                        strMailQuery.AppendLine("else");
                        strMailQuery.AppendLine("begin");

                        strMailQuery.AppendLine("if EXISTS(select  distinct UserID from AM_Entity_Role_User where EntityID= :Entityid and UserID = :Userid)");
                        strMailQuery.AppendLine("begin");
                        strMailQuery.AppendLine("select distinct UserID from AM_Entity_Role_User where EntityID= :Entityid and UserID = :Userid");
                        strMailQuery.AppendLine("end");
                        strMailQuery.AppendLine("else IF EXISTS (select UserID from TM_Task_Members where TaskID= :Entityid and UserID =:Userid)");
                        strMailQuery.AppendLine("begin");
                        strMailQuery.AppendLine("select distinct UserID from TM_Task_Members where TaskID=:Entityid and UserID = :Userid");
                        strMailQuery.AppendLine("end");
                        strMailQuery.AppendLine("else if EXISTS(select UserID from PM_Task_Members where TaskID= :Entityid and UserID =:Userid )");
                        strMailQuery.AppendLine("begin");
                        strMailQuery.AppendLine("select distinct UserID from PM_Task_Members where TaskID= :Entityid and UserID = :Userid");
                        strMailQuery.AppendLine("end");

                        strMailQuery.AppendLine("else");
                        strMailQuery.AppendLine("begin");
                        strMailQuery.AppendLine("select  :Userid");
                        strMailQuery.AppendLine("end");
                        strMailQuery.AppendLine("end");
                    }


                    listOfUsersToSendMail = txmail.PersistenceManager.CommonRepository.ExecuteQuerywithParam(strMailQuery.ToString(), parLIST).Cast<Hashtable>();
                    IList<UserNotificationDao> listOfNotificationsForMail = new List<UserNotificationDao>();
                    if (listOfUsersToSendMail.Count() > 0)
                    {
                        foreach (var val in listOfUsersToSendMail)
                        {
                            if (Actorid != Convert.ToInt32(val["UserID"]))
                            {
                                UserNotificationDao obj = new UserNotificationDao();
                                obj.Entityid = Convert.ToInt32(Entityid);
                                obj.Actorid = Actorid;
                                obj.CreatedOn = CreatedOn;
                                obj.Typeid = Typeid; // notification templateId
                                obj.IsViewed = false;
                                obj.IsSentInMail = false;
                                obj.TypeName = Typename;
                                obj.AttributeName = Attributename;
                                obj.FromValue = Fromvalue;
                                obj.ToValue = Tovalue;
                                obj.Userid = Convert.ToInt32(val["UserID"]);
                                listOfNotificationsForMail.Add(obj);
                            }
                        }

                    }

                    if (listOfNotificationsForMail.Count() > 0)
                    {
                        mailStatus = Insert_Mail(proxy, mailtemplateid, listOfNotificationsForMail);
                        if (mailStatus)
                            txmail.Commit();
                        else
                            txmail.Rollback();
                    }
                    else
                        txmail.Commit();
                }
                catch (Exception ex)
                {
                    txmail.Rollback();
                }
                // tx.Commit();

                return true;
            }

        }

        public bool Insert_Mail(CommonManagerProxy proxy, int mailtemplateid, IList<UserNotificationDao> listofusernotification)
        {
            using (ITransaction tx = proxy.MarcomManager.GetTransaction())
            {
                try
                {
                    UserDao user = new UserDao();
                    UserDao user1 = new UserDao();
                    IList<MailDao> maillist = new List<MailDao>();


                    string xmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
                    XDocument adminXmlDoc = XDocument.Load(xmlpath);
                    //The Key is root node current Settings
                    string xelementName = "ApplicationURL";
                    var xelementFilepath = XElement.Load(xmlpath);
                    var xmlElement = xelementFilepath.Element(xelementName);
                    string logopath1 = "";
                    string ImagePath1 = "";


                    var mailbodydetails = (from tt in tx.PersistenceManager.CommonRepository.Query<MailContentDao>() where tt.Id == mailtemplateid select tt).FirstOrDefault();
                    foreach (var userDefSubscriptionRow in listofusernotification)
                    {

                        user = tx.PersistenceManager.CommonRepository.Get<UserDao>(userDefSubscriptionRow.Actorid);
                        user1 = tx.PersistenceManager.CommonRepository.Get<UserDao>(userDefSubscriptionRow.Userid);
                        bool isSsoUser = user1.IsSSOUser;
                        var entitydetails = tx.PersistenceManager.PlanningRepository.Get<EntityDao>(userDefSubscriptionRow.Entityid);
                        string template = mailbodydetails.Body.ToString();
                        StringBuilder sb = new StringBuilder(template.ToString());
                        foreach (Match match in Regex.Matches(template, @"@(.+?)@"))
                        {
                            switch (match.Value.Trim())
                            {
                                case "@AssingedBy@":
                                    {
                                        sb.Replace(match.Value, user.FirstName + ' ' + user.LastName);
                                        break;
                                    }
                                case "@AssingedByMailID@":
                                    {
                                        sb.Replace(match.Value, user.Email);
                                        break;
                                    }
                                case "@ImagePath@":
                                    {
                                        logopath1 = xmlElement.Value.ToString() + "/assets/img/logo.png";
                                        sb.Replace(match.Value, logopath1);
                                        break;
                                    }
                                case "@AssingedByImage@":
                                    {

                                        var UserImagePath = xmlElement.Value.ToString() + "/UserImages/" + user.Id + ".jpg";
                                        if (System.IO.File.Exists(Path.Combine(Path.Combine(HttpRuntime.AppDomainAppPath + "/UserImages/", user.Id + ".jpg"))) == false)
                                            UserImagePath = xmlElement.Value.ToString() + "/UserImages/noimage.jpg";
                                        sb.Replace(match.Value, UserImagePath);
                                        break;
                                    }
                                case "@Time@":
                                    {
                                        //not user here;
                                        break;
                                    }
                                case "@ActorName@":
                                    {
                                        sb.Replace(match.Value, user.FirstName + ' ' + user.LastName);
                                        break;
                                    }
                                case "@AttributeName@":
                                    {
                                        sb.Replace(match.Value, userDefSubscriptionRow.AttributeName);
                                        break;
                                    }
                                case "@OldValue@":
                                    {
                                        try
                                        {
                                            if (Convert.ToInt32(userDefSubscriptionRow.Typeid) == 22 || Convert.ToInt32(userDefSubscriptionRow.Typeid) == 36 || Convert.ToInt32(userDefSubscriptionRow.Typeid) == 37)
                                            {
                                                var s = GetCurrency(Convert.ToInt32(userDefSubscriptionRow.FromValue.ToString()));
                                                sb.Replace(match.Value, s.ToString() + " " + proxy.MarcomManager.GlobalAdditionalSettings[1].CurrencyFormatvalue[0].Symbol);
                                            }
                                            else
                                            {
                                                sb.Replace(match.Value, userDefSubscriptionRow.FromValue);
                                            }
                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                        break;
                                    }
                                case "@NewValue@":
                                    {
                                        try
                                        {
                                            if (Convert.ToInt32(userDefSubscriptionRow.Typeid) == 22 || Convert.ToInt32(userDefSubscriptionRow.Typeid) == 36 || Convert.ToInt32(userDefSubscriptionRow.Typeid) == 37)
                                            {
                                                var s = GetCurrency(Convert.ToInt32(userDefSubscriptionRow.ToValue.ToString()));
                                                sb.Replace(match.Value, s.ToString() + " " + proxy.MarcomManager.GlobalAdditionalSettings[1].CurrencyFormatvalue[0].Symbol);
                                            }
                                            else
                                            {
                                                sb.Replace(match.Value, userDefSubscriptionRow.ToValue);
                                            }
                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                        break;
                                    }
                                case "@Users@":
                                    {
                                        sb.Replace(match.Value, user1.FirstName + " " + user1.LastName);
                                        break;
                                    }
                                case "@Role@":
                                    {

                                        var currententityroleobj = tx.PersistenceManager.CommonRepository.Query<BaseEntityDao>().Where(a => a.Id == Convert.ToInt32(userDefSubscriptionRow.Entityid)).SingleOrDefault();
                                        var entitypeobj = tx.PersistenceManager.CommonRepository.Query<EntityTypeDao>().Where(a => a.Id == currententityroleobj.Typeid).SingleOrDefault();
                                        if (entitypeobj.IsAssociate)
                                        {
                                            var role = tx.PersistenceManager.CommonRepository.Get<RoleDao>(Convert.ToInt32(userDefSubscriptionRow.ToValue));
                                            sb.Replace(match.Value, role.Caption);
                                        }
                                        else
                                        {
                                            var role = tx.PersistenceManager.CommonRepository.Get<RoleDao>(Convert.ToInt32(userDefSubscriptionRow.ToValue));
                                            sb.Replace(match.Value, role.Caption);

                                        }
                                        break;
                                    }
                                case "@EntityTypeName@":
                                    {
                                        if (Convert.ToInt32(userDefSubscriptionRow.Typeid) == 40 || Convert.ToInt32(userDefSubscriptionRow.Typeid) == 41 || Convert.ToInt32(userDefSubscriptionRow.Typeid) == 42)
                                        {
                                            var taskdet = (from tt in tx.PersistenceManager.CommonRepository.Query<EntityDao>() where tt.Id == Convert.ToInt32(userDefSubscriptionRow.ToValue) select tt.Typeid).FirstOrDefault();
                                            var tasktypename = tx.PersistenceManager.CommonRepository.Get<EntityTypeDao>(Convert.ToInt32(taskdet));
                                            sb.Replace(match.Value, tasktypename.Caption);
                                        }
                                        else
                                        {
                                            var entitytypename = tx.PersistenceManager.CommonRepository.Get<EntityTypeDao>(Convert.ToInt32(entitydetails.Typeid));
                                            sb.Replace(match.Value, entitytypename.Caption);
                                        }
                                        break;
                                    }
                                case "@EntityTypeNamefortask@":
                                    {

                                        sb.Replace(match.Value, userDefSubscriptionRow.TypeName);
                                        break;
                                    }
                                case "@EntityName@":
                                    {

                                        if (Convert.ToInt32(userDefSubscriptionRow.Typeid) == 40 || Convert.ToInt32(userDefSubscriptionRow.Typeid) == 41 || Convert.ToInt32(userDefSubscriptionRow.Typeid) == 42)
                                        {
                                            var taskdet = (from tt in tx.PersistenceManager.CommonRepository.Query<EntityDao>() where tt.Id == Convert.ToInt32(userDefSubscriptionRow.ToValue) select tt).FirstOrDefault();
                                            sb.Replace(match.Value, "<a href=" + GetEntityPathforMail(xmlElement.Value.ToString(), taskdet.Id, taskdet.Typeid, user1.Id, isSsoUser, taskdet.Parentid) + ">" + taskdet.Name + "</a>");
                                        }
                                        else
                                        {
                                            if (userDefSubscriptionRow.Typeid == 20 || userDefSubscriptionRow.Typeid == 26)
                                            {
                                                var costcentredetails = (from tt in tx.PersistenceManager.CommonRepository.Query<EntityDao>() where tt.Id == entitydetails.Id select tt).FirstOrDefault();
                                                sb.Replace(match.Value, "<a href=" + GetEntityPathforMail(xmlElement.Value.ToString(), costcentredetails.Id, 7, user1.Id, isSsoUser, costcentredetails.Parentid) + ">" + userDefSubscriptionRow.AttributeName + "</a>");
                                            }
                                            else
                                            {
                                                sb.Replace(match.Value, "<a href=" + GetEntityPathforMail(xmlElement.Value.ToString(), entitydetails.Id, entitydetails.Typeid, user1.Id, isSsoUser, entitydetails.Parentid) + ">" + entitydetails.Name + "</a>");
                                            }
                                        }

                                        break;
                                    }
                                case "@fundingrequest@":
                                    {

                                        sb.Replace(match.Value, "<a href=" + GetEntityPathforMail(xmlElement.Value.ToString(), entitydetails.Id, entitydetails.Typeid, user1.Id, isSsoUser, entitydetails.Parentid) + ">" + entitydetails.Name + "</a>");

                                        break;
                                    }
                                case "@FundingRequestState@":
                                    {
                                        sb.Replace(match.Value, userDefSubscriptionRow.ToValue.ToString());
                                        break;
                                    }
                                case "@TransferAmount@":
                                    {
                                        sb.Replace(match.Value, Convert.ToString(userDefSubscriptionRow.AttributeName));
                                        break;
                                    }
                                case "@FromCC@":
                                    {
                                        sb.Replace(match.Value, Convert.ToString(userDefSubscriptionRow.FromValue));
                                        break;
                                    }
                                case "@ToCC@":
                                    {
                                        sb.Replace(match.Value, Convert.ToString(userDefSubscriptionRow.ToValue));
                                        break;
                                    }
                                case "@AttributeValue@":
                                    {

                                        if (userDefSubscriptionRow.Typeid == 20)
                                        {
                                            var formattedvalue = GetCurrency(Convert.ToInt32(userDefSubscriptionRow.ToValue));
                                            sb.Replace(match.Value, Convert.ToString(formattedvalue));
                                        }
                                        else
                                        {
                                            sb.Replace(match.Value, Convert.ToString(userDefSubscriptionRow.ToValue));
                                        }

                                        break;
                                    }
                                case "@Path@":
                                    {

                                        var entityDetail = (from item in tx.PersistenceManager.PlanningRepository.Query<EntityDao>() where item.Id == userDefSubscriptionRow.Entityid select item).FirstOrDefault();
                                        EntityDao entparentdetails = new EntityDao();
                                        if (entityDetail.Typeid <= 50)   //for tasks
                                        {
                                            entparentdetails = (from item in tx.PersistenceManager.PlanningRepository.Query<EntityDao>() where item.Id == entityDetail.Parentid select item).FirstOrDefault();
                                            sb.Replace(match.Value, "<a href=" + GetEntityPathforMail(xmlElement.Value.ToString(), entparentdetails.Id, entparentdetails.Typeid, user1.Id, isSsoUser, entparentdetails.Parentid) + ">" + entparentdetails.Name + "</a>");
                                        }
                                        else
                                            //sb.Replace(match.Value, "<a href=" + GetEntityPathforMail(xmlElement.Value.ToString(), entityDetail.Id, entityDetail.Typeid, user1.Id, isSsoUser, entityDetail.Parentid) + ">" + entityDetail.Name + "</a>");
                                            sb.Replace("in @Path@", "");
                                        break;
                                    }
                                case "@TaskStatus@":
                                    {
                                        sb.Replace(match.Value, Convert.ToString(userDefSubscriptionRow.ToValue));
                                        break;
                                    }

                                case "@checklistname@":
                                    {
                                        sb.Replace(match.Value, Convert.ToString(userDefSubscriptionRow.AttributeName));
                                        break;
                                    }
                                case "@taskTypeName@":
                                    {
                                        var taskname = (from name in tx.PersistenceManager.TaskRepository.Query<EntityTaskDao>() where name.ID == userDefSubscriptionRow.Entityid select name).FirstOrDefault();
                                        var tasktype = (from type in tx.PersistenceManager.TaskRepository.Query<EntityTypeDao>() where type.Id == taskname.TaskType select type).FirstOrDefault();
                                        sb.Replace(match.Value, tasktype.Caption);
                                        break;
                                    }
                                case "@taskType@":
                                    {
                                        var taskname = (from name in tx.PersistenceManager.TaskRepository.Query<EntityTaskDao>() where name.ID == userDefSubscriptionRow.Entityid select name).FirstOrDefault();
                                        var tasktype = (from type in tx.PersistenceManager.TaskRepository.Query<EntityTypeDao>() where type.Id == taskname.TaskType select type).FirstOrDefault();
                                        sb.Replace(match.Value, tasktype.Caption);
                                        break;
                                    }
                                case "@TaskName@":
                                    {

                                        var taskname = (from name in tx.PersistenceManager.TaskRepository.Query<EntityTaskDao>() where name.ID == userDefSubscriptionRow.Entityid select name).FirstOrDefault();
                                        if (taskname == null)
                                        {
                                            sb.Replace(match.Value, "<a href=" + GetEntityPathforMail(xmlElement.Value.ToString(), entitydetails.Id, entitydetails.Typeid, user1.Id, isSsoUser, entitydetails.Parentid) + ">" + entitydetails.Name + "</a>");
                                        }
                                        else
                                        {
                                            sb.Replace(match.Value, "<a href=" + GetEntityPathforMail(xmlElement.Value.ToString(), taskname.ID, taskname.TaskType, user1.Id, isSsoUser, taskname.EntityID) + ">" + taskname.Name + "</a>");
                                        }
                                        break;
                                    }
                                case "@taskName@":
                                    {

                                        var taskname = (from name in tx.PersistenceManager.TaskRepository.Query<EntityTaskDao>() where name.ID == userDefSubscriptionRow.Entityid select name).FirstOrDefault();
                                        if (taskname == null)
                                        {
                                            sb.Replace(match.Value, "<a href=" + GetEntityPathforMail(xmlElement.Value.ToString(), entitydetails.Id, entitydetails.Typeid, user1.Id, isSsoUser, entitydetails.Parentid) + ">" + entitydetails.Name + "</a>");
                                        }
                                        else
                                        {
                                            sb.Replace(match.Value, "<a href=" + GetEntityPathforMail(xmlElement.Value.ToString(), taskname.ID, taskname.TaskType, user1.Id, isSsoUser, taskname.EntityID) + ">" + taskname.Name + "</a>");
                                        }

                                        break;
                                    }
                                case "@checkliststatus@":
                                    {
                                        sb.Replace(match.Value, userDefSubscriptionRow.FromValue);
                                        break;
                                    }
                                case "@filename@":
                                    {
                                        var filedetails = (from tt in tx.PersistenceManager.CommonRepository.Query<FileDao>() where tt.Id == Convert.ToInt32(userDefSubscriptionRow.ToValue) select tt).FirstOrDefault();
                                        if (filedetails == null)
                                            sb.Replace(match.Value, Convert.ToString(userDefSubscriptionRow.AttributeName));
                                        else
                                            sb.Replace(match.Value, "<a target=\"_blank\" href=\"download.aspx?FileID=" + filedetails.Fileguid + "&amp;FileFriendlyName=" + Convert.ToString(filedetails.Name) + "&amp;Ext=" + filedetails.Extension + "\">" + Convert.ToString(filedetails.Name) + "</a>");
                                        break;
                                    }
                                case "@linkname@":
                                    {
                                        var linkdetails = (from tt in tx.PersistenceManager.CommonRepository.Query<LinksDao>() where tt.ID == Convert.ToInt32(userDefSubscriptionRow.ToValue) select tt).FirstOrDefault();
                                        if (linkdetails == null)
                                            sb.Replace(match.Value, Convert.ToString(userDefSubscriptionRow.AttributeName));
                                        else
                                            sb.Replace(match.Value, "<a href=\"javascript:void(0);\" data-name=\"" + linkdetails.URL + "\"  data-id=\"" + linkdetails.ID + "\" >" + Convert.ToString(linkdetails.Name) + "</a>");
                                        break;
                                    }
                                case "@comment@":
                                    {
                                        sb.Replace(match.Value, Convert.ToString(userDefSubscriptionRow.ToValue));
                                        break;
                                    }
                                case "@feedtext@":
                                    {
                                        IList<IFeedSelection> feedtextlist = new List<IFeedSelection>();
                                        feedtextlist = GettingFeedsByEntityID(proxy, userDefSubscriptionRow.Entityid.ToString(), 0, false, Convert.ToInt32(userDefSubscriptionRow.FromValue));
                                        sb.Replace(match.Value, feedtextlist[0].FeedText.ToString());
                                        break;
                                    }

                                case "@TaskDate@":
                                    {
                                        //taskContent.Replace(match.Value, Convert.ToDateTime(tasks.ItemArray[8].ToString()).ToString("yyyy-MM-dd"));
                                        break;
                                    }

                            }
                        }
                        var mailsubscriptiondetails = (from tt in tx.PersistenceManager.CommonRepository.Query<UserMailSubscriptionDao>() where tt.Userid == user1.Id select tt).FirstOrDefault();
                        IList<MultiProperty> parDefultLIST = new List<MultiProperty>();
                        parDefultLIST.Add(new MultiProperty { propertyName = "mailtemplateid", propertyValue = mailtemplateid });
                        StringBuilder strIsMailDefault = new StringBuilder();
                        strIsMailDefault.AppendLine("declare @isMailMandatory bit");
                        strIsMailDefault.AppendLine("select isMailMandatory AS MailMandatory  from CM_SubscriptionType where id in(select mailSubscriptionTypeID from CM_MailContent where id= :mailtemplateid)");

                        var iSMailDefault = tx.PersistenceManager.CommonRepository.ExecuteQuerywithParam(strIsMailDefault.ToString(), parDefultLIST).Cast<Hashtable>().ToList();


                        if (mailsubscriptiondetails != null)
                        {
                            if (mailsubscriptiondetails.IsEmailEnable == true || (bool)iSMailDefault[0]["MailMandatory"] == true)
                            {
                                MailDao mdao = new MailDao();

                                mdao.ToMail = user1.Email.ToString();
                                mdao.Subject = mailbodydetails.Subject;
                                mdao.Body = sb.ToString().Replace("@Time@", DateTime.UtcNow.ToString("yyyy/MM/dd hh:mm"));
                                if (mailsubscriptiondetails.RecapReport == true && mailsubscriptiondetails.DayName != "")
                                {
                                    char[] delimiters = new char[] { ';' };
                                    string[] parts = mailsubscriptiondetails.DayName.ToString().Split(delimiters,
                                                     StringSplitOptions.RemoveEmptyEntries);
                                    if (parts == null)
                                    {
                                        parts = new string[0];
                                        parts[0] = "Daily";
                                    }
                                    DateTimeOffset dt = new DateTimeOffset();
                                    dt = DateTime.UtcNow;
                                    int nowdayofweek = Convert.ToInt32(DateTime.UtcNow.DayOfWeek);


                                    DateTimeOffset recapeMailDate = new DateTimeOffset();
                                    TimeSpan userTimeZoneSpan = new TimeSpan(Convert.ToInt32(user1.TimeZone.Split(':')[0]), Convert.ToInt32(user1.TimeZone.Split(':')[1]), 0);
                                    DateTime userCurrentDateTimeStamp = DateTime.UtcNow.Add(userTimeZoneSpan);

                                    var subscribedhours = mailsubscriptiondetails.Timing.Hours;
                                    var subscribedminutes = mailsubscriptiondetails.Timing.Minutes;
                                    TimeSpan userSubscribedTime = new TimeSpan(subscribedhours, subscribedminutes, 0);

                                    if (parts.Contains("Daily"))
                                    {
                                        if (userCurrentDateTimeStamp.TimeOfDay > userSubscribedTime)
                                            recapeMailDate = (new DateTimeOffset(DateTime.UtcNow.Date)).AddDays(1).Add(userSubscribedTime - userTimeZoneSpan);
                                        else
                                            recapeMailDate = (new DateTimeOffset(DateTime.UtcNow.Date)).Add(userSubscribedTime - userTimeZoneSpan);
                                    }
                                    else
                                    {
                                        Dictionary<DayOfWeek, int> subscribedWeekDays = new Dictionary<DayOfWeek, int>();
                                        foreach (var item in parts)
                                        {
                                            switch (item)
                                            {
                                                case "Sundays":
                                                    subscribedWeekDays.Add(DayOfWeek.Sunday, (int)DayOfWeek.Sunday + 1);
                                                    break;
                                                case "Mondays":
                                                    subscribedWeekDays.Add(DayOfWeek.Monday, (int)DayOfWeek.Monday + 1);
                                                    break;
                                                case "Tuesdays":
                                                    subscribedWeekDays.Add(DayOfWeek.Tuesday, (int)DayOfWeek.Tuesday + 1);
                                                    break;
                                                case "Wednesdays":
                                                    subscribedWeekDays.Add(DayOfWeek.Wednesday, (int)DayOfWeek.Wednesday + 1);
                                                    break;
                                                case "Thursdays":
                                                    subscribedWeekDays.Add(DayOfWeek.Thursday, (int)DayOfWeek.Thursday + 1);
                                                    break;
                                                case "Fridays":
                                                    subscribedWeekDays.Add(DayOfWeek.Friday, (int)DayOfWeek.Friday + 1);
                                                    break;
                                                case "Saturdays":
                                                    subscribedWeekDays.Add(DayOfWeek.Saturday, (int)DayOfWeek.Saturday + 1);
                                                    break;
                                            }
                                        }




                                        bool IsRecapeDateTimeCalculated = false;
                                        var daysWithSortOrder = (from day in subscribedWeekDays orderby day.Value ascending select day).ToList();
                                        for (int i = 0; i < daysWithSortOrder.Count(); i++)
                                        {
                                            if (daysWithSortOrder[i].Value == ((int)userCurrentDateTimeStamp.DayOfWeek + 1))
                                            {
                                                //Set Recape time to today if Subscribed time is not passed yet
                                                if (userCurrentDateTimeStamp.TimeOfDay < userSubscribedTime)
                                                {
                                                    recapeMailDate = (new DateTimeOffset(DateTime.UtcNow.Date)).Add(userSubscribedTime - userTimeZoneSpan);
                                                    IsRecapeDateTimeCalculated = true;
                                                }
                                                else
                                                {
                                                    //Set Recape time to same day of today on next week if no other days are subscribed
                                                    if (daysWithSortOrder.Count() == 1)
                                                    {
                                                        recapeMailDate = (new DateTimeOffset(DateTime.UtcNow.Date)).AddDays(7).Add(userSubscribedTime - userTimeZoneSpan);
                                                        IsRecapeDateTimeCalculated = true;
                                                    }
                                                }
                                            }
                                            // set Recape time in one of comming day of this week if available 
                                            else if (daysWithSortOrder[i].Value > ((int)userCurrentDateTimeStamp.DayOfWeek + 1))
                                            {
                                                int daysToAdd = daysWithSortOrder[i].Key - userCurrentDateTimeStamp.DayOfWeek;
                                                recapeMailDate = (new DateTimeOffset(DateTime.UtcNow.Date)).AddDays(daysToAdd).Add(userSubscribedTime - userTimeZoneSpan);
                                                IsRecapeDateTimeCalculated = true;
                                            }
                                            if (IsRecapeDateTimeCalculated) break;
                                        }
                                        //set Recape time if we had to set on one of next week day
                                        if (!IsRecapeDateTimeCalculated)
                                        {
                                            int daysToAdd = (7 - ((int)userCurrentDateTimeStamp.DayOfWeek + 1)) + (int)daysWithSortOrder[0].Value;
                                            recapeMailDate = (new DateTimeOffset(DateTime.UtcNow.Date)).AddDays(daysToAdd).Add(userSubscribedTime - userTimeZoneSpan);
                                            IsRecapeDateTimeCalculated = true;
                                        }
                                    }
                                    mdao.ActualTime = DateTime.Parse(DateTime.UtcNow.ToString("yyyy-MM-dd hh:mm:ss.fff tt"));
                                    mdao.RecapTime = recapeMailDate; //need to add recap time here
                                    mdao.isrecapmail = true;
                                    mdao.IsRecapSent = false;
                                }
                                else
                                {
                                    mdao.ActualTime = DateTime.Parse(DateTime.UtcNow.ToString("yyyy-MM-dd hh:mm:ss.fff tt"));
                                    mdao.isrecapmail = false;
                                }

                                mdao.NoOfTrial = 0;
                                mdao.IsRecapSent = false;
                                mdao.Status = "Pending";
                                mdao.mailtype = mailtemplateid;
                                maillist.Add(mdao);
                            }
                        }
                    }
                    tx.PersistenceManager.CommonRepository.Save<MailDao>(maillist);
                    tx.Commit();

                    return true;

                }
                catch (Exception ex)
                {
                    tx.Rollback();
                    return false;
                }
            }
        }

        public bool InsertMultiAssignedTaskMail(CommonManagerProxy proxy, int mailTemplateid, int actorId, List<int> multiTasks, IList<TaskMembersDao> taskMembers)
        {
            try
            {
                UserDao Actor = new UserDao();
                UserDao user1 = new UserDao();
                IList<MailDao> maillist = new List<MailDao>();


                string xmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
                XDocument adminXmlDoc = XDocument.Load(xmlpath);
                //The Key is root node current Settings
                string xelementName = "ApplicationURL";
                var xelementFilepath = XElement.Load(xmlpath);
                var xmlElement = xelementFilepath.Element(xelementName);
                string logopath1 = "";
                string ImagePath1 = "";

                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    var mailTemplate = (from tt in tx.PersistenceManager.CommonRepository.Query<MailContentDao>() where tt.Id == 59 || tt.Id == 58 select tt).ToList<MailContentDao>();
                    string multiassignedInnerContent = mailTemplate[0].Body.ToString();
                    string multiassignedCountHeader = mailTemplate[1].Body.ToString();
                    StringBuilder sb = new StringBuilder();
                    var actorDetails = (from item in tx.PersistenceManager.TaskRepository.Query<UserDao>() where item.Id == (int)actorId select item).FirstOrDefault();
                    var Memberslist = (
                            from x in taskMembers
                            select new { UserID = x.UserID }
                            ).Distinct().ToList();
                    foreach (var member in Memberslist)
                    {
                        if (member.UserID != actorId)
                        {
                            sb.Length = 0;
                            var memberDetails = (from item in tx.PersistenceManager.TaskRepository.Query<UserDao>() where item.Id == (int)member.UserID select item).FirstOrDefault();
                            TimeSpan userTimeZoneForReminder = new TimeSpan(Convert.ToInt32(memberDetails.TimeZone.Split(':')[0]), Convert.ToInt32((memberDetails.TimeZone.Split(':'))[1]), 0);

                            foreach (var task in multiTasks)
                            {
                                //string dateformate = tx.MarcomManager.GlobalAdditionalSettings[0].SettingValue.ToString().Replace('m', 'M');
                                var taskDetails = (from item in tx.PersistenceManager.TaskRepository.Query<EntityTaskDao>() where item.ID == (int)task select item).FirstOrDefault();
                                sb.Append(multiassignedInnerContent.ToString());
                                foreach (Match match in Regex.Matches(multiassignedInnerContent, @"@(.+?)@"))
                                {
                                    switch (match.Value.Trim())
                                    {
                                        //   to be included in user loop
                                        case "@AssingedBy@":
                                            {
                                                sb.Replace(match.Value, Actor.FirstName + ' ' + Actor.LastName);
                                                break;
                                            }
                                        case "@AssingedByMailID@":
                                            {
                                                sb.Replace(match.Value, Actor.Email);
                                                break;
                                            }
                                        case "@ImagePath@":
                                            {
                                                logopath1 = xmlElement.Value.ToString() + "/assets/img/logo.png";
                                                sb.Replace(match.Value, logopath1);
                                                break;
                                            }
                                        case "@AssingedByImage@":
                                            {
                                                if (System.IO.File.Exists((Path.Combine(xmlElement.Value.ToString() + "/UserImages/", actorId + ".jpg"))))
                                                    ImagePath1 = xmlElement.Value.ToString() + "/UserImages/" + actorId + ".jpg";
                                                else
                                                    ImagePath1 = xmlElement.Value.ToString() + "/UserImages/noimage.jpg";

                                                sb.Replace(match.Value, ImagePath1);
                                                break;
                                            }
                                        case "@ActorName@":
                                            {
                                                sb.Replace(match.Value, actorDetails.FirstName + ' ' + actorDetails.LastName);
                                                break;
                                            }
                                        case "@TaskType@":
                                            {
                                                var tasktype = (from type in tx.PersistenceManager.TaskRepository.Query<EntityTypeDao>() where type.Id == taskDetails.TaskType select type).FirstOrDefault();
                                                sb.Replace(match.Value, tasktype.Caption);
                                                break;
                                            }
                                        case "@TaskName@":
                                            {
                                                sb.Replace(match.Value, "<a href=" + GetEntityPathforMail(xmlElement.Value.ToString(), (int)task, taskDetails.TaskType, member.UserID, memberDetails.IsSSOUser, taskDetails.EntityID) + ">" + taskDetails.Name + "</a>");
                                                break;
                                            }
                                        case "@DueIn@":
                                            {
                                                if (taskDetails.DueDate.ToString() != "")
                                                {
                                                    TimeSpan differenceforremindertasks = Convert.ToDateTime(taskDetails.DueDate.ToString()).Add(userTimeZoneForReminder) - DateTime.UtcNow.Add(userTimeZoneForReminder);
                                                    if (differenceforremindertasks.Days.ToString() == "0")
                                                        sb.Replace(match.Value, "1");
                                                    else
                                                        sb.Replace(match.Value, differenceforremindertasks.Days.ToString());
                                                }
                                                else
                                                    sb.Replace(match.Value, "");

                                                break;
                                            }
                                        case "@TaskDate@":
                                            {
                                                if (taskDetails.DueDate.ToString() != "")
                                                {
                                                    sb.Replace(match.Value, Convert.ToDateTime(taskDetails.DueDate.ToString()).ToString("yyyy-MM-dd"));
                                                }
                                                else
                                                    sb.Replace(match.Value, "");

                                                break;
                                            }
                                        case "@Path@":
                                            {
                                                var entityDetail = (from item in tx.PersistenceManager.PlanningRepository.Query<EntityDao>() where item.Id == taskDetails.EntityID select item).FirstOrDefault();
                                                sb.Replace(match.Value, "<a href=" + GetEntityPathforMail(xmlElement.Value.ToString(), entityDetail.Id, entityDetail.Typeid, member.UserID, memberDetails.IsSSOUser, entityDetail.Parentid) + ">" + entityDetail.Name + "</a>");
                                                break;
                                            }
                                    }
                                }
                            }

                            var MemberSubscriptionDetails = (from tt in tx.PersistenceManager.CommonRepository.Query<UserMailSubscriptionDao>() where tt.Userid == member.UserID select tt).FirstOrDefault();
                            var memberDetailsToSend = (from item in tx.PersistenceManager.TaskRepository.Query<UserDao>() where item.Id == (int)member.UserID select item).FirstOrDefault();

                            if (MemberSubscriptionDetails.IsEmailEnable == true)
                            {
                                MailDao mdao = new MailDao();

                                mdao.ToMail = memberDetailsToSend.Email.ToString();
                                mdao.Subject = mailTemplate[0].Subject;
                                mdao.Body = sb.ToString().Replace("@Time@", DateTime.UtcNow.ToString("yyyy/MM/dd hh:mm"));
                                if (MemberSubscriptionDetails.RecapReport == true)
                                {
                                    char[] delimiters = new char[] { ';' };
                                    string[] subscribedDays = MemberSubscriptionDetails.DayName.ToString().Split(delimiters,
                                                     StringSplitOptions.RemoveEmptyEntries);
                                    DateTimeOffset dt = new DateTimeOffset();
                                    dt = DateTime.UtcNow;
                                    int nowdayofweek = Convert.ToInt32(DateTime.UtcNow.DayOfWeek);
                                    DateTimeOffset memberRecapeMailDate = new DateTimeOffset();
                                    TimeSpan memberTimeZoneSpan = new TimeSpan(Convert.ToInt32(memberDetailsToSend.TimeZone.Split(':')[0]), Convert.ToInt32(memberDetailsToSend.TimeZone.Split(':')[1]), 0);
                                    DateTime memberCurrentDateTimeStamp = DateTime.UtcNow.Add(memberTimeZoneSpan);

                                    var memberSubscribedHours = MemberSubscriptionDetails.Timing.Hours;
                                    var memberSubscribedMinutes = MemberSubscriptionDetails.Timing.Minutes;
                                    TimeSpan userSubscribedTime = new TimeSpan(memberSubscribedHours, memberSubscribedMinutes, 0);

                                    if (subscribedDays.Contains("Daily"))
                                    {
                                        if (memberCurrentDateTimeStamp.TimeOfDay > userSubscribedTime)
                                            memberRecapeMailDate = (new DateTimeOffset(DateTime.UtcNow.Date)).AddDays(1).Add(userSubscribedTime - memberTimeZoneSpan);
                                        else
                                            memberRecapeMailDate = (new DateTimeOffset(DateTime.UtcNow.Date)).Add(userSubscribedTime - memberTimeZoneSpan);
                                    }
                                    else
                                    {
                                        Dictionary<DayOfWeek, int> subscribedWeekDays = new Dictionary<DayOfWeek, int>();
                                        foreach (var item in subscribedDays)
                                        {
                                            switch (item)
                                            {
                                                case "Sundays":
                                                    subscribedWeekDays.Add(DayOfWeek.Sunday, (int)DayOfWeek.Sunday + 1);
                                                    break;
                                                case "Mondays":
                                                    subscribedWeekDays.Add(DayOfWeek.Monday, (int)DayOfWeek.Monday + 1);
                                                    break;
                                                case "Tuesdays":
                                                    subscribedWeekDays.Add(DayOfWeek.Tuesday, (int)DayOfWeek.Tuesday + 1);
                                                    break;
                                                case "Wednesdays":
                                                    subscribedWeekDays.Add(DayOfWeek.Wednesday, (int)DayOfWeek.Wednesday + 1);
                                                    break;
                                                case "Thursdays":
                                                    subscribedWeekDays.Add(DayOfWeek.Thursday, (int)DayOfWeek.Thursday + 1);
                                                    break;
                                                case "Fridays":
                                                    subscribedWeekDays.Add(DayOfWeek.Friday, (int)DayOfWeek.Friday + 1);
                                                    break;
                                                case "Saturdays":
                                                    subscribedWeekDays.Add(DayOfWeek.Saturday, (int)DayOfWeek.Saturday + 1);
                                                    break;
                                            }
                                        }

                                        bool IsRecapeDateTimeCalculated = false;
                                        var daysWithSortOrder = (from day in subscribedWeekDays orderby day.Value ascending select day).ToList();
                                        for (int i = 0; i < daysWithSortOrder.Count(); i++)
                                        {
                                            if (daysWithSortOrder[i].Value == ((int)memberCurrentDateTimeStamp.DayOfWeek + 1))
                                            {
                                                //Set Recape time to today if Subscribed time is not passed yet
                                                if (memberCurrentDateTimeStamp.TimeOfDay < userSubscribedTime)
                                                {
                                                    memberRecapeMailDate = (new DateTimeOffset(DateTime.UtcNow.Date)).Add(userSubscribedTime - memberTimeZoneSpan);
                                                    IsRecapeDateTimeCalculated = true;
                                                }
                                                else
                                                {
                                                    //Set Recape time to same day of today on next week if no other days are subscribed
                                                    if (daysWithSortOrder.Count() == 1)
                                                    {
                                                        memberRecapeMailDate = (new DateTimeOffset(DateTime.UtcNow.Date)).AddDays(7).Add(userSubscribedTime - memberTimeZoneSpan);
                                                        IsRecapeDateTimeCalculated = true;
                                                    }
                                                }
                                            }
                                            // set Recape time in one of comming day of this week if available 
                                            else if (daysWithSortOrder[i].Value > ((int)memberCurrentDateTimeStamp.DayOfWeek + 1))
                                            {
                                                int daysToAdd = daysWithSortOrder[i].Key - memberCurrentDateTimeStamp.DayOfWeek;
                                                memberRecapeMailDate = (new DateTimeOffset(DateTime.UtcNow.Date)).AddDays(daysToAdd).Add(userSubscribedTime - memberTimeZoneSpan);
                                                IsRecapeDateTimeCalculated = true;
                                            }
                                            if (IsRecapeDateTimeCalculated) break;
                                        }
                                        //set Recape time if we had to set on one of next week day
                                        if (!IsRecapeDateTimeCalculated)
                                        {
                                            int daysToAdd = (7 - ((int)memberCurrentDateTimeStamp.DayOfWeek + 1)) + (int)daysWithSortOrder[0].Value;
                                            memberRecapeMailDate = (new DateTimeOffset(DateTime.UtcNow.Date)).AddDays(daysToAdd).Add(userSubscribedTime - memberTimeZoneSpan);
                                            IsRecapeDateTimeCalculated = true;
                                        }
                                    }

                                    mdao.ActualTime = DateTime.Parse(DateTime.UtcNow.ToString("yyyy-MM-dd hh:mm:ss.fff tt"));
                                    mdao.RecapTime = memberRecapeMailDate;
                                    mdao.isrecapmail = true;
                                    mdao.IsRecapSent = false;
                                }
                                else
                                {
                                    mdao.ActualTime = DateTime.Parse(DateTime.UtcNow.ToString("yyyy-MM-dd hh:mm:ss.fff tt"));
                                    mdao.isrecapmail = false;
                                    mdao.IsRecapSent = false;
                                }

                                mdao.NoOfTrial = 0;
                                mdao.Status = "Pending";
                                mdao.mailtype = mailTemplateid;
                                maillist.Add(mdao);
                                tx.PersistenceManager.CommonRepository.Save<MailDao>(maillist);
                                tx.Commit();
                            }
                        }

                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public string GetEntityPathforMail(string ApplicationPath, int EntityID, int typeID, int UserId, bool isSsoUser, int parentID)
        {
            string ApplicationEntityPath = "";

            string SSOUserQueryString = "";
            try
            {
                if (isSsoUser)
                {
                    SSOUserQueryString = "&IsSSO=true";
                }

                if (typeID == 7)
                {
                    ApplicationEntityPath = "#/mui/planningtool/detail/section/" + parentID + "/financial/" + EntityID;
                }
                else if (typeID == 2 || typeID == 3 || typeID == 30 || typeID == 11 || typeID == 31)
                {
                    //Pass here Parent Path these are all asssociate
                    ApplicationEntityPath = "#/mui/planningtool/detail/section/" + parentID + "/task/" + EntityID;
                }
                else if (typeID == 5)
                {
                    ApplicationEntityPath = "#/mui/planningtool/costcentre/detail/section/" + EntityID;
                }
                else if (typeID == 10)
                {
                    ApplicationEntityPath = "#/mui/planningtool/objective/detail/section/" + EntityID;
                }

                else
                {
                    ApplicationEntityPath = "#/mui/planningtool/detail/section/" + EntityID;
                }
                ApplicationEntityPath = ApplicationPath + "/login.html?ReturnURL=" + System.Web.HttpUtility.UrlEncode(ApplicationEntityPath) + SSOUserQueryString;
            }
            catch (Exception ex)
            {

            }
            return ApplicationEntityPath;
        }
        /// <summary>
        /// Get Navigation Config.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <returns>String</returns>
        public string GetNavigationConfig(CommonManagerProxy proxy)
        {
            try
            {
                StringBuilder strtemplate = new StringBuilder();

                //strtemplate.Append("var app = angular.module('app', ['ngResource', 'ngCookies', 'ngGrid']);");

                strtemplate.Append("app.config(");
                strtemplate.Append("function ($routeProvider) {");


                string strActivityUrl = "mui/planningtool";
                string strCostCenterUrl = "mui/planningtool/costcentre";
                string strObjective = "mui/planningtool/objective";
                string strCalender = "mui/planningtool/calender";
                string strDashBoard = "mui/dashboard";
                string strmypage = "mui/mypage";
                string stradminsettings = "mui/admin/navigation";
                string strmetadatasettings = "mui/metadatasettings";
                string strtask = "mui/mytask";
                string strfundRequest = "mui/myfundingrequest";
                string strworkspaces = "mui/planningtool/workspaces";
                string strMediaBank = "mui/mediabank";
                string strfullpagecms = "fullpagecms";
                string strcmswithnavigation = "mui/cms";

                //Prabhu: We need to have a new page under my page called My Notification; Need to be done as a feature
                string strnotification = "mui/mynotification";


                //string strDashBoardwidget = "mui/admin/dashboardwidget";
                string strsupport = "mui/support";
                string defaultRoute = string.Empty;

                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    ///to get value from database
                    //  var usernavdetails =(from tt in tx.PersistenceManager.AccessRepository.Query<UserDao>() where tt.Id==proxy.MarcomManager.User.Id select tt).FirstOrDefault();

                    var topnavigationfeatures = proxy.MarcomManager.MetadataManager.GetFeaturesForTopNavigation();

                    IList<NavigationDao> navList = new List<NavigationDao>();
                    navList = tx.PersistenceManager.CommonRepository.Query<NavigationDao>().ToList();

                    var applicableNavigation = (from access in proxy.MarcomManager.User.ListOfUserGlobalRoles
                                                join navs in navList
                                                    on access.Moduleid equals navs.Moduleid
                                                where access.Featureid == navs.Featureid
                                                select navs).OrderBy(p => p.Id).ToList();

                    var supNav = (from nav in navList where nav.Featureid == 23 select nav).FirstOrDefault();

                    if (supNav != null)
                    {

                        NavigationDao dao = new NavigationDao();
                        dao.AddLanguageCode = supNav.AddLanguageCode;
                        dao.AddUserEmail = supNav.AddUserEmail;
                        dao.AddUserID = supNav.AddUserID;
                        dao.AddUserName = supNav.AddUserName;
                        dao.Caption = supNav.Caption;

                        dao.Description = supNav.Description;
                        dao.ExternalUrl = supNav.ExternalUrl;
                        dao.Featureid = supNav.Featureid;
                        dao.Id = supNav.Id;
                        applicableNavigation.Add(supNav);
                    }

                    tx.Commit();
                    if (applicableNavigation != null && applicableNavigation.Count() > 0)
                    {
                        ///replacing routes from database 
                        foreach (var nav in applicableNavigation)
                        {

                            switch ((FeatureID)nav.Featureid)
                            {
                                ///activity routes replacing
                                case FeatureID.Plan:
                                    strtemplate.Append("$routeProvider.when('/" + nav.Url + "', { action: 'mui.planningtool.default.list' });");

                                    ///Creating url tempalate for navigation
                                    //ActvityList related block
                                    strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/list', { action: 'mui.planningtool.default.list' });");
                                    strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/RootEntityTypeCreation', { action: 'mui.planningtool.RootEntityTypeCreation' });");
                                    strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/SubEntityCreation', { action: 'mui.planningtool.SubEntityCreation' });");
                                    strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/detail', { action: 'mui.planningtool.default.detail.ganttview' });");
                                    strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/detail/ganttview', { action: 'mui.planningtool.default.detail.ganttview' });");
                                    strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/detail/ganttview/:zoomlevel', { action: 'mui.planningtool.default.detail.ganttview' });");

                                    strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/detail/ganttview/:zoomlevel/:TrackID', { action: 'mui.planningtool.default.detail.ganttview' });");
                                    strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/detail/ganttview/plan/:TrackID', { action: 'mui.planningtool.default.detail.ganttview' });");
                                    strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/detail/listview/:TrackID', { action: 'mui.planningtool.default.detail.listview' });");
                                    //strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/detail/section/:ID/overview/:TrackID', { action: 'mui.planningtool.default.tests.tests2.overview' });");
                                    //strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/detail/section/:ID/:TrackID', { action: 'mui.planningtool.default.detail.section' });");

                                    strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/detail/listview', { action: 'mui.planningtool.default.detail.listview' });");
                                    strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/detail/section/:ID', { action: 'mui.planningtool.default.detail.section' });");
                                    strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/detail/section/:ID/overview', { action: 'mui.planningtool.default.detail.section.overview' });");

                                    //strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/tests/tests2/:ID/overview', { action: 'mui.planningtool.default.tests.tests2.overview' });");
                                    //strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/tests/section/:ID/overview', { action: 'mui.planningtool.default.tests.section.overview' });");
                                    //strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/detail/tests2/:ID/overview', { action: 'mui.planningtool.default.detail.tests2.overview' });");


                                    strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/detail/section/:ID/financial', { action: 'mui.planningtool.default.detail.section.financial' });");
                                    strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/detail/section/:ID/financial/:FundinRequestID', { action: 'mui.planningtool.default.detail.section.financial' });");
                                    strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/detail/section/:ID/workflow', { action: 'mui.planningtool.default.detail.section.workflow' });");
                                    strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/detail/section/:ID/member', { action: 'mui.planningtool.default.detail.section.member' });");
                                    strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/detail/section/:ID/objective', { action: 'mui.planningtool.default.detail.section.objective' });");
                                    strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/detail/section/:ID/attachment', { action: 'mui.planningtool.default.detail.section.attachment' });");
                                    strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/detail/section/:ID/attachment/:AssetID', { action: 'mui.planningtool.default.detail.section.attachment' });");
                                    strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/detail/section/:ID/presentation', { action: 'mui.planningtool.default.detail.section.presentation' });");
                                    strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/detail/section/:ID/task', { action: 'mui.planningtool.default.detail.section.task' });");
                                    strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/detail/section/:ID/task/:TaskID', { action: 'mui.planningtool.default.detail.section.task' });");
                                    strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/filtersettings', { action: 'mui.planningtool.filtersettings' });");
                                    strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/detail/detailfilter', { action: 'mui.planningtool.default.detail.detailfilter' });");



                                    //strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/detail/detailfilter', { action: 'mui.dam' });");
                                    break;
                                ///Cost center routes replacing
                                case FeatureID.CostCenter:
                                    strtemplate.Append("$routeProvider.when('/" + nav.Url + "', { action: 'mui.planningtool.costcentre.list' });");

                                    //Costcenter related block
                                    strtemplate.Append("$routeProvider.when('/" + strCostCenterUrl + "/list', { action: 'mui.planningtool.costcentre.list' });");
                                    strtemplate.Append("$routeProvider.when('/" + strCostCenterUrl + "/detail', { action: 'mui.planningtool.costcentre.detail.ganttview' });");
                                    strtemplate.Append("$routeProvider.when('/" + strCostCenterUrl + "/detail/ganttview', { action: 'mui.planningtool.costcentre.detail.ganttview' });");
                                    strtemplate.Append("$routeProvider.when('/" + strCostCenterUrl + "/detail/ganttview/:zoomlevel', { action: 'mui.planningtool.costcentre.detail.ganttview' });");
                                    strtemplate.Append("$routeProvider.when('/" + strCostCenterUrl + "/detail/listview', { action: 'mui.planningtool.costcentre.detail.listview' });");
                                    strtemplate.Append("$routeProvider.when('/" + strCostCenterUrl + "/detail/section/:ID', { action: 'mui.planningtool.costcentre.detail.section' });");
                                    strtemplate.Append("$routeProvider.when('/" + strCostCenterUrl + "/detail/section/:ID/overview', { action: 'mui.planningtool.costcentre.detail.section.overview' });");
                                    strtemplate.Append("$routeProvider.when('/" + strCostCenterUrl + "/detail/section/:ID/financial', { action: 'mui.planningtool.costcentre.detail.section.financial' });");
                                    strtemplate.Append("$routeProvider.when('/" + strCostCenterUrl + "/detail/section/:ID/workflow', { action: 'mui.planningtool.costcentre.detail.section.workflow' });");
                                    strtemplate.Append("$routeProvider.when('/" + strCostCenterUrl + "/detail/section/:ID/member', { action: 'mui.planningtool.costcentre.detail.section.member' });");
                                    strtemplate.Append("$routeProvider.when('/" + strCostCenterUrl + "/detail/section/:ID/attachment', { action: 'mui.planningtool.costcentre.detail.section.attachment' });");
                                    strtemplate.Append("$routeProvider.when('/" + strCostCenterUrl + "/detail/section/:ID/presentation', { action: 'mui.planningtool.costcentre.detail.section.presentation' });");
                                    strtemplate.Append("$routeProvider.when('/" + strCostCenterUrl + "/detail/section/:ID/task', { action: 'mui.planningtool.costcentre.detail.section.task' });");
                                    strtemplate.Append("$routeProvider.when('/" + strCostCenterUrl + "/detail/section/:ID/task/:TaskID', { action: 'mui.planningtool.costcentre.detail.section.task' });");
                                    strtemplate.Append("$routeProvider.when('/" + strCostCenterUrl + "/costcentrecreation', { action: 'mui.planningtool.costcentre.costcentrecreation' });");
                                    strtemplate.Append("$routeProvider.when('/" + strCostCenterUrl + "/costcentrecreation', { action: 'mui.planningtool.costcentre.FundrequestAction' });");
                                    strtemplate.Append("$routeProvider.when('/" + strCostCenterUrl + "/detail/detailfilter', { action: 'mui.planningtool.default.detail.detailfilter' });");

                                    strtemplate.Append("$routeProvider.when('/" + strCostCenterUrl + "/detail/ganttview/:zoomlevel/:TrackID', { action: 'mui.planningtool.costcentre.detail.ganttview' });");
                                    strtemplate.Append("$routeProvider.when('/" + strCostCenterUrl + "/detail/ganttview/:TrackID', { action: 'mui.planningtool.costcentre.detail.ganttview' });");
                                    strtemplate.Append("$routeProvider.when('/" + strCostCenterUrl + "/detail/listview/:TrackID', { action: 'mui.planningtool.costcentre.detail.listview' });");


                                    strtemplate.Append("$routeProvider.when('/" + strCostCenterUrl + "/detail/sectionentity/:ID', { action: 'mui.planningtool.costcentre.detail.sectionentity' });");
                                    strtemplate.Append("$routeProvider.when('/" + strCostCenterUrl + "/detail/sectionentity/:ID/overview', { action: 'mui.planningtool.costcentre.detail.sectionentity.overview' });");
                                    strtemplate.Append("$routeProvider.when('/" + strCostCenterUrl + "/detail/sectionentity/:ID/financial', { action: 'mui.planningtool.costcentre.detail.sectionentity.financial' });");
                                    strtemplate.Append("$routeProvider.when('/" + strCostCenterUrl + "/detail/sectionentity/:ID/workflow', { action: 'mui.planningtool.costcentre.detail.sectionentity.workflow' });");
                                    strtemplate.Append("$routeProvider.when('/" + strCostCenterUrl + "/detail/sectionentity/:ID/member', { action: 'mui.planningtool.costcentre.detail.sectionentity.member' });");

                                    strtemplate.Append("$routeProvider.when('/" + strCostCenterUrl + "/detail/sectionentity/:ID/attachment', { action: 'mui.planningtool.costcentre.detail.sectionentity.attachment' });");
                                    strtemplate.Append("$routeProvider.when('/" + strCostCenterUrl + "/detail/sectionentity/:ID/attachment/:AssetID', { action: 'mui.planningtool.costcentre.detail.sectionentity.attachment' });");

                                    strtemplate.Append("$routeProvider.when('/" + strCostCenterUrl + "/detail/sectionentity/:ID/presentation', { action: 'mui.planningtool.costcentre.detail.sectionentity.presentation' });");
                                    strtemplate.Append("$routeProvider.when('/" + strCostCenterUrl + "/detail/sectionentity/:ID/objective', { action: 'mui.planningtool.costcentre.detail.sectionentity.objective' });");
                                    strtemplate.Append("$routeProvider.when('/" + strCostCenterUrl + "/detail/sectionentity/:ID/task', { action: 'mui.planningtool.costcentre.detail.sectionentity.task' });");
                                    //strCostCenterUrl = nav.Url.Trim('/');
                                    break;
                                ///Objective replacing
                                case FeatureID.Objective:
                                    strtemplate.Append("$routeProvider.when('/" + nav.Url + "', { action: 'mui.planningtool.objective.list' });");

                                    //Objective related block
                                    strtemplate.Append("$routeProvider.when('/" + strObjective + "/list', { action: 'mui.planningtool.objective.list' });");
                                    strtemplate.Append("$routeProvider.when('/" + strObjective + "/detail', { action: 'mui.planningtool.objective.detail.ganttview' });");
                                    strtemplate.Append("$routeProvider.when('/" + strObjective + "/detail/ganttview', { action: 'mui.planningtool.objective.detail.ganttview' });");
                                    strtemplate.Append("$routeProvider.when('/" + strObjective + "/detail/ganttview/:zoomlevel', { action: 'mui.planningtool.objective.detail.ganttview' });");
                                    strtemplate.Append("$routeProvider.when('/" + strObjective + "/detail/listview', { action: 'mui.planningtool.objective.detail.listview' });");
                                    strtemplate.Append("$routeProvider.when('/" + strObjective + "/detail/section/:ID', { action: 'mui.planningtool.objective.detail.section' });");
                                    strtemplate.Append("$routeProvider.when('/" + strObjective + "/detail/section/:ID/overview', { action: 'mui.planningtool.objective.detail.section.overview' });");
                                    strtemplate.Append("$routeProvider.when('/" + strObjective + "/detail/section/:ID/member', { action: 'mui.planningtool.objective.detail.section.member' });");
                                    strtemplate.Append("$routeProvider.when('/" + strObjective + "/detail/section/:ID/attachment', { action: 'mui.planningtool.objective.detail.section.attachment' });");
                                    strtemplate.Append("$routeProvider.when('/" + strObjective + "/detail/section/:ID/presentation', { action: 'mui.planningtool.objective.detail.section.presentation' });");
                                    strtemplate.Append("$routeProvider.when('/" + strObjective + "/detail/section/:ID/task', { action: 'mui.planningtool.objective.detail.section.task' });");
                                    strtemplate.Append("$routeProvider.when('/" + strObjective + "/detail/section/:ID/task/:TaskID', { action: 'mui.planningtool.objective.detail.section.task' });");
                                    strtemplate.Append("$routeProvider.when('/" + strObjective + "/objectivecreation', { action: 'mui.planningtool.objective.objectivecreation' });");


                                    strtemplate.Append("$routeProvider.when('/" + strObjective + "/detail/ganttview/:zoomlevel/:TrackID', { action: 'mui.planningtool.objective.detail.ganttview' });");
                                    strtemplate.Append("$routeProvider.when('/" + strObjective + "/detail/ganttview/:TrackID', { action: 'mui.planningtool.objective.detail.ganttview' });");
                                    strtemplate.Append("$routeProvider.when('/" + strObjective + "/detail/listview/:TrackID', { action: 'mui.planningtool.objective.detail.listview' });");


                                    strtemplate.Append("$routeProvider.when('/" + strObjective + "/detail/sectionentity/:ID', { action: 'mui.planningtool.objective.detail.sectionentity' });");
                                    strtemplate.Append("$routeProvider.when('/" + strObjective + "/detail/sectionentity/:ID/overview', { action: 'mui.planningtool.objective.detail.sectionentity.overview' });");
                                    strtemplate.Append("$routeProvider.when('/" + strObjective + "/detail/sectionentity/:ID/financial', { action: 'mui.planningtool.objective.detail.sectionentity.financial' });");
                                    strtemplate.Append("$routeProvider.when('/" + strObjective + "/detail/sectionentity/:ID/workflow', { action: 'mui.planningtool.objective.detail.sectionentity.workflow' });");
                                    strtemplate.Append("$routeProvider.when('/" + strObjective + "/detail/sectionentity/:ID/member', { action: 'mui.planningtool.objective.detail.sectionentity.member' });");
                                    strtemplate.Append("$routeProvider.when('/" + strObjective + "/detail/sectionentity/:ID/objective', { action: 'mui.planningtool.objective.detail.sectionentity.objective' });");

                                    strtemplate.Append("$routeProvider.when('/" + strObjective + "/detail/sectionentity/:ID/attachment', { action: 'mui.planningtool.objective.detail.sectionentity.attachment' });");
                                    strtemplate.Append("$routeProvider.when('/" + strObjective + "/detail/sectionentity/:ID/attachment/:AssetID', { action: 'mui.planningtool.objective.detail.sectionentity.attachment' });");



                                    strtemplate.Append("$routeProvider.when('/" + strObjective + "/detail/sectionentity/:ID/presentation', { action: 'mui.planningtool.objective.detail.sectionentity.presentation' });");
                                    strtemplate.Append("$routeProvider.when('/" + strObjective + "/detail/sectionentity/:ID/task', { action: 'mui.planningtool.objective.detail.sectionentity.task' });");

                                    //strObjective = nav.Url.Trim('/');
                                    break;



                                case FeatureID.Calender:
                                    strtemplate.Append("$routeProvider.when('/" + nav.Url + "', { action: 'mui.planningtool.calender.list' });");

                                    //Objective related block
                                    strtemplate.Append("$routeProvider.when('/" + strCalender + "/list', { action: 'mui.planningtool.calender.list' });");
                                    strtemplate.Append("$routeProvider.when('/" + strCalender + "/detail', { action: 'mui.planningtool.calender.detail.ganttview' });");
                                    strtemplate.Append("$routeProvider.when('/" + strCalender + "/detail/ganttview', { action: 'mui.planningtool.calender.detail.ganttview' });");
                                    strtemplate.Append("$routeProvider.when('/" + strCalender + "/detail/ganttview/:zoomlevel', { action: 'mui.planningtool.calender.detail.ganttview' });");
                                    strtemplate.Append("$routeProvider.when('/" + strCalender + "/detail/listview', { action: 'mui.planningtool.calender.detail.listview' });");
                                    strtemplate.Append("$routeProvider.when('/" + strCalender + "/detail/section/:ID', { action: 'mui.planningtool.calender.detail.section' });");
                                    strtemplate.Append("$routeProvider.when('/" + strCalender + "/detail/section/:ID/overview', { action: 'mui.planningtool.calender.detail.section.overview' });");
                                    strtemplate.Append("$routeProvider.when('/" + strCalender + "/detail/section/:ID/member', { action: 'mui.planningtool.calender.detail.section.member' });");
                                    strtemplate.Append("$routeProvider.when('/" + strCalender + "/detail/section/:ID/attachment', { action: 'mui.planningtool.calender.detail.section.attachment' });");
                                    strtemplate.Append("$routeProvider.when('/" + strCalender + "/detail/section/:ID/presentation', { action: 'mui.planningtool.calender.detail.section.presentation' });");
                                    strtemplate.Append("$routeProvider.when('/" + strCalender + "/detail/section/:ID/task', { action: 'mui.planningtool.calender.detail.section.task' });");
                                    strtemplate.Append("$routeProvider.when('/" + strCalender + "/detail/section/:ID/task/:TaskID', { action: 'mui.planningtool.calender.detail.section.task' });");
                                    strtemplate.Append("$routeProvider.when('/" + strCalender + "/calendercreation', { action: 'mui.planningtool.calender.calendercreation' });");


                                    strtemplate.Append("$routeProvider.when('/" + strCalender + "/detail/ganttview/:zoomlevel/:TrackID', { action: 'mui.planningtool.calender.detail.ganttview' });");
                                    strtemplate.Append("$routeProvider.when('/" + strCalender + "/detail/ganttview/:TrackID', { action: 'mui.planningtool.calender.detail.ganttview' });");
                                    strtemplate.Append("$routeProvider.when('/" + strCalender + "/detail/listview/:TrackID', { action: 'mui.planningtool.calender.detail.listview' });");


                                    strtemplate.Append("$routeProvider.when('/" + strCalender + "/detail/sectionentity/:ID', { action: 'mui.planningtool.calender.detail.sectionentity' });");
                                    strtemplate.Append("$routeProvider.when('/" + strCalender + "/detail/sectionentity/:ID/overview', { action: 'mui.planningtool.calender.detail.sectionentity.overview' });");
                                    strtemplate.Append("$routeProvider.when('/" + strCalender + "/detail/sectionentity/:ID/financial', { action: 'mui.planningtool.calender.detail.sectionentity.financial' });");
                                    strtemplate.Append("$routeProvider.when('/" + strCalender + "/detail/sectionentity/:ID/workflow', { action: 'mui.planningtool.calender.detail.sectionentity.workflow' });");
                                    strtemplate.Append("$routeProvider.when('/" + strCalender + "/detail/sectionentity/:ID/member', { action: 'mui.planningtool.calender.detail.sectionentity.member' });");
                                    strtemplate.Append("$routeProvider.when('/" + strCalender + "/detail/sectionentity/:ID/objective', { action: 'mui.planningtool.calender.detail.sectionentity.objective' });");


                                    strtemplate.Append("$routeProvider.when('/" + strCalender + "/detail/sectionentity/:ID/attachment', { action: 'mui.planningtool.calender.detail.sectionentity.attachment' });");
                                    strtemplate.Append("$routeProvider.when('/" + strCalender + "/detail/sectionentity/:ID/attachment/:AssetID', { action: 'mui.planningtool.calender.detail.sectionentity.attachment' });");

                                    strtemplate.Append("$routeProvider.when('/" + strCalender + "/detail/sectionentity/:ID/presentation', { action: 'mui.planningtool.calender.detail.sectionentity.presentation' });");
                                    strtemplate.Append("$routeProvider.when('/" + strCalender + "/detail/sectionentity/:ID/task', { action: 'mui.planningtool.calender.detail.sectionentity.task' });");

                                    //strObjective = nav.Url.Trim('/');
                                    break;


                                ///Dashboard routes replacing
                                case FeatureID.Dashboard:
                                    //strDashBoard.Replace("mui/dashboard", nav.Url);
                                    strtemplate.Append("$routeProvider.when('/" + nav.Url + "', { action: 'mui.dashboard' });");

                                    strtemplate.Append("$routeProvider.when('/" + strDashBoard + "', { action: 'mui.dashboard' });");
                                    //strtemplate.Append("$routeProvider.when('/mui/dashboard', { action: 'mui.dashboard' });");
                                    //strDashBoard =  nav.Url.Trim('/');
                                    break;
                                case FeatureID.ExternalLink:
                                    strtemplate.Append("$routeProvider.when('/" + nav.Url + "/:ID', { action: 'mui.planningtool.iframe' });");
                                    break;
                                case FeatureID.MyTask:
                                    //strtemplate.Append("$routeProvider.when('/" + nav.Url + "',  { action: 'mui.mytask' });");

                                    //Prabhu: Need to make it as two separate page under my page let me know how to achive it Dynamically. More over if i set my task as default it will not work with previous code.
                                    strtemplate.Append("$routeProvider.when('/" + nav.Url + "',  { action: 'mui.mytask' });");
                                    strtemplate.Append("$routeProvider.when('/" + strtask + "',  { action: 'mui.mytask' });");

                                    //strtemplate.Append("$routeProvider.when('/" + strnotification + "',  { action: 'mui.mynotification' });");

                                    break;
                                case FeatureID.GeneralSettings:
                                    ////admin Task related urls
                                    strtemplate.Append("$routeProvider.when('/mui/admin/tasklistlibrary', { action: 'mui.admin.tasklistlibrary' });");
                                    strtemplate.Append("$routeProvider.when('/mui/admin/tasktemplate', { action: 'mui.admin.tasktemplate' });");
                                    strtemplate.Append("$routeProvider.when('/mui/admin/taskflag', { action: 'mui.admin.taskflag' });");
                                    strtemplate.Append("$routeProvider.when('/mui/admin/proofhqcredential', { action: 'mui.admin.proofhqcredential' });");


                                    ////strtemplate.Append("$routeProvider.when('/mui/admin', { action: 'mui.admin' });"); //old commented code
                                    strtemplate.Append("$routeProvider.when('/" + nav.Url + "', { action: 'mui.admin.navigation' });");
                                    strtemplate.Append("$routeProvider.when('/mui/admin/titlelogo', { action: 'mui.admin.titlelogo' });");
                                    strtemplate.Append("$routeProvider.when('/mui/admin/module', { action: 'mui.admin.module' });");
                                    strtemplate.Append("$routeProvider.when('/mui/admin/navigation', { action: 'mui.admin.navigation' });");
                                    strtemplate.Append("$routeProvider.when('/mui/admin/metadata', { action: 'mui.admin.metadata' });");
                                    strtemplate.Append("$routeProvider.when('/mui/admin/applicationsettings', { action: 'mui.admin.applicationsettings' });");
                                    //strtemplate.Append("$routeProvider.when('/mui/metadataconfiguration/entitytypeattributerelation', { action: 'mui.metadataconfiguration.entitytypeattributerelation' });");
                                    strtemplate.Append("$routeProvider.when('/mui/admin/user', { action: 'mui.admin.user' });");
                                    strtemplate.Append("$routeProvider.when('/mui/admin/apiusers', { action: 'mui.admin.apiusers' });");
                                    strtemplate.Append("$routeProvider.when('/mui/admin/role', { action: 'mui.admin.role' });");
                                    strtemplate.Append("$routeProvider.when('/mui/admin/PendingUser', { action: 'mui.admin.pendinguser' });");
                                    strtemplate.Append("$routeProvider.when('/mui/admin/listsettings', { action: 'mui.admin.listsettings' });");
                                    strtemplate.Append("$routeProvider.when('/mui/admin/versionsettings', { action: 'mui.admin.versionsettings' });");
                                    strtemplate.Append("$routeProvider.when('/mui/admin/ganttviewsettings', { action: 'mui.admin.ganttviewsettings' });");
                                    strtemplate.Append("$routeProvider.when('/mui/admin/listviewsettings', { action: 'mui.admin.listviewsettings' });");
                                    strtemplate.Append("$routeProvider.when('/mui/admin/rootlevelfiltersettings', { action: 'mui.admin.rootlevelfiltersettings' });");
                                    strtemplate.Append("$routeProvider.when('/mui/admin/detailfiltersettings', { action: 'mui.admin.detailfiltersettings' });");
                                    strtemplate.Append("$routeProvider.when('/mui/admin/searchengine', { action: 'mui.admin.searchengine' });");
                                    strtemplate.Append("$routeProvider.when('/mui/admin/statistics', { action: 'mui.admin.statistics' });");
                                    strtemplate.Append("$routeProvider.when('/mui/admin/broadcastmsg', { action: 'mui.admin.broadcastmsg' });");
                                    strtemplate.Append("$routeProvider.when('/mui/admin/myworkspacesettings', { action: 'mui.admin.myworkspacesettings' });");
                                    strtemplate.Append("$routeProvider.when('/mui/admin/ganttviewheaderbar', { action: 'mui.admin.ganttviewheaderbar' });");
                                    strtemplate.Append("$routeProvider.when('/mui/admin/tabs', { action: 'mui.admin.tabs' });");
                                    strtemplate.Append("$routeProvider.when('/mui/admin/tabsetting', { action: 'mui.admin.tabsetting' });");
                                    strtemplate.Append("$routeProvider.when('/mui/admin/updates', { action: 'mui.admin.updates' });");
                                    strtemplate.Append("$routeProvider.when('/mui/admin/passwordpolicy', { action: 'mui.admin.passwordpolicy' });");
                                    strtemplate.Append("$routeProvider.when('/mui/admin/plantabsettings', { action: 'mui.admin.plantabsettings' });");

                                    strtemplate.Append("$routeProvider.when('/mui/admin/WorkFlow', { action: 'mui.admin.WorkFlow' });");

                                    strtemplate.Append("$routeProvider.when('/mui/admin/additionalsettings', { action: 'mui.admin.additionalsettings' });");
                                    strtemplate.Append("$routeProvider.when('/mui/admin/NotificationFilterSettings', { action: 'mui.admin.notificationfiltersettings' });");
                                    strtemplate.Append("$routeProvider.when('/mui/admin/NewsFeedFilterSettings', { action: 'mui.admin.newsfeedfiltersettings' });");
                                    strtemplate.Append("$routeProvider.when('/mui/admin/SSO', { action: 'mui.admin.SSO' });");
                                    strtemplate.Append("$routeProvider.when('/mui/admin/languagesettings', { action: 'mui.admin.languagesettings' });");

                                    strtemplate.Append("$routeProvider.when('/mui/admin/POSettings', { action: 'mui.admin.POSettings' });");

                                    strtemplate.Append("$routeProvider.when('/mui/admin/PredefinedWorkTask', { action: 'mui.admin.PredefinedWorkTask' });");

                                    strtemplate.Append("$routeProvider.when('/mui/admin/treesettings', { action: 'mui.admin.treesettings' });");
                                    strtemplate.Append("$routeProvider.when('/mui/admin/assetcreation', { action: 'mui.admin.assetcreation' });");
                                    strtemplate.Append("$routeProvider.when('/mui/admin/Summaryviewsettings', { action: 'mui.admin.Summaryviewsettings' });");
                                    strtemplate.Append("$routeProvider.when('/mui/admin/Listviewsettings', { action: 'mui.admin.Listviewsettings' });");
                                    strtemplate.Append("$routeProvider.when('/mui/admin/Thumbnailviewsettings', { action: 'mui.admin.Thumbnailviewsettings' });");
                                    strtemplate.Append("$routeProvider.when('/mui/admin/FileProfilesettings', { action: 'mui.admin.FileProfilesettings' });");
                                    strtemplate.Append("$routeProvider.when('/mui/admin/optimaker', { action: 'mui.admin.optimaker' });");
                                    //strtemplate.Append("$routeProvider.when('/" + strDashBoardwidget + "/dashboardtemplate', { action: 'mui.admin.dashboardtemplate' });");
                                    //strtemplate.Append("$routeProvider.when('/" + strDashBoardwidget + "/dashboard', { action: 'mui.dashboard' });");

                                    strtemplate.Append("$routeProvider.when('/mui/admin/dashboardwidget', { action: 'mui.admin.dashboardwidget' });");
                                    strtemplate.Append("$routeProvider.when('/mui/admin/dashboardtemplate', { action: 'mui.admin.dashboardtemplate' });");
                                    strtemplate.Append("$routeProvider.when('/mui/admin/dashboard', { action: 'mui.admin.dashboard' });");
                                    strtemplate.Append("$routeProvider.when('/mui/admin/financialforecastsettings', { action: 'mui.admin.financialforecastsettings' });");
                                    strtemplate.Append("$routeProvider.when('/mui/admin/financialforecast', { action: 'mui.admin.financialforecast' });");
                                    strtemplate.Append("$routeProvider.when('/mui/admin/Units', { action: 'mui.admin.Units' });");
                                    strtemplate.Append("$routeProvider.when('/mui/admin/Currencyconvertersettings', { action: 'mui.admin.Currencyconvertersettings' });");
                                    strtemplate.Append("$routeProvider.when('/mui/admin/financialmetadata', { action: 'mui.admin.financialmetadata' });");
                                    strtemplate.Append("$routeProvider.when('/mui/admin/pometadata', { action: 'mui.admin.pometadata' });");
                                    strtemplate.Append("$routeProvider.when('/mui/admin/spenttransaction', { action: 'mui.admin.spenttransaction' });");

                                    strtemplate.Append("$routeProvider.when('/mui/admin/credentialmanagement', { action: 'mui.admin.credentialmanagement' });");
                                    strtemplate.Append("$routeProvider.when('/mui/admin/reports', { action: 'mui.admin.reports' });");
                                    strtemplate.Append("$routeProvider.when('/mui/admin/financialreportsettings', { action: 'mui.admin.financialreportsettings' });");
                                    strtemplate.Append("$routeProvider.when('/mui/admin/customview', { action: 'mui.admin.customview' });");
                                    strtemplate.Append("$routeProvider.when('/mui/admin/ganttviewreport', { action: 'mui.admin.ganttviewreport' });");
                                    strtemplate.Append("$routeProvider.when('/mui/admin/financialreport', { action: 'mui.admin.financialreport' });");
                                    strtemplate.Append("$routeProvider.when('/mui/admin/customlist', { action: 'mui.admin.customlist' });");
                                    strtemplate.Append("$routeProvider.when('/mui/admin/assetroles', { action: 'mui.admin.assetroles' });");
                                    strtemplate.Append("$routeProvider.when('/mui/admin/damviewsettings', { action: 'mui.admin.damviewsettings' });");
                                    strtemplate.Append("$routeProvider.when('/mui/admin/publishedsearchsettings', { action: 'mui.admin.publishedsearchsettings' });");
                                    strtemplate.Append("$routeProvider.when('/mui/admin/Layouttabsettings', { action: 'mui.admin.Layouttabsettings' });");
                                    strtemplate.Append("$routeProvider.when('/mui/admin/designlayout', { action: 'mui.admin.designlayout' });");
                                    strtemplate.Append("$routeProvider.when('/mui/admin/searchcriteria', { action: 'mui.admin.searchcriteria' });");
                                    strtemplate.Append("$routeProvider.when('/mui/admin/imageunits', { action: 'mui.admin.imageunits' });");
                                    break;
                                case FeatureID.MetadataSettings:
                                    strtemplate.Append("$routeProvider.when('/" + nav.Url + "', { action: 'mui.metadatasettings' });");
                                    strtemplate.Append("$routeProvider.when('/mui/metadataconfiguration/entitytypeattributerelation', { action: 'mui.metadataconfiguration.entitytypeattributerelation' });");
                                    strtemplate.Append("$routeProvider.when('/mui/metadataconfiguration/Taskentitytype', { action: 'mui.metadataconfiguration.Taskentitytype' });");
                                    strtemplate.Append("$routeProvider.when('/mui/metadataconfiguration/Damentitytype', { action: 'mui.metadataconfiguration.Damentitytype' });");
                                    strtemplate.Append("$routeProvider.when('/mui/metadatasettings', { action: 'mui.metadatasettings' });");
                                    strtemplate.Append("$routeProvider.when('/mui/MetadataConfiguration', { action: 'mui.MetadataConfiguration' });");
                                    strtemplate.Append("$routeProvider.when('/mui/metadataconfiguration/attribute', { action: 'mui.metadataconfiguration.attribute' });");
                                    strtemplate.Append("$routeProvider.when('/mui/metadataconfiguration/attributegroup', { action: 'mui.metadataconfiguration.attributegroup' });");
                                    strtemplate.Append("$routeProvider.when('/mui/metadataconfiguration/userdetails', { action: 'mui.metadataconfiguration.userdetails' });");
                                    strtemplate.Append("$routeProvider.when('/mui/metadataconfiguration/cmspagetypeattributerelation', { action: 'mui.metadataconfiguration.cmspagetypeattributerelation' });");
                                    break;
                                case FeatureID.Support:
                                    strtemplate.Append("$routeProvider.when('/" + nav.Url + "',  { action: 'mui.support' });");
                                    break;
                                case FeatureID.Notification:
                                    strtemplate.Append("$routeProvider.when('/" + nav.Url + "',  { action: 'mui.mynotification' });");
                                    strtemplate.Append("$routeProvider.when('/" + strnotification + "',  { action: 'mui.mynotification' });");
                                    break;
                                case FeatureID.FundRequest:
                                    strtemplate.Append("$routeProvider.when('/" + strfundRequest + "',  { action: 'mui.myfundingrequest' });");
                                    strtemplate.Append("$routeProvider.when('/" + nav.Url + "',  { action: 'mui.myfundingrequest' });");
                                    break;
                                case FeatureID.Workspaces:
                                    strtemplate.Append("$routeProvider.when('/" + nav.Url + "',  { action: 'mui.planningtool.workspaces.list' });");
                                    strtemplate.Append("$routeProvider.when('/" + strworkspaces + "',  { action: 'mui.planningtool.workspaces.list' });");

                                    strtemplate.Append("$routeProvider.when('/mui/planningtool/workspaces/list',  { action: 'mui.planningtool.workspaces.list' });");

                                    // New code for the Redirecting the entity the user who doesnt not have plan access
                                    strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/SubEntityCreation', { action: 'mui.planningtool.SubEntityCreation' });");
                                    strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/detail', { action: 'mui.planningtool.default.detail.ganttview' });");
                                    strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/detail/ganttview', { action: 'mui.planningtool.default.detail.ganttview' });");
                                    strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/detail/ganttview/:zoomlevel', { action: 'mui.planningtool.default.detail.ganttview' });");

                                    strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/detail/ganttview/:zoomlevel/:TrackID', { action: 'mui.planningtool.default.detail.ganttview' });");
                                    strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/detail/ganttview/plan/:TrackID', { action: 'mui.planningtool.default.detail.ganttview' });");
                                    strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/detail/listview/:TrackID', { action: 'mui.planningtool.default.detail.listview' });");

                                    strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/detail/listview', { action: 'mui.planningtool.default.detail.listview' });");
                                    strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/detail/section/:ID', { action: 'mui.planningtool.default.detail.section' });");
                                    strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/detail/section/:ID/overview', { action: 'mui.planningtool.default.detail.section.overview' });");

                                    strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/detail/section/:ID/financial', { action: 'mui.planningtool.default.detail.section.financial' });");
                                    strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/detail/section/:ID/financial/:FundinRequestID', { action: 'mui.planningtool.default.detail.section.financial' });");
                                    strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/detail/section/:ID/workflow', { action: 'mui.planningtool.default.detail.section.workflow' });");
                                    strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/detail/section/:ID/member', { action: 'mui.planningtool.default.detail.section.member' });");
                                    strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/detail/section/:ID/objective', { action: 'mui.planningtool.default.detail.section.objective' });");
                                    strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/detail/section/:ID/attachment', { action: 'mui.planningtool.default.detail.section.attachment' });");
                                    strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/detail/section/:ID/attachment/:AssetID', { action: 'mui.planningtool.default.detail.section.attachment' });");
                                    strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/detail/section/:ID/presentation', { action: 'mui.planningtool.default.detail.section.presentation' });");
                                    strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/detail/section/:ID/task', { action: 'mui.planningtool.default.detail.section.task' });");
                                    strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/detail/section/:ID/task/:TaskID', { action: 'mui.planningtool.default.detail.section.task' });");
                                    strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/filtersettings', { action: 'mui.planningtool.filtersettings' });");
                                    strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/detail/detailfilter', { action: 'mui.planningtool.default.detail.detailfilter' });");
                                    break;
                                ///MediaBank routes replacing
                                case FeatureID.MediaBank:
                                    strtemplate.Append("$routeProvider.when('/" + nav.Url + "', { action: 'mui.mediabank' });");
                                    strtemplate.Append("$routeProvider.when('/mediabank', { action: 'mui.mediabank' });");
                                    break;
                                //case FeatureID.DAM:
                                //    strtemplate.Append("$routeProvider.when('/mui/DAM/assetcreation',  { action: 'mui.DAM.assetcreation' });");
                                //    break;
                                case FeatureID.Full_Page_CMS:
                                    strtemplate.Append("$routeProvider.when('/" + nav.Url + "', { action: 'mui.cms.default.fullcmspage' });");
                                    strtemplate.Append("$routeProvider.when('/" + strcmswithnavigation + "/fullcmspage', { action: 'mui.cms.default.fullcmspage' });");

                                    strtemplate.Append("$routeProvider.when('/" + strcmswithnavigation + "/fullcmspage/list/:ID/entitycontent', { action: 'mui.cms.default.fullcmspage.list.entitycontent' });");
                                    strtemplate.Append("$routeProvider.when('/" + strcmswithnavigation + "/fullcmspage/section/:ID', { action: 'mui.cms.default.fullcmspage.section' });");

                                    strtemplate.Append("$routeProvider.when('/" + strcmswithnavigation + "/fullcmspage/section/:ID/content', { action: 'mui.cms.default.fullcmspage.section.content' });");
                                    strtemplate.Append("$routeProvider.when('/" + strcmswithnavigation + "/fullcmspage/section/:ID/settings', { action: 'mui.cms.default.fullcmspage.section.settings' });");

                                    strtemplate.Append("$routeProvider.when('/" + strcmswithnavigation + "/fullcmspage/section/:ID/overview', { action: 'mui.cms.default.fullcmspage.section.overview' });");
                                    strtemplate.Append("$routeProvider.when('/" + strcmswithnavigation + "/fullcmspage/section/:ID/member', { action: 'mui.cms.default.fullcmspage.section.member' });");

                                    strtemplate.Append("$routeProvider.when('/" + strcmswithnavigation + "/fullcmspage/section/:ID/task', { action: 'mui.cms.default.fullcmspage.section.task' });");
                                    strtemplate.Append("$routeProvider.when('/" + strcmswithnavigation + "/fullcmspage/section/:ID/task/:TaskID', { action: 'mui.cms.default.fullcmspage.section.task' });");
                                    strtemplate.Append("$routeProvider.when('/" + strcmswithnavigation + "/fullcmspage/section/:ID/attachment', { action: 'mui.cms.default.fullcmspage.section.attachment' });");
                                    strtemplate.Append("$routeProvider.when('/" + strcmswithnavigation + "/fullcmspage/section/:ID/attachment/:AssetID', { action: 'mui.cms.default.fullcmspage.section.attachment' });");

                                    break;
                                case FeatureID.CMS_With_Navigation:
                                    strtemplate.Append("$routeProvider.when('/" + nav.Url + "', { action: 'mui.cms.default.detail' });");
                                    strtemplate.Append("$routeProvider.when('/" + strcmswithnavigation + "/detail', { action: 'mui.cms.default.detail' });");
                                    strtemplate.Append("$routeProvider.when('/" + strcmswithnavigation + "/detail/section/:ID', { action: 'mui.cms.default.detail.section' });");
                                    strtemplate.Append("$routeProvider.when('/mui/cms/createcmsentity', { action: 'mui.cms.createcmsentity' });");

                                    //strtemplate.Append("$routeProvider.when('/mui/cms/default/list/', { action: 'mui.cms.default.list' });");

                                    strtemplate.Append("$routeProvider.when('/" + strcmswithnavigation + "/detail/list/:ID', { action: 'mui.cms.default.detail.list' });");
                                    strtemplate.Append("$routeProvider.when('/" + strcmswithnavigation + "/detail/list/:ID/entitycontent', { action: 'mui.cms.default.detail.list.entitycontent' });");

                                    strtemplate.Append("$routeProvider.when('/" + strcmswithnavigation + "/detail/section/:ID/content', { action: 'mui.cms.default.detail.section.content' });");
                                    strtemplate.Append("$routeProvider.when('/" + strcmswithnavigation + "/detail/section/:ID/settings', { action: 'mui.cms.default.detail.section.settings' });");

                                    strtemplate.Append("$routeProvider.when('/" + strcmswithnavigation + "/detail/section/:ID/overview', { action: 'mui.cms.default.detail.section.overview' });");
                                    strtemplate.Append("$routeProvider.when('/" + strcmswithnavigation + "/detail/section/:ID/member', { action: 'mui.cms.default.detail.section.member' });");

                                    strtemplate.Append("$routeProvider.when('/" + strcmswithnavigation + "/detail/section/:ID/task', { action: 'mui.cms.default.detail.section.task' });");
                                    strtemplate.Append("$routeProvider.when('/" + strcmswithnavigation + "/detail/section/:ID/task/:TaskID', { action: 'mui.cms.default.detail.section.task' });");
                                    strtemplate.Append("$routeProvider.when('/" + strcmswithnavigation + "/detail/section/:ID/attachment', { action: 'mui.cms.default.detail.section.attachment' });");
                                    strtemplate.Append("$routeProvider.when('/" + strcmswithnavigation + "/detail/section/:ID/attachment/:AssetID', { action: 'mui.cms.default.detail.section.attachment' });");

                                    break;
                                default:
                                    break;
                            }

                        }
                    }
                    switch (proxy.MarcomManager.User.StartPage)
                    {
                        case (int)FeatureID.Plan:
                            if (applicableNavigation.Where(p => p.Featureid == proxy.MarcomManager.User.StartPage).Count() != 0)
                                defaultRoute = strActivityUrl + "/list";
                            break;
                        case (int)FeatureID.CostCenter:
                            if (applicableNavigation.Where(p => p.Featureid == proxy.MarcomManager.User.StartPage).Count() != 0)
                                defaultRoute = strCostCenterUrl + "/list";
                            break;
                        case (int)FeatureID.Objective:
                            if (applicableNavigation.Where(p => p.Featureid == proxy.MarcomManager.User.StartPage).Count() != 0)
                                defaultRoute = strObjective + "/list";
                            break;
                        case (int)FeatureID.Dashboard:
                            if (applicableNavigation.Where(p => p.Featureid == proxy.MarcomManager.User.StartPage).Count() != 0)
                                defaultRoute = strDashBoard;
                            break;
                        case (int)FeatureID.Mypage:
                            if (applicableNavigation.Where(p => p.Featureid == proxy.MarcomManager.User.StartPage).Count() != 0)
                                defaultRoute = strmypage;
                            break;
                        case (int)FeatureID.GeneralSettings:
                            if (applicableNavigation.Where(p => p.Featureid == proxy.MarcomManager.User.StartPage).Count() != 0)
                                defaultRoute = stradminsettings;
                            break;
                        case (int)FeatureID.MetadataSettings:
                            if (applicableNavigation.Where(p => p.Featureid == proxy.MarcomManager.User.StartPage).Count() != 0)
                                defaultRoute = strmetadatasettings;
                            break;
                        case (int)FeatureID.MyTask:
                            if (applicableNavigation.Where(p => p.Featureid == proxy.MarcomManager.User.StartPage).Count() != 0)
                                defaultRoute = strtask;
                            break;
                        case (int)FeatureID.Workspaces:
                            if (applicableNavigation.Where(p => p.Featureid == proxy.MarcomManager.User.StartPage).Count() != 0)
                                defaultRoute = strworkspaces;
                            break;
                        case (int)FeatureID.Notification:
                            if (applicableNavigation.Where(p => p.Featureid == proxy.MarcomManager.User.StartPage).Count() != 0)
                                defaultRoute = strnotification;
                            break;
                        case (int)FeatureID.FundRequest:
                            if (applicableNavigation.Where(p => p.Featureid == proxy.MarcomManager.User.StartPage).Count() != 0)
                                defaultRoute = strfundRequest;
                            break;
                        case (int)FeatureID.MediaBank:
                            if (applicableNavigation.Where(p => p.Featureid == proxy.MarcomManager.User.StartPage).Count() != 0)
                                defaultRoute = strfundRequest;
                            break;
                        case (int)FeatureID.CMS_With_Navigation:
                            if (applicableNavigation.Where(p => p.Featureid == proxy.MarcomManager.User.StartPage).Count() != 0)
                                defaultRoute = strcmswithnavigation + "/detail";
                            break;
                        case (int)FeatureID.Full_Page_CMS:
                            if (applicableNavigation.Where(p => p.Featureid == proxy.MarcomManager.User.StartPage).Count() != 0)
                                defaultRoute = strcmswithnavigation + "/fullcmspage";
                            break;
                        default:
                            break;
                    }
                    if (defaultRoute == string.Empty)
                    {

                        if (applicableNavigation.Where(p => p.Featureid == (int)FeatureID.Plan).Count() != 0)
                            defaultRoute = strActivityUrl + "/list";
                        else if (applicableNavigation.Where(p => p.Featureid == (int)FeatureID.CostCenter).Count() != 0)
                            defaultRoute = strCostCenterUrl + "/list";
                        else if (applicableNavigation.Where(p => p.Featureid == (int)FeatureID.Objective).Count() != 0)
                            defaultRoute = strObjective + "/list";
                        else
                            defaultRoute = strmypage;

                    }
                }
                strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/customtab/:tabID', { action: 'mui.planningtool.default.detail.section.customtab' });");
                strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/customtab/:tabID/:ID', { action: 'mui.planningtool.default.detail.section.customtab' });");

                strtemplate.Append("$routeProvider.when('/" + strCostCenterUrl + "/customtab/:tabID', { action: 'mui.planningtool.costcentre.detail.section.customtab' });");
                strtemplate.Append("$routeProvider.when('/" + strCostCenterUrl + "/customtab/:tabID/:ID', { action: 'mui.planningtool.costcentre.detail.section.customtab' });");
                strtemplate.Append("$routeProvider.when('/" + strCostCenterUrl + "/customtab/sectionentity/:tabID/:ID', { action: 'mui.planningtool.costcentre.detail.sectionentity.customtab' });");
                strtemplate.Append("$routeProvider.when('/" + strCostCenterUrl + "/customtab/sectionentity/:ID', { action: 'mui.planningtool.costcentre.detail.sectionentity.customtab' });");

                strtemplate.Append("$routeProvider.when('/" + strObjective + "/customtab/:tabID', { action: 'mui.planningtool.objective.detail.section.customtab' });");
                strtemplate.Append("$routeProvider.when('/" + strObjective + "/customtab/:tabID/:ID', { action: 'mui.planningtool.objective.detail.section.customtab' });");
                strtemplate.Append("$routeProvider.when('/" + strObjective + "/customtab/sectionentity/:tabID/:ID', { action: 'mui.planningtool.objective.detail.sectionentity.customtab' });");
                strtemplate.Append("$routeProvider.when('/" + strObjective + "/customtab/sectionentity/:ID', { action: 'mui.planningtool.objective.detail.sectionentity.customtab' });");

                strtemplate.Append("$routeProvider.when('/" + strcmswithnavigation + "/customtab/:tabID', { action: 'mui.cms.default.detail.section.customtab' });");
                strtemplate.Append("$routeProvider.when('/" + strcmswithnavigation + "/customtab/:tabID/:ID', { action: 'mui.cms.default.detail.section.customtab' });");


                //strtemplate.Append("$routeProvider.when('/" + strCalender + "/customtab/:tabID', { action: 'mui.planningtool.calender.detail.section.customtab' });");
                //strtemplate.Append("$routeProvider.when('/" + strCalender + "/customtab/:tabID/:ID', { action: 'mui.planningtool.calender.detail.section.customtab' });");
                //strtemplate.Append("$routeProvider.when('/" + strCalender + "/customtab/sectionentity/:tabID/:ID', { action: 'mui.planningtool.calender.detail.sectionentity.customtab' });");
                //strtemplate.Append("$routeProvider.when('/" + strCalender + "/customtab/sectionentity/:ID', { action: 'mui.planningtool.calender.detail.sectionentity.customtab' });");

                strtemplate.Append("$routeProvider.when('/mui/mypage', { action: 'mui.mypage' });");
                strtemplate.Append("$routeProvider.when('/mui/mypage/:section', { action: 'mui.mypage' });");

                strtemplate.Append("$routeProvider.when('/mui/support', { action: 'mui.support' });");
                strtemplate.Append("$routeProvider.when('/mui/Notification', { action: 'mui.Notification' });");
                strtemplate.Append("$routeProvider.when('/mui/searchresult', { action: 'mui.searchresult' });");
                strtemplate.Append("$routeProvider.when('/mui/customsearchresult', { action: 'mui.customsearchresult' });");
                strtemplate.Append("$routeProvider.otherwise({ redirectTo: '/" + defaultRoute + "' });");
                strtemplate.Append("$routeProvider.when('/mui/dam',  { action: 'mui.dam' });");
                //strtemplate.Append("$routeProvider.otherwise({ redirectTo: '/mui/playground' });");
                strtemplate.Append("}");
                strtemplate.Append(");");
                return strtemplate.ToString();

            }


            catch
            {

            }

            return null;
        }

        /// <summary>
        /// Get Navigation Config.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <returns>String</returns>
        public string GetMediabankNavigationConfig(CommonManagerProxy proxy)
        {
            try
            {
                StringBuilder strtemplate = new StringBuilder();
                string strActivityUrl = "mui/planningtool";

                //strtemplate.Append("var app = angular.module('app', ['ngResource', 'ngCookies', 'ngGrid']);");

                strtemplate.Append("app.config(");
                strtemplate.Append("function ($routeProvider) {");
                //string strDashBoardwidget = "mui/admin/dashboardwidget";
                string defaultRoute = string.Empty;

                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    ///to get value from database
                    //  var usernavdetails =(from tt in tx.PersistenceManager.AccessRepository.Query<UserDao>() where tt.Id==proxy.MarcomManager.User.Id select tt).FirstOrDefault();

                    var topnavigationfeatures = proxy.MarcomManager.MetadataManager.GetFeaturesForTopNavigation();

                    IList<NavigationDao> navList = new List<NavigationDao>();
                    navList = tx.PersistenceManager.CommonRepository.Query<NavigationDao>().ToList();

                    var applicableNavigation = (from access in proxy.MarcomManager.User.ListOfUserGlobalRoles
                                                join navs in navList
                                                    on access.Moduleid equals navs.Moduleid
                                                where access.Featureid == navs.Featureid
                                                select navs).OrderBy(p => p.Id).ToList();

                    var supNav = (from nav in navList where nav.Featureid == 23 select nav).FirstOrDefault();

                    if (supNav != null)
                    {

                        NavigationDao dao = new NavigationDao();
                        dao.AddLanguageCode = supNav.AddLanguageCode;
                        dao.AddUserEmail = supNav.AddUserEmail;
                        dao.AddUserID = supNav.AddUserID;
                        dao.AddUserName = supNav.AddUserName;
                        dao.Caption = supNav.Caption;

                        dao.Description = supNav.Description;
                        dao.ExternalUrl = supNav.ExternalUrl;
                        dao.Featureid = supNav.Featureid;
                        dao.Id = supNav.Id;
                        applicableNavigation.Add(supNav);
                    }

                    tx.Commit();
                    if (applicableNavigation != null && applicableNavigation.Count() > 0)
                    {
                        ///replacing routes from database 
                        foreach (var nav in applicableNavigation)
                        {

                            switch ((FeatureID)nav.Featureid)
                            {
                                ///activity routes replacing
                                case FeatureID.Plan:
                                    strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/detail/section/:ID/attachment', { action: 'mui.planningtool.default.detail.section.attachment' });");
                                    strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/detail/section/:ID/attachment/:AssetID', { action: 'mui.planningtool.default.detail.section.attachment' });");
                                    break;
                                ///MediaBank routes replacing
                                case FeatureID.MediaBank:
                                    strtemplate.Append("$routeProvider.when('/mediabank', { action: 'mbmui.mediabank' });");
                                    break;
                                default:
                                    break;
                            }

                        }
                    }

                }
                strtemplate.Append("$routeProvider.otherwise({ redirectTo: '/mediabank' });");
                strtemplate.Append("}");
                strtemplate.Append(");");
                return strtemplate.ToString();

            }
            catch
            {

            }

            return null;
        }



        public string GetCalendarNavigationConfig(CommonManagerProxy proxy)
        {
            try
            {
                StringBuilder strtemplate = new StringBuilder();
                string strActivityUrl = "mui/planningtool";
                string strCalender = "mui/planningtool/calender";

                //strtemplate.Append("var app = angular.module('app', ['ngResource', 'ngCookies', 'ngGrid']);");

                strtemplate.Append("app.config(");
                strtemplate.Append("function ($routeProvider) {");
                //string strDashBoardwidget = "mui/admin/dashboardwidget";
                string defaultRoute = string.Empty;

                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    ///to get value from database
                    //  var usernavdetails =(from tt in tx.PersistenceManager.AccessRepository.Query<UserDao>() where tt.Id==proxy.MarcomManager.User.Id select tt).FirstOrDefault();

                    var topnavigationfeatures = proxy.MarcomManager.MetadataManager.GetFeaturesForTopNavigation();

                    IList<NavigationDao> navList = new List<NavigationDao>();
                    navList = tx.PersistenceManager.CommonRepository.Query<NavigationDao>().ToList();

                    var applicableNavigation = (from access in proxy.MarcomManager.User.ListOfUserGlobalRoles
                                                join navs in navList
                                                    on access.Moduleid equals navs.Moduleid
                                                where access.Featureid == navs.Featureid
                                                select navs).OrderBy(p => p.Id).ToList();

                    var supNav = (from nav in navList where nav.Featureid == 23 select nav).FirstOrDefault();

                    if (supNav != null)
                    {

                        NavigationDao dao = new NavigationDao();
                        dao.AddLanguageCode = supNav.AddLanguageCode;
                        dao.AddUserEmail = supNav.AddUserEmail;
                        dao.AddUserID = supNav.AddUserID;
                        dao.AddUserName = supNav.AddUserName;
                        dao.Caption = supNav.Caption;

                        dao.Description = supNav.Description;
                        dao.ExternalUrl = supNav.ExternalUrl;
                        dao.Featureid = supNav.Featureid;
                        dao.Id = supNav.Id;
                        applicableNavigation.Add(supNav);
                    }

                    tx.Commit();
                    if (applicableNavigation != null && applicableNavigation.Count() > 0)
                    {
                        ///replacing routes from database 
                        foreach (var nav in applicableNavigation)
                        {

                            switch ((FeatureID)nav.Featureid)
                            {
                                ///activity routes replacing
                                case FeatureID.Plan:

                                    strtemplate.Append("$routeProvider.when('/" + nav.Url + "', { action: 'mui.planningtool.default.list' });");

                                    ///Creating url tempalate for navigation
                                    //ActvityList related block
                                    strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/list', { action: 'mui.planningtool.default.list' });");
                                    strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/RootEntityTypeCreation', { action: 'mui.planningtool.RootEntityTypeCreation' });");
                                    strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/SubEntityCreation', { action: 'mui.planningtool.SubEntityCreation' });");
                                    strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/detail', { action: 'mui.planningtool.default.detail.ganttview' });");
                                    strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/detail/ganttview', { action: 'mui.planningtool.default.detail.ganttview' });");
                                    strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/detail/ganttview/:zoomlevel', { action: 'mui.planningtool.default.detail.ganttview' });");

                                    strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/detail/ganttview/:zoomlevel/:TrackID', { action: 'mui.planningtool.default.detail.ganttview' });");
                                    strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/detail/ganttview/plan/:TrackID', { action: 'mui.planningtool.default.detail.ganttview' });");
                                    strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/detail/listview/:TrackID', { action: 'mui.planningtool.default.detail.listview' });");
                                    //strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/detail/section/:ID/overview/:TrackID', { action: 'mui.planningtool.default.tests.tests2.overview' });");
                                    //strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/detail/section/:ID/:TrackID', { action: 'mui.planningtool.default.detail.section' });");

                                    strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/detail/listview', { action: 'mui.planningtool.default.detail.listview' });");
                                    strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/detail/section/:ID', { action: 'mui.planningtool.default.detail.section' });");
                                    strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/detail/section/:ID/overview', { action: 'mui.planningtool.default.detail.section.overview' });");

                                    //strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/tests/tests2/:ID/overview', { action: 'mui.planningtool.default.tests.tests2.overview' });");
                                    //strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/tests/section/:ID/overview', { action: 'mui.planningtool.default.tests.section.overview' });");
                                    //strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/detail/tests2/:ID/overview', { action: 'mui.planningtool.default.detail.tests2.overview' });");


                                    strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/detail/section/:ID/financial', { action: 'mui.planningtool.default.detail.section.financial' });");
                                    strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/detail/section/:ID/financial/:FundinRequestID', { action: 'mui.planningtool.default.detail.section.financial' });");
                                    strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/detail/section/:ID/workflow', { action: 'mui.planningtool.default.detail.section.workflow' });");
                                    strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/detail/section/:ID/member', { action: 'mui.planningtool.default.detail.section.member' });");
                                    strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/detail/section/:ID/objective', { action: 'mui.planningtool.default.detail.section.objective' });");
                                    strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/detail/section/:ID/attachment', { action: 'mui.planningtool.default.detail.section.attachment' });");
                                    strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/detail/section/:ID/attachment/:AssetID', { action: 'mui.planningtool.default.detail.section.attachment' });");
                                    strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/detail/section/:ID/presentation', { action: 'mui.planningtool.default.detail.section.presentation' });");
                                    strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/detail/section/:ID/task', { action: 'mui.planningtool.default.detail.section.task' });");
                                    strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/detail/section/:ID/task/:TaskID', { action: 'mui.planningtool.default.detail.section.task' });");
                                    strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/filtersettings', { action: 'mui.planningtool.filtersettings' });");
                                    strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/detail/detailfilter', { action: 'mui.planningtool.default.detail.detailfilter' });");



                                    //strtemplate.Append("$routeProvider.when('/" + strActivityUrl + "/detail/detailfilter', { action: 'mui.dam' });");
                                    break;
                               
                                ///
                                case FeatureID.Calender:
                                    strtemplate.Append("$routeProvider.when('/" + nav.Url + "', { action: 'mui.planningtool.calender.list' });");

                                    //Calendar related block
                                    strtemplate.Append("$routeProvider.when('/" + strCalender + "/list', { action: 'mui.planningtool.calender.list' });");
                                    strtemplate.Append("$routeProvider.when('/" + strCalender + "/detail', { action: 'mui.planningtool.calender.detail.ganttview' });");
                                    strtemplate.Append("$routeProvider.when('/" + strCalender + "/detail/ganttview', { action: 'mui.planningtool.calender.detail.ganttview' });");
                                    strtemplate.Append("$routeProvider.when('/" + strCalender + "/detail/ganttview/:zoomlevel', { action: 'mui.planningtool.calender.detail.ganttview' });");
                                    strtemplate.Append("$routeProvider.when('/" + strCalender + "/detail/listview', { action: 'mui.planningtool.calender.detail.listview' });");
                                    strtemplate.Append("$routeProvider.when('/" + strCalender + "/detail/section/:ID', { action: 'mui.planningtool.calender.detail.section' });");
                                    strtemplate.Append("$routeProvider.when('/" + strCalender + "/detail/section/:ID/overview', { action: 'mui.planningtool.calender.detail.section.overview' });");
                                    strtemplate.Append("$routeProvider.when('/" + strCalender + "/detail/section/:ID/member', { action: 'mui.planningtool.calender.detail.section.member' });");
                                    strtemplate.Append("$routeProvider.when('/" + strCalender + "/detail/section/:ID/attachment', { action: 'mui.planningtool.calender.detail.section.attachment' });");
                                    strtemplate.Append("$routeProvider.when('/" + strCalender + "/detail/section/:ID/presentation', { action: 'mui.planningtool.calender.detail.section.presentation' });");
                                    strtemplate.Append("$routeProvider.when('/" + strCalender + "/detail/section/:ID/task', { action: 'mui.planningtool.calender.detail.section.task' });");
                                    strtemplate.Append("$routeProvider.when('/" + strCalender + "/detail/section/:ID/task/:TaskID', { action: 'mui.planningtool.calender.detail.section.task' });");
                                    strtemplate.Append("$routeProvider.when('/" + strCalender + "/calendercreation', { action: 'mui.planningtool.calender.calendercreation' });");


                                    strtemplate.Append("$routeProvider.when('/" + strCalender + "/detail/ganttview/:zoomlevel/:TrackID', { action: 'mui.planningtool.calender.detail.ganttview' });");
                                    strtemplate.Append("$routeProvider.when('/" + strCalender + "/detail/ganttview/:TrackID', { action: 'mui.planningtool.calender.detail.ganttview' });");
                                    strtemplate.Append("$routeProvider.when('/" + strCalender + "/detail/listview/:TrackID', { action: 'mui.planningtool.calender.detail.listview' });");


                                    strtemplate.Append("$routeProvider.when('/" + strCalender + "/detail/sectionentity/:ID', { action: 'mui.planningtool.calender.detail.sectionentity' });");
                                    strtemplate.Append("$routeProvider.when('/" + strCalender + "/detail/sectionentity/:ID/overview', { action: 'mui.planningtool.calender.detail.sectionentity.overview' });");
                                    strtemplate.Append("$routeProvider.when('/" + strCalender + "/detail/sectionentity/:ID/financial', { action: 'mui.planningtool.calender.detail.sectionentity.financial' });");
                                    strtemplate.Append("$routeProvider.when('/" + strCalender + "/detail/sectionentity/:ID/workflow', { action: 'mui.planningtool.calender.detail.sectionentity.workflow' });");
                                    strtemplate.Append("$routeProvider.when('/" + strCalender + "/detail/sectionentity/:ID/member', { action: 'mui.planningtool.calender.detail.sectionentity.member' });");
                                    strtemplate.Append("$routeProvider.when('/" + strCalender + "/detail/sectionentity/:ID/objective', { action: 'mui.planningtool.calender.detail.sectionentity.objective' });");


                                    strtemplate.Append("$routeProvider.when('/" + strCalender + "/detail/sectionentity/:ID/attachment', { action: 'mui.planningtool.calender.detail.sectionentity.attachment' });");
                                    strtemplate.Append("$routeProvider.when('/" + strCalender + "/detail/sectionentity/:ID/attachment/:AssetID', { action: 'mui.planningtool.calender.detail.sectionentity.attachment' });");

                                    strtemplate.Append("$routeProvider.when('/" + strCalender + "/detail/sectionentity/:ID/presentation', { action: 'mui.planningtool.calender.detail.sectionentity.presentation' });");
                                    strtemplate.Append("$routeProvider.when('/" + strCalender + "/detail/sectionentity/:ID/task', { action: 'mui.planningtool.calender.detail.sectionentity.task' });");
                                    //strtemplate.Append("$routeProvider.when('/calendar', { action: 'calmui.calendar' });");
                                    //strObjective = nav.Url.Trim('/');
                                    break;
                                ///MediaBank routes replacing
                                case FeatureID.MediaBank:
                                    strtemplate.Append("$routeProvider.when('/mediabank', { action: 'mui.mediabank' });");
                                    break;
                                default:
                                    break;
                            }

                        }
                    }

                }
                strtemplate.Append("$routeProvider.otherwise({ redirectTo: '/planningtool' });");
                strtemplate.Append("}");
                strtemplate.Append(");");
                return strtemplate.ToString();

            }
            catch
            {

            }

            return null;
        }



        /// <summary>
        /// Inerting and Updating AdminSettings.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="key">The SettingKey.</param>
        /// <returns>True (or) False</returns>
        public bool AdminSettingsInsertUpdate(CommonManagerProxy proxy, string jsondata, string key, string data)
        {
            //SaveImage(data);

            string uploadPath = Path.Combine(HttpRuntime.AppDomainAppPath);
            string uploadImagePath = uploadPath + "assets\\img\\logo.png";

            if (System.IO.File.Exists(uploadImagePath))
            {
                System.IO.File.Delete(uploadImagePath);
            }
            System.IO.File.Copy(uploadPath + "\\" + data, uploadImagePath);

            dynamic jsObject = JsonConvert.DeserializeObject(jsondata);
            JProperty jprop = new JProperty(jsondata);
            // string xmlpath = AppDomain.CurrentDomain.BaseDirectory + "\\AdminSettings.xml";
            string xmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
            XDocument adminXmlDoc = XDocument.Load(xmlpath);
            //The Key is root node current Settings
            string xelementName = key;
            var xelementFilepath = XElement.Load(xmlpath);
            var xmlElement = xelementFilepath.Element(xelementName);
            if (xmlElement != null)
            {
                adminXmlDoc.Descendants(xelementName).Remove();
                adminXmlDoc.Save(xmlpath);
                XDocument xDocparse = JsonConvert.DeserializeXNode(jsondata);
                var vae = Convert.ToString(xDocparse);
                adminXmlDoc.Element("AppSettings").Add(xDocparse.Nodes().ElementAt(0));
                adminXmlDoc.Save(xmlpath);
            }
            else if (xmlElement == null)
            {
                XDocument xDocparse = JsonConvert.DeserializeXNode(jsondata);
                var vae = Convert.ToString(xDocparse);
                adminXmlDoc.Element("AppSettings").Add(xDocparse.Nodes().ElementAt(0));
                adminXmlDoc.Save(xmlpath);
            }

            return true;
        }

        public bool AdminSettingsforRootLevelInsertUpdate(CommonManagerProxy proxy, string jsondata, string key, int typeid)
        {

            dynamic jsObject = JsonConvert.DeserializeObject(jsondata);
            JProperty jprop = new JProperty(jsondata);
            // string xmlpath = AppDomain.CurrentDomain.BaseDirectory + "\\AdminSettings.xml";
            string xmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
            XDocument adminXmlDoc = XDocument.Load(xmlpath);
            //The Key is root node current Settings
            string xelementName = key;
            var xelementFilepath = XElement.Load(xmlpath);
            var xmlElement = xelementFilepath.Descendants("ListSettings").Descendants("RootLevel").Where(a => Convert.ToInt32(a.Attribute("typeid").Value) == typeid).Select(a => a);
            if (xmlElement != null)
            {
                var xDocparse = JsonConvert.DeserializeXNode(jsondata);
                XElement el = xDocparse.Descendants(key).Elements("EntityType").FirstOrDefault();
                //el.SetAttributeValue("ID", EntityTypeID);

                adminXmlDoc.Descendants("ListSettings").Descendants("RootLevel").Where(a => Convert.ToInt32(a.Attribute("typeid").Value) == typeid).Select(a => a).Descendants(xelementName).Remove();

                //adminXmlDoc.Descendants("ListSettings").Descendants("RootLevel").Where(a => Convert.ToInt32(a.Attribute("typeid").Value) == typeid).FirstOrDefault().Add(xDocparse);
                //elementsof.Add(xDocparse);

                //if (mainroot.Descendants(xelementName).Count() != 0)
                //{
                //    mainroot.Descendants(xelementName).Remove();
                //}

                //adminXmlDoc.Descendants("ListSettings").Descendants("RootLevel").Where(a => Convert.ToInt32(a.Attribute("typeid").Value) == typeid).Elements(xelementName).Remove();

                //adminXmlDoc.Descendants("ListSettings").Descendants("RootLevel").Where(a => Convert.ToInt32(a.Attribute("typeid").Value) == typeid).ElementAt(0).Add(xDocparse);

                XElement subjectElement2 = xDocparse.Descendants(key).FirstOrDefault();
                adminXmlDoc.Descendants("ListSettings").Descendants("RootLevel").Where(a => Convert.ToInt32(a.Attribute("typeid").Value) == typeid).FirstOrDefault().Add(subjectElement2);
                //var RootLevel = adminXmlDoc.Descendants(key).FirstOrDefault();
                //adminXmlDoc.Descendants(xelementName).Remove();
                //RootLevel.Descendants(xelementName).Remove();
                //adminXmlDoc.Save(xmlpath);
                ////RootLevel.Add(subjectElement2.Nodes());
                //var vae = Convert.ToString(xDocparse);
                //adminXmlDoc.Descendants(xelementName).Remove();
                ////                adminXmlDoc.Save(xmlpath);
                //adminXmlDoc.Element("AppSettings").Add(subjectElement2);
                adminXmlDoc.Save(xmlpath);
            }
            else if (xmlElement == null)
            {

                XDocument xDocparse = JsonConvert.DeserializeXNode(jsondata);
                var vae = Convert.ToString(xDocparse);
                adminXmlDoc.Element("AppSettings").Add(xDocparse.Nodes().ElementAt(0));
                adminXmlDoc.Save(xmlpath);
            }

            return true;
        }

        public bool AdminSettingsforReportInsertUpdate(CommonManagerProxy proxy, string jsondata, string key)
        {

            dynamic jsObject = JsonConvert.DeserializeObject(jsondata);
            JProperty jprop = new JProperty(jsondata);
            // string xmlpath = AppDomain.CurrentDomain.BaseDirectory + "\\AdminSettings.xml";
            string xmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
            XDocument adminXmlDoc = XDocument.Load(xmlpath);
            //The Key is root node current Settings
            string xelementName = key;
            var xelementFilepath = XElement.Load(xmlpath);
            var xmlElement = xelementFilepath.Descendants("ReportSettings").Descendants("GanttViewReport").Select(a => a);
            if (xmlElement != null)
            {
                var xDocparse = JsonConvert.DeserializeXNode(jsondata);
                adminXmlDoc.Descendants("ReportSettings").Descendants(xelementName).Remove();

                XElement subjectElement2 = xDocparse.Descendants(key).FirstOrDefault();
                adminXmlDoc.Descendants("ReportSettings").FirstOrDefault().Add(subjectElement2);
                adminXmlDoc.Save(xmlpath);
            }
            else if (xmlElement == null)
            {

                XDocument xDocparse = JsonConvert.DeserializeXNode(jsondata);
                var vae = Convert.ToString(xDocparse);
                adminXmlDoc.Element("AppSettings").Add(xDocparse.Nodes().ElementAt(0));
                adminXmlDoc.Save(xmlpath);
            }

            return true;
        }

        public bool AdminSettingsforGanttViewInsertUpdate(CommonManagerProxy proxy, string jsondata, string key)
        {

            dynamic jsObject = JsonConvert.DeserializeObject(jsondata);
            JProperty jprop = new JProperty(jsondata);
            // string xmlpath = AppDomain.CurrentDomain.BaseDirectory + "\\AdminSettings.xml";
            string xmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
            XDocument adminXmlDoc = XDocument.Load(xmlpath);
            //The Key is root node current Settings
            string xelementName = key;
            var xelementFilepath = XElement.Load(xmlpath);
            var xmlElement = xelementFilepath.Element(xelementName);
            if (xmlElement != null)
            {
                XDocument xDocparse = JsonConvert.DeserializeXNode(jsondata);
                adminXmlDoc.Descendants(xelementName).Descendants("EntityType").Remove();
                XElement subjectElement2 = xDocparse.Descendants(xelementName).FirstOrDefault();
                var RootLevel = adminXmlDoc.Descendants(xelementName).FirstOrDefault();
                RootLevel.Add(subjectElement2.Nodes());
                adminXmlDoc.Descendants(xelementName).Remove();
                adminXmlDoc.Save(xmlpath);

                adminXmlDoc.Element("AppSettings").Add(RootLevel);
                adminXmlDoc.Save(xmlpath);
            }
            else if (xmlElement == null)
            {
                XDocument xDocparse = JsonConvert.DeserializeXNode(jsondata);
                adminXmlDoc.Element("AppSettings").Add(xDocparse.Nodes().ElementAt(0));
                adminXmlDoc.Save(xmlpath);
            }

            return true;
        }

        public bool AdminSettingsforListViewInsertUpdate(CommonManagerProxy proxy, string jsondata, string key)
        {

            dynamic jsObject = JsonConvert.DeserializeObject(jsondata);
            JProperty jprop = new JProperty(jsondata);
            // string xmlpath = AppDomain.CurrentDomain.BaseDirectory + "\\AdminSettings.xml";
            string xmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
            XDocument adminXmlDoc = XDocument.Load(xmlpath);
            //The Key is root node current Settings
            string xelementName = key;
            var xelementFilepath = XElement.Load(xmlpath);
            var xmlElement = xelementFilepath.Element(xelementName);
            if (xmlElement != null)
            {
                XDocument xDocparse = JsonConvert.DeserializeXNode(jsondata);
                adminXmlDoc.Descendants(xelementName).Descendants("EntityType").Remove();
                XElement subjectElement2 = xDocparse.Descendants(xelementName).FirstOrDefault();
                var RootLevel = adminXmlDoc.Descendants(xelementName).FirstOrDefault();
                RootLevel.Add(subjectElement2.Nodes());
                adminXmlDoc.Descendants(xelementName).Remove();
                adminXmlDoc.Save(xmlpath);
                adminXmlDoc.Element("AppSettings").Add(RootLevel);
                adminXmlDoc.Save(xmlpath);
            }
            else if (xmlElement == null)
            {
                XDocument xDocparse = JsonConvert.DeserializeXNode(jsondata);
                adminXmlDoc.Element("AppSettings").Add(xDocparse.Nodes().ElementAt(0));
                adminXmlDoc.Save(xmlpath);
            }

            return true;
        }

        public bool AdminSettingsforRootLevelFilterSettingsInsertUpdate(CommonManagerProxy proxy, string jsondata, string key, int EntityTypeID)
        {

            dynamic jsObject = JsonConvert.DeserializeObject(jsondata);
            JProperty jprop = new JProperty(jsondata);
            // string xmlpath = AppDomain.CurrentDomain.BaseDirectory + "\\AdminSettings.xml";
            string xmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
            XDocument adminXmlDoc = XDocument.Load(xmlpath);
            //The Key is root node current Settings
            string xelementName = key;
            var xelementFilepath = XElement.Load(xmlpath);
            var xmlElement = xelementFilepath.Element(xelementName);
            if (xmlElement != null)
            {
                XDocument xDocparse = JsonConvert.DeserializeXNode(jsondata);
                XElement el = xDocparse.Descendants(key).Elements("EntityType").FirstOrDefault();

                el.SetAttributeValue("ID", EntityTypeID);
                var mainroot = adminXmlDoc.Descendants(key).Descendants("EntityType").Where(a => Convert.ToInt32(a.Attribute("ID").Value) == EntityTypeID).Select(a => a);
                if (mainroot.Count() != 0)
                {
                    mainroot.Remove();

                }
                XElement subjectElement2 = xDocparse.Descendants(key).FirstOrDefault();
                var RootLevel = adminXmlDoc.Descendants(key).FirstOrDefault();
                RootLevel.Add(subjectElement2.Nodes());
                var vae = Convert.ToString(xDocparse);
                adminXmlDoc.Descendants(xelementName).Remove();
                adminXmlDoc.Save(xmlpath);
                adminXmlDoc.Element("AppSettings").Add(RootLevel);
                adminXmlDoc.Save(xmlpath);
            }
            else if (xmlElement == null)
            {

                XDocument xDocparse = JsonConvert.DeserializeXNode(jsondata);
                var vae = Convert.ToString(xDocparse);
                adminXmlDoc.Element("AppSettings").Add(xDocparse.Nodes().ElementAt(0));
                adminXmlDoc.Save(xmlpath);
            }

            return true;
        }

        public bool AdminSettingsforDetailFilterInsertUpdate(CommonManagerProxy proxy, string jsondata, string key)
        {

            dynamic jsObject = JsonConvert.DeserializeObject(jsondata);
            JProperty jprop = new JProperty(jsondata);
            // string xmlpath = AppDomain.CurrentDomain.BaseDirectory + "\\AdminSettings.xml";
            string xmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
            XDocument adminXmlDoc = XDocument.Load(xmlpath);
            //The Key is root node current Settings
            string xelementName = key;
            var xelementFilepath = XElement.Load(xmlpath);
            var xmlElement = xelementFilepath.Element(xelementName);
            if (xmlElement != null)
            {
                XDocument xDocparse = JsonConvert.DeserializeXNode(jsondata);
                // adminXmlDoc.Descendants(xelementName).Descendants("EntityType").Remove();
                XElement subjectElement2 = xDocparse.Descendants(xelementName).FirstOrDefault();
                var RootLevel = adminXmlDoc.Descendants(xelementName).FirstOrDefault();
                adminXmlDoc.Descendants(xelementName).Remove();
                adminXmlDoc.Save(xmlpath);
                adminXmlDoc = XDocument.Load(xmlpath);
                RootLevel = adminXmlDoc.Descendants(xelementName).FirstOrDefault();
                RootLevel = XElement.Parse("<DetailFilter></DetailFilter>");

                RootLevel.Add(subjectElement2.Nodes());


                adminXmlDoc.Element("AppSettings").Add(RootLevel);
                adminXmlDoc.Save(xmlpath);
            }
            else if (xmlElement == null)
            {
                XDocument xDocparse = JsonConvert.DeserializeXNode(jsondata);
                adminXmlDoc.Element("AppSettings").Add(xDocparse.Nodes().ElementAt(0));
                adminXmlDoc.Save(xmlpath);
            }

            return true;
        }

        public bool AdminSettingsForRootLevelDelete(CommonManagerProxy proxy, string key, int EntityTypeID, int AttributeID)
        {
            try
            {
                string xmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
                XDocument adminXmlDoc = XDocument.Load(xmlpath);
                string xelementName = key;
                var xelementFilepath = XElement.Load(xmlpath);
                var xmlElement = xelementFilepath.Element(xelementName);
                if (xmlElement != null)
                {
                    var mainroot = adminXmlDoc.Descendants("ActivityRootLevel").Descendants("EntityType").Where(a => Convert.ToInt32(a.Attribute("ID").Value) == EntityTypeID);
                    //var result = adminXmlDoc.Descendants("ActivityRootLevel").Descendants("EntityType").Where(mid => Convert.ToInt32(mid.Element("ID").Value) == EntityTypeID).Select(m => m);
                    var result = mainroot.Elements("Attribute").Where(a => Convert.ToInt32(a.Element("Id").Value) == AttributeID);
                    //adminXmlDoc.Descendants("ActivityRootLevel").Descendants("Attribute").Remove();
                    foreach (XElement item in result)
                    {
                        item.Remove();
                    }
                    adminXmlDoc.Save(xmlpath);
                    var afterresult = mainroot.Elements("Attribute");
                    if (afterresult.Count() == 0)
                    {
                        afterresult.Remove();
                    }
                }
                return true;
            }
            catch
            {

                return false;
            }

        }
        public bool AdminSettingsForRootLevelDelete(CommonManagerProxy proxy, string key, int EntityTypeID)
        {
            try
            {
                string xmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
                XDocument adminXmlDoc = XDocument.Load(xmlpath);
                string xelementName = key;
                var xelementFilepath = XElement.Load(xmlpath);
                var xmlElement = xelementFilepath.Element(xelementName);
                if (xmlElement != null)
                {
                    var mainroot = adminXmlDoc.Descendants("ActivityRootLevel").Descendants("EntityType").Where(a => Convert.ToInt32(a.Attribute("ID").Value) == EntityTypeID);
                    //var result = adminXmlDoc.Descendants("ActivityRootLevel").Descendants("EntityType").Where(mid => Convert.ToInt32(mid.Element("ID").Value) == EntityTypeID).Select(m => m);
                    mainroot.Remove();
                    //adminXmlDoc.Descendants("ActivityRootLevel").Descendants("Attribute").Remove();
                    //foreach (XElement item in mainroot)
                    //{
                    //    item.Remove();
                    //}
                    adminXmlDoc.Save(xmlpath);
                }
                return true;
            }
            catch
            {

                return false;
            }

        }

        public string GetAdminSettings(CommonManagerProxy proxy, string LogoSettings, int typeid)
        {
            if (typeid != 0)
            {
                // string xmlpath =  AppDomain.CurrentDomain.BaseDirectory +"\\AdminSettings.xml";
                string xmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
                XDocument adminXdoc = XDocument.Load(xmlpath);
                var result = adminXdoc.Descendants("ListSettings").Descendants("RootLevel").Select(a => a).ToList().Where(a => Convert.ToInt32(a.Attribute("typeid").Value) == typeid).Select(a => a);
                var abc = result.Descendants(LogoSettings).ToList();
                var xElementResult = result;
                string jsonText = JsonConvert.SerializeObject(abc[0]);
                return jsonText;
            }
            else
            {
                string xmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
                XDocument adminXdoc = XDocument.Load(xmlpath);
                var result = adminXdoc.Descendants(LogoSettings).Select(a => a).ToList();
                var xElementResult = result[0];
                string jsonText = JsonConvert.SerializeObject(result[0]);
                return jsonText;
            }

        }

        public string GetAdminSettingselemntnode(CommonManagerProxy proxy, string LogoSettings, string elemntnode, int typeid)
        {
            //if (typeid != 0)
            //{
            //    // string xmlpath =  AppDomain.CurrentDomain.BaseDirectory +"\\AdminSettings.xml";
            //    string xmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
            //    XDocument adminXdoc = XDocument.Load(xmlpath);
            //    var result = adminXdoc.Descendants("ListSettings").Descendants("RootLevel").Select(a => a).ToList().Where(a => Convert.ToInt32(a.Attribute("typeid").Value) == typeid).Select(a => a);
            //    var abc = result.Descendants(LogoSettings).ToList();
            //    var xElementResult = result;
            //    string jsonText = JsonConvert.SerializeObject(abc[0]);
            //    return jsonText;
            //}
            if (LogoSettings != null && elemntnode != null)
            {
                string xmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
                XDocument adminXdoc = XDocument.Load(xmlpath);
                var result = adminXdoc.Descendants(LogoSettings).Descendants(elemntnode).Select(a => a).ToList();
                var xElementResult = result[0];
                string jsonText = JsonConvert.SerializeObject(result[0]);
                return jsonText;
            }
            return "";

        }


        /// <summary>
        /// Get Navigation ExternalLink.
        /// </summary>
        /// <param name="proxy">The Typeid.</param>
        /// <param name="proxy">The proxy.</param>
        /// <returns>IList<INavigation></returns>
        public string GetNavigationExternalLinksByID(CommonManagerProxy proxy, int ID)
        {
            try
            {

                IList<INavigation> navlist = new List<INavigation>();
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    ///to get value from database
                    var lst = tx.PersistenceManager.CommonRepository.GetEquals<NavigationDao>(NavigationDao.PropertyNames.Id, ID);
                    tx.Commit();


                    if (lst != null && lst.Count() > 0)
                    {
                        return lst.ElementAt(0).ExternalUrl;

                    }
                }
            }


            catch
            {

            }

            return null;
        }


        private void SaveImage(string data)
        {
            //string data = @"data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAGAAAAA8CAYAAACZ1L+0AAAABHNCSVQICAgIfAhkiAAAABl0RVh0U29mdHdhcmUAd3d3Lmlua3NjYXBlLm9yZ5vuPBoAAAuhSURBVHic7Zx5kBT1Fcc/r2f2EhZQDq9IvBADiRoGROWaBXcWTCokhaIVb4scRaQUhlJMorCgUiizSoyliWKZMjGR9UghCswSaQVEgQZEJAoiQiJqonJ44B7TL3/0zO7M7Bw7uz0Dhv1WTc30r1+/95vf6/f7vd97r1tUlaMRaklfoB+wRnz69eHqhxytCgBQS7oBU4DuwCPi0x2F7sNRrYAY1JLBwNPRzyzx6ReFkm0UStCRDPHpBmAYMBp4Wy25rFCyC6uANVLONikuqMw2Qnz6ATAC2AAsUkuWqiU98y23cArYJsV2KTMZQFPBZOYI8emXwATgBWAs8LpacnY+ZRZIASIcYpEBD4HahZHZPohPI8BE4HXgDOA1taQyX/IKo4CNLMRgOT7dWRB5HYT49Cvgh8AOHA/pRbXk+rzIyrcXZFtyuyEMZJBekVdBeYBa8h1gI1AKRIDx4tMX3JSRXwvYJDeIMB7lhrzKyRPEp/8EZkUPPcBTaonPTRn5U8Aq6a02t4tNCMekv6mYD6yP/u4CLFFLvu0W8/xNQRtlocJZMkhH5EdA4aCWDAQ2AUXRps3AEPFphz26/FjAOrlQlQmiPNkm+k0ymPVyUV764gLEp28Bj8c1nQcE3eCdFwWoh1nATt7jj1mJN0s/O8Ikhuir+eiLi5gLCXuYmWrJ6R1l6r4CLJkEjFGo5TKNZKRdJz2x+ZMhTHO9Hy5DfLoL+HNcUxnwcEf5uquAd6VE4SaEd4zPuT8j7TYpVg9/B279Bi3SdwPxG8lKteQnHWHoqgIiB7ga+K7AKvxZYuyHmK3KOwzSVW72IZ+IhqvNpOapHeHpqgJEGQ0QsZvdttTYIqcpTDRs7nFTfoFQm3Q8Qi05t73M3FPAu1IiwlCUjz3C0xlpm5grwmrO1+1Z+R550dPnSJyGAG5sLzP3FLCficDpwFZ8eiAt3Wa5RG0qGyM8kJWnJUUcYgaIuNbPDkJ8+jHwSlLzlWrJce3h554ChDEAYrAlE5na3IjB2qIhmnmaQgThiUMNLIQjLm33fNJxGTCuPYzcUcA2KVa4AFBgZVq69XICygWibMzK0+JelDVlF+oHrvTRXaS6efztYeTtWD+i+IqxCP1R/gUsS0dmCzcIlKMsychvq5yiwkgZxFBX+uc+NuGsA/E38Kj2MHLHApTTor8+xaeN6cjEYDiwncG6LiO/Bu4R4YkjcOoBIJq0T3Yg+qklJ+XKyx0FGPSKfu9LS7NF+qAMFcm8RrBWTlZlCCX8wZW+5Q9WiracrcCtRdhJXivpvZ9GJgDHAW9n5FTEdcAWBmiDS33LF95N0dYvVyauKECjFqCawQKgN4CtfJaRl3CROOHeIx37U7T1zpWJOxZgOwowJKMCekZp3k9LUSse4PvAa670K79IpYA+uTJxxwtSeiNkXANs6CkQQUlf/ncWJ9BENyIZaFJhs/QgwrXAbnwsLlDlhSsKcMECRDA4FgCbgxmoeuF0+sN0NE0NnAk08lV6mlScNcJ6hfsVnrOtgsWXjhQFqKI4C6bQNT0ZPRC+yBSmEDgN4UDWSGo8NuEDzozjUajqi1RWVpSiLSPc8oI+j34fm5ZCiKB4o/N8SngM9qMU5xT7KWEL8J/YoUJdm6/tGFLdbDkX9bqzBsQUoOkVILBTlSZOpwRInYBpYjsedrGWUi7kUJskD9AG2SQVts0UA3ZLccH2D+XR7y+BPThjkHmDmQKuVEXoBlmKMBblWRmsEzrM8BsAtWQccDawUHyadu3LhmYLCITMcuB4nFK8LqSfnhqA3cDecNCvAAr7BEASLaBy3oq+eLytEtdNX7J65Ux/E0BV6KWRthrtmgpF2e8tPfReY33ZoJZGmuqC/tXV1dXG6i6jRiZfYxh2w/JpozMWAIy9f9WJkaZI/1TnPJ76LcumVn0mPl0KLA2ETA+m2Q/HIrqSftyacKao/eGg//1YozcQMj3AQ8C1QC7JjzcDIfPScNC/3fCwI+r49YgnEG9RLej5yRcWd2ESsBBAMcIilOQgNx4vNzaWzRBJiMAeAHqYjCouktaRWVWDqpqXhmVSgm1HHhQhZa63iZJxwLJAyPQCVwO3keMOOBAyXwPuDgf9zxtRBj8jt8EH+B6wIRAyuzUpsT/TPXaycv7KH6QafAA15I5LHlja3kHvMGw17kx3bux95pmojG8DmyDwGO0IP+CE7hcHQmalAbQy0xxQDgz1lrIS2KvxmSLDmJ32KtW+jQ3H/LwDcjsEgYqxNS9XpDqnEZ0GmnFKDITMEuAmF7oyyQuck9T4DPAgtPJCPFHa35M4z53CAG3AkncMm9sAqkLmjwVa5mXEVrRW4PLmFvQ3P6pestDodszISNIaYNgMVOHRFlo+slNMCUrkoODp1vb/K3ZscG10DjA8/uzFc//R0yj2XJd0UROtvcWLgBOT2l7HKeQ9gJOYiocXZ8GeT9wsAYz20nrRWBAO+tOViqwJhMyTidv44CzICFzJEP1IQAJIdWIfdFFJo3dyQ1FkHGhswI7/ukvXKeGp/nnJQiprTCTucoX6umn+lPGhyhrzgjR9TQFdRGyjpgy7+D5z7Iqp/uYEklHinYxqWQu9vKpoT4HkBTlZ6QeB4eGgP1Ot6OpAyNwHCQULXb3ANhLj2H8LhMwncXz1ehyvJ/apx4lUmsDOcNC/q/kqn34IEAiZEzTRqtQw9M4lM4bvC8xfuQCR21v+n9xSOW/Fw3W3Xpw+jO0mbOZhcCnRO9qIMIdoBq+i2iwt6ioJ1Q2KPRtkQQpOpUnHH2UZ/BiSkzilBq0jjycB04E7gLuAe4EFOJ7SYzh1MXXAe4GQuTwQMpt3hNXV1Ya21NPH8MyyqRVvATR6pQbicwZ6nHg8rhS5tgWNRbxPfHmhMLhy/srxAN4ucjVoXCxH1tUFK5anYZW8U2/bprElYtAMA2fAniJ1bCMbAjhKAmBNV//lwMC482qINnscK2/27xdNLFlUkZsrQmavdshuF2yJzHXWAgeGGLMn1tZ6RDShPlXVTu9EuAhvOOj/GrgiEDJ/BfTF2Yx1xXFLi6LfxThmVw5cSeIaMAhgYm2tR+k9M+nW+MxWuT4QMltaJGERQqC8CGbgWF3esWLamO2VIbPZIVD0nAO7+zyGaPzTkFbd9IpMjyLVJx13T0nVGskJG9sbCJlPQcJGaGY46H8jHYdAyNyMUx0WQ3+A/Xv6/FTQ5MWqJ21z1yYH7qmrCd9SubcNtB2HYdyFbU8kOpWo6DXxp1V1ThYOyVm9EwIh81vhoP/fWa4blnRc78UpKCqPazw1EDJfAFJVN3SBVu7gropq01vUlTuyCM+EMjG8vwUmd4BHm1E3deSbVTXmYlVSbbjeWDG9YnEWFrtw3LyYwZcCWwMh83HSu6FnAclP4H8S84Li62/OjX7aijXF5XqNqsRPSxHQX6tK2sS6iJ4DLY9+qsikqvmv3Lt8+shd6a5xExGVuwy0lQJUdI62HsAEhIP+PYGQGQaq4pq7k/vm7K9e4Hc4j9/knEwA9kZEHvEoLyY266JwsCJjZuqSB5aWNDUeMwbVvtGmIhV7JnBdO/qRM1YER60P1LwcRjUQ17x1xbSKZ9vIogYnilCWjTANPgUeNcJB/5M4sQkT+CTLRQdxyjHWANXAUK/aI4BT42hUDc/cbNJfnDKuXmxN9jSuqgqZeX01QDyMCAkxIRHuzHb3xxAO+sM4Tsss4C2cpFCmvUA98AGwFif2dko46N/R+bqaw4zO19UcZriVkvy/hFoyCLglemgDM91+q1anAtJALemPEyfqjTO3X5WPV5p1KiAF1JJvAWGcwa8HJopPs+0N2oXONSAJakkvnGBjX5xqh9H5GnzoVEAC1JJyYClO8uQ54Dzx5fcJ/s4pKIroG1D+gvOg4S/FpwWpL+q0AEAt+QXOc1+vAmcUavDhKLeA6Ntza4D/AoPFp3sK3YejdieslgzAmeuXyWF8V8X/AGryz36xXfJpAAAAAElFTkSuQmCC";

            System.Drawing.Image newImage;
            var base64Data = Regex.Match(data, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
            var binData = Convert.FromBase64String(base64Data);

            using (var stream = new MemoryStream(binData))
            {
                newImage = System.Drawing.Image.FromStream(stream);



                string path = Path.Combine(HttpRuntime.AppDomainAppPath, "assets\\img");
                // System.IO.File.Delete(HttpContext.Current.Server.MapPath("\\images") + "\\logo.png");
                System.IO.File.Delete(path + "\\logo.png");
                //Mention path here where the image to save
                //mention here name or if you store name in that another field then just use it here that field value
                //newImage.Save(HttpContext.Current.Server.MapPath("\\images") + "\\logo.png");
                newImage.Save(path + "\\logo.png", System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            newImage.Dispose();
        }

        /// <summary>
        /// InsertFile.
        /// </summary>
        /// <param name="proxy">file Parameter</param>
        /// <returns>int</returns>
        public int InsertFile(CommonManagerProxy proxy, string Name, int VersionNo, string MimeType, string Extension, long Size, int OwnerID, DateTime CreatedOn, string Checksum, int ModuleID, int EntityID, String FileGuid, string Description, bool IsPlanEntity = false)
        {
            try
            {
                if (IsPlanEntity)
                    proxy.MarcomManager.AccessManager.TryEntityTypeAccess(EntityID, Modules.Planning);
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    Guid NewId = Guid.NewGuid();

                    string filePath = ReadAdminXML("FileManagment");
                    var DirInfo = System.IO.Directory.GetParent(filePath);
                    string newFilePath = DirInfo.FullName;
                    System.IO.File.Move(filePath + "\\" + FileGuid + Extension, newFilePath + "\\" + NewId + Extension);
                    FileDao fldao = new FileDao();
                    fldao.Checksum = Checksum;
                    fldao.CreatedOn = CreatedOn;
                    fldao.Entityid = EntityID;
                    fldao.Extension = Extension;
                    fldao.MimeType = MimeType;
                    fldao.Moduleid = ModuleID;
                    fldao.Name = Name;
                    fldao.Ownerid = OwnerID;
                    fldao.Size = Size;
                    fldao.VersionNo = VersionNo;
                    fldao.Description = HttpUtility.HtmlEncode(Description);
                    fldao.Fileguid = NewId;
                    tx.PersistenceManager.CommonRepository.Save<FileDao>(fldao);
                    AttachmentsDao attachdao = new AttachmentsDao();
                    attachdao.ActiveFileid = fldao.Id;
                    attachdao.ActiveVersionNo = fldao.VersionNo;
                    attachdao.Createdon = CreatedOn;
                    attachdao.Entityid = EntityID;
                    attachdao.ActiveFileVersionID = fldao.Id;
                    attachdao.VersioningFileId = fldao.Id;
                    attachdao.Name = Name;
                    attachdao.Typeid = 4;
                    tx.PersistenceManager.CommonRepository.Save<AttachmentsDao>(attachdao);

                    tx.Commit();

                    //Removing from the Search Engine
                    BrandSystems.Marcom.Core.Interface.Managers.IEventManager _eventManager = null;
                    BrandSystems.Marcom.Core.Interface.Managers.IPluginManager _pluginManager = null;
                    MarcomManager marcommanager = new MarcomManager(_eventManager, _pluginManager);
                    BrandSystems.Marcom.Core.Managers.Proxy.PlanningManagerProxy pProxy = new BrandSystems.Marcom.Core.Managers.Proxy.PlanningManagerProxy(marcommanager);
                    System.Threading.Tasks.Task addtaskforsearch = new System.Threading.Tasks.Task(() => PlanningManager.Instance.AddEntityAsync(pProxy, Name, fldao.Id, EntityID));
                    addtaskforsearch.Start();

                    FeedNotificationServer fs = new FeedNotificationServer();
                    NotificationFeedObjects obj = new NotificationFeedObjects();
                    obj.action = "attachment added";
                    obj.attachmenttype = "file";
                    obj.AttributeName = Name;
                    obj.Actorid = proxy.MarcomManager.User.Id;
                    obj.ToValue = Convert.ToString(attachdao.Id);
                    obj.EntityId = EntityID;
                    fs.AsynchronousNotify(obj);
                    return fldao.Id;

                }
            }
            catch (MarcomAccessDeniedException ex)
            {
                throw ex;
            }
            catch
            {

            }

            return 0;
        }




        /// <summary>
        /// InsertLink.
        /// </summary>
        /// <param name="proxy">file Parameter</param>
        /// <returns>int</returns>
        public int InsertLink(CommonManagerProxy proxy, int EntityID, string Name, string URL, string Description, int ActiveVersionNo, int TypeID, string CreatedOn, int OwnerID, int ModuleID)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    Guid NewId = Guid.NewGuid();
                    LinksDao lnkdao = new LinksDao();
                    lnkdao.EntityID = EntityID;
                    lnkdao.Name = HttpUtility.HtmlEncode(Name);
                    lnkdao.URL = HttpUtility.HtmlEncode((URL.Substring(0, 7).ToString() == "http://") ? URL : "http://" + URL);
                    lnkdao.ActiveVersionNo = ActiveVersionNo;
                    lnkdao.TypeID = TypeID;
                    lnkdao.CreatedOn = CreatedOn;
                    lnkdao.OwnerID = OwnerID;
                    lnkdao.ModuleID = ModuleID;
                    lnkdao.LinkGuid = NewId;
                    lnkdao.Description = HttpUtility.HtmlEncode(Description);
                    tx.PersistenceManager.CommonRepository.Save<LinksDao>(lnkdao);
                    tx.Commit();
                    FeedNotificationServer fs = new FeedNotificationServer();
                    NotificationFeedObjects obj = new NotificationFeedObjects();
                    obj.action = "attachment added";
                    obj.AttributeName = Name;
                    obj.attachmenttype = "link";
                    obj.Actorid = proxy.MarcomManager.User.Id;
                    obj.ToValue = lnkdao.ID.ToString();
                    obj.EntityId = EntityID;
                    fs.AsynchronousNotify(obj);
                    return lnkdao.ID;
                }
            }
            catch
            {


            }
            return 0;
        }


        /// <summary>
        /// DeleteFileByID.
        /// </summary>
        /// <param name="proxy">ID Parameter</param>
        /// <returns>bool</returns>
        public bool DeleteFileByID(CommonManagerProxy proxy, int ID)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    FileDao fldao = new FileDao();
                    fldao = tx.PersistenceManager.PlanningRepository.Get<FileDao>(FileDao.MappingNames.Id, ID);
                    string flPath = ReadAdminXML("FileManagment");//HttpContext.Current.Server.MapPath("~/documents/" + fldao.Fileguid + fldao.Extension);
                    var DirInfo = System.IO.Directory.GetParent(flPath);
                    string newFilePath = DirInfo.FullName + "\\" + fldao.Fileguid + fldao.Extension;
                    if (System.IO.File.Exists(newFilePath))
                    {
                        System.IO.File.Delete(newFilePath);
                        FeedNotificationServer fs = new FeedNotificationServer();
                        NotificationFeedObjects obj = new NotificationFeedObjects();
                        obj.action = "attachment deleted";
                        obj.attachmenttype = "file";
                        obj.AttributeName = fldao.Name;
                        obj.Actorid = proxy.MarcomManager.User.Id;
                        obj.ToValue = fldao.Id.ToString();
                        obj.EntityId = fldao.Entityid;

                        tx.PersistenceManager.PlanningRepository.DeleteByID<AttachmentsDao>(AttachmentsDao.MappingNames.ActiveFileid, ID);
                        tx.PersistenceManager.CommonRepository.DeleteByID<FileDao>(FileDao.MappingNames.Id, ID);


                        tx.Commit();

                        //Removing from the Search Engine
                        BrandSystems.Marcom.Core.Interface.Managers.IEventManager _eventManager = null;
                        BrandSystems.Marcom.Core.Interface.Managers.IPluginManager _pluginManager = null;
                        MarcomManager marcommanager = new MarcomManager(_eventManager, _pluginManager);
                        BrandSystems.Marcom.Core.Managers.Proxy.PlanningManagerProxy pProxy = new BrandSystems.Marcom.Core.Managers.Proxy.PlanningManagerProxy(marcommanager);
                        System.Threading.Tasks.Task addtaskforsearch = new System.Threading.Tasks.Task(() => PlanningManager.Instance.RemoveEntityAsync(pProxy, ID));
                        addtaskforsearch.Start();

                        fs.AsynchronousNotify(obj);
                        return true;
                    }

                }
            }
            catch
            {

            }

            return false;
        }


        /// <summary>
        /// DeleteLinkByID.
        /// </summary>
        /// <param name="proxy">ID Parameter</param>
        /// <returns>bool</returns>
        public bool DeleteLinkByID(CommonManagerProxy proxy, int ID)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    LinksDao fldao = new LinksDao();
                    fldao = tx.PersistenceManager.PlanningRepository.Get<LinksDao>(LinksDao.MappingNames.ID, ID);


                    FeedNotificationServer fs = new FeedNotificationServer();
                    NotificationFeedObjects obj = new NotificationFeedObjects();
                    obj.action = "attachment deleted";
                    obj.attachmenttype = "link";
                    obj.AttributeName = fldao.Name;
                    obj.Actorid = proxy.MarcomManager.User.Id;
                    obj.ToValue = fldao.ID.ToString();
                    obj.EntityId = fldao.EntityID;

                    // tx.PersistenceManager.PlanningRepository.DeleteByID<AttachmentsDao>(AttachmentsDao.MappingNames.ActiveFileid, ID);
                    //  tx.PersistenceManager.CommonRepository.DeleteByID<FileDao>(FileDao.MappingNames.Id, ID);
                    //tx.PersistenceManager.PlanningRepository.DeleteByID<LinksDao>(LinksDao.MappingNames.ID, ID);
                    tx.PersistenceManager.CommonRepository.DeleteByID<LinksDao>(LinksDao.MappingNames.ID, ID);
                    tx.Commit();
                    fs.AsynchronousNotify(obj);
                    return true;
                }
            }
            catch
            {

            }
            return false;
        }



        private string ReadAdminXML(string elementNode)
        {

            string xmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
            XDocument xDoc = XDocument.Load(xmlpath);
            if (xDoc.Root.Elements(elementNode).Count() > 0)
            {
                return (string)xDoc.Root.Elements(elementNode).FirstOrDefault().Elements("FolderPath").First();
            }
            string uploadImagePath = Path.Combine(HttpRuntime.AppDomainAppPath);
            return uploadImagePath = uploadImagePath + "UploadedImages\\";
        }

        /// <summary>
        /// Get File By  Entity ID.
        /// </summary>
        /// <param name="proxy">EntityID</param>
        /// <returns>int</returns>
        public IList<IFile> GetFileByEntityID(CommonManagerProxy proxy, int EntityID)
        {
            IList<IFile> flDetails = new List<IFile>();
            try
            {

                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {

                    var flDaoList = tx.PersistenceManager.CommonRepository.GetEquals<FileDao>(FileDao.MappingNames.Entityid, EntityID);

                    foreach (var val in flDaoList)
                    {
                        BrandSystems.Marcom.Core.Common.File fldao = new BrandSystems.Marcom.Core.Common.File();
                        fldao.Checksum = val.Checksum;
                        fldao.CreatedOn = val.CreatedOn;
                        fldao.Entityid = val.Entityid;
                        fldao.Extension = val.Extension;
                        fldao.MimeType = val.MimeType;
                        fldao.Moduleid = val.Moduleid;
                        fldao.Name = val.Name;
                        fldao.Ownerid = val.Ownerid;
                        fldao.Size = val.Size;
                        fldao.VersionNo = val.VersionNo;
                        fldao.Fileguid = val.Fileguid;
                        fldao.Id = val.Id;
                        var userDao = tx.PersistenceManager.CommonRepository.Get<UserDao>(UserDao.MappingNames.Id, val.Ownerid);
                        fldao.OwnerName = null;
                        if (userDao != null)
                            fldao.OwnerName = userDao.FirstName + " " + userDao.LastName;
                        flDetails.Add(fldao);
                    }

                    tx.Commit();

                    return flDetails;

                }
            }


            catch
            {

            }

            return null;
        }

        /// <summary>
        /// Get Links By  Entity ID.
        /// </summary>
        /// <param name="proxy">EntityID</param>
        /// <returns>int</returns>

        public IList<IFile> GetFilesandLinksByEntityID(CommonManagerProxy proxy, int EntityID)
        {
            IList<IFile> fileandAttachList = new List<IFile>();
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    var AttachmentsAndLinksSelectQuery = new StringBuilder();
                    if (EntityID != 0)
                    {
                        AttachmentsAndLinksSelectQuery.Append("select TMF.ID,TMTA.EntityID,TMF.Checksum,Tmf.CreatedOn,tmf.Extension,tmf.FileGuid,TMF.MimeType,tmf.ModuleID,TMF.Name,tmf.OwnerID,tmf.Size,tmf.VersionNo,'' as 'URL' , TMF.Description,TMTA.VersioningFileId,TMTA.ActiveFileVersionID from CM_File TMF inner join PM_Attachments TMTA on TMTA.ActiveFileVersionID = tmf.ID and TMTA.ActiveVersionNo = TMF.VersionNo where TMTA.EntityID = ? union select TML.ID, TML.EntityID, '' as 'Checksum' , tml.CreatedOn, 'Link' as 'Extension' ,TML.LinkGuid as 'FileGuid', '-' as 'MimeType',tml.ModuleID, tml.Name, tml.OwnerID,'0' as 'Size' ,tml.ActiveVersionNo as 'VersionNo' , tml.URL ,  TML.DESCRIPTION as 'Description', 0 AS VersioningFileId,0 AS ActiveFileVersionID from CM_Links TML where tml.EntityID = ?  order by CreatedOn DESC");
                    }
                    var Result = ((tx.PersistenceManager.CommonRepository.ExecuteQuerywithMinParam(AttachmentsAndLinksSelectQuery.ToString(), EntityID, EntityID)).Cast<Hashtable>().ToList());

                    foreach (var obj in Result)
                    {
                        BrandSystems.Marcom.Core.Common.File fldao = new BrandSystems.Marcom.Core.Common.File();
                        fldao.Id = Convert.ToInt32(obj["ID"]);
                        fldao.Name = Convert.ToString(obj["Name"]);
                        fldao.VersionNo = Convert.ToInt32(obj["VersionNo"]);
                        fldao.MimeType = Convert.ToString(obj["MimeType"]);
                        fldao.Ownerid = Convert.ToInt32(obj["OwnerID"]);
                        fldao.CreatedOn = DateTimeOffset.Parse(obj["CreatedOn"].ToString());
                        fldao.Moduleid = Convert.ToInt32(obj["ModuleID"]);
                        fldao.Size = Convert.ToInt32(obj["Size"]);
                        fldao.Extension = Convert.ToString(obj["Extension"]);
                        fldao.Entityid = Convert.ToInt32(obj["EntityID"]);
                        fldao.LinkURL = Convert.ToString(obj["URL"]);
                        fldao.Fileguid = (Guid)(obj["FileGuid"]);
                        fldao.VersioningFileId = Convert.ToInt32(obj["VersioningFileId"]);
                        fldao.ActiveFileVersionID = Convert.ToInt32(obj["ActiveFileVersionID"]);
                        fldao.Description = Convert.ToString(obj["Description"]);
                        var userDao = tx.PersistenceManager.CommonRepository.Get<UserDao>(UserDao.MappingNames.Id, fldao.Ownerid);
                        fldao.OwnerName = null;
                        if (userDao != null)
                            fldao.OwnerName = userDao.FirstName + " " + userDao.LastName;
                        fileandAttachList.Add(fldao);
                    }
                    tx.Commit();
                    return fileandAttachList;
                }
            }
            catch
            {

            }
            return null;
        }


        /// <summary>
        /// Getting Last inserted Feed by FeedId
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="FeedId">The FeedID</param>
        /// <returns>IFeed</returns>
        private string SelectFeedById(CommonManagerProxy proxy, int feedId)
        {
            try
            {
                string feedHtml = "";
                IFeed feedObj = new Feed();
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    FeedDao feedDaoObj = tx.PersistenceManager.CommonRepository.Get<FeedDao>(feedId);
                    feedObj.Id = feedDaoObj.Id;
                    feedObj.Actor = feedDaoObj.Actor;
                    feedObj.Templateid = feedDaoObj.Templateid;
                    feedObj.HappenedOn = feedDaoObj.HappenedOn;
                    feedObj.CommentedUpdatedOn = feedDaoObj.CommentedUpdatedOn;
                    feedObj.Entityid = feedDaoObj.Entityid;
                    feedObj.TypeName = feedDaoObj.TypeName;
                    feedObj.AttributeName = feedDaoObj.AttributeName;
                    feedObj.FromValue = feedDaoObj.FromValue;
                    feedObj.ToValue = feedDaoObj.ToValue;
                    string userName = tx.PersistenceManager.CommonRepository.Query<Marcom.Dal.User.Model.UserDao>().Where(a =>
                                    a.Id == Convert.ToInt32(feedObj.Actor)).Select(a => a.FirstName + " " + a.LastName).ToString();
                    string templateName = tx.PersistenceManager.CommonRepository.Query<FeedTemplateDao>().Where(a =>
                                                   a.Id == Convert.ToInt32(feedObj.Templateid)).Select(a => a.Template).ToString();
                    string EntityName = "";
                    templateName = "'" + userName + "'<a href='" + EntityName + "'>";
                    feedHtml = feedHtml + userName + templateName;
                    return feedHtml;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetEntityIdsForFeed(CommonManagerProxy proxy, int entityId)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    string entityIds = null;
                    var entityTypeInfo = (from item in tx.PersistenceManager.PlanningRepository.Query<EntityDao>() where item.Id == entityId select item.Typeid).FirstOrDefault();
                    //if (entityTypeInfo == 10)        //to fetch all entity ids that satisfy objective fulfilment and listed out in objective tree 
                    //{

                    //    entityIds = entityId.ToString();
                    //    entityIds = entityIds + "," + string.Join(",", (from item in tx.PersistenceManager.PlanningRepository.Query<ObjectiveEntityValueDao>()
                    //                                                    join item1 in tx.PersistenceManager.PlanningRepository.Query<EntityDao>()
                    //                                                    on item.Entityid equals item1.Id
                    //                                                    where item.Objectiveid == entityId && item1.Active == true
                    //                                                    select item.Entityid).ToArray());
                    //    return entityIds;
                    //}
                    //else if (entityTypeInfo == 5)  //to fetch all entity ids that have relations with the costcennters   and listed out in costcentre tree 
                    //{
                    //    entityIds = entityId.ToString();
                    //    entityIds = entityIds + "," + string.Join(",", (from item in tx.PersistenceManager.PlanningRepository.Query<EntityCostReleationsDao>()
                    //                                                    join item1 in tx.PersistenceManager.PlanningRepository.Query<EntityDao>()
                    //                                                    on item.EntityId equals item1.Id
                    //                                                    where item.CostcenterId == entityId && item1.Active == true
                    //                                                    select item.EntityId).ToArray());
                    //    return entityIds;
                    //}
                    //else
                    //{

                    return entityId.ToString();
                    //}
                }
            }
            catch (Exception ex)
            {
            }
            return null;
        }
        /// <summary>
        /// Getting Feeds By EntityID
        /// </summary>
        /// <param name="proxy"></param>
        /// <param name="entityId"></param>
        /// <param name="pageNo">If it is real time update, page number is not applicable (-1) </param>
        /// <param name="isForRealTimeUpdate"></param>
        /// <returns></returns>
        public IList<IFeedSelection> GettingFeedsByEntityID(CommonManagerProxy proxy, string entityId, int pageNo, bool isForRealTimeUpdate, int entityIdForReference, int newsfeedid = 0, string newsfeedgroupid = "")
        {

            try
            {
                if (proxy.MarcomManager.UserManager.OverviewFeedLock)
                    return new List<IFeedSelection>();

                proxy.MarcomManager.UserManager.OverviewFeedLock = true;
                //System.Threading.Thread.Sleep(6000);
                if (pageNo == 0)
                {
                    proxy.MarcomManager.UserManager.FeedInitialRequestedTime = DateTimeOffset.UtcNow;
                    proxy.MarcomManager.UserManager.FeedRecentlyUpdatedTime = DateTimeOffset.UtcNow;
                }
                if (pageNo > 0)
                {
                    pageNo = pageNo * 10;
                }
                IList<IFeedSelection> listfeedselection = new List<IFeedSelection>();
                IList<MultiProperty> childparLIST = new List<MultiProperty>();
                string[] qryUniquekeys = new string[1000];
                string[] userfeedSelection = new string[1000];
                using (ITransaction txuniquekey = proxy.MarcomManager.GetTransaction())
                {
                    qryUniquekeys = txuniquekey.PersistenceManager.PlanningRepository.Query<EntityDao>().Where(a => entityId.Split(',').Contains(a.Id.ToString())).Select(a => a.UniqueKey).ToArray();
                    userfeedSelection = (from user in txuniquekey.PersistenceManager.UserRepository.Query<UserDao>().ToList() where user.Id == proxy.MarcomManager.User.Id select user.FeedSelection).ToArray();

                    txuniquekey.Commit();
                }
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    var feedSelectQuery = new StringBuilder();
                    //newsfeedgroupid = "1,2";
                    string newFeedIdsformgroup = "";
                    if (newsfeedgroupid.Length > 0 && newsfeedgroupid != "-1" && newsfeedgroupid != "0")
                    {
                        IList<MultiProperty> feedgroupLIST = new List<MultiProperty>();
                        feedgroupLIST.Add(new MultiProperty { propertyName = "Id", propertyValue = newsfeedgroupid });
                        var sqlquery = new StringBuilder();
                        sqlquery.Append("DECLARE @Template VARCHAR(8000) ");
                        sqlquery.Append(" SELECT @Template = COALESCE(@Template + ', ', '') + TEMPLATE FROM [CM_FeedFilter_Group] WHERE id IN(" + newsfeedgroupid + ") ");
                        sqlquery.Append(" SELECT @Template AS Template ");

                        var newsfeedtempldid = ((tx.PersistenceManager.CommonRepository.ExecuteQuery(sqlquery.ToString()).Cast<Hashtable>().ToList()));


                        if (newsfeedtempldid.Count > 0)
                        {
                            newFeedIdsformgroup = newsfeedtempldid.Cast<Hashtable>().Select(a => (string)a["Template"]).FirstOrDefault();
                        }
                    }

                    else
                    {
                        string noacessnewsfeedgroupid = "";
                        bool blnAttachmentsView = false;
                        bool blnAttachmentEdit = false;
                        bool blnObjectives = false;
                        bool blnTask = false;
                        bool blnFinancial = false;

                        blnFinancial = proxy.MarcomManager.AccessManager.CheckUserAccess((int)Modules.Planning, (int)FeatureID.Financials);
                        blnTask = proxy.MarcomManager.AccessManager.CheckUserAccess((int)Modules.Planning, (int)FeatureID.Task);
                        blnObjectives = proxy.MarcomManager.AccessManager.CheckUserAccess((int)Modules.Planning, (int)FeatureID.Objective);
                        blnAttachmentsView = proxy.MarcomManager.AccessManager.CheckUserAccess((int)Modules.Planning, (int)FeatureID.Attachments);
                        blnAttachmentEdit = proxy.MarcomManager.AccessManager.CheckUserAccess((int)Modules.Planning, (int)FeatureID.AttchmentEdit);


                        if (blnFinancial != true)
                        {
                            if (noacessnewsfeedgroupid.Length > 0)
                                noacessnewsfeedgroupid = noacessnewsfeedgroupid + "," + (int)NewsfeedFilter.Financial;
                            else
                                noacessnewsfeedgroupid = Convert.ToString((int)NewsfeedFilter.Financial);
                        }
                        if (blnTask != true)
                        {
                            if (noacessnewsfeedgroupid.Length > 0)
                                noacessnewsfeedgroupid = noacessnewsfeedgroupid + "," + (int)NewsfeedFilter.Task_information;
                            else
                                noacessnewsfeedgroupid = Convert.ToString((int)NewsfeedFilter.Task_information);
                        }
                        if (blnObjectives != true)
                        {
                            if (noacessnewsfeedgroupid.Length > 0)
                                noacessnewsfeedgroupid = noacessnewsfeedgroupid + "," + (int)NewsfeedFilter.Objectives_Assignments;
                            else
                                noacessnewsfeedgroupid = Convert.ToString((int)NewsfeedFilter.Objectives_Assignments);
                        }

                        if (blnAttachmentsView != true && blnAttachmentEdit != true)
                        {
                            if (noacessnewsfeedgroupid.Length > 0)
                                noacessnewsfeedgroupid = noacessnewsfeedgroupid + "," + (int)NewsfeedFilter.Attachments;
                            else
                                noacessnewsfeedgroupid = Convert.ToString((int)NewsfeedFilter.Attachments);
                        }


                        IList<MultiProperty> feedgroupLIST = new List<MultiProperty>();
                        feedgroupLIST.Add(new MultiProperty { propertyName = "Id", propertyValue = newsfeedgroupid });

                        if (noacessnewsfeedgroupid.Length > 0)
                        {
                            var sqlquery = new StringBuilder();
                            sqlquery.Append("DECLARE @Template VARCHAR(8000) ");
                            sqlquery.Append(" SELECT @Template = COALESCE(@Template + ', ', '') + TEMPLATE FROM [CM_FeedFilter_Group] WHERE id  Not IN(" + noacessnewsfeedgroupid + ") ");
                            sqlquery.Append(" SELECT @Template AS Template ");

                            var newsfeedtempldid = ((tx.PersistenceManager.CommonRepository.ExecuteQuery(sqlquery.ToString()).Cast<Hashtable>().ToList()));
                            if (newsfeedtempldid.Count > 0)
                            {
                                newFeedIdsformgroup = newsfeedtempldid.Cast<Hashtable>().Select(a => (string)a["Template"]).FirstOrDefault();
                            }
                        }



                    }



                    if (userfeedSelection != null)
                    {
                        if (newsfeedid == 0)
                        {
                            if (Convert.ToString(entityId) != "0")
                            {
                                //childparLIST.Add(new MultiProperty { propertyName = "entityId", propertyValue = entityId });
                                //childparLIST.Add(new MultiProperty { propertyName = "userfeedSelection_0", propertyValue = userfeedSelection[0] });



                                feedSelectQuery.Append("select cmf.ID,cmf.Actor,cmf.UserID, cmf.TemplateID,cmf.HappenedOn,cmf.CommentedUpdatedOn,cmf.EntityID,cmf.AssocitedEntityID,cmf.AttributeGroupRecordName, cmf.TypeName,cmf.TypeName," +
                                                 "cmf.AttributeName,cmf.FromValue,cmf.ToValue,pme.Name as 'EntityName',pme.UniqueKey as 'EntiyUniquekey',pme.TypeId as 'Typeid', pme.ParentID  'EntiyParentID', parentEnt.Name as 'ParentName'," +
                                                     "umuse.FirstName as 'UserFirstName',umuse.LastName 'UserLastName',umuse.Email as 'UserEmail',umuse.Image as 'UserImage'," +
                                                     "umuse.TimeZone as 'UserTimeZone',umuse.FeedSelection as 'UserFeedselect', cmt.Template as 'FeedTemplate',cmf.Version as 'Version',pey.IsAssociate AS 'Associate' from CM_Feed cmf inner join PM_Entity pme on cmf.EntityID = pme.ID inner join UM_User umuse on" +
                                                 " umuse.ID = cmf.Actor Left join PM_Entity parentEnt on pme.ParentID = parentEnt.ID  inner join CM_Feed_Template cmt on cmt.ID = cmf.TemplateID LEFT JOIN MM_EntityType pey ON  pey.ID = pme.TypeId  where (pme.ID in (" + entityId.TrimEnd(',') + ") ");
                                for (int i = 0; i < qryUniquekeys.Count(); i++)
                                {
                                    if (i == 0)
                                        feedSelectQuery.Append("or (pme.id in (select  pe.ID FROM PM_Entity pe where pe.uniquekey like '" + qryUniquekeys[i] + ".%'");
                                    else
                                        feedSelectQuery.Append("union all SELECT pe.ID FROM PM_Entity pe where pe.uniquekey like '" + qryUniquekeys[i] + ".%'");
                                }
                                feedSelectQuery.Append("))");
                                //     "pme.UniqueKey  like " +
                                //"(SELECT pe.UniqueKey + '.%' FROM   PM_Entity pe WHERE  pe.ID in( :entityId))) and (pme.TypeID in (" + userfeedSelection[0] + ") or pme.TypeID in (select mm.ID from MM_EntityType mm where mm.IsAssociate=1) )and cmf.HappenedOn ");
                                feedSelectQuery.AppendLine("and (pme.TypeID in (" + userfeedSelection[0] + ") or pme.TypeID in (select mm.ID from MM_EntityType mm where mm.IsAssociate=1)) )and cmf.HappenedOn");

                                //if (newFeedIdsformgroup.Length > 0)
                                //{
                                //    feedSelectQuery.Append("and cmf.TemplateID in(" + newFeedIdsformgroup + ") ");
                                //}
                            }
                            else
                            {
                                childparLIST.Add(new MultiProperty { propertyName = "User_Id", propertyValue = proxy.MarcomManager.User.Id });
                                //childparLIST.Add(new MultiProperty { propertyName = "userfeedSelection_0", propertyValue = userfeedSelection[0] });

                                //feedSelectQuery.Append("select distinct cmf.ID,cmf.Actor,cmf.UserID,cmf.TemplateID,cmf.HappenedOn,cmf.CommentedUpdatedOn,cmf.EntityID,cmf.AssocitedEntityID,cmf.AttributeGroupRecordName,cmf.TypeName,cmf.TypeName," +
                                //                          "cmf.AttributeName,cmf.FromValue,cmf.ToValue,pme.Name as 'EntityName',pme.UniqueKey as 'EntiyUniquekey',pme.ParentID 'EntiyParentID', parentEnt.Name as 'ParentName'," +
                                //                         "umuse.FirstName as 'UserFirstName',umuse.LastName 'UserLastName',umuse.Email as 'UserEmail',umuse.Image as 'UserImage'," +
                                //                          "umuse.TimeZone as 'UserTimeZone',umuse.FeedSelection as 'UserFeedselect',cmt.Template as 'FeedTemplate' from CM_Feed cmf inner join PM_Entity pme on cmf.EntityID = pme.ID inner join PM_Entity parentEnt on pme.ParentID = parentEnt.ID  inner join UM_User umuse on" +
                                //                   " umuse.ID = cmf.Actor inner join CM_Feed_Template cmt on cmt.ID = cmf.TemplateID inner join AM_Entity_Role_User amr on amr.EntityID=cmf.EntityID where  amr.UserID= :User_Id and (pme.TypeID in (" + userfeedSelection[0] + ") or pme.TypeID in (select mm.ID from MM_EntityType mm where mm.IsAssociate=1)) and cmf.HappenedOn ");

                                feedSelectQuery.Append("select distinct cmf.ID,cmf.Actor,cmf.UserID,cmf.TemplateID,cmf.HappenedOn,cmf.CommentedUpdatedOn,cmf.EntityID,cmf.AssocitedEntityID,cmf.AttributeGroupRecordName,cmf.TypeName,cmf.TypeName," +
                                                          "cmf.AttributeName,cmf.FromValue,cmf.ToValue,pme.Name as 'EntityName',pme.UniqueKey as 'EntiyUniquekey',pme.ParentID 'EntiyParentID',isnull(parentEnt.Name,'-') as 'ParentName'," +
                                                         "umuse.FirstName as 'UserFirstName',umuse.LastName 'UserLastName',umuse.Email as 'UserEmail',umuse.Image as 'UserImage'," +
                                                          "umuse.TimeZone as 'UserTimeZone',umuse.FeedSelection as 'UserFeedselect',cmt.Template as 'FeedTemplate',cmf.Version as 'Version',pey.IsAssociate AS 'Associate' from CM_Feed cmf inner join PM_Entity pme on cmf.EntityID = pme.ID  LEFT outer JOIN  PM_Entity parentEnt on pme.ParentID = parentEnt.ID  inner join UM_User umuse on" +
                                                   " umuse.ID = cmf.Actor inner join CM_Feed_Template cmt on cmt.ID = cmf.TemplateID LEFT JOIN MM_EntityType pey ON  pey.ID = pme.TypeId  inner join AM_Entity_Role_User amr on ( amr.EntityID = cmf.EntityID OR  amr.EntityID =pme.ParentID) where  amr.UserID= :User_Id and (pme.TypeID in (" + userfeedSelection[0] + ") or pme.TypeID in (select mm.ID from MM_EntityType mm where mm.IsAssociate=1)) and cmf.HappenedOn ");

                            }
                            if (isForRealTimeUpdate)
                            {
                                feedSelectQuery.Append(" >= '" + (proxy.MarcomManager.UserManager.FeedRecentlyUpdatedTime).ToString("yyyy-MM-dd HH:mm:ss.FFFFFFF") + "'");
                                if (newFeedIdsformgroup.Length > 0)
                                {
                                    feedSelectQuery.Append("and cmf.TemplateID in(" + newFeedIdsformgroup + ") ");
                                }
                                feedSelectQuery.Append("ORDER BY cmf.HappenedOn asc ");
                            }
                            else
                            {
                                feedSelectQuery.Append(" <= '" + (proxy.MarcomManager.UserManager.FeedInitialRequestedTime).ToString("yyyy-MM-dd HH:mm:ss.FFFFFFF") + "'");
                                if (newFeedIdsformgroup.Length > 0)
                                {
                                    feedSelectQuery.Append("and cmf.TemplateID in(" + newFeedIdsformgroup + ") ");
                                }
                                feedSelectQuery.Append(" ORDER BY cmf.HappenedOn desc OFFSET " + pageNo + " ROWS FETCH NEXT 20 ROWS ONLY");
                            }
                        }
                        else
                        {
                            childparLIST.Add(new MultiProperty { propertyName = "entityId", propertyValue = entityId });
                            childparLIST.Add(new MultiProperty { propertyName = "newsfeedid", propertyValue = newsfeedid });
                            feedSelectQuery.Append("select cmf.ID,cmf.Actor,cmf.UserID, cmf.TemplateID,cmf.HappenedOn,cmf.CommentedUpdatedOn,cmf.EntityID,cmf.AssocitedEntityID,cmf.AttributeGroupRecordName,cmf.TypeName,cmf.TypeName," +
                                                  "cmf.AttributeName,cmf.FromValue,cmf.ToValue,pme.Name as 'EntityName',pme.UniqueKey as 'EntiyUniquekey',pme.TypeId as 'Typeid', pme.ParentID  'EntiyParentID', parentEnt.Name as 'ParentName'," +
                                                      "umuse.FirstName as 'UserFirstName',umuse.LastName 'UserLastName',umuse.Email as 'UserEmail',umuse.Image as 'UserImage'," +
                                                      "umuse.TimeZone as 'UserTimeZone',umuse.FeedSelection as 'UserFeedselect', cmt.Template as 'FeedTemplate',cmf.Version as 'Version',pey.IsAssociate AS 'Associate' from CM_Feed cmf inner join PM_Entity pme on cmf.EntityID = pme.ID inner join UM_User umuse on" +
                                                  " umuse.ID = cmf.Actor Left join PM_Entity parentEnt on pme.ParentID = parentEnt.ID  inner join CM_Feed_Template cmt on cmt.ID = cmf.TemplateID LEFT JOIN MM_EntityType pey ON  pey.ID = pme.TypeId  where (pme.ID= :entityId or pme.UniqueKey  like " +
                                                      "(SELECT pe.UniqueKey + '.%' FROM   PM_Entity pe WHERE  pe.ID in (:entityId))) and  cmf.id= :newsfeedid ");
                            if (newFeedIdsformgroup.Length > 0)
                            {
                                feedSelectQuery.Append("and cmf.TemplateID in(" + newFeedIdsformgroup + ") ");
                            }
                        }



                        var childEntiyResult = ((tx.PersistenceManager.CommonRepository.ExecuteQuerywithParam(feedSelectQuery.ToString(), childparLIST)).Cast<Hashtable>().ToList());

                        BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("========================== NEWS FEED COMMENTS QUERY EXECUTING =================================", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);
                        BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo(feedSelectQuery.ToString(), BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);

                        //DateTimeOffset retreiveTime; 
                        //if (childEntiyResult.Count > 0)
                        DateTimeOffset retreiveTime = DateTimeOffset.UtcNow;
                        TimeSpan offSet = new TimeSpan();
                        if (newsfeedid == 0)
                        {
                            offSet = TimeSpan.Parse(proxy.MarcomManager.User.TimeZone.TrimStart('+'));
                        }
                        //  ----------- Getting the comments for the list of feeds  ------------------------------


                        //List<int> arrFeedId = new List<int>();

                        //arrFeedId = childEntiyResult.Cast<Hashtable>().Select(a => (int)a["ID"]).ToList();

                        //foreach (var obj in childEntiyResult)
                        //{
                        //    arrFeedId.Add(Convert.ToInt32(obj["ID"]));
                        //}

                        string arrFeedIdRes = "";
                        if (childEntiyResult.Count > 0)
                        {
                            arrFeedIdRes = string.Join(",", childEntiyResult.Cast<Hashtable>().Select(a => (int)a["ID"]).ToArray());
                        }
                        feedSelectQuery.Clear();


                        if (arrFeedIdRes == "")
                        {
                            feedSelectQuery.Append("select cmfcom.ID,cmfcom.FeedID,cmfcom.Comment,cmfcom.CommentedOn,cmfcom.Actor," +
                             "umuse.FirstName as 'UserFirstName',umuse.LastName as 'UserLastName',umuse.Email as 'UserEmail',umuse.Image as 'UserImage'," +
                             "umuse.TimeZone as 'UserTimeZone' from  CM_Feed_Comment cmfcom inner join UM_User umuse on" +
                             " umuse.ID = cmfcom.Actor " +
                             " order by cmfcom.CommentedOn ASC");
                        }
                        else
                        {
                            feedSelectQuery.Append("select cmfcom.ID,cmfcom.FeedID,cmfcom.Comment,cmfcom.CommentedOn,cmfcom.Actor," +
                             "umuse.FirstName as 'UserFirstName',umuse.LastName as 'UserLastName',umuse.Email as 'UserEmail',umuse.Image as 'UserImage'," +
                             "umuse.TimeZone as 'UserTimeZone' from  CM_Feed_Comment cmfcom inner join UM_User umuse on" +
                             " umuse.ID = cmfcom.Actor where cmfcom.FeedID IN  " +
                             "(" + arrFeedIdRes + ") order by cmfcom.CommentedOn ASC");
                        }


                        var GetFeedcomments = ((tx.PersistenceManager.CommonRepository.ExecuteQuery(feedSelectQuery.ToString())).Cast<Hashtable>().ToList());


                        //---------------------------------------------------------------

                        DateTimeOffset userFeedInitialTime = proxy.MarcomManager.UserManager.FeedInitialRequestedTime; //User initial request time

                        var entityIDArr = childEntiyResult.Cast<Hashtable>().Select(a => (int)a["EntityID"]).Distinct().ToArray();

                        //total entities associated for newsfeed
                        var totalFeedEntities = (from tt in tx.PersistenceManager.PlanningRepository.Query<EntityDao>() where entityIDArr.Contains(tt.Id) select new { tt.Typeid, tt.Name, tt.Id, tt.Parentid }).ToList();

                        //total entitytype associated for this newsfeed 
                        var totalFeedEntityTypes = (from ent in totalFeedEntities
                                                    join entTp in tx.PersistenceManager.PlanningRepository.GetAll<EntityTypeDao>() on ent.Typeid equals entTp.Id
                                                    orderby entTp.Id
                                                    select new
                                                    {
                                                        entTp.Id,
                                                        entTp.Caption
                                                    }).ToList();

                        //total task entities associated for newsfeed
                        var totalFeedTaskEntities = (from tt in tx.PersistenceManager.PlanningRepository.Query<EntityTaskDao>() where entityIDArr.Contains(tt.ID) select tt).ToList<EntityTaskDao>();
                        //user belongs to these newsfeeds
                        var actorIDArr = childEntiyResult.Cast<Hashtable>().Select(a => (int)a["Actor"]).Distinct().ToArray();
                        var totalFeedActors = (from tt in tx.PersistenceManager.PlanningRepository.Query<UserDao>() where actorIDArr.Contains(tt.Id) select new { tt.FirstName, tt.LastName, tt.Id }).ToList();

                        var userIDArr = childEntiyResult.Cast<Hashtable>().Select(a => (int)a["UserID"]).Distinct().ToArray();
                        var totalFeedUsers = (from tt in tx.PersistenceManager.PlanningRepository.Query<UserDao>() where userIDArr.Contains(tt.Id) select new { tt.FirstName, tt.LastName, tt.Id }).ToList();

                        //total Role involved for this feeds
                        var totalrole = (from tt in tx.PersistenceManager.PlanningRepository.Query<RoleDao>() select new { tt.Caption, tt.Id }).ToList();

                        string dateformate = proxy.MarcomManager.GlobalAdditionalSettings[0].SettingValue.ToString().Replace('m', 'M');
                        dateformate += " hh:mm:ss tt";

                        foreach (var obj in childEntiyResult)
                        {

                            try
                            {
                                FeedSelection feedObj = new FeedSelection();
                                feedObj.FeedId = Convert.ToInt32(obj["ID"]);
                                feedObj.UserName = Convert.ToString(obj["UserFirstName"] + " " + obj["UserLastName"]);
                                feedObj.UserEmail = Convert.ToString(obj["UserEmail"]);
                                feedObj.UserImage = Convert.ToString(obj["UserImage"]);
                                feedObj.Actor = Convert.ToInt32(obj["Actor"]);

                                var typename = Convert.ToString(obj["TypeName"]);
                                var entityname = Convert.ToString(obj["EntityName"]);


                                //TimeSpan difference = (proxy.MarcomManager.UserManager.FeedInitialRequestedTime - DateTime.Parse(obj["HappenedOn"].ToString()));
                                if (newsfeedid == 0)
                                {
                                    TimeSpan difference = (userFeedInitialTime - DateTime.Parse(obj["HappenedOn"].ToString()));
                                    if (difference.Days > 0)
                                        if (difference.Days > 1)
                                            feedObj.FeedHappendTime = ((DateTimeOffset)obj["HappenedOn"] + offSet).ToString(dateformate);
                                        else
                                            feedObj.FeedHappendTime = "Yesterday at " + ((DateTimeOffset)obj["HappenedOn"] + offSet).DateTime.ToShortTimeString();
                                    else if (difference.Hours > 0)
                                        if (difference.Hours < 2)
                                            feedObj.FeedHappendTime = "about an hour ago";
                                        else
                                            feedObj.FeedHappendTime = difference.Hours + " hours ago";
                                    else if (difference.Minutes > 0)
                                        feedObj.FeedHappendTime = difference.Minutes + " minutes ago";
                                    else
                                        feedObj.FeedHappendTime = "Few seconds ago";
                                }
                                string template = Convert.ToString(obj["FeedTemplate"]);


                                //foreach (Match mm in Regex.Matches(template, @"(?<=\@#)(.*?)(?=\@#)"))
                                //{
                                //    template = template.Replace("#@", "").Replace("@#", "").ToString();
                                //}
                                StringBuilder sb = new StringBuilder(template);
                                foreach (Match match in Regex.Matches(template, @"@(.+?)@"))
                                {

                                    switch (match.Value.Trim())
                                    {
                                        case "@EntityTypeName@":
                                            {
                                                if (Convert.ToInt32(obj["TemplateID"]) == 56)
                                                {
                                                    sb.Replace(match.Value, "Additional Object");
                                                }

                                                if (Convert.ToInt32(obj["TemplateID"]) == 22)
                                                {
                                                    sb.Replace(match.Value, obj["AttributeName"].ToString());
                                                }
                                                else if (Convert.ToInt32(obj["TemplateID"]) == 24 || Convert.ToInt32(obj["TemplateID"]) == 25 || Convert.ToInt32(obj["TemplateID"]) == 26 || Convert.ToInt32(obj["TemplateID"]) == 27 || Convert.ToInt32(obj["TemplateID"]) == 28 || Convert.ToInt32(obj["TemplateID"]) == 31 || Convert.ToInt32(obj["TemplateID"]) == 32 || Convert.ToInt32(obj["TemplateID"]) == 33 || Convert.ToInt32(obj["TemplateID"]) == 34 || Convert.ToInt32(obj["TemplateID"]) == 35 || Convert.ToInt32(obj["TemplateID"]) == 36 || Convert.ToInt32(obj["TemplateID"]) == 13 || Convert.ToInt32(obj["TemplateID"]) == 17 || Convert.ToInt32(obj["TemplateID"]) == 22 || Convert.ToInt32(obj["TemplateID"]) == 40 || Convert.ToInt32(obj["TemplateID"]) == 39)
                                                {
                                                    if (Convert.ToInt32(entityIdForReference) == Convert.ToInt32(obj["EntiyParentID"]))
                                                        sb.Replace(match.Value, "");
                                                    else
                                                    {
                                                        if (Convert.ToInt32(entityIdForReference) == Convert.ToInt32(obj["EntiyParentID"]))
                                                            sb.Replace(match.Value, "");
                                                        else
                                                        {
                                                            if (Convert.ToInt32(obj["TemplateID"]) == 22 || Convert.ToInt32(obj["TemplateID"]) == 39 || Convert.ToInt32(obj["TemplateID"]) == 40 || Convert.ToInt32(obj["TemplateID"]) == 35 || Convert.ToInt32(obj["TemplateID"]) == 27 || Convert.ToInt32(obj["TemplateID"]) == 28)
                                                            {

                                                                var temptofetchtaskentitytype = (from tt in totalFeedEntities where tt.Id == Convert.ToInt32(obj["EntityID"]) select tt.Typeid).FirstOrDefault();
                                                                var taskdetailsforroottype = (from tt in totalFeedEntityTypes where tt.Id == Convert.ToInt32(temptofetchtaskentitytype) select tt).FirstOrDefault();
                                                                sb.Replace(match.Value, taskdetailsforroottype.Caption);

                                                            }
                                                            else
                                                            {
                                                                var entitytaskdetails = (from tt in totalFeedEntities where tt.Id == Convert.ToInt32(obj["AssocitedEntityID"]) select tt).FirstOrDefault();
                                                                var entitytasktypename = (from type in totalFeedEntityTypes where type.Id == entitytaskdetails.Typeid select type).FirstOrDefault();
                                                                sb.Replace(match.Value, entitytasktypename.Caption);
                                                            }
                                                        }
                                                    }
                                                }
                                                else if ((Convert.ToInt32(obj["TemplateID"]) == 61))
                                                {

                                                    EntityDao tasktype = tx.PersistenceManager.TaskRepository.Query<EntityDao>().Where(x => x.Id == Convert.ToInt32(obj["EntityID"])).FirstOrDefault();
                                                    EntityTypeDao objType = tx.PersistenceManager.TaskRepository.Query<EntityTypeDao>().Where(s => s.Id == tasktype.Typeid).SingleOrDefault();
                                                    sb.Replace(match.Value, objType.Caption);

                                                    //EntityDao tasktype1 = tx.PersistenceManager.TaskRepository.Query<EntityDao>().Where(x => x.Id == Convert.ToInt32(obj["ToValue"])).FirstOrDefault();
                                                    //EntityTypeDao objType1 = tx.PersistenceManager.TaskRepository.Query<EntityTypeDao>().Where(s => s.Id == tasktype1.Typeid).SingleOrDefault();
                                                    //sb.Replace("to " + match.Value, "to " + objType1.Caption);
                                                }


                                                else
                                                {
                                                    if (Convert.ToInt32(entityIdForReference) == Convert.ToInt32(obj["EntityID"]) && (obj["TemplateID"].ToString() == "1" || obj["TemplateID"].ToString() == "18"))
                                                        sb.Replace("new", "this");
                                                    sb.Replace(match.Value, typename);
                                                }
                                                break;
                                            }
                                        case "@FromEntityTypeName@":
                                            {
                                                if ((Convert.ToInt32(obj["TemplateID"]) == 61))
                                                {

                                                    if (obj["AttributeName"].ToString() != "" | obj["AttributeName"].ToString() != null)
                                                    {
                                                        EntityDao tasktype = tx.PersistenceManager.TaskRepository.Query<EntityDao>().Where(x => x.Id == Convert.ToInt32(obj["AttributeName"].ToString())).FirstOrDefault();
                                                        EntityTypeDao objType = tx.PersistenceManager.TaskRepository.Query<EntityTypeDao>().Where(s => s.Id == tasktype.Typeid).SingleOrDefault();
                                                        sb.Replace(match.Value, objType.Caption);
                                                    }
                                                    else
                                                        sb.Replace(match.Value, "");
                                                }
                                                break;
                                            }
                                        case "@FundingRequestState@":
                                            {
                                                sb.Replace(match.Value, obj["ToValue"].ToString());
                                                break;
                                            }
                                        case "@EntityNamefortask@":
                                            {
                                                var entitynameforfundcomment = (from tt in totalFeedEntities where tt.Id == Convert.ToInt32(obj["EntityID"]) select tt).FirstOrDefault();
                                                if (Convert.ToInt32(obj["Typeid"]) == 7)
                                                    sb.Replace(match.Value, "<a href='javascript:void(0);'  data-id=\"openfundrequestpopup\" data-parententityname='" + obj["ParentName"] + "' data-parentid='" + obj["EntiyParentID"] + "' data-entityid='" + obj["EntityID"] + "' data-entityname='" + obj["ParentName"] + "' data-fundingreqname='" + entityname + "' data-typeid='" + obj["Typeid"] + "'>" + entityname + "</a>");
                                                else
                                                {
                                                    sb.Replace(match.Value, "<a href='javascript:void(0);' data-id=\"taskpopupopen\"  data-id=\"openpopupfortaskedit\"  data-taskid='" + entitynameforfundcomment.Id + "'  data-entityid='" + entitynameforfundcomment.Parentid + "' data-typeid='" + entitynameforfundcomment.Typeid + "' >" + entitynameforfundcomment.Name + "</a>");
                                                }
                                                break;
                                            }
                                        case "@FromEntityName@":
                                            if ((Convert.ToInt32(obj["TemplateID"]) == 61))
                                            {

                                                if (obj["AttributeName"].ToString() != "" | obj["AttributeName"].ToString() != null)
                                                {
                                                    if (Convert.ToInt32(entityIdForReference) == Convert.ToInt32(obj["AttributeName"].ToString()))
                                                        sb.Replace(match.Value, "this");
                                                    else
                                                    {
                                                        EntityDao objEntity = tx.PersistenceManager.TaskRepository.Query<EntityDao>().Where(x => x.Id == Convert.ToInt32(obj["AttributeName"].ToString())).FirstOrDefault();
                                                        EntityTypeDao objType = tx.PersistenceManager.TaskRepository.Query<EntityTypeDao>().Where(s => s.Id == objEntity.Typeid).SingleOrDefault();
                                                        sb.Replace(match.Value, "<a href='javascript:void(0);' data-id=\"feedpath\" data-parentid='" + objEntity.Parentid + "'data-entityid='" + objEntity.Id + "' data-typeid='" + objType.Id + "' >" + objEntity.Name + "</a>");
                                                    }
                                                }
                                                else
                                                    sb.Replace(match.Value, "");
                                            }
                                            break;


                                        case "@EntityName@":
                                            {
                                                if (Convert.ToInt32(obj["TemplateID"]) == 24 || Convert.ToInt32(obj["TemplateID"]) == 25 || Convert.ToInt32(obj["TemplateID"]) == 39 || Convert.ToInt32(obj["TemplateID"]) == 26 || Convert.ToInt32(obj["TemplateID"]) == 27 || Convert.ToInt32(obj["TemplateID"]) == 28 || Convert.ToInt32(obj["TemplateID"]) == 31 || Convert.ToInt32(obj["TemplateID"]) == 32 || Convert.ToInt32(obj["TemplateID"]) == 33 || Convert.ToInt32(obj["TemplateID"]) == 34 || Convert.ToInt32(obj["TemplateID"]) == 35 || Convert.ToInt32(obj["TemplateID"]) == 36 || Convert.ToInt32(obj["TemplateID"]) == 13 || Convert.ToInt32(obj["TemplateID"]) == 17 || Convert.ToInt32(obj["TemplateID"]) == 22 || Convert.ToInt32(obj["TemplateID"]) == 40)
                                                {
                                                    if (Convert.ToInt32(entityIdForReference) == Convert.ToInt32(obj["EntiyParentID"]))
                                                        sb.Replace(match.Value, "");
                                                    else
                                                    {
                                                        if (Convert.ToInt32(obj["TemplateID"]) == 22 || Convert.ToInt32(obj["TemplateID"]) == 40 || Convert.ToInt32(obj["TemplateID"]) == 39 || Convert.ToInt32(obj["TemplateID"]) == 35 || Convert.ToInt32(obj["TemplateID"]) == 27 || Convert.ToInt32(obj["TemplateID"]) == 28)
                                                        {
                                                            var temptofetchtaskentity = (from tt in totalFeedTaskEntities where tt.ID == Convert.ToInt32(obj["EntityID"]) select tt.EntityID).FirstOrDefault();
                                                            var taskdetailsforroot = (from tt in totalFeedEntities where tt.Id == Convert.ToInt32(temptofetchtaskentity) select tt).FirstOrDefault();
                                                            sb.Replace(match.Value, "<a href='javascript:void(0);' data-id=\"feedpath\" data-parentid='" + taskdetailsforroot.Parentid + "'data-entityid='" + taskdetailsforroot.Id + "' data-typeid='" + taskdetailsforroot.Typeid + "' >" + taskdetailsforroot.Name + "</a>");

                                                        }
                                                        else
                                                        {
                                                            var entitytaskdetails = (from tt in totalFeedEntities where tt.Id == Convert.ToInt32(obj["AssocitedEntityID"]) select tt).FirstOrDefault();
                                                            sb.Replace(match.Value, "<a href='javascript:void(0);' data-id=\"feedpath\" data-parentid='" + entitytaskdetails.Parentid + "'data-entityid='" + entitytaskdetails.Id + "' data-typeid='" + entitytaskdetails.Typeid + "' >" + entitytaskdetails.Name + "</a>");
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    //Inserting the predefined objective
                                                    if (Convert.ToInt32(obj["TemplateID"]) == 57)
                                                    {
                                                        sb.Replace(match.Value, "<a href='javascript:void(0);' data-id=\"feedpath\" data-parentid='" + obj["EntityID"] + "' data-entityid='" + obj["AssocitedEntityID"] + "' data-entitytypeName ='PredefinedObjective' data-typeid='10'  >" + obj["AttributeName"] + "</a>");
                                                    }
                                                    //deleteing the predefined objectiave)
                                                    if (Convert.ToString(obj["TypeName"]) == "Predefined Objective" && Convert.ToInt32(obj["TemplateID"]) == 19)
                                                    {
                                                        sb.Replace(match.Value, "<a href='javascript:void(0);' data-id=\"feedpath\" data-parentid='" + obj["EntiyID"] + "'data-entityid='" + obj["AssocitedEntityID"] + "' data-typeid='" + obj["Typeid"] + "' data-entitytypeName ='PredefinedObjective' data-typeid='10' >" + obj["AttributeName"] + "</a>");
                                                    }

                                                    if (Convert.ToInt32(obj["TemplateID"]) == 56)
                                                    {
                                                        sb.Replace(match.Value, "<a href='javascript:void(0);' data-id=\"feedpath\" data-parentid='" + obj["EntiyParentID"] + "'  data-FromValue='" + obj["FromValue"] + "' data-ToValue='" + obj["ToValue"] + "' data-entityid='" + obj["EntityID"] + "'    data-typeid='" + obj["Typeid"] + "'>" + entityname + "</a>");
                                                    }
                                                    if (Convert.ToInt32(obj["TemplateID"]) == 48)
                                                    {
                                                        //EntiyParentID 
                                                        if (Convert.ToInt32(entityIdForReference) == Convert.ToInt32(obj["EntityID"]))
                                                            sb.Replace(match.Value, "");
                                                        else
                                                            sb.Replace(match.Value, "<a href='javascript:void(0);' data-id=\"feedpath\" data-parentid='" + obj["EntiyParentID"] + "'data-entityid='" + obj["EntityID"] + "' data-typeid='" + obj["Typeid"] + "' >" + entityname + "</a>").Replace("this", "");
                                                    }
                                                    if (Convert.ToInt32(obj["TemplateID"]) == 61)
                                                    {
                                                        //EntiyParentID 
                                                        if (Convert.ToInt32(entityIdForReference) == Convert.ToInt32(obj["EntityID"]))
                                                            sb.Replace(match.Value, "this");
                                                        else
                                                        {
                                                            EntityDao objEntity = tx.PersistenceManager.TaskRepository.Query<EntityDao>().Where(x => x.Id == Convert.ToInt32(obj["EntityID"].ToString())).FirstOrDefault();
                                                            EntityTypeDao objType = tx.PersistenceManager.TaskRepository.Query<EntityTypeDao>().Where(s => s.Id == objEntity.Typeid).SingleOrDefault();
                                                            sb.Replace(match.Value, "<a href='javascript:void(0);' data-id=\"feedpath\" data-parentid='" + objEntity.Parentid + "'data-entityid='" + obj["EntityID"] + "' data-typeid='" + objType.Id + "' >" + objEntity.Name + "</a>");
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (Convert.ToInt32(entityIdForReference) == Convert.ToInt32(obj["EntityID"]))
                                                            sb.Replace(match.Value, "");
                                                        else
                                                        {

                                                            if (Convert.ToInt32(obj["TemplateID"]) == 6)
                                                                sb.Replace(match.Value, "<a href='javascript:void(0);'  data-id=\"openfundrequestpopup\" data-parententityname='" + obj["ParentName"] + "' data-parentid='" + obj["EntiyParentID"] + "' data-entityid='" + obj["EntityID"] + "' data-entityname='" + obj["ParentName"] + "' data-fundingreqname='" + entityname + "' data-typeid='" + obj["Typeid"] + "'>" + entityname + "</a>");
                                                            else
                                                                sb.Replace(match.Value, "<a href='javascript:void(0);' data-id=\"feedpath\" data-parentid='" + obj["EntiyParentID"] + "'data-entityid='" + obj["EntityID"] + "' data-typeid='" + obj["Typeid"] + "' >" + entityname + "</a>");
                                                        }
                                                    }

                                                }
                                                break;
                                            }
                                        //case "@ObjectiveName@":
                                        //    if (Convert.ToInt32(obj["Typeid"]) > 50)
                                        //    {
                                        //        var objectiveDetails = (from item in tx.PersistenceManager.PlanningRepository.Query<EntityDao>() where item.Id == Convert.ToInt32(obj["AssocitedEntityID"]) select item).FirstOrDefault();
                                        //        sb.Replace(match.Value, "<a href='javascript:void(0);' data-id=\"objectivelink\" data-objectiveid='" + obj["AssocitedEntityID"] + "'>" + objectiveDetails.Name + "</a>");
                                        //    }

                                        //    break;
                                        case "@AttributeName@":
                                            {
                                                if ((Convert.ToInt32(obj["TemplateID"]) == 9) || (Convert.ToInt32(obj["TemplateID"]) == 12))
                                                {
                                                    var formattedvalue = GetCurrency(Convert.ToInt32(obj["ToValue"]));
                                                    sb.Replace(match.Value, Convert.ToString(formattedvalue) + " " + proxy.MarcomManager.GlobalAdditionalSettings[1].CurrencyFormatvalue[0].Symbol);
                                                }
                                                else if ((Convert.ToInt32(obj["TemplateID"]) == 7) || (Convert.ToInt32(obj["TemplateID"]) == 6) || (Convert.ToInt32(obj["TemplateID"]) == 21))
                                                {
                                                    sb.Replace(match.Value, "<a href='javascript:void(0);' data-id=\"costcenterlink\" data-costcentreid='" + obj["AssocitedEntityID"] + "'>" + Convert.ToString(obj["AttributeName"]) + "</a>");
                                                }
                                                else
                                                {
                                                    sb.Replace(match.Value, Convert.ToString(obj["AttributeName"]));
                                                }
                                                break;
                                            }
                                        case "@AttributeGroupAttributeName@":
                                            {
                                                sb.Replace(match.Value, Convert.ToString(obj["AttributeName"]));
                                                break;
                                            }
                                        case "@AttributeGroupNameRecord@":
                                            {
                                                sb.Replace(match.Value, Convert.ToString(obj["AttributeGroupRecordName"]));
                                                break;
                                            }
                                        case "@AttributeGroupName@":
                                            {
                                                sb.Replace(match.Value, Convert.ToString(obj["TypeName"]));
                                                break;
                                            }
                                        case "@CostCenterName@":
                                            {
                                                if (Convert.ToInt32(obj["TemplateID"]) == 60)
                                                {

                                                    if (entityIdForReference == Convert.ToInt32(obj["EntityID"]))
                                                    {
                                                        sb.Replace(match.Value, " ");
                                                    }
                                                    sb.Replace(match.Value, "<a href='javascript:void(0);' data-id=\"costcenterlink\" data-costcentreid='" + obj["AssocitedEntityID"] + "'>" + obj["AttributeName"] + "</a>");
                                                }
                                                else
                                                {
                                                    sb.Replace(match.Value, "<a href='javascript:void(0);' data-id=\"costcenterlink\" data-costcentreid='" + obj["AssocitedEntityID"] + "'>" + obj["AttributeName"] + "</a>");
                                                }
                                                break;
                                            }
                                        case "@ObjectiveName@":
                                            if (Convert.ToInt32(obj["Typeid"]) > 50)
                                            {
                                                var objectiveDetails = (from item in tx.PersistenceManager.PlanningRepository.Query<EntityDao>() where item.Id == Convert.ToInt32(obj["AssocitedEntityID"]) select item).FirstOrDefault();
                                                sb.Replace(match.Value, "<a href='javascript:void(0);' data-id=\"objectivelink\" data-objectiveid='" + obj["AssocitedEntityID"] + "'>" + objectiveDetails.Name + "</a>");
                                            }
                                            else if (Convert.ToInt32(obj["Typeid"]) == 11)
                                            {
                                                //    var objectiveDetails = (from item in tx.PersistenceManager.PlanningRepository.Query<EntityDao>() where item.Id == Convert.ToInt32(obj["EntityID"]) select item).FirstOrDefault();
                                                //    sb.Replace(match.Value, "<a href='javascript:void(o);' data-id=\"objectivelink\" data-objectiveid='" + obj["EntityID"] + "'>" + objectiveDetails.Name + "</a>");

                                                //StringBuilder addtionalObjQuery = new StringBuilder();
                                                //addtionalObjQuery.Append("SELECT pe.ID, pe.[Active], pe.Name,pma.TypeID,pma.ID as 'AdditoinalObectiveID',pma.Instruction,pma.IsEnableFeedback,pma.UnitID, ");
                                                //addtionalObjQuery.Append(" pma.PlannedTarget,pma.TargetOutcome,pma.RatingObjective,pma.Comments,pma.Fulfillment ");
                                                //addtionalObjQuery.Append("FROM PM_Entity pe INNER JOIN PM_AdditionalObjectiveEntityValues pma ON pe.ID = pma.EntityID");
                                                //addtionalObjQuery.Append(" AND pma.EntityID IN ");
                                                //addtionalObjQuery.Append("(SELECT pe.ID FROM PM_Entity pe WHERE pe.id = ? ) WHERE  pma.ID = ? ");
                                                //var additionalObjQueryResult = tx.PersistenceManager.PlanningRepository.ExecuteQuerywithMinParam(addtionalObjQuery.ToString(), Convert.ToInt32(obj["EntityID"]), Convert.ToInt32(obj["AssocitedEntityID"])).Cast<Hashtable>().ToList();
                                                //if (additionalObjQueryResult.Count > 0)
                                                //{
                                                //    var entitytaskdetails = (from item in tx.PersistenceManager.PlanningRepository.Query<EntityDao>() where item.Id == Convert.ToInt32(obj["EntityID"]) select item).FirstOrDefault();
                                                //    sb.Replace(match.Value, "<a href='javascript:void(0);' data-id=\"feedpath\" data-parentid='" + entitytaskdetails.Parentid + "'data-entityid='" + entitytaskdetails.Id + "' data-typeid='" + entitytaskdetails.Typeid + "' >" + entitytaskdetails.Name + "</a>");
                                                //}
                                                //else
                                                //{
                                                var entitytaskdetails = (from item in tx.PersistenceManager.PlanningRepository.Query<EntityDao>() where item.Id == Convert.ToInt32(obj["EntityID"]) select item).FirstOrDefault();
                                                sb.Replace(match.Value, "<a href='javascript:void(0);' data-id=\"feedpath\" data-parentid='" + entitytaskdetails.Parentid + "'data-entityid='" + entitytaskdetails.Id + "' data-typeid='" + entitytaskdetails.Typeid + "' >" + entitytaskdetails.Name + "</a>");
                                                //}
                                            }

                                            break;
                                        case "@ActorName@":
                                            {

                                                var user = (from tt in totalFeedActors where tt.Id == Convert.ToInt32(obj["Actor"]) select new { tt.FirstName, tt.LastName }).FirstOrDefault();
                                                sb.Replace(match.Value, user.FirstName + " " + user.LastName);
                                                break;
                                            }
                                        case "@NewsValue@":
                                            {
                                                sb.Replace(match.Value, Convert.ToString(obj["ToValue"]));
                                                break;
                                            }
                                        case "@PeriodComment@":
                                            if (Convert.ToInt32(obj["TemplateID"]) == 47)
                                            {
                                                sb.Replace(match.Value, Convert.ToString(obj["ToValue"]));

                                            }

                                            break;
                                        case "@startdate@":
                                            if (Convert.ToInt32(obj["TemplateID"]) == 47)
                                            {

                                                var EntityPerioddetails = (from tt in tx.PersistenceManager.PlanningRepository.Query<EntityPeriodDao>() where tt.Id == Convert.ToInt32(obj["AssocitedEntityID"]) select tt).FirstOrDefault();
                                                sb.Replace(match.Value, EntityPerioddetails.Startdate.ToString(proxy.MarcomManager.GlobalAdditionalSettings[0].SettingValue.ToString().Replace('m', 'M')));
                                                //sb.Replace(match.Value, EntityPerioddetails.Description); 
                                            }

                                            break;
                                        case "@enddate@":
                                            if (Convert.ToInt32(obj["TemplateID"]) == 47)
                                            {

                                                var EntityPerioddetails = (from tt in tx.PersistenceManager.PlanningRepository.Query<EntityPeriodDao>() where tt.Id == Convert.ToInt32(obj["AssocitedEntityID"]) select tt).FirstOrDefault();
                                                sb.Replace(match.Value, EntityPerioddetails.EndDate.ToString(proxy.MarcomManager.GlobalAdditionalSettings[0].SettingValue.ToString().Replace('m', 'M')));
                                                //sb.Replace(match.Value, EntityPerioddetails.Description); 
                                            }

                                            break;
                                        case "@Path@":
                                            {
                                                if (Convert.ToInt32(obj["TemplateID"]) == 24 ||
                                                    Convert.ToInt32(obj["TemplateID"]) == 13 ||
                                                    Convert.ToInt32(obj["TemplateID"]) == 17 ||
                                                    //Convert.ToInt32(obj["TemplateID"]) == 19 ||
                                                    Convert.ToInt32(obj["TemplateID"]) == 38 ||
                                                    Convert.ToInt32(obj["TemplateID"]) == 22 ||
                                                    Convert.ToInt32(obj["TemplateID"]) == 35 ||
                                                    Convert.ToInt32(obj["TemplateID"]) == 10 ||
                                                    Convert.ToInt32(obj["TemplateID"]) == 39 ||
                                                    Convert.ToInt32(obj["TemplateID"]) == 37 ||
                                                    Convert.ToInt32(obj["TemplateID"]) == 38 ||
                                                    Convert.ToInt32(obj["TemplateID"]) == 25 ||
                                                    Convert.ToInt32(obj["TemplateID"]) == 26 ||
                                                    Convert.ToInt32(obj["TemplateID"]) == 27 ||
                                                    Convert.ToInt32(obj["TemplateID"]) == 28 ||
                                                    Convert.ToInt32(obj["TemplateID"]) == 29 ||
                                                    Convert.ToInt32(obj["TemplateID"]) == 33 ||
                                                    Convert.ToInt32(obj["TemplateID"]) == 34 ||
                                                    Convert.ToInt32(obj["TemplateID"]) == 36 ||
                                                     Convert.ToInt32(obj["TemplateID"]) == 45 ||
                                                     Convert.ToInt32(obj["TemplateID"]) == 30 ||
                                                    Convert.ToInt32(obj["TemplateID"]) == 40 ||
                                                    Convert.ToInt32(obj["TemplateID"]) == 41 || Convert.ToInt32(obj["TemplateID"]) == 109 || Convert.ToInt32(obj["TemplateID"]) == 110)
                                                //Convert.ToInt32(obj["TemplateID"]) == 48)

                                                //Convert.ToInt32(obj["TemplateID"]) == 48 ||
                                                //Convert.ToInt32(obj["TemplateID"]) == 49)
                                                {

                                                    if (Convert.ToInt32(entityIdForReference) == Convert.ToInt32(obj["EntiyParentID"]))
                                                        sb.Replace(match.Value, "");
                                                    else
                                                        sb.Replace(match.Value, "<a href='javascript:void(0);' data-id=\"feedpath\" data-parentid='" + obj["ParentID"] + "'data-entityid='" + obj["EntiyParentID"] + "' data-typeid='" + obj["Typeid"] + "' >" + Convert.ToString(obj["ParentName"]) + "</a>");
                                                }

                                                else if (Convert.ToInt32(obj["TemplateID"]) == 57 || Convert.ToInt32(obj["TemplateID"]) == 12 || Convert.ToInt32(obj["TemplateID"]) == 58)
                                                {
                                                    if (Convert.ToInt32(entityIdForReference) == Convert.ToInt32(obj["EntityID"]))
                                                        sb.Replace(match.Value, "");
                                                    else
                                                        sb.Replace(match.Value, "<a href='javascript:void(0);' data-id=\"feedpath\" data-parentid='" + obj["EntiyID"] + "'data-entityid='" + obj["EntityID"] + "' data-typeid='" + obj["Typeid"] + "' >" + obj["EntityName"] + "</a>");
                                                }

                                                else if (Convert.ToString(obj["TypeName"]) == "Predefined Objective" && Convert.ToInt32(obj["TemplateID"]) == 19)
                                                {
                                                    if (Convert.ToInt32(entityIdForReference) == Convert.ToInt32(obj["EntityID"]))
                                                        sb.Replace(match.Value, "");
                                                    sb.Replace(match.Value, "<a href='javascript:void(0);' data-id=\"feedpath\" data-parentid='" + obj["EntiyID"] + "'data-entityid='" + obj["EntiyID"] + "' data-typeid='" + obj["Typeid"] + "' >" + obj["EntityName"] + "</a>");
                                                }

                                                else if (Convert.ToInt32(obj["TemplateID"]) == 48)
                                                {
                                                    if (Convert.ToInt32(entityIdForReference) == Convert.ToInt32(obj["EntiyParentID"]))
                                                        sb.Replace(match.Value, "");
                                                    else
                                                        sb.Replace(match.Value, "<a href='javascript:void(0);' data-id=\"feedpath\" data-parentid='" + obj["EntiyParentID"] + "'data-entityid='" + obj["AssocitedEntityID"] + "' data-typeid='" + obj["Typeid"] + "' >" + Convert.ToString(obj["FromValue"]) + "</a>");
                                                }

                                                else if (Convert.ToInt32(obj["TemplateID"]) == 49 || Convert.ToInt32(obj["TemplateID"]) == 56)
                                                {
                                                    if (Convert.ToInt32(entityIdForReference) == Convert.ToInt32(obj["EntiyParentID"]))
                                                        sb.Replace(match.Value, "");
                                                    else
                                                        sb.Replace(match.Value, "<a href='javascript:void(0);' data-id=\"feedpath\" data-parentid='" + obj["EntiyParentID"] + "'data-entityid='" + obj["EntiyParentID"] + "' data-typeid='" + "1" + "' >" + Convert.ToString(obj["ParentName"]) + "</a>");
                                                }
                                                else if ((Convert.ToInt32(obj["TemplateID"]) == 61))
                                                {
                                                    if (Convert.ToInt32(entityIdForReference) == Convert.ToInt32(obj["EntityID"]))
                                                    {
                                                        sb.Replace(match.Value, "");
                                                    }
                                                    else
                                                    {
                                                        EntityDao tasktype = tx.PersistenceManager.TaskRepository.Query<EntityDao>().Where(x => x.Id == Convert.ToInt32(obj["EntityID"])).FirstOrDefault();
                                                        sb.Replace(match.Value, "<a href='javascript:void(0);' data-id=\"feedpath\" data-parentid='" + Convert.ToInt32(obj["EntityParentID"]) + "'data-entityid='" + Convert.ToInt32(obj["EntityID"]) + "' data-typeid='" + Convert.ToInt32(obj["Typeid"]) + "' >" + tasktype.Name + "</a>");
                                                    }
                                                }
                                                else if (Convert.ToInt32(obj["TemplateID"]) == 19)
                                                {
                                                    if (Convert.ToInt32(entityIdForReference) == Convert.ToInt32(obj["EntiyParentID"]))
                                                        sb.Replace(match.Value, "");
                                                    else
                                                        sb.Replace(match.Value, "<a href='javascript:void(0);' data-id=\"feedpath\" data-parentid='" + obj["EntiyParentID"] + "'data-entityid='" + obj["EntiyParentID"] + "' data-typeid='" + "1" + "' >" + Convert.ToString(obj["ParentName"]) + "</a>");
                                                }
                                                else if (Convert.ToInt32(obj["TemplateID"]) == 101 || Convert.ToInt32(obj["TemplateID"]) == 102 || Convert.ToInt32(obj["TemplateID"]) == 103 || Convert.ToInt32(obj["TemplateID"]) == 104 || Convert.ToInt32(obj["TemplateID"]) == 105 || Convert.ToInt32(obj["TemplateID"]) == 106 || Convert.ToInt32(obj["TemplateID"]) == 107 || Convert.ToInt32(obj["TemplateID"]) == 113 || Convert.ToInt32(obj["TemplateID"]) == 114 || Convert.ToInt32(obj["TemplateID"]) == 115)
                                                {
                                                    if (Convert.ToInt32(obj["Associate"]) == 1)
                                                    {
                                                        if (Convert.ToInt32(entityIdForReference) == Convert.ToInt32(obj["EntiyParentID"]))
                                                            sb.Replace(match.Value, "");
                                                        else
                                                            sb.Replace(match.Value, "<a href='javascript:void(0);' data-id=\"feedpath\" data-parentid='" + obj["EntiyParentID"] + "'data-entityid='" + obj["EntiyParentID"] + "' data-typeid='" + "1" + "' >" + Convert.ToString(obj["ParentName"]) + "</a>");

                                                    }
                                                    else
                                                    {
                                                        if (Convert.ToInt32(entityIdForReference) == Convert.ToInt32(obj["EntityID"]))
                                                            sb.Replace(match.Value, "");
                                                        else
                                                            sb.Replace(match.Value, "<a href='javascript:void(0);'  data-id=\"taskpopupopen\" data-taskid='" + Convert.ToInt32(obj["EntityID"]) + "'  data-entityid='" + Convert.ToInt32(obj["EntiyParentID"]) + "' data-typeid='" + Convert.ToInt32(obj["Typeid"]) + "' >" + HttpUtility.HtmlDecode(entityname) + "</a>");
                                                        //sb.Replace(match.Value, "<a href='javascript:void(0);' data-id=\"feedpath\" data-parentid='" + obj["ParentID"] + "'data-entityid='" + obj["EntityID"] + "' data-typeid='" + obj["Typeid"] + "' >" + entityname + "</a>");
                                                    }

                                                }
                                                else
                                                {
                                                    if (Convert.ToInt32(obj["TemplateID"]) == 14 || Convert.ToInt32(obj["TemplateID"]) == 31 ||
                                                        Convert.ToInt32(obj["TemplateID"]) == 4 || Convert.ToInt32(obj["TemplateID"]) == 32 ||
                                                        Convert.ToInt32(obj["TemplateID"]) == 30 || Convert.ToInt32(obj["TemplateID"]) == 29 ||
                                                        Convert.ToInt32(obj["TemplateID"]) == 15 || Convert.ToInt32(obj["TemplateID"]) == 5 ||
                                                        Convert.ToInt32(obj["TemplateID"]) == 2 || Convert.ToInt32(obj["TemplateID"]) == 3 ||
                                                        Convert.ToInt32(obj["TemplateID"]) == 8 || Convert.ToInt32(obj["TemplateID"]) == 50 ||
                                                        Convert.ToInt32(obj["TemplateID"]) == 44 || Convert.ToInt32(obj["TemplateID"]) == 46 ||
                                                         Convert.ToInt32(obj["TemplateID"]) == 51 || Convert.ToInt32(obj["TemplateID"]) == 52 ||
                                                         Convert.ToInt32(obj["TemplateID"]) == 54 || Convert.ToInt32(obj["TemplateID"]) == 55 ||
                                                         Convert.ToInt32(obj["TemplateID"]) == 53 || Convert.ToInt32(obj["TemplateID"]) == 47)
                                                    {
                                                        if (Convert.ToInt32(entityIdForReference) == Convert.ToInt32(obj["EntityID"]))
                                                            sb.Replace(match.Value, "");
                                                        else
                                                            sb.Replace(match.Value, "<a href='javascript:void(0);' data-id=\"feedpath\" data-parentid='" + obj["ParentID"] + "'data-entityid='" + obj["EntityID"] + "' data-typeid='" + obj["Typeid"] + "' >" + entityname + "</a>");

                                                    }
                                                    //else if (Convert.ToInt32(obj["TemplateID"]) == 101 || Convert.ToInt32(obj["TemplateID"]) == 102 || Convert.ToInt32(obj["TemplateID"]) == 103 || Convert.ToInt32(obj["TemplateID"]) == 104 || Convert.ToInt32(obj["TemplateID"]) == 105 || Convert.ToInt32(obj["TemplateID"]) == 106 || Convert.ToInt32(obj["TemplateID"]) == 107 )
                                                    //{
                                                    //    if (Convert.ToInt32(obj["Associate"]) == 0)
                                                    //    {
                                                    //        if (Convert.ToInt32(entityIdForReference) == Convert.ToInt32(obj["ParentID"]))
                                                    //            sb.Replace(match.Value, "");
                                                    //        else
                                                    //            sb.Replace(match.Value, "<a href='javascript:void(0);' data-id=\"feedpath\" data-parentid='" + obj["ParentID"] + "'data-entityid='" + obj["EntityID"] + "' data-typeid='" + obj["Typeid"] + "' >" + entityname + "</a>");

                                                    //    }
                                                    //    else
                                                    //    {
                                                    //        if (Convert.ToInt32(entityIdForReference) == Convert.ToInt32(obj["EntityID"]))
                                                    //            sb.Replace(match.Value, "");
                                                    //        else
                                                    //            sb.Replace(match.Value, "<a href='javascript:void(0);'  data-id=\"taskpopupopen\" data-taskid='" + Convert.ToInt32(obj["EntityID"]) + "'  data-entityid='" + Convert.ToInt32(obj["EntiyParentID"]) + "' data-typeid='" + Convert.ToInt32(obj["Typeid"]) + "' >" + HttpUtility.HtmlDecode(entityname) + "</a>");
                                                    //        //sb.Replace(match.Value, "<a href='javascript:void(0);' data-id=\"feedpath\" data-parentid='" + obj["ParentID"] + "'data-entityid='" + obj["EntityID"] + "' data-typeid='" + obj["Typeid"] + "' >" + entityname + "</a>");
                                                    //    }

                                                    //}
                                                    else
                                                    {
                                                        if (Convert.ToInt32(entityIdForReference) == Convert.ToInt32(obj["EntityID"]) || ((Convert.ToInt32(entityIdForReference) == Convert.ToInt32(obj["EntiyParentID"])) && (Convert.ToInt32(obj["TemplateID"]) == 1) || Convert.ToInt32(obj["TemplateID"]) == 18))
                                                            sb.Replace(match.Value, "");
                                                        else
                                                            sb.Replace(match.Value, "<a href='javascript:void(0);' data-id=\"feedpath\" data-parentid='" + obj["ParentID"] + "'data-entityid='" + obj["EntityID"] + "' data-typeid='" + obj["Typeid"] + "' >" + Convert.ToString(obj["ParentName"]) + "</a>");
                                                    }
                                                }
                                                break;
                                            }
                                        case "@Users@":
                                            {

                                                if (Convert.ToInt32(obj["UserID"]) != 0)
                                                {
                                                    var user = (from tt in totalFeedUsers where tt.Id == Convert.ToInt32(obj["UserID"]) select new { tt.FirstName, tt.LastName }).FirstOrDefault();
                                                    sb.Replace(match.Value, user.FirstName + " " + user.LastName);
                                                }
                                                else
                                                {
                                                    sb.Replace(match.Value, "");
                                                }
                                                break;
                                            }
                                        case "@role@":
                                            {
                                                if (obj["ToValue"].ToString() != null && obj["ToValue"].ToString() != "-")
                                                {

                                                    var currententityroleobj = tx.PersistenceManager.CommonRepository.Query<BaseEntityDao>().Where(a => a.Id == Convert.ToInt32(obj["EntityID"])).SingleOrDefault();
                                                    var entitypeobj = tx.PersistenceManager.CommonRepository.Query<EntityTypeDao>().Where(a => a.Id == currententityroleobj.Typeid).SingleOrDefault();
                                                    if (entitypeobj.IsAssociate)
                                                    {
                                                        var role = (from tt in totalrole where tt.Id == Convert.ToInt32(obj["ToValue"]) select new { tt.Caption }).FirstOrDefault();
                                                        sb.Replace(match.Value, role.Caption);
                                                    }
                                                    else
                                                    {
                                                        var role = tx.PersistenceManager.CommonRepository.Get<EntityTypeRoleAclDao>(Convert.ToInt32(Convert.ToInt32(obj["ToValue"])));
                                                        sb.Replace(match.Value, role.Caption);

                                                    }
                                                }
                                                else
                                                {
                                                    sb.Replace(match.Value, "assignee(s)");
                                                }
                                                break;
                                            }
                                        case "@AttributeValue@":
                                            {
                                                if ((Convert.ToInt32(obj["TemplateID"]) == 20) || (Convert.ToInt32(obj["TemplateID"]) == 21) || (Convert.ToInt32(obj["TemplateID"]) == 6) || (Convert.ToInt32(obj["TemplateID"]) == 7) || (Convert.ToInt32(obj["TemplateID"]) == 9) || (Convert.ToInt32(obj["TemplateID"]) == 12) || (Convert.ToInt32(obj["TemplateID"]) == 58) || (Convert.ToInt32(obj["TemplateID"]) == 59))
                                                {
                                                    var formattedvalue = GetCurrency(Convert.ToInt32(obj["ToValue"]));
                                                    sb.Replace(match.Value, Convert.ToString(formattedvalue) + " " + proxy.MarcomManager.GlobalAdditionalSettings[1].CurrencyFormatvalue[0].ShortName);
                                                }


                                                else
                                                {
                                                    sb.Replace(match.Value, Convert.ToString(obj["ToValue"]));
                                                }
                                                break;
                                            }

                                        case "@Fromvalue@":
                                            {
                                                if ((Convert.ToInt32(obj["TemplateID"]) == 20) || (Convert.ToInt32(obj["TemplateID"]) == 21) || (Convert.ToInt32(obj["TemplateID"]) == 6) || (Convert.ToInt32(obj["TemplateID"]) == 7) || (Convert.ToInt32(obj["TemplateID"]) == 9) || (Convert.ToInt32(obj["TemplateID"]) == 12) || (Convert.ToInt32(obj["TemplateID"]) == 59))
                                                {


                                                    if (obj["FromValue"] == null || obj["FromValue"].ToString() == "")
                                                    {
                                                        sb.Replace(match.Value, "0");
                                                    }
                                                    else
                                                    {
                                                        var formattedvalue = GetCurrency(Convert.ToInt32(obj["FromValue"]));
                                                        sb.Replace(match.Value, Convert.ToString(formattedvalue) + " " + proxy.MarcomManager.GlobalAdditionalSettings[1].CurrencyFormatvalue[0].Symbol);
                                                    }
                                                }
                                                //else if ((Convert.ToInt32(obj["TemplateID"]) == 61))
                                                //{
                                                //    EntityDao tasktype = tx.PersistenceManager.TaskRepository.Query<EntityDao>().Where(x => x.Id == Convert.ToInt32(obj["FromValue"])).FirstOrDefault();
                                                //    sb.Replace(match.Value, "<a href='javascript:void(0);' data-id=\"feedpath\" data-parentid='" + Convert.ToInt32(obj["EntityParentID"]) + "'data-entityid='" + Convert.ToInt32(obj["FromValue"]) + "' data-typeid='" + Convert.ToInt32(obj["Typeid"]) + "' >" + tasktype.Name + "</a>");
                                                //}
                                                else
                                                {
                                                    sb.Replace(match.Value, Convert.ToString(obj["FromValue"]));
                                                }
                                                break;
                                            }
                                        case "@Tovalue@":
                                            {

                                                //if ((Convert.ToInt32(obj["TemplateID"]) == 61))
                                                //{
                                                //    EntityDao tasktype = tx.PersistenceManager.TaskRepository.Query<EntityDao>().Where(x => x.Id == Convert.ToInt32(obj["ToValue"])).FirstOrDefault();
                                                //    sb.Replace(match.Value, "<a href='javascript:void(0);' data-id=\"feedpath\" data-parentid='" + Convert.ToInt32(obj["EntityParentID"]) + "'data-entityid='" + Convert.ToInt32(obj["ToValue"]) + "' data-typeid='" + Convert.ToInt32(obj["Typeid"]) + "' >" + tasktype.Name + "</a>");
                                                //}
                                                //else
                                                //{
                                                //    sb.Replace(match.Value, Convert.ToString(obj["ToValue"]));
                                                //}
                                                sb.Replace(match.Value, Convert.ToString(obj["ToValue"]));
                                                break;
                                            }
                                        case "@TransferAmount@":
                                            {
                                                sb.Replace(match.Value, Convert.ToString(obj["AttributeName"]) + " " + proxy.MarcomManager.GlobalAdditionalSettings[1].CurrencyFormatvalue[0].Symbol);
                                                break;
                                            }
                                        case "@FromCC@":
                                            {
                                                var ccname1 = (from data1 in tx.PersistenceManager.PlanningRepository.Query<BaseEntityDao>() where data1.Id == Convert.ToInt32(obj["FromValue"]) select data1.Name).FirstOrDefault().ToString();
                                                sb.Replace(match.Value, "<a href='javascript:void(0);' data-id=\"costcenterlink\" data-costcentreid='" + Convert.ToInt32(obj["FromValue"]) + "'>" + ccname1 + "</a>");
                                                //sb.Replace(match.Value, Convert.ToString(obj["FromValue"]));
                                                break;
                                            }
                                        case "@ToCC@":
                                            {
                                                var ccname1 = (from data1 in tx.PersistenceManager.PlanningRepository.Query<BaseEntityDao>() where data1.Id == Convert.ToInt32(obj["ToValue"]) select data1.Name).FirstOrDefault().ToString();
                                                sb.Replace(match.Value, "<a href='javascript:void(0);' data-id=\"costcenterlink\" data-costcentreid='" + Convert.ToInt32(obj["ToValue"]) + "'>" + ccname1 + "</a>");

                                                //sb.Replace(match.Value, Convert.ToString(obj["ToValue"]));
                                                break;
                                            }
                                        case "@TaskStatus@":
                                            {
                                                sb.Replace(match.Value, Convert.ToString(obj["ToValue"]));
                                                break;
                                            }
                                        case "@checklistname@":
                                            {
                                                sb.Replace(match.Value, Convert.ToString(obj["AttributeName"]));
                                                break;
                                            }
                                        case "@taskTypeName@":
                                            {
                                                var taskdetails = (from tt in totalFeedEntities where tt.Id == Convert.ToInt32(obj["EntityID"]) select tt).FirstOrDefault();
                                                var tasktypename = (from type in totalFeedEntityTypes where type.Id == taskdetails.Typeid select type).FirstOrDefault();
                                                sb.Replace(match.Value, tasktypename.Caption);
                                                break;
                                            }
                                        case "@taskName@":
                                            {
                                                if ((Convert.ToInt32(obj["TemplateID"]) == 61))
                                                {

                                                    if (Convert.ToInt32(obj["AssocitedEntityID"]) > 0)
                                                    {
                                                        var entitytaskdetails = (from tt in tx.PersistenceManager.PlanningRepository.Query<EntityDao>() where tt.Id == Convert.ToInt32(obj["AssocitedEntityID"]) select tt).FirstOrDefault();
                                                        //var entitytasktypename = (from type in tx.PersistenceManager.PlanningRepository.Query<EntityTypeDao>() where type.Id == entitytaskdetails.Typeid select type).FirstOrDefault();

                                                        sb.Replace(match.Value, "<a href='javascript:void(0);'  data-id=\"taskpopupopen\" data-taskid='" + Convert.ToInt32(obj["AssocitedEntityID"]) + "'  data-entityid='" + entitytaskdetails.Parentid + "' data-typeid='" + entitytaskdetails.Typeid + "' >" + HttpUtility.HtmlDecode(entitytaskdetails.Name) + "</a>");
                                                    }

                                                }

                                                else
                                                {
                                                    if (Convert.ToInt32(entityIdForReference) == Convert.ToInt32(obj["EntityID"]))
                                                        sb.Replace(match.Value, "");
                                                    else
                                                    {
                                                        sb.Replace(match.Value, "<a href='javascript:void(0);'  data-id=\"taskpopupopen\" data-taskid='" + Convert.ToInt32(obj["EntityID"]) + "'  data-entityid='" + Convert.ToInt32(obj["EntiyParentID"]) + "' data-typeid='" + Convert.ToInt32(obj["Typeid"]) + "' >" + HttpUtility.HtmlDecode(entityname) + "</a>");
                                                    }
                                                }
                                                break;
                                            }

                                        case "@checkliststatus@":
                                            {
                                                sb.Replace(match.Value, Convert.ToString(obj["ToValue"]));
                                                break;
                                            }
                                        case "@filename@":
                                            {
                                                var attachdeils = (from tt in tx.PersistenceManager.CommonRepository.Query<AttachmentsDao>() where tt.Id == Convert.ToInt32(obj["ToValue"]) select tt).FirstOrDefault();
                                                FileDao filedetails = null;
                                                if (attachdeils != null)
                                                {
                                                    filedetails = (from tt1 in tx.PersistenceManager.CommonRepository.Query<FileDao>() where tt1.Id == attachdeils.ActiveFileid select tt1).FirstOrDefault();
                                                }
                                                if (filedetails == null)
                                                    sb.Replace(match.Value, Convert.ToString(obj["AttributeName"]));
                                                else
                                                    sb.Replace(match.Value, "<a target=\"_blank\" href=\"download.aspx?FileID=" + filedetails.Fileguid + "&amp;FileFriendlyName=" + Convert.ToString(filedetails.Name) + "&amp;Ext=" + filedetails.Extension + "\">" + Convert.ToString(filedetails.Name) + "</a>");
                                                break;
                                            }
                                        case "@linkname@":
                                            {
                                                var linkdetails = (from tt in tx.PersistenceManager.CommonRepository.Query<LinksDao>() where tt.ID == Convert.ToInt32(obj["ToValue"]) select tt).FirstOrDefault();
                                                if (linkdetails == null)
                                                    sb.Replace(match.Value, Convert.ToString(obj["AttributeName"]));
                                                else
                                                    sb.Replace(match.Value, "<a href=\"javascript:void(0);\" data-name=\"" + linkdetails.URL + "\"  data-id=\"" + linkdetails.ID + "\" data-command=\"openlink\">" + Convert.ToString(linkdetails.Name) + "</a>");

                                                break;
                                            }

                                        case "@TasksMembers@":
                                            {
                                                string commaseperatedusers = "";

                                                foreach (var c in obj["ToValue"].ToString().Split(','))
                                                {
                                                    if (c != "")
                                                    {
                                                        var user1 = tx.PersistenceManager.AccessRepository.Get<UserDao>(Convert.ToInt32(c));
                                                        if (user1.Id == Convert.ToInt32(obj["Actor"]))
                                                        {
                                                            commaseperatedusers = commaseperatedusers + "," + "Oneself";
                                                        }
                                                        else
                                                        {
                                                            commaseperatedusers = commaseperatedusers + ',' + user1.FirstName + ' ' + user1.LastName;

                                                        }
                                                        commaseperatedusers = commaseperatedusers.TrimStart().TrimStart(',');
                                                    }
                                                }
                                                sb.Replace(match.Value, commaseperatedusers);
                                                break;
                                            }
                                        case "@AssetName@":
                                            {

                                                int assetid = 0;
                                                //if (Convert.ToInt32(obj["TemplateID"]) == 109 || Convert.ToInt32(obj["TemplateID"]) == 110)

                                                //    assetid = Convert.ToInt32(obj["AssetId"]);
                                                //else
                                                assetid = Convert.ToInt32(obj["AssocitedEntityID"]);
                                                var Assetdetails = (from tt in tx.PersistenceManager.CommonRepository.Query<AssetsDao>() where tt.ID == assetid select tt).FirstOrDefault();
                                                if (Assetdetails == null)
                                                    sb.Replace(match.Value, Convert.ToString(obj["AttributeName"]));
                                                else
                                                    //sb.Replace(match.Value, Assetdetails.Name);

                                                    sb.Replace(match.Value, "<a href='javascript:void(0);' data-id=\"Assetpath\"data-entityid='" + Assetdetails.EntityID + "'data-Assetid='" + Assetdetails.ID + "' data-typeid='" + obj["Typeid"] + "' >" + Assetdetails.Name + "</a>");
                                                break;
                                            }

                                        case "@VersionNo@":
                                            {



                                                if (Convert.ToString(obj["Version"]) != null)
                                                    sb.Replace(match.Value, Convert.ToString(obj["Version"]));
                                                else
                                                    sb.Replace(match.Value, "0");


                                                break;
                                            }
                                        case "@AssetFileName@":
                                            {

                                                if (obj["ToValue"] != null)
                                                {
                                                    var Filedetails = (from tt in tx.PersistenceManager.CommonRepository.Query<DAMFileDao>() where tt.ID == Convert.ToInt32(obj["ToValue"]) select tt).FirstOrDefault();
                                                    //var Assetdetails = (from tt in tx.PersistenceManager.CommonRepository.Query<AssetsDao>() where tt.ID == Convert.ToInt32(obj["AssocitedEntityID"]) select tt).FirstOrDefault();
                                                    if (Filedetails == null)
                                                        sb.Replace(match.Value, Convert.ToString(obj["AttributeName"]));
                                                    else
                                                        sb.Replace(match.Value, Filedetails.Name);
                                                }
                                                else
                                                {
                                                    sb.Replace(match.Value, "-");
                                                }

                                                //sb.Replace(match.Value, "<a href='javascript:void(0);' data-id=\"Assetpath\"data-entityid='" + Assetdetails.EntityID + "'data-Assetid='" + Assetdetails.ID + "' data-typeid='" + obj["Typeid"] + "' >" + Assetdetails.Name + "</a>");
                                                break;
                                            }

                                    }
                                }

                                feedObj.FeedText = Convert.ToString(sb).Trim();
                                while (feedObj.FeedText.EndsWith("in"))
                                    feedObj.FeedText = feedObj.FeedText.Substring(0, feedObj.FeedText.Length - 2).Trim();
                                while (feedObj.FeedText.EndsWith("of"))
                                    feedObj.FeedText = feedObj.FeedText.Substring(0, feedObj.FeedText.Length - 2).Trim();

                                while (feedObj.FeedText.EndsWith("from"))
                                    feedObj.FeedText = feedObj.FeedText.Substring(0, feedObj.FeedText.Length - 4).Trim();


                                IList<IFeedComment> listOfFeedComment = new List<IFeedComment>();
                                foreach (var objcomment in GetFeedcomments)
                                {
                                    FeedComment feedCommentObj = new FeedComment();
                                    if (Convert.ToInt32(objcomment["FeedID"]) == Convert.ToInt32(obj["ID"]))
                                    {
                                        feedCommentObj.Id = Convert.ToInt32(objcomment["ID"]);
                                        feedCommentObj.Feedid = Convert.ToInt32(objcomment["FeedID"]);
                                        feedCommentObj.Comment = Convert.ToString(objcomment["Comment"]);
                                        feedCommentObj.UserName = Convert.ToString(objcomment["UserFirstName"] + " " + objcomment["UserLastName"]);
                                        feedCommentObj.Usermail = Convert.ToString(objcomment["UserEmail"]);
                                        feedCommentObj.Actor = Convert.ToInt32(objcomment["Actor"]);

                                        TimeSpan difference1 = (proxy.MarcomManager.UserManager.FeedInitialRequestedTime - DateTime.Parse(objcomment["CommentedOn"].ToString()));
                                        if (difference1.Days > 0)
                                            if (difference1.Days > 1)
                                                feedCommentObj.CommentedOn = (DateTime.Parse(objcomment["CommentedOn"].ToString()) + offSet).ToString(dateformate);
                                            else
                                                feedCommentObj.CommentedOn = "Yesterday at " + (DateTime.Parse(objcomment["CommentedOn"].ToString()) + offSet).ToShortTimeString();
                                        else if (difference1.Hours > 0)
                                            if (difference1.Days < 2)
                                                feedCommentObj.CommentedOn = "about an hour ago";
                                            else
                                                feedCommentObj.CommentedOn = difference1.Hours + " hours ago";
                                        else if (difference1.Minutes > 0)
                                            feedCommentObj.CommentedOn = difference1.Minutes + " minutes ago";
                                        else
                                            feedCommentObj.CommentedOn = "Few seconds ago";


                                        listOfFeedComment.Add(feedCommentObj);

                                    }
                                    feedObj.FeedComment = listOfFeedComment;
                                }

                                listfeedselection.Add(feedObj);
                            }
                            catch (Exception ex)
                            {
                            }
                            //2nd for to be closed
                        }
                        tx.Commit();
                        if (isForRealTimeUpdate && listfeedselection.Count > 0)
                        {
                            proxy.MarcomManager.UserManager.FeedRecentlyUpdatedTime = retreiveTime;
                        }
                        return listfeedselection;

                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                proxy.MarcomManager.UserManager.OverviewFeedLock = false;
            }
        }

        public object GetCurrency(int srcnumber)
        {
            string retCommaString = "";
            try
            {
                string getstr = null;
                getstr = srcnumber + "";
                int count = 0;
                string commaString = "";


                int i = 0;
                int j = 0;

                for (i = getstr.Length - 1; i >= 0; i += -1)
                {

                    if (count == 3)
                    {
                        commaString = commaString + " ";
                        count = 0;
                    }
                    commaString = commaString + getstr[i];


                    count = count + 1;

                }
                for (j = commaString.Length - 1; j >= 0; j += -1)
                {
                    retCommaString = retCommaString + commaString[j];
                }

            }
            catch (Exception ex)
            {
                return null;
            }
            return retCommaString;
        }



        // <summary>
        /// GettingFeedsByEntityID
        /// </summary>
        /// <param name="proxy"></param>
        /// <param name="AssetId"></param>
        /// <param name="pageNo">If it is real time update, page number is not applicable (-1) </param>
        /// <param name="isForRealTimeUpdate"></param>
        /// <returns></returns>
        public IList<IFeedSelection> GettingFeedsByAsset(CommonManagerProxy proxy, int AssetId, int pageNo, bool isForRealTimeUpdate)
        {
            try
            {
                if (proxy.MarcomManager.UserManager.AssetFeedLock)
                    return new List<IFeedSelection>();

                proxy.MarcomManager.UserManager.AssetFeedLock = true;
                if (pageNo == 0)
                {
                    proxy.MarcomManager.UserManager.AssetFeedInitialRequestedTime = DateTimeOffset.UtcNow;
                    proxy.MarcomManager.UserManager.AssetFeedRecentlyUpdatedTime = DateTimeOffset.UtcNow;
                }
                if (pageNo > 0)
                {
                    pageNo = pageNo * 10;
                }

                //proxy.MarcomManager.CommonManager
                // var assetTypeid1 = proxy.MarcomManager.CommonManager.CommonRepository.Get<BrandSystems.Marcom.Dal.DAM.Model.AssetsDao>(AssetId);

                IList<IFeedSelection> listfeedselection = new List<IFeedSelection>();
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    var feedSelectQuery = new StringBuilder();
                    //direct  asset action 
                    feedSelectQuery.Append(" SELECT * FROM  ( ");
                    if (AssetId != 0)
                        feedSelectQuery.Append("select cmf.ID,cmf.Actor,cmf.UserID, cmf.TemplateID,cmf.HappenedOn,cmf.AssocitedEntityID,cmf.CommentedUpdatedOn,cmf.EntityID,cmf.TypeName," +
                                             "cmf.AttributeName,cmf.FromValue,cmf.ToValue,pme.Name as 'EntityName',pme.UniqueKey as 'EntiyUniquekey',pme.TypeId as 'Typeid',pme.ParentID 'EntiyParentID', isnull(parentEnt.Name,'-') as 'ParentName'," +
                                             "umuse.FirstName as 'UserFirstName',umuse.LastName 'UserLastName',umuse.Email as 'UserEmail',umuse.Image as 'UserImage'," +
                                             "umuse.TimeZone as 'UserTimeZone',cmt.Template as 'FeedTemplate',da.Name AS 'AssetName',da.ID AS 'AssetId',cmf.Version as 'Version'  from CM_Feed cmf inner join PM_Entity pme on cmf.EntityID = pme.ID  left OUTER JOIN PM_Entity parentEnt on pme.ParentID = parentEnt.ID  inner join UM_User umuse on" +
                                             " umuse.ID = cmf.Actor inner join CM_Feed_Template cmt on cmt.ID = cmf.TemplateID  INNER JOIN DAM_Asset da ON  da.ID = cmf.AssocitedEntityID  where ( cmf.AssocitedEntityID = " + AssetId +
                                             " AND  cmt.ModuleID= 5 )  and cmf.HappenedOn ");
                    else
                        feedSelectQuery.Append("select cmf.ID,cmf.Actor,cmf.UserID,cmf.TemplateID,cmf.HappenedOn,cmf.CommentedUpdatedOn,cmf.EntityID,cmf.TypeName," +
                                                 "cmf.AttributeName,cmf.FromValue,cmf.ToValue,pme.Name as 'EntityName',pme.UniqueKey as 'EntiyUniquekey',pme.TypeId as 'Typeid',pme.ParentID 'EntiyParentID'," +
                                                 "umuse.FirstName as 'UserFirstName',umuse.LastName 'UserLastName',umuse.Email as 'UserEmail',umuse.Image as 'UserImage'," +
                                                 "umuse.TimeZone as 'UserTimeZone',cmt.Template as 'FeedTemplate' 'FeedTemplate',da.Name AS 'AssetName',da.ID AS 'AssetId' ,cmf.Version as 'Version'  from CM_Feed cmf inner join PM_Entity pme on cmf.EntityID = pme.ID inner join UM_User umuse on" +
                                           " umuse.ID = cmf.Actor inner join CM_Feed_Template cmt on cmt.ID = cmf.TemplateID  INNER JOIN DAM_Asset da ON  da.ID = cmf.AssocitedEntityID  where (cmf.Actor =" + proxy.MarcomManager.User.Id + " AND  cmt.ModuleID= 5 ) and cmf.HappenedOn ");
                    if (isForRealTimeUpdate)
                        feedSelectQuery.Append(">= '" + (proxy.MarcomManager.UserManager.AssetFeedRecentlyUpdatedTime).ToString("yyyy-MM-dd HH:mm:ss.FFFFFFF") + "'");
                    else
                        feedSelectQuery.Append("<= '" + (proxy.MarcomManager.UserManager.AssetFeedInitialRequestedTime).ToString("yyyy-MM-dd HH:mm:ss.FFFFFFF") + "'");
                    //feedSelectQuery.Append("<= '" + (proxy.MarcomManager.UserManager.AssetFeedInitialRequestedTime).ToString("yyyy-MM-dd HH:mm:ss.FFFFFFF") + "'" + " ORDER BY cmf.HappenedOn desc OFFSET " + pageNo + " ROWS FETCH NEXT 20 ROWS ONLY");

                    ///asset Task action

                    feedSelectQuery.Append(" UNION  ");

                    if (AssetId != 0)
                        feedSelectQuery.Append("select cmf.ID,cmf.Actor,cmf.UserID, cmf.TemplateID,cmf.HappenedOn,cmf.AssocitedEntityID,cmf.CommentedUpdatedOn,cmf.EntityID,cmf.TypeName," +
                                             "cmf.AttributeName,cmf.FromValue,cmf.ToValue,pme.Name as 'EntityName',pme.UniqueKey as 'EntiyUniquekey',pme.TypeId as 'Typeid',pme.ParentID 'EntiyParentID', parentEnt.Name as 'ParentName'," +
                                             "umuse.FirstName as 'UserFirstName',umuse.LastName 'UserLastName',umuse.Email as 'UserEmail',umuse.Image as 'UserImage'," +
                                             "umuse.TimeZone as 'UserTimeZone',cmt.Template as 'FeedTemplate',da.Name AS 'AssetName',da.ID AS 'AssetId',cmf.Version as 'Version' from CM_Feed cmf inner join PM_Entity pme on cmf.EntityID = pme.ID inner join PM_Entity parentEnt on pme.ParentID = parentEnt.ID  inner join UM_User umuse on" +
                                             " umuse.ID = cmf.Actor inner join CM_Feed_Template cmt on cmt.ID = cmf.TemplateID  LEFT OUTER JOIN TM_EntityTask tt ON  tt.ID = cmf.EntityID INNER  JOIN DAM_Asset da  ON  da.ID =  tt.AssetId  where ( tt.AssetId =" + AssetId +
                                             " AND  cmt.ModuleID= 3 AND cmf.TemplateID >100) and cmf.HappenedOn ");
                    else
                        feedSelectQuery.Append("select cmf.ID,cmf.Actor,cmf.UserID,cmf.TemplateID,cmf.HappenedOn,cmf.CommentedUpdatedOn,cmf.EntityID,cmf.TypeName," +
                                                 "cmf.AttributeName,cmf.FromValue,cmf.ToValue,pme.Name as 'EntityName',pme.UniqueKey as 'EntiyUniquekey',pme.TypeId as 'Typeid',pme.ParentID 'EntiyParentID'," +
                                                 "umuse.FirstName as 'UserFirstName',umuse.LastName 'UserLastName',umuse.Email as 'UserEmail',umuse.Image as 'UserImage'," +
                                                 "umuse.TimeZone as 'UserTimeZone',cmt.Template as 'FeedTemplate',da.Name AS 'AssetName',da.ID AS 'AssetId',cmf.Version as 'Version' from CM_Feed cmf inner join PM_Entity pme on cmf.EntityID = pme.ID inner join UM_User umuse on" +
                                           " umuse.ID = cmf.Actor inner join CM_Feed_Template cmt on cmt.ID = cmf.TemplateID  LEFT OUTER JOIN TM_EntityTask tt ON  tt.ID = cmf.EntityID INNER  JOIN DAM_Asset da  ON  da.ID =  tt.AssetId  where (cmf.Actor =" + proxy.MarcomManager.User.Id + " AND  cmt.ModuleID= 3 AND cmf.TemplateID >100 ) and cmf.HappenedOn ");
                    if (isForRealTimeUpdate)
                        feedSelectQuery.Append(">= '" + (proxy.MarcomManager.UserManager.AssetFeedRecentlyUpdatedTime).ToString("yyyy-MM-dd HH:mm:ss.FFFFFFF") + "'");
                    else
                        feedSelectQuery.Append("<= '" + (proxy.MarcomManager.UserManager.AssetFeedInitialRequestedTime).ToString("yyyy-MM-dd HH:mm:ss.FFFFFFF") + "'");
                    //feedSelectQuery.Append("<= '" + (proxy.MarcomManager.UserManager.AssetFeedInitialRequestedTime).ToString("yyyy-MM-dd HH:mm:ss.FFFFFFF") + "'" + " ORDER BY cmf.HappenedOn desc OFFSET " + pageNo + " ROWS FETCH NEXT 20 ROWS ONLY");


                    if (isForRealTimeUpdate)
                        feedSelectQuery.Append(") feed ");
                    else
                        feedSelectQuery.Append(") feed ORDER BY feed.HappenedOn desc OFFSET " + pageNo + " ROWS FETCH NEXT 20 ROWS ONLY");

                    var childEntiyResult = ((tx.PersistenceManager.CommonRepository.ExecuteQuery(feedSelectQuery.ToString())).Cast<Hashtable>().ToList());
                    TimeSpan offSet = TimeSpan.Parse(proxy.MarcomManager.User.TimeZone.TrimStart('+'));

                    //  ----------- Getting the comments for the list of feeds  ------------------------------
                    var asset = tx.PersistenceManager.CommonRepository.Get<BrandSystems.Marcom.Dal.DAM.Model.AssetsDao>(AssetId);

                    ArrayList arrFeedId = new ArrayList();

                    foreach (var obj in childEntiyResult)
                    {
                        arrFeedId.Add(Convert.ToInt32(obj["ID"]));
                    }

                    string arrFeedIdRes = "";
                    if (arrFeedId.Count > 0)
                    {
                        arrFeedIdRes = string.Join(",", arrFeedId.ToArray());
                    }
                    feedSelectQuery.Clear();


                    if (arrFeedIdRes == "")
                    {
                        feedSelectQuery.Append("select cmfcom.ID,cmfcom.FeedID,cmfcom.Comment,cmfcom.CommentedOn,cmfcom.Actor," +
                         "umuse.FirstName as 'UserFirstName',umuse.LastName as 'UserLastName',umuse.Email as 'UserEmail',umuse.Image as 'UserImage'," +
                         "umuse.TimeZone as 'UserTimeZone' from  CM_Feed_Comment cmfcom inner join UM_User umuse on" +
                         " umuse.ID = cmfcom.Actor where cmfcom.FeedID IN  " +
                         "('') order by cmfcom.CommentedOn asc");
                    }
                    else
                    {
                        feedSelectQuery.Append("select cmfcom.ID,cmfcom.FeedID,cmfcom.Comment,cmfcom.CommentedOn,cmfcom.Actor," +
                         "umuse.FirstName as 'UserFirstName',umuse.LastName as 'UserLastName',umuse.Email as 'UserEmail',umuse.Image as 'UserImage'," +
                         "umuse.TimeZone as 'UserTimeZone' from  CM_Feed_Comment cmfcom inner join UM_User umuse on" +
                         " umuse.ID = cmfcom.Actor where cmfcom.FeedID IN  " +
                         "(" + arrFeedIdRes + ") order by cmfcom.CommentedOn asc");
                    }

                    var GetFeedcomments = ((tx.PersistenceManager.CommonRepository.ExecuteQuery(feedSelectQuery.ToString())).Cast<Hashtable>().ToList());

                    //---------------------------------------------------------------
                    string dateformate = proxy.MarcomManager.GlobalAdditionalSettings[0].SettingValue.ToString().Replace('m', 'M');
                    dateformate += " hh:mm:ss tt";

                    var entityIDArr = childEntiyResult.Cast<Hashtable>().Select(a => (int)a["EntityID"]).Distinct().ToArray();

                    //total entities associated for newsfeed
                    var totalFeedEntities = (from tt in tx.PersistenceManager.PlanningRepository.Query<EntityDao>() where entityIDArr.Contains(tt.Id) select new { tt.Typeid, tt.Name, tt.Id, tt.Parentid }).ToList();

                    //total entitytype associated for this newsfeed 
                    var totalFeedEntityTypes = (from ent in totalFeedEntities
                                                join entTp in tx.PersistenceManager.PlanningRepository.GetAll<EntityTypeDao>() on ent.Typeid equals entTp.Id
                                                orderby entTp.Id
                                                select new
                                                {
                                                    entTp.Id,
                                                    entTp.Caption
                                                }).ToList();

                    //total task entities associated for newsfeed
                    var totalFeedTaskEntities = (from tt in tx.PersistenceManager.PlanningRepository.Query<EntityTaskDao>() where entityIDArr.Contains(tt.ID) select tt).ToList<EntityTaskDao>();



                    foreach (var obj in childEntiyResult)
                    {
                        FeedSelection feedObj = new FeedSelection();
                        feedObj.FeedId = Convert.ToInt32(obj["ID"]);
                        feedObj.UserName = Convert.ToString(obj["UserFirstName"] + " " + obj["UserLastName"]);
                        feedObj.UserEmail = Convert.ToString(obj["UserEmail"]);
                        feedObj.UserImage = Convert.ToString(obj["UserImage"]);
                        feedObj.Actor = Convert.ToInt32(obj["Actor"]);

                        var typename = Convert.ToString(obj["TypeName"]);
                        var entityname = Convert.ToString(obj["EntityName"]);

                        UserDao user = new UserDao();
                        user = tx.PersistenceManager.CommonRepository.Get<UserDao>(obj["UserID"]);

                        TimeSpan difference = (proxy.MarcomManager.UserManager.AssetFeedInitialRequestedTime - DateTime.Parse(obj["HappenedOn"].ToString()));
                        if (difference.Days > 0)
                            if (difference.Days > 1)
                                feedObj.FeedHappendTime = (DateTime.Parse(obj["HappenedOn"].ToString()) + offSet).ToString(dateformate);
                            else
                                feedObj.FeedHappendTime = "Yesterday at " + (DateTime.Parse(obj["HappenedOn"].ToString()) + offSet).ToShortTimeString();
                        else if (difference.Hours > 0)
                            if (difference.Hours < 2)
                                feedObj.FeedHappendTime = "about an hour ago";
                            else
                                feedObj.FeedHappendTime = difference.Hours + " hours ago";
                        else if (difference.Minutes > 0)
                            feedObj.FeedHappendTime = difference.Minutes + " minutes ago";
                        else
                            feedObj.FeedHappendTime = "Few seconds ago";

                        string template = Convert.ToString(obj["FeedTemplate"]);


                        if (template.Trim().EndsWith("in @Path@"))
                        {
                            template = template.Replace("in @Path@", " ");
                        }
                        if (template.Trim().EndsWith("in @EntityNamefortask@"))
                        {
                            template = template.Replace("in @EntityNamefortask@", " ");
                        }
                        if (template.Trim().EndsWith("in @taskTypeName@ @EntityNamefortask@"))
                        {
                            template = template.Replace("in @taskTypeName@ @EntityNamefortask@", " ");
                        }
                        if (template.Trim().EndsWith("in @EntityTypeName@ @EntityName@"))
                        {
                            template = template.Replace("in @EntityTypeName@ @EntityName@", " ");
                        }
                        //if (template.Trim().EndsWith("in @taskTypeName@ @taskName@"))
                        //{
                        //    template = template.Replace("in @taskTypeName@ @taskName@", " ");
                        //}
                        if (template.Trim().Contains("reinitiated @taskTypeName@ @taskName@ in @EntityTypeName@ @EntityName@"))
                        {
                            template = template.Replace("@taskTypeName@ @taskName@ in @EntityTypeName@ @EntityName@", " ");
                        }
                        //if (template.Trim().Contains("of @taskTypeName@ @taskName@"))
                        //{
                        //    template = template.Replace("of @taskTypeName@ @taskName@", " ");
                        //}
                        if (Convert.ToInt32(obj["TemplateID"]) == 38)
                        {
                            template = template.Replace("@taskName@", " ");
                        }

                        if (template.Trim().EndsWith("from @Path@"))
                        {
                            template = template.Replace("from @Path@", " ");
                        }

                        //foreach(Match  mm in Regex.Matches(template,@"(?<=\@#)(.*?)(?=\@#)"))
                        //{
                        //    template = template.Replace(mm.ToString(),"").Replace("@#@#@","").ToString();
                        //}
                        StringBuilder sb = new StringBuilder(template);
                        foreach (Match match in Regex.Matches(template, @"@(.+?)@"))
                        {
                            try
                            {
                                switch (match.Value.Trim())
                                {

                                    //case "@EntityTypeName@":
                                    //    {
                                    //        //  sb.Replace(match.Value, typename);
                                    //        //if (Convert.ToInt32(obj["TemplateID"]) == 22)
                                    //        //{
                                    //        //    sb.Replace(match.Value, obj["AttributeName"].ToString());
                                    //        //}
                                    //        //if (Convert.ToInt32(obj["TemplateID"]) == 24 || Convert.ToInt32(obj["TemplateID"]) == 25 || Convert.ToInt32(obj["TemplateID"]) == 26 || Convert.ToInt32(obj["TemplateID"]) == 27 || Convert.ToInt32(obj["TemplateID"]) == 28 || Convert.ToInt32(obj["TemplateID"]) == 31 || Convert.ToInt32(obj["TemplateID"]) == 32 || Convert.ToInt32(obj["TemplateID"]) == 33 || Convert.ToInt32(obj["TemplateID"]) == 34 || Convert.ToInt32(obj["TemplateID"]) == 36)
                                    //        //{
                                    //        //    var entitytaskdetails = (from tt in tx.PersistenceManager.PlanningRepository.Query<EntityDao>() where tt.Id == Convert.ToInt32(obj["AssocitedEntityID"]) select tt).FirstOrDefault();
                                    //        //    var entitytasktypename = (from type in tx.PersistenceManager.PlanningRepository.Query<EntityTypeDao>() where type.Id == entitytaskdetails.Typeid select type).FirstOrDefault();
                                    //        //    sb.Replace(match.Value, entitytasktypename.Caption);
                                    //        //}

                                    //        if (Convert.ToInt32(obj["TemplateID"]) == 22)
                                    //        {
                                    //            sb.Replace(match.Value, obj["AttributeName"].ToString());
                                    //        }
                                    //        else if (Convert.ToInt32(obj["TemplateID"]) == 24 || Convert.ToInt32(obj["TemplateID"]) == 25 || Convert.ToInt32(obj["TemplateID"]) == 26 || Convert.ToInt32(obj["TemplateID"]) == 27 || Convert.ToInt32(obj["TemplateID"]) == 28 || Convert.ToInt32(obj["TemplateID"]) == 31 || Convert.ToInt32(obj["TemplateID"]) == 32 || Convert.ToInt32(obj["TemplateID"]) == 33 || Convert.ToInt32(obj["TemplateID"]) == 34 || Convert.ToInt32(obj["TemplateID"]) == 35 || Convert.ToInt32(obj["TemplateID"]) == 36 || Convert.ToInt32(obj["TemplateID"]) == 13 || Convert.ToInt32(obj["TemplateID"]) == 17 || Convert.ToInt32(obj["TemplateID"]) == 22 || Convert.ToInt32(obj["TemplateID"]) == 40)
                                    //        {
                                    //            if (entityId == Convert.ToInt32(obj["EntiyParentID"]))
                                    //                sb.Replace(match.Value, "");
                                    //            else
                                    //            {
                                    //                if (entityId == Convert.ToInt32(obj["EntiyParentID"]))
                                    //                    sb.Replace(match.Value, "");
                                    //                else
                                    //                {
                                    //                    var entitytaskdetails = (from tt in tx.PersistenceManager.PlanningRepository.Query<EntityDao>() where tt.Id == Convert.ToInt32(obj["AssocitedEntityID"]) select tt).FirstOrDefault();
                                    //                    var entitytasktypename = (from type in tx.PersistenceManager.PlanningRepository.Query<EntityTypeDao>() where type.Id == entitytaskdetails.Typeid select type).FirstOrDefault();
                                    //                    sb.Replace(match.Value, entitytasktypename.Caption);
                                    //                }
                                    //            }
                                    //        }
                                    //        else
                                    //        {
                                    //            if (entityId == Convert.ToInt32(obj["EntityID"]) && obj["TemplateID"].ToString() == "1")
                                    //                sb.Replace("new", "this");
                                    //            sb.Replace(match.Value, typename);
                                    //        }

                                    //        break;
                                    //    }
                                    //case "@EntityName@":
                                    //    {
                                    //        //if (Convert.ToInt32(obj["TemplateID"]) == 24 || Convert.ToInt32(obj["TemplateID"]) == 25 || Convert.ToInt32(obj["TemplateID"]) == 26 || Convert.ToInt32(obj["TemplateID"]) == 27 || Convert.ToInt32(obj["TemplateID"]) == 28 || Convert.ToInt32(obj["TemplateID"]) == 31 || Convert.ToInt32(obj["TemplateID"]) == 32 || Convert.ToInt32(obj["TemplateID"]) == 33 || Convert.ToInt32(obj["TemplateID"]) == 34 || Convert.ToInt32(obj["TemplateID"]) == 36)
                                    //        //{
                                    //        //    var entitytaskdetails = (from tt in tx.PersistenceManager.PlanningRepository.Query<EntityDao>() where tt.Id == Convert.ToInt32(obj["AssocitedEntityID"]) select tt).FirstOrDefault();
                                    //        //    sb.Replace(match.Value, entitytaskdetails.Name);
                                    //        //}
                                    //        //else
                                    //        //{
                                    //        //    sb.Replace(match.Value, entityname);
                                    //        //}


                                    //        if (Convert.ToInt32(obj["TemplateID"]) == 24 || Convert.ToInt32(obj["TemplateID"]) == 25 || Convert.ToInt32(obj["TemplateID"]) == 26 || Convert.ToInt32(obj["TemplateID"]) == 27 || Convert.ToInt32(obj["TemplateID"]) == 28 || Convert.ToInt32(obj["TemplateID"]) == 31 || Convert.ToInt32(obj["TemplateID"]) == 32 || Convert.ToInt32(obj["TemplateID"]) == 33 || Convert.ToInt32(obj["TemplateID"]) == 34 || Convert.ToInt32(obj["TemplateID"]) == 35 || Convert.ToInt32(obj["TemplateID"]) == 36 || Convert.ToInt32(obj["TemplateID"]) == 13 || Convert.ToInt32(obj["TemplateID"]) == 17 || Convert.ToInt32(obj["TemplateID"]) == 22 || Convert.ToInt32(obj["TemplateID"]) == 40)
                                    //        {
                                    //            if (entityId == Convert.ToInt32(obj["ParentID"]))
                                    //                sb.Replace(match.Value, "").Replace("in", "");
                                    //            else
                                    //            {
                                    //                var entitytaskdetails = (from tt in tx.PersistenceManager.PlanningRepository.Query<EntityDao>() where tt.Id == Convert.ToInt32(obj["AssocitedEntityID"]) select tt).FirstOrDefault();
                                    //                // sb.Replace(match.Value, entitytaskdetails.Name);
                                    //                sb.Replace(match.Value, "<a href='javascript:void(0);' data-id=\"feedpath\" data-parentid='" + entitytaskdetails.Parentid + "'data-entityid='" + entitytaskdetails.Id + "' data-typeid='" + entitytaskdetails.Typeid + "' >" + entitytaskdetails.Name + "</a>");
                                    //            }
                                    //        }
                                    //        else
                                    //        {
                                    //            if (entityId == Convert.ToInt32(obj["EntityID"]))
                                    //                sb.Replace(match.Value, "").Replace("in", "");
                                    //            else
                                    //            {

                                    //                if (Convert.ToInt32(obj["TemplateID"]) == 6)
                                    //                {
                                    //                    sb.Replace(match.Value, "<a href='javascript:void(0);'  data-parentid='" + obj["EntiyParentID"] + "'data-entityid='" + obj["EntityID"] + "' data-typeid='" + obj["Typeid"] + "' >" + entityname + "</a>");
                                    //                }
                                    //                sb.Replace(match.Value, "<a href='javascript:void(0);' data-id=\"feedpath\" data-parentid='" + obj["ParentID"] + "'data-entityid='" + obj["EntityID"] + "' data-typeid='" + obj["Typeid"] + "' >" + entityname + "</a>");
                                    //            }

                                    //        }
                                    //        break;
                                    //    }
                                    case "@AttributeValue@":
                                        {
                                            if (Convert.ToInt32(obj["TemplateID"]) == 39)
                                            {
                                                sb.Replace(match.Value, Convert.ToString(obj["ToValue"]));
                                            }
                                            if ((Convert.ToInt32(obj["TemplateID"]) == 20) || (Convert.ToInt32(obj["TemplateID"]) == 21) || (Convert.ToInt32(obj["TemplateID"]) == 6) || (Convert.ToInt32(obj["TemplateID"]) == 7) || (Convert.ToInt32(obj["TemplateID"]) == 9) || (Convert.ToInt32(obj["TemplateID"]) == 12))
                                            {
                                                var formattedvalue = GetCurrency(Convert.ToInt32(obj["ToValue"]));
                                                sb.Replace(match.Value, Convert.ToString(formattedvalue) + " " + proxy.MarcomManager.GlobalAdditionalSettings[1].CurrencyFormatvalue[0].ShortName);
                                            }
                                            else
                                            {
                                                sb.Replace(match.Value, Convert.ToString(obj["ToValue"]));
                                            }
                                            break;
                                        }
                                    case "@FundingRequestState@":
                                        {
                                            sb.Replace(match.Value, obj["ToValue"].ToString());
                                            break;
                                        }
                                    case "@ActorName@":
                                        {
                                            UserDao user1 = new UserDao();
                                            user1 = tx.PersistenceManager.CommonRepository.Get<UserDao>(obj["Actor"]);
                                            sb.Replace(match.Value, user1.FirstName + " " + user1.LastName);
                                            break;
                                        }
                                    case "@NewsValue@":
                                        {
                                            sb.Replace(match.Value, Convert.ToString(obj["ToValue"]));
                                            break;
                                        }
                                    case "@Path@":
                                        {

                                            if (Convert.ToInt32(obj["TemplateID"]) == 24 ||
                                                  Convert.ToInt32(obj["TemplateID"]) == 13 ||
                                                  Convert.ToInt32(obj["TemplateID"]) == 17 ||
                                                  Convert.ToInt32(obj["TemplateID"]) == 38 ||
                                                  Convert.ToInt32(obj["TemplateID"]) == 22 ||
                                                   Convert.ToInt32(obj["TemplateID"]) == 10 ||
                                                     Convert.ToInt32(obj["TemplateID"]) == 39 ||
                                                     Convert.ToInt32(obj["TemplateID"]) == 38 ||
                                                  Convert.ToInt32(obj["TemplateID"]) == 25 || Convert.ToInt32(obj["TemplateID"]) == 26 || Convert.ToInt32(obj["TemplateID"]) == 27 || Convert.ToInt32(obj["TemplateID"]) == 28 || Convert.ToInt32(obj["TemplateID"]) == 31 || Convert.ToInt32(obj["TemplateID"]) == 32 || Convert.ToInt32(obj["TemplateID"]) == 33 || Convert.ToInt32(obj["TemplateID"]) == 34 || Convert.ToInt32(obj["TemplateID"]) == 35 || Convert.ToInt32(obj["TemplateID"]) == 36 || Convert.ToInt32(obj["TemplateID"]) == 40 || Convert.ToInt32(obj["TemplateID"]) == 104)
                                            {
                                                if (asset.EntityID == Convert.ToInt32(obj["EntiyParentID"]))
                                                    sb.Replace(match.Value, "");
                                                else
                                                    sb.Replace(match.Value, "<a href='javascript:void(0);' data-id=\"feedpath\" data-parentid='" + obj["ParentID"] + "'data-entityid='" + obj["EntiyParentID"] + "' data-typeid='" + obj["Typeid"] + "' >" + Convert.ToString(obj["ParentName"]) + "</a>");
                                            }
                                            else
                                            {
                                                if (Convert.ToInt32(obj["TemplateID"]) == 14 || Convert.ToInt32(obj["TemplateID"]) == 15)
                                                {
                                                    if (asset.EntityID != Convert.ToInt32(obj["EntiyParentID"]))
                                                        sb.Replace(match.Value, "");
                                                    else
                                                        sb.Replace(match.Value, "<a href='javascript:void(0);' data-id=\"feedpath\" data-parentid='" + obj["ParentID"] + "'data-entityid='" + obj["EntityID"] + "' data-typeid='" + obj["Typeid"] + "' >" + entityname + "</a>");

                                                }
                                                else
                                                {
                                                    if (asset.EntityID == Convert.ToInt32(obj["EntityID"]) || ((asset.EntityID == Convert.ToInt32(obj["EntiyParentID"])) && (Convert.ToInt32(obj["TemplateID"]) == 1) || Convert.ToInt32(obj["TemplateID"]) == 18))
                                                        sb.Replace(match.Value, "");
                                                    else
                                                        sb.Replace(match.Value, "<a href='javascript:void(0);' data-id=\"feedpath\" data-parentid='" + obj["ParentID"] + "'data-entityid='" + obj["EntityID"] + "' data-typeid='" + obj["Typeid"] + "' >" + Convert.ToString(obj["ParentName"]) + "</a>");
                                                }
                                            }

                                            //string feedpath = IncludePathForFeed(proxy, tx, feedObj.FeedId);
                                            //sb.Replace(match.Value, feedpath);
                                            break;
                                        }
                                    case "@Users@":
                                        {
                                            sb.Replace(match.Value, user.FirstName + " " + user.LastName);
                                            break;
                                        }
                                    case "@role@":
                                        {
                                            var role = tx.PersistenceManager.CommonRepository.Get<RoleDao>(Convert.ToInt32(obj["ToValue"]));
                                            sb.Replace(match.Value, role.Caption);
                                            break;
                                        }
                                    case "@AttributeName@":
                                        {

                                            if ((Convert.ToInt32(obj["TemplateID"]) == 9) || (Convert.ToInt32(obj["TemplateID"]) == 12))
                                            {
                                                var formattedvalue = GetCurrency(Convert.ToInt32(obj["ToValue"]));
                                                sb.Replace(match.Value, Convert.ToString(formattedvalue) + " " + proxy.MarcomManager.GlobalAdditionalSettings[1].CurrencyFormatvalue[0].Symbol);
                                            }
                                            else if ((Convert.ToInt32(obj["TemplateID"]) == 7) || (Convert.ToInt32(obj["TemplateID"]) == 6) || (Convert.ToInt32(obj["TemplateID"]) == 21))
                                            {
                                                sb.Replace(match.Value, "<a href='javascript:void(0);' data-id=\"costcenterlink\" data-costcentreid='" + obj["AssocitedEntityID"] + "'>" + Convert.ToString(obj["AttributeName"]) + "</a>");
                                            }
                                            else
                                            {
                                                sb.Replace(match.Value, Convert.ToString(obj["AttributeName"]));
                                            }

                                            break;
                                        }

                                    case "@Fromvalue@":
                                        {
                                            sb.Replace(match.Value, Convert.ToString(obj["FromValue"]));
                                            break;
                                        }
                                    case "@filename@":
                                        {
                                            //if (Convert.ToInt32(obj["TemplateID"]) == 29 || Convert.ToInt32(obj["TemplateID"]) == 30)
                                            //{
                                            //    sb.Replace(match.Value, Convert.ToString(obj["ToValue"]));
                                            //}
                                            //else
                                            //{
                                            var filedetails = (from tt in tx.PersistenceManager.CommonRepository.Query<FileDao>() where tt.Id == Convert.ToInt32(obj["ToValue"]) select tt).FirstOrDefault();
                                            if (filedetails == null)
                                                sb.Replace(match.Value, Convert.ToString(obj["AttributeName"]));
                                            else
                                                sb.Replace(match.Value, "<a target=\"_blank\" href=\"download.aspx?FileID=" + filedetails.Fileguid + "&amp;FileFriendlyName=" + Convert.ToString(filedetails.Name) + "&amp;Ext=" + filedetails.Extension + "\">" + Convert.ToString(filedetails.Name) + "</a>");
                                            //}
                                            break;
                                        }
                                    case "@linkname@":
                                        {
                                            var linkdetails = (from tt in tx.PersistenceManager.CommonRepository.Query<LinksDao>() where tt.ID == Convert.ToInt32(obj["ToValue"]) select tt).FirstOrDefault();
                                            if (linkdetails == null)
                                                sb.Replace(match.Value, Convert.ToString(obj["AttributeName"]));
                                            else
                                                sb.Replace(match.Value, "<a href=\"javascript:void(0);\" data-name=\"" + linkdetails.URL + "\"  data-id=\"" + linkdetails.ID + "\" data-command=\"openlink\">" + Convert.ToString(linkdetails.Name) + "</a>");
                                            break;
                                        }
                                    case "@TaskStatus@":
                                        {
                                            sb.Replace(match.Value, Convert.ToString(obj["ToValue"]));
                                            break;
                                        }
                                    case "@checklistname@":
                                        {
                                            sb.Replace(match.Value, Convert.ToString(obj["AttributeName"]));
                                            break;
                                        }
                                    case "@taskTypeName@":
                                        {
                                            var taskdetails = (from tt in totalFeedEntities where tt.Id == Convert.ToInt32(obj["EntityID"]) select tt).FirstOrDefault();
                                            var tasktypename = (from type in totalFeedEntityTypes where type.Id == taskdetails.Typeid select type).FirstOrDefault();
                                            sb.Replace(match.Value, tasktypename.Caption);
                                            break;
                                        }
                                    case "@taskName@":
                                        {

                                            sb.Replace(match.Value, entityname);
                                            // sb.Replace(match.Value, "<a href='javascript:void(0);'  data-id=\"taskpopupopen\" data-taskid='" + Convert.ToInt32(obj["EntityID"]) + "'  data-entityid='" + Convert.ToInt32(obj["EntiyParentID"]) + "' data-typeid='" + Convert.ToInt32(obj["Typeid"]) + "' >" + entityname + "</a>");
                                            //if (Convert.ToInt32(entityIdForReference) == Convert.ToInt32(obj["EntityID"]))
                                            //    sb.Replace(match.Value, "");
                                            //else
                                            //{
                                            //    sb.Replace(match.Value, "<a href='javascript:void(0);'  data-id=\"taskpopupopen\" data-taskid='" + Convert.ToInt32(obj["EntityID"]) + "'  data-entityid='" + Convert.ToInt32(obj["EntiyParentID"]) + "' data-typeid='" + Convert.ToInt32(obj["Typeid"]) + "' >" + entityname + "</a>");
                                            //}
                                            break;
                                        }
                                    //case "@taskTypeName@":
                                    //    {
                                    //        var taskdetails = (from tt in tx.PersistenceManager.PlanningRepository.Query<EntityDao>() where tt.Id == Convert.ToInt32(obj["EntityID"]) select tt).FirstOrDefault();
                                    //        var tasktypename = (from type in tx.PersistenceManager.PlanningRepository.Query<EntityTypeDao>() where type.Id == taskdetails.Typeid select type).FirstOrDefault();
                                    //        sb.Replace(match.Value, tasktypename.Caption);
                                    //        // sb.Replace(match.Value, Convert.ToString(obj["ToValue"]));
                                    //        break;
                                    //    }
                                    //case "@taskName@":
                                    //    {
                                    //        sb.Replace(match.Value, "<a href='javascript:void(0);' data-id=\"feedpath\" data-parentid='" + obj["ParentID"] + "'data-entityid='" + obj["EntityID"] + "' data-typeid='" + obj["Typeid"] + "' >" + entityname + "</a>");
                                    //        //sb.Replace(match.Value, Convert.ToString(obj["ToValue"]));
                                    //        break;
                                    //    }
                                    case "@checkliststatus@":
                                        {
                                            sb.Replace(match.Value, Convert.ToString(obj["ToValue"]));
                                            break;
                                        }
                                    case "@TasksMembers@":
                                        {
                                            string commaseperatedusers = "";

                                            foreach (var c in obj["ToValue"].ToString().Split(','))
                                            {
                                                if (c != "")
                                                {
                                                    var user1 = tx.PersistenceManager.AccessRepository.Get<UserDao>(Convert.ToInt32(c));
                                                    if (user1.Id == Convert.ToInt32(obj["Actor"]))
                                                    {
                                                        commaseperatedusers = commaseperatedusers + "," + "Oneself";
                                                    }
                                                    else
                                                    {
                                                        commaseperatedusers = commaseperatedusers + ',' + user1.FirstName + ' ' + user1.LastName;

                                                    }
                                                    commaseperatedusers = commaseperatedusers.TrimStart().TrimStart(',');
                                                }
                                            }
                                            sb.Replace(match.Value, commaseperatedusers);
                                            break;
                                        }

                                    case "@CostCenterName@":
                                        {
                                            sb.Replace(match.Value, "<a href='javascript:void(0);' data-id=\"costcenterlink\" data-costcentreid='" + obj["AssocitedEntityID"] + "'>" + obj["AttributeName"] + "</a>");

                                            //sb.Replace(match.Value, Convert.ToString(obj["AttributeName"]));
                                            break;
                                        }
                                    case "@AssetName@":
                                        {
                                            int assetid = 0;
                                            if (Convert.ToInt32(obj["TemplateID"]) == 109 || Convert.ToInt32(obj["TemplateID"]) == 110)

                                                assetid = Convert.ToInt32(obj["AssetId"]);
                                            else
                                                assetid = Convert.ToInt32(obj["AssocitedEntityID"]);
                                            var Assetdetails = (from tt in tx.PersistenceManager.CommonRepository.Query<AssetsDao>() where tt.ID == assetid select tt).FirstOrDefault();

                                            if (Assetdetails == null)
                                                sb.Replace(match.Value, Convert.ToString(obj["AttributeName"]));
                                            else
                                                sb.Replace(match.Value, Assetdetails.Name);

                                            //    sb.Replace(match.Value, "<a href='javascript:void(0);' data-id=\"Assetpath\"data-entityid='" + Assetdetails.EntityID + "'data-Assetid='" + Assetdetails.ID + "' data-typeid='" + obj["Typeid"] + "' >" + Assetdetails.Name + "</a>");
                                            //sb.Replace(match.Value, "this");
                                            break;
                                        }
                                    case "@VersionNo@":
                                        {



                                            if (Convert.ToString(obj["Version"]) != null)
                                                sb.Replace(match.Value, Convert.ToString(obj["Version"]));
                                            else
                                                sb.Replace(match.Value, "0");


                                            break;
                                        }
                                    case "@AssetFileName@":
                                        {
                                            if (obj["ToValue"] != null)
                                            {
                                                var Filedetails = (from tt in tx.PersistenceManager.CommonRepository.Query<DAMFileDao>() where tt.ID == Convert.ToInt32(obj["ToValue"]) select tt).FirstOrDefault();
                                                //var Assetdetails = (from tt in tx.PersistenceManager.CommonRepository.Query<AssetsDao>() where tt.ID == Convert.ToInt32(obj["AssocitedEntityID"]) select tt).FirstOrDefault();
                                                if (Filedetails == null)
                                                    sb.Replace(match.Value, Convert.ToString(obj["AttributeName"]));
                                                else
                                                    sb.Replace(match.Value, Filedetails.Name);
                                            }
                                            else { sb.Replace(match.Value, "-"); }
                                            //sb.Replace(match.Value, "<a href='javascript:void(0);' data-id=\"Assetpath\"data-entityid='" + Assetdetails.EntityID + "'data-Assetid='" + Assetdetails.ID + "' data-typeid='" + obj["Typeid"] + "' >" + Assetdetails.Name + "</a>");
                                            break;
                                        }

                                }
                            }
                            catch (Exception ex)
                            { }
                        }

                        feedObj.FeedText = Convert.ToString(sb);
                        while (feedObj.FeedText.EndsWith("in"))
                            feedObj.FeedText = feedObj.FeedText.Substring(0, feedObj.FeedText.Length - 2).Trim();
                        IList<IFeedComment> listOfFeedComment = new List<IFeedComment>();
                        foreach (var objcomment in GetFeedcomments)
                        {
                            FeedComment feedCommentObj = new FeedComment();
                            if (Convert.ToInt32(objcomment["FeedID"]) == Convert.ToInt32(obj["ID"]))
                            {
                                feedCommentObj.Id = Convert.ToInt32(objcomment["ID"]);
                                feedCommentObj.Feedid = Convert.ToInt32(objcomment["FeedID"]);
                                feedCommentObj.Comment = Convert.ToString(objcomment["Comment"]);
                                feedCommentObj.UserName = Convert.ToString(objcomment["UserFirstName"] + " " + objcomment["UserLastName"]);
                                feedCommentObj.Usermail = Convert.ToString(objcomment["UserEmail"]);
                                feedCommentObj.Actor = Convert.ToInt32(objcomment["Actor"]);

                                TimeSpan difference1 = (proxy.MarcomManager.UserManager.AssetFeedInitialRequestedTime - DateTime.Parse(objcomment["CommentedOn"].ToString()));
                                if (difference1.Days > 0)
                                    if (difference1.Days > 1)
                                        feedCommentObj.CommentedOn = Convert.ToString(DateTime.Parse(objcomment["CommentedOn"].ToString()) + offSet);
                                    else
                                        feedCommentObj.CommentedOn = "Yesterday at " + (DateTime.Parse(objcomment["CommentedOn"].ToString()) + offSet).ToShortTimeString();
                                else if (difference1.Hours > 0)
                                    if (difference1.Days < 2)
                                        feedCommentObj.CommentedOn = "about an hour ago";
                                    else
                                        feedCommentObj.CommentedOn = difference1.Hours + " hours ago";
                                else if (difference1.Minutes > 0)
                                    feedCommentObj.CommentedOn = difference1.Minutes + " minutes ago";
                                else
                                    feedCommentObj.CommentedOn = "Few seconds ago";


                                listOfFeedComment.Add(feedCommentObj);

                            }
                            feedObj.FeedComment = listOfFeedComment;
                        }

                        // feedObj.FeedComment = listOfFeedComment;
                        listfeedselection.Add(feedObj);
                    }
                    if (isForRealTimeUpdate && childEntiyResult.Count > 0)
                        proxy.MarcomManager.UserManager.AssetFeedRecentlyUpdatedTime = DateTimeOffset.UtcNow;
                    return listfeedselection;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                proxy.MarcomManager.UserManager.AssetFeedLock = false;
            }
        }

        /// <summary>
        /// Getting Feeds By EntityID  and funding request
        /// </summary>
        /// <param name="proxy"></param>
        /// <param name="entityId"></param>
        /// <param name="pageNo">If it is real time update, page number is not applicable (-1) </param>
        /// <param name="isForRealTimeUpdate"></param>
        /// <returns></returns>
        public IList<IFeedSelection> GettingFeedsByEntityIDandFundingrequest(CommonManagerProxy proxy, int entityId, int pageNo, bool isForRealTimeUpdate)
        {
            try
            {
                if (proxy.MarcomManager.UserManager.TaskFeedLock)
                    return new List<IFeedSelection>();

                proxy.MarcomManager.UserManager.TaskFeedLock = true;
                if (pageNo == 0)
                {
                    proxy.MarcomManager.UserManager.TaskFeedInitialRequestedTime = DateTimeOffset.UtcNow;
                    proxy.MarcomManager.UserManager.TaskFeedRecentlyUpdatedTime = DateTimeOffset.UtcNow;
                }
                if (pageNo > 0)
                {
                    pageNo = pageNo * 10;
                }
                IList<IFeedSelection> listfeedselection = new List<IFeedSelection>();
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    var feedSelectQuery = new StringBuilder();
                    if (entityId != 0)
                        feedSelectQuery.Append("select cmf.ID,cmf.Actor,cmf.UserID, cmf.TemplateID,cmf.HappenedOn,cmf.AssocitedEntityID,cmf.CommentedUpdatedOn,cmf.EntityID,cmf.TypeName,cmf.TypeName," +
                                             "cmf.AttributeName,cmf.FromValue,cmf.ToValue,pme.Name as 'EntityName',pme.UniqueKey as 'EntiyUniquekey',pme.ParentID 'EntiyParentID', parentEnt.Name as 'ParentName'," +
                                             "umuse.FirstName as 'UserFirstName',umuse.LastName 'UserLastName',umuse.Email as 'UserEmail',umuse.Image as 'UserImage'," +
                                             "umuse.TimeZone as 'UserTimeZone',cmt.Template as 'FeedTemplate',tt.AssetId  AS  'AssetId',cmf.Version AS 'Version' from CM_Feed cmf inner join PM_Entity pme on cmf.EntityID = pme.ID inner join PM_Entity parentEnt on pme.ParentID = parentEnt.ID  inner join UM_User umuse on" +
                                             " umuse.ID = cmf.Actor inner join CM_Feed_Template cmt on cmt.ID = cmf.TemplateID LEFT OUTER JOIN TM_EntityTask tt ON  tt.ID = cmf.EntityID  where (pme.ID= " + entityId + " or pme.UniqueKey  like " +
                                             "(SELECT pe.UniqueKey + '.%' FROM   PM_Entity pe WHERE  pe.ID =" + entityId + ")) and cmf.HappenedOn ");
                    else
                        feedSelectQuery.Append("select cmf.ID,cmf.Actor,cmf.UserID,cmf.TemplateID,cmf.HappenedOn,cmf.CommentedUpdatedOn,cmf.EntityID,cmf.TypeName,cmf.TypeName," +
                                                 "cmf.AttributeName,cmf.FromValue,cmf.ToValue,pme.Name as 'EntityName',pme.UniqueKey as 'EntiyUniquekey',pme.ParentID 'EntiyParentID'," +
                                                 "umuse.FirstName as 'UserFirstName',umuse.LastName 'UserLastName',umuse.Email as 'UserEmail',umuse.Image as 'UserImage'," +
                                                 "umuse.TimeZone as 'UserTimeZone',cmt.Template as 'FeedTemplate' ,tt.AssetId  AS  'AssetId',cmf.Version from CM_Feed cmf inner join PM_Entity pme on cmf.EntityID = pme.ID inner join UM_User umuse on" +
                                           " umuse.ID = cmf.Actor inner join CM_Feed_Template cmt on cmt.ID = cmf.TemplateID LEFT OUTER JOIN TM_EntityTask tt ON  tt.ID = cmf.EntityID  where (cmf.Actor =" + proxy.MarcomManager.User.Id + " ) and cmf.HappenedOn ");
                    if (isForRealTimeUpdate)
                        feedSelectQuery.Append(">= '" + (proxy.MarcomManager.UserManager.TaskFeedRecentlyUpdatedTime).ToString("yyyy-MM-dd HH:mm:ss.FFFFFFF") + "'");
                    else
                        feedSelectQuery.Append("<= '" + (proxy.MarcomManager.UserManager.TaskFeedInitialRequestedTime).ToString("yyyy-MM-dd HH:mm:ss.FFFFFFF") + "'" + " ORDER BY cmf.HappenedOn desc OFFSET " + pageNo + " ROWS FETCH NEXT 20 ROWS ONLY");

                    var childEntiyResult = ((tx.PersistenceManager.CommonRepository.ExecuteQuery(feedSelectQuery.ToString())).Cast<Hashtable>().ToList());
                    TimeSpan offSet = TimeSpan.Parse(proxy.MarcomManager.User.TimeZone.TrimStart('+'));

                    //  ----------- Getting the comments for the list of feeds  ------------------------------


                    ArrayList arrFeedId = new ArrayList();

                    foreach (var obj in childEntiyResult)
                    {
                        arrFeedId.Add(Convert.ToInt32(obj["ID"]));
                    }

                    string arrFeedIdRes = "";
                    if (arrFeedId.Count > 0)
                    {
                        arrFeedIdRes = string.Join(",", arrFeedId.ToArray());
                    }
                    feedSelectQuery.Clear();


                    if (arrFeedIdRes == "")
                    {
                        feedSelectQuery.Append("select cmfcom.ID,cmfcom.FeedID,cmfcom.Comment,cmfcom.CommentedOn,cmfcom.Actor," +
                         "umuse.FirstName as 'UserFirstName',umuse.LastName as 'UserLastName',umuse.Email as 'UserEmail',umuse.Image as 'UserImage'," +
                         "umuse.TimeZone as 'UserTimeZone' from  CM_Feed_Comment cmfcom inner join UM_User umuse on" +
                         " umuse.ID = cmfcom.Actor where cmfcom.FeedID IN  " +
                         "('') order by cmfcom.CommentedOn asc");
                    }
                    else
                    {
                        feedSelectQuery.Append("select cmfcom.ID,cmfcom.FeedID,cmfcom.Comment,cmfcom.CommentedOn,cmfcom.Actor," +
                         "umuse.FirstName as 'UserFirstName',umuse.LastName as 'UserLastName',umuse.Email as 'UserEmail',umuse.Image as 'UserImage'," +
                         "umuse.TimeZone as 'UserTimeZone' from  CM_Feed_Comment cmfcom inner join UM_User umuse on" +
                         " umuse.ID = cmfcom.Actor where cmfcom.FeedID IN  " +
                         "(" + arrFeedIdRes + ") order by cmfcom.CommentedOn asc");
                    }

                    var GetFeedcomments = ((tx.PersistenceManager.CommonRepository.ExecuteQuery(feedSelectQuery.ToString())).Cast<Hashtable>().ToList());

                    //---------------------------------------------------------------
                    string dateformate = proxy.MarcomManager.GlobalAdditionalSettings[0].SettingValue.ToString().Replace('m', 'M');
                    dateformate += " hh:mm:ss tt";

                    foreach (var obj in childEntiyResult)
                    {
                        FeedSelection feedObj = new FeedSelection();
                        feedObj.FeedId = Convert.ToInt32(obj["ID"]);
                        feedObj.UserName = Convert.ToString(obj["UserFirstName"] + " " + obj["UserLastName"]);
                        feedObj.UserEmail = Convert.ToString(obj["UserEmail"]);
                        feedObj.UserImage = Convert.ToString(obj["UserImage"]);
                        feedObj.Actor = Convert.ToInt32(obj["Actor"]);

                        var typename = Convert.ToString(obj["TypeName"]);
                        var entityname = Convert.ToString(obj["EntityName"]);

                        UserDao user = new UserDao();
                        user = tx.PersistenceManager.CommonRepository.Get<UserDao>(obj["UserID"]);

                        TimeSpan difference = (proxy.MarcomManager.UserManager.TaskFeedInitialRequestedTime - DateTime.Parse(obj["HappenedOn"].ToString()));
                        if (difference.Days > 0)
                            if (difference.Days > 1)
                                feedObj.FeedHappendTime = (DateTime.Parse(obj["HappenedOn"].ToString()) + offSet).ToString(dateformate);
                            else
                                feedObj.FeedHappendTime = "Yesterday at " + (DateTime.Parse(obj["HappenedOn"].ToString()) + offSet).ToShortTimeString();
                        else if (difference.Hours > 0)
                            if (difference.Hours < 2)
                                feedObj.FeedHappendTime = "about an hour ago";
                            else
                                feedObj.FeedHappendTime = difference.Hours + " hours ago";
                        else if (difference.Minutes > 0)
                            feedObj.FeedHappendTime = difference.Minutes + " minutes ago";
                        else
                            feedObj.FeedHappendTime = "Few seconds ago";

                        string template = Convert.ToString(obj["FeedTemplate"]);


                        if (template.Trim().EndsWith("in @Path@"))
                        {
                            template = template.Replace("in @Path@", " ");
                        }
                        if (template.Trim().EndsWith("in @EntityNamefortask@"))
                        {
                            template = template.Replace("in @EntityNamefortask@", " ");
                        }
                        if (template.Trim().EndsWith("in @taskTypeName@ @EntityNamefortask@"))
                        {
                            template = template.Replace("in @taskTypeName@ @EntityNamefortask@", " ");
                        }
                        if (template.Trim().EndsWith("in @EntityTypeName@ @EntityName@"))
                        {
                            template = template.Replace("in @EntityTypeName@ @EntityName@", " ");
                        }
                        if (template.Trim().EndsWith("in @taskTypeName@ @taskName@"))
                        {
                            template = template.Replace("in @taskTypeName@ @taskName@", " ");
                        }
                        if (template.Trim().Contains("reinitiated @taskTypeName@ @taskName@ in @EntityTypeName@ @EntityName@"))
                        {
                            template = template.Replace("@taskTypeName@ @taskName@ in @EntityTypeName@ @EntityName@", " ");
                        }
                        if (template.Trim().Contains("of @taskTypeName@ @taskName@"))
                        {
                            template = template.Replace("of @taskTypeName@ @taskName@", " ");
                        }
                        if (Convert.ToInt32(obj["TemplateID"]) == 38)
                        {
                            template = template.Replace("@taskName@", " ");
                        }

                        if (template.Trim().EndsWith("from @Path@"))
                        {
                            template = template.Replace("from @Path@", " ");
                        }
                        //foreach(Match  mm in Regex.Matches(template,@"(?<=\@#)(.*?)(?=\@#)"))
                        //{
                        //    template = template.Replace(mm.ToString(),"").Replace("@#@#@","").ToString();
                        //}
                        StringBuilder sb = new StringBuilder(template);
                        foreach (Match match in Regex.Matches(template, @"@(.+?)@"))
                        {
                            try
                            {
                                switch (match.Value.Trim())
                                {

                                    case "@EntityTypeName@":
                                        {
                                            //  sb.Replace(match.Value, typename);
                                            //if (Convert.ToInt32(obj["TemplateID"]) == 22)
                                            //{
                                            //    sb.Replace(match.Value, obj["AttributeName"].ToString());
                                            //}
                                            //if (Convert.ToInt32(obj["TemplateID"]) == 24 || Convert.ToInt32(obj["TemplateID"]) == 25 || Convert.ToInt32(obj["TemplateID"]) == 26 || Convert.ToInt32(obj["TemplateID"]) == 27 || Convert.ToInt32(obj["TemplateID"]) == 28 || Convert.ToInt32(obj["TemplateID"]) == 31 || Convert.ToInt32(obj["TemplateID"]) == 32 || Convert.ToInt32(obj["TemplateID"]) == 33 || Convert.ToInt32(obj["TemplateID"]) == 34 || Convert.ToInt32(obj["TemplateID"]) == 36)
                                            //{
                                            //    var entitytaskdetails = (from tt in tx.PersistenceManager.PlanningRepository.Query<EntityDao>() where tt.Id == Convert.ToInt32(obj["AssocitedEntityID"]) select tt).FirstOrDefault();
                                            //    var entitytasktypename = (from type in tx.PersistenceManager.PlanningRepository.Query<EntityTypeDao>() where type.Id == entitytaskdetails.Typeid select type).FirstOrDefault();
                                            //    sb.Replace(match.Value, entitytasktypename.Caption);
                                            //}

                                            if (Convert.ToInt32(obj["TemplateID"]) == 22)
                                            {
                                                sb.Replace(match.Value, obj["AttributeName"].ToString());
                                            }
                                            else if (Convert.ToInt32(obj["TemplateID"]) == 24 || Convert.ToInt32(obj["TemplateID"]) == 25 || Convert.ToInt32(obj["TemplateID"]) == 26 || Convert.ToInt32(obj["TemplateID"]) == 27 || Convert.ToInt32(obj["TemplateID"]) == 28 || Convert.ToInt32(obj["TemplateID"]) == 31 || Convert.ToInt32(obj["TemplateID"]) == 32 || Convert.ToInt32(obj["TemplateID"]) == 33 || Convert.ToInt32(obj["TemplateID"]) == 34 || Convert.ToInt32(obj["TemplateID"]) == 35 || Convert.ToInt32(obj["TemplateID"]) == 36 || Convert.ToInt32(obj["TemplateID"]) == 13 || Convert.ToInt32(obj["TemplateID"]) == 17 || Convert.ToInt32(obj["TemplateID"]) == 22 || Convert.ToInt32(obj["TemplateID"]) == 40)
                                            {
                                                if (entityId == Convert.ToInt32(obj["EntiyParentID"]))
                                                    sb.Replace(match.Value, "");
                                                else
                                                {
                                                    if (entityId == Convert.ToInt32(obj["EntiyParentID"]))
                                                        sb.Replace(match.Value, "");
                                                    else
                                                    {
                                                        var entitytaskdetails = (from tt in tx.PersistenceManager.PlanningRepository.Query<EntityDao>() where tt.Id == Convert.ToInt32(obj["AssocitedEntityID"]) select tt).FirstOrDefault();
                                                        var entitytasktypename = (from type in tx.PersistenceManager.PlanningRepository.Query<EntityTypeDao>() where type.Id == entitytaskdetails.Typeid select type).FirstOrDefault();
                                                        sb.Replace(match.Value, entitytasktypename.Caption);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (entityId == Convert.ToInt32(obj["EntityID"]) && obj["TemplateID"].ToString() == "1")
                                                    sb.Replace("new", "this");
                                                sb.Replace(match.Value, typename);
                                            }

                                            break;
                                        }
                                    case "@EntityName@":
                                        {
                                            //if (Convert.ToInt32(obj["TemplateID"]) == 24 || Convert.ToInt32(obj["TemplateID"]) == 25 || Convert.ToInt32(obj["TemplateID"]) == 26 || Convert.ToInt32(obj["TemplateID"]) == 27 || Convert.ToInt32(obj["TemplateID"]) == 28 || Convert.ToInt32(obj["TemplateID"]) == 31 || Convert.ToInt32(obj["TemplateID"]) == 32 || Convert.ToInt32(obj["TemplateID"]) == 33 || Convert.ToInt32(obj["TemplateID"]) == 34 || Convert.ToInt32(obj["TemplateID"]) == 36)
                                            //{
                                            //    var entitytaskdetails = (from tt in tx.PersistenceManager.PlanningRepository.Query<EntityDao>() where tt.Id == Convert.ToInt32(obj["AssocitedEntityID"]) select tt).FirstOrDefault();
                                            //    sb.Replace(match.Value, entitytaskdetails.Name);
                                            //}
                                            //else
                                            //{
                                            //    sb.Replace(match.Value, entityname);
                                            //}


                                            if (Convert.ToInt32(obj["TemplateID"]) == 24 || Convert.ToInt32(obj["TemplateID"]) == 25 || Convert.ToInt32(obj["TemplateID"]) == 26 || Convert.ToInt32(obj["TemplateID"]) == 27 || Convert.ToInt32(obj["TemplateID"]) == 28 || Convert.ToInt32(obj["TemplateID"]) == 31 || Convert.ToInt32(obj["TemplateID"]) == 32 || Convert.ToInt32(obj["TemplateID"]) == 33 || Convert.ToInt32(obj["TemplateID"]) == 34 || Convert.ToInt32(obj["TemplateID"]) == 35 || Convert.ToInt32(obj["TemplateID"]) == 36 || Convert.ToInt32(obj["TemplateID"]) == 13 || Convert.ToInt32(obj["TemplateID"]) == 17 || Convert.ToInt32(obj["TemplateID"]) == 22 || Convert.ToInt32(obj["TemplateID"]) == 40)
                                            {
                                                if (entityId == Convert.ToInt32(obj["ParentID"]))
                                                    sb.Replace(match.Value, "").Replace("in", "");
                                                else
                                                {
                                                    var entitytaskdetails = (from tt in tx.PersistenceManager.PlanningRepository.Query<EntityDao>() where tt.Id == Convert.ToInt32(obj["AssocitedEntityID"]) select tt).FirstOrDefault();
                                                    // sb.Replace(match.Value, entitytaskdetails.Name);
                                                    sb.Replace(match.Value, "<a href='javascript:void(0);' data-id=\"feedpath\" data-parentid='" + entitytaskdetails.Parentid + "'data-entityid='" + entitytaskdetails.Id + "' data-typeid='" + entitytaskdetails.Typeid + "' >" + entitytaskdetails.Name + "</a>");
                                                }
                                            }
                                            else
                                            {
                                                if (entityId == Convert.ToInt32(obj["EntityID"]))
                                                    sb.Replace(match.Value, "").Replace("in", "");
                                                else
                                                {

                                                    if (Convert.ToInt32(obj["TemplateID"]) == 6)
                                                    {
                                                        sb.Replace(match.Value, "<a href='javascript:void(0);'  data-parentid='" + obj["EntiyParentID"] + "'data-entityid='" + obj["EntityID"] + "' data-typeid='" + obj["Typeid"] + "' >" + entityname + "</a>");
                                                    }
                                                    sb.Replace(match.Value, "<a href='javascript:void(0);' data-id=\"feedpath\" data-parentid='" + obj["ParentID"] + "'data-entityid='" + obj["EntityID"] + "' data-typeid='" + obj["Typeid"] + "' >" + entityname + "</a>");
                                                }

                                            }
                                            break;
                                        }
                                    case "@AttributeValue@":
                                        {
                                            if (Convert.ToInt32(obj["TemplateID"]) == 39)
                                            {
                                                sb.Replace(match.Value, Convert.ToString(obj["ToValue"]));
                                            }
                                            if ((Convert.ToInt32(obj["TemplateID"]) == 20) || (Convert.ToInt32(obj["TemplateID"]) == 21) || (Convert.ToInt32(obj["TemplateID"]) == 6) || (Convert.ToInt32(obj["TemplateID"]) == 7) || (Convert.ToInt32(obj["TemplateID"]) == 9) || (Convert.ToInt32(obj["TemplateID"]) == 12))
                                            {
                                                var formattedvalue = GetCurrency(Convert.ToInt32(obj["ToValue"]));
                                                sb.Replace(match.Value, Convert.ToString(formattedvalue) + " " + proxy.MarcomManager.GlobalAdditionalSettings[1].CurrencyFormatvalue[0].ShortName);
                                            }
                                            else
                                            {
                                                sb.Replace(match.Value, Convert.ToString(obj["ToValue"]));
                                            }
                                            break;
                                        }
                                    case "@FundingRequestState@":
                                        {
                                            sb.Replace(match.Value, obj["ToValue"].ToString());
                                            break;
                                        }
                                    case "@ActorName@":
                                        {
                                            UserDao user1 = new UserDao();
                                            user1 = tx.PersistenceManager.CommonRepository.Get<UserDao>(obj["Actor"]);
                                            sb.Replace(match.Value, user1.FirstName + " " + user1.LastName);
                                            break;
                                        }
                                    case "@NewsValue@":
                                        {
                                            sb.Replace(match.Value, Convert.ToString(obj["ToValue"]));
                                            break;
                                        }
                                    case "@Path@":
                                        {
                                            //if (Convert.ToInt32(obj["TemplateID"]) == 24 || Convert.ToInt32(obj["TemplateID"]) == 25 || Convert.ToInt32(obj["TemplateID"]) == 26 || Convert.ToInt32(obj["TemplateID"]) == 27 || Convert.ToInt32(obj["TemplateID"]) == 28 || Convert.ToInt32(obj["TemplateID"]) == 31 || Convert.ToInt32(obj["TemplateID"]) == 32 || Convert.ToInt32(obj["TemplateID"]) == 33 || Convert.ToInt32(obj["TemplateID"]) == 34 || Convert.ToInt32(obj["TemplateID"]) == 36 || Convert.ToInt32(obj["TemplateID"]) == 40)
                                            //{
                                            //    if (entityId == Convert.ToInt32(obj["EntiyParentID"]))
                                            //        sb.Replace(match.Value, "").Replace("in", "");
                                            //    else
                                            //        sb.Replace(match.Value, "<a href='javascript:void(0);' data-id=\"feedpath\" data-parentid='" + obj["ParentID"] + "'data-entityid='" + obj["EntiyParentID"] + "' data-typeid='" + obj["Typeid"] + "' >" + Convert.ToString(obj["ParentName"]) + "</a>");
                                            //}
                                            //else
                                            //{
                                            //    if (entityId == Convert.ToInt32(obj["EntityID"]) || ((entityId == Convert.ToInt32(obj["EntiyParentID"])) && Convert.ToInt32(obj["TemplateID"]) == 1))
                                            //        sb.Replace(match.Value, "").Replace("in", "");
                                            //    else
                                            //        sb.Replace(match.Value, "<a href='javascript:void(0);' data-id=\"feedpath\" data-parentid='" + obj["ParentID"] + "'data-entityid='" + obj["EntityID"] + "' data-typeid='" + obj["Typeid"] + "' >" + Convert.ToString(obj["ParentName"]) + "</a>");
                                            //}
                                            if (Convert.ToInt32(obj["TemplateID"]) == 24 ||
                                                  Convert.ToInt32(obj["TemplateID"]) == 13 ||
                                                  Convert.ToInt32(obj["TemplateID"]) == 17 ||
                                                  Convert.ToInt32(obj["TemplateID"]) == 38 ||
                                                  Convert.ToInt32(obj["TemplateID"]) == 22 ||
                                                   Convert.ToInt32(obj["TemplateID"]) == 10 ||
                                                     Convert.ToInt32(obj["TemplateID"]) == 39 ||
                                                     Convert.ToInt32(obj["TemplateID"]) == 38 ||
                                                  Convert.ToInt32(obj["TemplateID"]) == 25 || Convert.ToInt32(obj["TemplateID"]) == 26 || Convert.ToInt32(obj["TemplateID"]) == 27 || Convert.ToInt32(obj["TemplateID"]) == 28 || Convert.ToInt32(obj["TemplateID"]) == 31 || Convert.ToInt32(obj["TemplateID"]) == 32 || Convert.ToInt32(obj["TemplateID"]) == 33 || Convert.ToInt32(obj["TemplateID"]) == 34 || Convert.ToInt32(obj["TemplateID"]) == 35 || Convert.ToInt32(obj["TemplateID"]) == 36 || Convert.ToInt32(obj["TemplateID"]) == 40)
                                            {
                                                if (entityId == Convert.ToInt32(obj["EntiyParentID"]))
                                                    sb.Replace(match.Value, "");
                                                else
                                                    sb.Replace(match.Value, "<a href='javascript:void(0);' data-id=\"feedpath\" data-parentid='" + obj["ParentID"] + "'data-entityid='" + obj["EntiyParentID"] + "' data-typeid='" + obj["Typeid"] + "' >" + Convert.ToString(obj["ParentName"]) + "</a>");
                                            }
                                            else
                                            {
                                                if (Convert.ToInt32(obj["TemplateID"]) == 14 || Convert.ToInt32(obj["TemplateID"]) == 15)
                                                {
                                                    if (entityId != Convert.ToInt32(obj["EntiyParentID"]))
                                                        sb.Replace(match.Value, "");
                                                    else
                                                        sb.Replace(match.Value, "<a href='javascript:void(0);' data-id=\"feedpath\" data-parentid='" + obj["ParentID"] + "'data-entityid='" + obj["EntityID"] + "' data-typeid='" + obj["Typeid"] + "' >" + entityname + "</a>");

                                                }
                                                else
                                                {
                                                    if (entityId == Convert.ToInt32(obj["EntityID"]) || ((entityId == Convert.ToInt32(obj["EntiyParentID"])) && (Convert.ToInt32(obj["TemplateID"]) == 1) || Convert.ToInt32(obj["TemplateID"]) == 18))
                                                        sb.Replace(match.Value, "");
                                                    else
                                                        sb.Replace(match.Value, "<a href='javascript:void(0);' data-id=\"feedpath\" data-parentid='" + obj["ParentID"] + "'data-entityid='" + obj["EntityID"] + "' data-typeid='" + obj["Typeid"] + "' >" + Convert.ToString(obj["ParentName"]) + "</a>");
                                                }
                                            }

                                            //string feedpath = IncludePathForFeed(proxy, tx, feedObj.FeedId);
                                            //sb.Replace(match.Value, feedpath);
                                            break;
                                        }
                                    case "@Users@":
                                        {
                                            sb.Replace(match.Value, user.FirstName + " " + user.LastName);
                                            break;
                                        }
                                    case "@role@":
                                        {
                                            var currententityroleobj = tx.PersistenceManager.CommonRepository.Query<BaseEntityDao>().Where(a => a.Id == Convert.ToInt32(obj["EntityID"])).SingleOrDefault();
                                            var entitypeobj = tx.PersistenceManager.CommonRepository.Query<EntityTypeDao>().Where(a => a.Id == currententityroleobj.Typeid).SingleOrDefault();
                                            if (entitypeobj.IsAssociate)
                                            {
                                                var role = tx.PersistenceManager.CommonRepository.Get<RoleDao>(Convert.ToInt32(obj["ToValue"]));
                                                sb.Replace(match.Value, role.Caption);
                                            }
                                            else
                                            {
                                                var role = tx.PersistenceManager.CommonRepository.Get<EntityTypeRoleAclDao>(Convert.ToInt32(Convert.ToInt32(obj["ToValue"])));
                                                sb.Replace(match.Value, role.Caption);

                                            }
                                            break;
                                        }
                                    case "@AttributeName@":
                                        {

                                            if ((Convert.ToInt32(obj["TemplateID"]) == 9) || (Convert.ToInt32(obj["TemplateID"]) == 12))
                                            {
                                                var formattedvalue = GetCurrency(Convert.ToInt32(obj["ToValue"]));
                                                sb.Replace(match.Value, Convert.ToString(formattedvalue) + " " + proxy.MarcomManager.GlobalAdditionalSettings[1].CurrencyFormatvalue[0].Symbol);
                                            }
                                            else if ((Convert.ToInt32(obj["TemplateID"]) == 7) || (Convert.ToInt32(obj["TemplateID"]) == 6) || (Convert.ToInt32(obj["TemplateID"]) == 21))
                                            {
                                                sb.Replace(match.Value, "<a href='javascript:void(0);' data-id=\"costcenterlink\" data-costcentreid='" + obj["AssocitedEntityID"] + "'>" + Convert.ToString(obj["AttributeName"]) + "</a>");
                                            }
                                            else
                                            {
                                                sb.Replace(match.Value, Convert.ToString(obj["AttributeName"]));
                                            }

                                            break;
                                        }

                                    case "@Fromvalue@":
                                        {
                                            sb.Replace(match.Value, Convert.ToString(obj["FromValue"]));
                                            break;
                                        }
                                    case "@filename@":
                                        {
                                            //if (Convert.ToInt32(obj["TemplateID"]) == 29 || Convert.ToInt32(obj["TemplateID"]) == 30)
                                            //{
                                            //    sb.Replace(match.Value, Convert.ToString(obj["ToValue"]));
                                            //}
                                            //else
                                            //{
                                            var filedetails = (from tt in tx.PersistenceManager.CommonRepository.Query<FileDao>() where tt.Id == Convert.ToInt32(obj["ToValue"]) select tt).FirstOrDefault();
                                            if (filedetails == null)
                                                sb.Replace(match.Value, Convert.ToString(obj["AttributeName"]));
                                            else
                                                sb.Replace(match.Value, "<a target=\"_blank\" href=\"download.aspx?FileID=" + filedetails.Fileguid + "&amp;FileFriendlyName=" + Convert.ToString(filedetails.Name) + "&amp;Ext=" + filedetails.Extension + "\">" + Convert.ToString(filedetails.Name) + "</a>");
                                            //}
                                            break;
                                        }
                                    case "@linkname@":
                                        {
                                            var linkdetails = (from tt in tx.PersistenceManager.CommonRepository.Query<LinksDao>() where tt.ID == Convert.ToInt32(obj["ToValue"]) select tt).FirstOrDefault();
                                            if (linkdetails == null)
                                                sb.Replace(match.Value, Convert.ToString(obj["AttributeName"]));
                                            else
                                                sb.Replace(match.Value, "<a href=\"javascript:void(0);\" data-name=\"" + linkdetails.URL + "\"  data-id=\"" + linkdetails.ID + "\" data-command=\"openlink\">" + Convert.ToString(linkdetails.Name) + "</a>");
                                            break;
                                        }
                                    case "@TaskStatus@":
                                        {
                                            sb.Replace(match.Value, Convert.ToString(obj["ToValue"]));
                                            break;
                                        }
                                    case "@checklistname@":
                                        {
                                            sb.Replace(match.Value, Convert.ToString(obj["AttributeName"]));
                                            break;
                                        }

                                    case "@taskTypeName@":
                                        {
                                            var taskdetails = (from tt in tx.PersistenceManager.PlanningRepository.Query<EntityDao>() where tt.Id == Convert.ToInt32(obj["EntityID"]) select tt).FirstOrDefault();
                                            var tasktypename = (from type in tx.PersistenceManager.PlanningRepository.Query<EntityTypeDao>() where type.Id == taskdetails.Typeid select type).FirstOrDefault();
                                            sb.Replace(match.Value, tasktypename.Caption);
                                            // sb.Replace(match.Value, Convert.ToString(obj["ToValue"]));
                                            break;
                                        }
                                    case "@taskName@":
                                        {
                                            if ((Convert.ToInt32(obj["TemplateID"]) == 113) || (Convert.ToInt32(obj["TemplateID"]) == 114) || (Convert.ToInt32(obj["TemplateID"]) == 115))
                                            {
                                                sb.Replace(match.Value, "");
                                            }
                                            else
                                            {
                                                sb.Replace(match.Value, "<a href='javascript:void(0);' data-id=\"feedpath\" data-parentid='" + obj["ParentID"] + "'data-entityid='" + obj["EntityID"] + "' data-typeid='" + obj["Typeid"] + "' >" + entityname + "</a>");
                                            }
                                            //sb.Replace(match.Value, Convert.ToString(obj["ToValue"]));
                                            break;
                                        }
                                    case "@checkliststatus@":
                                        {
                                            sb.Replace(match.Value, Convert.ToString(obj["ToValue"]));
                                            break;
                                        }
                                    case "@TasksMembers@":
                                        {
                                            string commaseperatedusers = "";

                                            foreach (var c in obj["ToValue"].ToString().Split(','))
                                            {
                                                if (c != "")
                                                {
                                                    var user1 = tx.PersistenceManager.AccessRepository.Get<UserDao>(Convert.ToInt32(c));
                                                    if (user1.Id == Convert.ToInt32(obj["Actor"]))
                                                    {
                                                        commaseperatedusers = commaseperatedusers + "," + "Oneself";
                                                    }
                                                    else
                                                    {
                                                        commaseperatedusers = commaseperatedusers + ',' + user1.FirstName + ' ' + user1.LastName;

                                                    }
                                                    commaseperatedusers = commaseperatedusers.TrimStart().TrimStart(',');
                                                }
                                            }
                                            sb.Replace(match.Value, commaseperatedusers);
                                            break;
                                        }

                                    case "@CostCenterName@":
                                        {
                                            sb.Replace(match.Value, "<a href='javascript:void(0);' data-id=\"costcenterlink\" data-costcentreid='" + obj["AssocitedEntityID"] + "'>" + obj["AttributeName"] + "</a>");

                                            //sb.Replace(match.Value, Convert.ToString(obj["AttributeName"]));
                                            break;
                                        }
                                    case "@AssetName@":
                                        {


                                            var Assetdetails = (from tt in tx.PersistenceManager.CommonRepository.Query<AssetsDao>() where tt.ID == Convert.ToInt32(obj["AssetId"]) select tt).FirstOrDefault();
                                            if (Assetdetails == null)
                                                sb.Replace(match.Value, Convert.ToString(obj["AttributeName"]));
                                            else
                                                sb.Replace(match.Value, Assetdetails.Name);

                                            //    sb.Replace(match.Value, "<a href='javascript:void(0);' data-id=\"Assetpath\"data-entityid='" + Assetdetails.EntityID + "'data-Assetid='" + Assetdetails.ID + "' data-typeid='" + obj["Typeid"] + "' >" + Assetdetails.Name + "</a>");
                                            //sb.Replace(match.Value, "this");
                                            break;
                                        }
                                    case "@VersionNo@":
                                        {



                                            if (Convert.ToString(obj["Version"]) != null)
                                                sb.Replace(match.Value, Convert.ToString(obj["Version"]));
                                            else
                                                sb.Replace(match.Value, "0");


                                            break;
                                        }

                                }
                            }
                            catch (Exception ex)
                            { }
                        }

                        feedObj.FeedText = Convert.ToString(sb);
                        while (feedObj.FeedText.EndsWith("in"))
                            feedObj.FeedText = feedObj.FeedText.Substring(0, feedObj.FeedText.Length - 2).Trim();
                        IList<IFeedComment> listOfFeedComment = new List<IFeedComment>();
                        foreach (var objcomment in GetFeedcomments)
                        {
                            FeedComment feedCommentObj = new FeedComment();
                            if (Convert.ToInt32(objcomment["FeedID"]) == Convert.ToInt32(obj["ID"]))
                            {
                                feedCommentObj.Id = Convert.ToInt32(objcomment["ID"]);
                                feedCommentObj.Feedid = Convert.ToInt32(objcomment["FeedID"]);
                                feedCommentObj.Comment = Convert.ToString(objcomment["Comment"]);
                                feedCommentObj.UserName = Convert.ToString(objcomment["UserFirstName"] + " " + objcomment["UserLastName"]);
                                feedCommentObj.Usermail = Convert.ToString(objcomment["UserEmail"]);
                                feedCommentObj.Actor = Convert.ToInt32(objcomment["Actor"]);

                                TimeSpan difference1 = (proxy.MarcomManager.UserManager.TaskFeedInitialRequestedTime - DateTime.Parse(objcomment["CommentedOn"].ToString()));
                                if (difference1.Days > 0)
                                    if (difference1.Days > 1)
                                        feedCommentObj.CommentedOn = Convert.ToString(DateTime.Parse(objcomment["CommentedOn"].ToString()) + offSet);
                                    else
                                        feedCommentObj.CommentedOn = "Yesterday at " + (DateTime.Parse(objcomment["CommentedOn"].ToString()) + offSet).ToShortTimeString();
                                else if (difference1.Hours > 0)
                                    if (difference1.Days < 2)
                                        feedCommentObj.CommentedOn = "about an hour ago";
                                    else
                                        feedCommentObj.CommentedOn = difference1.Hours + " hours ago";
                                else if (difference1.Minutes > 0)
                                    feedCommentObj.CommentedOn = difference1.Minutes + " minutes ago";
                                else
                                    feedCommentObj.CommentedOn = "Few seconds ago";


                                listOfFeedComment.Add(feedCommentObj);

                            }
                            feedObj.FeedComment = listOfFeedComment;
                        }

                        // feedObj.FeedComment = listOfFeedComment;
                        listfeedselection.Add(feedObj);
                    }
                    if (isForRealTimeUpdate && childEntiyResult.Count > 0)
                        proxy.MarcomManager.UserManager.TaskFeedRecentlyUpdatedTime = DateTimeOffset.UtcNow;
                    return listfeedselection;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                proxy.MarcomManager.UserManager.TaskFeedLock = false;
            }
        }

        public string IncludePathForFeed(CommonManagerProxy proxy, ITransaction tx, int feedid)
        {
            try
            {
                IList<MultiProperty> prplst = new List<MultiProperty>();
                prplst.Add(new MultiProperty { propertyName = FeedDao.PropertyNames.Id, propertyValue = feedid });
                IList<FeedDao> feedentityid = new List<FeedDao>();
                string str = "";

                //using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                //{
                feedentityid = tx.PersistenceManager.CommonRepository.GetEquals<FeedDao>(prplst);

                var list = (proxy.MarcomManager.MetadataManager.GetPath(Convert.ToInt32(feedentityid[0].Entityid))).Cast<Hashtable>();

                foreach (var item in list)
                {
                    if (list.Count() > 1)
                    {
                        str = str + " / " + "<a href='javascript:void(0);' data-id=\"feedpath\" data-parentid='" + item["ParentID"] + "'data-entityid='" + item["ID"] + "' data-typeid='" + item["TypeID"] + "' >" + item["Name"] + "</a>";
                    }
                    else
                    {
                        str = str + "<a href='javascript:void(0);' data-id=\"feedpath\" data-parentid='" + item["ParentID"] + "'data-entityid='" + item["ID"] + "' data-typeid='" + item["TypeID"] + "' >" + item["Name"] + "</a>";
                        return str;
                    }
                }
                str = str.Substring(1, str.Length - 1).TrimStart('/');
                //tx.Commit();
                //}
                return str;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public string IncludePathForNotification(CommonManagerProxy proxy, ITransaction tx, int notificationId)
        {
            try
            {
                IList<MultiProperty> prplst = new List<MultiProperty>();
                prplst.Add(new MultiProperty { propertyName = UserNotificationDao.PropertyNames.Id, propertyValue = notificationId });
                IList<UserNotificationDao> notificationentityid = new List<UserNotificationDao>();
                string str = "";

                notificationentityid = tx.PersistenceManager.CommonRepository.GetEquals<UserNotificationDao>(prplst);
                if (notificationentityid[0].Typeid == 4 || notificationentityid[0].Typeid == 6)
                {
                    var fetchentityid = (from tt in tx.PersistenceManager.PlanningRepository.Query<EntityTaskDao>() where tt.ID == Convert.ToInt32(notificationentityid[0].Entityid) select tt.EntityID).FirstOrDefault();
                    var list1 = (proxy.MarcomManager.MetadataManager.GetPath(Convert.ToInt32(fetchentityid))).Cast<Hashtable>();
                    foreach (var item in list1)
                    {
                        if (list1.Count() > 1)
                        {
                            str = str + " / " + "<a data-id=\"notifypath\" data-entityid='" + item["ID"] + "' href=\"javascript:void(0);\" >" + item["Name"] + "</a>";
                        }
                        else
                        {
                            str = str + "<a data-id=\"notifypath\" data-entityid='" + item["ID"] + "' href=\"javascript:void(0);\" >" + item["Name"] + "</a>";
                            return str;
                        }
                    }
                }
                else
                {
                    var list = (proxy.MarcomManager.MetadataManager.GetPath(Convert.ToInt32(notificationentityid[0].Entityid))).Cast<Hashtable>();

                    foreach (var item in list)
                    {
                        if (list.Count() > 1)
                        {
                            str = str + " / " + "<a data-id=\"notifypath\" data-entityid='" + item["ID"] + "' href=\"javascript:void(0);\" >" + item["Name"] + "</a>";
                        }
                        else
                        {
                            str = str + "<a data-id=\"notifypath\" data-entityid='" + item["ID"] + "' href=\"javascript:void(0);\" >" + item["Name"] + "</a>";
                            return str;
                        }
                    }
                }
                str = str.Substring(1, str.Length - 1).TrimStart('/');
                //tx.Commit();
                //}
                return str;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public int UpdateIsviewedStatusNotification(CommonManagerProxy commonManagerProxy, int UserId, string ids, int flag)
        {

            try
            {

                IEnumerable<Hashtable> listresult;

                StringBuilder strqry = new StringBuilder();
                if (flag == 0)
                    strqry.AppendLine("update CM_User_Notification set IsViewed=1 where UserID= ? and id in  (" + ids + ")");
                else
                    strqry.AppendLine("update CM_User_Notification set IsViewed=1 where UserID= ?");
                using (ITransaction tx = commonManagerProxy.MarcomManager.GetTransaction())
                {
                    var a = tx.PersistenceManager.CommonRepository.ExecuteQuerywithMinParam(strqry.ToString(), UserId);

                    tx.Commit();
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
            return 1;
        }


        /// <summary>
        /// Gets the notification by ids.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="notificationid">The notificationid.</param>
        /// <param name="UserId">The user id.</param>
        /// <returns>IUserNotification</returns>
        public Tuple<IList<INotificationSelection>, int, IList> GetNotification(CommonManagerProxy proxy, int flag, int pageNo = 0)
        {

            IList<UserNotificationDao> notificationtionRow = new List<UserNotificationDao>();
            IList<UserNotificationDao> notificationtionunviewed = new List<UserNotificationDao>();
            IList<UserNotificationDao> addoldviewed = new List<UserNotificationDao>();
            int countofunread = 0;

            TimeSpan offSet = TimeSpan.Parse(proxy.MarcomManager.User.TimeZone.TrimStart('+'));
            IList<INotificationSelection> listnotificationselection = new List<INotificationSelection>();
            try
            {

                IUserNotification userNotification = new UserNotification();
                if (proxy.MarcomManager.AccessManager.CheckAccess(Modules.Admin, 4, FeatureID.Report, OperationId.Self) == true)
                {
                    using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                    {
                        UserNotificationDao userNotificationdao = new UserNotificationDao();
                        NotificationTypeDao notificatiotypedao = new NotificationTypeDao();
                        UserDao user = new UserDao();
                        UserDao user1 = new UserDao();
                        IList<MultiProperty> prplst = new List<MultiProperty>();
                        prplst.Add(new MultiProperty { propertyName = UserNotificationDao.PropertyNames.Userid, propertyValue = proxy.MarcomManager.User.Id });
                        int count = 0;

                        var notificationSelectQuery = new StringBuilder();
                        if (flag == 1)
                        {
                            //notificationtionRow = tx.PersistenceManager.CommonRepository.GetEquals<UserNotificationDao>(prplst).OrderByDescending(x => x.CreatedOn).ToList();
                            if (pageNo > 0)
                            {
                                pageNo = pageNo * 10;
                            }

                            string feedSelectQuery = "SELECT * FROM CM_User_Notification cun WHERE cun.UserID = ? " +
                                                       " and cun.UserID <> cun.ActorID ORDER BY cun.CreatedOn desc OFFSET ? ROWS FETCH NEXT 20 ROWS ONLY";

                            var notificationresult = (tx.PersistenceManager.CommonRepository.ExecuteQuerywithMinParam(feedSelectQuery, proxy.MarcomManager.User.Id, pageNo)).Cast<Hashtable>();

                            foreach (var objres in notificationresult)
                            {
                                UserNotificationDao feedObj = new UserNotificationDao();

                                feedObj.Id = Convert.ToInt32(objres["ID"]);
                                feedObj.Userid = Convert.ToInt32(objres["UserID"]);
                                feedObj.Entityid = Convert.ToInt32(objres["EntityID"]);
                                feedObj.Actorid = Convert.ToInt32(objres["ActorID"]);
                                feedObj.CreatedOn = DateTimeOffset.Parse(Convert.ToString(objres["CreatedOn"]));
                                feedObj.Typeid = Convert.ToInt32(objres["TypeID"]);
                                feedObj.IsViewed = Convert.ToBoolean(objres["IsViewed"]);
                                feedObj.IsSentInMail = Convert.ToBoolean(objres["IsSentInMail"]);
                                feedObj.TypeName = Convert.ToString(objres["TypeName"]);
                                feedObj.AttributeName = Convert.ToString(objres["AttributeName"]);
                                feedObj.FromValue = Convert.ToString(objres["FromValue"]);
                                feedObj.ToValue = Convert.ToString(objres["ToValue"]);
                                notificationtionRow.Add(feedObj);
                            }
                        }
                        else if (flag == 2)
                        {
                            notificationtionunviewed = (from tt in tx.PersistenceManager.CommonRepository.Query<UserNotificationDao>() where tt.Userid == proxy.MarcomManager.User.Id && tt.Actorid != proxy.MarcomManager.User.Id && tt.IsViewed == false select tt).ToList().OrderByDescending(x => x.CreatedOn.ToString("yyyy-MM-dd hh:mm:ss tt+zz")).ToList();
                            countofunread = notificationtionunviewed.Count();
                            notificationtionRow = notificationtionunviewed;

                        }
                        else
                        {
                            notificationtionunviewed = (from tt in tx.PersistenceManager.CommonRepository.GetEquals<UserNotificationDao>(prplst).OrderByDescending(x => x.CreatedOn).ToList() where tt.IsViewed == false select tt).ToList();

                            if (notificationtionunviewed.Count() >= 5)
                            {
                                for (int i = 0; i < notificationtionunviewed.Count(); i++)
                                {
                                    notificationtionRow.Add(notificationtionunviewed[i]);
                                }
                            }
                            else if (notificationtionunviewed.Count() < 5 && notificationtionunviewed.Count() != 0)
                            {
                                for (int i = 0; i < notificationtionunviewed.Count(); i++)
                                {
                                    notificationtionRow.Add(notificationtionunviewed[i]);
                                }
                            }

                            countofunread = notificationtionunviewed.Count();
                            if (notificationtionunviewed.Count() < 5)
                            {
                                addoldviewed = (from tt in tx.PersistenceManager.CommonRepository.GetEquals<UserNotificationDao>(prplst).OrderByDescending(x => x.CreatedOn).ToList() where tt.IsViewed == true select tt).ToList().Take((5 - notificationtionunviewed.Count())).ToList();
                                //addoldviewed = tx.PersistenceManager.CommonRepository.GetEquals<UserNotificationDao>(prplst).OrderByDescending(x => x.CreatedOn).Take((5 - notificationtionunviewed.Count())).ToList();
                                foreach (var y in addoldviewed)
                                {
                                    //if (y.IsViewed == true)
                                    //{
                                    notificationtionRow.Add(y);
                                    //}
                                }

                            }
                        }





                        foreach (var userDefSubscriptionRow in notificationtionRow)
                        {
                            try
                            {
                                INotificationSelection notificationText = new NotificationSelection();
                                user = tx.PersistenceManager.CommonRepository.Get<UserDao>(userDefSubscriptionRow.Actorid);
                                user1 = tx.PersistenceManager.CommonRepository.Get<UserDao>(userDefSubscriptionRow.Userid);
                                notificationText.Notificationid = userDefSubscriptionRow.Id;
                                notificatiotypedao = tx.PersistenceManager.CommonRepository.Get<NotificationTypeDao>(userDefSubscriptionRow.Typeid);
                                var template = notificatiotypedao.Template;

                                var entitydetails = tx.PersistenceManager.PlanningRepository.Get<EntityDao>(userDefSubscriptionRow.Entityid);

                                TimeSpan difference = (DateTime.Parse(DateTimeOffset.Now.ToString("yyyy-MM-dd hh:mm:ss tt")) - DateTime.Parse(userDefSubscriptionRow.CreatedOn.ToString("yyyy-MM-dd hh:mm:ss tt")));
                                if (difference.Days > 0)
                                    if (difference.Days > 1)
                                        notificationText.NotificationHappendTime = Convert.ToString(DateTime.Parse(userDefSubscriptionRow.CreatedOn.ToString()) + offSet);
                                    else
                                        notificationText.NotificationHappendTime = "Yesterday at " + (DateTime.Parse(userDefSubscriptionRow.CreatedOn.ToString()) + offSet).ToShortTimeString();
                                else if (difference.Hours > 0)
                                    if (difference.Hours < 2)
                                        notificationText.NotificationHappendTime = "about an hour ago";
                                    else
                                        notificationText.NotificationHappendTime = difference.Hours + " hours ago";
                                else if (difference.Minutes > 0)
                                    notificationText.NotificationHappendTime = difference.Minutes + " minutes ago";
                                else
                                    notificationText.NotificationHappendTime = "Few seconds ago";

                                StringBuilder sb = new StringBuilder(template.ToString());
                                foreach (Match match in Regex.Matches(template, @"@(.+?)@"))
                                {
                                    switch (match.Value.Trim())
                                    {
                                        case "@ActorName@":
                                            {
                                                sb.Replace(match.Value, user.FirstName + ' ' + user.LastName);
                                                break;
                                            }
                                        case "@AttributeName@":
                                            {
                                                sb.Replace(match.Value, userDefSubscriptionRow.AttributeName);
                                                break;
                                            }
                                        case "@OldValue@":
                                            {
                                                try
                                                {
                                                    if (Convert.ToInt32(userDefSubscriptionRow.Typeid) == 22 || Convert.ToInt32(userDefSubscriptionRow.Typeid) == 36 || Convert.ToInt32(userDefSubscriptionRow.Typeid) == 37)
                                                    {
                                                        var s = GetCurrency(Convert.ToInt32(userDefSubscriptionRow.FromValue.ToString()));
                                                        sb.Replace(match.Value, s.ToString() + " " + proxy.MarcomManager.GlobalAdditionalSettings[1].CurrencyFormatvalue[0].Symbol);
                                                    }
                                                    else
                                                    {
                                                        sb.Replace(match.Value, userDefSubscriptionRow.FromValue);
                                                    }
                                                }
                                                catch (Exception ex)
                                                {

                                                }
                                                break;
                                            }
                                        case "@NewValue@":
                                            {
                                                try
                                                {
                                                    if (Convert.ToInt32(userDefSubscriptionRow.Typeid) == 22 || Convert.ToInt32(userDefSubscriptionRow.Typeid) == 36 || Convert.ToInt32(userDefSubscriptionRow.Typeid) == 37)
                                                    {
                                                        var s = GetCurrency(Convert.ToInt32(userDefSubscriptionRow.ToValue.ToString()));
                                                        sb.Replace(match.Value, s.ToString() + " " + proxy.MarcomManager.GlobalAdditionalSettings[1].CurrencyFormatvalue[0].Symbol);
                                                    }
                                                    else
                                                    {
                                                        sb.Replace(match.Value, userDefSubscriptionRow.ToValue);
                                                    }
                                                }
                                                catch (Exception ex)
                                                {

                                                }
                                                break;
                                            }
                                        case "@Users@":
                                            {
                                                sb.Replace(match.Value, user1.FirstName + " " + user1.LastName);
                                                break;
                                            }
                                        case "@role@":
                                            {
                                                var currententityroleobj = tx.PersistenceManager.CommonRepository.Query<BaseEntityDao>().Where(a => a.Id == userDefSubscriptionRow.Entityid).SingleOrDefault();
                                                var entitypeobj = tx.PersistenceManager.CommonRepository.Query<EntityTypeDao>().Where(a => a.Id == currententityroleobj.Typeid).SingleOrDefault();
                                                if (entitypeobj.IsAssociate)
                                                {
                                                    var role = tx.PersistenceManager.CommonRepository.Get<RoleDao>(Convert.ToInt32(userDefSubscriptionRow.ToValue));
                                                    sb.Replace(match.Value, role.Caption);
                                                }
                                                else
                                                {
                                                    var role = tx.PersistenceManager.CommonRepository.Get<EntityTypeRoleAclDao>(Convert.ToInt32(userDefSubscriptionRow.ToValue));
                                                    sb.Replace(match.Value, role.Caption);
                                                }
                                                break;
                                            }
                                        case "@EntityTypeName@":
                                            {
                                                if (Convert.ToInt32(userDefSubscriptionRow.Typeid) == 40 || Convert.ToInt32(userDefSubscriptionRow.Typeid) == 41 || Convert.ToInt32(userDefSubscriptionRow.Typeid) == 42)
                                                {
                                                    var taskdet = (from tt in tx.PersistenceManager.CommonRepository.Query<EntityDao>() where tt.Id == Convert.ToInt32(userDefSubscriptionRow.ToValue) select tt.Typeid).FirstOrDefault();
                                                    var tasktypename = tx.PersistenceManager.CommonRepository.Get<EntityTypeDao>(Convert.ToInt32(taskdet));
                                                    sb.Replace(match.Value, tasktypename.Caption);
                                                }
                                                else
                                                {
                                                    var entitytypename = tx.PersistenceManager.CommonRepository.Get<EntityTypeDao>(Convert.ToInt32(entitydetails.Typeid));
                                                    sb.Replace(match.Value, entitytypename.Caption);
                                                }
                                                break;
                                            }
                                        case "@EntityTypeNamefortask@":
                                            {

                                                sb.Replace(match.Value, userDefSubscriptionRow.TypeName);
                                                break;
                                            }
                                        case "@EntityName@":
                                            {

                                                if (Convert.ToInt32(userDefSubscriptionRow.Typeid) == 40 || Convert.ToInt32(userDefSubscriptionRow.Typeid) == 41 || Convert.ToInt32(userDefSubscriptionRow.Typeid) == 42)
                                                {
                                                    var taskdet = (from tt in tx.PersistenceManager.CommonRepository.Query<EntityDao>() where tt.Id == Convert.ToInt32(userDefSubscriptionRow.ToValue) select tt).FirstOrDefault();
                                                    // sb.Replace(match.Value, taskdet.Name);
                                                    sb.Replace(match.Value, "<a href='javascript:void(0);' ng-click=\"NotificationRedirectEntityPath('" + taskdet.Id + "')\"  data-id=\"notifypath\" data-entityid='" + taskdet.Id + "' data-typeid='" + taskdet.Typeid + "' >" + taskdet.Name + "</a>");
                                                }
                                                else
                                                {
                                                    if (userDefSubscriptionRow.Typeid == 26)
                                                    {
                                                        var costcentredetails = (from tt in tx.PersistenceManager.CommonRepository.Query<EntityDao>() where tt.Id == entitydetails.Id select tt).FirstOrDefault();
                                                        sb.Replace(match.Value, "<a href='javascript:void(0);' data-id=\"costcenterlink\" data-costcentreid='" + costcentredetails.Parentid + "'>" + userDefSubscriptionRow.AttributeName + "</a>");
                                                    }
                                                    else if (userDefSubscriptionRow.Typeid == 20)
                                                    {

                                                        var costcentredetails = (from tt in tx.PersistenceManager.CommonRepository.Query<EntityCostReleationsDao>() where tt.EntityId == entitydetails.Parentid select tt).FirstOrDefault();
                                                        sb.Replace(match.Value, "<a href='javascript:void(0);' data-id=\"costcenterlink\" data-costcentreid='" + costcentredetails.CostcenterId + "'>" + userDefSubscriptionRow.AttributeName + "</a>");
                                                    }
                                                    else
                                                    {
                                                        sb.Replace(match.Value, "<a href='javascript:void(0);' ng-click=\"NotificationRedirectEntityPath('" + entitydetails.Id + "')\" data-id=\"notifypath\" data-entityid='" + entitydetails.Id + "' data-typeid='" + entitydetails.Typeid + "' >" + entitydetails.Name + "</a>");
                                                    }
                                                }

                                                // sb.Replace(match.Value, entitydetails.Name);
                                                break;
                                            }
                                        case "@fundingrequest@":
                                            {
                                                if (flag == 1)
                                                {
                                                    sb.Replace(match.Value, "<a href='javascript:void(0);'   data-id=\"openpopupforfundrequestforviewall\" data-entityid='" + entitydetails.Id + "' data-typeid='" + entitydetails.Typeid + "' >" + entitydetails.Name + "</a>");
                                                }
                                                else
                                                {
                                                    sb.Replace(match.Value, "<a href='javascript:void(0);' data-entityid='" + entitydetails.Id + "' data-typeid='" + entitydetails.Typeid + "' ng-click=\"OpenFundRequestActionfromNotify('" + entitydetails.Id + "')\"   >" + entitydetails.Name + "</a>");
                                                }
                                                break;
                                            }
                                        case "@FundingRequestState@":
                                            {
                                                sb.Replace(match.Value, userDefSubscriptionRow.ToValue.ToString());
                                                break;
                                            }
                                        case "@TransferAmount@":
                                            {
                                                sb.Replace(match.Value, Convert.ToString(userDefSubscriptionRow.AttributeName));
                                                break;
                                            }
                                        case "@FromCC@":
                                            {
                                                sb.Replace(match.Value, Convert.ToString(userDefSubscriptionRow.FromValue));
                                                break;
                                            }
                                        case "@ToCC@":
                                            {
                                                sb.Replace(match.Value, Convert.ToString(userDefSubscriptionRow.ToValue));
                                                break;
                                            }
                                        case "@AttributeValue@":
                                            {

                                                if (userDefSubscriptionRow.Typeid == 20)
                                                {
                                                    var formattedvalue = GetCurrency(Convert.ToInt32(userDefSubscriptionRow.ToValue));
                                                    sb.Replace(match.Value, Convert.ToString(formattedvalue));
                                                }
                                                else
                                                {
                                                    sb.Replace(match.Value, Convert.ToString(userDefSubscriptionRow.ToValue));
                                                }

                                                break;
                                            }
                                        case "@Path@":
                                            {

                                                var entityDetail = (from item in tx.PersistenceManager.PlanningRepository.Query<EntityDao>() where item.Id == userDefSubscriptionRow.Entityid select item).FirstOrDefault();
                                                EntityDao entparentdetails = new EntityDao();
                                                if (entityDetail.Typeid <= 50)   //for tasks
                                                {
                                                    entparentdetails = (from item in tx.PersistenceManager.PlanningRepository.Query<EntityDao>() where item.Id == entityDetail.Parentid select item).FirstOrDefault();
                                                    sb.Replace(match.Value, "<a href='javascript:void(0);' ng-click=\"NotificationRedirectEntityPath('" + entparentdetails.Id + "')\" data-id=\"notifypath\" on data-entityid='" + entparentdetails.Id + "' data-typeid='" + entparentdetails.Typeid + "' >" + entparentdetails.Name + "</a>");
                                                }
                                                else
                                                {
                                                    //sb.Replace(match.Value, "<a href='javascript:void(0);' ng-click=\"NotificationRedirectEntityPath('" + entityDetail.Id + "')\" data-id=\"notifypath\" data-entityid='" + entityDetail.Id + "' data-typeid='" + entityDetail.Typeid + "' >" + entityDetail.Name + "</a>");
                                                    sb.Replace("in @Path@", "");
                                                }
                                                break;
                                            }
                                        case "@TaskStatus@":
                                            {
                                                sb.Replace(match.Value, Convert.ToString(userDefSubscriptionRow.ToValue));
                                                break;
                                            }

                                        case "@checklistname@":
                                            {
                                                sb.Replace(match.Value, Convert.ToString(userDefSubscriptionRow.AttributeName));
                                                break;
                                            }
                                        case "@taskTypeName@":
                                            {
                                                var taskname = (from name in tx.PersistenceManager.TaskRepository.Query<EntityTaskDao>() where name.ID == userDefSubscriptionRow.Entityid select name).FirstOrDefault();
                                                var tasktype = (from type in tx.PersistenceManager.TaskRepository.Query<EntityTypeDao>() where type.Id == taskname.TaskType select type).FirstOrDefault();
                                                sb.Replace(match.Value, tasktype.Caption);
                                                break;
                                            }
                                        case "@taskName@":
                                            {
                                                //var taskname = (from name in tx.PersistenceManager.TaskRepository.Query<EntityTaskDao>() where name.ID == userDefSubscriptionRow.Entityid select name).FirstOrDefault();
                                                var taskdetail = (from item1 in tx.PersistenceManager.TaskRepository.Query<EntityTaskDao>()
                                                                  where item1.ID == userDefSubscriptionRow.Entityid
                                                                  select new { id = item1.ID, name = item1.Name, entityid = item1.EntityID, tasktype = item1.TaskType }).FirstOrDefault();
                                                var EntityTypeID = tx.PersistenceManager.TaskRepository.Query<BaseEntityDao>().Where(a => a.Id == userDefSubscriptionRow.Entityid).Select(a => a.Typeid).FirstOrDefault();

                                                if ((EntityTypeList)EntityTypeID == EntityTypeList.FundinngRequest)
                                                {
                                                    if (flag == 1)
                                                    {
                                                        sb.Replace(match.Value, "<a href='javascript:void(0);'   data-id=\"openpopupforfundrequestforviewall\" data-entityid='" + entitydetails.Id + "' data-typeid='" + entitydetails.Typeid + "' >" + entitydetails.Name + "</a>");
                                                    }
                                                    else
                                                    {
                                                        sb.Replace(match.Value, "<a href='javascript:void(0);' data-entityid='" + entitydetails.Id + "' data-typeid='" + entitydetails.Typeid + "' ng-click=\"OpenFundRequestActionfromNotify('" + entitydetails.Id + "')\"   >" + entitydetails.Name + "</a>");
                                                    }
                                                }
                                                else if (taskdetail != null && taskdetail.name != null)
                                                {
                                                    if (flag == 1)
                                                    {
                                                        sb.Replace(match.Value, "<a href='javascript:void(0);' ng-click=\"OpenTaskPopUp()\"  data-id=\"openpopupfortaskeditforviewall\" data-taskid='" + taskdetail.id + "'  data-entityid='" + taskdetail.entityid + "' data-typeid='" + taskdetail.tasktype + "' >" + taskdetail.name + "</a>");
                                                    }
                                                    else
                                                    {
                                                        sb.Replace(match.Value, "<a href='javascript:void(0);' ng-click=\"OpenTopNotificationTaskPopUp('" + taskdetail.id + "', '" + taskdetail.entityid + "', '" + taskdetail.tasktype + "')\"  data-id=\"openpopupfortaskedit\" data-taskid='" + taskdetail.id + "'  data-entityid='" + taskdetail.entityid + "' data-typeid='" + taskdetail.tasktype + "' >" + taskdetail.name + "</a>");
                                                    }
                                                }
                                                else
                                                {
                                                    sb.Replace(match.Value, "<a href='javascript:void(0);'  ng-click=\"NotificationRedirectEntityPath('" + entitydetails.Id + "')\"  data-id=\"notifypath\" data-entityid='" + entitydetails.Id + "' data-typeid='" + entitydetails.Typeid + "' >" + entitydetails.Name + "</a>");
                                                }
                                                break;
                                            }
                                        case "@checkliststatus@":
                                            {
                                                sb.Replace(match.Value, userDefSubscriptionRow.FromValue);
                                                break;
                                            }
                                        case "@filename@":
                                            {
                                                var filedetails = (from tt in tx.PersistenceManager.CommonRepository.Query<FileDao>() where tt.Id == Convert.ToInt32(userDefSubscriptionRow.ToValue) select tt).FirstOrDefault();
                                                if (filedetails == null)
                                                    sb.Replace(match.Value, Convert.ToString(userDefSubscriptionRow.AttributeName));
                                                else
                                                    sb.Replace(match.Value, "<a target=\"_blank\" href=\"download.aspx?FileID=" + filedetails.Fileguid + "&amp;FileFriendlyName=" + Convert.ToString(filedetails.Name) + "&amp;Ext=" + filedetails.Extension + "\">" + Convert.ToString(filedetails.Name) + "</a>");

                                                break;
                                            }
                                        case "@linkname@":
                                            {
                                                var linkdetails = (from tt in tx.PersistenceManager.CommonRepository.Query<LinksDao>() where tt.ID == Convert.ToInt32(userDefSubscriptionRow.ToValue) select tt).FirstOrDefault();
                                                if (linkdetails == null)
                                                    sb.Replace(match.Value, Convert.ToString(userDefSubscriptionRow.AttributeName));
                                                else
                                                    sb.Replace(match.Value, "<a href=\"javascript:void(0);\" data-name=\"" + linkdetails.URL + "\"  data-id=\"" + linkdetails.ID + "\" >" + Convert.ToString(linkdetails.Name) + "</a>");
                                                // sb.Replace(match.Value, userDefSubscriptionRow.FromValue);
                                                break;
                                            }
                                        case "@comment@":
                                            {
                                                sb.Replace(match.Value, Convert.ToString(userDefSubscriptionRow.ToValue));
                                                break;
                                            }
                                        case "@feedtext@":
                                            {
                                                IList<IFeedSelection> feedtextlist = new List<IFeedSelection>();
                                                //if(userDefSubscriptionRow.FromValue
                                                feedtextlist = GettingFeedsByEntityID(proxy, userDefSubscriptionRow.Entityid.ToString(), 0, false, Convert.ToInt32(userDefSubscriptionRow.FromValue));
                                                sb.Replace(match.Value, feedtextlist[0].FeedText.ToString());
                                                break;
                                            }
                                    }

                                }
                                notificationText.NotificationText = Convert.ToString(sb);
                                notificationText.UserName = user.FirstName + ' ' + user.LastName;
                                notificationText.UserEmail = user.Email;
                                notificationText.UserImage = user.Image;
                                notificationText.UserId = user.Id;        //actorid
                                notificationText.Isviewed = userDefSubscriptionRow.IsViewed;
                                notificationText.Typeid = userDefSubscriptionRow.Typeid;
                                if (notificationText.Typeid == 4 || notificationText.Typeid == 6)
                                {

                                    var tasklistId = (from t in tx.PersistenceManager.CommonRepository.Query<EntityTaskDao>() where t.ID == userDefSubscriptionRow.Entityid select new { tasklistID = t.TaskListID, EntityID = t.EntityID }).FirstOrDefault();
                                    notificationText.TaskListID = tasklistId.tasklistID;
                                    notificationText.TaskEntityID = tasklistId.EntityID;
                                }

                                listnotificationselection.Add(notificationText);
                            }
                            catch (Exception ex)
                            {

                            }
                        }

                        tx.Commit();
                    }

                }
            }

            catch (Exception ex)
            {

            }
            // FeedNotificationServer fns = new FeedNotificationServer();
            // IList<INotificationSelection> listofnotificationstosend = new List<INotificationSelection>();
            // listofnotificationstosend = (from item in listnotificationselection where item.Isviewed == false select item).ToList();
            //fns.SendMailForNotifications(listofnotificationstosend);
            IList broadcastMessages = GetBroadcastMessagesbyuser(proxy);
            Tuple<IList<INotificationSelection>, int, IList> notificationObjects = Tuple.Create(listnotificationselection, countofunread, broadcastMessages);

            return notificationObjects;
        }


        public bool CheckUserPermissionForEntity(CommonManagerProxy proxy, int entityID)
        {
            ITransaction tx = proxy.MarcomManager.GetTransaction();
            bool permission = false;
            try
            {

                if (proxy.MarcomManager.AccessManager.CheckUserAccess((int)Modules.Planning, (int)FeatureID.ViewEditAll, (int)OperationId.View))
                    permission = true;
                else
                {
                    var entityDetail = (from member in tx.PersistenceManager.AccessRepository.Query<EntityRoleUserDao>()
                                        where member.Userid == proxy.MarcomManager.User.Id && member.Entityid == entityID
                                        select member).FirstOrDefault();

                    if (entityDetail != null)
                        permission = true;
                }
                tx.Commit();
            }
            catch { tx.Rollback(); }
            return permission;
        }
        /// <summary>
        /// Getting Feeds by EntiyID and Last requested time
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="FeedId">The EntityID</param>
        /// <param name="lastFeedRequestedTime">The Last Requested Time</param>
        /// <returns>IList<IFeedSelection></IFeedSelection></returns>
        public IList<IFeedSelection> GettingFeedsByLastRequestedTime(CommonManagerProxy proxy, int entityId, string lastFeedRequestedTime)
        {
            try
            {
                if (lastFeedRequestedTime == "0")
                    lastFeedRequestedTime = DateTime.UtcNow.ToString();
                IList<IFeedSelection> listfeedselection = new List<IFeedSelection>();
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    //var templateDao = tx.PersistenceManager.CommonRepository.Query<FeedTemplateDao>();
                    //string feedSelectQuery = "select cmf.ID,cmf.Actor,cmf.TemplateID,cmf.HappenedOn,cmf.CommentedUpdatedOn,cmf.EntityID,cmf.TypeName,cmf.TypeName,cmf.AttributeName,cmf.FromValue,cmf.ToValue,pme.UniqueKey,pme.ID,pme.ParentID from CM_Feed cmf inner join PM_Entity pme on cmf.EntityID = pme.ID where pme.UniqueKey  like (SELECT pe.UniqueKey + '%' FROM   PM_Entity pe WHERE  pe.ID = '" + entityId + "')";
                    string feedSelectQuery = "select cmf.ID,cmf.Actor,cmf.TemplateID,cmf.HappenedOn,cmf.CommentedUpdatedOn,cmf.EntityID,cmf.TypeName,cmf.TypeName," +
                                             "cmf.AttributeName,cmf.FromValue,cmf.ToValue,pme.Name as 'EntityName',pme.UniqueKey as 'EntiyUniquekey',pme.ParentID 'EntiyParentID'," +
                                             "umuse.FirstName as 'UserFirstName',umuse.LastName 'UserLastName',umuse.Email as 'UserEmail',umuse.Image as 'UserImage'," +
                                             "umuse.TimeZone as 'UserTimeZone',cmt.Template as 'FeedTemplate'  from CM_Feed cmf inner join PM_Entity pme on cmf.EntityID = pme.ID inner join UM_User umuse on" +
                                             " umuse.ID = cmf.Actor inner join CM_Feed_Template cmt on cmt.ID = cmf.TemplateID where pme.ID= ? or pme.UniqueKey  like " +
                                             "(SELECT pe.UniqueKey + '.%' FROM   PM_Entity pe WHERE  pe.ID = ?) and cmf.HappenedOn < ?" +
                                             " ORDER BY pme.UniqueKey OFFSET 0 ROWS FETCH NEXT 20 ROWS ONLY";
                    var childEntiyResult = (tx.PersistenceManager.CommonRepository.ExecuteQuerywithMinParam(feedSelectQuery, entityId, entityId, lastFeedRequestedTime)).Cast<Hashtable>();
                    string dateformate = proxy.MarcomManager.GlobalAdditionalSettings[0].SettingValue.ToString().Replace('m', 'M');
                    dateformate += " hh:mm:ss tt";

                    foreach (var obj in childEntiyResult)
                    {
                        FeedSelection feedObj = new FeedSelection();
                        feedObj.FeedId = Convert.ToInt32(obj["ID"]);
                        feedObj.UserName = Convert.ToString(obj["UserFirstName"] + " " + obj["UserLastName"]);
                        feedObj.UserEmail = Convert.ToString(obj["UserEmail"]);
                        feedObj.UserImage = Convert.ToString(obj["UserImage"]);
                        string usertimezone = "05:30";
                        // TimeSpan offSet = TimeSpan.Parse(obj["UserTimeZone"].ToString());
                        TimeSpan offSet = TimeSpan.Parse(usertimezone);
                        feedObj.FeedHappendTime = (DateTime.Parse(obj["HappenedOn"].ToString()) + offSet).ToString(dateformate);
                        string template = Convert.ToString(obj["FeedTemplate"]);
                        StringBuilder sb = new StringBuilder(template);
                        //foreach (Match match in Regex.Matches(template, @"(?<!\w)@\w+"))
                        foreach (Match match in Regex.Matches(template, @"@(.+?)@"))
                        {
                            switch (match.Value.Trim())
                            {
                                case "@EntityTypeName@":
                                    {
                                        sb.Replace(match.Value, Convert.ToString(obj["TypeName"]));
                                        break;
                                    }
                                case "@EntityName@":
                                    {
                                        sb.Replace(match.Value, Convert.ToString(obj["EntityName"]));
                                        break;
                                    }
                            }
                        }
                        feedObj.FeedText = Convert.ToString(sb);
                        listfeedselection.Add(feedObj);
                    }
                    return listfeedselection;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets the type of all subscription.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <returns>IList</returns>
        public IList<IEntityType> GetEntityTypeforSubscription(CommonManagerProxy proxy, int ID)
        {

            try
            {
                IList<IEntityType> Entitytype = new List<IEntityType>();
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    EntityTypeDao EntityTypeDao = new EntityTypeDao();
                    EntityTypeDao = tx.PersistenceManager.MetadataRepository.Get<EntityTypeDao>(ID);
                    IEntityType entType = new BrandSystems.Marcom.Core.Metadata.EntityType();
                    entType.Id = EntityTypeDao.Id;
                    entType.Caption = EntityTypeDao.Caption;
                    entType.ColorCode = EntityTypeDao.ColorCode;
                    entType.ShortDescription = EntityTypeDao.ShortDescription;
                    Entitytype.Add(entType);

                    StringBuilder strqry = new StringBuilder();

                    strqry.AppendLine("WITH GetPath ");
                    strqry.AppendLine("AS ");
                    strqry.AppendLine("(");
                    strqry.AppendLine("SELECT ID, ParentActivityTypeID, ChildActivityTypeID FROM [dbo].[MM_EntityType_Hierarchy] pe WHERE pe.ParentActivityTypeID = ?");
                    strqry.AppendLine("UNION ALL");
                    strqry.AppendLine("SELECT ent.ID, ent.ParentActivityTypeID, ent.ChildActivityTypeID");
                    strqry.AppendLine("FROM [dbo].[MM_EntityType_Hierarchy] ent INNER JOIN GetPath AS Parent ON ent.ParentActivityTypeID = Parent.ChildActivityTypeID)");
                    strqry.AppendLine("SELECT distinct  ChildActivityTypeID,me.Caption,me.ID ,me.ColorCode,me.ShortDescription FROM GetPath  inner join MM_EntityType me on GetPath.ChildActivityTypeID=me.ID");
                    strqry.AppendLine("order by ChildActivityTypeID");

                    var listresult = (tx.PersistenceManager.MetadataRepository.ExecuteQuerywithMinParam(strqry.ToString(), ID)).Cast<Hashtable>();

                    foreach (var item in listresult)
                    {
                        entType = new BrandSystems.Marcom.Core.Metadata.EntityType();
                        entType.Id = Convert.ToInt32(item["ID"]);
                        entType.Caption = Convert.ToString(item["Caption"]);
                        entType.ColorCode = Convert.ToString(item["ColorCode"]);
                        entType.ShortDescription = Convert.ToString(item["ShortDescription"]);
                        Entitytype.Add(entType);
                    }

                    tx.Commit();
                    return Entitytype;

                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }



        /// <summary>
        /// Gets the Autosubscribed entity details
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <returns>IList</returns>
        public String GetAutoSubscriptionDetails(CommonManagerProxy proxy, int UserID, int EntityID)
        {
            //IList<IUserAutoSubscription> userAutoSubscription = new List<IUserAutoSubscription>();
            StringBuilder str = new StringBuilder();
            try
            {

                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    // var entityid = tx.PersistenceManager.CommonRepository.Get<EntityDao>(EntityDao.PropertyNames.Id, EntityID);
                    IList<MultiProperty> prplst = new List<MultiProperty>();
                    prplst.Add(new MultiProperty { propertyName = UserAutoSubscriptionDao.PropertyNames.Entityid, propertyValue = EntityID });
                    prplst.Add(new MultiProperty { propertyName = UserAutoSubscriptionDao.PropertyNames.Userid, propertyValue = UserID });

                    IList<MultiProperty> prplstformultisubscribtion = new List<MultiProperty>();
                    prplstformultisubscribtion.Add(new MultiProperty { propertyName = UserSubscriptionDao.PropertyNames.Entityid, propertyValue = EntityID });
                    prplstformultisubscribtion.Add(new MultiProperty { propertyName = UserSubscriptionDao.PropertyNames.Userid, propertyValue = UserID });

                    var autosubdao = tx.PersistenceManager.CommonRepository.GetEquals<UserAutoSubscriptionDao>(prplstformultisubscribtion);
                    var singlesub = tx.PersistenceManager.CommonRepository.GetEquals<UserSubscriptionDao>(prplst);


                    if (autosubdao.Count() > 0)
                    {
                        str.Append("Auto Subscribed");
                    }

                    else if (singlesub.Count() > 0)
                    {
                        str.Append("Unsubscribe");
                    }
                    else
                    {
                        str.Append("Subscribe");
                    }

                    //IUserAutoSubscription autosub = new UserAutoSubscription();
                    //foreach (var da in subdao)
                    //{
                    //    autosub.Entityid = da.Entityid;
                    //    autosub.EntityTypeid = da.EntityTypeid;                       
                    //    autosub.Userid = da.Userid;
                    //    userAutoSubscription.Add(autosub);
                    //}


                    tx.Commit();
                }


            }
            catch (Exception ex)
            {

            }
            return str.ToString();
        }




        /// <summary>
        /// selects WidgetTemplate.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="TemplateID">The WidgetTemplate ID.</param>
        /// <returns>IList</returns>
        public IList<IWidgetTemplate> WidgetTemplate_Select(CommonManagerProxy proxy, int TemplateID)
        {

            try
            {
                IList<IWidgetTemplate> widgetTemplatelist = new List<IWidgetTemplate>();
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    IList<WidgetTemplateDao> widgetTemplatedao;
                    widgetTemplatedao = tx.PersistenceManager.CommonRepository.GetAll<WidgetTemplateDao>();
                    tx.Commit();
                    var linqwidgetTemplatedao = from t in widgetTemplatedao select t;
                    if (TemplateID > 0)
                    {
                        linqwidgetTemplatedao = from t in linqwidgetTemplatedao where t.Id == TemplateID select t;
                    }
                    else
                    {
                        linqwidgetTemplatedao = from t in linqwidgetTemplatedao select t;
                    }

                    //var linqnavigationdao = from t in navigationdao select t;

                    foreach (var temp in linqwidgetTemplatedao.ToList())
                    {
                        IWidgetTemplate widgetTemplate = new WidgetTemplate();
                        widgetTemplate.Id = temp.Id;
                        widgetTemplate.TemplateName = temp.TemplateName;
                        widgetTemplate.TemplateDescription = temp.TemplateDescription;
                        widgetTemplatelist.Add(widgetTemplate);
                    }
                }
                return widgetTemplatelist;
            }
            catch (Exception ex)
            {


            }
            return null;
        }


        /// <summary>
        /// selects WidgetTypes.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="TypeID">The WidgetType ID.</param>
        /// <returns>IList</returns>
        public IList<IWidgetTypes> WidgetTypes_Select(CommonManagerProxy proxy, int userId, bool isAdmin, int typeId = 0)
        {
            try
            {

                IList<IWidgetTypes> widgetTypeslist = new List<IWidgetTypes>();
                StringBuilder roleId = new StringBuilder();

                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {

                    if (isAdmin == true)
                    {
                        string widgetTypeQuery = "SELECT Id,TypeName,ISDynamic FROM CM_WidgetTypes ";
                        var widgetTypeNames = tx.PersistenceManager.CommonRepository.ExecuteQuery(widgetTypeQuery).Cast<Hashtable>().ToList();
                        foreach (var temp in widgetTypeNames)
                        {
                            IWidgetTypes widgetTypes = new WidgetTypes();
                            widgetTypes.Id = (int)temp["Id"];
                            widgetTypes.TypeName = (string)temp["TypeName"];
                            widgetTypes.ISDynamic = (int)temp["ISDynamic"];
                            widgetTypeslist.Add(widgetTypes);
                        }
                    }
                    else
                    {

                        var GlobalRolelist = tx.PersistenceManager.AccessRepository.Query<GlobalRoleUserDao>().Where(a => a.Userid == userId).Select(t => t.GlobalRoleId);
                        //var roleList = from t in GlobalRolelist where t.Userid == userId select t;

                        foreach (var obj in GlobalRolelist)
                        {
                            roleId.Append(obj + ",");
                        }

                        string widgetQuery = "SELECT Distinct WidgetTypeID FROM CM_WidgetTypeRoles WHERE RoleID IN(" + roleId.ToString().TrimEnd(',') + ")";
                        var resutel = tx.PersistenceManager.CommonRepository.ExecuteQuery(widgetQuery).Cast<Hashtable>().ToList();
                        StringBuilder widgetTypeId = new StringBuilder();
                        foreach (var obs in resutel)
                        {
                            widgetTypeId.Append((int)obs["WidgetTypeID"] + ",");
                        }
                        string widgetTypeQuery = "SELECT Id,TypeName,ISDynamic FROM CM_WidgetTypes WHERE ID IN(" + widgetTypeId.ToString().TrimEnd(',') + ")";
                        var widgetTypeNames = tx.PersistenceManager.CommonRepository.ExecuteQuery(widgetTypeQuery).Cast<Hashtable>().ToList();
                        foreach (var temp in widgetTypeNames)
                        {
                            IWidgetTypes widgetTypes = new WidgetTypes();
                            widgetTypes.Id = (int)temp["Id"];
                            widgetTypes.TypeName = (string)temp["TypeName"];
                            widgetTypes.ISDynamic = (int)temp["ISDynamic"];
                            widgetTypeslist.Add(widgetTypes);
                        }
                    }
                    //var WidgetRolelist = tx.PersistenceManager.PlanningRepository.Query<WidgetTypeRolesDao>().Where(a => a.RoleID == );

                    //var userWidgetRoles = WidgetRolelist.Join(GlobalRolelist, wd => wd.RoleID, gu => gu.GlobalRoleId, (wd, gu) => 
                    //                       new { wd, gu }).Select(a => new { WidgetTypes = a.wd.WidgetTypeID });

                    //IList<WidgetTypesDao> widgetTypesdao;
                    //widgetTypesdao = tx.PersistenceManager.CommonRepository.GetAll<WidgetTypesDao>();
                    //tx.Commit();

                    //var linqwidgetTypesdao = from t in widgetTypesdao select t;
                    //if (TypeID > 0)
                    //{
                    //    linqwidgetTypesdao = from t in linqwidgetTypesdao where t.Id == TypeID select t;
                    //}
                    //else
                    //{
                    //    linqwidgetTypesdao = from t in linqwidgetTypesdao select t;
                    //}

                    ////var linqnavigationdao = from t in navigationdao select t;

                    //foreach (var temp in linqwidgetTypesdao.ToList())
                    //{
                    //    IWidgetTypes widgetTypes = new WidgetTypes();
                    //    widgetTypes.Id = temp.Id;
                    //    widgetTypes.TypeName = temp.TypeName;
                    //    widgetTypes.ISDynamic = temp.ISDynamic;
                    //    widgetTypeslist.Add(widgetTypes);
                    //}
                }
                return widgetTypeslist;
            }
            catch (Exception ex)
            {


            }
            return null;
        }

        /// <summary>
        /// select WidgetTypeRoles.
        /// </summary>
        /// <param name="WidgetTypeID">The WidgetType ID.</param>

        /// <returns>
        /// int[]
        /// </returns>
        public int[] GetWidgetTypeRolesByID(CommonManagerProxy proxy, int WidgetTypeID)
        {
            try
            {


                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    var GlobalRolelist = tx.PersistenceManager.PlanningRepository.Query<WidgetTypeRolesDao>();
                    var roleList = from t in GlobalRolelist where t.WidgetTypeID == WidgetTypeID select t;
                    int[] GlobalRoleId = new int[roleList.ToList().Count()];
                    for (var i = 0; i < roleList.ToList().Count(); i++)
                    {
                        GlobalRoleId[i] = roleList.ToList().ElementAt(i).RoleID;
                    }
                    //foreach (var val in roleList)
                    //{
                    //    GlobalRoleId.Add( val.Userid);
                    //}

                    tx.Commit();
                    return GlobalRoleId;

                }




            }
            catch (Exception ex)
            {
                return null;
            }
            return null;
        }


        /// <summary>
        /// selects WidgetTypeDimension.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="WidgetTypeID">The WidgetType ID.</param>
        /// <returns>IList</returns>
        public IList<IWidgetTypeDimension> WidgetTypeDimension_Select(CommonManagerProxy proxy, int WidgetTypeID)
        {
            try
            {
                IList<IWidgetTypeDimension> widgetTypedimensionlist = new List<IWidgetTypeDimension>();
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    IList<WidgetTypeDimensionDao> widgetTypesdimensiondao;
                    widgetTypesdimensiondao = tx.PersistenceManager.CommonRepository.GetAll<WidgetTypeDimensionDao>();
                    tx.Commit();
                    var linqwidgetTypesdimensiondao = from t in widgetTypesdimensiondao select t;
                    if (WidgetTypeID > 0)
                    {
                        linqwidgetTypesdimensiondao = from t in linqwidgetTypesdimensiondao where t.WidgetTypeID == WidgetTypeID select t;
                    }
                    else
                    {
                        linqwidgetTypesdimensiondao = from t in linqwidgetTypesdimensiondao select t;
                    }

                    //var linqnavigationdao = from t in navigationdao select t;

                    foreach (var temp in linqwidgetTypesdimensiondao.ToList())
                    {
                        IWidgetTypeDimension widgetTypesdimension = new WidgetTypeDimension();
                        widgetTypesdimension.Id = temp.Id;
                        widgetTypesdimension.WidgetTypeID = temp.WidgetTypeID;
                        widgetTypesdimension.DimensionName = temp.DimensionName;
                        widgetTypedimensionlist.Add(widgetTypesdimension);
                    }
                }
                return widgetTypedimensionlist;
            }
            catch (Exception ex)
            {


            }
            return null;
        }

        /// <summary>
        /// Inserts the WidgetTemplate.
        /// </summary>
        /// <param name="commonManagerProxy">The common manager proxy.</param>
        /// <param name="TemplateName">The TemplateName.</param>
        /// <returns>int</returns>
        internal int InsertWidgetTemplate(CommonManagerProxy commonManagerProxy, string TemplateName, string TemplateDescription)
        {

            try
            {
                WidgetTemplateDao dao = new WidgetTemplateDao();
                using (ITransaction tx = commonManagerProxy.MarcomManager.GetTransaction())
                {
                    // insert notification/subscription logic
                    if (tx.PersistenceManager.PlanningRepository.GetEquals<WidgetTemplateDao>(WidgetTemplateDao.PropertyNames.TemplateName, TemplateName).Count == 0)
                    {
                        dao.Id = dao.Id;
                        dao.TemplateName = TemplateName;
                        dao.TemplateDescription = TemplateDescription;
                        tx.PersistenceManager.PlanningRepository.Save<WidgetTemplateDao>(dao);
                        tx.Commit();
                    }
                }
                if (dao.Id > 0)
                {
                    string dashboardwidgetpath = Path.Combine(HttpRuntime.AppDomainAppPath, ConfigurationManager.AppSettings["dashboardwidget"]);
                    dashboardwidgetpath = dashboardwidgetpath + "\\" + "DashboardTemplate_" + dao.Id + ".xml";
                    XmlDocument mappingdoc = new XmlDocument();
                    XmlNode docNode = mappingdoc.CreateXmlDeclaration("1.0", "UTF-8", null);
                    mappingdoc.AppendChild(docNode);
                    XmlElement root = mappingdoc.CreateElement("root");
                    mappingdoc.AppendChild(root);
                    XmlComment comment = mappingdoc.CreateComment("widget below...");
                    root.AppendChild(comment);
                    //XmlNode rootnode = mappingdoc.AppendChild(mappingdoc.CreateElement("Dashboard-Template"));  
                    mappingdoc.Save(dashboardwidgetpath);
                }





                return dao.Id;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool WidgetTemplate_Update(CommonManagerProxy proxy, int ID, string TemplateName, string TemplateDescription)
        {
            try
            {
                WidgetTemplate _UpdateWidgetTemplate = new WidgetTemplate();
                //     if (proxy.MarcomManager.AccessManager.CheckAccess(Modules.Admin, 4, FeatureID.Report, OperationId.Self, 1) == true)
                //{
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    WidgetTemplateDao dao = new WidgetTemplateDao();
                    dao.Id = ID;
                    dao.TemplateName = TemplateName;
                    dao.TemplateDescription = TemplateDescription;
                    tx.PersistenceManager.CommonRepository.Save<WidgetTemplateDao>(dao);
                    tx.Commit();

                }

                return true;
            }
            //        return false; 
            //}

            catch (Exception ex)
            {

            }
            return false;
        }

        internal int InsertWidgetTypeRoles(CommonManagerProxy commonManagerProxy, int widgetTypeID, int roleID)
        {

            try
            {
                using (ITransaction tx = commonManagerProxy.MarcomManager.GetTransaction())
                {

                    WidgetTypeRolesDao dao = new WidgetTypeRolesDao();
                    dao.Id = dao.Id;
                    dao.WidgetTypeID = widgetTypeID;
                    dao.RoleID = roleID;
                    tx.PersistenceManager.PlanningRepository.Save<WidgetTypeRolesDao>(dao);
                    tx.Commit();
                    return dao.Id;

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return 0;
        }

        internal int InsertWidgetTemplateRoles(CommonManagerProxy commonManagerProxy, int WidgetTemplateID, int roleID)
        {

            try
            {
                using (ITransaction tx = commonManagerProxy.MarcomManager.GetTransaction())
                {

                    WidgetTemplateRolesDao dao = new WidgetTemplateRolesDao();
                    dao.Id = dao.Id;
                    dao.WidgetTemplateID = WidgetTemplateID;
                    dao.RoleID = roleID;
                    tx.PersistenceManager.PlanningRepository.Save<WidgetTemplateRolesDao>(dao);
                    tx.Commit();
                    return dao.Id;

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return 0;
        }


        /// Get the WidgetTemplateRoles  By TemplateID.
        /// </summary>
        /// <param name="WidgetTemplateID">The WidgetTemplateID.</param>
        /// <returns>int[] </returns>
        /// 
        public int[] GetWidgetTemplateRolesByTemplateID(CommonManagerProxy commonManagerProxy, int WidgetTemplateID)
        {
            //return null;
            try
            {


                using (ITransaction tx = commonManagerProxy.MarcomManager.GetTransaction())
                {
                    var GlobalRolelist = tx.PersistenceManager.PlanningRepository.Query<WidgetTemplateRolesDao>();
                    var roleList = from t in GlobalRolelist where t.WidgetTemplateID == WidgetTemplateID select t;
                    int[] GlobalRoleId = new int[roleList.ToList().Count()];
                    for (var i = 0; i < roleList.ToList().Count(); i++)
                    {
                        GlobalRoleId[i] = roleList.ToList().ElementAt(i).RoleID;
                    }
                    //foreach (var val in roleList)
                    //{
                    //    GlobalRoleId.Add( val.Userid);
                    //}

                    tx.Commit();
                    return GlobalRoleId;

                }




            }
            catch (Exception ex)
            {
                return null;
            }
            return null;
        }

        /// <summary>
        /// Delete the global role user.
        /// </summary>
        /// <param name="userid">The userid.</param>
        /// <returns>bool</returns>
        public bool DeleteWidgetTemplateRoles(CommonManagerProxy commonManagerProxy, int WidgetTemplateID)
        {
            //return null;
            try
            {


                using (ITransaction tx = commonManagerProxy.MarcomManager.GetTransaction())
                {
                    tx.PersistenceManager.PlanningRepository.DeleteByID<WidgetTemplateRolesDao>(WidgetTemplateRolesDao.PropertyNames.WidgetTemplateID, WidgetTemplateID);
                    tx.Commit();
                    return true;
                }




            }
            catch (Exception ex)
            {
                return false;
            }
            return false;
        }

        /// <summary>
        /// Delete the global role user.
        /// </summary>
        /// <param name="userid">The userid.</param>
        /// <returns>bool</returns>
        public bool DeleteWidgetTypeRoles(CommonManagerProxy commonManagerProxy, int WidgetTypeID)
        {
            //return null;
            try
            {


                using (ITransaction tx = commonManagerProxy.MarcomManager.GetTransaction())
                {
                    tx.PersistenceManager.PlanningRepository.DeleteByID<WidgetTypeRolesDao>(WidgetTypeRolesDao.PropertyNames.WidgetTypeID, WidgetTypeID);
                    tx.Commit();
                    return true;
                }




            }
            catch (Exception ex)
            {
                return false;
            }
            return false;
        }

        //public string NewsFeedTime(DateTime happendTime)
        //{
        //    DateTime startTime = DateTime.Now;

        //    DateTime HappendTime = DateTime.Now.AddSeconds(75);

        //    TimeSpan span = endTime.Subtract(startTime);

        //    var time = "";
        //    var delta = 0;
        //    const int SECOND = 1;
        //    const int MINUTE = 60 * SECOND;
        //    const int HOUR = 60 * MINUTE;
        //    const int DAY = 24 * HOUR;
        //    const int MONTH = 30 * DAY;

        //    if (delta < 0)
        //    {
        //        time = "not yet";
        //    }
        //    if (delta < 1 * MINUTE)
        //    {
        //        time = ts.Seconds == 1 ? "one second ago" : ts.Seconds + " seconds ago";
        //    }
        //    if (delta < 2 * MINUTE)
        //    {
        //        return "a minute ago";
        //    }
        //    if (delta < 45 * MINUTE)
        //    {
        //        return ts.Minutes + " minutes ago";
        //    }
        //    if (delta < 90 * MINUTE)
        //    {
        //        return "an hour ago";
        //    }
        //    if (delta < 24 * HOUR)
        //    {
        //        return ts.Hours + " hours ago";
        //    }
        //    if (delta < 48 * HOUR)
        //    {
        //        return "yesterday";
        //    }
        //    if (delta < 30 * DAY)
        //    {
        //        return ts.Days + " days ago";
        //    }
        //    if (delta < 12 * MONTH)
        //    {
        //        int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
        //        return months <= 1 ? "one month ago" : months + " months ago";
        //    }
        //    else
        //    {
        //        int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
        //        return years <= 1 ? "one year ago" : years + " years ago";
        //    }
        //}

        /// <summary>
        /// Get user information.
        /// </summary>
        /// <param name="proxy">UserID</param>
        /// <returns>list</returns>
        public IList<IFile> GetMyPageInfo(CommonManagerProxy proxy, int EntityID)
        {
            IList<IFile> flDetails = new List<IFile>();
            try
            {

                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {

                    var flDaoList = tx.PersistenceManager.CommonRepository.GetAll<FileDao>(FileDao.MappingNames.Entityid, EntityID);

                    foreach (var val in flDaoList)
                    {
                        BrandSystems.Marcom.Core.Common.File fldao = new BrandSystems.Marcom.Core.Common.File();
                        fldao.Checksum = val.Checksum;
                        fldao.CreatedOn = val.CreatedOn;
                        fldao.Entityid = val.Entityid;
                        fldao.Extension = val.Extension;
                        fldao.MimeType = val.MimeType;
                        fldao.Moduleid = val.Moduleid;
                        fldao.Name = val.Name;
                        fldao.Ownerid = val.Ownerid;
                        fldao.Size = val.Size;
                        fldao.VersionNo = val.VersionNo;
                        fldao.Fileguid = val.Fileguid;
                        fldao.Id = val.Id;
                        flDetails.Add(fldao);
                    }

                    tx.Commit();

                    return flDetails;

                }
            }


            catch
            {

            }

            return null;
        }



        /// <summary>
        /// Gets the user default subscription by user id.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="SubScriptionTypeId">The sub scription user id.</param>
        /// <returns>IList</returns>
        public Tuple<IList<ISubscriptionType>, string[], string[]> GetUserDefaultSubscriptionByUserID(CommonManagerProxy proxy)
        {

            try
            {
                IList<ISubscriptionType> Systemsubscriptiontypes = new List<ISubscriptionType>();
                string[] userSubscripitonTypes = new string[500];
                string[] userMailSubscripitonTypes = new string[500];
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    var linqsub = (from t in tx.PersistenceManager.PlanningRepository.Query<UserDefaultSubscriptionDao>() where t.UserID == proxy.MarcomManager.User.Id select t).ToList<UserDefaultSubscriptionDao>();
                    var str = string.Join(",", linqsub.Select(p => p.SubscriptionTypeID));
                    userSubscripitonTypes = (str.Split(','));

                    var mailtypes = string.Join(",", linqsub.Select(p => p.MailSubscriptionTypeID));
                    userMailSubscripitonTypes = (mailtypes.Split(','));

                    var subscritiotypedao = (from subscriptiontypes in tx.PersistenceManager.PlanningRepository.Query<SubscriptionTypeDao>() select subscriptiontypes).ToList<SubscriptionTypeDao>();

                    foreach (var subtype in subscritiotypedao)
                    {
                        ISubscriptionType sub = new SubscriptionType();
                        sub.Caption = subtype.Caption;
                        sub.Id = subtype.Id;
                        sub.isAppDefault = subtype.isAppDefault;
                        sub.isAppMandatory = subtype.isAppMandatory;
                        sub.isMailDefault = subtype.isMailDefault;
                        sub.isMailMandatory = subtype.isMailMandatory;
                        Systemsubscriptiontypes.Add(sub);
                    }

                    var notificationsubscriptions = Tuple.Create(Systemsubscriptiontypes, userSubscripitonTypes, userMailSubscripitonTypes);
                    tx.Commit();
                    return notificationsubscriptions;
                }

            }
            catch (Exception ex)
            {
                return null;
            }

        }

        /// <summary>
        /// Update subscription for user id.
        /// </summary>
        /// <param name="proxy"> subscription types</param>
        /// <returns>bool</returns>
        public bool SaveSelectedDefaultSubscription(CommonManagerProxy proxy, string subscriptionTypeIds, string mailSubscritpitonTypeIds)
        {
            try
            {
                UserDefaultSubscriptionDao dao = new UserDefaultSubscriptionDao();

                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    var userSubscriptionDetails = (from item in tx.PersistenceManager.CommonRepository.Query<UserDefaultSubscriptionDao>() where item.UserID == proxy.MarcomManager.User.Id select item).FirstOrDefault();
                    if (userSubscriptionDetails != null)
                    {
                        dao.Id = userSubscriptionDetails.Id;
                    }
                    dao.UserID = proxy.MarcomManager.User.Id;
                    if (subscriptionTypeIds != "")
                    {
                        dao.SubscriptionTypeID = subscriptionTypeIds;
                        dao.MailSubscriptionTypeID = (userSubscriptionDetails != null ? userSubscriptionDetails.MailSubscriptionTypeID : "");

                    }
                    if (mailSubscritpitonTypeIds != "")
                    {
                        dao.SubscriptionTypeID = (userSubscriptionDetails != null ? userSubscriptionDetails.SubscriptionTypeID : "");
                        dao.MailSubscriptionTypeID = mailSubscritpitonTypeIds;
                    }
                    tx.PersistenceManager.CommonRepository.Save<UserDefaultSubscriptionDao>(dao);
                    tx.Commit();
                }
                return true;

            }
            catch
            {

            }

            return false;
        }


        /// <summary>
        /// Update subscription by email.
        /// </summary>
        /// <param name="proxy"> </param>
        /// <returns>bool</returns>
        public bool SaveNotificationByMail(CommonManagerProxy proxy, string ColumnName, string ColumnValue)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    UserMailSubscriptionDao usermailsub = new UserMailSubscriptionDao();

                    usermailsub = (from item in tx.PersistenceManager.MetadataRepository.Query<UserMailSubscriptionDao>() where item.Userid == proxy.MarcomManager.User.Id select item).ToList<UserMailSubscriptionDao>().FirstOrDefault();

                    if (usermailsub == null)
                    {
                        UserMailSubscriptionDao usermailsub1 = new UserMailSubscriptionDao();
                        usermailsub1.Userid = proxy.MarcomManager.User.Id;
                        usermailsub1.LastUpdatedOn = System.DateTime.UtcNow;
                        usermailsub1.IsEmailEnable = true;
                        usermailsub1.DayName = "";
                        usermailsub1.Timing = TimeSpan.Parse("00:00");
                        usermailsub1.RecapReport = false;

                        tx.PersistenceManager.CommonRepository.Save<UserMailSubscriptionDao>(usermailsub1);
                        tx.Commit();
                        return true;
                    }
                    else
                    {
                        switch (ColumnName)
                        {
                            case "IsMailEnable":
                                if (Convert.ToInt16(ColumnValue) == 1)
                                {
                                    usermailsub.IsEmailEnable = true;
                                }
                                else
                                {
                                    usermailsub.IsEmailEnable = false;
                                }
                                break;
                            case "RecapReport":
                                if (Convert.ToInt16(ColumnValue) == 1)
                                {
                                    usermailsub.RecapReport = true;
                                }
                                else
                                {
                                    usermailsub.RecapReport = false;
                                }
                                break;
                            case "DayName":
                                usermailsub.DayName = ColumnValue;
                                break;
                            case "Timing":
                                usermailsub.Timing = TimeSpan.Parse(ColumnValue);
                                break;
                        }
                    }
                    tx.PersistenceManager.CommonRepository.Save<UserMailSubscriptionDao>(usermailsub);
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
        /// Update Task Notification.
        /// </summary>
        /// <param name="proxy"> </param>
        /// <returns>bool</returns>
        public bool SaveTaskNotificationByMail(CommonManagerProxy proxy, string ColumnName, string ColumnValue)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    UserTaskNotificationMailSettingsDao usermailsub = new UserTaskNotificationMailSettingsDao();

                    usermailsub = (from item in tx.PersistenceManager.MetadataRepository.Query<UserTaskNotificationMailSettingsDao>() where item.Userid == proxy.MarcomManager.User.Id select item).ToList<UserTaskNotificationMailSettingsDao>().FirstOrDefault();

                    if (usermailsub == null)
                    {
                        UserTaskNotificationMailSettingsDao usermailsub1 = new UserTaskNotificationMailSettingsDao();
                        usermailsub1.Userid = proxy.MarcomManager.User.Id;
                        usermailsub1.LastUpdatedOn = System.DateTime.UtcNow;
                        usermailsub1.IsNotificationEnable = false;
                        usermailsub1.NoOfDays = 1;
                        usermailsub1.NotificationTiming = TimeSpan.Parse("00:00");
                        usermailsub1.IsEmailEnable = false;
                        usermailsub1.MailTiming = TimeSpan.Parse("00:00");

                        tx.PersistenceManager.CommonRepository.Save<UserTaskNotificationMailSettingsDao>(usermailsub1);
                        tx.Commit();
                        return true;
                    }
                    else
                    {
                        switch (ColumnName)
                        {
                            case "IsNotificationEnable":
                                if (Convert.ToInt16(ColumnValue) == 1)
                                {
                                    usermailsub.IsNotificationEnable = true;
                                    usermailsub.IsEmailEnable = true;
                                }
                                else
                                {
                                    usermailsub.IsNotificationEnable = false;
                                    usermailsub.IsEmailEnable = false;
                                }
                                break;

                            case "NotificationTiming":

                                usermailsub.NotificationTiming = TimeSpan.Parse(ColumnValue);
                                break;

                            case "NoOfDays":

                                usermailsub.NoOfDays = Convert.ToInt16(ColumnValue);
                                break;

                            case "IsEmailEnable":
                                if (Convert.ToInt16(ColumnValue) == 1)
                                {
                                    usermailsub.IsEmailEnable = true;
                                }
                                else
                                {
                                    usermailsub.IsEmailEnable = false;
                                }
                                break;

                            case "MailTiming":

                                usermailsub.MailTiming = TimeSpan.Parse(ColumnValue);
                                break;
                            case "IsEmailAssignedEnable":
                                if (Convert.ToInt16(ColumnValue) == 1)
                                {
                                    usermailsub.IsEmailAssignedEnable = true;
                                }
                                else
                                {
                                    usermailsub.IsEmailAssignedEnable = false;
                                }
                                break;
                        }
                    }
                    tx.PersistenceManager.CommonRepository.Save<UserTaskNotificationMailSettingsDao>(usermailsub);
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
        /// Insert widget.
        /// </summary>
        /// <returns>Iwidget Object.</returns>
        public string InsertUpdateWidget(CommonManagerProxy proxy, string templateid, string widgetid, string caption, string description, int widgettypeid, int filterid, int attributeid, bool isdynamic, string widgetQuery, int dimensionid, string matrixid, int columnval, int rowval, int sizeXval, int sizeYval, bool IsAdminPage, int visualtypeid, int NoOfItem, string listofentityid, string listofSelectEntityID)
        {
            try
            {

                if (listofentityid.IndexOf("\n") != -1)
                {
                    string strlistofentityid = listofentityid.Replace("\n", "");
                    listofentityid = strlistofentityid;
                }

                string dashboardwidgetpath = Path.Combine(HttpRuntime.AppDomainAppPath, ConfigurationManager.AppSettings["dashboardwidget"]);

                Marcom.Dal.User.Model.UserDao uDao = new Marcom.Dal.User.Model.UserDao();
                WidgetDao obj;


                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {

                    int ischanges;
                    ischanges = (from item in tx.PersistenceManager.CommonRepository.Query<Marcom.Dal.User.Model.UserDao>() where item.Id == proxy.MarcomManager.User.Id select item.IsDashboardCustomized).FirstOrDefault();


                    if (IsAdminPage == true)
                    {
                        dashboardwidgetpath = dashboardwidgetpath + "\\DashboardTemplate_" + templateid + ".xml";
                    }
                    else
                    {
                        dashboardwidgetpath = dashboardwidgetpath + "\\UserDashboardTemplate_" + proxy.MarcomManager.User.Id + ".xml";
                    }

                    if (widgetid == "" || widgetid == null)
                    {
                        Guid newwidgetid = Guid.NewGuid();
                        widgetid = newwidgetid.ToString();

                        columnval = 1;
                        rowval = 1;
                        sizeXval = 2;
                        sizeYval = 1;
                    }

                    if (ischanges == 0 && IsAdminPage == false && templateid == null)
                    {
                        XmlDocument mappingdoc1 = new XmlDocument();
                        XmlNode docNode1 = mappingdoc1.CreateXmlDeclaration("1.0", "UTF-8", null);
                        mappingdoc1.AppendChild(docNode1);
                        XmlElement root = mappingdoc1.CreateElement("root");
                        mappingdoc1.AppendChild(root);
                        XmlComment comment = mappingdoc1.CreateComment("widget below...");
                        root.AppendChild(comment);
                        //XmlNode rootnode = mappingdoc.AppendChild(mappingdoc.CreateElement("Dashboard-Template"));  
                        mappingdoc1.Save(dashboardwidgetpath);
                        uDao = (from item in tx.PersistenceManager.CommonRepository.Query<Marcom.Dal.User.Model.UserDao>() where item.Id == proxy.MarcomManager.User.Id select item).FirstOrDefault();
                        uDao.IsDashboardCustomized = 1;
                        tx.PersistenceManager.PlanningRepository.Save<Marcom.Dal.User.Model.UserDao>(uDao);
                        tx.Commit();

                    }

                    else if (ischanges == 0 && IsAdminPage == false)
                    {
                        string sourcefile = "";
                        string Destinationpath = "";

                        string XMLfilepath = Path.Combine(HttpRuntime.AppDomainAppPath, ConfigurationManager.AppSettings["dashboardwidget"]);
                        sourcefile = XMLfilepath + sourcefile + "\\DashboardTemplate_" + templateid + ".xml";
                        Destinationpath = XMLfilepath + Destinationpath + "\\UserDashboardTemplate_" + proxy.MarcomManager.User.Id + ".xml";

                        System.IO.File.Copy(sourcefile, Destinationpath);
                        uDao = (from item in tx.PersistenceManager.CommonRepository.Query<Marcom.Dal.User.Model.UserDao>() where item.Id == proxy.MarcomManager.User.Id select item).FirstOrDefault();
                        uDao.IsDashboardCustomized = 1;
                        tx.PersistenceManager.PlanningRepository.Save<Marcom.Dal.User.Model.UserDao>(uDao);
                        tx.Commit();
                    }
                    obj = new WidgetDao(widgetid.ToString(), caption, description, widgettypeid, filterid, attributeid, isdynamic, dimensionid, matrixid, widgetQuery, columnval, rowval, sizeXval, sizeYval, visualtypeid, NoOfItem, listofentityid, listofSelectEntityID);

                    if (widgettypeid == 3)
                    {

                        string strCollectionEnityids = "";
                        string strParentIDs = "";
                        string strChildIDs = "";
                        if (widgettypeid == 3 && dimensionid == 2)
                        {



                            string[] entityids = listofentityid.Split(',');
                            List<string> list = entityids.ToList();
                            List<int> ParentList = new List<int>();
                            List<int> ChildIDsList = new List<int>();
                            StringBuilder squery = new StringBuilder();
                            squery.Append("select  ID,Level,UniqueKey from  pm_entity where id in(" + listofentityid + ")  order by level,id ");
                            //tx.PersistenceManager.CommonRepository.CreateQuery(
                            List<Hashtable> entityidListResult = tx.PersistenceManager.CommonRepository.ReportExecuteQuery(squery.ToString());
                            IList<Entity> neen = new List<Entity>();
                            IList<Entity> enidnew = new List<Entity>();

                            IList<Entity> childidnew = new List<Entity>();
                            ///neen.RemoveAt()
                            //IList<Entity> neen =tx.PersistenceManager.CommonRepository.CreateQuery("select  ID,Level,UniqueKey from  pm_entity where id in(" + listofentityid + ")  order by level,id "
                            ///int entityidListMaxCount = entityidListResult.Count;
                            ///
                            for (int i = 0; i < entityidListResult.Count; i++)
                            {
                                Entity enidnew1 = new Entity();
                                enidnew1.Id = Convert.ToInt32(entityidListResult[i]["ID"]);
                                enidnew1.Level = Convert.ToInt32(entityidListResult[i]["Level"]);
                                enidnew1.UniqueKey = entityidListResult[i]["UniqueKey"].ToString();

                                enidnew.Add(enidnew1);
                            }
                            childidnew = enidnew.Where(a => a.Level != 0).ToList();

                            IList<Entity> parentId = new List<Entity>();

                            IList<Entity> chilId = new List<Entity>();


                            foreach (var val in enidnew)
                            {
                                if (val.Level == 0)
                                {
                                    Entity enidnew1 = new Entity();
                                    enidnew1.Id = val.Id;
                                    enidnew1.Level = val.Level;
                                    enidnew1.UniqueKey = val.UniqueKey;
                                    parentId.Add(enidnew1);

                                }
                            }


                            foreach (var val in childidnew)
                            {
                                var uniKey = val.UniqueKey.Split('.')[0];
                                var childuniKey = val.UniqueKey;


                                string qry = "select id from PM_entity where uniquekey like '" + uniKey + ".%'  or uniquekey='" + uniKey + "'  order by id asc";
                                IList allCollection = tx.PersistenceManager.CommonRepository.ExecuteQuery(qry);
                                string qry1 = "select id from PM_entity where uniquekey like '" + childuniKey + ".%'  or uniquekey='" + childuniKey + "'  order by id asc";
                                IList childCollection = tx.PersistenceManager.CommonRepository.ExecuteQuery(qry1);
                                if (allCollection != null)
                                {
                                    int[] idArr = allCollection.Cast<Hashtable>().Select(a => (int)a["id"]).ToArray();
                                    int[] childidArr = childCollection.Cast<Hashtable>().Select(a => (int)a["id"]).ToArray();

                                    Entity child = chilId.Where(a => childCollection.Contains(a.Id)).FirstOrDefault();
                                    Entity prnt = parentId.Where(a => idArr.Contains(a.Id)).FirstOrDefault();
                                    if (child == null && prnt == null)
                                    {
                                        Entity enidnew1 = new Entity();
                                        enidnew1.Id = val.Id;
                                        enidnew1.Level = val.Level;
                                        enidnew1.UniqueKey = val.UniqueKey;

                                        chilId.Add(enidnew1);
                                    }

                                }
                                else
                                {
                                    Entity enidnew1 = new Entity();
                                    enidnew1.Id = val.Id;
                                    enidnew1.Level = val.Level;
                                    enidnew1.UniqueKey = val.UniqueKey;
                                    chilId.Add(enidnew1);

                                }
                            }
                            List<int> IDs = new List<int>();
                            List<int> prntIDs = new List<int>();
                            List<int> childIDs = new List<int>();
                            prntIDs = parentId.Select(a => (int)a.Id).ToList();
                            childIDs = chilId.Select(a => (int)a.Id).ToList();
                            IDs.AddRange(prntIDs);
                            IDs.AddRange(childIDs);
                            var CollectionEnityid = string.Join(", ", IDs);
                            var ParentID = string.Join(", ", prntIDs);
                            var ChildID = string.Join(", ", childIDs);
                            strCollectionEnityids = CollectionEnityid.ToString();
                            strParentIDs = ParentID.ToString();
                            strChildIDs = ChildID.ToString();
                            obj.ListofSelectEntityID = strCollectionEnityids;
                            string dynQry = GenerateDynamicQuery(dimensionid, listofentityid, strCollectionEnityids, strParentIDs, strChildIDs);
                            obj.WidgetQuery = dynQry.Replace("<", "&lt;").Replace(">", "&gt;");
                        }
                        else if (widgettypeid == 3 && dimensionid != 2)
                        {
                            obj.ListofSelectEntityID = "";
                            obj.WidgetQuery = GenerateDynamicQuery(dimensionid, listofentityid);
                        }

                    }
                    else
                    {
                        obj.WidgetQuery = "";
                        obj.ListofSelectEntityID = "";
                    }

                }

                XElement SavedXML = PersistenceManager.Instance.MetadataRepository.SaveObject<WidgetDao>(dashboardwidgetpath, obj);
                return widgetid.ToString();
            }
            catch (Exception ex)
            {
                return "";
            }

        }

        public string GenerateDynamicQuery(int widgetdimensionid, string listofentityid, string strCollectionEnityids = "", string strrootids = "", string strChildIDs = "")
        {
            StringBuilder strQuery = new StringBuilder();

            try
            {
                if (widgetdimensionid == 1)
                {

                    strQuery.AppendLine(" SELECT (		 	 	  ");
                    strQuery.AppendLine("	 SELECT SUM(isnull( pefav.ApprovedAllocatedAmount,0))	 	 	  ");
                    strQuery.AppendLine("	 FROM   PM_Financial pefav	 	 	  ");
                    strQuery.AppendLine("	 WHERE  pefav.EntityID IN (" + listofentityid + ")	 	 	  ");
                    strQuery.AppendLine("	  )                                 AS TotalAssigned,	 	 	  ");
                    strQuery.AppendLine("	 Isnull(SUM(fin.PlannedAmount),0)            AS Planned,	 	 	  ");
                    strQuery.AppendLine("	 Isnull(SUM(fin.RequestedAmount),0)          AS Requested,	 	 	  ");
                    strQuery.AppendLine("	 Isnull(SUM(fin.ApprovedAllocatedAmount),0)  AS Approved,	 	 	  ");
                    strQuery.AppendLine("	 Isnull(SUM(fin.ApprovedBudget),0)           AS ApprovedBudget,	 	 	  ");
                    strQuery.AppendLine("	 Isnull(SUM(fin.BudgetDeviation),0)          AS BudgetDeviation,	 	 	  ");
                    strQuery.AppendLine("	 (	 	 	  ");
                    strQuery.AppendLine("		 SELECT Isnull(SUM(pefav.Commited),0) 	 	  ");
                    strQuery.AppendLine("		 FROM   PM_Financial pefav	 	  ");
                    strQuery.AppendLine("		 INNER JOIN PM_Entity pe	 	  ");
                    strQuery.AppendLine("		 	ON  pe.ID = pefav.EntityID 	  ");
                    strQuery.AppendLine("		  AND pe.[Active] = 1 	  ");
                    strQuery.AppendLine("		 AND pefav.CostCenterID IN (" + listofentityid + ")	 	  ");
                    strQuery.AppendLine("		 AND pe.TypeId != 7	 	  ");
                    strQuery.AppendLine("	  )                                 AS Commited,	 	  ");
                    strQuery.AppendLine("	  (	 	 	  ");
                    strQuery.AppendLine("		SELECT Isnull(SUM(pefav.Spent),0) 	 	  ");
                    strQuery.AppendLine("		 FROM  PM_Financial pefav	 	  ");
                    strQuery.AppendLine("		 INNER JOIN PM_Entity pe 	  ");
                    strQuery.AppendLine("		 ON  pe.ID = pefav.EntityID	 	  ");
                    strQuery.AppendLine("		 AND pe.[Active] = 1	 	  ");
                    strQuery.AppendLine("		AND pefav.CostCenterID IN ( " + listofentityid + ") 	 	  ");
                    strQuery.AppendLine("		AND pe.TypeId !=7 	 	  ");
                    strQuery.AppendLine("	   )                                 AS Spent	 	 	  ");
                    strQuery.AppendLine("	FROM   (	 	 	  ");
                    strQuery.AppendLine("	SELECT pefav.PlannedAmount    AS PlannedAmount,	 	 	  ");
                    strQuery.AppendLine("	 pefav.RequestedAmount  AS RequestedAmount,	 	 	  ");
                    strQuery.AppendLine("	 pefav.ApprovedAllocatedAmount AS ApprovedAllocatedAmount,	 	 	  ");
                    strQuery.AppendLine("	 pefav.ApprovedBudget,	 	 	  ");
                    strQuery.AppendLine("	 CASE	 	 	  ");
                    strQuery.AppendLine("	  WHEN pefav.ApprovedBudgetDate IS NULL THEN 0	 	 	  ");
                    strQuery.AppendLine("	  ELSE pefav.ApprovedBudget - pefav.ApprovedAllocatedAmount	 	 	  ");
                    strQuery.AppendLine("	 END                    AS BudgetDeviation	 	 	  ");
                    strQuery.AppendLine("	 FROM   PM_Financial pefav	 	 	  ");
                    strQuery.AppendLine("	 INNER JOIN PM_Entity pe	 	 	  ");
                    strQuery.AppendLine("	  ON  pe.ID = pefav.EntityID	 	 	  ");
                    strQuery.AppendLine("	  AND pe.[Active] = 1	 	 	  ");
                    strQuery.AppendLine("	  AND pefav.CostCenterID in (" + listofentityid + ")	 	 	  ");
                    strQuery.AppendLine("	 AND pe.TypeId != 7	 	 	  ");
                    strQuery.AppendLine("	 AND pe.Level = 1	 	 	  ");
                    strQuery.AppendLine("	 )                                 AS fin	 	 	  ");


                }
                else if (widgetdimensionid == 2)
                {


                    //--insert all the entityids here
                    strQuery.AppendLine("	DECLARE @IDs NVARCHAR(MAX) ");
                    strQuery.AppendLine("	DECLARE @ExcludedList AS TABLE(ID INT) ");

                    strQuery.AppendLine("	SELECT @IDs = '" + strCollectionEnityids + "'  ");
                    strQuery.AppendLine("	DECLARE @xml XML ");
                    strQuery.AppendLine("	SET @xml = N'<root><r>' + REPLACE(@IDs, ',', '</r><r>') + '</r></root>' ");

                    strQuery.AppendLine("	INSERT INTO @ExcludedList ");
                    strQuery.AppendLine("	SELECT r.value('.', 'int') ");
                    strQuery.AppendLine("	FROM   @xml.nodes('//root/r') AS records(r) ");

                    //--insert only the oarent entity ids here
                    strQuery.AppendLine("	DECLARE @ParentIDs NVARCHAR(MAX)  ");
                    strQuery.AppendLine("	DECLARE @ExcludedParentList AS TABLE(ID INT)  ");

                    strQuery.AppendLine("	SELECT @ParentIDs = '" + strrootids + "'  ");
                    strQuery.AppendLine("	DECLARE @Parentxml XML  ");
                    strQuery.AppendLine("	SET @Parentxml = N'<root><r>' + REPLACE(@ParentIDs, ',', '</r><r>') + '</r></root>'   ");

                    strQuery.AppendLine("	INSERT INTO @ExcludedParentList  ");
                    strQuery.AppendLine("	SELECT r.value('.', 'int')  ");
                    strQuery.AppendLine("	FROM   @Parentxml.nodes('//root/r') AS records(r)  ");


                    //--insert chuldren entity ids here
                    strQuery.AppendLine("	DECLARE @ChildIDs NVARCHAR(MAX)   ");
                    strQuery.AppendLine("	DECLARE @ExcludedChildList AS TABLE(ID INT)   ");

                    strQuery.AppendLine("	SELECT @ChildIDs = '" + strChildIDs + "'   ");
                    strQuery.AppendLine("	DECLARE @Childxml XML    ");
                    strQuery.AppendLine("	SET @Childxml = N'<root><r>' + REPLACE(@ChildIDs, ',', '</r><r>') + '</r></root>'  ");

                    strQuery.AppendLine("	INSERT INTO @ExcludedChildList   ");
                    strQuery.AppendLine("	SELECT r.value('.', 'int')  ");
                    strQuery.AppendLine("	FROM   @Childxml.nodes('//root/r') AS records(r) ");


                    strQuery.AppendLine("  SELECT Isnull(SUM(subapproved),0)  AS 'subapproved',Isnull(SUM(subchildalloc),0)  AS 'subchildalloc',Isnull(SUM(Planned),0)  AS 'Planned',  ");
                    strQuery.AppendLine("      Isnull(SUM(Approved),0)         AS 'Approved',  ");
                    strQuery.AppendLine("       Isnull(SUM(ApprovedBudget),0)   AS 'ApprovedBudget',  ");
                    strQuery.AppendLine("       Isnull(SUM(BudgetDeviation),0)  AS 'BudgetDeviation',  ");
                    strQuery.AppendLine("       Isnull(SUM(Commited),0)         AS 'Commited',  ");
                    strQuery.AppendLine("       Isnull(SUM(Spent),0)            AS 'Spent',  ");
                    strQuery.AppendLine("       Isnull(SUM(SubAllocation),0)    AS 'SubAllocation', ");
                    strQuery.AppendLine("       Isnull(SUM(Available),0)        AS 'Available',  ");
                    strQuery.AppendLine("      Isnull(SUM(inRequest),0)        AS 'inRequest', ");
                    strQuery.AppendLine("      Isnull(SUM((Approved -SubAllocation)- (TotalSpentAmount)+NotSpentAmount),0) AS 'Available'  ");
                    strQuery.AppendLine("		FROM   (    	 	  ");
                    //        --Level 0 entities sum calculation
                    //        --check parent id availability query


                    if (strrootids.Length > 0)
                    {

                        strQuery.AppendLine("          SELECT main.Approved -(main.Commited + main.Spent) AS 'subapproved',0.0  AS 'subchildalloc',main.Planned,   ");
                        strQuery.AppendLine("                 main.Approved,   ");
                        strQuery.AppendLine("                main.ApprovedBudget,   ");
                        strQuery.AppendLine("                 main.BudgetDeviation,   ");
                        strQuery.AppendLine("                main.Commited,  ");
                        strQuery.AppendLine("               main.Spent,  ");
                        strQuery.AppendLine("                main.SubAllocation,   ");
                        strQuery.AppendLine("                main.Approved -(main.Commited + main.Spent) AS Available,  ");
                        strQuery.AppendLine("                0  AS inRequest,   ");
                        strQuery.AppendLine("                0 AS TotalSpentAmount,  ");
                        strQuery.AppendLine("               0 AS NotSpentAmount   ");
                        strQuery.AppendLine("        FROM   (   ");
                        strQuery.AppendLine("		SELECT Isnull(SUM(fin.PlannedAmount),0) AS Planned,    	 	  ");
                        strQuery.AppendLine("		Isnull(SUM(fin.ApprovedAllocatedAmount),0) AS Approved,    	 	  ");
                        strQuery.AppendLine("		Isnull(SUM(fin.ApprovedBudget),0) AS ApprovedBudget,    	 	  ");
                        strQuery.AppendLine("                          Isnull(SUM(fin.BudgetDeviation),0) AS BudgetDeviation,  ");
                        strQuery.AppendLine("                          Isnull(SUM(fin.SubAllocatedAmount),0) AS SubAllocation,   ");
                        strQuery.AppendLine("		(    	 	  ");
                        strQuery.AppendLine("                              SELECT Isnull(SUM(pefav2.Commited),0)   ");
                        strQuery.AppendLine("		FROM   PM_Financial pefav2    	 	  ");
                        strQuery.AppendLine("		INNER JOIN PM_Entity pe2    	 	  ");
                        strQuery.AppendLine("		ON  pefav2.EntityID = pe2.ID    	 	  ");
                        strQuery.AppendLine("		AND pe2.[Active] = 1   	 	  ");
                        strQuery.AppendLine("                                          AND pe2.TypeId != 7   ");
                        strQuery.AppendLine("		INNER JOIN PM_Entity pe    	 	  ");
                        strQuery.AppendLine("                                        ON  pe.ID IN  (SELECT *   ");
                        strQuery.AppendLine("                                             FROM   @ExcludedParentList)   ");
                        strQuery.AppendLine("                                         AND pe2.UniqueKey LIKE pe.UniqueKey   ");
                        strQuery.AppendLine("                                               + '.%'   ");
                        strQuery.AppendLine("                           )  AS Commited,   ");
                        strQuery.AppendLine("                          (   ");
                        strQuery.AppendLine("                              SELECT Isnull(SUM(pefav2.Spent),0)   ");
                        strQuery.AppendLine("                             FROM   PM_Financial pefav2  ");
                        strQuery.AppendLine("                                   INNER JOIN PM_Entity pe2 ");
                        strQuery.AppendLine("                                          ON  pefav2.EntityID = pe2.ID ");
                        strQuery.AppendLine("                                         AND pe2.[Active] = 1  ");
                        strQuery.AppendLine("                                          AND pe2.TypeId != 7   ");
                        strQuery.AppendLine("                                     INNER JOIN PM_Entity pe  ");
                        strQuery.AppendLine("                                          ON  pe.ID IN  (SELECT * ");
                        strQuery.AppendLine("                                               FROM   @ExcludedParentList) ");
                        strQuery.AppendLine("                                          AND pe2.UniqueKey LIKE pe.UniqueKey  ");
                        strQuery.AppendLine("                                              + '.%' ");
                        strQuery.AppendLine("                          )  AS Spent ");
                        strQuery.AppendLine("                   FROM   ( ");
                        strQuery.AppendLine("                              SELECT pefav.PlannedAmount AS PlannedAmount, ");
                        strQuery.AppendLine("		pefav.ApprovedAllocatedAmount AS     	 	  ");
                        strQuery.AppendLine("		ApprovedAllocatedAmount,    	 	  ");
                        strQuery.AppendLine("		pefav.ApprovedBudget AS ApprovedBudget,    	 	  ");
                        strQuery.AppendLine("                                     CASE  ");
                        strQuery.AppendLine("                                          WHEN pefav.ApprovedBudgetDate IS  ");
                        strQuery.AppendLine("                                               NULL THEN 0  ");
                        strQuery.AppendLine("                                          ELSE pefav.ApprovedBudget - pefav.ApprovedAllocatedAmount ");
                        strQuery.AppendLine("                                     END AS BudgetDeviation, ");
                        strQuery.AppendLine("                                     ISNULL( ");
                        strQuery.AppendLine("                                         (  ");
                        strQuery.AppendLine("                                             SELECT Isnull(SUM(pefav2.ApprovedAllocatedAmount),0) ");
                        strQuery.AppendLine("                                             FROM   PM_Financial pefav2  ");
                        strQuery.AppendLine("                                                   INNER JOIN PM_Entity pe ");
                        strQuery.AppendLine("                                                         ON  pefav2.EntityID =  ");
                        strQuery.AppendLine("                                                             pe.ID  ");
                        strQuery.AppendLine("                                                        AND pe.parentID IN  (SELECT *  ");
                        strQuery.AppendLine("                                              FROM   @ExcludedParentList)  ");
                        strQuery.AppendLine("                                                        AND pe.[Active] = 1  ");
                        strQuery.AppendLine("                                                         AND pe.TypeId != 7   ");
                        strQuery.AppendLine("                                                        AND pefav2.CostCenterID =  ");
                        strQuery.AppendLine("                                                             pefav.CostCenterID  ");
                        strQuery.AppendLine("                                        ),  ");
                        strQuery.AppendLine("                                        0  ");
                        strQuery.AppendLine("                                       ) AS SubAllocatedAmount ");
                        strQuery.AppendLine("       FROM   PM_Financial pefav ");
                        strQuery.AppendLine("       INNER JOIN PM_Entity pe ");
                        strQuery.AppendLine("             ON  pe.ID = pefav.EntityID ");
                        strQuery.AppendLine(" WHERE  pe.ParentID in (SELECT *   ");
                        strQuery.AppendLine("               FROM   @ExcludedParentList)  ");
                        strQuery.AppendLine("       AND pe.[Active] = 1 ");
                        strQuery.AppendLine("  )  AS fin ");
                        strQuery.AppendLine("               )  AS main  ");

                        strQuery.AppendLine("		UNION ALL                    	 	  ");
                    }

                    //--uptp this parent query


                    //--get sum of inrequest amount for all entity

                    strQuery.AppendLine("               SELECT 0.0   AS 'subapproved',0.0  AS 'subchildalloc',0  AS Planned, ");
                    strQuery.AppendLine("                      0                     AS Approved, ");
                    strQuery.AppendLine("                       0                     AS ApprovedBudget, ");
                    strQuery.AppendLine("                       0                     AS BudgetDeviation, ");
                    strQuery.AppendLine("                       0                     AS Commited,  ");
                    strQuery.AppendLine("                       0                     AS Spent, ");
                    strQuery.AppendLine("                       0                     AS SubAllocation, ");
                    strQuery.AppendLine("                       0                     AS Available,  ");
                    strQuery.AppendLine("                      main.RequestedAmount  AS inRequest, ");
                    strQuery.AppendLine("                       0 AS TotalSpentAmount,  ");
                    strQuery.AppendLine("                       0 AS NotSpentAmount ");
                    strQuery.AppendLine("                FROM   ( ");
                    strQuery.AppendLine("                          SELECT Isnull(SUM(RequestedAmount),0) AS RequestedAmount ");
                    strQuery.AppendLine("                           FROM   ( ");
                    strQuery.AppendLine("                                     SELECT SUM(ISNULL(tblchildren.RequestedAmount, 0)) AS  ");
                    strQuery.AppendLine("                                             RequestedAmount  ");
                    strQuery.AppendLine("                                      FROM   (  ");
                    strQuery.AppendLine("                                                 SELECT pefav.CostCenterID, ");
                    strQuery.AppendLine("                                                        pefav.EntityID,  ");
                    strQuery.AppendLine("                                                        pefav.[Status]  ");
                    strQuery.AppendLine("                                                 FROM   PM_Financial pefav ");
                    strQuery.AppendLine("                                                        INNER JOIN PM_Entity pe ");
                    strQuery.AppendLine("                                                             ON  pefav.EntityID = pe.ID ");
                    strQuery.AppendLine("                                                 WHERE  pe.ID IN (SELECT *  ");
                    strQuery.AppendLine("                                                                  FROM   @ExcludedList) ");
                    strQuery.AppendLine("                                                        AND pe.Active = 1  ");
                    strQuery.AppendLine("                                                       AND (LEN(pe.UniqueKey) - LEN(REPLACE(pe.UniqueKey, '.', ''))) =   ");
                    strQuery.AppendLine("                                                           0 ");
                    strQuery.AppendLine("                                            ) AS tbl ");
                    strQuery.AppendLine("                                            LEFT OUTER JOIN ( ");
                    strQuery.AppendLine("                                                      SELECT pefav.CostCenterID, ");
                    strQuery.AppendLine("                                                             Isnull(SUM(pefav.RequestedAmount),0) AS  ");
                    strQuery.AppendLine("                                                             RequestedAmount ");
                    strQuery.AppendLine("                                                      FROM   PM_Financial  ");
                    strQuery.AppendLine("                                                             pefav ");
                    strQuery.AppendLine("                                                             INNER JOIN PM_Entity ");
                    strQuery.AppendLine("                                                                  pe  ");
                    strQuery.AppendLine("                                                                  ON  pefav.EntityID =  ");
                    strQuery.AppendLine("                                                                      pe.ID  ");
                    strQuery.AppendLine("                                                     WHERE  pe.ParentID IN (SELECT *  ");
                    strQuery.AppendLine("                                                                             FROM   @ExcludedList) ");
                    strQuery.AppendLine("                                                             AND pe.Active = 1  ");
                    strQuery.AppendLine("                                                      GROUP BY  ");
                    strQuery.AppendLine("                                                             pefav.CostCenterID  ");
                    strQuery.AppendLine("                                                  ) AS tblchildren  ");
                    strQuery.AppendLine("                                                  ON  tblchildren.CostCenterID = tbl.CostCenterID  ");
                    strQuery.AppendLine("		UNION ALL               	 	  ");
                    strQuery.AppendLine("                                      SELECT SUM(ISNULL(pefav.RequestedAmount, 0)) ");
                    strQuery.AppendLine("                                      FROM   PM_Financial pefav  ");
                    strQuery.AppendLine("                                             INNER JOIN PM_Entity pe  ");
                    strQuery.AppendLine("                                                  ON  pefav.EntityID = pe.ID ");
                    strQuery.AppendLine("                                      WHERE  pe.ID IN (SELECT *  ");
                    strQuery.AppendLine("                                                       FROM   @ExcludedList) ");
                    strQuery.AppendLine("                                             AND pe.Active = 1  ");
                    strQuery.AppendLine("                                             AND (LEN(pe.UniqueKey) - LEN(REPLACE(pe.UniqueKey, '.', '')))  ");
                    strQuery.AppendLine("                                                 != 0 ");
                    strQuery.AppendLine("                                  ) B  ");
                    strQuery.AppendLine("                       )                     AS main ");



                    //   --from Level 1 onwards entities sum calculation

                    // --check condition for child present or not


                    if (strChildIDs.Length > 0)
                    {

                        strQuery.AppendLine("		UNION ALL    	 	  ");

                        strQuery.AppendLine("                SELECT main.Approved -(main.Commited + main.Spent) AS 'subapproved',main.SubAllocation   AS 'subchildalloc',main.Planned, ");
                        strQuery.AppendLine("                       main.Approved, ");
                        strQuery.AppendLine("                       main.ApprovedBudget, ");
                        strQuery.AppendLine("                       main.BudgetDeviation, ");
                        strQuery.AppendLine("                       main.Commited, ");
                        strQuery.AppendLine("                       main.Spent, ");
                        strQuery.AppendLine("                       main.SubAllocation, ");
                        strQuery.AppendLine("                       (main.Approved - main.Spent) AS Available, ");
                        strQuery.AppendLine("                       0  AS inRequest, ");
                        strQuery.AppendLine("                       0 AS TotalSpentAmount, ");
                        strQuery.AppendLine("                       0 AS NotSpentAmount ");
                        strQuery.AppendLine("                FROM   (  ");
                        strQuery.AppendLine("                           SELECT Isnull(SUM(fin.PlannedAmount),0) AS Planned, ");
                        strQuery.AppendLine("                                  Isnull(SUM(fin.ApprovedAllocatedAmount),0) AS Approved, ");
                        strQuery.AppendLine("                                  Isnull(SUM(fin.ApprovedBudget),0) AS ApprovedBudget, ");
                        strQuery.AppendLine("                                  Isnull(SUM(fin.BudgetDeviation),0) AS BudgetDeviation, ");
                        strQuery.AppendLine("                                  Isnull(SUM(fin.SubAllocatedAmount),0) AS SubAllocation, ");
                        strQuery.AppendLine("                                  ( ");
                        strQuery.AppendLine("                                       SELECT Isnull(SUM(pefav2.Commited),0) ");
                        strQuery.AppendLine("                                      FROM   PM_Financial pefav2  ");
                        strQuery.AppendLine("                                              INNER JOIN PM_Entity pe2 ");
                        strQuery.AppendLine("                                                  ON  pefav2.EntityID = pe2.ID ");
                        strQuery.AppendLine("                                                  AND pe2.[Active] = 1  ");
                        strQuery.AppendLine("                                                  AND pe2.TypeId != 7 ");
                        strQuery.AppendLine("                                             INNER JOIN PM_Entity pe  ");
                        strQuery.AppendLine("                                                  ON  pe.ID IN  (SELECT *  ");
                        strQuery.AppendLine("                                                       FROM   @ExcludedChildList) ");
                        strQuery.AppendLine("                                                   AND pe2.UniqueKey LIKE pe.UniqueKey ");
                        strQuery.AppendLine("                                                       + '%' ");
                        strQuery.AppendLine("                                  )  AS Commited, ");
                        strQuery.AppendLine("                                  ( ");
                        strQuery.AppendLine("                                      SELECT Isnull(SUM(pefav2.Spent),0) ");
                        strQuery.AppendLine("                                       FROM   PM_Financial pefav2  ");
                        strQuery.AppendLine("                                             INNER JOIN PM_Entity pe2 ");
                        strQuery.AppendLine("                                                  ON  pefav2.EntityID = pe2.ID ");
                        strQuery.AppendLine("                                                   AND pe2.[Active] = 1  ");
                        strQuery.AppendLine("                                                  AND pe2.TypeId != 7 ");
                        strQuery.AppendLine("                                             INNER JOIN PM_Entity pe ");
                        strQuery.AppendLine("                                                  ON  pe.ID IN  (SELECT *   ");
                        strQuery.AppendLine("                                                       FROM   @ExcludedChildList)  ");
                        strQuery.AppendLine("                                                  AND pe2.UniqueKey LIKE pe.UniqueKey  ");
                        strQuery.AppendLine("                                                      + '%'  ");
                        strQuery.AppendLine("                                  )  AS Spent ");
                        strQuery.AppendLine("                            FROM   ( ");
                        strQuery.AppendLine("                                      SELECT pefav.PlannedAmount AS PlannedAmount, ");
                        strQuery.AppendLine("                                             pefav.ApprovedAllocatedAmount AS  ");
                        strQuery.AppendLine("                                             ApprovedAllocatedAmount, ");
                        strQuery.AppendLine("                                             pefav.ApprovedBudget AS ApprovedBudget, ");
                        strQuery.AppendLine("                                              CASE  ");
                        strQuery.AppendLine("                                                   WHEN pefav.ApprovedBudgetDate IS  ");
                        strQuery.AppendLine("                                                       NULL THEN 0 ");
                        strQuery.AppendLine("                                                   ELSE pefav.ApprovedBudget - pefav.ApprovedAllocatedAmount ");
                        strQuery.AppendLine("                                              END AS BudgetDeviation, ");
                        strQuery.AppendLine("                                             ISNULL( ");
                        strQuery.AppendLine("                                                 (  ");
                        strQuery.AppendLine("                                                     SELECT Isnull(SUM(pefav2.ApprovedAllocatedAmount),0) ");
                        strQuery.AppendLine("                                                     FROM   PM_Financial pefav2  ");
                        strQuery.AppendLine("                                                            INNER JOIN PM_Entity pe  ");
                        strQuery.AppendLine("                                                                 ON  pefav2.EntityID = ");
                        strQuery.AppendLine("                                                                     pe.ID ");
                        strQuery.AppendLine("                                                                 AND pe.parentID IN  (SELECT *  ");
                        strQuery.AppendLine("                                                       FROM   @ExcludedChildList) ");
                        strQuery.AppendLine("                                                                 AND pe.[Active] = 1 ");
                        strQuery.AppendLine("                                                                 AND pe.TypeId != 7  ");
                        strQuery.AppendLine("                                                                 AND pefav2.CostCenterID =  ");
                        strQuery.AppendLine("                                                                     pefav.CostCenterID  ");
                        strQuery.AppendLine("                                                 ), ");
                        strQuery.AppendLine("                                                 0  ");
                        strQuery.AppendLine("                                             ) AS SubAllocatedAmount ");
                        strQuery.AppendLine("                                      FROM   PM_Financial pefav  ");
                        strQuery.AppendLine("                                      WHERE  pefav.EntityID IN  (SELECT *  ");
                        strQuery.AppendLine("                                                       FROM   @ExcludedChildList) ");
                        strQuery.AppendLine("                                  )  AS fin ");
                        strQuery.AppendLine("                       )  AS main  ");
                    }

                    //--upto this sub child query
                    strQuery.AppendLine("             )                        A ");


                }
                else if (widgetdimensionid == 3)
                {
                    strQuery.AppendLine("	SELECT browser AS NAME, COUNT(*) AS VALUE FROM UM_LoginDetail GROUP BY browser ORDER BY browser ");
                }
                else if (widgetdimensionid == 4)
                {
                    strQuery.AppendLine("	  SELECT browser AS NAME,Version as VERSION , COUNT(*) AS VALUE FROM UM_LoginDetail GROUP BY browser,Version ORDER BY browser,Version ");
                }
                else if (widgetdimensionid == 5)
                {
                    strQuery.AppendLine("	SELECT OS AS NAME, COUNT(*) AS VALUE FROM UM_LoginDetail GROUP BY OS ORDER BY OS ");
                }
                else if (widgetdimensionid == 6)
                {
                    strQuery.AppendLine("	SELECT CountryName AS NAME, COUNT(*) AS VALUE FROM UM_LoginDetail GROUP BY CountryName having len(CountryName)>0  ORDER BY CountryName ");
                }

            }
            catch (Exception ex)
            {
                return "";
            }
            return strQuery.ToString();
        }

        /// <summary>
        /// Gets the widget details.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <returns>List of IWidget</returns>
        public IList<IWidget> GetWidgetDetails(CommonManagerProxy proxy, string widgetid, string templateid, bool IsAdminPage)
        {
            try
            {
                string dashboardwidgetpath = Path.Combine(HttpRuntime.AppDomainAppPath, ConfigurationManager.AppSettings["dashboardwidget"]);
                if (IsAdminPage == true)
                {
                    dashboardwidgetpath = dashboardwidgetpath + "\\DashboardTemplate_" + templateid + ".xml";
                }
                else
                {
                    int ischanges;
                    using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                    {
                        ischanges = (from item in tx.PersistenceManager.CommonRepository.Query<Marcom.Dal.User.Model.UserDao>() where item.Id == proxy.MarcomManager.User.Id select item.IsDashboardCustomized).FirstOrDefault();
                    }
                    if (ischanges == 1)
                    {
                        dashboardwidgetpath = dashboardwidgetpath + "\\UserDashboardTemplate_" + proxy.MarcomManager.User.Id + ".xml";
                    }
                    else
                    {
                        dashboardwidgetpath = dashboardwidgetpath + "\\DashboardTemplate_" + templateid + ".xml";
                    }
                }

                IList<IWidget> _iiwidget = new List<IWidget>();

                XDocument docx = XDocument.Load(dashboardwidgetpath);
                var result = docx.Root.Elements("Widget_Table").Elements("Widget").Where(mid => Convert.ToString(mid.Element("ID").Value.Trim()) == widgetid).Select(m => m);

                foreach (XElement xEle in result)
                {
                    IWidget _iwidgetinfo = new Widget();
                    _iwidgetinfo.Id = xEle.Element("ID").Value.ToString();
                    _iwidgetinfo.Caption = xEle.Element("Caption").Value.ToString();
                    _iwidgetinfo.Description = xEle.Element("Description").Value.ToString();
                    _iwidgetinfo.WidgetTypeID = Convert.ToInt32(xEle.Element("WidgetTypeID").Value.ToString());
                    _iwidgetinfo.IsDynamic = Convert.ToBoolean(xEle.Element("IsDynamic").Value.ToString());
                    _iwidgetinfo.FilterID = Convert.ToInt32(xEle.Element("FilterID").Value.ToString());
                    _iwidgetinfo.AttributeID = Convert.ToInt32(xEle.Element("AttributeID").Value.ToString());
                    _iwidgetinfo.DimensionID = Convert.ToInt32(xEle.Element("DimensionID").Value.ToString());
                    _iwidgetinfo.MatrixID = Convert.ToString(xEle.Element("MatrixID").Value.ToString());
                    _iwidgetinfo.WidgetQuery = xEle.Element("WidgetQuery").Value.ToString();
                    _iwidgetinfo.VisualType = Convert.ToInt32(xEle.Element("VisualType").Value.ToString());
                    _iwidgetinfo.NoOfItem = Convert.ToInt32(xEle.Element("NoOfItem").Value.ToString());

                    if (xEle.Element("ListOfEntityID") == null)
                    {
                        _iwidgetinfo.ListOfEntityIDs = "";
                    }
                    else
                    {
                        _iwidgetinfo.ListOfEntityIDs = Convert.ToString(xEle.Element("ListOfEntityID").Value.ToString());
                    }

                    if (xEle.Element("ListofSelectEntityID") == null)
                    {
                        _iwidgetinfo.ListofSelectEntityIDs = "";
                    }
                    else
                    {
                        _iwidgetinfo.ListofSelectEntityIDs = Convert.ToString(xEle.Element("ListofSelectEntityID").Value.ToString());
                    }
                    _iiwidget.Add(_iwidgetinfo);
                }

                return _iiwidget;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        /// <summary>
        /// Get Widget List for the user
        /// </summary>
        /// <param name="proxy"></param>
        /// <param name="userid"> </param>
        /// <returns>Ilist<Iwidget> </returns>
        public IList<IWidget> GetWidgetDetailsByUserID(CommonManagerProxy proxy, int userid, bool isAdmin, int GlobalTemplateID)
        {
            try
            {
                IList<IWidget> _iiwidget = new List<IWidget>();
                IList<WidgetDao> dao = new List<WidgetDao>();
                int templateid = 0;
                int[] roles = null;
                string xmlpath;
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {

                    //var templateObject = (from user in tx.PersistenceManager.UserRepository.Query<UserDao>()
                    //                      join test in tx.PersistenceManager.UserRepository.Query<WidgetTemplateDao>() on user.DashboardTemplateID equals test.Id
                    //                      where user.Id == userid
                    //                      select new
                    //                      {
                    //                          templatename = test.TemplateName,
                    //                          templateid = test.Id
                    //                      }).FirstOrDefault();
                    if (isAdmin)
                    {
                        templateid = GlobalTemplateID;
                        xmlpath = tx.PersistenceManager.CommonRepository.GetXmlPath(GlobalTemplateID, false);
                    }
                    else
                    {
                        var templateIDObj = (from user in tx.PersistenceManager.UserRepository.Query<GlobalRoleUserDao>()
                                             join test in tx.PersistenceManager.UserRepository.Query<WidgetTemplateRolesDao>() on user.GlobalRoleId equals test.RoleID
                                             where user.Userid == userid
                                             select new
                                             {
                                                 templateid = test.WidgetTemplateID
                                             }).OrderByDescending(test => test.templateid).FirstOrDefault();
                        int IsDashboardCustomised = Convert.ToInt32((from val in tx.PersistenceManager.UserRepository.Query<UserDao>() where val.Id == userid select val.IsDashboardCustomized).FirstOrDefault());
                        bool IsUserChanged = (IsDashboardCustomised == 1 ? true : false);
                        if (IsDashboardCustomised == 0)
                            templateid = Convert.ToInt32(templateIDObj.templateid);
                        else
                            templateid = userid;
                        xmlpath = tx.PersistenceManager.CommonRepository.GetXmlPath(templateid, IsUserChanged);
                    }

                    dao = tx.PersistenceManager.CommonRepository.GetObject<WidgetDao>(xmlpath);
                    roles = (from val in tx.PersistenceManager.UserRepository.Query<GlobalRoleUserDao>() where val.Userid == userid select val.GlobalRoleId).ToArray();

                    if (dao == null)
                    {
                        IWidget iwidget = new Widget();
                        iwidget.TemplateID = templateid;
                        _iiwidget.Add(iwidget);
                    }
                    else
                    {

                        foreach (var item in dao)
                        {
                            var userAccess = (from val in tx.PersistenceManager.UserRepository.Query<GlobalRoleUserDao>() where val.Userid == userid select val).ToList();
                            var widgetRoleAccess = (from val in tx.PersistenceManager.UserRepository.Query<WidgetTypeRolesDao>() where val.WidgetTypeID == item.WidgetTypeID select val.RoleID).ToList();
                            var userHasAccess = userAccess.Where(a => widgetRoleAccess.Contains(a.GlobalRoleId)).Select(a => a).ToList();
                            // var widgetRoleAccess = (from val in tx.PersistenceManager.UserRepository.Query<WidgetTypeRolesDao>() where val.WidgetTypeID == item.WidgetTypeID select val.RoleID);
                            // var userHasAccess = from access in widgetRoleAccess where roles.Contains(1) select access;
                            //var userHasAccess = from access in widgetRoleAccess select access;
                            //int IsDashboardCustomised = Convert.ToInt32((from val in tx.PersistenceManager.UserRepository.Query<UserDao>() where val.Id == userid select val.IsDashboardCustomized).FirstOrDefault());
                            //bool IsUserChanged = (IsDashboardCustomised == 1 ? true : false);
                            //if (userHasAccess.Count() > 0 || isAdmin || IsUserChanged == false)
                            if (userHasAccess.Count() > 0 || isAdmin)
                            {
                                IWidget iwidget = new Widget();
                                iwidget.Id = item.ID;
                                iwidget.TemplateID = templateid;
                                iwidget.WidgetTypeID = item.WidgetTypeID;
                                iwidget.IsDynamic = item.IsDynamic;
                                iwidget.VisualType = item.VisualType;
                                iwidget.NoOfItem = item.NoOfItem;
                                iwidget.Row = item.Row;
                                iwidget.Column = item.Column;
                                iwidget.Caption = item.Caption;
                                iwidget.FilterID = item.FilterID;
                                iwidget.Description = item.Description;
                                iwidget.AttributeID = item.AttributeID;
                                iwidget.SizeX = item.SizeX;
                                iwidget.SizeY = item.SizeY;
                                iwidget.DimensionID = item.DimensionID;
                                iwidget.MatrixID = item.MatrixID;
                                iwidget.WidgetQuery = item.WidgetQuery;
                                _iiwidget.Add(iwidget);
                            }
                        }
                    }
                }
                return _iiwidget;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Delete widget.
        /// </summary>
        /// <returns>Iwidget Object.</returns>
        public bool DeleteWidget(CommonManagerProxy proxy, string templateid, string widgetid, bool IsAdminPage)
        {
            string dashboardwidgetpath = Path.Combine(HttpRuntime.AppDomainAppPath, ConfigurationManager.AppSettings["dashboardwidget"]);

            if (IsAdminPage == true)
            {
                dashboardwidgetpath = dashboardwidgetpath + "\\DashboardTemplate_" + templateid + ".xml";
            }
            else
            {
                int ischanges;
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {

                    ischanges = (from item in tx.PersistenceManager.CommonRepository.Query<Marcom.Dal.User.Model.UserDao>() where item.Id == proxy.MarcomManager.User.Id select item.IsDashboardCustomized).FirstOrDefault();
                }
                if (ischanges == 1)
                {
                    dashboardwidgetpath = dashboardwidgetpath + "\\UserDashboardTemplate_" + proxy.MarcomManager.User.Id + ".xml";
                }
                else
                {
                    dashboardwidgetpath = dashboardwidgetpath + "\\DashboardTemplate_" + templateid + ".xml";
                }
            }

            try
            {
                WidgetDao obj = new WidgetDao(widgetid);
                if (PersistenceManager.Instance.MetadataRepository.DeleteObject<WidgetDao>(dashboardwidgetpath, obj) == true)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Get Dynamic Widget Content
        /// </summary>
        /// <param name="proxy"></param>
        /// <param name="userid"> </param>
        /// <param name="WidgetId"> </param>
        /// <param name="WidgetTypeID"> </param>
        /// <param name="IsDynamic"> </param>
        /// <returns>List<object> </returns>
        public List<object> GetDynamicwidgetContentUserID(CommonManagerProxy proxy, int userid, int widgetTypeid, string widgetId, int dimensionid)
        {
            try
            {

                string xmlpath = "";
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {

                    var templateIDObj = (from user in tx.PersistenceManager.UserRepository.Query<GlobalRoleUserDao>()
                                         join test in tx.PersistenceManager.UserRepository.Query<WidgetTemplateRolesDao>() on user.GlobalRoleId equals test.RoleID
                                         where user.Userid == userid
                                         select new
                                         {
                                             templateid = test.WidgetTemplateID
                                         }).OrderByDescending(item => item.templateid).FirstOrDefault();

                    int IsDashboardCustomised = Convert.ToInt32((from val in tx.PersistenceManager.UserRepository.Query<UserDao>() where val.Id == userid select val.IsDashboardCustomized).FirstOrDefault());
                    if (IsDashboardCustomised == 0)
                        xmlpath = tx.PersistenceManager.CommonRepository.GetXmlPath(Convert.ToInt32(templateIDObj.templateid), false);
                    else
                    {
                        xmlpath = tx.PersistenceManager.CommonRepository.GetXmlPath(userid, true);
                    }
                    string DynamicQuery = tx.PersistenceManager.CommonRepository.GetDynamicWidgetQuery(widgetId, widgetTypeid, dimensionid, xmlpath);
                    string strListofSelectEntityID = "";

                    List<object> finreslt = new List<object>();
                    int[] nums = { };
                    int spentamount = 0;

                    if (dimensionid == 1)
                    {
                        DynamicQuery = DynamicQuery.Replace("@UserID", proxy.MarcomManager.User.Id.ToString()).Replace("&lt;", "<").Replace("&gt;", ">");
                        var costcentreFinancialSummaryResult = tx.PersistenceManager.PlanningRepository.ExecuteQuery(DynamicQuery.ToString());
                        foreach (var obj in costcentreFinancialSummaryResult)
                        {
                            var inTotalAssigned = ((System.Collections.Hashtable)(obj))["TotalAssigned"];
                            var Planned = ((System.Collections.Hashtable)(obj))["Planned"];
                            var inRequest = ((System.Collections.Hashtable)(obj))["Requested"];
                            var Approved = ((System.Collections.Hashtable)(obj))["Approved"];
                            var ApprovedBudget = ((System.Collections.Hashtable)(obj))["ApprovedBudget"];
                            var BudgetDeviation = ((System.Collections.Hashtable)(obj))["BudgetDeviation"];
                            var Commited = ((System.Collections.Hashtable)(obj))["Commited"];
                            var Spent = ((System.Collections.Hashtable)(obj))["Spent"];

                            finreslt.Add(new
                            {
                                NAME = "Assigned Amount",
                                VALUE = Convert.ToInt64((decimal)inTotalAssigned)
                            });

                            finreslt.Add(new
                            {
                                NAME = "In requests",
                                VALUE = Convert.ToInt64((decimal)inRequest)
                            });
                            finreslt.Add(new
                            {
                                NAME = "Approved budgets",
                                VALUE = Convert.ToInt64((decimal)ApprovedBudget)
                            });
                            finreslt.Add(new
                            {
                                NAME = "Approved allocated amount",
                                VALUE = Convert.ToInt64((decimal)Approved)
                            });
                            finreslt.Add(new
                            {
                                NAME = "Total spent",
                                VALUE = Convert.ToInt64((decimal)Spent)
                            });


                            finreslt.Add(new
                            {
                                NAME = "Available to spent",
                                VALUE = Convert.ToInt64((decimal)Approved) - Convert.ToInt64((decimal)Commited)
                            });
                        }

                    }

                    if (dimensionid == 2)
                    {
                        strListofSelectEntityID = tx.PersistenceManager.CommonRepository.GetListofSelectEntityID(widgetId, widgetTypeid, dimensionid, xmlpath);
                        if (strListofSelectEntityID != null)
                            nums = Array.ConvertAll(strListofSelectEntityID.Split(','), int.Parse);
                        if (nums.Length > 0)
                        {
                            foreach (var id in nums)
                            {
                                spentamount += proxy.MarcomManager.PlanningManager.GetOverviewFinancialAmount(id);
                            }
                        }
                        DynamicQuery = DynamicQuery.Replace("@UserID", proxy.MarcomManager.User.Id.ToString()).Replace("&lt;", "<").Replace("&gt;", ">");
                        var costcentreFinancialSummaryResult = tx.PersistenceManager.PlanningRepository.ExecuteQuery(DynamicQuery.ToString());



                        foreach (var obj in costcentreFinancialSummaryResult)
                        {

                            var inRequest = ((System.Collections.Hashtable)(obj))["inRequest"];
                            var ApprovedBudget = ((System.Collections.Hashtable)(obj))["ApprovedBudget"];
                            var Approved = ((System.Collections.Hashtable)(obj))["Approved"];
                            var Spent = ((System.Collections.Hashtable)(obj))["Spent"];
                            var SubAllocation = ((System.Collections.Hashtable)(obj))["SubAllocation"];
                            var subapproved = ((System.Collections.Hashtable)(obj))["subapproved"];
                            var subchildalloc = ((System.Collections.Hashtable)(obj))["subchildalloc"];

                            finreslt.Add(new
                            {
                                NAME = "In requests",
                                VALUE = Convert.ToInt64((decimal)inRequest)
                            });
                            finreslt.Add(new
                            {
                                NAME = "Approved budgets",
                                VALUE = Convert.ToInt64((decimal)ApprovedBudget)
                            });
                            finreslt.Add(new
                            {
                                NAME = "Approved allocated amount",
                                VALUE = Convert.ToInt64((decimal)Approved)
                            });
                            finreslt.Add(new
                            {
                                NAME = "Total spent",
                                VALUE = Convert.ToInt64((decimal)Spent)
                            });

                            long nonrestAmount = 0;
                            try
                            {
                                nonrestAmount = Convert.ToInt64((decimal)subapproved) -
                                     Convert.ToInt64((decimal)subchildalloc);
                            }
                            catch
                            {

                            }


                            finreslt.Add(new
                            {
                                NAME = "Available to spent",
                                VALUE = Convert.ToInt64(nonrestAmount - spentamount)
                            });
                        }



                    }
                    if (dimensionid > 2)
                    {
                        DynamicQuery = DynamicQuery.Replace("@UserID", proxy.MarcomManager.User.Id.ToString());
                        var ResultDetails = tx.PersistenceManager.PlanningRepository.ExecuteQuery(DynamicQuery.ToString());
                        foreach (var obj in ResultDetails)
                        {
                            if (dimensionid == 4)
                            {
                                finreslt.Add(new
                                {
                                    NAME = ((System.Collections.Hashtable)(obj))["NAME"],
                                    VERSION = ((System.Collections.Hashtable)(obj))["VERSION"],
                                    VALUE = ((System.Collections.Hashtable)(obj))["VALUE"]
                                });
                            }
                            else
                            {
                                finreslt.Add(new
                                {
                                    NAME = ((System.Collections.Hashtable)(obj))["NAME"],
                                    VALUE = ((System.Collections.Hashtable)(obj))["VALUE"]
                                });
                            }



                        }

                    }
                    tx.Commit();
                    return finreslt;

                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Insert widget.
        /// </summary>
        /// <returns>Iwidget Object.</returns>
        public string WidgetDragEdit(CommonManagerProxy proxy, IList<IWidget> widgetdata, bool IsAdminPage)
        {
            try
            {
                string dashboardwidgetpath = Path.Combine(HttpRuntime.AppDomainAppPath, ConfigurationManager.AppSettings["dashboardwidget"]);

                Marcom.Dal.User.Model.UserDao uDao = new Marcom.Dal.User.Model.UserDao();
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    int ischanges;
                    ischanges = (from item in tx.PersistenceManager.CommonRepository.Query<Marcom.Dal.User.Model.UserDao>() where item.Id == proxy.MarcomManager.User.Id select item.IsDashboardCustomized).FirstOrDefault();

                    if (IsAdminPage == true)
                    {
                        dashboardwidgetpath = dashboardwidgetpath + "\\DashboardTemplate_" + widgetdata[0].TemplateID + ".xml";
                    }
                    else
                    {
                        dashboardwidgetpath = dashboardwidgetpath + "\\UserDashboardTemplate_" + proxy.MarcomManager.User.Id + ".xml";
                    }

                    if (ischanges == 0 && IsAdminPage == false)
                    {
                        string sourcefile = "";
                        string Destinationpath = "";

                        string XMLfilepath = Path.Combine(HttpRuntime.AppDomainAppPath, ConfigurationManager.AppSettings["dashboardwidget"]);
                        sourcefile = XMLfilepath + sourcefile + "\\DashboardTemplate_" + widgetdata[0].TemplateID + ".xml";
                        Destinationpath = XMLfilepath + Destinationpath + "\\UserDashboardTemplate_" + proxy.MarcomManager.User.Id + ".xml";

                        System.IO.File.Copy(sourcefile, Destinationpath);
                        uDao = (from item in tx.PersistenceManager.CommonRepository.Query<Marcom.Dal.User.Model.UserDao>() where item.Id == proxy.MarcomManager.User.Id select item).FirstOrDefault();
                        uDao.IsDashboardCustomized = 1;
                        tx.PersistenceManager.PlanningRepository.Save<Marcom.Dal.User.Model.UserDao>(uDao);
                        tx.Commit();
                    }

                    if (widgetdata != null)
                    {
                        IList<WidgetDao> Ientityper = new List<WidgetDao>();

                        WidgetDao perioddao = new WidgetDao();
                        foreach (var a in widgetdata)
                        {
                            perioddao = null;
                            perioddao = new WidgetDao();

                            perioddao.TemplateID = a.TemplateID;
                            perioddao.ID = a.Id;
                            perioddao.WidgetTypeID = -100;
                            perioddao.Column = a.Column;
                            perioddao.Row = a.Row;
                            perioddao.SizeX = a.SizeX;
                            perioddao.SizeY = a.SizeY;

                            XElement SavedXML = PersistenceManager.Instance.MetadataRepository.SaveObject<WidgetDao>(dashboardwidgetpath, perioddao);
                        }
                    }
                }




                //---------------------------------------

                return null;

            }
            catch (Exception ex)
            {
            }
            return null;
        }

        /// <summary>
        /// Get subscription by user id.
        /// </summary>
        /// <param name="proxy"> </param>
        /// <returns>bool</returns>
        //public IUserMailSubscription GetNotificationBYType(CommonManagerProxy proxy, string pCaption)
        //{

        //    try
        //    {

        //        IUserMailSubscription usermailsub = new IUserMailSubscription();

        //        using (ITransaction tx = proxy.MarcomManager.GetTransaction())
        //        {
        //            IUserMailSubscription mailsub = new UserMailSubscriptionDao();

        //            mailsub = tx.PersistenceManager.CommonRepository.GetAll<UserMailSubscriptionDao>();
        //            tx.Commit();


        //            mailsub.IsEmailEnable = usermailsub.IsEmailEnable;
        //            mailsub.DayName = usermailsub.DayName;
        //            mailsub.Timing = usermailsub.Timing;
        //            mailsub.RecapReport = usermailsub.RecapReport;

        //            return mailsub;
        //        }

        //    }
        //    catch (Exception ex)
        //    {

        //    }

        //    return null;
        //}

        /// <summary>
        /// Get subscription by user id.
        /// </summary>
        /// <param name="proxy"> </param>
        /// <returns>bool</returns>
        public IUserMailSubscription GetSubscriptionByUserId(CommonManagerProxy proxy)
        {
            try
            {
                IUserMailSubscription _subdata = new UserMailSubscription();

                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    UserMailSubscriptionDao dao = new UserMailSubscriptionDao();
                    dao = tx.PersistenceManager.CommonRepository.Get<UserMailSubscriptionDao>(UserMailSubscriptionDao.PropertyNames.Userid, proxy.MarcomManager.User.Id);

                    _subdata.IsEmailEnable = dao.IsEmailEnable;
                    _subdata.DayName = dao.DayName;
                    _subdata.Timing = dao.Timing;
                    _subdata.RecapReport = dao.RecapReport;
                }
                return _subdata;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public IUserTaskNotificationMailSettings GetTaskSubscriptionByUserId(CommonManagerProxy proxy)
        {
            try
            {
                IUserTaskNotificationMailSettings _subdata = new UserTaskNotificationMailSettings();

                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    UserTaskNotificationMailSettingsDao dao = new UserTaskNotificationMailSettingsDao();
                    dao = tx.PersistenceManager.CommonRepository.Get<UserTaskNotificationMailSettingsDao>(UserTaskNotificationMailSettingsDao.PropertyNames.Userid, proxy.MarcomManager.User.Id);

                    _subdata.IsNotificationEnable = dao.IsNotificationEnable;
                    _subdata.NoOfDays = dao.NoOfDays;
                    _subdata.NotificationTiming = dao.NotificationTiming;
                    _subdata.IsEmailEnable = dao.IsEmailEnable;
                    _subdata.MailTiming = dao.MailTiming;

                }
                return _subdata;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        /// <summary>
        /// Get subscription by user id.
        /// </summary>
        /// <param name="proxy"> </param>
        /// <returns>bool</returns>
        public bool GetIsSubscribedFromSettings(CommonManagerProxy proxy)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    //UserDefaultSubscriptionDao dao = new UserDefaultSubscriptionDao();
                    // dao = tx.PersistenceManager.CommonRepository.Get<UserDefaultSubscriptionDao>(UserDefaultSubscriptionDao.PropertyNames.Userid, proxy.MarcomManager.User.Id);
                    var dao = (from data1 in tx.PersistenceManager.CommonRepository.Query<UserDefaultSubscriptionDao>() where data1.UserID == (int)proxy.MarcomManager.User.Id select data1).FirstOrDefault().ToString();
                    if (dao != null)
                        return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public Tuple<IList<EntityTypeDao>, string[]> GetNotAssociateEntityTypes(CommonManagerProxy proxy)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    IList<EntityTypeDao> dao = new List<EntityTypeDao>();
                    dao = tx.PersistenceManager.CommonRepository.GetEquals<EntityTypeDao>(EntityTypeDao.PropertyNames.IsAssociate, false).ToList();
                    string[] str = new string[500];

                    str = (proxy.MarcomManager.User.Feedselection.Split(','));
                    if (dao != null)
                    {
                        var feedsetting = Tuple.Create(dao, str);
                        return feedsetting;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        #region Instance of Classes In ServiceLayer reference

        public IWidget Widgetservice()
        {
            return new Widget();
        }

        #endregion


        //---------------------------------------------------new logic for mail functionality----------------------------------------//

        public bool InsertMail(MailHolder mailHolder, string subject, string body)
        {
            //using (ITransaction tx = proxy.MarcomManager.GetTransaction())
            //{
            Mail.MailServer.Instance.InsertMailIntoDb(mailHolder);

            //if (mailHolder.PrimaryTo != null)
            //{                    
            //    Mail.MailServer.Instance.HandleSendMail();
            //}

            return true;
            // }


        }
        public bool HandleUnScheduledMail(CommonManagerProxy proxy, MailHolder mailHolder, string subject, string body)
        {
            //using (ITransaction tx = proxy.MarcomManager.GetTransaction())
            //{
            Mail.MailServer.Instance.HandleUnScheduledMail(mailHolder);
            //}

            return true;
        }
        public bool HandleSendMail(CommonManagerProxy proxy)
        {
            //using (ITransaction tx = proxy.MarcomManager.GetTransaction())
            //{
            Mail.MailServer.Instance.HandleSendMail();
            //}

            return true;
        }


        //-------------------getusersid based on entityid and notification temolate to send mail--------------------
        public IList<int> GetListofUserIdForNotificationbyMail(CommonManagerProxy proxy, int notificationtemplateid, int entityid)
        {

            try
            {
                IList<int> users = new List<int>();
                IList<UserNotificationDao> usernotification = new List<UserNotificationDao>();
                IList<UserMailSubscriptionDao> usermailsubscription = new List<UserMailSubscriptionDao>();
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    UserNotificationDao dao = new UserNotificationDao();

                    IList<MultiProperty> prpList = new List<MultiProperty>();
                    prpList.Add(new MultiProperty { propertyName = UserNotificationDao.PropertyNames.Typeid, propertyValue = notificationtemplateid });
                    prpList.Add(new MultiProperty { propertyName = UserNotificationDao.PropertyNames.Entityid, propertyValue = entityid });
                    prpList.Add(new MultiProperty { propertyName = UserNotificationDao.PropertyNames.IsSentInMail, propertyValue = false });
                    usernotification = tx.PersistenceManager.CommonRepository.GetEquals<UserNotificationDao>(prpList);

                    IList<MultiProperty> prpList1 = new List<MultiProperty>();
                    prpList1.Add(new MultiProperty { propertyName = UserMailSubscriptionDao.PropertyNames.RecapReport, propertyValue = true });
                    prpList1.Add(new MultiProperty { propertyName = UserMailSubscriptionDao.PropertyNames.IsEmailEnable, propertyValue = true });

                    usermailsubscription = tx.PersistenceManager.CommonRepository.GetEquals<UserMailSubscriptionDao>(prpList1);

                    //  var u = (from tt in usernotification where tt.IsSentInMail == false select tt.Userid) .ToList<int>();


                    var t = (from x in usernotification.ToList()
                             join y in usermailsubscription on x.Userid equals y.Userid into result
                             from val in result
                             select val).ToList();
                    foreach (var c in t)
                    {
                        users.Add(c.Userid);
                    }
                    return users;
                }

            }
            catch (Exception ex)
            {
                return null;
            }
            return null;
        }


        public bool InsertPoSettingXML(CommonManagerProxy commonManagerProxy, string Prefix, string DateFormat, string DigitFormat, string NumberCount)
        {
            try
            {



                IPurchaseOrderSettings posetting = new PurchaseOrderSettings();

                PurchaseOrderSettingsDao obj = new PurchaseOrderSettingsDao(Prefix, DateFormat, DigitFormat, NumberCount, DateTime.Today);
                string xmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
                bool SavedXML = PersistenceManager.Instance.CommonRepository.SaveObject<PurchaseOrderSettingsDao>(xmlpath, obj);

                IList<PurchaseOrderSettingsDao> alreadyExistCount = new List<PurchaseOrderSettingsDao>();
                alreadyExistCount = GetPoSSettings(commonManagerProxy, "PurchaseOrderSettings_Table");

                using (ITransaction tx = commonManagerProxy.MarcomManager.GetTransaction())
                {
                    if (alreadyExistCount.Count() > 1)
                    {
                        var sequencenumber = new StringBuilder();
                        sequencenumber.Append("ALTER SEQUENCE PurchaseOrderID RESTART WITH ? ");
                        tx.PersistenceManager.CommonRepository.ExecuteQuerywithMinParam(sequencenumber.ToString(), DigitFormat);

                    }
                    else
                    {
                        var sequencenumber = new StringBuilder();
                        sequencenumber.Append("CREATE SEQUENCE PurchaseOrderID ");
                        sequencenumber.Append("START WITH ? ");
                        sequencenumber.Append("INCREMENT BY 1 ; ");
                        tx.PersistenceManager.CommonRepository.ExecuteQuerywithMinParam(sequencenumber.ToString(), DigitFormat);
                    }
                    tx.Commit();
                }

                return SavedXML;



            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public IList<PurchaseOrderSettingsDao> GetPoSSettings(CommonManagerProxy proxy, string PoSettings)
        {

            string xmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
            XDocument adminXdoc = XDocument.Load(xmlpath);
            var result = adminXdoc.Descendants(PoSettings).Select(a => a).ToList();
            var xElementResult = result[0];


            IList<PurchaseOrderSettingsDao> lstStudent = null;
            lstStudent = adminXdoc.Root.Elements(PoSettings).Elements("PurchaseOrderSettings").Select(e => new PurchaseOrderSettingsDao
            {

                Prefix = e.Element("Prefix").Value.Replace("\n", ""),
                DateFormat = e.Element("DateFormat").Value.Replace("\n", ""),
                NumberCount = e.Element("NumberCount").Value.Replace("\n", ""),
                DigitFormat = e.Element("DigitFormat").Value.Replace("\n", "")

            }).Cast<PurchaseOrderSettingsDao>().OrderByDescending(a => a.ID).ToList();

            return lstStudent.OrderByDescending(a => a.ID).ToList();

        }

        public List<object> GetProofHQSSettings(CommonManagerProxy proxy)
        {
            try
            {

                string xmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
                XDocument adminXdoc = XDocument.Load(xmlpath);
                List<object> lstResult = new List<object>();
                var result = (from AdminAttributes in adminXdoc.Descendants("ProofHQSettings_Table").Descendants("ProofHQSettings")
                              select new
                              {
                                  UserName = HttpUtility.HtmlDecode(AdminAttributes.Element("Email").Value),
                                  Password = HttpUtility.HtmlDecode(AdminAttributes.Element("Password").Value)
                              }).Distinct().ToList();
                lstResult.Add(new { UserCredential = result[0] });
                return lstResult;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public bool UpdateProofHQSSettings(CommonManagerProxy proxy, string userName, string password)
        {
            try
            {
                string xmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
                XDocument adminXdoc = XDocument.Load(xmlpath);
                string uname = System.Security.SecurityElement.Escape(userName), pwd = System.Security.SecurityElement.Escape(password);
                XElement root = adminXdoc.Root;
                // Update the proofHQ values
                root.Elements("ProofHQSettings_Table").Elements("ProofHQSettings").Select(e => e.Element("Email")).Single().SetValue(uname);
                root.Elements("ProofHQSettings_Table").Elements("ProofHQSettings").Select(e => e.Element("Password")).Single().SetValue(pwd);
                //update the comment
                adminXdoc.Save(xmlpath);
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public bool Insert_AdminEmail(CommonManagerProxy proxy, string jsondata)
        {

            string xmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
            XDocument adminXmlDoc = XDocument.Load(xmlpath);
            //The Key is root node current Settings
            string xelementName = "MailConfig";
            var xelementFilepath = XElement.Load(xmlpath);
            var xmlElement = xelementFilepath.Element(xelementName).Element("AdminEmailID");
            adminXmlDoc.Descendants("MailConfig").Descendants("AdminEmailID").FirstOrDefault().SetValue(jsondata);
            adminXmlDoc.Save(xmlpath);
            return true;
        }
        public string GetEmailids(CommonManagerProxy proxy)
        {
            string emailids = "";
            string xmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
            XDocument adminXmlDoc = XDocument.Load(xmlpath);
            //The Key is root node current Settings
            string xelementName = "MailConfig";
            var xelementFilepath = XElement.Load(xmlpath);
            var xmlElement = xelementFilepath.Element(xelementName).Element("AdminEmailID");
            emailids = xmlElement.Value.ToString();
            return emailids;
        }

        public bool InsertUpdateAdditionalSettings(CommonManagerProxy proxy, int id, string Settingname, string settingValue)
        {
            using (ITransaction tx = proxy.MarcomManager.GetTransaction())
            {
                //tx.PersistenceManager.CommonRepository.ExecuteQuery("UPDATE CM_AdditionalSettings SETSettingName ='" + Settingname + "' ,SettingValue = '" + settingValue + "' WHERE 	ID =" + id);
                tx.PersistenceManager.CommonRepository.ExecuteQuerywithMinParam("UPDATE CM_AdditionalSettings set SettingValue=? WHERE  SettingName =?", settingValue, Settingname);

                tx.Commit();
                return true;
            }
            return false;
        }



        public IList<IAdditionalSettings> GetAdditionalSettings(CommonManagerProxy proxy)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    IList<IAdditionalSettings> additionalsettings = new List<IAdditionalSettings>();
                    //tx.PersistenceManager.CommonRepository.ExecuteQuery("UPDATE CM_AdditionalSettings SETSettingName ='" + Settingname + "' ,SettingValue = '" + settingValue + "' WHERE 	ID =" + id);
                    var result = tx.PersistenceManager.CommonRepository.ExecuteQuery("SELECT id,SettingName,SettingValue FROM CM_AdditionalSettings").Cast<Hashtable>();
                    foreach (var val in result)
                    {
                        //  additionalsettings = new List<IAdditionalSettings>();
                        IAdditionalSettings additionalsettings1 = new AdditionalSettings();
                        additionalsettings1.Id = Convert.ToInt32(val["id"]);
                        additionalsettings1.SettingName = val["SettingName"].ToString();
                        additionalsettings1.SettingValue = val["SettingValue"].ToString();
                        if (Convert.ToInt32(val["id"]) == 2)
                        {
                            var currlist = tx.PersistenceManager.CommonRepository.Query<CurrencyTypeDao>().Where(a => a.Id == Convert.ToInt32(val["SettingValue"])).Select(a => a).ToList();
                            foreach (var cc in currlist)
                            {
                                ICurrencyType cur = new CurrencyType();
                                cur.Id = cc.Id;
                                cur.Name = cc.Name;
                                cur.ShortName = cc.ShortName;
                                cur.Symbol = cc.Symbol;
                                additionalsettings1.CurrencyFormatvalue = new List<ICurrencyType>();
                                additionalsettings1.CurrencyFormatvalue.Add(cur);
                            }


                            // additionalsettings.Add(new AdditionalSettings { Id = onvert.ToInt32(val["id"]), SettingName = val["SettingName"].ToString(), SettingValue = val["SettingValue"].ToString() });
                        }
                        additionalsettings.Add(additionalsettings1);
                    }
                    tx.Commit();

                    proxy.MarcomManager.GlobalAdditionalSettings = additionalsettings;
                    return additionalsettings;

                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public IList<ILanguageType> GetLanguageTypes(CommonManagerProxy proxy)
        {
            using (ITransaction tx = proxy.MarcomManager.GetTransaction())
            {
                try
                {
                    IList<ILanguageType> languagetypesettings = new List<ILanguageType>();
                    IList<LanguageTypeDao> dao = new List<LanguageTypeDao>();
                    dao = tx.PersistenceManager.CommonRepository.GetAll<LanguageTypeDao>();

                    foreach (var item in dao)
                    {
                        var pendingtranslation = tx.PersistenceManager.CommonRepository.ExecuteQuery("SELECT COUNT(Lang" + item.ID + "_IsUpdated) AS PendingTranslation FROM CM_LanguageContent WHERE Lang" + item.ID + "_IsUpdated=0");
                        foreach (var item2 in pendingtranslation)
                        {
                            var languageid = 1;

                            string xmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
                            XDocument adminXmlDoc = XDocument.Load(xmlpath);
                            var DefaultLanguage = adminXmlDoc.Descendants("DefaultLanguage").FirstOrDefault();
                            if (DefaultLanguage != null)
                            {
                                languageid = Convert.ToInt32(adminXmlDoc.Descendants("DefaultLanguage").ElementAt(0).Value);
                            }
                            bool IsDefaultLanguage = false;
                            if (item.ID == Convert.ToInt32(languageid))
                            {
                                IsDefaultLanguage = true;
                            }

                            languagetypesettings.Add(new LanguageType { ID = item.ID, Name = item.Name, Description = item.Description, InheritedFrom = item.InheritedFrom, AddedOn = item.AddedOn, PendingTranslation = (int)((System.Collections.Hashtable)(item2))["PendingTranslation"], IsDefaultLanguage = IsDefaultLanguage });
                        }
                        tx.Commit();
                    }

                    return languagetypesettings;

                }
                catch (Exception ex)
                {
                    return null;
                }
            }

        }

        public bool SaveNewLanguage(CommonManagerProxy proxy, int InheritedId, string Name, string Description)
        {
            try
            {
                LanguageTypeDao langtype = new LanguageTypeDao();

                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    DateTime now = DateTime.Now;
                    langtype.InheritedFrom = InheritedId;
                    //langtype.ID = 0;
                    langtype.Name = Name;
                    langtype.Description = Description;
                    langtype.AddedOn = now.Month + "/" + now.Day + "/" + now.Year;
                    tx.PersistenceManager.CommonRepository.Save<LanguageTypeDao>(langtype);

                    string strqry = "ALTER table CM_LanguageContent ADD Lang" + langtype.ID + "_Caption NVARCHAR(1000), Lang" + langtype.ID + "_IsUpdated BIT";
                    tx.PersistenceManager.CommonRepository.ExecuteQuery(strqry);
                    tx.Commit();
                }
                using (ITransaction tx2 = proxy.MarcomManager.GetTransaction())
                {
                    string strinherit = "Select Lang" + InheritedId + "_Caption as Caption,ID from CM_LanguageContent";
                    var Inheritedvalues = tx2.PersistenceManager.CommonRepository.ExecuteQuery(strinherit);
                    foreach (var item in Inheritedvalues)
                    {
                        try
                        {
                            string abc = "UPDATE CM_LanguageContent set Lang" + langtype.ID + "_Caption = '" + (string)((System.Collections.Hashtable)(item))["Caption"] + "' ,Lang" + langtype.ID + "_IsUpdated = 1  where ID=" + (int)((System.Collections.Hashtable)(item))["ID"] + "";
                            tx2.PersistenceManager.CommonRepository.ExecuteQuery(abc);
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    tx2.Commit();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public IList GetLanguageContent(CommonManagerProxy proxy, int StartRows, int NextRows)
        {

            using (ITransaction tx2 = proxy.MarcomManager.GetTransaction())
            {
                StringBuilder getcontent = new StringBuilder();
                getcontent.Append("select * from CM_LanguageContent ");
                getcontent.Append(" order by ID asc ");
                getcontent.Append(" offset " + StartRows + " rows fetch next " + NextRows + " rows only");

                var result = tx2.PersistenceManager.CommonRepository.ExecuteQuery(getcontent.ToString());
                return result;
            }
        }

        public bool UpdateLanguageContent(CommonManagerProxy proxy, int LangTypeID, int ContentID, string newValue)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    string updatestring = "Update CM_LanguageContent set Lang" + LangTypeID + "_Caption = ? , Lang" + LangTypeID + "_IsUpdated=ISNULL(Lang" + LangTypeID + "_IsUpdated,0) + 1 where ID = ?";
                    tx.PersistenceManager.CommonRepository.ExecuteQuerywithMinParam(updatestring, newValue, ContentID);
                    tx.Commit();
                }
                return true;
            }
            catch
            {

                return false;
            }
        }

        public string GetLanguageSettings(CommonManagerProxy proxy, int LangID)
        {
            using (ITransaction tx = proxy.MarcomManager.GetTransaction())
            {
                var languageid = 1;

                string xmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
                XDocument adminXmlDoc = XDocument.Load(xmlpath);
                var DefaultLanguage = adminXmlDoc.Descendants("DefaultLanguage").FirstOrDefault();

                //var DefaultLanguage = adminXmlDoc.Descendants("DefaultLanguage").ElementAt(0).Value;

                var languagetypeid = tx.PersistenceManager.CommonRepository.Get<UserDao>(UserDao.PropertyNames.Id, proxy.MarcomManager.User.Id).LanguageSettings;
                if (languagetypeid == 0)
                {
                    if (DefaultLanguage == null)
                    {
                    }
                    else
                    {
                        languageid = Convert.ToInt32(adminXmlDoc.Descendants("DefaultLanguage").ElementAt(0).Value);
                    }
                }
                else
                {
                    languageid = languagetypeid;
                }
                string strget = "select ID,Lang" + languageid + "_Caption as Caption from CM_LanguageContent";
                var result = tx.PersistenceManager.CommonRepository.ExecuteQuery(strget);

                StringBuilder stringjsong = new StringBuilder();
                stringjsong.Append("{ ");
                int count = result.Count;
                int i = 1;
                foreach (var item in result)
                {
                    try
                    {
                        string captionval = ((string)((System.Collections.Hashtable)(item))["Caption"]) == null ? "" : ((string)((System.Collections.Hashtable)(item))["Caption"]).Replace("\"", "\\\"");
                        stringjsong.Append("\"Res_" + (int)((System.Collections.Hashtable)(item))["ID"] + "\":\"" + captionval + "\"");
                        if (count > i)
                        {
                            stringjsong.Append(",");
                        }
                        i++;
                    }
                    catch (Exception ex)
                    {

                    }
                }
                stringjsong.Append(" }");
                return stringjsong.ToString();
            }
        }

        /// <summary>
        /// Get Running PO Number
        /// </summary>
        /// <param name="proxy"></param>
        /// <returns>Running PO number<Iwidget> </returns>
        public string GetCurrentPONumber(CommonManagerProxy proxy)
        {

            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {

                    string ExactRandomNumber = "";
                    var sequencenumber = new StringBuilder();
                    sequencenumber.Append("SELECT TOP 1 LastPO FROM MM_CurrentPONumberHistory ORDER BY LastPO desc");
                    IList sequenceNoVal = tx.PersistenceManager.PlanningRepository.ExecuteQuery(sequencenumber.ToString());
                    ExactRandomNumber = (string)((System.Collections.Hashtable)(sequenceNoVal)[0])["LastPO"];
                    return ExactRandomNumber;
                }
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public bool UpdateLanguageName(CommonManagerProxy proxy, int LangTypeID, string NewValue, int NameOrDesc)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    if (NameOrDesc == 1)
                        tx.PersistenceManager.CommonRepository.ExecuteQuerywithMinParam("Update CM_LanguageType set Name= ? where ID= ? ", NewValue, LangTypeID);
                    else
                        tx.PersistenceManager.CommonRepository.ExecuteQuerywithMinParam("Update CM_LanguageType set Description=? where ID= ?", NewValue, LangTypeID);
                    tx.Commit();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool SetDefaultLanguage(CommonManagerProxy proxy, int LangID)
        {

            try
            {
                string xmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
                XDocument adminXmlDoc = XDocument.Load(xmlpath);
                var DefaultLanguage = adminXmlDoc.Descendants("DefaultLanguage").FirstOrDefault();
                if (DefaultLanguage == null)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("<DefaultLanguage>" + LangID + "</DefaultLanguage>");
                    XElement.Parse(sb.ToString());
                    adminXmlDoc.Root.Add(XElement.Parse(sb.ToString()));
                    adminXmlDoc.Save(xmlpath);
                }
                else
                {
                    adminXmlDoc.Descendants("DefaultLanguage").Remove();
                    adminXmlDoc.Save(xmlpath);
                    StringBuilder sb = new StringBuilder();
                    sb.Append("<DefaultLanguage>" + LangID + "</DefaultLanguage>");
                    XElement.Parse(sb.ToString());
                    adminXmlDoc.Root.Add(XElement.Parse(sb.ToString()));
                    adminXmlDoc.Save(xmlpath);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public IList LanguageSearch(CommonManagerProxy proxy, int langid, string searchtext, string searchaddedon, int StartRows)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    IList<MultiProperty> parLIST = new List<MultiProperty>();
                    string strsearch = "";
                    if (searchaddedon == "")
                    {
                        parLIST.Add(new MultiProperty { propertyName = "searchtext", propertyValue = searchtext + "%" });
                        strsearch = "SELECT * FROM CM_LanguageContent clc WHERE clc.Lang" + langid + "_Caption LIKE :searchtext AND clc.Lang" + langid + "_IsUpdated !=0 order by id asc offset " + StartRows + " rows fetch next 20 rows only;";
                    }
                    else if (searchaddedon != "")
                    {
                        parLIST.Add(new MultiProperty { propertyName = "searchtext", propertyValue = searchtext + "%" });
                        parLIST.Add(new MultiProperty { propertyName = "searchaddedon", propertyValue = searchaddedon });
                        strsearch = "SELECT * FROM CM_LanguageContent clc WHERE CONVERT(VARCHAR(10),AddedOn,10) >= :searchaddedon and clc.Lang" + langid + "_Caption LIKE :searchtext order by id asc offset " + StartRows + " rows fetch next 20 rows only;";
                    }
                    if (searchtext == "")
                    {
                        strsearch = "";
                        strsearch = "select * from CM_LanguageContent order by ID asc  offset 0 rows fetch next 20 rows only;";
                        parLIST.Clear();
                    }
                    var searchlist = tx.PersistenceManager.CommonRepository.ExecuteQuerywithParam(strsearch, parLIST);
                    return searchlist;
                }
            }
            catch
            {

                return null;
            }
        }

        public IList LanguageSearchs(CommonManagerProxy proxy, int langid, string searchtext)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    IList<MultiProperty> parLIST = new List<MultiProperty>();
                    string strsearch = "";
                    strsearch = "SELECT * FROM CM_LanguageContent clc WHERE clc.Lang" + langid + "_Caption LIKE '%" + searchtext + "%'";

                    var searchlist = tx.PersistenceManager.CommonRepository.ExecuteQuery(strsearch);
                    return searchlist;
                }
            }
            catch
            {

                return null;
            }
        }
        public int GetDefaultLangFromXML(CommonManagerProxy proxy)
        {

            string xmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
            XDocument adminXmlDoc = XDocument.Load(xmlpath);
            var DefaultLanguage = adminXmlDoc.Descendants("DefaultLanguage").FirstOrDefault();

            var languageid = Convert.ToInt32(adminXmlDoc.Descendants("DefaultLanguage").ElementAt(0).Value);

            return Convert.ToInt32(languageid);
        }


        public IUserTaskNotificationMailSettings GetUserDefaultTaskNotificationMailSettings(CommonManagerProxy proxy)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    IUserTaskNotificationMailSettings UsertaskNotify = new UserTaskNotificationMailSettings();
                    var userNotiFydao = tx.PersistenceManager.TaskRepository.Get<UserTaskNotificationMailSettingsDao>(UserTaskNotificationMailSettingsDao.PropertyNames.Userid, proxy.MarcomManager.User.Id);
                    if (userNotiFydao != null)
                    {
                        UsertaskNotify.Id = userNotiFydao.Id;
                        UsertaskNotify.IsEmailEnable = userNotiFydao.IsEmailEnable;
                        UsertaskNotify.IsNotificationEnable = userNotiFydao.IsNotificationEnable;
                        UsertaskNotify.LastSentOn = userNotiFydao.LastSentOn;
                        UsertaskNotify.LastUpdatedOn = userNotiFydao.LastUpdatedOn;
                        UsertaskNotify.MailTiming = userNotiFydao.MailTiming;
                        UsertaskNotify.NoOfDays = userNotiFydao.NoOfDays;
                        UsertaskNotify.NotificationTiming = userNotiFydao.NotificationTiming;
                        UsertaskNotify.Userid = userNotiFydao.Userid;
                        UsertaskNotify.IsEmailAssignedEnable = userNotiFydao.IsEmailAssignedEnable;
                    }

                    return UsertaskNotify;
                }
            }
            catch
            {

            }
            return null;

        }

        public string GetEditorText(CommonManagerProxy proxy)
        {
            string xmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
            XDocument adminXdoc = XDocument.Load(xmlpath);
            var result = adminXdoc.Descendants("SupportText").ElementAt(0).Value;
            return result;

        }

        public bool InsertEditortext(CommonManagerProxy proxy, int[] entityList, String Content = null)
        {
            try
            {
                proxy.MarcomManager.AccessManager.TryAccess(Modules.Common, FeatureID.Support, OperationId.Edit);
                string xmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
                XDocument adminXmlDoc = XDocument.Load(xmlpath);
                StringBuilder sb = new StringBuilder();
                adminXmlDoc.Descendants("SupportText").Remove();
                adminXmlDoc.Root.Add(new XElement("SupportText", Content));
                adminXmlDoc.Save(xmlpath);
                return true;
            }
            catch (MarcomAccessDeniedException ex)
            {
                throw ex;
            }
            catch
            {
                return false;
            }
        }
        public IList<SSO> GetSSODetails(CommonManagerProxy proxy)
        {
            try
            {
                string xmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
                XDocument adminXmlDoc = XDocument.Load(xmlpath);
                //The Key is root node current Settings
                string xelementName = "SSO";
                var xelementFilepath = XElement.Load(xmlpath);
                var xmlElement = xelementFilepath.Element(xelementName);
                IList<SSO> ssolist = new List<SSO>();
                SSO sso = new SSO();
                sso.Algorithmoption = new List<string>();


                var algos = Enum.GetValues(typeof(BrandSystems.Cryptography.Security.AlgoType)).Cast<BrandSystems.Cryptography.Security.AlgoType>().Select(v => v.ToString());

                foreach (var obj in algos)
                {
                    sso.Algorithmoption.Add(obj);
                }
                sso.PaddingModeoption = new List<string>();
                var paddingmodes = Enum.GetValues(typeof(System.Security.Cryptography.PaddingMode)).Cast<System.Security.Cryptography.PaddingMode>().Select(v => v.ToString());
                foreach (var obj in paddingmodes)
                {
                    sso.PaddingModeoption.Add(obj);
                }
                sso.CipherModeoption = new List<string>();
                var ciphermodes = Enum.GetValues(typeof(System.Security.Cryptography.CipherMode)).Cast<System.Security.Cryptography.CipherMode>().Select(v => v.ToString());
                foreach (var obj in ciphermodes)
                {
                    sso.CipherModeoption.Add(obj);
                }

                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    IList<GlobalRoleDao> groups = new List<GlobalRoleDao>();
                    groups = (from role in tx.PersistenceManager.AccessRepository.Query<GlobalRoleDao>() select role).ToList<GlobalRoleDao>();
                    sso.UserGroupsoption = new List<GlobalRole>();
                    foreach (var str in groups)
                    {
                        GlobalRole grb = new GlobalRole();
                        grb.Caption = str.Caption;
                        grb.Id = str.Id;
                        sso.UserGroupsoption.Add(grb);
                    }

                }

                sso.SSOTokenModeoption = new List<string>();
                sso.SSOTokenModeoption.Add("TEXT");
                sso.SSOTokenModeoption.Add("XML");

                foreach (var des in xmlElement.Descendants())
                {

                    switch (des.Name.ToString())
                    {
                        case "Key":
                            sso.key = des.Value.ToString();
                            break;
                        case "IV":
                            sso.IV = des.Value.ToString();
                            break;
                        case "Algorithm":
                            sso.AlgorithmValue = des.Value.ToString();
                            break;
                        case "PaddingMode":
                            sso.PaddingModeValue = des.Value.ToString();
                            break;
                        case "CipherMode":
                            sso.CipherModeValue = des.Value.ToString();
                            break;
                        case "SSOTokenMode":
                            sso.SSOTokenModeValue = des.Value.ToString();
                            break;
                        case "SSOTimeDifference":
                            sso.SSOTimeDifference = des.Value.ToString();
                            break;
                        case "SSODefaultGroups":
                            sso.UserGroupsvalue = des.Value.ToString();
                            break;
                        case "IntranetLoginUrl":
                            sso.ClientIntranetUrl = des.Value.ToString();
                            break;
                        default:
                            break;

                    }

                }
                ssolist.Add(sso);
                return ssolist;
            }
            catch
            {
                return null;
            }

        }

        public bool UpdateSSOSettings(CommonManagerProxy proxy, string key, string iv, string algo, string paddingmode, string ciphermode, string tokenmode, string SSOTimeDifference, string ssousergroups, string ClientIntranetUrl)
        {
            try
            {
                string xmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
                string xelementName = "SSO";
                var xelementFilepath = XElement.Load(xmlpath);
                var xmlElement = xelementFilepath.Element(xelementName);
                foreach (var des in xmlElement.Descendants())
                {
                    switch (des.Name.ToString())
                    {
                        case "Key":
                            des.Value = key;
                            break;
                        case "IV":
                            des.Value = iv;
                            break;
                        case "Algorithm":
                            des.Value = algo;
                            break;
                        case "PaddingMode":
                            des.Value = paddingmode;
                            break;
                        case "CipherMode":
                            des.Value = ciphermode;
                            break;
                        case "SSOTokenMode":
                            des.Value = tokenmode;
                            break;
                        case "SSOTimeDifference":
                            des.Value = SSOTimeDifference;
                            break;
                        case "SSODefaultGroups":
                            des.Value = ssousergroups;
                            break;
                        case "IntranetLoginUrl":
                            des.Value = ClientIntranetUrl;
                            break;
                        default:
                            break;

                    }

                }

                xelementFilepath.Save(xmlpath);

                return true;
            }
            catch
            {
                return false;
            }
        }


        public bool IsActiveEntity(CommonManagerProxy proxy, int EntityID)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {

                    return tx.PersistenceManager.CommonRepository.Query<EntityDao>().Where(a => a.Id == EntityID).Select(a => a.Active).FirstOrDefault();

                }

            }
            catch
            {

            }

            return false;

        }





        public IList<IUnits> GetUnits(CommonManagerProxy proxy)
        {
            try
            {
                IList<IUnits> objunitsList = new List<IUnits>();
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    var unitsDao = tx.PersistenceManager.PlanningRepository.GetAll<UnitsDao>();
                    tx.Commit();
                    foreach (var obj in unitsDao)
                    {
                        Units unitsObj = new Units();
                        unitsObj.Id = obj.Id;
                        unitsObj.Caption = obj.Caption;
                        objunitsList.Add(unitsObj);
                    }
                }
                return objunitsList;
            }
            catch (Exception ex)
            {
                throw null;
            }
        }


        public IList<IFeedFilterGroup> GetFilterGroup(CommonManagerProxy proxy)
        {
            try
            {
                IList<IFeedFilterGroup> objfeedgroupList = new List<IFeedFilterGroup>();
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    var unitsDao = tx.PersistenceManager.PlanningRepository.GetAll<FeedFilterGroupDao>();
                    tx.Commit();

                    bool blnFinancial = false;
                    bool blnTask = false;
                    bool blnObjectives = false;
                    bool blnAttachmentsView = false;
                    bool blnAttachmentEdit = false;
                    blnFinancial = proxy.MarcomManager.AccessManager.CheckUserAccess((int)Modules.Planning, (int)FeatureID.Financials);
                    blnTask = proxy.MarcomManager.AccessManager.CheckUserAccess((int)Modules.Planning, (int)FeatureID.Task);
                    blnObjectives = proxy.MarcomManager.AccessManager.CheckUserAccess((int)Modules.Planning, (int)FeatureID.Objective);
                    blnAttachmentsView = proxy.MarcomManager.AccessManager.CheckUserAccess((int)Modules.Planning, (int)FeatureID.Attachments);
                    blnAttachmentEdit = proxy.MarcomManager.AccessManager.CheckUserAccess((int)Modules.Planning, (int)FeatureID.AttchmentEdit);

                    foreach (var obj in unitsDao)
                    {
                        FeedFilterGroup unitsObj = new FeedFilterGroup();
                        if (obj.FeedGroup == "Financial")
                        {
                            if (blnFinancial)
                            {
                                unitsObj.Id = obj.Id;
                                unitsObj.FeedGroup = obj.FeedGroup;
                                unitsObj.Template = obj.Template;

                            }

                        }
                        else if (obj.FeedGroup == "Task information")
                        {

                            if (blnTask)
                            {
                                unitsObj.Id = obj.Id;
                                unitsObj.FeedGroup = obj.FeedGroup;
                                unitsObj.Template = obj.Template;

                            }

                        }
                        else if (obj.FeedGroup == "Objectives & Assignments")
                        {

                            if (blnObjectives)
                            {
                                unitsObj.Id = obj.Id;
                                unitsObj.FeedGroup = obj.FeedGroup;
                                unitsObj.Template = obj.Template;

                            }


                        }
                        else if (obj.FeedGroup == "Attachments")
                        {
                            if (blnAttachmentsView || blnAttachmentEdit)
                            {
                                unitsObj.Id = obj.Id;
                                unitsObj.FeedGroup = obj.FeedGroup;
                                unitsObj.Template = obj.Template;

                            }


                        }
                        else
                        {
                            unitsObj.Id = obj.Id;
                            unitsObj.FeedGroup = obj.FeedGroup;
                            unitsObj.Template = obj.Template;
                        }
                        objfeedgroupList.Add(unitsObj);
                    }
                }
                IList<IFeedFilterGroup> objfeedList = objfeedgroupList.Where(x => x.Id != 0).ToList();
                return objfeedList;
            }
            catch (Exception ex)
            {
                throw null;
            }
        }







        public bool DeleteFeedGroupByid(CommonManagerProxy commonManagerProxy, int id)
        {
            try
            {
                FeedFilterGroupDao dao = new FeedFilterGroupDao();
                using (ITransaction tx = commonManagerProxy.MarcomManager.GetTransaction())
                {
                    dao = tx.PersistenceManager.PlanningRepository.Get<FeedFilterGroupDao>(id);
                    tx.PersistenceManager.PlanningRepository.Delete<FeedFilterGroupDao>(dao);
                    tx.Commit();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public int InsertUpdateFeedFilterGroup(CommonManagerProxy commonManagerProxy, int Id, string feedfiltergroupname, string feedactions)
        {

            try
            {
                using (ITransaction tx = commonManagerProxy.MarcomManager.GetTransaction())
                {

                    FeedFilterGroupDao dao = new FeedFilterGroupDao();
                    if (Id != 0)
                        dao.Id = Id;
                    dao.FeedGroup = feedfiltergroupname;
                    dao.Template = feedactions;
                    tx.PersistenceManager.PlanningRepository.Save<FeedFilterGroupDao>(dao);
                    tx.Commit();
                    return dao.Id;

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return 0;
        }

        public IList<IFeedTemplate> GetFeedTemplates(CommonManagerProxy proxy)
        {
            try
            {
                IList<IFeedTemplate> objfeedgroupList = new List<IFeedTemplate>();
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    var feedTemplates = tx.PersistenceManager.CommonRepository.GetAll<FeedTemplateDao>();

                    foreach (var ele in feedTemplates)
                    {
                        IFeedTemplate obj = new FeedTemplate();
                        obj.Id = ele.Id;
                        obj.Template = ele.Template;
                        obj.Action = ele.Action;
                        objfeedgroupList.Add(obj);
                    }
                    tx.Commit();

                }
                return objfeedgroupList;
            }
            catch (Exception ex)
            {
                throw null;
            }
        }


        /// <summary>
        /// Gets the Units.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public IList<IUnits> GetUnitsByID(CommonManagerProxy proxy, int id)
        {
            try
            {
                IList<IUnits> getunitslist = new List<IUnits>();
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    IList<UnitsDao> unitsdao = new List<UnitsDao>();
                    unitsdao = tx.PersistenceManager.CommonRepository.GetAll<UnitsDao>();
                    tx.Commit();
                    var linqunits = from t in unitsdao where t.Id == id select t;
                    foreach (var temp in linqunits.ToList())
                    {
                        IUnits units = new Units();
                        units.Id = temp.Id;
                        units.Caption = temp.Caption;

                        getunitslist.Add(units);
                    }
                }
                return getunitslist;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        ////public bool DeleteUnitsByid(CommonManagerProxy proxy ,int ID)
        ////{
        ////IUnits units = new Units();
        ////try
        ////{

        ////using (ITransaction tx = proxy.MarcomManager.GetTransaction())
        ////{
        ////tx.PersistenceManager.CommonRepository.DeleteByID<UnitsDao>(UnitsDao.PropertyNames.Id, ID);
        ////tx.Commit();
        ////return true;
        ////}
        ////}
        ////catch
        ////{
        ////}
        ////return false;
        ////}


        public bool DeleteUnitsByid(CommonManagerProxy commonManagerProxy, int id)
        {
            try
            {
                UnitsDao dao = new UnitsDao();
                using (ITransaction tx = commonManagerProxy.MarcomManager.GetTransaction())
                {
                    dao = tx.PersistenceManager.PlanningRepository.Get<UnitsDao>(id);
                    tx.PersistenceManager.PlanningRepository.Delete<UnitsDao>(dao);
                    tx.Commit();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public int InsertAdminNotificationSettings(CommonManagerProxy proxy, JArray subscriptionObject)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    IList<SubscriptionTypeDao> listOfSubscriptions = new List<SubscriptionTypeDao>();
                    SubscriptionTypeDao subscriptionType = new SubscriptionTypeDao();

                    foreach (var jobj in subscriptionObject)
                    {
                        subscriptionType = new SubscriptionTypeDao();
                        subscriptionType.Caption = jobj["Caption"] == null ? "" : (string)jobj["Caption"];
                        subscriptionType.isAppDefault = jobj["isAppDefault"] == null ? false : (bool)jobj["isAppDefault"];
                        subscriptionType.isAppMandatory = jobj["isAppMandatory"] == null ? false : (bool)jobj["isAppMandatory"];
                        subscriptionType.isMailDefault = jobj["isMailDefault"] == null ? false : (bool)jobj["isMailDefault"];
                        subscriptionType.isMailMandatory = jobj["isMailMandatory"] == null ? false : (bool)jobj["isMailMandatory"];
                        subscriptionType.Id = jobj["Id"] == null ? 0 : (int)jobj["Id"];

                        listOfSubscriptions.Add(subscriptionType);
                    }

                    tx.PersistenceManager.CommonRepository.Save<SubscriptionTypeDao>(listOfSubscriptions);

                    tx.Commit();
                    return 1;
                }
            }
            catch (Exception ex)
            {

            }
            return 0;
        }
        public bool InsertUpdateUnits(CommonManagerProxy proxy, string caption, int ID)
        {
            try
            {
                IList<MultiProperty> parLIST = new List<MultiProperty>();
                UnitsDao dao = new UnitsDao();
                dao.Caption = caption;
                if (ID != 0)
                {
                    dao.Id = ID;
                }
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    if (ID == 0)
                    {
                        parLIST.Add(new MultiProperty { propertyName = "caption", propertyValue = caption });
                        // var query = "INSERT INTO PM_Objective_Unit select  MAX(ID)+1 , '" + caption + "'  from PM_Objective_Unit";
                        var query = "INSERT INTO PM_Objective_Unit select  isnull(MAX(id),0)+1 , :caption  from PM_Objective_Unit ";
                        tx.PersistenceManager.CommonRepository.ExecuteQuerywithParam(query.ToString(), parLIST);
                    }

                    else
                    {
                        parLIST.Add(new MultiProperty { propertyName = "caption", propertyValue = caption });
                        parLIST.Add(new MultiProperty { propertyName = "ID", propertyValue = ID });
                        var query = "UPDATE PM_Objective_Unit set Caption = :caption where ID= :ID";
                        tx.PersistenceManager.CommonRepository.ExecuteQuerywithParam(query.ToString(), parLIST);
                    }

                    tx.Commit();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool InsertCurrencyFormat(CommonManagerProxy proxy, string ShortName, string Name, string Symbol, int Id = 0)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    CurrencyTypeDao currency = new CurrencyTypeDao();
                    if (Id != 0)
                        currency.Id = Id;
                    currency.ShortName = ShortName;
                    currency.Name = Name;
                    currency.Symbol = Symbol;
                    tx.PersistenceManager.CommonRepository.Save<CurrencyTypeDao>(currency);
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
        /// GetUniqueuserhit
        /// </summary>
        /// <param name="proxy"></param>
        /// <param name="year"> </param>
        /// <param name="month"> </param>
        /// <returns>Ilist </returns>
        public IList GetUniqueuserhit(CommonManagerProxy proxy, int year, int month)
        {
            IList<MultiProperty> parLIST = new List<MultiProperty>();
            try
            {
                IList listresult;
                StringBuilder strqry = new StringBuilder();

                parLIST.Add(new MultiProperty { propertyName = "Year", propertyValue = year });


                if (month > 0)
                {
                    parLIST.Add(new MultiProperty { propertyName = "Month", propertyValue = month });
                    strqry.Append("select a.days AS Days,count(*) as value from (SELECT UserID,DATEPART(DAY,LoginTime)AS DAYS FROM UM_LoginDetail WHERE  YEAR(logintime)= :Year  ");
                    strqry.Append(" AND MONTH(logintime)= :Month ");
                    strqry.Append(" GROUP BY UserID,DATEPART(DAY,LoginTime) )a group by a.days ");
                }
                else
                {
                    strqry.Append("select a.days AS Days,count(*) as value from (SELECT UserID,DATEPART(MONTH,LoginTime)AS DAYS FROM UM_LoginDetail WHERE  YEAR(logintime)= :Year  GROUP BY UserID,DATEPART(MONTH,LoginTime) )a group by a.days ");
                }


                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    listresult = tx.PersistenceManager.CommonRepository.ExecuteQuerywithParam(strqry.ToString(), parLIST);

                    tx.Commit();
                }

                return listresult;

            }
            catch (Exception)
            {

                return null;
            }

        }
        //<summary>
        //GetApplicationhit
        //</summary>
        //<param name="proxy"></param>
        //<param name="year"> </param>
        //<param name="month"> </param>
        //<returns>Ilist </returns>
        public IList GetApplicationhit(CommonManagerProxy proxy, int year, int month)
        {
            IList<MultiProperty> parLIST = new List<MultiProperty>();
            try
            {
                IList listresult;
                StringBuilder strqry = new StringBuilder();

                parLIST.Add(new MultiProperty { propertyName = "Year", propertyValue = year });


                if (month > 0)
                {
                    parLIST.Add(new MultiProperty { propertyName = "Month", propertyValue = month });
                    strqry.Append(" SELECT DATEPART(DAY,LoginTime)AS Days,count(*) as value FROM UM_LoginDetail WHERE YEAR(logintime)= :Year   ");
                    strqry.Append(" AND MONTH(logintime)= :Month ");
                    strqry.Append(" GROUP BY DATEPART(DAY,LoginTime) ORDER BY DAYS ");
                }
                else
                {
                    strqry.Append(" SELECT DATEPART(MONTH,LoginTime)AS Days,count(*) as value FROM UM_LoginDetail WHERE YEAR(logintime)= :Year   ");
                    strqry.Append(" GROUP BY DATEPART(MONTH,LoginTime) ORDER BY DAYS  ");
                }


                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    listresult = tx.PersistenceManager.CommonRepository.ExecuteQuerywithParam(strqry.ToString(), parLIST);

                    tx.Commit();
                }

                return listresult;

            }
            catch (Exception)
            {

                return null;
            }

        }


        //<summary>
        //GetBrowserStatistic
        //</summary>
        //<param name="proxy"></param>
        //<param name="year"> </param>
        //<param name="month"> </param>
        //<returns>Ilist </returns>
        public IList GetBrowserStatistic(CommonManagerProxy proxy, int year, int month)
        {
            IList<MultiProperty> parLIST = new List<MultiProperty>();
            try
            {
                IList listresult;
                StringBuilder strqry = new StringBuilder();

                parLIST.Add(new MultiProperty { propertyName = "Year", propertyValue = year });
                strqry.Append("  SELECT browser AS NAME, COUNT(*) AS VALUE FROM UM_LoginDetail WHERE YEAR(logintime)= :Year   ");

                if (month > 0)
                {
                    parLIST.Add(new MultiProperty { propertyName = "Month", propertyValue = month });
                    strqry.Append(" AND MONTH(logintime)= :Month ");

                }

                strqry.Append(" GROUP BY browser ORDER BY browser  ");
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    listresult = tx.PersistenceManager.CommonRepository.ExecuteQuerywithParam(strqry.ToString(), parLIST);

                    tx.Commit();
                }

                return listresult;

            }
            catch (Exception)
            {

                return null;
            }

        }


        //<summary>
        //GetBrowserVersionStatistic
        //</summary>
        //<param name="proxy"></param>
        //<param name="year"> </param>
        //<param name="month"> </param>
        //<returns>Ilist </returns>
        public IList GetBrowserVersionStatistic(CommonManagerProxy proxy, int year, int month)
        {
            IList<MultiProperty> parLIST = new List<MultiProperty>();
            try
            {
                IList listresult;
                StringBuilder strqry = new StringBuilder();

                parLIST.Add(new MultiProperty { propertyName = "Year", propertyValue = year });
                strqry.Append(" SELECT browser AS NAME,Version as VERSION , COUNT(*) AS VALUE FROM UM_LoginDetail WHERE YEAR(logintime)= :Year	");

                if (month > 0)
                {
                    parLIST.Add(new MultiProperty { propertyName = "Month", propertyValue = month });
                    strqry.Append(" AND MONTH(logintime)= :Month ");
                }

                strqry.Append(" GROUP BY browser,Version ORDER BY browser,Version  ");
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    listresult = tx.PersistenceManager.CommonRepository.ExecuteQuerywithParam(strqry.ToString(), parLIST);

                    tx.Commit();
                }

                return listresult;

            }
            catch (Exception)
            {

                return null;
            }

        }

        //<summary>
        //GetBrowserVersionStatistic
        //</summary>
        //<param name="proxy"></param>
        //<param name="year"> </param>
        //<param name="month"> </param>
        //<returns>Ilist </returns>
        public IList GetUserStatistic(CommonManagerProxy proxy)
        {
            IList<MultiProperty> parLIST = new List<MultiProperty>();
            try
            {
                IList listresult;
                StringBuilder strqry = new StringBuilder();

                //parLIST.Add(new MultiProperty { propertyName = "Year", propertyValue = year });

                strqry.Append(" SELECT ");
                strqry.Append("case ");
                strqry.Append("when IsSSOUser =1 then 'SSOUser'  ");
                strqry.Append("when IsSSOUser =0 then 'User'  ");
                strqry.Append("End  ");
                strqry.Append("as Users , count(*) as value from  um_user group by IsSSOUser ");
                //strqry.Append("as Users , count(*) as value from  um_user where id in  ");
                // strqry.Append("(select distinct UserID FROM UM_LoginDetail WHERE YEAR(logintime)= : Year ");
                //  if (month > 0)
                //{
                //    parLIST.Add(new MultiProperty { propertyName = "Month", propertyValue = month });
                //    strqry.Append(" AND MONTH(logintime)= :Month ");
                //}

                //strqry.Append(") group by IsSSOUser ");

                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    listresult = tx.PersistenceManager.CommonRepository.ExecuteQuery(strqry.ToString());
                    //listresult = tx.PersistenceManager.CommonRepository.ExecuteQuerywithParam(strqry.ToString(), parLIST);

                    tx.Commit();
                }

                return listresult;

            }
            catch (Exception)
            {

                return null;
            }

        }

        public IList GetOSStatistic(CommonManagerProxy proxy)
        {
            IList<MultiProperty> parLIST = new List<MultiProperty>();
            try
            {
                IList listresult;
                StringBuilder strqry = new StringBuilder();

                //parLIST.Add(new MultiProperty { propertyName = "Year", propertyValue = year });
                //strqry.Append("  SELECT OS AS NAME, COUNT(*) AS VALUE FROM UM_LoginDetail WHERE YEAR(logintime)= :Year	");

                //if (month > 0)
                //{
                //    parLIST.Add(new MultiProperty { propertyName = "Month", propertyValue = month });
                //    strqry.Append(" AND MONTH(logintime)= :Month ");
                //}

                //strqry.Append(" GROUP BY OS  ");
                strqry.Append("  SELECT OS AS NAME, COUNT(*) AS VALUE FROM UM_LoginDetail GROUP BY  OS ");
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    listresult = tx.PersistenceManager.CommonRepository.ExecuteQuery(strqry.ToString());
                    //listresult = tx.PersistenceManager.CommonRepository.ExecuteQuerywithParam(strqry.ToString(), parLIST);

                    tx.Commit();
                }

                return listresult;

            }
            catch (Exception)
            {

                return null;
            }

        }
        public IList GetstartpageStatistic(CommonManagerProxy proxy)
        {
            //IList<MultiProperty> parLIST = new List<MultiProperty>();
            try
            {
                IList listresult;
                StringBuilder strqry = new StringBuilder();

                //parLIST.Add(new MultiProperty { propertyName = "Year", propertyValue = year });
                strqry.Append("select ");
                strqry.Append("case ");
                strqry.Append("when StartPage =0 then 'Setstartpage' ");
                strqry.Append("else (select caption from MM_Feature  where id=StartPage) ");
                strqry.Append("end  as page , count(*) AS value from um_user group by StartPage ");

                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    listresult = tx.PersistenceManager.CommonRepository.ExecuteQuery(strqry.ToString());

                    tx.Commit();
                }

                return listresult;

            }
            catch (Exception)
            {

                return null;
            }

        }
        public IList GetUserRoleStatistic(CommonManagerProxy proxy)
        {
            //IList<MultiProperty> parLIST = new List<MultiProperty>();
            try
            {
                IList listresult;
                StringBuilder strqry = new StringBuilder();

                //parLIST.Add(new MultiProperty { propertyName = "Year", propertyValue = year });
                strqry.Append("select ");
                strqry.Append("case ");
                strqry.Append(" when GlobalRoleId > 0 then (select Caption from AM_GlobalRole where id=GlobalRoleId) ");
                strqry.Append("END as Role ,count(*) as value from AM_GlobalRole_User  group by  GlobalRoleId  ");


                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    listresult = tx.PersistenceManager.CommonRepository.ExecuteQuery(strqry.ToString());

                    tx.Commit();
                }

                return listresult;

            }
            catch (Exception)
            {

                return null;
            }

        }
        public IList GetEnityStatistic(CommonManagerProxy proxy)
        {
            //IList<MultiProperty> parLIST = new List<MultiProperty>();
            try
            {
                IList listresult;
                StringBuilder strqry = new StringBuilder();

                //parLIST.Add(new MultiProperty { propertyName = "Year", propertyValue = year });
                strqry.Append("select ");
                strqry.Append("case ");
                strqry.Append(" when active=1 then 'Active Entityes' ");
                strqry.Append(" when active=0 then 'Deleted Entityes' ");
                strqry.Append(" end  AS EnitiesStatus, count(*)  as  value from  PM_Entity where TypeID in(select id from MM_EntityType where IsAssociate=0) group by active  ");

                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    listresult = tx.PersistenceManager.CommonRepository.ExecuteQuery(strqry.ToString());

                    tx.Commit();
                }

                return listresult;

            }
            catch (Exception)
            {

                return null;
            }

        }

        public IList GetEnityCreateationStatistic(CommonManagerProxy proxy, int year, int month)
        {
            IList<MultiProperty> parLIST = new List<MultiProperty>();
            try
            {
                IList listresult;
                StringBuilder strqry = new StringBuilder();

                parLIST.Add(new MultiProperty { propertyName = "Year", propertyValue = year });


                if (month > 0)
                {
                    parLIST.Add(new MultiProperty { propertyName = "Month", propertyValue = month });
                    strqry.Append("SELECT DATEPART(DAY,cf.HappenedOn)AS Days,count(*) value FROM  PM_Entity pe ");
                    strqry.Append(" JOIN CM_Feed  cf on cf.EntityID =pe.id ");
                    strqry.Append("AND cf.TemplateID in(18,1) ");
                    strqry.Append("AND  YEAR(cf.HappenedOn)=:Year ");
                    strqry.Append(" AND MONTH(cf.HappenedOn)= :Month ");
                    strqry.Append("AND  pe.TypeID IN(SELECT met.ID FROM  MM_EntityType met  WHERE met.IsAssociate=0) ");
                    strqry.Append("GROUP BY  DATEPART(DAY,cf.HappenedOn) ORDER BY Days  ");
                }
                else
                {
                    strqry.Append("SELECT DATEPART(MONTH,cf.HappenedOn)AS Days,count(*) value FROM  PM_Entity pe ");
                    strqry.Append(" JOIN CM_Feed  cf on cf.EntityID =pe.id ");
                    strqry.Append("AND cf.TemplateID in(18,1) ");
                    strqry.Append("AND  YEAR(cf.HappenedOn)=:Year ");
                    strqry.Append("AND  pe.TypeID IN(SELECT met.ID FROM  MM_EntityType met  WHERE met.IsAssociate=0) ");
                    strqry.Append("GROUP BY  DATEPART(MONTH,cf.HappenedOn) ORDER BY Days  ");
                }
                //strqry.Append("select a.days AS Days,count(*) as value from (SELECT UserID,DATEPART(DAY,LoginTime)AS DAYS FROM UM_LoginDetail WHERE  YEAR(logintime)= :Year  ");
                //if (month > 0)
                //{
                //    parLIST.Add(new MultiProperty { propertyName = "Month", propertyValue = month });
                //    strqry.Append(" AND MONTH(logintime)= :Month ");
                //}
                //strqry.Append(" GROUP BY UserID,DATEPART(DAY,LoginTime) )a group by a.days ");

                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    listresult = tx.PersistenceManager.CommonRepository.ExecuteQuerywithParam(strqry.ToString(), parLIST);

                    tx.Commit();
                }

                return listresult;

            }
            catch (Exception)
            {

                return null;
            }

        }

        public int InsertBroadcastMessages(CommonManagerProxy proxy, int userId, string username, string broadcastmsg)
        {
            try
            {
                //List<Hashtable> listresult;
                //IList listresult;
                int maxbcid;
                IList<MultiProperty> parLIST = new List<MultiProperty>();
                IList<MultiProperty> parLIST1 = new List<MultiProperty>();
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    if (userId > 0)
                    {
                        parLIST.Add(new MultiProperty { propertyName = "Userid", propertyValue = userId });
                        parLIST1.Add(new MultiProperty { propertyName = "Userid", propertyValue = userId });
                        parLIST.Add(new MultiProperty { propertyName = "Username", propertyValue = username });
                        parLIST.Add(new MultiProperty { propertyName = "Messages", propertyValue = broadcastmsg });
                        var query = "INSERT INTO [dbo].[CM_BroadcastMsg] ([Userid],[Username],[Messages]) VALUES  (:Userid ,:Username,:Messages) ";
                        tx.PersistenceManager.CommonRepository.ExecuteQuerywithParam(query.ToString(), parLIST);
                        // var query = "INSERT INTO PM_Objective_Unit select  MAX(ID)+1 , '" + caption + "'  from PM_Objective_Unit";
                        //var query = "INSERT INTO CM_BroadcastMsg select  isnull(MAX(id),0)+1 , :caption  from PM_Objective_Unit ";
                        //var query = "INSERT INTO CM_BroadcastMsg select :Userid,:Username,Messages ";

                        //var retrunquery = " select isnull(MAX(id),0) ID from CM_BroadcastMsg";
                        //var listresult = tx.PersistenceManager.CommonRepository.ExecuteQuery(retrunquery.ToString()).Cast<Hashtable>();
                        // int tempId = Convert.ToInt32(listresult);
                        //int taskListMaxCount = listresult.Count;
                        //for (int i = 0; i < taskListMaxCount; i++)
                        //{
                        //  maxbcid = Convert.ToInt32(listresult);                               
                        //}

                        //maxbcid = listresult[0][0];//Convert.ToInt32(listresult[0][0]);

                        StringBuilder strqry = new StringBuilder();
                        strqry.Append("INSERT INTO [dbo].[CM_Broadcast_userstrus]([Userid],[BroadcastID],Userstatus) ");
                        strqry.Append("SELECT uu.ID,(SELECT  isnull(MAX(id),0) FROM [CM_BroadcastMsg]) as bcid,0 FROM  UM_User uu WHERE uu.ID NOT IN(:Userid) ");
                        tx.PersistenceManager.CommonRepository.ExecuteQuerywithParam(strqry.ToString(), parLIST1);
                        tx.Commit();
                        return 1;

                    }

                    else
                    {
                        return 0;
                    }


                }

            }
            catch
            {
                return 0;
            }
            return 0;

        }
        public IList GetBroadcastMessages(CommonManagerProxy proxy)
        {
            //IList<MultiProperty> parLIST = new List<MultiProperty>();
            try
            {
                IList listresult;
                StringBuilder strqry = new StringBuilder();

                //parLIST.Add(new MultiProperty { propertyName = "Year", propertyValue = year });
                strqry.Append("SELECT  id, Username,convert(varchar, Createdate, 120) as Createdate,Messages FROM [CM_BroadcastMsg]  ORDER BY id desc ");

                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    listresult = tx.PersistenceManager.CommonRepository.ExecuteQuery(strqry.ToString());

                    tx.Commit();
                }

                return listresult;

            }
            catch (Exception)
            {

                return null;
            }

        }
        public IList GetBroadcastMessagesbyuser(CommonManagerProxy proxy)
        {
            IList<MultiProperty> parLIST = new List<MultiProperty>();
            try
            {
                IList listresult;
                StringBuilder strqry = new StringBuilder();

                parLIST.Add(new MultiProperty { propertyName = "Userid", propertyValue = proxy.MarcomManager.User.Id });

                //strqry.Append(" DECLARE @Names VARCHAR(8000) ");
                //strqry.Append("SELECT @Names = COALESCE(@Names + ', ', '') + Messages FROM CM_BroadcastMsg WHERE id IN(SELECT DISTINCT BroadcastID  FROM  [CM_Broadcast_userstrus] WHERE Userstatus =0 AND Userid=:Userid) ");
                //strqry.Append("SELECT @Names AS Messages");
                //strqry.Append("SELECT isnull(@Names,'-') AS Messages,:Userid AS Userid  ");
                //strqry.Append("SELECT Messages  FROM CM_BroadcastMsg WHERE id IN(SELECT DISTINCT BroadcastID  FROM  [CM_Broadcast_userstrus] WHERE Userstatus =0 AND Userid=:Userid)  ORDER BY id  desc ");
                strqry.Append("SELECT Messages,convert(varchar, Createdate, 120) as Createdate FROM CM_BroadcastMsg AS bm  ");
                strqry.Append(" JOIN  CM_Broadcast_userstrus bcu ON  bm.id=bcu.BroadcastID ");
                strqry.Append(" WHERE bcu.Userstatus=0 AND  bcu.Userid= :Userid ");
                strqry.Append(" ORDER BY bm.id  desc ");
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    listresult = tx.PersistenceManager.CommonRepository.ExecuteQuerywithParam(strqry.ToString(), parLIST);

                    tx.Commit();
                }

                return listresult;

            }
            catch (Exception)
            {

                return null;
            }

        }

        public int updateBroadcastMessagesbyuser(CommonManagerProxy proxy)
        {
            IList<MultiProperty> parLIST = new List<MultiProperty>();
            try
            {
                IList listresult;
                StringBuilder strqry = new StringBuilder();

                parLIST.Add(new MultiProperty { propertyName = "Userid", propertyValue = proxy.MarcomManager.User.Id });
                strqry.Append("UPDATE [CM_Broadcast_userstrus] SET Userstatus=1 WHERE userid=:Userid");
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    listresult = tx.PersistenceManager.CommonRepository.ExecuteQuerywithParam(strqry.ToString(), parLIST);

                    tx.Commit();
                    return 1;
                }



            }
            catch (Exception)
            {

                return 0;
            }

        }

        public IList<IBandwithData> GetbandwidthStatistic(CommonManagerProxy proxy, int year, int month)
        {
            IList<MultiProperty> parLIST = new List<MultiProperty>();
            IList<IBandwithData> objBandwithlist = new List<IBandwithData>();
            try
            {
                //IList listresult;
                StringBuilder strqry = new StringBuilder();
                parLIST.Add(new MultiProperty { propertyName = "Year", propertyValue = year });

                if (month > 0)
                {
                    parLIST.Add(new MultiProperty { propertyName = "Month", propertyValue = month });
                    strqry.Append(" SELECT DATEPART(DAY,LoginTime)AS Days,count(*) as value FROM UM_LoginDetail WHERE YEAR(logintime)= :Year   ");
                    strqry.Append(" AND MONTH(logintime)= :Month ");
                    strqry.Append(" GROUP BY DATEPART(DAY,LoginTime) ORDER BY DAYS ");
                }
                else
                {
                    strqry.Append(" SELECT DATEPART(MONTH,LoginTime)AS Days,count(*) as value FROM UM_LoginDetail WHERE YEAR(logintime)= :Year   ");
                    strqry.Append(" GROUP BY DATEPART(MONTH,LoginTime) ORDER BY DAYS  ");
                }

                //double UsedBWr = 0;
                //UsedBWr=ParseW3CLog("testit",14,05,19);

                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {

                    var listresult = tx.PersistenceManager.CommonRepository.ExecuteQuerywithParam(strqry.ToString(), parLIST).Cast<Hashtable>();
                    tx.Commit();
                    int j = 1;
                    int reseycount = listresult.Count();

                    foreach (var val in listresult)
                    {
                        BandwithData objBandwith = new BandwithData();
                        objBandwith.Id = j;
                        objBandwith.Days = Convert.ToInt32(val["Days"]);
                        BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("========================== Bandwith log file before calculate size =================================:", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);
                        objBandwith.Cvalue = ParseW3CLog(year, month, Convert.ToInt32(val["Days"]));
                        BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("========================== Bandwith log file after calculate size =================================:", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);

                        objBandwithlist.Add(objBandwith);
                        j++;
                    }


                }

                return objBandwithlist;

            }
            catch (Exception)
            {

                return null;
            }

        }

        public double ParseW3CLog(int year, int month, int days)
        {
            double UsedBW = 0;
            try
            {
                // prepare LogParser Recordset & Record objects
                string weblogpath = ConfigurationManager.AppSettings["weblogpath"];
                string FilterVirtualDir = ConfigurationManager.AppSettings["weblogFilterVirtualDir"];
                string weblogpatext = "u_ex";
                string mothvalue = "";
                string dayvalues = "";
                string yearvalue = year.ToString();
                if (month.ToString().Length > 1 && month > 0)
                { mothvalue = month.ToString(); }
                else if (month > 0)
                    mothvalue = "0" + month.ToString();
                if (days.ToString().Length > 1 && days > 0)
                { dayvalues = days.ToString(); }
                else if (days > 0)
                    dayvalues = "0" + days.ToString();

                string filepath = weblogpath + weblogpatext + yearvalue.Substring(yearvalue.Length - 2); ;
                if (mothvalue.Length > 0)
                    filepath = filepath + mothvalue;
                if (dayvalues.Length > 0 && mothvalue.Length > 0)
                {
                    filepath = filepath + dayvalues + ".log";
                }
                else if (dayvalues.Length > 0 && mothvalue.Length == 0)
                {
                    filepath = filepath + dayvalues + "*.log";
                }
                else
                {
                    filepath = filepath + "*.log";
                }

                ILogRecordset rsLP = null;
                ILogRecord rowLP = null;

                MSUtil.LogQueryClassClass LogParser = null;
                MSUtil.COMW3CInputContextClassClass W3Clog = null;


                int Unitsprocessed;
                double sizeInBytes;

                string strSQL = null;

                LogParser = new LogQueryClassClass();
                W3Clog = new COMW3CInputContextClassClass();
                LogParser.maxParseErrors = -1;

                //W3C Logparsing SQL. Replace this SQL query with whatever 
                //you want to retrieve. The example below 
                //will sum up all the bandwidth
                //Usage of a specific folder with name 
                //"userID". Download Log Parser 2.2 
                //from Microsoft and see sample queries.

                //strSQL = @"SELECT SUM(sc-bytes) from C:\\logs" +
                //         @"\\*.log WHERE cs-uri-stem LIKE '%/" +
                //         userID + "/%' ";
                BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("========================== Bandwith log file path =================================:" + filepath, BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);
                if (FilterVirtualDir.Length > 0 && FilterVirtualDir != null && FilterVirtualDir != "")
                {
                    strSQL = @"SELECT SUM(sc-bytes) from " + filepath + " WHERE cs-uri-stem LIKE '%/" + FilterVirtualDir + "/%' ";
                }
                else
                {
                    strSQL = @"SELECT SUM(sc-bytes) from " + filepath;
                }
                // run the query against W3C log
                rsLP = LogParser.Execute(strSQL, W3Clog);

                rowLP = rsLP.getRecord();

                Unitsprocessed = rsLP.inputUnitsProcessed;

                if (rowLP.getValue(0).ToString() == "0" ||
                    rowLP.getValue(0).ToString() == "")
                {
                    //Return 0 if an err occured
                    UsedBW = 0;
                    return UsedBW;
                }

                //Bytes to MB Conversion
                double Bytes = Convert.ToDouble(rowLP.getValue(0).ToString());
                UsedBW = Bytes / (1024 * 1024);

                //Round to 3 decimal places
                UsedBW = Math.Round(UsedBW, 0);
            }
            catch (Exception ex)
            {
                BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("error in Bandwith" + " " + ex.Message, BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);
                return 0;
            }

            return UsedBW;
        }
        public IList<IReportContainer> GetAPIEntityDetails(CommonManagerProxy proxy, bool IsshowFinancialDetl, bool IsDetailIncluded, bool IsshowTaskDetl, bool IsshowMemberDetl, int ExpandingEntityIDStr, bool IncludeChildrenStr, bool IsRootLevelEntity, int offsetstart = 0, int offsetend = 20)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    IList<IReportContainer> rptcollection = new List<IReportContainer>();
                    StringBuilder EntityFinQry = new StringBuilder();

                    int[] SelectedEntityIds = { };
                    var totalchildrenIDarr = new StringBuilder();
                    if (IsRootLevelEntity)
                    {
                        totalchildrenIDarr.Append(" SELECT  pe.ID as 'entityid' FROM PM_Entity pe INNER JOIN MM_EntityType met  ON pe.TypeID=met.ID AND met.IsAssociate=0 AND met.IsRootLevel=0 AND met.Category=2 AND pe.[Active]=1 AND met.id NOT IN(5,10) ORDER BY pe.UniqueKey offset " + offsetstart + " ROWS FETCH NEXT " + offsetend + " row ONLY ");
                        IList totalchildrenIDobj = tx.PersistenceManager.PlanningRepository.ExecuteQuery(totalchildrenIDarr.ToString());
                        int[] IdArr = totalchildrenIDobj.Cast<dynamic>().Select(a => (int)a["entityid"]).ToArray().Select(a => a).ToArray();

                        SelectedEntityIds = IdArr;
                    }
                    else
                    {

                        if (IncludeChildrenStr)
                        {
                            totalchildrenIDarr.Append(" SELECT pe.ID as 'entityid' FROM   PM_Entity pe INNER JOIN MM_EntityType met ON  pe.TypeID = met.id AND met.IsAssociate = 0 AND met.Category = 2 AND pe.[Active] = 1 where  pe.ID = " + ExpandingEntityIDStr + " or pe.UniqueKey LIKE  (SELECT pe1.UniqueKey FROM PM_Entity pe1 WHERE pe1.id=" + ExpandingEntityIDStr + ")+ '.%'  ORDER BY pe.UniqueKey asc ");
                            IList totalchildrenIDobj = tx.PersistenceManager.PlanningRepository.ExecuteQuery(totalchildrenIDarr.ToString());
                            int[] IdArr = totalchildrenIDobj.Cast<dynamic>().Select(a => (int)a["entityid"]).ToArray().Select(a => a).ToArray();

                            SelectedEntityIds = IdArr;

                        }
                        else
                        {
                            SelectedEntityIds = ExpandingEntityIDStr.ToString().Split(',').Select(a => Convert.ToInt32(a)).ToArray();
                        }
                    }



                    var BasicEntityData = (from tbl1 in tx.PersistenceManager.ReportRepository.Query<EntityDao>().Where(a => SelectedEntityIds.Contains(a.Id))
                                           join tbl2 in tx.PersistenceManager.ReportRepository.Query<EntityTypeDao>()
                                           on tbl1.Typeid equals tbl2.Id
                                           select new { Name = tbl1.Name, ShortDescription = tbl2.ShortDescription, ColorCode = tbl2.ColorCode, ID = tbl1.Id, TypeID = tbl1.Typeid, Level = tbl1.Level });
                    string InClause = String.Join(",", SelectedEntityIds.Select(a => a.ToString()).ToArray());

                    foreach (var currentval in BasicEntityData)
                    {
                        rptcollection.Add(new BrandSystems.Marcom.Core.Report.ReportContainer { ID = currentval.ID, TypeID = currentval.TypeID, Name = currentval.Name, ShortDescription = currentval.ShortDescription, ColorCode = currentval.ColorCode, Level = currentval.Level });
                    }

                    string xmlpath = tx.PersistenceManager.MetadataRepository.GetXmlPath(MarcomManagerFactory.ActiveMetadataVersionNumber);


                    int[] EntityTypeID = tx.PersistenceManager.MetadataRepository.Query<BaseEntityDao>().Where(a => SelectedEntityIds.Contains(a.Id)).Select(a => a.Typeid).ToArray();



                    String CollectionEntitypes = String.Join(",", EntityTypeID.Select(a => a.ToString()).ToArray());
                    if (IsDetailIncluded)
                    {

                        int[] BlockAttrIDs = { (int)SystemDefinedAttributes.Name };


                        foreach (var CurrentItem in EntityTypeID.Distinct())
                        {
                            Dictionary<string, string> attrColection = new Dictionary<string, string>();

                            EntityFinQry.Clear();
                            EntityFinQry.Append(" select ent.ID ");
                            string scourcetablename = "[MM_AttributeRecord_" + CurrentItem + "]";
                            var EntitypeAttributeCollection = from entityAttributeTbl in tx.PersistenceManager.MetadataRepository.GetObject<EntityTypeAttributeRelationDao>(xmlpath).Where(a => a.EntityTypeID == CurrentItem && !BlockAttrIDs.Contains(a.AttributeID))
                                                              join attributetbl in tx.PersistenceManager.MetadataRepository.GetObject<AttributeDao>(xmlpath)
                                                              on entityAttributeTbl.AttributeID equals attributetbl.Id
                                                              orderby entityAttributeTbl.SortOrder
                                                              select new { AttributeTypeID = attributetbl.AttributeTypeID, ColumnName = "attr_" + attributetbl.Id, AttributeId = attributetbl.Id, Caption = attributetbl.Caption, IsSpecial = attributetbl.IsSpecial };

                            attrColection.Add("ID", "ID");


                            foreach (var newColumnList in EntitypeAttributeCollection)
                            {
                                if (newColumnList.IsSpecial)
                                {
                                    if ((int)newColumnList.AttributeId == (int)SystemDefinedAttributes.Name)
                                    {
                                        //EntityFinQry.Append(",COALESCE(NULLIF(ent.name,''), '-') ");
                                        //EntityFinQry.Append(" AS  [");
                                        //EntityFinQry.Append(newColumnList.Caption);
                                        //EntityFinQry.Append("]");
                                    }
                                    else if (newColumnList.AttributeTypeID == (int)SystemDefinedAttributes.Owner)
                                    {
                                        EntityFinQry.Append(",(SELECT (ISNULL(us.FirstName,'') + ' ' + ISNULL(us.LastName,'')) AS VALUE  FROM UM_User us INNER JOIN AM_Entity_Role_User aeru ON us.ID=aeru.UserID AND  aeru.RoleID=1  ");
                                        EntityFinQry.Append(" and  aeru.EntityID = ent.ID   ) ");
                                        EntityFinQry.Append(" AS  [");
                                        EntityFinQry.Append(newColumnList.Caption);
                                        EntityFinQry.Append("]");
                                        attrColection.Add(newColumnList.AttributeId.ToString(), newColumnList.Caption);

                                    }
                                }
                                else if (newColumnList.AttributeTypeID == (int)AttributesList.TextSingleLine || newColumnList.AttributeTypeID == (int)AttributesList.TextMultiLine || newColumnList.AttributeTypeID == (int)AttributesList.TextMoney)
                                {
                                    EntityFinQry.Append(",COALESCE(NULLIF(" + newColumnList.ColumnName + ",''), '-') ");
                                    EntityFinQry.Append(" AS  [");
                                    EntityFinQry.Append(newColumnList.Caption);
                                    EntityFinQry.Append("]");
                                    attrColection.Add(newColumnList.AttributeId.ToString(), newColumnList.Caption);
                                }
                                else if (newColumnList.AttributeTypeID == (int)AttributesList.DateTime)
                                {
                                    EntityFinQry.Append(",REPLACE( CONVERT(varchar, ISNULL(" + newColumnList.ColumnName + ",''),121),'1900-01-01 00:00:00.000','-') ");   //AS [Date  time],
                                    EntityFinQry.Append(" AS  [");
                                    EntityFinQry.Append(newColumnList.Caption);
                                    EntityFinQry.Append("]");
                                    attrColection.Add(newColumnList.AttributeId.ToString(), newColumnList.Caption);

                                }
                                else if (newColumnList.AttributeTypeID == (int)AttributesList.ListSingleSelection)
                                {
                                    EntityFinQry.Append(",ISNULL( (select top 1 Caption from  mm_option where ID IN(SELECT " + newColumnList.ColumnName + " FROM " + scourcetablename + "  where ID=Att.ID)),'-') AS  [" + newColumnList.Caption + "] ");
                                    attrColection.Add(newColumnList.AttributeId.ToString(), newColumnList.Caption);
                                }
                                else if (newColumnList.AttributeTypeID == (int)AttributesList.ListMultiSelection)
                                {
                                    EntityFinQry.Append(",ISNULL( (select distinct (SELECT ");
                                    EntityFinQry.Append("STUFF( ");
                                    EntityFinQry.Append("( ");
                                    EntityFinQry.Append("SELECT ',' +  mo.Caption ");
                                    EntityFinQry.Append("FROM   MM_MultiSelect mms2 ");
                                    EntityFinQry.Append("INNER JOIN MM_Option mo ");
                                    EntityFinQry.Append("ON  mms2.OptionID = mo.ID ");
                                    EntityFinQry.Append("WHERE  mms2.EntityID = mms.EntityID AND mms2.AttributeID=mms.AttributeID ");
                                    EntityFinQry.Append("FOR XML PATH('')  ");
                                    EntityFinQry.Append("),1,1,''  ");
                                    EntityFinQry.Append(") AS VALUE ");
                                    EntityFinQry.Append("FROM   MM_MultiSelect mms ");
                                    EntityFinQry.Append("WHERE  mms.EntityID=Att.ID and  mms.AttributeID = " + newColumnList.AttributeId + "  ");
                                    EntityFinQry.Append("GROUP BY  ");
                                    EntityFinQry.Append("mms.EntityID,mms.AttributeID) ),'-')   as [" + newColumnList.Caption + "]");
                                    attrColection.Add(newColumnList.AttributeId.ToString(), newColumnList.Caption);

                                }
                                else if (newColumnList.AttributeTypeID == (int)AttributesList.DropDownTree)
                                {
                                    var TreeLeveldao = tx.PersistenceManager.MetadataRepository.GetObject<TreeLevelDao>(xmlpath);
                                    var Treelist = TreeLeveldao.Where(a => a.AttributeID == newColumnList.AttributeId);
                                    foreach (var treecount in Treelist)
                                    {
                                        EntityFinQry.Append(",ISNULL( (SELECT mtn.Caption ");
                                        EntityFinQry.Append("FROM   MM_TreeNode mtn ");
                                        EntityFinQry.Append("INNER JOIN MM_TreeValue mtv ");
                                        EntityFinQry.Append("ON  mtv.NodeID = mtn.ID ");
                                        EntityFinQry.Append("AND mtv.AttributeID = mtn.AttributeID ");
                                        EntityFinQry.Append("WHERE  mtv.AttributeID =" + newColumnList.AttributeId + " ");
                                        EntityFinQry.Append("AND EntityID= Att.ID ");
                                        EntityFinQry.Append("AND mtv.[LEVEL]=" + treecount.Level + "),'-')  AS [" + newColumnList.Caption + "] ");
                                        attrColection.Add(newColumnList.AttributeId + "_" + treecount.Level, treecount.LevelName);
                                    }


                                }
                                else if (newColumnList.AttributeTypeID == (int)AttributesList.TreeMultiSelection)
                                {
                                    var TreeLeveldao = tx.PersistenceManager.MetadataRepository.GetObject<TreeLevelDao>(xmlpath);
                                    var Treelist = TreeLeveldao.Where(a => a.AttributeID == newColumnList.AttributeId);

                                    foreach (var treecount in Treelist)
                                    {
                                        if (Treelist.Count() != treecount.Level)
                                        {
                                            EntityFinQry.Append(",ISNULL( (SELECT mtn.Caption ");
                                            EntityFinQry.Append("FROM   MM_TreeNode mtn ");
                                            EntityFinQry.Append("INNER JOIN MM_TreeValue mtv ");
                                            EntityFinQry.Append("ON  mtv.NodeID = mtn.ID ");
                                            EntityFinQry.Append("AND mtv.AttributeID = mtn.AttributeID ");
                                            EntityFinQry.Append("WHERE  mtv.AttributeID =" + newColumnList.AttributeId + " ");
                                            EntityFinQry.Append("AND EntityID= Att.ID ");
                                            EntityFinQry.Append("AND mtv.[LEVEL]=" + treecount.Level + "),'-')  AS [" + newColumnList.Caption + "] ");
                                            attrColection.Add(newColumnList.AttributeId + "_" + treecount.Level, treecount.LevelName);
                                        }
                                        else
                                        {
                                            EntityFinQry.Append(",ISNULL( (SELECT  ");
                                            EntityFinQry.Append("STUFF( ");
                                            EntityFinQry.Append("( ");
                                            EntityFinQry.Append("SELECT ', ' +  mtn.Caption ");
                                            EntityFinQry.Append("FROM   MM_TreeNode mtn ");
                                            EntityFinQry.Append("INNER JOIN MM_TreeValue mtv ");
                                            EntityFinQry.Append("ON  mtv.NodeID = mtn.ID and  mtv.AttributeID=" + newColumnList.AttributeId + " ");
                                            EntityFinQry.Append("AND mtn.Level = " + treecount.Level + " WHERE mtv.EntityID = Att.ID AND mtv.AttributeID =" + newColumnList.AttributeId + " ");
                                            EntityFinQry.Append("FOR XML PATH('') ");
                                            EntityFinQry.Append("), ");
                                            EntityFinQry.Append("1, ");
                                            EntityFinQry.Append("2, ");
                                            EntityFinQry.Append("'' ");
                                            EntityFinQry.Append(") ),'-') AS [" + newColumnList.Caption + "] ");
                                            attrColection.Add(newColumnList.AttributeId + "_" + treecount.Level, treecount.LevelName);
                                        }
                                    }
                                }
                                else if (newColumnList.AttributeTypeID == (int)AttributesList.CheckBoxSelection)
                                {
                                    EntityFinQry.Append(",CASE when " + newColumnList.ColumnName + " = 1 THEN 'True' ");
                                    EntityFinQry.Append(" when " + newColumnList.ColumnName + " = 0 THEN 'False' ELSE '-' END ");
                                    EntityFinQry.Append(" AS  [");
                                    EntityFinQry.Append(newColumnList.Caption);
                                    EntityFinQry.Append("] ");
                                    attrColection.Add(newColumnList.AttributeId.ToString(), newColumnList.Caption);
                                }
                                else if (newColumnList.AttributeTypeID == (int)AttributesList.Period)
                                {
                                    EntityFinQry.Append(",( SELECT REPLACE(( SELECT ISNULL((SELECT MIN(ISNULL(Startdate, '-'))");
                                    EntityFinQry.Append("FROM   PM_EntityPeriod ");
                                    EntityFinQry.Append(" WHERE  EntityID = Att.ID),'')),'1900-01-01','-') ) as Startdate ");
                                    attrColection.Add("Startdate", "Startdate");

                                    EntityFinQry.Append(",( SELECT REPLACE(( SELECT ISNULL((SELECT MAX(ISNULL(EndDate, '-'))");
                                    EntityFinQry.Append("FROM   PM_EntityPeriod ");
                                    EntityFinQry.Append(" WHERE  EntityID = Att.ID),'')),'1900-01-01','-') ) as EndDate ");
                                    attrColection.Add("EndDate", "EndDate");
                                }

                            }

                            EntityFinQry.Append(" from ");
                            EntityFinQry.Append(scourcetablename);
                            EntityFinQry.Append(" AS Att INNER JOIN PM_Entity AS  ent ON ent.Id= Att.ID and  att.id in(" + InClause + ") ");
                            var MetadataResult = tx.PersistenceManager.ReportRepository.ExecuteQuery(EntityFinQry.ToString()).Cast<Hashtable>();

                            foreach (var CurrentMetadata in MetadataResult)
                            {
                                int currentIndex = rptcollection.Select(a => a.ID).ToList().IndexOf(Convert.ToInt32(CurrentMetadata["ID"]));
                                CurrentMetadata.Remove("ID");
                                if (currentIndex != -1)
                                {
                                    rptcollection[currentIndex].MetadataCollections = CurrentMetadata;
                                    //rptcollection[currentIndex].MetadataColumnCollection = attrColection;
                                }
                            }
                        }



                    }

                    if (IsshowMemberDetl)
                    {
                        EntityFinQry.Clear();

                        EntityFinQry.Append("  SELECT aeru.EntityID as 'ID',ISNULL(ar.Caption,'-') as 'Caption',(uu.FirstName +' ' +uu.LastName) AS 'Name' FROM AM_Entity_Role_User aeru INNER JOIN AM_Role ar ON aeru.RoleID=ar.ID   ");
                        EntityFinQry.Append("   AND aeru.EntityID IN(" + InClause + ")   ");
                        EntityFinQry.Append("   INNER JOIN UM_User uu ON aeru.UserID=uu.ID GROUP BY aeru.EntityID,ar.Caption,(uu.FirstName +' ' +uu.LastName)   ");



                        List<Hashtable> MemberResult = tx.PersistenceManager.ReportRepository.ReportExecuteQuery(EntityFinQry.ToString());

                        int currentMemberOldId = 0;
                        int currentMemberdata = -1;
                        List<Hashtable> objMemberCollection = new List<Hashtable>();
                        //for (int i = MemberResult.Count - 1; i >= 0; i--)
                        //{
                        int memberListMaxCount = MemberResult.Count;
                        for (int i = 0; i < memberListMaxCount; i++)
                        {
                            currentMemberOldId = Convert.ToInt32(MemberResult[i]["ID"]);
                            currentMemberdata = rptcollection.Select(a => a.ID).ToList().IndexOf(Convert.ToInt32(MemberResult[i]["ID"]));
                            if (currentMemberdata != -1)
                            {
                                if (i != memberListMaxCount - 1)
                                {
                                    if (currentMemberOldId != Convert.ToInt32(MemberResult[i + 1]["ID"]))
                                    {

                                        if (objMemberCollection != null & objMemberCollection.Count > 0)
                                        {
                                            MemberResult[i].Remove("ID");
                                            objMemberCollection.Add(MemberResult[i]);

                                        }
                                        else
                                        {
                                            objMemberCollection = null;
                                            objMemberCollection = new List<Hashtable>();
                                            MemberResult[i].Remove("ID");
                                            objMemberCollection.Add(MemberResult[i]);
                                        }
                                        rptcollection[currentMemberdata].MemberCollections = objMemberCollection;
                                        objMemberCollection = null;
                                        objMemberCollection = new List<Hashtable>();

                                    }
                                    else
                                    {
                                        MemberResult[i].Remove("ID");
                                        objMemberCollection.Add(MemberResult[i]);
                                    }
                                }
                                else
                                {
                                    MemberResult[i].Remove("ID");
                                    objMemberCollection.Add(MemberResult[i]);
                                    rptcollection[currentMemberdata].MemberCollections = objMemberCollection;
                                    objMemberCollection = null;
                                }
                            }

                            //MemberResult.RemoveAt(i);
                        }
                    }


                    if (IsshowFinancialDetl)
                    {

                        EntityFinQry.Clear();



                        EntityFinQry.Append("  SELECT fin.EntityID  AS ID,fin.CostCenterID as CostCenterID,'/api/Metadata/GetEntityByID/' + cast(fin.CostCenterID as varchar(10)) as [Href], ");
                        EntityFinQry.Append("         ISNULL(SUM(fin.TotalPlannedAmount), 0) AS Planned, ");
                        EntityFinQry.Append("         ISNULL(SUM(fin.TotalRequested), 0) AS Requested, ");
                        EntityFinQry.Append("         ISNULL(SUM(fin.TotalApprovedAmount), 0) AS ApprovedAllocation, ");
                        EntityFinQry.Append("         ISNULL(SUM(fin.ApprovedBudget), 0) AS ApprovedBudget, ");
                        EntityFinQry.Append("         ISNULL(SUM(fin.BudgetDeviation), 0) AS BudgetDeviation, ");
                        EntityFinQry.Append("         ISNULL(SUM(fin.TotalCommitedAmount), 0) AS Commited, ");
                        EntityFinQry.Append("         ISNULL(SUM(fin.TotalSpentAmount), 0) AS Spent, ");
                        EntityFinQry.Append("         ISNULL( ");
                        EntityFinQry.Append("             SUM(fin.TotalApprovedAmount) - SUM(fin.TotalSpentAmount), ");
                        EntityFinQry.Append("             0 ");
                        EntityFinQry.Append("         )             AS AvailableToSpend, ");
                        EntityFinQry.Append("         ( ");
                        EntityFinQry.Append("             SELECT TOP 1         NAME ");
                        EntityFinQry.Append("             FROM   PM_Entity     pe ");
                        EntityFinQry.Append("             WHERE  id = fin.CostCenterID ");
                        EntityFinQry.Append("         )             AS Name ");
                        EntityFinQry.Append("  FROM   ( ");
                        EntityFinQry.Append("             SELECT pefav.EntityID, ");
                        EntityFinQry.Append("                    pefav.CostCenterID, ");
                        EntityFinQry.Append("                    CASE  ");
                        EntityFinQry.Append("                         WHEN pe.level = 0 THEN ( ");
                        EntityFinQry.Append("                                  SELECT SUM(pf.PlannedAmount) ");
                        EntityFinQry.Append("                                  FROM   PM_Financial pf ");
                        EntityFinQry.Append("                                         INNER JOIN PM_Entity pe2 ");
                        EntityFinQry.Append("                                              ON  pe2.ID = pf.EntityID ");
                        EntityFinQry.Append("                                              AND pe2.UniqueKey LIKE pe.UniqueKey  ");
                        EntityFinQry.Append("                                                  + '.%' ");
                        EntityFinQry.Append("                                              AND pe2.[Level] = 1 ");
                        //EntityFinQry.Append("                                              AND pe2.TypeId IN (" + CollectionEntitypes + ") ");
                        EntityFinQry.Append("                                              AND pe2.[Active] = 1 ");
                        EntityFinQry.Append("                                              AND pf.CostCenterID = pefav.CostCenterID ");
                        EntityFinQry.Append("                              ) ");
                        EntityFinQry.Append("                         ELSE pefav.PlannedAmount ");
                        EntityFinQry.Append("                    END  AS TotalPlannedAmount, ");
                        EntityFinQry.Append("                    CASE  ");
                        EntityFinQry.Append("                         WHEN pe.level = 0 THEN ( ");
                        EntityFinQry.Append("                                  SELECT SUM(pf.RequestedAmount) ");
                        EntityFinQry.Append("                                  FROM   PM_Financial pf ");
                        EntityFinQry.Append("                                         INNER JOIN PM_Entity pe2 ");
                        EntityFinQry.Append("                                              ON  pe2.ID = pf.EntityID ");
                        EntityFinQry.Append("                                              AND pe2.UniqueKey LIKE pe.UniqueKey  ");
                        EntityFinQry.Append("                                                  + '.%' ");
                        EntityFinQry.Append("                                              AND pe2.[Level] = 1 ");
                        EntityFinQry.Append("                                              AND pe2.TypeId IN (" + CollectionEntitypes + ") ");
                        EntityFinQry.Append("                                              AND pe2.[Active] = 1 ");
                        EntityFinQry.Append("                                              AND pf.CostCenterID = pefav.CostCenterID ");
                        EntityFinQry.Append("                              ) ");
                        EntityFinQry.Append("                         ELSE pefav.RequestedAmount ");
                        EntityFinQry.Append("                    END  AS TotalRequested, ");
                        EntityFinQry.Append("                    CASE  ");
                        EntityFinQry.Append("                         WHEN pe.level = 0 THEN ( ");
                        EntityFinQry.Append("                                  SELECT SUM(pf.ApprovedAllocatedAmount) ");
                        EntityFinQry.Append("                                  FROM   PM_Financial pf ");
                        EntityFinQry.Append("                                         INNER JOIN PM_Entity pe2 ");
                        EntityFinQry.Append("                                              ON  pe2.ID = pf.EntityID ");
                        EntityFinQry.Append("                                              AND pe2.UniqueKey LIKE pe.UniqueKey  ");
                        EntityFinQry.Append("                                                  + '.%' ");
                        EntityFinQry.Append("                                              AND pe2.[Level] = 1 ");
                        //EntityFinQry.Append("                                              AND pe2.TypeId IN (" + CollectionEntitypes + ") ");
                        EntityFinQry.Append("                                              AND pe2.[Active] = 1 ");
                        EntityFinQry.Append("                                              AND pf.CostCenterID = pefav.CostCenterID ");
                        EntityFinQry.Append("                              ) ");
                        EntityFinQry.Append("                         ELSE pefav.ApprovedAllocatedAmount ");
                        EntityFinQry.Append("                    END  AS TotalApprovedAmount, ");
                        EntityFinQry.Append("                    ISNULL( ");
                        EntityFinQry.Append("                        ( ");
                        EntityFinQry.Append("                            SELECT SUM(pefav2.Spent) AS Spent ");
                        EntityFinQry.Append("                            FROM   PM_Financial pefav2 ");
                        EntityFinQry.Append("                                   INNER JOIN PM_Entity pe2 ");
                        EntityFinQry.Append("                                        ON  pe2.ID = pefav2.EntityID ");
                        EntityFinQry.Append("                                        AND pe2.[Active] = 1 ");
                        EntityFinQry.Append("                                        AND pe2.TypeId IN (" + CollectionEntitypes + ") ");
                        EntityFinQry.Append("                                        AND pefav2.CostCenterID = pefav.CostCenterID ");
                        EntityFinQry.Append("                            WHERE  pe2.UniqueKey LIKE pe.UniqueKey + '%' ");
                        EntityFinQry.Append("                        ), ");
                        EntityFinQry.Append("                        0 ");
                        EntityFinQry.Append("                    )    AS TotalSpentAmount, ");
                        EntityFinQry.Append("                    ISNULL( ");
                        EntityFinQry.Append("                        ( ");
                        EntityFinQry.Append("                            SELECT SUM(pefav2.Commited) AS Commited ");
                        EntityFinQry.Append("                            FROM   PM_Financial pefav2 ");
                        EntityFinQry.Append("                                   INNER JOIN PM_Entity pe2 ");
                        EntityFinQry.Append("                                        ON  pe2.ID = pefav2.EntityID ");
                        EntityFinQry.Append("                                        AND pe2.[Active] = 1 ");
                        EntityFinQry.Append("                                        AND pe2.TypeId IN (" + CollectionEntitypes + ") ");
                        EntityFinQry.Append("                                        AND pefav2.CostCenterID = pefav.CostCenterID ");
                        EntityFinQry.Append("                            WHERE  pe2.UniqueKey LIKE pe.UniqueKey + '%' ");
                        EntityFinQry.Append("                        ), ");
                        EntityFinQry.Append("                        0 ");
                        EntityFinQry.Append("                    )    AS TotalCommitedAmount, ");
                        EntityFinQry.Append("                    CASE  ");
                        EntityFinQry.Append("                         WHEN pe.level = 0 THEN ( ");
                        EntityFinQry.Append("                                  SELECT SUM(pf.ApprovedBudget) ");
                        EntityFinQry.Append("                                  FROM   PM_Financial pf ");
                        EntityFinQry.Append("                                         INNER JOIN PM_Entity pe2 ");
                        EntityFinQry.Append("                                              ON  pe2.ID = pf.EntityID ");
                        EntityFinQry.Append("                                              AND pe2.UniqueKey LIKE pe.UniqueKey  ");
                        EntityFinQry.Append("                                                  + '.%' ");
                        EntityFinQry.Append("                                              AND pe2.[Level] = 1 ");
                        //EntityFinQry.Append("                                              AND pe2.TypeId IN (" + CollectionEntitypes + ") ");
                        EntityFinQry.Append("                                              AND pe2.[Active] = 1 ");
                        EntityFinQry.Append("                                              AND pf.CostCenterID = pefav.CostCenterID ");
                        EntityFinQry.Append("                              ) ");
                        EntityFinQry.Append("                         ELSE pefav.ApprovedBudget ");
                        EntityFinQry.Append("                    END  AS ApprovedBudget, ");
                        EntityFinQry.Append("                    CASE  ");
                        EntityFinQry.Append("                         WHEN pe.level = 0 THEN ( ");
                        EntityFinQry.Append("                                  SELECT SUM( ");
                        EntityFinQry.Append("                                             CASE  ");
                        EntityFinQry.Append("                                                  WHEN pf.ApprovedBudgetDate IS  ");
                        EntityFinQry.Append("                                                       NULL THEN 0 ");
                        EntityFinQry.Append("                                                  WHEN (pf.ApprovedBudget - pf.ApprovedAllocatedAmount)  ");
                        EntityFinQry.Append("                                                       < 0 THEN 0 ");
                        EntityFinQry.Append("                                                  ELSE pf.ApprovedBudget - pf.ApprovedAllocatedAmount ");
                        EntityFinQry.Append("                                             END ");
                        EntityFinQry.Append("                                         ) ");
                        EntityFinQry.Append("                                  FROM   PM_Financial pf ");
                        EntityFinQry.Append("                                         INNER JOIN PM_Entity pe2 ");
                        EntityFinQry.Append("                                              ON  pe2.ID = pf.EntityID ");
                        EntityFinQry.Append("                                              AND pe2.UniqueKey LIKE pe.UniqueKey  ");
                        EntityFinQry.Append("                                                  + '.%' ");
                        EntityFinQry.Append("                                              AND pe2.[Level] = 1 ");
                        EntityFinQry.Append("                                              AND pe2.TypeId IN (" + CollectionEntitypes + ") ");
                        EntityFinQry.Append("                                              AND pe2.[Active] = 1 ");
                        EntityFinQry.Append("                                              AND pf.CostCenterID = pefav.CostCenterID ");
                        EntityFinQry.Append("                              ) ");
                        EntityFinQry.Append("                         ELSE CASE  ");
                        EntityFinQry.Append("                                   WHEN pefav.ApprovedBudgetDate IS NULL THEN 0 ");
                        EntityFinQry.Append("                                   WHEN (pefav.ApprovedBudget - pefav.ApprovedAllocatedAmount)  ");
                        EntityFinQry.Append("                                        < 0 THEN 0 ");
                        EntityFinQry.Append("                                   ELSE pefav.ApprovedBudget - pefav.ApprovedAllocatedAmount ");
                        EntityFinQry.Append("                              END ");
                        EntityFinQry.Append("                    END  AS BudgetDeviation ");
                        EntityFinQry.Append("             FROM   PM_Financial pefav ");
                        EntityFinQry.Append("                    INNER JOIN PM_Entity pe ");
                        EntityFinQry.Append("                         ON  pe.ID = pefav.EntityID ");
                        EntityFinQry.Append("                         AND pe.[active] = 1 ");
                        EntityFinQry.Append("                         AND pe.id IN (" + InClause + ") ");
                        EntityFinQry.Append("         )             AS fin ");
                        EntityFinQry.Append("  GROUP BY ");
                        EntityFinQry.Append("         fin.entityid, ");
                        EntityFinQry.Append("         fin.CostCenterID ");
                        EntityFinQry.Append("  ORDER BY ");
                        EntityFinQry.Append("         fin.EntityID  ");


                        List<Hashtable> FinancialResult = tx.PersistenceManager.ReportRepository.ReportExecuteQuery(EntityFinQry.ToString());

                        int currentFinanceOldId = 0;
                        int currentFinancedata = -1;
                        List<Hashtable> objFinanceCollection = new List<Hashtable>();
                        //for (int i = FinancialResult.Count - 1; i >= 0; i--)
                        //{
                        int financialListMaxCount = FinancialResult.Count;
                        for (int i = 0; i < financialListMaxCount; i++)
                        {
                            currentFinanceOldId = Convert.ToInt32(FinancialResult[i]["ID"]);
                            currentFinancedata = rptcollection.Select(a => a.ID).ToList().IndexOf(Convert.ToInt32(FinancialResult[i]["ID"]));
                            if (currentFinancedata != -1)
                            {
                                if (i != financialListMaxCount - 1)
                                {
                                    if (currentFinanceOldId != Convert.ToInt32(FinancialResult[i + 1]["ID"]))
                                    {
                                        if (objFinanceCollection != null & objFinanceCollection.Count > 0)
                                        {
                                            FinancialResult[i].Remove("ID");
                                            objFinanceCollection.Add(FinancialResult[i]);

                                        }
                                        else
                                        {
                                            objFinanceCollection = null;
                                            objFinanceCollection = new List<Hashtable>();
                                            FinancialResult[i].Remove("ID");
                                            objFinanceCollection.Add(FinancialResult[i]);
                                        }
                                        rptcollection[currentFinancedata].CostcentreCollections = objFinanceCollection;
                                        objFinanceCollection = null;
                                        objFinanceCollection = new List<Hashtable>();

                                    }
                                    else
                                    {
                                        FinancialResult[i].Remove("ID");
                                        objFinanceCollection.Add(FinancialResult[i]);
                                    }
                                }
                                else
                                {
                                    FinancialResult[i].Remove("ID");
                                    objFinanceCollection.Add(FinancialResult[i]);
                                    rptcollection[currentFinancedata].CostcentreCollections = objFinanceCollection;
                                    objFinanceCollection = null;
                                }
                            }

                            //FinancialResult.RemoveAt(i);
                        }

                    }

                    if (IsshowTaskDetl)
                    {
                        EntityFinQry.Clear();

                        EntityFinQry.Append("  SELECT tetl.EntityID AS ID, ISNULL(tetl.Name, '-') AS Name, ISNULL(tetl.ID, 0)  AS TaskListID ");
                        EntityFinQry.Append("  FROM   TM_EntityTaskList tetl ");
                        EntityFinQry.Append("  WHERE  tetl.EntityID  IN (" + InClause + ") ORDER BY tetl.EntityID, tetl.Sortorder");


                        List<Hashtable> TaskListResult = tx.PersistenceManager.ReportRepository.ReportExecuteQuery(EntityFinQry.ToString());

                        int currentTaskListOldId = 0;
                        int currentTaskListdata = -1;
                        List<Hashtable> objTaskListCollection = new List<Hashtable>();
                        //for (int i = TaskListResult.Count - 1; i >= 0; i--)
                        //{
                        int taskListMaxCount = TaskListResult.Count;
                        for (int i = 0; i < taskListMaxCount; i++)
                        {
                            currentTaskListOldId = Convert.ToInt32(TaskListResult[i]["ID"]);
                            currentTaskListdata = rptcollection.Select(a => a.ID).ToList().IndexOf(Convert.ToInt32(TaskListResult[i]["ID"]));
                            if (currentTaskListdata != -1)
                            {
                                if (i != taskListMaxCount - 1)
                                {
                                    if (currentTaskListOldId != Convert.ToInt32(TaskListResult[i + 1]["ID"]))
                                    {

                                        if (objTaskListCollection != null & objTaskListCollection.Count > 0)
                                        {
                                            TaskListResult[i].Remove("ID");
                                            objTaskListCollection.Add(TaskListResult[i]);
                                        }
                                        else
                                        {
                                            TaskListResult[i].Remove("ID");
                                            objTaskListCollection = null;
                                            objTaskListCollection = new List<Hashtable>();
                                            objTaskListCollection.Add(TaskListResult[i]);
                                        }
                                        rptcollection[currentTaskListdata].TaskListCollections = objTaskListCollection;
                                        objTaskListCollection = null;
                                        objTaskListCollection = new List<Hashtable>();
                                    }
                                    else
                                    {
                                        TaskListResult[i].Remove("ID");
                                        objTaskListCollection.Add(TaskListResult[i]);
                                    }
                                }
                                else
                                {
                                    TaskListResult[i].Remove("ID");
                                    objTaskListCollection.Add(TaskListResult[i]);
                                    rptcollection[currentTaskListdata].TaskListCollections = objTaskListCollection;
                                    objTaskListCollection = null;
                                }
                            }
                            //TaskListResult.RemoveAt(i);
                        }



                        EntityFinQry.Clear();


                        EntityFinQry.Append(" SELECT tet.EntityID AS 'ID', ISNULL(tet.TaskListID, 0)  AS 'TaskListID', ISNULL(tet.Name, '-') AS 'TaskName', ");
                        EntityFinQry.Append(" ISNULL(STUFF((SELECT ', ' + (uu.FirstName + ' ' + uu.LastName) FROM   TM_Task_Members ttm INNER JOIN UM_User uu ON  ttm.UserID = uu.ID AND ttm.RoleID = 4 WHERE  ttm.TaskID = tet.ID FOR XML PATH('') ),1,2,''),'') AS 'UserName', ");
                        EntityFinQry.Append("  CASE WHEN tet.DueDate IS NULL THEN '' ELSE CASE  WHEN tet.TaskStatus = 1 OR tet.TaskStatus = 0 THEN CONVERT(VARCHAR(10), tet.DueDate, 20)  ");
                        EntityFinQry.Append("   + ' (' + CASE when CAST(DATEDIFF(dd, GETDATE(), tet.DueDate) AS NVARCHAR(10))=0 THEN 'Today)' ELSE CAST(DATEDIFF(dd, GETDATE(), tet.DueDate) AS NVARCHAR(10)) + ' days)' end ");
                        EntityFinQry.Append("  ELSE '' ");
                        EntityFinQry.Append("  END ");
                        EntityFinQry.Append("  END                        AS 'DueDate', ");
                        //EntityFinQry.Append(" CASE WHEN tet.DueDate IS NULL THEN '' ELSE CASE WHEN tet.TaskStatus = 1 OR tet.TaskStatus = 0 THEN CONVERT(VARCHAR(10), tet.DueDate, 20) + ' (' + CAST(DATEDIFF(dd, GETDATE(), tet.DueDate) AS NVARCHAR(10))+ ' days)' ELSE '' END END AS 'DueDate', ");
                        EntityFinQry.Append(" CASE WHEN (ISNULL(tet.TaskStatus, 0) = 0) THEN 'Unassigned' WHEN (ISNULL(tet.TaskStatus, 0) = 1) THEN 'In progress' WHEN (ISNULL(tet.TaskStatus, 0) = 2) THEN 'Completed' WHEN (ISNULL(tet.TaskStatus, 0) = 3) THEN 'Approved' ");
                        EntityFinQry.Append(" WHEN (ISNULL(tet.TaskStatus, 0) = 4) THEN 'Unable to complete' WHEN (ISNULL(tet.TaskStatus, 0) = 5 OR ISNULL(tet.TaskStatus, 0) = 6 ) THEN 'Rejected' WHEN (ISNULL(tet.TaskStatus, 0) = 7) THEN 'Not applicable' ");
                        EntityFinQry.Append(" WHEN (ISNULL(tet.TaskStatus, 0) = 8) THEN 'Completed' END + CASE WHEN tet.TaskType = 2 AND tet.TaskStatus IN (0, 1) THEN CASE WHEN ( SELECT COUNT(1) FROM TM_EntityTaskCheckList tetcl WHERE tetcl.TaskId = tet.ID ) > 0 THEN ' (' + ");
                        EntityFinQry.Append(" CAST((SELECT COUNT(1) FROM TM_EntityTaskCheckList tetcl WHERE tetcl.TaskId = tet.ID AND tetcl.[Status] = 1 ) AS NVARCHAR(10) ) + '/' + CAST((SELECT COUNT(1) FROM TM_EntityTaskCheckList tetcl WHERE tetcl.TaskId = tet.ID ) AS NVARCHAR(10) ) + ')' ");
                        EntityFinQry.Append(" ELSE '' END WHEN tet.TaskType IN (3, 31) AND tet.TaskStatus = 1 THEN CASE WHEN ( SELECT COUNT(1) FROM TM_Task_Members ttm WHERE ttm.TaskID = tet.ID AND ttm.RoleID = 4 ) > 0 THEN  ' (' + CAST( ( SELECT COUNT(1) FROM TM_Task_Members ttm ");
                        EntityFinQry.Append(" WHERE ttm.TaskID = tet.ID AND ttm.RoleID = 4 AND ttm.ApprovalStatus IS NOT NULL ) AS NVARCHAR(10) ) + '/' + CAST( (SELECT COUNT(1) FROM TM_Task_Members ttm ");
                        EntityFinQry.Append(" WHERE ttm.TaskID = tet.ID AND ttm.RoleID = 4 ) AS NVARCHAR(10) ) + ')' ELSE '' END ELSE '' END AS 'Status' FROM TM_EntityTask tet INNER JOIN TM_EntityTaskList tetl ON tet.TaskListID = tetl.ID WHERE  tet.EntityID IN (" + InClause + ") ORDER BY tet.EntityID, tetl.Sortorder, tet.Sortorder ");


                        List<Hashtable> TaskDetlResult = tx.PersistenceManager.ReportRepository.ReportExecuteQuery(EntityFinQry.ToString());

                        int currentTaskOldId = 0;
                        int currentTaskdata = -1;
                        List<Hashtable> objTaskCollection = new List<Hashtable>();
                        //for (int i = TaskDetlResult.Count - 1; i >= 0; i--)
                        //{
                        int taskMaxCount = TaskDetlResult.Count;
                        for (int i = 0; i < taskMaxCount; i++)
                        {
                            currentTaskOldId = Convert.ToInt32(TaskDetlResult[i]["ID"]);
                            currentTaskdata = rptcollection.Select(a => a.ID).ToList().IndexOf(Convert.ToInt32(TaskDetlResult[i]["ID"]));
                            if (currentTaskdata != -1)
                            {
                                if (i != taskMaxCount - 1)
                                {
                                    if (currentTaskOldId != Convert.ToInt32(TaskDetlResult[i + 1]["ID"]))
                                    {
                                        if (objTaskCollection != null & objTaskCollection.Count > 0)
                                        {
                                            TaskDetlResult[i].Remove("ID");
                                            objTaskCollection.Add(TaskDetlResult[i]);

                                        }
                                        else
                                        {
                                            TaskDetlResult[i].Remove("ID");
                                            objTaskCollection = null;
                                            objTaskCollection = new List<Hashtable>();
                                            objTaskCollection.Add(TaskDetlResult[i]);
                                        }

                                        rptcollection[currentTaskdata].TaskCollections = objTaskCollection;
                                        objTaskCollection = null;
                                        objTaskCollection = new List<Hashtable>();

                                    }
                                    else
                                    {
                                        TaskDetlResult[i].Remove("ID");
                                        objTaskCollection.Add(TaskDetlResult[i]);
                                    }
                                }
                                else
                                {
                                    TaskDetlResult[i].Remove("ID");
                                    objTaskCollection.Add(TaskDetlResult[i]);
                                    rptcollection[currentTaskdata].TaskCollections = objTaskCollection;
                                    objTaskCollection = null;
                                }
                            }

                            //TaskDetlResult.RemoveAt(i);
                        }



                        EntityFinQry.Clear();

                        EntityFinQry.Append("  SELECT DISTINCT tet.EntityID AS ID,ISNULL( metso.StatusOptions,'-') as Name,   ");
                        EntityFinQry.Append("   (SELECT COUNT(1) FROM TM_EntityTask tet1 WHERE tet1.EntityID=tet.EntityID AND  tet1.taskstatus=" + (int)TaskStatus.In_progress + ") TasksInProgress,");
                        EntityFinQry.Append("   (SELECT COUNT(1) FROM TM_EntityTask tet1 WHERE tet1.EntityID=tet.EntityID AND  tet1.taskstatus=" + (int)TaskStatus.Unassigned + ") UnassignedTasks, ");
                        EntityFinQry.Append("   (SELECT COUNT(1) FROM TM_EntityTask tet1 WHERE tet1.EntityID=tet.EntityID AND  tet1.taskstatus=" + (int)TaskStatus.In_progress + " AND tet1.DueDate< GETDATE()) OverdueTasks,");
                        EntityFinQry.Append("   (SELECT COUNT(1) FROM TM_EntityTask tet1 WHERE tet1.EntityID=tet.EntityID AND  tet1.taskstatus=" + (int)TaskStatus.Unable_to_complete + ") UnableToComplete ");
                        EntityFinQry.Append("   FROM TM_EntityTask tet INNER JOIN MM_EntityStatus mes ON tet.EntityID=mes.EntityID and tet.TaskListID!=0 AND tet.EntityID IN(" + InClause + ") ");
                        EntityFinQry.Append("   INNER JOIN MM_EntityTypeStatus_Options metso ON mes.StatusID=metso.ID ");

                        List<Hashtable> TaskOverviewResult = tx.PersistenceManager.ReportRepository.ReportExecuteQuery(EntityFinQry.ToString());

                        foreach (var Currentval in TaskOverviewResult)
                        {
                            int currentIndex = rptcollection.Select(a => a.ID).ToList().IndexOf(Convert.ToInt32(Currentval["ID"]));
                            if (currentIndex != -1)
                            {
                                Currentval.Remove("ID");
                                rptcollection[currentIndex].TaskOverviewSummary = Currentval;
                            }
                        }

                    }




                    EntityFinQry.Clear();

                    EntityFinQry.Append("  SELECT pe.id AS ID,poev.ObjectiveID AS ObjectiveID,(SELECT TOP 1 pe1.Name FROM pm_entity pe1  WHERE pe1.id=poev.ObjectiveID) as Name,'/api/Metadata/GetEntityByID/' + cast(poev.ObjectiveID as varchar(10)) as [Href]  ");
                    EntityFinQry.Append("    FROM PM_Entity pe INNER JOIN PM_ObjectiveEntityValue poev ON pe.EntityID=poev.EntityID WHERE pe.[Active]=1 and pe.id in(" + InClause + ") ");


                    List<Hashtable> objResult = tx.PersistenceManager.ReportRepository.ReportExecuteQuery(EntityFinQry.ToString());



                    if (objResult != null)
                    {

                        int currentOBJOldId = 0;
                        int currentOBJdata = -1;
                        List<Hashtable> objOBJCollection = new List<Hashtable>();
                        //for (int i = CCResult.Count - 1; i >= 0; i--)
                        //{
                        int OBJMaxCount = objResult.Count;
                        for (int i = 0; i < OBJMaxCount; i++)
                        {
                            currentOBJOldId = Convert.ToInt32(objResult[i]["ID"]);
                            currentOBJdata = rptcollection.Select(a => a.ID).ToList().IndexOf(Convert.ToInt32(objResult[i]["ID"]));
                            if (currentOBJdata != -1)
                            {
                                if (i != OBJMaxCount - 1)
                                {
                                    if (currentOBJOldId != Convert.ToInt32(objResult[i + 1]["ID"]))
                                    {
                                        if (objOBJCollection != null & objOBJCollection.Count > 0)
                                        {
                                            objResult[i].Remove("ID");
                                            objOBJCollection.Add(objResult[i]);

                                        }
                                        else
                                        {
                                            objResult[i].Remove("ID");
                                            objOBJCollection = null;
                                            objOBJCollection = new List<Hashtable>();
                                            objOBJCollection.Add(objResult[i]);
                                        }

                                        rptcollection[currentOBJdata].ObjectiveCollections = objOBJCollection;
                                        objOBJCollection = null;
                                        objOBJCollection = new List<Hashtable>();

                                    }
                                    else
                                    {
                                        objResult[i].Remove("ID");
                                        objOBJCollection.Add(objResult[i]);
                                    }
                                }
                                else
                                {
                                    objResult[i].Remove("ID");
                                    objOBJCollection.Add(objResult[i]);
                                    rptcollection[currentOBJdata].ObjectiveCollections = objOBJCollection;
                                    objOBJCollection = null;
                                }
                            }

                            //CCResult.RemoveAt(i);
                        }

                    }








                    EntityFinQry.Clear();



                    EntityFinQry.Append("  SELECT distinct fin.EntityID  AS ID, ");
                    EntityFinQry.Append("         ISNULL(SUM(fin.TotalPlannedAmount), 0) AS Planned, ");
                    EntityFinQry.Append("         ISNULL(SUM(fin.TotalRequested), 0) AS Requested, ");
                    EntityFinQry.Append("         ISNULL(SUM(fin.TotalApprovedAmount), 0) AS ApprovedAllocation, ");
                    EntityFinQry.Append("         ISNULL(SUM(fin.ApprovedBudget), 0) AS ApprovedBudget, ");
                    EntityFinQry.Append("         ISNULL(SUM(fin.BudgetDeviation), 0) AS BudgetDeviation, ");
                    EntityFinQry.Append("         ISNULL(SUM(fin.TotalCommitedAmount), 0) AS Commited, ");
                    EntityFinQry.Append("         ISNULL(SUM(fin.TotalSpentAmount), 0) AS Spent, ");
                    EntityFinQry.Append("         ISNULL( ");
                    EntityFinQry.Append("             SUM(fin.TotalApprovedAmount) - SUM(fin.TotalSpentAmount), ");
                    EntityFinQry.Append("             0 ");
                    EntityFinQry.Append("         )             AS AvailableToSpend ");
                    //EntityFinQry.Append("         ,( ");
                    //EntityFinQry.Append("             SELECT TOP 1         NAME ");
                    //EntityFinQry.Append("             FROM   PM_Entity     pe ");
                    //EntityFinQry.Append("             WHERE  id = fin.CostCenterID ");
                    //EntityFinQry.Append("         )             AS NAME ");
                    EntityFinQry.Append("  FROM   ( ");
                    EntityFinQry.Append("             SELECT pefav.EntityID, ");
                    EntityFinQry.Append("                    pefav.CostCenterID, ");
                    EntityFinQry.Append("                    CASE  ");
                    EntityFinQry.Append("                         WHEN pe.level = 0 THEN ( ");
                    EntityFinQry.Append("                                  SELECT SUM(pf.PlannedAmount) ");
                    EntityFinQry.Append("                                  FROM   PM_Financial pf ");
                    EntityFinQry.Append("                                         INNER JOIN PM_Entity pe2 ");
                    EntityFinQry.Append("                                              ON  pe2.ID = pf.EntityID ");
                    EntityFinQry.Append("                                              AND pe2.UniqueKey LIKE pe.UniqueKey  ");
                    EntityFinQry.Append("                                                  + '.%' ");
                    EntityFinQry.Append("                                              AND pe2.[Level] = 1 ");
                    //EntityFinQry.Append("                                              AND pe2.TypeId IN (" + CollectionEntitypes + ") ");
                    EntityFinQry.Append("                                              AND pe2.[Active] = 1 ");
                    //EntityFinQry.Append("                                              AND pf.CostCenterID = pefav.CostCenterID ");
                    EntityFinQry.Append("                              ) ");
                    EntityFinQry.Append("                         ELSE pefav.PlannedAmount ");
                    EntityFinQry.Append("                    END  AS TotalPlannedAmount, ");
                    EntityFinQry.Append("                    CASE  ");
                    EntityFinQry.Append("                         WHEN pe.level = 0 THEN ( ");
                    EntityFinQry.Append("                                  SELECT SUM(pf.RequestedAmount) ");
                    EntityFinQry.Append("                                  FROM   PM_Financial pf ");
                    EntityFinQry.Append("                                         INNER JOIN PM_Entity pe2 ");
                    EntityFinQry.Append("                                              ON  pe2.ID = pf.EntityID ");
                    EntityFinQry.Append("                                              AND pe2.UniqueKey LIKE pe.UniqueKey  ");
                    EntityFinQry.Append("                                                  + '.%' ");
                    EntityFinQry.Append("                                              AND pe2.[Level] = 1 ");
                    EntityFinQry.Append("                                              AND pe2.TypeId IN (" + CollectionEntitypes + ") ");
                    EntityFinQry.Append("                                              AND pe2.[Active] = 1 ");
                    //EntityFinQry.Append("                                              AND pf.CostCenterID = pefav.CostCenterID ");
                    EntityFinQry.Append("                              ) ");
                    EntityFinQry.Append("                         ELSE pefav.RequestedAmount ");
                    EntityFinQry.Append("                    END  AS TotalRequested, ");
                    EntityFinQry.Append("                    CASE  ");
                    EntityFinQry.Append("                         WHEN pe.level = 0 THEN ( ");
                    EntityFinQry.Append("                                  SELECT SUM(pf.ApprovedAllocatedAmount) ");
                    EntityFinQry.Append("                                  FROM   PM_Financial pf ");
                    EntityFinQry.Append("                                         INNER JOIN PM_Entity pe2 ");
                    EntityFinQry.Append("                                              ON  pe2.ID = pf.EntityID ");
                    EntityFinQry.Append("                                              AND pe2.UniqueKey LIKE pe.UniqueKey  ");
                    EntityFinQry.Append("                                                  + '.%' ");
                    EntityFinQry.Append("                                              AND pe2.[Level] = 1 ");
                    //EntityFinQry.Append("                                              AND pe2.TypeId IN (" + CollectionEntitypes + ") ");
                    EntityFinQry.Append("                                              AND pe2.[Active] = 1 ");
                    //EntityFinQry.Append("                                              AND pf.CostCenterID = pefav.CostCenterID ");
                    EntityFinQry.Append("                              ) ");
                    EntityFinQry.Append("                         ELSE pefav.ApprovedAllocatedAmount ");
                    EntityFinQry.Append("                    END  AS TotalApprovedAmount, ");
                    EntityFinQry.Append("                    ISNULL( ");
                    EntityFinQry.Append("                        ( ");
                    EntityFinQry.Append("                            SELECT SUM(pefav2.Spent) AS Spent ");
                    EntityFinQry.Append("                            FROM   PM_Financial pefav2 ");
                    EntityFinQry.Append("                                   INNER JOIN PM_Entity pe2 ");
                    EntityFinQry.Append("                                        ON  pe2.ID = pefav2.EntityID ");
                    EntityFinQry.Append("                                        AND pe2.[Active] = 1 ");
                    EntityFinQry.Append("                                        AND pe2.TypeId IN (" + CollectionEntitypes + ") ");
                    //EntityFinQry.Append("                                        AND pefav2.CostCenterID = pefav.CostCenterID ");
                    EntityFinQry.Append("                            WHERE  pe2.UniqueKey LIKE pe.UniqueKey + '%' ");
                    EntityFinQry.Append("                        ), ");
                    EntityFinQry.Append("                        0 ");
                    EntityFinQry.Append("                    )    AS TotalSpentAmount, ");
                    EntityFinQry.Append("                    ISNULL( ");
                    EntityFinQry.Append("                        ( ");
                    EntityFinQry.Append("                            SELECT SUM(pefav2.Commited) AS Commited ");
                    EntityFinQry.Append("                            FROM   PM_Financial pefav2 ");
                    EntityFinQry.Append("                                   INNER JOIN PM_Entity pe2 ");
                    EntityFinQry.Append("                                        ON  pe2.ID = pefav2.EntityID ");
                    EntityFinQry.Append("                                        AND pe2.[Active] = 1 ");
                    EntityFinQry.Append("                                        AND pe2.TypeId IN (" + CollectionEntitypes + ") ");
                    //EntityFinQry.Append("                                        AND pefav2.CostCenterID = pefav.CostCenterID ");
                    EntityFinQry.Append("                            WHERE  pe2.UniqueKey LIKE pe.UniqueKey + '%' ");
                    EntityFinQry.Append("                        ), ");
                    EntityFinQry.Append("                        0 ");
                    EntityFinQry.Append("                    )    AS TotalCommitedAmount, ");
                    EntityFinQry.Append("                    CASE  ");
                    EntityFinQry.Append("                         WHEN pe.level = 0 THEN ( ");
                    EntityFinQry.Append("                                  SELECT SUM(pf.ApprovedBudget) ");
                    EntityFinQry.Append("                                  FROM   PM_Financial pf ");
                    EntityFinQry.Append("                                         INNER JOIN PM_Entity pe2 ");
                    EntityFinQry.Append("                                              ON  pe2.ID = pf.EntityID ");
                    EntityFinQry.Append("                                              AND pe2.UniqueKey LIKE pe.UniqueKey  ");
                    EntityFinQry.Append("                                                  + '.%' ");
                    EntityFinQry.Append("                                              AND pe2.[Level] = 1 ");
                    //EntityFinQry.Append("                                              AND pe2.TypeId IN (" + CollectionEntitypes + ") ");
                    EntityFinQry.Append("                                              AND pe2.[Active] = 1 ");
                    //EntityFinQry.Append("                                              AND pf.CostCenterID = pefav.CostCenterID ");
                    EntityFinQry.Append("                              ) ");
                    EntityFinQry.Append("                         ELSE pefav.ApprovedBudget ");
                    EntityFinQry.Append("                    END  AS ApprovedBudget, ");
                    EntityFinQry.Append("                    CASE  ");
                    EntityFinQry.Append("                         WHEN pe.level = 0 THEN ( ");
                    EntityFinQry.Append("                                  SELECT SUM( ");
                    EntityFinQry.Append("                                             CASE  ");
                    EntityFinQry.Append("                                                  WHEN pf.ApprovedBudgetDate IS  ");
                    EntityFinQry.Append("                                                       NULL THEN 0 ");
                    EntityFinQry.Append("                                                  WHEN (pf.ApprovedBudget - pf.ApprovedAllocatedAmount)  ");
                    EntityFinQry.Append("                                                       < 0 THEN 0 ");
                    EntityFinQry.Append("                                                  ELSE pf.ApprovedBudget - pf.ApprovedAllocatedAmount ");
                    EntityFinQry.Append("                                             END ");
                    EntityFinQry.Append("                                         ) ");
                    EntityFinQry.Append("                                  FROM   PM_Financial pf ");
                    EntityFinQry.Append("                                         INNER JOIN PM_Entity pe2 ");
                    EntityFinQry.Append("                                              ON  pe2.ID = pf.EntityID ");
                    EntityFinQry.Append("                                              AND pe2.UniqueKey LIKE pe.UniqueKey  ");
                    EntityFinQry.Append("                                                  + '.%' ");
                    EntityFinQry.Append("                                              AND pe2.[Level] = 1 ");
                    //EntityFinQry.Append("                                              AND pe2.TypeId IN (" + CollectionEntitypes + ") ");
                    EntityFinQry.Append("                                              AND pe2.[Active] = 1 ");
                    //EntityFinQry.Append("                                              AND pf.CostCenterID = pefav.CostCenterID ");
                    EntityFinQry.Append("                              ) ");
                    EntityFinQry.Append("                         ELSE CASE  ");
                    EntityFinQry.Append("                                   WHEN pefav.ApprovedBudgetDate IS NULL THEN 0 ");
                    EntityFinQry.Append("                                   WHEN (pefav.ApprovedBudget - pefav.ApprovedAllocatedAmount)  ");
                    EntityFinQry.Append("                                        < 0 THEN 0 ");
                    EntityFinQry.Append("                                   ELSE pefav.ApprovedBudget - pefav.ApprovedAllocatedAmount ");
                    EntityFinQry.Append("                              END ");
                    EntityFinQry.Append("                    END  AS BudgetDeviation ");
                    EntityFinQry.Append("             FROM   PM_Financial pefav ");
                    EntityFinQry.Append("                    INNER JOIN PM_Entity pe ");
                    EntityFinQry.Append("                         ON  pe.ID = pefav.EntityID ");
                    EntityFinQry.Append("                         AND pe.[active] = 1 ");
                    EntityFinQry.Append("                         AND pe.id IN (" + InClause + ") ");
                    EntityFinQry.Append("         )             AS fin ");
                    EntityFinQry.Append("  GROUP BY ");
                    EntityFinQry.Append("         fin.entityid ");
                    EntityFinQry.Append("         ,fin.CostCenterID ");
                    EntityFinQry.Append("  ORDER BY ");
                    EntityFinQry.Append("         fin.EntityID  ");


                    List<Hashtable> CCResult = tx.PersistenceManager.ReportRepository.ReportExecuteQuery(EntityFinQry.ToString());


                    if (CCResult != null)
                    {


                        int currentCCOldId = 0;
                        int currentCCdata = -1;
                        List<Hashtable> objCCCollection = new List<Hashtable>();
                        //for (int i = CCResult.Count - 1; i >= 0; i--)
                        //{
                        int CCMaxCount = CCResult.Count;
                        for (int i = 0; i < CCMaxCount; i++)
                        {
                            currentCCOldId = Convert.ToInt32(CCResult[i]["ID"]);
                            currentCCdata = rptcollection.Select(a => a.ID).ToList().IndexOf(Convert.ToInt32(CCResult[i]["ID"]));
                            if (currentCCdata != -1)
                            {
                                if (i != CCMaxCount - 1)
                                {
                                    if (currentCCOldId != Convert.ToInt32(CCResult[i + 1]["ID"]))
                                    {
                                        if (objCCCollection != null & objCCCollection.Count > 0)
                                        {
                                            CCResult[i].Remove("ID");
                                            objCCCollection.Add(CCResult[i]);

                                        }
                                        else
                                        {
                                            CCResult[i].Remove("ID");
                                            objCCCollection = null;
                                            objCCCollection = new List<Hashtable>();
                                            objCCCollection.Add(CCResult[i]);
                                        }

                                        rptcollection[currentCCdata].FinancialCollections = objCCCollection;
                                        objCCCollection = null;
                                        objCCCollection = new List<Hashtable>();

                                    }
                                    else
                                    {
                                        CCResult[i].Remove("ID");
                                        objCCCollection.Add(CCResult[i]);
                                    }
                                }
                                else
                                {
                                    CCResult[i].Remove("ID");
                                    objCCCollection.Add(CCResult[i]);
                                    rptcollection[currentCCdata].FinancialCollections = objCCCollection;
                                    objCCCollection = null;
                                }
                            }

                            //CCResult.RemoveAt(i);
                        }


                    }


                    tx.Commit();

                    return rptcollection;

                }
            }
            catch
            {

            }
            return null;

        }


        public IList<IGanttviewHeaderBar> GetAllGanttHeaderBar(CommonManagerProxy proxy)
        {
            try
            {
                IList<IGanttviewHeaderBar> ganttlst = new List<IGanttviewHeaderBar>();

                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    var GanttHeaderColle = tx.PersistenceManager.CommonRepository.Query<GanttviewHeaderBarDao>();
                    if (GanttHeaderColle != null)
                    {
                        foreach (var currentdata in GanttHeaderColle.ToList())
                        {
                            ganttlst.Add(new GanttviewHeaderBar { Id = currentdata.Id, Name = currentdata.Name, Description = currentdata.Description, ColorCode = currentdata.ColorCode, EndDate = currentdata.EndDate.ToString("yyyy/MM/dd"), Startdate = currentdata.Startdate.ToString("yyyy/MM/dd") });
                        }
                    }
                    tx.Commit();
                }
                return ganttlst;
            }
            catch
            {

            }
            return null;

        }

        public int InsertUpdateGanttHeaderBar(CommonManagerProxy proxy, int Id, string name, string description, DateTime startdate, DateTime enddate, string colorcode)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    GanttviewHeaderBarDao currentdao = new GanttviewHeaderBarDao();
                    currentdao.Id = Id;
                    currentdao.Description = description;
                    currentdao.Startdate = startdate;
                    currentdao.EndDate = enddate;
                    currentdao.ColorCode = colorcode;
                    currentdao.Name = name;
                    tx.PersistenceManager.CommonRepository.Save<GanttviewHeaderBarDao>(currentdao);
                    tx.Commit();
                    return currentdao.Id;
                }

            }
            catch
            {

            }
            return 0;

        }


        public bool DeleteGanttHeaderBar(CommonManagerProxy proxy, int Id)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {

                    tx.PersistenceManager.CommonRepository.DeleteByID<GanttviewHeaderBarDao>(Id);
                    tx.Commit();
                    return true;
                }

            }
            catch
            {

            }
            return false;

        }



        public IList<ICurrencyConverter> getCurrencyconverterData(CommonManagerProxy proxy)
        {
            try
            {
                IList<ICurrencyConverter> objcurrencyconverterlist = new List<ICurrencyConverter>();
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    var objcurrencyconverterdao = tx.PersistenceManager.CommonRepository.GetAll<CurrencyconverterDao>();
                    tx.Commit();
                    foreach (var i in objcurrencyconverterdao)
                    {
                        Currencyconverter objconverter = new Currencyconverter();
                        objconverter.Id = i.Id;
                        objconverter.Startdate = i.Startdate.ToString("yyyy/MMM/dd");
                        objconverter.Enddate = i.Enddate.ToString("yyyy/MMM/dd");
                        objconverter.Currencytype = i.Currencytype;

                        objconverter.Currencyrate = i.Currencyrate;
                        objcurrencyconverterlist.Add(objconverter);
                    }
                }
                return objcurrencyconverterlist;
            }
            catch (Exception ex)
            {
                return null;
            }
        }



        public bool Insertupdatecurrencyconverter(CommonManagerProxy proxy, DateTime Startdate, DateTime Enddate, string Currencytype, double Currencyrate, int ID)
        {

            try
            {
                IList<MultiProperty> parLIST = new List<MultiProperty>();
                CurrencyconverterDao dao = new CurrencyconverterDao();
                dao.Startdate = Startdate;
                dao.Enddate = Enddate;
                dao.Currencyrate = Currencyrate;
                dao.Currencytype = Currencytype;
                if (ID != 0)
                {
                    dao.Id = ID;
                }
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    tx.PersistenceManager.PlanningRepository.Save<CurrencyconverterDao>(dao);
                    tx.Commit();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public bool DeleteCurrencyconverterData(CommonManagerProxy proxy, int id)
        {
            try
            {
                CurrencyconverterDao dao = new CurrencyconverterDao();
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    dao = tx.PersistenceManager.CommonRepository.Get<CurrencyconverterDao>(id);
                    tx.Commit();
                }
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    tx.PersistenceManager.CommonRepository.Delete<CurrencyconverterDao>(dao);
                    tx.Commit();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public IList<ICurrencyConverter> GetRatesByID(CommonManagerProxy proxy, int id)
        {
            try
            {
                IList<ICurrencyConverter> getrateslist = new List<ICurrencyConverter>();
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {

                    IList<CurrencyconverterDao> ratesdao = new List<CurrencyconverterDao>();
                    ratesdao = tx.PersistenceManager.CommonRepository.GetAll<CurrencyconverterDao>();
                    tx.Commit();
                    var linqrates = from t in ratesdao where t.Id == id select t;
                    foreach (var temp in linqrates.ToList())
                    {
                        ICurrencyConverter rates = new Currencyconverter();
                        rates.Id = temp.Id;
                        rates.Startdate = temp.Startdate.ToString("yyyy / MM / dd");
                        rates.Enddate = temp.Enddate.ToString("yyyy / MM / dd");
                        rates.Currencytype = temp.Currencytype;
                        rates.Currencyrate = temp.Currencyrate;
                        getrateslist.Add(rates);
                    }
                }
                return getrateslist;

            }
            catch (Exception ex)
            {
                return null;
            }

        }



        public IList<ICurrencyConverter> GetExchangesratesbyCurrencytype(CommonManagerProxy proxy, int id)
        {
            try
            {
                IList<ICurrencyConverter> getrateslist = new List<ICurrencyConverter>();
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    IList<CurrencyconverterDao> ratesdao = new List<CurrencyconverterDao>();
                    ratesdao = tx.PersistenceManager.CommonRepository.GetAll<CurrencyconverterDao>();
                    tx.Commit();
                    var linqrates = from t in ratesdao where int.Parse(t.Currencytype) == id select t;
                    foreach (var temp in linqrates.ToList())
                    {
                        ICurrencyConverter rates = new Currencyconverter();
                        rates.Id = temp.Id;
                        rates.Startdate = temp.Startdate.ToString("yyyy/MMM/ dd");
                        rates.Enddate = temp.Enddate.ToString("yyyy/MMM/dd");
                        rates.Currencytype = temp.Currencytype;
                        rates.Currencyrate = temp.Currencyrate;
                        getrateslist.Add(rates);
                    }
                }
                return getrateslist;
            }
            catch (Exception ex)
            {
                return null;
            }


        }

        public int InsertEntityAttachmentsVersion(CommonManagerProxy proxy, int EntityID, IList<IAttachments> EntityAttachments, IList<IFile> EntityFiles, int FileID, int VersioningFileId)
        {
            int fileid = 0;
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    var FileObj1 = tx.PersistenceManager.TaskRepository.Query<FileDao>().ToList().Where(item => item.Id == FileID && item.Entityid == EntityID).Select(item => item.VersionNo).FirstOrDefault();
                    if (FileObj1 > 0)
                    {
                        tx.PersistenceManager.TaskRepository.ExecuteQuerywithMinParam(" update PM_Attachments set activefileversionID = 0 where EntityID = ?  and VersioningFileId = ? ", EntityID, VersioningFileId);
                        tx.Commit();
                    }
                }

                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    var FileObj = tx.PersistenceManager.TaskRepository.Query<AttachmentsDao>().ToList().Where(item => item.VersioningFileId == VersioningFileId && item.Entityid == EntityID).Select(item => item.ActiveVersionNo).Max();
                    FileObj += 1;
                    IList<FileDao> ifile = new List<FileDao>();
                    FileDao fldao = new FileDao();
                    if (EntityFiles != null)
                    {

                        foreach (var a in EntityFiles)
                        {
                            Guid NewId = Guid.NewGuid();

                            string filePath = ReadAdminXML("FileManagment");
                            var DirInfo = System.IO.Directory.GetParent(filePath);
                            string newFilePath = DirInfo.FullName;
                            System.IO.File.Move(filePath + "\\" + a.strFileID.ToString() + a.Extension, newFilePath + "\\" + NewId + a.Extension);
                            fldao = new FileDao();
                            fldao.Checksum = a.Checksum;
                            fldao.CreatedOn = a.CreatedOn;
                            fldao.Entityid = EntityID;
                            fldao.Extension = a.Extension;
                            fldao.MimeType = a.MimeType;
                            fldao.Moduleid = a.Moduleid;
                            fldao.Name = a.Name;
                            fldao.Ownerid = a.Ownerid;
                            fldao.Size = a.Size;
                            fldao.VersionNo = FileObj;
                            fldao.Fileguid = NewId;
                            fldao.Description = a.Description;
                            ifile.Add(fldao);
                            //descriptionObj.Add(NewId, a.Description);


                        }
                        tx.PersistenceManager.PlanningRepository.Save<FileDao>(ifile);
                        fileid = fldao.Id;
                        BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("Saved in File", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);
                    }


                    if (ifile != null)
                    {
                        IList<AttachmentsDao> iattachment = new List<AttachmentsDao>();
                        foreach (var a in ifile)
                        {
                            if (FileObj == 1)
                            {
                                // var result = descriptionObj.Where(r => r.Key == a.Fileguid).ToList();
                                AttachmentsDao attachedao = new AttachmentsDao();
                                attachedao.ActiveFileid = a.Id;
                                attachedao.ActiveVersionNo = FileObj;
                                attachedao.Createdon = DateTime.Now;
                                attachedao.Entityid = EntityID;
                                attachedao.Name = a.Name;
                                attachedao.Typeid = 4;
                                attachedao.ActiveFileVersionID = a.Id;
                                attachedao.VersioningFileId = VersioningFileId;
                                //attachedao.Description = result[0].Value;
                                iattachment.Add(attachedao);
                            }
                            else
                            {
                                AttachmentsDao attachedao = new AttachmentsDao();
                                attachedao.ActiveFileid = a.Id;
                                attachedao.ActiveVersionNo = FileObj;
                                attachedao.Createdon = DateTime.Now;
                                attachedao.Entityid = EntityID;
                                attachedao.Name = a.Name;
                                attachedao.Typeid = 4;
                                attachedao.ActiveFileVersionID = a.Id;
                                attachedao.VersioningFileId = VersioningFileId;
                                //attachedao.Description = result[0].Value;
                                iattachment.Add(attachedao);
                            }
                            //..for feed

                            FeedNotificationServer fs = new FeedNotificationServer();
                            NotificationFeedObjects obj = new NotificationFeedObjects();
                            obj.action = "attachment added";
                            obj.AttributeName = a.Name;
                            obj.attachmenttype = "file";
                            obj.Actorid = proxy.MarcomManager.User.Id;
                            obj.ToValue = Convert.ToString(a.Id);
                            obj.EntityId = EntityID;
                            fs.AsynchronousNotify(obj);
                        }
                        tx.PersistenceManager.PlanningRepository.Save<AttachmentsDao>(iattachment);
                        BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("Saved Attachments", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);

                        ////////////////Task Reinitialize concept for Approval task
                        //////////////EntityTaskDao entityTask = new EntityTaskDao();
                        //////////////IList<TaskMembersDao> itaskMemDao = new List<TaskMembersDao>();
                        //////////////var taskDao = (from item in tx.PersistenceManager.TaskRepository.Query<EntityTaskDao>() where item.ID == taskID select item).ToList();
                        //////////////if (taskDao.Count() > 0)
                        //////////////{
                        //////////////    entityTask = taskDao.FirstOrDefault();
                        //////////////    TaskMembersDao memdao = new TaskMembersDao();
                        //////////////    if (entityTask != null)
                        //////////////    {
                        //////////////        if (entityTask.TaskType != 2)
                        //////////////        {
                        //////////////            if (entityTask.TaskStatus == (int)TaskStatus.Rejected || entityTask.TaskStatus == (int)TaskStatus.Unable_to_complete || entityTask.TaskStatus == (int)TaskStatus.Approved)
                        //////////////            {

                        //////////////                entityTask.TaskStatus = (int)TaskStatus.In_progress;
                        //////////////                tx.PersistenceManager.TaskRepository.Save<EntityTaskDao>(entityTask);
                        //////////////            }

                        //////////////            var totalTaskmembers = tx.PersistenceManager.PlanningRepository.Query<TaskMembersDao>().Where(a => a.TaskID == taskID && a.RoleID != 1).ToList();
                        //////////////            if (totalTaskmembers != null)
                        //////////////            {
                        //////////////                foreach (var mem in totalTaskmembers)
                        //////////////                {
                        //////////////                    mem.ApprovalStatus = null;
                        //////////////                    itaskMemDao.Add(mem);
                        //////////////                }
                        //////////////                tx.PersistenceManager.TaskRepository.Save<TaskMembersDao>(itaskMemDao);
                        //////////////            }

                        //////////////        }
                        //////////////    }
                        //////////////}

                    }


                    tx.Commit();
                    return fileid;
                }

            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public IList<ICustomTab> GetCustomTabsByTypeID(CommonManagerProxy proxy, int TypeID)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {

                    IList<CustomTabDao> tabCollection = new List<CustomTabDao>();
                    if (TypeID != 0)
                        tabCollection = tx.PersistenceManager.CommonRepository.Query<CustomTabDao>().Where(a => a.Typeid == TypeID).OrderBy(a => a.SortOrder).Cast<CustomTabDao>().ToList();
                    else
                        tabCollection = tx.PersistenceManager.CommonRepository.Query<CustomTabDao>().OrderBy(a => a.SortOrder).Cast<CustomTabDao>().ToList();

                    if (tabCollection != null)
                    {
                        IList<ICustomTab> Customtablist = new List<ICustomTab>();
                        foreach (var CurrentTabdata in tabCollection.ToList())
                        {
                            if (CurrentTabdata.EntityTypeID == 0)
                            {
                                Customtablist.Add(new CustomTab
                                {
                                    Id = CurrentTabdata.ID,
                                    Name = CurrentTabdata.Name,
                                    Typeid = CurrentTabdata.Typeid,
                                    AddEntityID = CurrentTabdata.AddEntityID,
                                    AddLanguageCode = CurrentTabdata.AddLanguageCode,
                                    AddUserEmail = CurrentTabdata.AddUserEmail,
                                    AddUserID = CurrentTabdata.AddUserID,
                                    AddUserName = CurrentTabdata.AddUserName,
                                    ExternalUrl = CurrentTabdata.ExternalUrl,
                                    IsSytemDefined = CurrentTabdata.IsSytemDefined,
                                    SortOrder = CurrentTabdata.SortOrder
                                });
                            }
                        }
                        return Customtablist;

                    }

                }
            }
            catch
            {

            }
            return null;

        }

        public IList<ICustomTab> GetCustomEntityTabsByTypeID(CommonManagerProxy proxy, int TypeID, int CalID, int EntityTypeID = 0, int EntityID = 0)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    IList<int> customtabs = new List<int>();
                    bool status = true;
                    IList<CustomTabDao> tabCollection = new List<CustomTabDao>();

                    int[] FeatureCollections = tx.PersistenceManager.CommonRepository.Query<FeatureDao>().Where(a => (Modules)a.ModuleID == Modules.Planning).Select(a => a.Id).ToArray();
                    int[] FeaturePermission = proxy.MarcomManager.User.ListOfUserGlobalRoles.Where(a => a.AccessPermission == true && (FeatureCollections.Contains(a.Featureid)) && a.Moduleid == (int)Modules.Planning).Select(a => a.Featureid).ToArray();
                    int[] CurrentUserRole = proxy.MarcomManager.User.ListOfUserGlobalRoles.Select(a => a.GlobalRoleid).Distinct().ToArray();

                    //List<CustomTabDao> Tabs = tx.PersistenceManager.UserRepository.Query<CustomTabDao>().Where(x => x.EntityTypeID == EntityTypeID).ToList();
                    if (CalID != 0)
                    {
                        customtabs = tx.PersistenceManager.CommonRepository.Query<CalenderTabDao>().Where(a => a.Calenderid == CalID).Select(a => a.CustomTabid).ToList();
                    }

                    if (FeaturePermission.Length > 0)
                    {
                        string xmlpath = string.Empty;
                        var entityObj = (from item in tx.PersistenceManager.PlanningRepository.Query<EntityDao>()
                                         where item.Id == EntityID
                                         select item).FirstOrDefault();

                        EntityTypeID = entityObj.Typeid;

                        tabCollection = tx.PersistenceManager.CommonRepository.Query<CustomTabDao>().Where(a => (a.Typeid == TypeID &&
                            (FeaturePermission.Contains(a.FeatureID) || a.FeatureID == 0) && (a.EntityTypeID == EntityTypeID || a.EntityTypeID == 0)))
                            .OrderBy(a => a.SortOrder).Cast<CustomTabDao>().ToList();

                        if (tabCollection != null)
                        {

                            xmlpath = tx.PersistenceManager.MetadataRepository.GetXmlPath(entityObj.Version);

                            int cnt = 0;
                            IList<ICustomTab> Customtablist = new List<ICustomTab>();
                            string qry = "SELECT COUNT(*) as GlobalAccess FROM   AM_Entity_Role_User aeru  inner join AM_EntityTypeRoleAcl aera on aera.ID = aeru.RoleID  WHERE aeru.EntityID = " + EntityID + " AND aeru.UserID = " + proxy.MarcomManager.User.Id + " AND aera.EntityRoleID IN (1,2,8)";
                            var result = tx.PersistenceManager.CommonRepository.ExecuteQuery(qry);

                            var attrRoleAcc = tx.PersistenceManager.MetadataRepository.GetObject<AttributeGroupRoleAccessDao>(xmlpath);

                            foreach (var CurrentTabdata in tabCollection.ToList())
                            {
                                status = true;

                                if (CurrentTabdata.AttributeGroupID == 0)
                                {
                                    var res = tx.PersistenceManager.CommonRepository.Query<CustomTabEntityTypeRoleAclDao>().Where(a => a.CustomTabID == CurrentTabdata.ID).ToList();
                                    if (CurrentTabdata.IsSytemDefined == false)
                                    {
                                        if (res.Count > 0)
                                        {
                                            var accss = res.Where(a => a.EntityTypeID == EntityTypeID).ToList();
                                            if (accss.Count > 0)
                                            {
                                                if ((int)((System.Collections.Hashtable)(result)[0])["GlobalAccess"] <= 0)
                                                {
                                                    if (!CurrentUserRole.Contains(1))
                                                    {
                                                        if (CurrentTabdata.IsSytemDefined == false && res.Count > 0 && res.Where(a => CurrentUserRole.Contains(a.GlobalRoleID)).Count() <= 0)
                                                        {
                                                            status = false;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (CurrentTabdata.AttributeGroupID > 0)
                                {
                                    if ((int)((System.Collections.Hashtable)(result)[0])["GlobalAccess"] <= 0)
                                    {
                                        if (!CurrentUserRole.Contains(1))
                                        {
                                            if (attrRoleAcc != null)
                                            {
                                                cnt = attrRoleAcc.Where(a => a.EntityTypeID == EntityTypeID && a.AttributeGroupID == CurrentTabdata.AttributeGroupID && CurrentUserRole.Contains(a.GlobalRoleID)).Count();
                                                if (cnt <= 0)
                                                    status = false;
                                            }
                                        }
                                    }
                                }

                                if (status == true)
                                {

                                    if (CalID == 0)
                                    {
                                        Customtablist.Add(new CustomTab
                                        {
                                            Id = CurrentTabdata.ID,
                                            Name = CurrentTabdata.Name,
                                            Typeid = CurrentTabdata.Typeid,
                                            AddEntityID = CurrentTabdata.AddEntityID,
                                            AddLanguageCode = CurrentTabdata.AddLanguageCode,
                                            AddUserEmail = CurrentTabdata.AddUserEmail,
                                            AddUserID = CurrentTabdata.AddUserID,
                                            AddUserName = CurrentTabdata.AddUserName,
                                            ExternalUrl = CurrentTabdata.ExternalUrl,
                                            IsSytemDefined = CurrentTabdata.IsSytemDefined,
                                            SortOrder = CurrentTabdata.SortOrder,
                                            ControleID = CurrentTabdata.ControleID,
                                            AttributeGroupID = CurrentTabdata.AttributeGroupID
                                        });
                                    }
                                    else
                                    {
                                        foreach (int id in customtabs)
                                        {
                                            if (CurrentTabdata.ID == id)
                                            {
                                                Customtablist.Add(new CustomTab
                                                {
                                                    Id = CurrentTabdata.ID,
                                                    Name = CurrentTabdata.Name,
                                                    Typeid = CurrentTabdata.Typeid,
                                                    AddEntityID = CurrentTabdata.AddEntityID,
                                                    AddLanguageCode = CurrentTabdata.AddLanguageCode,
                                                    AddUserEmail = CurrentTabdata.AddUserEmail,
                                                    AddUserID = CurrentTabdata.AddUserID,
                                                    AddUserName = CurrentTabdata.AddUserName,
                                                    ExternalUrl = CurrentTabdata.ExternalUrl,
                                                    IsSytemDefined = CurrentTabdata.IsSytemDefined,
                                                    SortOrder = CurrentTabdata.SortOrder,
                                                    ControleID = CurrentTabdata.ControleID,
                                                    AttributeGroupID = CurrentTabdata.AttributeGroupID
                                                });
                                            }
                                        }
                                    }
                                }
                            }
                            return Customtablist;
                        }
                    }
                }
            }
            catch
            {

            }
            return null;

        }

        public string GetCustomTabUrlTabsByTypeID(CommonManagerProxy proxy, int tabID, int entityID)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {

                    var CurrentTabdata = tx.PersistenceManager.CommonRepository.Query<CustomTabDao>().Where(a => a.ID == tabID).SingleOrDefault();
                    if (CurrentTabdata != null)
                    {
                        string EncryptedUrl = CurrentTabdata.ExternalUrl;
                        string Url = "PARAMETER=(";

                        bool IsExist = false;
                        if (CurrentTabdata.AddUserID)
                        {
                            Url += "UID=" + proxy.MarcomManager.User.Id.ToString();
                            IsExist = true;
                        }
                        if (CurrentTabdata.AddUserName)
                        {
                            if (IsExist)
                                Url += "&UN=" + proxy.MarcomManager.User.FirstName + " " + proxy.MarcomManager.User.LastName;
                            else
                                Url += "UN=" + proxy.MarcomManager.User.FirstName + " " + proxy.MarcomManager.User.LastName;
                            IsExist = true;
                        }
                        if (CurrentTabdata.AddLanguageCode)
                        {
                            if (IsExist)
                                Url += "&LN=" + proxy.MarcomManager.User.Language;
                            else
                                Url += "LN=" + proxy.MarcomManager.User.Language;
                            IsExist = true;
                        }
                        if (CurrentTabdata.AddUserEmail)
                        {
                            if (IsExist)
                                Url += "&UE=" + proxy.MarcomManager.User.Email;
                            else
                                Url += "UE=" + proxy.MarcomManager.User.Email;
                            IsExist = true;
                        }
                        if (CurrentTabdata.AddEntityID)
                        {
                            if (IsExist)
                                Url += "&EID=" + entityID;
                            else
                                Url += "EID=" + entityID;
                            IsExist = true;
                        }

                        if (IsExist)
                        {
                            var tabCollection = tx.PersistenceManager.CommonRepository.Query<TabEncryptionDao>().Where(a => a.CustomTabID == tabID).SingleOrDefault();

                            Url += ")&TIMESTAMP=" + Convert.ToInt32((DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds).ToString();
                            EncryptedUrl += "?token=" + EncryptDecryptQueryString.EncryptCustomTabUrlQueryString(Url.ToString(), tabCollection.EncryKey, tabCollection.EncryIV, tabCollection.Algorithm, tabCollection.PaddingMode, tabCollection.CipherMode);
                        }
                        return EncryptedUrl;
                    }
                }
            }
            catch
            {

            }
            return null;

        }

        public int[] InsertUpdateCustomTab(CommonManagerProxy proxy, int ID, int Typeid, string Name, string ExternalUrl, bool AddEntityID, bool AddLanguageCode, bool AddUserEmail, bool AddUserName, bool AddUserID, int tabencryID, string encryKey, string encryIV, string algorithm, string paddingMode, string cipherMode, string entitytypeids, string globalids)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    //if intSortOrder=tx.PersistenceManager.CommonRepository.Query<CustomTabDao>().ToList().Count();
                    CustomTabDao CurrentDao = new CustomTabDao();
                    CurrentDao.ID = ID;
                    CurrentDao.AddEntityID = AddEntityID;
                    CurrentDao.AddLanguageCode = AddLanguageCode;
                    CurrentDao.AddUserEmail = AddUserEmail;
                    CurrentDao.AddUserID = AddUserID;
                    CurrentDao.AddUserName = AddUserName;
                    CurrentDao.ExternalUrl = ((ExternalUrl.Contains("http://") || ExternalUrl.Contains("https://")) ? ExternalUrl : "http://" + ExternalUrl);
                    CurrentDao.IsSytemDefined = false;
                    CurrentDao.Name = Name;
                    CurrentDao.SortOrder = (ID != 0 ? tx.PersistenceManager.CommonRepository.Query<CustomTabDao>().Where(a => a.ID == ID).Select(a => a.SortOrder).SingleOrDefault() : (tx.PersistenceManager.CommonRepository.Query<CustomTabDao>().ToList().Count() + 1));
                    CurrentDao.Typeid = Typeid;
                    CurrentDao.ControleID = "customtab" + CurrentDao.ID.ToString();
                    tx.PersistenceManager.CommonRepository.Save<CustomTabDao>(CurrentDao);

                    TabEncryptionDao tabencrydao = new TabEncryptionDao();
                    tabencrydao.ID = tabencryID;
                    tabencrydao.CustomTabID = CurrentDao.ID;
                    tabencrydao.EncryKey = encryKey;
                    tabencrydao.EncryIV = encryIV;
                    tabencrydao.Algorithm = algorithm;
                    tabencrydao.PaddingMode = paddingMode;
                    tabencrydao.CipherMode = cipherMode;
                    tx.PersistenceManager.CommonRepository.Save<TabEncryptionDao>(tabencrydao);

                    tx.PersistenceManager.PlanningRepository.ExecuteQuerywithMinParam("delete from  CM_CustomTabEntityTypeRoleAcl where CustomTabID=?", CurrentDao.ID);
                    IList<CustomTabEntityTypeRoleAclDao> listentitytype = new List<CustomTabEntityTypeRoleAclDao>();

                    if (entitytypeids.Length > 0)
                    {
                        foreach (var obj in entitytypeids.Split(','))
                        {
                            foreach (var itm in globalids.Split(','))
                            {
                                listentitytype.Add(new CustomTabEntityTypeRoleAclDao { CustomTabID = CurrentDao.ID, EntityTypeID = int.Parse(obj), GlobalRoleID = int.Parse(itm) });
                            }
                        }
                        tx.PersistenceManager.CommonRepository.Save<CustomTabEntityTypeRoleAclDao>(listentitytype);
                    }
                    tx.Commit();
                    return new[] { CurrentDao.ID, tabencrydao.ID };
                }
            }
            catch
            {

            }
            return new[] { 0, 0 };

        }

        public bool DeleteCustomtabByID(CommonManagerProxy proxy, int ID, int AttributeTypeID, int EntityTypeID)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    tx.PersistenceManager.CommonRepository.ExecuteQuerywithMinParam("DELETE FROM CM_CutomTabEncryptionRelation WHERE CustomTabID = ?", ID);
                    tx.PersistenceManager.CommonRepository.ExecuteQuerywithMinParam("DELETE FROM CM_CustomTabEntityTypeRoleAcl WHERE CustomTabID = ?", ID);
                    if (EntityTypeID != 0)
                    {
                        string DeleteAttributeGroup = "delete dbo.CM_CustomTabs where EntityTypeID = " + EntityTypeID + " and attributegroupid =" + AttributeTypeID + "";
                        tx.PersistenceManager.CommonRepository.ExecuteQuery(DeleteAttributeGroup);
                    }
                    else
                    {
                        tx.PersistenceManager.CommonRepository.DeleteByID<CustomTabDao>(ID);
                    }
                    tx.Commit();


                    return true;
                }
            }
            catch
            {

            }
            return false;
        }

        public bool UpdateCustomTabSortOrder(CommonManagerProxy proxy, int ID, int sortorder)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    CustomTabDao CurrentDao = new CustomTabDao();
                    IList<MultiProperty> tabIpparams = new List<MultiProperty>();
                    tabIpparams.Add(new MultiProperty { propertyName = CustomTabDao.PropertyNames.SortOrder, propertyValue = sortorder });
                    IList<MultiProperty> tabCondparams = new List<MultiProperty>();
                    tabCondparams.Add(new MultiProperty { propertyName = CustomTabDao.PropertyNames.ID, propertyValue = ID });
                    tx.PersistenceManager.CommonRepository.UpdateByID<CustomTabDao>(tabIpparams, tabCondparams);
                    tx.Commit();
                    return true;
                }
            }
            catch
            {

            }
            return false;

        }


        public bool InsertUpdateApplicationUrlTrack(CommonManagerProxy proxy, Guid TrackID, string TrackValue)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    //Guid IsExist;// = tx.PersistenceManager.CommonRepository.Query<ApplicationUrlTrackDao>().Where(a => a.TrackValue == TrackValue).Select(a => a.TrackID).Cast<Guid>();
                    //if (IsExist == null)
                    //{
                    //    IsExist = Guid.NewGuid();
                    //}
                    ApplicationUrlTrackDao trackDao = new ApplicationUrlTrackDao();
                    trackDao.TrackID = TrackID;
                    trackDao.TrackValue = TrackValue;
                    trackDao.DOI = DateTime.Now;
                    tx.PersistenceManager.CommonRepository.Save<ApplicationUrlTrackDao>(trackDao);
                    tx.Commit();
                    return true;
                }
            }
            catch
            {

            }
            return false;

        }

        public string GetApplicationUrlTrackByID(CommonManagerProxy proxy, Guid TrackID)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    var CurrentRecord = tx.PersistenceManager.CommonRepository.Query<ApplicationUrlTrackDao>().Where(a => a.TrackID == TrackID).SingleOrDefault();
                    //IApplicationUrlTrack apptrack = new ApplicationUrlTrack();
                    if (CurrentRecord != null)
                    {
                        //apptrack.TrackValue = CurrentRecord.TrackValue;
                        return CurrentRecord.TrackValue;
                    }
                    tx.Commit();
                    //return apptrack;
                }
            }
            catch
            {

            }
            return null;

        }

        public bool UpdateCustomTabSettings(CommonManagerProxy proxy, string key, string iv, string algo, string paddingmode, string ciphermode, string tokenmode)
        {
            try
            {
                string xmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
                string xelementName = "CustomTab";
                var xelementFilepath = XElement.Load(xmlpath);
                var xmlElement = xelementFilepath.Element(xelementName);
                foreach (var des in xmlElement.Descendants())
                {
                    switch (des.Name.ToString())
                    {
                        case "Key":
                            des.Value = key;
                            break;
                        case "IV":
                            des.Value = iv;
                            break;
                        case "Algorithm":
                            des.Value = algo;
                            break;
                        case "PaddingMode":
                            des.Value = paddingmode;
                            break;
                        case "CipherMode":
                            des.Value = ciphermode;
                            break;
                        case "SSOTokenMode":
                            des.Value = tokenmode;
                            break;
                        default:
                            break;

                    }

                }

                xelementFilepath.Save(xmlpath);

                return true;
            }
            catch
            {

            }
            return false;
        }

        public IList<SSO> GetCustomTabSettingDetails(CommonManagerProxy proxy)
        {
            try
            {
                string xmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
                XDocument adminXmlDoc = XDocument.Load(xmlpath);
                //The Key is root node current Settings
                string xelementName = "CustomTab";
                var xelementFilepath = XElement.Load(xmlpath);
                var xmlElement = xelementFilepath.Element(xelementName);
                IList<SSO> ssolist = new List<SSO>();
                SSO sso = new SSO();
                sso.Algorithmoption = new List<string>();


                var algos = Enum.GetValues(typeof(BrandSystems.Cryptography.Security.AlgoType)).Cast<BrandSystems.Cryptography.Security.AlgoType>().Select(v => v.ToString());

                foreach (var obj in algos)
                {
                    sso.Algorithmoption.Add(obj);
                }
                sso.PaddingModeoption = new List<string>();
                var paddingmodes = Enum.GetValues(typeof(System.Security.Cryptography.PaddingMode)).Cast<System.Security.Cryptography.PaddingMode>().Select(v => v.ToString());
                foreach (var obj in paddingmodes)
                {
                    sso.PaddingModeoption.Add(obj);
                }
                sso.CipherModeoption = new List<string>();
                var ciphermodes = Enum.GetValues(typeof(System.Security.Cryptography.CipherMode)).Cast<System.Security.Cryptography.CipherMode>().Select(v => v.ToString());
                foreach (var obj in ciphermodes)
                {
                    sso.CipherModeoption.Add(obj);
                }

                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    IList<GlobalRoleDao> groups = new List<GlobalRoleDao>();
                    groups = (from role in tx.PersistenceManager.AccessRepository.Query<GlobalRoleDao>() select role).ToList<GlobalRoleDao>();
                    sso.UserGroupsoption = new List<GlobalRole>();
                    foreach (var str in groups)
                    {
                        GlobalRole grb = new GlobalRole();
                        grb.Caption = str.Caption;
                        grb.Id = str.Id;
                        sso.UserGroupsoption.Add(grb);
                    }

                }

                sso.SSOTokenModeoption = new List<string>();
                sso.SSOTokenModeoption.Add("TEXT");
                sso.SSOTokenModeoption.Add("XML");

                foreach (var des in xmlElement.Descendants())
                {

                    switch (des.Name.ToString())
                    {
                        case "Key":
                            sso.key = des.Value.ToString();
                            break;
                        case "IV":
                            sso.IV = des.Value.ToString();
                            break;
                        case "Algorithm":
                            sso.AlgorithmValue = des.Value.ToString();
                            break;
                        case "PaddingMode":
                            sso.PaddingModeValue = des.Value.ToString();
                            break;
                        case "CipherMode":
                            sso.CipherModeValue = des.Value.ToString();
                            break;
                        default:
                            break;

                    }

                }
                ssolist.Add(sso);
                return ssolist;
            }
            catch
            {
                return null;
            }

        }




        public IConvertedcurrencies GetConvertedcurrencies(CommonManagerProxy proxy, int Amount, int CurrencyId, string Currencytype, DateTime Duedate)
        {
            try
            {
                IConvertedcurrencies dao1 = new ConvertedCurrencies();

                // string duedate = Duedate.ToString().Replace('/' , '-');

                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    int currencytypeid = 0;
                    DateTime? Startdate = null;
                    DateTime? Enddate = null;
                    string settingvalue = "";
                    decimal currencyrate = 0;
                    var CurrencySelectQuery = new StringBuilder();
                    var IdofCurrency = tx.PersistenceManager.CommonRepository.ExecuteQuery("select id from pm_currencytype where shortname='" + Currencytype + "'");

                    foreach (var value in IdofCurrency)
                    {
                        currencytypeid = (int)((System.Collections.Hashtable)(value))["id"];
                    }

                    //var IdofCurrency = tx.PersistenceManager.CommonRepository.ExecuteQuery("select id from pm_currencytype where shortname='" + Currencytype + "'");
                    //var Query = tx.PersistenceManager.CommonRepository.ExecuteQuery("select cc.Id,cc.Currencyrate,cc.Startdate,cc.Enddate from CM_currencyconverter cc inner join PM_CurrencyType ctype on cc.Currencytype=ctype.ID where  cc.Startdate<='" + duedate + "' and cc.Enddate >='" + duedate + "' and  cc.Currencytype='" + currencytypeid + "'").Cast<Hashtable>();
                    CurrencySelectQuery.Append("select cc.Id,cc.Currencyrate,cc.Startdate,cc.Enddate from CM_currencyconverter cc inner join PM_CurrencyType ctype on cc.Currencytype=ctype.ID where  cast( cc.Startdate as date)<= cast(? as date) and cast(cc.Enddate as date) >= cast( ? as date) and  cc.Currencytype=" + currencytypeid);

                    var Query = ((tx.PersistenceManager.CommonRepository.ExecuteQuerywithMinParam(CurrencySelectQuery.ToString(), Duedate.ToString("yyyy/MM/dd"), Duedate.ToString("yyyy/MM/dd"))).Cast<Hashtable>().ToList());

                    var result = tx.PersistenceManager.CommonRepository.ExecuteQuery("SELECT SettingValue FROM CM_AdditionalSettings where id=2");
                    foreach (var item in result)
                    {
                        settingvalue = (string)((System.Collections.Hashtable)(item))["SettingValue"];
                    }

                    var currlist = tx.PersistenceManager.CommonRepository.Query<CurrencyTypeDao>().Where(a => a.Id == Convert.ToInt32(settingvalue));

                    IConvertedcurrencies daoi = new ConvertedCurrencies();

                    foreach (var value in Query)
                    {
                        currencytypeid = (int)((System.Collections.Hashtable)(value))["Id"];
                        Startdate = ((DateTime)((System.Collections.Hashtable)(value))["Startdate"]); ;
                        Enddate = ((DateTime)((System.Collections.Hashtable)(value))["Enddate"]);
                    }

                    if (currencytypeid == 0)
                    {
                        daoi.Id = CurrencyId;
                        daoi.ConvertedAmount = 0;
                        foreach (var val in currlist)
                        {
                            daoi.Currency = val.ShortName;
                        }
                        daoi.Error = "Currency not available";
                    }

                    else if (currencytypeid == 0 || Startdate == null || Enddate == null
                        )
                    {
                        daoi.Id = CurrencyId;
                        foreach (var val in currlist)
                        {
                            daoi.Currency = val.ShortName;
                        }
                        daoi.ConvertedAmount = 0;
                        daoi.Error = "Exchange rates are not available for this currency";
                    }

                    else if (Query != null && Query.Count() != 0)
                    {
                        foreach (var val in Query)
                        {
                            currencyrate = (decimal)val["Currencyrate"];
                        }
                        foreach (var cc1 in currlist)
                        {
                            daoi.Currency = cc1.ShortName;
                        }
                        daoi.Id = CurrencyId;
                        decimal getCurrencyratebyAmount;
                        getCurrencyratebyAmount = Amount / currencyrate;
                        daoi.ConvertedAmount = Math.Round((decimal)getCurrencyratebyAmount, 2);
                        daoi.Error = "";
                        tx.Commit();
                    }
                    return daoi;
                }
            }

            catch (Exception e)
            {
                throw e;
            }
        }

        public IList<IConvertedcurrencies> CurrencyConvertJSON(CommonManagerProxy proxy, JObject curr)
        {
            try
            {
                JObject jobj = JObject.Parse(curr.ToString());
                JArray SortOrderUpdate = (JArray)jobj["Convertcurrencies"];
                IList<IConvertedcurrencies> objcurrncy = new List<IConvertedcurrencies>();
                IConvertedcurrencies intobj = null;

                foreach (var item in SortOrderUpdate)
                {
                    intobj = GetConvertedcurrencies(proxy, (int)item["Amount"], (int)item["ID"], (string)item["Currency"], (DateTime)item["Date"]);
                    objcurrncy.Add(intobj);
                }
                return objcurrncy;
            }

            catch (Exception e)
            {
                throw e;
            }
        }


        public Tuple<IUpdateSettings, string> GetUpdateSettings(CommonManagerProxy proxy)
        {
            try
            {
                IList<IUpdateSettings> iiupdatelist = new List<IUpdateSettings>();
                IUpdateSettings IUpdate = new UpdateSettings();
                IList<UpdateSettingsDao> updSettingsdao = new List<UpdateSettingsDao>();
                string xmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
                XDocument adminXmlDoc = XDocument.Load(xmlpath);
                string xelementName = "UpgradeToolURL";
                var xelementFilepath = XElement.Load(xmlpath);
                string defMember_firstName = "";
                string defMember_lastName = "";
                string upgradetoolurl = xelementFilepath.Element(xelementName).Value.ToString();
                string upgradetoolxmlPath = xelementFilepath.Element("UpgradeToolxmlPath").Value.ToString();

                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    var CurrentVersion = new StringBuilder();
                    CurrentVersion.Append("select * from MarcomUpdateDetails m3 where CurrentVersion=1");
                    var CurrentVersionResult = ((tx.PersistenceManager.CommonRepository.ExecuteQuery(CurrentVersion.ToString())).Cast<Hashtable>().ToList());
                    IList<UpdateSettingsDao> dem = tx.PersistenceManager.CommonRepository.GetAll<UpdateSettingsDao>();

                    IUpdateSettings updates = new BrandSystems.Marcom.Core.Common.UpdateSettings();
                    foreach (var CurreVer in CurrentVersionResult)
                    {
                        IList<IUpdateSettings> Current = new List<IUpdateSettings>();
                        updates.Client = Convert.ToString(CurreVer["Client"]);
                        updates.CurrentVersion = Convert.ToString(CurreVer["Version"]);
                        updates.Licence = Convert.ToString(CurreVer["License"]);
                        updates.InstanceId = Convert.ToInt32(CurreVer["InstanceId"]);
                        updates.VersionId = Convert.ToInt32(CurreVer["VersionId"]);
                    }

                    var d = getupgradedetsfromxml(upgradetoolxmlPath, updates.InstanceId);
                    IList<IAvailableUpdates> ListAvailUpdates = new List<IAvailableUpdates>();
                    IList<IPreviousUpdates> ListPrevUpdates = new List<IPreviousUpdates>();
                    string emailname = proxy.MarcomManager.User.Email.ToString();

                    if (emailname == "amarnath.m@brandsystems.in" || emailname == "priyanka.goswami@brandsystems.in" || emailname == "peter.svahn@brandsystems.com" || emailname == "prabhudatta.tripathy@brandsystems.in")
                    {
                        foreach (var obj in d.Item1)
                        {
                            UpgradeTool.Version v1 = ((UpgradeTool.Version)(obj));
                            IAvailableUpdates availUp = new AvailableUpdates();
                            availUp.NewVersion = v1.versionname;
                            availUp.Feature = v1.features.ToString();
                            availUp.DateReleased = v1.datereleased;
                            availUp.Id = v1.Id;
                            availUp.DocPath = v1.DocPath;
                            ListAvailUpdates.Add(availUp);
                        }
                    }
                    updates.AvailableUpdates = ListAvailUpdates;
                    var temp1 = "";
                    foreach (var obj1 in d.Item2)
                    {
                        UpgradeTool.Version v2 = ((UpgradeTool.Version)(obj1));
                        IPreviousUpdates PrevUpdates = new PreviousUpdates();
                        PrevUpdates.Feature = v2.features.ToString();
                        PrevUpdates.Version = v2.versionname;
                        PrevUpdates.Status = "Success";
                        PrevUpdates.DocPath = v2.DocPath;
                        var temp_date = dem.Where(a => a.VersionId == v2.Id).Select(a => a.Date).SingleOrDefault().ToString();
                        var memberid = dem.Where(a => a.VersionId == v2.Id).Select(a => a.MemberId).SingleOrDefault();
                        if (!temp_date.Contains("0001"))
                        {
                            temp1 = temp_date;
                        }
                        PrevUpdates.date = (temp_date.Contains("0001")) ? temp1 : temp_date;
                        //PrevUpdates.date = temp_date;
                        PrevUpdates.Id = v2.Id;
                        var memberDetails = (from item in tx.PersistenceManager.TaskRepository.Query<UserDao>() where item.Id == (int)memberid select item).FirstOrDefault();

                        if (memberDetails != null)
                        {
                            defMember_firstName = memberDetails.FirstName;
                            defMember_lastName = memberDetails.LastName;
                            PrevUpdates.FirstName = memberDetails.FirstName;
                            PrevUpdates.LastName = memberDetails.LastName;
                        }
                        else
                        {
                            PrevUpdates.FirstName = defMember_firstName;
                            PrevUpdates.LastName = defMember_lastName;
                        }
                        DateTime lastUpdatedat = Convert.ToDateTime(temp_date);
                        bool Isadmin = (emailname == "amarnath.m@brandsystems.in" || emailname == "priyanka.goswami@brandsystems.in" || emailname == "peter.svahn@brandsystems.com" || emailname == "prabhudatta.tripathy@brandsystems.in") ? true : false;
                        PrevUpdates.showbutton = ((DateTime.UtcNow < lastUpdatedat.AddHours(24) && updates.CurrentVersion == PrevUpdates.Version && d.Item3 != "") && (Isadmin == true)) ? true : false;
                        ListPrevUpdates.Add(PrevUpdates);
                    }
                    updates.PreviousUpdates = ListPrevUpdates;
                    var tuple = Tuple.Create(updates, upgradetoolurl);
                    return tuple;
                }
            }
            catch
            {
                return null;
            }
        }

        private Tuple<IList, IList, string> getupgradedetsfromxml(string upgradetoolxmlPath, int instanceId)
        {
            try
            {
                XmlRootAttribute xRoot = new System.Xml.Serialization.XmlRootAttribute();
                xRoot.ElementName = "marcominstances";
                xRoot.IsNullable = true;
                XmlSerializer serializer = new XmlSerializer(typeof(marcominstances), xRoot);
                object obj;
                using (TextReader reader = new StreamReader(upgradetoolxmlPath + "UpdateTool.xml"))
                {
                    obj = serializer.Deserialize(reader);
                }
                marcominstances objmarcomInstances = (marcominstances)obj;
                var InstanceDetails = objmarcomInstances.instance.Where(a => a.Id == Convert.ToInt32(instanceId)).ToList();
                string lastUpdatedat = InstanceDetails[0].latestUpdateAt;

                //Get the new updates and previous updates..
                XmlRootAttribute xRoot_Version = new System.Xml.Serialization.XmlRootAttribute();
                xRoot.ElementName = "marcomVersions";
                xRoot.IsNullable = true;
                XmlSerializer serializer_Version = new XmlSerializer(typeof(marcomVersions), xRoot_Version);
                object obj_Ver;
                using (TextReader reader = new StreamReader(upgradetoolxmlPath + "marcomVersions.xml"))
                {
                    obj_Ver = serializer_Version.Deserialize(reader);
                }
                marcomVersions objmarcomVersions = (marcomVersions)obj_Ver;
                var versionDetails = objmarcomVersions;
                IList newUpdatesAvailable = objmarcomVersions.Version.Where(a => a.Id > Convert.ToInt32(InstanceDetails[0].CurrentVersionId)).ToList();
                IList updateHistory = objmarcomVersions.Version.Where(a => a.Id <= Convert.ToInt32(InstanceDetails[0].CurrentVersionId)).ToList();
                var d = newUpdatesAvailable;
                var tuple = Tuple.Create(newUpdatesAvailable, updateHistory, lastUpdatedat);
                return tuple;
            }
            catch (Exception)
            {
                return null;
            }
        }



        public IList<PasswordSetting> GetPasswordPolicyDetails(CommonManagerProxy proxy)
        {
            try
            {
                string xmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
                XDocument adminXmlDoc = XDocument.Load(xmlpath);
                string xelementName = "PasswordPolicy";
                var xelementFilepath = XElement.Load(xmlpath);
                var xmlElement = xelementFilepath.Element(xelementName);
                IList<PasswordSetting> pplist = new List<PasswordSetting>();
                PasswordSetting pp = new PasswordSetting();
                foreach (var des in xmlElement.Descendants())
                {
                    switch (des.Name.ToString())
                    {
                        case "minLength":
                            pp.MinLength = des.Value != "" ? Convert.ToInt32(des.Value) : 0;
                            break;
                        case "maxLength":
                            pp.MaxLength = des.Value != "" ? Convert.ToInt32(des.Value) : 0;
                            break;
                        case "numsLength":
                            pp.NumsLength = des.Value != "" ? Convert.ToInt32(des.Value) : 0;
                            break;
                        case "upperLength":
                            pp.UpperLength = des.Value != "" ? Convert.ToInt32(des.Value) : 0;
                            break;
                        case "specialLength":
                            pp.SpecialLength = des.Value != "" ? Convert.ToInt32(des.Value) : 0;
                            break;
                        case "barWidth":
                            pp.BarWidth = des.Value != "" ? Convert.ToInt32(des.Value) : 0;
                            break;
                        case "specialChars":
                            pp.SpecialChars = des.Value.ToString();
                            break;
                        case "useMultipleColors":
                            pp.MultipleColors = des.Value != "" ? Convert.ToInt32(des.Value) : 0;
                            break;
                        default:
                            break;
                    }
                }
                pplist.Add(pp);
                return pplist;
            }
            catch
            {
                return null;
            }
        }


        public bool UpdatePasswordPolicy(CommonManagerProxy proxy, string MinLength, string Maxlength, string Numlength, string UpperLength, string SpecialLength, string SpecialChars, string BarWidth, string MultipleColors)
        {
            try
            {
                string xmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
                string xelementName = "PasswordPolicy";
                var xelementFilepath = XElement.Load(xmlpath);
                var xmlElement = xelementFilepath.Element(xelementName);
                foreach (var des in xmlElement.Descendants())
                {
                    switch (des.Name.ToString())
                    {
                        case "minLength":
                            des.Value = MinLength.ToString();
                            break;
                        case "maxLength":
                            des.Value = Maxlength.ToString();
                            break;
                        case "numsLength":
                            des.Value = Numlength.ToString();
                            break;
                        case "upperLength":
                            des.Value = UpperLength.ToString();
                            break;
                        case "specialChars":
                            des.Value = SpecialChars.ToString();
                            break;
                        case "specialLength":
                            des.Value = SpecialLength.ToString();
                            break;
                        case "barWidth":
                            des.Value = BarWidth;
                            break;
                        case "useMultipleColors":
                            des.Value = MultipleColors;
                            break;
                        default:
                            break;
                    }
                }
                xelementFilepath.Save(xmlpath);
                return true;
            }
            catch
            {
                return false;
            }
        }



        public string GetOptimakerAddresspoints(CommonManagerProxy proxy)
        {
            try
            {
                return ConfigurationManager.AppSettings["CreateDOCGeneratedAsset"] + "," + ConfigurationManager.AppSettings["CreateversionDOCGeneratedAsset"];
            }
            catch
            {
                return "";
            }


        }

        public bool IsAvailableAsset(CommonManagerProxy proxy, int AssetID)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    var aseetinto = tx.PersistenceManager.CommonRepository.Query<AssetsDao>().Where(a => a.ID == AssetID).Select(a => a.ID).ToList();
                    if (aseetinto.Count > 0)
                        return true;
                    else
                        return false;

                }

            }
            catch
            {
                return false;

            }
        }

        //Get include plan tabs using GetPlantabsettings
        public Hashtable GetPlantabsettings(CommonManagerProxy proxy)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    Hashtable hashcol = new Hashtable();
                    string xmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
                    XDocument adminXmlDoc = XDocument.Load(xmlpath);
                    hashcol["Financials"] = adminXmlDoc.Root.Descendants("PlanTabSetting").Descendants("Financials").Select(a => a.Value).FirstOrDefault();
                    hashcol["Objectives"] = adminXmlDoc.Root.Descendants("PlanTabSetting").Descendants("Objectives").Select(a => a.Value).FirstOrDefault();
                    return hashcol;
                }

            }
            catch
            {


            }
            return null;
        }

        //Updatee plan tabs using UpdatePlanTabIds passing typeID
        public bool UpdatePlanTabsettings(CommonManagerProxy proxy, string jsondata)
        {

            string xmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
            XDocument adminXmlDoc = XDocument.Load(xmlpath);
            adminXmlDoc.Descendants("PlanTabSetting").Remove();
            XElement xmlTree = XElement.Parse("<PlanTabSetting>" + jsondata + "</PlanTabSetting>");
            adminXmlDoc.Element("AppSettings").Add(xmlTree);
            adminXmlDoc.Save(xmlpath);
            return true;

        }

        public IList<ITabEncryption> GetCustomTabEncryptionByID(CommonManagerProxy proxy)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {

                    IList<TabEncryptionDao> tabCollection = new List<TabEncryptionDao>();
                    tabCollection = tx.PersistenceManager.CommonRepository.Query<TabEncryptionDao>().Cast<TabEncryptionDao>().ToList();

                    if (tabCollection != null)
                    {
                        IList<ITabEncryption> CustomtabEncryplist = new List<ITabEncryption>();
                        foreach (var CurrentTabdata in tabCollection.ToList())
                        {
                            CustomtabEncryplist.Add(new TabEncryption
                            {
                                ID = CurrentTabdata.ID,
                                CustomTabID = CurrentTabdata.CustomTabID,
                                EncryKey = CurrentTabdata.EncryKey,
                                EncryIV = CurrentTabdata.EncryIV,
                                Algorithm = CurrentTabdata.Algorithm,
                                PaddingMode = CurrentTabdata.PaddingMode,
                                CipherMode = CurrentTabdata.CipherMode
                            });
                        }
                        return CustomtabEncryplist;

                    }

                }
            }
            catch
            {

            }
            return null;

        }

        public IList<ICustomTabEntityTypeAcl> GetCustomTabAccessByID(CommonManagerProxy proxy, int TypeID)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {

                    IList<ICustomTabEntityTypeAcl> entityaccessCollection = new List<ICustomTabEntityTypeAcl>();

                    var entitytypeCollections = tx.PersistenceManager.CommonRepository.Query<CustomTabEntityTypeRoleAclDao>().Where(a => a.CustomTabID == TypeID);

                    if (entitytypeCollections.Count() > 0)
                    {
                        foreach (var obj in entitytypeCollections)
                        {
                            entityaccessCollection.Add(new CustomTabEntityTypeAcl { ID = obj.ID, CustomTabID = obj.CustomTabID, EntityTypeID = obj.EntityTypeID, GlobalRoleID = obj.GlobalRoleID });
                        }
                        return entityaccessCollection;
                    }
                    return null;
                }
            }
            catch
            {

            }
            return null;

        }


        public IList<ICustomTab> GetCustomEntityTabsfrCalID(CommonManagerProxy proxy, int TypeID, int CalID)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    //IList<CustomTabDao> tabCollection = new List<CustomTabDao>();
                    int[] FeatureCollections = tx.PersistenceManager.CommonRepository.Query<FeatureDao>().Where(a => (Modules)a.ModuleID == Modules.Planning).Select(a => a.Id).ToArray();
                    int[] FeaturePermission = proxy.MarcomManager.User.ListOfUserGlobalRoles.Where(a => a.AccessPermission == true && (FeatureCollections.Contains(a.Featureid)) && a.Moduleid == (int)Modules.Planning).Select(a => a.Featureid).ToArray();
                    if (FeaturePermission.Length > 0)
                    {
                        StringBuilder sbquery = new StringBuilder();
                        sbquery.AppendLine("SELECT");
                        sbquery.AppendLine("ID,");
                        sbquery.AppendLine("Name,");
                        sbquery.AppendLine("TypeID,");
                        sbquery.AppendLine(" AddEntityID,");
                        sbquery.AppendLine(" AddLanguageCode,");
                        sbquery.AppendLine("AddUserEmail,");
                        sbquery.AppendLine(" AddUserID,");
                        sbquery.AppendLine("AddUserName,");
                        sbquery.AppendLine("ExternalUrl,");
                        sbquery.AppendLine("IsSytemDefined,");
                        sbquery.AppendLine("SortOrder,");
                        sbquery.AppendLine(" ControleID");
                        sbquery.AppendLine("FROM CM_CustomTabs CC");
                        sbquery.AppendLine("WHERE CC.ID IN (SELECT");
                        sbquery.AppendLine("CustomTabID");
                        sbquery.AppendLine("FROM PM_CalenderTab");
                        sbquery.AppendLine("WHERE CalenderID =" + CalID + ") AND CC.TypeID =" + TypeID);
                        var tabCollection = tx.PersistenceManager.PlanningRepository.ExecuteQuery(sbquery.ToString()).Cast<Hashtable>().ToList();

                        //tabCollection = tx.PersistenceManager.CommonRepository.Query<CustomTabDao>().Where(a => a.Typeid == TypeID && (FeaturePermission.Contains(a.FeatureID) || a.FeatureID == 0)).OrderBy(a => a.SortOrder).Cast<CustomTabDao>().ToList();
                        if (tabCollection != null)
                        {
                            IList<ICustomTab> Customtablist = new List<ICustomTab>();
                            foreach (var CurrentTabdata in tabCollection.ToList())
                            {
                                Customtablist.Add(new CustomTab
                                {
                                    Id = Convert.ToInt32(CurrentTabdata["ID"]),
                                    Name = CurrentTabdata["Name"].ToString(),
                                    Typeid = Convert.ToInt32(CurrentTabdata["Typeid"]),
                                    AddEntityID = (bool)(CurrentTabdata["AddEntityID"]),
                                    AddLanguageCode = (bool)(CurrentTabdata["AddLanguageCode"]),
                                    AddUserEmail = (bool)(CurrentTabdata["AddUserEmail"]),
                                    AddUserID = (bool)(CurrentTabdata["AddUserID"]),
                                    AddUserName = (bool)(CurrentTabdata["AddUserName"]),
                                    ExternalUrl = CurrentTabdata["ExternalUrl"].ToString(),
                                    IsSytemDefined = (bool)(CurrentTabdata["IsSytemDefined"]),
                                    SortOrder = Convert.ToInt32(CurrentTabdata["SortOrder"]),
                                    ControleID = CurrentTabdata["ControleID"].ToString()
                                });
                            }
                            return Customtablist;
                        }
                    }
                }
            }
            catch
            {
            }
            return null;

        }
        public string GetAdminLayoutSettings(CommonManagerProxy proxy, string LogoSettings, int typeid)
        {
            if (typeid != 0)
            {
                // string xmlpath =  AppDomain.CurrentDomain.BaseDirectory +"\\AdminSettings.xml";
                string xmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
                XDocument adminXdoc = XDocument.Load(xmlpath);
                var result = adminXdoc.Descendants("LayoutView").Select(a => a).ToList();
                var abc = result.Descendants(LogoSettings).ToList();
                var xElementResult = result;
                string jsonText = JsonConvert.SerializeObject(result[0]);
                return jsonText;
            }
            else
            {
                string xmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
                XDocument adminXdoc = XDocument.Load(xmlpath);
                var result = adminXdoc.Descendants(LogoSettings).Select(a => a).ToList();
                var xElementResult = result[0];
                string jsonText = JsonConvert.SerializeObject(result[0]);
                return jsonText;
            }

        }
        public string GetAdminLayoutFinSettings(CommonManagerProxy proxy, string LogoSettings, int typeid)
        {
            if (typeid != 0)
            {
                // string xmlpath =  AppDomain.CurrentDomain.BaseDirectory +"\\AdminSettings.xml";
                string xmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
                XDocument adminXdoc = XDocument.Load(xmlpath);
                var result = adminXdoc.Descendants("LayoutFinanicalView").Select(a => a).ToList();
                var abc = result.Descendants(LogoSettings).ToList();
                var xElementResult = result;
                string jsonText = JsonConvert.SerializeObject(result[0]);
                return jsonText;
            }
            else
            {
                string xmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
                XDocument adminXdoc = XDocument.Load(xmlpath);
                var result = adminXdoc.Descendants(LogoSettings).Select(a => a).ToList();
                var xElementResult = result[0];
                string jsonText = JsonConvert.SerializeObject(result[0]);
                return jsonText;
            }

        }

        public string GetAdminLayoutObjectiveSettings(CommonManagerProxy proxy, string LogoSettings, int typeid)
        {
            if (typeid != 0)
            {
                // string xmlpath =  AppDomain.CurrentDomain.BaseDirectory +"\\AdminSettings.xml";
                string xmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
                XDocument adminXdoc = XDocument.Load(xmlpath);
                var result = adminXdoc.Descendants("LayoutObjectiveView").Select(a => a).ToList();
                var abc = result.Descendants(LogoSettings).ToList();
                var xElementResult = result;
                string jsonText = JsonConvert.SerializeObject(result[0]);
                return jsonText;
            }
            else
            {
                string xmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
                XDocument adminXdoc = XDocument.Load(xmlpath);
                var result = adminXdoc.Descendants(LogoSettings).Select(a => a).ToList();
                var xElementResult = result[0];
                string jsonText = JsonConvert.SerializeObject(result[0]);
                return jsonText;
            }

        }

        public bool LayoutDesign(CommonManagerProxy proxy, string jsondata, string key, int typeid)
        {

            dynamic jsObject = JsonConvert.DeserializeObject(jsondata);
            JProperty jprop = new JProperty(jsondata);
            // string xmlpath = AppDomain.CurrentDomain.BaseDirectory + "\\AdminSettings.xml";
            string xmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
            XDocument adminXmlDoc = XDocument.Load(xmlpath);
            //The Key is root node current Settings
            string xelementName = key;
            var xelementFilepath = XElement.Load(xmlpath);
            var xmlElement = xelementFilepath.Descendants("LayoutoverDesigns").Descendants("RootLevel").Where(a => Convert.ToInt32(a.Attribute("typeid").Value) == typeid).Select(a => a);
            if (xmlElement != null)
            {
                adminXmlDoc.Descendants(xelementName).Remove();
                adminXmlDoc.Save(xmlpath);
                XDocument xDocparse = JsonConvert.DeserializeXNode(jsondata);
                var vae = Convert.ToString(xDocparse);
                adminXmlDoc.Element("AppSettings").Add(xDocparse.Nodes().ElementAt(0));
                adminXmlDoc.Save(xmlpath);

                //var xDocparse = JsonConvert.DeserializeXNode(jsondata);
                //XElement el = xDocparse.Descendants(key).Elements("EntityType").FirstOrDefault();


                //adminXmlDoc.Descendants("LayoutoverDesigns").Descendants("RootLevel").Where(a => Convert.ToInt32(a.Attribute("typeid").Value) == typeid).Select(a => a).Descendants(xelementName).Remove();

                //XElement subjectElement2 = xDocparse.Descendants(key).FirstOrDefault();
                //adminXmlDoc.Descendants("LayoutoverDesigns").Descendants("RootLevel").Where(a => Convert.ToInt32(a.Attribute("typeid").Value) == typeid).FirstOrDefault().Add(subjectElement2);

                //adminXmlDoc.Save(xmlpath);
            }
            else if (xmlElement == null)
            {

                XDocument xDocparse = JsonConvert.DeserializeXNode(jsondata);
                var vae = Convert.ToString(xDocparse);
                adminXmlDoc.Element("AppSettings").Add(xDocparse.Nodes().ElementAt(0));
                adminXmlDoc.Save(xmlpath);
            }

            return true;
        }


        public IList<IAttribute> GetAttributeSearchCriteria(CommonManagerProxy proxy, int EntityTypeID)
        {
            try
            {

                int[] entitytypeids = null;
                int version = MarcomManagerFactory.AdminMetadataVersionNumber;
                IList<IAttribute> _iiAttribute = new List<IAttribute>();
                IList<AttributeDao> dao = new List<AttributeDao>();
                IList<EntityTypeAttributeRelationDao> entityTypeRealtionDao = new List<EntityTypeAttributeRelationDao>();
                IList<AttributeDao> attributesDao = new List<AttributeDao>();

                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {

                    int[] systemAttributes = { Convert.ToInt32(SystemDefinedAttributes.MyRoleGlobalAccess), Convert.ToInt32(SystemDefinedAttributes.EntityStatus),
                                                    Convert.ToInt32(SystemDefinedAttributes.MyRoleEntityAccess),Convert.ToInt32(SystemDefinedAttributes.EntityOnTimeStatus),
                                                    Convert.ToInt32(SystemDefinedAttributes.ObjectiveStatus),Convert.ToInt32(SystemDefinedAttributes.Owner),
                                                    Convert.ToInt32(SystemDefinedAttributes.Name),Convert.ToInt32(SystemDefinedAttributes.Taskname),
                                                    Convert.ToInt32(SystemDefinedAttributes.Taskdescription),Convert.ToInt32(SystemDefinedAttributes.Notes),
                                                    Convert.ToInt32(SystemDefinedAttributes.Taskduedate)
                                                 };
                    int[] tasktypeIds = tx.PersistenceManager.MetadataRepository.Query<EntitytasktypeDao>().Select(a => a.EntitytypeId).ToArray();

                    if (EntityTypeID == 1)
                    {
                        entitytypeids = tx.PersistenceManager.MetadataRepository.Query<EntityTypeDao>().Where(a => a.ModuleID == 3 && a.Category == 2 && !tasktypeIds.Contains(a.Id)).Select(a => a.Id).ToArray();
                        var llist = entitytypeids;
                    }
                    else if (EntityTypeID == 2)
                    {
                        entitytypeids = tx.PersistenceManager.MetadataRepository.Query<EntitytasktypeDao>().Select(a => a.EntitytypeId).ToArray();

                    }
                    else if (EntityTypeID == 3)
                    {
                        entitytypeids = tx.PersistenceManager.MetadataRepository.Query<EntityTypeDao>().Where(a => a.ModuleID == 5).Select(a => a.Id).ToArray();
                    }
                    if (version == 0)
                    {
                        entityTypeRealtionDao = tx.PersistenceManager.MetadataRepository.Query<EntityTypeAttributeRelationDao>().Where(a => entitytypeids.Contains(a.EntityTypeID)).ToList();
                        var atrsdao1 = tx.PersistenceManager.MetadataRepository.Query<AttributeDao>().Select(a => a).ToList<AttributeDao>();
                        var atrsdao = atrsdao1.Where(a => !systemAttributes.Contains(a.Id)).ToList();
                        attributesDao = atrsdao.Join(entityTypeRealtionDao, a => a.Id, b => b.AttributeID, (ab, bc) =>
                         new { ab, bc }).Where(a => (a.ab.AttributeTypeID == Convert.ToInt32(AttributesList.ListSingleSelection) || a.ab.AttributeTypeID == Convert.ToInt32(AttributesList.ListMultiSelection)) && entitytypeids.Contains(a.bc.EntityTypeID)).Select
                            (a => a.ab).Distinct().ToList();
                        tx.Commit();
                    }
                    else
                    {
                        string xmlpath = GetXmlWorkingPath();
                        entityTypeRealtionDao = tx.PersistenceManager.MetadataRepository.GetObject<EntityTypeAttributeRelationDao>(xmlpath).Where(a => entitytypeids.Contains(a.EntityTypeID)).ToList();
                        var atrsdao1 = tx.PersistenceManager.MetadataRepository.GetObject<AttributeDao>(xmlpath).Select(a => a).ToList<AttributeDao>();
                        var atrsdao = atrsdao1.Where(a => !systemAttributes.Contains(a.Id)).ToList();
                        attributesDao = atrsdao.Join(entityTypeRealtionDao, a => a.Id, b => b.AttributeID, (ab, bc) =>
                         new { ab, bc }).Where(a => (a.ab.AttributeTypeID == Convert.ToInt32(AttributesList.ListSingleSelection) || a.ab.AttributeTypeID == Convert.ToInt32(AttributesList.ListMultiSelection)) && entitytypeids.Contains(a.bc.EntityTypeID)).Select
                            (a => a.ab).Distinct().ToList();

                        tx.Commit();
                    }
                }
                foreach (var item in attributesDao)
                {
                    IAttribute _iAttribute = new BrandSystems.Marcom.Core.Metadata.Attribute();
                    _iAttribute.Caption = item.Caption;
                    _iAttribute.Description = item.Description;
                    _iAttribute.AttributeTypeID = item.AttributeTypeID;
                    _iAttribute.IsSystemDefined = item.IsSystemDefined;
                    _iAttribute.IsSpecial = item.IsSpecial;
                    _iAttribute.Id = item.Id;
                    _iiAttribute.Add(_iAttribute);
                }
                return _iiAttribute;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public int[] GetSearchCriteriaTypesIds(CommonManagerProxy proxy, int searchtype)
        {
            try
            {

                int[] entitytypeids = null;
                int version = MarcomManagerFactory.AdminMetadataVersionNumber;

                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {

                    int[] systemAttributes = { Convert.ToInt32(SystemDefinedAttributes.MyRoleGlobalAccess), Convert.ToInt32(SystemDefinedAttributes.EntityStatus),
                                                    Convert.ToInt32(SystemDefinedAttributes.MyRoleEntityAccess),Convert.ToInt32(SystemDefinedAttributes.EntityOnTimeStatus),
                                                    Convert.ToInt32(SystemDefinedAttributes.ObjectiveStatus),Convert.ToInt32(SystemDefinedAttributes.Owner),
                                                    Convert.ToInt32(SystemDefinedAttributes.Name),Convert.ToInt32(SystemDefinedAttributes.Taskname),
                                                    Convert.ToInt32(SystemDefinedAttributes.Taskdescription),Convert.ToInt32(SystemDefinedAttributes.Notes),
                                                    Convert.ToInt32(SystemDefinedAttributes.Taskduedate)
                                                 };
                    int[] tasktypeIds = tx.PersistenceManager.MetadataRepository.Query<EntitytasktypeDao>().Select(a => a.EntitytypeId).ToArray();

                    if (searchtype == 1)
                    {
                        if (tasktypeIds != null)
                            entitytypeids = tx.PersistenceManager.MetadataRepository.Query<EntityTypeDao>().Where(a => a.ModuleID == 3 && a.Category == 2 && !tasktypeIds.Contains(a.Id)).Select(a => a.Id).ToArray();
                        else
                            entitytypeids = tx.PersistenceManager.MetadataRepository.Query<EntityTypeDao>().Where(a => a.ModuleID == 3 && a.Category == 2).Select(a => a.Id).ToArray();
                        var llist = entitytypeids;
                    }
                    else if (searchtype == 2)
                    {
                        entitytypeids = tx.PersistenceManager.MetadataRepository.Query<EntitytasktypeDao>().Select(a => a.EntitytypeId).ToArray();

                    }
                    else if (searchtype == 3)
                    {
                        entitytypeids = tx.PersistenceManager.MetadataRepository.Query<EntityTypeDao>().Where(a => a.ModuleID == 5).Select(a => a.Id).ToArray();
                    }
                    tx.Commit();
                }

                return entitytypeids;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public string GetXmlWorkingPath()
        {
            string mappingfilesPath = AppDomain.CurrentDomain.BaseDirectory;
            if (!MarcomManagerFactory.viewOldMetadataVersion)
            {
                if (MarcomManagerFactory.IsWorkingWithCurrentWorkingVersion)
                    mappingfilesPath = mappingfilesPath + "MetadataXML" + @"\CurrentMetadataWorking.xml";
                else
                    mappingfilesPath = mappingfilesPath + "MetadataXML" + @"\FutureMetadataWorking.xml";
            }
            else
                mappingfilesPath = mappingfilesPath + "MetadataXML" + @"\MetadataVersion_V" + MarcomManagerFactory.oldMetadataVersionNumber + ".xml";
            if (System.IO.File.Exists(mappingfilesPath))
                return mappingfilesPath;
            else
                return null;
        }


        public bool SearchadminSettingsforRootLevelInsertUpdate(CommonManagerProxy proxy, string jsondata, string LogoSettings, string key, int typeid)
        {

            dynamic jsObject = JsonConvert.DeserializeObject(jsondata);
            JProperty jprop = new JProperty(jsondata);
            // string xmlpath = AppDomain.CurrentDomain.BaseDirectory + "\\AdminSettings.xml";
            string xmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
            XDocument adminXmlDoc = XDocument.Load(xmlpath);
            var Defaultrootssetting = adminXmlDoc.Descendants("Productionsettings").FirstOrDefault();
            var DefaultLogoSettings = adminXmlDoc.Descendants("Productionsettings").Descendants(LogoSettings).FirstOrDefault();
            if (Defaultrootssetting == null && DefaultLogoSettings == null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<Productionsettings><" + LogoSettings + "></" + LogoSettings + "></Productionsettings>");
                XElement.Parse(sb.ToString());
                adminXmlDoc.Root.Add(XElement.Parse(sb.ToString()));
                adminXmlDoc.Save(xmlpath);

            }
            else if (DefaultLogoSettings == null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<" + LogoSettings + "></" + LogoSettings + ">");
                adminXmlDoc.Element("AppSettings").Descendants("Productionsettings").FirstOrDefault().Add(XElement.Parse(sb.ToString()));
                adminXmlDoc.Save(xmlpath);
            }


            string xelementName = key;
            var xelementFilepath = XElement.Load(xmlpath);
            var xmlElement = xelementFilepath.Descendants("Productionsettings").Descendants(LogoSettings).Descendants("ProductionType").FirstOrDefault();
            //var xmlElement = xelementFilepath.Descendants("DAMsettings").Descendants(LogoSettings).Descendants("AssetType").Where(a => Convert.ToInt32(a.Attribute("ID").Value) == typeid).Select(a => a);
            if (xmlElement != null)
            {
                var xDocparse = JsonConvert.DeserializeXNode(jsondata);
                XElement el = xDocparse.Elements("ProductionType").FirstOrDefault();
                if (el != null)
                {
                    el.SetAttributeValue("ID", typeid);
                }

                // var xmlElement1 = xmlElement.sless(a => Convert.ToInt32(a.Value) == typeid);
                var xmlElement1 = xelementFilepath.Descendants("Productionsettings").Descendants(LogoSettings).Descendants("ProductionType").Where(a => Convert.ToInt32(a.Attribute("ID").Value) == typeid).FirstOrDefault();
                if (xmlElement1 != null)
                {
                    adminXmlDoc.Descendants("Productionsettings").Descendants(LogoSettings).Descendants("ProductionType").Where(a => Convert.ToInt32(a.Attribute("ID").Value) == typeid).Remove();
                    XElement e2 = xDocparse.Elements("ProductionType").Descendants("Attributes").FirstOrDefault();
                    if (e2.IsEmpty == false)
                    {
                        adminXmlDoc.Element("AppSettings").Descendants("Productionsettings").Descendants(LogoSettings).FirstOrDefault().Add(xDocparse.Nodes().ElementAt(0));

                    }
                    adminXmlDoc.Save(xmlpath);
                }
                else
                {
                    adminXmlDoc.Element("AppSettings").Descendants("Productionsettings").Descendants(LogoSettings).FirstOrDefault().Add(xDocparse.Nodes().ElementAt(0));
                    adminXmlDoc.Save(xmlpath);
                }



            }
            else if (xmlElement == null)
            {

                XDocument xDocparse = JsonConvert.DeserializeXNode(jsondata);
                XElement el = xDocparse.Elements("ProductionType").FirstOrDefault();
                XElement e2 = xDocparse.Elements("ProductionType").Descendants("Attributes").FirstOrDefault();
                if (el != null)
                {
                    el.SetAttributeValue("ID", typeid);
                }
                var vae = Convert.ToString(xDocparse);
                if (e2.IsEmpty == false)
                {
                    adminXmlDoc.Element("AppSettings").Descendants("Productionsettings").Descendants(LogoSettings).FirstOrDefault().Add(xDocparse.Nodes().ElementAt(0));
                    //adminXmlDoc.Descendants("ListSettings").Descendants("RootLevel").Where(a => Convert.ToInt32(a.Attribute("typeid").Value) == typeid).FirstOrDefault()
                    adminXmlDoc.Save(xmlpath);
                }
            }

            return true;
        }

        public IList<ICustomTab> GetCustomEntityTabsByTypeID(CommonManagerProxy proxy, int TypeID)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {

                    IList<CustomTabDao> tabCollection = new List<CustomTabDao>();

                    int[] FeatureCollections = tx.PersistenceManager.CommonRepository.Query<FeatureDao>().Where(a => (Modules)a.ModuleID == Modules.Planning).Select(a => a.Id).ToArray();
                    int[] FeaturePermission = proxy.MarcomManager.User.ListOfUserGlobalRoles.Where(a => a.AccessPermission == true && (FeatureCollections.Contains(a.Featureid)) && a.Moduleid == (int)Modules.Planning).Select(a => a.Featureid).ToArray();

                    if (FeaturePermission.Length > 0)
                    {
                        tabCollection = tx.PersistenceManager.CommonRepository.Query<CustomTabDao>().Where(a => a.Typeid == TypeID && (FeaturePermission.Contains(a.FeatureID) || a.FeatureID == 0)).OrderBy(a => a.SortOrder).Cast<CustomTabDao>().ToList();
                        if (tabCollection != null)
                        {
                            IList<ICustomTab> Customtablist = new List<ICustomTab>();
                            foreach (var CurrentTabdata in tabCollection.ToList())
                            {
                                Customtablist.Add(new CustomTab
                                {
                                    Id = CurrentTabdata.ID,
                                    Name = CurrentTabdata.Name,
                                    Typeid = CurrentTabdata.Typeid,
                                    AddEntityID = CurrentTabdata.AddEntityID,
                                    AddLanguageCode = CurrentTabdata.AddLanguageCode,
                                    AddUserEmail = CurrentTabdata.AddUserEmail,
                                    AddUserID = CurrentTabdata.AddUserID,
                                    AddUserName = CurrentTabdata.AddUserName,
                                    ExternalUrl = CurrentTabdata.ExternalUrl,
                                    IsSytemDefined = CurrentTabdata.IsSytemDefined,
                                    SortOrder = CurrentTabdata.SortOrder,
                                    ControleID = CurrentTabdata.ControleID
                                });
                            }
                            return Customtablist;

                        }
                    }
                }
            }
            catch
            {

            }
            return null;

        }


        public bool LayoutSettingsApplyChanges(CommonManagerProxy proxy, string TabType, string TabLocation)
        {
            try
            {
                LayoutSettings(TabType, TabLocation);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }

        }


        private void LayoutSettings(string layouttype, string sectiontype)
        {
            try
            {
                //variable declarations
                Dictionary<int, String> OverViewSections = new Dictionary<int, String>();
                string OutputAppendingFile = string.Empty;
                string htmltemplatepaths = string.Empty;
                string srcfilename = string.Empty;
                string entitytype = string.Empty;
                XmlDocument XmlSettingsDoc = new XmlDocument();

                switch (layouttype)
                {
                    case "Plans":
                        switch (sectiontype)
                        {
                            case "Overview":
                                //add all the sections with the ids to dictonary
                                OverViewSections.Add(1, "1_Header.cshtml");
                                OverViewSections.Add(100, "3_NewsFeed.cshtml");
                                OverViewSections.Add(101, "4_EntityStatus.cshtml");
                                OverViewSections.Add(102, "5_TaskSummary.cshtml");
                                OverViewSections.Add(103, "6_Milestone.cshtml");
                                OverViewSections.Add(104, "7_Financial.cshtml");
                                OverViewSections.Add(105, "8_Details.cshtml");
                                OverViewSections.Add(2, "2_Footer.cshtml");
                                //load the xml settings
                                XmlSettingsDoc.Load(Path.Combine(HttpRuntime.AppDomainAppPath, @"Layouts\Plans\Overview\plan_overview.xml"));
                                //set the appending file
                                OutputAppendingFile = Path.Combine(HttpRuntime.AppDomainAppPath, @"Layouts\Plans\Overview\9_Overview.cshtml");
                                htmltemplatepaths = @"Layouts\Plans\Overview\";
                                srcfilename = "overview.cshtml"; entitytype = "Plans";
                                break;
                            case "Financial":
                                //add all the sections with the ids to dictonary
                                OverViewSections.Add(1, "1_Header.html");
                                OverViewSections.Add(106, "3_FundingCostCentre.html");
                                OverViewSections.Add(107, "4_FinancialSummary.html");
                                OverViewSections.Add(108, "5_FinancialDetails.html");
                                OverViewSections.Add(109, "6_FinanciaForecast.html");
                                OverViewSections.Add(2, "2_Footer.html");
                                //load the xml settings
                                XmlSettingsDoc.Load(Path.Combine(HttpRuntime.AppDomainAppPath, @"Layouts\Plans\Financial\plan_financial.xml"));
                                //set the appending file
                                OutputAppendingFile = Path.Combine(HttpRuntime.AppDomainAppPath, @"Layouts\Plans\Financial\7_Financial.cshtml");
                                htmltemplatepaths = @"Layouts\Plans\Financial\";
                                srcfilename = "Financial.cshtml"; entitytype = "Plans";
                                break;
                            case "Objective":
                                //add all the sections with the ids to dictonary
                                OverViewSections.Add(1, "1_Header.html");
                                OverViewSections.Add(110, "2_Predefined.html");
                                OverViewSections.Add(111, "3_AdditionalObjectives.html");
                                OverViewSections.Add(2, "4_Footer.html");
                                //load the xml settings
                                XmlSettingsDoc.Load(Path.Combine(HttpRuntime.AppDomainAppPath, @"Layouts\Plans\Objective\plan_objective.xml"));
                                //set the appending file
                                OutputAppendingFile = Path.Combine(HttpRuntime.AppDomainAppPath, @"Layouts\Plans\Objective\5_Objective.html");
                                htmltemplatepaths = @"Layouts\Plans\Objective\";
                                srcfilename = "objective.html"; entitytype = "Plans";
                                break;
                        }
                        break;
                    case "Cost Centre":
                        switch (sectiontype)
                        {
                            case "Overview":
                                //add all the sections with the ids to dictonary
                                OverViewSections.Add(1, "1_Header.cshtml");
                                OverViewSections.Add(112, "2_NewsFeed.cshtml");
                                OverViewSections.Add(113, "3_EntityStatus.cshtml");
                                OverViewSections.Add(114, "4_TaskSummary.cshtml");
                                OverViewSections.Add(115, "5_FinancialSummary.cshtml");
                                OverViewSections.Add(116, "6_Details.cshtml");
                                OverViewSections.Add(2, "7_Footer.cshtml");
                                //load the xml settings
                                XmlSettingsDoc.Load(Path.Combine(HttpRuntime.AppDomainAppPath, @"Layouts\CostCentre\Overview\costcentre_overview.xml"));
                                //set the appending file
                                OutputAppendingFile = Path.Combine(HttpRuntime.AppDomainAppPath, @"Layouts\CostCentre\Overview\8_Overview.cshtml");
                                htmltemplatepaths = @"Layouts\CostCentre\Overview\";
                                srcfilename = "overview.cshtml"; entitytype = "CostCentre";
                                break;
                        }
                        break;
                    case "Objective":
                        switch (sectiontype)
                        {
                            case "Overview":
                                //add all the sections with the ids to dictonary
                                OverViewSections.Add(1, "1_Header.html");
                                OverViewSections.Add(117, "2_NewsFeed.html");
                                OverViewSections.Add(118, "3_ObjectivesAssignments.html");
                                OverViewSections.Add(119, "4_TaskSummary.html");
                                OverViewSections.Add(120, "5_ObjectiveSummary.html");
                                OverViewSections.Add(121, "6_ObjectiveFulfillment.html");
                                OverViewSections.Add(122, "7_Details.html");
                                OverViewSections.Add(2, "8_Footer.html");
                                //load the xml settings
                                XmlSettingsDoc.Load(Path.Combine(HttpRuntime.AppDomainAppPath, @"Layouts\Objectives\Overview\objectives_overview.xml"));
                                //set the appending file
                                OutputAppendingFile = Path.Combine(HttpRuntime.AppDomainAppPath, @"Layouts\Objectives\Overview\9_Overview.html");
                                htmltemplatepaths = @"Layouts\Objectives\Overview\";
                                srcfilename = "overview.html"; entitytype = "Objectives";
                                break;
                        }
                        break;
                }


                //clear the contents of the cshtml template file
                System.IO.File.WriteAllText(OutputAppendingFile, string.Empty);
                //form the HEADER for overview cshtml
                WriteContentsToFile(string.Empty, OutputAppendingFile, Path.Combine(HttpRuntime.AppDomainAppPath, htmltemplatepaths) + OverViewSections[1]);


                if (!(XmlSettingsDoc == null))
                {
                    XmlNodeList rowlist = XmlSettingsDoc.SelectNodes("root/block");
                    int rowno = 0;

                    // Loop through the nodelist 
                    for (int i = 0; i < rowlist.Count; i++)
                    {
                        rowno = i + 1;
                        //add row (rowfluid div)
                        WriteContentsToFile(@"<div id=""RowDiv" + rowno + @""" class=""row-fluid"">", OutputAppendingFile, string.Empty);

                        // Set for each element in the nodelist to XmlNode
                        XmlNode node = rowlist[i];

                        // Get the node's attributes
                        XmlAttributeCollection attCol = node.Attributes;

                        XmlNodeList collist = node.SelectNodes("lists");
                        //loop through each column (span)
                        foreach (XmlNode col in collist)
                        {
                            //get column width (span width)
                            string spanvalue = col["BlockWidth"].InnerText;
                            string spantag = @"<div class=""span" + spanvalue + @""">";
                            //draw span column
                            if (spanvalue == "12")
                            {
                                spantag = @"<div class=""span" + spanvalue + @" nomargin"">";
                            }
                            WriteContentsToFile(spantag, OutputAppendingFile, string.Empty);
                            //get all sections in this column
                            XmlNodeList sectionlist = col.SelectNodes("list");
                            //loop through each section and form the section
                            foreach (XmlNode section in sectionlist)
                            {
                                //get the section id
                                string sectionid = section["listUniqueID"].InnerText;
                                string inputfilepath = Path.Combine(HttpRuntime.AppDomainAppPath, htmltemplatepaths) + OverViewSections[int.Parse(sectionid)];
                                //WriteContentsToFile(sectionid, OverviewAppendingFile, string.Empty);
                                WriteContentsToFile(string.Empty, OutputAppendingFile, inputfilepath);
                            }

                            //end span column
                            WriteContentsToFile("</div>", OutputAppendingFile, string.Empty);
                        }


                        //end row fluid div
                        WriteContentsToFile("</div>", OutputAppendingFile, string.Empty);
                    }

                }

                //form the FOOTER for overview cshtml
                WriteContentsToFile(string.Empty, OutputAppendingFile, Path.Combine(HttpRuntime.AppDomainAppPath, htmltemplatepaths) + OverViewSections[2]);

                //Take backup of existing files and move the new file
                BackupMoveLayoutFiles(OutputAppendingFile, srcfilename, entitytype);
            }
            catch (Exception e)
            {
            }
        }

        private void BackupMoveLayoutFiles(string srcpath, string srcfilename, string entitytype)
        {
            try
            {
                //create backup folder with time stamp
                string backupfolderName = string.Empty;
                string destpath = string.Empty;

                //take backup of existing files
                switch (entitytype)
                {
                    case "Plans":
                        destpath = HttpRuntime.AppDomainAppPath + @"views\mui\planningtool\default\detail\section\" + srcfilename;
                        backupfolderName = Path.Combine(HttpRuntime.AppDomainAppPath, "LayoutBackup") + @"\Layoutsbackup_Plans" + DateTime.Now.ToString("HH-mm-ss-fff");
                        System.IO.File.Copy(Path.Combine(HttpRuntime.AppDomainAppPath, @"views\mui\planningtool\default\detail\section\overview.cshtml"), backupfolderName + "overview.cshtml", true);
                        System.IO.File.Copy(Path.Combine(HttpRuntime.AppDomainAppPath, @"views\mui\planningtool\default\detail\section\Financial.cshtml"), backupfolderName + "Financial.cshtml", true);
                        System.IO.File.Copy(Path.Combine(HttpRuntime.AppDomainAppPath, @"views\mui\planningtool\default\detail\section\objective.html"), backupfolderName + "objective.html", true);
                        break;
                    case "CostCentre":
                        backupfolderName = Path.Combine(HttpRuntime.AppDomainAppPath, "LayoutBackup") + @"\Layoutsbackup_CostCentre" + DateTime.Now.ToString("HH-mm-ss-fff");
                        System.IO.File.Copy(Path.Combine(HttpRuntime.AppDomainAppPath, @"views\mui\planningtool\costcentre\detail\section\overview.cshtml"), backupfolderName + "overview.cshtml", true);
                        destpath = HttpRuntime.AppDomainAppPath + @"views\mui\planningtool\costcentre\detail\section\" + srcfilename;
                        break;
                    case "Objectives":
                        destpath = HttpRuntime.AppDomainAppPath + @"views\mui\planningtool\objective\detail\section\" + srcfilename;
                        backupfolderName = Path.Combine(HttpRuntime.AppDomainAppPath, "LayoutBackup") + @"\Layoutsbackup_Objectives" + DateTime.Now.ToString("HH-mm-ss-fff");
                        break;
                }

                Directory.CreateDirectory(backupfolderName);

                //remove readonly from file
                bool isReadOnly = (System.IO.File.GetAttributes(destpath) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly;
                if (isReadOnly == true) { System.IO.File.SetAttributes(destpath, System.IO.File.GetAttributes(destpath) & ~FileAttributes.ReadOnly); }

                //Replace the modified file 
                System.IO.File.Copy(srcpath, destpath, true);

            }
            catch (Exception ex)
            {

                throw;
            }

        }

        private void WriteContentsToFile(string inputcontents, string outputfilepath, string inputpath)
        {
            if (inputpath == string.Empty)
            {
                using (StreamWriter sw = new StreamWriter(outputfilepath, true))
                {
                    sw.WriteLine(inputcontents);
                }
            }
            else
            {
                // Read all text
                StreamReader sr = System.IO.File.OpenText(inputpath);
                string body = sr.ReadToEnd();
                System.IO.File.AppendAllText(outputfilepath, body);
            }
        }



        public bool UpdateExpirytime(CommonManagerProxy proxy, string ActualTime)
        {
            string xmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
            XDocument adminXmlDoc = XDocument.Load(xmlpath);
            var DefaultReportsetting = adminXmlDoc.Descendants("Expirysetting").FirstOrDefault();
            var DefaultSchemaResponse = adminXmlDoc.Descendants("Expirysetting").Descendants("ActionDateExecutionTime").FirstOrDefault();
            if (DefaultReportsetting == null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<Expirysetting><ActionDateExecutionTime>" + ActualTime + "</ActionDateExecutionTime></Expirysetting>");
                XElement.Parse(sb.ToString());
                adminXmlDoc.Root.Add(XElement.Parse(sb.ToString()));
                adminXmlDoc.Save(xmlpath);

            }
            else if (DefaultSchemaResponse == null)
            {

                XElement ReportSettingsElement = adminXmlDoc.Descendants("Expirysetting").FirstOrDefault();
                ReportSettingsElement.SetElementValue("ActionDateExecutionTime", ActualTime);
                adminXmlDoc.Save(xmlpath);
            }
            else if (DefaultReportsetting != null && DefaultSchemaResponse != null)
            {
                adminXmlDoc.Descendants("Expirysetting").Descendants("ActionDateExecutionTime").Remove();
                adminXmlDoc.Save(xmlpath);
                XElement ReportSettingsElement = adminXmlDoc.Descendants("Expirysetting").FirstOrDefault();
                ReportSettingsElement.SetElementValue("ActionDateExecutionTime", ActualTime);
                adminXmlDoc.Save(xmlpath);
            }
            return true;
        }


        public string Getexpirytime(CommonManagerProxy proxy)
        {

            using (ITransaction tx = proxy.MarcomManager.GetTransaction())
            {
                string xmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
                XDocument adminXmlDoc = XDocument.Load(xmlpath);
                var ReportServerSchemaResponse = adminXmlDoc.Descendants("Expirysetting").Descendants("ActionDateExecutionTime").ElementAt(0).Value;
                tx.Commit();
                return Convert.ToString(ReportServerSchemaResponse);
            }
        }
    }
}

