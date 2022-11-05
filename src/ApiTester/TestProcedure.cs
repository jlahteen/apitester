
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Xml;

namespace ApiTester
{
    /// <summary>
    /// Represents a test procedure. A test procedure contains at least one test phase represented by the
    /// <see cref="TestPhase"/> class.
    /// </summary>
    public class TestProcedure : TestElement
    {
        #region Public Methods

        /// <summary>
        /// Creates an XmlNamespaceManager object.
        /// </summary>
        /// <param name="nameTable">Specifies an XmlNameTable object.</param>
        /// <returns>Returns the created XmlNamespaceManager object.</returns>
        /// <remarks>The default namespace URI for test procedure XML documents have been added to the returned
        /// instance.</remarks>
        public static XmlNamespaceManager CreateXmlNamespaceManager(XmlNameTable nameTable)
        {
            XmlNamespaceManager namespaceManager = new XmlNamespaceManager(nameTable);

            namespaceManager.AddNamespace("def", c_xmlNamespaceUri);

            return(namespaceManager);
        }

        /// <summary>
        /// Gets the current progress status of this TestProcedure instance.
        /// </summary>
        /// <param name="progressStatus">Specifies a ProgressStatus structure where the current progress status will be
        /// stored.</param>
        public void GetCurrentProgressStatus(ref ProgressStatus progressStatus)
        {
            progressStatus.CurrentPhase = m_currentPhase + 1;
            progressStatus.PhaseCount = m_testPhases.Count;

            if (progressStatus.CurrentPhase > progressStatus.PhaseCount)
                progressStatus.CurrentPhase = progressStatus.PhaseCount;

            m_testPhases[progressStatus.CurrentPhase - 1].GetCurrentProgressStatus(ref progressStatus);
        }

        /// <summary>
        /// Gets a reference to a TestObject instance.
        /// </summary>
        /// <param name="name">Specifies the name of a test object as given in the test procedure XML file.</param>
        /// <returns>Returns a TestObject instance whose <see cref="TestObject.Name"/> property matches the specified
        /// name.</returns>
        public TestObject GetTestObject(string name)
        {
            return(m_testObjects[name]);
        }

        /// <summary>
        /// Initializes this TestProcedure instance and all its TestPhase instances.
        /// </summary>
        /// <param name="testProcedureNode">Specifies a testProcedure XML node based on which the instance will be
        /// initialized.</param>
        public override void Initialize(XmlNode testProcedureNode)
        {
            XmlNamespaceManager nsManager = CreateXmlNamespaceManager(testProcedureNode.OwnerDocument.NameTable);
            XmlNode testPhasesNode;
            TestPhase testPhase;

            m_name = XmlUtilities.GetAttribute(testProcedureNode, "name");

            CreateTestObjects(testProcedureNode.SelectSingleNode("def:testObjects", nsManager));

            m_testPhases = new List<TestPhase>();

            testPhasesNode = testProcedureNode.SelectSingleNode("def:testPhases", nsManager);

            foreach (XmlNode testPhaseNode in testPhasesNode.ChildNodes)
            {
                testPhase = new TestPhase();

                testPhase.SetParent(this);

                testPhase.Initialize(testPhaseNode);

                m_testPhases.Add(testPhase);
            }

            this.Status = FindMostEffectiveStatus(this.Children);
        }

        /// <summary>
        /// Requests all TestPhase instances to update the current test operation statuses into a specified
        /// StatusSummary object.
        /// </summary>
        /// <param name="statusSummary">Specifies a StatusSummary object.</param>
        public void UpdateStatusSummary(StatusSummary statusSummary)
        {
            foreach (TestPhase testPhase in m_testPhases)
                testPhase.UpdateStatusSummary(statusSummary);
        }

        /// <summary>
        /// Resets this TestProcedure instance and all its TestPhase instances.
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            foreach (TestObject testObject in m_testObjects.Values)
                testObject.Reset();

            foreach (TestPhase testPhase in m_testPhases)
                testPhase.Reset();

            this.Status = FindMostEffectiveStatus(this.Children);
        }

        /// <summary>
        /// Runs this TestProcedure by running all its TestPhase instances.
        /// </summary>
        public override void Run()
        {
            bool suspendOnFailedStatus = s_configSettings.SuspendTestProcedureOnFailedStatus;
            bool suspendOnFaultedStatus = s_configSettings.SuspendTestProcedureOnFaultedStatus;
            int sleepTime = s_configSettings.SleepTimeAfterTestPhase;

            this.StartStopwatch();

            for (m_currentPhase = 0; m_currentPhase < m_testPhases.Count; m_currentPhase++)
            {
                m_testPhases[m_currentPhase].Run();

                if (m_testPhases[m_currentPhase].Status == TestElementStatus.Failed)
                {
                    if (suspendOnFailedStatus)
                        break;
                }
                else if (m_testPhases[m_currentPhase].Status == TestElementStatus.Faulted)
                    if (suspendOnFaultedStatus)
                        break;

                if (sleepTime != 0)
                    Thread.Sleep(sleepTime);
            }

            this.Status = FindMostEffectiveStatus(this.Children);

            this.StopStopwatch();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// See <see cref="TestElement.Children"/>.
        /// </summary>
        public override TestElement[] Children
        {
            get {return(m_testPhases.ToArray());}
        }

        /// <summary>
        /// Gets a read-only collection of the TestPhase instances belonging to this TestProcedure instance.
        /// </summary>
        public ReadOnlyCollection<TestPhase> TestPhases
        {
            get {return(m_testPhases.AsReadOnly());}
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Creates necessary TestObject instances based on a specified testObjects XML node.
        /// </summary>
        /// <param name="testObjectsNode">Specifies a testObjects XML node. Can be null.</param>
        private void CreateTestObjects(XmlNode testObjectsNode)
        {
            TestObject testObject;

            m_testObjects = new SortedList<string, TestObject>();

            if (testObjectsNode == null)
                return;

            foreach (XmlNode testObjectNode in testObjectsNode.ChildNodes)
            {
                testObject = new TestObject(testObjectNode);

                m_testObjects.Add(testObject.Name, testObject);
            }
        }

        #endregion

        #region Private Constants

        /// <summary>
        /// Defines the default namespace URI for test procedure XML documents.
        /// </summary>
        private const string c_xmlNamespaceUri = "http://github.com/jlahteen/apitester/testprocedure.xsd";

        #endregion

        #region Private Fields

        /// <summary>
        /// Specifies the phase number currently running.
        /// </summary>
        private volatile int m_currentPhase;

        /// <summary>
        /// Specifies the list of TestObject instances used by the TestPhase objects belonging to this TestProcedure
        /// instance.
        /// </summary>
        private SortedList<string, TestObject> m_testObjects;

        /// <summary>
        /// Specifies the list of the TestPhase objects belonging to this TestProcedure instance.
        /// </summary>
        private List<TestPhase> m_testPhases;

        #endregion
    }
}
