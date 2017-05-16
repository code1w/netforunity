using Sfs2X.Core.Sockets;
using Sfs2X.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
namespace Sfs2X.Core
{
	public class ThreadManager
	{
		private bool running = false;
		private Thread inThread;
		private bool inHasQueuedItems = false;
		private Queue<Hashtable> inThreadQueue = new Queue<Hashtable>();
		private object inQueueLocker = new object();
		private Thread outThread;
		private bool outHasQueuedItems = false;
		private Queue<Hashtable> outThreadQueue = new Queue<Hashtable>();
		private object outQueueLocker = new object();
		private static void Sleep(int ms)
		{
			Thread.Sleep(ms);
		}
		private void InThread()
		{
			while (this.running)
			{
				ThreadManager.Sleep(5);
				if (this.inHasQueuedItems)
				{
					object obj = this.inQueueLocker;
					Monitor.Enter(obj);
					try
					{
						while (this.inThreadQueue.Count > 0)
						{
							Hashtable item = this.inThreadQueue.Dequeue();
							this.ProcessItem(item);
						}
						this.inHasQueuedItems = false;
					}
					finally
					{
						Monitor.Exit(obj);
					}
				}
			}
		}
		private void OutThread()
		{
			while (this.running)
			{
				ThreadManager.Sleep(5);
				if (this.outHasQueuedItems)
				{
					object obj = this.outQueueLocker;
					Monitor.Enter(obj);
					try
					{
						while (this.outThreadQueue.Count > 0)
						{
							Hashtable item = this.outThreadQueue.Dequeue();
							this.ProcessOutItem(item);
						}
						this.outHasQueuedItems = false;
					}
					finally
					{
						Monitor.Exit(obj);
					}
				}
			}
		}
		private void ProcessOutItem(Hashtable item)
		{
			object obj = item["callback"];
			WriteBinaryDataDelegate writeBinaryDataDelegate = obj as WriteBinaryDataDelegate;
			if (writeBinaryDataDelegate != null)
			{
				ByteArray binData = item["data"] as ByteArray;
				PacketHeader header = item["header"] as PacketHeader;
				bool udp = (bool)item["udp"];
				writeBinaryDataDelegate(header, binData, udp);
			}
		}
		private void ProcessItem(Hashtable item)
		{
			object obj = item["callback"];
			OnDataDelegate onDataDelegate = obj as OnDataDelegate;
			if (onDataDelegate != null)
			{
				byte[] msg = (byte[])item["data"];
				onDataDelegate(msg);
			}
			else
			{
				ParameterizedThreadStart parameterizedThreadStart = obj as ParameterizedThreadStart;
				if (parameterizedThreadStart != null)
				{
					parameterizedThreadStart(item);
				}
			}
		}
		public void Start()
		{
			if (!this.running)
			{
				this.running = true;
				if (this.inThread == null)
				{
					this.inThread = new Thread(new ThreadStart(this.InThread));
					this.inThread.IsBackground = true;
					this.inThread.Start();
				}
				if (this.outThread == null)
				{
					this.outThread = new Thread(new ThreadStart(this.OutThread));
					this.outThread.IsBackground = true;
					this.outThread.Start();
				}
			}
		}
		public void Stop()
		{
			Thread thread = new Thread(new ThreadStart(this.StopThread));
			thread.Start();
		}
		private void StopThread()
		{
			this.running = false;
			if (this.inThread != null)
			{
				this.inThread.Join();
			}
			if (this.outThread != null)
			{
				this.outThread.Join();
			}
			this.inThread = null;
			this.outThread = null;
		}
		public void EnqueueDataCall(OnDataDelegate callback, byte[] data)
		{
			Hashtable hashtable = new Hashtable();
			hashtable["callback"] = callback;
			hashtable["data"] = data;
			object obj = this.inQueueLocker;
			Monitor.Enter(obj);
			try
			{
				this.inThreadQueue.Enqueue(hashtable);
				this.inHasQueuedItems = true;
			}
			finally
			{
				Monitor.Exit(obj);
			}
		}
		public void EnqueueCustom(ParameterizedThreadStart callback, Hashtable data)
		{
			data["callback"] = callback;
			object obj = this.inQueueLocker;
			Monitor.Enter(obj);
			try
			{
				this.inThreadQueue.Enqueue(data);
				this.inHasQueuedItems = true;
			}
			finally
			{
				Monitor.Exit(obj);
			}
		}
		public void EnqueueSend(WriteBinaryDataDelegate callback, PacketHeader header, ByteArray data, bool udp)
		{
			Hashtable hashtable = new Hashtable();
			hashtable["callback"] = callback;
			hashtable["header"] = header;
			hashtable["data"] = data;
			hashtable["udp"] = udp;
			object obj = this.outQueueLocker;
			Monitor.Enter(obj);
			try
			{
				this.outThreadQueue.Enqueue(hashtable);
				this.outHasQueuedItems = true;
			}
			finally
			{
				Monitor.Exit(obj);
			}
		}
	}
}
