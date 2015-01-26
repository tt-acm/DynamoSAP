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

namespace DynamoSAP.Definitions
{
    [IsVisibleInDynamoLibrary(false)]
    public class Definition
    {
        public Type Type { get; set; }
    }

    [IsVisibleInDynamoLibrary(false)]
    public enum Type
    {
        LoadPattern,
        LoadCase,
        LoadCombo,
        Group       
    }
}
