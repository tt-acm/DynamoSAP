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
        public List<Element> Frames { get; set; }

        public List<LoadPattern> LoadPatterns { get; set; }

        public List<LoadCase> LoadCases { get; set; }

        public List<Restraint> Restraints { get; set; }

        public List<Load> Loads { get; set; }

        public List<Release> Releases { get; set; }

        public static StructuralModel Collector_Frames(List<Element> frames)
        {
            StructuralModel mySt = new StructuralModel();
            mySt.Frames = frames;
            return mySt;
        }

        public static StructuralModel Collector(List<Element> frames, List<LoadPattern> loadPatterns, List<LoadCase> loadCases, List<Restraint> restraints, List<Load> loads, List<Release> releases)
        {
            return new StructuralModel(frames, loadPatterns, loadCases, restraints, loads, releases);
        }

        private StructuralModel() { }

        private StructuralModel(List<Element> frames, List<LoadPattern> loadPatterns, List<LoadCase> loadCases, List<Restraint> restraints, List<Load> loads, List<Release> releases)
        {
            Frames = frames;
            LoadPatterns = loadPatterns;
            LoadCases = loadCases;
            Restraints = restraints;
            Loads = loads;
            Releases = releases;
        }
    }
}
