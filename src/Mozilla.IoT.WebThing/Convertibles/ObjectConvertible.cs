namespace Mozilla.IoT.WebThing.Convertibles
{
    /// <summary>
    /// Convert value to <see cref="bool"/>
    /// </summary>
    public class ObjectConvertible : IConvertible
    {
        /// <summary>
        /// Static Instance of <see cref="ObjectConvertible"/>
        /// </summary>
        public static ObjectConvertible Instance { get; } = new ObjectConvertible();

        /// <inheritdoc/>
        public object? Convert(object? value) 
            => value;
    }
}
