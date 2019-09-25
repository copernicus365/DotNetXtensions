using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetXtensions
{
	public static class XConcurrent
	{
		/// <summary>
		/// Concurrently executes async actions for each item of input sequence,
		/// with <paramref name="maxConcurrent"/> limiting the number of concurrent
		/// items executed at once (via SemaphoreSlim).
		/// </summary>
		/// <remarks>
		/// Code altered version of [Jay Shah's on stackexchange's codereview](https://codereview.stackexchange.com/a/194060/34549).
		/// </remarks>
		/// <typeparam name="T">T</typeparam>
		/// <param name="enumerable">Sequence</param>
		/// <param name="maxConcurrent">Maximum number of actions firing at once.</param>
		/// <param name="action">An async action to execute per item.
		/// First integer argument is an index.</param>
		public static async Task ForEachAsyncConcurrent<T>(
			this IEnumerable<T> enumerable,
			int maxConcurrent,
			Func<int, T, Task> action)
		{
			if(maxConcurrent < 1)
				throw new ArgumentOutOfRangeException(nameof(maxConcurrent));

			using(var semaphoreSlim = new SemaphoreSlim(
				maxConcurrent, maxConcurrent)) {

				var tasksWithThrottler = new List<Task>();
				int i = 0;

				foreach(var item in enumerable) {

					int j = i++;

					// Increment the number of currently running tasks and wait if they are more than limit.
					await semaphoreSlim.WaitAsync();

					tasksWithThrottler.Add(Task.Run(async () => {
						await action(j, item).ContinueWith(res => {
							// action is completed, so decrement the number of currently running tasks
							semaphoreSlim.Release();
						});
					}));
				}

				// Wait for all tasks to complete.
				await Task.WhenAll(tasksWithThrottler.ToArray());
			}
		}

		// --- Need to think through further if this one is really worth it ---
		// 
		///// <summary>
		///// Concurrently executes async actions for each item of input sequence.
		///// </summary>
		///// <typeparam name="T">T</typeparam>
		///// <param name="enumerable">Sequence</param>
		///// <param name="action">An async action to execute per item</param>
		//public static async Task ForEachAsyncConcurrent<T>(this IEnumerable<T> enumerable, Func<T, Task> action)
		//{
		//	await Task.WhenAll(enumerable.Select(item => action(item)));
		//}

	}
}
