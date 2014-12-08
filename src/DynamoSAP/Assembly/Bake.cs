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

        /// <summary>
        /// Export the Dynamo Structural Project to SAP2000
        /// </summary>
        /// <param name="StructuralModel">Structural Model to bake</param>
        /// <param name="Bake">Set Boolean to True to bake the model</param>
        /// <returns>Structural Model</returns>
        public static StructuralModel ToSAP(StructuralModel StructuralModel, bool Bake)
        {
            if (StructuralModel != null)
            {
                if (Bake)
                {
                    SAPModel.CreateSAPModel(ref StructuralModel);
                }
            }

            return StructuralModel;

        }

        private Bake() { }
    }
}
