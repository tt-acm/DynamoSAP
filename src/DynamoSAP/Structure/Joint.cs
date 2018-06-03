/// Developed by Thornton Tomasetti's CORE Studio for Autodesk
/// http://core.thorntontomasetti.com
/// COREstudio Developers: Elcin Ertugrul and Ana Garcia Puyol

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

//DYNAMO
using DynamoSAP.Assembly;
using DynamoSAP.Definitions;
using ProtoCore.Lang;
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;
using DynamoServices;

//SAP
using SAP2000v20;
using SAPConnection;


namespace DynamoSAP.Structure
{
   // [RegisterForTrace]
    public class Joint:Element
    {
        //FIELD
        internal Point BasePt { get; set; }
        internal Restraint JointRestraint { get; set; }
        
       
        //QUERY NODES

        public Point BasePoint
        {
            get { return BasePt; }
        }

        internal int ID { get; private set; }

        /// <summary>
        /// Unique Label
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Label;
        }

        /// <summary>
        /// Create Joint from Dynamo Point
        /// </summary>
        /// <param name="Point"></param>
        /// <returns></returns>
        [RegisterForTrace]
        public static Joint FromPoint(Point Point)
        {
            Joint tJoint;
            //JointID tJointId = TraceUtils.GetTraceData(TRACE_ID) as JointID;
              
            Dictionary<string, ISerializable> getObjs= ProtoCore.Lang.TraceUtils.GetObjectFromTLS();

            JointID tJointId = null;

            foreach (var k in getObjs.Keys)
            {
                tJointId = getObjs[k] as JointID;
            }

            if (tJointId == null)
            {
                // trace cache log didnot return an object, create new one !
                tJoint = new Joint(Point);
                // Set Label
                tJoint.Label = String.Format("dyn_{0}", tJoint.ID.ToString());  
            }
            else
            {
                tJoint = TracedJointManager.GetJointbyID(tJointId.IntID);
                string test = tJoint.Label;
                tJoint.BasePt = Point;
            }

            //Set the trace data on the return to be this Joint

            Dictionary<string, ISerializable> objs = new Dictionary<string, ISerializable>();
            objs.Add(TRACE_ID, new JointID { IntID = tJoint.ID });
            ProtoCore.Lang.TraceUtils.SetObjectToTLS(objs);
            
            return tJoint;
        }

        /// <summary>
        /// Set Restraints to Joint
        /// </summary>
        /// <param name="Joint">Joint</param>
        /// <param name="Restraint">Restraint</param>
        /// <returns></returns>
        public static Joint SetRestraint( Joint Joint, Restraint Restraint )
        {
            // Create a new Joint using the properties of the input Joint
            Joint newJoint = Joint.FromPoint(Joint.BasePoint);
            // Create label
            newJoint.Label = String.Format("dyn_{0}", Joint.ID.ToString());
            // set the restraint
            newJoint.JointRestraint = Restraint;
           return newJoint;
        }

        //PRIVATE CONSTRUCTORS
        internal Joint(){}
        internal Joint(Point Point)
        {
            BasePt = Point;
            this.Type = Structure.Type.Joint;

            //register for cache
            ID = TracedJointManager.GetNextUnusedID();
            TracedJointManager.RegisterJointforID(ID, this);
        }

    }
}
