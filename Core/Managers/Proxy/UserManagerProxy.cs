using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrandSystems.Marcom.Core.Interface;
using System.Collections;
using BrandSystems.Marcom.Core.Interface.Managers;
using BrandSystems.Marcom.Core.User.Interface;
using BrandSystems.Marcom.Core.Planning.Interface;

namespace BrandSystems.Marcom.Core.Managers.Proxy
{
    internal partial class UserManagerProxy : IUserManager, IManagerProxy
    {
        // Reference to the MarcomManager
        private IMarcomManager _marcomManager = null;

        // Example of cache for the logged in user's things
        /// <summary>
        /// The user groups for logged in user
        /// </summary>
        private IList _userGroupsForLoggedInUser = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserManagerProxy"/> class.
        /// </summary>
        /// <param name="marcomManager">The marcom manager.</param>
        public UserManagerProxy(IMarcomManager marcomManager)
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

        public Boolean TaskFeedLock { get; set; }

        public Boolean OverviewFeedLock { get; set; }

        public Boolean AssetFeedLock { get; set; }
        /// <summary>
        /// Initializes the I user.
        /// </summary>
        /// <param name="strbody">The strbody.</param>
        /// <returns>
        /// IUser
        /// </returns>
        public IUser initializeIUser(string strbody)
        {
         return   UserManager.Instance.initializeIUser(strbody);
        }

        /// <summary>
        ///  Inserts Users.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>
        /// string
        /// </returns>
        public int User_Insert(IUser user)
        {
          return  UserManager.Instance.User_Insert(this, user);
        }
  

        /// <summary>
        /// Inserts Users.
        /// </summary>
        /// <param name="Email">The email.</param>
        /// <param name="FirstName">The first name.</param>
        /// <param name="LastName">The last name.</param>
        /// <param name="Image">The image.</param>
        /// <param name="Language">The language.</param>
        /// <param name="Password">The password.</param>
        /// <param name="StartPage">The start page.</param>
        /// <param name="timezone">The timezone.</param>
        /// <param name="UserName">Name of the user.</param>
        /// <returns>int</returns>
        public int User_Insert(string Email, string FirstName, string LastName, string Image, string Language, string Password, int? StartPage, string timezone, string UserName, int DashboardTemplateID, bool IsApiUser, IList<IAttributeData> entityattributedata)
        {
            return UserManager.Instance.User_Insert(this, Email, FirstName, LastName, Image, Language, Password, StartPage, timezone, UserName, DashboardTemplateID, IsApiUser, entityattributedata);
        }

        /// <summary>
        /// Update users.
        /// </summary>
        /// <param name="usermgr">The usermgr.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool User_Update( IUser usermgr)
        {
            return UserManager.Instance.User_Update(this, usermgr);
        }

        /// <summary>
        /// Update users.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="Email">The email.</param>
        /// <param name="FirstName">The first name.</param>
        /// <param name="LastName">The last name.</param>
        /// <param name="Image">The image.</param>
        /// <param name="Language">The language.</param>
        /// <param name="StartPage">The start page.</param>
        /// <param name="timezone">The timezone.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool User_Update(int id, string Email, string FirstName, string LastName, string Image, string Language, int? StartPage, string timezone, int DashboardTemplateID, bool IsSSOUser, bool IsApiUser, IList<IAttributeData> entityattributedata)
        {
            return UserManager.Instance.User_Update(this, id, Email, FirstName, LastName, Image, Language, StartPage, timezone, DashboardTemplateID, IsSSOUser, IsApiUser, entityattributedata);
        }

        /// <summary>
        /// Select Users by ID.
        /// </summary>
        /// <param name="userid">The userid.</param>
        /// <returns>
        /// IUser
        /// </returns>
        public IUser User_SelectByID(int userid)
        {
            return UserManager.Instance.User_SelectByID(this, userid);
        }

        public Tuple<List<int>, List<string>> GetStartpages()
        {
            return UserManager.Instance.GetStartpages(this); 
        }
        /// <summary>
        /// Valids the user.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <returns>
        /// IUser
        /// </returns>
        public IUser valid_User(string userName, string password)
        {
            return UserManager.Instance.valid_User(this, userName, password);
        }

        /// <summary>
        /// Delete Users by ID.
        /// </summary>
        /// <param name="userid">The userid.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool User_DeleteByID( int userid)
        {
            return UserManager.Instance.User_DeleteByID(this, userid);
        }

        /// <summary>
        /// Check User Involvement in entity by ID.
        /// </summary>
        /// <param name="userid">The userid.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool User_CheckUserInvolvementByID(int userid)
        {
            return UserManager.Instance.User_CheckUserInvolvementByID(this, userid);
        }

        /// <summary>
        /// Users select name by ID.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>
        /// string
        /// </returns>
       public string User_SelectNameByID( int userId)
        {
            return UserManager.Instance.User_SelectNameByID(this, userId);
        }

       /// <summary>
       /// Returning user selection feeds for Entity.
       /// </summary>
       /// <param name="UseriD">The user ID</param>
       /// <returns>string</returns>
        public string UserEntityselections(int userId)
        {
            return UserManager.Instance.UserEntityselections(this, userId);
        }

        /// <summary>
        /// Updating user selection feeds for Entity.
        /// </summary>
        /// <param name="UseriD">The user ID</param>
        /// <param name="Feedselection">User selection Entitytpes as comma separated</param>
        /// <returns>bool</returns>
        public bool UserFeedselectionUpdate(int userId, string feedSelection)
        {
            return UserManager.Instance.UserFeedselectionUpdate(this, userId, feedSelection);
        }  

          /// <summary>
        /// User select.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <returns>IList</returns>
        public IList<IUser> GetUsers()
        {
            return UserManager.Instance.GetUsers(this);
        }
        /// <summary>
        /// Pending users for admin
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <returns>IList</returns>
        public Tuple<IList<IPendingUser>, IList<IPendingUser>> GetPendingUsers()
        {
            return UserManager.Instance.GetPendingUsers(this);
        }

        public bool UpdateUsersToRegister(string selectedusers, bool status)
        {
            return UserManager.Instance.UpdateUsersToRegister(this, selectedusers, status);
        }
        public IList<IUser> GetUserByEntityID(int EntityID)
        {
            return UserManager.Instance.GetUserByEntityID(this, EntityID);
        }
        public IList<IEntityUsers> GetMembersByEntityID(int EntityID)
        {
            return UserManager.Instance.GetMembersByEntityID(this, EntityID);
        }
        public IList<IUser> GetMemberList(string queryString, int GroupID)
        {
            return UserManager.Instance.GetMemberList(this, queryString, GroupID);
        }

        /// <summary>
        /// Get UserDateTime as per user's time zone
        /// </summary>
        /// <returns></returns>
        public string UserDateTime()
        {
            TimeSpan offSet = TimeSpan.Parse(this.MarcomManager.User.TimeZone);
            return (DateTime.UtcNow + offSet).ToString("dd-MMM-yyyy, HH:mm:ss");
        }

        /// <summary>
        /// FeedInitialRequestedTime
        /// </summary>
        public DateTimeOffset FeedInitialRequestedTime { get; set; }

        /// <summary>
        /// FeedRecentlyUpdatedTime
        /// </summary>
        public DateTimeOffset FeedRecentlyUpdatedTime { get; set; }

        /// <summary>
        /// TaskAndFundingRequestFeedInitialRequestedTime
        /// </summary>
        public DateTimeOffset TaskFeedInitialRequestedTime { get; set; }

        /// <summary>
        /// TaskAndFundingRequestFeedRecentlyUpdatedTime
        /// </summary>
        public DateTimeOffset TaskFeedRecentlyUpdatedTime { get; set; }


        ///AssetFeedInitialRequestedTime
        public DateTimeOffset AssetFeedInitialRequestedTime { get; set; }

        /// <summary>
        /// AssetFeedRecentlyUpdatedTime
        /// </summary>
        public DateTimeOffset AssetFeedRecentlyUpdatedTime { get; set; }


      /// <summary>
        /// Update user by column.
        /// </summary>
        /// <param name="usermgr">The usermgr.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool Userinfo_UpdateByColumn(string ColumnName, string ColumnValue)
        {
            return UserManager.Instance.Userinfo_UpdateByColumn(this, ColumnName, ColumnValue);
        }

        /// <summary>
        /// save users image.
        /// </summary>
        /// <param name="source img">The source img.</param>
        /// <param name="destination img">The destination img.</param>
        /// <param name="imgwidth img">The imgwidth img.</param>
        /// <param name="imgheight img">The imgheight img.</param>
        /// <param name="imgX img">The imgX img.</param>
        /// <param name="imgY img">The imgY img.</param>
        /// <returns>bool</returns>
        public bool SaveUserImage(string sourcepath,  int imgwidth, int imgheight, int imgX, int imgY)
        {
            return UserManager.Instance.SaveUserImage(this, sourcepath,  imgwidth, imgheight, imgX, imgY);
        }

        public bool insetlogin(int UserID, string IPAddress,  string Browser, string Version, int MajorVersion, string MinorVersion, string OS, bool IIsSSO)
        {
            return UserManager.Instance.insetlogin(this,UserID,IPAddress, Browser, Version, MajorVersion, MinorVersion, OS, IIsSSO);
        }

        /// <summary>
        /// Getting Objective Users.
        /// </summary>
        /// <param name="typeId">The EntityTypeId.</param>
        /// <returns>IList of Users</returns>
        public IList<IUser> GetAllObjectiveMembers(int entityTypeId)
        {
            return UserManager.Instance.GetAllObjectiveMembers(this, entityTypeId);
        }

        public bool UpdateNewLanguageType(int UserID, int langtypeid)
        {
            return UserManager.Instance.UpdateNewLanguageType(this, UserID, langtypeid);
        }

        public int IsSSOUser()
        {
        return UserManager.Instance.IsSSOUser(this);
        }


        public IList GetAPIusersDetails()
        {
            return UserManager.Instance.GetAPIusersDetails(this);
        }


        public bool GenerateGuidforSelectedAPI(int[] userIds)
        {
            return UserManager.Instance.GenerateGuidforSelectedAPI(this, userIds);
        }
    }
}
