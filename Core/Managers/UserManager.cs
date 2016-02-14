using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrandSystems.Marcom.Core.Interface;
using NHibernate.UserTypes;
using BrandSystems.Marcom.Core.Interface.Managers;
using BrandSystems.Marcom.Dal.User.Model;
using BrandSystems.Marcom.Core.Managers.Proxy;
using BrandSystems.Marcom.Dal.User;
using BrandSystems.Marcom.Core.User.Interface;
using Newtonsoft.Json.Linq;
using BrandSystems.Marcom.Core.Access;
using BrandSystems.Marcom.Dal.Base;
using BrandSystems.Marcom.Dal.Access.Model;
using System.Data;
using System.Data.SqlClient;
using BrandSystems.Marcom.Utility;
using BrandSystems.Marcom.Dal.Base;
using System.Collections;
using System.IO;
using System.Drawing.Drawing2D;
using SD = System.Drawing;
using System.Web;
using System.Net.Mail;
using System.Threading.Tasks;
using BrandSystems.Marcom.Core.Metadata;
using System.Globalization;
using BrandSystems.Marcom.Dal.Metadata.Model;
using BrandSystems.Marcom.Dal.Task.Model;
using BrandSystems.Marcom.Dal.Common.Model;
using BrandSystems.Marcom.Core.Common;
using BrandSystems.Marcom.Core.Planning.Interface;
using BrandSystems.Marcom.Dal.Planning.Model;
using System.Text.RegularExpressions;

namespace BrandSystems.Marcom.Core.Managers
{
    internal partial class UserManager : IManager
    {
        /// <summary>
        /// The instance
        /// </summary>
        private static UserManager instance = new UserManager();

        // Example of simple caching (system wide cache)
        /// <summary>
        /// The _user types
        /// </summary>
        private Dictionary<long, IUserType> _userTypes = new Dictionary<long, IUserType>();

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        internal static UserManager Instance
        {
            get { return instance; }
        }

        /// <summary>
        /// Initializes the specified marcom manager.
        /// </summary>
        /// <param name="marcomManager">The marcom manager.</param>
        void IManager.Initialize(IMarcomManager marcomManager)
        {
            // Cache things here...
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

        /// <summary>
        /// Initializes the I user.
        /// </summary>
        /// <param name="strbody">The strbody.</param>
        /// <returns>IUser</returns>
        public IUser initializeIUser(string strbody)
        {
            JObject jobj = JObject.Parse(strbody.ToUpper());
            IUser user = new BrandSystems.Marcom.Core.User.User();
            user.Email = jobj["EMAIL"] == null ? "" : (string)jobj["EMAIL"];
            user.FirstName = jobj["FIRSTNAME"] == null ? "" : (string)jobj["FIRSTNAME"];
            user.Image = jobj["IMAGE"] == null ? "" : (string)jobj["IMAGE"];
            user.Language = jobj["LANGUAGE"] == null ? "" : (string)jobj["LANGUAGE"];
            user.LastName = jobj["LASTNAME"] == null ? "" : (string)jobj["LASTNAME"];
            user.Password = jobj["PASSWORD"] == null ? null : (byte[])jobj["PASSWORD"];
            user.SaltPassword = jobj["SALTPASSWORD"] == null ? "" : (string)jobj["SALTPASSWORD"];
            user.StartPage = jobj["STARTPAGE"] == null ? 0 : (int)jobj["STARTPAGE"];
            user.TimeZone = jobj["TIMEZONE"] == null ? "" : (string)jobj["TIMEZONE"];
            user.UserName = jobj["USERNAME"] == null ? "" : (string)jobj["USERNAME"];
            user.UserName = jobj["DashboardTemplateID"] == null ? "" : (string)jobj["DashboardTemplateID"];

            return user;
        }


        /// <summary>
        /// Insert Users.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="usermgr">The usermgr.</param>
        /// <returns>string</returns>
        public int User_Insert(UserManagerProxy proxy, IUser usermgr)
        {
            try
            {
                if (proxy.MarcomManager.AccessManager.CheckAccess(Modules.Admin, 4, FeatureID.Report, OperationId.Self) == true)
                {
                    using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                    {
                        if (tx.PersistenceManager.UserRepository.GetEquals<UserDao>(UserDao.PropertyNames.UserName, usermgr.UserName).Count == 0)
                        {
                            UserDao user = new UserDao();
                            user.Email = usermgr.Email;
                            user.FirstName = usermgr.FirstName;
                            user.LastName = usermgr.LastName;
                            user.Image = usermgr.Image;
                            user.Language = usermgr.Language;
                            user.Password = usermgr.Password;
                            user.SaltPassword = usermgr.SaltPassword;
                            user.StartPage = usermgr.StartPage;
                            user.TimeZone = usermgr.TimeZone;
                            user.UserName = usermgr.UserName;
                            user.DashboardTemplateID = usermgr.DashboardTemplateID;


                            tx.Commit();
                            return user.Id;
                        }
                        return 0;
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return 0;
        }

        /// <summary>
        /// Insert Users.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
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
        public int User_Insert(UserManagerProxy proxy, string Email, string FirstName, string LastName, string Image, string Language, string Password, int? StartPage, string timezone, string UserName, int DashboardTemplateID, bool IsApiUser, IList<IAttributeData> entityattributedata)
        {

            try
            {
                if (proxy.MarcomManager.AccessManager.CheckAccess(Modules.Admin, 4, FeatureID.Report, OperationId.Self) == true)
                {
                    using (ITransaction txpendinguser = proxy.MarcomManager.GetTransaction())
                    {

                        var pndStatus = txpendinguser.PersistenceManager.UserRepository.Query<PendingUserDao>().Where(a => a.Email == Email && a.ActivationStatus == "Pending").Select(a => a);
                        if (txpendinguser.PersistenceManager.UserRepository.GetEquals<UserDao>(UserDao.PropertyNames.Email, Email).Count != 0)
                        {
                            return 1;
                        }
                        if (pndStatus != null)
                        {
                            if (pndStatus.Count() != 0)
                            {
                                return 2;
                            }
                        }


                    }
                    PasswordSetting passwordSetting = Helper.GetPasswordSetting();
                    StringBuilder sbPasswordRegx = new StringBuilder(string.Empty);

                    //min and max
                    sbPasswordRegx.Append(@"(?=^.{" + passwordSetting.MinLength + "," + passwordSetting.MaxLength + "}$)");

                    //numbers length
                    sbPasswordRegx.Append(@"(?=(?:.*?\d){" + passwordSetting.NumsLength + "})");

                    //a-z characters
                    sbPasswordRegx.Append(@"(?=.*[a-z])");

                    //A-Z length
                    sbPasswordRegx.Append(@"(?=(?:.*?[A-Z]){" + passwordSetting.UpperLength + "})");

                    //special characters length
                    sbPasswordRegx.Append(@"(?=(?:.*?[" + passwordSetting.SpecialChars + "]){" + passwordSetting.SpecialLength + "})");

                    //(?!.*\s) - no spaces
                    //[0-9a-zA-Z!@#$%*()_+^&] -- valid characters
                    sbPasswordRegx.Append(@"(?!.*\s)[0-9a-zA-Z" + passwordSetting.SpecialChars + "]*$");
                    if (Regex.IsMatch(Password, sbPasswordRegx.ToString()))
                    {
                        using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                        {

                            if (tx.PersistenceManager.UserRepository.GetEquals<UserDao>(UserDao.PropertyNames.Email, Email).Count == 0)
                            {
                                UserDao user = new UserDao();
                                user.Email = Email;
                                user.FirstName = FirstName;
                                user.LastName = LastName;
                                user.Image = Image;
                                user.Language = Language;
                                user.SaltPassword = BCrypt.GenerateSalt();
                                user.Password = BCrypt.HashByteArrayPassword(Password, user.SaltPassword);
                                user.StartPage = StartPage;
                                user.TimeZone = timezone;
                                user.UserName = UserName;
                                user.DashboardTemplateID = DashboardTemplateID;
                                user.IsAPIUser = IsApiUser;


                                //user.Gender = Convert.ToBoolean(Gender);

                                IList<EntityTypeDao> feedselectiontypes = new List<EntityTypeDao>();
                                feedselectiontypes = tx.PersistenceManager.MetadataRepository.GetAll<EntityTypeDao>();
                                string entitytypes = string.Join(", ", (from mm in feedselectiontypes select mm.Id).ToArray());


                                user.FeedSelection = entitytypes;

                                IList<MultiProperty> prpList = new List<MultiProperty>();
                                prpList.Add(new MultiProperty { propertyName = UserDao.PropertyNames.Email, propertyValue = Email });
                                prpList.Add(new MultiProperty { propertyName = UserDao.PropertyNames.UserName, propertyValue = UserName });
                                var existinguser = tx.PersistenceManager.AccessRepository.GetEquals<UserDao>(prpList);


                                if (existinguser.Count() == 0 && existinguser.Count() != null)
                                {
                                    tx.PersistenceManager.UserRepository.Save<UserDao>(user);
                                    if (entityattributedata != null && entityattributedata.Count() > 0)
                                        InsertUserDetailsAttributes(tx, entityattributedata, user.Id, (int)EntityTypeList.UserDetails);
                                }
                                else
                                {
                                    return 1;
                                }

                                tx.Commit();
                                if (user.IsAPIUser)
                                {
                                    using (ITransaction txUserApi = proxy.MarcomManager.GetTransaction())
                                    {

                                        UserAPIInterfaceDao apidao = new UserAPIInterfaceDao();
                                        apidao.APIGuid = Guid.NewGuid().ToString();
                                        apidao.UserID = user.Id;
                                        txUserApi.PersistenceManager.UserRepository.Save<UserAPIInterfaceDao>(apidao);
                                        txUserApi.Commit();

                                    }
                                }

                                using (ITransaction txmail = proxy.MarcomManager.GetTransaction())
                                {

                                    UserTaskNotificationMailSettingsDao dao = new UserTaskNotificationMailSettingsDao();
                                    dao.IsEmailEnable = true;
                                    dao.IsNotificationEnable = true;
                                    dao.LastUpdatedOn = DateTimeOffset.UtcNow;
                                    dao.NoOfDays = 0;
                                    dao.Userid = user.Id;
                                    txmail.PersistenceManager.TaskRepository.Save<UserTaskNotificationMailSettingsDao>(dao);
                                    string defaultNotificationSubscriptionTypes = "";
                                    string defaultMailSubscriptionTypes = "";
                                    IList<SubscriptionTypeDao> subscriptiontypes = new List<SubscriptionTypeDao>();
                                    subscriptiontypes = (from defaulttypes in tx.PersistenceManager.CommonRepository.Query<SubscriptionTypeDao>() where defaulttypes.isAppDefault || defaulttypes.isMailDefault select defaulttypes).ToList<SubscriptionTypeDao>();
                                    defaultNotificationSubscriptionTypes = string.Join(", ", (from defaultnotifications in subscriptiontypes where defaultnotifications.isAppDefault select defaultnotifications.Id).ToArray());
                                    defaultMailSubscriptionTypes = string.Join(", ", (from defaultnotifications in subscriptiontypes where defaultnotifications.isMailDefault select defaultnotifications.Id).ToArray());
                                    UserDefaultSubscriptionDao userdefsubscriptiondet = new UserDefaultSubscriptionDao();
                                    userdefsubscriptiondet.UserID = user.Id;
                                    userdefsubscriptiondet.SubscriptionTypeID = defaultNotificationSubscriptionTypes;
                                    userdefsubscriptiondet.MailSubscriptionTypeID = defaultMailSubscriptionTypes;
                                    txmail.PersistenceManager.CommonRepository.Save<UserDefaultSubscriptionDao>(userdefsubscriptiondet);

                                    UserMailSubscriptionDao usermailsub = new UserMailSubscriptionDao();
                                    usermailsub = (from item in tx.PersistenceManager.MetadataRepository.Query<UserMailSubscriptionDao>() where item.Userid == user.Id select item).ToList<UserMailSubscriptionDao>().FirstOrDefault();
                                    if (usermailsub == null)
                                    {
                                        UserMailSubscriptionDao usermailsub1 = new UserMailSubscriptionDao();
                                        usermailsub1.Userid = user.Id;
                                        usermailsub1.LastUpdatedOn = System.DateTime.UtcNow;
                                        usermailsub1.IsEmailEnable = false;
                                        usermailsub1.DayName = "";
                                        usermailsub1.Timing = TimeSpan.Parse("00:00");
                                        usermailsub1.RecapReport = false;
                                        txmail.PersistenceManager.CommonRepository.Save<UserMailSubscriptionDao>(usermailsub1);
                                    }
                                    txmail.Commit();
                                }
                                return user.Id;
                            }
                            else
                            {
                                return 1;
                            }
                        }
                    }
                    else
                    {
                        return -100;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return 0;
        }
        public bool InsertUserDetailsAttributes(ITransaction tx, IList<IAttributeData> attributeData, int UserID, int typeId)
        {
            if (attributeData != null)
            {
                string entityName = "AttributeRecord" + typeId + "_V" + MarcomManagerFactory.ActiveMetadataVersionNumber;
                IList<IDynamicAttributes> listdynamicattributes = new List<IDynamicAttributes>();
                Dictionary<string, object> dictAttr = new Dictionary<string, object>();
                IList<UserDetailsMultiSelectDao> listMultiselect = new List<UserDetailsMultiSelectDao>();
                IList<UserDetailsTreeValueDao> listreeval = new List<UserDetailsTreeValueDao>();
                listreeval.Clear();
                BrandSystems.Marcom.Dal.Planning.Model.DynamicAttributesDao dynamicdao = new BrandSystems.Marcom.Dal.Planning.Model.DynamicAttributesDao();

                ArrayList entityids = new ArrayList();
                foreach (var obj in attributeData)
                {
                    entityids.Add(obj.ID);
                }
                var result = from item in tx.PersistenceManager.PlanningRepository.Query<AttributeDao>() where entityids.Contains(item.Id) select item;
                var entityTypeCategory = tx.PersistenceManager.PlanningRepository.Query<EntityTypeDao>().Where(a => a.Id == typeId).Select(a => a.Category).FirstOrDefault();
                var dynamicAttResult = result.Where(a => ((a.Id != 69) && (a.AttributeTypeID == 1 || a.AttributeTypeID == 2 || a.AttributeTypeID == 3 || a.AttributeTypeID == 5 || a.AttributeTypeID == 8 || a.AttributeTypeID == 9 || a.AttributeTypeID == 11)));
                var treevalResult = result.Where(a => a.AttributeTypeID == (Convert.ToInt32(AttributesList.DropDownTree)));
                var multiAttrResult = result.Where(a => a.AttributeTypeID == (Convert.ToInt32(AttributesList.ListMultiSelection)));
                var multiselecttreevalResult = result.Where(a => a.AttributeTypeID == (Convert.ToInt32(AttributesList.TreeMultiSelection)));

                if (treevalResult.Count() > 0)
                {
                    var treeValQuery = attributeData.Join(treevalResult,
                                 post => post.ID,
                                 meta => meta.Id,
                                 (post, meta) => new { databaseval = post });
                    if (treeValQuery.Count() > 0)
                    {
                        foreach (var treeattr in treeValQuery)
                        {
                            foreach (var treevalobj in treeattr.databaseval.Value)
                            {
                                UserDetailsTreeValueDao tre = new UserDetailsTreeValueDao();
                                tre.Attributeid = treeattr.databaseval.ID;
                                tre.UserID = UserID;
                                tre.Nodeid = treevalobj;
                                tre.Level = treeattr.databaseval.Level;
                                listreeval.Add(tre);
                            }
                        }
                        tx.PersistenceManager.UserRepository.Save<UserDetailsTreeValueDao>(listreeval);
                    }
                }
                if (multiselecttreevalResult.Count() > 0)
                {
                    var multiselecttreeValQuery = attributeData.Join(multiselecttreevalResult,
                                 post => post.ID,
                                 meta => meta.Id,
                                 (post, meta) => new { databaseval = post });
                    if (multiselecttreeValQuery.Count() > 0)
                    {
                        foreach (var treeattr in multiselecttreeValQuery)
                        {
                            foreach (var treevalobj in treeattr.databaseval.Value)
                            {
                                UserDetailsTreeValueDao tre = new UserDetailsTreeValueDao();
                                tre.Attributeid = treeattr.databaseval.ID;
                                tre.UserID = UserID;
                                tre.Nodeid = treevalobj;
                                tre.Level = treeattr.databaseval.Level;
                                listreeval.Add(tre);
                            }
                        }
                        tx.PersistenceManager.UserRepository.Save<UserDetailsTreeValueDao>(listreeval);
                    }
                }
                if (multiAttrResult.Count() > 0)
                {

                    tx.PersistenceManager.PlanningRepository.DeleteByID<UserDetailsMultiSelectDao>(UserID);

                    string deletequery = "DELETE FROM MM_UserDetailsMultiSelect WHERE UserID = ? ";
                    tx.PersistenceManager.UserRepository.ExecuteQuerywithMinParam(deletequery.ToString(), Convert.ToInt32(UserID));

                    var query = attributeData.Join(multiAttrResult,
                             post => post.ID,
                             meta => meta.Id,
                             (post, meta) => new { databaseval = post, attrappval = meta });
                    foreach (var at in query)
                    {
                        UserDetailsMultiSelectDao mt = new UserDetailsMultiSelectDao();
                        mt.Attributeid = at.databaseval.ID;
                        mt.UserID = UserID;
                        mt.Optionid = Convert.ToInt32(at.databaseval.Value);
                        listMultiselect.Add(mt);
                    }
                    tx.PersistenceManager.PlanningRepository.Save<UserDetailsMultiSelectDao>(listMultiselect);
                }

                if (dynamicAttResult.Count() > 0 || entityTypeCategory != 1)
                {
                    Dictionary<string, dynamic> attr = new Dictionary<string, dynamic>();


                    var dynamicAttrQuery = attributeData.Join(dynamicAttResult,
                                post => post.ID,
                                meta => meta.Id,
                                (post, meta) => new { databaseval = post });
                    foreach (var ab in dynamicAttrQuery)
                    {

                        string key = Convert.ToString((int)ab.databaseval.ID);
                        int attributedataType = ab.databaseval.TypeID;
                        // dynamic value = ab.databaseval.Value;
                        dynamic value = null;
                        switch (attributedataType)
                        {
                            case 1:
                            case 2:
                            case 11:
                                {
                                    value = Convert.ToString(ab.databaseval.Value == null ? "" : HttpUtility.HtmlEncode((string)ab.databaseval.Value));
                                    break;
                                }
                            case 3:
                                {
                                    value = Convert.ToString((Convert.ToString(ab.databaseval.Value) == null || Convert.ToString(ab.databaseval.Value) == "") ? 0 : (int)ab.databaseval.Value);
                                    break;
                                }
                            case 5:
                                {
                                    value = DateTime.Parse(ab.databaseval.Value == null ? "" : (string)ab.databaseval.Value.ToString());
                                    break;
                                }
                            case 8:
                                {
                                    value = Convert.ToInt32(((ab.databaseval.Value == null) ? 0 : (int)ab.databaseval.Value));
                                    break;
                                }
                            case 9:
                                {
                                    value = value = Convert.ToBoolean(ab.databaseval.Value != "True" ? 0 : 1);
                                    break;
                                }
                        }
                        attr.Add(key, value);
                    }
                    dictAttr = attr != null ? attr : null;
                    dynamicdao.Id = UserID;
                    dynamicdao.Attributes = dictAttr;

                    tx.PersistenceManager.PlanningRepository.SaveByentity<BrandSystems.Marcom.Dal.Planning.Model.DynamicAttributesDao>(entityName, dynamicdao);
                }
            }
            return true;
        }


        /// <summary>
        /// Update users.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="usermgr">The usermgr.</param>
        /// <returns>bool</returns>
        public bool User_Update(UserManagerProxy proxy, IUser usermgr)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    if (tx.PersistenceManager.UserRepository.GetEquals<UserDao>(UserDao.PropertyNames.UserName, usermgr.UserName).Count == 0)
                    {
                        UserDao users = new UserDao();
                        users = tx.PersistenceManager.UserRepository.Get<UserDao>(usermgr.Id);
                        UserDao user = new UserDao();
                        user.Email = usermgr.Email.Length > 0 ? usermgr.Email : users.Email;
                        user.FirstName = usermgr.FirstName.Length > 0 ? usermgr.FirstName : users.FirstName;
                        user.LastName = usermgr.LastName.Length > 0 ? usermgr.LastName : users.LastName;
                        user.Image = usermgr.Image.Length > 0 ? usermgr.Image : users.Image;
                        user.Language = usermgr.Language.Length > 0 ? usermgr.Language : users.Language;
                        user.Password = usermgr.Password.Length > 0 ? usermgr.Password : users.Password;
                        user.SaltPassword = usermgr.SaltPassword.Length > 0 ? usermgr.SaltPassword : users.SaltPassword;
                        user.StartPage = usermgr.StartPage != 0 ? usermgr.StartPage : users.StartPage;
                        user.TimeZone = usermgr.TimeZone.Length > 0 ? usermgr.TimeZone : users.TimeZone;
                        user.UserName = usermgr.UserName.Length > 0 ? usermgr.UserName : users.UserName;
                        user.DashboardTemplateID = usermgr.DashboardTemplateID != 0 ? usermgr.DashboardTemplateID : users.DashboardTemplateID;
                        user.IsSSOUser = usermgr.IsSSOUser != false ? usermgr.IsSSOUser : users.IsSSOUser;
                        user.Id = usermgr.Id;
                        tx.PersistenceManager.UserRepository.Save<UserDao>(user);
                        tx.Commit();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return false;

        }

        /// <summary>
        /// Update users.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="id">The id.</param>
        /// <param name="Email">The email.</param>
        /// <param name="FirstName">The first name.</param>
        /// <param name="LastName">The last name.</param>
        /// <param name="Image">The image.</param>
        /// <param name="Language">The language.</param>
        /// <param name="StartPage">The start page.</param>
        /// <param name="timezone">The timezone.</param>
        /// <returns>bool</returns>
        public bool User_Update(UserManagerProxy proxy, int id, string Email, string FirstName, string LastName, string Image, string Language, int? StartPage, string timezone, int DashboardTemplateID, bool IsSSOUser, bool IsApiUser, IList<IAttributeData> entityattributedata)
        {
            try
            {
                if (proxy.MarcomManager.AccessManager.CheckAccess(Modules.Admin, 4, FeatureID.Report, OperationId.Self) == true)
                {
                    using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                    {
                        IList<MultiProperty> valList = new List<MultiProperty>();
                        if (Email.Length > 0)
                        {
                            MultiProperty val = new MultiProperty();
                            val.propertyName = UserDao.PropertyNames.Email;
                            val.propertyValue = Email;
                            valList.Add(val);
                        }
                        if (FirstName.Length > 0)
                        {
                            MultiProperty val = new MultiProperty();
                            val.propertyName = UserDao.PropertyNames.FirstName;
                            val.propertyValue = FirstName;
                            valList.Add(val);
                        }
                        if (LastName.Length > 0)
                        {
                            MultiProperty val = new MultiProperty();
                            val.propertyName = UserDao.PropertyNames.LastName;
                            val.propertyValue = LastName;
                            valList.Add(val);
                        }
                        if (Image.Length > 0)
                        {
                            MultiProperty val = new MultiProperty();
                            val.propertyName = UserDao.PropertyNames.Image;
                            val.propertyValue = Image;
                            valList.Add(val);
                        }
                        if (Language.Length > 0)
                        {
                            MultiProperty val = new MultiProperty();
                            val.propertyName = UserDao.PropertyNames.Language;
                            val.propertyValue = Language;
                            valList.Add(val);
                        }
                        if (StartPage != 0)
                        {
                            MultiProperty val = new MultiProperty();
                            val.propertyName = UserDao.PropertyNames.StartPage;
                            val.propertyValue = StartPage;
                            valList.Add(val);
                        }
                        if (timezone.Length > 0)
                        {
                            MultiProperty val = new MultiProperty();
                            val.propertyName = UserDao.PropertyNames.TimeZone;
                            val.propertyValue = timezone;
                            valList.Add(val);
                        }
                        if (DashboardTemplateID != 0)
                        {
                            MultiProperty val = new MultiProperty();
                            val.propertyName = UserDao.PropertyNames.DashboardTemplateID;
                            val.propertyValue = DashboardTemplateID;
                            valList.Add(val);
                        }

                        if (IsSSOUser)
                        {
                            MultiProperty val = new MultiProperty();
                            val.propertyName = UserDao.PropertyNames.IsSSOUser;
                            val.propertyValue = IsSSOUser;
                            valList.Add(val);
                        }

                        if (IsSSOUser == false)
                        {
                            MultiProperty val = new MultiProperty();
                            val.propertyName = UserDao.PropertyNames.IsSSOUser;
                            val.propertyValue = IsSSOUser;
                            valList.Add(val);
                        }

                        MultiProperty Newval = new MultiProperty();
                        Newval.propertyName = UserDao.PropertyNames.IsAPIUser;
                        Newval.propertyValue = IsApiUser;
                        valList.Add(Newval);

                        UserAPIInterfaceDao apidao = new UserAPIInterfaceDao();
                        apidao = tx.PersistenceManager.UserRepository.Get<UserAPIInterfaceDao>(UserAPIInterfaceDao.PropertyNames.UserID, id);
                        if (IsApiUser)
                        {

                            if (apidao == null)
                            {
                                apidao = new UserAPIInterfaceDao();
                                apidao.APIGuid = Guid.NewGuid().ToString();
                                apidao.UserID = id;
                                tx.PersistenceManager.UserRepository.Save<UserAPIInterfaceDao>(apidao);
                            }
                        }
                        else
                        {
                            if (apidao != null)
                            {
                                tx.PersistenceManager.UserRepository.DeleteByID<UserAPIInterfaceDao>(UserAPIInterfaceDao.PropertyNames.UserID, id);
                            }
                        }


                        IList<MultiProperty> condList = new List<MultiProperty>();
                        MultiProperty conVal = new MultiProperty();
                        conVal.propertyName = UserDao.PropertyNames.Id;
                        conVal.propertyValue = id;
                        condList.Add(conVal);
                        tx.PersistenceManager.UserRepository.UpdateByID<UserDao>(valList, condList);
                        if (entityattributedata != null && entityattributedata.Count() > 0)
                            InsertUserDetailsAttributes(tx, entityattributedata, id, (int)EntityTypeList.UserDetails);
                        tx.Commit();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return false;
        }

        /// <summary>
        /// User delete by ID.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="userid">The userid.</param>
        /// <returns>bool</returns>
        public bool User_DeleteByID(UserManagerProxy proxy, int userid)
        {
            try
            {
                UserDao userval = new UserDao();
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    tx.PersistenceManager.UserRepository.DeleteByID<UserDao>(userid);
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
        /// Check User Involvement in entity by ID.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="userid">The userid.</param>
        /// <returns>bool</returns>
        public bool User_CheckUserInvolvementByID(UserManagerProxy proxy, int userid)
        {
            using (ITransaction tx = proxy.MarcomManager.GetTransaction())
            {
                try
                {
                    UserDao userval = new UserDao();
                    var checkuserdependency = (from user in tx.PersistenceManager.AccessRepository.Query<EntityRoleUserDao>() where user.Userid == userid select user).FirstOrDefault();
                    if (checkuserdependency == null)
                    {
                        return true;
                    }
                    else
                    {

                        return false;
                    }

                }
                catch (Exception ex)
                {
                    return false;
                }
                finally
                {
                    tx.Commit();
                }
            }
            return false;
        }

        /// <summary>
        /// User select by ID.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="userid">The userid.</param>
        /// <returns>IUser</returns>
        public IUser User_SelectByID(UserManagerProxy proxy, int userid)
        {

            try
            {
                IUser users = new BrandSystems.Marcom.Core.User.User();
                UserDao userval = new UserDao();
                if (proxy.MarcomManager.AccessManager.CheckAccess(Modules.Admin, 4, FeatureID.Report, OperationId.Self) == true)
                {
                    using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                    {
                        userval = tx.PersistenceManager.UserRepository.Get<UserDao>(userid);
                        //tx.Commit();
                        users.Password = userval.Password;
                        users.SaltPassword = userval.SaltPassword;
                        users.Email = userval.Email;

                        users.FirstName = userval.FirstName;
                        users.LastName = userval.LastName;
                        users.Image = userval.Image;
                        users.Language = userval.Language;
                        users.StartPage = userval.StartPage;
                        users.TimeZone = userval.TimeZone;
                        users.UserName = userval.UserName;
                        users.Id = userval.Id;
                        users.Gender = userval.Gender;
                        if (userval.Phone == null || userval.Phone == "0")
                        {
                            userval.Phone = "-";
                        }
                        users.Phone = userval.Phone;
                        users.Address = userval.Address == null || userval.Address == "" ? "-" : userval.Address;
                        users.City = userval.City == null || userval.City == "" ? "-" : userval.City;
                        users.ZipCode = userval.ZipCode;
                        users.Website = userval.Website == null || userval.Website == "" ? "-" : userval.Website;
                        users.Feedselection = userval.FeedSelection;
                        users.DashboardTemplateID = userval.DashboardTemplateID;
                        users.Designation = userval.Designation == null || userval.Designation == "" ? "-" : userval.Designation;
                        users.Title = userval.Title == null || userval.Title == "" ? "-" : userval.Title;
                        users.LanguageSettings = userval.LanguageSettings;
                        users.IsSSOUser = userval.IsSSOUser;
                        users.IsAPIUser = userval.IsAPIUser;



                        var userresult = from itemRes in tx.PersistenceManager.PlanningRepository.Query<UserVisibleInfoDao>() select itemRes;
                        ArrayList arryAttributeids = new ArrayList();
                        foreach (var obj in userresult)
                        {
                            arryAttributeids.Add(obj.AttributeId);
                        }
                        var result = proxy.MarcomManager.MetadataManager.GetUserDetailsAttributes(12, userid);
                        if (result != null)
                        {
                            if (result.Count > 0)
                            {
                                var result1 = from item in result where arryAttributeids.Contains(item.AttributeID) select item;

                                if (result1 != null)
                                {
                                    int i = 0;
                                    int attrvalid = 0;
                                    foreach (var obj in result1)
                                    {
                                        if (obj.AttributeTypeID == 3)
                                        {
                                            if (obj.AttributeValue != null)
                                            {
                                                if (obj.AttributeValue.ToString() == "")
                                                {
                                                    if (i == 0)
                                                    {
                                                        users.QuickInfo1 = "-";
                                                        users.QuickInfo1AttributeCaption = (string)obj.AttributeCaption;
                                                    }
                                                    else
                                                    {
                                                        users.QuickInfo2 = "-";
                                                        users.QuickInfo2AttributeCaption = (string)obj.AttributeCaption;
                                                    }
                                                }
                                                else
                                                {
                                                    attrvalid = (int)obj.AttributeValue;
                                                    var singleCaption = (from item in tx.PersistenceManager.PlanningRepository.Query<OptionDao>() where item.Id == attrvalid select item.Caption).ToList();
                                                    if (i == 0)
                                                    {
                                                        users.QuickInfo1 = singleCaption[0].ToString();
                                                        users.QuickInfo1AttributeCaption = (string)obj.AttributeCaption;
                                                    }
                                                    else
                                                    {
                                                        users.QuickInfo2 = singleCaption[0].ToString();
                                                        users.QuickInfo2AttributeCaption = (string)obj.AttributeCaption;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (i == 0)
                                                {
                                                    users.QuickInfo1 = "-";
                                                    users.QuickInfo1AttributeCaption = (string)obj.AttributeCaption;
                                                }
                                                else
                                                {
                                                    users.QuickInfo2 = "-";
                                                    users.QuickInfo2AttributeCaption = (string)obj.AttributeCaption;
                                                }
                                            }
                                        }
                                        else if (obj.AttributeTypeID == 4)
                                        {
                                            var multiselectResult = tx.PersistenceManager.PlanningRepository.Query<Marcom.Dal.User.Model.UserDetailsMultiSelectDao>().Where(a => a.UserID == userid && a.Attributeid == Convert.ToInt32(obj.AttributeID)).Select(a => a.Optionid).ToList();

                                            var singleCaption = (from item in tx.PersistenceManager.PlanningRepository.Query<OptionDao>() where multiselectResult.Contains(item.Id) select item.Caption).ToList();
                                            if (singleCaption.Count > 0)
                                            {
                                                if (i == 0)
                                                {
                                                    users.QuickInfo1 = String.Join(",", singleCaption).ToString();
                                                    users.QuickInfo1AttributeCaption = (string)obj.AttributeCaption;
                                                }
                                                else
                                                {
                                                    users.QuickInfo2 = String.Join(",", singleCaption).ToString();
                                                    users.QuickInfo2AttributeCaption = (string)obj.AttributeCaption;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (i == 0)
                                            {
                                                users.QuickInfo1 = (string)obj.AttributeValue;
                                                users.QuickInfo1AttributeCaption = (string)obj.AttributeCaption;
                                            }
                                            else
                                            {
                                                users.QuickInfo2 = (string)obj.AttributeValue;
                                                users.QuickInfo2AttributeCaption = (string)obj.AttributeCaption;
                                            }
                                        }
                                        i++;
                                    }

                                }
                            }
                        }
                    }
                    return users;
                }
            }
            catch (Exception ex)
            {

            }

            return null;
        }

        /// <summary>
        /// User select.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <returns>IList</returns>
        public IList<IUser> GetUsers(UserManagerProxy proxy)
        {

            try
            {
                IList<IUser> users = new List<IUser>();
                if (proxy.MarcomManager.AccessManager.CheckAccess(Modules.Admin, 4, FeatureID.Report, OperationId.Self) == true)
                {
                    using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                    {
                        IList<UserDao> userdao = new List<UserDao>();
                        userdao = tx.PersistenceManager.UserRepository.GetAll<UserDao>();
                        tx.Commit();
                        foreach (var val in userdao.ToList())
                        {
                            IUser userVal = new BrandSystems.Marcom.Core.User.User();
                            userVal.Email = val.Email;
                            userVal.FirstName = val.FirstName;
                            userVal.Id = val.Id;
                            userVal.Image = val.Image;
                            userVal.Language = val.Language;
                            userVal.LastName = val.LastName;
                            userVal.Password = val.Password;
                            userVal.SaltPassword = val.SaltPassword;
                            userVal.StartPage = val.StartPage;
                            userVal.TimeZone = val.TimeZone;
                            userVal.UserName = val.UserName;
                            userVal.DashboardTemplateID = val.DashboardTemplateID;
                            userVal.Gender = val.Gender;
                            userVal.IsSSOUser = val.IsSSOUser;
                            userVal.IsAPIUser = val.IsAPIUser;
                            users.Add(userVal);
                        }
                    }
                    return users;
                }
            }
            catch (Exception ex)
            {

            }

            return null;
        }


        /// <summary>
        /// User select.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <returns>IList</returns>
        public Tuple<IList<IPendingUser>, IList<IPendingUser>> GetPendingUsers(UserManagerProxy proxy)
        {

            try
            {
                IList<IPendingUser> pendinuserslist = new List<IPendingUser>();
                IList<IPendingUser> approvedrejectedlist = new List<IPendingUser>();
                if (proxy.MarcomManager.AccessManager.CheckAccess(Modules.Admin, 4, FeatureID.Report, OperationId.Self) == true)
                {
                    using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                    {
                        IList<PendingUserDao> userdao = new List<PendingUserDao>();
                        userdao = tx.PersistenceManager.UserRepository.GetAll<PendingUserDao>();
                        IList<PendingUserDao> puserdao = new List<PendingUserDao>();
                        IList<PendingUserDao> aruserdao = new List<PendingUserDao>();
                        puserdao = (from t in userdao
                                    where t.ActivationStatus == "Pending"
                                    select t).ToList<PendingUserDao>();


                        foreach (var val in puserdao.ToList())
                        {
                            IPendingUser userVal = new BrandSystems.Marcom.Core.User.PendingUser();
                            userVal.Email = val.Email;
                            userVal.FirstName = val.FirstName;
                            userVal.Id = val.Id;
                            userVal.ActivationStatus = val.ActivationStatus;
                            userVal.Language = val.Language;
                            userVal.LastName = val.LastName;
                            userVal.Password = val.Password;
                            userVal.SaltPassword = val.SaltPassword;
                            userVal.TimeZone = val.TimeZone;
                            userVal.UserName = val.UserName;
                            userVal.CompanyName = val.CompanyName;
                            userVal.Title = val.Title;
                            userVal.Partners = val.Partners;
                            userVal.Department = val.Department;
                            pendinuserslist.Add(userVal);
                        }

                        aruserdao = (from r in userdao where !(from rr in puserdao select rr.Id).ToList().Contains(r.Id) select r).ToList();
                        //aruserdao = userdao.Except(puserdao);
                        foreach (var val in aruserdao.ToList())
                        {
                            IPendingUser userVal = new BrandSystems.Marcom.Core.User.PendingUser();
                            userVal.Email = val.Email;
                            userVal.FirstName = val.FirstName;
                            userVal.Id = val.Id;

                            userVal.Language = val.Language;
                            userVal.LastName = val.LastName;
                            userVal.Password = val.Password;
                            userVal.SaltPassword = val.SaltPassword;
                            userVal.TimeZone = val.TimeZone;
                            userVal.UserName = val.UserName;
                            userVal.ActivationStatus = val.ActivationStatus;
                            approvedrejectedlist.Add(userVal);
                        }
                        //tx.Commit();
                    }
                    //return users;
                    var tuple = Tuple.Create(pendinuserslist, approvedrejectedlist);

                    return tuple;
                }

            }
            catch (Exception ex)
            {

            }

            return null;
        }

        /// <summary>
        /// navigation select.for set start page
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <returns>IList</returns>
        public Tuple<List<int>, List<string>> GetStartpages(UserManagerProxy proxy)
        {


            List<int> enumvalues = new List<int>();
            List<string> enumnames = new List<string>();

            foreach (NavigationTypeID navtype in Enum.GetValues(typeof(NavigationTypeID)))
            {

                enumvalues.Add((int)navtype);
                enumnames.Add(navtype.ToString());
            }


            var tuple = Tuple.Create(enumvalues, enumnames);
            return tuple;


        }


        /// <summary>
        /// User select.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <returns>IList</returns>
        public bool UpdateUsersToRegister(UserManagerProxy proxy, string selectedusers, bool status)
        {

            try
            {
                IList<UserDao> users1 = new List<UserDao>();
                PendingUserDao puserdao = new PendingUserDao();
                IList<IPendingUser> users = new List<IPendingUser>();
                if (proxy.MarcomManager.AccessManager.CheckAccess(Modules.Admin, 4, FeatureID.Report, OperationId.Self) == true)
                {


                    IEnumerable<Hashtable> listresult;
                    StringBuilder strqry = new StringBuilder();


                    using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                    {

                        if (status == true)
                            puserdao.ActivationStatus = "Approved";
                        else
                            puserdao.ActivationStatus = "Rejected";

                        strqry.AppendLine("update UM_Pending_User set ActivationStatus='" + puserdao.ActivationStatus + "' where  id in (" + selectedusers + ") ");
                        tx.PersistenceManager.UserRepository.CreateQuery(strqry.ToString());
                        //tx.Commit();
                        //}
                        // using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                        //    {


                        IList<PendingUserDao> userdao = new List<PendingUserDao>();


                        userdao = tx.PersistenceManager.UserRepository.GetAll<PendingUserDao>();
                        int[] ids = selectedusers.Split(',').Select(int.Parse).ToArray();
                        userdao = ((from t in userdao where t.ActivationStatus == "Approved" select t).ToList()).Where(p => ids.Contains(p.Id)).ToList();

                        IList<EntityTypeDao> feedselectiontypes = new List<EntityTypeDao>();
                        feedselectiontypes = tx.PersistenceManager.MetadataRepository.GetEquals<EntityTypeDao>(EntityTypeDao.PropertyNames.IsAssociate, false);
                        string entitytypes = string.Join(", ", (from mm in feedselectiontypes select mm.Id).ToArray());

                        foreach (var approveduser in userdao)
                        {
                            UserDao usrdao = new UserDao();
                            usrdao.FirstName = approveduser.FirstName;
                            usrdao.LastName = approveduser.LastName;
                            usrdao.Password = approveduser.Password;
                            usrdao.SaltPassword = approveduser.SaltPassword;
                            usrdao.UserName = approveduser.UserName;
                            usrdao.TimeZone = "+01:00";
                            usrdao.Email = approveduser.Email;
                            usrdao.Gender = Convert.ToBoolean(approveduser.Gender);
                            usrdao.Designation = approveduser.Department;
                            usrdao.Title = approveduser.Title;
                            usrdao.StartPage = 0;
                            usrdao.FeedSelection = entitytypes;
                            usrdao.Language = approveduser.Language;
                            users1.Add(usrdao);
                        }
                        tx.PersistenceManager.UserRepository.Save<UserDao>(users1);

                        IList<GlobalRoleUserDao> provideaccess = new List<GlobalRoleUserDao>();

                        foreach (var newusers in users1)
                        {
                            GlobalRoleUserDao globalRoleUserDao = new GlobalRoleUserDao();
                            globalRoleUserDao.Userid = newusers.Id;
                            globalRoleUserDao.GlobalRoleId = 10;
                            provideaccess.Add(globalRoleUserDao);

                        }
                        if (provideaccess.Count() > 0)
                        {
                            tx.PersistenceManager.AccessRepository.Save<GlobalRoleUserDao>(provideaccess);

                            try
                            {
                                System.Threading.Tasks.Task task3 = new System.Threading.Tasks.Task(() => SendMail(users1));
                                task3.Start();
                            }
                            catch (AggregateException ae)
                            {
                                // Assume we know what's going on with this particular exception. 
                                // Rethrow anything else. AggregateException.Handle provides 
                                // another way to express this. See later example. 
                                foreach (var e in ae.InnerExceptions)
                                {
                                    LogHandler.LogInfo("mail exception while user registration", LogHandler.LogType.General);
                                }
                            }

                            //---------------------> ADD USER REGISTRATION DATA TO USER DETAILS TABLE <-----------------
                            for (int k = 0; k < ids.Length; k++)
                            {
                                string entityName = "AttributeRecord12PendingUser_V" + MarcomManagerFactory.ActiveMetadataVersionNumber;
                                string userdetail = "AttributeRecord12_V" + MarcomManagerFactory.ActiveMetadataVersionNumber;
                                var userregisterdetails = (from item in tx.PersistenceManager.PlanningRepository.GetAll<DynamicAttributesDao>(entityName) where item.Id == ids[k] select item);
                                foreach (var objdyn in userregisterdetails)
                                {
                                    DynamicAttributesDao dynamicdao = new DynamicAttributesDao();
                                    dynamicdao.Id = users1[k].Id;
                                    dynamicdao.Attributes = objdyn.Attributes;
                                    tx.PersistenceManager.AccessRepository.SaveByentity(userdetail, dynamicdao);
                                }

                                var usermultiSelectValuedao = (from item in tx.PersistenceManager.PlanningRepository.Query<UserRegistrationMultiSelectDao>()
                                                               where item.UserID == ids[k]
                                                               select item).ToList();

                                if (usermultiSelectValuedao.Count > 0)
                                {
                                    IList<UserDetailsMultiSelectDao> listMultiselect = new List<UserDetailsMultiSelectDao>();
                                    foreach (var at in usermultiSelectValuedao)
                                    {
                                        Marcom.Dal.User.Model.UserDetailsMultiSelectDao mt = new Marcom.Dal.User.Model.UserDetailsMultiSelectDao();
                                        mt.Attributeid = at.Attributeid;
                                        mt.UserID = users1[k].Id;
                                        mt.Optionid = at.Optionid;
                                        listMultiselect.Add(mt);
                                    }
                                    tx.PersistenceManager.PlanningRepository.Save<Marcom.Dal.User.Model.UserDetailsMultiSelectDao>(listMultiselect);
                                }

                            }
                            //---------------------> ENDS HERE <-----------------

                        }

                        tx.Commit();

                        using (ITransaction txmail = proxy.MarcomManager.GetTransaction())
                        {
                            foreach (var user in users1)
                            {
                                UserTaskNotificationMailSettingsDao dao = new UserTaskNotificationMailSettingsDao();
                                dao.IsEmailEnable = true;
                                dao.IsNotificationEnable = true;
                                dao.LastUpdatedOn = DateTimeOffset.UtcNow;
                                dao.NoOfDays = 0;
                                dao.Userid = user.Id;
                                txmail.PersistenceManager.TaskRepository.Save<UserTaskNotificationMailSettingsDao>(dao);


                                string defaultNotificationSubscriptionTypes = "";
                                string defaultMailSubscriptionTypes = "";
                                IList<SubscriptionTypeDao> subscriptiontypes = new List<SubscriptionTypeDao>();
                                subscriptiontypes = (from defaulttypes in tx.PersistenceManager.CommonRepository.Query<SubscriptionTypeDao>() where defaulttypes.isAppDefault || defaulttypes.isMailDefault select defaulttypes).ToList<SubscriptionTypeDao>();
                                defaultNotificationSubscriptionTypes = string.Join(", ", (from defaultnotifications in subscriptiontypes where defaultnotifications.isAppDefault select defaultnotifications.Id).ToArray());
                                defaultMailSubscriptionTypes = string.Join(", ", (from defaultnotifications in subscriptiontypes where defaultnotifications.isMailDefault select defaultnotifications.Id).ToArray());
                                UserDefaultSubscriptionDao userdefsubscriptiondet = new UserDefaultSubscriptionDao();
                                userdefsubscriptiondet.UserID = user.Id;
                                userdefsubscriptiondet.SubscriptionTypeID = defaultNotificationSubscriptionTypes;
                                userdefsubscriptiondet.MailSubscriptionTypeID = defaultMailSubscriptionTypes;
                                txmail.PersistenceManager.CommonRepository.Save<UserDefaultSubscriptionDao>(userdefsubscriptiondet);
                                txmail.Commit();
                            }

                        }


                    }





                    return true;

                }
            }
            catch (Exception ex)
            {

            }

            return false;
        }
        public void SendMail(IList<UserDao> users)
        {

            foreach (var approval in users)
            {

                string ToMail = approval.Email;
                string Subject = "Your request is approved";
                SmtpClient objsmtp = new SmtpClient();
                MailMessage mail = new MailMessage();
                StringBuilder Body = new StringBuilder();
                // Body.Append("Your request is approved,please login");

                Body.Append(@"<table cellspacing='0' cellpadding='0' border='0' style='border-collapse: collapse;'");
                Body.Append("    width: 98%>");
                Body.Append("    <tbody>");
                Body.Append("        <tr>");
                Body.Append("            <td style='font-size: 12px; font-family: lucida grande,tahoma,verdana,arial,sans-serif'>");
                Body.Append("                <table cellspacing='0' cellpadding='0' style='border-collapse: collapse; width: 620px>'");
                Body.Append("                    <tbody>");
                Body.Append("                        <tr>");
                Body.Append("                            <td style='font-size: 16px; font-family: lucida grande,tahoma,verdana,arial,sans-serif;'");
                Body.Append("                                background: #A5A5A5; color: #ffffff; font-weight: bold; vertical-align: baseline;");
                Body.Append("                                letter-spacing: 0; text-align: left; padding: 5px 20px; border-top: 2px solid #E4E4E4; border-bottom:2px solid #E4E4E4;>");
                Body.Append("                                <span style='background: #A5A5A5; color: #fff; font-weight: bold; font-family: 'lucida grande',tahoma,verdana,arial,sans-serif;");
                Body.Append("                                    vertical-align: middle; font-size: 13px; letter-spacing: 0; text-align: left;");
                Body.Append("                                    vertical-align: baseline'>From Brandsystems</span>");
                Body.Append("                            </td>");
                Body.Append("                        </tr>");
                Body.Append("                    </tbody>");
                Body.Append("                </table>");
                Body.Append("                <table cellspacing='0' cellpadding='0' border='0' width='620px' style='border-collapse: collapse;");
                Body.Append("                    width: 620px'>");
                Body.Append("                    <tbody>");
                Body.Append("                        <tr>");
                Body.Append("                            <td style='padding: 0px; background-color: #f2f2f2; border-left: none; border-right: none;");
                Body.Append("                                border-top: none; border-bottom: none'>");
                Body.Append("                                <table cellspacing='0' cellpadding='0' width='620px' style='border-collapse: collapse'>");
                Body.Append("                                    <tbody>");
                Body.Append("                                        <tr>");
                Body.Append("                                            <td style='font-size: 11px; font-family: 'lucida grande',tahoma,verdana,arial,sans-serif;");
                Body.Append("                                                padding: 0px; width: 620px'");
                Body.Append("                                                <table cellspacing='0' cellpadding='0' border='0' style='border-collapse: collapse;");
                Body.Append("                                                    width: 100%'>");
                Body.Append("                                                    <tbody>");
                Body.Append("                                                        <tr>");
                Body.Append("                                                            <td style='padding: 20px; background-color: #fff; border-left: none; border-right: none;");
                Body.Append("                                                                border-top: none; border-bottom: none;'>");
                Body.Append("                                                                <table cellspacing='0' cellpadding='0' style='border-collapse: collapse'>");
                Body.Append("                                                                    <tbody>");
                Body.Append("                                                                        <tr>");
                Body.Append("                                                                            <td valign='top' style='font-size: 12px; font-family: lucida grande,tahoma,verdana,arial,sans-serif;");
                Body.Append("                                                                                width: 100%; text-align: left'>");
                Body.Append("                                                                                <table cellspacing='0' cellpadding='0' style='border-collapse: collapse; width: 100%'>");
                Body.Append("                                                                                    <tbody>");
                Body.Append("                                                                                        <tr>");
                Body.Append("                                                                                            <td style='font-size: 13px; font-family: lucida grande,tahoma,verdana,arial,sans-serif;");
                Body.Append("                                                                                                padding-top: 5px; padding-bottom: 5px';>");
                Body.Append("Your User Registration is approved.please login to the application." + " ");

                Body.Append("                                                                                            </td>");
                Body.Append("                                                                                        </tr>");
                Body.Append("                                                                                    </tbody>");
                Body.Append("                                                                                </table>");
                Body.Append("                                                                            </td>");
                Body.Append("                                                                        </tr>");
                Body.Append("                                                                    </tbody>");
                Body.Append("                                                                </table>");
                Body.Append("                                                            </td>");
                Body.Append("                                                        </tr>");
                Body.Append("                                                    </tbody>");
                Body.Append("                                                </table>");
                Body.Append("                                            </td>");
                Body.Append("                                        </tr>");
                Body.Append("                                        <tr>");
                Body.Append("                                            <td style='font-size: 11px; font-family: lucida grande,tahoma,verdana,arial,sans-serif;");
                Body.Append("                                                padding: 0px; width: 620px'>");
                Body.Append("                                                <table cellspacing='0' cellpadding='0' border='0' style='border-collapse: collapse;");
                Body.Append("                                                    width: 100%'>");
                Body.Append("                                                    <tbody>");
                Body.Append("                                                        <tr>");
                Body.Append("                                                            <td style='padding: 7px 20px; background-color: #f2f2f2; border-left: none; border-right: none;");
                Body.Append("                                                                border-top: 1px solid #ccc; border-bottom: 1px solid #ccc'>");
                Body.Append("                                                            </td>");
                Body.Append("                                                        </tr>");
                Body.Append("                                                    </tbody>");
                Body.Append("                                                </table>");
                Body.Append("                                            </td>");
                Body.Append("                                        </tr>");
                Body.Append("                                    </tbody>");
                Body.Append("                                </table>");
                Body.Append("                            </td>");
                Body.Append("                        </tr>");
                Body.Append("                    </tbody>");
                Body.Append("                </table>");
                Body.Append("                <table cellspacing='0' cellpadding='0' border='0' style='border-collapse: collapse;");
                Body.Append("                    width: 620px'>");
                Body.Append("                    <tbody>");
                Body.Append("                        <tr>");
                Body.Append("                            <td style='border-right: none; color: #999999; font-size: 11px; border-bottom: none;");
                Body.Append("                                font-family: lucida grande,tahoma,verdana,arial,sans-serif; border: none; border-top: none;");
                Body.Append("                                padding: 30px 20px; border-left: none'>");
                Body.Append("                                <span>");
                Body.Append("<br />");
                Body.Append("                                    This is a autogenerated mail from Brandsystems Planning tool (COMSYS). You can´t");
                Body.Append("                                    respond to this mail in any form. If you encounter any problem with Brandsystems");
                Body.Append("                                    Planning tool (COMSYS), please contact the COMSYS Helpdesk, at email:comsys.noreply@brandsystems.in<br />");
                Body.Append("<br />");
                Body.Append("</span>");
                Body.Append("                            </td>");
                Body.Append("                        </tr>");
                Body.Append("                    </tbody>");
                Body.Append("                </table>");
                Body.Append("                <span style='width: 620px';>");
                Body.Append("                    <img style='border: 0; width: 1px; min-height: 1px';><u></u></span>");
                Body.Append("            </td>");
                Body.Append("        </tr>");
                Body.Append("    </tbody>");
                Body.Append("</table>");
                //  objsmtp.Credentials = new System.Net.NetworkCredential("comsys.noreply@brandsystems.in", "comsys@123");

                // objsmtp.Host = "mail.brandsystems.in";
                mail.From = new System.Net.Mail.MailAddress(System.Configuration.ConfigurationManager.AppSettings["Email"]);
                mail.IsBodyHtml = true;
                mail.To.Add(ToMail);
                mail.Subject = Subject;
                mail.Body = Body.ToString();
                try
                {
                    objsmtp.Send(mail);
                }
                catch (Exception ex)
                {

                }

            }
        }


        /// <summary>
        /// Valids the user.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <returns>IUser</returns>
        public IUser valid_User(UserManagerProxy proxy, string userName, string password)
        {

            try
            {
                IList<UserDao> userList = new List<UserDao>();
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {

                    userList = tx.PersistenceManager.UserRepository.GetEquals<UserDao>(UserDao.PropertyNames.UserName, userName);
                    if (BCrypt.CheckBytePassword(password, userList.ElementAt(0).SaltPassword, userList.ElementAt(0).Password) == true)
                    {
                        IUser user = new BrandSystems.Marcom.Core.User.User();
                        user.Email = userList.ElementAt(0).Email;
                        user.FirstName = userList.ElementAt(0).FirstName;
                        user.Id = userList.ElementAt(0).Id;
                        user.Image = userList.ElementAt(0).Image;
                        user.Language = userList.ElementAt(0).Language;
                        user.LastName = userList.ElementAt(0).LastName;
                        user.Password = userList.ElementAt(0).Password;
                        user.SaltPassword = userList.ElementAt(0).SaltPassword;
                        user.StartPage = userList.ElementAt(0).StartPage;
                        user.TimeZone = userList.ElementAt(0).TimeZone;
                        user.UserName = userList.ElementAt(0).UserName;
                        user.DashboardTemplateID = userList.ElementAt(0).DashboardTemplateID;
                        user.Feedselection = userList.ElementAt(0).FeedSelection;

                        proxy.MarcomManager.User = user;
                        return user;
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return null;
        }




        /// <summary>
        /// User select name by ID.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>string</returns>
        public string User_SelectNameByID(UserManagerProxy proxy, int userId)
        {
            try
            {
                IUser user = User_SelectByID(proxy, userId);
                if (user.Id != 0)
                {
                    return user.UserName + "  " + user.LastName;
                }
            }
            catch (Exception ex)
            {

            }

            return null;
        }

        /// <summary>
        /// Validates the user.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="UserPwd">The user PWD.</param>
        /// <returns>IUser</returns>
        public IUser ValidateUser(int ID, bool IsAPIUser)
        {
            ClsDb objDb = new ClsDb();
            if (IsAPIUser == false)
            {
                return objDb.GetUserByID("SELECT ID,FirstName,LastName,UserName,Password,PasswordSalt,Email,Image,Language,TimeZone,StartPage,DashboardTemplateID,FeedSelection FROM UM_User  WHERE ID=" + ID + " and IsAPIUser=0", CommandType.Text);
            }
            else
            {
                return objDb.GetUserByID("SELECT ID,FirstName,LastName,UserName,Password,PasswordSalt,Email,Image,Language,TimeZone,StartPage,DashboardTemplateID,FeedSelection FROM UM_User  WHERE ID=" + ID + " and IsAPIUser=1", CommandType.Text);
            }
        }




        /// <summary>
        /// Returning user selection feeds for Entity.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="UseriD">The user ID</param>
        /// <returns>string</returns>
        public string UserEntityselections(UserManagerProxy proxy, int userId)
        {

            try
            {
                UserDao userval = new UserDao();
                string feedvalues = "";
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    userval = tx.PersistenceManager.UserRepository.Get<UserDao>(userId);
                    tx.Commit();
                    feedvalues = userval.FeedSelection;
                }
                return feedvalues;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public IList<IUser> GetMemberList(UserManagerProxy proxy, string querystring, int EntityID)
        {
            IList<IUser> users = new List<IUser>();
            IList<UserDao> dao = new List<UserDao>();
            using (ITransaction tx = proxy.MarcomManager.GetTransaction())
            {
                var currentUserRole = (from data in tx.PersistenceManager.UserRepository.Query<GlobalRoleUserDao>()
                                       where data.Userid == proxy.MarcomManager.User.Id
                                       orderby data.GlobalRoleId ascending
                                       select data.GlobalRoleId).FirstOrDefault();

                ////var globalRoledao = (from item in tx.PersistenceManager.UserRepository.Query<GlobalAclDao>()
                ////                     where item.EntityTypeid == EntityID
                ////                     select item.GlobalRoleid).ToList();
                //var globalRoleUserdao = (from item in tx.PersistenceManager.UserRepository.Query<GlobalRoleUserDao>()
                //                         where currentUserRole == item.GlobalRoleId
                //                         select item.Userid).ToList();
                //if (currentUserRole == 0)
                //{
                //    dao = (from item in tx.PersistenceManager.UserRepository.Query<UserDao>()
                //           where (item.FirstName + " " + item.LastName).StartsWith(querystring)
                //           select item).ToList();
                //}
                //else
                //{
                //    dao = (from item in tx.PersistenceManager.UserRepository.Query<UserDao>()
                //           where globalRoleUserdao.Contains(item.Id) && (item.FirstName + " " + item.LastName).StartsWith(querystring)
                //           select item).ToList();
                //}
                dao = (from item in tx.PersistenceManager.UserRepository.Query<UserDao>()
                       where (item.FirstName + " " + item.LastName).StartsWith(querystring) && !item.IsAPIUser
                       select item).ToList();
                foreach (var val in dao)
                {
                    IUser userVal = new BrandSystems.Marcom.Core.User.User();
                    userVal.Email = val.Email;
                    userVal.FirstName = val.FirstName;
                    userVal.Id = val.Id;
                    userVal.Image = val.Image;
                    userVal.LastName = val.LastName;
                    userVal.UserName = val.UserName;
                    //userVal.Designation = (val.Designation == null ? "-" : val.Title);
                    userVal.Designation = (val.Designation == null ? "-" : val.Designation);
                    userVal.Title = (val.Title == null ? "-" : val.Title);

                    var userresult = from itemRes in tx.PersistenceManager.PlanningRepository.Query<UserVisibleInfoDao>() select itemRes;
                    ArrayList arryAttributeids = new ArrayList();
                    foreach (var obj in userresult)
                    {
                        arryAttributeids.Add(obj.AttributeId);
                    }
                    var result = proxy.MarcomManager.MetadataManager.GetUserDetailsAttributes(12, val.Id);
                    if (result != null)
                    {
                        if (result.Count > 0)
                        {
                            var result1 = from item in result where arryAttributeids.Contains(item.AttributeID) select item;

                            if (result1 != null)
                            {
                                int i = 0;
                                int attrvalid = 0;
                                foreach (var obj in result1)
                                {
                                    if (obj.AttributeTypeID == 3)
                                    {
                                        if (obj.AttributeValue != null)
                                        {
                                            if (obj.AttributeValue.ToString() == "")
                                            {
                                                if (i == 0)
                                                {
                                                    userVal.QuickInfo1 = "-";
                                                    userVal.QuickInfo1AttributeCaption = (string)obj.AttributeCaption;
                                                }
                                                else
                                                {
                                                    userVal.QuickInfo2 = "-";
                                                    userVal.QuickInfo2AttributeCaption = (string)obj.AttributeCaption;
                                                }
                                            }
                                            else
                                            {
                                                attrvalid = (int)obj.AttributeValue;
                                                var singleCaption = (from item in tx.PersistenceManager.PlanningRepository.Query<OptionDao>() where item.Id == attrvalid select item.Caption).ToList();
                                                if (i == 0)
                                                {
                                                    userVal.QuickInfo1 = singleCaption[0].ToString();
                                                    userVal.QuickInfo1AttributeCaption = (string)obj.AttributeCaption;
                                                }
                                                else
                                                {
                                                    userVal.QuickInfo2 = singleCaption[0].ToString();
                                                    userVal.QuickInfo2AttributeCaption = (string)obj.AttributeCaption;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (i == 0)
                                            {
                                                userVal.QuickInfo1 = "-";
                                                userVal.QuickInfo1AttributeCaption = (string)obj.AttributeCaption;
                                            }
                                            else
                                            {
                                                userVal.QuickInfo2 = "-";
                                                userVal.QuickInfo2AttributeCaption = (string)obj.AttributeCaption;
                                            }
                                        }
                                    }
                                    else if (obj.AttributeTypeID == 4)
                                    {
                                        var multiselectResult = tx.PersistenceManager.PlanningRepository.Query<Marcom.Dal.User.Model.UserDetailsMultiSelectDao>().Where(a => a.UserID == val.Id && a.Attributeid == Convert.ToInt32(obj.AttributeID)).Select(a => a.Optionid).ToList();

                                        var singleCaption = (from item in tx.PersistenceManager.PlanningRepository.Query<OptionDao>() where multiselectResult.Contains(item.Id) select item.Caption).ToList();
                                        if (singleCaption.Count > 0)
                                        {
                                            if (i == 0)
                                            {
                                                userVal.QuickInfo1 = String.Join(",", singleCaption).ToString();
                                                userVal.QuickInfo1AttributeCaption = (string)obj.AttributeCaption;
                                            }
                                            else
                                            {
                                                userVal.QuickInfo2 = String.Join(",", singleCaption).ToString();
                                                userVal.QuickInfo2AttributeCaption = (string)obj.AttributeCaption;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (i == 0)
                                        {
                                            userVal.QuickInfo1 = (string)obj.AttributeValue;
                                            userVal.QuickInfo1AttributeCaption = (string)obj.AttributeCaption;
                                        }
                                        else
                                        {
                                            userVal.QuickInfo2 = (string)obj.AttributeValue;
                                            userVal.QuickInfo2AttributeCaption = (string)obj.AttributeCaption;
                                        }
                                    }
                                    i++;
                                }

                            }
                        }

                    }

                    users.Add(userVal);
                }
            }
            return users;

        }

        /// <summary>
        /// Updating user selection feeds for Entity.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="UseriD">The user ID</param>
        /// <param name="Feedselection">User selection Entitytpes as comma separated</param>
        /// <returns>bool</returns>
        public bool UserFeedselectionUpdate(UserManagerProxy proxy, int userId, string feedSelection)
        {

            try
            {
                UserDao userFeeddao = new UserDao();
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {

                    userFeeddao.Id = userId;
                    userFeeddao.FeedSelection = feedSelection;
                    string query = @"Update UM_User set FeedSelection ='" + feedSelection + "' where ID=" + userId;
                    tx.PersistenceManager.UserRepository.CreateQuery(query);
                    tx.Commit();
                    return true;
                }
            }
            catch (Exception ex)
            {

            }
            return false;
        }
        public IList<IUser> GetUserByEntityID(UserManagerProxy proxy, int EntityID)
        {
            using (ITransaction tx = proxy.MarcomManager.GetTransaction())
            {
                IList<IUser> _iuser = new List<IUser>();
                IList<UserDao> dao = new List<UserDao>();
                IUser user = new BrandSystems.Marcom.Core.User.User();
                BrandSystems.Marcom.Dal.Access.Model.EntityRoleUserDao entityroleuser = new Dal.Access.Model.EntityRoleUserDao();
                IList<BrandSystems.Marcom.Dal.Access.Model.EntityRoleUserDao> roleusers = new List<BrandSystems.Marcom.Dal.Access.Model.EntityRoleUserDao>();
                roleusers = tx.PersistenceManager.MetadataRepository.GetAll<BrandSystems.Marcom.Dal.Access.Model.EntityRoleUserDao>();
                var entitymembers = from members in roleusers where members.Entityid == EntityID select members.Userid;
                IList<BrandSystems.Marcom.Dal.User.Model.UserDao> listmembers = new List<BrandSystems.Marcom.Dal.User.Model.UserDao>();
                for (int i = 0; i < entitymembers.Count(); i++)
                {
                    BrandSystems.Marcom.Dal.User.Model.UserDao users = new Dal.User.Model.UserDao();
                    users = tx.PersistenceManager.MetadataRepository.Get<UserDao>(entitymembers.ElementAt(i));

                    user.Email = users.Email;
                    user.FirstName = users.FirstName;
                    user.Id = users.Id;
                    user.Image = users.Image;
                    user.Language = users.Language;
                    user.LastName = users.LastName;
                    user.Password = users.Password;
                    user.SaltPassword = users.SaltPassword;
                    user.StartPage = users.StartPage;
                    user.TimeZone = users.TimeZone;
                    user.UserName = users.UserName;
                    user.DashboardTemplateID = users.DashboardTemplateID;
                    user.IsAPIUser = users.IsAPIUser;
                    _iuser.Add(user);
                }

                return _iuser;
            }
        }


        public IList<IEntityUsers> GetMembersByEntityID(UserManagerProxy proxy, int EntityID)
        {
            using (ITransaction tx = proxy.MarcomManager.GetTransaction())
            {
                IList<IEntityUsers> _iientityusers = new List<IEntityUsers>();
                IList<IUser> _iuser = new List<IUser>();
                IList<UserDao> dao = new List<UserDao>();
                IUser user = new BrandSystems.Marcom.Core.User.User();
                BrandSystems.Marcom.Dal.Access.Model.EntityRoleUserDao entityroleuser = new Dal.Access.Model.EntityRoleUserDao();
                IList<BrandSystems.Marcom.Dal.Access.Model.EntityRoleUserDao> roleusers = new List<BrandSystems.Marcom.Dal.Access.Model.EntityRoleUserDao>();
                roleusers = tx.PersistenceManager.MetadataRepository.GetAll<BrandSystems.Marcom.Dal.Access.Model.EntityRoleUserDao>();
                var entitymembers = from members in roleusers where members.Entityid == EntityID select members;


                IList<BrandSystems.Marcom.Dal.User.Model.UserDao> listmembers = new List<BrandSystems.Marcom.Dal.User.Model.UserDao>();
                for (int i = 0; i < entitymembers.Count(); i++)
                {
                    IEntityUsers entityuser = new BrandSystems.Marcom.Core.User.EntityUsers();

                    BrandSystems.Marcom.Dal.User.Model.UserDao users = new Dal.User.Model.UserDao();
                    users = tx.PersistenceManager.MetadataRepository.Get<UserDao>(entitymembers.ElementAt(i).Userid);

                    if (entitymembers.ElementAt(i).Roleid == 1)
                    {
                        entityuser.IsOwner = true;
                    }
                    else
                        entityuser.IsOwner = false;
                    if (users != null)
                    {
                        entityuser.OwnerName = users.FirstName + " " + users.LastName;
                        entityuser.Email = users.Email;
                        _iientityusers.Add(entityuser);
                    }
                    //user.Email = users.Email;
                    //user.FirstName = users.FirstName;
                    //user.Id = users.Id;
                    //user.Image = users.Image;
                    //user.Language = users.Language;
                    //user.LastName = users.LastName;
                    //user.Password = users.Password;
                    //user.SaltPassword = users.SaltPassword;
                    //user.StartPage = users.StartPage;
                    //user.TimeZone = users.TimeZone;
                    //user.UserName = users.UserName;

                    //_iuser.Add(user);
                }

                return _iientityusers;
            }
        }


        /// <summary>
        /// Update users.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="usermgr">The usermgr.</param>
        /// <returns>bool</returns>
        public bool Userinfo_UpdateByColumn(UserManagerProxy proxy, string ColumnName, string ColumnValue)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    UserDao user = new UserDao();

                    user = tx.PersistenceManager.UserRepository.Get<UserDao>(proxy.MarcomManager.User.Id);

                    switch (ColumnName)
                    {
                        case "FirstName":
                            user.FirstName = ColumnValue.ToString();
                            proxy.MarcomManager.User.FirstName = ColumnValue.ToString();
                            break;

                        case "LastName":
                            user.LastName = ColumnValue.ToString();
                            proxy.MarcomManager.User.LastName = ColumnValue.ToString();
                            break;

                        case "Gender":
                            break;

                        case "Phone":
                            user.Phone = ColumnValue;
                            proxy.MarcomManager.User.Phone = ColumnValue;
                            break;

                        case "Address":
                            user.Address = ColumnValue;
                            proxy.MarcomManager.User.Address = ColumnValue.ToString();
                            break;

                        case "City":
                            user.City = ColumnValue;
                            proxy.MarcomManager.User.City = ColumnValue.ToString();
                            break;

                        case "ZipCode":
                            user.ZipCode = Convert.ToInt32(ColumnValue);
                            proxy.MarcomManager.User.ZipCode = Convert.ToInt32(ColumnValue);
                            break;

                        case "Website":
                            user.Website = ColumnValue;
                            proxy.MarcomManager.User.Website = ColumnValue;
                            break;

                        case "StartPage":
                            user.StartPage = Convert.ToInt32(ColumnValue);
                            proxy.MarcomManager.User.StartPage = Convert.ToInt32(ColumnValue);
                            break;

                        case "FeedSelection":
                            user.FeedSelection = ColumnValue;
                            proxy.MarcomManager.User.Feedselection = ColumnValue;
                            break;

                        case "Designation":
                            user.Designation = ColumnValue;
                            proxy.MarcomManager.User.Designation = ColumnValue;
                            break;

                        case "Title":
                            user.Title = ColumnValue;
                            proxy.MarcomManager.User.Title = ColumnValue;
                            break;
                        default:
                            break;

                    }


                    tx.PersistenceManager.UserRepository.Save<UserDao>(user);
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
        /// save users image.
        /// </summary>
        /// <param name="source img">The source img.</param>
        /// <param name="destination img">The destination img.</param>
        /// <param name="imgwidth img">The imgwidth img.</param>
        /// <param name="imgheight img">The imgheight img.</param>
        /// <param name="imgX img">The imgX img.</param>
        /// <param name="imgY img">The imgY img.</param>
        /// <returns>bool</returns>
        public bool SaveUserImage(UserManagerProxy proxy, string sourcepath, int imgwidth, int imgheight, int imgX, int imgY)
        {
            try
            {

                string orgsourcepath = HttpContext.Current.Server.MapPath(sourcepath);

                orgsourcepath = orgsourcepath.Replace("user\\", "");
                using (SD.Image OriginalImage = SD.Image.FromFile(orgsourcepath))
                {
                    using (SD.Bitmap bmp = new SD.Bitmap(imgwidth, imgheight))
                    {
                        bmp.SetResolution(OriginalImage.HorizontalResolution, OriginalImage.VerticalResolution);
                        using (SD.Graphics Graphic = SD.Graphics.FromImage(bmp))
                        {
                            Graphic.SmoothingMode = SmoothingMode.AntiAlias;
                            Graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            Graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
                            Graphic.DrawImage(OriginalImage, new SD.Rectangle(0, 0, imgwidth, imgheight), imgX, imgY, imgwidth, imgheight, SD.GraphicsUnit.Pixel);

                            var cloned = new SD.Bitmap(bmp).Clone(new SD.Rectangle(new SD.Point(0, 0), bmp.Size), bmp.PixelFormat);
                            var nbmp = new SD.Bitmap(cloned, new SD.Size(120, 140));

                            MemoryStream ms = new MemoryStream();
                            nbmp.Save(ms, OriginalImage.RawFormat);



                            byte[] CropImage = ms.GetBuffer();
                            using (MemoryStream ms1 = new MemoryStream(CropImage, 0, CropImage.Length))
                            {
                                ms.Write(CropImage, 0, CropImage.Length);
                                using (SD.Image CroppedImage = SD.Image.FromStream(ms, true))
                                {
                                    string destinationpath = HttpContext.Current.Server.MapPath("UserImages/" + proxy.MarcomManager.User.Id + ".jpg");
                                    destinationpath = destinationpath.Replace("user\\", "");
                                    if (System.IO.File.Exists(destinationpath))
                                    {
                                        System.IO.File.Delete(destinationpath);
                                    }
                                    CroppedImage.Save(destinationpath, CroppedImage.RawFormat);
                                }
                            }
                        }
                    }
                }


            }
            catch (Exception ex)
            {

            }
            return false;

        }

        public bool insertloginAsynchronous(UserManagerProxy proxy, int UserID, string IPAddress, string Browser, string Version, int MajorVersion, string MinorVersion, string OS, bool IIsSSO)
        {
            double IPNumber = 0;
            ////ipadd = getipnumber(IPAddress);
            //ipadd = Dot2LongIP(IPAddress);
            //int ipadd1 = (int) Math.Round(ipadd,0);
            //string ipnumb = ipadd.ToString();

            try
            {
                LogHandler.LogInfo("************************user details are going to save in userlogindao " + DateTime.Now + " ************************", LogHandler.LogType.General);
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    UserLoginDao UserLogin = new UserLoginDao();
                    UserLogin.UserID = UserID;
                    UserLogin.LoginTime = DateTime.Now;
                    UserLogin.IPAddress = IPAddress;
                    IPNumber = GET_IPnumber(IPAddress);
                    UserLogin.IPNumber = IPNumber.ToString();
                    UserLogin.Browser = Browser;
                    UserLogin.Version = Version;
                    UserLogin.MajorVersion = MajorVersion;
                    UserLogin.MinorVersion = MinorVersion;
                    UserLogin.OS = OS;
                    UserLogin.IsSSO = IIsSSO;
                    //var list = new List<>;
                    // list = CountryName_Select(proxy, IPNumber.ToString());
                    IEnumerable<Hashtable> listresult;
                    listresult = CountryName_Select(proxy, IPNumber.ToString()).Cast<Hashtable>(); ;
                    //string ;
                    bool blnCountryName = false;
                    if (listresult != null)
                    {
                        foreach (var val in listresult)
                        {
                            string cname = val["CountryName"].ToString();
                            if (cname.Length > 0 && cname != null)
                            {
                                UserLogin.CountryName = cname;
                                blnCountryName = true;
                            }
                            else
                            {
                                UserLogin.CountryName = "";
                                blnCountryName = true;
                            }
                        }
                    }
                    if (blnCountryName == false)
                    {
                        UserLogin.CountryName = "";
                    }


                    tx.PersistenceManager.UserRepository.Save<UserLoginDao>(UserLogin);
                    tx.Commit();


                    return true;
                }

            }
            catch (Exception ex)
            {
                LogHandler.LogInfo("************************ user details save in userlogindao failed due to " + ex.Message + "" + DateTime.Now + " ************************", LogHandler.LogType.General);
                return false;
            }
        }

        public bool insetlogin(UserManagerProxy proxy, int UserID, string IPAddress, string Browser, string Version, int MajorVersion, string MinorVersion, string OS, bool IIsSSO)
        {
            try
            {
                LogHandler.LogInfo("************************System thread going to  track user details using  insertloginAsynchronous method" + DateTime.Now + " ************************", LogHandler.LogType.General);
                System.Threading.Tasks.Task insertloginasyn = new System.Threading.Tasks.Task(() => insertloginAsynchronous(proxy, UserID, IPAddress, Browser, Version, MajorVersion, MinorVersion, OS, IIsSSO));
                insertloginasyn.Start();
                return true;
            }
            catch (Exception ex)
            {
                LogHandler.LogInfo("************************System thread failed for insertloginAsynchronous method due to " + ex.Message + "" + DateTime.Now + " ************************", LogHandler.LogType.General);
                return false;
            }


        }

        public double GET_IPnumber(string DottedIP)
        {
            int i;
            string[] arrDec;
            double num = 0;
            if (DottedIP == "")
            {
                return 0;
            }
            else if (DottedIP.IndexOf(".") == -1)
            {
                return 0;
            }
            else
            {
                arrDec = DottedIP.Split('.');
                for (i = arrDec.Length - 1; i >= 0; i--)
                {
                    num += ((int.Parse(arrDec[i]) % 256) * Math.Pow(256, (3 - i)));
                }
                return num;
            }
        }

        public IList CountryName_Select(UserManagerProxy proxy, string IPNumber)
        {

            try
            {
                IList listresult;
                StringBuilder strqry = new StringBuilder();

                strqry.Append("SELECT CountryName FROM UT_ip_tocountry WHERE (([BeginingIP] <= ? ) AND ([EndingIP] >=  ? ))");
                //strqry.Append("select DISTINCT  * from");
                //strqry.Append("(");
                //strqry.Append("SELECT TOP(" + Topx + ") pea.id AS 'ActivityID',pea.name AS 'ActivityName',SUM(pefav.ApprovedAllocatedAmount) AS 'ApprovedAllocatedAmount',SUM(pefav.Spent) AS 'Spent',pea.TypeId,");
                //strqry.Append("(SELECT met.ShortDescription FROM MM_EntityType met WHERE met.ID = pea.TypeId) AS ShortDescription,");
                //strqry.Append("(SELECT met.ColorCode FROM MM_EntityType met WHERE met.ID = pea.TypeId) AS ColorCode, ");
                //strqry.Append("mw.Name AS Status ");
                //strqry.Append("FROM PM_Entity pea  INNER JOIN AM_Entity_Role_User per  ON  per.EntityID = pea.ID INNER JOIN PM_Financial pefav ON  pea.ID = pefav.EntityID  INNER JOIN MM_WorkFlow_Steps mw ON mw.id = pea.ActiveEntityStateID  INNER JOIN AM_Role mr ");
                //strqry.Append("ON  mr.ID = per.RoleID 	WHERE per.UserID = " + UserID + " AND  pea.Level = 1 AND pea.[Active] = 1 	GROUP BY pea.id,pea.name,mr.Caption,pea.TypeId,mw.Name ");
                //strqry.Append("ORDER BY  SUM(pefav.ApprovedAllocatedAmount) DESC ");
                //strqry.Append(") a ");
                //strqry.Append("where (a.Spent <> 0 or a.ApprovedAllocatedAmount <> 0)");
                using (ITransaction tx1 = proxy.MarcomManager.GetTransaction())
                {
                    listresult = tx1.PersistenceManager.MetadataRepository.ExecuteQuerywithMinParam(strqry.ToString(), IPNumber, IPNumber);
                    tx1.Commit();
                }


                return listresult;

            }
            catch (Exception ex)
            {
                LogHandler.LogInfo("************************failed to get countries due to " + ex.Message + "" + DateTime.Now + " ************************", LogHandler.LogType.General);
                return null;
            }

        }

        /// <summary>
        /// Getting Objective Users.
        /// </summary>
        /// <param name="proxy">The Proxy.</param>
        /// <param name="typeId">The EntityTypeId.</param>
        /// <returns>IList of Users</returns>
        public IList<IUser> GetAllObjectiveMembers(UserManagerProxy proxy, int entityTypeId)
        {
            try
            {
                IList<IUser> ientityMembers = new List<IUser>();
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    //var membersList = tx.PersistenceManager.PlanningRepository.Query<EntityDao>().Join(tx.PersistenceManager.PlanningRepository.Query<EntityRoleUserDao>(),
                    //    ent => ent.Id, entr => entr.Entityid, (ent, entr) => new { ent, entr }).Join(tx.PersistenceManager.PlanningRepository.Query<UserDao>(), entru => entru.entr.Userid,
                    //    usd => usd.Id, (entru, usd) => new { entru, usd }).Where(a => a.entru.ent.Typeid == Convert.ToInt32(EntityTypeIDs.Objective));           

                    string UserQuery = "SELECT DISTINCT uu.ID, uu.FirstName, uu.LastName FROM PM_Entity pe INNER JOIN AM_Entity_Role_User aeru ON aeru.EntityID = pe.ID INNER JOIN UM_User uu ON uu.ID = aeru.UserID WHERE pe.TypeID = ?";
                    var membersList = tx.PersistenceManager.PlanningRepository.ExecuteQuerywithMinParam(UserQuery, entityTypeId).Cast<Hashtable>().ToList();

                    foreach (var obj in membersList)
                    {
                        IUser userObj = new BrandSystems.Marcom.Core.User.User();
                        userObj.Id = (int)obj["ID"];
                        userObj.FirstName = (string)obj["FirstName"];
                        userObj.LastName = (string)obj["LastName"];
                        ientityMembers.Add(userObj);
                    }
                }
                return ientityMembers;
            }
            catch
            {

            }
            return null;
        }

        public bool UpdateNewLanguageType(UserManagerProxy proxy, int UserID, int langtypeid)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    string updatelang = "UPDATE UM_User SET LanguageSettings = ? WHERE ID = ?";
                    tx.PersistenceManager.CommonRepository.ExecuteQuerywithMinParam(updatelang, langtypeid, UserID);
                    tx.Commit();
                }
                return true;
            }
            catch
            {

                return false;
            }

        }



        public int IsSSOUser(UserManagerProxy proxy)
        {
            try
            {
                int issso = 0;
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    int userid = proxy.MarcomManager.User.Id;
                    var GlobalRoleID = "select * from AM_GlobalRole_User where UserId = ?";
                    var result = tx.PersistenceManager.CommonRepository.ExecuteQuerywithMinParam(GlobalRoleID, userid).Cast<Hashtable>().ToList();

                    foreach (var obj in result)
                    {
                        if (issso < 1)
                        {
                            int roleid = Convert.ToInt32(obj["GlobalRoleId"]);
                            var count = "select count(*) as count from AM_GlobalAcl aga inner join AM_GlobalRole aga2 on aga2.ID = aga.GlobalRoleID where aga.GlobalRoleID= ? and aga.FeatureID=29";
                            var isssocount = tx.PersistenceManager.CommonRepository.ExecuteQuerywithMinParam(count, roleid);
                            issso = (int)((System.Collections.Hashtable)(isssocount[0]))["count"];
                        }
                    }
                }
                return issso;

            }
            catch
            {
                return 0;
            }
        }



        public IList GetAPIusersDetails(UserManagerProxy proxy)
        {
            try
            {
                IList APIusersDetails = null;

                StringBuilder strqry = new StringBuilder();
                strqry.Append("select UMU.FirstName, UMU.LastName,UMA.APIGuid , UMA.UserID from UM_User UMU inner join UM_UserAPIInterface UMA on UMA.UserID = umu.ID");

                using (ITransaction tx1 = proxy.MarcomManager.GetTransaction())
                {
                    APIusersDetails = tx1.PersistenceManager.MetadataRepository.ExecuteQuery(strqry.ToString());
                    tx1.Commit();
                }

                return APIusersDetails;
            }
            catch (Exception ex)
            {

            }

            return null;
        }



        public bool GenerateGuidforSelectedAPI(UserManagerProxy proxy, int[] userIds)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    foreach (int userid in userIds)
                    {
                        UserAPIInterfaceDao objapidao = new UserAPIInterfaceDao();
                        objapidao.UserID = userid;
                        objapidao.APIGuid = Guid.NewGuid().ToString();
                        tx.PersistenceManager.UserRepository.Save<UserAPIInterfaceDao>(objapidao);
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

        public int GetApiUserIDByAuthToken(ITransaction tx, string ApiGuid)
        {
            try
            {
                //using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                //{
                var CurrentObj = tx.PersistenceManager.UserRepository.Get<UserAPIInterfaceDao>(UserAPIInterfaceDao.PropertyNames.APIGuid, ApiGuid);
                return CurrentObj.UserID;

                //}
            }
            catch
            {

            }
            return 0;
        }

    }

}

