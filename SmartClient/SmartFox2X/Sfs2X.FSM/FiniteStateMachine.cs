using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
namespace Sfs2X.FSM
{
	public class FiniteStateMachine
	{
		public delegate void OnStateChangeDelegate(int fromStateName, int toStateName);
		private List<FSMState> states = new List<FSMState>();
		private volatile int currentStateName;
		public FiniteStateMachine.OnStateChangeDelegate onStateChange;
		private object locker = new object();
		public void AddState(object st)
		{
			int stateName = (int)st;
			FSMState fSMState = new FSMState();
			fSMState.SetStateName(stateName);
			this.states.Add(fSMState);
		}
		public void AddAllStates(Type statesEnumType)
		{
			IEnumerator enumerator = Enum.GetValues(statesEnumType).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object current = enumerator.Current;
					this.AddState(current);
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
		}
		public void AddStateTransition(object from, object to, object tr)
		{
			int num = (int)from;
			int outputState = (int)to;
			int transition = (int)tr;
			FSMState fSMState = this.FindStateObjByName(num);
			fSMState.AddTransition(transition, outputState);
		}
		public int ApplyTransition(object tr)
		{
			object obj = this.locker;
			Monitor.Enter(obj);
			int result;
			try
			{
				int transition = (int)tr;
				int num = this.currentStateName;
				this.currentStateName = this.FindStateObjByName(this.currentStateName).ApplyTransition(transition);
				if (num != this.currentStateName)
				{
					if (this.onStateChange != null)
					{
						this.onStateChange(num, this.currentStateName);
					}
				}
				result = this.currentStateName;
			}
			finally
			{
				Monitor.Exit(obj);
			}
			return result;
		}
		public int GetCurrentState()
		{
			object obj = this.locker;
			Monitor.Enter(obj);
			int result;
			try
			{
				result = this.currentStateName;
			}
			finally
			{
				Monitor.Exit(obj);
			}
			return result;
		}
		public void SetCurrentState(object state)
		{
			int toStateName = (int)state;
			if (this.onStateChange != null)
			{
				this.onStateChange(this.currentStateName, toStateName);
			}
			this.currentStateName = toStateName;
		}
		private FSMState FindStateObjByName(object st)
		{
			int num = (int)st;
			FSMState result;
			foreach (FSMState current in this.states)
			{
				if (num.Equals(current.GetStateName()))
				{
					result = current;
					return result;
				}
			}
			result = null;
			return result;
		}
	}
}
