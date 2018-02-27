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
            };
            // No POC information
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
