/// Developed by Thornton Tomasetti's CORE Studio for Autodesk
/// http://core.thorntontomasetti.com
/// CORE Developers: Elcin Ertugrul and Ana Garcia Puyol

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SAP2000v20;
using DynamoSAP;
using DynamoSAP_UI;

// interop.COM services for SAP
using System.Runtime.InteropServices;

//DYNAMO
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;

namespace SAPConnection
{
    [SupressImportIntoVM]
    public class GroupMapper
    {
        public static void DefineGroup( ref cSapModel Model, string GroupName)
        {
            long ret = Model.GroupDef.SetGroup(GroupName);
        }
        public static void GetSAPGroupList(ref cSapModel Model, ref List<string> myGroupList)
        {
            int num = 0;
            string[] Names = null;
            long ret = Model.GroupDef.GetNameList(ref num, ref Names);
            if (Names != null)
            {
                myGroupList = Names.ToList();
            }
        }

        public static void ClearGroupAssigment (ref cSapModel Model, string GroupName)
        {
            long ret = Model.GroupDef.Clear(GroupName);
        }

        public static void Delete(ref cSapModel Model, string GroupName)
        {
            long ret = Model.GroupDef.Delete(GroupName);
        }

        public static void SetGroupAssign_Frm(ref cSapModel Model, string GroupName, List<string> FrmLabels)
        {
            foreach (var id in FrmLabels)
            {
                long ret = Model.FrameObj.SetGroupAssign(id, GroupName);
            }
        }

        public static void SetGroupAssign_Shell(ref cSapModel Model, string GroupName, List<string> ShellLabels)
        {
            foreach (var id in ShellLabels)
            {
                long ret = Model.AreaObj.SetGroupAssign(id, GroupName);
            }
        }

        public static void SetGroupAssign_Joint(ref cSapModel Model, string GroupName, List<string> JointLabels)
        {
            foreach (var id in JointLabels)
            {
                long ret = Model.PointObj.SetGroupAssign(id, GroupName);
            }
        }

        public static void SetGroupAssign_Cable(ref cSapModel Model, string GroupName, List<string> CableLabels)
        {
            foreach (var id in CableLabels)
            {
                long ret = Model.CableObj.SetGroupAssign(id, GroupName);
            }
        }

        public static void GetGroupAssignments( ref cSapModel Model, string GroupName, ref int[] types, ref string[] Labels)
        {
            int  num = 0;
            long ret = Model.GroupDef.GetAssignments(GroupName, ref num, ref types, ref Labels);       
        }


    }
}
