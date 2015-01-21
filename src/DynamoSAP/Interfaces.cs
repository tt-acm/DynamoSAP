using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DynamoSAP.Structure;
using DynamoSAP.Definitions;

//DYNAMO
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;

namespace DynamoSAP
{
    [SupressImportIntoVM]
    public interface IModel
    {
        List<Element> StructuralElements { get; }
        List<LoadPattern> LoadPatterns { get; }
        List<LoadCase> LoadCases { get; }
    }

    

}
