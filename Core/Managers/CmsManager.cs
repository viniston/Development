using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Web;
using SD = System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BrandSystems.Marcom.Core.Cms;
using BrandSystems.Marcom.Core.Cms.Interface;
using BrandSystems.Marcom.Core.Core.Managers.Proxy;
using BrandSystems.Marcom.Core.Interface;
using BrandSystems.Marcom.Dal.CMS.Model;
using BrandSystems.Marcom.Dal.Metadata.Model;
using BrandSystems.Marcom.Core.Metadata;
using BrandSystems.Marcom.Core.Metadata.Interface;
using BrandSystems.Marcom.Dal.Access.Model;
using BrandSystems.Marcom.Core.Planning.Interface;
using BrandSystems.Marcom.Core.Planning;
using BrandSystems.Marcom.Core.Utility;
using BrandSystems.Marcom.Core.Common;
using BrandSystems.Marcom.Core.Access.Interface;
using BrandSystems.Marcom.Core.Common.Interface;
using BrandSystems.Marcom.Dal.Base;
using BrandSystems.Marcom.Dal.User.Model;
using BrandSystems.Marcom.Dal.Planning.Model;
using BrandSystems.Marcom.Dal.Access.Model;

namespace BrandSystems.Marcom.Core.Core.Managers
{
    internal partial class CmsManager : IManager
    {
        private static CmsManager instance = new CmsManager();

        internal static CmsManager Instance
        {
            get { return instance; }
        }

        public void Initialize(IMarcomManager marcomManager)
        {
            // Cache and initialize things here...
        }

        public void CommitCaches()
        {

        }

        public void RollbackCaches()
        {

        }

        #region CMS methods

        public IList GetAllCmsEntitiesByNavID(CmsManagerProxy proxy, int NavID, int StartpageNo, int MaxPageNo)
        {
            try
            {
                IList<ICmsEntity> lstEntity = new List<ICmsEntity>();
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {

                    //var ObjLst = (from item in tx.PersistenceManager.CmsRepository.Query<CmsEntityDao>().Where(cmsentity => cmsentity.NavID == NavID && cmsentity.Active == true)
                    //              orderby item.UniqueKey
                    //              select item).ToList().Skip(StartpageNo).Take(MaxPageNo);

                    //var userole = (from itm in proxy.MarcomManager.User.ListOfUserGlobalRoles select itm.GlobalRoleid).Distinct().ToList();
                    //if (proxy.MarcomManager.User.ListOfUserGlobalRoles.Where(a => (FeatureID)a.Featureid == FeatureID.CMS_ContentEdit).Count() > 0)
                    //{
                    //    ObjLst = (from item in tx.PersistenceManager.CmsRepository.Query<CmsEntityDao>().ToList()
                    //              join pgeacc in tx.PersistenceManager.CmsRepository.Query<CmsEntityPageAccessDao>().ToList()
                    //              on item.ID equals pgeacc.EntityID
                    //              join usrrole in userole.ToList()
                    //              on pgeacc.RoleID equals usrrole
                    //              where item.NavID == NavID && item.Active == true
                    //              orderby item.UniqueKey
                    //              select item).Distinct().ToList().Skip(StartpageNo).Take(MaxPageNo);
                    //}
                    //else if (proxy.MarcomManager.User.ListOfUserGlobalRoles.Where(a => (FeatureID)a.Featureid == FeatureID.CMS_ContentView).Count() > 0)
                    //{
                    //    var ObjLst1 = (from item in tx.PersistenceManager.CmsRepository.Query<CmsEntityDao>().ToList()
                    //                   join pgeacc in tx.PersistenceManager.CmsRepository.Query<CmsEntityPageAccessDao>().ToList()
                    //                   on item.ID equals pgeacc.EntityID
                    //                   join usrrole in userole.ToList()
                    //                   on pgeacc.RoleID equals usrrole
                    //                   where item.NavID == NavID && item.Active == true
                    //                   orderby item.UniqueKey
                    //                   select item).Distinct().ToList().Skip(StartpageNo).Take(MaxPageNo);

                    //    ObjLst = ObjLst1.Where(a => DateTime.Parse(a.PublishedDate) <= System.DateTime.Today);
                    //}

                    StringBuilder query = new StringBuilder();

                    query.Append("  SELECT ce.ID,pe.[Active],ce.Version,met.ShortDescription,pe.Level,pe.Name,ce.NavID,pe.ParentID,ce.PublishedDate, ce.PublishedTime, ce.PublishedStatus ");
                    query.Append(" , ce.TemplateID,pe.UniqueKey,met.ColorCode ");
                    query.Append(" ,CASE WHEN pe.IsLock=1 THEN 0 WHEN ((SELECT COUNT(*) ");
                    query.Append("  FROM   AM_GlobalRole_User agru ");
                    query.Append("  WHERE  agru.UserId = " + proxy.MarcomManager.User.Id + " ");
                    query.Append("   AND agru.GlobalRoleId IN (1) ");
                    query.Append("  )>0) THEN 1 ");
                    query.Append(" WHEN (( SELECT COUNT(*) ");
                    query.Append("  FROM   AM_Entity_Role_User aeru ");
                    query.Append("  INNER JOIN AM_EntityTypeRoleAcl aera ");
                    query.Append("   ON  aera.ID = aeru.RoleID ");
                    query.Append("  WHERE  aeru.EntityID = ce.id ");
                    query.Append("  AND aeru.UserID = " + proxy.MarcomManager.User.Id + " ");
                    query.Append("   AND aera.EntityRoleID IN (1) ");
                    query.Append("  )>0) THEN 1 ");
                    query.Append("  WHEN ((SELECT COUNT(*) ");
                    query.Append("  FROM   AM_Entity_Role_User aeru ");
                    query.Append("  INNER JOIN AM_EntityTypeRoleAcl aera ");
                    query.Append(" ON  aera.ID = aeru.RoleID ");
                    query.Append("  WHERE  aeru.EntityID = ce.id ");
                    query.Append("  AND aeru.UserID = " + proxy.MarcomManager.User.Id + " ");
                    query.Append("  AND aera.EntityRoleID IN (2 ,8) ");
                    query.Append(" )>0) THEN 2 ");
                    query.Append("  WHEN ((SELECT COUNT(*) ");
                    query.Append("  FROM   AM_Entity_Role_User aeru ");
                    query.Append("  INNER JOIN AM_EntityTypeRoleAcl aera ");
                    query.Append(" ON  aera.ID = aeru.RoleID ");
                    query.Append("  WHERE  aeru.EntityID = ce.id ");
                    query.Append("  AND aeru.UserID = " + proxy.MarcomManager.User.Id + " ");
                    query.Append("  AND aera.EntityRoleID NOT IN (1, 2 ,8) ");
                    query.Append(" )>0) THEN 3 ");
                    query.Append("  ELSE 4 END  AS Permission ");
                    query.Append("  ,CASE WHEN ");
                    query.Append(" ((SELECT COUNT(*) FROM CMS_Entity ce2 WHERE ce2.NavID = " + NavID + " AND ce2.[Active] = 1 AND ce2.ParentID = ce.ID )>0) ");
                    query.Append("  THEN 1 ELSE 0 END AS IsChildrenPresent ");
                    query.Append(" FROM   CMS_Entity ce INNER JOIN PM_Entity pe ON  ce.Id = pe.ID  ");
                    query.Append(" INNER JOIN MM_EntityType met ON  pe.TypeID = met.ID ");
                    query.Append(" WHERE ce.NavID = " + NavID + " AND pe.[Active] = 1 ORDER BY pe.UniqueKey ");

                    
                    //query.Append(" WHERE ce.NavID = " + NavID + " AND pe.[Active] = 1 AND ce.PublishedDate + ' ' + ce.PublishedTime <= convert(CHAR(16), getdate(), 120) ORDER BY pe.UniqueKey ");


                    var ObjLst = tx.PersistenceManager.CmsRepository.ExecuteQuery(query.ToString());

                    //if (ObjLst != null)
                    //{
                    //    foreach (var item in ObjLst)
                    //    {
                    //        CmsEntity entity = new CmsEntity();
                    //        entity.ID = item.ID;
                    //        entity.Active = item.Active;
                    //        entity.Version = item.Version;
                    //        entity.Description = item.Description;
                    //        entity.Level = item.Level;
                    //        entity.Name = item.Name;
                    //        entity.NavID = item.NavID;
                    //        entity.ParentID = item.ParentID;
                    //        entity.PublishedDate = item.PublishedDate;
                    //        entity.PublishedTime = item.PublishedTime;
                    //        entity.PublishedStatus = item.PublishedStatus;
                    //        entity.TemplateID = item.TemplateID;
                    //        entity.UniqueKey = item.UniqueKey;
                    //        entity.IsChildrenPresent = (ObjLst.Where(a => a.ParentID == item.ID && item.Active == true).ToList().Count() > 0);
                    //        entity.Tag = Convert.ToString(tx.PersistenceManager.CmsRepository.Query<TagsDao>().Where(a => a.EntityID == item.ID).Select(a => a.Tag));
                    //        lstEntity.Add(entity);

                    //    }
                    return ObjLst;

                    //}

                }
            }
            catch
            {


            }
            return null;
        }

        public int InsertCMSEntity(CmsManagerProxy proxy, int Version, string Description, int level, string Name, int NavID, int ParentID, string PublishedDate, string PublishedTime, int TemplateID, string UniqueKey, string tag)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    string UniquekeyVal = "";
                    int Entitylevel = 0;
                    if (ParentID != 0)
                    {
                        int CurrentColl = tx.PersistenceManager.CmsRepository.Query<CmsEntityDao>().Where(a => a.ParentID == ParentID).Select(a => a.ID).Count() + 1;
                        UniquekeyVal = UniqueKey + '.' + CurrentColl;
                        Entitylevel = level + 1;
                    }
                    else
                    {
                        int CurrentColl = tx.PersistenceManager.CmsRepository.Query<CmsEntityDao>().Where(a => a.ParentID == 0).Select(a => a.ID).Count() + 1;
                        UniquekeyVal = CurrentColl.ToString();
                    }

                    CmsEntityDao Entitydao = new CmsEntityDao();
                    Entitydao.ID = 0;
                    Entitydao.Version = MarcomManagerFactory.ActiveMetadataVersionNumber;
                    Entitydao.Description = Description;
                    Entitydao.Level = Entitylevel;
                    Entitydao.Name = Name;
                    Entitydao.NavID = NavID;
                    Entitydao.ParentID = ParentID;
                    Entitydao.PublishedDate = PublishedDate;
                    Entitydao.PublishedTime = PublishedTime;
                    Entitydao.PublishedStatus = true;
                    Entitydao.TemplateID = TemplateID;
                    Entitydao.UniqueKey = UniquekeyVal;
                    Entitydao.Active = true;
                    tx.PersistenceManager.CmsRepository.Save<CmsEntityDao>(Entitydao);

                    tx.Commit();
                    string templatePath = Path.Combine(HttpRuntime.AppDomainAppPath);
                    templatePath = templatePath + "CMSFiles\\Templates\\Files\\CMSTemplate-" + TemplateID + ".html";
                    if (System.IO.File.Exists(templatePath))
                    {
                        using (StreamReader reader = new StreamReader(templatePath))
                        {
                            StringBuilder Content = new StringBuilder();
                            Content.Append(reader.ReadToEnd());
                            InsertRevisedEntityContent(proxy, Entitydao.ID, Content.ToString(), true);
                        }

                    }

                    int[] temparr = proxy.MarcomManager.AccessManager.GetGlobalRoleUserByID(proxy.MarcomManager.User.Id);


                    UpdateCmsEntityPageAccess(proxy, temparr, Entitydao.ID, true);

                    BrandSystems.Marcom.Core.Utility.FeedNotificationServer fs = new Utility.FeedNotificationServer();
                    NotificationFeedObjects obj = new NotificationFeedObjects();
                    obj.action = "create cms entity";
                    obj.Actorid = proxy.MarcomManager.User.Id;
                    obj.EntityId = Entitydao.ID;
                    obj.AttributeName = Name;
                    obj.CreatedOn = DateTimeOffset.Now;
                    obj.ParentId = ParentID;
                    fs.AsynchronousNotify(obj);

                    return Entitydao.ID;
                }
            }
            catch
            {
            }
            return 0;
        }

        public bool DeleteCmsEntity(CmsManagerProxy proxy, int ID)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    tx.PersistenceManager.CmsRepository.ExecuteQuerywithMinParam("UPDATE PM_Entity SET [Active] =0 WHERE id=?", ID);
                    tx.PersistenceManager.CmsRepository.ExecuteQuerywithMinParam("UPDATE CMS_Entity SET [Active] =0 WHERE id=?", ID);
                    tx.Commit();
                    proxy.MarcomManager.PlanningManager.RemoveEntityAsync(ID);
                    return true;
                }
            }
            catch
            {

            }
            return false;
        }

        public int InsertRevisedEntityContent(CmsManagerProxy proxy, int EntityID, string Content, bool IsFromRootLevel = false)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    bool blnActive = true;
                    if (tx.PersistenceManager.CmsRepository.Query<RevisedEntityContentDao>().Where(a => a.EntityID == EntityID).Count() > 0)
                    {
                        blnActive = false;
                    }
                    RevisedEntityContentDao revEntityDao = new RevisedEntityContentDao();
                    revEntityDao.ID = 0;
                    revEntityDao.Content = Content;
                    revEntityDao.Active = blnActive;
                    revEntityDao.EntityID = EntityID;
                    revEntityDao.CreatedOn = System.DateTime.Now.ToString();
                    tx.PersistenceManager.CmsRepository.Save<RevisedEntityContentDao>(revEntityDao);

                    if (IsFromRootLevel != true)
                    {
                        NotificationFeedObjects obj = new NotificationFeedObjects();
                        FeedNotificationServer fs = new FeedNotificationServer();
                        obj.action = "cms entity content changed";
                        obj.Actorid = proxy.MarcomManager.User.Id;
                        obj.EntityId = EntityID;
                        fs.AsynchronousNotify(obj);
                    }


                    tx.Commit();
                    return revEntityDao.ID;
                }
            }
            catch
            {

            }
            return 0;

        }

        public int UpdateRevisedEntityContent(CmsManagerProxy proxy, int EntityID, string Content)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    RevisedEntityContentDao revEntityDao = new RevisedEntityContentDao();
                    revEntityDao.ID = 0;
                    revEntityDao.Content = Content;
                    revEntityDao.Active = false;
                    revEntityDao.EntityID = EntityID;
                    revEntityDao.CreatedOn = System.DateTime.Now.ToString();
                    tx.PersistenceManager.CmsRepository.Save<RevisedEntityContentDao>(revEntityDao);
                    tx.Commit();
                    return revEntityDao.ID;
                }
            }
            catch
            {

            }
            return 0;

        }

        public bool DeleteRevisedEntityContentID(CmsManagerProxy proxy, int ID)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {

                    tx.PersistenceManager.CmsRepository.DeleteByID<RevisedEntityContentDao>(ID);
                    tx.Commit();
                    return true;
                }
            }
            catch
            {

            }
            return false;

        }

        public IRevisedEntityContent GetRevisedContentByFeature(CmsManagerProxy proxy, int EntityID)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    IRevisedEntityContent revContent = new RevisedEntityContent();
                    if (proxy.MarcomManager.User.ListOfUserGlobalRoles.Where(a => (FeatureID)a.Featureid == FeatureID.CMS_ContentEdit).Count() > 0)
                    {
                        int status=0;
                        StringBuilder str_getpermission = new StringBuilder();
                        str_getpermission.Append(" SELECT case ");
                        str_getpermission.Append(" when ((SELECT COUNT(*) FROM    AM_GlobalRole_User agru  ");
                        str_getpermission.Append(" WHERE  agru.UserId = " + proxy.MarcomManager.User.Id + " AND agru.GlobalRoleId in (1)) >0) then 1 ");
                        str_getpermission.Append(" when (( ");
                        str_getpermission.Append(" SELECT COUNT(*) FROM   AM_Entity_Role_User aeru  inner join AM_EntityTypeRoleAcl aera on aera.ID = aeru.RoleID  WHERE   ");
                        str_getpermission.Append(" aeru.EntityID = " + EntityID + " AND aeru.UserID = " + proxy.MarcomManager.User.Id + " AND aera.EntityRoleID IN (1) ");
                        str_getpermission.Append(" ) > 0) then 1 ");
                        str_getpermission.Append(" when (( ");
                        str_getpermission.Append(" SELECT COUNT(*) FROM   AM_Entity_Role_User aeru  inner join AM_EntityTypeRoleAcl aera on aera.ID = aeru.RoleID  WHERE   ");
                        str_getpermission.Append(" aeru.EntityID = " + EntityID + " AND aeru.UserID = " + proxy.MarcomManager.User.Id + " AND aera.EntityRoleID IN (2,8) ");
                        str_getpermission.Append(" )>0) then 2 ");
                        str_getpermission.Append(" else 3 end as Permission ");
                        var isRevPermission = tx.PersistenceManager.PlanningRepository.ExecuteQuery(str_getpermission.ToString());
                        status = (int)((System.Collections.Hashtable)(isRevPermission)[0])["Permission"];

                        int LatestID = 0;
                        if (status < 3)
                            LatestID = tx.PersistenceManager.CmsRepository.Query<RevisedEntityContentDao>().Where(a => a.EntityID == EntityID).OrderByDescending(a => a.ID)
                                  .Select(a => a.ID).First();
                        else if(status == 3)
                            LatestID = tx.PersistenceManager.CmsRepository.Query<RevisedEntityContentDao>().Where(a => a.EntityID == EntityID && a.Active == true)
                                 .Select(a => a.ID).First();

                        var RevColle = tx.PersistenceManager.CmsRepository.Query<RevisedEntityContentDao>().Where(a => a.ID == LatestID).First();
                        if (RevColle != null)
                        {
                            revContent.ID = RevColle.ID;
                            revContent.Active = RevColle.Active;
                            revContent.Content = RevColle.Content;
                            revContent.EntityID = RevColle.EntityID;
                        }
                        else
                        {
                            return null;
                        }

                        //revContent = (IRevisedEntityContent)tx.PersistenceManager.CmsRepository.Query<RevisedEntityContentDao>().Where(a => a.EntityID == EntityID).Cast<RevisedEntityContent>();
                    }
                    else if (proxy.MarcomManager.User.ListOfUserGlobalRoles.Where(a => (FeatureID)a.Featureid == FeatureID.CMS_ContentView).Count() > 0)
                    {
                        var RevColle = tx.PersistenceManager.CmsRepository.Query<RevisedEntityContentDao>().Where(a => a.EntityID == EntityID && a.Active == true).First();
                        if (RevColle != null)
                        {
                            revContent.ID = RevColle.ID;
                            revContent.Active = RevColle.Active;
                            revContent.Content = RevColle.Content;
                            revContent.EntityID = RevColle.EntityID;
                        }
                        else
                        {
                            return null;
                        }
                    }

                    tx.Commit();
                    return revContent;
                }
            }
            catch
            {

            }
            return null;

        }

        /// <summary>
        /// LOAD ATTRIBUTES IN DETAIL BLOCK WITH VALUES
        /// </summary>
        /// <param name="proxy"></param>
        /// <param name="CmsEntityID"></param>
        /// <returns></returns>
        public ICmsEntity GetCmsEntityAttributeDetails(CmsManagerProxy proxy, int CmsEntityID)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    IList<CmsEntityDao> lstentity;
                    lstentity = tx.PersistenceManager.CmsRepository.GetAll<CmsEntityDao>();

                    IList<TagsDao> lstentitytag;
                    lstentitytag = tx.PersistenceManager.CmsRepository.GetAll<TagsDao>();

                    var res = (from entityitm in lstentity.ToList()
                               join tagitem in lstentitytag
                               on entityitm.ID equals tagitem.EntityID into tagitemdup
                               from tagitem1 in tagitemdup.DefaultIfEmpty()
                               where entityitm.ID == CmsEntityID
                               select new
                               {
                                   EntityCaption = Convert.ToString(entityitm.Name),
                                   EntityDescription = Convert.ToString(entityitm.Description),
                                   EntityTag = tagitem1 == null ? "" : Convert.ToString(tagitem1.Tag)
                               }).ToList();


                    ICmsEntity cmsentityval = new CmsEntity();

                    cmsentityval.Name = res[0].EntityCaption;
                    cmsentityval.Description = res[0].EntityDescription;
                    cmsentityval.Tag = res[0].EntityTag;

                    tx.Commit();
                    return cmsentityval;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return null;
        }

        /// <summary>
        /// UPDATE ATTRIBUTES VALUE IN DETAIL BLOCK
        /// </summary>
        /// <param name="proxy"></param>
        /// <param name="CmsEntityID"></param>
        /// <param name="NewValue"></param>
        /// <param name="attrName"></param>
        /// <returns></returns>
        public int UpdateCmsEntityDetailsBlockValues(CmsManagerProxy proxy, int CmsEntityID, string NewValue, string attrName)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    NotificationFeedObjects obj = new NotificationFeedObjects();
                    CmsEntityDao dao = new CmsEntityDao();
                    dao = tx.PersistenceManager.CmsRepository.Get<CmsEntityDao>(CmsEntityDao.PropertyNames.ID, CmsEntityID);

                    if (attrName.ToUpper() == "NAME")
                    {
                        obj.Attributetypeid = 1;
                        obj.FromValue = dao.Name;
                        obj.AttributeName = "Name";
                        dao.Name = NewValue.Trim();
                        tx.PersistenceManager.CmsRepository.Save<CmsEntityDao>(dao);
                    }
                    else if (attrName.ToUpper() == "DESCRIPTION")
                    {
                        obj.Attributetypeid = 2;
                        obj.FromValue = dao.Description;
                        obj.AttributeName = "Description";

                        dao.Description = NewValue.Trim();
                        tx.PersistenceManager.CmsRepository.Save<CmsEntityDao>(dao);
                    }
                    else if (attrName.ToUpper() == "TAGS")
                    {
                        TagsDao tagdao = new TagsDao();
                        tagdao = tx.PersistenceManager.CmsRepository.Get<TagsDao>(TagsDao.PropertyNames.EntityID, CmsEntityID);

                        obj.Attributetypeid = 3;
                        obj.FromValue = tagdao.Tag;
                        obj.AttributeName = "Tag";

                        tagdao.Tag = NewValue.Trim();
                        tx.PersistenceManager.CmsRepository.Save<TagsDao>(tagdao);
                    }
                    else if (attrName.ToUpper() == "PUBLISHDATETIME")
                    {
                        string[] arr = NewValue.Split('@').ToArray();

                        obj.Attributetypeid = 4;
                        obj.FromValue = dao.PublishedDate + " @ " + dao.PublishedTime;
                        obj.AttributeName = "PublishedDateTime";

                        dao.PublishedDate = arr[0].ToString().Trim();
                        dao.PublishedTime = arr[1].ToString().Trim();
                        tx.PersistenceManager.CmsRepository.Save<CmsEntityDao>(dao);
                    }
                    else if (attrName.ToUpper() == "REVISIONS")
                    {
                        var revdao = tx.PersistenceManager.CmsRepository.Query<RevisedEntityContentDao>().Where(a => a.EntityID == CmsEntityID).ToList();

                        obj.Attributetypeid = 5;
                        obj.AttributeName = "Revisions";

                        string Pdate = "", PTime = "", Dt = "";

                        foreach (var itm in revdao)
                        {
                            if (itm.Active == true)
                                obj.FromValue = itm.ID.ToString();

                            if (itm.ID == int.Parse(NewValue))
                            {
                                itm.Active = true;
                                Dt = itm.CreatedOn;
                            }
                            else
                                itm.Active = false;
                        }
                        tx.PersistenceManager.CmsRepository.Save<RevisedEntityContentDao>(revdao);

                        dao.PublishedDate = Dt.Substring(0, Dt.LastIndexOf('/') + 5);
                        dao.PublishedTime = Dt.Substring(Dt.LastIndexOf('/') + 5);
                        tx.PersistenceManager.CmsRepository.Save<CmsEntityDao>(dao);
                    }
                    else if (attrName.ToUpper() == "PUBLISHSTATUS")
                    {
                        var revdao = tx.PersistenceManager.CmsRepository.Query<RevisedEntityContentDao>().Where(a => a.EntityID == CmsEntityID).ToList();
                        string[] statusval = NewValue.ToString().Split(',');

                        foreach (var itm in revdao)
                        {
                            if (statusval[0] == "0")
                            {
                                itm.Active = false;
                            }
                            else
                            {
                                if (itm.ID == int.Parse(statusval[1]))
                                    itm.Active = true;
                            }
                        }
                        tx.PersistenceManager.CmsRepository.Save<RevisedEntityContentDao>(revdao);
                    }

                    tx.Commit();

                    FeedNotificationServer fs = new FeedNotificationServer();
                    obj.action = "Cms metadata update";
                    obj.Actorid = proxy.MarcomManager.User.Id;
                    obj.AttributeId = Convert.ToInt32(1);
                    obj.EntityId = CmsEntityID;
                    obj.ToValue = NewValue;


                    fs.AsynchronousNotify(obj);

                    return CmsEntityID;
                }

            }
            catch (Exception ex)
            {
                return 0;
            }
            return 0;

        }

        /// <summary>
        /// SAVE CMS UPLOADED FILES
        /// </summary>
        /// <param name="proxy"></param>
        /// <param name="sourcepath"></param>
        /// <param name="destinationfolder"></param>
        /// <param name="imgwidth"></param>
        /// <param name="imgheight"></param>
        /// <param name="imgX"></param>
        /// <param name="imgY"></param>
        /// <returns></returns>
        public bool SaveUploaderImage(CmsManagerProxy proxy, string sourcepath, string destinationfolder, int imgwidth, int imgheight, int imgX, int imgY)
        {
            try
            {
                string orgsourcepath = HttpContext.Current.Server.MapPath("~/" + sourcepath);

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

                            int maxPixels = 100;
                            int originalWidth = imgwidth;
                            int originalHeight = imgheight;
                            double factor;
                            if (originalWidth > originalHeight)
                            {
                                factor = (double)maxPixels / originalWidth;
                            }
                            else
                            {
                                factor = (double)maxPixels / originalHeight;
                            }

                            var cloned = new SD.Bitmap(bmp).Clone(new SD.Rectangle(new SD.Point(0, 0), bmp.Size), bmp.PixelFormat);
                            var nbmp = new SD.Bitmap(cloned, new SD.Size((int)(originalWidth * factor), (int)(originalHeight * factor)));

                            MemoryStream ms = new MemoryStream();
                            nbmp.Save(ms, OriginalImage.RawFormat);

                            byte[] CropImage = ms.GetBuffer();
                            using (MemoryStream ms1 = new MemoryStream(CropImage, 0, CropImage.Length))
                            {
                                ms.Write(CropImage, 0, CropImage.Length);
                                using (SD.Image CroppedImage = SD.Image.FromStream(ms, true))
                                {
                                    string destinationpath = HttpContext.Current.Server.MapPath("~/" + destinationfolder + "/" + sourcepath.ToString().Split('/')[2]);
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

                return true;
            }
            catch (Exception ex)
            {

            }
            return false;

        }

        /// <summary>
        /// GET PAGE ACCESS FOR THE ENTITY ID PASSED
        /// </summary>
        /// <param name="proxy"></param>
        /// <param name="EntityID"></param>
        /// <returns></returns>
        public IList GetCmsEntityPageAccess(CmsManagerProxy proxy, int EntityID)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    IList<GlobalRoleDao> GblRoleDao;
                    GblRoleDao = tx.PersistenceManager.AccessRepository.GetAll<GlobalRoleDao>();

                    IList<CmsEntityPageAccessDao> pgeDao = new List<CmsEntityPageAccessDao>();
                    pgeDao = (from itm in tx.PersistenceManager.CmsRepository.Query<CmsEntityPageAccessDao>() where itm.EntityID == EntityID select itm).ToList();

                    var res = (from gblitem in GblRoleDao.ToList()
                               join pgeitem in pgeDao
                               on gblitem.Id equals pgeitem.RoleID into pgedup
                               from pgesubitm in pgedup.DefaultIfEmpty()
                               select new
                               {
                                   RoleID = Convert.ToInt32(gblitem.Id),
                                   Caption = Convert.ToString(gblitem.Caption),
                                   IsActive = pgesubitm == null ? false : true
                               }).ToList();

                    tx.Commit();
                    return res;
                }
            }
            catch (Exception ex)
            {
            }
            return null;
        }

        /// <summary>
        /// INSERT UPDATE THE PAGE ACCESS ON CLICK IN SETTIGS SPAGE
        /// </summary>
        /// <param name="proxy"></param>
        /// <param name="RoleIDs"></param>
        /// <param name="CmsEntityID"></param>
        /// <returns></returns>
        public bool UpdateCmsEntityPageAccess(CmsManagerProxy proxy, int[] RoleIDs, int CmsEntityID, bool IsFromRootLevel = false)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    if (IsFromRootLevel != true)
                    {
                        string GetRoles = "SELECT RoleID FROM CMS_EntityPage_Access WHERE entityid = " + CmsEntityID + "";
                        var ExRoles = tx.PersistenceManager.CmsRepository.ExecuteQuery(GetRoles).Cast<Hashtable>().ToList();
                        var Listroleids = ExRoles.Select(x => Convert.ToString(x["RoleID"])).ToList();
                        var RolesDB = string.Join(",", Listroleids.ToArray());

                        foreach (int ij in RoleIDs)
                        {
                            bool checkrole = RolesDB.Contains("," + ij + ",");
                            string[] checkrol1 = RolesDB.Split(',');
                            int j = checkrol1.Length;
                            string check = checkrol1[0] == "" ? "0" : checkrol1[0];
                            string checklast = checkrol1[j - 1] == "" ? "0" : checkrol1[j - 1];
                            if ((ij != int.Parse(check) && ij != int.Parse(checklast)))
                            {
                                if (checkrole == false)
                                {
                                    BrandSystems.Marcom.Core.Utility.FeedNotificationServer fs = new Utility.FeedNotificationServer();
                                    NotificationFeedObjects obj = new NotificationFeedObjects();
                                    obj.action = "changed pageaccess";
                                    obj.Actorid = proxy.MarcomManager.User.Id;
                                    obj.EntityId = CmsEntityID;
                                    obj.ToValue = "Checked";
                                    string roleName = tx.PersistenceManager.CmsRepository.Get<GlobalRoleDao>(ij).Caption;
                                    obj.AttributeName = roleName;
                                    obj.CreatedOn = DateTimeOffset.Now;
                                    fs.AsynchronousNotify(obj);
                                }
                            }
                        }


                        var te1 = string.Join(",", RoleIDs.ToArray());
                        foreach (var ij in ExRoles)
                        {
                            int ij1 = int.Parse(ij["RoleID"].ToString());
                            bool checkrole = te1.Contains("," + ij1 + ",");
                            string[] checkrol1 = te1.Split(',');
                            int j = checkrol1.Length;

                            string check1 = checkrol1[0] == "" ? "0" : checkrol1[0];
                            string checklast1 = checkrol1[j - 1] == "" ? "0" : checkrol1[j - 1];

                            if (ij1 != int.Parse(check1) && ij1 != int.Parse(checklast1))
                            {
                                if (checkrole == false)
                                {
                                    BrandSystems.Marcom.Core.Utility.FeedNotificationServer fs = new Utility.FeedNotificationServer();
                                    NotificationFeedObjects obj = new NotificationFeedObjects();
                                    obj.action = "changed pageaccess";
                                    obj.Actorid = proxy.MarcomManager.User.Id;
                                    obj.EntityId = CmsEntityID;
                                    obj.ToValue = "UnChecked";
                                    string roleName = tx.PersistenceManager.CmsRepository.Get<GlobalRoleDao>(ij1).Caption;
                                    obj.AttributeName = roleName;
                                    obj.CreatedOn = DateTimeOffset.Now;
                                    fs.AsynchronousNotify(obj);
                                }
                            }
                        }
                    }
                    tx.PersistenceManager.CmsRepository.DeleteByID<CmsEntityPageAccessDao>(CmsEntityPageAccessDao.PropertyNames.EntityID, CmsEntityID);
                    IList<CmsEntityPageAccessDao> idao = new List<CmsEntityPageAccessDao>();
                    for (int i = 0; i < RoleIDs.Length; i++)
                    {
                        idao.Add(new CmsEntityPageAccessDao { RoleID = RoleIDs[i], EntityID = CmsEntityID });
                    }

                    tx.PersistenceManager.CmsRepository.Save<CmsEntityPageAccessDao>(idao);
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
        /// GET LIST OF PUBLISHED VERSION IDS FOR THE ENTITY ID
        /// </summary>
        /// <param name="proxy"></param>
        /// <param name="CmsEntityID"></param>
        /// <returns></returns>
        public IList GetCmsEntityPublishVersion(CmsManagerProxy proxy, int CmsEntityID)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {

                    var entitydao = tx.PersistenceManager.CmsRepository.Query<CmsEntityDao>().Where(a => a.ID == CmsEntityID).Select(a => a).ToList();
                    var vercontent = tx.PersistenceManager.CmsRepository.Query<RevisedEntityContentDao>().Where(a => a.EntityID == CmsEntityID).Select(a => a).ToList();

                    var res = (from enty in entitydao
                               join con in vercontent
                               on enty.ID equals con.EntityID into condup
                               from conval in condup.DefaultIfEmpty()
                               select new
                               {
                                   EntityID = enty.ID,
                                   PublishedOn = enty.PublishedDate,
                                   PublishedTime = enty.PublishedTime,
                                   ContentVersionID = conval.ID,
                                   Active = conval.Active,
                                   CreatedOn = conval.CreatedOn
                               }).ToList();
                    tx.Commit();
                    return res;
                }
            }
            catch (Exception ex)
            {
            }
            return null;
        }

        /// <summary>
        /// GET CMS NEWS FEED FOR THE ENTITY
        /// </summary>
        /// <param name="proxy"></param>
        /// <param name="entityId"></param>
        /// <param name="pageNo"></param>
        /// <param name="isForRealTimeUpdate"></param>
        /// <param name="entityIdForReference"></param>
        /// <param name="newsfeedid"></param>
        /// <param name="newsfeedgroupid"></param>
        /// <returns></returns>
        public IList<IFeedSelection> GettingCmsFeedsByEntityID(CmsManagerProxy proxy, string entityId, int pageNo, bool isForRealTimeUpdate, int entityIdForReference, int newsfeedid = 0, string newsfeedgroupid = "")
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
                    qryUniquekeys = txuniquekey.PersistenceManager.PlanningRepository.Query<CmsEntityDao>().Where(a => entityId.Split(',').Contains(a.ID.ToString())).Select(a => a.UniqueKey).ToArray();
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


                    if (Convert.ToString(entityId) != "0")
                    {

                        feedSelectQuery.Append("select cmf.ID,cmf.Actor,cmf.UserID, cmf.TemplateID,cmf.HappenedOn,cmf.CommentedUpdatedOn,cmf.EntityID,cmf.AssocitedEntityID,cmf.AttributeGroupRecordName, cmf.TypeName,cmf.TypeName," +
                                         "cmf.AttributeName,cmf.FromValue,cmf.ToValue,pme.Name as 'EntityName',pme.UniqueKey as 'EntiyUniquekey', pme.ParentID  'EntiyParentID', parentEnt.Name as 'ParentName'," +
                                             "umuse.FirstName as 'UserFirstName',umuse.LastName 'UserLastName',umuse.Email as 'UserEmail',umuse.Image as 'UserImage'," +
                                             "umuse.TimeZone as 'UserTimeZone',umuse.FeedSelection as 'UserFeedselect', cmt.Template as 'FeedTemplate' from CM_Feed cmf inner join CMS_Entity pme on cmf.EntityID = pme.ID inner join UM_User umuse on" +
                                         " umuse.ID = cmf.Actor Left join CMS_Entity parentEnt on pme.ParentID = parentEnt.ID  inner join CM_Feed_Template cmt on cmt.ID = cmf.TemplateID and cmt.ModuleID = 6 where (pme.ID in (" + entityId.TrimEnd(',') + ") ");
                        for (int i = 0; i < qryUniquekeys.Count(); i++)
                        {
                            if (i == 0)
                                feedSelectQuery.Append("or (pme.id in (select  pe.ID FROM CMS_Entity pe where pe.uniquekey like '" + qryUniquekeys[i] + ".%'");
                            else
                                feedSelectQuery.Append("union all SELECT pe.ID FROM CMS_Entity pe where pe.uniquekey like '" + qryUniquekeys[i] + ".%'");
                        }
                        feedSelectQuery.Append("))");
                        feedSelectQuery.AppendLine(" )and cmf.HappenedOn");

                    }
                    else
                    {
                        childparLIST.Add(new MultiProperty { propertyName = "User_Id", propertyValue = proxy.MarcomManager.User.Id });

                        feedSelectQuery.Append("select distinct cmf.ID,cmf.Actor,cmf.UserID,cmf.TemplateID,cmf.HappenedOn,cmf.CommentedUpdatedOn,cmf.EntityID,cmf.AssocitedEntityID,cmf.AttributeGroupRecordName,cmf.TypeName,cmf.TypeName," +
                                                  "cmf.AttributeName,cmf.FromValue,cmf.ToValue,pme.Name as 'EntityName',pme.UniqueKey as 'EntiyUniquekey',pme.ParentID 'EntiyParentID',isnull(parentEnt.Name,'-') as 'ParentName'," +
                                                 "umuse.FirstName as 'UserFirstName',umuse.LastName 'UserLastName',umuse.Email as 'UserEmail',umuse.Image as 'UserImage'," +
                                                  "umuse.TimeZone as 'UserTimeZone',umuse.FeedSelection as 'UserFeedselect',cmt.Template as 'FeedTemplate' from CM_Feed cmf inner join CMS_Entity pme on cmf.EntityID = pme.ID  LEFT outer JOIN  CMS_Entity parentEnt on pme.ParentID = parentEnt.ID  inner join UM_User umuse on" +
                                           " umuse.ID = cmf.Actor inner join CM_Feed_Template cmt on cmt.ID = cmf.TemplateID and cmt.ModuleID = 6 inner join AM_Entity_Role_User amr on ( amr.EntityID = cmf.EntityID OR  amr.EntityID =pme.ParentID) where  amr.UserID= :User_Id and (pme.TypeID in (" + userfeedSelection[0] + ") or pme.TypeID in (select mm.ID from MM_EntityType mm where mm.IsAssociate=1)) and cmf.HappenedOn ");

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

                    var childEntiyResult = ((tx.PersistenceManager.CommonRepository.ExecuteQuerywithParam(feedSelectQuery.ToString(), childparLIST)).Cast<Hashtable>().ToList());

                    BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("========================== NEWS FEED COMMENTS QUERY EXECUTING =================================", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);
                    BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo(feedSelectQuery.ToString(), BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);

                    DateTimeOffset retreiveTime = DateTimeOffset.UtcNow;
                    TimeSpan offSet = new TimeSpan();

                    offSet = TimeSpan.Parse(proxy.MarcomManager.User.TimeZone.TrimStart('+'));

                    //  ----------- Getting the comments for the list of feeds  ------------------------------

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
                    var totalFeedEntities = (from tt in tx.PersistenceManager.PlanningRepository.Query<CmsEntityDao>() where entityIDArr.Contains(tt.ID) select new { tt.Name, tt.ID, tt.ParentID }).ToList();

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

                            StringBuilder sb = new StringBuilder(template);
                            foreach (Match match in Regex.Matches(template, @"@(.+?)@"))
                            {

                                switch (match.Value.Trim())
                                {

                                    case "@CmsEntityName@":
                                        {
                                            if (Convert.ToInt32(entityIdForReference) == Convert.ToInt32(obj["EntityID"]))
                                            {
                                                sb.Replace("new", "this");
                                                sb.Replace(match.Value, "");
                                            }
                                            else
                                                sb.Replace(match.Value, "<a href='javascript:void(0);' data-id=\"feedpath\" data-parentid='" + obj["EntiyParentID"] + "'data-entityid='" + obj["EntityID"] + "' data-typeid='" + obj["Typeid"] + "' >" + entityname + "</a>").Replace("this", "");

                                            break;
                                        }
                                    case "@AttributeName@":
                                        {
                                            sb.Replace(match.Value, Convert.ToString(obj["AttributeName"]));
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

                                    case "@Path@":
                                        {

                                            if (Convert.ToInt32(entityIdForReference) == Convert.ToInt32(obj["EntityID"]) || ((Convert.ToInt32(entityIdForReference) == Convert.ToInt32(obj["EntiyParentID"])) && (Convert.ToInt32(obj["TemplateID"]) == 201) || Convert.ToInt32(obj["TemplateID"]) == 208))
                                                sb.Replace(match.Value, "");

                                            else
                                                if (Convert.ToInt32(obj["TemplateID"]) == 209 || Convert.ToInt32(obj["TemplateID"]) == 210)
                                                {
                                                    sb.Replace(match.Value, "<a href='javascript:void(0);' data-id=\"feedpath\" data-parentid='" + obj["EntityParentID"] + "'data-entityid='" + obj["EntityID"] + "' data-typeid='" + obj["Typeid"] + "' >" + Convert.ToString(obj["EntityName"]) + "</a>");
                                                }
                                            sb.Replace(match.Value, "<a href='javascript:void(0);' data-id=\"feedpath\" data-parentid='" + obj["ParentID"] + "'data-entityid='" + obj["EntityID"] + "' data-typeid='" + obj["Typeid"] + "' >" + Convert.ToString(obj["ParentName"]) + "</a>");
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

                                    case "@AttributeValue@":
                                        {
                                            if ((Convert.ToInt32(obj["TemplateID"]) == 20) || (Convert.ToInt32(obj["TemplateID"]) == 21) || (Convert.ToInt32(obj["TemplateID"]) == 6) || (Convert.ToInt32(obj["TemplateID"]) == 7) || (Convert.ToInt32(obj["TemplateID"]) == 9) || (Convert.ToInt32(obj["TemplateID"]) == 12) || (Convert.ToInt32(obj["TemplateID"]) == 58) || (Convert.ToInt32(obj["TemplateID"]) == 59))
                                            {
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

                                            }
                                            else
                                            {
                                                sb.Replace(match.Value, Convert.ToString(obj["FromValue"]));
                                            }
                                            break;
                                        }

                                    case "@checkliststatus@":
                                        {
                                            sb.Replace(match.Value, Convert.ToString(obj["ToValue"]));
                                            break;
                                        }


                                    default:
                                        break;

                                }
                            }

                            feedObj.FeedText = Convert.ToString(sb).Trim();
                            while (feedObj.FeedText.EndsWith("in"))
                                feedObj.FeedText = feedObj.FeedText.Substring(0, feedObj.FeedText.Length - 2).Trim();
                            while (feedObj.FeedText.EndsWith("of"))
                                feedObj.FeedText = feedObj.FeedText.Substring(0, feedObj.FeedText.Length - 2).Trim();
                            while (feedObj.FeedText.EndsWith("for"))
                                feedObj.FeedText = feedObj.FeedText.Substring(0, feedObj.FeedText.Length - 3).Trim();
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

        public bool IsActiveEntity(CmsManagerProxy proxy, int EntityID)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    return tx.PersistenceManager.CommonRepository.Query<CmsEntityDao>().Where(a => a.ID == EntityID).Select(a => a.Active).FirstOrDefault();
                }
            }
            catch { }
            return false;
        }

        public bool SaveCMSEntityColor(string shortdescription, string colorcode)
        {
            try
            {
                string xmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
                XDocument adminXmlDoc = XDocument.Load(xmlpath);
                string desc = shortdescription == null ? adminXmlDoc.Descendants("CMSEntityColorSetting").Descendants("Description").FirstOrDefault().Value = "" :
                    adminXmlDoc.Descendants("CMSEntityColorSetting").Descendants("Description").FirstOrDefault().Value = shortdescription;
                string color = colorcode == null ? adminXmlDoc.Descendants("CMSEntityColorSetting").Descendants("ColorCode").FirstOrDefault().Value = "" :
                    adminXmlDoc.Descendants("CMSEntityColorSetting").Descendants("ColorCode").FirstOrDefault().Value = colorcode;
                adminXmlDoc.Save(xmlpath);
                return true;
            }
            catch (Exception ex)
            {

            }
            return false;
        }

        public IList GetCMSBreadCrum(CmsManagerProxy proxy, int CmsEntityID)
        {
            try
            {
                int intUserID = proxy.MarcomManager.User.Id;
                IList listresult;
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    IList<MultiProperty> ParList = new List<MultiProperty>();
                    //bool IsLock = proxy.MarcomManager.PlanningManager.GetLockStatus(CmSentityID).Item1;
                    //bool IsReadOnly = proxy.MarcomManager.PlanningManager.GetLockStatus(CmSentityID).Item2;
                    StringBuilder strqry = new StringBuilder();
                    ParList.Add(new MultiProperty { propertyName = "intUserID", propertyValue = intUserID });
                    ParList.Add(new MultiProperty { propertyName = "EntityID", propertyValue = CmsEntityID });
                    strqry.Append("WITH GetPath ");
                    strqry.Append("AS ");
                    strqry.Append("(");
                    strqry.Append("SELECT pe.ID, pe.Name, pe.ParentID, pe.UniqueKey  FROM  PM_Entity pe INNER JOIN CMS_Entity ce  ON pe.ID = ce.ID  WHERE pe.ID = :EntityID ");
                    strqry.Append("UNION ALL ");
                    strqry.Append("SELECT pe1.ID, pe1.Name, pe1.ParentID, pe1.UniqueKey FROM  PM_Entity pe1 INNER JOIN CMS_Entity ent ON pe1.ID = ent.ID  INNER JOIN GetPath AS Child ON ent.id = Child.ParentID");
                    strqry.Append(") ");
                    strqry.Append("SELECT *, ");
                    strqry.Append(" CASE ");
                    strqry.Append(" (SELECT COUNT(*) FROM CMS_Entity_Role_User aeru WHERE aeru.EntityID=GetPath.ID AND aeru.RoleID IN (1,2) AND aeru.UserID= :intUserID) ");
                    strqry.Append(" WHEN 0 THEN 0 ");
                    strqry.Append(" ELSE 1 ");
                    strqry.Append(" END AS EntityAccess FROM GetPath ORDER BY ID ");



                    listresult = tx.PersistenceManager.MetadataRepository.ExecuteQuerywithParam(strqry.ToString(), ParList);
                    tx.Commit();
                }

                return listresult;

            }
            catch (Exception)
            {


            }
            return null;
        }

        /// <summary>
        /// DUPLICATE THE SELECTED ENTITY
        /// </summary>
        /// <param name="proxy"></param>
        /// <param name="CmsEntityID"></param>
        /// <param name="ParentID"></param>
        /// <param name="DuplicateTimes"></param>
        /// <param name="IsDuplicateChild"></param>
        /// <param name="dupEntityName"></param>
        /// <returns></returns>
        public ArrayList DuplicateEntity(CmsManagerProxy proxy, int CmsEntityID, int ParentID, int DuplicateTimes, bool IsDuplicateChild, List<string> dupEntityName)
        {
            ArrayList DuplicatedIds = new ArrayList();
            try
            {

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
            return null;
        }

        //public Tuple<int, int, string, int> CreateDuplicateEntities(ITransaction tx, string uniquekey, int ParentID, bool isDuplicateChild, string dupEntityName, CmsEntityDao entitylistObj, IList<CmsEntityRoleUserDao> entyRoleDao, IList<RevisedEntityContentDao> entycontent, IList<CmsEntityPageAccessDao> entypgeacc, IList<TagsDao> entytag, string uniqueKey)
        //{
        //    try
        //    {
        //        //using (ITransaction tx = proxy.MarcomManager.GetTransaction())
        //        //{

        //        //newUniqueKey.Clear();
        //        //newUniqueKey.Append("SELECT ISNULL(" + uniquekey + "  +'.'+ CAST((SELECT COUNT(*)+1 FROM CMS_Entity ce WHERE ce.ParentID = ?)AS NVARCHAR(10)),ISNULL((CAST((SELECT COUNT(*)+1 FROM CMS_Entity ce WHERE ce.ParentID = ?)AS NVARCHAR(10))) ,0)+1)AS UniqueKey FROM CMS_Entity AS pe WHERE  pe.ParentID =?");

        //        CmsEntityDao entyDao = new CmsEntityDao();

        //        if (dupEntityName == "")
        //            entyDao.Name = entitylistObj.Name;
        //        else
        //            entyDao.Name = dupEntityName;
        //        entyDao.Level = (uniqueKey.Split('.').Length - 1);
        //        entyDao.NavID = entitylistObj.NavID;
        //        entyDao.ParentID = ParentID;
        //        entyDao.PublishedDate = entitylistObj.PublishedDate;
        //        entyDao.PublishedStatus = entitylistObj.PublishedStatus;
        //        entyDao.PublishedTime = entitylistObj.PublishedTime;
        //        entyDao.TemplateID = entitylistObj.TemplateID;
        //        entyDao.UniqueKey = uniqueKey;
        //        entyDao.Version = entitylistObj.Version;
        //        entyDao.Active = entitylistObj.Active;
        //        entyDao.Description = entitylistObj.Description;
        //        tx.PersistenceManager.CmsRepository.Save<CmsEntityDao>(entyDao);

        //        IList<CmsEntityRoleUserDao> ientyrole = new List<CmsEntityRoleUserDao>();
        //        foreach (var itm in entyRoleDao)
        //        {
        //            ientyrole.Add(new CmsEntityRoleUserDao
        //            {
        //                Entityid = entyDao.ID,
        //                Roleid = itm.Roleid,
        //                Userid = 1,
        //                IsInherited = itm.IsInherited,
        //                InheritedFromEntityid = itm.InheritedFromEntityid
        //            });
        //        }
        //        tx.PersistenceManager.CmsRepository.Save<CmsEntityRoleUserDao>(ientyrole);


        //        IList<RevisedEntityContentDao> ientycontent = new List<RevisedEntityContentDao>();
        //        foreach (var cnt in entycontent.Where(a => a.EntityID == entitylistObj.ID).ToList())
        //        {
        //            ientycontent.Add(new RevisedEntityContentDao { EntityID = entyDao.ID, Content = cnt.Content, Active = cnt.Active });
        //        }
        //        tx.PersistenceManager.CmsRepository.Save<RevisedEntityContentDao>(ientycontent);


        //        IList<CmsEntityPageAccessDao> iientypgeacc = new List<CmsEntityPageAccessDao>();
        //        foreach (var pge in entypgeacc.Where(a => a.EntityID == entitylistObj.ID).ToList())
        //        {
        //            iientypgeacc.Add(new CmsEntityPageAccessDao { EntityID = entyDao.ID, RoleID = pge.EntityID });
        //        }
        //        tx.PersistenceManager.CmsRepository.Save<CmsEntityPageAccessDao>(iientypgeacc);


        //        IList<TagsDao> iientytag = new List<TagsDao>();
        //        foreach (var tg in entytag.Where(a => a.EntityID == entitylistObj.ID).ToList())
        //        {
        //            iientytag.Add(new TagsDao { EntityID = entyDao.ID, Tag = tg.Tag });
        //        }
        //        tx.PersistenceManager.CmsRepository.Save<TagsDao>(iientytag);

        //        Tuple<int, int, string, int> listofdup = Tuple.Create(entyDao.ID, entyDao.ParentID, entyDao.UniqueKey, entyDao.Level);

        //        //tx.Commit();
        //        return listofdup;
        //        //}
        //    }
        //    catch (Exception ex)
        //    { }
        //    return null;
        //}

        public IList<ICmsEntity> GetAllSubEntitiesByEntityID(CmsManagerProxy proxy, int EntityID)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    IList<ICmsEntity> entitylistObj = new List<ICmsEntity>();
                    entitylistObj = (from item in tx.PersistenceManager.CmsRepository.Query<CmsEntityDao>()
                                     where item.ParentID == EntityID && item.Active == true
                                     select item).OrderBy(a => a.UniqueKey).Cast<ICmsEntity>().ToList();
                    tx.Commit();
                    return entitylistObj;
                }
            }
            catch (Exception ex) { }
            return null;
        }

        public IList<ICmsEntity> GetCmsEntitiesByID(CmsManagerProxy proxy, int[] CmsEntityID)
        {
            try
            {
                IList<ICmsEntity> lstEntity = new List<ICmsEntity>();
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    var ObjLst = (from item in tx.PersistenceManager.CmsRepository.Query<CmsEntityDao>().Where(cmsentity => CmsEntityID.Contains(cmsentity.ID) && cmsentity.Active == true)
                                  orderby item.UniqueKey
                                  select item).ToList();

                    if (ObjLst != null)
                    {
                        foreach (var item in ObjLst)
                        {
                            CmsEntity entity = new CmsEntity();
                            entity.ID = item.ID;
                            entity.Active = item.Active;
                            entity.Version = item.Version;
                            entity.Description = item.Description;
                            entity.Level = item.Level;
                            entity.Name = item.Name;
                            entity.NavID = item.NavID;
                            entity.ParentID = item.ParentID;
                            entity.PublishedDate = item.PublishedDate;
                            entity.PublishedTime = item.PublishedTime;
                            entity.PublishedStatus = item.PublishedStatus;
                            entity.TemplateID = item.TemplateID;
                            entity.UniqueKey = item.UniqueKey;
                            entity.IsChildrenPresent = (ObjLst.Where(a => a.ParentID == item.ID && item.Active == true).ToList().Count() > 0);
                            entity.Tag = Convert.ToString(tx.PersistenceManager.CmsRepository.Query<TagsDao>().Where(a => a.EntityID == item.ID).Select(a => a.Tag));
                            lstEntity.Add(entity);
                        }
                        return lstEntity;

                    }

                }
            }
            catch
            {


            }
            return null;
        }

        public bool GetIsEditFeatureEnabled(CmsManagerProxy proxy)
        {
            try
            {
                if (proxy.MarcomManager.User.ListOfUserGlobalRoles.Where(a => (FeatureID)a.Featureid == FeatureID.CMS_ContentEdit).Count() > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex) { return false; } return false;
        }

        #endregion
    }
}
