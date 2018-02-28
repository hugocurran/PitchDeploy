using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace PitchDeploy.HpsdFilterModel
{
    public class PolicyRules
    {
        public List<SessionStatusReleaseRule> SessionStatus { get; set; }
        public List<ObjectReleaseReplaceRule> ObjectRelease { get; set; }
        public List<InteractionReleaseReplaceRule> InteractionRelease { get; set; }

        public PolicyRules()
        {
            SessionStatus = new List<SessionStatusReleaseRule>();
            ObjectRelease = new List<ObjectReleaseReplaceRule>();
            InteractionRelease = new List<InteractionReleaseReplaceRule>();
        }

        public XElement ToHPSD()
        {
            XElement hpsd = new XElement("policyRules");
            foreach (var session in SessionStatus)
                hpsd.Add(session.ToHPSD());
            foreach (var obj in ObjectRelease)
                hpsd.Add(obj.ToHPSD());
            foreach (var inter in InteractionRelease)
                hpsd.Add(inter.ToHPSD());
            return hpsd;
        }

    }

    public class SessionStatusReleaseRule
    {
        public string Condition_SessionName { get; set; }
        public string RuleName { get; set; }

        public XElement ToHPSD()
        {
            XElement hpsd = new XElement("sessionStatusReleaseRule",
                new XAttribute("name", RuleName),
                new XElement("condition",
                    new XElement("sessionName", Condition_SessionName)),
                new XElement("release")
                );
            return hpsd;
        }

    }

    public class ObjectReleaseReplaceRule
    {
        public string Condition_ProducingFederate { get; set; }
        public string Condition_ObjectClass { get; set; }
        public string Condition_InstanceID { get; set; }
        public List<string> Release_Attribute { get; set; }
        public string RuleName { get; set; }

        public XElement ToHPSD()
        {
            XElement hpsd = new XElement("objectReleaseReplaceRule",
                new XAttribute("name", RuleName),
                new XElement("condition",
                    new XElement("producingFederate", Condition_ProducingFederate),
                    new XElement("objectClass", Condition_ObjectClass),
                    new XElement("instanceID", Condition_InstanceID))
                );
            XElement release = new XElement("release");
            foreach (string rel in Release_Attribute)
                release.Add(new XElement("attribute",
                    new XElement("name", rel)));
            hpsd.Element("objectReleaseReplaceRule").Add(release);
            return hpsd;
        }

    }

    public class InteractionReleaseReplaceRule
    {
        public string Condition_ProducingFederate { get; set; }
        public string Condition_InteractionClass { get; set; }
        public List<string>Release_Parameter { get; set; }
        public string RuleName { get; set; }

        public XElement ToHPSD()
        {
            XElement hpsd = new XElement("interactionReleaseReplaceRule",
                new XAttribute("name", RuleName),
                new XElement("condition",
                    new XElement("producingFederate", Condition_ProducingFederate),
                    new XElement("interactionClass", Condition_InteractionClass))
                );
            XElement release = new XElement("release");
            foreach (string rel in Release_Parameter)
                release.Add(new XElement("parameter",
                    new XElement("name", rel)));
            hpsd.Element("interactionReleaseReplaceRule").Add(release);
            return hpsd;
        }

    }
}
