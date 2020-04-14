namespace Mozilla.IoT.WebThing.Convertibles.Strings
{
    /// <summary>
    /// Convert value to <see cref="string"/>
    /// </summary>
    public class StringConvertible : IConvertible
    {
        /// <summary>
        /// Static Instance of <see cref="StringConvertible"/>
        /// </summary>
        public static StringConvertible Instance { get; } = new StringConvertible();

        /// <inheritdoc/>
        public object? Convert(object? value) => value?.ToString();
    }
}
