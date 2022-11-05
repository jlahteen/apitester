
using System;
using System.IO;
using System.Net;
using System.Xml;

namespace ApiTester
{
    /// <summary>
    /// Represents a Web service call operation.
    /// </summary>
    public class WebServiceCall : HttpRequest
    {
        #region Public Methods

        /// <summary>
        /// Initializes this WebServiceCall instance.
        /// </summary>
        /// <param name="webServiceCallNode">Specifies a webServiceCall XML node based on which the instance will be
        /// initialized.</param>
        public override void Initialize(XmlNode webServiceCallNode)
        {
            Initialize(webServiceCallNode, "text/xml", "POST");
        }

        #endregion
    }
}
