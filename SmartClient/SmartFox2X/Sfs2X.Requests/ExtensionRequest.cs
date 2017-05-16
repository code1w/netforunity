using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Exceptions;
using System;
using System.Collections.Generic;
namespace Sfs2X.Requests
{
	public class ExtensionRequest : BaseRequest
	{
		public static readonly string KEY_CMD = "c";
		public static readonly string KEY_PARAMS = "p";
		public static readonly string KEY_ROOM = "r";
		private string extCmd;
		private ISFSObject parameters;
		private Room room;
		private bool useUDP;
		public bool UseUDP
		{
			get
			{
				return this.useUDP;
			}
		}
		private void Init(string extCmd, ISFSObject parameters, Room room, bool useUDP)
		{
			this.targetController = 1;
			this.extCmd = extCmd;
			this.parameters = parameters;
			this.room = room;
			this.useUDP = useUDP;
			if (parameters == null)
			{
				parameters = new SFSObject();
			}
		}
		public ExtensionRequest(string extCmd, ISFSObject parameters, Room room, bool useUDP) : base(RequestType.CallExtension)
		{
			this.Init(extCmd, parameters, room, useUDP);
		}
		public ExtensionRequest(string extCmd, ISFSObject parameters, Room room) : base(RequestType.CallExtension)
		{
			this.Init(extCmd, parameters, room, false);
		}
		public ExtensionRequest(string extCmd, ISFSObject parameters) : base(RequestType.CallExtension)
		{
			this.Init(extCmd, parameters, null, false);
		}
		public override void Validate(SmartFox sfs)
		{
			List<string> list = new List<string>();
			if (this.extCmd == null || this.extCmd.Length == 0)
			{
				list.Add("Missing extension command");
			}
			if (this.parameters == null)
			{
				list.Add("Missing extension parameters");
			}
			if (list.Count > 0)
			{
				throw new SFSValidationError("ExtensionCall request error", list);
			}
		}
		public override void Execute(SmartFox sfs)
		{
			this.sfso.PutUtfString(ExtensionRequest.KEY_CMD, this.extCmd);
			this.sfso.PutInt(ExtensionRequest.KEY_ROOM, (this.room != null) ? this.room.Id : -1);
			this.sfso.PutSFSObject(ExtensionRequest.KEY_PARAMS, this.parameters);
		}
	}
}
