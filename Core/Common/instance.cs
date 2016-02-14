using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace UpgradeTool
{
    //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    //[XmlRoot("instance")]
    [Serializable]
    public class instance
    {
        //[XmlAttribute(AttributeName = "Id")]
        public int Id { get; set; }
        //[XmlAttribute(AttributeName = "ClientName")]

        public string ClientName { get; set; }
        //[XmlAttribute(AttributeName = "Link")]
        public string Link { get; set; }
        //[XmlAttribute(AttributeName = "CurrentVersionId")]
        public int CurrentVersionId { get; set; }
       // [XmlAttribute(AttributeName = "IISPhyPath")]
        public string IISPhyPath { get; set; }
       // [XmlAttribute(AttributeName = "dbserver")]
        public string dbserver { get; set; }
       // [XmlAttribute(AttributeName = "dbname")]
        public string dbname { get; set; }
        public string applicationPoolName { get; set; }
        public string dbUsername { get; set; }
        public string dbPassword { get; set; }
       // [XmlAttribute(AttributeName = "connectionstring")]
        public string connectionstring { get; set; }
        public string Versionname { get; set; }
        public string latestUpdateAt { get; set; }
        public string folderdetails { get; set; }
        public string PreviousVersion { get; set; }
        public string dbBackupPath { get; set; }
        public string applicationBackupPath { get; set; }
        
    }   
}