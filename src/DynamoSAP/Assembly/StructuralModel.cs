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


        public static StructuralModel Collector(List<Element> StructuralElements)
        {
            StructuralModel mySt = new StructuralModel();
            mySt.StructuralElements = StructuralElements;
            return mySt;
        }

        public static StructuralModel Collector(List<Element> StructuralElements, List<LoadPattern> LoadPatterns, List<LoadCase> LoadCases, List<Restraint> Restraints)
        {
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
