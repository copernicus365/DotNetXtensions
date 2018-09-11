# DotNetXtensions

DotNetXtensions is a general purpose toolbox of helpful .NET functions, extensions methods, and types.

See [XString](https://github.com/copernicus365/DotNetXtensions/blob/master/DotNetXtensions/src/XString.cs) for example, with its many helpful string extension methods, such as the following (note that the default behavior for these kind of core extension methods is that they *are* null tolerant):

* `str.SubstringMax(12)` // if str.Length >= 12, return str, else take a substring of first 12 chars. This helps because `string.Substring` of course throws an exception if your maxLength is over `str.Length`. Without these you would need a couple extra lines every time 
* `str.NullIfEmpty()` // Return null if the string is already empty ("")
* `str.TrimIfNeeded()` // highly performant, returns same string if didn't need trimmed, no unnecessary allocs!!
* `str.NullIfEmptyTrimmed()` (combines TrimIfNeeded and NullIfEmpty)
* `str.StartsWithIgnoreWhite()` // string.StartsWith that ignores initial whitespace

# Projects

Split up into multiple projects, you of course only need to reference which ones you want or need to.

### DotNetXtensions 
The core project, is mostly made of extension methods on common .NET types, while trying to avoid any too specialized use cases.

### DotNetXtensions.Cryptography 

Contains both encryption and compression members, providing quick and easy extension methods on strings and byte arrays. For instance `str.GetSHA()` and `str.GetSHAString()` (with hex string result), `str.CompressToBase64()` (DeflateStream compression), and types like: 

* `CryptoRandom`
* `RandomStringGenerator`
* `RandomNumbers`

### DotNetXtensions.Net 

See for instance `XHttp.GetAsync` which returns a `HttpResponseInfo`. This allows one to easily make 304 NotModified requests, as well as to easily get back ETags and so forth (see parameter `HttpNotModifiedProperties settings`).

### DotNetXtensions.Common 

Some less common types, for instance, a `GeoNames` type and enums for countries and US / Canada states, some `TimeZones` types, a `Colour` struct (adapted [from](https://github.com/sherif-elmetainy/DotnetGD/blob/master/src/CodeArt.DotnetGD/Color.cs)), and so forth.

### DotNetXtensions.Json

General purpose Json functions built on Newtonsoft.Json.

### DotNetXtensions.X2

X2 has a load of miscellaneous types. Often you may want just a cs file from these or two, or else you could reference the whole project.

### Note on IsNulle / NotNulle
Easily the extension method(s) I use the most have a controversial name: `IsNulle` and `NotNulle`, although there are related fuller names:  `IsNullOrEmpty` and `NotNullOrEmpty`. Example usage: 

* `str.IsNulle()` // checks if collection (or string, or dictionary) *is null or empty*
* `coll.NotNulle()` // checks if collection (or string, or dictionary) *is *not* null or empty*

I find that:

`if(name.IsNulle()) { ...` 

is far more readable at a glance than:

`if(string.IsNullOrEmpty(name)) { ...` 

This allows the variable (`name` in this case) to stay front and center as you look at the code, and the same goes with collections or dictionaries: `items.IsNulle()` rather than `items != null && items.Count > 0`. It is a shame that there is no such extension method or a native member of string, nor a LINQ extension method on collection and array types, because again, I find I use these incessantly, and to great benefit in code composition and readability. I'm sure though that some will not like the unconventional abbeviated naming scheme, and understanbly so, but again, the fuller name on these extensions is still available.

## DNXPublic Compilation symbol

These projects are all built with the conditional compilation symbol `DNXPublic`, and most of the types are defined with reference to that at the top as follows: 

```cs
#if DNXPublic
namespace DotNetXtensions
#else
namespace DotNetXtensionsPrivate
#endif
{
#if DNXPublic
	public
#endif
	static class XString
```

It's a bit ugly, but the purpose is to allow one to make a direct link to one of these files and not have that type (`XString` in this case) bleed through to any project that uses this library that made the link. This is especially important when, let's say your main project references DNX, and then you have a smaller helper library that does something (WidgetParserProject), which smaller project you would like to *not* have to have a full dependency on the DNX project. Without these, there would end up being a conflict between basically two XString types being defined. All you have to know is that this problem is already fixed because of this special compilation setting. Your own projects never have to worry about it or do anything. Sorry if that's a bit confusing, but I wanted to explain what that was all about.

