using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace UpgradeTool
{
   [Serializable, System.Xml.Serialization.XmlRoot("DevelopmentVersions")]
    public class DevelopmentVersions
    {
       [XmlElement]
       public List<Version> Version { get; set; }

    }
}