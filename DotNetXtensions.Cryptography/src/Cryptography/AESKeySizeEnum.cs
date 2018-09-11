namespace DotNetXtensions.Cryptography
{
	/// <summary>
	/// An enumeration representing the bit-length of the key to be used in AES encryption.
	/// </summary>
	public enum AESKeySize
	{
		/// <summary>
		/// A 128 bit length key (equivalent to 16 bytes).
		/// </summary>
		_128Bit,
		/// <summary>
		/// A 192 bit length key (equivalent to 24 bytes).
		/// </summary>
		_192Bit,
		/// <summary>
		/// A 256 bit length key (equivalent to 32 bytes).
		/// </summary>
		_256Bit
	}
}
