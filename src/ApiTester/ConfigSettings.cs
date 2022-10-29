
using System;
using System.Linq;
using System.Xml;

namespace ApiTester
{
    /// <summary>
    /// Represents a class that encapsulates common configuration settings. These configuration settings are shared
    /// between all <see cref="TestElement"/> instances.
    /// </summary>
    public class ConfigSettings
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="configSettingsNode">Specifies a configSettings XML node holding common configuration settings
        /// as attributes.</param>
        public ConfigSettings(XmlNode configSettingsNode)
        {
            string value;

            if (!String.IsNullOrEmpty(value = XmlUtilities.GetAttribute(configSettingsNode, "sleepTimeAfterTestOperation")))
                m_sleepTimeAfterTestOperation = Convert.ToInt32(value);

            if (!String.IsNullOrEmpty(value = XmlUtilities.GetAttribute(configSettingsNode, "sleepTimeAfterTestPhase")))
                m_sleepTimeAfterTestPhase = Convert.ToInt32(value);

            if ((new string[]{"", "1", "true"}).Contains<string>(XmlUtilities.GetAttribute(configSettingsNode, "suspendTestPhaseOnFailedStatus")))
                m_suspendTestPhaseOnFailedStatus = true;
            else
                m_suspendTestPhaseOnFailedStatus = false;

            if ((new string[]{"", "1", "true"}).Contains<string>(XmlUtilities.GetAttribute(configSettingsNode, "suspendTestPhaseOnFaultedStatus")))
                m_suspendTestPhaseOnFaultedStatus = true;
            else
                m_suspendTestPhaseOnFaultedStatus = false;

            if ((new string[]{"", "1", "true"}).Contains<string>(XmlUtilities.GetAttribute(configSettingsNode, "suspendTestProcedureOnFailedStatus")))
                m_suspendTestProcedureOnFailedStatus = true;
            else
                m_suspendTestProcedureOnFailedStatus = false;

            if ((new string[]{"", "1", "true"}).Contains<string>(XmlUtilities.GetAttribute(configSettingsNode, "suspendTestProcedureOnFaultedStatus")))
                m_suspendTestProcedureOnFaultedStatus = true;
            else
                m_suspendTestProcedureOnFaultedStatus = false;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the time in milliseconds to suspend execution after each test operation. The default is 0.
        /// </summary>
        public int SleepTimeAfterTestOperation
        {
            get {return(m_sleepTimeAfterTestOperation);}
        }

        /// <summary>
        /// Gets the time in milliseconds to suspend execution after each test phase. The default is 0.
        /// </summary>
        public int SleepTimeAfterTestPhase
        {
            get {return(m_sleepTimeAfterTestPhase);}
        }

        /// <summary>
        /// Gets a boolean value determining whether an execution of a test phase must be suspended when its status
        /// turns out to be <see cref="TestElementStatus.Failed"/>. The default is true.
        /// </summary>
        public bool SuspendTestPhaseOnFailedStatus
        {
            get {return(m_suspendTestPhaseOnFailedStatus);}
        }

        /// <summary>
        /// Gets a boolean value determining whether an execution of a test phase must be suspended when its status
        /// turns out to be <see cref="TestElementStatus.Faulted"/>. The default is true.
        /// </summary>
        public bool SuspendTestPhaseOnFaultedStatus
        {
            get {return(m_suspendTestPhaseOnFaultedStatus);}
        }

        /// <summary>
        /// Gets a boolean value determining whether an execution of a test procedure must be suspended when its status
        /// turns out to be <see cref="TestElementStatus.Failed"/>. The default is true.
        /// </summary>
        public bool SuspendTestProcedureOnFailedStatus
        {
            get {return(m_suspendTestProcedureOnFailedStatus);}
        }

        /// <summary>
        /// Gets a boolean value determining whether an execution of a test procedure must be suspended when its status
        /// turns out to be <see cref="TestElementStatus.Faulted"/>. The default is true.
        /// </summary>
        public bool SuspendTestProcedureOnFaultedStatus
        {
            get {return(m_suspendTestProcedureOnFaultedStatus);}
        }

        #endregion

        #region Private Fields

        /// <summary>
        /// Stores the <see cref="SleepTimeAfterTestOperation"/> property.
        /// </summary>
        private int m_sleepTimeAfterTestOperation;

        /// <summary>
        /// Stores the <see cref="SleepTimeAfterTestPhase"/> property.
        /// </summary>
        private int m_sleepTimeAfterTestPhase;

        /// <summary>
        /// Stores the <see cref="SuspendTestPhaseOnFailedStatus"/> property.
        /// </summary>
        private bool m_suspendTestPhaseOnFailedStatus;

        /// <summary>
        /// Stores the <see cref="SuspendTestPhaseOnFaultedStatus"/> property.
        /// </summary>
        private bool m_suspendTestPhaseOnFaultedStatus;

        /// <summary>
        /// Stores the <see cref="SuspendTestProcedureOnFailedStatus"/> property.
        /// </summary>
        private bool m_suspendTestProcedureOnFailedStatus;

        /// <summary>
        /// Stores the <see cref="SuspendTestProcedureOnFaultedStatus"/> property.
        /// </summary>
        private bool m_suspendTestProcedureOnFaultedStatus;

        #endregion
    }
}
