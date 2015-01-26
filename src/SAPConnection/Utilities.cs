/// Developed by Thornton Tomasetti's CORE Studio for Autodesk
/// http://core.thorntontomasetti.com
/// CORE Developers: Elcin Ertugrul and Ana Garcia Puyol

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
            toUnit = L_UnitStringMapper(toUnit);
            fromUnit = L_UnitStringMapper(fromUnit);

            double conversionFactor = 1.0; // default value

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

        public static string L_UnitStringMapper(string Unit)
        {
            string outUnit = "m"; //default

            if (Unit == "kgf_m_C" || Unit == "kN_m_C" || Unit == "N_m_C" || Unit == "Ton_m_C" || Unit == "m" || Unit.ToLower().Contains("meter")) outUnit = "m";
            else if (Unit == "kgf_cm_C" || Unit == "kN_cm_C" || Unit == "N_cm_C" || Unit == "Ton_cm_C" || Unit.ToLower().Contains("cm") || Unit.ToLower().Contains("centimeter")) outUnit = "cm";
            else if (Unit == "kgf_mm_C" || Unit == "kN_mm_C" || Unit == "N_mm_C" || Unit == "Ton_mm_C" || Unit.ToLower().Contains("mm") || Unit.ToLower().Contains("milimeter")) outUnit = "mm";
            else if (Unit == "kip_ft_F" || Unit == "lb_ft_F" || Unit.ToLower().Contains("ft") || Unit.ToLower().Contains("feet")) outUnit = "ft";
            else if (Unit == "kip_in_F" || Unit == "lb_in_F" || Unit.ToLower().Contains("in") || Unit.ToLower().Contains("inch")) outUnit = "in";

            return outUnit;
        }
    }
}
