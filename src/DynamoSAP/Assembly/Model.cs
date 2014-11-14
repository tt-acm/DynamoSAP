using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DynamoSAP.Structure;
using DynamoSAP.Structure;

//DYNAMO
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;

namespace DynamoSAP.Assembly
{
    public class Model : IModel
    {

        public List<Element> Frames { get; set; }

        public List<LoadPattern> LoadPatterns { get; set; }

        public List<LoadCase> LoadCases { get; set; }

        public List<Restraint> Restraints { get; set; }

        public List<Load> Loads { get; set; }

        public List<Release> Releases { get; set; }

        public static Model Collector_Frames(List<Element> frames)
        {
            Model mySt = new Model();
            mySt.Frames = frames;
            return mySt;
        }

        public static Model Collector(List<Element> frames, List<LoadPattern> loadPatterns, List<LoadCase> loadCases, List<Restraint> restraints, List<Load> loads, List<Release> releases)
        {
            return new Model(frames, loadPatterns, loadCases, restraints, loads, releases);
        }

        private Model() { }

        private Model(List<Element> frames, List<LoadPattern> loadPatterns, List<LoadCase> loadCases, List<Restraint> restraints, List<Load> loads, List<Release> releases)
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
