using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//DYNAMO
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;

//SAP 
using SAP2000v16;
using SAPConnection;
using DynamoSAP.Assembly;
using DynamoSAP.Structure;

namespace DynamoSAP.Analysis
{
    public class Results : IResults
    {
        public List<FrameResults> FrameResults { get; set; }
        private static cSapModel mySapModel;

        //public static Dictionary<string, FrameForces> Run(string filepath, string loadcase, bool runIt)
        public static FrameForces Run(string filepath, string loadcase, bool runIt)
        {
            Dictionary<string, FrameForces> myFrameForces = new Dictionary<string, FrameForces>();
           // if (runIt)
           // { // if the boolean is set to true                 
                SAP2000v16.SapObject mySapObject = null;
                // open sap     
                SAPConnection.Initialize.OpenModel(ref mySapObject, ref mySapModel, filepath, runIt);

                // run analysis
                SAPConnection.AnalysisMapper.RunAnalysis(ref mySapModel, filepath, runIt);

                // loop over frames get results and populate to dictionary
                Dictionary<string, string> minMaxString = new Dictionary<string, string>();
                List<double> forceValues = new List<double>();
                
            myFrameForces = SAPConnection.AnalysisMapper.GetFrameForces(ref mySapModel, loadcase, ref minMaxString, ref forceValues);

            List<FrameResults> fresults=new List<FrameResults>();

          //      FrameAnalysisData fAnalysisData = new FrameAnalysisData(forceValues[0], forceValues[1], forceValues[2], forceValues[3], forceValues[4], forceValues[5]);
          //  FrameResults fres=new FrameResults(minMaxString.Keys, )

            Results rr = new Results(fresults);
            
                return myFrameForces["1"]; //Return Analysis

           // }

            

        }

        private Results() { }
        private Results(List<FrameResults> fresults)
        {
            FrameResults = fresults;
        }
    }
}
