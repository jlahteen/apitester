
using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Xml;

namespace ApiTester
{
    /// <summary>
    /// Defines an abstract base class for HTTP request operations.
    /// </summary>
    public abstract class HttpRequest : TestOperation
    {
        #region Public Methods

        /// <summary>
        /// Checks the test result of the HTTP request associated with this instance.
        /// </summary>
        /// <returns>Returns true if the test result is correct, otherwise false.</returns>
        public override bool CheckTestResult()
        {
            if (String.IsNullOrEmpty(m_trueResponseContent))
                return(true);
            else
                return(m_realResponseContent == m_trueResponseContent ? true : false);
        }

        /// <summary>
        /// Runs the HTTP request associated with this instance.
        /// </summary>
        public override void Run()
        {
            WebRequest request;
            StreamWriter requestWriter;
            StreamReader responseReader;

            // Create a request
            request = WebRequest.Create(m_uri);

            // Set necessary request headers

            request.Method = m_method;
            request.ContentType = m_contentType;

            if (m_headers.Count > 0)
                request.Headers.Add(m_headers);

            // Write the request content to the request if necessary

            if (!String.IsNullOrEmpty(m_requestContent))
            {
                requestWriter = new StreamWriter(request.GetRequestStream());

                requestWriter.Write(m_requestContent);

                requestWriter.Flush();
                requestWriter.Close();
            }

            // Read the response

            responseReader = new StreamReader(request.GetResponse().GetResponseStream());

            m_realResponseContent = responseReader.ReadToEnd();

            responseReader.Close();
        }

        #endregion

        #region Protected Constructors

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        protected HttpRequest()
        {
            m_headers = new NameValueCollection();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Initializes this HttpRequest instance.
        /// </summary>
        /// <param name="httpRequestNode">Specifies an httpRequest XML node based on which the instance will be
        /// initialized.</param>
        /// <param name="contentType">Specifies a content type to be used with this HTTP request.</param>
        /// <param name="method">Specifies a method to be used with this HTTP request.</param>
        protected void Initialize(XmlNode httpRequestNode, string contentType, string method)
        {
            // Initialize the URI of the HTTP request
            m_uri = XmlUtilities.GetAttribute(httpRequestNode, "uri");

            // Read the request file if necessary

            m_requestFile = XmlUtilities.GetAttribute(httpRequestNode, "requestFile");

            if (!String.IsNullOrEmpty(m_requestFile))
                if (m_requestFile.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                    m_requestContent = ReadXmlFile(m_requestFile);
                else
                    m_requestContent = ReadTextFile(m_requestFile);

            // Read the response file if necessary

            m_responseFile = XmlUtilities.GetAttribute(httpRequestNode, "responseFile");

            if (!String.IsNullOrEmpty(m_responseFile))
                if (m_responseFile.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                    m_trueResponseContent = ReadXmlFile(m_responseFile);
                else
                    m_trueResponseContent = ReadTextFile(m_responseFile);

            // Initialize the content type
            m_contentType = contentType;

            // Initialize the request method
            m_method = method;

            // Initialize the SOAPAction header in case of a Web service call
            if (this is WebServiceCall)
                m_headers.Add("SOAPAction", "\"" + XmlUtilities.GetAttribute(httpRequestNode, "soapAction") + "\"");
        }

        /// <summary>
        /// Reads a specified UTF-8 encoded text file.
        /// </summary>
        /// <param name="filePath">Specifies a text file path.</param>
        /// <returns>Returns the text file content as a string.</returns>
        protected string ReadTextFile(string filePath)
        {
            StreamReader fileReader = null;
            string fileContent;

            try
            {
                fileReader = File.OpenText(filePath);

                fileContent = fileReader.ReadToEnd();

                fileReader.Close();

                return(fileContent);
            }

            finally
            {
                if (fileReader != null)
                    fileReader.Close();
            }
        }

        /// <summary>
        /// Reads a specified XML file.
        /// </summary>
        /// <param name="filePath">Specifies an XML file path.</param>
        /// <returns>Returns the XML file content as a string.</returns>
        /// <remarks>Insignificant white space characters between XML nodes have been removed from the return value.</remarks>
        protected string ReadXmlFile(string filePath)
        {
            XmlDocument xmlDocument = new XmlDocument();

            xmlDocument.Load(filePath);

            return(xmlDocument.OuterXml);
        }

        #endregion

        #region Private Fields

        /// <summary>
        /// Specifies the content type used in this HTTP request.
        /// </summary>
        private string m_contentType;

        /// <summary>
        /// Specifies the headers of this HTTP request.
        /// </summary>
        private NameValueCollection m_headers;

        /// <summary>
        /// Specifies the method (GET or POST) used in this HTTP request.
        /// </summary>
        private string m_method;

        /// <summary>
        /// Specifies the real response content returned by this HTTP request.
        /// </summary>
        private string m_realResponseContent;

        /// <summary>
        /// Specifies the content of the HTTP request file.
        /// </summary>
        private string m_requestContent;

        /// <summary>
        /// Specifies the file containing the HTTP request.
        /// </summary>
        private string m_requestFile;

        /// <summary>
        /// Specifies the file containing the HTTP response that is expected to be returned by this HTTP request.
        /// </summary>
        private string m_responseFile;

        /// <summary>
        /// Specifies the true response content for this HTTP request.
        /// </summary>
        private string m_trueResponseContent;

        /// <summary>
        /// Specifies the URI of this HTTP request.
        /// </summary>
        private string m_uri;

        #endregion
    }
}
