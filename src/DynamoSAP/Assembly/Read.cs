using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DynamoSAP.Structure;
using DynamoSAP;

//DYNAMO
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;

using SAPConnection;
using SAP2000v16;

namespace DynamoSAP.Assembly
{
    public class Read
    {
        //// DYNAMO NODES ////
        [MultiReturn("StructuralModel", "units")]
        public static Dictionary<string, object> SAPModel(string FilePath, bool read)
        {
            StructuralModel Model = new StructuralModel();
            Model.StructuralElements = new List<Element>();
            cSapModel mySapModel = null;
            string units = string.Empty;
            // Open & instantiate SAP file
            Initialize.OpenSAPModel(FilePath, ref mySapModel, ref units);

            // Populate the model's elemets

            //Get Frames          
            string[] FrmIds = null;
            StructureMapper.GetFrameIds(ref FrmIds, ref mySapModel);
            for (int i = 0; i < FrmIds.Length; i++)
            {
                Point s = null;
                Point e = null;
                string matProp = "A992Fy50"; // default value
                string secName = "W12X14"; // default value
                string secCatalog = "AISC14"; // default value
                string Just = "MiddleCenter"; // default value
                double Rot = 0; // default value

                StructureMapper.GetFrm(ref mySapModel, FrmIds[i], ref s, ref e, ref matProp, ref secName, ref Just, ref Rot, ref secCatalog);
                SectionProp secProp = new SectionProp(secName,  matProp , secCatalog);
                Frame d_frm = new Frame(s, e, secProp, Just, Rot);

                d_frm.Label = FrmIds[i];
                // get Guid
                string guid = string.Empty;
                StructureMapper.GetGUIDFrm(ref mySapModel, FrmIds[i], ref guid);
                d_frm.GUID = guid;
                Model.StructuralElements.Add(d_frm);
            }

            // Return outputs
            return new Dictionary<string, object>
            {
                {"StructuralModel", Model},
                {"units", units}
            };

        }

        [MultiReturn("StructuralModel","units")]
        public static Dictionary<string,object> SAPModel(bool read)
        {
            StructuralModel Model = new StructuralModel();
            
            cSapModel mySapModel = null;
            string units = string.Empty;

            // Open & instantiate SAP file
            Initialize.GrabOpenSAP(ref mySapModel, ref units);

            StructuralModelFromSapFile(ref mySapModel, ref Model);

            // Return outputs
            return new Dictionary<string, object>
            {
                {"StructuralModel", Model},
                {"units", units}
            };
            
        }

        private Read() { }

        internal static void StructuralModelFromSapFile(ref cSapModel SapModel, ref StructuralModel model)
        {
            model.StructuralElements = new List<Element>();
            if (SapModel != null)
            {
                // Populate the model's elements
                //Get Frames          
                string[] FrmIds = null;
                StructureMapper.GetFrameIds(ref FrmIds, ref SapModel);
                for (int i = 0; i < FrmIds.Length; i++)
                {
                    Point s = null;
                    Point e = null;
                    string matProp = "A992Fy50"; // default value
                    string secName = "W12X14"; // default value
                    string secCatalog = "AISC14"; // default value
                    string Just = "MiddleCenter"; // default value
                    double Rot = 0; // default value

                    StructureMapper.GetFrm(ref SapModel, FrmIds[i], ref s, ref e, ref matProp, ref secName, ref Just, ref Rot, ref secCatalog);
                    SectionProp secProp = new SectionProp(secName, matProp, secCatalog);
                    Frame d_frm = new Frame(s, e, secProp, Just, Rot);
                    d_frm.Label = FrmIds[i];
                    // get Guid
                    string guid = string.Empty;
                    StructureMapper.GetGUIDFrm(ref SapModel, FrmIds[i], ref guid);
                    d_frm.GUID = guid;
                    model.StructuralElements.Add(d_frm);
                }
            }
            else 
            {
                throw new Exception("Make sure SAP Model is open!");
            }
        }
    }
}
