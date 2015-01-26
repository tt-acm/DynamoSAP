/// Developed by Thornton Tomasetti's CORE Studio for Autodesk
/// http://core.thorntontomasetti.com
/// CORE Developers: Elcin Ertugrul and Ana Garcia Puyol

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//DYNAMO
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;

namespace DynamoSAP
{
    [SupressImportIntoVM]
    public class FrameAnalysisData:IFrameAnalysisData
    {
        public double P { get; set; }

        public double V2{ get; set; }

        public double V3 { get; set; }

        public double T { get; set; }

        public double M2 { get; set; }

        public double M3 { get; set; }

        public FrameAnalysisData(double p, double v2, double v3, double t, double m2, double m3)
        {
            P = p;
            V2 = v2;
            V3 = v2;
            T = t;
            M2 = m2;
            M3 = m3;
        }
    }

    [SupressImportIntoVM]
    public class FrameResults : IFrameResults
    {
        public string ID { get; set; }
        public Dictionary<string, Dictionary<double, FrameAnalysisData>> Results { get; set; } // string LC, and analysis data

        public FrameResults (string id, Dictionary<string, Dictionary<double, FrameAnalysisData>> results)
        {
            ID = id;
            Results = results;
        }
    }
    
}
