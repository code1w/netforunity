using KaiGeX.Exceptions;
using KaiGeX.Util;
using System;
using System.Text;
namespace KaiGeX.Protocol.Serialization
{
	public class DefaultObjectDumpFormatter
	{
		public static readonly char TOKEN_INDENT_OPEN = '{';
		public static readonly char TOKEN_INDENT_CLOSE = '}';
		public static readonly char TOKEN_DIVIDER = ';';
		public static readonly char NEW_LINE = '\n';
		public static readonly char TAB = '\t';
		public static readonly char DOT = '.';
		public static readonly int HEX_BYTES_PER_LINE = 16;
		public static readonly int MAX_DUMP_LENGTH = 1024;
		public static string PrettyPrintDump(string rawDump)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			for (int i = 0; i < rawDump.Length; i++)
			{
				char c = rawDump[i];
				if (c == DefaultObjectDumpFormatter.TOKEN_INDENT_OPEN)
				{
					num++;
					stringBuilder.Append(DefaultObjectDumpFormatter.NEW_LINE + DefaultObjectDumpFormatter.GetFormatTabs(num));
				}
				else
				{
					if (c == DefaultObjectDumpFormatter.TOKEN_INDENT_CLOSE)
					{
						num--;
						if (num < 0)
						{
							throw new SFSError("DumpFormatter: the indentPos is negative. TOKENS ARE NOT BALANCED!");
						}
						stringBuilder.Append(DefaultObjectDumpFormatter.NEW_LINE + DefaultObjectDumpFormatter.GetFormatTabs(num));
					}
					else
					{
						if (c == DefaultObjectDumpFormatter.TOKEN_DIVIDER)
						{
							stringBuilder.Append(DefaultObjectDumpFormatter.NEW_LINE + DefaultObjectDumpFormatter.GetFormatTabs(num));
						}
						else
						{
							stringBuilder.Append(c);
						}
					}
				}
			}
			if (num != 0)
			{
				throw new SFSError("DumpFormatter: the indentPos is not == 0. TOKENS ARE NOT BALANCED!");
			}
			return stringBuilder.ToString();
		}
		private static string GetFormatTabs(int howMany)
		{
			return DefaultObjectDumpFormatter.StrFill(DefaultObjectDumpFormatter.TAB, howMany);
		}
		private static string StrFill(char ch, int howMany)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < howMany; i++)
			{
				stringBuilder.Append(ch);
			}
			return stringBuilder.ToString();
		}
		public static string HexDump(ByteArray ba)
		{
			return DefaultObjectDumpFormatter.HexDump(ba, DefaultObjectDumpFormatter.HEX_BYTES_PER_LINE);
		}
		public static string HexDump(ByteArray ba, int bytesPerLine)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("Binary Size: " + ba.Length.ToString() + DefaultObjectDumpFormatter.NEW_LINE);
			string result;
			if (ba.Length > DefaultObjectDumpFormatter.MAX_DUMP_LENGTH)
			{
				stringBuilder.Append("** Data larger than max dump size of " + DefaultObjectDumpFormatter.MAX_DUMP_LENGTH + ". Data not displayed");
				result = stringBuilder.ToString();
			}
			else
			{
				StringBuilder stringBuilder2 = new StringBuilder();
				StringBuilder stringBuilder3 = new StringBuilder();
				int num = 0;
				int num2 = 0;
				do
				{
					byte b = ba.Bytes[num];
					string text = string.Format("{0:x2}", b);
					if (text.Length == 1)
					{
						text = "0" + text;
					}
					stringBuilder2.Append(text + " ");
					char value;
					if (b >= 33 && b <= 126)
					{
						value = Convert.ToChar(b);
					}
					else
					{
						value = DefaultObjectDumpFormatter.DOT;
					}
					stringBuilder3.Append(value);
					if (++num2 == bytesPerLine)
					{
						num2 = 0;
						stringBuilder.Append(string.Concat(new object[]
						{
							stringBuilder2.ToString(),
							DefaultObjectDumpFormatter.TAB,
							stringBuilder3.ToString(),
							DefaultObjectDumpFormatter.NEW_LINE
						}));
						stringBuilder2 = new StringBuilder();
						stringBuilder3 = new StringBuilder();
					}
				}
				while (++num < ba.Length);
				if (num2 != 0)
				{
					for (int i = bytesPerLine - num2; i > 0; i--)
					{
						stringBuilder2.Append("   ");
						stringBuilder3.Append(" ");
					}
					stringBuilder.Append(string.Concat(new object[]
					{
						stringBuilder2.ToString(),
						DefaultObjectDumpFormatter.TAB,
						stringBuilder3.ToString(),
						DefaultObjectDumpFormatter.NEW_LINE
					}));
				}
				result = stringBuilder.ToString();
			}
			return result;
		}
	}
}
