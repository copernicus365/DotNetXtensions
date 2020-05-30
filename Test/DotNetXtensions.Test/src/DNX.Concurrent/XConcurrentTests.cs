using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

using DotNetXtensions;

using Xunit;

namespace DotNetXtensions.Test
{
	public class XConcurrentTests : DnxTestBase
	{
		[Fact]
		public async Task RunTasksSequentially1()
		{
			bool diagnostic = false;

			int[] inputSeq = Enumerable.Range(0, 21).ToArray();
			var queue = new ConcurrentQueue<(int val, int i)>();
			//var completeds = new List<(int val, int i)>(); // -- doesn't work!!! is not concurrent!...

			await inputSeq.ForEachAsyncConcurrent(5, async (int i, int val) => {

				if(diagnostic)
					$"START: {i}: Val: {val}".Print();

				await Task.Delay(1);
				queue.Enqueue((val, i)); // completeds.Add((val, i));

				if(diagnostic)
					$"DONE: {i}: Val: {val}".Print();
			});

			(int val, int i)[] allArr = queue.ToArray();
			int[] arr = allArr.Select(v => v.val).ToArray();
			int[] sorted = arr.OrderBy(v => v).ToArray();

			bool hadAll = sorted.SequenceEqual(inputSeq);
			bool isScrammbled = !arr.SequenceEqual(inputSeq);

			True(isScrammbled);
			True(hadAll);
		}


	}
}
