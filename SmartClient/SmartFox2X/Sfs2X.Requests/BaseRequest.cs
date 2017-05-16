using Sfs2X.Bitswarm;
using Sfs2X.Entities.Data;
using System;
namespace Sfs2X.Requests
{
	public class BaseRequest : IRequest
	{
		public static readonly string KEY_ERROR_CODE = "ec";
		public static readonly string KEY_ERROR_PARAMS = "ep";
		protected ISFSObject sfso;
		private int id;
		protected int targetController;
		private bool isEncrypted;
		public int TargetController
		{
			get
			{
				return this.targetController;
			}
			set
			{
				this.targetController = value;
			}
		}
		public bool IsEncrypted
		{
			get
			{
				return this.isEncrypted;
			}
			set
			{
				this.isEncrypted = value;
			}
		}
		public IMessage Message
		{
			get
			{
				IMessage message = new Message();
				message.Id = this.id;
				message.IsEncrypted = this.isEncrypted;
				message.TargetController = this.targetController;
				message.Content = this.sfso;
				if (this is ExtensionRequest)
				{
					message.IsUDP = (this as ExtensionRequest).UseUDP;
				}
				return message;
			}
		}
		public int Id
		{
			get
			{
				return this.id;
			}
			set
			{
				this.id = value;
			}
		}
		public RequestType Type
		{
			get
			{
				return (RequestType)this.id;
			}
			set
			{
				this.id = (int)value;
			}
		}
		public BaseRequest(RequestType tp)
		{
			this.sfso = SFSObject.NewInstance();
			this.targetController = 0;
			this.isEncrypted = false;
			this.id = (int)tp;
		}
		public BaseRequest(int id)
		{
			this.sfso = SFSObject.NewInstance();
			this.targetController = 0;
			this.isEncrypted = false;
			this.id = id;
		}
		public virtual void Validate(SmartFox sfs)
		{
		}
		public virtual void Execute(SmartFox sfs)
		{
		}
	}
}
