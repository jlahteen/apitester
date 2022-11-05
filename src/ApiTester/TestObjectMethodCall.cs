
using System;
using System.Reflection;
using System.Xml;

namespace ApiTester
{
    /// <summary>
    /// Defines a test operation that represents a method call on a test object.
    /// </summary>
    public class TestObjectMethodCall : TestOperation
    {
        #region Public Methods

        /// <summary>
        /// Checks the test result for this TestObjectMethodCall instance.
        /// </summary>
        /// <returns>Returns true if the test result is correct, otherwise false.</returns>
        public override bool CheckTestResult()
        {
            if (m_trueReturnValue == null)
                return(true);

            else if (m_realReturnValue.GetType().FullName != m_trueReturnValue.GetType().FullName)
                return(false);

            else if (m_realReturnValue.ToString() != m_trueReturnValue.ToString())
                return(false);

            else
                return(true);
        }

        /// <summary>
        /// Initializes this TestObjectMethodCall instance.
        /// </summary>
        /// <param name="testObjectMethodCallNode">Specifies a testObjectMethodCall XML node based on which the
        /// instance will be initialized.</param>
        public override void Initialize(XmlNode testObjectMethodCallNode)
        {
            XmlNamespaceManager namespaceManager = TestProcedure.CreateXmlNamespaceManager(testObjectMethodCallNode.OwnerDocument.NameTable);
            TestProcedure testProcedure;
            XmlNode returnsNode;

            m_methodName = XmlUtilities.GetAttribute(testObjectMethodCallNode, "method");

            testProcedure = (TestProcedure)this.Parent.Parent;

            m_testObject = testProcedure.GetTestObject(XmlUtilities.GetAttribute(testObjectMethodCallNode, "testObject"));

            m_parameters = TestObject.CreateParameters(testObjectMethodCallNode.SelectSingleNode("def:parameters", namespaceManager));

            if ((returnsNode = testObjectMethodCallNode.SelectSingleNode("def:returns", namespaceManager)) != null)
                m_trueReturnValue = TestObject.CreateValueTypeObject(XmlUtilities.GetAttribute(returnsNode, "type"), XmlUtilities.GetAttribute(returnsNode, "value"));
        }

        /// <summary>
        /// Runs the test object method associated with this TestObjectMethodCall instance.
        /// </summary>
        public override void Run()
        {
            m_realReturnValue = m_testObject.RunMethod(m_methodName, m_parameters);
        }

        #endregion

        #region Private Fields

        /// <summary>
        /// Specifies the name of the method to be called.
        /// </summary>
        private string m_methodName;

        /// <summary>
        /// Specifies an array of parameters to be passed for the method.
        /// </summary>
        private object[] m_parameters;

        /// <summary>
        /// Specifies the real value returned by the method.
        /// </summary>
        private object m_realReturnValue;

        /// <summary>
        /// Specifies the TestObject instance that executes the method.
        /// </summary>
        private TestObject m_testObject;

        /// <summary>
        /// Specifies the true value for the method to return.
        /// </summary>
        private object m_trueReturnValue;

        #endregion
    }
}
