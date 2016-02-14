using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrandSystems.Marcom.Core.Interface;
using BrandSystems.Marcom.Core.Interface.Managers;
using BrandSystems.Marcom.Core.Planning.Interface;
using BrandSystems.Marcom.Core.Access.Interface;
using System.Collections;
using Newtonsoft.Json.Linq;

namespace BrandSystems.Marcom.Core.Managers.Proxy
{
    internal class PlanningManagerProxy : IPlanningManager, IManagerProxy
    {
        // Reference to the MarcomManager
        /// <summary>
        /// The _marcom manager
        /// </summary>
        private MarcomManager _marcomManager = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlanningManagerProxy"/> class.
        /// </summary>
        /// <param name="marcomManager">The marcom manager.</param>
        internal PlanningManagerProxy(MarcomManager marcomManager)
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

        /// <summary>
        /// Intializes the imilestone.
        /// </summary>
        /// <param name="strBody">accepts string body containing imilestone objects</param>
        /// <returns>
        /// IMilestone
        /// </returns>
        public IMilestone IntializeImilestone(string strBody)
        {
            return PlanningManager.Instance.IntializeImilestone(strBody);
        }

        /// <summary>
        /// Creates the milestone.
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <param name="name">The name.</param>
        /// <param name="dueDate">The due date.</param>
        /// <returns>last inserted id</returns>
        //public int CreateMilestone(int entityId, int milestoneTypeId, string name, IList<IAttributeData> entityattributedata)
        //{
        //    return PlanningManager.Instance.CreateMilestone(this, entityId,milestoneTypeId, name, entityattributedata);
        //}

        /// <summary>
        /// Creates the milestone.
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <param name="name">The name.</param>
        /// <param name="description">The description.</param>
        /// <param name="dueDate">The due date.</param>
        /// <returns>last inserted id</returns>
        //public int CreateMilestone(int entityId, int milestoneTypeId, string name,IList<IAttributeData> entityattributedata)
        //{
        //    return PlanningManager.Instance.CreateMilestone(this, entityId,milestoneTypeId,name,entityattributedata);
        //}

        /// <summary>
        /// Creates the milestone.
        /// </summary>
        /// <param name="entityTypeId">The Entity Type Id.</param>
        /// <param name="name">The Name</param>
        /// <param name="attributes">List of attributes Data</param>
        /// <returns>last inserted id</returns>
        public int CreateMilestone(int milestoneTypeId, string name, IList<IAttributeData> entityattributedata)
        {
            return PlanningManager.Instance.CreateMilestone(this, milestoneTypeId, name, entityattributedata);
        }

        /// <summary>
        /// Gets the milestone.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>
        /// IMilestone
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public IMilestone GetMilestone(long id)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Updates the milestone.
        /// </summary>
        /// <param name="milestoneTypeID">The MilestoneTypID</param>
        /// <param name="milestoneName">The MilestoneName</param>
        /// <param name="milestoneObj">The List of Milstone AttributeData</param>
        /// <param name="entityId">The EntityID</param>
        /// <returns>bool</returns>
        public bool UpdateMilestone(int milestoneTypeID, string milstoneName, IList<IAttributeData> milestoneObj, int entityId)
        {
            return PlanningManager.Instance.UpdateMilestone(this, milestoneTypeID, milstoneName, milestoneObj, entityId);
        }

        /// <summary>
        /// Updates the milestone.
        /// </summary>
        /// <param name="Id">The id.</param>
        /// <param name="Entityid">The entityid.</param>
        /// <param name="Name">The name.</param>
        /// <param name="Description">The description.</param>
        /// <param name="Status">The status.</param>
        /// <param name="DueDate">The due date.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool UpdateMilestone(int Id, int Entityid, string Name, string Description, int Status, DateTimeOffset DueDate)
        {
            return PlanningManager.Instance.UpdateMilestone(this, Id, Entityid, Name, Description, Status, DueDate);
        }
        public void notificationForAddMember(int costcenterid, int entityid)
        {
            PlanningManager.Instance.notificationForAddMember(this, costcenterid, entityid);

        }
        /// <summary>
        /// Deletes the mile stone.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>
        /// bool
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public bool DeleteMileStone(int id)
        {
            return PlanningManager.Instance.DeleteMileStone(this, id);
        }

        /// <summary>
        /// Gets the milestone by id.
        /// </summary>
        /// <param name="id">id.</param>
        /// <returns>
        /// IMilestone
        /// </returns>
        public IList<IAttributeData> GetMilestoneById(int id)
        {
            return PlanningManager.Instance.GetMilestoneById(this, id);
        }

        /// <summary>
        /// Deletes the activity releation type hierachy.
        /// </summary>
        /// <param name="parentactivitytypeid">The parentactivitytypeid.</param>
        /// <param name="childactivitytypeid">The childactivitytypeid.</param>
        /// <param name="sortorder">The sortorder.</param>
        /// <returns>bool</returns>
        public bool DeleteActivityReleationTypeHierachy(int parentactivitytypeid, int childactivitytypeid, int sortorder)
        {
            return PlanningManager.Instance.DeleteActivityReleation(this, parentactivitytypeid, childactivitytypeid, sortorder);
        }

        /// <summary>
        /// Creates the entity color code.
        /// </summary>
        /// <param name="entitytypeid">The entitytypeid.</param>
        /// <param name="colorcode">The colorcode.</param>
        /// <param name="attributeid">The attributeid.</param>
        /// <param name="optionid">The optionid.</param>
        /// <param name="id">The id.</param>
        /// <returns>IEntityColorCode</returns>
        public int CreateEntityColorCode(int entitytypeid, string colorcode, int attributeid, int optionid, int id)
        {
            return PlanningManager.Instance.CreateEntityColorCode(this, entitytypeid, colorcode, attributeid, optionid, id);
        }

        /// <summary>
        /// Gets the entity color code by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>
        /// IEntityColorCode
        /// </returns>
        public IEntityColorCode GetEntityColorCodeById(int id)
        {
            return PlanningManager.Instance.GetEntityColorCodeById(this, id);
        }

        /// <summary>
        /// Deletes the entity color code.
        /// </summary>
        /// <param name="entitycolorcode">The entitycolorcode.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool DeleteEntityColorCode(IEntityColorCode entitycolorcode)
        {
            return PlanningManager.Instance.DeleteEntityColorCode(this, entitycolorcode);
        }

        /// <summary>
        /// Deletes the entity color code.
        /// </summary>
        /// <param name="Id">The id.</param>
        /// <param name="EntityTypeid">The entity typeid.</param>
        /// <param name="Attributeid">The attributeid.</param>
        /// <param name="Optionid">The optionid.</param>
        /// <param name="ColorCode">The color code.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool DeleteEntityColorCode(int Id, int EntityTypeid, int Attributeid, int Optionid, string ColorCode)
        {
            return PlanningManager.Instance.DeleteEntityColorCode(this, Id, EntityTypeid, Attributeid, Optionid, ColorCode);
        }


        /// <summary>
        /// Gets the entityPresentation code by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>
        /// IEntityPresentation
        /// </returns>
        public IEntityPresentation GetPresentationByEntityId(int id)
        {
            return PlanningManager.Instance.GetPresentationByEntityId(this, id);
        }

        /// <summary>
        /// Creates the presentation.
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <param name="PublishedOn">The published on.</param>
        /// <param name="content">The content.</param>
        /// <returns>
        /// IEntityPresentation
        /// </returns>
        public int CreatePresentation(int entityId, DateTimeOffset PublishedOn, int[] entityList, string content = null)
        {
            return PlanningManager.Instance.CreatePresentation(this, entityId, PublishedOn, entityList, content);
        }

        //Financials
        /// <summary>
        /// Adds the cost center.
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <param name="costcenterId">The costcenter id.</param>
        /// <param name="plannedAmount">The planned amount.</param>
        /// <param name="requestedAmount">The requested amount.</param>
        /// <param name="approvedallocatedAmount">The approvedallocated amount.</param>
        /// <param name="approvedBudget">The approved budget.</param>
        /// <param name="commited">The commited.</param>
        /// <param name="spent">The spent.</param>
        /// <param name="approvedbudgetDate">The approvedbudget date.</param>
        /// <returns>
        /// int
        /// </returns>
        public int AddCostCenter(int entityId, int costcenterId, decimal plannedAmount, decimal requestedAmount, decimal approvedallocatedAmount, decimal approvedBudget, decimal commited, decimal spent, DateTimeOffset approvedbudgetDate)
        {
            return PlanningManager.Instance.AddCostCenter(this, entityId, costcenterId, plannedAmount, requestedAmount, approvedallocatedAmount, approvedBudget, commited, spent, approvedbudgetDate);
        }

        /// <summary>
        /// Deletes the cost center.
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <param name="costcenterId">The costcenter id.</param>
        /// <returns>
        /// IFinancial
        /// </returns>
        public bool DeleteCostCenter(int id)
        {
            return PlanningManager.Instance.DeleteCostCenter(this, id);
        }

        /// <summary>
        /// Creates the funding request.
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <param name="costcenterId">The costcenter id.</param>
        /// <param name="money">The money.</param>
        /// <returns>IFundingRequest</returns>
        public int CreateFundingRequest(int entityId, int costcenterId, decimal money, string duedate, string comment)
        {
            return PlanningManager.Instance.CreateFundingRequest(this, entityId, costcenterId, money, duedate, comment);
        }

        /// <summary>
        /// Updates the funding request.
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <param name="costcenterId">The costcenter id.</param>
        /// <param name="state">The state.</param>
        /// <returns>
        /// IFundingRequest
        /// </returns>
        public bool UpdateFundingRequest(int entityId, int costcenterId, int state)
        {
            return PlanningManager.Instance.UpdateFundingRequest(this, entityId, costcenterId, state);
        }

        /// <summary>
        /// Deletes the funding request.
        /// </summary>
        /// <param name="Id">The id.</param>
        /// <returns>
        /// IFundingRequest
        /// </returns>
        public bool DeleteFundingRequest(int Id)
        {
            return PlanningManager.Instance.DeleteFundingRequest(this, Id);
        }

        /// <summary>
        /// get funding request.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="entityId">The entity id.</param>
        /// <returns>IFundingRequests</returns>
        public IList<IFundingRequest> getfundingRequestsByEntityID(int EntityId)
        {
            return PlanningManager.Instance.getfundingRequestsByEntityID(this, EntityId);
        }
        //Objective
        /// <summary>
        /// Creates the objective.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="typeId">The type id.</param>
        /// <param name="name">The name.</param>
        /// <param name="isEnableFeedback">The is enable feedback.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <param name="dateRule">The date rule.</param>
        /// <param name="isMandatory">The is mandatory.</param>
        /// <param name="numeric">The numeric.</param>
        /// <param name="ratings">Additional attributes if any.</param>
        /// <param name="conditions">Additional attributes if any.</param>
        /// <returns>Last inserted Objective ID</returns>
        //public int CreateObjective(int typeId, String name, IObjectiveNumeric numeric, IList<IObjectiveRating> ratings, IList<IObjectiveCondition> conditions,IList<IEntityRoleUser> entityMembers, IList<IAttributeData> entityattributedata)
        //{
        //    return PlanningManager.Instance.CreateObjective(this, typeId, name, numeric, ratings, conditions,entityMembers,entityattributedata);
        //}

        /// <summary>
        /// Creates the objective.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="typeId">The type id.</param>
        /// <param name="name">The name.</param>
        /// <param name="instruction">The instruction.</param>
        /// <param name="isEnableFeedback">The is enable feedback.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <param name="dateRule">The date rule.</param>
        /// <param name="isMandatory">The is mandatory.</param>
        /// <param name="numeric">Additional  attributes if any</param>
        /// <param name="ratings">Additional  attributes if any</param>
        /// <param name="conditions">Additional attributes if any</param>
        /// <returns>Last inserted Objective ID</returns>
        //public int CreateObjective(int typeId, String name, IObjectiveNumeric numeric, IList<IObjectiveRating> ratings, IList<IObjectiveCondition> conditions, IList<IEntityRoleUser> entityMembers, IList<IAttributeData> entityattributedata)
        //{
        //    return PlanningManager.Instance.CreateObjective(this,typeId, name,numeric, ratings, conditions,entityMembers,entityattributedata,null);
        //}

        /// <summary>
        /// Creates the objective.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="id">The id.</param>
        /// <param name="typeId">The type id.</param>
        /// <param name="name">The name.</param>
        /// <param name="objDescription">The Description.</param>
        /// <param name="instruction">The instruction.</param>
        /// <param name="isEnableFeedback">The is enable feedback.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <param name="dateRule">The date rule.</param>
        /// <param name="isMandatory">The is mandatory.</param>
        /// <param name="objNumeric">The objNumeric.</param>
        /// <param name="objRatings">The objRatings.</param>
        /// <param name="ratingObjArr">The Ratings Caption List</param>
        /// <param name="objFullfilConditions">The objFullfilConditions.</param>
        /// <param name="objEntityMembers">The MembersList.</param>
        /// <returns>Last inserted Objective ID</returns>
        public int CreateObjective(int typeId, String name, bool objStatus, string objDescription, string objInstruction, bool objIsEnableFeedback, DateTime objStartDate, DateTime objEndDate, int objDateRule, bool objMandatory, IObjectiveNumeric objNumeric, IObjectiveNumeric objNonNumeric, IList<IObjectiveRating> objRatings, List<string> ratingObjArr, IList<IObjectiveFulfillCondtions> objFullfilConditions, IList<IEntityRoleUser> objEntityMembers)
        {
            return PlanningManager.Instance.CreateObjective(this, typeId, name, objStatus, objDescription, objInstruction, objIsEnableFeedback, objStartDate, objEndDate, objDateRule, objMandatory, objNumeric, objNonNumeric, objRatings, ratingObjArr, objFullfilConditions, objEntityMembers);
        }

        /// <summary>
        /// Updates the objective.
        /// </summary>
        /// <param name="objectiveData">The objective data.</param>
        /// <returns>
        /// IObjective
        /// </returns>
        public bool UpdateObjective(IObjective objectiveData)
        {
            return PlanningManager.Instance.UpdateObjective(this, objectiveData);
        }

        /// <summary>
        /// Deletes the objective.
        /// </summary>
        /// <param name="objective">The objective.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool DeleteObjective(int objectiveId)
        {
            return PlanningManager.Instance.DeleteObjective(this, objectiveId);
        }
        //Objective-Units Creation

        /// <summary>
        /// Creates the units.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="caption">The caption.</param>
        /// <returns>
        /// last inserted id
        /// </returns>
        public int CreateUnits(int id, string caption)
        {
            return PlanningManager.Instance.CreateUnits(this, id, caption);
        }

        /// <summary>
        /// Selects the units by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>
        /// IObjectiveUnit
        /// </returns>
        public IObjectiveUnit SelectUnitsById(int id)
        {
            return PlanningManager.Instance.SelectUnitsById(this, id);
        }

        /// <summary>
        /// Updateunitses the specified units.
        /// </summary>
        /// <param name="units">The units.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool UpdateUnits(IObjectiveUnit units)
        {
            return PlanningManager.Instance.UpdateUnits(this, units);
        }

        //Objective-Units Deletion
        /// <summary>
        /// Deletes the units by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool DeleteUnitsById(int id)
        {
            return PlanningManager.Instance.DeleteUnitsById(this, id);
        }

        //Objective-Ratings

        /// <summary>
        /// Creates the ratings.
        /// </summary>
        /// <param name="objectiveid">The objectiveid.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <returns>
        /// IObjectiveRating
        /// </returns>
        public int CreateRatings(int objectiveid, String caption, int sortOrder)
        {
            return PlanningManager.Instance.CreateRatings(this, objectiveid, caption, sortOrder);
        }

        /// <summary>
        /// Selects the ratings.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>
        /// IObjectiveRating
        /// </returns>
        public IObjectiveRating SelectRatings(int id)
        {
            return PlanningManager.Instance.SelectRatings(this, id);
        }

        /// <summary>
        /// Deletes the ratings.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool DeleteRatings(int id)
        {
            return PlanningManager.Instance.DeleteRatings(this, id);
        }

        /// <summary>
        /// Selects the objective by ID.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns>
        /// IObjective
        /// </returns>
        public IObjective SelectObjectiveByID(int ID)
        {
            return PlanningManager.Instance.SelectObjectiveByID(this, ID);
        }

        //Presentation

        /// <summary>
        /// Updates the presentation.
        /// </summary>
        /// <param name="presentation">The presentation.</param>
        /// <returns>
        /// IEntityPresentation
        /// </returns>
        public bool UpdatePresentation(IEntityPresentation presentation)
        {
            return PlanningManager.Instance.UpdatePresentation(this, presentation);
        }

        /// <summary>
        /// Updates the presentation.
        /// </summary>
        /// <param name="EntityId">The entity id.</param>
        /// <param name="PublishedOn">The published on.</param>
        /// <param name="Content">The content.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool UpdatePresentation(int EntityId, DateTimeOffset PublishedOn, string Content)
        {
            return PlanningManager.Instance.UpdatePresentation(this, EntityId, PublishedOn, Content);
        }

        /// <summary>
        /// Publishes the this level.
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <param name="PublishedOn">The published on.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool PublishThisLevel(int entityId, DateTimeOffset PublishedOn)
        {
            return PlanningManager.Instance.PublishThisLevel(this, entityId, PublishedOn);
        }

        //Attachments

        /// <summary>
        /// Creates the attachments.
        /// </summary>
        /// <param name="Entityid">The entityid.</param>
        /// <param name="Name">The name.</param>
        /// <param name="ActiveVersionNo">The active version no.</param>
        /// <param name="ActiveFileid">The active fileid.</param>
        /// <returns>
        /// last inserted id
        /// </returns>
        public int CreateAttachments(int Entityid, String Name, int ActiveVersionNo, int ActiveFileid)
        {
            return PlanningManager.Instance.CreateAttachments(this, Entityid, Name, ActiveVersionNo, ActiveFileid);
        }

        /// <summary>
        /// Gets the attachments by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>
        /// IList<IAttachments>
        /// </returns>
        public IList<IAttachments> GetAttachmentsById(int id)
        {
            return PlanningManager.Instance.GetAttachmentsById(this, id);
        }

        /// <summary>
        /// Deletes the attachments.
        /// </summary>
        /// <param name="Attachments">The attachments.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool DeleteAttachments(IAttachments Attachments)
        {
            return PlanningManager.Instance.DeleteAttachments(this, Attachments);
        }

        /// <summary>
        /// Deletes the attachments.
        /// </summary>
        /// <param name="Id">The id.</param>
        /// <param name="Entityid">The entityid.</param>
        /// <param name="Name">The name.</param>
        /// <param name="ActiveVersionNo">The active version no.</param>
        /// <param name="ActiveFileid">The active fileid.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool DeleteAttachments(int Id, int Entityid, string Name, int ActiveVersionNo, int ActiveFileid)
        {
            return PlanningManager.Instance.DeleteAttachments(this, Id, Entityid, Name, ActiveVersionNo, ActiveFileid);
        }


        //Entity Period

        /// <summary>
        /// Creates the entity period.
        /// </summary>
        /// <param name="Entityid">The entityid.</param>
        /// <param name="Startdate">The startdate.</param>
        /// <param name="EndDate">The end date.</param>
        /// <param name="Description">The description.</param>
        /// <param name="SortOrder">The sort order.</param>
        /// <returns>
        /// int
        /// </returns>
        public int CreateEntityPeriod(int Entityid, DateTime Startdate, DateTime EndDate, string Description, int SortOrder)
        {
            return PlanningManager.Instance.CreateEntityPeriod(this, Entityid, Startdate, EndDate, Description, SortOrder);
        }

        /// <summary>
        /// Gets the entity period by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>
        /// IEntityPeriod
        /// </returns>
        public IEntityPeriod GetEntityPeriodById(int id)
        {
            return PlanningManager.Instance.GetEntityPeriodById(this, id);
        }

        /// <summary>
        /// Updates the entity period.
        /// </summary>
        /// <param name="EntityPeriod">The entity period.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool UpdateEntityPeriod(IEntityPeriod EntityPeriod)
        {
            return PlanningManager.Instance.UpdateEntityPeriod(this, EntityPeriod);
        }

        /// <summary>
        /// Updates the entity period.
        /// </summary>
        /// <param name="Entityid">The entityid.</param>
        /// <param name="Startdate">The startdate.</param>
        /// <param name="EndDate">The end date.</param>
        /// <param name="Description">The description.</param>
        /// <param name="SortOrder">The sort order.</param>
        /// <returns>
        /// IEntityPeriod
        /// </returns>
        public string UpdateEntityPeriod(DateTime Startdate, DateTime EndDate, string Description, int Id)
        {
            return PlanningManager.Instance.UpdateEntityPeriod(this, Startdate, EndDate, Description, Id);
        }

        /// <summary>
        /// Deletes the entity period.
        /// </summary>
        /// <param name="EntityPeriod">The entity period.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool DeleteEntityPeriod(IEntityPeriod EntityPeriod)
        {
            return PlanningManager.Instance.DeleteEntityPeriod(this, EntityPeriod);
        }

        /// <summary>
        /// Deletes the entity period.
        /// </summary>
        /// <param name="Entityid">The entityid.</param>
        /// <param name="Startdate">The startdate.</param>
        /// <param name="EndDate">The end date.</param>
        /// <param name="Description">The description.</param>
        /// <param name="SortOrder">The sort order.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool DeleteEntityPeriod(int Entityid, DateTime Startdate, DateTime EndDate, string Description, int SortOrder)
        {
            return PlanningManager.Instance.DeleteEntityPeriod(this, Entityid, Startdate, EndDate, Description, SortOrder);
        }

        //Entity Color Code

        /// <summary>
        /// Updates the entity color code.
        /// </summary>
        /// <param name="EntityColorCode">The entity color code.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool UpdateEntityColorCode(IEntityColorCode EntityColorCode)
        {
            return PlanningManager.Instance.UpdateEntityColorCode(this, EntityColorCode);
        }

        /// <summary>
        /// Updates the entity color code.
        /// </summary>
        /// <param name="Id">The id.</param>
        /// <param name="EntityTypeid">The entity typeid.</param>
        /// <param name="Attributeid">The attributeid.</param>
        /// <param name="Optionid">The optionid.</param>
        /// <param name="ColorCode">The color code.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool UpdateEntityColorCode(int Id, int EntityTypeid, int Attributeid, int Optionid, string ColorCode)
        {
            return PlanningManager.Instance.UpdateEntityColorCode(this, Id, EntityTypeid, Attributeid, Optionid, ColorCode);
        }

        //Changes done by rajkumar

        /// <summary>
        /// Notification  milestone update.
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <param name="entityName">Name of the entity.</param>
        /// <param name="entityAttributeId">The entity attribute id.</param>
        /// <param name="entityAttributeOldValue">The entity attribute old value.</param>
        /// <param name="attributeValue">The attribute value.</param>
        /// <param name="parentId">The parent id.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool Notification_MilestoneUpdated(int entityId = 0, string entityName = "", int entityAttributeId = 0, string entityAttributeOldValue = "", string attributeValue = "", int parentId = 0)
        {
            return PlanningManager.Instance.Notification_MilestoneUpdated(this, entityId, entityName, entityAttributeId, entityAttributeOldValue, attributeValue, parentId);
        }

        /// <summary>
        /// Notification milestone delete.
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool Notification_MilestoneDelete(int entityId = 0)
        {
            return PlanningManager.Instance.Notification_MilestoneDelete(this, entityId);
        }

        /// <summary>
        /// Notification milestone create.
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool Notification_MilestoneCreate(int entityId)
        {
            return PlanningManager.Instance.Notification_MilestoneCreate(this, entityId);
        }

        /// <summary>
        /// Notification additional objective create.
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool Notification_AdditionalObjectiveCreate(int entityId)
        {
            return PlanningManager.Instance.Notification_AdditionalObjectiveCreate(this, entityId);
        }

        /// <summary>
        /// Notification cost center add.
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool Notification_CostCenterAdd(int entityId)
        {
            return PlanningManager.Instance.Notification_CostCenterAdd(this, entityId);
        }

        /// <summary>
        /// Notification entity attachment create.
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool Notification_EntityAttachmentCreated(int entityId)
        {
            return PlanningManager.Instance.Notification_EntityAttachmentCreated(this, entityId);
        }

        /// <summary>
        /// Notification entity date insert.
        /// </summary>
        /// <param name="newval">The newval.</param>
        /// <param name="entityId">The entity id.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool Notification_EntityDateInsert(string newval, int entityId)
        {
            return PlanningManager.Instance.Notification_EntityDateInsert(this, newval, entityId);
        }

        /// <summary>
        /// Notification_s the entity date delete.
        /// </summary>
        /// <param name="newval">The newval.</param>
        /// <param name="entityId">The entity id.</param>
        /// <returns></returns>
        public bool Notification_EntityDateDelete(string newval, int entityId)
        {
            return PlanningManager.Instance.Notification_EntityDateDelete(this, newval, entityId);
        }

        /// <summary>
        /// Notification task created.
        /// </summary>
        /// <param name="entitytypename">The entitytypename.</param>
        /// <param name="entityId">The entity id.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool Notification_TaskCreated(string entitytypename, int entityId)
        {
            return PlanningManager.Instance.Notification_TaskCreated(this, entitytypename, entityId);
        }

        /// <summary>
        /// Notification entity create.
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool Notification_EntityCreate(int entityId)
        {
            return PlanningManager.Instance.Notification_EntityCreate(this, entityId);
        }

        /// <summary>
        /// Notification entity update.
        /// </summary>
        /// <param name="oldvalue">The oldvalue.</param>
        /// <param name="newvalue">The newvalue.</param>
        /// <param name="entityId">The entity id.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool Notification_EntityUpdated(string oldvalue, string newvalue, int entityId, string attributeName)
        {
            return PlanningManager.Instance.Notification_EntityUpdated(this, oldvalue, newvalue, entityId, attributeName);
        }

        /// <summary>
        /// Notification entity delete.
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool Notification_EntityDeleted(int entityId)
        {
            return PlanningManager.Instance.Notification_EntityDeleted(this, entityId);
        }

        /// <summary>
        /// Notification entity comment add.
        /// </summary>
        /// <param name="comment">The comment.</param>
        /// <param name="entityId">The entity id.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool Notification_EntityCommentAdded(string comment, int entityId)
        {
            return PlanningManager.Instance.Notification_EntityCommentAdded(this, comment, entityId);
        }

        /// <summary>
        /// Notification task metadata update.
        /// </summary>
        /// <param name="attributename">The attributename.</param>
        /// <param name="oldvalue">The oldvalue.</param>
        /// <param name="newvalue">The newvalue.</param>
        /// <param name="entityId">The entity id.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool Notification_TaskMetadataUpdated(string attributename, string oldvalue, string newvalue, int entityId)
        {
            return PlanningManager.Instance.Notification_TaskMetadataUpdated(this, attributename, oldvalue, newvalue, entityId);
        }

        /// <summary>
        /// Notification task member add.
        /// </summary>
        /// <param name="EntityTypeName">Name of the entity type.</param>
        /// <param name="entityId">The entity id.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool Notification_TaskMemberAdded(string EntityTypeName, int entityId)
        {
            return PlanningManager.Instance.Notification_TaskMemberAdded(this, EntityTypeName, entityId);
        }

        /// <summary>
        /// Notification task status changed.
        /// </summary>
        /// <param name="EntityTypeName">Name of the entity type.</param>
        /// <param name="Entitystate">The entitystate.</param>
        /// <param name="entityId">The entity id.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool Notification_TaskStatusChanged(string EntityTypeName, string Entitystate, int entityId)
        {
            return PlanningManager.Instance.Notification_TaskStatusChanged(this, EntityTypeName, Entitystate, entityId);
        }

        /// <summary>
        /// Notification entity state changed.
        /// </summary>
        /// <param name="oldvalue">The oldvalue.</param>
        /// <param name="newvalue">The newvalue.</param>
        /// <param name="entityId">The entity id.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool Notification_EntityStateChanged(string oldvalue, string newvalue, int entityId)
        {
            return PlanningManager.Instance.Notification_EntityStateChanged(this, oldvalue, newvalue, entityId);
        }

        /// <summary>
        /// Notification entity member add.
        /// </summary>
        /// <param name="users">The users.</param>
        /// <param name="role">The role.</param>
        /// <param name="entityId">The entity id.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool Notification_EntityMemberAdded(string users, string role, int entityId)
        {
            return PlanningManager.Instance.Notification_EntityMemberAdded(this, users, role, entityId);
        }

        /// <summary>
        /// Notification entity attachment delete.
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool Notification_EntityAttachmentDeleted(int entityId)
        {
            return PlanningManager.Instance.Notification_EntityAttachmentDeleted(this, entityId);
        }

        /// <summary>
        /// Notification entity member role update.
        /// </summary>
        /// <param name="users">The users.</param>
        /// <param name="oldvalue">The oldvalue.</param>
        /// <param name="newvalue">The newvalue.</param>
        /// <param name="entityId">The entity id.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool Notification_EntityMemberRoleUpdated(string users, string oldvalue, string newvalue, int entityId)
        {
            return PlanningManager.Instance.Notification_EntityMemberRoleUpdated(this, users, oldvalue, newvalue, entityId);
        }

        /// <summary>
        /// Notification entity member removed.
        /// </summary>
        /// <param name="users">The users.</param>
        /// <param name="entityId">The entity id.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool Notification_EntityMemberRemoved(string users, int entityId)
        {
            return PlanningManager.Instance.Notification_EntityMemberRemoved(this, users, entityId);
        }

        /// <summary>
        /// Notification entity duplicated.
        /// </summary>
        /// <param name="entitytypename">The entitytypename.</param>
        /// <param name="countofnos">The countofnos.</param>
        /// <param name="sublevels">The sublevels.</param>
        /// <param name="entityId">The entity id.</param>
        /// <param name="entityName">Name of the entity.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool Notification_EntityDuplicated(string entitytypename, string countofnos, string sublevels, int entityId, string entityName)
        {
            return PlanningManager.Instance.Notification_EntityDuplicated(this, entitytypename, countofnos, sublevels, entityId, entityName);
        }

        /// <summary>
        /// Notification fund request created.
        /// </summary>
        /// <param name="RequestedAmount">The requested amount.</param>
        /// <param name="CostcenterName">Name of the costcenter.</param>
        /// <param name="entityId">The entity id.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool Notification_FundRequestCreated(float RequestedAmount, string CostcenterName, int entityId)
        {
            return PlanningManager.Instance.Notification_FundRequestCreated(this, RequestedAmount, CostcenterName, entityId);
        }

        /// <summary>
        /// Notification released funds.
        /// </summary>
        /// <param name="ReleaseAmount">The release amount.</param>
        /// <param name="PathTemplate">The path template.</param>
        /// <param name="CostcenterName">Name of the costcenter.</param>
        /// <param name="entityId">The entity id.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool Notification_ReleasedFunds(float ReleaseAmount, string PathTemplate, string CostcenterName, int entityId)
        {
            return PlanningManager.Instance.Notification_ReleasedFunds(this, ReleaseAmount, PathTemplate, CostcenterName, entityId);
        }

        /// <summary>
        /// Notification costcenter assigned amount changed.
        /// </summary>
        /// <param name="oldval">The oldval.</param>
        /// <param name="newval">The newval.</param>
        /// <param name="CostcenterName">Name of the costcenter.</param>
        /// <param name="entityId">The entity id.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool Notification_CostcenterAssignedAmountChanged(string oldval, string newval, string CostcenterName, int entityId)
        {
            return PlanningManager.Instance.Notification_CostcenterAssignedAmountChanged(this, oldval, newval, CostcenterName, entityId);
        }

        /// <summary>
        /// Notification entity plan budget updated.
        /// </summary>
        /// <param name="oldval">The oldval.</param>
        /// <param name="newval">The newval.</param>
        /// <param name="entityId">The entity id.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool Notification_EntityPlanBudgetUpdated(string oldval, string newval, int entityId)
        {
            return PlanningManager.Instance.Notification_EntityPlanBudgetUpdated(this, oldval, newval, entityId);
        }

        /// <summary>
        /// Notification entity approved allocated updated.
        /// </summary>
        /// <param name="oldval">The oldval.</param>
        /// <param name="newval">The newval.</param>
        /// <param name="entityId">The entity id.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool Notification_EntityApprovedAllocatedUpdated(string oldval, string newval, int entityId)
        {
            return PlanningManager.Instance.Notification_EntityApprovedAllocatedUpdated(this, oldval, newval, entityId);
        }

        /// <summary>
        /// Notification funding request deleted.
        /// </summary>
        /// <param name="costcentername">The costcentername.</param>
        /// <param name="entityId">The entity id.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool Notification_FundingRequestDeleted(string costcentername, int entityId)
        {
            return PlanningManager.Instance.Notification_FundingRequestDeleted(this, costcentername, entityId);
        }

        /// <summary>
        /// Notification funding request statechanged.
        /// </summary>
        /// <param name="FundingRequestState">State of the funding request.</param>
        /// <param name="CostcenterName">Name of the costcenter.</param>
        /// <param name="entityId">The entity id.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool Notification_FundingRequestStatechanged(string FundingRequestState, string CostcenterName, int entityId)
        {
            return PlanningManager.Instance.Notification_FundingRequestStatechanged(this, FundingRequestState, CostcenterName, entityId);
        }

        /// <summary>
        /// Notification  cost center deleted.
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <param name="entityName">Name of the entity.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool Notification_CostCenterDeleted(int entityId, string entityName)
        {
            return PlanningManager.Instance.Notification_CostCenterDeleted(this, entityId, entityName);
        }

        /// <summary>
        /// Notification  money transferred.
        /// </summary>
        /// <param name="Amount">The amount.</param>
        /// <param name="FromCostcenterName">Name of from costcenter.</param>
        /// <param name="ToCostCenterName">Name of to cost center.</param>
        /// <param name="entityId">The entity id.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool Notification_MoneyTransferred(float Amount, string FromCostcenterName, string ToCostCenterName, int entityId)
        {
            return PlanningManager.Instance.Notification_MoneyTransferred(this, Amount, FromCostcenterName, ToCostCenterName, entityId);
        }

        /// <summary>
        /// Notification insert cost center.
        /// </summary>
        /// <param name="CostcenterName">Name of the costcenter.</param>
        /// <param name="entityId">The entity id.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool Notification_InsertCostCenter(string CostcenterName, int entityId)
        {
            return PlanningManager.Instance.Notification_InsertCostCenter(this, CostcenterName, entityId);
        }

        /// <summary>
        /// Notification enable disable workflow.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="entityId">The entity id.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool Notification_EnableDisableWorkflow(string state, int entityId)
        {
            return PlanningManager.Instance.Notification_EnableDisableWorkflow(this, state, entityId);
        }

        /// <summary>
        /// Notification entity commit budget updated.
        /// </summary>
        /// <param name="OldValue">The old value.</param>
        /// <param name="NewValue">The new value.</param>
        /// <param name="entityId">The entity id.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool Notification_EntityCommitBudgetUpdated(string OldValue, string NewValue, int entityId)
        {
            return PlanningManager.Instance.Notification_EntityCommitBudgetUpdated(this, OldValue, NewValue, entityId);
        }

        /// <summary>
        /// Notification entity spent budget updated.
        /// </summary>
        /// <param name="OldValue">The old value.</param>
        /// <param name="NewValue">The new value.</param>
        /// <param name="entityId">The entity id.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool Notification_EntitySpentBudgetUpdated(string OldValue, string NewValue, int entityId)
        {
            return PlanningManager.Instance.Notification_EntitySpentBudgetUpdated(this, OldValue, NewValue, entityId);
        }

        /// <summary>
        /// Notification cost center approved budget updated.
        /// </summary>
        /// <param name="CostcenterName">Name of the costcenter.</param>
        /// <param name="entityId">The entity id.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool Notification_CostCenterApprovedBudgetUpdated(string CostcenterName, int entityId)
        {
            return PlanningManager.Instance.Notification_CostCenterApprovedBudgetUpdated(this, CostcenterName, entityId);
        }

        /// <summary>
        /// Gets the tree node.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <returns>
        /// List of ITreeNode
        /// </returns>
        public string GetEntitydescendants(int attributeID)
        {
            return PlanningManager.Instance.GetEntitydescendants(this, attributeID);
        }

        /// <summary>
        /// Creates the entity.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="id">The id.</param>
        /// <param name="parentId">The parent id.</param>
        /// <param name="typeId">The type id.</param>
        /// <param name="active">The active.</param>
        /// <param name="uniqueKey">The unique key.</param>
        /// <param name="name">The name.</param>
        /// <param name="entityMembers">The entity members.</param>
        /// <param name="entityCostcenters">The entity costcenters.</param>
        /// <param name="periods">The periods.</param>
        /// <returns>Lastinserted Entity Id value</returns>
        public int CreateEntity(int parentId, int typeId, Boolean active, String name, IList<IEntityRoleUser> entityMembers, IList<IEntityCostReleations> entityCostcenters, IList<IEntityPeriod> periods)
        {
            return PlanningManager.Instance.CreateEntity(this, parentId, typeId, active, name, entityMembers, entityCostcenters, periods);
        }
        /// <summary>
        /// Creates the entity.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="id">The id.</param>
        /// <param name="parentId">The parent id.</param>
        /// <param name="typeId">The type id.</param>
        /// <param name="active">The active.</param>
        /// <param name="uniqueKey">The unique key.</param>
        /// <param name="IsLock"> The IsLock</param>
        /// <param name="name">The name.</param>
        /// <param name="entityMembers">The entity members.</param>
        /// <param name="entityCostcenters">The entity costcenters.</param>
        /// <param name="presentation">The presentation.</param>
        /// <param name="periods">The periods.</param>
        /// <returns>Lastinserted Entity Id value</returns>
        public int CreateEntity(int parentId, int typeId, Boolean active, Boolean isLock, String name, IList<IEntityRoleUser> entityMembers, IList<IEntityCostReleations> entityCostcenters, IList<IEntityPeriod> periods)
        {
            return PlanningManager.Instance.CreateEntity(this, parentId, typeId, active, isLock, name, entityMembers, entityCostcenters, periods);
        }

        /// <summary>
        /// Creates the Entity
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="parentId">The parent id.</param>
        /// <param name="typeId">The type id.</param>
        /// <param name="active">The active.</param>
        /// <param name="uniqueKey">The unique key.</param>
        /// <param name="isLock">The is lock.</param>
        /// <param name="name">The name.</param>
        /// <param name="entityMembers">The entity members.</param>
        /// <param name="entityObjectvalues">The entity ObjectiveEntityValues.</param>
        /// <param name="entityCostcenters">The entity costcenters.</param>
        /// <param name="periods">The periods.</param>
        /// <param name="attributes"> The attributes</param>
        /// <returns>Lastinserted Entity Id value</returns>
        public int CreateEntity(int parentId, int typeId, Boolean active, Boolean isLock, string name, IList<IEntityRoleUser> entityMembers, IList<IEntityCostReleations> entityCostcentres, IList<IEntityPeriod> entityPeriods, IList<IFundingRequest> listFundrequest, IList<IAttributeData> entityattributedata, int[] assetIdArr = null, IList<IObjectiveEntityValue> entityObjectvalues = null, IList<object> attributes = null)
        {
            return PlanningManager.Instance.CreateEntity(this, parentId, typeId, active, isLock, name, entityMembers, entityCostcentres, entityPeriods, listFundrequest, entityattributedata, assetIdArr, entityObjectvalues, attributes);
        }
        /// <summary>
        /// Creates the Entity.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="id">The id.</param>
        /// <param name="parentId">The parent id.</param>
        /// <param name="typeId">The type id.</param>
        /// <param name="active">The active.</param>
        /// <param name="uniqueKey">The unique key.</param>
        /// <param name="isLock">The is lock.</param>
        /// <param name="name">The name.</param>
        /// <param name="attributes"> The attributes</param>
        /// <returns>Lastinserted Entity Id value</returns>
        public int CreateFundRequest(int parentId, int typeId, Boolean active, Boolean isLock, string name, IList<IFundingRequest> listFundrequest, IList<IFundingRequestHolder> entityattributedata)
        {
            return PlanningManager.Instance.CreateFundRequest(this, parentId, typeId, active, isLock, name, listFundrequest, entityattributedata);
        }
        /// <summary>
        /// Selecting all children and parent entities based on unique-key.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="typeId">The type id.</param>
        /// <returns>
        ///IList<IEntity>
        /// </returns>
        public IList<IEntity> SelectAllchildeEtities(int id)
        {
            return PlanningManager.Instance.SelectAllchildeEtities(this, id);
        }

        /// <summary>
        /// Selecting only particular Entity.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="typeId">The type id.</param>
        /// <returns>
        ///IEntity
        /// </returns>
        public IEntity SelectEntityByID(int id)
        {
            return PlanningManager.Instance.SelectEntityByID(this, id);
        }

        /// <summary>
        /// Updates the Entity.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="entitydata">The entitydata.</param>
        /// <returns>True (or) False</returns>
        public bool UpdateEntity(IEntity entitydata)
        {
            return PlanningManager.Instance.UpdateEntity(this, entitydata);
        }

        /// <summary>
        /// Creating Costcentre entity.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="id">The id.</param>
        /// <param name="name">The Name.</param>
        /// <param name="assignedAmount">The Assignedamount for costcentre.</param>
        /// <param name="entityattributedata">The IList<IAttributeData> AttributeData</param>
        /// <param name="entityMembers">The EntityMembers.</param>
        /// <returns>Last Inserted Costcentre ID</returns>
        public int CreateCostcentre(int typeId, string name, int assignedAmount, IList<IAttributeData> entityattributedata, IList<IEntityRoleUser> entityMembers)
        {
            return PlanningManager.Instance.CreateCostcentre(this, typeId, name, assignedAmount, entityattributedata, entityMembers);
        }

        /// <summary>
        /// Getting Costcentre 
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="id">The id.</param>
        /// <returns>ICostCentreData</returns>
        public ICostCentreData GetCostcentre(int id)
        {
            return PlanningManager.Instance.GetCostcentre(this, id);
        }

        /// <summary>
        /// Getting Costcentre 
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="id">The id.</param>
        /// <returns>ICostCentreData</returns>
        public IList GetCostcentreforEntityCreation(int EntityTypeID, int fiscalyear = 0, int entityid = 0)
        {
            return PlanningManager.Instance.GetCostcentreforEntityCreation(this, EntityTypeID, fiscalyear, entityid);
        }

        /// <summary>
        /// Getting Member for Entity Creation
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="id">The entityid.</param>
        /// <returns>IMemberData</returns>
        public IList GetGlobalMembers(int entityid = 0)
        {
            return PlanningManager.Instance.GetGlobalMembers(this, entityid);
        }

        /// <summary>
        /// add Costcentre 
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="entityId">The entityid.</param>
        /// <param name="entityCostcentres">The IEntityCostReleations.</param>
        /// <returns>ICostCentreData</returns>
        public bool AddCostCenterForFinancial(int entityId, IList<IEntityCostReleations> entityCostcentres, bool isForceful)
        {
            return PlanningManager.Instance.AddCostCenterForFinancial(this, entityId, entityCostcentres, isForceful);
        }

        /// <summary>
        /// Getting Costcentre for Financial
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="id">The id.</param>
        /// <returns>ICostCentreData</returns>
        public IList GetCostcentreforFinancial(int entityid = 0)
        {
            return PlanningManager.Instance.GetCostcentreforFinancial(this, entityid);
        }

        /// <summary>
        /// Getting Entity Financial Details 
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="id">The entityid.</param>
        /// <returns>IList</returns>
        public Tuple<IList, IList, IList, IList, IList, int, IList<IFinancialMetadataAttributewithValues>, Tuple<List<object>>> GetEntityFinancialdDetails(int entityid, int userID, int startRow, int endRow, bool includedetails)
        {
            return PlanningManager.Instance.GetEntityFinancialdDetails(this, entityid, userID, startRow, endRow, includedetails);
        }

        /// <summary>
        /// Getting Cost Center Amount  Details 
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="id">The costCenterId.</param>
        /// <returns>IList</returns>
        public IList GetCostcenterBeforeApprovalAmountDetails(int costCenterId, int entityId)
        {
            return PlanningManager.Instance.GetCostcenterBeforeApprovalAmountDetails(this, costCenterId, entityId);
        }

        /// <summary>
        /// Update Planned Amount in Financial
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="id">The entityid.</param>
        /// <param name="CostCenterId">The CostCenter ID</param>
        /// <param name="Amount">Planned Amount</param>
        /// <returns>Bool</returns>
        public bool EntityPlannedAmountInsert(int entityID, int CostcenterId, Decimal PlannedAmount, int currencyType, string description)
        {
            return PlanningManager.Instance.EntityPlannedAmountInsert(this, entityID, CostcenterId, PlannedAmount, currencyType, description);
        }

        /// <summary>
        /// Update Approve Planned Amount in Financial
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="id">The entityid.</param>
        /// <param name="CostCenterId">The CostCenter ID</param>
        /// <param name="Amount">Planned Amount</param>
        /// <returns>Bool</returns>
        public bool EntityApprovePlannedAmountInsert(int entityID, int CostcenterId, Decimal AvailableAmount, Decimal PlannedAmount, Decimal EntityApprovePlannedAmountInsert, int currencyType)
        {
            return PlanningManager.Instance.EntityApprovePlannedAmountInsert(this, entityID, CostcenterId, AvailableAmount, PlannedAmount, EntityApprovePlannedAmountInsert, currencyType);
        }

        /// <summary>
        /// AdjustApprove Allocation Amount
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="id">The entityid.</param>
        /// <param name="Amount">Approve Planned Updation</param>
        /// <returns>Bool</returns>
        public bool AdjustApproveAllocation(int entityID)
        {
            return PlanningManager.Instance.AdjustApproveAllocation(this, entityID);
        }

        /// <summary>
        /// Update Approved Allocated Amount in Financial
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="id">The entityid.</param>
        /// <param name="CostCenterId">The CostCenter ID</param>
        /// <param name="Amount">Release Amount</param>
        /// <returns>True (or) False</returns>
        public bool ReleaseFund(int entityID, int CostcenterId, Decimal ReleaseAmount)
        {
            return PlanningManager.Instance.ReleaseAmount(this, entityID, CostcenterId, ReleaseAmount);
        }

        /// <summary>
        /// Update Status  in Financial
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="id">The entityid.</param>
        /// <param name="CostCenterId">The CostCenter ID</param>
        /// <param name="Status">the Status</param>
        /// <returns>Bool</returns>
        public bool UpdateFundRequestStatus(int entityID, int CostcenterId, int FundRequestID, int status)
        {
            return PlanningManager.Instance.UpdateFundRequestStatus(this, entityID, CostcenterId, FundRequestID, status);
        }

        /// <summary>
        /// Update Request Amount in Financial
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="id">The entityid.</param>
        /// <param name="CostCenterId">The CostCenter ID</param>
        /// <param name="Amount">Request Amount</param>
        /// <returns>Bool</returns>
        public bool EntityRequestAmountInsert(int entityID, int CostcenterId, Decimal RequestAmount)
        {
            return PlanningManager.Instance.EntityRequestAmountInsert(this, entityID, CostcenterId, RequestAmount);
        }

        /// <summary>
        /// Transfer Money from one entityCostcenter into another costcenter in Financial
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="id">The entityid.</param>
        /// <param name="CostCenterId">The FromCostCenter ID</param>
        /// <param name="CostCenterId">The ToCostCenter ID</param>
        /// <param name="Amount">Transfer Amount</param>
        /// <returns>True (or) False</returns>
        public bool EntityMoneyTransfer(int entityID, int FromCostcenterId, int ToCostCenterId, Decimal TransferAmount)
        {
            return PlanningManager.Instance.EntityMoneyTransfer(this, entityID, FromCostcenterId, ToCostCenterId, TransferAmount);
        }
        /// <summary>
        /// Updating Costcentre 
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="costcentredata">The CostcentreData.</param>
        /// <returns>True (or) False</returns>
        public bool UpdateCostcentre(ICostCentreData costcentredata)
        {
            return PlanningManager.Instance.UpdateCostcentre(this, costcentredata);
        }
        /// <summary>
        /// Deleting Costcentre entity.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="costcenterId">The CostcentreID.</param>
        /// <returns>True (or) False</returns>
        public bool DeleteCostcentreentity(int costcenterId)
        {
            return PlanningManager.Instance.DeleteCostcentreentity(this, costcenterId);
        }
        /// <summary>
        /// Deleting Costcentre Relation in Financial.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="id">The Entityid.</param>
        /// <param name="id">The CostCenterid.</param>
        /// <returns>true (or) False</returns>
        public bool DeleteCostcentreFinancial(int entityID, int costcenterId)
        {
            return PlanningManager.Instance.DeleteCostcentreFinancial(this, entityID, costcenterId);
        }
        /// <summary>
        /// Creating Objective entityvalues.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="objectiveId">The ObjectiveId.</param>
        /// <param name="entityId">The EntityId.</param>
        /// <param name="plannedTarget">The PlannedTarget.</param>
        /// <param name="targetOutcome">The TargetOutcome.</param>
        /// <param name="ratingObjective">The RatingObjective.</param>
        /// <param name="comments">The Comments.</param>
        /// <param name="status">The Status.</param>
        /// <param name="fullfilment">The Fulfilment.</param>
        /// <returns>IObjectiveEntityValue</returns>
        public IObjectiveEntityValue Objectiveentityvalues(int objectiveId, int entityId, int plannedTarget, int targetOutcome, int ratingObjective, string comments, int status, int fullfilment)
        {
            return PlanningManager.Instance.Objectiveentityvalues(this, objectiveId, entityId, plannedTarget, targetOutcome, ratingObjective, comments, status, fullfilment);
        }
        /// <summary>
        /// Creating Objective Condition.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="objCondition">The TotalObjectiveCondition.</param>
        /// <returns>IList<IObjectiveCondition></returns>
        public IList<IObjectiveCondition> ObjectiveConditionvalues(IList<IObjectiveCondition> objCondition)
        {
            return PlanningManager.Instance.ObjectiveConditionvalues(this, objCondition);
        }

        /// <summary>
        /// Getting EntityAttribute values By Entityname
        /// </summary>
        /// <param name="entityTypeId">The EntityTypeID</param>
        /// <returns>IList<IDynamicAttributes></returns>
        public IList<IDynamicAttributes> GetEntityAttributes(int entityId)
        {
            return PlanningManager.Instance.GetEntityAttributes(this, entityId);
        }
        /// <summary>
        /// Inserting EntityAttribute values By Entityname
        /// </summary>
        /// <param name="entityTypeId">The EntityTypeID</param>
        /// <returns>Last inserted Id</returns>
        public int InsertEntityAttributes(IList<IAttributeData> attributes, int entityTypeId)
        {
            return PlanningManager.Instance.InsertEntityAttributes(this, attributes, entityTypeId);
        }

        /// <summary>
        /// Deleting Entity
        /// </summary>
        /// <param name="entityId">The EntityID</param>
        /// <returns>True (or) False</returns>
        public bool DeleteEntity(int entityId)
        {
            return PlanningManager.Instance.DeleteEntity(this, entityId);
        }

        #region Instance of Classes In ServiceLayer reference
        /// <summary>
        /// Returns EntityRolesUser class.
        /// </summary>
        public IEntityRoleUser Entityrolesservice()
        {
            return PlanningManager.Instance.Entityrolesservice();
        }

        /// <summary>
        /// Returns PurchaseOrder class.
        /// </summary>
        public IPurchaseOrder PurchaseOrderservice()
        {
            return PlanningManager.Instance.PurchaseOrderservice();
        }

        /// <summary>
        /// Returns PurchaseOrderDetail class.
        /// </summary>
        public IPurchaseOrderDetail PurchaseOrderDetailservice()
        {
            return PlanningManager.Instance.PurchaseOrderDetailservice();
        }


        /// <summary>
        /// Returns PurchaseOrder class.
        /// </summary>
        public IInvoice Invoiceservice()
        {
            return PlanningManager.Instance.Invoiceservice();
        }

        /// <summary>
        /// Returns PurchaseOrderDetail class.
        /// </summary>
        public IInvoiceDetail InvoiceDetailservice()
        {
            return PlanningManager.Instance.InvoiceDetailservice();
        }
        /// <summary>
        /// Returns EntityCostcentrerelation class.
        /// </summary>
        public IEntityCostReleations EntityCostcentrerelationservice()
        {
            return PlanningManager.Instance.EntityCostcentrerelationservice();
        }

        /// <summary>
        /// Returns Supplier class.
        /// </summary>
        public ISupplier Supplierservice()
        {
            return PlanningManager.Instance.Supplierservice();
        }
        /// <summary>
        /// Returns Entitypresentation class.
        /// </summary>
        public IEntityPresentation Entitypresentationservice()
        {
            return PlanningManager.Instance.Entitypresentationservice();
        }
        /// <summary>
        /// Returns Entityperiod class.
        /// </summary>
        public IEntityPeriod Entityperiodservice()
        {
            return PlanningManager.Instance.Entityperiodservice();
        }
        /// <summary>
        /// Returns Financial class.
        /// </summary>
        public IFinancial Entityfinanicalservice()
        {
            return PlanningManager.Instance.Entityfinanicalservice();
        }
        /// <summary>
        /// Returns FundingRequest class.
        /// </summary>
        public IFundingRequest EntityFundingrequestservice()
        {
            return PlanningManager.Instance.EntityFundingrequestservice();
        }
        /// <summary>
        /// Returns Objective class.
        /// </summary>
        public IObjective Objectiveservice()
        {
            return PlanningManager.Instance.Objectiveservice();
        }
        /// <summary>
        /// Returns Costcentre class.
        /// </summary>
        public ICostCenter Costcentreservice()
        {
            return PlanningManager.Instance.Costcentreservice();
        }
        /// <summary>
        /// Returns AttributeData class.
        /// </summary>
        public IAttributeData AttributeDataservice()
        {
            return PlanningManager.Instance.AttributeDataservice();
        }

        public IFundingRequestHolder FundingRequestHolderservice()
        {
            return PlanningManager.Instance.FundingRequestHolderservice();
        }
        /// <summary>
        /// Returns ObjectiveNumeric class.
        /// </summary>
        public IObjectiveNumeric ObjNumericservice()
        {
            return PlanningManager.Instance.ObjNumericservice();
        }
        /// <summary>
        /// Returns ObjectiveRating class.
        /// </summary>
        public IObjectiveRating ObjRatingservice()
        {
            return PlanningManager.Instance.ObjRatingservice();
        }
        /// <summary>
        /// Returns ObjectiveCondition class.
        /// </summary>
        public IObjectiveCondition Objectiveconditionservice()
        {
            return PlanningManager.Instance.Objectiveconditionservice();
        }
        /// <summary>
        /// Returns ObjectiveEntityValues class.
        /// </summary>
        public IObjectiveEntityValue ObjEnityvalservice()
        {
            return PlanningManager.Instance.ObjEnityvalservice();
        }
        /// <summary>
        /// Returns BaseEntity class.
        /// </summary>
        public IBaseEntity Baseentityservice()
        {
            return PlanningManager.Instance.Baseentityservice();
        }
        public ICostCentreData CostcentreDataservice()
        {
            return PlanningManager.Instance.CostcentreDataservice();
        }
        public IEntity EntityService()
        {
            return PlanningManager.Instance.EntityService();
        }

        /// <summary>
        /// Returns FilterSettingsValues class.
        /// </summary>
        public IFiltersettingsValues FilterSettingsValuesService()
        {
            return PlanningManager.Instance.FilterSettingsValuesService();
        }

        /// <summary>
        /// Returns IObjectiveFulfillCondition class.
        /// </summary>
        public IObjectiveFulfillCondtions ObjectiveFulfillmentCondtionValuesService()
        {
            return PlanningManager.Instance.ObjectiveFulfillmentCondtionValuesService();
        }



        /// <summary>
        /// Returns IObjectiveFulfillCondition class.
        /// </summary>
        public ICalenderFulfillCondtions CalenderFulfillmentCondtionValuesService()
        {
            return PlanningManager.Instance.CalenderFulfillmentCondtionValuesService();
        }
        /// <summary>
        /// Returns Task class.
        /// </summary>
        public ITask TasksService()
        {
            return PlanningManager.Instance.TasksService();
        }

        // <summary>
        /// Returns TaskAttachment class.
        /// </summary>
        public ITaskAttachment TasksAttachmentService()
        {
            return PlanningManager.Instance.TasksAttachmentService();
        }
        #endregion

        /// <summary>
        /// Updating EntityAttribute values
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="IList<IAttributeData> attributes">The AttributesData</param>
        /// <param name="enityId">The EnityID</param>
        /// <returns>True (or) False</returns>
        public bool UpdateAttributeData(IList<IAttributeData> attributes, int entityId)
        {
            return PlanningManager.Instance.UpdateAttributeData(this, attributes, entityId);
        }
        /// <summary>
        /// Gets the attributes details by entityID.
        /// </summary>
        /// <param name="Id">The entityId.</param>
        /// <returns>
        /// Ilist
        /// </returns>
        public IList<IAttributeData> GetEntityAttributesDetails(int id)
        {
            return PlanningManager.Instance.GetEntityAttributesDetails(this, id);
        }

        public IList Get_EntityIDs(int ActivityListID, int CostCenterID = 0, int ObjectiveID = 0, bool IsGlobalAdmin = false, int FilterID = 0, string PublishDate = null, int UserID = 0)
        {
            return PlanningManager.Instance.Get_EntityIDs(this, ActivityListID, CostCenterID, ObjectiveID, IsGlobalAdmin, FilterID, PublishDate, UserID);
        }

        /// <summary>
        /// GetChildTreeNodes.
        /// </summary>
        /// <param name="Id">The ParentID.</param>
        /// <returns>
        /// IList
        /// </returns>
        public IList GetChildTreeNodes(int ParentID)
        {
            return PlanningManager.Instance.GetChildTreeNodes(this, ParentID);
        }

        /// <summary>
        /// GetParentTreeNodes.
        /// </summary>
        /// <param name="IdArr">The IdArr.</param>
        /// <returns>
        /// IList
        /// </returns>
        public IList GetParentTreeNodes(int[] IdArr)
        {
            return PlanningManager.Instance.GetParentTreeNodes(this, IdArr);
        }

        /// <summary>
        /// Inserting FilterSettings values for ActivityListLevel
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="filterName">The FilterName</param>
        /// <param name="keyword">The Keyword</param>
        /// <param name="userId">The UserID</param>
        /// <param name="entityTypeId">The EntityTypeID</param>
        /// <param name="startDate">The StartDate</param>
        /// <param name="endDate">The EndDate</param>
        /// <param name="whereCondition">The WhereCondition</param>
        /// <returns>int</returns>
        public int InsertFilterSettings(string filterName, string keyword, int userId, int typeId, string entityTypeId, int IsDetailFilter, string startDate, string endDate, string whereCondition, IList<IFiltersettingsValues> filterAttributes, int filterId = 0)
        {
            return PlanningManager.Instance.InsertFilterSettings(this, filterName, keyword, userId, typeId, entityTypeId, IsDetailFilter, startDate, endDate, whereCondition, filterAttributes, filterId);
        }

        /// <summary>
        /// GetFilterSettings.
        /// </summary>
        /// <returns>
        /// IList<FilterSettings>
        /// </returns>
        public IList<IFilterSettings> GetFilterSettings(int typeId)
        {
            return PlanningManager.Instance.GetFilterSettings(this, typeId);
        }
        public IList GetApprovedBudgetDate(string ListId)
        {
            return PlanningManager.Instance.GetApprovedBudgetDate(this, ListId);
        }

        public IList<IFilterSettings> GetFilterSettingsForDetail(int typeId)
        {
            return PlanningManager.Instance.GetFilterSettingsForDetail(this, typeId);
        }
        /// <summary>
        /// GetFilterSettingsValues.
        /// </summary>
        /// <returns>
        /// IFilterSettings
        /// </returns>
        public IFilterSettings GetFilterSettingValuesByFilertId(int filterId)
        {
            return PlanningManager.Instance.GetFilterSettingValuesByFilertId(this, filterId);
        }

        /// <summary>
        /// Get member.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns>IList of IEntityRoleUser</returns>
        public IList<IEntityRoleUser> GetMember(int EntityID)
        {
            return PlanningManager.Instance.GetMember(this, EntityID);
        }

        /// <summary>
        /// GetFundrequestTaskMember.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns>IList of IEntityRoleUser</returns>
        public IList<ITaskMember> GetFundrequestTaskMember(int EntityID)
        {
            return PlanningManager.Instance.GetFundrequestTaskMember(this, EntityID);
        }

        /// <summary>
        /// Updating member
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="int EntityID">The EntityID</param>
        /// <param name="int RoleID">The RoleID</param>
        /// <param name="int Assignee">The Assignee</param>
        /// <returns>int</returns>
        public int InsertMember(int EntityID, int RoleID, int Assignee, bool IsInherited, int InheritedFromEntityid)
        {
            return PlanningManager.Instance.InsertMember(this, EntityID, RoleID, Assignee, IsInherited, InheritedFromEntityid);
        }
        /// <summary>
        /// Deleting Entity
        /// </summary>
        /// <param name="entityId">The EntityID</param>
        /// <param name="Assignee">The Assignee</param>
        /// <returns>True (or) False</returns>
        public bool DeleteMember(int ID)
        {
            return PlanningManager.Instance.DeleteMember(this, ID);
        }

        /// <summary>
        /// Updating member
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="int EntityID">The EntityID</param>
        /// <param name="int RoleID">The RoleID</param>
        /// <param name="int Assignee">The Assignee</param>
        /// <returns>True (or) False</returns>
        public bool UpdateMember(int ID, int EntityID, int RoleID, int Assignee, bool IsInherited, int InheritedFromEntityid, bool IsPlanEntity = false)
        {
            return PlanningManager.Instance.UpdateMember(this, ID, EntityID, RoleID, Assignee, IsInherited, InheritedFromEntityid, IsPlanEntity);
        }

        /// <summary>
        /// Getting All Milestones based on EntityId.
        /// </summary>
        /// <param name="entityId">The EntityId</param>
        /// <param name="entitytypeId">The MileStoneTypeId</param>
        /// <returns>IList of IMilestoneMetadata</returns>
        public IList<IMilestoneMetadata> GetMilestoneMetadata(int entityId, int entitytypeId)
        {
            return PlanningManager.Instance.GetMilestoneMetadata(this, entityId, entitytypeId);
        }

        /// <summary>
        /// Getting All Milestones based on EntityId.
        /// </summary>
        /// <param name="entityId">The EntityId</param>
        /// <param name="entitytypeId">The MileStoneTypeId</param>
        /// <returns>IList of IMilestoneMetadata</returns>
        public IList<IMilestoneMetadata> GetMilestoneforWidget(int entityId, int entitytypeId)
        {
            return PlanningManager.Instance.GetMilestoneforWidget(this, entityId, entitytypeId);
        }

        /// <summary>
        /// Delete FilterSettings and Values.
        /// </summary>
        /// <returns>
        /// True or False
        /// </returns>
        public bool DeleteFilterSettings(int filterId)
        {
            return PlanningManager.Instance.DeleteFilterSettings(this, filterId);
        }

        /// <summary>
        /// Get Entity Period.
        /// </summary>
        /// <param name="EntityID">The EntityID.</param>
        /// <returns>IList of IEntityPeriod</returns>
        public IList<IEntityPeriod> GetEntityPeriod(int EntityID)
        {
            return PlanningManager.Instance.GetEntityPeriod(this, EntityID);
        }

        /// <summary>
        /// Inserting Entity Period
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="int EntityID">The EntityID</param>
        /// <param name="string StartDate">The StartDate</param>
        /// <param name="string EndDate">The EndDate</param>
        /// <param name="int SortOrder">The SortOrder</param>
        /// <param name="string Description">The Description</param>
        /// <returns>int</returns>
        public int InsertEntityPeriod(int EntityID, string StartDate, string EndDate, int SortOrder, string Description)
        {
            return PlanningManager.Instance.InsertEntityPeriod(this, EntityID, StartDate, EndDate, SortOrder, Description);
        }
        /// <summary>
        /// Deleting Entity Period
        /// </summary>
        /// <param name="entityId">The EntityID</param>
        /// <param name="Assignee">The Assignee</param>
        /// <returns>True (or) False</returns>
        public bool DeleteEntityPeriod(int ID)
        {
            return PlanningManager.Instance.DeleteEntityPeriod(this, ID);
        }

        /// <summary>
        /// Updating Entity Period
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="int EntityID">The EntityID</param>
        /// <param name="string StartDate">The StartDate</param>
        /// <param name="string EndDate">The EndDate</param>
        /// <param name="int SortOrder">The SortOrder</param>
        /// <param name="string Description">The Description</param>
        /// <returns>int</returns>
        public bool UpdateEntityPeriod(int ID, int EntityID, string StartDate, string EndDate, int SortOrder, string Description)
        {
            return PlanningManager.Instance.UpdateEntityPeriod(this, ID, EntityID, StartDate, EndDate, SortOrder, Description);
        }


        /// <summary>
        /// Deleting RootLevelcostCentre
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="costcentreId">The CostcentreID</param>
        /// <returns>True (or) False</returns>
        public bool DeleteRootCostcentre(int costcentreId)
        {
            return PlanningManager.Instance.DeleteRootCostcentre(this, costcentreId);
        }

        /// <summary>
        /// Getting CostcentreFinancialSummaryBlockDetails
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="costCentreId">The CostcentreID</param>
        /// <returns>IFinancialOverview</returns>
        public IFinancialOverview GettingCostcentreFinancialOverview(int costCentreId)
        {
            return PlanningManager.Instance.GettingCostcentreFinancialOverview(this, costCentreId);
        }
        /// <summary>
        /// Updating Costcentre Assigned Amount
        /// </summary>
        /// <param name="costcenreId">The CostcentreID</param>
        /// <param name="totalAssignedAmount">The Total Assigned Amount</param>
        /// <returns>int</returns>
        public int UpdateTotalAssignedAmount(int costcentreId, int totalAssignedAmount)
        {
            return PlanningManager.Instance.UpdateTotalAssignedAmount(this, costcentreId, totalAssignedAmount);
        }

        /// <summary>
        /// Getting All Units.
        /// </summary>
        /// <returns>List of IObjectiveUnit</returns>
        public IList<IObjectiveUnit> GettingObjectiveUnits()
        {
            return PlanningManager.Instance.GettingObjectiveUnits(this);
        }
        public bool AddEntity(string EntityID, string name)
        {
            return PlanningManager.Instance.AddEntity(this, EntityID, name);
        }
        public bool UpdateEntityforSearch(string EntityID, string name)
        {
            return PlanningManager.Instance.UpdateEntityforSearch(this, EntityID, name);
        }
        public bool RemoveEntity(int EntityID, string name)
        {
            return PlanningManager.Instance.RemoveEntity(this, EntityID, name);
        }
        public List<BrandSystems.Marcom.Core.Planning.ResultEntity> QuickSearch(String Text, int ModuleIds, Boolean IsGlobalAdmin)
        {
            return PlanningManager.Instance.QuickSearch(this, Text, ModuleIds, IsGlobalAdmin);
        }
        public bool UpdateSearchEngine()
        {
            return PlanningManager.Instance.UpdateSearchEngine(this);
        }
        public BrandSystems.Marcom.Core.Planning.SearchResult Search(string Text, List<BrandSystems.Marcom.Core.Planning.SearchTerm> SearchTerm, int[] ETypeID, int PageID, bool IsGlobalAdmin)
        {
            return PlanningManager.Instance.Search(this, Text, SearchTerm, ETypeID, PageID, IsGlobalAdmin);
        }

        /// <summary>
        /// Getting Objective & Assignments Type Block
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="objectiveId">The ObjectiveID</param>
        /// <returns>IObjectiveSummaryDeatils</returns>
        public IObjectiveSummaryDetails GettingObjectiveSummaryBlockDetails(int objectiveId)
        {
            return PlanningManager.Instance.GettingObjectiveSummaryBlockDetails(this, objectiveId);
        }

        /// <summary>
        /// Getting Objective & Assignments Fulfillment Block
        /// </summary>
        /// <param name="objectiveId">The ObjectiveID</param>
        /// <returns>IObjectiveFulfullConditions</returns>
        public IObjectiveFulfillCondtions GettingObjectiveFulfillmentBlockDetails(int objectiveId)
        {
            return PlanningManager.Instance.GettingObjectiveFulfillmentBlockDetails(this, objectiveId);
        }

        /// <summary>
        /// Duplicating the entites
        /// </summary>
        /// <param name="proxy"></param>
        /// <param name="entityId"></param>
        /// <returns>EntityID</returns>
        public ArrayList DuplicateEntity(int entityID, int parentID, int DuplicateTimes, bool IsDuplicateChild, Dictionary<string, bool> duplicateitems = null, List<string> listEntityNamesToDuplicate = null)
        {
            return PlanningManager.Instance.DuplicateEntity(this, entityID, parentID, DuplicateTimes, IsDuplicateChild, duplicateitems, listEntityNamesToDuplicate);
        }
        /// <summary>
        /// Inserting Additional Objective & Assignments 
        /// </summary>
        /// <param name="entityId">The EntityID</param>
        /// <param name="entityTypeId">The EntityTypeId</param>
        /// <param name="objectiveTye">The ObjectiveTypeId</param>
        /// <param name="name">The Name</param>
        /// <param name="instruction">The Instruction</param>
        /// <param name="isEnablefeeback">The IsEnableFeedback</param>
        /// <param name="untiId">The UnitId</param>
        /// <param name="plannedTarget">The PlannedTarget</param>
        /// <param name="targetOutcome">The TargetOutcome</param>
        /// <param name="ratingObjective">The RatingObjective</param>
        /// <param name="comments">The Comments</param>
        /// <param name="fulFillment">The Fulfillment</param>
        /// <param name="objectiveStatus">The ObjectiveStatus</param>
        /// <param name="entityMembers">The Entity Users</param>
        /// <param name="ratings">The Ratings</param>
        /// <returns>Last Inserted Additional EntityId</returns>
        public int InsertAdditionalObjective(int entityId, int entityTypeId, int objectiveTypeId, string name, string instruction, bool isEnableFeedback, int unitId, decimal plannedTarget, decimal targetOutCome, int ratingObjective, string comments, int fulFillment, int objectiveStatus, IEntityRoleUser entityMembers, List<string> ratings = null)
        {
            return PlanningManager.Instance.InsertAdditionalObjective(this, entityId, entityTypeId, objectiveTypeId, name, instruction, isEnableFeedback, unitId, plannedTarget, targetOutCome, ratingObjective, comments, fulFillment, objectiveStatus, entityMembers, ratings);
        }

        /// <summary>
        /// Getting Objectives for Activity Entity Select 
        /// </summary>
        /// <param name="entityId">The EntityID</param>
        /// <returns>IObjectivesToEntitySelect</returns>
        public IList<IObjectivesToEntitySelect> GettingObjectivestoEntitySelect(int entityId)
        {
            return PlanningManager.Instance.GettingObjectivestoEntitySelect(this, entityId);
        }

        /// <summary>
        /// Updating Objective & Assignments Summary Block
        /// </summary>
        /// <param name="objectiveId">The ObjectiveID</param>
        /// <param name="instruction">The Instruction</param>
        /// <param name="isEnableComments">The EnableComments</param>
        /// <param name="unitId">The UnitID</param>
        /// <param name="globalBaseline">The GlobaleBaseLine</param>
        /// <param name="globalTarget">The GlobalTarget</param>
        /// <returns>True or False</returns>
        public bool UpdateObjectiveSummaryBlockData(int objectiveId, int objectiveTypeId, string instruction, bool isEnableComments, int unitId = 0, decimal globalBaseline = 0, decimal globalTarget = 0)
        {
            return PlanningManager.Instance.UpdateObjectiveSummaryBlockData(this, objectiveId, objectiveTypeId, instruction, isEnableComments, unitId, globalBaseline, globalTarget);
        }
        /// <summary>
        /// Getting Objective & Assignments Fulfillment Block
        /// </summary>
        /// <param name="objectiveId">The ObjectiveID</param>
        /// <returns>IList of IObjectiveFulfullConditions</returns>
        public IList<IObjectiveFulfillCondtions> GettingEditObjectiveFulfillmentDetails(int objectiveId)
        {
            return PlanningManager.Instance.GettingEditObjectiveFulfillmentDetails(this, objectiveId);
        }
        /// <summary>
        /// Getting Entity Predefine Objectie AttributeDetails
        /// </summary>
        /// <param name="entityId">The EntityID</param>
        /// <returns>List of IEntityPredefineObjectiveAttributes</returns>
        public List<IEntityPredefineObjectiveAttributes> GettingEntityPredefineObjectives(int entityId)
        {
            return PlanningManager.Instance.GettingEntityPredefineObjectives(this, entityId);
        }

        /// <summary>
        /// Inserting predefined Objectives for Entity
        /// </summary>
        /// <param name="objectiveId">The ObjectiveID</param>
        /// <param name="entityId">The EntityID</param>
        /// <returns>List of Last Inserted Objective IDs</returns>
        public List<int> InsertPredefineObjectivesforEntity(List<int> objectiveId, int entityId)
        {
            return PlanningManager.Instance.InsertPredefineObjectivesforEntity(this, objectiveId, entityId);
        }

        /// <summary>
        /// Getting Entity Predefine Objectie AttributeDetails
        /// </summary>
        /// <param name="attribteDate">The IList of AttributeData</param>
        /// <param name="startDate">The StartDate</param>
        /// <param name="enddate">The EndDate</param>
        /// <param name="entityTypeId">The EntityTypeID</param>
        /// <returns>List of IEntityPredefineObjectiveAttributes</returns>
        public List<IEntityPredefineObjectiveAttributes> GettingPredefineObjectivesForEntityMetadata(IList<IAttributeData> attributeData, DateTime startDate, DateTime endDate, int entityTypeID)
        {
            return PlanningManager.Instance.GettingPredefineObjectivesForEntityMetadata(this, attributeData, startDate, endDate, entityTypeID);
        }

        /// <summary>
        /// Getting Entity Predefine Objecties
        /// </summary>
        /// <param name="entityId">The EntityID</param>
        /// <returns>IList of IPredefineObjectives</returns>
        public IList<IPredefineObjectives> LoadPredefineObjectives(int entityId)
        {
            return PlanningManager.Instance.LoadPredefineObjectives(this, entityId);
        }

        /// <summary>
        /// Updating predefined Objectives for Entity
        /// </summary>
        /// <param name="objectiveEntityId">The ObjectiveEntiyId</param>
        /// <param name="objectiveId">The ObjectiveID</param>
        /// <param name="entityId">The EntityID</param>
        /// <param name="plannedTarget">The PlannedTarget</param>
        /// <param name="targetOutCome">The TargetOutcome</param>
        /// <param name="ratingObjective">The RatignObjective</param>
        /// <param name="comments">The Comments</param>
        /// <param name="status">The Status</param>
        /// <param name="fulfilled">The FulFilled</param>
        /// <returns>True or False</returns>
        public bool UpdatePredefineObjectivesforEntity(int objectiveEntiyId, int objectiveId, int entityId, decimal plannedTarget = 0, decimal targetOutcome = 0, int ratingObjective = 0, string comments = null, int status = 0, int fulfillment = 0)
        {
            return PlanningManager.Instance.UpdatePredefineObjectivesforEntity(this, objectiveEntiyId, objectiveId, entityId, plannedTarget, targetOutcome, ratingObjective, comments, status, fulfillment);
        }

        /// <summary>
        /// Getting Entity Additional Objecties
        /// </summary>
        /// <param name="entityId">The EntityID</param>
        /// <returns>IList of IPredefineObjectives</returns>
        public IList<IPredefineObjectives> GettingAddtionalObjectives(int entityId)
        {
            return PlanningManager.Instance.GettingAddtionalObjectives(this, entityId);
        }

        /// <summary>
        /// Inserting and Updating Mandatoy Objective condition satisfied Entities
        /// </summary>
        /// <param name="objectiveId">The ObjectiveID</param>
        /// <param name="objectiveName">The ObjectiveName</param>
        /// <param name="objectiveDescription">The ObjectiveDescription</param>
        /// <returns>True or False</returns>
        public bool UpdatingObjectiveOverDetails(int objectiveId, string objectiveName, string objectiveDescription, string Typeid)
        {
            return PlanningManager.Instance.UpdatingObjectiveOverDetails(this, objectiveId, objectiveName, objectiveDescription, Typeid);
        }

        /// <summary>
        /// Updating Additional Objective & Assignments 
        /// </summary>
        /// <param name="objectiveEntityId">The ObjectiveEntityID</param>
        /// <param name="entityId">The EntityID</param>
        /// <param name="objectiveTye">The ObjectiveTypeId</param>
        /// <param name="instruction">The Instruction</param>
        /// <param name="isEnablefeeback">The IsEnableFeedback</param>
        /// <param name="untiId">The UnitId</param>
        /// <param name="plannedTarget">The PlannedTarget</param>
        /// <param name="targetOutcome">The TargetOutcome</param>
        /// <param name="ratingObjective">The RatingObjective</param>
        /// <param name="comments">The Comments</param>
        /// <param name="fulFillment">The Fulfillment</param>
        /// <returns>True or False</returns>
        public bool UpdateAdditionalObjectivesforEntity(int objectiveEntityId, int entityId, int objectiveTypeId, string instruction, bool isEnableFeedback, int unitId, decimal plannedTarget, decimal targetOutCome, int ratingObjective, string comments, int fulFillment, string instructions, int Objstatus, string Ojectivename)
        {
            return PlanningManager.Instance.UpdateAdditionalObjectivesforEntity(this, objectiveEntityId, entityId, objectiveTypeId, instruction, isEnableFeedback, unitId, plannedTarget, targetOutCome, ratingObjective, comments, fulFillment, instructions, Objstatus, Ojectivename);
        }

        /// <summary>
        /// Reinserting Objective Fulfillment Conditions
        /// </summary>
        /// <param name="objectiveId">The ObjectiveID</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <param name="dateRule">The date rule.</param>
        /// <param name="isMandatory">The is mandatory.</param>
        /// <param name="objFullfilConditions">The objFullfilConditions.</param>
        /// <returns>Last inserted Condition ID</returns>
        public int UpdateObjectiveFulfillmentCondition(int objectiveId, string objStartDate, string objEndDate, int objDateRule, bool objMandatory, IList<IObjectiveFulfillCondtions> objFullfilConditions, string ObjectiveFulfillDeatils)
        {
            return PlanningManager.Instance.UpdateObjectiveFulfillmentCondition(this, objectiveId, objStartDate, objEndDate, objDateRule, objMandatory, objFullfilConditions, ObjectiveFulfillDeatils);
        }
        //string ObjectiveFulfillDeatils
        /// <summary>
        /// Deleting Objective Fulfillment Conditions 
        /// </summary>
        /// <param name="objectiveId">The ObjectiveID</param>
        /// <returns>True or False</returns>
        public bool DeleteObjectiveFulfillment(int objectiveId)
        {
            return PlanningManager.Instance.DeleteObjectiveFulfillment(this, objectiveId);
        }

        /// <summary>
        /// Getting Additional Objective Ratings
        /// </summary>
        /// <param name="objectiveId">The ObjectiveID</param>
        /// <returns>IList of Additional Ratings</returns>
        public IList<IAddtionalObjectiveRating> GettingAdditionalObjRatings(int objectiveId)
        {
            return PlanningManager.Instance.GettingAdditionalObjRatings(this, objectiveId);
        }

        /// <summary>
        /// Getting Predefine Objective Ratings
        /// </summary>
        /// <param name="objectiveId">The ObjectiveID</param>
        /// <returns>IList of Objective Ratings</returns>
        public IList<IObjectiveRating> GettingPredefineObjRatings(int objectiveId)
        {
            return PlanningManager.Instance.GettingPredefineObjRatings(this, objectiveId);
        }

        /// <summary>
        /// Updating Entity Image Name 
        /// </summary>
        /// <param name="entityId">The EntityID</param>
        /// <param name="attributeId">The AttributeID</param>
        /// <param name="imageName">The ImageName</param>
        ///  <param name="attribtueData">The AttributeData</param>
        /// <returns>True or False</returns>
        public bool UpdateImageName(int entityId, int attributeId, string imageName)
        {
            return PlanningManager.Instance.UpdateImageName(this, entityId, attributeId, imageName);
        }
        public bool EntityForeCastInsert(int entityID, int CostcenterId, Double QuarterAmount, int Quater)
        {
            return PlanningManager.Instance.EntityForeCastInsert(this, entityID, CostcenterId, QuarterAmount, Quater);
        }

        public IList GetForeCastForCCDetl(int CostcenterId)
        {
            return PlanningManager.Instance.GetForeCastForCCDetl(this, CostcenterId);
        }

        public bool EntityForecastAmountUpdate(int EntityID)
        {
            return PlanningManager.Instance.EntityForecastAmountUpdate(this, EntityID);
        }
        public bool DeleteFundRequest(int fundingReqID, int EntityID)
        {
            return PlanningManager.Instance.DeleteFundRequest(this, fundingReqID, EntityID);
        }
        /// <summary>
        /// Getting WorkFlowSteps with Tasks
        /// </summary>
        ///  <param name="proxy"></param>
        /// <param name="EntityTypeID">The EntityTypeID</param>
        /// <returns>IList of IWorkFlowStepsWithTasks</returns>
        public IList<IWorkFlowStepsWithTasks> GetAllWorkFlowStepsWithTasks(int entityID)
        {
            return PlanningManager.Instance.GetAllWorkFlowStepsWithTasks(this, entityID);
        }

        public int InsertTaskWithAttachments(int parentEntityID, int taskTypeID, string TaskName, int StepID, IList<ITask> TaskList, IList<ITaskMember> TaskMembers, IList<ITaskAttachment> TaskAttachments, IList<BrandSystems.Marcom.Core.Common.Interface.IFile> TaskFiles)
        {
            return PlanningManager.Instance.InsertTaskWithAttachments(this, parentEntityID, taskTypeID, TaskName, StepID, TaskList, TaskMembers, TaskAttachments, TaskFiles);

        }

        /// <summary>
        /// Getting Task details
        /// </summary>
        ///  <param name="proxy"></param>
        /// <param name="TaskID">The TaskID</param>
        /// <returns>IList of ITask</returns>
        public IList<ITask> GetWorkFlowTaskDetails(int taskID)
        {
            return PlanningManager.Instance.GetWorkFlowTaskDetails(this, taskID);
        }

        /// <summary>
        /// Updating Milestone status 
        /// </summary>
        /// <param name="entityId">The EntityID</param>
        /// <param name="status">The status</param>
        /// <returns>True or False</returns>
        public bool UpdatingMilestoneStatus(int entityId, int status)
        {
            return PlanningManager.Instance.UpdatingMilestoneStatus(this, entityId, status);
        }


        /// <summary>
        /// Updating Entity Active status 
        /// </summary>
        ///  <param name="proxy"></param>
        /// <param name="entityId">The EntityID</param>
        /// <param name="status">The status</param>
        /// <returns>True or False</returns>
        public bool UpdateEntityActiveStatus(int entityId, int status)
        {
            return PlanningManager.Instance.UpdateEntityActiveStatus(this, entityId, status);
        }

        /// <summary>
        /// Updating Task status 
        /// </summary>
        ///  <param name="proxy"></param>
        /// <param name="entityId">The TaskID</param>
        /// <param name="status">The Status</param>
        /// <returns>True or False</returns>
        public int UpdateTaskStatus(int taskID, int status, int entityID = 0)
        {
            return PlanningManager.Instance.UpdateTaskStatus(this, taskID, status, entityID);
        }

        /// <summary>
        /// Updating Predefine In-Line Edit Planned Target and TargetOutcome 
        /// </summary>
        /// <param name="objectiveId">The ObjectievID</param>
        /// <param name="entityId">The EntityID</param>
        /// <param name="plannedTarget">The PlannedTarget</param>
        /// <param name="targetOutcome">The TargetOutcome</param>
        /// <returns>True or False</returns>
        public bool UpdatePredefineObjectiveinLineData(int objectiveId, int entityId, int plannedTaget, int targetOutcome)
        {
            return PlanningManager.Instance.UpdatePredefineObjectiveinLineData(this, objectiveId, entityId, plannedTaget, targetOutcome);
        }

        /// <summary>
        /// Updating Additional In-Line Edit Planned Target and TargetOutcome 
        /// </summary>
        /// <param name="entityId">The EntityID</param>
        /// <param name="plannedTarget">The PlannedTarget</param>
        /// <param name="targetOutcome">The TargetOutcome</param>
        /// <returns>True or False</returns>
        public bool UpdateAdditionalObjectiveinLineData(int entityId, string objectivnename)
        {
            return PlanningManager.Instance.UpdateAdditionalObjectiveinLineData(this, entityId, objectivnename);
        }
        /// <summary>
        /// Updating Unassigned Task status 
        /// </summary>
        ///  <param name="proxy"></param>
        /// <param name="entityId">The TaskID</param>
        /// <returns>True or False</returns>
        public bool UpdateUnassignedTaskStatus(int predefinedTaskID, int EntityID)
        {
            return PlanningManager.Instance.UpdateUnassignedTaskStatus(this, predefinedTaskID, EntityID);
        }

        /// <summary>
        /// Getting WorkFlowSteps with Tasks
        /// </summary>
        ///  <param name="proxy"></param>
        /// <param name="EntityTypeID">The EntityTypeID</param>
        /// <returns>IList of IWorkFlowStepsWithTasks</returns>
        public IList<IWorkFlowOverView> GetWorkFlowSummary(int entityID)
        {
            return PlanningManager.Instance.GetWorkFlowSummary(this, entityID);
        }
        /// <summary>
        /// Deleting Entity
        /// </summary>
        /// <param name="costcentreId">The CostcentreID</param>
        /// <returns>One (or) Two</returns>
        public int DeleteCostcentre(int costcentreId)
        {
            return PlanningManager.Instance.DeleteCostcentre(this, costcentreId);
        }

        /// <summary>
        /// Deleting Activity Predefine Objective
        /// </summary>
        /// <param name="entityId">The EntityID</param>
        /// <param name="objectiveId">The ObjectiveID</param>
        /// <returns>True (or) False</returns>
        public bool DeleteActivityPredefineObjective(int entityId, int objectiveId)
        {
            return PlanningManager.Instance.DeleteActivityPredefineObjective(this, entityId, objectiveId);
        }

        /// <summary>
        /// Deleting Activity Additional Objective
        /// </summary>
        /// <param name="objectiveId">The ObjectiveID</param>
        /// <returns>True (or) False</returns>
        public bool DeleteAdditionalObjective(int objectiveId, int entityID)
        {
            return PlanningManager.Instance.DeleteAdditionalObjective(this, objectiveId, entityID);
        }
        /// <summary>
        /// Getting Entity OWners List
        /// </summary>
        /// <param name="entityId">The EntityID</param>
        /// <returns>IList of IEntityOwners</returns>
        public IList<IEntityOwners> EntityOwnersList(int entityId)
        {
            return PlanningManager.Instance.EntityOwnersList(this, entityId);
        }

        /// <summary>
        /// Updating Objective OwnerDetails
        /// </summary>
        /// <param name="entitId">The EntityID</param>
        /// <param name="userId">The UserID</param>
        /// <param name="roleId">The RoleID</param>
        /// <param name="oldOwnerId">The Old Objective Ownerid</param>
        /// <returns>True (or) False</returns>
        public bool UpdateObjectiveOwner(int entityId, int userId, int roleId, int oldOwnerId)
        {
            return PlanningManager.Instance.UpdateObjectiveOwner(this, entityId, userId, roleId, oldOwnerId);
        }

        public bool InsertTaskMembers(int parentEntityID, int TaskID, IList<ITaskMember> TaskMembers)
        {
            return PlanningManager.Instance.InsertTaskMembers(this, parentEntityID, TaskID, TaskMembers);

        }

        public bool InsertTaskAttachments(int parentEntityID, int TaskID, IList<ITaskAttachment> TaskAttachments, IList<BrandSystems.Marcom.Core.Common.Interface.IFile> TaskFiles)
        {
            return PlanningManager.Instance.InsertTaskAttachments(this, parentEntityID, TaskID, TaskAttachments, TaskFiles);

        }

        public IList<BrandSystems.Marcom.Core.Common.Interface.IFile> GetTaskAttachmentFile(int taskID)
        {
            return PlanningManager.Instance.GetTaskAttachmentFile(this, taskID);
        }

        /// <summary>
        /// DeleteFileByID.
        /// </summary>
        /// <param name="proxy">ID Parameter</param>
        /// <returns>bool</returns>
        public bool DeleteFileByID(int ID)
        {
            return PlanningManager.Instance.DeleteFileByID(this, ID);
        }
        /// <summary>
        /// Updating Objective Status
        /// </summary>
        /// <param name="objectiveId">The ObjectiveId</param>
        /// <param name="objectiveStatus">The ObjectiveStatus</param>
        /// <returns>True (or) False</returns>
        public bool UpdateObjectivestatus(int objectiveId, int objectiveStatus)
        {
            return PlanningManager.Instance.UpdateObjectivestatus(this, objectiveId, objectiveStatus);
        }

        public int financialcostcentrestatus(int costcentreid, int entityID)
        {
            return PlanningManager.Instance.financialcostcentrestatus(this, costcentreid, entityID);
        }

        public string GetEntitiPeriodByIdForGantt(int EntityID)
        {
            return PlanningManager.Instance.GetEntitiPeriodByIdForGantt(this, EntityID);
        }


        public IList GetPeriodByIdForGantt(int ID)
        {
            return PlanningManager.Instance.GetPeriodByIdForGantt(this, ID);
        }

        /// <summary>
        /// Getting Task details
        /// </summary>
        ///  <param name="proxy"></param>
        /// <param name="TaskID">The TaskID</param>
        /// <returns>IList of ITask</returns>
        public IList<ITask> GetFundRequestTaskDetails(string entityUniqueKey, int CostcentreID)
        {
            return PlanningManager.Instance.GetFundRequestTaskDetails(this, entityUniqueKey, CostcentreID);
        }

        /// <summary>
        /// Getting Task details
        /// </summary>
        ///  <param name="proxy"></param>
        /// <param name="TaskID">The TaskID</param>
        /// <returns>IList of ITask</returns>
        public IList<ITask> GetNewsfeedFundRequestTaskDetails(int fundID)
        {
            return PlanningManager.Instance.GetNewsfeedFundRequestTaskDetails(this, fundID);
        }

        public string GetMilestoneByEntityID(int EntityID)
        {
            return PlanningManager.Instance.GetMilestoneByEntityID(this, EntityID);
        }
        public bool PendingFundRequest(int EntityID)
        {
            return PlanningManager.Instance.PendingFundRequest(this, EntityID);
        }

        public bool UpdateLock(int EntityID, int IsLock)
        {
            return PlanningManager.Instance.UpdateLock(this, EntityID, IsLock);
        }

        public bool IsLockAvailable(int EntityIDs)
        {
            return PlanningManager.Instance.IsLockAvailable(this, EntityIDs);
        }


        /// <summary>
        ///Update Approved Budget
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="Costcentreid">The Costcentre id.</param>
        /// <returns>true/false</returns>
        public bool UpdateCostCentreApprovedBudget(int[] costcentreList)
        {
            return PlanningManager.Instance.UpdateCostCentreApprovedBudget(this, costcentreList);
        }

        public Tuple<IList<IPurchaseOrder>, bool> GetAllPurchaseOrdersByEntityID(int entityid)
        {
            return PlanningManager.Instance.GetAllPurchaseOrdersByEntityID(this, entityid);
        }

        public int CreateNewPurchaseOrder(IList<IPurchaseOrder> listpurchaseOrder, IList<IPurchaseOrderDetail> POdetailList, IList<IAttributeData> entityattributedata, bool DirectPO)
        {
            return PlanningManager.Instance.CreateNewPurchaseOrder(this, listpurchaseOrder, POdetailList, entityattributedata, DirectPO);
        }

        /// <summary>
        /// Getting All CurrencyTypes.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <returns>List of ICurrencyType</returns>
        public IList<ICurrencyType> GetAllCurrencyType()
        {
            return PlanningManager.Instance.GetAllCurrencyType(this);
        }

        /// <summary>
        /// Update Commit Amount in Financial
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="id">The entityid.</param>
        /// <param name="CostCenterId">The CostCenter ID</param>
        /// <param name="Amount">Commit Amount</param>
        /// <returns>Bool</returns>
        public bool EntityCommittedAmountInsert(int entityID, int CostcenterId, Decimal AvailableAmount, Decimal CommitAmount)
        {
            return PlanningManager.Instance.EntityCommittedAmountInsert(this, entityID, CostcenterId, AvailableAmount, CommitAmount);
        }

        public bool EntityDirectSpentCommittedAmountInsert(int entityID, int CostcenterId, Decimal AvailableAmount, Decimal CommitAmount)
        {
            return PlanningManager.Instance.EntityDirectSpentCommittedAmountInsert(this, entityID, CostcenterId, AvailableAmount, CommitAmount);
        }

        public bool EntitySpendAmountInsert(int entityID, int CostcenterId, Decimal AvailableAmount, Decimal CommitAmount)
        {
            return PlanningManager.Instance.EntitySpendAmountInsert(this, entityID, CostcenterId, AvailableAmount, CommitAmount);
        }


        /// <summary>
        /// Getting All Supplier.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <returns>List of ISupplier</returns>
        public IList<ISupplier> GetAllSupplier()
        {
            return PlanningManager.Instance.GetAllSupplier(this);
        }

        /// <summary>
        /// Delete Workflow Tasks
        /// </summary>
        /// <param name="entityId">The EntityID.</param>
        /// <returns>true/false</returns>
        public bool EnableDisableWorkFlow(int entityId, bool IsEnableWorkflow)
        {
            return PlanningManager.Instance.EnableDisableWorkFlow(this, entityId, IsEnableWorkflow);
        }

        /// <summary>
        /// Enable & Disable WorkFlow Status
        /// </summary>
        /// <param name="entityId">The EntityID.</param>
        /// <returns>True or False</returns>
        public bool EnableDisableWorkFlowStatus(int entityId)
        {
            return PlanningManager.Instance.EnableDisableWorkFlowStatus(this, entityId);
        }

        /// <summary>
        /// WorkFlow Tasks Count
        /// </summary>
        /// <param name="entityId">The EntityID.</param>
        /// <returns>Work Tasks Count</returns>
        public int WorkFlowTaskCount(int entityId)
        {
            return PlanningManager.Instance.WorkFlowTaskCount(this, entityId);
        }

        public bool ApprovePurchaseOrders(int[] POIDArr, int entityID = 0)
        {
            return PlanningManager.Instance.ApprovePurchaseOrders(this, POIDArr, entityID);
        }

        public bool SendPurchaseOrders(int[] POIDArr, int entityID = 0)
        {
            return PlanningManager.Instance.SendPurchaseOrders(this, POIDArr, entityID);
        }

        public bool RejectPurchaseOrders(int[] POIDArr, int entityID = 0)
        {
            return PlanningManager.Instance.RejectPurchaseOrders(this, POIDArr, entityID);
        }

        public int CreateNewSupplier(IList<ISupplier> listSupplier)
        {
            return PlanningManager.Instance.CreateNewSupplier(this, listSupplier);
        }

        public Tuple<IList<IInvoice>, bool> GetAllInvoiceByEntityID(int entityid)
        {
            return PlanningManager.Instance.GetAllInvoiceByEntityID(this, entityid);
        }

        public IList<IPurchaseOrder> GetAllSentPurchaseOrdersByEntityID(int entityid)
        {
            return PlanningManager.Instance.GetAllSentPurchaseOrdersByEntityID(this, entityid);
        }
        public int CreateNewInvoice(IList<IInvoice> listpurchaseOrder, IList<IInvoiceDetail> POdetailList, IList<IAttributeData> entityattributedata)
        {
            return PlanningManager.Instance.CreateNewInvoice(this, listpurchaseOrder, POdetailList, entityattributedata);
        }


        public int CreateInvoiceAndPurchaseOrder(IList<IInvoice> listInvoice, IList<IInvoiceDetail> InvoicedetailList, IList<IPurchaseOrder> listpurchaseOrder, IList<IPurchaseOrderDetail> POdetailList)
        {
            return PlanningManager.Instance.CreateInvoiceAndPurchaseOrder(this, listInvoice, InvoicedetailList, listpurchaseOrder, POdetailList);
        }

        public int UpdatePurchaseOrder(int POID, IList<IPurchaseOrder> listpurchaseOrder, IList<IPurchaseOrderDetail> POdetailList, IList<IAttributeData> entityattributedata)
        {
            return PlanningManager.Instance.UpdatePurchaseOrder(this, POID, listpurchaseOrder, POdetailList, entityattributedata);
        }

        public bool PeriodAvailability(int EntityTypeID)
        {
            return PlanningManager.Instance.PeriodAvailability(this, EntityTypeID);
        }

        public IList<IPlanning> GetPlanningTransactionsByEID(JArray jObject)
        {
            return PlanningManager.Instance.GetPlanningTransactionsByEID(this, jObject);
        }
        public bool DeletePlanTransactions(JArray planObj, int entityID = 0)
        {
            return PlanningManager.Instance.DeletePlanTransactions(this, planObj, entityID);
        }

        public List<IEntityPredefineObjectiveAttributes> GettingPredefineTemplatesForEntityMetadata(IList<IAttributeData> attributeData, int entityTypeID, int entityID)
        {
            return PlanningManager.Instance.GettingPredefineTemplatesForEntityMetadata(this, attributeData, entityTypeID, entityID);
        }

        public IList<BrandSystems.Marcom.Core.Planning.Gantt> getReportData(string listIDS)
        {

            return PlanningManager.Instance.getReportData(this, listIDS);
        }




        /// <summary>
        /// Gets Currency type.
        /// </summary>
        /// <returns>
        /// IList
        /// </returns>
        public IList<ICurrencyType> GetCurrencyTypeFFSettings()
        {
            return PlanningManager.Instance.GetCurrencyListFFsettings(this);


        }



        public bool UpdateFinancialForecastSettings(int id, string Name, string ShortName, string Symbol)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes currency type by id.
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool DeleteCurrencyListFFSettings(int id)
        {
            //throw new NotImplementedException();
            return PlanningManager.Instance.DeleteCurrencyListFFSettings(this, id);
        }

        /// <summary>
        /// Creates the Currencytype.
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="Name">Name</param>
        /// <param name="ShortName">Short name</param>
        /// <param name="Symbol">symbol</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool InsertUpdateCurrencyListFFSettings(int id, string Name, string ShortName, string Symbol)
        {
            return PlanningManager.Instance.InsertUpdateCurrencyListFFSettings(this, Name, ShortName, Symbol, id);
            //  throw new NotImplementedException();
        }

        /// <summary>
        /// Gets Divison ids the ratings.
        /// </summary>
        /// <returns>
        /// object
        /// </returns>

        public object getDivisonIds()
        {
            return PlanningManager.Instance.getDivisonIds(this);
        }

        /// <summary>
        /// Gets Divisons By id.
        /// </summary>
        /// <param name="DivisonID">Divisonid</param>
        /// <returns>
        /// bool
        /// </returns>

        public bool SetDivisonsFFSettings(int DivisonID)
        {
            return PlanningManager.Instance.SetDivisonsFFSettings(this, DivisonID);
        }


        /// <summary>
        /// Gets DivisonName.
        /// </summary>
        /// 
        /// <returns>
        /// string
        /// </returns>
        public string GetDivisonName()
        {
            //throw new NotImplementedException();
            return PlanningManager.Instance.GetDivisonName(this);
        }


        public IList<IValdiationWithAttributeRelationData> GetEntityAttributesValidationDetails(int id, int entityTypeID)
        {
            return PlanningManager.Instance.GetEntityAttributesValidationDetails(this, id, entityTypeID);
        }

        /// <summary>
        /// Get the entity status by entity id
        /// </summary>
        /// <param name="proxy"></param>
        /// <param name="entityID"></param>
        /// <returns></returns>
        public IEntityStatus GetEntityStatusByEntityID(int entityID)
        {
            return PlanningManager.Instance.GetEntityStatusByEntityID(this, entityID);
        }

        public bool UpdateEntityStatus(int entityID, int statusID, int intimeID, string comment)
        {
            return PlanningManager.Instance.UpdateEntityStatus(this, entityID, statusID, intimeID, comment);
        }



        public bool CheckForMemberAvailabilityForEntity(int EntityID)
        {
            return PlanningManager.Instance.CheckForMemberAvailabilityForEntity(this, EntityID);
        }

        public int CreateAttributeGroupRecord(int AttributeGroupRecordID, int parentId, int typeId, int GroupID, Boolean isLock, string name, int SortOrder, IList<IAttributeData> entityattributedata)
        {
            return PlanningManager.Instance.CreateAttributeGroupRecord(this, AttributeGroupRecordID, parentId, typeId, GroupID, isLock, name, SortOrder, entityattributedata); ;
        }

        public bool UpdateImageNameFromAttribtueGroup(int entityId, int attributeId, string imageName, int GroupID, int GroupRecordID)
        {
            return PlanningManager.Instance.UpdateImageNameFromAttribtueGroup(this, entityId, attributeId, imageName, GroupID, GroupRecordID);
        }

        public bool DeleteEntityAttributeGroupRecord(int GroupID, int GroupRecordID, int ParentID)
        {
            return PlanningManager.Instance.DeleteEntityAttributeGroupRecord(this, GroupID, GroupRecordID, ParentID);
        }

        public bool SaveUploaderImage(string sourcepath, int imgwidth, int imgheight, int imgX, int imgY)
        {
            return PlanningManager.Instance.SaveUploaderImage(this, sourcepath, imgwidth, imgheight, imgX, imgY);
        }

        public IList<IAttributeData> GetEntityAttributesDetailsUserDetails(int UserID)
        {
            return PlanningManager.Instance.GetEntityAttributesDetailsUserDetails(this, UserID);
        }

        public int GetCurrentDivisionId()
        {
            return PlanningManager.Instance.GetCurrentDivisionId(this);
        }

        /// <summary>
        /// Getting  FinancialMetada attribute details
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="id">The financialresult.</param>
        /// <returns>IFinancialMetadataAttributewithValues<</returns>
        public IList<IFinancialMetadataAttributewithValues> GetFundingCostcenterMetadata(int metadataType)
        {
            return PlanningManager.Instance.GetFundingCostcenterMetadata(this, metadataType);
        }



        public bool SaveFinancialDynamicValues(int finID, int AttributeTypeid, int attributeid, List<object> NewValue)
        {
            return PlanningManager.Instance.SaveFinancialDynamicValues(this, finID, AttributeTypeid, attributeid, NewValue);
        }

        public string GetCaptionofPeriod(int entityId)
        {
            return PlanningManager.Instance.GetCaptionofPeriod(this, entityId);
        }

        /// <summary>
        /// Create New Purchase Order from the API
        /// </summary>
        /// <param name="proxy"></param>
        /// <param name="EntityID">The EntityID</param>
        /// <param name="CostCenterID">The Cost Center ID </param>
        /// <param name="IPurchaseOrder">The IPurchaseOrder</param>
        /// <param name="IList<IAttributeData>">The client specific attribute IDs and values</param>
        /// <returns>Purchase order ID</returns>
        public int InsertUpdatePO(int EntityID, int CostCenterID, IPurchaseOrder ipurchaseOrder, IList<IAttributeData> MetadataValues)
        {
            return PlanningManager.Instance.InsertUpdatePO(this, EntityID, CostCenterID, ipurchaseOrder, MetadataValues);
        }

        /// <summary>
        /// Create New Spent Transaction Order from the API
        /// </summary>
        /// <param name="proxy"></param>
        /// <param name="EntityID">The EntityID</param>
        /// <param name="CostCenterID">The Cost Center ID </param>
        /// <param name="IInvoice">The IInvoice</param>
        /// <param name="IList<IAttributeData>">The client specific attribute IDs and values</param>
        /// <returns>Spent Transaction order ID</returns>
        public int InsertApiSpentTransaction(int EntityID, int CostCenterID, IInvoice iInvoiceObj, IList<IAttributeData> MetadataValues)
        {
            return PlanningManager.Instance.InsertApiSpentTransaction(this, EntityID, CostCenterID, iInvoiceObj, MetadataValues);
        }

        /// <summary>
        /// Update Spent Transaction Order from the API
        /// </summary>
        /// <param name="proxy"></param>
        /// <param name="EntityID">The EntityID</param>
        /// <param name="CostCenterID">The Cost Center ID </param>
        /// <param name="IInvoice">The IInvoice</param>
        /// <param name="IList<IAttributeData>">The client specific attribute IDs and values</param>
        /// <returns>Spent Transaction order ID</returns>
        public int UpdateApiSpentTransaction(int EntityID, int CostCenterID, ArrayList UpdateAttributes, IInvoice iInvoiceObj, IList<IAttributeData> MetadataValues)
        {
            return PlanningManager.Instance.UpdateApiSpentTransaction(this, EntityID, CostCenterID, UpdateAttributes, iInvoiceObj, MetadataValues);
        }


        public List<bool> GetFinanncailforecastData()
        {
            return PlanningManager.Instance.GetFinanncailforecastData(this);
        }

        public bool UpdateFFData(string Financialforecast, bool status)
        {
            return PlanningManager.Instance.UpdateFFData(this, Financialforecast, status);
        }

        public bool UpdateFinancialMetadata(int EntityId, int CostCenterID, IList<IAttributeData> MetadataValues)
        {
            return PlanningManager.Instance.UpdateFinancialMetadata(this, EntityId, CostCenterID, MetadataValues);
        }

         /// <summary>
        /// Create New Purchase Order from the API
        /// </summary>
        /// <param name="proxy"></param>
        /// <param name="EntityID">The EntityID</param>
        /// <param name="CostCenterID">The Cost Center ID </param>
        /// <param name="IPurchaseOrder">The IPurchaseOrder</param>
        /// <param name="IList<IAttributeData>">The client specific attribute IDs and values</param>
        /// <returns>Purchase order ID</returns>
        public int UpdateApiPO(int EntityID, int CostCenterID, ArrayList jattributes, IPurchaseOrder ipurchaseOrder, IList<IAttributeData> MetadataValues)
        {
            return PlanningManager.Instance.UpdateApiPO(this, EntityID, CostCenterID, jattributes, ipurchaseOrder, MetadataValues);
        }

        /// <summary>
        /// Getting API Entity Financial Details 
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="id">The entityid.</param>
        /// <returns>IList</returns>
        public IList<IFinancialDetail> GetApiEntityFinancialdDetails(int entityid, int userID, int costcenterid = 0)
        {
            return PlanningManager.Instance.GetApiEntityFinancialdDetails(this, entityid, userID, costcenterid);
        }
        public int GetOverviewFinancialAmount(int entityId)
        {
            return PlanningManager.Instance.GetOverviewFinancialAmount(this, entityId);
        }

        /// <summary>
        /// GetLockStatus
        /// </summary>
        /// <param name="proxy">The Proxy</param>
        /// <param name="EntityID"></param>
        /// <returns>bool Lock status</returns>
        public Tuple<bool, bool> GetLockStatus(int EntityID)
            {
            return PlanningManager.Instance.GetLockStatus(this, EntityID);
            }

        public IEntity GetEntityDetailsByID(int EntityID)
        {
            return PlanningManager.Instance.GetEntityDetailsByID(this, EntityID);
        }

        /// <summary>
        /// GetEntityLevelAccess
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="EntityID"></param>
        /// <returns>string of user access and entityrole access to convert into json</returns>
        public string GetEntityLevelAccess(int UserID, int EntityID)
        {
            return PlanningManager.Instance.GetEntityLevelAccess(this, UserID, EntityID);
        }

        public string InsertUpdateEntityPeriodLst(JObject jobj, int EntityID)
        {
            return PlanningManager.Instance.InsertUpdateEntityPeriodLst(this, jobj, EntityID);
        }

        public bool GetAttachmentEditFeature()
        {
            return PlanningManager.Instance.GetAttachmentEditFeature(this);
        }

        public int CreateImportedAttributeGroupRecord(int AttributeGroupRecordID, int parentId, int typeId, int GroupID, Boolean isLock, string name, int SortOrder, IList<IAttributeData> entityattributedata)
        {
            return PlanningManager.Instance.CreateImportedAttributeGroupRecord(this, AttributeGroupRecordID, parentId, typeId, GroupID, isLock, name, SortOrder, entityattributedata); ;
        }


        public bool updateOverviewStatus(JArray statusObject)
        {
            return PlanningManager.Instance.updateOverviewStatus(this, statusObject);
        }


        public IList<IEntity> GetEntitiesfrCalender(IList<ICalenderFulfillCondtions> objFullfilConditions)
        {
            return PlanningManager.Instance.GetEntitiesfrCalender(this, objFullfilConditions);
        }

        public int CreateCalender(string CalenderName, bool status, string CalenderDescription, IList<ICalenderFulfillCondtions> objCalenderFullfilConditionsList, IList<IEntityRoleUser> objMembersList, List<int> selectedEntities, DateTime CalenderPublishedOn, int CalenderVisPeriod, int CalenderVisType, bool CalenderisExternal, List<int> selectedTabs)
        {
            return PlanningManager.Instance.CreateCalender(this, CalenderName, status, CalenderDescription, objCalenderFullfilConditionsList, objMembersList, selectedEntities, CalenderPublishedOn, CalenderVisPeriod, CalenderVisType, CalenderisExternal, selectedTabs);
        }
        //
        public IList<ICalender> GetCalenders()
        {
            return PlanningManager.Instance.GetCalenders(this);
        }
        public IList<int> GetEntitiesforSelectedCalender(int calenderID)
        {
            return PlanningManager.Instance.GetEntitiesforSelectedCalender(this, calenderID);
        }
        public ICalenderFulfillCondtions GettingCalenderFulfillmentBlockDetails(int calenderID)
        {
            return PlanningManager.Instance.GettingCalenderFulfillmentBlockDetails(this, calenderID);
        }

        public Tuple<IList<ICalenderFulfillCondtions>, IList<int>> GettingEditCalenderFulfillmentDetails(int calenderId)
        {
            return PlanningManager.Instance.GettingEditCalenderFulfillmentDetails(this, calenderId);
        }

        public int UpdateCalenderFulfillmentCondition(int calenderId, IList<ICalenderFulfillCondtions> calFullfilConditions, string CalFulfillDeatils, List<int> selectedEntities)
        {
            return PlanningManager.Instance.UpdateCalenderFulfillmentCondition(this, calenderId, calFullfilConditions, CalFulfillDeatils, selectedEntities);
        }

        public bool UpdatingCalenderOverDetails(int calId, string calName, string calDescription, string Typeid)
        {
            return PlanningManager.Instance.UpdatingCalenderOverDetails(this, calId, calName, calDescription, Typeid);
        }

        public ICalender GetCalenderDetailsByID(int EntityID)
        {
            return PlanningManager.Instance.GetCalenderDetailsByID(this, EntityID);
        }

        public int SaveCalenderDetails(int calID, bool isExternal, int VisibilityPeriod, int VisibilityType, DateTime CalenderPublishedOn, List<int> selectedTabs)
        {
            return PlanningManager.Instance.SaveCalenderDetails(this, calID, isExternal, VisibilityPeriod, VisibilityType, CalenderPublishedOn, selectedTabs);
        }

        public BrandSystems.Marcom.Core.Planning.SearchResult CustomSearch(string Text, List<BrandSystems.Marcom.Core.Planning.SearchTerm> SearchTerm, int searchtype, int PageID, bool IsGlobalAdmin)
        {
            return PlanningManager.Instance.CustomSearch(this, Text, SearchTerm, searchtype, PageID, IsGlobalAdmin);
        }
        public IList<int> GetTabsforCalender(int calenderID)
        {
            return PlanningManager.Instance.GetTabsforCalender(this, calenderID);
        }

        public int GetCalendarDetailsbyExternalID(string ExternalUrlID)
        {
            return PlanningManager.Instance.GetCalendarDetailsbyExternalID(this, ExternalUrlID);
        }

        public Tuple<object, object> getfinancialForecastIds()
        {
            return PlanningManager.Instance.getfinancialForecastIds(this);
        }


        public IFinancialForecastSettings GetFinancialForecastsettings()
        {
            return PlanningManager.Instance.GetFinancialForecastsettings(this);
        }

        public int UpdateFinancialForecastsettings(int Id, bool IsFinancialforecast, int ForecastDivision, int ForecastBasis, int ForecastLevel, int Forecastdeadlines)
        {
            return PlanningManager.Instance.UpdateFinancialForecastsettings(this, Id, IsFinancialforecast, ForecastDivision, ForecastBasis, ForecastLevel, Forecastdeadlines);
        }
        public int CreateCmsPageEntity(int parentId, int typeId, Boolean active, Boolean isLock, string name, IList<IEntityRoleUser> entityMembers, IList<IEntityCostReleations> entityCostcentres, IList<IEntityPeriod> entityPeriods, IList<IFundingRequest> listFundrequest, IList<IAttributeData> entityattributedata, int NavID, int TemplateID, string PublishedDate, string PublishedTime, int[] assetIdArr = null, IList<IObjectiveEntityValue> entityObjectvalues = null, IList<object> attributes = null)
        {
            return PlanningManager.Instance.CreateCmsPageEntity(this, parentId, typeId, active, isLock, name, entityMembers, entityCostcentres, entityPeriods, listFundrequest, entityattributedata, NavID, TemplateID, PublishedDate, PublishedTime, assetIdArr, entityObjectvalues, attributes);
        }

        public Object GetEntityFinancialdForecastHeadings(int entityID, int divisionID, bool Iscc)
        {
            return PlanningManager.Instance.GetEntityFinancialdForecastHeadings(this, entityID, divisionID, Iscc);
        }

        public Tuple<string, string> GetlastUpdatedtime(int entityID)
        {
            return PlanningManager.Instance.GetlastUpdatedtime(this, entityID);
        }
        public bool RemoveEntityAsync(int EntityID)
        {
            return PlanningManager.Instance.RemoveEntityAsync(this, EntityID);
        }
    }
}
