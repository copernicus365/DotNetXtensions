# DotNetXtensions.Common

Provides some common types and functions, but which do not make the cut to be put in the root DotNetXtensions library.

Some of the types:

* `GeoNames` with two enums: `Country` (all world countries) and `State` (US and Canada states). `GeoNames` works with these and provides for instance dictionaries and lists which provide lookups (etc) of all country names with their abbreviations, the same for states, and so forth.
* `MimeTypes` - Types and functions for working with some of the most common mimetypes.
* `Colour` (struct) and `ColorRGB` - types for dealing with colors (the `Colour` type is a modified version of [Sherif Elmetain's](https://github.com/sherif-elmetainy/DotnetGD/blob/master/src/CodeArt.DotnetGD/Color.cs)).
* `TextFuncs` - A type containing many commonly needed text manipulation functions, all focused on high-performance. Examples: `ClearXmlTags`, `ClearEscapedXmlTags`, `ParseQueryString` (to key-values), `PascalToUnderscoreString`, `ConvertNCarriageReturnsToRN`, and so forth. 
* `Diacritics` - A type for dealing with diacritics in a highly performant manner
* `CsvFuncs`, basic type for dealing with CSVs.
* `TimeZones` - types that deal with converting to and from TZ time-zones, as the default .NET types do not have readily actionable TZ types to work with, this makes it much simpler. Among other things, this allows you to lookup the `System.TimeZoneInfo` by a TZ string identifier, and the reverse, to get the tz id by a `System.TimeZoneInfo`.
