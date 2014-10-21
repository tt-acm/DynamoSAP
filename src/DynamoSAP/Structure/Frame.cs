using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DynamoSAP.Utilities;
using Autodesk.DesignScript.Geometry;

namespace DynamoSAP.Structure
{
    public class Frame
    {
        // FIELDS
        // Curve  class that holds nodes ! (vertices)
        private Line BaseCrv { get; set; }

        //matprop
        private string MatProp { get; set; }
        //sectionprop
        private string SecProp { get; set; }
        //justification - follow SAP enum for InsertionPoint set default 5
        private int Just { get; set; } 
        //rotation
        private double Angle { get; set; }


        public static double Rotation (Frame f)
        {
            return f.Angle;
        }

        public static Line BaseCurve (Frame f)
        {
            return f.BaseCrv;
        }

        public static string getType(object o)
        {
            return o.GetType().ToString();
        }
        

        // STATIC CONSTRUCTORS

        public static Frame FromCurve(Line Line, string matProp, string secProp, int just, double angle)
        {
            return new Frame(Line,matProp, secProp, just, angle);
        }

        
        private Frame(){}
        private Frame(Line line,string matProp, string secProp, int just, double angle)
        {
            BaseCrv = line;
            Angle = angle;
            MatProp = matProp;
            SecProp= secProp;
            Just = just;
        }

    }
}
