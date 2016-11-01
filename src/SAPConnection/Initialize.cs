/// Developed by Thornton Tomasetti's CORE Studio for Autodesk
/// http://core.thorntontomasetti.com
/// CORE Developers: Elcin Ertugrul and Ana Garcia Puyol

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SAP2000v18;
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
        public static void InitializeSapModel(ref cSapModel mySapModel, string units)
        {
            // Create SAP Object
            SAP2000v18.cHelper helper = new SAP2000v18.Helper();
            SAP2000v18.cOAPI applicationObject;
            applicationObject = helper.CreateObject("CSI.SAP2000.API.SapObject");

            // get enum from Units
            eUnits Units = (eUnits)Enum.Parse(typeof(eUnits), units);

            //Start application
            applicationObject.ApplicationStart(Units, true);

            //Get a reference to cSapModel to access all OAPI classes and functions 
            mySapModel = applicationObject.SapModel;

            //Initialize model
            mySapModel.InitializeNewModel(Units);

            // Create new Model
            mySapModel.File.NewBlank();

            //SET UP ... SET UP ... SET UP ... SET UP
            DefineMaterials(ref mySapModel);

        }

        public static string GetModelFilename(ref cSapModel mySapModel)
        {
            return mySapModel.GetModelFilename();
        }

        public static void OpenSAPModel(string filePath, ref cSapModel mySapModel, ref string units)
        {
            ////Dynamically load SAP2000.exe assembly from the program installation folder 
            //string pathToSAPEXE = System.IO.Path.Combine(System.Environment.GetEnvironmentVariable("ProgramFiles(x86)"), "Computers and Structures", "SAP2000 18", "SAP2000.exe");
            //System.Reflection.Assembly SAPAssembly = System.Reflection.Assembly.LoadFrom(pathToSAPEXE);

            ////Create an instance of SAPObject and get a reference to cOAPI interface 
            //cOAPI applicationObject = (cOAPI)SAPAssembly.CreateInstance("CSI.SAP2000.API.SapObject");

            // Create SAP Object
            SAP2000v18.cHelper helper = new SAP2000v18.Helper();
            SAP2000v18.cOAPI applicationObject;
            applicationObject = helper.CreateObject("CSI.SAP2000.API.SapObject");

            //Start application
            applicationObject.ApplicationStart();

            //Get a reference to cSapModel to access all OAPI classes and functions 
            mySapModel = applicationObject.SapModel;

            //Initialize model
            mySapModel.InitializeNewModel();

            // Open existing Etabs File 
            mySapModel.File.OpenFile(filePath);

            // Get Units
            units = mySapModel.GetPresentUnits().ToString();
        }

        public static void GrabOpenSAP(ref cSapModel ActiveModel, ref string ModelUnits, string DynInputUnits = "kip_ft_F")
        {
            //Process[] SapInstances = Process.GetProcessesByName("SAP2000");

            ////http://docs.csiamerica.com/help-files/sap2000-oapi/SAP2000_API_Fuctions/General_Functions/SetAsActiveObject.htm
            //if (SapInstances.LongLength >= 1)
            //{
            //    SapObject Obj;
            //    object getObj = ROTHelper.GetActiveObject("SAP2000v18.SapObject");
            //    if (getObj == null)
            //    {
            //        Obj = new SapObject();
            //        getObj = ROTHelper.GetActiveObject("SAP2000v18.SapObject");
            //    }
            //    if (getObj != null)
            //    {
            //        Obj = (SapObject)getObj;
            //        mySapModel = Obj.SapModel;
            //        // get enum from Units & Set to model
            //        if (! String.IsNullOrEmpty(DynInputUnits))
            //        {
            //            eUnits Units = (eUnits)Enum.Parse(typeof(eUnits), DynInputUnits);
            //            try
            //            {
            //                int ret = mySapModel.SetPresentUnits(Units);
            //            }
            //            catch(Exception ex) {
            //                string message = ex.Message;
            //            }

            //        }
            //        ModelUnits = mySapModel.GetPresentUnits().ToString();
            //    }


            //OAPI SAP 2000 v18 Attaching to a Manually Started Instance of SAP2000
 
            SAP2000v18.cHelper helper = new SAP2000v18.Helper();
            SAP2000v18.cOAPI applicationObject;
            applicationObject = helper.GetObject("CSI.SAP2000.API.SapObject");

            if (applicationObject == null)
            {
               // return false;
            }
            if (applicationObject != null)
            {
                ActiveModel = applicationObject.SapModel;
                string path = ActiveModel.GetModelFilename(true);

                // get enum from Units & Set to model
                if (!String.IsNullOrEmpty(DynInputUnits))
                {
                   eUnits Units = (eUnits)Enum.Parse(typeof(eUnits), DynInputUnits);
                   int ret = ActiveModel.SetPresentUnits(Units);
                }

                ModelUnits = ActiveModel.GetPresentUnits().ToString();

                //return true;
            }

            //SET UP ... SET UP ... SET UP ... SET UP
            try
                {
                    DefineMaterials(ref ActiveModel);
                }
                catch { }
        }

    public static void Release(ref cSapModel Model)
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();

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

            if (MatNames!=null)
            {
                if (!MatNames.Contains("A36"))
                {
                    ret = SapModel.PropMaterial.AddQuick(ref MatName, eMatType.Steel, eMatTypeSteel.ASTM_A36);
                }
                if (!MatNames.Contains("A53GrB"))
                {
                    ret = SapModel.PropMaterial.AddQuick(ref MatName, eMatType.Steel, eMatTypeSteel.ASTM_A53GrB);
                }
                if (!MatNames.Contains("A500GrB42"))
                {
                    ret = SapModel.PropMaterial.AddQuick(ref MatName, eMatType.Steel, eMatTypeSteel.ASTM_A500GrB_Fy42);
                }
                if (!MatNames.Contains("A500GrB46"))
                {
                    ret = SapModel.PropMaterial.AddQuick(ref MatName, eMatType.Steel, eMatTypeSteel.ASTM_A500GrB_Fy46);
                }
                if (!MatNames.Contains("A572Gr50"))
                {
                    ret = SapModel.PropMaterial.AddQuick(ref MatName, eMatType.Steel, eMatTypeSteel.ASTM_A572Gr50);
                }
                if (!MatNames.Contains("A913Gr50"))
                {
                    ret = SapModel.PropMaterial.AddQuick(ref MatName, eMatType.Steel, eMatTypeSteel.ASTM_A913Gr50);
                }
                if (!MatNames.Contains("A992Fy50"))
                {
                    ret = SapModel.PropMaterial.AddQuick(ref MatName, eMatType.Steel, eMatTypeSteel.ASTM_A992_Fy50);
                }
                if (!MatNames.Contains("4000Psi"))
                {
                    ret = SapModel.PropMaterial.AddQuick(ref MatName, eMatType.Concrete, eMatTypeSteel.ASTM_A913Gr50, eMatTypeConcrete.FC4000_NormalWeight);
                } 
            }
            else
            {
                ret = SapModel.PropMaterial.AddQuick(ref MatName, eMatType.Steel, eMatTypeSteel.ASTM_A36);
                ret = SapModel.PropMaterial.AddQuick(ref MatName, eMatType.Steel, eMatTypeSteel.ASTM_A53GrB);
                ret = SapModel.PropMaterial.AddQuick(ref MatName, eMatType.Steel, eMatTypeSteel.ASTM_A500GrB_Fy42);
                ret = SapModel.PropMaterial.AddQuick(ref MatName, eMatType.Steel, eMatTypeSteel.ASTM_A500GrB_Fy46);
                ret = SapModel.PropMaterial.AddQuick(ref MatName, eMatType.Steel, eMatTypeSteel.ASTM_A572Gr50);
                ret = SapModel.PropMaterial.AddQuick(ref MatName, eMatType.Steel, eMatTypeSteel.ASTM_A913Gr50);
                ret = SapModel.PropMaterial.AddQuick(ref MatName, eMatType.Concrete, eMatTypeSteel.ASTM_A913Gr50, eMatTypeConcrete.FC4000_NormalWeight);
            }

            return true;
        }


    }
}
