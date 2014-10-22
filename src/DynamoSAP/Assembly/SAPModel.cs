using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.InteropServices;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.IO;

using SAPApplication;

using DynamoSAP.Structure;

//DYNAMO
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;

//SAP 
using SAP2000v16;

namespace DynamoSAP.Assembly
{
    public class SAPModel
    {
       //PROPERTIES


       //PRIVATE METHODS
        private static void CreateFrame(Frame f, ref cSapModel mySapModel)
        {
            string dummy = string.Empty;
            long ret = 0;
            //Create Frame
            ret = mySapModel.FrameObj.AddByCoord(f.BaseCrv.StartPoint.X,
                f.BaseCrv.StartPoint.Y,
                f.BaseCrv.StartPoint.Z,
                f.BaseCrv.EndPoint.X,
                f.BaseCrv.EndPoint.Y,
                f.BaseCrv.EndPoint.Z,
                ref dummy);

            f.Label = dummy; // 

        }

         //DYNAMO NODES
        public static string CreateSAPModel(List<Element> SAPElements)
        {
            //1. Instantiate SAPModel
            SAP2000v16.SapObject mySapObject = null;
            SAP2000v16.cSapModel mySapModel = null;

            try 
            {	        
	            SAPApplication.Application.InitializeSapModel(ref mySapObject, ref mySapModel);
            }
            catch (Exception)
            {
                SAPApplication.Application.Release(ref mySapObject, ref mySapModel);
            };

            //2. Create Geometry
            foreach (var el in SAPElements)
            {
                if (el.GetType().ToString().Contains("Frame"))
                {
                    CreateFrame(el as Frame , ref mySapModel);
                }
            }

            // 3. Assigns Constraints to Nodes

            // 4. Define LoadPattern

            // 5. Loads 

            //if can't set to null, will be a hanging process
            mySapModel = null;
            mySapObject = null; 

            return "Success";
        }


    }
}
