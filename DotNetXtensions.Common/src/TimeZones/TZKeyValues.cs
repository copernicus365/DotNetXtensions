using System.Linq;

namespace DotNetXtensions.Globalization
{
	/// <summary>
	/// Note from 2018-04: I'm not happy how this was done, a lot was done for 
	/// perf concerns, because most tz keys didn't have a multi-value, so 
	/// stuff was done to not use a collection when only the singular 
	/// Value property was set. However, if I had the time I would totally
	/// fix go through and fix this up. We could keep the perf concern but
	/// either: 1) keep the type readonly, then even as a struct, would be 
	/// way better and remove problems, or 2) make an ExtraValues property,
	/// instead of as is done where it looks like `Values` which serves some
	/// kind of seemingly messed up multi-purpose and I'm not sure correctly.
	/// </summary>
	public class TZKeyValues
	{
		string[] m_values;

		public TZKeyValues() { }

		public TZKeyValues(string key, string value)
		{
			Key = key;
			Value = value;
		}

		public string Key { get; set; }

		public string Value { get; set; }

		public string[] Values
		{
			get { return m_values; }
			set
			{
				if (Value == null && value != null && value.Length > 0) {
					Value = value[0];
					if (value.Length > 1)
						m_values = value.Skip(1).ToArray();
				}
				else
					m_values = value;
			}
		}

		public override string ToString()
		{
			return string.Format("{0}, {1}", Key ?? "", Value ?? "");
		}

	}
}
