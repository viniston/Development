using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrandSystems.Marcom.Core.Interface.Managers;
using BrandSystems.Marcom.Core.Interface;
using BrandSystems.Marcom.Core.Common.Interface;
using BrandSystems.Marcom.Dal.Common.Model;
using System.Xml.Linq;
using System.IO;
using BrandSystems.Marcom.Core.Metadata.Interface;
using System.Collections;
using Mail;
using BrandSystems.Marcom.Core.Common;
using BrandSystems.Marcom.Core.Task;
using BrandSystems.Marcom.Core.Task.Interface;
using BrandSystems.Marcom.Dal.Task.Model;
using Newtonsoft.Json.Linq;
using BrandSystems.Marcom.Core.Report.Interface;
using BrandSystems.Marcom.Core.Metadata;
using BrandSystems.Marcom.Core.Planning.Interface;

namespace BrandSystems.Marcom.Core.Managers.Proxy
{
    internal partial class CommonManagerProxy : ICommonManager, IManagerProxy
    {
        // Reference to the MarcomManager
        private MarcomManager _marcomManager = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommonManagerProxy"/> class.
        /// </summary>
        /// <param name="marcomManager">The marcom manager.</param>
        internal CommonManagerProxy(MarcomManager marcomManager)
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


        #region Instance of Classes In ServiceLayer reference
        /// <summary>
        /// Returns File class.
        /// </summary>
        public IFile Fileservice()
        {
            return CommonManager.Instance.Fileservice();
        }

        #endregion

        /// <summary>
        /// Initializes the type of the isubscription.
        /// </summary>
        /// <param name="strBody">The STR body.</param>
        /// <returns>
        /// ISubscriptionType
        /// </returns>
        public ISubscriptionType initializeIsubscriptionType(string strBody)
        {
            return MarcomManager.CommonManager.initializeIsubscriptionType(strBody);
        }

        /// <summary>
        /// Initializes the inavigation.
        /// </summary>
        /// <param name="strBody">The STR body.</param>
        /// <returns>
        /// INavigation
        /// </returns>
        public INavigation initializeInavigation(string strBody)
        {
            return MarcomManager.CommonManager.initializeInavigation(strBody);
        }

        /// <summary>
        /// Initializes the I user mail subscription.
        /// </summary>
        /// <param name="strBody">The STR body.</param>
        /// <returns>
        /// IUserMailSubscription
        /// </returns>
        public IUserMailSubscription initializeIUserMailSubscription(string strBody)
        {
            return MarcomManager.CommonManager.initializeIUserMailSubscription(strBody);
        }



        /// <summary>
        /// Gets the type of all subscription.
        /// </summary>
        /// <returns>
        /// IList
        /// </returns>
        public IList<ISubscriptionType> GetAllSubscriptionType()
        {
            return CommonManager.Instance.GetAllSubscriptionType(this);
        }

        /// <summary>
        /// Get Widget List for the user
        /// </summary>
        /// <param name="proxy"></param>
        /// <param name="userid"> </param>
        /// <returns>IWidgetContainer </returns>
        public IList<IWidget> GetWidgetDetailsByUserID(int userid, bool isAdmin, int GlobalTemplateID)
        {
            return CommonManager.Instance.GetWidgetDetailsByUserID(this, userid, isAdmin, GlobalTemplateID);
        }

        public bool CheckUserPermissionForEntity(int entityID)
        {
            return CommonManager.Instance.CheckUserPermissionForEntity(this, entityID);
        }

        /// <summary>
        /// Get Dynamic Widget Content
        /// </summary>
        /// <param name="proxy"></param>
        /// <param name="userid"> </param>
        /// <param name="WidgetId"> </param>
        /// <param name="WidgetTypeID"> </param>
        /// <param name="IsDynamic"> </param>
        /// <returns>Ilist </returns>
        public List<object> GetDynamicwidgetContentUserID(int userid, int widgetTypeid, string widgetId, int dimensionid)
        {
            return CommonManager.Instance.GetDynamicwidgetContentUserID(this, userid, widgetTypeid, widgetId, dimensionid);
        }
        /// <summary>
        /// Gets the user subscription settings.
        /// </summary>
        /// <param name="SubscribtionTypeID">The subscribtion type ID.</param>
        /// <returns>
        /// IUserSubscription
        /// </returns>
        public IUserSubscription GetUserSubscriptionSettings(string SubscribtionTypeID)
        {
            return CommonManager.Instance.GetUserSubscriptionSettings(this, SubscribtionTypeID);
        }

        /// <summary>
        /// Updates the user subscription settings.
        /// </summary>
        /// <param name="UserId">The user id.</param>
        /// <param name="subscriptiontype">The subscriptiontype.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool UpdateUserSubscriptionSettings(int UserId, ISubscriptionType subscriptiontype)
        {
            return CommonManager.Instance.UpdateUserSubscriptionSettings(this, UserId, subscriptiontype);
        }

        /// <summary>
        /// Updates the user subscription settings.
        /// </summary>
        /// <param name="UserId">The user id.</param>
        /// <param name="Id">The id.</param>
        /// <returns>
        /// IUserDefaultSubscription
        /// </returns>
        public bool UpdateUserSubscriptionSettings(int UserId, int Id)
        {
            return CommonManager.Instance.UpdateUserSubscriptionSettings(this, UserId, Id);
        }


        /// <summary>
        /// Gets the notification by ids.
        /// </summary>
        /// <param name="notificationid">The notificationid.</param>
        /// <param name="UserId">The user id.</param>
        /// <returns>
        /// IUserNotification
        /// </returns>
        public Tuple<IList<INotificationSelection>, int, IList> GetNotification(int flag, int pageNo = 0)
        {
            return CommonManager.Instance.GetNotification(this, flag, pageNo);
        }


        public int UpdateIsviewedStatusNotification(int UserId, string ids, int flag)
        {
            return CommonManager.Instance.UpdateIsviewedStatusNotification(this, UserId, ids, flag);
        }

        /// <summary>
        /// Insert Navigation.
        /// </summary>
        /// <param name="navigation">The navigation.</param>
        /// <returns>
        /// string
        /// </returns>
        public int Navigation_Insert(INavigation navigation)
        {
            return CommonManager.Instance.Navigation_Insert(this, navigation);
        }

        /// <summary>
        /// Insert Navigation.
        /// </summary>
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
        /// <returns>
        /// last inserted id
        /// </returns>
        public int Navigation_Insert(int Id, int Typeid, int Parentid, int Moduleid, int Featureid, string Caption, string Description, string Url, string JavaScript, bool IsActive, bool IsPopup, bool IsIframe, bool IsDynamicPage, bool IsExternal, bool AddUserName, bool AddUserEmail, bool IsDefault, string ExternalUrl, string Imageurl, int GlobalRoleid, bool AddUserID, bool AddLanguageCode)
        {
            return CommonManager.Instance.Navigation_Insert(this, Id, Typeid, Parentid, Moduleid, Featureid, Caption, Description, Url, JavaScript, IsActive, IsPopup, IsIframe, IsDynamicPage, IsExternal, AddUserName, AddUserEmail, IsDefault, ExternalUrl, Imageurl, GlobalRoleid, AddUserID, AddLanguageCode);
        }


        /// <summary>
        /// Update Navigation.
        /// </summary>
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
        /// <param name="IsDefault">if set to <c>true</c> [is Default].</param>
        /// <param name="IsExternal">if set to <c>true</c> [is External].</param>
        /// <param name="External Url">The External URL.</param>
        /// <returns>
        /// bool 
        /// </returns>
        public bool Navigation_Update(int ID, int typeID, int parentId, int moduleid, int featureid, string caption, string Description, string URL, string externalurl, bool IsExternal, bool IsDefault, bool IsEnable, bool AddUserID, bool AddLanguageCode, bool AddUserEmail, bool AddUserName, int SortOrder)
        {
            return CommonManager.Instance.Navigation_Update(this, ID, typeID, parentId, moduleid, featureid, caption, Description, URL, externalurl, IsExternal, IsDefault, IsEnable, AddUserID, AddLanguageCode, AddUserEmail, AddUserName, SortOrder);
        }




        /// <summary>
        /// Deletes Navigation.
        /// </summary>
        /// <param name="navigation">The navigation.</param>
        /// <returns>
        /// string
        /// </returns>
        public bool Navigation_Delete(INavigation navigation)
        {
            return CommonManager.Instance.Navigation_Delete(this, navigation);
        }

        /// <summary>
        /// Deletes Navigation.
        /// </summary>
        /// <param name="Id">The id.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool Navigation_Delete(int Id)
        {
            return CommonManager.Instance.Navigation_Delete(this, Id);
        }

        /// <summary>
        /// select Navigation.
        /// </summary>
        /// <param name="NavigationID">The navigation ID.</param>
        /// <param name="ParentID">The parent ID.</param>
        /// <returns>
        /// IList
        /// </returns>
        public IList<INavigation> Navigation_Select(bool IsParentID, int UserID, int flag)
        {
            return CommonManager.Instance.Navigation_Select(this, IsParentID, UserID, flag);
        }

        public bool UpdateNavigationSortOrder(int Id, int SortOrder)
        {
            return CommonManager.Instance.UpdateNavigationSortOrder(this, Id, SortOrder);
        }

        /// <summary>
        /// Gets the group ID for navigation.
        /// </summary>
        /// <param name="NavID">The nav ID.</param>
        /// <param name="UserID">The user ID.</param>
        /// <returns>
        /// INavigationAccess
        /// </returns>
        public INavigationAccess GetGroupIDForNavigation(int NavID, int UserID)
        {
            return CommonManager.Instance.GetGroupIDForNavigation(this, NavID, UserID);
        }

        /// <summary>
        /// Inserts the subscription notificationsettings.
        /// </summary>
        /// <param name="userid">The userid.</param>
        /// <param name="lastsenton">The lastsenton.</param>
        /// <param name="lastupdatedon">The lastupdatedon.</param>
        /// <param name="Timing">The timing.</param>
        /// <param name="IsEmailEnable">if set to <c>true</c> [is email enable].</param>
        /// <param name="DayName">Name of the day.</param>
        /// <param name="RecapReport">if set to <c>true</c> [recap report].</param>
        /// <returns>
        /// int
        /// </returns>
        public int InsertSubscriptionNotificationsettings(int userid, DateTimeOffset lastsenton, DateTimeOffset lastupdatedon, TimeSpan Timing, bool IsEmailEnable = true, string DayName = "daily", bool RecapReport = true)
        {
            return CommonManager.Instance.InsertSubscriptionNotificationsettings(this, userid, lastsenton, lastupdatedon, Timing, IsEmailEnable, DayName, RecapReport);
        }

        // public IUserMailSubscription UpdateRecapNotificationsettings(int userid,  DateTimeOffset lastupdatedon,  bool RecapReport = true)
        /// <summary>
        /// Updates the recap notificationsettings.
        /// </summary>
        /// <param name="Id">The id.</param>
        /// <param name="usrmailsbcrptn">The usrmailsbcrptn.</param>
        /// <returns></returns>
        public bool UpdateRecapNotificationsettings(int Id, IUserMailSubscription usrmailsbcrptn)
        {
            return CommonManager.Instance.UpdateRecapNotificationsettings(this, Id, usrmailsbcrptn);
        }

        /// <summary>
        /// Updates the recap notificationsettings.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="Userid">The userid.</param>
        /// <param name="LastSentOn">The last sent on.</param>
        /// <param name="LastUpdatedOn">The last updated on.</param>
        /// <param name="IsEmailEnable">if set to <c>true</c> [is email enable].</param>
        /// <param name="DayName">Name of the day.</param>
        /// <param name="Timing">The timing.</param>
        /// <param name="RecapReport">if set to <c>true</c> [recap report].</param>
        /// <returns>
        /// IUserMailSubscription
        /// </returns>
        public bool UpdateRecapNotificationsettings(int id, int Userid, DateTimeOffset LastSentOn, DateTimeOffset LastUpdatedOn, bool IsEmailEnable, string DayName, TimeSpan Timing, bool RecapReport)
        {
            return CommonManager.Instance.UpdateRecapNotificationsettings(this, id, Userid, LastSentOn, LastUpdatedOn, IsEmailEnable, DayName, Timing, RecapReport);
        }

        //feed id select 
        /// <summary>
        /// Gets the feed by ID.
        /// </summary>
        /// <param name="entityid">The entityid.</param>
        /// <returns></returns>
        public IList<IFeed> GetFeedByID(int entityid)
        {
            return CommonManager.Instance.GetFeedByID(this, entityid);
        }

        //feed comment insert
        /// <summary>
        /// Inserts the feed comment.
        /// </summary>
        /// <param name="feedid">The feedid.</param>
        /// <param name="actor">The actor.</param>
        /// <param name="comment">The comment.</param>
        /// <param name="commentupdatedon">The commentupdatedon.</param>
        /// <returns>
        /// int
        /// </returns>
        public string InsertFeedComment(int feedid, int actor, string comment)
        {
            return CommonManager.Instance.InsertFeedComment(this, feedid, actor, comment);
        }

        /// <summary>
        /// Inserts the content of the update mail.
        /// </summary>
        /// <param name="Subject">The subject.</param>
        /// <param name="Body">The body.</param>
        /// <param name="description">The description.</param>
        /// <param name="id">The id.</param>
        /// <returns>
        /// int
        /// </returns>
        public int InsertUpdateMailContent(string Subject, string Body, string description, int id)
        {
            return CommonManager.Instance.InsertUpdateMailContent(this, Subject, Body, description, id);
        }

        /// <summary>
        /// Deletes the content of the mail.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public bool DeleteMailContent(int id)
        {
            return CommonManager.Instance.DeleteMailContent(this, id);
        }

        /// <summary>
        /// Inserts the update mail footer.
        /// </summary>
        /// <param name="Body">The body.</param>
        /// <param name="description">The description.</param>
        /// <param name="id">The id.</param>
        /// <returns>
        /// int
        /// </returns>
        public int InsertUpdateMailFooter(string Body, string description, int id)
        {
            return CommonManager.Instance.InsertUpdateMailFooter(this, Body, description, id);
        }

        /// <summary>
        /// Deletes the mail footer.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool DeleteMailFooter(int id)
        {
            return CommonManager.Instance.DeleteMailFooter(this, id);
        }

        //Auto Subscription
        /// <summary>
        /// Updates the user single entity subscription.
        /// </summary>
        /// <param name="EntityId">The entity id.</param>
        /// <param name="SubscriptionTypeIDs">The subscription type I ds.</param>
        /// <param name="UserID">The user ID.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool UpdateUserSingleEntitySubscription(int EntityId, SubscriptionTypeDao SubscriptionTypeIDs, int UserID)
        {
            return CommonManager.Instance.UpdateUserSingleEntitySubscription(this, EntityId, SubscriptionTypeIDs, UserID);
        }

        /// <summary>
        /// Updates the user single entity subscription.
        /// </summary>
        /// <param name="EntityId">The entity id.</param>
        /// <param name="Id">The id.</param>
        /// <param name="Caption">The caption.</param>
        /// <param name="IsAutomated">if set to <c>true</c> [is automated].</param>
        /// <param name="Userid">The userid.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool UpdateUserSingleEntitySubscription(int EntityId, int Id, string Caption, bool IsAutomated, int Userid)
        {
            return CommonManager.Instance.UpdateUserSingleEntitySubscription(this, EntityId, Id, Caption, IsAutomated, Userid);
        }


        //MultiSubscription Load
        /// <summary>
        /// Loads User multi subscription.
        /// </summary>
        /// <param name="EntitiyId">The entitiy id.</param>
        /// <param name="UserId">The user id.</param>
        /// <returns>
        /// int
        /// </returns>
        public Tuple<int[], int, int> UserMultiSubscriptionLoad(int EntitiyId, int UserId)
        {
            return CommonManager.Instance.UserMultiSubscriptionLoad(this, EntitiyId, UserId);
        }

        //MultiSubscription save and update
        /// <summary>
        /// Saves the update multi subscription.
        /// </summary>
        /// <param name="Levels">The levels.</param>
        /// <param name="EntityId">The entity id.</param>
        /// <param name="UserId">The user id.</param>
        /// <param name="IsMultiLevel">if set to <c>true</c> [is multi level].</param>
        /// <param name="EntityTypeId">The entity type id.</param>
        /// <returns></returns>
        public String SaveUpdateMultiSubscription(int[] levels, int EntityId, int Userid, bool IsMultiLevel, int EntityTypeId, DateTimeOffset SubscribedOn, DateTimeOffset LastUpdatedOn, int filteroption)
        {
            return CommonManager.Instance.SaveUpdateMultiSubscription(this, levels, EntityId, Userid, IsMultiLevel, EntityTypeId, SubscribedOn, LastUpdatedOn, filteroption);
        }

        //MultiSubscription Unscubscribe
        /// <summary>
        /// Unsubscribes the multi subscription.
        /// </summary>
        /// <param name="EntitiyId">The entitiy id.</param>
        /// <param name="UserId">The user id.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool UnsubscribeMultiSubscription(int EntitiyId, int UserId)
        {
            return CommonManager.Instance.UnsubscribeMultiSubscription(this, EntitiyId, UserId);
        }

        /// <summary>
        /// Saves the update feed template.
        /// </summary>
        /// <param name="ModuleId">The module id.</param>
        /// <param name="FeatureId">The feature id.</param>
        /// <param name="Template">The template.</param>
        /// <returns>
        /// int
        /// </returns>
        public int SaveUpdateFeedTemplate(int ModuleId, int FeatureId, String Template)
        {
            return CommonManager.Instance.SaveUpdateFeedTemplate(this, ModuleId, FeatureId, Template);
        }

        /// <summary>
        /// Saves the update feed.
        /// </summary>
        /// <param name="Actor">The actor.</param>
        /// <param name="TemplateId">The template id.</param>
        /// <param name="EntityId">The entity id.</param>
        /// <param name="TypeName">Name of the type.</param>
        /// <param name="AttributeName">Name of the attribute.</param>
        /// <param name="FromValue">From value.</param>
        /// <param name="ToValue">To value.</param>
        /// <returns>
        /// Last Inserted Feed ID
        /// </returns>
        public int SaveUpdateFeed(int Actor, int TemplateId, int EntityId, String TypeName, String AttributeName, String FromValue, String ToValue, int UserId, int associatedentityid, string attributeGroupRecordName, int Version)
        {
            return CommonManager.Instance.SaveUpdateFeed(this, Actor, TemplateId, EntityId, TypeName, AttributeName, FromValue, ToValue, UserId, associatedentityid, attributeGroupRecordName, Version);
        }

        /// <summary>
        /// Gets the type of the notification BY.
        /// </summary>
        /// <param name="pCaption">The p caption.</param>
        /// <returns>
        /// INotificationType
        /// </returns>
        public INotificationType GetNotificationBYType(string pCaption)
        {
            return CommonManager.Instance.GetNotificationBYType(this, pCaption);
        }

        /// <summary>
        /// Gets the user default subscription.
        /// </summary>
        /// <param name="SubScriptionTypeId">The sub scription type id.</param>
        /// <returns>
        /// IList
        /// </returns>
        public IList<IUserDefaultSubscription> GetUserDefaultSubscription(ISubscriptionType SubScriptionTypeId)
        {
            return CommonManager.Instance.GetUserDefaultSubscription(this, SubScriptionTypeId);
        }

        /// <summary>
        /// Gets the user default subscription by user id.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="SubScriptionTypeId">The sub scription user id.</param>
        /// <returns>IList</returns>
        public Tuple<IList<ISubscriptionType>, string[], string[]> GetUserDefaultSubscriptionByUserID()
        {
            return CommonManager.Instance.GetUserDefaultSubscriptionByUserID(this);
        }

        /// <summary>
        /// Gets the user default subscription.
        /// </summary>
        /// <param name="Id">The id.</param>
        /// <param name="Caption">The caption.</param>
        /// <param name="IsAutomated">if set to <c>true</c> [is automated].</param>
        /// <returns>
        /// IList
        /// </returns>
        public IList<IUserDefaultSubscription> GetUserDefaultSubscription(int Id, string Caption, bool IsAutomated)
        {
            return CommonManager.Instance.GetUserDefaultSubscription(this, Id, Caption, IsAutomated);
        }


        /// <summary>
        /// Insert User notification.
        /// </summary>
        /// <param name="Id">The id.</param>
        /// <param name="Userid">The userid.</param>
        /// <param name="Entityid">The entityid.</param>
        /// <param name="Actorid">The actorid.</param>
        /// <param name="CreatedOn">The created on.</param>
        /// <param name="Typeid">The typeid.</param>
        /// <param name="IsViewed">if set to <c>true</c> [is viewed].</param>
        /// <param name="IsSentInMail">if set to <c>true</c> [is sent in mail].</param>
        /// <param name="NotificationText">The notification text.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool UserNotification_Insert(int Entityid, int Actorid, DateTimeOffset CreatedOn, int Typeid, bool IsViewed, bool IsSentInMail, string Typename, string Attributename, string Fromvalue, string Tovalue, int mailtemplateid, int userid)
        {
            return CommonManager.Instance.UserNotification_Insert(this, Entityid, Actorid, CreatedOn, Typeid, IsViewed, IsSentInMail, Typename, Attributename, Fromvalue, Tovalue, mailtemplateid, userid);
        }


        public bool Insert_Mail(int mailtemplateid, IList<UserNotificationDao> listofusernotification)
        {
            return CommonManager.Instance.Insert_Mail(this, mailtemplateid, listofusernotification);

        }
        public bool InsertMultiAssignedTaskMail(int mailTemplateid, int actorId, List<int> multiTasks, IList<TaskMembersDao> taskMembers)
        {
            return CommonManager.Instance.InsertMultiAssignedTaskMail(this, mailTemplateid, actorId, multiTasks, taskMembers);

        }
        /// <summary>
        /// Get Navigation Config.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <returns>String</returns>
        public string GetNavigationConfig()
        {
            return CommonManager.Instance.GetNavigationConfig(this);
        }

        /// <summary>
        /// Get Navigation Config.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <returns>String</returns>
        public string GetMediabankNavigationConfig()
        {
            return CommonManager.Instance.GetMediabankNavigationConfig(this);
        }
        /// <summary>
        /// Inerting and Updating AdminSettings.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="key">The SettingKey.</param>
        /// <returns>True (or) False</returns>
        public bool AdminSettingsInsertUpdate(string jsondata, string key, string data)
        {
            return CommonManager.Instance.AdminSettingsInsertUpdate(this, jsondata, key, data);
        }
        public bool AdminSettingsforRootLevelInsertUpdate(string jsondata, string key, int typeid)
        {
            return CommonManager.Instance.AdminSettingsforRootLevelInsertUpdate(this, jsondata, key, typeid);
        }

        public bool AdminSettingsforReportInsertUpdate(string jsondata, string key)
        {
            return CommonManager.Instance.AdminSettingsforReportInsertUpdate(this, jsondata, key);
        }

        public bool AdminSettingsforGanttViewInsertUpdate(string jsondata, string key)
        {
            return CommonManager.Instance.AdminSettingsforGanttViewInsertUpdate(this, jsondata, key);
        }

        public bool AdminSettingsforListViewInsertUpdate(string jsondata, string key)
        {
            return CommonManager.Instance.AdminSettingsforListViewInsertUpdate(this, jsondata, key);
        }

        public bool AdminSettingsforRootLevelFilterSettingsInsertUpdate(string jsondata, string key, int EntityTypeID)
        {
            return CommonManager.Instance.AdminSettingsforRootLevelFilterSettingsInsertUpdate(this, jsondata, key, EntityTypeID);
        }

        public bool AdminSettingsforDetailFilterInsertUpdate(string jsondata, string key)
        {
            return CommonManager.Instance.AdminSettingsforDetailFilterInsertUpdate(this, jsondata, key);
        }

        public bool AdminSettingsForRootLevelDelete(string key, int EntityTypeID)
        {
            return CommonManager.Instance.AdminSettingsForRootLevelDelete(this, key, EntityTypeID);
        }

        public bool AdminSettingsForRootLevelDelete(string key, int EntityTypeID, int AttributeID)
        {
            return CommonManager.Instance.AdminSettingsForRootLevelDelete(this, key, EntityTypeID, AttributeID);
        }

        /// <summary>
        /// Get Navigation ExternalLink.
        /// </summary>
        /// <param name="proxy">The Typeid.</param>
        /// <param name="proxy">The proxy.</param>
        /// <returns>IList<INavigation></returns>
        public string GetNavigationExternalLinksByID(int typeid)
        {
            return CommonManager.Instance.GetNavigationExternalLinksByID(this, typeid);
        }

        public string GetAdminSettings(string LogoSettings, int typeid)
        {
            return CommonManager.Instance.GetAdminSettings(this, LogoSettings, typeid);
        }


        public string GetAdminSettingselemntnode(string LogoSettings, string elemntnode, int typeid)
        {
            return CommonManager.Instance.GetAdminSettingselemntnode(this, LogoSettings, elemntnode, typeid);
        }

        /// <summary>
        /// InsertFile.
        /// </summary>
        /// <param name="proxy">file Parameter</param>
        /// <returns>int</returns>
        public int InsertFile(string Name, int VersionNo, string MimeType, string Extension, long Size, int OwnerID, DateTime CreatedOn, string Checksum, int ModuleID, int EntityID, String FileGuid, string Description, bool IsPlanEntity = false)
        {
            return CommonManager.Instance.InsertFile(this, Name, VersionNo, MimeType, Extension, Size, OwnerID, CreatedOn, Checksum, ModuleID, EntityID, FileGuid, Description, IsPlanEntity);
        }

        ///<summary>
        ///InsertLink
        /// </summary>

        public int InsertLink(int EntityID, string Name, string URL, string Description, int ActiveVersionNo, int TypeID, string CreatedOn, int OwnerID, int ModuleID)
        {
            return CommonManager.Instance.InsertLink(this, EntityID, Name, URL, Description, ActiveVersionNo, TypeID, CreatedOn, OwnerID, ModuleID);
        }


        /// <summary>
        /// DeleteFileByID.
        /// </summary>
        /// <param name="proxy">ID Parameter</param>
        /// <returns>bool</returns>
        public bool DeleteFileByID(int ID)
        {
            return CommonManager.Instance.DeleteFileByID(this, ID);
        }


        /// <summary>
        /// DeleteLinkByID.
        /// </summary>
        /// <param name="proxy">ID Parameter</param>
        /// <returns>bool</returns>
        public bool DeleteLinkByID(int ID)
        {
            return CommonManager.Instance.DeleteLinkByID(this, ID);
        }


        /// <summary>
        /// Get File By  Entity ID.
        /// </summary>
        /// <param name="proxy">EntityID</param>
        /// <returns>int</returns>
        public IList<IFile> GetFileByEntityID(int EntityID)
        {
            return CommonManager.Instance.GetFileByEntityID(this, EntityID);
        }


        /// <summary>
        /// Get Links By  Entity ID.
        /// </summary>
        /// <param name="proxy">EntityID</param>
        /// <returns>int</returns>
        public IList<IFile> GetFilesandLinksByEntityID(int EntityID)
        {
            return CommonManager.Instance.GetFilesandLinksByEntityID(this, EntityID);
        }


        /// <summary>
        /// GettingFeedsByAsset
        /// </summary>
        /// <param name="FeedId">The EntityID</param>
        /// <param name="lastFeedRequestedTime">The Last Requested Time</param>
        /// <returns>IList<IFeedSelection></IFeedSelection></returns>
        public IList<IFeedSelection> GettingFeedsByAsset(int AssetId, int pageNo)
        {
            return CommonManager.Instance.GettingFeedsByAsset(this, AssetId, pageNo, false);
        }


        /// <summary>
        /// GettingFeedsByAsset
        /// </summary>
        /// <param name="FeedId">The EntityID</param>
        /// <param name="lastFeedRequestedTime">The Last Requested Time</param>
        /// <returns>IList<IFeedSelection></IFeedSelection></returns>
        public IList<IFeedSelection> GettingLatestFeedsByAsset(int AssetId)
        {
            return CommonManager.Instance.GettingFeedsByAsset(this, AssetId, -1, true);
        }



        /// <summary>
        /// Getting Feeds by EntiyID
        /// </summary>
        /// <param name="FeedId">The EntityID</param>
        /// <param name="lastFeedRequestedTime">The Last Requested Time</param>
        /// <returns>IList<IFeedSelection></IFeedSelection></returns>
        public IList<IFeedSelection> GettingFeedsByEntityIDandFundingrequest(int entityId, int pageNo, bool islatestfeed)
        {
            return CommonManager.Instance.GettingFeedsByEntityIDandFundingrequest(this, entityId, pageNo, islatestfeed);
        }


        /// <summary>
        /// Getting Feeds by EntiyID
        /// </summary>
        /// <param name="FeedId">The EntityID</param>
        /// <param name="lastFeedRequestedTime">The Last Requested Time</param>
        /// <returns>IList<IFeedSelection></IFeedSelection></returns>
        public IList<IFeedSelection> GetFeedsByEntityID(string entityId, int pageNo, int entityIdForReference = 0, string newsfeedgroupid = "")
        {
            return CommonManager.Instance.GettingFeedsByEntityID(this, entityId, pageNo, false, entityIdForReference, 0, newsfeedgroupid);
        }
        public string GetEntityIdsForFeed(int entityId)
        {
            return CommonManager.Instance.GetEntityIdsForFeed(this, entityId);
        }
        /// <summary>
        /// Getting Feeds by EntiyID and Last requested time
        /// </summary>
        /// <param name="FeedId">The EntityID</param>
        /// <param name="lastFeedRequestedTime">The Last Requested Time</param>
        /// <returns>IList<IFeedSelection></IFeedSelection></returns>
        public IList<IFeedSelection> GetLatestFeedsByEntityID(string entityId, int entityIdForReference = 0, string newsfeedgroupid = "")
        {
            return CommonManager.Instance.GettingFeedsByEntityID(this, entityId, -1, true, Convert.ToInt32(entityIdForReference), 0, newsfeedgroupid);
        }
        public bool InsertUserSingleEntitySubscription(int UserId, int EntityId, int EntitytypeId, DateTimeOffset SubscribedOn, DateTimeOffset LastUpdatedOn, string issubscribe, int filteroption)
        {
            return CommonManager.Instance.InsertUserSingleEntitySubscription(this, UserId, EntityId, EntitytypeId, SubscribedOn, LastUpdatedOn, issubscribe, filteroption);
        }
        public IList<IEntityType> GetEntityTypeforSubscription(int ID)
        {
            return CommonManager.Instance.GetEntityTypeforSubscription(this, ID);
        }

        public String GetAutoSubscriptionDetails(int UserID, int EntityID)
        {
            return CommonManager.Instance.GetAutoSubscriptionDetails(this, UserID, EntityID);
        }

        /// <summary>
        /// update the user default subscription by user id.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="SubScriptionTypeId">The sub scription user id.</param>
        /// <returns>IList</returns>
        public bool SaveSelectedDefaultSubscription(string subscriptionTypeIds, string mailSubscritpitonTypeIds)
        {
            return CommonManager.Instance.SaveSelectedDefaultSubscription(this, subscriptionTypeIds, mailSubscritpitonTypeIds);
        }

        /// <summary>
        /// Update Notification by email.
        /// </summary>
        /// <param name="proxy"> </param>
        /// <returns>bool</returns>
        public bool SaveNotificationByMail(string ColumnName, string ColumnValue)
        {
            return CommonManager.Instance.SaveNotificationByMail(this, ColumnName, ColumnValue);
        }


        /// <summary>
        /// Update Task Notification.
        /// </summary>
        /// <param name="proxy"> </param>
        /// <returns>bool</returns>
        public bool SaveTaskNotificationByMail(string ColumnName, string ColumnValue)
        {
            return CommonManager.Instance.SaveTaskNotificationByMail(this, ColumnName, ColumnValue);
        }
        /// <summary>
        /// select WidgetTemplate.
        /// </summary>
        /// <param name="TemplateID">The WidgetTemplate ID.</param>

        /// <returns>
        /// IList
        /// </returns>
        public IList<IWidgetTemplate> WidgetTemplate_Select(int TemplateID)
        {
            return CommonManager.Instance.WidgetTemplate_Select(this, TemplateID);
        }

        /// <summary>
        /// select WidgetTypes.
        /// </summary>
        /// <param name="TypeID">The WidgetType ID.</param>

        /// <returns>
        /// IList
        /// </returns>
        public IList<IWidgetTypes> WidgetTypes_Select(int userId, bool isAdmin, int typeId = 0)
        {
            return CommonManager.Instance.WidgetTypes_Select(this, userId, isAdmin, typeId);
        }

        /// <summary>
        /// select WidgetTypeRoles.
        /// </summary>
        /// <param name="WidgetTypeID">The WidgetType ID.</param>

        /// <returns>
        /// int[]
        /// </returns>
        public int[] GetWidgetTypeRolesByID(int WidgetTypeID)
        {
            return CommonManager.Instance.GetWidgetTypeRolesByID(this, WidgetTypeID);
        }

        /// <summary>
        /// select WidgetTypeDimension.
        /// </summary>
        /// <param name="WidgetTypeID">The WidgetType ID.</param>

        /// <returns>
        /// IList
        /// </returns>
        public IList<IWidgetTypeDimension> WidgetTypeDimension_Select(int WidgetTypeID)
        {
            return CommonManager.Instance.WidgetTypeDimension_Select(this, WidgetTypeID);
        }

        //WidgetTemplate insert
        /// <summary>
        /// <param name="TemplateName">The TemplateName.</param>
        /// <returns>
        /// int
        /// </returns>
        public int InsertWidgetTemplate(string TemplateName, string TemplateDescription)
        {
            return CommonManager.Instance.InsertWidgetTemplate(this, TemplateName, TemplateDescription);
        }


        /// <summary>
        /// Update WidgetTemplate.
        /// </summary>
        /// <param name="Id">The id.</param>
        /// <param name="TemplateName">The TemplateName.</param>
        /// <param name="TemplateDescription">The TemplateDescription.</param>

        /// <returns>
        /// bool 
        /// </returns>
        public bool WidgetTemplate_Update(int ID, string TemplateName, string TemplateDescription)
        {
            return CommonManager.Instance.WidgetTemplate_Update(this, ID, TemplateName, TemplateDescription);
        }

        /// <summary>
        /// Inserts the WidgetTypeRoles.
        /// </summary>
        /// <param name="widgetTypeID">The widgetTypeID.</param>
        /// <param name="roleID">The roleID.</param>
        /// <returns>int</returns>
        public int InsertWidgetTypeRoles(int widgetTypeID, int roleID)
        {
            return CommonManager.Instance.InsertWidgetTypeRoles(this, widgetTypeID, roleID);
        }

        /// <summary>
        /// Inserts the WidgetTemplateRoles.
        /// </summary>
        /// <param name="WidgetTemplateID">The WidgetTemplateID.</param>
        /// <param name="roleID">The roleID.</param>
        /// <returns>int</returns>
        public int InsertWidgetTemplateRoles(int WidgetTemplateID, int roleID)
        {
            return CommonManager.Instance.InsertWidgetTemplateRoles(this, WidgetTemplateID, roleID);
        }

        /// Get the WidgetTemplateRoles  By TemplateID.
        /// </summary>
        /// <param name="WidgetTemplateID">The WidgetTemplateID.</param>
        /// <returns>int[] </returns>
        public int[] GetWidgetTemplateRolesByTemplateID(int WidgetTemplateID)
        {
            return CommonManager.Instance.GetWidgetTemplateRolesByTemplateID(this, WidgetTemplateID);
        }
        /// <summary>
        /// Delete the WidgetTemplateRoles.
        /// </summary>
        /// <param name="WidgetTemplateID">The WidgetTemplateID.</param>
        /// <param name="roleID">The roleID.</param>
        /// <returns>bool</returns>
        public bool DeleteWidgetTemplateRoles(int WidgetTemplateID)
        {
            return CommonManager.Instance.DeleteWidgetTemplateRoles(this, WidgetTemplateID);
        }

        /// Delete the WidgetTypeRoles
        /// </summary>
        /// <param name="WidgetTypeID">The WidgetTypeID.</param>
        /// <returns>bool</returns>
        public bool DeleteWidgetTypeRoles(int WidgetTypeID)
        {
            return CommonManager.Instance.DeleteWidgetTypeRoles(this, WidgetTypeID);
        }

        /// <summary>
        /// Insert widget.
        /// </summary>
        /// <returns>Iwidget Object.</returns>
        public string InsertUpdateWidget(string templateid, string widgetid, string caption, string description, int widgettypeid, int filterid, int attributeid, bool isstatic, string widgetQuery, int dimensionid, string matrixid, int columnval, int rowval, int sizeXval, int sizeYval, bool IsAdminPage, int visualtypeid, int NoOfItem, string listofentityid, string listofSelectEntityID)
        {
            return CommonManager.Instance.InsertUpdateWidget(this, templateid, widgetid, caption, description, widgettypeid, filterid, attributeid, isstatic, widgetQuery, dimensionid, matrixid, columnval, rowval, sizeXval, sizeYval, IsAdminPage, visualtypeid, NoOfItem, listofentityid, listofSelectEntityID);
        }

        /// <summary>
        /// Gets the widget details.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <returns>List of IWidget</returns>
        public IList<IWidget> GetWidgetDetails(string widgetid, string templateid, bool IsAdminPage)
        {
            return CommonManager.Instance.GetWidgetDetails(this, widgetid, templateid, IsAdminPage);
        }

        /// <summary>
        /// Delete widget.
        /// </summary>
        /// <returns>Iwidget Object.</returns>
        public bool DeleteWidget(string templateid, string widgetid, bool IsAdminPage)
        {
            return CommonManager.Instance.DeleteWidget(this, templateid, widgetid, IsAdminPage);
        }

        /// <summary>
        /// Insert widget.
        /// </summary>
        /// <returns>Iwidget Object.</returns>
        public string WidgetDragEdit(IList<IWidget> widgetdata, bool IsAdminPage)
        {
            return CommonManager.Instance.WidgetDragEdit(this, widgetdata, IsAdminPage);
        }

        /// <summary>
        /// Get subscription by user id.
        /// </summary>
        /// <param name="proxy"> </param>
        /// <returns>bool</returns>
        public IUserMailSubscription GetSubscriptionByUserId()
        {
            return CommonManager.Instance.GetSubscriptionByUserId(this);
        }

        public IUserTaskNotificationMailSettings GetTaskSubscriptionByUserId()
        {
            return CommonManager.Instance.GetTaskSubscriptionByUserId(this);
        }

        /// <summary>
        /// Get subscription by user id.
        /// </summary>
        /// <param name="proxy"> </param>
        /// <returns>bool</returns>
        public bool GetIsSubscribedFromSettings()
        {
            return CommonManager.Instance.GetIsSubscribedFromSettings(this);
        }
        public Tuple<IList<BrandSystems.Marcom.Dal.Metadata.Model.EntityTypeDao>, string[]> GetNotAssociateEntityTypes()
        {
            return CommonManager.Instance.GetNotAssociateEntityTypes(this);
        }


        #region Instance of Classes In ServiceLayer reference
        /// <summary>
        /// Returns EntityRolesUser class.
        /// </summary>
        public IWidget Widgetservice()
        {
            return CommonManager.Instance.Widgetservice();
        }

        #endregion



        //new logic
        public bool InsertMail(BrandSystems.Marcom.Core.Common.MailHolder mailHolder, string subject, string body)
        {
            return CommonManager.Instance.InsertMail(mailHolder, subject, body);
        }

        public bool HandleSendMail()
        {
            return CommonManager.Instance.HandleSendMail(this);
        }
        public bool HandleUnScheduledMail(BrandSystems.Marcom.Core.Common.MailHolder mailHolder, string subject, string body)
        {
            return CommonManager.Instance.HandleUnScheduledMail(this, mailHolder, subject, body);
        }
        public IList<int> GetListofUserIdForNotificationbyMail(int notificationtemplateid, int entityid)
        {
            return CommonManager.Instance.GetListofUserIdForNotificationbyMail(this, notificationtemplateid, entityid);
        }

        public bool InsertPoSettingXML(string Prefix, string DateFormat, string DigitFormat, string NumberCount)
        {
            return CommonManager.Instance.InsertPoSettingXML(this, Prefix, DateFormat, DigitFormat, NumberCount);
        }

        public IList<PurchaseOrderSettingsDao> GetPoSSettings(string PoSettings)
        {
            return CommonManager.Instance.GetPoSSettings(this, PoSettings);
        }
        public bool Insert_AdminEmail(string jsondata)
        {
            return CommonManager.Instance.Insert_AdminEmail(this, jsondata);
        }

        public string GetEmailids()
        {
            return CommonManager.Instance.GetEmailids(this);
        }
        public bool InsertUpdateAdditionalSettings(int id, string Settingname, string settingValue)
        {

            return CommonManager.Instance.InsertUpdateAdditionalSettings(this, id, Settingname, settingValue);
        }

        public IList<IAdditionalSettings> GetAdditionalSettings()
        {

            return CommonManager.Instance.GetAdditionalSettings(this);
        }

        public IList<ILanguageType> GetLanguageTypes()
        {
            return CommonManager.Instance.GetLanguageTypes(this);
        }

        public bool SaveNewLanguage(int InheritedId, string Name, string Description)
        {
            return CommonManager.Instance.SaveNewLanguage(this, InheritedId, Name, Description);
        }

        public IList GetLanguageContent(int StartRows, int NextRows)
        {
            return CommonManager.Instance.GetLanguageContent(this, StartRows, NextRows);
        }

        public bool UpdateLanguageContent(int LangTypeID, int ContentID, string newValue)
        {
            return CommonManager.Instance.UpdateLanguageContent(this, LangTypeID, ContentID, newValue);
        }

        public String GetLanguageSettings(int LangID)
        {
            return CommonManager.Instance.GetLanguageSettings(this, LangID);
        }

        /// <summary>
        /// Get Running PO Number
        /// </summary>
        /// <param name="proxy"></param>
        /// <returns>Running PO number<Iwidget> </returns>
        public string GetCurrentPONumber()
        {
            return CommonManager.Instance.GetCurrentPONumber(this);
        }

        public bool UpdateLanguageName(int LangTypeID, string NewValue, int NameOrDesc)
        {
            return CommonManager.Instance.UpdateLanguageName(this, LangTypeID, NewValue, NameOrDesc);
        }

        public bool SetDefaultLanguage(int LangID)
        {
            return CommonManager.Instance.SetDefaultLanguage(this, LangID);
        }

        public IList LanguageSearch(int langid, string searchtext, string searchdate, int StartRows)
        {
            return CommonManager.Instance.LanguageSearch(this, langid, searchtext, searchdate, StartRows);
        }

        public IList LanguageSearchs(int langid, string searchtext)
        {
            return CommonManager.Instance.LanguageSearchs(this, langid, searchtext);
        }

        public int GetDefaultLangFromXML()
        {
            return CommonManager.Instance.GetDefaultLangFromXML(this);
        }

        public IUserTaskNotificationMailSettings GetUserDefaultTaskNotificationMailSettings()
        {
            return CommonManager.Instance.GetUserDefaultTaskNotificationMailSettings(this);

        }

        public string GetEditorText()
        {
            return CommonManager.Instance.GetEditorText(this);
            //throw new NotImplementedException();
        }

        public bool InsertEditortext(int[] entityList, string Content = null)
        {
            //throw new NotImplementedException();
            return CommonManager.Instance.InsertEditortext(this, entityList, Content);
        }

        public IList<SSO> GetSSODetails()
        {
            return CommonManager.Instance.GetSSODetails(this);
        }
        public bool UpdateSSOSettings(string key, string iv, string algo, string paddingmode, string ciphermode, string tokenmode, string SSOTimeDifference, string ssousergroups, string ClientIntranetUrl)
        {
            return CommonManager.Instance.UpdateSSOSettings(this, key, iv, algo, paddingmode, ciphermode, tokenmode, SSOTimeDifference, ssousergroups, ClientIntranetUrl);
        }

        public bool IsActiveEntity(int EntityID)
        {
            return CommonManager.Instance.IsActiveEntity(this, EntityID);
        }




        public IList<IUnits> GetUnits()
        {
            return CommonManager.Instance.GetUnits(this);
        }
        public IList<IFeedFilterGroup> GetFilterGroup()
        {
            return CommonManager.Instance.GetFilterGroup(this);
        }
        public IList<IFeedTemplate> GetFeedTemplates()
        {
            return CommonManager.Instance.GetFeedTemplates(this);
        }
        public IList<IUnits> GetUnitsByID(int id)
        {
            return CommonManager.Instance.GetUnitsByID(this, id);
        }
        public int InsertUpdateFeedFilterGroup(int Id, string feedfiltergroupname, string feedactions)
        {
            return CommonManager.Instance.InsertUpdateFeedFilterGroup(this, Id, feedfiltergroupname, feedactions);
        }
        public bool DeleteFeedGroupByid(int id)
        {
            return CommonManager.Instance.DeleteFeedGroupByid(this, id);
        }
        public string GetEntityPathforMail(string ApplicationPath, int EntityID, int typeID, int UserId, bool isSsoUser, int parentID)
        {
            return CommonManager.Instance.GetEntityPathforMail(ApplicationPath, EntityID, typeID, UserId, isSsoUser, parentID);
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
        public bool InsertUpdateUnits(string caption, int unitsid)
        {
            return CommonManager.Instance.InsertUpdateUnits(this, caption, unitsid);
        }

        public bool DeleteUnitsByid(int unitsid)
        {
            return CommonManager.Instance.DeleteUnitsByid(this, unitsid);
        }

        public TopNavigation GetTopNavigation()
        {
            return CommonManager.Instance.GetTopNavigation(this);
        }
        public int InsertAdminNotificationSettings(JArray subscriptionObject)
        {
            return CommonManager.Instance.InsertAdminNotificationSettings(this, subscriptionObject);

        }

        public bool InsertCurrencyFormat(string ShortName, string Name, string Symbol, int Id = 0)
        {
            return CommonManager.Instance.InsertCurrencyFormat(this, ShortName, Name, Symbol, Id);
        }

        public int InsertBroadcastMessages(int userId, string username, string broadcastmsg)
        {
            return CommonManager.Instance.InsertBroadcastMessages(this, userId, username, broadcastmsg);
        }

        public IList GetBroadcastMessages()
        {
            return CommonManager.Instance.GetBroadcastMessages(this);
        }

        public IList GetBroadcastMessagesbyuser()
        {
            return CommonManager.Instance.GetBroadcastMessagesbyuser(this);
        }
        public int updateBroadcastMessagesbyuser()
        {
            return CommonManager.Instance.updateBroadcastMessagesbyuser(this);
        }
        public IList<IReportContainer> GetAPIEntityDetails(bool IsshowFinancialDetl, bool IsDetailIncluded, bool IsshowTaskDetl, bool IsshowMemberDetl, int ExpandingEntityIDStr, bool IncludeChildrenStr, bool IsRootLevelEntity, int offsetstart = 0, int offsetend = 20)
        {
            return CommonManager.Instance.GetAPIEntityDetails(this, IsshowFinancialDetl, IsDetailIncluded, IsshowTaskDetl, IsshowMemberDetl, ExpandingEntityIDStr, IncludeChildrenStr, IsRootLevelEntity, offsetstart, offsetend);
        }

        public int InsertUpdateGanttHeaderBar(int Id, string name, string description, DateTime startdate, DateTime enddate, string colorcode)
        {
            return CommonManager.Instance.InsertUpdateGanttHeaderBar(this, Id, name, description, startdate, enddate, colorcode);
        }

        public IList<IGanttviewHeaderBar> GetAllGanttHeaderBar()
        {
            return CommonManager.Instance.GetAllGanttHeaderBar(this);
        }


        public bool DeleteGanttHeaderBar(int Id)
        {
            return CommonManager.Instance.DeleteGanttHeaderBar(this, Id);
        }

        /// <param name="year"> </param>
        /// <param name="month"> </param>
        /// <returns>Ilist </returns>
        public IList GetUniqueuserhit(int year, int month)
        {
            return CommonManager.Instance.GetUniqueuserhit(this, year, month);
        }

        /// <param name="year"> </param>
        /// <param name="month"> </param>
        /// <returns>Ilist </returns>
        public IList GetApplicationhit(int year, int month)
        {
            return CommonManager.Instance.GetApplicationhit(this, year, month);
        }


        /// <param name="year"> </param>
        /// <param name="month"> </param>
        /// <returns>Ilist </returns>
        public IList GetBrowserStatistic(int year, int month)
        {
            return CommonManager.Instance.GetBrowserStatistic(this, year, month);
        }

        public IList GetBrowserVersionStatistic(int year, int month)
        {
            return CommonManager.Instance.GetBrowserVersionStatistic(this, year, month);
        }

        public IList GetUserStatistic()
        {
            return CommonManager.Instance.GetUserStatistic(this);
        }

        public IList GetOSStatistic()
        {
            return CommonManager.Instance.GetOSStatistic(this);
        }

        public IList GetstartpageStatistic()
        {
            return CommonManager.Instance.GetstartpageStatistic(this);
        }

        public IList GetUserRoleStatistic()
        {
            return CommonManager.Instance.GetUserRoleStatistic(this);
        }

        public IList GetEnityStatistic()
        {
            return CommonManager.Instance.GetEnityStatistic(this);
        }

        public IList GetEnityCreateationStatistic(int year, int month)
        {
            return CommonManager.Instance.GetEnityCreateationStatistic(this, year, month);
        }
        public IList<IBandwithData> GetbandwidthStatistic(int year, int month)
        {
            return CommonManager.Instance.GetbandwidthStatistic(this, year, month);
        }

        public IList<ICurrencyConverter> getCurrencyconverterData()
        {
            return CommonManager.Instance.getCurrencyconverterData(this);

        }

        public bool Insertupdatecurrencyconverter(DateTime Startdate, DateTime Enddate, string Currencytype, double Currencyrate, int id)
        {
            return CommonManager.Instance.Insertupdatecurrencyconverter(this, Startdate, Enddate, Currencytype, Currencyrate, id);
        }

        public bool DeleteCurrencyconverterData(int id)
        {
            return CommonManager.Instance.DeleteCurrencyconverterData(this, id);
        }

        public IList<ICurrencyConverter> GetRatesByID(int id)
        {
            return CommonManager.Instance.GetRatesByID(this, id);
        }
        public IList<ICurrencyConverter> GetExchangesratesbyCurrencytype(int id)
        {
            return CommonManager.Instance.GetExchangesratesbyCurrencytype(this, id);
        }

        public IList<ICustomTab> GetCustomTabsByTypeID(int TypeID)
        {
            return CommonManager.Instance.GetCustomTabsByTypeID(this, TypeID);
        }

        public int[] InsertUpdateCustomTab(int ID, int Typeid, string Name, string ExternalUrl, bool AddEntityID, bool AddLanguageCode, bool AddUserEmail, bool AddUserName, bool AddUserID, int tabencryID, string encryKey, string encryIV, string algorithm, string paddingMode, string cipherMode, string entitytypeids, string globalids)
        {
            return CommonManager.Instance.InsertUpdateCustomTab(this, ID, Typeid, Name, ExternalUrl, AddEntityID, AddLanguageCode, AddUserEmail, AddUserName, AddUserID, tabencryID, encryKey, encryIV, algorithm, paddingMode, cipherMode, entitytypeids, globalids);
        }

        public bool DeleteCustomtabByID(int ID, int attributetypeid, int entitytypeid)
        {
            return CommonManager.Instance.DeleteCustomtabByID(this, ID, attributetypeid, entitytypeid);
        }

        public bool UpdateCustomTabSortOrder(int ID, int sortorder)
        {
            return CommonManager.Instance.UpdateCustomTabSortOrder(this, ID, sortorder);
        }

        public int InsertEntityAttachmentsVersion(int EntityID, IList<IAttachments> EntityAttachments, IList<IFile> EntityFiles, int FileID, int VersioningFileId)
        {
            return CommonManager.Instance.InsertEntityAttachmentsVersion(this, EntityID, EntityAttachments, EntityFiles, FileID, VersioningFileId);
        }

        public bool InsertUpdateApplicationUrlTrack(Guid TrackID, string TrackValue)
        {
            return CommonManager.Instance.InsertUpdateApplicationUrlTrack(this, TrackID, TrackValue);
        }

        public string GetApplicationUrlTrackByID(Guid TrackID)
        {
            return CommonManager.Instance.GetApplicationUrlTrackByID(this, TrackID);
        }

        public IList<ICustomTab> GetCustomEntityTabsByTypeID(int TypeID, int CalID, int EntityTypeId = 0, int EntityID = 0)
        {
            return CommonManager.Instance.GetCustomEntityTabsByTypeID(this, TypeID, CalID, EntityTypeId, EntityID);
        }


        public string GetCustomTabUrlTabsByTypeID(int tabID, int entityID)
        {
            return CommonManager.Instance.GetCustomTabUrlTabsByTypeID(this, tabID, entityID);
        }

        public bool UpdateCustomTabSettings(string key, string iv, string algo, string paddingmode, string ciphermode, string tokenmode)
        {
            return CommonManager.Instance.UpdateCustomTabSettings(this, key, iv, algo, paddingmode, ciphermode, tokenmode);
        }

        public IList<SSO> GetCustomTabSettingDetails()
        {
            return CommonManager.Instance.GetCustomTabSettingDetails(this);
        }


        public IConvertedcurrencies GetConvertedcurrencies(int Amount, int id, string Currencytype, DateTime Duedate)
        {
            return CommonManager.Instance.GetConvertedcurrencies(this, Amount, id, Currencytype, Duedate);
        }

        public IList<IConvertedcurrencies> CurrencyConvertJSON(JObject curr)
        {
            return CommonManager.Instance.CurrencyConvertJSON(this, curr);
        }
        public Tuple<IUpdateSettings, string> GetUpdateSettings()
        {
            return CommonManager.Instance.GetUpdateSettings(this);
        }

        public IList<PasswordSetting> GetPasswordPolicyDetails()
        {
            return CommonManager.Instance.GetPasswordPolicyDetails(this);
        }

        public bool UpdatePasswordPolicy(string MinLength, string Maxlength, string Numlength, string UpperLength, string SpecialLength, string SpecialChars, string BarWidth, string MultipleColors)
        {
            return CommonManager.Instance.UpdatePasswordPolicy(this, MinLength, Maxlength, Numlength, UpperLength, SpecialLength, SpecialChars, BarWidth, MultipleColors);
        }

        public Hashtable GetPlantabsettings()
        {
            return CommonManager.Instance.GetPlantabsettings(this);
        }

        public bool UpdatePlanTabsettings(string jsondata)
        {
            return CommonManager.Instance.UpdatePlanTabsettings(this, jsondata);
        }

        public IList<ITabEncryption> GetCustomTabEncryptionByID()
        {
            return CommonManager.Instance.GetCustomTabEncryptionByID(this);
        }

        /// <summary>
        /// Get Optimaker settings Address points.
        /// </summary>        /// <param name="proxy">The proxy.</param>
        /// <returns>String</returns>
        public string GetOptimakerAddresspoints()
        {
            return CommonManager.Instance.GetOptimakerAddresspoints(this);
        }

        public bool IsAvailableAsset(int AssetID)
        {
            return CommonManager.Instance.IsAvailableAsset(this, AssetID);
        }

        public IList<ICustomTabEntityTypeAcl> GetCustomTabAccessByID(int TabID)
        {
            return CommonManager.Instance.GetCustomTabAccessByID(this, TabID);
        }

        public string GetAdminLayoutSettings(string LogoSettings, int typeid)
        {
            return CommonManager.Instance.GetAdminLayoutSettings(this, LogoSettings, typeid);
        }

        public string GetAdminLayoutFinSettings(string LogoSettings, int typeid)
        {
            return CommonManager.Instance.GetAdminLayoutFinSettings(this, LogoSettings, typeid);
        }
        public string GetAdminLayoutObjectiveSettings(string LogoSettings, int typeid)
        {
            return CommonManager.Instance.GetAdminLayoutObjectiveSettings(this, LogoSettings, typeid);
        }
        public bool LayoutDesign(string jsondata, string key, int typeid)
        {
            return CommonManager.Instance.LayoutDesign(this, jsondata, key, typeid);
        }
        public IList<IAttribute> GetAttributeSearchCriteria(int TypeID)
        {
            return CommonManager.Instance.GetAttributeSearchCriteria(this, TypeID);
        }

        public bool SearchadminSettingsforRootLevelInsertUpdate(string jsondata, string LogoSettings, string key, int typeid)
        {
            return CommonManager.Instance.SearchadminSettingsforRootLevelInsertUpdate(this, jsondata, LogoSettings, key, typeid);

        }


        public IList<ICustomTab> GetCustomEntityTabsfrCalID(int TypeID, int CalID)
        {
            return CommonManager.Instance.GetCustomEntityTabsfrCalID(this, TypeID, CalID);
        }
        public int[] GetSearchCriteriaTypesIds(int searchtype)
        {
            return CommonManager.Instance.GetSearchCriteriaTypesIds(this, searchtype);
        }
        public IList<ICustomTab> GetCustomEntityTabsByTypeID(int TypeID)
        {
            return CommonManager.Instance.GetCustomEntityTabsByTypeID(this, TypeID);
        }
        public bool LayoutSettingsApplyChanges(string TabType, string TabLocation)
        {
            return CommonManager.Instance.LayoutSettingsApplyChanges(this, TabType, TabLocation);
        }

        public List<object> GetProofHQSSettings()
        {
            return CommonManager.Instance.GetProofHQSSettings(this);
        }

        public bool UpdateProofHQSSettings(string userName, string password)
        {
            return CommonManager.Instance.UpdateProofHQSSettings(this, userName, password);

        }
        public bool UpdateExpirytime(string ActualTime)
        {
            return CommonManager.Instance.UpdateExpirytime(this, ActualTime);
        }
        public string Getexpirytime()
        {
            return CommonManager.Instance.Getexpirytime(this);
        }
        public string GetCalendarNavigationConfig()
        {
            return CommonManager.Instance.GetCalendarNavigationConfig(this);
        }
    }

}
