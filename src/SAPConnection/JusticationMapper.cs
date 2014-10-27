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
    public class JustificationMapper
    {
        public static bool DynamoToSAPFrm(ref cSapModel Model, string Label , string Just)
        {
            //int cardinalPoint = 0; // use Just
            double[] offset1 = new double[3];
            double[] offset2 = new double[3];

            offset1[1] = 0;
            offset2[1] = 0;

            offset1[2] = 0;
            offset2[2] = 0;

            //TODO: Mapping Needed  Vertical Justification/ Lateral Justification 1 = bottom left2 = bottom center 3 = bottom right 4 = middle left 5 = middle center 6 = middle right 7 = top left 8 = top center 9 = top right 10 = centroid 11 = shear center
            int justification = 5; // default middle center
            if (Just == "BottomLeft")
            {
                justification = 1;
            }
            else if (Just == "BottomCenter")
            {
                justification = 2;
            }
            else if (Just == "BottomRight")
            {
                justification = 3;
            }
            else if (Just == "MiddleLeft")
            {
                justification = 4;
            }
            else if (Just == "MiddleCenter")
            {
                justification = 5;
            }
            else if (Just == "MiddleRight")
            {
                justification = 6;
            }
            else if (Just == "TopLeft")
            {
                justification = 7;
            }
            else if (Just == "topcenter")
            {
                justification = 8;
            }
            else if (Just == "TopRight")
            {
                justification = 9;
            }
            else if (Just == "Centroid")
            {
                justification = 10;
            }
            else if (Just == "ShearCenter")
            {
                justification = 11;
            }

            int ret = Model.FrameObj.SetInsertionPoint(Label, justification, false, true, ref offset1, ref offset2);

            return true;
        }

        public static void SetRotationFrm(ref cSapModel Model, string Label, double Angle)
        {
            int  ret = Model.FrameObj.SetLocalAxes(Label, Angle);
        }
    }
}
