
using System;

namespace ApiTester
{
    /// <summary>
    /// A static class that contains array utility functions.
    /// </summary>
    public static class ArrayUtilities
    {
        #region Public Methods

        /// <summary>
        /// Appends a new item to a specified array.
        /// </summary>
        /// <typeparam name="T">Specifies a type for array items.</typeparam>
        /// <param name="array">Specifies an array. The initial value can be null.</param>
        /// <param name="item">Specifies an item to be appended.</param>
        public static void ArrayAppend<T>(ref T[] array, T item)
        {
            if (array == null)
                Array.Resize<T>(ref array, 1);
            else
                Array.Resize<T>(ref array, array.Length + 1);

            array[array.Length - 1] = item;
        }

        #endregion
    }
}
