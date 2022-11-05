
using System;
using System.Xml;

namespace ApiTester
{
    /// <summary>
    /// Represents an HTTP GET request operation.
    /// </summary>
    public class HttpGetRequest : HttpRequest
    {
        #region Public Methods

        /// <summary>
        /// Initializes this HttpGetRequest instance.
        /// </summary>
        /// <param name="httpGetRequestNode">Specifies an httpGetRequest XML node based on which the instance will be
        /// initialized.</param>
        public override void Initialize(XmlNode httpGetRequestNode)
        {
            Initialize(httpGetRequestNode, "text/html", "GET");
        }

        #endregion
    }
}
