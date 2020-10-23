using System.Collections.Generic;
using System.Linq;

namespace DotNetXtensions
{
	/// <summary>
	/// Extension methods for Dictionary.
	/// </summary>
	public static partial class XDictionary
	{
		public static bool NoKey<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
		{
			if(dict == null) return false;
			bool noExist = !dict.ContainsKey(key);
			return noExist;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="dict">The source dictionary.</param>
		/// <param name="ignore">Values to ignore.</param>
		/// <returns></returns>
		public static Dictionary<TValue, TKey> ReverseDictionary<TKey, TValue>(
			this Dictionary<TKey, TValue> dict,
			IEnumerable<TKey> ignore = null) //bool firstInWins = false)
		{
			if(dict == null)
				return null;

			Dictionary<TKey, TKey> ignoreD = ignore != null ? ignore.ToDictionary(k => k) : null;
			if(ignoreD.IsNulle())
				ignoreD = null;

			bool chkIgnoreD = ignoreD != null;

			Dictionary<TValue, TKey> rdict = new Dictionary<TValue, TKey>(dict.Count);

			foreach(var kv in dict) {
				if(chkIgnoreD && ignoreD.ContainsKey(kv.Key))
					continue;

				//if (firstInWins && rdict.ContainsKey(kv.Value))
				//	continue;

				rdict.Add(kv.Value, kv.Key);
			}
			return rdict;
		}

	}
}
