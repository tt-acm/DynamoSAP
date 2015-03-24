/// Developed by Thornton Tomasetti's CORE Studio for Autodesk
/// http://core.thorntontomasetti.com
/// CORE Developers: Elcin Ertugrul and Ana Garcia Puyol

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SAP2000v16;
using DynamoSAP_UI;

// interop.COM services for SAP
using System.Runtime.InteropServices;

//DYNAMO
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;

namespace SAPConnection
{
    [SupressImportIntoVM]
    public class StructureMapper
    {
        // Draw Frame Object return ID
        public static void CreateorUpdateFrm(ref cSapModel Model, double iX, double iY, double iZ, double jX, double jY, double jZ, ref string Id, bool update, ref string error)
        {
            if (!update)
            {
                //1. Create Frame
                string dummy = string.Empty;
                long ret = Model.FrameObj.AddByCoord(iX, iY, iZ, jX, jY, jZ, ref dummy);
                Id = dummy;
                if (ret == 1) error = string.Format("Error creating frame{0}", dummy);
            }
            else
            {
                // update location
                string startPoint = string.Empty;
                string endPoint = string.Empty;
                //long ret = Model.FrameObj.GetPoints(Id, ref startPoint, ref endPoint);
                //ret = Model.EditPoint.ChangeCoordinates(startPoint, iX, iY, iZ);
                //ret = Model.EditPoint.ChangeCoordinates(endPoint, jX, jY, jZ);

                long ret = Model.PointObj.AddCartesian(iX, iY, iZ, ref startPoint);
                ret = Model.PointObj.AddCartesian(jX, jY, jZ, ref endPoint);
                ret = Model.EditFrame.ChangeConnectivity(Id, startPoint, endPoint);
                
                if (ret == 1) error = string.Format("Error updating frame{0}", Id);

            }

        }

        public static void CreateorUpdateArea(ref cSapModel Model, Mesh m, ref string Id, bool update, double SF, ref string error)
        {
            int counter = 0;
            if (!update) // Create new one
            {
                List<string> ProfilePts = new List<string>();
                
                long ret = 0;
                foreach (var v in m.VertexPositions)
                {
                    string dummy = null;
                    ret = Model.PointObj.AddCartesian(v.X * SF, v.Y * SF, v.Z * SF, ref dummy);
                    
                    ProfilePts.Add(dummy);
                }
                

                string[] names = ProfilePts.ToArray();
                string dummyarea = string.Empty;
                ret = Model.AreaObj.AddByPoint(ProfilePts.Count(), ref names, ref dummyarea);
                if (ret == 1) counter++;
                Id = dummyarea;
                
            }
            else
            {

                // Existing
                int eNumberofPts = 0;
                string[] ePtNames = null;
                long ret = Model.AreaObj.GetPoints(Id, ref eNumberofPts, ref ePtNames);
                
                // Compare the number of points
                if (eNumberofPts == m.VertexPositions.Count())
                {
                    for (int i = 0; i < eNumberofPts; i++)
                    {
                        ret = Model.EditPoint.ChangeCoordinates_1(ePtNames[i], m.VertexPositions[i].X * SF, m.VertexPositions[i].Y * SF, m.VertexPositions[i].Z * SF);
                        if (ret == 1) counter++;
                    }
                }
                else if (eNumberofPts > m.VertexPositions.Count()) // remove Points
                {
                    for (int i = 0; i < eNumberofPts; i++)
                    {
                        if (i < m.VertexPositions.Count())
                        {
                            ret = Model.EditPoint.ChangeCoordinates_1(ePtNames[i], m.VertexPositions[i].X * SF, m.VertexPositions[i].Y * SF, m.VertexPositions[i].Z * SF);
                            if (ret == 1) counter++;
                        }
                        else
                        {
                            ret = Model.SelectObj.ClearSelection();
                            ret = Model.AreaObj.SetSelected(Id, true);
                            ret = Model.PointObj.SetSelected(ePtNames[i], true);
                            ret = Model.EditArea.PointRemove();
                            if (ret == 1) counter++;
                        }
                    }
                }
                else if (eNumberofPts < m.VertexPositions.Count()) // add points
                {
                    for (int i = 0; i < m.VertexPositions.Count(); i++)
                    {
                        if (i < eNumberofPts)
                        {
                            ret = Model.EditPoint.ChangeCoordinates_1(ePtNames[i], m.VertexPositions[i].X * SF, m.VertexPositions[i].Y * SF, m.VertexPositions[i].Z * SF);
                            if (ret == 1) counter++;
                        }
                        else
                        {
                            // add point to latest edge
                            ret = Model.SelectObj.ClearSelection();
                            int a = i - 1;
                            ret = Model.AreaObj.SetSelectedEdge(Id, a, true);

                            ret = Model.EditArea.PointAdd();

                            // the repeat the first step so # of name and has updated
                            int tempnumb = 0;
                            string[] TempPtNames = null;
                            ret = Model.AreaObj.GetPoints(Id, ref tempnumb, ref TempPtNames);
                            ret = Model.EditPoint.ChangeCoordinates_1(TempPtNames[i], m.VertexPositions[i].X * SF, m.VertexPositions[i].Y * SF, m.VertexPositions[i].Z * SF);
                            if (ret == 1) counter++;
                        }
                    }

                }

            }

            if (counter > 0) error = string.Format("Error creating Mesh{0}", Id);
        }
        public static void CreateorUpdateArea(ref cSapModel Model, Surface s, ref string Id, bool update, double SF, ref string error)
        {
            Curve[] PerimeterCrvs = s.PerimeterCurves();
            List<Point> SurfPoints = new List<Point>();
            long ret = 0;
            int counter = 0;
            foreach (var crv in PerimeterCrvs)
            {
                SurfPoints.Add(crv.StartPoint);
            }

            if (!update) // Create new
            {
                List<string> ProfilePts = new List<string>();
                foreach (var v in SurfPoints)
                {
                    string dummy = null;
                    ret = Model.PointObj.AddCartesian(v.X * SF, v.Y * SF, v.Z * SF, ref dummy);
                    ProfilePts.Add(dummy);
                }

                string[] names = ProfilePts.ToArray();
                string dummyarea = string.Empty;
                ret = Model.AreaObj.AddByPoint(ProfilePts.Count(), ref names, ref dummyarea);
                if (ret == 1) counter++;
                Id = dummyarea;
            }
            else
            {  // TODO: Update Shell

               
                // Existing
                int eNumberofPts = 0;
                string[] ePtNames = null;
                ret = Model.AreaObj.GetPoints(Id, ref eNumberofPts, ref ePtNames);

                // Compare the number of points
                if (eNumberofPts == SurfPoints.Count())
                {
                    for (int i = 0; i < eNumberofPts; i++)
                    {
                        ret = Model.EditPoint.ChangeCoordinates_1(ePtNames[i], SurfPoints[i].X * SF, SurfPoints[i].Y * SF, SurfPoints[i].Z * SF);
                        if (ret == 1) counter++;
                    }
                }
                else if (eNumberofPts > SurfPoints.Count()) // remove Points
                {
                    for (int i = 0; i < eNumberofPts; i++)
                    {
                        if (i < SurfPoints.Count())
                        {
                            ret = Model.EditPoint.ChangeCoordinates_1(ePtNames[i], SurfPoints[i].X * SF, SurfPoints[i].Y * SF, SurfPoints[i].Z * SF);
                        }
                        else
                        {
                            ret = Model.SelectObj.ClearSelection();
                            ret = Model.AreaObj.SetSelected(Id, true);
                            ret = Model.PointObj.SetSelected(ePtNames[i], true);
                            ret = Model.EditArea.PointRemove();
                        }
                    }
                    if (ret == 1) counter++;
                }
                else if (eNumberofPts < SurfPoints.Count()) // add points
                {
                    for (int i = 0; i < SurfPoints.Count(); i++)
                    {
                        if (i < eNumberofPts)
                        {
                            ret = Model.EditPoint.ChangeCoordinates_1(ePtNames[i], SurfPoints[i].X * SF, SurfPoints[i].Y * SF, SurfPoints[i].Z * SF);
                        }
                        else
                        {
                            // add point to latest edge
                            ret = Model.SelectObj.ClearSelection();
                            int a = i - 1;
                            ret = Model.AreaObj.SetSelectedEdge(Id, a, true);

                            ret = Model.EditArea.PointAdd();

                            // the repeat the first step so # of name and has updated
                            int tempnumb = 0;
                            string[] TempPtNames = null;
                            ret = Model.AreaObj.GetPoints(Id, ref tempnumb, ref TempPtNames);


                            ret = Model.EditPoint.ChangeCoordinates_1(TempPtNames[i], SurfPoints[i].X * SF, SurfPoints[i].Y * SF, SurfPoints[i].Z * SF);
                        }
                    }
                    if (ret == 1) counter++;

                }

            }
            if (counter > 0) error = string.Format("Error creating Mesh{0}", Id);
        }

        public static void CreateorUpdateJoint(ref cSapModel Model, Point pt, ref string Id, bool update, double SF)
        {
            if (!update) // create new Joint
            {
                string dummy = string.Empty;
                long ret = Model.PointObj.AddCartesian(pt.X * SF, pt.Y * SF, pt.Z * SF, ref dummy);
                Id = dummy;
            }
            else
            {
                long ret = Model.EditPoint.ChangeCoordinates_1(Id, pt.X * SF, pt.Y * SF, pt.Z * SF);
            }
        }

        public static bool ChangeNameSAPFrm(ref cSapModel Model, string Name, string NewName)
        {
            long ret = Model.FrameObj.ChangeName(Name, NewName);
            if (ret == 0) { return true; } else { return false; }
        }

        public static bool ChangeNameSAPArea(ref cSapModel Model, string Name, string NewName)
        {
            long ret = Model.AreaObj.ChangeName(Name, NewName);
            if (ret == 0) { return true; } else { return false; }
        }

        public static bool ChangeNameSAPJoint(ref cSapModel Model, string Name, string NewName)
        {
            long ret = Model.PointObj.ChangeName(Name, NewName);
            if (ret == 0) { return true; } else { return false; }
        }

        //Check if Section exists
        public static bool IsSectionExistsFrm(ref cSapModel mySapModel, string DSection, ref string error)
        {
            int number = 0;
            string[] SectionNames = null;
            long ret = mySapModel.PropFrame.GetNameList(ref number, ref SectionNames);
            if (ret == 1) error = "Error getting the  section property names";
            if (SectionNames.Contains(DSection))
            {
                return true;
            }
            return false;
        }

        public static void ImportPropFrm(ref cSapModel mySapModel, string SectionName, string MatProp, string SecCatalog, ref string error)
        {

            long ret = mySapModel.PropFrame.ImportProp(SectionName, MatProp, SecCatalog, SectionName);
            if (ret == 1) error = string.Format("Error importing the section property {0}",SectionName);
        }

        public static void SetSectionFrm(ref cSapModel mySapModel, string Name, string SectionProfile, ref string error)
        {
            long ret = mySapModel.FrameObj.SetSection(Name, SectionProfile);
            if (ret == 1) error = string.Format("Error setting section {0}",Name);
        }

        // Area  prop
        public static void SetPropArea(ref cSapModel Model, string PropName, string ShellType, bool DOF, string MatProp, double MatAngle, double Thickness, double Bending, ref string error)
        {
            int type = (int)((ShellType)Enum.Parse(typeof(ShellType), ShellType));
            long ret = Model.PropArea.SetShell_1(PropName, type, DOF, MatProp, MatAngle, Thickness, Bending);
            if (ret == 1) error = string.Format("Error setting the area property {0}", PropName);
        }
        public static void SetShellPropArea(ref cSapModel Model, string AreaId, string PropName, ref string error)
        {
            long ret = Model.AreaObj.SetProperty(AreaId, PropName);
            if (ret == 1) error = string.Format("Error setting the area property of shell {0}", AreaId);
        }

        // READ FROM SAPMODEL

        // to extract the Section Names on Specific Section Catalog
        public static void GetSectionsfromCatalog(ref cSapModel Model, string SC, ref string[] Names)
        {
            int number = 0;
            eFramePropType[] PropType = null;
            long ret = Model.PropFrame.GetPropFileNameList(SC, ref number, ref Names, ref PropType);
        }

        public static void GetLoadPatterns(ref cSapModel Model, ref string[] LoadPatternNames, ref string[] LoadPatternTypes, ref double[] LoadPatternMultipliers)
        {
            int number = 0;
            int ret = Model.LoadPatterns.GetNameList(ref number, ref LoadPatternNames);

            LoadPatternMultipliers = new double[number];
            LoadPatternTypes = new string[number];

            foreach (string lpname in LoadPatternNames)
            {
                double mult = 0;
                eLoadPatternType type = eLoadPatternType.LTYPE_DEAD;
                int pos = Array.IndexOf(LoadPatternNames, lpname);
                Model.LoadPatterns.GetLoadType(lpname, ref type);

                ret = Model.LoadPatterns.GetSelfWTMultiplier(lpname, ref mult);
                LoadPatternMultipliers[pos] = mult;
                //int typeInt = (int)type;
                LoadPatternTypes[pos] = type.ToString();
            }

        }

        public static void GetLoadCases(ref cSapModel Model, ref string[] LoadCaseNames, ref double[] LoadCaseMultipliers, ref string[] LoadCaseTypes)
        {

            int NumberNames = 0;
            int ret = Model.LoadCases.GetNameList(ref NumberNames, ref LoadCaseNames);

            LoadCaseMultipliers = new double[NumberNames];
            LoadCaseTypes = new string[NumberNames];


            foreach (string lcname in LoadCaseNames)
            {

                //Parameters that we need to get
                //dummy eLoadCaseType
                eLoadCaseType cType = eLoadCaseType.CASE_LINEAR_STATIC;
                int subType = 0;

                int pos = Array.IndexOf(LoadCaseNames, lcname);

                //get the load case type
                ret = Model.LoadCases.GetType(lcname, ref cType, ref subType);
                LoadCaseTypes[pos] = cType.ToString();

            }
        }

        public static void GetLoadCombos(ref cSapModel Model, ref string[] LoadComboNames, ref string[][] LoadComboTypes, ref string[][] LoadComboCase, ref double[][] Multipliers, ref string[][] LoadComboDefinitions)
        {
            int NumberNames = 0;
            int ret = Model.RespCombo.GetNameList(ref NumberNames, ref LoadComboNames);

            LoadComboTypes=new string[NumberNames][];
            LoadComboCase = new string[NumberNames][];
            Multipliers = new double[NumberNames][];
            LoadComboDefinitions = new string[NumberNames][];

            if (LoadComboNames != null)
            {
                foreach (string lc in LoadComboNames)
                {
                    int pos = Array.IndexOf(LoadComboNames, lc);


                    int numberItems = 0;
                    eCType[] cType = null;
                    string[] cName = null;
                    double[] sf = null;
                    ret = Model.RespCombo.GetCaseList(lc, ref numberItems, ref cType, ref cName, ref sf);

                    LoadComboTypes[pos] = new string[numberItems];
                    LoadComboCase[pos] = new string[numberItems];
                    Multipliers[pos] = new double[numberItems];
                    LoadComboDefinitions[pos] = new string[numberItems];
                    foreach (string cn in cName)
                    {
                        int pos2 = Array.IndexOf(cName, cn);
                        try
                        {
                            LoadComboTypes[pos][pos2] = cType[pos2].ToString();
                            LoadComboCase[pos][pos2] = cn;
                            Multipliers[pos][pos2] = sf[pos2];
                            LoadComboDefinitions[pos][pos2] = cName[pos2];

                        }
                        catch (Exception ex)
                        {

                            string t = ex.Message;
                        }
                    }
                } 
            }
        }

        public static void GetFrm(ref cSapModel Model, string frmId, ref Point i, ref Point j, ref string MatProp, ref string SecName, ref string Just, ref double Rot, ref string SecCatalog, double LSF) //Length Scale Factor
        {

            long ret = 0;
            // Get Geometry
            // SAP Frame start and end point
            string StartPoint = string.Empty;
            string EndPoint = string.Empty;
            Double myStartX = 0;
            Double myStartY = 0;
            Double myStartZ = 0;
            Double myEndX = 0;
            Double myEndY = 0;
            Double myEndZ = 0;

            // getting start and end point
            ret = Model.FrameObj.GetPoints(frmId, ref StartPoint, ref EndPoint);
            //getting coordinates of starting point
            ret = Model.PointObj.GetCoordCartesian(StartPoint, ref myStartX, ref myStartY, ref myStartZ);
            //getting coordinates of ending point
            ret = Model.PointObj.GetCoordCartesian(EndPoint, ref myEndX, ref myEndY, ref myEndZ);

            i = Point.ByCoordinates(myStartX * LSF, myStartY * LSF, myStartZ * LSF);
            j = Point.ByCoordinates(myEndX * LSF, myEndY * LSF, myEndZ * LSF);

            // Section
            string SAuto = string.Empty;
            ret = Model.FrameObj.GetSection(frmId, ref SecName, ref SAuto);

            // MatProp
            MaterialMapper.SapToDynamo(ref Model, SecName, ref MatProp, ref SecCatalog);

            // Justification
            Just = JustificationMapper.SapToDynamoFrm(ref Model, frmId);

            // Rotation
            bool ifadvanced = false;
            ret = Model.FrameObj.GetLocalAxes(frmId, ref Rot, ref ifadvanced);

        }

        public static void GetShell(ref cSapModel Model, string areaid, ref Surface BaseS, double LSF, ref string PropName) //Length Scale Factor
        {
            long ret = 0;

            int NumOfPts = 0;
            string[] PtsNames = null;
            ret = Model.AreaObj.GetPoints(areaid, ref NumOfPts, ref PtsNames);

            List<Point> dynPts = new List<Point>();
            List<IndexGroup> igs = new List<IndexGroup>();
            for (int i = 0; i < PtsNames.Count(); i++)
            {
                double x = 0;
                double y = 0;
                double z = 0;

                ret = Model.PointObj.GetCoordCartesian(PtsNames[i], ref x, ref y, ref z);

                Point p = Point.ByCoordinates(x * LSF, y * LSF, z * LSF);
                dynPts.Add(p);
            }

            //Mesh.ByPointsFaceIndices()
            BaseS = Surface.ByPerimeterPoints(dynPts);

            // Get assigned Property Name
            ret = Model.AreaObj.GetProperty(areaid, ref PropName);
        }

        /// <summary>
        /// Harvesting the active SAP and creates dictionardy holds FramesGUID and Labels
        /// </summary>
        /// <param name="Model">Active SAP Model</param>
        /// <param name="myFrameList"> List of Labels of SAP Frames </param>
        public static void GetSAPFrameList(ref cSapModel Model, ref List<string> myFrameList) // <GUID, Label> 
        {
            string[] IDs = null;
            int NumbOfFrames = 0;
            long ret = Model.FrameObj.GetNameList(ref NumbOfFrames, ref IDs);
            if (IDs != null)
            {
                myFrameList = IDs.ToList();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Model"></param>
        /// <param name="Label"></param>
        public static void DeleteFrm(ref cSapModel Model, string Label)
        {
            long ret = Model.FrameObj.Delete(Label);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Model"></param>
        /// <param name="Label"></param>
        public static void DeleteArea(ref cSapModel Model, string Label, ref string error)
        {
            long ret = Model.AreaObj.Delete(Label);
            if (ret == 1) error = string.Format("Error deleting shell {0}", Label);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Model"></param>
        /// <param name="Label"></param>
        public static void DeleteJoint(ref cSapModel Model, string Label, ref string error)
        {
            long ret = Model.PointObj.DeleteSpecialPoint(Label);
            if (ret == 1) error = string.Format("Error deleting joint {0}", Label);
        }

        public static void GetSAPAreaList(ref cSapModel Model, ref List<string> myAreaList)
        {
            string[] IDs = null;
            int NumbOfAreas = 0;
            long ret = Model.AreaObj.GetNameList(ref NumbOfAreas, ref IDs);
            if (IDs != null)
            {
                myAreaList = IDs.ToList();
            }

        }

        public static void GetSAPJointList(ref cSapModel Model, ref List<string> myJointList)
        {
            string[] IDs = null;
            int NumbOfAreas = 0;
            long ret = Model.PointObj.GetNameList(ref NumbOfAreas, ref IDs);
            if (IDs != null)
            {
                myJointList = IDs.ToList();
            }
        }

        public static void DeleteUnconnectedPts(ref cSapModel Model)
        {
            // get name list
            List<string> PtList = new List<string>();
            GetSAPJointList(ref Model, ref PtList);
            foreach (var pt in PtList)
            {
                int Num = 0;
                int[] Otype = null;
                string[] OName = null;
                int[] refNum = null;
                Model.PointObj.GetConnectivity(pt, ref Num, ref Otype, ref OName, ref refNum);
                if (Num == 0)
                { // delete if not connected
                    int ret = Model.PointObj.SetSpecialPoint(pt, false);
                    ret = Model.EditPoint.ChangeCoordinates_1(pt, 0, 0, 0);  // this will merge them into one location at least !
                    ret = Model.PointObj.DeleteSpecialPoint(pt);
                }
            }

        }
        public static void GetShellProp(ref cSapModel Model, string PropName, ref string ShellType, ref bool DOF, ref string MatProp, ref double MatAngle, ref double Thickness, ref double Bending)
        {
            int type = 1;
            int color = 1;
            string notes = string.Empty;
            string guid = string.Empty;

            long ret = Model.PropArea.GetShell_1(PropName, ref type, ref DOF, ref MatProp, ref MatAngle, ref Thickness, ref Bending, ref color, ref notes, ref guid);

            ShellType = Enum.GetName(typeof(ShellType), type);

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Model"></param>
        public static void RefreshView(ref cSapModel Model)
        {
            Model.View.RefreshView(0, false);
        }

    }
}
