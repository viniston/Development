using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using BrandSystems.Marcom.Core.Core.Managers.Proxy;
using BrandSystems.Marcom.Core.Interface;
using BrandSystems.Marcom.Core.Interface.Managers;
using BrandSystems.Marcom.Core.Task;
using BrandSystems.Marcom.Core.Task.Interface;
using BrandSystems.Marcom.Dal.Task.Model;
using BrandSystems.Marcom.Dal.Planning.Model;
using System.Collections;
using BrandSystems.Marcom.Core.Common.Interface;
using BrandSystems.Marcom.Dal.Common.Model;
using System.Web;
using System.IO;
using System.Xml.Linq;
using BrandSystems.Marcom.Core.Planning.Interface;
using BrandSystems.Marcom.Dal.Metadata.Model;
using BrandSystems.Marcom.Dal.Access.Model;
using BrandSystems.Marcom.Core.Planning;
using BrandSystems.Marcom.Dal.Base;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using BrandSystems.Marcom.Core.Common;
using BrandSystems.Marcom.Core.Utility;
using BrandSystems.Marcom.Dal.User.Model;
using Mail;
using System.Threading.Tasks;
using BrandSystems.Marcom.Core.Access.Interface;
using BrandSystems.Marcom.Core.Access;
using BrandSystems.Marcom.Dal.DAM.Model;
using BrandSystems.Marcom.Core.DAM.Interface;
using BrandSystems.Marcom.Core.DAM;
using BrandSystems.Marcom.Core.com.proofhq.www;


namespace BrandSystems.Marcom.Core.Managers
{
    internal partial class TaskManager : IManager
    {
        /// <summary>
        /// The instance
        /// </summary>
        private static TaskManager instance = new TaskManager();

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        internal static TaskManager Instance
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
        /// Returns Task class.
        /// </summary>
        public IAdminTask TasksService()
        {
            return new BrandSystems.Marcom.Core.Task.AdminTask();
        }

        /// <summary>
        /// Returns Task class.
        /// </summary>
        public IEntityTask EntityTasksService()
        {
            return new BrandSystems.Marcom.Core.Task.EntityTask();
        }

        /// <summary>
        /// Returns Task class.
        /// </summary>
        public ITaskMembers EntityTaskMembersService()
        {
            return new BrandSystems.Marcom.Core.Task.TaskMembers();
        }

        /// <summary>
        /// Returns TaskAttachment class.
        /// </summary>
        public INewTaskAttachments TasksAttachmentService()
        {
            return new NewTaskAttachments();
        }


        public IAttachments EntityTasksAttachmentService()
        {
            return new Attachments();
        }


        public IAdminTaskCheckList AdminTaskCheckListService()
        {
            return new AdminTaskCheckList();
        }

        public IEntityTaskCheckList EntityTaskCheckListService()
        {
            return new EntityTaskCheckList();
        }

        public void test(TaskManagerProxy proxy)
        {
            //string str = "rajkumar";

        }

        public Tuple<int, ITaskList> InsertUpdateTaskList(TaskManagerProxy proxy, int id, string caption, string description, int sortorder)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    IList<ITaskList> itask = new List<ITaskList>();
                    TaskListDao tasklistdao = new TaskListDao();
                    string newSortOrder = "SELECT COUNT(*)+1 AS SortOrder FROM TM_TaskList ";
                    IList sortOrderVal = tx.PersistenceManager.PlanningRepository.ExecuteQuery(newSortOrder);
                    int sortOrderID = (int)((System.Collections.Hashtable)(sortOrderVal)[0])["SortOrder"];

                    if (id > 0)
                    {
                        tasklistdao.ID = id;
                    }
                    tasklistdao.Caption = (caption.Trim().Length > 0 ? caption : tasklistdao.Caption);
                    tasklistdao.Description = (description.Trim().Length > 0 ? description : tasklistdao.Description);
                    tasklistdao.Sortorder = (sortOrderID > 0 ? sortOrderID : tasklistdao.Sortorder);
                    tx.PersistenceManager.TaskRepository.Save<TaskListDao>(tasklistdao);
                    tx.Commit();
                    ITaskList itsklst = new TaskList();
                    itsklst.Caption = HttpUtility.HtmlDecode(tasklistdao.Caption);
                    itsklst.Id = tasklistdao.ID;
                    itsklst.Description = HttpUtility.HtmlDecode(tasklistdao.Description);
                    itsklst.SortOder = tasklistdao.Sortorder;
                    Tuple<int, ITaskList> taskObj = Tuple.Create(tasklistdao.ID, itsklst);

                    return taskObj;
                }
            }
            catch
            {
                return null;
            }

        }


        public bool DeleteSystemTaskList(TaskManagerProxy proxy, int id)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    tx.PersistenceManager.TaskRepository.DeleteByID<TempTaskListDao>(TempTaskListDao.PropertyNames.TaskListID, id);
                    tx.PersistenceManager.TaskRepository.DeleteByID<AdminTaskDao>(AdminTaskDao.PropertyNames.TaskListID, id);
                    tx.PersistenceManager.TaskRepository.DeleteByID<TaskListDao>(id);
                    tx.Commit();
                    return true;
                }
            }
            catch
            {

            }
            return false;
        }

        public IList<ITaskLibraryTemplateHolder> GetTaskList(TaskManagerProxy proxy)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    IList<ITaskLibraryTemplateHolder> tasklist = new List<ITaskLibraryTemplateHolder>();
                    var adminTaskList = tx.PersistenceManager.PlanningRepository.GetAll<TaskListDao>().OrderBy(a => a.Sortorder);

                    foreach (var res in adminTaskList)
                    {
                        ITaskLibraryTemplateHolder tskLst = new TaskLibraryTemplateHolder();
                        tskLst.LibraryName = res.Caption;
                        tskLst.LibraryDescription = res.Description;
                        tskLst.ID = res.ID;
                        tskLst.SortOrder = res.Sortorder;
                        IList<IAdminTask> iitaskDetails = new List<IAdminTask>();
                        //tskLst.TaskList = GetTaskListDetails(proxy, res.ID);
                        tskLst.TaskCount = tx.PersistenceManager.TaskRepository.Query<AdminTaskDao>().Where(a => a.TaskListID == res.ID).Count();
                        tskLst.TaskList = iitaskDetails;
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

        public int AddUpdateTaskFlag(TaskManagerProxy proxy, string caption, string colorcode, string description, int sortorderid, int id = 0)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {

                    TaskFlagDao dao = new TaskFlagDao();
                    if (id > 0)
                    {

                        dao.ID = id;
                    }
                    else
                    {
                        if (tx.PersistenceManager.TaskRepository.Query<TaskFlagDao>().ToList().Count > 0)
                        {
                            sortorderid = tx.PersistenceManager.TaskRepository.Query<TaskFlagDao>().ToList().Select(a => a.Sortorder).LastOrDefault();
                            sortorderid += 1;
                        }
                    }
                    dao.Caption = (caption.Trim().Length > 0 ? caption : dao.Caption);
                    dao.Description = (description.Trim().Length > 0 ? description : dao.Description);
                    dao.ColorCode = (colorcode.Trim().Length > 0 ? colorcode : dao.ColorCode);
                    dao.Sortorder = sortorderid;
                    tx.PersistenceManager.TaskRepository.Save<TaskFlagDao>(dao);
                    tx.Commit();
                    return dao.ID;
                }


            }
            catch (Exception ex)
            {
            }
            return 0;
        }

        public bool DeleteTaskFlagById(TaskManagerProxy proxy, int flagid)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    tx.PersistenceManager.TaskRepository.DeleteByID<TaskFlagDao>(flagid);
                    tx.Commit();
                    return true;
                }
            }
            catch (Exception ex)
            {

            }
            return false;
        }

        public Object GetTaskTypes(TaskManagerProxy proxy)
        {

            dynamic workTypeList = Enum.GetValues(typeof(TaskTypes)).Cast<TaskTypes>().Select(e => new { TaskType = e, Name = e.ToString().Replace("_", " ") });
            return workTypeList;
        }


        public bool updateTaskMemberFlag(TaskManagerProxy proxy, int taskid, string colorCode)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    TaskMembersDao memDao = new TaskMembersDao();
                    memDao = tx.PersistenceManager.TaskRepository.Query<TaskMembersDao>().Where(a => a.UserID == proxy.MarcomManager.User.Id && a.TaskID == taskid).Select(a => a).FirstOrDefault();
                    if (memDao != null)
                        memDao.FlagColorCode = colorCode;
                    tx.PersistenceManager.TaskRepository.Save<TaskMembersDao>(memDao);
                    tx.Commit();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// Getting Task details
        /// </summary>
        ///  <param name="proxy"></param>
        /// <param name="StepID">The TaskListID</param>
        /// <returns>IList of IAdminTask</returns>
        public IList<IAdminTask> GetTaskListDetails(TaskManagerProxy proxy, int taskListID)
        {
            IList<IAdminTask> iitaskDetails = new List<IAdminTask>();
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    var TaskList = (from task in tx.PersistenceManager.PlanningRepository.Query<AdminTaskDao>() where task.TaskListID == taskListID select task).OrderBy(a => a.Sortorder).ToList();
                    foreach (var val in TaskList)
                    {
                        var basedao = (from task in tx.PersistenceManager.PlanningRepository.Query<BaseEntityDao>() where task.Id == val.ID select task).FirstOrDefault();
                        var typedao = (from task in tx.PersistenceManager.PlanningRepository.Query<EntityTypeDao>() where task.Id == basedao.Typeid select task);
                        IAdminTask admintsk = new AdminTask();
                        admintsk.Caption = val.Caption;
                        admintsk.Description = val.Description;
                        admintsk.Id = val.ID;
                        admintsk.TaskTypeName = Enum.GetName(typeof(TaskTypes), val.TaskType).Replace("_", " ");
                        admintsk.TaskType = val.TaskType;
                        admintsk.SortOder = val.Sortorder;
                        admintsk.ColorCode = "";
                        admintsk.ShortDesc = "";
                        if (typedao != null)
                        {
                            admintsk.ColorCode = typedao.FirstOrDefault().ColorCode;
                            admintsk.ShortDesc = typedao.FirstOrDefault().ShortDescription;
                        }
                        admintsk.TaskListID = val.TaskListID;
                        var taskattachment = (from task in tx.PersistenceManager.TaskRepository.Query<AssetsDao>() where task.EntityID == val.ID select task).ToList();
                        //var taskattachment = (from task in tx.PersistenceManager.TaskRepository.Query<NewTaskAttachmentsDao>() where task.Entityid == val.ID select task).ToList();
                        //var taskLinks = (from task in tx.PersistenceManager.TaskRepository.Query<TaskLinksDao>() where task.EntityID == val.ID select task).ToList();
                        //admintsk.TotalTaskAttachment = taskattachment.Count() + taskLinks.Count();
                        if (taskattachment.Count() > 0)
                        {
                            admintsk.TotalTaskAttachment = taskattachment.Count();
                            var attachmentNames = taskattachment.Select(a => a.Name).ToArray();
                            admintsk.AttachmentCollection = string.Join<string>(" , ", attachmentNames);
                        }
                        else
                        {
                            admintsk.TotalTaskAttachment = 0;
                            admintsk.AttachmentCollection = "";
                        }
                        admintsk.AdminTaskCheckList = tx.PersistenceManager.TaskRepository.Query<AdminTaskCheckListDao>().Where(a => a.TaskId == val.ID).ToList<AdminTaskCheckListDao>();
                        admintsk.AttributeData = GetEntityAttributesDetails(proxy, val.ID);
                        iitaskDetails.Add(admintsk);
                    }

                }
                return iitaskDetails;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public int InsertUpdateTaskTemplateCondition(TaskManagerProxy proxy, int id, int tasktemplateID, int typeID, int sortorder, int attributeID, string value, int attributeLevel, int conditionType)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    TasktemplateConditionDao tempConditionDao = new TasktemplateConditionDao();
                    if (id != 0)
                    {
                        tempConditionDao.ID = id;
                    }
                    tempConditionDao.TaskTempID = tasktemplateID;
                    tempConditionDao.TypeID = typeID;
                    tempConditionDao.AttributeID = attributeID;
                    // tempConditionDao.AttributeTypeID = attributeTypeID;
                    tempConditionDao.AttributeLevel = attributeLevel;
                    tempConditionDao.Value = value;
                    tempConditionDao.ConditionType = conditionType;
                    tempConditionDao.Sortorder = sortorder;
                    tx.PersistenceManager.TaskRepository.Save<TasktemplateConditionDao>(tempConditionDao);
                    tx.Commit();
                    InsertingTaskTempConditionQuery(proxy, tasktemplateID);
                    return tempConditionDao.ID;
                }
            }
            catch
            {

            }
            return 0;
        }


        public void InsertingTaskTempConditionQuery(TaskManagerProxy proxy, int templateId)
        {
            StringBuilder tempCriteria = new StringBuilder();
            using (ITransaction tx = proxy.MarcomManager.GetTransaction())
            {
                var taskTempCondValResult = tx.PersistenceManager.TaskRepository.Query<TasktemplateConditionDao>().ToList().Where(a => a.TaskTempID == templateId).OrderBy(a => a.Sortorder);

                if (taskTempCondValResult.ToList().Count > 0)
                {
                    tempCriteria.Append(" ((SELECT COUNT(1) FROM #TEMP t where (");

                    bool IsChecked = false;
                    foreach (var val in taskTempCondValResult)
                    {
                        if (IsChecked == true)
                        {

                            if (val.ConditionType == 1)
                            {
                                tempCriteria.Append(" or ");
                            }
                            else if (val.ConditionType == 2)
                            {
                                tempCriteria.Append(" and ");
                            }
                        }
                        if (val.TypeID != 0)
                        {
                            tempCriteria.Append("( ");
                            tempCriteria.Append(" t.EntityTypeId=" + val.TypeID + " ");
                            if (val.AttributeID != 0)
                            {
                                tempCriteria.Append(" and t.AttributeID=" + val.AttributeID + " ");
                                if (val.Value.Length != 0)
                                {
                                    //tempCriteria.Append(" and  t.AttributeValue in(" + val.Value + ")  and t.AttributeLevel=" + val.AttributeLevel + " ");
                                    tempCriteria.Append(" and  t.AttributeValue in(" + val.Value + ")");
                                }

                            }

                        }
                        tempCriteria.Append(" ) ");
                        IsChecked = true;

                    }
                    tempCriteria.Append(" )) >0) ");
                    tx.PersistenceManager.TaskRepository.ExecuteQuerywithMinParam(" UPDATE TM_TaskTemplate SET Tempcriteria= ? WHERE Id = ?", tempCriteria.ToString(), templateId);
                    tx.Commit();
                }
            }

        }



        public IList<ITaskTemplate> GetTaskTemplateDetails(TaskManagerProxy proxy)
        {
            IList<ITaskTemplate> taskTempdetail = new List<ITaskTemplate>();
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    //var tasktemDetldao= tx.PersistenceManager.TaskRepository.GetAll<TasktemplateConditionDao>().Cast<TasktemplateCondition>().ToList();
                    var tasktemDetldao = tx.PersistenceManager.TaskRepository.Query<TaskTemplateDao>();
                    foreach (var val in tasktemDetldao)
                    {
                        //ITasktemplateCondition tempcond = new TasktemplateCondition();
                        //tempcond.AttributeID = val.AttributeID;
                        //tempcond.AttributeLevel = val.AttributeLevel;
                        //tempcond.AttributeTypeID = val.AttributeTypeID;
                        //tempcond.ConditionType = val.ConditionType;
                        //tempcond.Id = val.ID;
                        //tempcond.SortOder = val.Sortorder;
                        //tempcond.TypeID = val.TypeID;
                        //tempcond.TaskTempID = val.TaskTempID;
                        //tempcond.Value = val.Value;

                        ITaskTemplate tasktemp = new TaskTemplate();
                        tasktemp.Id = val.ID;
                        tasktemp.Name = val.Name;
                        tasktemp.Description = val.Description;
                        tasktemp.TemplateCriteria = (val.TemplateCriteriaText != null && val.TemplateCriteriaText.Length > 0 ? val.TemplateCriteriaText : "");

                        var taskListresult = from res1 in tx.PersistenceManager.TaskRepository.Query<TempTaskListDao>()
                                             join res2 in tx.PersistenceManager.TaskRepository.Query<TaskListDao>()
                                             on res1.TaskListID equals res2.ID
                                             orderby res1.Sortorder
                                             where res1.TempID == val.ID
                                             select new { res2 };
                        if (taskListresult != null)
                        {

                            IList<ITaskList> taskListlst = new List<ITaskList>();
                            foreach (var taskdetl in taskListresult.ToList())
                            {
                                int totaltaskspresent = 0;

                                totaltaskspresent = tx.PersistenceManager.TaskRepository.Query<AdminTaskDao>().ToList().Where(a => a.TaskListID == taskdetl.res2.ID).Count();
                                taskListlst.Add(new TaskList { Id = taskdetl.res2.ID, Caption = taskdetl.res2.Caption, Description = taskdetl.res2.Description, SortOder = taskdetl.res2.Sortorder, TotalTasks = totaltaskspresent });
                            }
                            tasktemp.TaskListDetails = taskListlst;
                        }

                        tasktemp.TempCondDetails = null;
                        taskTempdetail.Add(tasktemp);
                    }


                }

            }
            catch
            {

            }
            return taskTempdetail;

        }


        public IList<ITasktemplateCondition> GetTaskTemplateConditionByTaskTempId(TaskManagerProxy proxy, int TaskTempID)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    var temptempConddetailresult = tx.PersistenceManager.TaskRepository.Query<TasktemplateConditionDao>().Where(a => a.TaskTempID == TaskTempID);
                    if (temptempConddetailresult != null)
                    {
                        IList<ITasktemplateCondition> tempCondlst = new List<ITasktemplateCondition>();
                        foreach (var tasktempconddetl in temptempConddetailresult)
                        {

                            ITasktemplateCondition tempcond = new TasktemplateCondition();
                            tempcond.AttributeID = tasktempconddetl.AttributeID;
                            tempcond.AttributeLevel = tasktempconddetl.AttributeLevel;
                            tempcond.AttributeTypeID = tasktempconddetl.AttributeTypeID;
                            tempcond.ConditionType = tasktempconddetl.ConditionType;
                            tempcond.Id = tasktempconddetl.ID;
                            tempcond.SortOder = tasktempconddetl.Sortorder;
                            tempcond.TypeID = tasktempconddetl.TypeID;
                            tempcond.TaskTempID = tasktempconddetl.TaskTempID;
                            tempcond.Value = tasktempconddetl.Value;
                            tempCondlst.Add(tempcond);
                        }
                        return tempCondlst;

                    }
                }


            }
            catch
            {

            }
            return null;
        }



        public bool UpdateTaskListSortOrder(TaskManagerProxy proxy, int id, int sortorder)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    TaskListDao tasklistdao = new TaskListDao();
                    tasklistdao = (from item in tx.PersistenceManager.TaskRepository.Query<TaskListDao>() where item.ID == id select item).FirstOrDefault();
                    if (tasklistdao != null)
                    {
                        tasklistdao.Sortorder = sortorder;
                    }
                    tx.PersistenceManager.TaskRepository.Save<TaskListDao>(tasklistdao);
                    tx.Commit();
                    return true;
                }
            }
            catch
            {
                return false;
            }

        }

        public Tuple<int, IAdminTask> InsertTaskWithAttachments(TaskManagerProxy proxy, int taskTypeID, int entitytypeid, string TaskName, IList<IAdminTask> TaskList, IList<INewTaskAttachments> TaskAttachments, IList<IFile> TaskFiles, IList<IAttributeData> entityattributedata, IList<IAdminTaskCheckList> AdminTaskChkLst, JArray arrAttchObj)
        {
            try
            {
                BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("-----------------------------------------------------------------------------------------------", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);
                BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("Started creating Task", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);
                int entityId = 0;

                IAdminTask iadmtsk = new AdminTask();
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    IList<AdminTaskDao> iTask = new List<AdminTaskDao>();
                    entityId = GetBaseEntityID(entitytypeid, TaskName, tx, 0, true, false);

                    if (TaskList != null)
                    {

                        foreach (var a in TaskList)
                        {
                            string newSortOrder = "SELECT COUNT(*)+1 AS SortOrder FROM TM_Admin_Task WHERE TaskListID= ? ";
                            IList sortOrderVal = tx.PersistenceManager.PlanningRepository.ExecuteQuerywithMinParam(newSortOrder, a.TaskListID);
                            int sortOrderID = (int)((System.Collections.Hashtable)(sortOrderVal)[0])["SortOrder"];

                            AdminTaskDao taskdao = new AdminTaskDao();
                            taskdao.Caption = HttpUtility.HtmlEncode(a.Caption);
                            taskdao.TaskListID = a.TaskListID;
                            taskdao.Description = HttpUtility.HtmlEncode(a.Description);
                            taskdao.TaskType = a.TaskType;
                            taskdao.Sortorder = sortOrderID;
                            taskdao.ID = entityId;
                            iTask.Add(taskdao);

                            iadmtsk.Caption = a.Caption;
                            iadmtsk.TaskListID = a.TaskListID;
                            iadmtsk.Description = a.Description;
                            iadmtsk.TaskType = a.TaskType;
                            iadmtsk.TaskTypeName = Enum.GetName(typeof(TaskTypes), a.TaskType).Replace("_", " ");
                            iadmtsk.SortOder = 1;
                            var fileNames = TaskAttachments.Select(at => at.Name).ToArray();
                            iadmtsk.AttachmentCollection = string.Join<string>(" , ", fileNames);

                        }
                        tx.PersistenceManager.PlanningRepository.Save<AdminTaskDao>(iTask);
                        iadmtsk.Id = entityId;
                        if (AdminTaskChkLst != null)
                        {
                            IList<AdminTaskCheckListDao> taskChklst = new List<AdminTaskCheckListDao>();
                            for (int i = 0; i <= AdminTaskChkLst.Count() - 1; i++)
                            {
                                if (AdminTaskChkLst[i].NAME.Trim() != "")
                                    taskChklst.Add(new AdminTaskCheckListDao { NAME = AdminTaskChkLst[i].NAME, TaskId = iadmtsk.Id, SortOrder = (i + 1) });

                            }
                            if (taskChklst.Count > 0)
                                tx.PersistenceManager.PlanningRepository.Save<AdminTaskCheckListDao>(taskChklst);
                            iadmtsk.AdminTaskCheckList = taskChklst;
                        }


                        BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("Saved in Task", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);
                    }

                    //TaskAttachment now Dam  Asset onely 
                    //IList<TaskLinksDao> iTaskLink = new List<TaskLinksDao>();
                    //if (arrAttchObj.Count > 0)
                    //{

                    //    foreach (var data in arrAttchObj)
                    //    {
                    //        string ext = (string)data["Extension"];
                    //        if (ext == "Link")
                    //        {
                    //            Guid NewId = Guid.NewGuid();
                    //            TaskLinksDao lnks = new TaskLinksDao();
                    //            lnks.Name = (string)data["FileName"];
                    //            string LinkURL = (string)data["URL"];
                    //            lnks.LinkType = (int)data["LinkType"];
                    //            lnks.URL = LinkURL;
                    //            lnks.Description = (string)data["FileDescription"];
                    //            lnks.CreatedOn = DateTime.Now.ToString();
                    //            lnks.EntityID = entityId;
                    //            lnks.ModuleID = 1;
                    //            lnks.OwnerID = proxy.MarcomManager.User.Id;
                    //            lnks.ActiveVersionNo = 1;
                    //            lnks.LinkGuid = NewId;
                    //            iTaskLink.Add(lnks);


                    //        }
                    //    }
                    //    tx.PersistenceManager.CommonRepository.Save<TaskLinksDao>(iTaskLink);
                    //    BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("Saved in Links", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);
                    //}



                    //if (TaskFiles != null)
                    //{
                    //    IList<TaskFileDao> ifile = new List<TaskFileDao>();
                    //    if (TaskFiles != null)
                    //    {

                    //        foreach (var a in TaskFiles)
                    //        {
                    //            Guid NewId = Guid.NewGuid();
                    //            string filePath = ReadAdminXML("FileManagment");
                    //            var DirInfo = System.IO.Directory.GetParent(filePath);
                    //            string newFilePath = DirInfo.FullName;
                    //            System.IO.File.Move(filePath + "\\" + a.strFileID.ToString() + a.Extension, newFilePath + "\\" + NewId + a.Extension);
                    //            TaskFileDao fldao = new TaskFileDao();
                    //            fldao.Checksum = a.Checksum;
                    //            fldao.CreatedOn = a.CreatedOn;
                    //            fldao.Entityid = entityId;
                    //            fldao.Extension = a.Extension;
                    //            fldao.MimeType = a.MimeType;
                    //            fldao.Moduleid = a.Moduleid;
                    //            fldao.Name = a.Name;
                    //            fldao.Ownerid = a.Ownerid;
                    //            fldao.Size = a.Size;
                    //            fldao.VersionNo = a.VersionNo;
                    //            fldao.Fileguid = NewId;
                    //            fldao.Description = a.Description;
                    //            ifile.Add(fldao);
                    //        }
                    //        tx.PersistenceManager.PlanningRepository.Save<TaskFileDao>(ifile);
                    //        BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("Saved in File", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);
                    //    }

                    //    iadmtsk.TotalTaskAttachment = ifile.Count + iTaskLink.Count;

                    //    if (ifile != null)
                    //    {

                    //        IList<NewTaskAttachmentsDao> iattachment = new List<NewTaskAttachmentsDao>();
                    //        foreach (var a in ifile)
                    //        {
                    //            NewTaskAttachmentsDao attachedao = new NewTaskAttachmentsDao();
                    //            attachedao.ActiveFileid = a.Id;
                    //            attachedao.ActiveVersionNo = 1;
                    //            attachedao.Createdon = DateTime.Now;
                    //            attachedao.Entityid = entityId;
                    //            attachedao.Name = a.Name;
                    //            attachedao.Typeid = taskTypeID;
                    //            iattachment.Add(attachedao);
                    //        }
                    //        tx.PersistenceManager.PlanningRepository.Save<NewTaskAttachmentsDao>(iattachment);
                    //        BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("Saved in Task Members", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);

                    //    }

                    //}
                    var taskattachment = (from task in tx.PersistenceManager.TaskRepository.Query<AssetsDao>() where task.EntityID == entityId select task).ToList();
                    if (taskattachment.Count > 0)
                    {
                        iadmtsk.TotalTaskAttachment = taskattachment.Count();

                    }
                    else { iadmtsk.TotalTaskAttachment = 0; }
                    if (entityattributedata != null)
                    {
                        var result = InsertEntityAttributes(tx, entityattributedata, entityId, entitytypeid);
                    }
                    tx.Commit();
                    iadmtsk.AttributeData = GetEntityAttributesDetails(proxy, entityId);
                }
                Tuple<int, IAdminTask> taskObj = Tuple.Create(entityId, iadmtsk);
                return taskObj;
            }
            catch (Exception ex)
            {

                return null;
            }


        }

        public int InsertTempTaskList(TaskManagerProxy proxy, int templateId, int tasklistId, int SortOrder)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {

                    TempTaskListDao temptaskdao = new TempTaskListDao();
                    temptaskdao.TempID = templateId;
                    temptaskdao.TaskListID = tasklistId;
                    temptaskdao.Sortorder = SortOrder;
                    tx.PersistenceManager.TaskRepository.Save<TempTaskListDao>(temptaskdao);
                    tx.Commit();
                    return temptaskdao.ID;
                }
            }
            catch
            {

            }
            return 0;
        }


        public int InsertUpdateTemplate(TaskManagerProxy proxy, int ID, string caption, string description)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {

                    TaskTemplateDao temptaskdao = new TaskTemplateDao();
                    if (ID != 0)
                    {
                        temptaskdao.ID = ID;
                    }
                    temptaskdao.Name = (caption.Length > 0 ? caption : temptaskdao.Name);
                    temptaskdao.Description = description != null ? (description.Length > 0 ? description : temptaskdao.Description) : "";
                    tx.PersistenceManager.TaskRepository.Save<TaskTemplateDao>(temptaskdao);
                    tx.Commit();
                    return temptaskdao.ID;
                }
            }
            catch
            {

            }
            return 0;
        }

        public bool DeleteTaskTemplateListById(TaskManagerProxy proxy, int templateID)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    tx.PersistenceManager.TaskRepository.DeleteByID<TasktemplateConditionDao>(TasktemplateConditionDao.PropertyNames.TaskTempID, templateID);
                    tx.PersistenceManager.TaskRepository.DeleteByID<TempTaskListDao>(TempTaskListDao.PropertyNames.TempID, templateID);
                    tx.PersistenceManager.TaskRepository.DeleteByID<TaskTemplateDao>(templateID);
                    tx.Commit();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private string ReadAdminXML(string elementNode)
        {

            string xmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
            XDocument xDoc = XDocument.Load(xmlpath);
            if (xDoc.Root.Elements(elementNode).Count() > 0)
            {
                return (string)xDoc.Root.Elements(elementNode).FirstOrDefault().Elements("FolderPath").First();
            }
            string uploadImagePath = Path.Combine(HttpRuntime.AppDomainAppPath);
            return uploadImagePath = uploadImagePath + "UploadedImages\\";
        }

        //public bool InsertEntityAttributes(ITransaction tx, IList<IAttributeData> attributeData, int entityId, int typeId)
        //{
        //    if (attributeData != null)
        //    {
        //        BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("Started inseting values into Dynamic tables", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);

        //        string entityName = "AttributeRecord" + typeId + "_V" + MarcomManagerFactory.ActiveMetadataVersionNumber;
        //        IList<IDynamicAttributes> listdynamicattributes = new List<IDynamicAttributes>();
        //        Dictionary<string, object> dictAttr = new Dictionary<string, object>();
        //        IList<MultiSelectDao> listMultiselect = new List<MultiSelectDao>();
        //        IList<TreeValueDao> listreeval = new List<TreeValueDao>();
        //        listreeval.Clear();
        //        DynamicAttributesDao dynamicdao = new DynamicAttributesDao();

        //        ArrayList entityids = new ArrayList();
        //        foreach (var obj in attributeData)
        //        {
        //            entityids.Add(obj.ID);
        //        }
        //        var result = from item in tx.PersistenceManager.PlanningRepository.Query<AttributeDao>() where entityids.Contains(item.Id) select item;
        //        //var removingOwner = result.Where(a => a.Id != Convert.ToInt32(SystemDefinedAttributes.Owner));
        //        var entityTypeCategory = tx.PersistenceManager.PlanningRepository.Query<EntityTypeDao>().Where(a => a.Id == typeId).Select(a => a.Category).FirstOrDefault();
        //        var dynamicAttResult = result.Where(a => ((a.Id != 69) && (a.AttributeTypeID == 1 || a.AttributeTypeID == 2 || a.AttributeTypeID == 3 || a.AttributeTypeID == 5 || a.AttributeTypeID == 8 || a.AttributeTypeID == 9 || a.AttributeTypeID == 11)));
        //        var treenodeResult = result.Where(a => a.AttributeTypeID == (Convert.ToInt32(AttributesList.Tree)));
        //        var treevalResult = result.Where(a => a.AttributeTypeID == (Convert.ToInt32(AttributesList.DropDownTree)));
        //        var multiAttrResult = result.Where(a => a.AttributeTypeID == (Convert.ToInt32(AttributesList.ListMultiSelection)));
        //        var systemDefinedResults = result.Where(a => a.IsSpecial == true);

        //        if (systemDefinedResults.Count() > 0)
        //        {

        //            foreach (var val in systemDefinedResults)
        //            {
        //                SystemDefinedAttributes systemType = (SystemDefinedAttributes)val.Id;
        //                var dataResult = attributeData.Join(systemDefinedResults,
        //                        post => post.ID,
        //                        meta => meta.Id,
        //                        (post, meta) => new { databaseval = post }).Where(a => a.databaseval.ID == Convert.ToInt32(SystemDefinedAttributes.Owner));
        //                switch (systemType)
        //                {
        //                    case SystemDefinedAttributes.Owner:
        //                        //IList<EntityRoleUserDao> Ientitroledao = new List<EntityRoleUserDao>();
        //                        //if (dataResult.Count() > 0)
        //                        //{
        //                        //    foreach (var ownerObj in dataResult)
        //                        //    {
        //                        //        EntityRoleUserDao entityroledao = new EntityRoleUserDao();
        //                        //        entityroledao.Entityid = entityId;
        //                        //        entityroledao.Roleid = 1;
        //                        //        entityroledao.Userid = ownerObj.databaseval.Value;
        //                        //        entityroledao.IsInherited = false;
        //                        //        entityroledao.InheritedFromEntityid = 0;
        //                        //        Ientitroledao.Add(entityroledao);

        //                        //    }
        //                        //    tx.PersistenceManager.PlanningRepository.Save<EntityRoleUserDao>(Ientitroledao);
        //                        //}
        //                        break;
        //                }
        //            }
        //        }

        //        if (treevalResult.Count() > 0)
        //        {
        //            var treeValQuery = attributeData.Join(treevalResult,
        //                         post => post.ID,
        //                         meta => meta.Id,
        //                         (post, meta) => new { databaseval = post });
        //            if (treeValQuery.Count() > 0)
        //            {
        //                foreach (var treeattr in treeValQuery)
        //                {
        //                    foreach (var treevalobj in treeattr.databaseval.Value)
        //                    {
        //                        TreeValueDao tre = new TreeValueDao();
        //                        tre.Attributeid = treeattr.databaseval.ID;
        //                        tre.Entityid = entityId;
        //                        tre.Nodeid = treevalobj;
        //                        tre.Level = treeattr.databaseval.Level;
        //                        listreeval.Add(tre);
        //                    }
        //                }
        //                tx.PersistenceManager.PlanningRepository.Save<Marcom.Dal.Metadata.Model.TreeValueDao>(listreeval);
        //            }
        //        }
        //        if (multiAttrResult.Count() > 0)
        //        {
        //            tx.PersistenceManager.PlanningRepository.DeleteByID<Marcom.Dal.Metadata.Model.MultiSelectDao>(entityId);
        //            var query = attributeData.Join(multiAttrResult,
        //                     post => post.ID,
        //                     meta => meta.Id,
        //                     (post, meta) => new { databaseval = post, attrappval = meta });
        //            foreach (var at in query)
        //            {
        //                Marcom.Dal.Metadata.Model.MultiSelectDao mt = new Marcom.Dal.Metadata.Model.MultiSelectDao();
        //                mt.Attributeid = at.databaseval.ID;
        //                mt.Entityid = entityId;
        //                mt.Optionid = Convert.ToInt32(at.databaseval.Value);
        //                listMultiselect.Add(mt);
        //            }
        //            tx.PersistenceManager.PlanningRepository.Save<Marcom.Dal.Metadata.Model.MultiSelectDao>(listMultiselect);
        //        }
        //        if (treenodeResult.Count() > 0)
        //        {
        //            var treenodequery = attributeData.Join(treenodeResult,
        //                         post => post.ID,
        //                         meta => meta.Id,
        //                         (post, meta) => new { databaseval = post });
        //            foreach (var et in treenodequery)
        //            {
        //                foreach (var treenodeobj in et.databaseval.Value)
        //                {
        //                    Marcom.Dal.Metadata.Model.TreeValueDao tre = new Marcom.Dal.Metadata.Model.TreeValueDao();
        //                    tre.Attributeid = et.databaseval.ID;
        //                    tre.Entityid = entityId;
        //                    tre.Nodeid = treenodeobj;
        //                    tre.Level = et.databaseval.Level;
        //                    listreeval.Add(tre);
        //                }
        //            }
        //            tx.PersistenceManager.PlanningRepository.Save<Marcom.Dal.Metadata.Model.TreeValueDao>(listreeval);
        //        }
        //        if (dynamicAttResult.Count() > 0 || entityTypeCategory != 1)
        //        {
        //            Dictionary<string, dynamic> attr = new Dictionary<string, dynamic>();


        //            var dynamicAttrQuery = attributeData.Join(dynamicAttResult,
        //                        post => post.ID,
        //                        meta => meta.Id,
        //                        (post, meta) => new { databaseval = post });
        //            foreach (var ab in dynamicAttrQuery)
        //            {

        //                string key = Convert.ToString((int)ab.databaseval.ID);
        //                int attributedataType = ab.databaseval.TypeID;
        //                // dynamic value = ab.databaseval.Value;
        //                dynamic value = null;
        //                switch (attributedataType)
        //                {
        //                    case 1:
        //                    case 2:
        //                    case 11:
        //                        {
        //                            value = Convert.ToString(ab.databaseval.Value == null ? "" : (string)ab.databaseval.Value);
        //                            break;
        //                        }
        //                    case 3:
        //                        {
        //                            value = Convert.ToString(ab.databaseval.Value == null ? 0 : (int)ab.databaseval.Value);
        //                            break;
        //                        }
        //                    case 5:
        //                        {
        //                            value = DateTime.Parse(ab.databaseval.Value == null ? "" : (string)ab.databaseval.Value, CultureInfo.InvariantCulture);
        //                            break;
        //                        }
        //                    case 8:
        //                        {

        //                            value = Convert.ToInt32(((ab.databaseval.Value == null) ? 0 : (int)ab.databaseval.Value));
        //                            break;
        //                        }
        //                    case 9:
        //                        {
        //                            value = value = Convert.ToBoolean(ab.databaseval.Value != null ? 0 : (int)ab.databaseval.Value);
        //                            break;
        //                        }
        //                }
        //                attr.Add(key, value);
        //            }
        //            dictAttr = attr != null ? attr : null;
        //            dynamicdao.Id = entityId;
        //            dynamicdao.Attributes = dictAttr;
        //            tx.PersistenceManager.PlanningRepository.SaveByentity<DynamicAttributesDao>(entityName, dynamicdao);
        //            BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("Saved Succesfully into Dynamic tables", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);

        //        }
        //    }
        //    return true;
        //}

        public bool InsertEntityAttributes(ITransaction tx, IList<IAttributeData> attributeData, int entityId, int typeId)
        {
            if (attributeData != null)
            {
                BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("Started inseting values into Dynamic tables", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);

                string entityName = "AttributeRecord" + typeId + "_V" + MarcomManagerFactory.ActiveMetadataVersionNumber;
                IList<IDynamicAttributes> listdynamicattributes = new List<IDynamicAttributes>();
                Dictionary<string, object> dictAttr = new Dictionary<string, object>();
                IList<MultiSelectDao> listMultiselect = new List<MultiSelectDao>();
                IList<TreeValueDao> listreeval = new List<TreeValueDao>();
                listreeval.Clear();
                DynamicAttributesDao dynamicdao = new DynamicAttributesDao();

                ArrayList entityids = new ArrayList();
                foreach (var obj in attributeData)
                {
                    entityids.Add(obj.ID);
                }
                var result = from item in tx.PersistenceManager.PlanningRepository.Query<AttributeDao>() where entityids.Contains(item.Id) select item;
                //var removingOwner = result.Where(a => a.Id != Convert.ToInt32(SystemDefinedAttributes.Owner));
                var entityTypeCategory = tx.PersistenceManager.PlanningRepository.Query<EntityTypeDao>().Where(a => a.Id == typeId).Select(a => a.Category).FirstOrDefault();
                var dynamicAttResult = result.Where(a => ((a.Id != 69) && (a.AttributeTypeID == 1 || a.AttributeTypeID == 2 || a.AttributeTypeID == 3 || a.AttributeTypeID == 5 || a.AttributeTypeID == 8 || a.AttributeTypeID == 9 || a.AttributeTypeID == 11)));
                var treenodeResult = result.Where(a => a.AttributeTypeID == (Convert.ToInt32(AttributesList.Tree)));
                var treevalResult = result.Where(a => a.AttributeTypeID == (Convert.ToInt32(AttributesList.DropDownTree)));
                var multiAttrResult = result.Where(a => a.AttributeTypeID == (Convert.ToInt32(AttributesList.ListMultiSelection)));
                var systemDefinedResults = result.Where(a => a.IsSpecial == true);
                var multiselecttreevalResult = result.Where(a => a.AttributeTypeID == (Convert.ToInt32(AttributesList.TreeMultiSelection)));
                var multiselectPricingtreevalResult = result.Where(a => a.AttributeTypeID == (Convert.ToInt32(AttributesList.DropDownTreePricing)));

                if (systemDefinedResults.Count() > 0)
                {

                    foreach (var val in systemDefinedResults)
                    {
                        SystemDefinedAttributes systemType = (SystemDefinedAttributes)val.Id;
                        var dataResult = attributeData.Join(systemDefinedResults,
                                post => post.ID,
                                meta => meta.Id,
                                (post, meta) => new { databaseval = post }).Where(a => a.databaseval.ID == Convert.ToInt32(SystemDefinedAttributes.Owner));
                        switch (systemType)
                        {
                            case SystemDefinedAttributes.Owner:
                                IList<EntityRoleUserDao> Ientitroledao = new List<EntityRoleUserDao>();
                                if (dataResult.Count() > 0)
                                {
                                    foreach (var ownerObj in dataResult)
                                    {
                                        EntityRoleUserDao entityroledao = new EntityRoleUserDao();
                                        entityroledao.Entityid = entityId;
                                        var NewObj = tx.PersistenceManager.PlanningRepository.Query<EntityTypeRoleAclDao>().Where(t => t.EntityTypeID == typeId && (EntityRoles)t.EntityRoleID == EntityRoles.Owner).SingleOrDefault();
                                        int RoleID = NewObj.ID;
                                        entityroledao.Roleid = RoleID;
                                        entityroledao.Userid = ownerObj.databaseval.Value;
                                        entityroledao.IsInherited = false;
                                        entityroledao.InheritedFromEntityid = 0;
                                        Ientitroledao.Add(entityroledao);

                                    }
                                    tx.PersistenceManager.PlanningRepository.Save<EntityRoleUserDao>(Ientitroledao);
                                }
                                break;
                        }
                    }
                }

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
                                TreeValueDao tre = new TreeValueDao();
                                tre.Attributeid = treeattr.databaseval.ID;
                                tre.Entityid = entityId;
                                tre.Nodeid = treevalobj;
                                tre.Level = treeattr.databaseval.Level;
                                tre.Value = "";
                                listreeval.Add(tre);
                            }
                        }
                        tx.PersistenceManager.PlanningRepository.Save<Marcom.Dal.Metadata.Model.TreeValueDao>(listreeval);
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
                                TreeValueDao tre = new TreeValueDao();
                                tre.Attributeid = treeattr.databaseval.ID;
                                tre.Entityid = entityId;
                                tre.Nodeid = treevalobj;
                                tre.Level = treeattr.databaseval.Level;
                                tre.Value = "";
                                listreeval.Add(tre);
                            }
                        }
                        tx.PersistenceManager.PlanningRepository.Save<Marcom.Dal.Metadata.Model.TreeValueDao>(listreeval);
                    }
                }

                if (multiselectPricingtreevalResult.Count() > 0)
                {
                    var multiselecttreeValQuery = attributeData.Join(multiselectPricingtreevalResult,
                                 post => post.ID,
                                 meta => meta.Id,
                                 (post, meta) => new { databaseval = post });
                    if (multiselecttreeValQuery.Count() > 0)
                    {
                        foreach (var treeattr in multiselecttreeValQuery)
                        {
                            foreach (var treevalobj in treeattr.databaseval.Value)
                            {
                                TreeValueDao tre = new TreeValueDao();
                                tre.Attributeid = treeattr.databaseval.ID;
                                tre.Entityid = entityId;
                                tre.Nodeid = treevalobj;
                                tre.Level = treeattr.databaseval.Level;
                                tre.Value = treeattr.databaseval.SpecialValue;
                                listreeval.Add(tre);
                            }
                        }
                        tx.PersistenceManager.PlanningRepository.Save<Marcom.Dal.Metadata.Model.TreeValueDao>(listreeval);
                    }
                }
                if (multiAttrResult.Count() > 0)
                {

                    tx.PersistenceManager.PlanningRepository.DeleteByID<Marcom.Dal.Metadata.Model.MultiSelectDao>(entityId);

                    string deletequery = "DELETE FROM MM_MultiSelect WHERE EntityID = ? ";
                    tx.PersistenceManager.PlanningRepository.ExecuteQuerywithMinParam(deletequery.ToString(), Convert.ToInt32(entityId));

                    var query = attributeData.Join(multiAttrResult,
                             post => post.ID,
                             meta => meta.Id,
                             (post, meta) => new { databaseval = post, attrappval = meta });
                    foreach (var at in query)
                    {
                        Marcom.Dal.Metadata.Model.MultiSelectDao mt = new Marcom.Dal.Metadata.Model.MultiSelectDao();
                        mt.Attributeid = at.databaseval.ID;
                        mt.Entityid = entityId;
                        mt.Optionid = Convert.ToInt32(at.databaseval.Value);
                        listMultiselect.Add(mt);
                    }
                    tx.PersistenceManager.PlanningRepository.Save<Marcom.Dal.Metadata.Model.MultiSelectDao>(listMultiselect);
                }
                if (treenodeResult.Count() > 0)
                {
                    var treenodequery = attributeData.Join(treenodeResult,
                                 post => post.ID,
                                 meta => meta.Id,
                                 (post, meta) => new { databaseval = post });
                    foreach (var et in treenodequery)
                    {
                        foreach (var treenodeobj in et.databaseval.Value)
                        {
                            Marcom.Dal.Metadata.Model.TreeValueDao tre = new Marcom.Dal.Metadata.Model.TreeValueDao();
                            tre.Attributeid = et.databaseval.ID;
                            tre.Entityid = entityId;
                            tre.Nodeid = treenodeobj;
                            tre.Level = et.databaseval.Level;
                            listreeval.Add(tre);
                        }
                    }
                    tx.PersistenceManager.PlanningRepository.Save<Marcom.Dal.Metadata.Model.TreeValueDao>(listreeval);
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
                                    if (ab.databaseval.Value != null)
                                        value = DateTime.Parse(ab.databaseval.Value == null ? "" : (string)ab.databaseval.Value.ToString());
                                    else
                                        value = null;
                                    break;
                                }
                            case 8:
                                {
                                    value = 0;
                                    int n;
                                    if (ab.databaseval.Value != null || ab.databaseval.Value != "")
                                    {
                                        if (int.TryParse(Convert.ToString(ab.databaseval.Value), out n))
                                        {
                                            value = Convert.ToInt32(ab.databaseval.Value);
                                        }
                                    }
                                    break;
                                }
                            case 9:
                                {
                                    value = value = Convert.ToBoolean(ab.databaseval.Value == null ? false : ab.databaseval.Value);
                                    break;
                                }
                        }
                        attr.Add(key, value);
                    }
                    dictAttr = attr != null ? attr : null;
                    dynamicdao.Id = entityId;
                    dynamicdao.Attributes = dictAttr;

                    tx.PersistenceManager.PlanningRepository.SaveByentity<DynamicAttributesDao>(entityName, dynamicdao);
                    BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("Saved Succesfully into Dynamic tables", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);

                }
            }
            return true;
        }
        /// <summary>
        /// Getting All Milestones based on EntityId.
        /// </summary>
        /// <param name="entityId">The EntityId</param>
        /// <param name="entitytypeId">The MileStoneTypeId</param>
        /// <returns>IList of IMilestoneMetadata</returns>
        public IList<IMilestoneMetadata> GetAdminTaskMetadata(TaskManagerProxy proxy, int entityId, int entitytypeId)
        {
            IList<IMilestoneMetadata> listMilestone = new List<IMilestoneMetadata>();
            string entityName = "AttributeRecord" + entitytypeId + "_V" + MarcomManagerFactory.ActiveMetadataVersionNumber;
            List<int> attrIds = new List<int>();
            using (ITransaction tx = proxy.MarcomManager.GetTransaction())
            {
                //var attrResult = tx.PersistenceManager.PlanningRepository.GetAll<DynamicAttributesDao>(entityName).Where(a => Convert.ToInt32(a.Attributes["" + Convert.ToInt32(SystemDefinedAttributes.MilestoneEntityID) + ""]) == entityId);
                var attrResult = tx.PersistenceManager.PlanningRepository.GetAll<DynamicAttributesDao>(entityName).Where(a => a.Id == entityId);
                var entityTypeResult = tx.PersistenceManager.PlanningRepository.Query<EntityTypeAttributeRelationDao>().Where(a => a.EntityTypeID == entitytypeId).ToList();
                var attributeResult = tx.PersistenceManager.PlanningRepository.Query<AttributeDao>();
                foreach (var obj in attrResult)
                {
                    MilestoneMetadata milestonedata = new MilestoneMetadata();
                    IList<IAttributeData> listAttr = new List<IAttributeData>();
                    milestonedata.EntityId = obj.Id;
                    milestonedata.AttributeData = null;
                    var getMilestoneName = tx.PersistenceManager.PlanningRepository.Get<AdminTaskDao>(milestonedata.EntityId);
                    AttributeData attrformilestone = new AttributeData();
                    attrformilestone.ID = Convert.ToInt32(SystemDefinedAttributes.Name);
                    attrformilestone.Caption = entityTypeResult.Where(a => a.AttributeID == (Convert.ToInt32(SystemDefinedAttributes.Name)) && a.EntityTypeID == entitytypeId).Select(a => a.Caption).First();
                    attrformilestone.Value = getMilestoneName.Caption;
                    attrformilestone.TypeID = attributeResult.Where(a => a.Id == Convert.ToInt32(SystemDefinedAttributes.Name)).Select(a => a.AttributeTypeID).First();
                    attrformilestone.IsSpecial = true;
                    attrformilestone.Lable = entityTypeResult.Where(a => a.AttributeID == (Convert.ToInt32(SystemDefinedAttributes.Name)) && a.EntityTypeID == entitytypeId).Select(a => a.Caption).First();
                    attrformilestone.Level = 0;
                    listAttr.Add(attrformilestone);
                    if (obj.Attributes != null)
                    {

                        foreach (DictionaryEntry ob in obj.Attributes)
                        {
                            AttributeData attr = new AttributeData();
                            int attributeid = Convert.ToInt32((object)ob.Key);
                            attr.ID = attributeResult.Where(a => a.Id == attributeid).Select(a => a.Id).First();
                            attr.TypeID = attributeResult.Where(a => a.Id == attributeid).Select(a => a.AttributeTypeID).First();
                            attr.Caption = entityTypeResult.Where(a => a.AttributeID == attributeid && a.EntityTypeID == entitytypeId).Select(a => a.Caption).First();
                            if (Convert.ToInt32(ob.Key) == Convert.ToInt32(SystemDefinedAttributes.DueDate))
                                attr.Value = ((System.DateTime)(ob.Value)).Date.ToString();
                            else
                                attr.Value = ob.Value;
                            listAttr.Add(attr);

                        }
                    }
                    milestonedata.AttributeData = listAttr;
                    listMilestone.Add(milestonedata);
                }
            }
            return listMilestone;
        }

        public Tuple<bool, IAdminTask> UpdateAdminTask(TaskManagerProxy proxy, int milestoneTypeID, string Name, string description, int tasktype, IList<IAttributeData> milestoneObj, int entityId, IList<IAdminTaskCheckList> adminChkLst)
        {
            try
            {

                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    AdminTaskDao dao = new AdminTaskDao();
                    dao = (from item in tx.PersistenceManager.TaskRepository.Query<AdminTaskDao>() where item.ID == entityId select item).FirstOrDefault(); ;
                    dao.Caption = Name;
                    dao.Description = description;
                    dao.TaskType = tasktype;
                    tx.PersistenceManager.PlanningRepository.Save<AdminTaskDao>(dao);
                    IAdminTask iadmtsk = new AdminTask();

                    iadmtsk.Id = dao.ID;
                    iadmtsk.Caption = dao.Caption;
                    iadmtsk.TaskListID = dao.TaskListID;
                    iadmtsk.Description = dao.Description;
                    iadmtsk.TaskType = dao.TaskType;
                    iadmtsk.TaskTypeName = Enum.GetName(typeof(TaskTypes), dao.TaskType).Replace("_", " ");
                    iadmtsk.SortOder = 1;
                    var taskattachment = (from task in tx.PersistenceManager.TaskRepository.Query<NewTaskAttachmentsDao>() where task.Entityid == entityId select task).ToList().Count();
                    var taskLinks = (from task in tx.PersistenceManager.TaskRepository.Query<TaskLinksDao>() where task.EntityID == entityId select task).ToList().Count();
                    iadmtsk.TotalTaskAttachment = taskattachment + taskLinks;
                    IList<AdminTaskCheckListDao> chkLstdao = new List<AdminTaskCheckListDao>();
                    foreach (var chklstitem in adminChkLst)
                    {
                        chkLstdao.Add(new AdminTaskCheckListDao { ID = chklstitem.ID, NAME = chklstitem.NAME, SortOrder = chklstitem.SortOrder, TaskId = dao.ID });
                    }
                    tx.PersistenceManager.TaskRepository.Save<AdminTaskCheckListDao>(chkLstdao);
                    iadmtsk.AdminTaskCheckList = chkLstdao;
                    tx.Commit();
                    iadmtsk.AttributeData = GetEntityAttributesDetails(proxy, entityId);
                    Tuple<bool, IAdminTask> taskObj = Tuple.Create(true, iadmtsk);
                    return taskObj;
                }

            }
            catch (Exception ex)
            {
                return null;
            }



        }


        public IList<IFile> GetEntityTaskAttachmentFile(TaskManagerProxy proxy, int taskID)
        {
            try
            {
                IList<IFile> iifilelist = new List<IFile>();

                IList<FileDao> entityroleuserdao = new List<FileDao>();
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    var AttachmentsAndLinksSelectQuery = new StringBuilder();
                    if (taskID != 0)
                    {
                        AttachmentsAndLinksSelectQuery.Append("select TMF.ID,TMTA.EntityID,TMF.Checksum,Tmf.CreatedOn,tmf.Extension,tmf.FileGuid,TMF.MimeType,tmf.ModuleID,TMF.Name,tmf.OwnerID,tmf.Size,tmf.VersionNo,'' as 'URL' , '' AS 'LinkType', TMF.Description,TMTA.VersioningFileId,TMTA.ActiveFileVersionID  from CM_File TMF inner join PM_Attachments TMTA on TMTA.ActiveFileVersionID = tmf.ID and TMTA.ActiveVersionNo = TMF.VersionNo where TMTA.EntityID = ? union select TML.ID, TML.EntityID, '' as 'Checksum' , tml.CreatedOn, 'Link' as 'Extension' , TML.LinkGuid as 'FileGuid', '-' as 'MimeType',tml.ModuleID, tml.Name, tml.OwnerID,'0' as 'Size' , tml.ActiveVersionNo as 'VersionNo' , tml.URL , tml.LinkType,  TML.DESCRIPTION as 'Description', 0 AS VersioningFileId,0 AS ActiveFileVersionID from CM_Links TML where tml.EntityID = ?  order by CreatedOn DESC");

                    }
                    var Result = ((tx.PersistenceManager.CommonRepository.ExecuteQuerywithMinParam(AttachmentsAndLinksSelectQuery.ToString(), taskID, taskID)).Cast<Hashtable>().ToList());

                    IFile taskFile = new BrandSystems.Marcom.Core.Common.File();
                    foreach (var obj in Result)
                    {

                        taskFile = new BrandSystems.Marcom.Core.Common.File();
                        taskFile.Id = Convert.ToInt32(obj["ID"]);
                        taskFile.Entityid = Convert.ToInt32(obj["EntityID"]);
                        taskFile.Checksum = Convert.ToString(obj["Checksum"]);
                        taskFile.CreatedOn = DateTimeOffset.Parse(obj["CreatedOn"].ToString());
                        taskFile.StrCreatedDate = taskFile.CreatedOn.ToString("dd-MM-yyyy");
                        taskFile.Extension = Convert.ToString(obj["Extension"]);
                        taskFile.Fileguid = (Guid)(obj["FileGuid"]);
                        taskFile.MimeType = Convert.ToString(obj["MimeType"]);
                        taskFile.Moduleid = Convert.ToInt32(obj["ModuleID"]);
                        taskFile.Name = Convert.ToString(obj["Name"]);
                        taskFile.Ownerid = Convert.ToInt32(obj["OwnerID"]);
                        taskFile.Size = Convert.ToInt32(obj["Size"]);
                        taskFile.VersionNo = Convert.ToInt32(obj["VersionNo"]);
                        taskFile.LinkURL = Convert.ToString(obj["URL"]);
                        taskFile.LinkType = Convert.ToInt32(obj["LinkType"]);
                        taskFile.Description = Convert.ToString(obj["Description"]);
                        taskFile.VersioningFileId = Convert.ToInt32(obj["VersioningFileId"]);
                        taskFile.ActiveFileVersionID = Convert.ToInt32(obj["ActiveFileVersionID"]);
                        //var fileDescriptionobj = (from attach in tx.PersistenceManager.PlanningRepository.Query<NewTaskAttachmentsDao>() where attach.Entityid == taskID && attach.ActiveFileid == Convert.ToInt32(obj["ID"]) select attach.Description).FirstOrDefault();
                        taskFile.Description = Convert.ToString(obj["Description"]);
                        var OwnerName = tx.PersistenceManager.TaskRepository.Query<UserDao>().Where(a => a.Id == taskFile.Ownerid).Select(a => a.FirstName + " " + a.LastName).FirstOrDefault();
                        taskFile.OwnerName = OwnerName;
                        iifilelist.Add(taskFile);
                    }
                    tx.Commit();
                }

                return iifilelist;
            }
            catch (Exception ex)
            {
                return null;
            }

        }



        public IList<IFile> ViewAllFilesByEntityID(TaskManagerProxy proxy, int taskID, int VersionFileID)
        {
            try
            {
                IList<IFile> iifilelist = new List<IFile>();

                IList<FileDao> entityroleuserdao = new List<FileDao>();
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    var AttachmentsSelectQuery = new StringBuilder();
                    if (taskID != 0)
                    {
                        AttachmentsSelectQuery.Append("select TMF.ID,TMTA.EntityID,TMF.Checksum,Tmf.CreatedOn,tmf.Extension,tmf.FileGuid,TMF.MimeType,tmf.ModuleID,TMF.Name,tmf.OwnerID,tmf.Size,tmf.VersionNo , TMF.Description,  TMTA.ActiveFileVersionID from CM_File TMF inner join PM_Attachments TMTA  on TMTA.ActiveFileID = TMF.ID and TMTA.EntityID = TMF.EntityID and TMTA.VersioningFileId= ?  where TMTA.EntityID = ? order by ActiveFileVersionID DESC");
                    }
                    var Result = ((tx.PersistenceManager.CommonRepository.ExecuteQuerywithMinParam(AttachmentsSelectQuery.ToString(), VersionFileID, taskID)).Cast<Hashtable>().ToList());


                    //entityroleuserdao = (from attach in tx.PersistenceManager.PlanningRepository.QEntityIDuery<FileDao>() where attach.Entityid == taskID select attach).ToList<FileDao>();

                    foreach (var obj in Result)
                    {
                        IFile taskFile = new BrandSystems.Marcom.Core.Common.File();
                        taskFile = new BrandSystems.Marcom.Core.Common.File();
                        taskFile.Id = Convert.ToInt32(obj["ID"]);
                        taskFile.Entityid = Convert.ToInt32(obj["EntityID"]);
                        taskFile.Checksum = Convert.ToString(obj["Checksum"]);
                        taskFile.CreatedOn = DateTimeOffset.Parse(obj["CreatedOn"].ToString());
                        taskFile.StrCreatedDate = taskFile.CreatedOn.ToString("dd-MM-yyyy");
                        taskFile.Extension = Convert.ToString(obj["Extension"]);
                        taskFile.Fileguid = (Guid)(obj["FileGuid"]);
                        taskFile.MimeType = Convert.ToString(obj["MimeType"]);
                        taskFile.Moduleid = Convert.ToInt32(obj["ModuleID"]);
                        taskFile.Name = Convert.ToString(obj["Name"]);
                        taskFile.Ownerid = Convert.ToInt32(obj["OwnerID"]);
                        taskFile.Size = Convert.ToInt32(obj["Size"]);
                        taskFile.VersionNo = Convert.ToInt32(obj["VersionNo"]);
                        taskFile.Description = Convert.ToString(obj["Description"]);
                        taskFile.ActiveFileVersionID = Convert.ToInt32(obj["ActiveFileVersionID"]);
                        var OwnerName = tx.PersistenceManager.TaskRepository.Query<UserDao>().Where(a => a.Id == taskFile.Ownerid).Select(a => a.FirstName + " " + a.LastName).FirstOrDefault();
                        taskFile.OwnerName = OwnerName;

                        var fileDescriptionobj = (from attach in tx.PersistenceManager.PlanningRepository.Query<NewTaskAttachmentsDao>() where attach.Entityid == taskID && attach.ActiveFileid == Convert.ToInt32(obj["ID"]) select attach.Description).FirstOrDefault();
                        iifilelist.Add(taskFile);
                    }
                    //IList<TaskLinksDao> linksdao = new List<TaskLinksDao>();
                    //ITasklinks taskLink = new BrandSystems.Marcom.Core.Task.TaskLinks();
                    //linksdao = (from attach in tx.PersistenceManager.PlanningRepository.Query<TaskLinksDao>() where attach.EntityID == taskID select attach).ToList<TaskLinksDao>();
                    //foreach (var entmem in linksdao)
                    //{
                    //    taskLink = new BrandSystems.Marcom.Core.Task.TaskLinks();
                    //    taskLink.ID = entmem.ID;
                    //    taskLink.EntityID = entmem.EntityID;
                    //    taskLink.CreatedOn = Convert.ToDateTime(entmem.CreatedOn).ToString();
                    //    //taskLink.StrCreatedDate = entmem.CreatedOn;
                    //    taskLink.Extension = "link";
                    //    taskLink.Description = entmem.Description;
                    //    taskLink.LinkGuid = entmem.LinkGuid;
                    //    taskLink.ModuleID = entmem.ModuleID;
                    //    taskLink.URL = entmem.URL;
                    //    taskLink.Name = entmem.Name;
                    //    taskLink.OwnerID = entmem.OwnerID;
                    //    taskLink.ActiveVersionNo = entmem.ActiveVersionNo;
                    //    iifilelist.Add(taskLink);
                    //}


                }

                return iifilelist;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public bool UpdateAttachmentVersionNo(TaskManagerProxy proxy, int taskID, int SelectedVersion, int VersioningFileId)
        {
            try
            {
                var assetId = 0;
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    tx.PersistenceManager.TaskRepository.ExecuteQuerywithMinParam(" update PM_Attachments set activefileversionID = 0 where EntityID = ? and VersioningFileId = ?", taskID, VersioningFileId);
                    tx.PersistenceManager.TaskRepository.ExecuteQuerywithMinParam(" update PM_Attachments set activeFileVersionID = ActiveFileID where EntityID = ? and VersioningFileId= ? and ActiveVersionNo= ? ", taskID, VersioningFileId, SelectedVersion);
                    assetId = tx.PersistenceManager.TaskRepository.Query<EntityTaskDao>().Where(item => item.ID == taskID).Select(item => item.AssetId).FirstOrDefault();
                    if ((int)assetId != 0)
                    {
                        var activefile = tx.PersistenceManager.TaskRepository.Query<DAMFileDao>().Where(item => item.AssetID == (int)assetId && item.VersionNo == SelectedVersion).Select(item => item.ID).FirstOrDefault();
                        AssetsDao objassetDao = new AssetsDao();
                        objassetDao = tx.PersistenceManager.TaskRepository.Get<AssetsDao>((int)assetId);
                        objassetDao.ActiveFileID = (int)activefile;
                        tx.PersistenceManager.TaskRepository.Save<AssetsDao>(objassetDao);
                    }
                    tx.Commit();
                    return true;
                }
            }
            catch
            {
            }
            return false;

        }

        public IList<ITaskFile> GetTaskAttachmentFile(TaskManagerProxy proxy, int taskID)
        {
            try
            {
                IList<ITaskFile> iifilelist = new List<ITaskFile>();

                IList<TaskFileDao> entityroleuserdao = new List<TaskFileDao>();
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    var AttachmentsAndLinksSelectQuery = new StringBuilder();
                    if (taskID != 0)
                    {
                        AttachmentsAndLinksSelectQuery.Append("select TMF.ID,TMTA.EntityID,TMF.Checksum,Tmf.CreatedOn,tmf.Extension,tmf.FileGuid,TMF.MimeType,tmf.ModuleID,TMF.Name,tmf.OwnerID,tmf.Size,tmf.VersionNo,'' as 'URL' ,'' AS 'LinkType', TMF.Description from TM_File TMF inner join TM_Task_Attachments TMTA on TMTA.ActiveFileID = tmf.ID and TMTA.ActiveVersionNo = TMF.VersionNo where TMTA.EntityID = ? union select TML.ID, TML.EntityID, '' as 'Checksum' , tml.CreatedOn, 'Link' as 'Extension' , TML.LinkGuid as 'FileGuid', '-' as 'MimeType',tml.ModuleID, tml.Name, tml.OwnerID,'0' as 'Size' , tml.ActiveVersionNo as 'VersionNo' , tml.URL , tml.LinkType,  TML.DESCRIPTION as 'Description' from TM_Links TML where tml.EntityID = ? order by CreatedOn DESC ");

                    }
                    var Result = ((tx.PersistenceManager.CommonRepository.ExecuteQuerywithMinParam(AttachmentsAndLinksSelectQuery.ToString(), taskID, taskID)).Cast<Hashtable>().ToList());
                    //entityroleuserdao = (from attach in tx.PersistenceManager.PlanningRepository.QEntityIDuery<FileDao>() where attach.Entityid == taskID select attach).ToList<FileDao>();
                    ITaskFile taskFile = new BrandSystems.Marcom.Core.Task.TaskFile();
                    foreach (var obj in Result)
                    {
                        taskFile = new BrandSystems.Marcom.Core.Task.TaskFile();
                        taskFile.Id = Convert.ToInt32(obj["ID"]);
                        taskFile.Entityid = Convert.ToInt32(obj["EntityID"]);
                        taskFile.Checksum = Convert.ToString(obj["Checksum"]);
                        taskFile.CreatedOn = DateTimeOffset.Parse(obj["CreatedOn"].ToString());
                        taskFile.StrCreatedDate = taskFile.CreatedOn.ToString("dd-MM-yyyy");
                        taskFile.Extension = Convert.ToString(obj["Extension"]);
                        taskFile.Fileguid = (Guid)(obj["FileGuid"]);
                        taskFile.MimeType = Convert.ToString(obj["MimeType"]);
                        taskFile.Moduleid = Convert.ToInt32(obj["ModuleID"]);
                        taskFile.Name = Convert.ToString(obj["Name"]);
                        taskFile.Ownerid = Convert.ToInt32(obj["OwnerID"]);
                        taskFile.Size = Convert.ToInt32(obj["Size"]);
                        taskFile.VersionNo = Convert.ToInt32(obj["VersionNo"]);
                        taskFile.linkURL = Convert.ToString(obj["URL"]);
                        taskFile.LinkType = Convert.ToInt32(obj["LinkType"]);
                        taskFile.Description = Convert.ToString(obj["Description"]);
                        //if (taskFile.Extension != "Link")
                        //{
                        //    var fileDescriptionobj = (from attach in tx.PersistenceManager.TaskRepository.Query<AttachmentsDao>() where attach.Entityid == taskID && attach.ActiveFileid == Convert.ToInt32(obj["ID"]) select attach.Description).FirstOrDefault();
                        //    taskFile.Description = fileDescriptionobj;
                        //}
                        iifilelist.Add(taskFile);
                    }
                }

                return iifilelist;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public bool InsertEntityTaskAttachments(TaskManagerProxy proxy, int taskID, IList<IAttachments> TaskAttachments, IList<IFile> TaskFiles)
        {
            try
            {
                BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("-----------------------------------------------------------------------------------------------", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);
                BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("Started creating Task", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);

                FeedNotificationServer fs = new FeedNotificationServer();
                WorkFlowNotifyHolder obj = new WorkFlowNotifyHolder();

                obj.Actorid = proxy.MarcomManager.User.Id;
                obj.EntityId = taskID;

                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    var adminObj = tx.PersistenceManager.TaskRepository.Query<AdminTaskDao>().ToList().Where(item => item.ID == taskID).Select(item => item.TaskType).FirstOrDefault();
                    IList<FileDao> ifile = new List<FileDao>();
                    Dictionary<Guid, string> descriptionObj = new Dictionary<Guid, string>();
                    if (TaskFiles != null)
                    {

                        foreach (var a in TaskFiles)
                        {
                            Guid NewId = Guid.NewGuid();

                            string filePath = ReadAdminXML("FileManagment");
                            var DirInfo = System.IO.Directory.GetParent(filePath);
                            string newFilePath = DirInfo.FullName;
                            System.IO.File.Move(filePath + "\\" + a.strFileID.ToString() + a.Extension, newFilePath + "\\" + NewId + a.Extension);
                            FileDao fldao = new FileDao();
                            fldao.Checksum = a.Checksum;
                            fldao.CreatedOn = a.CreatedOn;
                            fldao.Entityid = taskID;
                            fldao.Extension = a.Extension;
                            fldao.MimeType = a.MimeType;
                            fldao.Moduleid = a.Moduleid;
                            fldao.Name = a.Name;
                            fldao.Ownerid = a.Ownerid;
                            fldao.Size = a.Size;
                            fldao.VersionNo = a.VersionNo;
                            fldao.Fileguid = NewId;
                            fldao.Description = a.Description;
                            ifile.Add(fldao);
                            //descriptionObj.Add(NewId, a.Description);

                        }
                        tx.PersistenceManager.PlanningRepository.Save<FileDao>(ifile);
                        //Adding to the Search Engine
                        BrandSystems.Marcom.Core.Interface.Managers.IEventManager _eventManager = null;
                        BrandSystems.Marcom.Core.Interface.Managers.IPluginManager _pluginManager = null;
                        MarcomManager marcommanager = new MarcomManager(_eventManager, _pluginManager);
                        BrandSystems.Marcom.Core.Managers.Proxy.PlanningManagerProxy pProxy = new BrandSystems.Marcom.Core.Managers.Proxy.PlanningManagerProxy(marcommanager);
                        System.Threading.Tasks.Task addtaskforsearch = new System.Threading.Tasks.Task(() => PlanningManager.Instance.AddEntityAsync(pProxy, ifile[0].Id, taskID, ifile[0].Name));
                        addtaskforsearch.Start();

                        BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("Saved in File", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);
                    }


                    if (ifile != null)
                    {

                        IList<AttachmentsDao> iattachment = new List<AttachmentsDao>();
                        foreach (var a in ifile)
                        {
                            // var result = descriptionObj.Where(r => r.Key == a.Fileguid).ToList();
                            AttachmentsDao attachedao = new AttachmentsDao();
                            attachedao.ActiveFileid = a.Id;
                            attachedao.ActiveVersionNo = 1;
                            attachedao.Createdon = DateTime.Now;
                            attachedao.Entityid = taskID;
                            attachedao.Name = a.Name;
                            attachedao.Typeid = adminObj;
                            attachedao.ActiveFileVersionID = a.Id;
                            attachedao.VersioningFileId = a.Id;
                            //attachedao.Description = result[0].Value;
                            iattachment.Add(attachedao);

                            obj.ToValue = Convert.ToString(a.Id);
                            obj.AttributeName = a.Name;
                            obj.attachmenttype = "file";

                        }
                        tx.PersistenceManager.PlanningRepository.Save<AttachmentsDao>(iattachment);
                        BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("Saved in Task Members", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);

                    }

                    //Task Reinitialize concept for Approval task
                    EntityTaskDao entityTask = new EntityTaskDao();
                    IList<TaskMembersDao> itaskMemDao = new List<TaskMembersDao>();
                    var taskDao = (from item in tx.PersistenceManager.TaskRepository.Query<EntityTaskDao>() where item.ID == taskID select item).ToList();
                    if (taskDao.Count() > 0)
                    {

                        entityTask = taskDao.FirstOrDefault();
                        TaskMembersDao memdao = new TaskMembersDao();
                        if (entityTask != null)
                        {
                            obj.action = "attachment added";
                            if (entityTask.TaskType != 2)
                            {

                                if (entityTask.TaskStatus == (int)TaskStatus.Rejected || entityTask.TaskStatus == (int)TaskStatus.Unable_to_complete || entityTask.TaskStatus == (int)TaskStatus.Approved || entityTask.TaskStatus == (int)TaskStatus.Completed)
                                {
                                    obj.action = "Task Reinitialized";
                                    entityTask.TaskStatus = (int)TaskStatus.In_progress;
                                    tx.PersistenceManager.TaskRepository.Save<EntityTaskDao>(entityTask);
                                }
                                var Allmembers = tx.PersistenceManager.PlanningRepository.Query<TaskMembersDao>().Where(a => a.TaskID == taskID).ToList();
                                //var totalTaskmembers = tx.PersistenceManager.PlanningRepository.Query<TaskMembersDao>().Where(a => a.TaskID == taskID && a.RoleID != 1).ToList();
                                var totalTaskmembers = (from members in Allmembers where members.RoleID != 1 select members).ToList<TaskMembersDao>();
                                if (totalTaskmembers != null)
                                {
                                    foreach (var mem in totalTaskmembers)
                                    {
                                        mem.ApprovalStatus = null;
                                        itaskMemDao.Add(mem);
                                    }
                                    tx.PersistenceManager.TaskRepository.Save<TaskMembersDao>(itaskMemDao);

                                    //obj.ientityRoles = itaskMemDao;
                                }
                                if (Allmembers != null)
                                {
                                    //IList<TaskMembersDao> itaskalllist=List<
                                    //    foreach (var mem in Allmembers)
                                    //    {

                                    //    }

                                    obj.ientityRoles = Allmembers;
                                }

                            }

                            tx.Commit();
                            fs.AsynchronousNotify(obj);
                            return true;
                        }

                    }
                    return false;
                }

            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public int InsertEntityTaskAttachmentsVersion(TaskManagerProxy proxy, int taskID, IList<IAttachments> TaskAttachments, IList<IFile> TaskFiles, int FileID, int VersioningFileId)
        {
            int fileid = 0;
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    var FileObj1 = tx.PersistenceManager.TaskRepository.Query<FileDao>().ToList().Where(item => item.Id == FileID && item.Entityid == taskID).Select(item => item.VersionNo).FirstOrDefault();
                    if (FileObj1 > 0)
                    {
                        tx.PersistenceManager.TaskRepository.ExecuteQuerywithMinParam(" update PM_Attachments set activefileversionID = 0 where EntityID = ?  and VersioningFileId = ? ", taskID, VersioningFileId);
                        tx.Commit();
                    }
                }

                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    var FileObj = tx.PersistenceManager.TaskRepository.Query<AttachmentsDao>().ToList().Where(item => item.VersioningFileId == VersioningFileId && item.Entityid == taskID).Select(item => item.ActiveVersionNo).Max();
                    FileObj += 1;
                    var adminObj = tx.PersistenceManager.TaskRepository.Query<AdminTaskDao>().ToList().Where(item => item.ID == taskID).Select(item => item.TaskType).FirstOrDefault();

                    IList<FileDao> ifile = new List<FileDao>();
                    FileDao fldao = new FileDao();
                    if (TaskFiles != null)
                    {

                        foreach (var a in TaskFiles)
                        {
                            Guid NewId = Guid.NewGuid();

                            string filePath = ReadAdminXML("FileManagment");
                            var DirInfo = System.IO.Directory.GetParent(filePath);
                            string newFilePath = DirInfo.FullName;
                            System.IO.File.Move(filePath + "\\" + a.strFileID.ToString() + a.Extension, newFilePath + "\\" + NewId + a.Extension);
                            fldao = new FileDao();
                            fldao.Checksum = a.Checksum;
                            fldao.CreatedOn = a.CreatedOn;
                            fldao.Entityid = taskID;
                            fldao.Extension = a.Extension;
                            fldao.MimeType = a.MimeType;
                            fldao.Moduleid = a.Moduleid;
                            fldao.Name = a.Name;
                            fldao.Ownerid = a.Ownerid;
                            fldao.Size = a.Size;
                            fldao.VersionNo = FileObj;
                            fldao.Fileguid = NewId;
                            fldao.Description = a.Description;
                            ifile.Add(fldao);
                            //descriptionObj.Add(NewId, a.Description);


                        }
                        tx.PersistenceManager.PlanningRepository.Save<FileDao>(ifile);
                        fileid = fldao.Id;
                        BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("Saved in File", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);
                    }


                    if (ifile != null)
                    {
                        IList<AttachmentsDao> iattachment = new List<AttachmentsDao>();
                        foreach (var a in ifile)
                        {
                            if (FileObj == 1)
                            {
                                // var result = descriptionObj.Where(r => r.Key == a.Fileguid).ToList();
                                AttachmentsDao attachedao = new AttachmentsDao();
                                attachedao.ActiveFileid = a.Id;
                                attachedao.ActiveVersionNo = FileObj;
                                attachedao.Createdon = DateTime.Now;
                                attachedao.Entityid = taskID;
                                attachedao.Name = a.Name;
                                attachedao.Typeid = adminObj;
                                attachedao.ActiveFileVersionID = a.Id;
                                attachedao.VersioningFileId = VersioningFileId;
                                //attachedao.Description = result[0].Value;
                                iattachment.Add(attachedao);
                            }
                            else
                            {
                                AttachmentsDao attachedao = new AttachmentsDao();
                                attachedao.ActiveFileid = a.Id;
                                attachedao.ActiveVersionNo = FileObj;
                                attachedao.Createdon = DateTime.Now;
                                attachedao.Entityid = taskID;
                                attachedao.Name = a.Name;
                                attachedao.Typeid = adminObj;
                                attachedao.ActiveFileVersionID = a.Id;
                                attachedao.VersioningFileId = VersioningFileId;
                                //attachedao.Description = result[0].Value;
                                iattachment.Add(attachedao);
                            }
                            //..for feed

                            FeedNotificationServer fs = new FeedNotificationServer();
                            NotificationFeedObjects obj = new NotificationFeedObjects();
                            obj.action = "attachment added";
                            obj.AttributeName = a.Name;
                            obj.attachmenttype = "file";
                            obj.Actorid = proxy.MarcomManager.User.Id;
                            obj.ToValue = Convert.ToString(a.Id);
                            obj.EntityId = taskID;
                            fs.AsynchronousNotify(obj);
                        }
                        tx.PersistenceManager.PlanningRepository.Save<AttachmentsDao>(iattachment);
                        BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("Saved in Task Members", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);

                        //Task Reinitialize concept for Approval task
                        EntityTaskDao entityTask = new EntityTaskDao();
                        IList<TaskMembersDao> itaskMemDao = new List<TaskMembersDao>();
                        var taskDao = (from item in tx.PersistenceManager.TaskRepository.Query<EntityTaskDao>() where item.ID == taskID select item).ToList();
                        if (taskDao.Count() > 0)
                        {
                            entityTask = taskDao.FirstOrDefault();
                            TaskMembersDao memdao = new TaskMembersDao();
                            if (entityTask != null)
                            {
                                if (entityTask.TaskType != 2)
                                {
                                    if (entityTask.TaskStatus == (int)TaskStatus.Rejected || entityTask.TaskStatus == (int)TaskStatus.Unable_to_complete || entityTask.TaskStatus == (int)TaskStatus.Approved)
                                    {

                                        entityTask.TaskStatus = (int)TaskStatus.In_progress;
                                        tx.PersistenceManager.TaskRepository.Save<EntityTaskDao>(entityTask);
                                    }

                                    var totalTaskmembers = tx.PersistenceManager.PlanningRepository.Query<TaskMembersDao>().Where(a => a.TaskID == taskID && a.RoleID != 1).ToList();
                                    if (totalTaskmembers != null)
                                    {
                                        foreach (var mem in totalTaskmembers)
                                        {
                                            mem.ApprovalStatus = null;
                                            itaskMemDao.Add(mem);
                                        }
                                        tx.PersistenceManager.TaskRepository.Save<TaskMembersDao>(itaskMemDao);
                                    }

                                }
                            }
                        }

                    }


                    tx.Commit();
                    return fileid;
                }

            }
            catch (Exception ex)
            {
                return 0;
            }


        }

        public bool InsertTaskAttachments(TaskManagerProxy proxy, int taskID, IList<INewTaskAttachments> TaskAttachments, IList<IFile> TaskFiles)
        {
            try
            {
                BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("-----------------------------------------------------------------------------------------------", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);
                BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("Started creating Task", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);

                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    var adminObj = tx.PersistenceManager.TaskRepository.Query<AdminTaskDao>().ToList().Where(item => item.ID == taskID).Select(item => item.TaskType).FirstOrDefault();
                    IList<TaskFileDao> ifile = new List<TaskFileDao>();
                    Dictionary<Guid, string> descriptionObj = new Dictionary<Guid, string>();
                    if (TaskFiles != null)
                    {

                        foreach (var a in TaskFiles)
                        {
                            Guid NewId = Guid.NewGuid();

                            string filePath = ReadAdminXML("FileManagment");
                            var DirInfo = System.IO.Directory.GetParent(filePath);
                            string newFilePath = DirInfo.FullName;
                            System.IO.File.Move(filePath + "\\" + a.strFileID.ToString() + a.Extension, newFilePath + "\\" + NewId + a.Extension);
                            TaskFileDao fldao = new TaskFileDao();
                            fldao.Checksum = a.Checksum;
                            fldao.CreatedOn = a.CreatedOn;
                            fldao.Entityid = taskID;
                            fldao.Extension = a.Extension;
                            fldao.MimeType = a.MimeType;
                            fldao.Moduleid = a.Moduleid;
                            fldao.Name = a.Name;
                            fldao.Ownerid = a.Ownerid;
                            fldao.Size = a.Size;
                            fldao.VersionNo = a.VersionNo;
                            fldao.Fileguid = NewId;
                            fldao.Description = a.Description;
                            ifile.Add(fldao);
                            //descriptionObj.Add(NewId, a.Description);

                        }
                        tx.PersistenceManager.PlanningRepository.Save<TaskFileDao>(ifile);
                        BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("Saved in File", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);
                    }


                    if (ifile != null)
                    {

                        IList<NewTaskAttachmentsDao> iattachment = new List<NewTaskAttachmentsDao>();
                        foreach (var a in ifile)
                        {
                            var result = descriptionObj.Where(r => r.Key == a.Fileguid).ToList();
                            NewTaskAttachmentsDao attachedao = new NewTaskAttachmentsDao();
                            attachedao.ActiveFileid = a.Id;
                            attachedao.ActiveVersionNo = 1;
                            attachedao.Createdon = DateTime.Now;
                            attachedao.Entityid = taskID;
                            attachedao.Name = a.Name;
                            attachedao.Typeid = adminObj;
                            //attachedao.Description = result[0].Value;
                            iattachment.Add(attachedao);
                        }
                        tx.PersistenceManager.PlanningRepository.Save<NewTaskAttachmentsDao>(iattachment);
                        BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("Saved in Task Members", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);

                    }

                    tx.Commit();
                    return true;

                }

            }
            catch (Exception ex)
            {

            }

            return false;
        }

        public bool UpdateTemplateTaskListSortOrder(TaskManagerProxy proxy, int TemplateId, int taskListid, int sortorder)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    TempTaskListDao tasklistdao = new TempTaskListDao();
                    tasklistdao = (from item in tx.PersistenceManager.TaskRepository.Query<TempTaskListDao>() where item.TaskListID == taskListid && item.TempID == TemplateId select item).FirstOrDefault();
                    if (tasklistdao != null)
                    {
                        tasklistdao.Sortorder = sortorder;
                    }
                    tx.PersistenceManager.TaskRepository.Save<TempTaskListDao>(tasklistdao);
                    tx.Commit();
                    return true;
                }
            }
            catch
            {
                return false;
            }

        }

        public bool DeleteAttachments(TaskManagerProxy proxy, int ActiveFileid)
        {
            try
            {

                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    NewTaskAttachmentsDao attachDao = new NewTaskAttachmentsDao();
                    attachDao = (from item in tx.PersistenceManager.TaskRepository.Query<NewTaskAttachmentsDao>() where item.ActiveFileid == ActiveFileid select item).FirstOrDefault();
                    TaskFileDao fileDao = new TaskFileDao();

                    string newFilePath = "";
                    fileDao = (from item in tx.PersistenceManager.TaskRepository.Query<TaskFileDao>() where item.Id == ActiveFileid select item).FirstOrDefault();
                    if (attachDao != null)
                        tx.PersistenceManager.PlanningRepository.Delete<NewTaskAttachmentsDao>(attachDao);
                    if (fileDao != null)
                    {
                        tx.PersistenceManager.PlanningRepository.Delete<TaskFileDao>(fileDao);
                        string flPath = ReadAdminXML("FileManagment");
                        var DirInfo = System.IO.Directory.GetParent(flPath);
                        newFilePath = DirInfo.FullName + "\\" + fileDao.Fileguid + fileDao.Extension;

                    }
                    tx.Commit();

                    if (System.IO.File.Exists(newFilePath))
                    {
                        System.IO.File.Delete(newFilePath);
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }

        }



        public IList<IMyTaskCollection> GetEntityUpcomingTaskList(TaskManagerProxy proxy, int FilterByentityID, int FilterStatusID, int EntityID, bool IsChildren)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    IList<MultiProperty> parLIST = new List<MultiProperty>();
                    StringBuilder MytaskQry = new StringBuilder();


                    IList<IMyTaskCollection> mytaskcollection = new List<IMyTaskCollection>();



                    MytaskQry.Append(" SELECT  tet.ID 'TaskId',tet.Name 'TaskName',tet.[Description] 'TaskDescription',pe.ID 'EntityId' , pe.Name 'EntityName',tet.EntityTaskListID 'IsAdminTask', ");
                    MytaskQry.Append(" ttm.RoleID 'RoleId', CONVERT(VARCHAR(10), tet.DueDate,111) 'DueDate',DATEDIFF(d,GETDATE(),tet.DueDate) 'Noofdays', ");
                    MytaskQry.Append("case when (DATEDIFF(d, GETDATE(), tet.DueDate)<0) then 0 when (DATEDIFF(d, GETDATE(), tet.DueDate)<7) then 1 ");
                    MytaskQry.Append(" when (DATEDIFF(d, GETDATE(), tet.DueDate)<0) then 2  else 3 end  'Weeks',");

                    MytaskQry.Append("( ");
                    MytaskQry.Append(" SELECT STUFF( ");
                    MytaskQry.Append("( ");
                    MytaskQry.Append(" SELECT ', ' + uu.FirstName +' ' + uu.LastName ");
                    MytaskQry.Append("FROM   TM_Task_Members tm2 ");
                    MytaskQry.Append("INNER JOIN UM_User uu  ");
                    MytaskQry.Append("ON  tm2.UserID = uu.ID ");
                    MytaskQry.Append("AND tm2.TaskID = tet.ID  ");

                    MytaskQry.Append("FOR XML PATH('') ");
                    MytaskQry.Append("), ");
                    MytaskQry.Append("1, ");
                    MytaskQry.Append("2, ");
                    MytaskQry.Append("'' ");
                    MytaskQry.Append(") )    ");

                    //MytaskQry.Append(" (SELECT top 1 uu2.FirstName + ' ' + uu2.LastName   FROM UM_User uu2 INNER  JOIN TM_Task_Members ttm2 ON uu2.ID=ttm2.UserID AND ttm2.TaskID=tet.ID AND ttm2.RoleID=" + (AssignRole == 1 ? 4 : 1) + ") 'UserName' ");
                    //MytaskQry.Append(" ,(SELECT top 1 uu2.ID  FROM UM_User uu2 INNER  JOIN TM_Task_Members ttm2 ON uu2.ID=ttm2.UserID AND ttm2.TaskID=tet.ID AND ttm2.RoleID=" + (AssignRole == 1 ? 4 : 1) + ") 'UserID',tet.TaskStatus 'TaskStatus',tet.TaskType 'TaskType' ");
                    //MytaskQry.Append("   FROM AM_Entity_Role_User aeru INNER JOIN  PM_Entity pe ON aeru.EntityID=pe.id	  AND pe.[Active]=1 ");
                    //MytaskQry.Append(" INNER JOIN TM_EntityTaskList tetl ON aeru.EntityID=tetl.EntityID INNER JOIN TM_EntityTask tet ON tetl.ID=tet.EntityTaskListID ");
                    //MytaskQry.Append(" INNER JOIN TM_Task_Members ttm ON tet.ID=ttm.TaskID  INNER JOIN UM_User uu ON ttm.UserID=uu.ID AND ttm.UserID=" + proxy.MarcomManager.User.Id + " ");
                    //MytaskQry.Append(" and ttm.roleid=" + AssignRole);

                    MytaskQry.Append("   FROM PM_Entity pe  ");
                    parLIST.Add(new MultiProperty { propertyName = "EntityID", propertyValue = EntityID });
                    MytaskQry.Append(" INNER JOIN  TM_EntityTask tet ON pe.ID=tet.EntityID  AND pe.[Active]=1  and tet.EntityID= :EntityID");
                    MytaskQry.Append(" INNER JOIN TM_Task_Members ttm ON tet.ID=ttm.TaskID  ");
                    //INNER JOIN UM_User uu ON ttm.UserID=uu.ID AND ttm.UserID=" + proxy.MarcomManager.User.Id + " ");
                    //MytaskQry.Append(" and ttm.roleid=" + AssignRole);

                    if (FilterStatusID != -1)
                        parLIST.Add(new MultiProperty { propertyName = "FilterStatusID", propertyValue = FilterStatusID });
                    MytaskQry.Append(" and tet.TaskStatus= :FilterStatusID");
                    if (FilterByentityID == 2)
                        MytaskQry.Append(" ORDER BY pe.UniqueKey,DATEDIFF(d,GETDATE(),tet.DueDate) ");
                    else
                        MytaskQry.Append(" ORDER BY DATEDIFF(d,GETDATE(),tet.DueDate) ");

                    //MytaskQry.Append("  offset " + StartRowno + " ROWS FETCH NEXT " + MaxRowNo + " ROWS only ");
                    //MytaskQry.Append(" and ttm.roleid=" + AssignRole + " ORDER BY pe.UniqueKey offset " + StartRowno + " ROWS FETCH NEXT " + MaxRowNo + " ROWS only ");

                    var MyTaskResult = tx.PersistenceManager.PlanningRepository.ExecuteQuerywithParam(MytaskQry.ToString(), parLIST).Cast<Hashtable>();//<EntityTaskListDao>().Where(a => a.EntityID == entityID).Select(a => a);
                    //FilterByentityID = 1;


                    if (FilterByentityID == 2)
                    {
                        IList<int> LstofEntityIds = new List<int>();
                        foreach (var CurrentId in MyTaskResult)
                        {
                            if (!LstofEntityIds.Contains(Convert.ToInt32(CurrentId["EntityId"])))
                            {
                                LstofEntityIds.Add(Convert.ToInt32(CurrentId["EntityId"]));
                            }
                        }

                        foreach (var EntityIds in LstofEntityIds)
                        {
                            IMyTaskCollection mytaskCurrentdata = new MyTaskCollection();
                            List<MyTask> mytasklst = new List<MyTask>();
                            foreach (var val in MyTaskResult)
                            {
                                if (EntityIds == Convert.ToInt32(val["EntityId"]))
                                {

                                    MyTask Currentmytask = new MyTask();
                                    //Currentmytask.UserName = val["UserName"].ToString();
                                    //Currentmytask.UserID = Convert.ToInt32(val["UserID"]);
                                    Currentmytask.DueDate = val["DueDate"].ToString();//DateTime.Parse((string)val["DueDate"], CultureInfo.InvariantCulture); //Convert.ToDateTime(val["DueDate"]);  //DateTime.Parse(ab.databaseval.Value == null ? "" : (string)ab.databaseval.Value, CultureInfo.InvariantCulture)
                                    Currentmytask.EntityId = Convert.ToInt32(val["EntityId"]);
                                    Currentmytask.EntityName = val["EntityName"].ToString();
                                    Currentmytask.Noofdays = Convert.ToInt32(val["Noofdays"]);
                                    //Currentmytask.RoleId = Convert.ToInt32(val["RoleId"]);
                                    Currentmytask.TaskDescription = val["TaskDescription"].ToString();
                                    Currentmytask.TaskId = Convert.ToInt32(val["TaskId"]);
                                    Currentmytask.IsAdminTask = Convert.ToInt32(val["IsAdminTask"]);
                                    Currentmytask.TaskName = val["TaskName"].ToString();
                                    Currentmytask.Path = proxy.MarcomManager.MetadataManager.GetPath(Convert.ToInt32(val["TaskId"]));
                                    Currentmytask.TaskStatus = Convert.ToInt32(val["TaskStatus"]);
                                    Currentmytask.TaskTypeId = Convert.ToInt32(val["TaskType"]);
                                    mytasklst.Add(Currentmytask);
                                    mytaskCurrentdata.FilterHeaderName = val["EntityName"].ToString();
                                }
                            }
                            mytaskCurrentdata.TaskCollection = mytasklst;
                            mytaskcollection.Add(mytaskCurrentdata);
                        }
                    }

                    else if (FilterByentityID == 1)
                    {

                        foreach (var EntityIds in Enum.GetValues(typeof(UpcomingWeeks)))
                        {
                            IMyTaskCollection mytaskCurrentdata = new MyTaskCollection();
                            List<MyTask> mytasklst = new List<MyTask>();
                            bool IsWeeks = false;
                            foreach (var val in MyTaskResult)
                            {
                                if ((int)EntityIds == Convert.ToInt32(val["Weeks"]))
                                {
                                    MyTask Currentmytask = new MyTask();
                                    //Currentmytask.UserName = val["UserName"].ToString();
                                    //Currentmytask.UserID = Convert.ToInt32(val["UserID"]);
                                    Currentmytask.DueDate = val["DueDate"].ToString();//DateTime.Parse((string)val["DueDate"], CultureInfo.InvariantCulture); //Convert.ToDateTime(val["DueDate"]);  //DateTime.Parse(ab.databaseval.Value == null ? "" : (string)ab.databaseval.Value, CultureInfo.InvariantCulture)
                                    Currentmytask.EntityId = Convert.ToInt32(val["EntityId"]);
                                    Currentmytask.EntityName = val["EntityName"].ToString();
                                    Currentmytask.Noofdays = Convert.ToInt32(val["Noofdays"]);
                                    //Currentmytask.RoleId = Convert.ToInt32(val["RoleId"]);
                                    Currentmytask.TaskDescription = val["TaskDescription"].ToString();
                                    Currentmytask.TaskId = Convert.ToInt32(val["TaskId"]);
                                    Currentmytask.IsAdminTask = Convert.ToInt32(val["IsAdminTask"]);
                                    Currentmytask.TaskName = val["TaskName"].ToString();
                                    Currentmytask.Path = proxy.MarcomManager.MetadataManager.GetPath(Convert.ToInt32(val["TaskId"]));
                                    Currentmytask.TaskStatus = Convert.ToInt32(val["TaskStatus"]);
                                    Currentmytask.TaskTypeId = Convert.ToInt32(val["TaskType"]);
                                    mytasklst.Add(Currentmytask);
                                    mytaskCurrentdata.FilterHeaderName = ((int)EntityIds != (int)UpcomingWeeks.Upcoming ? Enum.GetName(typeof(UpcomingWeeks), EntityIds).Replace("_", " ") : Enum.GetName(typeof(UpcomingWeeks), EntityIds) + "(2 weeks or more)");
                                    // MyTaskResult.ToList().re
                                    MyTaskResult.ToList().Remove(val);
                                    IsWeeks = true;
                                    //mytaskCurrentdata.FilterHeaderName = val["EntityName"].ToString();
                                }
                            }
                            if (IsWeeks == true)
                            {
                                mytaskCurrentdata.TaskCollection = mytasklst;
                                mytaskcollection.Add(mytaskCurrentdata);
                            }
                        }
                    }

                    else
                    {
                        IMyTaskCollection mytaskCurrentdata = new MyTaskCollection();
                        List<MyTask> mytasklst = new List<MyTask>();
                        foreach (var val in MyTaskResult)
                        {

                            //string x = proxy.MarcomManager.GlobalAdditionalSettings[0].SettingValue;
                            //DateTime z = DateTime.ParseExact(val["DueDate"].ToString(), proxy.MarcomManager.GlobalAdditionalSettings[0].SettingValue.ToString(), CultureInfo.InvariantCulture);

                            MyTask Currentmytask = new MyTask();
                            //Currentmytask.UserName = val["UserName"].ToString();
                            //Currentmytask.UserID = Convert.ToInt32(val["UserID"]);
                            Currentmytask.DueDate = val["DueDate"].ToString();//DateTime.Parse((string)val["DueDate"], CultureInfo.InvariantCulture); //Convert.ToDateTime(val["DueDate"]);  //DateTime.Parse(ab.databaseval.Value == null ? "" : (string)ab.databaseval.Value, CultureInfo.InvariantCulture)
                            Currentmytask.EntityId = Convert.ToInt32(val["EntityId"]);
                            Currentmytask.EntityName = val["EntityName"].ToString();
                            Currentmytask.Noofdays = Convert.ToInt32(val["Noofdays"]);
                            //Currentmytask.RoleId = Convert.ToInt32(val["RoleId"]);
                            Currentmytask.TaskDescription = val["TaskDescription"].ToString();
                            Currentmytask.TaskId = Convert.ToInt32(val["TaskId"]);
                            Currentmytask.IsAdminTask = Convert.ToInt32(val["IsAdminTask"]);
                            Currentmytask.TaskName = val["TaskName"].ToString();
                            Currentmytask.Path = proxy.MarcomManager.MetadataManager.GetPath(Convert.ToInt32(val["TaskId"]));
                            Currentmytask.TaskStatus = Convert.ToInt32(val["TaskStatus"]);
                            Currentmytask.TaskTypeId = Convert.ToInt32(val["TaskType"]);
                            mytasklst.Add(Currentmytask);


                        }
                        mytaskCurrentdata.TaskCollection = mytasklst;
                        mytaskcollection.Add(mytaskCurrentdata);
                    }
                    return mytaskcollection;
                }
            }
            catch
            {
                return null;
            }

        }

        public IList<ITaskLibraryTemplateHolder> GetEntityTaskList(TaskManagerProxy proxy, int entityID)
        {

            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    IList<ITaskLibraryTemplateHolder> tasklist = new List<ITaskLibraryTemplateHolder>();
                    var entityTaskList = tx.PersistenceManager.PlanningRepository.Query<EntityTaskListDao>().Where(a => a.EntityID == entityID).Select(a => a).OrderBy(a => a.Sortorder);
                    List<int> idarr = entityTaskList.Select(a => a.ID).ToList();
                    foreach (var val in entityTaskList)
                    {
                        //var entityTasks = tx.PersistenceManager.PlanningRepository.Query<EntityTaskDao>().Where(a => a.EntityID == entityID && a.TaskListID == val.TaskListID).Select(a => a);
                        ITaskLibraryTemplateHolder tskLst = new TaskLibraryTemplateHolder();
                        tskLst.LibraryName = HttpUtility.HtmlDecode(val.Name);
                        tskLst.LibraryDescription = HttpUtility.HtmlDecode(val.Description);
                        tskLst.ID = val.ID;
                        tskLst.SortOrder = val.Sortorder;
                        tskLst.TaskList = GetEntityTaskListDetails(proxy, val.EntityID, val.ID);
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

        /// <summary>
        /// Getting Task details
        /// </summary>
        ///  <param name="proxy"></param>
        /// <param name="StepID">The TaskListID</param>
        /// <returns>IList of IAdminTask</returns>
        public IList<IEntityTask> GetEntityTaskListDetails(TaskManagerProxy proxy, int entityID, int taskListID)
        {
            IList<IEntityTask> iitaskDetails = new List<IEntityTask>();
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    var TaskList = (from task in tx.PersistenceManager.PlanningRepository.Query<EntityTaskDao>() where task.EntityID == entityID && task.TaskListID == taskListID select task).OrderBy(a => a.Sortorder).ToList();

                    object _lock = new object();
                    foreach (var val in TaskList)
                    {
                        IEntityTask admintsk = new EntityTask();
                        admintsk.Name = HttpUtility.HtmlDecode(val.Name);
                        admintsk.Description = val.Description != null ? HttpUtility.HtmlDecode(val.Description) : "-";
                        admintsk.Id = val.ID;
                        var basedao = (from task in tx.PersistenceManager.PlanningRepository.Query<BaseEntityDao>() where task.Id == val.ID select task).FirstOrDefault();
                        var typedao = (from task in tx.PersistenceManager.PlanningRepository.Query<EntityTypeDao>() where task.Id == basedao.Typeid select task);
                        admintsk.TaskTypeID = basedao.Typeid;
                        admintsk.TaskTypeName = Enum.GetName(typeof(TaskTypes), val.TaskType).Replace("_", " ");
                        admintsk.TaskType = val.TaskType;
                        admintsk.TaskListID = val.TaskListID;
                        admintsk.colorcode = "";
                        admintsk.shortdesc = "";
                        if (typedao != null)
                        {
                            admintsk.colorcode = typedao.FirstOrDefault().ColorCode;
                            admintsk.shortdesc = typedao.FirstOrDefault().ShortDescription;
                        }
                        admintsk.EntityID = val.EntityID;
                        admintsk.TaskStatus = val.TaskStatus;
                        admintsk.Note = val.Note != null ? HttpUtility.HtmlDecode(val.Note) : "-";
                        admintsk.EntityTaskListID = val.EntityTaskListID;
                        admintsk.AssetId = val.AssetId;
                        if (val.TaskStatus != 8)
                            admintsk.StatusName = Enum.GetName(typeof(TaskStatus), val.TaskStatus).Replace("_", " ");
                        else
                            admintsk.StatusName = val.TaskType == 3 ? "Approved" : "Completed";
                        admintsk.SortOrder = val.Sortorder;
                        admintsk.strDate = "";
                        admintsk.TaskCheckList = null;
                        admintsk.DueDate = null;
                        if (val.DueDate != null)
                        {
                            admintsk.DueDate = val.DueDate.Value;
                            if (admintsk.DueDate.Value.Date == DateTime.Now.Date)
                                admintsk.totalDueDays = 0;
                            else
                            {
                                TimeSpan tm = val.DueDate.Value.Date - DateTime.Now.Date;
                                admintsk.totalDueDays = (tm.Days);
                            }
                            admintsk.strDate = val.DueDate.Value.ToString("yyyy-MM-dd");
                        }
                        var taskmembers = GetTaskMember(proxy, val.ID);
                        var taskAssigneesList = taskmembers.Where(a => a.RoleID != 1).Select(a => a).ToList();
                        var taskchecklistCount = tx.PersistenceManager.TaskRepository.Query<EntityTaskCheckListDao>().Where(a => a.TaskId == val.ID).ToList();
                        if (taskmembers.Count() > 0)
                        {

                            admintsk.taskMembers = taskmembers;
                            if (taskAssigneesList.Count() > 0)
                            {
                                var totalTaskmembers = taskmembers.Where(a => a.TaskID == val.ID && a.RoleID != 1).ToList();
                                var currentTaskRount = totalTaskmembers.Select(a => a.ApprovalRount).First();
                                if (admintsk.TaskType != (int)TaskTypes.Work_Task)//if (admintsk.TaskType != (int)TaskTypes.Work_Task && admintsk.TaskStatus != (int)TaskStatus.Unassigned)
                                {
                                    admintsk.TotaltaskAssigness = taskmembers.Where(a => a.RoleID != 1).Select(a => a).ToList();
                                }
                                admintsk.taskAssigness = taskAssigneesList.Where(a => a.ApprovalRount == currentTaskRount);
                                admintsk.ProgressCount = "";
                                if (val.EntityTaskListID == 0)
                                {
                                    admintsk.taskAssigness = taskAssigneesList.Where(a => a.ApprovalRount == currentTaskRount);
                                    var responsedMembers = taskAssigneesList.Where(a => a.ApprovalRount == currentTaskRount && a.ApprovalStatus != null).Select(a => a).ToList().Count();
                                    if (admintsk.TaskType != (int)TaskTypes.Work_Task && (admintsk.TaskStatus != (int)TaskStatus.Unassigned && admintsk.TaskStatus != (int)TaskStatus.Approved && admintsk.TaskStatus != (int)TaskStatus.Completed))
                                    {
                                        if (val.TaskStatus == 2 || val.TaskStatus == 3 || val.TaskStatus == 4)
                                        {
                                            admintsk.ProgressCount = "";
                                        }
                                        else
                                        {
                                            admintsk.ProgressCount = "(" + responsedMembers.ToString() + "/" + taskAssigneesList.Where(a => a.ApprovalRount == currentTaskRount).Count().ToString() + ")";
                                        }
                                    }
                                    else
                                    {

                                        if (taskchecklistCount.Count > 0 && admintsk.TaskStatus == (int)TaskStatus.In_progress)
                                        {
                                            var completedChecklists = taskchecklistCount.Where(a => a.Status == true).ToList().Count();
                                            admintsk.ProgressCount = "(" + completedChecklists.ToString() + "/" + taskchecklistCount.Count().ToString() + ")";

                                        }
                                        else
                                        {
                                            admintsk.ProgressCount = "";
                                        }
                                    }
                                    if (taskAssigneesList.Count > 1)
                                    {
                                        var userIdArr = taskAssigneesList.Where(a => a.RoleID != 1 && a.ApprovalRount == currentTaskRount).Select(a => a.UserID).ToList();
                                        var assigneeNameObj = tx.PersistenceManager.UserRepository.Query<BrandSystems.Marcom.Dal.User.Model.UserDao>().Where(a => userIdArr.Contains(a.Id)).Select(a => a.FirstName + " " + a.LastName).ToArray();
                                        admintsk.AssigneeName = string.Join<string>(" , ", assigneeNameObj);
                                        admintsk.AssigneeID = 0;
                                    }
                                    else if (taskAssigneesList.Count == 1)
                                    {
                                        var assigneeNameObj = tx.PersistenceManager.UserRepository.Query<BrandSystems.Marcom.Dal.User.Model.UserDao>().Where(a => a.Id == taskAssigneesList[0].UserID).Select(a => a).FirstOrDefault();
                                        admintsk.AssigneeName = assigneeNameObj.FirstName + " " + assigneeNameObj.LastName;
                                        admintsk.AssigneeID = assigneeNameObj.Id;

                                    }

                                }
                                else
                                {
                                    admintsk.taskAssigness = null;
                                    admintsk.AssigneeName = "";
                                }
                            }
                        }
                        if (taskchecklistCount.Count > 0 && val.TaskStatus == (int)TaskStatus.Unassigned && val.TaskType == (int)TaskTypes.Work_Task)
                        {

                            var completedChecklists = taskchecklistCount.Where(a => a.Status == true).ToList().Count();
                            admintsk.ProgressCount = "(" + completedChecklists.ToString() + "/" + taskchecklistCount.Count().ToString() + ")";
                        }

                        //admintsk.AttributeData = GetEntityAttributesDetails(proxy, val.ID);
                        admintsk.AttributeData = null;
                        lock (_lock) { iitaskDetails.Add(admintsk); }
                    }

                    tx.Commit();
                }
                return iitaskDetails.OrderBy(a => a.SortOrder).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Getting Task details
        /// </summary>
        ///  <param name="proxy"></param>
        /// <param name="StepID">The TaskListID</param>
        /// <returns>IList of IAdminTask</returns>
        public IEntityTask GetEntityTaskDetails(TaskManagerProxy proxy, int EntityTaskID)
        {

            try
            {
                IEntityTask admintsk = new EntityTask();
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    var val = (from task in tx.PersistenceManager.PlanningRepository.Query<EntityTaskDao>() where task.ID == EntityTaskID select task).FirstOrDefault();

                    var basedao = (from task in tx.PersistenceManager.PlanningRepository.Query<BaseEntityDao>() where task.Id == EntityTaskID select task).FirstOrDefault();
                    var typedao = (from task in tx.PersistenceManager.PlanningRepository.Query<EntityTypeDao>() where task.Id == basedao.Typeid select task);

                    admintsk.Name = HttpUtility.HtmlDecode(val.Name);
                    admintsk.Description = val.Description != null ? HttpUtility.HtmlDecode(val.Description) : "-";
                    admintsk.Id = val.ID;
                    admintsk.TaskTypeName = Enum.GetName(typeof(TaskTypes), val.TaskType).Replace("_", " ");
                    admintsk.TaskType = val.TaskType;
                    admintsk.TaskListID = val.TaskListID;
                    admintsk.TaskStatus = val.TaskStatus;
                    admintsk.EntityID = val.EntityID;
                    admintsk.EntityTaskListID = val.EntityTaskListID;
                    admintsk.colorcode = "";
                    admintsk.shortdesc = "";
                    if (typedao != null)
                    {
                        admintsk.colorcode = (typedao.FirstOrDefault().ColorCode != null) ? typedao.FirstOrDefault().ColorCode : "";
                        admintsk.shortdesc = (typedao.FirstOrDefault().ShortDescription != null) ? typedao.FirstOrDefault().ShortDescription : "";
                    }
                    admintsk.Note = val.Note != null ? HttpUtility.HtmlDecode(val.Note) : "-";
                    if (val.TaskStatus != 8)
                        admintsk.StatusName = Enum.GetName(typeof(TaskStatus), val.TaskStatus).Replace("_", " ");
                    else
                        admintsk.StatusName = val.TaskType == 3 ? "Approved" : "Completed";
                    admintsk.SortOrder = val.Sortorder;
                    admintsk.strDate = "";
                    admintsk.TaskCheckList = null;
                    admintsk.TaskTypeID = basedao.Typeid;
                    admintsk.DueDate = null;
                    if (val.DueDate != null)
                    {
                        admintsk.DueDate = val.DueDate.Value;
                        if (admintsk.DueDate.Value.Date == DateTime.Now.Date)
                            admintsk.totalDueDays = 0;
                        else
                        {
                            TimeSpan tm = val.DueDate.Value.Date - DateTime.Now.Date;
                            admintsk.totalDueDays = (tm.Days);
                        }
                        admintsk.strDate = val.DueDate.Value.ToString("yyyy-MM-dd");
                    }
                    var taskmembers = GetTaskMember(proxy, val.ID);
                    var taskAssigneesList = taskmembers.Where(a => a.RoleID != 1).Select(a => a).ToList();
                    var taskchecklistCount = tx.PersistenceManager.TaskRepository.Query<EntityTaskCheckListDao>().Where(a => a.TaskId == val.ID).ToList();
                    if (taskmembers.Count() > 0)
                    {

                        admintsk.taskMembers = taskmembers;
                        if (taskAssigneesList.Count() > 0)
                        {
                            var totalTaskmembers = taskmembers.Where(a => a.TaskID == val.ID && a.RoleID != 1).ToList();
                            var currentTaskRount = totalTaskmembers.Select(a => a.ApprovalRount).First();
                            if (admintsk.TaskType != (int)TaskTypes.Work_Task)
                            {
                                admintsk.TotaltaskAssigness = taskmembers.Where(a => a.RoleID != 1).Select(a => a).ToList();
                            }
                            admintsk.taskAssigness = taskAssigneesList.Where(a => a.ApprovalRount == currentTaskRount);
                            admintsk.ProgressCount = "";
                            if (val.EntityTaskListID == 0)
                            {
                                admintsk.taskAssigness = taskAssigneesList.Where(a => a.ApprovalRount == currentTaskRount);
                                var responsedMembers = taskAssigneesList.Where(a => a.ApprovalRount == currentTaskRount && a.ApprovalStatus != null).Select(a => a).ToList().Count();
                                if (admintsk.TaskType != (int)TaskTypes.Work_Task && (admintsk.TaskStatus != (int)TaskStatus.Unassigned && admintsk.TaskStatus != (int)TaskStatus.Approved && admintsk.TaskStatus != (int)TaskStatus.Completed))
                                {
                                    if (val.TaskStatus == 2 || val.TaskStatus == 3 || val.TaskStatus == 4)
                                    {
                                        admintsk.ProgressCount = "";
                                    }
                                    else
                                    {
                                        admintsk.ProgressCount = "(" + responsedMembers.ToString() + "/" + taskAssigneesList.Where(a => a.ApprovalRount == currentTaskRount).Count().ToString() + ")";
                                    }
                                }
                                else
                                {
                                    if (taskchecklistCount.Count > 0 && admintsk.TaskStatus == (int)TaskStatus.In_progress)
                                    {
                                        var completedChecklists = taskchecklistCount.Where(a => a.Status == true).ToList().Count();
                                        admintsk.ProgressCount = "(" + completedChecklists.ToString() + "/" + taskchecklistCount.Count().ToString() + ")";


                                    }
                                    else
                                    {
                                        admintsk.ProgressCount = "";
                                    }
                                }
                                if (taskAssigneesList.Count > 1)
                                {
                                    var userIdArr = taskAssigneesList.Where(a => a.RoleID != 1 && a.ApprovalRount == currentTaskRount).Select(a => a.UserID).ToList();
                                    var assigneeNameObj = tx.PersistenceManager.UserRepository.Query<BrandSystems.Marcom.Dal.User.Model.UserDao>().Where(a => userIdArr.Contains(a.Id)).Select(a => a.FirstName + " " + a.LastName).ToArray();
                                    admintsk.AssigneeName = string.Join<string>(" , ", assigneeNameObj);
                                    admintsk.AssigneeID = 0;
                                }
                                else if (taskAssigneesList.Count == 1)
                                {
                                    var assigneeNameObj = tx.PersistenceManager.UserRepository.Query<BrandSystems.Marcom.Dal.User.Model.UserDao>().Where(a => a.Id == taskAssigneesList[0].UserID).Select(a => a).FirstOrDefault();
                                    if (assigneeNameObj != null)
                                    {
                                        admintsk.AssigneeName = assigneeNameObj.FirstName + " " + assigneeNameObj.LastName;
                                        admintsk.AssigneeID = assigneeNameObj.Id;
                                    }
                                    else
                                    {
                                        admintsk.AssigneeName = " ";
                                        admintsk.AssigneeID = 0;
                                    }

                                }

                            }
                            else
                            {
                                admintsk.taskAssigness = null;
                                admintsk.AssigneeName = "";
                            }
                        }
                    }
                    if (taskchecklistCount.Count > 0 && val.TaskStatus == (int)TaskStatus.Unassigned && val.TaskType == (int)TaskTypes.Work_Task)
                    {
                        var completedChecklists = taskchecklistCount.Where(a => a.Status == true).ToList().Count();
                        admintsk.ProgressCount = "(" + completedChecklists.ToString() + "/" + taskchecklistCount.Count().ToString() + ")";
                    }
                    admintsk.AttributeData = null;

                }

                return admintsk;
            }
            catch
            {
                return null;
            }
        }

        public IList<IMilestoneMetadata> GetEntityMetadata(TaskManagerProxy proxy, int entityId, int entitytypeId)
        {
            IList<IMilestoneMetadata> listMilestone = new List<IMilestoneMetadata>();
            string entityName = "AttributeRecord" + entitytypeId + "_V" + MarcomManagerFactory.ActiveMetadataVersionNumber;
            List<int> attrIds = new List<int>();
            using (ITransaction tx = proxy.MarcomManager.GetTransaction())
            {
                //var attrResult = tx.PersistenceManager.PlanningRepository.GetAll<DynamicAttributesDao>(entityName).Where(a => Convert.ToInt32(a.Attributes["" + Convert.ToInt32(SystemDefinedAttributes.MilestoneEntityID) + ""]) == entityId);
                var attrResult = tx.PersistenceManager.PlanningRepository.GetAll<DynamicAttributesDao>(entityName).Where(a => a.Id == entityId);
                var entityTypeResult = tx.PersistenceManager.PlanningRepository.Query<EntityTypeAttributeRelationDao>().Where(a => a.EntityTypeID == entitytypeId).ToList();
                var attributeResult = tx.PersistenceManager.PlanningRepository.Query<AttributeDao>();
                foreach (var obj in attrResult)
                {
                    MilestoneMetadata milestonedata = new MilestoneMetadata();
                    IList<IAttributeData> listAttr = new List<IAttributeData>();
                    milestonedata.EntityId = obj.Id;
                    milestonedata.AttributeData = null;
                    var getMilestoneName = tx.PersistenceManager.PlanningRepository.Get<EntityTaskDao>(milestonedata.EntityId);

                    AttributeData attrformilestone = new AttributeData();
                    attrformilestone.ID = Convert.ToInt32(SystemDefinedAttributes.Name);
                    attrformilestone.Caption = entityTypeResult.Where(a => a.AttributeID == (Convert.ToInt32(SystemDefinedAttributes.Name)) && a.EntityTypeID == entitytypeId).Select(a => a.Caption).First();
                    attrformilestone.Value = getMilestoneName.Name;
                    attrformilestone.TypeID = attributeResult.Where(a => a.Id == Convert.ToInt32(SystemDefinedAttributes.Name)).Select(a => a.AttributeTypeID).First();
                    attrformilestone.IsSpecial = true;
                    attrformilestone.Lable = entityTypeResult.Where(a => a.AttributeID == (Convert.ToInt32(SystemDefinedAttributes.Name)) && a.EntityTypeID == entitytypeId).Select(a => a.Caption).First();
                    attrformilestone.Level = 0;
                    listAttr.Add(attrformilestone);
                    if (obj.Attributes != null)
                    {
                        foreach (DictionaryEntry ob in obj.Attributes)
                        {
                            AttributeData attr = new AttributeData();
                            int attributeid = Convert.ToInt32((object)ob.Key);
                            attr.ID = attributeResult.Where(a => a.Id == attributeid).Select(a => a.Id).First();
                            attr.TypeID = attributeResult.Where(a => a.Id == attributeid).Select(a => a.AttributeTypeID).First();
                            attr.Caption = entityTypeResult.Where(a => a.AttributeID == attributeid && a.EntityTypeID == entitytypeId).Select(a => a.Caption).First();
                            if (Convert.ToInt32(ob.Key) == Convert.ToInt32(SystemDefinedAttributes.DueDate))
                                attr.Value = ((System.DateTime)(ob.Value)).Date.ToString();
                            else
                                attr.Value = ob.Value;
                            listAttr.Add(attr);

                        }
                    }
                    milestonedata.AttributeData = listAttr;
                    listMilestone.Add(milestonedata);
                }
                tx.Commit();
            }
            return listMilestone;
        }

        /// <summary>
        /// Gets the EntityName  by entityID.
        /// </summary>
        /// <param name="Id">The entityId.</param>
        /// <returns>
        /// EntityName
        /// </returns>
        public string GetEntityName(ITransaction tx, int entityId, int mappingFileVersion = 0)
        {
            string entityName = string.Empty;
            EntityDao dao = new EntityDao();
            var result = tx.PersistenceManager.PlanningRepository.Get<EntityDao>(entityId);
            if (mappingFileVersion == 0)
                entityName = "AttributeRecord" + result.Typeid + "_V" + MarcomManagerFactory.ActiveMetadataVersionNumber;
            else
                entityName = "AttributeRecord" + result.Typeid + "_V" + mappingFileVersion.ToString();
            return entityName;
        }

        /// <summary>
        /// Gets the attributes details by entityID.
        /// </summary>
        /// <param name="Id">The entityId.</param>
        /// <returns>
        /// Ilist
        /// </returns>
        public IList<IAttributeData> GetEntityAttributesDetails(TaskManagerProxy proxy, int entityId)
        {

            IList<IAttributeData> attributesWithValues = new List<IAttributeData>();
            IList<ITreeDropDownLabel> droplabel;
            IList<ITreeDropDownCaption> itreeCaption = new List<ITreeDropDownCaption>();
            AttributeData attributedate;
            try
            {

                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {

                    //var allattributes = tx.PersistenceManager.PlanningRepository.GetAll<BrandSystems.Marcom.Dal.Metadata.Model.AttributeDao>();
                    var entityObj = (from item in tx.PersistenceManager.PlanningRepository.Query<EntityDao>()
                                     where item.Id == entityId
                                     select item).FirstOrDefault();
                    IList<EntityPeriod> listentPeriods = new List<EntityPeriod>();
                    foreach (var aobe in entityObj.Periods)
                    {
                        EntityPeriod entpr = new EntityPeriod();
                        entpr.Id = aobe.Id;
                        entpr.Entityid = aobe.Entityid;
                        entpr.Startdate = aobe.Startdate;
                        entpr.EndDate = aobe.EndDate;
                        entpr.Description = aobe.Description != "" ? aobe.Description : "-";
                        entpr.SortOrder = aobe.SortOrder;
                        listentPeriods.Add(entpr);
                    }
                    string xmlpath = tx.PersistenceManager.MetadataRepository.GetXmlPath(entityObj.Version);
                    int[] notAllowedAttrs = { (int)SystemDefinedAttributes.Name, 81, 82, 83, 84 };
                    XDocument docx = XDocument.Load(xmlpath);
                    var rddd = (from EntityAttrRel in docx.Root.Elements("EntityTypeAttributeRelation_Table").Elements("EntityTypeAttributeRelation")
                                join Attr in docx.Root.Elements("Attribute_Table").Elements("Attribute") on Convert.ToInt32(EntityAttrRel.Element("AttributeID").Value) equals Convert.ToInt32(Attr.Element("ID").Value)
                                where Convert.ToInt32(EntityAttrRel.Element("EntityTypeID").Value) == entityObj.Typeid && !notAllowedAttrs.Contains(Convert.ToInt32(Attr.Element("ID").Value))
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
                    if (rddd.Count() > 0)
                    {
                        var entityName = GetEntityName(tx, entityId, entityObj.Version);
                        var dynamicvalues = tx.PersistenceManager.PlanningRepository.GetAll<DynamicAttributesDao>(entityName).Where(a => a.Id == entityId).Select(a => a.Attributes).SingleOrDefault();



                        foreach (var val in attributesdetails)
                        {


                            AttributesList attypeid = (AttributesList)val.AttributeTypeID;
                            if (Convert.ToInt32(AttributesList.DropDownTree) == val.AttributeTypeID || Convert.ToInt32(AttributesList.DropDownTree) == val.AttributeTypeID)
                            {
                                treevaluedao = new List<TreeValueDao>();
                                treevaluedao = tx.PersistenceManager.PlanningRepository.Query<TreeValueDao>().Where(a => a.Entityid == entityId && a.Attributeid == val.ID).OrderBy(a => a.Level).ToList();
                                treevalues = new List<int>();
                                treevalues = (from treevalue in treevaluedao where treevalue.Attributeid == val.ID select treevalue.Nodeid).ToList();
                            }
                            if (Convert.ToInt32(AttributesList.TreeMultiSelection) == val.AttributeTypeID || Convert.ToInt32(AttributesList.TreeMultiSelection) == val.AttributeTypeID || Convert.ToInt32(AttributesList.DropDownTreePricing) == val.AttributeTypeID || Convert.ToInt32(AttributesList.DropDownTreePricing) == val.AttributeTypeID)
                            {
                                multiselecttreevalues = new List<TreeValueDao>();
                                multiselecttreevalues = tx.PersistenceManager.PlanningRepository.Query<TreeValueDao>().Where(a => a.Entityid == entityId && a.Attributeid == val.ID).OrderBy(a => a.Level).ToList();
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
                                        attributedate.Value = (string)entityObj.Name;
                                    }
                                    else
                                    {
                                        if(dynamicvalues != null)
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
                                    {
                                        attributedate.Caption = dynamicvalues[val.ID.ToString()] == "" ? "-" : (dynamic)dynamicvalues[val.ID.ToString()];
                                        attributedate.Value = (dynamic)dynamicvalues[val.ID.ToString()];
                                    }
                                    else
                                    {
                                        attributedate.Caption = "-";
                                        attributedate.Value = "-";
                                    }
                                    attributedate.ID = val.ID;
                                    attributedate.TypeID = val.AttributeTypeID;
                                    attributedate.Lable = val.Caption.Trim();

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
                                            //attributedate.Value = (from item in tx.PersistenceManager.PlanningRepository.Query<EntityTypeRoleAclDao>() where item.EntityTypeID == entityObj.Typeid && item.Roleid == 1 select item.Userid).First();
                                            var currentRole = tx.PersistenceManager.PlanningRepository.Query<EntityTypeRoleAclDao>().Where(t => t.EntityTypeID == entityObj.Typeid && (EntityRoles)t.EntityRoleID == EntityRoles.Owner).SingleOrDefault();
                                            attributedate.Value = (from item in tx.PersistenceManager.PlanningRepository.Query<EntityRoleUserDao>() where item.Entityid == entityId && item.Roleid == currentRole.ID select item.Userid).First();
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
                                    var multiSelectValuedao = (from item in tx.PersistenceManager.PlanningRepository.Query<MultiSelectDao>()
                                                               where item.Entityid == entityId
                                                               select item).ToList();
                                    attributedate = new AttributeData();
                                    attributedate.ID = val.ID;
                                    attributedate.Lable = val.Caption.Trim();
                                    attributedate.IsSpecial = val.IsSpecial;
                                    attributedate.TypeID = val.AttributeTypeID;
                                    var optionIDs = (from multiValues in multiSelectValuedao where multiValues.Attributeid == val.ID select multiValues.Optionid).ToArray();
                                    var optioncaption = (from item in tx.PersistenceManager.PlanningRepository.Query<OptionDao>() where optionIDs.Contains(item.Id) select item.Caption).ToList();
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
                                    attributedate.Lable = val.Caption.Trim(); ;
                                    if (dynamicvalues != null)
                                    {
                                        if ((object)dynamicvalues[val.ID.ToString()] != null)
                                            attributedate.Value = (object)dynamicvalues[val.ID.ToString()];
                                        else
                                            attributedate.Value = null;
                                    }
                                    else {
                                        attributedate.Value = null;
                                    }
                                    attributedate.IsReadOnly = val.IsReadOnly;

                                    attributesWithValues.Add(attributedate);
                                    break;

                                case AttributesList.DropDownTree:
                                    attributedate = new AttributeData();
                                    attributedate.ID = val.ID;
                                    attributedate.IsSpecial = val.IsSpecial;
                                    droplabel = new List<ITreeDropDownLabel>();

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
                                            ITreeDropDownLabel dropdownlabel = new TreeDropDownLabel();
                                            ITreeDropDownCaption treecaption = new TreeDropDownCaption();
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
                                                    ITreeDropDownLabel dropdownlabel2 = new TreeDropDownLabel();
                                                    ITreeDropDownCaption treecaption2 = new TreeDropDownCaption();
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
                                            ITreeDropDownLabel dropdownlabel = new TreeDropDownLabel();
                                            ITreeDropDownCaption treecaption = new TreeDropDownCaption();
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
                                    attributedate.IsReadOnly = val.IsReadOnly;
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
                                    attributedate.IsReadOnly = val.IsReadOnly;
                                    if (!val.ChooseFromParent)
                                        attributedate.tree = proxy.MarcomManager.MetadataManager.GetAttributeTreeNode(val.ID, entityId);
                                    else
                                        attributedate.tree = proxy.MarcomManager.MetadataManager.GetDetailAttributeTreeNodeFromParent(val.ID, entityId, val.ChooseFromParent);
                                    attributesWithValues.Add(attributedate);
                                    break;

                                case AttributesList.DropDownTreePricing:
                                    attributedate = new AttributeData();
                                    attributedate.ID = val.ID;
                                    attributedate.TypeID = val.AttributeTypeID;
                                    attributedate.IsSpecial = val.IsSpecial;
                                    attributedate.Lable = val.Caption.Trim();
                                    attributedate.Value = treevalues;
                                    attributedate.IsReadOnly = val.IsReadOnly;
                                    if (val.ChooseFromParent)
                                        attributedate.DropDownPricing = proxy.MarcomManager.MetadataManager.GetDropDownTreePricingObjectFromParentDetail(val.ID, val.InheritFromParent, true, entityId, entityObj.Parentid);
                                    else
                                        attributedate.DropDownPricing = proxy.MarcomManager.MetadataManager.GetDropDownTreePricingObjectDetail(val.ID, val.InheritFromParent, false, entityId, 0);
                                    attributesWithValues.Add(attributedate);
                                    break;

                                case AttributesList.Period:
                                    attributedate = new AttributeData();
                                    attributedate.ID = val.ID;
                                    attributedate.TypeID = val.AttributeTypeID;
                                    attributedate.IsSpecial = val.IsSpecial;
                                    attributedate.Caption = val.Caption;
                                    attributedate.Lable = val.Caption;
                                    var periods = listentPeriods;
                                    //var periods = entityObj.Periods;
                                    if (periods.Count() > 0)
                                        attributedate.Value = periods.ToList();
                                    else
                                        attributedate.Value = "-";
                                    attributedate.IsReadOnly = val.IsReadOnly;
                                    attributesWithValues.Add(attributedate);
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

                                    attributedate.IsReadOnly = val.IsReadOnly;

                                    attributesWithValues.Add(attributedate);
                                    break;
                                case AttributesList.TreeMultiSelection:
                                    attributedate = new AttributeData();
                                    attributedate.ID = val.ID;
                                    attributedate.IsSpecial = val.IsSpecial;
                                    attributedate.IsReadOnly = val.IsReadOnly;

                                    droplabel = new List<ITreeDropDownLabel>();

                                    //var multiselecttreeLevelList = tx.PersistenceManager.PlanningRepository.Query<TreeLevelDao>().Where(a => a.AttributeID == val.ID).ToList();

                                    var multiselecttreeLevelList = tx.PersistenceManager.MetadataRepository.GetObject<TreeLevelDao>(xmlpath).Where(a => a.AttributeID == val.ID).ToList();


                                    List<int> multiselectdropdownResults = new List<int>();
                                    if (multiselecttreevalues.Count > 0)
                                    {
                                        foreach (var lvlObj in multiselecttreevalues)
                                        {
                                            multiselecttreeLevelList.Remove(multiselecttreeLevelList.Where(a => a.Level == lvlObj.Level).FirstOrDefault());
                                        }
                                        var entityTreeLevelList = multiselecttreevalues.Select(a => a.Level).ToList();
                                        multiselectdropdownResults = (from treevalue in multiselecttreevalues where treevalue.Attributeid == val.ID select treevalue.Nodeid).ToList();
                                        //var nodes = (from item in tx.PersistenceManager.PlanningRepository.Query<TreeNodeDao>() where multiselectdropdownResults.Contains(item.Id) select item.Level);
                                        var nodes = (from item in tx.PersistenceManager.MetadataRepository.GetObject<TreeNodeDao>(xmlpath) where multiselectdropdownResults.Contains(item.Id) select item.Level);
                                        var distinctNodes = nodes.Distinct();
                                        int lastRow = 0;
                                        foreach (var dropnode in distinctNodes)
                                        {
                                            ITreeDropDownLabel dropdownlabel = new TreeDropDownLabel();
                                            ITreeDropDownCaption treecaption = new TreeDropDownCaption();
                                            //var nodelevels = (from item in tx.PersistenceManager.PlanningRepository.Query<TreeLevelDao>() where item.Level == dropnode && item.AttributeID == val.ID select item).SingleOrDefault();
                                            var nodelevels = (from item in tx.PersistenceManager.MetadataRepository.GetObject<TreeLevelDao>(xmlpath) where item.Level == dropnode && item.AttributeID == val.ID select item).SingleOrDefault();
                                            treecaption.Level = nodelevels.Level;
                                            dropdownlabel.Level = nodelevels.Level;
                                            dropdownlabel.Label = nodelevels.LevelName.Trim();
                                            itreeCaption.Add(treecaption);
                                            droplabel.Add(dropdownlabel);
                                            if (lastRow == distinctNodes.Count() - 1)
                                            {
                                                foreach (var levelObj in multiselecttreeLevelList)
                                                {
                                                    ITreeDropDownLabel dropdownlabel2 = new TreeDropDownLabel();
                                                    ITreeDropDownCaption treecaption2 = new TreeDropDownCaption();
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
                                        //attributedate.Caption = (from item in tx.PersistenceManager.PlanningRepository.Query<TreeNodeDao>() where temptreevalues.Contains(item.Id) orderby item.Level select item.Caption).ToList();
                                        attributedate.Caption = (from item in tx.PersistenceManager.MetadataRepository.GetObject<TreeNodeDao>(xmlpath) where temptreevalues.Contains(item.Id) orderby item.Level select item.Caption).ToList();
                                        attributedate.TypeID = val.AttributeTypeID;
                                        attributedate.Value = multiselecttreevalues;
                                        attributedate.IsInheritFromParent = val.InheritFromParent;
                                        attributedate.IsChooseFromParent = val.ChooseFromParent;
                                    }
                                    else
                                    {
                                        foreach (var levelObj in multiselecttreeLevelList)
                                        {
                                            ITreeDropDownLabel dropdownlabel = new TreeDropDownLabel();
                                            ITreeDropDownCaption treecaption = new TreeDropDownCaption();
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

                                case AttributesList.TextMoney:
                                    attributedate = new AttributeData();
                                    attributedate.ID = val.ID;
                                    attributedate.TypeID = val.AttributeTypeID;
                                    attributedate.Lable = val.Caption.Trim();
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


                                    attributedate.IsSpecial = val.IsSpecial;
                                    attributedate.IsReadOnly = val.IsReadOnly;

                                    attributesWithValues.Add(attributedate);
                                    break;

                                default:

                                    break;
                            }
                        }
                    }
                    tx.Commit();
                }
                return attributesWithValues;
            }
            catch (Exception ex)
            {

            }
            return null;
        }


        public IList<IMilestoneMetadata> GetDynamicEntityRelation(ITransaction txInner, TaskManagerProxy proxy, int entityId, int entitytypeId)
        {
            IList<IMilestoneMetadata> listMilestone = new List<IMilestoneMetadata>();
            string entityName = "AttributeRecord" + entitytypeId + "_V" + MarcomManagerFactory.ActiveMetadataVersionNumber;
            List<int> attrIds = new List<int>();

            //var attrResult = tx.PersistenceManager.PlanningRepository.GetAll<DynamicAttributesDao>(entityName).Where(a => Convert.ToInt32(a.Attributes["" + Convert.ToInt32(SystemDefinedAttributes.MilestoneEntityID) + ""]) == entityId);
            var attrResult = txInner.PersistenceManager.PlanningRepository.GetAll<DynamicAttributesDao>(entityName).Where(a => a.Id == entityId);
            var entityTypeResult = txInner.PersistenceManager.PlanningRepository.Query<EntityTypeAttributeRelationDao>().Where(a => a.EntityTypeID == entitytypeId).ToList();
            var attributeResult = txInner.PersistenceManager.PlanningRepository.Query<AttributeDao>();
            foreach (var obj in attrResult)
            {
                MilestoneMetadata milestonedata = new MilestoneMetadata();
                IList<IAttributeData> listAttr = new List<IAttributeData>();
                milestonedata.EntityId = obj.Id;
                milestonedata.AttributeData = null;
                var getMilestoneName = txInner.PersistenceManager.PlanningRepository.Get<EntityTaskDao>(milestonedata.EntityId);
                AttributeData attrformilestone = new AttributeData();
                attrformilestone.ID = Convert.ToInt32(SystemDefinedAttributes.Name);
                attrformilestone.Caption = entityTypeResult.Where(a => a.AttributeID == (Convert.ToInt32(SystemDefinedAttributes.Name)) && a.EntityTypeID == entitytypeId).Select(a => a.Caption).First();
                attrformilestone.Value = getMilestoneName.Name;
                attrformilestone.TypeID = attributeResult.Where(a => a.Id == Convert.ToInt32(SystemDefinedAttributes.Name)).Select(a => a.AttributeTypeID).First();
                attrformilestone.IsSpecial = true;
                attrformilestone.Lable = entityTypeResult.Where(a => a.AttributeID == (Convert.ToInt32(SystemDefinedAttributes.Name)) && a.EntityTypeID == entitytypeId).Select(a => a.Caption).First();
                attrformilestone.Level = 0;
                listAttr.Add(attrformilestone);
                if (obj.Attributes != null)
                {
                    foreach (DictionaryEntry ob in obj.Attributes)
                    {
                        AttributeData attr = new AttributeData();
                        int attributeid = Convert.ToInt32((object)ob.Key);
                        attr.ID = attributeResult.Where(a => a.Id == attributeid).Select(a => a.Id).First();
                        attr.TypeID = attributeResult.Where(a => a.Id == attributeid).Select(a => a.AttributeTypeID).First();
                        attr.Caption = entityTypeResult.Where(a => a.AttributeID == attributeid && a.EntityTypeID == entitytypeId).Select(a => a.Caption).First();
                        if (Convert.ToInt32(ob.Key) == Convert.ToInt32(SystemDefinedAttributes.DueDate))
                            attr.Value = ((System.DateTime)(ob.Value)).Date.ToString();
                        else
                            attr.Value = ob.Value;
                        listAttr.Add(attr);

                    }
                }
                milestonedata.AttributeData = listAttr;
                listMilestone.Add(milestonedata);
            }

            return listMilestone;
        }



        public class TLinks
        {
            public string FileName { get; set; }
            public string Extension { get; set; }
            public string Size { get; set; }
            public Guid FileGuid { get; set; }
            public string FileDescription { get; set; }
            public int ID { get; set; }
            public int LinkID { get; set; }
        }

        public class RootObj
        {
            public string objectType { get; set; }
            public List<TLinks> TLinks { get; set; }
        }

        public Tuple<int, IEntityTask> InsertEntityTaskWithAttachments(TaskManagerProxy proxy, int parentEntityID, int taskTypeID, int entitytypeid, string TaskName, IList<IEntityTask> TaskList, IList<INewTaskAttachments> TaskAttachments, IList<IFile> TaskFiles, IList<ITaskMembers> taskMembers, IList<IAttributeData> entityattributedata, JArray arrAttchObj, IList<IEntityTaskCheckList> AdminTaskChkLst)
        {
            try
            {
                proxy.MarcomManager.AccessManager.TryEntityTypeAccess(parentEntityID, Modules.Planning);
                BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("-----------------------------------------------------------------------------------------------", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);
                BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("Started creating Task", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);
                int entityId = 0;
                WorkFlowNotifyHolder workFlowNotifyHolder = new WorkFlowNotifyHolder();

                workFlowNotifyHolder.Actorid = proxy.MarcomManager.User.Id;
                workFlowNotifyHolder.action = "Tasks";
                workFlowNotifyHolder.TypeName = taskTypeID.ToString();



                IEntityTask iadmtsk = new EntityTask();
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    if (taskTypeID == 32)
                    {
                        AssetsDao assetdaoforID = new AssetsDao();
                        assetdaoforID = tx.PersistenceManager.PlanningRepository.Query<AssetsDao>().Where(ab => ab.ID == TaskList[0].AssetId).Select(ab => ab).FirstOrDefault();
                        workFlowNotifyHolder.AttributeName = assetdaoforID.Name;
                        workFlowNotifyHolder.AssociatedEntityId = TaskList[0].AssetId;
                        if (assetdaoforID.ActiveFileID > 0)
                        {
                            DAMFileDao fileinfo = new DAMFileDao();
                            fileinfo = tx.PersistenceManager.PlanningRepository.Query<DAMFileDao>().Where(ab => ab.ID == assetdaoforID.ActiveFileID).Select(ab => ab).FirstOrDefault();
                            workFlowNotifyHolder.Version = fileinfo.VersionNo;
                        }
                        else
                        {
                            workFlowNotifyHolder.Version = 1;
                        }
                    }
                    IList<EntityTaskDao> iTask = new List<EntityTaskDao>();
                    entityId = GetBaseEntityID(entitytypeid, TaskName, tx, parentEntityID, true, false);
                    if (TaskList != null)
                    {
                        int sortOrderID = 0;
                        foreach (var a in TaskList)
                        {
                            IList<MultiProperty> paramList = new List<MultiProperty>();
                            paramList.Add(new MultiProperty { propertyName = "EntityID", propertyValue = parentEntityID });
                            paramList.Add(new MultiProperty { propertyName = "TaskListID", propertyValue = a.TaskListID });
                            string newSortOrder = "SELECT COUNT(*)+1 as SortOrder FROM TM_EntityTask  WHERE taskListID= :TaskListID AND EntityID = :EntityID";
                            IList sortOrderVal = tx.PersistenceManager.PlanningRepository.ExecuteQuerywithParam(newSortOrder, paramList);
                            sortOrderID = (int)((System.Collections.Hashtable)(sortOrderVal)[0])["SortOrder"];
                            EntityTaskDao taskdao = new EntityTaskDao();
                            taskdao.Name = HttpUtility.HtmlEncode(a.Name);
                            taskdao.TaskListID = a.TaskListID;
                            taskdao.Description = HttpUtility.HtmlEncode(a.Description);
                            taskdao.TaskType = a.TaskType;
                            taskdao.EntityTaskListID = a.EntityTaskListID;
                            taskdao.DueDate = a.DueDate;
                            taskdao.Note = a.Note;
                            taskdao.EntityID = parentEntityID;
                            taskdao.ID = entityId;
                            taskdao.Sortorder = sortOrderID;
                            taskdao.AssetId = a.AssetId;
                            if (taskMembers.Count() > 0)
                                taskdao.TaskStatus = (int)TaskStatus.In_progress;
                            else if (taskMembers.Count() == 0)
                                taskdao.TaskStatus = (int)TaskStatus.Unassigned;
                            iTask.Add(taskdao);


                        }

                        tx.PersistenceManager.PlanningRepository.Save<EntityTaskDao>(iTask);
                        iadmtsk.Id = entityId;
                        BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("Saved in Task", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);

                        workFlowNotifyHolder.iEntityTask = new List<EntityTaskDao>();
                        workFlowNotifyHolder.iEntityTask = iTask;
                    }

                    if (AdminTaskChkLst != null)
                    {

                        IList<EntityTaskCheckListDao> taskChklst = new List<EntityTaskCheckListDao>();
                        for (int i = 0; i <= AdminTaskChkLst.Count() - 1; i++)
                        {
                            if (AdminTaskChkLst[i].Name.Trim() != "")
                                taskChklst.Add(new EntityTaskCheckListDao { Name = AdminTaskChkLst[i].Name.Trim(), TaskId = entityId, SortOrder = (i + 1) });

                        }
                        if (taskChklst.Count > 0)
                            tx.PersistenceManager.PlanningRepository.Save<EntityTaskCheckListDao>(taskChklst);
                        iadmtsk.TaskCheckList = taskChklst;
                    }

                    IList<LinksDao> ilinks = new List<LinksDao>();
                    foreach (var data in arrAttchObj)
                    {
                        LinksDao lnks = new LinksDao();
                        string ext = (string)data["Extension"];
                        if (ext == "Link")
                        {
                            Guid NewId = Guid.NewGuid();
                            lnks = new LinksDao();
                            lnks.Name = (string)data["FileName"];
                            string LinkURL = (string)data["URL"];
                            //lnks.URL = (LinkURL.Substring(0, 7).ToString() == "http://") ? LinkURL : "http://" + LinkURL;
                            lnks.URL = LinkURL;
                            lnks.Description = (string)data["FileDescription"];
                            lnks.LinkType = (int)data["LinkType"];
                            lnks.CreatedOn = DateTime.Now.ToString();
                            lnks.EntityID = entityId;
                            lnks.ModuleID = 1;
                            lnks.OwnerID = proxy.MarcomManager.User.Id;
                            lnks.ActiveVersionNo = 1;
                            lnks.LinkGuid = NewId;
                            ilinks.Add(lnks);

                        }
                    }

                    if (ilinks.Count > 0)
                    {
                        tx.PersistenceManager.CommonRepository.Save<LinksDao>(ilinks);
                        BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("Saved in Links", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);
                    }

                    Dictionary<Guid, string> descriptionObj = new Dictionary<Guid, string>();
                    if (TaskFiles != null)
                    {
                        IList<FileDao> ifile = new List<FileDao>();
                        if (TaskFiles != null)
                        {
                            foreach (var a in TaskFiles)
                            {
                                Guid NewId = Guid.NewGuid();
                                string filePath = ReadAdminXML("FileManagment");
                                var DirInfo = System.IO.Directory.GetParent(filePath);
                                string newFilePath = DirInfo.FullName;
                                if (a.IsExist)
                                    System.IO.File.Copy(filePath + "\\" + a.strFileID.ToString() + a.Extension, newFilePath + "\\" + NewId + a.Extension);
                                else
                                    System.IO.File.Move(filePath + "\\" + a.strFileID.ToString() + a.Extension, newFilePath + "\\" + NewId + a.Extension);
                                FileDao fldao = new FileDao();
                                fldao.Checksum = a.Checksum;
                                fldao.CreatedOn = a.CreatedOn;
                                fldao.Entityid = entityId;
                                fldao.Extension = a.Extension;
                                fldao.MimeType = a.MimeType;
                                fldao.Moduleid = a.Moduleid;
                                fldao.Name = a.Name;
                                fldao.Ownerid = a.Ownerid;
                                fldao.Size = a.Size;
                                fldao.VersionNo = a.VersionNo;
                                fldao.Fileguid = NewId;
                                fldao.Description = a.Description;
                                ifile.Add(fldao);
                                //descriptionObj.Add(NewId, a.Description);
                            }
                            tx.PersistenceManager.PlanningRepository.Save<FileDao>(ifile);
                            BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("Saved in File", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);
                        }

                        if (ifile.Count != 0)
                        {

                            IList<AttachmentsDao> iattachment = new List<AttachmentsDao>();
                            foreach (var a in ifile)
                            {
                                var result = descriptionObj.Where(r => r.Key == a.Fileguid).ToList();
                                AttachmentsDao attachedao = new AttachmentsDao();
                                attachedao.ActiveFileid = a.Id;
                                attachedao.ActiveVersionNo = (a.VersionNo != null) ? a.VersionNo : 1;
                                attachedao.Createdon = DateTime.Now;
                                attachedao.Entityid = entityId;
                                attachedao.Name = a.Name;
                                attachedao.Typeid = taskTypeID;
                                attachedao.ActiveFileVersionID = a.Id;
                                attachedao.VersioningFileId = a.Id;
                                //attachedao.Description = result[0].Value;
                                iattachment.Add(attachedao);
                            }
                            tx.PersistenceManager.PlanningRepository.Save<AttachmentsDao>(iattachment);
                            BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("Saved in Task Members", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);

                            //workFlowNotifyHolder.taskattachmentdao = new List<object>();
                            // workFlowNotifyHolder.isattachment = true;
                            for (int i = 0; i < iattachment.Count(); i++)
                            {
                                if (i != 0)
                                {
                                    // workFlowNotifyHolder.obj2.Add(iattachment[i]);  // add the iaatchment[i] to this instance workFlowNotifyHolder.obj2 properly...because yout property is list<object> it seems like you can't add Ilist<INewattachment> 
                                }
                            }

                        }

                    }
                    if (taskMembers.Count != 0)
                    {
                        IList<TaskMembersDao> ientityRole = new List<TaskMembersDao>();
                        TaskMembersDao entroledao = new TaskMembersDao();
                        entroledao.RoleID = 1;
                        entroledao.TaskID = entityId;
                        entroledao.UserID = proxy.MarcomManager.User.Id;
                        entroledao.ApprovalRount = 1;
                        entroledao.ApprovalStatus = null;
                        entroledao.FlagColorCode = "f5f5f5";
                        ientityRole.Add(entroledao);
                        foreach (var a in taskMembers)
                        {
                            entroledao = new TaskMembersDao();
                            entroledao.RoleID = a.RoleID;
                            entroledao.FlagColorCode = "f5f5f5";
                            entroledao.TaskID = entityId;
                            entroledao.UserID = a.UserID;
                            entroledao.ApprovalRount = 1;
                            entroledao.ApprovalStatus = null;
                            ientityRole.Add(entroledao);
                        }
                        tx.PersistenceManager.TaskRepository.Save<TaskMembersDao>(ientityRole);
                        BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("Saved in Task Members", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);
                        // workFlowNotifyHolder.ismemberadded = true;
                        workFlowNotifyHolder.ientityTaskRole = new List<TaskMembersDao>();
                        for (int i = 0; i < ientityRole.Count(); i++)
                        {
                            // if (ientityRole[i].Userid != proxy.MarcomManager.User.Id)
                            {
                                workFlowNotifyHolder.ientityTaskRole.Add(ientityRole[i]);
                            }
                        }


                    }
                    if (entityattributedata != null)
                    {
                        var result = InsertEntityAttributes(tx, entityattributedata, entityId, entitytypeid);
                    }
                    tx.Commit();
                    iadmtsk.AttributeData = GetEntityAttributesDetails(proxy, entityId);
                    FeedNotificationServer fs = new FeedNotificationServer();
                    fs.AsynchronousNotify((NotificationFeedObjects)workFlowNotifyHolder);
                }
                iadmtsk = GetEntityTaskDetails(proxy, entityId);

                //mail


                //end mail

                //Adding to the Search Engine
                BrandSystems.Marcom.Core.Interface.Managers.IEventManager _eventManager = null;
                BrandSystems.Marcom.Core.Interface.Managers.IPluginManager _pluginManager = null;
                MarcomManager marcommanager = new MarcomManager(_eventManager, _pluginManager);
                BrandSystems.Marcom.Core.Managers.Proxy.PlanningManagerProxy pProxy = new BrandSystems.Marcom.Core.Managers.Proxy.PlanningManagerProxy(marcommanager);
                System.Threading.Tasks.Task addtaskforsearch = new System.Threading.Tasks.Task(() => PlanningManager.Instance.AddEntityAsync(pProxy, entityId, TaskName, TaskList[0].Description, "Task"));
                addtaskforsearch.Start();

                Tuple<int, IEntityTask> taskObj = Tuple.Create(entityId, iadmtsk);
                return taskObj;
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
        public int GetBaseEntityID(int typeId, string name, ITransaction tx, int parentId = 0, Boolean active = false, Boolean isLock = false, bool enableDisableWorkflow = false, bool DuplicateTask = false)
        {


            StringBuilder newUniqueKey = new StringBuilder();
            StringBuilder activestepQuery = new StringBuilder();
            StringBuilder parentStateQuery = new StringBuilder();
            newUniqueKey.Append("SELECT ISNULL((SELECT TOP 1 UniqueKey FROM PM_Entity as pe where  pe.ID = ? ) + '.'  + CAST(ISNULL(max([EntityID]), 0) + 1 as nvarchar(10)) , ISNULL(max([EntityID]), 0) + 1) as UniqueKey, ISNULL(max([EntityID]), 0) + 1 as EntityID FROM PM_Entity as pe  where  pe.ParentID = ? ");
            IList uniqueKeyVal = tx.PersistenceManager.TaskRepository.ExecuteQuerywithMinParam(newUniqueKey.ToString(), parentId, parentId);
            string uniqueKey = (string)((System.Collections.Hashtable)(uniqueKeyVal)[0])["UniqueKey"];
            int entityKeyID = (int)((System.Collections.Hashtable)(uniqueKeyVal)[0])["EntityID"];
            activestepQuery.Append("SELECT TOP 1 ISNULL( mwfs.id , 0) AS StepID FROM MM_WorkFlow_Steps mwfs WHERE mwfs.WorkFlowID = (SELECT met.WorkFlowID FROM MM_EntityType met WHERE met.ID= ? ) ORDER BY mwfs.ID asc");
            IList activeStepUniqueRes = tx.PersistenceManager.PlanningRepository.ExecuteQuerywithMinParam(activestepQuery.ToString(), Convert.ToInt32(typeId));
            int activeStepID = 0;
            if (activeStepUniqueRes.Count > 0)
            {
                activeStepID = (int)((System.Collections.Hashtable)(activeStepUniqueRes)[0])["StepID"];
            }
            parentStateQuery.Append("SELECT pe.ActiveEntityStateID as EntityState FROM PM_Entity pe WHERE pe.id= ? ");
            IList parentStateRes = tx.PersistenceManager.PlanningRepository.ExecuteQuerywithMinParam(parentStateQuery.ToString(), parentId);
            int parentStateID = 0;
            if (parentStateRes.Count > 0 && parentId != 0)
            {
                parentStateID = (int)((System.Collections.Hashtable)(parentStateRes)[0])["EntityState"];
            }
            BaseEntityDao basentdao = new BaseEntityDao();
            basentdao.Typeid = typeId;
            if (DuplicateTask)
                basentdao.Name = "Copy of " + HttpUtility.HtmlEncode(name);
            else
                basentdao.Name = HttpUtility.HtmlEncode(name);
            basentdao.Parentid = parentId;
            basentdao.Active = active;
            basentdao.UniqueKey = uniqueKey;
            basentdao.EntityID = entityKeyID;
            basentdao.EntityStateID = parentStateID;
            basentdao.IsLock = isLock;
            basentdao.Level = (uniqueKey.Split('.').Length - 1);
            basentdao.Version = MarcomManagerFactory.ActiveMetadataVersionNumber;
            basentdao.ActiveEntityStateID = activeStepID;
            basentdao.EnableDisableWorkflow = enableDisableWorkflow;
            tx.PersistenceManager.PlanningRepository.Save<BaseEntityDao>(basentdao);
            return basentdao.Id;
        }

        public IList<IEntityTaskList> GetOverViewEntityTaskList(TaskManagerProxy proxy, int entityID)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    IList<IEntityTaskList> tasklist = new List<IEntityTaskList>();
                    var entityTaskList = tx.PersistenceManager.PlanningRepository.Query<EntityTaskListDao>().Where(a => a.EntityID == entityID).Select(a => a);
                    int ActIveTaskListId = 0;
                    IList<EntityTaskDao> entitytaskdao = new List<EntityTaskDao>();
                    foreach (var val in entityTaskList)
                    {
                        IEntityTaskList tskLst = new EntityTaskList();
                        tskLst.Description = val.Description;
                        tskLst.EntityID = val.EntityID;
                        tskLst.Id = val.ID;
                        tskLst.Name = val.Name;
                        tskLst.OnTimeComment = val.OnTimeComment;
                        tskLst.OnTimeStatus = val.OnTimeStatus;
                        tskLst.TaskListID = val.TaskListID;
                        if (ActIveTaskListId == 0)
                        {
                            ActIveTaskListId = val.ID;
                            tskLst.ActiveEntityStateID = ActIveTaskListId;
                        }
                        //tskLst.ActiveEntityStateID = val.TaskListID;
                        var Currentitem = tx.PersistenceManager.PlanningRepository.Query<EntityDao>().Where(a => a.Id == entityID);
                        if (Currentitem != null)
                        {
                            if (Currentitem.Select(a => a.ActiveEntityStateID).SingleOrDefault() != 0)
                                tskLst.ActiveEntityStateID = Currentitem.Select(a => a.ActiveEntityStateID).SingleOrDefault();
                        }
                        entitytaskdao = (from task in tx.PersistenceManager.PlanningRepository.Query<EntityTaskDao>()
                                         where task.EntityID == entityID
                                         select task).ToList<EntityTaskDao>();

                        //tskLst.TaskInProgress = tx.PersistenceManager.PlanningRepository.Query<EntityTaskDao>().Where(a => a.EntityID == entityID && a.TaskStatus == (int)TaskStatus.In_progress).Count();
                        //tskLst.UnAssignedTasks = tx.PersistenceManager.PlanningRepository.Query<EntityTaskDao>().Where(a => a.EntityID == entityID && a.TaskStatus == (int)TaskStatus.Unassigned).Count();
                        //tskLst.OverDueTasks = tx.PersistenceManager.PlanningRepository.Query<EntityTaskDao>().Where(a => a.EntityID == entityID && a.TaskStatus == (int)TaskStatus.In_progress && a.DueDate < DateTime.Now).Count();
                        //tskLst.UnableToComplete = tx.PersistenceManager.PlanningRepository.Query<EntityTaskDao>().Where(a => a.EntityID == entityID && a.TaskStatus == (int)TaskStatus.Unable_to_complete).Count();

                        tskLst.TaskInProgress = (from statustask in entitytaskdao where statustask.TaskStatus == (int)TaskStatus.In_progress select statustask.TaskStatus).Count();
                        tskLst.UnAssignedTasks = (from statustask in entitytaskdao where statustask.TaskStatus == (int)TaskStatus.Unassigned select statustask.TaskStatus).Count();
                        tskLst.OverDueTasks = (from statustask in entitytaskdao where statustask.TaskStatus == (int)TaskStatus.In_progress && statustask.DueDate < DateTime.Now.Date select statustask.TaskStatus).Count();
                        tskLst.UnableToComplete = (from statustask in entitytaskdao where statustask.TaskStatus == (int)TaskStatus.Unable_to_complete select statustask.TaskStatus).Count();
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

        public bool UpdateOverviewEntityTaskList(TaskManagerProxy proxy, int OnTimeStatus, string OnTimeComment, int ID)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {

                    EntityTaskListDao tskdao = new EntityTaskListDao();

                    tskdao = tx.PersistenceManager.TaskRepository.Get<EntityTaskListDao>(ID);
                    tskdao.OnTimeComment = (OnTimeComment != "-" ? OnTimeComment : tskdao.OnTimeComment);
                    if (OnTimeComment == "-")
                    {
                        tskdao.OnTimeStatus = OnTimeStatus;
                    }
                    tx.PersistenceManager.TaskRepository.Save<EntityTaskListDao>(tskdao);
                    tx.Commit();
                    return true;
                }
            }
            catch
            {
                return false;
            }

        }

        /// <summary>
        /// Get member.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns>IList of IEntityRoleUser</returns>
        public IList<ITaskMembers> GetTaskMember(TaskManagerProxy proxy, int taskID)
        {
            try
            {
                IList<ITaskMembers> ientityMembers = new List<ITaskMembers>();
                IList<TaskMembersDao> entityroleuserdao = new List<TaskMembersDao>();
                BrandSystems.Marcom.Core.User.Interface.IUser users = new BrandSystems.Marcom.Core.User.User();
                BrandSystems.Marcom.Dal.User.Model.UserDao userval = new BrandSystems.Marcom.Dal.User.Model.UserDao();
                RoleDao roleval = new RoleDao();
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    entityroleuserdao = (from item in tx.PersistenceManager.PlanningRepository.Query<TaskMembersDao>() where item.TaskID == taskID select item).ToList<TaskMembersDao>();
                    //entityroleuserdao = tx.PersistenceManager.PlanningRepository.GetEquals<TaskMemberDao>(TaskMemberDao.PropertyNames.TaskID, taskID);
                    foreach (var entmem in entityroleuserdao)
                    {
                        ITaskMembers entityMembers = new TaskMembers();
                        userval = tx.PersistenceManager.UserRepository.Get<BrandSystems.Marcom.Dal.User.Model.UserDao>(entmem.UserID);
                        roleval = tx.PersistenceManager.UserRepository.Get<RoleDao>(entmem.RoleID);
                        entityMembers.Id = entmem.ID;
                        entityMembers.TaskID = entmem.TaskID;
                        entityMembers.RoleID = entmem.RoleID;
                        entityMembers.UserID = entmem.UserID;
                        entityMembers.FlagColorCode = entmem.FlagColorCode;
                        entityMembers.ApprovalRount = entmem.ApprovalRount;
                        entityMembers.ApprovalStatus = entmem.ApprovalStatus;
                        if (userval != null)
                        {
                            entityMembers.UserName = userval.FirstName + " " + userval.LastName;
                            entityMembers.UserEmail = userval.Email;
                            entityMembers.DepartmentName = (userval.Designation == null ? "-" : userval.Designation);
                            entityMembers.Title = (userval.Title == null ? "-" : userval.Title);
                        }
                        else
                        {
                            entityMembers.UserName = " ";
                            entityMembers.UserEmail = " ";
                            entityMembers.DepartmentName = " ";
                            entityMembers.Title = " ";
                        }
                        entityMembers.Role = roleval.Caption;
                        ientityMembers.Add(entityMembers);
                    }
                    tx.Commit();
                }
                return ientityMembers;
            }
            catch
            {

            }
            return null;
        }

        public Tuple<bool, IList<ITaskMembers>> InsertTaskMembers(TaskManagerProxy proxy, int EntityID, int taskID, IList<ITaskMembers> TaskMembers, IList<ITaskMembers> TaskGlobalMembers)
        {
            try
            {
                BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("-----------------------------------------------------------------------------------------------", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);
                BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("Started creating Task", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);


                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    var taskType = (from item in tx.PersistenceManager.PlanningRepository.Query<BaseEntityDao>() where item.Id == taskID select item).FirstOrDefault();
                    IList<EntityTaskDao> iTask = new List<EntityTaskDao>();
                    IList<EntityRoleUserDao> Ientitroledao = new List<EntityRoleUserDao>();
                    EntityTaskDao taskdao = new EntityTaskDao();
                    IList<MultiProperty> prplst = new List<MultiProperty>();
                    prplst.Add(new MultiProperty { propertyName = TaskDao.PropertyNames.ID, propertyValue = taskID });
                    taskdao = (tx.PersistenceManager.AccessRepository.GetEquals<EntityTaskDao>(prplst)).FirstOrDefault();

                    if (taskdao.TaskType == (int)TaskTypes.Work_Task && taskdao.TaskStatus == (int)TaskStatus.Unassigned)
                    {
                        taskdao.TaskStatus = (int)TaskStatus.In_progress;
                        tx.PersistenceManager.TaskRepository.Save<EntityTaskDao>(taskdao);
                    }

                    IList<TaskMembersDao> ientityRole = new List<TaskMembersDao>();

                    //Task Reinitialize concept for Approval task
                    if (taskdao.TaskType == (int)TaskTypes.Approval_Task || taskdao.TaskType == (int)TaskTypes.Reviewal_Task)
                    {

                        TaskMembersDao memdao = new TaskMembersDao();
                        memdao = tx.PersistenceManager.PlanningRepository.Query<TaskMembersDao>().Where(a => a.TaskID == taskID && a.RoleID != 1 && a.UserID == proxy.MarcomManager.User.Id).FirstOrDefault();
                        TaskMembersDao taskMembers = new TaskMembersDao();
                        var totalTaskmembers = tx.PersistenceManager.PlanningRepository.Query<TaskMembersDao>().Where(a => a.TaskID == taskID && a.RoleID != 1).ToList();

                        if (taskdao.TaskStatus == (int)TaskStatus.Rejected || taskdao.TaskStatus == (int)TaskStatus.Unable_to_complete || taskdao.TaskStatus == (int)TaskStatus.Unassigned)
                        {

                            taskdao.TaskStatus = (int)TaskStatus.In_progress;
                            tx.PersistenceManager.TaskRepository.Save<EntityTaskDao>(taskdao);
                            if (TaskMembers != null)
                            {
                                TaskMembersDao entroledao = new TaskMembersDao();
                                foreach (var a in TaskMembers)
                                {
                                    entroledao = new TaskMembersDao();
                                    entroledao.RoleID = a.RoleID;
                                    entroledao.TaskID = taskID;
                                    entroledao.UserID = a.UserID;
                                    entroledao.ApprovalRount = 1;
                                    entroledao.FlagColorCode = "f5f5f5";
                                    entroledao.ApprovalStatus = null;
                                    ientityRole.Add(entroledao);
                                }

                            }
                            if (totalTaskmembers.Count() > 0)
                            {
                                foreach (var item in totalTaskmembers)
                                {
                                    item.ApprovalStatus = null;
                                    item.ApprovalRount = 1;
                                    ientityRole.Add(item);
                                }
                            }
                            if (ientityRole.Count() > 0)
                                tx.PersistenceManager.PlanningRepository.Save<TaskMembersDao>(ientityRole);
                            BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("Saved in Task Members", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);

                        }
                        else
                        {
                            if (TaskMembers != null)
                            {
                                TaskMembersDao entroledao = new TaskMembersDao();
                                foreach (var a in TaskMembers)
                                {
                                    entroledao = new TaskMembersDao();
                                    entroledao.RoleID = a.RoleID;
                                    entroledao.TaskID = taskID;
                                    entroledao.UserID = a.UserID;
                                    entroledao.ApprovalRount = 1;
                                    entroledao.FlagColorCode = "f5f5f5";
                                    entroledao.ApprovalStatus = null;
                                    ientityRole.Add(entroledao);
                                }
                                tx.PersistenceManager.PlanningRepository.Save<TaskMembersDao>(ientityRole);

                                BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("Saved in Task Members", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);
                            }
                        }
                    }
                    else
                    {
                        if (TaskMembers != null)
                        {
                            TaskMembersDao entroledao = new TaskMembersDao();
                            foreach (var a in TaskMembers)
                            {
                                entroledao = new TaskMembersDao();
                                entroledao.RoleID = a.RoleID;
                                entroledao.TaskID = taskID;
                                entroledao.UserID = a.UserID;
                                entroledao.ApprovalRount = 1;
                                entroledao.FlagColorCode = "f5f5f5";
                                entroledao.ApprovalStatus = null;
                                ientityRole.Add(entroledao);
                            }
                            tx.PersistenceManager.PlanningRepository.Save<TaskMembersDao>(ientityRole);

                            BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("Saved in Task Members", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);
                        }
                    }

                    if (TaskGlobalMembers.Count > 0)
                    {
                        foreach (var a in TaskGlobalMembers)
                        {
                            var distinctCount = 0;
                            if (Ientitroledao.Count() > 0)
                                distinctCount = Ientitroledao.Where(i => i.Roleid == a.RoleID && i.Userid == a.UserID).Count();
                            if (distinctCount == 0)
                            {
                                EntityRoleUserDao entityroledao = new EntityRoleUserDao();
                                entityroledao.Entityid = EntityID;
                                entityroledao.Roleid = a.RoleID;
                                entityroledao.Userid = a.UserID;
                                entityroledao.IsInherited = false;
                                entityroledao.InheritedFromEntityid = 0;
                                Ientitroledao.Add(entityroledao);
                            }
                            try
                            {
                                UserNotificationDao userNotify = new UserNotificationDao();
                                userNotify.Entityid = EntityID;
                                userNotify.Actorid = proxy.MarcomManager.User.Id;
                                userNotify.CreatedOn = DateTimeOffset.Now;
                                userNotify.Typeid = 14;
                                userNotify.IsViewed = false;
                                userNotify.IsSentInMail = false;
                                userNotify.TypeName = "";
                                userNotify.AttributeName = "";
                                userNotify.FromValue = "";
                                userNotify.ToValue = a.RoleID.ToString();
                                userNotify.Userid = Convert.ToInt32(a.UserID);
                                if (proxy.MarcomManager.User.Id != Convert.ToInt32(a.UserID))
                                {
                                    tx.PersistenceManager.CommonRepository.Save<UserNotificationDao>(userNotify);
                                }
                                IFeed feedList = new Feed();
                                FeedDao feedObj = new FeedDao();
                                feedObj.Actor = proxy.MarcomManager.User.Id;
                                feedObj.Templateid = 3;
                                feedObj.Entityid = EntityID;
                                feedObj.TypeName = "MemberAdded";
                                feedObj.HappenedOn = DateTimeOffset.UtcNow;
                                feedObj.CommentedUpdatedOn = DateTimeOffset.MinValue;

                                feedObj.AttributeName = "";
                                feedObj.FromValue = "";
                                feedObj.ToValue = a.RoleID.ToString();
                                feedObj.UserID = Convert.ToInt32(a.UserID);

                                tx.PersistenceManager.CommonRepository.Save<FeedDao>(feedObj);

                            }
                            catch
                            {

                            }
                        }
                    }
                    if (Ientitroledao.Count > 0)
                    {
                        tx.PersistenceManager.PlanningRepository.Save<EntityRoleUserDao>(Ientitroledao);
                        BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("Saved Entity Role Users", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);
                    }

                    tx.Commit();
                    IList<ITaskMembers> ientityMembers = new List<ITaskMembers>();
                    ientityMembers = GetTaskMember(proxy, taskID);
                    Tuple<bool, IList<ITaskMembers>> retObj = Tuple.Create(true, ientityMembers);

                    FeedNotificationServer fs = new FeedNotificationServer();
                    WorkFlowNotifyHolder obj = new WorkFlowNotifyHolder();
                    obj.action = "task member added";
                    obj.ientityRoles = ientityRole;
                    obj.EntityId = taskID;
                    obj.Actorid = proxy.MarcomManager.User.Id;
                    fs.AsynchronousNotify(obj);

                    //Adding to the Search Engine
                    BrandSystems.Marcom.Core.Interface.Managers.IEventManager _eventManager = null;
                    BrandSystems.Marcom.Core.Interface.Managers.IPluginManager _pluginManager = null;
                    MarcomManager marcommanager = new MarcomManager(_eventManager, _pluginManager);
                    BrandSystems.Marcom.Core.Managers.Proxy.PlanningManagerProxy pProxy = new BrandSystems.Marcom.Core.Managers.Proxy.PlanningManagerProxy(marcommanager);
                    System.Threading.Tasks.Task addtaskforsearch = new System.Threading.Tasks.Task(() => PlanningManager.Instance.UpdateEntityforSearchAsync(pProxy, EntityID, taskType.Name));
                    addtaskforsearch.Start();

                    if (taskdao.TaskType == (int)TaskTypes.Proof_approval_task) // calling proof hq tasks  memeber creation
                    {
                        System.Threading.Tasks.Task addmembersintoProof = new System.Threading.Tasks.Task(() => ProofHQMemberInsertion(proxy, taskID, ientityRole));
                        addmembersintoProof.Start();
                    }

                    return retObj;
                }

            }
            catch (Exception ex)
            {
                return null;
            }


        }


        //ProofHQ member Insertion

        #region ProofHQMemberInsertion

        void ProofHQMemberInsertion(TaskManagerProxy proxy, int taskId, IList<TaskMembersDao> ientityRole)
        {
            //Prabhu will put logic here for inserting members into proofHQ tasks
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    string UIDinClause = "( " + String.Join(",", ientityRole.Select(x => x.UserID.ToString()).Distinct().ToArray()) + " )";
                    var sbUsers = new StringBuilder();
                    sbUsers.AppendLine(" SELECT * FROM UM_User uu WHERE uu.ID IN  " + UIDinClause + "");
                    IList taskAssigneesList = tx.PersistenceManager.PlanningRepository.ExecuteQuery(sbUsers.ToString()); // assigness mailid and user name

                    //now prabhu will call the proof hq member creation method in this place
                    string xmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
                    XDocument adminXmlDoc = XDocument.Load(xmlpath);
                    string xelementName = "ProofHQSettings_Table";
                    var xelementFilepath = XElement.Load(xmlpath);
                    var pfEmail = xelementFilepath.Element(xelementName).Element("ProofHQSettings").Element("Email");
                    var pfPwd = xelementFilepath.Element(xelementName).Element("ProofHQSettings").Element("Password");

                    var client = new soapService();
                    SOAPLoginObject response = client.doLogin(pfEmail.Value, pfPwd.Value);

                    IList proofs = tx.PersistenceManager.DamRepository.ExecuteQuery("SELECT ttpr.ProofID FROM TM_Task_Proof_Relation ttpr WHERE ttpr.TaskID = " + taskId); // assets attached for this task

                    int proofID = 0;

                    if (proofs != null && proofs.Count > 0)
                    {
                        foreach (var proof in proofs)
                        {
                            proofID = Convert.ToInt32((int)((System.Collections.Hashtable)(proof))["ProofID"]);

                        }
                    }

                    IList taskDetails = tx.PersistenceManager.DamRepository.ExecuteQuery("SELECT ID, Name, DueDate FROM [dbo].[TM_EntityTask] WHERE ID = " + taskId); // assets attached for this task

                    string proofName = null;
                    string deadline = null;
                    if (taskDetails != null && taskDetails.Count > 0)
                    {
                        proofName = Convert.ToString((string)((System.Collections.Hashtable)(taskDetails[0]))["Name"]);
                        if (((System.Collections.Hashtable)(taskDetails[0]))["DueDate"] != null)
                        {
                            deadline = DateTime.Parse(Convert.ToString((DateTime)((System.Collections.Hashtable)(taskDetails[0]))["DueDate"])).ToString("yyyy-MM-dd hh:mm");
                        }

                    }


                    var Recipients = new List<SOAPFileRecipientObject>();

                    foreach (var item in taskAssigneesList)
                    {
                        // assigneermail = item.

                        var FirstName = Convert.ToString((string)((System.Collections.Hashtable)(item))["FirstName"]);
                        var LastName = Convert.ToString((string)((System.Collections.Hashtable)(item))["LastName"]);
                        var Email = Convert.ToString((string)((System.Collections.Hashtable)(item))["Email"]);


                        Recipients.Add(new SOAPFileRecipientObject() { deadline = deadline, email = Email, name = FirstName + " " + LastName, notifications = "all new comments and replies", position = "", primary_decision_maker = "false", role = "reviewer & approver" });



                    }

                    client.addProofReviewers(response.session, proofID, Recipients.ToArray(), true);

                    tx.Commit();
                }
            }
            catch (Exception ex)
            {

            }
        }

        #endregion

        public Tuple<bool, IList<ITaskMembers>> InsertUnAssignedTaskMembers(TaskManagerProxy proxy, int EntityID, int taskID, IList<ITaskMembers> TaskMembers, ITaskMembers TaskOwner, IList<ITaskMembers> GlobalTaskMembers)
        {
            try
            {
                proxy.MarcomManager.AccessManager.TryEntityTypeAccess(EntityID, Modules.Planning);

                BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("-----------------------------------------------------------------------------------------------", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);
                BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("Started creating Task", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);


                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {

                    string tskOwnerQry = "SELECT COUNT(1) AS 'ExistCount' FROM TM_Task_Members ttm WHERE ttm.TaskID=" + taskID + " AND ttm.RoleID=1";
                    IList existOwnerLst = tx.PersistenceManager.PlanningRepository.ExecuteQuery(tskOwnerQry);
                    int ownerCount = (int)((System.Collections.Hashtable)(existOwnerLst)[0])["ExistCount"];

                    if (TaskOwner != null && ownerCount == 0)
                    {
                        TaskMembersDao entroleOwnerdao = new TaskMembersDao();

                        entroleOwnerdao = new TaskMembersDao();
                        entroleOwnerdao.RoleID = TaskOwner.RoleID;
                        entroleOwnerdao.TaskID = taskID;
                        entroleOwnerdao.UserID = TaskOwner.UserID;
                        entroleOwnerdao.ApprovalRount = 1;
                        entroleOwnerdao.FlagColorCode = "f5f5f5";
                        entroleOwnerdao.ApprovalStatus = null;
                        tx.PersistenceManager.PlanningRepository.Save<TaskMembersDao>(entroleOwnerdao);
                    }

                    IList<TaskMembersDao> ientityRole = new List<TaskMembersDao>();
                    IList<EntityRoleUserDao> Ientitroledao = new List<EntityRoleUserDao>();
                    if (TaskMembers != null)
                    {
                        TaskMembersDao entroledao = new TaskMembersDao();
                        foreach (var a in TaskMembers)
                        {
                            entroledao = new TaskMembersDao();
                            entroledao.RoleID = a.RoleID;
                            entroledao.TaskID = taskID;
                            entroledao.UserID = a.UserID;
                            entroledao.ApprovalRount = 1;
                            entroledao.FlagColorCode = "f5f5f5";
                            entroledao.ApprovalStatus = null;
                            ientityRole.Add(entroledao);
                        }
                        tx.PersistenceManager.PlanningRepository.Save<TaskMembersDao>(ientityRole);

                        BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("Saved in Task Members", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);
                    }

                    IList<TaskMembersDao> itaskmemberRole = new List<TaskMembersDao>();
                    itaskmemberRole = tx.PersistenceManager.TaskRepository.GetEquals<TaskMembersDao>(TaskMembersDao.PropertyNames.TaskID, taskID);

                    if (itaskmemberRole != null)
                    {
                        EntityTaskDao taskdao = new EntityTaskDao();
                        taskdao = (tx.PersistenceManager.AccessRepository.GetEquals<EntityTaskDao>(EntityTaskDao.PropertyNames.ID, taskID)).FirstOrDefault();

                        taskdao.TaskStatus = (int)TaskStatus.In_progress;
                        taskdao.EntityTaskListID = 0;
                        tx.PersistenceManager.PlanningRepository.Save<EntityTaskDao>(taskdao);
                    }

                    if (GlobalTaskMembers.Count > 0)
                    {
                        foreach (var a in GlobalTaskMembers)
                        {
                            var distinctCount = 0;
                            if (Ientitroledao.Count() > 0)
                                distinctCount = Ientitroledao.Where(i => i.Roleid == a.RoleID && i.Userid == a.UserID).Count();
                            if (distinctCount == 0)
                            {
                                EntityRoleUserDao entityroledao = new EntityRoleUserDao();
                                entityroledao.Entityid = EntityID;
                                entityroledao.Roleid = a.RoleID;
                                entityroledao.Userid = a.UserID;
                                entityroledao.IsInherited = false;
                                entityroledao.InheritedFromEntityid = 0;
                                Ientitroledao.Add(entityroledao);
                            }

                        }
                    }
                    if (Ientitroledao.Count > 0)
                    {
                        tx.PersistenceManager.PlanningRepository.Save<EntityRoleUserDao>(Ientitroledao);
                        BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("Saved Entity Role Users", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);
                    }

                    tx.Commit();
                    IList<ITaskMembers> ientityMembers = new List<ITaskMembers>();
                    ientityMembers = GetTaskMember(proxy, taskID);
                    Tuple<bool, IList<ITaskMembers>> retObj = Tuple.Create(true, ientityMembers);

                    FeedNotificationServer fs = new FeedNotificationServer();
                    WorkFlowNotifyHolder obj = new WorkFlowNotifyHolder();
                    obj.action = "task member added";
                    obj.ientityRoles = itaskmemberRole;
                    obj.EntityId = taskID;
                    obj.Actorid = proxy.MarcomManager.User.Id;
                    fs.AsynchronousNotify(obj);
                    return retObj;
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


        /// Updating Task status 
        /// </summary>
        ///  <param name="proxy"></param>
        /// <param name="entityId">The TaskID</param>
        /// <param name="status">The Status</param>
        /// <returns>True or False</returns>
        public Tuple<bool, int, string> UpdateTaskStatus(TaskManagerProxy proxy, int taskID, int status, int entityID = 0)
        {
            try
            {
                if (entityID != 0)
                    proxy.MarcomManager.AccessManager.TryEntityTypeAccess(entityID, Modules.Planning);
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    try
                    {
                        Tuple<bool, int, string> retObj = null;
                        EntityTaskDao EntityTaskDao = new EntityTaskDao();
                        IList<EntityTaskDao> Itask = new List<EntityTaskDao>();
                        TaskMembersDao TaskMembersDao = new TaskMembersDao();
                        IList<TaskMembersDao> Itaskmember = new List<TaskMembersDao>();
                        IList<MultiProperty> prplst = new List<MultiProperty>();
                        FeedNotificationServer fs = new FeedNotificationServer();
                        WorkFlowNotifyHolder wfhobj = new WorkFlowNotifyHolder();

                        var taskType = (from item in tx.PersistenceManager.PlanningRepository.Query<EntityTaskDao>() where item.ID == taskID select item).FirstOrDefault();
                        prplst.Add(new MultiProperty { propertyName = EntityTaskDao.PropertyNames.ID, propertyValue = taskID });
                        EntityTaskDao = (tx.PersistenceManager.AccessRepository.GetEquals<EntityTaskDao>(prplst)).FirstOrDefault();
                        var allTaskMembers = tx.PersistenceManager.PlanningRepository.Query<TaskMembersDao>().Where(a => a.TaskID == taskID).ToList();
                        //var currentTaskrountMembers =   tx.PersistenceManager.PlanningRepository.Query<TaskMembersDao>().Where(a => a.TaskID == taskID && a.RoleID != 1).ToList();
                        var currentTaskrountMembers = (from item in allTaskMembers where item.RoleID != 1 select item).ToList<TaskMembersDao>();
                        TaskMembersDao = (from data in tx.PersistenceManager.PlanningRepository.Query<TaskMembersDao>() where data.RoleID != 1 && data.UserID == proxy.MarcomManager.User.Id && data.TaskID == taskID select data).FirstOrDefault();
                        wfhobj.FromValue = (TaskMembersDao.ApprovalStatus != null ? ((TaskStatus)TaskMembersDao.ApprovalStatus).ToString() : "");
                        if (status == (int)TaskStatus.Withdrawn)
                        {
                            var unUpdatedUsers = currentTaskrountMembers.Where(a => a.ApprovalStatus == null).Select(a => a).ToList();
                            string statusname = "";
                            if (unUpdatedUsers.Count() == currentTaskrountMembers.Count())
                            {
                                EntityTaskDao.TaskStatus = status;
                                tx.Commit();
                                statusname = Enum.GetName(typeof(TaskStatus), status).Replace("_", " ");
                                retObj = Tuple.Create(true, (int)TaskStatus.Withdrawn, statusname);
                            }
                            else
                            {
                                retObj = Tuple.Create(false, 0, "");
                            }
                            return retObj;
                        }
                        else
                        {
                            if (taskType.TaskType == 2)
                            {
                                if (TaskMembersDao != null)
                                {
                                    if (status != (int)TaskStatus.RevokeTask)
                                        TaskMembersDao.ApprovalStatus = (int)(TaskStatus)status;
                                    else
                                        TaskMembersDao.ApprovalStatus = null;
                                    Itaskmember.Add(TaskMembersDao);
                                    tx.PersistenceManager.PlanningRepository.Save<TaskMembersDao>(Itaskmember);
                                    tx.Commit();
                                }
                                else
                                    tx.Commit();
                                using (ITransaction txinner = proxy.MarcomManager.GetTransaction())
                                {
                                    currentTaskrountMembers = tx.PersistenceManager.PlanningRepository.Query<TaskMembersDao>().Where(a => a.TaskID == taskID && a.RoleID != 1).ToList();

                                    var total_Approval_Status_For_This_Round = currentTaskrountMembers.Where(a => a.ApprovalStatus == (int)TaskStatus.Completed).Count();

                                    if (status != (int)TaskStatus.RevokeTask)
                                    {

                                        if (currentTaskrountMembers.Count() == 1 || status == (int)TaskStatus.Completed)
                                        {

                                            EntityTaskDao.TaskStatus = status;
                                        }

                                        else
                                        {

                                            if (currentTaskrountMembers.Count() == (total_Approval_Status_For_This_Round))
                                                EntityTaskDao.TaskStatus = status;

                                        }
                                    }
                                    else
                                    {
                                        if (currentTaskrountMembers.Count() != (total_Approval_Status_For_This_Round))
                                            EntityTaskDao.TaskStatus = (int)TaskStatus.In_progress;
                                    }
                                    Itask.Add(EntityTaskDao);
                                    txinner.PersistenceManager.PlanningRepository.Save<EntityTaskDao>(Itask);
                                    txinner.Commit();
                                }
                            }
                            else if (taskType.TaskType == 3 || taskType.TaskType == 32)
                            {
                                if (TaskMembersDao != null)
                                {
                                    if (status != (int)TaskStatus.RevokeTask)
                                        TaskMembersDao.ApprovalStatus = (int)(TaskStatus)status;
                                    else
                                        TaskMembersDao.ApprovalStatus = null;
                                    Itaskmember.Add(TaskMembersDao);
                                    tx.PersistenceManager.PlanningRepository.Save<TaskMembersDao>(TaskMembersDao);
                                    tx.Commit();
                                }
                                else
                                    tx.Commit();

                                using (ITransaction txinner = proxy.MarcomManager.GetTransaction())
                                {
                                    currentTaskrountMembers = tx.PersistenceManager.PlanningRepository.Query<TaskMembersDao>().Where(a => a.TaskID == taskID && a.RoleID != 1).ToList();

                                    var total_Approval_Status_For_This_Round = currentTaskrountMembers.Where(a => a.ApprovalStatus == (int)TaskStatus.Approved).Count();

                                    if (status != (int)TaskStatus.RevokeTask)
                                    {

                                        if (EntityTaskDao.TaskStatus != (int)TaskStatus.Unable_to_complete && EntityTaskDao.TaskStatus != (int)TaskStatus.Rejected)
                                        {

                                            if (status == (int)TaskStatus.Unable_to_complete || status == (int)TaskStatus.Rejected)
                                            {
                                                EntityTaskDao.TaskStatus = status;
                                            }
                                            else
                                            {
                                                if (currentTaskrountMembers.Count() == (total_Approval_Status_For_This_Round))
                                                    EntityTaskDao.TaskStatus = status;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //var rejectedMembers = currentTaskrountMembers.Where(a => a.ApprovalStatus == (int)TaskStatus.Rejected || a.ApprovalStatus==(int)TaskStatus.Unable_to_complete);
                                        //int taskstatus = (int)TaskStatus.In_progress;
                                        //if (rejectedMembers.Count() > 0)
                                        //    taskstatus = rejectedMembers.OrderByDescending(a => a.ApprovalStatus).Select(a=>a.ApprovalStatus).Take(1);
                                        if (currentTaskrountMembers.Count() != (total_Approval_Status_For_This_Round))
                                            EntityTaskDao.TaskStatus = (int)TaskStatus.In_progress;
                                    }
                                    Itask.Add(EntityTaskDao);
                                    txinner.PersistenceManager.PlanningRepository.Save<EntityTaskDao>(Itask);
                                    txinner.Commit();

                                }
                            }
                            else if (taskType.TaskType == 31)
                            {
                                if (TaskMembersDao != null)
                                {
                                    if (status != (int)TaskStatus.RevokeTask)
                                        TaskMembersDao.ApprovalStatus = (int)(TaskStatus)status;
                                    else
                                        TaskMembersDao.ApprovalStatus = null;
                                    Itaskmember.Add(TaskMembersDao);
                                    tx.PersistenceManager.PlanningRepository.Save<TaskMembersDao>(TaskMembersDao);
                                    tx.Commit();
                                }
                                else
                                    tx.Commit();

                                using (ITransaction txinner = proxy.MarcomManager.GetTransaction())
                                {
                                    currentTaskrountMembers = tx.PersistenceManager.PlanningRepository.Query<TaskMembersDao>().Where(a => a.TaskID == taskID && a.RoleID != 1).ToList();
                                    var total_Approval_Status_For_This_Round = currentTaskrountMembers.Where(a => a.ApprovalStatus == (int)TaskStatus.Completed).Count();
                                    if (EntityTaskDao.TaskStatus != (int)TaskStatus.Unable_to_complete && EntityTaskDao.TaskStatus != (int)TaskStatus.Rejected)
                                    {

                                        if (status == (int)TaskStatus.Unable_to_complete || status == (int)TaskStatus.Rejected)
                                        {
                                            EntityTaskDao.TaskStatus = status;
                                        }
                                        else
                                        {
                                            if (currentTaskrountMembers.Count() == (total_Approval_Status_For_This_Round))
                                                EntityTaskDao.TaskStatus = status;
                                        }
                                    }
                                    Itask.Add(EntityTaskDao);
                                    txinner.PersistenceManager.PlanningRepository.Save<EntityTaskDao>(Itask);
                                    txinner.Commit();

                                }

                            }
                            string statusname = "";
                            statusname = Enum.GetName(typeof(TaskStatus), Itask.FirstOrDefault().TaskStatus).Replace("_", " ");
                            retObj = Tuple.Create(true, Itask.FirstOrDefault().TaskStatus, statusname);


                            wfhobj.Actorid = TaskMembersDao.UserID;
                            wfhobj.action = "Task Status change";
                            wfhobj.Tasktatus = Enum.GetName(typeof(TaskStatus), status).Replace("_", " ");
                            wfhobj.obj2 = new List<object>();
                            foreach (var c in allTaskMembers)
                            {
                                wfhobj.obj2.Add((TaskMembersDao)c);

                            }
                            wfhobj.EntityId = taskID;
                            wfhobj.AttributeId = taskType.TaskType;
                            fs.AsynchronousNotify(wfhobj);

                            return retObj;
                        }
                    }
                    catch
                    {
                        return null;
                    }
                }
            }
            catch (MarcomAccessDeniedException ex)
            {
                throw ex;
            }
        }

        public IList<ITaskFlag> GetTaskFlags(TaskManagerProxy proxy)
        {
            try
            {
                IList<ITaskFlag> iiFlags = new List<ITaskFlag>();
                IList<TaskFlagDao> iflagdao = new List<TaskFlagDao>();
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    iflagdao = tx.PersistenceManager.TaskRepository.GetAll<TaskFlagDao>();
                    foreach (var val in iflagdao)
                    {
                        ITaskFlag iflag = new TaskFlag();
                        iflag.Caption = val.Caption;
                        iflag.ColorCode = val.ColorCode;
                        iflag.Description = val.Description;
                        iflag.Id = val.ID;
                        iflag.SortOder = val.Sortorder;
                        iiFlags.Add(iflag);
                    }
                    return iiFlags;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public Tuple<int, IEntityTaskList> InsertUpdateEntityTaskList(TaskManagerProxy proxy, int id, string caption, string description, int sortorder, int entityID)
        {
            try
            {
                proxy.MarcomManager.AccessManager.TryEntityTypeAccess(entityID, Modules.Planning);
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    IList<IEntityTaskList> itask = new List<IEntityTaskList>();
                    EntityTaskListDao tasklistdao = new EntityTaskListDao();
                    string newSortOrder = "SELECT COUNT(*)+1 AS SortOrder FROM TM_EntityTaskList";
                    IList sortOrderVal = tx.PersistenceManager.PlanningRepository.ExecuteQuery(newSortOrder);
                    int sortOrderID = (int)((System.Collections.Hashtable)(sortOrderVal)[0])["SortOrder"];

                    if (id > 0)
                    {
                        tasklistdao = tx.PersistenceManager.TaskRepository.Query<EntityTaskListDao>().Where(a => a.ID == id).Select(a => a).FirstOrDefault();
                    }
                    tasklistdao.Name = (caption.Trim().Length > 0 ? HttpUtility.HtmlEncode(caption) : tasklistdao.Name);
                    tasklistdao.Description = (description.Trim().Length > 0 ? HttpUtility.HtmlEncode(description) : description);
                    if (id == 0)
                        tasklistdao.Sortorder = sortOrderID;
                    tasklistdao.EntityID = entityID;
                    tasklistdao.OnTimeStatus = (tasklistdao.OnTimeStatus > 0 ? tasklistdao.OnTimeStatus : 0);
                    tasklistdao.OnTimeComment = (tasklistdao.OnTimeComment != "" ? tasklistdao.OnTimeComment : "");
                    tx.PersistenceManager.TaskRepository.Save<EntityTaskListDao>(tasklistdao);
                    tx.Commit();

                    IEntityTaskList itsklst = new EntityTaskList();
                    itsklst.Name = HttpUtility.HtmlDecode(tasklistdao.Name);
                    itsklst.EntityParentID = entityID;
                    itsklst.Id = tasklistdao.ID;
                    itsklst.Description = HttpUtility.HtmlDecode(tasklistdao.Description);
                    itsklst.Sortorder = tasklistdao.Sortorder;
                    Tuple<int, IEntityTaskList> taskObj = Tuple.Create(tasklistdao.ID, itsklst);

                    return taskObj;
                }
            }
            catch (MarcomAccessDeniedException ex)
            {
                throw ex;
            }
            catch
            {
                return null;
            }

        }

        public bool UpdateEntityTaskListSortOrder(TaskManagerProxy proxy, JArray SortOrderObject)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    IList<EntityTaskListDao> ientitylist = new List<EntityTaskListDao>();

                    if (SortOrderObject.Count() > 0)
                    {
                        foreach (var data in SortOrderObject)
                        {

                            EntityTaskListDao tasklistdao = new EntityTaskListDao();
                            tasklistdao = (from item in tx.PersistenceManager.TaskRepository.Query<EntityTaskListDao>() where item.ID == (int)data["TaskListID"] select item).FirstOrDefault();
                            if (tasklistdao != null)
                            {
                                tasklistdao.Sortorder = (int)data["SortOrderID"];
                            }
                            ientitylist.Add(tasklistdao);
                        }

                        tx.PersistenceManager.TaskRepository.Save<EntityTaskListDao>(ientitylist);
                        tx.Commit();
                        return true;
                    }
                    else
                        return false;
                }
            }
            catch
            {
                return false;
            }

        }

        public Tuple<int, IEntityTask> InsertUnassignedEntityTaskWithAttachments(TaskManagerProxy proxy, int parentEntityID, int taskTypeID, int entitytypeid, string TaskName, IEntityTask TaskList, IList<INewTaskAttachments> TaskAttachments, IList<IFile> TaskFiles, IList<ITaskMembers> taskMembers, IList<IAttributeData> entityattributedata, IList<IEntityTaskCheckList> AdminTaskChkLst, JArray arrAttchObj)
        {
            try
            {
                BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("-----------------------------------------------------------------------------------------------", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);
                BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("Started creating Task", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);
                int entityId = 0;

                WorkFlowNotifyHolder workFlowNotifyHolder = new WorkFlowNotifyHolder();

                workFlowNotifyHolder.Actorid = proxy.MarcomManager.User.Id;
                workFlowNotifyHolder.action = "Tasks";
                workFlowNotifyHolder.TypeName = taskTypeID.ToString();

                IEntityTask iadmtsk = new EntityTask();
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    IList<EntityTaskDao> iTask = new List<EntityTaskDao>();
                    entityId = TaskList.Id;
                    if (TaskList != null)
                    {

                        EntityTaskDao taskdao = new EntityTaskDao();
                        taskdao = tx.PersistenceManager.TaskRepository.Query<EntityTaskDao>().Where(a => a.ID == TaskList.Id).Select(a => a).FirstOrDefault();
                        taskdao.Name = TaskList.Name;
                        taskdao.TaskListID = TaskList.TaskListID;
                        taskdao.Description = TaskList.Description;
                        taskdao.Note = TaskList.Note;
                        taskdao.TaskType = TaskList.TaskType;
                        taskdao.EntityTaskListID = TaskList.EntityTaskListID;
                        taskdao.DueDate = TaskList.DueDate;
                        taskdao.EntityID = parentEntityID;
                        if (taskMembers.Count() > 0)
                            taskdao.TaskStatus = (int)TaskStatus.In_progress;
                        else if (taskMembers.Count() == 0)
                            taskdao.TaskStatus = (int)TaskStatus.Unassigned;
                        iTask.Add(taskdao);

                        tx.PersistenceManager.PlanningRepository.Save<EntityTaskDao>(iTask);
                        iadmtsk.Id = entityId;
                        BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("Saved in Task", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);

                        workFlowNotifyHolder.iEntityTask = new List<EntityTaskDao>();
                        workFlowNotifyHolder.iEntityTask = iTask;
                    }
                    tx.PersistenceManager.TaskRepository.DeleteByID<EntityTaskCheckListDao>(EntityTaskCheckListDao.PropertyNames.TaskId, TaskList.Id);
                    if (AdminTaskChkLst != null)
                    {
                        IList<EntityTaskCheckListDao> taskChklst = new List<EntityTaskCheckListDao>();
                        for (int i = 0; i <= AdminTaskChkLst.Count() - 1; i++)
                        {
                            if (AdminTaskChkLst[i].Name.Trim() != "")
                                taskChklst.Add(new EntityTaskCheckListDao { Name = AdminTaskChkLst[i].Name.Trim(), TaskId = iadmtsk.Id, SortOrder = (i + 1) });

                        }
                        if (taskChklst.Count > 0)
                            tx.PersistenceManager.PlanningRepository.Save<EntityTaskCheckListDao>(taskChklst);
                        iadmtsk.TaskCheckList = taskChklst;
                    }


                    foreach (var data in arrAttchObj)
                    {
                        IList<LinksDao> ilinks = new List<LinksDao>();
                        LinksDao lnks = new LinksDao();
                        string ext = (string)data["Extension"];
                        if (ext == "Link")
                        {
                            Guid NewId = Guid.NewGuid();
                            lnks = new LinksDao();
                            lnks.Name = (string)data["FileName"];
                            string LinkURL = (string)data["URL"];
                            lnks.URL = LinkURL;
                            lnks.Description = (string)data["FileDescription"];
                            lnks.CreatedOn = DateTime.Now.ToString();
                            lnks.EntityID = entityId;
                            lnks.ModuleID = 1;
                            lnks.OwnerID = proxy.MarcomManager.User.Id;
                            lnks.ActiveVersionNo = 1;
                            lnks.LinkGuid = NewId;
                            ilinks.Add(lnks);

                        }
                        tx.PersistenceManager.CommonRepository.Save<LinksDao>(ilinks);
                        BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("Saved in Links", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);
                    }



                    Dictionary<Guid, string> descriptionObj = new Dictionary<Guid, string>();
                    if (TaskFiles != null)
                    {
                        IList<FileDao> ifile = new List<FileDao>();
                        if (TaskFiles != null)
                        {

                            foreach (var a in TaskFiles)
                            {
                                Guid NewId = Guid.NewGuid();

                                string filePath = ReadAdminXML("FileManagment");
                                var DirInfo = System.IO.Directory.GetParent(filePath);
                                string newFilePath = DirInfo.FullName;
                                System.IO.File.Move(filePath + "\\" + a.strFileID.ToString() + a.Extension, newFilePath + "\\" + NewId + a.Extension);
                                FileDao fldao = new FileDao();
                                fldao.Checksum = a.Checksum;
                                fldao.CreatedOn = a.CreatedOn;
                                fldao.Entityid = entityId;
                                fldao.Extension = a.Extension;
                                fldao.MimeType = a.MimeType;
                                fldao.Moduleid = a.Moduleid;
                                fldao.Name = a.Name;
                                fldao.Ownerid = a.Ownerid;
                                fldao.Size = a.Size;
                                fldao.VersionNo = a.VersionNo;
                                fldao.Fileguid = NewId;
                                //fldao.VersioningFileId = fldao.Id;
                                ifile.Add(fldao);
                                descriptionObj.Add(NewId, a.Description);
                            }
                            tx.PersistenceManager.PlanningRepository.Save<FileDao>(ifile);
                            BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("Saved in File", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);
                        }

                        if (ifile.Count != 0)
                        {

                            IList<AttachmentsDao> iattachment = new List<AttachmentsDao>();
                            foreach (var a in ifile)
                            {
                                var result = descriptionObj.Where(r => r.Key == a.Fileguid).ToList();
                                AttachmentsDao attachedao = new AttachmentsDao();
                                attachedao.ActiveFileid = a.Id;
                                attachedao.ActiveVersionNo = 1;
                                attachedao.Createdon = DateTime.Now;
                                attachedao.Entityid = entityId;
                                attachedao.Name = a.Name;
                                attachedao.Typeid = taskTypeID;
                                attachedao.ActiveFileVersionID = a.Id;
                                attachedao.VersioningFileId = a.Id;
                                attachedao.Typeid = taskTypeID;
                                // attachedao.Description = result[0].Value;
                                iattachment.Add(attachedao);
                            }
                            tx.PersistenceManager.PlanningRepository.Save<AttachmentsDao>(iattachment);
                            BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("Saved in Task Members", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);

                            for (int i = 0; i < iattachment.Count(); i++)
                            {
                                if (i != 0)
                                {
                                    workFlowNotifyHolder.obj2.Add(iattachment[i]);
                                }
                            }
                        }

                    }
                    if (taskMembers.Count != 0)
                    {
                        IList<TaskMembersDao> ientityRole = new List<TaskMembersDao>();
                        TaskMembersDao entroledao = new TaskMembersDao();
                        entroledao.RoleID = 1;
                        entroledao.TaskID = entityId;
                        entroledao.UserID = proxy.MarcomManager.User.Id;
                        entroledao.ApprovalRount = 1;
                        entroledao.ApprovalStatus = null;
                        entroledao.FlagColorCode = "f5f5f5";
                        ientityRole.Add(entroledao);
                        foreach (var a in taskMembers)
                        {
                            entroledao = new TaskMembersDao();
                            entroledao.RoleID = a.RoleID;
                            entroledao.FlagColorCode = "f5f5f5";
                            entroledao.TaskID = entityId;
                            entroledao.UserID = a.UserID;
                            entroledao.ApprovalRount = 1;
                            entroledao.ApprovalStatus = null;
                            ientityRole.Add(entroledao);
                        }
                        tx.PersistenceManager.TaskRepository.Save<TaskMembersDao>(ientityRole);
                        BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("Saved in Task Members", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);

                        workFlowNotifyHolder.ientityTaskRole = new List<TaskMembersDao>();
                        for (int i = 0; i < ientityRole.Count(); i++)
                        {
                            // if (ientityRole[i].Userid != proxy.MarcomManager.User.Id)
                            {
                                workFlowNotifyHolder.ientityTaskRole.Add(ientityRole[i]);
                            }
                        }
                    }
                    if (entityattributedata != null)
                    {
                        var result = InsertEntityAttributes(tx, entityattributedata, entityId, entitytypeid);
                    }
                    tx.Commit();
                    iadmtsk.AttributeData = GetEntityMetadata(proxy, entityId, 30);
                    FeedNotificationServer fs = new FeedNotificationServer();
                    fs.AsynchronousNotify((NotificationFeedObjects)workFlowNotifyHolder);
                }
                iadmtsk = GetEntityTaskDetails(proxy, entityId);
                Tuple<int, IEntityTask> taskObj = Tuple.Create(entityId, iadmtsk);
                return taskObj;
            }
            catch (Exception ex)
            {

                return null;
            }


        }

        public ITaskLibraryTemplateHolder DuplicateEntityTaskList(TaskManagerProxy proxy, int id, int entityID)
        {
            try
            {
                ITaskLibraryTemplateHolder tskLst = new TaskLibraryTemplateHolder();
                int tasklistID = 0;
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {

                    var entityTaskListdao = tx.PersistenceManager.TaskRepository.Query<EntityTaskListDao>().Where(a => a.ID == id).Select(a => a).FirstOrDefault();
                    IList<IEntityTaskList> itask = new List<IEntityTaskList>();
                    EntityTaskListDao tasklistdao = new EntityTaskListDao();
                    string newSortOrder = "SELECT COUNT(*)+1 AS SortOrder FROM TM_EntityTaskList ";
                    IList sortOrderVal = tx.PersistenceManager.PlanningRepository.ExecuteQuery(newSortOrder);
                    int sortOrderID = (int)((System.Collections.Hashtable)(sortOrderVal)[0])["SortOrder"];
                    tasklistdao.Name = "Copy of " + entityTaskListdao.Name;
                    tasklistdao.Description = entityTaskListdao.Description;
                    tasklistdao.Sortorder = entityTaskListdao.Sortorder + 1 ;
                    tasklistdao.EntityID = entityID;
                    tasklistdao.OnTimeStatus = entityTaskListdao.OnTimeStatus;
                    tasklistdao.OnTimeComment = entityTaskListdao.OnTimeComment;
                    tx.PersistenceManager.TaskRepository.Save<EntityTaskListDao>(tasklistdao);

                    tskLst.LibraryName = tasklistdao.Name;
                    tskLst.LibraryDescription = tasklistdao.Description;
                    tskLst.ID = tasklistdao.ID;
                    tskLst.SortOrder = tasklistdao.Sortorder;
                    tskLst.IsGetTasks = false;
                    tskLst.IsExpanded = false;

                    tasklistID = tasklistdao.ID;

                    var entityTasks = tx.PersistenceManager.TaskRepository.Query<EntityTaskDao>().Where(a => a.TaskListID == id).Select(a => a).ToList();
                    tx.Commit();

                    foreach (var val in entityTasks)
                    {
                        using (ITransaction txInnerLoop = proxy.MarcomManager.GetTransaction())
                        {

                            var basedao = txInnerLoop.PersistenceManager.TaskRepository.Query<BaseEntityDao>().Where(a => a.Id == val.ID).Select(a => a).FirstOrDefault();

                            IList<IEntityTask> listTask = new List<IEntityTask>();

                            IEntityTask taskval = new EntityTask();
                            taskval.Name = "Copy of " + val.Name;
                            taskval.TaskListID = tasklistdao.ID;
                            taskval.Description = val.Description;
                            taskval.TaskType = val.TaskType;
                            taskval.SortOrder = val.Sortorder + 1;
                            taskval.EntityTaskListID = 0;
                            taskval.DueDate = null;
                            taskval.EntityID = entityID;
                            taskval.Id = val.ID;
                            listTask.Add(taskval);

                            IList<IFile> listFiles = new List<IFile>();
                            listFiles = null;
                            IList<IAttachments> listTaskattachment = new List<IAttachments>();
                            listTaskattachment = null;
                            IList<Ilinks> listTaskLinks = new List<Ilinks>();
                            listTaskLinks = null;
                            //now Damassetonely no cm_files   and links
                            //var entityTasksFiles = txInnerLoop.PersistenceManager.TaskRepository.Query<FileDao>().Where(a => a.Entityid == val.ID).Select(a => a).ToList();

                            //IList<IFile> listFiles = new List<IFile>();
                            //if (entityTasksFiles.Count > 0)
                            //{
                            //    foreach (var arm in entityTasksFiles)
                            //    {
                            //        IFile attachval = new BrandSystems.Marcom.Core.Common.File();
                            //        attachval.Checksum = arm.Checksum;
                            //        attachval.CreatedOn = arm.CreatedOn;
                            //        attachval.Extension = arm.Extension;
                            //        attachval.MimeType = arm.MimeType;
                            //        attachval.Moduleid = arm.Moduleid;
                            //        attachval.Name = arm.Name;
                            //        attachval.Ownerid = arm.Ownerid;
                            //        attachval.Size = arm.Size;
                            //        attachval.VersionNo = arm.VersionNo;
                            //        attachval.Fileguid = arm.Fileguid;
                            //        attachval.Description = arm.Description;
                            //        listFiles.Add(attachval);
                            //    }
                            //}
                            //else if (entityTasksFiles.Count == 0)
                            //{
                            //    listFiles = null;
                            //}

                            //var entityTasksAttachments = txInnerLoop.PersistenceManager.TaskRepository.Query<AttachmentsDao>().Where(a => a.Entityid == val.ID).Select(a => a).ToList();


                            //IList<IAttachments> listTaskattachment = new List<IAttachments>();
                            //if (entityTasksAttachments.Count > 0)
                            //{
                            //    foreach (var arm in entityTasksAttachments)
                            //    {
                            //        IAttachments attachval = new Attachments();
                            //        attachval.Name = arm.Name;
                            //        attachval.ActiveFileid = arm.ActiveFileid;
                            //        attachval.ActiveVersionNo = arm.ActiveVersionNo;
                            //        attachval.ActiveFileVersionID = arm.ActiveFileVersionID;
                            //        attachval.VersioningFileId = arm.VersioningFileId;
                            //        attachval.Createdon = DateTime.UtcNow;
                            //        attachval.Typeid = 4;
                            //        listTaskattachment.Add(attachval);
                            //    }
                            //}
                            //else if (entityTasksAttachments.Count == 0)
                            //{
                            //    listTaskattachment = null;
                            //}


                            //var entityTasksLinks = tx.PersistenceManager.TaskRepository.Query<LinksDao>().Where(a => a.EntityID == val.ID).Select(a => a).ToList();


                            //IList<Ilinks> listTaskLinks = new List<Ilinks>();
                            //if (entityTasksLinks.Count > 0)
                            //{
                            //    foreach (var arm in entityTasksLinks)
                            //    {
                            //        Ilinks attachval = new Links();
                            //        attachval.Name = arm.Name;
                            //        attachval.ActiveVersionNo = 1;
                            //        attachval.OwnerID = arm.OwnerID;
                            //        attachval.URL = arm.URL;
                            //        attachval.ModuleID = 1;
                            //        attachval.LinkGuid = attachval.LinkGuid;
                            //        attachval.Description = attachval.Description;
                            //        attachval.CreatedOn = DateTime.UtcNow.ToString();
                            //        attachval.TypeID = 0;
                            //        listTaskLinks.Add(attachval);
                            //    }
                            //}
                            //else if (entityTasksLinks.Count == 0)
                            //{
                            //    listTaskLinks = null;
                            //}

                            var entityTasksChecklist = tx.PersistenceManager.TaskRepository.Query<EntityTaskCheckListDao>().Where(a => a.TaskId == val.ID).Select(a => a).ToList();


                            IList<IEntityTaskCheckList> listTaskhecklist = new List<IEntityTaskCheckList>();
                            if (entityTasksChecklist.Count > 0)
                            {
                                foreach (var arm in entityTasksChecklist)
                                {
                                    IEntityTaskCheckList attachval = new EntityTaskCheckList();
                                    attachval.Name = arm.Name;
                                    attachval.CompletedOn = null;
                                    attachval.TaskId = val.ID;
                                    attachval.UserId = 0;
                                    attachval.OwnerId = 0;
                                    attachval.Status = false;
                                    attachval.SortOrder = arm.SortOrder;
                                    listTaskhecklist.Add(attachval);
                                }
                            }
                            else if (entityTasksChecklist.Count == 0)
                            {
                                listTaskhecklist = null;
                            }

                            IList<IAttributeData> listAttributes = new List<IAttributeData>();
                            listAttributes = GetEntityAttributesDetails(proxy, val.ID);


                            InsertEntityTaskWithAttachments(proxy, txInnerLoop, entityID, basedao.Typeid, val.Name, listTask, listTaskattachment, listFiles, listTaskLinks, listTaskhecklist, listAttributes, true);

                        }

                    }
                }
                tskLst.TaskList = GetEntityTaskListDetails(proxy, entityID, tasklistID);
                return tskLst;
            }
            catch
            {
                return null;
            }

        }

        public int InsertEntityTaskWithAttachments(TaskManagerProxy proxy, ITransaction txLoop, int parentEntityID, int taskTypeID, string TaskName, IList<IEntityTask> TaskList, IList<IAttachments> TaskAttachments, IList<IFile> TaskFiles, IList<Ilinks> TaskLinks, IList<IEntityTaskCheckList> TaskChecklist, IList<IAttributeData> entityattributedata, bool DuplicateTask = false)
        {
            try
            {
                proxy.MarcomManager.AccessManager.TryEntityTypeAccess(parentEntityID, Modules.Planning);
                BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("-----------------------------------------------------------------------------------------------", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);
                BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("Started creating Task", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);
                int entityId = 0;

                WorkFlowNotifyHolder workFlowNotifyHolder = new WorkFlowNotifyHolder();

                workFlowNotifyHolder.Actorid = proxy.MarcomManager.User.Id;
                workFlowNotifyHolder.action = "Tasks";
                workFlowNotifyHolder.TypeName = taskTypeID.ToString();

                Dictionary<int, int> Tasksmappingdict = new Dictionary<int, int>();
                IEntityTask iadmtsk = new EntityTask();

                IList<EntityTaskDao> iTask = new List<EntityTaskDao>();
                entityId = GetBaseEntityID(taskTypeID, TaskName, txLoop, parentEntityID, true, false, false, DuplicateTask);
                if (TaskList != null)
                {
                    foreach (var a in TaskList)
                    {

                        EntityTaskDao taskdao = new EntityTaskDao();
                        taskdao.Name = a.Name;
                        taskdao.TaskListID = a.TaskListID;
                        taskdao.Description = a.Description;
                        taskdao.TaskType = a.TaskType;
                        taskdao.EntityTaskListID = a.EntityTaskListID;
                        taskdao.DueDate = a.DueDate;
                        taskdao.Note = a.Note;
                        taskdao.EntityID = parentEntityID;
                        taskdao.ID = entityId;
                        taskdao.Sortorder = a.SortOrder + 1;
                        if (DuplicateTask == true && a.Id > 0 && entityId > 0)
                        {
                            if (!Tasksmappingdict.ContainsKey(entityId))
                            {
                                Tasksmappingdict.Add(a.Id, entityId);
                            }
                        }
                        taskdao.TaskStatus = (int)TaskStatus.Unassigned;
                        iTask.Add(taskdao);

                    }

                    txLoop.PersistenceManager.TaskRepository.Save<EntityTaskDao>(iTask);
                    iadmtsk.Id = entityId;
                    if (iTask.Count > 0 && TaskList != null)
                    {
                        //Adding to the Search Engine
                        BrandSystems.Marcom.Core.Interface.Managers.IEventManager _eventManager = null;
                        BrandSystems.Marcom.Core.Interface.Managers.IPluginManager _pluginManager = null;
                        MarcomManager marcommanager = new MarcomManager(_eventManager, _pluginManager);
                        BrandSystems.Marcom.Core.Managers.Proxy.PlanningManagerProxy pProxy = new BrandSystems.Marcom.Core.Managers.Proxy.PlanningManagerProxy(marcommanager);
                        System.Threading.Tasks.Task addtaskforsearch = new System.Threading.Tasks.Task(() => PlanningManager.Instance.AddEntityAsync(pProxy, entityId, iTask[0].Name, "", "Task"));
                        addtaskforsearch.Start();
                    }
                    BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("Saved in Task", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);

                    workFlowNotifyHolder.iEntityTask = new List<EntityTaskDao>();
                    workFlowNotifyHolder.iEntityTask = iTask;
                }
                if (TaskFiles != null)
                {
                    IList<FileDao> ifile = new List<FileDao>();
                    if (TaskFiles != null)
                    {

                        foreach (var a in TaskFiles)
                        {

                            Guid NewId = Guid.NewGuid();
                            string filePath = ReadAdminXML("FileManagment");
                            var DirInfo = System.IO.Directory.GetParent(filePath);
                            string newFilePath = DirInfo.FullName;
                            System.IO.File.Copy(filePath + "\\" + a.Fileguid.ToString() + a.Extension, newFilePath + "\\" + NewId + a.Extension);
                            FileDao fldao = new FileDao();
                            fldao.Checksum = a.Checksum;
                            fldao.CreatedOn = a.CreatedOn;
                            fldao.Entityid = entityId;
                            fldao.Extension = a.Extension;
                            fldao.MimeType = a.MimeType;
                            fldao.Moduleid = a.Moduleid;
                            fldao.Name = a.Name;
                            fldao.Ownerid = a.Ownerid;
                            fldao.Size = a.Size;
                            fldao.VersionNo = a.VersionNo;
                            fldao.Fileguid = NewId;
                            fldao.Description = a.Description;
                            ifile.Add(fldao);
                        }
                        txLoop.PersistenceManager.TaskRepository.Save<FileDao>(ifile);
                        BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("Saved in File", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);
                    }

                    if (ifile.Count != 0)
                    {

                        IList<AttachmentsDao> iattachment = new List<AttachmentsDao>();
                        foreach (var a in ifile)
                        {
                            AttachmentsDao attachedao = new AttachmentsDao();
                            attachedao.ActiveFileid = a.Id;
                            attachedao.ActiveVersionNo = 1;
                            attachedao.Createdon = DateTime.Now;
                            attachedao.Entityid = entityId;
                            attachedao.Name = a.Name;
                            attachedao.Typeid = taskTypeID;
                            attachedao.ActiveFileVersionID = a.Id;
                            attachedao.VersioningFileId = a.Id;
                            iattachment.Add(attachedao);

                        }
                        txLoop.PersistenceManager.TaskRepository.Save<AttachmentsDao>(iattachment);
                        BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("Saved in Task Members", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);

                    }

                }

                //copy links from the source tasks

                IList<LinksDao> ilinks = new List<LinksDao>();
                if (TaskLinks != null)
                {

                    foreach (var a in TaskLinks)
                    {

                        Guid NewId = Guid.NewGuid();

                        LinksDao linkdao = new LinksDao();
                        linkdao.Name = a.Name;
                        linkdao.EntityID = entityId;
                        linkdao.ActiveVersionNo = 1;
                        linkdao.OwnerID = a.OwnerID;
                        linkdao.URL = a.URL;
                        linkdao.ModuleID = 1;
                        linkdao.LinkGuid = NewId;
                        linkdao.Description = a.Description;
                        linkdao.CreatedOn = a.CreatedOn;
                        linkdao.TypeID = 0;

                        ilinks.Add(linkdao);
                    }
                    txLoop.PersistenceManager.TaskRepository.Save<LinksDao>(ilinks);
                    BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("Saved in links", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);
                }

                //copy all the checklist

                IList<EntityTaskCheckListDao> icheckist = new List<EntityTaskCheckListDao>();
                if (TaskChecklist != null)
                {

                    foreach (var a in TaskChecklist)
                    {
                        EntityTaskCheckListDao attachval = new EntityTaskCheckListDao();
                        attachval.Name = a.Name;
                        attachval.CompletedOn = null;
                        attachval.TaskId = entityId;
                        attachval.UserId = 0;
                        attachval.OwnerId = 0;
                        attachval.Status = false;
                        attachval.SortOrder = a.SortOrder;
                        icheckist.Add(attachval);
                    }
                    txLoop.PersistenceManager.TaskRepository.Save<EntityTaskCheckListDao>(icheckist);
                    BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("Saved in checklists", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);
                }


                if (entityattributedata != null)
                {
                    if (entityattributedata.Count > 0)
                    {
                        var result = InsertEntityAttributes(txLoop, entityattributedata, entityId, taskTypeID);
                    }
                }

                txLoop.Commit();
                FeedNotificationServer fs = new FeedNotificationServer();
                fs.AsynchronousNotify((NotificationFeedObjects)workFlowNotifyHolder);
                //Task Attachment
                //CreateAsset Transaction not taking 
                if (DuplicateTask == true && Tasksmappingdict != null)
                {
                    foreach (KeyValuePair<int, int> kvp in Tasksmappingdict)
                    {
                        //Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value);

                        int[] assetIdlist = { };
                        using (ITransaction tx1 = proxy.MarcomManager.GetTransaction())
                        {

                            assetIdlist = tx1.PersistenceManager.DamRepository.Query<AssetsDao>().Where(a => a.EntityID == kvp.Key).Select(a => a.ID).Distinct().ToArray();
                            tx1.Commit();
                        }
                        if (assetIdlist.Length > 0)
                        {
                            for (int i = 0; i < assetIdlist.Length; i++)
                            {
                                int newasset = 0;
                                List<IAssets> assetdet = new List<IAssets>();
                                IAssets asset = new Assets();
                                asset = proxy.MarcomManager.DigitalAssetManager.GetAssetAttributesDetails(assetIdlist[i]);

                                // IAssets assetdata = proxy.MarcomManager.DigitalAssetManager.GetAssetAttributesDetails(AssetID);
                                IList<IAttributeData> AttributeDatanew = new List<IAttributeData>();
                                AttributeDatanew = asset.AttributeData;
                                if (asset.Category == 0)
                                {

                                    var Filesassest = asset.Files.Where(a => a.ID == asset.ActiveFileID).Select(a => a).ToList();
                                    newasset = proxy.MarcomManager.DigitalAssetManager.CreateAsset(0, Convert.ToInt32(asset.AssetTypeid), asset.Name, AttributeDatanew, Filesassest[0].Name, 1, Filesassest[0].MimeType, Filesassest[0].Extension, Convert.ToInt64(Filesassest[0].Size), kvp.Value, Filesassest[0].Fileguid.ToString(), Filesassest[0].Description, true, Filesassest[0].Status, 0, Filesassest[0].Additionalinfo, asset.AssetAccess, asset.ID);

                                }
                                else
                                {
                                    newasset = proxy.MarcomManager.DigitalAssetManager.CreateBlankAsset(0, Convert.ToInt32(asset.AssetTypeid), asset.Name, AttributeDatanew, kvp.Value, asset.Category, asset.Url, true, 0, asset.AssetAccess, asset.ID);


                                }
                            }

                        }
                    }

                }
                //if (assetIdArr.Length > 0)
                //{
                //    for (int i = 0; i < assetIdArr.Length; i++)
                //    {
                //        int newasset = 0;
                //        List<IAssets> assetdet = new List<IAssets>();
                //        IAssets asset = new Assets();
                //        asset = proxy.MarcomManager.DigitalAssetManager.GetAssetAttributesDetails(assetIdArr[i]);

                //        // IAssets assetdata = proxy.MarcomManager.DigitalAssetManager.GetAssetAttributesDetails(AssetID);
                //        IList<IAttributeData> AttributeDatanew = new List<IAttributeData>();
                //        AttributeDatanew = asset.AttributeData;
                //        if (asset.Category == 0)
                //        {

                //            var Filesassest = asset.Files.Where(a => a.ID == asset.ActiveFileID).Select(a => a).ToList();
                //            newasset = proxy.MarcomManager.DigitalAssetManager.CreateAsset(0, Convert.ToInt32(asset.AssetTypeid), asset.Name, AttributeDatanew, Filesassest[0].Name, 1, Filesassest[0].MimeType, Filesassest[0].Extension, Convert.ToInt64(Filesassest[0].Size), entityId, Filesassest[0].Fileguid.ToString(), Filesassest[0].Description, true, Filesassest[0].Status, 0, Filesassest[0].Additionalinfo, asset.AssetAccess, asset.ID);

                //        }
                //        else
                //        {
                //            newasset = proxy.MarcomManager.DigitalAssetManager.CreateBlankAsset(0, Convert.ToInt32(asset.AssetTypeid), asset.Name, AttributeDatanew, entityId, asset.Category, asset.Url, true, 0, asset.AssetAccess, asset.ID);


                //        }
                //    }

                //}
                return entityId;
            }
            catch (MarcomAccessDeniedException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {

                return 0;
            }


        }
        public IList GetMytasks(TaskManagerProxy proxy, int FilterByentityID, int[] FilterStatusID, int pageNo, int AssignRole)
        {
            try
            {
                IList<MultiProperty> parLIST = new List<MultiProperty>();
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    StringBuilder MytaskQry = new StringBuilder();
                    IList<IMyTaskCollection> mytaskcollection = new List<IMyTaskCollection>();
                    MytaskQry.AppendLine(" DECLARE @RowsPerPage INT = " + 15 + ", ");
                    MytaskQry.AppendLine(" @PageNumber INT =" + pageNo + " ");
                    MytaskQry.Append(" SELECT  tet.ID 'TaskId',tet.Name 'TaskName',tet.[Description] 'TaskDescription',pe.ID 'EntityId' , pe.Name 'EntityName',tet.EntityTaskListID 'IsAdminTask',tet.TaskListID 'TaskListID', COUNT(*) OVER() AS 'Total_COUNT',");
                    MytaskQry.Append("        ( ");
                    MytaskQry.Append("        SELECT TOP 1 met.ColorCode ");
                    MytaskQry.Append("       FROM   MM_EntityType met ");
                    MytaskQry.Append("        WHERE  id                = ( ");
                    MytaskQry.Append("                 SELECT TOP 1 PM_Entity.TypeID ");
                    MytaskQry.Append("              FROM   PM_Entity ");
                    MytaskQry.Append("               WHERE  id     = tet.ID ");
                    MytaskQry.Append("          ) ");
                    MytaskQry.Append("        )              AS 'colorcode', ");
                    MytaskQry.Append("        ( ");
                    MytaskQry.Append("           SELECT TOP 1 met.ShortDescription ");
                    MytaskQry.Append("         FROM   MM_EntityType met ");
                    MytaskQry.Append("         WHERE  id                = ( ");
                    MytaskQry.Append("                 SELECT TOP 1 PM_Entity.TypeID ");
                    MytaskQry.Append("                 FROM   PM_Entity ");
                    MytaskQry.Append("                WHERE  id     = tet.ID ");
                    MytaskQry.Append("            ) ");
                    MytaskQry.Append("       )              AS 'ShortDescription', ");
                    MytaskQry.Append(" ttm.RoleID 'RoleId', isnull(CONVERT(VARCHAR(10), tet.DueDate,111),'') 'DueDate',isnull(DATEDIFF(d,GETDATE(),tet.DueDate),0) 'Noofdays',   dbo.pathinfo( tet.id ) as 'pathinfo',");
                    MytaskQry.Append("case when (DATEDIFF(d, GETDATE(), tet.DueDate)<0) then 0 when (DATEDIFF(d, GETDATE(), tet.DueDate)<7) then 1 ");
                    MytaskQry.Append(" when (DATEDIFF(d, GETDATE(), tet.DueDate)<0) then 2  else 3 end  'Weeks',");
                    MytaskQry.Append(" (SELECT top 1 uu2.FirstName + ' ' + uu2.LastName   FROM UM_User uu2 INNER  JOIN TM_Task_Members ttm2 ON uu2.ID=ttm2.UserID AND ttm2.TaskID=tet.ID AND ttm2.RoleID=" + (AssignRole == 1 ? 4 : 1) + ") 'UserName' ");
                    MytaskQry.Append(" ,(SELECT top 1 uu2.ID  FROM UM_User uu2 INNER  JOIN TM_Task_Members ttm2 ON uu2.ID=ttm2.UserID AND ttm2.TaskID=tet.ID AND ttm2.RoleID=" + (AssignRole == 1 ? 4 : 1) + ") 'UserID',tet.TaskStatus 'TaskStatus',tet.TaskType 'TaskType', ");
                    MytaskQry.Append("    (SELECT met.ColorCode FROM  MM_EntityType met WHERE met.ID=pe.TypeID) 'EColorCode',  ");
                    MytaskQry.Append("   (SELECT met.ShortDescription FROM  MM_EntityType met WHERE met.ID=pe.TypeID) 'EShortDescription' , ");
                    MytaskQry.Append(" CASE WHEN tet.TaskStatus=2 or tet.TaskStatus=3 or tet.TaskStatus=8 THEN  ");
                    MytaskQry.Append("       	''    ");
                    MytaskQry.Append("       	WHEN tet.TaskType=2 then  ");
                    MytaskQry.Append("       	( ");
                    MytaskQry.Append("       	SELECT CONVERT(NVARCHAR(50) ,COUNT(*))+'/'+CONVERT( NVARCHAR(50),(SELECT count(*) FROM TM_EntityTask tetsub1 ");
                    MytaskQry.Append("        	INNER JOIN TM_EntityTaskCheckList tetclsub1 ON tetsub1.ID = tetclsub1.TaskId 	 ");
                    MytaskQry.Append("        	WHERE tetsub1.TaskType = 2 AND tetsub1.ID = tet.ID ))  ");
                    MytaskQry.Append("        	FROM TM_EntityTask tetsub ");
                    MytaskQry.Append("        	INNER JOIN TM_EntityTaskCheckList tetclsub ");
                    MytaskQry.Append("        	ON tetsub.ID = tetclsub.TaskId ");
                    MytaskQry.Append("        	WHERE tetsub.TaskType = 2 AND tetsub.ID = tet.ID  AND tetclsub.[Status] = 1 ");
                    MytaskQry.Append("       	) ");
                    MytaskQry.Append("       	 ");
                    MytaskQry.Append("       else ");
                    MytaskQry.Append("       	 ");
                    MytaskQry.Append("       	(SELECT CONVERT(NVARCHAR(50), COUNT(*)) + '/' + CONVERT( ");
                    MytaskQry.Append("            NVARCHAR(50), ");
                    MytaskQry.Append("            ( ");
                    MytaskQry.Append("                SELECT COUNT(*) ");
                    MytaskQry.Append("                FROM   TM_Task_Members ttm ");
                    MytaskQry.Append("                WHERE  ttm.TaskID = tet.ID ");
                    MytaskQry.Append("                       AND ttm.RoleID != 1 ");
                    MytaskQry.Append("            ) ");
                    MytaskQry.Append("        )                ");
                    MytaskQry.Append(" FROM   TM_Task_Members     ttm ");
                    MytaskQry.Append(" WHERE  ttm.TaskID = tet.ID ");
                    MytaskQry.Append("        AND ttm.RoleID != 1 ");
                    MytaskQry.Append("        AND ttm.ApprovalStatus IS NOT NULL )   ");
                    MytaskQry.Append("        	 ");
                    MytaskQry.Append("        	 ");
                    MytaskQry.Append("         ");
                    MytaskQry.Append("        	 end ProgressCount ");
                    MytaskQry.Append("   FROM PM_Entity pe  ");
                    MytaskQry.Append(" INNER JOIN  TM_EntityTask tet ON pe.ID=tet.EntityID  AND pe.[Active]=1 ");
                    parLIST.Add(new MultiProperty { propertyName = "User_Id", propertyValue = proxy.MarcomManager.User.Id });
                    MytaskQry.Append(" INNER JOIN TM_Task_Members ttm ON tet.ID=ttm.TaskID  INNER JOIN UM_User uu ON ttm.UserID=uu.ID AND ttm.UserID= :User_Id ");
                    parLIST.Add(new MultiProperty { propertyName = "AssignRole", propertyValue = AssignRole });
                    MytaskQry.Append(" and ttm.roleid= :AssignRole");

                    if (FilterStatusID.Length > 0)
                    {
                        string inClause = "("
                                         + String.Join(",", FilterStatusID.Select(x => x.ToString()).ToArray())
                                       + ")";
                        MytaskQry.Append(" and tet.TaskStatus in" + inClause + "");
                    }
                    if (FilterByentityID == 2)
                        MytaskQry.Append(" ORDER BY pe.UniqueKey,DATEDIFF(d,GETDATE(),tet.DueDate) ");
                    else
                        MytaskQry.Append(" ORDER BY DATEDIFF(d,GETDATE(),tet.DueDate) ");

                    MytaskQry.AppendLine(" OFFSET(@PageNumber - 1) * @RowsPerPage ROWS ");

                    MytaskQry.AppendLine(" FETCH NEXT @RowsPerPage ROWS ONLY ");
                    var MyTaskResult = tx.PersistenceManager.PlanningRepository.ExecuteQuerywithParam(MytaskQry.ToString(), parLIST);
                    return MyTaskResult;
                }
            }
            catch
            {
                return null;
            }

        }
        public IList<IMyTaskCollection> GetMytasksAPI(TaskManagerProxy proxy, int FilterByentityID, int[] FilterStatusID, int StartRowno, int MaxRowNo, int AssignRole, int UserID)
        {
            try
            {
                IList<MultiProperty> parLIST = new List<MultiProperty>();
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {

                    StringBuilder MytaskQry = new StringBuilder();


                    IList<IMyTaskCollection> mytaskcollection = new List<IMyTaskCollection>();



                    MytaskQry.Append(" SELECT  tet.ID 'TaskId',tet.Name 'TaskName',tet.[Description] 'TaskDescription',pe.ID 'EntityId' , pe.Name 'EntityName',tet.EntityTaskListID 'IsAdminTask', ");
                    MytaskQry.Append(" ttm.RoleID 'RoleId', isnull(CONVERT(VARCHAR(10), tet.DueDate,111),'') 'DueDate',isnull(DATEDIFF(d,GETDATE(),tet.DueDate),0) 'Noofdays', ");
                    MytaskQry.Append("case when (DATEDIFF(d, GETDATE(), tet.DueDate)<0) then 0 when (DATEDIFF(d, GETDATE(), tet.DueDate)<7) then 1 ");
                    MytaskQry.Append(" when (DATEDIFF(d, GETDATE(), tet.DueDate)<0) then 2  else 3 end  'Weeks',");
                    MytaskQry.Append(" (SELECT top 1 uu2.FirstName + ' ' + uu2.LastName   FROM UM_User uu2 INNER  JOIN TM_Task_Members ttm2 ON uu2.ID=ttm2.UserID AND ttm2.TaskID=tet.ID AND ttm2.RoleID=" + (AssignRole == 1 ? 4 : 1) + ") 'UserName' ");
                    MytaskQry.Append(" ,(SELECT top 1 uu2.ID  FROM UM_User uu2 INNER  JOIN TM_Task_Members ttm2 ON uu2.ID=ttm2.UserID AND ttm2.TaskID=tet.ID AND ttm2.RoleID=" + (AssignRole == 1 ? 4 : 1) + ") 'UserID',tet.TaskStatus 'TaskStatus',tet.TaskType 'TaskType', ");
                    MytaskQry.Append("    (SELECT met.ColorCode FROM  MM_EntityType met WHERE met.ID=pe.TypeID) 'ColorCode',  ");
                    MytaskQry.Append("   (SELECT met.ShortDescription FROM  MM_EntityType met WHERE met.ID=pe.TypeID) 'ShortDescription' , ");

                    MytaskQry.Append(" CASE WHEN tet.TaskStatus=2 or tet.TaskStatus=3 or tet.TaskStatus=8 THEN  ");
                    MytaskQry.Append("       	''    ");
                    MytaskQry.Append("       	WHEN tet.TaskType=2 then  ");
                    MytaskQry.Append("       	( ");
                    MytaskQry.Append("       	SELECT CONVERT(NVARCHAR(50) ,COUNT(*))+'/'+CONVERT( NVARCHAR(50),(SELECT count(*) FROM TM_EntityTask tetsub1 ");
                    MytaskQry.Append("        	INNER JOIN TM_EntityTaskCheckList tetclsub1 ON tetsub1.ID = tetclsub1.TaskId 	 ");
                    MytaskQry.Append("        	WHERE tetsub1.TaskType = 2 AND tetsub1.ID = tet.ID ))  ");
                    MytaskQry.Append("        	FROM TM_EntityTask tetsub ");
                    MytaskQry.Append("        	INNER JOIN TM_EntityTaskCheckList tetclsub ");
                    MytaskQry.Append("        	ON tetsub.ID = tetclsub.TaskId ");
                    MytaskQry.Append("        	WHERE tetsub.TaskType = 2 AND tetsub.ID = tet.ID  AND tetclsub.[Status] = 1 ");
                    MytaskQry.Append("       	) ");
                    MytaskQry.Append("       	 ");
                    MytaskQry.Append("       else ");
                    MytaskQry.Append("       	 ");
                    MytaskQry.Append("       	(SELECT CONVERT(NVARCHAR(50), COUNT(*)) + '/' + CONVERT( ");
                    MytaskQry.Append("            NVARCHAR(50), ");
                    MytaskQry.Append("            ( ");
                    MytaskQry.Append("                SELECT COUNT(*) ");
                    MytaskQry.Append("                FROM   TM_Task_Members ttm ");
                    MytaskQry.Append("                WHERE  ttm.TaskID = tet.ID ");
                    MytaskQry.Append("                       AND ttm.RoleID != 1 ");
                    MytaskQry.Append("            ) ");
                    MytaskQry.Append("        )                ");
                    MytaskQry.Append(" FROM   TM_Task_Members     ttm ");
                    MytaskQry.Append(" WHERE  ttm.TaskID = tet.ID ");
                    MytaskQry.Append("        AND ttm.RoleID != 1 ");
                    MytaskQry.Append("        AND ttm.ApprovalStatus IS NOT NULL )   ");
                    MytaskQry.Append("        	 ");
                    MytaskQry.Append("        	 ");
                    MytaskQry.Append("         ");
                    MytaskQry.Append("        	 end ProgressCount ");
                    MytaskQry.Append("   FROM PM_Entity pe  ");
                    MytaskQry.Append(" INNER JOIN  TM_EntityTask tet ON pe.ID=tet.EntityID  AND pe.[Active]=1 ");
                    parLIST.Add(new MultiProperty { propertyName = "User_Id", propertyValue = UserID });
                    MytaskQry.Append(" INNER JOIN TM_Task_Members ttm ON tet.ID=ttm.TaskID  INNER JOIN UM_User uu ON ttm.UserID=uu.ID AND ttm.UserID= :User_Id ");
                    parLIST.Add(new MultiProperty { propertyName = "AssignRole", propertyValue = AssignRole });
                    MytaskQry.Append(" and ttm.roleid= :AssignRole");



                    if (FilterStatusID.Length > 0)
                    {
                        string inClause = "("
                                         + String.Join(",", FilterStatusID.Select(x => x.ToString()).ToArray())
                                       + ")";
                        MytaskQry.Append(" and tet.TaskStatus in" + inClause + "");
                    }
                    if (FilterByentityID == 2)
                        MytaskQry.Append(" ORDER BY pe.UniqueKey,DATEDIFF(d,GETDATE(),tet.DueDate) ");
                    else
                        MytaskQry.Append(" ORDER BY DATEDIFF(d,GETDATE(),tet.DueDate) ");

                    var MyTaskResult = tx.PersistenceManager.PlanningRepository.ExecuteQuerywithParam(MytaskQry.ToString(), parLIST).Cast<Hashtable>();//<EntityTaskListDao>().Where(a => a.EntityID == entityID).Select(a => a);


                    if (FilterByentityID == 2)
                    {
                        IList<int> LstofEntityIds = new List<int>();
                        foreach (var CurrentId in MyTaskResult)
                        {
                            if (!LstofEntityIds.Contains(Convert.ToInt32(CurrentId["EntityId"])))
                            {
                                LstofEntityIds.Add(Convert.ToInt32(CurrentId["EntityId"]));
                            }
                        }

                        foreach (var EntityIds in LstofEntityIds)
                        {
                            IMyTaskCollection mytaskCurrentdata = new MyTaskCollection();
                            List<MyTask> mytasklst = new List<MyTask>();
                            foreach (var val in MyTaskResult)
                            {
                                if (EntityIds == Convert.ToInt32(val["EntityId"]))
                                {
                                    MyTask Currentmytask = new MyTask();
                                    Currentmytask.UserName = val["UserName"].ToString();
                                    Currentmytask.UserID = Convert.ToInt32(val["UserID"]);
                                    Currentmytask.DueDate = val["DueDate"].ToString();//DateTime.Parse((string)val["DueDate"], CultureInfo.InvariantCulture); //Convert.ToDateTime(val["DueDate"]);  //DateTime.Parse(ab.databaseval.Value == null ? "" : (string)ab.databaseval.Value, CultureInfo.InvariantCulture)
                                    Currentmytask.EntityId = Convert.ToInt32(val["EntityId"]);
                                    Currentmytask.EntityName = val["EntityName"].ToString();
                                    Currentmytask.Noofdays = Convert.ToInt32(val["Noofdays"]);
                                    Currentmytask.RoleId = Convert.ToInt32(val["RoleId"]);
                                    Currentmytask.TaskDescription = val["TaskDescription"].ToString();
                                    Currentmytask.TaskId = Convert.ToInt32(val["TaskId"]);
                                    Currentmytask.IsAdminTask = Convert.ToInt32(val["IsAdminTask"]);
                                    Currentmytask.TaskName = val["TaskName"].ToString();
                                    Currentmytask.Path = proxy.MarcomManager.MetadataManager.GetPath(Convert.ToInt32(val["TaskId"]));
                                    Currentmytask.TaskStatus = Convert.ToInt32(val["TaskStatus"]);
                                    Currentmytask.TaskTypeId = Convert.ToInt32(val["TaskType"]);
                                    Currentmytask.ProgressCount = "";
                                    if (val["ProgressCount"].ToString().Length > 0)
                                    {
                                        Currentmytask.ProgressCount = "(" + val["ProgressCount"].ToString() + ")";
                                    }

                                    mytasklst.Add(Currentmytask);
                                    mytaskCurrentdata.FilterHeaderName = val["EntityName"].ToString();
                                    mytaskCurrentdata.EntityColorCode = (string)val["ColorCode"];
                                    mytaskCurrentdata.EntityShortDescription = (string)val["ShortDescription"];


                                }
                            }
                            mytaskCurrentdata.TaskCollection = mytasklst;
                            mytaskcollection.Add(mytaskCurrentdata);
                        }
                    }

                    else if (FilterByentityID == 1)
                    {

                        foreach (var EntityIds in Enum.GetValues(typeof(UpcomingWeeks)))
                        {
                            IMyTaskCollection mytaskCurrentdata = new MyTaskCollection();
                            List<MyTask> mytasklst = new List<MyTask>();
                            bool IsWeeks = false;
                            foreach (var val in MyTaskResult)
                            {
                                if ((int)EntityIds == Convert.ToInt32(val["Weeks"]))
                                {
                                    MyTask Currentmytask = new MyTask();
                                    Currentmytask.UserName = val["UserName"].ToString();
                                    Currentmytask.UserID = Convert.ToInt32(val["UserID"]);
                                    Currentmytask.DueDate = val["DueDate"].ToString();//DateTime.Parse((string)val["DueDate"], CultureInfo.InvariantCulture); //Convert.ToDateTime(val["DueDate"]);  //DateTime.Parse(ab.databaseval.Value == null ? "" : (string)ab.databaseval.Value, CultureInfo.InvariantCulture)
                                    Currentmytask.EntityId = Convert.ToInt32(val["EntityId"]);
                                    Currentmytask.EntityName = val["EntityName"].ToString();
                                    Currentmytask.Noofdays = Convert.ToInt32(val["Noofdays"]);
                                    Currentmytask.RoleId = Convert.ToInt32(val["RoleId"]);
                                    Currentmytask.TaskDescription = val["TaskDescription"].ToString();
                                    Currentmytask.TaskId = Convert.ToInt32(val["TaskId"]);
                                    Currentmytask.IsAdminTask = Convert.ToInt32(val["IsAdminTask"]);
                                    Currentmytask.TaskName = val["TaskName"].ToString();
                                    Currentmytask.Path = proxy.MarcomManager.MetadataManager.GetPath(Convert.ToInt32(val["TaskId"]));
                                    Currentmytask.TaskStatus = Convert.ToInt32(val["TaskStatus"]);
                                    Currentmytask.TaskTypeId = Convert.ToInt32(val["TaskType"]);
                                    Currentmytask.ProgressCount = "";
                                    if (val["ProgressCount"].ToString().Length > 0)
                                    {
                                        Currentmytask.ProgressCount = "(" + val["ProgressCount"].ToString() + ")";
                                    }

                                    mytasklst.Add(Currentmytask);
                                    mytaskCurrentdata.FilterHeaderName = ((int)EntityIds != (int)UpcomingWeeks.Upcoming ? Enum.GetName(typeof(UpcomingWeeks), EntityIds).Replace("_", " ") : Enum.GetName(typeof(UpcomingWeeks), EntityIds) + "(2 weeks or more)");
                                    MyTaskResult.ToList().Remove(val);
                                    IsWeeks = true;
                                }
                            }
                            if (IsWeeks == true)
                            {
                                mytaskCurrentdata.TaskCollection = mytasklst;
                                mytaskcollection.Add(mytaskCurrentdata);
                            }
                        }
                    }

                    else
                    {
                        IMyTaskCollection mytaskCurrentdata = new MyTaskCollection();
                        List<MyTask> mytasklst = new List<MyTask>();
                        foreach (var val in MyTaskResult)
                        {
                            MyTask Currentmytask = new MyTask();
                            Currentmytask.UserName = val["UserName"].ToString();
                            Currentmytask.UserID = Convert.ToInt32(val["UserID"]);
                            Currentmytask.DueDate = val["DueDate"].ToString();//DateTime.Parse((string)val["DueDate"], CultureInfo.InvariantCulture); //Convert.ToDateTime(val["DueDate"]);  //DateTime.Parse(ab.databaseval.Value == null ? "" : (string)ab.databaseval.Value, CultureInfo.InvariantCulture)
                            Currentmytask.EntityId = Convert.ToInt32(val["EntityId"]);
                            Currentmytask.EntityName = val["EntityName"].ToString();
                            Currentmytask.Noofdays = Convert.ToInt32(val["Noofdays"]);
                            Currentmytask.RoleId = Convert.ToInt32(val["RoleId"]);
                            Currentmytask.TaskDescription = val["TaskDescription"].ToString();
                            Currentmytask.TaskId = Convert.ToInt32(val["TaskId"]);
                            Currentmytask.IsAdminTask = Convert.ToInt32(val["IsAdminTask"]);
                            Currentmytask.TaskName = val["TaskName"].ToString();
                            Currentmytask.Path = proxy.MarcomManager.MetadataManager.GetPath(Convert.ToInt32(val["TaskId"]));
                            Currentmytask.TaskStatus = Convert.ToInt32(val["TaskStatus"]);
                            Currentmytask.TaskTypeId = Convert.ToInt32(val["TaskType"]);
                            Currentmytask.ProgressCount = "";
                            if (val["ProgressCount"].ToString().Length > 0)
                            {
                                Currentmytask.ProgressCount = "(" + val["ProgressCount"].ToString() + ")";
                            }
                            mytasklst.Add(Currentmytask);
                        }
                        mytaskCurrentdata.TaskCollection = mytasklst;
                        mytaskcollection.Add(mytaskCurrentdata);
                    }
                    return mytaskcollection;
                }
            }
            catch
            {
                return null;
            }

        }
        public bool DeleteEntityTaskLis(TaskManagerProxy proxy, int taskListID, int entityID)
        {
            try
            {
                if (entityID != 0)
                    proxy.MarcomManager.AccessManager.TryEntityTypeAccess(entityID, Modules.Planning);
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    var taskCollection = tx.PersistenceManager.TaskRepository.Query<EntityTaskDao>().Where(a => a.TaskListID == taskListID && a.TaskStatus != (int)TaskStatus.Unassigned).Select(a => a).ToList();
                    if (taskCollection.Count > 0)
                    {
                        return false;
                    }
                    else
                    {
                        tx.PersistenceManager.TaskRepository.DeleteByID<EntityTaskListDao>(EntityTaskListDao.PropertyNames.ID, taskListID);
                        //EntityTaskDao entitytask = new EntityTaskDao();
                        var entitytask = from tasks in tx.PersistenceManager.TaskRepository.GetAll<EntityTaskDao>() where tasks.TaskListID == taskListID select tasks;
                        foreach (var item in entitytask)
                        {
                            //Removing from the Search Engine
                            BrandSystems.Marcom.Core.Interface.Managers.IEventManager _eventManager = null;
                            BrandSystems.Marcom.Core.Interface.Managers.IPluginManager _pluginManager = null;
                            MarcomManager marcommanager = new MarcomManager(_eventManager, _pluginManager);
                            BrandSystems.Marcom.Core.Managers.Proxy.PlanningManagerProxy pProxy = new BrandSystems.Marcom.Core.Managers.Proxy.PlanningManagerProxy(marcommanager);
                            System.Threading.Tasks.Task addtaskforsearch = new System.Threading.Tasks.Task(() => PlanningManager.Instance.RemoveEntityAsync(pProxy, item.ID));
                            addtaskforsearch.Start();
                        }

                        tx.PersistenceManager.TaskRepository.DeleteByID<EntityTaskDao>(EntityTaskDao.PropertyNames.TaskListID, taskListID);
                        tx.Commit();
                        return true;
                    }
                }

            }
            catch (MarcomAccessDeniedException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public Tuple<int, IEntityTask> DuplicateEntityTask(TaskManagerProxy proxy, int taskId, int entityID = 0)
        {
            try
            {
                int[] assetIdlist = { };

                if (entityID != 0)
                    proxy.MarcomManager.AccessManager.TryEntityTypeAccess(entityID, Modules.Planning);
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    //if (taskId > 0)
                    //    assetIdlist = tx.PersistenceManager.DamRepository.Query<AssetsDao>().Where(a => a.EntityID == taskId).Select(a => a.ID).Distinct().ToArray();

                    var taskDao = tx.PersistenceManager.TaskRepository.Query<EntityTaskDao>().Where(a => a.ID == taskId).Select(a => a).FirstOrDefault();
                    var baseDao = tx.PersistenceManager.TaskRepository.Query<BaseEntityDao>().Where(a => a.Id == taskId).Select(a => a).FirstOrDefault();
                    IEntityTask iadmtsk = new EntityTask();
                    IList<IEntityTask> listTask = new List<IEntityTask>();

                    IEntityTask taskval = new EntityTask();
                    taskval.Name = "Copy of " + HttpUtility.HtmlDecode(taskDao.Name);
                    taskval.TaskListID = taskDao.TaskListID;
                    taskval.Description = taskDao.Description;
                    taskval.TaskType = taskDao.TaskType;
                    taskval.SortOrder = taskDao.Sortorder + 1;
                    taskval.EntityTaskListID = 0;
                    taskval.DueDate = null;
                    taskval.EntityID = taskDao.EntityID;
                    taskval.Id = taskId;
                    listTask.Add(taskval);



                    var entityTasksFiles = tx.PersistenceManager.TaskRepository.Query<FileDao>().Where(a => a.Entityid == taskDao.ID).Select(a => a).ToList();

                    IList<IFile> listFiles = new List<IFile>();
                    if (entityTasksFiles.Count > 0)
                    {
                        foreach (var arm in entityTasksFiles)
                        {
                            IFile attachval = new BrandSystems.Marcom.Core.Common.File();
                            attachval.Checksum = arm.Checksum;
                            attachval.CreatedOn = arm.CreatedOn;
                            attachval.Extension = arm.Extension;
                            attachval.MimeType = arm.MimeType;
                            attachval.Moduleid = arm.Moduleid;
                            attachval.Name = arm.Name;
                            attachval.Ownerid = arm.Ownerid;
                            attachval.Size = arm.Size;
                            attachval.Fileguid = arm.Fileguid;
                            attachval.VersionNo = arm.VersionNo;
                            attachval.Description = arm.Description;
                            listFiles.Add(attachval);
                        }
                    }
                    else if (entityTasksFiles.Count == 0)
                    {
                        listFiles = null;
                    }

                    var entityTasksAttachments = tx.PersistenceManager.TaskRepository.Query<AttachmentsDao>().Where(a => a.Entityid == taskDao.ID).Select(a => a).ToList();


                    IList<IAttachments> listTaskattachment = new List<IAttachments>();
                    if (entityTasksAttachments.Count > 0)
                    {
                        foreach (var arm in entityTasksAttachments)
                        {
                            IAttachments attachval = new Attachments();
                            attachval.Name = arm.Name;
                            attachval.ActiveFileid = arm.ActiveFileid;
                            attachval.ActiveVersionNo = arm.ActiveVersionNo;
                            attachval.ActiveFileVersionID = arm.ActiveFileVersionID;
                            attachval.VersioningFileId = arm.VersioningFileId;
                            attachval.Createdon = DateTime.UtcNow;
                            attachval.Typeid = 4;
                            listTaskattachment.Add(attachval);
                        }
                    }
                    else if (entityTasksAttachments.Count == 0)
                    {
                        listTaskattachment = null;
                    }

                    var entityTasksLinks = tx.PersistenceManager.TaskRepository.Query<LinksDao>().Where(a => a.EntityID == taskDao.ID).Select(a => a).ToList();


                    IList<Ilinks> listTaskLinks = new List<Ilinks>();
                    if (entityTasksLinks.Count > 0)
                    {
                        foreach (var arm in entityTasksLinks)
                        {
                            Ilinks attachval = new Links();
                            attachval.Name = arm.Name;
                            attachval.ActiveVersionNo = 1;
                            attachval.OwnerID = arm.OwnerID;
                            attachval.URL = arm.URL;
                            attachval.ModuleID = 1;
                            attachval.LinkGuid = attachval.LinkGuid;
                            attachval.Description = attachval.Description;
                            attachval.CreatedOn = DateTime.UtcNow.ToString();
                            attachval.TypeID = 0;
                            listTaskLinks.Add(attachval);
                        }
                    }
                    else if (entityTasksLinks.Count == 0)
                    {
                        listTaskLinks = null;
                    }

                    var entityTasksChecklist = tx.PersistenceManager.TaskRepository.Query<EntityTaskCheckListDao>().Where(a => a.TaskId == taskDao.ID).Select(a => a).ToList();


                    IList<IEntityTaskCheckList> listTaskhecklist = new List<IEntityTaskCheckList>();
                    if (entityTasksChecklist.Count > 0)
                    {
                        foreach (var arm in entityTasksChecklist)
                        {
                            IEntityTaskCheckList attachval = new EntityTaskCheckList();
                            attachval.Name = arm.Name;
                            attachval.CompletedOn = null;
                            attachval.UserId = 0;
                            attachval.OwnerId = 0;
                            attachval.Status = false;
                            attachval.SortOrder = arm.SortOrder;
                            listTaskhecklist.Add(attachval);
                        }
                    }
                    else if (entityTasksChecklist.Count == 0)
                    {
                        listTaskhecklist = null;
                    }


                    IList<IAttributeData> listAttributes = new List<IAttributeData>();
                    listAttributes = GetEntityAttributesDetails(proxy, baseDao.Id);

                    int newTaskId = InsertEntityTaskWithAttachments(proxy, tx, taskDao.EntityID, baseDao.Typeid, taskDao.Name, listTask, listTaskattachment, listFiles, listTaskLinks, listTaskhecklist, listAttributes, true);
                    iadmtsk = GetEntityTaskDetails(proxy, newTaskId);
                    Tuple<int, IEntityTask> taskObj = Tuple.Create(newTaskId, iadmtsk);
                    return taskObj;
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

        public int DeleteEntityTask(TaskManagerProxy proxy, int taskID, int entityID)
        {
            try
            {
                proxy.MarcomManager.AccessManager.TryEntityTypeAccess(entityID, Modules.Planning);
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    FeedNotificationServer fs = new FeedNotificationServer();
                    NotificationFeedObjects obj1 = new NotificationFeedObjects();
                    obj1.Actorid = proxy.MarcomManager.User.Id;
                    obj1.action = "delete task";
                    obj1.EntityId = taskID;

                    var taskCollection = tx.PersistenceManager.TaskRepository.Query<EntityTaskDao>().Where(a => a.ID == taskID).Select(a => a).FirstOrDefault();
                    if (taskCollection.TaskStatus == (int)TaskStatus.Not_Applicable)
                    {

                        return -1;

                    }
                    else
                    {
                        var taskdetailsdelete = (from entid in tx.PersistenceManager.TaskRepository.Query<EntityTaskDao>() where entid.ID == taskID select entid).FirstOrDefault();
                        var entitydetaitldelete = (from ent in tx.PersistenceManager.TaskRepository.Query<EntityDao>() where ent.Id == Convert.ToInt32(taskdetailsdelete.EntityID) select ent).FirstOrDefault();
                        obj1.AssociatedEntityId = entitydetaitldelete.Id;
                        var tasktypechkrdelete = (from ent in tx.PersistenceManager.TaskRepository.Query<EntityTypeDao>() where ent.Id == Convert.ToInt32(taskdetailsdelete.TaskType) select ent).FirstOrDefault();
                        obj1.TypeName = tasktypechkrdelete.Caption;

                        fs.AsynchronousNotify(obj1);

                        var entityDao = tx.PersistenceManager.PlanningRepository.Get<EntityDao>(taskID);
                        entityDao.Id = taskID;
                        entityDao.Active = false;
                        tx.PersistenceManager.PlanningRepository.Save<EntityDao>(entityDao);
                        tx.PersistenceManager.TaskRepository.DeleteByID<EntityTaskDao>(EntityTaskDao.PropertyNames.ID, taskID);
                        tx.Commit();



                        //Adding to the Search Engine
                        BrandSystems.Marcom.Core.Interface.Managers.IEventManager _eventManager = null;
                        BrandSystems.Marcom.Core.Interface.Managers.IPluginManager _pluginManager = null;
                        MarcomManager marcommanager = new MarcomManager(_eventManager, _pluginManager);
                        BrandSystems.Marcom.Core.Managers.Proxy.PlanningManagerProxy pProxy = new BrandSystems.Marcom.Core.Managers.Proxy.PlanningManagerProxy(marcommanager);
                        System.Threading.Tasks.Task addtaskforsearch = new System.Threading.Tasks.Task(() => PlanningManager.Instance.RemoveEntityAsync(pProxy, taskID));
                        addtaskforsearch.Start();

                        return 1;
                    }
                }

            }
            catch (MarcomAccessDeniedException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                return -100;
            }

        }

        public Tuple<int, IEntityTask> CompleteUnnassignedEntityTask(TaskManagerProxy proxy, int taskId)
        {
            using (ITransaction tx = proxy.MarcomManager.GetTransaction())
            {
                try
                {
                    IEntityTask iadmtsk = new EntityTask();
                    EntityTaskDao taskdao = new EntityTaskDao();

                    taskdao = (from item in tx.PersistenceManager.PlanningRepository.Query<EntityTaskDao>() where item.ID == taskId select item).FirstOrDefault();

                    if (taskdao != null)
                    {
                        taskdao.DueDate = DateTime.Now;
                        taskdao.EntityTaskListID = 0;
                        taskdao.TaskStatus = (int)TaskStatus.ForcefulComplete;
                        tx.PersistenceManager.PlanningRepository.Save<EntityTaskDao>(taskdao);
                        BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("Saved in Task", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);
                    }
                    IList<EntityTaskDao> ientityTaskDao = new List<EntityTaskDao>();
                    ientityTaskDao.Add(taskdao);
                    WorkFlowNotifyHolder workFlowNotifyHolder = new WorkFlowNotifyHolder();
                    workFlowNotifyHolder.Actorid = proxy.MarcomManager.User.Id;
                    workFlowNotifyHolder.action = "forcefully completed";
                    workFlowNotifyHolder.TypeName = taskdao.TaskType.ToString();
                    workFlowNotifyHolder.iEntityTask = new List<EntityTaskDao>();
                    workFlowNotifyHolder.iEntityTask = ientityTaskDao;
                    workFlowNotifyHolder.ientityRoles = new List<TaskMembersDao>();
                    var memberCount = (from item in tx.PersistenceManager.PlanningRepository.Query<TaskMembersDao>() where item.TaskID == taskId select item).ToList();
                    IList<TaskMembersDao> ientityRole = new List<TaskMembersDao>();
                    IList<TaskMembersDao> iiRole = new List<TaskMembersDao>();
                    TaskMembersDao entroledao = new TaskMembersDao();
                    if (memberCount.Count() == 0)
                    {
                        entroledao = new TaskMembersDao();
                        entroledao.RoleID = 1;
                        entroledao.TaskID = taskId;
                        entroledao.UserID = proxy.MarcomManager.User.Id;
                        entroledao.ApprovalRount = 1;
                        entroledao.ApprovalStatus = (int)TaskStatus.Approved;
                        entroledao.FlagColorCode = "f5f5f5";
                        ientityRole.Add(entroledao);
                        tx.PersistenceManager.TaskRepository.Save<TaskMembersDao>(ientityRole);
                        BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("Saved in Task Assignor", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);
                        workFlowNotifyHolder.ientityRoles = ientityRole;
                    }
                    else
                    {
                        var memberPresentObj = memberCount.Where(a => a.RoleID != 1 && a.UserID == proxy.MarcomManager.User.Id); //Removal of assignee code commented as per new requirement
                        if (memberPresentObj.Count() > 0)
                        {
                            foreach (var dao in memberPresentObj)
                            {
                                dao.ApprovalStatus = (int)TaskStatus.Approved;
                                iiRole.Add(dao);
                            }
                            tx.PersistenceManager.TaskRepository.Save<TaskMembersDao>(iiRole);
                            BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("Deleted Task Assignees", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);
                        }
                    }

                    tx.Commit();

                    iadmtsk = GetEntityTaskDetails(proxy, taskId);
                    Tuple<int, IEntityTask> taskObj = Tuple.Create(taskId, iadmtsk);
                    FeedNotificationServer fs = new FeedNotificationServer();
                    NotificationFeedObjects obj = new NotificationFeedObjects();
                    obj.Actorid = proxy.MarcomManager.User.Id;
                    obj.action = "forcefully completed";
                    obj.EntityId = taskId;

                    fs.AsynchronousNotify(obj);

                    return taskObj;
                }
                catch
                {
                    throw;
                }
            }
        }

        public Tuple<bool, int, string> UpdatetasktoNotApplicableandUnassigned(TaskManagerProxy proxy, int taskID)
        {
            using (ITransaction tx = proxy.MarcomManager.GetTransaction())
            {
                try
                {
                    Tuple<bool, int, string> retObj = null;
                    EntityTaskDao EntityTaskDao = new EntityTaskDao();
                    IList<EntityTaskDao> Itask = new List<EntityTaskDao>();
                    IList<MultiProperty> prplst = new List<MultiProperty>();
                    var taskType = (from item in tx.PersistenceManager.PlanningRepository.Query<EntityTaskDao>() where item.ID == taskID select item).FirstOrDefault();
                    prplst.Add(new MultiProperty { propertyName = EntityTaskDao.PropertyNames.ID, propertyValue = taskID });
                    EntityTaskDao = (tx.PersistenceManager.AccessRepository.GetEquals<EntityTaskDao>(prplst)).FirstOrDefault();
                    if (EntityTaskDao != null)
                    {
                        if (EntityTaskDao.TaskStatus == (int)TaskStatus.Unassigned)
                            EntityTaskDao.TaskStatus = (int)TaskStatus.Not_Applicable;
                        else
                            EntityTaskDao.TaskStatus = (int)TaskStatus.Unassigned;
                        Itask.Add(EntityTaskDao);
                        tx.PersistenceManager.PlanningRepository.Save<EntityTaskDao>(Itask);
                        tx.Commit();
                        string statusname = "";
                        statusname = Enum.GetName(typeof(TaskStatus), Itask.FirstOrDefault().TaskStatus).Replace("_", " ");
                        retObj = Tuple.Create(true, Itask.FirstOrDefault().TaskStatus, statusname);

                        return retObj;
                    }
                    return null;
                }
                catch
                {
                    return null;
                }
            }
        }

        public bool DeleteAdminTask(TaskManagerProxy proxy, int taskID)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    var taskCollection = tx.PersistenceManager.TaskRepository.Query<AdminTaskDao>().Where(a => a.ID == taskID).Select(a => a).FirstOrDefault();

                    tx.PersistenceManager.TaskRepository.DeleteByID<AdminTaskDao>(AdminTaskDao.PropertyNames.ID, taskID);
                    tx.PersistenceManager.TaskRepository.DeleteByID<AdminTaskCheckListDao>(AdminTaskCheckListDao.PropertyNames.TaskId, taskID);
                    tx.Commit();
                    return true;

                }

            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public bool DeleteTemplateConditionById(TaskManagerProxy proxy, int templateCondID)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {

                    tx.PersistenceManager.TaskRepository.DeleteByID<TasktemplateConditionDao>(templateCondID);
                    tx.Commit();
                    return true;

                }

            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public bool DeleteAdminTemplateTaskRelationById(TaskManagerProxy proxy, int TaskListId, int TemplateId)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    IList<MultiProperty> prpList = new List<MultiProperty>();
                    prpList.Add(new MultiProperty { propertyName = TempTaskListDao.PropertyNames.TempID, propertyValue = TemplateId });
                    prpList.Add(new MultiProperty { propertyName = TempTaskListDao.PropertyNames.TaskListID, propertyValue = TaskListId });
                    tx.PersistenceManager.TaskRepository.DeleteByID<TempTaskListDao>(prpList);
                    tx.Commit();
                    return true;
                }

            }
            catch (Exception ex)
            {
                //return false;
            }
            return false;
        }

        public bool DeleteTaskMemberById(TaskManagerProxy proxy, int id, int taskID)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    TaskMembersDao taskMemDao = new TaskMembersDao();
                    EntityTaskDao entDao = new EntityTaskDao();
                    WorkFlowNotifyHolder obj = new WorkFlowNotifyHolder();
                    FeedNotificationServer fs = new FeedNotificationServer();

                    IList<TaskMembersDao> totalmembers = new List<TaskMembersDao>();

                    using (ITransaction txinner = proxy.MarcomManager.GetTransaction())
                    {
                        taskMemDao = txinner.PersistenceManager.TaskRepository.Query<TaskMembersDao>().Where(a => a.ID == id).Select(a => a).FirstOrDefault();
                        txinner.PersistenceManager.TaskRepository.Delete<TaskMembersDao>(taskMemDao);
                        txinner.Commit();
                        totalmembers = txinner.PersistenceManager.TaskRepository.Query<TaskMembersDao>().Where(a => a.TaskID == taskID && a.RoleID != 1).Select(a => a).ToList();
                    }

                    entDao = tx.PersistenceManager.TaskRepository.Query<EntityTaskDao>().Where(a => a.ID == taskID).Select(a => a).FirstOrDefault();
                    obj.ientityRoles = new List<TaskMembersDao>();
                    obj.ientityRoles.Add(taskMemDao);
                    // obj.ientityRoles = totalmembers;


                    if ((totalmembers.Count() - 1) <= 0)
                    {
                        entDao.TaskStatus = (int)TaskStatus.Unassigned;
                        tx.PersistenceManager.TaskRepository.Save<EntityTaskDao>(entDao);
                    }
                    else
                    {

                        var currentTaskrountMembers = (from item in totalmembers where item.RoleID != 1 select item).ToList<TaskMembersDao>();
                        if (entDao.TaskType == 31)
                        {
                            var total_Completed_Status_For_This_Round = currentTaskrountMembers.Where(a => a.ApprovalStatus == (int)TaskStatus.Completed && a.UserID != id).Count();
                            if ((currentTaskrountMembers.Count() - 1) == (total_Completed_Status_For_This_Round))
                            {
                                entDao.TaskStatus = (int)TaskStatus.Completed;
                                tx.PersistenceManager.TaskRepository.Save<EntityTaskDao>(entDao);
                            }
                        }
                        else if (entDao.TaskType == 3)
                        {
                            var total_Approval_Status_For_This_Round = currentTaskrountMembers.Where(a => a.ApprovalStatus == (int)TaskStatus.Approved && a.UserID != id).Count();
                            if ((currentTaskrountMembers.Count() - 1) == (total_Approval_Status_For_This_Round))
                            {
                                entDao.TaskStatus = (int)TaskStatus.Approved;
                                tx.PersistenceManager.TaskRepository.Save<EntityTaskDao>(entDao);
                            }
                        }
                    }

                    tx.Commit();
                    obj.action = "Removed Task Member";
                    obj.Actorid = proxy.MarcomManager.User.Id;
                    obj.EntityId = taskID;
                    fs.AsynchronousNotify(obj);

                    //Adding to the Search Engine
                    BrandSystems.Marcom.Core.Interface.Managers.IEventManager _eventManager = null;
                    BrandSystems.Marcom.Core.Interface.Managers.IPluginManager _pluginManager = null;
                    MarcomManager marcommanager = new MarcomManager(_eventManager, _pluginManager);
                    BrandSystems.Marcom.Core.Managers.Proxy.PlanningManagerProxy pProxy = new BrandSystems.Marcom.Core.Managers.Proxy.PlanningManagerProxy(marcommanager);
                    System.Threading.Tasks.Task addtaskforsearch = new System.Threading.Tasks.Task(() => PlanningManager.Instance.UpdateEntityforSearchAsync(pProxy, taskID, entDao.Name));
                    addtaskforsearch.Start();

                    return true;

                }

            }
            catch (Exception ex)
            {
                //return false;
            }
            return false;
        }

        public bool CopyFileFromTaskToEntityAttachments(TaskManagerProxy proxy, int entityID, int ActiveFileID)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    var getTaskAttachFileDetails = tx.PersistenceManager.TaskRepository.Get<TaskAttachmentsDao>(NewTaskAttachmentsDao.PropertyNames.ActiveFileid, ActiveFileID);
                    var getFileDetails = tx.PersistenceManager.TaskRepository.Get<FileDao>(ActiveFileID);
                    if (getFileDetails != null)
                    {
                        getFileDetails.Id = 0;
                        string OldFilename = getFileDetails.Fileguid + getFileDetails.Extension;
                        Guid NewGuid = Guid.NewGuid();
                        getFileDetails.Fileguid = NewGuid;
                        string NewFilename = NewGuid + getFileDetails.Extension;
                        string Filepath = Path.Combine(HttpRuntime.AppDomainAppPath);
                        Filepath += "UploadedImages\\";
                        System.IO.File.Copy(Filepath + OldFilename, Filepath + NewFilename);
                        tx.PersistenceManager.TaskRepository.Save<FileDao>(getFileDetails);
                        AttachmentsDao Attachdao = new AttachmentsDao();
                        Attachdao.ActiveFileid = getFileDetails.Id;
                        Attachdao.Typeid = getTaskAttachFileDetails.Typeid;
                        Attachdao.Entityid = entityID;
                        Attachdao.Name = getFileDetails.Name;
                        Attachdao.ActiveVersionNo = getFileDetails.VersionNo;
                        Attachdao.Createdon = DateTime.Now;
                        Attachdao.ActiveFileVersionID = getFileDetails.Id;
                        Attachdao.VersioningFileId = getFileDetails.Id;
                        tx.PersistenceManager.TaskRepository.Save<AttachmentsDao>(Attachdao);
                        tx.Commit();
                        return true;
                    }

                }
            }
            catch
            {

            }
            return false;
        }

        public bool insertAttachmentToTask(TaskManagerProxy proxy, int taskID, int ActiveFileID)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    var getTaskAttachFileDetails = tx.PersistenceManager.TaskRepository.Get<TaskAttachmentsDao>(NewTaskAttachmentsDao.PropertyNames.ActiveFileid, ActiveFileID);
                    var getFileDetails = tx.PersistenceManager.TaskRepository.Get<FileDao>(ActiveFileID);
                    if (getFileDetails != null)
                    {
                        getFileDetails.Id = 0;
                        string OldFilename = getFileDetails.Fileguid + getFileDetails.Extension;
                        Guid NewGuid = Guid.NewGuid();
                        getFileDetails.Fileguid = NewGuid;
                        string NewFilename = NewGuid + getFileDetails.Extension;
                        string Filepath = Path.Combine(HttpRuntime.AppDomainAppPath);
                        Filepath += "UploadedImages\\";
                        System.IO.File.Copy(Filepath + OldFilename, Filepath + NewFilename);
                        tx.PersistenceManager.TaskRepository.Save<FileDao>(getFileDetails);
                        NewTaskAttachmentsDao Attachdao = new NewTaskAttachmentsDao();
                        Attachdao.ActiveFileid = getFileDetails.Id;
                        Attachdao.Typeid = getTaskAttachFileDetails.Typeid;
                        Attachdao.Entityid = taskID;
                        Attachdao.Name = getFileDetails.Name;
                        Attachdao.ActiveVersionNo = getFileDetails.VersionNo;
                        Attachdao.Createdon = DateTime.Now;
                        tx.PersistenceManager.TaskRepository.Save<NewTaskAttachmentsDao>(Attachdao);
                        tx.Commit();
                        return true;
                    }

                }
            }
            catch
            {

            }
            return false;
        }


        public void UpdateTaskTemplateCriteria(TaskManagerProxy proxy, string TemplateCriteriaText, int TemplateID)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    tx.PersistenceManager.TaskRepository.ExecuteQuerywithMinParam(" UPDATE TM_TaskTemplate SET TemplateCriteriaText= ?  WHERE Id =?", TemplateCriteriaText.ToString(), TemplateID);
                    tx.Commit();
                }
            }
            catch
            {

            }

        }


        public bool UpdatetaskLinkDescription(TaskManagerProxy proxy, int id, string friendlyName, string description, string LinkURL, int LinkType)
        {

            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    LinksDao fileDao = new LinksDao();
                    FeedNotificationServer fs = new FeedNotificationServer();
                    fileDao = tx.PersistenceManager.PlanningRepository.Query<LinksDao>().Where(a => a.ID == id).Select(a => a).FirstOrDefault();

                    if (fileDao.Name != friendlyName)
                    {
                        NotificationFeedObjects obj = new NotificationFeedObjects();
                        obj.Actorid = proxy.MarcomManager.User.Id;
                        obj.action = "Changed the task details";
                        obj.EntityId = fileDao.EntityID;
                        obj.AttributeName = "Link Name";
                        obj.FromValue = fileDao.Name;
                        obj.ToValue = friendlyName;
                        fs.AsynchronousNotify(obj);
                    }
                    if (fileDao.Description != description)
                    {
                        NotificationFeedObjects obj1 = new NotificationFeedObjects();
                        obj1.Actorid = proxy.MarcomManager.User.Id;
                        obj1.action = "Changed the task details";
                        obj1.EntityId = fileDao.EntityID;
                        obj1.AttributeName = "link description";
                        obj1.FromValue = fileDao.Description;
                        obj1.ToValue = description;
                        fs.AsynchronousNotify(obj1);
                    }
                    if (fileDao.URL != LinkURL)
                    {
                        NotificationFeedObjects obj1 = new NotificationFeedObjects();
                        obj1.Actorid = proxy.MarcomManager.User.Id;
                        obj1.action = "Changed the task details";
                        obj1.EntityId = fileDao.EntityID;
                        obj1.AttributeName = "link URL";
                        obj1.FromValue = fileDao.URL;
                        obj1.ToValue = LinkURL;
                        fs.AsynchronousNotify(obj1);
                    }
                    fileDao.Name = friendlyName;
                    fileDao.Description = description;
                    fileDao.URL = LinkURL;
                    fileDao.LinkType = LinkType;
                    tx.PersistenceManager.TaskRepository.Save<LinksDao>(fileDao);
                    tx.Commit();
                    return true;
                }

            }
            catch
            {
                return false;
            }
        }




        public bool UpdatetaskAttachmentDescription(TaskManagerProxy proxy, int id, string friendlyName, string description)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    FeedNotificationServer fs = new FeedNotificationServer();


                    AttachmentsDao attachDao = new AttachmentsDao();
                    attachDao = tx.PersistenceManager.PlanningRepository.Query<AttachmentsDao>().Where(a => a.ActiveFileid == id).Select(a => a).FirstOrDefault();


                    if (attachDao.Name != friendlyName)
                    {
                        NotificationFeedObjects obj = new NotificationFeedObjects();
                        obj.Actorid = proxy.MarcomManager.User.Id;
                        obj.action = "Changed the task details";
                        obj.EntityId = attachDao.Entityid;
                        obj.AttributeName = "File Name";
                        obj.FromValue = attachDao.Name;
                        obj.ToValue = friendlyName;
                        fs.AsynchronousNotify(obj);
                    }

                    attachDao.Name = (friendlyName.Trim().Length > 0 ? friendlyName : attachDao.Name);


                    tx.PersistenceManager.TaskRepository.Save<AttachmentsDao>(attachDao);
                    FileDao fileDao = new FileDao();
                    fileDao = tx.PersistenceManager.PlanningRepository.Query<FileDao>().Where(a => a.Id == id).Select(a => a).FirstOrDefault();
                    if (fileDao.Description != description)
                    {
                        NotificationFeedObjects obj1 = new NotificationFeedObjects();
                        obj1.Actorid = proxy.MarcomManager.User.Id;
                        obj1.action = "Changed the task details";
                        obj1.EntityId = attachDao.Entityid;
                        obj1.AttributeName = "file description";
                        obj1.FromValue = fileDao.Description;
                        obj1.ToValue = description;
                        fs.AsynchronousNotify(obj1);
                    }
                    fileDao.Name = attachDao.Name;
                    fileDao.Description = (description.Trim().Length > 0 ? description : fileDao.Description);
                    tx.PersistenceManager.TaskRepository.Save<FileDao>(fileDao);

                    tx.Commit();

                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// DeleteFileByID.
        /// </summary>
        /// <param name="proxy">ID Parameter</param>
        /// <returns>bool</returns>
        public bool DeleteFileByID(TaskManagerProxy proxy, int ID)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    FileDao fldao = new FileDao();
                    fldao = tx.PersistenceManager.PlanningRepository.Get<FileDao>(NewTaskAttachmentsDao.MappingNames.Id, ID);
                    string flPath = ReadAdminXML("FileManagment");//HttpContext.Current.Server.MapPath("~/documents/" + fldao.Fileguid + fldao.Extension);
                    var DirInfo = System.IO.Directory.GetParent(flPath);
                    string newFilePath = DirInfo.FullName + "\\" + fldao.Fileguid + fldao.Extension;
                    if (System.IO.File.Exists(newFilePath))
                    {
                        System.IO.File.Delete(newFilePath);
                        tx.PersistenceManager.PlanningRepository.DeleteByID<NewTaskAttachmentsDao>(TaskAttachmentsDao.MappingNames.ActiveFileid, ID);
                        tx.PersistenceManager.CommonRepository.DeleteByID<FileDao>(FileDao.MappingNames.Id, ID);
                        tx.Commit();
                        return true;
                    }
                }
            }
            catch
            {

            }

            return false;
        }


        public bool DeleteTaskFileByid(TaskManagerProxy proxy, int ID, int EntityId)
        {
            try
            {
                //proxy.MarcomManager.AccessManager.TryEntityTypeAccess(EntityId, Modules.Planning);
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    FileDao fldao = new FileDao();
                    fldao = tx.PersistenceManager.PlanningRepository.Get<FileDao>(NewTaskAttachmentsDao.MappingNames.Id, ID);
                    string flPath = ReadAdminXML("FileManagment");//HttpContext.Current.Server.MapPath("~/documents/" + fldao.Fileguid + fldao.Extension);
                    var DirInfo = System.IO.Directory.GetParent(flPath);
                    string newFilePath = DirInfo.FullName + "\\" + fldao.Fileguid + fldao.Extension;
                    tx.PersistenceManager.TaskRepository.ExecuteQuerywithMinParam("delete from PM_Attachments where ActiveFileID = ? and EntityID= ? ", ID, EntityId);
                    tx.PersistenceManager.TaskRepository.ExecuteQuerywithMinParam("delete from CM_File where ID =  ? and EntityID= ? ", ID, EntityId);

                    FeedNotificationServer fs = new FeedNotificationServer();
                    NotificationFeedObjects obj = new NotificationFeedObjects();
                    obj.action = "attachment deleted";
                    obj.Actorid = proxy.MarcomManager.User.Id;
                    obj.EntityId = EntityId;
                    obj.AttributeName = fldao.Name;
                    obj.ToValue = ID.ToString();
                    obj.attachmenttype = "file";
                    fs.AsynchronousNotify(obj);

                    tx.Commit();

                    //Removing from the Search Engine
                    BrandSystems.Marcom.Core.Interface.Managers.IEventManager _eventManager = null;
                    BrandSystems.Marcom.Core.Interface.Managers.IPluginManager _pluginManager = null;
                    MarcomManager marcommanager = new MarcomManager(_eventManager, _pluginManager);
                    BrandSystems.Marcom.Core.Managers.Proxy.PlanningManagerProxy pProxy = new BrandSystems.Marcom.Core.Managers.Proxy.PlanningManagerProxy(marcommanager);
                    System.Threading.Tasks.Task addtaskforsearch = new System.Threading.Tasks.Task(() => PlanningManager.Instance.RemoveEntityAsync(pProxy, ID));
                    addtaskforsearch.Start();

                    if (System.IO.File.Exists(newFilePath))
                    {
                        System.IO.File.Delete(newFilePath);
                    }

                    return true;

                }
            }
            catch (MarcomAccessDeniedException ex)
            {
                throw ex;
            }
            catch
            {

            }

            return false;
        }


        public bool DeleteTaskLinkByid(TaskManagerProxy proxy, int ID, int EntityId)
        {
            try
            {
                //proxy.MarcomManager.AccessManager.TryEntityTypeAccess(EntityId, Modules.Planning);
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    LinksDao fldao = new LinksDao();
                    fldao = tx.PersistenceManager.PlanningRepository.Get<LinksDao>(LinksDao.MappingNames.ID, ID);

                    FeedNotificationServer fs = new FeedNotificationServer();
                    NotificationFeedObjects obj = new NotificationFeedObjects();
                    obj.action = "attachment deleted";
                    obj.Actorid = proxy.MarcomManager.User.Id;
                    obj.EntityId = EntityId;
                    obj.ToValue = ID.ToString();
                    obj.AttributeName = fldao.Name;
                    obj.attachmenttype = "link";
                    //string deletefile = "delete  CM_link where ID= ' + ID + ' and Entityid=' + EntityId + '";
                    //tx.PersistenceManager.PlanningRepository.ExecuteQuery(deletefile);
                    fs.AsynchronousNotify(obj);
                    tx.PersistenceManager.TaskRepository.ExecuteQuerywithMinParam("delete from CM_links where ID = ? and EntityID = ? ", ID, EntityId);
                    // tx.PersistenceManager.PlanningRepository.DeleteByID<AttachmentsDao>(AttachmentsDao.MappingNames.Id, ID);

                    // tx.PersistenceManager.TaskRepository.DeleteByID<LinksDao>(LinksDao.MappingNames.ID, ID);
                    // tx.PersistenceManager.CommonRepository.DeleteByID<FileDao>(FileDao.MappingNames.Id, ID);
                    tx.Commit();
                    return true;
                }
            }
            catch (MarcomAccessDeniedException ex)
            {
                throw ex;
            }
            catch
            {
            }
            return false;
        }


















        /// <summary>
        /// InsertLink.(in CM_Links Table)
        /// </summary>
        /// <param name="proxy">file Parameter</param>
        /// <returns>int</returns>
        /// 
        public int InsertLink(TaskManagerProxy proxy, int EntityID, string Name, string URL, int linkType, string Description, int ActiveVersionNo, int TypeID, string CreatedOn, int OwnerID, int ModuleID)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    Guid NewId = Guid.NewGuid();
                    LinksDao lnkdao = new LinksDao();
                    lnkdao.EntityID = EntityID;
                    lnkdao.Name = Name;
                    lnkdao.URL = URL;
                    lnkdao.LinkType = linkType;
                    lnkdao.Description = Description;
                    lnkdao.ActiveVersionNo = 0;
                    lnkdao.TypeID = 9;
                    lnkdao.CreatedOn = DateTime.Now.ToString("yyyy-MM-dd");
                    lnkdao.OwnerID = proxy.MarcomManager.User.Id;
                    lnkdao.ModuleID = 1;
                    lnkdao.LinkGuid = NewId;
                    tx.PersistenceManager.CommonRepository.Save<LinksDao>(lnkdao);
                    tx.Commit();
                    FeedNotificationServer fs = new FeedNotificationServer();
                    NotificationFeedObjects obj = new NotificationFeedObjects();
                    obj.action = "attachment added";
                    obj.AttributeName = Name;
                    obj.attachmenttype = "link";
                    obj.Actorid = proxy.MarcomManager.User.Id;
                    obj.ToValue = Convert.ToString(lnkdao.ID);
                    obj.EntityId = EntityID;
                    fs.AsynchronousNotify(obj);
                    return lnkdao.ID;
                }
            }
            catch
            {


            }
            return 0;
        }

        /// <summary>
        /// InsertLink.(in TM_Links Table)
        /// </summary>
        /// <param name="proxy">file Parameter</param>
        /// <returns>int</returns>
        /// 
        public int InsertLinkInAdminTasks(TaskManagerProxy proxy, int EntityID, string Name, string URL, int LinkType, string Description, int ActiveVersionNo, int TypeID, string CreatedOn, int OwnerID, int ModuleID)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    Guid NewId = Guid.NewGuid();
                    TaskLinksDao lnkdao = new TaskLinksDao();
                    lnkdao.EntityID = EntityID;
                    lnkdao.Name = Name;
                    lnkdao.URL = URL;
                    lnkdao.LinkType = LinkType;
                    lnkdao.Description = Description;
                    lnkdao.ActiveVersionNo = 0;
                    lnkdao.TypeID = 9;
                    lnkdao.CreatedOn = DateTime.Now.ToString("yyyy-MM-dd");
                    lnkdao.OwnerID = proxy.MarcomManager.User.Id;
                    lnkdao.ModuleID = 1;
                    lnkdao.LinkGuid = NewId;
                    tx.PersistenceManager.CommonRepository.Save<TaskLinksDao>(lnkdao);
                    tx.Commit();
                    FeedNotificationServer fs = new FeedNotificationServer();
                    NotificationFeedObjects obj = new NotificationFeedObjects();
                    obj.action = "attachment added";
                    obj.AttributeName = Name;
                    obj.attachmenttype = "link";
                    obj.Actorid = proxy.MarcomManager.User.Id;
                    obj.ToValue = Convert.ToString(lnkdao.ID);
                    obj.EntityId = EntityID;
                    fs.AsynchronousNotify(obj);
                    return lnkdao.ID;
                }
            }
            catch
            {


            }
            return 0;
        }




        /// <summary>
        /// DeleteLinkByID.
        /// </summary>
        /// <param name="proxy">ID Parameter</param>
        /// <returns>bool</returns>
        public bool DeleteLinkByID(TaskManagerProxy proxy, int ID)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    LinksDao fldao = new LinksDao();
                    fldao = tx.PersistenceManager.PlanningRepository.Get<LinksDao>(LinksDao.MappingNames.ID, ID);
                    tx.PersistenceManager.PlanningRepository.DeleteByID<LinksDao>(LinksDao.MappingNames.ID, ID);
                    tx.PersistenceManager.CommonRepository.DeleteByID<LinksDao>(LinksDao.MappingNames.ID, ID);
                    tx.Commit();
                    return true;
                }
            }
            catch
            {

            }
            return false;
        }

        public bool SendReminderNotification(TaskManagerProxy proxy, int taskmemberid, int taskid)
        {
            try
            {
                using (ITransaction txInnerloop = proxy.MarcomManager.GetTransaction())
                {
                    String taskownerName = "";
                    String taskownerEmail = "";

                    String tasklogopath = "";
                    string taskImagePath = "";
                    string xmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
                    XDocument adminXmlDoc = XDocument.Load(xmlpath);
                    //The Key is root node current Settings
                    string xelementName = "ApplicationURL";
                    var xelementFilepath = XElement.Load(xmlpath);
                    var xmlElement = xelementFilepath.Element(xelementName);


                    MailServer mailServer = new MailServer();
                    MailHolder taskmailHolder = new MailHolder();
                    taskmailHolder.PrimaryTo = new List<string>();

                    var Userid = (from tt in txInnerloop.PersistenceManager.UserRepository.Query<UserDao>() where tt.Id == Convert.ToInt32(proxy.MarcomManager.User.Id) select tt).FirstOrDefault();

                    taskownerName = Userid.FirstName + " " + Userid.LastName;
                    taskownerEmail = Userid.Email;


                    tasklogopath = xmlElement.Value.ToString() + "/assets/img/logo.png";
                    taskImagePath = xmlElement.Value.ToString() + "/UserImages/" + Userid.Id + ".jpg";

                    if (System.IO.File.Exists(Path.Combine(Path.Combine(xmlElement.Value.ToString() + "/UserImages/", Userid.Id + ".jpg"))) == false)
                        taskImagePath = xmlElement.Value.ToString() + "/UserImages/noimage.jpg";
                    taskmailHolder.MailType = 38;
                    taskmailHolder.MailTemplateDictionary = new Dictionary<string, string>();

                    taskmailHolder.MailTemplateDictionary.Add("@AssingedBy@", taskownerName);
                    taskmailHolder.MailTemplateDictionary.Add("@Time@", DateTimeOffset.Now.ToString());
                    taskmailHolder.MailTemplateDictionary.Add("@AssingedByImage@", taskImagePath);
                    taskmailHolder.MailTemplateDictionary.Add("@ApplicationURL@", "");
                    taskmailHolder.MailTemplateDictionary.Add("@AssingedByMailID@", taskownerEmail);
                    taskmailHolder.MailTemplateDictionary.Add("@ImagePath@", tasklogopath);


                    var taskdetails = (from tg in txInnerloop.PersistenceManager.TaskRepository.Query<EntityTaskDao>() where tg.ID == Convert.ToInt32(taskid) select tg).FirstOrDefault();
                    var ttype = (from tt in txInnerloop.PersistenceManager.TaskRepository.Query<EntityTypeDao>() where tt.Id == Convert.ToInt32(taskdetails.TaskType) select tt).FirstOrDefault();
                    var mem = (from tg in txInnerloop.PersistenceManager.TaskRepository.Query<TaskMembersDao>() where tg.ID == Convert.ToInt32(taskmemberid) select tg).FirstOrDefault();
                    var taskEntityDetails = (from entity in txInnerloop.PersistenceManager.TaskRepository.Query<EntityDao>() where entity.Id == Convert.ToInt32(taskdetails.EntityID) select entity).FirstOrDefault();
                    var actorDetail = (from act in txInnerloop.PersistenceManager.UserRepository.Query<UserDao>() where act.Id == Convert.ToInt32(mem.UserID) select act).FirstOrDefault();
                    var actorName = actorDetail.FirstName + " " + actorDetail.LastName;

                    taskmailHolder.PrimaryTo.Add(actorDetail.Email);

                    taskmailHolder.MailTemplateDictionary.Add("@EntityTypeName@", ttype.Caption);
                    taskmailHolder.MailTemplateDictionary.Add("@EntityName@", "<a href=" + proxy.MarcomManager.CommonManager.GetEntityPathforMail(xmlElement.Value.ToString(), taskdetails.ID, taskdetails.TaskType, Userid.Id, Userid.IsSSOUser, taskdetails.EntityID) + ">" + taskdetails.Name + "</a>");
                    taskmailHolder.MailTemplateDictionary.Add("@DueDate@", Convert.ToDateTime(taskdetails.DueDate.ToString()).ToString("yyyy-MM-dd"));
                    taskmailHolder.MailTemplateDictionary.Add("@Path@", "<a href=" + proxy.MarcomManager.CommonManager.GetEntityPathforMail(xmlElement.Value.ToString(), taskEntityDetails.Id, taskEntityDetails.Typeid, Userid.Id, Userid.IsSSOUser, taskEntityDetails.Parentid) + ">" + taskEntityDetails.Name + "</a>");

                    mailServer.HandleUnScheduledMail(taskmailHolder);
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool UpdatetaskEntityTaskDetails(TaskManagerProxy proxy, int TaskID, string taskName, string description, string note, string Duedate, string taskaction, int entityID = 0)
        {

            try
            {
                if (entityID != 0)
                    proxy.MarcomManager.AccessManager.TryEntityTypeAccess(entityID, Modules.Planning);
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {

                    FeedNotificationServer fs = new FeedNotificationServer();
                    NotificationFeedObjects obj = new NotificationFeedObjects();
                    EntityTaskDao taskDao = new EntityTaskDao();
                    taskDao = tx.PersistenceManager.PlanningRepository.Query<EntityTaskDao>().Where(a => a.ID == TaskID).Select(a => a).FirstOrDefault();
                    if (taskaction == "nam")
                    {
                        obj.AttributeName = "Task Name";
                        obj.FromValue = taskDao.Name;
                        obj.ToValue = taskName;
                    }
                    if (taskaction == "des")
                    {
                        obj.AttributeName = "Description";
                        if (taskDao.Description != null)
                            obj.FromValue = (taskDao.Description.ToString().Length > 0 ? taskDao.Description.ToString() : "-");
                        else
                            obj.FromValue = "-";
                       obj.ToValue = description;
                    }
                    if (taskaction == "note")
                    {
                        obj.AttributeName = "Notes";
                        if (taskDao.Note != null)
                            obj.FromValue = (taskDao.Note.ToString().Length > 0 ? taskDao.Note.ToString() : "-");
                        else
                            obj.FromValue = "-";
                        obj.ToValue = note;
                    }
                    if (taskaction == "due" || taskaction == "unassignedue")
                    {
                        obj.AttributeName = "Due date";
                        obj.FromValue = (taskDao.DueDate.ToString().Length > 0 ? taskDao.DueDate.Value.ToString(proxy.MarcomManager.GlobalAdditionalSettings[0].SettingValue.ToString().Replace('m', 'M').ToString()) : "-");
                        DateTime tostartDate = new DateTime();
                        if (DateTime.TryParse(Duedate, out tostartDate))
                            obj.ToValue = (tostartDate.ToString().Length > 0 ? tostartDate.ToString(proxy.MarcomManager.GlobalAdditionalSettings[0].SettingValue.ToString().Replace('m', 'M').ToString()) : "-");
                        else
                            obj.ToValue = Duedate;
                    }
                    if (taskaction == "nam")
                        taskDao.Name = taskName.Trim().Length > 0 ? HttpUtility.HtmlEncode(taskName) : taskDao.Name;
                    if (taskaction == "des")
                        taskDao.Description = description.Trim().Length > 0 ? HttpUtility.HtmlEncode(description) : description;
                    if (taskaction == "note")
                        taskDao.Note = note.Trim().Length > 0 ? HttpUtility.HtmlEncode(note) : note;
                    if (taskaction == "due" || taskaction == "unassignedue")
                    {
                        DateTime startDate = new DateTime();
                        if (DateTime.TryParse(Duedate, out startDate))
                            taskDao.DueDate = startDate;
                        else
                            taskDao.DueDate = taskDao.DueDate;
                        if (Duedate.Length == 0 && (taskName.Trim().Length == 0 && description.Trim().Length == 0 && note.Trim().Length == 0))
                        {
                            taskDao.DueDate = null;
                        }
                    }

                    if (taskName != "")
                    {
                        try
                        {
                            BaseEntityDao basedao = new BaseEntityDao();
                            basedao = tx.PersistenceManager.TaskRepository.Query<BaseEntityDao>().Where(a => a.Id == TaskID).FirstOrDefault();
                            if (basedao != null)
                            {
                                basedao.Name = HttpUtility.HtmlEncode(taskName);
                                tx.PersistenceManager.TaskRepository.Save<BaseEntityDao>(basedao);
                            }
                        }
                        catch { }
                    }

                    tx.PersistenceManager.TaskRepository.Save<EntityTaskDao>(taskDao);
                    tx.Commit();
                    obj.Actorid = proxy.MarcomManager.User.Id;
                    obj.action = "Changed the task details";
                    obj.EntityId = TaskID;
                    fs.AsynchronousNotify(obj);

                    //Adding to the Search Engine
                    BrandSystems.Marcom.Core.Interface.Managers.IEventManager _eventManager = null;
                    BrandSystems.Marcom.Core.Interface.Managers.IPluginManager _pluginManager = null;
                    MarcomManager marcommanager = new MarcomManager(_eventManager, _pluginManager);
                    BrandSystems.Marcom.Core.Managers.Proxy.PlanningManagerProxy pProxy = new BrandSystems.Marcom.Core.Managers.Proxy.PlanningManagerProxy(marcommanager);
                    System.Threading.Tasks.Task addtaskforsearch = new System.Threading.Tasks.Task(() => PlanningManager.Instance.UpdateEntityforSearchAsync(pProxy, TaskID, taskName, "Task"));
                    addtaskforsearch.Start();

                    return true;
                }
            }
            catch (MarcomAccessDeniedException ex)
            {
                throw ex;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdatetaskEntityTaskDueDate(TaskManagerProxy proxy, int TaskID, string Duedate)
        {

            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {

                    FeedNotificationServer fs = new FeedNotificationServer();
                    NotificationFeedObjects obj = new NotificationFeedObjects();
                    EntityTaskDao taskDao = new EntityTaskDao();
                    taskDao = tx.PersistenceManager.PlanningRepository.Query<EntityTaskDao>().Where(a => a.ID == TaskID).Select(a => a).FirstOrDefault();
                    obj.AttributeName = "Due date";
                    obj.FromValue = (taskDao.DueDate.ToString().Length > 0 ? taskDao.DueDate.Value.ToString(proxy.MarcomManager.GlobalAdditionalSettings[0].SettingValue.ToString().Replace('m', 'M').ToString()) : "-");
                    obj.ToValue = "-";
                    DateTime startDate = new DateTime();
                    taskDao.DueDate = null;
                    tx.PersistenceManager.TaskRepository.Save<EntityTaskDao>(taskDao);
                    tx.Commit();

                    obj.Actorid = proxy.MarcomManager.User.Id;
                    obj.action = "Changed the task details";
                    obj.EntityId = TaskID;
                    fs.AsynchronousNotify(obj);

                    return true;
                }

            }
            catch
            {
                return false;
            }
        }

        public bool DeleteAdminTaskCheckListByID(TaskManagerProxy proxy, int chkLstID)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {

                    tx.PersistenceManager.TaskRepository.DeleteByID<AdminTaskCheckListDao>(chkLstID);
                    tx.Commit();
                    return true;
                }
            }
            catch
            {

            }
            return false;
        }

        public bool UpdateTaskSortOrder(TaskManagerProxy proxy, int TaskId, int taskListID, int sortorder)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    AdminTaskDao tasklistdao = new AdminTaskDao();
                    tasklistdao = (from item in tx.PersistenceManager.TaskRepository.Query<AdminTaskDao>() where item.TaskListID == taskListID && item.ID == TaskId select item).FirstOrDefault();
                    if (tasklistdao != null)
                    {
                        tasklistdao.Sortorder = sortorder;
                    }
                    tx.PersistenceManager.TaskRepository.Save<AdminTaskDao>(tasklistdao);
                    tx.Commit();
                    return true;
                }
            }
            catch
            {
                return false;
            }

        }

        public bool UpdateEntityTaskSortOrder(TaskManagerProxy proxy, JArray SortOrderObject)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    IList<EntityTaskDao> ientitylist = new List<EntityTaskDao>();
                    if (SortOrderObject.Count() > 0)
                    {
                        foreach (var data in SortOrderObject)
                        {

                            EntityTaskDao tasklistdao = new EntityTaskDao();
                            tasklistdao = (from item in tx.PersistenceManager.TaskRepository.Query<EntityTaskDao>() where item.TaskListID == (int)data["TaskListID"] && item.ID == (int)data["TaskID"] select item).FirstOrDefault();
                            if (tasklistdao != null)
                            {
                                tasklistdao.Sortorder = (int)data["SortOrderID"];
                            }
                            ientitylist.Add(tasklistdao);
                        }

                        tx.PersistenceManager.TaskRepository.Save<EntityTaskDao>(ientitylist);
                        tx.Commit();
                        return true;
                    }
                    else
                        return false;
                }
            }
            catch
            {
                return false;
            }

        }

        public IList<IEntityTaskCheckList> getTaskchecklist(TaskManagerProxy proxy, int TaskId)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    IList<IEntityTaskCheckList> iichecklist = new List<IEntityTaskCheckList>();
                    IList<EntityTaskCheckListDao> checkDao = new List<EntityTaskCheckListDao>();
                    checkDao = tx.PersistenceManager.TaskRepository.Query<EntityTaskCheckListDao>().Where(a => a.TaskId == TaskId).Select(a => a).OrderBy(a => a.SortOrder).ToList();
                    foreach (var data in checkDao)
                    {
                        IEntityTaskCheckList ichecklist = new EntityTaskCheckList();
                        ichecklist.Id = data.Id;
                        ichecklist.Name = data.Name;
                        ichecklist.SortOrder = data.SortOrder;
                        ichecklist.Status = data.Status;
                        string UserName = "";
                        if (data.UserId != 0)
                            UserName = tx.PersistenceManager.TaskRepository.Query<UserDao>().Where(u => u.Id == data.UserId).Select(u => u.FirstName + " " + u.LastName).FirstOrDefault();
                        if (data.Status != false)
                            ichecklist.CompletedOnValue = (string)data.CompletedOn.Value.ToString();
                        else
                            ichecklist.CompletedOnValue = "";
                        ichecklist.UserName = UserName;
                        ichecklist.UserId = data.UserId;
                        ichecklist.CompletedOn = data.CompletedOn;
                        string OwnerName = "";
                        if (data.OwnerId != 0)
                            OwnerName = tx.PersistenceManager.TaskRepository.Query<UserDao>().Where(u => u.Id == data.UserId).Select(u => u.FirstName + " " + u.LastName).FirstOrDefault();

                        ichecklist.OwnerName = OwnerName;
                        ichecklist.OwnerId = data.OwnerId;
                        ichecklist.IsExisting = true;
                        iichecklist.Add(ichecklist);

                    }
                    return iichecklist;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public int InsertUpdateEntityTaskCheckList(TaskManagerProxy proxy, int Id, int taskId, String CheckListName, bool ChkListStatus, bool ISowner, int sortOrder, bool IsNew, int entityID = 0)
        {
            try
            {
                if (entityID != 0)
                    proxy.MarcomManager.AccessManager.TryEntityTypeAccess(entityID, Modules.Planning);
                BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("-----------------------------------------------------------------------------------------------", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);
                BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("Started creating Task", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);
                FeedNotificationServer fs = new FeedNotificationServer();
                NotificationFeedObjects obj = new NotificationFeedObjects();
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    IList<EntityTaskCheckListDao> entityCheckDao = new List<EntityTaskCheckListDao>();
                    IList<EntityTaskCheckListDao> ExistCheckDao = new List<EntityTaskCheckListDao>();

                    int sortOrderID = sortOrder;
                    EntityTaskCheckListDao checkDao = new EntityTaskCheckListDao();
                    bool currentStatus = false;
                    if (Id == 0)
                    {

                        //New logic after checking checklist
                        ExistCheckDao = tx.PersistenceManager.TaskRepository.Query<EntityTaskCheckListDao>().Where(a => a.SortOrder >= sortOrder && a.TaskId == taskId).Select(a => a).ToList();
                        foreach (var item in ExistCheckDao)
                        {
                            item.SortOrder = item.SortOrder + 1;
                            entityCheckDao.Add(item);
                        }


                        checkDao.Id = Id;
                        checkDao.Name = CheckListName;
                        checkDao.OwnerId = (ISowner == true ? proxy.MarcomManager.User.Id : checkDao.OwnerId);
                        checkDao.SortOrder = sortOrderID;
                        checkDao.TaskId = taskId;
                        checkDao.UserId = (ISowner == true ? 0 : proxy.MarcomManager.User.Id);
                        checkDao.Status = ChkListStatus;
                        checkDao.CompletedOn = (ISowner == true ? null : (DateTime?)DateTime.UtcNow);
                        entityCheckDao.Add(checkDao);
                        tx.PersistenceManager.TaskRepository.Save<EntityTaskCheckListDao>(entityCheckDao);
                    }
                    else
                    {
                        checkDao = tx.PersistenceManager.TaskRepository.Query<EntityTaskCheckListDao>().Where(a => a.TaskId == taskId && a.Id == Id).Select(a => a).FirstOrDefault();

                        if (checkDao.Name != CheckListName)
                        {
                            obj.Actorid = proxy.MarcomManager.User.Id;
                            obj.action = "Changed the task details";
                            obj.EntityId = taskId;
                            obj.AttributeName = "Check list name";
                            obj.FromValue = checkDao.Name;
                            obj.ToValue = CheckListName;
                        }
                        checkDao.Id = Id;
                        checkDao.Name = CheckListName;
                        checkDao.OwnerId = (ISowner == true ? proxy.MarcomManager.User.Id : checkDao.OwnerId);
                        checkDao.TaskId = taskId;
                        checkDao.UserId = (ISowner == true ? 0 : proxy.MarcomManager.User.Id);
                        checkDao.Status = ChkListStatus;
                        checkDao.CompletedOn = (ISowner == true ? null : (DateTime?)DateTime.UtcNow);
                        entityCheckDao.Add(checkDao);
                        tx.PersistenceManager.TaskRepository.Save<EntityTaskCheckListDao>(entityCheckDao);
                        fs.AsynchronousNotify(obj);
                    }


                    if (Id == 0)
                    {
                        obj.action = "add check list";
                        obj.Actorid = proxy.MarcomManager.User.Id;
                        obj.EntityId = taskId;
                        obj.AttributeName = CheckListName;

                    }
                    else
                    {

                        var delchkdetails = (from tt in tx.PersistenceManager.TaskRepository.Query<EntityTaskCheckListDao>() where tt.Id == Id select tt).FirstOrDefault();
                        var taskdeldetais = (from tt in tx.PersistenceManager.TaskRepository.Query<EntityTaskDao>() where tt.ID == delchkdetails.TaskId select tt).FirstOrDefault();
                        var entdeldetais = (from tt in tx.PersistenceManager.TaskRepository.Query<EntityDao>() where tt.Id == taskdeldetais.EntityID select tt).FirstOrDefault();
                        currentStatus = delchkdetails.Status;

                        if (ChkListStatus == true)
                            obj.ToValue = "Checked";
                        else
                            obj.ToValue = "Unchecked";
                        obj.EntityId = taskId;
                        obj.Actorid = proxy.MarcomManager.User.Id;
                        obj.AttributeName = delchkdetails.Name;
                        obj.AssociatedEntityId = entdeldetais.Id;
                        obj.action = "status change check list";
                    }

                    tx.Commit();
                    if ((currentStatus != ChkListStatus) || (Id == 0))
                    {
                        fs.AsynchronousNotify(obj);
                    }
                    return checkDao.Id;
                }

            }
            catch (MarcomAccessDeniedException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
            }
            return 0;
        }

        public bool InsertTaskCheckList(TaskManagerProxy proxy, int taskID, string CheckListName, int sortOrder)
        {
            try
            {
                BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("-----------------------------------------------------------------------------------------------", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);
                BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("Started creating Task", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);

                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    IList<EntityTaskCheckListDao> entityCheckDao = new List<EntityTaskCheckListDao>();
                    IList<EntityTaskCheckListDao> ExistCheckDao = new List<EntityTaskCheckListDao>();
                    int sortOrderID = sortOrder;
                    if (sortOrderID == 1 || sortOrderID == 0)
                    {
                        string newSortOrder = "SELECT COUNT(*)+1 AS SortOrder FROM TM_EntityTaskCheckList tetcl WHERE tetcl.TaskId= ? ";
                        IList sortOrderVal = tx.PersistenceManager.PlanningRepository.ExecuteQuerywithMinParam(newSortOrder, taskID);
                        sortOrderID = (int)((System.Collections.Hashtable)(sortOrderVal)[0])["SortOrder"];
                    }
                    if (sortOrder > 1)
                    {
                        ExistCheckDao = tx.PersistenceManager.TaskRepository.Query<EntityTaskCheckListDao>().Where(a => a.SortOrder >= sortOrder && a.TaskId == taskID).Select(a => a).ToList();
                        foreach (var item in ExistCheckDao)
                        {
                            item.SortOrder = item.SortOrder + 1;
                            entityCheckDao.Add(item);
                        }

                    }

                    EntityTaskCheckListDao checkDao = new EntityTaskCheckListDao();
                    checkDao.Name = CheckListName;
                    checkDao.OwnerId = proxy.MarcomManager.User.Id;
                    checkDao.SortOrder = sortOrderID;
                    checkDao.TaskId = taskID;
                    checkDao.UserId = 0;
                    checkDao.Status = false;
                    checkDao.CompletedOn = null;
                    entityCheckDao.Add(checkDao);
                    tx.PersistenceManager.TaskRepository.Save<EntityTaskCheckListDao>(entityCheckDao);
                    tx.Commit();

                    FeedNotificationServer fs = new FeedNotificationServer();
                    NotificationFeedObjects obj = new NotificationFeedObjects();
                    obj.action = "add check list";
                    obj.Actorid = proxy.MarcomManager.User.Id;
                    obj.EntityId = taskID;
                    obj.AttributeName = CheckListName;

                    fs.AsynchronousNotify(obj);
                    return true;
                }

            }
            catch (Exception ex)
            {
                return false;
            }


        }

        /// <summary>
        /// Check the task checklist
        /// </summary>
        /// <param name="proxy">ID</param>
        /// <returns>bool</returns>
        public bool ChecksTaskCheckList(TaskManagerProxy proxy, int Id, bool status)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    FeedNotificationServer fs = new FeedNotificationServer();
                    NotificationFeedObjects obj = new NotificationFeedObjects();

                    EntityTaskCheckListDao checkDao = new EntityTaskCheckListDao();
                    checkDao = tx.PersistenceManager.TaskRepository.Query<EntityTaskCheckListDao>().Where(a => a.Id == Id).FirstOrDefault();
                    if (checkDao != null)
                    {
                        if (status == true)
                        {
                            checkDao.UserId = proxy.MarcomManager.User.Id;
                            checkDao.Status = true;
                            checkDao.CompletedOn = DateTime.Now;
                            obj.ToValue = "Checked";

                        }
                        else
                        {
                            checkDao.UserId = 0;
                            checkDao.Status = false;
                            checkDao.CompletedOn = null;
                            obj.ToValue = "Unchecked";
                        }
                        tx.PersistenceManager.TaskRepository.Save<EntityTaskCheckListDao>(checkDao);
                        var delchkdetails = (from tt in tx.PersistenceManager.TaskRepository.Query<EntityTaskCheckListDao>() where tt.Id == Id select tt).FirstOrDefault();
                        var taskdeldetais = (from tt in tx.PersistenceManager.TaskRepository.Query<EntityTaskDao>() where tt.ID == delchkdetails.TaskId select tt).FirstOrDefault();
                        var entdeldetais = (from tt in tx.PersistenceManager.TaskRepository.Query<EntityDao>() where tt.Id == taskdeldetais.EntityID select tt).FirstOrDefault();
                        tx.Commit();
                        obj.EntityId = taskdeldetais.ID;
                        obj.AttributeName = delchkdetails.Name;
                        obj.AssociatedEntityId = entdeldetais.Id;
                        obj.action = "status change check list";
                        obj.Actorid = proxy.MarcomManager.User.Id;

                        fs.AsynchronousNotify(obj);
                        return true;
                    }
                    return false;
                }
            }

            catch (Exception ex)
            {
                return false;
            }

        }

        public bool DeleteEntityCheckListByID(TaskManagerProxy proxy, int chkLstID)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    FeedNotificationServer fs = new FeedNotificationServer();
                    NotificationFeedObjects obj = new NotificationFeedObjects();

                    var delchkdetails = (from tt in tx.PersistenceManager.TaskRepository.Query<EntityTaskCheckListDao>() where tt.Id == chkLstID select tt).FirstOrDefault();
                    var taskdeldetais = (from tt in tx.PersistenceManager.TaskRepository.Query<EntityTaskDao>() where tt.ID == delchkdetails.TaskId select tt).FirstOrDefault();
                    var entdeldetais = (from tt in tx.PersistenceManager.TaskRepository.Query<EntityDao>() where tt.Id == taskdeldetais.EntityID select tt).FirstOrDefault();
                    obj.action = "del check list";
                    obj.Actorid = proxy.MarcomManager.User.Id;
                    obj.EntityId = taskdeldetais.ID;
                    obj.AttributeName = delchkdetails.Name;
                    obj.AssociatedEntityId = entdeldetais.Id;
                    fs.AsynchronousNotify(obj);

                    tx.PersistenceManager.TaskRepository.DeleteByID<EntityTaskCheckListDao>(chkLstID);
                    tx.Commit();

                    return true;
                }
            }
            catch
            {

            }
            return false;
        }

        public IList<IEntitySublevelTaskHolder> GetSublevelTaskList(TaskManagerProxy proxy, int entityID)
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
                    foreach (var item in dao)
                    {
                        IEntitySublevelTaskHolder itsk = new EntitySublevelTaskHolder();
                        itsk.EntityID = item.Id;
                        itsk.EntityName = item.Name;
                        var entTypeDao = tx.PersistenceManager.TaskRepository.Query<EntityTypeDao>().Where(a => a.Id == item.Typeid).Select(a => a).FirstOrDefault();
                        itsk.EntityTypeColorCode = entTypeDao.ColorCode;
                        itsk.EntityTypeID = item.Typeid;
                        itsk.EntityTypeShortDescription = entTypeDao.ShortDescription;
                        itsk.EntityUniqueKey = item.UniqueKey;
                        itsk.SortOrder = item.EntityID;
                        itsk.TaskListGroup = GetSublevelEntityTaskList(proxy, item.Id);
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
        public IList<ITaskLibraryTemplateHolder> GetSublevelEntityTaskList(TaskManagerProxy proxy, int entityID)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    IList<ITaskLibraryTemplateHolder> tasklist = new List<ITaskLibraryTemplateHolder>();
                    var entityTaskList = tx.PersistenceManager.PlanningRepository.Query<EntityTaskListDao>().Where(a => a.EntityID == entityID).Select(a => a).OrderBy(a => a.Sortorder);
                    foreach (var val in entityTaskList)
                    {
                        ITaskLibraryTemplateHolder tskLst = new TaskLibraryTemplateHolder();
                        tskLst.LibraryName = HttpUtility.HtmlDecode(val.Name);
                        tskLst.LibraryDescription = HttpUtility.HtmlDecode(val.Description);
                        tskLst.ID = val.ID;
                        tskLst.SortOrder = val.Sortorder;
                        IList<IEntityTask> ientitytask = new List<IEntityTask>();
                        tskLst.TaskList = ientitytask;
                        tskLst.IsExpanded = false;
                        tskLst.IsGetTasks = false;
                        string newSortOrder = "SELECT COUNT(*) AS 'taskcount' FROM TM_EntityTask tet WHERE tet.EntityID= ? AND tet.TaskListID= ?";
                        IList sortOrderVal = tx.PersistenceManager.PlanningRepository.ExecuteQuerywithMinParam(newSortOrder, entityID, val.ID);
                        int sortOrderID = 0;
                        sortOrderID = (int)((System.Collections.Hashtable)(sortOrderVal)[0])["taskcount"];
                        tskLst.TaskCount = sortOrderID;
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
        public bool CopyAttachmentsfromtask(TaskManagerProxy proxy, int[] fileids, int taskid)
        {

            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {

                    var taskFileCollection = tx.PersistenceManager.TaskRepository.Query<FileDao>().Where(a => fileids.Contains(a.Id)).Select(a => a).ToList();

                    IList<FileDao> iifiledao = new List<FileDao>();
                    foreach (var CurrentFileDetl in taskFileCollection)
                    {
                        FileDao newdao = new FileDao();
                        var AdminActiveFlID = CurrentFileDetl.Id;
                        newdao.Id = 0;
                        newdao.Entityid = taskid;
                        string OldFilename = CurrentFileDetl.Fileguid + CurrentFileDetl.Extension;
                        Guid NewGuid = Guid.NewGuid();
                        newdao.Fileguid = NewGuid;
                        newdao.Extension = CurrentFileDetl.Extension;
                        newdao.Moduleid = CurrentFileDetl.Moduleid;
                        newdao.Name = CurrentFileDetl.Name;
                        newdao.Ownerid = CurrentFileDetl.Ownerid;
                        newdao.Size = CurrentFileDetl.Size;
                        newdao.VersionNo = CurrentFileDetl.VersionNo;
                        newdao.MimeType = CurrentFileDetl.MimeType;
                        newdao.CreatedOn = CurrentFileDetl.CreatedOn;
                        newdao.Description = CurrentFileDetl.Description;


                        string NewFilename = NewGuid + CurrentFileDetl.Extension;
                        string Filepath = Path.Combine(HttpRuntime.AppDomainAppPath);
                        Filepath += "UploadedImages\\";
                        System.IO.File.Copy(Filepath + OldFilename, Filepath + NewFilename);
                        tx.PersistenceManager.TaskRepository.Save<FileDao>(newdao);



                        var getTaskAttachDetails = tx.PersistenceManager.TaskRepository.Get<AttachmentsDao>(AttachmentsDao.PropertyNames.ActiveFileid, AdminActiveFlID);
                        if (getTaskAttachDetails != null)
                        {

                            AttachmentsDao Attachdao = new AttachmentsDao();
                            Attachdao.ActiveFileid = newdao.Id;
                            Attachdao.Typeid = getTaskAttachDetails.Typeid;
                            Attachdao.Entityid = taskid;
                            Attachdao.Name = getTaskAttachDetails.Name;
                            Attachdao.ActiveVersionNo = getTaskAttachDetails.ActiveVersionNo;
                            Attachdao.ActiveFileVersionID = newdao.Id;
                            Attachdao.VersioningFileId = newdao.Id;
                            Attachdao.Createdon = DateTime.Now;
                            tx.PersistenceManager.TaskRepository.Save<AttachmentsDao>(Attachdao);

                        }
                        FeedNotificationServer fs = new FeedNotificationServer();
                        NotificationFeedObjects obj = new NotificationFeedObjects();
                        obj.action = "attachment added";
                        obj.AttributeName = CurrentFileDetl.Name;
                        obj.attachmenttype = "file";
                        obj.Actorid = proxy.MarcomManager.User.Id;
                        obj.ToValue = CurrentFileDetl.Name;
                        obj.EntityId = taskid;
                        fs.AsynchronousNotify(obj);
                    }

                    tx.Commit();
                    return true;
                }

            }

            catch
            {

            }
            return false;


        }


        public bool CopyAttachmentsfromtask(TaskManagerProxy proxy, int[] fileids, int taskid, int[] linkids)
        {
            // return false;
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {

                    var tasklinksCollection = tx.PersistenceManager.TaskRepository.Query<LinksDao>().Where(a => linkids.Contains(a.ID)).Select(a => a).ToList();

                    IList<LinksDao> iifiledao1 = new List<LinksDao>();
                    foreach (var CurrentLinkDet in tasklinksCollection)
                    {
                        LinksDao linkdao = new LinksDao();
                        var AdminlinkID = CurrentLinkDet.ID;
                        linkdao.ID = 0;
                        linkdao.EntityID = taskid;
                        string OldFilename1 = CurrentLinkDet.LinkGuid + CurrentLinkDet.URL;
                        Guid NewGuid1 = Guid.NewGuid();
                        linkdao.LinkGuid = NewGuid1;
                        linkdao.URL = CurrentLinkDet.URL;
                        linkdao.ModuleID = CurrentLinkDet.ModuleID;
                        linkdao.Name = CurrentLinkDet.Name;
                        linkdao.OwnerID = CurrentLinkDet.OwnerID;
                        linkdao.TypeID = CurrentLinkDet.TypeID;
                        linkdao.CreatedOn = CurrentLinkDet.CreatedOn;
                        linkdao.Description = CurrentLinkDet.Description;
                        string NewLinkname1 = NewGuid1 + CurrentLinkDet.URL;
                        tx.PersistenceManager.TaskRepository.Save<LinksDao>(linkdao);

                    }

                    var taskFileCollection = tx.PersistenceManager.TaskRepository.Query<FileDao>().Where(a => fileids.Contains(a.Id)).Select(a => a).ToList();

                    IList<FileDao> iifiledao = new List<FileDao>();
                    foreach (var CurrentFileDetl in taskFileCollection)
                    {
                        FileDao newdao = new FileDao();
                        var AdminActiveFlID = CurrentFileDetl.Id;
                        newdao.Id = 0;
                        newdao.Entityid = taskid;
                        string OldFilename = CurrentFileDetl.Fileguid + CurrentFileDetl.Extension;
                        Guid NewGuid = Guid.NewGuid();
                        newdao.Fileguid = NewGuid;
                        newdao.Extension = CurrentFileDetl.Extension;
                        newdao.Moduleid = CurrentFileDetl.Moduleid;
                        newdao.Name = CurrentFileDetl.Name;
                        newdao.Ownerid = CurrentFileDetl.Ownerid;
                        newdao.Size = CurrentFileDetl.Size;
                        newdao.VersionNo = CurrentFileDetl.VersionNo;
                        newdao.MimeType = CurrentFileDetl.MimeType;
                        newdao.CreatedOn = CurrentFileDetl.CreatedOn;
                        newdao.Description = CurrentFileDetl.Description;

                        string NewFilename = NewGuid + CurrentFileDetl.Extension;
                        string Filepath = Path.Combine(HttpRuntime.AppDomainAppPath);
                        Filepath += "UploadedImages\\";
                        System.IO.File.Copy(Filepath + OldFilename, Filepath + NewFilename);
                        tx.PersistenceManager.TaskRepository.Save<FileDao>(newdao);

                        var getTaskAttachDetails1 = tx.PersistenceManager.TaskRepository.Get<AttachmentsDao>(AttachmentsDao.PropertyNames.ActiveFileid, AdminActiveFlID);
                        if (getTaskAttachDetails1 != null)
                        {

                            AttachmentsDao Attachdao = new AttachmentsDao();
                            Attachdao.ActiveFileid = newdao.Id;
                            Attachdao.Typeid = getTaskAttachDetails1.Typeid;
                            Attachdao.Entityid = taskid;
                            Attachdao.Name = getTaskAttachDetails1.Name;
                            Attachdao.ActiveVersionNo = getTaskAttachDetails1.ActiveVersionNo;
                            Attachdao.ActiveFileVersionID = newdao.Id;
                            Attachdao.VersioningFileId = newdao.Id;
                            Attachdao.Createdon = DateTime.Now;
                            tx.PersistenceManager.TaskRepository.Save<AttachmentsDao>(Attachdao);

                        }
                    }
                    tx.Commit();
                    return true;
                }

            }


            catch
            {

            }
            return false;


        }


        public bool copytogeneralattachment(TaskManagerProxy proxy, int[] fileids, int taskid)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {


                    var taskFileCollection = tx.PersistenceManager.TaskRepository.Query<FileDao>().Where(a => fileids.Contains(a.Id)).Select(a => a).ToList();


                    IList<FileDao> iifiledao = new List<FileDao>();
                    foreach (var CurrentFileDetl in taskFileCollection)
                    {
                        FileDao newdao = new FileDao();
                        var AdminActiveFlID = CurrentFileDetl.Id;
                        newdao.Id = 0;
                        newdao.Entityid = taskid;
                        string OldFilename = CurrentFileDetl.Fileguid + CurrentFileDetl.Extension;
                        Guid NewGuid = Guid.NewGuid();
                        newdao.Fileguid = NewGuid;
                        newdao.Extension = CurrentFileDetl.Extension;
                        newdao.Moduleid = CurrentFileDetl.Moduleid;
                        newdao.Name = CurrentFileDetl.Name;
                        newdao.Ownerid = CurrentFileDetl.Ownerid;
                        newdao.Size = CurrentFileDetl.Size;
                        newdao.VersionNo = CurrentFileDetl.VersionNo;
                        newdao.MimeType = CurrentFileDetl.MimeType;
                        newdao.CreatedOn = CurrentFileDetl.CreatedOn;


                        string NewFilename = NewGuid + CurrentFileDetl.Extension;
                        string Filepath = Path.Combine(HttpRuntime.AppDomainAppPath);
                        Filepath += "UploadedImages\\";
                        System.IO.File.Copy(Filepath + OldFilename, Filepath + NewFilename);
                        tx.PersistenceManager.TaskRepository.Save<FileDao>(newdao);




                        var getTaskAttachDetails = tx.PersistenceManager.TaskRepository.Get<AttachmentsDao>(AttachmentsDao.PropertyNames.ActiveFileid, AdminActiveFlID);
                        if (getTaskAttachDetails != null)
                        {

                            AttachmentsDao Attachdao = new AttachmentsDao();
                            Attachdao.ActiveFileid = newdao.Id;
                            Attachdao.Typeid = getTaskAttachDetails.Typeid;
                            Attachdao.Entityid = taskid;
                            Attachdao.Name = getTaskAttachDetails.Name;
                            Attachdao.ActiveVersionNo = getTaskAttachDetails.ActiveVersionNo;
                            Attachdao.ActiveFileVersionID = newdao.Id;
                            Attachdao.Createdon = DateTime.Now;
                            tx.PersistenceManager.TaskRepository.Save<AttachmentsDao>(Attachdao);

                        }
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


        public bool copytogeneralattachment(TaskManagerProxy proxy, int fileids, int taskid, int linkids)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {


                    var tasklinksCollection = tx.PersistenceManager.TaskRepository.Query<LinksDao>().Where(a => a.ID == linkids).Select(a => a);
                    IList<LinksDao> iifiledao1 = new List<LinksDao>();
                    foreach (var CurrentLinkDet in tasklinksCollection)
                    {
                        LinksDao linkdao = new LinksDao();
                        var AdminlinkID = CurrentLinkDet.ID;
                        linkdao.ID = 0;
                        linkdao.EntityID = taskid;
                        string OldFilename1 = CurrentLinkDet.LinkGuid + CurrentLinkDet.URL;
                        Guid NewGuid1 = Guid.NewGuid();
                        linkdao.LinkGuid = NewGuid1;
                        linkdao.URL = CurrentLinkDet.URL;
                        linkdao.ModuleID = CurrentLinkDet.ModuleID;
                        linkdao.Name = CurrentLinkDet.Name;
                        linkdao.OwnerID = CurrentLinkDet.OwnerID;
                        linkdao.TypeID = CurrentLinkDet.TypeID;
                        linkdao.CreatedOn = CurrentLinkDet.CreatedOn;


                        string NewLinkname1 = NewGuid1 + CurrentLinkDet.URL;
                        tx.PersistenceManager.TaskRepository.Save<LinksDao>(linkdao);

                    }

                    var taskFileCollection = tx.PersistenceManager.TaskRepository.Query<FileDao>().Where(a => a.Id == fileids).Select(a => a);
                    IList<FileDao> iifiledao = new List<FileDao>();
                    foreach (var CurrentFileDetl in taskFileCollection)
                    {
                        FileDao newdao = new FileDao();
                        var AdminActiveFlID = CurrentFileDetl.Id;
                        newdao.Id = 0;
                        newdao.Entityid = taskid;
                        string OldFilename = CurrentFileDetl.Fileguid + CurrentFileDetl.Extension;
                        Guid NewGuid = Guid.NewGuid();
                        newdao.Fileguid = NewGuid;
                        newdao.Extension = CurrentFileDetl.Extension;
                        newdao.Moduleid = CurrentFileDetl.Moduleid;
                        newdao.Name = CurrentFileDetl.Name;
                        newdao.Ownerid = CurrentFileDetl.Ownerid;
                        newdao.Size = CurrentFileDetl.Size;
                        newdao.VersionNo = CurrentFileDetl.VersionNo;
                        newdao.MimeType = CurrentFileDetl.MimeType;
                        newdao.CreatedOn = CurrentFileDetl.CreatedOn;


                        string NewFilename = NewGuid + CurrentFileDetl.Extension;
                        string Filepath = Path.Combine(HttpRuntime.AppDomainAppPath);
                        Filepath += "UploadedImages\\";
                        System.IO.File.Copy(Filepath + OldFilename, Filepath + NewFilename);
                        tx.PersistenceManager.TaskRepository.Save<FileDao>(newdao);



                        var getTaskAttachDetails1 = tx.PersistenceManager.TaskRepository.Get<AttachmentsDao>(AttachmentsDao.PropertyNames.ActiveFileid, AdminActiveFlID);
                        if (getTaskAttachDetails1 != null)
                        {

                            AttachmentsDao Attachdao = new AttachmentsDao();
                            Attachdao.ActiveFileid = newdao.Id;
                            Attachdao.Typeid = getTaskAttachDetails1.Typeid;
                            Attachdao.Entityid = taskid;
                            Attachdao.Name = getTaskAttachDetails1.Name;
                            Attachdao.ActiveVersionNo = getTaskAttachDetails1.ActiveVersionNo;
                            Attachdao.ActiveFileVersionID = newdao.Id;
                            Attachdao.Createdon = DateTime.Now;
                            tx.PersistenceManager.TaskRepository.Save<AttachmentsDao>(Attachdao);

                        }
                    }
                    tx.Commit();
                    return true;
                }

            }


            catch
            {

            }
            return false;


        }


        public bool DeleteFileOrLinkById(TaskManagerProxy proxy, int fileids, int taskid, int linkids)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {


                    var tasklinksCollection = tx.PersistenceManager.TaskRepository.Query<LinksDao>().Where(a => a.ID == linkids).Select(a => a);
                    IList<LinksDao> iifiledao1 = new List<LinksDao>();
                    foreach (var CurrentLinkDet in tasklinksCollection)
                    {
                        LinksDao linkdao = new LinksDao();
                        var AdminlinkID = CurrentLinkDet.ID;
                        linkdao.ID = 0;
                        linkdao.EntityID = taskid;
                        string OldFilename1 = CurrentLinkDet.LinkGuid + CurrentLinkDet.URL;
                        Guid NewGuid1 = Guid.NewGuid();
                        linkdao.LinkGuid = NewGuid1;
                        linkdao.URL = CurrentLinkDet.URL;
                        linkdao.ModuleID = CurrentLinkDet.ModuleID;
                        linkdao.Name = CurrentLinkDet.Name;
                        linkdao.OwnerID = CurrentLinkDet.OwnerID;
                        linkdao.TypeID = CurrentLinkDet.TypeID;
                        linkdao.CreatedOn = CurrentLinkDet.CreatedOn;


                        string NewLinkname1 = NewGuid1 + CurrentLinkDet.URL;
                        tx.PersistenceManager.TaskRepository.Delete<LinksDao>(linkdao);

                    }



                    var taskFileCollection = tx.PersistenceManager.TaskRepository.Query<FileDao>().Where(a => a.Id == fileids).Select(a => a);

                    IList<FileDao> iifiledao = new List<FileDao>();
                    foreach (var CurrentFileDetl in taskFileCollection)
                    {
                        FileDao newdao = new FileDao();
                        var AdminActiveFlID = CurrentFileDetl.Id;
                        newdao.Id = 0;
                        newdao.Entityid = taskid;
                        string OldFilename = CurrentFileDetl.Fileguid + CurrentFileDetl.Extension;
                        Guid NewGuid = Guid.NewGuid();
                        newdao.Fileguid = NewGuid;
                        newdao.Extension = CurrentFileDetl.Extension;
                        newdao.Moduleid = CurrentFileDetl.Moduleid;
                        newdao.Name = CurrentFileDetl.Name;
                        newdao.Ownerid = CurrentFileDetl.Ownerid;
                        newdao.Size = CurrentFileDetl.Size;
                        newdao.VersionNo = CurrentFileDetl.VersionNo;
                        newdao.MimeType = CurrentFileDetl.MimeType;
                        newdao.CreatedOn = CurrentFileDetl.CreatedOn;


                        string NewFilename = NewGuid + CurrentFileDetl.Extension;
                        string Filepath = Path.Combine(HttpRuntime.AppDomainAppPath);
                        Filepath += "UploadedImages\\";
                        System.IO.File.Copy(Filepath + OldFilename, Filepath + NewFilename);
                        tx.PersistenceManager.TaskRepository.Delete<FileDao>(newdao);



                        var getTaskAttachDetails1 = tx.PersistenceManager.TaskRepository.Get<AttachmentsDao>(AttachmentsDao.PropertyNames.ActiveFileid, AdminActiveFlID);
                        if (getTaskAttachDetails1 != null)
                        {

                            AttachmentsDao Attachdao = new AttachmentsDao();
                            Attachdao.ActiveFileid = newdao.Id;
                            Attachdao.Typeid = getTaskAttachDetails1.Typeid;
                            Attachdao.Entityid = taskid;
                            Attachdao.Name = getTaskAttachDetails1.Name;
                            Attachdao.ActiveVersionNo = getTaskAttachDetails1.ActiveVersionNo;
                            Attachdao.ActiveFileVersionID = newdao.Id;
                            Attachdao.Createdon = DateTime.Now;
                            tx.PersistenceManager.TaskRepository.Delete<AttachmentsDao>(Attachdao);

                        }
                    }
                    tx.Commit();
                    return true;
                }

            }


            catch
            {

            }
            return false;

        }




        public bool CopyAttachmentsfromtaskToExistingTasks(TaskManagerProxy proxy, int[] TaskIDs, int FileID, string filetype)
        {
            // return false;
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    FileDao taskFileCollection = new FileDao();
                    LinksDao taskLinkCollection = new LinksDao();
                    if (filetype != "Link")
                    {
                        taskFileCollection = tx.PersistenceManager.TaskRepository.Query<FileDao>().Where(a => a.Id == FileID).Select(a => a).FirstOrDefault();
                    }
                    else
                    {
                        taskLinkCollection = tx.PersistenceManager.TaskRepository.Query<LinksDao>().Where(a => a.ID == FileID).Select(a => a).FirstOrDefault();
                    }

                    IList<FileDao> iifiledao = new List<FileDao>();
                    IList<AttachmentsDao> iiattach = new List<AttachmentsDao>();
                    IList<LinksDao> iilinks = new List<LinksDao>();
                    IList<TaskMembersDao> itaskMemDao = new List<TaskMembersDao>();

                    foreach (var CurrentFileDetl in TaskIDs)
                    {
                        if (filetype != "Link")
                        {

                            FileDao newdao = new FileDao();

                            newdao.Entityid = taskFileCollection.Entityid;
                            newdao.Id = taskFileCollection.Id;
                            newdao.Entityid = CurrentFileDetl;
                            string OldFilename = taskFileCollection.Fileguid + taskFileCollection.Extension;
                            Guid NewGuid = Guid.NewGuid();
                            newdao.Fileguid = NewGuid;
                            newdao.Extension = taskFileCollection.Extension;
                            newdao.Moduleid = taskFileCollection.Moduleid;
                            newdao.Name = taskFileCollection.Name;
                            newdao.Ownerid = taskFileCollection.Ownerid;
                            newdao.Size = taskFileCollection.Size;
                            newdao.VersionNo = taskFileCollection.VersionNo;
                            newdao.MimeType = taskFileCollection.MimeType;
                            newdao.CreatedOn = taskFileCollection.CreatedOn;


                            string NewFilename = NewGuid + taskFileCollection.Extension;
                            string Filepath = Path.Combine(HttpRuntime.AppDomainAppPath);
                            Filepath += "UploadedImages\\";
                            System.IO.File.Copy(Filepath + OldFilename, Filepath + NewFilename);

                            iifiledao.Add(newdao);

                            AttachmentsDao Attachdao = new AttachmentsDao();
                            Attachdao.ActiveFileid = newdao.Id;
                            Attachdao.Typeid = 0;
                            Attachdao.Entityid = newdao.Entityid;
                            Attachdao.Name = newdao.Name;
                            Attachdao.ActiveVersionNo = 1;
                            Attachdao.ActiveFileVersionID = newdao.Id;
                            Attachdao.Createdon = DateTime.Now;
                            iiattach.Add(Attachdao);

                            FeedNotificationServer fs = new FeedNotificationServer();
                            NotificationFeedObjects obj = new NotificationFeedObjects();
                            obj.action = "attachment added";
                            obj.AttributeName = newdao.Name;

                            obj.Actorid = proxy.MarcomManager.User.Id;
                            obj.attachmenttype = "file";
                            obj.ToValue = Convert.ToString(newdao.Id);
                            obj.EntityId = newdao.Entityid;
                            fs.AsynchronousNotify(obj);
                        }
                        else
                        {
                            LinksDao lnewdao = new LinksDao();
                            Guid NewGuid = Guid.NewGuid();
                            lnewdao.LinkGuid = NewGuid;
                            lnewdao.Description = taskLinkCollection.Description;
                            lnewdao.ModuleID = taskLinkCollection.ModuleID;
                            lnewdao.Name = taskLinkCollection.Name;
                            lnewdao.OwnerID = taskLinkCollection.OwnerID;
                            lnewdao.ActiveVersionNo = taskLinkCollection.ActiveVersionNo;
                            lnewdao.EntityID = CurrentFileDetl;
                            lnewdao.URL = taskLinkCollection.URL;
                            lnewdao.CreatedOn = taskLinkCollection.CreatedOn;

                            iilinks.Add(lnewdao);

                            FeedNotificationServer fs = new FeedNotificationServer();
                            NotificationFeedObjects obj = new NotificationFeedObjects();
                            obj.action = "attachment added";
                            obj.AttributeName = lnewdao.Name;
                            obj.Actorid = proxy.MarcomManager.User.Id;
                            obj.attachmenttype = "link";
                            obj.ToValue = Convert.ToString(lnewdao.ID);
                            obj.EntityId = lnewdao.EntityID;
                            fs.AsynchronousNotify(obj);
                        }

                        //Task Reinitialize concept for Approval task
                        EntityTaskDao entityTask = new EntityTaskDao();

                        var daoResult = (from item in tx.PersistenceManager.TaskRepository.Query<EntityTaskDao>() where item.ID == CurrentFileDetl select item).ToList();
                        if (daoResult.Count() > 0)
                        {
                            entityTask = daoResult.FirstOrDefault();
                            TaskMembersDao memdao = new TaskMembersDao();
                            if (entityTask != null)
                            {
                                if (entityTask.TaskType != 2)
                                {
                                    if (entityTask.TaskStatus == (int)TaskStatus.Rejected || entityTask.TaskStatus == (int)TaskStatus.Unable_to_complete || entityTask.TaskStatus == (int)TaskStatus.Approved)
                                    {

                                        entityTask.TaskStatus = (int)TaskStatus.In_progress;
                                        tx.PersistenceManager.TaskRepository.Save<EntityTaskDao>(entityTask);
                                    }

                                    var totalTaskmembers = tx.PersistenceManager.PlanningRepository.Query<TaskMembersDao>().Where(a => a.TaskID == CurrentFileDetl && a.RoleID != 1).ToList();
                                    if (totalTaskmembers != null)
                                    {
                                        foreach (var mem in totalTaskmembers)
                                        {
                                            mem.ApprovalStatus = null;
                                            itaskMemDao.Add(mem);
                                        }

                                    }
                                }
                            }
                        }
                    }


                    if (iiattach.Count() > 0)
                        tx.PersistenceManager.TaskRepository.Save<AttachmentsDao>(iiattach);
                    if (itaskMemDao.Count() > 0)
                        tx.PersistenceManager.TaskRepository.Save<TaskMembersDao>(itaskMemDao);
                    if (iilinks.Count() > 0)
                        tx.PersistenceManager.TaskRepository.Save<LinksDao>(iilinks);
                    tx.Commit();
                    return true;
                }

            }

            catch
            {

            }
            return false;


        }

        public bool EnableDisable(TaskManagerProxy proxy, bool status)
        {
            try
            {

                string xmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
                string xelementName = "TaskSettings";
                var xelementFilepath = XElement.Load(xmlpath);
                var xmlElement = xelementFilepath.Element(xelementName);
                foreach (var des in xmlElement.Descendants())
                {
                    if (des.Name.ToString() == "taskflagsetting")
                    {
                        des.Value = Convert.ToString(status);
                    }
                }
                xelementFilepath.Save(xmlpath);

                return true;

            }
            catch (Exception ex)
            {
                return false;
            }

        }
        public bool gettaskflagstatus(TaskManagerProxy proxy)
        {
            try
            {
                bool staus = false;
                string xmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
                string xelementName = "TaskSettings";
                var xelementFilepath = XElement.Load(xmlpath);
                var xmlElement = xelementFilepath.Element(xelementName);
                foreach (var des in xmlElement.Descendants())
                {
                    if (des.Name.ToString() == "taskflagsetting")
                    {
                        staus = Convert.ToBoolean(des.Value);
                    }
                }
                return staus;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        /// <summary>
        /// DeleteLinkByID.
        /// </summary>
        /// <param name="proxy">ID Parameter</param>
        /// <returns>bool</returns>
        public bool DeleteAdminTaskLinkByID(TaskManagerProxy proxy, int ID)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    tx.PersistenceManager.TaskRepository.DeleteByID<TaskLinksDao>(TaskLinksDao.MappingNames.ID, ID);
                    tx.Commit();
                    return true;
                }
            }
            catch
            {

            }
            return false;
        }

        public IList<ITaskLibraryTemplateHolder> GetEntityTaskListWithoutTasks(TaskManagerProxy proxy, int entityID)
        {

            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    IList<ITaskLibraryTemplateHolder> tasklist = new List<ITaskLibraryTemplateHolder>();
                    var entityTaskList = tx.PersistenceManager.PlanningRepository.Query<EntityTaskListDao>().Where(a => a.EntityID == entityID).Select(a => a);
                    foreach (var val in entityTaskList)
                    {
                        //var entityTasks = tx.PersistenceManager.PlanningRepository.Query<EntityTaskDao>().Where(a => a.EntityID == entityID && a.TaskListID == val.TaskListID).Select(a => a);
                        ITaskLibraryTemplateHolder tskLst = new TaskLibraryTemplateHolder();
                        tskLst.EntityParentID = entityID;
                        tskLst.LibraryName = HttpUtility.HtmlDecode(val.Name);
                        tskLst.LibraryDescription = HttpUtility.HtmlDecode(val.Description);
                        tskLst.ID = val.ID;
                        tskLst.SortOrder = val.Sortorder;
                        IList<IEntityTask> ientitytsk = new List<IEntityTask>();
                        tskLst.TaskList = ientitytsk;
                        string newSortOrder = "SELECT COUNT(*) AS 'taskcount' FROM TM_EntityTask tet WHERE tet.EntityID= ? AND tet.TaskListID= ?";
                        IList sortOrderVal = tx.PersistenceManager.PlanningRepository.ExecuteQuerywithMinParam(newSortOrder, entityID, val.ID);
                        int sortOrderID = 0;
                        sortOrderID = (int)((System.Collections.Hashtable)(sortOrderVal)[0])["taskcount"];
                        tskLst.TaskCount = sortOrderID;
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

        public int GettaskCountByStatus(TaskManagerProxy proxy, int tasklistID, int EntityID, int[] status)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    List<int> statusColl = status.Select(x => x).ToList();

                    if (statusColl.Contains(2) || statusColl.Contains(3))
                    {
                        statusColl.Add(8);
                    }
                    var taskount = 0;
                    if (status.Count() != 0)
                        taskount = tx.PersistenceManager.TaskRepository.Query<EntityTaskDao>().Where(a => a.EntityID == EntityID && a.TaskListID == tasklistID && statusColl.Contains(a.TaskStatus)).Count();
                    else
                        taskount = tx.PersistenceManager.TaskRepository.Query<EntityTaskDao>().Where(a => a.EntityID == EntityID && a.TaskListID == tasklistID).Count();
                    return taskount;

                }
            }
            catch
            {

            }
            return 0;
        }

        public bool UpdatetaskAdminLinkDescription(TaskManagerProxy proxy, int id, string Name, string description, string linkURL, int LinkType)
        {

            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    TaskLinksDao fileDao = new TaskLinksDao();
                    fileDao = tx.PersistenceManager.PlanningRepository.Query<TaskLinksDao>().Where(a => a.ID == id).Select(a => a).FirstOrDefault();
                    fileDao.Name = Name;
                    fileDao.URL = linkURL;
                    fileDao.Description = description;
                    fileDao.LinkType = LinkType;
                    tx.PersistenceManager.TaskRepository.Save<TaskLinksDao>(fileDao);
                    tx.Commit();
                    return true;
                }

            }
            catch
            {
                return false;
            }
        }




        public bool UpdatetaskAdminAttachmentDescription(TaskManagerProxy proxy, int id, string friendlyName, string description)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    NewTaskAttachmentsDao attachDao = new NewTaskAttachmentsDao();
                    attachDao = tx.PersistenceManager.PlanningRepository.Query<NewTaskAttachmentsDao>().Where(a => a.ActiveFileid == id).Select(a => a).FirstOrDefault();
                    attachDao.Name = (friendlyName.Trim().Length > 0 ? friendlyName : attachDao.Name);
                    tx.PersistenceManager.TaskRepository.Save<NewTaskAttachmentsDao>(attachDao);
                    TaskFileDao fileDao = new TaskFileDao();
                    fileDao = tx.PersistenceManager.PlanningRepository.Query<TaskFileDao>().Where(a => a.Id == id).Select(a => a).FirstOrDefault();
                    fileDao.Name = attachDao.Name;
                    fileDao.Description = (description.Trim().Length > 0 ? description : fileDao.Description);
                    tx.PersistenceManager.TaskRepository.Save<TaskFileDao>(fileDao);
                    tx.Commit();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public IList<ITaskLibraryTemplateHolder> GetExistingEntityTasksByEntityID(TaskManagerProxy proxy, int entityID)
        {

            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    IList<ITaskLibraryTemplateHolder> tasklist = new List<ITaskLibraryTemplateHolder>();
                    var entityTaskList = tx.PersistenceManager.PlanningRepository.Query<EntityTaskListDao>().Where(a => a.EntityID == entityID).Select(a => a).OrderBy(a => a.Sortorder);
                    List<int> idarr = entityTaskList.Select(a => a.ID).ToList();
                    foreach (var currentList in entityTaskList)
                    {
                        ITaskLibraryTemplateHolder tskLst = new TaskLibraryTemplateHolder();
                        tskLst.LibraryName = HttpUtility.HtmlEncode(currentList.Name);
                        tskLst.LibraryDescription = HttpUtility.HtmlEncode(currentList.Description);
                        tskLst.ID = currentList.ID;
                        tskLst.SortOrder = currentList.Sortorder;
                        tskLst.TaskList = GetEntityTaskBasicDetails(proxy, currentList.EntityID, currentList.ID);
                        tasklist.Add(tskLst);
                    } //close lambda expression


                    return tasklist;
                }
            }
            catch
            {
                return null;
            }

        }

        /// <summary>
        /// Getting Task details
        /// </summary>
        ///  <param name="proxy"></param>
        /// <param name="StepID">The TaskListID</param>
        /// <returns>IList of IAdminTask</returns>
        public IList<IEntityTask> GetEntityTaskBasicDetails(TaskManagerProxy proxy, int entityID, int taskListID)
        {
            IList<IEntityTask> iitaskDetails = new List<IEntityTask>();
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    var TaskList = (from task in tx.PersistenceManager.PlanningRepository.Query<EntityTaskDao>() where task.EntityID == entityID && task.TaskListID == taskListID select task).OrderBy(a => a.Sortorder).ToList();
                    foreach (var val in TaskList)
                    {

                        IEntityTask admintsk = new EntityTask();
                        admintsk.Name = HttpUtility.HtmlEncode(val.Name);
                        admintsk.Description = val.Description != null ? HttpUtility.HtmlEncode(val.Description) : "-";
                        admintsk.Note = val.Note != null ? HttpUtility.HtmlEncode(val.Note) : "-";
                        admintsk.Id = val.ID;
                        admintsk.TaskTypeName = Enum.GetName(typeof(TaskTypes), val.TaskType).Replace("_", " ");
                        admintsk.TaskType = val.TaskType;
                        admintsk.TaskListID = val.TaskListID;
                        admintsk.EntityID = val.EntityID;
                        admintsk.TaskStatus = val.TaskStatus;
                        admintsk.SortOrder = val.Sortorder;
                        iitaskDetails.Add(admintsk);
                    }

                }
                return iitaskDetails;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public IList GetMyFundingRequests(TaskManagerProxy proxy, int[] FilterStatusID, int AssignRole)
        {
            try
            {
                IList<MultiProperty> parLIST = new List<MultiProperty>();
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {

                    StringBuilder MytaskQry = new StringBuilder();

                    parLIST.Add(new MultiProperty { propertyName = "User_Id", propertyValue = proxy.MarcomManager.User.Id });
                    parLIST.Add(new MultiProperty { propertyName = "AssignRole", propertyValue = AssignRole });

                    MytaskQry.Append(" SELECT tet.ID 'TaskId', ");
                    MytaskQry.Append("       tet.Name 'TaskName', ");
                    MytaskQry.Append("      tet.[Description] 'TaskDescription', ");
                    MytaskQry.Append("      pe.ID 'EntityId', ");
                    MytaskQry.Append(" ( ");
                    MytaskQry.Append(" SELECT TOP 1 pe2.Name FROM PM_Entity pe2 ");
                    MytaskQry.Append("   WHERE  pe2.id = tet.EntityId ");
                    MytaskQry.Append(" ) 'EntityName', ");
                    MytaskQry.Append("      ttm.RoleID 'RoleId', ");
                    MytaskQry.Append("       ISNULL(CONVERT(VARCHAR(10), tet.DueDate, 111), '') 'DueDate', ");
                    MytaskQry.Append("       ISNULL(DATEDIFF(d, GETDATE(), tet.DueDate), 0) 'Noofdays', ");
                    MytaskQry.Append("       CASE  ");
                    MytaskQry.Append("            WHEN (DATEDIFF(d, GETDATE(), tet.DueDate) < 0) THEN 0 ");
                    MytaskQry.Append("            WHEN (DATEDIFF(d, GETDATE(), tet.DueDate) < 7) THEN 1 ");
                    MytaskQry.Append("           WHEN (DATEDIFF(d, GETDATE(), tet.DueDate) < 0) THEN 2 ");
                    MytaskQry.Append("           ELSE 3 ");
                    MytaskQry.Append("       END 'Weeks', ");
                    MytaskQry.Append("      ( ");
                    MytaskQry.Append("           SELECT TOP 1 uu2.FirstName + ' ' + uu2.LastName ");
                    MytaskQry.Append("           FROM   UM_User uu2 ");
                    MytaskQry.Append("           WHERE  uu2.ID = ( ");
                    MytaskQry.Append("                      SELECT pfr.RequestedBy ");
                    MytaskQry.Append("                     FROM   PM_FundingRequest pfr ");
                    MytaskQry.Append("                      WHERE  id = tet.ID ");
                    MytaskQry.Append("                  ) ");
                    MytaskQry.Append("       ) 'RequestUserName', ");
                    MytaskQry.Append("       ( ");
                    MytaskQry.Append("           SELECT TOP 1 pfr.RequestedBy ");
                    MytaskQry.Append("           FROM   PM_FundingRequest pfr ");
                    MytaskQry.Append("           WHERE  id = tet.ID ");
                    MytaskQry.Append("       ) 'RequestUserID', ");
                    MytaskQry.Append("       ( ");
                    MytaskQry.Append("          SELECT TOP 1  attr_20 ");
                    MytaskQry.Append("           FROM   MM_AttributeRecord_7     mar ");
                    MytaskQry.Append("           WHERE  mar.id = tet.ID ");
                    MytaskQry.Append("       ) 'RequestAmount', ");
                    MytaskQry.Append("       ( ");
                    MytaskQry.Append("           SELECT TOP 1 pe2.Name ");
                    MytaskQry.Append("           FROM   PM_Entity pe2 ");
                    MytaskQry.Append("           WHERE  pe2.ID = ( ");
                    MytaskQry.Append("                      SELECT pfr.CostCenterID ");
                    MytaskQry.Append("                      FROM   PM_FundingRequest pfr ");
                    MytaskQry.Append("                      WHERE  id = tet.ID ");
                    MytaskQry.Append("                  ) ");
                    MytaskQry.Append("       ) 'CCNAME', ");
                    MytaskQry.Append("       ( ");
                    MytaskQry.Append("           SELECT ISNULL(CONVERT(VARCHAR(10), pfr.LastUpdatedOn, 111), '') ");
                    MytaskQry.Append("          FROM   PM_FundingRequest pfr ");
                    MytaskQry.Append("           WHERE  id = tet.ID ");
                    MytaskQry.Append("       ) 'RequestDate', ");
                    MytaskQry.Append("       ( ");
                    MytaskQry.Append("           SELECT TOP 1 uu2.FirstName + ' ' + uu2.LastName ");
                    MytaskQry.Append("          FROM   UM_User uu2 ");
                    MytaskQry.Append("                  INNER  JOIN AM_Entity_Role_User aeru ");
                    MytaskQry.Append("                      ON  uu2.ID = aeru.UserID INNER JOIN AM_EntityTypeRoleAcl aetra ON aetra.ID = aeru.RoleID ");
                    MytaskQry.Append("                       AND aeru.EntityID = ( ");
                    MytaskQry.Append("                               SELECT TOP 1 pfr.CostCenterID ");
                    MytaskQry.Append("                               FROM   PM_FundingRequest pfr ");
                    MytaskQry.Append("                               WHERE  id = tet.ID ");
                    MytaskQry.Append("                           ) ");
                    MytaskQry.Append("                       AND  aetra.EntityRoleID = 1 ");
                    MytaskQry.Append("       ) 'CCOWNERNAME', ");
                    MytaskQry.Append("       tet.TaskStatus 'TaskStatus', ");
                    MytaskQry.Append("       7 'TaskType', ");
                    MytaskQry.Append("  (  ");
                    MytaskQry.Append(" SELECT met.ShortDescription ");
                    MytaskQry.Append("  FROM   MM_EntityType met ");
                    MytaskQry.Append("      WHERE  id = ( ");
                    MytaskQry.Append("              SELECT TOP 1         typeid  ");
                    MytaskQry.Append("              FROM   PM_Entity     pe2  ");
                    MytaskQry.Append("              WHERE  pe2.ID = tet.EntityId  ");
                    MytaskQry.Append("             )  ");
                    MytaskQry.Append("    ) 'EntityType', ");
                    MytaskQry.Append("    ( ");
                    MytaskQry.Append("       SELECT met.ColorCode ");
                    MytaskQry.Append("      FROM   MM_EntityType met ");
                    MytaskQry.Append("      WHERE  id = ( ");
                    MytaskQry.Append("                SELECT TOP 1         typeid ");
                    MytaskQry.Append("                FROM   PM_Entity     pe2 ");
                    MytaskQry.Append("               WHERE  pe2.ID = tet.EntityId ");
                    MytaskQry.Append("          ) ");
                    MytaskQry.Append("    ) 'EntityTypeCC', ");
                    MytaskQry.Append("       pe.UniqueKey ");
                    MytaskQry.Append("FROM   PM_Entity pe ");
                    MytaskQry.Append("       INNER JOIN PM_Task tet ");
                    MytaskQry.Append("            ON  pe.ID = tet.ID ");
                    MytaskQry.Append("            AND pe.[Active] = 1 ");
                    MytaskQry.Append("            AND pe.TypeID = 7 ");
                    MytaskQry.Append("       INNER JOIN PM_Task_Members ttm ");
                    MytaskQry.Append("            ON  tet.ID = ttm.TaskID ");
                    MytaskQry.Append("       INNER JOIN UM_User uu ");
                    MytaskQry.Append("            ON  ttm.UserID = uu.ID ");
                    MytaskQry.Append("            AND ttm.UserID = :User_Id ");
                    MytaskQry.Append("            AND ttm.roleid = :AssignRole "); //FundRequest Raised to me = 4 and FundRequest Raised by me =1

                    if (FilterStatusID.Length > 0)
                    {
                        string inClause = "("
                                         + String.Join(",", FilterStatusID.Select(x => x.ToString()).ToArray())
                                       + ")";
                        MytaskQry.Append(" AND tet.TaskStatus in" + inClause + "");
                    }
                    else
                    {
                        MytaskQry.Append("AND tet.TaskStatus IN (8,11,12) ");
                    }

                    MytaskQry.Append(" ORDER BY pe.UniqueKey,tet.TaskStatus,DATEDIFF(d,GETDATE(),tet.DueDate) ");

                    IList MyTaskResult = tx.PersistenceManager.PlanningRepository.ExecuteQuerywithParam(MytaskQry.ToString(), parLIST);

                    return MyTaskResult;
                }
            }
            catch
            {
                return null;
            }

        }

        public IList FetchUnassignedTaskforReassign(TaskManagerProxy proxy, int entityId, int taskListId)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    IList unassignedTasks = tx.PersistenceManager.TaskRepository.ExecuteQuerywithMinParam("SELECT * FROM TM_EntityTask tet WHERE tet.TaskStatus=0 AND tet.EntityID = ? AND tet.TaskListID = ?", entityId, taskListId);
                    return unassignedTasks;
                }

            }
            catch
            {
                throw null;
            }
        }


        /// <summary>
        /// Getting Task details
        /// </summary>
        ///  <param name="proxy"></param>
        /// <param name="StepID">The TaskListID</param>
        /// <returns>IList of IAdminTask</returns>
        public IList<IEntityTask> GetReassignedEntityTaskListDetails(TaskManagerProxy proxy, int[] taskCollection, int entityId, int taskListId)
        {
            IList<IEntityTask> iitaskDetails = new List<IEntityTask>();
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    var TaskList = (from task in tx.PersistenceManager.PlanningRepository.Query<EntityTaskDao>() where taskCollection.Contains(task.ID) && task.EntityID == entityId && task.TaskListID == taskListId select task).OrderBy(a => a.Sortorder).ToList();

                    object _lock = new object();
                    Parallel.ForEach(TaskList, val =>
                    {
                        IEntityTask admintsk = new EntityTask();
                        admintsk.Name = HttpUtility.HtmlEncode(val.Name);
                        var basedao = (from task in tx.PersistenceManager.PlanningRepository.Query<BaseEntityDao>() where task.Id == val.ID select task).FirstOrDefault();
                        admintsk.Description = val.Description != null ? HttpUtility.HtmlEncode(val.Description) : "-";
                        admintsk.Id = val.ID;
                        admintsk.TaskTypeName = Enum.GetName(typeof(TaskTypes), val.TaskType).Replace("_", " ");
                        admintsk.TaskType = val.TaskType;
                        admintsk.TaskListID = val.TaskListID;
                        admintsk.EntityID = val.EntityID;
                        admintsk.TaskStatus = val.TaskStatus;
                        admintsk.EntityTaskListID = val.EntityTaskListID;
                        admintsk.Note = val.Note != null ? HttpUtility.HtmlEncode(val.Note) : "-";
                        if (val.TaskStatus != 8)
                            admintsk.StatusName = Enum.GetName(typeof(TaskStatus), val.TaskStatus).Replace("_", " ");
                        else
                            admintsk.StatusName = val.TaskType == 3 ? "Approved" : "Completed";
                        admintsk.SortOrder = val.Sortorder;
                        admintsk.strDate = "";
                        admintsk.TaskCheckList = null;
                        admintsk.TaskTypeID = basedao.Typeid;
                        admintsk.DueDate = null;
                        if (val.DueDate != null)
                        {
                            admintsk.DueDate = val.DueDate.Value;
                            if (admintsk.DueDate.Value.Date == DateTime.Now.Date)
                                admintsk.totalDueDays = 0;
                            else
                            {
                                TimeSpan tm = val.DueDate.Value.Date - DateTime.Now.Date;
                                admintsk.totalDueDays = (tm.Days);
                            }
                            admintsk.strDate = val.DueDate.Value.ToString("yyyy-MM-dd");
                        }
                        var taskmembers = GetTaskMember(proxy, val.ID);
                        var taskAssigneesList = taskmembers.Where(a => a.RoleID != 1).Select(a => a).ToList();
                        var taskchecklistCount = tx.PersistenceManager.TaskRepository.Query<EntityTaskCheckListDao>().Where(a => a.TaskId == val.ID).ToList();
                        if (taskmembers.Count() > 0)
                        {

                            admintsk.taskMembers = taskmembers;
                            if (taskAssigneesList.Count() > 0)
                            {
                                var totalTaskmembers = taskmembers.Where(a => a.TaskID == val.ID && a.RoleID != 1).ToList();
                                var currentTaskRount = totalTaskmembers.Select(a => a.ApprovalRount).First();
                                if (admintsk.TaskType != (int)TaskTypes.Work_Task)//if (admintsk.TaskType != (int)TaskTypes.Work_Task && admintsk.TaskStatus != (int)TaskStatus.Unassigned)
                                {
                                    admintsk.TotaltaskAssigness = taskmembers.Where(a => a.RoleID != 1).Select(a => a).ToList();
                                }
                                admintsk.taskAssigness = taskAssigneesList.Where(a => a.ApprovalRount == currentTaskRount);
                                admintsk.ProgressCount = "";
                                if (val.EntityTaskListID == 0)
                                {
                                    admintsk.taskAssigness = taskAssigneesList.Where(a => a.ApprovalRount == currentTaskRount);
                                    var responsedMembers = taskAssigneesList.Where(a => a.ApprovalRount == currentTaskRount && a.ApprovalStatus != null).Select(a => a).ToList().Count();
                                    if (admintsk.TaskType != (int)TaskTypes.Work_Task && (admintsk.TaskStatus != (int)TaskStatus.Unassigned && admintsk.TaskStatus != (int)TaskStatus.Approved && admintsk.TaskStatus != (int)TaskStatus.Completed))
                                    {
                                        if (val.TaskStatus == 2 || val.TaskStatus == 3 || val.TaskStatus == 4)
                                        {
                                            admintsk.ProgressCount = "";
                                        }
                                        else
                                        {
                                            admintsk.ProgressCount = "(" + responsedMembers.ToString() + "/" + taskAssigneesList.Where(a => a.ApprovalRount == currentTaskRount).Count().ToString() + ")";
                                        }
                                    }
                                    else
                                    {

                                        if (taskchecklistCount.Count > 0 && admintsk.TaskStatus == (int)TaskStatus.In_progress)
                                        {
                                            var completedChecklists = taskchecklistCount.Where(a => a.Status == true).ToList().Count();
                                            admintsk.ProgressCount = "(" + completedChecklists.ToString() + "/" + taskchecklistCount.Count().ToString() + ")";

                                        }
                                        else
                                        {
                                            admintsk.ProgressCount = "";
                                        }
                                    }
                                    if (taskAssigneesList.Count > 1)
                                    {
                                        var userIdArr = taskAssigneesList.Where(a => a.RoleID != 1 && a.ApprovalRount == currentTaskRount).Select(a => a.UserID).ToList();
                                        var assigneeNameObj = tx.PersistenceManager.UserRepository.Query<BrandSystems.Marcom.Dal.User.Model.UserDao>().Where(a => userIdArr.Contains(a.Id)).Select(a => a.FirstName + " " + a.LastName).ToArray();
                                        admintsk.AssigneeName = string.Join<string>(" , ", assigneeNameObj);
                                        admintsk.AssigneeID = 0;
                                    }
                                    else if (taskAssigneesList.Count == 1)
                                    {
                                        var assigneeNameObj = tx.PersistenceManager.UserRepository.Query<BrandSystems.Marcom.Dal.User.Model.UserDao>().Where(a => a.Id == taskAssigneesList[0].UserID).Select(a => a).FirstOrDefault();
                                        admintsk.AssigneeName = assigneeNameObj.FirstName + " " + assigneeNameObj.LastName;
                                        admintsk.AssigneeID = assigneeNameObj.Id;

                                    }

                                }
                                else
                                {
                                    admintsk.taskAssigness = null;
                                    admintsk.AssigneeName = "";
                                }
                            }
                        }
                        if (taskchecklistCount.Count > 0 && val.TaskStatus == (int)TaskStatus.Unassigned && val.TaskType == (int)TaskTypes.Work_Task)
                        {

                            var completedChecklists = taskchecklistCount.Where(a => a.Status == true).ToList().Count();
                            admintsk.ProgressCount = "(" + completedChecklists.ToString() + "/" + taskchecklistCount.Count().ToString() + ")";
                        }
                        admintsk.AttributeData = null;
                        iitaskDetails.Add(admintsk);
                    });


                }
                return iitaskDetails.OrderBy(a => a.SortOrder).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public IList<IEntityTask> AssignUnassignTasktoMembers(TaskManagerProxy proxy, int entityId, int taskListId, int[] taskCollection, int[] memberCollection, DateTime? dueDate)
        {
            try
            {
                proxy.MarcomManager.AccessManager.TryEntityTypeAccess(entityId, Modules.Planning);
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    IList<EntityTaskDao> itsk_Dao = new List<EntityTaskDao>();
                    IList<IEntityTask> iitaskList = new List<IEntityTask>();
                    EntityTaskDao tsk_Dao = new EntityTaskDao();
                    TaskMembersDao tskmem_Dao = new TaskMembersDao();
                    IList<TaskMembersDao> itsk_mem_Dao = new List<TaskMembersDao>();
                    FeedNotificationServer fs = new FeedNotificationServer();
                    WorkFlowNotifyHolder obj = new WorkFlowNotifyHolder();
                    obj.MultiTask = new List<int>();
                    var taskListObj = tx.PersistenceManager.TaskRepository.Query<EntityTaskDao>().Where(a => taskCollection.Contains(a.ID) && a.TaskListID == taskListId && a.EntityID == entityId).ToList();
                    foreach (var val in taskListObj)
                    {
                        tsk_Dao = new EntityTaskDao();
                        tsk_Dao = val;
                        tsk_Dao.ID = val.ID;
                        tsk_Dao.EntityTaskListID = 0;
                        tsk_Dao.TaskStatus = (int)TaskStatus.In_progress;
                        tsk_Dao.DueDate = dueDate;

                        obj.MultiTask.Add(val.ID);
                        itsk_Dao.Add(tsk_Dao);
                        foreach (var mem in memberCollection)
                        {
                            tskmem_Dao = new TaskMembersDao();
                            tskmem_Dao.RoleID = 1;
                            tskmem_Dao.TaskID = val.ID;
                            tskmem_Dao.UserID = proxy.MarcomManager.User.Id;
                            tskmem_Dao.ApprovalRount = 1;
                            tskmem_Dao.ApprovalStatus = (int)TaskStatus.Approved;
                            tskmem_Dao.FlagColorCode = "f5f5f5";
                            itsk_mem_Dao.Add(tskmem_Dao);

                            tskmem_Dao = new TaskMembersDao();
                            tskmem_Dao.RoleID = 4;
                            tskmem_Dao.TaskID = val.ID;
                            tskmem_Dao.UserID = mem;
                            tskmem_Dao.ApprovalRount = 1;
                            tskmem_Dao.ApprovalStatus = null;
                            tskmem_Dao.FlagColorCode = "f5f5f5";
                            itsk_mem_Dao.Add(tskmem_Dao);

                        }

                    };
                    if (itsk_Dao.Count > 0)
                    {
                        tx.PersistenceManager.TaskRepository.Save<EntityTaskDao>(itsk_Dao);
                        tx.PersistenceManager.TaskRepository.Save<TaskMembersDao>(itsk_mem_Dao);
                        tx.Commit();
                        iitaskList = GetReassignedEntityTaskListDetails(proxy, taskCollection, entityId, taskListId);


                        obj.action = "multitask task member added";
                        obj.ientityRoles = itsk_mem_Dao;
                        obj.Actorid = proxy.MarcomManager.User.Id;
                        fs.AsynchronousNotify(obj);

                        return iitaskList;
                    }
                    else
                    {
                        return iitaskList;
                    }

                }

            }
            catch (MarcomAccessDeniedException ex)
            {
                throw ex;
            }
            catch
            {
                throw null;
            }
        }

        public IList<IAdminTaskCheckList> getAdminTaskchecklist(TaskManagerProxy proxy, int TaskId)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    IList<IAdminTaskCheckList> iichecklist = new List<IAdminTaskCheckList>();
                    IList<AdminTaskCheckListDao> checkDao = new List<AdminTaskCheckListDao>();
                    checkDao = tx.PersistenceManager.TaskRepository.Query<AdminTaskCheckListDao>().Where(a => a.TaskId == TaskId).Select(a => a).OrderBy(a => a.SortOrder).ToList();
                    foreach (var data in checkDao)
                    {
                        IAdminTaskCheckList ichecklist = new AdminTaskCheckList();
                        ichecklist.ID = data.ID;
                        ichecklist.NAME = HttpUtility.HtmlEncode(data.NAME);
                        ichecklist.TaskId = data.TaskId;
                        ichecklist.SortOrder = data.SortOrder;
                        iichecklist.Add(ichecklist);

                    }
                    return iichecklist;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public IList GetAdminTasksById(TaskManagerProxy proxy, string[] tasklistArr)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {

                    StringBuilder AdmintaskQry = new StringBuilder();


                    AdmintaskQry.Append("SELECT P.ID AS 'Id' , ");
                    AdmintaskQry.Append("       p.Caption, ");
                    AdmintaskQry.Append("       p.TaskType as 'TaskType',  ");
                    AdmintaskQry.Append("       p.[Description], ");
                    AdmintaskQry.Append("       p.SortOrder as 'SortOder',  ");
                    AdmintaskQry.Append("       ( SELECT TOP 1 pe.TypeID  FROM   PM_Entity pe WHERE  pe.ID = p.ID ) AS 'TaskID',  ");
                    AdmintaskQry.Append("        ( ");
                    AdmintaskQry.Append("        SELECT TOP 1 met.ColorCode ");
                    AdmintaskQry.Append("       FROM   MM_EntityType met ");
                    AdmintaskQry.Append("        WHERE  id                = ( ");
                    AdmintaskQry.Append("                 SELECT TOP 1 PM_Entity.TypeID ");
                    AdmintaskQry.Append("              FROM   PM_Entity ");
                    AdmintaskQry.Append("               WHERE  id     = p.ID ");
                    AdmintaskQry.Append("          ) ");
                    AdmintaskQry.Append("        )              AS 'colorcode', ");
                    AdmintaskQry.Append("        ( ");
                    AdmintaskQry.Append("           SELECT TOP 1 met.ShortDescription ");
                    AdmintaskQry.Append("         FROM   MM_EntityType met ");
                    AdmintaskQry.Append("         WHERE  id                = ( ");
                    AdmintaskQry.Append("                 SELECT TOP 1 PM_Entity.TypeID ");
                    AdmintaskQry.Append("                 FROM   PM_Entity ");
                    AdmintaskQry.Append("                WHERE  id     = p.ID ");
                    AdmintaskQry.Append("            ) ");
                    AdmintaskQry.Append("       )              AS 'ShortDescription', ");
                    AdmintaskQry.Append("      p.TaskListID as 'TaskListID',  ");
                    AdmintaskQry.Append("       ISNULL( ");
                    AdmintaskQry.Append("           ( ");
                    AdmintaskQry.Append("               SELECT CASE ");
                    AdmintaskQry.Append("                           WHEN p.TaskType = 2 THEN 'Work Task' ");
                    AdmintaskQry.Append("                           WHEN p.TaskType = 3 THEN 'Approval Task' ");
                    AdmintaskQry.Append("                           ELSE 'Review Task' ");
                    AdmintaskQry.Append("                      END ");
                    AdmintaskQry.Append("           ), ");
                    AdmintaskQry.Append("           'No task' ");
                    AdmintaskQry.Append("       )              AS 'TaskTypeName', ");
                    AdmintaskQry.Append("       ISNULL( ");
                    AdmintaskQry.Append("           STUFF( ");
                    AdmintaskQry.Append("               ( ");
                    AdmintaskQry.Append("                   SELECT ',' + tta.Name ");
                    AdmintaskQry.Append("                   FROM   DAM_Asset tta ");
                    AdmintaskQry.Append("                   WHERE  tta.EntityID = P.ID ");
                    AdmintaskQry.Append("                   ORDER BY ");
                    AdmintaskQry.Append("                          tta.Name ");
                    AdmintaskQry.Append("                          FOR XML PATH('') ");
                    AdmintaskQry.Append("               ), ");
                    AdmintaskQry.Append("               1, ");
                    AdmintaskQry.Append("               1, ");
                    AdmintaskQry.Append("               '' ");
                    AdmintaskQry.Append("           ), ");
                    AdmintaskQry.Append("           0 ");
                    AdmintaskQry.Append("       )              AS 'AttachmentCollection', ");
                    AdmintaskQry.Append("       ( ");
                    AdmintaskQry.Append("           ( ");
                    AdmintaskQry.Append("               SELECT COUNT(1) ");
                    AdmintaskQry.Append("               FROM   DAM_Asset tta ");
                    AdmintaskQry.Append("               WHERE  tta.EntityID = P.ID ");
                    AdmintaskQry.Append("           ) + ( ");
                    AdmintaskQry.Append("               SELECT COUNT(1) ");
                    AdmintaskQry.Append("               FROM   TM_Links tl ");
                    AdmintaskQry.Append("               WHERE  tl.EntityID = P.ID ");
                    AdmintaskQry.Append("           ) ");
                    AdmintaskQry.Append("       )              AS 'TotalTaskAttachment' ");
                    AdmintaskQry.Append(" FROM   TM_Admin_Task     P ");
                    string inClause = "("
                                        + String.Join(",", tasklistArr.Select(x => x.ToString()).ToArray())
                                      + ")";
                    AdmintaskQry.Append(" WHERE  p.TaskListID IN " + inClause + " ");
                    AdmintaskQry.Append(" ORDER BY");
                    AdmintaskQry.Append("       p.TaskListID, p.SortOrder  ");

                    IList AdminTaskResult = tx.PersistenceManager.TaskRepository.ExecuteQuery(AdmintaskQry.ToString());

                    return AdminTaskResult;
                }
            }
            catch
            {
                return null;
            }

        }

        public IList<IMilestoneMetadata> GetAdminTaskMetadatabyTaskID(TaskManagerProxy proxy, int taskID)
        {
            try
            {
                IList<IMilestoneMetadata> listMilestone = new List<IMilestoneMetadata>();
                listMilestone = GetAdminTaskMetadata(proxy, taskID, (int)EntityTypeList.Task);
                return listMilestone;

            }
            catch
            {
                return null;
            }

        }


        public IList<ITaskLibraryTemplateHolder> GetFiltertaskCountByStatus(TaskManagerProxy proxy, Dictionary<int, int> Maintask, int[] filter)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {

                    IList<ITaskLibraryTemplateHolder> iiCountHolder = new List<ITaskLibraryTemplateHolder>();
                    string parentEntClause = "("
                                         + String.Join(",", Maintask.Select(x => x.Value.ToString()).ToArray())
                                       + ")";
                    string parentTaskClause = "("
                                         + String.Join(",", Maintask.Select(x => x.Key.ToString()).ToArray())
                                       + ")";

                    List<int> statusColl = filter.Select(x => x).ToList();

                    if (statusColl.Contains(2) || statusColl.Contains(3))
                    {
                        statusColl.Add(8);
                    }

                    var taskqry = new StringBuilder();
                    int sortOrderID = 0;
                    string inClause = "";

                    foreach (var val in Maintask.Keys)
                    {
                        ITaskLibraryTemplateHolder icount = new TaskLibraryTemplateHolder();
                        taskqry = new StringBuilder();
                        sortOrderID = 0;
                        inClause = "";
                        icount.EntityParentID = Maintask[val];
                        icount.ID = val;
                        taskqry.AppendLine(" SELECT COUNT(*) AS taskcount FROM TM_EntityTask tet WHERE tet.EntityID= ? AND tet.TaskListID= ? ");
                        if (statusColl.Count > 0)
                        {
                            inClause = "("
                                       + String.Join(",", statusColl.Select(x => x.ToString()).ToArray())
                                     + ")";
                            taskqry.AppendLine(" AND tet.TaskStatus IN " + inClause + " ");
                        }
                        IList sortOrderVal = tx.PersistenceManager.PlanningRepository.ExecuteQuerywithMinParam(taskqry.ToString(), Maintask[val], val);
                        sortOrderID = (int)((System.Collections.Hashtable)(sortOrderVal)[0])["taskcount"];
                        icount.TaskCount = sortOrderID;
                        iiCountHolder.Add(icount);
                    }

                    return iiCountHolder;
                }
            }
            catch
            {
                return null;
            }
        }

        public IList<ITaskLibraryTemplateHolder> GetTemplateAdminTaskList(TaskManagerProxy proxy)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    IList<ITaskLibraryTemplateHolder> tasklist = new List<ITaskLibraryTemplateHolder>();
                    var adminTaskList = tx.PersistenceManager.PlanningRepository.GetAll<TaskListDao>().OrderBy(a => a.Sortorder);

                    foreach (var res in adminTaskList)
                    {
                        ITaskLibraryTemplateHolder tskLst = new TaskLibraryTemplateHolder();
                        tskLst.LibraryName = HttpUtility.HtmlEncode(res.Caption);
                        tskLst.LibraryDescription = HttpUtility.HtmlEncode(res.Description);
                        tskLst.ID = res.ID;
                        tskLst.SortOrder = res.Sortorder;
                        tskLst.TaskList = tx.PersistenceManager.TaskRepository.Query<AdminTaskDao>().Where(a => a.TaskListID == res.ID).Count();
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

        public bool UpdateEntityTask(TaskManagerProxy proxy, params object[] CollectionIds)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    EntityTaskDao ObjEntityTask = tx.PersistenceManager.TaskRepository.Get<EntityTaskDao>(Convert.ToInt32(CollectionIds[0].ToString()));
                    string Name1 = ObjEntityTask.Name;
                    int BentityName = ObjEntityTask.EntityID;
                    EntityDao ObjEntityBefore = tx.PersistenceManager.TaskRepository.Get<EntityDao>((int)(BentityName));
                    string ObjEntityNameBefore = ObjEntityBefore.Name;
                    int EntityNameBeforetypeid = ObjEntityBefore.Typeid;

                    int ObjEntityType = ObjEntityTask.TaskType;

                    EntityTaskListDao sourcetasklistdao = new EntityTaskListDao();
                    EntityTaskListDao desctasklistdao = new EntityTaskListDao();
                    sourcetasklistdao = (from item in tx.PersistenceManager.TaskRepository.Query<EntityTaskListDao>() where item.ID == (int)ObjEntityTask.TaskListID select item).FirstOrDefault();
                    desctasklistdao = (from item in tx.PersistenceManager.TaskRepository.Query<EntityTaskListDao>() where item.ID == Convert.ToInt32(CollectionIds[1].ToString()) select item).FirstOrDefault();
                    string FromTasklistname = sourcetasklistdao.Name;
                    string ToTasklistname = desctasklistdao.Name;

                    tx.PersistenceManager.TaskRepository.ExecuteQuery("UPDATE TM_EntityTask SET EntityID = " + CollectionIds[2] + ",TaskListID =" + CollectionIds[1] + " WHERE ID=" + CollectionIds[0]);
                    tx.Commit();
                    EntityTaskDao ObjEntityTask1 = tx.PersistenceManager.TaskRepository.Get<EntityTaskDao>(Convert.ToInt32(CollectionIds[0].ToString()));
                    int AentityName = ObjEntityTask1.EntityID;
                    EntityDao ObjEntityAfter = tx.PersistenceManager.TaskRepository.Get<EntityDao>((int)(AentityName));
                    int ObjEntityIDAfter = ObjEntityAfter.Id;
                    string ObjEntityNameAfter = ObjEntityAfter.Name;
                    int TaskId = ObjEntityAfter.Id;

                    FeedNotificationServer fs = new FeedNotificationServer();
                    NotificationFeedObjects obj = new NotificationFeedObjects();
                    obj.EntityId = TaskId;
                    obj.action = "MoveTask";
                    obj.FromValue = FromTasklistname;
                    obj.ToValue = ToTasklistname;
                    obj.AttributeName = BentityName.ToString();
                    //obj.FromValue = Convert.ToString(ObjEntityBefore.Id);
                    //obj.ToValue = Convert.ToString(ObjEntityAfter.Id);
                    obj.AssociatedEntityId = ObjEntityTask.ID;
                    obj.Userid = proxy.MarcomManager.User.Id;
                    obj.EntityTypeId = ObjEntityType;
                    fs.AsynchronousNotify(obj);
                    return true;
                }
            }
            catch
            {

            }
            return false;
        }


        public Tuple<IList<SourceDestinationMember>, IList<SourceDestinationMember>, bool> GetSourceToDestinationmembers(TaskManagerProxy proxy, int taskID, int sourceEntityID, int destinationEntityId)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {

                    Tuple<IList<SourceDestinationMember>, IList<SourceDestinationMember>, bool> srcdesttpl;
                    IList<SourceDestinationMember> srcItemColl = new List<SourceDestinationMember>();
                    IList<SourceDestinationMember> dstItemColl = new List<SourceDestinationMember>();
                    bool IsViewerPresent = false;
                    var ViewerColl = (from item1 in tx.PersistenceManager.TaskRepository.Query<EntityRoleUserDao>()
                                      join item2 in tx.PersistenceManager.TaskRepository.Query<EntityTypeRoleAclDao>()
                                      on item1.Roleid equals item2.ID
                                      where item1.Entityid == destinationEntityId && item2.EntityRoleID == (int)EntityRoles.Viewer
                                      select new { ID = item1.Id }).ToList();
                    if (ViewerColl.Count() > 0)
                    {
                        IsViewerPresent = true;
                    }
                    else
                    {
                        IsViewerPresent = false;
                        int[] UserTasklstCollections = tx.PersistenceManager.TaskRepository.Query<TaskMembersDao>().Where(a => a.TaskID == taskID).Select(a => a.UserID).Distinct().ToArray();

                        int tempUserid = 0;
                        var srcColl = (from item in tx.PersistenceManager.TaskRepository.Query<EntityRoleUserDao>().Where(a => a.Entityid == destinationEntityId && UserTasklstCollections.Contains(a.Userid))
                                       select new { Userid = item.Userid, Entityid = item.Entityid }).ToList().Distinct();
                        if (srcColl != null)
                        {
                            foreach (var srcval in srcColl)
                            {
                                if (tempUserid != srcval.Userid)
                                {
                                    tempUserid = srcval.Userid;
                                    SourceDestinationMember currentObj = new SourceDestinationMember();
                                    currentObj.Id = tx.PersistenceManager.TaskRepository.Query<TaskMembersDao>().Where(a => a.TaskID == taskID && a.UserID == srcval.Userid).Select(a => a.ID).First();
                                    int[] RoleIDs = tx.PersistenceManager.TaskRepository.Query<EntityRoleUserDao>().Where(a => a.Entityid == srcval.Entityid && a.Userid == srcval.Userid).Select(a => a.Roleid).ToArray();
                                    var currentRoleAclName = string.Join(",", tx.PersistenceManager.TaskRepository.Query<EntityTypeRoleAclDao>().Where(a => RoleIDs.Contains(a.ID)).Select(a => a.Caption).ToArray());
                                    currentObj.ActualRoleId = 0;
                                    currentObj.RoleName = currentRoleAclName;
                                    currentObj.EntityID = srcval.Entityid;
                                    currentObj.RoleID = 0;
                                    currentObj.Status = false;
                                    currentObj.UserID = srcval.Userid;
                                    currentObj.UserName = tx.PersistenceManager.TaskRepository.Query<UserDao>().Where(a => a.Id == srcval.Userid).Select(a => a.FirstName + " " + a.LastName).SingleOrDefault();
                                    currentObj.IstaskOwner = tx.PersistenceManager.TaskRepository.Query<TaskMembersDao>().Where(a => a.TaskID == taskID && a.UserID == srcval.Userid && a.RoleID == 1).ToList().Count() > 0;
                                    srcItemColl.Add(currentObj);
                                }

                            }
                        }

                        int[] destColl = UserTasklstCollections.Where(a => !srcColl.Select(x => x.Userid).Distinct().ToArray().Contains(a)).ToArray();

                        tempUserid = 0;
                        if (destColl != null)
                        {
                            int EntityTypeID = tx.PersistenceManager.AccessRepository.Query<EntityDao>().Where(a => a.Id == destinationEntityId).Select(a => a.Typeid).FirstOrDefault();
                            int RoleID = tx.PersistenceManager.TaskRepository.Query<EntityTypeRoleAclDao>().Where(a => a.EntityTypeID == EntityTypeID && a.EntityRoleID == (int)EntityRoles.Editer).Select(a => a.ID).First();
                            foreach (var dstval in destColl)
                            {
                                if (tempUserid != dstval)
                                {
                                    tempUserid = dstval;
                                    SourceDestinationMember currentObj = new SourceDestinationMember();
                                    currentObj.Id = tx.PersistenceManager.TaskRepository.Query<TaskMembersDao>().Where(a => a.TaskID == taskID && a.UserID == dstval).Select(a => a.ID).First();
                                    currentObj.ActualRoleId = 0;// currentRoleAcl.EntityRoleID;
                                    currentObj.RoleName = "";// currentRoleAcl.Caption;
                                    currentObj.EntityID = destinationEntityId;
                                    currentObj.RoleID = RoleID;
                                    currentObj.Status = false;
                                    currentObj.UserID = dstval;
                                    currentObj.UserName = tx.PersistenceManager.TaskRepository.Query<UserDao>().Where(a => a.Id == dstval).Select(a => a.FirstName + " " + a.LastName).SingleOrDefault();
                                    currentObj.IstaskOwner = tx.PersistenceManager.TaskRepository.Query<TaskMembersDao>().Where(a => a.TaskID == taskID && a.UserID == dstval && a.RoleID == 1).ToList().Count() > 0;
                                    dstItemColl.Add(currentObj);
                                }
                            }
                        }
                    }
                    srcdesttpl = Tuple.Create(srcItemColl, dstItemColl, IsViewerPresent);

                    return srcdesttpl;

                }
            }
            catch
            {

            }
            return null;
        }


        public IList<IEntityTypeRoleAcl> GetDestinationEntityIdRoleAccess(TaskManagerProxy proxy, int EntityID)
        {
            try
            {

                IList<IEntityTypeRoleAcl> _iientityTyperoleobj = new List<IEntityTypeRoleAcl>();
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    int EntityTypeID = tx.PersistenceManager.AccessRepository.Query<EntityDao>().Where(a => a.Id == EntityID).Select(a => a.Typeid).FirstOrDefault();
                    int[] FilterIds = { (int)EntityRoles.ExternalParticipate, (int)EntityRoles.Viewer, (int)EntityRoles.Owner, (int)EntityRoles.BudgerApprover };
                    var entityroleobj = tx.PersistenceManager.AccessRepository.Query<EntityTypeRoleAclDao>().Where(a => a.EntityTypeID == EntityTypeID && !FilterIds.Contains(a.EntityRoleID)).OrderBy(a => a.Sortorder);
                    foreach (var item in entityroleobj)
                    {
                        EntityTypeRoleAcl entitytypeRoleAclObj = new EntityTypeRoleAcl();
                        entitytypeRoleAclObj.ID = item.ID;
                        entitytypeRoleAclObj.EntityTypeID = item.EntityTypeID;
                        entitytypeRoleAclObj.EntityRoleID = item.EntityRoleID;
                        entitytypeRoleAclObj.ModuleID = item.ModuleID;
                        entitytypeRoleAclObj.Sortorder = item.Sortorder;
                        entitytypeRoleAclObj.Caption = item.Caption;
                        _iientityTyperoleobj.Add(entitytypeRoleAclObj);
                    }
                }
                return _iientityTyperoleobj;

            }
            catch
            {


            }
            return null;
        }

        public bool InsertUpdateDragTaskMembers(TaskManagerProxy proxy, IList<SourceDestinationMember> TaskMemberslst, IList<SourceDestinationMember> EntitytaskMemberLst, int SourceTaskID, int TargetTasklistID, int TargetEntityID)
        {
            try
            {
                if (UpdateEntityTask(proxy, SourceTaskID, TargetTasklistID, TargetEntityID))
                {
                    using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                    {
                        IList<EntityRoleUserDao> entityRoleUserDao = new List<EntityRoleUserDao>();
                        if (EntitytaskMemberLst != null)
                        {
                            foreach (var Currentitem in EntitytaskMemberLst)
                            {
                                if (Currentitem.Status)
                                {
                                    EntityRoleUserDao roleuser = new EntityRoleUserDao();
                                    roleuser.Id = 0;
                                    roleuser.Entityid = Currentitem.EntityID;
                                    roleuser.InheritedFromEntityid = 0;
                                    roleuser.IsInherited = false;
                                    roleuser.Roleid = Currentitem.RoleID;
                                    roleuser.Userid = Currentitem.UserID;
                                    entityRoleUserDao.Add(roleuser);
                                }
                                else
                                {
                                    tx.PersistenceManager.TaskRepository.DeleteByID<TaskMemberDao>(Currentitem.Id);
                                }



                            }

                            tx.PersistenceManager.TaskRepository.Save<EntityRoleUserDao>(entityRoleUserDao);
                        }


                        if (TaskMemberslst != null)
                        {

                            int[] IdArr = TaskMemberslst.ToList().Where(a => a.Status == false).Select(a => a.Id).ToArray();

                            if (IdArr.Length > 0)
                            {
                                tx.PersistenceManager.TaskRepository.ExecuteQuery("DELETE FROM TM_Task_Members WHERE id IN (" + string.Join(",", IdArr.Select(a => a).ToArray()) + ")");
                            }
                        }
                        tx.Commit();
                        return true;
                    }
                }
            }
            catch
            {

            }
            return false;
        }

        public bool UpdateDragEntityTaskListByTask(TaskManagerProxy proxy, int TaskID, int SrcTaskListID, int TargetTaskListID)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    tx.PersistenceManager.TaskRepository.ExecuteQuery("UPDATE TM_EntityTask SET TaskListID =" + TargetTaskListID + " WHERE ID=" + TaskID + " and TaskListID=" + SrcTaskListID);
                    tx.Commit();
                    return true;
                }
            }
            catch
            {

            }
            return false;
        }

        public IList GetEntityTaskAttachmentinfo(TaskManagerProxy proxy, int TaskID)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    StringBuilder sqlQry = new StringBuilder();
                    sqlQry.Append("SELECT SUM(df.Size) as totalfilesize,COUNT(*) as totalcount FROM DAM_Asset AS da LEFT OUTER JOIN  ");
                    sqlQry.Append("DAM_File AS df ON  da.ActiveFileID=df.ID ");
                    sqlQry.Append("WHERE EntityID=" + TaskID + "");
                    IList TaskResult = tx.PersistenceManager.TaskRepository.ExecuteQuery(sqlQry.ToString());
                    tx.Commit();
                    return TaskResult;


                }
            }
            catch
            {
                return null;
            }

        }

        /// Updating proof Task status 
        /// </summary>
        ///  <param name="proxy"></param>
        /// <param name="entityId">The TaskID</param>
        /// <param name="status">The Status</param>
        /// <returns>True or False</returns>
        public Tuple<bool, int, string> UpdateProofTaskStatus(TaskManagerProxy proxy, int taskID, int status, int userid)
        {
            try
            {

                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {

                    Tuple<bool, int, string> retObj = null;
                    EntityTaskDao EntityTaskDao = new EntityTaskDao();
                    IList<EntityTaskDao> Itask = new List<EntityTaskDao>();
                    TaskMembersDao TaskMembersDao = new TaskMembersDao();
                    IList<TaskMembersDao> Itaskmember = new List<TaskMembersDao>();
                    IList<MultiProperty> prplst = new List<MultiProperty>();
                    FeedNotificationServer fs = new FeedNotificationServer();
                    WorkFlowNotifyHolder wfhobj = new WorkFlowNotifyHolder();

                    var taskType = (from item in tx.PersistenceManager.PlanningRepository.Query<EntityTaskDao>() where item.ID == taskID select item).FirstOrDefault();
                    prplst.Add(new MultiProperty { propertyName = EntityTaskDao.PropertyNames.ID, propertyValue = taskID });
                    EntityTaskDao = (tx.PersistenceManager.AccessRepository.GetEquals<EntityTaskDao>(prplst)).FirstOrDefault();
                    var allTaskMembers = tx.PersistenceManager.PlanningRepository.Query<TaskMembersDao>().Where(a => a.TaskID == taskID).ToList();
                    var currentTaskrountMembers = (from item in allTaskMembers where item.RoleID != 1 select item).ToList<TaskMembersDao>();
                    TaskMembersDao = (from data in tx.PersistenceManager.PlanningRepository.Query<TaskMembersDao>() where data.RoleID != 1 && data.UserID == userid && data.TaskID == taskID select data).FirstOrDefault();
                    wfhobj.FromValue = (TaskMembersDao.ApprovalStatus != null ? ((TaskStatus)TaskMembersDao.ApprovalStatus).ToString() : "");
                    if (status == (int)TaskStatus.Withdrawn)
                    {
                        var unUpdatedUsers = currentTaskrountMembers.Where(a => a.ApprovalStatus == null).Select(a => a).ToList();
                        string statusname = "";
                        if (unUpdatedUsers.Count() == currentTaskrountMembers.Count())
                        {
                            EntityTaskDao.TaskStatus = status;
                            tx.Commit();
                            statusname = Enum.GetName(typeof(TaskStatus), status).Replace("_", " ");
                            retObj = Tuple.Create(true, (int)TaskStatus.Withdrawn, statusname);
                        }
                        else
                        {
                            retObj = Tuple.Create(false, 0, "");
                        }
                        return retObj;
                    }
                    else
                    {
                        if (taskType.TaskType == 2)
                        {
                            if (TaskMembersDao != null)
                            {
                                if (status != (int)TaskStatus.RevokeTask)
                                    TaskMembersDao.ApprovalStatus = (int)(TaskStatus)status;
                                else
                                    TaskMembersDao.ApprovalStatus = null;
                                Itaskmember.Add(TaskMembersDao);
                                tx.PersistenceManager.PlanningRepository.Save<TaskMembersDao>(Itaskmember);
                                tx.Commit();
                            }
                            else
                                tx.Commit();
                            using (ITransaction txinner = proxy.MarcomManager.GetTransaction())
                            {
                                currentTaskrountMembers = tx.PersistenceManager.PlanningRepository.Query<TaskMembersDao>().Where(a => a.TaskID == taskID && a.RoleID != 1).ToList();

                                var total_Approval_Status_For_This_Round = currentTaskrountMembers.Where(a => a.ApprovalStatus == (int)TaskStatus.Completed).Count();

                                if (status != (int)TaskStatus.RevokeTask)
                                {

                                    if (currentTaskrountMembers.Count() == 1 || status == (int)TaskStatus.Completed)
                                    {

                                        EntityTaskDao.TaskStatus = status;
                                    }

                                    else
                                    {

                                        if (currentTaskrountMembers.Count() == (total_Approval_Status_For_This_Round))
                                            EntityTaskDao.TaskStatus = status;

                                    }
                                }
                                else
                                {
                                    if (currentTaskrountMembers.Count() != (total_Approval_Status_For_This_Round))
                                        EntityTaskDao.TaskStatus = (int)TaskStatus.In_progress;
                                }
                                Itask.Add(EntityTaskDao);
                                txinner.PersistenceManager.PlanningRepository.Save<EntityTaskDao>(Itask);
                                txinner.Commit();
                            }
                        }
                        else if (taskType.TaskType == 3 || taskType.TaskType == 32 || taskType.TaskType == 36)
                        {
                            if (TaskMembersDao != null)
                            {
                                if (status != (int)TaskStatus.RevokeTask)
                                    TaskMembersDao.ApprovalStatus = (int)(TaskStatus)status;
                                else
                                    TaskMembersDao.ApprovalStatus = null;
                                Itaskmember.Add(TaskMembersDao);
                                tx.PersistenceManager.PlanningRepository.Save<TaskMembersDao>(TaskMembersDao);
                                tx.Commit();
                            }
                            else
                                tx.Commit();

                            using (ITransaction txinner = proxy.MarcomManager.GetTransaction())
                            {
                                currentTaskrountMembers = tx.PersistenceManager.PlanningRepository.Query<TaskMembersDao>().Where(a => a.TaskID == taskID && a.RoleID != 1).ToList();

                                var total_Approval_Status_For_This_Round = currentTaskrountMembers.Where(a => a.ApprovalStatus == (int)TaskStatus.Approved).Count();

                                if (status != (int)TaskStatus.RevokeTask)
                                {

                                    if (EntityTaskDao.TaskStatus != (int)TaskStatus.Unable_to_complete && EntityTaskDao.TaskStatus != (int)TaskStatus.Rejected)
                                    {

                                        if (status == (int)TaskStatus.Unable_to_complete || status == (int)TaskStatus.Rejected)
                                        {
                                            EntityTaskDao.TaskStatus = status;
                                        }
                                        else
                                        {
                                            if (currentTaskrountMembers.Count() == (total_Approval_Status_For_This_Round))
                                                EntityTaskDao.TaskStatus = status;
                                        }
                                    }
                                }
                                else
                                {

                                    if (currentTaskrountMembers.Count() != (total_Approval_Status_For_This_Round))
                                        EntityTaskDao.TaskStatus = (int)TaskStatus.In_progress;
                                }
                                Itask.Add(EntityTaskDao);
                                txinner.PersistenceManager.PlanningRepository.Save<EntityTaskDao>(Itask);
                                txinner.Commit();

                            }
                        }
                        else if (taskType.TaskType == 31)
                        {
                            if (TaskMembersDao != null)
                            {
                                if (status != (int)TaskStatus.RevokeTask)
                                    TaskMembersDao.ApprovalStatus = (int)(TaskStatus)status;
                                else
                                    TaskMembersDao.ApprovalStatus = null;
                                Itaskmember.Add(TaskMembersDao);
                                tx.PersistenceManager.PlanningRepository.Save<TaskMembersDao>(TaskMembersDao);
                                tx.Commit();
                            }
                            else
                                tx.Commit();

                            using (ITransaction txinner = proxy.MarcomManager.GetTransaction())
                            {
                                currentTaskrountMembers = tx.PersistenceManager.PlanningRepository.Query<TaskMembersDao>().Where(a => a.TaskID == taskID && a.RoleID != 1).ToList();
                                var total_Approval_Status_For_This_Round = currentTaskrountMembers.Where(a => a.ApprovalStatus == (int)TaskStatus.Completed).Count();
                                if (EntityTaskDao.TaskStatus != (int)TaskStatus.Unable_to_complete && EntityTaskDao.TaskStatus != (int)TaskStatus.Rejected)
                                {

                                    if (status == (int)TaskStatus.Unable_to_complete || status == (int)TaskStatus.Rejected)
                                    {
                                        EntityTaskDao.TaskStatus = status;
                                    }
                                    else
                                    {
                                        if (currentTaskrountMembers.Count() == (total_Approval_Status_For_This_Round))
                                            EntityTaskDao.TaskStatus = status;
                                    }
                                }
                                Itask.Add(EntityTaskDao);
                                txinner.PersistenceManager.PlanningRepository.Save<EntityTaskDao>(Itask);
                                txinner.Commit();

                            }

                        }
                        string statusname = "";
                        statusname = Enum.GetName(typeof(TaskStatus), Itask.FirstOrDefault().TaskStatus).Replace("_", " ");
                        retObj = Tuple.Create(true, Itask.FirstOrDefault().TaskStatus, statusname);


                        wfhobj.Actorid = TaskMembersDao.UserID;
                        wfhobj.action = "Task Status change";
                        wfhobj.Tasktatus = Enum.GetName(typeof(TaskStatus), status).Replace("_", " ");
                        wfhobj.obj2 = new List<object>();
                        foreach (var c in allTaskMembers)
                        {
                            wfhobj.obj2.Add((TaskMembersDao)c);

                        }
                        wfhobj.EntityId = taskID;
                        wfhobj.AttributeId = taskType.TaskType;
                        fs.AsynchronousNotify(wfhobj);

                        return retObj;
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Getting Task details
        /// </summary>
        /// <param name="proxy"></param>
        /// <param name="entityID">The entityID</param>
        /// <param name="taskListID">The taskListID</param>
        /// <param name="pageNo">The pageNo</param>
        /// <returns>IList</returns>
        public IList GetEntityTaskCollection(TaskManagerProxy proxy, int entityID, int taskListID, int pageNo)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("    DECLARE @RowsPerPage     INT =15, ");
                    sb.AppendLine("    @PageNumber      INT = " + pageNo + " ");
                    sb.AppendLine("    SELECT tet.id as 'Id', ");
                    sb.AppendLine("    tet.name, ");
                    sb.AppendLine("    tet.[Description], ");
                    sb.AppendLine("    tet.TaskStatus, ");
                    sb.AppendLine("    CONVERT(char(10), tet.DueDate,126) as 'DueDate', tet.EntityID AS 'EntityID', ");
                    sb.AppendLine("    tet.EntityTaskListID, ");
                    sb.AppendLine("    tet.Sortorder, ");
                    sb.AppendLine("    tet.TaskType, ");
                    sb.AppendLine("    tet.Note,tet.TaskListID, ");
                    sb.AppendLine("    tet.AssetId, ");
                    sb.AppendLine("    COUNT(*) OVER()            AS 'Total_COUNT', ");
                    sb.AppendLine("    ( ");
                    sb.AppendLine("       SELECT ( ");
                    sb.AppendLine("                  SELECT met1.ColorCode AS '@c', ");
                    sb.AppendLine("                         met1.ShortDescription AS '@s', ");
                    sb.AppendLine("                         met1.ID AS '@i' ");
                    sb.AppendLine("                       FOR XML PATH('p'), ");
                    sb.AppendLine("                         TYPE ");
                    sb.AppendLine("               ) ");
                    sb.AppendLine("            FOR XML PATH('root') ");
                    sb.AppendLine("    )                          AS 'TypeDetails', ");
                    sb.AppendLine("    dbo.taskusersinfo(tet.ID)  AS 'TotaTaskAssignees', ");
                    sb.AppendLine("     ( ");
                    sb.AppendLine("        SELECT COUNT(1) ");
                    sb.AppendLine("        FROM   TM_EntityTaskCheckList tetcl ");
                    sb.AppendLine("        WHERE  tetcl.TaskId = tet.ID ");
                    sb.AppendLine("     )                          AS 'checklists', ");
                    sb.AppendLine("     ( ");
                    sb.AppendLine("          SELECT COUNT(1) ");
                    sb.AppendLine("        FROM   TM_EntityTaskCheckList tetcl ");
                    sb.AppendLine("       WHERE  tetcl.TaskId = tet.ID ");
                    sb.AppendLine("           AND tetcl.[Status] = 1 ");
                    sb.AppendLine("       )                          AS 'completedchecklists' ");
                    sb.AppendLine("    FROM   TM_EntityTask tet ");
                    sb.AppendLine("    INNER JOIN PM_Entity pe1 on ");
                    sb.AppendLine("     tet.TaskListID = " + taskListID + "  AND tet.EntityID=" + entityID + "  ");
                    sb.AppendLine("     AND pe1.ID = tet.id ");
                    sb.AppendLine("    INNER JOIN MM_EntityType met1 ");
                    sb.AppendLine("        ON  met1.ID = pe1.TypeID ");
                    sb.AppendLine("    ORDER BY ");
                    sb.AppendLine("    tet.Sortorder              ASC ");
                    sb.AppendLine("    OFFSET(@PageNumber - 1) * @RowsPerPage ROWS ");
                    sb.AppendLine("    FETCH NEXT @RowsPerPage ROWS ONLY  ");

                    IList taskCollection = tx.PersistenceManager.TaskRepository.ExecuteQuery(sb.ToString());
                    tx.Commit();
                    return taskCollection;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
