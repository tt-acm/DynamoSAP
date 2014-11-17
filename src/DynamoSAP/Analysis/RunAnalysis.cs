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
    public class Analysis : IResults
    {
        public List<FrameResults> FrameResults { get; set; }
        private static cSapModel mySapModel;

        public static StructuralModel Run(StructuralModel Model, string Filepath, bool RunIt)
        {
            if (RunIt)
            {
                // open sap     
                SAPConnection.Initialize.OpenSAPModel(Filepath, ref mySapModel);
                // run analysis
                SAPConnection.AnalysisMapper.RunAnalysis(ref mySapModel, Filepath);
            }
            return Model;
        }

        public static Analysis GetResults(StructuralModel Model, string loadcase, bool Run)
        {
            List<FrameResults> frameResults = null;
            Analysis StructureResults = new Analysis();
            if (Run)
            {
                // loop over frames get results and populate to dictionary
                frameResults = SAPConnection.AnalysisMapper.GetFrameForces(ref mySapModel, loadcase);
                StructureResults.FrameResults = frameResults;
            }
            return StructureResults;
        }

        public static List<string> DecomposeResults(Analysis StructureResults, string ForceType, string loadcase, int FrameID)
        {

            
            
            int counter = 0;
            List<string> Forces = new List<string>();
            foreach (FrameAnalysisData fad in StructureResults.FrameResults[FrameID].Results[loadcase].Values)
            {
                if (ForceType == "Axial") //Get Axial Forces
                {
                    string axial = "Axial Force at station " + counter.ToString() + " equals" + fad.P.ToString();
                    Forces.Add(axial);
                    counter += 1;
                }
            }

            return Forces;
        }

        //Results private methods
        private Analysis() { }
        private Analysis(List<FrameResults> fresults)
        {
            FrameResults = fresults;
        }



    }
}
