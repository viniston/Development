using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrandSystems.Marcom.Core.Cms.Interface;
using BrandSystems.Marcom.Core.Interface;
using BrandSystems.Marcom.Core.Interface.Managers;
using BrandSystems.Marcom.Core.Planning.Interface;
using BrandSystems.Marcom.Core.Common.Interface;

namespace BrandSystems.Marcom.Core.Core.Managers.Proxy
{
    internal partial class CmsManagerProxy : ICmsManager, IManagerProxy
    {
        private MarcomManager _marcommanager;

        internal CmsManagerProxy(MarcomManager marcommanager)
        {
            _marcommanager = marcommanager;
        }

        internal MarcomManager MarcomManager
        {
            get { return _marcommanager; }
        }

        public int InsertCMSEntity(int Version, string Description, int level, string Name, int NavID, int ParentID, string PublishedDate, string PublishedTime, int TemplateID, string UniqueKey, string tag)
        {
            return CmsManager.Instance.InsertCMSEntity(this, Version, Description, level, Name, NavID, ParentID, PublishedDate, PublishedTime, TemplateID, UniqueKey, tag);
        }

        public IList GetAllCmsEntitiesByNavID(int NavID, int StartpageNo, int MaxPageNo)
        {
            return CmsManager.Instance.GetAllCmsEntitiesByNavID(this, NavID, StartpageNo, MaxPageNo);
        }

        public bool DeleteCmsEntity(int ID)
        {
            return CmsManager.Instance.DeleteCmsEntity(this, ID);
        }

        public int InsertRevisedEntityContent(int EntityID, string Content)
        {
            return CmsManager.Instance.InsertRevisedEntityContent(this, EntityID, Content);
        }

        public int UpdateRevisedEntityContent(int EntityID, string Content)
        {
            return CmsManager.Instance.UpdateRevisedEntityContent(this, EntityID, Content);
        }

        public bool DeleteRevisedEntityContentID(int ID)
        {
            return CmsManager.Instance.DeleteRevisedEntityContentID(this, ID);
        }

        public IRevisedEntityContent GetRevisedContentByFeature(int EntityID)
        {
            return CmsManager.Instance.GetRevisedContentByFeature(this, EntityID);
        }

        public ICmsEntity GetCmsEntityAttributeDetails(int CmsEntityID)
        {
            return CmsManager.Instance.GetCmsEntityAttributeDetails(this, CmsEntityID);
        }

        public int UpdateCmsEntityDetailsBlockValues(int CmsEntityID, string NewValue, string attrName)
        {
            return CmsManager.Instance.UpdateCmsEntityDetailsBlockValues(this, CmsEntityID, NewValue, attrName);
        }

        public bool SaveUploaderImage(string sourcepath, string destinationfolder, int imgwidth, int imgheight, int imgX, int imgY)
        {
            return CmsManager.Instance.SaveUploaderImage(this, sourcepath, destinationfolder, imgwidth, imgheight, imgX, imgY);
        }

        public bool SaveCMSEntityColor(string description, string colorcode)
        {
            return CmsManager.Instance.SaveCMSEntityColor(description, colorcode);
        }

        public IList GetCmsEntityPageAccess(int EntityID)
        {
            return CmsManager.Instance.GetCmsEntityPageAccess(this, EntityID);
        }

        public bool UpdateCmsEntityPageAccess(int[] RoleIDs, int CmsEntityID)
        {
            return CmsManager.Instance.UpdateCmsEntityPageAccess(this, RoleIDs, CmsEntityID);
        }

        public IList GetCmsEntityPublishVersion(int CmsEntityID)
        {
            return CmsManager.Instance.GetCmsEntityPublishVersion(this, CmsEntityID);
        }

        public IList<IFeedSelection> GettingCmsFeedsByEntityID(string entityId, int pageNo, bool isForRealTimeUpdate, int entityIdForReference, int newsfeedid = 0, string newsfeedgroupid = "")
        {
            return CmsManager.Instance.GettingCmsFeedsByEntityID(this, entityId, pageNo, isForRealTimeUpdate, entityIdForReference, newsfeedid = 0, newsfeedgroupid = "");
        }

        public bool IsActiveEntity(int EntityID)
        {
            return CmsManager.Instance.IsActiveEntity(this, EntityID);
        }

        public IList GetCMSBreadCrum(int CmsEntityID)
        {
            return CmsManager.Instance.GetCMSBreadCrum(this, CmsEntityID);
        }

        public ArrayList DuplicateEntity(int CmsEntityID, int ParentLevel, int DuplicateTimes, bool IsDuplicateChild, List<string> dupEntityName)
        {
            return CmsManager.Instance.DuplicateEntity(this, CmsEntityID, ParentLevel, DuplicateTimes, IsDuplicateChild, dupEntityName);
        }

        public IList<ICmsEntity> GetCmsEntitiesByID(int[] CmsEntityID)
        {
            return CmsManager.Instance.GetCmsEntitiesByID(this, CmsEntityID);
        }

        public bool GetIsEditFeatureEnabled()
        {
            return CmsManager.Instance.GetIsEditFeatureEnabled(this);
        }
    }
}
