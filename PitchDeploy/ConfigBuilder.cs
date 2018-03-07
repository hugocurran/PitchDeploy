using FPDL.Common;
using FPDL.Deploy;
using PitchDeploy.HlaTreeWalker;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;

namespace PitchDeploy
{
    public static class ConfigBuilder
    {
        public static MemoryStream CreateStreamFromString(string value)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(value ?? ""));
        }

        public static string Create(DeployObject deploy, Component component)
        {
            // Find the modules we need
            ModuleFederation federation = (ModuleFederation)findModule(component.Modules, Enums.ModuleType.federation);
            ModuleImport import = (ModuleImport)findModule(component.Modules, Enums.ModuleType.import);
            ModuleExport export = (ModuleExport)findModule(component.Modules, Enums.ModuleType.export);
            ModuleExtension extension = (ModuleExtension)findModule(component.Modules, Enums.ModuleType.extension);
            ModuleOsp[] osp = findModules(component.Modules);

            // Create an HlaObject tree to use when defining attribute lists
            //MemoryStream ms = CreateStreamFromString(Properties.Resources.HLAfeatures);
            StringReader ms = new StringReader(Properties.Resources.HLAfeatures);
            HlaObjectNode hlaObjectTree = TreeReader.CreateTree(ms);
            
            StringBuilder sb = new StringBuilder();

            // Side A - always the side connected to a federation
            sb.AppendLine("# A side settings (Federation Side)");
            sb.AppendLine("A.side_name=" + federation.FederationName);
            sb.AppendLine("A.description=Federate " + federation.FederateName + " on " + federation.FederationName + " federation");
            switch(federation.RTI.HlaSpec)
            {
                case "Pitch pRTI 1516":
                    sb.AppendLine("A.profile=A");
                    break;
                case "HLA Evolved":
                    sb.AppendLine("A.profile=B");
                    break;
                case "Pitch pRTI 1.3":
                    sb.AppendLine("A.profile=C");
                    break;
                case "RTI NG":
                    sb.AppendLine("A.profile=D");
                    break;
                case "MAK 1.3 RTI":
                    sb.AppendLine("A.profile=E");
                    break;
                case "MAK 1516 RTI":
                    sb.AppendLine("A.profile=F");
                    break;
                case "RTI NG C++":
                    sb.AppendLine("A.profile=G");
                    break;
                case "RTI DLC C++":
                    sb.AppendLine("A.profile=H");
                    break;
                case "Evolved C++":
                    sb.AppendLine("A.profile=I");
                    break;
                default:
                    sb.AppendLine("A.profile=B");
                    break;
            }
            sb.AppendLine("A.configurationType=" + extension.Parameters["configurationType"]);
            sb.AppendLine("A.host=" + federation.RTI.CrcAddress);
            sb.AppendLine("A.port=" + federation.RTI.CrcPortNumber);
            sb.AppendLine("A.designator=");
            sb.AppendLine("A.federation=" + federation.FederationName);
            sb.AppendLine("A.type=" + deploy.Systems[0].SystemType.ToString());
            sb.AppendLine("A.name=" + federation.FederateName);
            sb.AppendLine("A.create=" + extension.Parameters["create"]);
            sb.Append("A.evolvedFomModules=");
            for (int i = 0; i < federation.RTI.FomFile.Count; i++)
            {
                if (i == federation.RTI.FomFile.Count-1)
                    sb.AppendFormat("{0}\n", federation.RTI.FomFile[i]);
                else
                    sb.AppendFormat("{0};", federation.RTI.FomFile[i]);
            }
            sb.AppendLine("A.destroy=false");
            sb.AppendLine("A.disableRequest=false");
            sb.AppendLine("A.logicalTimeFactoryClassName=");
            sb.AppendLine("A.conveyProducingFederate=true");

            // If this is a High proxy then this is the Export Policy list, otherwise the Import Policy
            //string[] foo;
            //if (component.ComponentName.ToUpper().Contains("HIGH"))
            //    foo = ListBuilder.ObjectList(export, hlaObjectTree);
            //else
            //    foo = ListBuilder.ObjectList(import, hlaObjectTree);

            //string result = String.Join("; ", foo);


            string result = (component.ComponentName.ToUpper().Contains("HIGH")) ?
                    string.Join("; ", ListBuilder.ObjectList(export, hlaObjectTree)) :
                    string.Join("; ", ListBuilder.ObjectList(import, hlaObjectTree));
            sb.AppendLine("A.objects=" + result);
            // same idea for interactions
            result = (component.ComponentName.ToUpper().Contains("HIGH")) ?
                    string.Join("; ", ListBuilder.InteractionList(export, hlaObjectTree)) :
                    string.Join("; ", ListBuilder.InteractionList(import, hlaObjectTree));
            sb.AppendLine("A.interactions=" + result);
            sb.AppendLine();

            // B Side -- Always connected to OSP
            sb.AppendLine("# B side settings (HPSD side)");
            sb.AppendLine("B.side_name=" + component.ComponentName);
            sb.AppendLine("B.description=Federate " + federation.FederateName + " HPSD interface");
            sb.AppendLine("B.profile=M");   // HPSD designator
            sb.AppendLine("B.configurationType=LocalSettingsDesignator");
            sb.AppendLine("B.host=0.0.0.0");
            sb.AppendLine("B.port=0");
            sb.AppendFormat("B.designator={0};DEBUG;CONNECT_TIMEOUT\\=10;HEARTBEAT\\=5\n",
                getDesignator(osp, component));
            sb.AppendLine("B.federation=" + component.ComponentName);
            sb.AppendLine("B.type=" + deploy.Systems[0].SystemType.ToString());
            sb.AppendLine("B.name=" + federation.FederateName);
            sb.AppendLine("B.create=" + extension.Parameters["create"]); 
            sb.Append("B.evolvedFomModules=");
            for (int i = 0; i < federation.RTI.FomFile.Count; i++)
            {
                if (i == federation.RTI.FomFile.Count - 1)
                    sb.AppendFormat("{0}\n", federation.RTI.FomFile[i]);
                else
                    sb.AppendFormat("{0};", federation.RTI.FomFile[i]);
            }
            sb.AppendLine("B.destroy=false");
            sb.AppendLine("B.disableRequest=false");
            sb.AppendLine("B.logicalTimeFactoryClassName=");
            sb.AppendLine("B.conveyProducingFederate=true");

            // If this is a High proxy then this is the Import Policy list, otherwise the Export Policy
            result = (component.ComponentName.ToUpper().Contains("HIGH")) ?
                    string.Join("; ", ListBuilder.ObjectList(import, hlaObjectTree)) :
                    string.Join("; ", ListBuilder.ObjectList(export, hlaObjectTree));
            sb.AppendLine("B.objects=" + result);
            // same idea for interactions
            result = (component.ComponentName.ToUpper().Contains("HIGH")) ?
                    string.Join("; ", ListBuilder.InteractionList(import, hlaObjectTree)) :
                    string.Join("; ", ListBuilder.InteractionList(export, hlaObjectTree));
            sb.AppendLine("B.interactions=" + result);

            return sb.ToString();
        }

        // Hackity-hack-hack
        private static string getDesignator(ModuleOsp[] modules, Component component)
        {
            StringBuilder sb = new StringBuilder();
            string send = "", receive = "";
            foreach (ModuleOsp mod in modules)
            {
                if (mod.Path.ToUpper().Contains("EXPORT"))
                {
                    if (component.ComponentName.ToUpper().Contains("HIGH"))
                    {
                        string[] s = mod.OutputPort.Split(':');
                        send = String.Format("{0}\\:{1}", s[0], s[1]);
                    }
                    else
                    {
                        string[] s = mod.InputPort.Split(':');
                        receive = String.Format("{0}\\:{1}", s[0], s[1]);
                    }
                }
                else    // IMPORT
                {
                    if (component.ComponentName.ToUpper().Contains("HIGH"))
                    {
                        string[] s = mod.InputPort.Split(':');
                        receive = String.Format("{0}\\:{1}", s[0], s[1]);
                    }
                    else
                    {
                        string[] s = mod.OutputPort.Split(':');
                        send = String.Format("{0}\\:{1}", s[0], s[1]);
                    }
                }
            }
            return String.Format("{0},{1}", send, receive);
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

        private static ModuleOsp[] findModules(List<IModule> modules)
        {
            List<ModuleOsp> mods = new List<ModuleOsp>();
            foreach (var module in modules)
            {
                if (module.GetModuleType() == Enums.ModuleType.osp)
                    mods.Add((ModuleOsp)module);
            }
            return mods.ToArray();
        }
    }
}
