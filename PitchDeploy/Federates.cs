using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace PitchDeploy
{
    public class Federates
    {
        public List<string> Federate { get; set; }

        public Federates()
        {
            Federate = new List<string> ();
        }

        public XElement ToHPSD(XNamespace ns)
        {
            XElement hpsd = new XElement(ns + "federates");
            foreach (string fed in Federate)
                hpsd.Add(new XElement(ns + "federate", fed));

            return hpsd;
        }
    }
}
