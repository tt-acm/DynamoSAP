using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
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
        public string LoadCombination { get; set; }

        private static cSapModel mySapModel;

        [MultiReturn("Structural Model", "Load Cases", "Load Patterns", "FilePath")]
        public static Dictionary<string, object> Run(bool Run)
        {
            List<string> LoadCaseNames = new List<string>();
            List<string> LoadPatternNames = new List<string>();
            StructuralModel Model = new StructuralModel();
            string SaveAs = "";
            if (Run)
            {
               

                string units = string.Empty;

                SAPConnection.Initialize.GrabOpenSAP(ref mySapModel, ref units);

                Read.StructuralModelFromSapFile(ref mySapModel, ref Model);

                SaveAs = mySapModel.GetModelFilename();

                // add defense for an untitled file
                if (SaveAs == "")
                {
                    throw new Exception("File has not been saved before. Please, go to SAP and save the file");
                }
                if (mySapModel == null)
                {
                    throw new Exception("SAP Model was not grabbed. Use run analysis with filepath");
                }

                else
                {// run analysis
                    SAPConnection.AnalysisMapper.RunAnalysis(ref mySapModel, SaveAs, ref LoadCaseNames, ref LoadPatternNames);
                }
            }
            return new Dictionary<string, object>
            {
                {"Structural Model", Model},
                {"Load Cases", LoadCaseNames},
                {"Load Patterns", LoadPatternNames},
                {"File Path", SaveAs}
            };
        }
        [MultiReturn("Structural Model", "Load Cases", "Load Patterns")]
        public static Dictionary<string, object> Run(string FilePath, bool Run)
        {
            List<string> LoadCaseNames = new List<string>();
            List<string> LoadPatternNames = new List<string>();
            StructuralModel Model = new StructuralModel();
            if (Run)
            {
                string units = string.Empty;

                SAPConnection.Initialize.OpenSAPModel(FilePath, ref mySapModel, ref units);
                Read.StructuralModelFromSapFile(ref mySapModel, ref Model);
                // run analysis
                SAPConnection.AnalysisMapper.RunAnalysis(ref mySapModel, FilePath, ref LoadCaseNames, ref LoadPatternNames);
            }
            return new Dictionary<string, object>
            {
                {"Structural Model", Model},
                {"Load Cases", LoadCaseNames},
                {"Load Patterns", LoadPatternNames}
            };
        }

        public static Analysis GetResults(string LoadCombination, bool Get)
        {
            List<FrameResults> frameResults = null;
            Analysis StructureResults = new Analysis();
            if (Get)
            {
                // loop over frames get results and populate to dictionary
                frameResults = SAPConnection.AnalysisMapper.GetFrameForces(ref mySapModel, LoadCombination);
                StructureResults.FrameResults = frameResults;
                StructureResults.LoadCombination = LoadCombination;
            }
            return StructureResults;
        }

        public static List<List<double>> DecomposeResults(StructuralModel StructuralModel, Analysis AnalysisResults, string ForceType)
        {
            List<List<double>> Forces = new List<List<double>>();


            for (int i = 0; i < StructuralModel.StructuralElements.Count; i++)
            {
                List<double> ff = new List<double>();
                // linq inqury
                FrameResults frmresult = (from frm in AnalysisResults.FrameResults
                                          where frm.ID == StructuralModel.StructuralElements[i].Label
                                          select frm).First();

                foreach (FrameAnalysisData fad in frmresult.Results[AnalysisResults.LoadCombination].Values)
                {

                    if (ForceType == "Axial") //Get Axial Forces P
                    {
                        ff.Add(fad.P);
                    }
                    else if (ForceType == "Shear22") // Get Shear V2
                    {
                        ff.Add(fad.V2);
                    }
                    else if (ForceType == "Shear33") // Get Shear V3
                    {
                        ff.Add(fad.V3);
                    }

                    else if (ForceType == "Torsion") // Get Torsion T
                    {
                        ff.Add(fad.T);
                    }

                    else if (ForceType == "Moment22") // Get Moment M2
                    {
                        ff.Add(fad.M2);
                    }
                    else if (ForceType == "Moment33") // Get Moment M3
                    {
                        ff.Add(fad.M3);
                    }
                }
                Forces.Add(ff);
            }

            return Forces;
        }

        public static List<List<Mesh>> VisualizeResults(StructuralModel StructuralModel, Analysis AnalysisResults, string ForceType, double scale, bool Visualize)
        {
            List<List<Mesh>> myVizMeshes = new List<List<Mesh>>();
            if (Visualize)
            {
                for (int i = 0; i < StructuralModel.StructuralElements.Count; i++)
                {
                    List<Mesh> mm = new List<Mesh>();
                    // get the frame's curve specified by the frameID

                    Frame f = (Frame)StructuralModel.StructuralElements[i];
                    Curve c = f.BaseCrv;

                    //CREATE LOCAL COORDINATE SYSTEM
                    Vector xAxis = c.TangentAtParameter(0.0);
                    Vector yAxis = c.NormalAtParameter(0.0);
                    //This ensures the right axis for the Z direction  
                    CoordinateSystem localCS = CoordinateSystem.ByOriginVectors(c.StartPoint, xAxis, yAxis);


                    //TEST TO VISUALIZE NORMALS
                    //Point pt = c.PointAtParameter(0.5);
                    //Line ln = Line.ByStartPointDirectionLength(pt, localCS.ZAxis, 30.0);
                    //myLines.Add(ln);


                    List<Point> MeshPoints = new List<Point>();

                    int count = 0;

                    double t2 = 0.0;
                    double t1 = 0.0;
                    int zeroCount = 0;
                    foreach (double t in AnalysisResults.FrameResults[i].Results[AnalysisResults.LoadCombination].Keys)
                    {
                        double newt = t;
                        if (t < 0) newt = -t;
                        Mesh m = null;
                        IndexGroup ig = null;

                        count += 1;

                        Point cPoint = c.PointAtParameter(newt); // curve Point
                        Point vPoint = null; // value Point

                        double translateCoord = 0.0;

                        if (ForceType == "Axial") // Get Axial P
                        {
                            translateCoord = AnalysisResults.FrameResults[i].Results[AnalysisResults.LoadCombination][newt].P * -scale;
                        }

                        else if (ForceType == "Shear22") // Get Shear V2
                        {
                            translateCoord = AnalysisResults.FrameResults[i].Results[AnalysisResults.LoadCombination][newt].V2 * -scale;
                        }
                        else if (ForceType == "Shear33") // Get Shear V3
                        {
                            translateCoord = AnalysisResults.FrameResults[i].Results[AnalysisResults.LoadCombination][newt].V3 * scale;
                        }

                        else if (ForceType == "Torsion") // Get Torsion T
                        {
                            translateCoord = AnalysisResults.FrameResults[i].Results[AnalysisResults.LoadCombination][newt].T * -scale;
                        }

                        else if (ForceType == "Moment22") // Get Moment M2
                        {
                            translateCoord = AnalysisResults.FrameResults[i].Results[AnalysisResults.LoadCombination][newt].M2 * scale;
                        }

                        else if (ForceType == "Moment33") // Get Moment M3
                        {
                            translateCoord = AnalysisResults.FrameResults[i].Results[AnalysisResults.LoadCombination][newt].M3 * scale;
                        }

                        // if there is no value for the force (it is zero), add one to the count
                        if (translateCoord == 0.0) zeroCount++;

                        double d2 = 0.0;
                        double d1 = 0.0;
                        double pZ = 0.0;

                        if (ForceType == "Moment22")
                        {
                            vPoint = (Point)cPoint.Translate(localCS.YAxis, translateCoord); // Translate in the Y direction to match the visualization of SAP
                            if (MeshPoints.Count > 0)
                            {
                                d2 = vPoint.Y;
                                d1 = MeshPoints[MeshPoints.Count - 1].Y;
                                pZ = cPoint.Y;
                            }
                        }
                        else
                        {
                            vPoint = (Point)cPoint.Translate(localCS.ZAxis, translateCoord); // All the other types must be translate in the Z direction} 
                            if (MeshPoints.Count > 0)
                            {
                                d2 = vPoint.Z;
                                d1 = MeshPoints[MeshPoints.Count - 1].Z;

                                pZ = cPoint.Z;// Z value of the point being visualized
                            }
                        }

                        Point pzero = null;
                        if (MeshPoints.Count == 0)
                        {
                            MeshPoints.Add(cPoint); //index 0
                            MeshPoints.Add(vPoint); //index 1
                        }

                        else// if a previous point has been added
                        {
                            if (count != AnalysisResults.FrameResults[i].Results[AnalysisResults.LoadCombination].Keys.Count) // if it's not the end of the list
                            {
                                List<IndexGroup> indices = new List<IndexGroup>();

                                double tzero;//parameter at which the value of the forces = pZ

                                t2 = newt * c.Length; // current t parameter of the point being visualized
                                if ((d1 > pZ && d2 < pZ) || (d1 < pZ && d2 > pZ)) // if there is a change in the force sign, calculate the intersection point
                                {

                                    // the function of the line is
                                    //y= (t2-t1)tzero/(d2-d1)+d1  This has to be equal to pZ
                                    double ml = (d2 - d1) / (t2 - t1);
                                    tzero = (pZ - d1) / ml; // multiply by the length of the curve and add the X coordinate of the last mesh point


                                    tzero += t1;

                                    //pzero= Point.ByCartesianCoordinates(CoordinateSystem.Identity(), tzero, cPoint.Y, pZ); //CHECK THAT THIS IS CORRECT

                                    pzero = Point.ByCartesianCoordinates(localCS, tzero, 0.0, 0.0); //CHECK THAT THIS IS CORRECT
                                    MeshPoints.Add(pzero); //index 2 

                                    ig = IndexGroup.ByIndices(0, 1, 2);
                                    indices.Add(ig);
                                    // Color coding here

                                }
                                else
                                {
                                    MeshPoints.Add(vPoint); //index 2 (note: vPoint before cPoint)
                                    MeshPoints.Add(cPoint); //index 3 
                                    if (MeshPoints.Count == 3)
                                    {
                                        ig = IndexGroup.ByIndices(0, 1, 2);
                                        indices.Add(ig);
                                    }
                                    else
                                    {
                                        ig = IndexGroup.ByIndices(0, 1, 2, 3);
                                        indices.Add(ig);
                                    }
                                    //color coding here
                                }

                                // Add face
                                //append...??
                                m = Mesh.ByPointsFaceIndices(MeshPoints, indices);
                                mm.Add(m);

                                MeshPoints.Clear();

                                if ((d1 > pZ && d2 < pZ) || (d1 < pZ && d2 > pZ)) // if there is a change in the force sign
                                {
                                    MeshPoints.Add(pzero); //new face index 0
                                }
                                else
                                {
                                    MeshPoints.Add(cPoint); //new face index 0
                                    MeshPoints.Add(vPoint); //new face index 1   
                                }
                            }
                            else
                            {
                                MeshPoints.Add(vPoint); //index 2 (note: vPoint before cPoint)
                                MeshPoints.Add(cPoint); //index 3 

                                // Add face
                                List<IndexGroup> indices = new List<IndexGroup>();
                                if (MeshPoints.Count == 3)
                                {
                                    ig = IndexGroup.ByIndices(0, 1, 2);
                                    indices.Add(ig);
                                }
                                else
                                {
                                    ig = IndexGroup.ByIndices(0, 1, 2, 3);
                                    indices.Add(ig);
                                }
                                //append...??
                                m = Mesh.ByPointsFaceIndices(MeshPoints, indices);
                                mm.Add(m);
                            }
                        }
                        t1 = newt * c.Length;
                    }
                    // if all the values were zero, do not add the mesh
                    if (zeroCount == AnalysisResults.FrameResults[i].Results[AnalysisResults.LoadCombination].Keys.Count)
                    {
                        continue;
                    }
                    else
                    {
                        myVizMeshes.Add(mm);
                    }

                }
            }
            return myVizMeshes;
        }

        public static List<Object> TranslateDisplay(List<Object> AnalysisMeshes, Vector Direction)
        {
            List<Object> objs = new List<Object>();

            foreach (Object obj in AnalysisMeshes)
            {
                try
                {
                    Mesh m = (Mesh)obj;
                    Mesh newm = null;
                    Point[] pp = m.VertexPositions;
                    List<Point> mypoints = new List<Point>();
                    foreach (Point ppt in pp)
                    {
                        Point p = (Point)ppt.Translate(Direction);
                        mypoints.Add(p);
                    }
                    IndexGroup ig = null;
                    List<IndexGroup> indices = new List<IndexGroup>();
                    if (mypoints.Count == 4)
                    {
                        ig = IndexGroup.ByIndices(0, 1, 2, 3);

                    }
                    else
                    {
                        ig = IndexGroup.ByIndices(0, 1, 2);
                    }
                    indices.Add(ig);

                    newm = Mesh.ByPointsFaceIndices(mypoints, indices);
                    objs.Add(newm);
                }
                catch (Exception)
                {

                    //throw;
                }
                try
                {
                    Line ln = (Line)obj;
                    Line lnt = (Line)ln.Translate(Direction);
                    objs.Add(lnt);
                }
                catch (Exception)
                {
                    //throw;
                }
            }


            return objs;
        }

        //Results private methods
        private Analysis() { }
        private Analysis(List<FrameResults> fresults, string loadcombination)
        {
            FrameResults = fresults;
            LoadCombination = loadcombination;
        }

    }


}
