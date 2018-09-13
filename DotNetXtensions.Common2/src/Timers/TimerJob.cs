using System;
using System.Timers;

namespace DotNetXtensions.Timers
{

	public class TimerJob : IDisposable
	{
		#region Fields / Properties

		Timer timer;
		Action execute;
		TimeSpan interval;
		bool isRunning;
		public Action ExecuteAction
		{
			get { return execute; }
			set { execute = value; }
		}

		public TimeSpan Interval
		{
			get { return interval; }
			set { interval = value; }
		}

		#endregion

		#region Constructors

		public TimerJob()
		{
		}

		public TimerJob(Action execute, TimeSpan interval, bool startImmediately = true, bool executeImmediately = false)
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

		public void Start(bool executeImmediately = false)
		{
			if (timer == null) {
				timer = new Timer();
				timer.Elapsed += timer_Elapsed;
			}

			timer.Interval = interval.TotalMilliseconds;
			if (executeImmediately)
				timer_Elapsed(this, null);
			timer.Start();
		}

		void timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			if (!isRunning) {
				try {
					isRunning = true;
					timer.Enabled = false;
					Execute();
				} finally {
					isRunning = false;
					timer.Enabled = true;
				}
			}
		}

		public void Execute()
		{
			execute(); //job.Execute();
			isRunning = false;
		}

		public void Dispose()
		{
			if (timer != null)
				timer.Dispose(); // timers must be disposed
		}
	}
}
