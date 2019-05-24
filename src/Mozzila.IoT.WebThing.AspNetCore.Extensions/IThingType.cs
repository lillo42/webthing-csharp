using System.Collections.Generic;
using Mozzila.IoT.WebThing;

namespace WebThing.AspNetCore.Extensions
{
    public interface IThingType
    {
        /// <summary>
        /// The thing at the given index.
        /// </summary>
        /// <param name="index">Index of thing.</param>
        Thing this[int index] { get; }
        
        /// <summary>
        /// The <see cref="IEnumerable{T}"/>> of things.
        /// </summary>
        IEnumerable<Thing> Things { get; }
        
        /// <summary>
        /// 
        /// </summary>
        string Name { get; }
    }
}
