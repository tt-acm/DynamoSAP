using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamoSAP.Analysis
{
    public class LoadCase
    {
        //FIELDS
        private string name { get; set; }
        private string type { get; set; }
        private List<LoadPattern> loadPatterns = new List<LoadPattern>();
        private List<double> sFs = new List<double>();

        //QUERY NODES
        public string Name { get { return name; } }
        public string Type { get { return type; } }
        public List<LoadPattern> LoadPatterns { get { return loadPatterns; } }
        public List<double> SFs { get { return sFs; } }

        //CREATE NODE
        public static LoadCase SetLoadCase(string Name, List<LoadPattern> LoadPatterns, List<double> SFs, string Type)
        {
            // Check if the number of Patterms are equal to SF
            if (LoadPatterns.Count() != SFs.Count())
            {
                throw new Exception("Make sure number of Scae factors is the same with number of  Load patterns");
            }

                return new LoadCase(Name, LoadPatterns, SFs, Type);
        }

        //PRIVATE CONSTRUCTOR

        private LoadCase(string Name, List<LoadPattern> LoadPatterns, List<double> SFs, string Type = "CASE_LINEAR_STATIC") // Type has default value
        {
            name = Name;
            type = Type;
            loadPatterns = LoadPatterns;
            sFs = SFs;
        }

    }
}
