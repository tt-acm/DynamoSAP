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
        public string GUID { get; set; }
        public string Label { get; set; }

        //internal string getType(object o) // ?
        //{
        //    return o.GetType().ToString();
        //}

    }
}
