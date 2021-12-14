using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Caching.Distributed;

namespace DotNetXtensions.Cache
{
	public interface IDCache<T, TId>
	{
		string CacheKeyPrefix { get; }
		DistributedCacheEntryOptions CacheSetOptions { get; }
		JsonSerializerOptions JsonSerializeOptions { get; set; }
		CacheGetSrc LastCacheSrc { get; }

		bool MemoryCacheIsEnabled { get; }

		bool ExistsInMemoryCache(TId id);

		string GetCacheKey(TId id);

		void INIT(IDistributedCache dcache, string cacheKeyPrefix, DistributedCacheEntryOptions cacheSetOptions, TimeSpan? inMemoryCacheExpirationTime = null);

		Task<T> GetAsync(TId id, bool skipMemoryCache = false, CancellationToken token = default);
		Task RefreshAsync(TId id, CancellationToken token = default);
		Task RemoveAsync(TId id, CancellationToken token = default);
		Task SetAsync(TId id, T item, DistributedCacheEntryOptions options = null, CancellationToken token = default);

	}
}