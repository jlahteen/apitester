
using System;
using System.Reflection;
using System.Runtime.Remoting;

namespace ApiTester
{
    /// <summary>
    /// A static class that provides services for creating instances of such classes whose type is not directly
    /// referencable in the current programming context. A typical scenario is to create instances of classes that are
    /// not available at the build time but will implement a well-known interface.
    /// </summary>
    public static class ObjectFactory
    {
        #region Public Methods

        /// <summary>
        /// Creates an instance of a specified class by using the default constructor.
        /// </summary>
        /// <param name="assemblyClassUri">An AssemblyClassUri object specifying an assembly class.</param>
        /// <returns>Returns the created instance.</returns>
        public static object CreateInstance(AssemblyClassUri assemblyClassUri)
        {
            return(Activator.CreateInstanceFrom(assemblyClassUri.AssemblyFilePath, assemblyClassUri.FullClassName).Unwrap());
        }

        /// <summary>
        /// Creates an instance of a specified class by using a non-default constructor.
        /// </summary>
        /// <param name="assemblyClassUri">An AssemblyClassUri object specifying an assembly class.</param>
        /// <param name="parameters">An array of parameters that will be passed to the corresponding constructor.</param>
        /// <returns>Returns the created instance.</returns>
        public static object CreateInstance(AssemblyClassUri assemblyClassUri, object[] parameters)
        {
            ObjectHandle objectHandle;

            objectHandle = Activator.CreateInstanceFrom(assemblyClassUri.AssemblyFilePath, assemblyClassUri.FullClassName, false, BindingFlags.CreateInstance, null, parameters, null, null);

            return(objectHandle.Unwrap());
        }

        /// <summary>
        /// Creates an instance of a specified class by using the default constructor.
        /// </summary>
        /// <typeparam name="T">Specifies a type for the return value. An assembly class specified with the
        /// assemblyClassUri parameter must be castable to this type.</typeparam>
        /// <param name="assemblyClassUri">An AssemblyClassUri object that specifies an assembly class.</param>
        /// <returns>Returns the created instance casted to the requested type.</returns>
        public static T CreateInstance<T>(AssemblyClassUri assemblyClassUri)
        {
            return((T)CreateInstance(assemblyClassUri));
        }

        #endregion
    }
}
