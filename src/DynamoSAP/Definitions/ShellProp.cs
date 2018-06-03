/// Developed by Thornton Tomasetti's CORE Studio for Autodesk
/// http://core.thorntontomasetti.com
/// CORE Developers: Elcin Ertugrul and Ana Garcia Puyol

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//DYNAMO
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;

//SAP
using SAP2000v20;
using SAPConnection;

namespace DynamoSAP.Definitions
{
    public class ShellProp
    {
        // FIELDS
        internal string PropName { get; set; }
        internal int ShellType { get; set; }
        internal bool DOF { get; set; }
        internal string MatProp { get; set; }
        internal double MatAngle { get; set; } // degree
        internal double Thickness { get; set; }
        internal double Bending { get; set; }

        // DYNAMO NODES
        /// <summary>
        /// Define a Shell Property
        /// </summary>
        /// <param name="Name"> The Name of an existing or new property. It is modified if existing, otherwise a new property is added. </param>
        /// <param name="ShellType"> Indicates the Shell Type</param>
        /// <param name="DOF"> True if drilling degrees of freedom are included in the element formulation in the analysis model. This item does not apply when ShellType = Plate_thin, Plate_thick and Shell_layered/nonlinear </param>
        /// <param name="MatProp"> Material property of shell property. Doesn't apply when ShellType = Shell_layered </param>
        /// <param name="MatAngle"> Material Angle [deg]. Doesn't apply when ShellType = Shell_layered</param>
        /// <param name="Thickness"> The membrabe thickness. Doesn't apply when ShellType = Shell_layered</param>
        /// <param name="Bending">The bending thickness. Doesn't apply when ShellType = Shell_layered</param>
        /// <returns></returns>
        public static ShellProp Define(string Name, int ShellType = 1, bool DOF = true, string MatProp = "4000Psi", double MatAngle= 0, double Thickness = 0, double Bending = 0)
        {
            string mat = SAPConnection.MaterialMapper.DynamoToSap(MatProp);
            return new ShellProp(Name, ShellType, DOF, mat, MatAngle, Thickness, Bending);
        }

        /// <summary>
        /// Decompose a Section Property
        /// </summary>
        /// <param name="ShellProp"></param>
        /// <returns> Name, ShellType, DOF, MatProp, MatAngle, Thickness, Bending </returns>
        [MultiReturn("Name", "ShellType", "DOF", "MatProp", "MatAngle", "Thickness", "Bending")]
        public static Dictionary<string, object> Decompose(ShellProp ShellProp)
        {
            return new Dictionary<string, object>
            {
                {"Name", ShellProp.PropName},
                {"ShellType", ShellProp.ShellType},
                {"DOF", ShellProp.DOF},
                {"MatProp", ShellProp.MatProp},
                {"MatAngle", ShellProp.MatAngle},
                {"Thickness", ShellProp.Thickness},
                {"Bending", ShellProp.Bending}
            };
        }

        //PRIVATE CONSTRUCTORS
        internal ShellProp() { }
        internal ShellProp(string _name, int _shellType, bool _dof, string _matprop, double _matAngle, double _thickness, double _bending)
        {
            PropName = _name;
            ShellType =_shellType;
            DOF = _dof;
            MatProp = _matprop;
            MatAngle = _matAngle;
            Thickness = _thickness;
            Bending = _bending;
        }

    }
}
