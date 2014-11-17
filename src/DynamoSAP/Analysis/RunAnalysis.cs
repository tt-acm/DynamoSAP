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

        public static StructuralModel RunAnalysis(StructuralModel Model, string Filepath, bool Run)
        {
          
            if (Run)
            { // if the boolean is set to true                 

                // open sap     
                SAPConnection.Initialize.OpenSAPModel(Filepath, ref mySapModel);

                // run analysis
                SAPConnection.AnalysisMapper.RunAnalysis(ref mySapModel, Filepath);
               
            }
            return Model;
                       
        }

        public static string GetResults(StructuralModel Model, string loadcase, bool Run){
            if (Run) { 
            // loop over frames get results and populate to dictionary
            List<FrameResults> frameResults = null; 

            frameResults = SAPConnection.AnalysisMapper.GetFrameForces(ref mySapModel, loadcase);

            Results structureResult = new Results(frameResults);

            return "Results"; //return results

             }
            else
            {
                return "Run set to False";
            }
        }

        private Results() { }
        private Results(List<FrameResults> fresults)
        {
            FrameResults = fresults;
        }
    }
}
