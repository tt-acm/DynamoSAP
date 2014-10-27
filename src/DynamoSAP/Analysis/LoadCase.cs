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
        private List<LoadPattern> loadPatterns = new List<LoadPattern>();
        private List<double> sFs = new List<double>();

        //QUERY NODES
        public string Name { get { return name; } }
        public List<LoadPattern> LoadPatterns { get { return loadPatterns; } }
        public List<double> SFs { get { return sFs; } }

        //CREATE NODE
        public static LoadCase SetLoadCase(string Name, List<LoadPattern> LoadPatterns, List<double> SFs)
        {
            // Check if the number of Patterms are equal to SF
            if (LoadPatterns.Count() != SFs.Count())
            {
                throw new Exception("Make sure number of Scae factors is the same with number of  Load patterns");
            }

                return new LoadCase(Name, LoadPatterns, SFs);
        }

        //PRIVATE CONSTRUCTOR

        private LoadCase(string Name, List<LoadPattern> LoadPatterns, List<double> SFs)
        {
            name = Name;
            loadPatterns = LoadPatterns;
            sFs = SFs;
        }

    }
}
