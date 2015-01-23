using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Autodesk.DesignScript.Runtime;

namespace DynamoSAP.Definitions
{
    public class LoadCase : Definition
    {
        //FIELDS
        internal string name { get; set; }
        internal string type { get; set; }
        internal List<LoadPattern> loadPatterns = new List<LoadPattern>();
        internal List<double> sFs = new List<double>();

        //CREATE NODE
        /// <summary>
        /// Set a Load Case
        /// </summary>
        /// <param name="Name">Name of the Load Case</param>
        /// <param name="LoadPatterns">Load Patterns</param>
        /// <param name="ScaleFactors">Scale factor of each load assigned to the load case</param>
        /// <param name="Type"> Type of Load Case. Use the  Load Case Type Dropdown</param>
        /// <returns>New Load Case</returns>
        public static LoadCase SetLoadCase(string Name, List<LoadPattern> LoadPatterns, List<double> ScaleFactors, string Type)
        {
            // Check if the number of Patterms are equal to SF
            if (LoadPatterns.Count() != ScaleFactors.Count())
            {
                throw new Exception("Make sure the number of Scale factors is the same as the number of Load patterns");
            }

            return new LoadCase(Name, LoadPatterns, ScaleFactors, Type);
        }

        /// <summary>
        /// Decompose a Load Case
        /// </summary>
        /// <param name="LoadCase">Load Case to decompose</param>
        /// <returns>Name, Type, Load Patterns and Scale Factors of the Load Case</returns>
        [MultiReturn("Name", "Type", "Load Patterns", "Scale Factors")]
        public static Dictionary<string, object> Decompose(LoadCase LoadCase)
        {
            // Return outputs
            return new Dictionary<string, object>
            {
                {"Name", LoadCase.name},
                {"Type", LoadCase.type},
                {"Load Patterns", LoadCase.loadPatterns},
                {"SFs", LoadCase.sFs},
            };
        }

        internal LoadCase()
        {
            this.Type = Definitions.Type.LoadCase;
        }
        //PRIVATE CONSTRUCTOR

        private LoadCase(string Name, List<LoadPattern> LoadPatterns, List<double> SFs, string LoadCaseType = "CASE_LINEAR_STATIC") // Type has default value
        {
            name = Name;
            type = LoadCaseType;
            loadPatterns = LoadPatterns;
            sFs = SFs;
            this.Type = Definitions.Type.LoadCase;
        }

    }
}
