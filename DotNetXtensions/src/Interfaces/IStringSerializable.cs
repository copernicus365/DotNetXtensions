#if DNXPublic
namespace DotNetXtensions
#else
namespace DotNetXtensionsPrivate
#endif
{
	/// <summary>
	/// For types that need to custom serialize to and from a string and that can
	/// have a public parameterless constructor (new()), all you have to do is implement
	/// IStringSerializable on that type and this converter will suddenly work for 
	/// Json serialization. The nice thing about IStringSerializable is, if indeed your custom
	/// type already is serializing / deserializing to and from a string, then that logic can be
	/// baked quite apart from anything related to Json, or to a given Json library or other serializer.
	/// For instance, our "Colour" type (a struct) can implement this interface without necessarily now
	/// having anything to say or do with Json.
	/// </summary>
#if DNXPublic
	public
#endif
		interface IStringSerializable
	{
		string Serialize();

		object Deserialize(string serializedObj);
	}


	public interface IStringSerializable<T> : IStringSerializable
	{
		new T Deserialize(string serializedObj);
	}

}
