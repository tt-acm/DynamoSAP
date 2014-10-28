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
    public class RestraintMapper
    {
        public static void SetRestaints(ref cSapModel Model, Point Pt, bool[] restaints)
        { 
            //Get Points
            int num = 0;
            string[] PtIds = null;
            int ret = Model.PointObj.GetNameList(ref num, ref PtIds);

            for (int i = 0; i < num; i++)
            {
                double x = 0; double y = 0; double z = 0;
                ret = Model.PointObj.GetCoordCartesian(PtIds[i], ref x, ref y, ref z);

                if (Pt.X == Math.Round(x,3) && Pt.Y == Math.Round(y,3) && Pt.Z == Math.Round(z,3)) //TODO: Math Round per linear length???
                {
                    ret = Model.PointObj.SetRestraint(PtIds[i], restaints);
                    break;
                }
            }
        }
    }
}
