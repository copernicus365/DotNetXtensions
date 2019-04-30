using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

#if !DNXPrivate
namespace DotNetXtensions
{
	public
#else
namespace DotNetXtensionsPrivate
{
#endif
	static class XConsole
	{

		/// <summary>
		/// Obtains the next character or function key pressed by the user, but validates that 
		/// only a single character was entered by returning the errorKey value if more than one 
		/// key was entered. This is done by checking if Console.KeyAvailable is true, and if it is, 
		/// Console.ReadKey is called repeatedly (which does not prompt the user, so happens immediately,
		/// which is actually the core problem this function is meant to solve)
		/// until Console.KeyAvailable is false.
		/// </summary>
		/// <param name="intercept">Determines whether to display the pressed key in the console window.
		/// TRUE to NOT display the pressed key.</param>
		/// <param name="errorKey">The error key to return if more than one character was entered
		/// by the prompt after this function calls Console.ReadKey.</param>
		[DebuggerStepThrough]
		public static ConsoleKey ReadKeySingle(bool intercept = false, ConsoleKey errorKey = ConsoleKey.Clear)
		{
			ConsoleKey key = Console.ReadKey(intercept).Key;
			while (Console.KeyAvailable) {
				Console.ReadKey(true);
				key = errorKey;
			}
			return key;
		}


	}
}