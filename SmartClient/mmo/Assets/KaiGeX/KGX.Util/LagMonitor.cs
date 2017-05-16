using KaiGeX.Requests;
using System;
using System.Collections.Generic;
using System.Timers;
namespace KaiGeX.Util
{
	public class LagMonitor
	{
		private int lastReqTime;
		private List<int> valueQueue;
		private int interval;
		private int queueSize;
		private Timer pollTimer;
		private KaiGeNet sfs;
		public bool IsRunning
		{
			get
			{
				return this.pollTimer.Enabled;
			}
		}
		public int LastPingTime
		{
			get
			{
				int result;
				if (this.valueQueue.Count > 0)
				{
					result = this.valueQueue[this.valueQueue.Count - 1];
				}
				else
				{
					result = 0;
				}
				return result;
			}
		}
		public int AveragePingTime
		{
			get
			{
				int result;
				if (this.valueQueue.Count == 0)
				{
					result = 0;
				}
				else
				{
					int num = 0;
					foreach (int current in this.valueQueue)
					{
						num += current;
					}
					result = num / this.valueQueue.Count;
				}
				return result;
			}
		}
		public LagMonitor(KaiGeNet sfs) : this(sfs, 4, 10)
		{
		}
		public LagMonitor(KaiGeNet sfs, int interval) : this(sfs, interval, 10)
		{
		}
		public LagMonitor(KaiGeNet sfs, int interval, int queueSize)
		{
			if (interval < 1)
			{
				interval = 1;
			}
			this.sfs = sfs;
			this.valueQueue = new List<int>();
			this.interval = interval;
			this.queueSize = queueSize;
			this.pollTimer = new Timer();
			this.pollTimer.Enabled = false;
			this.pollTimer.AutoReset = true;
			this.pollTimer.Elapsed += new ElapsedEventHandler(this.OnPollEvent);
			this.pollTimer.Interval = (double)(interval * 1000);
		}
		public void Start()
		{
			if (!this.IsRunning)
			{
				this.pollTimer.Start();
			}
		}
		public void Stop()
		{
			if (this.IsRunning)
			{
				this.pollTimer.Stop();
			}
		}
		public void Destroy()
		{
			this.Stop();
			this.pollTimer.Dispose();
			this.sfs = null;
		}
		public int OnPingPong()
		{
			int item = DateTime.Now.Millisecond - this.lastReqTime;
			if (this.valueQueue.Count >= this.queueSize)
			{
				this.valueQueue.RemoveAt(0);
			}
			this.valueQueue.Add(item);
			return this.AveragePingTime;
		}
		private void OnPollEvent(object source, ElapsedEventArgs e)
		{
			Console.WriteLine("********** Polling!!");
			this.lastReqTime = DateTime.Now.Millisecond;
			this.sfs.Send(new PingPongRequest());
		}
	}
}
