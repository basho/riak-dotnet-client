using System;

namespace CorrugatedIron
{
	/// <summary>
	/// Contains the values of the logging level.
	/// </summary>
	public enum RiakLogLevel
	{
		// totally didn't rip off PHP right here
		/// <summary>
		/// Indicates the bottom logging level.
		/// </summary>
		Trace = 1,
		/// <summary>
		/// Indicates the 2nd logging level from the bottom.
		/// </summary>
		Debug = 2,
		/// <summary>
		/// Indicates the 3rd logging level from the bottom.
		/// </summary>
		Info = 4,
		/// <summary>
		/// Indicates the 3rd logging level from the top.
		/// </summary>
		Warn = 8,
		/// <summary>
		/// Indicates the 2nd logging level from the top.
		/// </summary>
		Error = 16,
		/// <summary>
		/// Indicates the top logging level.
		/// </summary>
		Fatal = 32
	}
}
