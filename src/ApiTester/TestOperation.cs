
using System;
using System.Xml;

namespace ApiTester
{
    /// <summary>
    /// Defines an abstract base class for test operation classes.
    /// </summary>
    /// <remarks>Custom operations can be implemented by writing classes that inherit from this class.</remarks>
    public abstract class TestOperation : TestElement
    {
        #region Public Methods

        /// <summary>
        /// Resets this TestOperation instance, that is, returns the state of the instance to the state where the
        /// <see cref="TestElement.Initialize(XmlNode)"/> method has left it.
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            m_exception = null;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the exception occurred in this TestOperation instance. Returns null if no exception has occurred.
        /// </summary>
        public string Exception
        {
            get {return(m_exception != null ? m_exception.ToString().Replace("\r\n", "") : null);}
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Sets a value for the <see cref="Exception"/> property.
        /// </summary>
        /// <param name="exception">Specifies an exception.</param>
        internal void SetException(Exception exception)
        {
            m_exception = exception;
        }

        #endregion

        #region Private Fields

        /// <summary>
        /// Stores the <see cref="Exception"/> property.
        /// </summary>
        private Exception m_exception;

        #endregion
    }
}
