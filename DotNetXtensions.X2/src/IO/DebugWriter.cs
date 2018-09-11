using System;
using System.Diagnostics;
using System.IO;

namespace DotNetXtensions
{

	/// <summary>
	/// Overrides the many Write and WriteLine methods of StringWriter,
	/// writing these then first to Debug.Write / Debug.WriteLine, and then
	/// also to base as well (base.Write / base.WriteLine). One can change 
	/// where this writes to though by simply overriding: 
	/// <code>_Write(string value)</code> and <code>_WriteLine(string value)</code>. 
	/// All methods herein redirect to these two methods.
	/// <para />
	/// Inspired by: http://stackoverflow.com/a/1583569/264031
	/// </summary>
	public class DebugWriter : StringWriter
	{
		StreamWriter swriter;

		public TextWriter OriginalConsoleOut { get; set; }

		/// <summary>
		/// If Console.Out has been set to this or to something else,
		/// when this is true and when OriginalConsoleOut is not null,
		/// output will be written in addition to OriginalConsoleOut. 
		/// </summary>
		public bool AlsoWriteToOriginalConsoleOut { get; protected set; }



		public DebugWriter() { }

		public DebugWriter(
			string writeToLogFilePath, 
			bool deleteLogFileContentsFirst,
			bool setConsoleOutToThis = false,
			bool writeToOrigConsoleOutStill = false)
		{
			if (writeToLogFilePath.NotNulle()) {

				if (deleteLogFileContentsFirst && File.Exists(writeToLogFilePath))
					File.WriteAllText(writeToLogFilePath, ""); // clear the file contents if any

				swriter = new StreamWriter(File.Open(writeToLogFilePath,
					FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read)) { AutoFlush = true, };
			}

			if (setConsoleOutToThis)
				SetConsoleOutToThis(writeToOrigConsoleOutStill);
		}



		[DebuggerStepThrough]
		protected virtual void _Write(string value) 
		{
			Debug.Write(value);

			if (AlsoWriteToOriginalConsoleOut && OriginalConsoleOut != null)
				OriginalConsoleOut.Write(value);

            if (swriter != null)
				swriter.Write(value);
		}

		[DebuggerStepThrough]
		protected virtual void _WriteLine(string value)
		{
			Debug.WriteLine(value);

			if (AlsoWriteToOriginalConsoleOut && OriginalConsoleOut != null)
				OriginalConsoleOut.WriteLine(value);

			if (swriter != null)
				swriter.WriteLine(value);
		}

		public void SetConsoleOutToThis(bool writeToOrigConsoleOutStill = false)
		{
			OriginalConsoleOut = Console.Out;
			AlsoWriteToOriginalConsoleOut = writeToOrigConsoleOutStill;
			Console.SetOut(this);
		}

		public static DebugWriter SetConsoleOut(
			bool writeToOrigConsoleOutStill = false,
            string writeToLogFilePath = null,
			bool deleteLogFileContentsFirst = false)
		{
			return new DebugWriter(
				writeToLogFilePath,
				deleteLogFileContentsFirst,
				true, //setConsoleOutToThis
				writeToOrigConsoleOutStill);
        }

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (swriter != null)
				swriter?.Dispose();
		}

		#region --- Write Overloads ---

		[DebuggerStepThrough]
		public override void Write(object value)
		{
			_Write(value.ToStringN());
		}

		[DebuggerStepThrough]
		public override void Write(string value)
		{
			_Write(value);
		}

		public override void Write(char value)
		{
			_Write(value.ToStringN());
		}

		public override void Write(int value)
		{
			_Write(value.ToStringN());
		}

		public override void Write(long value)
		{
			_Write(value.ToStringN());
		}

		public override void Write(decimal value)
		{
			_Write(value.ToStringN());
		}

		public override void Write(double value)
		{
			_Write(value.ToStringN());
		}

		public override void Write(float value)
		{
			_Write(value.ToStringN());
		}

		public override void Write(string format, object arg0)
		{
			_Write(string.Format(format, arg0));
		}

		public override void Write(string format, object arg0, object arg1)
		{
			_Write(string.Format(format, arg0, arg1));
		}

		public override void Write(string format, object arg0, object arg1, object arg2)
		{
			_Write(string.Format(format, arg0, arg1, arg2));
		}

		public override void Write(string format, params object[] arg)
		{
			_Write(string.Format(format, arg));
		}


		public override void Write(char[] buffer)
		{
			_Write(new string(buffer));
		}

		public override void Write(char[] buffer, int index, int count)
		{
			_Write(new string(buffer, index, count));
		}


		[DebuggerStepThrough]
		public override void WriteLine()
		{
			_WriteLine(Environment.NewLine);
		}

		[DebuggerStepThrough]
		public override void WriteLine(object value)
		{
			_WriteLine(value.ToStringN());
		}

		[DebuggerStepThrough]
		public override void WriteLine(string value)
		{
			_WriteLine(value);
		}

		public override void WriteLine(char value)
		{
			_WriteLine(value.ToStringN());
		}

		public override void WriteLine(int value)
		{
			_WriteLine(value.ToStringN());
		}

		public override void WriteLine(long value)
		{
			_WriteLine(value.ToStringN());
		}

		public override void WriteLine(decimal value)
		{
			_WriteLine(value.ToStringN());
		}

		public override void WriteLine(double value)
		{
			_WriteLine(value.ToStringN());
		}

		public override void WriteLine(float value)
		{
			_WriteLine(value.ToStringN());
		}

		public override void WriteLine(string format, object arg0)
		{
			_WriteLine(string.Format(format, arg0));
		}

		public override void WriteLine(string format, object arg0, object arg1)
		{
			_WriteLine(string.Format(format, arg0, arg1));
		}

		public override void WriteLine(string format, object arg0, object arg1, object arg2)
		{
			_WriteLine(string.Format(format, arg0, arg1, arg2));
		}

		public override void WriteLine(string format, params object[] arg)
		{
			_WriteLine(string.Format(format, arg));
		}

		public override void WriteLine(char[] buffer)
		{
			_WriteLine(new string(buffer));
		}

		public override void WriteLine(char[] buffer, int index, int count)
		{
			_WriteLine(new string(buffer, index, count));
		}

		#endregion
	}
}
