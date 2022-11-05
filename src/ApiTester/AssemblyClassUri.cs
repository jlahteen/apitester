
using System;

namespace ApiTester
{
    /// <summary>
    /// Defines a class that can be used for identifying assembly classes with file URIs. An assembly class URI is a
    /// file URI whose fragment part specifies a full class name in a referenced (assembly) file. For example,
    /// file:///C:\MyAssemblies\MyAssembly.dll#MyNamespace.MyClass is a valid assembly class URI.
    /// </summary>
    public class AssemblyClassUri
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="fileUri">Specifies a file URI whose fragment part determines a full class name in a referenced
        /// (assembly) file.</param>
        /// <remarks>This constructor also accepts an incomplete file URI consisting only of a file name or a relative
        /// file path and the fragment part. For example, MyAssembly.dll#MyNamespace.MyClass is a valid file URI for
        /// this constructor.</remarks>
        public AssemblyClassUri(string fileUri)
        {
            if (fileUri == null)
                throw new ArgumentNullException("fileUri");

            fileUri = fileUri.Trim(null);

            if (fileUri.StartsWith("\\"))
                fileUri = Environment.CurrentDirectory.Substring(0, "C:".Length) + fileUri;
            else if (!fileUri.Contains(":\\"))
                fileUri = Environment.CurrentDirectory + "\\" + fileUri;

            if (!fileUri.Contains("://"))
                fileUri = "file:///" + fileUri;

            m_fileUri = new Uri(fileUri);

            if (!m_fileUri.IsFile)
                throw new UriFormatException(String.Format("Specified URI '{0}' is not a file URI.", fileUri));

            else if (!m_fileUri.Fragment.Contains("."))
                throw new UriFormatException(String.Format("Fragment '{0}' of the specified file URI '{1}' doesn't include a dot ('.') character separating the namespace and name of the class.", m_fileUri.Fragment, fileUri));

            else if (m_fileUri.Fragment.StartsWith("#."))
                throw new UriFormatException("Fragment cannot start with a dot ('.') character.");

            else if (m_fileUri.Fragment.EndsWith("."))
                throw new UriFormatException("Fragment cannot end with a dot ('.') character.");

            else if (!m_fileUri.LocalPath.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) && !m_fileUri.LocalPath.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                throw new UriFormatException("Type of the file specified by the file URI must be '.dll' or '.exe'.");
        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="assemblyFile">Specifies an assembly file. This value can be any relative or absolute file
        /// path.</param>
        /// <param name="fullClassName">Specifies the full name of a class that must be defined by the specified
        /// assembly file.</param>
        public AssemblyClassUri(string assemblyFile, string fullClassName) : this(assemblyFile + "#" + fullClassName)
        {}

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the full path of the assembly file associated with this AssemblyClassUri instance.
        /// </summary>
        public string AssemblyFilePath
        {
            get {return(m_fileUri.LocalPath);}
        }

        /// <summary>
        /// Gets the file URI that is wrapped by this instance.
        /// </summary>
        public Uri FileUri
        {
            get {return(m_fileUri);}
        }

        /// <summary>
        /// Gets the full name of the assembly class associated with this AssemblyClassUri instance.
        /// </summary>
        public string FullClassName
        {
            get {return(m_fileUri.Fragment.TrimStart(new char[]{'#'}));}
        }

        #endregion

        #region Private Fields

        /// <summary>
        /// Stores the <see cref="FileUri"/> property.
        /// </summary>
        private Uri m_fileUri;

        #endregion
    }
}
