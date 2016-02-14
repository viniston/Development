using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrandSystems.Marcom.Core.Interface;
using System.Collections;
using BrandSystems.Marcom.Core.Interface.Managers;
using BrandSystems.Marcom.Core.Report.Interface;
using BrandSystems.Marcom.Utility;
using BrandSystems.Marcom.Metadata.Interface;
using BrandSystems.Marcom.Core.Planning.Interface;
using BrandSystems.Marcom.Core.Metadata;
using BrandSystems.Marcom.Core.Metadata.Interface;
using Newtonsoft.Json.Linq;


namespace BrandSystems.Marcom.Core.Managers.Proxy
{
    internal partial class ReportManagerProxy : IReportManager, IManagerProxy
    {
        // Reference to the MarcomManager
        private IMarcomManager _marcomManager = null;

        // Example of cache for the logged in user's things
        /// <summary>
        /// The user groups for logged in user
        /// </summary>


        /// <summary>
        /// Initializes a new instance of the <see cref="ReportManagerProxy"/> class.
        /// </summary>
        /// <param name="marcomManager">The marcom manager.</param>
        public ReportManagerProxy(IMarcomManager marcomManager)
        {
            _marcomManager = marcomManager;
            // Do some initialization.... 
            // i.e. cache logged in user specific things (or maybe use lazy loading for that)
        }

        /// Reference to the MarcomManager (only internal)
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

        // <summary>
        /// Update users.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="ReportUrl">The ReportUrl.</param>
        /// <param name="AdminUsername">The AdminUsername.</param>
        /// <param name="AdminPassword">The AdminPassword.</param>
        /// <param name="ViewerUsername">The ViewerUsername.</param>
        /// <param name="ViewerPassword">The ViewerPassword</param>
        /// <param name="Category">Category.</param>
        /// <returns>bool</returns>

        public bool ReportCredential_InsertUpdate(string ReportUrl, string AdminUsername, string AdminPassword, string ViewerUsername, string ViewerPassword, int Category, int DataViewID, int id)
        {
            return ReportManager.Instance.ReportCredential_InsertUpdate(this, ReportUrl, AdminUsername, AdminPassword, ViewerUsername, ViewerPassword, Category, DataViewID, id);

        }

        public int ReportCredential_ValidateSave(string ReportUrl, string AdminUsername, string AdminPassword, string ViewerUsername, string ViewerPassword, int Category, int DataViewID, int id)
        {
            return ReportManager.Instance.ReportCredential_ValidateSave(this, ReportUrl, AdminUsername, AdminPassword, ViewerUsername, ViewerPassword, Category, DataViewID, id);
        }
        public IList ReportCredential_Select(int id)
        {
            return ReportManager.Instance.ReportCredential_Select(this, id);
        }

        public bool Report_InsertUpdate(int OID, string Name, string Caption, string Description, string Preview, bool Show, int CategoryId, bool EntityLevel, bool SubLevel, int id)
        {
            return ReportManager.Instance.Report_InsertUpdate(this, OID, Name, Caption, Description, Preview, Show, CategoryId, EntityLevel, SubLevel, id);
        }

        public bool UpdateReportImage(string sourcepath, int imgwidth, int imgheight, int imgX, int imgY, int OID, string Preview, string ReportName)
        {
            return ReportManager.Instance.UpdateReportImage(this, sourcepath, imgwidth, imgheight, imgX, imgY, OID, Preview, ReportName);
        }

        public IList<IReports> MergeReports(int OID)
        {
            return ReportManager.Instance.MergeReports(this, OID);
        }

        public IList CustomViews_Select(int ID)
        {
            return ReportManager.Instance.CustomViews_Select(this, ID);
        }

        public bool CustomViews_DeleteByID(int ID)
        {
            return ReportManager.Instance.CustomViews_DeleteByID(this, ID);
        }
        public int pushviewSchema()
        {
            return ReportManager.Instance.pushviewSchema(this);
        }
        public string CustomViews_Validate(string Name, string Query, int ID = 0)
        {
            return ReportManager.Instance.CustomViews_Validate(this, Name, Query, ID);
        }
        public int CustomViews_Insert(string Name, string Description, string Query)
        {
            return ReportManager.Instance.CustomViews_Insert(this, Name, Description, Query);
        }
        public bool CustomViews_Update(int ID, string Name, string Description, string Query)
        {
            return ReportManager.Instance.CustomViews_Update(this, ID, Name, Description, Query);
        }

        public IList ReportLogin(string ReportUrl, string ViewerUsername, string ViewerPassword)
        {
            return ReportManager.Instance.ReportLogin(ReportUrl, ViewerUsername, ViewerPassword);
        }

        public IList<IDataView> Dataview_select(int DataViewID, string AdminUsername = null)
        {
            return ReportManager.Instance.Dataview_select(DataViewID, AdminUsername);
        }

        public IEnumerable<ReportModel> GetListOfReports(string ViewerUsername, string ViewerPassword)
        {
            return ReportManager.Instance.GetListOfReports(this, ViewerUsername, ViewerPassword);
        }
        public IList<IReports> ShowReports(int OID, bool show)
        {
            return ReportManager.Instance.ShowReports(this, OID, show);
        }

        public bool UpdateReportSchemaResponse(int status)
        {
            return ReportManager.Instance.UpdateReportSchemaResponse(this, status);
        }

        public int GetReportViewSchemaResponse()
        {
            return ReportManager.Instance.GetReportViewSchemaResponse(this);
        }

        public IList GetFinancialSummaryDetlRpt(string SelectedEntityTypeIDs)
        {
            return ReportManager.Instance.GetFinancialSummaryDetlRpt(this, SelectedEntityTypeIDs);
        }

        public IList GetFinancialSummaryDetlRptByAttribute(string EntityTypeId, int attributeID)
        {
            return ReportManager.Instance.GetFinancialSummaryDetlRptByAttribute(this, EntityTypeId, attributeID);
        }

        public IList GetEntityFinancialSummaryDetl(string EntityTypeID, List<string> AttributeID, List<int> FinancialAttributes)
        {
            return ReportManager.Instance.GetEntityFinancialSummaryDetl(this, EntityTypeID, AttributeID, FinancialAttributes);
        }


        public string ListofRecordsSystemReport(int FilterID, IList<IFiltersettingsValues> filterSettingValues, int[] IdArr, string SortOrderColumn, bool IsDesc, ListSettings listSetting, bool IncludeChildren, int enumEntityTypeIds, int EntityID, bool IsSingleID, int UserID, int Level, bool IsobjectiveRootLevel, int ExpandingEntityID, string GanttstartDate, string Ganttenddate, bool IsMonthly)
        {
            return ReportManager.Instance.ListofRecordsSystemReport(this, FilterID, filterSettingValues, IdArr, SortOrderColumn, IsDesc, listSetting, IncludeChildren, enumEntityTypeIds, EntityID, IsSingleID, UserID, Level, IsobjectiveRootLevel, ExpandingEntityID, GanttstartDate, Ganttenddate, IsMonthly);
        }
        public Guid? GetStrucuralRptDetail(ListSettings listSetting, bool IsshowFinancialDetl, bool IsDetailIncluded, bool IsshowTaskDetl, bool IsshowMemberDetl, int ExpandingEntityIDStr, bool IncludeChildrenStr)
        {
            return ReportManager.Instance.GetStrucuralRptDetail(this, listSetting, IsshowFinancialDetl, IsDetailIncluded, IsshowTaskDetl, IsshowMemberDetl, ExpandingEntityIDStr, IncludeChildrenStr);
        }


        public string GetReportJSONData(int reportId)
        {
            return ReportManager.Instance.GetReportJSONData(this, reportId);
        }
        public IList<IEntityTypeAttributeRelationwithLevels> GetEntityTypeAttributeRelationWithLevelsByID(string ids)
        {
            return ReportManager.Instance.GetEntityTypeAttributeRelationWithLevelsByID(this, ids);
        }

        public bool InsertUpdateReportSettingXML(JObject jsonXML, int reportID)
        {
            return ReportManager.Instance.InsertUpdateReportSettingXML(this, jsonXML, reportID);
        }

        public IList GetFinancialReportSettings()
        {
            return ReportManager.Instance.GetFinancialReportSettings(this);
        }

        public bool insertupdatefinancialreportsettings(string reportsettingname, int reportID, string ReportImage, string description, JObject jsonXML)
        {
            return ReportManager.Instance.insertupdatefinancialreportsettings(this, reportsettingname, reportID, ReportImage, description, jsonXML);
        }

        public IList<IEntityTypeAttributeRelationwithLevels> GetAllEntityTypeAttributeRelationWithLevels()
        {
            return ReportManager.Instance.GetAllEntityTypeAttributeRelationWithLevels(this);
        }

        public Tuple<Guid, string> GenerateFinancialExcel(int ReportID)
        {
            return ReportManager.Instance.GenerateFinancialExcel(this, ReportID);
        }

        public bool UpdateFinancialSettingsReportImage(string sourcepath, int imgwidth, int imgheight, int imgX, int imgY, string Preview)
        {
            return ReportManager.Instance.UpdateFinancialSettingsReportImage(this, sourcepath, imgwidth, imgheight, imgX, imgY, Preview);
        }

        /// <summary>
        /// Deletes the financial report settings.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="Id">The id.</param>
        /// <returns>bool</returns>
        public bool DeletefinancialreportByID(int reportID)
        {
            return ReportManager.Instance.DeletefinancialreportByID(this, reportID);
        }


        public string ExportTaskListtoExcel(int entityId, int taskListId, bool isEntireTaskList, bool IsIncludeSublevel)
        {
            return ReportManager.Instance.ExportTaskListtoExcel(this, entityId, taskListId, isEntireTaskList, IsIncludeSublevel);
        }

        /// <summary>
        /// Getting list of Options for Fulfillment Entity Type Attributes
        /// </summary>
        /// <param name="proxy">The Proxy</param>
        /// <param name="entityTypeId">The EntityTypeID</param>
        /// <returns>IList of IAttribute</returns>
        public IList<IAttribute> GetFulfillmentAttribute(int[] entityTypeId)
        {
            return ReportManager.Instance.GetFulfillmentAttribute(this, entityTypeId);
        }

        public int InsertUpdateCustomlist(int ID, string Name, string Description, string XmlData, string ValidatedQuery)
        {
            return ReportManager.Instance.InsertUpdateCustomlist(this, ID, Name, Description, XmlData, ValidatedQuery);
        }

        public bool DeleteCustomList(int ID)
        {
            return ReportManager.Instance.DeleteCustomList(this, ID);
        }

        public IList<ICustomList> GetAllCustomList()
        {
            return ReportManager.Instance.GetAllCustomList(this);
        }

        public Tuple<string, string> CustomList_Validate(string Name, string XmlData)
        {
            return ReportManager.Instance.CustomList_Validate(this, Name, XmlData);
        }

        public IList<IOption> GetFulfillmentAttributeOptions(int attributeId, int attributeLevel = 0)
        {
            return ReportManager.Instance.GetFulfillmentAttributeOptions(this, attributeId, attributeLevel);
        }

        public bool insertupdatetabsettings(int tabtype, int tablocation, JObject jsonXML)
        {
            return ReportManager.Instance.insertupdatetabsettings(this, tabtype, tablocation, jsonXML);
        }

        public string GetLayoutData(int tabtype, int tablocation)
        {
            return ReportManager.Instance.GetLayoutData(this, tabtype, tablocation);
        }
    }
}
