using System;
namespace Sfs2X.Requests
{
	public class HandshakeRequest : BaseRequest, IRequest
	{
		public static readonly string KEY_SESSION_TOKEN = "tk";
		public static readonly string KEY_API = "api";
		public static readonly string KEY_COMPRESSION_THRESHOLD = "ct";
		public static readonly string KEY_RECONNECTION_TOKEN = "rt";
		public static readonly string KEY_CLIENT_TYPE = "cl";
		public static readonly string KEY_MAX_MESSAGE_SIZE = "ms";
		public HandshakeRequest(string apiVersion, string reconnectionToken, string clientDetails) : base(RequestType.Handshake)
		{
			this.sfso.PutUtfString(HandshakeRequest.KEY_API, apiVersion);
			this.sfso.PutUtfString(HandshakeRequest.KEY_CLIENT_TYPE, clientDetails);
			this.sfso.PutBool("bin", true);
			if (reconnectionToken != null)
			{
				this.sfso.PutUtfString(HandshakeRequest.KEY_RECONNECTION_TOKEN, reconnectionToken);
			}
		}
	}
}
