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

namespace DynamoSAP.Structure
{
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

        public Line BaseCurve
        {
            get { return BaseCrv; }
        }


        //PUBLIC METHODS
        public override string ToString()
        {
            return "Frame";
        }

        // Frame From Curve
        public static Frame FromLine(Line Line, SectionProp SectionProp, string Justification = "MiddleCenter", double Rotation = 0)
        {
            return new Frame(Line, SectionProp, Justification, Rotation);
        }
        // Frame from Nodes
        public static Frame FromEndPoints(Point i, Point j, SectionProp SectionProp, string Justification = "MiddleCenter", double Rotation = 0)
        {
            return new Frame(i, j, SectionProp, Justification, Rotation);
        }

        // Set Custom Releases to Frame
        public static Frame SetReleases(Frame frame, Release Release)
        {
            // Create a new Frame using the properties of the input frame
            Frame newFrame = Frame.FromLine(frame.BaseCurve, frame.SecProp, frame.Just, frame.Angle);
            // Pass the GUID of the existing frame to override it
            // The collector will check if there are duplicate GUIDs
            newFrame.GUID = frame.GUID;
            // Add any loads the frame already has
            newFrame.Loads = frame.Loads;
            // Set the release in the node
            newFrame.Releases = Release;
            return newFrame;

        }

        // Set Loads to Frame
        public static Frame SetLoad(Frame frame, Load Load, bool replaceExisting = false)
        {
            // Create a new Frame using the properties of the input frame
            Frame newFrame = Frame.FromLine(frame.BaseCurve, frame.SecProp, frame.Just, frame.Angle);
            // Pass the GUID of the existing frame to override it
            // The collector will check if there are duplicate GUIDs
            newFrame.GUID = frame.GUID;
            // Add any releases the frame already has
            newFrame.Releases = frame.Releases;
            
            //Set the load in the node
            List<Load> frameLoads = new List<Load>();
            if (frame.Loads != null)
            {
                if (replaceExisting) // if true, delete the list and add the new Load
                {
                    frame.Loads.Clear();
                }
                else
                {
                    foreach (Load l in frame.Loads)
                    {
                        frameLoads.Add(l);
                    }
                }
            }
            frameLoads.Add(Load);
            newFrame.Loads = frameLoads;

            return newFrame;
        }

        // DYNAMO DISPLAY NODES

        // Display Releases
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

        // Display Loads
        public static List<Object> DisplayLoads(Frame Frame, string LPattern = "Show All", double scale = 1.0, bool showValues = true, double textSize = 1.0)
        {
            //List<List<Object>> LoadViz = new List<List<Object>>();
            List<Object> LoadObjects = new List<Object>();
            foreach (Load load in Frame.Loads)
            {
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

                Curve c = Frame.BaseCrv;
                if (load.Val > 0) scale = -scale; // make negative and change the direction of the arrow

                List<double> dd = new List<double>(); // parameter values where arrows will be drawn
                bool isDistributed = false;
                if (load.LoadType == "DistributedLoad")
                {
                    isDistributed = true;
                    //number of arrows to represent a Distributed Load
                    int n = Convert.ToInt32((load.Dist - load.Dist2) / 0.05);
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
                Point A = null;
                Point B = null;
                Vector v2 = null;
                Vector v3 = null;
                Vector xAxis = c.TangentAtParameter(0.0);
                for (int i = 0; i < dd.Count; i++)
                {
                    List<Point> pps = new List<Point>();
                    List<IndexGroup> igs = new List<IndexGroup>();
                    // Line of the arrow
                    Point p1 = c.PointAtParameter(dd[i]);
                    Point p2 = null;
                    Point p3 = null;
                    Point p4 = null;

                    //MUST ADD SOME CONDITION HERE FOR LOCAL OR GLOBAL COORDINATE SYSTEM

                    if (load.Dir == 4) // if it's the X Direction
                    {
                        v2 = Vector.ByCoordinates(20.0 * scale, 0.0, 0.0);
                        v3 = Vector.ByCoordinates(5.0 * scale, 0.0, 0.0);
                    }

                    if (load.Dir == 5) // if it's the Y Direction
                    {
                        v2 = Vector.ByCoordinates(0.0, 20.0 * scale, 0.0);
                        v3 = Vector.ByCoordinates(0.0, 5.0 * scale, 0.0);
                    }
                    if (load.Dir == 6) // if it's the Z Direction
                    {
                        v2 = Vector.ByCoordinates(0.0, 0.0, 20.0 * scale);
                        v3 = Vector.ByCoordinates(0.0, 0.0, 5.0 * scale);
                    }

                    p2 = (Point)p1.Translate(v2);

                    p3 = (Point)p1.Translate(xAxis, 5.0 * scale);
                    p3 = (Point)p3.Translate(v3);

                    p4 = (Point)p1.Translate(xAxis, -5.0 * scale);
                    p4 = (Point)p4.Translate(v3);

                    pps.Add(p1); pps.Add(p3); pps.Add(p4);

                    //Triangle of the arrow
                    igs.Add(IndexGroup.ByIndices(0, 1, 2));
                    Mesh m = Mesh.ByPointsFaceIndices(pps, igs);

                    LoadObjects.Add(Line.ByStartPointEndPoint(p1, p2));
                    LoadObjects.Add(m);
                    if (isDistributed) // create top line
                    {
                        if (i == 0)
                        {
                            A = p2;
                        }
                        else if (i == dd.Count - 1)
                        {
                            B = p2;

                            Line topLine = Line.ByStartPointEndPoint(A, B);
                            LoadObjects.Add(topLine);
                        }
                        else if (i == Convert.ToInt32(dd.Count / 2)) // if it is the middle point
                        {
                            labelLocation = (Point)p2.Translate(v2.Normalized().Scale(20));
                        }
                    }
                    else
                    {
                        labelLocation = (Point)p2.Translate(v2.Normalized().Scale(20));
                    }

                }
                if (showValues)
                {
                    //CREATE LABEL
                    //get the text curves
                    List<Curve> textCurves = new List<Curve>();
                    string value = Math.Round(load.Val, 2).ToString(); // value of the load rounded to two decimals

                    //create ZX Plane to host the text
                    Plane pl = null;
                    if (c.StartPoint.Z == c.EndPoint.Z) //if it is a beam
                    {
                        pl = Plane.ByOriginXAxisYAxis(labelLocation, xAxis, v2);
                    }
                    else //if it is a column or an inclined member
                    {
                        pl = Plane.ByOriginXAxisYAxis(labelLocation, v2, CoordinateSystem.Identity().ZAxis);
                    }

                    //call the function to create the text
                    textCurves = Text.FromStringOriginAndScale(value, pl, textSize).ToList();
                    foreach (Curve textc in textCurves)
                    {
                        LoadObjects.Add(textc);
                    }
                }

                //LoadViz.Add(LoadObjects);
            }

            return LoadObjects;
        }




        // Decompose
        [MultiReturn("BaseCurve", "SectionProp", "Justification", "Rotation", "Loads", "Releases")]
        public static Dictionary<string, object> Decompose(Frame frame)
        {
            // Return outputs
            return new Dictionary<string, object>
            {
                {"BaseCurve", frame.BaseCrv},
                {"SectionProp", frame.SecProp},
                {"Justification", frame.Just},
                {"Rotation", frame.Angle},
                {"Loads", frame.Loads},
                {"Releases", frame.Releases}
            };
        }

        // PRIVATE CONSTRUCTORS
        internal Frame() { }
        internal Frame(Line line, SectionProp secProp, string just, double angle)
        {
            BaseCrv = line;
            Angle = angle;
            SecProp = secProp;
            Just = just;
        }
        internal Frame(Point i, Point j, SectionProp secProp, string just, double angle)
        {
            BaseCrv = Line.ByStartPointEndPoint(i, j);
            Angle = angle;
            SecProp = secProp;
            Just = just;
        }

    }
}
