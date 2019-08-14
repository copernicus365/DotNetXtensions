using System;
using System.Threading.Tasks;

namespace DotNetXtensions.Timers
{
	/// <summary>
	/// (Notes: idea at this time is a simpler timer job, and an asynchronous one.
	/// Built off of the core of TimerJobAsync2, with some simplifications).
	/// </summary>
	public class SimpleTimerJob
	{
		public readonly TimeSpan Interval;
		public string Id { get; set; }

		public Action Action {
			get => _Action;
			set => _Action = value ?? throw new ArgumentNullException();
		}
		Action _Action;

		bool _stop;

		public Action<Exception, string> RegisterException { get; set; }

		public static Action<Exception, string> RegisterExceptionStatic { get; set; }

		public SimpleTimerJob() { }

		public SimpleTimerJob(
			TimeSpan interval,
			Action action = null,
			string id = null)
		{
			if(interval < TimeSpan.FromMilliseconds(1))
				throw new ArgumentOutOfRangeException("Interval must be greater or equal to 1 millisecond.");
			Interval = interval;
			if(action != null)
				Action = action;
			Id = id;
		}

		public bool IsStopped => _stop;

		public void Stop() => _stop = true;

		public SimpleTimerJob Start()
		{
			if(!_stop) {
				_stop = false;
				Task.Run(_RunLoopAsync);
			}
			return this;
		}

		async Task _RunLoopAsync()
		{
			await Task.Delay(Interval);

			while(!_stop) {
				try {
					Action();
				}
				catch(Exception ex) {
					(RegisterException ?? RegisterExceptionStatic)?.Invoke(ex, Id);
					await Task.Delay(TimeSpan.FromMinutes(1));
				}

				if(_stop)
					break;

				await Task.Delay(Interval);
			}
		}

	}
}
