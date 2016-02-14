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

namespace BrandSystems.Marcom.Core.Managers
{
    internal partial class DigitalAssetManager : IManager
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

        private static DigitalAssetManager instance = new DigitalAssetManager();
        internal static DigitalAssetManager Instance
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


        public IList<IEntityType> GetDAMEntityTypes(DigitalAssetManagerProxy proxy)
        {
            try
            {
                string xmlPath = string.Empty;
                int version = MarcomManagerFactory.ActiveMetadataVersionNumber;
                IList<IEntityType> listentity = new List<IEntityType>();
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    xmlPath = tx.PersistenceManager.MetadataRepository.GetXmlPath(version);
                    XDocument xDoc = XDocument.Load(xmlPath);
                    var entityTypeHeirarchyDao = tx.PersistenceManager.MetadataRepository.GetObject<EntityTypeDao>(xmlPath);
                    var entityTypeHeirarchyDaoresultObj = entityTypeHeirarchyDao.Where(a => a.ModuleID == 5).ToList();
                    foreach (var entObj in entityTypeHeirarchyDaoresultObj)
                    {
                        EntityType entity = new EntityType();
                        entity.Id = entObj.Id;
                        entity.Caption = entObj.Caption;
                        entity.ColorCode = entObj.ColorCode;
                        entity.ShortDescription = entObj.ShortDescription;
                        listentity.Add(entity);
                    }
                }

                return listentity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<IAttribute> GetAttributeDAMCreate(DigitalAssetManagerProxy proxy, int EntityTypeID)
        {
            try
            {


                int version = MarcomManagerFactory.AdminMetadataVersionNumber;
                IList<IAttribute> _iiAttribute = new List<IAttribute>();
                IList<AttributeDao> dao = new List<AttributeDao>();
                IList<EntityTypeAttributeRelationDao> entityTypeRealtionDao = new List<EntityTypeAttributeRelationDao>();
                IList<AttributeDao> attributesDao = new List<AttributeDao>();

                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    if (version == 0)
                    {
                        entityTypeRealtionDao = tx.PersistenceManager.MetadataRepository.Query<EntityTypeAttributeRelationDao>().Where(a => a.EntityTypeID == EntityTypeID).ToList();
                        var atrsdao1 = tx.PersistenceManager.MetadataRepository.Query<AttributeDao>().Select(a => a).ToList<AttributeDao>();
                        var atrsdao = atrsdao1.Where(a => a.Id != Convert.ToInt32(SystemDefinedAttributes.EntityStatus) && a.Id != Convert.ToInt32(SystemDefinedAttributes.MyRoleGlobalAccess) && a.Id != Convert.ToInt32(SystemDefinedAttributes.MyRoleEntityAccess) && a.Id != Convert.ToInt32(SystemDefinedAttributes.EntityOnTimeStatus) && a.Id != Convert.ToInt32(SystemDefinedAttributes.ObjectiveStatus) && a.Id != Convert.ToInt32(SystemDefinedAttributes.Owner) && a.Id != Convert.ToInt32(SystemDefinedAttributes.Name));
                        attributesDao = atrsdao.Join(entityTypeRealtionDao, a => a.Id, b => b.AttributeID, (ab, bc) =>
                         new { ab, bc }).Where(a => (a.ab.AttributeTypeID == Convert.ToInt32(AttributesList.TextSingleLine) || a.ab.AttributeTypeID == Convert.ToInt32(AttributesList.TextMultiLine) || a.ab.AttributeTypeID == Convert.ToInt32(AttributesList.ListSingleSelection) || a.ab.AttributeTypeID == Convert.ToInt32(AttributesList.ListMultiSelection)) && a.bc.EntityTypeID == EntityTypeID).Select
                            (a => a.ab).ToList();
                        //dao = tx.PersistenceManager.MetadataRepository.GetAll<AttributeDao>().Where(a => a.AttributeTypeID == (int)AttributesList.TextSingleLine || a.AttributeTypeID == (int)AttributesList.ListSingleSelection || a.AttributeTypeID == (int)AttributesList.DateTime).ToList();
                        tx.Commit();
                    }
                    else
                    {
                        string xmlpath = GetXmlWorkingPath();
                        entityTypeRealtionDao = tx.PersistenceManager.MetadataRepository.GetObject<EntityTypeAttributeRelationDao>(xmlpath).Where(a => a.EntityTypeID == EntityTypeID).ToList();
                        var atrsdao1 = tx.PersistenceManager.MetadataRepository.GetObject<AttributeDao>(xmlpath).Select(a => a).ToList<AttributeDao>();
                        var atrsdao = atrsdao1.Where(a => a.Id != Convert.ToInt32(SystemDefinedAttributes.EntityStatus) && a.Id != Convert.ToInt32(SystemDefinedAttributes.MyRoleGlobalAccess) && a.Id != Convert.ToInt32(SystemDefinedAttributes.MyRoleEntityAccess) && a.Id != Convert.ToInt32(SystemDefinedAttributes.EntityOnTimeStatus) && a.Id != Convert.ToInt32(SystemDefinedAttributes.ObjectiveStatus) && a.Id != Convert.ToInt32(SystemDefinedAttributes.Owner) && a.Id != Convert.ToInt32(SystemDefinedAttributes.Name));
                        attributesDao = atrsdao.Join(entityTypeRealtionDao, a => a.Id, b => b.AttributeID, (ab, bc) =>
                         new { ab, bc }).Where(a => (a.ab.AttributeTypeID == Convert.ToInt32(AttributesList.TextSingleLine) || a.ab.AttributeTypeID == Convert.ToInt32(AttributesList.TextMultiLine) || a.ab.AttributeTypeID == Convert.ToInt32(AttributesList.ListSingleSelection) || a.ab.AttributeTypeID == Convert.ToInt32(AttributesList.ListMultiSelection)) && a.bc.EntityTypeID == EntityTypeID).Select
                            (a => a.ab).ToList();

                        tx.Commit();
                    }
                }
                foreach (var item in attributesDao)
                {
                    IAttribute _iAttribute = new BrandSystems.Marcom.Core.Metadata.Attribute();
                    _iAttribute.Caption = item.Caption;
                    _iAttribute.Description = item.Description;
                    _iAttribute.AttributeTypeID = item.AttributeTypeID;
                    _iAttribute.IsSystemDefined = item.IsSystemDefined;
                    _iAttribute.IsSpecial = item.IsSpecial;
                    _iAttribute.Id = item.Id;

                    _iiAttribute.Add(_iAttribute);

                    //if (item.AttributeTypeID == (int)AttributesList.DropDownTree)
                    //{
                    //    using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                    //    {
                    //        var treedao = tx.PersistenceManager.PlanningRepository.GetAll<TreeLevelDao>().Where(a => a.AttributeID == item.Id);
                    //        foreach (var treeitem in treedao)
                    //        {
                    //            IAttribute _iAttribute2 = new BrandSystems.Marcom.Core.Metadata.Attribute();

                    //            _iAttribute2.Caption = treeitem.LevelName;
                    //            _iAttribute2.Description = item.Description;
                    //            _iAttribute2.AttributeTypeID = item.AttributeTypeID;
                    //            _iAttribute2.IsSystemDefined = item.IsSystemDefined;
                    //            _iAttribute2.IsSpecial = item.IsSpecial;
                    //            _iAttribute2.Id = item.Id;
                    //            _iiAttribute.Add(_iAttribute2);
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    //    _iAttribute.Caption = item.Caption;
                    //    _iAttribute.Description = item.Description;
                    //    _iAttribute.AttributeTypeID = item.AttributeTypeID;
                    //    _iAttribute.IsSystemDefined = item.IsSystemDefined;
                    //    _iAttribute.IsSpecial = item.IsSpecial;
                    //    _iAttribute.Id = item.Id;
                    //    _iiAttribute.Add(_iAttribute);
                    //}
                }
                return _iiAttribute;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public IList<IAttribute> GetDAMAttribute(DigitalAssetManagerProxy proxy, int EntityTypeID)
        {
            try
            {
                int version = MarcomManagerFactory.AdminMetadataVersionNumber;
                IList<IAttribute> _iiAttribute = new List<IAttribute>();
                IList<AttributeDao> dao = new List<AttributeDao>();
                IList<EntityTypeAttributeRelationDao> entityTypeRealtionDao = new List<EntityTypeAttributeRelationDao>();
                IList<AttributeDao> attributesDao = new List<AttributeDao>();

                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {

                    entityTypeRealtionDao = tx.PersistenceManager.MetadataRepository.Query<EntityTypeAttributeRelationDao>().Where(a => a.EntityTypeID == EntityTypeID).ToList();
                    var atrsdao1 = tx.PersistenceManager.MetadataRepository.Query<AttributeDao>().Select(a => a).ToList<AttributeDao>();
                    var atrsdao = atrsdao1.Where(a => a.Id != Convert.ToInt32(SystemDefinedAttributes.EntityStatus) && a.Id != Convert.ToInt32(SystemDefinedAttributes.MyRoleGlobalAccess) && a.Id != Convert.ToInt32(SystemDefinedAttributes.MyRoleEntityAccess) && a.Id != Convert.ToInt32(SystemDefinedAttributes.EntityOnTimeStatus) && a.Id != Convert.ToInt32(SystemDefinedAttributes.ObjectiveStatus) && a.Id != Convert.ToInt32(SystemDefinedAttributes.Owner));
                    attributesDao = atrsdao.Join(entityTypeRealtionDao, ab => ab.Id, bc => bc.AttributeID, (ab, bc) =>
                         new { ab, bc }).Where(a => (a.ab.AttributeTypeID == Convert.ToInt32(AttributesList.CheckBoxSelection) || a.ab.AttributeTypeID == Convert.ToInt32(AttributesList.DropDownTree) || a.ab.AttributeTypeID == Convert.ToInt32(AttributesList.DateTime) || a.ab.AttributeTypeID == Convert.ToInt32(AttributesList.ListMultiSelection) || a.ab.AttributeTypeID == Convert.ToInt32(AttributesList.ListSingleSelection) || a.ab.AttributeTypeID == Convert.ToInt32(AttributesList.ParentEntityName) || a.ab.AttributeTypeID == Convert.ToInt32(AttributesList.Period) || a.ab.AttributeTypeID == Convert.ToInt32(AttributesList.TextMoney) || a.ab.AttributeTypeID == Convert.ToInt32(AttributesList.TextMultiLine) || a.ab.AttributeTypeID == Convert.ToInt32(AttributesList.TextSingleLine) || a.ab.AttributeTypeID == Convert.ToInt32(AttributesList.Tree) || a.ab.AttributeTypeID == Convert.ToInt32(AttributesList.TreeMultiSelection)) && a.bc.EntityTypeID == EntityTypeID).Select
                         (a => a.ab).ToList();
                    tx.Commit();

                }
                foreach (var item in attributesDao)
                {
                    IAttribute _iAttribute = new BrandSystems.Marcom.Core.Metadata.Attribute();



                    if (item.AttributeTypeID == (int)AttributesList.DropDownTree || item.AttributeTypeID == (int)AttributesList.TreeMultiSelection)
                    {
                        using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                        {
                            var treedao = tx.PersistenceManager.PlanningRepository.GetAll<TreeLevelDao>().Where(a => a.AttributeID == item.Id);
                            foreach (var treeitem in treedao)
                            {
                                IAttribute _iAttribute2 = new BrandSystems.Marcom.Core.Metadata.Attribute();

                                _iAttribute2.Caption = treeitem.LevelName;
                                _iAttribute2.Description = item.Description;
                                _iAttribute2.AttributeTypeID = item.AttributeTypeID;
                                _iAttribute2.IsSystemDefined = item.IsSystemDefined;
                                _iAttribute2.IsSpecial = item.IsSpecial;
                                _iAttribute2.Id = item.Id;
                                _iAttribute2.Level = treeitem.Level;
                                _iiAttribute.Add(_iAttribute2);
                            }
                        }
                    }
                    else
                    {
                        _iAttribute.Caption = item.Caption;
                        _iAttribute.Description = item.Description;
                        _iAttribute.AttributeTypeID = item.AttributeTypeID;
                        _iAttribute.IsSystemDefined = item.IsSystemDefined;
                        _iAttribute.IsSpecial = item.IsSpecial;
                        _iAttribute.Id = item.Id;
                        _iAttribute.Level = 0;
                        _iiAttribute.Add(_iAttribute);
                    }

                }
                return _iiAttribute;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public IList<IAttribute> GetDAMCommonAttribute(DigitalAssetManagerProxy proxy)
        {
            try
            {
                int version = MarcomManagerFactory.AdminMetadataVersionNumber;
                IList<IAttribute> _iiAttribute = new List<IAttribute>();
                IList<AttributeDao> dao = new List<AttributeDao>();
                IList<EntityTypeDao> entityTypeDao = new List<EntityTypeDao>();
                IList<EntityTypeAttributeRelationDao> entityTypeRealtionDao = new List<EntityTypeAttributeRelationDao>();
                IList<AttributeDao> attributesDao = new List<AttributeDao>();

                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    //var totalchildrenIDarr = new StringBuilder();
                    //totalchildrenIDarr.Append("SELECT id FROM  MM_EntityType met WHERE ModuleID=5");
                    IList totalchildrenIDobj = tx.PersistenceManager.PlanningRepository.ExecuteQuery("SELECT id FROM  MM_EntityType met WHERE ModuleID=5");
                    int[] IdArrnew = totalchildrenIDobj.Cast<dynamic>().Select(a => (int)a["id"]).ToArray();

                    //entityTypeDao = tx.PersistenceManager.MetadataRepository.Query<EntityTypeDao>().Where(a => a.ModuleID == 5).ToList();
                    entityTypeRealtionDao = tx.PersistenceManager.MetadataRepository.Query<EntityTypeAttributeRelationDao>().Where(a => IdArrnew.Contains(a.EntityTypeID)).ToList<EntityTypeAttributeRelationDao>();
                    var atrsdao1 = tx.PersistenceManager.MetadataRepository.Query<AttributeDao>().Select(a => a).ToList<AttributeDao>();
                    var atrsdao = atrsdao1.Where(a => a.Id != Convert.ToInt32(SystemDefinedAttributes.EntityStatus) && a.Id != Convert.ToInt32(SystemDefinedAttributes.MyRoleGlobalAccess) && a.Id != Convert.ToInt32(SystemDefinedAttributes.MyRoleEntityAccess) && a.Id != Convert.ToInt32(SystemDefinedAttributes.EntityOnTimeStatus) && a.Id != Convert.ToInt32(SystemDefinedAttributes.ObjectiveStatus) && a.Id != Convert.ToInt32(SystemDefinedAttributes.Owner));
                    //attributesDao = atrsdao.Join(entityTypeRealtionDao, ab => ab.Id, bc => bc.AttributeID, (ab, bc) =>
                    //     new { ab, bc }).Where(a => (a.ab.AttributeTypeID == Convert.ToInt32(AttributesList.CheckBoxSelection) || a.ab.AttributeTypeID == Convert.ToInt32(AttributesList.DropDownTree) || a.ab.AttributeTypeID == Convert.ToInt32(AttributesList.DateTime) || a.ab.AttributeTypeID == Convert.ToInt32(AttributesList.ListMultiSelection) || a.ab.AttributeTypeID == Convert.ToInt32(AttributesList.ListSingleSelection) || a.ab.AttributeTypeID == Convert.ToInt32(AttributesList.ParentEntityName) || a.ab.AttributeTypeID == Convert.ToInt32(AttributesList.Period) || a.ab.AttributeTypeID == Convert.ToInt32(AttributesList.TextMoney) || a.ab.AttributeTypeID == Convert.ToInt32(AttributesList.TextMultiLine) || a.ab.AttributeTypeID == Convert.ToInt32(AttributesList.TextSingleLine) || a.ab.AttributeTypeID == Convert.ToInt32(AttributesList.Tree) || a.ab.AttributeTypeID == Convert.ToInt32(AttributesList.TreeMultiSelection))).Select
                    //     (a => a.ab).ToList();

                    attributesDao = atrsdao.Join(entityTypeRealtionDao, ab => ab.Id, bc => bc.AttributeID, (ab, bc) =>
                        new { ab, bc }).Where(a => (a.ab.AttributeTypeID == Convert.ToInt32(AttributesList.ListSingleSelection) || a.ab.AttributeTypeID == Convert.ToInt32(AttributesList.ListMultiSelection))).Select
                        (a => a.ab).ToList();
                    tx.Commit();


                }
                var atrdao = attributesDao.GroupBy(p => p.Id).Select(g => g.First()).ToList();
                //var dao1 = attributesDao.GroupBy(x => x.Description);
                foreach (var item in atrdao)
                {
                    IAttribute _iAttribute = new BrandSystems.Marcom.Core.Metadata.Attribute();



                    if (item.AttributeTypeID == (int)AttributesList.DropDownTree || item.AttributeTypeID == (int)AttributesList.TreeMultiSelection)
                    {
                        using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                        {
                            var treedao = tx.PersistenceManager.PlanningRepository.GetAll<TreeLevelDao>().Where(a => a.AttributeID == item.Id);
                            foreach (var treeitem in treedao)
                            {
                                IAttribute _iAttribute2 = new BrandSystems.Marcom.Core.Metadata.Attribute();

                                _iAttribute2.Caption = treeitem.LevelName;
                                _iAttribute2.Description = item.Description;
                                _iAttribute2.AttributeTypeID = item.AttributeTypeID;
                                _iAttribute2.IsSystemDefined = item.IsSystemDefined;
                                _iAttribute2.IsSpecial = item.IsSpecial;
                                _iAttribute2.Id = item.Id;
                                _iAttribute2.Level = treeitem.Level;
                                _iiAttribute.Add(_iAttribute2);
                            }
                        }
                    }
                    else
                    {
                        _iAttribute.Caption = item.Caption;
                        _iAttribute.Description = item.Description;
                        _iAttribute.AttributeTypeID = item.AttributeTypeID;
                        _iAttribute.IsSystemDefined = item.IsSystemDefined;
                        _iAttribute.IsSpecial = item.IsSpecial;
                        _iAttribute.Id = item.Id;
                        _iAttribute.Level = 0;
                        _iiAttribute.Add(_iAttribute);
                    }

                }
                return _iiAttribute;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public bool DAMadminSettingsforRootLevelInsertUpdate(DigitalAssetManagerProxy proxy, string jsondata, string LogoSettings, string key, int typeid)
        {

            dynamic jsObject = JsonConvert.DeserializeObject(jsondata);
            JProperty jprop = new JProperty(jsondata);
            // string xmlpath = AppDomain.CurrentDomain.BaseDirectory + "\\AdminSettings.xml";
            string xmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
            XDocument adminXmlDoc = XDocument.Load(xmlpath);
            var Defaultrootssetting = adminXmlDoc.Descendants("DAMsettings").FirstOrDefault();
            var DefaultLogoSettings = adminXmlDoc.Descendants("DAMsettings").Descendants(LogoSettings).FirstOrDefault();
            if (Defaultrootssetting == null && DefaultLogoSettings == null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<DAMsettings><" + LogoSettings + "></" + LogoSettings + "></DAMsettings>");
                XElement.Parse(sb.ToString());
                adminXmlDoc.Root.Add(XElement.Parse(sb.ToString()));
                adminXmlDoc.Save(xmlpath);

            }
            else if (DefaultLogoSettings == null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<" + LogoSettings + "></" + LogoSettings + ">");
                adminXmlDoc.Element("AppSettings").Descendants("DAMsettings").FirstOrDefault().Add(XElement.Parse(sb.ToString()));
                adminXmlDoc.Save(xmlpath);
            }


            string xelementName = key;
            var xelementFilepath = XElement.Load(xmlpath);
            var xmlElement = xelementFilepath.Descendants("DAMsettings").Descendants(LogoSettings).Descendants("AssetType").FirstOrDefault();
            //var xmlElement = xelementFilepath.Descendants("DAMsettings").Descendants(LogoSettings).Descendants("AssetType").Where(a => Convert.ToInt32(a.Attribute("ID").Value) == typeid).Select(a => a);
            if (xmlElement != null)
            {
                var xDocparse = JsonConvert.DeserializeXNode(jsondata);
                XElement el = xDocparse.Elements("AssetType").FirstOrDefault();
                if (el != null)
                {
                    el.SetAttributeValue("ID", typeid);
                }

                // var xmlElement1 = xmlElement.sless(a => Convert.ToInt32(a.Value) == typeid);
                var xmlElement1 = xelementFilepath.Descendants("DAMsettings").Descendants(LogoSettings).Descendants("AssetType").Where(a => Convert.ToInt32(a.Attribute("ID").Value) == typeid).FirstOrDefault();
                if (xmlElement1 != null)
                {
                    adminXmlDoc.Descendants("DAMsettings").Descendants(LogoSettings).Descendants("AssetType").Where(a => Convert.ToInt32(a.Attribute("ID").Value) == typeid).Remove();
                    XElement e2 = xDocparse.Elements("AssetType").Descendants("Attributes").FirstOrDefault();
                    if (e2.IsEmpty == false)
                    {
                        adminXmlDoc.Element("AppSettings").Descendants("DAMsettings").Descendants(LogoSettings).FirstOrDefault().Add(xDocparse.Nodes().ElementAt(0));

                    }
                    adminXmlDoc.Save(xmlpath);
                }
                else
                {
                    adminXmlDoc.Element("AppSettings").Descendants("DAMsettings").Descendants(LogoSettings).FirstOrDefault().Add(xDocparse.Nodes().ElementAt(0));
                    adminXmlDoc.Save(xmlpath);
                }



            }
            else if (xmlElement == null)
            {

                XDocument xDocparse = JsonConvert.DeserializeXNode(jsondata);
                XElement el = xDocparse.Elements("AssetType").FirstOrDefault();
                XElement e2 = xDocparse.Elements("AssetType").Descendants("Attributes").FirstOrDefault();
                if (el != null)
                {
                    el.SetAttributeValue("ID", typeid);
                }
                var vae = Convert.ToString(xDocparse);
                if (e2.IsEmpty == false)
                {
                    adminXmlDoc.Element("AppSettings").Descendants("DAMsettings").Descendants(LogoSettings).FirstOrDefault().Add(xDocparse.Nodes().ElementAt(0));
                    //adminXmlDoc.Descendants("ListSettings").Descendants("RootLevel").Where(a => Convert.ToInt32(a.Attribute("typeid").Value) == typeid).FirstOrDefault()
                    adminXmlDoc.Save(xmlpath);
                }
            }

            return true;
        }

        public bool DAMadminSettingsforRootLevelInsertUpdate(DigitalAssetManagerProxy proxy, string jsondata, string LogoSettings)
        {

            dynamic jsObject = JsonConvert.DeserializeObject(jsondata);
            JProperty jprop = new JProperty(jsondata);
            // string xmlpath = AppDomain.CurrentDomain.BaseDirectory + "\\AdminSettings.xml";
            string xmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
            XDocument adminXmlDoc = XDocument.Load(xmlpath);
            var Defaultrootssetting = adminXmlDoc.Descendants("DAMsettings").FirstOrDefault();
            var DefaultLogoSettings = adminXmlDoc.Descendants("DAMsettings").Descendants(LogoSettings).FirstOrDefault();
            if (Defaultrootssetting == null && DefaultLogoSettings == null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<DAMsettings><" + LogoSettings + "></" + LogoSettings + "></DAMsettings>");
                XElement.Parse(sb.ToString());
                adminXmlDoc.Root.Add(XElement.Parse(sb.ToString()));
                adminXmlDoc.Save(xmlpath);

            }
            else if (DefaultLogoSettings == null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<" + LogoSettings + "></" + LogoSettings + ">");
                adminXmlDoc.Element("AppSettings").Descendants("DAMsettings").FirstOrDefault().Add(XElement.Parse(sb.ToString()));
                adminXmlDoc.Save(xmlpath);
            }


            //string xelementName = key;
            var xelementFilepath = XElement.Load(xmlpath);
            var xmlElement = xelementFilepath.Descendants("DAMsettings").Descendants(LogoSettings).FirstOrDefault();
            //var xmlElement = xelementFilepath.Descendants("DAMsettings").Descendants(LogoSettings).Descendants("AssetType").Where(a => Convert.ToInt32(a.Attribute("ID").Value) == typeid).Select(a => a);
            if (xmlElement != null)
            {
                var xDocparse = JsonConvert.DeserializeXNode(jsondata);
                XElement e2 = xDocparse.Elements(LogoSettings).Descendants("Attributes").FirstOrDefault();
                adminXmlDoc.Descendants("DAMsettings").Descendants(LogoSettings).Remove();

                if (e2.IsEmpty == false)
                {

                    adminXmlDoc.Element("AppSettings").Descendants("DAMsettings").FirstOrDefault().Add(xDocparse.Nodes().ElementAt(0));

                }
                adminXmlDoc.Save(xmlpath);



            }
            else if (xmlElement == null)
            {

                XDocument xDocparse = JsonConvert.DeserializeXNode(jsondata);

                XElement e2 = xDocparse.Elements(LogoSettings).Descendants("Attributes").FirstOrDefault();

                var vae = Convert.ToString(xDocparse);
                if (e2.IsEmpty == false)
                {
                    adminXmlDoc.Element("AppSettings").Descendants("DAMsettings").FirstOrDefault().Add(xDocparse.Nodes().ElementAt(0));
                    adminXmlDoc.Save(xmlpath);
                }
            }

            return true;
        }

        public string DAMGetAdminSettings(DigitalAssetManagerProxy proxy, string LogoSettings, string key, int typeid)
        {
            if (typeid != 0)
            {
                // string xmlpath =  AppDomain.CurrentDomain.BaseDirectory +"\\AdminSettings.xml";
                string jsonText = "";
                string xmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
                XDocument adminXdoc = XDocument.Load(xmlpath);
                var result = adminXdoc.Descendants("DAMsettings").Descendants(LogoSettings).Descendants("AssetType").FirstOrDefault();
                if (result != null)
                {
                    var result1 = adminXdoc.Descendants("DAMsettings").Descendants(LogoSettings).Descendants("AssetType").Select(a => a).ToList().Where(a => Convert.ToInt32(a.Attribute("ID").Value) == typeid).Select(a => a);
                    if (result1 != null)
                    {
                        var abc = result1.ToList();
                        //var abc = result.Descendants(LogoSettings).Descendants(key).ToList();
                        //var xElementResult = result;
                        if (abc.Count > 0)
                        {
                            jsonText = JsonConvert.SerializeObject(abc[0]);
                        }
                    }
                }
                return jsonText;
            }
            else
            {
                string xmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
                XDocument adminXdoc = XDocument.Load(xmlpath);
                var result = adminXdoc.Descendants("DAMsettings").Descendants(LogoSettings).Descendants("AssetType").Select(a => a).ToList();
                var xElementResult = result[0];
                string jsonText = JsonConvert.SerializeObject(result[0]);
                return jsonText;
            }

        }


        public string DAMGetAdminSettings(DigitalAssetManagerProxy proxy, string LogoSettings)
        {

            // string xmlpath =  AppDomain.CurrentDomain.BaseDirectory +"\\AdminSettings.xml";
            string jsonText = "";
            string xmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
            XDocument adminXdoc = XDocument.Load(xmlpath);
            var result = adminXdoc.Descendants("DAMsettings").Descendants(LogoSettings).FirstOrDefault();
            if (result != null)
            {


                var abc = adminXdoc.Descendants("DAMsettings").Descendants(LogoSettings).ToList();

                if (abc.Count > 0)
                {
                    jsonText = JsonConvert.SerializeObject(abc[0]);
                }

            }
            return jsonText;
        }
        public string GetXmlWorkingPath()
        {
            string mappingfilesPath = AppDomain.CurrentDomain.BaseDirectory;
            if (!MarcomManagerFactory.viewOldMetadataVersion)
            {
                if (MarcomManagerFactory.IsWorkingWithCurrentWorkingVersion)
                    mappingfilesPath = mappingfilesPath + "MetadataXML" + @"\CurrentMetadataWorking.xml";
                else
                    mappingfilesPath = mappingfilesPath + "MetadataXML" + @"\FutureMetadataWorking.xml";
            }
            else
                mappingfilesPath = mappingfilesPath + "MetadataXML" + @"\MetadataVersion_V" + MarcomManagerFactory.oldMetadataVersionNumber + ".xml";
            if (System.IO.File.Exists(mappingfilesPath))
                return mappingfilesPath;
            else
                return null;
        }

        // Tuple<IList<ISubscriptionType>, string[], string[]>
        public Tuple<IList, IList> GetDAMExtensions(DigitalAssetManagerProxy proxy)
        {
            try
            {
                IList data = null;
                IList dataExt = null;
                string xmlPath = string.Empty;
                int version = MarcomManagerFactory.ActiveMetadataVersionNumber;
                string adminxmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
                XDocument adminXmlDoc = XDocument.Load(adminxmlpath);
                IList<IEntityTypeStatusOptions> listentity = new List<IEntityTypeStatusOptions>();
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    xmlPath = tx.PersistenceManager.MetadataRepository.GetXmlPath(version);
                    XDocument xDoc = XDocument.Load(xmlPath);
                    var rddd = (from DAMtypes in xDoc.Root.Elements("EntityType_Table").Elements("EntityType")
                                //join DAMstatusoptions in xDoc.Root.Elements("DamTypeFileExtension_Options_Table").Elements("DamTypeFileExtension_Options") on Convert.ToInt32(DAMtypes.Element("ID").Value) equals Convert.ToInt32(DAMstatusoptions.Element("EntityTypeID").Value)
                                where Convert.ToInt32(DAMtypes.Element("ModuleID").Value) == 5
                                select new
                                {
                                    damID = Convert.ToInt16(DAMtypes.Element("ID").Value),
                                    damCaption = Convert.ToString(DAMtypes.Element("Caption").Value),
                                    //damExtention = Convert.ToString(DAMstatusoptions.Element("ExtensionOptions").Value),
                                    Id = Convert.ToInt16(DAMtypes.Element("ID").Value)
                                }).ToList();
                    data = rddd;

                    var rdddEXT = (from DAMtypes in xDoc.Root.Elements("EntityType_Table").Elements("EntityType")
                                   join DAMstatusoptions in xDoc.Root.Elements("DamTypeFileExtension_Options_Table").Elements("DamTypeFileExtension_Options") on Convert.ToInt32(DAMtypes.Element("ID").Value) equals Convert.ToInt32(DAMstatusoptions.Element("EntityTypeID").Value)
                                   where Convert.ToInt32(DAMtypes.Element("ModuleID").Value) == 5
                                   select new
                                   {
                                       //damID = Convert.ToInt16(DAMtypes.Element("ID").Value),
                                       //damCaption = Convert.ToString(DAMtypes.Element("Caption").Value),
                                       damExtention = Convert.ToString(DAMstatusoptions.Element("ExtensionOptions").Value),
                                       Id = Convert.ToInt16(DAMtypes.Element("ID").Value)
                                   }).ToList();
                    dataExt = rdddEXT;

                }

                return Tuple.Create(data, dataExt);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public IList<IDamTypeAttributeRelation> GetDamAttributeRelation(DigitalAssetManagerProxy proxy, int damID)
        {
            try
            {

                IList data = null;
                int version = MarcomManagerFactory.ActiveMetadataVersionNumber;
                IList<IDamTypeAttributeRelation> _iiDamtoAttrRel = new List<IDamTypeAttributeRelation>();
                IList<DamTypeAttributeRelation> dao = new List<DamTypeAttributeRelation>();
                IList<EntityTypeAttributeRelationDao> daoAttrType = new List<EntityTypeAttributeRelationDao>();
                // var xmetadataDoc = null;
                string xmlpath = string.Empty;
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    xmlpath = tx.PersistenceManager.MetadataRepository.GetXmlPath(version);
                    daoAttrType = tx.PersistenceManager.MetadataRepository.GetObject<EntityTypeAttributeRelationDao>(xmlpath);
                    var entityttyperesult = daoAttrType.Where(a => a.EntityTypeID == damID).OrderBy(x => x.SortOrder);
                    var attrIDs = entityttyperesult.Select(a => a.AttributeID).ToList();
                    IList<IOption> optionSelection = GetOptionList(proxy, attrIDs);
                    string Adminxmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
                    XDocument xDoc = XDocument.Load(Adminxmlpath);


                    var adminxdoc = from item in xDoc.Root.Elements("DAMsettings").Elements("AssetCreation").Elements("AssetType") where Convert.ToInt32(item.Attribute("ID").Value) == damID select item;

                    XDocument metadataxDoc = XDocument.Load(xmlpath);
                    var rddd = (from AdminAttributes in adminxdoc.Elements("Attributes").Elements("Attribute")
                                join MetadataAttributes in metadataxDoc.Root.Elements("EntityTypeAttributeRelation_Table").Elements("EntityTypeAttributeRelation") on damID equals Convert.ToInt32(MetadataAttributes.Element("EntityTypeID").Value)
                                where Convert.ToInt32(AdminAttributes.Element("Id").Value) == Convert.ToInt32(MetadataAttributes.Element("AttributeID").Value)
                                select new
                                {
                                    ID = Convert.ToInt16(AdminAttributes.Element("Id").Value),
                                    Caption = Convert.ToString(MetadataAttributes.Element("Caption").Value),
                                    SotOrder = Convert.ToInt32(AdminAttributes.Element("SortOrder").Value),
                                    DefaultValue = Convert.ToString(MetadataAttributes.Element("DefaultValue").Value)
                                }).ToList();

                    //Get User Details metadata values
                    IList<IAttributeData> entityUserAttrVal = new List<IAttributeData>();
                    entityUserAttrVal = proxy.MarcomManager.PlanningManager.GetEntityAttributesDetailsUserDetails(proxy.MarcomManager.User.Id);

                    string InheritVal = "";
                    foreach (var it in rddd)
                    {
                        int typeID = Convert.ToInt32(metadataxDoc.Root.Elements("Attribute_Table").Elements("Attribute").Where(a => Convert.ToInt32(a.Element("ID").Value) == Convert.ToInt32(it.ID)).Select(a => a.Element("AttributeTypeID").Value).First());
                        IList<IOption> optionSinglrSelection = null;
                        // IList<IOption> optionmultiselection = null;
                        if (typeID == (int)AttributesList.ListSingleSelection)
                        {
                            optionSinglrSelection = (from options in optionSelection
                                                     where options.AttributeID == it.ID
                                                     select options).OrderBy(a => a.Caption).ToList<IOption>();
                        }
                        else if (typeID == (int)AttributesList.ListMultiSelection)
                        {
                            optionSinglrSelection = (from options in optionSelection
                                                     where options.AttributeID == it.ID
                                                     select options).OrderBy(a => a.SortOrder).ToList<IOption>();
                        }

                        //If asset type value is null call the user metadata value
                        if (it.DefaultValue == "")
                        {
                            if ((entityUserAttrVal.Where(a => a.ID == it.ID).Select(a => a.Value).ToList().Count) > 0)
                            {
                                InheritVal = (string)entityUserAttrVal.Where(a => a.ID == it.ID).Select(a => Convert.ToString(a.Value)).FirstOrDefault();
                            }
                        }
                        else
                            InheritVal = Convert.ToString(it.DefaultValue);

                        _iiDamtoAttrRel.Add(new DamTypeAttributeRelation { ID = Convert.ToInt32(it.ID), Caption = Convert.ToString(it.Caption), SortOrder = Convert.ToInt32(it.SotOrder), TypeID = typeID, Options = optionSinglrSelection, DefaultValue = InheritVal });
                    }
                }
                return _iiDamtoAttrRel.OrderBy(a => a.SortOrder).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<IOption> GetOptionList(DigitalAssetManagerProxy proxy, List<int> options, bool IsforAdmin = false)
        {

            try
            {
                int version = MarcomManagerFactory.ActiveMetadataVersionNumber;
                if (IsforAdmin)
                    version = MarcomManagerFactory.AdminMetadataVersionNumber;
                string xmlpath = string.Empty;
                IList<IOption> _iioption = new List<IOption>();
                IList<OptionDao> dao = new List<OptionDao>();
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    if (IsforAdmin == false)
                        xmlpath = tx.PersistenceManager.MetadataRepository.GetXmlPath(version);
                    else
                        xmlpath = GetXmlWorkingPath();

                    if (version == 0)
                    {
                        dao = tx.PersistenceManager.MetadataRepository.GetAll<OptionDao>();

                    }
                    else
                    {
                        dao = (from item in tx.PersistenceManager.MetadataRepository.GetObject<OptionDao>(xmlpath) where options.Contains(item.AttributeID) select item).ToList<OptionDao>();

                    }
                    foreach (var item in dao)
                    {
                        IOption _ioption = new Option();
                        _ioption.Caption = item.Caption;
                        _ioption.AttributeID = item.AttributeID;
                        _ioption.SortOrder = item.SortOrder;
                        _ioption.Id = item.Id;
                        _iioption.Add(_ioption);
                    }
                    tx.Commit();
                }
                return _iioption;
            }
            catch (Exception ex)
            {
                return null;
            }
        }





        public int CreateAsset(DigitalAssetManagerProxy proxy, int FolderID, int TypeId, string Name, IList<IAttributeData> listattributevalues, string FileName, int VersionNo, string MimeType, string Extension, long Size, int EntityID, String FileGuid, string Description, bool IsforDuplicate = false, int Duplicatefilestatus = 0, int LinkedAssetID = 0, string fileAdditionalinfo = null, string strAssetAccess = null, int SourceAssetID = 0, bool IsforAdmin = false, int assetactioncode = 0, bool blnAttach = false)
        {
            int version = MarcomManagerFactory.ActiveMetadataVersionNumber;
            int AssetID = 0;
            int ActiveFileID = 0;
            int OwnerID = 0;

            try
            {
                Guid NewId = Guid.NewGuid();
                OwnerID = Convert.ToInt32(proxy.MarcomManager.User.Id);
                if (!IsforDuplicate)
                {

                    string filePath = ReadAdminXML("FileManagment");
                    var DirInfo = System.IO.Directory.GetParent(filePath);
                    string newFilePath = DirInfo.FullName;
                    System.IO.File.Move(filePath + "\\" + FileGuid + Extension, newFilePath + "\\" + NewId + Extension);
                }


                BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("-----------------------------------------------------------------------------------------------", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);
                BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("Started creating Asset", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);

                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {

                    if (IsforAdmin)
                        version = MarcomManagerFactory.AdminMetadataVersionNumber;
                    string xmlpath = tx.PersistenceManager.MetadataRepository.GetXmlPath(version);
                    XDocument docx = XDocument.Load(xmlpath);
                    var rddd = (from EntityAttrRel in docx.Root.Elements("EntityTypeAttributeRelation_Table").Elements("EntityTypeAttributeRelation")
                                join Attr in docx.Root.Elements("Attribute_Table").Elements("Attribute") on Convert.ToInt32(EntityAttrRel.Element("AttributeID").Value) equals Convert.ToInt32(Attr.Element("ID").Value)
                                where Convert.ToInt32(EntityAttrRel.Element("EntityTypeID").Value) == TypeId && EntityAttrRel.Element("DefaultValue").Value != ""
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
                                    IsReadOnly = Convert.ToBoolean(Convert.ToInt32(EntityAttrRel.Element("IsReadOnly").Value)),
                                    DefaultValue = EntityAttrRel.Element("DefaultValue").Value
                                }).ToList();

                    var attributesdetails = rddd;


                    List<int> generalAttributes = new List<int>();
                    generalAttributes = listattributevalues.Select(a => a.ID).ToList();
                    generalAttributes.Add((int)SystemDefinedAttributes.MyRoleEntityAccess);
                    generalAttributes.Add((int)SystemDefinedAttributes.MyRoleGlobalAccess);
                    generalAttributes.Add((int)SystemDefinedAttributes.EntityOnTimeStatus);
                    generalAttributes.Add((int)SystemDefinedAttributes.Name);
                    generalAttributes.Add((int)SystemDefinedAttributes.EntityStatus);

                    attributesdetails = attributesdetails.Where(a => !generalAttributes.Contains(a.ID)).ToList();


                    AssetsDao assetdao = new AssetsDao();
                    assetdao.CreatedBy = OwnerID;
                    assetdao.Name = HttpUtility.HtmlEncode(Name);
                    assetdao.FolderID = FolderID;
                    assetdao.EntityID = EntityID;
                    assetdao.Createdon = DateTime.UtcNow;
                    assetdao.UpdatedOn = DateTime.UtcNow;
                    assetdao.ActiveFileID = 0;
                    assetdao.AssetTypeid = TypeId;
                    assetdao.LinkedAssetID = LinkedAssetID;
                    assetdao.AssetAccess = strAssetAccess;
                    tx.PersistenceManager.PlanningRepository.Save<AssetsDao>(assetdao);


                    //insert the Attributes other than default ones to listattributevalues
                    if (attributesdetails != null)
                    {

                        foreach (var item in attributesdetails)
                        {
                            string[] defDatalist = item.DefaultValue.Split(',').ToArray();
                            foreach (var defData in defDatalist)
                            {
                                IAttributeData newdata = new AttributeData();
                                newdata.AttributeRecordID = 0;
                                newdata.Caption = item.Caption;
                                newdata.DropDownPricing = null;
                                newdata.ID = item.ID;
                                newdata.IsChooseFromParent = false;
                                newdata.IsInheritFromParent = false;
                                newdata.IsLock = false;
                                newdata.IsReadOnly = false;
                                newdata.IsSpecial = false;
                                newdata.Lable = null;
                                newdata.Level = 0;
                                newdata.SortOrder = 0;
                                newdata.specialValue = null;
                                newdata.SpecialValue = null;
                                newdata.tree = null;
                                newdata.TypeID = item.AttributeTypeID;
                                newdata.Value = defData;
                                listattributevalues.Add(newdata);
                            }
                        }
                    }


                    if (listattributevalues != null)
                    {
                        var result = InsertAssetAttributes(tx, listattributevalues, assetdao.ID, TypeId, SourceAssetID, IsforDuplicate);
                    }

                    if (FileGuid != null || FileGuid == "")
                    {
                        if (!IsforDuplicate)
                        {
                            DAMFileDao damdao = new DAMFileDao();
                            damdao.CreatedOn = DateTime.UtcNow;
                            damdao.Extension = Extension;
                            damdao.MimeType = MimeType;
                            damdao.Name = HttpUtility.HtmlEncode(FileName);
                            damdao.OwnerID = OwnerID;
                            damdao.Size = Size;
                            damdao.VersionNo = VersionNo;
                            damdao.Description = HttpUtility.HtmlEncode(Description);
                            damdao.FileGuid = NewId;
                            damdao.AssetID = assetdao.ID;
                            //if (IsforDuplicate)
                            //{
                            //    damdao.Status = Duplicatefilestatus;
                            //    damdao.Additionalinfo = fileAdditionalinfo;
                            //}
                            tx.PersistenceManager.CommonRepository.Save<DAMFileDao>(damdao);
                            ActiveFileID = damdao.ID;
                        }
                        else
                        {
                            List<IAssets> assetdet = new List<IAssets>();
                            IAssets asset = new Assets();
                            asset = GetAssetAttributesDetails(proxy, SourceAssetID, false);
                            List<IDAMFile> iifilelist = new List<IDAMFile>();
                            IDAMFile assetfile = new DAMFile();
                            string filePath = ReadAdminXML("FileManagment");
                            string PreviewfilePath = filePath.Replace("Original", "Preview");
                            var DirInfo = System.IO.Directory.GetParent(filePath);
                            string newFilePath = DirInfo.FullName;
                            iifilelist = asset.Files.Select(a => a).ToList();
                            Dictionary<string, string> fileidpath = new Dictionary<string, string>();
                            foreach (var value in iifilelist)
                            {
                                Guid NewId1 = Guid.NewGuid();
                                string Newfilevalue = NewId1.ToString();
                                fileidpath.Add(value.Fileguid.ToString().ToLower() + value.Extension.ToString().ToLower(), Newfilevalue);
                                //string smallfile = PreviewfilePath + "\\Small_" + value.Fileguid.ToString().ToLower() + ".jpg";
                                //string Bigfile = PreviewfilePath + "\\Big_" + value.Fileguid.ToString().ToLower() + ".jpg";
                                //if (System.IO.File.Exists(filePath + "\\" + value.Fileguid.ToString().ToLower() + value.Extension.ToString().ToLower()))
                                //    System.IO.File.Copy(filePath + "\\" + value.Fileguid.ToString().ToLower() + Extension, newFilePath + "\\" + NewId1 + value.Extension.ToString().ToLower());
                                //try
                                //{
                                //    if (System.IO.File.Exists(smallfile))
                                //    {
                                //        System.IO.File.Copy(smallfile, PreviewfilePath + "\\Small_" + NewId1.ToString().ToLower() + ".jpg");
                                //    }
                                //    if (System.IO.File.Exists(Bigfile))
                                //    {
                                //        System.IO.File.Copy(Bigfile, PreviewfilePath + "\\Big_" + NewId1.ToString().ToLower() + ".jpg");
                                //    }
                                //}
                                //catch (Exception ex)
                                //{
                                //}

                                DAMFileDao damdao = new DAMFileDao();
                                damdao.CreatedOn = DateTime.UtcNow;
                                damdao.Extension = value.Extension;
                                damdao.MimeType = value.MimeType;
                                damdao.Name = HttpUtility.HtmlEncode(value.Name);
                                damdao.OwnerID = OwnerID;
                                damdao.Size = value.Size;
                                damdao.VersionNo = value.VersionNo;
                                damdao.Description = HttpUtility.HtmlEncode(value.Description);
                                damdao.FileGuid = NewId1;
                                damdao.AssetID = assetdao.ID;
                                damdao.Status = value.Status;
                                damdao.Additionalinfo = value.Additionalinfo;
                                tx.PersistenceManager.CommonRepository.Save<DAMFileDao>(damdao);
                                if (FileGuid.ToString().ToLower() == value.Fileguid.ToString().ToLower())
                                {
                                    ActiveFileID = damdao.ID;
                                }

                            }
                            //Assetimagecopywiththreading
                            //System.Threading.Tasks.Task taskforAssetimagecopy = new System.Threading.Tasks.Task(() => proxy.MarcomManager.DigitalAssetManager.DAMadminSettingsDeleteAttributeRelationAllViews(entitytypeid, attributeid));
                            System.Threading.Tasks.Task taskforAssetimagecopy = new System.Threading.Tasks.Task(() => Assetimagecopy(fileidpath, filePath, PreviewfilePath, newFilePath));
                            taskforAssetimagecopy.Start();
                            //Assetimagecopy(fileidpath,filePath,PreviewfilePath, newFilePath);

                        }
                    }
                    BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("Asset is saved in DAM_Asset with assetId : " + AssetID, BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);
                    tx.Commit();
                    AssetID = assetdao.ID;
                }
                using (ITransaction tx1 = proxy.MarcomManager.GetTransaction())
                {
                    AssetsDao assetdaoforID = new AssetsDao();
                    IList<AssetsDao> lassetDao = new List<AssetsDao>();
                    lassetDao = tx1.PersistenceManager.PlanningRepository.Query<AssetsDao>().Where(a => a.ID == AssetID).Select(a => a).ToList<AssetsDao>();
                    if (lassetDao != null)
                    {
                        if (lassetDao.Count > 0)
                        {
                            assetdaoforID = lassetDao.FirstOrDefault();
                            assetdaoforID.ActiveFileID = ActiveFileID;
                            tx1.PersistenceManager.PlanningRepository.Save<AssetsDao>(assetdaoforID);
                        }
                    }
                    tx1.Commit();
                }

                try
                {
                    if (FolderID == 0 && blnAttach == true)
                    {
                        BrandSystems.Marcom.Core.Utility.FeedNotificationServer fs1 = new Utility.FeedNotificationServer();
                        NotificationFeedObjects obj1 = new NotificationFeedObjects();
                        obj1.action = "Attach Asset";
                        obj1.Actorid = proxy.MarcomManager.User.Id;
                        obj1.EntityId = EntityID;
                        obj1.AttributeName = HttpUtility.HtmlEncode(Name);
                        obj1.TypeName = "Attach Asset";
                        obj1.CreatedOn = DateTimeOffset.Now;
                        obj1.AssociatedEntityId = AssetID;
                        if (ActiveFileID > 0)
                        {
                            using (ITransaction tx2 = proxy.MarcomManager.GetTransaction())
                            {
                                DAMFileDao fileinfo = new DAMFileDao();
                                fileinfo = tx2.PersistenceManager.PlanningRepository.Query<DAMFileDao>().Where(a => a.ID == ActiveFileID).Select(a => a).FirstOrDefault();
                                obj1.Version = fileinfo.VersionNo;
                                tx2.Commit();
                            }

                        }
                        else
                            obj1.Version = 1;
                        fs1.AsynchronousNotify(obj1);

                    }
                    else if (FolderID == 0 && assetactioncode == 1)
                    {
                        BrandSystems.Marcom.Core.Utility.FeedNotificationServer fs1 = new Utility.FeedNotificationServer();
                        NotificationFeedObjects obj1 = new NotificationFeedObjects();
                        obj1.action = "Moved Asset";
                        obj1.Actorid = proxy.MarcomManager.User.Id;
                        obj1.EntityId = EntityID;
                        obj1.AttributeName = HttpUtility.HtmlEncode(Name);
                        obj1.TypeName = "Moved Asset";
                        obj1.CreatedOn = DateTimeOffset.Now;
                        obj1.AssociatedEntityId = AssetID;
                        if (ActiveFileID > 0)
                        {
                            using (ITransaction tx2 = proxy.MarcomManager.GetTransaction())
                            {
                                DAMFileDao fileinfo = new DAMFileDao();
                                fileinfo = tx2.PersistenceManager.PlanningRepository.Query<DAMFileDao>().Where(a => a.ID == ActiveFileID).Select(a => a).FirstOrDefault();
                                obj1.Version = fileinfo.VersionNo;
                                tx2.Commit();
                            }

                        }
                        else
                            obj1.Version = 1;
                        fs1.AsynchronousNotify(obj1);

                    }
                    else if (FolderID == 0 && assetactioncode == 2)
                    {
                        BrandSystems.Marcom.Core.Utility.FeedNotificationServer fs1 = new Utility.FeedNotificationServer();
                        NotificationFeedObjects obj1 = new NotificationFeedObjects();
                        obj1.action = "Copy Asset";
                        obj1.Actorid = proxy.MarcomManager.User.Id;
                        obj1.EntityId = EntityID;
                        obj1.AttributeName = HttpUtility.HtmlEncode(Name);
                        obj1.TypeName = "Copy Asset";
                        obj1.CreatedOn = DateTimeOffset.Now;
                        obj1.AssociatedEntityId = AssetID;
                        if (ActiveFileID > 0)
                        {
                            using (ITransaction tx2 = proxy.MarcomManager.GetTransaction())
                            {
                                DAMFileDao fileinfo = new DAMFileDao();
                                fileinfo = tx2.PersistenceManager.PlanningRepository.Query<DAMFileDao>().Where(a => a.ID == ActiveFileID).Select(a => a).FirstOrDefault();
                                obj1.Version = fileinfo.VersionNo;
                                tx2.Commit();
                            }

                        }
                        else
                            obj1.Version = 1;
                        fs1.AsynchronousNotify(obj1);

                    }
                    else
                    {
                        BrandSystems.Marcom.Core.Utility.FeedNotificationServer fs = new Utility.FeedNotificationServer();
                        NotificationFeedObjects obj = new NotificationFeedObjects();
                        obj.action = "create Asset";
                        obj.Actorid = proxy.MarcomManager.User.Id;
                        obj.EntityId = EntityID;
                        obj.AttributeName = HttpUtility.HtmlEncode(Name);
                        obj.TypeName = "Asset creation";
                        obj.CreatedOn = DateTimeOffset.Now;
                        obj.AssociatedEntityId = AssetID;
                        if (ActiveFileID > 0)
                        {
                            using (ITransaction tx2 = proxy.MarcomManager.GetTransaction())
                            {
                                DAMFileDao fileinfo = new DAMFileDao();
                                fileinfo = tx2.PersistenceManager.PlanningRepository.Query<DAMFileDao>().Where(a => a.ID == ActiveFileID).Select(a => a).FirstOrDefault();
                                obj.Version = fileinfo.VersionNo;
                                tx2.Commit();
                            }

                        }
                        else
                            obj.Version = 1;
                        fs.AsynchronousNotify(obj);
                    }

                    BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("Updated the Feeds", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);

                }
                catch (Exception ex)
                {
                    BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("error in feed" + " " + ex.Message, BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);
                }

                try
                {
                    BrandSystems.Marcom.Core.Interface.Managers.IEventManager _eventManager = null;
                    BrandSystems.Marcom.Core.Interface.Managers.IPluginManager _pluginManager = null;
                    MarcomManager marcommanager = new MarcomManager(_eventManager, _pluginManager);
                    BrandSystems.Marcom.Core.Managers.Proxy.PlanningManagerProxy pProxy = new BrandSystems.Marcom.Core.Managers.Proxy.PlanningManagerProxy(marcommanager);
                    System.Threading.Tasks.Task addtaskforsearch = new System.Threading.Tasks.Task(() => PlanningManager.Instance.AddEntityAsyncDam(pProxy, AssetID, ActiveFileID, "Attachments"));
                    addtaskforsearch.Start();
                }
                catch { }

                if (!IsforDuplicate && FolderID == 0)
                {
                    ReinitializeTask(proxy, EntityID);
                }

                return AssetID;
            }
            catch (MarcomAccessDeniedException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                BrandSystems.Marcom.Core.Metadata.LogHandler.LogError("Failed to create Asset", ex);
                BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("-----------------------------------------------------------------------------------------------", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);
                return 0;
            }
        }


        public bool InsertAssetAttributes(ITransaction tx, IList<IAttributeData> attributeData, int entityId, int typeId, int SourceAssetID = 0, bool IsforDuplicate = false)
        {
            if (attributeData != null)
            {
                BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("Started inseting values into Dynamic tables", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);

                string entityName = "AttributeRecord" + typeId + "_V" + MarcomManagerFactory.ActiveMetadataVersionNumber;
                IList<BrandSystems.Marcom.Core.Metadata.Interface.IDynamicAttributes> listdynamicattributes = new List<BrandSystems.Marcom.Core.Metadata.Interface.IDynamicAttributes>();
                Dictionary<string, object> dictAttr = new Dictionary<string, object>();
                IList<DamMultiSelectValueDao> listMultiselect = new List<DamMultiSelectValueDao>();
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
                                listreeval.Add(tre);
                            }
                        }
                        tx.PersistenceManager.PlanningRepository.Save<Marcom.Dal.Metadata.Model.TreeValueDao>(listreeval);
                    }
                }
                if (multiAttrResult.Count() > 0)
                {
                    tx.PersistenceManager.PlanningRepository.DeleteByID<Marcom.Dal.Metadata.Model.MultiSelectDao>(entityId);

                    string deletequery = "DELETE FROM MM_DAM_MultiSelectValue WHERE AssetID = ? ";
                    tx.PersistenceManager.DamRepository.ExecuteQuerywithMinParam(deletequery.ToString(), Convert.ToInt32(entityId));

                    if (!IsforDuplicate)
                    {
                        var query = attributeData.Join(multiAttrResult,
                                 post => post.ID,
                                 meta => meta.Id,
                                 (post, meta) => new { databaseval = post, attrappval = meta });
                        foreach (var at in query)
                        {
                            Marcom.Dal.DAM.Model.DamMultiSelectValueDao mt = new Marcom.Dal.DAM.Model.DamMultiSelectValueDao();
                            mt.AttributeID = at.databaseval.ID;
                            mt.AssetID = entityId;
                            mt.OptionID = Convert.ToInt32(at.databaseval.Value);
                            listMultiselect.Add(mt);
                        }
                        tx.PersistenceManager.DamRepository.Save<Marcom.Dal.DAM.Model.DamMultiSelectValueDao>(listMultiselect);
                    }
                    else
                    {
                        if (SourceAssetID > 0)
                        {
                            StringBuilder insertquery = new StringBuilder();
                            insertquery.AppendLine(" INSERT INTO [dbo].[MM_DAM_MultiSelectValue]([AssetID],[AttributeID],[OptionID])");
                            insertquery.AppendLine(" SELECT " + Convert.ToInt32(entityId) + ",AttributeID,OptionID FROM MM_DAM_MultiSelectValue WHERE  AssetID= ? ");
                            tx.PersistenceManager.DamRepository.ExecuteQuerywithMinParam(insertquery.ToString(), SourceAssetID);
                        }

                    }
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
                                    value = Convert.ToString((ab.databaseval.Value == null) || Convert.ToString(ab.databaseval.Value) == "" ? "" : HttpUtility.HtmlEncode((string)ab.databaseval.Value));
                                    break;
                                }
                            case 3:
                                {
                                    value = Convert.ToString((Convert.ToString(ab.databaseval.Value) == null || Convert.ToString(ab.databaseval.Value) == "") ? 0 : Convert.ToString(ab.databaseval.Value));
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
                                    value = Convert.ToInt32(((ab.databaseval.Value == null) ? 0 : (int)ab.databaseval.Value));
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



        private string ReadAdminXML(string elementNode)
        {
            string xmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
            XDocument xDoc = XDocument.Load(xmlpath);
            if (xDoc.Root.Elements(elementNode).Count() > 0)
            {
                return (string)xDoc.Root.Elements(elementNode).FirstOrDefault().Elements("FolderPath").First();
            }
            string uploadImagePath = Path.Combine(HttpRuntime.AppDomainAppPath);
            return uploadImagePath = uploadImagePath + "DAMFiles\\Original\\";
        }


        private string ReadAdminXML_uploadedfiles(string elementNode)
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

        /// <summary>
        /// Gets the Entity Dam Folder.
        /// </summary>
        /// <param name="attributeID">The EntityID.</param>
        /// <returns>string</returns>
        public string GetEntityDamFolder(DigitalAssetManagerProxy proxy, int entityID)
        {
            try
            {
                IList<IFolder> _iitreenode = new List<IFolder>();
                IList<FolderDao> dao = new List<FolderDao>();


                string tree = string.Empty;

                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {

                    dao = tx.PersistenceManager.DamRepository.Query<FolderDao>().Where(a => a.EntityID == entityID).ToList();

                    var parentNode = from node in dao
                                     where node.ParentNodeID == 0
                                     select node;
                    UITreeNode uiNode = new UITreeNode();
                    uiNode.Caption = "Root";
                    uiNode.Description = "Root";
                    uiNode.Level = 0;
                    uiNode.id = 0;
                    uiNode.SortOrder = 0;
                    uiNode.IsDeleted = false;
                    uiNode.Key = entityID.ToString();
                    uiNode.ColorCode = "ffd300";
                    uiNode.Children = new List<UITreeNode>();
                    RecursionTreeSerialization(uiNode, parentNode.OrderBy(a => a.SortOrder).ToList<FolderDao>(), dao);
                    tree = JsonConvert.SerializeObject(uiNode);

                    tx.Commit();
                }
                return tree;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public void RecursionTreeSerialization(UITreeNode uiParentNode, IList<FolderDao> nodes, IList<FolderDao> allNode)
        {
            foreach (FolderDao node in nodes)
            {
                UITreeNode uiNode = new UITreeNode();
                uiNode.Caption = node.Caption;
                uiNode.Description = node.Description;
                uiNode.Level = node.Level;
                uiNode.id = node.Id;
                uiNode.SortOrder = node.SortOrder;
                uiNode.Key = node.KEY;
                uiNode.IsDeleted = false;
                uiNode.ColorCode = node.Colorcode;
                uiNode.Children = new List<UITreeNode>();
                uiParentNode.Children.Add(uiNode);
                var children = from nodeChildren in allNode
                               where nodeChildren.ParentNodeID == node.Id
                               select nodeChildren;
                if (children.Count() > 0)
                    RecursionTreeSerialization(uiNode, children.OrderBy(a => a.SortOrder).ToList<FolderDao>(), allNode);
            }

        }


        public List<object> GetAssets(DigitalAssetManagerProxy proxy, int folderid, int entityID, int viewType, int orderbyid, int pageNo, bool ViewAll = false)
        {

            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    List<object> returnObj = new List<object>();

                    string viewName = Enum.GetName(typeof(AssetView), viewType);
                    int totalrecords = 20;
                    if (viewType == (int)AssetView.ListView)
                        totalrecords = 30;
                    if (viewType == (int)AssetView.SummaryView)
                        totalrecords = 10;
                    if (ViewAll == true)
                        totalrecords = 5000;
                    StringBuilder assetQuery = new StringBuilder();
                    IList<MultiProperty> assetQuery_parLIST = new List<MultiProperty>();
                    if (ViewAll == false)
                        assetQuery_parLIST.Add(new MultiProperty { propertyName = "folderid", propertyValue = folderid });
                    assetQuery_parLIST.Add(new MultiProperty { propertyName = "entityID", propertyValue = entityID });

                    assetQuery.AppendLine(" DECLARE @RowsPerPage INT = " + totalrecords + ", ");
                    assetQuery.AppendLine(" @PageNumber INT =" + pageNo + " ");
                    assetQuery.AppendLine(" DECLARE @OrderBy INT = " + orderbyid + "; ");
                    assetQuery.AppendLine(" SELECT a.ID AS 'FileUniqueID', ");
                    assetQuery.AppendLine(" a.Name as 'FileName', ");
                    assetQuery.AppendLine(" a.Extension, ");
                    assetQuery.AppendLine(" a.[Size],  a.[VersionNo], ");
                    assetQuery.AppendLine(" a.OwnerID, ");
                    assetQuery.AppendLine(" a.CreatedOn, ");
                    assetQuery.AppendLine(" a.FileGuid, ");
                    assetQuery.AppendLine(" a.[Description], ");
                    assetQuery.AppendLine(" a.AssetID, ");
                    assetQuery.AppendLine(" da.FolderID, ");
                    assetQuery.AppendLine(" da.EntityID, ");
                    assetQuery.AppendLine(" da.ID AS 'AssetUniqueID', ");
                    assetQuery.AppendLine(" da.Name as 'AssetName', da.Url as 'LinkURL', a.MimeType as 'MimeType', ");
                    assetQuery.AppendLine(" da.AssetTypeid, ");
                    assetQuery.AppendLine(" da.ActiveFileID, ");
                    assetQuery.AppendLine(" met.ColorCode, ");
                    assetQuery.AppendLine(" met.ShortDescription, ");
                    assetQuery.AppendLine(" a.[Status], ");
                    assetQuery.AppendLine(" da.Category, da.IsPublish, ISNULL(da.LinkedAssetID,0) as LinkedAssetID ");
                    assetQuery.AppendLine(" FROM   DAM_File a ");
                    assetQuery.AppendLine(" RIGHT OUTER JOIN DAM_Asset da ");
                    assetQuery.AppendLine(" ON  a.AssetID = da.ID AND a.ID=da.ActiveFileID");
                    assetQuery.AppendLine(" INNER JOIN MM_EntityType met ");
                    assetQuery.AppendLine(" ON  met.ID = da.AssetTypeid ");
                    assetQuery.AppendLine(" WHERE  da.id IN (SELECT da2.id ");
                    assetQuery.AppendLine(" FROM   DAM_Asset da2 ");
                    assetQuery.AppendLine(" WHERE ");
                    if (ViewAll == false)
                        assetQuery.AppendLine(" da2.FolderID = :folderid  AND ");
                    assetQuery.AppendLine(" da2.entityid = :entityID)  ");
                    assetQuery.AppendLine("  ORDER BY  ");
                    assetQuery.AppendLine(" CASE   ");
                    assetQuery.AppendLine(" WHEN @OrderBy = 1  ");
                    assetQuery.AppendLine(" THEN da.Name  ");
                    assetQuery.AppendLine(" END       asc,  ");
                    assetQuery.AppendLine(" CASE   ");
                    assetQuery.AppendLine(" WHEN @OrderBy = 2  ");
                    assetQuery.AppendLine(" THEN da.Name  ");
                    assetQuery.AppendLine(" END desc,  ");
                    assetQuery.AppendLine(" CASE   ");
                    assetQuery.AppendLine(" WHEN @OrderBy = 3  ");
                    assetQuery.AppendLine(" THEN da.Createdon   ");
                    assetQuery.AppendLine(" END       asc , ");
                    assetQuery.AppendLine(" CASE   ");
                    assetQuery.AppendLine(" WHEN @OrderBy = 4 ");
                    assetQuery.AppendLine(" THEN da.Createdon   ");
                    assetQuery.AppendLine(" END       desc  ");

                    assetQuery.AppendLine(" OFFSET(@PageNumber - 1) * @RowsPerPage ROWS ");

                    assetQuery.AppendLine(" FETCH NEXT @RowsPerPage ROWS ONLY ");


                    IList assets = tx.PersistenceManager.DamRepository.ExecuteQuerywithParam(assetQuery.ToString(), assetQuery_parLIST);

                    int version = MarcomManagerFactory.ActiveMetadataVersionNumber;
                    string xmlpath = string.Empty;

                    xmlpath = tx.PersistenceManager.MetadataRepository.GetXmlPath(version);
                    var xmetadataDoc = XDocument.Load(xmlpath);

                    string Adminxmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
                    XDocument xDoc = XDocument.Load(Adminxmlpath);


                    IList<XElement> tempresults = new List<XElement>();
                    IList<XElement> result = new List<XElement>();

                    if (viewType == (int)AssetView.ThumbnailView)
                    {
                        tempresults = xDoc.Descendants("DAMsettings").Descendants(viewName).Descendants("AssetType").Descendants("Attributes").Descendants("Attribute").Where(a => Convert.ToBoolean(a.Element("IsColumn").Value) == true).Select(a => a).ToList();
                        foreach (var rec in tempresults)
                        {
                            IList<XElement> duplist = new List<XElement>();
                            duplist = result.Where(a => Convert.ToInt32(a.Element("Id").Value) == Convert.ToInt32(rec.Element("Id").Value)).Select(a => a).ToList();
                            if (duplist.Count == 0)
                                result.Add(rec);
                        }
                    }
                    else if (viewType == (int)AssetView.SummaryView)
                    {
                        IList<XElement> thumpnail = xDoc.Descendants("DAMsettings").Descendants("ThumbnailView").Descendants("AssetType").Descendants("Attributes").Descendants("Attribute").Where(a => Convert.ToBoolean(a.Element("IsColumn").Value) == true).Select(a => a).ToList();
                        IList<XElement> summaryList = xDoc.Descendants("DAMsettings").Descendants(viewName).Descendants("AssetType").Descendants("Attributes").Descendants("Attribute").Where(a => Convert.ToBoolean(a.Element("IsColumn").Value) == true).Select(a => a).ToList();
                        IList<XElement> tempList = new List<XElement>(thumpnail.Concat(summaryList));
                        foreach (var rec in tempList)
                        {
                            IList<XElement> duplist = new List<XElement>();
                            duplist = result.Where(a => Convert.ToInt32(a.Element("Id").Value) == Convert.ToInt32(rec.Element("Id").Value)).Select(a => a).ToList();
                            if (duplist.Count == 0)
                                result.Add(rec);
                        }

                    }
                    else if (viewType == (int)AssetView.ListView)
                    {
                        tempresults = xDoc.Descendants("DAMsettings").Descendants(viewName).Descendants("AssetType").Descendants("Attributes").Descendants("Attribute").Where(a => Convert.ToBoolean(a.Element("IsColumn").Value) == true).Select(a => a).ToList();
                        foreach (var rec in tempresults)
                        {
                            IList<XElement> duplist = new List<XElement>();
                            duplist = result.Where(a => Convert.ToInt32(a.Element("Id").Value) == Convert.ToInt32(rec.Element("Id").Value)).Select(a => a).ToList();
                            if (duplist.Count == 0)
                                result.Add(rec);
                        }
                    }


                    int[] attrsidarr = result.Distinct().Select(a => Convert.ToInt32(a.Element("Id").Value)).Distinct().ToArray();

                    var typeidArr = assets.Cast<Hashtable>().Select(a => (int)a["AssetTypeid"]).Distinct().ToArray();

                    var idArr = assets.Cast<Hashtable>().Select(a => (int)a["AssetUniqueID"]).ToArray();

                    IList<AttributeDao> attributes = new List<AttributeDao>();
                    attributes = (from attrbs in tx.PersistenceManager.MetadataRepository.GetObject<AttributeDao>(xmlpath) where attrsidarr.Contains(attrbs.Id) select attrbs).ToList<AttributeDao>();

                    IList<EntityTypeAttributeRelationDao> entityAttributes = new List<EntityTypeAttributeRelationDao>();
                    entityAttributes = (from attrbs in tx.PersistenceManager.MetadataRepository.GetObject<EntityTypeAttributeRelationDao>(xmlpath) where typeidArr.Contains(attrbs.EntityTypeID) select attrbs).ToList<EntityTypeAttributeRelationDao>();




                    var attributerelationList = (from AdminAttributes in result
                                                 join ser in attributes on Convert.ToInt16(AdminAttributes.Element("Id").Value) equals ser.Id
                                                 select new
                                                 {
                                                     ID = Convert.ToInt16(AdminAttributes.Element("Id").Value),
                                                     SotOrder = Convert.ToInt32(AdminAttributes.Element("SortOrder").Value),
                                                     Type = ser.AttributeTypeID,
                                                     IsSpecial = ser.IsSpecial,
                                                     Field = ser.Id,
                                                     Level = Convert.ToInt16(AdminAttributes.Element("Level").Value),
                                                 }).Distinct().ToList();

                    int[] attrSelectType = { 1, 2, 3, 5 };

                    StringBuilder damQuery = new StringBuilder();
                    damQuery.AppendLine("(");
                    int EntitypeLenghth = typeidArr.Distinct().Count();
                    for (var i = 0; i < typeidArr.Distinct().Count(); i++)
                    {
                        //if (typeidArr[i] != 34 && typeidArr[i] != 33)
                        //{
                        damQuery.AppendLine("select ID");
                        foreach (var currentval in attributerelationList.Where(a => a.IsSpecial != true && attrSelectType.Contains(a.Type)).ToList())
                        {
                            int val = entityAttributes.ToList().Where(a => a.EntityTypeID == typeidArr[i] && a.AttributeID == currentval.ID).Count();
                            if (val != 0)
                                damQuery.AppendLine(" ,Attr_" + currentval.ID + " as 'Attr_" + currentval.ID + "'");
                            else
                                damQuery.AppendLine(" ,'-' as  'Attr_" + currentval.ID + "'");
                        }
                        damQuery.AppendLine(" from MM_AttributeRecord_" + typeidArr[i]);
                        if (i < EntitypeLenghth - 1)
                        {
                            damQuery.AppendLine(" union ");
                        }
                        //}
                    }
                    damQuery.AppendLine(") subtbl");

                    StringBuilder mainTblQry = new StringBuilder();
                    mainTblQry.AppendLine("select subtbl.ID  ");
                    int LastTreeLevel = attributerelationList.Where(a => (AttributesList)a.Type == AttributesList.TreeMultiSelection).OrderByDescending(a => a.Level).Select(a => a.Level).FirstOrDefault();
                    for (int j = 0; j < attributerelationList.Count(); j++)
                    {
                        string CurrentattrID = attributerelationList[j].ID.ToString();
                        if (attributerelationList[j].IsSpecial == true)
                        {
                            switch ((SystemDefinedAttributes)attributerelationList[j].ID)
                            {
                                case SystemDefinedAttributes.Name:
                                    mainTblQry.AppendLine(",pe.Name  as '" + attributerelationList[j].Field + "'");
                                    break;
                                case SystemDefinedAttributes.Owner:
                                    mainTblQry.Append(",ISNULL( (SELECT top 1  ISNULL(us.FirstName,'') + ' ' + ISNULL(us.LastName,'')  FROM UM_User us INNER JOIN AM_Entity_Role_User aeru ON us.ID=aeru.UserID AND aeru.EntityID=subtbl.Id  INNER JOIN AM_EntityTypeRoleAcl aetra ON  aeru.RoleID = aetra.ID AND  aetra.EntityTypeID=pe.TypeID AND aetra.EntityRoleID = 1),'-') as '" + attributerelationList[j].Field + "'");
                                    break;
                                case SystemDefinedAttributes.EntityStatus:
                                    mainTblQry.Append(", CASE WHEN pe.TypeID = " + (int)EntityTypeList.Objective + " THEN (SELECT case when ISNULL(po.ObjectiveStatus,0)=0 THEN 'Deactivated'  ELSE 'Active'  END from  PM_Objective po WHERE po.id=subtbl.Id) else isnull((SELECT  metso.StatusOptions FROM MM_EntityStatus mes INNER JOIN MM_EntityTypeStatus_Options metso ON mes.StatusID=metso.ID AND mes.EntityID=subtbl.id AND metso.IsRemoved=0),'-') end as '" + attributerelationList[j].Field + "'");
                                    break;
                                case SystemDefinedAttributes.EntityOnTimeStatus:
                                    mainTblQry.Append(", CASE WHEN pe.TypeID = " + (int)EntityTypeList.Objective + " THEN (SELECT case when ISNULL(po.ObjectiveStatus,0)=0 THEN '-'  ELSE '-'  END from  PM_Objective po WHERE po.id=subtbl.Id) else isnull((SELECT CASE WHEN (SELECT mes.IntimeStatus FROM MM_EntityStatus mes WHERE mes.EntityID=subtbl.id) = 0 THEN 'On time' WHEN (SELECT mes.IntimeStatus FROM MM_EntityStatus mes WHERE mes.EntityID=subtbl.id) = 1 THEN 'Delayed' WHEN (SELECT mes.IntimeStatus FROM MM_EntityStatus mes WHERE mes.EntityID=subtbl.id) = 2 THEN 'On hold' ELSE 'On time' END AS ontimestatus), '-') END AS '" + attributerelationList[j].Field + "'");
                                    break;
                                case SystemDefinedAttributes.MyRoleEntityAccess:

                                    mainTblQry.Append(", (select STUFF((SELECT',' +   ar.Caption ");
                                    mainTblQry.Append(" FROM AM_EntityTypeRoleAcl ar INNER JOIN AM_Entity_Role_User aeru ON ar.ID=aeru.RoleID  AND aeru.EntityID= pe.Id AND aeru.UserId= " + proxy.MarcomManager.User.Id + " ");
                                    mainTblQry.Append(" FOR XML PATH('')),1,1,'') AS x) AS '" + attributerelationList[j].Field + "'");
                                    break;
                                case SystemDefinedAttributes.MyRoleGlobalAccess:
                                    mainTblQry.Append(",(select STUFF((SELECT',' +   agr.Caption ");
                                    mainTblQry.Append(" FROM AM_GlobalRole agr  INNER JOIN AM_GlobalRole_User agru  ON agr.ID=agru.GlobalRoleId  AND agru.UserId= " + proxy.MarcomManager.User.Id + " ");
                                    mainTblQry.Append(" FOR XML PATH('')),1,1,'') AS x) AS '" + attributerelationList[j].Field + "'");
                                    break;
                            }
                        }
                        else if ((AttributesList)attributerelationList[j].Type == AttributesList.ListMultiSelection || (AttributesList)attributerelationList[j].Type == AttributesList.DropDownTree || (AttributesList)attributerelationList[j].Type == AttributesList.Tree || (AttributesList)attributerelationList[j].Type == AttributesList.Period || (AttributesList)attributerelationList[j].Type == AttributesList.TreeMultiSelection)
                        {
                            switch ((AttributesList)attributerelationList[j].Type)
                            {
                                case AttributesList.ListMultiSelection:

                                    if (attributerelationList[j].ID != (int)SystemDefinedAttributes.ObjectiveType)
                                    {

                                        mainTblQry.Append(" ,(SELECT  ");
                                        mainTblQry.Append(" STUFF( ");
                                        mainTblQry.Append(" ( ");
                                        mainTblQry.Append("  SELECT ', ' + mo.Caption ");
                                        mainTblQry.Append(" FROM MM_Option mo     ");
                                        mainTblQry.Append(" INNER JOIN MM_DAM_MultiSelectValue mms2  ");
                                        mainTblQry.Append("  ON  mo.ID = mms2.OptionID ");
                                        mainTblQry.Append(" AND mms2.AttributeID = " + attributerelationList[j].ID);
                                        mainTblQry.Append(" WHERE  mms2.AssetID = mms.AssetID FOR XML PATH('') ");
                                        mainTblQry.Append(" ), ");
                                        mainTblQry.Append("  1, ");
                                        mainTblQry.Append(" 2, ");
                                        mainTblQry.Append(" '' ");
                                        mainTblQry.Append("  )               AS VALUE ");
                                        mainTblQry.Append(" FROM   MM_DAM_MultiSelectValue     mms ");
                                        mainTblQry.Append(" WHERE  mms.AssetID=subtbl.Id and  mms.AttributeID = " + CurrentattrID + " ");
                                        mainTblQry.Append(" GROUP BY ");
                                        mainTblQry.Append("  mms.AssetID) as '" + attributerelationList[j].Field + "'");
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
                                    mainTblQry.Append("   AND mtn.Level = " + attributerelationList[j].Level + " ");
                                    mainTblQry.Append("  WHERE  mtv.EntityID = subtbl.Id ");
                                    mainTblQry.Append(" AND mtv.AttributeID = " + CurrentattrID + "   ");
                                    mainTblQry.Append(" ), ");
                                    mainTblQry.Append(" '' ");
                                    mainTblQry.Append(" ) ) as '" + attributerelationList[j].Field + "'");
                                    break;
                                case AttributesList.Tree:
                                    mainTblQry.Append(" ,'IsTree' as '" + attributerelationList[j].Field + "'");
                                    break;
                                case AttributesList.Period:
                                    mainTblQry.Append(" ,(SELECT (SELECT CONVERT(NVARCHAR(10), pep.StartDate, 120)  '@s', CONVERT(NVARCHAR(10), pep.EndDate, 120) '@e',");
                                    mainTblQry.Append(" pep.[Description] '@d', ROW_NUMBER() over(ORDER BY pep.Startdate) '@sid',");
                                    mainTblQry.Append(" pep.ID '@o'");
                                    mainTblQry.Append(" FROM   PM_EntityPeriod pep");
                                    mainTblQry.Append(" WHERE  pep.EntityID = subtbl.Id ORDER BY pep.Startdate FOR XML PATH('p'),");
                                    mainTblQry.Append(" TYPE");
                                    mainTblQry.Append(" ) FOR XML PATH('root')");
                                    mainTblQry.Append(" )  AS 'Period'");

                                    mainTblQry.Append(",(SELECT ISNULL(CAST(MIN(pep.Startdate) AS VARCHAR(10)) + '  ' + CAST(MAX(pep.EndDate)AS VARCHAR(10)),'-' )  ");
                                    mainTblQry.Append(" FROM PM_EntityPeriod pep WHERE pep.EntityID= subtbl.Id) AS TempPeriod ");

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
                                    if (LastTreeLevel == attributerelationList[j].Level)
                                    {
                                        mainTblQry.Append(" ,(SELECT  ");
                                        mainTblQry.Append(" STUFF( ");
                                        mainTblQry.Append(" ( ");
                                        mainTblQry.Append(" SELECT ', ' +  mtn.Caption ");
                                        mainTblQry.Append(" FROM   MM_TreeNode mtn ");
                                        mainTblQry.Append(" INNER JOIN MM_TreeValue mtv ");
                                        mainTblQry.Append(" ON  mtv.NodeID = mtn.ID and  mtv.AttributeID=" + attributerelationList[j].ID);
                                        mainTblQry.Append("  AND mtn.Level = " + attributerelationList[j].Level + " WHERE mtv.EntityID = subtbl.Id AND mtv.AttributeID = " + CurrentattrID + "  ");
                                        mainTblQry.Append(" FOR XML PATH('') ");
                                        mainTblQry.Append(" ), ");
                                        mainTblQry.Append("  1, ");
                                        mainTblQry.Append(" 2, ");
                                        mainTblQry.Append(" '' ");
                                        mainTblQry.Append(" ) ) as '" + attributerelationList[j].Field + "'");
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
                                        mainTblQry.Append("   AND mtn.Level = " + attributerelationList[j].Level + " ");
                                        mainTblQry.Append("  WHERE  mtv.EntityID = subtbl.Id ");
                                        mainTblQry.Append(" AND mtv.AttributeID = " + CurrentattrID + "   ");
                                        mainTblQry.Append(" ), ");
                                        mainTblQry.Append(" '' ");
                                        mainTblQry.Append(" ) ) as '" + attributerelationList[j].Field + "'");
                                    }
                                    break;
                            }
                        }
                        else if ((AttributesList)attributerelationList[j].Type == AttributesList.Link)
                        {
                            mainTblQry.Append(",(isnull( (SELECT top 1 URL FROM CM_Links  WHERE ModuleId = 5 AND  entityid=subtbl.ID),'') ) as '" + attributerelationList[j].Field + "'");
                        }
                        else if ((AttributesList)attributerelationList[j].Type == AttributesList.ListSingleSelection)
                        {
                            mainTblQry.Append(",(isnull( (SELECT top 1 caption FROM MM_Option  WHERE AttributeID=" + CurrentattrID + " AND id=subtbl.Attr_" + CurrentattrID + "),'') ) as '" + attributerelationList[j].Field + "'");
                        }
                        else if ((AttributesList)attributerelationList[j].Type == AttributesList.CheckBoxSelection)
                        {
                            mainTblQry.Append(" ,isnull(cast(subtbl.attr_" + CurrentattrID + " as varchar(50)), '') as '" + attributerelationList[j].Field + "'");
                        }
                        else if ((AttributesList)attributerelationList[j].Type == AttributesList.DateTime)
                        {
                            mainTblQry.Append(" ,REPLACE(CONVERT(varchar,isnull(subtbl.attr_" + CurrentattrID + " ,''),121),'1900-01-01 00:00:00.000', '') as '" + attributerelationList[j].Field + "'");
                        }
                        else if ((AttributesList)attributerelationList[j].Type == AttributesList.ParentEntityName)
                        {
                            mainTblQry.Append(" ,isnull((SELECT top 1 pe2.name  + '!@#' + met.ShortDescription + '!@#' + met.ColorCode FROM PM_Entity pe2 INNER JOIN MM_EntityType met ON pe2.TypeID=met.ID  WHERE  pe2.id=pe.parentid), '') as '" + attributerelationList[j].Field + "'");
                        }
                        else
                        {
                            mainTblQry.Append(" ,isnull(subtbl.attr_" + CurrentattrID + " , '') as '" + attributerelationList[j].Field + "'");
                        }

                    }
                    mainTblQry.AppendLine(" from DAM_Asset pe inner join  ");
                    mainTblQry.AppendLine(damQuery.ToString());
                    mainTblQry.AppendLine("  on subtbl.id=pe.Id  where ");
                    if (ViewAll == false)
                        mainTblQry.AppendLine(" pe.folderid=:folderid and ");
                    mainTblQry.AppendLine(" pe.entityid=:entityID");

                    IList dynamicData = tx.PersistenceManager.DamRepository.ExecuteQuerywithParam(mainTblQry.ToString(), assetQuery_parLIST);

                    returnObj.Add(new
                    {
                        AssetFiles = assets,
                        AssetTypeAttrRel = attributerelationList,
                        AssetDynData = dynamicData
                    });


                    return returnObj;
                }
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public List<object> GetStatusFilteredEntityAsset(DigitalAssetManagerProxy proxy, int folderid, int entityID, int viewType, int orderbyid, int pageNo, string statusFilter)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    List<object> returnObj = new List<object>();
                    string viewName = Enum.GetName(typeof(AssetView), viewType);
                    int totalrecords = 20;
                    if (viewType == (int)AssetView.ListView)
                        totalrecords = 30;
                    if (viewType == (int)AssetView.SummaryView)
                        totalrecords = 10;
                    IList assets = null;
                    //List<object> assets = new List<object>();
                    IList approvedassetList2 = null;
                    IList approvedassetList3 = null;
                    IList approvedassetList = null;
                    IList<MultiProperty> assetQuery_parLIST = new List<MultiProperty>();
                    string[] statusFilterselection = statusFilter.Split(',');
                    foreach (var filteroption in statusFilterselection)
                    {

                        switch (Convert.ToInt32(filteroption))
                        {
                            case 1:
                                //Prepared
                                Console.WriteLine("Case 1");
                                break;
                            case 2:
                                //Approved
                                StringBuilder assetQueryfrApprovedAssetIDs = new StringBuilder();
                                assetQuery_parLIST.Add(new MultiProperty { propertyName = "folderid", propertyValue = folderid });
                                assetQuery_parLIST.Add(new MultiProperty { propertyName = "entityID", propertyValue = entityID });

                                assetQueryfrApprovedAssetIDs.AppendLine("CREATE TABLE #LocalTempTable(");
                                assetQueryfrApprovedAssetIDs.AppendLine("[AssetID] int,");
                                assetQueryfrApprovedAssetIDs.AppendLine("[Count] int, ");
                                assetQueryfrApprovedAssetIDs.AppendLine("[ID] int) ");

                                assetQueryfrApprovedAssetIDs.AppendLine("INSERT INTO #LocalTempTable(AssetID, [Count], [ID])");

                                assetQueryfrApprovedAssetIDs.AppendLine("SELECT te.AssetId,da.ID,COUNT(te.ID) FROM DAM_Asset da ");
                                assetQueryfrApprovedAssetIDs.AppendLine("INNER JOIN TM_EntityTask te ON te.AssetId = da.ID ");
                                assetQueryfrApprovedAssetIDs.AppendLine("WHERE te.TaskStatus = 3  AND  te.AssetId IN (SELECT ID FROM DAM_Asset WHERE EntityID = " + entityID + " ) ");
                                assetQueryfrApprovedAssetIDs.AppendLine("GROUP BY te.AssetId, da.ID ");
                                assetQueryfrApprovedAssetIDs.AppendLine("HAVING (COUNT(te.ID) = (select COUNT(*) from TM_EntityTask te1 WHERE te1.AssetId = da.ID))");
                                assetQueryfrApprovedAssetIDs.AppendLine(" DECLARE @RowsPerPage INT = " + totalrecords + ", ");
                                assetQueryfrApprovedAssetIDs.AppendLine(" @PageNumber INT =" + pageNo + " ");
                                assetQueryfrApprovedAssetIDs.AppendLine(" DECLARE @OrderBy INT = " + orderbyid + "; ");
                                assetQueryfrApprovedAssetIDs.AppendLine(" SELECT a.ID AS 'FileUniqueID', ");
                                assetQueryfrApprovedAssetIDs.AppendLine(" a.Name as 'FileName', ");
                                assetQueryfrApprovedAssetIDs.AppendLine(" a.Extension, ");
                                assetQueryfrApprovedAssetIDs.AppendLine(" a.[Size],  a.[VersionNo], ");
                                assetQueryfrApprovedAssetIDs.AppendLine(" a.OwnerID, ");
                                assetQueryfrApprovedAssetIDs.AppendLine(" a.CreatedOn, ");
                                assetQueryfrApprovedAssetIDs.AppendLine(" a.FileGuid, ");
                                assetQueryfrApprovedAssetIDs.AppendLine(" a.[Description], ");
                                assetQueryfrApprovedAssetIDs.AppendLine(" a.AssetID, ");
                                assetQueryfrApprovedAssetIDs.AppendLine(" da.FolderID, ");
                                assetQueryfrApprovedAssetIDs.AppendLine(" da.EntityID, ");
                                assetQueryfrApprovedAssetIDs.AppendLine(" da.ID AS 'AssetUniqueID', ");
                                assetQueryfrApprovedAssetIDs.AppendLine(" da.Name as 'AssetName', da.Url as 'LinkURL', a.MimeType as 'MimeType', ");
                                assetQueryfrApprovedAssetIDs.AppendLine(" da.AssetTypeid, ");
                                assetQueryfrApprovedAssetIDs.AppendLine(" da.ActiveFileID, ");
                                assetQueryfrApprovedAssetIDs.AppendLine(" met.ColorCode, ");
                                assetQueryfrApprovedAssetIDs.AppendLine(" met.ShortDescription, ");
                                assetQueryfrApprovedAssetIDs.AppendLine(" a.[Status], ");
                                assetQueryfrApprovedAssetIDs.AppendLine(" da.Category, da.IsPublish ");
                                assetQueryfrApprovedAssetIDs.AppendLine(" FROM   DAM_File a ");
                                assetQueryfrApprovedAssetIDs.AppendLine(" RIGHT OUTER JOIN DAM_Asset da ");
                                assetQueryfrApprovedAssetIDs.AppendLine(" ON  a.AssetID = da.ID AND a.ID=da.ActiveFileID");
                                assetQueryfrApprovedAssetIDs.AppendLine(" INNER JOIN MM_EntityType met ");
                                assetQueryfrApprovedAssetIDs.AppendLine(" ON  met.ID = da.AssetTypeid ");
                                assetQueryfrApprovedAssetIDs.AppendLine(" WHERE  da.id IN (SELECT da2.id ");
                                assetQueryfrApprovedAssetIDs.AppendLine(" FROM   DAM_Asset da2 ");
                                assetQueryfrApprovedAssetIDs.AppendLine(" WHERE  da2.FolderID = :folderid ");
                                assetQueryfrApprovedAssetIDs.AppendLine(" AND da2.entityid = :entityID ");
                                assetQueryfrApprovedAssetIDs.AppendLine("AND da2.ID IN (select AssetID FROM #LocalTempTable)");
                                assetQueryfrApprovedAssetIDs.AppendLine("  ORDER BY  ");
                                assetQueryfrApprovedAssetIDs.AppendLine(" CASE   ");
                                assetQueryfrApprovedAssetIDs.AppendLine(" WHEN @OrderBy = 1  ");
                                assetQueryfrApprovedAssetIDs.AppendLine(" THEN da.Name  ");
                                assetQueryfrApprovedAssetIDs.AppendLine(" END       asc,  ");
                                assetQueryfrApprovedAssetIDs.AppendLine(" CASE   ");
                                assetQueryfrApprovedAssetIDs.AppendLine(" WHEN @OrderBy = 2  ");
                                assetQueryfrApprovedAssetIDs.AppendLine(" THEN da.Name  ");
                                assetQueryfrApprovedAssetIDs.AppendLine(" END desc,  ");
                                assetQueryfrApprovedAssetIDs.AppendLine(" CASE   ");
                                assetQueryfrApprovedAssetIDs.AppendLine(" WHEN @OrderBy = 3  ");
                                assetQueryfrApprovedAssetIDs.AppendLine(" THEN da.Createdon   ");
                                assetQueryfrApprovedAssetIDs.AppendLine(" END       asc , ");
                                assetQueryfrApprovedAssetIDs.AppendLine(" CASE   ");
                                assetQueryfrApprovedAssetIDs.AppendLine(" WHEN @OrderBy = 4 ");
                                assetQueryfrApprovedAssetIDs.AppendLine(" THEN da.Createdon   ");
                                assetQueryfrApprovedAssetIDs.AppendLine(" END       desc  ");
                                assetQueryfrApprovedAssetIDs.AppendLine(" OFFSET(@PageNumber - 1) * @RowsPerPage ROWS ");
                                assetQueryfrApprovedAssetIDs.AppendLine(" FETCH NEXT @RowsPerPage ROWS ONLY )");

                                assetQueryfrApprovedAssetIDs.AppendLine(" DROP TABLE #LocalTempTable ");

                                approvedassetList2 = tx.PersistenceManager.DamRepository.ExecuteQuerywithParam(assetQueryfrApprovedAssetIDs.ToString(), assetQuery_parLIST);
                                break;
                            case 3:
                                //Published
                                StringBuilder assetQuery = new StringBuilder();
                                assetQuery_parLIST.Add(new MultiProperty { propertyName = "folderid", propertyValue = folderid });
                                assetQuery_parLIST.Add(new MultiProperty { propertyName = "entityID", propertyValue = entityID });

                                assetQuery.AppendLine(" DECLARE @RowsPerPage INT = " + totalrecords + ", ");
                                assetQuery.AppendLine(" @PageNumber INT =" + pageNo + " ");
                                assetQuery.AppendLine(" DECLARE @OrderBy INT = " + orderbyid + "; ");
                                assetQuery.AppendLine(" SELECT a.ID AS 'FileUniqueID', ");
                                assetQuery.AppendLine(" a.Name as 'FileName', ");
                                assetQuery.AppendLine(" a.Extension, ");
                                assetQuery.AppendLine(" a.[Size],  a.[VersionNo], ");
                                assetQuery.AppendLine(" a.OwnerID, ");
                                assetQuery.AppendLine(" a.CreatedOn, ");
                                assetQuery.AppendLine(" a.FileGuid, ");
                                assetQuery.AppendLine(" a.[Description], ");
                                assetQuery.AppendLine(" a.AssetID, ");
                                assetQuery.AppendLine(" da.FolderID, ");
                                assetQuery.AppendLine(" da.EntityID, ");
                                assetQuery.AppendLine(" da.ID AS 'AssetUniqueID', ");
                                assetQuery.AppendLine(" da.Name as 'AssetName', da.Url as 'LinkURL', a.MimeType as 'MimeType', ");
                                assetQuery.AppendLine(" da.AssetTypeid, ");
                                assetQuery.AppendLine(" da.ActiveFileID, ");
                                assetQuery.AppendLine(" met.ColorCode, ");
                                assetQuery.AppendLine(" met.ShortDescription, ");
                                assetQuery.AppendLine(" a.[Status], ");
                                assetQuery.AppendLine(" da.Category, da.IsPublish ");
                                assetQuery.AppendLine(" FROM   DAM_File a ");
                                assetQuery.AppendLine(" RIGHT OUTER JOIN DAM_Asset da ");
                                assetQuery.AppendLine(" ON  a.AssetID = da.ID AND a.ID=da.ActiveFileID");
                                assetQuery.AppendLine(" INNER JOIN MM_EntityType met ");
                                assetQuery.AppendLine(" ON  met.ID = da.AssetTypeid ");
                                assetQuery.AppendLine(" WHERE  da.id IN (SELECT da2.id ");
                                assetQuery.AppendLine(" FROM   DAM_Asset da2 ");
                                assetQuery.AppendLine(" WHERE  da2.FolderID = :folderid ");
                                assetQuery.AppendLine(" AND da2.entityid = :entityID ");
                                assetQuery.AppendLine(" AND da2.IsPublish = 1)");
                                assetQuery.AppendLine("  ORDER BY  ");
                                assetQuery.AppendLine(" CASE   ");
                                assetQuery.AppendLine(" WHEN @OrderBy = 1  ");
                                assetQuery.AppendLine(" THEN da.Name  ");
                                assetQuery.AppendLine(" END       asc,  ");
                                assetQuery.AppendLine(" CASE   ");
                                assetQuery.AppendLine(" WHEN @OrderBy = 2  ");
                                assetQuery.AppendLine(" THEN da.Name  ");
                                assetQuery.AppendLine(" END desc,  ");
                                assetQuery.AppendLine(" CASE   ");
                                assetQuery.AppendLine(" WHEN @OrderBy = 3  ");
                                assetQuery.AppendLine(" THEN da.Createdon   ");
                                assetQuery.AppendLine(" END       asc , ");
                                assetQuery.AppendLine(" CASE   ");
                                assetQuery.AppendLine(" WHEN @OrderBy = 4 ");
                                assetQuery.AppendLine(" THEN da.Createdon   ");
                                assetQuery.AppendLine(" END       desc  ");
                                assetQuery.AppendLine(" OFFSET(@PageNumber - 1) * @RowsPerPage ROWS ");
                                assetQuery.AppendLine(" FETCH NEXT @RowsPerPage ROWS ONLY ");
                                assets = tx.PersistenceManager.DamRepository.ExecuteQuerywithParam(assetQuery.ToString(), assetQuery_parLIST);
                                break;
                            default:
                                // Console.WriteLine("Default case");
                                break;
                        }
                    }





                    int version = MarcomManagerFactory.ActiveMetadataVersionNumber;
                    string xmlpath = string.Empty;

                    xmlpath = tx.PersistenceManager.MetadataRepository.GetXmlPath(version);
                    var xmetadataDoc = XDocument.Load(xmlpath);

                    string Adminxmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
                    XDocument xDoc = XDocument.Load(Adminxmlpath);

                    IList<XElement> tempresults = new List<XElement>();
                    IList<XElement> result = new List<XElement>();

                    if (viewType == (int)AssetView.ThumbnailView)
                    {
                        tempresults = xDoc.Descendants("DAMsettings").Descendants(viewName).Descendants("AssetType").Descendants("Attributes").Descendants("Attribute").Where(a => Convert.ToBoolean(a.Element("IsColumn").Value) == true).Select(a => a).ToList();
                        foreach (var rec in tempresults)
                        {
                            IList<XElement> duplist = new List<XElement>();
                            duplist = result.Where(a => Convert.ToInt32(a.Element("Id").Value) == Convert.ToInt32(rec.Element("Id").Value)).Select(a => a).ToList();
                            if (duplist.Count == 0)
                                result.Add(rec);
                        }
                    }
                    else if (viewType == (int)AssetView.SummaryView)
                    {
                        IList<XElement> thumpnail = xDoc.Descendants("DAMsettings").Descendants("ThumbnailView").Descendants("AssetType").Descendants("Attributes").Descendants("Attribute").Where(a => Convert.ToBoolean(a.Element("IsColumn").Value) == true).Select(a => a).ToList();
                        IList<XElement> summaryList = xDoc.Descendants("DAMsettings").Descendants(viewName).Descendants("AssetType").Descendants("Attributes").Descendants("Attribute").Where(a => Convert.ToBoolean(a.Element("IsColumn").Value) == true).Select(a => a).ToList();
                        IList<XElement> tempList = new List<XElement>(thumpnail.Concat(summaryList));
                        foreach (var rec in tempList)
                        {
                            IList<XElement> duplist = new List<XElement>();
                            duplist = result.Where(a => Convert.ToInt32(a.Element("Id").Value) == Convert.ToInt32(rec.Element("Id").Value)).Select(a => a).ToList();
                            if (duplist.Count == 0)
                                result.Add(rec);
                        }

                    }
                    else if (viewType == (int)AssetView.ListView)
                    {
                        tempresults = xDoc.Descendants("DAMsettings").Descendants(viewName).Descendants("AssetType").Descendants("Attributes").Descendants("Attribute").Where(a => Convert.ToBoolean(a.Element("IsColumn").Value) == true).Select(a => a).ToList();
                        foreach (var rec in tempresults)
                        {
                            IList<XElement> duplist = new List<XElement>();
                            duplist = result.Where(a => Convert.ToInt32(a.Element("Id").Value) == Convert.ToInt32(rec.Element("Id").Value)).Select(a => a).ToList();
                            if (duplist.Count == 0)
                                result.Add(rec);
                        }
                    }

                    int[] attrsidarr = result.Distinct().Select(a => Convert.ToInt32(a.Element("Id").Value)).ToArray();

                    var typeidArr = assets.Cast<Hashtable>().Select(a => (int)a["AssetTypeid"]).Distinct().ToArray();

                    var idArr = assets.Cast<Hashtable>().Select(a => (int)a["AssetUniqueID"]).ToArray();

                    IList<AttributeDao> attributes = new List<AttributeDao>();
                    attributes = (from attrbs in tx.PersistenceManager.MetadataRepository.Query<AttributeDao>() where attrsidarr.Contains(attrbs.Id) select attrbs).ToList<AttributeDao>();

                    IList<EntityTypeAttributeRelationDao> entityAttributes = new List<EntityTypeAttributeRelationDao>();
                    entityAttributes = (from attrbs in tx.PersistenceManager.MetadataRepository.Query<EntityTypeAttributeRelationDao>() where typeidArr.Contains(attrbs.EntityTypeID) select attrbs).ToList<EntityTypeAttributeRelationDao>();




                    var attributerelationList = (from AdminAttributes in result
                                                 join ser in attributes on Convert.ToInt16(AdminAttributes.Element("Id").Value) equals ser.Id
                                                 select new
                                                 {
                                                     ID = Convert.ToInt16(AdminAttributes.Element("Id").Value),
                                                     SotOrder = Convert.ToInt32(AdminAttributes.Element("SortOrder").Value),
                                                     Type = ser.AttributeTypeID,
                                                     IsSpecial = ser.IsSpecial,
                                                     Field = ser.Id,
                                                     Level = Convert.ToInt16(AdminAttributes.Element("Level").Value),
                                                 }).Distinct().ToList();

                    int[] attrSelectType = { 1, 2, 3, 5 };

                    StringBuilder damQuery = new StringBuilder();
                    damQuery.AppendLine("(");
                    int EntitypeLenghth = typeidArr.Distinct().Count();
                    for (var i = 0; i < typeidArr.Distinct().Count(); i++)
                    {

                        damQuery.AppendLine("select ID");
                        foreach (var currentval in attributerelationList.Where(a => a.IsSpecial != true && attrSelectType.Contains(a.Type)).ToList())
                        {
                            int val = entityAttributes.ToList().Where(a => a.EntityTypeID == typeidArr[i] && a.AttributeID == currentval.ID).Count();
                            if (val != 0)
                                damQuery.AppendLine(" ,Attr_" + currentval.ID + " as 'Attr_" + currentval.ID + "'");
                            else
                                damQuery.AppendLine(" ,'-' as  'Attr_" + currentval.ID + "'");
                        }
                        damQuery.AppendLine(" from MM_AttributeRecord_" + typeidArr[i]);
                        if (i < EntitypeLenghth - 1)
                        {
                            damQuery.AppendLine(" union ");
                        }
                    }
                    damQuery.AppendLine(") subtbl");

                    StringBuilder mainTblQry = new StringBuilder();
                    mainTblQry.AppendLine("select subtbl.ID  ");
                    int LastTreeLevel = attributerelationList.Where(a => (AttributesList)a.Type == AttributesList.TreeMultiSelection).OrderByDescending(a => a.Level).Select(a => a.Level).FirstOrDefault();
                    for (int j = 0; j < attributerelationList.Count(); j++)
                    {
                        string CurrentattrID = attributerelationList[j].ID.ToString();
                        if (attributerelationList[j].IsSpecial == true)
                        {
                            switch ((SystemDefinedAttributes)attributerelationList[j].ID)
                            {
                                case SystemDefinedAttributes.Name:
                                    mainTblQry.AppendLine(",pe.Name  as '" + attributerelationList[j].Field + "'");
                                    break;
                                case SystemDefinedAttributes.Owner:
                                    mainTblQry.Append(",ISNULL( (SELECT top 1  ISNULL(us.FirstName,'') + ' ' + ISNULL(us.LastName,'')  FROM UM_User us INNER JOIN AM_Entity_Role_User aeru ON us.ID=aeru.UserID AND aeru.EntityID=subtbl.Id  INNER JOIN AM_EntityTypeRoleAcl aetra ON  aeru.RoleID = aetra.ID AND  aetra.EntityTypeID=pe.TypeID AND aetra.EntityRoleID = 1),'-') as '" + attributerelationList[j].Field + "'");
                                    break;
                                case SystemDefinedAttributes.EntityStatus:
                                    mainTblQry.Append(", CASE WHEN pe.TypeID = " + (int)EntityTypeList.Objective + " THEN (SELECT case when ISNULL(po.ObjectiveStatus,0)=0 THEN 'Deactivated'  ELSE 'Active'  END from  PM_Objective po WHERE po.id=subtbl.Id) else isnull((SELECT  metso.StatusOptions FROM MM_EntityStatus mes INNER JOIN MM_EntityTypeStatus_Options metso ON mes.StatusID=metso.ID AND mes.EntityID=subtbl.id AND metso.IsRemoved=0),'-') end as '" + attributerelationList[j].Field + "'");
                                    break;
                                case SystemDefinedAttributes.EntityOnTimeStatus:
                                    mainTblQry.Append(", CASE WHEN pe.TypeID = " + (int)EntityTypeList.Objective + " THEN (SELECT case when ISNULL(po.ObjectiveStatus,0)=0 THEN '-'  ELSE '-'  END from  PM_Objective po WHERE po.id=subtbl.Id) else isnull((SELECT CASE WHEN (SELECT mes.IntimeStatus FROM MM_EntityStatus mes WHERE mes.EntityID=subtbl.id) = 0 THEN 'On time' WHEN (SELECT mes.IntimeStatus FROM MM_EntityStatus mes WHERE mes.EntityID=subtbl.id) = 1 THEN 'Delayed' WHEN (SELECT mes.IntimeStatus FROM MM_EntityStatus mes WHERE mes.EntityID=subtbl.id) = 2 THEN 'On hold' ELSE 'On time' END AS ontimestatus), '-') END AS '" + attributerelationList[j].Field + "'");
                                    break;
                                case SystemDefinedAttributes.MyRoleEntityAccess:

                                    mainTblQry.Append(", (select STUFF((SELECT',' +   ar.Caption ");
                                    mainTblQry.Append(" FROM AM_EntityTypeRoleAcl ar INNER JOIN AM_Entity_Role_User aeru ON ar.ID=aeru.RoleID  AND aeru.EntityID= pe.Id AND aeru.UserId= " + proxy.MarcomManager.User.Id + " ");
                                    mainTblQry.Append(" FOR XML PATH('')),1,1,'') AS x) AS '" + attributerelationList[j].Field + "'");
                                    break;
                                case SystemDefinedAttributes.MyRoleGlobalAccess:
                                    mainTblQry.Append(",(select STUFF((SELECT',' +   agr.Caption ");
                                    mainTblQry.Append(" FROM AM_GlobalRole agr  INNER JOIN AM_GlobalRole_User agru  ON agr.ID=agru.GlobalRoleId  AND agru.UserId= " + proxy.MarcomManager.User.Id + " ");
                                    mainTblQry.Append(" FOR XML PATH('')),1,1,'') AS x) AS '" + attributerelationList[j].Field + "'");
                                    break;
                            }
                        }
                        else if ((AttributesList)attributerelationList[j].Type == AttributesList.ListMultiSelection || (AttributesList)attributerelationList[j].Type == AttributesList.DropDownTree || (AttributesList)attributerelationList[j].Type == AttributesList.Tree || (AttributesList)attributerelationList[j].Type == AttributesList.Period || (AttributesList)attributerelationList[j].Type == AttributesList.TreeMultiSelection)
                        {
                            switch ((AttributesList)attributerelationList[j].Type)
                            {
                                case AttributesList.ListMultiSelection:

                                    if (attributerelationList[j].ID != (int)SystemDefinedAttributes.ObjectiveType)
                                    {

                                        mainTblQry.Append(" ,(SELECT  ");
                                        mainTblQry.Append(" STUFF( ");
                                        mainTblQry.Append(" ( ");
                                        mainTblQry.Append(" SELECT ', ' +  mo.Caption ");
                                        mainTblQry.Append(" FROM   MM_DAM_MultiSelectValue mms2 ");
                                        mainTblQry.Append(" INNER JOIN MM_Option mo ");
                                        mainTblQry.Append(" ON  mms2.OptionID = mo.ID and  mms2.AttributeID=" + attributerelationList[j].ID);
                                        mainTblQry.Append("  WHERE  mms2.AssetID = mms.AssetID ");
                                        mainTblQry.Append(" FOR XML PATH('') ");
                                        mainTblQry.Append(" ), ");
                                        mainTblQry.Append("  1, ");
                                        mainTblQry.Append(" 2, ");
                                        mainTblQry.Append(" '' ");
                                        mainTblQry.Append("  )               AS VALUE ");
                                        mainTblQry.Append(" FROM   MM_DAM_MultiSelectValue     mms ");
                                        mainTblQry.Append(" WHERE  mms.AssetID=subtbl.Id and  mms.AttributeID = " + CurrentattrID + " ");
                                        mainTblQry.Append(" GROUP BY ");
                                        mainTblQry.Append("  mms.AssetID) as '" + attributerelationList[j].Field + "'");
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
                                    mainTblQry.Append("   AND mtn.Level = " + attributerelationList[j].Level + " ");
                                    mainTblQry.Append("  WHERE  mtv.EntityID = subtbl.Id ");
                                    mainTblQry.Append(" AND mtv.AttributeID = " + CurrentattrID + "   ");
                                    mainTblQry.Append(" ), ");
                                    mainTblQry.Append(" '-' ");
                                    mainTblQry.Append(" ) ) as '" + attributerelationList[j].Field + "'");
                                    break;
                                case AttributesList.Tree:
                                    mainTblQry.Append(" ,'IsTree' as '" + attributerelationList[j].Field + "'");
                                    break;
                                case AttributesList.Period:
                                    mainTblQry.Append(" ,(SELECT (SELECT CONVERT(NVARCHAR(10), pep.StartDate, 120)  '@s', CONVERT(NVARCHAR(10), pep.EndDate, 120) '@e',");
                                    mainTblQry.Append(" pep.[Description] '@d', ROW_NUMBER() over(ORDER BY pep.Startdate) '@sid',");
                                    mainTblQry.Append(" pep.ID '@o'");
                                    mainTblQry.Append(" FROM   PM_EntityPeriod pep");
                                    mainTblQry.Append(" WHERE  pep.EntityID = subtbl.Id ORDER BY pep.Startdate FOR XML PATH('p'),");
                                    mainTblQry.Append(" TYPE");
                                    mainTblQry.Append(" ) FOR XML PATH('root')");
                                    mainTblQry.Append(" )  AS 'Period'");

                                    mainTblQry.Append(",(SELECT ISNULL(CAST(MIN(pep.Startdate) AS VARCHAR(10)) + '  ' + CAST(MAX(pep.EndDate)AS VARCHAR(10)),'-' )  ");
                                    mainTblQry.Append(" FROM PM_EntityPeriod pep WHERE pep.EntityID= subtbl.Id) AS TempPeriod ");

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
                                    if (LastTreeLevel == attributerelationList[j].Level)
                                    {
                                        mainTblQry.Append(" ,(SELECT  ");
                                        mainTblQry.Append(" STUFF( ");
                                        mainTblQry.Append(" ( ");
                                        mainTblQry.Append(" SELECT ', ' +  mtn.Caption ");
                                        mainTblQry.Append(" FROM   MM_TreeNode mtn ");
                                        mainTblQry.Append(" INNER JOIN MM_TreeValue mtv ");
                                        mainTblQry.Append(" ON  mtv.NodeID = mtn.ID and  mtv.AttributeID=" + attributerelationList[j].ID);
                                        mainTblQry.Append("  AND mtn.Level = " + attributerelationList[j].Level + " WHERE mtv.EntityID = subtbl.Id AND mtv.AttributeID = " + CurrentattrID + "  ");
                                        mainTblQry.Append(" FOR XML PATH('') ");
                                        mainTblQry.Append(" ), ");
                                        mainTblQry.Append("  1, ");
                                        mainTblQry.Append(" 2, ");
                                        mainTblQry.Append(" '' ");
                                        mainTblQry.Append(" ) ) as '" + attributerelationList[j].Field + "'");
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
                                        mainTblQry.Append("   AND mtn.Level = " + attributerelationList[j].Level + " ");
                                        mainTblQry.Append("  WHERE  mtv.EntityID = subtbl.Id ");
                                        mainTblQry.Append(" AND mtv.AttributeID = " + CurrentattrID + "   ");
                                        mainTblQry.Append(" ), ");
                                        mainTblQry.Append(" '-' ");
                                        mainTblQry.Append(" ) ) as '" + attributerelationList[j].Field + "'");
                                    }
                                    break;
                            }
                        }
                        else if ((AttributesList)attributerelationList[j].Type == AttributesList.Link)
                        {
                            mainTblQry.Append(",(isnull( (SELECT top 1 URL FROM CM_Links  WHERE ModuleId = 5 AND  entityid=subtbl.ID),'-') ) as '" + attributerelationList[j].Field + "'");
                        }
                        else if ((AttributesList)attributerelationList[j].Type == AttributesList.ListSingleSelection)
                        {
                            mainTblQry.Append(",(isnull( (SELECT top 1 caption FROM MM_Option  WHERE AttributeID=" + CurrentattrID + " AND id=subtbl.Attr_" + CurrentattrID + "),'-') ) as '" + attributerelationList[j].Field + "'");
                        }
                        else if ((AttributesList)attributerelationList[j].Type == AttributesList.CheckBoxSelection)
                        {
                            mainTblQry.Append(" ,isnull(cast(subtbl.attr_" + CurrentattrID + " as varchar(50)), '-') as '" + attributerelationList[j].Field + "'");
                        }
                        else if ((AttributesList)attributerelationList[j].Type == AttributesList.DateTime)
                        {
                            mainTblQry.Append(" ,REPLACE(CONVERT(varchar,isnull(subtbl.attr_" + CurrentattrID + " ,''),121),'1900-01-01 00:00:00.000', '-') as '" + attributerelationList[j].Field + "'");
                        }
                        else if ((AttributesList)attributerelationList[j].Type == AttributesList.ParentEntityName)
                        {
                            mainTblQry.Append(" ,isnull((SELECT top 1 pe2.name  + '!@#' + met.ShortDescription + '!@#' + met.ColorCode FROM PM_Entity pe2 INNER JOIN MM_EntityType met ON pe2.TypeID=met.ID  WHERE  pe2.id=pe.parentid), '-') as '" + attributerelationList[j].Field + "'");
                        }
                        else
                        {
                            mainTblQry.Append(" ,isnull(subtbl.attr_" + CurrentattrID + " , '-') as '" + attributerelationList[j].Field + "'");
                        }

                    }
                    mainTblQry.AppendLine(" from DAM_Asset pe inner join  ");
                    mainTblQry.AppendLine(damQuery.ToString());
                    mainTblQry.AppendLine("  on subtbl.id=pe.Id  where pe.folderid=:folderid and pe.entityid=:entityID");
                    IList dynamicData = tx.PersistenceManager.DamRepository.ExecuteQuerywithParam(mainTblQry.ToString(), assetQuery_parLIST);

                    returnObj.Add(new
                    {
                        AssetFiles = assets,
                        AssetTypeAttrRel = attributerelationList,
                        AssetDynData = dynamicData
                    });

                    return returnObj;
                }
            }
            catch (Exception)
            {

                throw;
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

                    var assetName = GetAssetName(tx, assetId, 1);
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

        public List<object> GetDAMViewSettings(DigitalAssetManagerProxy proxy)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    List<object> returnObj = new List<object>();
                    string Adminxmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
                    XDocument xDoc = XDocument.Load(Adminxmlpath);


                    var thumpnail = (from AdminAttributes in xDoc.Descendants("DAMsettings").Descendants("ThumbnailView").Descendants("AssetType").Descendants("Attributes").Descendants("Attribute")
                                     join ser in tx.PersistenceManager.MetadataRepository.Query<AttributeDao>() on Convert.ToInt16(AdminAttributes.Element("Id").Value) equals ser.Id
                                     where Convert.ToBoolean(AdminAttributes.Element("IsColumn").Value) == true
                                     select new
                                     {
                                         ID = Convert.ToInt16(AdminAttributes.Element("Id").Value),
                                         Caption = ser.Caption,
                                         SotOrder = Convert.ToInt32(AdminAttributes.Element("SortOrder").Value),
                                         Type = ser.AttributeTypeID,
                                         assetType = Convert.ToInt16(AdminAttributes.Parent.Parent.Attributes("ID").ToList().FirstOrDefault().Value.ToString()),
                                         Field = ser.Id,
                                     }).Distinct().ToList();

                    var listview = (from AdminAttributes in xDoc.Descendants("DAMsettings").Descendants("ListView").Descendants("AssetType").Descendants("Attributes").Descendants("Attribute")
                                    join ser in tx.PersistenceManager.MetadataRepository.Query<AttributeDao>() on Convert.ToInt16(AdminAttributes.Element("Id").Value) equals ser.Id
                                    where Convert.ToBoolean(AdminAttributes.Element("IsColumn").Value) == true
                                    select new
                                    {
                                        ID = Convert.ToInt16(AdminAttributes.Element("Id").Value),
                                        Caption = ser.Caption,
                                        SotOrder = Convert.ToInt32(AdminAttributes.Element("SortOrder").Value),
                                        Type = ser.AttributeTypeID,
                                        assetType = Convert.ToInt16(AdminAttributes.Parent.Parent.Attributes("ID").ToList().FirstOrDefault().Value.ToString()),
                                        Field = ser.Id,
                                    }).Distinct().ToList().GroupBy(a => a.ID).Select(b => b.First());

                    var summaryList = (from AdminAttributes in xDoc.Descendants("DAMsettings").Descendants("SummaryView").Descendants("AssetType").Descendants("Attributes").Descendants("Attribute")
                                       join ser in tx.PersistenceManager.MetadataRepository.Query<AttributeDao>() on Convert.ToInt16(AdminAttributes.Element("Id").Value) equals ser.Id
                                       where Convert.ToBoolean(AdminAttributes.Element("IsColumn").Value) == true
                                       select new
                                       {
                                           ID = Convert.ToInt16(AdminAttributes.Element("Id").Value),
                                           Caption = ser.Caption,
                                           SotOrder = Convert.ToInt32(AdminAttributes.Element("SortOrder").Value),
                                           attrType = ser.AttributeTypeID,
                                           assetType = Convert.ToInt16(AdminAttributes.Parent.Parent.Attributes("ID").ToList().FirstOrDefault().Value.ToString()),
                                           Field = ser.Id,
                                       }).Distinct().ToList();


                    returnObj.Add(new
                    {
                        ThumbnailSettings = thumpnail,
                        SummaryViewSettings = summaryList,
                        ListViewSettings = listview
                    });
                    return returnObj;
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public string GetAssetName(ITransaction tx, int assetId, int mappingFileVersion = 0)
        {
            string assetName = string.Empty;
            AssetsDao dao = new AssetsDao();
            var result = tx.PersistenceManager.PlanningRepository.Get<AssetsDao>(assetId);
            if (mappingFileVersion == 0)
                assetName = "AttributeRecord" + result.AssetTypeid + "_V" + MarcomManagerFactory.ActiveMetadataVersionNumber;
            else
                assetName = "AttributeRecord" + result.AssetTypeid + "_V" + mappingFileVersion.ToString();
            return assetName;
        }

        public int CreateFolder(DigitalAssetManagerProxy proxy, int entityID, int parentNodeID, int Level, int nodeId, int id, string Key, int sortorder, string caption, string description, string colorcode)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    FolderDao fDao = new FolderDao();
                    fDao = tx.PersistenceManager.DamRepository.Query<FolderDao>().Where(a => a.Id == id).Select(a => a).FirstOrDefault();
                    if (fDao == null)
                    {
                        fDao = new FolderDao();
                        fDao.Caption = caption;
                        fDao.Description = description;
                        fDao.EntityID = entityID;
                        fDao.ParentNodeID = parentNodeID;
                        fDao.KEY = Key;
                        fDao.NodeID = nodeId;
                        fDao.Level = Level;
                        fDao.SortOrder = sortorder;
                        fDao.Colorcode = colorcode;
                    }
                    else
                    {
                        fDao.Caption = caption;
                        fDao.Description = description;
                        fDao.Colorcode = colorcode;
                    }
                    tx.PersistenceManager.DamRepository.Save<FolderDao>(fDao);
                    tx.Commit();
                    return fDao.Id;

                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public int UpdateFolder(DigitalAssetManagerProxy proxy, int id, string caption, string description, string colorcode)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    FolderDao fDao = new FolderDao();
                    fDao = tx.PersistenceManager.DamRepository.Query<FolderDao>().Where(a => a.Id == id).Select(a => a).FirstOrDefault();
                    if (fDao != null)
                    {
                        fDao.Caption = caption;
                        fDao.Description = description;
                        fDao.Colorcode = colorcode;
                        tx.PersistenceManager.DamRepository.Save<FolderDao>(fDao);
                    }
                    tx.Commit();
                    return fDao.Id;
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public bool DeleteFolder(DigitalAssetManagerProxy proxy, int[] idArr)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    IList<FolderDao> iifDao = new List<FolderDao>();
                    IList<AssetsDao> iiasstDao = new List<AssetsDao>();
                    iifDao = tx.PersistenceManager.DamRepository.Query<FolderDao>().Where(a => idArr.Contains(a.Id)).Select(a => a).ToList();
                    if (iifDao.Count > 0)
                    {
                        iiasstDao = tx.PersistenceManager.DamRepository.Query<AssetsDao>().Where(a => idArr.Contains(a.FolderID)).Select(a => a).ToList();
                        if (iiasstDao.Count > 0)
                        {

                            string assetIDinClause = "("
                                  + String.Join(",", iiasstDao.Select(a => a.ID).ToArray().Select(x => x.ToString()).Distinct().ToArray())
                                + ")";
                            int[] assetTypeobj = iiasstDao.Select(a => a.AssetTypeid).Distinct().ToArray();

                            StringBuilder qry = new StringBuilder();

                            foreach (var val in assetTypeobj)
                            {
                                qry.AppendLine("DELETE FROM MM_AttributeRecord_" + val + " WHERE id IN " + assetIDinClause + "");
                            }
                            tx.PersistenceManager.DamRepository.ExecuteQuery(qry.ToString());
                            tx.PersistenceManager.DamRepository.Delete<AssetsDao>(iiasstDao);
                        }
                        tx.PersistenceManager.DamRepository.Delete<FolderDao>(iifDao);
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

        public bool DeleteAssets(DigitalAssetManagerProxy proxy, int[] idArr)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {

                    IList<AssetsDao> iiasstDao = new List<AssetsDao>();
                    iiasstDao = tx.PersistenceManager.DamRepository.Query<AssetsDao>().Where(a => idArr.Contains(a.ID)).Select(a => a).ToList();
                    if (iiasstDao.Count > 0)
                    {

                        string assetIDinClause = "("
                              + String.Join(",", iiasstDao.Select(a => a.ID).ToArray().Select(x => x.ToString()).Distinct().ToArray())
                            + ")";
                        int[] assetTypeobj = iiasstDao.Select(a => a.AssetTypeid).Distinct().ToArray();

                        StringBuilder qry = new StringBuilder();

                        foreach (var val in assetTypeobj)
                        {
                            qry.AppendLine("DELETE FROM MM_AttributeRecord_" + val + " WHERE id IN " + assetIDinClause + "");
                        }
                        tx.PersistenceManager.DamRepository.ExecuteQuery(qry.ToString());
                        tx.PersistenceManager.DamRepository.Delete<AssetsDao>(iiasstDao);
                        foreach (var valasset in iiasstDao)
                        {
                            BrandSystems.Marcom.Core.Utility.FeedNotificationServer fs = new Utility.FeedNotificationServer();
                            NotificationFeedObjects obj = new NotificationFeedObjects();
                            obj.action = "delete Asset";
                            obj.Actorid = proxy.MarcomManager.User.Id;
                            obj.EntityId = valasset.EntityID;
                            obj.AttributeName = valasset.Name;
                            obj.TypeName = "Asset deletion";
                            obj.CreatedOn = DateTimeOffset.Now;
                            obj.AssociatedEntityId = valasset.ID;
                            if (valasset.ActiveFileID > 0)
                            {
                                var FileObj = tx.PersistenceManager.DamRepository.Query<DAMFileDao>().ToList().Where(item => item.ID == valasset.ActiveFileID).Select(item => item.VersionNo).Max();
                                obj.Version = FileObj;
                            }
                            else
                                obj.Version = 1;
                            fs.AsynchronousNotify(obj);
                        }
                    }

                    tx.Commit();

                    BrandSystems.Marcom.Core.Interface.Managers.IEventManager _eventManager = null;
                    BrandSystems.Marcom.Core.Interface.Managers.IPluginManager _pluginManager = null;
                    MarcomManager marcommanager = new MarcomManager(_eventManager, _pluginManager);
                    BrandSystems.Marcom.Core.Managers.Proxy.PlanningManagerProxy pProxy = new BrandSystems.Marcom.Core.Managers.Proxy.PlanningManagerProxy(marcommanager);
                    System.Threading.Tasks.Task addtaskforsearch = new System.Threading.Tasks.Task(() => PlanningManager.Instance.RemoveEntityAsyncDAM(pProxy, idArr));
                    addtaskforsearch.Start();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public string DownloadDamFiles(DigitalAssetManagerProxy proxy, int[] assetid, int sendDamFiles = 0, List<KeyValuePair<string, int>> CroppedList = null)
        {
            try
            {
                string directoryPath = AppDomain.CurrentDomain.BaseDirectory;
                string newguid = Guid.NewGuid().ToString();
                string DamDir = directoryPath + "DAMFiles\\Original";
                string sDir = directoryPath + "DAMDownloads";
                string directoryDestPath = directoryPath + "DAMDownloads";
                DeleteZipFiles(directoryDestPath); // delete all the previous zip files from the destination
                CleanSrcFolder(sDir);  //Clear previous files from the source file

                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    if (assetid.Length > 0)
                    {
                        string assetIDinClause = "("
                                 + String.Join(",", assetid.Select(a => a).ToArray().Select(x => x.ToString()).Distinct().ToArray())
                               + ")";
                        StringBuilder qry = new StringBuilder();
                        qry.AppendLine("  SELECT df.Name,df.FileGuid,");
                        qry.AppendLine(" df.Extension,df.VersionNo,df.AssetID,(SELECT EntityID FROM DAM_Asset WHERE  id=df.AssetID) AS EntityID,(SELECT NAME  FROM DAM_Asset WHERE  id=df.AssetID) AS AssetName");
                        qry.AppendLine(" FROM   DAM_File df");
                        qry.AppendLine(" WHERE  id IN ( SELECT da.ActiveFileID");
                        qry.AppendLine(" FROM   DAM_Asset da");
                        qry.AppendLine("  WHERE  id IN " + assetIDinClause + " )");
                        IList files = tx.PersistenceManager.DamRepository.ExecuteQuery(qry.ToString());
                        tx.Commit();
                        if (files != null && files.Count > 0)
                        {
                            if (createNewFolder(sDir + "\\" + newguid))
                            {
                                sDir = sDir + "\\" + newguid;
                                foreach (var file in files) //logic for copy all the files from the dam original files into dam folder from where zip will generate
                                {
                                    var matches = from val in CroppedList where Convert.ToInt32(val.Key) == Convert.ToInt32((int)((System.Collections.Hashtable)(file))["AssetID"]) select val.Value;
                                    if (matches.FirstOrDefault() == 1)
                                        DamDir = directoryPath + "DAMFiles\\Temp";
                                    else
                                        DamDir = directoryPath + "DAMFiles\\Original";
                                    if (System.IO.File.Exists(DamDir + "\\" + Convert.ToString((Guid)((System.Collections.Hashtable)(file))["FileGuid"]) + (string)((System.Collections.Hashtable)(file))["Extension"]))
                                    {
                                        string srcPath = DamDir + "\\" + Convert.ToString((Guid)((System.Collections.Hashtable)(file))["FileGuid"]) + (string)((System.Collections.Hashtable)(file))["Extension"];
                                        string destPath = System.IO.Path.Combine(sDir, (string)((System.Collections.Hashtable)(file))["Name"]);
                                        //Copy the file from sourcepath and place into mentioned target path, 
                                        ////Overwrite the file if same file is exist in target path
                                        System.IO.File.Copy(srcPath, srcPath.Replace(srcPath, destPath), true);
                                        if (sendDamFiles == 0)
                                        {
                                            BrandSystems.Marcom.Core.Utility.FeedNotificationServer fs = new Utility.FeedNotificationServer();
                                            NotificationFeedObjects obj = new NotificationFeedObjects();
                                            obj.action = "Download Asset";
                                            obj.Actorid = proxy.MarcomManager.User.Id;
                                            obj.EntityId = Convert.ToInt32((int)((System.Collections.Hashtable)(file))["EntityID"]);
                                            obj.AttributeName = Convert.ToString((string)((System.Collections.Hashtable)(file))["AssetName"]);
                                            obj.TypeName = "Asset Download";
                                            obj.CreatedOn = DateTimeOffset.Now;
                                            obj.AssociatedEntityId = Convert.ToInt32((int)((System.Collections.Hashtable)(file))["AssetID"]);
                                            obj.Version = Convert.ToInt32((int)((System.Collections.Hashtable)(file))["VersionNo"]); ;
                                            fs.AsynchronousNotify(obj);
                                        }
                                    }
                                }
                                GenerateZipFolder(directoryPath, directoryDestPath, sDir, newguid);  // generate zip folder

                                return newguid;
                            }
                        }
                    }


                    tx.Commit();

                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }




        public int AttachAssetsEntityCreation(DigitalAssetManagerProxy proxy, int entityID, int[] assetid)
        {
            int newasset = 0;
            try
            {
                IList<FolderDao> iflderDao = new List<FolderDao>();
                Dictionary<int, int> foldermappingdict = new Dictionary<int, int>();
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {

                    if (assetid.Length > 0)
                    {
                        int[] folderIds = tx.PersistenceManager.DamRepository.Query<AssetsDao>().Where(a => assetid.Contains(a.ID)).Select(a => a.FolderID).Distinct().ToArray();
                        iflderDao = tx.PersistenceManager.DamRepository.Query<FolderDao>().Where(a => folderIds.Contains(a.Id)).ToList();
                        FolderDao fdao = new FolderDao();
                        if (iflderDao != null)
                        {
                            foreach (var folder in iflderDao)
                            {

                                fdao = new FolderDao();
                                fdao.Id = 0;
                                fdao.Caption = folder.Caption;
                                fdao.Description = folder.Description;
                                fdao.EntityID = entityID;
                                fdao.KEY = folder.KEY;
                                fdao.Level = folder.Level;
                                fdao.NodeID = folder.NodeID;
                                fdao.ParentNodeID = folder.ParentNodeID;
                                fdao.SortOrder = folder.SortOrder;
                                fdao.Colorcode = (folder.Colorcode != null) ? folder.Colorcode : "ffd300";
                                tx.PersistenceManager.DamRepository.Save<FolderDao>(fdao);
                                if (!foldermappingdict.ContainsKey(fdao.Id))
                                {
                                    foldermappingdict.Add(folder.Id, fdao.Id);
                                }

                            }
                            tx.Commit();

                        }

                    }
                }


                if (iflderDao != null)
                {

                    for (int i = 0; i < assetid.Length; i++)
                    {

                        List<IAssets> assetdet = new List<IAssets>();
                        IAssets asset = new Assets();
                        asset = GetAssetAttributesDetails(proxy, assetid[i], false);


                        IList<IAttributeData> AttributeDatanew = new List<IAttributeData>();
                        AttributeDatanew = asset.AttributeData;
                        if (asset.Category == 0)
                        {
                            var Filesassest = asset.Files.Where(a => a.ID == asset.ActiveFileID).Select(a => a).ToList();
                            newasset = CreateAsset(proxy, Convert.ToInt32(foldermappingdict[asset.FolderID]), Convert.ToInt32(asset.AssetTypeid), asset.Name, AttributeDatanew, Filesassest[0].Name, 1, Filesassest[0].MimeType, Filesassest[0].Extension, Convert.ToInt64(Filesassest[0].Size), entityID, Filesassest[0].Fileguid.ToString(), Filesassest[0].Description, true, Filesassest[0].Status, 0, Filesassest[0].Additionalinfo, asset.AssetAccess, asset.ID);
                        }
                        else
                        {
                            newasset = CreateBlankAsset(proxy, Convert.ToInt32(foldermappingdict[asset.FolderID]), Convert.ToInt32(asset.AssetTypeid), asset.Name, AttributeDatanew, entityID, asset.Category, asset.Url, true, 0, asset.AssetAccess, asset.ID);

                        }
                    }
                }

                return newasset;

            }
            catch (Exception ex)
            {
                return 0;
            }

        }

        public int DuplicateAssets(DigitalAssetManagerProxy proxy, int[] assetid)
        {
            int newasset = 0;
            try
            {



                //using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                //{

                if (assetid.Length > 0)
                {
                    for (int i = 0; i < assetid.Length; i++)
                    {

                        List<IAssets> assetdet = new List<IAssets>();
                        IAssets asset = new Assets();
                        asset = GetAssetAttributesDetails(proxy, assetid[i], false);


                        IList<IAttributeData> AttributeDatanew = new List<IAttributeData>();
                        AttributeDatanew = asset.AttributeData;
                        if (asset.Category == 0)
                        {
                            var Filesassest = asset.Files.Where(a => a.ID == asset.ActiveFileID).Select(a => a).ToList();

                            newasset = CreateAsset(proxy, Convert.ToInt32(asset.FolderID), Convert.ToInt32(asset.AssetTypeid), "Copy of " + asset.Name, AttributeDatanew, Filesassest[0].Name, 1, Filesassest[0].MimeType, Filesassest[0].Extension, Convert.ToInt64(Filesassest[0].Size), Convert.ToInt32(asset.EntityID), Filesassest[0].Fileguid.ToString(), Filesassest[0].Description, true, Filesassest[0].Status, 0, Filesassest[0].Additionalinfo, asset.AssetAccess, asset.ID);
                        }
                        else
                        {
                            newasset = CreateBlankAsset(proxy, Convert.ToInt32(asset.FolderID), Convert.ToInt32(asset.AssetTypeid), "Copy of " + asset.Name, AttributeDatanew, Convert.ToInt32(asset.EntityID), asset.Category, asset.Url, true, 0, asset.AssetAccess, asset.ID);
                            //newasset = CreateAsset(proxy, Convert.ToInt32(asset.FolderID), Convert.ToInt32(asset.AssetTypeid), "Copy of " + asset.Name, AttributeDatanew, Filesassest[0].Name, Convert.ToInt32(Filesassest[0].VersionNo), Filesassest[0].MimeType, Filesassest[0].Extension, Convert.ToInt64(Filesassest[0].Size), Convert.ToInt32(proxy.MarcomManager.User.Id), Convert.ToDateTime(d2), 5, Convert.ToInt32(asset.EntityID), Filesassest[0].Fileguid.ToString(), Filesassest[0].Description, true);

                        }
                        //}
                        //tx.Commit();


                    }
                    return newasset;
                }
            }
            catch (Exception ex)
            {
                return 0;
            }

            return newasset;
        }

        public bool createNewFolder(string filePath)
        {

            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
                return true;
            }
            return false;
        }

        public void GenerateZipFolder(string directoryPath, string directoryDestPath, string sDir, string newguiid)
        {
            try
            {
                DirectoryInfo directorySelected = new DirectoryInfo(directoryPath);
                if (!string.IsNullOrWhiteSpace(sDir) && Directory.Exists(sDir))
                {
                    string targetFile = Path.Combine(directoryDestPath, newguiid + ".zip");
                    ZipPath(targetFile, sDir, null, true, null);
                }
            }
            catch { }
        }

        public void DeleteZipFiles(string path)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            FileInfo[] files = di.GetFiles("*.zip")
                                 .Where(p => p.Extension == ".zip").ToArray();
            foreach (FileInfo file in files)
                try
                {
                    if (file.LastAccessTime < DateTime.Now.AddDays(-5))
                    {
                        file.Attributes = FileAttributes.Normal;
                        System.IO.File.Delete(file.FullName);
                    }
                }
                catch { }

        }

        public void CleanSrcFolder(string path)
        {
            DirectoryInfo di1 = new DirectoryInfo(path);
            FileInfo[] files = di1.GetFiles();
            DirectoryInfo[] diArr = di1.GetDirectories();
            System.DateTime dteDateToDeleteBy = default(System.DateTime);
            dteDateToDeleteBy = DateTime.Today.Date.AddDays(-4);
            foreach (FileInfo file in files)
            {
                try
                {
                    int filedifferentdays = ((TimeSpan)(file.LastAccessTime - dteDateToDeleteBy)).Days;

                    if (filedifferentdays < 0)
                    {
                        file.Attributes = FileAttributes.Normal;
                        System.IO.File.Delete(file.FullName);
                    }


                }
                catch { }
            }
            foreach (DirectoryInfo dri in diArr)
            {
                try
                {
                    int dirdifferentdays = ((TimeSpan)(dri.LastAccessTime - dteDateToDeleteBy)).Days;

                    if (dirdifferentdays < 0)
                    {
                        dri.Delete(true);

                    }


                }
                catch { }

            }
        }

        public static void ZipPath(string zipFilePath, string sourceDir, string pattern, bool withSubdirs, string password)
        {
            FastZip fz = new FastZip();
            if (password != null)
                fz.Password = password;

            fz.CreateZip(zipFilePath, sourceDir, withSubdirs, pattern);
        }
        public List<FileInfo> GetFiles(string path, params string[] extensions)
        {
            List<FileInfo> list = new List<FileInfo>();
            foreach (string ext in extensions)
                list.AddRange(new DirectoryInfo(path).GetFiles("*" + ext).Where(p =>
                      p.Extension.Equals(ext, StringComparison.CurrentCultureIgnoreCase))
                      .ToArray());
            return list;
        }


        public DAMFileDao SaveFiletoAsset(DigitalAssetManagerProxy digitalAssetManagerProxy, int AssetID, int Status, string MimeType, long Size, string FileGuid, DateTime CreatedOn, string Extension, string Name, int VersionNo, string Description, int OwnerID)
        {
            List<int> tasklist = new List<int>();
            List<int> tasklistAssetTask = new List<int>();
            int fileid = 0;
            int versioningfileid;
            DAMFileDao filedetails = new DAMFileDao();
            try
            {
                Guid NewId = Guid.NewGuid();
                string filePath = ReadAdminXML("FileManagment");
                var DirInfo = System.IO.Directory.GetParent(filePath);
                string newFilePath = DirInfo.FullName;
                System.IO.File.Move(filePath + "\\" + FileGuid + Extension, newFilePath + "\\" + NewId + Extension);
                using (ITransaction tx = digitalAssetManagerProxy.MarcomManager.GetTransaction())
                {
                    var FileObj = 0;
                    if (VersionNo != 0)
                    {
                        FileObj = tx.PersistenceManager.DamRepository.Query<DAMFileDao>().ToList().Where(item => item.AssetID == AssetID).Select(item => item.VersionNo).Max();
                    }
                    filedetails.AssetID = AssetID;
                    filedetails.Status = Status;
                    filedetails.MimeType = MimeType;
                    filedetails.Size = Size;
                    filedetails.FileGuid = NewId;
                    filedetails.CreatedOn = CreatedOn;
                    filedetails.Extension = Extension;
                    filedetails.Name = Name;
                    filedetails.VersionNo = FileObj + 1;
                    filedetails.Description = Description;
                    filedetails.OwnerID = OwnerID;
                    tx.PersistenceManager.DamRepository.Save<DAMFileDao>(filedetails);
                    fileid = filedetails.ID;
                    AssetsDao assetDao = new AssetsDao();
                    assetDao = (from item in tx.PersistenceManager.PlanningRepository.Query<AssetsDao>()
                                where item.ID == AssetID
                                select item).FirstOrDefault();
                    //Updating the Activefileid in the DAM_Asset table 
                    tx.PersistenceManager.DamRepository.ExecuteQuerywithMinParam("UPDATE DAM_Asset SET ActiveFileID = ? ,UpdatedOn =?,Category=?  WHERE ID = ? ", fileid, CreatedOn, 0, AssetID);
                    //Updating the ApprovalStatus in the TM_Task_Members table who are related to Tasks, as we are adding a new Version file to the Asset which is used in the Task creation.
                    tx.PersistenceManager.DamRepository.ExecuteQuerywithMinParam("UPDATE TM_Task_Members SET ApprovalStatus = NULL WHERE TaskID IN (SELECT ID from TM_EntityTask where AssetId = ?)", AssetID);
                    tasklist = tx.PersistenceManager.TaskRepository.Query<EntityTaskDao>().ToList().Where(item => item.AssetId == AssetID).Select(item => item.ID).ToList();
                    if (assetDao.FolderID == 0)
                    {
                        tasklistAssetTask = tasklist = tx.PersistenceManager.TaskRepository.Query<EntityTaskDao>().ToList().Where(item => item.ID == assetDao.EntityID).Select(item => item.ID).ToList();
                    }
                    tx.Commit();

                    if (FileObj > 0)
                    {
                        try
                        {
                            var asset = tx.PersistenceManager.DamRepository.Get<BrandSystems.Marcom.Dal.DAM.Model.AssetsDao>(AssetID);
                            BrandSystems.Marcom.Core.Utility.FeedNotificationServer fs = new Utility.FeedNotificationServer();
                            NotificationFeedObjects obj = new NotificationFeedObjects();
                            obj.action = "Add AssetVersion";
                            obj.Actorid = digitalAssetManagerProxy.MarcomManager.User.Id;
                            obj.EntityId = asset.EntityID;
                            obj.ToValue = fileid.ToString();
                            obj.AttributeName = HttpUtility.HtmlEncode(Name);
                            obj.TypeName = "Add Asset Version";
                            obj.CreatedOn = DateTimeOffset.Now;
                            obj.AssociatedEntityId = AssetID;
                            obj.Version = FileObj + 1;
                            fs.AsynchronousNotify(obj);

                            //BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("Updated the Feeds", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);

                        }
                        catch { }
                    }
                }

                if (tasklist.Count > 0)
                {
                    Guid NewIdfrtask = Guid.NewGuid();
                    string filePathfrTasks = ReadAdminXML_uploadedfiles("FileManagment");
                    var DirInfofrTasks = System.IO.Directory.GetParent(filePathfrTasks);
                    string newFilePathfrTasks = DirInfofrTasks.FullName;
                    System.IO.File.Copy(newFilePath + "\\" + NewId + Extension, newFilePathfrTasks + "\\" + NewIdfrtask + Extension);

                    foreach (var taskid in tasklist)
                    {
                        using (ITransaction tx = digitalAssetManagerProxy.MarcomManager.GetTransaction())
                        {
                            versioningfileid = tx.PersistenceManager.TaskRepository.Query<AttachmentsDao>().ToList().Where(item => item.Entityid == (int)taskid).Select(item => item.VersioningFileId).FirstOrDefault();
                            //get the active file from cm_file table using taskid....this is to get the active version no from cm_file used below.
                            if (versioningfileid > 0)
                            {
                                var FileObj1 = tx.PersistenceManager.TaskRepository.Query<FileDao>().ToList().Where(item => item.Id == (int)versioningfileid && item.Entityid == (int)taskid).Select(item => item.VersionNo).FirstOrDefault();
                                if (FileObj1 > 0)
                                {
                                    tx.PersistenceManager.TaskRepository.ExecuteQuerywithMinParam(" update PM_Attachments set activefileversionID = 0 where EntityID = ?  and VersioningFileId = ? ", (int)taskid, (int)versioningfileid);
                                    tx.Commit();
                                }
                            }
                        }

                        using (ITransaction tx = digitalAssetManagerProxy.MarcomManager.GetTransaction())
                        {
                            if (versioningfileid > 0)
                            {
                                var FileObj = tx.PersistenceManager.TaskRepository.Query<AttachmentsDao>().ToList().Where(item => item.VersioningFileId == versioningfileid && item.Entityid == (int)taskid).Select(item => item.ActiveVersionNo).Max();
                                FileObj += 1;
                                var adminObj = tx.PersistenceManager.TaskRepository.Query<AdminTaskDao>().ToList().Where(item => item.ID == (int)taskid).Select(item => item.TaskType).FirstOrDefault();
                                IList<FileDao> ifile = new List<FileDao>();
                                FileDao fldao = new FileDao();
                                fldao = new FileDao();
                                fldao.CreatedOn = DateTime.Now;
                                fldao.Entityid = (int)taskid;
                                fldao.Extension = Extension;
                                fldao.MimeType = MimeType;
                                fldao.Moduleid = 0;
                                fldao.Name = Name;
                                fldao.Ownerid = OwnerID;
                                fldao.Size = Size;
                                fldao.VersionNo = FileObj;
                                fldao.Fileguid = NewIdfrtask;
                                fldao.Description = Description;
                                ifile.Add(fldao);
                                //descriptionObj.Add(NewId, a.Description);
                                tx.PersistenceManager.PlanningRepository.Save<FileDao>(ifile);
                                fileid = fldao.Id;
                                BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("Saved in File table", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);

                                IList<AttachmentsDao> iattachment = new List<AttachmentsDao>();
                                AttachmentsDao attachedao = new AttachmentsDao();
                                attachedao.ActiveFileid = fileid;
                                attachedao.ActiveVersionNo = FileObj;
                                attachedao.Createdon = DateTime.Now;
                                attachedao.Entityid = taskid;
                                attachedao.Name = Name;
                                attachedao.Typeid = adminObj;
                                attachedao.ActiveFileVersionID = fileid;
                                attachedao.VersioningFileId = versioningfileid;
                                //attachedao.Description = result[0].Value;
                                iattachment.Add(attachedao);
                                tx.PersistenceManager.PlanningRepository.Save<AttachmentsDao>(iattachment);
                                BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("Saved in Attachments", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);
                                tx.Commit();
                            }
                        }
                        //ReinitializeTask(digitalAssetManagerProxy, taskid);
                    }
                }

                try
                {
                    BrandSystems.Marcom.Core.Interface.Managers.IEventManager _eventManager = null;
                    BrandSystems.Marcom.Core.Interface.Managers.IPluginManager _pluginManager = null;
                    MarcomManager marcommanager = new MarcomManager(_eventManager, _pluginManager);
                    BrandSystems.Marcom.Core.Managers.Proxy.PlanningManagerProxy pProxy = new BrandSystems.Marcom.Core.Managers.Proxy.PlanningManagerProxy(marcommanager);
                    System.Threading.Tasks.Task addtaskforsearch = new System.Threading.Tasks.Task(() => PlanningManager.Instance.AddEntityAsyncDam(pProxy, AssetID, fileid));
                    addtaskforsearch.Start();
                }

                catch { }
                if (tasklistAssetTask.Count > 0)
                {

                    foreach (var Assettaskid in tasklistAssetTask)
                    {
                        ReinitializeTask(digitalAssetManagerProxy, Assettaskid);
                    }
                }
                return filedetails;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool UpdateAssetVersion(DigitalAssetManagerProxy digitalAssetManagerProxy, int AssetID, int fileid)
        {
            try
            {
                List<int> tasklist = new List<int>();
                using (ITransaction tx = digitalAssetManagerProxy.MarcomManager.GetTransaction())
                {
                    DateTime d1 = DateTime.UtcNow;
                    tx.PersistenceManager.DamRepository.ExecuteQuerywithMinParam("Update DAM_Asset SET [ActiveFileID] = ?,UpdatedOn=?  WHERE [ID]= ?  ", fileid, d1, AssetID);
                    tasklist = tx.PersistenceManager.TaskRepository.Query<EntityTaskDao>().ToList().Where(item => item.AssetId == AssetID).Select(item => item.ID).ToList();
                    tx.Commit();
                }
                using (ITransaction tx = digitalAssetManagerProxy.MarcomManager.GetTransaction())
                {
                    foreach (var taskid in tasklist)
                    {
                        var activefileid = tx.PersistenceManager.DamRepository.Query<DAMFileDao>().Where(item => item.AssetID == AssetID && item.ID == fileid).Select(item => item.VersionNo).FirstOrDefault();
                        tx.PersistenceManager.TaskRepository.ExecuteQuerywithMinParam(" update PM_Attachments set activefileversionID = 0 where EntityID = ?", taskid);
                        tx.PersistenceManager.TaskRepository.ExecuteQuerywithMinParam(" update PM_Attachments set activeFileVersionID = ActiveFileID where EntityID = ? and ActiveVersionNo= ? ", (int)taskid, (int)activefileid);
                        tx.Commit();
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        public bool SaveDetailBlockForAssets(DigitalAssetManagerProxy proxy, int AssetID, int AttributeTypeid, int attributeid, List<object> NewValue, int Level)
        {
            try
            {
                //proxy.MarcomManager.AccessManager.TryEntityTypeAccess(EntityID, Modules.Planning);
                Guid userSession = MarcomManagerFactory.GetSystemSession();
                IMarcomManager managers = MarcomManagerFactory.GetMarcomManager(userSession);
                NotificationFeedObjects obj = new NotificationFeedObjects();
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    var currentUserRole = (from item in tx.PersistenceManager.DamRepository.Query<BrandSystems.Marcom.Dal.Access.Model.EntityRoleUserDao>() where item.Entityid == AssetID select item).FirstOrDefault();
                    var attrdetails = (from item in tx.PersistenceManager.DamRepository.Query<AttributeDao>() where item.Id == attributeid select item).FirstOrDefault();
                    var DynamicQuery = new StringBuilder();
                    var assetTypeid = tx.PersistenceManager.DamRepository.Get<BrandSystems.Marcom.Dal.DAM.Model.AssetsDao>(AssetID);
                    var attr = tx.PersistenceManager.DamRepository.Get<BrandSystems.Marcom.Dal.Metadata.Model.AttributeDao>(attributeid);
                    BrandSystems.Marcom.Core.Utility.FeedNotificationServer fs = new Utility.FeedNotificationServer();

                    string stroldvalue = "";
                    IList<MultiProperty> prplst = new List<MultiProperty>();
                    if (AttributeTypeid == 1)
                    {
                        if (attrdetails.IsSpecial == true && attrdetails.Id == Convert.ToInt32(SystemDefinedAttributes.Name))
                        {
                            using (ITransaction txtemp = managers.GetTransaction())
                            {
                                obj.obj3 = txtemp.PersistenceManager.DamRepository.ExecuteQuerywithMinParam("select * from DAM_Asset where id=?", AssetID).Cast<Hashtable>().ToList();
                                txtemp.Commit();
                            }
                            BrandSystems.Marcom.Dal.DAM.Model.AssetsDao assetdao = new BrandSystems.Marcom.Dal.DAM.Model.AssetsDao();
                            assetdao.ID = AssetID;
                            assetdao = tx.PersistenceManager.DamRepository.Get<BrandSystems.Marcom.Dal.DAM.Model.AssetsDao>(assetdao.ID);
                            assetdao.Name = HttpUtility.HtmlEncode((string)NewValue[0]);
                            tx.PersistenceManager.DamRepository.Save<BrandSystems.Marcom.Dal.DAM.Model.AssetsDao>(assetdao);
                        }
                        else
                        {
                            string str = "select * from MM_AttributeRecord_" + assetTypeid.AssetTypeid + " where ID= ? ";
                            IList item = tx.PersistenceManager.DamRepository.ExecuteQuerywithMinParam(str, Convert.ToInt32(AssetID));
                            obj.obj3 = item;
                            DynamicQuery.Append("update  MM_AttributeRecord_" + assetTypeid.AssetTypeid + " set Attr_" + attributeid + "= ?  where ID= ?");
                            tx.PersistenceManager.DamRepository.ExecuteQuerywithMinParam(DynamicQuery.ToString(), HttpUtility.HtmlEncode((string)NewValue[0]), AssetID);
                        }

                    }
                    else if (AttributeTypeid == 2)
                    {
                        if (attrdetails.IsSpecial == false)
                        {
                            string str = "select * from MM_AttributeRecord_" + assetTypeid.AssetTypeid + " where ID= ? ";
                            IList item = tx.PersistenceManager.DamRepository.ExecuteQuerywithMinParam(str, Convert.ToInt32(AssetID));
                            obj.obj3 = item;
                            DynamicQuery.Append("update  MM_AttributeRecord_" + assetTypeid.AssetTypeid + " set Attr_" + attributeid + "= ? where ID= ? ");
                        }
                        tx.PersistenceManager.DamRepository.ExecuteQuerywithMinParam(DynamicQuery.ToString(), HttpUtility.HtmlEncode((string)NewValue[0]), AssetID);
                    }

                    else if (AttributeTypeid == 3)
                    {
                        IList<MultiProperty> parList3 = new List<MultiProperty>();
                        if (attrdetails.IsSpecial == true && attrdetails.Id == Convert.ToInt32(SystemDefinedAttributes.Owner))
                        {

                            prplst.Add(new MultiProperty { propertyName = BrandSystems.Marcom.Dal.Access.Model.EntityRoleUserDao.PropertyNames.Entityid, propertyValue = AssetID });
                            prplst.Add(new MultiProperty { propertyName = BrandSystems.Marcom.Dal.Access.Model.EntityRoleUserDao.PropertyNames.Roleid, propertyValue = 1 });
                            obj.obj3 = (tx.PersistenceManager.PlanningRepository.GetEquals<BrandSystems.Marcom.Dal.Access.Model.EntityRoleUserDao>(prplst)).ToList();

                            var currentRoleEditorObj = tx.PersistenceManager.AccessRepository.Query<EntityTypeRoleAclDao>().Where(ta => ta.EntityTypeID == assetTypeid.AssetTypeid && (EntityRoles)ta.EntityRoleID == EntityRoles.Editer).Take(1).SingleOrDefault();
                            var currentRoleOwnerObj = tx.PersistenceManager.AccessRepository.Query<EntityTypeRoleAclDao>().Where(ta => ta.EntityTypeID == assetTypeid.AssetTypeid && (EntityRoles)ta.EntityRoleID == EntityRoles.Owner).SingleOrDefault();
                            int count = Convert.ToInt32((from u in tx.PersistenceManager.UserRepository.Query<BrandSystems.Marcom.Dal.Access.Model.EntityRoleUserDao>() where u.Roleid == currentRoleEditorObj.ID && u.Entityid == AssetID && u.Userid == (int)NewValue[0] select u).Count());
                            if (count > 0)
                            {
                                parList3.Add(new MultiProperty { propertyName = "UserID_NewValue", propertyValue = (int)NewValue[0] });
                                parList3.Add(new MultiProperty { propertyName = "EntityID", propertyValue = AssetID });
                                DynamicQuery.Append("update  AM_Entity_Role_User set UserID= :UserID_NewValue where EntityID= :EntityID and RoleID=" + currentRoleOwnerObj.ID);

                            }
                            else
                            {
                                // insert the owner and change the previous owner
                                parList3.Add(new MultiProperty { propertyName = "EntityID", propertyValue = AssetID });
                                DynamicQuery.Append("update  AM_Entity_Role_User set RoleID=" + currentRoleEditorObj.ID + " where EntityID= :EntityID and RoleID=" + currentRoleOwnerObj.ID + "  ");
                                int isherited = currentUserRole.IsInherited == true ? 1 : 0;
                                //parList3.Add(new MultiProperty { propertyName = "EntityID", propertyValue = EntityID });
                                parList3.Add(new MultiProperty { propertyName = "UserID_NewValue", propertyValue = (int)NewValue[0] });
                                parList3.Add(new MultiProperty { propertyName = "isherited", propertyValue = isherited });
                                parList3.Add(new MultiProperty { propertyName = "currentUserRole_InheritedFromEntityid", propertyValue = currentUserRole.InheritedFromEntityid });

                                DynamicQuery.Append(" insert into AM_Entity_Role_User values (:EntityID, " + currentRoleOwnerObj.ID + ", :UserID_NewValue, :isherited, :currentUserRole_InheritedFromEntityid)");
                            }
                        }
                        else
                        {
                            obj.EntityTypeId = assetTypeid.AssetTypeid;

                            string str = "select * from MM_AttributeRecord_" + assetTypeid.AssetTypeid + " where ID= ? ";
                            IList item = tx.PersistenceManager.MetadataRepository.ExecuteQuerywithMinParam(str, Convert.ToInt32(AssetID));
                            obj.obj3 = item;
                            parList3.Add(new MultiProperty { propertyName = "UserID_NewValue", propertyValue = (int)NewValue[0] });
                            parList3.Add(new MultiProperty { propertyName = "AssetID", propertyValue = AssetID });
                            DynamicQuery.Append("update  MM_AttributeRecord_" + assetTypeid.AssetTypeid + " set Attr_" + attributeid + "= :UserID_NewValue where ID= :AssetID");

                        }
                        tx.PersistenceManager.DamRepository.ExecuteQuerywithParam(DynamicQuery.ToString(), parList3);
                    }


                    else if (AttributeTypeid == 4)
                    {
                        obj.obj3 = new ArrayList();
                        obj.obj2 = new List<object>();

                        //obj.obj3 = tx.PersistenceManager.CommonRepository.GetEquals<BrandSystems.Marcom.Dal.Planning.Model.BaseEntityDao>(BrandSystems.Marcom.Dal.Planning.Model.BaseEntityDao.PropertyNames.Id, EntityID).ToList();
                        //BrandSystems.Marcom.Dal.Planning.Model.BaseEntityDao baseentity = new BrandSystems.Marcom.Dal.Planning.Model.BaseEntityDao();

                        //baseentity.Id = EntityID;
                        //baseentity = tx.PersistenceManager.CommonRepository.Get<BrandSystems.Marcom.Dal.Planning.Model.BaseEntityDao>(baseentity.Id);
                        ////oldvalue = baseentity.Name;
                        //baseentity.Name = (string)NewValue[0];

                        //if (attrdetails.IsSpecial == false)
                        //{
                        //    string str = "select * from MM_AttributeRecord_" + entityTypeid.Typeid + " where ID=" + Convert.ToInt32(EntityID) + "";
                        //    IList item = tx.PersistenceManager.MetadataRepository.ExecuteQuery(str);
                        //    obj.obj3 = item;

                        //    DynamicQuery.Append("update  MM_AttributeRecord_" + entityTypeid.Typeid + " set Attr_" + attributeid + "=" + (int)NewValue[0] + " where ID=" + EntityID + "");

                        //}
                        //tx.PersistenceManager.PlanningRepository.ExecuteQuery(DynamicQuery.ToString());


                        IList<DamMultiSelectValueDao> listMultiselect = new List<DamMultiSelectValueDao>();
                        var listOfOldValuesforFeed = (from item in tx.PersistenceManager.DamRepository.Query<DamMultiSelectValueDao>() where item.AttributeID == Convert.ToInt32(attributeid) && item.AssetID == Convert.ToInt32(AssetID) select item).ToList();
                        string query = "DELETE FROM MM_DAM_MultiSelectValue WHERE AssetID = ? AND AttributeID = ? ";
                        tx.PersistenceManager.DamRepository.ExecuteQuerywithMinParam(query.ToString(), Convert.ToInt32(AssetID), attributeid);
                        foreach (var element in listOfOldValuesforFeed)
                        {
                            obj.obj3.Add(element.OptionID);
                        }
                        foreach (var at in NewValue)
                        {
                            Marcom.Dal.DAM.Model.DamMultiSelectValueDao mt = new Marcom.Dal.DAM.Model.DamMultiSelectValueDao();
                            mt.AttributeID = attributeid;
                            mt.AssetID = Convert.ToInt32(AssetID);
                            mt.OptionID = Convert.ToInt32(at);
                            listMultiselect.Add(mt);
                        }
                        tx.PersistenceManager.DamRepository.Save<Marcom.Dal.DAM.Model.DamMultiSelectValueDao>(listMultiselect);
                    }
                    tx.PersistenceManager.DamRepository.ExecuteQuery("Update DAM_Asset set LinkedAssetID=0 where ID=" + AssetID);
                    BrandSystems.Marcom.Dal.DAM.Model.AssetsDao assetdaonew = new BrandSystems.Marcom.Dal.DAM.Model.AssetsDao();
                    assetdaonew.ID = AssetID;
                    assetdaonew = tx.PersistenceManager.DamRepository.Get<BrandSystems.Marcom.Dal.DAM.Model.AssetsDao>(assetdaonew.ID);
                    assetdaonew.UpdatedOn = DateTime.UtcNow;
                    tx.PersistenceManager.DamRepository.Save<BrandSystems.Marcom.Dal.DAM.Model.AssetsDao>(assetdaonew);
                    tx.Commit();

                    obj.action = "Asset metadata update";
                    obj.Actorid = proxy.MarcomManager.User.Id;
                    obj.AttributeId = Convert.ToInt32(attributeid);
                    obj.EntityId = assetTypeid.EntityID;
                    obj.AttributeDetails = new List<AttributeDao>();
                    obj.AttributeDetails.Add(attrdetails);
                    obj.Attributetypeid = AttributeTypeid;
                    obj.AssociatedEntityId = assetTypeid.ID;
                    obj.obj2 = NewValue;
                    if (assetTypeid.ActiveFileID > 0)
                    {
                        var FileObj = tx.PersistenceManager.DamRepository.Query<DAMFileDao>().ToList().Where(item => item.ID == assetTypeid.ActiveFileID).Select(item => item.VersionNo).Max();
                        obj.Version = FileObj;
                    }
                    else
                        obj.Version = 1;
                    fs.AsynchronousNotify(obj);
                }

                try
                {
                    BrandSystems.Marcom.Core.Interface.Managers.IEventManager _eventManager = null;
                    BrandSystems.Marcom.Core.Interface.Managers.IPluginManager _pluginManager = null;
                    MarcomManager marcommanager = new MarcomManager(_eventManager, _pluginManager);
                    BrandSystems.Marcom.Core.Managers.Proxy.PlanningManagerProxy pProxy = new BrandSystems.Marcom.Core.Managers.Proxy.PlanningManagerProxy(marcommanager);
                    System.Threading.Tasks.Task addtaskforsearch = new System.Threading.Tasks.Task(() => PlanningManager.Instance.UpdateEntityAsyncForDam(pProxy, AssetID, HttpUtility.HtmlEncode((string)NewValue[0]), "Attachments"));
                    addtaskforsearch.Start();
                }
                catch { }
                return true;

            }
            catch (MarcomAccessDeniedException ex)
            {
                throw ex;
            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// UpdateAssetAccess
        /// </summary>
        /// <param name="proxy"></param>
        /// <param name="AssetID"></param>
        /// <param name="AcessID"></param>
        /// <returns>success</returns>
        public bool UpdateAssetAccess(DigitalAssetManagerProxy proxy, int AssetID, string AcessID)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    var pervasset = tx.PersistenceManager.DamRepository.Get<AssetsDao>(AssetID);
                    string fromvalues = ((pervasset.AssetAccess == "" || pervasset.AssetAccess == null) ? "0" : pervasset.AssetAccess.Trim());
                    AssetsDao asset = new AssetsDao();
                    asset = tx.PersistenceManager.DamRepository.Get<AssetsDao>(AssetID);
                    asset.AssetAccess = AcessID;
                    asset.LinkedAssetID = 0;
                    asset.UpdatedOn = DateTime.UtcNow;
                    tx.PersistenceManager.DamRepository.Save<AssetsDao>(asset);
                    tx.Commit();

                    try
                    {
                        var afterasset = tx.PersistenceManager.DamRepository.Get<AssetsDao>(AssetID);
                        BrandSystems.Marcom.Core.Utility.FeedNotificationServer fs = new Utility.FeedNotificationServer();
                        NotificationFeedObjects obj = new NotificationFeedObjects();
                        obj.action = "Asset AccessChanged";
                        obj.Actorid = proxy.MarcomManager.User.Id;
                        obj.EntityId = pervasset.EntityID;
                        obj.FromValue = fromvalues;
                        obj.ToValue = afterasset.AssetAccess;
                        obj.AttributeName = HttpUtility.HtmlEncode(pervasset.Name);
                        obj.TypeName = "Asset Access Changed";
                        obj.CreatedOn = DateTimeOffset.Now;
                        obj.AssociatedEntityId = pervasset.ID;
                        if (afterasset.ActiveFileID > 0)
                        {
                            var FileObj = tx.PersistenceManager.DamRepository.Query<DAMFileDao>().ToList().Where(item => item.ID == afterasset.ActiveFileID).Select(item => item.VersionNo).Max();
                            obj.Version = FileObj;
                        }
                        else
                            obj.Version = 1;
                        fs.AsynchronousNotify(obj);

                        //BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("Updated the Feeds", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);

                    }
                    catch (Exception ex)
                    {
                        BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("error in feed" + " " + ex.Message, BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);
                    }
                }
                try
                {
                    BrandSystems.Marcom.Core.Interface.Managers.IEventManager _eventManager = null;
                    BrandSystems.Marcom.Core.Interface.Managers.IPluginManager _pluginManager = null;
                    MarcomManager marcommanager = new MarcomManager(_eventManager, _pluginManager);
                    BrandSystems.Marcom.Core.Managers.Proxy.PlanningManagerProxy pProxy = new BrandSystems.Marcom.Core.Managers.Proxy.PlanningManagerProxy(marcommanager);
                    System.Threading.Tasks.Task addtaskforsearch = new System.Threading.Tasks.Task(() => PlanningManager.Instance.UpdateEntityAsyncForDam(pProxy, AssetID, ""));
                    addtaskforsearch.Start();
                }
                catch { }
                return true;
            }
            catch { return false; }
        }


        public IList<IOption> GetOptionDetailListByAssetID(DigitalAssetManagerProxy proxy, int attributeID, int AssetID)
        {

            try
            {
                int version = MarcomManagerFactory.ActiveMetadataVersionNumber;
                string xmlpath = string.Empty;
                IList<IOption> _iioption = new List<IOption>();
                IList<OptionDao> dao = new List<OptionDao>();

                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    //xmlpath = tx.PersistenceManager.MetadataRepository.GetXmlPath(version);
                    //xmlpath = GetXmlWorkingPath();
                    //dao = tx.PersistenceManager.MetadataRepository.GetObject<OptionDao>(xmlpath);

                    //----------------------------------

                    var assetObj = (from item in tx.PersistenceManager.PlanningRepository.Query<AssetsDao>()
                                    where item.ID == AssetID
                                    select item).FirstOrDefault();

                    xmlpath = tx.PersistenceManager.MetadataRepository.GetXmlPath(version);
                    XDocument docx = XDocument.Load(xmlpath);
                    //xmlpath = tx.PersistenceManager.MetadataRepository.GetXmlPath(entityObj.Version);
                    dao = tx.PersistenceManager.MetadataRepository.GetObject<OptionDao>(xmlpath).OrderBy(a => a.SortOrder).ToList<OptionDao>();


                    //XDocument docx = XDocument.Load(xmlpath);
                    var rddd = (from EntityAttrRel in docx.Root.Elements("EntityTypeAttributeRelation_Table").Elements("EntityTypeAttributeRelation")
                                join Attr in docx.Root.Elements("Attribute_Table").Elements("Attribute") on Convert.ToInt32(EntityAttrRel.Element("AttributeID").Value) equals Convert.ToInt32(Attr.Element("ID").Value)
                                where Convert.ToInt32(EntityAttrRel.Element("EntityTypeID").Value) == assetObj.AssetTypeid && Convert.ToInt32(EntityAttrRel.Element("AttributeID").Value) == attributeID && Convert.ToInt32(EntityAttrRel.Element("ChooseFromParentOnly").Value) == 1
                                select new
                                {
                                    ID = Convert.ToInt16(Attr.Element("ID").Value),
                                    Caption = EntityAttrRel.Element("Caption").Value,
                                    AttributeTypeID = Convert.ToInt16(Attr.Element("AttributeTypeID").Value),
                                    Description = Attr.Element("Description").Value,
                                    IsSystemDefined = Convert.ToBoolean(Convert.ToInt32(Attr.Element("IsSystemDefined").Value)),
                                    IsSpecial = Convert.ToBoolean(Convert.ToInt32(Attr.Element("IsSpecial").Value))
                                }).ToList();
                    var attributesdetails1 = rddd;

                    //----------------------------------

                    //XDocument docx = XDocument.Load(xmlpath);
                    var attributesdetails = (from c in docx.Root.Elements("Attribute_Table").Elements("Attribute")
                                             where Convert.ToInt32(c.Element("ID").Value) == attributeID
                                             select new
                                             {
                                                 ID = Convert.ToInt16(c.Element("ID").Value),
                                                 Caption = c.Element("Caption").Value,
                                                 AttributeTypeID = Convert.ToInt16(c.Element("AttributeTypeID").Value),
                                                 Description = c.Element("Description").Value,
                                                 IsSystemDefined = Convert.ToBoolean(Convert.ToInt32(c.Element("IsSystemDefined").Value)),
                                                 IsSpecial = Convert.ToBoolean(Convert.ToInt32(c.Element("IsSpecial").Value))
                                             }).ToList();
                    foreach (var val in attributesdetails)
                    {

                        if (val.IsSpecial == true && val.AttributeTypeID == 3)
                        {
                            SystemDefinedAttributes systemType = (SystemDefinedAttributes)val.ID;

                            switch (systemType)
                            {
                                case SystemDefinedAttributes.Owner:

                                    IList<BrandSystems.Marcom.Core.User.Interface.IEntityUsers> _iientityusers = new List<BrandSystems.Marcom.Core.User.Interface.IEntityUsers>();
                                    IList<BrandSystems.Marcom.Core.User.Interface.IUser> _iuser = new List<BrandSystems.Marcom.Core.User.Interface.IUser>();
                                    IList<BrandSystems.Marcom.Dal.User.Model.UserDao> userDao = new List<BrandSystems.Marcom.Dal.User.Model.UserDao>();
                                    BrandSystems.Marcom.Core.User.Interface.IUser user = new BrandSystems.Marcom.Core.User.User();
                                    BrandSystems.Marcom.Dal.Access.Model.EntityRoleUserDao entityroleuser = new Dal.Access.Model.EntityRoleUserDao();
                                    IList<BrandSystems.Marcom.Dal.Access.Model.EntityRoleUserDao> roleusers = new List<BrandSystems.Marcom.Dal.Access.Model.EntityRoleUserDao>();
                                    var memberList = (from item in tx.PersistenceManager.UserRepository.Query<BrandSystems.Marcom.Dal.Access.Model.EntityRoleUserDao>() where item.Entityid == AssetID select item).ToList<BrandSystems.Marcom.Dal.Access.Model.EntityRoleUserDao>();
                                    var entitymembers = memberList.GroupBy(x => x.Userid).Select(x => x.FirstOrDefault()).ToList<BrandSystems.Marcom.Dal.Access.Model.EntityRoleUserDao>();
                                    IList<BrandSystems.Marcom.Dal.User.Model.UserDao> listmembers = new List<BrandSystems.Marcom.Dal.User.Model.UserDao>();
                                    for (int i = 0; i < entitymembers.Count(); i++)
                                    {
                                        BrandSystems.Marcom.Core.User.Interface.IEntityUsers entityuser = new BrandSystems.Marcom.Core.User.EntityUsers();

                                        BrandSystems.Marcom.Dal.User.Model.UserDao users = new Dal.User.Model.UserDao();
                                        users = tx.PersistenceManager.MetadataRepository.Get<BrandSystems.Marcom.Dal.User.Model.UserDao>(entitymembers.ElementAt(i).Userid);

                                        if (entitymembers.ElementAt(i).Roleid == 1)
                                        {
                                            entityuser.IsOwner = true;
                                        }
                                        else
                                            entityuser.IsOwner = false;
                                        if (users != null)
                                        {
                                            IOption _ioption = new Option();
                                            _ioption.Caption = users.FirstName + " " + users.LastName;
                                            _ioption.AttributeID = attributeID;
                                            _ioption.SortOrder = 0;
                                            _ioption.Id = users.Id;
                                            _iioption.Add(_ioption);
                                        }

                                    }

                                    break;
                            }

                        }
                        else if (val.IsSpecial == false && (val.AttributeTypeID == 3 || val.AttributeTypeID == 4))
                        {
                            if (val.AttributeTypeID == 4 && attributesdetails1.Count > 0)
                            {
                                IList<IAttributeData> entityAttrVal = new List<IAttributeData>();
                                entityAttrVal = proxy.MarcomManager.PlanningManager.GetEntityAttributesDetails(assetObj.EntityID);

                                IList<IAttributeData> entityAttrVal1 = new List<IAttributeData>();
                                entityAttrVal1 = entityAttrVal.Where(a => a.ID == attributeID).Select(a => a).ToList();

                                var tempcaption = (dynamic)entityAttrVal1.Where(a => a.ID == attributeID).Select(a => a.Caption).ToList();
                                string[] optioncaptionArr = tempcaption[0].Split(',');

                                for (int i = 0; i < optioncaptionArr.Length; i++)
                                {
                                    IOption _ioption = new Option();
                                    _ioption.Caption = optioncaptionArr[i].ToString();
                                    _ioption.AttributeID = attributeID;
                                    _ioption.SortOrder = 0;
                                    _ioption.Id = entityAttrVal1[0].Value[i];
                                    _iioption.Add(_ioption);
                                }

                            }
                            else if (val.AttributeTypeID == 3 && attributesdetails1.Count > 0)
                            {
                                IList<IAttributeData> entityAttrVal = new List<IAttributeData>();
                                entityAttrVal = proxy.MarcomManager.PlanningManager.GetEntityAttributesDetails(assetObj.EntityID);

                                IList<IAttributeData> entityAttrVal1 = new List<IAttributeData>();
                                entityAttrVal1 = entityAttrVal.Where(a => a.ID == attributeID).Select(a => a).ToList();

                                if (entityAttrVal1 != null)
                                {
                                    IOption _ioption = new Option();
                                    _ioption.Caption = entityAttrVal1[0].Caption[0];
                                    _ioption.AttributeID = attributeID;
                                    _ioption.SortOrder = 0;
                                    _ioption.Id = entityAttrVal1[0].Value;
                                    _iioption.Add(_ioption);
                                }
                            }
                            else
                            {
                                var optionresult = dao.Where(a => a.AttributeID == attributeID);
                                foreach (var item in optionresult)
                                {
                                    IOption _ioption = new Option();
                                    _ioption.Caption = item.Caption;
                                    _ioption.AttributeID = item.AttributeID;
                                    _ioption.SortOrder = item.SortOrder;
                                    _ioption.Id = item.Id;
                                    _iioption.Add(_ioption);
                                }
                            }
                        }
                    }

                }
                return _iioption;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// CheckPreviewGenerator
        /// </summary>
        /// <param name="proxy"></param>
        /// <param name="Assetids"></param>
        /// <returns>string of preview generated ids</returns>
        public string CheckPreviewGenerator(DigitalAssetManagerProxy proxy, string Assetids)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    string qrypreview = "select ID from DAM_File where ID in (" + Assetids + ") and Status=" + (int)AssetStatus.Done;
                    //string qrypreview = "select ID from DAM_File where ID in (" + Assetids + ")";
                    var previewgeneratedids = tx.PersistenceManager.DamRepository.ExecuteQuery(qrypreview).Cast<Hashtable>().ToList();
                    IList<object> obj = new List<object>();
                    foreach (var item in previewgeneratedids)
                    {
                        obj.Add(item["ID"].ToString());
                    }
                    string abc = string.Join(",", obj.Select(a => a));

                    return abc;
                }
            }
            catch
            {
                return "";
            }
        }
        /// <summary>
        /// PublishAssets
        /// </summary>
        /// <param name="proxy"></param>
        /// <param name="idArr"></param>
        /// <param name="Published"></param>
        /// <returns>true if ispublish value is updated</returns>
        public bool PublishAssets(DigitalAssetManagerProxy proxy, int[] idArr, bool IsPublished = true)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    IList<AssetsDao> iiasstDao = new List<AssetsDao>();
                    iiasstDao = tx.PersistenceManager.DamRepository.Query<AssetsDao>().Where(a => idArr.Contains(a.ID)).Select(a => a).ToList();
                    if (iiasstDao.Count > 0)
                    {

                        string assetIDinClause = "("
                              + String.Join(",", iiasstDao.Select(a => a.ID).ToArray().Select(x => x.ToString()).Distinct().ToArray())
                            + ")";
                        int[] assetTypeobj = iiasstDao.Select(a => a.AssetTypeid).Distinct().ToArray();

                        StringBuilder qry = new StringBuilder();

                        foreach (var val in assetTypeobj)
                        {
                            qry.AppendLine("Update DAM_Asset set IsPublish=" + Convert.ToInt32(IsPublished) + " where id IN " + assetIDinClause + "");
                        }
                        tx.PersistenceManager.DamRepository.ExecuteQuery(qry.ToString());
                        //tx.PersistenceManager.DamRepository.Delete<AssetsDao>(iiasstDao);
                    }

                    tx.Commit();
                    foreach (var valasset in iiasstDao)
                    {
                        BrandSystems.Marcom.Core.Utility.FeedNotificationServer fs = new Utility.FeedNotificationServer();
                        NotificationFeedObjects obj = new NotificationFeedObjects();
                        obj.action = "Publish Asset";
                        obj.Actorid = proxy.MarcomManager.User.Id;
                        obj.EntityId = valasset.EntityID;
                        obj.AttributeName = valasset.Name;
                        obj.TypeName = "Asset Publish";
                        obj.CreatedOn = DateTimeOffset.Now;
                        obj.AssociatedEntityId = valasset.ID;
                        if (valasset.ActiveFileID > 0)
                        {
                            var FileObj = tx.PersistenceManager.DamRepository.Query<DAMFileDao>().ToList().Where(item => item.ID == valasset.ActiveFileID).Select(item => item.VersionNo).Max();
                            obj.Version = FileObj;
                        }
                        else
                            obj.Version = 1;
                        fs.AsynchronousNotify(obj);
                    }
                    try
                    {
                        BrandSystems.Marcom.Core.Interface.Managers.IEventManager _eventManager = null;
                        BrandSystems.Marcom.Core.Interface.Managers.IPluginManager _pluginManager = null;
                        MarcomManager marcommanager = new MarcomManager(_eventManager, _pluginManager);
                        BrandSystems.Marcom.Core.Managers.Proxy.PlanningManagerProxy pProxy = new BrandSystems.Marcom.Core.Managers.Proxy.PlanningManagerProxy(marcommanager);
                        System.Threading.Tasks.Task addtaskforsearch = new System.Threading.Tasks.Task(() => PlanningManager.Instance.UpdateEntityAsyncForDam(pProxy, Convert.ToInt32(idArr[0]), ""));
                        addtaskforsearch.Start();
                    }

                    catch { }

                    return true;
                }
            }
            catch
            {
                return false;
            }

        }

        public int CreateBlankAsset(DigitalAssetManagerProxy proxy, int FolderID, int TypeId, string Name, IList<IAttributeData> listattributevalues, int EntityID, int Category, string url = null, bool IsforDuplicate = false, int LinkedAssetID = 0, string strAssetAccess = null, int SourceAssetID = 0, int assetactioncode = 0, bool blnAttach = false)
        {
            int AssetID = 0;
            int OwnerID = 0;
            try
            {
                OwnerID = Convert.ToInt32(proxy.MarcomManager.User.Id);

                BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("-----------------------------------------------------------------------------------------------", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);
                BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("Started creating Asset", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);


                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    AssetsDao assetdao = new AssetsDao();
                    assetdao.CreatedBy = OwnerID;
                    assetdao.Name = HttpUtility.HtmlEncode(Name);
                    assetdao.FolderID = FolderID;
                    assetdao.EntityID = EntityID;
                    assetdao.Createdon = DateTime.UtcNow;
                    assetdao.UpdatedOn = DateTime.UtcNow;
                    assetdao.ActiveFileID = 0;
                    assetdao.AssetTypeid = TypeId;
                    assetdao.Category = Category;
                    if (Category == 2)
                    {
                        assetdao.Url = url;
                    }
                    assetdao.LinkedAssetID = LinkedAssetID;
                    assetdao.AssetAccess = strAssetAccess;
                    tx.PersistenceManager.PlanningRepository.Save<AssetsDao>(assetdao);

                    if (listattributevalues != null)
                    {
                        var result = InsertAssetAttributes(tx, listattributevalues, assetdao.ID, TypeId, SourceAssetID, IsforDuplicate);
                    }
                    BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("Asset is saved in DAM_Asset with assetId : " + AssetID, BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);
                    tx.Commit();
                    AssetID = assetdao.ID;
                }
                try
                {
                    if (FolderID == 0 && blnAttach == true)
                    {
                        BrandSystems.Marcom.Core.Utility.FeedNotificationServer fs1 = new Utility.FeedNotificationServer();
                        NotificationFeedObjects obj1 = new NotificationFeedObjects();
                        obj1.action = "Attach Asset";
                        obj1.Actorid = proxy.MarcomManager.User.Id;
                        obj1.EntityId = EntityID;
                        obj1.AttributeName = HttpUtility.HtmlEncode(Name);
                        obj1.TypeName = "Attach Asset";
                        obj1.CreatedOn = DateTimeOffset.Now;
                        obj1.AssociatedEntityId = AssetID;
                        obj1.Version = 1;
                        fs1.AsynchronousNotify(obj1);

                    }
                    else if (FolderID == 0 && assetactioncode == 1)
                    {
                        BrandSystems.Marcom.Core.Utility.FeedNotificationServer fs1 = new Utility.FeedNotificationServer();
                        NotificationFeedObjects obj1 = new NotificationFeedObjects();
                        obj1.action = "Moved Asset";
                        obj1.Actorid = proxy.MarcomManager.User.Id;
                        obj1.EntityId = EntityID;
                        obj1.AttributeName = HttpUtility.HtmlEncode(Name);
                        obj1.TypeName = "Moved Asset";
                        obj1.CreatedOn = DateTimeOffset.Now;
                        obj1.AssociatedEntityId = AssetID;
                        obj1.Version = 1;
                        fs1.AsynchronousNotify(obj1);

                    }
                    else if (FolderID == 0 && assetactioncode == 2)
                    {
                        BrandSystems.Marcom.Core.Utility.FeedNotificationServer fs1 = new Utility.FeedNotificationServer();
                        NotificationFeedObjects obj1 = new NotificationFeedObjects();
                        obj1.action = "Copy Asset";
                        obj1.Actorid = proxy.MarcomManager.User.Id;
                        obj1.EntityId = EntityID;
                        obj1.AttributeName = HttpUtility.HtmlEncode(Name);
                        obj1.TypeName = "Copy Asset";
                        obj1.CreatedOn = DateTimeOffset.Now;
                        obj1.AssociatedEntityId = AssetID;
                        obj1.Version = 1;
                        fs1.AsynchronousNotify(obj1);

                    }
                    else
                    {
                        BrandSystems.Marcom.Core.Utility.FeedNotificationServer fs = new Utility.FeedNotificationServer();
                        NotificationFeedObjects obj = new NotificationFeedObjects();
                        obj.action = "create Asset";
                        obj.Actorid = proxy.MarcomManager.User.Id;
                        obj.EntityId = EntityID;
                        obj.AttributeName = HttpUtility.HtmlEncode(Name);
                        obj.TypeName = "Asset creation";
                        obj.CreatedOn = DateTimeOffset.Now;
                        obj.AssociatedEntityId = AssetID;
                        obj.Version = 1;
                        fs.AsynchronousNotify(obj);
                    }
                    BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("Updated the Feeds", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);

                }
                catch (Exception ex)
                {
                    BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("error in feed" + " " + ex.Message, BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);
                }
                if (!IsforDuplicate && FolderID == 0)
                {
                    ReinitializeTask(proxy, EntityID);
                }
                return AssetID;
            }
            catch (MarcomAccessDeniedException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                BrandSystems.Marcom.Core.Metadata.LogHandler.LogError("Failed to create Asset", ex);
                BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("-----------------------------------------------------------------------------------------------", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);
                return 0;
            }
        }

        /// <summary>
        /// Send Mail
        /// </summary>
        /// <param name="proxy"></param>
        /// <param name="idarr"></param>
        /// <returns>true if mail sent</returns>
        public bool SendMail(DigitalAssetManagerProxy proxy, int[] idarr, string toAddress, string subject, List<KeyValuePair<string, int>> CroppedList = null)
        {
            SmtpClient objSMTP = new SmtpClient();
            MailMessage _email = new MailMessage();
            StringBuilder body = new StringBuilder();
            string ToMail = toAddress;
            _email.From = new System.Net.Mail.MailAddress(System.Configuration.ConfigurationManager.AppSettings["Email"]);
            _email.IsBodyHtml = true;
            _email.To.Add(ToMail);

            _email.From = new System.Net.Mail.MailAddress(System.Configuration.ConfigurationManager.AppSettings["Email"]);
            _email.IsBodyHtml = true;
            _email.To.Add(ToMail);
            _email.Subject = subject;


            string xmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
            XDocument adminXmlDoc = XDocument.Load(xmlpath);
            //The Key is root node current Settings
            string xelementName = "ApplicationURL";
            var xelementFilepath = XElement.Load(xmlpath);
            var xmlElement = xelementFilepath.Element(xelementName);
            string str = xmlElement.Value;

            string directoryPath = AppDomain.CurrentDomain.BaseDirectory;
            string newguid = Guid.NewGuid().ToString();
            string DamDir = directoryPath + "DAMFiles\\Original";
            string sDir = directoryPath + "DAMDownloads";
            string directoryDestPath = directoryPath + "DAMDownloads";

            IList<DAMFileDao> files = new List<DAMFileDao>();
            int[] assetactivefiles = { };
            string fileguid = "";


            if (idarr.Length > 0)
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    assetactivefiles = tx.PersistenceManager.DamRepository.Query<AssetsDao>().Where(a => idarr.Contains(a.ID)).Select(a => a.ActiveFileID).ToArray();
                    files = tx.PersistenceManager.DamRepository.Query<DAMFileDao>().Where(a => assetactivefiles.Contains(a.ID)).Select(a => a).ToList();
                    tx.Commit();
                }
                if (idarr.Length > 1)
                    fileguid = DownloadDamFiles(proxy, idarr, 1, CroppedList);
                else
                {
                    fileguid = files[0].FileGuid.ToString();

                }
            }

            string[] filetypes = { ".jpg", ".jpeg", ".png", ".psd", ".bmp", ".tif", ".tiff", ".gif" };

            string html = "";
            html += "<html dir=\"ltr\">";
            html += "  <head>";
            html += "    <style type=\"text/css\" id=\"owaParaStyle\"></style>";
            html += "    <style type=\"text/css\" id=\"owaTempEditStyle\"></style>";
            html += "  </head>";
            html += "  <body fpstyle=\"1\" ocsi=\"1\">";
            html += "    <table width=\"100%\">";
            html += "      <tbody>";
            html += "        <tr>";
            html += "          <td class=\"bodytext\" colspan=\"3\">This file is sent to you from the Marcom Media Bank by: " + proxy.MarcomManager.User.FirstName + " " + proxy.MarcomManager.User.LastName + "</td>";
            html += "        </tr>";
            html += "        <tr>";
            if (idarr.Length > 1)
                html += "          <td><a href=\"" + str + "DAMDownload.aspx?FileID=" + fileguid.ToString() + "&FileFriendlyName=" + fileguid + "&Ext=.zip&token=" + System.DateTime.Now.Hour.ToString("dd-MM-yyyy") + "\" target=\"_blank\">Mediabank_" + System.DateTime.Now.ToString("yyyyMMdd") + ".zip</a></td>";
            else
                html += "          <td><a href=\"" + str + "DAMDownload.aspx?FileID=" + fileguid.ToString() + "&FileFriendlyName=" + files[0].Name.ToString() + "&Ext=" + files[0].Extension + "&token=" + System.DateTime.Now.Hour.ToString("dd-MM-yyyy") + "\" target=\"_blank\">" + files[0].Name + "</a></td>";
            html += "        </tr>";
            html += "        <tr>";
            html += "          <td class=\"bodytext\">This file will be available for download for 5 days.</td>";
            html += "        </tr>";
            html += "        <tr>";
            html += "          <td class=\"bodytext\"></td>";
            html += "        </tr>";
            html += "        <tr>";
            html += "          <td class=\"boldGrey\">Message from sender:</td>";
            html += "        </tr>";
            html += "        <tr>";
            html += "          <td class=\"bodytext\"></td>";
            html += "        </tr>";
            html += "        <tr>";
            html += "          <td>&nbsp;</td>";
            html += "        </tr>";
            html += "        <tr>";
            html += "          <td class=\"bodytext\" bgcolor=\"#C0C0C0\">Number of files:&nbsp; " + idarr.Length.ToString() + "</td>";
            html += "        </tr>";
            html += "        <tr>";
            html += "          <td>";
            html += "            <br>";
            if (idarr.Length > 0)
            {
                foreach (var file in files)
                {
                    string ext = file.Extension.ToLower();
                    html += "            <table cellspacing=\"0\" cellpadding=\"0\" style=''>";
                    html += "              <tbody>";
                    html += "                <tr>";
                    html += "                  <td width=\"\"></td>";
                    html += "                  <td class=\"divOuterFloat_Largethumba\"></td>";
                    html += "                  <td class=\"divInnerFloatGrey_Largethumb\">";
                    html += "                    <div class=\"imgFloat\" style=\"width:1px; height:56\"></div>";
                    if (filetypes.Contains(ext))
                        html += "                    <div><img  src=\"" + str + "StreamingImageForMail.aspx?FileID=" + file.FileGuid.ToString() + "&FileFriendlyName=" + file.Name + "&Ext=" + file.Extension + "&token=" + System.DateTime.Now.Hour.ToString("yyyyMMdd") + "\"></div>";
                    else
                        html += "                    <div><img style=\"height:100px;\" src=\"" + str + "StreamingImageForMail.aspx?FileID=" + file.FileGuid.ToString() + "&FileFriendlyName=" + file.Name + "&Ext=" + file.Extension + "&token=" + System.DateTime.Now.Hour.ToString("yyyyMMdd") + "\"></div>";
                    html += "                    <div class=\"imgFloat\" style=\"width:1px; height:56\"></div>";
                    html += "                  </td>";
                    html += "                  <td width=\"20\">&nbsp;</td>";
                    html += "                  <td width=\"200\"><font size=\"2\"><span class=\"boldgrey\"></span></font><font size=\"2\"><b><span class=\"bodytext\">" + file.Name.Substring(0, file.Name.LastIndexOf(".")) + "" + file.Extension + "</span></b><br>";
                    html += "                    <font size=\"2\"><span class=\"boldgrey\"><br>";
                    html += "                    </span></font><span class=\"bodytext\"><span></span></span></font><br>";
                    html += "                    <font size=\"2\"><span class=\"boldgrey\"></span></font><font size=\"2\"><span class=\"bodytext\">";
                    html += "                    </span></font><br>";
                    html += "                    <font size=\"2\"><span class=\"boldgrey\"></span></font><font size=\"2\"><span class=\"bodytext\"></span></font>";
                    html += "                  </td>";
                    html += "                </tr>";
                    html += "                <tr>";
                    html += "                  <td colspan=\"3\"></td>";
                    html += "                  <td></td>";
                    html += "                </tr>";
                    html += "                <tr></tr>";
                    html += "              </tbody>";
                    html += "            </table>";
                    html += "            <table border=\"0\" cellspacing=\"0\" cellpadding=\"3\" width=\"100%\">";
                    html += "              <tbody>";
                    html += "                <tr>";
                    html += "                  <td>";
                    html += "                    <br>";
                    html += "                    <hr color=\"#808080\" size=\"1\">";
                    html += "                  </td>";
                    html += "                </tr>";
                    html += "              </tbody>";
                    html += "            </table>";
                }
            }
            html += "            <br>";
            html += "          </td>";
            html += "        </tr>";
            html += "      </tbody>";
            html += "    </table>";
            html += "  </body>";
            html += "</html>";

            _email.Body = html.ToString();

            objSMTP.Send(_email);
            if (idarr.Length > 0)
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    IList<AssetsDao> iiasstDao = new List<AssetsDao>();
                    iiasstDao = tx.PersistenceManager.DamRepository.Query<AssetsDao>().Where(a => idarr.Contains(a.ID)).Select(a => a).ToList();

                    if (iiasstDao.Count > 0)
                    {
                        foreach (var valasset in iiasstDao)
                        {
                            BrandSystems.Marcom.Core.Utility.FeedNotificationServer fs = new Utility.FeedNotificationServer();
                            NotificationFeedObjects obj = new NotificationFeedObjects();
                            obj.action = "Send Asset";
                            obj.Actorid = proxy.MarcomManager.User.Id;
                            obj.EntityId = valasset.EntityID;
                            obj.AttributeName = valasset.Name;
                            obj.TypeName = "Asset Send";
                            obj.CreatedOn = DateTimeOffset.Now;
                            obj.AssociatedEntityId = valasset.ID;
                            if (valasset.ActiveFileID > 0)
                            {
                                var FileObj = tx.PersistenceManager.DamRepository.Query<DAMFileDao>().ToList().Where(item => item.ID == valasset.ActiveFileID).Select(item => item.VersionNo).Max();
                                obj.Version = FileObj;
                            }
                            else
                                obj.Version = 1;
                            fs.AsynchronousNotify(obj);
                        }
                    }
                    tx.Commit();
                }


            }


            return true;
        }

        /// <summary>
        /// MoveFilesToUploadImagesToCreateTask
        /// </summary>used to move the files from dam files to uploadimages folder to create task
        /// <param name="proxy"></param>
        /// <param name="TaskFiles"></param>
        /// <returns>true if success</returns>
        public Tuple<IList<BrandSystems.Marcom.Core.Common.Interface.IFile>, int> MoveFilesToUploadImagesToCreateTask(DigitalAssetManagerProxy proxy, IList<BrandSystems.Marcom.Core.Common.Interface.IFile> TaskFiles, int ParentEntityID)
        {
            try
            {
                IList<BrandSystems.Marcom.Core.Common.Interface.IFile> ifile = new List<BrandSystems.Marcom.Core.Common.Interface.IFile>();
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {

                    if (TaskFiles != null)
                    {
                        foreach (var a in TaskFiles)
                        {
                            BrandSystems.Marcom.Core.Common.Interface.IFile file = new BrandSystems.Marcom.Core.Common.File();
                            Guid NewId = Guid.NewGuid();
                            string filePath = ReadAdminXML("FileManagment");
                            var DirInfo = System.IO.Directory.GetParent(filePath);
                            string newFilePath = DirInfo.FullName;
                            string uploadImagePath = Path.Combine(HttpRuntime.AppDomainAppPath);
                            uploadImagePath = uploadImagePath + "UploadedImages\\";
                            if (a.IsExist)
                                System.IO.File.Copy(filePath + "\\" + a.strFileID.ToString() + a.Extension, uploadImagePath + "\\" + NewId + a.Extension);
                            else
                                System.IO.File.Move(filePath + "\\" + a.strFileID.ToString() + a.Extension, uploadImagePath + "\\" + NewId + a.Extension);

                            file.ActiveFileVersionID = a.ActiveFileVersionID;
                            file.Checksum = a.Checksum;
                            file.CreatedOn = a.CreatedOn;
                            file.Description = a.Description;
                            file.Extension = a.Extension;
                            file.Fileguid = NewId;
                            file.IsExist = a.IsExist;
                            file.LinkURL = a.LinkURL;
                            file.MimeType = a.MimeType;
                            file.Moduleid = a.Moduleid;
                            file.Name = a.Name;
                            file.Ownerid = a.Ownerid;
                            file.OwnerName = a.OwnerName;
                            file.Size = a.Size;
                            file.StrCreatedDate = a.StrCreatedDate;
                            file.strFileID = a.strFileID;
                            file.VersioningFileId = a.VersioningFileId;
                            file.VersionNo = a.VersionNo;
                            ifile.Add(file);
                        }
                        return Tuple.Create(ifile, 0);
                    }
                }
                return null;
            }
            catch { return null; }

        }


        public bool DeleteAttachmentVersionByAssetID(DigitalAssetManagerProxy proxy, int fileid)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    DAMFileDao damfldao = new DAMFileDao();
                    damfldao = tx.PersistenceManager.DamRepository.Get<DAMFileDao>(DAMFileDao.MappingNames.ID, fileid);
                    string filename = damfldao.Name;
                    int VersionNo = 1;
                    VersionNo = damfldao.VersionNo;
                    var asset = tx.PersistenceManager.DamRepository.Get<BrandSystems.Marcom.Dal.DAM.Model.AssetsDao>(damfldao.AssetID);
                    string flPath = ReadAdminXML("FileManagment");//HttpContext.Current.Server.MapPath("~/documents/" + fldao.Fileguid + fldao.Extension);
                    var DirInfo = System.IO.Directory.GetParent(flPath);
                    string newFilePath = DirInfo.FullName + "\\" + damfldao.FileGuid + damfldao.Extension;
                    if (System.IO.File.Exists(newFilePath))
                    {
                        System.IO.File.Delete(newFilePath);
                        tx.PersistenceManager.DamRepository.DeleteByID<DAMFileDao>(DAMFileDao.MappingNames.ID, fileid);
                        tx.Commit();
                        try
                        {

                            BrandSystems.Marcom.Core.Utility.FeedNotificationServer fs = new Utility.FeedNotificationServer();
                            NotificationFeedObjects obj = new NotificationFeedObjects();
                            obj.action = "Delete AssetVersion";
                            obj.Actorid = proxy.MarcomManager.User.Id;
                            obj.EntityId = asset.EntityID;
                            obj.ToValue = fileid.ToString();
                            obj.AttributeName = HttpUtility.HtmlEncode(filename);
                            obj.TypeName = "Delete Asset Version";
                            obj.CreatedOn = DateTimeOffset.Now;
                            obj.AssociatedEntityId = asset.ID;
                            obj.Version = VersionNo;
                            fs.AsynchronousNotify(obj);

                            //BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("Updated the Feeds", BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);

                        }
                        catch (Exception ex)
                        {
                            BrandSystems.Marcom.Core.Metadata.LogHandler.LogInfo("error in feed" + " " + ex.Message, BrandSystems.Marcom.Core.Metadata.LogHandler.LogType.General);
                        }

                        return true;
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// UpdateAssetAccessSettings to update asset access based on role
        /// </summary>
        /// <param name="proxy"></param>
        /// <param name="RoleID"></param>
        /// <param name="IsChecked"></param>
        /// <returns>true or false</returns>
        public bool UpdateAssetAccessSettings(DigitalAssetManagerProxy proxy, int RoleID, bool IsChecked)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    GlobalRoleDao globalrole = new GlobalRoleDao();
                    globalrole = tx.PersistenceManager.DamRepository.Get<GlobalRoleDao>(RoleID);
                    globalrole.IsAssetAccess = IsChecked;
                    tx.PersistenceManager.DamRepository.Save<GlobalRoleDao>(globalrole);
                    tx.Commit();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }



        public bool UpdateAssetDetails(DigitalAssetManagerProxy proxy, int assetId, string assetName)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    string oldassetname = "";
                    string temp_oldassetname = "";
                    AssetsDao assetdao = new AssetsDao();
                    assetdao = tx.PersistenceManager.DamRepository.Get<AssetsDao>(assetId);
                    temp_oldassetname = assetdao.Name;
                    oldassetname = temp_oldassetname;
                    assetdao.Name = HttpUtility.HtmlEncode(assetName);
                    assetdao.LinkedAssetID = 0;
                    assetdao.UpdatedOn = DateTime.UtcNow;
                    if (oldassetname != assetName && assetName != "")
                    {
                        BrandSystems.Marcom.Core.Utility.FeedNotificationServer fs = new Utility.FeedNotificationServer();
                        NotificationFeedObjects obj = new NotificationFeedObjects();
                        obj.action = "Asset metadata update";
                        obj.Actorid = proxy.MarcomManager.User.Id;
                        obj.AttributeId = -1;
                        obj.EntityId = assetdao.EntityID;
                        //obj.AttributeDetails = new List<AttributeDao>();
                        //obj.AttributeDetails.Add(attrdetails);
                        obj.Attributetypeid = -1;
                        obj.AssociatedEntityId = assetdao.ID;
                        obj.FromValue = HttpUtility.HtmlEncode(oldassetname);
                        obj.ToValue = HttpUtility.HtmlEncode(assetName);
                        obj.AttributeName = "AssetName";
                        //obj.obj2 = assetName;
                        if (assetdao.ActiveFileID > 0)
                        {
                            var FileObj = tx.PersistenceManager.DamRepository.Query<DAMFileDao>().ToList().Where(item => item.ID == assetdao.ActiveFileID).Select(item => item.VersionNo).Max();
                            obj.Version = FileObj;
                        }
                        else
                            obj.Version = 1;
                        fs.AsynchronousNotify(obj);
                    }
                    //assetdao.ID = assetId;
                    tx.PersistenceManager.DamRepository.Save<AssetsDao>(assetdao);
                    tx.Commit();

                    try
                    {
                        BrandSystems.Marcom.Core.Interface.Managers.IEventManager _eventManager = null;
                        BrandSystems.Marcom.Core.Interface.Managers.IPluginManager _pluginManager = null;
                        MarcomManager marcommanager = new MarcomManager(_eventManager, _pluginManager);
                        BrandSystems.Marcom.Core.Managers.Proxy.PlanningManagerProxy pProxy = new BrandSystems.Marcom.Core.Managers.Proxy.PlanningManagerProxy(marcommanager);
                        System.Threading.Tasks.Task updatesearchforassetname = new System.Threading.Tasks.Task(() => PlanningManager.Instance.UpdateEntityAsyncForDam(pProxy, assetId, assetName, "Attachments"));
                        updatesearchforassetname.Start();
                    }
                    catch { }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }



        public IList<DAMFileDao> GetAssetActiveFileinfo(DigitalAssetManagerProxy proxy, int assetId)
        {
            try
            {

                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    IList<DAMFileDao> filesinfo = new List<DAMFileDao>();
                    var assetactivefiles = tx.PersistenceManager.DamRepository.Query<AssetsDao>().Where(a => a.ID == assetId).Select(a => a).FirstOrDefault();
                    filesinfo = tx.PersistenceManager.DamRepository.Query<DAMFileDao>().Where(a => a.ID == assetactivefiles.ActiveFileID).Select(a => a).ToList();
                    tx.Commit();
                    return filesinfo;
                }

            }
            catch (Exception)
            {
                return null;
            }
        }

        public IList<DAMFiledownloadinfo> CropRescale(DigitalAssetManagerProxy proxy, int assetId, int TopLeft, int TopRight, int bottomLeft, int bottomRight, int CropFormat, int ScaleWidth, int ScaleHeight, int Dpi, int profileid, string fileformate)
        {
            try
            {
                string directoryPath = AppDomain.CurrentDomain.BaseDirectory;
                //string newguid = Guid.NewGuid().ToString();
                string sDir = directoryPath + "DAMDownloads";
                CleanSrcFolder(sDir);  //Clear previous files from the source file
                BrandSystems.Marcom.Core.Utility.MediaHandler.MediaHandler MH = new BrandSystems.Marcom.Core.Utility.MediaHandler.MediaHandler();
                if (MH.MHClientReady())
                {
                    bool status = false;
                    BrandSystems.Marcom.Core.Utility.MediaHandler.ItemRequest obItemRequest = new BrandSystems.Marcom.Core.Utility.MediaHandler.ItemRequest();
                    obItemRequest.Unit = BrandSystems.Marcom.Core.Utility.MediaHandler.ItemRequest.Units.Pixels;
                    obItemRequest.CropHeight = Convert.ToDouble(bottomRight) - Convert.ToDouble(TopRight);
                    obItemRequest.CropWidth = Convert.ToDouble(bottomLeft) - Convert.ToDouble(TopLeft);
                    obItemRequest.CropTop = Convert.ToDouble(TopRight);
                    obItemRequest.CropLeft = Convert.ToDouble(TopLeft);
                    obItemRequest.DimensionRefId = Convert.ToInt32(profileid == 0 ? 0 : profileid);
                    obItemRequest.Format = Convert.ToString(fileformate == null ? "" : fileformate.ToString());
                    obItemRequest.XResolutionDPI = Convert.ToInt32(Dpi == 0 ? 0 : Dpi);
                    obItemRequest.ScaleHeight = Convert.ToInt32(ScaleHeight == 0 ? 0 : ScaleHeight);
                    obItemRequest.ScaleWidth = Convert.ToInt32(ScaleWidth == 0 ? 0 : ScaleWidth);
                    obItemRequest.CornerColor = "white";
                    //if (profileid > 0)
                    //{
                    //    using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                    //    {
                    //        var GetFileProfiles = tx.PersistenceManager.CommonRepository.Query<BrandSystems.Marcom.Dal.DAM.Model.DamFileProfileDao>().Where(a => a.ID == profileid).Select(a => a).ToList();
                    //        obItemRequest.ScaleHeight = Convert.ToInt32(GetFileProfiles[0].Height == 0 ? 0 : GetFileProfiles[0].Height); 
                    //        obItemRequest.ScaleWidth = Convert.ToInt32(GetFileProfiles[0].Width == 0 ? 0 : GetFileProfiles[0].Width);
                    //        obItemRequest.XResolutionDPI = Convert.ToInt32(GetFileProfiles[0].DPI == 0 ? 0 : GetFileProfiles[0].DPI);
                    //        obItemRequest.Format = Convert.ToString(GetFileProfiles[0].Extension == null ? "" : GetFileProfiles[0].Extension);
                    //    }
                    //}
                    string[] itemInfo = new string[7];
                    string errorinfo = "";
                    string sessinon = "";
                    string m_strServername = ConfigurationManager.AppSettings["ServerName"];

                    using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                    {
                        IList<DAMFileDao> filesinfo = new List<DAMFileDao>();

                        var assetactivefiles = tx.PersistenceManager.DamRepository.Query<AssetsDao>().Where(a => a.ID == assetId).Select(a => a).FirstOrDefault();
                        filesinfo = tx.PersistenceManager.DamRepository.Query<DAMFileDao>().Where(a => a.ID == assetactivefiles.ActiveFileID).Select(a => a).ToList();
                        string applicationPath = AppDomain.CurrentDomain.BaseDirectory;
                        //string applicationPath = ConfigurationManager.AppSettings["OriginalXMLpath"];
                        //applicationPath = applicationPath.Replace(":", "$");
                        string strIPAddress = ConfigurationManager.AppSettings["IPAddress"].ToString();
                        string soureFolder = "DAMFiles\\Original\\";
                        string newguid = Guid.NewGuid().ToString();
                        string DestinationFolder = "DAMDownloads\\" + newguid.ToString() + "\\";
                        string strExtension = filesinfo[0].Extension;
                        strExtension = strExtension.Substring(1, strExtension.Length - 1).ToUpper();
                        //string sourcefile="\\\\" + strIPAddress + "\\" + applicationPath + soureFolder +filesinfo[0].FileGuid +filesinfo[0].Extension;
                        //string DestinationPath = "\\\\" + strIPAddress + "\\" + applicationPath + DestinationFolder + filesinfo[0].Name;
                        string sourcefile = applicationPath + soureFolder + filesinfo[0].FileGuid + filesinfo[0].Extension;
                        string DestinationPath = applicationPath + DestinationFolder + filesinfo[0].Name;
                        status = MH.Rescale(obItemRequest, sourcefile, DestinationPath, ref itemInfo, ref errorinfo, sessinon);
                        // test1.Rescale(obItemRequest , "e:\\test\\test1.jpg", "e:\\test1\\testnew11.jpg", ref itemInfo, ref errorinfo, sessinon);
                        IList<DAMFiledownloadinfo> filesdowninfo = new List<DAMFiledownloadinfo>();
                        DAMFiledownloadinfo damdowninfo = new DAMFiledownloadinfo();
                        if (status)
                        {
                            damdowninfo.Width = Convert.ToDouble(itemInfo[0].ToString() == "" ? 0 : Convert.ToDouble(itemInfo[0].ToString()));
                            damdowninfo.Height = Convert.ToDouble(itemInfo[1].ToString() == "" ? 0 : Convert.ToDouble(itemInfo[1].ToString()));
                            damdowninfo.XDpi = Convert.ToDouble(itemInfo[2].ToString() == "" ? 0 : Convert.ToDouble(itemInfo[2].ToString()));
                            damdowninfo.YDpi = Convert.ToDouble(itemInfo[3].ToString() == "" ? 0 : Convert.ToDouble(itemInfo[3].ToString()));
                            damdowninfo.FileFomat = (itemInfo[4].ToString() == "" ? "-" : itemInfo[4].ToString());
                            damdowninfo.FileName = (itemInfo[5].ToString() == "" ? "-" : itemInfo[5].ToString());
                            damdowninfo.FilePath = newguid.ToString() + "/" + itemInfo[5].ToString();

                            BrandSystems.Marcom.Core.Utility.FeedNotificationServer fs = new Utility.FeedNotificationServer();
                            NotificationFeedObjects obj = new NotificationFeedObjects();
                            obj.action = "Download Asset";
                            obj.Actorid = proxy.MarcomManager.User.Id;
                            obj.EntityId = assetactivefiles.EntityID;
                            obj.AttributeName = assetactivefiles.Name;
                            obj.TypeName = "Asset Download";
                            obj.CreatedOn = DateTimeOffset.Now;
                            obj.AssociatedEntityId = assetId;
                            obj.Version = filesinfo[0].VersionNo;
                            fs.AsynchronousNotify(obj);


                        }
                        else
                        {
                            damdowninfo.Width = 0;
                            damdowninfo.Height = 0;
                            damdowninfo.XDpi = 0;
                            damdowninfo.YDpi = 0;
                            damdowninfo.FileFomat = "-";
                            damdowninfo.FileName = "-";
                            damdowninfo.FilePath = "-";
                        }
                        damdowninfo.Errorinfo = (errorinfo.ToString() == "" ? "-" : errorinfo.ToString());
                        damdowninfo.Status = status;

                        filesdowninfo.Add(damdowninfo);
                        tx.Commit();
                        return filesdowninfo;
                    }

                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public IList GetMimeType(DigitalAssetManagerProxy proxy)
        {
            try
            {
                IList filesinfo = new List<string>();
                string xmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "MediaHandlerSettings.xml");
                XDocument MHXmlDoc = XDocument.Load(xmlpath);
                //var result = MHXmlDoc.Descendants("MHSetting").Descendants("CropFileFormat").Descendants("File").Select(a => a).FirstOrDefault();
                var Croplist = MHXmlDoc.Descendants("MHSetting").Descendants("CropFileFormat").Descendants("Ext").Select(a => a.Value).ToList();

                filesinfo = Croplist.ToList();
                //if (abc.Count > 0)
                //{
                //    jsonText = JsonConvert.SerializeObject(abc);
                //}
                //filesinfo.Add(jsonText);
                //filesinfo = jsonText.ToList();
                //filesinfo = result.ToList();
                //BrandSystems.Marcom.Core.Utility.MediaHandler.MediaHandler MH = new BrandSystems.Marcom.Core.Utility.MediaHandler.MediaHandler();
                //IList filesinfo = new List<string>();
                //string errorinfo = "";
                //filesinfo = MH.GetMimeTypes(ref errorinfo).ToList();
                return filesinfo;
            }
            catch (Exception)
            {
                return null;
            }

        }


        public IList<IDamFileProfile> GetProfileFiles(DigitalAssetManagerProxy proxy)
        {
            try
            {
                IList<IDamFileProfile> objDamfileprofile = new List<IDamFileProfile>();
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    var GetFileProfiles = (from tt in tx.PersistenceManager.CommonRepository.Query<BrandSystems.Marcom.Dal.DAM.Model.DamFileProfileDao>() select tt).ToList();
                    foreach (var Profile in GetFileProfiles)
                    {
                        DamFileProfile dm = new DamFileProfile();
                        dm.ID = Profile.ID;
                        dm.Name = Profile.Name;
                        dm.Height = Profile.Height;
                        dm.Width = Profile.Width;
                        dm.Extension = Profile.Extension;
                        dm.DPI = Profile.DPI;
                        string AssetAccess = Profile.IsAssetAccess;
                        if (AssetAccess != "0")
                        {
                            int[] arrayroles = AssetAccess.Split(',').Select(str => int.Parse(str)).ToArray();
                            List<string> strValues = new List<string>();
                            foreach (int value in arrayroles)
                            {
                                var tempcaption = tx.PersistenceManager.PlanningRepository.Query<GlobalRoleDao>().Where(c => c.Id == value && c.IsAssetAccess == true).SingleOrDefault();
                                if (tempcaption != null)
                                {
                                    strValues.Add(tempcaption.Caption);
                                }
                            }
                            dm.IsAssetAccess = string.Join(",", strValues.ToArray());
                        }
                        else
                        {
                            dm.IsAssetAccess = "";
                        }
                        objDamfileprofile.Add(dm);
                    }
                    return objDamfileprofile;
                }
            }
            catch
            {

            }
            return null;
        }

        public IList<IDamFileProfile> GetProfileFilesByUser(DigitalAssetManagerProxy proxy, int UserId)
        {
            try
            {
                IList<IDamFileProfile> objDamfileprofile = new List<IDamFileProfile>();
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    int userid = proxy.MarcomManager.User.Id;
                    var GlobalRoleIDquery = "select agru.GlobalRoleId from AM_GlobalRole_User AS agru  JOIN AM_GlobalRole AS agr ON agr.ID=agru.GlobalRoleId WHERE agr.IsAssetAccess=1 AND agru.UserId= ?";
                    var result = tx.PersistenceManager.CommonRepository.ExecuteQuerywithMinParam(GlobalRoleIDquery, userid).Cast<Hashtable>().ToList();

                    //var GlobalRolelist = tx.PersistenceManager.AccessRepository.Query<GlobalRoleUserDao>();
                    //var roleList = from t in GlobalRolelist where t.Userid == UserId select t;
                    if (result.ToList().Count() > 0)
                    {
                        int[] GlobalRoleIdvalue = new int[result.ToList().Count()];
                        for (var i = 0; i < result.ToList().Count(); i++)
                        {
                            GlobalRoleIdvalue[i] = Convert.ToInt32(result[i]["GlobalRoleId"]);
                        }



                        //GlobalRoleIdvalue.co

                        var GetFileProfiles = (from tt in tx.PersistenceManager.CommonRepository.Query<BrandSystems.Marcom.Dal.DAM.Model.DamFileProfileDao>() select tt).ToList();
                        foreach (var Profile in GetFileProfiles)
                        {
                            DamFileProfile dm = new DamFileProfile();
                            dm.ID = Profile.ID;
                            dm.Name = Profile.Name;
                            dm.Height = Profile.Height;
                            dm.Width = Profile.Width;
                            dm.Extension = Profile.Extension;
                            dm.DPI = Profile.DPI;
                            string AssetAccess = Profile.IsAssetAccess;
                            if (AssetAccess != "0")
                            {
                                int[] arrayroles = AssetAccess.Split(',').Select(str => int.Parse(str)).ToArray();
                                bool blnContains = false;
                                foreach (int value in arrayroles)
                                {
                                    if (GlobalRoleIdvalue.Contains(value))
                                    {
                                        blnContains = true;
                                    }
                                }
                                if (blnContains)
                                {
                                    objDamfileprofile.Add(dm);
                                }
                            }
                        }
                        return objDamfileprofile;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch
            {

            }
            return null;
        }

        public bool InsertUpdateProfileFiles(DigitalAssetManagerProxy proxy, int id, string name, double height, double width, string extension, int dpi, string AssetFileProfileRole)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    DamFileProfileDao profile = new DamFileProfileDao();
                    if (id == 0)
                    {
                        profile.ID = 0;
                        profile.Name = name;
                        profile.Height = height;
                        profile.Width = width;
                        profile.Extension = extension;
                        profile.DPI = dpi;
                        profile.IsAssetAccess = AssetFileProfileRole == "" ? "0" : AssetFileProfileRole;
                    }
                    else
                    {
                        profile = tx.PersistenceManager.DamRepository.Query<DamFileProfileDao>().Where(a => a.ID == id).Select(a => a).FirstOrDefault();
                        if (profile != null)
                        {
                            profile.Name = name;
                            profile.Height = height;
                            profile.Width = width;
                            profile.Extension = extension;
                            profile.DPI = dpi;
                            profile.IsAssetAccess = AssetFileProfileRole == "" ? "0" : AssetFileProfileRole;
                        }
                    }
                    tx.PersistenceManager.DamRepository.Save<DamFileProfileDao>(profile);
                    tx.Commit();
                    return true;
                }
            }
            catch
            {
            }
            return false;
        }

        public bool DeleteProfilefile(DigitalAssetManagerProxy proxy, int Id)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {

                    tx.PersistenceManager.CommonRepository.DeleteByID<DamFileProfileDao>(Id);
                    tx.Commit();
                    return true;
                }
            }
            catch
            {
            }
            return false;

        }

        public IList<IOptimakerSettings> GetOptimakerSettings(DigitalAssetManagerProxy proxy)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {

                    var optimakerCollDao = tx.PersistenceManager.DamRepository.Query<OptimakerSettingsDao>().ToList();
                    IList<IOptimakerSettings> optiColl = new List<IOptimakerSettings>();
                    foreach (var item in optimakerCollDao)
                    {
                        optiColl.Add(new OptimakerSettings
                        {
                            ID = item.ID,
                            Name = item.Name,
                            CategoryID = item.CategoryID,
                            Description = item.Description,
                            DepartmentID = item.DepartmentID,
                            DocID = item.DocID,
                            DocType = item.DocType,
                            DocVersionID = item.DocVersionID,
                            PreviewImage = item.PreviewImage
                        });
                    }

                    return optiColl;
                }

            }
            catch
            {

            }
            return null;
        }

        public IList<ICategory> GetOptmakerCatagories(DigitalAssetManagerProxy proxy)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    var CategoryCollDao = tx.PersistenceManager.DamRepository.Query<CategoryDao>().ToList();
                    IList<ICategory> CatagoryCol = new List<ICategory>();
                    CatagoryCol.Add(new Category
                    {
                        id = 0,
                        Caption = "Categories",
                        ParentKey = "0",
                        IsDeleted = false,
                        ParentID = 0,
                        Level = 0
                    });
                    foreach (var item in CategoryCollDao)
                    {
                        CatagoryCol.Add(new Category { id = item.ID, Caption = item.Name, ParentKey = item.ParentKey, ParentID = item.ParentID, Level = item.Level });
                    }
                    return CatagoryCol;

                }
            }
            catch
            {

            }
            return null;
        }


        public string GetCategoryTreeCollection(DigitalAssetManagerProxy proxy)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    var CategoryCollDao = tx.PersistenceManager.DamRepository.Query<CategoryDao>().ToList();
                    IList<ICategory> CatagoryCol = new List<ICategory>();
                    CatagoryCol.Add(new Category
                    {
                        id = 0,
                        Caption = "Categories",
                        ParentKey = "0",
                        IsDeleted = false,
                        ParentID = 0,
                        Children = RecursionCategoryCollection(tx, 0)
                    });



                    //Category CatagoryCol = new Category();
                    //CatagoryCol.id = 0;
                    //CatagoryCol.Caption = "Categories";
                    //CatagoryCol.ParentKey = "0";
                    //CatagoryCol.IsDeleted = false;
                    //CatagoryCol.ParentID = 0;
                    //CatagoryCol.Children = RecursionCategoryCollection(tx, 0);

                    tx.Commit();
                    return JsonConvert.SerializeObject(CatagoryCol);
                    // return CatagoryCol;

                }
            }
            catch
            {

            }
            return null;
        }

        public List<Category> RecursionCategoryCollection(ITransaction tx, int ParentID)
        {

            var CategoryCollDao = tx.PersistenceManager.DamRepository.Query<CategoryDao>().ToList().Where(a => a.ParentID == ParentID);
            List<Category> CatagoryCol = new List<Category>();


            foreach (var item in CategoryCollDao)
            {
                CatagoryCol.Add(new Category
                {
                    id = item.ID,
                    Caption = item.Name,
                    ParentKey = item.ParentKey,
                    IsDeleted = false,
                    ParentID = item.ParentID,
                    Children = RecursionCategoryCollection(tx, item.ID)
                });
            }
            return CatagoryCol;
        }


        public bool InsertUpdateCatergory(DigitalAssetManagerProxy proxy, JObject jobj)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    tx.PersistenceManager.DamRepository.ExecuteQuery("TRUNCATE TABLE DAM_Category");
                    tx.Commit();
                }

                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {

                    IList<CategoryDao> catdao = new List<CategoryDao>();
                    JArray arr = (JArray)jobj["CategoryColl"][0]["Children"];
                    for (int i = 0; i < arr.Count; i++)
                    {
                        CategoryDao ctadao = new CategoryDao();
                        ctadao.ID = 0;
                        ctadao.Name = (string)arr[i]["Caption"];
                        ctadao.ParentID = 0;
                        ctadao.Level = 1;
                        ctadao.ParentKey = "0." + i;
                        catdao.Add(ctadao);
                        tx.PersistenceManager.DamRepository.Save<CategoryDao>(ctadao);
                        RecursiveInsertupdateCategory(tx, (JArray)arr[i]["Children"], ctadao.ParentKey, ctadao.ID);
                    }

                    tx.Commit();
                }
            }
            catch
            {

            }
            return false;
        }


        public void RecursiveInsertupdateCategory(ITransaction tx, JArray arr, string ParentKey, int ParentID)
        {
            //IList<CategoryDao> catdao = new List<CategoryDao>();
            for (int i = 0; i < arr.Count; i++)
            {
                CategoryDao ctadao = new CategoryDao();
                ctadao.ID = 0;
                ctadao.Name = (string)arr[i]["Caption"];
                ctadao.ParentID = ParentID;
                ctadao.Level = ParentKey.Split('.').Length;
                ctadao.ParentKey = ParentKey + "." + i;
                tx.PersistenceManager.DamRepository.Save<CategoryDao>(ctadao);
                RecursiveInsertupdateCategory(tx, (JArray)arr[i]["Children"], ctadao.ParentKey, ctadao.ID);
            }

        }


        public int InsertUpdateOptimakerSettings(DigitalAssetManagerProxy proxy, int ID, string Name, int CategoryId, string Description, int DocId, int DeptId, int DocType, int DocVersionId, string PreviewImage)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {

                    OptimakerSettingsDao optsetdao = new OptimakerSettingsDao();
                    optsetdao.ID = ID;
                    optsetdao.Name = Name;
                    optsetdao.CategoryID = CategoryId;
                    optsetdao.DepartmentID = DeptId;
                    optsetdao.DocID = DocId;
                    optsetdao.DocType = DocType;
                    optsetdao.DocVersionID = DocVersionId;
                    optsetdao.Description = Description;
                    optsetdao.PreviewImage = PreviewImage;
                    tx.PersistenceManager.DamRepository.Save<OptimakerSettingsDao>(optsetdao);
                    tx.Commit();

                    return optsetdao.ID;
                }
            }
            catch
            {

            }

            return 0;

        }


        public bool DeleteOptimakeSetting(DigitalAssetManagerProxy proxy, int ID)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    tx.PersistenceManager.DamRepository.DeleteByID<OptimakerSettingsDao>(ID);
                    tx.Commit();

                    return true;
                }
            }
            catch
            {

            }

            return false;

        }


        public bool DeleteOptimakerCategory(DigitalAssetManagerProxy proxy, int ID)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    tx.PersistenceManager.DamRepository.DeleteByID<CategoryDao>(ID);
                    tx.Commit();

                    return true;
                }
            }
            catch
            {

            }

            return false;

        }

        public string GetOptimakerSettingsBaseURL(DigitalAssetManagerProxy proxy)
        {
            try
            {
                return ConfigurationManager.AppSettings["OptimakerBaseURL"] + "," + ConfigurationManager.AppSettings["DOCTemplateURL"] + "," + ConfigurationManager.AppSettings["DocTemplatePath"];
            }
            catch
            {
                return "";
            }


        }

        public string GetMediaGeneratorSettingsBaseURL(DigitalAssetManagerProxy proxy)
        {
            try
            {
                return ConfigurationManager.AppSettings["mediageneratorBaseURL"];
            }
            catch
            {
                return "";
            }


        }

        public IList GetAllOptimakerSettings(DigitalAssetManagerProxy proxy, int categoryId)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {

                    StringBuilder strBuilder = new StringBuilder();
                    strBuilder.AppendLine(" SELECT dos.ID, ");
                    strBuilder.AppendLine(" dos.NAME               AS 'Name', ");
                    strBuilder.AppendLine(" dos.CategoryID, ");
                    strBuilder.AppendLine(" dos.[Description]      AS 'Description', ");
                    strBuilder.AppendLine(" dos.DocID              AS 'DocID', ");
                    strBuilder.AppendLine(" dos.DepartmentID       AS 'DepartmentID', ");
                    strBuilder.AppendLine("dos.DocType            AS 'DocType', ");
                    strBuilder.AppendLine(" dos.DocVersionID       AS 'DocVersionID', ");
                    strBuilder.AppendLine(" NULL                   AS 'WordPath', ");
                    strBuilder.AppendLine(" dos.PreviewImage       AS 'PreviewImage' ");
                    strBuilder.AppendLine("FROM   DAM_OptimakerSettings     dos ");
                    strBuilder.AppendLine("WHERE  dos.CategoryID = ?  ");

                    strBuilder.AppendLine("UNION  ");

                    strBuilder.AppendLine("SELECT dos.ID, ");
                    strBuilder.AppendLine("dos.Name                  AS 'Name', ");
                    strBuilder.AppendLine("dos.CategoryID, ");
                    strBuilder.AppendLine("dos.[DESCRIPTION]         AS 'Description', ");
                    strBuilder.AppendLine("NULL                      AS DocID, ");
                    strBuilder.AppendLine(" NULL                      AS 'DepartmentID', ");
                    strBuilder.AppendLine("NULL                      AS 'DocType', ");
                    strBuilder.AppendLine("NULL                      AS 'DocVersionID', ");
                    strBuilder.AppendLine("dos.Worddocpath           AS 'WordPath', ");
                    strBuilder.AppendLine("dos.PreviewImage          AS 'PreviewImage' ");
                    strBuilder.AppendLine("FROM   DAM_WordTemplateSettings     dos ");
                    strBuilder.AppendLine("WHERE  dos.CategoryID = ?  ");


                    IList result = tx.PersistenceManager.DamRepository.ExecuteQuerywithMinParam(strBuilder.ToString(), categoryId, categoryId);
                    tx.Commit();
                    return result;
                }
            }
            catch
            {
                return null;
            }


        }

        public IList<IWordTemplateSettings> GetWordTemplateSettings(DigitalAssetManagerProxy proxy)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {

                    var WordtempCollDao = tx.PersistenceManager.DamRepository.Query<WordTemplateSettingsDao>().ToList();
                    IList<IWordTemplateSettings> wordTempColl = new List<IWordTemplateSettings>();
                    foreach (var item in WordtempCollDao)
                    {
                        wordTempColl.Add(new WordTemplateSettings
                        {
                            ID = item.ID,
                            Name = item.Name,
                            CategoryID = item.CategoryID,
                            Description = item.Description,
                            Worddocpath = item.Worddocpath,
                            PreviewImage = item.PreviewImage,
                        });
                    }

                    return wordTempColl;
                }

            }
            catch
            {

            }
            return null;
        }

        public int InsertUpdateWordTemplateSettings(DigitalAssetManagerProxy proxy, int ID, string Name, int CategoryId, string Description, string Worddocpath, string PreviewImage)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {

                    var CurrentDao = tx.PersistenceManager.DamRepository.Query<WordTemplateSettingsDao>().Where(a => a.ID == ID).SingleOrDefault();


                    var splitArr = Worddocpath.Split('.');

                    string filePath = Path.Combine(HttpRuntime.AppDomainAppPath, @"DAMFiles\Templates\Word\Documents\");

                    string DocumentPath = Worddocpath;
                    if (CurrentDao != null && CurrentDao.Worddocpath != Worddocpath)
                    {
                        DocumentPath = Guid.NewGuid().ToString();
                        System.IO.File.Move(filePath + "\\" + Worddocpath, filePath + "\\" + DocumentPath);
                        DocumentPath += "." + splitArr[1];
                    }

                    WordTemplateSettingsDao wordtempdao = new WordTemplateSettingsDao();
                    wordtempdao.ID = ID;
                    wordtempdao.Name = Name;
                    wordtempdao.CategoryID = CategoryId;
                    wordtempdao.Worddocpath = DocumentPath;
                    wordtempdao.PreviewImage = PreviewImage;
                    wordtempdao.Description = Description;
                    tx.PersistenceManager.DamRepository.Save<WordTemplateSettingsDao>(wordtempdao);
                    tx.Commit();

                    return wordtempdao.ID;
                }
            }
            catch
            {

            }

            return 0;

        }


        public bool DeleteWordtempSetting(DigitalAssetManagerProxy proxy, int ID)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    tx.PersistenceManager.DamRepository.DeleteByID<WordTemplateSettingsDao>(ID);
                    tx.Commit();

                    return true;
                }
            }
            catch
            {

            }

            return false;

        }

        public void CreateMediaGeneratorAsset(int entityID, int folderID, int assetTypeID, int createdBy, int docVersionID, string assetName, string jpegname, long jpegsize, int jpegversion, string jpegdesc, string jpegGUID, string highResGUID, string lowResGUID, int docID)
        {
            int AssetID = 0;
            Guid userSession = MarcomManagerFactory.GetSystemSession();
            IMarcomManager managers = MarcomManagerFactory.GetMarcomManager(userSession);
            DAMFileDao damdao = new DAMFileDao();
            AssetsDao assetdao = new AssetsDao();
            try
            {
                using (ITransaction tx = managers.GetTransaction())
                {
                    var mgadao = tx.PersistenceManager.DamRepository.Query<MediaGeneratorAssetDao>().Where(a => a.DocVersionID == docVersionID).Select(a => a);
                    int resultcount = mgadao.Count();
                    if (mgadao == null || resultcount == 0)
                    {
                        //assetdao.CreatedBy = proxy.MarcomManager.User.Id;
                        assetdao.Name = HttpUtility.HtmlEncode(assetName);
                        assetdao.FolderID = folderID;
                        assetdao.EntityID = entityID;
                        assetdao.Createdon = DateTime.UtcNow;
                        assetdao.AssetTypeid = assetTypeID;
                        assetdao.CreatedBy = createdBy;
                        assetdao.ActiveFileID = 0;
                        assetdao.UpdatedOn = DateTime.UtcNow;
                        //assetdao.Url = docVersionID.ToString();
                        tx.PersistenceManager.DamRepository.Save<AssetsDao>(assetdao);
                        AssetID = assetdao.ID;

                        StringBuilder insertDynQuery = new StringBuilder();
                        insertDynQuery.Append("INSERT INTO MM_AttributeRecord_" + assetTypeID + "(ID) VALUES(" + AssetID + ")");
                        tx.PersistenceManager.DamRepository.ExecuteQuery(insertDynQuery.ToString());


                    }
                    else
                        AssetID = mgadao.FirstOrDefault().AssetID;
                    tx.Commit();
                }


                using (ITransaction tx1 = managers.GetTransaction())
                {
                    if (jpegGUID != null || jpegGUID != "")
                    {
                        damdao.CreatedOn = DateTime.UtcNow;
                        damdao.Extension = ".jpg";
                        damdao.MimeType = "image/jpeg";
                        damdao.Name = assetName; //HttpUtility.HtmlEncode(jpegname);
                        damdao.OwnerID = createdBy;
                        damdao.Size = jpegsize;
                        damdao.VersionNo = jpegversion;
                        damdao.Description = HttpUtility.HtmlEncode(jpegdesc);
                        damdao.FileGuid = Guid.Parse(jpegGUID);
                        damdao.AssetID = AssetID;
                        damdao.Status = 0;
                        tx1.PersistenceManager.CommonRepository.Save<DAMFileDao>(damdao);
                    }


                    AssetsDao assetdaoforID = new AssetsDao();
                    assetdaoforID = tx1.PersistenceManager.PlanningRepository.Query<AssetsDao>().Where(a => a.ID == AssetID).Select(a => a).FirstOrDefault();
                    assetdaoforID.ActiveFileID = damdao.ID;
                    tx1.PersistenceManager.PlanningRepository.Save<AssetsDao>(assetdaoforID);
                    MediaGeneratorAssetDao mgadao = new MediaGeneratorAssetDao();
                    mgadao.AssetID = AssetID;
                    mgadao.DocVersionID = docVersionID;
                    mgadao.HighResPdf = highResGUID;
                    mgadao.LowResPdf = lowResGUID;
                    mgadao.Jpeg = jpegGUID;
                    mgadao.Version = damdao.ID;
                    mgadao.DocID = docID;
                    tx1.PersistenceManager.DamRepository.Save<MediaGeneratorAssetDao>(mgadao);
                    tx1.Commit();
                }

            }
            catch (Exception e)
            {
                string LogFile = HttpContext.Current.Server.MapPath("D:/inetpub/wwwroot/Marcom_dam_demo/Logs/Log_optimaker.txt");
                using (FileStream fs = new FileStream(LogFile, FileMode.Append, FileAccess.Write))
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine("EXCEPTION IN CORE FUNCTION CreateMediaGeneratorAsset" + e);
                    sw.WriteLine("----------------");
                }
            }
        }
        /// <summary>
        /// get the attribtues for thumbnail, list, summary view in admin settings
        /// </summary>
        /// <param name="proxy"></param>
        /// <param name="ViewType"></param>
        /// <param name="DamType"></param>
        /// <returns></returns>
        public List<object> GetDAMViewAdminSettings(DigitalAssetManagerProxy proxy, string ViewType, int DamType)
        {
            try
            {
                //ViewType = "ThumbnailView";
                //DamType = 103;
                string Adminxmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
                XDocument xDoc = XDocument.Load(Adminxmlpath);
                var newtest = xDoc.Descendants("DAMsettings").Descendants(ViewType).Descendants("AssetType").Select(a => a).ToList().Where(a => Convert.ToInt32(a.Attribute("ID").Value) == DamType).Select(a => a);
                EntityTypeAttributeRelation re = new EntityTypeAttributeRelation();
                re.AttributeID = 1;
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    IList<IEntityTypeAttributeRelation> entitytyperelation = new List<IEntityTypeAttributeRelation>();
                    entitytyperelation = proxy.MarcomManager.MetadataManager.GetEntityTypeAttributeRelationByID(DamType);
                    int version = MarcomManagerFactory.ActiveMetadataVersionNumber;
                    string xmlpath = tx.PersistenceManager.MetadataRepository.GetXmlPath(version);
                    XDocument docx = XDocument.Load(xmlpath);
                    var a = proxy.MarcomManager.MetadataManager.GetEntityTypeAttributeRelationByID(DamType);
                    var b = docx.Root.Elements("EntityTypeAttributeRelation_Table").Elements("EntityTypeAttributeRelation");
                    var c = newtest.Elements("Attributes").Elements("Attribute");
                    var d = docx.Root.Elements("Attribute_Table").Elements("Attribute");

                    var listview = (from relation in docx.Root.Elements("EntityTypeAttributeRelation_Table").Elements("EntityTypeAttributeRelation")
                                    join adminattr in newtest.Elements("Attributes").Elements("Attribute") on Convert.ToInt32(relation.Element("AttributeID").Value) equals Convert.ToInt32(adminattr.Element("Id").Value) into gj
                                    join attr in docx.Root.Elements("Attribute_Table").Elements("Attribute") on Convert.ToInt32(relation.Element("AttributeID").Value) equals Convert.ToInt32(attr.Element("ID").Value)
                                    from subpet in gj.DefaultIfEmpty()
                                    where Convert.ToInt32(relation.Element("EntityTypeID").Value) == DamType
                                    && Convert.ToInt32(attr.Element("ID").Value) != 71 && Convert.ToInt32(attr.Element("ID").Value) != 74 && Convert.ToInt32(attr.Element("ID").Value) != 75
                                    // && relation.Element("Caption").Value != "MyRoleGlobalAccess" && relation.Element("Caption").Value != "MyRoleEntityAccess"
                                    orderby subpet == null ? 0 : Convert.ToInt32(subpet.Element("SortOrder").Value)
                                    select new
                                    {
                                        ID = subpet == null ? Convert.ToInt32(attr.Element("ID").Value) : Convert.ToInt32(subpet.Element("Id").Value),
                                        DisplayName = subpet == null ? attr.Element("Caption").Value : subpet.Element("DisplayName").Value,
                                        SortOrder = subpet == null ? 0 : Convert.ToInt32(subpet.Element("SortOrder").Value),
                                        Level = subpet == null ? 0 : Convert.ToInt32(subpet.Element("Level").Value),
                                        Type = subpet == null ? Convert.ToInt32(attr.Element("AttributeTypeID").Value) : Convert.ToInt32(subpet.Element("Type").Value),
                                        IsColumn = subpet == null ? false : Convert.ToBoolean(subpet.Element("IsColumn").Value),
                                        IsToolTip = subpet == null ? false : Convert.ToBoolean(subpet.Element("IsToolTip").Value),
                                    }).OrderBy(so => so.SortOrder).ToList();

                    List<object> obj = new List<object>();
                    obj.Add(listview);
                    return obj;

                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public bool UpdateDamViewStatus(DigitalAssetManagerProxy proxy, string ViewType, int DamType, IList<IDamViewAttribtues> attributeslist)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    attributeslist = attributeslist.OrderBy(a => a.SortOrder).Select(a => a).ToList();
                    var xEle = new XElement("Attributes",
                from attr in attributeslist
                select new XElement("Attribute",
                             new XElement("Id", attr.ID),
                               new XElement("DisplayName", attr.DisplayName),
                               new XElement("SortOrder", attr.SortOrder),
                               new XElement("Level", attr.Level),
                               new XElement("Type", attr.Type),
                               new XElement("IsColumn", attr.IsColumn),
                               new XElement("IsToolTip", attr.IsToolTip)
                           ));

                    string xmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
                    XDocument adminXmlDoc = XDocument.Load(xmlpath);
                    var Defaultrootssetting = adminXmlDoc.Descendants("DAMsettings").FirstOrDefault();
                    var DefaultLogoSettings = adminXmlDoc.Descendants("DAMsettings").Descendants(ViewType).FirstOrDefault();
                    if (Defaultrootssetting == null && DefaultLogoSettings == null)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("<DAMsettings><" + ViewType + "></" + ViewType + "></DAMsettings>");
                        XElement.Parse(sb.ToString());
                        adminXmlDoc.Root.Add(XElement.Parse(sb.ToString()));
                        adminXmlDoc.Save(xmlpath);
                    }
                    else if (DefaultLogoSettings == null)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("<" + ViewType + "></" + ViewType + ">");
                        adminXmlDoc.Element("AppSettings").Descendants("DAMsettings").FirstOrDefault().Add(XElement.Parse(sb.ToString()));
                        adminXmlDoc.Save(xmlpath);
                    }

                    var checkcount = adminXmlDoc.Descendants("DAMsettings").Descendants(ViewType).Descendants("AssetType").Where(a => Convert.ToInt32(a.Attribute("ID").Value) == DamType);
                    if (checkcount.Count() == 0)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("<AssetType ID='" + DamType + "'></AssetType>");
                        adminXmlDoc.Element("AppSettings").Descendants("DAMsettings").Descendants(ViewType).FirstOrDefault().Add(XElement.Parse(sb.ToString()));
                        adminXmlDoc.Save(xmlpath);
                    }
                    adminXmlDoc.Descendants("DAMsettings").Descendants(ViewType).Descendants("AssetType").Where(a => Convert.ToInt32(a.Attribute("ID").Value) == DamType).Select(a => a).Descendants("Attributes").Remove();
                    adminXmlDoc.Descendants("DAMsettings").Descendants(ViewType).Descendants("AssetType").Where(a => Convert.ToInt32(a.Attribute("ID").Value) == DamType).FirstOrDefault().Add(xEle);
                    adminXmlDoc.Save(xmlpath);

                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<object> GetDAMToolTipSettings(DigitalAssetManagerProxy proxy)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    List<object> returnObj = new List<object>();
                    string Adminxmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
                    XDocument xDoc = XDocument.Load(Adminxmlpath);


                    var thumpnail = (from AdminAttributes in xDoc.Descendants("DAMsettings").Descendants("ThumbnailView").Descendants("AssetType").Descendants("Attributes").Descendants("Attribute")
                                     join ser in tx.PersistenceManager.MetadataRepository.Query<AttributeDao>() on Convert.ToInt16(AdminAttributes.Element("Id").Value) equals ser.Id
                                     where Convert.ToBoolean(AdminAttributes.Element("IsToolTip").Value) == true
                                     select new
                                     {
                                         ID = Convert.ToInt16(AdminAttributes.Element("Id").Value),
                                         Caption = ser.Caption,
                                         SotOrder = Convert.ToInt32(AdminAttributes.Element("SortOrder").Value),
                                         Type = ser.AttributeTypeID,
                                         assetType = Convert.ToInt16(AdminAttributes.Parent.Parent.Attributes("ID").ToList().FirstOrDefault().Value.ToString()),
                                         Field = ser.Id,
                                     }).Distinct().ToList();

                    var listview = (from AdminAttributes in xDoc.Descendants("DAMsettings").Descendants("ListView").Descendants("AssetType").Descendants("Attributes").Descendants("Attribute")
                                    join ser in tx.PersistenceManager.MetadataRepository.Query<AttributeDao>() on Convert.ToInt16(AdminAttributes.Element("Id").Value) equals ser.Id
                                    where Convert.ToBoolean(AdminAttributes.Element("IsToolTip").Value) == true
                                    select new
                                    {
                                        ID = Convert.ToInt16(AdminAttributes.Element("Id").Value),
                                        Caption = ser.Caption,
                                        SotOrder = Convert.ToInt32(AdminAttributes.Element("SortOrder").Value),
                                        Type = ser.AttributeTypeID,
                                        assetType = Convert.ToInt16(AdminAttributes.Parent.Parent.Attributes("ID").ToList().FirstOrDefault().Value.ToString()),
                                        Field = ser.Id,
                                    }).Distinct().ToList();


                    returnObj.Add(new
                    {
                        ThumbnailSettings = thumpnail,
                        ListViewSettings = listview
                    });
                    return returnObj;
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public void CreateDOCGeneratedAsset(int entityID, int folderID, int assetTypeID, int createdBy, string intialfilename, string assetName)
        {
            int ActiveFileID = 0;
            int AssetID;
            Guid NewId = Guid.NewGuid();
            string intialfilepath = Path.Combine(HttpRuntime.AppDomainAppPath, ConfigurationManager.AppSettings["assetdocpath"]);
            string filePath = ReadAdminXML("FileManagment");
            var DirInfo = System.IO.Directory.GetParent(filePath);
            string newFilePath = DirInfo.FullName;
            System.IO.File.Copy(intialfilepath + "\\" + intialfilename, newFilePath + "\\" + NewId + Path.GetExtension(intialfilepath + "\\" + intialfilename));
            System.IO.File.Copy(intialfilepath + "\\" + intialfilename.Replace(".docx", ".pdf"), newFilePath + "\\" + NewId + ".pdf");
            Guid userSession = MarcomManagerFactory.GetSystemSession();
            IMarcomManager managers = MarcomManagerFactory.GetMarcomManager(userSession);
            try
            {
                using (ITransaction tx = managers.GetTransaction())
                {
                    AssetsDao assetdao = new AssetsDao();
                    assetdao.CreatedBy = createdBy;
                    assetdao.Name = HttpUtility.HtmlEncode(assetName);
                    assetdao.FolderID = folderID;
                    assetdao.EntityID = entityID;
                    assetdao.Createdon = DateTime.UtcNow;
                    assetdao.AssetTypeid = assetTypeID;
                    assetdao.UpdatedOn = DateTime.UtcNow;
                    //assetdao.Url = docVersionID.ToString();
                    tx.PersistenceManager.DamRepository.Save<AssetsDao>(assetdao);
                    AssetID = assetdao.ID;

                    //Metadatainsertion comes here.

                    if (intialfilename != null || intialfilename == "")
                    {
                        FileInfo f2 = new FileInfo(intialfilepath + "\\" + intialfilename);

                        DAMFileDao damdao = new DAMFileDao();
                        damdao.CreatedOn = DateTime.UtcNow;
                        damdao.Extension = Path.GetExtension(intialfilepath + "\\" + intialfilename);
                        damdao.MimeType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                        //damdao.MimeType = GetContentType(intialfilepath + "\\" + intilafilename);
                        damdao.Name = Convert.ToString(NewId);
                        damdao.OwnerID = createdBy;
                        damdao.Size = f2.Length;
                        damdao.VersionNo = 1;
                        //damdao.Description = HttpUtility.HtmlEncode(Description);
                        damdao.FileGuid = NewId;
                        damdao.AssetID = assetdao.ID;
                        tx.PersistenceManager.CommonRepository.Save<DAMFileDao>(damdao);
                        ActiveFileID = damdao.ID;
                    }


                    StringBuilder insertDynQuery = new StringBuilder();
                    insertDynQuery.Append("INSERT INTO MM_AttributeRecord_" + assetTypeID + "(ID) VALUES(" + AssetID + ")");
                    tx.PersistenceManager.DamRepository.ExecuteQuery(insertDynQuery.ToString());

                    tx.Commit();
                }
                using (ITransaction tx1 = managers.GetTransaction())
                {
                    AssetsDao assetdaoforID = new AssetsDao();
                    assetdaoforID = tx1.PersistenceManager.PlanningRepository.Query<AssetsDao>().Where(a => a.ID == AssetID).Select(a => a).FirstOrDefault();
                    assetdaoforID.ActiveFileID = ActiveFileID;
                    tx1.PersistenceManager.PlanningRepository.Save<AssetsDao>(assetdaoforID);
                    tx1.Commit();
                }
            }
            catch (Exception e)
            {
            }
        }


        public string GetContentType(string fname)
        {
            // set a default mimetype if not found.
            string contentType = "application/octet-stream";
            try
            {
                // get the registry classes root
                RegistryKey classes = Registry.ClassesRoot;

                // find the sub key based on the file extension
                RegistryKey fileClass = classes.OpenSubKey(Path.GetExtension(fname));
                contentType = fileClass.GetValue("Content Type").ToString();
            }
            catch { }

            return contentType;
        }
        public List<object> GetPublishedAssets(DigitalAssetManagerProxy proxy, int viewType, int orderbyid, int pageNo)
        {

            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    List<object> returnObj = new List<object>();

                    string viewName = Enum.GetName(typeof(AssetView), viewType);
                    int totalrecords = 20;
                    if (viewType == (int)AssetView.ListView)
                        totalrecords = 30;
                    if (viewType == (int)AssetView.SummaryView)
                        totalrecords = 10;


                    StringBuilder assetQuery = new StringBuilder();
                    IList<MultiProperty> assetQuery_parLIST = new List<MultiProperty>();
                    assetQuery_parLIST.Add(new MultiProperty { propertyName = "ispublish", propertyValue = 1 });


                    assetQuery.AppendLine(" DECLARE @RowsPerPage INT = " + totalrecords + ", ");
                    assetQuery.AppendLine(" @PageNumber INT =" + pageNo + " ");
                    assetQuery.AppendLine(" DECLARE @OrderBy INT = " + orderbyid + "; ");
                    assetQuery.AppendLine(" SELECT a.ID AS 'FileUniqueID', ");
                    assetQuery.AppendLine(" a.Name as 'FileName', ");
                    assetQuery.AppendLine(" a.Extension, ");
                    assetQuery.AppendLine(" a.[Size],  a.[VersionNo], ");
                    assetQuery.AppendLine(" a.OwnerID, ");
                    assetQuery.AppendLine(" a.CreatedOn, ");
                    assetQuery.AppendLine(" a.FileGuid, ");
                    assetQuery.AppendLine(" a.[Description], ");
                    assetQuery.AppendLine(" a.AssetID, ");
                    assetQuery.AppendLine(" da.FolderID, ");
                    assetQuery.AppendLine(" da.EntityID, ");
                    assetQuery.AppendLine(" da.ID AS 'AssetUniqueID', ");
                    assetQuery.AppendLine(" da.Name as 'AssetName', da.Url as 'LinkURL', a.MimeType as 'MimeType', ");
                    assetQuery.AppendLine(" da.AssetTypeid, ");
                    assetQuery.AppendLine(" da.ActiveFileID, ");
                    assetQuery.AppendLine(" met.ColorCode, ");
                    assetQuery.AppendLine(" met.ShortDescription, ");
                    assetQuery.AppendLine(" a.[Status], ");
                    assetQuery.AppendLine(" da.Category, da.IsPublish, ISNULL(da.LinkedAssetID,0) as LinkedAssetID ");
                    assetQuery.AppendLine(" FROM   DAM_File a ");
                    assetQuery.AppendLine(" RIGHT OUTER JOIN DAM_Asset da ");
                    assetQuery.AppendLine(" ON  a.AssetID = da.ID AND a.ID=da.ActiveFileID");
                    assetQuery.AppendLine(" INNER JOIN MM_EntityType met ");
                    assetQuery.AppendLine(" ON  met.ID = da.AssetTypeid ");
                    assetQuery.AppendLine(" WHERE  da.id IN (SELECT da2.id ");
                    assetQuery.AppendLine(" FROM   DAM_Asset da2 ");
                    assetQuery.AppendLine(" WHERE  da2.IsPublish=1) ");
                    assetQuery.AppendLine("  ORDER BY  ");
                    assetQuery.AppendLine(" CASE   ");
                    assetQuery.AppendLine(" WHEN @OrderBy = 1  ");
                    assetQuery.AppendLine(" THEN da.Name  ");
                    assetQuery.AppendLine(" END       asc,  ");
                    assetQuery.AppendLine(" CASE   ");
                    assetQuery.AppendLine(" WHEN @OrderBy = 2  ");
                    assetQuery.AppendLine(" THEN da.Name  ");
                    assetQuery.AppendLine(" END desc,  ");
                    assetQuery.AppendLine(" CASE   ");
                    assetQuery.AppendLine(" WHEN @OrderBy = 3  ");
                    assetQuery.AppendLine(" THEN da.Createdon   ");
                    assetQuery.AppendLine(" END       asc , ");
                    assetQuery.AppendLine(" CASE   ");
                    assetQuery.AppendLine(" WHEN @OrderBy = 4 ");
                    assetQuery.AppendLine(" THEN da.Createdon   ");
                    assetQuery.AppendLine(" END       desc  ");

                    assetQuery.AppendLine(" OFFSET(@PageNumber - 1) * @RowsPerPage ROWS ");

                    assetQuery.AppendLine(" FETCH NEXT @RowsPerPage ROWS ONLY ");


                    IList assets = tx.PersistenceManager.DamRepository.ExecuteQuery(assetQuery.ToString());

                    int version = MarcomManagerFactory.ActiveMetadataVersionNumber;
                    string xmlpath = string.Empty;

                    xmlpath = tx.PersistenceManager.MetadataRepository.GetXmlPath(version);
                    var xmetadataDoc = XDocument.Load(xmlpath);

                    string Adminxmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
                    XDocument xDoc = XDocument.Load(Adminxmlpath);

                    IList<XElement> tempresults = new List<XElement>();
                    IList<XElement> result = new List<XElement>();

                    if (viewType == (int)AssetView.ThumbnailView)
                    {
                        tempresults = xDoc.Descendants("DAMsettings").Descendants(viewName).Descendants("AssetType").Descendants("Attributes").Descendants("Attribute").Where(a => Convert.ToBoolean(a.Element("IsColumn").Value) == true).Select(a => a).ToList();
                        foreach (var rec in tempresults)
                        {
                            IList<XElement> duplist = new List<XElement>();
                            duplist = result.Where(a => Convert.ToInt32(a.Element("Id").Value) == Convert.ToInt32(rec.Element("Id").Value)).Select(a => a).ToList();
                            if (duplist.Count == 0)
                                result.Add(rec);
                        }
                    }
                    else if (viewType == (int)AssetView.SummaryView)
                    {
                        IList<XElement> thumpnail = xDoc.Descendants("DAMsettings").Descendants("ThumbnailView").Descendants("AssetType").Descendants("Attributes").Descendants("Attribute").Where(a => Convert.ToBoolean(a.Element("IsColumn").Value) == true).Select(a => a).ToList();
                        IList<XElement> summaryList = xDoc.Descendants("DAMsettings").Descendants(viewName).Descendants("AssetType").Descendants("Attributes").Descendants("Attribute").Where(a => Convert.ToBoolean(a.Element("IsColumn").Value) == true).Select(a => a).ToList();
                        IList<XElement> tempList = new List<XElement>(thumpnail.Concat(summaryList));
                        foreach (var rec in tempList)
                        {
                            IList<XElement> duplist = new List<XElement>();
                            duplist = result.Where(a => Convert.ToInt32(a.Element("Id").Value) == Convert.ToInt32(rec.Element("Id").Value)).Select(a => a).ToList();
                            if (duplist.Count == 0)
                                result.Add(rec);
                        }

                    }
                    else if (viewType == (int)AssetView.ListView)
                    {
                        tempresults = xDoc.Descendants("DAMsettings").Descendants(viewName).Descendants("AssetType").Descendants("Attributes").Descendants("Attribute").Where(a => Convert.ToBoolean(a.Element("IsColumn").Value) == true).Select(a => a).ToList();
                        foreach (var rec in tempresults)
                        {
                            IList<XElement> duplist = new List<XElement>();
                            duplist = result.Where(a => Convert.ToInt32(a.Element("Id").Value) == Convert.ToInt32(rec.Element("Id").Value)).Select(a => a).ToList();
                            if (duplist.Count == 0)
                                result.Add(rec);
                        }
                    }

                    int[] attrsidarr = result.Distinct().Select(a => Convert.ToInt32(a.Element("Id").Value)).ToArray();

                    var typeidArr = assets.Cast<Hashtable>().Select(a => (int)a["AssetTypeid"]).Distinct().ToArray();

                    var idArr = assets.Cast<Hashtable>().Select(a => (int)a["AssetUniqueID"]).ToArray();

                    IList<AttributeDao> attributes = new List<AttributeDao>();
                    attributes = (from attrbs in tx.PersistenceManager.MetadataRepository.Query<AttributeDao>() where attrsidarr.Contains(attrbs.Id) select attrbs).ToList<AttributeDao>();

                    IList<EntityTypeAttributeRelationDao> entityAttributes = new List<EntityTypeAttributeRelationDao>();
                    entityAttributes = (from attrbs in tx.PersistenceManager.MetadataRepository.Query<EntityTypeAttributeRelationDao>() where typeidArr.Contains(attrbs.EntityTypeID) select attrbs).ToList<EntityTypeAttributeRelationDao>();




                    var attributerelationList = (from AdminAttributes in result
                                                 join ser in attributes on Convert.ToInt16(AdminAttributes.Element("Id").Value) equals ser.Id
                                                 select new
                                                 {
                                                     ID = Convert.ToInt16(AdminAttributes.Element("Id").Value),
                                                     SotOrder = Convert.ToInt32(AdminAttributes.Element("SortOrder").Value),
                                                     Type = ser.AttributeTypeID,
                                                     IsSpecial = ser.IsSpecial,
                                                     Field = ser.Id,
                                                     Level = Convert.ToInt16(AdminAttributes.Element("Level").Value),
                                                 }).Distinct().ToList();

                    int[] attrSelectType = { 1, 2, 3, 5 };

                    StringBuilder damQuery = new StringBuilder();
                    damQuery.AppendLine("(");
                    int EntitypeLenghth = typeidArr.Distinct().Count();
                    for (var i = 0; i < typeidArr.Distinct().Count(); i++)
                    {

                        damQuery.AppendLine("select ID");
                        foreach (var currentval in attributerelationList.Where(a => a.IsSpecial != true && attrSelectType.Contains(a.Type)).ToList())
                        {
                            int val = entityAttributes.ToList().Where(a => a.EntityTypeID == typeidArr[i] && a.AttributeID == currentval.ID).Count();
                            if (val != 0)
                                damQuery.AppendLine(" ,Attr_" + currentval.ID + " as 'Attr_" + currentval.ID + "'");
                            else
                                damQuery.AppendLine(" ,'-' as  'Attr_" + currentval.ID + "'");
                        }
                        damQuery.AppendLine(" from MM_AttributeRecord_" + typeidArr[i]);
                        if (i < EntitypeLenghth - 1)
                        {
                            damQuery.AppendLine(" union ");
                        }
                    }
                    damQuery.AppendLine(") subtbl");

                    StringBuilder mainTblQry = new StringBuilder();
                    mainTblQry.AppendLine("select subtbl.ID  ");
                    int LastTreeLevel = attributerelationList.Where(a => (AttributesList)a.Type == AttributesList.TreeMultiSelection).OrderByDescending(a => a.Level).Select(a => a.Level).FirstOrDefault();
                    for (int j = 0; j < attributerelationList.Count(); j++)
                    {
                        string CurrentattrID = attributerelationList[j].ID.ToString();
                        if (attributerelationList[j].IsSpecial == true)
                        {
                            switch ((SystemDefinedAttributes)attributerelationList[j].ID)
                            {
                                case SystemDefinedAttributes.Name:
                                    mainTblQry.AppendLine(",pe.Name  as '" + attributerelationList[j].Field + "'");
                                    break;
                                case SystemDefinedAttributes.Owner:
                                    mainTblQry.Append(",ISNULL( (SELECT top 1  ISNULL(us.FirstName,'') + ' ' + ISNULL(us.LastName,'')  FROM UM_User us INNER JOIN AM_Entity_Role_User aeru ON us.ID=aeru.UserID AND aeru.EntityID=subtbl.Id  INNER JOIN AM_EntityTypeRoleAcl aetra ON  aeru.RoleID = aetra.ID AND  aetra.EntityTypeID=pe.TypeID AND aetra.EntityRoleID = 1),'-') as '" + attributerelationList[j].Field + "'");
                                    break;
                                case SystemDefinedAttributes.EntityStatus:
                                    mainTblQry.Append(", CASE WHEN pe.TypeID = " + (int)EntityTypeList.Objective + " THEN (SELECT case when ISNULL(po.ObjectiveStatus,0)=0 THEN 'Deactivated'  ELSE 'Active'  END from  PM_Objective po WHERE po.id=subtbl.Id) else isnull((SELECT  metso.StatusOptions FROM MM_EntityStatus mes INNER JOIN MM_EntityTypeStatus_Options metso ON mes.StatusID=metso.ID AND mes.EntityID=subtbl.id AND metso.IsRemoved=0),'-') end as '" + attributerelationList[j].Field + "'");
                                    break;
                                case SystemDefinedAttributes.EntityOnTimeStatus:
                                    mainTblQry.Append(", CASE WHEN pe.TypeID = " + (int)EntityTypeList.Objective + " THEN (SELECT case when ISNULL(po.ObjectiveStatus,0)=0 THEN '-'  ELSE '-'  END from  PM_Objective po WHERE po.id=subtbl.Id) else isnull((SELECT CASE WHEN (SELECT mes.IntimeStatus FROM MM_EntityStatus mes WHERE mes.EntityID=subtbl.id) = 0 THEN 'On time' WHEN (SELECT mes.IntimeStatus FROM MM_EntityStatus mes WHERE mes.EntityID=subtbl.id) = 1 THEN 'Delayed' WHEN (SELECT mes.IntimeStatus FROM MM_EntityStatus mes WHERE mes.EntityID=subtbl.id) = 2 THEN 'On hold' ELSE 'On time' END AS ontimestatus), '-') END AS '" + attributerelationList[j].Field + "'");
                                    break;
                                case SystemDefinedAttributes.MyRoleEntityAccess:

                                    mainTblQry.Append(", (select STUFF((SELECT',' +   ar.Caption ");
                                    mainTblQry.Append(" FROM AM_EntityTypeRoleAcl ar INNER JOIN AM_Entity_Role_User aeru ON ar.ID=aeru.RoleID  AND aeru.EntityID= pe.Id AND aeru.UserId= " + proxy.MarcomManager.User.Id + " ");
                                    mainTblQry.Append(" FOR XML PATH('')),1,1,'') AS x) AS '" + attributerelationList[j].Field + "'");
                                    break;
                                case SystemDefinedAttributes.MyRoleGlobalAccess:
                                    mainTblQry.Append(",(select STUFF((SELECT',' +   agr.Caption ");
                                    mainTblQry.Append(" FROM AM_GlobalRole agr  INNER JOIN AM_GlobalRole_User agru  ON agr.ID=agru.GlobalRoleId  AND agru.UserId= " + proxy.MarcomManager.User.Id + " ");
                                    mainTblQry.Append(" FOR XML PATH('')),1,1,'') AS x) AS '" + attributerelationList[j].Field + "'");
                                    break;
                            }
                        }
                        else if ((AttributesList)attributerelationList[j].Type == AttributesList.ListMultiSelection || (AttributesList)attributerelationList[j].Type == AttributesList.DropDownTree || (AttributesList)attributerelationList[j].Type == AttributesList.Tree || (AttributesList)attributerelationList[j].Type == AttributesList.Period || (AttributesList)attributerelationList[j].Type == AttributesList.TreeMultiSelection)
                        {
                            switch ((AttributesList)attributerelationList[j].Type)
                            {
                                case AttributesList.ListMultiSelection:

                                    if (attributerelationList[j].ID != (int)SystemDefinedAttributes.ObjectiveType)
                                    {

                                        mainTblQry.Append(" ,(SELECT  ");
                                        mainTblQry.Append(" STUFF( ");
                                        mainTblQry.Append(" ( ");
                                        mainTblQry.Append(" SELECT ', ' +  mo.Caption ");
                                        mainTblQry.Append(" FROM   MM_DAM_MultiSelectValue mms2 ");
                                        mainTblQry.Append(" INNER JOIN MM_Option mo ");
                                        mainTblQry.Append(" ON  mms2.OptionID = mo.ID and  mms2.AttributeID=" + attributerelationList[j].ID);
                                        mainTblQry.Append("  WHERE  mms2.AssetID = mms.AssetID ");
                                        mainTblQry.Append(" FOR XML PATH('') ");
                                        mainTblQry.Append(" ), ");
                                        mainTblQry.Append("  1, ");
                                        mainTblQry.Append(" 2, ");
                                        mainTblQry.Append(" '' ");
                                        mainTblQry.Append("  )               AS VALUE ");
                                        mainTblQry.Append(" FROM   MM_DAM_MultiSelectValue     mms ");
                                        mainTblQry.Append(" WHERE  mms.AssetID=subtbl.Id and  mms.AttributeID = " + CurrentattrID + " ");
                                        mainTblQry.Append(" GROUP BY ");
                                        mainTblQry.Append("  mms.AssetID) as '" + attributerelationList[j].Field + "'");
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
                                    mainTblQry.Append("   AND mtn.Level = " + attributerelationList[j].Level + " ");
                                    mainTblQry.Append("  WHERE  mtv.EntityID = subtbl.Id ");
                                    mainTblQry.Append(" AND mtv.AttributeID = " + CurrentattrID + "   ");
                                    mainTblQry.Append(" ), ");
                                    mainTblQry.Append(" '-' ");
                                    mainTblQry.Append(" ) ) as '" + attributerelationList[j].Field + "'");
                                    break;
                                case AttributesList.Tree:
                                    mainTblQry.Append(" ,'IsTree' as '" + attributerelationList[j].Field + "'");
                                    break;
                                case AttributesList.Period:
                                    mainTblQry.Append(" ,(SELECT (SELECT CONVERT(NVARCHAR(10), pep.StartDate, 120)  '@s', CONVERT(NVARCHAR(10), pep.EndDate, 120) '@e',");
                                    mainTblQry.Append(" pep.[Description] '@d', ROW_NUMBER() over(ORDER BY pep.Startdate) '@sid',");
                                    mainTblQry.Append(" pep.ID '@o'");
                                    mainTblQry.Append(" FROM   PM_EntityPeriod pep");
                                    mainTblQry.Append(" WHERE  pep.EntityID = subtbl.Id ORDER BY pep.Startdate FOR XML PATH('p'),");
                                    mainTblQry.Append(" TYPE");
                                    mainTblQry.Append(" ) FOR XML PATH('root')");
                                    mainTblQry.Append(" )  AS 'Period'");

                                    mainTblQry.Append(",(SELECT ISNULL(CAST(MIN(pep.Startdate) AS VARCHAR(10)) + '  ' + CAST(MAX(pep.EndDate)AS VARCHAR(10)),'-' )  ");
                                    mainTblQry.Append(" FROM PM_EntityPeriod pep WHERE pep.EntityID= subtbl.Id) AS TempPeriod ");

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
                                    if (LastTreeLevel == attributerelationList[j].Level)
                                    {
                                        mainTblQry.Append(" ,(SELECT  ");
                                        mainTblQry.Append(" STUFF( ");
                                        mainTblQry.Append(" ( ");
                                        mainTblQry.Append(" SELECT ', ' +  mtn.Caption ");
                                        mainTblQry.Append(" FROM   MM_TreeNode mtn ");
                                        mainTblQry.Append(" INNER JOIN MM_TreeValue mtv ");
                                        mainTblQry.Append(" ON  mtv.NodeID = mtn.ID and  mtv.AttributeID=" + attributerelationList[j].ID);
                                        mainTblQry.Append("  AND mtn.Level = " + attributerelationList[j].Level + " WHERE mtv.EntityID = subtbl.Id AND mtv.AttributeID = " + CurrentattrID + "  ");
                                        mainTblQry.Append(" FOR XML PATH('') ");
                                        mainTblQry.Append(" ), ");
                                        mainTblQry.Append("  1, ");
                                        mainTblQry.Append(" 2, ");
                                        mainTblQry.Append(" '' ");
                                        mainTblQry.Append(" ) ) as '" + attributerelationList[j].Field + "'");
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
                                        mainTblQry.Append("   AND mtn.Level = " + attributerelationList[j].Level + " ");
                                        mainTblQry.Append("  WHERE  mtv.EntityID = subtbl.Id ");
                                        mainTblQry.Append(" AND mtv.AttributeID = " + CurrentattrID + "   ");
                                        mainTblQry.Append(" ), ");
                                        mainTblQry.Append(" '-' ");
                                        mainTblQry.Append(" ) ) as '" + attributerelationList[j].Field + "'");
                                    }
                                    break;
                            }
                        }
                        else if ((AttributesList)attributerelationList[j].Type == AttributesList.Link)
                        {
                            mainTblQry.Append(",(isnull( (SELECT top 1 URL FROM CM_Links  WHERE ModuleId = 5 AND  entityid=subtbl.ID),'-') ) as '" + attributerelationList[j].Field + "'");
                        }
                        else if ((AttributesList)attributerelationList[j].Type == AttributesList.ListSingleSelection)
                        {
                            mainTblQry.Append(",(isnull( (SELECT top 1 caption FROM MM_Option  WHERE AttributeID=" + CurrentattrID + " AND id=subtbl.Attr_" + CurrentattrID + "),'-') ) as '" + attributerelationList[j].Field + "'");
                        }
                        else if ((AttributesList)attributerelationList[j].Type == AttributesList.CheckBoxSelection)
                        {
                            mainTblQry.Append(" ,isnull(cast(subtbl.attr_" + CurrentattrID + " as varchar(50)), '-') as '" + attributerelationList[j].Field + "'");
                        }
                        else if ((AttributesList)attributerelationList[j].Type == AttributesList.DateTime)
                        {
                            mainTblQry.Append(" ,REPLACE(CONVERT(varchar,isnull(subtbl.attr_" + CurrentattrID + " ,''),121),'1900-01-01 00:00:00.000', '-') as '" + attributerelationList[j].Field + "'");
                        }
                        else if ((AttributesList)attributerelationList[j].Type == AttributesList.ParentEntityName)
                        {
                            mainTblQry.Append(" ,isnull((SELECT top 1 pe2.name  + '!@#' + met.ShortDescription + '!@#' + met.ColorCode FROM PM_Entity pe2 INNER JOIN MM_EntityType met ON pe2.TypeID=met.ID  WHERE  pe2.id=pe.parentid), '-') as '" + attributerelationList[j].Field + "'");
                        }
                        else
                        {
                            mainTblQry.Append(" ,isnull(subtbl.attr_" + CurrentattrID + " , '-') as '" + attributerelationList[j].Field + "'");
                        }

                    }
                    mainTblQry.AppendLine(" from DAM_Asset pe inner join  ");
                    mainTblQry.AppendLine(damQuery.ToString());
                    mainTblQry.AppendLine("  on subtbl.id=pe.Id  where pe.IsPublish=1");

                    IList dynamicData = tx.PersistenceManager.DamRepository.ExecuteQuery(mainTblQry.ToString());

                    returnObj.Add(new
                    {
                        AssetFiles = assets,
                        AssetTypeAttrRel = attributerelationList,
                        AssetDynData = dynamicData
                    });


                    return returnObj;
                }
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public int AddPublishedFilesToDAM(DigitalAssetManagerProxy proxy, int[] assetid, int EntityID, int FolderID)
        {
            int newasset = 0;
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {

                    if (assetid.Length > 0)
                    {
                        for (int i = 0; i < assetid.Length; i++)
                        {

                            List<IAssets> assetdet = new List<IAssets>();
                            IAssets asset = new Assets();
                            asset = GetAssetAttributesDetails(proxy, assetid[i], false);


                            IList<IAttributeData> AttributeDatanew = new List<IAttributeData>();
                            AttributeDatanew = asset.AttributeData;
                            if (asset.Category == 0)
                            {
                                var Filesassest = asset.Files.Where(a => a.ID == asset.ActiveFileID).Select(a => a).ToList();


                                newasset = CreateAsset(proxy, FolderID, Convert.ToInt32(asset.AssetTypeid), asset.Name, AttributeDatanew, Filesassest[0].Name, 1, Filesassest[0].MimeType, Filesassest[0].Extension, Convert.ToInt64(Filesassest[0].Size), EntityID, Filesassest[0].Fileguid.ToString(), Filesassest[0].Description, true, Filesassest[0].Status, assetid[i]);
                            }
                            else
                            {
                                newasset = CreateBlankAsset(proxy, FolderID, Convert.ToInt32(asset.AssetTypeid), asset.Name, AttributeDatanew, EntityID, asset.Category, asset.Url, true, assetid[i]);
                                //newasset = CreateAsset(proxy, Convert.ToInt32(asset.FolderID), Convert.ToInt32(asset.AssetTypeid), "Copy of " + asset.Name, AttributeDatanew, Filesassest[0].Name, Convert.ToInt32(Filesassest[0].VersionNo), Filesassest[0].MimeType, Filesassest[0].Extension, Convert.ToInt64(Filesassest[0].Size), Convert.ToInt32(proxy.MarcomManager.User.Id), Convert.ToDateTime(d2), 5, Convert.ToInt32(asset.EntityID), Filesassest[0].Fileguid.ToString(), Filesassest[0].Description, true);

                            }
                        }
                        tx.Commit();
                        return newasset;

                    }

                }
            }
            catch (Exception ex)
            {
                return 0;
            }

            return newasset;
        }


        public void CreateversionDOCGeneratedAsset(int entityID, int folderID, int assetTypeID, int createdBy, string intialfilename, string assetName, int assetId)
        {
            int fileid = 0;
            DAMFileDao filedetails = new DAMFileDao();
            try
            {
                Guid NewId = Guid.NewGuid();
                string filePath = ReadAdminXML("FileManagment");
                var DirInfo = System.IO.Directory.GetParent(filePath);
                string newFilePath = DirInfo.FullName;
                System.IO.File.Copy(filePath + "\\" + intialfilename, newFilePath + "\\" + NewId + ".docx");
                System.IO.File.Copy(filePath + "\\" + intialfilename.Replace(".docx", ".pdf"), newFilePath + "\\" + NewId + ".pdf");
                Guid userSession = MarcomManagerFactory.GetSystemSession();
                IMarcomManager managers = MarcomManagerFactory.GetMarcomManager(userSession);
                using (ITransaction tx = managers.GetTransaction())
                {
                    FileInfo f2 = new FileInfo(filePath + "\\" + intialfilename);
                    var FileObj = tx.PersistenceManager.DamRepository.Query<DAMFileDao>().ToList().Where(item => item.AssetID == assetId).Select(item => item.VersionNo).Max();
                    filedetails.AssetID = assetId;
                    filedetails.Status = 0;
                    filedetails.MimeType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                    filedetails.Size = f2.Length;
                    filedetails.FileGuid = NewId;
                    filedetails.CreatedOn = DateTime.UtcNow;
                    filedetails.Extension = Path.GetExtension(filePath + "\\" + intialfilename);
                    filedetails.Name = Convert.ToString(NewId);
                    filedetails.VersionNo = FileObj + 1;
                    filedetails.Description = "";
                    filedetails.OwnerID = createdBy;
                    tx.PersistenceManager.DamRepository.Save<DAMFileDao>(filedetails);
                    fileid = filedetails.ID;
                    DateTime d1 = DateTime.UtcNow;
                    tx.PersistenceManager.DamRepository.ExecuteQuerywithMinParam("UPDATE DAM_Asset SET ActiveFileID = ? ,UpdatedOn=? WHERE ID = ?", fileid, d1, assetId);
                    tx.Commit();
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        public string getkeyvaluefromwebconfig(DigitalAssetManagerProxy proxy, string key)
        {
            try
            {
                //return Path.Combine(HttpRuntime.AppDomainAppPath, ConfigurationManager.AppSettings[key]);
                return ConfigurationManager.AppSettings[key].ToString();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public string jsonText { get; set; }
        /// <summary>
        /// GetBreadCrumFolderPath
        /// </summary>
        /// <param name="proxy"></param>
        /// <param name="EntityID"></param>
        /// <param name="CurrentFolderID"></param>
        /// <returns>returns list of folder path</returns>
        public IList GetBreadCrumFolderPath(DigitalAssetManagerProxy proxy, int EntityID, int CurrentFolderID)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    StringBuilder strpath = new StringBuilder();

                    IList<MultiProperty> assetQuery_parLIST = new List<MultiProperty>();
                    assetQuery_parLIST.Add(new MultiProperty { propertyName = "Entityid", propertyValue = EntityID });
                    assetQuery_parLIST.Add(new MultiProperty { propertyName = "ID", propertyValue = CurrentFolderID });

                    strpath.Append(" WITH GetFolder ");
                    strpath.Append(" AS ");
                    strpath.Append(" ( ");
                    strpath.Append(" SELECT df.caption,df.ParentNodeID,df.ID,df.NodeID,df.[Level],df.SortOrder,df.Entityid ");
                    strpath.Append(" FROM DAM_Folder df WHERE df.Entityid=:Entityid AND df.ID=:ID ");
                    strpath.Append(" UNION ALL ");
                    strpath.Append(" SELECT df2.caption,df2.ParentNodeID,df2.ID,df2.NodeID,df2.[Level],df2.SortOrder,df2.Entityid ");
                    strpath.Append(" FROM DAM_Folder df2 ");
                    strpath.Append("              INNER JOIN GetFolder  AS Child   ON  df2.id = Child.ParentNodeID ");
                    strpath.Append(" ) ");
                    strpath.Append(" SELECT * ");
                    strpath.Append(" FROM   GetFolder  ");
                    strpath.Append(" ORDER BY ");
                    strpath.Append(" Level  ");
                    IList breadcrumpath = tx.PersistenceManager.DamRepository.ExecuteQuerywithParam(strpath.ToString(), assetQuery_parLIST);
                    return breadcrumpath;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool AddNewsFeedinfo(DigitalAssetManagerProxy proxy, int assetId, string action, string TypeName)
        {

            using (ITransaction tx = proxy.MarcomManager.GetTransaction())
            {

                IList<DAMFileDao> filesinfo = new List<DAMFileDao>();
                var assetactivefiles = tx.PersistenceManager.DamRepository.Query<AssetsDao>().Where(a => a.ID == assetId).Select(a => a).FirstOrDefault();
                if (assetactivefiles.ActiveFileID > 0)
                    filesinfo = tx.PersistenceManager.DamRepository.Query<DAMFileDao>().Where(a => a.ID == assetactivefiles.ActiveFileID).Select(a => a).ToList();
                BrandSystems.Marcom.Core.Utility.FeedNotificationServer fs = new Utility.FeedNotificationServer();
                NotificationFeedObjects obj = new NotificationFeedObjects();
                obj.action = action;
                obj.Actorid = proxy.MarcomManager.User.Id;
                obj.EntityId = assetactivefiles.EntityID;
                obj.AttributeName = assetactivefiles.Name;
                obj.TypeName = TypeName;
                obj.CreatedOn = DateTimeOffset.Now;
                obj.AssociatedEntityId = assetId;
                if (assetactivefiles.ActiveFileID > 0)
                    obj.Version = filesinfo[0].VersionNo;
                else
                    obj.Version = 1;
                fs.AsynchronousNotify(obj);
                tx.Commit();
            }
            return true;
        }

        /// <summary>
        /// GetAllFolderStructure
        /// </summary>
        /// <param name="proxy"></param>
        /// <param name="EntityID"></param>
        /// <returns>function to return folder structure for the entity</returns>
        public IList<object> GetAllFolderStructure(DigitalAssetManagerProxy proxy, int EntityID)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    IList<object> obj = new List<object>();
                    IList<FolderDao> allfolders = new List<FolderDao>();
                    FolderDao tempfolder = new FolderDao();
                    tempfolder.Id = 0;
                    tempfolder.NodeID = 0;
                    tempfolder.ParentNodeID = 0;
                    tempfolder.Level = 0;
                    tempfolder.KEY = "0";
                    tempfolder.EntityID = EntityID;
                    tempfolder.Caption = "Digital Asset";
                    tempfolder.SortOrder = 0;
                    tempfolder.Colorcode = "ffd300";

                    allfolders = tx.PersistenceManager.DamRepository.GetEquals<FolderDao>("EntityID", EntityID);
                    //allfolders.Add(tempfolder);

                    allfolders = (from a in allfolders select a).OrderBy(a => a.Id).ToList<FolderDao>();
                    foreach (var item in allfolders)
                    {
                        IList<MultiProperty> assetQuery_parLIST = new List<MultiProperty>();
                        assetQuery_parLIST.Add(new MultiProperty { propertyName = "Entityid", propertyValue = EntityID });
                        assetQuery_parLIST.Add(new MultiProperty { propertyName = "ID", propertyValue = item.Id });

                        StringBuilder sbfolder = new StringBuilder();
                        sbfolder.Append("WITH GetFolder ");
                        sbfolder.Append(" AS ");
                        sbfolder.Append(" ( ");
                        sbfolder.Append(" SELECT df.ID,df.caption,df.ParentNodeID,df.NodeID,df.[Level],df.SortOrder,df.[KEY],df.Entityid,df.Colorcode ");
                        sbfolder.Append(" FROM DAM_Folder df WHERE df.Entityid=:Entityid AND df.ID=:ID ");
                        sbfolder.Append(" UNION ALL ");
                        sbfolder.Append(" SELECT df2.ID,df2.caption,df2.ParentNodeID,df2.NodeID,df2.[Level],df2.SortOrder,df2.[KEY],df2.Entityid,df2.Colorcode ");
                        sbfolder.Append(" FROM DAM_Folder df2 ");
                        sbfolder.Append(" INNER JOIN GetFolder  AS Child   ON  df2.id = Child.ParentNodeID ");
                        sbfolder.Append(" ) ");
                        sbfolder.Append(" SELECT DISTINCT * ");
                        sbfolder.Append(" FROM   GetFolder  ");
                        //sbfolder.Append(" UNION ALL ");
                        //sbfolder.Append(" SELECT 0,'Digital Asset',0,0,0,0,'0'," + EntityID + " ");
                        sbfolder.Append(" ORDER BY ");
                        sbfolder.Append(" ID  ");

                        IList breadcrumpath = tx.PersistenceManager.DamRepository.ExecuteQuerywithParam(sbfolder.ToString(), assetQuery_parLIST);
                        IList<FolderDao> folderlist = new List<FolderDao>();
                        foreach (var fold in breadcrumpath)
                        {
                            FolderDao folder = new FolderDao();
                            folder.Id = (int)((System.Collections.Hashtable)(fold))["ID"];
                            folder.NodeID = (int)((System.Collections.Hashtable)(fold))["NodeID"];
                            folder.ParentNodeID = (int)((System.Collections.Hashtable)(fold))["ParentNodeID"];
                            folder.Level = (int)((System.Collections.Hashtable)(fold))["Level"];
                            folder.KEY = ((System.Collections.Hashtable)(fold))["KEY"].ToString();
                            folder.EntityID = (int)((System.Collections.Hashtable)(fold))["Entityid"];
                            folder.Caption = ((System.Collections.Hashtable)(fold))["caption"].ToString();
                            folder.SortOrder = (int)((System.Collections.Hashtable)(fold))["SortOrder"];
                            folder.Colorcode = (((System.Collections.Hashtable)(fold))["Colorcode"] != null) ? ((System.Collections.Hashtable)(fold))["Colorcode"].ToString() : "ffd300";
                            folderlist.Add(folder);
                        }
                        obj.Add(folderlist);
                    }
                    return obj;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public bool SaveCropedImageForLightBox(DigitalAssetManagerProxy proxy, string sourcepath, int imgwidth, int imgheight, int imgX, int imgY)
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
                                    string destinationpath = HttpContext.Current.Server.MapPath("~/DAMFiles/Temp/" + sourcepath.ToString().Split('/')[2]);
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
                return false;
            }


        }

        public List<object> GetMediaAssets(DigitalAssetManagerProxy proxy, int viewType, int orderbyid, int pageNo, int[] assettypeArr, string filterSchema = null)
        {

            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    List<object> returnObj = new List<object>();

                    XDocument xd = new XDocument();
                    filterSchema = filterSchema.Replace("\"<\"", "\"&lt;\"");
                    xd = XDocument.Parse(filterSchema);


                    string viewName = Enum.GetName(typeof(AssetView), viewType);
                    int totalrecords = 18;
                    if (viewType == (int)AssetView.ListView)
                        totalrecords = 18;
                    if (viewType == (int)AssetView.SummaryView)
                        totalrecords = 20;
                    StringBuilder assetQuery = new StringBuilder();



                    int version = MarcomManagerFactory.ActiveMetadataVersionNumber;
                    string xmlpath = string.Empty;

                    xmlpath = tx.PersistenceManager.MetadataRepository.GetXmlPath(version);
                    var xmetadataDoc = XDocument.Load(xmlpath);

                    string Adminxmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
                    XDocument xDoc = XDocument.Load(Adminxmlpath);

                    IList<XElement> tempresults = new List<XElement>();
                    IList<XElement> result = new List<XElement>();

                    if (viewType == (int)AssetView.ThumbnailView)
                    {
                        tempresults = xDoc.Descendants("DAMsettings").Descendants(viewName).Descendants("AssetType").Descendants("Attributes").Descendants("Attribute").Where(a => Convert.ToBoolean(a.Element("IsColumn").Value) == true).Select(a => a).ToList();
                        foreach (var rec in tempresults)
                        {
                            IList<XElement> duplist = new List<XElement>();
                            duplist = result.Where(a => Convert.ToInt32(a.Element("Id").Value) == Convert.ToInt32(rec.Element("Id").Value)).Select(a => a).ToList();
                            if (duplist.Count == 0)
                                result.Add(rec);
                        }
                    }
                    else if (viewType == (int)AssetView.SummaryView)
                    {
                        IList<XElement> thumpnail = xDoc.Descendants("DAMsettings").Descendants("ThumbnailView").Descendants("AssetType").Descendants("Attributes").Descendants("Attribute").Where(a => Convert.ToBoolean(a.Element("IsColumn").Value) == true).Select(a => a).ToList();
                        IList<XElement> summaryList = xDoc.Descendants("DAMsettings").Descendants(viewName).Descendants("AssetType").Descendants("Attributes").Descendants("Attribute").Where(a => Convert.ToBoolean(a.Element("IsColumn").Value) == true).Select(a => a).ToList();
                        IList<XElement> tempList = new List<XElement>(thumpnail.Concat(summaryList));
                        foreach (var rec in tempList)
                        {
                            IList<XElement> duplist = new List<XElement>();
                            duplist = result.Where(a => Convert.ToInt32(a.Element("Id").Value) == Convert.ToInt32(rec.Element("Id").Value)).Select(a => a).ToList();
                            if (duplist.Count == 0)
                                result.Add(rec);
                        }

                    }
                    else if (viewType == (int)AssetView.ListView)
                    {
                        tempresults = xDoc.Descendants("DAMsettings").Descendants(viewName).Descendants("AssetType").Descendants("Attributes").Descendants("Attribute").Where(a => Convert.ToBoolean(a.Element("IsColumn").Value) == true).Select(a => a).ToList();
                        foreach (var rec in tempresults)
                        {
                            IList<XElement> duplist = new List<XElement>();
                            duplist = result.Where(a => Convert.ToInt32(a.Element("Id").Value) == Convert.ToInt32(rec.Element("Id").Value)).Select(a => a).ToList();
                            if (duplist.Count == 0)
                                result.Add(rec);
                        }
                    }

                    int[] attrsidarr = result.Distinct().Select(a => Convert.ToInt32(a.Element("Id").Value)).ToArray();

                    IList<EntityTypeAttributeRelationDao> assetEntityAttributes = new List<EntityTypeAttributeRelationDao>();
                    assetEntityAttributes = (from attrbs in tx.PersistenceManager.MetadataRepository.Query<EntityTypeAttributeRelationDao>() where assettypeArr.Contains(attrbs.EntityTypeID) select attrbs).ToList<EntityTypeAttributeRelationDao>();
                    int[] assetAttributes = assetEntityAttributes.Distinct().Select(a => a.AttributeID).ToArray();
                    int[] mediadistinctAttrs = assetAttributes.Distinct().ToArray();
                    IList<AttributeDao> mediaAttributes = new List<AttributeDao>();
                    mediaAttributes = (from attrbs in tx.PersistenceManager.MetadataRepository.Query<AttributeDao>() where mediadistinctAttrs.Contains(attrbs.Id) select attrbs).ToList<AttributeDao>();



                    IList<AttributeDao> attributes = new List<AttributeDao>();
                    attributes = (from attrbs in tx.PersistenceManager.MetadataRepository.Query<AttributeDao>() where attrsidarr.Contains(attrbs.Id) select attrbs).ToList<AttributeDao>();

                    var attributerelationList = (from AdminAttributes in result
                                                 join ser in attributes on Convert.ToInt16(AdminAttributes.Element("Id").Value) equals ser.Id
                                                 select new
                                                 {
                                                     ID = Convert.ToInt16(AdminAttributes.Element("Id").Value),
                                                     SotOrder = Convert.ToInt32(AdminAttributes.Element("SortOrder").Value),
                                                     Type = ser.AttributeTypeID,
                                                     IsSpecial = ser.IsSpecial,
                                                     Field = ser.Id,
                                                     Level = Convert.ToInt16(AdminAttributes.Element("Level").Value),
                                                     Caption = AdminAttributes.Element("DisplayName").Value,
                                                 }).Distinct().ToList();

                    IList<DamAttributeList> mediaAttributeEelationList = new List<DamAttributeList>();

                    mediaAttributeEelationList = (from AdminAttributes in xDoc.Descendants("publishedsearchcriteria").Descendants("Attributes").Descendants("Attribute")
                                                  join ser in tx.PersistenceManager.MetadataRepository.Query<AttributeDao>() on Convert.ToInt16(AdminAttributes.Element("Id").Value) equals ser.Id
                                                  select new DamAttributeList
                                                  {
                                                      ID = Convert.ToInt16(AdminAttributes.Element("Id").Value),
                                                      SortOrder = Convert.ToInt32(AdminAttributes.Element("SortOrder").Value),
                                                      Type = ser.AttributeTypeID,
                                                      IsSpecial = ser.IsSpecial,
                                                      Field = ser.Id,
                                                      Level = Convert.ToInt16(AdminAttributes.Element("Level").Value),
                                                      Caption = AdminAttributes.Element("DisplayName").Value,
                                                  }).Distinct().ToList();

                    IEnumerable<DamAttributeList> attributerelationList1 = (from attr in mediaAttributeEelationList
                                                                            join ser in mediaAttributes on attr.ID equals ser.Id
                                                                            select new DamAttributeList
                                                                            {
                                                                                ID = attr.ID,
                                                                                SortOrder = attr.SortOrder,
                                                                                Type = attr.Type,
                                                                                IsSpecial = attr.IsSpecial,
                                                                                Field = attr.Field,
                                                                                Level = attr.Level,
                                                                                Caption = attr.Caption
                                                                            }).Distinct().ToList();

                    IList<XElement> criteriaLsts = xd.Descendants("CustomList").Descendants("Criterias").Descendants("Criteria").ToList();


                    IList filtereddata = new List<IQueryable>();
                    if (criteriaLsts.Count > 0)
                        try { filtereddata = Filtered_Published_Assets(proxy, filterSchema, attributerelationList1, assettypeArr, criteriaLsts); }
                        catch { }


                    assetQuery.AppendLine(" DECLARE @RowsPerPage INT = " + totalrecords + ", ");
                    assetQuery.AppendLine(" @PageNumber INT =" + pageNo + " ");
                    assetQuery.AppendLine(" DECLARE @OrderBy INT = " + orderbyid + "; ");
                    assetQuery.AppendLine(" SELECT a.ID AS 'FileUniqueID', ");
                    assetQuery.AppendLine(" a.Name as 'FileName', ");
                    assetQuery.AppendLine(" a.Extension, ");
                    assetQuery.AppendLine(" a.[Size],  a.[VersionNo], ");
                    assetQuery.AppendLine(" a.OwnerID, ");
                    assetQuery.AppendLine(" a.CreatedOn, ");
                    assetQuery.AppendLine(" a.FileGuid, ");
                    assetQuery.AppendLine(" a.[Description], ");
                    assetQuery.AppendLine(" a.AssetID, ");
                    assetQuery.AppendLine(" da.FolderID, ");
                    assetQuery.AppendLine(" da.EntityID, ");
                    assetQuery.AppendLine(" da.ID AS 'AssetUniqueID', ");
                    assetQuery.AppendLine(" da.Name as 'AssetName', da.Url as 'LinkURL', a.MimeType as 'MimeType', ");
                    assetQuery.AppendLine(" da.AssetTypeid, ");
                    assetQuery.AppendLine(" da.ActiveFileID, ");
                    assetQuery.AppendLine(" met.ColorCode, ");
                    assetQuery.AppendLine(" met.ShortDescription, ");
                    assetQuery.AppendLine(" a.[Status], ");
                    assetQuery.AppendLine(" da.Category, da.IsPublish, ISNULL(da.LinkedAssetID,0) as LinkedAssetID ,");
                    assetQuery.AppendLine(" COUNT(*) OVER()              AS 'Total_COUNT'");
                    assetQuery.AppendLine(" FROM   DAM_File a ");
                    assetQuery.AppendLine(" RIGHT OUTER JOIN DAM_Asset da ");
                    assetQuery.AppendLine(" ON  a.AssetID = da.ID AND a.ID=da.ActiveFileID");
                    assetQuery.AppendLine(" INNER JOIN MM_EntityType met ");
                    assetQuery.AppendLine(" ON  met.ID = da.AssetTypeid ");
                    assetQuery.AppendLine(" WHERE  da.id IN (SELECT da2.id ");
                    assetQuery.AppendLine(" FROM   DAM_Asset da2 ");
                    assetQuery.AppendLine(" WHERE ");
                    assetQuery.AppendLine(" da2.IsPublish = 1 ");
                    if (criteriaLsts.Count > 0)
                    {
                        if (filtereddata != null)
                        {
                            int[] assetidArr = { };
                            if (filtereddata.Count > 0)
                            {
                                assetidArr = filtereddata.Cast<Hashtable>().Select(a => (int)a["ID"]).ToArray();
                                if (assetidArr != null)
                                {
                                    string AIDinClause = "("
                                           + String.Join(",", assetidArr.Select(x => x.ToString()).Distinct().ToArray())
                                         + ")";
                                    assetQuery.AppendLine("AND da2.ID IN " + AIDinClause + "");

                                }
                            }
                            else
                            {
                                assetQuery.AppendLine("AND da2.ID IN (-100) ");
                            }
                        }
                        else
                        {
                            assetQuery.AppendLine("AND da2.ID IN (-100) ");
                        }
                    }
                    else
                    {
                        string TypeIDinClause = "("
                                        + String.Join(",", assettypeArr.Select(x => x.ToString()).Distinct().ToArray())
                                      + ")";
                        assetQuery.AppendLine("AND da2.assettypeid IN " + TypeIDinClause + "");
                    }
                    assetQuery.AppendLine("   )  ");
                    assetQuery.AppendLine("  ORDER BY  ");
                    assetQuery.AppendLine(" CASE   ");
                    assetQuery.AppendLine(" WHEN @OrderBy = 1  ");
                    assetQuery.AppendLine(" THEN da.Name  ");
                    assetQuery.AppendLine(" END       asc,  ");
                    assetQuery.AppendLine(" CASE   ");
                    assetQuery.AppendLine(" WHEN @OrderBy = 2  ");
                    assetQuery.AppendLine(" THEN da.Name  ");
                    assetQuery.AppendLine(" END desc,  ");
                    assetQuery.AppendLine(" CASE   ");
                    assetQuery.AppendLine(" WHEN @OrderBy = 3  ");
                    assetQuery.AppendLine(" THEN da.Createdon   ");
                    assetQuery.AppendLine(" END       asc , ");
                    assetQuery.AppendLine(" CASE   ");
                    assetQuery.AppendLine(" WHEN @OrderBy = 4 ");
                    assetQuery.AppendLine(" THEN da.Createdon   ");
                    assetQuery.AppendLine(" END       desc  ");

                    assetQuery.AppendLine(" OFFSET(@PageNumber - 1) * @RowsPerPage ROWS ");

                    assetQuery.AppendLine(" FETCH NEXT @RowsPerPage ROWS ONLY ");


                    IList assets = tx.PersistenceManager.DamRepository.ExecuteQuery(assetQuery.ToString());

                    var typeidArr = assets.Cast<Hashtable>().Select(a => (int)a["AssetTypeid"]).Distinct().ToArray();

                    var idArr = assets.Cast<Hashtable>().Select(a => (int)a["AssetUniqueID"]).ToArray();

                    IList<EntityTypeAttributeRelationDao> entityAttributes = new List<EntityTypeAttributeRelationDao>();
                    entityAttributes = (from attrbs in assetEntityAttributes where typeidArr.Contains(attrbs.EntityTypeID) select attrbs).ToList<EntityTypeAttributeRelationDao>();



                    int[] attrSelectType = { 1, 2, 3, 5 };

                    StringBuilder damQuery = new StringBuilder();
                    damQuery.AppendLine("(");
                    int EntitypeLenghth = typeidArr.Distinct().Count();
                    for (var i = 0; i < typeidArr.Distinct().Count(); i++)
                    {
                        //if (typeidArr[i] != 34 && typeidArr[i] != 33)
                        //{
                        damQuery.AppendLine("select ID");
                        foreach (var currentval in attributerelationList.Where(a => a.IsSpecial != true && attrSelectType.Contains(a.Type)).ToList())
                        {
                            int val = entityAttributes.ToList().Where(a => a.EntityTypeID == typeidArr[i] && a.AttributeID == currentval.ID).Count();
                            if (val != 0)
                                damQuery.AppendLine(" ,Attr_" + currentval.ID + " as 'Attr_" + currentval.ID + "'");
                            else
                                damQuery.AppendLine(" ,'-' as  'Attr_" + currentval.ID + "'");
                        }
                        damQuery.AppendLine(" from MM_AttributeRecord_" + typeidArr[i]);
                        if (i < EntitypeLenghth - 1)
                        {
                            damQuery.AppendLine(" union ");
                        }
                        //}
                    }
                    damQuery.AppendLine(") subtbl");

                    StringBuilder mainTblQry = new StringBuilder();
                    mainTblQry.AppendLine("select subtbl.ID  ");
                    int LastTreeLevel = attributerelationList.Where(a => (AttributesList)a.Type == AttributesList.TreeMultiSelection).OrderByDescending(a => a.Level).Select(a => a.Level).FirstOrDefault();
                    for (int j = 0; j < attributerelationList.Count(); j++)
                    {
                        string CurrentattrID = attributerelationList[j].ID.ToString();
                        if (attributerelationList[j].IsSpecial == true)
                        {
                            switch ((SystemDefinedAttributes)attributerelationList[j].ID)
                            {
                                case SystemDefinedAttributes.Name:
                                    mainTblQry.AppendLine(",pe.Name  as '" + attributerelationList[j].Field + "'");
                                    break;
                                case SystemDefinedAttributes.Owner:
                                    mainTblQry.Append(",ISNULL( (SELECT top 1  ISNULL(us.FirstName,'') + ' ' + ISNULL(us.LastName,'')  FROM UM_User us INNER JOIN AM_Entity_Role_User aeru ON us.ID=aeru.UserID AND aeru.EntityID=subtbl.Id  INNER JOIN AM_EntityTypeRoleAcl aetra ON  aeru.RoleID = aetra.ID AND  aetra.EntityTypeID=pe.TypeID AND aetra.EntityRoleID = 1),'-') as '" + attributerelationList[j].Field + "'");
                                    break;
                                case SystemDefinedAttributes.EntityStatus:
                                    mainTblQry.Append(", CASE WHEN pe.TypeID = " + (int)EntityTypeList.Objective + " THEN (SELECT case when ISNULL(po.ObjectiveStatus,0)=0 THEN 'Deactivated'  ELSE 'Active'  END from  PM_Objective po WHERE po.id=subtbl.Id) else isnull((SELECT  metso.StatusOptions FROM MM_EntityStatus mes INNER JOIN MM_EntityTypeStatus_Options metso ON mes.StatusID=metso.ID AND mes.EntityID=subtbl.id AND metso.IsRemoved=0),'-') end as '" + attributerelationList[j].Field + "'");
                                    break;
                                case SystemDefinedAttributes.EntityOnTimeStatus:
                                    mainTblQry.Append(", CASE WHEN pe.TypeID = " + (int)EntityTypeList.Objective + " THEN (SELECT case when ISNULL(po.ObjectiveStatus,0)=0 THEN '-'  ELSE '-'  END from  PM_Objective po WHERE po.id=subtbl.Id) else isnull((SELECT CASE WHEN (SELECT mes.IntimeStatus FROM MM_EntityStatus mes WHERE mes.EntityID=subtbl.id) = 0 THEN 'On time' WHEN (SELECT mes.IntimeStatus FROM MM_EntityStatus mes WHERE mes.EntityID=subtbl.id) = 1 THEN 'Delayed' WHEN (SELECT mes.IntimeStatus FROM MM_EntityStatus mes WHERE mes.EntityID=subtbl.id) = 2 THEN 'On hold' ELSE 'On time' END AS ontimestatus), '-') END AS '" + attributerelationList[j].Field + "'");
                                    break;
                                case SystemDefinedAttributes.MyRoleEntityAccess:

                                    mainTblQry.Append(", (select STUFF((SELECT',' +   ar.Caption ");
                                    mainTblQry.Append(" FROM AM_EntityTypeRoleAcl ar INNER JOIN AM_Entity_Role_User aeru ON ar.ID=aeru.RoleID  AND aeru.EntityID= pe.Id AND aeru.UserId= " + proxy.MarcomManager.User.Id + " ");
                                    mainTblQry.Append(" FOR XML PATH('')),1,1,'') AS x) AS '" + attributerelationList[j].Field + "'");
                                    break;
                                case SystemDefinedAttributes.MyRoleGlobalAccess:
                                    mainTblQry.Append(",(select STUFF((SELECT',' +   agr.Caption ");
                                    mainTblQry.Append(" FROM AM_GlobalRole agr  INNER JOIN AM_GlobalRole_User agru  ON agr.ID=agru.GlobalRoleId  AND agru.UserId= " + proxy.MarcomManager.User.Id + " ");
                                    mainTblQry.Append(" FOR XML PATH('')),1,1,'') AS x) AS '" + attributerelationList[j].Field + "'");
                                    break;
                            }
                        }
                        else if ((AttributesList)attributerelationList[j].Type == AttributesList.ListMultiSelection || (AttributesList)attributerelationList[j].Type == AttributesList.DropDownTree || (AttributesList)attributerelationList[j].Type == AttributesList.Tree || (AttributesList)attributerelationList[j].Type == AttributesList.Period || (AttributesList)attributerelationList[j].Type == AttributesList.TreeMultiSelection)
                        {
                            switch ((AttributesList)attributerelationList[j].Type)
                            {
                                case AttributesList.ListMultiSelection:

                                    if (attributerelationList[j].ID != (int)SystemDefinedAttributes.ObjectiveType)
                                    {

                                        mainTblQry.Append(" ,(SELECT  ");
                                        mainTblQry.Append(" STUFF( ");
                                        mainTblQry.Append(" ( ");
                                        mainTblQry.Append("  SELECT ', ' + mo.Caption ");
                                        mainTblQry.Append(" FROM MM_Option mo     ");
                                        mainTblQry.Append(" INNER JOIN MM_DAM_MultiSelectValue mms2  ");
                                        mainTblQry.Append("  ON  mo.ID = mms2.OptionID ");
                                        mainTblQry.Append(" AND mms2.AttributeID = " + attributerelationList[j].ID);
                                        mainTblQry.Append(" WHERE  mms2.AssetID = mms.AssetID FOR XML PATH('') ");
                                        mainTblQry.Append(" ), ");
                                        mainTblQry.Append("  1, ");
                                        mainTblQry.Append(" 2, ");
                                        mainTblQry.Append(" '' ");
                                        mainTblQry.Append("  )               AS VALUE ");
                                        mainTblQry.Append(" FROM   MM_DAM_MultiSelectValue     mms ");
                                        mainTblQry.Append(" WHERE  mms.AssetID=subtbl.Id and  mms.AttributeID = " + CurrentattrID + " ");
                                        mainTblQry.Append(" GROUP BY ");
                                        mainTblQry.Append("  mms.AssetID) as '" + attributerelationList[j].Field + "'");
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
                                    mainTblQry.Append("   AND mtn.Level = " + attributerelationList[j].Level + " ");
                                    mainTblQry.Append("  WHERE  mtv.EntityID = subtbl.Id ");
                                    mainTblQry.Append(" AND mtv.AttributeID = " + CurrentattrID + "   ");
                                    mainTblQry.Append(" ), ");
                                    mainTblQry.Append(" '-' ");
                                    mainTblQry.Append(" ) ) as '" + attributerelationList[j].Field + "'");
                                    break;
                                case AttributesList.Tree:
                                    mainTblQry.Append(" ,'IsTree' as '" + attributerelationList[j].Field + "'");
                                    break;
                                case AttributesList.Period:
                                    mainTblQry.Append(" ,(SELECT (SELECT CONVERT(NVARCHAR(10), pep.StartDate, 120)  '@s', CONVERT(NVARCHAR(10), pep.EndDate, 120) '@e',");
                                    mainTblQry.Append(" pep.[Description] '@d', ROW_NUMBER() over(ORDER BY pep.Startdate) '@sid',");
                                    mainTblQry.Append(" pep.ID '@o'");
                                    mainTblQry.Append(" FROM   PM_EntityPeriod pep");
                                    mainTblQry.Append(" WHERE  pep.EntityID = subtbl.Id ORDER BY pep.Startdate FOR XML PATH('p'),");
                                    mainTblQry.Append(" TYPE");
                                    mainTblQry.Append(" ) FOR XML PATH('root')");
                                    mainTblQry.Append(" )  AS 'Period'");

                                    mainTblQry.Append(",(SELECT ISNULL(CAST(MIN(pep.Startdate) AS VARCHAR(10)) + '  ' + CAST(MAX(pep.EndDate)AS VARCHAR(10)),'-' )  ");
                                    mainTblQry.Append(" FROM PM_EntityPeriod pep WHERE pep.EntityID= subtbl.Id) AS TempPeriod ");

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
                                    if (LastTreeLevel == attributerelationList[j].Level)
                                    {
                                        mainTblQry.Append(" ,(SELECT  ");
                                        mainTblQry.Append(" STUFF( ");
                                        mainTblQry.Append(" ( ");
                                        mainTblQry.Append(" SELECT ', ' +  mtn.Caption ");
                                        mainTblQry.Append(" FROM   MM_TreeNode mtn ");
                                        mainTblQry.Append(" INNER JOIN MM_TreeValue mtv ");
                                        mainTblQry.Append(" ON  mtv.NodeID = mtn.ID and  mtv.AttributeID=" + attributerelationList[j].ID);
                                        mainTblQry.Append("  AND mtn.Level = " + attributerelationList[j].Level + " WHERE mtv.EntityID = subtbl.Id AND mtv.AttributeID = " + CurrentattrID + "  ");
                                        mainTblQry.Append(" FOR XML PATH('') ");
                                        mainTblQry.Append(" ), ");
                                        mainTblQry.Append("  1, ");
                                        mainTblQry.Append(" 2, ");
                                        mainTblQry.Append(" '' ");
                                        mainTblQry.Append(" ) ) as '" + attributerelationList[j].Field + "'");
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
                                        mainTblQry.Append("   AND mtn.Level = " + attributerelationList[j].Level + " ");
                                        mainTblQry.Append("  WHERE  mtv.EntityID = subtbl.Id ");
                                        mainTblQry.Append(" AND mtv.AttributeID = " + CurrentattrID + "   ");
                                        mainTblQry.Append(" ), ");
                                        mainTblQry.Append(" '-' ");
                                        mainTblQry.Append(" ) ) as '" + attributerelationList[j].Field + "'");
                                    }
                                    break;
                            }
                        }
                        else if ((AttributesList)attributerelationList[j].Type == AttributesList.Link)
                        {
                            mainTblQry.Append(",(isnull( (SELECT top 1 URL FROM CM_Links  WHERE ModuleId = 5 AND  entityid=subtbl.ID),'-') ) as '" + attributerelationList[j].Field + "'");
                        }
                        else if ((AttributesList)attributerelationList[j].Type == AttributesList.ListSingleSelection)
                        {
                            mainTblQry.Append(",(isnull( (SELECT top 1 caption FROM MM_Option  WHERE AttributeID=" + CurrentattrID + " AND id=subtbl.Attr_" + CurrentattrID + "),'-') ) as '" + attributerelationList[j].Field + "'");
                        }
                        else if ((AttributesList)attributerelationList[j].Type == AttributesList.CheckBoxSelection)
                        {
                            mainTblQry.Append(" ,isnull(cast(subtbl.attr_" + CurrentattrID + " as varchar(50)), '-') as '" + attributerelationList[j].Field + "'");
                        }
                        else if ((AttributesList)attributerelationList[j].Type == AttributesList.DateTime)
                        {
                            mainTblQry.Append(" ,REPLACE(CONVERT(varchar,isnull(subtbl.attr_" + CurrentattrID + " ,''),121),'1900-01-01 00:00:00.000', '-') as '" + attributerelationList[j].Field + "'");
                        }
                        else
                        {
                            mainTblQry.Append(" ,isnull(subtbl.attr_" + CurrentattrID + " , '-') as '" + attributerelationList[j].Field + "'");
                        }

                    }
                    mainTblQry.AppendLine(" from DAM_Asset pe inner join  ");
                    mainTblQry.AppendLine(damQuery.ToString());
                    mainTblQry.AppendLine("  on subtbl.id=pe.Id  where ");
                    mainTblQry.AppendLine(" pe.IsPublish = 1");

                    IList dynamicData = tx.PersistenceManager.DamRepository.ExecuteQuery(mainTblQry.ToString());
                    tx.Commit();

                    returnObj.Add(new
                    {
                        AssetFiles = assets,
                        AssetTypeAttrRel = attributerelationList,
                        AssetDynData = dynamicData,
                        AssetConditionData = filtereddata
                    });


                    return returnObj;
                }
            }
            catch (Exception ex)
            {
                return null;
            }

        }



        public IList Filtered_Published_Assets(DigitalAssetManagerProxy proxy, string criteriaSchema, IEnumerable<DamAttributeList> attr, int[] entitytypeIds, IList<XElement> criteriaLsts)
        {



            IList dynamicData = null;
            try
            {
                //Tuple<IList, string> cumstomlist_validate = null;
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {



                    int[] attrsidarr = attr.Distinct().Select(a => a.ID).ToArray();

                    IList<AttributeDao> attributes = new List<AttributeDao>();
                    attributes = (from attrbs in tx.PersistenceManager.MetadataRepository.Query<AttributeDao>() where attrsidarr.Contains(attrbs.Id) select attrbs).ToList<AttributeDao>();
                    var attributerelationList = (from AdminAttributes in attr
                                                 join ser in attributes on AdminAttributes.ID equals ser.Id
                                                 select new
                                                 {
                                                     ID = AdminAttributes.ID,
                                                     Type = ser.AttributeTypeID,
                                                     IsSpecial = ser.IsSpecial,
                                                     Field = ser.Id,
                                                     Level = AdminAttributes.Level,
                                                     Caption = AdminAttributes.Caption
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
                    if (entitytypeIds.Length == 0)
                    {
                        int version = MarcomManagerFactory.ActiveMetadataVersionNumber;

                        string xmlPath = tx.PersistenceManager.MetadataRepository.GetXmlPath(version);
                        XDocument xDoc = XDocument.Load(xmlPath);
                        var assetypes = (from DAMtypes in xDoc.Root.Elements("EntityType_Table").Elements("EntityType")
                                         where Convert.ToInt32(DAMtypes.Element("ModuleID").Value) == 5
                                         select new
                                         {
                                             damID = Convert.ToInt32(DAMtypes.Element("ID").Value),
                                             damCaption = Convert.ToString(DAMtypes.Element("Caption").Value),
                                             Id = Convert.ToInt32(DAMtypes.Element("ID").Value)
                                         }).ToList();
                        entitytypeIds = assetypes.Select(a => a.Id).ToArray();
                    }

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
                                        subdamQuery.AppendLine(",(SELECT pe.Name FROM DAM_Asset pe WHERE pe.ID = [MM_AttributeRecord_" + entitytypeIds[j] + "].ID)  as '" + attributerelationList[i].Caption + "'");
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
                                            subdamQuery.AppendLine("  AND id = [MM_AttributeRecord_" + entitytypeIds[j] + "].[Attr_" + attributerelationList[i].ID.ToString() + "]");
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

                                            innerjoindamQuery.AppendLine("  INNER JOIN MM_DAM_MultiSelectValue  mms" + entitytypeIds[j] + "" + i + " ");
                                            innerjoindamQuery.AppendLine("  ON   mms" + entitytypeIds[j] + "" + i + ".AssetID = [MM_AttributeRecord_" + entitytypeIds[j] + "].ID ");
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
                                if (val > 0)
                                    subdamQuery.Append(" ,isnull(cast([MM_AttributeRecord_" + entitytypeIds[j] + "].attr_" + attributerelationList[i].ID + " as varchar(50)), NULL) AS [" + attributerelationList[i].Caption + "]");
                                else
                                {
                                    subdamQuery.AppendLine(",  NULL ");
                                    subdamQuery.AppendLine("AS [" + attributerelationList[i].Caption + "] ");
                                }

                            }
                            else if ((AttributesList)attributerelationList[i].Type == AttributesList.DateTime)
                            {
                                if (val > 0)
                                    subdamQuery.Append(" ,REPLACE(CONVERT(varchar,isnull([MM_AttributeRecord_" + entitytypeIds[j] + "].attr_" + attributerelationList[i].ID + " ,''),121),'1900-01-01 00:00:00.000', NULL) AS [" + attributerelationList[i].Caption + "]");
                                else
                                {
                                    subdamQuery.AppendLine(",  NULL ");
                                    subdamQuery.AppendLine("AS [" + attributerelationList[i].Caption + "] ");
                                }
                            }
                            else
                            {
                                if (val > 0)
                                    subdamQuery.Append(" ,isnull([MM_AttributeRecord_" + entitytypeIds[j] + "].attr_" + attributerelationList[i].ID + " , NULL) AS [" + attributerelationList[i].Caption + "]");
                                else
                                {
                                    subdamQuery.AppendLine(",  NULL ");
                                    subdamQuery.AppendLine("AS [" + attributerelationList[i].Caption + "] ");
                                }
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
                    mainResultQuery.AppendLine("   INNER JOIN DAM_Asset pe");
                    mainResultQuery.AppendLine("  ON  pe.ID = tbl.ID AND pe.IsPublish=1");

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
                                mainResultQuery.AppendLine(criteriaLists[k].Operator + "(");
                                mainResultQuery.AppendLine(criteriaLists[k].Value);
                                mainResultQuery.AppendLine(") ");
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
                    tx.Commit();
                    return dynamicData;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<object> GetMediaBankFilterAttributes(DigitalAssetManagerProxy proxy)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    List<object> returnObj = new List<object>();
                    string Adminxmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
                    XDocument xDoc = XDocument.Load(Adminxmlpath);
                    IList<DamAttributeList> attributerelationList = new List<DamAttributeList>();

                    attributerelationList = (from AdminAttributes in xDoc.Descendants("publishedsearchcriteria").Descendants("Attributes").Descendants("Attribute")
                                             join ser in tx.PersistenceManager.MetadataRepository.Query<AttributeDao>() on Convert.ToInt16(AdminAttributes.Element("Id").Value) equals ser.Id
                                             select new DamAttributeList
                         {
                             ID = Convert.ToInt16(AdminAttributes.Element("Id").Value),
                             SortOrder = Convert.ToInt32(AdminAttributes.Element("SortOrder").Value),
                             Type = ser.AttributeTypeID,
                             IsSpecial = ser.IsSpecial,
                             Field = ser.Id,
                             Level = Convert.ToInt16(AdminAttributes.Element("Level").Value),
                             Caption = AdminAttributes.Element("DisplayName").Value,
                         }).Distinct().ToList();

                    if (attributerelationList.Count > 0)
                    {
                        attributerelationList = attributerelationList.OrderBy(a => a.SortOrder).ToList();
                    }

                    foreach (var attr in attributerelationList)
                    {
                        AttributesList attypeid = (AttributesList)attr.Type;

                        switch (attypeid)
                        {
                            case AttributesList.ListSingleSelection:
                            case AttributesList.ListMultiSelection:
                                returnObj.Add(new
                                    {
                                        AttributeCaption = attr.Caption,
                                        AttributeID = attr.ID,
                                        AttributeTypeID = attr.Type,
                                        Level = attr.Level,
                                        SortOrder = attr.SortOrder,
                                        Options = (from item in tx.PersistenceManager.DamRepository.Query<OptionDao>() where item.AttributeID == attr.ID select item).ToList()
                                    });
                                break;
                        }

                    }




                    return returnObj;
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public int MoveAssets(DigitalAssetManagerProxy proxy, int[] assetid, int folderId, int entityid, int actioncode)
        {
            int newasset = 0;
            try
            {

                //using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                //{

                if (assetid.Length > 0)
                {
                    for (int i = 0; i < assetid.Length; i++)
                    {

                        List<IAssets> assetdet = new List<IAssets>();
                        IAssets asset = new Assets();
                        asset = GetAssetAttributesDetails(proxy, assetid[i], false);


                        IList<IAttributeData> AttributeDatanew = new List<IAttributeData>();
                        AttributeDatanew = asset.AttributeData;
                        if (asset.Category == 0)
                        {
                            var Filesassest = asset.Files.Where(a => a.ID == asset.ActiveFileID).Select(a => a).ToList();
                            newasset = CreateAsset(proxy, folderId, asset.AssetTypeid, asset.Name, AttributeDatanew, Filesassest[0].Name, 1, Filesassest[0].MimeType, Filesassest[0].Extension, Convert.ToInt64(Filesassest[0].Size), entityid, Filesassest[0].Fileguid.ToString(), Filesassest[0].Description, true, Filesassest[0].Status, 0, Filesassest[0].Additionalinfo, asset.AssetAccess, asset.ID, false, actioncode);
                        }
                        else
                        {
                            newasset = CreateBlankAsset(proxy, folderId, asset.AssetTypeid, asset.Name, AttributeDatanew, entityid, asset.Category, asset.Url, true, 0, asset.AssetAccess, asset.ID, actioncode);
                        }
                    }
                    if (actioncode == (int)AssetOperation.Cut)
                    {
                        //delete logic goes here
                        bool isDone = DeleteAssets(proxy, assetid);
                    }

                    //tx.Commit();
                    return newasset;

                    //}

                }
            }
            catch (Exception ex)
            {
                return 0;
            }

            return newasset;
        }

        public bool DAMadminSettingsDeleteAttributeRelationAllViews(DigitalAssetManagerProxy proxy, int typeid, int ID)
        {


            string xmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
            XDocument adminXmlDoc = XDocument.Load(xmlpath);


            IList<XElement> ThumbnailViewresults = new List<XElement>();
            ThumbnailViewresults = adminXmlDoc.Descendants("DAMsettings").Descendants("ThumbnailView").Descendants("AssetType").Descendants("Attributes").Descendants("Attribute").Where(a => Convert.ToInt32(a.Element("Id").Value) == ID).Select(a => a).ToList();
            if (ThumbnailViewresults.Count > 0)
            {
                adminXmlDoc.Descendants("DAMsettings").Descendants("ThumbnailView").Descendants("AssetType").Descendants("Attributes").Descendants("Attribute").Where(a => Convert.ToInt32(a.Element("Id").Value) == ID).Remove();
                adminXmlDoc.Save(xmlpath);
            }
            IList<XElement> SummaryViewresults = new List<XElement>();
            SummaryViewresults = adminXmlDoc.Descendants("DAMsettings").Descendants("SummaryView").Descendants("AssetType").Descendants("Attributes").Descendants("Attribute").Where(a => Convert.ToInt32(a.Element("Id").Value) == ID).Select(a => a).ToList();
            if (SummaryViewresults.Count > 0)
            {
                adminXmlDoc.Descendants("DAMsettings").Descendants("SummaryView").Descendants("AssetType").Descendants("Attributes").Descendants("Attribute").Where(a => Convert.ToInt32(a.Element("Id").Value) == ID).Remove();
                adminXmlDoc.Save(xmlpath);
            }
            IList<XElement> ListViewresults = new List<XElement>();
            ListViewresults = adminXmlDoc.Descendants("DAMsettings").Descendants("ListView").Descendants("AssetType").Descendants("Attributes").Descendants("Attribute").Where(a => Convert.ToInt32(a.Element("Id").Value) == ID).Select(a => a).ToList();
            if (ListViewresults.Count > 0)
            {
                adminXmlDoc.Descendants("DAMsettings").Descendants("ListView").Descendants("AssetType").Descendants("Attributes").Descendants("Attribute").Where(a => Convert.ToInt32(a.Element("Id").Value) == ID).Remove();
                adminXmlDoc.Save(xmlpath);
            }

            return true;
        }

        public bool Assetimagecopy(Dictionary<string, string> filepairpaths, string filePath, string PreviewfilePath, string newFilePath)
        {
            foreach (KeyValuePair<string, string> kvp in filepairpaths)
            {
                //Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
                string orginalfilepair = kvp.Key;
                int i = orginalfilepair.IndexOf('.');
                string orginalfileguid = orginalfilepair.Substring(0, i);
                string orginalExtension = orginalfilepair.Substring(i);
                string smallfile = PreviewfilePath + "\\Small_" + orginalfileguid + orginalExtension;
                string Bigfile = PreviewfilePath + "\\Big_" + orginalfileguid + orginalExtension;
                string orex = orginalExtension;

                try
                {
                    if (System.IO.File.Exists(filePath + "\\" + orginalfileguid + orginalExtension))
                    {
                        System.IO.File.Copy(filePath + "\\" + orginalfileguid + orginalExtension, newFilePath + "\\" + kvp.Value + orginalExtension.ToLower());
                    }
                    if (System.IO.File.Exists(PreviewfilePath + "\\Small_" + orginalfileguid + ".jpg"))
                    {
                        System.IO.File.Copy(PreviewfilePath + "\\Small_" + orginalfileguid + ".jpg", PreviewfilePath + "\\Small_" + kvp.Value.ToString() + ".jpg");
                    }
                    if (System.IO.File.Exists(PreviewfilePath + "\\Big_" + orginalfileguid + ".jpg"))
                    {
                        System.IO.File.Copy(PreviewfilePath + "\\Big_" + orginalfileguid + ".jpg", PreviewfilePath + "\\Big_" + kvp.Value.ToString() + ".jpg");
                    }
                }
                catch (Exception ex)
                {
                }
            }

            return true;
        }

        public bool ReinitializeTask(DigitalAssetManagerProxy proxy, int taskID)
        {

            try
            {
                FeedNotificationServer fs = new FeedNotificationServer();
                WorkFlowNotifyHolder obj = new WorkFlowNotifyHolder();


                //Task Reinitialize concept for Approval task
                EntityTaskDao entityTask = new EntityTaskDao();
                IList<TaskMembersDao> itaskMemDao = new List<TaskMembersDao>();
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
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
                        }

                        fs.AsynchronousNotify(obj);
                    }
                    tx.Commit();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public int AttachAssets(DigitalAssetManagerProxy proxy, int[] assetid, int entityid, int folderId, bool blnAttach)
        {
            int newasset = 0;
            try
            {

                //using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                //{

                if (assetid.Length > 0)
                {
                    for (int i = 0; i < assetid.Length; i++)
                    {

                        List<IAssets> assetdet = new List<IAssets>();
                        IAssets asset = new Assets();
                        asset = GetAssetAttributesDetails(proxy, assetid[i], false);


                        IList<IAttributeData> AttributeDatanew = new List<IAttributeData>();
                        AttributeDatanew = asset.AttributeData;
                        if (asset.Category == 0)
                        {
                            var Filesassest = asset.Files.Where(a => a.ID == asset.ActiveFileID).Select(a => a).ToList();
                            newasset = CreateAsset(proxy, folderId, asset.AssetTypeid, asset.Name, AttributeDatanew, Filesassest[0].Name, 1, Filesassest[0].MimeType, Filesassest[0].Extension, Convert.ToInt64(Filesassest[0].Size), entityid, Filesassest[0].Fileguid.ToString(), Filesassest[0].Description, true, Filesassest[0].Status, 0, Filesassest[0].Additionalinfo, asset.AssetAccess, asset.ID, false, 0, blnAttach);
                        }
                        else
                        {
                            newasset = CreateBlankAsset(proxy, folderId, asset.AssetTypeid, asset.Name, AttributeDatanew, entityid, asset.Category, asset.Url, true, 0, asset.AssetAccess, asset.ID, 0, blnAttach);
                        }
                    }


                    //tx.Commit();
                    return newasset;

                    //}

                }
            }
            catch (Exception ex)
            {
                return 0;
            }

            return newasset;
        }

        public List<int> AttachAssetsforProofTask(DigitalAssetManagerProxy proxy, int[] assetid, int entityid, int folderId, bool blnAttach)
        {
            List<int> newasset = new List<int>();
            try
            {


                if (assetid.Length > 0)
                {
                    for (int i = 0; i < assetid.Length; i++)
                    {

                        List<IAssets> assetdet = new List<IAssets>();
                        IAssets asset = new Assets();
                        asset = GetAssetAttributesDetails(proxy, assetid[i], false);


                        IList<IAttributeData> AttributeDatanew = new List<IAttributeData>();
                        AttributeDatanew = asset.AttributeData;
                        if (asset.Category == 0)
                        {
                            var Filesassest = asset.Files.Where(a => a.ID == asset.ActiveFileID).Select(a => a).ToList();
                            newasset.Add(CreateAsset(proxy, folderId, asset.AssetTypeid, asset.Name, AttributeDatanew, Filesassest[0].Name, 1, Filesassest[0].MimeType, Filesassest[0].Extension, Convert.ToInt64(Filesassest[0].Size), entityid, Filesassest[0].Fileguid.ToString(), Filesassest[0].Description, true, Filesassest[0].Status, 0, Filesassest[0].Additionalinfo, asset.AssetAccess, asset.ID, false, 0, blnAttach));
                        }
                        else
                        {
                            newasset.Add(CreateBlankAsset(proxy, folderId, asset.AssetTypeid, asset.Name, AttributeDatanew, entityid, asset.Category, asset.Url, true, 0, asset.AssetAccess, asset.ID, 0, blnAttach));
                        }
                    }

                    return newasset;

                }
            }
            catch (Exception ex)
            {
                return null;
            }

            return newasset;
        }

        public List<object> GetSearchAssets(DigitalAssetManagerProxy proxy, string assetIDs)
        {

            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    List<object> returnObj = new List<object>();

                    string viewName = Enum.GetName(typeof(AssetView), 1);
                    StringBuilder assetQuery = new StringBuilder();
                    assetQuery.AppendLine(" DECLARE @OrderBy INT = " + 4 + "; ");
                    assetQuery.AppendLine(" SELECT a.ID AS 'FileUniqueID', ");
                    assetQuery.AppendLine(" a.Name as 'FileName', ");
                    assetQuery.AppendLine(" a.Extension, ");
                    assetQuery.AppendLine(" a.[Size],  a.[VersionNo], ");
                    assetQuery.AppendLine(" a.OwnerID, ");
                    assetQuery.AppendLine(" a.CreatedOn, ");
                    assetQuery.AppendLine(" a.FileGuid, ");
                    assetQuery.AppendLine(" a.[Description], ");
                    assetQuery.AppendLine(" a.AssetID, ");
                    assetQuery.AppendLine(" da.FolderID, ");
                    assetQuery.AppendLine(" da.EntityID, ");
                    assetQuery.AppendLine(" da.ID AS 'AssetUniqueID', ");
                    assetQuery.AppendLine(" da.Name as 'AssetName', da.Url as 'LinkURL', a.MimeType as 'MimeType', ");
                    assetQuery.AppendLine(" da.AssetTypeid, ");
                    assetQuery.AppendLine(" da.ActiveFileID, ");
                    assetQuery.AppendLine(" met.ColorCode, ");
                    assetQuery.AppendLine(" met.ShortDescription, ");
                    assetQuery.AppendLine(" a.[Status], ");
                    assetQuery.AppendLine(" da.Category, da.IsPublish, ISNULL(da.LinkedAssetID,0) as LinkedAssetID ");
                    assetQuery.AppendLine(" FROM   DAM_File a ");
                    assetQuery.AppendLine(" RIGHT OUTER JOIN DAM_Asset da ");
                    assetQuery.AppendLine(" ON  a.AssetID = da.ID AND a.ID=da.ActiveFileID");
                    assetQuery.AppendLine(" INNER JOIN MM_EntityType met ");
                    assetQuery.AppendLine(" ON  met.ID = da.AssetTypeid ");
                    assetQuery.AppendLine(" WHERE  da.id IN " + assetIDs + "  ");
                    assetQuery.AppendLine("  ORDER BY  ");
                    assetQuery.AppendLine(" CASE   ");
                    assetQuery.AppendLine(" WHEN @OrderBy = 1  ");
                    assetQuery.AppendLine(" THEN da.Name  ");
                    assetQuery.AppendLine(" END       asc,  ");
                    assetQuery.AppendLine(" CASE   ");
                    assetQuery.AppendLine(" WHEN @OrderBy = 2  ");
                    assetQuery.AppendLine(" THEN da.Name  ");
                    assetQuery.AppendLine(" END desc,  ");
                    assetQuery.AppendLine(" CASE   ");
                    assetQuery.AppendLine(" WHEN @OrderBy = 3  ");
                    assetQuery.AppendLine(" THEN da.Createdon   ");
                    assetQuery.AppendLine(" END       asc , ");
                    assetQuery.AppendLine(" CASE   ");
                    assetQuery.AppendLine(" WHEN @OrderBy = 4 ");
                    assetQuery.AppendLine(" THEN da.Createdon   ");
                    assetQuery.AppendLine(" END       desc  ");
                    IList assets = tx.PersistenceManager.DamRepository.ExecuteQuery(assetQuery.ToString());

                    int version = MarcomManagerFactory.ActiveMetadataVersionNumber;
                    string xmlpath = string.Empty;

                    xmlpath = tx.PersistenceManager.MetadataRepository.GetXmlPath(version);
                    var xmetadataDoc = XDocument.Load(xmlpath);

                    string Adminxmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
                    XDocument xDoc = XDocument.Load(Adminxmlpath);


                    IList<XElement> tempresults = new List<XElement>();
                    IList<XElement> result = new List<XElement>();
                    tempresults = xDoc.Descendants("DAMsettings").Descendants(viewName).Descendants("AssetType").Descendants("Attributes").Descendants("Attribute").Where(a => Convert.ToBoolean(a.Element("IsColumn").Value) == true).Select(a => a).ToList();
                    foreach (var rec in tempresults)
                    {
                        IList<XElement> duplist = new List<XElement>();
                        duplist = result.Where(a => Convert.ToInt32(a.Element("Id").Value) == Convert.ToInt32(rec.Element("Id").Value)).Select(a => a).ToList();
                        if (duplist.Count == 0)
                            result.Add(rec);
                    }

                    var thumpnail = (from AdminAttributes in xDoc.Descendants("DAMsettings").Descendants("ThumbnailView").Descendants("AssetType").Descendants("Attributes").Descendants("Attribute")
                                     join ser in tx.PersistenceManager.MetadataRepository.Query<AttributeDao>() on Convert.ToInt16(AdminAttributes.Element("Id").Value) equals ser.Id
                                     where Convert.ToBoolean(AdminAttributes.Element("IsColumn").Value) == true
                                     select new
                                     {
                                         ID = Convert.ToInt16(AdminAttributes.Element("Id").Value),
                                         Caption = ser.Caption,
                                         SotOrder = Convert.ToInt32(AdminAttributes.Element("SortOrder").Value),
                                         Type = ser.AttributeTypeID,
                                         assetType = Convert.ToInt16(AdminAttributes.Parent.Parent.Attributes("ID").ToList().FirstOrDefault().Value.ToString()),
                                         Field = ser.Id,
                                     }).Distinct().ToList();

                    int[] attrsidarr = result.Distinct().Select(a => Convert.ToInt32(a.Element("Id").Value)).Distinct().ToArray();

                    var typeidArr = assets.Cast<Hashtable>().Select(a => (int)a["AssetTypeid"]).Distinct().ToArray();

                    var idArr = assets.Cast<Hashtable>().Select(a => (int)a["AssetUniqueID"]).ToArray();

                    IList<AttributeDao> attributes = new List<AttributeDao>();
                    attributes = (from attrbs in tx.PersistenceManager.MetadataRepository.GetObject<AttributeDao>(xmlpath) where attrsidarr.Contains(attrbs.Id) select attrbs).ToList<AttributeDao>();

                    IList<EntityTypeAttributeRelationDao> entityAttributes = new List<EntityTypeAttributeRelationDao>();
                    entityAttributes = (from attrbs in tx.PersistenceManager.MetadataRepository.GetObject<EntityTypeAttributeRelationDao>(xmlpath) where typeidArr.Contains(attrbs.EntityTypeID) select attrbs).ToList<EntityTypeAttributeRelationDao>();




                    var attributerelationList = (from AdminAttributes in result
                                                 join ser in attributes on Convert.ToInt16(AdminAttributes.Element("Id").Value) equals ser.Id
                                                 select new
                                                 {
                                                     ID = Convert.ToInt16(AdminAttributes.Element("Id").Value),
                                                     SotOrder = Convert.ToInt32(AdminAttributes.Element("SortOrder").Value),
                                                     Type = ser.AttributeTypeID,
                                                     IsSpecial = ser.IsSpecial,
                                                     Field = ser.Id,
                                                     Level = Convert.ToInt16(AdminAttributes.Element("Level").Value),
                                                 }).Distinct().ToList();

                    int[] attrSelectType = { 1, 2, 3, 5 };

                    StringBuilder damQuery = new StringBuilder();
                    damQuery.AppendLine("(");
                    int EntitypeLenghth = typeidArr.Distinct().Count();
                    for (var i = 0; i < typeidArr.Distinct().Count(); i++)
                    {

                        damQuery.AppendLine("select ID");
                        foreach (var currentval in attributerelationList.Where(a => a.IsSpecial != true && attrSelectType.Contains(a.Type)).ToList())
                        {
                            int val = entityAttributes.ToList().Where(a => a.EntityTypeID == typeidArr[i] && a.AttributeID == currentval.ID).Count();
                            if (val != 0)
                                damQuery.AppendLine(" ,Attr_" + currentval.ID + " as 'Attr_" + currentval.ID + "'");
                            else
                                damQuery.AppendLine(" ,'-' as  'Attr_" + currentval.ID + "'");
                        }
                        damQuery.AppendLine(" from MM_AttributeRecord_" + typeidArr[i]);
                        if (i < EntitypeLenghth - 1)
                        {
                            damQuery.AppendLine(" union ");
                        }
                        //}
                    }
                    damQuery.AppendLine(") subtbl");

                    StringBuilder mainTblQry = new StringBuilder();
                    mainTblQry.AppendLine("select subtbl.ID  ");
                    int LastTreeLevel = attributerelationList.Where(a => (AttributesList)a.Type == AttributesList.TreeMultiSelection).OrderByDescending(a => a.Level).Select(a => a.Level).FirstOrDefault();
                    for (int j = 0; j < attributerelationList.Count(); j++)
                    {
                        string CurrentattrID = attributerelationList[j].ID.ToString();
                        if (attributerelationList[j].IsSpecial == true)
                        {
                            switch ((SystemDefinedAttributes)attributerelationList[j].ID)
                            {
                                case SystemDefinedAttributes.Name:
                                    mainTblQry.AppendLine(",pe.Name  as '" + attributerelationList[j].Field + "'");
                                    break;
                                case SystemDefinedAttributes.Owner:
                                    mainTblQry.Append(",ISNULL( (SELECT top 1  ISNULL(us.FirstName,'') + ' ' + ISNULL(us.LastName,'')  FROM UM_User us INNER JOIN AM_Entity_Role_User aeru ON us.ID=aeru.UserID AND aeru.EntityID=subtbl.Id  INNER JOIN AM_EntityTypeRoleAcl aetra ON  aeru.RoleID = aetra.ID AND  aetra.EntityTypeID=pe.TypeID AND aetra.EntityRoleID = 1),'-') as '" + attributerelationList[j].Field + "'");
                                    break;
                                case SystemDefinedAttributes.EntityStatus:
                                    mainTblQry.Append(", CASE WHEN pe.TypeID = " + (int)EntityTypeList.Objective + " THEN (SELECT case when ISNULL(po.ObjectiveStatus,0)=0 THEN 'Deactivated'  ELSE 'Active'  END from  PM_Objective po WHERE po.id=subtbl.Id) else isnull((SELECT  metso.StatusOptions FROM MM_EntityStatus mes INNER JOIN MM_EntityTypeStatus_Options metso ON mes.StatusID=metso.ID AND mes.EntityID=subtbl.id AND metso.IsRemoved=0),'-') end as '" + attributerelationList[j].Field + "'");
                                    break;
                                case SystemDefinedAttributes.EntityOnTimeStatus:
                                    mainTblQry.Append(", CASE WHEN pe.TypeID = " + (int)EntityTypeList.Objective + " THEN (SELECT case when ISNULL(po.ObjectiveStatus,0)=0 THEN '-'  ELSE '-'  END from  PM_Objective po WHERE po.id=subtbl.Id) else isnull((SELECT CASE WHEN (SELECT mes.IntimeStatus FROM MM_EntityStatus mes WHERE mes.EntityID=subtbl.id) = 0 THEN 'On time' WHEN (SELECT mes.IntimeStatus FROM MM_EntityStatus mes WHERE mes.EntityID=subtbl.id) = 1 THEN 'Delayed' WHEN (SELECT mes.IntimeStatus FROM MM_EntityStatus mes WHERE mes.EntityID=subtbl.id) = 2 THEN 'On hold' ELSE 'On time' END AS ontimestatus), '-') END AS '" + attributerelationList[j].Field + "'");
                                    break;
                                case SystemDefinedAttributes.MyRoleEntityAccess:

                                    mainTblQry.Append(", (select STUFF((SELECT',' +   ar.Caption ");
                                    mainTblQry.Append(" FROM AM_EntityTypeRoleAcl ar INNER JOIN AM_Entity_Role_User aeru ON ar.ID=aeru.RoleID  AND aeru.EntityID= pe.Id AND aeru.UserId= " + proxy.MarcomManager.User.Id + " ");
                                    mainTblQry.Append(" FOR XML PATH('')),1,1,'') AS x) AS '" + attributerelationList[j].Field + "'");
                                    break;
                                case SystemDefinedAttributes.MyRoleGlobalAccess:
                                    mainTblQry.Append(",(select STUFF((SELECT',' +   agr.Caption ");
                                    mainTblQry.Append(" FROM AM_GlobalRole agr  INNER JOIN AM_GlobalRole_User agru  ON agr.ID=agru.GlobalRoleId  AND agru.UserId= " + proxy.MarcomManager.User.Id + " ");
                                    mainTblQry.Append(" FOR XML PATH('')),1,1,'') AS x) AS '" + attributerelationList[j].Field + "'");
                                    break;
                            }
                        }
                        else if ((AttributesList)attributerelationList[j].Type == AttributesList.ListMultiSelection || (AttributesList)attributerelationList[j].Type == AttributesList.DropDownTree || (AttributesList)attributerelationList[j].Type == AttributesList.Tree || (AttributesList)attributerelationList[j].Type == AttributesList.Period || (AttributesList)attributerelationList[j].Type == AttributesList.TreeMultiSelection)
                        {
                            switch ((AttributesList)attributerelationList[j].Type)
                            {
                                case AttributesList.ListMultiSelection:

                                    if (attributerelationList[j].ID != (int)SystemDefinedAttributes.ObjectiveType)
                                    {

                                        mainTblQry.Append(" ,(SELECT  ");
                                        mainTblQry.Append(" STUFF( ");
                                        mainTblQry.Append(" ( ");
                                        mainTblQry.Append("  SELECT ', ' + mo.Caption ");
                                        mainTblQry.Append(" FROM MM_Option mo     ");
                                        mainTblQry.Append(" INNER JOIN MM_DAM_MultiSelectValue mms2  ");
                                        mainTblQry.Append("  ON  mo.ID = mms2.OptionID ");
                                        mainTblQry.Append(" AND mms2.AttributeID = " + attributerelationList[j].ID);
                                        mainTblQry.Append(" WHERE  mms2.AssetID = mms.AssetID FOR XML PATH('') ");
                                        mainTblQry.Append(" ), ");
                                        mainTblQry.Append("  1, ");
                                        mainTblQry.Append(" 2, ");
                                        mainTblQry.Append(" '' ");
                                        mainTblQry.Append("  )               AS VALUE ");
                                        mainTblQry.Append(" FROM   MM_DAM_MultiSelectValue     mms ");
                                        mainTblQry.Append(" WHERE  mms.AssetID=subtbl.Id and  mms.AttributeID = " + CurrentattrID + " ");
                                        mainTblQry.Append(" GROUP BY ");
                                        mainTblQry.Append("  mms.AssetID) as '" + attributerelationList[j].Field + "'");
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
                                    mainTblQry.Append("   AND mtn.Level = " + attributerelationList[j].Level + " ");
                                    mainTblQry.Append("  WHERE  mtv.EntityID = subtbl.Id ");
                                    mainTblQry.Append(" AND mtv.AttributeID = " + CurrentattrID + "   ");
                                    mainTblQry.Append(" ), ");
                                    mainTblQry.Append(" '' ");
                                    mainTblQry.Append(" ) ) as '" + attributerelationList[j].Field + "'");
                                    break;
                                case AttributesList.Tree:
                                    mainTblQry.Append(" ,'IsTree' as '" + attributerelationList[j].Field + "'");
                                    break;
                                case AttributesList.Period:
                                    mainTblQry.Append(" ,(SELECT (SELECT CONVERT(NVARCHAR(10), pep.StartDate, 120)  '@s', CONVERT(NVARCHAR(10), pep.EndDate, 120) '@e',");
                                    mainTblQry.Append(" pep.[Description] '@d', ROW_NUMBER() over(ORDER BY pep.Startdate) '@sid',");
                                    mainTblQry.Append(" pep.ID '@o'");
                                    mainTblQry.Append(" FROM   PM_EntityPeriod pep");
                                    mainTblQry.Append(" WHERE  pep.EntityID = subtbl.Id ORDER BY pep.Startdate FOR XML PATH('p'),");
                                    mainTblQry.Append(" TYPE");
                                    mainTblQry.Append(" ) FOR XML PATH('root')");
                                    mainTblQry.Append(" )  AS 'Period'");

                                    mainTblQry.Append(",(SELECT ISNULL(CAST(MIN(pep.Startdate) AS VARCHAR(10)) + '  ' + CAST(MAX(pep.EndDate)AS VARCHAR(10)),'-' )  ");
                                    mainTblQry.Append(" FROM PM_EntityPeriod pep WHERE pep.EntityID= subtbl.Id) AS TempPeriod ");

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
                                    if (LastTreeLevel == attributerelationList[j].Level)
                                    {
                                        mainTblQry.Append(" ,(SELECT  ");
                                        mainTblQry.Append(" STUFF( ");
                                        mainTblQry.Append(" ( ");
                                        mainTblQry.Append(" SELECT ', ' +  mtn.Caption ");
                                        mainTblQry.Append(" FROM   MM_TreeNode mtn ");
                                        mainTblQry.Append(" INNER JOIN MM_TreeValue mtv ");
                                        mainTblQry.Append(" ON  mtv.NodeID = mtn.ID and  mtv.AttributeID=" + attributerelationList[j].ID);
                                        mainTblQry.Append("  AND mtn.Level = " + attributerelationList[j].Level + " WHERE mtv.EntityID = subtbl.Id AND mtv.AttributeID = " + CurrentattrID + "  ");
                                        mainTblQry.Append(" FOR XML PATH('') ");
                                        mainTblQry.Append(" ), ");
                                        mainTblQry.Append("  1, ");
                                        mainTblQry.Append(" 2, ");
                                        mainTblQry.Append(" '' ");
                                        mainTblQry.Append(" ) ) as '" + attributerelationList[j].Field + "'");
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
                                        mainTblQry.Append("   AND mtn.Level = " + attributerelationList[j].Level + " ");
                                        mainTblQry.Append("  WHERE  mtv.EntityID = subtbl.Id ");
                                        mainTblQry.Append(" AND mtv.AttributeID = " + CurrentattrID + "   ");
                                        mainTblQry.Append(" ), ");
                                        mainTblQry.Append(" '' ");
                                        mainTblQry.Append(" ) ) as '" + attributerelationList[j].Field + "'");
                                    }
                                    break;
                            }
                        }
                        else if ((AttributesList)attributerelationList[j].Type == AttributesList.Link)
                        {
                            mainTblQry.Append(",(isnull( (SELECT top 1 URL FROM CM_Links  WHERE ModuleId = 5 AND  entityid=subtbl.ID),'') ) as '" + attributerelationList[j].Field + "'");
                        }
                        else if ((AttributesList)attributerelationList[j].Type == AttributesList.ListSingleSelection)
                        {
                            mainTblQry.Append(",(isnull( (SELECT top 1 caption FROM MM_Option  WHERE AttributeID=" + CurrentattrID + " AND id=subtbl.Attr_" + CurrentattrID + "),'') ) as '" + attributerelationList[j].Field + "'");
                        }
                        else if ((AttributesList)attributerelationList[j].Type == AttributesList.CheckBoxSelection)
                        {
                            mainTblQry.Append(" ,isnull(cast(subtbl.attr_" + CurrentattrID + " as varchar(50)), '') as '" + attributerelationList[j].Field + "'");
                        }
                        else if ((AttributesList)attributerelationList[j].Type == AttributesList.DateTime)
                        {
                            mainTblQry.Append(" ,REPLACE(CONVERT(varchar,isnull(subtbl.attr_" + CurrentattrID + " ,''),121),'1900-01-01 00:00:00.000', '') as '" + attributerelationList[j].Field + "'");
                        }
                        else if ((AttributesList)attributerelationList[j].Type == AttributesList.ParentEntityName)
                        {
                            mainTblQry.Append(" ,isnull((SELECT top 1 pe2.name  + '!@#' + met.ShortDescription + '!@#' + met.ColorCode FROM PM_Entity pe2 INNER JOIN MM_EntityType met ON pe2.TypeID=met.ID  WHERE  pe2.id=pe.parentid), '') as '" + attributerelationList[j].Field + "'");
                        }
                        else
                        {
                            mainTblQry.Append(" ,isnull(subtbl.attr_" + CurrentattrID + " , '') as '" + attributerelationList[j].Field + "'");
                        }

                    }
                    mainTblQry.AppendLine(" from DAM_Asset pe inner join  ");
                    mainTblQry.AppendLine(damQuery.ToString());
                    mainTblQry.AppendLine("  on subtbl.id=pe.Id  where ");

                    mainTblQry.AppendLine(" pe.id in " + assetIDs + "");

                    IList dynamicData = tx.PersistenceManager.DamRepository.ExecuteQuery(mainTblQry.ToString());

                    returnObj.Add(new
                    {
                        AssetFiles = assets,
                        AssetTypeAttrRel = attributerelationList,
                        AssetDynData = dynamicData,
                        thumbnailxmlsettings = thumpnail
                    });

                    tx.Commit();
                    return returnObj;
                }
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public List<object> GetCustomFilterAttributes(DigitalAssetManagerProxy proxy, int typeID)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    List<object> returnObj = new List<object>();
                    string Adminxmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
                    XDocument xDoc = XDocument.Load(Adminxmlpath);
                    IList<DamAttributeList> attributerelationList = new List<DamAttributeList>();

                    attributerelationList = (from AdminAttributes in xDoc.Descendants("Productionsettings").Descendants("ProductionCreation").Descendants("ProductionType").Where(a => Convert.ToInt16(a.Attribute("ID").Value) == typeID).Descendants("Attributes").Descendants("Attribute")
                                             join ser in tx.PersistenceManager.MetadataRepository.Query<AttributeDao>() on Convert.ToInt16(AdminAttributes.Element("Id").Value) equals ser.Id
                                             select new DamAttributeList
                                             {
                                                 ID = Convert.ToInt16(AdminAttributes.Element("Id").Value),
                                                 SortOrder = Convert.ToInt32(AdminAttributes.Element("SortOrder").Value),
                                                 Type = ser.AttributeTypeID,
                                                 IsSpecial = ser.IsSpecial,
                                                 Field = ser.Id,
                                                 Caption = AdminAttributes.Element("DisplayName").Value,
                                             }).Distinct().ToList();
                    if (attributerelationList.Count > 0)
                    {
                        attributerelationList = attributerelationList.OrderBy(a => a.SortOrder).ToList();
                    }

                    foreach (var attr in attributerelationList)
                    {
                        AttributesList attypeid = (AttributesList)attr.Type;

                        switch (attypeid)
                        {
                            case AttributesList.ListSingleSelection:
                            case AttributesList.ListMultiSelection:
                                returnObj.Add(new
                                {
                                    AttributeCaption = attr.Caption,
                                    AttributeID = attr.ID,
                                    AttributeTypeID = attr.Type,
                                    Level = attr.Level,
                                    SortOrder = attr.SortOrder,
                                    Options = (from item in tx.PersistenceManager.DamRepository.Query<OptionDao>() where item.AttributeID == attr.ID select item).ToList()
                                });
                                break;
                        }

                    }

                    return returnObj;
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public string GetSearchCriteriaAdminSettings(DigitalAssetManagerProxy proxy, string LogoSettings, string key, int typeid)
        {
            if (typeid != 0)
            {
                string jsonText = "";
                string xmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
                XDocument adminXdoc = XDocument.Load(xmlpath);
                var result = adminXdoc.Descendants("Productionsettings").Descendants("ProductionCreation").Descendants("ProductionType").Select(a => a).ToList();
                if (result != null)
                {
                    var result1 = adminXdoc.Descendants("Productionsettings").Descendants("ProductionCreation").Descendants("ProductionType").Select(a => a).ToList().Where(a => Convert.ToInt32(a.Attribute("ID").Value) == typeid).Select(a => a);
                    if (result1 != null)
                    {
                        var abc = result1.ToList();
                        if (abc.Count > 0)
                        {
                            jsonText = JsonConvert.SerializeObject(abc[0]);
                        }
                    }
                }
                return jsonText;
            }
            else
            {
                string xmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
                XDocument adminXdoc = XDocument.Load(xmlpath);
                var result = adminXdoc.Descendants("Productionsettings").Descendants("ProductionCreation").Descendants("ProductionType").Select(a => a).ToList().Where(a => Convert.ToInt32(a.Attribute("ID").Value) == 1).Select(a => a).ToList();
                var xElementResult = result[0];
                string jsonText = JsonConvert.SerializeObject(result[0]);
                return jsonText;
            }
        }


        #region ProofHqProofCreation

        /// <summary>
        /// Creates the proof by ProofHQ.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="taskid">The taskid.</param>
        /// <param name="assetids">int[] assetids.</param>
        /// <returns>bool</returns>
        public bool CreateProofTaskWithAttachment(DigitalAssetManagerProxy proxy, int taskID, int[] assetIds)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction()) // initialize the transaction
                {
                    string xmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
                    XDocument adminXmlDoc = XDocument.Load(xmlpath);
                    string xelementName = "ProofHQSettings_Table";
                    var xelementFilepath = XElement.Load(xmlpath);
                    var pfEmail = xelementFilepath.Element(xelementName).Element("ProofHQSettings").Element("Email");
                    var pfPwd = xelementFilepath.Element(xelementName).Element("ProofHQSettings").Element("Password");

                    var client = new soapService();
                    SOAPLoginObject response = client.doLogin(pfEmail.Value, pfPwd.Value);
                    //var taskmembers = proxy.MarcomManager.TaskManager.GetTaskMember(taskID); // get all the task memebrs along with owner
                    //var taskAssigneesList = taskmembers.Where(a => a.RoleID != 1).Select(a => a).ToList(); //only the assignees




                    string AIDinClause = "( " + String.Join(",", assetIds.Select(x => x.ToString()).Distinct().ToArray()) + " )";
                    var sb = new StringBuilder();
                    sb.AppendLine(" SELECT *");
                    sb.AppendLine("FROM   DAM_File df");
                    sb.AppendLine("WHERE  df.AssetID IN " + AIDinClause + "");
                    sb.AppendLine("AND df.ID IN (SELECT da.ActiveFileID");
                    sb.AppendLine(" FROM   DAM_Asset da");
                    sb.AppendLine("WHERE  id IN " + AIDinClause + " )");

                    string uploadfilepath = "";
                    string uploadfilename = "";

                    IList files = tx.PersistenceManager.DamRepository.ExecuteQuery(sb.ToString()); // assets attached for this task



                    if (files != null && files.Count > 0)
                    {
                        foreach (var file in files)
                        {
                            var Extension = Convert.ToString((string)((System.Collections.Hashtable)(file))["Extension"]);
                            var FileGuid = Convert.ToString((Guid)((System.Collections.Hashtable)(file))["FileGuid"]);
                            uploadfilename = Convert.ToString((string)((System.Collections.Hashtable)(file))["Name"]);
                            uploadfilepath = System.Web.HttpContext.Current.Server.MapPath("").Replace("dam\\CreateProofTaskWithAttachment", "DAMFiles\\Original\\") + FileGuid + Extension;
                        }
                    }


                    IList taskDetails = tx.PersistenceManager.DamRepository.ExecuteQuery("SELECT ID, Name, DueDate FROM [dbo].[TM_EntityTask] WHERE ID = " + taskID); // assets attached for this task

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

                    if (proofName == null)
                    {
                        proofName = uploadfilename;
                    }

                    IList taskAssigneesList = tx.PersistenceManager.DamRepository.ExecuteQuery("SELECT uu.ID, uu.FirstName, uu.LastName, uu.Email FROM   UM_User uu WHERE  uu.ID IN (SELECT ttm.UserID FROM TM_Task_Members ttm WHERE  ttm.TaskID = " + taskID + " AND ttm.RoleID = 4)"); // assets attached for this task

                    string assigneermail = "";


                    var Recipients = new List<SOAPFileRecipientObject>();

                    foreach (var item in taskAssigneesList)
                    {
                        // assigneermail = item.
                        if (assigneermail != "")
                        {
                            assigneermail = assigneermail + ";";
                        }


                        var FirstName = Convert.ToString((string)((System.Collections.Hashtable)(item))["FirstName"]);
                        var LastName = Convert.ToString((string)((System.Collections.Hashtable)(item))["LastName"]);
                        var Email = Convert.ToString((string)((System.Collections.Hashtable)(item))["Email"]);

                        assigneermail = assigneermail + Email;

                        Recipients.Add(new SOAPFileRecipientObject() { deadline = deadline, email = Email, name = FirstName + " " + LastName, notifications = "User value from user's Personal settings", position = "", primary_decision_maker = "false", role = "reviewer & approver" });

                        var users = client.findUsersByEmail(response.session, Email, 0);

                        //if (users.Length == 0)
                        //{
                        //    try
                        //    {
                        //         var user = client.addUser(response.session, 0, Email, FirstName, LastName, "", 6, "marcom123", "marcom123", false, "Deprecated", "Australia/Sydney", false, true);
                        //    }
                        //    catch (Exception)
                        //    {

                        //    }
                        //}

                    }



                    //Prabhu will put proofHq creation logic here

                    //Check if user alredy present in proofHQ or not

                    //everything except upload file and url can be left blank if needed
                    string outdata = UploadFileEx(uploadfilepath, uploadfilename, "http://www.proofhq.com/soap/upload", "uploadfile", "multipart/form-data",
                         null, null);

                    XDocument doc = XDocument.Parse(outdata);
                    var filehash = doc.Descendants("filehash").ToList<XElement>()[0].Value;


                    var proof = client.createProof(response.session, 0, filehash, proofName, uploadfilename, null, null, deadline,
                    assigneermail, 0, 0, false, false, "3", "5", false, true, false, true, true, false, false,
                false, false, true, true, true, true, false, false, false, null, null, true, null, false, true, false,
                true, true, false, true, true, false, true, true);

                    if (proof != null)
                    {

                        client.addProofReviewers(response.session, proof.file_id, Recipients.ToArray(), true);


                        //The Key is root node current Settings


                        var xmlApplicationUrl = xelementFilepath.Element("ApplicationURL");
                        if (xmlApplicationUrl != null)
                        {
                            string appurl = Convert.ToString(xmlApplicationUrl.Value);

                            var CallbackObject = client.setProofCallback(response.session, proof.file_id, "3", appurl + "proofstatusupdate.aspx?tid=" + taskID, 1);
                        }


                        //insert query for insert into task proof table
                        var sbInsert = new StringBuilder();
                        // sbInsert.AppendLine(" if NOT exists (select * from SampleTable where proofid = proofid) ");
                        sbInsert.AppendLine("INSERT INTO TM_Task_Proof_Relation ( TaskID, ProofID ) VALUES(" + taskID + "," + proof.file_id + ")");
                        tx.PersistenceManager.DamRepository.ExecuteQuery(sbInsert.ToString());  //insert query


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


        /// <summary>
        /// Creates the proof by ProofHQ.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="taskid">The taskid.</param>
        /// <param name="assetids">int[] assetids.</param>
        /// <returns>bool</returns>
        public int GetProofIDByTaskID(DigitalAssetManagerProxy proxy, int taskID)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction()) // initialize the transaction
                {

                    IList proofs = tx.PersistenceManager.DamRepository.ExecuteQuery("SELECT ttpr.ProofID FROM TM_Task_Proof_Relation ttpr WHERE ttpr.TaskID = " + taskID); // assets attached for this task

                    int proofID = 0;

                    if (proofs != null && proofs.Count > 0)
                    {
                        foreach (var proof in proofs)
                        {
                            proofID = Convert.ToInt32((int)((System.Collections.Hashtable)(proof))["ProofID"]);

                        }
                    }

                    tx.Commit();

                    return proofID;
                }
            }
            catch (Exception ex)
            {
                return -1;
            }

        }

        /// <summary>
        /// Creates the proof by ProofHQ.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="taskid">The taskid.</param>
        /// <param name="assetids">int[] assetids.</param>
        /// <returns>bool</returns>
        public int GetUserIDByEmail(DigitalAssetManagerProxy proxy, string email)
        {
            try
            {
                using (ITransaction tx = proxy.MarcomManager.GetTransaction()) // initialize the transaction
                {

                    IList userList = tx.PersistenceManager.DamRepository.ExecuteQuery("SELECT uu.ID FROM UM_User uu WHERE uu.Email LIKE '" + email + "'"); // assets attached for this task

                    int UserID = 0;

                    if (userList != null && userList.Count > 0)
                    {
                        foreach (var user in userList)
                        {
                            UserID = Convert.ToInt32((int)((System.Collections.Hashtable)(user))["ID"]);

                        }
                    }

                    tx.Commit();

                    return UserID;
                }
            }
            catch (Exception ex)
            {
                return -1;
            }

        }

        public static string UploadFileEx(string uploadfile, string filename, string url,
            string fileFormName, string contenttype, NameValueCollection querystring,
            CookieContainer cookies)
        {
            if ((fileFormName == null) ||
                (fileFormName.Length == 0))
            {
                fileFormName = "file";
            }

            if ((contenttype == null) ||
                (contenttype.Length == 0))
            {
                contenttype = "application/octet-stream";
            }


            string postdata;
            postdata = "?";
            if (querystring != null)
            {
                foreach (string key in querystring.Keys)
                {
                    postdata += key + "=" + querystring.Get(key) + "&";
                }
            }
            var uri = new Uri(url + postdata);


            string boundary = "----------" + DateTime.Now.Ticks.ToString("x");
            var webrequest = (HttpWebRequest)WebRequest.Create(uri);
            webrequest.CookieContainer = cookies;
            webrequest.ContentType = "multipart/form-data; boundary=" + boundary;
            webrequest.Method = "POST";
            webrequest.Timeout = 2139999999;

            // Build up the post message header
            var sb = new StringBuilder();
            sb.Append("--");
            sb.Append(boundary);
            sb.Append("\r\n");
            sb.Append("Content-Disposition: form-data; name=\"");
            sb.Append(fileFormName);
            sb.Append("\"; filename=\"");
            sb.Append(filename);
            sb.Append("\"");
            sb.Append("\r\n");
            sb.Append("Content-Type: ");
            sb.Append(contenttype);
            sb.Append("\r\n");
            sb.Append("\r\n");

            string postHeader = sb.ToString();
            byte[] postHeaderBytes = Encoding.UTF8.GetBytes(postHeader);

            // Build the trailing boundary string as a byte array
            // ensuring the boundary appears on a line by itself
            byte[] boundaryBytes =
                Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            var fileStream = new FileStream(uploadfile,
                FileMode.Open, FileAccess.Read);
            long length = postHeaderBytes.Length + fileStream.Length +
                          boundaryBytes.Length;
            webrequest.ContentLength = length;

            Stream requestStream = webrequest.GetRequestStream();

            // Write out our post header
            requestStream.WriteTimeout = 2139999999;
            requestStream.ReadTimeout = 2139999999;
            requestStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);

            //// Write out the file contents
            //var buffer = new Byte[checked((uint)Math.Min(4096,(int)fileStream.Length))];
            //int bytesRead = 0;
            //int bytesStart = 0;
            //int noOfBytesToRead = buffer.Length;
            //try {

            //    while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
            //    {

            //        requestStream.Write(buffer, 0, bytesRead);
            //        bytesStart = bytesStart + bytesRead;
            //        noOfBytesToRead = noOfBytesToRead + bytesRead;
            //        if (noOfBytesToRead > fileStream.Length)
            //        {
            //            noOfBytesToRead = ((int)fileStream.Length) - bytesStart;
            //        }
            //    }
            //}
            // catch { }
            //// Write out the trailing boundary

            // Write out the file contents
            var buffer = new Byte[((int)fileStream.Length)];
            int bytesRead = 0;

            try
            {

                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    requestStream.Write(buffer, 0, bytesRead);
                }
            }
            catch { }
            //var buffer = new Byte[checked((uint)Math.Min(4096, (int)fileStream.Length))];
            //byte[] bytes = new byte[fileStream.Length];
            //int numBytesToRead = (int)fileStream.Length;
            //int numBytesRead = 0;
            //  while (numBytesToRead > 0)
            //{
            //        int n = fileStream.Read(bytes, numBytesRead, numBytesToRead);
            //        if (n == 0)
            //            break;

            //        numBytesRead += n;
            //        numBytesToRead -= n;
            //}
            // numBytesToRead = bytes.Length;
            requestStream.Write(boundaryBytes, 0, boundaryBytes.Length);
            WebResponse responce = webrequest.GetResponse();
            Stream s = responce.GetResponseStream();
            var sr = new StreamReader(s);

            return sr.ReadToEnd();
        }

        #endregion

        //public IAssets ChangeAssettype(DigitalAssetManagerProxy proxy, int assetTypeID, int assetId, bool IsforAdmin = false)
        //{
        //    try
        //    {
        //        int version = MarcomManagerFactory.ActiveMetadataVersionNumber;
        //        IAssets asset = new Assets();
        //        AssetsDao assetObj = new AssetsDao();
        //        MediaGeneratorAssetDao mediageneratorObj = new MediaGeneratorAssetDao();
        //        List<IAssets> assetdet = new List<IAssets>();
        //        if (IsforAdmin)
        //            version = MarcomManagerFactory.AdminMetadataVersionNumber;
        //        IList<IAttributeData> attributesWithValues = new List<IAttributeData>();
        //        IList<BrandSystems.Marcom.Core.Planning.Interface.ITreeDropDownLabel> droplabel;
        //        IList<BrandSystems.Marcom.Core.Planning.Interface.ITreeDropDownCaption> itreeCaption = new List<BrandSystems.Marcom.Core.Planning.Interface.ITreeDropDownCaption>();
        //        AttributeData attributedate;
        //        IList<IDAMFile> iifilelist = new List<IDAMFile>();
        //        using (ITransaction tx = proxy.MarcomManager.GetTransaction())
        //        {
        //            assetObj = (from item in tx.PersistenceManager.PlanningRepository.Query<AssetsDao>()
        //                        where item.ID == assetId
        //                        select item).FirstOrDefault();

        //            var AssetSelectQuery = new StringBuilder();
        //            if (assetId != 0)
        //            {
        //                AssetSelectQuery.Append("select [ID],[Name],[VersionNo],[MimeType],[Extension],[Size],[OwnerID],[CreatedOn],[Checksum],[FileGuid],[Description],[AssetID],[Status],isnull(Additionalinfo,'') AS Additionalinfo FROM DAM_File where AssetID = ? ORDER BY [VersionNo] ASC");
        //            }
        //            var Result = ((tx.PersistenceManager.CommonRepository.ExecuteQuerywithMinParam(AssetSelectQuery.ToString(), assetId)).Cast<Hashtable>().ToList());
        //            foreach (var obj in Result)
        //            {
        //                DAMFile damFile = new DAMFile();
        //                damFile.ID = Convert.ToInt32(obj["ID"]);
        //                damFile.AssetID = Convert.ToInt32(obj["AssetID"]);
        //                damFile.Name = Convert.ToString(obj["Name"]);
        //                damFile.VersionNo = Convert.ToInt32(obj["VersionNo"]);
        //                damFile.MimeType = Convert.ToString(obj["MimeType"]);
        //                damFile.Extension = Convert.ToString(obj["Extension"]);
        //                damFile.Size = Convert.ToInt64(obj["Size"]);
        //                damFile.Ownerid = Convert.ToInt32(obj["OwnerID"]);
        //                damFile.CreatedOn = DateTimeOffset.Parse(obj["CreatedOn"].ToString());
        //                damFile.StrCreatedDate = DateTimeOffset.Parse(obj["CreatedOn"].ToString()) != null ? DateTimeOffset.Parse(obj["CreatedOn"].ToString()).ToString("yyyy-MM-dd") : "";
        //                damFile.Checksum = Convert.ToString(obj["Checksum"]);
        //                damFile.Fileguid = (Guid)(obj["FileGuid"]);
        //                damFile.Description = Convert.ToString(obj["Description"]);
        //                var userDao = tx.PersistenceManager.DamRepository.Get<UserDao>(UserDao.MappingNames.Id, damFile.Ownerid);
        //                damFile.OwnerName = null;
        //                if (userDao != null)
        //                    damFile.OwnerName = userDao.FirstName + " " + userDao.LastName;
        //                damFile.Status = Convert.ToInt32(obj["Status"]);
        //                damFile.Additionalinfo = Convert.ToString(obj["Additionalinfo"]);
        //                damFile.StrCreatedDateTime = DateTimeOffset.Parse(obj["CreatedOn"].ToString()) != null ? DateTimeOffset.Parse(obj["CreatedOn"].ToString()).ToString("yyyy-MM-dd HH:mm") : "";
        //                damFile.Activestatus = Convert.ToInt32(obj["ID"]) == assetObj.ActiveFileID ? true : false;

        //                iifilelist.Add(damFile);
        //            }
        //            asset.Files = iifilelist;
        //            //var allattributes = tx.PersistenceManager.PlanningRepository.GetAll<BrandSystems.Marcom.Dal.Metadata.Model.AttributeDao>();

        //            try
        //            {
        //                if (assetObj.AssetTypeid == 33)
        //                {
        //                    mediageneratorObj = (from item in tx.PersistenceManager.PlanningRepository.Query<MediaGeneratorAssetDao>()
        //                                         where item.AssetID == assetId
        //                                         select item).OrderByDescending(a => a.ID).FirstOrDefault();

        //                    asset.MediaGeneratorData = mediageneratorObj;
        //                }
        //            }
        //            catch
        //            {

        //            }

        //            asset.ID = assetObj.ID;
        //            asset.Name = assetObj.Name;
        //            asset.FolderID = assetObj.FolderID;
        //            asset.EntityID = assetObj.EntityID;
        //            asset.AssetTypeid = assetObj.AssetTypeid;
        //            asset.CreatedBy = assetObj.CreatedBy;
        //            asset.Createdon = assetObj.Createdon;
        //            asset.StrCreatedDate = assetObj.Createdon != null ? assetObj.Createdon.ToString("yyyy-MM-dd") : "";
        //            asset.ActiveFileID = assetObj.ActiveFileID;
        //            asset.Status = assetObj.Status;
        //            asset.Active = assetObj.Active;
        //            asset.AssetAccess = assetObj.AssetAccess == null ? "" : assetObj.AssetAccess;
        //            asset.Category = assetObj.Category;
        //            asset.Url = assetObj.Url;
        //            asset.IsPublish = assetObj.IsPublish;
        //            asset.LinkedAssetID = assetObj.LinkedAssetID;
        //            asset.StrUpdatedOn = assetObj.UpdatedOn != null ? assetObj.UpdatedOn.ToString("yyyy-MM-dd") : asset.StrCreatedDate;

        //            //if (asset.AssetTypeid != 34 && asset.AssetTypeid != 33)
        //            //{
        //            string xmlpath = tx.PersistenceManager.MetadataRepository.GetXmlPath(version);
        //            XDocument docx = XDocument.Load(xmlpath);
        //            var rddd = (from EntityAttrRel in docx.Root.Elements("EntityTypeAttributeRelation_Table").Elements("EntityTypeAttributeRelation")
        //                        join Attr in docx.Root.Elements("Attribute_Table").Elements("Attribute") on Convert.ToInt32(EntityAttrRel.Element("AttributeID").Value) equals Convert.ToInt32(Attr.Element("ID").Value)
        //                        where Convert.ToInt32(EntityAttrRel.Element("EntityTypeID").Value) == assetObj.AssetTypeid
        //                        orderby Convert.ToInt32(EntityAttrRel.Element("SortOrder").Value)
        //                        select new
        //                        {
        //                            ID = Convert.ToInt16(Attr.Element("ID").Value),
        //                            Caption = EntityAttrRel.Element("Caption").Value,
        //                            AttributeTypeID = Convert.ToInt16(Attr.Element("AttributeTypeID").Value),
        //                            Description = Attr.Element("Description").Value,
        //                            IsSystemDefined = Convert.ToBoolean(Convert.ToInt32(Attr.Element("IsSystemDefined").Value)),
        //                            IsSpecial = Convert.ToBoolean(Convert.ToInt32(Attr.Element("IsSpecial").Value)),
        //                            InheritFromParent = Convert.ToBoolean(Convert.ToInt32(EntityAttrRel.Element("InheritFromParent").Value)),
        //                            ChooseFromParent = Convert.ToBoolean(Convert.ToInt32(EntityAttrRel.Element("ChooseFromParentOnly").Value)),
        //                            IsReadOnly = Convert.ToBoolean(Convert.ToInt32(EntityAttrRel.Element("IsReadOnly").Value))
        //                        }).ToList();

        //            var attributesdetails = rddd;
        //            //var multiSelectValuedao = (from item in tx.PersistenceManager.PlanningRepository.Query<MultiSelectDao>()
        //            //                           where item.Entityid == entityId
        //            //                           select item).ToList();

        //            List<TreeValueDao> treevaluedao = new List<TreeValueDao>();
        //            List<int> treevalues = new List<int>();

        //            List<TreeValueDao> multiselecttreevalues = new List<TreeValueDao>();
        //            List<int> temptreevalues = new List<int>();

        //            //IList<IAttributeData> entityUserAttrVal = new List<IAttributeData>();
        //            //entityUserAttrVal = proxy.MarcomManager.PlanningManager.GetEntityAttributesDetailsUserDetails(proxy.MarcomManager.User.Id);

        //            var assetName = GetAssetName(tx, assetId, 1);
        //            var dynamicvalues = tx.PersistenceManager.DamRepository.GetAll<DynamicAttributesDao>(assetName).Where(a => a.Id == assetId).Select(a => a.Attributes).SingleOrDefault();
        //            foreach (var val in attributesdetails)
        //            {
        //                AttributesList attypeid = (AttributesList)val.AttributeTypeID;
        //                if (Convert.ToInt32(AttributesList.DropDownTree) == val.AttributeTypeID || Convert.ToInt32(AttributesList.DropDownTree) == val.AttributeTypeID)
        //                {
        //                    treevaluedao = new List<TreeValueDao>();
        //                    treevaluedao = tx.PersistenceManager.PlanningRepository.Query<TreeValueDao>().Where(a => a.Entityid == assetId && a.Attributeid == val.ID).OrderBy(a => a.Level).ToList();
        //                    treevalues = new List<int>();
        //                    treevalues = (from treevalue in treevaluedao where treevalue.Attributeid == val.ID select treevalue.Nodeid).ToList();
        //                }
        //                if (Convert.ToInt32(AttributesList.TreeMultiSelection) == val.AttributeTypeID || Convert.ToInt32(AttributesList.TreeMultiSelection) == val.AttributeTypeID)
        //                {
        //                    multiselecttreevalues = new List<TreeValueDao>();
        //                    multiselecttreevalues = tx.PersistenceManager.PlanningRepository.Query<TreeValueDao>().Where(a => a.Entityid == assetId && a.Attributeid == val.ID).OrderBy(a => a.Level).ToList();
        //                    temptreevalues = new List<int>();
        //                    temptreevalues = (from treevalue in multiselecttreevalues where treevalue.Attributeid == val.ID select treevalue.Nodeid).ToList();
        //                }
        //                switch (attypeid)
        //                {
        //                    case AttributesList.TextSingleLine:
        //                        attributedate = new AttributeData();
        //                        attributedate.ID = val.ID;
        //                        attributedate.TypeID = val.AttributeTypeID;
        //                        attributedate.Lable = val.Caption.Trim();
        //                        if (val.IsSpecial == true && val.ID == Convert.ToInt32(SystemDefinedAttributes.Name))
        //                        {
        //                            attributedate.Caption = Enum.GetName(typeof(SystemDefinedAttributes), Convert.ToInt32(SystemDefinedAttributes.Name)) == "" ? "-" : Enum.GetName(typeof(SystemDefinedAttributes), Convert.ToInt32(SystemDefinedAttributes.Name));
        //                            attributedate.Value = (string)assetObj.Name;
        //                        }
        //                        else
        //                        {
        //                            if (dynamicvalues != null)
        //                            {
        //                                attributedate.Caption = dynamicvalues[val.ID.ToString()] == "" ? "-" : (dynamic)dynamicvalues[val.ID.ToString()];
        //                                attributedate.Value = (dynamic)dynamicvalues[val.ID.ToString()];
        //                            }
        //                            else
        //                            {
        //                                attributedate.Caption = "-";
        //                                attributedate.Value = "-";
        //                            }
        //                        }
        //                        attributedate.IsSpecial = val.IsSpecial;



        //                        attributesWithValues.Add(attributedate);
        //                        break;

        //                    case AttributesList.TextMultiLine:
        //                        attributedate = new AttributeData();
        //                        if (dynamicvalues != null)
        //                            attributedate.Caption = dynamicvalues[val.ID.ToString()] == "" ? "-" : (dynamic)dynamicvalues[val.ID.ToString()];
        //                        else
        //                            attributedate.Caption = "-";
        //                        attributedate.ID = val.ID;
        //                        attributedate.TypeID = val.AttributeTypeID;
        //                        attributedate.Lable = val.Caption.Trim();
        //                        if (dynamicvalues != null)
        //                            attributedate.Value = (dynamic)dynamicvalues[val.ID.ToString()];
        //                        else
        //                            attributedate.Value = "-";
        //                        attributedate.IsSpecial = val.IsSpecial;


        //                        attributesWithValues.Add(attributedate);
        //                        break;

        //                    case AttributesList.ListSingleSelection:
        //                        attributedate = new AttributeData();
        //                        attributedate.ID = val.ID;
        //                        attributedate.TypeID = val.AttributeTypeID;
        //                        attributedate.Lable = val.Caption.Trim();
        //                        attributedate.IsSpecial = val.IsSpecial;
        //                        if (val.IsSpecial == true)
        //                        {
        //                            if (val.AttributeTypeID == 3)
        //                            {
        //                                var currentRole = tx.PersistenceManager.PlanningRepository.Query<EntityTypeRoleAclDao>().Where(t => t.EntityTypeID == assetObj.AssetTypeid && (EntityRoles)t.EntityRoleID == EntityRoles.Owner).SingleOrDefault();
        //                                attributedate.Value = assetObj.CreatedBy;
        //                                int value = Convert.ToInt32(attributedate.Value);
        //                                var singleCaption = (from item in tx.PersistenceManager.PlanningRepository.Query<UserDao>() where item.Id == value select item.FirstName + " " + item.LastName);
        //                                attributedate.Caption = singleCaption;
        //                            }
        //                        }
        //                        else if (val.IsSpecial == false)
        //                        {
        //                            if (dynamicvalues == null)
        //                            {
        //                                attributedate.Value = 0;
        //                                attributedate.Caption = "";
        //                            }
        //                            else
        //                            {
        //                                attributedate.Value = dynamicvalues[val.ID.ToString()] == null ? 0 : (dynamic)dynamicvalues[val.ID.ToString()];

        //                                var singleCaption = (from item in tx.PersistenceManager.PlanningRepository.Query<OptionDao>() where item.Id == Convert.ToInt32(dynamicvalues[val.ID.ToString()]) select item.Caption).ToList();
        //                                attributedate.Caption = singleCaption;
        //                            }
        //                        }

        //                        attributesWithValues.Add(attributedate);
        //                        break;

        //                    case AttributesList.ListMultiSelection:
        //                        var multiSelectValuedao = (from item in tx.PersistenceManager.PlanningRepository.Query<DamMultiSelectValueDao>()
        //                                                   where item.AssetID == assetId
        //                                                   select item).ToList();
        //                        attributedate = new AttributeData();
        //                        attributedate.ID = val.ID;
        //                        attributedate.Lable = val.Caption.Trim();
        //                        attributedate.IsSpecial = val.IsSpecial;
        //                        attributedate.TypeID = val.AttributeTypeID;
        //                        var optionIDs = (from multiValues in multiSelectValuedao where multiValues.AttributeID == val.ID select multiValues.OptionID).ToArray();
        //                        var optioncaption = (from item in tx.PersistenceManager.DamRepository.Query<OptionDao>() where optionIDs.Contains(item.Id) select item.Caption).ToList();
        //                        string Multicaptionresults = string.Join<string>(", ", optioncaption);
        //                        attributedate.Caption = Multicaptionresults;
        //                        attributedate.Value = optionIDs;


        //                        attributesWithValues.Add(attributedate);
        //                        break;

        //                    case AttributesList.DateTime:
        //                        attributedate = new AttributeData();
        //                        attributedate.Caption = val.Caption.Trim();
        //                        attributedate.ID = val.ID;
        //                        attributedate.IsSpecial = val.IsSpecial;
        //                        attributedate.TypeID = val.AttributeTypeID;
        //                        attributedate.Lable = val.Caption.Trim();
        //                        if ((object)dynamicvalues[val.ID.ToString()] != null)
        //                            attributedate.Value = (object)dynamicvalues[val.ID.ToString()];
        //                        else
        //                            attributedate.Value = null;


        //                        attributesWithValues.Add(attributedate);
        //                        break;

        //                    case AttributesList.DropDownTree:
        //                        attributedate = new AttributeData();
        //                        attributedate.ID = val.ID;
        //                        attributedate.IsSpecial = val.IsSpecial;
        //                        droplabel = new List<BrandSystems.Marcom.Core.Planning.Interface.ITreeDropDownLabel>();

        //                        var treeLevelList = tx.PersistenceManager.PlanningRepository.Query<TreeLevelDao>().Where(a => a.AttributeID == val.ID).ToList();
        //                        List<int> dropdownResults = new List<int>();
        //                        if (treevaluedao.Count > 0)
        //                        {
        //                            foreach (var lvlObj in treevaluedao)
        //                            {
        //                                treeLevelList.Remove(treeLevelList.Where(a => a.Level == lvlObj.Level).FirstOrDefault());
        //                            }
        //                            var entityTreeLevelList = treevaluedao.Select(a => a.Level).ToList();
        //                            dropdownResults = (from treevalue in treevaluedao where treevalue.Attributeid == val.ID select treevalue.Nodeid).ToList();
        //                            var nodes = (from item in tx.PersistenceManager.PlanningRepository.Query<TreeNodeDao>() where dropdownResults.Contains(item.Id) select item.Level);
        //                            var distinctNodes = nodes.Distinct();
        //                            int lastRow = 0;
        //                            foreach (var dropnode in distinctNodes)
        //                            {
        //                                BrandSystems.Marcom.Core.Planning.Interface.ITreeDropDownLabel dropdownlabel = new BrandSystems.Marcom.Core.Planning.TreeDropDownLabel();
        //                                BrandSystems.Marcom.Core.Planning.Interface.ITreeDropDownCaption treecaption = new BrandSystems.Marcom.Core.Planning.TreeDropDownCaption();
        //                                var nodelevels = (from item in tx.PersistenceManager.PlanningRepository.Query<TreeLevelDao>() where item.Level == dropnode && item.AttributeID == val.ID select item).SingleOrDefault();
        //                                treecaption.Level = nodelevels.Level;
        //                                dropdownlabel.Level = nodelevels.Level;
        //                                dropdownlabel.Label = nodelevels.LevelName.Trim();
        //                                itreeCaption.Add(treecaption);
        //                                droplabel.Add(dropdownlabel);
        //                                if (lastRow == distinctNodes.Count() - 1)
        //                                {
        //                                    foreach (var levelObj in treeLevelList)
        //                                    {
        //                                        BrandSystems.Marcom.Core.Planning.Interface.ITreeDropDownLabel dropdownlabel2 = new BrandSystems.Marcom.Core.Planning.TreeDropDownLabel();
        //                                        BrandSystems.Marcom.Core.Planning.Interface.ITreeDropDownCaption treecaption2 = new BrandSystems.Marcom.Core.Planning.TreeDropDownCaption();
        //                                        treecaption2.Level = levelObj.Level;
        //                                        dropdownlabel2.Level = levelObj.Level;
        //                                        dropdownlabel2.Label = levelObj.LevelName.Trim();
        //                                        itreeCaption.Add(treecaption2);
        //                                        droplabel.Add(dropdownlabel2);
        //                                    }
        //                                }
        //                                lastRow++;
        //                            }
        //                            attributedate.Lable = droplabel;
        //                            var captionlist = from item in tx.PersistenceManager.PlanningRepository.Query<TreeNodeDao>() where treevalues.Contains(item.Id) orderby item.Level select item.Caption;
        //                            string result = string.Join<string>(",", captionlist);
        //                            attributedate.Caption = result;
        //                            attributedate.TypeID = val.AttributeTypeID;
        //                            attributedate.Value = treevalues;
        //                            attributedate.IsInheritFromParent = val.InheritFromParent;
        //                            attributedate.IsChooseFromParent = val.ChooseFromParent;
        //                        }
        //                        else
        //                        {
        //                            foreach (var levelObj in treeLevelList)
        //                            {
        //                                BrandSystems.Marcom.Core.Planning.Interface.ITreeDropDownLabel dropdownlabel = new BrandSystems.Marcom.Core.Planning.TreeDropDownLabel();
        //                                BrandSystems.Marcom.Core.Planning.Interface.ITreeDropDownCaption treecaption = new BrandSystems.Marcom.Core.Planning.TreeDropDownCaption();
        //                                treecaption.Level = levelObj.Level;
        //                                dropdownlabel.Level = levelObj.Level;
        //                                dropdownlabel.Label = levelObj.LevelName.Trim();
        //                                itreeCaption.Add(treecaption);
        //                                droplabel.Add(dropdownlabel);
        //                            }
        //                            attributedate.Lable = droplabel;
        //                            attributedate.Caption = "-";
        //                            attributedate.TypeID = val.AttributeTypeID;
        //                            attributedate.Value = treevalues;
        //                            attributedate.IsInheritFromParent = val.InheritFromParent;
        //                            attributedate.IsChooseFromParent = val.ChooseFromParent;
        //                        }

        //                        attributesWithValues.Add(attributedate);
        //                        break;

        //                    case AttributesList.Tree:
        //                        attributedate = new AttributeData();
        //                        attributedate.ID = val.ID;
        //                        attributedate.TypeID = val.AttributeTypeID;
        //                        attributedate.IsSpecial = val.IsSpecial;
        //                        var treeCaptionList = (from item in tx.PersistenceManager.PlanningRepository.Query<TreeNodeDao>() where treevalues.Contains(item.Id) select item.Caption).ToList();
        //                        string treeCaptionResult = string.Join<string>(", ", treeCaptionList);
        //                        attributedate.Caption = treeCaptionResult;
        //                        attributedate.Lable = val.Caption.Trim();
        //                        attributedate.Value = treevalues;

        //                        attributesWithValues.Add(attributedate);
        //                        break;

        //                    case AttributesList.Link:
        //                        try
        //                        {
        //                            var linkdao = (from item in tx.PersistenceManager.PlanningRepository.Query<LinksDao>()
        //                                           where item.EntityID == assetId && item.ModuleID == 5
        //                                           select item).ToList();
        //                            attributedate = new AttributeData();
        //                            attributedate.ID = val.ID;
        //                            attributedate.Lable = val.Caption.Trim();
        //                            attributedate.IsSpecial = val.IsSpecial;
        //                            attributedate.TypeID = val.AttributeTypeID;
        //                            var linkUrl = (from item in linkdao select item.URL).ToList();
        //                            string linkurlresults = string.Join<string>(", ", linkUrl);
        //                            attributedate.Caption = linkurlresults;
        //                            var linkName = (from item in linkdao select item.Name).ToList();
        //                            string linkNameresults = string.Join<string>(", ", linkName);
        //                            var linkType = (from item in linkdao select item.LinkType.ToString()).ToList();
        //                            string linkTyperesults = string.Join<string>(", ", linkType);
        //                            attributedate.Caption = linkurlresults;
        //                            attributedate.Value = linkNameresults;
        //                            attributedate.specialValue = linkTyperesults;

        //                            attributesWithValues.Add(attributedate);
        //                        }
        //                        catch { }
        //                        break;
        //                    case AttributesList.Uploader:
        //                        attributedate = new AttributeData();
        //                        attributedate.ID = val.ID;
        //                        attributedate.TypeID = val.AttributeTypeID;
        //                        attributedate.IsSpecial = val.IsSpecial;

        //                        if (dynamicvalues != null)
        //                        {
        //                            attributedate.Caption = dynamicvalues[val.ID.ToString()] == null ? "No thumnail present" : (dynamic)dynamicvalues[val.ID.ToString()];
        //                            attributedate.Value = dynamicvalues[val.ID.ToString()] == null ? "" : (dynamic)dynamicvalues[val.ID.ToString()];
        //                        }
        //                        else
        //                        {
        //                            attributedate.Caption = "No thumnail present";
        //                            attributedate.Value = "";
        //                        }
        //                        attributedate.Lable = val.Caption.Trim();



        //                        attributesWithValues.Add(attributedate);
        //                        break;
        //                    case AttributesList.TreeMultiSelection:
        //                        attributedate = new AttributeData();
        //                        attributedate.ID = val.ID;
        //                        attributedate.IsSpecial = val.IsSpecial;


        //                        droplabel = new List<BrandSystems.Marcom.Core.Planning.Interface.ITreeDropDownLabel>();

        //                        var multiselecttreeLevelList = tx.PersistenceManager.PlanningRepository.Query<TreeLevelDao>().Where(a => a.AttributeID == val.ID).ToList();
        //                        List<int> multiselectdropdownResults = new List<int>();
        //                        if (multiselecttreevalues.Count > 0)
        //                        {
        //                            foreach (var lvlObj in multiselecttreevalues)
        //                            {
        //                                multiselecttreeLevelList.Remove(multiselecttreeLevelList.Where(a => a.Level == lvlObj.Level).FirstOrDefault());
        //                            }
        //                            var entityTreeLevelList = multiselecttreevalues.Select(a => a.Level).ToList();
        //                            multiselectdropdownResults = (from treevalue in multiselecttreevalues where treevalue.Attributeid == val.ID select treevalue.Nodeid).ToList();
        //                            var nodes = (from item in tx.PersistenceManager.PlanningRepository.Query<TreeNodeDao>() where multiselectdropdownResults.Contains(item.Id) select item.Level);
        //                            var distinctNodes = nodes.Distinct();
        //                            int lastRow = 0;
        //                            foreach (var dropnode in distinctNodes)
        //                            {
        //                                BrandSystems.Marcom.Core.Planning.Interface.ITreeDropDownLabel dropdownlabel = new BrandSystems.Marcom.Core.Planning.TreeDropDownLabel();
        //                                BrandSystems.Marcom.Core.Planning.Interface.ITreeDropDownCaption treecaption = new BrandSystems.Marcom.Core.Planning.TreeDropDownCaption();
        //                                var nodelevels = (from item in tx.PersistenceManager.PlanningRepository.Query<TreeLevelDao>() where item.Level == dropnode && item.AttributeID == val.ID select item).SingleOrDefault();
        //                                treecaption.Level = nodelevels.Level;
        //                                dropdownlabel.Level = nodelevels.Level;
        //                                dropdownlabel.Label = nodelevels.LevelName.Trim();
        //                                itreeCaption.Add(treecaption);
        //                                droplabel.Add(dropdownlabel);
        //                                if (lastRow == distinctNodes.Count() - 1)
        //                                {
        //                                    foreach (var levelObj in multiselecttreeLevelList)
        //                                    {
        //                                        BrandSystems.Marcom.Core.Planning.Interface.ITreeDropDownLabel dropdownlabel2 = new BrandSystems.Marcom.Core.Planning.TreeDropDownLabel();
        //                                        BrandSystems.Marcom.Core.Planning.Interface.ITreeDropDownCaption treecaption2 = new BrandSystems.Marcom.Core.Planning.TreeDropDownCaption();
        //                                        treecaption2.Level = levelObj.Level;
        //                                        dropdownlabel2.Level = levelObj.Level;
        //                                        dropdownlabel2.Label = levelObj.LevelName.Trim();
        //                                        itreeCaption.Add(treecaption2);
        //                                        droplabel.Add(dropdownlabel2);
        //                                    }
        //                                }
        //                                lastRow++;
        //                            }
        //                            attributedate.Lable = droplabel;
        //                            attributedate.Caption = (from item in tx.PersistenceManager.PlanningRepository.Query<TreeNodeDao>() where temptreevalues.Contains(item.Id) orderby item.Level select item.Caption).ToList();
        //                            attributedate.TypeID = val.AttributeTypeID;
        //                            attributedate.Value = multiselecttreevalues;
        //                            attributedate.IsInheritFromParent = val.InheritFromParent;
        //                            attributedate.IsChooseFromParent = val.ChooseFromParent;
        //                        }
        //                        else
        //                        {
        //                            foreach (var levelObj in multiselecttreeLevelList)
        //                            {
        //                                BrandSystems.Marcom.Core.Planning.Interface.ITreeDropDownLabel dropdownlabel = new BrandSystems.Marcom.Core.Planning.TreeDropDownLabel();
        //                                BrandSystems.Marcom.Core.Planning.Interface.ITreeDropDownCaption treecaption = new BrandSystems.Marcom.Core.Planning.TreeDropDownCaption();
        //                                treecaption.Level = levelObj.Level;
        //                                dropdownlabel.Level = levelObj.Level;
        //                                dropdownlabel.Label = levelObj.LevelName.Trim();
        //                                itreeCaption.Add(treecaption);
        //                                droplabel.Add(dropdownlabel);
        //                            }
        //                            attributedate.Lable = droplabel;
        //                            attributedate.Caption = "-";
        //                            attributedate.TypeID = val.AttributeTypeID;
        //                            attributedate.Value = multiselecttreevalues;
        //                            attributedate.IsInheritFromParent = val.InheritFromParent;
        //                            attributedate.IsChooseFromParent = val.ChooseFromParent;
        //                        }
        //                        attributesWithValues.Add(attributedate);
        //                        break;

        //                    default:

        //                        break;
        //                }
        //                tx.Commit();
        //            }
        //            asset.AttributeData = attributesWithValues;

        //            return asset;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}



        public IAssets ChangeAssettype(DigitalAssetManagerProxy proxy, int assetTypeID, int assetId, bool IsforAdmin = false)
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
                    asset.AssetTypeid = assetTypeID;
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
                                where Convert.ToInt32(EntityAttrRel.Element("EntityTypeID").Value) == assetTypeID
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

                    var assetName = GetAssetName(tx, assetId, 1);
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
                                        var currentRole = tx.PersistenceManager.PlanningRepository.Query<EntityTypeRoleAclDao>().Where(t => t.EntityTypeID == assetTypeID && (EntityRoles)t.EntityRoleID == EntityRoles.Owner).SingleOrDefault();
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


        public IList<IDamTypeAttributeRelation> GetAssetAttributueswithTypeID(DigitalAssetManagerProxy proxy, int assetTypeID)
        {
            try
            {

                IList data = null;
                int version = MarcomManagerFactory.ActiveMetadataVersionNumber;
                IList<IDamTypeAttributeRelation> _iiDamtoAttrRel = new List<IDamTypeAttributeRelation>();
                IList<DamTypeAttributeRelation> dao = new List<DamTypeAttributeRelation>();
                IList<EntityTypeAttributeRelationDao> daoAttrType = new List<EntityTypeAttributeRelationDao>();
                // var xmetadataDoc = null;
                string xmlpath = string.Empty;
                using (ITransaction tx = proxy.MarcomManager.GetTransaction())
                {
                    xmlpath = tx.PersistenceManager.MetadataRepository.GetXmlPath(version);
                    daoAttrType = tx.PersistenceManager.MetadataRepository.GetObject<EntityTypeAttributeRelationDao>(xmlpath);
                    var entityttyperesult = daoAttrType.Where(a => a.EntityTypeID == assetTypeID).OrderBy(x => x.SortOrder);
                    var attrIDs = entityttyperesult.Select(a => a.AttributeID).ToList();
                    IList<IOption> optionSelection = GetOptionList(proxy, attrIDs);
                    //string Adminxmlpath = Path.Combine(HttpRuntime.AppDomainAppPath, "AdminSettings.xml");
                    //XDocument xDoc = XDocument.Load(Adminxmlpath);


                    //var adminxdoc = from item in xDoc.Root.Elements("DAMsettings").Elements("AssetCreation").Elements("AssetType") where Convert.ToInt32(item.Attribute("ID").Value) == damID select item;

                    XDocument metadataxDoc = XDocument.Load(xmlpath);
                    //var rddd1 = (from AdminAttributes in adminxdoc.Elements("Attributes").Elements("Attribute")
                    //            join MetadataAttributes in metadataxDoc.Root.Elements("EntityTypeAttributeRelation_Table").Elements("EntityTypeAttributeRelation") on damID equals Convert.ToInt32(MetadataAttributes.Element("EntityTypeID").Value)
                    //            where Convert.ToInt32(AdminAttributes.Element("Id").Value) == Convert.ToInt32(MetadataAttributes.Element("AttributeID").Value)
                    //            select new
                    //            {
                    //                ID = Convert.ToInt16(AdminAttributes.Element("Id").Value),
                    //                Caption = Convert.ToString(MetadataAttributes.Element("Caption").Value),
                    //                SotOrder = Convert.ToInt32(AdminAttributes.Element("SortOrder").Value),
                    //                DefaultValue = Convert.ToString(MetadataAttributes.Element("DefaultValue").Value)
                    //            }).ToList();

                    var rddd = (from MetadataAttributes in metadataxDoc.Root.Elements("EntityTypeAttributeRelation_Table").Elements("EntityTypeAttributeRelation")
                                where Convert.ToInt32(MetadataAttributes.Element("EntityTypeID").Value) == Convert.ToInt32(assetTypeID)
                                select new
                                {
                                    ID = Convert.ToInt16(MetadataAttributes.Element("AttributeID").Value),
                                    Caption = Convert.ToString(MetadataAttributes.Element("Caption").Value),
                                    SotOrder = Convert.ToInt32(MetadataAttributes.Element("SortOrder").Value),
                                    DefaultValue = Convert.ToString(MetadataAttributes.Element("DefaultValue").Value)
                                }).ToList();


                    //Get User Details metadata values
                    IList<IAttributeData> entityUserAttrVal = new List<IAttributeData>();
                    entityUserAttrVal = proxy.MarcomManager.PlanningManager.GetEntityAttributesDetailsUserDetails(proxy.MarcomManager.User.Id);

                    foreach (var it in rddd)
                    {
                        string InheritVal = "";
                        int typeID = Convert.ToInt32(metadataxDoc.Root.Elements("Attribute_Table").Elements("Attribute").Where(a => Convert.ToInt32(a.Element("ID").Value) == Convert.ToInt32(it.ID)).Select(a => a.Element("AttributeTypeID").Value).First());
                        IList<IOption> optionSinglrSelection = null;
                        // IList<IOption> optionmultiselection = null;
                        if (typeID == (int)AttributesList.ListSingleSelection)
                        {
                            optionSinglrSelection = (from options in optionSelection
                                                     where options.AttributeID == it.ID
                                                     select options).OrderBy(a => a.Caption).ToList<IOption>();
                        }
                        else if (typeID == (int)AttributesList.ListMultiSelection)
                        {
                            optionSinglrSelection = (from options in optionSelection
                                                     where options.AttributeID == it.ID
                                                     select options).OrderBy(a => a.SortOrder).ToList<IOption>();
                        }

                        //If asset type value is null call the user metadata value
                        if (it.DefaultValue == "")
                        {
                            if ((entityUserAttrVal.Where(a => a.ID == it.ID).Select(a => a.Value).ToList().Count) > 0)
                            {
                                InheritVal = (string)entityUserAttrVal.Where(a => a.ID == it.ID).Select(a => Convert.ToString(a.Value)).FirstOrDefault();
                            }
                        }
                        else
                            InheritVal = Convert.ToString(it.DefaultValue);

                        _iiDamtoAttrRel.Add(new DamTypeAttributeRelation { ID = Convert.ToInt32(it.ID), Caption = Convert.ToString(it.Caption), SortOrder = Convert.ToInt32(it.SotOrder), TypeID = typeID, Options = optionSinglrSelection, DefaultValue = InheritVal });
                    }
                }
                return _iiDamtoAttrRel.OrderBy(a => a.SortOrder).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int SaveAssetTypeofAsset(DigitalAssetManagerProxy digitalAssetManagerProxy, int typeid, string Name, IList<IAttributeData> listattributevalues, int assetID, int oldAssetTypeId, bool IsforAdmin = false)
        {
            try
            {
                int version = MarcomManagerFactory.ActiveMetadataVersionNumber;
                using (ITransaction tx = digitalAssetManagerProxy.MarcomManager.GetTransaction())
                {
                    DateTime updatedtime  = DateTime.UtcNow;
                    StringBuilder updateAsset = new StringBuilder();
                    updateAsset.AppendLine("UPDATE DAM_Asset");
                    updateAsset.AppendLine("SET	AssetTypeid = " + typeid + ",");
                    updateAsset.AppendLine("	Name = '" + Name + "'");
                    updateAsset.AppendLine("	,UpdatedOn = '" + updatedtime + "'");
                    updateAsset.AppendLine("WHERE ID = " + assetID);
                    tx.PersistenceManager.PlanningRepository.ExecuteQuery(updateAsset.ToString());


                    StringBuilder deletedynAttribute = new StringBuilder();
                    deletedynAttribute.AppendLine("DELETE FROM MM_AttributeRecord_" + oldAssetTypeId);
                    deletedynAttribute.AppendLine("WHERE ID = " + assetID);
                    tx.PersistenceManager.PlanningRepository.ExecuteQuery(deletedynAttribute.ToString());

                    if (IsforAdmin)
                        version = MarcomManagerFactory.AdminMetadataVersionNumber;
                    string xmlpath = tx.PersistenceManager.MetadataRepository.GetXmlPath(version);
                    XDocument docx = XDocument.Load(xmlpath);
                    var rddd = (from EntityAttrRel in docx.Root.Elements("EntityTypeAttributeRelation_Table").Elements("EntityTypeAttributeRelation")
                                join Attr in docx.Root.Elements("Attribute_Table").Elements("Attribute") on Convert.ToInt32(EntityAttrRel.Element("AttributeID").Value) equals Convert.ToInt32(Attr.Element("ID").Value)
                                where Convert.ToInt32(EntityAttrRel.Element("EntityTypeID").Value) == typeid && EntityAttrRel.Element("DefaultValue").Value != ""
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
                                    IsReadOnly = Convert.ToBoolean(Convert.ToInt32(EntityAttrRel.Element("IsReadOnly").Value)),
                                    DefaultValue = EntityAttrRel.Element("DefaultValue").Value
                                }).ToList();

                    var attributesdetails = rddd;

                    List<int> generalAttributes = new List<int>();
                    generalAttributes = listattributevalues.Select(a => a.ID).ToList();
                    generalAttributes.Add((int)SystemDefinedAttributes.MyRoleEntityAccess);
                    generalAttributes.Add((int)SystemDefinedAttributes.MyRoleGlobalAccess);
                    generalAttributes.Add((int)SystemDefinedAttributes.EntityOnTimeStatus);
                    generalAttributes.Add((int)SystemDefinedAttributes.Name);
                    generalAttributes.Add((int)SystemDefinedAttributes.EntityStatus);

                    attributesdetails = attributesdetails.Where(a => !generalAttributes.Contains(a.ID)).ToList();
                    if (attributesdetails != null)
                    {

                        foreach (var item in attributesdetails)
                        {
                            string[] defDatalist = item.DefaultValue.Split(',').ToArray();
                            foreach (var defData in defDatalist)
                            {
                                IAttributeData newdata = new AttributeData();
                                newdata.AttributeRecordID = 0;
                                newdata.Caption = item.Caption;
                                newdata.DropDownPricing = null;
                                newdata.ID = item.ID;
                                newdata.IsChooseFromParent = false;
                                newdata.IsInheritFromParent = false;
                                newdata.IsLock = false;
                                newdata.IsReadOnly = false;
                                newdata.IsSpecial = false;
                                newdata.Lable = null;
                                newdata.Level = 0;
                                newdata.SortOrder = 0;
                                newdata.specialValue = null;
                                newdata.SpecialValue = null;
                                newdata.tree = null;
                                newdata.TypeID = item.AttributeTypeID;
                                newdata.Value = defData;
                                listattributevalues.Add(newdata);
                            }
                        }
                    }

                    if (listattributevalues != null)
                    {
                        var result = InsertAssetAttributes(tx, listattributevalues, assetID, typeid, 0, false);
                    }
                    tx.Commit();
                }
                return assetID;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
