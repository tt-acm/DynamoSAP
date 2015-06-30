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
using SAP2000v16;
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
        /// Test Model Revit model
        /// </summary>
        [Test, TestModel(@".\TestModel.rvt")]
        public void SpaceStructure()
        { 
            // Launch SAP2000v16 and Open a blank model
            SapObject mySapObject = null;
            cSapModel mySapModel = null;
            SAPConnection.Initialize.InitializeSapModel(ref mySapObject, ref mySapModel, "kip_ft_F");

            //Open and Run the sample file
            OpenAndRunDynamoDefinition(@".\Sample_1a_SpaceStructure.dyn");

            // Test Logic is here --->
            string failreport = string.Empty;

            // Check dropdown Material Types 
            var material = (string)GetPreviewValue("89e9e6aa-5ea9-4641-8bee-743b2173ada1");
            if (material != "A992Fy50") failreport = string.Format(failreport + "{0}{1}", Environment.NewLine, "The node called '" + "Materials Dropdown" + "' returns wrong result !");
                //Assert.Fail("The node called '" + "Materials Dropdown" + "' returns wrong result !");

            // Check dropdown Section Catalog Types
            var sectionCatalog = (string)GetPreviewValue("579cbea6-5b14-4023-bfef-44fde2eff3ee");
            if (sectionCatalog != "AISC14") failreport = string.Format(failreport + "{0}{1}", Environment.NewLine, "The node called '" + "Section Catalog Dropdown" + "' returns wrong result !");
                //Assert.Fail("The node called '" + "Section Catalog Dropdown" + "' returns wrong result !");

            // Check Section Name read from SAP
            var sectionName = (string)GetPreviewValue("ba2e32bc-89b3-4566-9555-de98e9e98c2b");
            if (sectionName != "HSS5.563X.375") failreport = string.Format(failreport + "{0}{1}", Environment.NewLine, "The node called '" + "Section Name" + "' returns wrong result !");
                //Assert.Fail("The node called '" + "Section Name" + "' returns wrong result !");

            // Check dropdown Units
            var units = (string)GetPreviewValue("aa80e305-bb4a-418b-87f4-022dd0268680");
            if (units == "kip_ft_F") failreport = string.Format(failreport + "{0}{1}", Environment.NewLine, "The node called '" + "Units" + "' returns wrong result !");
                //Assert.Fail("The node called '" + "Units" + "' returns wrong result !");


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
    }
}
