using KaiGeX.Entities.Data;
using KaiGeX.Exceptions;
using KaiGeX.Util;
using System;
using System.Collections;
using System.Reflection;
namespace KaiGeX.Protocol.Serialization
{
	public class DefaultSFSDataSerializer : ISFSDataSerializer
	{
		private static readonly string CLASS_MARKER_KEY = "$C";
		private static readonly string CLASS_FIELDS_KEY = "$F";
		private static readonly string FIELD_NAME_KEY = "N";
		private static readonly string FIELD_VALUE_KEY = "V";
		private static DefaultSFSDataSerializer instance;
		private static Assembly runningAssembly = null;
		public static DefaultSFSDataSerializer Instance
		{
			get
			{
				if (DefaultSFSDataSerializer.instance == null)
				{
					DefaultSFSDataSerializer.instance = new DefaultSFSDataSerializer();
				}
				return DefaultSFSDataSerializer.instance;
			}
		}
		public static Assembly RunningAssembly
		{
			get
			{
				return DefaultSFSDataSerializer.runningAssembly;
			}
			set
			{
				DefaultSFSDataSerializer.runningAssembly = value;
			}
		}
		private DefaultSFSDataSerializer()
		{
		}
		public ByteArray Object2Binary(ISFSObject obj)
		{
			ByteArray byteArray = new ByteArray();
			byteArray.WriteByte(Convert.ToByte(18));
			byteArray.WriteShort(Convert.ToInt16(obj.Size()));
			return this.Obj2bin(obj, byteArray);
		}
		private ByteArray Obj2bin(ISFSObject obj, ByteArray buffer)
		{
			string[] keys = obj.GetKeys();
			string[] array = keys;
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i];
				SFSDataWrapper data = obj.GetData(text);
				buffer = this.EncodeSFSObjectKey(buffer, text);
				buffer = this.EncodeObject(buffer, data.Type, data.Data);
			}
			return buffer;
		}
		public ByteArray Array2Binary(ISFSArray array)
		{
			ByteArray byteArray = new ByteArray();
			byteArray.WriteByte(Convert.ToByte(17));
			byteArray.WriteShort(Convert.ToInt16(array.Size()));
			return this.Arr2bin(array, byteArray);
		}
		private ByteArray Arr2bin(ISFSArray array, ByteArray buffer)
		{
			for (int i = 0; i < array.Size(); i++)
			{
				SFSDataWrapper wrappedElementAt = array.GetWrappedElementAt(i);
				buffer = this.EncodeObject(buffer, wrappedElementAt.Type, wrappedElementAt.Data);
			}
			return buffer;
		}
		public ISFSObject Binary2Object(ByteArray data)
		{
			if (data.Length < 3)
			{
				throw new SFSCodecError("Can't decode an SFSObject. Byte data is insufficient. Size: " + data.Length + " byte(s)");
			}
			data.Position = 0;
			return this.DecodeSFSObject(data);
		}
		private ISFSObject DecodeSFSObject(ByteArray buffer)
		{
			SFSObject sFSObject = SFSObject.NewInstance();
			byte b = buffer.ReadByte();
			if (b != Convert.ToByte(18))
			{
				throw new SFSCodecError(string.Concat(new object[]
				{
					"Invalid SFSDataType. Expected: ",
					SFSDataType.SFS_OBJECT,
					", found: ",
					b
				}));
			}
			int num = (int)buffer.ReadShort();
			if (num < 0)
			{
				throw new SFSCodecError("Can't decode SFSObject. Size is negative: " + num);
			}
			try
			{
				for (int i = 0; i < num; i++)
				{
					string text = buffer.ReadUTF();
					SFSDataWrapper sFSDataWrapper = this.DecodeObject(buffer);
					if (sFSDataWrapper == null)
					{
						throw new SFSCodecError("Could not decode value for SFSObject with key: " + text);
					}
					sFSObject.Put(text, sFSDataWrapper);
				}
			}
			catch (SFSCodecError sFSCodecError)
			{
				throw sFSCodecError;
			}
			return sFSObject;
		}
		public ISFSArray Binary2Array(ByteArray data)
		{
			if (data.Length < 3)
			{
				throw new SFSCodecError("Can't decode an SFSArray. Byte data is insufficient. Size: " + data.Length + " byte(s)");
			}
			data.Position = 0;
			return this.DecodeSFSArray(data);
		}
		private ISFSArray DecodeSFSArray(ByteArray buffer)
		{
			ISFSArray iSFSArray = SFSArray.NewInstance();
			SFSDataType sFSDataType = (SFSDataType)Convert.ToInt32(buffer.ReadByte());
			if (sFSDataType != SFSDataType.SFS_ARRAY)
			{
				throw new SFSCodecError(string.Concat(new object[]
				{
					"Invalid SFSDataType. Expected: ",
					SFSDataType.SFS_ARRAY,
					", found: ",
					sFSDataType
				}));
			}
			int num = (int)buffer.ReadShort();
			if (num < 0)
			{
				throw new SFSCodecError("Can't decode SFSArray. Size is negative: " + num);
			}
			try
			{
				for (int i = 0; i < num; i++)
				{
					SFSDataWrapper sFSDataWrapper = this.DecodeObject(buffer);
					if (sFSDataWrapper == null)
					{
						throw new SFSCodecError("Could not decode SFSArray item at index: " + i);
					}
					iSFSArray.Add(sFSDataWrapper);
				}
			}
			catch (SFSCodecError sFSCodecError)
			{
				throw sFSCodecError;
			}
			return iSFSArray;
		}
		private SFSDataWrapper DecodeObject(ByteArray buffer)
		{
			SFSDataType sFSDataType = (SFSDataType)Convert.ToInt32(buffer.ReadByte());
			SFSDataWrapper result;
			if (sFSDataType == SFSDataType.NULL)
			{
				result = this.BinDecode_NULL(buffer);
			}
			else
			{
				if (sFSDataType == SFSDataType.BOOL)
				{
					result = this.BinDecode_BOOL(buffer);
				}
				else
				{
					if (sFSDataType == SFSDataType.BOOL_ARRAY)
					{
						result = this.BinDecode_BOOL_ARRAY(buffer);
					}
					else
					{
						if (sFSDataType == SFSDataType.BYTE)
						{
							result = this.BinDecode_BYTE(buffer);
						}
						else
						{
							if (sFSDataType == SFSDataType.BYTE_ARRAY)
							{
								result = this.BinDecode_BYTE_ARRAY(buffer);
							}
							else
							{
								if (sFSDataType == SFSDataType.SHORT)
								{
									result = this.BinDecode_SHORT(buffer);
								}
								else
								{
									if (sFSDataType == SFSDataType.SHORT_ARRAY)
									{
										result = this.BinDecode_SHORT_ARRAY(buffer);
									}
									else
									{
										if (sFSDataType == SFSDataType.INT)
										{
											result = this.BinDecode_INT(buffer);
										}
										else
										{
											if (sFSDataType == SFSDataType.INT_ARRAY)
											{
												result = this.BinDecode_INT_ARRAY(buffer);
											}
											else
											{
												if (sFSDataType == SFSDataType.LONG)
												{
													result = this.BinDecode_LONG(buffer);
												}
												else
												{
													if (sFSDataType == SFSDataType.LONG_ARRAY)
													{
														result = this.BinDecode_LONG_ARRAY(buffer);
													}
													else
													{
														if (sFSDataType == SFSDataType.FLOAT)
														{
															result = this.BinDecode_FLOAT(buffer);
														}
														else
														{
															if (sFSDataType == SFSDataType.FLOAT_ARRAY)
															{
																result = this.BinDecode_FLOAT_ARRAY(buffer);
															}
															else
															{
																if (sFSDataType == SFSDataType.DOUBLE)
																{
																	result = this.BinDecode_DOUBLE(buffer);
																}
																else
																{
																	if (sFSDataType == SFSDataType.DOUBLE_ARRAY)
																	{
																		result = this.BinDecode_DOUBLE_ARRAY(buffer);
																	}
																	else
																	{
																		if (sFSDataType == SFSDataType.UTF_STRING)
																		{
																			result = this.BinDecode_UTF_STRING(buffer);
																		}
																		else
																		{
																			if (sFSDataType == SFSDataType.UTF_STRING_ARRAY)
																			{
																				result = this.BinDecode_UTF_STRING_ARRAY(buffer);
																			}
																			else
																			{
																				if (sFSDataType == SFSDataType.SFS_ARRAY)
																				{
																					buffer.Position--;
																					result = new SFSDataWrapper(17, this.DecodeSFSArray(buffer));
																				}
																				else
																				{
																					if (sFSDataType != SFSDataType.SFS_OBJECT)
																					{
																						throw new Exception("Unknow SFSDataType ID: " + sFSDataType);
																					}
																					buffer.Position--;
																					ISFSObject iSFSObject = this.DecodeSFSObject(buffer);
																					byte type = Convert.ToByte(18);
																					object data = iSFSObject;
																					if (iSFSObject.ContainsKey(DefaultSFSDataSerializer.CLASS_MARKER_KEY) && iSFSObject.ContainsKey(DefaultSFSDataSerializer.CLASS_FIELDS_KEY))
																					{
																						type = Convert.ToByte(19);
																						data = this.Sfs2Cs(iSFSObject);
																					}
																					result = new SFSDataWrapper((int)type, data);
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
										}
									}
								}
							}
						}
					}
				}
			}
			return result;
		}
		private ByteArray EncodeObject(ByteArray buffer, int typeId, object data)
		{
			switch (typeId)
			{
			case 0:
				buffer = this.BinEncode_NULL(buffer);
				break;
			case 1:
				buffer = this.BinEncode_BOOL(buffer, (bool)data);
				break;
			case 2:
				buffer = this.BinEncode_BYTE(buffer, (byte)data);
				break;
			case 3:
				buffer = this.BinEncode_SHORT(buffer, (short)data);
				break;
			case 4:
				buffer = this.BinEncode_INT(buffer, (int)data);
				break;
			case 5:
				buffer = this.BinEncode_LONG(buffer, (long)data);
				break;
			case 6:
				buffer = this.BinEncode_FLOAT(buffer, (float)data);
				break;
			case 7:
				buffer = this.BinEncode_DOUBLE(buffer, (double)data);
				break;
			case 8:
				buffer = this.BinEncode_UTF_STRING(buffer, (string)data);
				break;
			case 9:
				buffer = this.BinEncode_BOOL_ARRAY(buffer, (bool[])data);
				break;
			case 10:
				buffer = this.BinEncode_BYTE_ARRAY(buffer, (ByteArray)data);
				break;
			case 11:
				buffer = this.BinEncode_SHORT_ARRAY(buffer, (short[])data);
				break;
			case 12:
				buffer = this.BinEncode_INT_ARRAY(buffer, (int[])data);
				break;
			case 13:
				buffer = this.BinEncode_LONG_ARRAY(buffer, (long[])data);
				break;
			case 14:
				buffer = this.BinEncode_FLOAT_ARRAY(buffer, (float[])data);
				break;
			case 15:
				buffer = this.BinEncode_DOUBLE_ARRAY(buffer, (double[])data);
				break;
			case 16:
				buffer = this.BinEncode_UTF_STRING_ARRAY(buffer, (string[])data);
				break;
			case 17:
				buffer = this.AddData(buffer, this.Array2Binary((ISFSArray)data));
				break;
			case 18:
				buffer = this.AddData(buffer, this.Object2Binary((SFSObject)data));
				break;
			case 19:
				buffer = this.AddData(buffer, this.Object2Binary(this.Cs2Sfs(data)));
				break;
			default:
				throw new SFSCodecError("Unrecognized type in SFSObject serialization: " + typeId);
			}
			return buffer;
		}
		private SFSDataWrapper BinDecode_NULL(ByteArray buffer)
		{
			return new SFSDataWrapper(SFSDataType.NULL, null);
		}
		private SFSDataWrapper BinDecode_BOOL(ByteArray buffer)
		{
			return new SFSDataWrapper(SFSDataType.BOOL, buffer.ReadBool());
		}
		private SFSDataWrapper BinDecode_BYTE(ByteArray buffer)
		{
			return new SFSDataWrapper(SFSDataType.BYTE, buffer.ReadByte());
		}
		private SFSDataWrapper BinDecode_SHORT(ByteArray buffer)
		{
			return new SFSDataWrapper(SFSDataType.SHORT, buffer.ReadShort());
		}
		private SFSDataWrapper BinDecode_INT(ByteArray buffer)
		{
			return new SFSDataWrapper(SFSDataType.INT, buffer.ReadInt());
		}
		private SFSDataWrapper BinDecode_LONG(ByteArray buffer)
		{
			return new SFSDataWrapper(SFSDataType.LONG, buffer.ReadLong());
		}
		private SFSDataWrapper BinDecode_FLOAT(ByteArray buffer)
		{
			return new SFSDataWrapper(SFSDataType.FLOAT, buffer.ReadFloat());
		}
		private SFSDataWrapper BinDecode_DOUBLE(ByteArray buffer)
		{
			return new SFSDataWrapper(SFSDataType.DOUBLE, buffer.ReadDouble());
		}
		private SFSDataWrapper BinDecode_UTF_STRING(ByteArray buffer)
		{
			return new SFSDataWrapper(SFSDataType.UTF_STRING, buffer.ReadUTF());
		}
		private SFSDataWrapper BinDecode_BOOL_ARRAY(ByteArray buffer)
		{
			int typedArraySize = this.GetTypedArraySize(buffer);
			bool[] array = new bool[typedArraySize];
			for (int i = 0; i < typedArraySize; i++)
			{
				array[i] = buffer.ReadBool();
			}
			return new SFSDataWrapper(SFSDataType.BOOL_ARRAY, array);
		}
		private SFSDataWrapper BinDecode_BYTE_ARRAY(ByteArray buffer)
		{
			int num = buffer.ReadInt();
			if (num < 0)
			{
				throw new SFSCodecError("Array negative size: " + num);
			}
			ByteArray data = new ByteArray(buffer.ReadBytes(num));
			return new SFSDataWrapper(SFSDataType.BYTE_ARRAY, data);
		}
		private SFSDataWrapper BinDecode_SHORT_ARRAY(ByteArray buffer)
		{
			int typedArraySize = this.GetTypedArraySize(buffer);
			short[] array = new short[typedArraySize];
			for (int i = 0; i < typedArraySize; i++)
			{
				array[i] = buffer.ReadShort();
			}
			return new SFSDataWrapper(SFSDataType.SHORT_ARRAY, array);
		}
		private SFSDataWrapper BinDecode_INT_ARRAY(ByteArray buffer)
		{
			int typedArraySize = this.GetTypedArraySize(buffer);
			int[] array = new int[typedArraySize];
			for (int i = 0; i < typedArraySize; i++)
			{
				array[i] = buffer.ReadInt();
			}
			return new SFSDataWrapper(SFSDataType.INT_ARRAY, array);
		}
		private SFSDataWrapper BinDecode_LONG_ARRAY(ByteArray buffer)
		{
			int typedArraySize = this.GetTypedArraySize(buffer);
			long[] array = new long[typedArraySize];
			for (int i = 0; i < typedArraySize; i++)
			{
				array[i] = buffer.ReadLong();
			}
			return new SFSDataWrapper(SFSDataType.LONG_ARRAY, array);
		}
		private SFSDataWrapper BinDecode_FLOAT_ARRAY(ByteArray buffer)
		{
			int typedArraySize = this.GetTypedArraySize(buffer);
			float[] array = new float[typedArraySize];
			for (int i = 0; i < typedArraySize; i++)
			{
				array[i] = buffer.ReadFloat();
			}
			return new SFSDataWrapper(SFSDataType.FLOAT_ARRAY, array);
		}
		private SFSDataWrapper BinDecode_DOUBLE_ARRAY(ByteArray buffer)
		{
			int typedArraySize = this.GetTypedArraySize(buffer);
			double[] array = new double[typedArraySize];
			for (int i = 0; i < typedArraySize; i++)
			{
				array[i] = buffer.ReadDouble();
			}
			return new SFSDataWrapper(SFSDataType.DOUBLE_ARRAY, array);
		}
		private SFSDataWrapper BinDecode_UTF_STRING_ARRAY(ByteArray buffer)
		{
			int typedArraySize = this.GetTypedArraySize(buffer);
			string[] array = new string[typedArraySize];
			for (int i = 0; i < typedArraySize; i++)
			{
				array[i] = buffer.ReadUTF();
			}
			return new SFSDataWrapper(SFSDataType.UTF_STRING_ARRAY, array);
		}
		private int GetTypedArraySize(ByteArray buffer)
		{
			short num = buffer.ReadShort();
			if (num < 0)
			{
				throw new SFSCodecError("Array negative size: " + num);
			}
			return (int)num;
		}
		private ByteArray BinEncode_NULL(ByteArray buffer)
		{
			ByteArray byteArray = new ByteArray();
			byteArray.WriteByte(SFSDataType.NULL);
			return this.AddData(buffer, byteArray);
		}
		private ByteArray BinEncode_BOOL(ByteArray buffer, bool val)
		{
			ByteArray byteArray = new ByteArray();
			byteArray.WriteByte(SFSDataType.BOOL);
			byteArray.WriteBool(val);
			return this.AddData(buffer, byteArray);
		}
		private ByteArray BinEncode_BYTE(ByteArray buffer, byte val)
		{
			ByteArray byteArray = new ByteArray();
			byteArray.WriteByte(SFSDataType.BYTE);
			byteArray.WriteByte(val);
			return this.AddData(buffer, byteArray);
		}
		private ByteArray BinEncode_SHORT(ByteArray buffer, short val)
		{
			ByteArray byteArray = new ByteArray();
			byteArray.WriteByte(SFSDataType.SHORT);
			byteArray.WriteShort(val);
			return this.AddData(buffer, byteArray);
		}
		private ByteArray BinEncode_INT(ByteArray buffer, int val)
		{
			ByteArray byteArray = new ByteArray();
			byteArray.WriteByte(SFSDataType.INT);
			byteArray.WriteInt(val);
			return this.AddData(buffer, byteArray);
		}
		private ByteArray BinEncode_LONG(ByteArray buffer, long val)
		{
			ByteArray byteArray = new ByteArray();
			byteArray.WriteByte(SFSDataType.LONG);
			byteArray.WriteLong(val);
			return this.AddData(buffer, byteArray);
		}
		private ByteArray BinEncode_FLOAT(ByteArray buffer, float val)
		{
			ByteArray byteArray = new ByteArray();
			byteArray.WriteByte(SFSDataType.FLOAT);
			byteArray.WriteFloat(val);
			return this.AddData(buffer, byteArray);
		}
		private ByteArray BinEncode_DOUBLE(ByteArray buffer, double val)
		{
			ByteArray byteArray = new ByteArray();
			byteArray.WriteByte(SFSDataType.DOUBLE);
			byteArray.WriteDouble(val);
			return this.AddData(buffer, byteArray);
		}
		private ByteArray BinEncode_INT(ByteArray buffer, double val)
		{
			ByteArray byteArray = new ByteArray();
			byteArray.WriteByte(SFSDataType.DOUBLE);
			byteArray.WriteDouble(val);
			return this.AddData(buffer, byteArray);
		}
		private ByteArray BinEncode_UTF_STRING(ByteArray buffer, string val)
		{
			ByteArray byteArray = new ByteArray();
			byteArray.WriteByte(SFSDataType.UTF_STRING);
			byteArray.WriteUTF(val);
			return this.AddData(buffer, byteArray);
		}
		private ByteArray BinEncode_BOOL_ARRAY(ByteArray buffer, bool[] val)
		{
			ByteArray byteArray = new ByteArray();
			byteArray.WriteByte(SFSDataType.BOOL_ARRAY);
			byteArray.WriteShort(Convert.ToInt16(val.Length));
			for (int i = 0; i < val.Length; i++)
			{
				byteArray.WriteBool(val[i]);
			}
			return this.AddData(buffer, byteArray);
		}
		private ByteArray BinEncode_BYTE_ARRAY(ByteArray buffer, ByteArray val)
		{
			ByteArray byteArray = new ByteArray();
			byteArray.WriteByte(SFSDataType.BYTE_ARRAY);
			byteArray.WriteInt(val.Length);
			byteArray.WriteBytes(val.Bytes);
			return this.AddData(buffer, byteArray);
		}
		private ByteArray BinEncode_SHORT_ARRAY(ByteArray buffer, short[] val)
		{
			ByteArray byteArray = new ByteArray();
			byteArray.WriteByte(SFSDataType.SHORT_ARRAY);
			byteArray.WriteShort(Convert.ToInt16(val.Length));
			for (int i = 0; i < val.Length; i++)
			{
				byteArray.WriteShort(val[i]);
			}
			return this.AddData(buffer, byteArray);
		}
		private ByteArray BinEncode_INT_ARRAY(ByteArray buffer, int[] val)
		{
			ByteArray byteArray = new ByteArray();
			byteArray.WriteByte(SFSDataType.INT_ARRAY);
			byteArray.WriteShort(Convert.ToInt16(val.Length));
			for (int i = 0; i < val.Length; i++)
			{
				byteArray.WriteInt(val[i]);
			}
			return this.AddData(buffer, byteArray);
		}
		private ByteArray BinEncode_LONG_ARRAY(ByteArray buffer, long[] val)
		{
			ByteArray byteArray = new ByteArray();
			byteArray.WriteByte(SFSDataType.LONG_ARRAY);
			byteArray.WriteShort(Convert.ToInt16(val.Length));
			for (int i = 0; i < val.Length; i++)
			{
				byteArray.WriteLong(val[i]);
			}
			return this.AddData(buffer, byteArray);
		}
		private ByteArray BinEncode_FLOAT_ARRAY(ByteArray buffer, float[] val)
		{
			ByteArray byteArray = new ByteArray();
			byteArray.WriteByte(SFSDataType.FLOAT_ARRAY);
			byteArray.WriteShort(Convert.ToInt16(val.Length));
			for (int i = 0; i < val.Length; i++)
			{
				byteArray.WriteFloat(val[i]);
			}
			return this.AddData(buffer, byteArray);
		}
		private ByteArray BinEncode_DOUBLE_ARRAY(ByteArray buffer, double[] val)
		{
			ByteArray byteArray = new ByteArray();
			byteArray.WriteByte(SFSDataType.DOUBLE_ARRAY);
			byteArray.WriteShort(Convert.ToInt16(val.Length));
			for (int i = 0; i < val.Length; i++)
			{
				byteArray.WriteDouble(val[i]);
			}
			return this.AddData(buffer, byteArray);
		}
		private ByteArray BinEncode_UTF_STRING_ARRAY(ByteArray buffer, string[] val)
		{
			ByteArray byteArray = new ByteArray();
			byteArray.WriteByte(SFSDataType.UTF_STRING_ARRAY);
			byteArray.WriteShort(Convert.ToInt16(val.Length));
			for (int i = 0; i < val.Length; i++)
			{
				byteArray.WriteUTF(val[i]);
			}
			return this.AddData(buffer, byteArray);
		}
		private ByteArray EncodeSFSObjectKey(ByteArray buffer, string val)
		{
			buffer.WriteUTF(val);
			return buffer;
		}
		private ByteArray AddData(ByteArray buffer, ByteArray newData)
		{
			buffer.WriteBytes(newData.Bytes);
			return buffer;
		}
		public ISFSObject Cs2Sfs(object csObj)
		{
			ISFSObject iSFSObject = SFSObject.NewInstance();
			this.ConvertCsObj(csObj, iSFSObject);
			return iSFSObject;
		}
		private void ConvertCsObj(object csObj, ISFSObject sfsObj)
		{
			Type type = csObj.GetType();
			string fullName = type.FullName;
			if (!(csObj is SerializableSFSType))
			{
				throw new SFSCodecError(string.Concat(new object[]
				{
					"Cannot serialize object: ",
					csObj,
					", type: ",
					fullName,
					" -- It doesn't implement the SerializableSFSType interface"
				}));
			}
			ISFSArray iSFSArray = SFSArray.NewInstance();
			sfsObj.PutUtfString(DefaultSFSDataSerializer.CLASS_MARKER_KEY, fullName);
			sfsObj.PutSFSArray(DefaultSFSDataSerializer.CLASS_FIELDS_KEY, iSFSArray);
			FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			FieldInfo[] array = fields;
			for (int i = 0; i < array.Length; i++)
			{
				FieldInfo fieldInfo = array[i];
				string name = fieldInfo.Name;
				object value = fieldInfo.GetValue(csObj);
				ISFSObject iSFSObject = SFSObject.NewInstance();
				SFSDataWrapper sFSDataWrapper = this.WrapField(value);
				if (sFSDataWrapper == null)
				{
					throw new SFSCodecError(string.Concat(new object[]
					{
						"Cannot serialize field of object: ",
						csObj,
						", field: ",
						name,
						", type: ",
						fieldInfo.GetType().Name,
						" -- unsupported type!"
					}));
				}
				iSFSObject.PutUtfString(DefaultSFSDataSerializer.FIELD_NAME_KEY, name);
				iSFSObject.Put(DefaultSFSDataSerializer.FIELD_VALUE_KEY, sFSDataWrapper);
				iSFSArray.AddSFSObject(iSFSObject);
			}
		}
		private SFSDataWrapper WrapField(object val)
		{
			SFSDataWrapper result;
			if (val == null)
			{
				result = new SFSDataWrapper(SFSDataType.NULL, null);
			}
			else
			{
				SFSDataWrapper sFSDataWrapper = null;
				if (val is bool)
				{
					sFSDataWrapper = new SFSDataWrapper(SFSDataType.BOOL, val);
				}
				else
				{
					if (val is byte)
					{
						sFSDataWrapper = new SFSDataWrapper(SFSDataType.BYTE, val);
					}
					else
					{
						if (val is short)
						{
							sFSDataWrapper = new SFSDataWrapper(SFSDataType.SHORT, val);
						}
						else
						{
							if (val is int)
							{
								sFSDataWrapper = new SFSDataWrapper(SFSDataType.INT, val);
							}
							else
							{
								if (val is long)
								{
									sFSDataWrapper = new SFSDataWrapper(SFSDataType.LONG, val);
								}
								else
								{
									if (val is float)
									{
										sFSDataWrapper = new SFSDataWrapper(SFSDataType.FLOAT, val);
									}
									else
									{
										if (val is double)
										{
											sFSDataWrapper = new SFSDataWrapper(SFSDataType.DOUBLE, val);
										}
										else
										{
											if (val is string)
											{
												sFSDataWrapper = new SFSDataWrapper(SFSDataType.UTF_STRING, val);
											}
											else
											{
												if (val is ArrayList)
												{
													sFSDataWrapper = new SFSDataWrapper(SFSDataType.SFS_ARRAY, this.UnrollArray(val as ArrayList));
												}
												else
												{
													if (val is SerializableSFSType)
													{
														sFSDataWrapper = new SFSDataWrapper(SFSDataType.SFS_OBJECT, this.Cs2Sfs(val));
													}
													else
													{
														if (val is Hashtable)
														{
															sFSDataWrapper = new SFSDataWrapper(SFSDataType.SFS_OBJECT, this.UnrollDictionary(val as Hashtable));
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
				}
				result = sFSDataWrapper;
			}
			return result;
		}
		private ISFSArray UnrollArray(ArrayList arr)
		{
			ISFSArray iSFSArray = SFSArray.NewInstance();
			IEnumerator enumerator = arr.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object current = enumerator.Current;
					SFSDataWrapper sFSDataWrapper = this.WrapField(current);
					if (sFSDataWrapper == null)
					{
						throw new SFSCodecError("Cannot serialize field of array: " + current + " -- unsupported type!");
					}
					iSFSArray.Add(sFSDataWrapper);
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			return iSFSArray;
		}
		private ISFSObject UnrollDictionary(Hashtable dict)
		{
			ISFSObject iSFSObject = SFSObject.NewInstance();
			IEnumerator enumerator = dict.Keys.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					string text = (string)enumerator.Current;
					SFSDataWrapper sFSDataWrapper = this.WrapField(dict[text]);
					if (sFSDataWrapper == null)
					{
						throw new SFSCodecError(string.Concat(new object[]
						{
							"Cannot serialize field of dictionary with key: ",
							text,
							", ",
							dict[text],
							" -- unsupported type!"
						}));
					}
					iSFSObject.Put(text, sFSDataWrapper);
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			return iSFSObject;
		}
		public object Sfs2Cs(ISFSObject sfsObj)
		{
			if (!sfsObj.ContainsKey(DefaultSFSDataSerializer.CLASS_MARKER_KEY) || !sfsObj.ContainsKey(DefaultSFSDataSerializer.CLASS_FIELDS_KEY))
			{
				throw new SFSCodecError("The SFSObject passed does not represent any serialized class.");
			}
			string utfString = sfsObj.GetUtfString(DefaultSFSDataSerializer.CLASS_MARKER_KEY);
			Type type;
			if (DefaultSFSDataSerializer.runningAssembly == null)
			{
				type = Type.GetType(utfString);
			}
			else
			{
				type = DefaultSFSDataSerializer.runningAssembly.GetType(utfString);
			}
			if (type == null)
			{
				throw new SFSCodecError("Cannot find type: " + utfString);
			}
			object obj = Activator.CreateInstance(type);
			if (!(obj is SerializableSFSType))
			{
				throw new SFSCodecError(string.Concat(new object[]
				{
					"Cannot deserialize object: ",
					obj,
					", type: ",
					utfString,
					" -- It doesn't implement the SerializableSFSType interface"
				}));
			}
			this.ConvertSFSObject(sfsObj.GetSFSArray(DefaultSFSDataSerializer.CLASS_FIELDS_KEY), obj, type);
			return obj;
		}
		private void ConvertSFSObject(ISFSArray fieldList, object csObj, Type objType)
		{
			for (int i = 0; i < fieldList.Size(); i++)
			{
				ISFSObject sFSObject = fieldList.GetSFSObject(i);
				string utfString = sFSObject.GetUtfString(DefaultSFSDataSerializer.FIELD_NAME_KEY);
				SFSDataWrapper data = sFSObject.GetData(DefaultSFSDataSerializer.FIELD_VALUE_KEY);
				object value = this.UnwrapField(data);
				FieldInfo field = objType.GetField(utfString, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (field != null)
				{
					field.SetValue(csObj, value);
				}
			}
		}
		private object UnwrapField(SFSDataWrapper wrapper)
		{
			object result = null;
			int type = wrapper.Type;
			if (type <= 8)
			{
				result = wrapper.Data;
			}
			else
			{
				if (type == 17)
				{
					result = this.RebuildArray(wrapper.Data as ISFSArray);
				}
				else
				{
					if (type == 18)
					{
						ISFSObject iSFSObject = wrapper.Data as ISFSObject;
						if (iSFSObject.ContainsKey(DefaultSFSDataSerializer.CLASS_MARKER_KEY) && iSFSObject.ContainsKey(DefaultSFSDataSerializer.CLASS_FIELDS_KEY))
						{
							result = this.Sfs2Cs(iSFSObject);
						}
						else
						{
							result = this.RebuildDict(wrapper.Data as ISFSObject);
						}
					}
					else
					{
						if (type == 19)
						{
							result = wrapper.Data;
						}
					}
				}
			}
			return result;
		}
		private ArrayList RebuildArray(ISFSArray sfsArr)
		{
			ArrayList arrayList = new ArrayList();
			for (int i = 0; i < sfsArr.Size(); i++)
			{
				arrayList.Add(this.UnwrapField(sfsArr.GetWrappedElementAt(i)));
			}
			return arrayList;
		}
		private Hashtable RebuildDict(ISFSObject sfsObj)
		{
			Hashtable hashtable = new Hashtable();
			string[] keys = sfsObj.GetKeys();
			for (int i = 0; i < keys.Length; i++)
			{
				string key = keys[i];
				hashtable[key] = this.UnwrapField(sfsObj.GetData(key));
			}
			return hashtable;
		}
	}
}
