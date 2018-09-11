
// --- Old Code ---

///// <summary>
///// Execute's an async Task T method which has a void return value synchronously.
///// From: http://stackoverflow.com/a/5097066/264031
///// </summary>
///// <param name="task">Task T method to execute</param>
//public static void RunSync(this Task task)
//{
//	var oldContext = SynchronizationContext.Current;
//	var synch = new ExclusiveSynchronizationContext();
//	SynchronizationContext.SetSynchronizationContext(synch);
//	synch.Post(async _ => {
//		try {
//			await task;
//		} catch (Exception e) {
//			synch.InnerException = e;
//			throw;
//		} finally {
//			synch.EndMessageLoop();
//		}
//	}, null);
//	synch.BeginMessageLoop();

//	SynchronizationContext.SetSynchronizationContext(oldContext);
//}

///// <summary>
///// Execute's an async Task T method which has a T return type synchronously.
///// From: http://stackoverflow.com/a/5097066/264031
///// </summary>
///// <typeparam name="T">Return Type</typeparam>
///// <param name="task">Task T method to execute</param>
//public static T RunSync<T>(this Task<T> task)
//{
//	var oldContext = SynchronizationContext.Current;
//	var synch = new ExclusiveSynchronizationContext();
//	SynchronizationContext.SetSynchronizationContext(synch);
//	T ret = default(T);
//	synch.Post(async _ => {
//		try {
//			ret = await task;
//		} catch (Exception e) {
//			synch.InnerException = e;
//			throw;
//		} finally {
//			synch.EndMessageLoop();
//		}
//	}, null);
//	synch.BeginMessageLoop();
//	SynchronizationContext.SetSynchronizationContext(oldContext);
//	return ret;
//}

//public static Exception Run(Func<Task> function)
//{
//	if (function != null) {
//		Exception ex = null;
//		Task.Run(() => {
//			try { function(); }
//			catch (Exception x) {
//				ex = x;
//			}
//		}).ConfigureAwait(continueOnCapturedContext: false);
//		return ex;
//	}
//	return null;
//}


//#region --- ExclusiveSynchronizationContext internal class ---

///// <summary>
///// Internal class, used for RuncSync methods, from 
///// http://stackoverflow.com/questions/5095183/how-would-i-run-an-async-taskt-method-synchronously
///// </summary>
//internal class ExclusiveSynchronizationContext : SynchronizationContext, IDisposable
//{
//	private bool done;
//	public Exception InnerException { get; set; }
//	readonly AutoResetEvent workItemsWaiting = new AutoResetEvent(false);
//	readonly Queue<Tuple<SendOrPostCallback, object>> items =
//		 new Queue<Tuple<SendOrPostCallback, object>>();

//	public override void Send(SendOrPostCallback d, object state)
//	{
//		throw new NotSupportedException("We cannot send to our same thread");
//	}

//	public override void Post(SendOrPostCallback d, object state)
//	{
//		lock (items) {
//			items.Enqueue(Tuple.Create(d, state));
//		}
//		workItemsWaiting.Set();
//	}

//	public void EndMessageLoop()
//	{
//		Post(_ => done = true, null);
//	}

//	public void BeginMessageLoop()
//	{
//		while (!done) {
//			Tuple<SendOrPostCallback, object> task = null;
//			lock (items) {
//				if (items.Count > 0) {
//					task = items.Dequeue();
//				}
//			}
//			if (task != null) {
//				task.Item1(task.Item2);
//				if (InnerException != null) // the method threw an exeption
//                   {
//					throw new AggregateException("AsyncHelpers.Run method threw an exception.", InnerException);
//				}
//			}
//			else {
//				workItemsWaiting.WaitOne();
//			}
//		}
//	}

//	public override SynchronizationContext CreateCopy()
//	{
//		return this;
//	}

//	public void Dispose()
//	{
//		if(workItemsWaiting != null)
//			workItemsWaiting.Dispose();
//	}
//}
