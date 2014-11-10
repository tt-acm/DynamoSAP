using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.DesignScript.Geometry;

using DynamoSAP.Structure;

namespace DynamoSAP.Analysis
{
    class Release
    {

        //FIELDS

        internal string name;
        internal bool u1ii;
        internal bool u1jj;
        internal bool u2ii;
        internal bool u2jj;
        internal bool u3ii;
        internal bool u3jj;
        internal bool r1ii;
        internal bool r1jj;
        internal bool r2ii;
        internal bool r2jj;
        internal bool r3ii;
        internal bool r3jj;

        //QUERY NODES
        public string Name {get { return name; }}
        public bool U1ii {get { return u1ii; }}
        public bool U1jj {get { return u1jj; }}
        public bool U2ii {get { return u2ii; }}
        public bool U2jj { get { return u2jj; } }
        public bool U3ii { get { return u3ii; } }
        public bool U3jj { get { return u3jj; } }
        public bool R1ii { get { return r1ii; } }
        public bool R1jj { get { return r1jj; } }
        public bool R2ii { get { return r2ii; } }
        public bool R2jj { get { return r2jj; } }
        public bool R3ii { get { return r3ii; } }
        public bool R3jj { get { return r3jj; } }
        
        // PUBLIC METHODS
        public static Release SetRelease(Frame F, bool U1ii, bool U1jj, bool U2ii, bool U2jj, bool U3ii,bool U3jj, bool R1ii,bool R1jj,bool R2ii, bool R2jj, bool R3ii,bool R3jj ){
            return new Release(F,  U1ii,  U1jj,  U2ii,  U2jj, U3ii, U3jj, R1ii, R1jj, R2ii, R2jj, R3ii, R3jj);
        }

        // PRIVATE CONSTRUCTOR
        private Release() { }
        private Release(Frame f, bool U1ii, bool U1jj, bool U2ii, bool U2jj, bool U3ii,bool U3jj, bool R1ii,bool R1jj,bool R2ii, bool R2jj, bool R3ii,bool R3jj )
        {
            name = f.Label;
            u1ii = U1ii;
            u1jj = U1jj;
            u2ii = U2ii;
            u2jj = U2jj;
            u3ii = U3ii;
            u3jj = U3jj;
            r1ii = R1ii;
            r1jj = R1jj;
            r2ii = R2ii;
            r2jj = R2jj;
            r3ii = R3ii;
            r3jj = R3jj;
            
        }
    }
}
