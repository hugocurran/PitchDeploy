using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace PitchDeploy.HpsdFilterModel
{
    public class FomInformation
    {
        public List<string> FomModule { get; set; }
        public List<InstanceId> InstanceID { get; set; }

        public FomInformation()
        {
            FomModule = new List<string>();
            InstanceID = new List<InstanceId>();
        }

        public XElement ToHPSD(XNamespace ns)
        {
            XElement hpsd = new XElement(ns+"fomInformation");
            foreach (string fomModule in FomModule)
                hpsd.Add(new XElement(ns+"fomModule", fomModule));
            foreach (var inst in InstanceID)
                hpsd.Add(inst.ToHPSD(ns));
            return hpsd;
        }
    }

    public class InstanceId
    {
        public string ObjectClass { get; set; }
        public string Attribute_Name { get; set; }
        public string Attribute_Encoding { get; set; }

        public XElement ToHPSD(XNamespace ns)
        {
            XElement hpsd = new XElement(ns + "instanceIdAttribute",
                new XElement(ns + "objectClass", ObjectClass),
                    new XElement(ns + "attribute",
                        new XElement(ns + "name", Attribute_Name),
                        new XElement(ns + "encoding", Attribute_Encoding)
                    )
                );
            return hpsd;
        }
    }
}
