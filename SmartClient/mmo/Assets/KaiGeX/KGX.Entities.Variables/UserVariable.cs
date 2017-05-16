﻿using KaiGeX.Entities.Data;
using System;
namespace KaiGeX.Entities.Variables
{
	public interface UserVariable
	{
		string Name
		{
			get;
		}
		VariableType Type
		{
			get;
		}
		object Value
		{
			get;
		}
		bool GetBoolValue();
		int GetIntValue();
		double GetDoubleValue();
		string GetStringValue();
		ISFSObject GetSFSObjectValue();
		ISFSArray GetSFSArrayValue();
		bool IsNull();
		ISFSArray ToSFSArray();
	}
}
