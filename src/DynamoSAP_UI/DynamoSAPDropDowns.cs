using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using DSCoreNodesUI;
using Dynamo.Models;
using SAP2000v16;

namespace DynamoSAP_UI
{
    [NodeName("LoadPatternTypes")]
    [NodeCategory("DynamoSAP.Definitions.LoadPattern")]
    [NodeDescription("Select Load Pattern type to use with Set Load Pattern node")]
    [IsDesignScriptCompatible]
    public class LoadPatternTypes: EnumAsString<eLoadPatternType>
    {
        public LoadPatternTypes(WorkspaceModel workspace) : base(workspace) { }
    }

    [NodeName("LoadCaseTypes")]
    [NodeCategory("DynamoSAP.Definitions.LoadCase")]
    [NodeDescription("Select Load Case type to use with Set Load Case node")]
    [IsDesignScriptCompatible]
    public class LoadCaseTypes : EnumAsString<eLoadCaseType>
    {
        public LoadCaseTypes(WorkspaceModel workspace) : base(workspace) { }
    }

    [NodeName("LoadComboTypes")]
    [NodeCategory("DynamoSAP.Definitions.LoadCombo")]
    [NodeDescription("Select Load Combo type to use with Set Load Combo node")]
    [IsDesignScriptCompatible]
    public class LoadComboTypes : EnumAsString<eCType>
    {
        public LoadComboTypes(WorkspaceModel workspace) : base(workspace) { }
    }

    [NodeName("CoordinateSystem")]
    [NodeCategory("DynamoSAP.Definitions.Load")]
    [NodeDescription("Select Coordinate System to use with Load nodes")]
    [IsDesignScriptCompatible]
    public class CoordinateSystem : EnumAsString<CSystem>
    {
        public CoordinateSystem(WorkspaceModel workspace) : base(workspace) { }
    }

    [NodeName("Justifications")]
    [NodeCategory("DynamoSAP.Structure.Frame")]
    [NodeDescription("Select Justification to use with Create Frame nodes")]
    [IsDesignScriptCompatible]
    public class JustificationTypes : EnumAsString<Justification>
    {
        public JustificationTypes(WorkspaceModel workspace) : base(workspace) { }
    }

    public enum CSystem // Dynamo change the order, ans starts at 0
    {
        Global = 1,
        Local = 2,
    }

    public enum Justification // Dynamo change the order, ans starts at 0
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
    public class ForceTypes : EnumAsString<ForceType>
    {
        public ForceTypes(WorkspaceModel workspace) : base(workspace) { }
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
    public class SectionCatalogs : EnumAsString<SectionCatalog>
    {
        public SectionCatalogs(WorkspaceModel workspace) : base(workspace) { }
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
    public class Materials : EnumAsString<Material>
    {
        public Materials(WorkspaceModel workspace) : base(workspace) { }
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
    public class Units: EnumAsString<eUnits>
    {
        public Units(WorkspaceModel workspace) : base(workspace) { }
    }

    [NodeName("ShellTypes")]
    [NodeCategory("DynamoSAP.Definitions.ShellProp")]
    [NodeDescription("Shell Types")]
    [IsDesignScriptCompatible]
    public class ShellTypes : EnumAsString<ShellType>
    {
        public ShellTypes(WorkspaceModel workspace) : base(workspace) { }
    }

 
    public enum ShellType
    {
        Shell_thin = 1,
        Shell_thick ,
        Plate_thin,
        Plate_thick,
        Membrane,
        Shell_layered
    }
}
