using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Autodesk.DesignScript.Geometry;
using DynamoSAP.Structure;
using DynamoSAP.Assembly;
using Autodesk.DesignScript.Runtime;


namespace DynamoSAP.Definitions
{
    /// <summary>
    /// Load
    /// </summary>
    public class Load
    {
        //Load Type
        internal string LoadType;

        //Load Pattern
        internal LoadPattern lPattern { get; set; }
        //My Type
        internal int FMType { get; set; }
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


        //PUBLIC METHODS
        public override string ToString()
        {
            if (LoadType == "DistributedLoad") return "DistributedLoad";
            else return "PointLoad";
        }
        /// <summary>
        /// Decompose a Load
        /// </summary>
        /// <param name="Load">Load to decompose</param>
        /// <returns>Load Type, Load Pattern, Force or Moment, Direction, Distance 1, Distance 2, Value 1, Value 2, Coordinate System, Relative Distance</returns>
        [MultiReturn("Load Type", "Load Pattern", "Force/Moment Type", "Direction", "Distance", "Distance 2", "Value", "Value 2", "Coordinate System", "Relative Distance")]
        public static Dictionary<string, object> Decompose(Load Load)
        {
            if (Load != null)
            {
                string d2 = ""; // if it is a Point Load this should return empty
                string v2 = "";// if it is a Point Load this should return empty

                if (Load.LoadType == "DistributedLoad")
                {
                    //if it is a Distributed Load
                    d2 = Load.Dist2.ToString();
                    v2 = Load.Val2.ToString();
                }

                string forceOrMoment = "";
                if (Load.FMType == 1) forceOrMoment = "Force";
                else if (Load.FMType == 2) forceOrMoment = "Moment";


                string axisDir = "";
                if (Load.CSys == "GLOBAL" || Load.CSys == "global" || Load.CSys == "Global")
                {
                    if (Load.Dir == 4) axisDir = "Global X direction";
                    else if (Load.Dir == 5) axisDir = "Global Y direction";
                    else if (Load.Dir == 6) axisDir = "Global Z direction";
                    else if (Load.Dir == 7) axisDir = "Projected X direction";
                    else if (Load.Dir == 8) axisDir = "Projected Y direction";
                    else if (Load.Dir == 9) axisDir = "Projected Z direction";
                    else if (Load.Dir == 10) axisDir = "Gravity direction";
                    else if (Load.Dir == 11) axisDir = "Projected gravity direction";
                }


                else // for local coordinate systems
                {
                    if (Load.Dir == 1) axisDir = "Local 1 axis";
                    else if (Load.Dir == 2) axisDir = "Local 2 axis";
                    else if (Load.Dir == 3) axisDir = "Local 3 axis";
                }


                // Return outputs
                return new Dictionary<string, object>
            {
                {"Load Type", Load.LoadType},
                {"Load Pattern", Load.lPattern.name},
                {"Force/Moment Type", forceOrMoment},
                {"Direction", axisDir},
                {"Distance", Load.Dist},
                {"Distance 2", d2},
                {"Value", Load.Val},
                {"Value 2", v2},
                {"Coordinate System", Load.CSys},
                {"Relative Distance", Load.RelDist}
            };
            }
            else
            {
                return new Dictionary<string, object>
            {
                {"Load Type", null},
                {"Load Pattern", null},
                {"Force/Moment Type", null},
                {"Direction", null},
                {"Distance", null},
                {"Distance 2", null},
                {"Value",null},
                {"Value 2", null},
                {"Coordinate System", null},
                {"Relative Distance",null}
            };
            }
        }

        /// <summary>
        /// This function assigns loads to frame objects.
        /// Parameters description below as presented in the SAP CSi OAPI Documentation
        /// </summary>
        /// <param name="LoadPattern">The name of a defined load pattern.</param>
        /// <param name="LoadType">Force or moment load. Use the Load Type dropdown</param>
        /// <param name="Direction">This is an integer between 1 and 11, indicating the direction of the load.
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
        /// <param name="Distance">This is the distance from the I-End of the frame object to the load location. 
        /// This may be a relative distance (0 less or equal to Dist less or equal to 1) or an actual distance, 
        /// depending on the value of the RelDist item. [L] when RelDist is False</param>
        /// <param name="Value">This is the value of the point load. [F] when MyType is 1 and [FL] when MyType is 2</param>
        /// <param name="CoordSystem">This is Local or the name of a defined coordinate system. 
        /// It is the coordinate system in which the loads are specified.</param>
        /// <param name="RelativeDistance">If this item is True, the specified Dist item is a relative distance, 
        /// otherwise it is an actual distance.</param>
        /// <param name="Replace">If this item is True, all previous loads, if any, assigned to the specified frame object(s), 
        /// in the specified load pattern, are deleted before making the new assignment.</param>
        /// <returns>Load at a point along a Frame</returns>
        //DYNAMO CREATE NODES
        public static Load PointLoad(LoadPattern LoadPattern, string LoadType, int Direction, double Distance, double Value, string CoordSystem = "Global", bool RelativeDistance = true, bool Replace = true)
        {
            int ltype = 1;
            if (LoadType == "Moment") ltype = 2;
            
            CheckCoordSysAndDir(Direction, CoordSystem);
            Load l = new Load(LoadPattern, ltype, Direction, Distance, Value, CoordSystem, RelativeDistance);
            l.LoadType = "PointLoad";
            return l;
        }

        /// <summary>
        /// This function assigns distributed loads to frame objects.
        /// Parameters description below as presented in the SAP CSi OAPI Documentation
        /// </summary>
        /// <param name="LoadPattern">The name of a defined load pattern</param>
        /// <param name="LoadType">Force or moment load. Use the Load Type dropdown</param>
        ///<param name="Direction">This is an integer between 1 and 11, indicating the direction of the load.
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
        /// <param name="Distance">This is the distance from the I-End of the frame object to the load location. 
        /// This may be a relative distance (0 less or equal to Dist less or equal to 1) or an actual distance, 
        /// depending on the value of the RelDist item. [L] when RelDist is False</param>
        /// <param name="Distance2">This is the distance from the I-End of the frame object to the load location. 
        /// This may be a relative distance (0 less or equal to Dist2 less or equal to 1) or an actual distance, 
        /// depending on the value of the RelDist item. [L] when RelDist is False</param>
        /// <param name="Value">This is the load value at the start of the distributed load. [F] when MyType is 1 and [FL] when MyType is 2</param>
        /// <param name="Value2">This is the load value at the end of the distributed load. [F] when MyType is 1 and [FL] when MyType is 2</param>
        /// <param name="CoordSystem">This is Local or the name of a defined coordinate system. It is the coordinate system in which the loads are specified.</param>
        /// <param name="RelativeDistance">If this item is True, the specified Dist item is a relative distance, otherwise it is an actual distance.</param>
        /// <returns></returns>
        public static Load DistributedLoad(LoadPattern LoadPattern, string LoadType, int Direction, double Distance, double Distance2, double Value, double Value2, string CoordSystem = "Global", bool RelativeDistance = true)
        {
            int ltype = 1;
            if (LoadType == "Moment") ltype = 2;

            CheckCoordSysAndDir(Direction, CoordSystem);
            Load l = new Load(LoadPattern, ltype, Direction, Distance, Distance2, Value, Value2, CoordSystem, RelativeDistance);
            l.LoadType = "DistributedLoad";
            return l;
        }
        internal Load() { }

        //constructor for PointLoads
        internal Load(LoadPattern loadPat, int myType, int dir, double dist, double val, string cSys, bool relDist)
        {
            lPattern = loadPat;
            FMType = myType;
            Dir = dir;
            Dist = dist;
            Val = val;
            CSys = cSys;
            RelDist = relDist;

        }

        //constructor for DistributedLoads
        internal Load(LoadPattern loadPat, int myType, int dir, double dist, double dist2, double val, double val2, string cSys, bool relDist)
        {
            lPattern = loadPat;
            FMType = myType;
            Dir = dir;
            Dist = dist;
            Dist2 = dist2;
            Val = val;
            Val2 = val2;
            CSys = cSys;
            RelDist = relDist;

        }

        //Private method
        private static void CheckCoordSysAndDir(int dir, string CS)
        {
            //Check that the settings are correct
            if (CS != "Global")
            {
                if (dir == 0 || dir == 4 || dir == 5 || dir == 6 || dir == 7 || dir == 8 || dir == 9 || dir == 10 || dir == 11)
                {
                    throw new Exception("The direction setting and the Coordinate System are not compatible. Please, change these values");
                }
            }
            else // for global coordinate system
            {
                if (dir == 0 || dir == 1 || dir == 2 || dir == 3)
                {
                    throw new Exception("The direction setting and the Coordinate System are not compatible. Please, change these values");
                }
            }
        }
    }
}
