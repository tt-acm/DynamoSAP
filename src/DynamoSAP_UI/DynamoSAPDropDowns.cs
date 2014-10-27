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
    [NodeName("LoadPatternTypeDropDown")]
    [NodeCategory("DynamoSAP.DynamoSAP.Analysis.LoadPattern")]
    [NodeDescription("Select Load Pattern to use with Set Load Pattern node")]
    [IsDesignScriptCompatible]
    public class LoadPatternTypeDropDown: EnumAsString<eLoadPatternType>
    {
        public LoadPatternTypeDropDown(WorkspaceModel workspace) : base(workspace) { }
    }

    [NodeName("LoadCaseTypeDropDown")]
    [NodeCategory("DynamoSAP.DynamoSAP.Analysis.LoadCase")]
    [NodeDescription("Select Load Case to use with Set Load Case node")]
    [IsDesignScriptCompatible]
    public class LoadCaseTypeDropDown : EnumAsString<eLoadCaseType>
    {
        public LoadCaseTypeDropDown(WorkspaceModel workspace) : base(workspace) { }
    }

    // TODO: Add LoadDirectionDropDown Global or Local CS

    [NodeName("JustificationDropDown")]
    [NodeCategory("DynamoSAP.DynamoSAP.Structure.Frame")]
    [NodeDescription("Select Justification to use with Create Frame nodes")]
    [IsDesignScriptCompatible]
    public class JustificationTypeDropdown : EnumAsString<Justification>
    {
        public JustificationTypeDropdown(WorkspaceModel workspace) : base(workspace) { }
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
}
