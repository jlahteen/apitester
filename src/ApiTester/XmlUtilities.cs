
using System;
using System.Xml;

namespace ApiTester
{
    /// <summary>
    /// A static class that contains XML utility functions.
    /// </summary>
    public static class XmlUtilities
    {
        #region Public Methods

        /// <summary>
        /// Appends a child node to a specified node.
        /// </summary>
        /// <param name="node">Specifies a node.</param>
        /// <param name="childName">Specifies a name for the child node.</param>
        /// <returns>Returns the appended child node.</returns>
        public static XmlNode AppendChild(XmlNode node, string childName)
        {
            return(AppendChild(node, childName, null, null, null));
        }

        /// <summary>
        /// Appends a child node to a specified node.
        /// </summary>
        /// <param name="node">Specifies a node.</param>
        /// <param name="childName">Specifies a name for the child node.</param>
        /// <param name="childInnerText">Specifies an inner text for the child node.</param>
        /// <returns>Returns the appended child node.</returns>
        public static XmlNode AppendChild(XmlNode node, string childName, string childInnerText)
        {
            return(AppendChild(node, childName, childInnerText, null, null));
        }

        /// <summary>
        /// Appends a child node to a specified node.
        /// </summary>
        /// <param name="node">Specifies a node.</param>
        /// <param name="qualifiedChildName">Specifies a qualified name for the child node.</param>
        /// <param name="namespaceUri">Specifies a namespace URI for the child node.</param>
        /// <param name="innerText">Specifies an inner text for the child node. Can be null.</param>
        /// <returns>Returns the appended child node.</returns>
        public static XmlNode AppendChild(XmlNode node, string qualifiedChildName, string namespaceUri, string innerText)
        {
            XmlNode child;

            child = node.OwnerDocument.CreateElement(qualifiedChildName, namespaceUri);

            if (innerText != null)
                child.InnerText = innerText;

            return(node.AppendChild(child));
        }

        /// <summary>
        /// Appends a child node to a specified node.
        /// </summary>
        /// <param name="node">Specifies a node.</param>
        /// <param name="childName">Specifies a name for the child node.</param>
        /// <param name="childInnerText">Specifies an inner text for the child node. Can be null.</param>
        /// <param name="attrNames">A string array specifying names for the attributes to be added to the child node.</param>
        /// <param name="attrValues">A string array specifying values for the attributes to be added to the child node.</param>
        /// <returns>Returns the appended child node.</returns>
        /// <remarks>
        /// The lengths of <paramref name="attrNames"/> and <paramref name="attrValues"/> must match. The array
        /// attrNames can be null in which case the array attrValues will also be ignored.
        /// </remarks>
        public static XmlNode AppendChild(XmlNode node, string childName, string childInnerText, string[] attrNames, string[] attrValues)
        {
            XmlNode child;
            int i;
            XmlAttribute attr;

            child = node.OwnerDocument.CreateElement(childName);

            if (childInnerText != null)
                child.InnerText = childInnerText;

            if (attrNames != null)
                for (i = 0; i < attrNames.Length; i++)
                {
                    attr = node.OwnerDocument.CreateAttribute(attrNames[i]);
                    attr.Value = attrValues[i];

                    child.Attributes.Append(attr);
                }

            node.AppendChild(child);

            return(child);
        }

        /// <summary>
        /// Copies a node to another node.
        /// </summary>
        /// <param name="source">Specifies a source node.</param>
        /// <param name="destination">Specifies a destination node.</param>
        /// <remarks>Copying covers all attributes and child nodes.</remarks>
        public static void CopyNode(XmlNode source, XmlNode destination)
        {
            destination.InnerXml = source.InnerXml;

            destination.Attributes.RemoveAll();

            foreach (XmlAttribute attr in source.Attributes)
                SetAttribute(destination, attr.Name, attr.Value);
        }

        /// <summary>
        /// Gets an attribute value from a specified node.
        /// </summary>
        /// <param name="node">Specifies a node.</param>
        /// <param name="attrName">Specifies an attribute name.</param>
        /// <returns>Returns the value of the specified attribute or an empty string, if the attribute wasn't found.</returns>
        public static string GetAttribute(XmlNode node, string attrName)
        {
            XmlNode attr;

            if ((attr = node.Attributes.GetNamedItem(attrName)) != null)
                return(attr.Value);
            else
                return("");
        }

        /// <summary>
        /// Sets an attribute to a specified node.
        /// </summary>
        /// <param name="node">Specifies a node.</param>
        /// <param name="attrName">Specifies a name for the attribute to be set.</param>
        /// <param name="attrValue">Specifies a value for the attribute to be set.</param>
        /// <remarks>If the attribute already exists the function just updates its value.</remarks>
        public static void SetAttribute(XmlNode node, string attrName, string attrValue)
        {
            XmlNode attr;
            XmlAttribute newAttr;

            if ((attr = node.Attributes.GetNamedItem(attrName)) == null)
            {
                newAttr = node.OwnerDocument.CreateAttribute(attrName);

                attr = node.Attributes.Append(newAttr);
            }

            attr.Value = attrValue;
        }

        #endregion
    }
}
