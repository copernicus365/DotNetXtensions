using System.Diagnostics;

#if DNXPublic
namespace DotNetXtensions
#else
namespace DotNetXtensionsPrivate
#endif
{
	/// <summary>
	/// A simple class based KeyValue pair type. The chief benefit
	/// over the framework's KeyValuePair struct type is that Key and
	/// Value are not readonly with KV. (And as an aside, how much nicer
	/// it would have been if the framework's struct had been named
	/// with the short and tidy "KV" instead.)
	/// </summary>
	/// <typeparam name="TKey">Type T, no constraints.</typeparam>
	/// <typeparam name="TValue">Type Y, no constraints.</typeparam>
#if DNXPublic
	public
#endif
	class KV<TKey, TValue>
	{
		/// <summary>
		/// KV Key.
		/// </summary>
		public TKey Key { get; set; }

		/// <summary>
		/// KV Value.
		/// </summary>
		public TValue Value { get; set; }

		[DebuggerStepThrough]
		public KV() { }

		[DebuggerStepThrough]
		public KV(TKey key, TValue value)
		{
			Key = key;
			Value = value;
		}

		[DebuggerStepThrough]
		public override string ToString()
		{
			const int max = 120;
			return "[" + Key.ToStringN().SubstringMax(max) + ", " + Value.ToStringN().SubstringMax(max) + "]"; 
		}
	}

	/// <summary>
	/// A simple class based KeyValue pair type. The chief benefit
	/// over the framework's KeyValuePair struct type is that Key and
	/// Value are not readonly with KV. (And as an aside, how much nicer
	/// it would have been if the framework's struct had been named
	/// with the short and tidy "KV" instead.)
	/// </summary>
#if DNXPublic
	public
#endif
	class KV
	{
		/// <summary>
		/// KV Key.
		/// </summary>
		public object Key { get; set; }

		/// <summary>
		/// KV Value.
		/// </summary>
		public object Value { get; set; }

		public KV() { }

		public KV(object key, object value)
		{
			Key = key;
			Value = value;
		}

		public override string ToString()
		{
			const int max = 120;
			string key = Key?.ToString().E().SubstringMax(max);
			string val = Value?.ToString().E().SubstringMax(max);
			return $"[{key}, {val}]";
		}
	}
}
