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
                        int pos = Array.IndexOf(LoadPatternNames, lpname);
                        model.LoadPatterns.Add(new LoadPattern(lpname, LoadPatternTypes[pos], LoadPatternMultipliers[pos]));
                    }
                }


                // METHODS FOR FRAMES

                //2. GET LOADS

                //Get Point Loads
                string[] framesWithPointLoads = null;
                string[] PlPattern = null;
                int Pnumber = 0;
                int[] PmyType = null;
                string[] PCsys = null;
                int[] Pdir = null;
                double[] PRelDist = null;
                double[] PDist = null;
                double[] PVal = null;

                StructureMapper.GetPointLoads(ref SapModel, ref framesWithPointLoads, ref Pnumber, ref PlPattern, ref PmyType, ref PCsys, ref Pdir, ref PRelDist, ref PDist, ref PVal);

                // Get Distributed Loads
                string[] framesWithDistributedLoads = null;
                string[] DlPattern = null;
                int Dnumber = 0;
                int[] DmyType = null;
                string[] DCsys = null;
                int[] Ddir = null;
                double[] DRD1 = null;
                double[] DRD2 = null;
                double[] DDist1 = null;
                double[] DDist2 = null;
                double[] DVal1 = null;
                double[] DVal2 = null;

                StructureMapper.GetDistributedLoads(ref SapModel, ref framesWithDistributedLoads, ref Dnumber, ref DlPattern, ref DmyType, ref DCsys, ref Ddir, ref DRD1, ref DRD2, ref DDist1, ref DDist2, ref DVal1, ref DVal2);

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



//!!!!!!!! MUST LOOK TO CHECK IF THE FRAME HAS MORE THAN ONE LOAD

                    //Check if the frame has a load
                    if (framesWithDistributedLoads != null)
                    {
                        int Dpos = Array.IndexOf(framesWithDistributedLoads, d_frm.Label);
                        if (Dpos > -1)
                        {
                            d_frm.Loads = new List<Load>();
                            
                           
                            
                            LoadPattern Dlp = null;
                            foreach (LoadPattern loadp in model.LoadPatterns)
                            {
                                if (loadp.name == DlPattern[Dpos])
                                {
                                    Dlp = loadp;
                                    break;
                                }
                            }
                            if (Dlp != null)
                            {
                                // using relDist as true, and using the relative distance values DRD1 and DRD2
                                bool relDist = true;
                                Load l = new Load(Dlp, DmyType[Dpos], Ddir[Dpos], DRD1[Dpos], DRD2[Dpos], DVal1[Dpos], DVal2[Dpos], DCsys[Dpos], relDist);
                                l.LoadType = "DistributedLoad";
                                d_frm.Loads.Add(l);
                            }
                        }
                    }

                    if (framesWithPointLoads != null)
                    {
                        int Ppos = Array.IndexOf(framesWithPointLoads, d_frm.Label);
                        if (Ppos > -1)
                        {
                            if (d_frm.Loads == null)
                            {
                                d_frm.Loads = new List<Load>();
                            }

                            LoadPattern Plp = null;
                            foreach (LoadPattern loadp in model.LoadPatterns)
                            {
                                if (loadp.name == PlPattern[Ppos])
                                {
                                    Plp = loadp;
                                    break;
                                }
                            }

                            if (Plp != null)
                            {
                                bool relativedist = true;
                                Load l = new Load(Plp, PmyType[Ppos], Pdir[Ppos], PRelDist[Ppos], PVal[Ppos], PCsys[Ppos], relativedist);
                                l.LoadType = "PointLoad";
                                d_frm.Loads.Add(l);
                            }
                        }
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
