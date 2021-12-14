# DotNetXtensions.DCache

`DCache` is intended to be a simple typed version of `IDistributedCache`,
useful when getting or putting a single type from the cache of a given type `T`.
Thus behind the scenes this type handles serialization / deserialization of the raw
strings and bytes to and from the cache, allowing the caller to treat this almost as
as a simple in-memory dictionary. A second major purpose
of this type is that it allows *in-memory* caching to be employed (for a specified time),
by means of an internal `DotNetXtensions.CacheDictionary{TKey, TValue}` instance
(thus the dependency on nuget pkg `DotNetXtensions.Common`). This can be powerful in less
change-sensitive scenarios. In those cases it can remove the extra
latency of a cache call entirely, when within a certain window and when this (server etc) instance
had let's say just accessed the given item and it is still in memory.

## CONSIDERATIONS

This type makes a number of basic assumptions, that makes this type useful only when
the following considerations are acceptable:

1) This  encapsulates a single type. Nothing stops using multiple instances, but compared to
the full `IDistributedCache`, this type is limited to a single keyed type
access of items from the backing cache. E.g. to access a `CustomerInfo` type stored in the cache
to be saved / retrieved using a singular keyed prefix, namely `CacheKeyPrefix`. 

2) Serialization / deserialization of the type `T` is performed right now
by System.Text.Json.JsonSerializer (.NET's new json serializer). While a binary serializer may
be nice, the simplicity of this right now wins out. While you can set a custom
`System.Text.Json.JsonSerializerOptions` instance, if this json serializer doesn't
suffice for serializing the type `T`, then this won't work.

3) The key type `TId` must be able to serialize to a string for the cache
key by means of calling ToString on it. This limitation could be solved later, but is here for the moment.

4) This type is strongly bound (on purpose) to `IDistributedCache`,
and thus also to the `Microsoft.Extensions.Caching.Distributed` namespace, and
to the closely related `DistributedCacheEntryOptions` type. It also takes a
dependency on the internally used `CacheDictionary{TKey, TValue}` type from
