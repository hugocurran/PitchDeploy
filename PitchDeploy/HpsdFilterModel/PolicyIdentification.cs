using System;
using System.Collections.Generic;
using System.Text;
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
            XElement hpsd = new XElement(ns + "policyIdentification",
                new XElement(ns + "name", PolicyName),
                new XElement(ns + "version", PolicyVersion),
                new XElement(ns + "createdDate", CreatedDate.ToShortDateString()),
                new XElement(ns + "modifiedDate", ModifiedDate.ToShortDateString()),
                new XElement(ns + "status", Status),
                new XElement(ns + "description", Description)
                );
            foreach (POC poc in Poc)
                hpsd.Element(ns + "policyIdentification").Add(poc.ToHPSD(ns));
            return hpsd;
        }
    }
}
