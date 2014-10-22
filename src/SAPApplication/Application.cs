using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SAP2000v16;
// interop.COM services for SAP
using System.Runtime.InteropServices;

//DYNAMO
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;

namespace SAPApplication
{
    [SupressImportIntoVM] 
    public class Application
    {
        public static void InitializeSapModel (ref SapObject mySAPObject, ref cSapModel mySapModel)
        {

            long ret = 0;

            //TO DO: Grab open Instance if already open!!!

            //Create SAP2000 Object
            mySAPObject = new SAP2000v16.SapObject();

            //Start Application
            mySAPObject.ApplicationStart(SAP2000v16.eUnits.kip_in_F, true); //TODO: Pass E_unit as constructor

            //Create SapModel object
            mySapModel = mySAPObject.SapModel;

            //initialize the model
            ret = mySapModel.InitializeNewModel(eUnits.kip_in_F); // TODO: Pass Eunit as Constructor

            //create new blank model
            ret = mySapModel.File.NewBlank();

        }

        public static void Release(ref SapObject SAP, ref cSapModel Model)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();

            if (SAP != null)
            {
                Marshal.FinalReleaseComObject(SAP);
            }

            if (Model != null)
            {
                Marshal.FinalReleaseComObject(Model);
            }

        }

    }
}
