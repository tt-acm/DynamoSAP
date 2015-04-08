/// Developed by Thornton Tomasetti's CORE Studio for Autodesk
/// http://core.thorntontomasetti.com
/// CORE Developers: Elcin Ertugrul and Ana Garcia Puyol

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
        public static void InitializeSapModel(ref SapObject mySAPObject, ref cSapModel mySapModel, string units)
        {

            long ret = 0;

            //TO DO: Grab open Instance if already open!!! 

            //Create SAP2000 Object
            mySAPObject = new SAP2000v16.SapObject();

            // get enum from Units
            eUnits Units = (eUnits)Enum.Parse(typeof(eUnits), units);

            //Start Application
            mySAPObject.ApplicationStart(Units, true);

            //Create SapModel object
            mySapModel = mySAPObject.SapModel;

            //initialize the model
            ret = mySapModel.InitializeNewModel(Units);

            //create new blank model
            ret = mySapModel.File.NewBlank();

            //SET UP ... SET UP ... SET UP ... SET UP
            DefineMaterials(ref mySapModel);

        }

        public static string GetModelFilename(ref cSapModel mySapModel)
        {
            return mySapModel.GetModelFilename();
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

        public static void GrabOpenSAP(ref cSapModel mySapModel, string units)
        {
            Process[] SapInstances = Process.GetProcessesByName("SAP2000");

            //http://docs.csiamerica.com/help-files/sap2000-oapi/SAP2000_API_Fuctions/General_Functions/SetAsActiveObject.htm
            if (SapInstances.LongLength >= 1)
            {
                SapObject Obj;
                object getObj = ROTHelper.GetActiveObject("SAP2000v16.SapObject");
                if (getObj == null)
                {
                    Obj = new SapObject();
                    getObj = ROTHelper.GetActiveObject("SAP2000v16.SapObject");
                }
                if (getObj != null)
                {
                    Obj = (SapObject)getObj;
                    mySapModel = Obj.SapModel;
                    // get enum from Units & Set to model
                    eUnits Units = (eUnits)Enum.Parse(typeof(eUnits), units);
                    mySapModel.SetPresentUnits(Units);
                }

                //SET UP ... SET UP ... SET UP ... SET UP
                DefineMaterials(ref mySapModel);
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
