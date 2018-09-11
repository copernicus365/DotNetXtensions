using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace DotNetXtensions
{
	/// <summary>
	/// A general purpose "Result" type, which can be used
	/// to return Success, a Message, optionally a Count, etc.
	/// </summary>
	public class Result
	{
		[DebuggerStepThrough]
		public Result() { }

		[DebuggerStepThrough]
		public Result(bool success, string message = null)
		{
			Success = success;
			Message = message;
		}

		public bool Success { get; set; }

		public string Message { get; set; }

		public int Count { get; set; }

		public string PrivateMessage { get; set; }

		public CRUDType CrudType { get; set; }

		public override string ToString()
		{
			return string.Format("{0} ({1}) {2}", 
				Success ? "Success" : "Fail",
				Count,
				Message.IsNulle() ? "" : Message.SubstringMax(30, "...", true));
		}

	}

	/// <summary>
	/// A general purpose "Result" type which combines a "Value" of a generic
	/// type. This can can be used to return Success, a Message, optionally a Count, 
	/// and finally a Value of specified type.
	/// </summary>
	public class Result<TResult> : Result
	{
		[DebuggerStepThrough]
		public Result() { }

		[DebuggerStepThrough]
		public Result(bool success, string message = null)
		{
			Success = success;
			Message = message;
		}

		[DebuggerStepThrough]
		public Result(bool success, string message, TResult result)
		{
			Success = success;
			Message = message;
			Value = result;
		}

		[DebuggerStepThrough]
		public Result(Result r, TResult value)
		{
			Count = r.Count;
			CrudType = r.CrudType;
			Message = r.Message;
			PrivateMessage = r.PrivateMessage;
			Success = r.Success;
			Value = value;
		}

		public TResult Value { get; set; }

	}

	public enum CRUDType
	{
		None = 0,
		Get = 1,
		Add = 2,
		Update = 3,
		Delete = 4
	}
}
