using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetXtensions.Concurrent
{
	public static class XConcurrent
	{
		public static async Task RunTasksSequentially<T>(
			this IEnumerable<T> seq,
			int concurrentTasks,
			Func<T, int, Task> func)
		{
			var semSlim = new SemaphoreSlim(initialCount: concurrentTasks.MinMax(1, 12));
			var tasks = new List<Task>();

			int index = 0;
			object lockObj = new object();

			foreach(T item in seq) {

				await semSlim.WaitAsync();

				tasks.Add(
					Task.Run(async () => {
						int i;
						lock(lockObj) {
							i = index++;
						}
						try {
							await func(item, i);
						}
						finally {
							semSlim.Release();
						}
					}));
			}
			await Task.WhenAll(tasks);
		}
	}
}
