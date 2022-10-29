
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;

namespace ApiTester
{
    /// <summary>
    /// Defines an abstract base class for runnable test elements.
    /// </summary>
    public abstract class TestElement
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public TestElement()
        {
            m_stopwatch = new Stopwatch();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Checks the test result regarding to the activity from which this TestElement instance is responsible for.
        /// </summary>
        /// <returns>Returns true if the test result is correct, otherwise false.</returns>
        /// <remarks>This method performs no checking but provides a default implementation for all classes derived
        /// from this class. The return value of this implementation is always true.</remarks>
        public virtual bool CheckTestResult()
        {
            return(true);
        }

        /// <summary>
        /// Initializes this TestElement instance.
        /// </summary>
        /// <param name="testElementNode">Specifies an XML node based on which the instance must be initialized.</param>
        public abstract void Initialize(XmlNode testElementNode);

        /// <summary>
        /// Resets this TestElement instance, that is, returns the state of the instance to the state where the
        /// <see cref="Initialize(XmlNode)"/> method has left it.
        /// </summary>
        public virtual void Reset()
        {
            m_stopwatch.Reset();
        }

        /// <summary>
        /// Runs this TestElement instance, that is, performs all that activity from which this instance is responsible
        /// for.
        /// </summary>
        public abstract void Run();

        /// <summary>
        /// Stores common configuration settings to the class. These configuration settings are shared between all test
        /// elements.
        /// </summary>
        /// <param name="configSettings">Specifies a ConfigSettings object holding the configuration settings.</param>
        public static void SetConfigSettings(ConfigSettings configSettings)
        {
            s_configSettings = configSettings;
        }

        /// <summary>
        /// Terminates the use of this TestElement instance, that is, frees all such resources taken by the instance
        /// that must be explicitly released.
        /// </summary>
        public virtual void Terminate()
        {}

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the child TestElement instances of this instance. Returns null if this instance has no children.
        /// </summary>
        public virtual TestElement[] Children
        {
            get {return(null);}
        }

        /// <summary>
        /// Gets the elapsed time related to this TestElement instance.
        /// </summary>
        public TimeSpan ElapsedTime
        {
            get {return(m_stopwatch.Elapsed);}
        }

        /// <summary>
        /// Gets the time when this TestElement instance stopped running.
        /// </summary>
        public DateTime EndTime
        {
            get {return(m_endTime);}
        }

        /// <summary>
        /// Gets the name of this TestElement instance.
        /// </summary>
        public string Name
        {
            get {return(m_name);}
        }

        /// <summary>
        /// Gets the parent TestElement instance of this instance. Returns null if this instance has no parent.
        /// </summary>
        public TestElement Parent
        {
            get {return(m_parent);}
        }

        /// <summary>
        /// Gets the time when this TestElement instance started running.
        /// </summary>
        public DateTime StartTime
        {
            get {return(m_startTime);}
        }

        #endregion

        #region Protected Fields

        /// <summary>
        /// Specifies common configuration settings that are shared between all instances of this class.
        /// </summary>
        protected static ConfigSettings s_configSettings;

        /// <summary>
        /// Stores the <see cref="Name"/> property.
        /// </summary>
        protected string m_name;

        #endregion

        #region Internal Methods

        /// <summary>
        /// Finds the most effective <see cref="Status"/> value among the TestElement objects in a specified array.
        /// </summary>
        /// <param name="testElements">Specifies an array of TestElement objects.</param>
        /// <returns>Returns the most effective Status value among the TestElement objects in the specified array.</returns>
        internal static TestElementStatus FindMostEffectiveStatus(TestElement[] testElements)
        {
            TestElementStatus status = default(TestElementStatus);

            foreach (TestElement testElement in testElements)
                if (testElement.Status > status)
                    status = testElement.Status;

            return(status);
        }

        /// <summary>
        /// Sets a specified TestElement instance as the parent for this instance.
        /// </summary>
        /// <param name="testElement">Specifies a TestElement instance.</param>
        internal void SetParent(TestElement testElement)
        {
            m_parent = testElement;
        }

        /// <summary>
        /// Starts the stopwatch that measures execution time for this TestElement instance.
        /// </summary>
        internal void StartStopwatch()
        {
            m_stopwatch.Start();

            m_startTime = DateTime.Now;
        }

        /// <summary>
        /// Stops the stopwatch that measures execution time for this TestElement instance.
        /// </summary>
        internal void StopStopwatch()
        {
            m_stopwatch.Stop();

            m_endTime = DateTime.Now;
        }

        #endregion

        #region Internal Properties

        /// <summary>
        /// Gets or sets the current status of this TestElement instance.
        /// </summary>
        internal TestElementStatus Status
        {
            get {return(m_status);}

            set {m_status = value;}
        }

        #endregion

        #region Private Fields

        /// <summary>
        /// Stores the <see cref="EndTime"/> property.
        /// </summary>
        private DateTime m_endTime;

        /// <summary>
        /// Stores the <see cref="Parent"/> property.
        /// </summary>
        private TestElement m_parent;

        /// <summary>
        /// Stores the <see cref="StartTime"/> property.
        /// </summary>
        private DateTime m_startTime;

        /// <summary>
        /// Stores the <see cref="Status"/> property.
        /// </summary>
        private TestElementStatus m_status;

        /// <summary>
        /// Specifies a Stopwatch object that measures execution time for this TestElement instance.
        /// </summary>
        private Stopwatch m_stopwatch;

        #endregion
    }
}
