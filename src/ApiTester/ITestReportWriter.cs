
using System;

namespace ApiTester
{
    /// <summary>
    /// Defines an interface for test report writers. This interface makes possible to implement a case-specific test
    /// report writer in such cases where the built-in writer is not suitable for some reason.
    /// </summary>
    public interface ITestReportWriter
    {
        #region Methods

        /// <summary>
        /// Closes the TestReportWriter instance. A TestReportWriter instance must perform all necessary actions in
        /// this method to finish its job.
        /// </summary>
        void Close();

        /// <summary>
        /// Opens the TestReportWriter instance. A TestReportWriter instance must perform all necessary preparations in
        /// this method in order to be able to write test reports.
        /// </summary>
        /// <param name="outputDirectory">Specifies a directory for output files. The value is quaranteed to end with a
        /// backslash ('\'). A TestReportWriter instance should respect this setting when producing output files.</param>
        /// <param name="suggestedFileName">Specifies a file name that is suggested for the test report file. This name
        /// doesn't include a file extension. A TestReportWriter instance should use this value because it is, for
        /// instance, unique enough in cases when there are multiple test processes running simultaneously.</param>
        void Open(string outputDirectory, string suggestedFileName);

        /// <summary>
        /// Writes a test report based on an executed TestProcedure instance.
        /// </summary>
        /// <param name="testProcedure">Specifies an executed TestProcedure instance.</param>
        /// <remarks>If TestRunner is ordered to repeat a test procedure multiple times, this method will be called
        /// after each single execution of the test procedure.</remarks>
        void WriteTestReport(TestProcedure testProcedure);

        #endregion
    }
}
