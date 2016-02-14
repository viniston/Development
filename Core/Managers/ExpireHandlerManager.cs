using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.IO;
using BrandSystems.Marcom.Core.com.proofhq.www;
using BrandSystems.Marcom.Core.Interface;
using BrandSystems.Marcom.Core.Metadata;
using BrandSystems.Marcom.Core.Metadata.Interface;
using BrandSystems.Marcom.Dal.Metadata.Model;
using BrandSystems.Marcom.Dal.Base;
using System.Collections;
using System.Web;
using NHibernate.Mapping;
using System.Xml.Linq;
using System.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using BrandSystems.Marcom.Dal.Planning.Model;
using BrandSystems.Marcom.Dal.User.Model;
using BrandSystems.Marcom.Core.Planning;
using BrandSystems.Marcom.Core.Planning.Interface;
using BrandSystems.Marcom.Dal.Access.Model;
using BrandSystems.Marcom.Core.Core.Managers.Proxy;
using BrandSystems.Marcom.Core.Dam.Interface;
using BrandSystems.Marcom.Core.Dam;
using BrandSystems.Marcom.Core.DAM.Interface;
using BrandSystems.Marcom.Dal.DAM.Model;
using BrandSystems.Marcom.Core.DAM;
using BrandSystems.Marcom.DAM;
using System.IO.Compression;
using ICSharpCode.SharpZipLib.Zip;
using BrandSystems.Marcom.Core.Common;
using System.Net.Mail;
using BrandSystems.Marcom.Dal.Task.Model;
using BrandSystems.Marcom.Dal.Common.Model;
using Microsoft.Win32;
using System.Drawing.Drawing2D;
using SD = System.Drawing;
using BrandSystems.Marcom.Core.Utility;
using System.Net;
using BrandSystems.Marcom.Dal.ExpireHandler.Model;
using BrandSystems.Marcom.Core.Managers.Proxy;
using BrandSystems.Marcom.Core.ExpireHandler.Interface;
using BrandSystems.Marcom.Core.ExpireHandler;
using System.Globalization;


namespace BrandSystems.Marcom.Core.Managers
{
    internal partial class ExpireHandlerManager : IManager
    {
        void IManager.Initialize(IMarcomManager marcomManager)
        {
            //throw new System.NotImplementedException();
        }

        void IManager.CommitCaches()
        {
            // throw new System.NotImplementedException();
        }

        void IManager.RollbackCaches()
        {
            //throw new System.NotImplementedException();
        }

        private static ExpireHandlerManager instance = new ExpireHandlerManager();
        internal static ExpireHandlerManager Instance
        {
            get { return instance; }
        }
        /// <summary>
        /// The working metadata
        /// </summary>
        string currentworkingMetadata = Path.Combine(HttpRuntime.AppDomainAppPath, ConfigurationManager.AppSettings["currentworkingMetadata"]);
        /// <summary>
        /// The history metadata
        /// </summary>
        string currentSyncDBXML = Path.Combine(HttpRuntime.AppDomainAppPath, ConfigurationManager.AppSettings["currentSyncDBXML"]);

        /// <summary>
        /// The version of metadata
        /// </summary>
        string versionMetadata = Path.Combine(HttpRuntime.AppDomainAppPath, ConfigurationManager.AppSettings["versionMetadata"]);

        //string currentMetadata = Path.Combine(HttpRuntime.AppDomainAppPath, ConfigurationManager.AppSettings["CurrentMetadata"]);

        public int CreateExpireAction(ExpireHandlerManagerProxy proxy, int ActionID, int SourceID, int SourceEnityID, int SourceFrom, string Actionexutedays, string DateActionexpiredate, bool Actionexute, bool ispublish,int ActionsourceId, IList<IAttributeData> listattributevalues)
        {
            int version = MarcomManagerFactory.ActiveMetadataVersionNumber;

            int ActionSourceID = 0;
            int OwnerID = 0;
        
            try
            {
                Guid NewId = Guid.NewGuid();
                OwnerID = Convert.ToInt32(proxy.MarcomManager.User.Id);
               


                BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("-----------------------------------------------------------------------------------------------", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);
                BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("Started CreateExpireAction", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);

                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {

                    if (ActionsourceId > 0)//update Exiting expireAction
                    {
                        ExpireHandlerActionSourceDao Expiresourcedao = new ExpireHandlerActionSourceDao();
                        ExpireHandlerActionSourceDataDao ExpiresourceDatadao1 = new ExpireHandlerActionSourceDataDao();
                        Expiresourcedao = new ExpireHandlerActionSourceDao();
                        Expiresourcedao = (from tt in tx.PersistenceManager.ExpireHandlerRepository.Query<ExpireHandlerActionSourceDao>() where tt.Id == ActionsourceId select tt).Select(a => a).FirstOrDefault();
                        ExpiresourceDatadao1 = (from tt in tx.PersistenceManager.ExpireHandlerRepository.Query<ExpireHandlerActionSourceDataDao>() where tt.ActionSourceID== ActionsourceId select tt).Select(a => a).FirstOrDefault();
                        if (Expiresourcedao != null)
                        {
                            string exActionexutedays = (Actionexutedays.ToString() != "-1") ? Actionexutedays.ToString() : Expiresourcedao.Actionexutedays;
                            Expiresourcedao.Actionexutedays = exActionexutedays;
                            Expiresourcedao.Actionexutedate = getorginalexpirtime(DateActionexpiredate, exActionexutedays);
                            tx.PersistenceManager.ExpireHandlerRepository.Save<ExpireHandlerActionSourceDao>(Expiresourcedao);                            

                        }
                        if (ExpiresourceDatadao1 != null)
                        {
                            IList<ExpireHandlerActionSourceDataDao> ExpireHandlerSourceDataAction = new List<ExpireHandlerActionSourceDataDao>();
                            ExpireHandlerActionSourceDataDao ExpiresourceDatadao = new ExpireHandlerActionSourceDataDao();
                            if (listattributevalues != null)
                            {
                                foreach (var val in listattributevalues)
                                {
                                    ExpiresourceDatadao = new ExpireHandlerActionSourceDataDao();
                                    ExpiresourceDatadao.ActionSourceID = ActionSourceID;
                                    ExpiresourceDatadao.AttributeID = val.ID;
                                    ExpiresourceDatadao.AttributeTypeID = val.TypeID;
                                    ExpiresourceDatadao.AttributeCaption = val.Caption;
                                    ExpiresourceDatadao.NodeID = val.Value;
                                    ExpiresourceDatadao.Level = val.Level;
                                    ExpiresourceDatadao.Ispublish = ispublish;
                                    ExpireHandlerSourceDataAction.Add(ExpiresourceDatadao);
                                   
                                }
                                tx.PersistenceManager.ExpireHandlerRepository.Save<ExpireHandlerActionSourceDataDao>(ExpiresourceDatadao);
                            }
                            else
                            {
                                ExpiresourceDatadao = new ExpireHandlerActionSourceDataDao();
                                ExpiresourceDatadao = (from tt in tx.PersistenceManager.ExpireHandlerRepository.Query<ExpireHandlerActionSourceDataDao>() where tt.Id == ExpiresourceDatadao1.Id select tt).Select(a => a).FirstOrDefault();
                                ExpiresourceDatadao.Ispublish = ispublish;
                                tx.PersistenceManager.ExpireHandlerRepository.Save<ExpireHandlerActionSourceDataDao>(ExpiresourceDatadao);

                            }

                        }
                        tx.Commit();
                        ActionSourceID = ActionsourceId;
                        return ActionSourceID;

                    }
                    else
                    {

                        ExpireHandlerActionSourceDao Expiresourcedao = new ExpireHandlerActionSourceDao();
                        Expiresourcedao.ActionID = ActionID;
                        Expiresourcedao.SourceID = SourceID;
                        Expiresourcedao.SourceEnityID = SourceEnityID;
                        Expiresourcedao.SourceFrom = SourceFrom;
                        Expiresourcedao.Actionexutedays = Actionexutedays.ToString();
                        Expiresourcedao.Actionexutedate = getorginalexpirtime(DateActionexpiredate, Actionexutedays);
                        //Expiresourcedao.Actionexute = Actionexute;
                        Expiresourcedao.ActionownerId = OwnerID;
                        tx.PersistenceManager.ExpireHandlerRepository.Save<ExpireHandlerActionSourceDao>(Expiresourcedao);
                        ActionSourceID = Expiresourcedao.Id;



                        if (listattributevalues != null)
                        {
                            foreach (var val in listattributevalues)
                            {
                                ExpireHandlerActionSourceDataDao ExpiresourceDatadao = new ExpireHandlerActionSourceDataDao();
                                ExpiresourceDatadao.ActionSourceID = ActionSourceID;
                                ExpiresourceDatadao.AttributeID = val.ID;
                                ExpiresourceDatadao.AttributeTypeID = val.TypeID;
                                ExpiresourceDatadao.AttributeCaption = val.Caption;
                                ExpiresourceDatadao.NodeID = val.Value;
                                ExpiresourceDatadao.Level = val.Level;
                                ExpiresourceDatadao.Ispublish = ispublish;
                                tx.PersistenceManager.ExpireHandlerRepository.Save<ExpireHandlerActionSourceDataDao>(ExpiresourceDatadao);
                            }
                        }
                        else
                        {
                            ExpireHandlerActionSourceDataDao ExpiresourceDatadao = new ExpireHandlerActionSourceDataDao();
                            ExpiresourceDatadao.ActionSourceID = ActionSourceID;
                            //ExpiresourceDatadao.AttributeID = 0;
                            //ExpiresourceDatadao.AttributeTypeID = 0;
                            //ExpiresourceDatadao.AttributeCaption = "-";
                            //ExpiresourceDatadao.NodeID = "-";
                            //ExpiresourceDatadao.Level = 0;
                            ExpiresourceDatadao.Ispublish = ispublish;
                            tx.PersistenceManager.ExpireHandlerRepository.Save<ExpireHandlerActionSourceDataDao>(ExpiresourceDatadao);

                        }
                        tx.Commit();

                        BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("CreateExpireAction is saved in EH_ActionSource with ActionSourceID : " + ActionSourceID, BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);
                    }
                }
            }
            catch (Exception ex)
            {
                BrandSystems.Marcom.Core.Metadata.LogHandler.LogError("Failed to EH_ActionSource", ex);
                BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("-----------------------------------------------------------------------------------------------", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);
                return 0;
            }

            return ActionSourceID;
        }


        public bool UpdateExpireActionDate(ExpireHandlerManagerProxy proxy, int SourceID, string DateActionexpiredate, int SourcetypeID, int ActionID, string Actionexutedays)
        {
            int version = MarcomManagerFactory.ActiveMetadataVersionNumber;


            int OwnerID = 0;

            try
            {
                Guid NewId = Guid.NewGuid();
                OwnerID = Convert.ToInt32(proxy.MarcomManager.User.Id);



                BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("-----------------------------------------------------------------------------------------------", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);
                BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("Started CreateExpireAction", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);

                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    IList<ExpireHandlerActionSourceDao> ExpireHandlerAction = new List<ExpireHandlerActionSourceDao>();
                    var actionrows = (from tt in tx.PersistenceManager.ExpireHandlerRepository.Query<ExpireHandlerActionSourceDao>() where tt.SourceID == SourceID && tt.Actionexute == false select tt).Select(a => a).ToList();
                    ExpireHandlerActionSourceDao Expiresourcedao = new ExpireHandlerActionSourceDao();
                    if (actionrows != null)
                    {
                        foreach (var val in actionrows)
                        {
                            Expiresourcedao = new ExpireHandlerActionSourceDao();
                            Expiresourcedao = (from tt in tx.PersistenceManager.ExpireHandlerRepository.Query<ExpireHandlerActionSourceDao>() where tt.Id == val.Id select tt).Select(a => a).FirstOrDefault();
                            string exActionexutedays = (Actionexutedays.ToString() != "-1") ? Actionexutedays.ToString() : val.Actionexutedays;
                            Expiresourcedao.Actionexutedays = exActionexutedays;
                            Expiresourcedao.Actionexutedate = getorginalexpirtime(DateActionexpiredate, exActionexutedays);
                            ExpireHandlerAction.Add(Expiresourcedao);
                            //ActionSourceID = Expiresourcedao.Id;

                        }
                        tx.PersistenceManager.ExpireHandlerRepository.Save<ExpireHandlerActionSourceDao>(ExpireHandlerAction);
                        //tx.Commit();
                    }
                    if (SourcetypeID > 0)
                    {
                        DateTime MyDateTime1 = getorginalexpirtime(DateActionexpiredate, "-1");

                        string updateCostcentreQuery = "UPDATE MM_AttributeRecord_" + SourcetypeID + " SET Attr_" + Convert.ToInt32(SystemDefinedAttributes.Dateaction) + " ='" + MyDateTime1.ToString()+ "' WHERE ID=" + SourceID + "";
                        tx.PersistenceManager.ExpireHandlerRepository.ExecuteQuery(updateCostcentreQuery);
                    }
                    tx.Commit();





                    //BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("CreateExpireAction is saved in EH_ActionSource with ActionSourceID : " + ActionSourceID, BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);

                }
            }
            catch (Exception ex)
            {
                BrandSystems.Marcom.Core.Metadata.LogHandler.LogError("Failed to EH_ActionSource", ex);
                BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("-----------------------------------------------------------------------------------------------", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);
                return false;
            }

            return true;
        }

        public DateTime getorginalexpirtime(string Expiredate, string Expireoptions)
        {
            int selectedoption;
            string[] formats = {"M/d/yyyy h:mm:ss tt", "M/d/yyyy h:mm tt", 
                                           "MM/dd/yyyy hh:mm:ss", "M/d/yyyy h:mm:ss", 
                                           "M/d/yyyy hh:mm tt", "M/d/yyyy hh tt", 
                                            "M/d/yyyy h:mm", "M/d/yyyy h:mm", 
                                             "MM/dd/yyyy hh:mm", "M/dd/yyyy hh:mm",
                                             "M/d/yyyy", "d/M/yyyy", "M-d-yyyy",
                                            "d-M-yyyy", "d-MMM-yy", "d-MMMM-yyyy",
                                            "dd/MM/yyyy","dd-MM-yyyy",
                                             };
             DateTime result;
                     DateTime MyDateTime;
                    DateTime.TryParseExact(Expiredate, formats, System.Globalization.CultureInfo.InvariantCulture, DateTimeStyles.None, out MyDateTime);
                    selectedoption = Convert.ToInt32(Expireoptions);
            switch (selectedoption)
            {
                case 1:
                    result = MyDateTime.AddDays(+1);                    
                    break;
                case 2:
                    result = MyDateTime.AddDays(+2);                    
                    break;
                case 3:
                    result = MyDateTime.AddDays(+3);                    
                    break;
                case 4:
                    result = MyDateTime.AddDays(+4);                    
                    break;
                case 5:
                    result = MyDateTime.AddDays(+5);                    
                    break;
                case 6:
                    result = MyDateTime.AddDays(+6);                    
                    break;
                case 7:
                    result = MyDateTime.AddDays(+7);                    
                    break;
                case 8:
                    result = MyDateTime.AddDays(+14);                    
                    break;
                case 9:
                    result = MyDateTime.AddDays(+21);                    
                    break;
                case 10:
                    result = MyDateTime.AddMonths(+1);                    
                    break;
                case 11:
                    result = MyDateTime.AddMonths(+2);                    
                    break;
                case 12:
                    result = MyDateTime.AddMonths(+3);                    
                    break;
                case 13:
                    result = MyDateTime.AddMonths(+4);                    
                    break;
                case 14:
                    result = MyDateTime.AddMonths(+5);                    
                    break;
                case 15:
                    result = MyDateTime.AddMonths(+6);                    
                    break;
                case 16:
                    result = MyDateTime.AddMonths(+7);                    
                    break;
                case 17:
                    result = MyDateTime.AddMonths(+8);                    
                    break;
                case 18:
                    result = MyDateTime.AddMonths(+9);                    
                    break;
                case 19:
                    result = MyDateTime.AddMonths(+10);                    
                    break;
                case 20:
                    result = MyDateTime.AddMonths(+11);                    
                    break;
                case 21:
                    result = MyDateTime.AddMonths(+12);                    
                    break;
                case 22:
                    result = MyDateTime.AddDays(-1);                   
                    break;
                case 23:
                    result = MyDateTime.AddDays(-2);                    
                    break;
                case 24:
                    result = MyDateTime.AddDays(-3);                    
                    break;
                case 25:
                    result = MyDateTime.AddDays(-4);                    
                    break;
                case 26:
                    result = MyDateTime.AddDays(-5);                    
                    break;
                case 27:
                    result = MyDateTime.AddDays(-6);                    
                    break;
                case 28:
                    result = MyDateTime.AddDays(-7);                    
                    break;
                case 29:
                    result = MyDateTime.AddDays(-14);
                    
                    break;
                case 30:
                    result = MyDateTime.AddDays(-21);
                    
                    break;
                case 31:
                    result = MyDateTime.AddMonths(-1);
                    
                    break;
                case 32:
                    result = MyDateTime.AddMonths(-2);
                    
                    break;
                case 33:
                    result = MyDateTime.AddMonths(-3);                    
                    break;
                case 34:
                    result = MyDateTime.AddMonths(-4);
                    
                    break;
                case 35:
                    result = MyDateTime.AddMonths(-5);                    
                    break;
                case 36:
                    result = MyDateTime.AddMonths(-6);                    
                    break;
                case 37:
                    result = MyDateTime.AddMonths(-7);                    
                    break;
                case 38:
                    result = MyDateTime.AddMonths(-8);                    
                    break;
                case 39:
                    result = MyDateTime.AddMonths(-9);                    
                    break;
                case 40:
                    result = MyDateTime.AddMonths(-10);                    
                    break;
                case 41:
                    result = MyDateTime.AddMonths(-11);                    
                    break;
                case 42:
                    result = MyDateTime.AddMonths(-12);                    
                    break;
                default:
                   result=MyDateTime;
                    break;
            }

            return result;
        }
        public IList<IExpireActionSources> GetExpireActions(ExpireHandlerManagerProxy proxy, int SourceID, int ActionID)
        {
            int version = MarcomManagerFactory.ActiveMetadataVersionNumber;
            
            var ActionSelectQuery = new StringBuilder();
          
            int ActionSourceID = 0;
            IList<IExpireActionSources> iiActionSourcelist = new List<IExpireActionSources>();
             ExpireActionSources ExpireActionSource = new ExpireActionSources();
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    IList<MultiProperty> paramList = new List<MultiProperty>();
                    
                    paramList.Add(new MultiProperty { propertyName = "SourceID", propertyValue = SourceID });

                    if (ActionID > 0)
                    {
                        paramList.Add(new MultiProperty { propertyName = "ActionID", propertyValue = ActionID });
                        ActionSelectQuery.Append("SELECT id,ActionID,SourceID,SourceEnityID,SourceFrom,isnull(Actionexutedays,'0 days') AS Actionexutedays,Actionexutedate,ActionownerId FROM EH_ActionSource WHERE Actionexute=0 AND SourceID=:SourceID AND ActionID=:ActionID ");
                    }
                    else {
                        ActionSelectQuery.Append("SELECT id,ActionID,SourceID,SourceEnityID,SourceFrom,isnull(Actionexutedays,'0 days') AS Actionexutedays,Actionexutedate,ActionownerId FROM EH_ActionSource WHERE  Actionexute=0 AND SourceID=:SourceID");
                    }
                    var ActionResult = ((tx.PersistenceManager.CommonRepository.ExecuteQuerywithParam(ActionSelectQuery.ToString(), paramList)).Cast<Hashtable>().ToList());
                    if (ActionResult != null)
                    {
                        foreach (var obj in ActionResult)
                        {
                            ExpireActionSource = new ExpireActionSources();
                            ExpireActionSource.SourceFrom = Convert.ToInt32(obj["SourceFrom"]);
                            ExpireActionSource.SourceEnityID = Convert.ToInt32(obj["SourceEnityID"]);
                            ExpireActionSource.SourceID = Convert.ToInt32(obj["SourceID"]);
                            ExpireActionSource.ActionID = Convert.ToInt32(obj["ActionID"]);
                            ExpireActionSource.Actionexutedays = Convert.ToString(obj["Actionexutedays"]);
                            ExpireActionSource.ActionownerId = Convert.ToInt32(obj["ActionownerId"]);
                            ExpireActionSource.Id = Convert.ToInt32(obj["id"]);
                            ActionSourceID = Convert.ToInt32(obj["id"]);

                            if (ActionSourceID > 0)
                            {
                                var ActionDataSelectQuery = new StringBuilder();
                                ActionDataSelectQuery.Append("SELECT ID,ActionSourceID, AttributeID,AttributeCaption,AttributeTypeID,NodeID,[LEVEL],Ispublish FROM  EH_ActionSourceData  WHERE  ActionSourceID=?");
                                var ActionDataResult = ((tx.PersistenceManager.CommonRepository.ExecuteQuerywithMinParam(ActionDataSelectQuery.ToString(), ActionSourceID)).Cast<Hashtable>().ToList());
                                if (ActionDataResult != null)
                                {
                                    IList<IExpireActionSourcedatas> ActionSourcedatalist = new List<IExpireActionSourcedatas>();
                                    ExpireActionSourcedatas ActionSourcedata;
                                    foreach (var obj1 in ActionDataResult)
                                    {
                                        ActionSourcedata = new ExpireActionSourcedatas(); 
                                        ActionSourcedata.Id = Convert.ToInt32(obj1["ID"]);
                                        ActionSourcedata.ActionSourceID = Convert.ToInt32(obj1["ActionSourceID"]);
                                        ActionSourcedata.AttributeID = Convert.ToInt32(obj1["AttributeID"]);
                                        ActionSourcedata.AttributeCaption = Convert.ToString(obj1["AttributeCaption"]);
                                        ActionSourcedata.AttributeTypeID = Convert.ToInt32(obj1["AttributeTypeID"]);
                                        ActionSourcedata.NodeID = Convert.ToString(obj1["NodeID"]);
                                        ActionSourcedata.Level = Convert.ToInt32(obj1["LEVEL"]);
                                        ActionSourcedata.Ispublish = (obj1["Ispublish"] != null) ? true : false;
                                        //ActionSourcedatalist.Add(ActionSourcedata);
                                        //iiActionSourcedataslist.Add(ActionSourcedata);
                                    }
                                    ExpireActionSource.ActionSourcedatas = ActionSourcedatalist;
                                }

                            }

                            iiActionSourcelist.Add(ExpireActionSource);
                        }
                        return iiActionSourcelist;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return null;
        }


        public bool DeleteExpireAction(ExpireHandlerManagerProxy proxy, int ActionsourceId)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    IList <ExpireHandlerActionSourceDao> iiExpiresourcedao = new List<ExpireHandlerActionSourceDao>();
                    IList<ExpireHandlerActionSourceDataDao> iiExpiresourceDatadao = new List<ExpireHandlerActionSourceDataDao>();
                    iiExpiresourcedao = tx.PersistenceManager.DamRepository.Query<ExpireHandlerActionSourceDao>().Where(a => a.Id == ActionsourceId).Select(a => a).ToList();
                    iiExpiresourceDatadao = tx.PersistenceManager.DamRepository.Query<ExpireHandlerActionSourceDataDao>().Where(a => a.ActionSourceID == ActionsourceId).Select(a => a).ToList();
                    if (iiExpiresourcedao.Count > 0)
                    {

                        if (iiExpiresourceDatadao.Count > 0)
                        {
                            tx.PersistenceManager.ExpireHandlerRepository.Delete<ExpireHandlerActionSourceDataDao>(iiExpiresourceDatadao);
                        }
                        tx.PersistenceManager.ExpireHandlerRepository.Delete<ExpireHandlerActionSourceDao>(iiExpiresourcedao);
                       
                    }
                    tx.Commit();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public IAssets GetAssetAttributesDetails(DigitalAssetManagerProxy proxy, int assetId, bool IsforAdmin = false)
        {
            int version = MarcomManagerFactory.ActiveMetadataVersionNumber;
            IAssets asset = new Assets();
            AssetsDao assetObj = new AssetsDao();
            MediaGeneratorAssetDao mediageneratorObj = new MediaGeneratorAssetDao();
            List<IAssets> assetdet = new List<IAssets>();
            if (IsforAdmin)
                version = MarcomManagerFactory.AdminMetadataVersionNumber;
            IList<IAttributeData> attributesWithValues = new List<IAttributeData>();
            IList<BrandSystems.Marcom.Core.Planning.Interface.ITreeDropDownLabel> droplabel;
            IList<BrandSystems.Marcom.Core.Planning.Interface.ITreeDropDownCaption> itreeCaption = new List<BrandSystems.Marcom.Core.Planning.Interface.ITreeDropDownCaption>();
            AttributeData attributedate;
            IList<IDAMFile> iifilelist = new List<IDAMFile>();
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    assetObj = (from item in tx.PersistenceManager.PlanningRepository.Query<AssetsDao>()
                                where item.ID == assetId
                                select item).FirstOrDefault();

                    var AssetSelectQuery = new StringBuilder();
                    if (assetId != 0)
                    {
                        AssetSelectQuery.Append("select [ID],[Name],[VersionNo],[MimeType],[Extension],[Size],[OwnerID],[CreatedOn],[Checksum],[FileGuid],[Description],[AssetID],[Status],isnull(Additionalinfo,'') AS Additionalinfo FROM DAM_File where AssetID = ? ORDER BY [VersionNo] ASC");
                    }
                    var Result = ((tx.PersistenceManager.CommonRepository.ExecuteQuerywithMinParam(AssetSelectQuery.ToString(), assetId)).Cast<Hashtable>().ToList());
                    foreach (var obj in Result)
                    {
                        DAMFile damFile = new DAMFile();
                        damFile.ID = Convert.ToInt32(obj["ID"]);
                        damFile.AssetID = Convert.ToInt32(obj["AssetID"]);
                        damFile.Name = Convert.ToString(obj["Name"]);
                        damFile.VersionNo = Convert.ToInt32(obj["VersionNo"]);
                        damFile.MimeType = Convert.ToString(obj["MimeType"]);
                        damFile.Extension = Convert.ToString(obj["Extension"]);
                        damFile.Size = Convert.ToInt64(obj["Size"]);
                        damFile.Ownerid = Convert.ToInt32(obj["OwnerID"]);
                        damFile.CreatedOn = DateTimeOffset.Parse(obj["CreatedOn"].ToString());
                        damFile.StrCreatedDate = DateTimeOffset.Parse(obj["CreatedOn"].ToString()) != null ? DateTimeOffset.Parse(obj["CreatedOn"].ToString()).ToString("yyyy-MM-dd") : "";
                        damFile.Checksum = Convert.ToString(obj["Checksum"]);
                        damFile.Fileguid = (Guid)(obj["FileGuid"]);
                        damFile.Description = Convert.ToString(obj["Description"]);
                        var userDao = tx.PersistenceManager.DamRepository.Get<UserDao>(UserDao.MappingNames.Id, damFile.Ownerid);
                        damFile.OwnerName = null;
                        if (userDao != null)
                            damFile.OwnerName = userDao.FirstName + " " + userDao.LastName;
                        damFile.Status = Convert.ToInt32(obj["Status"]);
                        damFile.Additionalinfo = Convert.ToString(obj["Additionalinfo"]);
                        damFile.StrCreatedDateTime = DateTimeOffset.Parse(obj["CreatedOn"].ToString()) != null ? DateTimeOffset.Parse(obj["CreatedOn"].ToString()).ToString("yyyy-MM-dd HH:mm") : "";
                        damFile.Activestatus = Convert.ToInt32(obj["ID"]) == assetObj.ActiveFileID ? true : false;

                        iifilelist.Add(damFile);
                    }
                    asset.Files = iifilelist;
                    //var allattributes = tx.PersistenceManager.PlanningRepository.GetAll<BrandSystems.Marcom.Dal.Metadata.Model.AttributeDao>();

                    try
                    {
                        if (assetObj.AssetTypeid == 33)
                        {
                            mediageneratorObj = (from item in tx.PersistenceManager.PlanningRepository.Query<MediaGeneratorAssetDao>()
                                                 where item.AssetID == assetId
                                                 select item).OrderByDescending(a => a.ID).FirstOrDefault();

                            asset.MediaGeneratorData = mediageneratorObj;
                        }
                    }
                    catch
                    {

                    }

                    asset.ID = assetObj.ID;
                    asset.Name = assetObj.Name;
                    asset.FolderID = assetObj.FolderID;
                    asset.EntityID = assetObj.EntityID;
                    asset.AssetTypeid = assetObj.AssetTypeid;
                    asset.CreatedBy = assetObj.CreatedBy;
                    asset.Createdon = assetObj.Createdon;
                    asset.StrCreatedDate = assetObj.Createdon != null ? assetObj.Createdon.ToString("yyyy-MM-dd") : "";
                    asset.ActiveFileID = assetObj.ActiveFileID;
                    asset.Status = assetObj.Status;
                    asset.Active = assetObj.Active;
                    asset.AssetAccess = assetObj.AssetAccess == null ? "" : assetObj.AssetAccess;
                    asset.Category = assetObj.Category;
                    asset.Url = assetObj.Url;
                    asset.IsPublish = assetObj.IsPublish;
                    asset.LinkedAssetID = assetObj.LinkedAssetID;
                    asset.StrUpdatedOn = assetObj.UpdatedOn != null ? assetObj.UpdatedOn.ToString("yyyy-MM-dd") : asset.StrCreatedDate;

                    //if (asset.AssetTypeid != 34 && asset.AssetTypeid != 33)
                    //{
                    string xmlpath = tx.PersistenceManager.MetadataRepository.GetXmlPath(version);
                    XDocument docx = XDocument.Load(xmlpath);
                    var rddd = (from EntityAttrRel in docx.Root.Elements("EntityTypeAttributeRelation_Table").Elements("EntityTypeAttributeRelation")
                                join Attr in docx.Root.Elements("Attribute_Table").Elements("Attribute") on Convert.ToInt32(EntityAttrRel.Element("AttributeID").Value) equals Convert.ToInt32(Attr.Element("ID").Value)
                                where Convert.ToInt32(EntityAttrRel.Element("EntityTypeID").Value) == assetObj.AssetTypeid
                                orderby Convert.ToInt32(EntityAttrRel.Element("SortOrder").Value)
                                select new
                                {
                                    ID = Convert.ToInt16(Attr.Element("ID").Value),
                                    Caption = EntityAttrRel.Element("Caption").Value,
                                    AttributeTypeID = Convert.ToInt16(Attr.Element("AttributeTypeID").Value),
                                    Description = Attr.Element("Description").Value,
                                    IsSystemDefined = Convert.ToBoolean(Convert.ToInt32(Attr.Element("IsSystemDefined").Value)),
                                    IsSpecial = Convert.ToBoolean(Convert.ToInt32(Attr.Element("IsSpecial").Value)),
                                    InheritFromParent = Convert.ToBoolean(Convert.ToInt32(EntityAttrRel.Element("InheritFromParent").Value)),
                                    ChooseFromParent = Convert.ToBoolean(Convert.ToInt32(EntityAttrRel.Element("ChooseFromParentOnly").Value)),
                                    IsReadOnly = Convert.ToBoolean(Convert.ToInt32(EntityAttrRel.Element("IsReadOnly").Value))
                                }).ToList();

                    var attributesdetails = rddd;
                    //var multiSelectValuedao = (from item in tx.PersistenceManager.PlanningRepository.Query<MultiSelectDao>()
                    //                           where item.Entityid == entityId
                    //                           select item).ToList();

                    List<TreeValueDao> treevaluedao = new List<TreeValueDao>();
                    List<int> treevalues = new List<int>();

                    List<TreeValueDao> multiselecttreevalues = new List<TreeValueDao>();
                    List<int> temptreevalues = new List<int>();

                    //IList<IAttributeData> entityUserAttrVal = new List<IAttributeData>();
                    //entityUserAttrVal = proxy.MarcomManager.PlanningManager.GetEntityAttributesDetailsUserDetails(proxy.MarcomManager.User.Id);

                    //var assetName = GetAssetName(tx, assetId, 1);
                    var assetName = "";
                    var dynamicvalues = tx.PersistenceManager.DamRepository.GetAll<DynamicAttributesDao>(assetName).Where(a => a.Id == assetId).Select(a => a.Attributes).SingleOrDefault();
                    foreach (var val in attributesdetails)
                    {
                        AttributesList attypeid = (AttributesList)val.AttributeTypeID;
                        if (Convert.ToInt32(AttributesList.DropDownTree) == val.AttributeTypeID || Convert.ToInt32(AttributesList.DropDownTree) == val.AttributeTypeID)
                        {
                            treevaluedao = new List<TreeValueDao>();
                            treevaluedao = tx.PersistenceManager.PlanningRepository.Query<TreeValueDao>().Where(a => a.Entityid == assetId && a.Attributeid == val.ID).OrderBy(a => a.Level).ToList();
                            treevalues = new List<int>();
                            treevalues = (from treevalue in treevaluedao where treevalue.Attributeid == val.ID select treevalue.Nodeid).ToList();
                        }
                        if (Convert.ToInt32(AttributesList.TreeMultiSelection) == val.AttributeTypeID || Convert.ToInt32(AttributesList.TreeMultiSelection) == val.AttributeTypeID)
                        {
                            multiselecttreevalues = new List<TreeValueDao>();
                            multiselecttreevalues = tx.PersistenceManager.PlanningRepository.Query<TreeValueDao>().Where(a => a.Entityid == assetId && a.Attributeid == val.ID).OrderBy(a => a.Level).ToList();
                            temptreevalues = new List<int>();
                            temptreevalues = (from treevalue in multiselecttreevalues where treevalue.Attributeid == val.ID select treevalue.Nodeid).ToList();
                        }
                        switch (attypeid)
                        {
                            case AttributesList.TextSingleLine:
                                attributedate = new AttributeData();
                                attributedate.ID = val.ID;
                                attributedate.TypeID = val.AttributeTypeID;
                                attributedate.Lable = val.Caption.Trim();
                                if (val.IsSpecial == true && val.ID == Convert.ToInt32(SystemDefinedAttributes.Name))
                                {
                                    attributedate.Caption = Enum.GetName(typeof(SystemDefinedAttributes), Convert.ToInt32(SystemDefinedAttributes.Name)) == "" ? "-" : Enum.GetName(typeof(SystemDefinedAttributes), Convert.ToInt32(SystemDefinedAttributes.Name));
                                    attributedate.Value = (string)assetObj.Name;
                                }
                                else
                                {
                                    if (dynamicvalues != null)
                                    {
                                        attributedate.Caption = dynamicvalues[val.ID.ToString()] == "" ? "-" : (dynamic)dynamicvalues[val.ID.ToString()];
                                        attributedate.Value = (dynamic)dynamicvalues[val.ID.ToString()];
                                    }
                                    else
                                    {
                                        attributedate.Caption = "-";
                                        attributedate.Value = "-";
                                    }
                                }
                                attributedate.IsSpecial = val.IsSpecial;



                                attributesWithValues.Add(attributedate);
                                break;

                            case AttributesList.TextMultiLine:
                                attributedate = new AttributeData();
                                if (dynamicvalues != null)
                                    attributedate.Caption = dynamicvalues[val.ID.ToString()] == "" ? "-" : (dynamic)dynamicvalues[val.ID.ToString()];
                                else
                                    attributedate.Caption = "-";
                                attributedate.ID = val.ID;
                                attributedate.TypeID = val.AttributeTypeID;
                                attributedate.Lable = val.Caption.Trim();
                                if (dynamicvalues != null)
                                    attributedate.Value = (dynamic)dynamicvalues[val.ID.ToString()];
                                else
                                    attributedate.Value = "-";
                                attributedate.IsSpecial = val.IsSpecial;


                                attributesWithValues.Add(attributedate);
                                break;

                            case AttributesList.ListSingleSelection:
                                attributedate = new AttributeData();
                                attributedate.ID = val.ID;
                                attributedate.TypeID = val.AttributeTypeID;
                                attributedate.Lable = val.Caption.Trim();
                                attributedate.IsSpecial = val.IsSpecial;
                                if (val.IsSpecial == true)
                                {
                                    if (val.AttributeTypeID == 3)
                                    {
                                        var currentRole = tx.PersistenceManager.PlanningRepository.Query<EntityTypeRoleAclDao>().Where(t => t.EntityTypeID == assetObj.AssetTypeid && (EntityRoles)t.EntityRoleID == EntityRoles.Owner).SingleOrDefault();
                                        attributedate.Value = assetObj.CreatedBy;
                                        int value = Convert.ToInt32(attributedate.Value);
                                        var singleCaption = (from item in tx.PersistenceManager.PlanningRepository.Query<UserDao>() where item.Id == value select item.FirstName + " " + item.LastName);
                                        attributedate.Caption = singleCaption;
                                    }
                                }
                                else if (val.IsSpecial == false)
                                {
                                    if (dynamicvalues == null)
                                    {
                                        attributedate.Value = 0;
                                        attributedate.Caption = "";
                                    }
                                    else
                                    {
                                        attributedate.Value = dynamicvalues[val.ID.ToString()] == null ? 0 : (dynamic)dynamicvalues[val.ID.ToString()];

                                        var singleCaption = (from item in tx.PersistenceManager.PlanningRepository.Query<OptionDao>() where item.Id == Convert.ToInt32(dynamicvalues[val.ID.ToString()]) select item.Caption).ToList();
                                        attributedate.Caption = singleCaption;
                                    }
                                }

                                attributesWithValues.Add(attributedate);
                                break;

                            case AttributesList.ListMultiSelection:
                                var multiSelectValuedao = (from item in tx.PersistenceManager.PlanningRepository.Query<DamMultiSelectValueDao>()
                                                           where item.AssetID == assetId
                                                           select item).ToList();
                                attributedate = new AttributeData();
                                attributedate.ID = val.ID;
                                attributedate.Lable = val.Caption.Trim();
                                attributedate.IsSpecial = val.IsSpecial;
                                attributedate.TypeID = val.AttributeTypeID;
                                var optionIDs = (from multiValues in multiSelectValuedao where multiValues.AttributeID == val.ID select multiValues.OptionID).ToArray();
                                var optioncaption = (from item in tx.PersistenceManager.DamRepository.Query<OptionDao>() where optionIDs.Contains(item.Id) select item.Caption).ToList();
                                string Multicaptionresults = string.Join<string>(", ", optioncaption);
                                attributedate.Caption = Multicaptionresults;
                                attributedate.Value = optionIDs;


                                attributesWithValues.Add(attributedate);
                                break;

                            case AttributesList.DateTime:
                                attributedate = new AttributeData();
                                attributedate.Caption = val.Caption.Trim();
                                attributedate.ID = val.ID;
                                attributedate.IsSpecial = val.IsSpecial;
                                attributedate.TypeID = val.AttributeTypeID;
                                attributedate.Lable = val.Caption.Trim();
                                if ((object)dynamicvalues[val.ID.ToString()] != null)
                                    attributedate.Value = (object)dynamicvalues[val.ID.ToString()];
                                else
                                    attributedate.Value = null;


                                attributesWithValues.Add(attributedate);
                                break;

                            case AttributesList.Dateaction:
                                attributedate = new AttributeData();
                                attributedate.ID = val.ID;
                                attributedate.TypeID = val.AttributeTypeID;
                                attributedate.Lable = val.Caption.Trim();
                                if (val.IsSpecial == true && val.ID == Convert.ToInt32(SystemDefinedAttributes.Dateaction))
                                {
                                    attributedate.Caption = Enum.GetName(typeof(SystemDefinedAttributes), Convert.ToInt32(SystemDefinedAttributes.Dateaction)) == "" ? "-" : Enum.GetName(typeof(SystemDefinedAttributes), Convert.ToInt32(SystemDefinedAttributes.Dateaction));
                                    attributedate.Value = (string)assetObj.Createdon.ToString();
                                }
                                else
                                {
                                    if (dynamicvalues != null)
                                    {
                                        attributedate.Caption = dynamicvalues[val.ID.ToString()] == "" ? "-" : (dynamic)dynamicvalues[val.ID.ToString()];
                                        attributedate.Value = (dynamic)dynamicvalues[val.ID.ToString()];
                                    }
                                    else
                                    {
                                        attributedate.Caption = "-";
                                        attributedate.Value = "-";
                                    }
                                }
                                attributedate.IsSpecial = val.IsSpecial;



                                attributesWithValues.Add(attributedate);
                                break;

                            case AttributesList.DropDownTree:
                                attributedate = new AttributeData();
                                attributedate.ID = val.ID;
                                attributedate.IsSpecial = val.IsSpecial;
                                droplabel = new List<BrandSystems.Marcom.Core.Planning.Interface.ITreeDropDownLabel>();

                                var treeLevelList = tx.PersistenceManager.PlanningRepository.Query<TreeLevelDao>().Where(a => a.AttributeID == val.ID).ToList();
                                List<int> dropdownResults = new List<int>();
                                if (treevaluedao.Count > 0)
                                {
                                    foreach (var lvlObj in treevaluedao)
                                    {
                                        treeLevelList.Remove(treeLevelList.Where(a => a.Level == lvlObj.Level).FirstOrDefault());
                                    }
                                    var entityTreeLevelList = treevaluedao.Select(a => a.Level).ToList();
                                    dropdownResults = (from treevalue in treevaluedao where treevalue.Attributeid == val.ID select treevalue.Nodeid).ToList();
                                    var nodes = (from item in tx.PersistenceManager.PlanningRepository.Query<TreeNodeDao>() where dropdownResults.Contains(item.Id) select item.Level);
                                    var distinctNodes = nodes.Distinct();
                                    int lastRow = 0;
                                    foreach (var dropnode in distinctNodes)
                                    {
                                        BrandSystems.Marcom.Core.Planning.Interface.ITreeDropDownLabel dropdownlabel = new BrandSystems.Marcom.Core.Planning.TreeDropDownLabel();
                                        BrandSystems.Marcom.Core.Planning.Interface.ITreeDropDownCaption treecaption = new BrandSystems.Marcom.Core.Planning.TreeDropDownCaption();
                                        var nodelevels = (from item in tx.PersistenceManager.PlanningRepository.Query<TreeLevelDao>() where item.Level == dropnode && item.AttributeID == val.ID select item).SingleOrDefault();
                                        treecaption.Level = nodelevels.Level;
                                        dropdownlabel.Level = nodelevels.Level;
                                        dropdownlabel.Label = nodelevels.LevelName.Trim();
                                        itreeCaption.Add(treecaption);
                                        droplabel.Add(dropdownlabel);
                                        if (lastRow == distinctNodes.Count() - 1)
                                        {
                                            foreach (var levelObj in treeLevelList)
                                            {
                                                BrandSystems.Marcom.Core.Planning.Interface.ITreeDropDownLabel dropdownlabel2 = new BrandSystems.Marcom.Core.Planning.TreeDropDownLabel();
                                                BrandSystems.Marcom.Core.Planning.Interface.ITreeDropDownCaption treecaption2 = new BrandSystems.Marcom.Core.Planning.TreeDropDownCaption();
                                                treecaption2.Level = levelObj.Level;
                                                dropdownlabel2.Level = levelObj.Level;
                                                dropdownlabel2.Label = levelObj.LevelName.Trim();
                                                itreeCaption.Add(treecaption2);
                                                droplabel.Add(dropdownlabel2);
                                            }
                                        }
                                        lastRow++;
                                    }
                                    attributedate.Lable = droplabel;
                                    var captionlist = from item in tx.PersistenceManager.PlanningRepository.Query<TreeNodeDao>() where treevalues.Contains(item.Id) orderby item.Level select item.Caption;
                                    string result = string.Join<string>(",", captionlist);
                                    attributedate.Caption = result;
                                    attributedate.TypeID = val.AttributeTypeID;
                                    attributedate.Value = treevalues;
                                    attributedate.IsInheritFromParent = val.InheritFromParent;
                                    attributedate.IsChooseFromParent = val.ChooseFromParent;
                                }
                                else
                                {
                                    foreach (var levelObj in treeLevelList)
                                    {
                                        BrandSystems.Marcom.Core.Planning.Interface.ITreeDropDownLabel dropdownlabel = new BrandSystems.Marcom.Core.Planning.TreeDropDownLabel();
                                        BrandSystems.Marcom.Core.Planning.Interface.ITreeDropDownCaption treecaption = new BrandSystems.Marcom.Core.Planning.TreeDropDownCaption();
                                        treecaption.Level = levelObj.Level;
                                        dropdownlabel.Level = levelObj.Level;
                                        dropdownlabel.Label = levelObj.LevelName.Trim();
                                        itreeCaption.Add(treecaption);
                                        droplabel.Add(dropdownlabel);
                                    }
                                    attributedate.Lable = droplabel;
                                    attributedate.Caption = "-";
                                    attributedate.TypeID = val.AttributeTypeID;
                                    attributedate.Value = treevalues;
                                    attributedate.IsInheritFromParent = val.InheritFromParent;
                                    attributedate.IsChooseFromParent = val.ChooseFromParent;
                                }

                                attributesWithValues.Add(attributedate);
                                break;

                            case AttributesList.Tree:
                                attributedate = new AttributeData();
                                attributedate.ID = val.ID;
                                attributedate.TypeID = val.AttributeTypeID;
                                attributedate.IsSpecial = val.IsSpecial;
                                var treeCaptionList = (from item in tx.PersistenceManager.PlanningRepository.Query<TreeNodeDao>() where treevalues.Contains(item.Id) select item.Caption).ToList();
                                string treeCaptionResult = string.Join<string>(", ", treeCaptionList);
                                attributedate.Caption = treeCaptionResult;
                                attributedate.Lable = val.Caption.Trim();
                                attributedate.Value = treevalues;

                                attributesWithValues.Add(attributedate);
                                break;

                            case AttributesList.Link:
                                try
                                {
                                    var linkdao = (from item in tx.PersistenceManager.PlanningRepository.Query<LinksDao>()
                                                   where item.EntityID == assetId && item.ModuleID == 5
                                                   select item).ToList();
                                    attributedate = new AttributeData();
                                    attributedate.ID = val.ID;
                                    attributedate.Lable = val.Caption.Trim();
                                    attributedate.IsSpecial = val.IsSpecial;
                                    attributedate.TypeID = val.AttributeTypeID;
                                    var linkUrl = (from item in linkdao select item.URL).ToList();
                                    string linkurlresults = string.Join<string>(", ", linkUrl);
                                    attributedate.Caption = linkurlresults;
                                    var linkName = (from item in linkdao select item.Name).ToList();
                                    string linkNameresults = string.Join<string>(", ", linkName);
                                    var linkType = (from item in linkdao select item.LinkType.ToString()).ToList();
                                    string linkTyperesults = string.Join<string>(", ", linkType);
                                    attributedate.Caption = linkurlresults;
                                    attributedate.Value = linkNameresults;
                                    attributedate.specialValue = linkTyperesults;

                                    attributesWithValues.Add(attributedate);
                                }
                                catch { }
                                break;
                            case AttributesList.Uploader:
                                attributedate = new AttributeData();
                                attributedate.ID = val.ID;
                                attributedate.TypeID = val.AttributeTypeID;
                                attributedate.IsSpecial = val.IsSpecial;

                                if (dynamicvalues != null)
                                {
                                    attributedate.Caption = dynamicvalues[val.ID.ToString()] == null ? "No thumnail present" : (dynamic)dynamicvalues[val.ID.ToString()];
                                    attributedate.Value = dynamicvalues[val.ID.ToString()] == null ? "" : (dynamic)dynamicvalues[val.ID.ToString()];
                                }
                                else
                                {
                                    attributedate.Caption = "No thumnail present";
                                    attributedate.Value = "";
                                }
                                attributedate.Lable = val.Caption.Trim();



                                attributesWithValues.Add(attributedate);
                                break;
                            case AttributesList.TreeMultiSelection:
                                attributedate = new AttributeData();
                                attributedate.ID = val.ID;
                                attributedate.IsSpecial = val.IsSpecial;


                                droplabel = new List<BrandSystems.Marcom.Core.Planning.Interface.ITreeDropDownLabel>();

                                var multiselecttreeLevelList = tx.PersistenceManager.PlanningRepository.Query<TreeLevelDao>().Where(a => a.AttributeID == val.ID).ToList();
                                List<int> multiselectdropdownResults = new List<int>();
                                if (multiselecttreevalues.Count > 0)
                                {
                                    foreach (var lvlObj in multiselecttreevalues)
                                    {
                                        multiselecttreeLevelList.Remove(multiselecttreeLevelList.Where(a => a.Level == lvlObj.Level).FirstOrDefault());
                                    }
                                    var entityTreeLevelList = multiselecttreevalues.Select(a => a.Level).ToList();
                                    multiselectdropdownResults = (from treevalue in multiselecttreevalues where treevalue.Attributeid == val.ID select treevalue.Nodeid).ToList();
                                    var nodes = (from item in tx.PersistenceManager.PlanningRepository.Query<TreeNodeDao>() where multiselectdropdownResults.Contains(item.Id) select item.Level);
                                    var distinctNodes = nodes.Distinct();
                                    int lastRow = 0;
                                    foreach (var dropnode in distinctNodes)
                                    {
                                        BrandSystems.Marcom.Core.Planning.Interface.ITreeDropDownLabel dropdownlabel = new BrandSystems.Marcom.Core.Planning.TreeDropDownLabel();
                                        BrandSystems.Marcom.Core.Planning.Interface.ITreeDropDownCaption treecaption = new BrandSystems.Marcom.Core.Planning.TreeDropDownCaption();
                                        var nodelevels = (from item in tx.PersistenceManager.PlanningRepository.Query<TreeLevelDao>() where item.Level == dropnode && item.AttributeID == val.ID select item).SingleOrDefault();
                                        treecaption.Level = nodelevels.Level;
                                        dropdownlabel.Level = nodelevels.Level;
                                        dropdownlabel.Label = nodelevels.LevelName.Trim();
                                        itreeCaption.Add(treecaption);
                                        droplabel.Add(dropdownlabel);
                                        if (lastRow == distinctNodes.Count() - 1)
                                        {
                                            foreach (var levelObj in multiselecttreeLevelList)
                                            {
                                                BrandSystems.Marcom.Core.Planning.Interface.ITreeDropDownLabel dropdownlabel2 = new BrandSystems.Marcom.Core.Planning.TreeDropDownLabel();
                                                BrandSystems.Marcom.Core.Planning.Interface.ITreeDropDownCaption treecaption2 = new BrandSystems.Marcom.Core.Planning.TreeDropDownCaption();
                                                treecaption2.Level = levelObj.Level;
                                                dropdownlabel2.Level = levelObj.Level;
                                                dropdownlabel2.Label = levelObj.LevelName.Trim();
                                                itreeCaption.Add(treecaption2);
                                                droplabel.Add(dropdownlabel2);
                                            }
                                        }
                                        lastRow++;
                                    }
                                    attributedate.Lable = droplabel;
                                    attributedate.Caption = (from item in tx.PersistenceManager.PlanningRepository.Query<TreeNodeDao>() where temptreevalues.Contains(item.Id) orderby item.Level select item.Caption).ToList();
                                    attributedate.TypeID = val.AttributeTypeID;
                                    attributedate.Value = multiselecttreevalues;
                                    attributedate.IsInheritFromParent = val.InheritFromParent;
                                    attributedate.IsChooseFromParent = val.ChooseFromParent;
                                }
                                else
                                {
                                    foreach (var levelObj in multiselecttreeLevelList)
                                    {
                                        BrandSystems.Marcom.Core.Planning.Interface.ITreeDropDownLabel dropdownlabel = new BrandSystems.Marcom.Core.Planning.TreeDropDownLabel();
                                        BrandSystems.Marcom.Core.Planning.Interface.ITreeDropDownCaption treecaption = new BrandSystems.Marcom.Core.Planning.TreeDropDownCaption();
                                        treecaption.Level = levelObj.Level;
                                        dropdownlabel.Level = levelObj.Level;
                                        dropdownlabel.Label = levelObj.LevelName.Trim();
                                        itreeCaption.Add(treecaption);
                                        droplabel.Add(dropdownlabel);
                                    }
                                    attributedate.Lable = droplabel;
                                    attributedate.Caption = "-";
                                    attributedate.TypeID = val.AttributeTypeID;
                                    attributedate.Value = multiselecttreevalues;
                                    attributedate.IsInheritFromParent = val.InheritFromParent;
                                    attributedate.IsChooseFromParent = val.ChooseFromParent;
                                }
                                attributesWithValues.Add(attributedate);
                                break;

                            default:

                                break;
                        }
                    }
                    tx.Commit();
                }
                //}

                asset.AttributeData = attributesWithValues;

                return asset;

            }
            catch (Exception ex)
            {

            }
            return null;
        }

    }
}
