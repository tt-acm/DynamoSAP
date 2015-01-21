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
        [MultiReturn("Name", "Elements")]
        public static Dictionary<string, object> Decompose(Group Group)
        {
            return new Dictionary<string, object>
            {
                {"Name", Group.Name},
                {"Elements",Group.GroupElements},
            };

        }

        //PRIVATE CONSTRUCTOR
        private Group(string name, List<Element>elements)
        {
            Name = name;
            GroupElements = elements;
            Type = Definitions.Type.Group;
        }
    }
}
