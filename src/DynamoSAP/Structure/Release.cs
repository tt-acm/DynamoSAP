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

        
        // PUBLIC METHODS
        public static Release SetRelease( bool U1i = false, bool U1j = false, bool U2i = false, bool U2j = false, bool U3i = false, bool U3j = false, bool R1i = false, bool R1j = false, bool R2i = false, bool R2j = false, bool R3i = false, bool R3j = false)
        {
            return new Release(U1i,  U1j,  U2i,  U2j, U3i, U3j, R1i, R1j, R2i, R2j, R3i, R3j);
        }


        // PRIVATE CONSTRUCTOR
        private Release() { }
        private Release( bool U1i, bool U1j, bool U2i, bool U2j, bool U3i,bool U3j, bool R1i,bool R1j,bool R2i, bool R2j, bool R3i,bool R3j)
        {
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
