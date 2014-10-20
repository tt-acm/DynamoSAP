using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.DesignScript.Runtime;

namespace DynamoSAP.Utilities
{
    [IsVisibleInDynamoLibrary(false)]
    public class SElement
    {
        public string GUID { get; set; }
        public string Label { get; set; }
        public SType Type;

    }

    public class SType
    { 
        // Write enum
        // Frame, Area, Link, Cable etc
    
    }
}
