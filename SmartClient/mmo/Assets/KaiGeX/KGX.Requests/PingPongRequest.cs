using System;
namespace KaiGeX.Requests
{
	public class PingPongRequest : BaseRequest
	{
		public PingPongRequest() : base(RequestType.PingPong)
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
