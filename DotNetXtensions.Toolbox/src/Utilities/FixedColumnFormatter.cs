using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using DotNetXtensions;

namespace DotNetXtensions
{
	public class FixedColumnFormatter
	{
		#region --- PROPERTIES ---

		public string FormatString { get; private set; }

		public int[] Widths { get; private set; }

		public int Count { get { return Widths?.Length ?? 0; } }

		public bool CutExcessValues { get; set; }

		public bool PrintToConsole { get; set; } = true;

		#endregion

		#region --- INIT ---

		public FixedColumnFormatter Init(int width, int count)
		{
			int[] widths = Enumerable.Repeat(width, count).ToArray();
			return Init(widths);
		}

		public FixedColumnFormatter Init(int[] widths)
		{
			FormatString = GetAlignedFormatString(true, ref widths);
			Widths = widths;
			return this;
		}

		public FixedColumnFormatter Init(params object[] list)
		{
			return Init(list);
		}

		public FixedColumnFormatter Init(int paddedAdd = 0, int minColWidth = 1, params object[] list)
		{
			if(list.NotNulle()) {
				int[] widths = new int[list.Length];
				for(int i = 0; i < list.Length; i++) {
					string val = list[i]?.ToString();
					widths[i] = Math.Max(val.CountN() + paddedAdd, minColWidth);
				}
				FormatString = GetAlignedFormatString(true, ref widths);
				Widths = widths;
			}
			return this;
		}

		public static string GetAlignedFormatString(bool rightAligned, ref int[] widths)
		{
			if(widths.IsNulle())
				return null;

			StringBuilder sb = new StringBuilder(widths.Length * 7);

			if(!rightAligned)
				widths[0] = 0;
			else
				widths[widths.Length - 1] = 0;

			for(int i = 0; i < widths.Length; i++) {
				int width = widths[i];
				if(rightAligned && width > 0)
					width *= -1;

				sb.Append("{")
					.Append(i)
					.AppendIf(width != 0, ",", width)
					.Append("}");
			}

			string result = sb.ToString();
			return result;
		}

		#endregion

		public string Print(params object[] list)
		{
			return __ToFormattedString(list);
		}

		//public IEnumerable<string> ToFormattedString(IEnumerable<IEnumerable<object>> lists)
		//{
		//	return lists.Select(list => __ToFormattedString(list));
		//}

		string __ToFormattedString(IEnumerable<object> objList)
		{
			string[] list = objList?.Select(obj => obj?.ToString() ?? "").ToArray();

			if(Count < 1) {

			}

			if(list.LengthN() != Count)
				throw new ArgumentOutOfRangeException();

			if(CutExcessValues) {
				for(int i = 0; i < list.Length; i++) {
					int itemLen = list[i].CountN();
					int maxWidth = Widths[i];
					if(maxWidth > 0 && itemLen > maxWidth)
						list[i] = list[i].SubstringMax(maxWidth, tryBreakOnWord: false);
				}
			}

			string result = string.Format(FormatString, list);
			if(PrintToConsole)
				Console.WriteLine(result);

			return result;
		}

	}
}
