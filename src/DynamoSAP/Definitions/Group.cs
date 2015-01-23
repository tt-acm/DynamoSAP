using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Autodesk.DesignScript.Geometry;
using DynamoSAP.Structure;
using DynamoSAP.Assembly;
using Autodesk.DesignScript.Runtime;

namespace DynamoSAP.Definitions
{
    public class Group:Definition
    {
        //FIELDS
        internal string Name { get; set; }
        internal List<Element> GroupElements = new List<Element>();

        //CREATE NODE
        /// <summary>
        /// Define Group 
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Elements"></param>
        /// <returns></returns>
        public static Group Define(string Name, List<Element> Elements)
        {
            return new Group(Name, Elements);
        }

        /// <summary>
        /// Decompose Group
        /// </summary>
        /// <param name="Group"></param>
        /// <returns></returns>
        [MultiReturn("Name", "Frames", "Shells", "Joints")]
        public static Dictionary<string, object> Decompose(Group Group)
        {
            List<Element> Frms = new List<Element>();
            List<Element> Shells = new List<Element>();
            List<Element> Joints = new List<Element>();

            foreach (var el in Group.GroupElements)
            {
                if (el.Type == Structure.Type.Frame)
                {
                    Frms.Add(el);
                }
                else if (el.Type == Structure.Type.Shell)
                {
                    Shells.Add(el);
                }
                else if (el.Type == Structure.Type.Joint)
                {
                    Joints.Add(el);
                }
            }

            return new Dictionary<string, object>
            {
                {"Name", Group.Name},
                {"Frames",Frms},
                {"Shells", Shells},
                {"Joints", Joints},
            };

        }

        //PRIVATE CONSTRUCTOR
        internal Group() { this.Type = Definitions.Type.Group; }
        internal Group(string name, List<Element>elements)
        {
            Name = name;
            GroupElements = elements;
            this.Type = Definitions.Type.Group;
        }
    }
}
