/// Developed by Thornton Tomasetti's CORE Studio for Autodesk
/// http://core.thorntontomasetti.com
/// CORE Developers: Elcin Ertugrul and Ana Garcia Puyol

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
    public class Group : Definition
    {
        //FIELDS
        [SupressImportIntoVMAttribute]
        public string Name { get; set; }
        [SupressImportIntoVMAttribute]
        public List<Element> GroupElements { get; set; }

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
        /// Add an element to an existing group
        /// </summary>
        /// <param name="Group">Group to add an element to</param>
        /// <param name="Element">Element to add</param>
        /// <returns></returns>
        public static Group AddElement(Group Group, Element Element)
        {
            Group newGroup = new Group();
            newGroup.Name = Group.Name;
            List<Element> newGroupElements = Group.GroupElements;
            //check that the element doesn't already exist in the group
            if (!Group.GroupElements.Contains(Element))
            {
                newGroupElements.Add(Element);
            }
            else
            {
                throw new Exception("This element already exists in the group");
            }

            newGroup.GroupElements = newGroupElements;
            return newGroup;
        }

        /// <summary>
        /// Remove an element from a group
        /// </summary>
        /// <param name="Group">Group to remove element from</param>
        /// <param name="Element">Element to remove</param>
        /// <returns></returns>
        public static Group RemoveElement(Group Group, Element Element)
        {
            Group newGroup = new Group();
            newGroup.Name = Group.Name;
            List<Element> newGroupElements = Group.GroupElements;

            if (!Group.GroupElements.Contains(Element))
            {
                throw new Exception("This element is not in the group");
            }
            else
            {
                newGroupElements.Remove(Element);
            }
            newGroup.GroupElements = newGroupElements;
            return newGroup;
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
        internal Group(string name, List<Element> elements)
        {
            Name = name;
            GroupElements = elements;
            this.Type = Definitions.Type.Group;
        }
    }
}
