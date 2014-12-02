// ROT Helper class to get Running Object Table 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.InteropServices;

//DYNAMO
using Autodesk.DesignScript.Runtime;
using System.Collections;

namespace SAPConnection
{
    [SupressImportIntoVM]
    public class ROTHelper
    {

        #region APIs

        [DllImport("ole32.dll")]
        private static extern int GetRunningObjectTable(int reserved,
            out UCOMIRunningObjectTable prot);

        [DllImport("ole32.dll")]
        private static extern int CreateBindCtx(int reserved,
            out UCOMIBindCtx ppbc);

        [DllImport("ole32.dll", PreserveSig = false)]
        private static extern void CLSIDFromProgIDEx([MarshalAs(UnmanagedType.LPWStr)] string progId, out Guid clsid);

        [DllImport("ole32.dll", PreserveSig = false)]
        private static extern void CLSIDFromProgID([MarshalAs(UnmanagedType.LPWStr)] string progId, out Guid clsid);

        [DllImport("ole32.dll")]
        private static extern int ProgIDFromCLSID([In()]ref Guid clsid, [MarshalAs(UnmanagedType.LPWStr)]out string lplpszProgID);

        #endregion

        #region Public Methods

        /// <summary>
        /// Converts a COM class ID into a prog id.
        /// </summary>
        /// <param name="progID">The prog id to convert to a class id.</param>
        /// <returns>Returns the matching class id or the prog id if it wasn't found.</returns>
        public static string ConvertProgIdToClassId(string progID)
        {
            Guid testGuid;
            try
            {
                CLSIDFromProgIDEx(progID, out testGuid);
            }
            catch
            {
                try
                {
                    CLSIDFromProgID(progID, out testGuid);
                }
                catch
                {
                    return progID;
                }
            }
            return testGuid.ToString().ToUpper();
        }

        /// <summary>
        /// Converts a COM class ID into a prog id.
        /// </summary>
        /// <param name="classID">The class id to convert to a prog id.</param>
        /// <returns>Returns the matching class id or null if it wasn't found.</returns>
        public static string ConvertClassIdToProgId(string classID)
        {
            Guid testGuid = new Guid(classID.Replace("!", ""));
            string progId = null;
            try
            {
                ProgIDFromCLSID(ref testGuid, out progId);
            }
            catch (Exception)
            {
                return null;
            }
            return progId;
        }

        /// <summary>
        /// Get a snapshot of the running object table (ROT).
        /// </summary>
        /// <returns>A hashtable mapping the name of the object in the ROT to the corresponding object
        /// <param name="filter">The filter to apply to the list (nullable).</param>
        /// <returns>A hashtable of the matching entries in the ROT</returns>
        public static Hashtable GetActiveObjectList(string filter)
        {
            Hashtable result = new Hashtable();

            int numFetched;
            UCOMIRunningObjectTable runningObjectTable;
            UCOMIEnumMoniker monikerEnumerator;
            UCOMIMoniker[] monikers = new UCOMIMoniker[1];

            GetRunningObjectTable(0, out runningObjectTable);
            runningObjectTable.EnumRunning(out monikerEnumerator);
            monikerEnumerator.Reset();

            while (monikerEnumerator.Next(1, monikers, out numFetched) == 0)
            {
                UCOMIBindCtx ctx;
                CreateBindCtx(0, out ctx);

                string runningObjectName;
                monikers[0].GetDisplayName(ctx, null, out runningObjectName);

                object runningObjectVal;
                runningObjectTable.GetObject(monikers[0], out runningObjectVal);
                if (filter == null || filter.Length == 0 || filter.IndexOf(filter) != -1)
                {
                    result[runningObjectName] = runningObjectVal;
                }
            }

            return result;
        }

        /// <summary>
        /// Returns an object from the ROT, given a prog Id.
        /// </summary>
        /// <param name="progId">The prog id of the object to return.</param>
        /// <returns>The requested object, or null if the object is not found.</returns>
        public static object GetActiveObject(string progId)
        {
            // Convert the prog id into a class id
            string classId = ConvertProgIdToClassId(progId);

            UCOMIRunningObjectTable prot = null;
            UCOMIEnumMoniker pMonkEnum = null;
            try
            {
                int Fetched = 0;
                // Open the running objects table.
                GetRunningObjectTable(0, out prot);
                prot.EnumRunning(out pMonkEnum);
                pMonkEnum.Reset();
                UCOMIMoniker[] pmon = new UCOMIMoniker[1];

                // Iterate through the results
                while (pMonkEnum.Next(1, pmon, out Fetched) == 0)
                {
                    UCOMIBindCtx pCtx;

                    CreateBindCtx(0, out pCtx);

                    string displayName;
                    pmon[0].GetDisplayName(pCtx, null, out displayName);
                    Marshal.ReleaseComObject(pCtx);
                    if (displayName.IndexOf(classId) != -1)
                    {
                        // Return the matching object
                        object objReturnObject;
                        prot.GetObject(pmon[0], out objReturnObject);
                        return objReturnObject;
                    }
                }
                return null;
            }
            finally
            {
                // Free resources
                if (prot != null)
                    Marshal.ReleaseComObject(prot);
                if (pMonkEnum != null)
                    Marshal.ReleaseComObject(pMonkEnum);
            }
        }

        #endregion


    }
}
