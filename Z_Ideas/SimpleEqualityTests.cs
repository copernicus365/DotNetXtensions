using System.Collections.Generic;
using System.Linq;

using DotNetXtensions;

using Xunit;

namespace DotNetXtensions.Test
{
	public class SimpleEqualityTests : XUnitTestBase
	{
		[Fact]
		public void Test_NullArray()
		{
			var p1 = new PayloadFoo1() {
				Age = 201,
				LastName = "Hi",
				Id = 888
			};
			var p2 = p1.Copy();

			bool isEqual = p1.Equals(p2);
			True(isEqual);
		}

		[Fact]
		public void Test_EmptyArray()
		{
			var p1 = new PayloadFoo1() {
				Age = 201,
				LastName = "Hi",
				Id = 888,
				Names = new string[] { },
			};
			var p2 = p1.Copy();

			bool isEqual = p1.Equals(p2);
			True(isEqual);
		}

		[Fact]
		public void Test_SuccessWithArray()
		{
			var p1 = new PayloadFoo1() {
				Age = 201,
				LastName = "Hi",
				Id = 888,
				Names = new string[] { "Bob", "Cynthia" },
			};
			var p2 = p1.Copy();

			bool isEqual = p1.Equals(p2);
			True(isEqual);
		}

		[Fact]
		public void Test_FailWithArray()
		{
			var p1 = new PayloadFoo1() {
				Age = 201,
				LastName = "Hi",
				Id = 888,
				Names = new string[] { "Bob", "Cynthia" },
			};
			var p2 = p1.Copy();
			p2.Names[0] = "Robert";

			bool isEqual = p1.Equals(p2);
			True(!isEqual);
		}

		[Fact]
		public void Test_NotEqual1()
		{
			var p1 = new PayloadFoo1() {
				Age = 201,
				LastName = "Hi",
				Id = 888
			};
			var p2 = p1.Copy();

			bool isEqual = p1.Equals(p2);
			True(isEqual);
		}

		public class PayloadFoo1 : SimpleEquality<PayloadFoo1>
		{
			public int Id { get; set; }

			public string[] Names { get; set; }

			public int Age { get; set; }

			public string LastName { get; set; }

			public override bool Equals(PayloadFoo1 other) => IsEqual(this, other);

			public override IEnumerable<object> GetEqualityItems()
			{
				yield return Id;
				yield return Age;
				yield return LastName;

				if(Names != null) {
					foreach(var p in Names)
						yield return p;
				}
			}

			public PayloadFoo1 Copy()
			{
				var p = (PayloadFoo1)MemberwiseClone();
				p.Names = p.Names?.ToArray();
				return p;
			}
		}
	}
}
