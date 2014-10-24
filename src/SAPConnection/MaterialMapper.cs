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

    }
}
