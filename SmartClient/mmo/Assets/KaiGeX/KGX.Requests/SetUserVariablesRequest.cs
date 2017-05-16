﻿using KaiGeX.Entities.Data;
using KaiGeX.Entities.Variables;
using KaiGeX.Exceptions;
using System;
using System.Collections.Generic;
namespace KaiGeX.Requests
{
	public class SetUserVariablesRequest : BaseRequest
	{
		public static readonly string KEY_USER = "u";
		public static readonly string KEY_VAR_LIST = "vl";
		private ICollection<UserVariable> userVariables;
		public SetUserVariablesRequest(ICollection<UserVariable> userVariables) : base(RequestType.SetUserVariables)
		{
			this.userVariables = userVariables;
		}
		public override void Validate(KaiGeNet sfs)
		{
			List<string> list = new List<string>();
			if (this.userVariables == null || this.userVariables.Count == 0)
			{
				list.Add("No variables were specified");
			}
			if (list.Count > 0)
			{
				throw new SFSValidationError("SetUserVariables request error", list);
			}
		}
		public override void Execute(KaiGeNet sfs)
		{
			ISFSArray iSFSArray = SFSArray.NewInstance();
			foreach (UserVariable current in this.userVariables)
			{
				iSFSArray.AddSFSArray(current.ToSFSArray());
			}
			this.sfso.PutSFSArray(SetUserVariablesRequest.KEY_VAR_LIST, iSFSArray);
		}
	}
}
