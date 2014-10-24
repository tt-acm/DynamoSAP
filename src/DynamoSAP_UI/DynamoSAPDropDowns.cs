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
    [NodeDescription("Select Load Pattern from the dropdown")]
    [IsDesignScriptCompatible]
    public class LoadPatternTypeDropDown: EnumAsInt<eLoadPatternType>
    {
        public LoadPatternTypeDropDown(WorkspaceModel workspace) : base(workspace) { }
    }
}
