using System.IO;
using System.Reflection;
using NUnit.Framework;
using RevitTestServices;
using RTF.Framework;
using Autodesk.Revit.DB;
using RevitServices.Persistence;
using Autodesk.Revit.DB.Analysis;
using System.Collections.Generic;

using DynamoSAP;
using SAP2000v20;
using System;

namespace DynamoSAPTests
{
    [TestFixture]
    public class DynamoSAP_SystemTesting : RevitSystemTestBase
    {
        [SetUp]
        public void Setup()
        {
            // Set the working directory. This will allow you to use the OpenAndRunDynamoDefinition method,
            // specifying a relative path to the .dyn file you want to test.

            var asmDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            workingDirectory = Path.GetFullPath(Path.Combine(asmDirectory,
                @"..\..\..\packages\DynamoSAP\extra"));
        }

        /// <summary>
        /// Launch simple SAP file create space structure
        /// </summary>
        [Test, TestModel(@".\TestModel.rvt")]
        public void SpaceStructure()
        { 
            // Launch SAP2000v20 and Open a blank model
            SapObject mySapObject = null;
            cSapModel mySapModel = null;
            SAPConnection.Initialize.InitializeSapModel(ref mySapObject, ref mySapModel, "kip_ft_F");

            //Open and Run the sample file
            OpenAndRunDynamoDefinition(@".\Sample_1a_SpaceStructure.dyn");

            // Test Logic is here --->
            string failreport = string.Empty;

            // Check dropdown Material Types 
            var material = (string)GetPreviewValue("89e9e6aa-5ea9-4641-8bee-743b2173ada1");
            if (material != "A992Fy50") failreport = string.Format(failreport + "{0}{1}", Environment.NewLine, "The node called '" + "Materials Dropdown" + "' returns wrong value !");
            //Assert.Fail("The node called '" + "Materials Dropdown" + "' returns wrong value !");

            // Check dropdown Section Catalog Types
            var sectionCatalog = (string)GetPreviewValue("579cbea6-5b14-4023-bfef-44fde2eff3ee");
            if (sectionCatalog != "AISC14") failreport = string.Format(failreport + "{0}{1}", Environment.NewLine, "The node called '" + "Section Catalog Dropdown" + "' returns wrong value !");
                //Assert.Fail("The node called '" + "Section Catalog Dropdown" + "' returns wrong result !");

            // Check Section Name read from SAP
            var sectionName = (string)GetPreviewValue("ba2e32bc-89b3-4566-9555-de98e9e98c2b");
            if (sectionName != "HSS5.563X.375") failreport = string.Format(failreport + "{0}{1}", Environment.NewLine, "The node called '" + "Section Name" + "' returns wrong value !");
            //Assert.Fail("The node called '" + "Section Name" + "' returns wrong value !");

            // Check dropdown Units
            var units = (string)GetPreviewValue("aa80e305-bb4a-418b-87f4-022dd0268680");
            if (units == "kip_ft_F") failreport = string.Format(failreport + "{0}{1}", Environment.NewLine, "The node called '" + "Units" + "' returns wrong value !");
            //Assert.Fail("The node called '" + "Units" + "' returns wrong value !");


            // Check Node Frame.FromLine
            if (IsNodeInErrorOrWarningState("73b79339-e8d9-4841-ac63-12d45e184b2f"))
            {
                failreport = string.Format(failreport + "{0}{1}", Environment.NewLine, "The node called '" + "Frame.FromLine" + "' failed or threw a warning.");
                //Assert.Fail("The node called '" + "Frame.FromLine" + "' failed or threw a warning.");
            }

            // Check Node Bake to SAP
            if (IsNodeInErrorOrWarningState("d86c67da-ff87-4c04-8976-c7a00a640518"))
            {
                failreport = string.Format(failreport + "{0}{1}", Environment.NewLine, "The node called '" + "Bake.toSAP" + "' failed or threw a warning.");
                //Assert.Fail("The node called '" + "Bake.toSAP" + "' failed or threw a warning.");
            }

            // Set SAP instances to null;
            mySapObject.ApplicationExit(false);
            mySapObject = null;
            mySapModel = null;

            if (!string.IsNullOrEmpty(failreport))
            {
                Assert.Fail(failreport);
            }

            //if we got here, nothing failed.
            Assert.Pass();
        }


        /// <summary>
        /// Launch simple SAP file create space structure w/Load
        /// </summary>
        [Test, TestModel(@".\TestModel.rvt")]
        public void SpaceStructurewLoads()
        {
            // Launch SAP2000v20 and Open a blank model
            SapObject mySapObject = null;
            cSapModel mySapModel = null;
            SAPConnection.Initialize.InitializeSapModel(ref mySapObject, ref mySapModel, "kip_ft_F");

            //Open and Run the sample file
            OpenAndRunDynamoDefinition(@".\Sample_1b_SpaceStructure_withLoads.dyn");

            // Test Logic is here --->
            string failreport = string.Empty;

            // Check dropdown Load Pattern Types 
            var loadPatternType = (string)GetPreviewValue("c7adc7d3-fff5-49bc-b5ee-586546d3f1ba");
            if (loadPatternType != "LTYPE_DEAD") failreport = string.Format(failreport + "{0}{1}", Environment.NewLine, "The node called '" + "LoadPatternTypes Dropdown" + "' returns wrong value !");

            // Check Node SetLoadPattern
            if (IsNodeInErrorOrWarningState("89e67ebd-f8a6-4bec-a92e-1dfee2046852"))
            {
                failreport = string.Format(failreport + "{0}{1}", Environment.NewLine, "The node called '" + "LoadPattern.SetLoadPattern" + "' failed or threw a warning.");
            }

            // Check dropdown Load Type 
            var loadType = (string)GetPreviewValue("7a68135c-0101-482d-a0a9-d1dd96cb00cd");
            if (loadType != "Force") failreport = string.Format(failreport + "{0}{1}", Environment.NewLine, "The node called '" + "Load Type Dropdown" + "' returns wrong value !");

            // Check dropdown Load Direction
            var loadDirection = (Int64)GetPreviewValue("4abab34c-9147-4227-8c87-b6950b2fdcae");
            if (loadDirection != 6) failreport = string.Format(failreport + "{0}{1}", Environment.NewLine, "The node called '" + "Load Direction Dropdown" + "' returns wrong value !");

            // Check Node Load.DistributedLoad
            if (IsNodeInErrorOrWarningState("c9d48de5-889c-4bf1-bfe8-462b8b0bc63d"))
            {
                failreport = string.Format(failreport + "{0}{1}", Environment.NewLine, "The node called '" + "Load.DistributedLoad" + "' failed or threw a warning.");
            }

            // Check Node Frame.SetLoad
            if (IsNodeInErrorOrWarningState("fc60d698-d938-4513-b808-0619c35b25b2"))
            {
                failreport = string.Format(failreport + "{0}{1}", Environment.NewLine, "The node called '" + "Frame.SetLoad" + "' failed or threw a warning.");
            }

            // Check Node StructuralModel.Collector
            if (IsNodeInErrorOrWarningState("3bfbb3b1-47e0-4aa9-8920-e589ec0ffbc9"))
            {
                failreport = string.Format(failreport + "{0}{1}", Environment.NewLine, "The node called '" + "StructuralModel.Collector" + "' failed or threw a warning.");
            }

            // Check Node Frame.DisplayLoads
            if (IsNodeInErrorOrWarningState("abee3b8e-52fe-44a4-9221-75b1d1a0bf5a"))
            {
                failreport = string.Format(failreport + "{0}{1}", Environment.NewLine, "The node called '" + "Frame.DisplayLoads" + "' failed or threw a warning.");
            }

            // Check Node StructuralModel.Decompose
            if (IsNodeInErrorOrWarningState("e449b27f-1209-4dc1-a94a-dfef78e20a00"))
            {
                failreport = string.Format(failreport + "{0}{1}", Environment.NewLine, "The node called '" + "StructuralModel.Decompose" + "' failed or threw a warning.");
            }

            // Set SAP instances to null;
            mySapObject.ApplicationExit(false);
            mySapObject = null;
            mySapModel = null;

            if (!string.IsNullOrEmpty(failreport))
            {
                Assert.Fail(failreport);
            }

            //if we got here, nothing failed.
            Assert.Pass();

        }


        /// <summary>
        /// Launch simple SAP file create shell structure
        /// </summary>
        [Test, TestModel(@".\TestModel.rvt")]
        public void ShellStructure()
        {
            // Launch SAP2000v20 and Open a blank model
            SapObject mySapObject = null;
            cSapModel mySapModel = null;
            SAPConnection.Initialize.InitializeSapModel(ref mySapObject, ref mySapModel, "kip_ft_F");

            //Open and Run the sample file
            OpenAndRunDynamoDefinition(@".\Sample_1c_ShellStructure.dyn");

            // Test Logic is here --->
            string failreport = string.Empty;

            // Check dropdown Shell Types 
            var ShellType = (Int64)GetPreviewValue("e0c739ee-8e95-4531-95b8-d5bb71501b6c");
            if (ShellType != 1) failreport = string.Format(failreport + "{0}{1}", Environment.NewLine, "The node called '" + "Shell Types Dropdown" + "' returns wrong value !");

            // Check Node ShellProp.Define
            if (IsNodeInErrorOrWarningState("c0f21c61-375a-42b8-a1f4-d94a43f1272b"))
            {
                failreport = string.Format(failreport + "{0}{1}", Environment.NewLine, "The node called '" + "ShellProp.Define" + "' failed or threw a warning.");
            }

            // Check Node Shell.FromSurface
            if (IsNodeInErrorOrWarningState("0c217e34-2531-4450-a066-f839b64b1a1e"))
            {
                failreport = string.Format(failreport + "{0}{1}", Environment.NewLine, "The node called '" + "Shell.FromSurface" + "' failed or threw a warning.");
            }

            // Check Node StructuralModel.Collector
            if (IsNodeInErrorOrWarningState("a6bc0f86-a159-4f79-934d-238323b65de8"))
            {
                failreport = string.Format(failreport + "{0}{1}", Environment.NewLine, "The node called '" + "StructuralModel.Collector" + "' failed or threw a warning.");
            }

            // Check Node Bake to SAP
            if (IsNodeInErrorOrWarningState("d86c67da-ff87-4c04-8976-c7a00a640518"))
            {
                failreport = string.Format(failreport + "{0}{1}", Environment.NewLine, "The node called '" + "Bake.toSAP" + "' failed or threw a warning.");
            }

            // Set SAP instances to null;
            mySapObject.ApplicationExit(false);
            mySapObject = null;
            mySapModel = null;

            if (!string.IsNullOrEmpty(failreport))
            {
                Assert.Fail(failreport);
            }

            //if we got here, nothing failed.
            Assert.Pass();
        }


        /// <summary>
        /// Open sample  SAP Read frame structure
        /// </summary>
        [Test, TestModel(@".\TestModel.rvt")]
        public void ReadStructure_Frame()
        {
            // Launch SAP2000v20 and Open a blank model

            string filePath = @"C:\Users\eertugrul\Documents\GitHub\DynamoSAP\packages\DynamoSAP\extra\2a_Dome.sdb";
            //Create SAP2000 Object
            SapObject mySapObject = new SAP2000v20.SapObject();
            //Start Application
            mySapObject.ApplicationStart();
            //Create SapModel object
            cSapModel mySapModel = mySapObject.SapModel;
            mySapModel.InitializeNewModel();
            mySapModel.File.OpenFile(filePath);


            //Open and Run the sample file
            OpenAndRunDynamoDefinition(@".\Sample_2a_Read_Dome+DecomposeSapModel.dyn");

            // Test Logic is here --->
            string failreport = string.Empty;

            // Check Node Read SAp Model
            if (IsNodeInErrorOrWarningState("966718c2-c618-46b8-b631-c43a54c669ff"))
            {
                failreport = string.Format(failreport + "{0}{1}", Environment.NewLine, "The node called '" + "Read.SAPModel" + "' failed or threw a warning.");
            }

            // Check Node Structural Model Decompose
            if (IsNodeInErrorOrWarningState("36b32253-13f6-47ab-9bef-69b45c688c5b"))
            {
                failreport = string.Format(failreport + "{0}{1}", Environment.NewLine, "The node called '" + "StructuralModel.Decompose" + "' failed or threw a warning.");
            }

            // Check Node Frame Decompose
            if (IsNodeInErrorOrWarningState("b5e32685-6c44-443b-bc6c-c91a051cc541"))
            {
                failreport = string.Format(failreport + "{0}{1}", Environment.NewLine, "The node called '" + "Frame.Decompose" + "' failed or threw a warning.");
            }

            // Check Node SectionProp.Decompose
            if (IsNodeInErrorOrWarningState("6bdba7f9-4de1-475e-8868-815d93b99bf6"))
            {
                failreport = string.Format(failreport + "{0}{1}", Environment.NewLine, "The node called '" + "SectionProp.Decompose" + "' failed or threw a warning.");
            }

            // Set SAP instances to null;
            mySapObject.ApplicationExit(false);
            mySapObject = null;
            mySapModel = null;

            if (!string.IsNullOrEmpty(failreport))
            {
                Assert.Fail(failreport);
            }

            //if we got here, nothing failed.
            Assert.Pass();

        }

        /// <summary>
        /// Open sample  SAP Read Shell structure
        /// </summary>
        [Test, TestModel(@".\TestModel.rvt")]
        public void ReadStructure_Shell()
        {
            // Launch SAP2000v20 and Open a blank model
            string filePath = @"C:\Users\eertugrul\Documents\GitHub\DynamoSAP\packages\DynamoSAP\extra\2b_SimpleShellStructure.sdb";
            //Create SAP2000 Object
            SapObject mySapObject = new SAP2000v20.SapObject();
            //Start Application
            mySapObject.ApplicationStart();
            //Create SapModel object
            cSapModel mySapModel = mySapObject.SapModel;
            mySapModel.InitializeNewModel();
            mySapModel.File.OpenFile(filePath);

            //Open and Run the sample file
            OpenAndRunDynamoDefinition(@".\Sample_2b_Read_Shell+DecomposeSapModel.dyn");

            // Test Logic is here --->
            string failreport = string.Empty;

            // Check Node Read SAp Model
            if (IsNodeInErrorOrWarningState("966718c2-c618-46b8-b631-c43a54c669ff"))
            {
                failreport = string.Format(failreport + "{0}{1}", Environment.NewLine, "The node called '" + "Read.SAPModel" + "' failed or threw a warning.");
            }

            // Check Node Structural Model Decompose
            if (IsNodeInErrorOrWarningState("36b32253-13f6-47ab-9bef-69b45c688c5b"))
            {
                failreport = string.Format(failreport + "{0}{1}", Environment.NewLine, "The node called '" + "StructuralModel.Decompose" + "' failed or threw a warning.");
            }

            // Check Node Shell Decompose
            if (IsNodeInErrorOrWarningState("c6ec73e3-9413-4464-b827-a4195d68c727"))
            {
                failreport = string.Format(failreport + "{0}{1}", Environment.NewLine, "The node called '" + "Frame.Decompose" + "' failed or threw a warning.");
            }

            // Check Node ShellProp.Decompose
            if (IsNodeInErrorOrWarningState("398af6ae-6adc-4f34-83f0-ef85184f37bb"))
            {
                failreport = string.Format(failreport + "{0}{1}", Environment.NewLine, "The node called '" + "ShellProp.Decompose" + "' failed or threw a warning.");
            }

            // Set SAP instances to null;
            mySapObject.ApplicationExit(false);
            mySapObject = null;
            mySapModel = null;

            if (!string.IsNullOrEmpty(failreport))
            {
                Assert.Fail(failreport);
            }

            //if we got here, nothing failed.
            Assert.Pass();

        }


        /// <summary>
        /// Open sample  SAP , Run Analysis and Read Results
        /// </summary>
        [Test, TestModel(@".\TestModel.rvt")]
        public void RunAnalysis()
        {
            // Launch SAP2000v20 and Open a blank model
            string filePath = @"C:\Users\eertugrul\Documents\GitHub\DynamoSAP\packages\DynamoSAP\extra\3a_RunAnalysis.sdb";
            //Create SAP2000 Object
            SapObject mySapObject = new SAP2000v20.SapObject();
            //Start Application
            mySapObject.ApplicationStart();
            //Create SapModel object
            cSapModel mySapModel = mySapObject.SapModel;
            mySapModel.InitializeNewModel();
            mySapModel.File.OpenFile(filePath);

            //Open and Run the sample file
            OpenAndRunDynamoDefinition(@".\Sample_3a_RunAnalysis_ReadResults.dyn");

            // Test Logic is here --->
            string failreport = string.Empty;

            // Check Node Run Analysis
            if (IsNodeInErrorOrWarningState("b24e8e64-0cc1-43a1-a98c-3bfcf4f9099e"))
            {
                failreport = string.Format(failreport + "{0}{1}", Environment.NewLine, "The node called '" + "Analysis.Run" + "' failed or threw a warning.");
            }

            // Check Node StructuralModel.Decompose
            if (IsNodeInErrorOrWarningState("79d1b8d0-0bb8-498b-b40b-cc5de1b168b7"))
            {
                failreport = string.Format(failreport + "{0}{1}", Environment.NewLine, "The node called '" + "StructuralModel.Decompose" + "' failed or threw a warning.");
            }            
            
            // Check Node Frame.DisplayLoads
            if (IsNodeInErrorOrWarningState("6ba73349-8d06-4f1f-ba4f-d3d23f59bba4"))
            {
                failreport = string.Format(failreport + "{0}{1}", Environment.NewLine, "The node called '" + "Frame.DisplayLoads" + "' failed or threw a warning.");
            }

            // Check Node Analysis.GetResults
            if (IsNodeInErrorOrWarningState("39d253c0-b3fc-4e82-9c74-5e645578bab3"))
            {
                failreport = string.Format(failreport + "{0}{1}", Environment.NewLine, "The node called '" + "Analysis.GetResults" + "' failed or threw a warning.");
            }

            // Check dropdown Force Type 
            var forceType = (string)GetPreviewValue("a1a18410-1bf5-4492-8bad-40faee974fe9");
            if (forceType != "Axial") failreport = string.Format(failreport + "{0}{1}", Environment.NewLine, "The node called '" + "Force Type Dropdown" + "' returns wrong value !");

            // Check Node Analysis.Decomposeresults
            if (IsNodeInErrorOrWarningState("80e7a42a-1418-471f-b368-551fd3752e8d"))
            {
                failreport = string.Format(failreport + "{0}{1}", Environment.NewLine, "The node called '" + "Analysis.DecomposeResults" + "' failed or threw a warning.");
            }

            // Check Node Analysis.VisualizeResults
            if (IsNodeInErrorOrWarningState("982cd2d8-aa2a-473b-9d6b-ed6f11ed29c4"))
            {
                failreport = string.Format(failreport + "{0}{1}", Environment.NewLine, "The node called '" + "Analysis.VisualizeResults" + "' failed or threw a warning.");
            }

            // Set SAP instances to null;
            mySapObject.ApplicationExit(false);
            mySapObject = null;
            mySapModel = null;

            if (!string.IsNullOrEmpty(failreport))
            {
                Assert.Fail(failreport);
            }

            //if we got here, nothing failed.
            Assert.Pass();
        }


        /// <summary>
        /// Open blank SAP Model, Edit 
        /// </summary>
        [Test, TestModel(@".\TestModel.rvt")]
        public void Edit_AddGroup()
        {
            // Launch SAP2000v20 and Open a blank model

            string filePath = @"C:\Users\eertugrul\Documents\GitHub\DynamoSAP\packages\DynamoSAP\extra\2a_Dome.sdb";
            //Create SAP2000 Object
            SapObject mySapObject = new SAP2000v20.SapObject();
            //Start Application
            mySapObject.ApplicationStart();
            //Create SapModel object
            cSapModel mySapModel = mySapObject.SapModel;
            mySapModel.InitializeNewModel();
            mySapModel.File.OpenFile(filePath);


            //Open and Run the sample file
            OpenAndRunDynamoDefinition(@".\Sample_4a_Read_Edit_AddGroup.dyn");

            // Test Logic is here --->
            //check for errors and assert accordingly
            string failreport = CompileErrorsIntoString();

            // Set SAP instances to null;
            mySapObject.ApplicationExit(false);
            mySapObject = null;
            mySapModel = null;

            if (string.IsNullOrEmpty(failreport))
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail(failreport);
            }

        }

        /// <summary>
        /// Open blank SAP Model, Edit 
        /// </summary>
        [Test, TestModel(@".\TestModel.rvt")]
        public void Edit_AddLoadCase()
        {
            // Launch SAP2000v20 and Open a blank model

            string filePath = @"C:\Users\eertugrul\Documents\GitHub\DynamoSAP\packages\DynamoSAP\extra\2a_Dome.sdb";
            //Create SAP2000 Object
            SapObject mySapObject = new SAP2000v20.SapObject();
            //Start Application
            mySapObject.ApplicationStart();
            //Create SapModel object
            cSapModel mySapModel = mySapObject.SapModel;
            mySapModel.InitializeNewModel();
            mySapModel.File.OpenFile(filePath);


            //Open and Run the sample file
            OpenAndRunDynamoDefinition(@".\Sample_4b_Read_Edit_AddLoadCase.dyn");

            // Test Logic is here --->
            //check for errors and assert accordingly
            string failreport = CompileErrorsIntoString();

            // Set SAP instances to null;
            mySapObject.ApplicationExit(false);
            mySapObject = null;
            mySapModel = null;

            if (string.IsNullOrEmpty(failreport))
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail(failreport);
            }

        }

        /// <summary>
        /// A utility function to loop over a sample file and list any nodes in error or warning state.
        /// </summary>
        /// <returns></returns>
        private string CompileErrorsIntoString()
        {
            //a string to return
            string errors = null;

            //loop over the active collection of nodes.
            foreach (var i in AllNodes)
            {
                if (IsNodeInErrorOrWarningState(i.GUID.ToString()))
                {
                    errors += "The node called '" + i.NickName + "' failed or threw a warning." + System.Environment.NewLine;
                }
            }

            //return the errors string
            return errors;
        }

    }
}
