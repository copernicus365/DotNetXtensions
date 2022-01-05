using Newtonsoft.Json;

namespace DotNetXtensions.Test;

public class XJsonTests : DnxTestBase
{
	public class Foo
	{
		public string Name { get; set; }
		public int Age { get; set; }

		public Foo() { }

		public Foo(string name, int age)
		{
			Name = name;
			Age = age;
		}
	}

	[Fact]
	public void Test_JsonSingleQuote_NoIndent()
	{
		var foo = new Foo() { Name = "Skip", Age = 5 };


		string result = foo.ToJson(indent: false, camelCase: true, singleQuotes: true);

		string exp = "{'name':'Skip','age':5}";

		True(result == exp);
	}

	[Fact]
	public void Test_Json_SingleQuote_NoIndent111()
		=> jsonSingleQuote_Indent_Tests(false, true);

	[Fact]
	public void Test_Json_SingleQuote_Indent()
		=> jsonSingleQuote_Indent_Tests(true, true);

	[Fact]
	public void Test_Json_DblQuote_NoIndent()
		=> jsonSingleQuote_Indent_Tests(false, false);

	[Fact]
	public void Test_Json_DblQuote_Indent()
		=> jsonSingleQuote_Indent_Tests(true, false);


	[Fact]
	public void Test_NullAndDefValueHandling()
	{
		string json = "{'name':'Joey','age':13}".Replace('\'', '"');

		var foo1 = new Foo("Joey", 13);

		string jsonBk1 = foo1.ToJson(
			indent: false,
			camelCase: true,
			defValueHandling: DefaultValueHandling.Populate,
			nullValueHandling: NullValueHandling.Include);

		True(json == jsonBk1);
	}

	void jsonSingleQuote_Indent_Tests(bool indent, bool singlQ)
	{
		var foo = new Foo() { Name = "Skip", Age = 5 };

		string result = foo.ToJson(indent: indent, camelCase: true, singleQuotes: singlQ)
			.ToUnixLines();

		string exp = indent
			? @"{
	'name': 'Skip',
	'age': 5
}"
			: "{'name':'Skip','age':5}";

		if(!singlQ)
			exp = exp.Replace('\'', '"');

		True(result == exp);
	}

	[Fact]
	public void Test_NullAndDefValueHandling_AllSet()
	{
		string json = "{'name':'Joey','age':13}".Replace('\'', '"');

		var foo1 = new Foo("Joey", 13);

		string jsonBk1 = foo1.ToJson(
			indent: false,
			camelCase: true,
			defValueHandling: DefaultValueHandling.Populate,
			nullValueHandling: NullValueHandling.Include);

		True(json == jsonBk1);
	}

	[Fact]
	public void Test_NullValueHandling_includeDefaults_Include()
	{
		var obj = new Foo(null, 0);

		string json = obj.ToJson(indent: false, camelCase: true, includeDefaults: true);

		string jsonExpected = "{'name':null,'age':0}".Replace('\'', '"');
		True(jsonExpected == json);
	}

	[Fact]
	public void Test_NullValueHandling_includeDefaults_Ignore()
	{
		var obj = new Foo(null, 0);

		string json = obj.ToJson(indent: false, camelCase: true, includeDefaults: false);

		string jsonExpected = "{}";
		True(jsonExpected == json);
	}
}
