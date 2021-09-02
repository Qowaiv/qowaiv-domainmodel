#pragma warning disable S2365
// Properties should not make collection or array copies
// Required for debugging purposes.

using System.Collections;
using System.Diagnostics;
using System.Linq;

namespace ConquerClub.Domain.Diagnostics
{
    /// <summary>Allows the debugger to display collections.</summary>
    internal class CollectionDebugView
    {
        /// <summary>Initializes a new instance of the <see cref="CollectionDebugView"/> class.Constructor.</summary>
        /// <param name="enumeration">
        /// The enumeration that has to be shown.
        /// </param>
        public CollectionDebugView(IEnumerable enumeration) => collection = enumeration;

        /// <summary>A reference to the enumeration to display.</summary>
        private readonly IEnumerable collection;

        /// <summary>Gets the array that is shown by the debugger.</summary>
        /// <remarks>
        /// Every time the enumeration is shown in the debugger, a new array is created.
        /// By doing this, it is always in sync with the current state of the enumeration.
        /// </remarks>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public object[] Items => collection.Cast<object>().ToArray();
    }
}
