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
        /// No Need for Revit model !!! 
        /// </summary>
        [Test, TestModel(@".\TestModel.rvt")]
        public void DefineStructure()
        { 
            // Launch SAP2000v16 and Open 
            SapObject mySapObject = null;
            cSapModel mySapModel = null;
            SAPConnection.Initialize.InitializeSapModel(ref mySapObject, ref mySapModel, "kip_ft_F");

            //Open and Run the sample file
            OpenAndRunDynamoDefinition(@".\Sample_1a_SpaceStructure.dyn");

            // Test Logic is here --->

            // Check dropdown Material Types 
            var material = (string)GetPreviewValue("89e9e6aa-5ea9-4641-8bee-743b2173ada1");
            if (material != "A992Fy50") Assert.Fail("The node called '" + "Materials Dropdown" + "' returns wrong result !");

            // Check dropdown Section Catalog Types
            var sectionCatalog = (string)GetPreviewValue("579cbea6-5b14-4023-bfef-44fde2eff3ee");
            if (sectionCatalog != "AISC14") Assert.Fail("The node called '" + "Section Catalog Dropdown" + "' returns wrong result !");

            // Check Section Name read from SAP
            var sectionName = (string)GetPreviewValue("ba2e32bc-89b3-4566-9555-de98e9e98c2b");
            if (sectionName != "HSS5.563X.375") Assert.Fail("The node called '" + "Section Name" + "' returns wrong result !");

            // Check dropdown Units

            // Check Node Frame.FromLine
            if (IsNodeInErrorOrWarningState("73b79339-e8d9-4841-ac63-12d45e184b2f"))
            {
                Assert.Fail("The node called '" + "Frame.FromLine" + "' failed or threw a warning.");
            }

            // Check Node Bake to SAP
            if (IsNodeInErrorOrWarningState("d86c67da-ff87-4c04-8976-c7a00a640518"))
            {
                Assert.Fail("The node called '" + "Bake.toSAP" + "' failed or threw a warning.");
            }

            // Set SAP instances to null;
            mySapObject.ApplicationExit(false);
            mySapObject = null;
            mySapModel = null;

            //if we got here, nothing failed.
            Assert.Pass();
        }
    }
}
