using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace DotNetXtensions.Cryptography
{
	/// <summary>
	/// A cryptographic random number generator which implements <see cref="Random"/>, 
	/// and which internally uses <see cref="RNGCryptoServiceProvider"/>,
	/// and that uses an unsigned integer buffer (uint[]) to greatly expediate the performance of 
	/// getting random integers. 
	/// <para />
	/// Core idea concerning random generation is from Toub and Farkas, though their code had no cache and 
	/// no high performance tuning, and some of the algorithm they posted needlessly did arithmetic within 
	/// each iteration of the loop. Even so, their code was the basis for detecting when a min-max range
	/// generation may suffer from a small decrease in randomness
	/// (although it may be an edge case, i.e. when min-max range is huge, which probably is highly untypical).
	/// <para />
	/// Here are some excerpts from their commentary on this problem (citation from web archive):
	/// <para />
	/// [start citation]"The problem we just explained regarding the mod approach happens because some values in the target range 
	/// have the potential to be slightly favored over other values. We can fix this by detecting when such favoritism 
	/// would happen and, in those cases, just trying again. As we saw in the previous example, the favoritism 
	/// happens when the randomly selected value satisfies the following condition:
	/// <para />
	/// <code><![CDATA[RandomValue >= RandomRange - (RandomRange % TargetRange)]]></code>
	/// <para />
	/// In our previous example, the RandomRange is 256 and the TargetRange is 6; therefore, favoritism happens when the RandomValue is >= 252. For small target ranges, the chances of this happening are very small. For huge target ranges (such as a target range of size 2^31+1), the chances of this happening are no worse than 50 percent. Thus, we can generate a random value and check to see if it falls into this favoritism range. 
	/// [end citation]
	/// Toub and Farkas source: https://web.archive.org/web/20090304194122/http://msdn.microsoft.com:80/en-us/magazine/cc163367.aspx, 
	/// code source: https://web.archive.org/web/20090104020243/http://msdn.microsoft.com:80/en-us/magazine/cc164453.aspx?code=true&amp;level=root&amp;file=CryptoRandom.cs
	/// <para />
	/// See also following StackOverflow threads: https://stackoverflow.com/questions/6299197/rngcryptoserviceprovider-generate-number-in-a-range-faster-and-retain-distribu/47143701?noredirect=1#comment81241555_47143701
	/// </summary>
	public class CryptoRandom : Random
	{
		/// <summary>
		/// That's 4 ints, anything else not worth the effort.
		/// </summary>
		public const int MinBufferSize = 16;

		/// <summary>
		/// (Remark: We had this static, but need to error check setter that it's multiple of 4 / int)
		/// 
		/// Default is 256 bytes (32 ints). Note that the buffer size is *fixed* upon
		/// creation, and that for min-max, many randoms are wasted, so don't want to get
		/// too small.
		/// </summary>
		public const int DefBufferSize = 256; // (32 ints)

		const long con_2to32nd = (long)uint.MaxValue + 1;
		const double con_2to32nd_double = 1.0 + uint.MaxValue;

		public readonly int BufferSize;
		public readonly int BufferSizeInt;

		/// <summary>
		/// The *integer* position within the <see cref="_intBuffer"/>.
		/// It turns out thus far we've never needed the byte position, so 
		/// decided to just track it by int, the one we need anyways.
		/// </summary>
		int _intPos;
		byte[] _buffer;
		uint[] _intBuffer;
		RNGCryptoServiceProvider _rng;


		public CryptoRandom() : this(0) { }

		public CryptoRandom(int bufferSize)
		{
			const int _maxBuffSize = int.MaxValue / 16;

			if (bufferSize == 0)
				bufferSize = DefBufferSize;
			else if (bufferSize > _maxBuffSize)
				bufferSize = _maxBuffSize;

			BufferSize = Math.Max(bufferSize, MinBufferSize);

			BufferSizeInt = BufferSize / 4;

			_intPos = BufferSizeInt;

			_rng = new RNGCryptoServiceProvider();
			// don't set buffers till needed
		}


		public void ResetBuffer()
		{
			if (_buffer == null)
				_buffer = new byte[BufferSize];
			// else if (_buffer.Length != BufferSize) throw new Exception();

			_intPos = 0;
			_rng.GetBytes(_buffer);

			if (_intBuffer == null)
				_intBuffer = new uint[BufferSizeInt];

			Buffer.BlockCopy(_buffer, 0, _intBuffer, 0, BufferSize);
		}



		/// <summary>
		/// Returns the next random integer from cache, or if increment was out of bounds (see <see cref="BufferSizeInt"/>),
		/// resets the cache first (see <see cref="ResetBuffer"/>). Otherwise, does an immediate index within 
		/// the internal uint cache array. It's fair to say it is impossible for this to be any faster. 
		/// Furthermore, the interals of many other members in this class are greatly beautified by
		/// making simple calls to this function to get a next random number, instead of repeating the work of
		/// messy random byte array generation and integer conversions from them. All of that work is encapsulated 
		/// once within <see cref="ResetBuffer"/>.
		/// </summary>
		public uint NextInt()
		{
			_intPos++;
			if (_intPos >= BufferSizeInt)
				ResetBuffer();
			uint rand = _intBuffer[_intPos]; // for diagnostics, lets us see the copy first
			return rand;
		}

		public override int Next()
			=> (int)NextInt() & 0x7FFFFFFF; // remove sign bit

		public override double NextDouble()
			=> NextInt() / con_2to32nd_double;



		public override int Next(int max)
			=> Next(0, max);

		public override int Next(int min, int max)
		{
			if (min > max)
				throw new ArgumentOutOfRangeException(nameof(min));

			long diff = max - min;
			if (diff < 1)
				return min;

			long remainder = con_2to32nd % diff;
			long maxRandPlus = con_2to32nd - remainder;

			while (true) {
				uint rand = NextInt();
				if (rand < maxRandPlus) {
					int val = (int)(min + (rand % diff));
					return val;
				}
			}
		}



		public int[] Next(int[] array, int max)
			=> Next(array, 0, max);

		/// <summary>
		/// Fills the elements of the input array with random numbers.
		/// The same array is returned for convenience (reference is the same as input).
		/// </summary>
		/// <param name="array">The array to fill with random values.</param>
		/// <param name="min">The inclusive lower bound of the random number generated.</param>
		/// <param name="max">The exclusive upper bound of the random number generated.</param>
		public int[] Next(int[] array, int min, int max)
		{
			if (min > max)
				throw new ArgumentOutOfRangeException(nameof(min));

			if (array == null)
				throw new ArgumentNullException(nameof(array));

			int arrLen = array.Length;
			if (arrLen == 0)
				return array;

			if (min == max) {
				for (int i = 0; i < arrLen; i++) // I *think* this is correct. it is what we do for a single min-max int get
					array[i] = min;
				return array;
			}

			long diff = max - min;
			long remainder = con_2to32nd % diff;
			long maxRandPlus = con_2to32nd - remainder;

			for (int i = 0; i < arrLen;)  { // NOTE: NO increment here, increments when condition passes...
				uint rand = NextInt();
				if (rand < maxRandPlus) {
					int val = (int)(min + (rand % diff));
					array[i++] = val; // NOTE INCREMENET++!
				}
			}
			return array;
		}





		/// <summary>
		/// Gets random bytes from the internal <see cref="RNGCryptoServiceProvider"/>
		/// instance. We make this method available simply to fully implement the <see cref="Random"/> class,
		/// NO internal state of this instance (such as the internal buffer or buffer size) is relevant.
		/// </summary>
		public override void NextBytes(byte[] buffer)
		{
			if (buffer == null)
				throw new ArgumentNullException(nameof(buffer));
			if (buffer.Length > 0)
				_rng.GetBytes(buffer);
		}

		/// <summary>
		/// Fills the elements of the input array with random numbers. 
		/// The same array is returned for convenience (reference is the same as input).
		/// </summary>
		/// <param name="array">The array to fill with random values.</param>
		public uint[] Next(uint[] array)
		{
			if (array == null)
				throw new ArgumentNullException(nameof(array));

			int arrLen = array.Length;
			if (arrLen == 0)
				return array;

			for (int i = 0; i < arrLen; i++) {
				uint rand = NextInt();
				array[i] = rand;
			}
			return array;
		}

	}
}
