using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//DYNAMO
using Autodesk.DesignScript.Runtime;
//SAP
using SAP2000v16;

namespace DynamoSAP.Structure
{
    [IsVisibleInDynamoLibrary(false)]
    public static class Utilities
    {

        public static string MaterialMapper_DyanmoToSap(string DMat)
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

        // PFUFFF: GETTING TYPE EXCEPTION ASK DYNAMO TEAM ????

        //internal static bool ProfileMapper_IsSectionExists(string DSection, ref cSapModel mySapModel)
        //{
        //    int number = 0;
        //    string[] SectionNames = null;
        //    long ret = mySapModel.PropFrame.GetNameList(ref number, ref SectionNames);

        //    if (SectionNames.Contains(DSection))
        //    {
        //        return true;
        //    }
        //    return false;
        //}

        //public static bool Justification_DynamoToSAP(ref cSapModel mySapModel, int Just, string FrmId)
        //{
        //    //int cardinalPoint = 0; // use Just
        //    double[] offset1 = new double[3];
        //    double[] offset2 = new double[3];

        //    offset1[1] = 0;
        //    offset2[1] = 0;

        //    offset1[2] = 0;
        //    offset2[2] = 0;

        //    //TODO: Mapping Needed  Vertical Justification/ Lateral Justification 1 = bottom left2 = bottom center 3 = bottom right 4 = middle left 5 = middle center 6 = middle right 7 = top left 8 = top center 9 = top right 10 = centroid 11 = shear center

        //    long ret = mySapModel.FrameObj.SetInsertionPoint(FrmId, Just, false, true, ref offset1, ref offset2);

        //    return true;
        //}
    }
}
