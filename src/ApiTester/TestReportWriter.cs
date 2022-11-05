
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ApiTester
{
    /// <summary>
    /// Implements a built-in test report writer.
    /// </summary>
    public class TestReportWriter : ITestReportWriter
    {
        #region Public Methods

        /// <summary>
        /// See <see cref="ITestReportWriter.Close()"/>.
        /// </summary>
        public void Close()
        {
            m_testReportFile.Close();
        }

        /// <summary>
        /// See <see cref="ITestReportWriter.Open(string, string)"/>.
        /// </summary>
        public void Open(string outputDirectory, string suggestedFileName)
        {
            m_testReportFile = new TextOutputFile(outputDirectory + suggestedFileName + ".txt");
        }

        /// <summary>
        /// See <see cref="ITestReportWriter.WriteTestReport(TestProcedure)"/>.
        /// </summary>
        public void WriteTestReport(TestProcedure testProcedure)
        {
            InitializeTestPhaseNameFieldWidth(testProcedure.TestPhases);

            InitializeTestPhaseExceptionFieldWidth(testProcedure.TestPhases);

            WriteHeader(testProcedure);

            foreach (TestPhase testPhase in testProcedure.TestPhases)
                ReportTestPhase(testPhase);

            WriteSeparatorLine();

            m_testReportFile.WriteLine();
            m_testReportFile.WriteLine();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Initializes the width of the test phase exception field according to a specified TestPhase object
        /// collection.
        /// </summary>
        /// <param name="testPhases">Specifies a collection of TestPhase objects.</param>
        private void InitializeTestPhaseExceptionFieldWidth(ReadOnlyCollection<TestPhase> testPhases)
        {
            string exception;

            m_exceptionFieldWidth = 0;

            foreach (TestPhase testPhase in testPhases)
                if ((exception = testPhase.GetException()) == null)
                    continue;
                else if (exception.Length > m_exceptionFieldWidth)
                    m_exceptionFieldWidth = exception.Length;
        }

        /// <summary>
        /// Initializes the width of the test phase name field according to a specified TestPhase object collection.
        /// </summary>
        /// <param name="testPhases">Specifies a collection of TestPhase objects.</param>
        private void InitializeTestPhaseNameFieldWidth(ReadOnlyCollection<TestPhase> testPhases)
        {
            m_nameFieldWidth = "Test Phase".Length;

            foreach (TestPhase testPhase in testPhases)
                if (testPhase.Name.Length > m_nameFieldWidth)
                    m_nameFieldWidth = testPhase.Name.Length;
        }

        /// <summary>
        /// Reports information related to a specified TestPhase instance.
        /// </summary>
        /// <param name="testPhase">Specifies a TestPhase instance.</param>
        private void ReportTestPhase(TestPhase testPhase)
        {
            WriteLeftAlignField(testPhase.Name, m_nameFieldWidth);

            WriteLeftAlignField(testPhase.Status.ToString(), c_statusFieldWidth);

            WriteElapsedTimeField(testPhase.ElapsedTime);

            WriteRightAlignField(testPhase.TestOperationsExecuted.ToString() + "/" + testPhase.TestOperationCount.ToString(), c_executedFieldWidth);

            WriteElapsedTimeField(testPhase.GetMinimumTestOperationTime());
            WriteElapsedTimeField(testPhase.GetMaximumTestOperationTime());
            WriteElapsedTimeField(testPhase.GetAverageTestOperationTime());

            if (testPhase.GetException() != null)
                WriteLeftAlignField(testPhase.GetException(), m_exceptionFieldWidth);
            else if (m_exceptionFieldWidth > 0)
                WriteLeftAlignField("", m_exceptionFieldWidth);

            m_testReportFile.WriteLine();
        }

        /// <summary>
        /// Writes an elapsed time field to the test report.
        /// </summary>
        /// <param name="elapsedTime">Specifies an elapsed time.</param>
        private void WriteElapsedTimeField(TimeSpan elapsedTime)
        {
            string elapsedTimeField = String.Format("{0:00}:{1:00}:{2:00}.{3:0000000}", elapsedTime.Days * 24 + elapsedTime.Hours, elapsedTime.Minutes, elapsedTime.Seconds, elapsedTime.Ticks % 10000000);

            WriteRightAlignField(elapsedTimeField, c_elapsedTimeFieldWidth);
        }

        /// <summary>
        /// Writes a header to the test report.
        /// </summary>
        /// <param name="testProcedure">Specifies a TestProcedure object.</param>
        private void WriteHeader(TestProcedure testProcedure)
        {
            // Write a separator line
            WriteSeparatorLine();

            // Write an empty line

            WriteLeftAlignField("", HeaderWidth - 1);

            if (m_exceptionFieldWidth > 0)
                WriteLeftAlignField("", m_exceptionFieldWidth);

            m_testReportFile.WriteLine();

            // Write the Test Procedure field

            m_testReportFile.Write("{0,-" + c_headerCaptionWidth.ToString() + "}", "Test Procedure:");
            WriteLeftAlignField(testProcedure.Name, HeaderFieldWidth);

            if (m_exceptionFieldWidth > 0)
                WriteLeftAlignField("", m_exceptionFieldWidth);

            m_testReportFile.WriteLine();

            // Write the Started field

            m_testReportFile.Write("{0,-" + c_headerCaptionWidth.ToString() + "}", "Started:");
            WriteLeftAlignField(String.Format("{0:yyyy-MM-dd HH:mm:ss}", testProcedure.StartTime), HeaderFieldWidth);

            if (m_exceptionFieldWidth > 0)
                WriteLeftAlignField("", m_exceptionFieldWidth);

            m_testReportFile.WriteLine();

            // Write the Elapsed Time field

            m_testReportFile.Write("{0,-" + c_headerCaptionWidth.ToString() + "}", "Elapsed Time:");
            WriteLeftAlignField(testProcedure.ElapsedTime.ToString(), HeaderFieldWidth);

            if (m_exceptionFieldWidth > 0)
                WriteLeftAlignField("", m_exceptionFieldWidth);

            m_testReportFile.WriteLine();

            // Write the Status field

            m_testReportFile.Write("{0,-" + c_headerCaptionWidth.ToString() + "}", "Status:");
            WriteLeftAlignField(testProcedure.Status.ToString(), HeaderFieldWidth);

            if (m_exceptionFieldWidth > 0)
                WriteLeftAlignField("", m_exceptionFieldWidth);

            m_testReportFile.WriteLine();

            // Write an empty line

            WriteLeftAlignField("", HeaderWidth - 1);

            if (m_exceptionFieldWidth > 0)
                WriteLeftAlignField("", m_exceptionFieldWidth);

            m_testReportFile.WriteLine();

            // Write a separator line
            WriteSeparatorLine();

            // Write the column header line #1

            WriteLeftAlignField("Test Phase", m_nameFieldWidth);
            WriteLeftAlignField("Test Phase", c_statusFieldWidth);
            WriteLeftAlignField("Test Phase", c_elapsedTimeFieldWidth);

            WriteLeftAlignField("Test Operations", c_executedFieldWidth);

            WriteLeftAlignField("Test Operation", c_elapsedTimeFieldWidth);
            WriteLeftAlignField("Test Operation", c_elapsedTimeFieldWidth);
            WriteLeftAlignField("Test Operation", c_elapsedTimeFieldWidth);

            if (m_exceptionFieldWidth > 0)
                WriteLeftAlignField("Test Phase", m_exceptionFieldWidth);

            m_testReportFile.WriteLine();

            // Write the column header line #2

            WriteLeftAlignField("Name", m_nameFieldWidth);
            WriteLeftAlignField("Status", c_statusFieldWidth);
            WriteLeftAlignField("Elapsed Time", c_elapsedTimeFieldWidth);

            WriteLeftAlignField("Executed", c_executedFieldWidth);

            WriteLeftAlignField("Minimum Time", c_elapsedTimeFieldWidth);
            WriteLeftAlignField("Maximum Time", c_elapsedTimeFieldWidth);
            WriteLeftAlignField("Average Time", c_elapsedTimeFieldWidth);

            if (m_exceptionFieldWidth > 0)
                WriteLeftAlignField("Exception", m_exceptionFieldWidth);

            m_testReportFile.WriteLine();

            // Write the column header line #3

            m_testReportFile.Write("".PadRight(m_nameFieldWidth, '-') + "|");
            m_testReportFile.Write("".PadRight(c_statusFieldWidth, '-') + "|");
            m_testReportFile.Write("".PadRight(c_elapsedTimeFieldWidth, '-') + "|");

            m_testReportFile.Write("".PadRight(c_executedFieldWidth, '-') + "|");

            m_testReportFile.Write("".PadRight(c_elapsedTimeFieldWidth, '-') + "|");
            m_testReportFile.Write("".PadRight(c_elapsedTimeFieldWidth, '-') + "|");
            m_testReportFile.Write("".PadRight(c_elapsedTimeFieldWidth, '-') + "|");

            if (m_exceptionFieldWidth > 0)
                m_testReportFile.Write("".PadRight(m_exceptionFieldWidth, '-') + "|");

            m_testReportFile.WriteLine();
        }

        /// <summary>
        /// Writes a left align field to the test report.
        /// </summary>
        /// <param name="fieldValue">Specifies a field value.</param>
        /// <param name="fieldWidth">Specifies a field width.</param>
        private void WriteLeftAlignField(string fieldValue, int fieldWidth)
        {
            m_testReportFile.Write("{0,-" + fieldWidth.ToString() + "}|", fieldValue);
        }

        /// <summary>
        /// Writes a right align field to the test report.
        /// </summary>
        /// <param name="fieldValue">Specifies a field value.</param>
        /// <param name="fieldWidth">Specifies a field width.</param>
        private void WriteRightAlignField(string fieldValue, int fieldWidth)
        {
            m_testReportFile.Write("{0," + fieldWidth.ToString() + "}|", fieldValue);
        }

        /// <summary>
        /// Writes a separator line to the test report file.
        /// </summary>
        private void WriteSeparatorLine()
        {
            m_testReportFile.WriteLine("".PadRight(ReportWidth - 1, '-') + "|");
        }

        #endregion

        #region Private Properties

        /// <summary>
        /// Gets the header field width.
        /// </summary>
        private int HeaderFieldWidth
        {
            get {return(HeaderWidth - c_headerCaptionWidth - 1);}
        }

        /// <summary>
        /// Gets the header width.
        /// </summary>
        private int HeaderWidth
        {
            get {return(m_nameFieldWidth + 1 + c_statusFieldWidth + 1 + c_executedFieldWidth + 1 + 4 * (c_elapsedTimeFieldWidth + 1));}
        }

        /// <summary>
        /// Gets the report width.
        /// </summary>
        private int ReportWidth
        {
            get {return(HeaderWidth + (m_exceptionFieldWidth > 0 ? m_exceptionFieldWidth + 1 : 0));}
        }

        #endregion

        #region Private Constants

        /// <summary>
        /// Specifies the width for the elapsed time fields.
        /// </summary>
        private const int c_elapsedTimeFieldWidth = 18;

        /// <summary>
        /// Specifies the width of the test operations executed field.
        /// </summary>
        private const int c_executedFieldWidth = 15;

        /// <summary>
        /// Specifies the width for header captions.
        /// </summary>
        private const int c_headerCaptionWidth = 16;

        /// <summary>
        /// Specifies the width of the test phase status field.
        /// </summary>
        private const int c_statusFieldWidth = 13;

        #endregion

        #region Private Fields

        /// <summary>
        /// Specifies the width of the test phase exception field.
        /// </summary>
        private int m_exceptionFieldWidth;

        /// <summary>
        /// Specifies the width of the test phase name field.
        /// </summary>
        private int m_nameFieldWidth;

        /// <summary>
        /// Specifies the test report file.
        /// </summary>
        private TextOutputFile m_testReportFile;

        #endregion
    }
}
