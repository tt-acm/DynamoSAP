/// Developed by Thornton Tomasetti's CORE Studio for Autodesk
/// http://core.thorntontomasetti.com
/// CORE Developers: Elcin Ertugrul and Ana Garcia Puyol

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using Dynamo.Models;
using SAP2000v16;
using DSCoreNodesUI;
using Dynamo.Nodes;
using ProtoCore.AST.AssociativeAST;

namespace DynamoSAP_UI
{
    public abstract class DynamoSAPStringDropdownBase : DSDropDownBase
	{
        /// <summary>
        /// Constructor for this abstract class.  Pass in the name of the node, and the enum to convert
        /// </summary>
        /// <param name="value">The name of the node</param>
        /// <param name="e">The enum to populate the dropdown list with</param>
        public DynamoSAPStringDropdownBase(string value, Enum e)
            : base(value) 
        {
            stringsFromEnum(e);
        }

        /// <summary>
        /// A local variable to store the list of strings representing the enum
        /// </summary>
        private List<string> myDropdownItems = new List<string>();

        /// <summary>
        /// Populate our local list of strings using the enum that was passed into the constructor
        /// </summary>
        /// <param name="e"></param>
        private void stringsFromEnum(Enum e) 
        {
            foreach (var i in Enum.GetValues(e.GetType()))
            {
                myDropdownItems.Add(i.ToString());
            }
        }

        /// <summary>
        /// The populate items override.  Not sure why this gets called before the constructor, but it does!
        /// </summary>
        public override void PopulateItems()
        {
            Items.Clear();
            foreach (var i in myDropdownItems)
            {
                Items.Add(new DynamoDropDownItem(i, i)); 
            }
            SelectedIndex = 0;
        }

        /// <summary>
        /// Absolutely no clue what this does.  I found an example here and modified it: https://github.com/DynamoDS/DynamoRevit/blob/Revit2015/src/Libraries/RevitNodesUI/RevitDropDown.cs
        /// Ian also helped with this link... https://github.com/DynamoDS/Dynamo/commit/19d37337742f87bbf4bc6283de10ee7bbf7927a1  looks like everything is working again now
        /// </summary>
        /// <param name="inputAstNodes"></param>
        /// <returns></returns>
        public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
        {
            if (Items.Count == 0 || Items.Count == -1)
            {
                PopulateItems();
            }

            var stringNode = AstFactory.BuildStringNode((string)Items[SelectedIndex].Item);
            var assign = AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), stringNode);

            return new List<AssociativeNode> { assign };
        }
    }

    public abstract class DynamoSAPIntDropdownBase : DSDropDownBase
    {
        /// <summary>
        /// Constructor for this abstract class.  Pass in the name of the node, and the enum to convert
        /// </summary>
        /// <param name="value">The name of the node</param>
        /// <param name="e">The enum to populate the dropdown list with</param>
        public DynamoSAPIntDropdownBase(string value, Enum e)
            : base(value)
        {
            stringsFromEnum(e);
        }

        /// <summary>
        /// A local variable to store the list of strings representing the enum
        /// </summary>
        private List<string> myDropdownItems = new List<string>();

        /// <summary>
        /// Populate our local list of strings using the enum that was passed into the constructor
        /// </summary>
        /// <param name="e"></param>
        private void stringsFromEnum(Enum e)
        {
            foreach (var i in Enum.GetValues(e.GetType()))
            {
                myDropdownItems.Add(i.ToString());
            }
        }

        /// <summary>
        /// The populate items override.  Not sure why this gets called before the constructor, but it does!
        /// </summary>
        public override void PopulateItems()
        {
            Items.Clear();
            for (int i = 0; i < myDropdownItems.Count; i++)
            {
                Items.Add(new DynamoDropDownItem(myDropdownItems[i], i));
            }
            SelectedIndex = 0;
        }

        /// <summary>
        /// Absolutely no clue what this does.  I found an example here and modified it: https://github.com/DynamoDS/DynamoRevit/blob/Revit2015/src/Libraries/RevitNodesUI/RevitDropDown.cs
        /// Ian also helped with this link... https://github.com/DynamoDS/Dynamo/commit/19d37337742f87bbf4bc6283de10ee7bbf7927a1  looks like everything is working again now
        /// </summary>
        /// <param name="inputAstNodes"></param>
        /// <returns></returns>
        public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
        {
            if (Items.Count == 0 || Items.Count == -1)
            {
                PopulateItems();
            }

            var intNode = AstFactory.BuildIntNode((int)Items[SelectedIndex].Item);
            var assign = AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), intNode);

            return new List<AssociativeNode> { assign };
        }
    }


	[NodeName("LoadPatternTypes")]
    [NodeCategory("DynamoSAP.Definitions.LoadPattern")]
    [NodeDescription("Select Load Pattern type to use with Set Load Pattern node")]
    [IsDesignScriptCompatible]
    public class LoadPatternTypes : DynamoSAPStringDropdownBase
    {
        public LoadPatternTypes() : base(">", new eLoadPatternType()) { }
    }

    [NodeName("LoadCaseTypes")]
    [NodeCategory("DynamoSAP.Definitions.LoadCase")]
    [NodeDescription("Select Load Case type to use with Set Load Case node")]
    [IsDesignScriptCompatible]
    public class LoadCaseTypes : DynamoSAPStringDropdownBase
    {
        public LoadCaseTypes() : base(">", new eLoadCaseType()) { }
    }

    [NodeName("LoadComboTypes")]
    [NodeCategory("DynamoSAP.Definitions.LoadCombo")]
    [NodeDescription("Select Load Combo type to use with Set Load Combo node")]
    [IsDesignScriptCompatible]
    public class LoadComboTypes : DynamoSAPStringDropdownBase
    {
        public LoadComboTypes() : base(">", new eCType()) { }
    }

    [NodeName("CoordinateSystem")]
    [NodeCategory("DynamoSAP.Definitions.Load")]
    [NodeDescription("Select the Coordinate System to use with Load nodes")]
    [IsDesignScriptCompatible]
    public class CoordinateSystem : DynamoSAPStringDropdownBase
    {
        public CoordinateSystem() : base(">", new CSystem()) { }
    }

    [NodeName("LoadDirection")]
    [NodeCategory("DynamoSAP.Definitions.Load")]
    [NodeDescription("Select the Direction of the Load ")]
    [IsDesignScriptCompatible]
    //public class LoadDirection : EnumAsInt<LDir>
    public class LoadDirection : DynamoSAPIntDropdownBase
    {
        public LoadDirection():base(">",new LDir()){}
    }


    [NodeName("LoadType")]
    [NodeCategory("DynamoSAP.Definitions.Load")]
    [NodeDescription("Select the Load Type to use with Load nodes")]
    [IsDesignScriptCompatible]
    public class LoadType : DynamoSAPStringDropdownBase
    {
        public LoadType() : base(">", new LType()) { }
    }

    [NodeName("Justifications")]
    [NodeCategory("DynamoSAP.Structure.Frame")]
    [NodeDescription("Select Justification to use with Create Frame nodes")]
    [IsDesignScriptCompatible]
    public class JustificationTypes : DynamoSAPIntDropdownBase
    {
        public JustificationTypes() : base(">", new Justification()) { }
    }

    public enum CSystem
    {
        Global = 1,
        Local = 2,
    }
    public enum LDir
    {
        a_LocalX = 1,
        a_LocalY = 2,
        a_LocalZ = 3,
        b_GlobalX = 4,
        b_GlobalY = 5,
        b_GlobalZ = 6,
        c_GlobalProjectedX = 7,
        c_GlobalProjectedY = 8,
        c_GlobalProjectedZ = 9,
        Gravity = 10,
        ProjectedGravity = 11,
    }

    public enum LType // Dynamo changes the order, and starts from 0
    {
        Force = 1,
        Moment = 2,
    }

    public enum Justification
    {
        BottomLeft = 1,
        BottomCenter = 2,
        BottomRight = 3,
        MiddleLeft = 4,
        MiddleCenter = 5,
        MiddleRight = 6,
        TopLeft = 7,
        TopCenter = 8,
        TopRight = 9,
        Centroid = 10,
        ShearCenter = 11
    }

    [NodeName("ForceTypes")]
    [NodeCategory("DynamoSAP.Analysis.Analysis")]
    [NodeDescription("Select Force Type to use with Decompose Result component")]
    [IsDesignScriptCompatible]
    public class ForceTypes : DynamoSAPStringDropdownBase
    {
        public ForceTypes() : base(">", new ForceType()) { }
    }

    public enum ForceType
    {
        Axial = 1,
        Shear22 = 2,
        Shear33 = 3,
        Torsion = 4,
        Moment22 = 5,
        Moment33 = 6,
    }


    [NodeName("SectionCatalogs")]
    [NodeCategory("DynamoSAP.Definitions.SectionProp")]
    [NodeDescription("Select Section Catalog as input Sections Node to retrive the section names of selected catalog")]
    [IsDesignScriptCompatible]
    public class SectionCatalogs : DynamoSAPStringDropdownBase
    {
        public SectionCatalogs() : base(">", new SectionCatalog()) { }
    }
    public enum SectionCatalog
    {
        AISC,
        AISC3,
        AISC13,
        AISC13M,
        AISC14,
        AISC14M,
        AISCASD9,
        AISCLRFD1,
        AISCLRFD2,
        AISCLRFD3,
        Aluminum,
        AusNZV8,
        BSShapes,
        BSShapes2006,
        Chinese,
        ChineseGB08,
        CISC,
        CISC9,
        CISC10,
        EURO,
        Indian,
        joists,
        SECTIONS,
        SECTIONS8
    }

    [NodeName("Materials")]
    [NodeCategory("DynamoSAP.Definitions")]
    [NodeDescription("Select Materials to set Section Property")]
    [IsDesignScriptCompatible]
    public class Materials : DynamoSAPStringDropdownBase
    {
        public Materials() : base(">", new Material()) { }
    }

    public enum Material
    {
        A36,
        A53GrB,
        A500GrB42,
        A500GrB46,
        A572Gr50,
        A913Gr50,
        A992Fy50,
        Concrete_4000Psi
    }


    [NodeName("Units")]
    [NodeCategory("DynamoSAP.Assembly")]
    [NodeDescription("Select units")]
    [IsDesignScriptCompatible]
    public class Units : DynamoSAPStringDropdownBase
    {
        public Units() : base(">", new eUnits()) { }
    }

    [NodeName("ShellTypes")]
    [NodeCategory("DynamoSAP.Definitions.ShellProp")]
    [NodeDescription("Shell Types")]
    [IsDesignScriptCompatible]
    public class ShellTypes : DynamoSAPIntDropdownBase
    {
        public ShellTypes() : base(">", new ShellType()) { }
    }


    public enum ShellType
    {
        Shell_thin = 1,
        Shell_thick,
        Plate_thin,
        Plate_thick,
        Membrane,
        Shell_layered
    } 
	
}
