using System;
using FPDL.Deploy;
using FPDL.Common;
using System.Linq;
using System.Runtime.CompilerServices;
using FPDL;
using System.Xml.Linq;
using System.IO;

[assembly: InternalsVisibleTo("Tests")]

namespace PitchDeploy
{
    class PitchDeploy
    {
        

        // Expect deploy file name on the command line
        static void Main(string[] args)
        {
            if ((args.Count() != 2) || (args.Count() > 2))
            {
                Console.WriteLine("Usage: PitchDeploy deployfile.xml componentName");
                return;
            }

            // Parse the policy file
            Console.WriteLine("Loading Deploy file");

            IFpdlObject fpdlObject = FpdlReader.Parse(args[0]);
            if (fpdlObject.GetType() != typeof(DeployObject))
                Console.WriteLine("Not a FPDL Deploy document");
            DeployObject deploy = (DeployObject)fpdlObject;

            string name = args[1];

            Console.WriteLine("Design Reference: {0}\nDeploy Reference: {1}",
                deploy.DesignReference, 
                deploy.ConfigMgmt.DocReference);

            if (deploy.Systems.Count > 1)
            {
                Console.WriteLine("Ambiguous Deploy document - more than one system defined");
                return;
            }

            XElement hpsdPolicy = null;
            string extenderConfig = "";
            foreach (Component component in deploy.Systems[0].Components)
            {
                // Look for Proxy component with the name we was given
                if ((component.ComponentType == Enums.ComponentType.proxy) 
                    && (name.ToUpper() == component.ComponentName.ToUpper()))

                {
                    hpsdPolicy = FilterBuilder.Create(deploy, component);
                    extenderConfig = ConfigBuilder.Create(deploy, component);
                    break;
                }
            }

            // Output the two files, using the provided name
            // Config
            File.WriteAllText(name + ".settings", extenderConfig);
            // Filter
            hpsdPolicy.AddFirst(new XAttribute("xmlns", "http://www.pitchtechnologies.com/schemas/hpsd081"),
                                new XAttribute("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance"),
                                new XAttribute("xsi:schemaLocation", "http://www.pitchtechnologies.com/schemas/hpsd081 HPSDpolicy081.xsd")
                                );
            XDocument doc = new XDocument(hpsdPolicy);
            doc.Save(name + ".xml");

            Console.WriteLine("Files generated and saved to CWD");
        }
    }
}
