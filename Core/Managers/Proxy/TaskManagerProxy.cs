using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrandSystems.Marcom.Core.Interface;
using BrandSystems.Marcom.Core.Interface.Managers;
using BrandSystems.Marcom.Core.Managers;
using BrandSystems.Marcom.Core.Task.Interface;
using BrandSystems.Marcom.Core.Common.Interface;
using BrandSystems.Marcom.Core.Planning.Interface;
using Newtonsoft.Json.Linq;
using System.Collections;
using BrandSystems.Marcom.Core.Access.Interface;
using BrandSystems.Marcom.Core.Task;

namespace BrandSystems.Marcom.Core.Core.Managers.Proxy
{
    internal partial class TaskManagerProxy : ITaskManager, IManagerProxy
    {
        // Reference to the MarcomManager
        private MarcomManager _marcomManager = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommonManagerProxy"/> class.
        /// </summary>
        /// <param name="marcomManager">The marcom manager.</param>
        internal TaskManagerProxy(MarcomManager marcomManager)
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

        public void test()
        {
            TaskManager.Instance.test(this);
        }

        /// <summary>
        /// Returns Task class.
        /// </summary>
        public IAdminTask TasksService()
        {
            return TaskManager.Instance.TasksService();
        }

        /// <summary>
        /// Returns Task class.
        /// </summary>
        public IEntityTask EntityTasksService()
        {
            return TaskManager.Instance.EntityTasksService();
        }

        /// <summary>
        /// Returns Task class.
        /// </summary>
        public ITaskMembers EntityTaskMembersService()
        {
            return TaskManager.Instance.EntityTaskMembersService();
        }

        // <summary>
        /// Returns TaskAttachment class.
        /// </summary>
        public INewTaskAttachments TasksAttachmentService()
        {
            return TaskManager.Instance.TasksAttachmentService();
        }

        public Tuple<int, ITaskList> InsertUpdateTaskList(int id, string caption, string description, int sortorder)
        {
            return TaskManager.Instance.InsertUpdateTaskList(this, id, caption, description, sortorder);
        }

        public bool DeleteSystemTaskList(int id)
        {
            return TaskManager.Instance.DeleteSystemTaskList(this, id);
        }

        public IList<ITaskLibraryTemplateHolder> GetTaskList()
        {
            return TaskManager.Instance.GetTaskList(this);
        }


        public Object GetTaskTypes()
        {
            return TaskManager.Instance.GetTaskTypes(this);
        }
        public bool UpdateTaskListSortOrder(int id, int sortorder)
        {
            return TaskManager.Instance.UpdateTaskListSortOrder(this, id, sortorder);
        }

        public int InsertUpdateTaskTemplateCondition(int id, int tasktemplateID, int typeID, int sortorder, int attributeID, string value, int attributeLevel, int conditionType)
        {
            return TaskManager.Instance.InsertUpdateTaskTemplateCondition(this, id, tasktemplateID, typeID, sortorder, attributeID, value, attributeLevel, conditionType);
        }

        public int InsertTempTaskList(int templateId, int tasklistId, int SortOrder)
        {
            return TaskManager.Instance.InsertTempTaskList(this, templateId, tasklistId, SortOrder);
        }

        public int InsertUpdateTemplate(int ID, string caption, string description)
        {
            return TaskManager.Instance.InsertUpdateTemplate(this, ID, caption, description);
        }

        public IList<ITaskTemplate> GetTaskTemplateDetails()
        {
            return TaskManager.Instance.GetTaskTemplateDetails(this);
        }

        public bool DeleteTaskTemplateListById(int templateID)
        {
            return TaskManager.Instance.DeleteTaskTemplateListById(this, templateID);
        }

        public Tuple<int, IAdminTask> InsertTaskWithAttachments(int taskTypeID, int typeid, string TaskName, IList<IAdminTask> TaskList, IList<INewTaskAttachments> TaskAttachments, IList<IFile> TaskFiles, IList<IAttributeData> entityattributedata, IList<IAdminTaskCheckList> AdminTaskChkLst, JArray arrAttchObj)
        {
            return TaskManager.Instance.InsertTaskWithAttachments(this, taskTypeID, typeid, TaskName, TaskList, TaskAttachments, TaskFiles, entityattributedata, AdminTaskChkLst, arrAttchObj);
        }
        public int AddUpdateTaskFlag(string caption, string colorcode, string description, int sortorderid, int id = 0)
        {
            return TaskManager.Instance.AddUpdateTaskFlag(this, caption, colorcode, description, sortorderid, id);
        }
        public Tuple<bool, IAdminTask> UpdateAdminTask(int milestoneTypeID, string Name, string description, int tasktype, IList<IAttributeData> milestoneObj, int entityId, IList<IAdminTaskCheckList> adminChkLst)
        {
            return TaskManager.Instance.UpdateAdminTask(this, milestoneTypeID, Name, description, tasktype, milestoneObj, entityId, adminChkLst);
        }
        public IList<ITaskFile> GetTaskAttachmentFile(int taskID)
        {
            return TaskManager.Instance.GetTaskAttachmentFile(this, taskID);
        }
        public IList<IFile> GetEntityTaskAttachmentFile(int taskID)
        {
            return TaskManager.Instance.GetEntityTaskAttachmentFile(this, taskID);
        }

        public IList<IFile> ViewAllFilesByEntityID(int taskID, int VersionFileID)
        {
            return TaskManager.Instance.ViewAllFilesByEntityID(this, taskID, VersionFileID);
        }



        public bool UpdateAttachmentVersionNo(int taskID, int SelectedVersion, int VersioningFileId)
        {
            return TaskManager.Instance.UpdateAttachmentVersionNo(this, taskID, SelectedVersion, VersioningFileId);
        }


        public bool InsertTaskAttachments(int TaskID, IList<INewTaskAttachments> TaskAttachments, IList<IFile> TaskFiles)
        {
            return TaskManager.Instance.InsertTaskAttachments(this, TaskID, TaskAttachments, TaskFiles);

        }

        public bool InsertEntityTaskAttachments(int TaskID, IList<IAttachments> TaskAttachments, IList<IFile> TaskFiles)
        {
            return TaskManager.Instance.InsertEntityTaskAttachments(this, TaskID, TaskAttachments, TaskFiles);

        }

        public int InsertEntityTaskAttachmentsVersion(int TaskID, IList<IAttachments> TaskAttachments, IList<IFile> TaskFiles, int FileID, int VersioningFileId)
        {
            return TaskManager.Instance.InsertEntityTaskAttachmentsVersion(this, TaskID, TaskAttachments, TaskFiles, FileID, VersioningFileId);

        }



        public IAttachments EntityTasksAttachmentService()
        {
            return TaskManager.Instance.EntityTasksAttachmentService();
        }

        public IList<ITaskFlag> GetTaskFlags()
        {
            return TaskManager.Instance.GetTaskFlags(this);
        }
        public bool DeleteTaskFlagById(int flagid)
        {
            return TaskManager.Instance.DeleteTaskFlagById(this, flagid);
        }

        public bool UpdateTemplateTaskListSortOrder(int TemplateId, int taskListid, int sortorder)
        {
            return TaskManager.Instance.UpdateTemplateTaskListSortOrder(this, TemplateId, taskListid, sortorder);

        }

        public bool DeleteAttachments(int ActiveFileid)
        {
            return TaskManager.Instance.DeleteAttachments(this, ActiveFileid);
        }

        public IList<ITaskLibraryTemplateHolder> GetEntityTaskList(int entityID)
        {
            return TaskManager.Instance.GetEntityTaskList(this, entityID);
        }

        public IList<IEntityTaskList> GetOverViewEntityTaskList(int entityID)
        {
            return TaskManager.Instance.GetOverViewEntityTaskList(this, entityID);
        }

        public bool UpdateOverviewEntityTaskList(int OnTimeStatus, string OnTimeComment, int ID)
        {
            return TaskManager.Instance.UpdateOverviewEntityTaskList(this, OnTimeStatus, OnTimeComment, ID);
        }
        public Tuple<int, IEntityTask> InsertEntityTaskWithAttachments(int parentEntityID, int taskTypeID, int entitytypeid, string TaskName, IList<IEntityTask> TaskList, IList<INewTaskAttachments> TaskAttachments, IList<IFile> TaskFiles, IList<ITaskMembers> taskMembers, IList<IAttributeData> entityattributedata, JArray attachFiles, IList<IEntityTaskCheckList> entityCheckList)
        {
            return TaskManager.Instance.InsertEntityTaskWithAttachments(this, parentEntityID, taskTypeID, entitytypeid, TaskName, TaskList, TaskAttachments, TaskFiles, taskMembers, entityattributedata, attachFiles, entityCheckList);
        }
        public Tuple<bool, IList<ITaskMembers>> InsertTaskMembers(int EntityID, int taskID, IList<ITaskMembers> TaskMembers, IList<ITaskMembers> TaskGlobalMembers)
        {
            return TaskManager.Instance.InsertTaskMembers(this, EntityID, taskID, TaskMembers, TaskGlobalMembers);

        }
        public Tuple<bool, int, string> UpdateTaskStatus(int taskID, int status, int entityID = 0)
        {
            return TaskManager.Instance.UpdateTaskStatus(this, taskID, status, entityID);
        }
        public bool updateTaskMemberFlag(int taskid, string colorCode)
        {
            return TaskManager.Instance.updateTaskMemberFlag(this, taskid, colorCode);
        }
        public Tuple<int, IEntityTaskList> InsertUpdateEntityTaskList(int id, string caption, string description, int sortorder, int entityID)
        {
            return TaskManager.Instance.InsertUpdateEntityTaskList(this, id, caption, description, sortorder, entityID);

        }

        public bool UpdateEntityTaskListSortOrder(JArray SortOrderObject)
        {
            return TaskManager.Instance.UpdateEntityTaskListSortOrder(this, SortOrderObject);
        }
        public Tuple<int, IEntityTask> InsertUnassignedEntityTaskWithAttachments(int parentEntityID, int taskTypeID, int entitytypeid, string TaskName, IEntityTask TaskList, IList<INewTaskAttachments> TaskAttachments, IList<IFile> TaskFiles, IList<ITaskMembers> taskMembers, IList<IAttributeData> entityattributedata, IList<IEntityTaskCheckList> AdminTaskChkLst, JArray arrAttchObj)
        {
            return TaskManager.Instance.InsertUnassignedEntityTaskWithAttachments(this, parentEntityID, taskTypeID, entitytypeid, TaskName, TaskList, TaskAttachments, TaskFiles, taskMembers, entityattributedata, AdminTaskChkLst, arrAttchObj);

        }
        public ITaskLibraryTemplateHolder DuplicateEntityTaskList(int id, int entityID)
        {
            return TaskManager.Instance.DuplicateEntityTaskList(this, id, entityID);

        }

        public IList GetMytasks(int FilterByentityID, int[] FilterStatusID, int pageNo, int AssignRole)
        {
            return TaskManager.Instance.GetMytasks(this, FilterByentityID, FilterStatusID, pageNo, AssignRole);
        }

        public IList<IMyTaskCollection> GetMytasksAPI(int FilterByentityID, int[] FilterStatusID, int StartRowno, int MaxRowNo, int AssignRole, int UserId)
        {
            return TaskManager.Instance.GetMytasksAPI(this, FilterByentityID, FilterStatusID, StartRowno, MaxRowNo, AssignRole, UserId);
        }
        public bool DeleteEntityTaskLis(int taskListID, int entityID)
        {
            return TaskManager.Instance.DeleteEntityTaskLis(this, taskListID, entityID);
        }

        public Tuple<int, IEntityTask> DuplicateEntityTask(int taskId, int entityID = 0)
        {
            return TaskManager.Instance.DuplicateEntityTask(this, taskId, entityID);
        }

        public int DeleteEntityTask(int taskID, int entityID)
        {
            return TaskManager.Instance.DeleteEntityTask(this, taskID, entityID);
        }

        public Tuple<int, IEntityTask> CompleteUnnassignedEntityTask(int taskId)
        {
            return TaskManager.Instance.CompleteUnnassignedEntityTask(this, taskId);
        }

        public Tuple<bool, int, string> UpdatetasktoNotApplicableandUnassigned(int taskID)
        {
            return TaskManager.Instance.UpdatetasktoNotApplicableandUnassigned(this, taskID);
        }

        public bool DeleteAdminTask(int taskID)
        {
            return TaskManager.Instance.DeleteAdminTask(this, taskID);
        }

        public bool DeleteTemplateConditionById(int templateCondID)
        {
            return TaskManager.Instance.DeleteTemplateConditionById(this, templateCondID);
        }

        public bool DeleteAdminTemplateTaskRelationById(int TaskListId, int TemplateId)
        {
            return TaskManager.Instance.DeleteAdminTemplateTaskRelationById(this, TaskListId, TemplateId);
        }

        public bool DeleteTaskMemberById(int id, int taskID)
        {
            return TaskManager.Instance.DeleteTaskMemberById(this, id, taskID);
        }

        public bool CopyFileFromTaskToEntityAttachments(int entityID, int ActiveFileID)
        {
            return TaskManager.Instance.CopyFileFromTaskToEntityAttachments(this, entityID, ActiveFileID);
        }

        public void UpdateTaskTemplateCriteria(string TemplateCriteriaText, int TemplateID)
        {
            TaskManager.Instance.UpdateTaskTemplateCriteria(this, TemplateCriteriaText, TemplateID);
        }

        public bool UpdatetaskAttachmentDescription(int id, string friendlyName, string description)
        {
            return TaskManager.Instance.UpdatetaskAttachmentDescription(this, id, friendlyName, description);
        }
        public bool UpdatetaskLinkDescription(int id, string friendlyName, string description, string URL, int linktype)
        {
            return TaskManager.Instance.UpdatetaskLinkDescription(this, id, friendlyName, description, URL, linktype);
        }


        public bool DeleteFileByID(int ID)
        {
            return TaskManager.Instance.DeleteFileByID(this, ID);
        }
        public int InsertLink(int EntityID, string Name, string URL, int linkType, string Description, int ActiveVersionNo, int TypeID, string CreatedOn, int OwnerID, int ModuleID)
        {
            return TaskManager.Instance.InsertLink(this, EntityID, Name, URL, linkType, Description, ActiveVersionNo, TypeID, CreatedOn, OwnerID, ModuleID);
        }
        public int InsertLinkInAdminTasks(int EntityID, string Name, string URL, int LinkType, string Description, int ActiveVersionNo, int TypeID, string CreatedOn, int OwnerID, int ModuleID)
        {
            return TaskManager.Instance.InsertLinkInAdminTasks(this, EntityID, Name, URL, LinkType, Description, ActiveVersionNo, TypeID, CreatedOn, OwnerID, ModuleID);
        }


        public bool SendReminderNotification(int taskmemberid, int taskid)
        {
            return TaskManager.Instance.SendReminderNotification(this, taskmemberid, taskid);
        }
        public bool DeleteLinkByID(int ID)
        {
            return TaskManager.Instance.DeleteLinkByID(this, ID);
        }
        public bool UpdatetaskEntityTaskDetails(int TaskID, string taskName, string description, string note, string Duedate, string taskaction, int entityID = 0)
        {
            return TaskManager.Instance.UpdatetaskEntityTaskDetails(this, TaskID, taskName, description, note, Duedate, taskaction, entityID);
        }
        public bool UpdatetaskEntityTaskDueDate(int TaskID, string Duedate)
        {
            return TaskManager.Instance.UpdatetaskEntityTaskDueDate(this, TaskID, Duedate);
        }
        public IEntityTask GetEntityTaskDetails(int EntityTaskID)
        {
            return TaskManager.Instance.GetEntityTaskDetails(this, EntityTaskID);
        }

        public IAdminTaskCheckList AdminTaskCheckListService()
        {
            return TaskManager.Instance.AdminTaskCheckListService();
        }

        public IEntityTaskCheckList EntityTaskCheckListService()
        {
            return TaskManager.Instance.EntityTaskCheckListService();
        }

        public bool DeleteAdminTaskCheckListByID(int chkLstID)
        {

            return TaskManager.Instance.DeleteAdminTaskCheckListByID(this, chkLstID);
        }

        public bool UpdateTaskSortOrder(int TaskId, int taskListID, int sortorder)
        {
            return TaskManager.Instance.UpdateTaskSortOrder(this, TaskId, taskListID, sortorder);
        }

        public bool UpdateEntityTaskSortOrder(JArray SortOrderObject)
        {
            return TaskManager.Instance.UpdateEntityTaskSortOrder(this, SortOrderObject);
        }

        public IList<IEntityTaskCheckList> getTaskchecklist(int TaskId)
        {
            return TaskManager.Instance.getTaskchecklist(this, TaskId);
        }

        public bool InsertTaskCheckList(int taskID, string CheckListName, int sortOrder)
        {
            return TaskManager.Instance.InsertTaskCheckList(this, taskID, CheckListName, sortOrder);
        }
        /// <summary>
        /// Check the task checklist
        /// </summary>
        /// <param name="proxy">ID</param>
        /// <returns>bool</returns>
        public bool ChecksTaskCheckList(int Id, bool Status)
        {
            return TaskManager.Instance.ChecksTaskCheckList(this, Id, Status);
        }

        public bool DeleteEntityCheckListByID(int chkLstID)
        {

            return TaskManager.Instance.DeleteEntityCheckListByID(this, chkLstID);
        }

        public IList<IEntitySublevelTaskHolder> GetSublevelTaskList(int entityID)
        {
            return TaskManager.Instance.GetSublevelTaskList(this, entityID);
        }

        public IList<IMyTaskCollection> GetEntityUpcomingTaskList(int FilterByentityID, int FilterStatusID, int EntityID, bool IsChildren)
        {
            return TaskManager.Instance.GetEntityUpcomingTaskList(this, FilterByentityID, FilterStatusID, EntityID, IsChildren);
        }
        public bool CopyAttachmentsfromtask(int[] fileids, int taskid)
        {

            return TaskManager.Instance.CopyAttachmentsfromtask(this, fileids, taskid);
        }


        public bool CopyAttachmentsfromtaskToExistingTasks(int[] TaskIDList, int FileID, string filetype)
        {

            return TaskManager.Instance.CopyAttachmentsfromtaskToExistingTasks(this, TaskIDList, FileID, filetype);
        }

        public bool CopyAttachmentsfromtask(int[] fileids, int taskid, int[] linkids)
        {

            return TaskManager.Instance.CopyAttachmentsfromtask(this, fileids, taskid, linkids);
        }


        public bool copytogeneralattachment(int[] taskid, int fileid)
        {
            return TaskManager.Instance.copytogeneralattachment(this, taskid, fileid);
        }

        public bool copytogeneralattachment(int fileid, int taskid, int linkids)
        {
            return TaskManager.Instance.copytogeneralattachment(this, fileid, taskid, linkids);
        }

        public bool DeleteLinkById(int Id)
        {
            return TaskManager.Instance.DeleteLinkByID(this, Id);
        }

        public bool EnableDisable(bool status)
        {
            return TaskManager.Instance.EnableDisable(this, status);
        }
        public bool gettaskflagstatus()
        {
            return TaskManager.Instance.gettaskflagstatus(this);
        }



        public bool DeleteTaskFileByid(int ID, int EntityId)
        {
            return TaskManager.Instance.DeleteTaskFileByid(this, ID, EntityId);
        }


        public bool DeleteTaskLinkByid(int ID, int EntityId)
        {
            return TaskManager.Instance.DeleteTaskLinkByid(this, ID, EntityId);
        }


        //public bool DeleteTaskLinkByid(int ID)
        //{
        //    throw new NotImplementedException();
        //}

        /// <summary>
        /// DeleteLinkByID.
        /// </summary>
        /// <param name="proxy">ID Parameter</param>
        /// <returns>bool</returns>
        public bool DeleteAdminTaskLinkByID(int ID)
        {
            return TaskManager.Instance.DeleteAdminTaskLinkByID(this, ID);
        }

        public int InsertUpdateEntityTaskCheckList(int Id, int taskId, String CheckListName, bool ChkListStatus, bool ISowner, int sortOrder, bool IsNew, int entityID = 0)
        {
            return TaskManager.Instance.InsertUpdateEntityTaskCheckList(this, Id, taskId, CheckListName, ChkListStatus, ISowner, sortOrder, IsNew, entityID);
        }
        public IList<ITaskLibraryTemplateHolder> GetEntityTaskListWithoutTasks(int entityID)
        {
            return TaskManager.Instance.GetEntityTaskListWithoutTasks(this, entityID);
        }
        public IList<IEntityTask> GetEntityTaskListDetails(int entityID, int taskListID)
        {
            return TaskManager.Instance.GetEntityTaskListDetails(this, entityID, taskListID);
        }
        public int GettaskCountByStatus(int tasklistID, int EntityID, int[] status)
        {
            return TaskManager.Instance.GettaskCountByStatus(this, tasklistID, EntityID, status);
        }

        public Tuple<bool, IList<ITaskMembers>> InsertUnAssignedTaskMembers(int EntityID, int taskID, IList<ITaskMembers> TaskMembers, ITaskMembers TaskOwner, IList<ITaskMembers> GlobalTaskMembers)
        {
            return TaskManager.Instance.InsertUnAssignedTaskMembers(this, EntityID, taskID, TaskMembers, TaskOwner, GlobalTaskMembers);

        }
        public bool UpdatetaskAdminAttachmentDescription(int id, string friendlyName, string description)
        {
            return TaskManager.Instance.UpdatetaskAdminAttachmentDescription(this, id, friendlyName, description);
        }
        public bool UpdatetaskAdminLinkDescription(int id, string friendlyName, string description, string URL, int LinkType)
        {
            return TaskManager.Instance.UpdatetaskAdminLinkDescription(this, id, friendlyName, description, URL, LinkType);
        }

        public IList<ITaskLibraryTemplateHolder> GetExistingEntityTasksByEntityID(int entityID)
        {
            return TaskManager.Instance.GetExistingEntityTasksByEntityID(this, entityID);
        }
        public IList GetMyFundingRequests(int[] FilterStatusID, int AssignRole)
        {
            return TaskManager.Instance.GetMyFundingRequests(this, FilterStatusID, AssignRole);
        }
        public IList FetchUnassignedTaskforReassign(int entityId, int taskListId)
        {
            return TaskManager.Instance.FetchUnassignedTaskforReassign(this, entityId, taskListId);
        }

        public IList<IEntityTask> AssignUnassignTasktoMembers(int entityId, int taskListId, int[] taskCollection, int[] memberCollection, DateTime? dueDate)
        {
            return TaskManager.Instance.AssignUnassignTasktoMembers(this, entityId, taskListId, taskCollection, memberCollection, dueDate);
        }

        public IList GetAdminTasksById(string[] tasklistArr)
        {
            return TaskManager.Instance.GetAdminTasksById(this, tasklistArr);
        }
        public IList<IAdminTaskCheckList> getAdminTaskchecklist(int TaskId)
        {
            return TaskManager.Instance.getAdminTaskchecklist(this, TaskId);
        }

        public IList<IMilestoneMetadata> GetAdminTaskMetadatabyTaskID(int taskID)
        {
            return TaskManager.Instance.GetAdminTaskMetadatabyTaskID(this, taskID);
        }

        public IList<ITasktemplateCondition> GetTaskTemplateConditionByTaskTempId(int TaskTempID)
        {
            return TaskManager.Instance.GetTaskTemplateConditionByTaskTempId(this, TaskTempID);
        }

        public IList<ITaskLibraryTemplateHolder> GetFiltertaskCountByStatus(Dictionary<int, int> Maintask, int[] filter)
        {
            return TaskManager.Instance.GetFiltertaskCountByStatus(this, Maintask, filter);
        }

        public IList<ITaskLibraryTemplateHolder> GetTemplateAdminTaskList()
        {
            return TaskManager.Instance.GetTemplateAdminTaskList(this);
        }

        public bool UpdateEntityTask(params object[] CollectionIds)
        {
            return TaskManager.Instance.UpdateEntityTask(this, CollectionIds);
        }

        public Tuple<IList<SourceDestinationMember>, IList<SourceDestinationMember>, bool> GetSourceToDestinationmembers(int taskID, int sourceEntityID, int destinationEntityId)
        {
            return TaskManager.Instance.GetSourceToDestinationmembers(this, taskID, sourceEntityID, destinationEntityId);
        }

        public IList<IEntityTypeRoleAcl> GetDestinationEntityIdRoleAccess(int EntityID)
        {
            return TaskManager.Instance.GetDestinationEntityIdRoleAccess(this, EntityID);
        }

        public bool InsertUpdateDragTaskMembers(IList<SourceDestinationMember> TaskMemberslst, IList<SourceDestinationMember> EntitytaskMemberLst, int SourceTaskID, int TargetTasklistID, int TargetEntityID)
        {
            return TaskManager.Instance.InsertUpdateDragTaskMembers(this, TaskMemberslst, EntitytaskMemberLst, SourceTaskID, TargetTasklistID, TargetEntityID);
        }

        public bool UpdateDragEntityTaskListByTask(int TaskID, int SrcTaskListID, int TargetTaskListID)
        {
            return TaskManager.Instance.UpdateDragEntityTaskListByTask(this, TaskID, SrcTaskListID, TargetTaskListID);
        }
        /// <summary>
        /// Gets the attributes details by entityID.
        /// </summary>
        /// <param name="Id">The entityId.</param>
        /// <returns>
        /// Ilist
        /// </returns>
        public IList<IAttributeData> GetEntityAttributesDetails(int entityId)
        {
            return TaskManager.Instance.GetEntityAttributesDetails(this, entityId);
        }
        public IList GetEntityTaskAttachmentinfo(int TaskId)
        {
            return TaskManager.Instance.GetEntityTaskAttachmentinfo(this, TaskId);
        }

        /// <summary>
        /// Get member.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns>IList of IEntityRoleUser</returns>
        public IList<ITaskMembers> GetTaskMember(int taskID)
        {
            return TaskManager.Instance.GetTaskMember(this, taskID);
        }


        /// Updating proof Task status 
        /// </summary>
        ///  <param name="proxy"></param>
        /// <param name="entityId">The TaskID</param>
        /// <param name="status">The Status</param>
        /// <returns>True or False</returns>
        public Tuple<bool, int, string> UpdateProofTaskStatus(int taskID, int status, int userid)
        {
            return TaskManager.Instance.UpdateProofTaskStatus(this, taskID, status, userid);
        }

        /// <summary>
        /// Getting Task details
        /// </summary>
        /// <param name="proxy"></param>
        /// <param name="entityID">The entityID</param>
        /// <param name="taskListID">The taskListID</param>
        /// <param name="pageNo">The pageNo</param>
        /// <returns>IList</returns>
        public IList GetEntityTaskCollection(int entityID, int taskListID, int pageNo)
        {
            return TaskManager.Instance.GetEntityTaskCollection(this, entityID, taskListID, pageNo);
        }
    }
}
