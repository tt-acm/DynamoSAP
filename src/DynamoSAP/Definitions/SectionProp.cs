/// Developed by Thornton Tomasetti's CORE Studio for Autodesk
/// http://core.thorntontomasetti.com
/// CORE Developers: Elcin Ertugrul and Ana Garcia Puyol

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//DYNAMO
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;

//SAP
using SAP2000v20;
using SAPConnection;

namespace DynamoSAP.Definitions
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

        /// <summary>
        /// Returns the Section Names of a selected catalog
        /// </summary>
        /// <param name="catalog">Catalog to read</param>
        /// <returns>Section Names</returns>
        public static List<string> Sections (string catalog)
        {
            List<string> sectionsnames = new List<string>();
            cSapModel mySapModel = null;
            string ModelUnits = string.Empty;
            // Open & instantiate SAP file
            string sc = catalog + ".PRO";
            Initialize.GrabOpenSAP(ref mySapModel, ref ModelUnits, "");
            if (mySapModel == null)
            {
                throw new Exception("Make sure a SAP Model is open!");
            }
            string[] Names = null;
            StructureMapper.GetSectionsfromCatalog(ref mySapModel, sc, ref Names);
            sectionsnames = Names.ToList();
            return sectionsnames;
        }
        /// <summary>
        /// Define a Section property
        /// </summary>
        /// <param name="Name">Name of the section property</param>
        /// <param name="Material">Material of the section property</param>
        /// <param name="SectionCatalog">Section Catalog</param>
        /// <returns>Section Property</returns>
        public static SectionProp Define(string Name = "W12X14", string Material = "A992Fy50", string SectionCatalog = "AISC14")
        {
            return new SectionProp(Name, Material, SectionCatalog);
        }

        /// <summary>
        /// Decompose a Section Property
        /// </summary>
        /// <param name="SectProp">Section Property to decompose</param>
        /// <returns>Name, Material, Catalog</returns>
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
        internal SectionProp(string _name, string _matprop , string _seccatalog)
        {
            SectName = _name;
            SectCatalog = _seccatalog + ".PRO";
            MatProp = _matprop;
        }


    }
}
