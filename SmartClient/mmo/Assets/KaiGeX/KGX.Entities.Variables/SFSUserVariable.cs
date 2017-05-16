using KaiGeX.Entities.Data;
using KaiGeX.Exceptions;
using System;
namespace KaiGeX.Entities.Variables
{
	public class SFSUserVariable : UserVariable
	{
		protected string name;
		protected VariableType type;
		protected object val;
		public string Name
		{
			get
			{
				return this.name;
			}
		}
		public VariableType Type
		{
			get
			{
				return this.type;
			}
		}
		public object Value
		{
			get
			{
				return this.val;
			}
		}
		public static UserVariable FromSFSArray(ISFSArray sfsa)
		{
			return new SFSUserVariable(sfsa.GetUtfString(0), sfsa.GetElementAt(2), (int)sfsa.GetByte(1));
		}
		public SFSUserVariable(string name, object val, int type)
		{
			this.name = name;
			if (type > -1)
			{
				this.val = val;
				this.type = (VariableType)type;
			}
			else
			{
				this.SetValue(val);
			}
		}
		public SFSUserVariable(string name, object val) : this(name, val, -1)
		{
		}
		public bool GetBoolValue()
		{
			return (bool)this.val;
		}
		public int GetIntValue()
		{
			return (int)this.val;
		}
		public double GetDoubleValue()
		{
			return (double)this.val;
		}
		public string GetStringValue()
		{
			return this.val as string;
		}
		public ISFSObject GetSFSObjectValue()
		{
			return this.val as ISFSObject;
		}
		public ISFSArray GetSFSArrayValue()
		{
			return this.val as ISFSArray;
		}
		public bool IsNull()
		{
			return this.type == VariableType.NULL;
		}
		public virtual ISFSArray ToSFSArray()
		{
			ISFSArray iSFSArray = SFSArray.NewInstance();
			iSFSArray.AddUtfString(this.name);
			iSFSArray.AddByte((byte)this.type);
			this.PopulateArrayWithValue(iSFSArray);
			return iSFSArray;
		}
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"[UVar: ",
				this.name,
				", type: ",
				this.type,
				", value: ",
				this.val,
				"]"
			});
		}
		private void PopulateArrayWithValue(ISFSArray arr)
		{
			switch (this.type)
			{
			case VariableType.NULL:
				arr.AddNull();
				break;
			case VariableType.BOOL:
				arr.AddBool(this.GetBoolValue());
				break;
			case VariableType.INT:
				arr.AddInt(this.GetIntValue());
				break;
			case VariableType.DOUBLE:
				arr.AddDouble(this.GetDoubleValue());
				break;
			case VariableType.STRING:
				arr.AddUtfString(this.GetStringValue());
				break;
			case VariableType.OBJECT:
				arr.AddSFSObject(this.GetSFSObjectValue());
				break;
			case VariableType.ARRAY:
				arr.AddSFSArray(this.GetSFSArrayValue());
				break;
			}
		}
		private void SetValue(object val)
		{
			this.val = val;
			if (val == null)
			{
				this.type = VariableType.NULL;
			}
			else
			{
				if (val is bool)
				{
					this.type = VariableType.BOOL;
				}
				else
				{
					if (val is int)
					{
						this.type = VariableType.INT;
					}
					else
					{
						if (val is double)
						{
							this.type = VariableType.DOUBLE;
						}
						else
						{
							if (val is string)
							{
								this.type = VariableType.STRING;
							}
							else
							{
								if (val is object)
								{
									string text = val.GetType().Name;
									if (text == "SFSObject")
									{
										this.type = VariableType.OBJECT;
									}
									else
									{
										if (!(text == "SFSArray"))
										{
											throw new SFSError("Unsupport SFS Variable type: " + text);
										}
										this.type = VariableType.ARRAY;
									}
								}
							}
						}
					}
				}
			}
		}
	}
}
