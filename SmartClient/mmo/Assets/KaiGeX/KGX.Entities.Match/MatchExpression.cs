using KaiGeX.Entities.Data;
using System;
using System.Text;
namespace KaiGeX.Entities.Match
{
	public class MatchExpression
	{
		private string varName;
		private IMatcher condition;
		private object varValue;
		internal LogicOperator logicOp;
		internal MatchExpression parent;
		internal MatchExpression next;
		public string VarName
		{
			get
			{
				return this.varName;
			}
		}
		public IMatcher Condition
		{
			get
			{
				return this.condition;
			}
		}
		public object VarValue
		{
			get
			{
				return this.varValue;
			}
		}
		public LogicOperator LogicOp
		{
			get
			{
				return this.logicOp;
			}
		}
		internal static MatchExpression ChainedMatchExpression(string varName, IMatcher condition, object value, LogicOperator logicOp, MatchExpression parent)
		{
			return new MatchExpression(varName, condition, value)
			{
				logicOp = logicOp,
				parent = parent
			};
		}
		public MatchExpression(string varName, IMatcher condition, object varValue)
		{
			this.varName = varName;
			this.condition = condition;
			this.varValue = varValue;
		}
		public MatchExpression And(string varName, IMatcher condition, object varValue)
		{
			this.next = MatchExpression.ChainedMatchExpression(varName, condition, varValue, LogicOperator.AND, this);
			return this.next;
		}
		public MatchExpression Or(string varName, IMatcher condition, object varValue)
		{
			this.next = MatchExpression.ChainedMatchExpression(varName, condition, varValue, LogicOperator.OR, this);
			return this.next;
		}
		public bool HasNext()
		{
			return this.next != null;
		}
		public MatchExpression Next()
		{
			return this.next;
		}
		public MatchExpression Rewind()
		{
			MatchExpression matchExpression = this;
			while (matchExpression.parent != null)
			{
				matchExpression = matchExpression.parent;
			}
			return matchExpression;
		}
		public string AsString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (this.logicOp != null)
			{
				stringBuilder.Append(" " + this.logicOp.Id + " ");
			}
			stringBuilder.Append("(");
			stringBuilder.Append(string.Concat(new object[]
			{
				this.varName,
				" ",
				this.condition.Symbol,
				" ",
				(!(this.varValue is string)) ? this.varValue : ("'" + this.varValue + "'")
			}));
			stringBuilder.Append(")");
			return stringBuilder.ToString();
		}
		public override string ToString()
		{
			MatchExpression matchExpression = this.Rewind();
			StringBuilder stringBuilder = new StringBuilder(matchExpression.AsString());
			while (matchExpression.HasNext())
			{
				matchExpression = matchExpression.next;
				stringBuilder.Append(matchExpression.AsString());
			}
			return stringBuilder.ToString();
		}
		public ISFSArray ToSFSArray()
		{
			MatchExpression matchExpression = this.Rewind();
			ISFSArray iSFSArray = new SFSArray();
			iSFSArray.AddSFSArray(matchExpression.ExpressionAsSFSArray());
			while (matchExpression.HasNext())
			{
				matchExpression = matchExpression.Next();
				iSFSArray.AddSFSArray(matchExpression.ExpressionAsSFSArray());
			}
			return iSFSArray;
		}
		private ISFSArray ExpressionAsSFSArray()
		{
			ISFSArray iSFSArray = new SFSArray();
			if (this.logicOp != null)
			{
				iSFSArray.AddUtfString(this.logicOp.Id);
			}
			else
			{
				iSFSArray.AddNull();
			}
			iSFSArray.AddUtfString(this.varName);
			iSFSArray.AddByte((byte)this.condition.Type);
			iSFSArray.AddUtfString(this.condition.Symbol);
			if (this.condition.Type == 0)
			{
				iSFSArray.AddBool(Convert.ToBoolean(this.varValue));
			}
			else
			{
				if (this.condition.Type == 1)
				{
					iSFSArray.AddDouble(Convert.ToDouble(this.varValue));
				}
				else
				{
					iSFSArray.AddUtfString(Convert.ToString(this.varValue));
				}
			}
			return iSFSArray;
		}
	}
}
