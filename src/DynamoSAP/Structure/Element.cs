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
        private string guid = System.Guid.NewGuid().ToString();

        public string GUID { get { return guid; } }

        public string Label { get; set; }

        //internal string getType(object o) // ?
        //{
        //    return o.GetType().ToString();
        //}

    }
}
