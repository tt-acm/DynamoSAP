using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//DYNAMO
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;

namespace DynamoSAP.Structure
{
    [IsVisibleInDynamoLibrary(false)]
    public class Element
    {
        /// <summary>
        /// Label 
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Type
        /// </summary>
        public Type Type { get; set; }

    }

    public enum  Type
    {
        Frame,
        Cable,
        Shell
    }
}
