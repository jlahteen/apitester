
using System;

namespace ApiTester
{
    /// <summary>
    /// Defines a static class to facilitate command line argument handling.
    /// </summary>
    public static class CommandLineArguments
    {
        #region Public Methods

        /// <summary>
        /// Gets the value of an integer option from the command line arguments.
        /// </summary>
        /// <param name="optionName">Specifies an option name.</param>
        /// <param name="minValue">Specifies a minimum value for the option.</param>
        /// <param name="maxValue">Specifies a maximum value for the option.</param>
        /// <param name="defaultValue">Specifies a default value for the option. Can be null.</param>
        public static int GetIntegerOptionValue(string optionName, int minValue, int maxValue, int? defaultValue)
        {
            string optionValue;
            int value;

            if (defaultValue.HasValue)
                optionValue = GetOptionValue(optionName, defaultValue.ToString());
            else
                optionValue = GetOptionValue(optionName, null);

            try
            {
                value = Convert.ToInt32(optionValue);

                if (value < minValue || value > maxValue)
                    throw new OverflowException();
            }

            catch (OverflowException)
            {
                throw new ArgumentException(String.Format("Value of the command line option '{0}' must be between {1}-{2}. ", optionName, minValue, maxValue));
            }

            catch (FormatException)
            {
                throw new ArgumentException(String.Format("Value '{0}' of the command line option '{1}' is not a valid integer value.", optionValue, optionName));
            }

            return(value);
        }

        /// <summary>
        /// Gets an option value from the command line arguments.
        /// </summary>
        /// <param name="optionName">Specifies an option name.</param>
        /// <param name="defaultValue">Specifies a default value for the option. Can be null.</param>
        /// <returns>Returns the value of the specified option.</returns>
        /// <remarks>A command line argument is interpreted as an option if it begins with a '/' character.</remarks>
        /// <seealso cref="GetParameter(int)"/>
        public static string GetOptionValue(string optionName, string defaultValue)
        {
            string[] args = Environment.GetCommandLineArgs();
            string value = null;

            foreach (string arg in args)
                if (arg.StartsWith("/" + optionName + ":", StringComparison.OrdinalIgnoreCase))
                {
                    if (arg.Length > optionName.Length + "/:".Length)
                        value = arg.Substring(optionName.Length + "/:".Length);

                    break;
                }

            if (value == null)
                value = defaultValue;

            if (value == null)
                throw new ArgumentException(String.Format("Command line option '{0}' was not specified.", optionName));

            return(value);
        }

        /// <summary>
        /// Gets a command line parameter by a specified index.
        /// </summary>
        /// <param name="index">Specifies a zero-based parameter index.</param>
        /// <remarks>A command line argument is interpreted as a parameter if it doesn't begin with a '/' character.</remarks>
        /// <seealso cref="GetOptionValue(string, string)"/>
        public static string GetParameter(int index)
        {
            string[] args = Environment.GetCommandLineArgs();
            int i = -2;

            foreach (string arg in args)
            {
                if (!arg.StartsWith("/"))
                    i++;

                if (i == index)
                    return(arg);
            }

            throw new ArgumentException(String.Format("There are not enough command line arguments to get the parameter #{0}.", index));
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the number of the command line options. The return value doesn't include the number of the parameters.
        /// </summary>
        /// <seealso cref="GetOptionValue(string, string)"/>
        public static int OptionCount
        {
            get
            {
                string[] args = Environment.GetCommandLineArgs();
                int count = 0;

                foreach (string arg in args)
                    count += arg.StartsWith("/") ? 1 : 0;

                return(count);
            }
        }

        /// <summary>
        /// Gets the number of the command line parameters. The return value doesn't include the number of the options.
        /// </summary>
        /// <seealso cref="GetParameter(int)"/>
        public static int ParameterCount
        {
            get {return(Environment.GetCommandLineArgs().Length - OptionCount - 1);}
        }

        #endregion
    }
}
