namespace Mozilla.IoT.WebThing.Convertibles
{
    /// <summary>
    /// Represent convertible object to other object.
    /// </summary>
    public interface IConvertible
    {
        /// <summary>
        /// Convert object to other object.
        /// </summary>
        /// <param name="value">The instance to be convert.</param>
        /// <returns>The convert object.</returns>
        object? Convert(object? value);
    }
}
