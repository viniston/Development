using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using BrandSystems.Marcom.Core.Interface;
using BrandSystems.Marcom.Core.Interface.Managers;
using BrandSystems.Marcom.Core.Metadata.Interface;
using BrandSystems.Marcom.Dal.Metadata.Model;
using System.Xml;
using BrandSystems.Marcom.Core.Metadata;
using BrandSystems.Marcom.Metadata.Interface;
using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;
using BrandSystems.Marcom.Metadata;
using BrandSystems.Marcom.Core.Planning.Interface;
using BrandSystems.Marcom.Core.Common.Interface;
using BrandSystems.Marcom.Core.Access.Interface;
using BrandSystems.Marcom.Core.Managers;
using BrandSystems.Marcom.Core.Dam;
using BrandSystems.Marcom.Core.Dam.Interface;
using BrandSystems.Marcom.Core.DAM;
using BrandSystems.Marcom.Core.DAM.Interface;
using BrandSystems.Marcom.Dal.DAM.Model;

namespace BrandSystems.Marcom.Core.Core.Managers.Proxy
{
    internal partial class DigitalAssetManagerProxy : IDigitalAssetManager, IManagerProxy
    {
        // Reference to the MarcomManager
        /// <summary>
        /// The _marcom manager      
        /// </summary>
        private MarcomManager _marcomManager = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataManagerProxy" /> class.
        /// </summary>
        /// <param name="marcomManager">The marcom manager.</param>
        internal DigitalAssetManagerProxy(MarcomManager marcomManager)
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

        public IList<IEntityType> GetDAMEntityTypes()
        {
            return DigitalAssetManager.Instance.GetDAMEntityTypes(this);
        }

        public IList<IAttribute> GetAttributeDAMCreate(int EntityTypeID)
        {
            return DigitalAssetManager.Instance.GetAttributeDAMCreate(this, EntityTypeID);
        }
        public IList<IAttribute> GetDAMAttribute(int EntityTypeID)
        {
            return DigitalAssetManager.Instance.GetDAMAttribute(this, EntityTypeID);
        }

        public IList<IAttribute> GetDAMCommonAttribute()
        {
            return DigitalAssetManager.Instance.GetDAMCommonAttribute(this);
        }

        public bool DAMadminSettingsforRootLevelInsertUpdate(string jsondata, string LogoSettings, string key, int typeid)
        {
            return DigitalAssetManager.Instance.DAMadminSettingsforRootLevelInsertUpdate(this, jsondata, LogoSettings, key, typeid);

        }

        public bool DAMadminSettingsforRootLevelInsertUpdate(string jsondata, string LogoSettings)
        {
            return DigitalAssetManager.Instance.DAMadminSettingsforRootLevelInsertUpdate(this, jsondata, LogoSettings);

        }
        public string DAMGetAdminSettings(string LogoSettings, string key, int typeid)
        {
            return DigitalAssetManager.Instance.DAMGetAdminSettings(this, LogoSettings, key, typeid);
        }

        public string DAMGetAdminSettings(string LogoSettings)
        {
            return DigitalAssetManager.Instance.DAMGetAdminSettings(this, LogoSettings);
        }


        public Tuple<IList, IList> GetDAMExtensions()
        {
            return DigitalAssetManager.Instance.GetDAMExtensions(this);
        }


        public IList<IDamTypeAttributeRelation> GetDamAttributeRelation(int damID)
        {
            return DigitalAssetManager.Instance.GetDamAttributeRelation(this, damID);
        }


        /// <summary>
        /// Gets the DAM folder.
        /// </summary>
        /// <param name="attributeID">The EntityID.</param>
        /// <returns>string</returns>
        public string GetEntityDamFolder(int entityID)
        {
            return DigitalAssetManager.Instance.GetEntityDamFolder(this, entityID);
        }

        public int CreateAsset(int parentId, int TypeId, string Name, IList<IAttributeData> listattributevalues, string FileName, int VersionNo, string MimeType, string Extension, long Size, int EntityID, String FileGuid, string Description, bool IsforDuplicate = false, int Duplicatefilestatus = 0, int LinkedAssetID = 0, string fileAdditionalinfo = null, string strAssetAccess = null, int SourceAssetID = 0, bool IsforAdmin = false, int assetactioncode = 0, bool blnAttach = false)
        {
            return DigitalAssetManager.Instance.CreateAsset(this, parentId, TypeId, Name, listattributevalues, FileName, VersionNo, MimeType, Extension, Size, EntityID, FileGuid, Description, IsforDuplicate, Duplicatefilestatus, LinkedAssetID, fileAdditionalinfo, strAssetAccess, SourceAssetID, IsforAdmin, assetactioncode, blnAttach);
        }

        public List<object> GetAssets(int folderid, int entityID, int viewType, int orderby, int pageNo, bool ViewAll = false)
        {
            return DigitalAssetManager.Instance.GetAssets(this, folderid, entityID, viewType, orderby, pageNo, ViewAll);
        }

        public IAssets GetAssetAttributesDetails(int id)
        {
            return DigitalAssetManager.Instance.GetAssetAttributesDetails(this, id);
        }
        public List<object> GetDAMViewSettings()
        {
            return DigitalAssetManager.Instance.GetDAMViewSettings(this);
        }

        public int CreateFolder(int entityID, int parentNodeID, int Level, int nodeId, int id, string Key, int sortorder, string caption, string description, string colorcode)
        {
            return DigitalAssetManager.Instance.CreateFolder(this, entityID, parentNodeID, Level, nodeId, id, Key, sortorder, caption, description, colorcode);
        }

        public int UpdateFolder(int id, string caption, string description, string colorcode)
        {
            return DigitalAssetManager.Instance.UpdateFolder(this, id, caption, description, colorcode);
        }

        public bool DeleteFolder(int[] idArr)
        {
            return DigitalAssetManager.Instance.DeleteFolder(this, idArr);
        }

        public bool DeleteAssets(int[] idArr)
        {
            return DigitalAssetManager.Instance.DeleteAssets(this, idArr);
        }


        public DAMFileDao SaveFiletoAsset(int AssetID, int Status, string MimeType, long Size, string FileGuid, DateTime CreatedOn, string Extension, string Name, int VersionNo, string Description, int OwnerID)
        {
            return DigitalAssetManager.Instance.SaveFiletoAsset(this, AssetID, Status, MimeType, Size, FileGuid, CreatedOn, Extension, Name, VersionNo, Description, OwnerID);
        }

        public bool UpdateAssetVersion(int AssetID, int fileid)
        {
            return DigitalAssetManager.Instance.UpdateAssetVersion(this, AssetID, fileid);
        }

        public string DownloadDamFiles(int[] assetid, int sendDamFiles = 0, List<KeyValuePair<string, int>> CroppedList = null)
        {
            return DigitalAssetManager.Instance.DownloadDamFiles(this, assetid, sendDamFiles, CroppedList);
        }
        /// <summary>
        /// UpdateAssetAccess
        /// </summary>
        /// <param name="AssetID"></param>
        /// <param name="AccessID"></param>
        /// <returns>success</returns>
        public bool UpdateAssetAccess(int AssetID, string AccessID)
        {
            return DigitalAssetManager.Instance.UpdateAssetAccess(this, AssetID, AccessID);
        }


        public bool SaveDetailBlockForAssets(int AssetID, int AttributeTypeid, int OldValue, List<object> NewValue, int Level)
        {
            return DigitalAssetManager.Instance.SaveDetailBlockForAssets(this, AssetID, AttributeTypeid, OldValue, NewValue, Level);
        }

        public IList<IOption> GetOptionDetailListByAssetID(int id, int AssetID)
        {
            return DigitalAssetManager.Instance.GetOptionDetailListByAssetID(this, id, AssetID);
        }

        public int DuplicateAssets(int[] assetid)
        {
            return DigitalAssetManager.Instance.DuplicateAssets(this, assetid);
        }

        public int AttachAssetsEntityCreation(int entityID, int[] assetid)
        {
            return DigitalAssetManager.Instance.AttachAssetsEntityCreation(this, entityID, assetid);
        }

        /// <summary>
        /// CheckPreviewGenerator
        /// </summary>
        /// <param name="AssetIds"></param>
        /// <returns>returns preview generated ids</returns>
        public string CheckPreviewGenerator(string AssetIds)
        {
            return DigitalAssetManager.Instance.CheckPreviewGenerator(this, AssetIds);
        }

        /// <summary>
        /// PublishAssets
        /// </summary>
        /// <param name="idArr"></param>
        /// <param name="IsPublished"></param>
        /// <returns>true if ispublish value is updated</returns>
        public bool PublishAssets(int[] idArr, bool IsPublished)
        {
            return DigitalAssetManager.Instance.PublishAssets(this, idArr, IsPublished = true);
        }

        public int CreateBlankAsset(int parentId, int TypeId, string Name, IList<IAttributeData> listattributevalues, int EntityID, int Category, string url = null, bool IsforDuplicate = false, int LinkedAssetID = 0, string strAssetAccess = null, int SourceAssetID = 0)
        {
            return DigitalAssetManager.Instance.CreateBlankAsset(this, parentId, TypeId, Name, listattributevalues, EntityID, Category, url, IsforDuplicate, LinkedAssetID, strAssetAccess, SourceAssetID);
        }

        /// <summary>
        /// Send Mail
        /// </summary>
        /// <param name="proxy"></param>
        /// <param name="idarr"></param>
        /// <returns>true if mail sent</returns>
        public bool SendMail(int[] idarr, string toAddress, string subject, List<KeyValuePair<string, int>> CroppedList = null)
        {
            return DigitalAssetManager.Instance.SendMail(this, idarr, toAddress, subject, CroppedList);
        }

        /// <summary>
        /// MoveFilesToUploadImagesToCreateTask
        /// </summary>used to move the files from dam files to uploadimages folder to create task
        /// <param name="proxy"></param>
        /// <param name="TaskFiles"></param>
        /// <returns>new guid</returns>
        public Tuple<IList<BrandSystems.Marcom.Core.Common.Interface.IFile>, int> MoveFilesToUploadImagesToCreateTask(IList<BrandSystems.Marcom.Core.Common.Interface.IFile> TaskFiles, int ParentEntityID)
        {
            return DigitalAssetManager.Instance.MoveFilesToUploadImagesToCreateTask(this, TaskFiles, ParentEntityID);
        }

        public bool DeleteAttachmentVersionByAssetID(int fileid)
        {
            return DigitalAssetManager.Instance.DeleteAttachmentVersionByAssetID(this, fileid);
        }

        public bool UpdateAssetAccessSettings(int RoleID, bool IsChecked)
        {
            return DigitalAssetManager.Instance.UpdateAssetAccessSettings(this, RoleID, IsChecked);
        }


        public bool UpdateAssetDetails(int assetID, string assetName)
        {
            return DigitalAssetManager.Instance.UpdateAssetDetails(this, assetID, assetName);
        }

        public IList<IDamFileProfile> GetProfileFiles()
        {
            return DigitalAssetManager.Instance.GetProfileFiles(this);
        }

        public bool InsertUpdateProfileFiles(int id, string ProfileName, double height, double width, string extension, int dpi, string Role)
        {
            return DigitalAssetManager.Instance.InsertUpdateProfileFiles(this, id, ProfileName, height, width, extension, dpi, Role);
        }

        public bool DeleteProfilefile(int id)
        {
            return DigitalAssetManager.Instance.DeleteProfilefile(this, id);
        }
        public IList<IOptimakerSettings> GetOptimakerSettings()
        {
            return DigitalAssetManager.Instance.GetOptimakerSettings(this);
        }

        public IList<ICategory> GetOptmakerCatagories()
        {
            return DigitalAssetManager.Instance.GetOptmakerCatagories(this);
        }
        public string GetCategoryTreeCollection()
        {
            return DigitalAssetManager.Instance.GetCategoryTreeCollection(this);
        }

        public bool InsertUpdateCatergory(JObject jobj)
        {
            return DigitalAssetManager.Instance.InsertUpdateCatergory(this, jobj);
        }

        public int InsertUpdateOptimakerSettings(int ID, string Name, int CategoryId, string Description, int DocId, int DeptId, int DocType, int DocVersionId, string PreviewImage)
        {
            return DigitalAssetManager.Instance.InsertUpdateOptimakerSettings(this, ID, Name, CategoryId, Description, DocId, DeptId, DocType, DocVersionId, PreviewImage);
        }

        public bool DeleteOptimakeSetting(int ID)
        {
            return DigitalAssetManager.Instance.DeleteOptimakeSetting(this, ID);
        }

        public bool DeleteOptimakerCategory(int ID)
        {
            return DigitalAssetManager.Instance.DeleteOptimakerCategory(this, ID);
        }

        public IList GetAllOptimakerSettings(int categoryId)
        {
            return DigitalAssetManager.Instance.GetAllOptimakerSettings(this, categoryId);
        }
        public string GetOptimakerSettingsBaseURL()
        {
            return DigitalAssetManager.Instance.GetOptimakerSettingsBaseURL(this);
        }
        public string GetMediaGeneratorSettingsBaseURL()
        {
            return DigitalAssetManager.Instance.GetMediaGeneratorSettingsBaseURL(this);
        }

        public void CreateMediaGeneratorAsset(int entityID, int folderID, int assetTypeID, int createdBy, int docVersionID, string assetName, string jpegname, long jpegsize, int jpegversion, string jpegdesc, string jpegGUID, string highResGUID, string lowResGUID, int docID)
        {
            DigitalAssetManager.Instance.CreateMediaGeneratorAsset(entityID, folderID, assetTypeID, createdBy, docVersionID, assetName, jpegname, jpegsize, jpegversion, jpegdesc, jpegGUID, highResGUID, lowResGUID, docID);
        }


        public List<object> GetDAMViewAdminSettings(string ViewType, int DamType)
        {
            return DigitalAssetManager.Instance.GetDAMViewAdminSettings(this, ViewType, DamType);
        }

        public bool UpdateDamViewStatus(string ViewType, int DamType, IList<IDamViewAttribtues> attributeslist)
        {
            return DigitalAssetManager.Instance.UpdateDamViewStatus(this, ViewType, DamType, attributeslist);
        }

        public List<object> GetDAMToolTipSettings()
        {
            return DigitalAssetManager.Instance.GetDAMToolTipSettings(this);
        }

        public int InsertUpdateWordTemplateSettings(int ID, string Name, int CategoryId, string Description, string Worddocpath, string PreviewImage)
        {
            return DigitalAssetManager.Instance.InsertUpdateWordTemplateSettings(this, ID, Name, CategoryId, Description, Worddocpath, PreviewImage);
        }

        public bool DeleteWordtempSetting(int ID)
        {
            return DigitalAssetManager.Instance.DeleteWordtempSetting(this, ID);
        }

        public IList<IWordTemplateSettings> GetWordTemplateSettings()
        {
            return DigitalAssetManager.Instance.GetWordTemplateSettings(this);
        }
        public List<object> GetStatusFilteredEntityAsset(int folderid, int entityID, int viewType, int orderby, int pageNo, string statusFilter)
        {
            return DigitalAssetManager.Instance.GetStatusFilteredEntityAsset(this, folderid, entityID, viewType, orderby, pageNo, statusFilter);
        }
        public void CreateDOCGeneratedAsset(int entityID, int folderID, int assetTypeID, int createdBy, string intialfilename, string assetName)
        {
            DigitalAssetManager.Instance.CreateDOCGeneratedAsset(entityID, folderID, assetTypeID, createdBy, intialfilename, assetName);
        }


        public List<object> GetPublishedAssets(int viewType, int orderby, int pageNo)
        {
            return DigitalAssetManager.Instance.GetPublishedAssets(this, viewType, orderby, pageNo);
        }

        public int AddPublishedFilesToDAM(int[] assetids, int EntityID, int FolderID)
        {
            return DigitalAssetManager.Instance.AddPublishedFilesToDAM(this, assetids, EntityID, FolderID);
        }

        public void CreateversionDOCGeneratedAsset(int entityID, int folderID, int assetTypeID, int createdBy, string intialfilename, string assetName, int assetId)
        {
            DigitalAssetManager.Instance.CreateversionDOCGeneratedAsset(entityID, folderID, assetTypeID, createdBy, intialfilename, assetName, assetId);
        }

        public string getkeyvaluefromwebconfig(string key)
        {
            return DigitalAssetManager.Instance.getkeyvaluefromwebconfig(this, key);
        }


        public IList<IDamFileProfile> GetProfileFilesByUser(int UserId)
        {
            return DigitalAssetManager.Instance.GetProfileFilesByUser(this, UserId);
        }
        public IList<DAMFileDao> GetAssetActiveFileinfo(int assetId)
        {
            return DigitalAssetManager.Instance.GetAssetActiveFileinfo(this, assetId);
        }

        public IList<DAMFiledownloadinfo> CropRescale(int assetId, int TopLeft, int TopRight, int bottomLeft, int bottomRight, int CropFormat, int ScaleWidth, int ScaleHeight, int Dpi, int profileid, string fileformate)
        {
            return DigitalAssetManager.Instance.CropRescale(this, assetId, TopLeft, TopRight, bottomLeft, bottomRight, CropFormat, ScaleWidth, ScaleHeight, Dpi, profileid, fileformate);
        }
        public IList GetMimeType()
        {
            return DigitalAssetManager.Instance.GetMimeType(this);
        }

        public IList GetBreadCrumFolderPath(int EntityID, int CurrentFolderID)
        {
            return DigitalAssetManager.Instance.GetBreadCrumFolderPath(this, EntityID, CurrentFolderID);
        }

        public IList<object> GetAllFolderStructure(int EntityID)
        {
            return DigitalAssetManager.Instance.GetAllFolderStructure(this, EntityID);
        }

        public bool AddNewsFeedinfo(int assetId, string action, string TypeName)
        {
            return DigitalAssetManager.Instance.AddNewsFeedinfo(this, assetId, action, TypeName);
        }


        public bool SaveCropedImageForLightBox(string sourcepath, int imgwidth, int imgheight, int imgX, int imgY)
        {
            return DigitalAssetManager.Instance.SaveCropedImageForLightBox(this, sourcepath, imgwidth, imgheight, imgX, imgY);
        }
        public List<object> GetMediaAssets(int viewType, int orderbyid, int pageNo, int[] assettypeArr, string filterSchema = null)
        {
            return DigitalAssetManager.Instance.GetMediaAssets(this, viewType, orderbyid, pageNo, assettypeArr, filterSchema);
        }

        public List<object> GetMediaBankFilterAttributes()
        {
            return DigitalAssetManager.Instance.GetMediaBankFilterAttributes(this);
        }

        public int MoveAssets(int[] assetid, int folderId, int entityid, int actioncode)
        {
            return DigitalAssetManager.Instance.MoveAssets(this, assetid, folderId, entityid, actioncode);
        }
        public bool DAMadminSettingsDeleteAttributeRelationAllViews(int typeid, int ID)
        {
            return DigitalAssetManager.Instance.DAMadminSettingsDeleteAttributeRelationAllViews(this, typeid, ID);

        }

        public List<object> GetSearchAssets(string assetIDs)
        {
            return DigitalAssetManager.Instance.GetSearchAssets(this, assetIDs);
        }

        public List<object> GetCustomFilterAttributes(int typeID)
        {
            return DigitalAssetManager.Instance.GetCustomFilterAttributes(this, typeID);
        }

        public int AttachAssets(int[] assetid, int entityid, int folderId, bool blnAttach)
        {
            return DigitalAssetManager.Instance.AttachAssets(this, assetid, entityid, folderId, blnAttach);
        }
        public string GetSearchCriteriaAdminSettings(string LogoSettings, string key, int typeid)
        {
            return DigitalAssetManager.Instance.GetSearchCriteriaAdminSettings(this, LogoSettings, key, typeid);
        }

        public List<int> AttachAssetsforProofTask(int[] assetid, int entityid, int folderId, bool blnAttach)
        {
            return DigitalAssetManager.Instance.AttachAssetsforProofTask(this, assetid, entityid, folderId, blnAttach);
        }

        /// <summary>
        /// Creates the proof by ProofHQ.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="taskid">The taskid.</param>
        /// <param name="assetids">int[] assetids.</param>
        /// <returns>bool</returns>
        public bool CreateProofTaskWithAttachment(int taskID, int[] assetIds)
        {
            return DigitalAssetManager.Instance.CreateProofTaskWithAttachment(this, taskID, assetIds);
        }

        public int GetProofIDByTaskID(int taskid)
        {
            return DigitalAssetManager.Instance.GetProofIDByTaskID(this, taskid);
        }

        public int GetUserIDByEmail(string email)
        {
            return DigitalAssetManager.Instance.GetUserIDByEmail(this, email);
        }


        public IAssets ChangeAssettype(int assetTypeID, int damID)
        {
            return DigitalAssetManager.Instance.ChangeAssettype(this, assetTypeID, damID);
        }
        public IList<IDamTypeAttributeRelation> GetAssetAttributueswithTypeID(int assetTypeID)
        {
            return DigitalAssetManager.Instance.GetAssetAttributueswithTypeID(this, assetTypeID);
        }

        public int SaveAssetTypeofAsset(int typeid, string Name, IList<IAttributeData> listattributevalues, int AssetID, int oldAssetTypeId)
        {
            return DigitalAssetManager.Instance.SaveAssetTypeofAsset(this, typeid, Name, listattributevalues, AssetID, oldAssetTypeId);
        }

    }
}
