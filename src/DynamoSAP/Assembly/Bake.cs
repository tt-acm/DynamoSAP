using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.InteropServices;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.IO;

using SAPConnection;
using DynamoSAP.Structure;

//DYNAMO
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;

//SAP 
using SAP2000v16;

namespace DynamoSAP.Assembly
{
    public class Bake
    {
        
        //// DYNAMO NODES ////
        public static StructuralModel ToSAP (StructuralModel Model, bool Bake)
        {
            if (Bake)
            {
                SAPModel.CreateSAPModel(ref Model);                
            }

            return Model;
           
        }

        private Bake() {}
    }
}
