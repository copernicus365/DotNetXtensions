# DotNetXtensions

DotNetXtensions is a general purpose toolbox of helpful .NET functions, extensions methods, and types. See corresponding [nugets](https://www.nuget.org/packages?q=DotNetXtensions).

## Per project documentation
(Just a brief sampling of documentation for just a few of the projects follows for the moment)

### DotNetXtensions 
The core project, which is mostly made of extension methods on common .NET types, while trying to avoid any too specialized use cases.

See [XString](https://github.com/copernicus365/DotNetXtensions/blob/master/DotNetXtensions/src/XString.cs) for example, with its many helpful string extension methods, such as the following (note that the default behavior for these kind of core extension methods is that they *are* null tolerant):

* `str.SubstringMax(12)` // if str.Length >= 12, return str, else take a substring of first 12 chars. This helps because `string.Substring` of course throws an exception if your maxLength is over `str.Length`. Without these you would need a couple extra lines every time 
* `str.NullIfEmpty()` // Return null if the string is already empty ("")
* `str.TrimIfNeeded()` // highly performant, returns same string if didn't need trimmed, no unnecessary allocs!!
* `str.NullIfEmptyTrimmed()` (combines TrimIfNeeded and NullIfEmpty)
* `str.StartsWithIgnoreWhite()` // string.StartsWith that ignores initial whitespace

### DotNetXtensions.Cryptography 

Contains both encryption and compression members, providing quick and easy extension methods on strings and byte arrays. For instance `str.GetSHA()` and `str.GetSHAString()` (with hex string result), `str.CompressToBase64()` (DeflateStream compression), and types like: 

* `CryptoRandom`
* `RandomStringGenerator`
* `RandomNumbers`

### DotNetXtensions.Net 

See for instance `XHttp.GetAsync` which returns a `HttpResponseInfo`. This allows one to easily make 304 NotModified requests, as well as to easily get back ETags and so forth (see parameter `HttpNotModifiedProperties settings`).

### DotNetXtensions.Json

General purpose Json functions built on Newtonsoft.Json.

## DNXPrivate Compilation symbol

Sometimes it is desirable to reference just one or more .cs code files to, for instance, use in a utility project, without necessarily wanting or needing to make a full nuget dependency, nor to expose those members, especially extension methods, beyond the internal use-case. This can be facilitated in that many of the extension method classes have a `DNXPrivate` compilation symbol. By including `DNXPrivate` as a compilation symbol in the host library, you can keep the referenced code files internal only.
