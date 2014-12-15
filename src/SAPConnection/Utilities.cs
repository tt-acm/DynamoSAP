using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// DYNAMO
using Autodesk.DesignScript.Runtime;

namespace SAPConnection
{
    [SupressImportIntoVM]
    public class Utilities
    {
        public static double UnitConversion(string toUnit, string fromUnit)
        {
            // input string mapper
            if (toUnit == "kgf_m_C" || toUnit == "kN_m_C" || toUnit == "N_m_C" || toUnit == "Ton_m_C" || toUnit.ToLower().Contains("meter")) toUnit = "m";
            else if (toUnit == "kgf_cm_C" || toUnit == "kN_cm_C" || toUnit == "N_cm_C" || toUnit == "Ton_cm_C" || toUnit.ToLower().Contains("cm") || toUnit.ToLower().Contains("centimeter")) toUnit = "cm";
            else if (toUnit == "kgf_mm_C" || toUnit == "kN_mm_C" || toUnit == "N_mm_C" || toUnit == "Ton_mm_C" || toUnit.ToLower().Contains("mm") || toUnit.ToLower().Contains("milimeter")) toUnit = "mm";
            else if (toUnit == "kip_ft_F" || toUnit == "lb_ft_F" || toUnit.ToLower().Contains("ft") || toUnit.ToLower().Contains("feet")) toUnit = "ft";
            else if (toUnit == "kip_in_F" || toUnit == "lb_in_F" || toUnit.ToLower().Contains("in") || toUnit.ToLower().Contains("inch")) toUnit = "in";

            toUnit = toUnit.ToLower();
            fromUnit = fromUnit.ToLower();
            double conversionFactor = 1.0;

            //if project units = meters
            if (fromUnit == "m")
            {
                if (toUnit == "m")
                { conversionFactor = 1; }
                else if (toUnit == "cm")
                { conversionFactor = 100; }
                else if (toUnit == "mm")
                { conversionFactor = 1000; }
                else if (toUnit == "ft")
                { conversionFactor = 1 / 0.3048; }
                else if (toUnit == "in")
                { conversionFactor = 1000 / 25.4; }
            }

            //else if project units = centimeters
            else if (fromUnit == "cm")
            {
                if (toUnit == "m")
                { conversionFactor = 0.01; }
                else if (toUnit == "cm")
                { conversionFactor = 1; }
                else if (toUnit == "mm")
                { conversionFactor = 10; }
                else if (toUnit == "ft")
                { conversionFactor = 1 / 30.48; }
                else if (toUnit == "in")
                { conversionFactor = 10 / 25.4; }
            }

            //else if project units = milimeters
            else if (fromUnit == "mm")
            {
                if (toUnit == "m")
                { conversionFactor = 0.001; }
                else if (toUnit == "cm")
                { conversionFactor = 0.1; }
                else if (toUnit == "mm")
                { conversionFactor = 1; }
                else if (toUnit == "ft")
                { conversionFactor = 1 / 304.8; }
                else if (toUnit == "in")
                { conversionFactor = 1 / 25.4; }
            }

            //else if project units = feet
            else if (fromUnit == "ft")
            {
                if (toUnit == "m")
                { conversionFactor = .3048; }
                else if (toUnit == "cm")
                { conversionFactor = 30.48; }
                else if (toUnit == "mm")
                { conversionFactor = 304.8; }
                else if (toUnit == "ft")
                { conversionFactor = 1; }
                else if (toUnit == "in")
                { conversionFactor = 12; }
            }

            //else if project units = inches
            else if (fromUnit == "in")
            {
                if (toUnit == "m")
                { conversionFactor = .0254; }
                else if (toUnit == "cm")
                { conversionFactor = 2.54; }
                else if (toUnit == "mm")
                { conversionFactor = 25.4; }
                else if (toUnit == "ft")
                { conversionFactor = 1 / 12.000; }
                else if (toUnit == "in")
                { conversionFactor = 1; }
            }

            return conversionFactor;
        
        }
    }
}
