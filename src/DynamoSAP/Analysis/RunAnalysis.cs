using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamoSAP.Analysis
{
    public class Analysis : IResults
    {
        public List<FrameResults> FrameResults { get; set; }

        public static string Run (string filepath, bool Run) //return AnalysisResults
        { 
            // open sap
            // run analysis
            // loop over frames get results
            // populate the to dicionary

            return "results";
        }


        private Analysis(List<FrameResults> fresults)
        {
            FrameResults = fresults;
        }
    }
}
