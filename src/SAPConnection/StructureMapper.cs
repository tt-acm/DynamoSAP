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
        public static void CreateorUpdateFrm(ref cSapModel Model, double iX, double iY, double iZ, double jX, double jY, double jZ, ref string Id, bool update)
        {
            if (!update)
            {
                //1. Create Frame
                long ret = Model.FrameObj.AddByCoord(iX, iY, iZ, jX, jY, jZ, ref Id);
            }
            else 
            {
                // update location
                string startPoint = string.Empty;
                string endPoint = string.Empty;
                long ret = Model.FrameObj.GetPoints(Id, ref startPoint, ref endPoint);
                ret = Model.EditPoint.ChangeCoordinates(startPoint, iX, iY, iZ);
                ret = Model.EditPoint.ChangeCoordinates(endPoint, jX, jY, jZ);
            }
            
        }


        public static void SetGUIDFrm(ref cSapModel Model, string Label, string GUID)
        {
            long ret = Model.FrameObj.SetGUID(Label, GUID);
        }

        public static bool ChangeNameSAPFrm(ref cSapModel Model, string Name, string NewName)
        {
            long ret = Model.FrameObj.ChangeName(Name, NewName);
            if (ret == 0) { return true; } else { return false; }
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



        // READ FROM SAPMODEL
        public static void GetFrameIds(ref string[] Names, ref cSapModel Model)
        {
            int number = 0;
            int ret = Model.FrameObj.GetNameList(ref number, ref Names);
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

        public static void GetGUIDFrm(ref cSapModel Model, string Label, ref string GUID)
        {
            long ret = Model.FrameObj.GetGUID(Label, ref GUID);
            if (String.IsNullOrEmpty(GUID))
            {
                ret = Model.FrameObj.SetGUID(Label);
                ret = Model.FrameObj.GetGUID(Label, ref GUID);
            }
        }

        /// <summary>
        /// Harvesting the active SAP and creates dictionardy holds FramesGUID and Labels
        /// </summary>
        /// <param name="Model">Active SAP Model</param>
        /// <param name="myFrameList"> List of Labels of SAP Frames </param>
        public static void GetSAPFrameList(ref cSapModel Model, ref List<string> myFrameList) // <GUID, Label> 
        {
            string[] ID = null;
            int NumbOfFrames = 0;
            int ret = Model.FrameObj.GetNameList(ref NumbOfFrames, ref ID);
            if (ID != null)
            {
                myFrameList = ID.ToList();
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
        public static void RefreshView(ref cSapModel Model)
        {
            Model.View.RefreshView(0, false);
        }

    }
}
