/// Developed by Thornton Tomasetti's CORE Studio for Autodesk
/// http://core.thorntontomasetti.com
/// CORE Developers: Elcin Ertugrul and Ana Garcia Puyol

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SAP2000v20;
// interop.COM services for SAP
using System.Runtime.InteropServices;

//DYNAMO
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;

namespace SAPConnection
{
    [SupressImportIntoVM]
    public class MaterialMapper
    {
        public static string DynamoToSap(string DMat)
        {

            if (DMat == null || DMat.ToLower().Contains("steel") || DMat.ToLower().Contains("st"))
            {
                return "A992Fy50";  // default steel;
            }
            else if (DMat == null || DMat.ToLower().Contains("concrete") || DMat.ToLower().Contains("conc"))
            {
                return "4000Psi";
            }

            if (DMat.Contains("A36"))
            {

                return "A36";
            }
            else if (DMat.Contains("A53"))
            {

                return "A53GrB";
            }
            else if (DMat.Contains("A500"))
            {
                if (DMat.Contains("42"))
                {

                    return "A500GrB42";
                }
                else if (DMat.Contains("46"))
                {

                    return "A500GrB46";
                }
            }
            else if (DMat.Contains("A572"))
            {

                return "A572Gr50";
            }
            else if (DMat.Contains("A913"))
            {

                return "A913Gr50";
            }
            else if (DMat.Contains("A992"))
            {

                return "A992Fy50";
            }

            return "A992Fy50"; // Default
        }

        public static void SapToDynamo(ref cSapModel Model, string SecName, ref string MatProp, ref string filename)
        {
            long ret = 0;
            eFramePropType PropType = new eFramePropType();
            ret = Model.PropFrame.GetTypeOAPI(SecName, ref PropType);

            string nameinfile = string.Empty; // If the section property was imported from a property file, this is the name of that file. If the section property was not imported, this item is blank.
            int color = 0;
            string notes = string.Empty;
            string Guid = string.Empty;
            double t2 = 0; double t3 = 0; double TW = 0; double TF = 0; double t2b = 0; double t3b = 0;

            if (PropType == eFramePropType.Rectangular)
            {
                ret = Model.PropFrame.GetRectangle(SecName, ref filename, ref MatProp, ref t3, ref t2, ref color, ref notes, ref Guid);
            }
            else if (PropType == eFramePropType.Pipe)
            {
                ret = Model.PropFrame.GetPipe(SecName, ref filename, ref MatProp, ref t3, ref TW, ref color, ref notes, ref Guid);
            }
            else if (PropType == eFramePropType.Circle)
            {
                ret = Model.PropFrame.GetCircle(SecName, ref filename, ref MatProp, ref t3, ref color, ref notes, ref Guid);
            }
            else if (PropType == eFramePropType.Box)
            {
                ret = Model.PropFrame.GetTube(SecName, ref filename, ref MatProp, ref t3, ref t2, ref TF, ref TW, ref color, ref notes, ref Guid);
            }
            else if (PropType == eFramePropType.I)
            {
                ret = Model.PropFrame.GetISection(SecName, ref filename, ref MatProp, ref t3, ref t2, ref TF, ref TW, ref t2b, ref t3b, ref color, ref notes, ref Guid);
            }
            else if (PropType == eFramePropType.Channel)
            {
                ret = Model.PropFrame.GetChannel(SecName, ref filename, ref MatProp, ref t3, ref t2, ref TF, ref TW, ref color, ref notes, ref Guid);
            }
            else if (PropType == eFramePropType.T)
            {
                ret = Model.PropFrame.GetTee(SecName, ref filename, ref MatProp, ref t3, ref t2, ref TF, ref TW, ref color, ref notes, ref Guid);
            }
            else if (PropType == eFramePropType.Angle)
            {
                ret = Model.PropFrame.GetAngle(SecName, ref filename, ref MatProp, ref t3, ref t2, ref TF, ref TW, ref color, ref notes, ref Guid);
            }
            else
            {
                ret = Model.PropFrame.GetNameInPropFile(SecName, ref nameinfile, ref filename, ref MatProp, ref PropType);
            }

            if (!String.IsNullOrEmpty(filename))
            {
                filename = Path.GetFileNameWithoutExtension(filename);
            }

        }
    }
}
