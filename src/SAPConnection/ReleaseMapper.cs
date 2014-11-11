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
        public static void SetReleases(ref cSapModel Model, string name, bool[] ireleases, bool[] jreleases)
        {
            //not sure how this works or if this approach even makes sense...
            
            double[] StartValue=new double[6];
            double[] EndValue = new double[6];
            
            for (int i = 0; i < 6; i++)
            {
                if (ireleases[i] == true) { 
                    StartValue[i] = 0;
                }
                if (jreleases[i] == true)
                {
                    EndValue[i] = 0;
                }
                
            }
            
            int ret = Model.FrameObj.SetReleases (name, ireleases, jreleases,StartValue, EndValue);

        }
    }
}
