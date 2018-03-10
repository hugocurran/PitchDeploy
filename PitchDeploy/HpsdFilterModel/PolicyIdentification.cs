using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace PitchDeploy.HpsdFilterModel
{
    public class PolicyIdentification
    {

        public string PolicyName { get; set; }
        public string PolicyVersion { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public List<POC> Poc { get; set; }

        public PolicyIdentification()
        {
            Poc = new List<POC>();
        }

        public XElement ToHPSD(XNamespace ns)
        {
            string[] d1 = XmlConvert.ToString(CreatedDate, XmlDateTimeSerializationMode.Utc).Split('T');
            string[] d2 = XmlConvert.ToString(ModifiedDate, XmlDateTimeSerializationMode.Utc).Split('T');
            XElement hpsd = new XElement(ns + "policyIdentification",
                new XElement(ns + "name", PolicyName),
                new XElement(ns + "version", PolicyVersion),
                new XElement(ns + "createdDate", d1[0]),
                new XElement(ns + "modifiedDate", d2[0]),
                new XElement(ns + "status", Status),
                new XElement(ns + "description", Description)
                );
            foreach (POC poc in Poc)
                hpsd.Element(ns + "policyIdentification").Add(poc.ToHPSD(ns));
            return hpsd;
        }
    }
}
