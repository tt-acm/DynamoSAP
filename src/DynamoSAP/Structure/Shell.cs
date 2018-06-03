/// Developed by Thornton Tomasetti's CORE Studio for Autodesk
/// http://core.thorntontomasetti.com
/// COREsudio Developers: Elcin Ertugrul and Ana Garcia Puyol

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

//DYNAMO
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;
using DynamoSAP.Assembly;
using DynamoSAP.Definitions;
using ProtoCore.Lang;
using DynamoServices;

//SAP
using SAP2000v20;
using SAPConnection;


namespace DynamoSAP.Structure
{
    //[RegisterForTrace]
    public class Shell : Element
    {
        
        // FIELDS

        // Mesh
        internal Mesh BaseM { get; set; }
        // Surface
        internal Surface BaseS { get; set; }
        // Shell Property
        internal ShellProp shellProp { get; set; }
        
        // QUERY NODES
        public Mesh BaseMesh
        {
            get { return BaseM; }
        }

        public Surface BaseSurface
        {
            get { return BaseS; }
        }

        //public ShellProp ShellProp
        //{
        //    get { return shellProp; }
        //}

        /// <summary>
        /// ID of the Curve
        /// </summary>
        internal int ID { get; private set; }

        //PUBLIC METHODS
        /// <summary>
        /// Type of the structural element
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            //return String.Format("ShellID: {0}", ID.ToString());
            return Label;
        }

        //PUBLIC NODES
        /// <summary>
        /// Creates Shell from Mesh
        /// </summary>
        /// <param name="Mesh">Dynamo Mesh</param>
        /// <param name="ShellProp">Shell Property</param>
        /// <returns></returns>
        [RegisterForTrace]
        public static Shell FromMesh(Mesh Mesh, ShellProp ShellProp)
        {
            Shell tShell;
            ShellID tShellid = null;
            //TraceUtils.GetTraceData(TRACE_ID) as ShellID;
            Dictionary<string, ISerializable> getObjs = ProtoCore.Lang.TraceUtils.GetObjectFromTLS();
            foreach (var k in getObjs.Keys)
            {
                tShellid = getObjs[k] as ShellID;
            }

            if (tShellid == null)
            {
                // trace cache log didnoy return an objec, create new one !
                tShell = new Shell(Mesh, ShellProp);
                // Set Label
                tShell.Label = String.Format("dyn_{0}", tShell.ID.ToString()); 
            }
            else
            {
                tShell = TracedShellManager.GetShellbyID(tShellid.IntID);

                tShell.BaseM = Mesh;
                tShell.shellProp = ShellProp;
            }

            // Set the trace data on the return to be this Shell
            //TraceUtils.SetTraceData(TRACE_ID, new ShellID { IntID = tShell.ID });
            Dictionary<string, ISerializable> objs = new Dictionary<string, ISerializable>();
            objs.Add(TRACE_ID, new ShellID { IntID = tShell.ID });
            ProtoCore.Lang.TraceUtils.SetObjectToTLS(objs);

            return tShell;
        }

        /// <summary>
        /// Creates Shell from Surface
        /// </summary>
        /// <param name="Surface">Dynamo Surface</param>
        /// <param name="ShellProp">Shell Property </param>
        /// <returns></returns>
        [RegisterForTrace]
        public static Shell FromSurface(Surface Surface, ShellProp ShellProp)
        {
            // TODO: IsPlanar logic should be added here! HANDLETHE ERROR.
            Shell tShell;
            ShellID tShellid = null;
                //TraceUtils.GetTraceData(TRACE_ID) as ShellID;

            Dictionary<string, ISerializable> getObjs = ProtoCore.Lang.TraceUtils.GetObjectFromTLS();
            foreach (var k in getObjs.Keys)
            {
                tShellid = getObjs[k] as ShellID;
            }

            if (tShellid == null)
            {
                // trace cache log didnoy return an objec, create new one !
                tShell = new Shell(Surface, ShellProp);
                // Set Label
                tShell.Label = String.Format("dyn_{0}", tShell.ID.ToString());
            }
            else
            {
                tShell = TracedShellManager.GetShellbyID(tShellid.IntID);

                tShell.BaseS = Surface;
                tShell.shellProp = ShellProp;
            }

            // Set the trace data on the return to be this Shell
            //TraceUtils.SetTraceData(TRACE_ID, new ShellID { IntID = tShell.ID });
            Dictionary<string, ISerializable> objs = new Dictionary<string, ISerializable>();
            objs.Add(TRACE_ID, new ShellID { IntID = tShell.ID });
            ProtoCore.Lang.TraceUtils.SetObjectToTLS(objs);

            return tShell;
        }

        // DECOMPOSE NODE

        /// <summary>
        /// Decompose a Shell into its geometry and Shell Property
        /// </summary>
        /// <param name="Shell"></param>
        /// <returns></returns>
        [MultiReturn("BaseSurface", "BaseMesh", "ShellProp")]
        public static Dictionary<string,object> Decompose( Shell Shell)
        {
            //Return outputs
            if (Shell.Type == Structure.Type.Shell)
            {
                return new Dictionary<string, object>
                {
                    {"BaseSurface", Shell.BaseS},
                    {"BaseMesh", Shell.BaseM},
                    {"ShellProp", Shell.shellProp},                
                }; 
            }
            return null;

        }

        //PRIVATE CONSTRUCTORS
        internal Shell() { }
        internal Shell(Mesh mesh, ShellProp shellprop)
        {
            BaseM = mesh;
            shellProp = shellprop;
            this.Type = Structure.Type.Shell;

            //register for cache
            ID = TracedShellManager.GetNextUnusedID();
            TracedShellManager.RegisterShellforID(ID, this);
        }
        internal Shell(Surface surface, ShellProp shellprop)
        {
            BaseS = surface;
            shellProp = shellprop;
            this.Type = Structure.Type.Shell;

            //register for cache
            ID = TracedShellManager.GetNextUnusedID();
            TracedShellManager.RegisterShellforID(ID, this);
        }

    }
}
