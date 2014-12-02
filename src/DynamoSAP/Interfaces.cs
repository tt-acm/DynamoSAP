using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DynamoSAP.Structure;

//DYNAMO
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;

namespace DynamoSAP
{
    [SupressImportIntoVM]
    public interface IModel
    {
        List<Element> Frames { get; }
        List<LoadPattern> LoadPatterns { get; }
        List<LoadCase> LoadCases { get; }
        List<Restraint> Restraints { get; }
        List<Load> Loads { get; }
        List<Release> Releases { get; }
    }

    

}
