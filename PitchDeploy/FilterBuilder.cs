using FPDL.Common;
using FPDL.Deploy;
using PitchDeploy.HpsdFilterModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace PitchDeploy
{
    internal static class FilterBuilder
    {

        public static XElement Create(DeployObject deploy, Component component)
        {
            HpsdPolicy hpsdPolicy = new HpsdPolicy();

            // We can assume that there is only one System in the deploy doc
            DeploySystem system = deploy.Systems[0];


            // PolicyIdentification <--> ConfigMgmt
            PolicyIdentification polID = new PolicyIdentification
            {
                PolicyName = system.FederateName,
                PolicyVersion = ConfigMgmt.VersionToString(deploy.ConfigMgmt.CurrentVersion),
                CreatedDate = deploy.ConfigMgmt.Created.date,
                ModifiedDate = deploy.ConfigMgmt.Changed[deploy.ConfigMgmt.Changed.Count - 1].date,
                Status = "",
                Description = deploy.ConfigMgmt.Description
            }; // No POC information

            hpsdPolicy.PolicyIdentification = polID;
            // =======================================

            // FomInformation <--> Federation, Extensions
            FomInformation fominfo = new FomInformation();

            // List of fomModule in Federation module
            ModuleFederation fedModule = (ModuleFederation)findModule(component.Modules, Enums.ModuleType.federation);
            foreach (string file in fedModule.RTI.FomFile)
                fominfo.FomModule.Add(file);

            // InstanceIdAttributes are a Vendor extension
            ModuleExtension extModule = (ModuleExtension)findModule(component.Modules, Enums.ModuleType.extension);
            if (extModule.VendorName != "Pitch Technologies")
                throw new ApplicationException("Pitch Technologies vendor extensions not found");
            foreach(KeyValuePair<string, string> p in extModule.Parameters)
            {
                // General format key = <identifier>:<parameterName>
                // we expect: key = instanceIdAttribute:<fully qualified attribute>
                string[] sa1 = p.Key.Split(':');
                if (sa1[0] == "instanceIdAttribute")
                {
                    string attribName = sa1[1].Split('.').Last();
                    string objclass = sa1[1].Substring(0, attribName.Length + 1);  // trailing '.'
                    string encoding = p.Value;
                    InstanceId instId = new InstanceId
                    {
                        ObjectClass = objclass,
                        Attribute_Name = attribName,
                        Attribute_Encoding = encoding
                    };
                    fominfo.InstanceID.Add(instId);
                }
            }
            hpsdPolicy.FomInformation = fominfo;
            // =====================================================================================

            // Federates <--->  Export module
            Federates feds = new Federates();

            // List of federates in Export module
            ModuleExport expModule = (ModuleExport)findModule(component.Modules, Enums.ModuleType.federation);
            foreach (Source source in expModule.Sources)
                feds.Federate.Add(source.FederateName);

            hpsdPolicy.Federates = feds;
            // =====================================================================================

            // PolicyRules <----> Export module
            PolicyRules rule = new PolicyRules();
            int ruleNumber = 0;

            // Status message - session name = federateName
            SessionStatusReleaseRule sess = new SessionStatusReleaseRule
            {
                Condition_SessionName = system.FederateName,
                RuleName = ruleNumber++.ToString()
            };
            rule.SessionStatus.Add(sess);

            // Object update/create
            foreach (Source source in expModule.Sources)
            {
                
                string federateName = source.FederateName;
                string entityID = "";
                if (source.SourceType == Source.Type.Federate)
                    entityID = "*";
                else
                    entityID = source.EntityId;

                foreach(HlaObject hlaObj in source.Objects)
                {
                    ObjectReleaseReplaceRule obj = new ObjectReleaseReplaceRule()
                    {
                        Condition_ProducingFederate = federateName,
                        Condition_InstanceID = entityID,
                        RuleName = ruleNumber++.ToString(),
                        Condition_ObjectClass = hlaObj.ObjectClassName.NoHlaRoot()
                    };
                    foreach (HlaAttribute attrib in hlaObj.Attributes)
                        obj.Release_Attribute.Add(attrib.AttributeName);
                    rule.ObjectRelease.Add(obj);
                }

                foreach (HlaInteraction hlaInt in source.Interactions)
                {
                    InteractionReleaseReplaceRule intr = new InteractionReleaseReplaceRule()
                    {
                        Condition_ProducingFederate = federateName,
                        RuleName = ruleNumber++.ToString(),
                        Condition_InteractionClass = hlaInt.InteractionClassName.NoHlaRoot()
                    };
                    foreach (HlaParameter para in hlaInt.Parameters)
                        intr.Release_Parameter.Add(para.ParameterName);
                    rule.InteractionRelease.Add(intr);
                }
            }
            return rule.ToHPSD();
        }

        private static IModule findModule(List<IModule> modules, Enums.ModuleType moduleType)
        {
            foreach (var module in modules)
            {
                if (module.GetModuleType() == moduleType)
                    return module;
            }
            return null;
        }
    }
}
