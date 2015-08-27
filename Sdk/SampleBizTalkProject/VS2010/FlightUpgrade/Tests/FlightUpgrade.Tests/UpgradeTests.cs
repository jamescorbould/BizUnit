
using System.Collections.ObjectModel;
using BizUnit.TestSteps.Common;
using BizUnit.TestSteps.DataLoaders.File;
using BizUnit.TestSteps.File;
using BizUnit.TestSteps.Soap;
using BizUnit.TestSteps.ValidationSteps.Xml;
using BizUnit.Xaml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BizUnit.Sdk.FlightUpgrade.Tests
{
    /// <summary>
    /// Summary description for UpgradeTests
    /// </summary>
    [TestClass]
    public class UpgradeTests
    {
        public UpgradeTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void Upgrade_Elligable_Test()
        {
            var testCase = new TestCase();
            testCase.Name = "Upgrade_Elligable_Test";
            testCase.Purpose = "Test successful upgrade";
            testCase.Description = "Test upgrade succeeds for passenger/flight not elligable for upgrade";
            testCase.Category = "BizUnit SDK: BVT";
            testCase.Reference = "Use case: 10.3.4";
            testCase.ExpectedResults = "Upgrade succeeds";
            testCase.Preconditions = "Solution should be deployed, bound and started";

            // First ensure the target directory is empty...
            var delFiles = new DeleteStep();
            delFiles.FilePathsToDelete = new Collection<string> {@"..\..\..\Data\Out\*.xml"};
            testCase.SetupSteps.Add(delFiles);

            // Then execute the main scenario, execute a response-response web set step which is executed concurrently.
            // i.e. whilst this step is waiting for the response the next step, FileReadMultipleStep and then CreateStep
            // will be executed.
            var wsStep = new WebServiceStep();
            wsStep.Action = "Upgrade";
            wsStep.ServiceUrl = "http://localhost/BizUnit.Sdk.FlightUpgrade/BizUnit_Sdk_FlightUpgrade_ProcessRequest_UpgradePort.svc";
            wsStep.RequestBody = new FileDataLoader { FilePath = @"..\..\..\Tests\FlightUpgrade.Tests\Data\Request.xml" };
            wsStep.RunConcurrently = true;

            // Add validation....
            var validation = new XmlValidationStep();
            var schemaResultType = new SchemaDefinition
            {
                XmlSchemaPath = @"..\..\..\Src\FlightUpgrade\ResponseMsg.xsd",
                XmlSchemaNameSpace = "http://bizUnit.sdk.flightUpgrade/upgradeResponse"
            };
            validation.XmlSchemas.Add(schemaResultType);

            var responseXpath = new XPathDefinition();
            responseXpath.Description = "GetProducts_RS/Result/result";
            responseXpath.XPath = "/*[local-name()='UpgradeResponse' and namespace-uri()='http://bizUnit.sdk.flightUpgrade/upgradeResponse']/*[local-name()='UpgradeResult' and namespace-uri()='']/*[local-name()='Result' and namespace-uri()='']";
            responseXpath.Value = "true";
            validation.XPathValidations.Add(responseXpath);

            var fileReadStep = new FileReadMultipleStep();
            fileReadStep.DirectoryPath = @"..\..\..\Data\Out";
            fileReadStep.SearchPattern = "*.xml";
            fileReadStep.ExpectedNumberOfFiles = 1;
            fileReadStep.Timeout = 5000;
            fileReadStep.DeleteFiles = true;

            var createFileStep = new CreateStep();
            createFileStep.CreationPath = @"..\..\..\Data\In\UpgradeResponse.xml";
            createFileStep.DataSource = new FileDataLoader { FilePath = @"..\..\..\Tests\FlightUpgrade.Tests\Data\Response.xml" };
          
            testCase.ExecutionSteps.Add(wsStep);
            testCase.ExecutionSteps.Add(fileReadStep);
            testCase.ExecutionSteps.Add(createFileStep);

            var bizUnit = new BizUnit(testCase);
            bizUnit.RunTest();
            TestCase.SaveToFile(testCase, "Upgrade_Elligable_Test.xml");
        }

        [TestMethod]
        public void Upgrade_NotElligable_NoResponseValidation_Test()
        {
            var testCase = new TestCase();
            testCase.Name = "Upgrade_NotElligable_NoResponseValidation_Test";
            testCase.Purpose = "Test failed upgrade";
            testCase.Description = "Test upgrade denied for passenger/flight not elligable for upgrade";
            testCase.Category = "BizUnit SDK: BVT";
            testCase.Reference = "Use case: 10.3.5";
            testCase.ExpectedResults = "Upgrade failed";
            testCase.Preconditions = "Solution should be deployed, bound and started";

            var wsStep = new WebServiceStep();
            wsStep.Action = "Upgrade";
            wsStep.ServiceUrl = "http://localhost/BizUnit.Sdk.FlightUpgrade/BizUnit_Sdk_FlightUpgrade_ProcessRequest_UpgradePort.svc";
            wsStep.RequestBody = new FileDataLoader { FilePath = @"..\..\..\Tests\FlightUpgrade.Tests\Data\Request_NotElligible.xml" };
            wsStep.RunConcurrently = true;

            testCase.ExecutionSteps.Add(wsStep);

            var bizUnit = new BizUnit(testCase);
            bizUnit.RunTest();
            TestCase.SaveToFile(testCase, "Upgrade_NotElligable_NoResponseValidation_Test.xml");
        }
        
        [TestMethod]
        public void Upgrade_NotElligable_Test()
        {
            var testCase = new TestCase();
            testCase.Name = "Upgrade_NotElligable_Test";
            testCase.Purpose = "Test failed upgrade";
            testCase.Description = "Test upgrade denied for passenger/flight not elligable for upgrade";
            testCase.Category = "BizUnit SDK: BVT";
            testCase.Reference = "Use case: 10.3.5";
            testCase.ExpectedResults = "Upgrade failed";
            testCase.Preconditions = "Solution should be deployed, bound and started";

            var wsStep = new WebServiceStep();
            wsStep.Action = "Upgrade";
            wsStep.ServiceUrl = "http://localhost/BizUnit.Sdk.FlightUpgrade/BizUnit_Sdk_FlightUpgrade_ProcessRequest_UpgradePort.svc";
            wsStep.RequestBody = new FileDataLoader { FilePath = @"..\..\..\Tests\FlightUpgrade.Tests\Data\Request_NotElligible.xml" };
            wsStep.RunConcurrently = true;
            
            // Add validation....
            var validation = new XmlValidationStep();
            var schemaResultType = new SchemaDefinition
            {
                XmlSchemaPath = @"..\..\..\Src\FlightUpgrade\ResponseMsg.xsd",
                XmlSchemaNameSpace = "http://bizUnit.sdk.flightUpgrade/upgradeResponse"
            };
            validation.XmlSchemas.Add(schemaResultType);

            var responseXpath = new XPathDefinition();
            responseXpath.Description = "GetProducts_RS/Result/result";
            responseXpath.XPath = "/*[local-name()='UpgradeResponse' and namespace-uri()='http://bizUnit.sdk.flightUpgrade/upgradeResponse']/*[local-name()='UpgradeResult' and namespace-uri()='']/*[local-name()='Result' and namespace-uri()='']";
            responseXpath.Value = "false";
            validation.XPathValidations.Add(responseXpath);

            wsStep.SubSteps.Add(validation);

            testCase.ExecutionSteps.Add(wsStep);

            var bizUnit = new BizUnit(testCase);
            bizUnit.RunTest();
            TestCase.SaveToFile(testCase, "Upgrade_NotElligable_Test.xml");
        }
    }
}
