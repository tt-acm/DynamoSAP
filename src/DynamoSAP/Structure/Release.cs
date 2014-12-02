using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.DesignScript.Geometry;

using DynamoSAP.Structure;


namespace DynamoSAP.Structure
{
    public class Release
    {

        //FIELDS

        //Frame Name
        internal Frame frm { get; set; }
        
        //U1 Releases
        internal bool u1i;
        internal bool u1j;
        //U2 Releases
        internal bool u2i;
        internal bool u2j;
        //U3 Releases
        internal bool u3i;
        internal bool u3j;
        //R1 Releases
        internal bool r1i;
        internal bool r1j;
        //R2 Releases
        internal bool r2i;
        internal bool r2j;
        //R3 Releases
        internal bool r3i;
        internal bool r3j;

        //QUERY NODES
        
        public Frame Frame { get { return frm; } }
        public bool U1i {get { return u1i; }}
        public bool U1j {get { return u1j; }}
        public bool U2i {get { return u2i; }}
        public bool U2j { get { return u2j; } }
        public bool U3i { get { return u3i; } }
        public bool U3j { get { return u3j; } }
        public bool R1i { get { return r1i; } }
        public bool R1j { get { return r1j; } }
        public bool R2i { get { return r2i; } }
        public bool R2j { get { return r2j; } }
        public bool R3i { get { return r3i; } }
        public bool R3j { get { return r3j; } }
        
        // PUBLIC METHODS
        public static Release SetRelease(Frame F, bool U1i = false, bool U1j = false, bool U2i = false, bool U2j = false, bool U3i = false, bool U3j = false, bool R1i = false, bool R1j = false, bool R2i = false, bool R2j = false, bool R3i = false, bool R3j = false)
        {
            return new Release(F,  U1i,  U1j,  U2i,  U2j, U3i, U3j, R1i, R1j, R2i, R2j, R3i, R3j);
        }

        public static List<Sphere> Display(Release R, double radius=10.0)
        {

            List<Sphere> rSpheres = new List<Sphere>();
            if (R.U1i == true || R.U2i == true || R.U3i == true || R.R1i == true || R.R2i == true || R.R3i == true)
            {
                Sphere s = Sphere.ByCenterPointRadius(R.Frame.BaseCrv.PointAtParameter(0.15), radius);
                rSpheres.Add(s);
            }
            else if (R.U1j == true || R.U2j == true || R.U3j == true || R.R1j == true || R.R2j == true || R.R3j == true)
            {
                Sphere s = Sphere.ByCenterPointRadius(R.Frame.BaseCrv.PointAtParameter(0.85), radius);
                rSpheres.Add(s);
            }
            return rSpheres;
        }

        // PRIVATE CONSTRUCTOR
        private Release() { }
        private Release(Frame f, bool U1i, bool U1j, bool U2i, bool U2j, bool U3i,bool U3j, bool R1i,bool R1j,bool R2i, bool R2j, bool R3i,bool R3j)
        {
            frm = f;
            u1i = U1i;
            u1j = U1j;
            u2i = U2i;
            u2j = U2j;
            u3i = U3i;
            u3j = U3j;
            r1i = R1i;
            r1j = R1j;
            r2i = R2i;
            r2j = R2j;
            r3i = R3i;
            r3j = R3j;
            
        }
    }
}
