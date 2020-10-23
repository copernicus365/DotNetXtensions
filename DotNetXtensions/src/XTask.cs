using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetXtensions
{
	/// <summary>
	/// Task helpers.
	/// </summary>
	/// <remarks>
	/// *OLD* links (may be helpful still, but not up to date, but I don't want to delete these yet):
	/// Tutorial:
	/// http://blogs.msdn.com/b/pfxteam/archive/2012/01/20/10259049.aspx
	/// http://blogs.msdn.com/b/pfxteam/archive/2012/01/21/10259307.aspx
	/// http://blogs.msdn.com/b/pfxteam/archive/2012/02/02/10263555.aspx
	/// 
	/// On old RunSync way (commented out now):
	/// http://stackoverflow.com/questions/5095183/how-would-i-run-an-async-taskt-method-synchronously
	/// orig: http://social.msdn.microsoft.com/Forums/en/async/thread/163ef755-ff7b-4ea5-b226-bbe8ef5f4796
	/// </remarks>
	public static class XTask
	{
		public static T RunSync<T>(this Func<Task<T>> func)
		{
			T result = Task.Run(async () => await func()).GetAwaiter().GetResult();
			return result;
		}

		public static void RunSync(this Func<Task> func)
		{
			Task.Run(async () => await func()).GetAwaiter().GetResult();
		}

		/// <summary>
		/// (BETA, Unhappy with confidence level): When Task.Run needs to be 
		/// used to fire off a task / function and forget about it, this can be
		/// used as an alternative in that it doesn't give the 4014 warning.
		/// This simply calls Task.Run(function).ConfigureAwait(continueOnCapturedContext: false);
		/// 
		/// See discussion here: http://stackoverflow.com/questions/5613951/simplest-way-to-do-a-fire-and-forget-method-in-c-sharp-4-0
		/// </summary>
		/// <param name="function"></param>
		public static void RunAndForget(Func<Task> function)
		{
			if(function != null) {
				Task.Run(function).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		public static void RunAndForget<TResult>(Func<Task<TResult>> function)
		{
			if(function != null) {
				Task.Run(function).ConfigureAwait(continueOnCapturedContext: false);
			}

			//#pragma warning disable 4014

			//Task.Factory.StartNew(() => "hi1".Print()); //, TaskCreationOptions)
			//         task.Start();
			//         Task.Run(() => "hi2".Print()); //.ConfigureAwait(false);

			//#pragma warning restore 4014
		}

		/// <summary>
		/// Source: http://stackoverflow.com/a/22078975/264031
		/// </summary>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="task"></param>
		/// <param name="timeout"></param>
		public static async Task<TResult> TimeoutAfterAsync<TResult>(this Task<TResult> task, TimeSpan timeout)
		{
			var timeoutCancellationTokenSource = new CancellationTokenSource();

			Task completedTask = await Task.WhenAny(task, Task.Delay(timeout, timeoutCancellationTokenSource.Token));
			if(completedTask == task) {
				timeoutCancellationTokenSource.Cancel();
				return await task;  // Very important in order to propagate exceptions
			}
			else {
				throw new TimeoutException("The operation has timed out.");
			}
		}
	}
}
