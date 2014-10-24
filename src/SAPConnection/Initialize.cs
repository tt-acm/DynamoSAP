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

        public static string MaterialMapper_DyanmoToSap(string DMat)
        {

            if (DMat == null || DMat.ToLower().Contains("steel") || DMat.ToLower().Contains("st"))
            {
                return "A992Fy50";  // default steel;
            }
            else if (DMat == null || DMat.ToLower().Contains("concrete") || DMat.ToLower().Contains("conc"))
            {
                return "4000Psi";
            }

            if (DMat.Contains("A36"))
            {

                return "A36";
            }
            else if (DMat.Contains("A53"))
            {

                return "A53GrB";
            }
            else if (DMat.Contains("A500"))
            {
                if (DMat.Contains("42"))
                {

                    return "A500GrB42";
                }
                else if (DMat.Contains("46"))
                {

                    return "A500GrB46";
                }
            }
            else if (DMat.Contains("A572"))
            {

                return "A572Gr50";
            }
            else if (DMat.Contains("A913"))
            {

                return "A913Gr50";
            }
            else if (DMat.Contains("A992"))
            {

                return "A992Fy50";
            }

            return "A992Fy50"; // Default
        }
        //Check if Section exists
        public static bool IsSectionExists(string DSection, ref cSapModel mySapModel)
        {
            int number = 0;
            string[] SectionNames = null;
            long ret = mySapModel.PropFrame.GetNameList(ref number, ref SectionNames);

            if (SectionNames.Contains(DSection))
            {
                return true;
            }
            return false;
        }

        public static bool Justification_DynamoToSAP(ref cSapModel mySapModel, int Just, string FrmId)
        {
            //int cardinalPoint = 0; // use Just
            double[] offset1 = new double[3];
            double[] offset2 = new double[3];

            offset1[1] = 0;
            offset2[1] = 0;

            offset1[2] = 0;
            offset2[2] = 0;

            //TODO: Mapping Needed  Vertical Justification/ Lateral Justification 1 = bottom left2 = bottom center 3 = bottom right 4 = middle left 5 = middle center 6 = middle right 7 = top left 8 = top center 9 = top right 10 = centroid 11 = shear center

            int ret = mySapModel.FrameObj.SetInsertionPoint(FrmId, Just, false, true, ref offset1, ref offset2);

            return true;
        }

        public static int DefineSection(ref cSapModel mySapModel, string SectionName, string MatProp, string SecCatalog, string SectionProfile)
        {
            return mySapModel.PropFrame.ImportProp(SectionName, MatProp, SecCatalog, SectionProfile);
        }
        
        public static int SetSection(ref cSapModel mySapModel, string Name, string SectionProfile)
        {
            return mySapModel.FrameObj.SetSection(Name, SectionProfile);           
        }

    }
}
