using System;
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

        //// DYNAMO NODES ////

        /// <summary>
        /// Create SAP2000 model from Dynamo Structural Model
        /// </summary>
        /// <param name="StructuralModel">Structural Model to bake</param>
        /// <param name="Units">Set Units of SapModel</param>
        /// <param name="Bake">Set Boolean to True to bake the model</param>
        /// <returns>Structural Model</returns>
        public static StructuralModel ToSAP(StructuralModel StructuralModel, bool Bake, string Units = "kip_ft_F")
        {
            // 1. Calculate Lenght Conversion Factor
            string fromUnit = "m"; // Dynamo API Units
            LengthUnit LU = DynamoUnits.Length.LengthUnit; // Display Units 

            double LengthSF = SAPConnection.Utilities.UnitConversion(Units, fromUnit); // Lenght Conversion Factor

            // 2. Create new SAP Model and bake Stuctural Model 
            if (StructuralModel != null)
            {
                if (Bake) CreateSAPModel(ref StructuralModel, Units , LengthSF);
            }
            return StructuralModel;
        }

        private Bake() { }

        #region PRIVATE SAP METHODS
        //CREATE FRAME METHOD
        private static void CreateFrame(Frame f, ref cSapModel mySapModel, double SF)
        {
            // Draw Frm Object return Label
            string dummy = string.Empty;
            //1. Create Frame
            SAPConnection.StructureMapper.DrawFrm(ref mySapModel, f.BaseCrv.StartPoint.X*SF,
                f.BaseCrv.StartPoint.Y*SF,
                f.BaseCrv.StartPoint.Z*SF,
                f.BaseCrv.EndPoint.X*SF,
                f.BaseCrv.EndPoint.Y*SF,
                f.BaseCrv.EndPoint.Z*SF,
                ref dummy);

            // TODO: set custom name !
            f.Label = dummy; // for now passing the SAP label to Frame label!

            // 2. Set GUID
            SAPConnection.StructureMapper.SetGUIDFrm(ref mySapModel, f.Label, f.GUID);

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

            SAPConnection.ReleaseMapper.SetReleases(ref mySapModel, frm.Label, iireleases, jjreleases);
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

        // Create Sap Model from a Dynamo Model
        private static void CreateSAPModel(ref StructuralModel StructuralModel, string Units, double SF)
        {
            string report = string.Empty;

            //1. Instantiate SAPModel
            SAP2000v16.SapObject mySapObject = null;

            try
            {
                SAPConnection.Initialize.InitializeSapModel(ref mySapObject, ref mySapModel, Units);
            }
            catch (Exception)
            {
                SAPConnection.Initialize.Release(ref mySapObject, ref mySapModel);
            };

            //2. Create Frames (Sets Releases)
            foreach (var el in StructuralModel.StructuralElements)
            {
                if (el.GetType().ToString().Contains("Frame"))
                {
                    CreateFrame(el as Frame, ref mySapModel, SF);
                    Frame frm = el as Frame;

                    // Set Releases
                    if (frm.Releases != null)
                    {
                        SetReleases(el as Frame, ref mySapModel); // Set releases 
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

            //if can't set to null, will be a hanging process
            mySapModel = null;
            mySapObject = null;
        }
        #endregion
    }
}
