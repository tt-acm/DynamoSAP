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
using DynamoSAP.Definitions;

namespace DynamoSAP.Structure
{
    [DSNodeServices.RegisterForTrace]
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
        public static Joint FromPoint(Point Point)
        {
            Joint tJoint;
            JointID tJointId = DSNodeServices.TraceUtils.GetTraceData(TRACE_ID) as JointID;

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

                tJoint.BasePt = Point;

            }

            //Set the trace data on the return to be this Joint
            DSNodeServices.TraceUtils.SetTraceData(TRACE_ID, new JointID { IntID = tJoint.ID });
            
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
            Joint newJoint = Joint.FromPoint(Joint.BasePoint);
            newJoint.JointRestraint = Restraint;
            newJoint.Label = Joint.Label;
            return newJoint;
        }

        //public static Joint SetJointForce ( Joint Joint, bool replaceExisting = false)
        //{
        //    if (replaceExisting)
        //    {
                
        //    }

        //    return Joint;
        //}

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
