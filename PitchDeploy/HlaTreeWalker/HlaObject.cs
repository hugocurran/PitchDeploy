﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PitchDeploy.HlaTreeWalker
{
    /// <summary>
    /// Class that defines a linked list of object nodes ( HLAobjectRoot -> BaseEntity -> PhysicalEntity -> Aircraft) along with the attribs/params for each node
    /// </summary>
    public class HlaObjectNode
    {
        public string Name { get; set; }
        public List<HlaObjectNode> Objects { get; set; }
        public List<string> Attributes { get; set; }
        public HlaObjectNode Parent { get; set; }

        public HlaObjectNode(HlaObjectNode parent, string line )
        {
            Objects = new List<HlaObjectNode>();
            Attributes = new List<string>();

            // chop line; first element is our name
            string[] s = line.Split('.');

            // if there are < 2 elements that is an error
            if (s.Count() < 2)
                throw new ApplicationException("Line from " + parent.Name + " only has an attribute");

             Parent = parent;
             // our name is the first element
             Name = s[0];

            // If there are only 2 elements, the second may be an attribute
            if (s.Count() == 2)
            {
                if (s[1] != "")     // The Split() above will give use an empty string eg HLAobjectRoot. is [HLAobjectRoot] [] or a populated string
                {
                    Attributes.Add(s[1]);
                    return;
                }
                return; // Not an attribute
            }

            // There are > 2 elements; s[1] is the next object name
            HlaObjectNode nextObject;
            if ((nextObject = Objects.Find(x => x.Name == s[1])) != null)   // Check if we have seen the object before
                nextObject.Add(line.Substring(Name.Length + 1));
            else
                Objects.Add(new HlaObjectNode(this, line.Substring(Name.Length + 1)));
        }

        public void Add(string line)
        {
            // chop line; first element is our name
            string[] s = line.Split('.');
            if (s[0] != Name)
            {
                if (Parent != null)
                    throw new ApplicationException("Line from " + Parent.Name + " is not for me");
                else
                throw new ApplicationException("Line from caller is not for me");
            }

            // if there are < 2 elements that is an error
            if (s.Count() < 2)
                throw new ApplicationException("Line from " + Parent.Name + " only has an attribute");

            // If there are only 2 elements, the second is an attribute
            if (s.Count() == 2)
            {
                Attributes.Add(s[1]);
                return;
            }

            // There are > 2 elements; s[1] is the next object name
            HlaObjectNode nextObject;
            if ((nextObject = Objects.Find(x => x.Name == s[1])) != null)
                nextObject.Add(line.Substring(Name.Length + 1));
            else
                Objects.Add(new HlaObjectNode(this, line.Substring(Name.Length + 1)));
        }
    }
}
