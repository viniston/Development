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

namespace BrandSystems.Marcom.Core.Managers.Proxy
{
    /// <summary>
    /// 
    /// </summary>
    internal partial class MetadataManagerProxy : IMetadataManager, IManagerProxy
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
        internal MetadataManagerProxy(MarcomManager marcomManager)
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
        /// Gets the module.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <returns>
        /// List of Imodule
        /// </returns>
        public IList<IModule> GetModule()
        {
            return MetadataManager.Instance.GetModule(this);
        }
        /// <summary>
        /// Gets the module.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns>
        /// List of Imodule
        /// </returns>
        public IList<IModule> GetModuleByID(int ID)
        {
            return MetadataManager.Instance.GetModuleByID(this, ID);
        }
        /// <summary>
        /// Inserts the update module.
        /// </summary>
        /// <param name="caption">The caption.</param>
        /// <param name="description">The description.</param>
        /// <param name="isenable">if set to <c>true</c> [isenable].</param>
        /// <param name="id">The id.</param>
        /// <returns>
        /// INT
        /// </returns>
        public int InsertUpdateModule(string caption, string description, bool isenable, int id = 0)
        {
            return MetadataManager.Instance.InsertUpdateModule(this, caption, description, isenable, id);
        }

        /// <summary>
        /// Deletes the module.
        /// </summary>
        /// <param name="moduleid">The moduleid.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool DeleteModule(int moduleid)
        {
            return MetadataManager.Instance.DeleteModule(this, moduleid);
        }

        /// <summary>
        /// Gets the modulefeature.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <returns>
        /// List of modulefeatrues
        /// </returns>
        public IList<IModuleFeature> GetModulefeature(int version)
        {
            return MetadataManager.Instance.GetModulefeature(this, version);
        }
        /// <summary>
        /// Inserts the update modulefeature.
        /// </summary>
        /// <param name="moduleid">The moduleid.</param>
        /// <param name="featureid">The featureid.</param>
        /// <param name="isenable">if set to <c>true</c> [isenable].</param>
        /// <returns>
        /// int
        /// </returns>
        public int InsertUpdateModulefeature(int moduleid, int featureid, bool isenable, int ID)
        {
            return MetadataManager.Instance.InsertUpdateModulefeature(this, moduleid, featureid, isenable, ID);
        }


        /// <summary>
        /// Deletes the module feature.
        /// </summary>
        /// <param name="moduleid">The moduleid.</param>
        /// <param name="featureid">The featureid.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool DeleteModuleFeature(int id)
        {
            return MetadataManager.Instance.DeleteModuleFeature(this, id);
        }
        /// <summary>
        /// Gets the type of the entity by ID.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns>
        /// List of Entitytype
        /// </returns>
        public IList<IEntityType> GetEntityTypeByID(int ID)
        {
            return MetadataManager.Instance.GetEntityTypeByID(this, ID);
        }
        /// <summary>
        /// Gets the type of the entity.
        /// </summary>
        /// <returns>
        /// List of Entitytype
        /// </returns>
        public IList<IEntityType> GetEntityType(int ModuleID)
        {
            return MetadataManager.Instance.GetEntityType(this, ModuleID);
        }

        /// <summary>
        /// Gets the details of WorkflowType.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <returns>List of IWorkFlowType</returns>
        public IList<IWorkFlowType> GetWorkFlowDetails()
        {
            return MetadataManager.Instance.GetWorkFlowDetails(this);
        }

        /// <summary>
        /// Gets the type of the entity.
        /// </summary>
        /// <returns>
        /// List of Entitytype
        /// </returns>
        public IList<IEntityType> GetEntityTypeIsAssociate()
        {
            return MetadataManager.Instance.GetEntityTypeIsAssociate(this);
        }

        /// <summary>
        /// Gets the type of the entity.
        /// </summary>
        /// <returns>
        /// List of Entitytype
        /// </returns>
        public IList<IEntityType> GetEntityTypefromDB()
        {
            return MetadataManager.Instance.GetEntityTypefromDB(this);
        }
        /// <summary>
        /// Inserts the type of the update entity.
        /// </summary>
        /// <param name="metadataManagerProxy">The metadata manager proxy.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="description">The description.</param>
        /// <param name="ModuleId">The module id.</param>
        /// <param name="IsSystemDefined">if set to <c>true</c> [is system defined].</param>
        /// <param name="Category">The category.</param>
        /// <param name="shortDescription">The ShortDescription.</param>
        /// <param name="colorCode">The ColorCode.</param>
        /// <param name="Id">The ID as Optional Parameter</param>
        /// <returns>INT.</returns>
        public int InsertUpdateEntityType(string caption, string description, int moduleId, int category, string shortDescription, string colorCode, bool isassociate, int WorkFlowID, bool IsRootLevel, int Id = 0)
        {
            return MetadataManager.Instance.InsertUpdateEntityType(this, caption, description, moduleId, category, shortDescription, colorCode, isassociate, WorkFlowID, IsRootLevel, Id);
        }


        /// <summary>
        /// Deletes the type of the entity.
        /// </summary>
        /// <param name="Entitytypeid">The entitytypeid.</param>
        /// <returns>
        /// bool
        /// </returns>
        public int DeleteEntityType(int Entitytypeid)
        {
            return MetadataManager.Instance.DeleteEntityType(this, Entitytypeid);
        }

        /// <summary>
        /// Gets the entity typefeature.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <returns>
        /// List of IEntityTypeFeature.
        /// </returns>
        public IList<IEntityTypeFeature> GetEntityTypefeatureByID(int entitytypeId)
        {
            return MetadataManager.Instance.GetEntityTypefeatureByID(this, entitytypeId);
        }

        /// <summary>
        /// Gets the entity typefeature.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <returns>
        /// List of IEntityTypeFeature.
        /// </returns>
        public IList<IEntityTypeFeature> GetEntityTypefeature(int version)
        {
            return MetadataManager.Instance.GetEntityTypefeature(this, version);
        }

        //public IList<IFeature> GetEntityTypefeatureByID(int TypeID)
        //{
        //    return MetadataManager.Instance.GetEntityTypefeatureByID(this, TypeID);
        //}

        /// <summary>
        /// Inserts the entity typefeature.
        /// </summary>
        /// <param name="typeid">The typeid.</param>
        /// <param name="featureid">The featureid.</param>
        /// <returns>
        /// INT
        /// </returns>
        public int InsertEntityTypefeature(int typeid, int featureid, int id = 0)
        {
            return MetadataManager.Instance.InsertEntityTypefeature(this, typeid, featureid, id);
        }


        /// <summary>
        /// Deletes the entity type feature.
        /// </summary>
        /// <param name="Entitytypeid">The entitytypeid.</param>
        /// <param name="featureID">The feature ID.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool DeleteEntityTypeFeature(int id)
        {
            return MetadataManager.Instance.DeleteEntityTypeFeature(this, id);
        }

        //EntitytypeAttributeRelationship

        /// <summary>
        /// Creates the entitytyperelation.
        /// </summary>
        /// <param name="entitytypeId">The entitytype id.</param>
        /// <param name="attributeId">The attribute id.</param>
        /// <param name="validationId">The validation id.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <returns>
        /// INT.
        /// </returns>
        public int InsertUpdateEntityTypeAttributeRelation(int entitytypeId, int attributeId, string validationId, int sortOrder, string DefaultValue, bool InheritFromParent, bool IsReadOnly, bool ChooseFromParentOnly, bool IsValidationNeeded, string Caption, bool IsSystemDefined, string PlaceHolderValue, int ID = 0)
        {
            return MetadataManager.Instance.InsertUpdateEntityTypeAttributeRelation(this, entitytypeId, attributeId, validationId, sortOrder, DefaultValue, InheritFromParent, IsReadOnly, ChooseFromParentOnly, IsValidationNeeded, Caption, IsSystemDefined, PlaceHolderValue, ID);
        }

        /// <summary>
        /// Gets the entitytyperelation.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <returns>
        /// List of IEntityTypeAttributeRelation
        /// </returns>
        public IList<IEntityTypeAttributeRelation> GetEntitytyperelation(int version)
        {
            return MetadataManager.Instance.GetEntitytypeRelation(this, version);
        }


        /// <summary>
        /// Gets the entitytyperelation.
        /// </summary>
        /// <returns>
        /// List of IEntityTypeAttributeRelation
        /// </returns>
        public IList<IEntityTypeAttributeRelation> GetEntityTypeAttributeRelationByID(int id)
        {
            return MetadataManager.Instance.GetEntityTypeAttributeRelationByID(this, id);
        }


        /// <summary>
        /// Gets the entitytyperelation.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <returns>
        /// List of IEntityTypeAttributeRelationwithLevels
        /// </returns>
        public IList<IEntityTypeAttributeRelationwithLevels> GetEntityTypeAttributeRelationWithLevelsByID(int id, int ParentID = 0)
        {
            return MetadataManager.Instance.GetEntityTypeAttributeRelationWithLevelsByID(this, id, ParentID);
        }
        /// <summary>
        /// Gets the Options
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="version">The version.</param>
        /// <returns>List of IOption</returns>
        public IList<IOption> GetOptionListByID(int id, bool isforadmin)
        {
            return MetadataManager.Instance.GetOptionListByID(this, id, isforadmin);
        }

        /// <summary>
        /// Gets the Options based on attributeid
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="attributeID">The AttributeID.</param>
        /// <param name="EntityID">The EntityID.</param>
        /// <returns>List of IOption</returns>
        public IList<IOption> GetOptionDetailListByID(int id, int entityID)
        {
            return MetadataManager.Instance.GetOptionDetailListByID(this, id, entityID);
        }


        /// <summary>
        /// Deletes the entity typerelation.
        /// </summary>
        /// <param name="EntityTypeID">The entity type ID.</param>
        /// <param name="AttributeID">The attribute ID.</param>
        /// <returns></returns>
        public bool DeleteEntityAttributeRelation(int ID)
        {
            return MetadataManager.Instance.DeleteEntityAttributeRelation(this, ID);
        }

        /// <summary>
        /// Gets the feature.
        /// </summary>
        /// <returns>
        /// List of IFeature
        /// </returns>
        public IList<IFeature> GetFeature()
        {
            return MetadataManager.Instance.GetFeature(this);
        }

        /// <summary>
        /// Gets the attributetype.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <returns>
        /// List of IAttributeType
        /// </returns>
        public IList<IAttributeType> GetAttributetype()
        {
            return MetadataManager.Instance.GetAttributetype(this);
        }

        public IList<IAttribute> GetAttributeTypeByEntityTypeID(int EnitityTypeID, bool IsAdmin)
        {
            return MetadataManager.Instance.GetAttributeTypeByEntityTypeID(this, EnitityTypeID, IsAdmin);
        }

        /// <summary>
        /// Inserts the upadate attributetype.
        /// </summary>
        /// <param name="Caption">The caption.</param>
        /// <param name="ClassName">Name of the class.</param>
        /// <param name="IsSelectable">if set to <c>true</c> [is selectable].</param>
        /// <param name="DataType">Type of the data.</param>
        /// <param name="SqlType">Type of the SQL.</param>
        /// <param name="Length">The length.</param>
        /// <param name="IsNullable">if set to <c>true</c> [is nullable].</param>
        /// <param name="Id">The id.</param>
        /// <returns>
        /// INT
        /// </returns>       
        public int InsertUpadateAttributetype(string Caption, string ClassName, bool IsSelectable, string DataType, string SqlType, int Length, bool IsNullable, int Id = 0)
        {
            return MetadataManager.Instance.InsertUpadateAttributetype(this, Caption, ClassName, IsSelectable, DataType, SqlType, Length, IsNullable, Id);
        }
        /// <summary>
        /// Gets the attribute by ID.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns>
        /// List of IAttribute.
        /// </returns>
        public IList<IAttribute> GetAttributeByID(int ID)
        {
            return MetadataManager.Instance.GetAttributeByID(this, ID);
        }
        /// <summary>
        /// Deletes the attributetype.
        /// </summary>
        /// <param name="Id">The id.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool DeleteAttributetype(int Id)
        {
            return MetadataManager.Instance.DeleteAttributetype(this, Id);
        }

        public IList<IAttribute> GetAttributefromDB()
        {
            return MetadataManager.Instance.GetAttributefromDB(this);
        }

        public IList<IAttribute> GetAttributesforDetailFilter()
        {
            return MetadataManager.Instance.GetAttributesforDetailFilter(this);
        }
        /// <summary>
        /// Gets the attribute.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <returns>
        /// List of IAttribute.
        /// </returns>
        public IList<IAttribute> GetAttribute()
        {
            return MetadataManager.Instance.GetAttribute(this);
        }


        /// <summary>
        /// Gets the attribute with Tree level values.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <returns>
        /// List of IAttribute.
        /// </returns>
        public IList<IAttribute> GetAttributeWithLevels()
        {
            return MetadataManager.Instance.GetAttributeWithLevels(this);
        }

        /// <summary>
        /// Inserts the update attribute.
        /// </summary>
        /// <param name="caption">The caption.</param>
        /// <param name="attributetypeid">The attributetypeid.</param>
        /// <param name="issystemdefined">if set to <c>true</c> [issystemdefined].</param>
        /// <param name="isforeign">if set to <c>true</c> [isforeign].</param>
        /// <param name="ismultiselect">if set to <c>true</c> [ismultiselect].</param>
        /// <param name="foreigntablename">The foreigntablename.</param>
        /// <param name="foreignidcolumn">The foreignidcolumn.</param>
        /// <param name="foreignattributecolumn">The foreignattributecolumn.</param>
        /// <param name="foreignorderbytable">The foreignorderbytable.</param>
        /// <param name="foreignorderbyidcolumn">The foreignorderbyidcolumn.</param>
        /// <param name="foreignorderbycolumnname">The foreignorderbycolumnname.</param>
        /// <param name="id">The id.</param>
        /// <returns>
        /// int
        /// </returns>
        public int InsertUpdateAttribute(string caption, string description, int attributetypeid, bool issystemdefined, bool isspecial, int id = 0)
        {
            return MetadataManager.Instance.InsertUpdateAttribute(this, caption, description, attributetypeid, issystemdefined, isspecial, id);
        }

        /// <summary>
        /// Deletes the attribute.
        /// </summary>
        /// <param name="attributeid">The attributeid.</param>
        /// <returns>
        /// bool
        /// </returns>
        public int DeleteAttribute(int attributeid)
        {
            return MetadataManager.Instance.DeleteAttribute(this, attributeid);
        }

        /// <summary>
        /// Gets the option.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <returns>
        /// List of IOption
        /// </returns>
        public IList<IOption> GetOption(int version)
        {
            return MetadataManager.Instance.GetOption(this, version);
        }

        /// <summary>
        /// Inserts the update option.
        /// </summary>
        /// <param name="caption">The caption.</param>
        /// <param name="attributeid">The attributeid.</param>
        /// <param name="sortorder">The sortorder.</param>
        /// <param name="id">The id.</param>
        /// <returns>
        /// int
        /// </returns>
        public int InsertUpdateOption(string caption, int attributeid, int sortorder, int id)
        {
            return MetadataManager.Instance.InsertUpdateOption(this, caption, attributeid, sortorder, id);
        }

        /// <summary>
        /// Deletes the option.
        /// </summary>
        /// <param name="optionid">The optionid.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool DeleteOption(int optionid)
        {
            return MetadataManager.Instance.DeleteOption(this, optionid);
        }

        /// <summary>
        /// Gets the multi select.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <returns>
        /// List of IMultiSelect
        /// </returns>
        public IList<IMultiSelect> GetMultiSelect(int version)
        {
            return MetadataManager.Instance.GetMultiSelect(this, version);
        }

        /// <summary>
        /// Inserts the multi select.
        /// </summary>
        /// <param name="entityid">The entityid.</param>
        /// <param name="attributeid">The attributeid.</param>
        /// <param name="optionid">The optionid.</param>
        /// <returns>
        /// int.
        /// </returns>
        public int InsertMultiSelect(int entityid, int attributeid, string optionid)
        {
            return MetadataManager.Instance.InsertMultiSelect(this, entityid, attributeid, optionid);
        }

        /// <summary>
        /// Deletes the MultiSelect.
        /// </summary>
        /// <param name="EntityID">The EntityID.</param>
        /// <param name="AttributeID">The AttributeID.</param>
        /// <returns>bool</returns>
        public bool DeleteMultiSelect(int ID)
        {
            return MetadataManager.Instance.DeleteMultiSelect(this, ID);
        }
        /// <summary>
        /// Gets the treelevel.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <returns>
        /// List of ITreeLevel.
        /// </returns>
        public IList<ITreeLevel> GetTreelevel(int version)
        {
            return MetadataManager.Instance.GetTreelevel(this, version);
        }


        /// <summary>
        /// Gets the treelevel by AttributeID.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="version">The version.</param>
        /// <param name="AttributeID">The AttributeID.</param>
        /// <param name="isAdminsettings">The IsAdminsettingsPresent.</param>
        /// <returns>List of ITreeLevel </returns>
        public IList<ITreeLevel> GetTreelevelByAttributeID(int AttributeID, bool isAdminsettings)
        {
            return MetadataManager.Instance.GetTreelevelByAttributeID(this, AttributeID, isAdminsettings);
        }

        /// <summary>
        /// Inserts the update treelevel.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="Levelname">The levelname.</param>
        /// <param name="attributeid">The attributeid.</param>
        /// <param name="id">The id.</param>
        /// <returns>
        /// ITreeLevel Object.
        /// </returns>
        public int InsertUpdateTreelevel(int level, string Levelname, int attributeid, bool ispercentage, int id)
        {
            return MetadataManager.Instance.InsertUpdateTreelevel(this, level, Levelname, attributeid, ispercentage, id);
        }

        public int InsertUpdateTree(JObject jObject, JArray treeLevelObj, int attributID)
        {
            return MetadataManager.Instance.InsertUpdateTree(this, jObject, treeLevelObj, attributID);
        }

        /// <summary>
        /// Deletes the tree level.
        /// </summary>
        /// <param name="treelevelid">The treelevelid.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool DeleteTreeLevel(int treelevelid)
        {
            return MetadataManager.Instance.DeleteTreeLevel(this, treelevelid);
        }

        /// <summary>
        /// Gets the tree node.
        /// </summary>
        /// <param name="attributeID">The AttributeID.</param>
        /// <param name="isAdminSettings">The IsAdminSettings.</param>
        /// <returns>string</returns>
        public string GetTreeNode(int attributeID, bool isAdminSettings, int parententityID = 0)
        {
            return MetadataManager.Instance.GetTreeNode(this, attributeID, isAdminSettings, parententityID);
        }

        public IList<DropDownTreePricing> GetDropDownTreePricingObject(int attributeID, bool isrootentity, bool isFetchParent = false, int entityid = 0, int parentid = 0)
        {
            return MetadataManager.Instance.GetDropDownTreePricingObject(this, attributeID, isrootentity, isFetchParent, entityid, parentid);
        }

        public IList<DropDownTreePricing> GetDropDownTreePricingObjectFromParent(int attributeID, bool isInheritfromParent, bool isFetchParent = false, int entityid = 0, int parentid = 0)
        {
            return MetadataManager.Instance.GetDropDownTreePricingObjectFromParent(this, attributeID, isInheritfromParent, isFetchParent, entityid, parentid);
        }

        public IList<DropDownTreePricing> GetDropDownTreePricingObjectFromParentDetail(int attributeID, bool isInheritfromParent, bool isFetchParent = false, int entityid = 0, int parentid = 0)
        {
            return MetadataManager.Instance.GetDropDownTreePricingObjectFromParentDetail(this, attributeID, isInheritfromParent, isFetchParent, entityid, parentid);
        }

        public IList<DropDownTreePricing> GetDropDownTreePricingObjectDetail(int attributeID, bool isInheritfromParent, bool isFetchParent = false, int entityid = 0, int parentid = 0)
        {
            return MetadataManager.Instance.GetDropDownTreePricingObjectDetail(this, attributeID, isInheritfromParent, isFetchParent, entityid, parentid);
        }

        /// <summary>
        /// Inserts the tree node.
        /// </summary>
        /// <param name="NodeID">The node ID.</param>
        /// <param name="ParentNodeID">The parent node ID.</param>
        /// <param name="Level">The level.</param>
        /// <param name="KEY">The KEY.</param>
        /// <param name="AttributeID">The attribute ID.</param>
        /// <param name="Caption">The caption.</param>
        /// <param name="id">The id.</param>
        /// <returns>
        /// int.
        /// </returns>
        public int InsertTreeNode(int NodeID, int ParentNodeID, int Level, string KEY, int AttributeID, string Caption, int SortOrder, string colorcode, int id)
        {
            return MetadataManager.Instance.InsertTreeNode(this, NodeID, ParentNodeID, Level, KEY, AttributeID, Caption, SortOrder, colorcode, id);
        }

        /// <summary>
        /// Deletes the tree node.
        /// </summary>
        /// <param name="Id">The id.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool DeleteTreeNode(int Id)
        {
            return MetadataManager.Instance.DeleteTreeNode(this, Id);
        }

        /// <summary>
        /// Gets the tree value.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <returns>
        /// List of ITreeValue
        /// </returns>
        public IList<ITreeValue> GetTreeValue(int version)
        {
            return MetadataManager.Instance.GetTreeValue(this, version);
        }

        /// <summary>
        /// Inserts the update tree value.
        /// </summary>
        /// <param name="attributeid">The attributeid.</param>
        /// <param name="nodeid">The nodeid.</param>
        /// <param name="id">The id.</param>
        /// <returns>
        /// int.
        /// </returns>
        public int InsertUpdateTreeValue(int entityid, int attributeid, int nodeid, int id)
        {
            return MetadataManager.Instance.InsertUpdateTreeValue(this, entityid, attributeid, nodeid, id);
        }

        /// <summary>
        /// Deletes the tree value.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool DeleteTreeValue(int id)
        {
            return MetadataManager.Instance.DeleteTreeValue(this, id);
        }

        /// <summary>
        /// Gets the validation.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <returns>
        /// List of IValidation
        /// </returns>
        public IList<IValidation> GetValidation(int version)
        {
            return MetadataManager.Instance.GetValidation(this, version);
        }

        /// <summary>
        /// Inserts the update validation.
        /// </summary>
        /// <param name="Caption">The caption.</param>
        /// <param name="optionid">The optionid.</param>
        /// <param name="id">The id.</param>
        /// <returns>
        /// IValidation Object.
        /// </returns>
        public int InsertUpdateValidation(IList<IValidation> ValList, int AttributeId, int EntityTypeID, int AttributeTypeID, int ID = 0)
        {
            return MetadataManager.Instance.InsertUpdateValidation(this, ValList, AttributeId, EntityTypeID, AttributeTypeID, ID);
        }

        public IValidation CreateValidationInstace()
        {
            return MetadataManager.Instance.CreateValidationInstace();
        }

        public IList<IValidation> GetAttributeValidationByEntityTypeId(int EntityTypeID, int AttributeId)
        {
            return MetadataManager.Instance.GetAttributeValidationByEntityTypeId(this, EntityTypeID, AttributeId);
        }

        public List<List<string>> GetValidationDationByEntitytype(int EntityTypeID)
        {
            return MetadataManager.Instance.GetValidationDationByEntitytype(this, EntityTypeID);
        }

        /// <summary>
        /// Deletes the validation.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool DeleteValidation(int id)
        {
            return MetadataManager.Instance.DeleteValidation(this, id);
        }

        /// <summary>
        /// Syncs to db.
        /// </summary>
        /// <returns></returns>
        public bool SyncToDb()
        {
            return MetadataManager.Instance.SyncToDb(this);
        }


        /// <summary>
        /// ListofRecord.
        /// </summary>
        /// <returns>IListofRecord</returns>
        public IListofRecord ListSetting(string elementNode)
        {
            return MetadataManager.Instance.ListSetting(elementNode);
        }

        /// <summary>
        /// Inserting EntityMultiSelectAttribute values
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="multiselectAttribute">The Selected Attribute Option values</param>
        /// <returns>bool</returns>
        public bool MultiSelectAttributeInsertion(IList<IMultiSelect> multiselectAttribute)
        {
            return MetadataManager.Instance.MultiSelectAttributeInsertion(this, multiselectAttribute);
        }
        /// <summary>
        /// Getting EntityMultiSelectAttribute values
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="IList<IMultiSelect> multiselectAttributes">The Selected Attribute Option values</param>
        /// <returns>bool</returns>
        public bool UpdateMultiSelectAttribute(IList<IMultiSelect> multiselectAttributes)
        {
            return MetadataManager.Instance.UpdateMultiSelectAttribute(this, multiselectAttributes);
        }
        /// <summary>
        /// Getting EntityMultiSelectAttribute values
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="entityId">The EntityID</param>
        /// <returns>IList<IMultiSelect></returns>
        public IList<IMultiSelect> GetMultiSelectAttributes(int entityId)
        {
            return MetadataManager.Instance.GetMultiSelectAttributes(this, entityId);
        }

        ///// <summary>
        ///// AttributeFilter
        ///// </summary>
        ///// <param name="proxy">The proxy.</param>
        ///// <param name="ListOfRecordSetting">Record Settings</param>
        ///// <returns>IList<IEntityTypeAttributeRelationwithLevels></returns>
        //public IList<IEntityTypeAttributeRelationwithLevels> AttributeFilter(ListSettings listSettings)
        //{

        //    return MetadataManager.Instance.AttributeFilter(this,listSettings);
        //}

        public string GetVersionsCountAndCurrentVersion(string key)
        {
            return MetadataManager.Instance.GetVersionsCountAndCurrentVersion(this, key);
        }
        public bool UpdateActiveVersion(int version)
        {
            return MetadataManager.Instance.UpdateActiveVersion(this, version);
        }
        public bool UpdateWorkingVersion(int version)
        {
            return MetadataManager.Instance.UpdateWorkingVersion(this, version);
        }

        /// <summary>
        /// AttributeFilter
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="ListOfRecordSetting">Record Settings</param>
        /// <returns>IList<IEntityTypeAttributeRelationwithLevels></returns>
        public IList<AttributeSettings> AttributeFilter(ListSettings listSettings)
        {

            return MetadataManager.Instance.AttributeFilter(this, listSettings);
        }
        /// <summary>
        /// <summary>
        /// Gettign Treenodes by AttributeId and Level.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="AttributeID">The AttributeID.</param>
        /// <param name="level">The LevelID.</param>
        /// <returns>List of ITreeNode</returns>
        public IList<ITreeNode> GetTreeNodeByLevel(int attributeID, int level)
        {
            return MetadataManager.Instance.GetTreeNodeByLevel(this, attributeID, level);
        }

        /// <summary>
        /// Gettign Attributes from AdminSettings xml and based on AttributeId and Level getting Nodes.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <returns>IList of IFiltersettingsAttributeData</returns>
        public IList<IFiltersettingsAttributes> GettingFilterAttribute(int typeId, string FilterType, int OptionFrom, JArray Ids)
        {
            return MetadataManager.Instance.GettingFilterAttribute(this, typeId, FilterType, OptionFrom, Ids);

        }

        /// <summary>
        /// Getting list of Entity Id's which are not Parent of this EntityTypeID
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="entityTypeId">The EntityTypeID.</param>
        /// <returns>IList of IEntityType</returns>
        public IList<IEntityType> GettingChildEntityTypes(int entityTypeId)
        {
            return MetadataManager.Instance.GettingChildEntityTypes(this, entityTypeId);
        }

        /// <summary>
        /// Adds the EntityType releation.
        /// </summary>
        /// <param name="parentactivityTypeId">The parentactivity type id.</param>
        /// <param name="childactivityTypeid">The childactivity typeid.</param>
        /// <param name="Id">The Id as option parameter.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <returns>int</returns>
        public int InsertEntityTypeHierarchy(int parentactivityTypeId, int childactivityTypeid, int sortOrder, int Id = 0)
        {
            return MetadataManager.Instance.InsertEntityTypeHierarchy(this, parentactivityTypeId, childactivityTypeid, sortOrder, Id);
        }

        /// <summary>
        /// Getting list of EntityHeirarchy based on EntityTypeID
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="entityTypeId">The EntityTypeID.</param>
        /// <returns>IList of IEntityHeirarchy</returns>
        public IList<IEntityTypeHierarchy> GettingEntityTypeHierarchy(int entityTypeId)
        {
            return MetadataManager.Instance.GettingEntityTypeHierarchy(this, entityTypeId);
        }

        public IList<IEntityType> GettingEntityTypeHierarchyForRootLevel(int entityTypeId)
        {
            return MetadataManager.Instance.GettingEntityTypeHierarchyForRootLevel(this, entityTypeId);
        }

        public bool GetOwnerForEntity(int EntityID)
        {
            return MetadataManager.Instance.GetOwnerForEntity(this, EntityID);
        }

        public IList<IEntityType> GettingEntityTypeHierarchyForAdminTree(int entityTypeId, int ModuleID = -1)
        {
            return MetadataManager.Instance.GettingEntityTypeHierarchyForAdminTree(this, entityTypeId, ModuleID);
        }

        public IList<IEntityAttributeDetails> GetAttributesForDetailBlock(int EntityID)
        {
            return MetadataManager.Instance.GetAttributesForDetailBlock(this, EntityID);
        }

        public bool SaveDetailBlock(int ID, int EntityID, string NewValue)
        {
            return MetadataManager.Instance.SaveDetailBlock(this, ID, EntityID, NewValue);
        }

        public bool SaveDetailBlockForLevels(int EntityID, int AttributeTypeid, int OldValue, List<object> NewValue, int Level)
        {
            return MetadataManager.Instance.SaveDetailBlockForLevels(this, EntityID, AttributeTypeid, OldValue, NewValue, Level);
        }

        public bool UpdateDropDownTreePricing(int EntityID, int AttributeTypeid, int Attributeid, IList<ITreeValue> NewValue)
        {
            return MetadataManager.Instance.UpdateDropDownTreePricing(this, EntityID, AttributeTypeid, Attributeid, NewValue);
        }

        public bool SaveDetailBlockForTreeLevels(int EntityID, int AttributeTypeid, int attributeid, IList<ITreeValue> NewValue, JArray jroldTree, JArray jrnewtree)
        {
            return MetadataManager.Instance.SaveDetailBlockForTreeLevels(this, EntityID, AttributeTypeid, attributeid, NewValue, jroldTree, jrnewtree);
        }

        /// <summary>
        /// Deletes the entity type Hierarchy.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="Entitytypeid">The entitytypeid.</param>
        /// <returns>bool</returns>
        public bool DeleteEntityTypeHierarchy(int id)
        {
            return MetadataManager.Instance.DeleteEntityTypeHierarchy(this, id);
        }

        public string GetOptionsFromXML(string elementNode, int typeid)
        {
            return MetadataManager.Instance.GetOptionsFromXML(elementNode, typeid);
        }

        /// <summary>
        /// Getting list of Entity Id's which are having period
        /// </summary>
        /// <returns>IList of EntityTypeDao</returns>
        public IList<IEntityType> GetFulfillmentEntityTypes()
        {
            return MetadataManager.Instance.GetFulfillmentEntityTypes(this);
        }

        public IList<IEntityType> GetFulfillmentEntityTypesfrCal()
        {
            return MetadataManager.Instance.GetFulfillmentEntityTypesfrCal(this);
        }
        /// <summary>
        /// Getting list of Options for Fulfillment Entity Type Attributes
        /// </summary>
        /// <param name="entityTypeId">The EntityTypeID</param>
        /// <returns>IList of IAttribute</returns>
        public IList<IAttribute> GetFulfillmentAttribute(int entityTypeId)
        {
            return MetadataManager.Instance.GetFulfillmentAttribute(this, entityTypeId);
        }

        /// <summary>
        /// Getting list of Options for Fulfillment Entity Type Attributes
        /// </summary>
        /// <param name="entityTypeId">The EntityTypeID</param>
        /// <returns>IList of IAttribute</returns>
        public IList<IAttribute> GetFulfillmentFinicalAttribute(int entityTypeId)
        {
            return MetadataManager.Instance.GetFulfillmentFinicalAttribute(this, entityTypeId);
        }
        /// <summary>
        /// Getting list of Options for Fulfillment Entity Type Attribute Options
        /// </summary>
        /// <param name="attributeId">The AttributeID</param>
        /// <param name="attributeLevel">The AttributeLevel</param>
        /// <returns>IList of IOptions</returns>
        public IList<IOption> GetFulfillmentAttributeOptions(int attributeId, int attributeLevel = 0)
        {
            return MetadataManager.Instance.GetFulfillmentAttributeOptions(this, attributeId, attributeLevel);
        }

        public List<int> GetAllEntityTypes()
        {
            return MetadataManager.Instance.GetAllEntityTypes(this);
        }

        public IListofRecord ListofRecords(int StartRowNo, int MaxNoofRow, int FilterID, IList<IFiltersettingsValues> filterSettingValues, int[] IdArr, string SortOrderColumn, bool IsDesc, ListSettings listSetting, bool IncludeChildren, int enumEntityTypeIds, int EntityID, bool IsSingleID, int UserID, int Level, bool IsobjectiveRootLevel, int ExpandingEntityID, bool IsWorkspaces = false)
        {
            return MetadataManager.Instance.ListofRecords(this, StartRowNo, MaxNoofRow, FilterID, filterSettingValues, IdArr, SortOrderColumn, IsDesc, listSetting, IncludeChildren, enumEntityTypeIds, EntityID, IsSingleID, UserID, Level, IsobjectiveRootLevel, ExpandingEntityID, IsWorkspaces);
        }


        public IList<int> ListofReportRecords(int FilterID, IList<IFiltersettingsValues> filterSettingValues, int[] IdArr, string SortOrderColumn, bool IsDesc, ListSettings listSetting, bool IncludeChildren, int enumEntityTypeIds, int EntityID, bool IsSingleID, int UserID, int Level, bool IsobjectiveRootLevel, int ExpandingEntityID)
        {
            return MetadataManager.Instance.ListofReportRecords(this, FilterID, filterSettingValues, IdArr, SortOrderColumn, IsDesc, listSetting, IncludeChildren, enumEntityTypeIds, EntityID, IsSingleID, UserID, Level, IsobjectiveRootLevel, ExpandingEntityID);
        }
        public IList GetPath(int EntityID, bool IsWorkspace = false)
        {
            return MetadataManager.Instance.GetPath(this, EntityID, IsWorkspace);
        }
        public bool DeleteOptionByAttributeID(int attributeid)
        {
            return MetadataManager.Instance.DeleteOptionByAttributeID(this, attributeid);
        }
        /// <summary>
        /// Inserts the update modulefeature.
        /// </summary>
        /// <param name="moduleid">The moduleid.</param>
        /// <param name="featureid">The featureid.</param>
        /// <param name="isenable">if set to <c>true</c> [isenable].</param>
        /// <returns>
        /// int
        /// </returns>
        public int InsertEntityHistory(int EntityID, int UserID)
        {
            return MetadataManager.Instance.InsertEntityHistory(this, EntityID, UserID);
        }

        /// <summary>
        /// Getting list of Options for Fulfillment Entity Type Attribute Options
        /// </summary>
        /// <param name="attributeId">The AttributeID</param>
        /// <returns>IList of IOptions</returns>
        public IList<IEntityHistory> EntityHistory_Select(int UserID, int Topx)
        {
            return MetadataManager.Instance.EntityHistory_Select(this, UserID, Topx);
        }

        public IList TopxActivity_Select(int UserID, int Topx)
        {
            return MetadataManager.Instance.TopxActivity_Select(this, UserID, Topx);
        }

        public IList TopxMyTask_Select(int UserID, int Topx)
        {
            return MetadataManager.Instance.TopxMyTask_Select(this, UserID, Topx);
        }
        /// <summary>
        /// Adds the WorkFlow.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="workflowid">The workflowid.</param>
        /// <param name="WorkFlowName">The WorkFlowName.</param>
        /// <param name="WorkFlowDescription">The WorkFlowName.</param>
        /// <param name="IWorkFlowSteps">The IWorkFlowSteps.</param>
        /// <param name="Id">The IPredefinedTasks.</param>
        /// <returns>int</returns>
        public int InsertWorkFlow(string name, string description, IList<IWorkFlowSteps> listWorkflowSteps, IList<IWorkFlowStepPredefinedTasks> listWorkflowStepstasks, int WorkFlowID = 0)
        {
            return MetadataManager.Instance.InsertWorkFlow(this, name, description, listWorkflowSteps, listWorkflowStepstasks, WorkFlowID);
        }

        public IWorkFlowSteps CreateWorkFlowStepsInstace()
        {
            return MetadataManager.Instance.CreateWorkFlowStepsInstace();
        }

        public IWorkFlowStepPredefinedTasks CreateWorkFlowStepPredefinedTasksInstace()
        {
            return MetadataManager.Instance.CreateWorkFlowStepPredefinedTasksInstace();
        }

        /// <summary>
        /// Inserts the type of the update entity.
        /// </summary>
        /// <param name="metadataManagerProxy">The metadata manager proxy.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="description">The description.</param>
        /// <param name="ModuleId">The module id.</param>
        /// <param name="IsSystemDefined">if set to <c>true</c> [is system defined].</param>
        /// <param name="Category">The category.</param>
        /// <param name="shortDescription">The ShortDescription.</param>
        /// <param name="colorCode">The ColorCode.</param>
        /// <param name="Id">The ID as Optional Parameter</param>
        /// <returns>INT.</returns>
        public int InsertUpdatePredefinedWorkTask(string caption, string description, IList<IPredefinedWorflowFileAttachement> fileAttachments, int workflowtypeID, int Id = 0)
        {
            return MetadataManager.Instance.InsertUpdatePredefinedWorkTask(this, caption, description, fileAttachments, workflowtypeID, Id);
        }

        ///GetPredefinedWorkflow By ID
        /// <summary>
        /// get the Predefined Workflow by id.
        /// </summary>
        /// <param name="caption">The caption.</param>
        /// <param name="description">The description.</param>
        /// <param name="id">The id.</param>
        /// <returns>Ilist of IPredefinedTasks</returns>
        public IList<IPredefinedTasks> GetPredefinedWorkflowByID(int ID)
        {
            return MetadataManager.Instance.GetPredefinedWorkflowByID(this, ID);
        }

        public IList<IWorkFlowType> GetWorkFlowDetailsByID(int ID)
        {
            return MetadataManager.Instance.GetWorkFlowDetailsByID(this, ID);
        }

        /// <summary>
        /// Gets the details of WorkflowStep by ID.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <returns>List of IWorkFlowType</returns>
        public IList<IWorkFlowSteps> GetWorkFlowTStepByID(int ID)
        {
            return MetadataManager.Instance.GetWorkFlowTStepByID(this, ID);
        }

        /// <summary>
        /// Gets the details of WorkflowStep PredefinedTask by ID.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <returns>List of IWorkFlowType</returns>
        public IList<IWorkFlowStepPredefinedTasks> GetWorkFlowStepPredefinedTaskByID(string IDs)
        {
            return MetadataManager.Instance.GetWorkFlowStepPredefinedTaskByID(this, IDs);
        }

        /// <summary>
        /// Deletes the Workflow and related tables.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="id">The id.</param>
        /// <returns>bool</returns>
        public bool DeleteWorkflowByID(int id)
        {
            return MetadataManager.Instance.DeleteWorkflowByID(this, id);
        }


        // <summary>
        /// Returns TaskAttachment class.
        /// </summary>
        public IPredefinedWorflowFileAttachement PredefinedWrkflwFileAttachmentService()
        {
            return MetadataManager.Instance.PredefinedWrkflwFileAttachmentService();
        }

        ///GetPredefinedWorkflow file attached By ID
        /// <summary>
        /// get the Predefined Workflow file attached  by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>Ilist of IPredefinedTasks</returns>
        public IList<IPredefinedWorflowFileAttachement> GetPredefinedWorkflowFilesAttchedByID(int ID)
        {
            return MetadataManager.Instance.GetPredefinedWorkflowFilesAttchedByID(this, ID);
        }

        /// <summary>
        /// Deletes the predefined Workflow file.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="id">The id.</param>
        /// <returns>bool</returns>
        public bool DeletePredWorkflowFileByID(string id)
        {
            return MetadataManager.Instance.DeletePredWorkflowFileByID(this, id);
        }

        /// <summary>
        /// Deletes the Workflow and related tables.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="id">The id.</param>
        /// <returns>bool</returns>
        public bool DeletePredefinedWorkflowByID(int id, string caption, string description, int workflowtypeID)
        {
            return MetadataManager.Instance.DeletePredefinedWorkflowByID(this, id, caption, description, workflowtypeID);
        }

        public IList<IFeature> GetModuleFeatures(int moduleID)
        {
            return MetadataManager.Instance.GetModuleFeatures(this, moduleID);
        }

        public IList<IFeature> GetModuleFeaturesForNavigation(int moduleID)
        {
            return MetadataManager.Instance.GetModuleFeaturesForNavigation(this, moduleID);
        }

        public int InsertUpdateFeature(string caption, string description, int moduleID, bool isenable, int id)
        {
            return MetadataManager.Instance.InsertUpdateFeature(this, caption, description, moduleID, isenable, id);
        }

        /// <summary>
        /// Gets the treelevel by AttributeID.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="version">The version.</param>
        /// <param name="AttributeID">The AttributeID.</param>
        /// <returns>List of ITreeNode </returns>
        public IList<ITreeNode> GetTreeNodeByAttributeID(int AttributeID)
        {
            return MetadataManager.Instance.GetTreeNodeByAttributeID(this, AttributeID);
        }


        /// <summary>
        /// Gets the metadata settings.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="version">The version.</param>
        /// <returns>List of Imetadatasettings </returns>
        public IList<IMetadataVersion> GetMetadataVersion()
        {
            return MetadataManager.Instance.GetMetadataVersion(this);
        }

        ///Insert MetadataVersion
        /// <summary>
        /// Insert new metadataversion.
        /// </summary>
        /// <param name="caption">The Name.</param>
        /// <param name="description">The description.</param>
        /// <returns>id</returns>
        public int InsertMetadataVersion(int metadataID, string metdataName, string metadataDescription, int selectedMetadataVer)
        {
            return MetadataManager.Instance.InsertMetadataVersion(this, metadataID, metdataName, metadataDescription, selectedMetadataVer);
        }

        ///GetXMlvaluesfromsysnctoDB
        /// <summary>
        /// Get if any value present or not in the sysnctodb
        /// </summary>
        /// <returns>ilist</returns>
        public int GetXmlNodes_CheckIfValueExistsOrNot()
        {
            return MetadataManager.Instance.GetXmlNodes_CheckIfValueExistsOrNot(this);
        }

        public IList<IEntityType> GetTaskFulfillmentEntityTypes()
        {
            return MetadataManager.Instance.GetTaskFulfillmentEntityTypes(this);
        }

        public IList<IEntityType> GettingEntityForRootLevel(bool IsRootLevel = true)
        {
            return MetadataManager.Instance.GettingEntityForRootLevel(this, IsRootLevel);
        }

        /// <summary>
        /// Getting list of EntityHeirarchy based on EntityTypeID
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="entityTypeId">The EntityTypeID.</param>
        /// <returns>IList of IEntityHeirarchy</returns>
        public IList<IEntityTypeHierarchy> GettingEntityTypeHierarchyForChildActivityType(int entityTypeId)
        {
            return MetadataManager.Instance.GettingEntityTypeHierarchyForChildActivityType(this, entityTypeId);
        }
        public IList<IFeature> GetFeaturesForTopNavigation()
        {
            return MetadataManager.Instance.GetFeaturesForTopNavigation(this);
        }
        public IList<INavigation> GetUserEnabledNavigations()
        {
            return MetadataManager.Instance.GetUserEnabledNavigations(this);
        }

        /// <summary>
        /// Gets the GetAttributeRelationByIDs.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <returns>List of GetAttributeRelationByIDs</returns>
        public IList<IAttribute> GetAttributeRelationByIDs(string ids)
        {
            return MetadataManager.Instance.GetAttributeRelationByIDs(this, ids);
        }

        /// <summary>
        /// Creates the entitytyperelation.
        /// </summary>
        /// <param name="entitytypeId">The entitytype id.</param>
        /// <param name="attributeId">The attribute id.</param>
        /// <param name="validationId">The validation id.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <returns>
        /// INT.
        /// </returns>
        public int InsertUpdateAttributeToAttributeRelations(int entitytypeId, int attributetypeID, int attributeId, int attributeOptionID, int attributeLevel, string attributeRelationID, int ID = 0)
        {
            return MetadataManager.Instance.InsertUpdateAttributeToAttributeRelations(this, entitytypeId, attributetypeID, attributeId, attributeOptionID, attributeLevel, attributeRelationID, ID);
        }

        /// <summary>
        /// Deletes the entity attributetoattributerelation.
        /// </summary>
        /// <param name="entitytypeid">The entitytypeid.</param>
        /// <returns>bool</returns>
        public bool DeleteAttributeToAttributeRelation(int entityID)
        {
            return MetadataManager.Instance.DeleteAttributeToAttributeRelation(this, entityID);
        }

        /// <summary>
        /// Gets the Get Attribute To AttributeRelation.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <returns>List of GetAttributeRelationByIDs</returns>
        public IList<IAttributeToAttributeRelations> GetAttributeToAttributeRelationsByID(int entityId)
        {
            return MetadataManager.Instance.GetAttributeToAttributeRelationsByID(this, entityId);
        }

        public IList<IAttributeToAttributeRelations> GetAttributeToAttributeRelationsByIDForEntity(int entityTypeID, int entityId = 0)
        {
            return MetadataManager.Instance.GetAttributeToAttributeRelationsByIDForEntity(this, entityTypeID, entityId);
        }

        public IList<IOption> GetAttributeOptionsInAttrToAttrRelations(int attributeId, int attributeLevel = 0)
        {
            return MetadataManager.Instance.GetAttributeOptionsInAttrToAttrRelations(this, attributeId, attributeLevel);
        }

        public int InsertUpdateEntityTypeStatusOption(int EntityTypeID, string StatusOption, int SortOrder, int ID = 0)
        {
            return MetadataManager.Instance.InsertUpdateEntityTypeStatusOption(this, EntityTypeID, StatusOption, SortOrder, ID);
        }

        public int InsertUpdateDamTypeFileExtensionOption(int EntityTypeID, string ExtensionOption, int SortOrder, int ID = 0)
        {
            return MetadataManager.Instance.InsertUpdateDamTypeFileExtensionOption(this, EntityTypeID, ExtensionOption, SortOrder, ID);
        }


        public IList<IEntityTypeStatusOptions> GetEntityStatusOptions(int EntityTypeID)
        {
            return MetadataManager.Instance.GetEntityStatusOptions(this, EntityTypeID);
        }

        public IList<IDamTypeFileExtension> GetDamTypeFileExtensionOptions(int EntityTypeID)
        {
            return MetadataManager.Instance.GetDamTypeFileExtensionOptions(this, EntityTypeID);
        }

        public IList<IDamTypeFileExtension> GetAllDamTypeFileExtensionOptions()
        {
            return MetadataManager.Instance.GetAllDamTypeFileExtensionOptions(this);
        }

        public bool DeleteEntityTypeStatusOptions(int EntityTypeID)
        {
            return MetadataManager.Instance.DeleteEntityTypeStatusOptions(this, EntityTypeID);
        }

        public bool DeleteDamTypeFileExtensionOptions(int EntityTypeID)
        {
            return MetadataManager.Instance.DeleteDamTypeFileExtensionOptions(this, EntityTypeID);
        }
        /// <summary>
        /// Get the entity type status
        /// </summary>
        /// <param name="entityTypeID"></param>
        /// <param name="isAdmin"></param>
        /// <param name="entityId"></param>
        /// <returns></returns>
        public IList<IEntityTypeStatusOptions> GetEntityStatus(int entityTypeID, bool isAdmin, int entityId = 0)
        {
            return MetadataManager.Instance.GetEntityStatus(this, entityTypeID, isAdmin, entityId);
        }


        /// <summary>
        /// Get the context menu entity type
        /// </summary>
        /// <returns>IList<IEntityType></returns>
        public IList<IEntityType> RootLevelEntityTypeHierarchy()
        {
            return MetadataManager.Instance.RootLevelEntityTypeHierarchy(this);
        }

        /// <summary>
        /// Get the context menu child entity type
        /// </summary>
        /// <returns>IList<IEntityType></returns>
        public IList<IEntityType> ChildEntityTypeHierarchy()
        {
            return MetadataManager.Instance.ChildEntityTypeHierarchy(this);
        }
        /// <summary>
        /// Gets the Options based on attributeid
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="attributeID">The AttributeID.</param>
        /// <param name="EntityID">The EntityID.</param>
        /// <returns>List of IOption</returns>
        public IList<IOption> GetOptionDetailListByIDOptimised(int entityTypeID, int entityID)
        {
            return MetadataManager.Instance.GetOptionDetailListByIDOptimised(this, entityTypeID, entityID);
        }

        /// <summary>
        /// Force view creation and push the Schema
        /// </summary>
        /// <returns>Status</returns>
        public int ReportViewCreationAndPushSchema()
        {
            return MetadataManager.Instance.ReportViewCreationAndPushSchema(this);
        }

        public IList<IFeature> GetAllModuleFeatures(int moduleID)
        {
            return MetadataManager.Instance.GetAllModuleFeatures(this, moduleID);
        }

        public IAttributeGroupAttributeRelation CreateAttributeRelationInstace()
        {
            return MetadataManager.Instance.CreateAttributeRelationInstace();
        }

        public int InsertUpdateAttributeGroupAndAttributeRelation(int attributegroupId, string attributegroupcaption, string attributegroupdescription, IList<IAttributeGroupAttributeRelation> ObjattributerelationList)
        {
            return MetadataManager.Instance.InsertUpdateAttributeGroupAndAttributeRelation(this, attributegroupId, attributegroupcaption, attributegroupdescription, ObjattributerelationList);
        }

        public IList<IAttributeGroup> GetAttributeGroup()
        {
            return MetadataManager.Instance.GetAttributeGroup(this);
        }

        public IList<IAttributeGroupAttributeRelation> GetAttributeGroupAttributeRelation(int attributegroupId)
        {
            return MetadataManager.Instance.GetAttributeGroupAttributeRelation(this, attributegroupId);
        }

        public int DeleteAttributeGroup(int attributegroupid)
        {
            return MetadataManager.Instance.DeleteAttributeGroup(this, attributegroupid);
        }

        public int DeleteAttributeGroupAttributeRelation(int attributeRelationId)
        {
            return MetadataManager.Instance.DeleteAttributeGroupAttributeRelation(this, attributeRelationId);
        }

        public int InsertUpdateEntityTypeAttributeGroup(IList<IEntityTypeAttributeGroupRelation> ObjattributegroupList, IList<IAttributeGroupRoleAccess> attrgroupaccess, int entitytypeId, string globalaccessids)
        {
            return MetadataManager.Instance.InsertUpdateEntityTypeAttributeGroup(this, ObjattributegroupList, attrgroupaccess, entitytypeId, globalaccessids);
        }


        public IEntityTypeAttributeGroupRelation CreateAttributeGroupRelationInstace()
        {
            return MetadataManager.Instance.CreateAttributeGroupRelationInstace();
        }

        public List<object> GetEntityTypeAttributeGroupRelation(int entitytypeId, int EntityID = 0, int AttributeGroupId = 0)
        {
            return MetadataManager.Instance.GetEntityTypeAttributeGroupRelation(this, entitytypeId, EntityID, AttributeGroupId);
        }

        public int DeleteEntityTypeAttributeGroupRelation(int attributegroupId)
        {
            return MetadataManager.Instance.DeleteEntityTypeAttributeGroupRelation(this, attributegroupId);
        }
        public IList<IEntityTypeAttributeGroupRelationwithLevels> GetAttributeGroupAttributeOptions(int GroupID, int EntityID, int GroupRecordID = 0)
        {
            return MetadataManager.Instance.GetAttributeGroupAttributeOptions(this, GroupID, EntityID, GroupRecordID);
        }
        public IList GetEntityAttributesGroupValues(int EntityID, int EntityTypeID, int GroupID, bool IsCmsContent)
        {
            return MetadataManager.Instance.GetEntityAttributesGroupValues(this, EntityID, EntityTypeID, GroupID, IsCmsContent);
        }
        public IList<IEntityType> DuplicateEntityType(int entitytypeId, string entitycaption, string entityshortdesc, string entitydescription, string entitycolor)
        {
            return MetadataManager.Instance.DuplicateEntityType(this, entitytypeId, entitycaption, entityshortdesc, entitydescription, entitycolor);
        }
        public IList<IEntityTypeAttributeRelationwithLevels> GetEntityTypeAttributeRelationWithLevelsByIDForUserDetails(int id, int ParentID = 0)
        {
            return MetadataManager.Instance.GetEntityTypeAttributeRelationWithLevelsByIDForUserDetails(this, id, ParentID);
        }
        public IList<IEntityTypeAttributeGroupRelationwithLevels> GetUserDetailsAttributes(int TypeID, int UserID = 0)
        {
            return MetadataManager.Instance.GetUserDetailsAttributes(this, TypeID, UserID);
        }
        public IList<IOption> GetOptionDetailListByIDForMyPage(int attributeid, int UserID)
        {
            return MetadataManager.Instance.GetOptionDetailListByIDForMyPage(this, attributeid, UserID);
        }
        public bool SaveDetailBlockForLevelsFromMyPage(int UserID, int AttributeTypeid, int OldValue, List<object> NewValue, int Level)
        {
            return MetadataManager.Instance.SaveDetailBlockForLevelsFromMyPage(this, UserID, AttributeTypeid, OldValue, NewValue, Level);
        }
        public IList<IUserVisibleInfo> GetUserVisiblity()
        {
            return MetadataManager.Instance.GetUserVisiblity(this);
        }
        public int InsertUpdateUserVisibleInfo(int AttributeId, int IsEnable)
        {
            return MetadataManager.Instance.InsertUpdateUserVisibleInfo(this, AttributeId, IsEnable);
        }

        public Tuple<int, int> InsertUpdateFinancialAttribute(string caption, string description, int attributetypeid, bool issystemdefined, bool isspecial, int finatttype, int id = 0)
        {
            return MetadataManager.Instance.InsertUpdateFinancialAttribute(this, caption, description, attributetypeid, issystemdefined, isspecial, finatttype, id);
        }

        public int InsertUpdateFinancialAttrOptions(string caption, int attributeid, int sortorder, int id)
        {
            return MetadataManager.Instance.InsertUpdateFinancialAttrOptions(this, caption, attributeid, sortorder, id);
        }

        public IList<IFinancialAttribute> GetFinancialAttribute()
        {
            return MetadataManager.Instance.GetFinancialAttribute(this);
        }

        public int DeleteFinancialAttribute(int attributeid)
        {
            return MetadataManager.Instance.DeleteFinancialAttribute(this, attributeid);
        }
        public bool DeleteFinancialOptionByAttributeID(int attributeid)
        {
            return MetadataManager.Instance.DeleteFinancialOptionByAttributeID(this, attributeid);
        }
        public bool UpdateFinancialMetadata(int ID, string Caption, string Description, int Fintype, int AttributeTypeid, bool IsSystemdefined, bool IsColumn, bool IsTooltip, bool IscommitTooltip, int SortOrder)
        {
            return MetadataManager.Instance.UpdateFinancialMetadata(this, ID, Caption, Description, Fintype, AttributeTypeid, IsSystemdefined, IsColumn, IsTooltip, IscommitTooltip, SortOrder);
        }
        public bool UpdateFinMetadataSortOrder(int ID, int SortOrder)
        {
            return MetadataManager.Instance.UpdateFinMetadataSortOrder(this, ID, SortOrder);
        }
        public IList<IFinancialOption> GetFinancialAttributeOptions(int ID)
        {
            return MetadataManager.Instance.GetFinancialAttributeOptions(this, ID);
        }
        public bool DeleteFinancialOption(int optionid)
        {
            return MetadataManager.Instance.DeleteFinancialOption(this, optionid);
        }
        public string GettingEntityTypeTreeStructure(int entityTypeId)
        {
            return MetadataManager.Instance.GettingEntityTypeTreeStructure(this, entityTypeId);
        }
        public List<string> GetSubEntityTypeAccessPermission(int entityId, string entityTypeId, int moduleID)
        {
            return MetadataManager.Instance.GetSubEntityTypeAccessPermission(this, entityId, entityTypeId, moduleID);
        }

        public int InsertUpdateEntityTypeRoleAccess(string Caption, int EntityTypeID, int EntityRoleID, int ModuleID, int SortOrder, int ID)
        {
            return MetadataManager.Instance.InsertUpdateEntityTypeRoleAccess(this, Caption, EntityTypeID, EntityRoleID, ModuleID, SortOrder, ID);

        }
        public Tuple<IList<IEntityTypeRoleAcl>, IList<IEntityTypeRoleAcl>> GetEntityTypeRoleAcl(int EntityTypeID)
        {
            return MetadataManager.Instance.GetEntityTypeRoleAcl(this, EntityTypeID);
        }

        public bool DeleteEntityTypeRoleAcl(int ID)
        {
            return MetadataManager.Instance.DeleteEntityTypeRoleAcl(this, ID);
        }
        /// <summary>
        /// GetRoleFeatures
        /// </summary>
        /// <param name="GlobalRoleID"></param>
        /// <returns>returns Ilist of RoleFeature</returns>
        public IList<IRoleFeature> GetRoleFeatures(int GlobalRoleID)
        {
            return MetadataManager.Instance.GetRoleFeatures(this, GlobalRoleID);
        }
        /// <summary>
        /// SaveUpdateRoleFeatures
        /// </summary>
        /// <param name="GlobalRoleID"></param>
        /// <param name="ModuleID"></param>
        /// <param name="FeatureID"></param>
        /// <param name="IsChecked"></param>
        /// <returns>returns true if save is success</returns>
        public bool SaveUpdateRoleFeatures(int GlobalRoleID, int ModuleID, int FeatureID, bool IsChecked, int GlobalAclId)
        {
            return MetadataManager.Instance.SaveUpdateRoleFeatures(this, GlobalRoleID, ModuleID, FeatureID, IsChecked, GlobalAclId);
        }

        public bool IsRoleExist(int ID)
        {
            return MetadataManager.Instance.IsRoleExist(this, ID);
        }


        /// <summary>
        /// Gets the tree node.
        /// </summary>
        /// <param name="attributeID">The AttributeID.</param>
        /// <param name="isAdminSettings">The IsAdminSettings.</param>
        /// <returns>string</returns>
        public string GetAttributeTreeNode(int attributeID, int entityID)
        {
            return MetadataManager.Instance.GetAttributeTreeNode(this, attributeID, entityID);
        }

        /// <summary>
        /// Gets the tree node from parent.
        /// </summary>
        /// <param name="attributeID">The AttributeID.</param>
        /// <param name="isAdminSettings">The IsAdminSettings.</param>
        /// <returns>string</returns>
        public string GetAttributeTreeNodeFromParent(int attributeID, int entityID, bool isChoosefromParent, bool isinheritfromParent = false)
        {
            return MetadataManager.Instance.GetAttributeTreeNodeFromParent(this, attributeID, entityID, isChoosefromParent, isinheritfromParent);
        }

        /// <summary>
        /// Gets the tree node from parent for detail block.
        /// </summary>
        /// <param name="attributeID">The AttributeID.</param>
        /// <param name="isAdminSettings">The IsAdminSettings.</param>
        /// <returns>string</returns>
        public string GetDetailAttributeTreeNodeFromParent(int attributeID, int entityID, bool isChoosefromParent)
        {
            return MetadataManager.Instance.GetDetailAttributeTreeNodeFromParent(this, attributeID, entityID, isChoosefromParent);
        }

        public bool GetWorkspacePermission()
        {
            return MetadataManager.Instance.GetWorkspacePermission(this);
        }
        public IList<DropDownTreePricing> PricingValues(int filterid, int attributeid, int attributetypeid)
        {
            return MetadataManager.Instance.PricingValues(this, filterid, attributeid, attributetypeid);
        }
        public IList<IEntityType> GetDAMEntityTypes()
        {
            return MetadataManager.Instance.GetDAMEntityTypes(this);
        }
        public bool SaveDetailBlockForLink(int EntityID, int AttributeTypeid, int attributeid, string url, List<object> name, int linktype, int module)
        {
            return MetadataManager.Instance.SaveDetailBlockForLink(this, EntityID, AttributeTypeid, attributeid, url, name, linktype, module);
        }
        public bool DeleteEntitytypeAttributeGrpAccessRole(int entityID)
        {
            return MetadataManager.Instance.DeleteEntitytypeAttributeGrpAccessRole(this, entityID);
        }

        public bool DamDeleteEntityAttributeRelation(int ID, int attributeid, int entitytypeid)
        {
            return MetadataManager.Instance.DamDeleteEntityAttributeRelation(this, ID, attributeid, entitytypeid);
        }
        public IAttributeGroupRoleAccess CreateAttributeGroupRoleAccessInstace()
        {
            return MetadataManager.Instance.CreateAttributeGroupRoleAccessInstace();
        }

        public string[] GetAttributeGroupImportedFileColumnName(string fileid)
        {
            return MetadataManager.Instance.GetAttributeGroupImportedFileColumnName(this, fileid);
        }

        public IList InsertImportedAttributeGroupData(List<object> LabelColumnValue, int EntityId, string attrgrpName, string filename, int AttrGrpID)
        {
            return MetadataManager.Instance.InsertImportedAttributeGroupData(this, LabelColumnValue, EntityId, attrgrpName, filename, AttrGrpID);
        }
        public List<object> FetchEntityStatusTree(int EntityId)
        {
            return MetadataManager.Instance.FetchEntityStatusTree(this, EntityId);
        }

        /// <summary>
        /// Gets the type of the task entity.
        /// </summary>
        /// <returns>
        /// List of Entitytype
        /// </returns>
        public IList<IEntityType> GetTaskEntityType()
        {
            return MetadataManager.Instance.GetTaskEntityType(this);
        }
        public IList<EntitytasktypeDao> GetEntityTaskType(int entitytypeid)
        {
            return MetadataManager.Instance.GetEntityTaskType(this, entitytypeid);
        }
        public int InsertUpdateEntityTaskType(int EntityTypeID, int taskType, int ID)
        {
            return MetadataManager.Instance.InsertUpdateEntityTaskType(this, EntityTypeID, taskType, ID);
        }
        /// <summary>
        /// Gets the type of the task entity.
        /// </summary>
        /// <returns>
        /// List of Entitytype
        /// </returns>
        public IList<IEntityType> GetTaskTypes()
        {
            return MetadataManager.Instance.GetTaskTypes(this);
        }
        public IList GetEntityAttributesGroupLabelNames(int EntityID,  int GroupID)
        {
            return MetadataManager.Instance.GetEntityAttributesGroupLabelNames(this, EntityID, GroupID);
        }
    }
}
