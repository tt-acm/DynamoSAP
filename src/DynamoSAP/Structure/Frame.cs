using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.DesignScript.Geometry;
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


        // QUERY NODES

        public Line BaseCurve
        {
            get { return BaseCrv; }
        }

        public string Justification
        {
            get { return Just; }
        }

        public double Rotation
        { 
            get{ return Angle;}
        }

        public string guid
        {
            get { return GUID; }
        }

        //PUBLIC METHODS
        public override string ToString()
        {
            return "Frame";
        }
 

        // Frame From Curve
        public static Frame FromLine(Line Line, SectionProp SecProp , string Just = "MiddleCenter", double Rot = 0)
        {
            return new Frame(Line, SecProp, Just, Rot);
        }
        // Frame from Nodes
        public static Frame FromEndPoints(Point i, Point j, SectionProp SecProp, string Just = "MiddleCenter", double Rot = 0)
        {
            return new Frame(i, j, SecProp, Just, Rot);
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
