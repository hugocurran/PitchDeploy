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

        public XElement ToHPSD(XNamespace ns)
        {
            XElement hpsd = new XElement(ns + "policyRules");

            foreach (var session in SessionStatus)
                hpsd.Add(session.ToHPSD(ns));
            foreach (var obj in ObjectRelease)
                hpsd.Add(obj.ToHPSD(ns));
            foreach (var inter in InteractionRelease)
                hpsd.Add(inter.ToHPSD(ns));
            return hpsd;
        }

    }

    public class SessionStatusReleaseRule
    {
        public string Condition_SessionName { get; set; }
        public string RuleName { get; set; }

        public XElement ToHPSD(XNamespace ns)
        {
            XElement hpsd = new XElement(ns + "sessionStatusReleaseRule",
                new XAttribute("name", RuleName),
                new XElement(ns + "condition",
                    new XElement(ns + "sessionName", Condition_SessionName)),
                new XElement(ns + "release")
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

        public ObjectReleaseReplaceRule()
        {
            Release_Attribute = new List<string>();
        }

        public XElement ToHPSD(XNamespace ns)
        {
            XElement hpsd = new XElement(ns + "objectReleaseReplaceRule",
                new XAttribute("name", RuleName),
                new XElement(ns + "condition",
                    new XElement(ns + "producingFederate", Condition_ProducingFederate),
                    new XElement(ns + "objectClass", Condition_ObjectClass),
                    new XElement(ns + "instanceID", Condition_InstanceID))
                );
            XElement release = new XElement(ns + "release");
            foreach (string rel in Release_Attribute)
                release.Add(new XElement(ns + "attribute",
                    new XElement(ns + "name", rel)));
            hpsd.Add(release);
            return hpsd;
        }

    }

    public class InteractionReleaseReplaceRule
    {
        public string Condition_ProducingFederate { get; set; }
        public string Condition_InteractionClass { get; set; }
        public List<string>Release_Parameter { get; set; }
        public string RuleName { get; set; }

        public InteractionReleaseReplaceRule()
        {
            Release_Parameter = new List<string>();
        }

        public XElement ToHPSD(XNamespace ns)
        {
            XElement hpsd = new XElement(ns + "interactionReleaseReplaceRule",
                new XAttribute("name", RuleName),
                new XElement(ns + "condition",
                    new XElement(ns + "producingFederate", Condition_ProducingFederate),
                    new XElement(ns + "interactionClass", Condition_InteractionClass))
                );
            XElement release = new XElement(ns + "release");
            foreach (string rel in Release_Parameter)
                release.Add(new XElement(ns + "parameter",
                    new XElement(ns + "name", rel)));
            hpsd.Add(release);
            return hpsd;
        }

    }
}
