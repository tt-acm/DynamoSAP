using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DynamoSAP.Structure;

//DYNAMO
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;
using System.Runtime.Serialization;

namespace DynamoSAP
{
    // Trace and Element Binding! read https://github.com/DynamoDS/Dynamo/wiki/Integration-Guide
    [SupressImportIntoVM]
    public class TracedFrameManager
    {
        public static int frmID = 0;

        public static int GetNextUnusedID()
        {
            int next = frmID;
            frmID++;
            return next;
        }

        public static Dictionary<int, Frame> FrmDictionary = new Dictionary<int, Frame>();

        public static Frame GetFramebyID(int id)
        {
            Frame ret;
            FrmDictionary.TryGetValue(id, out ret);
            return ret;
        }

        public static void RegisterFrmforID(int id, Frame frm)
        {
            if (FrmDictionary.ContainsKey(id))
            {
                FrmDictionary[id] = frm;
            }
            else
            {
                FrmDictionary.Add(id, frm);
            }

        }

    }

    [SupressImportIntoVM]
    public class FrmID : ISerializable
    {
        public int IntID { get; set; }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("intID", IntID, typeof(int));
        }

        public FrmID()
        {
            IntID = int.MinValue;
        }

        public FrmID(SerializationInfo info, StreamingContext context)
        {
            IntID = (int)info.GetValue("intID", typeof(int));
        }
    }
}
