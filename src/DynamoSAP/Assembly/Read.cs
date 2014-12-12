using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DynamoSAP.Structure;
using DynamoSAP;

//DYNAMO
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;

using SAPConnection;
using SAP2000v16;

namespace DynamoSAP.Assembly
{
    public class Read
    {
        //// DYNAMO NODES ////

        /// <summary>
        /// Read a SAP project from a filepath
        /// </summary>
        /// <param name="FilePath">Filepath of the project to read</param>
        /// <param name="read">Set Boolean to True to open and readthe project</param>
        /// <returns>Structural Model</returns>
        [MultiReturn("StructuralModel", "units")]
        public static Dictionary<string, object> SAPModel(string FilePath, bool read)
        {
            if (read)
            {
                StructuralModel Model = new StructuralModel();
                Model.StructuralElements = new List<Element>();
                cSapModel mySapModel = null;
                string units = string.Empty;
                // Open & instantiate SAP file
                Initialize.OpenSAPModel(FilePath, ref mySapModel, ref units);

                // Populate the model's elemets
                StructuralModelFromSapFile(ref mySapModel, ref Model);

                // Return outputs
                return new Dictionary<string, object>
                {
                    {"StructuralModel", Model},
                    {"units", units}
                };
            }
            else
            {
                throw new Exception("Set boolean True to read!");
            }

        }

        /// <summary>
        /// Read a SAP project from an open instance
        /// </summary>
        /// <param name="read">Set Boolean to True to read the open project</param>
        /// <returns>Structural Model</returns>
        [MultiReturn("StructuralModel", "units")]
        public static Dictionary<string, object> SAPModel(bool read)
        {
            if (read)
            {
                StructuralModel Model = new StructuralModel();

                cSapModel mySapModel = null;
                string units = string.Empty;

                // Open & instantiate SAP file
                Initialize.GrabOpenSAP(ref mySapModel, ref units);

                StructuralModelFromSapFile(ref mySapModel, ref Model);

                // Return outputs
                return new Dictionary<string, object>
                {
                    {"StructuralModel", Model},
                    {"units", units}
                };
            }
            else
            {
                throw new Exception("Set boolean True to read!");
            }

        }

        private Read() { }

        internal static void StructuralModelFromSapFile(ref cSapModel SapModel, ref StructuralModel model)
        {
            model.StructuralElements = new List<Element>();
            if (SapModel != null)
            {

                // 1. GET LOAD PATTERNS

                string[] LoadPatternNames = null;
                string[] LoadPatternTypes = null;
                double[] LoadPatternMultipliers = null;

                StructureMapper.GetLoadPatterns(ref SapModel, ref LoadPatternNames, ref LoadPatternTypes, ref LoadPatternMultipliers);
                if (LoadPatternNames != null)
                {
                    model.LoadPatterns = new List<LoadPattern>();
                    foreach (string lpname in LoadPatternNames)
                    {
                        int pos = Array.IndexOf(LoadPatternNames,lpname);
                        model.LoadPatterns.Add(new LoadPattern(lpname, LoadPatternTypes[pos], LoadPatternMultipliers[pos]));
                    }
                }
                //2. GET LOADS
                string[] framesWithLoads = null;
                string[] lPattern = null;
                int number = 0;
                int[] myType = null;
                string[] Csys = null;
                int[] dir = null;
                double[] RD1 = null;
                double[] RD2 = null;
                double[] Dist1 = null;
                double[] Dist2 = null;
                double[] Val1 = null;
                double[] Val2 = null;

                // This is only getting distributed loads right now MUST GET POINT LOADS TOO
                StructureMapper.GetLoads(ref SapModel, ref framesWithLoads, ref number, ref lPattern, ref myType, ref Csys, ref dir, ref RD1, ref RD2, ref Dist1, ref Dist2, ref Val1, ref Val2);





                // Populate the model's elements
                //3. GET FRAMES    
                string[] FrmIds = null;
                StructureMapper.GetFrameIds(ref FrmIds, ref SapModel);
                for (int i = 0; i < FrmIds.Length; i++)
                {
                    Point s = null;
                    Point e = null;
                    string matProp = "A992Fy50"; // default value
                    string secName = "W12X14"; // default value
                    string secCatalog = "AISC14"; // default value
                    string Just = "MiddleCenter"; // default value
                    double Rot = 0; // default value

                    StructureMapper.GetFrm(ref SapModel, FrmIds[i], ref s, ref e, ref matProp, ref secName, ref Just, ref Rot, ref secCatalog);
                    SectionProp secProp = new SectionProp(secName, matProp, secCatalog);
                    Frame d_frm = new Frame(s, e, secProp, Just, Rot);
                    d_frm.Label = FrmIds[i];
                    // get Guid
                    string guid = string.Empty;
                    StructureMapper.GetGUIDFrm(ref SapModel, FrmIds[i], ref guid);
                    d_frm.GUID = guid;
                    model.StructuralElements.Add(d_frm);



                    //!!!!!!!! MUST LOOK TO CHECK IF THE FRAME HAS MORE THAN ONE DISTRIBUTED LOAD
                    //check if the frame has a load
                    int pos = Array.IndexOf(framesWithLoads, d_frm.Label);
                    if (pos > -1)
                    {
                        d_frm.Loads = new List<Load>();
                        // the array contains the frame and the pos variable will hosts its position in the array
                        bool relDist = true;
                        if (RD1 == null) relDist = false;
                        LoadPattern lp = null;
                        foreach (LoadPattern loadp in model.LoadPatterns)
                        {
                            if (loadp.name == lPattern[pos])
                            {
                                lp = loadp;
                                break;
                            }
                        }
                        Load l = new Load(lp, myType[pos], dir[pos], Dist1[pos], Dist2[pos], Val1[pos], Val2[pos], Csys[pos], relDist);
                        l.LoadType = "DistributedLoad";
                        d_frm.Loads.Add(l);
                    }

                }
            }
            else
            {
                throw new Exception("Make sure SAP Model is open!");
            }
        }
    }
}
