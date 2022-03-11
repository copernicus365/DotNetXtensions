namespace DotNetXtensions.Collections
{
	/// <summary>
	/// A struct that allows the result of a failed binary search 
	/// (such as is produced by .NET's Array.BinarySearch) to be 
	/// easily handled, specifically by indicating the low and high 
	/// bounds (never more than one apart, but equivalent to each other
	/// at the sequence boundaries) where the binary search's queried value index
	/// would be expected. There are some unexpected difficulties in 
	/// trying to make use of the failed BinarySearch value, which 
	/// this struct simplifies.
	/// </summary>
	public struct BinarySearchFailedLowHigh
	{
		#region FIELDS (all readonly)

		/// <summary>
		/// The low index which, when OnEdgeLowHighAreSame == false, 
		/// will *always* be lesser than the originally sought value item.
		/// </summary>
		public readonly int Low;

		/// <summary>
		/// The high index which, when OnEdgeLowHighAreSame == false, 
		/// will *always* be greater than the originally sought value item.
		/// </summary>		
		public readonly int High;

		/// <summary>
		/// Results in *false* if constructor's failedNegativeReturn input param
		/// corresponds to an 
		/// index completely out of bounds when judged by constructor's 
		/// sequenceLength, or if either of these values were simply faulty
		/// (e.g. if failedNegativeReturn is not a negative number, which would
		/// indicate the search was not a failure).
		/// <para />
		/// One must always check this value is true before using the resulting 
		/// BinarySearchFailedLowHigh value.
		/// </summary>
		public readonly bool IsValid;

		#endregion

		#region PROPERTIES

		/// <summary>
		/// LowIndex == HighIndex (are equivalent) when the value 
		/// sought after was lesser than the first item in the sequence
		/// (== '-1' failedNegativeReturn) or greater (**by only one**)
		/// than the last item in the sequence. 
		/// <para />
		/// Thus WHEN TRUE, the sought after value would be expected to 
		/// be found either below the first item in the sequence 
		/// (in which case both LowIndex and HighIndex == 0) or after 
		/// the last (both equal sequenceLength - 1).
		/// </summary>
		public bool OnEdgeLowHighAreSame {
			get { return Low == High; }
		}

		#endregion

		#region CONSTRUCTOR

		/// <summary>
		/// BinarySearchFailedLowHigh constructor.
		/// </summary>
		/// <param name="failedNegativeReturn">The binary search's negative 
		/// number result.</param>
		/// <param name="sequenceLength">The length of the source sequence 
		/// is needed for when the value is at the boundaries of the sequence, 
		/// i.e. below the lowest or above the highest.</param>
		public BinarySearchFailedLowHigh(int failedNegativeReturn, int sequenceLength)
		{
			// struct, must always set 
			IsValid = true;

			// HighIndex ALWAYS will == this
			High = ~failedNegativeReturn; // value remains unless is out of bounds (on right)

			// Set LowIndex, although first if also resets HighIndex's value
			if(High == sequenceLength)
				Low = --High;
			else if(High == 0)
				Low = 0;
			else
				Low = High - 1;

			// validate that input params are in range, but do NOT throw exceptions, just 
			// set IsValid to FALSE
			if(sequenceLength < 1 || failedNegativeReturn >= 0 || High >= sequenceLength || Low < 0) {
				IsValid = false;
				Low = -1;
				High = -1;
			}
		}

		#endregion

		#region OTHER

		/// <summary>
		/// ToString overriden for easy viewing in debugging.
		/// </summary>
		public override string ToString()
		{
			return string.Format("Low:{0}   High:{1}   IsValid:{2}  Same:{3}",
				Low, High, IsValid, OnEdgeLowHighAreSame);
		}

		#endregion
	}
}
