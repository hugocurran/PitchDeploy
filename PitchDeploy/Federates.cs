using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace PitchDeploy
{
    public class Federates
    {
        public List<string> Federate { get; set; }

        public XElement ToHPSD()
        {
            XElement hpsd = null;
            foreach (string fed in Federate)
                hpsd.Add(new XElement("federate", fed));

            return hpsd;
        }
    }
}
