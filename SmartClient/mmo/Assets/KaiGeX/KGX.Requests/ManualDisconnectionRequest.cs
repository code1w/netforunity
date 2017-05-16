using System;
namespace KaiGeX.Requests
{
	public class ManualDisconnectionRequest : BaseRequest
	{
		public ManualDisconnectionRequest() : base(RequestType.ManualDisconnection)
		{
		}
		public override void Validate(KaiGeNet sfs)
		{
		}
		public override void Execute(KaiGeNet sfs)
		{
		}
	}
}
