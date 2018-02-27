using FPDL.Common;
using FPDL.Deploy;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace PitchDeploy.HpsdFilterModel
{
    public class HpsdPolicy
    {

        public PolicyIdentification PolicyIdentification { get; set; }
        public FomInformation FomInformation { get; set; }
        public Federates Federates { get; set; }
        public PolicyRules PolicyRules { get; set; }

        public HpsdPolicy()
        {
            // Initialise all the data structures
            //PolicyIdentification = new PolicyIdentification();
            FomInformation = new FomInformation();
            Federates = new Federates();
            PolicyRules = new PolicyRules();
        }

        public XElement ToHPSD()
        {
            XElement hpsd = new XElement("hpsdPolicy",
                PolicyIdentification.ToHPSD(),
                FomInformation.ToHPSD(),
                Federates.ToHPSD(),
                PolicyRules.ToHPSD());
            return hpsd;
        }


    }
}

