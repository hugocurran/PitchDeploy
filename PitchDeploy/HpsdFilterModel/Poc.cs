using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace PitchDeploy.HpsdFilterModel
{
    public class POC
    {
        public string PocName { get; private set; }
        public string PocType { get; private set; }
        public string PocOrg { get; private set; }
        public string PocEmail { get; private set; }
        public string PocPhone { get; private set; }

        public POC(string pocName, string pocType, string pocOrg, string pocEmail, string pocPhone)
        {
            PocName = pocName;
            PocType = pocType;
            PocOrg = pocOrg;
            PocEmail = pocEmail;
            PocPhone = pocPhone;
        }

        public XElement ToHPSD()
        {
            XElement hpsd = new XElement("poc",
                new XElement("pocName", PocName),
                new XElement("pocType", PocType),
                new XElement("pocOrg", PocOrg),
                new XElement("pocEmail", PocEmail),
                new XElement("pocPhone", PocPhone)
                );
            return hpsd;
        }
    }
}
