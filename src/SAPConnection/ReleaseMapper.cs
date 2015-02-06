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
    public class ReleaseMapper
    {
        public static void Set(ref cSapModel Model, string name, bool[] ireleases, bool[] jreleases, ref string error)
        {
            //not sure how this works or if this approach even makes sense...
            
            double[] StartPFixityValues = new double[6];
            double[] EndPFixityVValues = new double[6];

            
            int ret = Model.FrameObj.SetReleases(name, ireleases, jreleases, StartPFixityValues, EndPFixityVValues,eItemType.Object);

            if (ret == 1) error=string.Format("Error setting the release for frame {0}. Try changing the conditions",name);

        }

        public static void Get(ref cSapModel Model, string name, ref bool[] ireleases, ref bool[] jreleases)
        {
            double[] StartPFixityValues = new double[6];
            double[] EndPFixityVValues = new double[6];

            int ret = Model.FrameObj.GetReleases(name, ref ireleases, ref jreleases, ref StartPFixityValues, ref EndPFixityVValues);
        }
    }
}
