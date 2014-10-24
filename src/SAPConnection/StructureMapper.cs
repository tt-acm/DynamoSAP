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

    }
}
