using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetXtensions
{
	/// <summary>
	/// Type that allows one to write output in either fixed columns or
	/// in auto-figured width columns. This builder allows one to build up
	/// the ouput that will be fed into one of the two main static functions,
	/// which makes it much more user-friendly.
	/// 
	/// The internal static function PadElementsInLines was taken mostly from:
	/// http://pastebin.com/CVkavHgy
	/// which itself was admittedly "inspired from": 
	/// http://stackoverflow.com/a/27399595/264031
	/// 
	/// <para/>
	/// Example:
	/// <![CDATA[
	/// // Auto-width output (notice 0 or 1 arguments in constructor [padding size, default: 1])
	/// var b = new PaddedOutputBuilder(padding: 5)
	/// 			.Add("SomeReallyCoolUrl", "http://google.com/", 2323)
	/// 			.Add("ShortyUrl",   "http://dot.net/", 323923829);
	/// 
	/// string res = b.ToString().Print();
	/// 
	/// // Fixed width output per column. 
	/// var bFixed = new PaddedOutputBuilder(20, 30, 12) // args are for: params int[] paddingPerColumn
	/// 			.Add("SomeReallyCoolUrl", "http://google.com/", 2323)
	/// 			.Add("ShortyUrl",   "http://dot.net/", 323923829);
	/// 
	/// string res2 = bFixed.ToString().Print();
	/// 
	/// /*
	/// OUTPUT (Auto Width):
	/// SomeReallyCoolUrl     http://google.com/     2323 
	/// ShortyUrl             http://dot.net/        323923829
	/// 
	/// OUTPUT (Fixed Column Width):
	/// SomeReallyCoolUrl   http://google.com/            2323        
	/// ShortyUrl           http://dot.net/               323923829   
	/// */
	/// ]]>
	/// </summary>
	public class GridOutputBuilder
	{
		int _padding;
		int _numberOfColumns = -1;
		int[] _paddingPerColumn;
		List<string[]> _lines = new List<string[]>();

		public bool IsFixedColumnSize {
			get {
				return _paddingPerColumn.NotNulle();
			}
		}

		public GridOutputBuilder(int padding = 1)
		{
			_padding = padding;
		}

		public GridOutputBuilder(params int[] paddingPerColumn)
		{
			if(paddingPerColumn.IsNulle())
				throw new ArgumentNullException();
			_paddingPerColumn = paddingPerColumn;
			_numberOfColumns = paddingPerColumn.Length;
		}


		public GridOutputBuilder Add(params object[] items)
		{
			if(items.NotNulle()) {

				if(_numberOfColumns < 0)
					_numberOfColumns = items.Length;
				else if(_numberOfColumns != items.Length)
					throw new ArgumentOutOfRangeException();

				_lines.Add(items.Select(obj => obj?.ToString()).ToArray());
			}
			return this;
		}

		/// <summary>
		/// Effeciently clears the builder so it can be reused. The number of columns
		/// goes back to not set (so you can change the column count with the first Add after calling this).
		/// The only expensive operation is calling lines.Clear() on the internal List of string.
		/// </summary>
		public GridOutputBuilder Clear()
		{
			//if(padding != null) //int? padding = null
			//	_padding = padding.Value;
			_numberOfColumns = -1;
			_lines.Clear();
			return this;
		}



		/// <summary>
		/// Converts the input List of string arrays - where the outer List represents lines, and the inner 
		/// collection represents individual grid items) to a equal columned grid output string. 
		/// Each item in each line will be padded with the input padding size. Each array 
		/// must contain the same number of elements (i.e. grid items - they CAN be null or empty, but need to all
		/// be of the same size).
		/// <param name="lines">A list of string arrays, where each array in the list represents a row in the output grid.</param>
		/// <param name="padding">Additional padding between each element (default = 1)</param>
		/// </summary>
		public static string ToGridOutputAuto(int padding, List<string[]> lines)
		{
			int numLines = lines[0].Length;
			var maxValues = new int[numLines];

			for(int i = 0; i < numLines; i++)
				maxValues[i] = lines.Max(x => (x.Length > i + 1 && x[i] != null ? x[i].Length : 0)) + padding;

			var sb = new StringBuilder();

			for(int i = 0; i < lines.Count; i++) {

				string[] arr = lines[i];

				if(i != 0)
					sb.AppendLine();

				for(int j = 0; j < arr.Length; j++) {
					string value = arr[j] ?? "";
					string paddedVal = value.PadRight(maxValues[j]);
					sb.Append(paddedVal);
				}
			}
			return sb.ToString();
		}

		public static string ToGridOutputFixed(int[] paddingPerColumn, List<string[]> lines)
		{
			if(paddingPerColumn.IsNulle())
				throw new ArgumentOutOfRangeException();

			int columnsLen = paddingPerColumn.Length;

			var sb = new StringBuilder();

			for(int i = 0; i < lines.Count; i++) {

				string[] arr = lines[i];
				if(arr.Length != columnsLen)
					throw new ArgumentOutOfRangeException();

				if(i != 0)
					sb.AppendLine();

				for(int j = 0; j < columnsLen; j++) {
					string value = arr[j] ?? "";
					string paddedVal = value.PadRight(paddingPerColumn[j]);
					sb.Append(paddedVal);
				}
			}
			return sb.ToString();
		}


		/// <summary>
		/// Outputs this builder instance to a string based on <see cref="IsFixedColumnSize"/>.
		/// If so, as a fixed column output, else as an auto figured column width size.
		/// </summary>
		public override string ToString()
		{
			if(_lines.IsNulle())
				return null;

			if(IsFixedColumnSize)
				return ToGridOutputFixed(_paddingPerColumn, _lines);
			else
				return ToGridOutputAuto(_padding, _lines);
		}

	}
}
