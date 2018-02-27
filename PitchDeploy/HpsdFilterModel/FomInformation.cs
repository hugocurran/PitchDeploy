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

        public XElement ToHPSD()
        {
            XElement hpsd = new XElement("fomInformation");
            foreach (string fomModule in FomModule)
                hpsd.Add(new XElement("fomModule", fomModule));
            foreach (var inst in InstanceID)
                hpsd.Add(inst.ToHPSD());
            return hpsd;
        }
    }

    public class InstanceId
    {
        public string ObjectClass { get; set; }
        public string Attribute_Name { get; set; }
        public string Attribute_Encoding { get; set; }

        public XElement ToHPSD()
        {
            XElement hpsd = new XElement("instanceIdAttribute",
                new XElement("objectClass", ObjectClass),
                    new XElement("attribute",
                        new XElement("name", Attribute_Name),
                        new XElement("encoding", Attribute_Encoding)
                    )
                );
            return hpsd;
        }
    }
}
