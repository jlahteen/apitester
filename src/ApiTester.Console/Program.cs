
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Xml;
using ApiTester;
using Console = System.Console;

namespace ApiTester.Console
{
    /// <summary>
    /// A class that contains the main program of TestRunner.
    /// </summary>
    /// <remarks>
    /// 
    /// <para>TestRunner is a utility program that has been designed for automated testing. TestRunner is capable to
    /// run predefined test procedures saved in XML files. TestRunner can control the test execution as well as the
    /// test correctness. Test correctness means that the actual test results are equivalent to the predefined test
    /// results.</para>
    /// 
    /// <para>TestRunner can also be used for load testing. With TestRunner it’s very easy to generate high workloads
    /// againts different type of application services. Furthermore, TestRunner can produce test reports containing,
    /// for example, detailed information about elapsed times per each test phase etc. These test reports make it easy
    /// to analyze possible bottlenecks in performance of application services.</para>
    /// 
    /// <b><para>Testing Model</para></b>
    /// 
    /// <para>The testing model of TestRunner is relatively simple. The main concept is called a <i>test procedure</i>.
    /// A test procedure basically contains all the required information about how an automated test must be executed.</para>
    /// 
    /// <para>A test procedure is further divided into one or more <i>test phases</i>. There are no strict rules on how
    /// a test procedure should be divided into test phases. Test phases provide a way to structure test procedures
    /// case-specifically in the most convenient way.</para>
    /// 
    /// <para>A test phase is further divided into one or more <i>test operations</i>. A test operation is the most
    /// atomic element in the testing model of TestRunner. A test operation can be almost any single operation that
    /// makes sense from the application’s point of view.</para>
    /// 
    /// <para>TestRunner contains some built-in test operations such as HttpGetRequest, HttpPostRequest, WebServiceCall
    /// and ObjectMethodCall, but also application-specific custom operations are supported.</para>
    /// 
    /// <para>Test procedures must be saved in XML files that must conform to the certain XML schema
    /// (TestProcedure.xsd).</para>
    /// 
    /// <para><b>Command Line Options</b></para>
    /// 
    /// <para>TestRunner is a console program having the following syntax:</para>
    /// 
    /// <para>TestRunner &lt;options&gt; &lt;testProcedureXmlFile&gt;</para>
    /// 
    /// <para>Above &lt;testProcedureXmlFile&gt; specifies a test procedure XML file containing the test procedure to
    /// be executed. The options supported by TestRunner are the following:</para>
    /// 
    /// <list type="table">
    /// <listheader><term>Option</term><term>Description</term></listheader>
    /// 
    /// <item>
    /// <term>repeatCount</term>
    /// <description>Specifies how many times each launched test process repeats the test procedure. The default is 1.
    /// Use this option in load testing and in long-running stability tests.</description>
    /// </item>
    /// 
    /// <item>
    /// <term>parallelRunCount</term>
    /// <description>Specifies how many parallel test processes TestRunner launches to run the test procedure. The
    /// default is 1. This option can be used in load testing.</description>
    /// </item>
    /// 
    /// <item>
    /// <term>testReportWriter</term>
    /// <description>
    /// 
    /// <para>Specifies a .NET class that must be called to write the test report. A value must be a file URI that
    /// specifies the assembly file, class namespace and class name such that two latter of these values will be
    /// expressed with the URI fragment, e.g. file:///C:\MyAssemblies\MyAssembly.dll#MyNamespace.MyClass.</para>
    /// 
    /// <para>There are also two special values supported for this option: <i>None</i> tells to TestRunner not to write
    /// the test report; <i>BuiltIn</i> causes TestRunner to use the built-in test report writer.</para>
    /// 
    /// <para>The default for this option is None.</para>
    /// 
    /// <para>Each launched test process writes its own test report.</para>
    /// 
    /// </description>
    /// </item>
    /// 
    /// <item>
    /// <term>outputDirectory</term>
    /// <description>Specifies an output directory where TestRunner produces test report files and possible error log
    /// files. The default is the current directory.</description>
    /// </item>
    /// 
    /// </list>
    /// 
    /// <para>Specify each option at the command line in the format of /&lt;optionName&gt;:&lt;value&gt;, e.g.
    /// /repeatCount:10.</para>
    /// 
    /// <para><b>Progress Status Follow-up</b></para>
    /// 
    /// <para>After TestRunner has launched all test processes, it updates the current progress status in the console
    /// window until all test processes have been finished.</para>
    /// 
    /// <para>Finally, when the test procedure has been executed by all test processes, TestRunner lists the
    /// distribution of different the test operation statuses. This summary provides quick information for the user
    /// about how the test has gone.</para>
    /// 
    /// </remarks>
    public class Program
    {
        #region Public Methods

        /// <summary>
        /// Implements the main program of TestRunner.
        /// </summary>
        /// <param name="args">Specifies an array of command line arguments.</param>
        public static void Main(string[] args)
        {
            XmlDocument testProcedureDoc;
            XmlValidator xmlValidator;
            int repeatCount, parallelRunCount, i;
            List<TestProcess> testProcesses = new List<TestProcess>();

            // Check the number of the command line arguments

            if ((CommandLineArguments.ParameterCount != c_commandLineParameterCount) || (CommandLineArguments.OptionCount > c_maxCommandLineOptionCount))
            {
                WriteSyntaxInfo();

                return;
            }

            try
            {
                // Load the test procedure XML file
                testProcedureDoc = new XmlDocument();
                testProcedureDoc.Load(CommandLineArguments.GetParameter(0));

                // Validate the test procedure XML file
                xmlValidator = new XmlValidator();
                xmlValidator.AddSchema(AssemblyUtilities.LoadEmbeddedResourceFile("TestProcedure.xsd", null));
                xmlValidator.Validate(testProcedureDoc);

                // Get the RepeatCount command line argument
                repeatCount = CommandLineArguments.GetIntegerOptionValue("repeatCount", c_minRepeatCount, c_maxRepeatCount, 1);

                // Get the ParallelRunCount command line argument
                parallelRunCount = CommandLineArguments.GetIntegerOptionValue("parallelRunCount", c_minParallelRunCount, c_maxParallelRunCount, 1);

                // Set the common configuration settings
                TestElement.SetConfigSettings(new ConfigSettings(testProcedureDoc.DocumentElement.ChildNodes[0]));

                // Create the test processes
                for (i = 0; i < parallelRunCount; i++)
                    testProcesses.Add(new TestProcess(repeatCount));

                // Start the test processes
                for (i = 0; i < testProcesses.Count; i++)
                    testProcesses[i].Start(testProcedureDoc.DocumentElement);

                // Control the progress status of the test processes
                ControlProgressStatus(testProcesses, XmlUtilities.GetAttribute(testProcedureDoc.DocumentElement, "name"));
            }

            catch (Exception ex)
            {
                System.Console.Write(String.Format("Error: {0}\n\nPlease see the file TestRunner.log for the full exception information.\n", ex.Message));

                SaveException(ex);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Controls the progress status of specified TestProcess instances. The progress status will be updated in the
        /// console window at one second intervals. This function doesn't return until all TestProcess instances are
        /// finished.
        /// </summary>
        /// <param name="testProcesses">Specifies a list of TestProcess instances.</param>
        /// <param name="testProcedureName">Specifies the name of a test procedure that is being executed.</param>
        private static void ControlProgressStatus(List<TestProcess> testProcesses, string testProcedureName)
        {
            Stopwatch stopwatch = new Stopwatch();
            int i, completedCount;
            TestProcessState testProcessState;
            ProgressStatus progressStatus;
            TimeSpan elapsedTime;
            StatusSummary statusSummary = new StatusSummary();

            // Start the stopwatch
            stopwatch.Start();

            // Write general information to the console
            System.Console.Write(String.Format("\nRunning Test Procedure '{0}'\n", testProcedureName));
            System.Console.Write(String.Format("\nTest Started: {0:yyyy-MM-dd HH:mm:ss}\n\n", DateTime.Now));

            // Update the progress status in a loop until all test processes are finished

            System.Console.Write("Current Progress\n\n");

            do
            {
                completedCount = 0;

                System.Console.Write("  {0,7}|{1,-13}|{2,-13}|{3,-13}|{4,-13}\n", "Process", "State", "Repeat", "Phase", "Operation");
                System.Console.Write("  {0}|{1}|{2}|{3}|{4}\n", "".PadLeft(7, '-'), "".PadLeft(13, '-'), "".PadLeft(13, '-'), "".PadLeft(13, '-'), "".PadLeft(13, '-'));

                for (i = 0; i < testProcesses.Count; i++)
                {
                    testProcessState = testProcesses[i].State;

                    completedCount += testProcessState == TestProcessState.Finished ? 1 : 0;

                    System.Console.Write("  {0,7}|{1,-13}|", i + 1, testProcessState);

                    if (testProcessState == TestProcessState.Running)
                    {
                        progressStatus = testProcesses[i].GetCurrentProgressStatus();

                        System.Console.Write("{0,13}|", progressStatus.CurrentRepeat.ToString() + "/" + progressStatus.RepeatCount);
                        System.Console.Write("{0,13}|", progressStatus.CurrentPhase.ToString() + "/" + progressStatus.PhaseCount);
                        System.Console.Write("{0,13}\n", progressStatus.CurrentOperation.ToString() + "/" + progressStatus.OperationCount);
                    }
                    else
                        System.Console.Write("{0,13}|{0,13}|{0,13}\n", "-", "-", "-");
                }

                elapsedTime = stopwatch.Elapsed;

                System.Console.Write(String.Format("\n\nElapsed Time: {0:00}:{1:00}:{2:00}\n", elapsedTime.Days * 24 + elapsedTime.Hours, elapsedTime.Minutes, elapsedTime.Seconds));

                if (completedCount < testProcesses.Count)
                {
                    System.Console.Write("\nPlease wait... (Ctrl-C - Interrupt)");

                    Thread.Sleep(1000);

                    if (System.Console.CursorTop >= testProcesses.Count + 6)
                        System.Console.CursorTop -= testProcesses.Count + 6;

                    System.Console.CursorLeft = 0;
                }
                else
                {
                    foreach (TestProcess testProcess in testProcesses)
                        statusSummary += testProcess.StatusSummary;

                    System.Console.Write("\n{0,-35}\n\n", "Test Operation Status Summary:");

                    System.Console.Write("  {0,-13}|{1}|{2,-10}\n", "Status", "Percentage", "Count");
                    System.Console.Write("  {0}|{1}|{2}\n", "".PadLeft(13, '-'), "".PadLeft(10, '-'), "".PadLeft(10, '-'));

                    foreach (TestElementStatus status in Enum.GetValues(typeof(TestElementStatus)))
                        if (statusSummary[status] != 0)
                            System.Console.Write("  {0,-13}|{1,9:F2}%|{2,10}\n", status, ((double)statusSummary[status] / (double)statusSummary.TotalStatusCount) * 100, statusSummary[status]);

                    System.Console.Write("\nTest procedure has been finished. Please consult the report and log files\nfor more information about the test results.\n");
                }
            }
            while (completedCount < testProcesses.Count);
        }

        /// <summary>
        /// Saves a specified exception to a common log file.
        /// </summary>
        /// <param name="ex">Specifies an exception.</param>
        private static void SaveException(Exception ex)
        {
            TextOutputFile logFile = new TextOutputFile("TestRunner.log", true);

            logFile.WriteLine("{0}: {1}", DateTime.Now, ex.ToString());
            logFile.WriteLine();
        }

        /// <summary>
        /// Writes the syntax information to the console window.
        /// </summary>
        private static void WriteSyntaxInfo()
        {
            string version, syntaxInfo = AssemblyUtilities.LoadEmbeddedResourceFile("SyntaxInfo.txt", null);

            version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            syntaxInfo = syntaxInfo.Replace("<version>", version);

            System.Console.WriteLine(syntaxInfo);
        }

        #endregion

        #region Private Constants

        /// <summary>
        /// Defines the number for command line parameters.
        /// </summary>
        private const int c_commandLineParameterCount = 1;

        /// <summary>
        /// Defines the maximum number for command line options.
        /// </summary>
        private const int c_maxCommandLineOptionCount = 4;

        /// <summary>
        /// Defines the maximum value for the ParallelRunCount command line option.
        /// </summary>
        private const int c_maxParallelRunCount = 0x200;

        /// <summary>
        /// Defines the maximum value for the RepeatCount command line option.
        /// </summary>
        private const int c_maxRepeatCount = 0x1000000;

        /// <summary>
        /// Defines the minimum value for the ParallelRunCount command line option.
        /// </summary>
        private const int c_minParallelRunCount = 1;

        /// <summary>
        /// Defines the minimum value for the RepeatCount command line option.
        /// </summary>
        private const int c_minRepeatCount = 1;

        #endregion
    }
}
