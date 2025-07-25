namespace DNX.Test;

public class DnxTestBase : ProjectPaths
{
	public DnxTestBase() : base("data") { }

	public static T[] tarray<T>(params T[] arr) => arr;

	//public static KeyValuePair<string, string>[] GetKVsPerLine(string kvInput, char separator = ':')
	//{
	//	var kvs = kvInput
	//		.SplitLines(trimLines: true, removeEmptyLines: true)
	//		.Select(ln => {
	//			var items = ln.Split(separator);
	//			if(items.Length != 2) throw new ArgumentException();
	//			return new KeyValuePair<string, string>(items[0], items[1]);
	//		})
	//		.ToArray();
	//	return kvs;
	//}

	//public static DateTime[] AddMinutesArr(DateTime dt, params double[] minsFromBase)
	//{
	//	return minsFromBase.E()
	//		.Select(diff => dt.AddMinutes(diff))
	//		.ToArray();
	//}
}
