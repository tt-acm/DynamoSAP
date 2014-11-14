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
            bool exists = SAPConnection.StructureMapper.IsSectionExistsFrm(ref mySapModel, f.SectionProfile);
            if (!exists) // if doesnot exists define new sec property
            {
                string MatProp = SAPConnection.MaterialMapper.DynamoToSap(f.Material);
                string SecCatalog = "AISC14.pro"; // US_Imperial TODO: ASK TO USER ?
                //define new section property
                SAPConnection.StructureMapper.DefinePropFrm(ref mySapModel, f.SectionProfile, MatProp, SecCatalog, f.SectionProfile);
            }
            //Assign section profile toFrame
            SAPConnection.StructureMapper.SetSectionFrm(ref mySapModel, f.Label, f.SectionProfile);

            // 3. Set Justification TODO: Vertical & Lateral Justification
            SAPConnection.JustificationMapper.DynamoToSAPFrm(ref mySapModel, f.Label, f.Justification); // TO DO: lateral and vertical justificaton

            // 4. Set Rotation
            SAPConnection.JustificationMapper.SetRotationFrm(ref mySapModel, f.Label, f.Rotation);

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

            // Dictionary to hold Structure Frames on <string, string> <GUID,Label>
            Dictionary<string, string> SapModelFrmDict = new Dictionary<string, string>();

            //2. Create Geometry
            foreach (var el in model.Frames)
            {
                if (el.GetType().ToString().Contains("Frame"))
                {
                    CreateFrame(el as Frame, ref mySapModel);
                    SapModelFrmDict.Add(el.GUID, el.Label);
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


            // 6. Loads
            if (model.Loads != null)
            {
                foreach (Load load in model.Loads)
                {
                    // get Frame Label
                    string frmId = string.Empty;
                    //string frmId = SapModelFrmDict[load.Frame.GUID];
                    bool get = SapModelFrmDict.TryGetValue(load.Frame.GUID, out frmId);

                    if (!string.IsNullOrEmpty(frmId))
                    {
                        if (load.LoadType == "PointLoad")
                        {
                            //Call the CreatePointLoad method
                            SAPConnection.LoadMapper.CreatePointLoad(ref mySapModel, frmId, load.lPattern.Name, load.MyType, load.Dir, load.Dist, load.Val, load.CSys, load.RelDist, load.Replace);
                        }
                        if (load.LoadType == "DistributedLoad")
                        {
                            //Call the CreateDistributedLoad method
                            SAPConnection.LoadMapper.CreateDistributedLoad(ref mySapModel, frmId, load.lPattern.Name, load.MyType, load.Dir, load.Dist, load.Dist2, load.Val, load.Val2, load.CSys, load.RelDist, load.Replace);
                        }
                    }
                }
            }

            //// 7. Releases
            if (model.Releases != null)
            {
                foreach (Release rel in model.Releases)
                {
                    // get Frame Label
                    string frmId = string.Empty;
                    bool get = SapModelFrmDict.TryGetValue(rel.Frame.GUID, out frmId);

                    if (!string.IsNullOrEmpty(frmId))
                    {
                        List<bool> ireleases = new List<bool>();
                        ireleases.Add(rel.U1i); ireleases.Add(rel.U2i); ireleases.Add(rel.u3i);
                        ireleases.Add(rel.R1i); ireleases.Add(rel.R2i); ireleases.Add(rel.R3i);

                        List<bool> jreleases = new List<bool>();
                        jreleases.Add(rel.U1j); jreleases.Add(rel.U2j); jreleases.Add(rel.u3j);
                        jreleases.Add(rel.R1j); jreleases.Add(rel.R2j); jreleases.Add(rel.R3j);

                        bool[] iireleases = ireleases.ToArray();
                        bool[] jjreleases = jreleases.ToArray();

                        SAPConnection.ReleaseMapper.SetReleases(ref mySapModel, frmId, iireleases, jjreleases);
                    }
                }
            }

            //if can't set to null, will be a hanging process
            mySapModel = null;
            mySapObject = null;
        }
    }
}
