/// Developed by Thornton Tomasetti's CORE Studio for Autodesk
/// http://core.thorntontomasetti.com
/// CORE Developers: Elcin Ertugrul and Ana Garcia Puyol

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

    [SupressImportIntoVM]
    public class TracedShellManager
    {
        public static int shellID = 0;

        public static int GetNextUnusedID()
        {
            int next = shellID;
            shellID++;
            return next;
        }

        public static Dictionary<int, Shell> ShellDictionary = new Dictionary<int, Shell>();

        public static Shell GetShellbyID(int id)
        {
            Shell ret;
            ShellDictionary.TryGetValue(id, out ret);
            return ret;
        }

        public static void RegisterShellforID(int id, Shell shell)
        {
            if (ShellDictionary.ContainsKey(id))
            {
                ShellDictionary[id] = shell;
            }
            else
            {
                ShellDictionary.Add(id, shell);
            }

        }

    }

    [SupressImportIntoVM]
    public class ShellID : ISerializable
    {
        public int IntID { get; set; }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("intID", IntID, typeof(int));
        }

        public ShellID()
        {
            IntID = int.MinValue;
        }

        public ShellID(SerializationInfo info, StreamingContext context)
        {
            IntID = (int)info.GetValue("intID", typeof(int));
        }
    }

    [SupressImportIntoVM]
    public class TracedJointManager
    {
        public static int jointID = 0;

        public static int GetNextUnusedID()
        {
            int next = jointID;
            jointID++;
            return next;
        }

        public static Dictionary<int, Joint> JointDictionary = new Dictionary<int, Joint>();

        public static Joint GetJointbyID(int id)
        {
            Joint ret;
            JointDictionary.TryGetValue(id, out ret);
            return ret;
        }

        public static void RegisterJointforID(int id, Joint joint)
        {
            if (JointDictionary.ContainsKey(id))
            {
                JointDictionary[id] = joint;
            }
            else
            {
                JointDictionary.Add(id, joint);
            }
        }

    }

    [SupressImportIntoVM]
    public class JointID : ISerializable
    {
        public int IntID { get; set; }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("intID", IntID, typeof(int));
        }

        public JointID()
        {
            IntID = int.MinValue;
        }

        public JointID(SerializationInfo info, StreamingContext context)
        {
            IntID = (int)info.GetValue("intID", typeof(int));
        }
    }
}
