using System.Diagnostics.CodeAnalysis;

namespace Mozilla.IoT.WebThing.Json.Convertibles
{
    /// <summary>
    /// Represent convertible/getter value from json object.
    /// </summary>
    public interface IJsonConvertible
    {
        /// <summary>
        /// Try convert json value to specific value.
        /// </summary>
        /// <param name="source">The json object;</param>
        /// <param name="result">The result.</param>
        /// <returns>return true if could get value, otherwise return false.</returns>
        bool TryConvert(object source, [MaybeNull]out object? result);
    }
}
