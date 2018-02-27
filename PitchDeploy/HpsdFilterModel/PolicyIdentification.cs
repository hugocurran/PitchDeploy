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

        public XElement ToHPSD()
        {
            XElement hpsd = new XElement("policyIdentification",
                new XElement("name", PolicyName),
                new XElement("version", PolicyVersion),
                new XElement("createdDate", CreatedDate.ToShortDateString()),
                new XElement("modifiedDate", ModifiedDate.ToShortDateString()),
                new XElement("status", Status),
                new XElement("description", Description)
                );
            foreach (POC poc in Poc)
                hpsd.Element("policyIdentification").Add(poc.ToHPSD());
            return hpsd;
        }
    }
}
