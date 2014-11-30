using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TiS.Core.TisCommon.DataModel
{
    /// <summary>
    /// This is the generic EventArgs class for event args that carry a single argument:
    /// </summary>
    /// <typeparam name="T">Argument type</typeparam>
	public class EventArgs<T> : EventArgs
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="EventArgs{T}" /> class.
        /// </summary>
        /// <param name="value">The value.</param>
		public EventArgs(T value)
		{
			m_value = value;
		}

     	private T m_value;

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
		public T Value
		{
			get { return m_value; }
		}
	}
}
