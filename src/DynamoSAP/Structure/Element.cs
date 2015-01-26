/// Developed by Thornton Tomasetti's CORE Studio for Autodesk
/// http://core.thorntontomasetti.com
/// CORE Developers: Elcin Ertugrul and Ana Garcia Puyol

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


        internal const string TRACE_ID = "{0459D869-0C72-447F-96D8-08A7FB92214B}-REVIT"; // this constant can't change
    }

    [IsVisibleInDynamoLibrary(false)]
    public enum  Type
    {
        Frame,
        Cable,
        Shell,
        Joint
    }
}
