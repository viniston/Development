using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrandSystems.Marcom.Core.Interface;
using NHibernate.UserTypes;
using BrandSystems.Marcom.Core.Interface.Managers;
using BrandSystems.Marcom.Dal.Report.Model;
using BrandSystems.Marcom.Core.Managers.Proxy;
using BrandSystems.Marcom.Dal.Report;
using BrandSystems.Marcom.Core.Report.Interface;
using Newtonsoft.Json.Linq;
using BrandSystems.Marcom.Core.Access;
using BrandSystems.Marcom.Dal.Base;
using BrandSystems.Marcom.Dal.Access.Model;
using System.Data;
using System.Data.SqlClient;
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
using System.Net;
using DevExpress.ReportServer.ServiceModel.Client;
using DevExpress.ReportServer.ServiceModel.DataContracts;
using System.Configuration;
using BrandSystems.Marcom.Core.Report.BrandSystems.Marcom.Core.Report;
using System.ServiceModel;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;
using BrandSystems.Marcom.Core.Metadata.Interface;
using BrandSystems.Marcom.Dal.Planning.Model;
using BrandSystems.Marcom.Metadata.Interface;
using BrandSystems.Marcom.Core.Planning.Interface;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using BrandSystems.Marcom.Metadata;
using Newtonsoft.Json;
using System.Drawing;
using OfficeOpenXml.Drawing;
using BrandSystems.Marcom.Utility;
using OfficeOpenXml.Drawing.Chart;
using System.Drawing.Imaging;
using BrandSystems.Marcom.Core.Task.Interface;
using BrandSystems.Marcom.Dal.Task.Model;
using BrandSystems.Marcom.Core.Task;
using BrandSystems.Marcom.Core.Report;

namespace BrandSystems.Marcom.Core.Managers
{

    internal partial class ReportManager : IManager
    {
        /// <summary>
        /// The instance
        /// </summary>
        private static ReportManager instance = new ReportManager();

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
        internal static ReportManager Instance
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
        //public IUser initializeIUser(string strbody)
        //{
        //    JObject jobj = JObject.Parse(strbody.ToUpper());
        //    IUser user = new BrandSystems.Marcom.Core.User.User();
        //    user.Email = jobj["EMAIL"] == null ? "" : (string)jobj["EMAIL"];
        //    user.FirstName = jobj["FIRSTNAME"] == null ? "" : (string)jobj["FIRSTNAME"];
        //    user.Image = jobj["IMAGE"] == null ? "" : (string)jobj["IMAGE"];
        //    user.Language = jobj["LANGUAGE"] == null ? "" : (string)jobj["LANGUAGE"];
        //    user.LastName = jobj["LASTNAME"] == null ? "" : (string)jobj["LASTNAME"];
        //    user.Password = jobj["PASSWORD"] == null ? null : (byte[])jobj["PASSWORD"];
        //    user.SaltPassword = jobj["SALTPASSWORD"] == null ? "" : (string)jobj["SALTPASSWORD"];
        //    user.StartPage = jobj["STARTPAGE"] == null ? 0 : (int)jobj["STARTPAGE"];
        //    user.TimeZone = jobj["TIMEZONE"] == null ? "" : (string)jobj["TIMEZONE"];
        //    user.UserName = jobj["USERNAME"] == null ? "" : (string)jobj["USERNAME"];
        //    user.UserName = jobj["DashboardTemplateID"] == null ? "" : (string)jobj["DashboardTemplateID"];

        //    return user;
        //}
        string strcon = ConfigurationSettings.AppSettings["conn"].ToString();
        SqlConnection sqlcon;// = new SqlConnection();
        string reportstrcon = ConfigurationSettings.AppSettings["ReportServerconn"].ToString();
        SqlConnection reportsqlcon;// = new SqlConnection();

        //gantt Report Generation Global variables
        private int RowNo = 7;
        private int ColumnNo = 1;
        private int LastColumnNo = 0;
        private int tempcolmno = 0;
        private System.DateTime CalenderStartDate = new System.DateTime(System.DateTime.Now.Year - 1, 1, 1);
        private System.DateTime CalenderEndDate = new System.DateTime(System.DateTime.Now.Year + 1, 12, 31);
        Dictionary<int, string> taskAssigneeImages = new Dictionary<int, string>();
        List<string> taskHeaderLst = new List<string>();
        private int assigneeWidth = 16;
        private int assigneeHeight = 17;
        ExcelComment taskAssigneeComment = default(ExcelComment);

        private bool IsObjectiveOrCostCenterPresent = false;
        private int FilterID = 0;
        private string ListOfEntityID = "";
        private int TypeID = 0;
        private string GlobalAccess;
        string MilestoeList = "";

        private bool DBconnection()
        {
            try
            {
                sqlcon = new SqlConnection(strcon);
                if (sqlcon.State == ConnectionState.Closed)
                {
                    sqlcon.Open();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return false;
        }

        private bool ReportServerDBconnection()
        {
            try
            {
                reportsqlcon = new SqlConnection(reportstrcon);
                if (reportsqlcon.State == ConnectionState.Closed)
                {
                    reportsqlcon.Open();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return false;

        }
        public bool ReportCredential_InsertUpdate(ReportManagerProxy proxy, string ReportUrl, string AdminUsername, string AdminPassword, string ViewerUsername, string ViewerPassword, int Category, int DataViewID, int id)
        {
            try
            {

                //if (proxy.MarcomManager.AccessManager.CheckAccess(Modules.Admin, 4, FeatureID.Report, OperationId.Self) == true)
                //{
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    ReportCredentialDao RDao = new ReportCredentialDao();

                    if (id != 0)
                    {
                        RDao.Id = id;
                    }
                    RDao.ReportUrl = ReportUrl;
                    RDao.AdminUsername = AdminUsername;
                    RDao.AdminPassword = AdminPassword;
                    RDao.ViewerUsername = ViewerUsername;
                    RDao.ViewerPassword = ViewerPassword;
                    RDao.Category = Category;
                    RDao.DataViewID = DataViewID;
                    tx.PersistenceManager.ReportRepository.Save<ReportCredentialDao>(RDao);
                    tx.Commit();
                    return true;

                }
                //}
            }
            catch (Exception ex)
            {

            }

            return false;
        }

        public int ReportCredential_ValidateSave(ReportManagerProxy proxy, string ReportUrl, string AdminUsername, string AdminPassword, string ViewerUsername, string ViewerPassword, int Category, int DataViewID, int id)
        {
            try
            {
                CookieContainer cookieContainer = new CookieContainer();
                CookieContainer cookieContainer1 = new CookieContainer();
                IList<IDataView> Dview1 = new List<IDataView>();

                using (var callContext = new ServiceCallContext<IAuthenticationService>(cookieContainer, new EndpointAddress(ReportUrl + "AuthenticationService.svc")))
                {

                    //RSConfigInfo.ServerBasedAddress = ReportUrl;
                    //RSConfigInfo.ViewerUsername = AdminUsername;
                    //RSConfigInfo.ViewerPassword = AdminPassword;

                    if (!callContext.Channel.Login(AdminUsername, AdminPassword))
                    {
                        return 2;
                    }
                }

                using (var callContext1 = new ServiceCallContext<IAuthenticationService>(cookieContainer1, new EndpointAddress(ReportUrl + "AuthenticationService.svc")))
                {


                    //RSConfigInfo.ViewerUsername = ViewerUsername;
                    //RSConfigInfo.ViewerPassword = ViewerPassword;

                    if (!callContext1.Channel.Login(ViewerUsername, ViewerPassword))
                    {
                        return 3;
                    }
                    else
                    {
                        int Category1 = Category;
                        IList Result = GetCategories(cookieContainer1, ReportUrl).Select(x => x.Id == Category).ToList();
                        Boolean result1 = false;
                        for (var i = 0; i < Result.Count; i++)
                        {
                            Boolean result2 = Convert.ToBoolean(Result[i].ToString());
                            if (result2 == true)
                            {
                                result1 = true;

                            }

                        }

                        if (result1 == false)
                        {
                            return 4;
                        }

                    }
                }

                Dview1 = Dataview_select(DataViewID, AdminUsername);
                if (Dview1.Count > 0)
                {
                }
                else
                {
                    return 5;
                }

                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    ReportCredentialDao RDao = new ReportCredentialDao();

                    if (id != 0)
                    {
                        RDao.Id = id;
                    }
                    RDao.ReportUrl = ReportUrl;
                    RDao.AdminUsername = AdminUsername;
                    RDao.AdminPassword = AdminPassword;
                    RDao.ViewerUsername = ViewerUsername;
                    RDao.ViewerPassword = ViewerPassword;
                    RDao.Category = Category;
                    RDao.DataViewID = DataViewID;
                    tx.PersistenceManager.ReportRepository.Save<ReportCredentialDao>(RDao);
                    tx.Commit();
                    return 1;

                }
                //}
            }
            catch (Exception ex)
            {
                return 0;
            }

            return 1;
        }
        public IList ReportCredential_Select(ReportManagerProxy proxy, int ID)
        {

            try
            {
                IList listresult;
                StringBuilder strqry = new StringBuilder();
                IList<MultiProperty> paramList = new List<MultiProperty>();

                if (ID > 0)
                {
                    paramList.Add(new MultiProperty { propertyName = "ID", propertyValue = ID });
                    strqry.Append("SELECT ID,ReportUrl,AdminUsername,AdminPassword,ViewerUsername,ViewerPassword,Category,DataViewID FROM RM_ReportCredential WHERE id= :ID ");
                }
                else
                {
                    strqry.Append("SELECT TOP 1 ID,ReportUrl,AdminUsername,AdminPassword,ViewerUsername,ViewerPassword,Category,DataViewID FROM RM_ReportCredential ORDER BY  id  DESC");
                }

                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    if (paramList.Count > 0)
                    {
                        listresult = tx.PersistenceManager.ReportRepository.ExecuteQuerywithParam(strqry.ToString(), paramList);
                    }
                    else
                    {
                        listresult = tx.PersistenceManager.ReportRepository.ExecuteQuery(strqry.ToString());
                    }

                    tx.Commit();
                }

                return listresult;

            }
            catch (Exception)
            {

                return null;
            }

        }

        public IList Report_Select(ReportManagerProxy proxy, int OID)
        {

            try
            {
                IList listresult;
                StringBuilder strqry = new StringBuilder();
                IList<MultiProperty> paramList = new List<MultiProperty>();
                if (OID > 0)
                {
                    paramList.Add(new MultiProperty { propertyName = "OID", propertyValue = OID });
                    strqry.Append("SELECT ID,OID,Name,Caption,Description,Preview,Show,0,EntityLevel,SubLevel FROM  [dbo].[RM_Report] WHERE OID = :OID ");
                }
                else
                {
                    strqry.Append("SELECT ID,OID,Name,Caption,Description,Preview,Show,0,EntityLevel,SubLevel  FROM   [dbo].[RM_Report] ORDER BY OID");
                }

                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    if (paramList.Count > 0)
                    {
                        listresult = tx.PersistenceManager.ReportRepository.ExecuteQuerywithParam(strqry.ToString(), paramList);
                    }
                    else
                    {
                        listresult = tx.PersistenceManager.ReportRepository.ExecuteQuery(strqry.ToString());
                    }

                    tx.Commit();
                }


                return listresult;

            }
            catch (Exception)
            {

                return null;
            }

        }

        public IList CustomViews_Select(ReportManagerProxy proxy, int ID)
        {

            try
            {
                IList listresult;
                StringBuilder strqry = new StringBuilder();
                IList<MultiProperty> paramList = new List<MultiProperty>();
                if (ID > 0)
                {
                    paramList.Add(new MultiProperty { propertyName = "ID", propertyValue = ID });
                    strqry.Append("select isnull(cv.id,0) as ID, s.name AS Name,cv.Description as Description ,CONVERT(VARCHAR(10), crdate, 120) as Createdon,isnull(cv.Query,'') as Query  from sysobjects as s ");
                    strqry.Append("left outer join  RM_CustomViews as cv  on  s.name=cv.Name ");
                    strqry.Append("where s.type='v' and s.name LIKE 'CV_%' and cv.id=:ID ");
                }
                else
                {
                    strqry.Append("select isnull(cv.id,0) as ID, s.name AS Name,cv.Description as Description ,CONVERT(VARCHAR(10), crdate, 120) as Createdon,isnull(cv.Query,'') as Query  from sysobjects as s ");
                    strqry.Append("left outer join  RM_CustomViews as cv  on  s.name=cv.Name ");
                    strqry.Append("where s.type='v'  and s.name LIKE 'CV_%' order by  cv.id ");
                }

                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    if (paramList.Count > 0)
                    {
                        listresult = tx.PersistenceManager.ReportRepository.ExecuteQuerywithParam(strqry.ToString(), paramList);
                    }
                    else
                    {
                        listresult = tx.PersistenceManager.ReportRepository.ExecuteQuery(strqry.ToString());
                    }

                    tx.Commit();
                }

                return listresult;

            }
            catch (Exception)
            {

                return null;
            }

        }

        public bool CustomViews_DeleteByID(ReportManagerProxy proxy, int ID)
        {

            try
            {
                ClsDb clsDb = new ClsDb();
                DataSet dataSet = new DataSet();
                IList listresult;
                StringBuilder strqry = new StringBuilder();

                if (ID > 0)
                {
                    strqry.Append("Declare @SQL nvarchar(200),@viewname varchar(250) ");
                    strqry.Append("Select @viewname=name from  RM_CustomViews where  id=" + ID + " ");
                    strqry.Append("Set @SQL = N'DROP VIEW ' + @viewname ");
                    strqry.Append(" delete from  RM_CustomViews where  id=" + ID + " ");
                    //strqry.Append("go ");
                    strqry.Append("Exec (@SQL) ");

                    //strqry.Append("go ");

                    //strqry.Append("go ");
                }


                //using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                // {
                clsDb.MailData(strqry.ToString(), CommandType.Text);
                //   listresult = tx.PersistenceManager.ReportRepository.ExecuteQuery(strqry.ToString());
                //  }

                // bool schema = pushSchema(proxy);
                int resultpushSchema = pushviewSchema(proxy);
                if (resultpushSchema == 0)
                {
                    return true;
                }
                else
                {
                    //UpdateReportSchemaResponse(proxy, resultpushSchema);
                    return true;
                }


            }
            catch (Exception ex)
            {
                return false;

                LogHandler.LogError("******************************* Failed to do CustomViews_DeleteByID isscue at " + DateTime.Now + " *****************************", ex);
            }
            return false;
        }

        public string CustomViews_Validate(ReportManagerProxy proxy, string Name, string query, int ID)
        {

            try
            {



                string stringToCheck = query.ToUpper();
                string[] stringArray = { "CREATE ", "DROP ", "TRUNCATE ", "ALTER ", "INSERT ", "UPDATE ", "DELETE ", };
                IList listresult;
                if (stringArray.Any(stringToCheck.Contains))
                {
                    return "3";
                }
                if (ID == 0)
                {
                    using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                    {
                        string Name1 = "CV_" + Name;
                        var s = (from tt in tx.PersistenceManager.ReportRepository.Query<CustomViewsDao>() where tt.Name == Name1 select tt).FirstOrDefault();
                        if (s != null)
                        {
                            if (s.Name.Length > 0)
                            {
                                tx.Commit();
                                return "1";
                            }
                        }

                    }

                }

                using (ITransaction tx1 = proxy.MarcomManager.GetTransaction())
                {
                    listresult = tx1.PersistenceManager.ReportRepository.ExecuteQuery(query.ToString());
                    tx1.Commit();
                }

                //bool schema = pushSchema(proxy);
                return "2";

            }
            catch (Exception ex)
            {
                return ex.InnerException.Message.ToString();
            }
            return "0";
        }

        public int CustomViews_Insert(ReportManagerProxy proxy, string Name, string Description, string Query)
        {

            if (DBconnection() == true && ReportServerDBconnection() == true)
            {
                try
                {
                    ClsDb clsDb = new ClsDb();
                    DataSet dataSet = new DataSet();
                    IList listresult;
                    StringBuilder strqry = new StringBuilder();
                    string strqry1 = Query;
                    string Name1 = "CV_" + Name;
                    strqry1 = strqry1.Replace("'", "''");
                    string Description1 = Description;
                    Description1 = Description1.Replace("'", "''");
                    strqry.Append("Declare @SQL nvarchar(max) ");
                    strqry.Append("Set @SQL = N' CREATE VIEW  " + Name1);
                    strqry.Append(" AS " + strqry1 + "'");
                    strqry.Append("Exec (@SQL) ");
                    clsDb.MailData(strqry.ToString(), CommandType.Text);



                    DataSet dsnew = new DataSet();
                    dsnew = clsDb.MailData("INSERT INTO [dbo].[RM_CustomViews] ([Name],[Description],[Query]) VALUES ('" + Name1 + "','" + Description1 + "','" + strqry1 + "')  SELECT SCOPE_IDENTITY() ", CommandType.Text);

                    //bool schema = pushSchema(proxy);

                    int resultpushSchema = pushviewSchema(proxy);
                    if (resultpushSchema == 0)
                    {
                        return Convert.ToInt32(dsnew.Tables[0].Rows[0][0]);
                    }
                    else
                    {
                        //UpdateReportSchemaResponse(proxy, resultpushSchema);
                        return Convert.ToInt32(dsnew.Tables[0].Rows[0][0]); ;
                    }


                }
                catch (Exception ex)
                {
                    LogHandler.LogError("******************************* Failed to do CustomViews_Insert isscue at " + DateTime.Now + " *****************************", ex);
                    return 0;

                }

                return 0;
            }
            else { return 0; }

        }


        public bool CustomViews_Update(ReportManagerProxy proxy, int ID, string Name, string Description, string Query)
        {

            try
            {
                ClsDb clsDb = new ClsDb();
                DataSet dataSet = new DataSet();
                IList listresult;
                StringBuilder strqry = new StringBuilder();
                string strqry1 = Query;
                strqry1 = strqry1.Replace("'", "''");
                string Description1 = Description;
                Description1 = Description1.Replace("'", "''");
                strqry.Append("Declare @SQL nvarchar(max) ");
                strqry.Append("Set @SQL = N' ALTER VIEW  " + Name);
                strqry.Append(" AS " + strqry1 + "'");
                strqry.Append("Exec (@SQL) ");
                clsDb.MailData(strqry.ToString(), CommandType.Text);

                DataSet dsnew = new DataSet();
                dsnew = clsDb.MailData("UPDATE RM_CustomViews  SET Description='" + Description1 + "' ,Query='" + strqry1 + "' WHERE ID=" + ID + "", CommandType.Text);

                // bool schema = pushSchema(proxy);
                int resultpushSchema = pushviewSchema(proxy);
                if (resultpushSchema == 0)
                {
                    return true;
                }
                else
                {
                    //UpdateReportSchemaResponse(proxy, resultpushSchema);
                    return true;
                }



            }
            catch (Exception)
            {
                return false;

            }

            return false;

        }


        public bool Report_InsertUpdate(ReportManagerProxy proxy, int OID, string Name, string Caption, string Description, string Preview, bool Show, int CategoryId, bool EntityLevel, bool SubLevel, int id)
        {
            try
            {

                //if (proxy.MarcomManager.AccessManager.CheckAccess(Modules.Admin, 4, FeatureID.Report, OperationId.Self) == true)
                //{
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    ReportDao RDao = new ReportDao();

                    if (id != 0)
                    {
                        RDao.Id = id;
                    }
                    //RDao.Id = id;
                    RDao.OID = OID;
                    RDao.Name = HttpUtility.HtmlEncode(Name);
                    RDao.Caption = HttpUtility.HtmlEncode(Caption);
                    RDao.Description = HttpUtility.HtmlEncode(Description);
                    RDao.Preview = Preview;
                    RDao.Show = Show;
                    RDao.CategoryId = CategoryId;
                    RDao.EntityLevel = EntityLevel;
                    RDao.SubLevel = SubLevel;
                    tx.PersistenceManager.ReportRepository.Save<ReportDao>(RDao);
                    tx.Commit();
                    return true;

                }
                //}
            }
            catch (Exception ex)
            {
                return false;
            }

            return false;

        }

        public bool UpdateReportImage(ReportManagerProxy proxy, string sourcepath, int imgwidth, int imgheight, int imgX, int imgY, int OID, string Preview, string ReportName)
        {
            using (ITransaction tx = proxy.MarcomManager.GetTransaction())
            {
                try
                {
                    string orgsourcepath = HttpContext.Current.Server.MapPath(sourcepath);

                    orgsourcepath = orgsourcepath.Replace("report\\", "");
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
                                MemoryStream ms = new MemoryStream();
                                bmp.Save(ms, OriginalImage.RawFormat);
                                byte[] CropImage = ms.GetBuffer();
                                using (MemoryStream ms1 = new MemoryStream(CropImage, 0, CropImage.Length))
                                {
                                    ms.Write(CropImage, 0, CropImage.Length);
                                    using (SD.Image CroppedImage = SD.Image.FromStream(ms, true))
                                    {
                                        string destinationpath = HttpContext.Current.Server.MapPath("Files//ReportFiles//Images//" + Preview);
                                        destinationpath = destinationpath.Replace("report\\", "");
                                        if (File.Exists(destinationpath))
                                        {
                                            File.Delete(destinationpath);
                                        }
                                        CroppedImage.Save(destinationpath, CroppedImage.RawFormat);
                                    }
                                }
                            }
                        }
                    }




                    var reportResult = tx.PersistenceManager.ReportRepository.Query<ReportDao>().Where(a => a.OID == OID).FirstOrDefault();
                    string query = string.Empty;
                    IList<MultiProperty> paramList = new List<MultiProperty>();

                    if (reportResult != null)
                    {
                        paramList.Add(new MultiProperty { propertyName = "OID", propertyValue = OID });
                        paramList.Add(new MultiProperty { propertyName = "Preview", propertyValue = Preview });
                        query = "Update RM_Report set Preview= :Preview  where OID = :OID ";
                    }
                    else
                    {
                        paramList.Add(new MultiProperty { propertyName = "OID", propertyValue = OID });
                        paramList.Add(new MultiProperty { propertyName = "Preview", propertyValue = Preview });
                        paramList.Add(new MultiProperty { propertyName = "ReportName", propertyValue = ReportName });
                        query = "Insert into RM_Report (OID,[Name],Caption,Description,Preview,Show,EntityLevel,SubLevel) values( :OID ,:ReportName ,'-','-',:Preview,0,0,0)";
                    }
                    tx.PersistenceManager.ReportRepository.ExecuteQuerywithParam(query, paramList);
                    tx.Commit();
                    return true;
                }
                catch
                {
                    throw;
                }
            }
        }


        public IList ReportLogin(string ReportUrl, string ViewerUsername, string ViewerPassword)
        {
            try
            {

                CookieContainer cookieContainer = new CookieContainer();

                // RSConfigInfo.ServerBasedAddress="http://192.168.1.206:83/";
                var callContext = new ServiceCallContext<IAuthenticationService>(cookieContainer, new EndpointAddress(ReportUrl + "AuthenticationService.svc"));
                if (!callContext.Channel.Login(ViewerUsername, ViewerPassword))
                {
                    LogHandler.LogInfo("************************ log  not sucess " + DateTime.Now + " ************************", LogHandler.LogType.General);

                    callContext.Dispose();
                    return null;
                }
                else
                {
                    LogHandler.LogInfo("************************ log  sucess " + DateTime.Now + " ************************", LogHandler.LogType.General);

                    IList Result = GetCategories(cookieContainer, ReportUrl).Select(x => new ReportModel { Id = x.Id, Name = x.Name }).ToList();

                    return Result;


                }
                //using (var callContext = new ServiceCallContext<IAuthenticationService>(cookieContainer, new EndpointAddress("http://192.168.1.206:83/" + "AuthenticationService.svc"))
                //{

                //    //RSConfigInfo.ViewerUsername = ViewerUsername;
                //    //RSConfigInfo.ViewerPassword = ViewerPassword;

                //    //LogHandler.LogInfo("************************ ReportLogin trying to auntheticate " + DateTime.Now + " ************************", LogHandler.LogType.General);

                //    if (!callContext.Channel.Login(ViewerUsername, ViewerPassword))
                //    {
                //    //    LogHandler.LogInfo("************************ log not sucess " + DateTime.Now + " ************************", LogHandler.LogType.General);
                //        return null;
                //    }
                //    else
                //    {
                //        IList Result = GetCategories(cookieContainer).Select(x => new ReportModel { Id = x.Id, Name = x.Name }).ToList();
                //        return Result;
                //    }
                //}
                //Login(cookieContainer);
                // Read the authentication cookie and attach it to the response,
                // so that the embedded Silverlight viewer can authenticate on the Report Server
                //AttachAuthCookie(cookieContainer);

            }
            catch (Exception ex)
            {
                LogHandler.LogError("******************************* Failed to do CustomViews_Insert isscue at " + DateTime.Now + "ReportLogin->" + ex.Message + " *****************************", ex);

                return null;
            }
        }

        public IEnumerable<ReportModel> GetListOfReports(ReportManagerProxy proxy, string ViewerUsername, string ViewerPassword)
        {
            try
            {
                CookieContainer cookieContainer = new CookieContainer();
                IList relsed = ReportCredential_Select(proxy, 0);
                string ReportUrl = null;
                foreach (var item in relsed)
                {
                    ReportUrl = (string)((Hashtable)(item))["ReportUrl"];
                }

                using (var callContext = new ServiceCallContext<IAuthenticationService>(cookieContainer, new EndpointAddress(ReportUrl + "AuthenticationService.svc")))
                {

                    //RSConfigInfo.ViewerUsername = ViewerUsername;
                    //RSConfigInfo.ViewerPassword = ViewerPassword;

                    if (!callContext.Channel.Login(ViewerUsername, ViewerPassword))
                    {
                        return null;
                    }
                    else
                    {
                        using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                        {
                            ReportCredentialDao rptCrd = new ReportCredentialDao();
                            var val = tx.PersistenceManager.ReportRepository.GetAll<ReportCredentialDao>().LastOrDefault();


                            IEnumerable<ReportModel> Result = GetReports(cookieContainer, val.Category, proxy).Select(x => new ReportModel { Id = x.Id, Name = x.Name, Category = x.CategoryId });
                            tx.Commit();
                            return Result;
                        }
                    }
                }
                //Login(cookieContainer);
                // Read the authentication cookie and attach it to the response,
                // so that the embedded Silverlight viewer can authenticate on the Report Server
                //AttachAuthCookie(cookieContainer);

            }
            catch (Exception ex)
            {
                LogHandler.LogInfo("******************************* Failed to GetListOfReports " + ex.Message + " *****************************", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);

            }
            return null;
        }

        IEnumerable<ReportCatalogItemDto> GetReports(CookieContainer cookieContainer, int CategoryID, ReportManagerProxy proxy)
        {
            IList relsed1 = ReportCredential_Select(proxy, 0);
            string ReportUrl = null;
            foreach (var item in relsed1)
            {
                ReportUrl = (string)((Hashtable)(item))["ReportUrl"];
            }
            using (var callContext = new ServiceCallContext<IReportServerFacadeAsync>(cookieContainer, new EndpointAddress(ReportUrl + "ReportServerFacade.svc")))
            {
                Task<IEnumerable<ReportCatalogItemDto>> task =
                    Task<IEnumerable<ReportCatalogItemDto>>.Factory.FromAsync(callContext.Channel.BeginGetReports, callContext.Channel.EndGetReports, null);

                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    var list = task.Result.Where(a => a.CategoryId == CategoryID).Select(a => a).ToList();
                    IList<ReportDao> reportList = new List<ReportDao>();

                    foreach (var val in list.ToList())
                    {
                        var i = 0;
                        ReportDao report = new ReportDao();
                        report.Name = HttpUtility.HtmlEncode(val.Name);
                        report.OID = val.Id;
                        report.CategoryId = val.CategoryId;
                        reportList.Add(report);
                    }

                    //tx.PersistenceManager.ReportRepository.Save<ReportDao>(reportList);
                    tx.Commit();



                }
                return task.Result.Where(a => a.CategoryId == CategoryID).Select(a => a);
            }
        }

        IEnumerable<CategoryDto> GetCategories(CookieContainer cookieContainer, string ReportUrl)
        {
            try
            {
                using (var callContext = new ServiceCallContext<IReportServerFacadeAsync>(cookieContainer, new EndpointAddress(ReportUrl + "ReportServerFacade.svc")))
                {
                    Task<IEnumerable<CategoryDto>> task =
                        Task<IEnumerable<CategoryDto>>.Factory.FromAsync(callContext.Channel.BeginGetCategories, callContext.Channel.EndGetCategories, null);

                    LogHandler.LogInfo("************************Task count of data " + task.Result.ToList().Count + " ************************", LogHandler.LogType.General);

                    return task.Result;
                }
            }
            catch (Exception ex)
            {
                LogHandler.LogInfo("******************************* Failed to GetCategories " + ex.Message + " *****************************", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);

            }
            return null;
        }

        public IList<IDataView> Dataview_select(int DataViewID, string username = null)
        {
            IList<IDataView> Dview = new List<IDataView>();
            ClsDb clsDb = new ClsDb();
            if (DBconnection() == true && ReportServerDBconnection() == true)
            {
                if (username.Length > 0 && username != null && DataViewID > 0)
                {
                    //Dview = clsDb.Dataview_select("select oid as DataviewID ,name as Dataviewname from DataView where oid in( select DataView from [DataViewScope] where oid in(select OID from  UserPermissions where [user] in(  select oid from   UserAccount where   UserName='" + username + "'))) and  OID = " + DataViewID + " ", CommandType.Text);
                    Dview = clsDb.Dataview_select("SELECT oid  AS DataviewID,NAME  AS Dataviewname FROM  DataView  WHERE  oid IN (SELECT DataView  FROM   [DataViewScope] WHERE  oid IN (SELECT Scope  FROM  [dbo].[AccessControlEntry] WHERE mode IN(6,14)  AND oid IN (SELECT OID FROM   UserPermissions WHERE  [user] IN (SELECT oid  FROM UserAccount WHERE UserName ='" + username + "')))) and  OID = " + DataViewID, CommandType.Text);
                }
                else if (username.Length > 0 && username != null)
                {
                    //Dview = clsDb.Dataview_select("select oid as DataviewID ,name as Dataviewname from DataView where oid in( select DataView from [DataViewScope] where oid in(select OID from  UserPermissions where [user] in(  select oid from   UserAccount where   UserName='" + username + "'))) ", CommandType.Text);
                    Dview = clsDb.Dataview_select("SELECT oid  AS DataviewID,NAME  AS Dataviewname FROM  DataView  WHERE  oid IN (SELECT DataView  FROM   [DataViewScope] WHERE  oid IN (SELECT Scope  FROM  [dbo].[AccessControlEntry] WHERE mode IN(6,14)  AND oid IN (SELECT OID FROM   UserPermissions WHERE  [user] IN (SELECT oid  FROM UserAccount WHERE UserName ='" + username + "'))))", CommandType.Text);
                }
                else
                {
                    Dview = clsDb.Dataview_select("select oid as DataviewID ,name as Dataviewname from DataView", CommandType.Text);
                }
            }
            return Dview;
        }

        public int pushviewSchema(ReportManagerProxy proxy)
        {
            if (DBconnection() == true && ReportServerDBconnection() == true)
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    ReportCredentialDao rptCrd = new ReportCredentialDao();
                    var val = tx.PersistenceManager.ReportRepository.GetAll<ReportCredentialDao>().LastOrDefault();

                    if (val == null)
                    {
                        LogHandler.LogInfo("******************************* Failed to do pushviewSchema  NO ReportCredential isscue at " + DateTime.Now + " *****************************", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);
                        UpdateReportSchemaResponse(proxy, 1);
                        return 1;
                    }

                    else if ((val.AdminUsername.Length < 0 || val.DataViewID < 1))
                    {
                        LogHandler.LogInfo("******************************* Failed to do pushviewSchema ReportCredential isscue at " + DateTime.Now + " *****************************", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);
                        UpdateReportSchemaResponse(proxy, 1);
                        return 1;
                    }
                    //else

                    //{

                    //    LogHandler.LogInfo("******************************* Failed to do pushviewSchema  NO ReportCredential isscue at " + DateTime.Now + " *****************************", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);
                    //    UpdateReportSchemaResponse(proxy, 1);
                    //    return 1;

                    //}


                    bool result = pushSchema(proxy);
                    if (result == true)
                    {
                        LogHandler.LogInfo("******************************* Success pushviewSchema at " + DateTime.Now + " *****************************", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);
                        UpdateReportSchemaResponse(proxy, 0);
                        return 0;
                    }
                    else
                    {
                        LogHandler.LogInfo("******************************* Failed to do pushviewSchema at " + DateTime.Now + " *****************************", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);
                        UpdateReportSchemaResponse(proxy, 2);
                        return 2;
                    }
                }
            }
            else
            {
                LogHandler.LogInfo("******************************* Failed to do pushviewSchema sqlcoonction isscue at " + DateTime.Now + " *****************************", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);
                UpdateReportSchemaResponse(proxy, 3);
                return 3;
            }

        }
        public bool pushSchema(ReportManagerProxy proxy)
        {
            if (DBconnection() == true && ReportServerDBconnection() == true)
            {
                //SqlCommand sqlcmd1 = new SqlCommand();
                //sqlcmd1.Connection = sqlcon;
                //sqlcmd1.CommandType = CommandType.Text;
                //sqlcmd1.CommandText = "SELECT  * FROM  Vw_ActivityReport";
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    DataSet ds = new DataSet();
                    // var listofviews = tx.PersistenceManager.ReportRepository.ExecuteQuery("SELECT DISTINCT NAME ,ID FROM RM_CustomViews ORDER BY id");
                    var listofviews = tx.PersistenceManager.ReportRepository.ExecuteQuery("select s.name AS NAME FROM  sysobjects as s where s.type='v' ORDER BY crdate");
                    foreach (var itemss in listofviews)
                    {

                        try
                        {
                            string selectquery = "select * from " + ((Hashtable)(itemss))["NAME"].ToString();
                            string mappedname = ((Hashtable)(itemss))["NAME"].ToString();
                            SqlDataAdapter da = new SqlDataAdapter(selectquery, sqlcon);
                            da.FillSchema(ds, SchemaType.Mapped, mappedname);
                        }
                        catch (Exception ex)
                        {
                            LogHandler.LogError("******************************* Failed to do pushSchema isscue at in mappeing ;" + DateTime.Now + " *****************************", ex);
                            return false;
                        }


                    }
                    StringWriter strm = new StringWriter();
                    ds.WriteXmlSchema(strm);
                    strm.ToString();
                    StringBuilder Relationship = new StringBuilder();
                    try
                    {
                        Relationship.Append("<xs:annotation>");
                        Relationship.Append("<xs:appinfo>");

                        string Member = "SPV_Member";
                        string Financial = "SPV_Financial";
                        string Task = "SPV_Task";
                        string Task_List = "SPV_TaskList";
                        string Objective = "SPV_Objective";
                        string Attachment = "SPV_Attachment";
                        string Milestone = "SPV_Milestone";

                        Relationship.Append("<msdata:Relationship name=''SPV_TaskList_Task'' msdata:parent=''" + Task_List + "'' msdata:child=''" + Task + "'' msdata:parentkey=''" + "ID" + "'' msdata:childkey=''" + "TaskListID" + "'' />");

                        for (int i = 0; i < ds.Tables.Count; i++)
                        {
                            string tableName = ds.Tables[i].TableName;

                            string tableNameMember = ds.Tables[i].TableName + "_Member";
                            string tableNameFinancial = ds.Tables[i].TableName + "_Financial";
                            string tableNameTaskList = ds.Tables[i].TableName + "_TaskList";
                            string tableNameTask = ds.Tables[i].TableName + "_Task";
                            string tableNameObjective = ds.Tables[i].TableName + "_Objective";
                            string tableNameAttachment = ds.Tables[i].TableName + "_Attachment";
                            string tableNameMilestone = ds.Tables[i].TableName + "_Milestone";


                            string strparentkey = "ID";
                            string strchildkey = "EntityID";

                            if (tableName.StartsWith("SV_"))
                            {
                                Relationship.Append("<msdata:Relationship name=''" + tableNameMember + "'' msdata:parent=''" + tableName + "'' msdata:child=''" + Member + "'' msdata:parentkey=''" + strparentkey + "'' msdata:childkey=''" + strchildkey + "'' />");
                                Relationship.Append("<msdata:Relationship name=''" + tableNameFinancial + "'' msdata:parent=''" + tableName + "'' msdata:child=''" + Financial + "'' msdata:parentkey=''" + strparentkey + "'' msdata:childkey=''" + strchildkey + "'' />");
                                Relationship.Append("<msdata:Relationship name=''" + tableNameTaskList + "'' msdata:parent=''" + tableName + "'' msdata:child=''" + Task_List + "'' msdata:parentkey=''" + strparentkey + "'' msdata:childkey=''" + strchildkey + "'' />");
                                Relationship.Append("<msdata:Relationship name=''" + tableNameTask + "'' msdata:parent=''" + tableName + "'' msdata:child=''" + Task + "'' msdata:parentkey=''" + strparentkey + "'' msdata:childkey=''" + strchildkey + "'' />");
                                Relationship.Append("<msdata:Relationship name=''" + tableNameObjective + "'' msdata:parent=''" + tableName + "'' msdata:child=''" + Objective + "'' msdata:parentkey=''" + strparentkey + "'' msdata:childkey=''" + strchildkey + "'' />");
                                Relationship.Append("<msdata:Relationship name=''" + tableNameAttachment + "'' msdata:parent=''" + tableName + "'' msdata:child=''" + Attachment + "'' msdata:parentkey=''" + strparentkey + "'' msdata:childkey=''" + strchildkey + "'' />");
                                Relationship.Append("<msdata:Relationship name=''" + tableNameMilestone + "'' msdata:parent=''" + tableName + "'' msdata:child=''" + Milestone + "'' msdata:parentkey=''" + strparentkey + "'' msdata:childkey=''" + strchildkey + "'' />");

                            }
                            else if (tableName.StartsWith("CLV_"))
                            {
                                IList<ICustomList> CustomList = GetAllCustomList(proxy);
                                var customlistresult = CustomList.Where(c => c.Name == tableName.Replace("CLV_", "").ToString()).ToList();
                                if (customlistresult.Count != 0)
                                {
                                    XDocument xd = XDocument.Parse(customlistresult[0].XmlData);
                                    IList<XElement> AdditionalInfo = xd.Descendants("CustomList").Descendants("AdditionalInfos").Descendants("AdditionalInfo").ToList();


                                    //var attributerelationList = (from AdminAttributes in attributeLists
                                    //                             join ser in attributes on Convert.ToInt16(AdminAttributes.Attribute("Id").Value) equals ser.Id
                                    //                             select new
                                    //                             {
                                    //                                 ID = Convert.ToInt16(AdminAttributes.Attribute("Id").Value),
                                    //                                 Type = ser.AttributeTypeID,
                                    //                                 IsSpecial = ser.IsSpecial,
                                    //                                 Field = ser.Id,
                                    //                                 Level = Convert.ToInt16(AdminAttributes.Attribute("Level").Value),
                                    //                                 Caption = ser.Caption,
                                    //                             }).Distinct().ToList();-


                                    // loop through additional info

                                    foreach (var addinfo in AdditionalInfo)
                                    {
                                        switch (int.Parse(addinfo.FirstAttribute.Value))
                                        {
                                            case 1:
                                                Relationship.Append("<msdata:Relationship name=''" + tableNameMember + "'' msdata:parent=''" + tableName + "'' msdata:child=''" + Member + "'' msdata:parentkey=''" + strparentkey + "'' msdata:childkey=''" + strchildkey + "'' />");
                                                break;
                                            case 2:
                                                Relationship.Append("<msdata:Relationship name=''" + tableNameFinancial + "'' msdata:parent=''" + tableName + "'' msdata:child=''" + Financial + "'' msdata:parentkey=''" + strparentkey + "'' msdata:childkey=''" + strchildkey + "'' />");
                                                break;
                                            case 3:
                                                Relationship.Append("<msdata:Relationship name=''" + tableNameTaskList + "'' msdata:parent=''" + tableName + "'' msdata:child=''" + Task_List + "'' msdata:parentkey=''" + strparentkey + "'' msdata:childkey=''" + strchildkey + "'' />");
                                                break;
                                            case 4:
                                                Relationship.Append("<msdata:Relationship name=''" + tableNameTask + "'' msdata:parent=''" + tableName + "'' msdata:child=''" + Task + "'' msdata:parentkey=''" + strparentkey + "'' msdata:childkey=''" + strchildkey + "'' />");
                                                break;
                                            case 5:
                                                Relationship.Append("<msdata:Relationship name=''" + tableNameObjective + "'' msdata:parent=''" + tableName + "'' msdata:child=''" + Objective + "'' msdata:parentkey=''" + strparentkey + "'' msdata:childkey=''" + strchildkey + "'' />");
                                                break;
                                            case 6:
                                                Relationship.Append("<msdata:Relationship name=''" + tableNameAttachment + "'' msdata:parent=''" + tableName + "'' msdata:child=''" + Attachment + "'' msdata:parentkey=''" + strparentkey + "'' msdata:childkey=''" + strchildkey + "'' />");
                                                break;
                                            case 7:
                                                Relationship.Append("<msdata:Relationship name=''" + tableNameMilestone + "'' msdata:parent=''" + tableName + "'' msdata:child=''" + Milestone + "'' msdata:parentkey=''" + strparentkey + "'' msdata:childkey=''" + strchildkey + "'' />");
                                                break;

                                        }
                                    }


                                }

                            }
                        }
                        Relationship.Append("</xs:appinfo>");
                        Relationship.Append("</xs:annotation>");
                        Relationship.Append("</xs:schema>");
                    }
                    catch (Exception ex)
                    {
                        LogHandler.LogError("******************************* Failed to while adding realtionship" + DateTime.Now + " *****************************", ex);
                        return false;
                    }


                    StringBuilder DataViews = new StringBuilder();
                    DataViews.Append(strm.ToString().Replace("</xs:schema>", Relationship.ToString()));

                    if (ReportServerDBconnection() == true && DataViews.ToString().Length > 0)
                    {
                        try
                        {
                            ClsDb clsDb = new ClsDb();
                            DataSet dataSet = new DataSet();
                            DataSet ds1 = new DataSet();
                            IList<int> listresultdataview = new List<int>();
                            //StringBuilder strqry = new StringBuilder();
                            //clsDb.MailData(strqry.ToString(), CommandType.Text);
                            ReportCredentialDao rptCrd = new ReportCredentialDao();
                            var val = tx.PersistenceManager.ReportRepository.GetAll<ReportCredentialDao>().LastOrDefault();

                            //var listofdataviewid = tx.PersistenceManager.ReportRepository.ExecuteQuery("SELECT DISTINCT NAME ,ID FROM RM_CustomViews ORDER BY id");
                            tx.Commit();
                            DataSet dsnew = new DataSet();
                            dsnew = clsDb.reportserverData("SELECT oid  AS DataviewID,NAME  AS Dataviewname FROM  DataView  WHERE  oid IN (SELECT DataView  FROM   [DataViewScope] WHERE  oid IN (SELECT Scope  FROM  [dbo].[AccessControlEntry] WHERE mode IN(6,14)  AND oid IN (SELECT OID FROM   UserPermissions WHERE  [user] IN (SELECT oid  FROM UserAccount WHERE UserName ='" + val.AdminUsername + "')))) and  OID = " + val.DataViewID, CommandType.Text);
                            //dsnew = clsDb.reportserverData("select oid from DataView where oid in( select DataView from [DataViewScope] where oid in(select OID from  UserPermissions where [user] in(  select oid from  UserAccount where UserName='" + val.AdminUsername + "'))) and  OID = " + val.DataViewID + " ", CommandType.Text);

                            foreach (DataRow prod in dsnew.Tables[0].Rows)
                            {
                                listresultdataview.Add(Convert.ToInt32(prod.ItemArray[0]));
                            }

                            if (listresultdataview.Count == 1)
                            {
                                //strm = strm.ToString().Replace(',');
                                bool updatedate = clsDb.reportserverupdateData("update DataView set LastModifiedTime=getdate(),DbSchema='" + DataViews.ToString() + "' where oid=" + listresultdataview[0] + "", CommandType.Text);


                            }
                            else
                            {
                                return true;
                            }
                            return true;

                        }
                        catch (Exception ex)
                        {
                            LogHandler.LogError("******************************* Failed to do pushSchema isscue at " + DateTime.Now + " *****************************", ex);

                        }

                        return false;
                    }
                    else
                    {
                        return false;
                    }
                }

                //SqlDataAdapter da = new SqlDataAdapter("SELECT  * FROM  Vw_ActivityReport", sqlcon);
                //DataSet ds = new DataSet();
                //da.FillSchema(ds, SchemaType.Mapped, "Vw_ActivityReport");
                //StringWriter strm = new StringWriter();
                //ds.WriteXmlSchema(strm);
                //strm.ToString();
            }
            else
            {
                return false;
            }
        }


        public IList<IReports> MergeReports(ReportManagerProxy proxy, int oid)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    IList<IReports> datacollection = new List<IReports>();
                    IReports report;
                    datacollection = ReportSelectionForMerge(tx, 0);
                    ReportCredentialDao rptCrd1 = new ReportCredentialDao();
                    var val1 = tx.PersistenceManager.ReportRepository.GetAll<ReportCredentialDao>().LastOrDefault();
                    IList<IReports> iireports = new List<IReports>();
                    IEnumerable<ReportModel> Oids = GetListOfReports(proxy, val1.ViewerUsername, val1.ViewerPassword);
                    //var oidDataRes = Report_Select(proxy, 0);
                    foreach (var item in Oids)
                    {
                        report = new Reports();
                        var oidData = datacollection.Where(a => a.OID == item.Id).FirstOrDefault();
                        //oidData.Id = 0;
                        report.OID = item.Id;
                        report.Name = HttpUtility.HtmlDecode(item.Name);
                        report.CategoryId = item.Category;

                        //report.c
                        if (oidData != null)
                        {
                            if (oidData.Description != null || oidData.Caption != null || oidData.Preview != null)
                            {
                                report.Id = oidData.Id;
                                report.Status = "Linked";
                            }
                            else
                            {
                                report.Id = 0;
                                report.Status = "Unlinked";
                            }
                        }
                        else
                        {
                            report.Id = 0;
                            report.Status = "Unlinked";
                        }
                        //report.Id = oidData != null ? oidData.Id : 0;
                        report.Description = oidData != null ? (oidData.Description != null ? HttpUtility.HtmlDecode(oidData.Description) : "-") : "-";
                        report.Caption = oidData != null ? (oidData.Caption != null ? HttpUtility.HtmlDecode(oidData.Caption) : "-") : "-";
                        report.Preview = oidData != null ? (oidData.Preview != null ? oidData.Preview : "NoPreview.jpg") : "NoPreview.jpg";
                        report.Show = oidData != null ? oidData.Show : false;
                        report.EntityLevel = oidData != null ? oidData.EntityLevel : false;
                        report.SubLevel = oidData != null ? oidData.SubLevel : false;
                        //report.Status = oidData != null ? "Linked" : "Unlinked";
                        iireports.Add(report);

                    }
                    tx.Commit();
                    return iireports;

                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public IList<IReports> ReportSelectionForMerge(ITransaction tx, int OID)
        {

            try
            {
                IList listresult1;
                StringBuilder strqry = new StringBuilder();

                IList<IReports> datacollection1 = new List<IReports>();

                IList<MultiProperty> paramList = new List<MultiProperty>();


                if (OID > 0)
                {
                    paramList.Add(new MultiProperty { propertyName = "OID", propertyValue = OID });
                    strqry.Append("SELECT ID,OID,Name,Caption,Description,Preview,Show,0 AS status,EntityLevel,SubLevel  FROM [dbo].[RM_Report] WHERE OID =:OID ");
                }
                else
                {
                    strqry.Append("SELECT ID,OID,Name,Caption,Description,Preview,Show,0 AS status,EntityLevel,SubLevel  FROM [dbo].[RM_Report] ORDER BY OID");

                }
                if (paramList.Count > 0)
                {
                    listresult1 = tx.PersistenceManager.ReportRepository.ExecuteQuerywithParam(strqry.ToString(), paramList);
                }
                else
                {
                    listresult1 = tx.PersistenceManager.ReportRepository.ExecuteQuery(strqry.ToString());
                }
                foreach (var item in listresult1)
                {
                    IReports report = new Reports();

                    report.Id = (int)((Hashtable)(item))["ID"];
                    report.OID = (int)((Hashtable)(item))["OID"];
                    report.Name = HttpUtility.HtmlDecode((string)((Hashtable)(item))["Name"]);
                    report.Caption = HttpUtility.HtmlDecode((string)((Hashtable)(item))["Caption"]);
                    report.Description = HttpUtility.HtmlDecode((string)((Hashtable)(item))["Description"]);
                    report.Preview = (string)((Hashtable)(item))["Preview"];
                    report.Show = (bool)((Hashtable)(item))["Show"];
                    report.Status = "Linked";
                    report.EntityLevel = (bool)((Hashtable)(item))["EntityLevel"];
                    report.SubLevel = (bool)((Hashtable)(item))["SubLevel"];

                    datacollection1.Add(report);

                }


                return datacollection1;

            }
            catch (Exception)
            {

                return null;
            }

        }

        public IList<IReports> ShowReports(ReportManagerProxy proxy, int OID, bool show)
        {

            try
            {
                IList listresult1;
                StringBuilder strqry = new StringBuilder();

                IList<IReports> datacollection1 = new List<IReports>();



                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    IList<MultiProperty> paramList = new List<MultiProperty>();
                    if (OID > 0)
                    {
                        paramList.Add(new MultiProperty { propertyName = "OID", propertyValue = OID });
                        strqry.Append("SELECT ID,OID,Name,Caption,Description,Preview,Show,0 AS status,CategoryId,EntityLevel,SubLevel FROM RM_Report WHERE OID =:OID  AND CategoryId=(SELECT  TOP 1 isnull(Category,0) AS CategoryId  FROM RM_ReportCredential ORDER BY id DESC) ");
                    }
                    else if (show == true)
                    {
                        strqry.Append("SELECT ID,OID,Name,Caption,Description,Preview,Show,0 AS status,CategoryId,EntityLevel,SubLevel FROM RM_Report  WHERE Show=1 AND CategoryId=(SELECT  TOP 1 isnull(Category,0) AS CategoryId  FROM RM_ReportCredential ORDER BY id DESC)");
                    }

                    if (paramList.Count > 0)
                    {
                        listresult1 = tx.PersistenceManager.ReportRepository.ExecuteQuerywithParam(strqry.ToString(), paramList);
                    }
                    else
                    {
                        listresult1 = tx.PersistenceManager.ReportRepository.ExecuteQuery(strqry.ToString());
                    }
                    foreach (var item in listresult1)
                    {
                        IReports report = new Reports();

                        report.Id = (int)((Hashtable)(item))["ID"];
                        report.OID = (int)((Hashtable)(item))["OID"];
                        report.Name = HttpUtility.HtmlDecode((string)((Hashtable)(item))["Name"]);
                        report.Caption = HttpUtility.HtmlDecode((string)((Hashtable)(item))["Caption"]);
                        report.Description = HttpUtility.HtmlDecode((string)((Hashtable)(item))["Description"]);
                        report.Preview = (string)((Hashtable)(item))["Preview"];
                        report.Show = (bool)((Hashtable)(item))["Show"];
                        report.Status = "Linked";
                        report.EntityLevel = (bool)((Hashtable)(item))["EntityLevel"];
                        report.SubLevel = (bool)((Hashtable)(item))["SubLevel"];

                        datacollection1.Add(report);

                    }
                    tx.Commit();
                }


                return datacollection1;

            }
            catch (Exception)
            {

                return null;
            }

        }
        public bool UpdateReportSchemaResponse(ReportManagerProxy proxy, int status)
        {
            string xmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
            XDocument adminXmlDoc = XDocument.Load(xmlpath);
            var DefaultReportsetting = adminXmlDoc.Descendants("ReportSettings").FirstOrDefault();
            var DefaultSchemaResponse = adminXmlDoc.Descendants("ReportSettings").Descendants("ReportServerSchemaResponse").FirstOrDefault();
            if (DefaultReportsetting == null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<ReportSettings><ReportServerSchemaResponse>" + status + "</ReportServerSchemaResponse></ReportSettings>");
                XElement.Parse(sb.ToString());
                adminXmlDoc.Root.Add(XElement.Parse(sb.ToString()));
                adminXmlDoc.Save(xmlpath);

            }
            else if (DefaultSchemaResponse == null)
            {

                XElement ReportSettingsElement = adminXmlDoc.Descendants("ReportSettings").FirstOrDefault();
                ReportSettingsElement.SetElementValue("ReportServerSchemaResponse", status);
                adminXmlDoc.Save(xmlpath);
            }
            else if (DefaultReportsetting != null && DefaultSchemaResponse != null)
            {
                adminXmlDoc.Descendants("ReportSettings").Descendants("ReportServerSchemaResponse").Remove();
                adminXmlDoc.Save(xmlpath);
                XElement ReportSettingsElement = adminXmlDoc.Descendants("ReportSettings").FirstOrDefault();
                ReportSettingsElement.SetElementValue("ReportServerSchemaResponse", status);
                adminXmlDoc.Save(xmlpath);
            }
            return true;
        }

        public int GetReportViewSchemaResponse(ReportManagerProxy proxy)
        {

            using (ITransaction tx = proxy.MarcomManager.GetTransaction())
            {
                string xmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
                XDocument adminXmlDoc = XDocument.Load(xmlpath);
                var ReportServerSchemaResponse = adminXmlDoc.Descendants("ReportSettings").Descendants("ReportServerSchemaResponse").ElementAt(0).Value;
                tx.Commit();
                return Convert.ToInt32(ReportServerSchemaResponse);
            }
        }

        public string ListofRecordsSystemReport(ReportManagerProxy proxy, int FilterID, IList<IFiltersettingsValues> filterSettingValues, int[] IdArr, string SortOrderColumn, bool IsDesc, ListSettings listSetting, bool IncludeChildren, int enumEntityTypeIds, int EntityID, bool IsSingleID, int UserID, int Level, bool IsobjectiveRootLevel, int ExpandingEntityID, string GanttstartDate, string Ganttenddate, bool IsMonthly)
        {


            try
            {




                IEntityTypeAttributeRelation _ientitytyperelation = new EntityTypeAttributeRelation();
                IList<EntityTypeAttributeRelationDao> dao = new List<EntityTypeAttributeRelationDao>();
                IListofRecord lstrecord = new ListofRecord();
                StringBuilder strqry = new StringBuilder();
                StringBuilder strAttribute = new StringBuilder();
                StringBuilder dynamicTblQry = new StringBuilder();
                StringBuilder multiSelectTblQry = new StringBuilder();
                StringBuilder singleSelectTblQry = new StringBuilder();
                StringBuilder treeTblQry = new StringBuilder();
                StringBuilder periodTblQry = new StringBuilder();
                StringBuilder finalQry = new StringBuilder();

                StringBuilder topFilterQry = new StringBuilder();
                StringBuilder TempTblQry = new StringBuilder();

                StringBuilder Costcent = new StringBuilder();
                StringBuilder Objective = new StringBuilder();


                //StringBuilder XmlFilterQry = new StringBuilder();
                StringBuilder mainTblQry = new StringBuilder();
                IList<Hashtable> CollectedIdsResult = new List<Hashtable>();

                bool IsEntityAvailable = false;



                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    IQueryable<EntityDao> UniqueKeyCollection = null;
                    if (IdArr != null && IdArr.Length > 0)
                    {
                        string EntityIdArr = "("
                                               + String.Join(",", IdArr.Select(x => x.ToString()).ToArray())
                                             + ")";
                        UniqueKeyCollection =
                            tx.PersistenceManager.PlanningRepository.Query<EntityDao>()
                                .Where(a => IdArr.Contains(a.Id));


                    }

                    //Fetch all entity attribute relation
                    dao = tx.PersistenceManager.MetadataRepository.GetAll<EntityTypeAttributeRelationDao>();
                    var ValidEntityTypes = listSetting.EntityTypes;
                    List<int> newValidEntityTypes = new List<int>((int[])listSetting.EntityTypes.ToArray().Clone());

                    if (EntityTypeIDs.Activity == (EntityTypeIDs)enumEntityTypeIds)
                    {
                        topFilterQry.Append(
                            " DECLARE @EntityOrderIDs TABLE ([ID] [int] IDENTITY(1, 1) NOT NULL, EID INT,LEVEL int,PEID int)  ");
                        topFilterQry.Append(" INSERT INTO @EntityOrderIDs ");
                        topFilterQry.Append(" ( ");
                        topFilterQry.Append("  EID,LEVEL,PEID ");
                        topFilterQry.Append(" ) ");
                    }

                    else if (EntityTypeIDs.Costcenre == (EntityTypeIDs)enumEntityTypeIds)
                    {
                        topFilterQry.Append(
                            " DECLARE @CostCentreOrderIDs TABLE ([ID] [int] IDENTITY(1, 1) NOT NULL, EID INT, EKEY NVARCHAR(450), CostCenterID INT Default(0),LEVEL int,PEID int)  ");
                        topFilterQry.Append(
                            " DECLARE @EntityOrderIDs TABLE ([ID] [int] IDENTITY(1, 1) NOT NULL, EID INT, CostCenterID INT Default(0),LEVEL int,PEID int)  ");



                        for (var i = 0; i < IdArr.Length; i++)
                        {
                            topFilterQry.Append(" INSERT INTO @CostCentreOrderIDs ");
                            topFilterQry.Append(" ( ");
                            topFilterQry.Append("  EID,EKEY,CostCenterID,LEVEL,PEID ");
                            topFilterQry.Append(" ) ");
                            topFilterQry.Append(
                                " SELECT pe.ID , pe.UniqueKey, 0 as CostCenterID,pe.Level,pe.parentid FROM PM_Entity pe WHERE pe.ID=" +
                                IdArr[i]);
                            topFilterQry.Append(" INSERT INTO @CostCentreOrderIDs ");
                            topFilterQry.Append(" ( ");
                            topFilterQry.Append("  EID,EKEY,CostCenterID,LEVEL,PEID ");
                            topFilterQry.Append(" ) ");
                            topFilterQry.Append(
                                " SELECT pe.ID, pe.UniqueKey, pecr.CostCenterID,pe.Level,pe.parentid  FROM PM_EntityCostReleations pecr INNER JOIN PM_Entity pe ON pecr.EntityID=pe.ID ");
                            topFilterQry.Append(" AND pe.[Active]=1 AND pecr.IsActive=1  and pecr.CostCenterID=" + IdArr[i] + " ");

                            topFilterQry.Append("INNER JOIN PM_Entity_Sort pes ");
                            topFilterQry.Append("            ON  pe.ID = pes.id");
                            topFilterQry.Append(" ORDER BY ");
                            topFilterQry.Append("       pes.S1   ASC,");
                            topFilterQry.Append("       pes.L1   ASC,");
                            topFilterQry.Append("       pes.S2   ASC,");
                            topFilterQry.Append("       pes.L2   ASC,");
                            topFilterQry.Append("       pes.S3   ASC,");
                            topFilterQry.Append("       pes.L3   ASC,");
                            topFilterQry.Append("       pes.S4   ASC,");
                            topFilterQry.Append("       pes.L4   ASC,");
                            topFilterQry.Append("       pes.S5   ASC,");
                            topFilterQry.Append("       pes.L5   ASC,");
                            topFilterQry.Append("       pes.S6   ASC,");
                            topFilterQry.Append("       pes.L6   ASC,");
                            topFilterQry.Append("       pes.S7   ASC,");
                            topFilterQry.Append("       pes.L7   ASC,");
                            topFilterQry.Append("       pes.S8   ASC,");
                            topFilterQry.Append("       pes.L8   ASC,");
                            topFilterQry.Append("       pes.S9   ASC,");
                            topFilterQry.Append("       pes.L9   ASC,");
                            topFilterQry.Append("       pes.S10  ASC,");
                            topFilterQry.Append("       pes.L10  ASC");


                        }

                    }
                    else if (EntityTypeIDs.Objective == (EntityTypeIDs)enumEntityTypeIds)
                    {
                        topFilterQry.Append(
                            " DECLARE @ObjectiveOrderIDs TABLE ([ID] [int] IDENTITY(1, 1) NOT NULL, EID INT, EKEY NVARCHAR(450), ObjectveID INT Default(0),LEVEL int,PEID int)  ");
                        topFilterQry.Append(
                            " DECLARE @EntityOrderIDs TABLE ([ID] [int] IDENTITY(1, 1) NOT NULL, EID INT, ObjectveID INT Default(0),LEVEL int,PEID int)  ");



                        for (var i = 0; i < IdArr.Length; i++)
                        {
                            topFilterQry.Append(" INSERT INTO @ObjectiveOrderIDs ");
                            topFilterQry.Append(" ( ");
                            topFilterQry.Append("  EID,EKEY,ObjectveID,LEVEL,PEID");
                            topFilterQry.Append(" ) ");
                            topFilterQry.Append(
                                " SELECT pe.ID, pe.UniqueKey,0 as ObjectveID,pe.Level,pe.parentid  FROM PM_Entity pe WHERE pe.ID=" +
                                IdArr[i]);
                            topFilterQry.Append(" INSERT INTO @ObjectiveOrderIDs ");
                            topFilterQry.Append(" ( ");
                            topFilterQry.Append("  EID,EKEY,ObjectveID,LEVEL,PEID ");
                            topFilterQry.Append(" ) ");
                            topFilterQry.Append(
                                " SELECT pe.ID, pe.UniqueKey,poev.ObjectiveID,pe.Level,pe.parentid FROM PM_ObjectiveEntityValue poev INNER JOIN PM_Entity pe ON poev.EntityID=pe.ID  ");
                            topFilterQry.Append(" AND pe.[Active]=1 AND poev.ObjectiveID=" + IdArr[i] + " ");

                            topFilterQry.Append("INNER JOIN PM_Entity_Sort pes ");
                            topFilterQry.Append("            ON  pe.ID = pes.id");
                            topFilterQry.Append(" ORDER BY ");
                            topFilterQry.Append("       pes.S1   ASC,");
                            topFilterQry.Append("       pes.L1   ASC,");
                            topFilterQry.Append("       pes.S2   ASC,");
                            topFilterQry.Append("       pes.L2   ASC,");
                            topFilterQry.Append("       pes.S3   ASC,");
                            topFilterQry.Append("       pes.L3   ASC,");
                            topFilterQry.Append("       pes.S4   ASC,");
                            topFilterQry.Append("       pes.L4   ASC,");
                            topFilterQry.Append("       pes.S5   ASC,");
                            topFilterQry.Append("       pes.L5   ASC,");
                            topFilterQry.Append("       pes.S6   ASC,");
                            topFilterQry.Append("       pes.L6   ASC,");
                            topFilterQry.Append("       pes.S7   ASC,");
                            topFilterQry.Append("       pes.L7   ASC,");
                            topFilterQry.Append("       pes.S8   ASC,");
                            topFilterQry.Append("       pes.L8   ASC,");
                            topFilterQry.Append("       pes.S9   ASC,");
                            topFilterQry.Append("       pes.L9   ASC,");
                            topFilterQry.Append("       pes.S10  ASC,");
                            topFilterQry.Append("       pes.L10  ASC");

                        }

                    }
                    var filtervalues =
                        tx.PersistenceManager.PlanningRepository.GetEquals<FilterSettingsDao>(
                            FilterSettingsDao.PropertyNames.FilterID, FilterID);

                    ///Filter query 
                    if (FilterID > 0 || filterSettingValues.Count() > 0)
                    {
                        //Create a list to hold all the valid EntityTypes

                        IList<FiltersettingsValuesDao> filterValResult = new List<FiltersettingsValuesDao>();
                        IList<FilterSettingsDao> filterObject = new List<FilterSettingsDao>();

                        //To get filter data
                        if (filterSettingValues != null && filterSettingValues.Count() > 0)
                        {
                            if (filterSettingValues.ElementAt(0).AttributeId != 0)
                            {
                                foreach (var objFlter in filterSettingValues)
                                {
                                    if (objFlter.AttributeId != 0)
                                    {
                                        FiltersettingsValuesDao setValDao = new FiltersettingsValuesDao();
                                        setValDao.AttributeId = objFlter.AttributeId;
                                        setValDao.AttributeTypeId = objFlter.AttributeTypeId;
                                        setValDao.Level = objFlter.Level;
                                        setValDao.Value = objFlter.Value;
                                        filterValResult.Add(setValDao);
                                    }
                                }
                            }
                            if (filterSettingValues.ElementAt(0).EntityTypeIDs.Length > 0)
                            {
                                filterObject.Add(new FilterSettingsDao
                                {
                                    EntityTypeID = filterSettingValues.ElementAt(0).EntityTypeIDs
                                });
                                IsEntityAvailable = true;
                            }
                        }
                        else
                        {
                            filterValResult =
                                tx.PersistenceManager.PlanningRepository.GetEquals<FiltersettingsValuesDao>(
                                    FiltersettingsValuesDao.PropertyNames.FilterId, FilterID);
                            filterObject =
                                tx.PersistenceManager.PlanningRepository.GetEquals<FilterSettingsDao>(
                                    FilterSettingsDao.PropertyNames.FilterID, FilterID);

                        }

                        //Entity Types looping to get valid entityTypes
                        for (int fi = 0; fi < listSetting.EntityTypes.Count; fi++)
                        {

                            for (int fj = 0; fj < filterValResult.ToList().Count; fj++)
                            {

                                var lstEntiTypeAttribute = from val in dao.ToList()
                                                           where
                                                               val.AttributeID == filterValResult[fj].AttributeId &&
                                                               val.EntityTypeID == listSetting.EntityTypes[fi]
                                                           select val;
                                if (listSetting.EntityTypes.Count() != 0)
                                {
                                    if (lstEntiTypeAttribute.ToList().Count <= 0)
                                    {
                                        newValidEntityTypes.Remove(listSetting.EntityTypes[fi]);
                                    }
                                }
                            }
                        }

                        if (filterObject.Count() > 0)
                        {
                            if (filterObject != null && filterObject.ElementAt(0).EntityTypeID.Trim().Length > 0)
                            {
                                IsEntityAvailable = true;
                                newValidEntityTypes = null;
                                newValidEntityTypes = new List<int>();
                                string[] EntitypesArr = filterObject.ElementAt(0).EntityTypeID.Split(',');
                                for (int fo = 0; fo < EntitypesArr.Length; fo++)
                                {
                                    newValidEntityTypes.Add(Convert.ToInt32(EntitypesArr[fo]));
                                }
                            }
                        }

                        //To fetch entitytypes related data
                        for (int ve = 0; ve < newValidEntityTypes.Count; ve++)
                        {
                            string entityType = newValidEntityTypes[ve].ToString();
                            ;
                            dynamicTblQry.Append(" SELECT id FROM MM_AttributeRecord_" + entityType +
                                                 "  WHERE  1=1 ");

                            for (int fj = 0; fj < filterValResult.ToList().Count; fj++)
                            {
                                int attributeID = filterValResult[fj].AttributeId;
                                int attributeTypeID = filterValResult[fj].AttributeTypeId;
                                var FilterValue = filterValResult[fj].Value;
                                var FilterLevel = filterValResult[fj].Level;
                                var lstEntiTypeAttribute = from val in dao.ToList()
                                                           where
                                                               val.AttributeID == attributeID &&
                                                               val.EntityTypeID == newValidEntityTypes[ve]
                                                           select val;
                                if (lstEntiTypeAttribute.ToList().Count > 0)
                                {
                                    if ((AttributesList)attributeTypeID != AttributesList.ListMultiSelection ||
                                        (AttributesList)attributeTypeID != AttributesList.DropDownTree ||
                                        (AttributesList)attributeTypeID != AttributesList.Tree ||
                                        (AttributesList)attributeTypeID != AttributesList.Period)
                                    {

                                    }
                                    else
                                    {
                                        dynamicTblQry.Append(" and  Attr_" + attributeID.ToString() + " IN (" +
                                                             FilterValue.ToString() + ") ");
                                    }
                                }
                            }
                            if ((ve < newValidEntityTypes.Count - 1))
                            {
                                dynamicTblQry.Append(" UNION ALL ");
                            }
                        }

                        //To fetch related entity type attribute like(multiselect,tree..etc)
                        bool multiSelect = false;
                        bool tree = false;
                        bool period = false;
                        bool singleSelect = false;
                        bool OwnerId = false;
                        bool multiselecttree = false;
                        for (int fj = 0; fj < filterValResult.ToList().Count; fj++)
                        {
                            int attributeID = filterValResult[fj].AttributeId;
                            var FilterValue = filterValResult[fj].Value;
                            var FilterLevel = filterValResult[fj].Level;

                            switch ((AttributesList)filterValResult[fj].AttributeTypeId)
                            {

                                case AttributesList.ListMultiSelection:
                                    if (multiSelect == false)
                                    {
                                        var valMultiSelect =
                                            filterValResult.Where(a => a.AttributeId == attributeID)
                                                .Select(a => a.Value)
                                                .ToList();
                                        var valMultiLevel =
                                            filterValResult.Where(a => a.AttributeId == attributeID)
                                                .Select(a => a.Level)
                                                .ToList();
                                        string inMultiClause = "("
                                                                   +
                                                                   String.Join(",",
                                                                       valMultiSelect.Select(x => x.ToString())
                                                                           .ToArray())
                                               + ")";
                                        if (attributeID == (int)SystemDefinedAttributes.ObjectiveType)
                                        {
                                            multiSelectTblQry.Append(
                                                " SELECT po.id FROM PM_Objective po  where po.typeid in" +
                                                inMultiClause +
                                                " INTERSECT ");
                                        }
                                        else
                                        {
                                            string inMultiLevel = "("
                                                                      +
                                                                      String.Join(",",
                                                                          valMultiLevel.Select(x => x.ToString())
                                                                              .ToArray())
                                               + ")";
                                            multiSelectTblQry.Append(
                                                " SELECT DISTINCT mms.EntityID as id FROM MM_MultiSelect mms ");
                                            multiSelectTblQry.Append(" WHERE (mms.AttributeID = " +
                                                                     attributeID.ToString() + " AND mms.OptionID IN" +
                                                                     inMultiClause + ") INTERSECT ");
                                        }
                                    }
                                    multiSelect = true;
                                    break;
                                case AttributesList.DropDownTree:
                                    if (tree == false)
                                    {
                                        var valTreeSelect =
                                            filterValResult.Where(a => a.AttributeId == attributeID)
                                                .Select(a => a.Value)
                                                .ToList();
                                        var valTreeLevel =
                                            filterValResult.Where(a => a.AttributeId == attributeID)
                                                .Select(a => a.Level)
                                                .ToList();
                                        string inTreeClause = "("
                                                                  +
                                                                  String.Join(",",
                                                                      valTreeSelect.Select(x => x.ToString()).ToArray())
                                            + ")";
                                        string inTreeLevel = "("
                                                                 +
                                                                 String.Join(",",
                                                                     valTreeLevel.Select(x => x.ToString()).ToArray())
                                           + ")";
                                        treeTblQry.Append(
                                            " SELECT DISTINCT mms.EntityID as id FROM MM_TreeValue mms ");
                                        treeTblQry.Append(" WHERE (mms.AttributeID = " + attributeID.ToString() +
                                                          " AND  mms.NodeID IN" + inTreeClause.ToString() +
                                                          " AND  mms.LEVEL IN" + inTreeLevel + " ) INTERSECT ");
                                    }
                                    tree = true;
                                    break;
                                case AttributesList.ListSingleSelection:
                                    var lstSpecialAttribute =
                                        tx.PersistenceManager.MetadataRepository.GetbyCriteria<AttributeDao>(
                                            AttributeDao.PropertyNames.Id, AttributeDao.PropertyNames.IsSpecial,
                                            attributeID, true);
                                    //var lstSpecialAttribute = listSetting.Attributes.Where(a => a.Id == attributeID && a.IsSpecial == true);
                                    var val =
                                        filterValResult.Where(a => a.AttributeId == attributeID)
                                            .Select(a => a.Value)
                                            .ToList();
                                    if (lstSpecialAttribute != null)
                                    {
                                        //var val = filterValResult.Where(a => a.AttributeId == attributeID).Select(a => a.Value).ToList();
                                        if ((SystemDefinedAttributes)attributeID ==
                                            SystemDefinedAttributes.EntityStatus)
                                        {
                                            string inClause = "("
                                                                  +
                                                                  String.Join(",",
                                                                      val.Select(x => x.ToString()).ToArray())
                                             + ")";
                                            singleSelectTblQry.Append(" SELECT EntityID as Id FROM MM_EntityStatus ");
                                            singleSelectTblQry.Append(" WHERE StatusID in " + inClause +
                                                                      "  INTERSECT ");
                                        }
                                        else
                                        {
                                            string inClause = "("
                                                                  +
                                                                  String.Join(",",
                                                                      val.Select(x => x.ToString()).ToArray())
                                               + ")";
                                            singleSelectTblQry.Append(
                                                " SELECT EntityID as Id FROM AM_Entity_Role_User ");
                                            singleSelectTblQry.Append(" WHERE  USERID in " + inClause +
                                                                      " and RoleID=1 INTERSECT ");
                                        }
                                        OwnerId = true;
                                    }
                                    else
                                    {
                                        string inClause = "("
                                                              +
                                                              String.Join(",", val.Select(x => x.ToString()).ToArray())
                                         + ")";
                                        singleSelectTblQry.Append(" SELECT DISTINCT singleSelect.Id ");
                                        singleSelectTblQry.Append(" FROM   MM_Option mo INNER JOIN( ");
                                        bool isItFirstTime = false;
                                        foreach (var lstEntitypes in newValidEntityTypes)
                                        {
                                            var lstEntiTypeAttribute = from entityResult in dao.ToList()
                                                                       where
                                                                           entityResult.AttributeID == attributeID &&
                                                                           entityResult.EntityTypeID == lstEntitypes
                                                                       select val;
                                            if (lstEntiTypeAttribute.Count() > 0)
                                            {
                                                if (isItFirstTime == true)
                                                {
                                                    singleSelectTblQry.Append(" union ");
                                                }
                                                singleSelectTblQry.Append(" SELECT mar_" + lstEntitypes + ".Attr_" +
                                                                          attributeID + " as  Attr_" + attributeID +
                                                                          ",mar_" + lstEntitypes +
                                                                          ".id as Id from  MM_AttributeRecord_" +
                                                                          lstEntitypes + "  mar_" + lstEntitypes +
                                                                          " ");
                                                isItFirstTime = true;
                                            }

                                        }
                                        singleSelectTblQry.Append(") singleSelect ON  singleSelect.Attr_" +
                                                                  attributeID +
                                                                  " = mo.id  WHERE  mo.id in " + inClause +
                                                                  " INTERSECT ");
                                        singleSelect = true;

                                    }

                                    break;
                                case AttributesList.Period:

                                    period = true;
                                    break;
                                case AttributesList.TreeMultiSelection:
                                    if (multiselecttree == false)
                                    {
                                        var valTreeSelect =
                                            filterValResult.Where(a => a.AttributeId == attributeID)
                                                .Select(a => a.Value)
                                                .ToList();
                                        var valTreeLevel =
                                            filterValResult.Where(a => a.AttributeId == attributeID)
                                                .Select(a => a.Level)
                                                .ToList();
                                        string inTreeClause = "("
                                                                  +
                                                                  String.Join(",",
                                                                      valTreeSelect.Select(x => x.ToString()).ToArray())
                                            + ")";
                                        string inTreeLevel = "("
                                                                 +
                                                                 String.Join(",",
                                                                     valTreeLevel.Select(x => x.ToString()).ToArray())
                                           + ")";
                                        treeTblQry.Append(
                                            " SELECT DISTINCT mms.EntityID as id FROM MM_TreeValue mms ");
                                        treeTblQry.Append(" WHERE (mms.AttributeID = " + attributeID.ToString() +
                                                          " AND  mms.NodeID IN" + inTreeClause.ToString() +
                                                          " AND  mms.LEVEL IN" + inTreeLevel + " ) INTERSECT ");
                                    }
                                    multiselecttree = true;
                                    break;
                                default:
                                    break;

                            }

                        }
                        if (filtervalues.Count() > 0)
                        {
                            if (filtervalues[0].StartDate != null &&
                                filtervalues[0].StartDate.ToString() != "1990-01-01" &&
                                filtervalues[0].StartDate != "")
                            {
                                singleSelectTblQry.Append(
                                    " SELECT DISTINCT pep.EntityID AS ID FROM   PM_EntityPeriod pep where 1=1 ");
                                if (filtervalues[0].StartDate.ToString().Length > 0)
                                    singleSelectTblQry.Append(
                                        " AND  convert(VARCHAR(10),pep.Startdate,111)  >= convert(VARCHAR(10),'" +
                                        filtervalues[0].StartDate.ToString() + "',111) ");
                                if (filtervalues[0].EndDate.ToString().Length > 0)
                                    singleSelectTblQry.Append(
                                        " AND convert(VARCHAR(10),pep.EndDate,111)  <= convert(VARCHAR(10),'" +
                                        filtervalues[0].EndDate.ToString() + "',111) ");
                                singleSelectTblQry.Append(" INTERSECT ");
                            }
                        }
                        if (filterSettingValues.Count() > 0)
                        {
                            if ((filterSettingValues[0].StartDate != "" || filterSettingValues[0].EndDate != "") &&
                                (filterSettingValues[0].StartDate != null || filterSettingValues[0].EndDate != null))
                            {
                                singleSelectTblQry.Append(
                                    " SELECT DISTINCT pep.EntityID AS ID FROM   PM_EntityPeriod pep where 1=1 ");
                                if (filterSettingValues[0].StartDate.ToString().Length > 0)
                                    singleSelectTblQry.Append(
                                        " AND  convert(VARCHAR(10),pep.Startdate,111)  >= convert(VARCHAR(10),'" +
                                        filterSettingValues[0].StartDate.ToString() + "',111) ");
                                if (filterSettingValues[0].EndDate.ToString().Length > 0)
                                    singleSelectTblQry.Append(
                                        " AND convert(VARCHAR(10),pep.EndDate,111)  <= convert(VARCHAR(10),'" +
                                        filterSettingValues[0].EndDate.ToString() + "',111) ");
                                singleSelectTblQry.Append(" INTERSECT ");
                            }
                        }
                        switch ((EntityTypeIDs)enumEntityTypeIds)
                        {
                            case EntityTypeIDs.Activity:
                                TempTblQry.Append(" SELECT temptable.Id,pe.Level,pe.parentid  ");
                                break;
                            case EntityTypeIDs.Costcenre:
                                TempTblQry.Append(" SELECT temptable.Id,coi.CostCenterID,pe.Level,pe.parentid  ");
                                break;
                            case EntityTypeIDs.Objective:
                                TempTblQry.Append(" SELECT temptable.Id,oi.ObjectveID,pe.Level,pe.parentid  ");
                                break;
                        }

                        TempTblQry.Append(" FROM   ( ");
                        if (multiSelectTblQry.ToString().Length > 0)
                            TempTblQry.Append(multiSelectTblQry.ToString());
                        if (treeTblQry.ToString().Length > 0)
                            TempTblQry.Append(treeTblQry.ToString());
                        if (periodTblQry.ToString().Length > 0)
                            TempTblQry.Append(periodTblQry.ToString());
                        if (singleSelectTblQry.ToString().Length > 0)
                            TempTblQry.Append(singleSelectTblQry.ToString());
                        TempTblQry.Append("select DynamicEntittype.id from (" + dynamicTblQry.ToString() +
                                          ") DynamicEntittype ");
                        TempTblQry.Append(" ) temptable ");

                    }

                    else
                    {
                        //Entity Types looping
                        dynamicTblQry.Append(" SELECT id,Level,parentid FROM PM_Entity ");
                        if (listSetting.EntityTypes != null)
                        {
                            string inClause = "("
                                                  +
                                                  String.Join(",",
                                                      listSetting.EntityTypes.Select(x => x.ToString()).ToArray())
                                          + ")";
                            dynamicTblQry.Append(" where TypeID in " + inClause);
                        }
                        switch ((EntityTypeIDs)enumEntityTypeIds)
                        {
                            case EntityTypeIDs.Activity:
                                TempTblQry.Append(" SELECT temptable.Id,temptable.Level,temptable.parentid ");
                                break;
                            case EntityTypeIDs.Costcenre:
                                TempTblQry.Append(" SELECT temptable.Id,coi.CostCenterID,temptable.Level,temptable.parentid ");
                                break;
                            case EntityTypeIDs.Objective:
                                TempTblQry.Append(" SELECT temptable.Id,oi.ObjectveID,temptable.Level,temptable.parentid ");
                                break;
                        }


                        TempTblQry.Append(" FROM   ( ");

                        TempTblQry.Append(dynamicTblQry.ToString());
                        TempTblQry.Append(" ) temptable ");



                    }

                    if (EntityTypeIDs.Activity == (EntityTypeIDs)enumEntityTypeIds)
                    {
                        TempTblQry.Append(" INNER JOIN PM_Entity pe ");
                        TempTblQry.Append(" ON  temptable.Id = pe.ID");
                        TempTblQry.Append(" AND pe.Active = 1  ");
                        TempTblQry.Append("   INNER JOIN PM_Entity_Sort pes ");
                        TempTblQry.Append("             ON  pe.ID = pes.id ");

                        if (filtervalues.Count() > 0)
                        {
                            if (filtervalues[0].Keyword.ToString() != "")
                            {
                                TempTblQry.Append("  and pe.Name LIKE '%" + filtervalues[0].Keyword.ToString() + "%'");
                            }
                        }
                        else if (filterSettingValues.Count() > 0)
                        {
                            if (filterSettingValues.ElementAt(0).Keyword != "")
                            {
                                TempTblQry.Append("  and pe.Name LIKE '%" + filterSettingValues.ElementAt(0).Keyword.ToString() + "%'");
                            }
                        }



                        if (UserID != 0)
                        {
                            TempTblQry.Append(" INNER JOIN (SELECT DISTINCT aeru.EntityID,aeru.UserID ");
                            TempTblQry.Append("  FROM   AM_Entity_Role_User aeru where aeru.UserID=" +
                                              UserID.ToString());
                            TempTblQry.Append(" ) AS aeru ON  aeru.EntityID = temptable.id ");
                        }

                        if (GanttstartDate != null && Ganttenddate != null && GanttstartDate.Length > 0 && Ganttenddate.Length > 0)
                        {

                            TempTblQry.Append(" INNER JOIN [PM_EntityPeriod] pep   ON  pep.EntityID =pe.id  ");
                            TempTblQry.Append("  and  pep.Startdate >= '" + GanttstartDate + "'  ");
                            TempTblQry.Append("  AND   '" + Ganttenddate + "' >=  pep.EndDate    ");
                        }


                        topFilterQry.Append(TempTblQry.ToString());
                        StringBuilder SortorderQry = new StringBuilder();
                        if (UniqueKeyCollection != null && UniqueKeyCollection.Count() > 0)
                        {
                            if (IncludeChildren == true)
                            {
                                string inClause = "";
                                inClause = "("
                                                       + String.Join(",", IdArr.Select(x => x.ToString()).ToArray())
                                                     + ")";
                                SortorderQry.Append(" where (temptable.Id in " + inClause);
                                if (IsSingleID == false)
                                {
                                    foreach (var UniquekeyVal in UniqueKeyCollection.ToList())
                                    {
                                        SortorderQry.Append(" or pe.UniqueKey  like '" + UniquekeyVal.UniqueKey +
                                                            ".%'  ");
                                    }
                                    SortorderQry.Append(")");

                                }
                                else
                                {
                                    SortorderQry.Append(")");
                                }
                            }
                            else
                            {
                                if (FilterID == 0 && filterSettingValues.Count() == 0 && IdArr.Length == 1 && ExpandingEntityID > 0)
                                {
                                    SortorderQry.Append(" where temptable.Id =" + ExpandingEntityID);
                                }
                                else if (FilterID == 0 && filterSettingValues.Count() == 0)
                                {
                                    SortorderQry.Append(" where pe.ParentID =" + IdArr[0]);
                                }
                                else
                                {
                                    string inClause = "";
                                    inClause = "("
                                                           + String.Join(",", IdArr.Select(x => x.ToString()).ToArray())
                                                         + ")";
                                    SortorderQry.Append(" where (temptable.Id in " + inClause);
                                    foreach (var UniquekeyVal in UniqueKeyCollection.ToList())
                                    {
                                        SortorderQry.Append(" or pe.UniqueKey  like '" + UniquekeyVal.UniqueKey +
                                                            ".%' ");
                                    }
                                    SortorderQry.Append(")");
                                }
                            }
                        }


                        SortorderQry.Append("   ORDER BY ");
                        SortorderQry.Append("        pes.S1   ASC, ");
                        SortorderQry.Append("        pes.L1   ASC, ");
                        SortorderQry.Append("        pes.S2   ASC, ");
                        SortorderQry.Append("        pes.L2   ASC, ");
                        SortorderQry.Append("        pes.S3   ASC, ");
                        SortorderQry.Append("        pes.L3   ASC, ");
                        SortorderQry.Append("        pes.S4   ASC, ");
                        SortorderQry.Append("        pes.L4   ASC, ");
                        SortorderQry.Append("        pes.S5   ASC, ");
                        SortorderQry.Append("        pes.L5   ASC, ");
                        SortorderQry.Append("        pes.S6   ASC, ");
                        SortorderQry.Append("        pes.L6   ASC, ");
                        SortorderQry.Append("        pes.S7   ASC, ");
                        SortorderQry.Append("        pes.L7   ASC, ");
                        SortorderQry.Append("        pes.S8   ASC, ");
                        SortorderQry.Append("        pes.L8   ASC, ");
                        SortorderQry.Append("        pes.S9   ASC, ");
                        SortorderQry.Append("        pes.L9   ASC, ");
                        SortorderQry.Append("        pes.S10  ASC, ");
                        SortorderQry.Append("        pes.L10  ASC ");


                        topFilterQry.Append(SortorderQry.ToString());


                    }
                    else if (EntityTypeIDs.Costcenre == (EntityTypeIDs)enumEntityTypeIds)
                    {

                        //finalQry.Append(Costcent.ToString());
                        StringBuilder countQry = new StringBuilder();
                        countQry.Append(TempTblQry.ToString());
                        countQry.Append(" INNER JOIN  @CostCentreOrderIDs coi ");
                        countQry.Append("  ON  coi.EID = temptable.Id  ");

                        if (GanttstartDate != null && Ganttenddate != null && GanttstartDate.Length > 0 && Ganttenddate.Length > 0)
                        {

                            countQry.Append(" INNER JOIN [PM_EntityPeriod] pep   ON  pep.EntityID =temptable.id  ");
                            countQry.Append("  and  pep.Startdate >= '" + GanttstartDate + "'  ");
                            countQry.Append("  AND   '" + Ganttenddate + "' >=  pep.EndDate    ");
                        }

                        if (filtervalues.Count() > 0)
                        {
                            if (filtervalues[0].Keyword.ToString() != "")
                            {
                                countQry.Append(" inner join pm_entity pe on  temptable.Id=pe.id   ");
                                countQry.Append("  and pe.Name LIKE '%" + filtervalues[0].Keyword.ToString() + "%'");
                            }
                        }
                        else if (filterSettingValues.Count() > 0)
                        {
                            if (filterSettingValues.ElementAt(0).Keyword != "")
                            {
                                countQry.Append(" inner join pm_entity pe on  temptable.Id=pe.id   ");
                                countQry.Append("  and pe.Name LIKE '%" + filterSettingValues.ElementAt(0).Keyword.ToString() + "%'");
                            }
                        }


                        topFilterQry.Append(" INSERT INTO @EntityOrderIDs ");
                        topFilterQry.Append(" ( ");
                        topFilterQry.Append("  EID,CostCenterID,LEVEL,PEID ");
                        topFilterQry.Append(" ) ");

                        topFilterQry.Append(countQry.ToString());



                    }
                    else if (EntityTypeIDs.Objective == (EntityTypeIDs)enumEntityTypeIds)
                    {
                        finalQry.Append(Objective.ToString());
                        StringBuilder countQry = new StringBuilder();
                        countQry.Append(TempTblQry.ToString());
                        countQry.Append(" INNER JOIN  @ObjectiveOrderIDs oi ");
                        countQry.Append("  ON  oi.EID = temptable.Id  ");

                        if (GanttstartDate != null && Ganttenddate != null && GanttstartDate.Length > 0 && Ganttenddate.Length > 0)
                        {
                            countQry.Append(" INNER JOIN [PM_EntityPeriod] pep   ON  pep.EntityID =temptable.id  ");
                            countQry.Append("  and  pep.Startdate >= '" + GanttstartDate + "'  ");
                            countQry.Append("  AND   '" + Ganttenddate + "' >=  pep.EndDate    ");
                        }

                        if (filtervalues.Count() > 0)
                        {
                            if (filtervalues[0].Keyword.ToString() != "")
                            {
                                countQry.Append(" inner join pm_entity pe on  temptable.Id=pe.id   ");
                                countQry.Append("  and pe.Name LIKE '%" + filtervalues[0].Keyword.ToString() + "%'");
                            }
                        }
                        else if (filterSettingValues.Count() > 0)
                        {
                            if (filterSettingValues.ElementAt(0).Keyword != "")
                            {
                                countQry.Append(" inner join pm_entity pe on  temptable.Id=pe.id   ");
                                countQry.Append("  and pe.Name LIKE '%" + filterSettingValues.ElementAt(0).Keyword.ToString() + "%'");
                            }
                        }




                        topFilterQry.Append(" INSERT INTO @EntityOrderIDs ");
                        topFilterQry.Append(" ( ");
                        topFilterQry.Append("  EID,ObjectveID,LEVEL,PEID ");
                        topFilterQry.Append(" ) ");



                        topFilterQry.Append(countQry.ToString());


                    }



                    mainTblQry.Append(topFilterQry.ToString());



                    ///Main Query 
                    //To build inner query for main query
                    for (int i = 0; i < listSetting.EntityTypes.Count; i++)
                    {
                        strqry.Append("SELECT id");
                        for (int j = 0; j < listSetting.Attributes.Count; j++)
                        {
                            //var x = tx.PersistenceManager.MetadataRepository.Query<EntityTypeAttributeRelationDao>().Where(a => a.EntityTypeID == listSetting.EntityTypes[i] && a.AttributeID == listSetting.Attributes[j].Id).Select(a => a);

                            var x = from val in dao.ToList() where val.AttributeID == listSetting.Attributes[j].Id && val.EntityTypeID == listSetting.EntityTypes[i] select val;
                            string CurrentattrID = listSetting.Attributes[j].Id.ToString();
                            if (x.ToList().Count > 0)
                            {
                                if (!((AttributesList)listSetting.Attributes[j].Type == AttributesList.ListMultiSelection || (AttributesList)listSetting.Attributes[j].Type == AttributesList.DropDownTree || (AttributesList)listSetting.Attributes[j].Type == AttributesList.Tree || (AttributesList)listSetting.Attributes[j].Type == AttributesList.Period || (AttributesList)listSetting.Attributes[j].Type == AttributesList.TreeMultiSelection || listSetting.Attributes[j].IsSpecial == true))
                                {
                                    strqry.Append(" ,attr_" + CurrentattrID);

                                }
                            }
                            else
                            {
                                if (!((AttributesList)listSetting.Attributes[j].Type == AttributesList.ListMultiSelection || (AttributesList)listSetting.Attributes[j].Type == AttributesList.DropDownTree || (AttributesList)listSetting.Attributes[j].Type == AttributesList.Tree || (AttributesList)listSetting.Attributes[j].Type == AttributesList.Period || (AttributesList)listSetting.Attributes[j].Type == AttributesList.TreeMultiSelection || listSetting.Attributes[j].IsSpecial == true))
                                {
                                    strqry.Append(",null as attr_" + listSetting.Attributes[j].Field + " ");

                                }
                            }

                        }


                        strqry.Append("  FROM MM_AttributeRecord_" + listSetting.EntityTypes[i]);

                        if (i < listSetting.EntityTypes.Count - 1)
                        {
                            strqry.Append(" UNION ALL ");
                        }
                    }


                    //To fetch attributes and special sttribute values for main query

                    mainTblQry.Append("SELECT  subtbl.id as Id,");
                    mainTblQry.Append(" pe.ParentID, pe.TypeID, pe.UniqueKey, pe.IsLock, pe.Name, ");
                    mainTblQry.Append(" pe.EntityStateID, pe.EntityID,met.ColorCode,met.ShortDescription,pe.Level,(SELECT met.Caption FROM MM_EntityType met WHERE id=pe.typeid) as 'TypeName' ");
                    if (IsobjectiveRootLevel == false)
                    {
                        if (EntityTypeIDs.Activity == (EntityTypeIDs)enumEntityTypeIds)
                        {
                            if (IsEntityAvailable == true)
                            {
                                mainTblQry.Append(" ,0 AS TotalChildrenCount");
                            }
                            else
                            {

                                mainTblQry.Append(" ,isnull((SELECT COUNT(pe1.ParentID) ");
                                mainTblQry.Append(" FROM   PM_Entity pe1 ");
                                //if (FilterID != 0 || filterSettingValues.Count > 0)
                                //{
                                //    mainTblQry.Append("  INNER JOIN @EntityOrderIDs eoi ON  eoi.EID = pe1.Id ");
                                //}
                                mainTblQry.Append(" WHERE  pe1.ParentID = pe.Id ");
                                mainTblQry.Append(" AND pe1.[Active]=1 ");
                                if (listSetting.EntityTypes != null)
                                {
                                    string inEntiTypes = "("
                                                   + String.Join(",", listSetting.EntityTypes.Select(x => x.ToString()).ToArray())
                                                 + ")";
                                    mainTblQry.Append(" AND TypeID  IN " + inEntiTypes + " ");
                                }
                                mainTblQry.Append(" GROUP BY ");
                                mainTblQry.Append(" pe1.ParentID ");
                                mainTblQry.Append(" ),0) AS TotalChildrenCount");
                            }
                        }
                        else if (EntityTypeIDs.Costcenre == (EntityTypeIDs)enumEntityTypeIds)
                        {

                            ArrayList financialDisplayColumn = new ArrayList();
                            financialDisplayColumn.Add("Status");
                            financialDisplayColumn.Add("Planned");
                            financialDisplayColumn.Add("In requests");
                            financialDisplayColumn.Add("Appr/Alloc");
                            financialDisplayColumn.Add("Approved budget");
                            financialDisplayColumn.Add("Budget deviation");
                            financialDisplayColumn.Add("Appr sub allocation");
                            financialDisplayColumn.Add("Committed");
                            financialDisplayColumn.Add("Spent");
                            financialDisplayColumn.Add("Available");
                            ArrayList financialColumn = new ArrayList();
                            financialColumn.Add("Status");
                            financialColumn.Add("PlannedAmount");
                            financialColumn.Add("InRequest");
                            financialColumn.Add("ApprovedAllocatedAmount");
                            financialColumn.Add("ApprovedBudget");
                            financialColumn.Add("BudgetDeviation");
                            financialColumn.Add("ApprovedSubAllocatedAmount");
                            financialColumn.Add("Commited");
                            financialColumn.Add("Spent");
                            financialColumn.Add("Available");
                            Tuple<ArrayList, ArrayList> finColumn = Tuple.Create(financialColumn, financialDisplayColumn);
                            lstrecord.GeneralColumnDefs = finColumn;
                            mainTblQry.Append(" ,eoi.CostCenterID as CostCenterID");
                            mainTblQry.Append(" ,CASE WHEN eoi.CostCenterID !=0 then cast(eoi.CostCenterID AS VARCHAR) + '.' + pe.UniqueKey ELSE pe.UniqueKey end AS class ");
                            if (IsEntityAvailable == true)
                            {
                                mainTblQry.Append(" ,0 AS TotalChildrenCount,");
                            }
                            else
                            {
                                string inEntiTypes = "";
                                if (listSetting.EntityTypes != null)
                                {
                                    inEntiTypes = "("
                                                  + String.Join(",", listSetting.EntityTypes.Select(x => x.ToString()).ToArray())
                                                + ")";

                                }
                                mainTblQry.Append(" ,ISNULL(CASE when pe.TypeID=5 THEN (SELECT COUNT(1) FROM   PM_EntityCostReleations pecr INNER JOIN PM_Entity pe2 ");
                                mainTblQry.Append(" ON  pecr.EntityID = pe2.ID AND pe2.[Active]=1 AND pecr.IsActive=1 WHERE pecr.CostCenterID=pe.id ");
                                if (inEntiTypes.Length > 0)
                                {
                                    mainTblQry.Append(" and pe2.TypeID in" + inEntiTypes + " ");
                                }
                                mainTblQry.Append(") else (SELECT COUNT(1) FROM PM_EntityCostReleations pecr INNER JOIN PM_Entity pe2 ON pecr.EntityID=pe2.ID ");
                                mainTblQry.Append(" AND pe2.[Active]=1 AND pecr.IsActive=1   AND pe2.ParentID=pe.id and pecr.CostCenterID=eoi.CostCenterID ");
                                if (inEntiTypes.Length > 0)
                                {
                                    mainTblQry.Append(" and pe2.TypeID in" + inEntiTypes + "  ");
                                }
                                mainTblQry.Append(") end,0)   AS TotalChildrenCount, ");
                            }

                            mainTblQry.Append("CASE ");
                            mainTblQry.Append("            WHEN pe.TypeID = 5 THEN (");
                            mainTblQry.Append("                     SELECT ISNULL(SUM(pf.RequestedAmount), 0)");
                            mainTblQry.Append("                     FROM   PM_Financial pf");
                            mainTblQry.Append("                            INNER JOIN pm_Entity children");
                            mainTblQry.Append("                                 ON  pf.EntityID = children.ID");
                            mainTblQry.Append("                                 AND children.[Active] = 1");
                            mainTblQry.Append("                                 AND children.[level] = 1");
                            mainTblQry.Append("                     WHERE  pf.CostCenterID = subtbl.ID");
                            mainTblQry.Append("                 )");
                            mainTblQry.Append("            WHEN pe.[level] = 0 THEN (");
                            mainTblQry.Append("                     SELECT ISNULL(SUM(pf.RequestedAmount), 0)");
                            mainTblQry.Append("                     FROM   PM_Financial pf");
                            mainTblQry.Append("                            INNER JOIN pm_Entity children");
                            mainTblQry.Append("                                 ON  pf.EntityID = children.ID");
                            mainTblQry.Append("                                 AND children.[Active] = 1");
                            mainTblQry.Append("                     WHERE  children.ParentID = subtbl.ID");
                            mainTblQry.Append("                            AND pf.CostCenterID = eoi.CostCenterID");
                            mainTblQry.Append("                 )");
                            mainTblQry.Append("            ELSE (");
                            mainTblQry.Append("                     SELECT TOP 1 pf.RequestedAmount");
                            mainTblQry.Append("                     FROM   PM_Financial pf");
                            mainTblQry.Append("                     WHERE  pf.EntityID = subtbl.ID");
                            mainTblQry.Append("                            AND pf.CostCenterID = eoi.CostCenterID");
                            mainTblQry.Append("                 )");
                            mainTblQry.Append("       END        AS InRequest,");
                            mainTblQry.Append("       CASE ");
                            mainTblQry.Append("            WHEN pe.TypeID = 5 THEN (");
                            mainTblQry.Append("                     SELECT ISNULL(SUM(pf.PlannedAmount), 0)");
                            mainTblQry.Append("                     FROM   PM_Financial pf");
                            mainTblQry.Append("                            INNER JOIN pm_Entity children");
                            mainTblQry.Append("                                 ON  pf.EntityID = children.ID");
                            mainTblQry.Append("                                 AND children.[Active] = 1");
                            mainTblQry.Append("                                 AND children.[level] = 1");
                            mainTblQry.Append("                     WHERE  pf.CostCenterID = subtbl.ID");
                            mainTblQry.Append("                 )");
                            mainTblQry.Append("            WHEN pe.[level] = 0 THEN (");
                            mainTblQry.Append("                     SELECT ISNULL(SUM(pf.PlannedAmount), 0)");
                            mainTblQry.Append("                     FROM   PM_Financial pf");
                            mainTblQry.Append("                            INNER JOIN pm_Entity children");
                            mainTblQry.Append("                                 ON  pf.EntityID = children.ID");
                            mainTblQry.Append("                                 AND children.[Active] = 1");
                            mainTblQry.Append("                     WHERE  children.ParentID = subtbl.ID");
                            mainTblQry.Append("                            AND pf.CostCenterID = eoi.CostCenterID");
                            mainTblQry.Append("                 )");
                            mainTblQry.Append("            ELSE (");
                            mainTblQry.Append("                     SELECT pf.PlannedAmount");
                            mainTblQry.Append("                     FROM   PM_Financial pf");
                            mainTblQry.Append("                     WHERE  pf.EntityID = subtbl.ID");
                            mainTblQry.Append("                            AND pf.CostCenterID = eoi.CostCenterID");
                            mainTblQry.Append("                 )");
                            mainTblQry.Append("       END        AS PlannedAmount,");
                            mainTblQry.Append("       CASE ");
                            mainTblQry.Append("            WHEN pe.TypeID = 5 THEN (");
                            mainTblQry.Append("                     SELECT ISNULL(SUM(pf.ApprovedAllocatedAmount), 0)");
                            mainTblQry.Append("                     FROM   PM_Financial pf");
                            mainTblQry.Append("                            INNER JOIN pm_Entity children");
                            mainTblQry.Append("                                 ON  pf.EntityID = children.ID");
                            mainTblQry.Append("                                 AND children.[Active] = 1");
                            mainTblQry.Append("                                 AND children.[level] = 1");
                            mainTblQry.Append("                     WHERE  pf.CostCenterID = subtbl.ID");
                            mainTblQry.Append("                 )");
                            mainTblQry.Append("            WHEN pe.[level] = 0 THEN (");
                            mainTblQry.Append("                     SELECT ISNULL(SUM(pf.ApprovedAllocatedAmount), 0)");
                            mainTblQry.Append("                     FROM   PM_Financial pf");
                            mainTblQry.Append("                            INNER JOIN pm_Entity children");
                            mainTblQry.Append("                                 ON  pf.EntityID = children.ID");
                            mainTblQry.Append("                                 AND children.[Active] = 1");
                            mainTblQry.Append("                     WHERE  children.ParentID = subtbl.ID");
                            mainTblQry.Append("                            AND pf.CostCenterID = eoi.CostCenterID");
                            mainTblQry.Append("                 )");
                            mainTblQry.Append("            ELSE (");
                            mainTblQry.Append("                     SELECT pf.ApprovedAllocatedAmount");
                            mainTblQry.Append("                     FROM   PM_Financial pf");
                            mainTblQry.Append("                     WHERE  pf.EntityID = subtbl.ID");
                            mainTblQry.Append("                            AND pf.CostCenterID = eoi.CostCenterID");
                            mainTblQry.Append("                 )");
                            mainTblQry.Append("       END        AS ApprovedAllocatedAmount,");
                            mainTblQry.Append("       CASE ");
                            mainTblQry.Append("            WHEN pe.TypeID = 5 THEN (");
                            mainTblQry.Append("                     SELECT ISNULL(SUM(pf.ApprovedAllocatedAmount), 0)");
                            mainTblQry.Append("                     FROM   PM_Financial pf");
                            mainTblQry.Append("                            INNER JOIN pm_Entity children");
                            mainTblQry.Append("                                 ON  pf.EntityID = children.ID");
                            mainTblQry.Append("                                 AND children.[Active] = 1");
                            mainTblQry.Append("                                 AND children.[level] = 1");
                            mainTblQry.Append("                     WHERE  pf.CostCenterID = subtbl.ID");
                            mainTblQry.Append("                 )");
                            mainTblQry.Append("            WHEN pe.[level] = 0 THEN (");
                            mainTblQry.Append("                     SELECT ISNULL(SUM(pf.ApprovedAllocatedAmount), 0)");
                            mainTblQry.Append("                     FROM   PM_Financial pf");
                            mainTblQry.Append("                            INNER JOIN pm_Entity children");
                            mainTblQry.Append("                                 ON  pf.EntityID = children.ID");
                            mainTblQry.Append("                                 AND children.[Active] = 1");
                            mainTblQry.Append("                     WHERE  children.ParentID = subtbl.ID");
                            mainTblQry.Append("                            AND pf.CostCenterID = eoi.CostCenterID");
                            mainTblQry.Append("                 )");
                            mainTblQry.Append("            ELSE (");
                            mainTblQry.Append("                     SELECT ISNULL(SUM(pf.ApprovedAllocatedAmount), 0)");
                            mainTblQry.Append("                     FROM   PM_Financial pf");
                            mainTblQry.Append("                            INNER JOIN pm_Entity children");
                            mainTblQry.Append("                                 ON  pf.EntityID = children.ID");
                            mainTblQry.Append("                                 AND children.[Active] = 1");
                            mainTblQry.Append("                     WHERE  children.ParentID = subtbl.ID");
                            mainTblQry.Append("                            AND pf.CostCenterID = eoi.CostCenterID");
                            mainTblQry.Append("                 )");
                            mainTblQry.Append("       END        AS ApprovedSubAllocatedAmount,");
                            mainTblQry.Append("       CASE ");
                            mainTblQry.Append("            WHEN pe.TypeID = 5 THEN (");
                            mainTblQry.Append("                     SELECT ISNULL(SUM(pf.ApprovedBudget), 0)");
                            mainTblQry.Append("                     FROM   PM_Financial pf");
                            mainTblQry.Append("                            INNER JOIN pm_Entity children");
                            mainTblQry.Append("                                 ON  pf.EntityID = children.ID");
                            mainTblQry.Append("                                 AND children.[Active] = 1");
                            mainTblQry.Append("                                 AND children.[level] = 1");
                            mainTblQry.Append("                     WHERE  pf.CostCenterID = subtbl.ID");
                            mainTblQry.Append("                 )");
                            mainTblQry.Append("            WHEN pe.[level] = 0 THEN (");
                            mainTblQry.Append("                     SELECT ISNULL(SUM(pf.ApprovedBudget), 0)");
                            mainTblQry.Append("                     FROM   PM_Financial pf");
                            mainTblQry.Append("                            INNER JOIN pm_Entity children");
                            mainTblQry.Append("                                 ON  pf.EntityID = children.ID");
                            mainTblQry.Append("                                 AND children.[Active] = 1");
                            mainTblQry.Append("                     WHERE  children.ParentID = subtbl.ID");
                            mainTblQry.Append("                            AND pf.CostCenterID = eoi.CostCenterID");
                            mainTblQry.Append("                 )");
                            mainTblQry.Append("            ELSE (");
                            mainTblQry.Append("                     SELECT pf.ApprovedBudget");
                            mainTblQry.Append("                     FROM   PM_Financial pf");
                            mainTblQry.Append("                     WHERE  pf.EntityID = subtbl.ID");
                            mainTblQry.Append("                            AND pf.CostCenterID = eoi.CostCenterID");
                            mainTblQry.Append("                 )");
                            mainTblQry.Append("       END        AS ApprovedBudget,");
                            mainTblQry.Append("       CASE ");
                            mainTblQry.Append("            WHEN pe.TypeID = 5 THEN (");
                            mainTblQry.Append("                     SELECT ISNULL(SUM(pf.Commited), 0)");
                            mainTblQry.Append("                     FROM   PM_Financial pf");
                            mainTblQry.Append("                            INNER JOIN pm_Entity children");
                            mainTblQry.Append("                                 ON  pf.EntityID = children.ID");
                            mainTblQry.Append("                                 AND children.[Active] = 1");
                            mainTblQry.Append("                     WHERE  pf.CostCenterID = subtbl.ID");
                            mainTblQry.Append("                 )");
                            mainTblQry.Append("            WHEN pe.[level] = 0 THEN (");
                            mainTblQry.Append("                     SELECT ISNULL(SUM(pf.Commited), 0)");
                            mainTblQry.Append("                     FROM   PM_Financial pf");
                            mainTblQry.Append("                            INNER JOIN pm_Entity children");
                            mainTblQry.Append("                                 ON  pf.EntityID = children.ID");
                            mainTblQry.Append("                                 AND children.[Active] = 1");
                            mainTblQry.Append("                     WHERE  children.UniqueKey LIKE(");
                            mainTblQry.Append("                                SELECT pe.UniqueKey");
                            mainTblQry.Append("                                FROM   PM_Entity pe");
                            mainTblQry.Append("                                WHERE  pe.ID = subtbl.ID");
                            mainTblQry.Append("                            )");
                            mainTblQry.Append("                            AND pf.CostCenterID = eoi.CostCenterID");
                            mainTblQry.Append("                 )");
                            mainTblQry.Append("            ELSE (");
                            mainTblQry.Append("                     SELECT ISNULL(SUM(pf.Commited), 0)");
                            mainTblQry.Append("                     FROM   PM_Financial pf");
                            mainTblQry.Append("                            INNER JOIN pm_Entity children");
                            mainTblQry.Append("                                 ON  pf.EntityID = children.ID");
                            mainTblQry.Append("                                 AND children.[Active] = 1");
                            mainTblQry.Append("                     WHERE  children.ParentID = subtbl.ID");
                            mainTblQry.Append("                            AND pf.CostCenterID = eoi.CostCenterID");
                            mainTblQry.Append("                 )");
                            mainTblQry.Append("       END        AS Commited,");
                            mainTblQry.Append("       CASE ");
                            mainTblQry.Append("            WHEN pe.TypeID = 5 THEN (");
                            mainTblQry.Append("                     SELECT ISNULL(SUM(pf.Spent), 0)");
                            mainTblQry.Append("                     FROM   PM_Financial pf");
                            mainTblQry.Append("                            INNER JOIN pm_Entity children");
                            mainTblQry.Append("                                 ON  pf.EntityID = children.ID");
                            mainTblQry.Append("                                 AND children.[Active] = 1");
                            mainTblQry.Append("                     WHERE  pf.CostCenterID = subtbl.ID");
                            mainTblQry.Append("                 )");
                            mainTblQry.Append("            WHEN pe.[level] = 0 THEN (");
                            mainTblQry.Append("                     SELECT ISNULL(SUM(pf.Spent), 0)");
                            mainTblQry.Append("                     FROM   PM_Financial pf");
                            mainTblQry.Append("                            INNER JOIN pm_Entity children");
                            mainTblQry.Append("                                 ON  pf.EntityID = children.ID");
                            mainTblQry.Append("                                 AND children.[Active] = 1");
                            mainTblQry.Append("                     WHERE  children.UniqueKey LIKE(");
                            mainTblQry.Append("                                SELECT pe.UniqueKey");
                            mainTblQry.Append("                                FROM   PM_Entity pe");
                            mainTblQry.Append("                                WHERE  pe.ID = subtbl.ID");
                            mainTblQry.Append("                            )");
                            mainTblQry.Append("                            AND pf.CostCenterID = eoi.CostCenterID");
                            mainTblQry.Append("                 )");
                            mainTblQry.Append("            ELSE (");
                            mainTblQry.Append("                     SELECT ISNULL(SUM(pf.Spent), 0)");
                            mainTblQry.Append("                     FROM   PM_Financial pf");
                            mainTblQry.Append("                            INNER JOIN pm_Entity children");
                            mainTblQry.Append("                                 ON  pf.EntityID = children.ID");
                            mainTblQry.Append("                                 AND children.[Active] = 1");
                            mainTblQry.Append("                     WHERE  children.ParentID = subtbl.ID");
                            mainTblQry.Append("                            AND pf.CostCenterID = eoi.CostCenterID");
                            mainTblQry.Append("                 )");
                            mainTblQry.Append("       END        AS Spent,");
                            mainTblQry.Append("       CASE ");
                            mainTblQry.Append("            WHEN pe.TypeID = 5 THEN (");
                            mainTblQry.Append("                     SELECT ISNULL(SUM(pf.ApprovedAllocatedAmount), 0) - ISNULL(SUM(pf.ApprovedBudget), 0)");
                            mainTblQry.Append("                     FROM   PM_Financial pf");
                            mainTblQry.Append("                            INNER JOIN pm_Entity children");
                            mainTblQry.Append("                                 ON  pf.EntityID = children.ID");
                            mainTblQry.Append("                                 AND children.[Active] = 1");
                            mainTblQry.Append("                                 AND children.[level] = 1");
                            mainTblQry.Append("                     WHERE  pf.CostCenterID = subtbl.ID");
                            mainTblQry.Append("                 )");
                            mainTblQry.Append("            WHEN pe.[level] = 0 THEN (");
                            mainTblQry.Append("                     SELECT ISNULL(SUM(pf.ApprovedAllocatedAmount), 0) - ISNULL(SUM(pf.ApprovedBudget), 0)");
                            mainTblQry.Append("                     FROM   PM_Financial pf");
                            mainTblQry.Append("                            INNER JOIN pm_Entity children");
                            mainTblQry.Append("                                 ON  pf.EntityID = children.ID");
                            mainTblQry.Append("                                 AND children.[Active] = 1");
                            mainTblQry.Append("                     WHERE  children.ParentID = subtbl.ID");
                            mainTblQry.Append("                            AND pf.CostCenterID = eoi.CostCenterID");
                            mainTblQry.Append("                 )");
                            mainTblQry.Append("            ELSE (");
                            mainTblQry.Append("                     SELECT pf.ApprovedAllocatedAmount - pf.ApprovedBudget");
                            mainTblQry.Append("                     FROM   PM_Financial pf");
                            mainTblQry.Append("                     WHERE  pf.EntityID = subtbl.ID");
                            mainTblQry.Append("                            AND pf.CostCenterID = eoi.CostCenterID");
                            mainTblQry.Append("                 )");
                            mainTblQry.Append("       END        AS BudgetDeviation,");
                            mainTblQry.Append("       CASE ");
                            mainTblQry.Append("            WHEN pe.TypeID = 5 THEN (");
                            mainTblQry.Append("                     (");
                            mainTblQry.Append("                         SELECT ISNULL(SUM(pf.ApprovedAllocatedAmount), 0)");
                            mainTblQry.Append("                         FROM   PM_Financial pf");
                            mainTblQry.Append("                                INNER JOIN pm_Entity ");
                            mainTblQry.Append("                                     children");
                            mainTblQry.Append("                                     ON  pf.EntityID = children.ID");
                            mainTblQry.Append("                                     AND children.[Active] = 1");
                            mainTblQry.Append("                                     AND children.[level] = 1");
                            mainTblQry.Append("                         WHERE  pf.CostCenterID = subtbl.ID");
                            mainTblQry.Append("                     )");
                            mainTblQry.Append("                     -(");
                            mainTblQry.Append("                         SELECT ISNULL(SUM(pf.Commited), 0)");
                            mainTblQry.Append("                         FROM   PM_Financial pf");
                            mainTblQry.Append("                                INNER JOIN pm_Entity ");
                            mainTblQry.Append("                                     children");
                            mainTblQry.Append("                                     ON  pf.EntityID = children.ID");
                            mainTblQry.Append("                                     AND children.[Active] = 1");
                            mainTblQry.Append("                         WHERE  pf.CostCenterID = subtbl.ID");
                            mainTblQry.Append("                     )");
                            mainTblQry.Append("                 )");
                            mainTblQry.Append("            WHEN pe.[level] = 0 THEN (");
                            mainTblQry.Append("                     (");
                            mainTblQry.Append("                         SELECT ISNULL(SUM(pf.ApprovedAllocatedAmount), 0)");
                            mainTblQry.Append("                         FROM   PM_Financial pf");
                            mainTblQry.Append("                                INNER JOIN pm_Entity children");
                            mainTblQry.Append("                                     ON  pf.EntityID = children.ID");
                            mainTblQry.Append("                                     AND children.[Active] = 1");
                            mainTblQry.Append("                         WHERE  children.ParentID = subtbl.ID");
                            mainTblQry.Append("                                AND pf.CostCenterID = eoi.CostCenterID");
                            mainTblQry.Append("                     )");
                            mainTblQry.Append("                     -(");
                            mainTblQry.Append("                         SELECT ISNULL(SUM(pf.Commited), 0)");
                            mainTblQry.Append("                         FROM   PM_Financial pf");
                            mainTblQry.Append("                                INNER JOIN pm_Entity children");
                            mainTblQry.Append("                                     ON  pf.EntityID = children.ID");
                            mainTblQry.Append("                                     AND children.[Active] = 1");
                            mainTblQry.Append("                         WHERE  children.UniqueKey LIKE(");
                            mainTblQry.Append("                                    SELECT pe.UniqueKey");
                            mainTblQry.Append("                                    FROM   PM_Entity pe");
                            mainTblQry.Append("                                    WHERE  pe.ID = subtbl.ID");
                            mainTblQry.Append("                                )");
                            mainTblQry.Append("                                AND pf.CostCenterID = eoi.CostCenterID");
                            mainTblQry.Append("                     )");
                            mainTblQry.Append("                 )");
                            mainTblQry.Append("            ELSE (");
                            mainTblQry.Append("                     (");
                            mainTblQry.Append("                         SELECT pf.ApprovedAllocatedAmount - pf.Commited");
                            mainTblQry.Append("                         FROM   PM_Financial pf");
                            mainTblQry.Append("                         WHERE  pf.EntityID = subtbl.ID");
                            mainTblQry.Append("                                AND pf.CostCenterID = eoi.CostCenterID");
                            mainTblQry.Append("                     )");
                            mainTblQry.Append("                     ");
                            mainTblQry.Append("                     -(");
                            mainTblQry.Append("                         SELECT ISNULL(SUM(pf.ApprovedAllocatedAmount), 0)");
                            mainTblQry.Append("                         FROM   PM_Financial pf");
                            mainTblQry.Append("                                INNER JOIN pm_Entity children");
                            mainTblQry.Append("                                     ON  pf.EntityID = children.ID");
                            mainTblQry.Append("                                     AND children.[Active] = 1");
                            mainTblQry.Append("                         WHERE  children.ParentID = subtbl.ID");
                            mainTblQry.Append("                                AND pf.CostCenterID = eoi.CostCenterID");
                            mainTblQry.Append("                     )");
                            mainTblQry.Append("                 )");
                            mainTblQry.Append("       END        AS Available,");

                            mainTblQry.Append("        ");
                            mainTblQry.Append(" isnull((SELECT  metso.StatusOptions FROM MM_EntityStatus mes INNER JOIN MM_EntityTypeStatus_Options metso ON mes.StatusID=metso.ID AND mes.EntityID=pe.id),'-')  AS Status");
                        }
                        else if (EntityTypeIDs.Objective == (EntityTypeIDs)enumEntityTypeIds)
                        {
                            ArrayList objectiveDisplayColumn = new ArrayList();
                            objectiveDisplayColumn.Add("Type");
                            objectiveDisplayColumn.Add("Rating Objective");
                            objectiveDisplayColumn.Add("Target Outcome");
                            objectiveDisplayColumn.Add("Fulfilment");
                            objectiveDisplayColumn.Add("Status");
                            ArrayList objectiveColumn = new ArrayList();
                            objectiveColumn.Add("Type");
                            objectiveColumn.Add("RatingObjective");
                            objectiveColumn.Add("TargetOutcome");
                            objectiveColumn.Add("Fulfilment");
                            objectiveColumn.Add("Status");
                            Tuple<ArrayList, ArrayList> objColumn = Tuple.Create(objectiveColumn, objectiveDisplayColumn);
                            lstrecord.GeneralColumnDefs = objColumn;
                            mainTblQry.Append(" ,CASE when pe.TypeID =10 then eoi.EID else eoi.ObjectveID  end as ObjectveID");
                            mainTblQry.Append(" ,CASE WHEN eoi.ObjectveID !=0 then cast(eoi.ObjectveID AS VARCHAR) + '.' + pe.UniqueKey ELSE CAST(pe.id AS VARCHAR) end AS class ");
                            if (IsEntityAvailable == true)
                            {
                                mainTblQry.Append(" ,0 AS TotalChildrenCount,");
                            }
                            else
                            {
                                string inEntiTypes = "";
                                if (listSetting.EntityTypes != null)
                                {
                                    inEntiTypes = "("
                                                  + String.Join(",", listSetting.EntityTypes.Select(x => x.ToString()).ToArray())
                                                + ")";

                                }
                                mainTblQry.Append(" ,ISNULL(CASE when pe.TypeID=10 THEN (SELECT COUNT(1) FROM   PM_ObjectiveEntityValue pecr INNER JOIN PM_Entity pe2 ");
                                mainTblQry.Append(" ON  pecr.EntityID = pe2.ID AND pe2.[Active]=1  WHERE pecr.ObjectiveID=pe.id ");
                                if (inEntiTypes.Length > 0)
                                {
                                    mainTblQry.Append(" and pe2.TypeID in" + inEntiTypes + " ");
                                }
                                mainTblQry.Append(") else (SELECT COUNT(1) FROM PM_ObjectiveEntityValue pecr INNER JOIN PM_Entity pe2 ON pecr.EntityID=pe2.ID ");
                                mainTblQry.Append(" AND pe2.[Active]=1 AND pe2.ParentID=pe.id WHERE pecr.ObjectiveID=eoi.ObjectveID");
                                if (inEntiTypes.Length > 0)
                                {
                                    mainTblQry.Append(" and pe2.TypeID in" + inEntiTypes + "  ");
                                }
                                mainTblQry.Append(") end,0)   AS TotalChildrenCount, ");
                            }
                            mainTblQry.Append(" (SELECT TOP 1 CASE WHEN typeid=1 THEN 'Numeric(Quantitative)' WHEN typeid=2 THEN 'Numeric(Non Quantitative)' WHEN typeid=3 then 'Qualitative'   ");
                            mainTblQry.Append(" WHEN typeid=4 THEN 'Rating'  end FROM PM_Objective po  WHERE id =pe.ID)AS Type ");
                            mainTblQry.Append(" ,(SELECT Caption FROM PM_Objective_Rating WHERE ID = (SELECT TOP 1  poev.RatingObjective  ");
                            mainTblQry.Append(" FROM PM_ObjectiveEntityValue poev where poev.ObjectiveID=eoi.ObjectveID AND poev.EntityID=pe.ID)) AS RatingObjective, ");
                            //mainTblQry.Append(" (SELECT TOP 1 poev.PlannedTarget  ");
                            //mainTblQry.Append(" FROM PM_ObjectiveEntityValue poev WHERE poev.ObjectiveID=eoi.ObjectveID AND poev.EntityID=pe.ID)AS PlannedTarget, ");
                            mainTblQry.Append(" (SELECT TOP 1 poev.TargetOutcome  ");
                            mainTblQry.Append(" FROM PM_ObjectiveEntityValue poev WHERE poev.ObjectiveID=eoi.ObjectveID AND poev.EntityID=pe.ID)AS TargetOutcome, ");
                            // mainTblQry.Append(" (SELECT TOP 1 poev.Fulfilment ");
                            mainTblQry.Append(" (SELECT TOP 1 CASE WHEN  poev.Fulfilment = 1 THEN 'Fulfilled' WHEN poev.Fulfilment = 2 THEN 'Not Fulfilled' end ");
                            mainTblQry.Append(" FROM PM_ObjectiveEntityValue poev WHERE poev.ObjectiveID=eoi.ObjectveID AND poev.EntityID=pe.ID) AS Fulfilment, ");
                            mainTblQry.Append(" CASE WHEN pe.TypeID = " + (int)EntityTypeList.Objective + " THEN (SELECT case when ISNULL(po.ObjectiveStatus,0)=0 THEN 'Deactivated'  ELSE 'Active'  END from  PM_Objective po WHERE po.id=pe.Id) else  isnull((SELECT  metso.StatusOptions FROM MM_EntityStatus mes INNER JOIN MM_EntityTypeStatus_Options metso ON mes.StatusID=metso.ID AND mes.EntityID=pe.id),'-') end  AS Status");
                        }
                    }
                    else
                    {
                        ArrayList objectiveDisplayColumn = new ArrayList();
                        objectiveDisplayColumn.Add("Type");
                        objectiveDisplayColumn.Add("StartDate");
                        objectiveDisplayColumn.Add("EndDate");
                        objectiveDisplayColumn.Add("Owner");
                        objectiveDisplayColumn.Add("Status");
                        ArrayList objectiveColumn = new ArrayList();
                        objectiveColumn.Add("Type");
                        objectiveColumn.Add("StartDate");
                        objectiveColumn.Add("EndDate");
                        objectiveColumn.Add("Owner");
                        objectiveColumn.Add("Status");
                        Tuple<ArrayList, ArrayList> objColumn = Tuple.Create(objectiveColumn, objectiveDisplayColumn);
                        lstrecord.GeneralColumnDefs = objColumn;
                        mainTblQry.Append(" ,(SELECT TOP 1 CASE WHEN typeid=1 THEN 'Numeric(Quantitative)' WHEN typeid=2 THEN 'Numeric(Non Quantitative)' WHEN typeid=3 then 'Qualitative'   ");
                        mainTblQry.Append(" WHEN typeid=4 THEN 'Rating'  end FROM PM_Objective po  WHERE id =subtbl.ID)AS Type ");
                        mainTblQry.Append(" ,(SELECT top 1 CAST( po.StartDate AS NVARCHAR(10)) FROM PM_Objective po WHERE po.id=subtbl.Id) as StartDate, ");
                        mainTblQry.Append(" (SELECT top 1 CAST(po.EndDate AS NVARCHAR(10)) FROM PM_Objective po WHERE po.id=subtbl.Id) as EndDate, ");
                        mainTblQry.Append("ISNULL( (SELECT top 1  ISNULL(us.FirstName,'') + ' ' + ISNULL(us.LastName,'')  FROM UM_User us INNER JOIN AM_Entity_Role_User aeru ON us.ID=aeru.UserID AND aeru.EntityID=subtbl.Id INNER JOIN AM_EntityTypeRoleAcl aetra ON aeru.RoleID=aetra.ID AND aetra.EntityRoleID=1 ),'-') as Owner ");
                        mainTblQry.Append(",(SELECT case when ISNULL(po.ObjectiveStatus,0)=0 THEN 'Deactivated'  ELSE 'Active'  END from  PM_Objective po WHERE po.id=subtbl.Id)  AS Status");
                    }

                    int LastTreeLevel = listSetting.Attributes.Where(a => (AttributesList)a.Type == AttributesList.TreeMultiSelection).OrderByDescending(a => a.Level).Select(a => a.Level).FirstOrDefault();

                    for (int j = 0; j < listSetting.Attributes.Count; j++)
                    {

                        string CurrentattrID = listSetting.Attributes[j].Id.ToString();
                        if (listSetting.Attributes[j].IsSpecial == true)
                        {
                            switch ((SystemDefinedAttributes)listSetting.Attributes[j].Id)
                            {
                                case SystemDefinedAttributes.Owner:
                                    mainTblQry.Append(",ISNULL( (SELECT top 1  ISNULL(us.FirstName,'') + ' ' + ISNULL(us.LastName,'')  FROM UM_User us INNER JOIN AM_Entity_Role_User aeru ON us.ID=aeru.UserID AND aeru.EntityID=subtbl.Id  INNER JOIN AM_EntityTypeRoleAcl aetra ON  aeru.RoleID = aetra.ID AND  aetra.EntityTypeID=pe.TypeID AND aetra.EntityRoleID = 1),'-') as '" + listSetting.Attributes[j].Field + "'");
                                    break;
                                case SystemDefinedAttributes.EntityStatus:
                                    mainTblQry.Append(", CASE WHEN pe.TypeID = " + (int)EntityTypeList.Objective + " THEN (SELECT case when ISNULL(po.ObjectiveStatus,0)=0 THEN 'Deactivated'  ELSE 'Active'  END from  PM_Objective po WHERE po.id=subtbl.Id) else isnull((SELECT  metso.StatusOptions FROM MM_EntityStatus mes INNER JOIN MM_EntityTypeStatus_Options metso ON mes.StatusID=metso.ID AND mes.EntityID=subtbl.id),'-') end as '" + listSetting.Attributes[j].Field + "'");
                                    break;
                                case SystemDefinedAttributes.EntityOnTimeStatus:
                                    mainTblQry.Append(", CASE WHEN pe.TypeID = " + (int)EntityTypeList.Objective + " THEN (SELECT case when ISNULL(po.ObjectiveStatus,0)=0 THEN '-'  ELSE '-'  END from  PM_Objective po WHERE po.id=subtbl.Id) else isnull((SELECT CASE WHEN (SELECT mes.IntimeStatus FROM MM_EntityStatus mes WHERE mes.EntityID=subtbl.id) = 0 THEN 'On time' WHEN (SELECT mes.IntimeStatus FROM MM_EntityStatus mes WHERE mes.EntityID=subtbl.id) = 1 THEN 'Delayed' WHEN (SELECT mes.IntimeStatus FROM MM_EntityStatus mes WHERE mes.EntityID=subtbl.id) = 2 THEN 'On hold' ELSE 'On time' END AS ontimestatus), '-') END AS '" + listSetting.Attributes[j].Field + "'");
                                    break;
                            }
                        }
                        else if ((AttributesList)listSetting.Attributes[j].Type == AttributesList.ListMultiSelection || (AttributesList)listSetting.Attributes[j].Type == AttributesList.DropDownTree || (AttributesList)listSetting.Attributes[j].Type == AttributesList.Tree || (AttributesList)listSetting.Attributes[j].Type == AttributesList.Period || (AttributesList)listSetting.Attributes[j].Type == AttributesList.TreeMultiSelection)
                        {
                            switch ((AttributesList)listSetting.Attributes[j].Type)
                            {
                                case AttributesList.ListMultiSelection:

                                    if (listSetting.Attributes[j].Id != (int)SystemDefinedAttributes.ObjectiveType)
                                    {

                                        mainTblQry.Append(" ,(SELECT  ");
                                        mainTblQry.Append(" STUFF( ");
                                        mainTblQry.Append(" ( ");
                                        mainTblQry.Append(" SELECT ', ' +  mo.Caption ");
                                        mainTblQry.Append(" FROM   MM_MultiSelect mms2 ");
                                        mainTblQry.Append(" INNER JOIN MM_Option mo ");
                                        mainTblQry.Append(" ON  mms2.OptionID = mo.ID and  mms2.AttributeID=" + listSetting.Attributes[j].Id);
                                        mainTblQry.Append("  WHERE  mms2.EntityID = mms.EntityID ");
                                        mainTblQry.Append(" FOR XML PATH('') ");
                                        mainTblQry.Append(" ), ");
                                        mainTblQry.Append("  1, ");
                                        mainTblQry.Append(" 2, ");
                                        mainTblQry.Append(" '' ");
                                        mainTblQry.Append("  )               AS VALUE ");
                                        mainTblQry.Append(" FROM   MM_MultiSelect     mms ");
                                        mainTblQry.Append(" WHERE  mms.EntityID=subtbl.Id and  mms.AttributeID = " + CurrentattrID + " ");
                                        mainTblQry.Append(" GROUP BY ");
                                        mainTblQry.Append("  mms.EntityID) as '" + listSetting.Attributes[j].Field + "'");
                                    }

                                    break;
                                case AttributesList.DropDownTree:
                                    mainTblQry.Append(" ,(ISNULL( ");

                                    mainTblQry.Append(" ( ");
                                    mainTblQry.Append(" SELECT top 1 mtn.Caption ");
                                    mainTblQry.Append(" FROM   MM_TreeNode mtn ");
                                    mainTblQry.Append(" INNER JOIN MM_TreeValue mtv ");
                                    mainTblQry.Append("  ON  mtv.NodeID = mtn.ID ");
                                    mainTblQry.Append("  AND mtv.AttributeID = mtn.AttributeID ");
                                    mainTblQry.Append("   AND mtn.Level = " + listSetting.Attributes[j].Level + " ");
                                    mainTblQry.Append("  WHERE  mtv.EntityID = subtbl.Id ");
                                    mainTblQry.Append(" AND mtv.AttributeID = " + CurrentattrID + "   ");
                                    mainTblQry.Append(" ), ");
                                    mainTblQry.Append(" '-' ");
                                    mainTblQry.Append(" ) ) as '" + listSetting.Attributes[j].Field + "'");
                                    break;
                                case AttributesList.Tree:
                                    mainTblQry.Append(" ,'IsTree' as '" + listSetting.Attributes[j].Field + "'");
                                    break;
                                case AttributesList.Period:
                                    mainTblQry.Append(" ,(SELECT (SELECT CONVERT(NVARCHAR(10), pep.StartDate, 120)  '@s', CONVERT(NVARCHAR(10), pep.EndDate, 120) '@e',");
                                    mainTblQry.Append(" isnull(pep.[Description],'') '@d', ROW_NUMBER() over(ORDER BY pep.Startdate) '@sid',");
                                    mainTblQry.Append(" pep.ID '@o'");
                                    mainTblQry.Append(" FROM   PM_EntityPeriod pep");
                                    mainTblQry.Append(" WHERE  pep.EntityID = subtbl.Id ORDER BY pep.Startdate FOR XML PATH('p'),");
                                    mainTblQry.Append(" TYPE");
                                    mainTblQry.Append(" ) FOR XML PATH('root')");
                                    mainTblQry.Append(" )  AS 'Period'");
                                    mainTblQry.Append(" ,(SELECT (SELECT CONVERT(NVARCHAR(10), pep.Attr_56, 120)  '@s',");
                                    mainTblQry.Append(" pep.Attr_2 '@d',");
                                    mainTblQry.Append(" pep.Attr_67 '@ms',isnull(pem.Name,'') '@n',");
                                    mainTblQry.Append(" pep.ID '@o'");
                                    mainTblQry.Append(" FROM   MM_AttributeRecord_" + (int)EntityTypeList.Milestone + " pep  INNER JOIN PM_Entity pem ON pep.ID=pem.id ");
                                    mainTblQry.Append(" WHERE  pep.Attr_66 = subtbl.Id FOR XML PATH('p'),");
                                    mainTblQry.Append(" TYPE");
                                    mainTblQry.Append(" ) FOR XML PATH('root')");
                                    mainTblQry.Append(" )  AS 'MileStone'");
                                    break;
                                case AttributesList.TreeMultiSelection:
                                    if (LastTreeLevel == listSetting.Attributes[j].Level)
                                    {
                                        mainTblQry.Append(" ,(SELECT  ");
                                        mainTblQry.Append(" STUFF( ");
                                        mainTblQry.Append(" ( ");
                                        mainTblQry.Append(" SELECT ', ' +  mtn.Caption ");
                                        mainTblQry.Append(" FROM   MM_TreeNode mtn ");
                                        mainTblQry.Append(" INNER JOIN MM_TreeValue mtv ");
                                        mainTblQry.Append(" ON  mtv.NodeID = mtn.ID and  mtv.AttributeID=" + listSetting.Attributes[j].Id);
                                        mainTblQry.Append("  AND mtn.Level = " + listSetting.Attributes[j].Level + " WHERE mtv.EntityID = subtbl.Id AND mtv.AttributeID = " + CurrentattrID + "  ");
                                        mainTblQry.Append(" FOR XML PATH('') ");
                                        mainTblQry.Append(" ), ");
                                        mainTblQry.Append("  1, ");
                                        mainTblQry.Append(" 2, ");
                                        mainTblQry.Append(" '' ");
                                        mainTblQry.Append(" ) ) as '" + listSetting.Attributes[j].Field + "'");
                                    }
                                    else
                                    {
                                        mainTblQry.Append(" ,(ISNULL( ");

                                        mainTblQry.Append(" ( ");
                                        mainTblQry.Append(" SELECT top 1 mtn.Caption ");
                                        mainTblQry.Append(" FROM   MM_TreeNode mtn ");
                                        mainTblQry.Append(" INNER JOIN MM_TreeValue mtv ");
                                        mainTblQry.Append("  ON  mtv.NodeID = mtn.ID ");
                                        mainTblQry.Append("  AND mtv.AttributeID = mtn.AttributeID ");
                                        mainTblQry.Append("   AND mtn.Level = " + listSetting.Attributes[j].Level + " ");
                                        mainTblQry.Append("  WHERE  mtv.EntityID = subtbl.Id ");
                                        mainTblQry.Append(" AND mtv.AttributeID = " + CurrentattrID + "   ");
                                        mainTblQry.Append(" ), ");
                                        mainTblQry.Append(" '-' ");
                                        mainTblQry.Append(" ) ) as '" + listSetting.Attributes[j].Field + "'");
                                    }
                                    break;
                            }
                        }
                        else if ((AttributesList)listSetting.Attributes[j].Type == AttributesList.ListSingleSelection)
                        {
                            mainTblQry.Append(",(isnull( (SELECT top 1 caption FROM MM_Option  WHERE AttributeID=" + CurrentattrID + " AND id=subtbl.Attr_" + CurrentattrID + "),'-') ) as '" + listSetting.Attributes[j].Field + "'");
                        }
                        else if ((AttributesList)listSetting.Attributes[j].Type == AttributesList.CheckBoxSelection)
                        {
                            mainTblQry.Append(" ,isnull(cast(subtbl.attr_" + CurrentattrID + " as varchar(50)), '-') as '" + listSetting.Attributes[j].Field + "'");
                        }
                        else if ((AttributesList)listSetting.Attributes[j].Type == AttributesList.DateTime)
                        {
                            mainTblQry.Append(" ,REPLACE(CONVERT(varchar,isnull(subtbl.attr_" + CurrentattrID + " ,''),121),'1900-01-01 00:00:00.000', '-') as '" + listSetting.Attributes[j].Field + "'");
                            //--ISNULL(subtbl.attr_62, '-')  AS '62'
                            //REPLACE(CONVERT(varchar, ISNULL(subtbl.attr_62,''),121),'1900-01-01 00:00:00.000','-')  AS '62'
                        }
                        else
                        {
                            mainTblQry.Append(" ,isnull(subtbl.attr_" + CurrentattrID + " , '-') as '" + listSetting.Attributes[j].Field + "'");
                        }

                    }



                    //static query
                    mainTblQry.Append(" From (" + strqry.ToString() + ") as subtbl");


                    finalQry.Append(mainTblQry.ToString());


                    finalQry.Append("  INNER JOIN PM_Entity pe ");
                    finalQry.Append(" ON subtbl.Id=pe.ID INNER JOIN MM_EntityType met  ");
                    finalQry.Append(" ON pe.TypeID=met.ID  ");
                    finalQry.Append(" INNER JOIN @EntityOrderIDs eoi ON  eoi.EID = subtbl.Id ");


                    finalQry.Append(" ORDER BY eoi.ID ");


                    lstrecord.Data = tx.PersistenceManager.MetadataRepository.ExecuteQuery(finalQry.ToString());

                    AttributeDao attrdao = new AttributeDao();
                    IList<IAttribute> _iiAttribute = new List<IAttribute>();
                    foreach (var item in listSetting.Attributes)
                    {
                        int id = item.Id;
                        attrdao = tx.PersistenceManager.MetadataRepository.Get<AttributeDao>(id);
                        if (attrdao != null)
                        {
                            IAttribute _iAttribute = new BrandSystems.Marcom.Core.Metadata.Attribute();
                            _iAttribute.Caption = HttpUtility.HtmlDecode(attrdao.Caption);
                            _iAttribute.AttributeTypeID = attrdao.AttributeTypeID;
                            _iAttribute.IsSystemDefined = attrdao.IsSystemDefined;
                            _iAttribute.Id = attrdao.Id;
                            _iiAttribute.Add(_iAttribute);
                        }
                    }


                    lstrecord.DataCount = proxy.MarcomManager.EntitySortorderIdColle.Where(a => a.Level < (Level != 0 ? Level : 2)).ToList().Count;
                    lstrecord.Attributes = _iiAttribute;
                    lstrecord.ColumnDefs = listSetting.Attributes;


                    tx.Commit();

                    RowNo = 7;
                    ColumnNo = 11;
                    LastColumnNo = 0;
                    tempcolmno = 0;
                    CalenderStartDate = new System.DateTime(System.DateTime.Now.Year - 1, 1, 1);
                    CalenderEndDate = new System.DateTime(System.DateTime.Now.Year + 1, 12, 31);
                    //if (GanttstartDate == null || Ganttenddate == null || GanttstartDate.Length == 0 || Ganttenddate.Length == 0)
                    //{
                    //    CalenderStartDate = new System.DateTime(System.DateTime.Now.Year - 1, 1, 1);
                    //    CalenderEndDate = new System.DateTime(System.DateTime.Now.Year + 1, 12, 31);
                    //}
                    //else
                    //{
                    //    //DateTime dt = DateTime.ParseExact("24/01/2013", "dd/MM/yyyy", null);
                    //    CalenderStartDate = DateTime.Parse(GanttstartDate.ToString());
                    //    CalenderEndDate = DateTime.Parse(Ganttenddate.ToString());

                    //    //CalenderStartDate = DateTime.ParseExact(GanttstartDate.ToString(),"MM/dd/yyyy HH:mm:tt" , null);
                    //    //CalenderEndDate = DateTime.ParseExact(Ganttenddate.ToString(), "MM/dd/yyyy HH:mm:tt", null);

                    //}
                    IsObjectiveOrCostCenterPresent = false;
                    FilterID = 0;
                    ListOfEntityID = "";
                    TypeID = 0;


                    string returnfileid = Generatereport(lstrecord, IsMonthly);



                    return returnfileid;


                }

            }
            catch
            {
                return null;
            }


        }

        public string Generatereport(IListofRecord ListofRecords, bool IsMonthly)
        {

            try
            {

                string NewGuid = Guid.NewGuid().ToString();

                if (ListofRecords.Data != null)
                {



                    string mappingfilesPath = AppDomain.CurrentDomain.BaseDirectory;

                    dynamic fullpath = mappingfilesPath + ("/Files/ReportFiles/Images/Temp/") + NewGuid + ".xlsx";

                    FileInfo newFile = new FileInfo(fullpath);

                    ExcelPackage pck = new ExcelPackage(newFile);
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Quarterly View");


                    List<string> AttributeList = new List<string>();
                    GenerateTitle(ws, 1, IsMonthly);


                    AttributeList.Add("Name");
                    AttributeList.Add("ID#");
                    foreach (var item in ListofRecords.ColumnDefs)
                    {

                        if (item.Field != "68" && item.Type != 10)
                        {
                            AttributeList.Add(item.DisplayName);
                        }
                    }

                    GenerateDynamicColumn(ws, AttributeList);

                    // MergeIconColumnHeader(ws, 10, 11, 6);



                    tempcolmno = 10 + (AttributeList.Count + 2);
                    if (!IsMonthly)
                    {
                        LastColumnNo = GenerateMonthlyViewHeader(ws, ColumnNo);  //generate normal gantview header
                        GenerateGanttView(ws, ListofRecords);   //generate normal gantview header
                        int check = mergeQuarter(ws, tempcolmno);
                    }
                    if (IsMonthly)
                    {
                        LastColumnNo = GenerateGanttMonthlyViewHeader(ws, ColumnNo);
                        GenerateMonthlyGanttView(ws, ListofRecords);
                    }
                    ws.View.ShowGridLines = false;


                    pck.Workbook.Properties.Title = "Quarterly Gantt View";
                    pck.Workbook.Properties.Author = "Marcom Plarform";
                    pck.Workbook.Properties.Subject = "Quarterly Gantt View";
                    pck.Workbook.Properties.Keywords = "Quarterly Gantt View";

                    pck.Save();

                    string strFriendlyName = "";
                    if (IsMonthly) strFriendlyName = "Gantt-view-(Monthly)-" + System.DateTime.Now.ToString("dd-MM-yyyy") + ".xlsx";
                    else strFriendlyName = "Gantt-view-(Quarterly)-" + System.DateTime.Now.ToString("dd-MM-yyyy") + ".xlsx";

                    dynamic name = System.IO.Path.GetFileName(fullpath);
                    name = name.Replace(System.IO.Path.GetFileName(fullpath), strFriendlyName);
                    dynamic ext = System.IO.Path.GetExtension(fullpath);
                    string type = "";
                    type = "application/vnd.ms-excel";


                }
                return NewGuid;
            }
            catch (Exception ex)
            {
                //Log("Page_Load Exception", ex.Message + Constants.vbNewLine + ex.StackTrace);
            }

            return null;
        }

        protected int HexStrToBase10Int(string hex)
        {
            int base10value = 0;


            try
            {
                return (Convert.ToInt32(hex, 16));
            }
            catch
            { }


            return base10value;
        }

        public bool GenerateTitle(ExcelWorksheet ws, int StartColumnNo, bool ismonthly)
        {
            ws.Row(2).Height = 35;



            var _with1 = ws.Cells[1, 1];

            _with1.Value = ismonthly ? "Gantt View Monthly Report" : "Gantt View Quarterly Report";

            _with1.Style.Font.Name = "Arial";
            _with1.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(64, 64, 64));
            _with1.Style.Font.Size = 20;
            _with1.Style.Font.Bold = true;


            return true;
        }

        public bool GenerateDynamicColumn(ExcelWorksheet ws, List<string> ColumnName)
        {


            var _with1 = ws.Cells[6, 1, 6, 11];
            //_with1.Merge = true;
            _with1.Value = "Name";
            _with1.Merge = true;
            _with1.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            _with1.Style.Border.Left.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

            _with1.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            _with1.Style.Border.Right.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

            _with1.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            _with1.Style.Border.Top.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

            _with1.Style.Fill.PatternType = ExcelFillStyle.Solid;
            _with1.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(255, 255, 255));

            _with1.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            _with1.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

            _with1.Style.Font.Name = "Arial";
            _with1.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(51, 51, 51));
            _with1.Style.Font.Size = 11;
            _with1.Style.Font.Bold = true;
            _with1.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            _with1.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            ws.Column(1).Width = 3;
            ws.Column(2).Width = 3;
            ws.Column(3).Width = 3;
            ws.Column(4).Width = 3;
            ws.Column(5).Width = 3;
            ws.Column(6).Width = 3;
            ws.Column(7).Width = 3;
            ws.Column(8).Width = 3;
            ws.Column(9).Width = 3;
            ws.Column(10).Width = 3;


            foreach (var item in ColumnName)
            {

                _with1 = ws.Cells[6, ColumnNo];
                //_with1.Merge = true;
                _with1.Value = item != " " ? item.ToString() : " ";
                _with1.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                _with1.Style.Border.Left.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                _with1.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                _with1.Style.Border.Right.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                _with1.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                _with1.Style.Border.Top.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                _with1.Style.Fill.PatternType = ExcelFillStyle.Solid;
                _with1.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(255, 255, 255));

                _with1.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                _with1.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                _with1.Style.Font.Name = "Arial";
                _with1.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(51, 51, 51));
                _with1.Style.Font.Size = 11;
                _with1.Style.Font.Bold = true;
                _with1.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                _with1.Style.VerticalAlignment = ExcelVerticalAlignment.Center;


                if (ColumnNo == 11)
                {
                    ws.Column(ColumnNo).Width = 18;
                }
                else if (ColumnNo == 12)
                {
                    ws.Column(ColumnNo).Width = 18;
                }
                else if (ColumnNo == 13)
                {
                    ws.Column(ColumnNo).Width = 18;
                }
                else if (ColumnNo == 14)
                {
                    ws.Column(ColumnNo).Width = 18;
                }
                else if (ColumnNo == 15)
                {
                    ws.Column(ColumnNo).Width = 18;
                }

                else
                {
                    ws.Column(ColumnNo).Width = 18;
                }


                ColumnNo = ColumnNo + 1;
            }



            return true;
        }


        public void MergeIconColumnHeader(ExcelWorksheet ws, int startcolumnno, int endcolumnno, int rowno)
        {
            var _with1 = ws.Cells[rowno, startcolumnno, rowno, endcolumnno];
            _with1.Merge = true;
            _with1.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            _with1.Style.Border.Left.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

            _with1.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            _with1.Style.Border.Right.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

            _with1.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            _with1.Style.Border.Top.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

            _with1.Style.Fill.PatternType = ExcelFillStyle.Solid;
            _with1.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(255, 255, 255));

            _with1.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            _with1.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

            _with1.Style.Font.Name = "Arial";
            _with1.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(51, 51, 51));
            _with1.Style.Font.Size = 11;
            _with1.Style.Font.Bold = true;
            _with1.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            _with1.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        }


        public int mergeQuarter(ExcelWorksheet ws, int tempcolmno)
        {


            try
            {

                DateTime startDate = new DateTime(CalenderStartDate.Year, CalenderStartDate.Month, CalenderStartDate.Day);
                DateTime stopDate = new DateTime(CalenderEndDate.Year, CalenderEndDate.Month, CalenderEndDate.Day);
                int interval = 1;

                int isoddcolor = 0;

                int startq = tempcolmno - 1;
                int endq = tempcolmno - 1;

                DateTime dateTime = startDate;
                bool StartMonthEnd = true;


                while (dateTime <= stopDate)
                {

                    try
                    {
                        if ((ws.Cells[3, tempcolmno - 1].Value.ToString() == ws.Cells[3, tempcolmno].Value.ToString()) && (tempcolmno) != LastColumnNo)
                        {
                            endq = tempcolmno;

                        }
                        else
                        {
                            if ((tempcolmno) == LastColumnNo)
                                endq++;

                            var _with1 = ws.Cells[3, startq, 3, endq]; ;

                            _with1.Merge = true;
                            _with1.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            _with1.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            if (isoddcolor == 0)
                            {
                                _with1.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                _with1.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(255, 255, 255));
                                isoddcolor = 1;
                            }
                            else
                            {
                                _with1.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                _with1.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(242, 242, 242));
                                isoddcolor = 0;
                            }

                            startq = endq + 1;

                        }



                    }
                    catch (Exception ex)
                    {
                    }

                    dateTime += TimeSpan.FromDays(interval);
                    tempcolmno = tempcolmno + 1;
                }

                return 1;

            }
            catch (Exception ex)
            {
                return 0;
            }
        }


        public int GenerateGanttMonthlyViewHeader(ExcelWorksheet ws, int StartColumnNo)
        {

            int Column = StartColumnNo;
            int StartColumn = Column;
            ws.Row(2).Height = 12;
            ws.Row(3).Height = 12;
            ws.Row(4).Height = 12;
            ws.Row(5).Height = 20;
            ws.Row(6).Height = 20;

            DateTime startDate = new DateTime(CalenderStartDate.Year, CalenderStartDate.Month, CalenderStartDate.Day);
            DateTime stopDate = new DateTime(CalenderEndDate.Year, CalenderEndDate.Month, CalenderEndDate.Day);
            int interval = 1;

            DateTime dateTime = startDate;
            int DaysInMonth = System.DateTime.DaysInMonth(dateTime.Year, dateTime.Month);
            bool OnlyForFirstMonth = true;




            while (dateTime <= stopDate)
            {
                var _with1 = ws.Cells[5, Column];
                _with1.Value = new System.DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
                _with1.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                _with1.Style.Border.Left.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                _with1.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                _with1.Style.Border.Right.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                _with1.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                _with1.Style.Border.Top.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));


                _with1.Style.Font.Name = "Calibri";
                _with1.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(51, 51, 51));
                _with1.Style.Font.Size = 6;
                _with1.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                _with1.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                _with1.Style.Numberformat.Format = "ddd";

                var _with2 = ws.Cells[6, Column];
                _with2.Value = new System.DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
                _with2.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                _with2.Style.Border.Left.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                _with2.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                _with2.Style.Border.Right.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                _with2.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                _with2.Style.Border.Top.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                _with2.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                _with2.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                _with2.Style.Font.Name = "Calibri";
                _with2.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(51, 51, 51));
                _with2.Style.Font.Size = 6;
                _with2.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                _with2.Style.Numberformat.Format = "dd";

                int CurrentMonthinDays = System.DateTime.DaysInMonth(dateTime.Year, dateTime.Month);
                DateTime EndMonthDate = new DateTime(dateTime.Year, dateTime.Month, CurrentMonthinDays);
                dateTime += TimeSpan.FromDays(interval);
                Column = Column + 1;

                if (((EndMonthDate == dateTime | stopDate == dateTime | startDate == dateTime) & (stopDate >= dateTime)))
                {
                    if ((stopDate == dateTime) & EndMonthDate != dateTime)
                    {
                        CurrentMonthinDays = stopDate.Day;
                    }

                    if ((OnlyForFirstMonth == true))
                    {
                        if ((startDate.Day != 1 & startDate.Month == dateTime.Month & startDate.Year == dateTime.Year))
                        {
                            CurrentMonthinDays = (CurrentMonthinDays - startDate.Day + 1);
                            OnlyForFirstMonth = false;
                        }
                    }


                    var _with3 = ws.Cells[4, Column - (CurrentMonthinDays - 1), 4, Column];
                    _with3.Merge = true;
                    if (CurrentMonthinDays > 5)
                    {
                        _with3.Value = new System.DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
                    }
                    _with3.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    _with3.Style.Border.Left.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                    _with3.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    _with3.Style.Border.Right.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                    _with3.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    _with3.Style.Border.Top.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                    _with3.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    _with3.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));



                    _with3.Style.Font.Name = "Calibri";
                    _with3.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(51, 51, 51));
                    _with3.Style.Font.Size = 8;
                    _with3.Style.Font.Bold = true;

                    if ((CurrentMonthinDays < 15))
                    {
                        if ((dateTime.Month == CalenderEndDate.Month))
                        {
                            _with3.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        }
                        else
                        {
                            _with3.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        }



                    }
                    else
                    {
                        _with3.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }


                    // .Style.HorizontalAlignment = ExcelHorizontalAlignment.Center



                    _with3.Style.Numberformat.Format = "mmm yyyy";


                    if (dateTime.Month % 2 == 0)
                    {
                        _with3.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        _with3.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(242, 242, 242));
                    }
                }
                else
                {
                    // If they select Last day of the month in Start Date. this will get executed. 

                    if ((startDate == EndMonthDate))
                    {

                        var _with4 = ws.Cells[4, Column - 1, 4, Column - 1];
                        _with4.Merge = true;

                        _with4.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        _with4.Style.Border.Left.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                        _with4.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        _with4.Style.Border.Right.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                        _with4.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        _with4.Style.Border.Top.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                        _with4.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        _with4.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));



                        _with4.Style.Font.Name = "Calibri";
                        _with4.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(51, 51, 51));
                        _with4.Style.Font.Size = 8;
                        _with4.Style.Font.Bold = true;
                        _with4.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        _with4.Style.Numberformat.Format = "mmm yyyy";


                        if (startDate.Month % 2 == 0)
                        {
                            _with4.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            _with4.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(242, 242, 242));
                        }

                    }
                }

            }

            var _with5 = ws.Column(StartColumn);
            _with5.ColumnMax = Column - 1;
            _with5.Width = 3.5;


            return Column - 1;
        }

        public int GenerateMonthlyViewHeader(ExcelWorksheet ws, int StartColumnNo)
        {
            int Column = StartColumnNo;
            int StartColumn = Column;
            ws.Row(2).Height = 12;
            //ws.Row(3).Height = 10
            //ws.Row(4).Height = 10
            ws.Row(3).Height = 12;
            ws.Row(4).Height = 12;
            ws.Row(5).Height = 20;
            ws.Row(6).Height = 20;

            int IsColorOdd = 0;
            Calendar myCalendar = new GregorianCalendar();

            DateTime startDate = new DateTime(CalenderStartDate.Year, CalenderStartDate.Month, CalenderStartDate.Day);
            DateTime stopDate = new DateTime(CalenderEndDate.Year, CalenderEndDate.Month, CalenderEndDate.Day);
            int interval = 1;

            DateTime dateTime = startDate;
            int DaysInMonth = System.DateTime.DaysInMonth(dateTime.Year, dateTime.Month);
            bool OnlyForFirstMonth = true;

            int endweeknum = 0;
            endweeknum = myCalendar.GetWeekOfYear(stopDate, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);


            while (dateTime <= stopDate)
            {
                //With ws.Cells[3, Column)
                var _with1 = ws.Cells[5, Column];
                int weeknum = 0;
                CultureInfo ciCurr = CultureInfo.CurrentCulture;
                int WeekNumber = ciCurr.Calendar.GetWeekOfYear(dateTime, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);
                weeknum = WeekNumber;
                _with1.Value = weeknum;
                _with1.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                _with1.Style.Border.Left.Color.SetColor(System.Drawing.Color.FromArgb(0, 0, 0));
                _with1.Style.Fill.PatternType = ExcelFillStyle.Solid;
                _with1.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(255, 255, 255));
                _with1.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                _with1.Style.Border.Right.Color.SetColor(System.Drawing.Color.FromArgb(0, 0, 0));

                _with1.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                _with1.Style.Border.Top.Color.SetColor(System.Drawing.Color.FromArgb(0, 0, 0));
                _with1.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                _with1.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.FromArgb(0, 0, 0));

                _with1.Style.Font.Name = "arial";
                _with1.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(51, 51, 51));
                _with1.Style.Font.Size = 8;
                _with1.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                _with1.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                _with1.Merge = false;






                //With ws.Cells[4, Column)
                var _with2 = ws.Cells[6, Column];
                int wknum = 0;

                wknum = ciCurr.Calendar.GetWeekOfYear(dateTime, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);


                //wknum = String.Format(DatePart(DateInterval.WeekOfYear, new System.DateTime(dateTime.Year, dateTime.Month, dateTime.Day)));

                int Days = dateTime.DayOfWeek - CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
                Days = (6 + Days) % 7;

                dynamic weekStart = dateTime.AddDays(-Days);
                int currentweeknum = 0;
                currentweeknum = ciCurr.Calendar.GetWeekOfYear(weekStart, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);
                //currentweeknum = String.Format(Date(DateInterval.WeekOfYear, new System.DateTime(weekStart.Year, weekStart.Month, weekStart.Day)));
                //Dim weekStart = dateTime
                dynamic weekEnd = weekStart.AddDays(6);
                if (endweeknum == currentweeknum)
                {
                    //.Value = dateTime.Date.ToString("dd") + "/" + dateTime.Month.ToString() + " - " + weekEnd.Date.ToString("dd") + "/" + weekEnd.Month.ToString()
                    _with2.Value = dateTime.Date.ToString("dd") + "-" + dateTime.ToString("MMM");
                }
                else
                {
                    //.Value = dateTime.Date.ToString("dd") + "/" + dateTime.Month.ToString() + " - " + weekEnd.Date.ToString("dd") + "/" + weekEnd.Month.ToString()
                    _with2.Value = dateTime.Date.ToString("dd") + "-" + dateTime.ToString("MMM");

                }


                //.Value = New Date(dateTime.Year, dateTime.Month, dateTime.Day)
                _with2.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                _with2.Style.Border.Left.Color.SetColor(System.Drawing.Color.FromArgb(0, 0, 0));

                _with2.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                _with2.Style.Border.Right.Color.SetColor(System.Drawing.Color.FromArgb(0, 0, 0));

                _with2.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                _with2.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.FromArgb(0, 0, 0));

                _with2.Style.Font.Name = "arial";
                _with2.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(51, 51, 51));
                _with2.Style.Font.Size = 6;
                _with2.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                //.Style.Numberformat.Format = "dd / mm - dd / mm"


                int CurrentMonthinDays = System.DateTime.DaysInMonth(dateTime.Year, dateTime.Month);
                DateTime EndMonthDate = new DateTime(dateTime.Year, dateTime.Month, CurrentMonthinDays);


                int q = 0;
                q = ((dateTime.Month - 1) / 3) + 1;


                dateTime += TimeSpan.FromDays(interval);
                Column = Column + 1;


                if (((EndMonthDate == dateTime | stopDate == dateTime | startDate == dateTime) & (stopDate >= dateTime)))
                {
                    if ((stopDate == dateTime) & EndMonthDate != dateTime)
                    {
                        CurrentMonthinDays = stopDate.Day;
                    }

                    if ((OnlyForFirstMonth == true))
                    {
                        if ((startDate.Day != 1 & startDate.Month == dateTime.Month & startDate.Year == dateTime.Year))
                        {
                            CurrentMonthinDays = (CurrentMonthinDays - startDate.Day + 1);
                            OnlyForFirstMonth = false;
                        }
                    }



                    //With ws.Cells[2, Column - (CurrentMonthinDays - 1), 2, Column)
                    var _with3 = ws.Cells[4, Column - (CurrentMonthinDays - 1), 4, Column];
                    _with3.Merge = true;
                    if (CurrentMonthinDays > 5)
                    {
                        _with3.Value = new System.DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
                    }
                    _with3.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    _with3.Style.Border.Left.Color.SetColor(System.Drawing.Color.FromArgb(0, 0, 0));

                    _with3.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    _with3.Style.Border.Right.Color.SetColor(System.Drawing.Color.FromArgb(0, 0, 0));

                    _with3.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    _with3.Style.Border.Top.Color.SetColor(System.Drawing.Color.FromArgb(0, 0, 0));

                    _with3.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    _with3.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.FromArgb(0, 0, 0));



                    _with3.Style.Font.Name = "arial";
                    _with3.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(51, 51, 51));
                    _with3.Style.Font.Size = 10;
                    _with3.Style.Font.Bold = false;




                    if ((CurrentMonthinDays < 15))
                    {
                        if ((dateTime.Month == CalenderEndDate.Month))
                        {
                            _with3.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        }
                        else
                        {
                            _with3.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        }


                    }
                    else
                    {
                        _with3.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }


                    // .Style.HorizontalAlignment = ExcelHorizontalAlignment.Center



                    _with3.Style.Numberformat.Format = "mmm-yy";
                    //---Background generating based on quarter

                    //_with3.Style.Numberformat.Format = "mmm-yy";


                    if (IsColorOdd == 0)
                    {
                        _with3.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        _with3.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(255, 255, 255));
                        IsColorOdd = 1;
                    }
                    else
                    {
                        _with3.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        _with3.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(242, 242, 242));
                        IsColorOdd = 0;
                    }




                    ///''''''''''''''''''''''''''''''''''''''''''''''''''


                    var _with4 = ws.Cells[3, Column - (CurrentMonthinDays - 1), 3, Column];
                    //.Merge = True
                    string msg = "";
                    //If CurrentMonthinDays > 5 Then
                    //.Value = New Date(dateTime.Year, dateTime.Month, dateTime.Day)
                    q = ((dateTime.Month - 1) / 3) + 1;
                    _with4.Value = "Q" + q;
                    msg = "Q" + q;
                    //End If
                    _with4.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    _with4.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    _with4.Style.Border.Right.Color.SetColor(System.Drawing.Color.FromArgb(0, 0, 0));
                    _with4.Style.WrapText = true;
                    _with4.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    _with4.Style.Border.Top.Color.SetColor(System.Drawing.Color.FromArgb(0, 0, 0));

                    _with4.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    _with4.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.FromArgb(0, 0, 0));



                    _with4.Style.Font.Name = "arial";
                    _with4.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(51, 51, 51));
                    _with4.Style.Font.Size = 10;
                    _with4.Style.Font.Bold = false;




                    if ((CurrentMonthinDays < 15))
                    {
                        if ((dateTime.Month == CalenderEndDate.Month))
                        {
                            _with4.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        }
                        else
                        {
                            _with4.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        }



                    }
                    else
                    {
                        _with4.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }


                    ///''''''''''''''''''''''''''''''''''''''''''''''''''''

                }
                else
                {
                    // If they select Last day of the month in Start Date. this will get executed. 

                    if ((startDate == EndMonthDate))
                    {

                        //With ws.Cells[2, Column - 1, 2, Column - 1)
                        var _with5 = ws.Cells[4, Column - 1, 4, Column - 1];
                        _with5.Merge = true;

                        _with5.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        _with5.Style.Border.Left.Color.SetColor(System.Drawing.Color.FromArgb(0, 0, 0));

                        _with5.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        _with5.Style.Border.Right.Color.SetColor(System.Drawing.Color.FromArgb(0, 0, 0));

                        _with5.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        _with5.Style.Border.Top.Color.SetColor(System.Drawing.Color.FromArgb(0, 0, 0));

                        _with5.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        _with5.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.FromArgb(0, 0, 0));



                        _with5.Style.Font.Name = "arial";
                        _with5.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(51, 51, 51));
                        _with5.Style.Font.Size = 10;
                        _with5.Style.Font.Bold = false;
                        _with5.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        _with5.Style.Numberformat.Format = "mmm-yy";

                        if (IsColorOdd == 0)
                        {
                            _with5.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            _with5.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(255, 255, 255));
                            IsColorOdd = 1;
                        }
                        else
                        {
                            _with5.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            _with5.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(242, 242, 242));
                            IsColorOdd = 0;
                        }




                        ///'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                        var _with6 = ws.Cells[3, Column - 1, 3, Column - 1];
                        string msg = "";
                        q = ((dateTime.Month - 1) / 3) + 1;
                        _with6.Value = "Q" + q;
                        msg = "Q" + q;
                        //End If
                        _with6.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        _with6.Style.Border.Right.Color.SetColor(System.Drawing.Color.FromArgb(0, 0, 0));
                        _with6.Style.WrapText = true;
                        _with6.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        _with6.Style.Border.Top.Color.SetColor(System.Drawing.Color.FromArgb(0, 0, 0));

                        _with6.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        _with6.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.FromArgb(0, 0, 0));



                        _with6.Style.Font.Name = "arial";
                        _with6.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(51, 51, 51));
                        _with6.Style.Font.Size = 10;
                        _with6.Style.Font.Bold = false;


                        ///'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                    }
                }


            }

            var _with7 = ws.Column(StartColumn);
            _with7.ColumnMax = Column - 1;
            _with7.Width = 3.5;


            return Column - 1;
        }

        public void GenerateTypeIcon(ExcelWorksheet ws, string SD, string ccode, int startcolumnno, int endcolumnno, int rowno, int level)
        {
            int correctLevelcell = level + 1;

            //drawing the shortdescription with colorcode background

            var _with2 = ws.Cells[RowNo, correctLevelcell];
            _with2.Value = SD;
            _with2.Style.Fill.PatternType = ExcelFillStyle.Solid;
            _with2.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#" + ccode));
            _with2.Style.Font.Name = "arial";
            _with2.Style.Font.Color.SetColor(Color.White);
            _with2.Style.Font.Size = 8;
            _with2.Style.Font.Bold = true;
            _with2.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            _with2.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            _with2.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            _with2.Style.Border.Left.Color.SetColor(System.Drawing.Color.FromArgb(217, 217, 217));
            _with2.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            _with2.Style.Border.Right.Color.SetColor(System.Drawing.Color.FromArgb(217, 217, 217));
            _with2.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            _with2.Style.Border.Top.Color.SetColor(System.Drawing.Color.FromArgb(217, 217, 217));
            _with2.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            _with2.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.FromArgb(217, 217, 217));


            //check before any cell to merge
            if (correctLevelcell > 1)
            {
                //cells before icon 
                var cellsbefore = ws.Cells[rowno, startcolumnno, rowno, correctLevelcell - 1];
                cellsbefore.Merge = true;
                cellsbefore.Value = "";
                if (level == 0)
                {
                    cellsbefore.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    cellsbefore.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(217, 217, 217));
                }
                cellsbefore.Style.Font.Name = "arial";
                cellsbefore.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(51, 51, 51));
                cellsbefore.Style.Font.Size = 10;
                cellsbefore.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                cellsbefore.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                cellsbefore.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                cellsbefore.Style.Border.Left.Color.SetColor(System.Drawing.Color.FromArgb(217, 217, 217));
                cellsbefore.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                cellsbefore.Style.Border.Right.Color.SetColor(System.Drawing.Color.FromArgb(217, 217, 217));
                cellsbefore.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                cellsbefore.Style.Border.Top.Color.SetColor(System.Drawing.Color.FromArgb(217, 217, 217));
                cellsbefore.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                cellsbefore.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.FromArgb(217, 217, 217));
            }
            //cells after icon 
            var cellsafter = ws.Cells[rowno, correctLevelcell + 1, rowno, endcolumnno];
            cellsafter.Merge = true;
            //cellsafter.Value = "";
            if (level == 0)
            {
                cellsafter.Style.Fill.PatternType = ExcelFillStyle.Solid;
                cellsafter.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(217, 217, 217));
            }
            cellsafter.Style.Font.Name = "arial";
            cellsafter.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(51, 51, 51));
            cellsafter.Style.Font.Size = 10;
            cellsafter.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            cellsafter.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cellsafter.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            cellsafter.Style.Border.Left.Color.SetColor(System.Drawing.Color.FromArgb(217, 217, 217));
            cellsafter.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            cellsafter.Style.Border.Right.Color.SetColor(System.Drawing.Color.FromArgb(217, 217, 217));
            cellsafter.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            cellsafter.Style.Border.Top.Color.SetColor(System.Drawing.Color.FromArgb(217, 217, 217));
            cellsafter.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            cellsafter.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.FromArgb(217, 217, 217));

        }

        public int GenerateGanttView(ExcelWorksheet ws, IListofRecord Data)
        {


            ////code align


            IList<AttributeSettings> listColumnDefsdata = Data.ColumnDefs;
            IList listContent = Data.Data;
            int columnlen = listColumnDefsdata.Count;
            int listlen = listContent.Count;

            for (int i = 0; i < listlen; i++)
            {

                for (int j = 1; j <= 10; j++)
                {
                    var namecell = ws.Cells[RowNo, j];
                    var cellnamevalues = ((System.Collections.Hashtable)(listContent)[i])["Name"];
                    namecell.Value = HttpUtility.HtmlDecode(cellnamevalues.ToString());

                    namecell.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    namecell.Style.Border.Left.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                    namecell.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    namecell.Style.Border.Right.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                    namecell.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    namecell.Style.Border.Top.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                    namecell.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    namecell.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                    if ((int)((System.Collections.Hashtable)(listContent)[i])["Level"] == 0)
                    {
                        namecell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        namecell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(217, 217, 217));
                    }

                    namecell.Style.Font.Name = "arial";

                    namecell.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(51, 51, 51));
                    namecell.Style.Font.Size = 10;

                    namecell.Style.Font.Name = "arial";
                    namecell.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(51, 51, 51));
                    namecell.Style.Font.Size = 10;

                    namecell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    namecell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    //namecell.Style.Indent = ((int)((System.Collections.Hashtable)(listContent)[i])["Level"]);
                }


                var _with1 = ws.Cells[RowNo, 11];


                //.Value = ETResult.ToList()(0).FriendlyName

                var namevalues = ((System.Collections.Hashtable)(listContent)[i])["Name"];
                _with1.Value = HttpUtility.HtmlDecode(namevalues.ToString());

                _with1.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                _with1.Style.Border.Left.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                _with1.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                _with1.Style.Border.Right.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                _with1.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                _with1.Style.Border.Top.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                _with1.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                _with1.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));




                if ((int)((System.Collections.Hashtable)(listContent)[i])["Level"] == 0)
                {
                    _with1.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    _with1.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(217, 217, 217));
                }

                _with1.Style.Font.Name = "arial";

                _with1.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(51, 51, 51));
                _with1.Style.Font.Size = 10;

                _with1.Style.Font.Name = "arial";
                _with1.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(51, 51, 51));
                _with1.Style.Font.Size = 10;

                _with1.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                _with1.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                _with1.Style.Indent = ((int)((System.Collections.Hashtable)(listContent)[i])["Level"]);

                GenerateTypeIcon(ws, (string)((System.Collections.Hashtable)(listContent)[i])["ShortDescription"], (string)((System.Collections.Hashtable)(listContent)[i])["ColorCode"], 1, 11, RowNo, (int)((System.Collections.Hashtable)(listContent)[i])["Level"]);


                var _with2 = ws.Cells[RowNo, 12];
                //.Value = Item.Name
                //AddIconEntitytype(ws, 1, RowNo - 1, (int)((System.Collections.Hashtable)(listContent)[i])["Level"], (string)((System.Collections.Hashtable)(listContent)[i])["ColorCode"], (string)((System.Collections.Hashtable)(listContent)[i])["ShortDescription"]);

                _with2.Value = (int)((System.Collections.Hashtable)(listContent)[i])["Id"];
                _with2.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                _with2.Style.Border.Left.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                _with2.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                _with2.Style.Border.Right.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                _with2.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                _with2.Style.Border.Top.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                _with2.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                _with2.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));
                if ((int)((System.Collections.Hashtable)(listContent)[i])["Level"] == 0)
                {
                    _with2.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    _with2.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(217, 217, 217));
                }

                _with2.Style.Font.Name = "arial";
                _with2.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(51, 51, 51));
                _with2.Style.Font.Size = 10;


                _with2.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                _with2.Style.VerticalAlignment = ExcelVerticalAlignment.Center;



                int StartDynColNo = 13;

                for (int j = 0; j <= columnlen - 1; j++)
                {
                    if (listColumnDefsdata[j].Field != "68" && listColumnDefsdata[j].Type != 10)
                    {
                        var _with3 = ws.Cells[RowNo, StartDynColNo];
                        //.Value = Item.Name
                        _with3.Value = (((System.Collections.Hashtable)(listContent)[i])[listColumnDefsdata[j].Field] != null ? ((System.Collections.Hashtable)(listContent)[i])[listColumnDefsdata[j].Field] : "-");
                        _with3.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        _with3.Style.Border.Left.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                        _with3.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        _with3.Style.Border.Right.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                        _with3.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        _with3.Style.Border.Top.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                        _with3.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        _with3.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                        if ((int)((System.Collections.Hashtable)(listContent)[i])["Level"] == 0)
                        {
                            _with3.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            _with3.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(217, 217, 217));
                        }

                        _with3.Style.Font.Name = "arial";
                        _with3.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(51, 51, 51));
                        _with3.Style.Font.Size = 10;


                        _with3.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        _with3.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                        StartDynColNo = StartDynColNo + 1;



                    }
                }

                //Dheerak going to End Here //


                if (((int)((System.Collections.Hashtable)(listContent)[i])["Level"] == 0))
                {
                    var _with31 = ws.Cells[RowNo, ColumnNo, RowNo, LastColumnNo];
                    _with31.Merge = true;
                    _with31.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    _with31.Style.Border.Left.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                    _with31.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    _with31.Style.Border.Right.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                    _with31.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    _with31.Style.Border.Top.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));


                    _with31.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    _with31.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(217, 217, 217));

                    //milestone details will come here
                    string xmlMilestone = (string)((System.Collections.Hashtable)(listContent)[i])["MileStone"];
                    XmlDocument doc = new XmlDocument();

                    if (xmlMilestone != null && xmlMilestone.Length > 0)
                    {
                        doc.LoadXml(xmlMilestone);
                        XmlNodeList milestones = doc.DocumentElement.SelectNodes("//p");
                        ExcelComment Comment = default(ExcelComment);
                        MilestoeList = "";
                        foreach (XmlNode node in milestones)
                        {

                            System.DateTime StartDate = DateTime.Parse(node.Attributes["s"].Value);
                            int MileStartColumn = StartDate.Subtract(CalenderStartDate).Days + ColumnNo;
                            if (MileStartColumn < ColumnNo)
                            {
                                MileStartColumn = ColumnNo;
                            }
                            try
                            {

                                if (node.Attributes["ms"].Value == "0")
                                {
                                    AddImage(ws, MileStartColumn - 1, RowNo - 1, System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "assets\\img\\star.png");
                                    MilestoeList = MilestoeList + "Name: " + HttpUtility.HtmlDecode(node.Attributes["n"].Value) + Environment.NewLine + "Description: " + HttpUtility.HtmlDecode(node.Attributes["d"].Value) + Environment.NewLine + "Due Date: " + node.Attributes["s"].Value + Environment.NewLine + "Status: Not reached" + Environment.NewLine + Environment.NewLine;
                                    // MilestoeList =  MakeBold("Name: ","Name: ") + HttpUtility.HtmlDecode(node.Attributes["n"].Value) + Environment.NewLine + "Description: " + HttpUtility.HtmlDecode(node.Attributes["d"].Value) + Environment.NewLine + "Due Date: " + node.Attributes["s"].Value + Environment.NewLine + "Status: Not reached" + Environment.NewLine + Environment.NewLine;
                                    Comment = default(ExcelComment);
                                    Comment = ws.Comments.Add(ws.Cells[RowNo, MileStartColumn], MilestoeList + Environment.NewLine, "Marcom Platform");
                                    Comment.Font.FontName = "arial";
                                    Comment.Font.Size = 10;
                                    Comment.From.Column = 1;
                                    Comment.From.Row = 1;
                                    Comment.To.Column = 3;
                                    Comment.To.Row = 5;
                                    Comment.BackgroundColor = System.Drawing.Color.FromArgb(231, 242, 245);
                                }
                                else
                                {
                                    AddImage(ws, MileStartColumn - 1, RowNo - 1, System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "assets\\img\\starGreen.png");
                                    // var mid1 = node.Attributes["ms"].Value;
                                    MilestoeList = MilestoeList + "Name: " + HttpUtility.HtmlDecode(node.Attributes["n"].Value) + Environment.NewLine + "Description: " + HttpUtility.HtmlDecode(node.Attributes["d"].Value) + Environment.NewLine + "Due Date: " + node.Attributes["s"].Value + Environment.NewLine + "Status: Reached" + Environment.NewLine + Environment.NewLine;
                                    //MilestoeList = MakeBold("Name: ", "Name: ") + HttpUtility.HtmlDecode(node.Attributes["n"].Value) + Environment.NewLine + "Description: " + HttpUtility.HtmlDecode(node.Attributes["d"].Value) + Environment.NewLine + "Due Date: " + node.Attributes["s"].Value + Environment.NewLine + "Status: Reached" + Environment.NewLine + Environment.NewLine;
                                    Comment = default(ExcelComment);
                                    Comment = ws.Comments.Add(ws.Cells[RowNo, MileStartColumn], MilestoeList + Environment.NewLine, "Marcom Platform");
                                    Comment.Font.FontName = "arial";
                                    Comment.Font.Size = 10;
                                    Comment.From.Column = 1;
                                    Comment.From.Row = 1;
                                    Comment.To.Column = 3;
                                    Comment.To.Row = 5;
                                    Comment.BackgroundColor = System.Drawing.Color.FromArgb(231, 242, 245);
                                }

                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }


                    int StartColumnNo = ColumnNo;
                    int EndColumnNo = ColumnNo;
                    switch (new System.DateTime(CalenderStartDate.Year, CalenderStartDate.Month, CalenderStartDate.Day).DayOfWeek)
                    {
                        // need to take StartDate

                        case System.DayOfWeek.Monday:
                            EndColumnNo = EndColumnNo + 6;
                            break;
                        case System.DayOfWeek.Tuesday:
                            EndColumnNo = EndColumnNo + 5;
                            break;
                        case System.DayOfWeek.Wednesday:
                            EndColumnNo = EndColumnNo + 4;
                            break;
                        case System.DayOfWeek.Thursday:
                            EndColumnNo = EndColumnNo + 3;
                            break;
                        case System.DayOfWeek.Friday:
                            EndColumnNo = EndColumnNo + 2;
                            break;
                        case System.DayOfWeek.Saturday:
                            EndColumnNo = EndColumnNo + 1;
                            break;
                        case System.DayOfWeek.Sunday:
                            EndColumnNo = EndColumnNo + 0;
                            break;
                    }

                    var _with32 = ws.Cells[5, StartColumnNo, 5, EndColumnNo];
                    _with32.Merge = true;
                    _with32.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    _with32.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    var _with33 = ws.Cells[6, StartColumnNo, 6, EndColumnNo];
                    _with33.Merge = true;
                    _with33.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    _with33.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    bool IsOdd = true;

                    while (StartColumnNo <= LastColumnNo)
                    {


                        var _with34 = ws.Cells[RowNo, StartColumnNo, RowNo, EndColumnNo];
                        // .Style.Border.Left.Style = ExcelBorderStyle.Thin
                        //.Style.Border.Left.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165))
                        if (EndColumnNo - StartColumnNo <= 1)
                        {
                            _with34.AutoFitColumns(3.0);
                        }
                        else
                        {
                            _with34.AutoFitColumns(1.0);
                        }
                        //.Style.Border.Right.Style = ExcelBorderStyle.Thin
                        //.Style.Border.Right.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165))

                        _with34.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        _with34.Style.Border.Top.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                        _with34.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        _with34.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                        //.Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.FromArgb(165, 165, 165))

                        if (IsOdd)
                        {
                            _with34.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            _with34.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(242, 242, 242));
                            // .Merge = True




                            var _with35 = ws.Cells[5, StartColumnNo, 5, EndColumnNo];
                            _with35.Merge = true;
                            _with35.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            _with35.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                            var _with36 = ws.Cells[6, StartColumnNo, 6, EndColumnNo];
                            _with36.Merge = true;
                            _with36.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            _with36.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                            IsOdd = false;

                        }
                        else
                        {

                            var _with37 = ws.Cells[5, StartColumnNo, 5, EndColumnNo];
                            _with37.Merge = true;
                            _with37.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            _with37.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                            var _with38 = ws.Cells[6, StartColumnNo, 6, EndColumnNo];
                            _with38.Merge = true;
                            _with38.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            _with38.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                            IsOdd = true;
                            //.Merge = True
                        }


                        var _with39 = ws.Cells[RowNo, EndColumnNo, RowNo, EndColumnNo];
                        _with39.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        _with39.Style.Border.Right.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                        StartColumnNo = EndColumnNo + 1;
                        EndColumnNo = StartColumnNo + 6;

                        if (EndColumnNo > LastColumnNo)
                        {
                            EndColumnNo = LastColumnNo;
                        }
                    }


                }
                else
                {

                    int StartColumnNo = ColumnNo;
                    int EndColumnNo = ColumnNo;


                    switch (new System.DateTime(CalenderStartDate.Year, CalenderStartDate.Month, CalenderStartDate.Day).DayOfWeek)
                    {
                        // need to take StartDate

                        case System.DayOfWeek.Monday:
                            EndColumnNo = EndColumnNo + 6;
                            break;
                        case System.DayOfWeek.Tuesday:
                            EndColumnNo = EndColumnNo + 5;
                            break;
                        case System.DayOfWeek.Wednesday:
                            EndColumnNo = EndColumnNo + 4;
                            break;
                        case System.DayOfWeek.Thursday:
                            EndColumnNo = EndColumnNo + 3;
                            break;
                        case System.DayOfWeek.Friday:
                            EndColumnNo = EndColumnNo + 2;
                            break;
                        case System.DayOfWeek.Saturday:
                            EndColumnNo = EndColumnNo + 1;
                            break;
                        case System.DayOfWeek.Sunday:
                            EndColumnNo = EndColumnNo + 0;
                            break;
                    }

                    var _with40 = ws.Cells[5, StartColumnNo, 5, EndColumnNo];
                    _with40.Merge = true;
                    _with40.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    _with40.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    var _with41 = ws.Cells[6, StartColumnNo, 6, EndColumnNo];
                    _with41.Merge = true;
                    _with41.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    _with41.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    bool IsOdd = true;

                    while (StartColumnNo <= LastColumnNo)
                    {


                        var _with42 = ws.Cells[RowNo, StartColumnNo, RowNo, EndColumnNo];
                        // .Style.Border.Left.Style = ExcelBorderStyle.Thin
                        //.Style.Border.Left.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165))
                        if (EndColumnNo - StartColumnNo <= 1)
                        {
                            _with42.AutoFitColumns(3.0);
                        }
                        else
                        {
                            _with42.AutoFitColumns(1.0);
                        }
                        //.Style.Border.Right.Style = ExcelBorderStyle.Thin
                        //.Style.Border.Right.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165))

                        _with42.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        _with42.Style.Border.Top.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                        _with42.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        _with42.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                        //.Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.FromArgb(165, 165, 165))

                        if (IsOdd)
                        {
                            _with42.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            _with42.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(242, 242, 242));
                            // .Merge = True




                            //With ws.Cells[5, StartColumnNo, 5, EndColumnNo]
                            //    .Merge = True
                            //    .Style.HorizontalAlignment = ExcelHorizontalAlignment.Center
                            //    .Style.VerticalAlignment = ExcelVerticalAlignment.Center
                            //End With
                            var _with43 = ws.Cells[6, StartColumnNo, 6, EndColumnNo];
                            _with43.Merge = true;
                            _with43.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            _with43.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            IsOdd = false;

                        }
                        else
                        {

                            //With ws.Cells[5, StartColumnNo, 5, EndColumnNo]
                            //    .Merge = True
                            //    .Style.HorizontalAlignment = ExcelHorizontalAlignment.Center
                            //    .Style.VerticalAlignment = ExcelVerticalAlignment.Center
                            //End With
                            var _with44 = ws.Cells[6, StartColumnNo, 6, EndColumnNo];
                            _with44.Merge = true;
                            _with44.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            _with44.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            IsOdd = true;
                            //.Merge = True
                        }


                        var _with45 = ws.Cells[RowNo, EndColumnNo, RowNo, EndColumnNo];
                        _with45.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        _with45.Style.Border.Right.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                        StartColumnNo = EndColumnNo + 1;
                        EndColumnNo = StartColumnNo + 6;

                        if (EndColumnNo > LastColumnNo)
                        {
                            EndColumnNo = LastColumnNo;
                        }
                    }


                    //milestone data will come here
                    string xmlMilestone = (string)((System.Collections.Hashtable)(listContent)[i])["MileStone"];
                    XmlDocument doc = new XmlDocument();

                    if (xmlMilestone != null && xmlMilestone.Length > 0)
                    {
                        doc.LoadXml(xmlMilestone);
                        XmlNodeList milestones = doc.DocumentElement.SelectNodes("//p");
                        ExcelComment Comment = default(ExcelComment);
                        MilestoeList = "";
                        foreach (XmlNode node in milestones)
                        {

                            System.DateTime StartDate = DateTime.Parse(node.Attributes["s"].Value);
                            int MileStartColumn = StartDate.Subtract(CalenderStartDate).Days + ColumnNo;
                            if (MileStartColumn < ColumnNo)
                            {
                                MileStartColumn = ColumnNo;
                            }
                            try
                            {

                                if (node.Attributes["ms"].Value == "0")
                                {
                                    AddImage(ws, MileStartColumn - 1, RowNo - 1, System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "assets\\img\\star.png");
                                    MilestoeList = MilestoeList + "Name: " + HttpUtility.HtmlDecode(node.Attributes["n"].Value) + Environment.NewLine + "Description: " + HttpUtility.HtmlDecode(node.Attributes["d"].Value) + Environment.NewLine + "Due Date: " + node.Attributes["s"].Value + Environment.NewLine + "Status: Not reached" + Environment.NewLine + Environment.NewLine;
                                    Comment = default(ExcelComment);
                                    Comment = ws.Comments.Add(ws.Cells[RowNo, MileStartColumn], MilestoeList + Environment.NewLine, "Marcom Platform");
                                    Comment.Font.FontName = "arial";
                                    Comment.Font.Size = 10;
                                    Comment.From.Column = 1;
                                    Comment.From.Row = 1;
                                    Comment.To.Column = 10;
                                    Comment.To.Row = 15;
                                    Comment.BackgroundColor = System.Drawing.Color.FromArgb(231, 242, 245);
                                }
                                else
                                {
                                    AddImage(ws, MileStartColumn - 1, RowNo - 1, System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "assets\\img\\starGreen.png");
                                    MilestoeList = MilestoeList + "Name: " + HttpUtility.HtmlDecode(node.Attributes["n"].Value) + Environment.NewLine + "Description: " + HttpUtility.HtmlDecode(node.Attributes["d"].Value) + Environment.NewLine + "Due Date: " + node.Attributes["s"].Value + Environment.NewLine + "Status: Reached" + Environment.NewLine + Environment.NewLine;
                                    Comment = default(ExcelComment);
                                    Comment = ws.Comments.Add(ws.Cells[RowNo, MileStartColumn], MilestoeList + Environment.NewLine, "Marcom Platform");
                                    Comment.Font.FontName = "arial";
                                    Comment.Font.Size = 10;
                                    Comment.From.Column = 1;
                                    Comment.From.Row = 1;
                                    Comment.To.Column = 10;
                                    Comment.To.Row = 15;
                                    Comment.BackgroundColor = System.Drawing.Color.FromArgb(231, 242, 245);
                                }

                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }


                    string xmlString = (string)((System.Collections.Hashtable)(listContent)[i])["Period"];
                    //Create an XML Document and load your XML
                    doc = new XmlDocument();
                    if (xmlString != null && xmlString.Length > 0)
                    {
                        doc.LoadXml(xmlString);

                        //Get  nodes
                        XmlNodeList nodes = doc.DocumentElement.SelectNodes("//p");


                        //Iterates through your String appending the available Names
                        foreach (XmlNode node in nodes)
                        {

                            System.DateTime StartDate = DateTime.Parse(node.Attributes["s"].Value);
                            System.DateTime EndDate = DateTime.Parse(node.Attributes["e"].Value);
                            string Startdatecomment = node.Attributes["d"].Value;

                            CultureInfo ciCurr = CultureInfo.CurrentCulture;

                            //Calendar myCalendar = new GregorianCalendar();
                            //int EndWeekNumber = myCalendar.GetWeekOfYear(EndDate, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);
                            int EndWeekNumber = ciCurr.Calendar.GetWeekOfYear(EndDate, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);

                            //  int startweeknum = myCalendar.GetWeekOfYear(StartDate, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);
                            int startweeknum = ciCurr.Calendar.GetWeekOfYear(StartDate, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);
                            int BarStartColumn = StartDate.Subtract(CalenderStartDate).Days + ColumnNo;
                            int BarEndColumn = EndDate.Subtract(StartDate).Days + BarStartColumn;

                            int endcolmn = EndWeekNumber + startweeknum;

                            if (BarStartColumn < ColumnNo)
                            {
                                BarStartColumn = ColumnNo;
                            }

                            if (BarEndColumn > LastColumnNo)
                            {
                                BarEndColumn = LastColumnNo;
                            }

                            if (BarStartColumn < LastColumnNo & BarEndColumn > ColumnNo)
                            {

                                try
                                {


                                    var _with46 = ws.Cells[RowNo, BarStartColumn, RowNo, BarEndColumn];
                                    _with46.Merge = true;
                                    _with46.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    _with46.Style.Border.Left.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                                    _with46.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    _with46.Style.Border.Right.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                                    _with46.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    _with46.Style.Border.Top.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                                    _with46.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    _with46.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                                    _with46.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    System.Drawing.Color colorcode = default(System.Drawing.Color);

                                    _with46.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(HexStrToBase10Int((string)((System.Collections.Hashtable)(listContent)[i])["ColorCode"])));


                                    XmlNodeList milestones;
                                    //check milestone data count then put
                                    if (xmlMilestone != null)
                                    {
                                        doc.LoadXml(xmlMilestone);
                                        milestones = doc.DocumentElement.SelectNodes("//p");
                                    }
                                    else
                                    {
                                        milestones = null;
                                    }
                                    ExcelComment barComment = default(ExcelComment);
                                    if (milestones != null)
                                    {

                                        if (milestones.Count == 0)
                                        {
                                            MilestoeList = "-";
                                            barComment = ws.Comments.Add(ws.Cells[RowNo, BarStartColumn, RowNo, BarEndColumn], "Name: " + HttpUtility.HtmlDecode(namevalues.ToString()) + Environment.NewLine + "Start Date: " + StartDate.ToString("yyyy-MM-dd") + Environment.NewLine + "End Date: " + EndDate.ToString("yyyy-MM-dd") + Environment.NewLine + "Start/End Comment: " + HttpUtility.HtmlDecode(Startdatecomment.ToString()) + Environment.NewLine, "Marcom Platform");
                                            // barComment = ws.Comments.Add(ws.Cells[RowNo, BarStartColumn, RowNo, BarEndColumn], "Name: " + HttpUtility.HtmlDecode(node.Attributes["d"].Value) + Environment.NewLine + "Start Date: " + StartDate.ToString("yyyy-MM-dd") + Environment.NewLine + "End Date: " + EndDate.ToString("yyyy-MM-dd") + Environment.NewLine + "Start/End Comment: " + HttpUtility.HtmlDecode(Startdatecomment.ToString()) + Environment.NewLine, "Marcom Platform");


                                        }
                                        else if (milestones.Count > 0)
                                        {
                                            barComment = ws.Comments.Add(ws.Cells[RowNo, BarStartColumn, RowNo, BarEndColumn], "Name: " + HttpUtility.HtmlDecode(namevalues.ToString()) + Environment.NewLine + "Start Date: " + StartDate.ToString("yyyy-MM-dd") + Environment.NewLine + "End Date: " + EndDate.ToString("yyyy-MM-dd") + Environment.NewLine + "Start/End Comment: " + HttpUtility.HtmlDecode(Startdatecomment.ToString()) + Environment.NewLine + "MileStone Details: " + Environment.NewLine + MilestoeList + Environment.NewLine, "Marcom Platform");
                                            //MilestoeList = "";
                                        }
                                        //barComment = ws.Comments.Add(ws.Cells[RowNo, BarStartColumn, RowNo, BarEndColumn], "Name: " + HttpUtility.HtmlDecode(node.Attributes["d"].Value) + Environment.NewLine + "Start Date: " + StartDate.ToString("yyyy-MM-dd") + Environment.NewLine + "End Date: " + EndDate.ToString("yyyy-MM-dd") + Environment.NewLine + "Start/End Comment: " + HttpUtility.HtmlDecode(Startdatecomment.ToString()) + Environment.NewLine + "MileStone Details: " + MilestoeList + Environment.NewLine, "Marcom Platform");
                                        else
                                            barComment = ws.Comments.Add(ws.Cells[RowNo, BarStartColumn, RowNo, BarEndColumn], "Name: " + HttpUtility.HtmlDecode(namevalues.ToString()) + Environment.NewLine + "Start Date: " + StartDate.ToString("yyyy-MM-dd") + Environment.NewLine + "End Date: " + EndDate.ToString("yyyy-MM-dd") + Environment.NewLine + "Start/End Comment: " + HttpUtility.HtmlDecode(Startdatecomment.ToString()) + Environment.NewLine, "Marcom Platform");
                                        //barComment = ws.Comments.Add(ws.Cells[RowNo, BarStartColumn, RowNo, BarEndColumn], "Name: " + HttpUtility.HtmlDecode(node.Attributes["d"].Value) + Environment.NewLine + "Start Date: " + StartDate.ToString("yyyy-MM-dd") + Environment.NewLine + "End Date: " + EndDate.ToString("yyyy-MM-dd") + Environment.NewLine + "Start/End Comment: " + HttpUtility.HtmlDecode(Startdatecomment.ToString()) + Environment.NewLine, "Marcom Platform");
                                        barComment.Font.FontName = "Calibri";
                                        barComment.Font.Size = 10;
                                        barComment.From.Column = 1;
                                        barComment.From.Row = 1;
                                        barComment.To.Column = 10;
                                        if (milestones.Count == 0)
                                        {
                                            barComment.To.Row = 12;
                                        }
                                        else if (milestones.Count == 1)
                                        {
                                            barComment.To.Row = 14 + (milestones.Count * 4);
                                        }
                                        else
                                        {
                                            barComment.To.Row = 14 + (milestones.Count * 4);
                                        }
                                        barComment.BackgroundColor = System.Drawing.Color.FromArgb(231, 242, 245);
                                    }
                                    else
                                    {

                                        // barComment = ws.Comments.Add(ws.Cells[RowNo, BarStartColumn, RowNo, BarEndColumn], "Name: " + HttpUtility.HtmlDecode(node.Attributes["d"].Value) + Environment.NewLine + "Start Date: " + StartDate.ToString("yyyy-MM-dd") + Environment.NewLine + "End Date: " + EndDate.ToString("yyyy-MM-dd") + Environment.NewLine + "Start/End Comment: " + HttpUtility.HtmlDecode(Startdatecomment.ToString()) + Environment.NewLine, "Marcom Platform");
                                        barComment = ws.Comments.Add(ws.Cells[RowNo, BarStartColumn, RowNo, BarEndColumn], "Name: " + HttpUtility.HtmlDecode(namevalues.ToString()) + Environment.NewLine + "Start Date: " + StartDate.ToString("yyyy-MM-dd") + Environment.NewLine + "End Date: " + EndDate.ToString("yyyy-MM-dd") + Environment.NewLine + "Start/End Comment: " + HttpUtility.HtmlDecode(Startdatecomment.ToString()) + Environment.NewLine, "Marcom Platform");
                                        barComment.Font.FontName = "Calibri";
                                        barComment.Font.Size = 10;
                                        barComment.From.Column = 1;
                                        barComment.From.Row = 1;
                                        barComment.To.Column = 10;
                                        barComment.To.Row = 12;
                                        barComment.BackgroundColor = System.Drawing.Color.FromArgb(231, 242, 245);
                                    }

                                }
                                catch (Exception ex)
                                {
                                    //Log("Date overlaping for", Item.Name);

                                }
                            }




                        }
                    }
                }
                RowNo++;
            }


            return 0;
        }

        public int GenerateMonthlyGanttView(ExcelWorksheet ws, IListofRecord Data)
        {

            ////code align
            IList<AttributeSettings> listColumnDefsdata = Data.ColumnDefs;
            IList listContent = Data.Data;
            int columnlen = listColumnDefsdata.Count;
            int listlen = listContent.Count;

            for (int i = 0; i < listlen; i++)
            {

                for (int j = 1; j <= 10; j++)
                {
                    var namecell = ws.Cells[RowNo, j];
                    var cellnamevalues = ((System.Collections.Hashtable)(listContent)[i])["Name"];
                    namecell.Value = HttpUtility.HtmlDecode(cellnamevalues.ToString());

                    namecell.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    namecell.Style.Border.Left.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                    namecell.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    namecell.Style.Border.Right.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                    namecell.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    namecell.Style.Border.Top.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                    namecell.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    namecell.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                    if ((int)((System.Collections.Hashtable)(listContent)[i])["Level"] == 0)
                    {
                        namecell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        namecell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(217, 217, 217));
                    }

                    namecell.Style.Font.Name = "arial";

                    namecell.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(51, 51, 51));
                    namecell.Style.Font.Size = 10;

                    namecell.Style.Font.Name = "arial";
                    namecell.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(51, 51, 51));
                    namecell.Style.Font.Size = 10;

                    namecell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    namecell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                }

                var _with1 = ws.Cells[RowNo, 11];
                var namevalues = ((System.Collections.Hashtable)(listContent)[i])["Name"];
                _with1.Value = HttpUtility.HtmlDecode(namevalues.ToString());

                _with1.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                _with1.Style.Border.Left.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                _with1.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                _with1.Style.Border.Right.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                _with1.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                _with1.Style.Border.Top.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                _with1.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                _with1.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));




                if ((int)((System.Collections.Hashtable)(listContent)[i])["Level"] == 0)
                {
                    _with1.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    _with1.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(217, 217, 217));
                }

                _with1.Style.Font.Name = "arial";

                _with1.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(51, 51, 51));
                _with1.Style.Font.Size = 10;

                _with1.Style.Font.Name = "arial";
                _with1.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(51, 51, 51));
                _with1.Style.Font.Size = 10;

                _with1.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                _with1.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                _with1.Style.Indent = ((int)((System.Collections.Hashtable)(listContent)[i])["Level"]);

                GenerateTypeIcon(ws, (string)((System.Collections.Hashtable)(listContent)[i])["ShortDescription"], (string)((System.Collections.Hashtable)(listContent)[i])["ColorCode"], 1, 11, RowNo, (int)((System.Collections.Hashtable)(listContent)[i])["Level"]);

                var _with2 = ws.Cells[RowNo, 12];
                _with2.Value = (int)((System.Collections.Hashtable)(listContent)[i])["Id"];
                _with2.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                _with2.Style.Border.Left.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                _with2.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                _with2.Style.Border.Right.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                _with2.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                _with2.Style.Border.Top.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                _with2.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                _with2.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));
                if ((int)((System.Collections.Hashtable)(listContent)[i])["Level"] == 0)
                {
                    _with2.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    _with2.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(217, 217, 217));
                }

                _with2.Style.Font.Name = "arial";
                _with2.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(51, 51, 51));
                _with2.Style.Font.Size = 10;


                _with2.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                _with2.Style.VerticalAlignment = ExcelVerticalAlignment.Center;



                int StartDynColNo = 13;

                for (int j = 0; j <= columnlen - 1; j++)
                {
                    if (listColumnDefsdata[j].Field != "68" && listColumnDefsdata[j].Type != 10)
                    {
                        var _with3 = ws.Cells[RowNo, StartDynColNo];
                        //.Value = Item.Name
                        _with3.Value = (((System.Collections.Hashtable)(listContent)[i])[listColumnDefsdata[j].Field] != null ? ((System.Collections.Hashtable)(listContent)[i])[listColumnDefsdata[j].Field] : "-");
                        _with3.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        _with3.Style.Border.Left.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                        _with3.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        _with3.Style.Border.Right.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                        _with3.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        _with3.Style.Border.Top.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                        _with3.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        _with3.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                        if ((int)((System.Collections.Hashtable)(listContent)[i])["Level"] == 0)
                        {
                            _with3.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            _with3.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(217, 217, 217));
                        }

                        _with3.Style.Font.Name = "arial";
                        _with3.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(51, 51, 51));
                        _with3.Style.Font.Size = 10;


                        _with3.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        _with3.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                        StartDynColNo = StartDynColNo + 1;



                    }
                }


                if (((int)((System.Collections.Hashtable)(listContent)[i])["Level"] == 0))
                {
                    var _with5 = ws.Cells[RowNo, ColumnNo, RowNo, LastColumnNo];
                    _with5.Merge = true;
                    _with5.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    _with5.Style.Border.Left.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                    _with5.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    _with5.Style.Border.Right.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                    _with5.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    _with5.Style.Border.Top.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));


                    if (((int)((System.Collections.Hashtable)(listContent)[i])["Level"] == 0))
                    {
                        _with5.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        _with5.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(217, 217, 217));
                    }

                    _with5.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    _with5.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(217, 217, 217));


                    int StartColumnNo = ColumnNo;
                    int EndColumnNo = ColumnNo;

                    switch (new System.DateTime(CalenderStartDate.Year, CalenderStartDate.Month, CalenderStartDate.Day).DayOfWeek)
                    {
                        // need to take StartDate
                        case System.DayOfWeek.Monday:
                            EndColumnNo = EndColumnNo + 6;
                            break;
                        case System.DayOfWeek.Tuesday:
                            EndColumnNo = EndColumnNo + 5;
                            break;
                        case System.DayOfWeek.Wednesday:
                            EndColumnNo = EndColumnNo + 4;
                            break;
                        case System.DayOfWeek.Thursday:
                            EndColumnNo = EndColumnNo + 3;
                            break;
                        case System.DayOfWeek.Friday:
                            EndColumnNo = EndColumnNo + 2;
                            break;
                        case System.DayOfWeek.Saturday:
                            EndColumnNo = EndColumnNo + 1;
                            break;
                        case System.DayOfWeek.Sunday:
                            EndColumnNo = EndColumnNo + 0;
                            break;
                    }


                    bool IsOdd = true;

                    while (StartColumnNo <= LastColumnNo)
                    {
                        var _with6 = ws.Cells[RowNo, StartColumnNo, RowNo, EndColumnNo];
                        _with6.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        _with6.Style.Border.Left.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                        _with6.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        _with6.Style.Border.Right.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                        _with6.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        _with6.Style.Border.Top.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                        _with6.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        _with6.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                        if (IsOdd)
                        {
                            _with6.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            _with6.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(242, 242, 242));
                            IsOdd = false;
                        }
                        else
                        {
                            IsOdd = true;
                        }


                        StartColumnNo = EndColumnNo + 1;
                        EndColumnNo = StartColumnNo + 6;

                        if (EndColumnNo > LastColumnNo)
                        {
                            EndColumnNo = LastColumnNo;
                        }
                    }

                    //milestone details will come here
                    string xmlMilestone = (string)((System.Collections.Hashtable)(listContent)[i])["MileStone"];
                    XmlDocument doc = new XmlDocument();

                    if (xmlMilestone != null && xmlMilestone.Length > 0)
                    {
                        doc.LoadXml(xmlMilestone);
                        XmlNodeList milestones = doc.DocumentElement.SelectNodes("//p");
                        ExcelComment Comment = default(ExcelComment);
                        MilestoeList = "";
                        foreach (XmlNode node in milestones)
                        {

                            System.DateTime StartDate = DateTime.Parse(node.Attributes["s"].Value);
                            int MileStartColumn = StartDate.Subtract(CalenderStartDate).Days + ColumnNo;
                            if (MileStartColumn < ColumnNo)
                            {
                                MileStartColumn = ColumnNo;
                            }
                            try
                            {

                                if (node.Attributes["ms"].Value == "0")
                                {
                                    AddImage(ws, MileStartColumn - 1, RowNo - 1, System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "assets\\img\\star.png");
                                    MilestoeList = MilestoeList + "Name: " + HttpUtility.HtmlDecode(node.Attributes["n"].Value) + Environment.NewLine + "Description: " + HttpUtility.HtmlDecode(node.Attributes["d"].Value) + Environment.NewLine + "Due Date: " + node.Attributes["s"].Value + Environment.NewLine + "Status: Not reached" + Environment.NewLine + Environment.NewLine;
                                    Comment = default(ExcelComment);
                                    Comment = ws.Comments.Add(ws.Cells[RowNo, MileStartColumn], MilestoeList + Environment.NewLine, "Marcom Platform");
                                    Comment.Font.FontName = "arial";
                                    Comment.Font.Size = 10;
                                    Comment.From.Column = 1;
                                    Comment.From.Row = 1;
                                    Comment.To.Column = 10;
                                    Comment.To.Row = 15;
                                    Comment.BackgroundColor = System.Drawing.Color.FromArgb(231, 242, 245);
                                }
                                else
                                {
                                    AddImage(ws, MileStartColumn - 1, RowNo - 1, System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "assets\\img\\starGreen.png");
                                    MilestoeList = MilestoeList + "Name: " + HttpUtility.HtmlDecode(node.Attributes["n"].Value) + Environment.NewLine + "Description: " + HttpUtility.HtmlDecode(node.Attributes["d"].Value) + Environment.NewLine + "Due Date: " + node.Attributes["s"].Value + Environment.NewLine + "Status: Reached" + Environment.NewLine + Environment.NewLine;
                                    Comment = default(ExcelComment);
                                    Comment = ws.Comments.Add(ws.Cells[RowNo, MileStartColumn], MilestoeList + Environment.NewLine, "Marcom Platform");
                                    Comment.Font.FontName = "arial";
                                    Comment.Font.Size = 10;
                                    Comment.From.Column = 1;
                                    Comment.From.Row = 1;
                                    Comment.To.Column = 10;
                                    Comment.To.Row = 15;
                                    Comment.BackgroundColor = System.Drawing.Color.FromArgb(231, 242, 245);
                                }

                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }

                    string xmlString = (string)((System.Collections.Hashtable)(listContent)[i])["Period"];
                    //Create an XML Document and load your XML
                    doc = new XmlDocument();
                    if (xmlString != null && xmlString.Length > 0)
                    {
                        doc.LoadXml(xmlString);

                        //Get  nodes
                        XmlNodeList nodes = doc.DocumentElement.SelectNodes("//p");
                        //Iterates through your String appending the available Names
                        foreach (XmlNode node in nodes)
                        {
                            System.DateTime StartDate = DateTime.Parse(node.Attributes["s"].Value);
                            System.DateTime EndDate = DateTime.Parse(node.Attributes["e"].Value);
                            string Startdatecomment = node.Attributes["d"].Value;

                            CultureInfo ciCurr = CultureInfo.CurrentCulture;

                            //Calendar myCalendar = new GregorianCalendar();
                            int EndWeekNumber = ciCurr.Calendar.GetWeekOfYear(EndDate, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);

                            int startweeknum = ciCurr.Calendar.GetWeekOfYear(StartDate, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);
                            int BarStartColumn = StartDate.Subtract(CalenderStartDate).Days + ColumnNo;
                            int BarEndColumn = EndDate.Subtract(StartDate).Days + BarStartColumn;

                            int endcolmn = EndWeekNumber + startweeknum;

                            if (BarStartColumn < ColumnNo)
                            {
                                BarStartColumn = ColumnNo;
                            }

                            if (BarEndColumn > LastColumnNo)
                            {
                                BarEndColumn = LastColumnNo;
                            }

                            if (BarStartColumn < LastColumnNo & BarEndColumn > ColumnNo)
                            {

                                try
                                {

                                    var _with46 = ws.Cells[RowNo, BarStartColumn, RowNo, BarEndColumn];
                                    _with46.Merge = true;
                                    _with46.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    _with46.Style.Border.Left.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                                    _with46.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    _with46.Style.Border.Right.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                                    _with46.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    _with46.Style.Border.Top.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                                    _with46.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    _with46.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                                    _with46.Style.Fill.PatternType = ExcelFillStyle.Solid;

                                    _with46.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(HexStrToBase10Int((string)((System.Collections.Hashtable)(listContent)[i])["ColorCode"])));


                                    XmlNodeList milestones;
                                    //check milestone data count then put
                                    if (xmlMilestone != null)
                                    {
                                        doc.LoadXml(xmlMilestone);
                                        milestones = doc.DocumentElement.SelectNodes("//p");
                                    }
                                    else
                                    {
                                        milestones = null;
                                    }
                                    ExcelComment barComment = default(ExcelComment);
                                    if (milestones != null)
                                    {

                                        if (milestones.Count == 0)
                                        {
                                            MilestoeList = "-";
                                            barComment = ws.Comments.Add(ws.Cells[RowNo, BarStartColumn, RowNo, BarEndColumn], "Name: " + HttpUtility.HtmlDecode(namevalues.ToString()) + Environment.NewLine + "Start Date: " + StartDate.ToString("yyyy-MM-dd") + Environment.NewLine + "End Date: " + EndDate.ToString("yyyy-MM-dd") + Environment.NewLine + "Start/End Comment: " + HttpUtility.HtmlDecode(Startdatecomment.ToString()) + Environment.NewLine, "Marcom Platform");
                                        }
                                        else if (milestones.Count > 0)
                                        {
                                            barComment = ws.Comments.Add(ws.Cells[RowNo, BarStartColumn, RowNo, BarEndColumn], "Name: " + HttpUtility.HtmlDecode(namevalues.ToString()) + Environment.NewLine + "Start Date: " + StartDate.ToString("yyyy-MM-dd") + Environment.NewLine + "End Date: " + EndDate.ToString("yyyy-MM-dd") + Environment.NewLine + "Start/End Comment: " + HttpUtility.HtmlDecode(Startdatecomment.ToString()) + Environment.NewLine + "MileStone Details: " + Environment.NewLine + MilestoeList + Environment.NewLine, "Marcom Platform");
                                        }
                                        else
                                            barComment = ws.Comments.Add(ws.Cells[RowNo, BarStartColumn, RowNo, BarEndColumn], "Name: " + HttpUtility.HtmlDecode(namevalues.ToString()) + Environment.NewLine + "Start Date: " + StartDate.ToString("yyyy-MM-dd") + Environment.NewLine + "End Date: " + EndDate.ToString("yyyy-MM-dd") + Environment.NewLine + "Start/End Comment: " + HttpUtility.HtmlDecode(Startdatecomment.ToString()) + Environment.NewLine, "Marcom Platform");
                                        barComment.Font.FontName = "Calibri";
                                        barComment.Font.Size = 10;
                                        barComment.From.Column = 1;
                                        barComment.From.Row = 1;
                                        barComment.To.Column = 10;
                                        if (milestones.Count == 0)
                                        {
                                            barComment.To.Row = 12;
                                        }
                                        else if (milestones.Count == 1)
                                        {
                                            barComment.To.Row = 14 + (milestones.Count * 4);
                                        }
                                        else
                                        {
                                            barComment.To.Row = 14 + (milestones.Count * 4);
                                        }
                                        barComment.BackgroundColor = System.Drawing.Color.FromArgb(231, 242, 245);
                                    }
                                    else
                                    {
                                        barComment = ws.Comments.Add(ws.Cells[RowNo, BarStartColumn, RowNo, BarEndColumn], "Name: " + HttpUtility.HtmlDecode(namevalues.ToString()) + Environment.NewLine + "Start Date: " + StartDate.ToString("yyyy-MM-dd") + Environment.NewLine + "End Date: " + EndDate.ToString("yyyy-MM-dd") + Environment.NewLine + "Start/End Comment: " + HttpUtility.HtmlDecode(Startdatecomment.ToString()) + Environment.NewLine, "Marcom Platform");
                                        barComment.Font.FontName = "Calibri";
                                        barComment.Font.Size = 10;
                                        barComment.From.Column = 1;
                                        barComment.From.Row = 1;
                                        barComment.To.Column = 10;
                                        barComment.To.Row = 12;
                                        barComment.BackgroundColor = System.Drawing.Color.FromArgb(231, 242, 245);
                                    }

                                }
                                catch (Exception ex)
                                {
                                    //Log("Date overlaping for", Item.Name);

                                }
                            }

                        }
                    }

                }
                else
                {

                    int StartColumnNo = ColumnNo;
                    int EndColumnNo = ColumnNo;

                    switch (new System.DateTime(CalenderStartDate.Year, CalenderStartDate.Month, CalenderStartDate.Day).DayOfWeek)
                    {
                        // need to take StartDate
                        case System.DayOfWeek.Monday:
                            EndColumnNo = EndColumnNo + 6;
                            break;
                        case System.DayOfWeek.Tuesday:
                            EndColumnNo = EndColumnNo + 5;
                            break;
                        case System.DayOfWeek.Wednesday:
                            EndColumnNo = EndColumnNo + 4;
                            break;
                        case System.DayOfWeek.Thursday:
                            EndColumnNo = EndColumnNo + 3;
                            break;
                        case System.DayOfWeek.Friday:
                            EndColumnNo = EndColumnNo + 2;
                            break;
                        case System.DayOfWeek.Saturday:
                            EndColumnNo = EndColumnNo + 1;
                            break;
                        case System.DayOfWeek.Sunday:
                            EndColumnNo = EndColumnNo + 0;
                            break;
                    }


                    bool IsOdd = true;

                    while (StartColumnNo <= LastColumnNo)
                    {
                        var _with6 = ws.Cells[RowNo, StartColumnNo, RowNo, EndColumnNo];
                        _with6.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        _with6.Style.Border.Left.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                        _with6.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        _with6.Style.Border.Right.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                        _with6.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        _with6.Style.Border.Top.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                        _with6.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        _with6.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                        if (IsOdd)
                        {
                            _with6.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            _with6.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(242, 242, 242));
                            IsOdd = false;
                        }
                        else
                        {
                            IsOdd = true;
                        }


                        StartColumnNo = EndColumnNo + 1;
                        EndColumnNo = StartColumnNo + 6;

                        if (EndColumnNo > LastColumnNo)
                        {
                            EndColumnNo = LastColumnNo;
                        }
                    }

                    //milestone details will come here
                    string xmlMilestone = (string)((System.Collections.Hashtable)(listContent)[i])["MileStone"];
                    XmlDocument doc = new XmlDocument();

                    if (xmlMilestone != null && xmlMilestone.Length > 0)
                    {
                        doc.LoadXml(xmlMilestone);
                        XmlNodeList milestones = doc.DocumentElement.SelectNodes("//p");
                        ExcelComment Comment = default(ExcelComment);
                        MilestoeList = "";
                        foreach (XmlNode node in milestones)
                        {

                            System.DateTime StartDate = DateTime.Parse(node.Attributes["s"].Value);
                            int MileStartColumn = StartDate.Subtract(CalenderStartDate).Days + ColumnNo;
                            if (MileStartColumn < ColumnNo)
                            {
                                MileStartColumn = ColumnNo;
                            }
                            try
                            {

                                if (node.Attributes["ms"].Value == "0")
                                {
                                    AddImage(ws, MileStartColumn - 1, RowNo - 1, System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "assets\\img\\star.png");
                                    MilestoeList = MilestoeList + "Name: " + HttpUtility.HtmlDecode(node.Attributes["n"].Value) + Environment.NewLine + "Description: " + HttpUtility.HtmlDecode(node.Attributes["d"].Value) + Environment.NewLine + "Due Date: " + node.Attributes["s"].Value + Environment.NewLine + "Status: Not reached" + Environment.NewLine + Environment.NewLine;
                                    Comment = default(ExcelComment);
                                    Comment = ws.Comments.Add(ws.Cells[RowNo, MileStartColumn], MilestoeList + Environment.NewLine, "Marcom Platform");
                                    Comment.Font.FontName = "arial";
                                    Comment.Font.Size = 10;
                                    Comment.From.Column = 1;
                                    Comment.From.Row = 1;
                                    Comment.To.Column = 10;
                                    Comment.To.Row = 15;
                                    Comment.BackgroundColor = System.Drawing.Color.FromArgb(231, 242, 245);
                                }
                                else
                                {
                                    AddImage(ws, MileStartColumn - 1, RowNo - 1, System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "assets\\img\\starGreen.png");
                                    MilestoeList = MilestoeList + "Name: " + HttpUtility.HtmlDecode(node.Attributes["n"].Value) + Environment.NewLine + "Description: " + HttpUtility.HtmlDecode(node.Attributes["d"].Value) + Environment.NewLine + "Due Date: " + node.Attributes["s"].Value + Environment.NewLine + "Status: Reached" + Environment.NewLine + Environment.NewLine;
                                    Comment = default(ExcelComment);
                                    Comment = ws.Comments.Add(ws.Cells[RowNo, MileStartColumn], MilestoeList + Environment.NewLine, "Marcom Platform");
                                    Comment.Font.FontName = "arial";
                                    Comment.Font.Size = 10;
                                    Comment.From.Column = 1;
                                    Comment.From.Row = 1;
                                    Comment.To.Column = 10;
                                    Comment.To.Row = 15;
                                    Comment.BackgroundColor = System.Drawing.Color.FromArgb(231, 242, 245);
                                }

                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }

                    string xmlString = (string)((System.Collections.Hashtable)(listContent)[i])["Period"];
                    //Create an XML Document and load your XML
                    doc = new XmlDocument();
                    if (xmlString != null && xmlString.Length > 0)
                    {
                        doc.LoadXml(xmlString);

                        //Get  nodes
                        XmlNodeList nodes = doc.DocumentElement.SelectNodes("//p");
                        //Iterates through your String appending the available Names
                        foreach (XmlNode node in nodes)
                        {
                            System.DateTime StartDate = DateTime.Parse(node.Attributes["s"].Value);
                            System.DateTime EndDate = DateTime.Parse(node.Attributes["e"].Value);
                            string Startdatecomment = node.Attributes["d"].Value;
                            CultureInfo ciCurr = CultureInfo.CurrentCulture;
                            int EndWeekNumber = ciCurr.Calendar.GetWeekOfYear(EndDate, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);
                            int startweeknum = ciCurr.Calendar.GetWeekOfYear(StartDate, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);
                            int BarStartColumn = StartDate.Subtract(CalenderStartDate).Days + ColumnNo;
                            int BarEndColumn = EndDate.Subtract(StartDate).Days + BarStartColumn;

                            int endcolmn = EndWeekNumber + startweeknum;

                            if (BarStartColumn < ColumnNo)
                            {
                                BarStartColumn = ColumnNo;
                            }

                            if (BarEndColumn > LastColumnNo)
                            {
                                BarEndColumn = LastColumnNo;
                            }

                            if (BarStartColumn < LastColumnNo & BarEndColumn > ColumnNo)
                            {

                                try
                                {

                                    var _with46 = ws.Cells[RowNo, BarStartColumn, RowNo, BarEndColumn];
                                    _with46.Merge = true;
                                    _with46.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    _with46.Style.Border.Left.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                                    _with46.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    _with46.Style.Border.Right.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                                    _with46.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    _with46.Style.Border.Top.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                                    _with46.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    _with46.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

                                    _with46.Style.Fill.PatternType = ExcelFillStyle.Solid;

                                    _with46.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(HexStrToBase10Int((string)((System.Collections.Hashtable)(listContent)[i])["ColorCode"])));


                                    XmlNodeList milestones;
                                    //check milestone data count then put
                                    if (xmlMilestone != null)
                                    {
                                        doc.LoadXml(xmlMilestone);
                                        milestones = doc.DocumentElement.SelectNodes("//p");
                                    }
                                    else
                                    {
                                        milestones = null;
                                    }
                                    ExcelComment barComment = default(ExcelComment);
                                    if (milestones != null)
                                    {

                                        if (milestones.Count == 0)
                                        {
                                            MilestoeList = "-";
                                            barComment = ws.Comments.Add(ws.Cells[RowNo, BarStartColumn, RowNo, BarEndColumn], "Name: " + HttpUtility.HtmlDecode(namevalues.ToString()) + Environment.NewLine + "Start Date: " + StartDate.ToString("yyyy-MM-dd") + Environment.NewLine + "End Date: " + EndDate.ToString("yyyy-MM-dd") + Environment.NewLine + "Start/End Comment: " + HttpUtility.HtmlDecode(Startdatecomment.ToString()) + Environment.NewLine, "Marcom Platform");
                                        }
                                        else if (milestones.Count > 0)
                                        {
                                            barComment = ws.Comments.Add(ws.Cells[RowNo, BarStartColumn, RowNo, BarEndColumn], "Name: " + HttpUtility.HtmlDecode(namevalues.ToString()) + Environment.NewLine + "Start Date: " + StartDate.ToString("yyyy-MM-dd") + Environment.NewLine + "End Date: " + EndDate.ToString("yyyy-MM-dd") + Environment.NewLine + "Start/End Comment: " + HttpUtility.HtmlDecode(Startdatecomment.ToString()) + Environment.NewLine + "MileStone Details: " + Environment.NewLine + MilestoeList + Environment.NewLine, "Marcom Platform");
                                        }
                                        else
                                            barComment = ws.Comments.Add(ws.Cells[RowNo, BarStartColumn, RowNo, BarEndColumn], "Name: " + HttpUtility.HtmlDecode(namevalues.ToString()) + Environment.NewLine + "Start Date: " + StartDate.ToString("yyyy-MM-dd") + Environment.NewLine + "End Date: " + EndDate.ToString("yyyy-MM-dd") + Environment.NewLine + "Start/End Comment: " + HttpUtility.HtmlDecode(Startdatecomment.ToString()) + Environment.NewLine, "Marcom Platform");
                                        barComment.Font.FontName = "Calibri";
                                        barComment.Font.Size = 10;
                                        barComment.From.Column = 1;
                                        barComment.From.Row = 1;
                                        barComment.To.Column = 10;
                                        if (milestones.Count == 0)
                                        {
                                            barComment.To.Row = 12;
                                        }
                                        else if (milestones.Count == 1)
                                        {
                                            barComment.To.Row = 14 + (milestones.Count * 4);
                                        }
                                        else
                                        {
                                            barComment.To.Row = 14 + (milestones.Count * 4);
                                        }
                                        barComment.BackgroundColor = System.Drawing.Color.FromArgb(231, 242, 245);
                                    }
                                    else
                                    {
                                        barComment = ws.Comments.Add(ws.Cells[RowNo, BarStartColumn, RowNo, BarEndColumn], "Name: " + HttpUtility.HtmlDecode(namevalues.ToString()) + Environment.NewLine + "Start Date: " + StartDate.ToString("yyyy-MM-dd") + Environment.NewLine + "End Date: " + EndDate.ToString("yyyy-MM-dd") + Environment.NewLine + "Start/End Comment: " + HttpUtility.HtmlDecode(Startdatecomment.ToString()) + Environment.NewLine, "Marcom Platform");
                                        barComment.Font.FontName = "Calibri";
                                        barComment.Font.Size = 10;
                                        barComment.From.Column = 1;
                                        barComment.From.Row = 1;
                                        barComment.To.Column = 10;
                                        barComment.To.Row = 12;
                                        barComment.BackgroundColor = System.Drawing.Color.FromArgb(231, 242, 245);
                                    }

                                }
                                catch (Exception ex)
                                {
                                    //Log("Date overlaping for", Item.Name);

                                }
                            }

                        }
                    }
                }

                RowNo++;
            }


            return 0;
        }

        private void AddIconEntitytype(ExcelWorksheet ws, int columnIndex, int rowIndex, int level, string colorcode, string shortdesc)
        {

            //How to Add a Image using EP Plus
            Bitmap image = CreateBitmapImage(" " + shortdesc + " ", colorcode);
            ExcelPicture picture = null;
            if ((image != null))
            {
                picture = ws.Drawings.AddPicture("pic" + rowIndex.ToString() + columnIndex.ToString(), image);
                picture.From.Column = columnIndex;
                picture.From.Row = rowIndex;
                picture.From.ColumnOff = 20;
                picture.From.RowOff = 20;
                picture.SetPosition(rowIndex, 3, columnIndex, 3);
                //picture.From.ColumnOff = Pixel2MTU(2) 'Two pixel space for better alignment
                //picture.From.RowOff = Pixel2MTU(2) 'Two pixel space for better alignment
                picture.SetSize(image.Width, image.Height);
            }

        }




        private Bitmap CreateBitmapImage(string sImageText, string cccode)
        {
            Bitmap objBmpImage = new Bitmap(27, 12);

            int intWidth = 0;
            int intHeight = 0;

            // Create the Font object for the image text drawing.
            System.Drawing.Font objFont = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);

            // Create a graphics object to measure the text's width and height.
            Graphics objGraphics = Graphics.FromImage(objBmpImage);

            //// This is where the bitmap size is determined.
            intWidth = (int)objGraphics.MeasureString(sImageText, objFont).Width;
            intHeight = (int)objGraphics.MeasureString(sImageText, objFont).Height;

            // Create the bmpImage again with the correct size for the text and font.
            objBmpImage = new Bitmap(objBmpImage, new Size(intWidth, intHeight));


            // Add the colors to the new bitmap.
            objGraphics = Graphics.FromImage(objBmpImage);

            // Set Background color

            objGraphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

            objGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            objGraphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;



            objGraphics.Clear(System.Drawing.ColorTranslator.FromHtml("#" + cccode));
            objGraphics.SmoothingMode = SmoothingMode.HighQuality;

            objGraphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias; //  <-- This is the correct value to use. ClearTypeGridFit is better yet!
            objGraphics.DrawString(sImageText, objFont, new SolidBrush(System.Drawing.Color.White), 0, 0, StringFormat.GenericDefault);

            objGraphics.Flush();

            return (objBmpImage);
        }


        private void AddImage(ExcelWorksheet ws, int columnIndex, int rowIndex, string filePath)
        {
            //How to Add a Image using EP Plus
            Bitmap image = new Bitmap(filePath);
            //Bitmap image = new Bitmap(filePath);
            //ExcelPicture picture = null;
            ExcelPicture picture = null;
            if ((image != null))
            {
                picture = ws.Drawings.AddPicture("pic" + rowIndex.ToString() + columnIndex.ToString(), image);
                picture.From.Column = columnIndex;
                picture.From.Row = rowIndex;
                picture.From.ColumnOff = 20;
                picture.From.RowOff = 20;
                picture.SetPosition(rowIndex, 5, columnIndex, 5);
                //picture.From.ColumnOff = Pixel2MTU(2) 'Two pixel space for better alignment
                //picture.From.RowOff = Pixel2MTU(2) 'Two pixel space for better alignment
                picture.SetSize(13, 13);
            }
        }


        public string GetDefaultCurrencyinReports(ReportManagerProxy proxy)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    string defaultCurrency = "";

                    string currencyQry = "SELECT pct.ShortName AS 'CurrencyName' FROM PM_CurrencyType pct WHERE id = (SELECT TOP 1 cas.SettingValue FROM CM_AdditionalSettings cas WHERE id=2)"; //default currency name
                    IList currencyResult = tx.PersistenceManager.PlanningRepository.ExecuteQuery(currencyQry);
                    defaultCurrency = (string)((System.Collections.Hashtable)(currencyResult)[0])["CurrencyName"];
                    tx.Commit();
                    return defaultCurrency;
                }

            }
            catch (Exception)
            {
                return "";

            }

        }


        public IList GetFinancialSummaryDetlRpt(ReportManagerProxy proxy, string SelectedEntityTypeIDs)
        {
            try
            {
                StringBuilder FinSummaryQry = new StringBuilder();
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {



                    string SelectedIds = String.Join(",", proxy.MarcomManager.EntitySortorderIdColle.Select(a => a.EntityIds.ToString()).ToArray());
                    FinSummaryQry.Append(" SELECT COUNT(finTotal.EntityID)  AS col1, ");
                    FinSummaryQry.Append("        SUM(finTotal.col2)        AS col2, ");
                    FinSummaryQry.Append("        SUM(finTotal.col3)        AS col3, ");
                    FinSummaryQry.Append("        SUM(finTotal.col4)        AS col4, ");
                    FinSummaryQry.Append("        SUM(finTotal.col5)        AS col5, ");
                    FinSummaryQry.Append("        SUM(finTotal.col6)        AS col6, ");
                    FinSummaryQry.Append("        SUM(finTotal.col7)        AS col7, ");
                    FinSummaryQry.Append("        SUM(finTotal.col8)        AS col8, ");
                    FinSummaryQry.Append("        SUM(finTotal.col9)        AS col9 ");
                    FinSummaryQry.Append(" FROM   ( ");
                    FinSummaryQry.Append("            SELECT fin.EntityID, ");
                    FinSummaryQry.Append("                   ISNULL(SUM(fin.TotalPlannedAmount), 0) AS col2, ");
                    FinSummaryQry.Append("                   ISNULL(SUM(fin.TotalRequested), 0) AS col3, ");
                    FinSummaryQry.Append("                   ISNULL(SUM(fin.TotalApprovedAmount), 0) AS col4, ");
                    FinSummaryQry.Append("                   ISNULL(SUM(fin.ApprovedBudget), 0) col5, ");
                    FinSummaryQry.Append("                   ISNULL(SUM(fin.BudgetDeviation), 0) col6, ");
                    FinSummaryQry.Append("                   ISNULL(SUM(fin.TotalCommitedAmount), 0) AS col7, ");
                    FinSummaryQry.Append("                   ISNULL(SUM(fin.TotalSpentAmount), 0) AS col8, ");
                    FinSummaryQry.Append("                   ISNULL( ");
                    FinSummaryQry.Append("                       SUM(fin.TotalApprovedAmount) - SUM(fin.TotalSpentAmount), ");
                    FinSummaryQry.Append("                       0 ");
                    FinSummaryQry.Append("                   )  AS col9 ");
                    FinSummaryQry.Append("            FROM   ( ");
                    FinSummaryQry.Append("                       SELECT pf.EntityID, ");
                    FinSummaryQry.Append("                              pe.UniqueKey, ");
                    FinSummaryQry.Append("                              pe.TypeId, ");
                    FinSummaryQry.Append("                              pf.PlannedAmount AS TotalPlannedAmount, ");
                    FinSummaryQry.Append("                              pf.RequestedAmount AS TotalRequested, ");
                    FinSummaryQry.Append("                              pf.ApprovedAllocatedAmount AS TotalApprovedAmount, ");
                    FinSummaryQry.Append("                              ISNULL( ");
                    FinSummaryQry.Append("                                  ( ");
                    FinSummaryQry.Append("                                      SELECT SUM(pf2.Spent) AS Spent ");
                    FinSummaryQry.Append("                                      FROM   PM_Financial pf2 ");
                    FinSummaryQry.Append("                                             INNER JOIN PM_Entity pe2 ");
                    FinSummaryQry.Append("                                                  ON  pe2.ID = pf2.EntityID ");
                    FinSummaryQry.Append("                                                  AND pe2.[Active] = 1 ");
                    FinSummaryQry.Append("                                                  AND pe2.TypeId IN (SELECT met.ID ");
                    FinSummaryQry.Append("                                                                     FROM    ");
                    FinSummaryQry.Append("                                                                            MM_EntityType  ");
                    FinSummaryQry.Append("                                                                            met ");
                    FinSummaryQry.Append("                                                                     WHERE  met.IsAssociate =  ");
                    FinSummaryQry.Append("                                                                            0 ");
                    FinSummaryQry.Append("                                                                            AND  ");
                    FinSummaryQry.Append("                                                                                met.ID  ");
                    FinSummaryQry.Append("                                                                                NOT IN (5, 10)) ");
                    FinSummaryQry.Append("                                                  AND pf2.CostCenterID = pf.CostCenterID ");
                    FinSummaryQry.Append("                                      WHERE  pe2.UniqueKey LIKE pe.UniqueKey + ");
                    FinSummaryQry.Append("                                             '%' ");
                    FinSummaryQry.Append("                                  ), ");
                    FinSummaryQry.Append("                                  0 ");
                    FinSummaryQry.Append("                              )    AS TotalSpentAmount, ");
                    FinSummaryQry.Append("                              ISNULL( ");
                    FinSummaryQry.Append("                                  ( ");
                    FinSummaryQry.Append("                                      SELECT SUM(pf2.Commited) AS Commited ");
                    FinSummaryQry.Append("                                      FROM   PM_Financial pf2 ");
                    FinSummaryQry.Append("                                             INNER JOIN PM_Entity pe2 ");
                    FinSummaryQry.Append("                                                  ON  pe2.ID = pf2.EntityID ");
                    FinSummaryQry.Append("                                                  AND pe2.[Active] = 1 ");
                    FinSummaryQry.Append("                                                  AND pe2.TypeId IN (SELECT met.ID ");
                    FinSummaryQry.Append("                                                                     FROM    ");
                    FinSummaryQry.Append("                                                                            MM_EntityType  ");
                    FinSummaryQry.Append("                                                                            met ");
                    FinSummaryQry.Append("                                                                     WHERE  met.IsAssociate =  ");
                    FinSummaryQry.Append("                                                                            0 ");
                    FinSummaryQry.Append("                                                                            AND  ");
                    FinSummaryQry.Append("                                                                                met.ID  ");
                    FinSummaryQry.Append("                                                                                NOT IN (5, 10)) ");
                    FinSummaryQry.Append("                                                  AND pf2.CostCenterID = pf.CostCenterID ");
                    FinSummaryQry.Append("                                      WHERE  pe2.UniqueKey LIKE pe.UniqueKey + ");
                    FinSummaryQry.Append("                                             '%' ");
                    FinSummaryQry.Append("                                  ), ");
                    FinSummaryQry.Append("                                  0 ");
                    FinSummaryQry.Append("                              )    AS TotalCommitedAmount, ");
                    FinSummaryQry.Append("                              pf.ApprovedBudget AS ApprovedBudget, ");
                    FinSummaryQry.Append("                              CASE  ");
                    FinSummaryQry.Append("                                   WHEN pf.ApprovedBudgetDate IS NULL THEN 0 ");
                    FinSummaryQry.Append("                                   WHEN (pf.ApprovedBudget - pf.ApprovedAllocatedAmount)  ");
                    FinSummaryQry.Append("                                        < 0 THEN 0 ");
                    FinSummaryQry.Append("                                   ELSE pf.ApprovedBudget - pf.ApprovedAllocatedAmount ");
                    FinSummaryQry.Append("                              END  AS BudgetDeviation ");
                    FinSummaryQry.Append("                       FROM   PM_Financial pf ");
                    FinSummaryQry.Append("                              INNER JOIN PM_Entity pe ");
                    FinSummaryQry.Append("                                   ON  pe.ID = pf.EntityID ");
                    FinSummaryQry.Append("                                   AND pe.[Active] = 1 ");
                    FinSummaryQry.Append("                                   AND pe.TypeId IN (" + SelectedEntityTypeIDs + ") ");
                    FinSummaryQry.Append("                                   AND pe.ID IN (" + SelectedIds + ") ");
                    FinSummaryQry.Append("                   )  AS fin ");
                    FinSummaryQry.Append("            GROUP BY ");
                    FinSummaryQry.Append("                   fin.EntityID ");
                    FinSummaryQry.Append("        )                         AS finTotal");

                    return tx.PersistenceManager.ReportRepository.ExecuteQuery(FinSummaryQry.ToString());

                }

            }
            catch
            {

            }
            return null;

        }


        public IList GetFinancialSummaryDetlRptByAttribute(ReportManagerProxy proxy, string EntityTypeIds, int attributeID)
        {
            try
            {
                StringBuilder FinAttributeSummaryQry = new StringBuilder();
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    //int[] strEntityTypeIds = EntityTypeIds.Split(',').Select(a=>(int)a).ToArray();

                    int[] strEntityTypeIds = EntityTypeIds.Split(',').Select(int.Parse).ToArray();
                    var AttributeDetail = tx.PersistenceManager.ReportRepository.Query<AttributeDao>().Where(a => a.Id == attributeID).Select(a => a).SingleOrDefault();
                    int[] selectedEntitypes = tx.PersistenceManager.ReportRepository.Query<EntityTypeAttributeRelationDao>().Where(a => strEntityTypeIds.Contains(a.EntityTypeID) &&
                        a.AttributeID == attributeID).Select(a => a.EntityTypeID).Distinct().ToArray();
                    if (selectedEntitypes.Length > 0)
                    {
                        string inClause = "("
                                               + String.Join(",", selectedEntitypes.Select(x => x.ToString()).ToArray())
                                             + ")";

                        StringBuilder innerqry = new StringBuilder();
                        innerqry.Append("(");
                        for (var i = 0; i < selectedEntitypes.Length; i++)
                        {
                            switch ((AttributesList)AttributeDetail.AttributeTypeID)
                            {
                                case AttributesList.ListMultiSelection:
                                    //FinAttributeSummaryQry.Append(" SELECT ', ' +  mo.Caption ");
                                    innerqry.Append("SELECT id,'' as Attr_" + attributeID + " FROM MM_AttributeRecord_" + selectedEntitypes[i]);
                                    break;
                                case AttributesList.ListSingleSelection:
                                    innerqry.Append("SELECT id,Attr_" + attributeID + " FROM MM_AttributeRecord_" + selectedEntitypes[i]);
                                    break;
                            }

                            if (i < (selectedEntitypes.Length - 1))
                            {
                                innerqry.Append(" union all ");
                            }
                        }
                        innerqry.Append(")");
                        string SelectedIds = String.Join(",", proxy.MarcomManager.EntitySortorderIdColle.Select(a => a.EntityIds.ToString()).ToArray());



                        FinAttributeSummaryQry.Append(" SELECT AttrWise.col1, ");
                        FinAttributeSummaryQry.Append("        isnull(SUM(AttrWise.col2),0)  AS col2, ");
                        FinAttributeSummaryQry.Append("        isnull(SUM(AttrWise.col3),0)  AS col3, ");
                        FinAttributeSummaryQry.Append("        isnull(SUM(AttrWise.col4),0)  AS col4, ");
                        FinAttributeSummaryQry.Append("        isnull(SUM(AttrWise.col5),0)  AS col5, ");
                        FinAttributeSummaryQry.Append("        isnull(SUM(AttrWise.col6),0)  AS col6, ");
                        FinAttributeSummaryQry.Append("        isnull(SUM(AttrWise.col7),0)  AS col7, ");
                        FinAttributeSummaryQry.Append("        isnull(SUM(AttrWise.col8),0)  AS col8, ");
                        FinAttributeSummaryQry.Append("        isnull(SUM(AttrWise.col9),0)  AS col9 ");
                        FinAttributeSummaryQry.Append(" FROM   ( ");
                        FinAttributeSummaryQry.Append("            SELECT mo.Caption  AS col1, ");
                        FinAttributeSummaryQry.Append("                   0           AS col2, ");
                        FinAttributeSummaryQry.Append("                   0           AS col3, ");
                        FinAttributeSummaryQry.Append("                   0           AS col4, ");
                        FinAttributeSummaryQry.Append("                   0           AS col5, ");
                        FinAttributeSummaryQry.Append("                   0           AS col6, ");
                        FinAttributeSummaryQry.Append("                   0           AS col7, ");
                        FinAttributeSummaryQry.Append("                   0           AS col8, ");
                        FinAttributeSummaryQry.Append("                   0           AS col9 ");
                        FinAttributeSummaryQry.Append("            FROM   MM_Option      mo ");
                        FinAttributeSummaryQry.Append("            WHERE  mo.AttributeID = " + attributeID + "  ");
                        FinAttributeSummaryQry.Append("             ");
                        FinAttributeSummaryQry.Append("            UNION ALL ");
                        FinAttributeSummaryQry.Append("             ");
                        FinAttributeSummaryQry.Append("            SELECT mo.Caption AS col1, ");
                        FinAttributeSummaryQry.Append("                   finsummary.col2, ");
                        FinAttributeSummaryQry.Append("                   finsummary.col3, ");
                        FinAttributeSummaryQry.Append("                   finsummary.col4, ");
                        FinAttributeSummaryQry.Append("                   finsummary.col5, ");
                        FinAttributeSummaryQry.Append("                   finsummary.col6, ");
                        FinAttributeSummaryQry.Append("                   finsummary.col7, ");
                        FinAttributeSummaryQry.Append("                   finsummary.col8, ");
                        FinAttributeSummaryQry.Append("                   finsummary.col9 ");
                        FinAttributeSummaryQry.Append("            FROM   ( ");
                        FinAttributeSummaryQry.Append("                       SELECT fin.EntityID AS col1, ");
                        FinAttributeSummaryQry.Append("                              SUM(fin.TotalPlannedAmount) AS col2, ");
                        FinAttributeSummaryQry.Append("                              SUM(fin.TotalRequested) AS col3, ");
                        FinAttributeSummaryQry.Append("                              SUM(fin.TotalApprovedAmount) AS col4, ");
                        FinAttributeSummaryQry.Append("                              SUM(fin.ApprovedBudget) col5, ");
                        FinAttributeSummaryQry.Append("                              SUM(fin.BudgetDeviation) col6, ");
                        FinAttributeSummaryQry.Append("                              SUM(fin.TotalCommitedAmount) AS col7, ");
                        FinAttributeSummaryQry.Append("                              SUM(fin.TotalSpentAmount) AS col8, ");
                        FinAttributeSummaryQry.Append("                              SUM(fin.TotalApprovedAmount) - SUM(fin.TotalSpentAmount) AS  ");
                        FinAttributeSummaryQry.Append("                              col9 ");
                        FinAttributeSummaryQry.Append("                       FROM   ( ");
                        FinAttributeSummaryQry.Append("                                  SELECT pf.EntityID, ");
                        FinAttributeSummaryQry.Append("                                         pe.UniqueKey, ");
                        FinAttributeSummaryQry.Append("                                         pe.TypeId, ");
                        FinAttributeSummaryQry.Append("                                         pf.PlannedAmount AS TotalPlannedAmount, ");
                        FinAttributeSummaryQry.Append("                                         pf.RequestedAmount AS TotalRequested, ");
                        FinAttributeSummaryQry.Append("                                         pf.ApprovedAllocatedAmount AS  ");
                        FinAttributeSummaryQry.Append("                                         TotalApprovedAmount, ");
                        FinAttributeSummaryQry.Append("                                         ISNULL( ");
                        FinAttributeSummaryQry.Append("                                             ( ");
                        FinAttributeSummaryQry.Append("                                                 SELECT SUM(pf2.Spent) AS Spent ");
                        FinAttributeSummaryQry.Append("                                                 FROM   PM_Financial pf2 ");
                        FinAttributeSummaryQry.Append("                                                        INNER JOIN PM_Entity pe2 ");
                        FinAttributeSummaryQry.Append("                                                             ON  pe2.ID = pf2.EntityID ");
                        FinAttributeSummaryQry.Append("                                                             AND pe2.[Active] = 1 ");
                        FinAttributeSummaryQry.Append("                                                             AND pe2.TypeId IN (SELECT  ");
                        FinAttributeSummaryQry.Append("                                                                                       met.ID ");
                        FinAttributeSummaryQry.Append("                                                                                FROM    ");
                        FinAttributeSummaryQry.Append("                                                                                       MM_EntityType  ");
                        FinAttributeSummaryQry.Append("                                                                                       met ");
                        FinAttributeSummaryQry.Append("                                                                                WHERE   ");
                        FinAttributeSummaryQry.Append("                                                                                       met.IsAssociate =  ");
                        FinAttributeSummaryQry.Append("                                                                                       0 ");
                        FinAttributeSummaryQry.Append("                                                                                       AND  ");
                        FinAttributeSummaryQry.Append("                                                                                           met.ID  ");
                        FinAttributeSummaryQry.Append("                                                                                           NOT IN (5, 10)) ");
                        FinAttributeSummaryQry.Append("                                                             AND pf2.CostCenterID =  ");
                        FinAttributeSummaryQry.Append("                                                                 pf.CostCenterID ");
                        FinAttributeSummaryQry.Append("                                                 WHERE  pe2.UniqueKey LIKE pe.UniqueKey  ");
                        FinAttributeSummaryQry.Append("                                                        + ");
                        FinAttributeSummaryQry.Append("                                                        '%' ");
                        FinAttributeSummaryQry.Append("                                             ), ");
                        FinAttributeSummaryQry.Append("                                             0 ");
                        FinAttributeSummaryQry.Append("                                         ) AS TotalSpentAmount, ");
                        FinAttributeSummaryQry.Append("                                         ISNULL( ");
                        FinAttributeSummaryQry.Append("                                             ( ");
                        FinAttributeSummaryQry.Append("                                                 SELECT SUM(pf2.Commited) AS  ");
                        FinAttributeSummaryQry.Append("                                                        Commited ");
                        FinAttributeSummaryQry.Append("                                                 FROM   PM_Financial pf2 ");
                        FinAttributeSummaryQry.Append("                                                        INNER JOIN PM_Entity pe2 ");
                        FinAttributeSummaryQry.Append("                                                             ON  pe2.ID = pf2.EntityID ");
                        FinAttributeSummaryQry.Append("                                                             AND pe2.[Active] = 1 ");
                        FinAttributeSummaryQry.Append("                                                             AND pe2.TypeId IN (SELECT  ");
                        FinAttributeSummaryQry.Append("                                                                                       met.ID ");
                        FinAttributeSummaryQry.Append("                                                                                FROM    ");
                        FinAttributeSummaryQry.Append("                                                                                       MM_EntityType  ");
                        FinAttributeSummaryQry.Append("                                                                                       met ");
                        FinAttributeSummaryQry.Append("                                                                                WHERE   ");
                        FinAttributeSummaryQry.Append("                                                                                       met.IsAssociate =  ");
                        FinAttributeSummaryQry.Append("                                                                                       0 ");
                        FinAttributeSummaryQry.Append("                                                                                       AND  ");
                        FinAttributeSummaryQry.Append("                                                                                           met.ID  ");
                        FinAttributeSummaryQry.Append("                                                                                           NOT IN (5, 10)) ");
                        FinAttributeSummaryQry.Append("                                                             AND pf2.CostCenterID =  ");
                        FinAttributeSummaryQry.Append("                                                                 pf.CostCenterID ");
                        FinAttributeSummaryQry.Append("                                                 WHERE  pe2.UniqueKey LIKE pe.UniqueKey  ");
                        FinAttributeSummaryQry.Append("                                                        + ");
                        FinAttributeSummaryQry.Append("                                                        '%' ");
                        FinAttributeSummaryQry.Append("                                             ), ");
                        FinAttributeSummaryQry.Append("                                             0 ");
                        FinAttributeSummaryQry.Append("                                         ) AS TotalCommitedAmount, ");
                        FinAttributeSummaryQry.Append("                                         pf.ApprovedBudget AS ApprovedBudget, ");
                        FinAttributeSummaryQry.Append("                                         CASE  ");
                        FinAttributeSummaryQry.Append("                                              WHEN pf.ApprovedBudgetDate IS NULL THEN  ");
                        FinAttributeSummaryQry.Append("                                                   0 ");
                        FinAttributeSummaryQry.Append("                                              WHEN (pf.ApprovedBudget - pf.ApprovedAllocatedAmount)  ");
                        FinAttributeSummaryQry.Append("                                                   < 0 THEN 0 ");
                        FinAttributeSummaryQry.Append("                                              ELSE pf.ApprovedBudget - pf.ApprovedAllocatedAmount ");
                        FinAttributeSummaryQry.Append("                                         END AS BudgetDeviation ");
                        FinAttributeSummaryQry.Append("                                  FROM   PM_Financial pf ");
                        FinAttributeSummaryQry.Append("                                         INNER JOIN PM_Entity pe ");
                        FinAttributeSummaryQry.Append("                                              ON  pe.ID = pf.EntityID ");
                        FinAttributeSummaryQry.Append("                                              AND pe.[Active] = 1 ");
                        FinAttributeSummaryQry.Append("                                              AND pe.TypeId IN " + inClause + " ");
                        FinAttributeSummaryQry.Append("                                              AND pe.ID IN (" + SelectedIds + ") ");
                        FinAttributeSummaryQry.Append("                              ) AS fin ");
                        FinAttributeSummaryQry.Append("                       GROUP BY ");
                        FinAttributeSummaryQry.Append("                              fin.EntityID ");
                        FinAttributeSummaryQry.Append("                   ) finsummary ");
                        //FinAttributeSummaryQry.Append("                   INNER JOIN MM_AttributeRecord_" + EntityTypeId + " ma  ");
                        FinAttributeSummaryQry.Append("             Inner Join " + innerqry.ToString() + " ma ");
                        FinAttributeSummaryQry.Append("                        ON  finsummary.col1 = ma.ID ");
                        switch ((AttributesList)AttributeDetail.AttributeTypeID)
                        {
                            case AttributesList.ListMultiSelection:
                                //FinAttributeSummaryQry.Append(" SELECT ', ' +  mo.Caption ");
                                FinAttributeSummaryQry.Append(" inner join    MM_MultiSelect mms2  ON  mms2.EntityID=finsummary.col1 ");
                                FinAttributeSummaryQry.Append(" INNER JOIN MM_Option mo ");
                                FinAttributeSummaryQry.Append(" ON  mms2.OptionID = mo.ID and  mms2.AttributeID=" + attributeID);
                                FinAttributeSummaryQry.Append("  and  mms2.EntityID = finsummary.col1 ");
                                break;
                            case AttributesList.ListSingleSelection:
                                FinAttributeSummaryQry.Append("                   INNER JOIN MM_Option mo ");
                                FinAttributeSummaryQry.Append("                        ON  mo.ID = ma.Attr_" + attributeID + " ");
                                break;
                        }
                        //if (AttributeDetail.IsSpecial)
                        //{
                        //    switch ((SystemDefinedAttributes)AttributeDetail.Id)
                        //    { 
                        //    }
                        //}
                        //else
                        //{ 

                        //}

                        FinAttributeSummaryQry.Append("        )                      AttrWise ");
                        FinAttributeSummaryQry.Append(" GROUP BY ");
                        FinAttributeSummaryQry.Append("        AttrWise.col1 ");

                        return tx.PersistenceManager.ReportRepository.ExecuteQuery(FinAttributeSummaryQry.ToString());
                    }
                }

            }
            catch
            {

            }
            return null;

        }

        public IList GetEntityFinancialSummaryDetl(ReportManagerProxy proxy, string EntityTypeID, List<string> AttributeIDs, List<int> FinancialAttributes)
        {


            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    StringBuilder innerqry = new StringBuilder();



                    int[] strEntityTypeIds = EntityTypeID.Split(',').Select(int.Parse).ToArray();

                    innerqry.Append("(");
                    for (var i = 0; i < strEntityTypeIds.Length; i++)
                    {
                        innerqry.Append("SELECT id FROM MM_AttributeRecord_" + strEntityTypeIds[i]);

                        if (i < (strEntityTypeIds.Length - 1))
                        {
                            innerqry.Append(" union all ");
                        }
                    }

                    innerqry.Append(")");




                    string inClause = "("
                                               + String.Join(",", strEntityTypeIds.Select(x => x.ToString()).ToArray())
                                             + ")";


                    string SelectedIds = String.Join(",", proxy.MarcomManager.EntitySortorderIdColle.Select(a => a.EntityIds.ToString()).ToArray());

                    StringBuilder EntityFinQry = new StringBuilder();

                    EntityFinQry.Append(" SELECT Att.ID as EntityID  ");

                    for (var i = 0; i < FinancialAttributes.Count(); i++)
                    {
                        switch ((FinancialAttributesIds)FinancialAttributes[i])
                        {
                            case FinancialAttributesIds.ID:
                                EntityFinQry.Append("   ,Att.ID AS [col" + FinancialAttributes[i] + "] ");
                                break;
                            case FinancialAttributesIds.Name:
                                EntityFinQry.Append("   ,isnull(ent.Name,'-') AS [col" + FinancialAttributes[i] + "] ");
                                break;
                            //case FinancialAttributesIds.TotalNoOfItems:
                            //    EntityFinQry.Append("   ,0 AS [col" + FinancialAttributes[i] + "] ");
                            //    break;
                            case FinancialAttributesIds.Planned:
                                EntityFinQry.Append("   ,replace(cast(isnull(findata.Planned,0) as varchar(50)),'.00','') +' " + proxy.MarcomManager.GlobalAdditionalSettings[1].CurrencyFormatvalue[0].ShortName.ToUpper() + "' AS [col" + FinancialAttributes[i] + "] ");
                                break;
                            case FinancialAttributesIds.Requested:
                                EntityFinQry.Append("   ,replace(cast(isnull(findata.Requested,0) as varchar(50)),'.00','') +' " + proxy.MarcomManager.GlobalAdditionalSettings[1].CurrencyFormatvalue[0].ShortName.ToUpper() + "' AS [col" + FinancialAttributes[i] + "] ");
                                break;
                            case FinancialAttributesIds.ApprovedAllocation:
                                EntityFinQry.Append("   ,replace(cast(isnull(findata.ApprovedAllocation,0) as varchar(50)),'.00','') +' " + proxy.MarcomManager.GlobalAdditionalSettings[1].CurrencyFormatvalue[0].ShortName.ToUpper() + "' AS [col" + FinancialAttributes[i] + "] ");
                                break;
                            case FinancialAttributesIds.ApprovedBudget:
                                EntityFinQry.Append("   ,replace(cast(isnull(findata.ApprovedBudget,0) as varchar(50)),'.00','') +' " + proxy.MarcomManager.GlobalAdditionalSettings[1].CurrencyFormatvalue[0].ShortName.ToUpper() + "' AS [col" + FinancialAttributes[i] + "] ");
                                break;
                            case FinancialAttributesIds.BudgetDeviation:
                                EntityFinQry.Append("   ,replace(cast(isnull(findata.BudgetDeviation,0) as varchar(50)),'.00','') +' " + proxy.MarcomManager.GlobalAdditionalSettings[1].CurrencyFormatvalue[0].ShortName.ToUpper() + "' AS [col" + FinancialAttributes[i] + "] ");
                                break;
                            case FinancialAttributesIds.Commited:
                                EntityFinQry.Append("   ,replace(cast(isnull(findata.Commited,0) as varchar(50)),'.00','') +' " + proxy.MarcomManager.GlobalAdditionalSettings[1].CurrencyFormatvalue[0].ShortName.ToUpper() + "' AS [col" + FinancialAttributes[i] + "] ");
                                break;
                            case FinancialAttributesIds.Spent:
                                EntityFinQry.Append("   ,replace(cast(isnull(findata.Spent,0) as varchar(50)),'.00','') +' " + proxy.MarcomManager.GlobalAdditionalSettings[1].CurrencyFormatvalue[0].ShortName.ToUpper() + "' AS [col" + FinancialAttributes[i] + "] ");
                                break;
                            case FinancialAttributesIds.AvailableToSpend:
                                EntityFinQry.Append("   ,replace(cast(isnull(findata.AvailableToSpend,0) as varchar(50)),'.00','') +' " + proxy.MarcomManager.GlobalAdditionalSettings[1].CurrencyFormatvalue[0].ShortName.ToUpper() + "' AS  [col" + FinancialAttributes[i] + "] ");
                                break;
                        }

                    }


                    List<int> ListOfAttribute = new List<int>();

                    foreach (string item in AttributeIDs)
                    {
                        int Num;

                        bool Status = Int32.TryParse(item, out Num);
                        if (Status)
                        {
                            ListOfAttribute.Add(Int32.Parse(item));
                        }
                        else
                        {
                            var attrID = item.Split('_')[0];
                            ListOfAttribute.Add(Int32.Parse(attrID));
                        }
                    }

                    var attributeDao = tx.PersistenceManager.ReportRepository.Query<AttributeDao>().Where(a => ListOfAttribute.Contains(a.Id));



                    if (attributeDao != null)
                    {



                        foreach (var newColumnList in attributeDao)
                        {
                            if (newColumnList.IsSpecial) //Entity Status
                            {
                                if ((SystemDefinedAttributes)newColumnList.Id == SystemDefinedAttributes.Name)
                                {
                                    EntityFinQry.Append(", ent.name ");
                                    EntityFinQry.Append(" AS  [");
                                    EntityFinQry.Append("col" + newColumnList.Id);
                                    EntityFinQry.Append("]");
                                }
                                else if ((SystemDefinedAttributes)newColumnList.Id == SystemDefinedAttributes.EntityStatus)
                                {
                                    EntityFinQry.Append(", isnull((SELECT top 1  metso.StatusOptions FROM MM_EntityStatus mes INNER JOIN MM_EntityTypeStatus_Options metso ON mes.StatusID=metso.ID AND mes.EntityID=ent.ID AND metso.IsRemoved=0),'-') ");
                                    EntityFinQry.Append(" AS  [");
                                    EntityFinQry.Append("col" + newColumnList.Id);
                                    EntityFinQry.Append("]");
                                }
                                else if ((SystemDefinedAttributes)newColumnList.Id == SystemDefinedAttributes.Owner)
                                {
                                    EntityFinQry.Append(",isnull((SELECT (ISNULL(us.FirstName,'') + ' ' + ISNULL(us.LastName,'')) AS VALUE  FROM UM_User us INNER JOIN AM_Entity_Role_User aeru ON us.ID=aeru.UserID  ");
                                    EntityFinQry.Append(" and  aeru.EntityID = ent.ID  INNER JOIN AM_EntityTypeRoleAcl aetra ON aeru.RoleID=aetra.ID AND aetra.EntityRoleID=1  ),'-') ");
                                    EntityFinQry.Append(" AS  [");
                                    EntityFinQry.Append("col" + newColumnList.Id);
                                    EntityFinQry.Append("]");
                                }
                            }
                            else if (newColumnList.AttributeTypeID == (int)AttributesList.TextSingleLine || newColumnList.AttributeTypeID == (int)AttributesList.TextMultiLine || newColumnList.AttributeTypeID == (int)AttributesList.TextMoney)
                            {
                                EntityFinQry.Append(",COALESCE(NULLIF(attr_" + newColumnList.Id + ",''), '-') ");
                                EntityFinQry.Append(" AS  [");
                                EntityFinQry.Append("col" + newColumnList.Id);
                                EntityFinQry.Append("]");
                            }
                            else if (newColumnList.AttributeTypeID == (int)AttributesList.DateTime)
                            {
                                EntityFinQry.Append(",REPLACE( CONVERT(varchar, ISNULL(attr_" + newColumnList.Id + ",''),121),'1900-01-01 00:00:00.000','-') ");   //AS [Date  time],
                                EntityFinQry.Append(" AS  [");
                                EntityFinQry.Append("col" + newColumnList.Id);
                                EntityFinQry.Append("]");

                            }
                            else if (newColumnList.AttributeTypeID == (int)AttributesList.ListSingleSelection)
                            {
                                //var SingleSeAttrs = attributeDao.Where(a => (AttributesList)a.AttributeTypeID == AttributesList.ListMultiSelection).Select(a => a.Id).ToArray();
                                StringBuilder selectOption = new StringBuilder();
                                selectOption.Append(" select child.attr_" + newColumnList.Id + " from (");
                                for (var i = 0; i < strEntityTypeIds.Length; i++)
                                {
                                    selectOption.Append(" SELECT id,");


                                    int collectionCount = tx.PersistenceManager.ReportRepository.Query<EntityTypeAttributeRelationDao>().Where(a => a.EntityTypeID == strEntityTypeIds[i] &&
                                                       a.AttributeID == newColumnList.Id).Count();
                                    if (collectionCount > 0)
                                        selectOption.Append("attr_" + newColumnList.Id);
                                    else
                                        selectOption.Append("0");


                                    selectOption.Append(" as attr_" + newColumnList.Id + " from MM_AttributeRecord_" + strEntityTypeIds[i]);

                                    if (i < (strEntityTypeIds.Length - 1))
                                    {
                                        innerqry.Append(" union all ");
                                    }
                                }
                                selectOption.Append(" ) child");
                                EntityFinQry.Append(",ISNULL( (select top 1 Caption from  mm_option where ID IN( " + selectOption.ToString() + "  where child.ID=Att.ID)),'-') AS  [col" + newColumnList.Id + "] ");
                                //EntityFinQry.Append(",ISNULL( (select top 1 Caption from  mm_option where ID IN(SELECT attr_" + newColumnList.Id + " FROM MM_AttributeRecord_" + EntityTypeID + "  where ID=Att.ID)),'-') AS  [col" + newColumnList.Id + "] ");
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
                                EntityFinQry.Append("WHERE  mms.EntityID=Att.ID and  mms.AttributeID = " + newColumnList.Id + "  ");
                                EntityFinQry.Append("GROUP BY  ");
                                EntityFinQry.Append("mms.EntityID,mms.AttributeID) ),'-')   as [col" + newColumnList.Id + "]");

                            }
                            else if (newColumnList.AttributeTypeID == (int)AttributesList.DropDownTree)
                            {

                                var Treelist = tx.PersistenceManager.ReportRepository.Query<TreeLevelDao>().Where(a => a.AttributeID == newColumnList.Id);
                                foreach (var treecount in Treelist)
                                {
                                    EntityFinQry.Append(",ISNULL( (SELECT top 1 mtn.Caption ");
                                    EntityFinQry.Append("FROM   MM_TreeNode mtn ");
                                    EntityFinQry.Append("INNER JOIN MM_TreeValue mtv ");
                                    EntityFinQry.Append("ON  mtv.NodeID = mtn.ID ");
                                    EntityFinQry.Append("AND mtv.AttributeID = mtn.AttributeID ");
                                    EntityFinQry.Append("WHERE  mtv.AttributeID =" + newColumnList.Id + " ");
                                    EntityFinQry.Append("AND EntityID= Att.ID ");
                                    EntityFinQry.Append("AND mtv.[LEVEL]=" + treecount.Level + "),'-')  AS [col" + newColumnList.Id + "_" + treecount.Level + "] ");
                                }


                            }
                            else if (newColumnList.AttributeTypeID == (int)AttributesList.TreeMultiSelection)
                            {
                                var Treelist = tx.PersistenceManager.ReportRepository.Query<TreeLevelDao>().Where(a => a.AttributeID == newColumnList.Id);
                                foreach (var treecount in Treelist)
                                {
                                    if (Treelist.Count() != treecount.Level)
                                    {
                                        EntityFinQry.Append(",ISNULL( (SELECT top 1 mtn.Caption ");
                                        EntityFinQry.Append("FROM   MM_TreeNode mtn ");
                                        EntityFinQry.Append("INNER JOIN MM_TreeValue mtv ");
                                        EntityFinQry.Append("ON  mtv.NodeID = mtn.ID ");
                                        EntityFinQry.Append("AND mtv.AttributeID = mtn.AttributeID ");
                                        EntityFinQry.Append("WHERE  mtv.AttributeID =" + newColumnList.Id + " ");
                                        EntityFinQry.Append("AND EntityID= Att.ID ");
                                        EntityFinQry.Append("AND mtv.[LEVEL]=" + treecount.Level + "),'-')  AS [col" + newColumnList.Id + "_" + treecount.Level + "] ");
                                    }
                                    else
                                    {
                                        EntityFinQry.Append(",ISNULL( (SELECT  ");
                                        EntityFinQry.Append("STUFF( ");
                                        EntityFinQry.Append("( ");
                                        EntityFinQry.Append("SELECT ', ' +  mtn.Caption ");
                                        EntityFinQry.Append("FROM   MM_TreeNode mtn ");
                                        EntityFinQry.Append("INNER JOIN MM_TreeValue mtv ");
                                        EntityFinQry.Append("ON  mtv.NodeID = mtn.ID and  mtv.AttributeID=" + newColumnList.Id + " ");
                                        EntityFinQry.Append("AND mtn.Level = " + treecount.Level + " WHERE mtv.EntityID = Att.ID AND mtv.AttributeID =" + newColumnList.Id + " ");
                                        EntityFinQry.Append("FOR XML PATH('') ");
                                        EntityFinQry.Append("), ");
                                        EntityFinQry.Append("1, ");
                                        EntityFinQry.Append("2, ");
                                        EntityFinQry.Append("'' ");
                                        EntityFinQry.Append(") ),'-') AS [col" + newColumnList.Id + "_" + treecount.Level + "] ");
                                    }
                                }
                            }
                            else if (newColumnList.AttributeTypeID == (int)AttributesList.CheckBoxSelection)
                            {
                                EntityFinQry.Append(",CASE when attr_" + newColumnList.Id + " = 1 THEN 'True' ");
                                EntityFinQry.Append(" when attr_" + newColumnList.Id + " = 0 THEN 'False' ELSE '-' END ");
                                EntityFinQry.Append(" AS  [");
                                EntityFinQry.Append("col" + newColumnList.Id);
                                EntityFinQry.Append("]");
                            }
                            else if (newColumnList.AttributeTypeID == (int)AttributesList.Period)
                            {


                                foreach (string attr in AttributeIDs)
                                {

                                    if (attr.IndexOf(newColumnList.Id + "_") == 0)
                                    {

                                        if (attr.Split('_')[1] == "1")
                                        {
                                            EntityFinQry.Append(",( SELECT REPLACE(( SELECT ISNULL((SELECT MIN(ISNULL(Startdate, '-'))");
                                            EntityFinQry.Append("FROM   PM_EntityPeriod ");
                                            EntityFinQry.Append(" WHERE  EntityID = Att.ID),'')),'1900-01-01','-') ) as col" + newColumnList.Id + "_1 ");
                                        }
                                        else if (attr.Split('_')[1] == "2")
                                        {
                                            EntityFinQry.Append(",( SELECT REPLACE(( SELECT ISNULL((SELECT MAX(ISNULL(EndDate, '-'))");
                                            EntityFinQry.Append("FROM   PM_EntityPeriod ");
                                            EntityFinQry.Append(" WHERE  EntityID = Att.ID),'')),'1900-01-01','-') ) as col" + newColumnList.Id + "_2 ");
                                        }

                                    }

                                }

                            }

                        }

                        //EntityFinQry.Append(" from MM_AttributeRecord_" + EntityTypeID);
                        EntityFinQry.Append(" from " + innerqry.ToString());
                        EntityFinQry.Append(" AS Att INNER JOIN PM_Entity AS  ent ON ent.Id= Att.ID and ent.active=1 ");
                        EntityFinQry.Append(" INNER JOIN   ");
                        EntityFinQry.Append("  (SELECT fin.EntityID,  ");
                        EntityFinQry.Append("                    SUM(fin.TotalPlannedAmount) AS Planned,  ");
                        EntityFinQry.Append("                    SUM(fin.TotalRequested) AS Requested,  ");
                        EntityFinQry.Append("                    SUM(fin.TotalApprovedAmount) AS ApprovedAllocation,  ");
                        EntityFinQry.Append("                    SUM(fin.ApprovedBudget) AS ApprovedBudget,  ");
                        EntityFinQry.Append("                    SUM(fin.BudgetDeviation) AS BudgetDeviation,  ");
                        EntityFinQry.Append("                    SUM(fin.TotalCommitedAmount) AS Commited,  ");
                        EntityFinQry.Append("                    SUM(fin.TotalSpentAmount) AS Spent,  ");
                        EntityFinQry.Append("                    SUM(fin.TotalApprovedAmount) - SUM(fin.TotalSpentAmount) AS   ");
                        EntityFinQry.Append("                    AvailableToSpend  ");
                        EntityFinQry.Append("             FROM   (  ");
                        EntityFinQry.Append("                        SELECT pefav.EntityID,  ");
                        EntityFinQry.Append(" case when pe.level=0 then  ( SELECT SUM(pf.PlannedAmount)");
                        EntityFinQry.Append(" FROM   PM_Financial pf");
                        EntityFinQry.Append("  INNER JOIN PM_Entity pe2");
                        EntityFinQry.Append("  ON  pe2.ID = pf.EntityID");
                        EntityFinQry.Append("  AND pe2.UniqueKey LIKE pe.UniqueKey  + '.%'");
                        EntityFinQry.Append("  AND pe2.[Level] = 1");
                        EntityFinQry.Append("  AND pe2.[Active] = 1 AND pf.CostCenterID=pefav.CostCenterID) else ");
                        EntityFinQry.Append("                               pefav.PlannedAmount end AS TotalPlannedAmount,  ");
                        //EntityFinQry.Append("                               pefav.PlannedAmount AS TotalPlannedAmount,  ");
                        EntityFinQry.Append("                               pefav.RequestedAmount AS TotalRequested,  ");
                        EntityFinQry.Append("                               pefav.ApprovedAllocatedAmount AS   ");
                        EntityFinQry.Append("                               TotalApprovedAmount,  ");
                        EntityFinQry.Append("                               ISNULL(  ");
                        EntityFinQry.Append("                                   (  ");
                        EntityFinQry.Append("                                       SELECT SUM(pefav2.Spent) AS Spent  ");
                        EntityFinQry.Append("                                       FROM   PM_Financial pefav2  ");
                        EntityFinQry.Append("                                              INNER JOIN PM_Entity pe2  ");
                        EntityFinQry.Append("                                                   ON  pe2.ID = pefav2.EntityID  ");
                        EntityFinQry.Append("                                                   AND pe2.[Active] = 1  ");
                        EntityFinQry.Append("                                                   AND pe2.TypeId IN (SELECT  ");
                        EntityFinQry.Append("                                                                                       met.ID ");
                        EntityFinQry.Append("                                                                                FROM    ");
                        EntityFinQry.Append("                                                                                       MM_EntityType  ");
                        EntityFinQry.Append("                                                                                       met ");
                        EntityFinQry.Append("                                                                                WHERE   ");
                        EntityFinQry.Append("                                                                                       met.IsAssociate =  ");
                        EntityFinQry.Append("                                                                                       0 ");
                        EntityFinQry.Append("                                                                                       AND  ");
                        EntityFinQry.Append("                                                                                           met.ID  ");
                        EntityFinQry.Append("                                                                                           NOT IN (5, 10)) ");
                        EntityFinQry.Append("                                                   AND pefav2.CostCenterID = pefav.CostCenterID  ");
                        EntityFinQry.Append("                                       WHERE  pe2.UniqueKey LIKE pe.UniqueKey +   ");
                        EntityFinQry.Append("                                              '%'  ");
                        EntityFinQry.Append("                                   ),  ");
                        EntityFinQry.Append("                                   0  ");
                        EntityFinQry.Append("                               )    AS TotalSpentAmount,  ");
                        EntityFinQry.Append("                               ISNULL(  ");
                        EntityFinQry.Append("                                   (  ");
                        EntityFinQry.Append("                                       SELECT SUM(pefav2.Commited) AS Commited  ");
                        EntityFinQry.Append("                                       FROM   PM_Financial pefav2  ");
                        EntityFinQry.Append("                                              INNER JOIN PM_Entity pe2  ");
                        EntityFinQry.Append("                                                   ON  pe2.ID = pefav2.EntityID  ");
                        EntityFinQry.Append("                                                   AND pe2.[Active] = 1  ");
                        EntityFinQry.Append("                                                   AND pe2.TypeId IN (SELECT  ");
                        EntityFinQry.Append("                                                                                       met.ID ");
                        EntityFinQry.Append("                                                                                FROM    ");
                        EntityFinQry.Append("                                                                                       MM_EntityType  ");
                        EntityFinQry.Append("                                                                                       met ");
                        EntityFinQry.Append("                                                                                WHERE   ");
                        EntityFinQry.Append("                                                                                       met.IsAssociate =  ");
                        EntityFinQry.Append("                                                                                       0 ");
                        EntityFinQry.Append("                                                                                       AND  ");
                        EntityFinQry.Append("                                                                                           met.ID  ");
                        EntityFinQry.Append("                                                                                           NOT IN (5, 10)) ");
                        EntityFinQry.Append("                                                   AND pefav2.CostCenterID = pefav.CostCenterID  ");
                        EntityFinQry.Append("                                       WHERE  pe2.UniqueKey LIKE pe.UniqueKey +   ");
                        EntityFinQry.Append("                                              '%'  ");
                        EntityFinQry.Append("                                   ),  ");
                        EntityFinQry.Append("                                   0  ");
                        EntityFinQry.Append("                               )    AS TotalCommitedAmount,  ");
                        EntityFinQry.Append("                               pefav.ApprovedBudget AS ApprovedBudget,  ");
                        EntityFinQry.Append("                               CASE   ");
                        EntityFinQry.Append("                                    WHEN pefav.ApprovedBudgetDate IS NULL THEN 0  ");
                        EntityFinQry.Append("                                    WHEN (pefav.ApprovedBudget - pefav.ApprovedAllocatedAmount)   ");
                        EntityFinQry.Append("                                         < 0 THEN 0  ");
                        EntityFinQry.Append("                                    ELSE pefav.ApprovedBudget - pefav.ApprovedAllocatedAmount  ");
                        EntityFinQry.Append("                               END  AS BudgetDeviation  ");
                        EntityFinQry.Append("                        FROM   PM_Financial pefav  ");
                        EntityFinQry.Append("                               INNER JOIN PM_Entity pe  ");
                        EntityFinQry.Append("                                    ON  pe.ID = pefav.EntityID  ");
                        EntityFinQry.Append("                                    AND pe.[Active] = 1  ");
                        EntityFinQry.Append("                    ) AS fin  ");
                        EntityFinQry.Append("             GROUP BY  ");
                        EntityFinQry.Append("                    fin.EntityID  ");
                        EntityFinQry.Append("  ) AS findata  ");
                        EntityFinQry.Append("  ON findata.EntityID = ent.ID ");
                        EntityFinQry.Append("  WHERE ent.ID IN (" + SelectedIds + ")  AND ent.TypeID in " + inClause);


                    }


                    return tx.PersistenceManager.MetadataRepository.ExecuteQuery(EntityFinQry.ToString());
                }
            }
            catch (Exception ex)
            {

            }

            return null;
        }

        public Guid? GetStrucuralRptDetail(ReportManagerProxy proxy, ListSettings listSetting, bool IsshowFinancialDetl, bool IsDetailIncluded, bool IsshowTaskDetl, bool IsshowMemberDetl, int ExpandingEntityIDStr, bool IncludeChildrenStr)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    IList<IReportContainer> rptcollection = new List<IReportContainer>();
                    StringBuilder EntityFinQry = new StringBuilder();

                    int[] SelectedEntityIds = { };
                    int[] EntityTypeID = { };

                    SelectedEntityIds = proxy.MarcomManager.EntitySortorderIdColle.Select(a => a.EntityIds).ToArray();
                    if (ExpandingEntityIDStr != 0 && IncludeChildrenStr == false)
                    {
                        int[] singleentity = { ExpandingEntityIDStr };
                        SelectedEntityIds = proxy.MarcomManager.EntitySortorderIdColle.Where(a => singleentity.Contains(a.EntityIds)).Select(a => a.EntityIds).ToArray();
                    }
                    else if (ExpandingEntityIDStr != 0 && IncludeChildrenStr == true)
                    {
                        var totalchildrenIDarr = new StringBuilder();
                        totalchildrenIDarr.Append(" SELECT pe.ID as 'entityid' FROM   PM_Entity pe INNER JOIN MM_EntityType met ON  pe.TypeID = met.id AND met.IsAssociate = 0 AND met.Category = 2 AND pe.[Active] = 1 where  pe.ID = " + ExpandingEntityIDStr + " or pe.UniqueKey LIKE  (SELECT pe1.UniqueKey FROM PM_Entity pe1 WHERE pe1.id=" + ExpandingEntityIDStr + ")+ '.%'  ORDER BY pe.UniqueKey asc ");
                        IList totalchildrenIDobj = tx.PersistenceManager.PlanningRepository.ExecuteQuery(totalchildrenIDarr.ToString());
                        int[] IdArr = totalchildrenIDobj.Cast<dynamic>().Select(a => (int)a["entityid"]).ToArray().Select(a => a).ToArray();

                        SelectedEntityIds = proxy.MarcomManager.EntitySortorderIdColle.Where(a => IdArr.Contains(a.EntityIds)).Select(a => a.EntityIds).ToArray();

                    }

                    var entityObj = (from tbl1 in tx.PersistenceManager.ReportRepository.Query<EntityDao>() where SelectedEntityIds.Contains(tbl1.Id) select tbl1);

                    var BasicEntityData = (from tbl1 in entityObj
                                           join tbl2 in tx.PersistenceManager.ReportRepository.Query<EntityTypeDao>()
                                           on tbl1.Typeid equals tbl2.Id
                                           select new { Name = tbl1.Name, ShortDescription = tbl2.ShortDescription, ColorCode = tbl2.ColorCode, ID = tbl1.Id, TypeID = tbl1.Typeid, Level = tbl1.Level });
                    string InClause = String.Join(",", SelectedEntityIds.Select(a => a.ToString()).ToArray());
                    List<int> sleenitylisttypeid = new List<int>();
                    foreach (var currentval in BasicEntityData)
                    {
                        sleenitylisttypeid.Add(currentval.TypeID);
                        rptcollection.Add(new BrandSystems.Marcom.Core.Report.ReportContainer { ID = currentval.ID, TypeID = currentval.TypeID, Name = currentval.Name, ShortDescription = currentval.ShortDescription, ColorCode = currentval.ColorCode, Level = currentval.Level });
                    }

                    if (sleenitylisttypeid.Count() == 0)
                    {
                        EntityTypeID = listSetting.EntityTypes.ToArray();
                    }
                    else {
                        EntityTypeID = sleenitylisttypeid.Select(a => a).ToArray();
                    }

                    String CollectionEntitypes = String.Join(",", EntityTypeID.Select(a => a.ToString()).ToArray());
                    if (IsDetailIncluded)
                    {
                        //int[] AttributeIDs = null;

                        int[] AttributeIDs = listSetting.Attributes.Select(a => a.Id).ToArray();

                        int[] BlockAttrIDs = { (int)SystemDefinedAttributes.Name };

                        string xmlpath = tx.PersistenceManager.MetadataRepository.GetXmlPath(MarcomManagerFactory.ActiveMetadataVersionNumber);
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
                                        EntityFinQry.Append(",(SELECT (ISNULL(us.FirstName,'') + ' ' + ISNULL(us.LastName,'')) AS VALUE  FROM UM_User us INNER JOIN AM_Entity_Role_User aeru ON us.ID=aeru.UserID  ");
                                        EntityFinQry.Append(" and  aeru.EntityID = ent.ID INNER JOIN AM_EntityTypeRoleAcl aetra ON aeru.RoleID=aetra.ID AND aetra.EntityRoleID=1    ) ");
                                        EntityFinQry.Append(" AS  [");
                                        EntityFinQry.Append(newColumnList.AttributeId);
                                        EntityFinQry.Append("]");
                                        attrColection.Add(newColumnList.AttributeId.ToString(), newColumnList.Caption);

                                    }
                                }
                                else if (newColumnList.AttributeTypeID == (int)AttributesList.TextSingleLine || newColumnList.AttributeTypeID == (int)AttributesList.TextMultiLine || newColumnList.AttributeTypeID == (int)AttributesList.TextMoney)
                                {
                                    EntityFinQry.Append(",COALESCE(NULLIF(" + newColumnList.ColumnName + ",''), '-') ");
                                    EntityFinQry.Append(" AS  [");
                                    EntityFinQry.Append(newColumnList.AttributeId);
                                    EntityFinQry.Append("]");
                                    attrColection.Add(newColumnList.AttributeId.ToString(), newColumnList.Caption);
                                }
                                else if (newColumnList.AttributeTypeID == (int)AttributesList.DateTime)
                                {
                                    EntityFinQry.Append(",REPLACE( CONVERT(varchar, ISNULL(" + newColumnList.ColumnName + ",''),121),'1900-01-01 00:00:00.000','-') ");   //AS [Date  time],
                                    EntityFinQry.Append(" AS  [");
                                    EntityFinQry.Append(newColumnList.AttributeId);
                                    EntityFinQry.Append("]");
                                    attrColection.Add(newColumnList.AttributeId.ToString(), newColumnList.Caption);

                                }
                                else if (newColumnList.AttributeTypeID == (int)AttributesList.ListSingleSelection)
                                {
                                    EntityFinQry.Append(",ISNULL( (select top 1 Caption from  mm_option where ID IN(SELECT " + newColumnList.ColumnName + " FROM " + scourcetablename + "  where ID=Att.ID)),'-') AS  [" + newColumnList.AttributeId + "] ");
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
                                    EntityFinQry.Append("mms.EntityID,mms.AttributeID) ),'-')   as [" + newColumnList.AttributeId + "]");
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
                                        EntityFinQry.Append("AND mtv.[LEVEL]=" + treecount.Level + "),'-')  AS [" + newColumnList.AttributeId + "_" + treecount.Level + "] ");
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
                                            EntityFinQry.Append("AND mtv.[LEVEL]=" + treecount.Level + "),'-')  AS [" + newColumnList.AttributeId + "_" + treecount.Level + "] ");
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
                                            EntityFinQry.Append(") ),'-') AS [" + newColumnList.AttributeId + "_" + treecount.Level + "] ");
                                            attrColection.Add(newColumnList.AttributeId + "_" + treecount.Level, treecount.LevelName);
                                        }
                                    }
                                }
                                else if (newColumnList.AttributeTypeID == (int)AttributesList.CheckBoxSelection)
                                {
                                    EntityFinQry.Append(",CASE when " + newColumnList.ColumnName + " = 1 THEN 'True' ");
                                    EntityFinQry.Append(" when " + newColumnList.ColumnName + " = 0 THEN 'False' ELSE '-' END ");
                                    EntityFinQry.Append(" AS  [");
                                    EntityFinQry.Append(newColumnList.AttributeId);
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

                            if (MetadataResult.Count() > 0)
                            {
                                foreach (var CurrentMetadata in MetadataResult)
                                {
                                    int currentIndex = rptcollection.Select(a => a.ID).ToList().IndexOf(Convert.ToInt32(CurrentMetadata["ID"]));
                                    if (currentIndex != -1)
                                    {
                                        rptcollection[currentIndex].MetadataCollections = CurrentMetadata;
                                        rptcollection[currentIndex].MetadataColumnCollection = attrColection;
                                    }
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
                                            objMemberCollection.Add(MemberResult[i]);

                                        }
                                        else
                                        {
                                            objMemberCollection = null;
                                            objMemberCollection = new List<Hashtable>();
                                            objMemberCollection.Add(MemberResult[i]);
                                        }
                                        rptcollection[currentMemberdata].MemberCollections = objMemberCollection;
                                        objMemberCollection = null;
                                        objMemberCollection = new List<Hashtable>();

                                    }
                                    else
                                    {
                                        objMemberCollection.Add(MemberResult[i]);
                                    }
                                }
                                else
                                {
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



                        EntityFinQry.Append("  SELECT fin.EntityID  AS ID, ");
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
                        EntityFinQry.Append("         )             AS NAME ");
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
                        EntityFinQry.Append("                                              AND pe2.TypeId IN (" + CollectionEntitypes + ") ");
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
                        EntityFinQry.Append("                                              AND pe2.TypeId IN (" + CollectionEntitypes + ") ");
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
                        EntityFinQry.Append("                                              AND pe2.TypeId IN (" + CollectionEntitypes + ") ");
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
                                            objFinanceCollection.Add(FinancialResult[i]);

                                        }
                                        else
                                        {
                                            objFinanceCollection = null;
                                            objFinanceCollection = new List<Hashtable>();
                                            objFinanceCollection.Add(FinancialResult[i]);
                                        }
                                        rptcollection[currentFinancedata].FinancialCollections = objFinanceCollection;
                                        objFinanceCollection = null;
                                        objFinanceCollection = new List<Hashtable>();

                                    }
                                    else
                                    {
                                        objFinanceCollection.Add(FinancialResult[i]);
                                    }
                                }
                                else
                                {
                                    objFinanceCollection.Add(FinancialResult[i]);
                                    rptcollection[currentFinancedata].FinancialCollections = objFinanceCollection;
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
                                            objTaskListCollection.Add(TaskListResult[i]);
                                        }
                                        else
                                        {
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
                                        objTaskListCollection.Add(TaskListResult[i]);
                                    }
                                }
                                else
                                {
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
                                            objTaskCollection.Add(TaskDetlResult[i]);

                                        }
                                        else
                                        {
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
                                        objTaskCollection.Add(TaskDetlResult[i]);
                                    }
                                }
                                else
                                {
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
                                rptcollection[currentIndex].TaskOverviewSummary = Currentval;
                            }
                        }

                    }

                    tx.Commit();
                    string CurrencyFormat = proxy.MarcomManager.GlobalAdditionalSettings[1].CurrencyFormatvalue[0].ShortName;
                    string CurrencySymbol = proxy.MarcomManager.GlobalAdditionalSettings[1].CurrencyFormatvalue[0].Symbol;
                    return GenerateStructuralReportExcel(rptcollection, IsshowFinancialDetl, IsDetailIncluded, IsshowTaskDetl, IsshowMemberDetl, ExpandingEntityIDStr, IncludeChildrenStr, CurrencyFormat, CurrencySymbol);

                }
            }
            catch (Exception ex)
            {

            }
            return null;

        }


        int StructuralRowNo;
        int StructuralColumnNo;
        //bool IsFinancialIncluded = true;
        //bool IsDetailIncluded = true;
        //bool IsTaskIncluded = true;
        //bool IsMemberIncluded = true;
        public Guid? GenerateStructuralReportExcel(IList<IReportContainer> rptContainer, bool IsFinancialIncluded, bool IsDetailIncluded, bool IsTaskIncluded, bool IsMemberIncluded, int ExpandingEntityIDStr, bool IncludeChildrenStr, string CurrencyFormat = null, string CurrencySymbol=null)
        {


            //    int StartRowNo = 4;
            StructuralRowNo = 6;
            StructuralColumnNo = 2;


            Guid NewGuid = Guid.NewGuid();

            string fullpath = AppDomain.CurrentDomain.BaseDirectory + ("/Files/ReportFiles/Images/Temp/") + NewGuid + ".xlsx";
            //var fullpath = @"C:\reports\" + Guid.NewGuid() + ".xlsx";
            FileInfo newFile = new FileInfo(fullpath);

            ExcelPackage pck = new ExcelPackage(newFile);

            pck.Workbook.Properties.Title = "Structure Report";
            pck.Workbook.Properties.Author = "Marcom Plarform";

            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Structure Report");

            ws.View.ShowGridLines = false;

            int ClNo = 2;
            int maxCol = 4;
            if (IsDetailIncluded)
            {
                maxCol = maxCol + 2;
            }
            if (IsTaskIncluded)
            {
                maxCol = maxCol + 6;
            }
            if (!IsTaskIncluded & IsMemberIncluded)
            {
                maxCol = maxCol + 2;
            }
            if (IsFinancialIncluded)
            {
                maxCol = maxCol + 7;
            }

            ExcelRange HeaderCell = ws.Cells[ClNo, ClNo, ClNo, maxCol];
            HeaderCell.Merge = true;
            HeaderCell.Value = "Structure Report";

            HeaderCell.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            HeaderCell.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.FromArgb(165, 165, 165));

            HeaderCell.Style.Font.Name = "Calibri";
            HeaderCell.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(64, 64, 64));
            HeaderCell.Style.Font.Size = 26;

            ws.Column(ClNo).Width = 18;
            ClNo += 1;
            ws.Column(ClNo).Width = 35;
            ClNo += 1;


            if (IsDetailIncluded)
            {
                ws.Column(ClNo).Width = 25;
                ClNo += 1;
                ws.Column(ClNo).Width = 40;
                ClNo += 1;
            }


            if (IsTaskIncluded | IsMemberIncluded)
            {
                ws.Column(ClNo).Width = 25;
                ClNo += 1;
                ws.Column(ClNo).Width = 25;
                ClNo += 1;
            }

            if (IsTaskIncluded)
            {
                ws.Column(ClNo).Width = 50;
                ClNo += 1;
                ws.Column(ClNo).Width = 25;
                ClNo += 1;
                ws.Column(ClNo).Width = 25;
                ClNo += 1;
                ws.Column(ClNo).Width = 25;
                ClNo += 1;
            }

            if (IsFinancialIncluded)
            {
                ws.Column(ClNo).Width = 30;
                ClNo += 1;
                ws.Column(ClNo).Width = 18;
                ClNo += 1;
                ws.Column(ClNo).Width = 18;
                ClNo += 1;
                ws.Column(ClNo).Width = 18;
                ClNo += 1;
                ws.Column(ClNo).Width = 18;
                ClNo += 1;
                ws.Column(ClNo).Width = 18;
                ClNo += 1;
                ws.Column(ClNo).Width = 18;
                ClNo += 1;
                ws.Column(ClNo).Width = 18;
                ClNo += 1;
                ws.Column(ClNo).Width = 18;
                ClNo += 1;
            }

            DrawStructure(ws, rptContainer, IsFinancialIncluded, IsDetailIncluded, IsTaskIncluded, IsMemberIncluded, CurrencyFormat, CurrencySymbol);


            ExcelRange _with1 = ws.Cells[StructuralRowNo, 3, StructuralRowNo, 19];

            _with1.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            _with1.Style.Border.Top.Color.SetColor(System.Drawing.Color.FromArgb(217, 217, 217));

            _with1.Style.Font.Name = "Calibri";
            _with1.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(64, 64, 64));
            _with1.Style.Font.Size = 26;


            // ws.Row(3).Height = 50
            for (int i = 6; i <= StructuralRowNo; i++)
            {
                ws.Row(i).Height = 22;
            }


            pck.Save();

            return NewGuid;

            //Process.Start(fullpath);

        }

        public void DrawStructure(ExcelWorksheet ws, IList<IReportContainer> data, bool IsFinancialIncluded, bool IsDetailIncluded, bool IsTaskIncluded, bool IsMemberIncluded, string CurrencyFormat = null, string CurrencySymbol = null)
        {
            try
            {
                if (data.Count > 0)
                {
                    foreach (IReportContainer item in data)
                    {
                        //Take no of attribute as the maximum no of row available for the Entity
                        int MaxRowHeight = 0;
                        int EntityRowNo = StructuralRowNo;


                        if (IsDetailIncluded)
                        {
                            MaxRowHeight = item.MetadataCollections.Count;
                        }
                        if (IsTaskIncluded)
                        {

                            int MaxRowHeightForTask = 0;
                            if (item.TaskListCollections != null)
                                MaxRowHeightForTask = item.TaskListCollections.Count * 2 + 1;

                            if (item.TaskCollections != null)
                                MaxRowHeightForTask = MaxRowHeightForTask + item.TaskCollections.Count;

                            if (MaxRowHeight < MaxRowHeightForTask)
                                MaxRowHeight = MaxRowHeightForTask;

                            //if (item.TaskCollections != null && item.TaskListCollections != null && MaxRowHeight < item.TaskCollections.Count + (item.TaskListCollections.Count + 1))
                            //    MaxRowHeight = item.TaskCollections.Count + (item.TaskListCollections.Count + 1);

                            if (IsMemberIncluded)
                            {

                                int MaxRowHeightForMemberTaskSummary = 0;
                                if (item.MemberCollections != null)
                                    MaxRowHeightForMemberTaskSummary = item.MemberCollections.Count + 1;



                                if (item.TaskOverviewSummary != null)
                                    MaxRowHeightForMemberTaskSummary = MaxRowHeightForMemberTaskSummary + 5;
                                else
                                {
                                    MaxRowHeightForMemberTaskSummary = MaxRowHeightForMemberTaskSummary + 1;
                                }

                                if (MaxRowHeight < MaxRowHeightForMemberTaskSummary)
                                    MaxRowHeight = MaxRowHeightForMemberTaskSummary;

                                //if (item.MemberCollections != null && MaxRowHeight < item.MemberCollections.Count + 6){ // 1 for Task summary header, 5 for task summary content, 1 for space between member and task summary, 1 for member header, 1 for space after member block
                                //    MaxRowHeight = item.MemberCollections.Count + 6;
                                //}
                            }
                        }
                        if (!IsTaskIncluded && IsMemberIncluded)
                        {
                            if (item.MemberCollections != null && MaxRowHeight < item.MemberCollections.Count + 2) // 1 for member header
                                MaxRowHeight = item.MemberCollections.Count + 2;
                        }

                        if (IsFinancialIncluded)
                        {
                            if (item.FinancialCollections != null && MaxRowHeight < item.FinancialCollections.Count + 2) //1 for financial Summary and 1 for financial header
                                MaxRowHeight = item.FinancialCollections.Count + 2;
                        }

                        //Draw the First row of the Entity Start

                        //Draw Sort Description 

                        int globalCountColumn = 2;

                        ExcelRange ShortDescriptionCell = ws.Cells[EntityRowNo, globalCountColumn, EntityRowNo + MaxRowHeight, globalCountColumn];
                        ShortDescriptionCell.Merge = true;
                        ShortDescriptionCell.Value = item.ShortDescription;


                        ShortDescriptionCell.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        ShortDescriptionCell.Style.Border.Left.Color.SetColor(System.Drawing.Color.FromArgb(217, 217, 217));

                        ShortDescriptionCell.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        ShortDescriptionCell.Style.Border.Right.Color.SetColor(System.Drawing.Color.FromArgb(217, 217, 217));

                        ShortDescriptionCell.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        ShortDescriptionCell.Style.Border.Top.Color.SetColor(System.Drawing.Color.FromArgb(217, 217, 217));

                        ShortDescriptionCell.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        ShortDescriptionCell.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.FromArgb(217, 217, 217));

                        ShortDescriptionCell.Style.Font.Name = "Calibri";
                        ShortDescriptionCell.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(255, 255, 255));
                        ShortDescriptionCell.Style.Font.Size = 18;
                        ShortDescriptionCell.Style.Font.Bold = true;

                        ShortDescriptionCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        ShortDescriptionCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(230, 230, 230));

                        ShortDescriptionCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        ShortDescriptionCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#" + item.ColorCode));
                        ShortDescriptionCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ShortDescriptionCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        ShortDescriptionCell.Style.WrapText = true;



                        ShortDescriptionCell.Style.Indent = item.Level;

                        globalCountColumn += 1;

                        //Draw Entity Name
                        ExcelRange EntityNameCell = ws.Cells[EntityRowNo, globalCountColumn, EntityRowNo + MaxRowHeight, globalCountColumn];
                        EntityNameCell.Merge = true;
                        EntityNameCell.Value = HttpUtility.HtmlDecode(item.Name); //Server.HtmlDecode(Item.Name);

                        EntityNameCell.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        EntityNameCell.Style.Border.Left.Color.SetColor(System.Drawing.Color.FromArgb(217, 217, 217));

                        EntityNameCell.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        EntityNameCell.Style.Border.Right.Color.SetColor(System.Drawing.Color.FromArgb(217, 217, 217));

                        EntityNameCell.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        EntityNameCell.Style.Border.Top.Color.SetColor(System.Drawing.Color.FromArgb(217, 217, 217));

                        EntityNameCell.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        EntityNameCell.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.FromArgb(217, 217, 217));

                        EntityNameCell.Style.Font.Name = "Calibri";
                        EntityNameCell.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(64, 64, 64));
                        EntityNameCell.Style.Font.Size = 14;
                        EntityNameCell.Style.Font.Bold = true;

                        EntityNameCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        EntityNameCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(230, 230, 230));

                        EntityNameCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        EntityNameCell.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        EntityNameCell.Style.WrapText = true;


                        EntityNameCell.Style.Indent = item.Level;

                        if (IsDetailIncluded)
                        {
                            globalCountColumn += 1;
                            ExcelRange DetailHeaderCell = ws.Cells[EntityRowNo, globalCountColumn, EntityRowNo, globalCountColumn + 1];
                            DetailHeaderCell.Merge = true;
                            DetailHeaderCell.Value = "DETAILS";

                            DetailHeaderCell.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            DetailHeaderCell.Style.Border.Left.Color.SetColor(System.Drawing.Color.FromArgb(217, 217, 217));

                            DetailHeaderCell.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            DetailHeaderCell.Style.Border.Right.Color.SetColor(System.Drawing.Color.FromArgb(217, 217, 217));

                            DetailHeaderCell.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            DetailHeaderCell.Style.Border.Top.Color.SetColor(System.Drawing.Color.FromArgb(217, 217, 217));

                            DetailHeaderCell.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            DetailHeaderCell.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.FromArgb(217, 217, 217));

                            DetailHeaderCell.Style.Font.Name = "Calibri";
                            DetailHeaderCell.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(64, 64, 64));
                            DetailHeaderCell.Style.Font.Size = 10;
                            DetailHeaderCell.Style.Font.Bold = true;

                            DetailHeaderCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            DetailHeaderCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(230, 230, 230));
                            DetailHeaderCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            DetailHeaderCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            DetailHeaderCell.Style.Indent = 1;
                            DetailHeaderCell.Style.WrapText = true;
                            globalCountColumn += 1;
                        }


                        if (IsTaskIncluded)
                        {
                            globalCountColumn += 1;


                            ExcelRange TaskSummaryHeaderCell = ws.Cells[EntityRowNo, globalCountColumn, EntityRowNo, globalCountColumn + 1];
                            TaskSummaryHeaderCell.Merge = true;

                            TaskSummaryHeaderCell.Value = "TASK SUMMARY";

                            TaskSummaryHeaderCell.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            TaskSummaryHeaderCell.Style.Border.Left.Color.SetColor(System.Drawing.Color.FromArgb(217, 217, 217));

                            TaskSummaryHeaderCell.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            TaskSummaryHeaderCell.Style.Border.Right.Color.SetColor(System.Drawing.Color.FromArgb(217, 217, 217));

                            TaskSummaryHeaderCell.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            TaskSummaryHeaderCell.Style.Border.Top.Color.SetColor(System.Drawing.Color.FromArgb(217, 217, 217));

                            TaskSummaryHeaderCell.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            TaskSummaryHeaderCell.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.FromArgb(217, 217, 217));

                            TaskSummaryHeaderCell.Style.Font.Name = "Calibri";
                            TaskSummaryHeaderCell.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(64, 64, 64));
                            TaskSummaryHeaderCell.Style.Font.Size = 10;
                            TaskSummaryHeaderCell.Style.Font.Bold = true;

                            TaskSummaryHeaderCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            TaskSummaryHeaderCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(230, 230, 230));
                            TaskSummaryHeaderCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            TaskSummaryHeaderCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            TaskSummaryHeaderCell.Style.Indent = 1;
                            TaskSummaryHeaderCell.Style.WrapText = true;


                            ExcelRange TaskHeaderCell = ws.Cells[EntityRowNo, globalCountColumn + 2, EntityRowNo, globalCountColumn + 5];
                            TaskHeaderCell.Merge = true;
                            //     .Value = "WORKFLOW DETAILS"

                            TaskHeaderCell.Value = "TASKS";

                            TaskHeaderCell.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            TaskHeaderCell.Style.Border.Left.Color.SetColor(System.Drawing.Color.FromArgb(217, 217, 217));

                            TaskHeaderCell.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            TaskHeaderCell.Style.Border.Right.Color.SetColor(System.Drawing.Color.FromArgb(217, 217, 217));

                            TaskHeaderCell.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            TaskHeaderCell.Style.Border.Top.Color.SetColor(System.Drawing.Color.FromArgb(217, 217, 217));

                            TaskHeaderCell.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            TaskHeaderCell.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.FromArgb(217, 217, 217));

                            TaskHeaderCell.Style.Font.Name = "Calibri";
                            TaskHeaderCell.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(64, 64, 64));
                            TaskHeaderCell.Style.Font.Size = 10;
                            TaskHeaderCell.Style.Font.Bold = true;

                            TaskHeaderCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            TaskHeaderCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(230, 230, 230));
                            TaskHeaderCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            TaskHeaderCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            TaskHeaderCell.Style.Indent = 1;
                            TaskHeaderCell.Style.WrapText = true;
                            globalCountColumn = globalCountColumn + 5;
                        }
                        if (!IsTaskIncluded & IsMemberIncluded)
                        {
                            globalCountColumn += 2;
                        }
                        if (IsFinancialIncluded)
                        {
                            globalCountColumn += 1;
                            ExcelRange FinancialHeaderCell = ws.Cells[EntityRowNo, globalCountColumn, EntityRowNo, globalCountColumn + 7];
                            FinancialHeaderCell.Merge = true;
                            FinancialHeaderCell.Value = "FINANCIALS";

                            FinancialHeaderCell.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            FinancialHeaderCell.Style.Border.Left.Color.SetColor(System.Drawing.Color.FromArgb(217, 217, 217));

                            FinancialHeaderCell.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            FinancialHeaderCell.Style.Border.Right.Color.SetColor(System.Drawing.Color.FromArgb(217, 217, 217));

                            FinancialHeaderCell.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            FinancialHeaderCell.Style.Border.Top.Color.SetColor(System.Drawing.Color.FromArgb(217, 217, 217));

                            FinancialHeaderCell.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            FinancialHeaderCell.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.FromArgb(217, 217, 217));

                            FinancialHeaderCell.Style.Font.Name = "Calibri";
                            FinancialHeaderCell.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(64, 64, 64));
                            FinancialHeaderCell.Style.Font.Size = 10;
                            FinancialHeaderCell.Style.Font.Bold = true;

                            FinancialHeaderCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            FinancialHeaderCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(230, 230, 230));
                            FinancialHeaderCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            FinancialHeaderCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            FinancialHeaderCell.Style.Indent = 1;
                            FinancialHeaderCell.Style.WrapText = true;
                        }
                        //Draw the First row of the Entity End

                        EntityRowNo = EntityRowNo + 1;

                        //Draw the Attribute for the Entity Start

                        int AttrRowNo = EntityRowNo;
                        int ValueColNumber = 3;
                        if (IsDetailIncluded && item.MetadataCollections != null)
                        {
                            ValueColNumber += 1;

                            foreach (string attr in item.MetadataColumnCollection.Keys)
                            {
                                ExcelRange attrNameCell = ws.Cells[AttrRowNo, ValueColNumber];



                                attrNameCell.Value = item.MetadataColumnCollection[attr];

                                attrNameCell.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                attrNameCell.Style.Border.Left.Color.SetColor(System.Drawing.Color.FromArgb(217, 217, 217));

                                attrNameCell.Style.Font.Name = "Calibri";
                                attrNameCell.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(166, 166, 166));
                                attrNameCell.Style.Font.Size = 10;

                                attrNameCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                attrNameCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(244, 244, 244));
                                attrNameCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                attrNameCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                attrNameCell.Style.Indent = 1;
                                attrNameCell.Style.WrapText = true;

                                var attrValueCell = ws.Cells[AttrRowNo, ValueColNumber + 1];

                                int val;
                                bool res = int.TryParse(item.MetadataCollections[attr].ToString(), out val);
                                if (res == false)
                                {
                                    attrValueCell.Value = item.MetadataCollections[attr].ToString();
                                }
                                else
                                {
                                    //if (item.TaskOverviewSummary != null)
                                    //{
                                    //    attrValueCell.Value = Convert.ToInt32(item.TaskOverviewSummary[key]);//Int32.Parse(item.MetadataCollections[key].ToString());
                                    //}
                                    attrValueCell.Value = item.MetadataCollections[attr];

                                }



                                attrValueCell.Style.Font.Name = "Calibri";
                                attrValueCell.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(64, 64, 64));
                                attrValueCell.Style.Font.Size = 10;

                                attrValueCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                attrValueCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                attrValueCell.Style.Indent = 1;
                                attrValueCell.Style.WrapText = true;

                                AttrRowNo = AttrRowNo + 1;

                            }


                            while (AttrRowNo <= StructuralRowNo + MaxRowHeight)
                            {
                                ExcelRange blankAttrCell = ws.Cells[AttrRowNo, ValueColNumber];
                                blankAttrCell.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                blankAttrCell.Style.Border.Left.Color.SetColor(System.Drawing.Color.FromArgb(217, 217, 217));

                                blankAttrCell.Style.Font.Name = "Calibri";
                                blankAttrCell.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(166, 166, 166));
                                blankAttrCell.Style.Font.Size = 10;

                                blankAttrCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                blankAttrCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(244, 244, 244));
                                blankAttrCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                blankAttrCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                blankAttrCell.Style.Indent = 1;
                                blankAttrCell.Style.WrapText = true;

                                AttrRowNo = AttrRowNo + 1;

                            }
                            ValueColNumber += 1;
                        }
                        //Draw the Attribute for the Entity End


                        int taskIsActive = 1;
                        //Draw the Workflow Summary for the Entity Start
                        if (IsTaskIncluded | IsMemberIncluded)
                        {
                            ValueColNumber += 1;

                            if (IsTaskIncluded)
                            {
                                if (item.TaskOverviewSummary != null)
                                {

                                    var OverallStatusNameCell = ws.Cells[EntityRowNo, ValueColNumber];


                                    OverallStatusNameCell.Value = "Overall status";



                                    OverallStatusNameCell.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    OverallStatusNameCell.Style.Border.Left.Color.SetColor(System.Drawing.Color.FromArgb
                                        (
                                            217, 217, 217));

                                    OverallStatusNameCell.Style.Font.Name = "Calibri";
                                    OverallStatusNameCell.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(166,
                                        166,
                                        166));
                                    OverallStatusNameCell.Style.Font.Size = 10;

                                    OverallStatusNameCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    OverallStatusNameCell.Style.Fill.BackgroundColor.SetColor(
                                        System.Drawing.Color.FromArgb(244, 244, 244));
                                    OverallStatusNameCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    OverallStatusNameCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    OverallStatusNameCell.Style.Indent = 1;
                                    OverallStatusNameCell.Style.WrapText = true;

                                    var OverallStatusValueCell = ws.Cells[EntityRowNo, ValueColNumber + 1];

                                    OverallStatusValueCell.Value = item.TaskOverviewSummary["Name"];



                                    OverallStatusValueCell.Style.Font.Name = "Calibri";
                                    OverallStatusValueCell.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(64,
                                        64,
                                        64));
                                    OverallStatusValueCell.Style.Font.Size = 10;
                                    OverallStatusValueCell.Style.Font.Bold = true;

                                    OverallStatusValueCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    OverallStatusValueCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    OverallStatusValueCell.Style.Indent = 1;
                                    OverallStatusValueCell.Style.WrapText = true;

                                    var TasksInProgressNameCell = ws.Cells[EntityRowNo + taskIsActive, ValueColNumber];


                                    TasksInProgressNameCell.Value = "Tasks in progress";

                                    TasksInProgressNameCell.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    TasksInProgressNameCell.Style.Border.Left.Color.SetColor(
                                        System.Drawing.Color.FromArgb(217, 217, 217));

                                    TasksInProgressNameCell.Style.Font.Name = "Calibri";
                                    TasksInProgressNameCell.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(
                                        166, 166,
                                        166));
                                    TasksInProgressNameCell.Style.Font.Size = 10;

                                    TasksInProgressNameCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    TasksInProgressNameCell.Style.Fill.BackgroundColor.SetColor(
                                        System.Drawing.Color.FromArgb(244, 244, 244));
                                    TasksInProgressNameCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    TasksInProgressNameCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    TasksInProgressNameCell.Style.Indent = 1;
                                    TasksInProgressNameCell.Style.WrapText = true;

                                    var TasksInProgressValueCell =
                                        ws.Cells[EntityRowNo + taskIsActive, ValueColNumber + 1];


                                    TasksInProgressValueCell.Value =
                                        Convert.ToInt32(item.TaskOverviewSummary["TasksInProgress"]);
                                    //Int32.Parse(item.TaskOverviewSummary["TasksInProgress"].ToString());

                                    TasksInProgressValueCell.Style.Font.Name = "Calibri";
                                    TasksInProgressValueCell.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(
                                        64, 64,
                                        64));
                                    TasksInProgressValueCell.Style.Font.Size = 10;

                                    TasksInProgressValueCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    TasksInProgressValueCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    TasksInProgressValueCell.Style.Indent = 1;
                                    TasksInProgressValueCell.Style.WrapText = true;
                                    taskIsActive += 1;

                                    var UnassignedTasksNameCell = ws.Cells[EntityRowNo + taskIsActive, ValueColNumber];


                                    UnassignedTasksNameCell.Value = "Unassigned Tasks";

                                    UnassignedTasksNameCell.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    UnassignedTasksNameCell.Style.Border.Left.Color.SetColor(
                                        System.Drawing.Color.FromArgb(217, 217, 217));

                                    UnassignedTasksNameCell.Style.Font.Name = "Calibri";
                                    UnassignedTasksNameCell.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(
                                        166, 166,
                                        166));
                                    UnassignedTasksNameCell.Style.Font.Size = 10;

                                    UnassignedTasksNameCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    UnassignedTasksNameCell.Style.Fill.BackgroundColor.SetColor(
                                        System.Drawing.Color.FromArgb(244, 244, 244));
                                    UnassignedTasksNameCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    UnassignedTasksNameCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    UnassignedTasksNameCell.Style.Indent = 1;
                                    UnassignedTasksNameCell.Style.WrapText = true;

                                    var UnassignedTasksValueCell =
                                        ws.Cells[EntityRowNo + taskIsActive, ValueColNumber + 1];


                                    UnassignedTasksValueCell.Value =
                                        Convert.ToInt32(item.TaskOverviewSummary["UnassignedTasks"]);
                                    //Int32.Parse(item.TaskOverviewSummary["UnassignedTasks"].ToString());

                                    UnassignedTasksValueCell.Style.Font.Name = "Calibri";
                                    UnassignedTasksValueCell.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(
                                        64, 64,
                                        64));
                                    UnassignedTasksValueCell.Style.Font.Size = 10;

                                    UnassignedTasksValueCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    UnassignedTasksValueCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    UnassignedTasksValueCell.Style.Indent = 1;
                                    UnassignedTasksValueCell.Style.WrapText = true;
                                    taskIsActive += 1;

                                    var OverdueTasksNameCell = ws.Cells[EntityRowNo + taskIsActive, ValueColNumber];


                                    OverdueTasksNameCell.Value = "Overdue Tasks";

                                    OverdueTasksNameCell.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    OverdueTasksNameCell.Style.Border.Left.Color.SetColor(System.Drawing.Color.FromArgb(
                                        217, 217, 217));

                                    OverdueTasksNameCell.Style.Font.Name = "Calibri";
                                    OverdueTasksNameCell.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(166,
                                        166,
                                        166));
                                    OverdueTasksNameCell.Style.Font.Size = 10;

                                    OverdueTasksNameCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    OverdueTasksNameCell.Style.Fill.BackgroundColor.SetColor(
                                        System.Drawing.Color.FromArgb(244, 244, 244));
                                    OverdueTasksNameCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    OverdueTasksNameCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    OverdueTasksNameCell.Style.Indent = 1;
                                    OverdueTasksNameCell.Style.WrapText = true;

                                    var OverdueTasksValueCell = ws.Cells[EntityRowNo + taskIsActive, ValueColNumber + 1];


                                    OverdueTasksValueCell.Value =
                                        Convert.ToInt32(item.TaskOverviewSummary["OverdueTasks"]);
                                    //Int32.Parse(item.TaskOverviewSummary["OverdueTasks"].ToString());

                                    OverdueTasksValueCell.Style.Font.Name = "Calibri";
                                    OverdueTasksValueCell.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(64, 64,
                                        64));
                                    OverdueTasksValueCell.Style.Font.Size = 10;

                                    OverdueTasksValueCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    OverdueTasksValueCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    OverdueTasksValueCell.Style.Indent = 1;
                                    OverdueTasksValueCell.Style.WrapText = true;

                                    taskIsActive += 1;


                                    var UnableToCompleteNameCell = ws.Cells[EntityRowNo + taskIsActive, ValueColNumber];


                                    UnableToCompleteNameCell.Value = "Unable to Complete";

                                    UnableToCompleteNameCell.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    UnableToCompleteNameCell.Style.Border.Left.Color.SetColor(
                                        System.Drawing.Color.FromArgb(217, 217, 217));

                                    UnableToCompleteNameCell.Style.Font.Name = "Calibri";
                                    UnableToCompleteNameCell.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(
                                        166,
                                        166, 166));
                                    UnableToCompleteNameCell.Style.Font.Size = 10;

                                    UnableToCompleteNameCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    UnableToCompleteNameCell.Style.Fill.BackgroundColor.SetColor(
                                        System.Drawing.Color.FromArgb(244, 244, 244));
                                    UnableToCompleteNameCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    UnableToCompleteNameCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    UnableToCompleteNameCell.Style.Indent = 1;
                                    UnableToCompleteNameCell.Style.WrapText = true;

                                    var UnableToCompleteValueCell =
                                        ws.Cells[EntityRowNo + taskIsActive, ValueColNumber + 1];


                                    UnableToCompleteValueCell.Value =
                                        Convert.ToInt32(item.TaskOverviewSummary["UnableToComplete"]);
                                    // Int32.Parse(item.TaskOverviewSummary["UnableToComplete"].ToString());

                                    UnableToCompleteValueCell.Style.Font.Name = "Calibri";
                                    UnableToCompleteValueCell.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(
                                        64, 64,
                                        64));
                                    UnableToCompleteValueCell.Style.Font.Size = 10;

                                    UnableToCompleteValueCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    UnableToCompleteValueCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    UnableToCompleteValueCell.Style.Indent = 1;
                                    UnableToCompleteValueCell.Style.WrapText = true;

                                    taskIsActive += 1;

                                }
                                else
                                {
                                    var NoTaskPresentCell = ws.Cells[EntityRowNo, ValueColNumber];


                                    NoTaskPresentCell.Value = "No tasks are availbale";



                                    NoTaskPresentCell.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    NoTaskPresentCell.Style.Border.Left.Color.SetColor(System.Drawing.Color.FromArgb(
                                        217, 217, 217));

                                    NoTaskPresentCell.Style.Font.Name = "Calibri";
                                    NoTaskPresentCell.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(166, 166,
                                        166));
                                    NoTaskPresentCell.Style.Font.Size = 10;

                                    NoTaskPresentCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    NoTaskPresentCell.Style.Fill.BackgroundColor.SetColor(
                                        System.Drawing.Color.FromArgb(244, 244, 244));
                                    NoTaskPresentCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    NoTaskPresentCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    NoTaskPresentCell.Style.Indent = 1;
                                    NoTaskPresentCell.Style.WrapText = true;


                                }
                            }
                            //Draw the Member for the Entity Start


                            if (IsMemberIncluded && item.MemberCollections != null)
                            {
                                if (!IsTaskIncluded)
                                {
                                    EntityRowNo = EntityRowNo - 2;
                                }
                                var MemberHeaderCell = ws.Cells[EntityRowNo + taskIsActive, ValueColNumber, EntityRowNo + taskIsActive, ValueColNumber + 1];
                                MemberHeaderCell.Merge = true;
                                MemberHeaderCell.Value = "MEMBERS";

                                MemberHeaderCell.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                MemberHeaderCell.Style.Border.Left.Color.SetColor(System.Drawing.Color.FromArgb(217, 217, 217));

                                MemberHeaderCell.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                MemberHeaderCell.Style.Border.Right.Color.SetColor(System.Drawing.Color.FromArgb(217, 217, 217));

                                MemberHeaderCell.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                MemberHeaderCell.Style.Border.Top.Color.SetColor(System.Drawing.Color.FromArgb(217, 217, 217));

                                MemberHeaderCell.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                MemberHeaderCell.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.FromArgb(217, 217, 217));

                                MemberHeaderCell.Style.Font.Name = "Calibri";
                                MemberHeaderCell.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(64, 64, 64));
                                MemberHeaderCell.Style.Font.Size = 10;
                                MemberHeaderCell.Style.Font.Bold = true;

                                MemberHeaderCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                MemberHeaderCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(230, 230, 230));
                                MemberHeaderCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                MemberHeaderCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                MemberHeaderCell.Style.Indent = 1;
                                MemberHeaderCell.Style.WrapText = true;

                                int MRowNo = EntityRowNo + taskIsActive + 1;
                                // 6


                                foreach (Hashtable memb in item.MemberCollections)
                                {
                                    var UserRoleCell = ws.Cells[MRowNo, ValueColNumber];
                                    UserRoleCell.Value = memb["Caption"];

                                    UserRoleCell.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    UserRoleCell.Style.Border.Left.Color.SetColor(System.Drawing.Color.FromArgb(217, 217, 217));

                                    UserRoleCell.Style.Font.Name = "Calibri";
                                    UserRoleCell.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(166, 166, 166));
                                    UserRoleCell.Style.Font.Size = 10;

                                    UserRoleCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    UserRoleCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(244, 244, 244));
                                    UserRoleCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    UserRoleCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    UserRoleCell.Style.Indent = 1;
                                    UserRoleCell.Style.WrapText = true;

                                    var UserNameCell = ws.Cells[MRowNo, ValueColNumber + 1];
                                    UserNameCell.Value = memb["Name"];

                                    UserNameCell.Style.Font.Name = "Calibri";
                                    UserNameCell.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(64, 64, 64));
                                    UserNameCell.Style.Font.Size = 10;

                                    UserNameCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    UserNameCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    UserNameCell.Style.Indent = 1;
                                    UserNameCell.Style.WrapText = true;

                                    MRowNo = MRowNo + 1;

                                }


                                while (MRowNo <= StructuralRowNo + MaxRowHeight)
                                {
                                    var MemberBlankCell = ws.Cells[MRowNo, ValueColNumber];
                                    MemberBlankCell.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    MemberBlankCell.Style.Border.Left.Color.SetColor(System.Drawing.Color.FromArgb(217, 217, 217));

                                    MemberBlankCell.Style.Font.Name = "Calibri";
                                    MemberBlankCell.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(166, 166, 166));
                                    MemberBlankCell.Style.Font.Size = 10;

                                    MemberBlankCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    MemberBlankCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(244, 244, 244));
                                    MemberBlankCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    MemberBlankCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    MemberBlankCell.Style.Indent = 1;
                                    MemberBlankCell.Style.WrapText = true;

                                    MRowNo = MRowNo + 1;

                                }
                                //Draw the Member for the Entity End
                                ValueColNumber += 1;
                            }

                        }

                        if (!IsMemberIncluded & IsTaskIncluded)
                        {
                            ValueColNumber += 2;
                        }
                        else if (!IsMemberIncluded & !IsTaskIncluded)
                        {
                            ValueColNumber = ValueColNumber;
                        }
                        else if (!IsTaskIncluded & IsMemberIncluded)
                        {
                            ValueColNumber = ValueColNumber;
                            EntityRowNo = EntityRowNo + 2;

                        }
                        else
                        {
                            ValueColNumber += 1;
                        }

                        if (IsTaskIncluded)
                        {
                            if (item.TaskListCollections != null)
                            {
                                //Draw the Workflow details for the Entity Start
                                int WRowNo = EntityRowNo;

                                for (int i = 0; i < item.TaskListCollections.Count; i++)
                                {
                                    var TaskListCell = ws.Cells[WRowNo, ValueColNumber, WRowNo, ValueColNumber + 3];
                                    TaskListCell.Merge = true;

                                    TaskListCell.Value = "(" + (i + 1) + ") " + item.TaskListCollections[i]["Name"];



                                    TaskListCell.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    TaskListCell.Style.Border.Left.Color.SetColor(System.Drawing.Color.FromArgb(217, 217, 217));

                                    TaskListCell.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    TaskListCell.Style.Border.Right.Color.SetColor(System.Drawing.Color.FromArgb(217, 217, 217));

                                    TaskListCell.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    TaskListCell.Style.Border.Top.Color.SetColor(System.Drawing.Color.FromArgb(217, 217, 217));

                                    TaskListCell.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    TaskListCell.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.FromArgb(217, 217, 217));

                                    TaskListCell.Style.Font.Name = "Calibri";
                                    TaskListCell.Style.Font.Size = 10;
                                    TaskListCell.Style.Font.Bold = true;

                                    TaskListCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    TaskListCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(244, 244, 244));
                                    TaskListCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    TaskListCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    TaskListCell.Style.Indent = 1;
                                    TaskListCell.Style.WrapText = true;
                                    WRowNo = WRowNo + 1;

                                    var ListID = item.TaskListCollections[i]["TaskListID"];
                                    if (item.TaskCollections != null)
                                    {
                                        foreach (Hashtable Task in item.TaskCollections)
                                        {

                                            if (Convert.ToInt32(ListID) == Convert.ToInt32(Task["TaskListID"]))
                                            {


                                                var TaskNameCell = ws.Cells[WRowNo, ValueColNumber];
                                                //TaskNameCell.Merge = true;


                                                TaskNameCell.Value = Task["TaskName"];

                                                TaskNameCell.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                                TaskNameCell.Style.Border.Left.Color.SetColor(
                                                    System.Drawing.Color.FromArgb(217, 217, 217));

                                                TaskNameCell.Style.Font.Name = "Calibri";
                                                //TaskNameCell.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(
                                                //    128,
                                                //    128, 128));
                                                TaskNameCell.Style.Font.Size = 10;

                                                TaskNameCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                                TaskNameCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                                TaskNameCell.Style.Indent = 1;
                                                TaskNameCell.Style.WrapText = false;

                                                TaskNameCell.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                                TaskNameCell.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.FromArgb(217, 217, 217));

                                                var TaskAssigneeCell = ws.Cells[WRowNo, ValueColNumber + 1];
                                                //TaskAssigneeCell.Merge = true;


                                                TaskAssigneeCell.Value = Task["UserName"];

                                                TaskAssigneeCell.Style.Font.Name = "Calibri";
                                                //TaskAssigneeCell.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb
                                                //    (
                                                //        128, 128, 128));
                                                TaskAssigneeCell.Style.Font.Size = 10;

                                                TaskAssigneeCell.Style.HorizontalAlignment =
                                                    ExcelHorizontalAlignment.Left;
                                                TaskAssigneeCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                                TaskAssigneeCell.Style.Indent = 1;
                                                TaskAssigneeCell.Style.WrapText = false;

                                                TaskAssigneeCell.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                                TaskAssigneeCell.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.FromArgb(217, 217, 217));

                                                var TaskDueDateCell = ws.Cells[WRowNo, ValueColNumber + 2];
                                                //TaskDueDateCell.Merge = true;


                                                TaskDueDateCell.Value = Task["DueDate"];

                                                TaskDueDateCell.Style.Font.Name = "Calibri";
                                                //TaskDueDateCell.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(
                                                //    128, 128, 128));
                                                TaskDueDateCell.Style.Font.Size = 10;

                                                TaskDueDateCell.Style.HorizontalAlignment =
                                                    ExcelHorizontalAlignment.Left;
                                                TaskDueDateCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                                TaskDueDateCell.Style.Indent = 1;
                                                TaskDueDateCell.Style.WrapText = true;

                                                TaskDueDateCell.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                                TaskDueDateCell.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.FromArgb(217, 217, 217));

                                                var TaskStatusCell = ws.Cells[WRowNo, ValueColNumber + 3];
                                                TaskStatusCell.Merge = true;


                                                TaskStatusCell.Value = Task["Status"];

                                                TaskStatusCell.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                                TaskStatusCell.Style.Border.Right.Color.SetColor(
                                                    System.Drawing.Color.FromArgb(217, 217, 217));

                                                TaskStatusCell.Style.Font.Name = "Calibri";
                                                TaskStatusCell.Style.Font.Color.SetColor(
                                                    System.Drawing.Color.FromArgb(128, 128, 128));
                                                TaskStatusCell.Style.Font.Size = 10;

                                                TaskStatusCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                                TaskStatusCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                                TaskStatusCell.Style.Indent = 1;
                                                TaskStatusCell.Style.WrapText = true;

                                                TaskStatusCell.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                                TaskStatusCell.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.FromArgb(217, 217, 217));

                                                WRowNo = WRowNo + 1;
                                            }

                                        }
                                    }

                                    var TaskBlankCell = ws.Cells[WRowNo, ValueColNumber, WRowNo, ValueColNumber + 3];
                                    TaskBlankCell.Merge = true;
                                    TaskBlankCell.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    TaskBlankCell.Style.Border.Left.Color.SetColor(System.Drawing.Color.FromArgb(217,
                                        217,
                                        217));

                                    TaskBlankCell.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    TaskBlankCell.Style.Border.Right.Color.SetColor(System.Drawing.Color.FromArgb(217,
                                        217,
                                        217));

                                    TaskBlankCell.Style.Font.Name = "Calibri";
                                    TaskBlankCell.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(128, 128, 128));
                                    TaskBlankCell.Style.Font.Size = 10;
                                    TaskBlankCell.Style.WrapText = true;
                                    WRowNo = WRowNo + 1;

                                }


                                while (WRowNo <= StructuralRowNo + MaxRowHeight)
                                {
                                    var TaskListBlankCell = ws.Cells[WRowNo, ValueColNumber, WRowNo, ValueColNumber + 3];
                                    TaskListBlankCell.Merge = true;
                                    TaskListBlankCell.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    TaskListBlankCell.Style.Border.Left.Color.SetColor(System.Drawing.Color.FromArgb(
                                        217,
                                        217, 217));

                                    TaskListBlankCell.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    TaskListBlankCell.Style.Border.Right.Color.SetColor(
                                        System.Drawing.Color.FromArgb(217,
                                            217, 217));

                                    TaskListBlankCell.Style.Font.Name = "Calibri";
                                    TaskListBlankCell.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(166, 166,
                                        166));
                                    TaskListBlankCell.Style.Font.Size = 10;
                                    TaskListBlankCell.Style.WrapText = true;


                                    WRowNo = WRowNo + 1;

                                }
                                //Draw the Workflow details for the Entity End
                                ValueColNumber = ValueColNumber + 3;
                            }
                            else
                            {
                                int WRowNo = EntityRowNo;
                                while (WRowNo <= StructuralRowNo + MaxRowHeight)
                                {
                                    var TaskListBlankCell = ws.Cells[WRowNo, ValueColNumber, WRowNo, ValueColNumber + 3];
                                    TaskListBlankCell.Merge = true;
                                    TaskListBlankCell.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    TaskListBlankCell.Style.Border.Left.Color.SetColor(System.Drawing.Color.FromArgb(
                                        217,
                                        217, 217));

                                    TaskListBlankCell.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    TaskListBlankCell.Style.Border.Right.Color.SetColor(
                                        System.Drawing.Color.FromArgb(217,
                                            217, 217));

                                    TaskListBlankCell.Style.Font.Name = "Calibri";
                                    TaskListBlankCell.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(166, 166,
                                        166));
                                    TaskListBlankCell.Style.Font.Size = 10;
                                    TaskListBlankCell.Style.WrapText = true;


                                    WRowNo = WRowNo + 1;

                                }
                                ValueColNumber = ValueColNumber + 3;
                            }
                        }


                        if (IsFinancialIncluded)
                        {

                            ValueColNumber += 1;
                            //Draw the Financial for the Entity Start
                            int FRowNo = EntityRowNo;
                            var CCHeaderCell = ws.Cells[FRowNo, ValueColNumber];
                            CCHeaderCell.Value = "Funding Cost Centres";

                            CCHeaderCell.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            CCHeaderCell.Style.Border.Left.Color.SetColor(System.Drawing.Color.FromArgb(217, 217,
                                217));

                            CCHeaderCell.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            CCHeaderCell.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.FromArgb(217, 217,
                                217));

                            CCHeaderCell.Style.Font.Name = "Calibri";
                            CCHeaderCell.Style.Font.Size = 10;
                            CCHeaderCell.Style.Font.Bold = true;

                            CCHeaderCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            CCHeaderCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(244, 244,
                                244));
                            CCHeaderCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            CCHeaderCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            CCHeaderCell.Style.Indent = 1;
                            CCHeaderCell.Style.WrapText = true;

                            var PlannedHeaderCell = ws.Cells[FRowNo, ValueColNumber + 1];
                            PlannedHeaderCell.Value = "Planned";

                            PlannedHeaderCell.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            PlannedHeaderCell.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.FromArgb(217,
                                217, 217));

                            PlannedHeaderCell.Style.Font.Name = "Calibri";
                            PlannedHeaderCell.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(166, 166, 166));
                            PlannedHeaderCell.Style.Font.Size = 10;

                            PlannedHeaderCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            PlannedHeaderCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(
                                244, 244, 244));
                            PlannedHeaderCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            PlannedHeaderCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            PlannedHeaderCell.Style.Indent = 1;
                            PlannedHeaderCell.Style.WrapText = true;

                            var RequestsHeaderCell = ws.Cells[FRowNo, ValueColNumber + 2];
                            RequestsHeaderCell.Value = "In Requests";

                            RequestsHeaderCell.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            RequestsHeaderCell.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.FromArgb(
                                217, 217, 217));

                            RequestsHeaderCell.Style.Font.Name = "Calibri";
                            RequestsHeaderCell.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(166, 166, 166));
                            RequestsHeaderCell.Style.Font.Size = 10;

                            RequestsHeaderCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            RequestsHeaderCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(
                                244, 244, 244));
                            RequestsHeaderCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            RequestsHeaderCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            RequestsHeaderCell.Style.Indent = 1;
                            RequestsHeaderCell.Style.WrapText = true;

                            var ApprAllocHeaderCell = ws.Cells[FRowNo, ValueColNumber + 3];
                            ApprAllocHeaderCell.Value = "Approved/Allocated";

                            ApprAllocHeaderCell.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            ApprAllocHeaderCell.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.FromArgb(
                                217, 217, 217));

                            ApprAllocHeaderCell.Style.Font.Name = "Calibri";
                            ApprAllocHeaderCell.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(166, 166,
                                166));
                            ApprAllocHeaderCell.Style.Font.Size = 10;

                            ApprAllocHeaderCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            ApprAllocHeaderCell.Style.Fill.BackgroundColor.SetColor(
                                System.Drawing.Color.FromArgb(244, 244, 244));
                            ApprAllocHeaderCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            ApprAllocHeaderCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ApprAllocHeaderCell.Style.Indent = 1;
                            ApprAllocHeaderCell.Style.WrapText = true;

                            var SubAllocHeaderCell = ws.Cells[FRowNo, ValueColNumber + 4];
                            SubAllocHeaderCell.Value = "Sub-Allocated";

                            SubAllocHeaderCell.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            SubAllocHeaderCell.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.FromArgb(
                                217, 217, 217));

                            SubAllocHeaderCell.Style.Font.Name = "Calibri";
                            SubAllocHeaderCell.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(166, 166, 166));
                            SubAllocHeaderCell.Style.Font.Size = 10;

                            SubAllocHeaderCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            SubAllocHeaderCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(
                                244, 244, 244));
                            SubAllocHeaderCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            SubAllocHeaderCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            SubAllocHeaderCell.Style.Indent = 1;
                            SubAllocHeaderCell.Style.WrapText = true;

                            var CommitedHeaderCell = ws.Cells[FRowNo, ValueColNumber + 5];
                            CommitedHeaderCell.Value = "Commited";

                            CommitedHeaderCell.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            CommitedHeaderCell.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.FromArgb(
                                217, 217, 217));

                            CommitedHeaderCell.Style.Font.Name = "Calibri";
                            CommitedHeaderCell.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(166, 166, 166));
                            CommitedHeaderCell.Style.Font.Size = 10;

                            CommitedHeaderCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            CommitedHeaderCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(
                                244, 244, 244));
                            CommitedHeaderCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            CommitedHeaderCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            CommitedHeaderCell.Style.Indent = 1;
                            CommitedHeaderCell.Style.WrapText = true;

                            var SpentHeaderCell = ws.Cells[FRowNo, ValueColNumber + 6];
                            SpentHeaderCell.Value = "Spent";

                            SpentHeaderCell.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            SpentHeaderCell.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.FromArgb(217,
                                217, 217));

                            SpentHeaderCell.Style.Font.Name = "Calibri";
                            SpentHeaderCell.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(166, 166, 166));
                            SpentHeaderCell.Style.Font.Size = 10;

                            SpentHeaderCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            SpentHeaderCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(244,
                                244, 244));
                            SpentHeaderCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            SpentHeaderCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            SpentHeaderCell.Style.Indent = 1;
                            SpentHeaderCell.Style.WrapText = true;

                            var AvailableHeaderCell = ws.Cells[FRowNo, ValueColNumber + 7];
                            AvailableHeaderCell.Value = "Available";

                            AvailableHeaderCell.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            AvailableHeaderCell.Style.Border.Right.Color.SetColor(System.Drawing.Color.FromArgb(
                                217, 217, 217));

                            AvailableHeaderCell.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            AvailableHeaderCell.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.FromArgb(
                                217, 217, 217));

                            AvailableHeaderCell.Style.Font.Name = "Calibri";
                            AvailableHeaderCell.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(166, 166,
                                166));
                            AvailableHeaderCell.Style.Font.Size = 10;

                            AvailableHeaderCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            AvailableHeaderCell.Style.Fill.BackgroundColor.SetColor(
                                System.Drawing.Color.FromArgb(244, 244, 244));
                            AvailableHeaderCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            AvailableHeaderCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            AvailableHeaderCell.Style.Indent = 1;
                            AvailableHeaderCell.Style.WrapText = true;


                            FRowNo = FRowNo + 1;
                            if (item.FinancialCollections != null)
                            {
                               

                                int FinStart = FRowNo;

                                foreach (Hashtable fin in item.FinancialCollections)
                                {


                                    var CCValueCell = ws.Cells[FRowNo, ValueColNumber];
                                    CCValueCell.Value = fin["NAME"];

                                    CCValueCell.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    CCValueCell.Style.Border.Left.Color.SetColor(System.Drawing.Color.FromArgb(217, 217,
                                        217));

                                    CCValueCell.Style.Font.Name = "Calibri";
                                    CCValueCell.Style.Font.Size = 10;

                                    CCValueCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    CCValueCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                                    CCValueCell.Style.Indent = 1;
                                    CCValueCell.Style.WrapText = false;



                                    var PlannedValueCell = ws.Cells[FRowNo, ValueColNumber + 1];
                                    PlannedValueCell.Value = Convert.ToInt32(fin["Planned"]);
                                    // Int32.Parse(fin["Planned"].ToString());

                                    PlannedValueCell.Style.Font.Name = "Calibri";
                                    PlannedValueCell.Style.Font.Size = 10;

                                    PlannedValueCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    PlannedValueCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    PlannedValueCell.Style.Numberformat.Format = "### ### ##0 [$" + CurrencyFormat + "]";

                                    PlannedValueCell.Style.Indent = 1;
                                    PlannedValueCell.Style.WrapText = true;


                                    var RequestsValueCell = ws.Cells[FRowNo, ValueColNumber + 2];
                                    RequestsValueCell.Value = Convert.ToInt32(fin["Requested"]);
                                    //Int32.Parse(fin["Requested"].ToString());

                                    RequestsValueCell.Style.Font.Name = "Calibri";
                                    RequestsValueCell.Style.Font.Size = 10;

                                    RequestsValueCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    RequestsValueCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    RequestsValueCell.Style.Numberformat.Format =  "### ### ##0 [$" + CurrencyFormat + "]";

                                    RequestsValueCell.Style.Indent = 1;
                                    RequestsValueCell.Style.WrapText = true;

                                    var ApprAllocValueCell = ws.Cells[FRowNo, ValueColNumber + 3];
                                    ApprAllocValueCell.Value = Convert.ToInt32(fin["ApprovedAllocation"]);
                                    //Int32.Parse(fin["ApprovedAllocation"].ToString());

                                    ApprAllocValueCell.Style.Font.Name = "Calibri";
                                    ApprAllocValueCell.Style.Font.Size = 10;

                                    ApprAllocValueCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    ApprAllocValueCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    ApprAllocValueCell.Style.Numberformat.Format =  "### ### ##0 [$" + CurrencyFormat + "]";

                                    ApprAllocValueCell.Style.Indent = 1;
                                    ApprAllocValueCell.Style.WrapText = true;

                                    var SubAllocValueCell = ws.Cells[FRowNo, ValueColNumber + 4];
                                    SubAllocValueCell.Value = 0;
                                    //Int32.Parse(fin["Planned"].ToString());fin["sub allocation"];

                                    SubAllocValueCell.Style.Font.Name = "Calibri";
                                    SubAllocValueCell.Style.Font.Size = 10;

                                    SubAllocValueCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    SubAllocValueCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    SubAllocValueCell.Style.Numberformat.Format =  "### ### ##0 [$" + CurrencyFormat + "]";

                                    SubAllocValueCell.Style.Indent = 1;
                                    SubAllocValueCell.Style.WrapText = true;

                                    var CommitedValueCell = ws.Cells[FRowNo, ValueColNumber + 5];
                                    CommitedValueCell.Value = Convert.ToInt32(fin["Committed"]);
                                    //Int32.Parse(fin["Committed"].ToString());

                                    CommitedValueCell.Style.Font.Name = "Calibri";
                                    CommitedValueCell.Style.Font.Size = 10;

                                    CommitedValueCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    CommitedValueCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    CommitedValueCell.Style.Numberformat.Format =  "### ### ##0 [$" + CurrencyFormat + "]";

                                    CommitedValueCell.Style.Indent = 1;
                                    CommitedValueCell.Style.WrapText = true;

                                    var SpentValueCell = ws.Cells[FRowNo, ValueColNumber + 6];
                                    SpentValueCell.Value = Convert.ToInt32(fin["Spent"]);
                                    //Int32.Parse(fin["Spent"].ToString());

                                    SpentValueCell.Style.Font.Name = "Calibri";
                                    SpentValueCell.Style.Font.Size = 10;

                                    SpentValueCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    SpentValueCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    SpentValueCell.Style.Numberformat.Format =  "### ### ##0 [$" + CurrencyFormat + "]";

                                    SpentValueCell.Style.Indent = 1;
                                    SpentValueCell.Style.WrapText = true;

                                    var AvailableValueCell = ws.Cells[FRowNo, ValueColNumber + 7];
                                    AvailableValueCell.Value = Convert.ToInt32(fin["AvailableToSpend"]);
                                    //Int32.Parse(fin["AvailableToSpend"].ToString());

                                    AvailableValueCell.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    AvailableValueCell.Style.Border.Right.Color.SetColor(
                                        System.Drawing.Color.FromArgb(217, 217, 217));

                                    AvailableValueCell.Style.Font.Name = "Calibri";
                                    AvailableValueCell.Style.Font.Size = 10;

                                    AvailableValueCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    AvailableValueCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    AvailableValueCell.Style.Numberformat.Format =  "### ### ##0 [$" + CurrencyFormat + "]";

                                    AvailableValueCell.Style.Indent = 1;
                                    AvailableValueCell.Style.WrapText = true;
                                    FRowNo = FRowNo + 1;

                                }




                                var CCSummaryCell = ws.Cells[FRowNo, ValueColNumber];
                                CCSummaryCell.Value = "Summary";

                                CCSummaryCell.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                CCSummaryCell.Style.Border.Left.Color.SetColor(System.Drawing.Color.FromArgb(217, 217,
                                    217));

                                CCSummaryCell.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                CCSummaryCell.Style.Border.Top.Color.SetColor(System.Drawing.Color.FromArgb(217, 217,
                                    217));

                                CCSummaryCell.Style.Font.Name = "Calibri";
                                CCSummaryCell.Style.Font.Size = 10;
                                CCSummaryCell.Style.Font.Bold = true;

                                CCSummaryCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                CCSummaryCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                CCSummaryCell.Style.Indent = 1;
                                CCSummaryCell.Style.WrapText = true;

                                var PlannedSummaryCell = ws.Cells[FRowNo, ValueColNumber + 1];

                                if (FinStart == FRowNo - 1)
                                {
                                    PlannedSummaryCell.Formula = ws.Cells[FinStart, ValueColNumber + 1].Address;
                                }
                                else
                                {
                                    PlannedSummaryCell.Formula = "Sum(" + ws.Cells[FinStart, ValueColNumber + 1].Address +
                                                                 "," + ws.Cells[FRowNo - 1, ValueColNumber + 1].Address +
                                                                 ")";
                                }


                                PlannedSummaryCell.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                PlannedSummaryCell.Style.Border.Top.Color.SetColor(System.Drawing.Color.FromArgb(217,
                                    217, 217));

                                PlannedSummaryCell.Style.Font.Name = "Calibri";
                                PlannedSummaryCell.Style.Font.Size = 10;
                                PlannedSummaryCell.Style.Font.Bold = true;

                                PlannedSummaryCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                PlannedSummaryCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                PlannedSummaryCell.Style.Numberformat.Format =  "### ### ##0 [$" + CurrencyFormat + "]";

                                PlannedSummaryCell.Style.Indent = 1;
                                PlannedSummaryCell.Style.WrapText = true;

                                var RequestsSummaryCell = ws.Cells[FRowNo, ValueColNumber + 2];

                                if (FinStart == FRowNo - 1)
                                {
                                    RequestsSummaryCell.Formula = ws.Cells[FinStart, ValueColNumber + 2].Address;
                                }
                                else
                                {
                                    RequestsSummaryCell.Formula = "Sum(" +
                                                                  ws.Cells[FinStart, ValueColNumber + 2].Address + "," +
                                                                  ws.Cells[FRowNo - 1, ValueColNumber + 2].Address + ")";
                                }


                                RequestsSummaryCell.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                RequestsSummaryCell.Style.Border.Top.Color.SetColor(System.Drawing.Color.FromArgb(217,
                                    217, 217));

                                RequestsSummaryCell.Style.Font.Name = "Calibri";
                                RequestsSummaryCell.Style.Font.Size = 10;
                                RequestsSummaryCell.Style.Font.Bold = true;

                                RequestsSummaryCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                RequestsSummaryCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                RequestsSummaryCell.Style.Numberformat.Format =  "### ### ##0 [$" + CurrencyFormat + "]";

                                RequestsSummaryCell.Style.Indent = 1;
                                RequestsSummaryCell.Style.WrapText = true;

                                var ApprAllocSummaryCell = ws.Cells[FRowNo, ValueColNumber + 3];
                                if (FinStart == FRowNo - 1)
                                {
                                    ApprAllocSummaryCell.Formula = ws.Cells[FinStart, ValueColNumber + 3].Address;
                                }
                                else
                                {
                                    ApprAllocSummaryCell.Formula = "Sum(" +
                                                                   ws.Cells[FinStart, ValueColNumber + 3].Address + "," +
                                                                   ws.Cells[FRowNo - 1, ValueColNumber + 3].Address +
                                                                   ")";
                                }


                                ApprAllocSummaryCell.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                ApprAllocSummaryCell.Style.Border.Top.Color.SetColor(System.Drawing.Color.FromArgb(217,
                                    217, 217));

                                ApprAllocSummaryCell.Style.Font.Name = "Calibri";
                                ApprAllocSummaryCell.Style.Font.Size = 10;
                                ApprAllocSummaryCell.Style.Font.Bold = true;

                                ApprAllocSummaryCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                ApprAllocSummaryCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                ApprAllocSummaryCell.Style.Numberformat.Format =  "### ### ##0 [$" + CurrencyFormat + "]";

                                ApprAllocSummaryCell.Style.Indent = 1;
                                ApprAllocSummaryCell.Style.WrapText = true;

                                var SubAllocSummaryCell = ws.Cells[FRowNo, ValueColNumber + 4];
                                if (FinStart == FRowNo - 1)
                                {
                                    SubAllocSummaryCell.Formula = ws.Cells[FinStart, ValueColNumber + 4].Address;
                                }
                                else
                                {
                                    SubAllocSummaryCell.Formula = "Sum(" +
                                                                  ws.Cells[FinStart, ValueColNumber + 4].Address + "," +
                                                                  ws.Cells[FRowNo - 1, ValueColNumber + 4].Address + ")";
                                }


                                SubAllocSummaryCell.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                SubAllocSummaryCell.Style.Border.Top.Color.SetColor(System.Drawing.Color.FromArgb(217,
                                    217, 217));

                                SubAllocSummaryCell.Style.Font.Name = "Calibri";
                                SubAllocSummaryCell.Style.Font.Size = 10;
                                SubAllocSummaryCell.Style.Font.Bold = true;

                                SubAllocSummaryCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                SubAllocSummaryCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                SubAllocSummaryCell.Style.Numberformat.Format =  "### ### ##0 [$" + CurrencyFormat + "]";

                                SubAllocSummaryCell.Style.Indent = 1;
                                SubAllocSummaryCell.Style.WrapText = true;

                                var CommitedSummaryCell = ws.Cells[FRowNo, ValueColNumber + 5];
                                if (FinStart == FRowNo - 1)
                                {
                                    CommitedSummaryCell.Formula = ws.Cells[FinStart, ValueColNumber + 5].Address;
                                }
                                else
                                {
                                    CommitedSummaryCell.Formula = "Sum(" +
                                                                  ws.Cells[FinStart, ValueColNumber + 5].Address + "," +
                                                                  ws.Cells[FRowNo - 1, ValueColNumber + 5].Address + ")";
                                }


                                CommitedSummaryCell.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                CommitedSummaryCell.Style.Border.Top.Color.SetColor(System.Drawing.Color.FromArgb(217,
                                    217, 217));

                                CommitedSummaryCell.Style.Font.Name = "Calibri";
                                CommitedSummaryCell.Style.Font.Size = 10;
                                CommitedSummaryCell.Style.Font.Bold = true;

                                CommitedSummaryCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                CommitedSummaryCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                CommitedSummaryCell.Style.Numberformat.Format =  "### ### ##0 [$" + CurrencyFormat + "]";

                                CommitedSummaryCell.Style.Indent = 1;
                                CommitedSummaryCell.Style.WrapText = true;

                                var SpentSummaryCell = ws.Cells[FRowNo, ValueColNumber + 6];
                                if (FinStart == FRowNo - 1)
                                {
                                    SpentSummaryCell.Formula = ws.Cells[FinStart, ValueColNumber + 6].Address;
                                }
                                else
                                {
                                    SpentSummaryCell.Formula = "Sum(" + ws.Cells[FinStart, ValueColNumber + 6].Address +
                                                               "," + ws.Cells[FRowNo - 1, ValueColNumber + 6].Address +
                                                               ")";
                                }



                                SpentSummaryCell.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                SpentSummaryCell.Style.Border.Top.Color.SetColor(System.Drawing.Color.FromArgb(217, 217,
                                    217));

                                SpentSummaryCell.Style.Font.Name = "Calibri";
                                SpentSummaryCell.Style.Font.Size = 10;
                                SpentSummaryCell.Style.Font.Bold = true;

                                SpentSummaryCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                SpentSummaryCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                SpentSummaryCell.Style.Numberformat.Format =  "### ### ##0 [$" + CurrencyFormat + "]";

                                SpentSummaryCell.Style.Indent = 1;
                                SpentSummaryCell.Style.WrapText = true;

                                var AvailableSummaryCell = ws.Cells[FRowNo, ValueColNumber + 7];
                                if (FinStart == FRowNo - 1)
                                {
                                    AvailableSummaryCell.Formula = ws.Cells[FinStart, ValueColNumber + 7].Address;
                                }
                                else
                                {
                                    AvailableSummaryCell.Formula = "Sum(" +
                                                                   ws.Cells[FinStart, ValueColNumber + 7].Address + "," +
                                                                   ws.Cells[FRowNo - 1, ValueColNumber + 7].Address +
                                                                   ")";
                                }



                                AvailableSummaryCell.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                AvailableSummaryCell.Style.Border.Right.Color.SetColor(System.Drawing.Color.FromArgb(
                                    217, 217, 217));

                                AvailableSummaryCell.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                AvailableSummaryCell.Style.Border.Top.Color.SetColor(System.Drawing.Color.FromArgb(217,
                                    217, 217));

                                AvailableSummaryCell.Style.Font.Name = "Calibri";
                                AvailableSummaryCell.Style.Font.Size = 10;
                                AvailableSummaryCell.Style.Font.Bold = true;

                                AvailableSummaryCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                AvailableSummaryCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                AvailableSummaryCell.Style.Numberformat.Format =  "### ### ##0 [$" + CurrencyFormat + "]";

                                AvailableSummaryCell.Style.Indent = 1;
                                AvailableSummaryCell.Style.WrapText = true;
                                FRowNo = FRowNo + 1;



                                while (FRowNo <= StructuralRowNo + MaxRowHeight)
                                {
                                    var FinancialBlankCell =
                                        ws.Cells[FRowNo, ValueColNumber, FRowNo, ValueColNumber + 7];
                                    FinancialBlankCell.Merge = true;
                                    FinancialBlankCell.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    FinancialBlankCell.Style.Border.Left.Color.SetColor(
                                        System.Drawing.Color.FromArgb(217, 217, 217));

                                    FinancialBlankCell.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    FinancialBlankCell.Style.Border.Right.Color.SetColor(
                                        System.Drawing.Color.FromArgb(217, 217, 217));

                                    FinancialBlankCell.Style.Font.Name = "Calibri";
                                    FinancialBlankCell.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(166, 166,
                                        166));
                                    FinancialBlankCell.Style.Font.Size = 10;
                                    FinancialBlankCell.Style.WrapText = true;

                                    FRowNo = FRowNo + 1;

                                }

                                //Draw the Financial for the Entity End
                            }
                            else
                            {

                                while (FRowNo <= StructuralRowNo + MaxRowHeight)
                                {
                                    var FinancialBlankCell =
                                        ws.Cells[FRowNo, ValueColNumber, FRowNo, ValueColNumber + 7];
                                    FinancialBlankCell.Merge = true;
                                    FinancialBlankCell.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    FinancialBlankCell.Style.Border.Left.Color.SetColor(
                                        System.Drawing.Color.FromArgb(217, 217, 217));

                                    FinancialBlankCell.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    FinancialBlankCell.Style.Border.Right.Color.SetColor(
                                        System.Drawing.Color.FromArgb(217, 217, 217));

                                    FinancialBlankCell.Style.Font.Name = "Calibri";
                                    FinancialBlankCell.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(166, 166,
                                        166));
                                    FinancialBlankCell.Style.Font.Size = 10;
                                    FinancialBlankCell.Style.WrapText = true;

                                    FRowNo = FRowNo + 1;

                                }


                            }
                        }




                        StructuralRowNo = StructuralRowNo + MaxRowHeight + 1;




                    }
                }

            }
            catch (Exception ex)
            {
            }





        }

        public string GetReportJSONData(ReportManagerProxy proxy, int reportId)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {

                    string mappingfilesPath = AppDomain.CurrentDomain.BaseDirectory;
                    mappingfilesPath = mappingfilesPath + "Reports" + @"\ReportSettings_" + reportId + ".xml";
                    System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                    doc.Load(mappingfilesPath);
                    string sbJSON = string.Empty;
                    sbJSON = tx.PersistenceManager.ReportRepository.XmlToJSON(doc);
                    tx.Commit();
                    return sbJSON;
                }

            }
            catch
            {
                throw null;
            }
        }



        /// <summary>
        /// Gets the entitytype relation.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="version">The version.</param>
        /// <returns>List of IEntityTypeAttributeRelationWithLevels</returns>
        public IList<IEntityTypeAttributeRelationwithLevels> GetEntityTypeAttributeRelationWithLevelsByID(ReportManagerProxy proxy, string ids)
        {

            try
            {
                List<int> idarr = ids.Split(',').Select(int.Parse).ToList();
                int version = MarcomManagerFactory.ActiveMetadataVersionNumber;
                string attributeCaption = string.Empty;
                string entitytypeCaption = string.Empty;
                Boolean isSpecial = false;
                string xmlpath = string.Empty;
                IList<IEntityTypeAttributeRelationwithLevels> _iientitytyperelation = new List<IEntityTypeAttributeRelationwithLevels>();
                IList<EntityTypeAttributeRelationDao> dao = new List<EntityTypeAttributeRelationDao>();
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    xmlpath = tx.PersistenceManager.MetadataRepository.GetXmlPath(version);
                    dao = tx.PersistenceManager.MetadataRepository.GetObject<EntityTypeAttributeRelationDao>(xmlpath);
                    int[] notallowedAttributes = { (int)SystemDefinedAttributes.MyRoleGlobalAccess, (int)SystemDefinedAttributes.MyRoleEntityAccess };
                    var entityttyperesult = dao.Where(a => idarr.Contains(a.EntityTypeID) && !notallowedAttributes.Contains(a.AttributeID)).OrderBy(x => x.SortOrder);
                    var attrIDs = entityttyperesult.Select(a => a.AttributeID).ToList();
                    IList<IAttributeData> entityAttrVal = new List<IAttributeData>();
                    var xDoc = XDocument.Load(xmlpath);
                    foreach (var item in entityttyperesult)
                    {
                        var duplicateattribute = _iientitytyperelation.Where(a => a.AttributeID == item.AttributeID).ToList();
                        if (duplicateattribute.Count == 0)
                        {

                            IEntityTypeAttributeRelationwithLevels _ientitytyperelation = new EntityTypeAttributeRelationwithLevels();
                            _ientitytyperelation.ID = item.ID;
                            _ientitytyperelation.EntityTypeID = item.EntityTypeID;
                            _ientitytyperelation.EntityTypeCaption = HttpUtility.HtmlDecode(Convert.ToString(xDoc.Root.Elements("EntityType_Table").Elements("EntityType").Where(a => Convert.ToInt32(a.Element("ID").Value) == Convert.ToInt32(item.EntityTypeID)).Select(a => a.Element("Caption").Value).First()));
                            _ientitytyperelation.AttributeID = item.AttributeID;
                            isSpecial = Convert.ToBoolean(Convert.ToInt32(xDoc.Root.Elements("Attribute_Table").Elements("Attribute").Where(a => Convert.ToInt32(a.Element("ID").Value) == Convert.ToInt32(item.AttributeID)).Select(a => a.Element("IsSpecial").Value).First()));
                            _ientitytyperelation.IsSpecial = isSpecial;
                            _ientitytyperelation.AttributeCaption = HttpUtility.HtmlDecode(item.Caption);
                            _ientitytyperelation.AttributeTypeID = Convert.ToInt32(xDoc.Root.Elements("Attribute_Table").Elements("Attribute").Where(a => Convert.ToInt32(a.Element("ID").Value) == Convert.ToInt32(item.AttributeID)).Select(a => a.Element("AttributeTypeID").Value).First());
                            _ientitytyperelation.ValidationID = item.ValidationID;
                            _ientitytyperelation.SortOrder = item.SortOrder;
                            _ientitytyperelation.DefaultValue = item.DefaultValue;
                            _ientitytyperelation.InheritFromParent = item.InheritFromParent;
                            _ientitytyperelation.PlaceHolderValue = HttpUtility.HtmlDecode(item.PlaceHolderValue);
                            _ientitytyperelation.IsReadOnly = item.IsReadOnly;
                            _ientitytyperelation.ChooseFromParentOnly = item.ChooseFromParentOnly;
                            _ientitytyperelation.IsValidationNeeded = item.IsValidationNeeded;
                            _ientitytyperelation.Caption = HttpUtility.HtmlDecode(item.Caption);
                            _ientitytyperelation.IsSystemDefined = item.IsSystemDefined;
                            _iientitytyperelation.Add(_ientitytyperelation);
                        }
                    }
                }
                return _iientitytyperelation;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public IList<IEntityTypeAttributeRelationwithLevels> GetAllEntityTypeAttributeRelationWithLevels(ReportManagerProxy proxy)
        {

            try
            {

                int version = MarcomManagerFactory.ActiveMetadataVersionNumber;
                string attributeCaption = string.Empty;
                string entitytypeCaption = string.Empty;
                string xmlpath = string.Empty;
                IList<IEntityTypeAttributeRelationwithLevels> _iientitytyperelation = new List<IEntityTypeAttributeRelationwithLevels>();
                IList<EntityTypeAttributeRelationDao> dao = new List<EntityTypeAttributeRelationDao>();
                IList<EntityTypeDao> entTypedao = new List<EntityTypeDao>();
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    int[] notallowedAttributes = { (int)SystemDefinedAttributes.MyRoleGlobalAccess, (int)SystemDefinedAttributes.MyRoleEntityAccess, (int)SystemDefinedAttributes.Owner };
                    int[] systemDefinedTypes = { (int)EntityTypeList.CostCentre, (int)EntityTypeList.FundinngRequest, (int)EntityTypeList.Milestone, (int)EntityTypeList.Objective, (int)EntityTypeList.Task };
                    xmlpath = tx.PersistenceManager.MetadataRepository.GetXmlPath(version);
                    dao = tx.PersistenceManager.MetadataRepository.GetObject<EntityTypeAttributeRelationDao>(xmlpath);
                    entTypedao = tx.PersistenceManager.MetadataRepository.GetObject<EntityTypeDao>(xmlpath);
                    var entityttyperesult = dao.Where(a => !notallowedAttributes.Contains(a.AttributeID) && !systemDefinedTypes.Contains(a.EntityTypeID)).OrderBy(x => x.SortOrder);
                    var q = (from pd in entityttyperesult
                             join od in entTypedao on pd.EntityTypeID equals od.Id
                             where od.IsAssociate == false && od.Category == 2
                             orderby od.Id
                             select new
                             {
                                 pd.AttributeID,
                                 pd.Caption,
                                 pd.EntityTypeID,
                                 pd.ID,
                                 pd.SortOrder,
                                 pd.PlaceHolderValue,
                                 pd.DefaultValue
                             }).ToList();
                    var attrIDs = q.Select(a => a.AttributeID).ToList();
                    IList<IAttributeData> entityAttrVal = new List<IAttributeData>();
                    var xDoc = XDocument.Load(xmlpath);
                    IEntityTypeAttributeRelationwithLevels _ientitytyperelation = new EntityTypeAttributeRelationwithLevels();
                    foreach (var item in q)
                    {
                        var duplicateattribute = _iientitytyperelation.Where(a => a.AttributeID == item.AttributeID && a.EntityTypeID == item.EntityTypeID).ToList();
                        if (duplicateattribute.Count == 0)
                        {

                            var attibutetype = Convert.ToInt32(xDoc.Root.Elements("Attribute_Table").Elements("Attribute").Where(a => Convert.ToInt32(a.Element("ID").Value) == Convert.ToInt32(item.AttributeID)).Select(a => a.Element("AttributeTypeID").Value).First());
                            if (attibutetype == (int)AttributesList.Period)
                            {
                                _ientitytyperelation = new EntityTypeAttributeRelationwithLevels();
                                _ientitytyperelation.ID = item.ID;
                                _ientitytyperelation.EntityTypeID = item.EntityTypeID;
                                _ientitytyperelation.EntityTypeCaption = Convert.ToString(xDoc.Root.Elements("EntityType_Table").Elements("EntityType").Where(a => Convert.ToInt32(a.Element("ID").Value) == Convert.ToInt32(item.EntityTypeID)).Select(a => a.Element("Caption").Value).First());
                                _ientitytyperelation.AttributeID = item.AttributeID;
                                _ientitytyperelation.strAttributeID = Convert.ToString(item.AttributeID) + "_1";
                                _ientitytyperelation.AttributeCaption = "Start date";
                                _ientitytyperelation.AttributeTypeID = Convert.ToInt32(xDoc.Root.Elements("Attribute_Table").Elements("Attribute").Where(a => Convert.ToInt32(a.Element("ID").Value) == Convert.ToInt32(item.AttributeID)).Select(a => a.Element("AttributeTypeID").Value).First());
                                _ientitytyperelation.SortOrder = item.SortOrder;
                                _ientitytyperelation.DefaultValue = item.DefaultValue;
                                _ientitytyperelation.Caption = item.Caption;
                                _iientitytyperelation.Add(_ientitytyperelation);

                                _ientitytyperelation = new EntityTypeAttributeRelationwithLevels();
                                _ientitytyperelation.ID = item.ID;
                                _ientitytyperelation.EntityTypeID = item.EntityTypeID;
                                _ientitytyperelation.EntityTypeCaption = Convert.ToString(xDoc.Root.Elements("EntityType_Table").Elements("EntityType").Where(a => Convert.ToInt32(a.Element("ID").Value) == Convert.ToInt32(item.EntityTypeID)).Select(a => a.Element("Caption").Value).First());
                                _ientitytyperelation.AttributeID = item.AttributeID;
                                _ientitytyperelation.strAttributeID = Convert.ToString(item.AttributeID) + "_2";
                                _ientitytyperelation.AttributeCaption = "End date";
                                _ientitytyperelation.AttributeTypeID = Convert.ToInt32(xDoc.Root.Elements("Attribute_Table").Elements("Attribute").Where(a => Convert.ToInt32(a.Element("ID").Value) == Convert.ToInt32(item.AttributeID)).Select(a => a.Element("AttributeTypeID").Value).First());
                                _ientitytyperelation.SortOrder = item.SortOrder;
                                _ientitytyperelation.DefaultValue = item.DefaultValue;
                                _ientitytyperelation.Caption = item.Caption;
                                _iientitytyperelation.Add(_ientitytyperelation);
                            }


                            else if (attibutetype == (int)AttributesList.DropDownTree)
                            {

                                IList<ITreeLevel> treeLevels = proxy.MarcomManager.MetadataManager.GetTreelevel(version);
                                IList<ITreeLevel> temptreeLevels = new List<ITreeLevel>();
                                temptreeLevels = (from level in treeLevels
                                                  where level.AttributeID == item.AttributeID
                                                  select level).ToList<ITreeLevel>();
                                foreach (var levelrec in temptreeLevels)
                                {
                                    _ientitytyperelation = new EntityTypeAttributeRelationwithLevels();
                                    _ientitytyperelation.ID = item.ID;
                                    _ientitytyperelation.EntityTypeID = item.EntityTypeID;
                                    _ientitytyperelation.EntityTypeCaption = Convert.ToString(xDoc.Root.Elements("EntityType_Table").Elements("EntityType").Where(a => Convert.ToInt32(a.Element("ID").Value) == Convert.ToInt32(item.EntityTypeID)).Select(a => a.Element("Caption").Value).First());
                                    _ientitytyperelation.AttributeID = item.AttributeID;
                                    _ientitytyperelation.strAttributeID = Convert.ToString(item.AttributeID) + "_" + Convert.ToString(levelrec.Level);
                                    _ientitytyperelation.AttributeCaption = levelrec.LevelName;
                                    _ientitytyperelation.AttributeTypeID = Convert.ToInt32(xDoc.Root.Elements("Attribute_Table").Elements("Attribute").Where(a => Convert.ToInt32(a.Element("ID").Value) == Convert.ToInt32(item.AttributeID)).Select(a => a.Element("AttributeTypeID").Value).First());
                                    _ientitytyperelation.SortOrder = item.SortOrder;
                                    _ientitytyperelation.DefaultValue = item.DefaultValue;
                                    _ientitytyperelation.Caption = item.Caption;
                                    _iientitytyperelation.Add(_ientitytyperelation);
                                }

                            }


                            else if (attibutetype == (int)AttributesList.TreeMultiSelection)
                            {

                                IList<ITreeLevel> treeLevels = proxy.MarcomManager.MetadataManager.GetTreelevel(version);
                                IList<ITreeLevel> temptreeLevels = new List<ITreeLevel>();
                                temptreeLevels = (from level in treeLevels
                                                  where level.AttributeID == item.AttributeID
                                                  select level).ToList<ITreeLevel>();
                                foreach (var levelrec in temptreeLevels)
                                {
                                    _ientitytyperelation = new EntityTypeAttributeRelationwithLevels();
                                    _ientitytyperelation.ID = item.ID;
                                    _ientitytyperelation.EntityTypeID = item.EntityTypeID;
                                    _ientitytyperelation.EntityTypeCaption = Convert.ToString(xDoc.Root.Elements("EntityType_Table").Elements("EntityType").Where(a => Convert.ToInt32(a.Element("ID").Value) == Convert.ToInt32(item.EntityTypeID)).Select(a => a.Element("Caption").Value).First());
                                    _ientitytyperelation.AttributeID = item.AttributeID;
                                    _ientitytyperelation.strAttributeID = Convert.ToString(item.AttributeID) + "_" + Convert.ToString(levelrec.Level);
                                    _ientitytyperelation.AttributeCaption = levelrec.LevelName;
                                    _ientitytyperelation.AttributeTypeID = Convert.ToInt32(xDoc.Root.Elements("Attribute_Table").Elements("Attribute").Where(a => Convert.ToInt32(a.Element("ID").Value) == Convert.ToInt32(item.AttributeID)).Select(a => a.Element("AttributeTypeID").Value).First());
                                    _ientitytyperelation.SortOrder = item.SortOrder;
                                    _ientitytyperelation.DefaultValue = item.DefaultValue;
                                    _ientitytyperelation.Caption = item.Caption;
                                    _iientitytyperelation.Add(_ientitytyperelation);
                                }

                            }

                            else
                            {
                                _ientitytyperelation = new EntityTypeAttributeRelationwithLevels();
                                _ientitytyperelation.ID = item.ID;
                                _ientitytyperelation.EntityTypeID = item.EntityTypeID;
                                _ientitytyperelation.EntityTypeCaption = Convert.ToString(xDoc.Root.Elements("EntityType_Table").Elements("EntityType").Where(a => Convert.ToInt32(a.Element("ID").Value) == Convert.ToInt32(item.EntityTypeID)).Select(a => a.Element("Caption").Value).First());
                                _ientitytyperelation.AttributeID = item.AttributeID;
                                _ientitytyperelation.strAttributeID = Convert.ToString(item.AttributeID);
                                _ientitytyperelation.AttributeCaption = item.Caption;
                                _ientitytyperelation.AttributeTypeID = Convert.ToInt32(xDoc.Root.Elements("Attribute_Table").Elements("Attribute").Where(a => Convert.ToInt32(a.Element("ID").Value) == Convert.ToInt32(item.AttributeID)).Select(a => a.Element("AttributeTypeID").Value).First());
                                _ientitytyperelation.SortOrder = item.SortOrder;
                                _ientitytyperelation.DefaultValue = item.DefaultValue;
                                _ientitytyperelation.Caption = item.Caption;
                                _iientitytyperelation.Add(_ientitytyperelation);
                            }
                        }
                    }
                }
                return _iientitytyperelation;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool InsertUpdateReportSettingXML(ReportManagerProxy proxy, JObject jsonXML, int reportID)
        {
            string mappingfilesPath = AppDomain.CurrentDomain.BaseDirectory;
            mappingfilesPath = mappingfilesPath + "Reports" + @"\ReportSettings_" + reportID + ".xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(mappingfilesPath);
            // To convert JSON text contained in string json into an XML node
            string strJsonXML = JsonConvert.SerializeObject(jsonXML);
            XmlDocument newdocContent = JsonConvert.DeserializeXmlNode(strJsonXML);

            XmlDocument doc1 = new XmlDocument();
            doc1 = newdocContent;
            doc1.Save(mappingfilesPath);
            return true;
        }

        public bool insertupdatefinancialreportsettings(ReportManagerProxy proxy, string reportsettingname, int reportID, string ReportImage, string description, JObject jsonXML)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    FinancialReportSettingsDao dao = new FinancialReportSettingsDao();
                    if (reportID > 0)
                    {
                        dao = tx.PersistenceManager.ReportRepository.Query<FinancialReportSettingsDao>().Where(a => a.Id == reportID).FirstOrDefault();
                        dao.ReportName = HttpUtility.HtmlEncode(reportsettingname);
                        dao.ReportDescription = HttpUtility.HtmlEncode(description);
                        dao.ReportImage = ReportImage;
                    }
                    else
                    {
                        dao.ReportName = HttpUtility.HtmlEncode(reportsettingname);
                        dao.ReportDescription = HttpUtility.HtmlEncode(description);
                        dao.ReportImage = ReportImage;
                    }
                    tx.PersistenceManager.ReportRepository.Save<FinancialReportSettingsDao>(dao);

                    tx.Commit();

                    string mappingfilesPath = AppDomain.CurrentDomain.BaseDirectory;
                    mappingfilesPath = mappingfilesPath + "Reports" + @"\ReportSettings_" + dao.Id + ".xml";
                    string strJsonXML = JsonConvert.SerializeObject(jsonXML);
                    XmlDocument newdocContent = JsonConvert.DeserializeXmlNode(strJsonXML);

                    XmlDocument doc1 = new XmlDocument();
                    doc1 = newdocContent;
                    doc1.Save(mappingfilesPath);

                    return true;
                }

            }

            catch
            {
                return false;
            }
        }

        public IList GetFinancialReportSettings(ReportManagerProxy proxy)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    string qry = "SELECT * FROM RM_FinancialReportSettings";
                    IList reportsettings = tx.PersistenceManager.TaskRepository.ExecuteQuery(qry);
                    return reportsettings;

                }

            }
            catch
            {
                throw null;
            }
        }



        public Tuple<Guid, string> GenerateFinancialExcel(ReportManagerProxy proxy, int ReportID)
        {
            String ReportXmlInput = AppDomain.CurrentDomain.BaseDirectory + ("/Reports/ReportSettings_") + ReportID + ".xml"; ;
            int StartRowNo = 4;

            //Get session information 
            Guid NewGuid = Guid.NewGuid();

            //Create a xlsx fill
            string fullpath = AppDomain.CurrentDomain.BaseDirectory + ("/Files/ReportFiles/Images/Temp/") + NewGuid + ".xlsx";
            //var fullpath = @"C:\reports\" + Guid.NewGuid() + ".xlsx";
            FileInfo newFile = new FileInfo(fullpath);






            //Get xml settings to draw financial data.
            XElement xs = XElement.Load(ReportXmlInput);

            var reportName = xs.Elements("caption").FirstOrDefault() != null ? xs.Elements("caption").FirstOrDefault().Value : "Financial Report";

            //Read basic information about the chart



            //Create Excel package and information about the company
            ExcelPackage pck = new ExcelPackage(newFile);

            pck.Workbook.Properties.Title = HttpUtility.HtmlEncode(reportName);
            pck.Workbook.Properties.Author = "Marcom Plarform";




            //Create worksheets
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add(reportName);

            ws.View.ShowGridLines = false;

            ws.Row(2).Height = 70;
            for (int i = 3; i < 100; i++)
            {
                ws.Row(i).Height = 22;
            }



            ws.Column(2).Width = 40;

            for (int i = 3; i < 50; i++)
            {
                ws.Column(i).Width = 20;
            }


            DrawName(ws, reportName);

            int previousBlockHeight = 0;

            //Loop through all the block
            foreach (XElement block in xs.Elements("block"))
            {
                //Read block default property
                var blockId = block.Elements("id").FirstOrDefault() != null ? block.Elements("id").FirstOrDefault().Value : "0";
                var blockCaption = block.Elements("caption").FirstOrDefault() != null ? block.Elements("caption").FirstOrDefault().Value : "Block";
                var blockType = block.Elements("Type").FirstOrDefault() != null ? block.Elements("Type").FirstOrDefault().Value : "1";
                var entityType = block.Elements("EntityType").FirstOrDefault() != null ? block.Elements("EntityType").FirstOrDefault().Value : "";
                var attributeId = block.Elements("AttributeID").FirstOrDefault() != null ? block.Elements("AttributeID").FirstOrDefault().Value : "";



                var columns = block.Elements("columns").Elements("column");
                var charts = block.Elements("charts").Elements("chart");



                switch (blockType)
                {
                    case "1":

                        StartRowNo = DrawSummary(StartRowNo, ws, blockCaption, GetFinancialSummaryDetlRpt(proxy, entityType), GetDefaultCurrencyinReports(proxy), columns, charts);

                        StartRowNo = StartRowNo + 1;

                        if (StartRowNo - previousBlockHeight < 7)
                        {
                            StartRowNo = StartRowNo + (StartRowNo - previousBlockHeight);
                        }

                        previousBlockHeight = StartRowNo;

                        break;
                    case "2":

                        StartRowNo = DrawAttributeWise(StartRowNo, ws, blockCaption, GetFinancialSummaryDetlRptByAttribute(proxy, entityType, Convert.ToInt32(attributeId)), GetDefaultCurrencyinReports(proxy), columns, charts);

                        StartRowNo = StartRowNo + 1;

                        if (StartRowNo - previousBlockHeight < 7)
                        {
                            StartRowNo = StartRowNo + (StartRowNo - previousBlockHeight);
                        }

                        previousBlockHeight = StartRowNo;

                        break;
                    case "3":

                        var AttributeIDCollections = columns.Elements("id").ToArray();


                        List<string> AttributeIDs = new List<string>();
                        List<int> FinancialAttributes = new List<int>();

                        foreach (string attribute in AttributeIDCollections)
                        {

                            int Num;

                            bool Status = Int32.TryParse(attribute, out Num);



                            if (Status)
                            {
                                if (Int32.Parse(attribute) > 0)
                                {
                                    AttributeIDs.Add(attribute);
                                }
                                else
                                {
                                    FinancialAttributes.Add(Int32.Parse(attribute));
                                }
                            }
                            else
                            {
                                AttributeIDs.Add(attribute);
                            }



                        }

                        StartRowNo = DrawDetail(StartRowNo, ws, blockCaption, GetEntityFinancialSummaryDetl(proxy, entityType, AttributeIDs, FinancialAttributes), columns);
                        StartRowNo = StartRowNo + 1;

                        if (StartRowNo - previousBlockHeight < 7)
                        {
                            StartRowNo = StartRowNo + (StartRowNo - previousBlockHeight);
                        }

                        previousBlockHeight = StartRowNo;

                        break;


                }

            }

            //Save package


            //Define report friendly name
            pck.Save();


            return Tuple.Create<Guid, string>(NewGuid, reportName);

        }


        private void DrawName(ExcelWorksheet ws, string caption)
        {
            var cel = ws.Cells[2, 2, 2, 26];
            cel.Merge = true;
            cel.Value = HttpUtility.HtmlDecode(caption);

            cel.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            cel.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.FromArgb(128, 128, 128));

            cel.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            cel.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            cel.Style.Font.Name = "Calibri";
            cel.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(51, 51, 51));
            cel.Style.Font.Size = 26;
            cel.Style.Font.Bold = true;


        }
        private int DrawSummary(int StartRowNo, ExcelWorksheet ws, string caption, IList data, string defaultCurrency, IEnumerable<XElement> columns, IEnumerable<XElement> chartdata)
        {

            //Draw table caption
            ws.Cells[StartRowNo, 2, StartRowNo, 2].Value = HttpUtility.HtmlDecode(caption.ToString());
            FormatBlockCaption(ws.Cells[StartRowNo, 2, StartRowNo, 2]);

            ws.Row(StartRowNo).Height = 25;
            StartRowNo = StartRowNo + 1;

            var SumRowStart = StartRowNo;

            //Draw column and value
            for (int i = 0; i < columns.ToList().Count(); i++)
            {
                var Column = columns.ToList()[i].Elements("caption").FirstOrDefault().Value;
                var Value = ((Hashtable)data[0])["col" + columns.ToList()[i].Elements("id").FirstOrDefault().Value];

                ws.Cells[StartRowNo, 2, StartRowNo, 2].Value = HttpUtility.HtmlDecode(Column.ToString());
                FormatCellFill(ws.Cells[StartRowNo, 2, StartRowNo, 2]);
                FormatNormalFont(ws.Cells[StartRowNo, 2, StartRowNo, 2]);

                ws.Cells[StartRowNo, 3, StartRowNo, 3].Value = Convert.ToInt32(Value);
                FormatCell(ws.Cells[StartRowNo, 3, StartRowNo, 3]);
                FormatNormalFontRight(ws.Cells[StartRowNo, 3, StartRowNo, 3]);


                if (columns.ToList()[i].Elements("id").FirstOrDefault().Value != "1")
                {
                    ws.Cells[StartRowNo, 3, StartRowNo, 3].Style.Numberformat.Format = "### ### ##0 [$" + defaultCurrency + "]";
                }

                StartRowNo = StartRowNo + 1;
            }

            //Draw chart
            for (int i = 0; i < chartdata.ToList().Count(); i++)
            {
                var id = chartdata.ToList()[i].Elements("id").FirstOrDefault().Value;
                var type = chartdata.ToList()[i].Elements("chartType").FirstOrDefault().Value;
                var col = chartdata.ToList()[i].Elements("columns").FirstOrDefault().Value;

                eChartType ct;

                switch (type)
                {
                    case "1":
                        ct = eChartType.ColumnStacked;

                        DrawColumnSingle(ws, id, SumRowStart - 1, 3 + (i * 4),
                            ws.Cells[SumRowStart + 1, 3, StartRowNo - 1, 3],
                            ws.Cells[SumRowStart + 1, 2, StartRowNo - 1, 2]);
                        break;


                    case "2":
                        ct = eChartType.BarStacked;
                        DrawBarSingle(ws, id, SumRowStart - 1, 3 + (i * 4),
                           ws.Cells[SumRowStart + 1, 3, StartRowNo - 1, 3],
                           ws.Cells[SumRowStart + 1, 2, StartRowNo - 1, 2]);
                        break;

                    case "3":

                        ct = eChartType.Pie;
                        DrawPieSingle(ws, id, SumRowStart - 1, 3 + (i * 4),
                           ws.Cells[SumRowStart + 1, 3, StartRowNo - 1, 3],
                           ws.Cells[SumRowStart + 1, 2, StartRowNo - 1, 2]);

                        break;
                }

            }

            return StartRowNo;
        }

        private int DrawAttributeWise(int StartRowNo, ExcelWorksheet ws, string caption, IList data, string defaultCurrency, IEnumerable<XElement> columns, IEnumerable<XElement> chartdata)
        {

            //Draw table caption
            ws.Cells[StartRowNo, 2, StartRowNo, 2].Value = HttpUtility.HtmlDecode(caption.ToString());
            FormatBlockCaption(ws.Cells[StartRowNo, 2, StartRowNo, 2]);

            ws.Row(StartRowNo).Height = 25;
            StartRowNo = StartRowNo + 1;

            //Draw table header
            for (int i = 0; i < columns.ToList().Count(); i++)
            {
                var Column = columns.ToList()[i].Elements("caption").FirstOrDefault().Value;

                if (columns.ToList()[i].Elements("id").FirstOrDefault().Value != "1")
                {
                    ws.Cells[StartRowNo, i + 2, StartRowNo, i + 2].Value = HttpUtility.HtmlDecode(Column.ToString());
                    FormatNormalFontRight(ws.Cells[StartRowNo, i + 2, StartRowNo, i + 2]);
                }
                else
                {
                    ws.Cells[StartRowNo, i + 2, StartRowNo, i + 2].Value = "";
                    FormatNormalFont(ws.Cells[StartRowNo, i + 2, StartRowNo, i + 2]);
                }

                FormatCellFillBold(ws.Cells[StartRowNo, i + 2, StartRowNo, i + 2]);



            }
            StartRowNo = StartRowNo + 1;

            var SumRowStart = StartRowNo;

            //Draw column and value
            for (int j = 0; j < data.Count; j++)
            {
                for (int i = 0; i < columns.ToList().Count(); i++)
                {
                    var Value = ((Hashtable)data[j])["col" + columns.ToList()[i].Elements("id").FirstOrDefault().Value];

                    if (columns.ToList()[i].Elements("id").FirstOrDefault().Value != "1")
                    {
                        ws.Cells[StartRowNo, i + 2, StartRowNo, i + 2].Value = Convert.ToInt32(Value);
                        FormatCell(ws.Cells[StartRowNo, i + 2, StartRowNo, i + 2]);
                        FormatNormalFontRight(ws.Cells[StartRowNo, i + 2, StartRowNo, i + 2]);
                        ws.Cells[StartRowNo, i + 2, StartRowNo, i + 2].Style.Numberformat.Format = "### ### ##0 [$" + defaultCurrency + "]";
                    }
                    else
                    {
                        ws.Cells[StartRowNo, i + 2, StartRowNo, i + 2].Value = HttpUtility.HtmlDecode(Value.ToString());
                        FormatCellFill(ws.Cells[StartRowNo, i + 2, StartRowNo, i + 2]);
                        FormatNormalFont(ws.Cells[StartRowNo, i + 2, StartRowNo, i + 2]);
                    }

                }
                StartRowNo = StartRowNo + 1;
            }

            for (int i = 0; i < columns.ToList().Count(); i++)
            {
                if (columns.ToList()[i].Elements("id").FirstOrDefault().Value != "1")
                {
                    ws.Cells[StartRowNo, i + 2, StartRowNo, i + 2].Formula = "Sum(" + ws.Cells[SumRowStart, i + 2].Address + "," + ws.Cells[StartRowNo - 1, i + 2].Address + ")";
                    FormatCell(ws.Cells[StartRowNo, i + 2, StartRowNo, i + 2]);
                    FormatNormalFontRightBold(ws.Cells[StartRowNo, i + 2, StartRowNo, i + 2]);
                    ws.Cells[StartRowNo, i + 2, StartRowNo, i + 2].Style.Numberformat.Format = "### ### ##0 [$" + defaultCurrency + "]";
                }
                else
                {
                    ws.Cells[StartRowNo, i + 2, StartRowNo, i + 2].Value = "Total";
                    FormatCellFill(ws.Cells[StartRowNo, i + 2, StartRowNo, i + 2]);
                    FormatNormalFontBold(ws.Cells[StartRowNo, i + 2, StartRowNo, i + 2]);
                }
            }

            StartRowNo = StartRowNo + 1;


            //Draw chart
            for (int i = 0; i < chartdata.ToList().Count(); i++)
            {
                var id = chartdata.ToList()[i].Elements("id").FirstOrDefault().Value;
                var type = chartdata.ToList()[i].Elements("chartType").FirstOrDefault().Value;
                var col = chartdata.ToList()[i].Elements("columns").FirstOrDefault().Value;

                switch (type)
                {
                    case "1":
                        if (col.Split(',').Length == 1)
                        {
                            var Title = "";
                            var CaptionIndex = 0;
                            var ValueIndex = 0;

                            for (int j = 0; j < columns.ToList().Count(); j++)
                            {
                                if (columns.ToList()[j].Elements("id").FirstOrDefault().Value == col)
                                {
                                    Title = columns.ToList()[j].Elements("caption").FirstOrDefault().Value;
                                    ValueIndex = j + 2;
                                }

                                if (columns.ToList()[j].Elements("id").FirstOrDefault().Value == "1")
                                {
                                    CaptionIndex = j + 2;
                                }
                            }

                            DrawColumnSingle(ws, id, SumRowStart - 2, columns.ToList().Count() + 1 + (i * 4),
                                                             ws.Cells[SumRowStart, ValueIndex, StartRowNo - 2, ValueIndex],
                                                             ws.Cells[SumRowStart, CaptionIndex, StartRowNo - 2, CaptionIndex], Title);
                        }
                        else if (col.Split(',').Length > 1)
                        {

                            var CaptionIndex = 0;

                            List<ExcelRangeBase> value = new List<ExcelRangeBase>();

                            for (int j = 0; j < columns.ToList().Count(); j++)
                            {
                                if (columns.ToList()[j].Elements("id").FirstOrDefault().Value == "1")
                                {
                                    CaptionIndex = j + 2;
                                }
                            }

                            for (int x = 0; x < col.Split(',').Length; x++)
                            {
                                var colIndex = col.Split(',')[x];

                                for (int j = 0; j < columns.ToList().Count(); j++)
                                {
                                    if (columns.ToList()[j].Elements("id").FirstOrDefault().Value == colIndex)
                                    {
                                        value.Add(ws.Cells[SumRowStart, j + 2, StartRowNo - 2, j + 2]);
                                    }
                                }
                            }

                            DrawColumnMultiple(ws, id, SumRowStart - 2, columns.ToList().Count() + 1 + (i * 4),
                                                            value,
                                                            ws.Cells[SumRowStart, CaptionIndex, StartRowNo - 2, CaptionIndex]);
                        }
                        break;

                    case "2":

                        if (col.Split(',').Length == 1)
                        {
                            var Title = "";
                            var CaptionIndex = 0;
                            var ValueIndex = 0;

                            for (int j = 0; j < columns.ToList().Count(); j++)
                            {
                                if (columns.ToList()[j].Elements("id").FirstOrDefault().Value == col)
                                {
                                    Title = columns.ToList()[j].Elements("caption").FirstOrDefault().Value;
                                    ValueIndex = j + 2;
                                }

                                if (columns.ToList()[j].Elements("id").FirstOrDefault().Value == "1")
                                {
                                    CaptionIndex = j + 2;
                                }
                            }

                            DrawBarSingle(ws, id, SumRowStart - 2, columns.ToList().Count() + 1 + (i * 4),
                                                             ws.Cells[SumRowStart, ValueIndex, StartRowNo - 2, ValueIndex],
                                                             ws.Cells[SumRowStart, CaptionIndex, StartRowNo - 2, CaptionIndex], Title);
                        }
                        else if (col.Split(',').Length > 1)
                        {

                            var CaptionIndex = 0;

                            List<ExcelRangeBase> value = new List<ExcelRangeBase>();

                            for (int j = 0; j < columns.ToList().Count(); j++)
                            {
                                if (columns.ToList()[j].Elements("id").FirstOrDefault().Value == "1")
                                {
                                    CaptionIndex = j + 2;
                                }
                            }

                            for (int x = 0; x < col.Split(',').Length; x++)
                            {
                                var colIndex = col.Split(',')[x];

                                for (int j = 0; j < columns.ToList().Count(); j++)
                                {
                                    if (columns.ToList()[j].Elements("id").FirstOrDefault().Value == colIndex)
                                    {
                                        value.Add(ws.Cells[SumRowStart, j + 2, StartRowNo - 2, j + 2]);
                                    }
                                }
                            }

                            DrawBarMultiple(ws, id, SumRowStart - 2, columns.ToList().Count() + 1 + (i * 4),
                                                            value,
                                                            ws.Cells[SumRowStart, CaptionIndex, StartRowNo - 2, CaptionIndex]);
                        }

                        break;
                    case "3":
                        if (col.Split(',').Length == 1)
                        {

                            var Title = "";
                            var CaptionIndex = 0;
                            var ValueIndex = 0;

                            for (int j = 0; j < columns.ToList().Count(); j++)
                            {
                                if (columns.ToList()[j].Elements("id").FirstOrDefault().Value == col)
                                {
                                    Title = columns.ToList()[j].Elements("caption").FirstOrDefault().Value;
                                    ValueIndex = j + 2;

                                }

                                if (columns.ToList()[j].Elements("id").FirstOrDefault().Value == "1")
                                {
                                    CaptionIndex = j + 2;
                                }
                            }

                            DrawPieSingle(ws, id, SumRowStart - 2, columns.ToList().Count() + 1 + (i * 4),
                                                             ws.Cells[SumRowStart, ValueIndex, StartRowNo - 2, ValueIndex],
                                                             ws.Cells[SumRowStart, CaptionIndex, StartRowNo - 2, CaptionIndex], Title);
                        }

                        break;
                }
            }

            return StartRowNo;
        }

        private int DrawDetail(int StartRowNo, ExcelWorksheet ws, string caption, IList data, IEnumerable<XElement> columns)
        {
            //Draw table caption
            ws.Cells[StartRowNo, 2, StartRowNo, 2].Value = HttpUtility.HtmlDecode(caption.ToString());
            FormatBlockCaption(ws.Cells[StartRowNo, 2, StartRowNo, 2]);

            ws.Row(StartRowNo).Height = 25;
            StartRowNo = StartRowNo + 1;

            //Draw table header
            for (int i = 0; i < columns.ToList().Count(); i++)
            {
                var Column = columns.ToList()[i].Elements("caption").FirstOrDefault().Value;

                ws.Cells[StartRowNo, i + 2, StartRowNo, i + 2].Value = HttpUtility.HtmlDecode(Column.ToString());

                if (columns.ToList()[i].Elements("id").FirstOrDefault().Value != "1")
                {

                    FormatNormalFontRight(ws.Cells[StartRowNo, i + 2, StartRowNo, i + 2]);

                }
                else
                {
                    FormatNormalFont(ws.Cells[StartRowNo, i + 2, StartRowNo, i + 2]);
                }
                FormatCellFillBold(ws.Cells[StartRowNo, i + 2, StartRowNo, i + 2]);

            }
            StartRowNo = StartRowNo + 1;

            var SumRowStart = StartRowNo;

            //Draw column and value
            for (int j = 0; j < data.Count; j++)
            {
                for (int i = 0; i < columns.ToList().Count(); i++)
                {
                    var Value = ((Hashtable)data[j])["col" + columns.ToList()[i].Elements("id").FirstOrDefault().Value];

                    if (columns.ToList()[i].Elements("id").FirstOrDefault().Value != "1")
                    {
                        ws.Cells[StartRowNo, i + 2, StartRowNo, i + 2].Value = HttpUtility.HtmlDecode(Value.ToString());
                        FormatCell(ws.Cells[StartRowNo, i + 2, StartRowNo, i + 2]);
                        FormatNormalFontRight(ws.Cells[StartRowNo, i + 2, StartRowNo, i + 2]);
                        //  ws.Cells[StartRowNo, i + 2, StartRowNo, i + 2].Style.Numberformat.Format =  "### ### ##0 [$" + CurrencyFormat + "]";
                    }
                    else
                    {
                        ws.Cells[StartRowNo, i + 2, StartRowNo, i + 2].Value = HttpUtility.HtmlDecode(Value.ToString());
                        FormatCell(ws.Cells[StartRowNo, i + 2, StartRowNo, i + 2]);
                        FormatNormalFont(ws.Cells[StartRowNo, i + 2, StartRowNo, i + 2]);
                    }

                }
                StartRowNo = StartRowNo + 1;
            }


            return StartRowNo;
        }


        private void DrawBarSingle(ExcelWorksheet ws, string id, int row, int column, ExcelRangeBase caption, ExcelRangeBase value, string Title = null)
        {

            Random rnd = new Random();

            var chart = ws.Drawings.AddChart("chart" + id + rnd.Next(1, 1000), eChartType.BarClustered) as ExcelBarChart;
            if (Title != null)
            {
                chart.Title.Text = chart.Title.Text = Title;
            }
            chart.SetPosition(Row: row, RowOffsetPixels: 1, Column: column, ColumnOffsetPixels: 20);
            chart.SetSize(PixelWidth: 500, PixelHeight: 250);


            var serise = chart.Series.Add(caption, value);

            chart.Legend.Remove();

            chart.Style = eChartStyle.Style10;

            chart.Border.Fill.Color = System.Drawing.Color.FromArgb(221, 221, 221);

            chart.Axis[1].Deleted = true;

            chart.Axis[0].MajorTickMark = eAxisTickMark.None;
            chart.Axis[0].MinorTickMark = eAxisTickMark.None;

            chart.Axis[0].Border.Fill.Color = System.Drawing.Color.FromArgb(127, 127, 127);

            chart.Axis[0].Font.Color = System.Drawing.Color.FromArgb(127, 127, 127);

            chart.DataLabel.ShowValue = true;
            chart.DataLabel.ShowCategory = false;

            chart.DataLabel.Font.Color = System.Drawing.Color.FromArgb(64, 64, 64);


            System.Xml.XmlNodeList nl = chart.ChartXml.GetElementsByTagName("c:majorGridlines");
            for (int x = 0; x < nl.Count; x++)
            {
                nl[x].ParentNode.RemoveChild(nl[x]);
            }
            System.Xml.XmlNodeList varyColors = chart.ChartXml.GetElementsByTagName("c:varyColors");
            varyColors[0].Attributes["val"].Value = "1";

            //System.Xml.XmlNodeList barChart = chart.ChartXml.GetElementsByTagName("c:barChart");

            ////barChart[0].OwnerDocument.Schemas[]

            //XmlElement gapWidth = barChart[0].OwnerDocument.CreateElement("c", "gapWidth", "http://schemas.openxmlformats.org/drawingml/2006/chart");


            //gapWidth.SetAttribute("val", "15");

            //barChart[0].AppendChild(gapWidth);
        }

        private void DrawColumnSingle(ExcelWorksheet ws, string id, int row, int column, ExcelRangeBase caption, ExcelRangeBase value, string Title = null)
        {

            Random rnd = new Random();

            var chart = ws.Drawings.AddChart("chart" + id + rnd.Next(1, 1000), eChartType.ColumnClustered) as ExcelBarChart;
            if (Title != null)
            {
                chart.Title.Text = chart.Title.Text = Title;
            }
            chart.SetPosition(Row: row, RowOffsetPixels: 1, Column: column, ColumnOffsetPixels: 20);
            chart.SetSize(PixelWidth: 500, PixelHeight: 250);


            var serise = chart.Series.Add(caption, value);

            chart.Legend.Remove();

            chart.Style = eChartStyle.Style10;

            chart.Border.Fill.Color = System.Drawing.Color.FromArgb(221, 221, 221);

            chart.Axis[1].Deleted = true;

            chart.Axis[0].MajorTickMark = eAxisTickMark.None;
            chart.Axis[0].MinorTickMark = eAxisTickMark.None;

            chart.Axis[0].Border.Fill.Color = System.Drawing.Color.FromArgb(127, 127, 127);

            chart.Axis[0].Font.Color = System.Drawing.Color.FromArgb(127, 127, 127);

            chart.DataLabel.ShowValue = true;
            chart.DataLabel.ShowCategory = false;

            chart.DataLabel.Font.Color = System.Drawing.Color.FromArgb(64, 64, 64);


            System.Xml.XmlNodeList nl = chart.ChartXml.GetElementsByTagName("c:majorGridlines");
            for (int x = 0; x < nl.Count; x++)
            {
                nl[x].ParentNode.RemoveChild(nl[x]);
            }
            System.Xml.XmlNodeList varyColors = chart.ChartXml.GetElementsByTagName("c:varyColors");
            varyColors[0].Attributes["val"].Value = "1";

            //System.Xml.XmlNodeList barChart = chart.ChartXml.GetElementsByTagName("c:barChart");

            ////barChart[0].OwnerDocument.Schemas[]

            //XmlElement gapWidth = barChart[0].OwnerDocument.CreateElement("c", "gapWidth", "http://schemas.openxmlformats.org/drawingml/2006/chart");


            //gapWidth.SetAttribute("val", "15");

            //barChart[0].AppendChild(gapWidth);
        }

        private void DrawPieSingle(ExcelWorksheet ws, string id, int row, int column, ExcelRangeBase caption, ExcelRangeBase value, string Title = null)
        {
            Random rnd = new Random();

            var chart = ws.Drawings.AddChart("chart" + id + rnd.Next(1, 1000), eChartType.Pie) as ExcelPieChart;
            if (Title != null)
            {
                chart.Title.Text = chart.Title.Text = Title;
            }

            chart.SetPosition(Row: row, RowOffsetPixels: 1, Column: column, ColumnOffsetPixels: 20);
            chart.SetSize(PixelWidth: 500, PixelHeight: 250);


            var serise = chart.Series.Add(caption, value);

            //chart.Legend.Remove();

            chart.Style = eChartStyle.Style10;

            chart.Border.Fill.Color = System.Drawing.Color.FromArgb(221, 221, 221);

            chart.DataLabel.ShowValue = true;
            chart.DataLabel.ShowCategory = false;

            chart.DataLabel.Font.Color = System.Drawing.Color.FromArgb(64, 64, 64);


            System.Xml.XmlNodeList nl = chart.ChartXml.GetElementsByTagName("c:majorGridlines");
            for (int x = 0; x < nl.Count; x++)
            {
                nl[x].ParentNode.RemoveChild(nl[x]);
            }
            System.Xml.XmlNodeList varyColors = chart.ChartXml.GetElementsByTagName("c:varyColors");
            varyColors[0].Attributes["val"].Value = "1";

        }

        private void DrawColumnMultiple(ExcelWorksheet ws, string id, int row, int column, List<ExcelRangeBase> caption, ExcelRangeBase value, string Title = null)
        {

            Random rnd = new Random();

            var chart = ws.Drawings.AddChart("chart" + id + rnd.Next(1, 1000), eChartType.ColumnClustered) as ExcelBarChart;
            if (Title != null)
            {
                chart.Title.Text = chart.Title.Text = Title;
            }
            chart.SetPosition(Row: row, RowOffsetPixels: 1, Column: column, ColumnOffsetPixels: 20);
            chart.SetSize(PixelWidth: 500, PixelHeight: 250);


            foreach (var item in caption)
            {
                chart.Series.Add(item, value);
            }

            //chart.Legend.Remove();

            chart.Legend.Position = eLegendPosition.Bottom;

            chart.Style = eChartStyle.Style10;

            chart.Border.Fill.Color = System.Drawing.Color.FromArgb(221, 221, 221);

            chart.Axis[1].Deleted = true;

            chart.Axis[0].MajorTickMark = eAxisTickMark.None;
            chart.Axis[0].MinorTickMark = eAxisTickMark.None;

            chart.Axis[0].Border.Fill.Color = System.Drawing.Color.FromArgb(127, 127, 127);

            chart.Axis[0].Font.Color = System.Drawing.Color.FromArgb(127, 127, 127);

            chart.DataLabel.ShowValue = true;
            chart.DataLabel.ShowCategory = false;

            chart.DataLabel.Font.Color = System.Drawing.Color.FromArgb(64, 64, 64);


            System.Xml.XmlNodeList nl = chart.ChartXml.GetElementsByTagName("c:majorGridlines");
            for (int x = 0; x < nl.Count; x++)
            {
                nl[x].ParentNode.RemoveChild(nl[x]);
            }
            System.Xml.XmlNodeList varyColors = chart.ChartXml.GetElementsByTagName("c:varyColors");
            varyColors[0].Attributes["val"].Value = "1";

        }
        private void DrawBarMultiple(ExcelWorksheet ws, string id, int row, int column, List<ExcelRangeBase> caption, ExcelRangeBase value, string Title = null)
        {

            Random rnd = new Random();

            var chart = ws.Drawings.AddChart("chart" + id + rnd.Next(1, 1000), eChartType.BarClustered) as ExcelBarChart;
            if (Title != null)
            {
                chart.Title.Text = chart.Title.Text = Title;
            }
            chart.SetPosition(Row: row, RowOffsetPixels: 1, Column: column, ColumnOffsetPixels: 20);
            chart.SetSize(PixelWidth: 500, PixelHeight: 250);


            foreach (var item in caption)
            {
                chart.Series.Add(item, value);
            }

            //chart.Legend.Remove();
            chart.Legend.Position = eLegendPosition.Bottom;

            chart.Style = eChartStyle.Style10;

            chart.Border.Fill.Color = System.Drawing.Color.FromArgb(221, 221, 221);

            chart.Axis[1].Deleted = true;

            chart.Axis[0].MajorTickMark = eAxisTickMark.None;
            chart.Axis[0].MinorTickMark = eAxisTickMark.None;

            chart.Axis[0].Border.Fill.Color = System.Drawing.Color.FromArgb(127, 127, 127);

            chart.Axis[0].Font.Color = System.Drawing.Color.FromArgb(127, 127, 127);

            chart.DataLabel.ShowValue = true;
            chart.DataLabel.ShowCategory = false;

            chart.DataLabel.Font.Color = System.Drawing.Color.FromArgb(64, 64, 64);


            System.Xml.XmlNodeList nl = chart.ChartXml.GetElementsByTagName("c:majorGridlines");
            for (int x = 0; x < nl.Count; x++)
            {
                nl[x].ParentNode.RemoveChild(nl[x]);
            }
            System.Xml.XmlNodeList varyColors = chart.ChartXml.GetElementsByTagName("c:varyColors");
            varyColors[0].Attributes["val"].Value = "1";

        }
        private void FormatCell(ExcelRange cel)
        {
            cel.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            cel.Style.Border.Left.Color.SetColor(System.Drawing.Color.FromArgb(221, 221, 221));

            cel.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            cel.Style.Border.Right.Color.SetColor(System.Drawing.Color.FromArgb(221, 221, 221));

            cel.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            cel.Style.Border.Top.Color.SetColor(System.Drawing.Color.FromArgb(221, 221, 221));

            cel.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            cel.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.FromArgb(221, 221, 221));
        }

        private void FormatCellFill(ExcelRange cel)
        {
            FormatCell(cel);
            cel.Style.Fill.PatternType = ExcelFillStyle.Solid;
            cel.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(244, 244, 244));
        }

        private void FormatCellFillBold(ExcelRange cel)
        {
            FormatCellFill(cel);
            cel.Style.Font.Bold = true;
        }

        private void FormatNormalFont(ExcelRange cel)
        {
            cel.Style.Font.Name = "Calibri";
            cel.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(51, 51, 51));
            cel.Style.Font.Size = 10;
            cel.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            cel.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cel.Style.Indent = 1;
        }

        private void FormatNormalFontRight(ExcelRange cel)
        {
            cel.Style.Font.Name = "Calibri";
            cel.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(51, 51, 51));
            cel.Style.Font.Size = 10;
            cel.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            cel.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cel.Style.Indent = 1;

        }

        private void FormatNormalFontCenter(ExcelRange cel)
        {
            cel.Style.Font.Name = "Calibri";
            cel.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(51, 51, 51));
            cel.Style.Font.Size = 10;
            cel.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cel.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cel.Style.Indent = 1;

        }

        private void FormatNormalFontBold(ExcelRange cel)
        {

            FormatNormalFont(cel);
            cel.Style.Font.Bold = true;
        }

        private void FormatNormalFontRightBold(ExcelRange cel)
        {

            FormatNormalFontRight(cel);
            cel.Style.Font.Bold = true;
        }


        private void FormatBlockCaption(ExcelRange cel)
        {
            cel.Style.Font.Name = "Calibri";
            cel.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(51, 51, 51));
            cel.Style.Font.Size = 14;
            cel.Style.Font.Bold = true;
            cel.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            cel.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        }

        public bool UpdateFinancialSettingsReportImage(ReportManagerProxy proxy, string sourcepath, int imgwidth, int imgheight, int imgX, int imgY, string Preview)
        {
            using (ITransaction tx = proxy.MarcomManager.GetTransaction())
            {
                try
                {
                    string orgsourcepath = HttpContext.Current.Server.MapPath(sourcepath);

                    orgsourcepath = orgsourcepath.Replace("report\\", "");
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
                                MemoryStream ms = new MemoryStream();
                                bmp.Save(ms, OriginalImage.RawFormat);
                                byte[] CropImage = ms.GetBuffer();
                                using (MemoryStream ms1 = new MemoryStream(CropImage, 0, CropImage.Length))
                                {
                                    ms.Write(CropImage, 0, CropImage.Length);
                                    using (SD.Image CroppedImage = SD.Image.FromStream(ms, true))
                                    {
                                        string destinationpath = HttpContext.Current.Server.MapPath("Files//ReportFiles//Images//" + Preview);
                                        destinationpath = destinationpath.Replace("report\\", "");
                                        if (File.Exists(destinationpath))
                                        {
                                            File.Delete(destinationpath);
                                        }
                                        CroppedImage.Save(destinationpath, CroppedImage.RawFormat);
                                    }
                                }
                            }
                        }
                    }
                    tx.Commit();
                    return true;
                }
                catch
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Deletes the financial report settings.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="Id">The id.</param>
        /// <returns>bool</returns>
        public bool DeletefinancialreportByID(ReportManagerProxy proxy, int reportID)
        {
            try
            {

                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    tx.PersistenceManager.PlanningRepository.DeleteByID<FinancialReportSettingsDao>(reportID);
                    tx.Commit();
                    string mappingfilesPath = AppDomain.CurrentDomain.BaseDirectory;
                    mappingfilesPath = mappingfilesPath + "Reports" + @"\ReportSettings_" + reportID + ".xml";
                    if (File.Exists(mappingfilesPath))
                    {
                        File.Delete(mappingfilesPath);
                    }
                }
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public string ExportTaskListtoExcel(ReportManagerProxy proxy, int entityId, int taskListId, bool isEntireTaskList, bool isIncludeSublevel)
        {
            try
            {
                proxy.MarcomManager.AccessManager.TryEntityTypeAccess(entityId, Modules.Planning);
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    IList<IEntitySublevelTaskHolder> iitaskDetails = new List<IEntitySublevelTaskHolder>();
                    iitaskDetails = GetReportEntityTaskListRecords(proxy, entityId, taskListId, isEntireTaskList, isIncludeSublevel);
                    string reportGUID = GenerateTaskListreport(iitaskDetails);
                    return reportGUID;
                }

            }
            catch (MarcomAccessDeniedException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                return null;
            }
        }



        public IList<IEntitySublevelTaskHolder> GetReportEntityTaskListRecords(ReportManagerProxy proxy, int entityID, int taskListId, bool isEntireTaskList, bool includesublevel)
        {

            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {

                    IList<IEntitySublevelTaskHolder> iitsk = new List<IEntitySublevelTaskHolder>();
                    var parentNode = from item in tx.PersistenceManager.TaskRepository.Query<EntityDao>()
                                     where item.Id == entityID && item.Active == true
                                     select item;
                    IList<EntityDao> dao = new List<EntityDao>();
                    string uniquekey = parentNode.First().UniqueKey;
                    int[] systemDefinedTypes = { (int)EntityTypeList.CostCentre, (int)EntityTypeList.FundinngRequest, (int)EntityTypeList.Milestone, (int)EntityTypeList.Objective, (int)EntityTypeList.Task, 
                                                   (int)TaskTypes.Approval_Task, (int)TaskTypes.Work_Task, (int)TaskTypes.Reviewal_Task };
                    dao = (from item in tx.PersistenceManager.TaskRepository.Query<EntityDao>() where item.Id == entityID select item).ToList();
                    int[] typeidArr = dao.Select(b => b.Typeid).Distinct().ToArray();
                    var entTypeList = tx.PersistenceManager.TaskRepository.Query<EntityTypeDao>().Where(a => typeidArr.Contains(a.Id)).Select(a => a);
                    foreach (var item in dao)
                    {
                        IEntitySublevelTaskHolder itsk = new EntitySublevelTaskHolder();
                        itsk.EntityID = item.Id;
                        itsk.EntityName = item.Name;
                        var entTypeDao = entTypeList.Where(a => a.Id == item.Typeid).Select(a => a).FirstOrDefault();
                        itsk.EntityTypeColorCode = entTypeDao.ColorCode;
                        itsk.EntityTypeID = item.Typeid;
                        itsk.EntityTypeShortDescription = entTypeDao.ShortDescription;
                        itsk.EntityUniqueKey = item.UniqueKey;
                        itsk.SortOrder = item.EntityID;
                        itsk.TaskListGroup = GetSublevelEntityTaskList(proxy, item.Id, taskListId, isEntireTaskList, false);
                        iitsk.Add(itsk);
                    }

                    if (includesublevel)
                    {
                        IList<IEntitySublevelTaskHolder> iisubleveltsk = new List<IEntitySublevelTaskHolder>();
                        iisubleveltsk = GetSublevelTaskList(proxy, entityID);
                        //iitsk = new List<IEntitySublevelTaskHolder>(iitsk.Concat(iisubleveltsk));
                        iitsk = iitsk.Union(iisubleveltsk).ToList();

                    }
                    return iitsk;
                }
            }
            catch
            {
                return null;
            }

        }

        public IList<IEntitySublevelTaskHolder> GetSublevelTaskList(ReportManagerProxy proxy, int entityID)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    IList<IEntitySublevelTaskHolder> iitsk = new List<IEntitySublevelTaskHolder>();
                    var parentNode = from item in tx.PersistenceManager.TaskRepository.Query<EntityDao>()
                                     where item.Id == entityID && item.Active == true
                                     select item;
                    IList<EntityDao> dao = new List<EntityDao>();
                    string uniquekey = parentNode.First().UniqueKey;
                    int[] systemDefinedTypes = { (int)EntityTypeList.CostCentre, (int)EntityTypeList.FundinngRequest, (int)EntityTypeList.Milestone, (int)EntityTypeList.Objective, (int)EntityTypeList.Task, 
                                                   (int)TaskTypes.Approval_Task, (int)TaskTypes.Work_Task, (int)TaskTypes.Reviewal_Task };
                    dao = (from item in tx.PersistenceManager.TaskRepository.Query<EntityDao>()
                           join associatetype in tx.PersistenceManager.TaskRepository.Query<EntityTypeDao>()
                              on item.Typeid equals associatetype.Id
                           where item.UniqueKey.StartsWith("" + uniquekey + ".") && item.Active == true && associatetype.IsAssociate == false && associatetype.Category == 2
                           && !systemDefinedTypes.Contains(associatetype.Id)
                           select item).OrderBy(a => a.UniqueKey).ToList<EntityDao>();
                    int[] typeidArr = dao.Select(b => b.Typeid).Distinct().ToArray();
                    var entTypeList = tx.PersistenceManager.TaskRepository.Query<EntityTypeDao>().Where(a => typeidArr.Contains(a.Id)).Select(a => a);
                    foreach (var item in dao)
                    {
                        IEntitySublevelTaskHolder itsk = new EntitySublevelTaskHolder();
                        itsk.EntityID = item.Id;
                        itsk.EntityName = item.Name;
                        var entTypeDao = entTypeList.Where(a => a.Id == item.Typeid).Select(a => a).FirstOrDefault();
                        itsk.EntityTypeColorCode = entTypeDao.ColorCode;
                        itsk.EntityTypeID = item.Typeid;
                        itsk.EntityTypeShortDescription = entTypeDao.ShortDescription;
                        itsk.EntityUniqueKey = item.UniqueKey;
                        itsk.SortOrder = item.EntityID;
                        itsk.TaskListGroup = GetSublevelEntityTaskList(proxy, item.Id, 0, false, true);
                        iitsk.Add(itsk);
                    }

                    return iitsk;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public IList<ITaskLibraryTemplateHolder> GetSublevelEntityTaskList(ReportManagerProxy proxy, int entityID, int tasklistID, bool isEntireTaskList, bool isSublevel)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    IList<ITaskLibraryTemplateHolder> tasklist = new List<ITaskLibraryTemplateHolder>();
                    IList<EntityTaskListDao> entityTaskList = new List<EntityTaskListDao>();
                    if (!isSublevel)
                    {
                        if (isEntireTaskList)
                        {
                            string taskcountStr = "SELECT COUNT(*) AS taskcount FROM TM_EntityTaskList tet WHERE tet.EntityID= ? ";
                            IList taskcountlst = tx.PersistenceManager.PlanningRepository.ExecuteQuerywithMinParam(taskcountStr, entityID);
                            int taskclount = (int)((System.Collections.Hashtable)(taskcountlst)[0])["taskcount"];
                            if (taskclount > 0)
                                entityTaskList = tx.PersistenceManager.ReportRepository.Query<EntityTaskListDao>().Where(a => a.EntityID == entityID).Select(a => a).OrderBy(a => a.Sortorder).ToList<EntityTaskListDao>();
                        }
                        else
                            entityTaskList = tx.PersistenceManager.ReportRepository.Query<EntityTaskListDao>().Where(a => a.EntityID == entityID && a.ID == tasklistID).Select(a => a).OrderBy(a => a.Sortorder).ToList<EntityTaskListDao>();
                    }
                    else
                    {
                        entityTaskList = tx.PersistenceManager.ReportRepository.Query<EntityTaskListDao>().Where(a => a.EntityID == entityID).Select(a => a).OrderBy(a => a.Sortorder).ToList<EntityTaskListDao>();
                    }

                    foreach (var val in entityTaskList)
                    {
                        ITaskLibraryTemplateHolder tskLst = new TaskLibraryTemplateHolder();
                        tskLst.LibraryName = val.Name;
                        tskLst.LibraryDescription = val.Description;
                        tskLst.ID = val.ID;
                        tskLst.SortOrder = val.Sortorder;
                        tskLst.TaskList = proxy.MarcomManager.TaskManager.GetEntityTaskListDetails(val.EntityID, val.ID);
                        tskLst.IsExpanded = false;
                        tskLst.IsGetTasks = false;
                        tasklist.Add(tskLst);
                    }

                    return tasklist;
                }
            }
            catch
            {
                return null;
            }

        }

        public string GenerateTaskListreport(IList<IEntitySublevelTaskHolder> iitaskDetails)
        {

            try
            {

                string NewGuid = Guid.NewGuid().ToString();

                if (iitaskDetails.Count > 0)
                {

                    string mappingfilesPath = AppDomain.CurrentDomain.BaseDirectory;

                    dynamic fullpath = mappingfilesPath + ("/Files/ReportFiles/Images/Temp/") + NewGuid + ".xlsx";

                    FileInfo newFile = new FileInfo(fullpath);

                    ExcelPackage pck = new ExcelPackage(newFile);
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("TaskList Export");


                    taskAssigneeImages = new Dictionary<int, string>();
                    taskHeaderLst = new List<string>();
                    taskHeaderLst.Add("Task Name");
                    taskHeaderLst.Add("Status");
                    taskHeaderLst.Add("Due date");
                    taskHeaderLst.Add("Assignee(s)");

                    DrawReportHeader(ws, 1, 2);//Draw Report Header
                    DrawTaskListReport(ws, iitaskDetails); // generate tasklist report
                    FillTaskAssigneeImages(ws, taskAssigneeImages); // to draw user images

                    ws.View.ShowGridLines = false;


                    pck.Workbook.Properties.Title = "Task List";
                    pck.Workbook.Properties.Author = "Marcom Plarform";
                    pck.Workbook.Properties.Subject = "Task List";
                    pck.Workbook.Properties.Keywords = "Task List";

                    pck.Save();

                    string strFriendlyName = "Task List-" + System.DateTime.Now.ToString("dd-MM-yyyy") + ".xlsx";

                    dynamic name = System.IO.Path.GetFileName(fullpath);
                    name = name.Replace(System.IO.Path.GetFileName(fullpath), strFriendlyName);
                    dynamic ext = System.IO.Path.GetExtension(fullpath);
                    string type = "";
                    type = "application/vnd.ms-excel";


                }
                return NewGuid;
            }
            catch (Exception ex)
            {
                //Log("Page_Load Exception", ex.Message + Constants.vbNewLine + ex.StackTrace);
            }

            return null;
        }

        public void DrawTaskListReport(ExcelWorksheet ws, IList<IEntitySublevelTaskHolder> iitaskDetails)
        {
            try
            {
                int currentRowNo = 2;
                ws.Column(1).Width = 2;
                ws.Column(2).Width = 3;

                foreach (var activity in iitaskDetails)
                {
                    GenerateTaskTypeIcon(ws, activity.EntityTypeShortDescription, activity.EntityTypeColorCode, activity.EntityName, 2, 3, currentRowNo);
                    currentRowNo = currentRowNo + 1;

                    if (activity.TaskListGroup.Count > 0)

                        foreach (var item in activity.TaskListGroup)
                        {
                            int taskcount = Enumerable.Count(item.TaskList);

                            DrawTaskListHeader(ws, currentRowNo, 2, item.LibraryName, taskcount);
                            currentRowNo = currentRowNo + 1;



                            IList<IEntityTask> iitasklistObj = new List<IEntityTask>();

                            iitasklistObj = item.TaskList;

                            if (iitasklistObj.Count > 0)
                            {
                                DrawTaskListTableHeader(ws, currentRowNo, 2, taskHeaderLst);
                                currentRowNo = currentRowNo + 1;
                                foreach (var task in iitasklistObj)
                                {
                                    ws.Row(currentRowNo).Height = 15;
                                    ws.Column(2).Width = 3;
                                    DrawTaskListIcon(ws, currentRowNo, 2);

                                    var namecell = ws.Cells[currentRowNo, 3];
                                    var cellnamevalues = HttpUtility.HtmlDecode(task.Name);
                                    namecell.Value = HttpUtility.HtmlDecode(cellnamevalues.ToString());

                                    ws.Column(3).Width = 50;
                                    namecell.Style.WrapText = true;
                                    // namecell.Style.Font.Bold = true;



                                    namecell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    namecell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(245, 245, 245));


                                    namecell.Style.Font.Name = "arial";

                                    namecell.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(0, 136, 204));
                                    namecell.Style.Font.Size = 10;



                                    namecell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    namecell.Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                                    //status icon 
                                    ws.Column(4).Width = 3;
                                    namecell = ws.Cells[currentRowNo, 4];
                                    namecell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    namecell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(245, 245, 245));
                                    AddTaskListSettingsImage(ws, 3, currentRowNo - 1, TaskStatusIcons(task.TaskStatus, task.totalDueDays, task.EntityTaskListID, task.TaskType).Item1);

                                    namecell = ws.Cells[currentRowNo, 5];
                                    cellnamevalues = HttpUtility.HtmlDecode(task.StatusName);
                                    namecell.Value = HttpUtility.HtmlDecode(cellnamevalues.ToString()) + task.ProgressCount;

                                    ws.Column(5).Width = 20;


                                    namecell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    namecell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(245, 245, 245));


                                    namecell.Style.Font.Name = "arial";

                                    namecell.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(51, 51, 51));
                                    namecell.Style.Font.Size = 10;



                                    namecell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    namecell.Style.VerticalAlignment = ExcelVerticalAlignment.Top;



                                    //due date
                                    namecell = ws.Cells[currentRowNo, 6];
                                    if (task.strDate != "" && (task.TaskStatus == 1 || task.TaskStatus == 0))
                                    {
                                        if (task.totalDueDays != 0)
                                            namecell.Value = task.strDate + "(" + task.totalDueDays + " days)";
                                        else
                                            namecell.Value = task.strDate + "(Today)";
                                    }
                                    else
                                        namecell.Value = "";

                                    ws.Column(6).Width = 30;

                                    namecell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    namecell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(245, 245, 245));


                                    namecell.Style.Font.Name = "arial";
                                    namecell.Style.Font.Color.SetColor(TaskStatusIcons(task.TaskStatus, task.totalDueDays, task.EntityTaskListID, task.TaskType).Item2);
                                    namecell.Style.Font.Size = 10;


                                    namecell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    namecell.Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                                    //assignees image
                                    ws.Column(7).Width = 3;
                                    namecell = ws.Cells[currentRowNo, 7];
                                    //ws.Row(currentRowNo).Height = 20;

                                    namecell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    namecell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(245, 245, 245));
                                    string assigneeName = "";

                                    //find the task assignee image path logic goes here
                                    try
                                    {
                                        if (task.taskAssigness != null && task.TaskStatus != 0)
                                        {
                                            if (Enumerable.Count(task.taskAssigness) == 1)
                                            {
                                                if (File.Exists(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "UserImages\\" + task.AssigneeID + ".jpg"))
                                                    taskAssigneeImages.Add(currentRowNo - 1, System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "UserImages\\" + task.AssigneeID + ".jpg");
                                                else
                                                    taskAssigneeImages.Add(currentRowNo - 1, System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "UserImages\\noimage.jpg");
                                                assigneeName = HttpUtility.HtmlDecode(task.AssigneeName);
                                            }
                                            else
                                            {
                                                taskAssigneeImages.Add(currentRowNo - 1, System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "assets\\img\\Group.png");
                                                assigneeName = "Group";
                                                var totalTaskmembers = task.taskMembers.Where(a => a.RoleID != 1).Select(a => a.UserName).ToList();
                                                string assignenames = String.Join(",", totalTaskmembers.Select(x => x.ToString()).ToArray());
                                                TaskAssigneeGroupNameComment(ws, currentRowNo, assignenames, totalTaskmembers.Count());
                                            }
                                        }
                                    }
                                    catch
                                    {

                                    }

                                    namecell = ws.Cells[currentRowNo, 8];
                                    namecell.Value = assigneeName;

                                    ws.Column(8).Width = 30;


                                    namecell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    namecell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(245, 245, 245));

                                    namecell.Style.Font.Name = "arial";
                                    namecell.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(51, 51, 51));
                                    namecell.Style.Font.Size = 10;

                                    namecell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    namecell.Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                                    currentRowNo++;
                                    ws.Row(currentRowNo).Height = 5;
                                    currentRowNo++;
                                }
                            }
                            else
                            {
                                GenerateEmptyTaskList(ws, 2, currentRowNo, "No task available");
                            }
                            currentRowNo = currentRowNo + 2;
                        }
                    else
                    {
                        GenerateEmptyTaskList(ws, 2, currentRowNo, "No tasklist available");
                        currentRowNo = currentRowNo + 2;
                    }
                }

            }
            catch (Exception ex)
            {

            }
        }

        public void TaskAssigneeGroupNameComment(ExcelWorksheet ws, int currentRowNo, string taskassignes, int assigneecount)
        {
            taskAssigneeComment = default(ExcelComment);
            taskAssigneeComment = ws.Comments.Add(ws.Cells[currentRowNo, 7], taskassignes + Environment.NewLine, "Marcom Platform");
            taskAssigneeComment.Font.FontName = "arial";
            taskAssigneeComment.Font.Size = 10;
            taskAssigneeComment.From.Column = 1;
            taskAssigneeComment.From.Row = 1;
            taskAssigneeComment.To.Column = assigneecount + 1;
            taskAssigneeComment.To.Row = assigneecount;
            taskAssigneeComment.BackgroundColor = System.Drawing.Color.FromArgb(231, 242, 245);
        }

        public Tuple<string, Color> TaskStatusIcons(int statusCode, int overdue, int IsAdminTask, int taskType)
        {
            var baseIconPath = "";
            string statusname = Enum.GetName(typeof(TaskStatus), statusCode);
            Color statuscolor = System.Drawing.Color.FromArgb(102, 102, 102);
            if (statusCode == 0 && overdue < 0 && IsAdminTask > 0)
                statuscolor = System.Drawing.Color.FromArgb(255, 17, 17);
            if (statusCode == 0 && overdue < 0 && IsAdminTask >= 0)
                statuscolor = System.Drawing.Color.FromArgb(255, 17, 17);
            if (statusCode == 1 && overdue < 0 && IsAdminTask >= 0)
                statuscolor = System.Drawing.Color.FromArgb(255, 17, 17);
            if (statusCode == 8)
                statusname = (taskType == 3 ? "Approved" : "Completed");
            if (File.Exists(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "assets\\img\\" + statusname + ".png"))
                baseIconPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "assets\\img\\" + statusname + ".png";
            else
                baseIconPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "assets\\img\\In_progress.png";
            Tuple<string, Color> returnobj = Tuple.Create(baseIconPath, statuscolor);
            return returnobj;
        }

        public void FillTaskAssigneeImages(ExcelWorksheet ws, Dictionary<int, string> taskimageColl)
        {
            try
            {
                foreach (var rowno in taskimageColl.Keys)
                {
                    AddTaskListAssigneeImage(ws, 6, rowno, taskimageColl[rowno]);
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void DrawTaskListIcon(ExcelWorksheet ws, int StartRowNo, int StartColumnNo)
        {

            var _with1 = ws.Cells[StartRowNo, StartColumnNo];
            _with1.Style.Fill.PatternType = ExcelFillStyle.Solid;
            _with1.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(245, 245, 245));
            AddTaskListSettingsImage(ws, 1, StartRowNo - 1, System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "assets\\img\\settings.png");

        }


        public void DrawTaskListHeader(ExcelWorksheet ws, int StartRowNo, int StartColumnNo, string TaskListName, int taskcount)
        {
            ws.Row(StartRowNo).Height = 20;
            //ws.Column(StartColumnNo).Width = 100;

            var _with1 = ws.Cells[StartRowNo, StartColumnNo];

            _with1.Value = HttpUtility.HtmlDecode(TaskListName) + " (" + Convert.ToString(taskcount) + " tasks) ";

            _with1.Style.Font.Name = "Arial";
            _with1.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(64, 64, 64));
            _with1.Style.Font.Size = 10;
            _with1.Style.Font.Bold = true;
            //_with1.Style.WrapText = true;


        }

        public void DrawReportHeader(ExcelWorksheet ws, int StartRowNo, int StartColumnNo)
        {
            ws.Row(StartRowNo).Height = 40;
            //ws.Column(StartColumnNo).Width = 100;

            var _with1 = ws.Cells[StartRowNo, StartColumnNo];


            _with1.Value = "TaskList Report " + System.DateTime.Now.ToString("yyyy-MM-dd") + "";

            _with1.Style.Font.Name = "Arial";
            _with1.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(64, 64, 64));
            _with1.Style.Font.Size = 16;
            _with1.Style.Font.Bold = true;
            //_with1.Style.WrapText = true;
            _with1.Style.VerticalAlignment = ExcelVerticalAlignment.Center;



        }

        public void DrawTaskListTableHeader(ExcelWorksheet ws, int StartRowNo, int StartColumnNo, List<string> taskHeaderLst)
        {
            ws.Row(StartRowNo).Height = 18;
            //task name header
            foreach (var header in taskHeaderLst)
            {

                var namecell = ws.Cells[StartRowNo, 2];

                if (header == "Task Name")
                {
                    namecell = ws.Cells[StartRowNo, 2, StartRowNo, 3];
                    namecell.Merge = true;
                }
                else if (header == "Status")
                {
                    namecell = ws.Cells[StartRowNo, 4, StartRowNo, 5];
                    namecell.Merge = true;
                }
                else if (header == "Due date")
                    namecell = ws.Cells[StartRowNo, 6];

                else if (header == "Assignee(s)")
                {
                    namecell = ws.Cells[StartRowNo, 7, StartRowNo, 8];
                    namecell.Merge = true;
                }

                namecell.Value = header;
                namecell.Style.Font.Bold = true;
                namecell.Style.Font.Name = "Calibri";
                namecell.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(51, 51, 51));
                namecell.Style.Font.Size = 10;
                namecell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                namecell.Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
            }
        }

        private void AddTaskListSettingsImage(ExcelWorksheet ws, int columnIndex, int rowIndex, string filePath)
        {
            //How to Add a Image using EP Plus
            Bitmap image = new Bitmap(filePath);
            //Bitmap image = new Bitmap(filePath);
            //ExcelPicture picture = null;
            ExcelPicture picture = null;
            if ((image != null))
            {
                picture = ws.Drawings.AddPicture("pic" + rowIndex.ToString() + columnIndex.ToString(), image);
                picture.From.Column = columnIndex;
                picture.From.Row = rowIndex;
                picture.From.ColumnOff = 20;
                picture.From.RowOff = 20;
                picture.SetPosition(rowIndex, 3, columnIndex, 3);

                picture.SetSize(14, 14);
            }
        }

        private void AddTaskListAssigneeImage(ExcelWorksheet ws, int columnIndex, int rowIndex, string filePath)
        {
            //How to Add a Image using EP Plus
            Bitmap image = new Bitmap(filePath);

            //Bitmap image = new Bitmap(filePath);
            //ExcelPicture picture = null;
            ExcelPicture picture = null;
            if ((image != null))
            {
                picture = ws.Drawings.AddPicture("pic" + rowIndex.ToString() + columnIndex.ToString(), image);
                picture.From.Column = columnIndex;
                picture.From.Row = rowIndex;
                picture.SetPosition(rowIndex, 3, columnIndex, 3);

                picture.SetSize(assigneeWidth, assigneeHeight);
            }
        }

        public void GenerateTaskTypeIcon(ExcelWorksheet ws, string SD, string ccode, string entityName, int startcolumnno, int endcolumnno, int rowno)
        {

            try
            {
                //drawing the shortdescription with colorcode background

                var _with2 = ws.Cells[rowno, startcolumnno];
                _with2.Value = SD;
                _with2.Style.Fill.PatternType = ExcelFillStyle.Solid;
                _with2.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#" + ccode));
                _with2.Style.Font.Name = "arial";
                _with2.Style.Font.Color.SetColor(Color.White);
                _with2.Style.Font.Size = 8;
                _with2.Style.Font.Bold = true;
                _with2.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                _with2.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                _with2.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                _with2.Style.Border.Left.Color.SetColor(System.Drawing.Color.FromArgb(217, 217, 217));
                _with2.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                _with2.Style.Border.Right.Color.SetColor(System.Drawing.Color.FromArgb(217, 217, 217));
                _with2.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                _with2.Style.Border.Top.Color.SetColor(System.Drawing.Color.FromArgb(217, 217, 217));
                _with2.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                _with2.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.FromArgb(217, 217, 217));


                ws.Row(rowno).Height = 20;

                var _with1 = ws.Cells[rowno, endcolumnno];

                _with1.Value = HttpUtility.HtmlDecode(entityName);

                _with1.Style.Font.Name = "Arial";
                _with1.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(64, 64, 64));
                _with1.Style.Font.Size = 12;
                _with1.Style.Font.Bold = true;
            }
            catch
            {

            }

        }

        public void GenerateEmptyTaskList(ExcelWorksheet ws, int startcolumnno, int rowno, string msg)
        {

            try
            {

                ws.Row(rowno).Height = 20;

                var _with1 = ws.Cells[rowno, startcolumnno];

                _with1.Value = msg;

                _with1.Style.Font.Name = "Arial";
                _with1.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(255, 0, 0));
                _with1.Style.Font.Size = 10;
                _with1.Style.Font.Bold = true;
            }
            catch
            {

            }

        }

        /// <summary>
        /// Getting list of Options for Fulfillment Entity Type Attributes
        /// </summary>
        /// <param name="proxy">The Proxy</param>
        /// <param name="entityTypeId">The EntityTypeID</param>
        /// <returns>IList of IAttribute</returns>
        public IList<IAttribute> GetFulfillmentAttribute(ReportManagerProxy proxy, int[] entityTypeId)
        {
            IList<IAttribute> listAttributes = new List<IAttribute>();
            string xmlPath = string.Empty;
            //int versionNumber = MarcomManagerFactory.AdminMetadataVersionNumber;

            int versionNumber = MarcomManagerFactory.ActiveMetadataVersionNumber;
            using (ITransaction tx = proxy.MarcomManager.GetTransaction())
            {
                //xmlPath = tx.PersistenceManager.MetadataRepository.GetXmlWorkingPath(versionNumber);
                xmlPath = tx.PersistenceManager.MetadataRepository.GetXmlPath(versionNumber);
                int[] FilterAttributes = { (int)SystemDefinedAttributes.MyRoleGlobalAccess, (int)SystemDefinedAttributes.MyRoleEntityAccess,
                                             (int)SystemDefinedAttributes.Owner };
                XDocument xDoc = XDocument.Load(xmlPath);
                IList<EntityTypeAttributeRelationDao> entityTypeRealtionDao = new List<EntityTypeAttributeRelationDao>();
                IList<AttributeDao> attributesDao = new List<AttributeDao>();
                if (versionNumber != 0)
                {

                    var entityTypeXmlDao = tx.PersistenceManager.MetadataRepository.GetObject<EntityTypeAttributeRelationDao>(xmlPath).Join
                        (tx.PersistenceManager.MetadataRepository.GetObject<AttributeDao>(xmlPath),
                        entr => entr.AttributeID, at => at.Id, (entr, at) => new { entr, at }).Where(a => entityTypeId.Contains(a.entr.EntityTypeID)
                        && !FilterAttributes.Contains(a.at.Id)).Select(a => a.at).Distinct();
                    //(a.at.AttributeTypeID == Convert.ToInt32(AttributesList.ListSingleSelection) ||
                    //a.at.AttributeTypeID == Convert.ToInt32(AttributesList.ListMultiSelection) ||
                    //a.at.AttributeTypeID == Convert.ToInt32(AttributesList.DropDownTree)) && (a.at.IsSpecial != true) && entityTypeId.Contains(a.entr.EntityTypeID)).Select(a => a.at).Distinct();
                    var treeXmlResult = entityTypeXmlDao.Where(a => a.AttributeTypeID == Convert.ToInt32(AttributesList.DropDownTree));

                    foreach (var obe in treeXmlResult)
                    {
                        var treeLevelXmlResult = tx.PersistenceManager.MetadataRepository.GetObject<TreeLevelDao>(xmlPath).Where(a => a.AttributeID == obe.Id);
                        foreach (var levelObj in treeLevelXmlResult)
                        {
                            IAttribute fullfillattributeObj = new BrandSystems.Marcom.Core.Metadata.Attribute();
                            fullfillattributeObj.Id = obe.Id;
                            fullfillattributeObj.Caption = levelObj.LevelName;
                            fullfillattributeObj.Level = levelObj.Level;
                            fullfillattributeObj.AttributeTypeID = obe.AttributeTypeID;
                            listAttributes.Add(fullfillattributeObj);
                        }
                    }
                    foreach (var obj in entityTypeXmlDao)
                    {
                        if (obj.AttributeTypeID != Convert.ToInt32(AttributesList.DropDownTree))
                        {

                            IAttribute fullfillattributeObj = new BrandSystems.Marcom.Core.Metadata.Attribute();
                            if (obj.Id != 308)
                            {
                                fullfillattributeObj.Id = obj.Id;
                                fullfillattributeObj.Caption = obj.Caption;
                                fullfillattributeObj.Level = 0;
                                fullfillattributeObj.AttributeTypeID = obj.AttributeTypeID;
                                listAttributes.Add(fullfillattributeObj);
                            }
                            else
                            {
                                fullfillattributeObj.Id = obj.Id;
                                fullfillattributeObj.Caption = "StartDate";
                                fullfillattributeObj.Level = 0;
                                fullfillattributeObj.AttributeTypeID = obj.AttributeTypeID;
                                listAttributes.Add(fullfillattributeObj);

                                IAttribute endDateObj = new BrandSystems.Marcom.Core.Metadata.Attribute();
                                endDateObj.Id = obj.Id;
                                endDateObj.Caption = "EndDate";
                                endDateObj.Level = -1;
                                endDateObj.AttributeTypeID = obj.AttributeTypeID;
                                listAttributes.Add(endDateObj);

                            }

                        }

                    }
                }

                else
                {
                    entityTypeRealtionDao = tx.PersistenceManager.MetadataRepository.Query<EntityTypeAttributeRelationDao>().Where(a => entityTypeId.Contains(a.EntityTypeID)).ToList();
                    attributesDao = tx.PersistenceManager.MetadataRepository.Query<AttributeDao>().Join(entityTypeRealtionDao, a => a.Id, b => b.AttributeID, (ab, bc) =>
                         new { ab, bc }).Where(a => entityTypeId.Contains(a.bc.EntityTypeID) && !FilterAttributes.Contains(a.bc.AttributeID)).Select(a => a.ab).Distinct().ToList();
                    //new { ab, bc }).Where(a => (a.ab.AttributeTypeID == Convert.ToInt32(AttributesList.ListSingleSelection)
                    //    || a.ab.AttributeTypeID == Convert.ToInt32(AttributesList.ListMultiSelection)
                    //    || a.ab.AttributeTypeID == Convert.ToInt32(AttributesList.DropDownTree)) && entityTypeId.Contains(a.bc.EntityTypeID)).Select
                    //    (a => a.ab).Distinct().ToList();
                    var treeResult = attributesDao.Where(a => a.AttributeTypeID == Convert.ToInt32(AttributesList.DropDownTree));
                    foreach (var obe in treeResult)
                    {
                        var treeLeveResult = tx.PersistenceManager.PlanningRepository.Query<TreeLevelDao>().Where(a => a.AttributeID == obe.Id);
                        foreach (var levelObj in treeLeveResult)
                        {
                            IAttribute fullfillattributeObj = new BrandSystems.Marcom.Core.Metadata.Attribute();
                            fullfillattributeObj.Id = obe.Id;
                            fullfillattributeObj.Caption = levelObj.LevelName;
                            fullfillattributeObj.Level = levelObj.Level;
                            fullfillattributeObj.AttributeTypeID = obe.AttributeTypeID;
                            listAttributes.Add(fullfillattributeObj);
                        }
                    }
                    foreach (var obj in attributesDao)
                    {
                        if (obj.AttributeTypeID != Convert.ToInt32(AttributesList.DropDownTree))
                        {
                            IAttribute fullfillattributeObj = new BrandSystems.Marcom.Core.Metadata.Attribute();
                            fullfillattributeObj.Id = obj.Id;
                            fullfillattributeObj.Caption = obj.Caption;
                            fullfillattributeObj.Level = 0;
                            fullfillattributeObj.AttributeTypeID = obj.AttributeTypeID;
                            listAttributes.Add(fullfillattributeObj);
                        }

                    }
                }
                IAttribute fullfillattributeIdObj = new BrandSystems.Marcom.Core.Metadata.Attribute();
                fullfillattributeIdObj.Id = -1;
                fullfillattributeIdObj.Caption = "ID";
                fullfillattributeIdObj.Level = 0;
                fullfillattributeIdObj.AttributeTypeID = -1;
                listAttributes.Add(fullfillattributeIdObj);

                return listAttributes;
            }
        }



        public int InsertUpdateCustomlist(ReportManagerProxy proxy, int ID, string Name, string Description, string XmlData, string ValidatedQuery)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {

                    int cutomReportCount = 0;
                    IList customListRes = tx.PersistenceManager.PlanningRepository.ExecuteQuerywithMinParam("SELECT COUNT(1) AS 'reportcount' FROM RM_CustomList rcl WHERE NAME= ?", Name);
                    cutomReportCount = (int)((System.Collections.Hashtable)(customListRes)[0])["reportcount"];
                    int returnflag = 0;
                    string strcon = ConfigurationSettings.AppSettings["conn"].ToString();
                    CustomListDao objdao = new CustomListDao();
                    objdao.Id = ID;
                    if (ID > 0)
                    {
                        if (objdao.Name != Name)
                            objdao.Name = Name;
                        if (objdao.Description != Description)
                            objdao.Description = Description;
                        if (objdao.XmlData != XmlData)
                            objdao.XmlData = XmlData;
                        string strSQLCommand = "";
                        using (SqlConnection sqlcon = new SqlConnection(strcon))
                        {
                            sqlcon.Open();
                            string strSQLdropCommand = "DROP VIEW CLV_" + Name;
                            SqlCommand dropcommand = new SqlCommand(strSQLdropCommand, sqlcon);
                            string returnvalue = (string)dropcommand.ExecuteScalar();
                            sqlcon.Close();
                            sqlcon.Open();
                            strSQLCommand = strSQLCommand + "     CREATE VIEW CLV_" + Name + " AS " + ValidatedQuery;
                            SqlCommand command = new SqlCommand(strSQLCommand, sqlcon);
                            string returnvaluecreate = (string)command.ExecuteScalar();
                            sqlcon.Close();
                        }
                        tx.PersistenceManager.ReportRepository.Save<CustomListDao>(objdao);
                        tx.Commit();
                        returnflag = objdao.Id;
                    }
                    else
                    {

                        if (cutomReportCount == 0)
                        {
                            objdao.Name = Name;
                            objdao.Description = Description;
                            objdao.XmlData = XmlData;
                            using (SqlConnection sqlcon = new SqlConnection(strcon))
                            {
                                //sqlcon.Open();
                                //string strSQLCommand = "CREATE VIEW CLV_" + Name + " AS " + ValidatedQuery;
                                //SqlCommand command = new SqlCommand(strSQLCommand, sqlcon);
                                //string returnvalue = (string)command.ExecuteScalar();
                                //sqlcon.Close();                                                   //viniston commented this. Check with Viniston
                                string strSQLCommand = "";
                                sqlcon.Open();
                                string strSQLdropCommand = " IF OBJECT_ID('CLV_" + Name + "', 'V') IS NOT NULL DROP VIEW CLV_" + Name + "";
                                SqlCommand dropcommand = new SqlCommand(strSQLdropCommand, sqlcon);
                                string returnvalue = (string)dropcommand.ExecuteScalar();
                                sqlcon.Close();
                                sqlcon.Open();
                                strSQLCommand = strSQLCommand + "     CREATE VIEW CLV_" + Name + " AS " + ValidatedQuery;
                                SqlCommand command = new SqlCommand(strSQLCommand, sqlcon);
                                string returnvaluecreate = (string)command.ExecuteScalar();
                                sqlcon.Close();
                            }
                            tx.PersistenceManager.ReportRepository.Save<CustomListDao>(objdao);
                            tx.Commit();
                            returnflag = objdao.Id;
                        }
                        else
                        {
                            returnflag = -1; // view already presented in the system.
                            tx.Commit();
                        }
                    }

                    return returnflag;
                }
            }
            catch (Exception ex)
            {
                return 0;
            }

        }



        public bool DeleteCustomList(ReportManagerProxy proxy, int ID)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {

                    tx.PersistenceManager.ReportRepository.DeleteByID<CustomListDao>(ID);
                    tx.Commit();
                    return true;

                }
            }
            catch
            {

            }
            return false;

        }


        public IList<ICustomList> GetAllCustomList(ReportManagerProxy proxy)
        {
            try
            {

                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    IList<ICustomList> clistdao = new List<ICustomList>();
                    var Collections = tx.PersistenceManager.ReportRepository.Query<CustomListDao>();
                    if (Collections != null)
                    {
                        foreach (var val in Collections)
                        {
                            clistdao.Add(new CustomList { Id = val.Id, Name = val.Name, Description = val.Description, XmlData = val.XmlData });
                        }
                        tx.Commit();
                        return clistdao;
                    }

                }

            }
            catch
            {

            }

            return null;


        }

        /* public string CustomList_Validate(ReportManagerProxy proxy, string Name, string xmldata, int ID)
         {

             try
             {



                 string stringToCheck = xmldata.ToUpper();
                 string[] stringArray = { "CREATE ", "DROP ", "TRUNCATE ", "ALTER ", "INSERT ", "UPDATE ", "DELETE ", };
                 IList listresult;
                 if (stringArray.Any(stringToCheck.Contains))
                 {
                     return "3";
                 }
                 if (ID == 0)
                 {
                     using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                     {
                         string Name1 = "CV_" + Name;
                         var s = (from tt in tx.PersistenceManager.ReportRepository.Query<CustomViewsDao>() where tt.Name == Name1 select tt).FirstOrDefault();
                         if (s != null)
                         {
                             if (s.Name.Length > 0)
                             {
                                 tx.Commit();
                                 return "1";
                             }
                         }

                     }

                 }

                 using (ITransaction tx1 = proxy.MarcomManager.GetTransaction())
                 {
                     listresult = tx1.PersistenceManager.ReportRepository.ExecuteQuery(xmldata.ToString());
                     tx1.Commit();
                 }

                 //bool schema = pushSchema(proxy);
                 return "2";

             }
             catch (Exception ex)
             {
                 return ex.InnerException.Message.ToString();
             }
             return "0";
         }*/
        public Tuple<string, string> CustomList_Validate(ReportManagerProxy proxy, string Name, string XmlData)
        {
            //bool atest = pushSchema(proxy);

            XmlData = XmlData.Replace("\"<\"", "\"&lt;\"");
            IList dynamicData = null;
            try
            {
                //Tuple<IList, string> cumstomlist_validate = null;
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {

                    //XmlData = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                    //                "<CustomList>" +
                    //                    "<EntityTypes>" +
                    //                        "<EntityType Id=\"61\" Caption=\"Marketing Plan\"/>" +
                    //                        "<EntityType Id=\"60\" Caption=\"Campaign\"/>" +
                    //                        "<EntityType Id=\"62\" Caption=\"POS\"/>" +
                    //                    "</EntityTypes>" +
                    //                    "<Attributes>" +
                    //                        "<Attribute Id=\"344\" Type=\"6\" Level=\"1\" Caption=\"Region country\" />" +
                    //                        "<Attribute Id=\"344\" Type=\"6\" Level=\"2\" Caption=\"Region City\" />" +
                    //                        "<Attribute Id=\"317\" Type=\"3\" Level=\"0\" Caption=\"Channel\" />" +
                    //                        "<Attribute Id=\"1\" Type=\"3\" Level=\"0\" Caption=\"Fiscal Year\" />" +
                    //                    "</Attributes>" +
                    //                    "<AdditionalInfos>" +
                    //                        "<AdditionalInfo Id=\"1\" Caption=\"Member\" />" +
                    //                        "<AdditionalInfo Id=\"2\" Caption=\"Financial\" />" +
                    //                        "<AdditionalInfo Id=\"3\" Caption=\"Task\" />" +
                    //                    "</AdditionalInfos>" +
                    //                    "<Criterias>" +
                    //                        "<Criteria Condition=\"0\" AttributeID=\"344\" AttributeTypeID=\"6\" AttributeLevel=\"1\" AttributeCaption=\"Region country\" Operator=\"IN\" Value=\"'Sweden','India'\" Options=\"Sweden@@@Sweden###India@@@India\"/>" +
                    //                        "<Criteria Condition=\"0\" AttributeID=\"344\" AttributeTypeID=\"6\" AttributeLevel=\"2\" AttributeCaption=\"Region City\" Operator=\"IN\" Value=\"'Gothunburg','Stockholm'\" Options=\"Gothunburg@@@Gothunburg###Stockholm@@@Stockholm###Delhi@@@Delhi###Bangalore@@@Bangalore\"/>" +
                    //                        "<Criteria Condition=\"0\" AttributeID=\"317\" AttributeTypeID=\"3\" AttributeLevel=\"0\" AttributeCaption=\"Channel\" Operator=\"NOT IN\" Value=\"'POS','Online'\" Options=\"POS@@@POS###TV@@@TV###Radio@@@Radio###Event@@@Event###Online@@@Online###Outdoor@@@Outdoor###Print Advertising@@@Print Advertising\"/>" +
                    //                        "<Criteria Condition=\"0\" AttributeID=\"1\" AttributeTypeID=\"3\" AttributeLevel=\"0\" AttributeCaption=\"Fiscal Year\" Operator=\"IN\" Value=\"'2011','2012','2013'\" Options=\"2011@@@2011###2012@@@2012###2013@@@2013###2014@@@2014###2015@@@2015###2016@@@2016###2017@@@2017###2018@@@2018\"/>" +
                    //                    "</Criterias>" +
                    //                "</CustomList>";

                    XDocument xd = XDocument.Parse(XmlData);
                    var DefaultReportsetting = xd.Descendants("CustomList").FirstOrDefault();
                    IList<XElement> entitytypes = xd.Descendants("CustomList").Descendants("EntityTypes").Descendants("EntityType").ToList();
                    IList<XElement> attributeLists = xd.Descendants("CustomList").Descendants("Attributes").Descendants("Attribute").ToList();
                    IList<XElement> criteriaLsts = xd.Descendants("CustomList").Descendants("Criterias").Descendants("Criteria").ToList();



                    int[] entitytypeIds = entitytypes.Distinct().Select(a => Convert.ToInt32(a.Attribute("Id").Value)).ToArray();


                    int[] attrsidarr = attributeLists.Distinct().Select(a => Convert.ToInt32(a.Attribute("Id").Value)).ToArray();

                    IList<AttributeDao> attributes = new List<AttributeDao>();
                    attributes = (from attrbs in tx.PersistenceManager.MetadataRepository.Query<AttributeDao>() where attrsidarr.Contains(attrbs.Id) select attrbs).ToList<AttributeDao>();


                    var attributerelationList = (from AdminAttributes in attributeLists
                                                 join ser in attributes on Convert.ToInt16(AdminAttributes.Attribute("Id").Value) equals ser.Id
                                                 select new
                                                 {
                                                     ID = Convert.ToInt16(AdminAttributes.Attribute("Id").Value),
                                                     Type = ser.AttributeTypeID,
                                                     IsSpecial = ser.IsSpecial,
                                                     Field = ser.Id,
                                                     Level = Convert.ToInt16(AdminAttributes.Attribute("Level").Value),
                                                     Caption = AdminAttributes.Attribute("Caption").Value,
                                                 }).Distinct().ToList();

                    var criteriaLists = (from AdminAttributes in criteriaLsts
                                         join ser in attributes on Convert.ToInt16(AdminAttributes.Attribute("AttributeID").Value) equals ser.Id
                                         select new
                                         {
                                             Condition = Convert.ToInt16(AdminAttributes.Attribute("Condition").Value),
                                             AttributeID = ser.Id,
                                             AttributeTypeID = Convert.ToInt16(AdminAttributes.Attribute("AttributeTypeID").Value),
                                             AttributeLevel = Convert.ToInt16(AdminAttributes.Attribute("AttributeLevel").Value),
                                             AttributeCaption = AdminAttributes.Attribute("AttributeCaption").Value,
                                             Operator = AdminAttributes.Attribute("Operator").Value,
                                             Value = AdminAttributes.Attribute("Value").Value
                                         }).Distinct().ToList();


                    IList<EntityTypeAttributeRelationDao> entityAttributes = new List<EntityTypeAttributeRelationDao>();
                    entityAttributes = (from attrbs in tx.PersistenceManager.MetadataRepository.Query<EntityTypeAttributeRelationDao>() where entitytypeIds.Contains(attrbs.EntityTypeID) select attrbs).ToList<EntityTypeAttributeRelationDao>();



                    StringBuilder mainResultQuery = new StringBuilder();
                    StringBuilder damQuery = new StringBuilder();
                    StringBuilder subdamQuery = new StringBuilder();

                    StringBuilder innerjoindamQuery = new StringBuilder();

                    int EntitypeLenghth = entitytypeIds.Distinct().Count();
                    int iMax = attributerelationList.Count();
                    int jMax = entitytypeIds.Length;

                    mainResultQuery.AppendLine("   SELECT tbl.ID, ");
                    for (int i = 0; i != iMax; i += 1)
                    {
                        mainResultQuery.AppendLine("    tbl.[" + attributerelationList[i].Caption + "]");
                        if (i < iMax - 1)
                            mainResultQuery.AppendLine(",");
                    }
                    mainResultQuery.AppendLine("   FROM   ( ");


                    for (int j = 0; j != jMax; j += 1)
                    {

                        subdamQuery = new StringBuilder();
                        innerjoindamQuery = new StringBuilder();

                        damQuery.AppendLine("   SELECT MM_AttributeRecord_" + entitytypeIds[j] + ".ID AS 'ID'");

                        for (int i = 0; i != iMax; i += 1)
                        {
                            int val = entityAttributes.ToList().Where(a => a.EntityTypeID == entitytypeIds[j] && a.AttributeID == attributerelationList[i].ID).Count();

                            if (attributerelationList[i].IsSpecial == true)
                            {
                                switch ((SystemDefinedAttributes)attributerelationList[i].ID)
                                {
                                    case SystemDefinedAttributes.Name:
                                        subdamQuery.AppendLine(",(SELECT pe.Name FROM PM_Entity pe WHERE pe.ID = [MM_AttributeRecord_" + entitytypeIds[j] + "].ID)  as '" + attributerelationList[i].Caption + "'");
                                        break;
                                    case SystemDefinedAttributes.Owner:
                                        subdamQuery.Append(",ISNULL( (SELECT top 1  ISNULL(us.FirstName,'') + ' ' + ISNULL(us.LastName,'')  FROM UM_User us INNER JOIN AM_Entity_Role_User aeru ON us.ID=aeru.UserID AND aeru.EntityID=[MM_AttributeRecord_" + entitytypeIds[j] + "].Id  INNER JOIN AM_EntityTypeRoleAcl aetra ON  aeru.RoleID = aetra.ID AND  aetra.EntityTypeID=pe.TypeID AND aetra.EntityRoleID = 1),'-') AS [" + attributerelationList[i].Caption + "]");
                                        break;
                                    case SystemDefinedAttributes.EntityStatus:
                                        subdamQuery.Append(", CASE WHEN pe.TypeID = " + (int)EntityTypeList.Objective + " THEN (SELECT case when ISNULL(po.ObjectiveStatus,0)=0 THEN 'Deactivated'  ELSE 'Active'  END from  PM_Objective po WHERE po.id=[MM_AttributeRecord_" + entitytypeIds[j] + "].Id) else isnull((SELECT  metso.StatusOptions FROM MM_EntityStatus mes INNER JOIN MM_EntityTypeStatus_Options metso ON mes.StatusID=metso.ID AND mes.EntityID=[MM_AttributeRecord_" + entitytypeIds[j] + "].id AND metso.IsRemoved=0),'-') end AS [" + attributerelationList[i].Caption + "]");
                                        break;
                                    case SystemDefinedAttributes.EntityOnTimeStatus:
                                        subdamQuery.Append(", CASE WHEN pe.TypeID = " + (int)EntityTypeList.Objective + " THEN (SELECT case when ISNULL(po.ObjectiveStatus,0)=0 THEN '-'  ELSE '-'  END from  PM_Objective po WHERE po.id=[MM_AttributeRecord_" + entitytypeIds[j] + "].Id) else isnull((SELECT CASE WHEN (SELECT mes.IntimeStatus FROM MM_EntityStatus mes WHERE mes.EntityID=[MM_AttributeRecord_" + entitytypeIds[j] + "].id) = 0 THEN 'On time' WHEN (SELECT mes.IntimeStatus FROM MM_EntityStatus mes WHERE mes.EntityID=[MM_AttributeRecord_" + entitytypeIds[j] + "].id) = 1 THEN 'Delayed' WHEN (SELECT mes.IntimeStatus FROM MM_EntityStatus mes WHERE mes.EntityID=[MM_AttributeRecord_" + entitytypeIds[j] + "].id) = 2 THEN 'On hold' ELSE 'On time' END AS ontimestatus), '-') END  AS [" + attributerelationList[i].Caption + "]");
                                        break;
                                }
                            }
                            else if ((AttributesList)attributerelationList[i].Type == AttributesList.ListMultiSelection || (AttributesList)attributerelationList[i].Type == AttributesList.ListSingleSelection || (AttributesList)attributerelationList[i].Type == AttributesList.DropDownTree || (AttributesList)attributerelationList[i].Type == AttributesList.Tree || (AttributesList)attributerelationList[i].Type == AttributesList.Period || (AttributesList)attributerelationList[i].Type == AttributesList.TreeMultiSelection)
                            {

                                switch ((AttributesList)attributerelationList[i].Type)
                                {
                                    case AttributesList.ListSingleSelection:

                                        if (val > 0)
                                        {
                                            subdamQuery.AppendLine(" ,ISNULL( ");
                                            subdamQuery.AppendLine("( ");
                                            subdamQuery.AppendLine(" SELECT TOP 1 caption ");
                                            subdamQuery.AppendLine(" FROM   MM_Option ");
                                            subdamQuery.AppendLine("WHERE  AttributeID = " + attributerelationList[i].ID + " ");
                                            subdamQuery.AppendLine("  AND id = [MM_AttributeRecord_" + entitytypeIds[j] + "].ID ");
                                            subdamQuery.AppendLine("  ), ");
                                            subdamQuery.AppendLine("    NULL )");
                                        }
                                        else
                                            subdamQuery.AppendLine(",  NULL ");
                                        subdamQuery.AppendLine("AS [" + attributerelationList[i].Caption + "] ");

                                        break;
                                    case AttributesList.ListMultiSelection:

                                        if (val > 0)
                                        {
                                            subdamQuery.AppendLine(" , ( ");
                                            subdamQuery.AppendLine("  SELECT ISNULL(caption, '-') ");
                                            subdamQuery.AppendLine("  FROM   MM_Option ");
                                            subdamQuery.AppendLine("  WHERE  id = mms" + entitytypeIds[j] + "" + i + ".OptionID ");
                                            subdamQuery.AppendLine("   ) ");

                                            innerjoindamQuery.AppendLine("  INNER JOIN MM_MultiSelect  mms" + entitytypeIds[j] + "" + i + " ");
                                            innerjoindamQuery.AppendLine("  ON   mms" + entitytypeIds[j] + "" + i + ".EntityID = [MM_AttributeRecord_" + entitytypeIds[j] + "].ID ");
                                            innerjoindamQuery.AppendLine("  AND  mms" + entitytypeIds[j] + "" + i + ".AttributeID =  " + attributerelationList[i].ID + "");
                                        }
                                        else
                                            subdamQuery.AppendLine(",  NULL ");
                                        subdamQuery.AppendLine("AS [" + attributerelationList[i].Caption + "] ");

                                        break;
                                    case AttributesList.DropDownTree:
                                    case AttributesList.TreeMultiSelection:

                                        if (val > 0)
                                        {
                                            subdamQuery.AppendLine(" , ( ");
                                            subdamQuery.AppendLine("  SELECT ISNULL(Caption, '-') ");
                                            subdamQuery.AppendLine("  FROM   MM_TreeNode ");
                                            subdamQuery.AppendLine("  WHERE  ID = mms" + entitytypeIds[j] + "" + i + ".NodeID ");
                                            subdamQuery.AppendLine("   ) ");

                                            innerjoindamQuery.AppendLine("  INNER JOIN MM_TreeValue  mms" + entitytypeIds[j] + "" + i + " ");
                                            innerjoindamQuery.AppendLine("  ON   mms" + entitytypeIds[j] + "" + i + ".EntityID = [MM_AttributeRecord_" + entitytypeIds[j] + "].ID ");
                                            innerjoindamQuery.AppendLine("  AND  mms" + entitytypeIds[j] + "" + i + ".AttributeID =  " + attributerelationList[i].ID + "");
                                            innerjoindamQuery.AppendLine("  AND  mms" + entitytypeIds[j] + "" + i + ".Level =  " + attributerelationList[i].Level + "");
                                        }
                                        else
                                            subdamQuery.AppendLine(",  NULL ");
                                        subdamQuery.AppendLine("AS [" + attributerelationList[i].Caption + "] ");

                                        break;
                                    case AttributesList.Period:
                                        if (val > 0)
                                        {
                                            if (attributerelationList[i].Level == 0)
                                                subdamQuery.Append(",( SELECT TOP 1 isnull(MIN( CONVERT(NVARCHAR(10), pep.StartDate, 120)),NULL) AS 'StartDate' FROM PM_EntityPeriod pep WHERE pep.EntityID= [MM_AttributeRecord_" + entitytypeIds[j] + "].ID ) AS [" + attributerelationList[i].Caption + "]");
                                            if (attributerelationList[i].Level == -1)
                                                subdamQuery.Append(",( SELECT TOP 1 isnull(MAX( CONVERT(NVARCHAR(10), pep.EndDate, 120)),NULL) AS 'EndDate' FROM PM_EntityPeriod pep WHERE pep.EntityID= [MM_AttributeRecord_" + entitytypeIds[j] + "].ID ) AS [" + attributerelationList[i].Caption + "]");

                                        }
                                        else
                                        {
                                            subdamQuery.AppendLine(",  NULL ");
                                            subdamQuery.AppendLine("AS [" + attributerelationList[i].Caption + "] ");
                                        }
                                        break;
                                    case AttributesList.Tree:
                                        subdamQuery.Append(" ,'IsTree' AS [" + attributerelationList[i].Caption + "] ");
                                        break;
                                }
                            }
                            else if ((AttributesList)attributerelationList[i].Type == AttributesList.CheckBoxSelection)
                            {
                                subdamQuery.Append(" ,isnull(cast([MM_AttributeRecord_" + entitytypeIds[j] + "].attr_" + attributerelationList[i].ID + " as varchar(50)), NULL) AS [" + attributerelationList[i].Caption + "]");
                            }
                            else if ((AttributesList)attributerelationList[i].Type == AttributesList.DateTime)
                            {
                                subdamQuery.Append(" ,REPLACE(CONVERT(varchar,isnull([MM_AttributeRecord_" + entitytypeIds[j] + "].attr_" + attributerelationList[i].ID + " ,''),121),'1900-01-01 00:00:00.000', NULL) AS [" + attributerelationList[i].Caption + "]");
                            }
                            else if ((AttributesList)attributerelationList[i].Type == AttributesList.ParentEntityName)
                            {
                                subdamQuery.Append(" ,isnull((SELECT top 1 pe2.name  + '!@#' + met.ShortDescription + '!@#' + met.ColorCode FROM PM_Entity pe2 INNER JOIN MM_EntityType met ON pe2.TypeID=met.ID  WHERE  pe2.id=pe.parentid), NULL) AS [" + attributerelationList[i].Caption + "]");
                            }
                            else
                            {
                                subdamQuery.Append(" ,isnull([MM_AttributeRecord_" + entitytypeIds[j] + "].attr_" + attributerelationList[i].ID + " , NULL) AS [" + attributerelationList[i].Caption + "]");
                            }

                        }
                        damQuery.AppendLine(subdamQuery.ToString());
                        damQuery.AppendLine(" FROM   [MM_AttributeRecord_" + entitytypeIds[j] + "] ");
                        damQuery.AppendLine(innerjoindamQuery.ToString());
                        if (j < jMax - 1)
                            damQuery.AppendLine("UNION ALL");
                    }

                    mainResultQuery.AppendLine(damQuery.ToString());
                    mainResultQuery.AppendLine("  ) AS tbl");
                    mainResultQuery.AppendLine("   INNER JOIN PM_Entity pe");
                    mainResultQuery.AppendLine("  ON  pe.ID = tbl.ID");

                    //Criteria will come
                    int kMax = criteriaLists.Count();

                    Boolean BraketStart = false;

                    if (kMax > 0)
                    {
                        mainResultQuery.AppendLine(" WHERE (");
                    }

                    for (int k = 0; k != kMax; k += 1)
                    {



                        bool IsAnd = false;
                        if (k > 0)
                        {

                            if (criteriaLists[k].Condition == 1)
                            {
                                mainResultQuery.AppendLine("     OR");
                                if (kMax - 1 > k)
                                {
                                    if (criteriaLists[k].Condition == 0)
                                    {
                                        mainResultQuery.AppendLine("  (");
                                        BraketStart = true;
                                    }
                                }

                            }
                            else
                            {
                                mainResultQuery.AppendLine(" AND");
                                if (kMax - 1 > k)
                                {
                                    if (criteriaLists[k].Condition == 1)
                                    {
                                        IsAnd = true;
                                    }

                                }
                                else
                                {
                                    IsAnd = true;
                                }

                            }

                        }
                        else
                        {
                            if (kMax - 1 > k)
                            {
                                if (criteriaLists[k].Condition == 0)
                                {
                                    mainResultQuery.AppendLine(" (");
                                    BraketStart = true;
                                }
                            }

                        }

                        mainResultQuery.AppendLine("tbl.[" + criteriaLists[k].AttributeCaption + "] ");
                        var Operator = criteriaLists[k].Operator;


                        switch (Operator)
                        {
                            case "IN":
                            case "NOT IN":

                                mainResultQuery.AppendLine(criteriaLists[k].Operator + "(");

                                mainResultQuery.AppendLine(criteriaLists[k].Value);

                                mainResultQuery.AppendLine(") ");
                                break;
                            case "<":
                            case ">":
                            case "=":

                                mainResultQuery.AppendLine(criteriaLists[k].Operator + " ");

                                mainResultQuery.AppendLine("'" + criteriaLists[k].Value + "' ");


                                break;
                            case "LIKE":

                                mainResultQuery.AppendLine(criteriaLists[k].Operator + " ");

                                mainResultQuery.AppendLine("'" + criteriaLists[k].Value + "%' ");


                                break;
                            default:
                                break;
                        }
                        if (IsAnd && BraketStart)
                        {
                            mainResultQuery.AppendLine(" )");
                            BraketStart = false;
                        }
                    }
                    if (kMax > 0)
                    {
                        mainResultQuery.Append("  )");
                    }

                    dynamicData = tx.PersistenceManager.ReportRepository.ExecuteQuery(mainResultQuery.ToString());
                    Tuple<string, string> cumstomlist_validate = Tuple.Create(mainResultQuery.ToString(), "0");
                    tx.Commit();
                    return cumstomlist_validate;
                }
                return Tuple.Create("", "1");
            }
            catch (Exception ex)
            {
                return Tuple.Create("", "-1");
            }
        }


        public IList<IOption> GetFulfillmentAttributeOptions(ReportManagerProxy proxy, int attributeId, int attributeLevel = 0)
        {
            IList<IOption> listAttributeoptions = new List<IOption>();
            string xmlPath = string.Empty;
            int versionNumber = MarcomManagerFactory.ActiveMetadataVersionNumber;
            using (ITransaction tx = proxy.MarcomManager.GetTransaction())
            {
                xmlPath = tx.PersistenceManager.MetadataRepository.GetXmlPath(versionNumber);

                //IList<AttributeDao> attributeDao = new List<AttributeDao>();
                IList<OptionDao> attributesOptionsDao = new List<OptionDao>();
                if (versionNumber != 0)
                {
                    var optionXmlResult = tx.PersistenceManager.MetadataRepository.GetObject<AttributeDao>(xmlPath).Join
                        (tx.PersistenceManager.MetadataRepository.GetObject<OptionDao>(xmlPath),
                        atr => atr.Id, opt => opt.AttributeID, (atr, opt) => new { atr, opt }).Where(a => a.atr.Id == attributeId).Select(a => a.opt);
                    if (attributeLevel != 0)
                    {
                        var treeXmlNodeResult = tx.PersistenceManager.MetadataRepository.GetObject<TreeNodeDao>(xmlPath).Where
                            (a => (a.Level == attributeLevel) && a.AttributeID == attributeId).ToList();
                        foreach (var nodeObj in treeXmlNodeResult)
                        {
                            Option fullfillattributeOptionObj = new Option();
                            fullfillattributeOptionObj.Id = nodeObj.Id;
                            fullfillattributeOptionObj.Caption = nodeObj.Caption;
                            fullfillattributeOptionObj.AttributeID = attributeId;
                            listAttributeoptions.Add(fullfillattributeOptionObj);
                        }
                    }
                    else
                    {
                        foreach (var obj in optionXmlResult)
                        {
                            Option fullfillattributeOptionObj = new Option();
                            fullfillattributeOptionObj.Id = obj.Id;
                            fullfillattributeOptionObj.Caption = obj.Caption;
                            fullfillattributeOptionObj.AttributeID = obj.AttributeID;
                            listAttributeoptions.Add(fullfillattributeOptionObj);
                        }
                    }
                }
                else
                {


                    if (attributeLevel != 0)
                    {
                        var attributeDao = tx.PersistenceManager.MetadataRepository.Query<AttributeDao>().Where(a => a.Id == attributeId).FirstOrDefault();
                        IList<TreeNodeDao> treeNodeResult = new List<TreeNodeDao>();

                        if ((AttributesList)attributeDao.AttributeTypeID != AttributesList.Tree)
                            treeNodeResult = tx.PersistenceManager.PlanningRepository.Query<TreeNodeDao>().Where(a => a.Level == attributeLevel && a.AttributeID == attributeId).Cast<TreeNodeDao>().ToList();
                        else
                            treeNodeResult = tx.PersistenceManager.PlanningRepository.Query<TreeNodeDao>().Where(a => a.AttributeID == attributeId).Cast<TreeNodeDao>().ToList();
                        foreach (var nodeObj in treeNodeResult)
                        {
                            Option fullfillattributeOptionObj = new Option();
                            fullfillattributeOptionObj.Id = nodeObj.NodeID;
                            fullfillattributeOptionObj.Caption = AddWhiteSpace(nodeObj.Level) + nodeObj.Caption;
                            fullfillattributeOptionObj.AttributeID = attributeId;
                            listAttributeoptions.Add(fullfillattributeOptionObj);
                        }
                    }
                    else
                    {
                        var attributeDao = tx.PersistenceManager.MetadataRepository.Query<AttributeDao>().Where(a => a.Id == attributeId).ToList();
                        attributesOptionsDao = tx.PersistenceManager.MetadataRepository.GetAll<OptionDao>().Join(attributeDao, a => a.AttributeID, b => b.Id, (ab, bc) =>
                            new { ab, bc }).Where(a => a.ab.AttributeID == attributeId).Select(a => a.ab).ToList();
                        tx.Commit();
                        foreach (var obj in attributesOptionsDao)
                        {
                            Option fullfillattributeOptionObj = new Option();
                            fullfillattributeOptionObj.Id = obj.Id;
                            fullfillattributeOptionObj.Caption = obj.Caption;
                            fullfillattributeOptionObj.AttributeID = obj.AttributeID;
                            listAttributeoptions.Add(fullfillattributeOptionObj);
                        }
                    }
                }
                tx.Commit();
                return listAttributeoptions;
            }
        }

        public string AddWhiteSpace(int level)
        {
            string space = "";
            for (int i = 0; i < level; i++)
            {
                space += " ";
            }
            return space;
        }


        public bool insertupdatetabsettings(ReportManagerProxy proxy, int tabtype, int tablocation, JObject jsonXML)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    string directoryname = tablocation < 2 ? "Plans" : (tablocation == 2 ? "CostCentre" : "Objectives"),
                      filename = "", foldername = "";
                    Pathname(tablocation, tabtype, ref filename, ref foldername);
                    string mappingfilesPath = AppDomain.CurrentDomain.BaseDirectory;
                    string folderstructure = @"\" + directoryname + @"\" + filename + @"\" + foldername + ".xml";
                    mappingfilesPath = mappingfilesPath + "Layouts" + folderstructure;
                    string strJsonXML = JsonConvert.SerializeObject(jsonXML);
                    XmlDocument newdocContent = JsonConvert.DeserializeXmlNode(strJsonXML);
                    XmlDocument doc1 = new XmlDocument();
                    doc1 = newdocContent;
                    doc1.Save(mappingfilesPath);

                    return true;
                }

            }

            catch
            {
                return false;
            }
        }


        private void Pathname(int locationplace, int tabtype, ref string filename, ref string foldername)
        {
            if (locationplace == 1)
            {
                filename = tabtype < 2 ? "Overview" : (tabtype == 2 ? "Financial" : "Objective");
                foldername = tabtype < 2 ? "plan_overview" : (tabtype == 2 ? "plan_financial" : "plan_objective");
            };
            if (locationplace == 2)
            {
                filename = tabtype < 2 ? "Overview" : (tabtype == 2 ? "Financial" : "Objective");
                foldername = tabtype < 2 ? "costcentre_overview" : (tabtype == 2 ? "" : "");
            };
            if (locationplace == 3)
            {
                filename = tabtype < 2 ? "Overview" : (tabtype == 2 ? "Financial" : "Objective");
                foldername = tabtype < 2 ? "objectives_overview" : (tabtype == 2 ? "" : "");
            };

        }

        public string GetLayoutData(ReportManagerProxy proxy, int tabtype, int tablocation)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    string directoryname = tablocation < 2 ? "Plans" : (tablocation == 2 ? "CostCentre" : "Objectives"),
                      filename = "", foldername = "";

                    Pathname(tablocation, tabtype, ref filename, ref foldername);

                    string mappingfilesPath = AppDomain.CurrentDomain.BaseDirectory;
                    string folderstructure = @"\" + directoryname + @"\" + filename + @"\" + foldername + ".xml";
                    mappingfilesPath = mappingfilesPath + "Layouts" + folderstructure;
                    System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                    doc.Load(mappingfilesPath);
                    string sbJSON = string.Empty;
                    sbJSON = tx.PersistenceManager.ReportRepository.XmlToJSON(doc);
                    tx.Commit();
                    return sbJSON;
                }

            }
            catch
            {
                throw null;
            }
        }



    }
}
