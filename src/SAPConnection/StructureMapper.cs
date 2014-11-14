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
            long ret = Model.FrameObj.AddByCoord(iX,iY,iZ,jX,jY,jZ,ref Id);
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

        public static void DefinePropFrm(ref cSapModel mySapModel, string SectionName, string MatProp, string SecCatalog, string SectionProfile)
        {
           long ret= mySapModel.PropFrame.ImportProp(SectionName, MatProp, SecCatalog, SectionProfile);
        }

        public static void SetSectionFrm(ref cSapModel mySapModel, string Name, string SectionProfile)
        {
            long ret= mySapModel.FrameObj.SetSection(Name, SectionProfile);
        }

        // READ FROM SAPMODEL
        public static void GetFrameIds(ref string[] Names, ref cSapModel Model)
        {
            int number = 0;
            int ret = Model.FrameObj.GetNameList(ref number, ref Names);
        }

        public static void GetFrm(ref cSapModel Model, string frmId, ref Point i, ref Point j, ref string MatProp, ref string SecProp, ref string Just, ref double Rot)
        { 
            
            long ret = 0;
            //Get Geometry
            // SAP Frame start and end point
            string StartPoint = string.Empty;
            string EndPoint = string.Empty;
            Double myStartX = 0;
            Double myStartY = 0;
            Double myStartZ = 0;
            Double myEndX = 0;
            Double myEndY = 0;
            Double myEndZ = 0;

            //getting start and end point
            ret = Model.FrameObj.GetPoints(frmId, ref StartPoint, ref EndPoint);
            //getting coordinates of starting point
            ret = Model.PointObj.GetCoordCartesian(StartPoint, ref myStartX, ref myStartY, ref myStartZ);
            //getting coordinates of ending point
            ret = Model.PointObj.GetCoordCartesian(EndPoint, ref myEndX, ref myEndY, ref myEndZ);

            i = Point.ByCoordinates(myStartX, myStartY, myStartZ);
            j = Point.ByCoordinates(myEndX, myEndY, myEndZ);
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
