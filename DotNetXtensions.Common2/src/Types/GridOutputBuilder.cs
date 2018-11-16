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
		public int Padding { get; set; } = 1;

		public bool IndentLineBreaks { get; set; } = true;

		int _colCount = -1;

		int[] _colPaddings; // remains null if never set

		List<string> _currRow; // remains null if never set = new List<string>();

		List<string[]> _lines = new List<string[]>();



		public bool IsFixedColumnSize => _colPaddings.NotNulle();

		public bool AutoNewRowsOnAdd { get; set; } //= true;

		public GridOutputBuilder() { }

		public GridOutputBuilder(int padding) => Padding = padding;

		public GridOutputBuilder(params int[] paddingPerColumn)
		{
			if (paddingPerColumn.IsNulle())
				throw new ArgumentNullException();
			_colPaddings = paddingPerColumn;
			_colCount = paddingPerColumn.Length;
		}

		public GridOutputBuilder Add(object item)
		{
			string itm = item?.ToString();

			if (_currRow == null)
				_currRow = new List<string>();
			else if (_currRow.Count >= _colCount) // should NEVER happen, because see below, we check and call AddRow, and this is the only public add method
				throw new ArgumentOutOfRangeException(); // $"Row is already filled, cannot add another item to it. Make sure to call {nameof(EndRow)} after a row is filled"); //, or to enable {nameof(AutoNewRowsOnAdd)}");

			_currRow.Add(itm);

			if (_currRow.Count == _colCount) {
				string[] arr = _currRow.ToArray();
				_currRow.Clear();
				_AddRow(arr);
			}

			//AddRow(
			//	_currRow.Select(s => s as object).ToArray()); // this is kindof messed, paying price of params interprets this type as a single object

			return this;
		}

		/// <summary>
		/// Ends current row if one has been started (see Add methods that allow items to be added 
		/// via multiple calls for a single row). Is harmless when called before a row has been started.
		/// </summary>
		public void EndRow()
		{
			if (_currRow.NotNulle()) {
				if (_colCount < 0)
					_colCount = _currRow.Count;
				AddRow(_currRow);
				_currRow.Clear();
			}
		}

		public GridOutputBuilder AddRow(params object[] items)
			=> _AddRow(items?.Select(obj => obj?.ToString()).ToArray());

		public GridOutputBuilder _AddRow(string[] items)
		{
			if (items.IsNulle())
				throw new ArgumentNullException();
			if (_currRow.NotNulle())
				throw new Exception("Internal invalid, current row should be empty or null");

			_assertRowLength(items.Length);
			_lines.Add(items);

			return this;
		}

		void _setRowLength(int rowLength)
		{
			if (_colCount < 0)
				_colCount = rowLength;
			else if (_colCount != rowLength)
				throw new ArgumentOutOfRangeException();
		}

		void _assertRowLength(int rowLength)
		{
			if (_colCount < 0)
				_colCount = rowLength;
			else if (_colCount != rowLength)
				throw new ArgumentOutOfRangeException();
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
			_colCount = -1;
			_lines.Clear();
			return this;
		}



		/// <summary>
		/// Converts the input List of string arrays - where the outer List represents lines, and the inner 
		/// collection represents individual grid items) to a equal columned grid output string. 
		/// Each item in each line will be padded with the input padding size. Each array 
		/// must contain the same number of elements (i.e. grid items - they CAN be null or empty, but need to all
		/// be of the same size).
		/// </summary>
		/// <param name="lines">A list of string arrays, where each array in the list represents a row in the output grid.</param>
		/// <param name="padding">Additional padding between each element (default = 1)</param>
		/// <param name="indentLineBreaks">True to have any line breaks within values to also be indented</param>
		public static string ToGridOutputAuto(
			int padding, 
			List<string[]> lines,
			bool indentLineBreaks)
		{
			int numLines = lines[0].Length;
			var maxValues = new int[numLines];
			var lbPads = indentLineBreaks ? new int[numLines] : null;

			for (int i = 0; i < numLines; i++) {
				maxValues[i] = lines.Max(x =>
					(x.Length > i + 1 && 
					 x[i] != null ? x[i].Length : 0)) 
					 + padding;
			}

			if (indentLineBreaks) {
				for (int i = 1; i < numLines; i++) {
					lbPads[i] = maxValues.Take(i + 1).Sum();
				}
			}

			var sb = new StringBuilder();

			for (int i = 0; i < lines.Count; i++) {

				string[] arr = lines[i];

				if (i != 0)
					sb.AppendLine();

				for (int j = 0; j < arr.Length; j++) {
					string value = arr[j] ?? "";
					int totalWidth = maxValues[j];

					string paddedVal = value.PadRight(totalWidth);

					if (indentLineBreaks && j > 0 && value.IndexOf('\n') > 0) {
						paddedVal = paddedVal.Replace("\n", "\n" + new string(' ', lbPads[j]));
					}

					sb.Append(paddedVal);
				}
			}
			return sb.ToString();
		}

		public static string ToGridOutputFixed(int[] paddingPerColumn, List<string[]> lines)
		{
			if (paddingPerColumn.IsNulle())
				throw new ArgumentOutOfRangeException();

			int columnsLen = paddingPerColumn.Length;

			var sb = new StringBuilder();

			for (int i = 0; i < lines.Count; i++) {

				string[] arr = lines[i];
				if (arr.Length != columnsLen)
					throw new ArgumentOutOfRangeException();

				if (i != 0)
					sb.AppendLine();

				for (int j = 0; j < columnsLen; j++) {
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
			if (_lines.IsNulle())
				return null;

			if (IsFixedColumnSize)
				return ToGridOutputFixed(_colPaddings, _lines);
			else
				return ToGridOutputAuto(Padding, _lines, IndentLineBreaks);
		}

	}
}
