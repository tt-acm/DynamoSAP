using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.DesignScript.Runtime;

namespace DynamoSAP.Structure
{
    public class LoadPattern
    {
        //Load Pattern Name
        internal string name { get; set; }
        // Type of Load Pattern
        internal string type { get; set; }
        // Multiplier
        internal double multiplier { get; set; }


        //DYNAMO NODE
        /// <summary>
        /// Create a Load Pattern
        /// </summary>
        /// <param name="Name">The name for the new load pattern</param>
        /// <param name="LType">Load Pattern Type. Use the Load Pattern Type Dropdown</param>
        /// <param name="Multiplier">The self weight multiplier for the new load pattern.</param>
        /// <returns>Load Pattern</returns>
        //public static LoadPattern SetLoadPattern(string Name, eLoadPatternType LoadPatternType, double Multiplier)
        public static LoadPattern SetLoadPattern(string Name, string LType, double Multiplier = 1)
        {
            return new LoadPattern(Name, LType, Multiplier);
        }


        // Decompose

       /// <summary>
       /// Decompose a Load Pattern
       /// </summary>
       /// <param name="LoadPattern">Load Pattern to Decompose</param>
       /// <returns>Name, Type and Multiplier of the Load Pattern</returns>
        [MultiReturn("Name", "Type", "Multiplier")]
        public static Dictionary<string, object> Decompose(LoadPattern LoadPattern)
        {
            // Return outputs
            return new Dictionary<string, object>
            {
                {"Name", LoadPattern.name},
                {"Type", LoadPattern.type},
                {"Multiplier", LoadPattern.multiplier}
            };
        }

        //PRIVATE CONSTRUCTOR
        internal LoadPattern(string Name, string LoadPatternType, double Multiplier)
        {
            name = Name;
            type = LoadPatternType;
            multiplier = Multiplier;
        }
    }
}
