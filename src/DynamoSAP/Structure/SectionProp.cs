using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//DYNAMO
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;

//SAP
using SAP2000v16;
using SAPConnection;

namespace DynamoSAP.Structure
{
    public class SectionProp
    {
        //FIELDS

        // Name of the Section
        internal string SectName { get; set; }
        // Section Catalog (file name)
        internal string SectCatalog { get; set; }
        // Material Property
        internal string MatProp { get; set; }

        // Returns the Section Names of a selected catalog
        public static List<string> Sections (string catalog)
        {
            List<string> sectionsnames = new List<string>();
            cSapModel mySapModel = null;
            string units = string.Empty;
            // Open & instantiate SAP file
            string sc = catalog + ".PRO";
            Initialize.GrabOpenSAP(ref mySapModel, ref units);
            if (mySapModel == null)
            {
                throw new Exception("Make sure a SAP Model is open !");
            }
            string[] Names = null;
            StructureMapper.GetSectionsfromCatalog(ref mySapModel, sc, ref Names);
            sectionsnames = Names.ToList();
            return sectionsnames;
        }

        public static SectionProp Define(string Name = "W12X14", string Material = "A992Fy50", string SectionCatalog = "AISC14")
        {
            return new SectionProp(Name, Material, SectionCatalog);
        }

        [MultiReturn("Name", "Material", "Catalog")]
        public static Dictionary<string,string> Decompose(SectionProp SectProp)
        {
            // Return outputs
            return new Dictionary<string, string>
            {
                {"Name", SectProp.SectName},
                {"Material", SectProp.MatProp},
                {"Catalog", SectProp.SectCatalog}
            };

        }

        // PRIVATE CONSTRUCTORS
        internal SectionProp() { }
        internal SectionProp(string _name, string _seccatalog, string _matprop)
        {
            SectName = _name;
            SectCatalog = _seccatalog;
            MatProp = _matprop;
        }


    }
}
