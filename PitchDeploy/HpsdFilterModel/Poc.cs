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

        public XElement ToHPSD(XNamespace ns)
        {
            XElement hpsd = new XElement(ns + "poc",
                new XElement(ns + "pocName", PocName),
                new XElement(ns + "pocType", PocType),
                new XElement(ns + "pocOrg", PocOrg),
                new XElement(ns + "pocEmail", PocEmail),
                new XElement(ns + "pocPhone", PocPhone)
                );
            return hpsd;
        }
    }
}
