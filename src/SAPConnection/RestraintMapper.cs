/// Developed by Thornton Tomasetti's CORE Studio for Autodesk
/// http://core.thorntontomasetti.com
/// CORE Developers: Elcin Ertugrul and Ana Garcia Puyol

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
        // Dynamo To SAP
        public static void Set(ref cSapModel Model, string Id, bool[] restaints)
        { 
           long ret = Model.PointObj.SetRestraint(Id, restaints);
        }

        // SAP to Dynamo
        public static void Get( ref cSapModel Model, string PtId, ref Point Pt, ref bool[] restraints, double SF)
        {
            // Get restraints
            int ret = Model.PointObj.GetRestraint(PtId, ref restraints);
            double x= 0; double y= 0; double z= 0;
            
            // Get Point
            ret = Model.PointObj.GetCoordCartesian(PtId, ref x, ref y, ref z);
            Pt = Point.ByCoordinates(x*SF, y*SF, z*SF);

        }

        // Get Points has restraints Assigned 
        public static void GetSupportedPts(ref cSapModel Model, ref List<string> PtIds)
        {
            Model.SelectObj.ClearSelection();
            List<bool> dof = new List<bool>();
            for (int i = 0; i < 6; i++)
            {
                dof.Add(true);
            }
            int ret = Model.SelectObj.SupportedPoints(dof.ToArray(), "GLOBAL", false, true, false, false, false, false, false); // Select the Points objects

            // Get selection
            int num = 0;
            int[] types = null;
            string[] Names = null;
            Model.SelectObj.GetSelected(ref num, ref types, ref Names);

            // Type 1 = Point, 2 = Frame, 3 = Cable, 4= Tendon, 5 = Area, 6 = Solid, 7 = Link
            for (int i = 0; i < num; i++)
            {
                if (types[i] == 1) PtIds.Add(Names[i]);
            }

            Model.SelectObj.ClearSelection();

        }

        public static int Count(ref cSapModel Model)
        {
            int count = Model.PointObj.CountRestraint();
            return count;
        }

    }
}
