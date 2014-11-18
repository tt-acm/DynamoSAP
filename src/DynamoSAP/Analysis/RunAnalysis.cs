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

        public static Analysis GetResults(StructuralModel Model, string LoadCase, bool Run)
        {
            List<FrameResults> frameResults = null;
            Analysis StructureResults = new Analysis();
            if (Run)
            {
                // loop over frames get results and populate to dictionary
                frameResults = SAPConnection.AnalysisMapper.GetFrameForces(ref mySapModel, LoadCase);
                StructureResults.FrameResults = frameResults;
            }
            return StructureResults;
        }

        public static List<double> DecomposeResults(Analysis AnalysisResults, string ForceType, string loadcase, int FrameID)
        {

            FrameID -= 1; // SAP starts numbering elements by 1, but the first dictionary in the list is in index 0

            List<double> Forces = new List<double>();
            foreach (FrameAnalysisData fad in AnalysisResults.FrameResults[FrameID].Results[loadcase].Values)
            {
                if (ForceType == "Axial") //Get Axial Forces P
                {
                    Forces.Add(fad.P);
                }
                else if (ForceType == "Shear22") // Get Shear V2
                {
                    Forces.Add(fad.V2);
                }
                else if (ForceType == "Shear33") // Get Shear V3
                {
                    Forces.Add(fad.V3);
                }

                else if (ForceType == "Torsion") // Get Torsion T
                {
                    Forces.Add(fad.T);
                }

                else if (ForceType == "Moment22") // Get Moment M2
                {
                    Forces.Add(fad.M2);
                }
                else if (ForceType == "Moment33") // Get Moment M3
                {
                    Forces.Add(fad.M3);
                }
            }

            return Forces;
        }

        public static List<Point> VisualizeResults(StructuralModel Model, Analysis AnalysisResults, string ForceType, string loadcase, List<int> FrameIDs, double scale)
        {
            List<Mesh> myVizMeshes = new List<Mesh>();
            List<Line> myLines = new List<Line>();
            List<Point> ptTest = new List<Point>();
            foreach (int id in FrameIDs)
            {
                List<PolySurface> subVizSurface = new List<PolySurface>();
                int fid = id - 1; // SAP starts numbering elements by 1, but the first dictionary in the list is in index 0

                List<Curve> crossSections = new List<Curve>();
                // get the frame's curve specified by the frameID
                Frame f = (Frame)Model.Frames[fid];
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

                //PolySurface loftSurface = null;
                List<Point> crossSectionPoints = new List<Point>();
                List<IndexGroup> indices = new List<IndexGroup>();
                int count = 0;
                Mesh m = null;
                foreach (double t in AnalysisResults.FrameResults[fid].Results[loadcase].Keys)
                {

                    IndexGroup ig = null;
                    
                    count += 1;

                    Point cPoint = c.PointAtParameter(t); // curve Point
                    Point vPoint = null; // value Point

                    double translateCoord = 0.0;

                    if (ForceType == "Axial") // Get Axial P
                    {
                        translateCoord = AnalysisResults.FrameResults[fid].Results[loadcase][t].P * -scale;
                    }

                    else if (ForceType == "Shear22") // Get Shear V2
                    {
                        translateCoord = AnalysisResults.FrameResults[fid].Results[loadcase][t].V2 * -scale;
                    }
                    else if (ForceType == "Shear33") // Get Shear V3
                    {
                        translateCoord = AnalysisResults.FrameResults[fid].Results[loadcase][t].V3 * scale;
                    }

                    else if (ForceType == "Torsion") // Get Torsion T
                    {
                        translateCoord = AnalysisResults.FrameResults[fid].Results[loadcase][t].T * scale;
                    }

                    else if (ForceType == "Moment22") // Get Moment M2
                    {
                        translateCoord = AnalysisResults.FrameResults[fid].Results[loadcase][t].M2 * scale;
                    }

                    else if (ForceType == "Moment33") // Get Moment M3
                    {
                        translateCoord = AnalysisResults.FrameResults[fid].Results[loadcase][t].M3 * scale;
                    }

                    //bool signChange = false;
                    double d1 = 0.0;
                    double d2 = 0.0;
                    double pZ=0.0;

                    if (ForceType == "Moment22")
                    {
                        vPoint = (Point)cPoint.Translate(localCS.YAxis, translateCoord); // Translate in the Y direction to match the visualization of SAP
                        if (crossSectionPoints.Count > 0)
                        {
                            d1 = vPoint.Y;
                            d2 = crossSectionPoints[crossSectionPoints.Count - 1].Y;
                            pZ = cPoint.Y;
                        }
                    }
                    else
                    {
                        vPoint = (Point)cPoint.Translate(localCS.ZAxis, translateCoord); // All the other types must be translate in the Z direction} 
                        if (crossSectionPoints.Count > 0)
                        {
                            d1 = vPoint.Z;
                            d2 = crossSectionPoints[crossSectionPoints.Count - 1].Z;

                            pZ = cPoint.Z;
                        }
                    }

                    if (crossSectionPoints.Count == 0)
                    {
                        crossSectionPoints.Add(cPoint); //index 0
                        crossSectionPoints.Add(vPoint); //index 1

                    }

                    else// if a previous point has been added
                    {
                        if (count != AnalysisResults.FrameResults[fid].Results[loadcase].Keys.Count) // if it's not the end of the list
                        {
                            if ((d2 > pZ && d1 < pZ) || (d2 < pZ && d1 > pZ)) // if there is a change in the force sign
                            {
                                crossSectionPoints.Add(cPoint); //index 2 
                                crossSectionPoints.Add(vPoint); //index 3                         
                                //color change here

                            }
                            else
                            {
                                crossSectionPoints.Add(vPoint); //index 2 (note: vPoint before cPoint)
                                crossSectionPoints.Add(cPoint); //index 3 
                            }

                           

                            crossSectionPoints.Add(cPoint); //new face index 0
                            crossSectionPoints.Add(vPoint); //new face index 1

                            ig = IndexGroup.ByIndices(0, 1, 2, 3);
                            indices.Add(ig);
                        }
                        else
                        {
                            crossSectionPoints.Add(vPoint); //index 2 (note: vPoint before cPoint)
                            crossSectionPoints.Add(cPoint); //index 3 

                        }
                                               
                           
                        
                        
                    }







                    

                }

                try
                {
                    m = Mesh.ByPointsFaceIndices(crossSectionPoints, indices);
                }
                catch { }

                myVizMeshes.Add(m);
                ptTest = crossSectionPoints;

            }
            return ptTest;

        }

        //Results private methods
        private Analysis() { }
        private Analysis(List<FrameResults> fresults)
        {
            FrameResults = fresults;

        }



    }


}
