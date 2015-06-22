using System.IO;
using System.Reflection;
using NUnit.Framework;
using RevitTestServices;
using RTF.Framework;
using Autodesk.Revit.DB;
using RevitServices.Persistence;


namespace DynamoSAPTests
{
    [TestFixture]
    public class SystemTestExample : RevitSystemTestBase
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
            //Open and Run the sample file
            OpenAndRunDynamoDefinition(@".\Sample_1a_SpaceStructure.dyn");

            // Test Logic is here
            //if (false)
            //{
            //    Assert.Fail();
            //}

            //if we got here, nothing failed.
            Assert.Pass();
        }
    }
}
