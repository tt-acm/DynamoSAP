using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.DesignScript.Geometry;


namespace DynamoSAP.Analysis
{
    public class Restraint
    {
        //FIELDS

        internal Point pt;
        internal bool u1;
        internal bool u2;
        internal bool u3;
        internal bool r1;
        internal bool r2;
        internal bool r3;

        //QUERY NODE
        public Point Pt {get { return pt; }}
        //Translations
        public bool U1 { get { return u1; } }
        public bool U2 { get { return u2; } }
        public bool U3 { get { return u3; } }
        //Rotations
        public bool R1 { get { return r1; } }
        public bool R2 { get { return r2; } }
        public bool R3 { get { return r3; } }

        // PUBLIC METHODS
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Point"></param>
        /// <param name="XX">Translation XX</param>
        /// <param name="YY">Translation YY</param>
        /// <param name="ZZ">Translation ZZ</param>
        /// <param name="X">Rotation X</param>
        /// <param name="Y">Rotation Y</param>
        /// <param name="Z">Rotation Z</param>
        /// <returns></returns>
        public static Restraint SetRestraint(Point Point, bool XX, bool YY, bool ZZ, bool X, bool Y, bool Z)
        {
            return new Restraint(Point, XX, YY, ZZ, X, Y, Z);
        }

        // FAST RESTRAINTS
        public static Restraint Fixed(Point Point)
        {
            return new Restraint(Point, true, true, true, true, true, true);
        }

        public static Restraint Pinned(Point Point)
        {
            return new Restraint(Point, true, true, true, false, false, false);
        }

        public static Restraint Roller(Point Point)
        {
            return new Restraint(Point, false, false, true, false, false, false);
        }

        public static Restraint Simple(Point Point)
        {
            return new Restraint(Point, false, false, false, false, false, false);
        }

        // PRIVATE CONSTRUCTOR
        private Restraint() { }
        private Restraint(Point Pt, bool U1, bool U2, bool U3, bool R1, bool R2, bool R3)
        {
            pt = Pt;
            u1 = U1;
            u2 = U2;
            u3 = U3;
            r1 = R1;
            r2 = R2;
            r3 = R3;
        }


    }
}
