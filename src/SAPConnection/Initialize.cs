/// Developed by Thornton Tomasetti's CORE Studio for Autodesk
/// http://core.thorntontomasetti.com
/// CORE Developers: Elcin Ertugrul and Ana Garcia Puyol

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAP2000v20;
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
		public static void InitializeSapModel(ref cSapModel mySAPObject, ref cSapModel mySapModel,
			string units)
		{
			//dimension the SapObject as cOAPI type
			cOAPI mySapObject = null;

			//Use ret to check if functions return successfully (ret = 0) or fail (ret = nonzero)
			var ret = 0;

			//create API helper object
			cHelper myHelper;
			try
			{
				myHelper = new Helper();
			}
			catch (Exception ex)
			{
				//Console.WriteLine("Cannot create an instance of the Helper object");
				return;
			}
			try
			{
				//create SapObject
				mySapObject = myHelper.CreateObjectProgID("CSI.SAP2000.API.SapObject");
			}
			catch (Exception ex)
			{
				//Console.WriteLine("Cannot start a new instance of the program.");
				return;
			}

			//start SAP2000 application
			ret = mySapObject.ApplicationStart();

			//create SapModel object
			mySapModel = mySapObject.SapModel;

			//initialize model
			ret = mySapModel.InitializeNewModel(eUnits.kN_m_C);

			//create new blank model
			ret = mySapModel.File.NewBlank();

			//define material property

			//ret = mySapModel.PropMaterial.SetMaterial("CONC", eMatType.Concrete, -1, "", "");
		}

		public static string GetModelFilename(ref cSapModel mySapModel)
		{
			return mySapModel.GetModelFilename();
		}

		public static void OpenSAPModel(string filePath, ref cSapModel mySapModel, ref string units)
		{
			int ret = 0;

			//Create SAP2000 Object
			cOAPI mySapObject = null;
			cHelper myHelper = new Helper();
			mySapObject = myHelper.CreateObjectProgID("CSI.SAP2000.API.SapObject");

			//Start Application
			mySapObject.ApplicationStart();

			//Create SapModel object
			mySapModel = mySapObject.SapModel;
			ret = mySapModel.InitializeNewModel();
			ret = mySapModel.File.OpenFile(filePath);
			units = mySapModel.GetPresentUnits().ToString();
		}

		public static void GrabOpenSAP(ref cSapModel mySapModel, ref string ModelUnits, string DynInputUnits = "kip_ft_F")
		{
			//dimension the SapObject as cOAPI type
			cOAPI mySapObject = null;

			//Use ret to check if functions return successfully (ret = 0) or fail (ret = nonzero)
			var ret = 0;

			//attach to a running instance of SAP2000
			try
			{
				//get the active SapObject

				//try the code form the example
				//mySapObject =(cOAPI) Marshal.GetActiveObject("CSI.SAP2000.API.SapObject");
				//get the active SapObject
				mySapObject = (cOAPI) System.Runtime.InteropServices.Marshal.GetActiveObject("CSI.SAP2000.API.SapObject");
			}
			catch (Exception ex)
			{
				Console.WriteLine("No running instance of the program found or failed to attach.");
				return;
			}

			//Create SapModel object
			mySapModel = mySapObject.SapModel;
			// get enum from Units & Set to model
			if (!String.IsNullOrEmpty(DynInputUnits))
			{
				eUnits Units = (eUnits) Enum.Parse(typeof(eUnits), DynInputUnits);
				try
				{
					ret = mySapModel.SetPresentUnits(Units);
				}
				catch (Exception ex)
				{
					string message = ex.Message;
				}
			}
			ModelUnits = mySapModel.GetPresentUnits().ToString();
		}

		//public static void Release(ref mySapObject SAP, ref cSapModel Model)
		//      {
		//          GC.Collect();
		//          GC.WaitForPendingFinalizers();

		//          if (SAP != null)
		//          {
		//              Marshal.FinalReleaseComObject(SAP);
		//          }

		//          if (Model != null)
		//          {
		//              Marshal.FinalReleaseComObject(Model);
		//          }

		//      }

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
			if (MatNames != null)
			{
				if (!MatNames.Contains("A36"))
				{
					ret = SapModel.PropMaterial.AddQuick(ref MatName, eMatType.Steel,
						eMatTypeSteel.ASTM_A36);
				}
				if (!MatNames.Contains("A53GrB"))
				{
					ret = SapModel.PropMaterial.AddQuick(ref MatName, eMatType.Steel,
						eMatTypeSteel.ASTM_A53GrB);
				}
				if (!MatNames.Contains("A500GrB42"))
				{
					ret = SapModel.PropMaterial.AddQuick(ref MatName, eMatType.Steel,
						eMatTypeSteel.ASTM_A500GrB_Fy42);
				}
				if (!MatNames.Contains("A500GrB46"))
				{
					ret = SapModel.PropMaterial.AddQuick(ref MatName, eMatType.Steel,
						eMatTypeSteel.ASTM_A500GrB_Fy46);
				}
				if (!MatNames.Contains("A572Gr50"))
				{
					ret = SapModel.PropMaterial.AddQuick(ref MatName, eMatType.Steel,
						eMatTypeSteel.ASTM_A572Gr50);
				}
				if (!MatNames.Contains("A913Gr50"))
				{
					ret = SapModel.PropMaterial.AddQuick(ref MatName, eMatType.Steel,
						eMatTypeSteel.ASTM_A913Gr50);
				}
				if (!MatNames.Contains("A992Fy50"))
				{
					ret = SapModel.PropMaterial.AddQuick(ref MatName, eMatType.Steel,
						eMatTypeSteel.ASTM_A992_Fy50);
				}
				if (!MatNames.Contains("4000Psi"))
				{
					ret = SapModel.PropMaterial.AddQuick(ref MatName, eMatType.Concrete,
						eMatTypeSteel.ASTM_A992_Fy50,
						eMatTypeConcrete.FC4000_NormalWeight);
				}
			}
			else
			{
				ret = SapModel.PropMaterial.AddQuick(ref MatName, eMatType.Steel,
					eMatTypeSteel.ASTM_A36);
				ret = SapModel.PropMaterial.AddQuick(ref MatName, eMatType.Steel,
					eMatTypeSteel.ASTM_A53GrB);
				ret = SapModel.PropMaterial.AddQuick(ref MatName, eMatType.Steel,
					eMatTypeSteel.ASTM_A500GrB_Fy42);
				ret = SapModel.PropMaterial.AddQuick(ref MatName, eMatType.Steel,
					eMatTypeSteel.ASTM_A500GrB_Fy46);
				ret = SapModel.PropMaterial.AddQuick(ref MatName, eMatType.Steel,
					eMatTypeSteel.ASTM_A572Gr50);
				ret = SapModel.PropMaterial.AddQuick(ref MatName, eMatType.Steel,
					eMatTypeSteel.ASTM_A913Gr50);
				ret = SapModel.PropMaterial.AddQuick(ref MatName, eMatType.Steel,
					eMatTypeSteel.ASTM_A992_Fy50);
				ret = SapModel.PropMaterial.AddQuick(ref MatName, eMatType.Concrete,
					eMatTypeSteel.ASTM_A992_Fy50,
					eMatTypeConcrete.FC4000_LightWeight);
			}
			return true;
		}
	}
}