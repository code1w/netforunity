using System;
using System.Collections;
namespace Sfs2X.Util
{
	public class XMLNode : Hashtable
	{
		public XMLNodeList GetNodeList(string path)
		{
			return this.GetObject(path) as XMLNodeList;
		}
		public XMLNode GetNode(string path)
		{
			return this.GetObject(path) as XMLNode;
		}
		public string GetValue(string path)
		{
			return this.GetObject(path) as string;
		}
		private object GetObject(string path)
		{
			string[] array = path.Split(new char[]
			{
				'>'
			});
			XMLNode xMLNode = this;
			XMLNodeList xMLNodeList = null;
			bool flag = false;
			object result;
			for (int i = 0; i < array.Length; i++)
			{
				if (flag)
				{
					xMLNode = (XMLNode)xMLNodeList[int.Parse(array[i])];
					flag = false;
				}
				else
				{
					object obj = xMLNode[array[i]];
					if (!(obj is ArrayList))
					{
						if (i != array.Length - 1)
						{
							string str = "";
							for (int j = 0; j <= i; j++)
							{
								str = str + ">" + array[j];
							}
						}
						result = obj;
						return result;
					}
					xMLNodeList = (XMLNodeList)(obj as ArrayList);
					flag = true;
				}
			}
			if (flag)
			{
				result = xMLNodeList;
				return result;
			}
			result = xMLNode;
			return result;
		}
	}
}
