using System;
using System.Diagnostics;
using System.Text;

namespace CorrugatedIron
{
	/// <summary>
	/// Represents a log data used by the <see cref="Logger"/> class.
	/// </summary>
	public class RiakLogData
	{
		private RiakLogLevel _level; 
		private object[] _debugging = new object[] {};
		private string _message = "empty message";

		public RiakLogLevel level
		{
			get
			{
				return _level;
			}
			internal set 
			{
				_level = value;
			}
		}
		public object[] debugging 
		{
			get
			{
				return _debugging;
			}
			internal set
			{
				_debugging = value;
			}
		}
		public string message
		{
			get 
			{
				return _message;
			}
			internal set
			{
				_message = value;
			}
		}
	}
}
