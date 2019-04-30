using System;
using System.Diagnostics;
using System.Text;
using DotNetXtensions;

namespace DotNetXtensions
{
#if !DNXPrivate
	public
#endif
	static class XEasy
	{

		[DebuggerStepThrough]
		public static T write<T>(T item)
		{
			Console.Write(item);
			return item;
		}

		[DebuggerStepThrough]
		public static void writeln()
		{
			Console.WriteLine();
		}

		[DebuggerStepThrough]
		public static T writeln<T>(T item)
		{
			Console.WriteLine(item);
			//if (XEase_WriteLine_AddExtraLineBreak)
			//	Console.Write("\r\n");
			return item;
		}

	}
}
