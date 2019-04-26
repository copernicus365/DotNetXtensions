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
		string[] m_ExtraValues;

		public TZKeyValues() { }

		public TZKeyValues(string key, string value)
		{
			Key = key;
			Value = value;
		}

		public string Key { get; set; }

		public string Value { get; set; }

		/// <summary>
		/// This is the old logic: It basically is doing this:
		/// IF Value hasn't been set yet, AND if input array is not Nulle,
		/// then set Value to the first item in this array, and then set ExtraValues to
		/// any subsequent (greater than Length 1) values in input array if there are any
		/// (else ExtraValues will remain null). 
		/// OTHERWISE, i.e. if Value is already non-null, OR if input array is null or empty, 
		/// then just sets ExtraValues directly to this input array.
		/// </summary>
		/// <param name="value"></param>
		public void SetValues(string[] value)
		{
			// note (2019/04): This was the orig logic for the setter (from like 2012), 
			// I *don't* like some of this, but need to carefully go thru all calls before 
			// changing any of this logic. Pry will make this "SetValuesOrig" and Obsolete,
			// etc, but a lot of things hang on this, so not willing to change it all yet, 
			if (Value == null && value.NotNulle()) { // I particularly don't like 
				Value = value[0];
				if (value.Length > 1)
					m_ExtraValues = value.Skip(1).ToArray();
			}
			else
				m_ExtraValues = value;
		}

		public string[] ExtraValues
		{
			get => m_ExtraValues;
			//private set // note 2019/04: We set this to private, then removed the whole setter to SetValues (soon: SetValuesOrig)
			//{
			//	if (Value == null && value.NotNulle()) {
			//		Value = value[0];
			//		if (value.Length > 1)
			//			m_ExtraValues = value.Skip(1).ToArray();
			//	}
			//	else
			//		m_ExtraValues = value;
			//}
		}

		public override string ToString()
		{
			return string.Format("{0}, {1}", Key ?? "", Value ?? "");
		}

	}
}
