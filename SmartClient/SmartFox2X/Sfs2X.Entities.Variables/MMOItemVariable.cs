using Sfs2X.Entities.Data;
using System;
namespace Sfs2X.Entities.Variables
{
	public class MMOItemVariable : SFSUserVariable, IMMOItemVariable, UserVariable
	{
		public new static IMMOItemVariable FromSFSArray(ISFSArray sfsa)
		{
			return new MMOItemVariable(sfsa.GetUtfString(0), sfsa.GetElementAt(2), (int)sfsa.GetByte(1));
		}
		public MMOItemVariable(string name, object val, int type) : base(name, val, type)
		{
		}
		public MMOItemVariable(string name, object val) : base(name, val, -1)
		{
		}
	}
}
