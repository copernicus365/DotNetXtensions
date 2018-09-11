using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

#if DNXPublic
namespace DotNetXtensions
#else
namespace DotNetXtensionsPrivate
#endif
{
	/// <summary>
	/// Extension methods for Collections.
	/// </summary>
#if DNXPublic
	public
#endif
	static class XPrint
	{

		#region Print

		/// <summary>
		/// Print will print an extra line break ("\r\n") if set to true.
		/// Useful when in some settings, Console.WriteLine isn't printing out
		/// a line break. For instance, this was true when running unit tests,
		/// where the result outputted to Output window without a break, 
		/// for some unknown reason.
		/// </summary>
		public static bool Setting_WriteLine_AddExtraLineBreak;


		[DebuggerStepThrough]
		static void println(object item)
		{
			Console.WriteLine(item);
			if(Setting_WriteLine_AddExtraLineBreak)
				Console.Write("\r\n");
		}

		[DebuggerStepThrough]
		static void print(object item)
		{
			Console.Write(item);
		}

		[DebuggerStepThrough]
		static void println(string item)
		{
			Console.WriteLine(item);
			if (Setting_WriteLine_AddExtraLineBreak)
				Console.Write("\r\n");
		}

		[DebuggerStepThrough]
		static void print(string item)
		{
			Console.Write(item);
		}

		[DebuggerStepThrough]
		public static T Print<T>(this T item)
		{
			println(item);
			return item;
		}

		[DebuggerStepThrough]
		public static T Print<T>(this T item, bool writeLn)
		{
			if (writeLn)
				println(item);
			else
				print(item);
			return item;
		}

		[DebuggerStepThrough]
		public static string Print(this string s)
		{
			println(s);
			return s;
		}

		[DebuggerStepThrough]
		public static string Print(this string s, bool writeLn)
		{
			if (writeLn)
				println(s);
			else
				print(s);
			return s;
		}

		[DebuggerStepThrough]
		public static void Print(this string format, params object[] args)
		{
			println(format == null ? null : format.FormatX(args));
		}

		#endregion Print

		#region PrintAndReadLine

		[DebuggerStepThrough]
		public static string PrintAndReadLine(this string s)
		{
			println(s);
			return Console.ReadLine();
		}

		public static ConsoleKeyInfo PrintAndReadKey(this string s, bool interceptKey = false)
		{
			println(s);
			return Console.ReadKey(interceptKey);
		}

		[DebuggerStepThrough]
		public static string PrintAndReadLine(this string format, params object[] args)
		{
			Print(format, args);
			return Console.ReadLine();
		}

		#endregion

		#region PrintAndReadKey

		/// <summary>
		/// Prints the input string, and then calls Console.ReadKey(true).
		/// </summary>
		/// <param name="s">string</param>
		[DebuggerStepThrough]
		public static ConsoleKey PrintAndReadKey(this string s)
		{
			println(s);
			return Console.ReadKey(true).Key;
		}

		/// <summary>
		/// Prints the input objects using the specified format string,
		/// and then calls Console.ReadKey(true).
		/// </summary>
		/// <param name="format">Format string.</param>
		/// <param name="args">Objects.</param>
		[DebuggerStepThrough]
		public static ConsoleKey PrintAndReadKey(this string format, params object[] args)
		{
			Print(format, args);
			return Console.ReadKey(true).Key;
		}

		#endregion

		#region PrintAndReadKey

		/// <summary>
		/// Prints the input string, and then calls XConsole.ReadKeySingle(true).
		/// </summary>
		/// <param name="s">string</param>
		/// <param name="errorKey"></param>

		[DebuggerStepThrough]
		public static ConsoleKey PrintAndReadSingleKey(this string s, ConsoleKey errorKey = ConsoleKey.Clear)
		{
			println(s);
			return XConsole.ReadKeySingle(true, errorKey);
		}

		/// <summary>
		/// Prints the input objects using the specified format string.
		/// </summary>
		/// <param name="format">Format string.</param>
		/// <param name="args">Objects.</param>
		[DebuggerStepThrough]
		public static ConsoleKey PrintAndReadSingleKey(this string format, params object[] args)
		{
			Print(format, args);
			return Console.ReadKey(true).Key;
		}

		#endregion

	}
}