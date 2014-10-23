using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAP2000v16;

namespace DynamoSAP.Analysis
{
    public class LoadPattern
    {
        //Load Pattern Name
        internal string Name { get; set; }
        
        // Type of Load Pattern - THIS THROWS AN EXCEPTION AS IT IS USING THE SAP LIBRARY. THIS IS NOT OPTIONAL!
        internal eLoadPatternType LoadPatternType { get; set; }
        
        // Multiplier
        internal double Multiplier = 1.0;


        //QUERY NODES
        public string PatternName {get{return Name;}}
        //public string PatternType { get { return LoadPatternType.ToString(); } }
        public double LoadMultiplier { get { return Multiplier; } }
        
        //DYNAMO NODE
        /// <summary>
        /// This node creates a Load Pattern
        /// </summary>
        /// <param name="Name">The name for the new load pattern</param>
        /// <param name="LoadPatternType">This is one of the following items in the eLoadPatternType enumeration:
        /// LTYPE_DEAD = 1
        /// LTYPE_SUPERDEAD = 2
        /// LTYPE_LIVE = 3
        /// LTYPE_REDUCELIVE = 4
        /// LTYPE_QUAKE = 5
        /// LTYPE_WIND= 6
        /// LTYPE_SNOW = 7
        /// LTYPE_OTHER = 8
        /// LTYPE_MOVE = 9
        /// LTYPE_TEMPERATURE = 10
        /// LTYPE_ROOFLIVE = 11
        /// LTYPE_NOTIONAL = 12
        /// LTYPE_PATTERNLIVE = 13
        /// LTYPE_WAVE= 14
        /// LTYPE_BRAKING = 15
        /// LTYPE_CENTRIFUGAL = 16
        /// LTYPE_FRICTION = 17
        /// LTYPE_ICE = 18
        /// LTYPE_WINDONLIVELOAD = 19
        /// LTYPE_HORIZONTALEARTHPRESSURE = 20
        /// LTYPE_VERTICALEARTHPRESSURE = 21
        /// LTYPE_EARTHSURCHARGE = 22
        /// LTYPE_DOWNDRAG = 23
        /// LTYPE_VEHICLECOLLISION = 24
        /// LTYPE_VESSELCOLLISION = 25
        /// LTYPE_TEMPERATUREGRADIENT = 26
        /// LTYPE_SETTLEMENT = 27
        /// LTYPE_SHRINKAGE = 28
        /// LTYPE_CREEP = 29
        /// LTYPE_WATERLOADPRESSURE = 30
        /// LTYPE_LIVELOADSURCHARGE = 31
        /// LTYPE_LOCKEDINFORCES = 32
        /// LTYPE_PEDESTRIANLL = 33
        /// LTYPE_PRESTRESS = 34
        /// LTYPE_HYPERSTATIC = 35
        /// LTYPE_BOUYANCY = 36
        /// LTYPE_STREAMFLOW = 37
        /// LTYPE_IMPACT = 38
        /// LTYPE_CONSTRUCTION = 39</param>
        /// <param name="Multiplier">The self weight multiplier for the new load pattern.</param>
        /// <returns></returns>
        //public static LoadPattern SetLoadPattern(string Name, eLoadPatternType LoadPatternType, double Multiplier)
            public static LoadPattern SetLoadPattern(string Name, double Multiplier)
        {
            //return new LoadPattern(Name,eLoadPatternType LoadPatternType, Multiplier);
             return new LoadPattern(Name, Multiplier);
        }

        //PRIVATE CONSTRUCTOR
       // private LoadPattern(string name, eLoadPatternType loadPatternType, double multiplier)
             private LoadPattern(string name, double multiplier)
        {
            Name = name;
            //LoadPatternType = loadPatternType;
            Multiplier = multiplier;
        }
    }
}
