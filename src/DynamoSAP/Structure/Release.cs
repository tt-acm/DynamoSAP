using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;

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
        /// <summary>
        /// Set a Release
        /// </summary>
        /// <param name="U1i">U1 Start value</param>
        /// <param name="U1j">U1 End value</param>
        /// <param name="U2i">U2 Start value</param>
        /// <param name="U2j">U2 End value</param>
        /// <param name="U3i">U3 Start value</param>
        /// <param name="U3j">U3 End value</param>
        /// <param name="R1i">R1 Start value</param>
        /// <param name="R1j">R1 End value</param>
        /// <param name="R2i">R2 Start value</param>
        /// <param name="R2j">R2 End value</param>
        /// <param name="R3i">R3 Start value</param>
        /// <param name="R3j">R4 End value</param>
        /// <returns>Release</returns>
        public static Release Set(bool U1i = false, bool U1j = false, bool U2i = false, bool U2j = false, bool U3i = false, bool U3j = false, bool R1i = false, bool R1j = false, bool R2i = false, bool R2j = false, bool R3i = false, bool R3j = false)
        {
            return new Release(U1i, U1j, U2i, U2j, U3i, U3j, R1i, R1j, R2i, R2j, R3i, R3j);
        }


        /// <summary>
        /// Decompose a Release
        /// </summary>
        /// <param name="release">Release to decompose</param>
        /// <returns>End releases</returns>
        [MultiReturn("U1i", "U1j", "U2i", "U2j", "U3i", "U3j", "R1i", "R1j", "R2i", "R2j", "R3i", "R3j")]
        public static Dictionary<string, object> Decompose(Release release)
        {
            // Return outputs
            return new Dictionary<string, object>
            {
                {"U1i", release.u1i},
                {"U1j", release.u1j},
                {"U2i", release.u2i},
                {"U2j", release.u2j},
                {"U3i", release.u3i},
                {"U3j", release.u3j},
                {"R1i", release.r1i},
                {"R1j", release.r1j},
                {"R2i", release.r2i},
                {"R2j", release.r2j},
                {"R3i", release.r3i},
                {"R3j", release.r3j}
            };
        }

        // PRIVATE CONSTRUCTOR
        private Release() { }
        private Release(bool U1i, bool U1j, bool U2i, bool U2j, bool U3i, bool U3j, bool R1i, bool R1j, bool R2i, bool R2j, bool R3i, bool R3j)
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
