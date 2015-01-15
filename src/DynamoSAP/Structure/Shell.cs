using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//DYNAMO
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;


//SAP
using SAP2000v16;
using SAPConnection;
using DynamoSAP.Assembly;

namespace DynamoSAP.Structure
{
    [DSNodeServices.RegisterForTrace]
    public class Shell : Element
    {
        // FIELDS

        // Mesh
        internal Mesh BaseM { get; set; }

        
        // QUERY NODES
        public Mesh BaseMesh
        {
            get { return BaseM; }
        }

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
            return String.Format("ShellID: {0}", ID.ToString());
        }

        //PUBLIC NODES
        public static Shell FromMesh(Mesh Mesh)
        {
            Shell tShell;
            ShellID tShellid = DSNodeServices.TraceUtils.GetTraceData(TRACE_ID) as ShellID;

            if (tShellid == null)
            {
                // trace cache log didnoy return an objec, create new one !
                tShell = new Shell(Mesh);
                // Set Label
                tShell.Label = String.Format("dyn_{0}", tShell.ID.ToString()); 
            }
            else
            {
                tShell = TracedShellManager.GetShellbyID(tShellid.IntID);

                tShell.BaseM = Mesh;
            }

            // Set the trace data on the return to be this Shell
            DSNodeServices.TraceUtils.SetTraceData(TRACE_ID, new ShellID { IntID = tShell.ID });

            return tShell;
        }

        //PRIVATE CONSTRUCTORS
        internal Shell() { }
        internal Shell(Mesh mesh)
        {
            BaseM = mesh;
            this.Type = Structure.Type.Shell;

            //register for cache
            ID = TracedShellManager.GetNextUnusedID();
            TracedShellManager.RegisterShellforID(ID, this);
        }

    }
}
