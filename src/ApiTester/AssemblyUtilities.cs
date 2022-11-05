
using System;
using System.IO;
using System.Reflection;

namespace ApiTester
{
    /// <summary>
    /// A static class that contains assembly utility functions.
    /// </summary>
    public static class AssemblyUtilities
    {
        #region Public Methods

        /// <summary>
        /// Loads an embedded resource file from the calling assembly.
        /// </summary>
        /// <param name="fileName">Specifies the name of a resource file to be loaded.</param>
        /// <param name="fileNamespace">Specifies the namespace of a resource file to be loaded. Can be null in which
        /// case the function loads the first resource file whose name matches the specified file name.</param>
        /// <returns>Returns the contents of the requested resource file.</returns>
        public static string LoadEmbeddedResourceFile(string fileName, string fileNamespace)
        {
            Assembly callingAssembly;
            string[] resourceNames;
            string fullFileName = null;
            StreamReader reader;
            String fileContents;

            // Get the calling assembly
            callingAssembly = Assembly.GetCallingAssembly();

            // Initialize the full file name

            if (fileNamespace == null)
            {
                resourceNames = callingAssembly.GetManifestResourceNames();

                foreach (string s in resourceNames)
                    if (s.EndsWith("." + fileName))
                    {
                        fullFileName = s;

                        break;
                    }
            }
            else
                fullFileName = fileNamespace + "." + fileName;

            // Read the contents of the file

            reader = new StreamReader(callingAssembly.GetManifestResourceStream(fullFileName));

            fileContents = reader.ReadToEnd();

            reader.Close();

            return(fileContents);
        }

        #endregion
    }
}
