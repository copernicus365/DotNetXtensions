using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetXtensions
{
	/// <summary>
	/// Provides a pump that supports running asynchronous methods on the current thread.
	/// 
	/// From Stephen Toub, http://blogs.msdn.com/b/pfxteam/archive/2012/01/20/10259049.aspx.
	/// 
	/// Referenced as influential to the AsyncHelpers code here:
	/// http://stackoverflow.com/questions/5095183/how-would-i-run-an-async-taskt-method-synchronously
	/// 
	/// Discussion: http://stackoverflow.com/questions/26941901/task-continuation-was-not-scheduled-on-thread-pool-thread/26942338#26942338
	/// 
	/// <code>
	/// static void Main() {
	/// 	DemoAsync().Wait();
	/// }
	/// 
	/// // becomes:
	/// 
	/// static void Main()
	/// {
	/// 	 AsyncPump.Run(async delegate {
	/// 		  await DemoAsync();
	/// 	 });
	/// }
	/// </code>
	/// </summary>
	public static class AsyncPump
	{
		/// <summary>
		/// Runs the specified asynchronous function.
		/// </summary>
		/// <param name="func">The asynchronous function to execute.</param>
		public static void Run(Func<Task> func)
		{
			if (func == null) throw new ArgumentNullException("func");

			var prevCtx = SynchronizationContext.Current;
			try {
				// Establish the new context
				var syncCtx = new SingleThreadSynchronizationContext();
				SynchronizationContext.SetSynchronizationContext(syncCtx);

				// Invoke the function and alert the context to when it completes
				var t = func();
				if (t == null) throw new InvalidOperationException("No task provided.");
				t.ContinueWith(delegate { syncCtx.Complete(); }, TaskScheduler.Default);

				// Pump continuations and propagate any exceptions
				syncCtx.RunOnCurrentThread();
				t.GetAwaiter().GetResult();
			} finally { SynchronizationContext.SetSynchronizationContext(prevCtx); }
		}

		/// <summary>
		/// Provides a SynchronizationContext that's single-threaded.
		/// </summary>
		private sealed class SingleThreadSynchronizationContext : SynchronizationContext, IDisposable
		{
			/// <summary>The queue of work items.</summary>
			private readonly BlockingCollection<KeyValuePair<SendOrPostCallback, object>> m_queue =
				 new BlockingCollection<KeyValuePair<SendOrPostCallback, object>>();
			
			/// <summary>The processing thread.</summary>
			private readonly Thread m_thread = Thread.CurrentThread;

			/// <summary>
			/// Dispatches an asynchronous message to the synchronization context.
			/// </summary>
			/// <param name="d">The System.Threading.SendOrPostCallback delegate to call.</param>
			/// <param name="state">The object passed to the delegate.</param>
			public override void Post(SendOrPostCallback d, object state)
			{
				if (d == null) throw new ArgumentNullException("d");
				m_queue.Add(new KeyValuePair<SendOrPostCallback, object>(d, state));
			}

			/// <summary>Not supported.</summary>
			public override void Send(SendOrPostCallback d, object state)
			{
				throw new NotSupportedException("Synchronously sending is not supported.");
			}

			/// <summary>Runs an loop to process all queued work items.</summary>
			public void RunOnCurrentThread()
			{
				foreach (var workItem in m_queue.GetConsumingEnumerable())
					workItem.Key(workItem.Value);
			}

			/// <summary>Notifies the context that no more work will arrive.</summary>
			public void Complete() { m_queue.CompleteAdding(); }

			public void Dispose()
			{
				if(m_queue != null)
					m_queue.Dispose();
			}
		}
	}
}