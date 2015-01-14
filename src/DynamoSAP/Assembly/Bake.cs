﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.InteropServices;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.IO;

using SAPConnection;
using DynamoSAP.Structure;

//DYNAMO
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;
using DynamoUnits;

//SAP 
using SAP2000v16;

namespace DynamoSAP.Assembly
{
    /// <summary>
    /// Create a structural model in SAP
    /// </summary>
    public class Bake
    {
        private static cSapModel mySapModel;
        private static List<string> SAPFrmList = new List<string>(); // List to hold 


        //// DYNAMO NODES ////

        /// <summary>
        ///  Create or Update SAP2000 model from Dynamo Structural Model
        /// </summary>
        /// <param name="StructuralModel">Dynamo Structural Model</param>
        /// <param name="Bake">Set true to bake the model in SAP2000</param>
        /// <param name="Units"></param>
        /// <param name="Delete"> Set false to update partial SAP Model! </param>
        /// <returns></returns>
        public static StructuralModel ToSAP(StructuralModel StructuralModel, bool Bake, string Units = "kip_ft_F", bool Delete = true )
        {
            // 1. Calculate Lenght Conversion Factor
            string fromUnit = "m"; // Dynamo API Units
            LengthUnit LU = DynamoUnits.Length.LengthUnit; // Display Units 

            double LengthSF = SAPConnection.Utilities.UnitConversion(Units, fromUnit); // Lenght Conversion Factor

            // Clear Frame Dictionary
            SAPFrmList.Clear();

            // 2. Create new SAP Model and bake Stuctural Model 
            if (StructuralModel != null)
            {
                if (Bake) CreateorUpdateSAPModel(ref StructuralModel, Units , LengthSF, Delete);
            }
            return StructuralModel;
        }

        // Private Constructor
        private Bake() { }



        #region PRIVATE SAP METHODS
        //CREATE FRAME METHOD

        //Create or Update Frame
        private static void CreateorUpdateFrame(Frame f, ref cSapModel mySapModel, double SF, bool update)
        {
            if (!update) // Create new 
            {
                // Draw Frm Object return Label
                string dummy = string.Empty;
                //1. Create Frame
                SAPConnection.StructureMapper.CreateorUpdateFrm(ref mySapModel, f.BaseCrv.StartPoint.X * SF,
                    f.BaseCrv.StartPoint.Y*SF,
                    f.BaseCrv.StartPoint.Z*SF,
                    f.BaseCrv.EndPoint.X*SF,
                    f.BaseCrv.EndPoint.Y*SF,
                    f.BaseCrv.EndPoint.Z*SF,
                    ref dummy, false);

                // Set custom Label to Frame in dynamo & Frame! User can match by using Label & ID
                bool renamed = SAPConnection.StructureMapper.ChangeNameSAPFrm(ref mySapModel, dummy, String.Format("dyn_{0}", f.ID.ToString()));
                if (renamed)
                {
                   f.Label = String.Format("dyn_{0}", f.ID.ToString());  
                }
                else 
                {
                    f.Label = dummy;
                }

            }
            else // Update Coordinates
            { 
                string id = f.Label;
                SAPConnection.StructureMapper.CreateorUpdateFrm(ref mySapModel, f.BaseCrv.StartPoint.X * SF,
                    f.BaseCrv.StartPoint.Y * SF,
                    f.BaseCrv.StartPoint.Z * SF,
                    f.BaseCrv.EndPoint.X * SF,
                    f.BaseCrv.EndPoint.Y * SF,
                    f.BaseCrv.EndPoint.Z * SF,
                    ref id, true);
            }


            // 3. Get or Define Section Profile
            bool exists = SAPConnection.StructureMapper.IsSectionExistsFrm(ref mySapModel, f.SecProp.SectName);
            if (!exists) // if doesnot exists define new sec property
            {
                string MatProp = SAPConnection.MaterialMapper.DynamoToSap(f.SecProp.MatProp);
                //Import new section property
                SAPConnection.StructureMapper.ImportPropFrm(ref mySapModel, f.SecProp.SectName, MatProp, f.SecProp.SectCatalog);
            }
            //Assign section profile toFrame
            SAPConnection.StructureMapper.SetSectionFrm(ref mySapModel, f.Label, f.SecProp.SectName);

            // 3. Set Justification TODO: Vertical & Lateral Justification
            SAPConnection.JustificationMapper.DynamoToSAPFrm(ref mySapModel, f.Label, f.Just); // TO DO: lateral and vertical justificaton

            // 4. Set Rotation
            SAPConnection.JustificationMapper.SetRotationFrm(ref mySapModel, f.Label, f.Angle);

        }

        // Set releases of a Frame
        private static void SetReleases(Frame frm, ref cSapModel mySapModel)
        {

            List<bool> ireleases = new List<bool>();
            ireleases.Add(frm.Releases.u1i); ireleases.Add(frm.Releases.u2i); ireleases.Add(frm.Releases.u3i);
            ireleases.Add(frm.Releases.r1i); ireleases.Add(frm.Releases.r2i); ireleases.Add(frm.Releases.r3i);

            List<bool> jreleases = new List<bool>();
            jreleases.Add(frm.Releases.u1j); jreleases.Add(frm.Releases.u2j); jreleases.Add(frm.Releases.u3j);
            jreleases.Add(frm.Releases.r1j); jreleases.Add(frm.Releases.r2j); jreleases.Add(frm.Releases.r3j);

            bool[] iireleases = ireleases.ToArray();
            bool[] jjreleases = jreleases.ToArray();

            SAPConnection.ReleaseMapper.Set(ref mySapModel, frm.Label, iireleases, jjreleases);
        }

        // Set Loads to a frame
        private static void SetLoads(Frame frm, ref cSapModel mySapModel)
        {
            foreach (var load in frm.Loads)
            {
                if (load.LoadType == "PointLoad")
                {
                    //Call the CreatePointLoad method
                    SAPConnection.LoadMapper.CreatePointLoad(ref mySapModel, frm.Label, load.lPattern.name, load.FMType, load.Dir, load.Dist, load.Val, load.CSys, load.RelDist, false);
                }
                if (load.LoadType == "DistributedLoad")
                {
                    //Call the CreateDistributedLoad method
                    SAPConnection.LoadMapper.CreateDistributedLoad(ref mySapModel, frm.Label, load.lPattern.name, load.FMType, load.Dir, load.Dist, load.Dist2, load.Val, load.Val2, load.CSys, load.RelDist, false);
                }
            }
        }

        // Create or Update Sap Model from a Dynamo Model
        private static void CreateorUpdateSAPModel(ref StructuralModel StructuralModel, string Units, double SF, bool delete)
        {
            // check if any SAP file is open, grab 

            string report = string.Empty;

            //1. Instantiate or Grab SAPModel

            SAP2000v16.SapObject mySapObject = null;
            string SapModelUnits = string.Empty;

            // Open & instantiate SAP file
            Initialize.GrabOpenSAP(ref mySapModel, ref SapModelUnits);

            if (mySapModel == null)
            {
                // Open a blank SAP Model
                try
                {
                    SAPConnection.Initialize.InitializeSapModel(ref mySapObject, ref mySapModel, Units);
                }
                catch (Exception)
                {
                    SAPConnection.Initialize.Release(ref mySapObject, ref mySapModel);
                };
            }

            

            //2. CREATE OR UPDATE SIMULTENOUSLY
           

            //2. Create or Update Frames (Sets Releases)
            // Harvest the elements from SAP Model
            SAPConnection.StructureMapper.GetSAPFrameList(ref mySapModel, ref SAPFrmList); // frms

            foreach (var el in StructuralModel.StructuralElements)
            {
                if (el.Type == Structure.Type.Frame)
                {
                    bool isupdate = SAPFrmList.Contains(el.Label);

                    CreateorUpdateFrame(el as Frame, ref mySapModel, SF, isupdate);

                    Frame frm = el as Frame;
                    // Set Releases
                    if (frm.Releases != null)
                    {
                        SetReleases(el as Frame, ref mySapModel); // Set releases 
                    }
                }
            }

            // DELETE 
            if (delete)
            {
                //Frms from SAP not in Structural elements
                foreach (var sapfrm in SAPFrmList)
                {
                    Element el = null;
                    try
                    {
                        el = (from f in StructuralModel.StructuralElements
                              where f.Label == sapfrm
                              select f).First();
                    }
                    catch (Exception) { }

                    if (el == null) // not in Dynamo Structure so delete from SAP Model
                    {
                        SAPConnection.StructureMapper.DeleteFrm(ref mySapModel, sapfrm);
                    }

                } 
            }

            // 3. Assigns Restraints to Nodes
            if (StructuralModel.Restraints != null)
            {
                foreach (var rest in StructuralModel.Restraints)
                {
                    List<bool> restraints = new List<bool>();
                    restraints.Add(rest.u1); restraints.Add(rest.u2); restraints.Add(rest.u3);
                    restraints.Add(rest.r1); restraints.Add(rest.r2); restraints.Add(rest.r3);

                    // Set restaints
                    SAPConnection.RestraintMapper.Set(ref mySapModel, rest.pt, restraints.ToArray());
                }
            }


            // 4. Add Load Patterns
            if (StructuralModel.LoadPatterns != null)
            {
                foreach (LoadPattern lp in StructuralModel.LoadPatterns)
                {
                    //Call the AddLoadPattern method
                    SAPConnection.LoadMapper.AddLoadPattern(ref mySapModel, lp.name, lp.type, lp.multiplier);
                }
            }

            // 5. Define Load Cases

            if (StructuralModel.LoadCases != null)
            {
                foreach (LoadCase lc in StructuralModel.LoadCases)
                {

                    List<string> types = new List<string>();
                    List<string> names = new List<string>();
                    List<double> SFs = new List<double>();

                    for (int i = 0; i < lc.loadPatterns.Count; i++)
                    {
                        types.Add("Load");
                        names.Add(lc.loadPatterns[i].name);
                        SFs.Add(lc.sFs[i]);
                    }

                    string[] Dtypes = types.ToArray();
                    string[] Dnames = names.ToArray();
                    double[] DSFs = SFs.ToArray();

                    SAPConnection.LoadMapper.AddLoadCase(ref mySapModel, lc.name, types.Count(), ref Dtypes, ref Dnames, ref DSFs, lc.type);
                }
            }

            foreach (var el in StructuralModel.StructuralElements)
            {
                if (el.GetType().ToString().Contains("Frame"))
                {
                    Frame frm = el as Frame;

                    // Set Loads
                    if (frm.Loads != null)
                    {
                        SetLoads(el as Frame, ref mySapModel);
                    }
                }
            }

            // refresh View 

            SAPConnection.StructureMapper.RefreshView(ref mySapModel);

            //if can't set to null, will be a hanging process
            mySapModel = null;
            mySapObject = null;
        }

        #endregion
    }
}
