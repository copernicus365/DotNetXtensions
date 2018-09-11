using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetXtensions;

namespace DotNetXtensions
{
	public static class XResultHelper
	{
		[DebuggerStepThrough]
		public static Result SetMessage(this Result result, bool success, string message)
		{
			if (result != null) {
				result.Success = success;
				result.Message = message;
				return result;
			}
			return result;
		}

		[DebuggerStepThrough]
		public static Result<T> SetMessage<T>(this Result<T> result, string message)
		{
			if (result != null) {
				result.Message = message;
				return result;
			}
			return result;
		}

		[DebuggerStepThrough]
		public static Result<T> SetMessage<T>(this Result<T> result, bool success, string message)
		{
			if (result != null) {
				result.Success = success;
				result.Message = message;
				return result;
			}
			return result;
		}

		[DebuggerStepThrough]
		public static Result SetMessage(this Result result, string message, bool? success = null, int? count = null)
		{
			if (result != null)
				result.Message = message;
			if (success != null)
				result.Success = success.Value;
			if (count != null)
				result.Count = count.Value;
			return result;
		}

		[DebuggerStepThrough]
		public static T Set<T>(this T result, bool? success = null, string message = null, int? count = null, string privMsg = null) where T : Result
		{
			if (success != null)
				result.Success = (bool)success;
			if (message != null)
				result.Message = message;
			if (count != null)
				result.Count = (int)count;
			if (privMsg != null)
				result.PrivateMessage = privMsg;
			return result;
		}

	}
}
