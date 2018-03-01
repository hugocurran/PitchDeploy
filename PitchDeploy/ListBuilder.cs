using FPDL.Common;
using FPDL.Deploy;
using PitchDeploy.HlaTreeWalker;
using System;
using System.Collections.Generic;
using System.Text;

namespace PitchDeploy
{
    internal static class ListBuilder
    {
        internal static string[] objectList(IModule policy, HlaObjectNode hlaObjectTree)
        {
            // objectList is a list of objectClassNames
            //List<string> objectList = new List<string>();
            // objectListPartial has key=objectClassName, value=comma separated list of attributes
            Dictionary<string, List<string>> objectList = new Dictionary<string, List<string>>();

            // Logic: We need to get the maximum list of attributes associated with each object:
            //      1. Find all objects in the policy with no attributes defined
            //      2. Add these objects and the full lst of attributes for the object
            //      3. Find the objects in the policy with some attributes defined
            //      4. Add these objects (and their attributes) if they are not there already

            if (policy.GetType() == typeof(ModuleExport))
            {
                ModuleExport exportPolicy = (ModuleExport)policy;
                foreach (Source source in exportPolicy.Sources)
                {
                    // Find the objects with no attributes defined and add the object and all attributes
                    foreach (HlaObject obj in source.Objects)
                    {
                        if (obj.Attributes.Count == 0)
                            addObject(objectList, obj, hlaObjectTree);
                    }
                    // Find the objects with attributes defined and add the object and attributes if needed
                    foreach (HlaObject obj in source.Objects)
                    { 
                        if (obj.Attributes.Count > 0)
                            addObjectWithAttributes(objectList, obj);
                    }
                }
            }
            else
            {
                ModuleImport importPolicy = (ModuleImport)policy;
                // Find the objects with no attributes defined and add the object and all attributes
                foreach (HlaObject obj in importPolicy.Objects)
                {
                    if (obj.Attributes.Count == 0)
                        addObject(objectList, obj, hlaObjectTree);
                }
                // Find the objects with attributes defined and add the object and attributes if needed
                foreach (HlaObject obj in importPolicy.Objects)
                {
                    if (obj.Attributes.Count > 0)
                        addObjectWithAttributes(objectList, obj);
                }
            }
            // Now we can build the string[]
            List<string> result = new List<string>();
            foreach(KeyValuePair<string, List<string>> obj in objectList)
            {

            }

            return objectList.ToArray();
        }

        // Add object with partial attribute list
        private static void addObjectWithAttributes(Dictionary<string, List<string>> objectList, HlaObject obj)
        {
            if (objectList.ContainsKey(obj.ObjectClassName))    // obect in list, update attribs as needed
            {
                // check the attribute list
                List<string> attribs = objectList[obj.ObjectClassName];
                foreach(HlaAttribute attrib in obj.Attributes)
                {
                    if (!attribs.Contains(attrib.AttributeName))
                        attribs.Add(attrib.AttributeName);
                }
            }
            else   // object not in list, add with attributes
            {
                // Build the attributes list
                List<string> attribs = new List<string>();
                foreach (HlaAttribute attrib in obj.Attributes)
                    attribs.Add(attrib.AttributeName);
                // Add the object
                objectList.Add(obj.ObjectClassName, attribs);
            }

        }
        // Add object with no attribute list defined
        private static void addObject(Dictionary<string, List<string>> objectList, HlaObject obj, HlaObjectNode hlaObjectTree)
        {
            // If the object is not in the list, add it
            if (!objectList.ContainsKey(obj.ObjectClassName))
            { 
                // Get the complete attribute list
                objectList.Add(obj.ObjectClassName, TreeReader.FindAttributes(hlaObjectTree, obj.ObjectClassName));
            }
        }
    }
}
