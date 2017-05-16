using Sfs2X.Protocol.Serialization;
using Sfs2X.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
namespace Sfs2X.Entities.Data
{
	public class SFSObject : ISFSObject
	{
		private Dictionary<string, SFSDataWrapper> dataHolder;
		private ISFSDataSerializer serializer;
		public static SFSObject NewFromBinaryData(ByteArray ba)
		{
			return DefaultSFSDataSerializer.Instance.Binary2Object(ba) as SFSObject;
		}
		public static SFSObject NewInstance()
		{
			return new SFSObject();
		}
		public SFSObject()
		{
			this.dataHolder = new Dictionary<string, SFSDataWrapper>();
			this.serializer = DefaultSFSDataSerializer.Instance;
		}
		private string Dump()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(Convert.ToString(DefaultObjectDumpFormatter.TOKEN_INDENT_OPEN));
			foreach (KeyValuePair<string, SFSDataWrapper> current in this.dataHolder)
			{
				SFSDataWrapper value = current.Value;
				string key = current.Key;
				int type = value.Type;
				stringBuilder.Append("(" + ((SFSDataType)type).ToString().ToLower() + ")");
				stringBuilder.Append(" " + key + ": ");
				if (type == 18)
				{
					stringBuilder.Append((value.Data as SFSObject).GetDump(false));
				}
				else
				{
					if (type == 17)
					{
						stringBuilder.Append((value.Data as SFSArray).GetDump(false));
					}
					else
					{
						if (type > 8 && type < 19)
						{
							stringBuilder.Append("[" + value.Data + "]");
						}
						else
						{
							stringBuilder.Append(value.Data);
						}
					}
				}
				stringBuilder.Append(DefaultObjectDumpFormatter.TOKEN_DIVIDER);
			}
			string text = stringBuilder.ToString();
			if (this.Size() > 0)
			{
				text = text.Substring(0, text.Length - 1);
			}
			return text + DefaultObjectDumpFormatter.TOKEN_INDENT_CLOSE;
		}
		public SFSDataWrapper GetData(string key)
		{
			return this.dataHolder[key];
		}
		public T GetValue<T>(string key)
		{
			T result;
			if (!this.dataHolder.ContainsKey(key))
			{
				result = default(T);
			}
			else
			{
				result = (T)((object)this.dataHolder[key].Data);
			}
			return result;
		}
		public bool GetBool(string key)
		{
			return this.GetValue<bool>(key);
		}
		public byte GetByte(string key)
		{
			return this.GetValue<byte>(key);
		}
		public short GetShort(string key)
		{
			return this.GetValue<short>(key);
		}
		public int GetInt(string key)
		{
			return this.GetValue<int>(key);
		}
		public long GetLong(string key)
		{
			return this.GetValue<long>(key);
		}
		public float GetFloat(string key)
		{
			return this.GetValue<float>(key);
		}
		public double GetDouble(string key)
		{
			return this.GetValue<double>(key);
		}
		public string GetUtfString(string key)
		{
			return this.GetValue<string>(key);
		}
		private ICollection GetArray(string key)
		{
			return this.GetValue<ICollection>(key);
		}
		public bool[] GetBoolArray(string key)
		{
			return (bool[])this.GetArray(key);
		}
		public ByteArray GetByteArray(string key)
		{
			return this.GetValue<ByteArray>(key);
		}
		public short[] GetShortArray(string key)
		{
			return (short[])this.GetArray(key);
		}
		public int[] GetIntArray(string key)
		{
			return (int[])this.GetArray(key);
		}
		public long[] GetLongArray(string key)
		{
			return (long[])this.GetArray(key);
		}
		public float[] GetFloatArray(string key)
		{
			return (float[])this.GetArray(key);
		}
		public double[] GetDoubleArray(string key)
		{
			return (double[])this.GetArray(key);
		}
		public string[] GetUtfStringArray(string key)
		{
			return (string[])this.GetArray(key);
		}
		public ISFSArray GetSFSArray(string key)
		{
			return this.GetValue<ISFSArray>(key);
		}
		public ISFSObject GetSFSObject(string key)
		{
			return this.GetValue<ISFSObject>(key);
		}
		public void PutNull(string key)
		{
			this.dataHolder[key] = new SFSDataWrapper(SFSDataType.NULL, null);
		}
		public void PutBool(string key, bool val)
		{
			this.dataHolder[key] = new SFSDataWrapper(SFSDataType.BOOL, val);
		}
		public void PutByte(string key, byte val)
		{
			this.dataHolder[key] = new SFSDataWrapper(SFSDataType.BYTE, val);
		}
		public void PutShort(string key, short val)
		{
			this.dataHolder[key] = new SFSDataWrapper(SFSDataType.SHORT, val);
		}
		public void PutInt(string key, int val)
		{
			this.dataHolder[key] = new SFSDataWrapper(SFSDataType.INT, val);
		}
		public void PutLong(string key, long val)
		{
			this.dataHolder[key] = new SFSDataWrapper(SFSDataType.LONG, val);
		}
		public void PutFloat(string key, float val)
		{
			this.dataHolder[key] = new SFSDataWrapper(SFSDataType.FLOAT, val);
		}
		public void PutDouble(string key, double val)
		{
			this.dataHolder[key] = new SFSDataWrapper(SFSDataType.DOUBLE, val);
		}
		public void PutUtfString(string key, string val)
		{
			this.dataHolder[key] = new SFSDataWrapper(SFSDataType.UTF_STRING, val);
		}
		public void PutBoolArray(string key, bool[] val)
		{
			this.dataHolder[key] = new SFSDataWrapper(SFSDataType.BOOL_ARRAY, val);
		}
		public void PutByteArray(string key, ByteArray val)
		{
			this.dataHolder[key] = new SFSDataWrapper(SFSDataType.BYTE_ARRAY, val);
		}
		public void PutShortArray(string key, short[] val)
		{
			this.dataHolder[key] = new SFSDataWrapper(SFSDataType.SHORT_ARRAY, val);
		}
		public void PutIntArray(string key, int[] val)
		{
			this.dataHolder[key] = new SFSDataWrapper(SFSDataType.INT_ARRAY, val);
		}
		public void PutLongArray(string key, long[] val)
		{
			this.dataHolder[key] = new SFSDataWrapper(SFSDataType.LONG_ARRAY, val);
		}
		public void PutFloatArray(string key, float[] val)
		{
			this.dataHolder[key] = new SFSDataWrapper(SFSDataType.FLOAT_ARRAY, val);
		}
		public void PutDoubleArray(string key, double[] val)
		{
			this.dataHolder[key] = new SFSDataWrapper(SFSDataType.DOUBLE_ARRAY, val);
		}
		public void PutUtfStringArray(string key, string[] val)
		{
			this.dataHolder[key] = new SFSDataWrapper(SFSDataType.UTF_STRING_ARRAY, val);
		}
		public void PutSFSArray(string key, ISFSArray val)
		{
			this.dataHolder[key] = new SFSDataWrapper(SFSDataType.SFS_ARRAY, val);
		}
		public void PutSFSObject(string key, ISFSObject val)
		{
			this.dataHolder[key] = new SFSDataWrapper(SFSDataType.SFS_OBJECT, val);
		}
		public void Put(string key, SFSDataWrapper val)
		{
			this.dataHolder[key] = val;
		}
		public bool ContainsKey(string key)
		{
			return this.dataHolder.ContainsKey(key);
		}
		public object GetClass(string key)
		{
			SFSDataWrapper sFSDataWrapper = this.dataHolder[key];
			object result;
			if (sFSDataWrapper != null)
			{
				result = sFSDataWrapper.Data;
			}
			else
			{
				result = null;
			}
			return result;
		}
		public string GetDump(bool format)
		{
			string result;
			if (!format)
			{
				result = this.Dump();
			}
			else
			{
				result = DefaultObjectDumpFormatter.PrettyPrintDump(this.Dump());
			}
			return result;
		}
		public string GetDump()
		{
			return this.GetDump(true);
		}
		public string GetHexDump()
		{
			return DefaultObjectDumpFormatter.HexDump(this.ToBinary());
		}
		public string[] GetKeys()
		{
			string[] array = new string[this.dataHolder.Keys.Count];
			this.dataHolder.Keys.CopyTo(array, 0);
			return array;
		}
		public bool IsNull(string key)
		{
			return !this.ContainsKey(key) || this.GetData(key).Data == null;
		}
		public void PutClass(string key, object val)
		{
			this.dataHolder[key] = new SFSDataWrapper(SFSDataType.CLASS, val);
		}
		public void RemoveElement(string key)
		{
			this.dataHolder.Remove(key);
		}
		public int Size()
		{
			return this.dataHolder.Count;
		}
		public ByteArray ToBinary()
		{
			return this.serializer.Object2Binary(this);
		}
	}
}
