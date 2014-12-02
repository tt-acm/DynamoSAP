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
using System.Diagnostics;
using System.Collections;

namespace SAPConnection
{
    [SupressImportIntoVM]
    public class Initialize
    {
        public static void InitializeSapModel(ref SapObject mySAPObject, ref cSapModel mySapModel)
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

            //SET UP ... SET UP ... SET UP ... SET UP
            DefineMaterials(ref mySapModel);

        }

        public static void OpenSAPModel(string filePath, ref cSapModel mySapModel, ref string units)
        {
            long ret = 0;
            //Create SAP2000 Object
            SapObject mySAPObject = new SAP2000v16.SapObject();
            //Start Application
            mySAPObject.ApplicationStart();
            //Create SapModel object
            mySapModel = mySAPObject.SapModel;
            ret = mySapModel.InitializeNewModel();
            ret = mySapModel.File.OpenFile(filePath);
            units = mySapModel.GetPresentUnits().ToString();
        }

        public static void GrabOpenSAP(ref cSapModel mySapModel, ref string units)
        {
            Process[] SapInstances = Process.GetProcessesByName("SAP2000");

            if (SapInstances.LongLength >= 1)
            {
                // keep contunie
                //mySapModel = SapInstances[0].CreateObjRef();
                object getObj = ROTHelper.GetActiveObject("SAP2000v16.SapObject");
                SapObject Obj = (SapObject)getObj;
                mySapModel = Obj.SapModel;
                units = mySapModel.GetPresentUnits().ToString();
            }
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

        // METHODS FOR SAP SET UP
        // Add Most Common Materials to SAP
        public static bool DefineMaterials(ref cSapModel SapModel)
        {
            // Add Most Common Standard Materials to SAP Model // call this before or during Create Structure 

            long ret = 0;
            string MatName = string.Empty;

            int number = 0;
            string[] MatNames = null;
            ret = SapModel.PropMaterial.GetNameList(ref number, ref MatNames);

            if (!MatNames.Contains("A36"))
            {
                ret = SapModel.PropMaterial.AddQuick(ref MatName, eMatType.MATERIAL_STEEL, eMatTypeSteel.MATERIAL_STEEL_SUBTYPE_ASTM_A36);
            }
            if (!MatNames.Contains("A53GrB"))
            {
                ret = SapModel.PropMaterial.AddQuick(ref MatName, eMatType.MATERIAL_STEEL, eMatTypeSteel.MATERIAL_STEEL_SUBTYPE_ASTM_A53GrB);
            }
            if (!MatNames.Contains("A500GrB42"))
            {
                ret = SapModel.PropMaterial.AddQuick(ref MatName, eMatType.MATERIAL_STEEL, eMatTypeSteel.MATERIAL_STEEL_SUBTYPE_ASTM_A500GrB_Fy42);
            }
            if (!MatNames.Contains("A500GrB46"))
            {
                ret = SapModel.PropMaterial.AddQuick(ref MatName, eMatType.MATERIAL_STEEL, eMatTypeSteel.MATERIAL_STEEL_SUBTYPE_ASTM_A500GrB_Fy46);
            }
            if (!MatNames.Contains("A572Gr50"))
            {
                ret = SapModel.PropMaterial.AddQuick(ref MatName, eMatType.MATERIAL_STEEL, eMatTypeSteel.MATERIAL_STEEL_SUBTYPE_ASTM_A572Gr50);
            }
            if (!MatNames.Contains("A913Gr50"))
            {
                ret = SapModel.PropMaterial.AddQuick(ref MatName, eMatType.MATERIAL_STEEL, eMatTypeSteel.MATERIAL_STEEL_SUBTYPE_ASTM_A913Gr50);
            }
            if (!MatNames.Contains("A992Fy50"))
            {
                ret = SapModel.PropMaterial.AddQuick(ref MatName, eMatType.MATERIAL_STEEL, eMatTypeSteel.MATERIAL_STEEL_SUBTYPE_ASTM_A992_Fy50);

            }
            if (!MatNames.Contains("4000Psi"))
            {
                ret = SapModel.PropMaterial.AddQuick(ref MatName, eMatType.MATERIAL_CONCRETE, eMatTypeSteel.MATERIAL_STEEL_SUBTYPE_ASTM_A992_Fy50, eMatTypeConcrete.MATERIAL_CONCRETE_SUBTYPE_FC4000_NORMALWEIGHT);
            }

            return true;
        }


    }
}
