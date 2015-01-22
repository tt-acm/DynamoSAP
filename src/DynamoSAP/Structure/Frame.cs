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
using DynamoSAP.Definitions;

namespace DynamoSAP.Structure
{
    [DSNodeServices.RegisterForTrace]
    public class Frame : Element
    {
        // FIELDS

        // Line
        internal Line BaseCrv { get; set; }
        //sectionprop
        internal SectionProp SecProp { get; set; }
        //justification - follow SAP enum for InsertionPoint set default 5
        internal string Just { get; set; }
        //rotation
        internal double Angle { get; set; }
        // Releases if not set SAP draws as no releases
        internal Release Releases { get; set; }
        internal List<Load> Loads { get; set; }


        // QUERY NODES
        /// <summary>
        /// Curve representing the frame
        /// </summary>
        public Line BaseCurve
        {
            get { return BaseCrv; }
        }

        /// <summary>
        /// ID of the Curve
        /// </summary>
        internal int ID { get; private set; }


        //PUBLIC METHODS
        /// <summary>
        /// Unique Label
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            //return String.Format("FrameID: {0}", ID.ToString());  
            return Label;
        }


        /// <summary>
        /// Create a Frame from a line
        /// </summary>
        /// <param name="Line">Line to create a frame</param>
        /// <param name="SectionProp">Section properties of the frame</param>
        /// <param name="Justification">Justification of the frame</param>
        /// <param name="Rotation">Angle rotation of the frame</param>
        /// <returns>Frame with all the properties set up by the inputs</returns>
        public static Frame FromLine(Line Line, SectionProp SectionProp, string Justification = "MiddleCenter", double Rotation = 0)
        {
            Frame tFrm;
            FrmID tFrmid = DSNodeServices.TraceUtils.GetTraceData(TRACE_ID) as FrmID;

            if (tFrmid == null)
            {
                // trace cache log didnot return an object, create new one !
                tFrm = new Frame(Line, SectionProp, Justification, Rotation);
                // Set Label
                tFrm.Label = String.Format("dyn_{0}", tFrm.ID.ToString());
            }
            else
            {
                tFrm = TracedFrameManager.GetFramebyID(tFrmid.IntID);

                tFrm.BaseCrv = Line;
                tFrm.SecProp = SectionProp;
                tFrm.Just = Justification;
                tFrm.Angle = Rotation;
            }

            //Set the trace data on the return to be this Frame
            DSNodeServices.TraceUtils.SetTraceData(TRACE_ID, new FrmID { IntID = tFrm.ID });

            return tFrm;
        }

        /// <summary>
        /// Create a Frame from the end points
        /// </summary>
        /// <param name="i">Start Point</param>
        /// <param name="j">End Point</param>
        /// <param name="SectionProp">Section properties of the frame</param>
        /// <param name="Justification">Justification of the frame</param>
        /// <param name="Rotation">Angle rotation of the frame</param>
        /// <returns>Frame with all the properties set up by the inputs</returns>
        public static Frame FromEndPoints(Point i, Point j, SectionProp SectionProp, string Justification = "MiddleCenter", double Rotation = 0)
        {
            Frame tFrm;
            FrmID tFrmid = DSNodeServices.TraceUtils.GetTraceData(TRACE_ID) as FrmID;

            if (tFrmid == null)
            {
                // trace cache log didnot return an object, create new one !
                tFrm = new Frame(i, j, SectionProp, Justification, Rotation);
                // Set Label
                tFrm.Label = String.Format("dyn_{0}", tFrm.ID.ToString());
            }
            else
            {
                tFrm = TracedFrameManager.GetFramebyID(tFrmid.IntID);

                tFrm.BaseCrv = Line.ByStartPointEndPoint(i, j);
                tFrm.SecProp = SectionProp;
                tFrm.Just = Justification;
                tFrm.Angle = Rotation;
            }

            //Set the trace data on the return to be this Frame
            DSNodeServices.TraceUtils.SetTraceData(TRACE_ID, new FrmID { IntID = tFrm.ID });

            return tFrm;
        }

        /// <summary>
        /// Set the Releases of a frame
        /// </summary>
        /// <param name="Frame">Frame to set up releases of</param>
        /// <param name="Release">Use the Release node</param>
        /// <returns>The frame with the new releases</returns>
        public static Frame SetReleases(Frame Frame, Release Release)
        {
            // Create a new Frame using the properties of the input frame
            Frame newFrm = Frame.FromLine(Frame.BaseCurve, Frame.SecProp, Frame.Just, Frame.Angle);
            // Set Label
            newFrm.Label = String.Format("dyn_{0}", newFrm.ID.ToString());
            // Add any loads the frame already has
            newFrm.Loads = Frame.Loads;
            // Set the release in the node
            newFrm.Releases = Release;
            return newFrm;
        }

        /// <summary>
        /// Set a Load on a Frame
        /// </summary>
        /// <param name="Frame">Frame to set up loads on</param>
        /// <param name="Load">Load to apply to the frame</param>
        /// <param name="replaceExisting">Set Boolean to True to replace existing Loads on the Frame</param>
        /// <returns>The frame with the new loads(and the previous ones, if it applies)</returns>
        public static Frame SetLoad(Frame Frame, Load Load, bool replaceExisting = false)
        {
            // Create a new Frame using the properties of the input frame
            Frame newFrm = Frame.FromLine(Frame.BaseCurve, Frame.SecProp, Frame.Just, Frame.Angle);
            // Set Label
            newFrm.Label = String.Format("dyn_{0}", newFrm.ID.ToString());
            // Add any releases the frame already has
            newFrm.Releases = Frame.Releases;

            //Set the load in the node
            List<Load> frameLoads = new List<Load>();
            if (Frame.Loads != null)
            {
                if (replaceExisting) // if true, delete the list and add the new Load
                {
                    Frame.Loads.Clear();
                }
                else
                {
                    foreach (Load l in Frame.Loads)
                    {
                        frameLoads.Add(l);
                    }
                }
            }
            frameLoads.Add(Load);
            newFrm.Loads = frameLoads;

            return newFrm;
        }

        // DYNAMO DISPLAY NODES

        /// <summary>
        /// Display the Releases of a Frame
        /// </summary>
        /// <param name="frame">Frame to display the releases on (if any)</param>
        /// <param name="radius">Radius of the spheres </param>
        /// <returns>Spheres representing the Releases</returns>
        public static List<Sphere> DisplayReleases(Frame frame, double radius = 10.0)
        {
            List<Sphere> rSpheres = new List<Sphere>();
            if (frame.Releases.u1i == true || frame.Releases.u2i == true || frame.Releases.u3i == true || frame.Releases.r1i == true || frame.Releases.r2i == true || frame.Releases.r3i == true)
            {
                Sphere s = Sphere.ByCenterPointRadius(frame.BaseCrv.PointAtParameter(0.15), radius);
                rSpheres.Add(s);
            }
            else if (frame.Releases.u1j == true || frame.Releases.u2j == true || frame.Releases.u3j == true || frame.Releases.r1j == true || frame.Releases.r2j == true || frame.Releases.r3j == true)
            {
                Sphere s = Sphere.ByCenterPointRadius(frame.BaseCrv.PointAtParameter(0.85), radius);
                rSpheres.Add(s);
            }
            return rSpheres;
        }

        /// <summary>
        /// Display the Loads
        /// </summary>
        /// <param name="StructuralModel">Structural Model</param>
        /// <param name="LPattern">Load Pattern to display</param>
        /// <param name="Size">Size of the arrows</param>
        /// <param name="ShowValues">Set Boolean to True to show the tags of the numeric values</param>
        /// <param name="TextSize">Size of the tags</param>
        /// <returns>Arrows and tags representing the loads</returns>
        public static List<List<Object>> DisplayLoads(StructuralModel StructuralModel, string LPattern = "Show All", double Size = 1.0, bool ShowValues = true, double TextSize = 1.0)
        {
            //List to hold all the load visualization objects
            List<List<Object>> LoadViz = new List<List<Object>>();
            
            // Length of the arrow
            Double length = 1.0;
            foreach (Element e in StructuralModel.StructuralElements)
            {
                if (e.GetType().ToString().Contains("Frame"))
                {
                    //get the length of the first frame found in the collection and use to set up a scale for the load display
                    length = ((Frame)e).BaseCrv.Length;
                    break;
                }
            }
            double arrowLenght = length / 6;
            
            // Loop through all the elements in the structural model
            foreach (Element e in StructuralModel.StructuralElements)
            {
                double sz = Size;
                //List to hold the load visualization objects per structural member
                List<Object> LoadObjects = new List<Object>();
                
                // 1. If the object is a frame
                if (e.GetType().ToString().Contains("Frame"))
                {
                    Frame f = e as Frame;
                    if (f.Loads != null && f.Loads.Count > 0)
                    {
                        //Loop through all the loads on the frame
                        foreach (Load load in f.Loads)
                        {
                            // If the Load type is  a moment, throw warning
                            if (load.FMType == 2) throw new Exception("Moment visualization is not supported");

                            Point labelLocation = null;
                            if (LPattern == "Show All")
                            {
                                //show all
                            }
                            else
                            {
                                if (load.lPattern.name != LPattern)
                                {
                                    continue; // show only the loads whose load pattern is the specified in the node
                                }
                            }

                            Curve c = f.BaseCrv;
                            if (load.Val > 0) sz = -sz; // make negative and change the direction of the arrow

                            // List to hold parameter values where arrows will be drawn
                            List<double> dd = new List<double>(); 
                            bool isDistributed = false;
                            if (load.LoadType == "DistributedLoad")
                            {
                                isDistributed = true;
                                //number of arrows to represent a Distributed Load
                                int n = Convert.ToInt32((load.Dist - load.Dist2) / 0.1);
                                double step = (load.Dist - load.Dist2) / n;

                                for (double i = load.Dist; i < load.Dist2; i += step)
                                {
                                    dd.Add(i);
                                }
                                dd.Add(load.Dist2);
                            }
                            else // if its a point load
                            {
                                dd.Add(load.Dist);
                            }
                            //First top point for distributed load visualization
                            Point A = null;
                            //Last top point for distributed load visualization
                            Point B = null;
                            
                            //Vector used to translate the arrow location point
                            Vector v = null;

                            Vector xAxis = c.TangentAtParameter(0.0);
                            Vector yAxis = c.NormalAtParameter(0.0);
                            Vector triangleNormal = null;
                            
                            //List to hold the index group of the mesh
                            List<IndexGroup> igs = new List<IndexGroup>();
                            igs.Add(IndexGroup.ByIndices(0, 1, 2));
                            
                            //Loop through all the parameter values along the curve.
                            // If it is a point load it will only have one value
                            for (int i = 0; i < dd.Count; i++)
                            {
                                //List to hold the points to create a triangular mesh per arrow
                                List<Point> pps = new List<Point>();

                                //Create the point where the arrow should be located
                                Point p1 = c.PointAtParameter(dd[i]);
                                Point p2 = null;
                                Point p3 = null;
                                Point p4 = null;
  
                                //Calculate the vector needed to create the line of the arrow
                                // if it's the local X Direction
                                if (load.Dir == 1) 
                                {
                                    v = xAxis;
                                }
                                // if it's the local Y Direction
                                else if (load.Dir == 2) 
                                {
                                    v = yAxis;
                                }
                                // if it's the local Z Direction
                                else if (load.Dir == 3) 
                                {
                                    v = xAxis.Cross(yAxis);
                                }
                                // if it's the global X Direction
                                else if (load.Dir == 4) 
                                {
                                    v = Vector.ByCoordinates(arrowLenght * sz, 0.0, 0.0);
                                }
                                // if it's the global Y Direction
                                else if (load.Dir == 5) 
                                {
                                    v = Vector.ByCoordinates(0.0, arrowLenght * sz, 0.0);
                                }
                                // if it's the global Z Direction
                                else if (load.Dir == 6) 
                                {
                                    v = Vector.ByCoordinates(0.0, 0.0, arrowLenght * sz);
                                }

                                // Create the line of the arrow
                                p2 = (Point)p1.Translate(v);
                                Line ln=Line.ByStartPointEndPoint(p1, p2);

                                // Create a temporary point to hold the position of the base of the triangle of the arrow
                                Point ptOnArrow = ln.PointAtDistance(arrowLenght/5);
                                triangleNormal = ln.Normal;
                                double triangleBase=0.10/1.25;
                                
                                // Translate the point on the arrow to the sides to create the base of the triangle
                                p3 = (Point)ptOnArrow.Translate(triangleNormal,triangleBase);
                                p4 = (Point)ptOnArrow.Translate(triangleNormal,-triangleBase);

                                // Add the points to the list
                                pps.Add(p1); pps.Add(p3); pps.Add(p4);

                                //Create the triangular mesh of the arrow
                                Mesh m = Mesh.ByPointsFaceIndices(pps, igs);

                                //Add the arrow objects to the list
                                LoadObjects.Add(ln);
                                LoadObjects.Add(m);

                                // Calculate the location of the labels                                
                                if (isDistributed) 
                                {
                                    if (i == 0)
                                    {
                                        A = p2;
                                    }
                                    else if (i == dd.Count - 1)
                                    {
                                        B = p2;
                                        //If it is a distributed load, create a top line
                                        Line topLine = Line.ByStartPointEndPoint(A, B);
                                        LoadObjects.Add(topLine);
                                    }
                                    else if (i == Convert.ToInt32(dd.Count / 2)) // if it is the middle point
                                    {
                                        labelLocation = (Point)p2.Translate(v.Normalized().Scale(arrowLenght/4));
                                    }
                                }
                                else
                                {
                                    labelLocation = (Point)p2.Translate(v.Normalized().Scale(arrowLenght/4));
                                }
                            }
                            
                            // If the user wants to see the values of the forces
                            if (ShowValues)
                            {
                                //List to hold the curves of the label
                                List<Curve> textCurves = new List<Curve>();
                                //Create label
                                string value = Math.Round(load.Val, 2).ToString(); // value of the load rounded to two decimals

                                //create XZ Plane to host the text
                                Plane pl = Plane.ByOriginXAxisYAxis(labelLocation, CoordinateSystem.Identity().XAxis, CoordinateSystem.Identity().ZAxis);

                                // Call the function to create the text
                                textCurves = Text.FromStringOriginAndScale(value, pl, TextSize).ToList();
                                foreach (Curve textc in textCurves)
                                {
                                    LoadObjects.Add(textc);
                                }
                            }
                        }
                    }
                }

                //TO DO:
                // 2. Add condition for cable

                // 3. Add contiditon for shell

                LoadViz.Add(LoadObjects);
            }

            // Return Load Visualization
            return LoadViz;
        }


        // DECOMPOSE NODE

        /// <summary>
        /// Decompose a Frame into its geometry and settings
        /// </summary>
        /// <param name="Frame">Frame to decompose</param>
        /// <returns>Base Curve, Section Properties, Justification, Angle Roation, Loads and Releases of a Frame</returns>
        [MultiReturn("BaseCurve", "SectionProp", "Justification", "Rotation", "Loads", "Releases")]
        public static Dictionary<string, object> Decompose(Frame Frame)
        {
            if (Frame.Type == Structure.Type.Frame)
            {
                // Return outputs
                return new Dictionary<string, object>
                {
                    {"BaseCurve", Frame.BaseCrv},
                    {"SectionProp", Frame.SecProp},
                    {"Justification", Frame.Just},
                    {"Rotation", Frame.Angle},
                    {"Loads", Frame.Loads},
                    {"Releases", Frame.Releases}
                };
            }
            return null;
        }


        // PRIVATE CONSTRUCTORS
        internal Frame() { }
        internal Frame(Line line, SectionProp secProp, string just, double angle)
        {
            BaseCrv = line;
            Angle = angle;
            SecProp = secProp;
            Just = just;
            this.Type = Structure.Type.Frame;

            //register for cache
            ID = TracedFrameManager.GetNextUnusedID();
            TracedFrameManager.RegisterFrmforID(ID, this);
        }
        internal Frame(Point i, Point j, SectionProp secProp, string just, double angle)
        {
            BaseCrv = Line.ByStartPointEndPoint(i, j);
            Angle = angle;
            SecProp = secProp;
            Just = just;
            this.Type = Structure.Type.Frame;

            //register for cache
            ID = TracedFrameManager.GetNextUnusedID();
            TracedFrameManager.RegisterFrmforID(ID, this);
        }

    }
}
