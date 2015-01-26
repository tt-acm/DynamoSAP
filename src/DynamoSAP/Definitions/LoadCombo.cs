/// Developed by Thornton Tomasetti's CORE Studio for Autodesk
/// http://core.thorntontomasetti.com
/// CORE Developers: Elcin Ertugrul and Ana Garcia Puyol

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Autodesk.DesignScript.Runtime;


namespace DynamoSAP.Definitions
{
    public class LoadCombo: Definition
    {
        //FIELDS
        internal string name { get; set; }
        internal string type { get; set; }
        internal List<Definition> loadDefinitions = new List<Definition>();
        internal List<double> sFs = new List<double>();


        //CREATE NODE
        /// <summary>
        /// Set a Load Combination
        /// </summary>
        /// <param name="Name">Name of the load combination</param>
        /// <param name="LoadDefinitions">Load Cases and Load Patterns to combine</param>
        /// <param name="ComboType">Combination Type. Use dropdown</param>
        /// <param name="ScaleFactors">Scale factors to apply to the load cases and patterns</param>
        /// <returns></returns>
        public static LoadCombo SetLoadCombo(string Name, string ComboType, List<Definition> LoadDefinitions, List<double> ScaleFactors)
        {
            if (LoadDefinitions.Count != ScaleFactors.Count)
            {
                throw new Exception("Make sure the number of Scale factors is the same as the number of Load patterns");
            }
            return new LoadCombo(Name, ComboType, LoadDefinitions, ScaleFactors);
        }

        /// <summary>
        /// Decompose a Load Combination
        /// </summary>
        /// <param name="LoadCombo">Load Combination to decompose</param>
        /// <returns></returns>
        [MultiReturn("Name", "Type", "Load Definitions", "Scale Factors")]
        public static Dictionary<string, object> Decompose(LoadCombo LoadCombo)
        {
            return new Dictionary<string, object>
            {
                {"Name", LoadCombo.name},
                {"Type",LoadCombo.type},
                {"Load Definitions", LoadCombo.loadDefinitions},
                {"SFs", LoadCombo.sFs}
            };
        }

        internal LoadCombo()
        {
            this.Type = Definitions.Type.LoadCombo;
        }

        internal LoadCombo(string Name, string ComboType, List<Definition> LoadDefinitions, List<double> SFs)
        {
            name = Name;
            type = ComboType;
            loadDefinitions = LoadDefinitions;
            sFs = SFs;
            this.Type = Definitions.Type.LoadCombo;
        }


    }
}
