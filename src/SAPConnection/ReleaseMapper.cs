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
    class ReleaseMapper
    {
        public static void SetReleases(ref cSapModel Model, string name, bool[] iireleases, bool[] jjreleases)
        {
            //not sure how this works or if this approach even makes sense...
            
            double[] StartValue=new double[5];
            double[] EndValue = new double[5];
            
            for (int i = 0; i < 5; i++)
            {
                if (iireleases[i] == true) { 
                    StartValue[i] = 0;
                }
                if (jjreleases[i] == true)
                {
                    EndValue[i] = 0;
                }
                
            }
            
            int ret = Model.FrameObj.SetReleases (name, iireleases, jjreleases,StartValue, EndValue);

        }
    }
}
