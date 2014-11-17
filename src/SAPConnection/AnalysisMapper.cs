using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SAP2000v16;
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
        public static void RunAnalysis(ref cSapModel mySapModel, string filepath)
        {
            int ret = mySapModel.File.Save(filepath);
            ret = mySapModel.Analyze.RunAnalysis();
        }

        public static List<FrameResults> GetFrameForces(ref cSapModel mySapModel, string lcase)
        {
                      
            
            List<FrameResults> fresults = new List<FrameResults>();
           
            string[] ID = null;
            int NumbOfFrames = 0;

            int ret = mySapModel.FrameObj.GetNameList(ref NumbOfFrames, ref ID);

            for (int i = 0; i < NumbOfFrames; i++)
            {
                Dictionary<string, Dictionary<int, FrameAnalysisData>> FrameAnalysis = new Dictionary<string, Dictionary<int, FrameAnalysisData>>();
                Dictionary<int, FrameAnalysisData> myFrameStationResults = new Dictionary<int, FrameAnalysisData>();

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

                //set case and combo output selections

                ret = mySapModel.Results.Setup.SetCaseSelectedForOutput(lcase);


                //get frame forces for frame objects      
                ret = mySapModel.Results.FrameForce(frameid, eItemTypeElm.ObjectElm, ref NumberResults, ref Obj, ref ObjSta, ref Elm, ref ElmSta, ref LoadCase, ref StepType, StepNum, ref P, ref V2, ref V3, ref T, ref M2, ref M3);

                int ComboType = 1;
                //NOT WORKING
                ret = mySapModel.RespCombo.GetType(lcase, ref ComboType); // 0 = Linear Additive 1 = Envelope 2 = Absolute Additive 3 = SRSS 4 = Range Additive

                int index = 0;
                int endindex = 0;
                if (ComboType == 1)
                {

                    //THIS IS NOT WORKING. THE RET TO GET THE COMBO IS NOT WORKING. AND THE STRINGS OF STEPTYPE ARE NULL, THERE IS NO STEP SET UP IN THIS SAMPLE
                    //index = Array.IndexOf(StepType, "Max");
                    //endindex = Array.LastIndexOf(StepType, "Max");

                    //TESTING THIS!
                    index = Array.IndexOf(LoadCase, lcase);
                    endindex = Array.LastIndexOf(LoadCase, lcase);
                }
                else if (ComboType == 3)
                {
                    //System.Windows.MessageBox.Show("The Selected Load Combination type is SRSS. The result won't be written to DB", "Result", System.Windows.MessageBoxButton.OK);
                    break;
                }
                else
                {
                    if (NumberResults != 0)
                    {
                        index = Array.IndexOf(LoadCase, lcase);
                        endindex = Array.LastIndexOf(LoadCase, lcase);
                    }
                    else
                    {
                        //string AnalysisError = "There is no analysis result for this member";

                    }
                }
                if (NumberResults != 0)
                {
                    double[] Axial = new double[2];
                    Axial[0] = P[index];
                    Axial[1] = P[endindex];

                    double[] Vmajor = new double[2];
                    Vmajor[0] = V2[index];
                    Vmajor[1] = V2[endindex];

                    double[] Vminor = new double[2];
                    Vminor[0] = V3[index];
                    Vminor[1] = V3[endindex];

                    double[] Torsion = new double[2];
                    Torsion[0] = T[index];
                    Torsion[1] = T[endindex];

                    double[] Mmajor = new double[2];
                    Mmajor[0] = M3[index];
                    Mmajor[1] = M3[endindex];

                    double[] Mminor = new double[2];
                    Mminor[0] = M2[index];
                    Mminor[1] = M2[endindex];

                    
                  
                    //for (int j = index; j <= endindex; j++)
                        for (int j = 0; j <= 2; j++)
                    {
                        FrameAnalysisData myForces = new FrameAnalysisData(P[j], V2[j], V3[j], T[j], M2[j], M3[j]);
                        myFrameStationResults.Add(j, myForces);
                    }
                   
                }              
                FrameAnalysis.Add(lcase, myFrameStationResults);
                
                FrameResults myFrameResults = new FrameResults(frameid, FrameAnalysis);
                fresults.Add(myFrameResults);
            }
            
            
            return fresults;
        }

    }
}

//public static string lcase { get; set; }}

