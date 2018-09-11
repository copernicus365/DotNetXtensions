
namespace DotNetXtensions
{
	/// <summary>
	/// Type that simply wraps a value (struct) type, enabling the value to be passed by reference.
	/// <para />
	/// Source adapted from solutions here: http://stackoverflow.com/questions/1434840/c-copy-one-bool-to-another-by-ref-not-val
	/// </summary>
	public class ValueWrapper<TValue> where TValue : struct
	{
		public TValue value;
			
		public TValue Value { get { return value; } set { this.value = value; } }

		public ValueWrapper() { }

		public ValueWrapper(TValue value)
		{
			this.value = value;
		}

		public static implicit operator TValue(ValueWrapper<TValue> wrapper)
		{
			if (wrapper == null)
				return default(TValue);
			return wrapper.Value;
		}

	}
}
