using System;
using System.Collections.Generic;
namespace Sfs2X.FSM
{
	public class FSMState
	{
		private int stateName;
		private Dictionary<int, int> transitions = new Dictionary<int, int>();
		public void SetStateName(int newStateName)
		{
			this.stateName = newStateName;
		}
		public int GetStateName()
		{
			return this.stateName;
		}
		public void AddTransition(int transition, int outputState)
		{
			this.transitions[transition] = outputState;
		}
		public int ApplyTransition(int transition)
		{
			int result = this.stateName;
			if (this.transitions.ContainsKey(transition))
			{
				result = this.transitions[transition];
			}
			return result;
		}
	}
}
