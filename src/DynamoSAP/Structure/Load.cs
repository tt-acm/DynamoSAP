using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Autodesk.DesignScript.Geometry;
using DynamoSAP.Structure;
using DynamoSAP.Assembly;


namespace DynamoSAP.Structure
{

    public class Load
    {
        //Load Type
        internal string LoadType;

        //Frame Name
        internal Frame frm { get; set; }
        //Load Pattern
        internal LoadPattern lPattern { get; set; }
        //My Type
        internal int MyType { get; set; }
        //Direction
        internal int Dir { get; set; }
        //Distance
        internal double Dist { get; set; }
        //Distance2
        internal double Dist2 { get; set; }
        //Value
        internal double Val { get; set; }
        //Value2
        internal double Val2 { get; set; }

        //Optional inputs
        internal string CSys;
        internal bool RelDist;
        internal bool Replace;

        //using this input from SAP causes an error in the compiler, it is optional...
        // internal static eItemType MyeItemType;

        //DYNAMO QUERY NODES
        public string LType { get { return LoadType; } }
        public Frame Frame { get { return frm; } }
        public LoadPattern Pattern { get { return lPattern; } }
        public int FMType { get { return MyType; } }
        public int Direction { get { return Dir; } }
        public string Distance1 { get { return Dist.ToString(); } }
        public string Distance2
        {
            get
            {
                string d2 = ""; // if it is a Point Load this should return empty
                if (LoadType == "DistributedLoad") d2 = Dist2.ToString();  //if it is a Distributed Load
                return d2;
            }
        }
        public string Value1 { get { return Val.ToString(); } }
        public string Value2
        {
            get
            {
                string v2 = "";// if it is a Point Load this should return empty
                if (LoadType == "DistributedLoad") v2 = Val2.ToString();  //if it is a Distributed Load
                return v2;
            }
        }

        /// <summary>
        /// This function assigns loads to frame objects.
        /// Parameters description below as presented in the SAP CSi OAPI Documentation
        /// </summary>
        /// <param name="FrameName">The name of an existing frame object or group, depending on the value of the ItemType item.</param>
        /// <param name="LoadPat">The name of a defined load pattern.</param>
        /// <param name="MyType">This is 1 or 2, indicating the type of point load.
        /// 1 = Force
        /// 2 = Moment</param>
        /// <param name="Dir">This is an integer between 1 and 11, indicating the direction of the load.
        /// 1 = Local 1 axis (only applies when CSys is Local)
        /// 2 = Local 2 axis (only applies when CSys is Local)
        /// 3 = Local 3 axis (only applies when CSys is Local)
        /// 4 = X direction (does not apply when CSys is Local)
        /// 5 = Y direction (does not apply when CSys is Local)
        /// 6 = Z direction (does not apply when CSys is Local)
        /// 7 = Projected X direction (does not apply when CSys is Local)
        /// 8 = Projected Y direction (does not apply when CSys is Local)
        /// 9 = Projected Z direction (does not apply when CSys is Local)
        /// 10 = Gravity direction (only applies when CSys is Global)
        /// 11 = Projected Gravity direction (only applies when CSys is Global)
        /// The positive gravity direction (see Dir = 10 and 11) is in the negative Global Z direction.</param>
        /// <param name="Dist">This is the distance from the I-End of the frame object to the load location. 
        /// This may be a relative distance (0 less or equal to Dist less or equal to 1) or an actual distance, 
        /// depending on the value of the RelDist item. [L] when RelDist is False</param>
        /// <param name="Val">This is the value of the point load. [F] when MyType is 1 and [FL] when MyType is 2</param>
        /// <param name="CSys">This is Local or the name of a defined coordinate system. 
        /// It is the coordinate system in which the loads are specified.</param>
        /// <param name="RelDist">If this item is True, the specified Dist item is a relative distance, 
        /// otherwise it is an actual distance.</param>
        /// <param name="Replace">If this item is True, all previous loads, if any, assigned to the specified frame object(s), 
        /// in the specified load pattern, are deleted before making the new assignment.</param>
        /// <returns></returns>

        // ItemType
        //This is one of the following items in the eItemType enumeration:
        //Object = 0
        //Group = 1
        //SelectedObjects = 2
        //If this item is Object, the assignment is made to the frame object specified by the Name item.
        //If this item is Group, the assignment is made to all frame objects in the group specified by the Name item.
        //If this item is SelectedObjects, assignment is made to all selected frame objects, and the Name item is ignored.

        //PUBLIC METHODS
        public override string ToString()
        {
            if (LoadType == "DistributedLoad")
            {
                return "DistributedLoad";
            }
            else
            {
                return "PointLoad";
            }
        }

        //DYNAMO CREATE NODES
        public static Load PointLoadOnFrame(Frame Frame, LoadPattern LPattern, int Type, int Dir, double Dist, double Val, string CSys = "Global", bool RelDist = true, bool Replace = true)
        {
            Load l = new Load(Frame, LPattern, Type, Dir, Dist, Val, CSys, RelDist, Replace);
            l.LoadType = "PointLoad";
            return l;
        }

        public static Load DistributedLoadOnFrame(Frame Frame, LoadPattern LPattern, int Type, int Dir, double Dist, double Dist2, double Val, double Val2, string CSys = "Global", bool RelDist = true, bool Replace = true)
        {
            Load l = new Load(Frame, LPattern, Type, Dir, Dist, Dist2, Val, Val2, CSys, RelDist, Replace);
            l.LoadType = "DistributedLoad";
            return l;
        }

        public static List<List<Object>> Display(StructuralModel StructuralModel, string LPattern="",double scale = 1.0)
        {
            List<List<Object>> LoadViz = new List<List<Object>>();

            foreach (Load l in StructuralModel.Loads)
            {
                if (LPattern == "")
                {
                   //show all
                }
                else
                {
                    if (l.lPattern.Name != LPattern)
                    {
                        continue; // show only the loads whose load pattern is the specified in the node
                    }
                }
                List<Object> lo = new List<Object>();
                Frame f = l.Frame;
                Curve c = f.BaseCrv;
                List<double> dd = new List<double>();
                bool isDistributed = false;
                if (l.LoadType == "DistributedLoad")
                {
                    isDistributed = true;
                    //number of arrows to represent a Distributed Load
                    int n = Convert.ToInt32((l.Dist - l.Dist2) / 0.05);
                    double step = (l.Dist - l.Dist2) / n;

                    for (double i = l.Dist; i < l.Dist2; i += step)
                    {
                        dd.Add(i);
                    }
                    dd.Add(l.Dist2);
                }
                else
                {
                    dd.Add(l.Dist);
                }
                Point A = null;
                Point B = null;
                for (int i = 0; i < dd.Count; i++)
                {
                    List<Point> pps = new List<Point>();
                    List<IndexGroup> igs = new List<IndexGroup>();

                    Vector xAxis = c.TangentAtParameter(0.0);

                    // Line of the arrow
                    Point p1 = c.PointAtParameter(dd[i]);

                    Point p2 = null;
                    Point p3 = null;
                    Point p4 = null;

                    //MUST ADD SOME CONDITION HERE FOR LOCAL OR GLOBAL COORDINATE SYSTEM
                    Vector v2=null;
                    Vector v3 = null;
                    if (l.Dir == 4) // if it's the X Direction
                    {
                        v2 = Vector.ByCoordinates(20.0*scale, 0.0, 0.0);
                        v3 = Vector.ByCoordinates(5.0 * scale, 0.0, 0.0);
                        
                        
                    }

                    if (l.Dir == 5) // if it's the Y Direction
                    {
                        v2 = Vector.ByCoordinates(0.0, 20.0 * scale, 0.0);
                        v3 = Vector.ByCoordinates(0.0, 5.0 * scale, 0.0);


                    }
                    if (l.Dir == 6) // if it's the Z Direction
                    {
                        v2 = Vector.ByCoordinates(0.0, 0.0, 20.0 * scale);
                        v3=Vector.ByCoordinates(0.0, 0.0, 5.0*scale);
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

                    lo.Add(Line.ByStartPointEndPoint(p1, p2));
                    lo.Add(m);
                    if (isDistributed)
                    {
                        if (i == 0)
                        {
                            A = p2;
                        }
                        else if (i == dd.Count - 1)
                        {
                            B = p2;
                            //Top line
                            Line topLine = Line.ByStartPointEndPoint(A, B);
                            lo.Add(topLine);
                        }
                    }
                }
                LoadViz.Add(lo);
            }

            return LoadViz;
        }

        //PRIVATE CONSTRUCTORS
        private Load() { }

        //constructor for PointLoads
        private Load(Frame frame, LoadPattern loadPat, int myType, int dir, double dist, double val, string cSys, bool relDist, bool replace)
        {
            frm = frame;
            lPattern = loadPat;
            MyType = myType;
            Dir = dir;
            Dist = dist;
            Val = val;
            CSys = cSys;
            RelDist = relDist;
            Replace = replace;
        }

        //constructor for DistributedLoads
        private Load(Frame frame, LoadPattern loadPat, int myType, int dir, double dist, double dist2, double val, double val2, string cSys, bool relDist, bool replace)
        {
            frm = frame;
            lPattern = loadPat;
            MyType = myType;
            Dir = dir;
            Dist = dist;
            Dist2 = dist2;
            Val = val;
            Val2 = val2;
            CSys = cSys;
            RelDist = relDist;
            Replace = replace;
        }


    }
}
