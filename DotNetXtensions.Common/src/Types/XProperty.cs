using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DotNetXtensions.Types
{
	public class XProperty<TValue> //where TValue : IEquatable<TValue> //, IEqualityComparer<TValue>
	{
		public Func<TValue> get;
		public Action<TValue> set;

		public XProperty(Func<TValue> get, Action<TValue> set)
		{
			this.get = get;
			this.set = set;
		}

		public TValue Obj
		{
			get { return get(); }
			set { set(value); }
		}

	}

	public class StringProp : XProperty<string> { public StringProp(Func<string> get, Action<string> set) : base(get, set) { } }
	public class DateTimeProp : XProperty<DateTime> { public DateTimeProp(Func<DateTime> get, Action<DateTime> set) : base(get, set) { } }
	public class IntProp : XProperty<int> { public IntProp(Func<int> get, Action<int> set) : base(get, set) { } }
	public class BoolProp : XProperty<bool> { public BoolProp(Func<bool> get, Action<bool> set) : base(get, set) { } }
	public class DateTimeNProp : XProperty<DateTime?> { public DateTimeNProp(Func<DateTime?> get, Action<DateTime?> set) : base(get, set) { } }
	public class IntNProp : XProperty<int?> { public IntNProp(Func<int?> get, Action<int?> set) : base(get, set) { } }
	public class BoolNProp : XProperty<bool?> { public BoolNProp(Func<bool?> get, Action<bool?> set) : base(get, set) { } }

}
