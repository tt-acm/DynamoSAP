/// Developed by Thornton Tomasetti's CORE Studio for Autodesk
/// http://core.thorntontomasetti.com
/// CORE Developers: Elcin Ertugrul and Ana Garcia Puyol

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using CoreNodeModels;
using SAP2000v16;
using Dynamo.Utilities;
using Dynamo.Graph.Nodes;
using ProtoCore.AST.AssociativeAST;

namespace DynamoSAP_UI
{
    [NodeNameAttribute("LoadPatternTypes")]
    [NodeCategoryAttribute("DynamoSAP.Definitions.LoadPattern")]
    [NodeDescriptionAttribute("Select Load Pattern type to use with Set Load Pattern node")]
    [IsDesignScriptCompatibleAttribute]
    public class LoadPatternTypes : CoreNodeModels.DSDropDownBase
    {
        
        public LoadPatternTypes() : base(">") { }

        public override void PopulateItems()
        {
            //clear items
            Items.Clear();

            //set up the collection
            var newItems = new List<DynamoDropDownItem>();
            foreach (var j in Enum.GetValues(new eLoadPatternType().GetType())) 
            {
                newItems.Add(new DynamoDropDownItem(j.ToString(), j.ToString()));
            }
            Items.AddRange(newItems);

            //set the selected index to 0
            SelectedIndex = 0;
        }
        public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
        {
            // Build an AST node for the type of object contained in your Items collection.

            var intNode = AstFactory.BuildStringNode((string)Items[SelectedIndex].Item);
            var assign = AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), intNode);

            return new List<AssociativeNode> { assign };
        }
    }

    [NodeName("LoadCaseTypes")]
    [NodeCategory("DynamoSAP.Definitions.LoadCase")]
    [NodeDescription("Select Load Case type to use with Set Load Case node")]
    [IsDesignScriptCompatible]
    public class LoadCaseTypes : DSDropDownBase
    {
        public LoadCaseTypes() : base(">") { }

        public override void PopulateItems()
        {
            //clear items
            Items.Clear();

            //set up the collection
            var newItems = new List<DynamoDropDownItem>();
            foreach (var j in Enum.GetValues(new eLoadCaseType().GetType())) 
            {
                newItems.Add(new DynamoDropDownItem(j.ToString(), j.ToString()));
            }
            Items.AddRange(newItems);

            //set the selected index to 0
            SelectedIndex = 0;
        }
        public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
        {
            // Build an AST node for the type of object contained in your Items collection.

            var intNode = AstFactory.BuildStringNode((string)Items[SelectedIndex].Item);
            var assign = AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), intNode);

            return new List<AssociativeNode> { assign };
        }
    }

    [NodeName("LoadComboTypes")]
    [NodeCategory("DynamoSAP.Definitions.LoadCombo")]
    [NodeDescription("Select Load Combo type to use with Set Load Combo node")]
    [IsDesignScriptCompatible]
    public class LoadComboTypes : DSDropDownBase
    {
       public LoadComboTypes() : base(">") { }

        public override void PopulateItems()
        {
            //clear items
            Items.Clear();

            //set up the collection
            var newItems = new List<DynamoDropDownItem>();
            foreach (var j in Enum.GetValues(new eCType().GetType())) 
            {
                newItems.Add(new DynamoDropDownItem(j.ToString(), j.ToString()));
            }
            Items.AddRange(newItems);

            //set the selected index to 0
            SelectedIndex = 0;
        }
        public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
        {
            // Build an AST node for the type of object contained in your Items collection.

            var intNode = AstFactory.BuildStringNode((string)Items[SelectedIndex].Item);
            var assign = AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), intNode);

            return new List<AssociativeNode> { assign };
        }
    }

    [NodeName("CoordinateSystem")]
    [NodeCategory("DynamoSAP.Definitions.Load")]
    [NodeDescription("Select the Coordinate System to use with Load nodes")]
    [IsDesignScriptCompatible]
    public class CoordinateSystem : DSDropDownBase
    {
       public CoordinateSystem() : base(">") { }

        public override void PopulateItems()
        {
            //clear items
            Items.Clear();

            //set up the collection
            var newItems = new List<DynamoDropDownItem>();
            foreach (var j in Enum.GetValues(new CSystem().GetType())) 
            {
                newItems.Add(new DynamoDropDownItem(j.ToString(), j.ToString()));
            }
            Items.AddRange(newItems);

            //set the selected index to 0
            SelectedIndex = 0;
        }
        public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
        {
            // Build an AST node for the type of object contained in your Items collection.

            var intNode = AstFactory.BuildStringNode((string)Items[SelectedIndex].Item);
            var assign = AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), intNode);

            return new List<AssociativeNode> { assign };
        }
    }

    [NodeName("LoadDirection")]
    [NodeCategory("DynamoSAP.Definitions.Load")]
    [NodeDescription("Select the Direction of the Load ")]
    [IsDesignScriptCompatible]
    //public class LoadDirection : EnumAsInt<LDir>
    public class LoadDirection : DSDropDownBase
    {
       public LoadDirection() : base(">") { }

        public override void PopulateItems()
        {
            //clear items
            Items.Clear();

            //set up the collection
            var newItems = new List<DynamoDropDownItem>();
            foreach (var j in Enum.GetValues(new LDir().GetType())) 
            {
                newItems.Add(new DynamoDropDownItem(j.ToString(), j));
            }
            Items.AddRange(newItems);

            //set the selected index to 0
            SelectedIndex = 0;
        }
        public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
        {
            // Build an AST node for the type of object contained in your Items collection.

            var intNode = AstFactory.BuildIntNode((int)Items[SelectedIndex].Item);
            var assign = AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), intNode);

            return new List<AssociativeNode> { assign };
        }
    }


    [NodeName("LoadType")]
    [NodeCategory("DynamoSAP.Definitions.Load")]
    [NodeDescription("Select the Load Type to use with Load nodes")]
    [IsDesignScriptCompatible]
    public class LoadType : DSDropDownBase
    {
       public LoadType() : base(">") { }

        public override void PopulateItems()
        {
            //clear items
            Items.Clear();

            //set up the collection
            var newItems = new List<DynamoDropDownItem>();
            foreach (var j in Enum.GetValues(new LType().GetType())) 
            {
                newItems.Add(new DynamoDropDownItem(j.ToString(), j.ToString()));
            }
            Items.AddRange(newItems);

            //set the selected index to 0
            SelectedIndex = 0;
        }
        public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
        {
            // Build an AST node for the type of object contained in your Items collection.

            var intNode = AstFactory.BuildStringNode((string)Items[SelectedIndex].Item);
            var assign = AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), intNode);

            return new List<AssociativeNode> { assign };
        }
    }

    [NodeName("Justifications")]
    [NodeCategory("DynamoSAP.Structure.Frame")]
    [NodeDescription("Select Justification to use with Create Frame nodes")]
    [IsDesignScriptCompatible]
    public class JustificationTypes : DSDropDownBase
    {
        public JustificationTypes() : base(">") { }

        public override void PopulateItems()
        {
            //clear items
            Items.Clear();

            //set up the collection
            var newItems = new List<DynamoDropDownItem>();
            foreach (var j in Enum.GetValues(new Justification().GetType())) 
            {
                newItems.Add(new DynamoDropDownItem(j.ToString(), j.ToString()));
            }
            Items.AddRange(newItems);

            //set the selected index to 0
            SelectedIndex = 0;
        }
        public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
        {
            // Build an AST node for the type of object contained in your Items collection.

            var intNode = AstFactory.BuildStringNode((string)Items[SelectedIndex].Item);
            var assign = AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), intNode);

            return new List<AssociativeNode> { assign };
        }
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
    public class ForceTypes : DSDropDownBase
    {
        public ForceTypes() : base(">") { }

        public override void PopulateItems()
        {
            //clear items
            Items.Clear();

            //set up the collection
            var newItems = new List<DynamoDropDownItem>();
            foreach (var j in Enum.GetValues(new ForceType().GetType())) 
            {
                newItems.Add(new DynamoDropDownItem(j.ToString(), j.ToString()));
            }
            Items.AddRange(newItems);

            //set the selected index to 0
            SelectedIndex = 0;
        }
        public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
        {
            // Build an AST node for the type of object contained in your Items collection.

            var intNode = AstFactory.BuildStringNode((string)Items[SelectedIndex].Item);
            var assign = AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), intNode);

            return new List<AssociativeNode> { assign };
        }
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
    public class SectionCatalogs : DSDropDownBase
    {
       public SectionCatalogs() : base(">") { }

        public override void PopulateItems()
        {
            //clear items
            Items.Clear();

            //set up the collection
            var newItems = new List<DynamoDropDownItem>();
            foreach (var j in Enum.GetValues(new SectionCatalog().GetType())) 
            {
                newItems.Add(new DynamoDropDownItem(j.ToString(), j.ToString()));
            }
            Items.AddRange(newItems);

            //set the selected index to 0
            SelectedIndex = 0;
        }
        public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
        {
            // Build an AST node for the type of object contained in your Items collection.

            var intNode = AstFactory.BuildStringNode((string)Items[SelectedIndex].Item);
            var assign = AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), intNode);

            return new List<AssociativeNode> { assign };
        }
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
    public class Materials : DSDropDownBase
    {
        public Materials() : base(">") { }

        public override void PopulateItems()
        {
            //clear items
            Items.Clear();

            //set up the collection
            var newItems = new List<DynamoDropDownItem>();
            foreach (var j in Enum.GetValues(new Material().GetType())) 
            {
                newItems.Add(new DynamoDropDownItem(j.ToString(), j.ToString()));
            }
            Items.AddRange(newItems);

            //set the selected index to 0
            SelectedIndex = 0;
        }
        public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
        {
            // Build an AST node for the type of object contained in your Items collection.

            var intNode = AstFactory.BuildStringNode((string)Items[SelectedIndex].Item);
            var assign = AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), intNode);

            return new List<AssociativeNode> { assign };
        }
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
    public class Units : DSDropDownBase
    {
        public Units() : base(">") { }

        public override void PopulateItems()
        {
            //clear items
            Items.Clear();

            //set up the collection
            var newItems = new List<DynamoDropDownItem>();
            foreach (var j in Enum.GetValues(new eUnits().GetType())) 
            {
                newItems.Add(new DynamoDropDownItem(j.ToString(), j.ToString()));
            }
            Items.AddRange(newItems);

            //set the selected index to 0
            SelectedIndex = 0;
        }
        public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
        {
            // Build an AST node for the type of object contained in your Items collection.

            var intNode = AstFactory.BuildStringNode((string)Items[SelectedIndex].Item);
            var assign = AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), intNode);

            return new List<AssociativeNode> { assign };
        }
    }

    [NodeName("ShellTypes")]
    [NodeCategory("DynamoSAP.Definitions.ShellProp")]
    [NodeDescription("Shell Types")]
    [IsDesignScriptCompatible]
    public class ShellTypes : DSDropDownBase
    {
        public ShellTypes() : base(">") { }

        public override void PopulateItems()
        {
            //clear items
            Items.Clear();

            //set up the collection
            var newItems = new List<DynamoDropDownItem>();
            foreach (var j in Enum.GetValues(new ShellType().GetType())) 
            {
                newItems.Add(new DynamoDropDownItem(j.ToString(), j));
            }
            Items.AddRange(newItems);

            //set the selected index to 0
            SelectedIndex = 0;
        }
        public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
        {
            // Build an AST node for the type of object contained in your Items collection.

            var intNode = AstFactory.BuildIntNode((int)Items[SelectedIndex].Item);
            var assign = AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), intNode);

            return new List<AssociativeNode> { assign };
        }
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
