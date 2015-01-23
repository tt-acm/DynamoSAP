using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DynamoSAP.Structure;
using DynamoSAP.Definitions;

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
        public List<Definition> ModelDefinitions { get; set; }


        // Check that the label does not exist, in case users added the same element twice
        private static void CheckDuplicates(List<Element> StructEl)
        {
            int errorcounter = 0;
            List<string> duplicates = new List<string>();
            List<string> frames = new List<string>();
            List<string> shells = new List<string>();
            List<string> joints = new List<string>();

            foreach (Element el in StructEl)
            {
                if (el.Type == Structure.Type.Frame)
                {
                    if (!frames.Contains(el.Label))
                    {
                        frames.Add(el.Label);
                    }
                    else
                    {
                        errorcounter++;
                        duplicates.Add("Frame: " + el.Label);

                    }
                }
                if (el.Type == Structure.Type.Shell)
                {
                    if (!shells.Contains(el.Label))
                    {
                        shells.Add(el.Label);
                    }
                    else
                    {
                        errorcounter++;
                        duplicates.Add("Shell: " + el.Label);

                    }
                }
                if (el.Type == Structure.Type.Joint)
                {
                    if (!joints.Contains(el.Label))
                    {
                        joints.Add(el.Label);
                    }
                    else
                    {
                        errorcounter++;
                        duplicates.Add("Joint: " + el.Label);
                    }
                }
            }

            if (errorcounter > 0)
            {
                string errorMessage = "One or more structural elements have been added twice: ";
                for (int i = 0; i < duplicates.Count; i++)
                {
                    errorMessage += duplicates[i] + " ";
                }
                // pass  this to  error log
                throw new Exception(errorMessage);
            }

        }

        /// <summary>
        /// Collects Structural Elements into a Structural Model
        /// </summary>
        /// <param name="StructuralElements">Structural elements in the project. Please, input as a flat list</param>
        /// <returns>Structural Model consisting of the structural elements provided</returns>
        public static StructuralModel Collector(List<Element> StructuralElements)
        {
            CheckDuplicates(StructuralElements);
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
        /// <returns>Structural Model consisting of all the elements provided</returns>
        public static StructuralModel Collector(List<Element> StructuralElements, List<Definition> Definitions)
        {
            CheckDuplicates(StructuralElements);
            return new StructuralModel(StructuralElements,Definitions);
        }

        /// <summary>
        /// Decomposes a Structural Model into its geometry and structural settings
        /// </summary>
        /// <param name="structuralModel">Structural Model to decompose </param>
        /// <returns> Frames, Shells,Joints, Load Patterns, Load Cases </returns>
        [MultiReturn("Frames","Shells","Joints", "Load Patterns", "Load Cases", "Groups")]
        public static Dictionary<string, object> Decompose(StructuralModel structuralModel)
        {
            List<Element> Frms = new List<Element>();
            List<Element> Shells = new List<Element>();
            List<Element> Joints = new List<Element>();

            foreach (var el in structuralModel.StructuralElements)
            {
                if (el.Type == Structure.Type.Frame)
                {
                    Frms.Add(el);
                }
                else if (el.Type == Structure.Type.Shell)
                {
                    Shells.Add(el);
                }
                else if (el.Type == Structure.Type.Joint)
                {
                    Joints.Add(el);
                }
            }

            List<Definition> LoadPatterns = new List<Definition>();
            List<Definition> LoadCases= new List<Definition>();
            List<Group> Groups = new List<Group>();

            foreach (var def in structuralModel.ModelDefinitions)
            {
                if (def.Type == Definitions.Type.LoadCase)
                {
                    LoadCases.Add(def);
                }
                else if (def.Type == Definitions.Type.LoadPattern)
                {
                    LoadPatterns.Add(def);
                }
                else if (def.Type == Definitions.Type.Group)
                {
                    Groups.Add(def as Group);
                }
            }

            // Return outputs
            return new Dictionary<string, object>
            {
                {"Frames", Frms},
                {"Shells", Shells},
                {"Joints", Joints},
                {"Load Patterns", LoadPatterns},
                {"Load Cases", LoadCases},
                {"Group", Groups}
               
            };
        }

        internal StructuralModel() { }

        private StructuralModel(List<Element> Elements, List<Definition> Definitions )
        {
            StructuralElements = Elements;
            ModelDefinitions = Definitions;

        }
    }
}
