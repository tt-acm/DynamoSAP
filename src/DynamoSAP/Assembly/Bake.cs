/// Developed by Thornton Tomasetti's CORE Studio for Autodesk
/// http://core.thorntontomasetti.com
/// CORE Developers: Elcin Ertugrul and Ana Garcia Puyol

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
using DynamoSAP.Definitions;

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
        private static List<string> SAPAreaList = new List<string>();
        private static List<string> SAPJointList = new List<string>();
        private static List<string> SAPGroupList = new List<string>();
        private static List<string> report = new List<string>();

        //// DYNAMO NODES ////

        /// <summary>
        ///  Create or Update SAP2000 model from Dynamo Structural Model
        /// </summary>
        /// <param name="StructuralModel">Dynamo Structural Model</param>
        /// <param name="Bake">Set true to bake the model in SAP2000</param>
        /// <param name="Units"></param>
        /// <param name="Delete"> Set false to update partial SAP Model! </param>
        /// <returns></returns>
        [MultiReturn("Structural Model", "Report")]
        public static Dictionary<string, object> ToSAP(StructuralModel StructuralModel, bool Bake, string Units = "kip_ft_F", bool Delete = true)
        {
            if (Bake)
            {
                // 1. Calculate Lenght Conversion Factor
                string fromUnit = "m"; // Dynamo API Units
                LengthUnit LU = DynamoUnits.Length.LengthUnit; // Display Units 

                double LengthSF = SAPConnection.Utilities.UnitConversion(Units, fromUnit); // Lenght Conversion Factor

                // Clear Frame & Area Dictionaries to hold 
                SAPFrmList.Clear();
                SAPAreaList.Clear();
                SAPJointList.Clear();
                report.Clear();

                // 2. Create new SAP Model and bake Stuctural Model 
                if (StructuralModel != null)
                {
                    CreateorUpdateSAPModel(ref StructuralModel, Units, LengthSF, Delete);
                }
            }
            else
            {
                throw new Exception("Node not run. Please, set boolean to true");
            }
            //return StructuralModel;

            return new Dictionary<string, object>
            {
                {"Structural Model", StructuralModel},
                {"Report", report}
            };
        }

        // Private Constructor
        private Bake() { }



        #region PRIVATE SAP METHODS
        //CREATE FRAME METHOD

        //Create or Update Frame
        private static void CreateorUpdateFrame(Frame f, ref cSapModel mySapModel, double SF, bool update)
        {
            string error = string.Empty;
            if (!update) // Create new 
            {
                // Draw Frm Object return Label
                string dummy = string.Empty;

                //1. Create Frame
                SAPConnection.StructureMapper.CreateorUpdateFrm(ref mySapModel, f.BaseCrv.StartPoint.X * SF,
                    f.BaseCrv.StartPoint.Y * SF,
                    f.BaseCrv.StartPoint.Z * SF,
                    f.BaseCrv.EndPoint.X * SF,
                    f.BaseCrv.EndPoint.Y * SF,
                    f.BaseCrv.EndPoint.Z * SF,
                    ref dummy, false, ref error);

                // Set custom Label to Frame in dynamo & Frame! User can match by using Label & ID
                bool renamed = SAPConnection.StructureMapper.ChangeNameSAPFrm(ref mySapModel, dummy, f.Label);
                if (!renamed)
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
                    ref id, true, ref error);
            }
            if (error != string.Empty)
            {
                report.Add(error);
                error = string.Empty;
            }

            // 3. Get or Define Section Profile
            bool exists = SAPConnection.StructureMapper.IsSectionExistsFrm(ref mySapModel, f.SecProp.SectName, ref error);
            if (error != string.Empty)
            {
                report.Add(error);
                error = string.Empty;
            }

            if (!exists) // if it does not exist, define a new sec property
            {
                string MatProp = SAPConnection.MaterialMapper.DynamoToSap(f.SecProp.MatProp);
                //Import new section property
                SAPConnection.StructureMapper.ImportPropFrm(ref mySapModel, f.SecProp.SectName, MatProp, f.SecProp.SectCatalog, ref error);
                if (error != string.Empty)
                {
                    report.Add(error);
                    error = string.Empty;
                }
            }
            //Assign section profile toFrame
            SAPConnection.StructureMapper.SetSectionFrm(ref mySapModel, f.Label, f.SecProp.SectName, ref error);
            if (error != string.Empty)
            {
                report.Add(error);
                error = string.Empty;
            }

            // 3. Set Justification TODO: Vertical & Lateral Justification
            SAPConnection.JustificationMapper.DynamoToSAPFrm(ref mySapModel, f.Label, f.Just, ref error); // TO DO: lateral and vertical justificaton
            if (error != string.Empty)
            {
                report.Add(error);
                error = string.Empty;
            }

            // 4. Set Rotation
            SAPConnection.JustificationMapper.SetRotationFrm(ref mySapModel, f.Label, f.Angle, ref error);
            if (error != string.Empty)
            {
                report.Add(error);
                error = string.Empty;
            }
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
            string error = string.Empty;
            SAPConnection.ReleaseMapper.Set(ref mySapModel, frm.Label, iireleases, jjreleases, ref error);

            if (error != string.Empty) report.Add(error);
        }

        // Set Loads to a frame
        private static void SetLoads(Frame frm, ref cSapModel mySapModel)
        {
            foreach (var load in frm.Loads)
            {
                if (load.LoadType == "PointLoad")
                {
                    //Call the CreatePointLoad method
                    SAPConnection.LoadMapper.CreatePointLoad(ref mySapModel, frm.Label, load.lPattern.name, load.FMType, load.Dir, load.Dist, load.Val, load.CSys, load.RelDist, true);
                }
                if (load.LoadType == "DistributedLoad")
                {
                    //Call the CreateDistributedLoad method
                    SAPConnection.LoadMapper.CreateDistributedLoad(ref mySapModel, frm.Label, load.lPattern.name, load.FMType, load.Dir, load.Dist, load.Dist2, load.Val, load.Val2, load.CSys, load.RelDist, true);
                }
            }
        }

        // Create or Update Area
        private static void CreateorUpdateArea(Shell s, ref cSapModel mySAPModel, double SF, bool update)
        {
            string error = string.Empty;
            if (!update) // create new one
            {
                string dummy = string.Empty;
                if (s.BaseMesh != null)
                {
                    SAPConnection.StructureMapper.CreateorUpdateArea(ref mySapModel, s.BaseMesh, ref dummy, false, SF, ref error);
                    if (error != string.Empty)
                    {
                        report.Add(error);
                        error = string.Empty;
                    }
                }
                else
                {
                    SAPConnection.StructureMapper.CreateorUpdateArea(ref mySapModel, s.BaseSurface, ref dummy, false, SF, ref error);
                    if (error != string.Empty)
                    {
                        report.Add(error);
                        error = string.Empty;
                    }
                }

                // Set custom Label to Frame in dynamo & Frame! User can match by using Label & ID
                bool renamed = SAPConnection.StructureMapper.ChangeNameSAPArea(ref mySapModel, dummy, s.Label);
                if (!renamed)
                {
                    s.Label = dummy;
                }
            }
            else // modify the existing
            {
                string id = s.Label;
                if (s.BaseMesh != null)
                {
                    SAPConnection.StructureMapper.CreateorUpdateArea(ref mySapModel, s.BaseMesh, ref id, true, SF, ref error);
                    if (error != string.Empty)
                    {
                        report.Add(error);
                        error = string.Empty;
                    }
                }
                else
                {
                    SAPConnection.StructureMapper.CreateorUpdateArea(ref mySapModel, s.BaseSurface, ref id, true, SF, ref error);
                    if (error != string.Empty)
                    {
                        report.Add(error);
                        error = string.Empty;
                    }
                }
            }

            // Define Shell Properties
            SAPConnection.StructureMapper.SetPropArea(ref mySapModel,
                s.shellProp.PropName,
                s.shellProp.ShellType,
                s.shellProp.DOF,
                s.shellProp.MatProp,
                s.shellProp.MatAngle,
                s.shellProp.Thickness * SF,
                s.shellProp.Bending,
                ref error);
            if (error != string.Empty)
            {
                report.Add(error);
                error = string.Empty;
            }

            SAPConnection.StructureMapper.SetShellPropArea(ref mySapModel, s.Label, s.shellProp.PropName, ref error);
            if (error != string.Empty) report.Add(error);

        }

        // Create or Update Joint
        private static void CreateorUpdateJoint(Joint j, ref cSapModel mySAPModel, double SF, bool update)
        {
            if (!update) // create new one
            {
                string dummy = string.Empty;
                StructureMapper.CreateorUpdateJoint(ref mySapModel, j.BasePt, ref dummy, false, SF);
                // Set custom Label to Frame in dynamo & Frame! User can match by using Label & ID
                bool renamed = SAPConnection.StructureMapper.ChangeNameSAPJoint(ref mySapModel, dummy, j.Label);
                if (!renamed)
                {
                    j.Label = dummy;
                }

            }
            else // modify the existing
            {
                string id = j.Label;
                StructureMapper.CreateorUpdateJoint(ref mySapModel, j.BasePt, ref id, true, SF);
            }

            // 2. Assigns Restraints to Node

            if (j.JointRestraint != null)
            {
                List<bool> restraints = new List<bool>();
                restraints.Add(j.JointRestraint.u1); restraints.Add(j.JointRestraint.u2); restraints.Add(j.JointRestraint.u3);
                restraints.Add(j.JointRestraint.r1); restraints.Add(j.JointRestraint.r2); restraints.Add(j.JointRestraint.r3);

                // Set restaints
                SAPConnection.RestraintMapper.Set(ref mySapModel, j.Label, restraints.ToArray());
            }

            ////  3. Assign Force to  Node
            //if (j.Loads != null)
            //{

            //}


        }

        //Create or Update Group
        private static void CreateorUpdateGroup(Group g, ref cSapModel mySAPModel, bool update)
        {
            if (!update)// define the group first
            {
                SAPConnection.GroupMapper.DefineGroup(ref mySapModel, g.Name);
            }
            else
            {
                // clear existing assignments
                SAPConnection.GroupMapper.ClearGroupAssigment(ref mySapModel, g.Name);
            }

            // Set assignments per element types
            List<string> Frms = new List<string>();
            List<string> Shells = new List<string>();
            List<string> Joints = new List<string>();
            try
            {
                Frms = (from el in g.GroupElements
                        where el.Type == Structure.Type.Frame
                        select el.Label).ToList();

                SAPConnection.GroupMapper.SetGroupAssign_Frm(ref mySapModel, g.Name, Frms);

            }
            catch (Exception) { }
            try
            {
                Shells = (from el in g.GroupElements
                          where el.Type == Structure.Type.Shell
                          select el.Label).ToList();

                SAPConnection.GroupMapper.SetGroupAssign_Shell(ref mySapModel, g.Name, Shells);

            }
            catch (Exception) { }
            try
            {
                Joints = (from el in g.GroupElements
                          where el.Type == Structure.Type.Joint
                          select el.Label).ToList();

                SAPConnection.GroupMapper.SetGroupAssign_Joint(ref mySapModel, g.Name, Joints);

            }
            catch (Exception) { }

        }

        // Create or Update Sap Model from a Dynamo Model
        private static void CreateorUpdateSAPModel(ref StructuralModel StructuralModel, string Units, double SF, bool delete)
        {

            string error = string.Empty;

            //1. INSTANTIATE NEW OR GRAB OPEN SAPMODEL 

            // check if any SAP file is open, grab 
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

            //2. Create or Update Frames (Sets Releases)
            // 2.a. Harvest the elements from SAP Model
            SAPConnection.StructureMapper.GetSAPFrameList(ref mySapModel, ref SAPFrmList); // frms
            SAPConnection.StructureMapper.GetSAPAreaList(ref mySapModel, ref SAPAreaList); // areas
            SAPConnection.StructureMapper.GetSAPJointList(ref mySapModel, ref SAPJointList); // joints
            
            // 2a. DELETE 
            if (delete)
            {
                //Delete Frms from SAP not in Structural elements
                foreach (var sapfrm in SAPFrmList)
                {
                    Element el = null;
                    try
                    {
                        el = (from f in StructuralModel.StructuralElements
                              where f.Label == sapfrm && f.Type == Structure.Type.Frame
                              select f).First();
                    }
                    catch (Exception) { }

                    if (el == null) // not in Dynamo Structure so delete from SAP Model
                    {
                        SAPConnection.StructureMapper.DeleteFrm(ref mySapModel, sapfrm);
                    }

                }

                // Delete Areas from SAP not in Structural elements
                foreach (var sapArea in SAPAreaList)
                {
                    //Element el = null;
                    //try
                    //{
                    //    el = (from f in StructuralModel.StructuralElements
                    //          where f.Label == sapArea
                    //          select f).First();
                    //}
                    //catch (Exception) { }

                    //if (el == null)
                    //{
                    SAPConnection.StructureMapper.DeleteArea(ref mySapModel, sapArea, ref error);
                    if (error != string.Empty)
                    {
                        report.Add(error);
                        error = string.Empty;
                    }
                    //}
                }

                // Delete Joints from SAP not in Structural elements
                foreach (var sapJoint in SAPJointList)
                {
                    Element el = null;
                    try
                    {
                        el = (from f in StructuralModel.StructuralElements
                              where f.Label == sapJoint && f.Type == Structure.Type.Joint
                              select f).First();
                    }
                    catch (Exception) { }

                    if (el == null && sapJoint.StartsWith("dyn")) // not in Dynamo Structure so delete from SAP Model
                    {
                        SAPConnection.StructureMapper.DeleteJoint(ref mySapModel, sapJoint, ref error);
                        if (error != string.Empty)
                        {
                            report.Add(error);
                            error = string.Empty;
                        }
                    }


                }

            }


            //2. CREATE OR UPDATE SIMULTENOUSLY
            //2.b. Create or Update 
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
                else if (el.Type == Structure.Type.Shell)
                {
                    bool isupdate = SAPAreaList.Contains(el.Label);

                    CreateorUpdateArea(el as Shell, ref mySapModel, SF, false);

                }
                else if (el.Type == Structure.Type.Joint)
                {
                    bool isupdate = SAPJointList.Contains(el.Label);
                    CreateorUpdateJoint(el as Joint, ref mySapModel, SF, isupdate);
                }
            }

            // LinqInquiry
            List<Definition> LPatterns = new List<Definition>();
            try
            {
                LPatterns = (from def in StructuralModel.ModelDefinitions
                             where def.Type == Definitions.Type.LoadPattern
                             select def).ToList();
            }
            catch (Exception)
            {
            }
            // Add Load Patterns to the SAP Model
            if (LPatterns.Count > 0)
            {
                foreach (LoadPattern lp in LPatterns)
                {
                    //Call the AddLoadPattern method
                    SAPConnection.LoadMapper.AddLoadPattern(ref mySapModel, lp.name, lp.type, lp.multiplier);
                }
            }

            List<Definition> LCases = new List<Definition>();
            try
            {
                LCases = (from def in StructuralModel.ModelDefinitions
                          where def.Type == Definitions.Type.LoadCase
                          select def).ToList();
            }
            catch { }

            if (LCases.Count > 0)
            {
                foreach (LoadCase lc in LCases)
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



            List<Definition> LCombo = new List<Definition>();

            try
            {
                LCombo = (from def in StructuralModel.ModelDefinitions
                          where def.Type == Definitions.Type.LoadCombo
                          select def).ToList();
            }
            catch { }

            if (LCombo.Count > 0)
            {
                foreach (LoadCombo lc in LCombo)
                {
                    List<string> types = new List<string>();
                    List<string> names = new List<string>();
                    List<double> SFs = new List<double>();

                    for (int i = 0; i < lc.loadDefinitions.Count; i++)
                    {
                        if (lc.loadDefinitions[i].Type == Definitions.Type.LoadCase)
                        {
                            types.Add("LoadCase");
                            names.Add(((LoadCase)lc.loadDefinitions[i]).name);

                        }
                        else if (lc.loadDefinitions[i].Type == Definitions.Type.LoadCombo)
                        {
                            types.Add("LoadCombo");
                            names.Add(((LoadCombo)lc.loadDefinitions[i]).name);

                        }
                        SFs.Add(lc.sFs[i]);
                    }


                    string[] Dtypes = types.ToArray();
                    string[] Dnames = names.ToArray();
                    double[] DSFs = SFs.ToArray();

                    SAPConnection.LoadMapper.AddLoadCombo(ref mySapModel, lc.name, Dtypes, Dnames, DSFs, lc.type);
                }
            }

            // Set Loads 
            foreach (var el in StructuralModel.StructuralElements)
            {
                if (el.Type == Structure.Type.Frame)
                {
                    Frame frm = el as Frame;

                    // Set Loads
                    if (frm.Loads != null)
                    {
                        SetLoads(el as Frame, ref mySapModel);
                    }
                }
            }

            // Create or Update Groups 
            // Harvest the names of the groups// delete the ones not in the SAP Model
            SAPConnection.GroupMapper.GetSAPGroupList(ref mySapModel, ref SAPGroupList);

            List<Definition> Groups = new List<Definition>();
            try
            {
                Groups = (from def in StructuralModel.ModelDefinitions
                          where def.Type == Definitions.Type.Group
                          select def).ToList();
            }
            catch (Exception)
            {
            }

            // Update or Create new one and set assignments    
            List<string> tempNames = new List<string>();
            if (Groups.Count > 0)
            {
                foreach (Group g in Groups)
                {
                    tempNames.Add(g.Name);

                    bool update = false;
                    if (SAPGroupList.Contains(g.Name))
                    {
                        update = true;
                    }

                    CreateorUpdateGroup(g, ref mySapModel, update);
                }

                // Delete from SAP Model
                foreach (var g in SAPGroupList)
                {
                    if (!tempNames.Contains(g))
                    {
                        SAPConnection.GroupMapper.Delete(ref mySapModel, g);
                    }
                }

            }

            // refresh View 
            SAPConnection.StructureMapper.RefreshView(ref mySapModel);

            // Delete unconnected points at the SAP
            SAPConnection.StructureMapper.DeleteUnconnectedPts(ref mySapModel);

            //if can't set to null, will be a hanging process
            mySapModel = null;
            mySapObject = null;
        }

        #endregion
    }
}
