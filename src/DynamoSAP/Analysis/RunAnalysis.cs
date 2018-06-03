/// Developed by Thornton Tomasetti's CORE Studio for Autodesk
/// http://core.thorntontomasetti.com
/// CORE Developers: Elcin Ertugrul and Ana Garcia Puyol

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
//DYNAMO
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;

//SAP 
using SAP2000v20;
using SAPConnection;
using DynamoSAP.Assembly;
using DynamoSAP.Structure;

namespace DynamoSAP.Analysis
{
    /// <summary>
    /// Nodes to Run analysis and get and visualize results
    /// </summary>
    public class Analysis : IResults
    {
        /// <summary>
        /// Analysis results for each frame
        /// </summary>
        public List<FrameResults> FrameResults { get; set; }
        /// <summary>
        /// Load combination (load case or load pattern) of the analysis results
        /// </summary>
        public string LCaseOrLPatternRun { get; set; }

        private static cSapModel mySapModel;


        /// <summary>
        /// Run analysis in an open instance of SAP2000
        /// </summary>
        /// <param name="Run">Set Boolean to True to run the analysis</param>
        /// <returns name ="Structural Model"> Structural Model that has been analyzed</returns>
        /// <returns name ="Load Cases"> Load Cases in the project</returns>
        /// <returns name ="Load Patterns"> Load Patterns in the project</returns>
        /// <returns name ="Filepath"> Filepath where the SAP2000 model is saved</returns>
        [MultiReturn("Structural Model", "Load Cases", "Load Patterns", "Load Combos", "Filepath")]
        public static Dictionary<string, object> Run(bool Run)
        {
            List<string> LoadCaseNames = new List<string>();
            List<string> LoadPatternNames = new List<string>();
            List<string> LoadComboNames = new List<string>();
            StructuralModel Model = new StructuralModel();
            string SaveAs = "";
            if (Run)
            {
                string modelunits = string.Empty;

                SAPConnection.Initialize.GrabOpenSAP(ref mySapModel, ref modelunits, "");

                Read.StructuralModelFromSapFile(ref mySapModel, ref Model, modelunits);

                SaveAs = SAPConnection.Initialize.GetModelFilename(ref mySapModel);

                if (SaveAs == "")
                {
                    throw new Exception("File has not been saved before. Please, go to SAP and save the file");
                }
                if (mySapModel == null)
                {
                    throw new Exception("SAP Model was not grabbed. Use run analysis with filepath");
                }
                else
                {
                    // run analysis
                    SAPConnection.AnalysisMapper.RunAnalysis(ref mySapModel, SaveAs, ref LoadCaseNames, ref LoadPatternNames, ref LoadComboNames);
                }
            }
            return new Dictionary<string, object>
            {
                {"Structural Model", Model},
                {"Load Cases", LoadCaseNames},
                {"Load Patterns", LoadPatternNames},
                {"Load Combos", LoadComboNames},
                {"Filepath", SaveAs}
            };
        }


        /// <summary>
        /// Run analysis from an existing SAP2000 project. 
        /// </summary>
        /// <param name="FilePath">Filepath of the file to read from</param>
        /// <param name="Run">Set Boolean to True to run the analysis</param>
        /// <returns name ="Structural Model"> Structural Model that has been analyzed</returns>
        /// <returns name ="Load Cases"> Load Cases in the project</returns>
        /// <returns name ="Load Patterns"> Load Patterns in the project</returns>
        [MultiReturn("Structural Model", "Load Cases", "Load Patterns", "Load Combos")]
        public static Dictionary<string, object> Run(string FilePath, bool Run)
        {
            List<string> LoadCaseNames = new List<string>();
            List<string> LoadPatternNames = new List<string>();
            List<string> LoadComboNames = new List<string>();
            StructuralModel Model = new StructuralModel();
            if (Run)
            {
                string units = string.Empty;

                SAPConnection.Initialize.OpenSAPModel(FilePath, ref mySapModel, ref units);
                Read.StructuralModelFromSapFile(ref mySapModel, ref Model, units);
                // run analysis
                SAPConnection.AnalysisMapper.RunAnalysis(ref mySapModel, FilePath, ref LoadCaseNames, ref LoadPatternNames, ref LoadComboNames);
            }
            return new Dictionary<string, object>
            {
                {"Structural Model", Model},
                {"Load Cases", LoadCaseNames},
                {"Load Patterns", LoadPatternNames},
                {"Load Combos", LoadComboNames}
            };
        }

        /// <summary>
        /// Get the results from the analysis run with the Analysis.Run component
        /// </summary>
        /// <param name="LPattern_LCase_LCombo">Choose a load case, load pattern or load combination</param>
        /// <param name="Get">Set Boolean to True to get the specified case;s analysis results </param>
        /// <returns></returns>
        public static Analysis GetResults(string LPattern_LCase_LCombo, bool Get)
        {
            List<FrameResults> frameResults = null;
            Analysis AnalysisResults = new Analysis();
            if (Get)
            {
                if (LPattern_LCase_LCombo == "MODAL")
                {
                    throw new Exception("MODAL case is not supported. Select another load case");
                }
                // loop over frames get results and populate to dictionary
                frameResults = SAPConnection.AnalysisMapper.GetFrameForces(ref mySapModel, LPattern_LCase_LCombo);
                AnalysisResults.FrameResults = frameResults;
                AnalysisResults.LCaseOrLPatternRun = LPattern_LCase_LCombo;
            }
            return AnalysisResults;
        }

        /// <summary>
        /// Decompose forces and moments in the model for a specific case
        /// </summary>
        /// <param name="StructuralModel">Structural model to get results from</param>
        /// <param name="AnalysisResults">Use Analysis.Run and Analysis.GetResults to select a specific case to decompose</param>
        /// <param name="ForceType">Use Force Type dropdown</param>
        /// <returns>Numerical values of forces and moments for each station in each structural member in the model </returns>
        public static List<List<double>> DecomposeResults(StructuralModel StructuralModel, Analysis AnalysisResults, string ForceType)
        {
            List<List<double>> Forces = new List<List<double>>();

            for (int i = 0; i < StructuralModel.StructuralElements.Count; i++)
            {
                List<double> ff = new List<double>();
                FrameResults frmresult = null;
                // linq inqury
                try
                {
                    frmresult = (from frm in AnalysisResults.FrameResults
                                 where frm.ID == StructuralModel.StructuralElements[i].Label
                                 select frm).First();

                }
                catch { }

                //if it is a joint, the element will not have frame results
                //Add 0.0 to maintain the order of the structural elements in the Structural Model
                if (frmresult == null || frmresult.Results.Count == 0)
                {
                    ff.Add(0.0);
                    Forces.Add(ff);
                    continue;
                }

                foreach (FrameAnalysisData fad in frmresult.Results[AnalysisResults.LCaseOrLPatternRun].Values)
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

        /// <summary>
        /// Visualize forces and moments in the model for a specific case
        /// </summary>
        /// <param name="StructuralModel">Structural model to visualize results on</param>
        /// <param name="AnalysisResults">Use Analysis.Run and Analysis.GetResults to select a specific case to decompose</param>
        /// <param name="ForceType">Use Force Type dropdown</param>
        /// <param name="Scale">Scale of the visualization</param>
        /// <param name="Visualize">Set Boolean to True to draw the meshes</param>
        /// <returns>Forces and moments  in the form of meshes for each station in each structural member in the model</returns>
        /// 
        public static List<List<Mesh>> VisualizeResults(StructuralModel StructuralModel, Analysis AnalysisResults, string ForceType, bool Visualize, double Scale = 1.0)
        {
            List<List<double>> myForces = DecomposeResults(StructuralModel, AnalysisResults, ForceType);
            double max = 0.0;
            for (int i = 0; i < myForces.Count; i++)
            {
                for (int j = 0; j < myForces[i].Count; j++)
                {
                    if (myForces[i][j] >= max) max = myForces[i][j];
                }
            }
            // Define a coefficient to visualize the forces
            Frame fs = (Frame)StructuralModel.StructuralElements[0];
            double lenght = 0.5 * fs.BaseCrv.Length;
            double coefficient = 0.0;
            if (max != 0) coefficient = lenght / max;

            List<List<Mesh>> VizMeshes = new List<List<Mesh>>();
            List<Line> frameNormals = new List<Line>();
            if (Visualize)
            {
                int j = 0;
                for (int i = 0; i < StructuralModel.StructuralElements.Count; i++)
                //for (int i = 0; i < AnalysisResults.FrameResults.Count; i++)
                {

                    //List of meshes per structural element in the model
                    List<Mesh> frameResultsMesh = new List<Mesh>();

                    // Linq inqury
                    FrameResults frmresult = null;
                    try
                    {
                        frmresult = (from frm in AnalysisResults.FrameResults
                                     where frm.ID == StructuralModel.StructuralElements[i].Label
                                     select frm).First();
                    }
                    catch { }

                    if (frmresult == null || StructuralModel.StructuralElements[i].Type!= Structure.Type.Frame)
                    {
                        //Add an empty list to match the tree that contains Joints
                        VizMeshes.Add(frameResultsMesh);
                        continue;
                    }


                    // Get the frame's curve specified by the frameID
                    // Frame f = (Frame)StructuralModel.StructuralElements[i];
                    Frame f = (Frame)(from frame in StructuralModel.StructuralElements
                                      where frame.Label == frmresult.ID
                                      select frame).First();

                    Curve c = f.BaseCrv;

                    //LOCAL COORDINATE SYSTEM
                    Vector xAxis = c.TangentAtParameter(0.0);
                    Vector yAxis = c.NormalAtParameter(0.0);
                    //This ensures the right axis for the Z direction  
                    CoordinateSystem localCS = CoordinateSystem.ByOriginVectors(c.StartPoint, xAxis, yAxis);

                    //LINES TO VISUALIZE THE NORMALS OF THE FRAME CURVES
                    //Point middlePt = c.PointAtParameter(0.5);
                    //Line ln = Line.ByStartPointDirectionLength(middlePt, localCS.ZAxis, 30.0);
                    //frameNormals.Add(ln);

                    //List to hold the points to make a mesh face
                    List<Point> MeshPoints = new List<Point>();

                    // t value of the previous station
                    double t1 = 0.0;
                    // t value of the current station
                    double t2 = 0.0;
                    // Local Z value of the previous station
                    double v1 = 0.0;
                    // Local Z value of the current station
                    double v2 = 0;
                    // Integer to count the number of times there are stations with value=0 (no force)
                    int zeroCount = 0;

                    // Loop through each station (t value) in the analysis results dictionary
                    foreach (double t in frmresult.Results[AnalysisResults.LCaseOrLPatternRun].Keys)
                    {
                        double newt = t;
                        if (t < 0) newt = -t;
                        Mesh m = null;

                        Point tPoint = c.PointAtParameter(newt); // Point on the curve at the specified t2
                        Point vPoint = null; // Point that is translated from the cPoint according to the value v2

                        // Double to hold the local Z value of the station multiplied by the scale factor
                        double translateCoord = 0.0;

                        if (ForceType == "Axial") // Get Axial P
                        {
                            v2 = AnalysisResults.FrameResults[j].Results[AnalysisResults.LCaseOrLPatternRun][newt].P;
                            translateCoord = v2 * coefficient * (-Scale);
                        }

                        else if (ForceType == "Shear22") // Get Shear V2
                        {
                            v2 = AnalysisResults.FrameResults[j].Results[AnalysisResults.LCaseOrLPatternRun][newt].V2;
                            translateCoord = v2 * coefficient * (-Scale);
                        }
                        else if (ForceType == "Shear33") // Get Shear V3
                        {
                            v2 = AnalysisResults.FrameResults[j].Results[AnalysisResults.LCaseOrLPatternRun][newt].V3;
                            translateCoord = v2 * coefficient * coefficient * Scale;
                        }

                        else if (ForceType == "Torsion") // Get Torsion T
                        {
                            v2 = AnalysisResults.FrameResults[j].Results[AnalysisResults.LCaseOrLPatternRun][newt].T;
                            translateCoord = v2 * coefficient * (-Scale);
                        }

                        else if (ForceType == "Moment22") // Get Moment M2
                        {
                            v2 = AnalysisResults.FrameResults[j].Results[AnalysisResults.LCaseOrLPatternRun][newt].M2;
                            translateCoord = v2 * coefficient * (-Scale);
                        }

                        else if (ForceType == "Moment33") // Get Moment M3
                        {
                            v2 = AnalysisResults.FrameResults[j].Results[AnalysisResults.LCaseOrLPatternRun][newt].M3;
                            translateCoord = v2 * coefficient * Scale;
                        }

                        v2 = Math.Round(v2, 4, MidpointRounding.AwayFromZero);

                        // if there is no value for the force (it is zero), add one to the count
                        if (translateCoord == 0.0) zeroCount++;

                        if (ForceType == "Moment22" || ForceType == "Shear33")
                        {
                            vPoint = (Point)tPoint.Translate(localCS.YAxis, translateCoord); // Translate in the Y direction to match the visualization of SAP
                        }
                        else
                        {
                            vPoint = (Point)tPoint.Translate(localCS.ZAxis, translateCoord); // All the other types must be translate in the Z direction} 
                        }

                        //Point that results from the intersection of the line between two value Points and the frame curve
                        Point pzero = null;

                        IndexGroup ig3 = IndexGroup.ByIndices(0, 1, 2);
                        IndexGroup ig4 = IndexGroup.ByIndices(0, 1, 2, 3);

                        //if no points have been added yet, add the point on the curve and then the point representing the value
                        if (MeshPoints.Count == 0)
                        {
                            MeshPoints.Add(tPoint); //index 0

                            // if the first value is not 0

                            if (v2 != 0.0)
                            {
                                MeshPoints.Add(vPoint); //index 1
                            }
                        }

                         // if a previous point(s) has been added
                        else
                        {
                            // List to hold the indices for the mesh face
                            List<IndexGroup> indices = new List<IndexGroup>();

                            //Parameter at which the value of the forces = 0. It is the X coordinate of the pzero
                            double tzero;

                            // Current t parameter of the point being visualized relative to the length of the frame curve
                            t2 = newt * c.Length;

                            // If there is a change in the force sign, calculate the intersection point
                            // Then, add two trianglular mesh faces
                            if ((v1 > 0 && v2 < 0) || (v1 < 0 && v2 > 0))
                            {
                                // The function of the line is: y= (t2-t1)tzero/(d2-d1)+d1  This has to be equal to 0
                                double ml = (v2 - v1) / (t2 - t1);
                                tzero = (0 - v1) / ml;

                                // Add the X coordinate of the last mesh point
                                tzero += t1;

                                pzero = Point.ByCartesianCoordinates(localCS, tzero, 0.0, 0.0);


                                //Add the third point for the first triangular mesh face
                                MeshPoints.Add(pzero); //index 2 
                                indices.Add(ig3);

                                // ||| Color coding here

                                m = Mesh.ByPointsFaceIndices(MeshPoints, indices);

                                frameResultsMesh.Add(m);

                                MeshPoints.Clear();

                                //Add the third point for the second triangular mesh face
                                MeshPoints.Add(pzero); //new face index 0
                                // Add the current station's points
                                MeshPoints.Add(tPoint); //new face index 1
                                MeshPoints.Add(vPoint); //new face index 2  

                                // ||| Color coding here

                                m = Mesh.ByPointsFaceIndices(MeshPoints, indices);
                                frameResultsMesh.Add(m);
                            }

                            // Create a quad mesh face or a triangular mesh if the first value was 0
                            else
                            {
                                MeshPoints.Add(vPoint); //index 2 (note: vPoint before cPoint)
                                MeshPoints.Add(tPoint); //index 3 


                                if (MeshPoints.Count == 4) indices.Add(ig4);
                                else indices.Add(ig3);

                                // ||| Color coding here

                                m = Mesh.ByPointsFaceIndices(MeshPoints, indices);
                                frameResultsMesh.Add(m);
                            }

                            //Clear the temporary list
                            MeshPoints.Clear();

                            // Add the current station's points
                            MeshPoints.Add(tPoint); //new face index 0
                            MeshPoints.Add(vPoint); //new face index 1   
                        }

                        // Update the values for the next station
                        t1 = newt * c.Length;
                        v1 = v2;
                    }

                    // If all the values were zero, show empty list in output for that specific member
                    if (zeroCount == AnalysisResults.FrameResults[j].Results[AnalysisResults.LCaseOrLPatternRun].Keys.Count)
                    {
                        frameResultsMesh.Clear();
                    }

                    VizMeshes.Add(frameResultsMesh);
                    j++;


                }
            }
            return VizMeshes;

        }


        //Results private methods

        private Analysis() { }
        private Analysis(List<FrameResults> fresults, string LcOrLp)
        {
            FrameResults = fresults;
            LCaseOrLPatternRun = LcOrLp;
        }

    }


}
