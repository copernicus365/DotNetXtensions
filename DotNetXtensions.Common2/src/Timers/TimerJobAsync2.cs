using System;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace DotNetXtensions.Timers
{
	public class TimerJobAsync2
	{
		// --- Fields / Properties ---

		public TimeSpan Interval { get; set; }
		public Func<Task> ExecuteFunc { get; set; }
		public bool RunJobOnTimerStart { get; set; }
		public string JobId { get; set; }
		public Action<Exception, string> RegisterException { get; set; }
		public static Action<Exception, string> RegisterExceptionStatic { get; set; }

		//CancellationTokenSource cancellation; // = new CancellationTokenSource(TimeSpan.FromMinutes(1));

		bool _stop;

		public TimerJobAsync2() { }

		public TimerJobAsync2(Func<Task> execute, TimeSpan interval, bool runJobOnTimerStart = false, string jobId = null)
		{
			ExecuteFunc = execute;
			Interval = interval;
			RunJobOnTimerStart = runJobOnTimerStart;
			JobId = jobId;
		}

		public void Stop()
		{
			_stop = true;
		}

		public TimerJobAsync2 Start(TimeSpan? firstRunDelay = null)
		{
			if (Interval < TimeSpan.FromMilliseconds(1))
				throw new ArgumentOutOfRangeException("Interval must be greater or equal to 1 millisecond.");

			_stop = false;
			Func<Task> ft = () => start_loop_async(firstRunDelay ?? TimeSpan.Zero);
			Task.Run(ft);
			return this;
		}

		async Task start_loop_async(TimeSpan firstRunDelay)
		{
			if (!RunJobOnTimerStart)
				await Task.Delay(Interval);

			if (firstRunDelay > TimeSpan.Zero)
				await Task.Delay(firstRunDelay);

			while (true) {
				if (_stop) break;

				if (ExecuteFunc != null) {
					try {
						Task exTask = ExecuteFunc();
						if (exTask != null)
							await exTask;
					}
					catch (Exception ex) {
						if (ex != null) {
							var exReg = RegisterException ?? RegisterExceptionStatic;
							if (exReg != null)
								exReg(ex, JobId);
						}
					}
				}

				if (_stop) break;
				await Task.Delay(Interval);
			}

		}

	}
}
