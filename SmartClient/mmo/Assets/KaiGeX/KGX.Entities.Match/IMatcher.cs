using System;
namespace KaiGeX.Entities.Match
{
	public interface IMatcher
	{
		string Symbol
		{
			get;
		}
		int Type
		{
			get;
		}
	}
}
