
using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml;

namespace ApiTester
{
    /// <summary>
    /// Represents a test phase. A test phase consists of at least one test operation represented by the
    /// <see cref="TestOperation"/> class.
    /// </summary>
    public class TestPhase : TestElement
    {
        #region Public Methods

        /// <summary>
        /// Gets the current progress status of this TestPhase instance.
        /// </summary>
        /// <param name="progressStatus">Specifies a structure where the progress status will be stored.</param>
        public void GetCurrentProgressStatus(ref ProgressStatus progressStatus)
        {
            progressStatus.CurrentOperation = m_currentOperation + 1;
            progressStatus.OperationCount = m_testOperations.Count;

            if (progressStatus.CurrentOperation > progressStatus.OperationCount)
                progressStatus.CurrentOperation = progressStatus.OperationCount;
        }

        /// <summary>
        /// Returns the average test operation time.
        /// </summary>
        public TimeSpan GetAverageTestOperationTime()
        {
            long tickSum = 0;
            int count = 0;

            foreach (TestOperation testOperation in m_testOperations)
                if (testOperation.Status == TestElementStatus.Passed || testOperation.Status == TestElementStatus.Failed)
                {
                    tickSum += testOperation.ElapsedTime.Ticks;
                    count++;
                }

            if (count > 0)
                return new TimeSpan(tickSum / count);
            else
                return new TimeSpan();
        }

        /// <summary>
        /// Returns the exception occurred in this TestPhase instance or null, if no exception has occurred.
        /// </summary>
        public string GetException()
        {
            foreach (TestOperation testOperation in m_testOperations)
                if (testOperation.Exception != null)
                    return(testOperation.Exception);

            return(null);
        }

        /// <summary>
        /// Returns the maximum test operation time.
        /// </summary>
        public TimeSpan GetMaximumTestOperationTime()
        {
            TimeSpan maxElapsedTime = new TimeSpan();

            foreach (TestOperation testOperation in m_testOperations)
                if (testOperation.Status == TestElementStatus.Passed || testOperation.Status == TestElementStatus.Failed)
                    if (testOperation.ElapsedTime > maxElapsedTime)
                        maxElapsedTime = testOperation.ElapsedTime;

            return(maxElapsedTime);
        }

        /// <summary>
        /// Returns the minimum test operation time.
        /// </summary>
        public TimeSpan GetMinimumTestOperationTime()
        {
            TimeSpan? minElapsedTime = null;

            foreach (TestOperation testOperation in m_testOperations)
                if (testOperation.Status == TestElementStatus.Passed || testOperation.Status == TestElementStatus.Failed)
                    if (minElapsedTime == null)
                        minElapsedTime = testOperation.ElapsedTime;
                    else if (testOperation.ElapsedTime < minElapsedTime)
                        minElapsedTime = testOperation.ElapsedTime;

            if (minElapsedTime == null)
                return(new TimeSpan());
            else
                return(minElapsedTime.Value);
        }

        /// <summary>
        /// Initializes this TestPhase instance and all its TestOperation instances.
        /// </summary>
        /// <param name="testPhaseNode">Specifies a testPhase XML node based on which the instance will be initialized.</param>
        public override void Initialize(XmlNode testPhaseNode)
        {
            XmlNode testOperationsNode;
            TestOperation testOperation;
            string assemblyFile, fullClassName;

            m_name = XmlUtilities.GetAttribute(testPhaseNode, "name");

            testOperationsNode = testPhaseNode.FirstChild;

            m_testOperations = new List<TestOperation>();

            foreach (XmlNode testOperationNode in testOperationsNode)
            {
                switch (testOperationNode.Name)
                {
                    case "httpGetRequest":
                        testOperation = new HttpGetRequest();

                        break;

                    case "httpPostRequest":
                        testOperation = new HttpPostRequest();

                        break;

                    case "webServiceCall":
                        testOperation = new WebServiceCall();

                        break;

                    case "testObjectMethodCall":
                        testOperation = new TestObjectMethodCall();

                        break;

                    case "customOperation":
                        assemblyFile = XmlUtilities.GetAttribute(testOperationNode, "assemblyFile");
                        fullClassName = XmlUtilities.GetAttribute(testOperationNode, "fullClassName");

                        testOperation = ObjectFactory.CreateInstance<TestOperation>(new AssemblyClassUri(assemblyFile, fullClassName));

                        break;

                    default:
                        throw new Exception(String.Format("Test operation element '{0}' is not supported.", testOperationNode.Name));
                }

                testOperation.SetParent(this);

                try
                {
                    testOperation.Initialize(testOperationNode);

                    testOperation.Status = TestElementStatus.Initialized;
                }

                catch (Exception ex)
                {
                    testOperation.Status = TestElementStatus.Faulted;

                    testOperation.SetException(ex);
                }

                m_testOperations.Add(testOperation);
            }

            this.Status = FindMostEffectiveStatus(this.Children);
        }

        /// <summary>
        /// Resets this TestPhase instance by resetting all its TestOperation instances.
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            m_currentOperation = 0;

            m_testOperationsExecuted = 0;

            foreach (TestOperation testOperation in m_testOperations)

                try
                {
                    testOperation.Reset();

                    testOperation.Status = TestElementStatus.Initialized;
                }

                catch (Exception ex)
                {
                    testOperation.Status = TestElementStatus.Faulted;

                    testOperation.SetException(ex);
                }

            this.Status = FindMostEffectiveStatus(this.Children);
        }

        /// <summary>
        /// Runs this TestPhase instance by running all its TestOperation instances.
        /// </summary>
        public override void Run()
        {
            TestElementStatus status = default(TestElementStatus);
            bool suspendOnFailedStatus = s_configSettings.SuspendTestPhaseOnFailedStatus;
            bool suspendOnFaultedStatus = s_configSettings.SuspendTestPhaseOnFaultedStatus;
            int sleepTime = s_configSettings.SleepTimeAfterTestOperation;

            this.StartStopwatch();

            // Run the test operations included in this test phase

            for ( ; m_currentOperation < m_testOperations.Count; m_currentOperation++, m_testOperationsExecuted++)
            {
                // Faulted test operations cannot be run
                if ((status = m_testOperations[m_currentOperation].Status) == TestElementStatus.Faulted)
                    if (suspendOnFaultedStatus)
                        break;
                    else
                        continue;

                try
                {
                    // Start the stopwatch
                    m_testOperations[m_currentOperation].StartStopwatch();

                    // Run the test operation
                    m_testOperations[m_currentOperation].Run();

                    // Stop the stopwatch
                    m_testOperations[m_currentOperation].StopStopwatch();

                    // Check the test result

                    if (m_testOperations[m_currentOperation].CheckTestResult())
                        m_testOperations[m_currentOperation].Status = TestElementStatus.Passed;
                    else
                    {
                        m_testOperations[m_currentOperation].Status = TestElementStatus.Failed;

                        if (suspendOnFailedStatus)
                            break;
                    }
                }

                catch (Exception ex)
                {
                    m_testOperations[m_currentOperation].Status = TestElementStatus.Faulted;

                    m_testOperations[m_currentOperation].SetException(ex);

                    if (suspendOnFaultedStatus)
                        break;
                }

                if (sleepTime != 0)
                    Thread.Sleep(sleepTime);
            }

            this.Status = FindMostEffectiveStatus(this.Children);

            this.StopStopwatch();
        }

        /// <summary>
        /// Requests all TestOperation instances to update the current status into a specified StatusSummary object.
        /// </summary>
        /// <param name="statusSummary">Specifies a StatusSummary object.</param>
        public void UpdateStatusSummary(StatusSummary statusSummary)
        {
            foreach (TestOperation testOperation in m_testOperations)
                statusSummary.RegisterStatus(testOperation.Status);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// See <see cref="TestElement.Children"/>.
        /// </summary>
        public override TestElement[] Children
        {
            get {return(m_testOperations.ToArray());}
        }

        /// <summary>
        /// Gets the number of the test operations related to this TestPhase instance.
        /// </summary>
        public int TestOperationCount
        {
            get {return(m_testOperations.Count);}
        }

        /// <summary>
        /// Gets the number of test operations that were executed in the latest call on the <see cref="Run()"/> method.
        /// </summary>
        public int TestOperationsExecuted
        {
            get {return(m_testOperationsExecuted);}
        }

        #endregion

        #region Private Fields

        /// <summary>
        /// Specifies the operation number currently running.
        /// </summary>
        private volatile int m_currentOperation;

        /// <summary>
        /// Specifies the list of the TestOperation instances belonging to this TestPhase instance.
        /// </summary>
        private List<TestOperation> m_testOperations;

        /// <summary>
        /// Stores the <see cref="TestOperationsExecuted"/> property.
        /// </summary>
        private int m_testOperationsExecuted;

        #endregion
    }
}
