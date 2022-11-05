
using System;

namespace ApiTester
{
    /// <summary>
    /// A class that can be used for storing test operation status summaries, that is, distributions of different
    /// status values.
    /// </summary>
    public class StatusSummary
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public StatusSummary()
        {
            m_statusCount = new int[Enum.GetValues(typeof(TestElementStatus)).Length];
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Registers a specified TestElementStatus value to this StatusSummary instance.
        /// </summary>
        /// <param name="status">Specifies a TestElementStatus value.</param>
        public void RegisterStatus(TestElementStatus status)
        {
            m_statusCount[(int)status]++;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the total count of TestOperationStatus values registered to this instance.
        /// </summary>
        public int TotalStatusCount
        {
            get
            {
                int sum = 0;

                foreach (int i in m_statusCount)
                    sum += i;

                return(sum);
            }
        }

        #endregion

        #region Public Indexers

        /// <summary>
        /// Returns the registered status count corresponding to a specified TestElementStatus value.
        /// </summary>
        /// <param name="status">Specifies a TestElementStatus value.</param>
        public int this[TestElementStatus status]
        {
            get {return(m_statusCount[(int)status]);}
        }

        #endregion

        #region Public Operators

        /// <summary>
        /// Implements the binary + operator for this class.
        /// </summary>
        /// <param name="statusSummary1">Specifies a StatusSummary object.</param>
        /// <param name="statusSummary2">Specifies a StatusSummary object.</param>
        public static StatusSummary operator + (StatusSummary statusSummary1, StatusSummary statusSummary2)
        {
            StatusSummary statusSummary = new StatusSummary();

            foreach (TestElementStatus status in Enum.GetValues(typeof(TestElementStatus)))
                statusSummary.m_statusCount[(int)status] = statusSummary1.m_statusCount[(int)status] + statusSummary2.m_statusCount[(int)status];

            return(statusSummary);
        }

        #endregion

        #region Private Fields

        /// <summary>
        /// Specifies an array that contains the registered status counts.
        /// </summary>
        private int[] m_statusCount;

        #endregion
    }
}
