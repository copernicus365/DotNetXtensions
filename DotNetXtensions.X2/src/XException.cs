using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace DotNetXtensions
{
	/// <summary>
	/// Extension methods for Collections.
	/// </summary>
#if DNXPublic
	public
#endif
	static class XException
	{
		/// <summary>
		/// Returns the InnerException if there is one, else returns self.
		/// Null input is allowed (returns ex / null back).
		/// </summary>
		public static Exception InnerExceptionOrSelf(this Exception ex)
		{
			if(ex == null)
				return ex;
			return ex.InnerException ?? ex;
		}

	}
}