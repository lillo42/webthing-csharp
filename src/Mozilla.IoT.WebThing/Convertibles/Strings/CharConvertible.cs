namespace Mozilla.IoT.WebThing.Convertibles.Strings
{
    /// <summary>
    /// Convert value to <see cref="char"/>
    /// </summary>
    public class CharConvertible : IConvertible
    {
        /// <summary>
        /// Static Instance of <see cref="CharConvertible"/>
        /// </summary>
        public static CharConvertible Instance { get; } = new CharConvertible();
        
        
        /// <inheritdoc/>
        public object? Convert(object? value)
        {
            if (value == null)
            {
                return null;
            }
            
            return char.Parse(value.ToString()!);
        }
    }
}
