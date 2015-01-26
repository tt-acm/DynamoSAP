/// Developed by Thornton Tomasetti's CORE Studio for Autodesk
/// http://core.thorntontomasetti.com
/// CORE Developers: Elcin Ertugrul and Ana Garcia Puyol

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
    public class LoadMapper
    {
        // DEFINE LOAD PATTERN In SAPMODEL METHOD
        public static void AddLoadPattern(ref cSapModel Model, string Name, string Type, double Multiplier)
        {
            eLoadPatternType type = (eLoadPatternType)Enum.Parse(typeof(eLoadPatternType), Type);
            int ret = Model.LoadPatterns.Add(Name, type, Multiplier);
        }

        // DEFINE LOAD CASE IN SAP
        public static void AddLoadCase(ref cSapModel Model, string Name, int LoadCount, ref string[] LoadType, ref string[] LoadName, ref double[] SF, string LCType)
        {

            if (LCType == eLoadCaseType.CASE_LINEAR_STATIC.ToString())
            {
                int ret = Model.LoadCases.StaticLinear.SetCase(Name);
                ret = Model.LoadCases.StaticLinear.SetLoads(Name, LoadCount, ref LoadType, ref LoadName, ref SF);
            }
            else if (LCType == eLoadCaseType.CASE_NONLINEAR_STATIC.ToString())
            {
                int ret = Model.LoadCases.StaticNonlinear.SetCase(Name);
                ret = Model.LoadCases.StaticNonlinear.SetLoads(Name, LoadCount, ref LoadType, ref LoadName, ref SF);

                //if it is nonlinear STAGED, it is different:
                //ret = Model.LoadCases.StaticNonlinearStaged.SetCase(Name);
                //must implement stageData methods

            }
            else if (LCType == eLoadCaseType.CASE_MODAL.ToString())
            {
                throw new Exception("Load Case Type not supported");

                //this needs more inputs
                //if SubType==1 (Eigen)
                //int ret = Model.LoadCases.ModalEigen.SetCase(Name);
                //ret = Model.LoadCases.ModalEigen.SetLoads(Name, LoadCount, ref Loadtype, ref LoadName, MyTargetPar, MyStaticCorrect);

                //if SubType==2 (Ritz)
                //int ret = Model.LoadCases.ModalRitz.SetCase(Name);
                //ret = SapModel.LoadCases.ModalRitz.SetLoads(Name, LoadCount, ref LoadType, ref LoadName, MyRitzMaxCyc, MyTargetPar);

            }
            else if (LCType == eLoadCaseType.CASE_RESPONSE_SPECTRUM.ToString())
            {
                throw new Exception("Load Case Type not supported");

                //int ret = Model.LoadCases.ResponseSpectrum.SetCase(Name);

                //this needs more inputs
                //ret = SapModel.LoadCases.ResponseSpectrum.SetLoads(Name, LoadCount, ref LoadName, MyFunc, ref SF, MyCSys, MyAng);

            }
            else if (LCType == eLoadCaseType.CASE_LINEAR_HISTORY.ToString())
            {
                throw new Exception("Load Case Type not supported");

                //int ret = Model.LoadCases.ModHistLinear.SetCase(Name);

                //this needs more inputs
                //ret = Model.LoadCases.ModHistLinear.SetLoads(Name, LoadCount, ref LoadType, ref LoadName, MyFunc, ref SF, MyTF, MyAT, MyCSys, MyAng);
                
                //this needs to pass the motion type, transient or periodic

            }
            else if (LCType == eLoadCaseType.CASE_NONLINEAR_HISTORY.ToString())
            {
                throw new Exception("Load Case Type not supported");

                //int ret = ret = Model.LoadCases.ModHistNonlinear.SetCase(Name);
                //this needs more inputs
                //ret = Model.LoadCases.ModHistNonlinear.SetLoads(Name, LoadCount, ref LoadType, ref LoadName, MyFunc, ref SF, MyTF, MyAT, MyCSys, MyAng)

            }
            else if (LCType == eLoadCaseType.CASE_LINEAR_DYNAMIC.ToString())
            {
                throw new Exception("Load Case Type not supported");

                //int ret = Model.LoadCases.DirHistLinear.SetCase(Name);
                //this needs more inputs
                //ret = Model.LoadCases.DirHistLinear.SetLoads(Name, LoadCount, ref LoadType, ref LoadName, MyFunc, ref SF, MyTF, MyAT, MyCSys, MyAng);
            }
            else if (LCType == eLoadCaseType.CASE_NONLINEAR_DYNAMIC.ToString())
            {
                int ret = Model.LoadCases.StaticNonlinear.SetCase(Name);
                ret = Model.LoadCases.StaticNonlinear.SetLoads(Name, LoadCount, ref LoadType, ref LoadName, ref SF);
            }
            else if (LCType == eLoadCaseType.CASE_MOVING_LOAD.ToString())
            {
                throw new Exception("Load Case Type not supported");
                //int ret = Model.LoadCases.Moving.SetCase(Name);

                //this needs more inputs
                //ret = Model.LoadCases.Moving.SetLoads(Name, LoadCount, MyMyClass, ref SF, MyMin, MyMax);

            }
            else if (LCType == eLoadCaseType.CASE_BUCKLING.ToString())
            {
                int ret = Model.LoadCases.Buckling.SetCase(Name);
                ret = Model.LoadCases.Buckling.SetLoads(Name, 2, ref LoadType, ref LoadName, ref SF);
            }
            else if (LCType == eLoadCaseType.CASE_STEADY_STATE.ToString())
            {
                throw new Exception("Load Case Type not supported");
               // int ret = Model.LoadCases.SteadyState.SetCase(Name);
                //this needs more inputs
                //ret = Model.LoadCases.SteadyState.SetLoads(Name, LoadCount, ref LoadType, ref LoadName, MyFunc, ref SF, MyPhaseAngle, MyCSys, MyAng);

            }
            else if (LCType == eLoadCaseType.CASE_POWER_SPECTRAL_DENSITY.ToString())
            {
                throw new Exception("Load Case Type not supported");
                //int ret = Model.LoadCases.PSD.SetCase(Name);
                //this needs more inputs
                //ret = Model.LoadCases.PSD.SetLoads(Name, LoadCount, ref LoadType, ref LoadName, MyFunc, ref SF, MyPhaseAngle, MyCSys, MyAng);
            }
            else if (LCType == eLoadCaseType.CASE_LINEAR_STATIC_MULTISTEP.ToString())
            {
                int ret = Model.LoadCases.StaticLinearMultistep.SetCase(Name);
                ret = Model.LoadCases.StaticLinearMultistep.SetLoads(Name, LoadCount, ref LoadType, ref LoadName, ref SF);
            }
            else if (LCType == eLoadCaseType.CASE_HYPERSTATIC.ToString())
            {
                int ret = Model.LoadCases.HyperStatic.SetCase(Name);

            }

        }

        public static void AddLoadCombo(ref cSapModel Model, string Name, string[] Types, string[] CName, double[] SF, string LCType)
        {
            int LCTypeInt = 0;
            if (LCType == "Linear Additive") LCTypeInt = 0;
            else if (LCType == "Envelope") LCTypeInt = 1;
            else if (LCType == "Absolute Additive") LCTypeInt = 2;
            else if (LCType == "SRSS") LCTypeInt = 3;
            else if (LCType == "Range Additive") LCTypeInt = 4;

            //add the Combination
            int ret = Model.RespCombo.Add(Name, (LCTypeInt));
            foreach (string def in CName)
            {
                int pos = Array.IndexOf(CName, def);
                //get the type of the definition
                eCType type = (eCType)Enum.Parse(typeof(eCType), Types.ElementAt(pos));
                //set the type of the combination
                ret = Model.RespCombo.SetCaseList(Name, ref type, def, SF.ElementAt(pos));

            }

        }
        //CREATE LOAD METHODS
        public static void CreatePointLoad(ref cSapModel Model, string FrameName, string LoadPat, int MyType, int Dir, double Dist, double Val, string CSys, bool RelDist, bool Replace)
        {
            int ret = Model.FrameObj.SetLoadPoint(FrameName, LoadPat, MyType, Dir, Dist, Val, CSys, RelDist, Replace);
        }
        public static void CreateDistributedLoad(ref cSapModel Model, string FrameName, string LoadPat, int MyType, int Dir, double Dist, double Dist2, double Val, double Val2, string CSys, bool RelDist, bool Replace)
        {
            int ret = Model.FrameObj.SetLoadDistributed(FrameName, LoadPat, MyType, Dir, Dist, Dist2, Val, Val2, CSys, RelDist, Replace);
        }

        public static void SetLoadForcetoJoint(ref cSapModel Model, string JointId, string LoadPat, double[] Values, bool Replace)
        {
            int ret = Model.PointObj.SetLoadForce(JointId, LoadPat, Values, Replace);
        }

        // READ LOAD METHODS
        public static void GetPointLoads(ref cSapModel Model, ref string[] FrameName, ref int NumberItems, ref string[] LoadPat, ref int[] MyType, ref string[] CSys, ref int[] Dir, ref double[] RelDist, ref double[] Dist, ref double[] Val)
        {
            int ret = Model.FrameObj.GetLoadPoint("ALL", ref NumberItems, ref FrameName, ref LoadPat, ref MyType, ref CSys, ref Dir, ref RelDist, ref Dist, ref Val, eItemType.Group);
        }

        public static void GetDistributedLoads(ref cSapModel Model, ref string[] FrameName, ref int NumberItems, ref string[] LoadPat, ref int[] MyType, ref string[] CSys, ref int[] Dir, ref double[] RD1, ref double[] RD2, ref double[] Dist1, ref double[] Dist2, ref double[] Val1, ref double[] Val2)
        {
            int ret = Model.FrameObj.GetLoadDistributed("ALL", ref NumberItems, ref FrameName, ref LoadPat, ref MyType, ref CSys, ref Dir, ref RD1, ref RD2, ref Dist1, ref Dist2, ref Val1, ref Val2, eItemType.Group);
        }


    }
}
