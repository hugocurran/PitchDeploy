using System;
using FPDL.Deploy;
using FPDL.Common;
using System.Linq;
using System.Runtime.CompilerServices;
using FPDL;

[assembly: InternalsVisibleTo("Tests")]

namespace PitchDeploy
{
    class Program
    {
        

        // Expect deploy file name on the command line
        static void Main(string[] args)
        {
            if ((args.Count() != 1) || (args.Count() > 2))
            {
                Console.WriteLine("Usage: Proxy deployfile.xml");
                return;
            }

            // Parse the policy file
            Console.WriteLine("Loading Deploy file");

            IFpdlObject fpdlObject = FpdlReader.Parse(args[0]);
            if (fpdlObject.GetType() != typeof(DeployObject))
                Console.WriteLine("Not a FPDL Deploy document");
            DeployObject deploy = (DeployObject)fpdlObject;

            Console.WriteLine("Design Reference: {0}\nDeploy Reference: {1}",
                deploy.DesignReference, 
                deploy.ConfigMgmt.DocReference);

            if (deploy.Systems.Count > 1)
            {
                Console.WriteLine("Ambiguous Deploy document - more than one system defined");
                return;
            }

            foreach (Component component in deploy.Systems[0].Components)
            {
                if (component.ComponentType == Enums.ComponentType.proxy)
                {

                }
            }

        }
    }
}
