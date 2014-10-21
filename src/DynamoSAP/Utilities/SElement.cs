using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.DesignScript.Runtime;

namespace DynamoSAP.Utilities
{
    [IsVisibleInDynamoLibrary(false)]
    public class SAPElement
    {
        public string GUID { get; set; }
        public string Label { get; set; }
    }

    [IsVisibleInDynamoLibrary(false)]
    public class SFrame:SAPElement
    { 
        // Curve  class that holds nodes ! (vertices)
        public SCurve BaseCurve { get; set; }

        //matprop
        public string MatProp { get; set; }
        //sectionprop
        public string SecProp { get; set; }
        //justification - follow SAP enum for InsertionPoint set default 5
        public int Just { get; set; } 

        //rotation
        public double Angle { get; set; }

        //CONSTRUCTORS
        public SFrame() { } // default
        public SFrame(string matProp, string secProp, int just, double angle)
        {
            MatProp = matProp;
            SecProp = secProp;
            Just = just;
            Angle = angle;
        }
        public SFrame(SCurve baseCrv, string matProp, string secProp, int just, double angle)
        {
            BaseCurve = baseCrv;
            MatProp = matProp;
            SecProp = secProp;
            Just = just;
            Angle = angle;
        }

    }
    [IsVisibleInDynamoLibrary(false)]
    public class DSType
    {
        public enum SType
        { 
            Frame,
            Area,
            Cable,
            Link
        }  
    }

    //Node and Curve
    [IsVisibleInDynamoLibrary(false)]
    public class SAPNode
    {
        //ATTRIBUTES
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public string ID { get; set; }
    
        //CONSTRUCTOR
        public SAPNode(string id, double x, double y, double z)
        {
            ID = id;
            X = x;
            Y = y;
            Z = z;
        }
        public SAPNode(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }

    [IsVisibleInDynamoLibrary(false)]
    public class SCurve
    {
        //FIELDS//
        private List<SAPNode> myPoints = new List<SAPNode>();
        public List<SAPNode> Points
        {
            get { return myPoints; }
        }


       //CONSTRUCTORS

        //default - empty object
        public SCurve() { }

        //full - set every field w/ constructor
        public SCurve(List<SAPNode> vertices)
        {
            myPoints = vertices;
        }
    
    }
}
