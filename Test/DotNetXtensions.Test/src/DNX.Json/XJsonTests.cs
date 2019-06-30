using System;
using Xunit;
using DotNetXtensions;
using DotNetXtensions.Json;

namespace DotNetXtensions.Test
{
	public class XJsonTests : BaseUnitTest
	{
		public class Foo
		{
			public string Name { get; set; }
			public int Age { get; set; }
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


		void jsonSingleQuote_Indent_Tests(bool indent, bool singlQ)
		{
			var foo = new Foo() { Name = "Skip", Age = 5 };

			string result = foo.ToJson(indent: indent, camelCase: true, singleQuotes: singlQ);

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
	}
}
