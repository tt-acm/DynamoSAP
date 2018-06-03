/// Developed by Thornton Tomasetti's CORE Studio for Autodesk
/// http://core.thorntontomasetti.com
/// CORE Developers: Elcin Ertugrul and Ana Garcia Puyol

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SAP2000v20;
// interop.COM services for SAP
using System.Runtime.InteropServices;
//DYNAMO
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;

using DynamoSAP;


namespace SAPConnection
{
    [SupressImportIntoVM]
    public class AnalysisMapper
    {
        public static void RunAnalysis(ref cSapModel mySapModel, string filepath, ref List<string> LoadCaseNames, ref List<string> LoadPatternNames, ref List<string> LoadComboNames)
        {
            bool isLocked = mySapModel.GetModelIsLocked();

            int ret = 0;
            if (!isLocked)
            {
                ret = mySapModel.File.Save(filepath);
                ret = mySapModel.Analyze.RunAnalysis();
            }
            int lcnumber = 0;
            int lpnumber = 0;
            int lcombonumber = 0;
            string[] LCNames = null;
            string[] LPNames = null;
            string[] LComboNames = null;

            ret = mySapModel.LoadCases.GetNameList(ref lcnumber, ref LCNames);
            if(LCNames!=null) LoadCaseNames = LCNames.ToList();

            ret = mySapModel.LoadPatterns.GetNameList(ref lpnumber, ref LPNames);
            if (LPNames != null) LoadPatternNames = LPNames.ToList();

            ret = mySapModel.RespCombo.GetNameList(ref lcombonumber, ref LComboNames);
            if(LComboNames!=null) LoadComboNames = LComboNames.ToList();

        }

        public static List<FrameResults> GetFrameForces(ref cSapModel mySapModel, string patter_case_combo)
        {
            List<FrameResults> fresults = new List<FrameResults>();

            string[] ID = null;
            int NumbOfFrames = 0;

            int ret = mySapModel.FrameObj.GetNameList(ref NumbOfFrames, ref ID);

            for (int i = 0; i < NumbOfFrames; i++)
            {
                Dictionary<string, Dictionary<double, FrameAnalysisData>> FrameAnalysis = new Dictionary<string, Dictionary<double, FrameAnalysisData>>();
                Dictionary<double, FrameAnalysisData> myFrameStationResults = new Dictionary<double, FrameAnalysisData>();

                // frame objectid
                string frameid = ID.GetValue(i).ToString();

                //results

                int NumberResults = 0;
                String[] Obj = null;
                double[] ObjSta = null;
                string[] Elm = null;
                double[] ElmSta = null;
                string[] LoadCase = null;
                string[] StepType = null;
                double[] StepNum = null;
                double[] P = null;
                double[] V2 = null;
                double[] V3 = null;
                double[] T = null;
                double[] M2 = null;
                double[] M3 = null;

                ret = mySapModel.Results.Setup.DeselectAllCasesAndCombosForOutput();

                int ComboType = -1;
                ret = mySapModel.RespCombo.GetTypeOAPI(patter_case_combo, ref ComboType); // 0 = Linear Additive 1 = Envelope 2 = Absolute Additive 3 = SRSS 4 = Range Additive

                if (ret != 0)//if it is not a load combo
                {
                    //set case 
                    ret = mySapModel.Results.Setup.SetCaseSelectedForOutput(patter_case_combo);
                }

                else
                {
                    //set combo 
                    ret = mySapModel.Results.Setup.SetComboSelectedForOutput(patter_case_combo);
                }

                //get frame forces for frame objects      
                ret = mySapModel.Results.FrameForce(frameid, eItemTypeElm.ObjectElm, ref NumberResults, ref Obj, ref ObjSta, ref Elm, ref ElmSta, ref LoadCase, ref StepType, ref StepNum, ref P, ref V2, ref V3, ref T, ref M2, ref M3);


                int index = 0;
                int endindex = 0;
                if (ret == 0)
                {
                    if (ComboType == 1)
                    {
                        index = Array.IndexOf(StepType, "Max");
                        endindex = Array.LastIndexOf(StepType, "Max");

                    }

                    else if (ComboType == 3)
                    {
                        //"The Selected Load Combination type is SRSS";
                        break;
                    }
                }

                if (NumberResults != 0)
                {
                    index = Array.IndexOf(LoadCase, patter_case_combo);
                    endindex = Array.LastIndexOf(LoadCase, patter_case_combo);
                }


                if (NumberResults != 0)
                {
                    double previoust = 0;
                    for (int j = index; j <= endindex; j++)
                    {
                        FrameAnalysisData myForces = new FrameAnalysisData(P[j], V2[j], V3[j], T[j], M2[j], M3[j]);
                        double t = ObjSta[j] / ObjSta[endindex];
                        if (t == previoust)
                        {
                            t = -t;
                        }
                        myFrameStationResults.Add(t, myForces); // instead of j, this should be a parameter t?
                        previoust = t;
                    }
                }
                FrameAnalysis.Add(patter_case_combo, myFrameStationResults);

                FrameResults myFrameResults = new FrameResults(frameid, FrameAnalysis);
                fresults.Add(myFrameResults);
            }

            return fresults;
        }
    }
}


