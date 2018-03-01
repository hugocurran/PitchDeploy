using System;
using System.Collections.Generic;
using System.Text;

namespace PitchDeploy.HlaTreeWalker
{
    public static class TreeReader
    {
        /// <summary>
        /// Create an HlaObject tree
        /// </summary>
        /// <param name="dataFilename">File containing the object model description</param>
        /// <returns></returns>
        public static HlaObjectNode CreateTree(string dataFilename)
        {
            string line;
            // Read the file line by line.  
            System.IO.StreamReader file =
                new System.IO.StreamReader(dataFilename);

            HlaObjectNode HlaObjectRoot = new HlaObjectNode(null, "HLAobjectRoot.");
            //HlaObject HlaInteractionRoot = new HlaObject(null, "HLAinteractionRoot.");

            while ((line = file.ReadLine()) != null)
            {
                string[] s = line.Split('.');
                switch (s[0])
                {
                    case "HLAobjectRoot":
                        HlaObjectRoot.Add(line);
                        break;
                    case "HLAinteractionRoot":
                        //HlaInteractionRoot.Add(line);
                        break;
                    default:
                        throw new ApplicationException("Corrupted datafile: Unknown root classname: " + s[0]);
                }
            }
            file.Close();
            return HlaObjectRoot;
        }

        /// <summary>
        /// For a given fully qualified ObjectClassName find all of the attributes
        /// </summary>
        /// <param name="hlaTreeRoot">Root object (eg HLAobjectRoot) of the class namespace</param>
        /// <param name="objectClassName">Fully qualified objectClassName</param>
        /// <returns></returns>
        public static List<string> FindAttributes(HlaObjectNode hlaTreeRoot, string objectClassName)
        {
            List<string> attribs = new List<string>();

            // Walk down the tree from the root and find all attributes
            string[] s = objectClassName.Split('.');
            // s[0] should be the root object name
            if (s[0] != hlaTreeRoot.Name)
                throw new ApplicationException("ObjectClassName is not within the HlaTree");

            HlaObjectNode currentNode = hlaTreeRoot;
            for(int i = 1; i < s.Length; i++)   // start at 1 to skip root
            {
                // Walk the tree - we should never see a null unless there is an error
                if ((currentNode = currentNode.Objects.Find(x => x.Name == s[i])) == null)
                    throw new ApplicationException("Tree walk error - node not in tree: " + s[i]);
            }

            // currentNode should now point to the HlaObject that has the object in it
            while (currentNode != hlaTreeRoot)
            {
                // get the attributes
                foreach (string attrib in currentNode.Attributes)
                    attribs.Add(attrib);
                // walk backwards
                currentNode = currentNode.Parent;
            }
            return attribs;
        }
    }
}
