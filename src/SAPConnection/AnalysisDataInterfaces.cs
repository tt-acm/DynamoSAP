using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;

namespace DynamoSAP
{
   
        [SupressImportIntoVM]
        public interface IFrameAnalysisData
        {
            double P { get; }
            double V2 { get; }
            double V3 { get; }
            double T { get; }
            double M2 { get; }
            double M3 { get; }
        }

        [SupressImportIntoVM]
        public interface IFrameResults
        {
            string ID { get; }
            Dictionary<string, Dictionary<double, FrameAnalysisData>> Results { get; }  // string = LoadCase Name, int station range 0-1

        }

        [SupressImportIntoVM]
        public interface IResults
        {
            List<FrameResults> FrameResults { get; }
            // ... other result types here
        }
    
}
