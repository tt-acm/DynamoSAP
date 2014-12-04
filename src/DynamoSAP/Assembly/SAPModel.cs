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

//SAP 
using SAP2000v16;

namespace DynamoSAP.Assembly
{
    [SupressImportIntoVM]
    public class SAPModel
    {
        private static cSapModel mySapModel;

        //// PRIVATE METHODS ////
        #region
        //CREATE FRAME METHOD
        private static void CreateFrame(Frame f, ref cSapModel mySapModel)
        {
            // Draw Frm Object return Label
            string dummy = string.Empty;
            //1. Create Frame
            SAPConnection.StructureMapper.DrawFrm(ref mySapModel, f.BaseCrv.StartPoint.X,
                f.BaseCrv.StartPoint.Y,
                f.BaseCrv.StartPoint.Z,
                f.BaseCrv.EndPoint.X,
                f.BaseCrv.EndPoint.Y,
                f.BaseCrv.EndPoint.Z,
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
                    SAPConnection.LoadMapper.CreatePointLoad(ref mySapModel, frm.Label, load.lPattern.Name, load.FMType, load.Dir, load.Dist, load.Val, load.CSys, load.RelDist, false);
                }
                if (load.LoadType == "DistributedLoad")
                {
                    //Call the CreateDistributedLoad method
                    SAPConnection.LoadMapper.CreateDistributedLoad(ref mySapModel, frm.Label, load.lPattern.Name, load.FMType, load.Dir, load.Dist, load.Dist2, load.Val, load.Val2, load.CSys, load.RelDist, false);
                }
            }
        }

        #endregion

        //// DYNAMO NODES ////
        //public static string CreateSAPModel(List<Element> SAPElements, List<LoadPattern> SAPLoadPatterns, List<LoadCase> SAPLoadCases, List<Restraint> SAPRestraints, List<Load> SAPLoads, List<Release> SAPReleases)
        public static void CreateSAPModel(ref StructuralModel model)
        {
            string report = string.Empty;

            //1. Instantiate SAPModel
            SAP2000v16.SapObject mySapObject = null;

            try
            {
                SAPConnection.Initialize.InitializeSapModel(ref mySapObject, ref mySapModel);
            }
            catch (Exception)
            {
                SAPConnection.Initialize.Release(ref mySapObject, ref mySapModel);
            };


            //2. Create Geometry
            foreach (var el in model.StructuralElements)
            {
                if (el.GetType().ToString().Contains("Frame"))
                {
                        CreateFrame(el as Frame, ref mySapModel);
                        Frame frm = el as Frame;

                        // Set Releases
                        if (frm.Releases != null)
                        {
                            SetReleases(el as Frame, ref mySapModel); // Set releases 
                        }
                        // Set Loads
                        if (frm.Loads.Count > 0)
                        {
                            SetLoads(el as Frame, ref mySapModel);
                        }

                }
            }


            // 3. Assigns Restraints to Nodes
            if (model.Restraints != null)
            {
                foreach (var rest in model.Restraints)
                {
                    List<bool> restraints = new List<bool>();
                    restraints.Add(rest.U1); restraints.Add(rest.U2); restraints.Add(rest.U3);
                    restraints.Add(rest.R1); restraints.Add(rest.R2); restraints.Add(rest.R3);

                    // Set restaints
                    SAPConnection.RestraintMapper.SetRestaints(ref mySapModel, rest.Pt, restraints.ToArray());
                }
            }


            // 4. Add Load Patterns
            if (model.LoadPatterns != null)
            {
                foreach (LoadPattern lp in model.LoadPatterns)
                {
                    //Call the AddLoadPattern method
                    SAPConnection.LoadMapper.AddLoadPattern(ref mySapModel, lp.Name, lp.Type, lp.Multiplier);
                }
            }

            // 5. Define Load Cases

            if (model.LoadCases != null)
            {
                foreach (LoadCase lc in model.LoadCases)
                {

                    List<string> types = new List<string>();
                    List<string> names = new List<string>();
                    List<double> SFs = new List<double>();

                    for (int i = 0; i < lc.LoadPatterns.Count; i++)
                    {
                        types.Add("Load");
                        names.Add(lc.LoadPatterns[i].Name);
                        SFs.Add(lc.SFs[i]);
                    }

                    string[] Dtypes = types.ToArray();
                    string[] Dnames = names.ToArray();
                    double[] DSFs = SFs.ToArray();

                    SAPConnection.LoadMapper.AddLoadCase(ref mySapModel, lc.Name, types.Count(), ref Dtypes, ref Dnames, ref DSFs, lc.Type);
                }
            }

            //if can't set to null, will be a hanging process
            mySapModel = null;
            mySapObject = null;
        }
    }
}
