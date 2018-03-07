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
            PolicyIdentification = new PolicyIdentification();
            FomInformation = new FomInformation();
            Federates = new Federates();
            PolicyRules = new PolicyRules();
        }

        public XElement ToHPSD()
        {

            XNamespace xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");
            XNamespace xsd = XNamespace.Get("http://www.w3.org/2001/XMLSchema");
            XNamespace ns = XNamespace.Get("http://www.pitchtechnologies.com/schemas/hpsd081");
            XElement hpsd = new XElement(ns + "hpsdPolicy",
                    new XAttribute("xmlns", ns.NamespaceName),
                    new XAttribute(XNamespace.Xmlns + "xsd", xsd.NamespaceName),
                    new XAttribute(XNamespace.Xmlns + "xsi", xsi.NamespaceName)
                );
            hpsd.Add(PolicyIdentification.ToHPSD(ns));
            hpsd.Add(FomInformation.ToHPSD(ns));
            hpsd.Add(Federates.ToHPSD(ns));
            hpsd.Add(PolicyRules.ToHPSD(ns));
            return hpsd;
        }


    }
}

