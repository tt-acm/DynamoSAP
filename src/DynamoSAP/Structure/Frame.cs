using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.DesignScript.Geometry;

namespace DynamoSAP.Structure
{
    public class Frame:Element
    {
        // FIELDS

        // Line
        internal Line BaseCrv { get; set; }
        //matprop
        internal string MatProp { get; set; }
        //sectionprop
        internal string SecProp { get; set; }
        //justification - follow SAP enum for InsertionPoint set default 5
        internal string Just { get; set; } 
        //rotation
        internal double Angle { get; set; }


        // QUERY NODES

        public Line BaseCurve
        {
            get { return BaseCrv; }
        }

        public string Material
        {
            get { return MatProp; }
        }

        public string SectionProfile
        {
            get { return SecProp; }
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
        public static Frame FromLine(Line Line, string MatProp = "Steel", string SecProp = "W12X14", string Just = "MiddleCenter", double Rot = 0)
        {
            return new Frame(Line, MatProp, SecProp, Just, Rot);
        }
        // Frame from Nodes
        public static Frame FromEndPoints(Point i, Point j, string MatProp = "Steel", string SecProp = "W12X14", string Just = "MiddleCenter", double Rot = 0)
        {
            return new Frame(i, j, MatProp, SecProp, Just, Rot);
        }

         // PRIVATE CONSTRUCTORS
        private Frame(){}
        private Frame(Line line,string matProp, string secProp, string just, double angle)
        {
            BaseCrv = line;
            Angle = angle;
            MatProp = matProp;
            SecProp= secProp;
            Just = just;
        }
        private Frame(Point i, Point j, string matProp, string secProp, string just, double angle)
        {
            BaseCrv = Line.ByStartPointEndPoint(i, j);
            Angle = angle;
            MatProp = matProp;
            SecProp = secProp;
            Just = just;
        }

    }
}
