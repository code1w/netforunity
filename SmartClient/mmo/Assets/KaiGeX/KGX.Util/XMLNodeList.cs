using System;
using System.Collections;
namespace KaiGeX.Util
{
	public class XMLNodeList : ArrayList
	{
		public XMLNode Pop()
		{
			XMLNode xMLNode = (XMLNode)this[this.Count - 1];
			this.Remove(xMLNode);
			return xMLNode;
		}
		public int Push(XMLNode item)
		{
			this.Add(item);
			return this.Count;
		}
	}
}
