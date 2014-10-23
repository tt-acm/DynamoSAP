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
        private static cSapModel mySapModel;

        //PRIVATE METHODS
        #region
        private static void CreateFrame(Frame f, ref cSapModel mySapModel)
        {
            string dummy = string.Empty;
            long ret = 0;
            //1. Create Frame
            ret = mySapModel.FrameObj.AddByCoord(f.BaseCrv.StartPoint.X,
                f.BaseCrv.StartPoint.Y,
                f.BaseCrv.StartPoint.Z,
                f.BaseCrv.EndPoint.X,
                f.BaseCrv.EndPoint.Y,
                f.BaseCrv.EndPoint.Z,
                ref dummy);

            // TODO: set custom name !
            f.Label = dummy; // for now passing the SAP label to Frame label!
            
            /// WHY NOT WORKING ????
            ////2. Get -Define -Set Section
            //bool exists = Utilities.ProfileMapper_IsSectionExists(ref mySapModel, f.SectionProfile);
            //if (!exists) 
            //{
            //    string MatProp = Utilities.MaterialMapper_DyanmoToSap(f.Material);
            //    string SecCatalog = "AISC14.pro"; // US_Imperial

            //    //define new section property
            //    ret = mySapModel.PropFrame.ImportProp(f.SectionProfile, MatProp, SecCatalog, f.SectionProfile);
            //}
            ////Assign section profile toFrame
            //ret = mySapModel.FrameObj.SetSection(dummy, f.SectionProfile);

            //// 3. Set Justification TODO: Vertical & Lateral Justification
            //Utilities.Justification_DynamoToSAP(ref mySapModel, f.Justification, dummy);

            //// 4. Set Rotation
            //ret = mySapModel.FrameObj.SetLocalAxes(dummy, f.Rotation);


        }


        //internal static bool IsSectionExists(string SectionProfile)
        //{
        //    int number = 0;
        //    string[] SectionNames = null;
        //    long ret = mySapModel.PropFrame.GetNameList(ref number, ref SectionNames);

        //    if (SectionNames.Contains(SectionProfile))
        //    {
        //        return false;
        //    }
        //    return true;
        //}

        //private static void Justification_DynamoToSAP(int Justification, string FrmId)
        //{
        //    //int cardinalPoint = 0; // use Just
        //    double[] offset1 = new double[3];
        //    double[] offset2 = new double[3];

        //    offset1[1] = 0;
        //    offset2[1] = 0;

        //    offset1[2] = 0;
        //    offset2[2] = 0;

        //    //TODO: Mapping Needed  Vertical Justification/ Lateral Justification 1 = bottom left2 = bottom center 3 = bottom right 4 = middle left 5 = middle center 6 = middle right 7 = top left 8 = top center 9 = top right 10 = centroid 11 = shear center

        //    long ret = mySapModel.FrameObj.SetInsertionPoint(FrmId, Justification, false, true, ref offset1, ref offset2);
        //}

        #endregion



        //DYNAMO NODES
        public static string CreateSAPModel(List<Element> SAPElements)
        {
            //1. Instantiate SAPModel
            SAP2000v16.SapObject mySapObject = null;
            //SAP2000v16.cSapModel mySapModel = null;

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
