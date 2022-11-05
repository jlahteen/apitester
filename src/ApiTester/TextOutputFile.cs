
using System;
using System.IO;

namespace ApiTester
{
    /// <summary>
    /// Defines a class that provides methods for writing UTF-8 encoded text to a file.
    /// </summary>
    public class TextOutputFile
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="filePath">Specifies a file path. If the file doesn't exist, it will be created. If the file
        /// does exist, new content will be appended to the file.</param>
        public TextOutputFile(string filePath)
        {
            FileInfo fileInfo = new FileInfo(filePath);

            if (!Directory.Exists(fileInfo.DirectoryName))
                throw new DirectoryNotFoundException(String.Format("Directory '{0}' does not exist.", fileInfo.DirectoryName));

            m_fileExists = fileInfo.Exists;

            m_filePath = filePath;
        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="filePath">Specifies a file path. If the file doesn't exist, it will be created. If the file
        /// does exist, new content will be appended to the file.</param>
        /// <param name="autoClose">Specifies whether the file must be closed after each write operation.</param>
        public TextOutputFile(string filePath, bool autoClose) : this(filePath)
        {
            m_autoClose = autoClose;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Closes the file. If the file is not open, calling this method has no effect.
        /// </summary>
        public void Close()
        {
            if (m_file != null)
            {
                m_file.Close();

                m_file = null;
            }
        }

        /// <summary>
        /// Writes a string to the file.
        /// </summary>
        /// <param name="s">Specifies a string.</param>
        public void Write(string s)
        {
            Write(s, false, null);
        }

        /// <summary>
        /// Writes a formatted string to the file.
        /// </summary>
        /// <param name="format">Specifies a string containing format items.</param>
        /// <param name="args">An array of format objects corresponding to the format items in format.</param>
        public void Write(string format, params object[] args)
        {
            Write(format, false, args);
        }

        /// <summary>
        /// Writes a line terminator to the file.
        /// </summary>
        public void WriteLine()
        {
            Write("", true, null);
        }

        /// <summary>
        /// Writes a string with a line terminator to the file.
        /// </summary>
        /// <param name="s">Specifies a string.</param>
        public void WriteLine(string s)
        {
            Write(s, true, null);
        }

        /// <summary>
        /// Writes a formatted string with a line terminator to the file.
        /// </summary>
        /// <param name="format">Specifies a string containing format items.</param>
        /// <param name="args">An array of format objects corresponding to the format items in format.</param>
        public void WriteLine(string format, params object[] args)
        {
            Write(format, true, args);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Opens the file.
        /// </summary>
        private void Open()
        {
            if (m_fileExists)
                m_file = File.AppendText(m_filePath);
            else
                m_file = File.CreateText(m_filePath);

            m_fileExists = true;
        }

        /// <summary>
        /// Writes a string to the text file.
        /// </summary>
        /// <param name="s">Specifies a string. The string can contain format items.</param>
        /// <param name="addLineTerminator">If true, the string will be written with an ending line terminator.</param>
        /// <param name="args">An array of format objects corresponding to the format items in s. Can be null.</param>
        private void Write(string s, bool addLineTerminator, params object[] args)
        {
            if (m_file == null)
                Open();

            if (args == null)
                m_file.Write(s);
            else
                m_file.Write(s, args);

            if (addLineTerminator)
                m_file.WriteLine();

            if (m_autoClose)
                Close();
        }

        #endregion

        #region Private Fields

        /// <summary>
        /// Specifies whether the file must be closed after each write operation.
        /// </summary>
        private bool m_autoClose;

        /// <summary>
        /// Specifies a StreamWriter object for writing text to the file.
        /// </summary>
        private StreamWriter m_file;

        /// <summary>
        /// Specifies whether the file exists.
        /// </summary>
        private bool m_fileExists;

        /// <summary>
        /// Specifies the file path.
        /// </summary>
        private string m_filePath;

        #endregion
    }
}
