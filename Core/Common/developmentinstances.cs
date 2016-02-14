using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace UpgradeTool
{
    [Serializable, System.Xml.Serialization.XmlRoot("Developmentinstances")]
    public class Developmentinstances
    {
        [XmlElement]
        public List<instance> instance { get; set; }
    }
}