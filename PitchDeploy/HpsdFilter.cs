using System;
using System.Collections.Generic;
using System.Text;

namespace PitchDeploy
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
    }
}
