
using System;
using System.Xml;

namespace ApiTester
{
    /// <summary>
    /// Represents an HTTP POST request operation.
    /// </summary>
    public class HttpPostRequest : HttpRequest
    {
        #region Public Methods

        /// <summary>
        /// Initializes this HttpPostRequest instance.
        /// </summary>
        /// <param name="httpPostRequestNode">Specifies an httpPostRequest XML node based on which the instance will be
        /// initialized.</param>
        public override void Initialize(XmlNode httpPostRequestNode)
        {
            Initialize(httpPostRequestNode, "application/x-www-form-urlencoded", "POST");
        }

        #endregion
    }
}
