using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetXtensions
{
	public abstract class ConcurrentTasksQueueHandler<T>
	{
		/// <summary>
		/// If a ConcurrentTasksRunner is not passed into one of the constructors
		/// for this type, a new one will be constructed with this semcount.
		/// </summary>
		public static int TaskRunnerDefaultSemCount = 12;


		DateTime lastActivity = DateTime.UtcNow;
		DateTime lastQueueLoop = DateTime.UtcNow;
		ConcurrentTasksRunner taskRunner;
		ConcurrentQueue<T> queue = new ConcurrentQueue<T>();


		public bool PrintTaskRunnerOutput { get; set; } = true;

		/// <summary>
		/// Setting to true will disable the continuously running while loops
		/// within this class (the two which are started by calling StartTasks)
		/// from running again. It will also set the TaskRunner instance to null.
		/// Any tasks that were already running or just starting
		/// up will not be affected though (i.e. there are no cancellations involved).
		/// </summary>
		public bool EndTaskLoops { get; set; }

		/// <summary>
		/// There are two sources which can provide items to the queue within this type:
		/// 1) Through calls to <see cref="AddToQueue"/>, and
		/// 2) Through implementation of <see cref="GetItemsFromExternalQueueAsync"/>.
		/// </summary>
		public readonly bool NoExternalQueueToCheck;

		/// <summary>
		/// For debug purposes, otherwise, this should not be done.
		/// </summary>
		public bool Debug_AwaitTasksAtLoopEnd { get; set; }

		public int SemCount { get { return taskRunner?.CurrentSemCount ?? -1; } }




		#region --- CONSTRUCTORS ---

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="taskRunnerSemCount">The number of concurrent tasks to run. 
		/// This value is fed into the constructor of a <see cref="ConcurrentTasksRunner"/>.</param>
		/// <param name="startTasksImmediately">True to immediately call <see cref="StartTasks"/> at end of constructor. </param>
		/// <param name="noExternalQueueToCheck">
		/// Sets the readonly field. If true, <see cref="__GetItemsFromExternalQueue_ContinuousLoop_Async"/> will never be run,
		/// and <see cref="GetItemsFromExternalQueueAsync"/> will never be called or used either.
		/// </param>
		public ConcurrentTasksQueueHandler(
			int? taskRunnerSemCount = null,
			bool startTasksImmediately = false,
			bool noExternalQueueToCheck = false)
			: this(startTasksImmediately, 
				  new ConcurrentTasksRunner((taskRunnerSemCount ?? TaskRunnerDefaultSemCount).Min(1)), 
				  noExternalQueueToCheck) { }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="startTasksImmediately">See overload.</param>
		/// <param name="tasksRunner">ConcurrentTasksRunner instance.</param>
		/// <param name="noExternalQueueToCheck">See overload.</param>
		public ConcurrentTasksQueueHandler(
			bool startTasksImmediately,
			ConcurrentTasksRunner tasksRunner,
			bool noExternalQueueToCheck = false)
		{
			$"UpdatesInbox Starting".Print();

			taskRunner = tasksRunner ?? new ConcurrentTasksRunner(TaskRunnerDefaultSemCount.Min(1));

			NoExternalQueueToCheck = noExternalQueueToCheck;

			if(startTasksImmediately)
				StartTasks();
		}


		/// <summary>
		/// This starts up two Tasks, one Task.Run that async awaits GetItemsFromExternalQueueAsync
		/// IF <see cref="NoExternalQueueToCheck"/> is false, and the other that runs the internal 
		/// <see cref="__HandleItemsFromLocalQueue_ContinuousLoop_Async"/>.
		/// <para/>
		/// This should only be ran once! Calling it more than once would cause that many more loops to be running, so we internally
		/// disallow this and will throw an exception. To restart, create a new instance of your type.
		/// </summary>
		public void StartTasks()
		{
			if(_startupRan == true)
				throw new Exception($"{nameof(StartTasks)} has already ran, it cannot be started again.");

			_startupRan = true;
			if(!NoExternalQueueToCheck)
				Task.Run(async () => await __GetItemsFromExternalQueue_ContinuousLoop_Async());
			Task.Run(async () => await __HandleItemsFromLocalQueue_ContinuousLoop_Async());
		}
		bool _startupRan;

		#endregion




		/// <summary>
		/// Should return an identifying string representation for this item,
		/// which will be used when printing out that this item is being handled, etc.
		/// </summary>
		/// <param name="itm">Item</param>
		public abstract string GetItemIdDisplay(T itm);

		public abstract Task HandleItemDequeue(T itm);

		/// <summary>
		/// Exception logger. The exception passed in will always not be the AggregateException.
		/// </summary>
		public abstract void LogException(Exception ex, string title);

		public abstract void PrintTrace(string val);

		async Task<T[]> _GetItemsFromMockOrExternalQueue()
		{
			if (MockGetItemsFromExternalQueue != null)
				return MockGetItemsFromExternalQueue();

			//if(GetItemsFromExternalQueue )
			T[] items = await GetItemsFromExternalQueueAsync();
			return items;
		}

		/// <summary>
		/// This method is called when <see cref="NoExternalQueueToCheck"/> is false (i.e. when there IS 
		/// an external queue or items source).
		/// In that case, as long as <see cref="StartTasks"/> has been called and has not been stopped,
		/// we will periodically call this type in order to get items from whatever the external source or
		/// queue is. In other words, we will be polling that external source, and this method once implemented 
		/// will provide those external items.
		/// <para/>
		/// Note that there is another way to add items to this type's internal queue, which is by calling 
		/// <see cref="AddToQueue(T[])"/>. Even when there is an external queue however, you can use that method 
		/// in case of mocking this type to see how it runs.
		/// </summary>
		public abstract Task<T[]> GetItemsFromExternalQueueAsync();

		///// <summary>
		///// This method is called when <see cref="NoExternalQueueToCheck"/> is false (i.e. when there IS 
		///// an external queue or items source).
		///// In that case, as long as <see cref="StartTasks"/> has been called and has not been stopped,
		///// we will periodically call this type in order to get items from whatever the external source or
		///// queue is. In other words, we will be polling that external source, and this method once implemented 
		///// will provide those external items.
		///// <para/>
		///// Note that there is another way to add items to this type's internal queue, which is by calling 
		///// <see cref="AddToQueue(T[])"/>. Even when there is an external queue however, you can use that method 
		///// in case of mocking this type to see how it runs.
		///// </summary>
		//public abstract async Task<T[]> GetItemsFromExternalQueueAsync();

		/// <summary>
		/// For mocking purposes, set this Func to non-null. It's logic will then replace 
		/// calls to <see cref="GetItemsFromExternalQueueAsync"/>, allowing mock items to be sent in 
		/// for test purposes. This is very useful as it allows one to by-pass the potentially difficult
		/// to test external queue. E.g. if an Azure queue is the external source, setting this will 
		/// allow you to test this without having to actually add and get items from that external 
		/// queue.
		/// </summary>
		public Func<T[]> MockGetItemsFromExternalQueue { get; set; }



		/// <summary>
		/// Add items manually to the internal queue. See notes on <see cref="GetItemsFromExternalQueueAsync"/> 
		/// for more information or understanding.
		/// </summary>
		/// <param name="items">Items to add.</param>
		public void AddToQueue(params T[] items)
		{
			if(items.NotNulle())
				foreach(var item in items)
					queue.Enqueue(item);
		}

		public abstract TimeSpan WaitTimeTillNextExternalQueueCheck(TimeSpan timeSinceLastCheck);




		public virtual TimeSpan DelayBetweenLocalQueueChecks { get; set; } = TimeSpan.FromSeconds(.5);


		/// <summary>
		/// This function is started in <see cref="StartTasks"/> after which time it
		/// runs continuously. After each wait period in the loop (defined by <see cref="DelayBetweenLocalQueueChecks"/>,
		/// typically no more than 1 second),
		/// the local queue which has had items dequeued to it has its items run through until the queue is empty.
		/// Then it waits again for the specified time before checking again.
		/// </summary>
		async Task __HandleItemsFromLocalQueue_ContinuousLoop_Async()
		{
			printTrace(2, $"------- Start Loop {nameof(__HandleItemsFromLocalQueue_ContinuousLoop_Async)} -------", 1);

#pragma warning disable CS4014
			while(EndTaskLoops == false) {

				await Task.Delay(DelayBetweenLocalQueueChecks);

				if(queue.Count > 0) {
					DateTime start = DateTime.Now;
					printTrace(2, $@"------- Start Handling Items Already Dequeued ({nameof(__HandleItemsFromLocalQueue_ContinuousLoop_Async)}) -------
Time:{start}", 1);

					List<Task> tasks = new List<Task>();
					int i = -1;
					while(queue.Count > 0 && EndTaskLoops == false) {
						int index = ++i; // must get local variable
						T _item;
						if(queue.TryDequeue(out _item) && _item != null) {

							// !!! MUST get local value! if outer 'item' gets set to null on next loop
							T itm = _item;
							string itemId = GetItemIdDisplay(itm);
							printTrace(2, $"[{index}] Dequeuing UpItem: {itemId}"); // {itm.Id}, {itm.UType}");

							Task task1 = taskRunner.RunTask(async () => {
								try { // try/cath here if you want it, *not* outside loop
									await HandleItemDequeue(itm);
								}
								catch(Exception ex) {								
									//ex.ToString().PrintTrace(cat, writeCatFirst: true, writeSemCount: true);
									__LogException(ex, $"Exception in {nameof(ConcurrentTasksQueueHandler<T>)}.{nameof(HandleItemDequeue)}");
								}
							},
								i: index, // had: index++, but why increment? is a local var set from i
								id: itemId, //$"{itm.UType}({itm.Id})",
								print: !PrintTaskRunnerOutput
									? (Action<string, bool>)null
									: (value, isFinished) => PrintTrace(value));

							tasks.Add(task1);
						}
					}

					if(Debug_AwaitTasksAtLoopEnd && tasks.NotNulle()) {
						printTrace(1, "--- Starting WaitAll ---", 1);
						try {
							Task.WaitAll(tasks.ToArray());
						}
						catch(Exception ex) {
							__LogException(ex, "UpdatesInbox.WaitAll");
						}
						printTrace(1, "--- Ending   WaitAll ---", 1);
					}
					printTrace(2, $@"------- End Dequeuing *{i}* Update Items {(Debug_AwaitTasksAtLoopEnd ? null : "(*not* awaiting all) ")}-------
Elapsed: {DateTime.Now - start}", 1);
				}
			}
#pragma warning restore CS4014
		}



		/// <summary>
		/// The main function of this method is to dequeue items from an external queue or otherwise source,
		/// and to add them to our internal queue. This does not actually perform the work of handling those dequeued
		/// items, which is taken care of by the other major function in this type (internal function: <see cref="__HandleItemsFromLocalQueue_ContinuousLoop_Async"/> ).
		/// Once called, this continuously gets items from <see cref="GetItemsFromExternalQueueAsync"/> and
		/// adds any of those to the internal queue. Any time the internal queue has more than the max allowed number of 
		/// concurrent items to act on (see <see cref="SemCount"/>), this cycle will wait (will keep looping but will
		/// only get items once there are less than max items already in local queue). The cycle herein Task.Delays
		/// between cycling according to <see cref="WaitTimeTillNextExternalQueueCheck"/>.
		/// </summary>
		async Task __GetItemsFromExternalQueue_ContinuousLoop_Async()
		{
			if(NoExternalQueueToCheck) // consider this as readonly once set at constructor
				return;

			int timesQueueCountSkipped = 0;

			while(EndTaskLoops == false) {

				TimeSpan timeSinceLastCheck = DateTime.UtcNow - lastActivity;
				TimeSpan waitTS = WaitTimeTillNextExternalQueueCheck(timeSinceLastCheck);

				await Task.Delay(waitTS);

				try {
					var change = DateTime.UtcNow - lastQueueLoop;

					if(change.TotalMinutes >= 1) {
						lastQueueLoop = DateTime.UtcNow;
						PrintTrace($"QCnt: {queue.Count}, Skips: {timesQueueCountSkipped}, SemCnt: {SemCount} ({DateTime.UtcNow.ToDateTimeStringEST()})");
					}

					int qCount = queue.Count;
					if(qCount < 1 || qCount < taskRunner.MaxSemCount) {
						timesQueueCountSkipped = 0;

						T[] items = await _GetItemsFromMockOrExternalQueue();

						if(items.NotNulle()) {
							lastActivity = DateTime.UtcNow;
							for(int i = 0; i < items.Length; i++) {
								var item = items[i];
								if(item != null)
									queue.Enqueue(item);
							}
						}
					}
					else
						timesQueueCountSkipped++;
				}
				catch(Exception ex) {
					__LogException(ex, "UpdatesInbox.Execute (Azure Queue)");
				}
			}
		}


		void __LogException(Exception ex, string title)
		{
			//! Step 1: If this is AggregateException, stop and call this same func with the inner exceptions
			if(ex != null) {
				var aggEx = ex as AggregateException;
				if(aggEx != null) {
					if(aggEx.InnerExceptions?.CountN() < 2)
						ex = aggEx.InnerException;
					else {
						foreach(var x in aggEx.InnerExceptions)
							if(x != null)
								__LogException(x, title);
						return;
					}
				}
			}

			//! Step 2: Ok, this is no longer an AggregateException

			if(ex != null && ex.InnerException != null)
				ex = ex.InnerException;

			title = $"{nameof(ConcurrentTasksQueueHandler<T>)}:{title}";

			PrintTrace($@"
--- Caught Exception: {title}
{ex}

");
			LogException(ex, title);
		}


		void printTrace(int newLines, string val, int linesAfter = 0)
		{
			val = string.Concat(Enumerable.Repeat("\r\n", Math.Max(newLines, 0))) + val + string.Concat(Enumerable.Repeat("\r\n", Math.Max(linesAfter, 0)));
			PrintTrace(val);
		}


	}
}
