
using System;
using System.Reflection;
using System.Xml;

namespace ApiTester
{
    /// <summary>
    /// Represents an external object that can participate in execution of test phases. Test objects are defined at the
    /// test procedure level.
    /// </summary>
    public class TestObject
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="testObjectNode">Specifies a testObject XML node based on which the instance will be
        /// initialized.</param>
        public TestObject(XmlNode testObjectNode)
        {
            string assemblyFile, fullClassName;
            object[] parameters;
            XmlNamespaceManager namespaceManager = TestProcedure.CreateXmlNamespaceManager(testObjectNode.OwnerDocument.NameTable);
            XmlNode resetMethodNode;

            m_name = XmlUtilities.GetAttribute(testObjectNode, "name");

            assemblyFile = XmlUtilities.GetAttribute(testObjectNode, "assemblyFile");

            fullClassName = XmlUtilities.GetAttribute(testObjectNode, "fullClassName");

            parameters = CreateParameters(testObjectNode.SelectSingleNode("def:constructor/def:parameters", namespaceManager));

            m_object = ObjectFactory.CreateInstance(new AssemblyClassUri(assemblyFile, fullClassName), parameters);

            m_type = m_object.GetType();

            if ((resetMethodNode = testObjectNode.SelectSingleNode("def:resetMethod", namespaceManager)) != null)
                m_resetMethodName = XmlUtilities.GetAttribute(resetMethodNode, "name");
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a parameter array corresponding to a specified parameters XML node.
        /// </summary>
        /// <param name="parametersNode">Specifies a parameters XML node. Can be null.</param>
        /// <returns>Returns the created parameter array.</returns>
        public static object[] CreateParameters(XmlNode parametersNode)
        {
            object[] parameters;

            if (parametersNode == null)
                return(new object[0]);

            parameters = new object[parametersNode.ChildNodes.Count];

            for (int i = 0; i < parameters.Length; i++)
                parameters[i] = CreateValueTypeObject(XmlUtilities.GetAttribute(parametersNode.ChildNodes[i], "type"), XmlUtilities.GetAttribute(parametersNode.ChildNodes[i], "value"));

            return(parameters);
        }

        /// <summary>
        /// Creates a value type object corresponding to a specified value type and its value.
        /// </summary>
        /// <param name="valueTypeName">Specifies a value type name.</param>
        /// <param name="value">Specifies a value (as a string) of the specified value type.</param>
        /// <returns>Returns the created value type object.</returns>
        public static object CreateValueTypeObject(string valueTypeName, string value)
        {
            object @object = null;

            switch (valueTypeName)
            {
                case "System.Boolean":
                    @object = Convert.ToBoolean(value);

                    break;

                case "System.Byte":
                    @object = Convert.ToByte(value);

                    break;

                case "System.Char":
                    @object = Convert.ToChar(value);

                    break;

                case "System.Decimal":
                    @object = Convert.ToDecimal(value);

                    break;

                case "System.Double":
                    @object = Convert.ToDouble(value);

                    break;

                case "System.Int16":
                    @object = Convert.ToInt16(value);

                    break;

                case "System.Int32":
                    @object = Convert.ToInt32(value);

                    break;

                case "System.Int64":
                    @object = Convert.ToInt64(value);

                    break;

                case "System.SByte":
                    @object = Convert.ToSByte(value);

                    break;

                case "System.Single":
                    @object = Convert.ToSingle(value);

                    break;

                case "System.String":
                    @object = Convert.ToString(value);

                    break;

                case "System.UInt32":
                    @object = Convert.ToUInt32(value);

                    break;

                case "System.UInt64":
                    @object = Convert.ToUInt64(value);

                    break;
            }

            return(@object);
        }

        /// <summary>
        /// Resets the state of the underlying test object.
        /// </summary>
        public void Reset()
        {
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod;

            if (m_resetMethodName != null)
                m_type.InvokeMember(m_resetMethodName, bindingFlags, null, m_object, null);
        }

        /// <summary>
        /// Runs a specified method with specified parameters on the underlying test object.
        /// </summary>
        /// <param name="methodName">Specifies a method name.</param>
        /// <param name="parameters">Specifies an array of parameters.</param>
        /// <returns>Returns the return value of the executed method.</returns>
        public object RunMethod(string methodName, object[] parameters)
        {
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod;

            return(m_type.InvokeMember(methodName, bindingFlags, null, m_object, parameters));
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the name of this TestObject instance as given in the test procedure XML file.
        /// </summary>
        public string Name
        {
            get {return(m_name);}
        }

        #endregion

        #region Private Fields

        /// <summary>
        /// Stores the <see cref="Name"/> property.
        /// </summary>
        private string m_name;

        /// <summary>
        /// Specifies the actual underlying test object.
        /// </summary>
        private object m_object;

        /// <summary>
        /// Specifies the name of the method that resets the state of the underlying test object.
        /// </summary>
        private string m_resetMethodName;

        /// <summary>
        /// Specifies the type that the underlying test object represents.
        /// </summary>
        private Type m_type;

        #endregion
    }
}
