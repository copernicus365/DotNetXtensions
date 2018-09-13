using System;
using System.Threading.Tasks;
using System.Timers;

namespace DotNetXtensions.Timers
{
	public class TimerJobAsync : IDisposable
	{
		#region Fields / Properties

		Timer timer;
		Func<Task> execute;
		TimeSpan interval;
		bool isRunning;

		public Func<Task> ExecuteAction
		{
			get { return execute; }
			set { execute = value; }
		}

		public Action<Exception> RegisterException;

		public TimeSpan Interval
		{
			get { return interval; }
			set { interval = value; }
		}

		#endregion

		#region Constructors

		public TimerJobAsync() { }

		public TimerJobAsync(Func<Task> execute, TimeSpan interval, bool startImmediately = true, bool executeImmediately = false)
		{
			this.execute = execute;
			this.interval = interval;

			if (startImmediately)
				Start(executeImmediately);
		}

		#endregion

		public void Stop()
		{
			if (timer != null)
				timer.Stop();
		}

		public void ReStart()
		{
			if (timer != null)
				timer.Start();
			else
				Start();
		}

		// NOTES: currently unsure if this is right, not really blocking the execute thread

		public void Start(bool executeImmediately = false)
		{
			if (timer == null) {
				timer = new Timer();
				timer.Elapsed += (s, e) => TimerFired(s, e);
			}

			timer.Interval = interval.TotalMilliseconds;
			if (executeImmediately)
				TimerFired(this, null);
			timer.Start();
		}

		void TimerFired(object sender, ElapsedEventArgs e)
		{
			if (!isRunning) {
				isRunning = true;
				timer.Enabled = false;

				Task.Run(execute).ContinueWith(t => {
					if (RegisterException != null && t != null && t.Exception != null && t.Exception.InnerException != null) {
						// continue with MUST catch an exception, or will pull down entire app!
						try {
							RegisterException(t.Exception.InnerException);
						} catch { }
					}

					isRunning = false;
					timer.Enabled = true;
				});

			}
		}

		public void Dispose()
		{
			if (timer != null)
				timer.Dispose(); // timers must be disposed
		}

	}
}
