namespace Mozilla.IoT.WebThing.Convertibles
{
    /// <summary>
    /// Convert value to <see cref="bool"/>
    /// </summary>
    public class BooleanConvertible : IConvertible
    {
        /// <summary>
        /// Static Instance of <see cref="BooleanConvertible"/>
        /// </summary>
        public static BooleanConvertible Instance { get; } = new BooleanConvertible();
        
        /// <inheritdoc/>
        public object? Convert(object? value) 
            => (bool?) value;
    }
}
