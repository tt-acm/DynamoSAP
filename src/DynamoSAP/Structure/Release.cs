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
        /// <param name="iP">P Start bool value</param>
        /// <param name="jP">P End bool value</param>
        /// <param name="iV2">V2 Start bool value</param>
        /// <param name="jV2">V2 End bool value</param>
        /// <param name="iV3">V3 Start bool value</param>
        /// <param name="jV3">V3 End bool value</param>
        /// <param name="iT">T Start bool value</param>
        /// <param name="jT">T End bool value</param>
        /// <param name="iM2">M2 Start bool value</param>
        /// <param name="jM2">M2 End bool value</param>
        /// <param name="iM3">M3 Start bool value</param>
        /// <param name="jM3">M3 End bool value</param>
        /// <returns>Release</returns>
        public static Release Set(bool iP = false, bool jP = false, bool iV2 = false, bool jV2 = false, bool iV3 = false, bool jV3 = false, bool iT = false, bool jT = false, bool iM2 = false, bool jM2 = false, bool iM3 = false, bool jM3 = false)
        {
            return new Release(iP, jP, iV2, jV2, iV3, jV3, iT, jT, iM2, jM2, iM3, jM3);
        }


        /// <summary>
        /// Decompose a Release
        /// </summary>
        /// <param name="release">Release to decompose</param>
        /// <returns>End releases</returns>
        [MultiReturn("iP", "jP", "iV2", "jV2", "iV3", "jV3", "iT", "jT", "iM2", "jM2", "iM3", "jM3")]
        public static Dictionary<string, object> Decompose(Release release)
        {
            // Return outputs
            return new Dictionary<string, object>
            {
                {"iP", release.u1i},
                {"jP", release.u1j},
                {"iV2", release.u2i},
                {"jV2", release.u2j},
                {"iV3", release.u3i},
                {"jV3", release.u3j},
                {"iT", release.r1i},
                {"jT", release.r1j},
                {"iM2", release.r2i},
                {"jM2", release.r2j},
                {"iM3", release.r3i},
                {"jM3", release.r3j}
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
