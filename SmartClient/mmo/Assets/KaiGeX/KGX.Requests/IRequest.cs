using KaiGeX.Bitswarm;
using System;
namespace KaiGeX.Requests
{
	public interface IRequest
	{
		int TargetController
		{
			get;
			set;
		}
		bool IsEncrypted
		{
			get;
			set;
		}
		IMessage Message
		{
			get;
		}
		void Validate(KaiGeNet sfs);
		void Execute(KaiGeNet sfs);
	}
}
