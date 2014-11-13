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
using DynamoSAP.Analysis;

//DYNAMO
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;

//SAP 
using SAP2000v16;

namespace DynamoSAP.Assembly
{
    public class Bake
    {
        //private static cSapModel mySapModel;

        //// DYNAMO NODES ////
        public static string BakeToSAP (Model model, bool Run)
        {
            SAPModel.CreateSAPModel(model.Frames, model.LoadPatterns, model.LoadCases, model.Restraints, model.Loads, model.Releases);
            return "heyoo";
        }
    }
}
