using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DynamoSAP.Structure;

//DYNAMO
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;

namespace DynamoSAP.Assembly
{
    public class StructuralModel : IModel
    {
        public List<Element> StructuralElements { get; set; }

        public List<LoadPattern> LoadPatterns { get; set; }

        public List<LoadCase> LoadCases { get; set; }

        public List<Restraint> Restraints { get; set; }

        // Check that GUID does not exist, users must add the same frame twice
        private static void CheckDuplicateFrame(List<Element> StructEl)
        {
            // Dictionary to hold Structure Frames on <string, string> <GUID,Label>
            Dictionary<string, string> SapModelFrmDict = new Dictionary<string, string>();
            foreach (Element el in StructEl)
            {
                if (!SapModelFrmDict.Keys.Contains(el.GUID))
                {
                    SapModelFrmDict.Add(el.GUID, el.Label);
                }
                // If the key exists, throw an error in the collector
                else
                {
                    throw new Exception("A structural element has been added twice. Please, make sure you do not have duplicate elements");
                }
            }
        }

        public static StructuralModel Collector(List<Element> StructuralElements)
        {
            CheckDuplicateFrame(StructuralElements);
            StructuralModel mySt = new StructuralModel();
            mySt.StructuralElements = StructuralElements;
            return mySt;
        }

        public static StructuralModel Collector(List<Element> StructuralElements, List<LoadPattern> LoadPatterns, List<LoadCase> LoadCases, List<Restraint> Restraints)
        {
            CheckDuplicateFrame(StructuralElements);
            return new StructuralModel(StructuralElements, LoadPatterns, LoadCases, Restraints);
        }

        internal StructuralModel() { }

        private StructuralModel(List<Element> Elements, List<LoadPattern> loadPatterns, List<LoadCase> loadCases, List<Restraint> restraints)
        {
            StructuralElements = Elements;
            LoadPatterns = loadPatterns;
            LoadCases = loadCases;
            Restraints = restraints;

        }
    }
}
