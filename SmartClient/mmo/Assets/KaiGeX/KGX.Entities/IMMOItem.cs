using KaiGeX.Entities.Data;
using KaiGeX.Entities.Variables;
using System;
using System.Collections.Generic;
namespace KaiGeX.Entities
{
	public interface IMMOItem
	{
		int Id
		{
			get;
		}
		Vec3D AOIEntryPoint
		{
			get;
			set;
		}
		List<IMMOItemVariable> GetVariables();
		IMMOItemVariable GetVariable(string name);
		void SetVariable(IMMOItemVariable variable);
		void SetVariables(List<IMMOItemVariable> variables);
		bool ContainsVariable(string name);
	}
}
