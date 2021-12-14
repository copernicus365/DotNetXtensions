using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

using DotNetXtensions.Collections;

using Microsoft.Extensions.Caching.Distributed;

namespace DotNetXtensions.Cache
{
	/// <summary>
	/// `DCache` is intended to be a simple typed version of <see cref="IDistributedCache"/>,
	/// useful when getting or putting a single type from the cache (type <typeparamref name="T"/>).
	/// Thus behind the scenes this type handles serialization / deserialization of the raw
	/// strings and bytes to and from the the cache, allowing the caller to treat this almost as
	/// as a simple in-memory dictionary. A second major purpose
	/// of this type is that it allows *in-memory* caching to be employed (for a specified time),
	/// by means of an internal <see cref="CacheDictionary{TKey, TValue}"/> instance.
	/// This can be powerful in less change-sensitive scenarios. In those cases it can remove the extra
	/// latency of a cache call entirely, when within a certain window and when this (server etc) instance
	/// had let's say just accessed the given item and it is still in memory.
	/// <para />
	/// CONSIDERATIONS:
	/// <para />
	/// This type makes a number of basic assumptions, that makes this type useful only when
	/// the following considerations are acceptable:
	/// <para />
	/// 1) This  encapsulates a single type. Nothing stops using multiple instances, but compared to
	/// the full <see cref="IDistributedCache"/>, this type is limited to a single keyed type
	/// access of items from the backing cache. E.g. to access a `CustomerInfo` type stored in the cache
	/// to be saved / retrieved using a singular keyed prefix, namely <see cref="CacheKeyPrefix"/>. 
	/// <para />
	/// 2) Serialization / deserialization of the type <typeparamref name="T"/> is performed right now
	/// by System.Text.Json.JsonSerializer (.NET's new json serializer). While a binary serializer may
	/// be nice, the simplicity of this right now wins out. While you can set a custom
	/// <see cref="System.Text.Json.JsonSerializerOptions"/> instance, if this json serializer doesn't
	/// suffice for serializing the type <typeparamref name="T"/>, then this won't work.
	/// <para />
	/// 3) The key type <typeparamref name="TId"/> must be able to serialize to a string for the cache
	/// key by means of calling ToString on it. This limitation could be solved later, but is here for the moment.
	/// <para />
	/// 4) This type is strongly bound (on purpose) to <see cref="IDistributedCache"/>,
	/// and thus also to the <see cref="Microsoft.Extensions.Caching.Distributed"/> namespace, and
	/// to the closely related <see cref="DistributedCacheEntryOptions"/> type. It also takes a
	/// dependency on the internally used <see cref="CacheDictionary{TKey, TValue}"/> type from
	/// <see cref="DotNetXtensions"/>.
	/// </summary>
	/// <typeparam name="T">Backing type</typeparam>
	/// <typeparam name="TId">Key type</typeparam>
	public class DCache<T, TId> : IDCache<T, TId>
	{
		IDistributedCache _dcache;

		public CacheDictionary<string, CacheData> MemCacheDict;



		public string CacheKeyPrefix { get; private set; }

		public DistributedCacheEntryOptions CacheSetOptions { get; private set; }

		public CacheGetSrc LastCacheSrc { get; private set; }

		public DCache() { }

		public DCache(
			IDistributedCache dcache,
			string cacheKeyPrefix,
			TimeSpan expiresAfter,
			TimeSpan? inMemoryCacheExpirationTime = null)
		{
			INIT(dcache, cacheKeyPrefix,
				  new DistributedCacheEntryOptions() { AbsoluteExpirationRelativeToNow = expiresAfter },
				  inMemoryCacheExpirationTime);
		}

		public DCache(
			IDistributedCache dcache,
			string cacheKeyPrefix,
			DistributedCacheEntryOptions cacheSetOptions,
			TimeSpan? inMemoryCacheExpirationTime = null)
		{
			INIT(dcache, cacheKeyPrefix, cacheSetOptions, inMemoryCacheExpirationTime);
		}

		/// <param name="dcache">IDistributedCache instance (typically a singleton).</param>
		/// <param name="cacheKeyPrefix">Cache key prefix. In the (rare) case of only wanting to use the item's id, input an empty string, null is not allowed.</param>
		/// <param name="cacheSetOptions">Default set options when setting a cache value.</param>
		/// <param name="inMemoryCacheExpirationTime">To use an internal memory cache, set this to any value greater than Zero.</param>
		public void INIT(
			IDistributedCache dcache,
			string cacheKeyPrefix,
			DistributedCacheEntryOptions cacheSetOptions,
			TimeSpan? inMemoryCacheExpirationTime = null)
		{
			_dcache = dcache ?? throw new ArgumentNullException(nameof(dcache));

			CacheKeyPrefix = cacheKeyPrefix; // allow

			CacheSetOptions = cacheSetOptions ?? throw new ArgumentNullException(nameof(cacheSetOptions));

			bool useInMemCache = inMemoryCacheExpirationTime > TimeSpan.Zero;

			MemCacheDict = !useInMemCache
				? null
				: new(expires: inMemoryCacheExpirationTime.Value) {
					RunPurgeTS = TimeSpan.FromMinutes(10)
				};
		}



		public bool MemoryCacheIsEnabled
			=> MemCacheDict != null;

		public string GetCacheKey(TId id)
			=> $"{CacheKeyPrefix}{id}";

		public bool ExistsInMemoryCache(TId id)
			=> MemCacheDict?.ContainsKey(GetCacheKey(id)) ?? false;



		public async Task<T> GetAsync(TId id, bool skipMemCache = false, CancellationToken token = default(CancellationToken))
		{
			string key = GetCacheKey(id);

			if(!skipMemCache && MemoryCacheIsEnabled && MemCacheDict.TryGetValue(key, out CacheData val)) {
				LastCacheSrc = CacheGetSrc.MemoryCache;
				return val.Item;
			}

			var itemSerializedJson = await _dcache.GetAsync(key, token);

			if(itemSerializedJson.IsNulle()) {
				LastCacheSrc = CacheGetSrc.None;
				return default;
			}

			LastCacheSrc = CacheGetSrc.DistributedCache;

			T item = Deserialize(itemSerializedJson);

			if(MemoryCacheIsEnabled)
				MemCacheDict[key] = new CacheData(DateTimeOffset.UtcNow, item);

			return item;
		}

		public async Task RefreshAsync(TId id, CancellationToken token = default(CancellationToken))
		{
			string key = GetCacheKey(id);

			await _dcache.RefreshAsync(key, token);
		}

		public async Task RemoveAsync(TId id, CancellationToken token = default(CancellationToken))
		{
			string key = GetCacheKey(id);

			await _dcache.RemoveAsync(key, token);
		}

		/// <param name="options">Set options, only required if different set options are needed from the default <see cref="CacheSetOptions"/>.</param>
		public async Task SetAsync(TId id, T item, DistributedCacheEntryOptions options = null, CancellationToken token = default(CancellationToken))
		{
			string json = Serialize(item);

			var binData = Encoding.UTF8.GetBytes(json);

			string key = GetCacheKey(id);

			var setOptions = options ?? CacheSetOptions;
			if(setOptions == null)
				throw new ArgumentNullException(nameof(options));

			await _dcache.SetAsync(key, binData, setOptions, token);

			if(MemoryCacheIsEnabled)
				MemCacheDict[key] = new CacheData(DateTimeOffset.UtcNow, item);
		}


		public virtual T Deserialize(byte[] json)
		{
			return JsonSerializer.Deserialize<T>(json, _serializerOpts);
		}

		public virtual string Serialize(T obj)
		{
			return JsonSerializer.Serialize(obj, _serializerOpts);
		}



		#region --- JsonSerializerOptions ---

		public JsonSerializerOptions JsonSerializeOptions { get; set; }

		JsonSerializerOptions _serializerOpts => JsonSerializeOptions ?? _DefJsonSerializeOpts;

		static JsonSerializerOptions _DefJsonSerializeOpts = new JsonSerializerOptions() {
			PropertyNamingPolicy = null,
			WriteIndented = false,
			AllowTrailingCommas = true,
			DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
			PropertyNameCaseInsensitive = true,
		};

		#endregion


		public record CacheData(DateTimeOffset CachedTime, T Item);

	}
}
