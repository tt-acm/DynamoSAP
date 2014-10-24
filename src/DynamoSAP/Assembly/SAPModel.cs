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
using DynamoSAP.Analysis;

//DYNAMO
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;

//SAP 
using SAP2000v16;

namespace DynamoSAP.Assembly
{
    public class SAPModel
    {
        private static int ret;
        private static cSapModel mySapModel;

        //// PRIVATE METHODS ////
        #region
        //CREATE FRAME METHOD
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
            //2. Get -Define -Set Section
            bool exists = SAPApplication.Application.IsSectionExists(f.SectionProfile, ref mySapModel);
            if (!exists)
            {
                string MatProp = SAPApplication.Application.MaterialMapper_DyanmoToSap(f.Material);
                string SecCatalog = "AISC14.pro"; // US_Imperial

                //define new section property
                //ret = mySapModel.PropFrame.ImportProp(f.SectionProfile, MatProp, SecCatalog, f.SectionProfile);
            }
            //Assign section profile toFrame
            //ret = mySapModel.FrameObj.SetSection(dummy, f.SectionProfile);

            // 3. Set Justification TODO: Vertical & Lateral Justification
            SAPApplication.Application.Justification_DynamoToSAP(ref mySapModel, f.Justification, dummy);

            // 4. Set Rotation
            //ret = mySapModel.FrameObj.SetLocalAxes(dummy, f.Rotation);

        }

        //CREATE LOAD METHODS
        private static void CreatePointLoad(Load load)
        {
           // ret = mySapModel.FrameObj.SetLoadPoint(load.FrameName, load.LoadPat, load.MyType, load.Dir, load.Dist, load.Val, load.CSys, load.RelDist, load.Replace);
        }
        private static void CreateDistributedLoad(Load load)
        {
           // ret = mySapModel.FrameObj.SetLoadDistributed(load.FrameName, load.LoadPat, load.MyType, load.Dir, load.Dist, load.Dist2, load.Val, load.Val2, load.CSys, load.RelDist, load.Replace);
        }

        //DEFINE LOAD PATTERN METHOD
        private static void AddLoadPattern(LoadPattern loadPat)
        { 
            //ret = mySapModel.LoadPatterns.Add(loadPat.Name,loadPat.LoadPatternType,loadPat.Multiplier);
        }

        #endregion



        //// DYNAMO NODES ////
        public static string CreateSAPModel(List<Element> SAPElements, List<LoadPattern> SAPLoadPatterns, List<Load> SAPLoads)
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
                    CreateFrame(el as Frame, ref mySapModel);
                }
            }



            // 3. Assigns Restraints to Nodes



            // 4. Add Load Patterns

            foreach (LoadPattern loadPat in SAPLoadPatterns)
            {
                //Call the AddLoadPattern method
                
                AddLoadPattern(loadPat);              
            }

            // 5. Define Load Cases
            


            // 6. Loads 
            foreach (Load load in SAPLoads)
            {
                if (load.LoadType == "PointLoad")
                {
                    //Call the CreatePointLoad method
                    
                    CreatePointLoad(load);
                }
                if (load.LoadType == "DistributedLoad")
                {
                    //Call the CreateDistributedLoad method
                   
                    CreateDistributedLoad(load);
                }
            }

            //if can't set to null, will be a hanging process
            mySapModel = null;
            mySapObject = null;

            return "Success";
        }


    }
}
