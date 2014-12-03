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
    public class Frame:Element
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

        internal Release Releases { get; set; }

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
        public static Frame FromLine(Line Line, SectionProp SectionProp , string Justification = "MiddleCenter", double Rotation = 0)
        {
            return new Frame(Line, SectionProp, Justification , Rotation);
        }
        // Frame from Nodes
        public static Frame FromEndPoints(Point i, Point j, SectionProp SectionProp, string Justification = "MiddleCenter", double Rotation = 0)
        {
            return new Frame(i, j, SectionProp, Justification, Rotation);
        }

        // Set Custom Releases to Frame
        public static Frame SetReleases(Frame frm, Release Release)
        {
            frm.Releases = Release;
            return frm;
        }

        // Display Nodes
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

        // Decompose
        [MultiReturn("BaseCurve", "SectionProp", "Justification", "Rotation")]
        public static Dictionary<string, object> Decompose(Frame frame)
        {
            // Return outputs
            return new Dictionary<string, object>
            {
                {"BaseCurve", frame.BaseCrv},
                {"SectionProp", frame.SecProp},
                {"Justification", frame.Just},
                {"Rotation", frame.Angle}
            };    
        }

         // PRIVATE CONSTRUCTORS
        internal Frame(){}
        internal Frame(Line line, SectionProp secProp, string just, double angle)
        {
            BaseCrv = line;
            Angle = angle;
            SecProp= secProp;
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
