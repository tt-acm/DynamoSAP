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
    public class StructureMapper
    {
        // Draw Frame Object return ID
        public static void DrawFrm(ref cSapModel Model, double iX, double iY, double iZ, double jX, double jY, double jZ, ref string Id)
        {
            //1. Create Frame
            long ret = Model.FrameObj.AddByCoord(iX, iY, iZ, jX, jY, jZ, ref Id);
        }

        public static void SetGUIDFrm(ref cSapModel Model, string Label, string GUID)
        {
            long ret = Model.FrameObj.SetGUID(Label, GUID);
        }

        //Check if Section exists
        public static bool IsSectionExistsFrm(ref cSapModel mySapModel, string DSection)
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

        public static void ImportPropFrm(ref cSapModel mySapModel, string SectionName, string MatProp, string SecCatalog)
        {
            long ret = mySapModel.PropFrame.ImportProp(SectionName, MatProp, SecCatalog, SectionName);
        }

        public static void SetSectionFrm(ref cSapModel mySapModel, string Name, string SectionProfile)
        {
            long ret = mySapModel.FrameObj.SetSection(Name, SectionProfile);
        }

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
                eLoadPatternType type=eLoadPatternType.LTYPE_DEAD;
                int pos = Array.IndexOf(LoadPatternNames,lpname);
                Model.LoadPatterns.GetLoadType(lpname, ref type);

                ret = Model.LoadPatterns.GetSelfWTMultiplier(lpname,ref mult);
                LoadPatternMultipliers[pos] = mult;
                //int typeInt = (int)type;
                LoadPatternTypes[pos] = type.ToString();
            }

        }

        public static void GetPointLoads(ref cSapModel Model, ref string[] FrameName, ref int NumberItems, ref string[] LoadPat, ref int[] MyType, ref string[] CSys, ref int[] Dir, ref double[] RelDist, ref double[] Dist, ref double[] Val){

            int ret = Model.FrameObj.GetLoadPoint("ALL", NumberItems, FrameName, LoadPat, MyType, CSys, Dir, RelDist, Dist, Val, SAP2000v16.eItemType.Group);
        }

        public static void GetDistributedLoads(ref cSapModel Model, ref string[] FrameName, ref int NumberItems, ref string[] LoadPat, ref int[] MyType, ref string[] CSys, ref int[] Dir, ref double[] RD1, ref double[] RD2, ref double[] Dist1, ref double[] Dist2, ref double[] Val1, ref double[] Val2)
        {
            int ret = 0;
            ret = Model.FrameObj.GetLoadDistributed("ALL", ref NumberItems, ref FrameName, ref LoadPat, ref MyType, ref CSys, ref Dir, ref RD1, ref RD2, ref Dist1, ref Dist2, ref Val1, ref Val2, SAP2000v16.eItemType.Group);
        }


        // READ FROM SAPMODEL
        public static void GetFrameIds(ref string[] Names, ref cSapModel Model)
        {
            int number = 0;
            int ret = Model.FrameObj.GetNameList(ref number, ref Names);
        }

        public static void GetFrm(ref cSapModel Model, string frmId, ref Point i, ref Point j, ref string MatProp, ref string SecName, ref string Just, ref double Rot, ref string SecCatalog)
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

            i = Point.ByCoordinates(myStartX, myStartY, myStartZ);
            j = Point.ByCoordinates(myEndX, myEndY, myEndZ);

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

        public static void GetGUIDFrm(ref cSapModel Model, string Label, ref string GUID)
        {
            long ret = Model.FrameObj.GetGUID(Label, ref GUID);
            if (String.IsNullOrEmpty(GUID))
            {
                ret = Model.FrameObj.SetGUID(Label);
                ret = Model.FrameObj.GetGUID(Label, ref GUID);
            }
        }
    }
}
