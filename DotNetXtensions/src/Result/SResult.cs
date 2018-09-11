using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace DotNetXtensions
{
	/// <summary>
	/// A *struct* based Result type ('S' in 'SResult' if for 'struct'), containg three fields: 
	/// A boolean Success value, a Message, and a Count. Consider this an option
	/// when you want *very low overhead (a struct)*, and might have otherwise used a KeyValuePair,
	/// which <see cref="SResult"/> is much more user-friendly than, when used for this purpose.
	/// Consider <see cref="Result"/> for a fuller optioned class based alterantive.
	/// </summary>
	public struct SResult
	{
		[DebuggerStepThrough]
		public SResult(bool success, string message = null, int count = 0)
		{
			Success = success;
			Message = message;
			Count = count;
		}

		public bool Success { get; private set; }

		public string Message { get; private set; }

		public int Count { get; private set; }

		public override string ToString()
		{
			return string.Format("{0} ({1}) {2}", 
				Success ? "Success" : "Fail",
				Count,
				Message.IsNulle() ? "" : Message.SubstringMax(30, "...", true));
		}

	}

	/// <summary>
	/// A *struct* based Result type ('S' in 'SResult' if for 'struct'), containg four fields: 
	/// A boolean Success value, a Message, a Count, and the return value <typeparamref name="TResult"/>. 
	/// Consider this an option when you want *very low overhead (a struct)*, and might have otherwise 
	/// used a KeyValuePair. Consider <see cref="Result{TResult}"/> for a fuller optioned class based alterantive.
	/// </summary>
	public struct SResult<TResult>
	{
		[DebuggerStepThrough]
		public SResult(bool success, TResult value, string message = null, int count = 0)
		{
			Success = success;
			Message = message;
			Count = count;
			Value = value;
		}

		public bool Success { get; private set; }

		public string Message { get; private set; }

		public int Count { get; private set; }

		public TResult Value { get; private set; }

		public override string ToString()
		{
			return string.Format("{0} ({1}) {2}",
				Success ? "Success" : "Fail",
				Count,
				Message.IsNulle() ? "" : Message.SubstringMax(30, "...", true));
		}

	}
}
