
namespace ApiTester
{
    #region Public Types

    /// <summary>
    /// Defines a structure to store the current progress status of a test process.
    /// </summary>
    public struct ProgressStatus
    {
        #region Public Fields

        /// <summary>
        /// Gets or sets the repeat number currently running.
        /// </summary>
        public int CurrentRepeat;

        /// <summary>
        /// Gets or sets the repeat count.
        /// </summary>
        public int RepeatCount;

        /// <summary>
        /// Gets or sets the phase number currently running.
        /// </summary>
        public int CurrentPhase;

        /// <summary>
        /// Gets or sets the phase count.
        /// </summary>
        public int PhaseCount;

        /// <summary>
        /// Gets or sets the operation number of the currently running test phase.
        /// </summary>
        public int CurrentOperation;

        /// <summary>
        /// Gets or sets the operation count of the currently running test phase.
        /// </summary>
        public int OperationCount;

        #endregion
    }

    /// <summary>
    /// Defines an enumeration for test element statuses.
    /// </summary>
    public enum TestElementStatus
    {
        /// <summary>
        /// The test element is uninitialized.
        /// </summary>
        Uninitialized,

        /// <summary>
        /// The test element has been successfully initialized.
        /// </summary>
        Initialized,

        /// <summary>
        /// The test element has been successfully executed (from a technical point of view), and the test result is
        /// correct.
        /// </summary>
        Passed,

        /// <summary>
        /// The test element has been successfully executed (from a technical point of view), but the test result is
        /// false.
        /// </summary>
        Failed,

        /// <summary>
        /// The test element is faulted, that is, an error has occurred in the test element.
        /// </summary>
        Faulted
    }

    /// <summary>
    /// Defines an enumeration for test process states.
    /// </summary>
    public enum TestProcessState
    {
        /// <summary>
        /// The test process is initializing itself.
        /// </summary>
        Initializing,

        /// <summary>
        /// The test process is running.
        /// </summary>
        Running,

        /// <summary>
        /// The test process is reporting test results.
        /// </summary>
        Reporting,

        /// <summary>
        /// The test process is resetting itself. If a test procedure is repeated multiple times, the state of a test
        /// process moves back from this state to the state Running.
        /// </summary>
        Resetting,

        /// <summary>
        /// The test process is terminating.
        /// </summary>
        Terminating,

        /// <summary>
        /// The test process is finished.
        /// </summary>
        Finished
    }

    #endregion
}
