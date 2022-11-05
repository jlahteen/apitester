
using System;
using System.Threading;
using System.Xml;

namespace ApiTester
{
    /// <summary>
    /// Represents a test process. There can be multiple test processes running simultaneously.
    /// </summary>
    public class TestProcess
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="repeatCount">An integer that specifies how many times to repeat a test procedure.</param>
        public TestProcess(int repeatCount)
        {
            m_fileNamePattern = "<testProcedureName>" + "-" + String.Format("{0:yyyy-MM-dd-HH-mm-ss}", DateTime.Now) + "-<fileType>.<threadID>";

            m_outputDirectory = CommandLineArguments.GetOptionValue("outputDirectory", ".");

            if (!m_outputDirectory.EndsWith("\\"))
                m_outputDirectory += "\\";

            m_repeatCount = repeatCount;

            m_statusSummary = new StatusSummary();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns the current progress status of this TestProcess instance.
        /// </summary>
        public ProgressStatus GetCurrentProgressStatus()
        {
            ProgressStatus progressStatus = new ProgressStatus();

            if (m_state > TestProcessState.Initializing)
            {
                progressStatus.CurrentRepeat = m_currentRepeat + 1;
                progressStatus.RepeatCount = m_repeatCount;

                if (progressStatus.CurrentRepeat > progressStatus.RepeatCount)
                    progressStatus.CurrentRepeat = progressStatus.RepeatCount;

                m_testProcedure.GetCurrentProgressStatus(ref progressStatus);
            }

            return(progressStatus);
        }

        /// <summary>
        /// Starts to execute a specified test procedure in the background.
        /// </summary>
        /// <param name="testProcedureNode">Specifies a testProcedure XML node.</param>
        public void Start(XmlNode testProcedureNode)
        {
            Thread testProcessThread;

            testProcessThread = new Thread(new ParameterizedThreadStart(this.TestProcessMain));

            testProcessThread.Start(testProcedureNode);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the current state of this TestProcess instance.
        /// </summary>
        public TestProcessState State
        {
            get {return(m_state);}
        }

        /// <summary>
        /// Gets the status summary of the executed test process. Do not call this property until the test process has
        /// finished.
        /// </summary>
        public StatusSummary StatusSummary
        {
            get
            {
                if (m_state != TestProcessState.Finished)
                    throw new InvalidOperationException("Test process is not yet finished.");

                return(m_statusSummary);
            }
        }

        /// <summary>
        /// Gets the name of the test procedure associated with this TestProcess instance.
        /// </summary>
        public string TestProcedureName
        {
            get {return(m_testProcedure.Name);}
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Builds a file name corresponding to a specified test procedure name and file type.
        /// </summary>
        /// <param name="testProcedureName">Specifies a test procedure name.</param>
        /// <param name="fileType">Specifies a file type, for example 'LogFile'.</param>
        /// <returns>Returns the built file name.</returns>
        private string BuildFileName(string testProcedureName, string fileType)
        {
            string fileName = m_fileNamePattern;

            fileName = fileName.Replace("<testProcedureName>", testProcedureName);

            fileName = fileName.Replace("<fileType>", fileType);

            fileName = fileName.Replace("<threadID>", Thread.CurrentThread.ManagedThreadId.ToString());

            return(fileName);
        }

        /// <summary>
        /// Creates an instance of ITestReportWriter corresponding to a testReportWriter option specified at the
        /// command line.
        /// </summary>
        /// <param name="testProcedureName">Specifies a test procedure name.</param>
        /// <returns>Returns the created ITestReportWriter instance or null, if 'None' was specified at the command
        /// line as a testReportWriter option.</returns>
        private ITestReportWriter CreateTestReportWriter(string testProcedureName)
        {
            string testReportWriterOption = CommandLineArguments.GetOptionValue("testReportWriter", "None");
            ITestReportWriter testReportWriter = null;

            if (testReportWriterOption.ToLower() == "none")
                testReportWriter = null;

            else if (testReportWriterOption.ToLower() == "builtin")
                testReportWriter = new TestReportWriter();

            else
                testReportWriter = ObjectFactory.CreateInstance<ITestReportWriter>(new AssemblyClassUri(testReportWriterOption));

            if (testReportWriter != null)
                testReportWriter.Open(m_outputDirectory, BuildFileName(testProcedureName, "TestReport"));

            return(testReportWriter);
        }

        /// <summary>
        /// Implements a 'main function' for test process threads. Behind each running test process there is a thread
        /// executing this TestProcess method.
        /// </summary>
        /// <param name="testProcedureNodeObject">A testProcedure XML node object that specifies a test procedure to be
        /// executed.</param>
        private void TestProcessMain(object testProcedureNodeObject)
        {
            string testProcedureName;

            try
            {
                // Get the test procedure name
                testProcedureName = XmlUtilities.GetAttribute((XmlNode)testProcedureNodeObject, "name");

                // Create a log file
                m_logFile = new TextOutputFile(m_outputDirectory + BuildFileName(testProcedureName, "LogFile") + ".log", true);

                // Create a test report writer
                m_testReportWriter = CreateTestReportWriter(testProcedureName);

                // Create a test procedure
                m_testProcedure = new TestProcedure();

                // Initialize the test procedure
                m_testProcedure.Initialize((XmlNode)testProcedureNodeObject);

                // Run the test procedure as many times as given

                for (m_currentRepeat = 0; m_currentRepeat < m_repeatCount; m_currentRepeat++)
                {
                    m_state = TestProcessState.Running;

                    m_testProcedure.Run();

                    m_state = TestProcessState.Reporting;

                    m_testProcedure.UpdateStatusSummary(m_statusSummary);

                    if (m_testReportWriter != null)
                        m_testReportWriter.WriteTestReport(m_testProcedure);

                    m_state = TestProcessState.Resetting;

                    m_testProcedure.Reset();
                }

                // Close the test report writer
                if (m_testReportWriter != null)
                    m_testReportWriter.Close();

                // Terminate the test procedure

                m_state = TestProcessState.Terminating;

                m_testProcedure.Terminate();
            }

            catch (Exception ex)
            {
                if (m_logFile != null)
                    m_logFile.WriteLine("Test process terminated unexpectedly on {0} at {1} due to the following exception:\r\n\r\n{2}", DateTime.Now.ToLongDateString(), DateTime.Now.ToLongTimeString() ,ex.ToString());
            }

            m_state = TestProcessState.Finished;
        }

        #endregion

        #region Private Fields

        /// <summary>
        /// Specifies the current repeat number when running the test procedure.
        /// </summary>
        private volatile int m_currentRepeat;

        /// <summary>
        /// Specifies the file name pattern for output files.
        /// </summary>
        private string m_fileNamePattern;

        /// <summary>
        /// Specifies the log file.
        /// </summary>
        private TextOutputFile m_logFile;

        /// <summary>
        /// Specifies the output directory.
        /// </summary>
        private string m_outputDirectory;

        /// <summary>
        /// Specifies how many times the test procedure must be repeated.
        /// </summary>
        private int m_repeatCount;

        /// <summary>
        /// Stores the <see cref="State"/> property.
        /// </summary>
        private volatile TestProcessState m_state;

        /// <summary>
        /// Stores the <see cref="StatusSummary"/> property.
        /// </summary>
        private StatusSummary m_statusSummary;

        /// <summary>
        /// Specifies the TestProcedure instance associated with this TestProcess instance.
        /// </summary>
        private TestProcedure m_testProcedure;

        /// <summary>
        /// Specifies the ITestReportWriter instance for this TestProcess instance.
        /// </summary>
        private ITestReportWriter m_testReportWriter;

        #endregion
    }
}
