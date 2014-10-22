using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SAP2000v16;
using DynamoSAP.Structure;

//DYNAMO
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;

namespace SAPApplication
{
    [SupressImportIntoVM] 
    public class Application
    {
        public static void LaunchNewSapModel (ref cSapModel mySapModel)
        {
            long ret = 0;

            SAP2000v16.SapObject mySAPObject;
            //SAP2000v16.cSapModel mySapModel;

            //Create SAP2000 Object
            mySAPObject = new SAP2000v16.SapObject();

            //Start Application
            mySAPObject.ApplicationStart(SAP2000v16.eUnits.kip_in_F, true); //TODO: Pass E_unit as constructor

            //Create SapModel object
            mySapModel = mySAPObject.SapModel;
            //mySapModel = new cSapModel();

            //initialize the model
            ret = mySapModel.InitializeNewModel(eUnits.kip_in_F); // TODO: Pass Eunit as Constructor

            //create new blank model
            ret = mySapModel.File.NewBlank();

        }

        // TODO: Release Methods

    }
}
