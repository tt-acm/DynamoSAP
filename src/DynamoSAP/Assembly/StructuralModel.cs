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
        [SupressImportIntoVMAttribute]
        public List<Element> StructuralElements { get; set; }
        [SupressImportIntoVMAttribute]
        public List<LoadPattern> LoadPatterns { get; set; }
        [SupressImportIntoVMAttribute]
        public List<LoadCase> LoadCases { get; set; }
        [SupressImportIntoVMAttribute]
        public List<Restraint> Restraints { get; set; }
        

        // Check that the GUID does not exist, in case users added the same element twice
        private static void CheckDuplicateFrame(List<Element> StructEl)
        {
            // Dictionary to hold Structure Frames on <string, string> <GUID,Label>
            List<string> SapModelFrmList = new List<string>();
            foreach (Element el in StructEl)
            {
                if (!SapModelFrmList.Contains(el.Label))
                {
                    SapModelFrmList.Add(el.Label);
                }
                // If the key exists, throw an error in the collector
                else
                {
                    throw new Exception("A structural element has been added twice. Please, make sure you do not have duplicate elements");
                }
            }
        }

        /// <summary>
        /// Collects Structural Elements into a Structural Model
        /// </summary>
        /// <param name="StructuralElements">Structural elements in the project. Please, input as a flat list</param>
        /// <returns>Structural Model consisting of the structural elements provided</returns>
        public static StructuralModel Collector(List<Element> StructuralElements)
        {
            CheckDuplicateFrame(StructuralElements);
            StructuralModel mySt = new StructuralModel();
            mySt.StructuralElements = StructuralElements;
            return mySt;
        }

        /// <summary>
        /// Collects Structural Elements, Load Patterns, Load Cases and Restraints into a Structural Model
        /// </summary>
        /// <param name="StructuralElements">Structural elements in the project. Please, input as a flat list</param>
        /// <param name="LoadPatterns">Load Patterns in the project. Please, input as a flat list</param>
        /// <param name="LoadCases">Load Cases in the project. Please, input as a flat list</param>
        /// <param name="Restraints">Restraints in the project. Please, input as a flat list</param>
        /// <returns>Structural Model consisting of all the elements provided</returns>
        public static StructuralModel Collector(List<Element> StructuralElements, List<LoadPattern> LoadPatterns, List<LoadCase> LoadCases, List<Restraint> Restraints)
        {
            CheckDuplicateFrame(StructuralElements);
            return new StructuralModel(StructuralElements, LoadPatterns, LoadCases, Restraints);
        }

        /// <summary>
        /// Decomposes a Structural Model into its geometry and structural settings
        /// </summary>
        /// <param name="structuralModel">Structural Model to decompose </param>
        /// <returns>Structural Elements, Load Patterns, Load Cases and Restraints of the project </returns>
        [MultiReturn("Structural Elements", "Load Patterns", "Load Cases", "Restraints")]
        public static Dictionary<string, object> Decompose(StructuralModel structuralModel)
        {
            // Return outputs
            return new Dictionary<string, object>
            {
                {"Structural Elements", structuralModel.StructuralElements},
                {"Load Patterns", structuralModel.LoadPatterns},
                {"Load Cases", structuralModel.LoadCases},
                {"Restraints", structuralModel.Restraints}
               
            };
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
