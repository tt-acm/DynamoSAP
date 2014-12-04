using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;

namespace DynamoSAP.Structure
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

        // Decompose
        [MultiReturn("Point", "U1", "U2","U3","R1","R2","R3")]
        public static Dictionary<string, object> Decompose(Restraint restraint)
        {
            // Return outputs
            return new Dictionary<string, object>
            {
                {"Point", restraint.pt},
                {"U1", restraint.u1},
                {"U2", restraint.u2},
                {"U3", restraint.u3},
                {"R1", restraint.r1},
                {"R2", restraint.r2},
                {"R3", restraint.r3}
            };
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
