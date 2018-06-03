/// Developed by Thornton Tomasetti's CORE Studio for Autodesk
/// http://core.thorntontomasetti.com
/// CORE Developers: Elcin Ertugrul and Ana Garcia Puyol

using System;
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
    public class JustificationMapper
    {
        public static bool DynamoToSAPFrm(ref cSapModel Model, string Label , string Just, ref string error)
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
            else if (Just == "TopCenter")
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
            if (ret == 1) error = string.Format("Error setting the justification of frame {0}", Label);
            return true;
        }

        public static void SetRotationFrm(ref cSapModel Model, string Label, double Angle, ref string error)
        {
            int  ret = Model.FrameObj.SetLocalAxes(Label, Angle);
            if (ret == 1) error = string.Format("Error setting the rotation of frame {0}", Label);
        }

        public static string SapToDynamoFrm(ref cSapModel Model, string frmId)
        { 
            int cardinalPoint = 0;
            bool isMirror2 = false;
            bool isMirror3 = false;
            bool isStiffTransform = true;
            double[] offset1 = null;
            double[] offset2 = null;
            string CSys = string.Empty;

            int ret = Model.FrameObj.GetInsertionPoint_1(frmId, ref cardinalPoint, ref isMirror2, ref isMirror3, ref isStiffTransform, ref offset1, ref offset2, ref CSys);

            if (cardinalPoint == 1) // bottom left
            {
                return "BottomLeft";
            }
            else if (cardinalPoint == 2) //bottom center
            {
                return "BottomCenter";
            }
            else if (cardinalPoint == 3) //bottom right
            {
                return "BottomRight";
            }
            else if (cardinalPoint == 4) //middle left
            {
                return "MiddleLeft";
            }
            else if (cardinalPoint == 5) //middle center
            {
                return "MiddleCenter";
            }
            else if (cardinalPoint == 6) //middle right
            {
                return "MiddleRight";
            }
            else if (cardinalPoint == 7) //top left
            {
                return "TopLeft";
            }
            else if (cardinalPoint == 8) //top center
            {
                return "TopCenter";
            }
            else if (cardinalPoint == 9) //top right
            {
                return "TopRight";
            }
            else if (cardinalPoint == 10) //centroid
            {
                return "Centroid";
            }
            else if (cardinalPoint == 11) //shearcenter
            {
                return "ShearCenter";
            }
            else
            {
                return "MiddleCenter";
            }
        
        }


    }
}
