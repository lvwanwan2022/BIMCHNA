using System.Diagnostics;
using System;
public static class GlobalMembersTExpressionTree
{


	#define _USE_MATH_DEFINES


	//替换  vars变量串 nums数字串 以空格分隔，支持表达式替换
	public static void TExpressionTree.Subs(string vars, string nums, bool output)
	{
		if (string.IsNullOrEmpty(vars) || string.IsNullOrEmpty(nums))
		{
			throw TError
			{
				enumError.ERROR_EMPTY_INPUT, TEXT("")
			}
			return;
		}

		List<string> VarsVector = StrSliceToVector(vars);
		List<string> NumsVector = StrSliceToVector(nums);

		if (VarsVector.Count == NumsVector.Count) //替换与被替换元素数目相等
		{
			for (size_t i = 0; i < VarsVector.Count; i++) //遍历被替换变量
			{
				//查表识别被替换变量
				auto it = pVariableTable.FindVariableTable(VarsVector[i]);
				if (it != pVariableTable.VariableTable.end()) //已识别出
				{
					string var = *it;
					//构建替换节点树
					TExpressionTree Expr = new TExpressionTree();
					Expr.LinkVariableTable(pVariableTable);
					Expr.Read(NumsVector[i], false);

					//得到所有被替换变量的位置
					List<TNode > VarsPos = new List<TNode >();
					GetVariablePos(var, VarsPos);
					for (size_t j = 0; j < VarsPos.Count; j++)
					{
						TNode newNode = CopyNodeTree(Expr.head);

						//连接到新节点
						if (VarsPos[j] != head)
						{
							if (VarsPos[j].parent.left != null && VarsPos[j].parent.left == VarsPos[j])
								VarsPos[j].parent.left = newNode;
							if (VarsPos[j].parent.right != null && VarsPos[j].parent.right == VarsPos[j])
								VarsPos[j].parent.right = newNode;
							newNode.parent = VarsPos[j].parent;
						}
						else
							head = newNode;

						//删掉旧节点
						VarsPos[j] = null;
					}
				}
			}
		}
		else
			throw TError
			{
				enumError.ERROR_SUBS_NOT_EQUAL, TEXT("")
			}

	}

	//读之前不清零，请自行处理
	public static void TExpressionTree.Read(string expression, bool bOutput)
	{
		Queue<TNode > InOrder = new Queue<TNode >();
		List<TNode > PostOrder = new List<TNode >();

		ReadToInOrder(expression, InOrder);
		InQueue2PostQueue(InOrder, PostOrder);
		BuildExpressionTree(PostOrder);
	}
}

//C++ TO C# CONVERTER TODO TASK: There is no equivalent to most C++ 'pragma' directives in C#:
//#pragma warning(disable:4703)
#define _CRT_SECURE_NO_WARNINGS
#define _CRT_NON_CONFORMING_SWPRINTFS




public class TExpressionTree
{
#define MAX_VAR_NAME
//C++ TO C# CONVERTER NOTE: The following #define macro was replaced in-line:
//ORIGINAL LINE: #define MIN_DOUBLE 1e-6
#define MIN_DOUBLE
	private enum enumMathOperator: int
	{
		MATH_NULL,
		//一元
		MATH_POSITIVE,
		MATH_NEGATIVE,

		//函数
		MATH_SIN,
		MATH_COS,
		MATH_TAN,
		MATH_ARCSIN,
		MATH_ARCCOS,
		MATH_ARCTAN,
		MATH_SQRT,
		MATH_LN,
		MATH_LOG10,
		MATH_EXP,

		//二元
		MATH_ADD,
		MATH_SUBSTRACT,
		MATH_MULTIPLY,
		MATH_DIVIDE,
		MATH_POWER,
		MATH_AND,
		MATH_OR,
		MATH_MOD,

		MATH_LEFT_PARENTHESIS,
		MATH_RIGHT_PARENTHESIS
	}

	private enum enumNodeType: int
	{
		NODE_NULL,
		NODE_NUMBER,
		NODE_OPERATOR,
		NODE_VARIABLE,
		NODE_FUNCTION
	}

	// 单个元素 
	private class TNode
	{
		public enumNodeType eType;
		public enumMathOperator eOperator;
		public double @value;
		public string varname;
		public TNode parent;
		public TNode left;
		public TNode right;
		public TNode()
		{
			eType = NODE_NULL;
			eOperator = MATH_NULL;
			@value = 0;
			parent = null;
			left = null;
			right = null;
		}
	}

	private void Release()
	{
		DeleteNode(head);
		head = null;
	}

	//运算符性质函数
	private int GetOperateNum(TExpressionTree.enumMathOperator eOperator)
	{
		switch (eOperator)
		{
		case enumMathOperator.MATH_SQRT:
		case enumMathOperator.MATH_SIN:
		case enumMathOperator.MATH_COS:
		case enumMathOperator.MATH_TAN:
		case enumMathOperator.MATH_ARCSIN:
		case enumMathOperator.MATH_ARCCOS:
		case enumMathOperator.MATH_ARCTAN:
		case enumMathOperator.MATH_LN:
		case enumMathOperator.MATH_LOG10:
		case enumMathOperator.MATH_EXP:
	
		case enumMathOperator.MATH_POSITIVE: //正负号
		case enumMathOperator.MATH_NEGATIVE:
			return 1;
	
		case enumMathOperator.MATH_MOD: //%
		case enumMathOperator.MATH_AND: //&
		case enumMathOperator.MATH_OR: //|
		case enumMathOperator.MATH_POWER: //^
		case enumMathOperator.MATH_MULTIPLY:
		case enumMathOperator.MATH_DIVIDE:
		case enumMathOperator.MATH_ADD:
		case enumMathOperator.MATH_SUBSTRACT:
			return 2;
	
		case enumMathOperator.MATH_LEFT_PARENTHESIS:
		case enumMathOperator.MATH_RIGHT_PARENTHESIS:
			return 0;
		default:
			return 0;
		}
	}
	//  
	private static string EnumOperatorToTChar(TExpressionTree.enumMathOperator eOperator)
	{
		switch (eOperator)
		{
		case enumMathOperator.MATH_POSITIVE:
			return (string)(TEXT("+"));
		case enumMathOperator.MATH_NEGATIVE:
			return (string)(TEXT("-"));
		case enumMathOperator.MATH_LEFT_PARENTHESIS:
			return (string)(TEXT("("));
		case enumMathOperator.MATH_RIGHT_PARENTHESIS:
			return (string)(TEXT(")"));
		case enumMathOperator.MATH_ADD:
			return (string)(TEXT("+"));
		case enumMathOperator.MATH_SUBSTRACT:
			return (string)(TEXT("-"));
		case enumMathOperator.MATH_MULTIPLY:
			return (string)(TEXT("*"));
		case enumMathOperator.MATH_DIVIDE:
			return (string)(TEXT("/"));
		case enumMathOperator.MATH_POWER:
			return (string)(TEXT("^"));
		case enumMathOperator.MATH_AND:
			return (string)(TEXT("&"));
		case enumMathOperator.MATH_OR:
			return (string)(TEXT("|"));
		case enumMathOperator.MATH_MOD:
			return (string)(TEXT("%"));
		default:
			throw TError
			{
				enumError.ERROR_WRONG_MATH_OPERATOR, (string)(TEXT("value:")) + To_string(eOperator)
			}
			break;
		}
	}

	// 返回运算符结合性 
	private bool isLeft2Right(enumMathOperator eOperator)
	{
		switch (eOperator)
		{
		case enumMathOperator.MATH_MOD: //%
		case enumMathOperator.MATH_AND: //&
		case enumMathOperator.MATH_OR: //|
		case enumMathOperator.MATH_MULTIPLY:
		case enumMathOperator.MATH_DIVIDE:
		case enumMathOperator.MATH_ADD:
		case enumMathOperator.MATH_SUBSTRACT:
			return true;
	
		case enumMathOperator.MATH_POSITIVE: //正负号为右结合
		case enumMathOperator.MATH_NEGATIVE:
		case enumMathOperator.MATH_POWER: //^
			return false;
			//函数和括号不计结合性
		default:
			return true;
		}
	}
	private bool inAssociativeLaws(enumMathOperator eOperator)
	{
		switch (eOperator)
		{
		case enumMathOperator.MATH_SQRT:
		case enumMathOperator.MATH_SIN:
		case enumMathOperator.MATH_COS:
		case enumMathOperator.MATH_TAN:
		case enumMathOperator.MATH_ARCSIN:
		case enumMathOperator.MATH_ARCCOS:
		case enumMathOperator.MATH_ARCTAN:
		case enumMathOperator.MATH_LN:
		case enumMathOperator.MATH_LOG10:
		case enumMathOperator.MATH_EXP:
	
		case enumMathOperator.MATH_POSITIVE: //正负号
		case enumMathOperator.MATH_NEGATIVE:
	
		case enumMathOperator.MATH_MOD: //%
		case enumMathOperator.MATH_AND: //&
		case enumMathOperator.MATH_OR: //|
		case enumMathOperator.MATH_POWER: //^
		case enumMathOperator.MATH_DIVIDE:
		case enumMathOperator.MATH_SUBSTRACT:
	
		case enumMathOperator.MATH_LEFT_PARENTHESIS:
		case enumMathOperator.MATH_RIGHT_PARENTHESIS:
			return false;
	
		case enumMathOperator.MATH_ADD:
		case enumMathOperator.MATH_MULTIPLY:
			return true;
		}
		Debug.Assert(0);
		return false;
	}

	// 返回运算符的优先级 
	private int Rank(enumMathOperator eOperator)
	{
		switch (eOperator)
		{
		case enumMathOperator.MATH_SQRT:
		case enumMathOperator.MATH_SIN:
		case enumMathOperator.MATH_COS:
		case enumMathOperator.MATH_TAN:
		case enumMathOperator.MATH_ARCSIN:
		case enumMathOperator.MATH_ARCCOS:
		case enumMathOperator.MATH_ARCTAN:
		case enumMathOperator.MATH_LN:
		case enumMathOperator.MATH_LOG10:
		case enumMathOperator.MATH_EXP:
			return 15;
	
		case enumMathOperator.MATH_POSITIVE: //除了函数，所有运算符均可将正负号挤出
		case enumMathOperator.MATH_NEGATIVE:
			return 14;
	
		case enumMathOperator.MATH_MOD: //%
			return 13;
	
		case enumMathOperator.MATH_AND: //&
		case enumMathOperator.MATH_OR: //|
			return 12;
	
		case enumMathOperator.MATH_POWER: //^
			return 11;
	
		case enumMathOperator.MATH_MULTIPLY:
		case enumMathOperator.MATH_DIVIDE:
			return 10;
	
		case enumMathOperator.MATH_ADD:
		case enumMathOperator.MATH_SUBSTRACT:
			return 5;
	
		case enumMathOperator.MATH_LEFT_PARENTHESIS: //左右括号优先级小是为了不被其余任何运算符挤出
		case enumMathOperator.MATH_RIGHT_PARENTHESIS:
			return 0;
		default:
			throw TError
			{
				enumError.ERROR_WRONG_MATH_OPERATOR, (string)(TEXT("VALUE:")) + To_string(eOperator)
			}
			break;
		}
	}


	// 是基本运算符()+-* /^&|% 
	private bool isBaseOperator(sbyte c)
	{
		switch (c)
		{
		case TEXT('('):
		case TEXT(')'):
		case TEXT('+'):
		case TEXT('-'):
		case TEXT('*'):
		case TEXT('/'):
		case TEXT('^'):
		case TEXT('&'):
		case TEXT('|'):
		case TEXT('%'):
			return true;
		}
		return false;
	}

	// 字符是0-9或. 
	private bool isDoubleChar(sbyte c)
	{
		if ((c >= TEXT('0') && c <= TEXT('9')) || c == TEXT('.'))
			return true;
		else
			return false;
	}

	// 有效性检查（返回0则出现异常字符） 
	private bool isLegal(sbyte c)
	{
		if (isDoubleChar(c))
			return true;
		if (isBaseOperator(c))
			return true;
		if (IsCharAlpha(c) || c == TEXT('_'))
			return true;
		return false;
	}


	//  
	private static TExpressionTree.enumMathOperator BaseOperatorCharToEnum(sbyte c)
	{
		switch (c)
		{
		case TEXT('('):
			return enumMathOperator.MATH_LEFT_PARENTHESIS;
		case TEXT(')'):
			return enumMathOperator.MATH_RIGHT_PARENTHESIS;
		case TEXT('+'):
			return enumMathOperator.MATH_ADD;
		case TEXT('-'):
			return enumMathOperator.MATH_SUBSTRACT;
		case TEXT('*'):
			return enumMathOperator.MATH_MULTIPLY;
		case TEXT('/'):
			return enumMathOperator.MATH_DIVIDE;
		case TEXT('^'):
			return enumMathOperator.MATH_POWER;
		case TEXT('&'):
			return enumMathOperator.MATH_AND;
		case TEXT('|'):
			return enumMathOperator.MATH_OR;
		case TEXT('%'):
			return enumMathOperator.MATH_MOD;
		default:
			return enumMathOperator.MATH_NULL;
		}
	}
	private static TExpressionTree.enumMathOperator Str2Function(string s)
	{
		if (s == TEXT("sin"))
		{
			return enumMathOperator.MATH_SIN;
		}
		if (s == TEXT("cos"))
		{
			return enumMathOperator.MATH_COS;
		}
		if (s == TEXT("tan"))
		{
			return enumMathOperator.MATH_TAN;
		}
		if (s == TEXT("arcsin"))
		{
			return enumMathOperator.MATH_ARCSIN;
		}
		if (s == TEXT("arccos"))
		{
			return enumMathOperator.MATH_ARCCOS;
		}
		if (s == TEXT("arctan"))
		{
			return enumMathOperator.MATH_ARCTAN;
		}
		if (s == TEXT("sqrt"))
		{
			return enumMathOperator.MATH_SQRT;
		}
		if (s == TEXT("ln"))
		{
			return enumMathOperator.MATH_LN;
		}
		if (s == TEXT("log10"))
		{
			return enumMathOperator.MATH_LOG10;
		}
		if (s == TEXT("exp"))
		{
			return enumMathOperator.MATH_EXP;
		}
		return enumMathOperator.MATH_NULL;
	}
	private static string Function2Str(TExpressionTree.enumMathOperator eOperator)
	{
		switch (eOperator)
		{
		case enumMathOperator.MATH_SIN:
			return TEXT("sin");
		case enumMathOperator.MATH_COS:
			return TEXT("cos");
		case enumMathOperator.MATH_TAN:
			return TEXT("tan");
		case enumMathOperator.MATH_ARCSIN:
			return TEXT("arcsin");
		case enumMathOperator.MATH_ARCCOS:
			return TEXT("arccos");
		case enumMathOperator.MATH_ARCTAN:
			return TEXT("arctan");
		case enumMathOperator.MATH_SQRT:
			return TEXT("sqrt");
		case enumMathOperator.MATH_LN:
			return TEXT("ln");
		case enumMathOperator.MATH_LOG10:
			return TEXT("log10");
		case enumMathOperator.MATH_EXP:
			return TEXT("exp");
		}
		throw TError
		{
			enumError.ERROR_WRONG_MATH_OPERATOR, (string)(TEXT("value=")) + To_string(eOperator)
		}
	}

	private TVariableTable pVariableTable;
	private int iVarAppearedCount;
	private TNode LastVarNode;
	//TVariableTable SelfVariableTable;

	//是整数 且 为偶数
	private bool IsIntAndEven(double n)
	{
		long i = (long)n;
		if (Math.Abs(n - i) < 1e-6)
			if (i % 2 == 0)
				return true;
		return false;
	}

	//由in order队列得到post order队列
	private void InQueue2PostQueue(ref Queue<TNode *> InOrder, ref List<TNode *> PostOrder)
	{
		int parenthesis_num = 0;
		Stack<TNode > temp = new Stack<TNode >();
		while (InOrder.Count > 0)
		{
			if (InOrder.Peek().eType == enumNodeType.NODE_NUMBER || InOrder.Peek().eType == enumNodeType.NODE_VARIABLE)
			{
				PostOrder.Add(InOrder.Peek()); //数字直接入栈
				InOrder.Dequeue();
			}
			else
			{
				if (InOrder.Peek().eOperator == enumMathOperator.MATH_LEFT_PARENTHESIS) //(左括号直接入栈
				{
					temp.Push(InOrder.Peek());
					InOrder.Dequeue();
					parenthesis_num++;
				}
				else
				{
					if (InOrder.Peek().eOperator == enumMathOperator.MATH_RIGHT_PARENTHESIS) //)出现右括号
					{
						parenthesis_num--;
						//pop至左括号
						while (temp.Count > 0)
						{
							if (temp.Peek().eOperator == enumMathOperator.MATH_LEFT_PARENTHESIS) //(
							{
								temp.Peek() = null;
								temp.Pop(); //扔掉左括号
								break;
							}
							else
							{
								PostOrder.Add(temp.Peek()); //入队
								temp.Pop();
							}
						}
	
						//取出函数
						if (temp.Count > 0 && temp.Peek().eType == enumNodeType.NODE_FUNCTION)
						{
							PostOrder.Add(temp.Peek());
							temp.Pop();
						}
	
						//pop所有取正取负
						while (temp.Count > 0)
						{
							if (temp.Peek().eOperator == enumMathOperator.MATH_POSITIVE || temp.Peek().eOperator == enumMathOperator.MATH_NEGATIVE)
							{
								PostOrder.Add(temp.Peek());
								temp.Pop();
							}
							else
								break;
						}
						InOrder.Peek() = null;
						InOrder.Dequeue(); //扔掉右括号
					}
					else //InOrder.front()不是括号
					{
						if (InOrder.Peek().eOperator == enumMathOperator.MATH_POSITIVE || InOrder.Peek().eOperator == enumMathOperator.MATH_NEGATIVE)
						{
							temp.Push(InOrder.Peek());
							InOrder.Dequeue();
						}
						else //不是括号也不是正负号
						{
							if (temp.Count > 0 && isLeft2Right(temp.Peek().eOperator) == true) //左结合
								while (temp.Count > 0 && Rank(InOrder.Peek().eOperator) <= Rank(temp.Peek().eOperator)) //临时栈有内容，且新进符号优先级低，则挤出高优先级及同优先级符号
								{
									PostOrder.Add(temp.Peek()); //符号进入post队列
									temp.Pop();
								}
							else //右结合
								while (temp.Count > 0 && Rank(InOrder.Peek().eOperator) < Rank(temp.Peek().eOperator)) //临时栈有内容，且新进符号优先级低，则挤出高优先级，但不挤出同优先级符号（因为右结合）
								{
									PostOrder.Add(temp.Peek()); //符号进入post队列
									temp.Pop();
								}
							temp.Push(InOrder.Peek()); //高优先级已全部挤出，当前符号入栈
							InOrder.Dequeue();
						}
					}
	
				}
			}
		}
	
		//剩下的元素全部入栈
		while (temp.Count > 0)
		{
			PostOrder.Add(temp.Peek());
			temp.Pop();
		}
		if (parenthesis_num != 0)
		{
			ReleaseVectorTNode(PostOrder);
			throw TError
			{
				enumError.ERROR_PARENTHESIS_NOT_MATCH, TEXT("")
			}
		}
	}
	private void ReadToInOrder(string expression, ref Queue<TNode *> InOrder)
	{
		if (string.IsNullOrEmpty(expression))
		{
			throw TError
			{
				enumError.ERROR_EMPTY_INPUT, expression
			}
			return;
		}
		Replace(expression, TEXT(" "), TEXT(""));
		Replace(expression, TEXT("\t"), TEXT(""));
		Replace(expression, TEXT("\r"), TEXT(""));
		Replace(expression, TEXT("\n"), TEXT(""));
	
		//过滤掉所有多余的加减
		Replace(expression, TEXT("--"), TEXT("+"));
		Replace(expression, TEXT("+-"), TEXT("-"));
		Replace(expression, TEXT("-+"), TEXT("-"));
	
		//字符合法性检查
		for (auto c : expression)
			if (!isLegal(c))
			{
				throw TError
				{
					enumError.ERROR_ILLEGALCHAR, (string)(TEXT("WRONG CHAR:")) + To_string(c)
				}
			}
	
		//粗切分：利用operator切分
//C++ TO C# CONVERTER TODO TASK: C# does not allow declaring types within methods:
//		struct TStrPiece
//		{
//			bool bBaseOperator;
//			string s;
//			TStrPiece(bool bIn, string sIn) :bBaseOperator(bIn), s(sIn)
//			{
//			}
//		};
		List<TStrPiece> Data = new List<TStrPiece>();
	
		string temp;
		for (auto c : expression)
		{
			if (!isBaseOperator(c))
			{
//C++ TO C# CONVERTER TODO TASK: The push_back method is not converted to C#:
				temp.push_back(c);
			}
			else
			{
				if (!string.IsNullOrEmpty(temp))
				{
					Data.emplace_back(false, temp);
//C++ TO C# CONVERTER TODO TASK: The clear method is not converted to C#:
					temp.clear();
				}
				Data.emplace_back(true, string{ c });
			}
		}
		if (!string.IsNullOrEmpty(temp))
		{
			Data.emplace_back(false, temp);
//C++ TO C# CONVERTER TODO TASK: The clear method is not converted to C#:
			temp.clear();
		}
	
	
		//二次切分：切分出4类元素
		//并送入Pre In order
		List<TNode > PreInOrder = new List<TNode >();
		TNode tempNode;
		//string tempTChar;
		enumMathOperator tempeOperator;
		for (uint i = 0; i < Data.Count; i++)
		{
			if (Data[i].bBaseOperator) //识别出基本运算符（括号也在其中）
			{
				tempNode = new TNode;
				tempNode.eType = enumNodeType.NODE_OPERATOR;
				tempNode.eOperator = BaseOperatorCharToEnum(Data[i].s[0]);
				PreInOrder.Add(tempNode);
			}
			else
			{
				//逐位检验是否为数字
				bool isDouble = true;
				for (auto c : Data[i].s)
					if (isDoubleChar(c) == false)
					{
						isDouble = false;
						break;
					}
	
				if (isDouble) //数字
				{
					tempNode = new TNode;
					tempNode.eType = enumNodeType.NODE_NUMBER;
					tempNode.value = std.stod(Data[i].s);
					PreInOrder.Add(tempNode);
				}
				else
				{
					if ((tempeOperator = Str2Function(Data[i].s)) != enumMathOperator.MATH_NULL) //识别出函数
					{
						tempNode = new TNode;
						tempNode.eType = enumNodeType.NODE_FUNCTION;
						tempNode.eOperator = tempeOperator;
						PreInOrder.Add(tempNode);
					}
					else //变量
					{
						//非运算符、数字、函数
	
						if (pVariableTable == null)
						{
							ReleaseVectorTNode(PreInOrder);
							throw TError
							{
								enumError.ERROR_NOT_LINK_VARIABLETABLE, TEXT("")
							}
							return;
						}
						if (!IsCharAlpha(Data[i].s[0]) && Data[i].s[0] != TEXT('_')) //变量名首字符需为下划线或字母
						{
							ReleaseVectorTNode(PreInOrder);
							throw TError
							{
								enumError.ERROR_INVALID_VARNAME, Data[i].s
							}
							return;
						}
	
	
						//
						if (pVariableTable.FindVariableTable(Data[i].s) == pVariableTable.VariableTable.end())
						{
							ReleaseVectorTNode(PreInOrder);
							throw TError
							{
								enumError.ERROR_UNDEFINED_VARIABLE, Data[i].s
							}
							return;
						}
	
						tempNode = new TNode;
						tempNode.eType = enumNodeType.NODE_VARIABLE;
						tempNode.varname = Data[i].s;
						PreInOrder.Add(tempNode);
	
						////得到自身的变量表 以解方程
						//if (SelfVariableTable.FindVariableTable(tempTChar) == NULL)
						//{
						//	SelfVariableTable.VariableTable.push_back(tempTChar);
						//	SelfVariableTable.VariableValue.push_back(pVariableTable->GetValueFromVarPoint(tempTChar));
						//}
						iVarAppearedCount++;
						LastVarNode = tempNode;
	
					}
				}
			}
		}
		//此时4大元素均已切分入PreInOrder
	
		//识别取正运算符与取负运算符
		bool bFirstOrParenFirst = false;
		bool bAferOneOperator = false;
		size_t i = 0;
		if (PreInOrder[0].eOperator == enumMathOperator.MATH_ADD)
		{
			PreInOrder[0].eOperator = enumMathOperator.MATH_POSITIVE;
			i++;
		}
		if (PreInOrder[0].eOperator == enumMathOperator.MATH_SUBSTRACT)
		{
			PreInOrder[0].eOperator = enumMathOperator.MATH_NEGATIVE;
			i++;
		}
		for (; i < PreInOrder.Count;)
		{
			if (PreInOrder[i].eType == enumNodeType.NODE_OPERATOR && PreInOrder[i].eOperator != enumMathOperator.MATH_RIGHT_PARENTHESIS)
			{
				if (i + 1 < PreInOrder.Count)
					i++;
				else
					break;
				if (PreInOrder[i].eOperator == enumMathOperator.MATH_ADD)
				{
					PreInOrder[i].eOperator = enumMathOperator.MATH_POSITIVE;
					i++;
					continue;
				}
				if (PreInOrder[i].eOperator == enumMathOperator.MATH_SUBSTRACT)
				{
					PreInOrder[i].eOperator = enumMathOperator.MATH_NEGATIVE;
					i++;
					continue;
				}
			}
			else
				i++;
		}
	
		for (auto pNode : PreInOrder)
		{
			InOrder.Enqueue(pNode);
		}
	}
	private static string Node2Str(TNode node)
	{
		switch (node.eType)
		{
		case enumNodeType.NODE_NUMBER:
			if (Math.Abs(node.value - (long)node.value) < 1e-6)
				return To_string((long)node.value);
			else
				return To_string(node.value);
			break;
		case enumNodeType.NODE_VARIABLE:
			return node.varname;
			break;
		case enumNodeType.NODE_FUNCTION:
			return Function2Str(node.eOperator);
			break;
		case enumNodeType.NODE_OPERATOR:
			return EnumOperatorToTChar(node.eOperator);
			break;
		}
		Debug.Assert(0);
		return TEXT("Error");
	}

	//将PostOrder建立为树，并进行表达式有效性检验（确保二元及一元运算符、函数均有操作数）
	private void BuildExpressionTree(ref List<TNode >[] PostOrder)
	{
		Stack<TNode > tempStack = new Stack<TNode >();
		//逐个识别PostOrder序列，构建表达式树
		for (auto pNodeNow : PostOrder)
		{
			switch (pNodeNow.eType)
			{
			case enumNodeType.NODE_NUMBER:
			case enumNodeType.NODE_VARIABLE:
				tempStack.Push(pNodeNow);
				break;
			case enumNodeType.NODE_FUNCTION:
			case enumNodeType.NODE_OPERATOR:
				if (GetOperateNum(pNodeNow.eOperator) == 2)
				{
					if (tempStack.Count == 0)
					{
						//释放所有TNode，报错
						ReleaseVectorTNode(PostOrder);
						//Stack只是对PostOrder的重排序，所以不用delete
						throw TError
						{
							enumError.ERROR_WRONG_EXPRESSION, TEXT("")
						}
						return;
					}
	
					pNodeNow.right = tempStack.Peek();
					tempStack.Peek().parent = pNodeNow;
					tempStack.Pop();
	
					if (tempStack.Count == 0)
					{
						//释放所有TNode，报错
						ReleaseVectorTNode(PostOrder);
						//Stack只是对PostOrder的重排序，所以不用delete
						throw TError
						{
							enumError.ERROR_WRONG_EXPRESSION, TEXT("")
						}
						return;
					}
	
					pNodeNow.left = tempStack.Peek();
					tempStack.Peek().parent = pNodeNow;
					tempStack.Pop();
	
					tempStack.Push(pNodeNow);
				}
				else
				{
					if (tempStack.Count == 0)
					{
						//释放所有TNode，报错
						ReleaseVectorTNode(PostOrder);
						//Stack只是对PostOrder的重排序，所以不用delete
						throw TError
						{
							enumError.ERROR_WRONG_EXPRESSION, TEXT("")
						}
						return;
					}
	
					pNodeNow.left = tempStack.Peek();
					tempStack.Peek().parent = pNodeNow;
					tempStack.Pop();
	
					tempStack.Push(pNodeNow);
				}
				break;
			}
		}
	
		//找出root
		head = PostOrder[0];
		while (head.parent != null)
		{
			head = head.parent;
		}
	}
	private void TraverseInOrder(TNode now, ref string output)
	{
		int has_parenthesis = 0;
		if (GetOperateNum(now.eOperator) == 1) //一元运算符：函数和取负
		{
			if (now.eType == enumNodeType.NODE_FUNCTION)
			{
				output += Node2Str(now) + TEXT("(");
				has_parenthesis = 1;
			}
			else
			{
				output += TEXT("(") + Node2Str(now);
				has_parenthesis = 1;
			}
		}
	
		if (GetOperateNum(now.eOperator) != 1) //非一元运算符才输出，即一元运算符的输出顺序已改变
		{
			if (now.eType == enumNodeType.NODE_OPERATOR) //本级为运算符
				if (now.parent != null)
										//本级为父级的右子树 且父级不满足结合律->加括号
										////两级都是右结合
						//||
						////父运算符存在，为除号，且本级为分子，则添加括号
						//(now->parent->eOperator == MATH_DIVIDE && now == now->parent->right)
					if ((GetOperateNum(now.parent.eOperator) == 2 && (Rank(now.parent.eOperator) > Rank(now.eOperator) || (Rank(now.parent.eOperator) == Rank(now.eOperator) && ((inAssociativeLaws(now.parent.eOperator) == false && now == now.parent.right) || (isLeft2Right(now.parent.eOperator) == false && isLeft2Right(now.eOperator) == false))))))
					{
						output += TEXT("(");
						has_parenthesis = 1;
					}
		}
	
		if (now.left != null) //左遍历
		{
			TraverseInOrder(now.left, ref output);
		}
	
		if (GetOperateNum(now.eOperator) != 1) //非一元运算符才输出，即一元运算符的输出顺序已改变
		{
			output += Node2Str(now);
		}
	
	
		if (now.right != null) //右遍历
		{
			TraverseInOrder(now.right, ref output);
		}
	
		//回到本级时补齐右括号，包住前面的东西
		if (has_parenthesis != 0)
		{
			output += TEXT(")");
		}
	}
	private void CalcNode(TNode Operator, TNode Node1)
	{
		CalcNode(Operator, Node1, null);
	}
//C++ TO C# CONVERTER NOTE: Overloaded method(s) are created above to convert the following method having default parameters:
//ORIGINAL LINE: void CalcNode(TNode *Operator, const TNode *Node1, const TNode *Node2 = null)
	private void CalcNode(TNode Operator, TNode Node1, TNode Node2)
	{
		double value1 = Node1.value;
		double value2 = (Node2 != null ? Node2.value : 0.0);
		Operator.eType = enumNodeType.NODE_NUMBER;
		switch (Operator.eOperator)
		{
		case enumMathOperator.MATH_SQRT:
			if (value1 < 0.0)
			{
				//恢复修改并抛出异常
				Operator.eType = enumNodeType.NODE_FUNCTION;
				throw TError
				{
					enumError.ERROR_I, (string)(TEXT("sqrt(")) + To_string(value1) + (string)(TEXT(")"))
				}
				return;
			}
			Operator.value = Math.Sqrt(value1);
			break;
		case enumMathOperator.MATH_SIN:
			Operator.value = Math.Sin(value1);
			break;
		case enumMathOperator.MATH_COS:
			Operator.value = Math.Cos(value1);
			break;
		case enumMathOperator.MATH_TAN:
		{
			//x!=k*pi+pi/2 -> 2*x/pi != 2*k+1(odd)
			double @value = value1 * 2.0 / M_PI;
			if (Math.Abs(@value - (int)@value) < 1e-6 && (int)@value % 2 != 1)
			{
				//恢复修改并抛出异常
				Operator.eType = enumNodeType.NODE_FUNCTION;
				throw TError
				{
					enumError.ERROR_OUTOF_DOMAIN, (string)(TEXT("tan(")) + To_string(@value) + (string)(TEXT(")"))
				}
				return;
			}
			Operator.value = Math.Tan(value1);
			break;
		}
		case enumMathOperator.MATH_ARCSIN:
			if (value1 < -1.0 || value1 > 1.0)
			{
				//恢复修改并抛出异常
				Operator.eType = enumNodeType.NODE_FUNCTION;
				throw TError
				{
					enumError.ERROR_OUTOF_DOMAIN, (string)(TEXT("arcsin(")) + To_string(value1) + (string)(TEXT(")"))
				}
				return;
			}
			Operator.value = Math.Asin(value1);
			break;
		case enumMathOperator.MATH_ARCCOS:
			if (value1 < -1.0 || value1 > 1.0)
			{
				//恢复修改并抛出异常
				Operator.eType = enumNodeType.NODE_FUNCTION;
				throw TError
				{
					enumError.ERROR_OUTOF_DOMAIN, (string)(TEXT("arccos(")) + To_string(value1) + (string)(TEXT(")"))
				}
			}
			Operator.value = Math.Acos(value1);
			break;
		case enumMathOperator.MATH_ARCTAN:
			Operator.value = Math.Atan(value1);
			break;
		case enumMathOperator.MATH_LN:
			if (value1 < 1e-6)
			{
				//恢复修改并抛出异常
				Operator.eType = enumNodeType.NODE_FUNCTION;
				throw TError
				{
					enumError.ERROR_OUTOF_DOMAIN, (string)(TEXT("ln(")) + To_string(value1) + (string)(TEXT(")"))
				}
			}
			Operator.value = Math.Log(value1);
			break;
		case enumMathOperator.MATH_LOG10:
			if (value1 < 1e-6) //log(0)或log(负数)
			{
				//恢复修改并抛出异常
				Operator.eType = enumNodeType.NODE_FUNCTION;
				throw TError
				{
					enumError.ERROR_OUTOF_DOMAIN, (string)(TEXT("log10(")) + To_string(value1) + (string)(TEXT(")"))
				}
			}
			Operator.value = Math.Log10(value1);
			break;
		case enumMathOperator.MATH_EXP:
			Operator.value = Math.Exp(value1);
			break;
		case enumMathOperator.MATH_POSITIVE:
			break;
		case enumMathOperator.MATH_NEGATIVE:
			Operator.value = -value1;
			break;
	
		case enumMathOperator.MATH_MOD: //%
			if ((int)value2 == 0)
				throw TError
				{
					enumError.ERROR_DIVIDE_ZERO, To_string(value2)
				}
			Operator.value = (int)value1 % (int)value2;
			break;
		case enumMathOperator.MATH_AND: //&
			Operator.value = (int)value1 & (int)value2;
			break;
		case enumMathOperator.MATH_OR: //|
			Operator.value = (int)value1 | (int)value2;
			break;
	
		case enumMathOperator.MATH_POWER: //^
			//0^0
			if (Math.Abs(value1) < 1e-6 && Math.Abs(value2) < 1e-6)
			{
				Operator.eType = enumNodeType.NODE_OPERATOR;
				throw TError
				{
					enumError.ERROR_ZERO_POWEROF_ZERO, TEXT("")
				}
			}
	
			//(-1)^0.5=i
			if (value1 < 0 && IsIntAndEven(1L / value2))
			{
				Operator.eType = enumNodeType.NODE_OPERATOR;
				throw TError
				{
					enumError.ERROR_I, (string)(TEXT("pow(")) + To_string(value1) + TEXT(",") + To_string(value2) + TEXT(")")
				}
			}
			Operator.value = Math.Pow(value1, value2);
			break;
	
		case enumMathOperator.MATH_MULTIPLY:
			Operator.value = value1 * value2;
			break;
		case enumMathOperator.MATH_DIVIDE:
			if (Math.Abs(value2) < 1e-6)
			{
				Operator.eType = enumNodeType.NODE_OPERATOR;
				throw TError
				{
					enumError.ERROR_DIVIDE_ZERO, TEXT("")
				}
			}
			Operator.value = value1 / value2;
			break;
	
		case enumMathOperator.MATH_ADD:
			Operator.value = value1 + value2;
			break;
		case enumMathOperator.MATH_SUBSTRACT:
			Operator.value = value1 - value2;
			break;
		}
		Operator.eOperator = enumMathOperator.MATH_NULL;
	}
	private void Simplify(TNode now)
	{
		//左遍历
		if (now.left != null)
			Simplify(now.left);
	
		//右遍历
		if (now.right != null)
			Simplify(now.right);
	
		//化简
		//OutputStr();
		if (GetOperateNum(now.eOperator) == 1)
		{
			bool ChildIs0 = (now.left.eType == enumNodeType.NODE_NUMBER && Math.Abs(now.left.value) < 1e-6);
			bool ChildIs1 = (now.left.eType == enumNodeType.NODE_NUMBER && Math.Abs(now.left.value - 1) < 1e-6);
	
			//sin(0)=0
			if (now.eOperator == enumMathOperator.MATH_SIN && ChildIs0)
			{
				LinkParent(now.left, now);
				now.left.value = 0;
				now.Dispose();
			}
	
			//cos(0)=1
			if (now.eOperator == enumMathOperator.MATH_COS && ChildIs0)
			{
				LinkParent(now.left, now);
				now.left.value = 1;
				now.Dispose();
			}
	
			if (now.eOperator == enumMathOperator.MATH_NEGATIVE && now.left.eType == enumNodeType.NODE_NUMBER)
			{
				TNode negative = now;
				TNode num = now.left;
				LinkParent(num, negative);
				num.value = -num.value;
				negative.Dispose();
			}
		}
	
		if (GetOperateNum(now.eOperator) == 2)
		{
			//下列每种情况必须互斥，因为每个情况都有返回值，涉及删除操作，若不返回连续执行将导致指针出错
			//不检查左右儿子是否存在，因为此处本身就是2元运算符
			bool LChildIs0 = (now.left.eType == enumNodeType.NODE_NUMBER && Math.Abs(now.left.value) < 1e-6);
			bool RChildIs0 = (now.right.eType == enumNodeType.NODE_NUMBER && Math.Abs(now.right.value) < 1e-6);
			bool LChildIs1 = (now.left.eType == enumNodeType.NODE_NUMBER && Math.Abs(now.left.value - 1) < 1e-6);
			bool RChildIs1 = (now.right.eType == enumNodeType.NODE_NUMBER && Math.Abs(now.right.value - 1) < 1e-6);
	
			//被除数为0
			if (now.eOperator == enumMathOperator.MATH_DIVIDE && RChildIs0)
				throw TError
				{
					enumError.ERROR_DIVIDE_ZERO, TEXT("")
				}
	
			//0的0次方
			if (now.eOperator == enumMathOperator.MATH_POWER && LChildIs0 && RChildIs0)
				throw TError
				{
					enumError.ERROR_ZERO_POWEROF_ZERO, TEXT("")
				}
	
			//若左右儿子都是数字，则计算出来
			if (now.left.eType == enumNodeType.NODE_NUMBER && now.right.eType == enumNodeType.NODE_NUMBER)
			{
				//try
				//{
				CalcNode(now, now.left, now.right);
				now.left = null;
				now.right = null;
				now.eOperator = enumMathOperator.MATH_NULL;
				now.left = null;
				now.right = null;
				//}
				//catch (enumError err)
				//{
				//	return nowError = err;
				//}
			}
	
			//任何节点的0次方均等于1，除了0的0次方已在前面报错
			if (now.eOperator == enumMathOperator.MATH_POWER && RChildIs0)
			{
				//替换掉当前运算符，这个1节点将在回溯时处理
				//新建一个1节点
				TNode temp;
				temp = new TNode;
				temp.eType = enumNodeType.NODE_NUMBER;
				temp.value = 1;
	
				//0节点连接到上面
				if (now != head)
				{
					if (now.parent.left == now)
					{
						now.parent.left = temp;
						temp.parent = now.parent;
					}
					if (now.parent.right == now)
					{
						now.parent.right = temp;
						temp.parent = now.parent;
					}
				}
				else
					head = temp;
	
				DeleteNode(now);
			}
	
			//任何数乘或被乘0、被0除、0的除0外的任何次方，等于0
			if ((now.eOperator == enumMathOperator.MATH_MULTIPLY && (LChildIs0 || RChildIs0)) || (now.eOperator == enumMathOperator.MATH_DIVIDE && LChildIs0) || (now.eOperator == enumMathOperator.MATH_POWER && LChildIs0))
			{
				//替换掉当前运算符，这个0节点将在回溯时处理
				//新建一个0节点
				TNode temp;
				temp = new TNode;
				temp.eType = enumNodeType.NODE_NUMBER;
				temp.value = 0;
	
				//0节点连接到上面
				if (now != head)
				{
					if (now.parent.left == now)
					{
						now.parent.left = temp;
						temp.parent = now.parent;
					}
					if (now.parent.right == now)
					{
						now.parent.right = temp;
						temp.parent = now.parent;
					}
				}
				else
					head = temp;
	
				DeleteNode(now);
			}
	
			//0-x=-x
			if (now.eOperator == enumMathOperator.MATH_SUBSTRACT && LChildIs0)
			{
				TNode LChild = now.left;
				TNode RChild = now.right;
				now.eOperator = enumMathOperator.MATH_NEGATIVE;
				now.left = RChild;
				now.right = null;
	
				LChild.Dispose();
			}
	
			//任何数加或被加0、被减0、乘或被乘1、被1除、开1次方，等于自身
			if ((now.eOperator == enumMathOperator.MATH_ADD && (LChildIs0 || RChildIs0)) || (now.eOperator == enumMathOperator.MATH_SUBSTRACT && RChildIs0) || (now.eOperator == enumMathOperator.MATH_MULTIPLY && (LChildIs1 || RChildIs1)) || (now.eOperator == enumMathOperator.MATH_DIVIDE && RChildIs1) || (now.eOperator == enumMathOperator.MATH_POWER && RChildIs1))
			{
				TNode remain = null;
				TNode num = null;
				if (LChildIs1 || LChildIs0)
				{
					num = now.left;
					remain = now.right;
				}
				if (RChildIs1 || RChildIs0)
				{
					num = now.right;
					remain = now.left;
				}
	
				//连接父级和剩余项
				if (now != head)
				{
					if (now.parent.left == now)
					{
						now.parent.left = remain;
						remain.parent = now.parent;
					}
					if (now.parent.right == now)
					{
						now.parent.right = remain;
						remain.parent = now.parent;
					}
				}
				else
				{
					head = remain;
					head.parent = null;
				}
				num.Dispose();
				now.Dispose();
			}
	
	
		}
	
	}
	private void GetNodeNum(TNode now, ref int n)
	{
		if (now.left != null)
			GetNodeNum(now.left, ref n);
		if (now.right != null)
			GetNodeNum(now.right, ref n);
		n++;
	}
	private int GetNodeNum(TNode head)
	{
		int num = 0;
		if (head != 0)
		{
			GetNodeNum(head, ref num);
			return num;
		}
		else
			return 0;
	}

	//删除node指向对象 可删除任意位置节点，如被删节点存在父节点则父节点左右儿子置0
	private void DeleteNode(TNode node)
	{
		if (node != null)
		{
			if (node.parent != null)
			{
				if (node.parent.left == node)
					node.parent.left = null;
				if (node.parent.right == node)
					node.parent.right = null;
			}
	
			DeleteNodeTraversal(node);
		}
	}

	//遍历删除节点 未处理父节点，未判断左右儿子是否存在
	private void DeleteNodeTraversal(TNode node)
	{
		if (node.left != null)
			DeleteNode(node.left);
		if (node.right != null)
			DeleteNode(node.right);
	
		node.Dispose();
	}

	//未完成求导：tan,arcsin,arccos,arctan
	private void Diff(TNode now, string var)
	{
		switch (now.eType)
		{
		case enumNodeType.NODE_VARIABLE:
			now.eType = enumNodeType.NODE_NUMBER;
			if (now.varname == var)
				now.value = 1;
			else
				now.value = 0;
			return;
		case enumNodeType.NODE_NUMBER:
			now.value = 0;
			return;
		case enumNodeType.NODE_OPERATOR: //当前为运算符节点
			switch (now.eOperator)
			{
			case enumMathOperator.MATH_POSITIVE:
			case enumMathOperator.MATH_NEGATIVE:
				if (now.left != null)
					Diff(now.left, var);
				return;
			case enumMathOperator.MATH_ADD:
			case enumMathOperator.MATH_SUBSTRACT:
				if (now.left != null)
					Diff(now.left, var);
				if (now.right != null)
					Diff(now.right, var);
				return;
			case enumMathOperator.MATH_MULTIPLY:
				if (now.left.eType == enumNodeType.NODE_NUMBER || now.right.eType == enumNodeType.NODE_NUMBER) //两个操作数中有一个是数字
				{
					if (now.left.eType == enumNodeType.NODE_NUMBER)
						Diff(now.right, var);
					else
						Diff(now.left, var);
				}
				else
				{
					TNode plus;
					plus = new TNode;
					plus.eType = enumNodeType.NODE_OPERATOR;
					plus.eOperator = enumMathOperator.MATH_ADD;
					if (now != head)
					{
						//plus上下行连接
						if (now.parent.left == now)
							now.parent.left = plus;
						if (now.parent.right == now)
							now.parent.right = plus;
						plus.parent = now.parent;
					}
					else
					{
						head = plus;
					}
					now.parent = plus;
					plus.left = now;
	
					//加入右节点
					TNode newRight;
					newRight = CopyNodeTree(now);
	
					plus.right = newRight;
					newRight.parent = plus;
	
					Diff(plus.left.left, var);
					Diff(plus.right.right, var);
				}
				return;
			case enumMathOperator.MATH_DIVIDE:
				if (now.right.eType == enumNodeType.NODE_NUMBER) // f(x)/number = f'(x)/number
				{
					Diff(now.left, var);
				}
				else
				{
					TNode divide = now;
					TNode u1 = now.left;
					TNode v1 = now.right;
	
					//创建减号
					TNode substract;
					substract = NewNode(enumNodeType.NODE_OPERATOR, enumMathOperator.MATH_SUBSTRACT);
	
					//创建2个乘号
					TNode multiply1;
					TNode multiply2;
					multiply1 = NewNode(enumNodeType.NODE_OPERATOR, enumMathOperator.MATH_MULTIPLY);
					multiply2 = NewNode(enumNodeType.NODE_OPERATOR, enumMathOperator.MATH_MULTIPLY);
	
					//创建乘方
					TNode power;
					power = NewNode(enumNodeType.NODE_OPERATOR, enumMathOperator.MATH_POWER);
	
					//连接除号下面2个节点：-, ^
					divide.left = substract;
					substract.parent = divide;
					divide.right = power;
					power.parent = divide;
	
					//连接减号下面2个节点
					substract.left = multiply1;
					multiply1.parent = substract;
					substract.right = multiply2;
					multiply2.parent = substract;
	
					//连接乘号1下面2个节点：u1, v1
					multiply1.left = u1;
					u1.parent = multiply1;
					multiply1.right = v1;
					v1.parent = multiply1;
	
					//创建u2, v2
					TNode u2;
					TNode v2;
					u2 = CopyNodeTree(u1);
					v2 = CopyNodeTree(v1);
	
					//连接乘号2下面的u2, v2
					multiply2.left = u2;
					u2.parent = multiply2;
					multiply2.right = v2;
					v2.parent = multiply2;
	
					//创建v3, 2
					TNode v3;
					TNode num2;
					v3 = CopyNodeTree(v1);
					num2 = NewNode(enumNodeType.NODE_NUMBER);
					num2.value = 2;
	
					//连接^下面的v3和2
					power.left = v3;
					v3.parent = power;
					power.right = num2;
					num2.parent = power;
	
					Diff(u1, var);
					Diff(v2, var);
	
				}
				return;
			case enumMathOperator.MATH_POWER:
			{
				bool LChildIsNumber = now.left.eType == enumNodeType.NODE_NUMBER;
				bool RChildIsNumber = now.right.eType == enumNodeType.NODE_NUMBER;
				if (LChildIsNumber && RChildIsNumber)
				{
					now.left = null;
					now.right = null;
					now.left = null;
					now.right = null;
					now.eType = enumNodeType.NODE_NUMBER;
					now.eOperator = enumMathOperator.MATH_NULL;
					now.value = 0.0;
					return;
				}
	
				TNode multiply1 = NewNode(enumNodeType.NODE_OPERATOR, enumMathOperator.MATH_MULTIPLY);
				TNode multiply2 = NewNode(enumNodeType.NODE_OPERATOR, enumMathOperator.MATH_MULTIPLY);
				TNode power = now;
				TNode u = now.left;
				TNode v = now.right;
				TNode u2 = CopyNodeTree(u);
				TNode v2 = CopyNodeTree(v);
	
				if (power == head)
				{
					head = multiply1;
				}
				else
				{
					if (power.parent.left == power)
						power.parent.left = multiply1;
					if (power.parent.right == power)
						power.parent.right = multiply1;
					multiply1.parent = power.parent;
				}
	
				if (RChildIsNumber)
					v.value -= 1.0;
	
				multiply1.left = power;
				power.parent = multiply1;
	
				multiply1.right = multiply2;
				multiply2.parent = multiply1;
	
				multiply2.left = v2;
				v2.parent = multiply2;
	
				if (RChildIsNumber)
				{
					multiply2.right = u2;
					u2.parent = multiply2;
					Diff(u2, var);
					return;
				}
				else
				{
					TNode ln = NewNode(enumNodeType.NODE_FUNCTION, enumMathOperator.MATH_LN);
	
					multiply2.right = ln;
					ln.parent = multiply2;
	
					ln.left = u2;
					u2.parent = ln;
	
					Diff(multiply2, var);
					return;
				}
				return;
			}
			}
			break;
		case enumNodeType.NODE_FUNCTION:
		{
			//不考虑定义域
			//函数内为数字则导为0
			if (now.left.eType == enumNodeType.NODE_NUMBER)
			{
				now.eType = enumNodeType.NODE_NUMBER;
				now.eOperator = enumMathOperator.MATH_NULL;
				now.value = 0;
				DeleteNode(now.left);
				now.left = null;
				return;
			}
	
			TNode function = now;
			switch (function.eOperator)
			{
			case enumMathOperator.MATH_SQRT:
			{
				//转化为幂求导
				TNode u = function.left;
				TNode power = NewNode(enumNodeType.NODE_OPERATOR, enumMathOperator.MATH_POWER);
				TNode num1half = NewNode(enumNodeType.NODE_NUMBER);
				num1half.value = 0.5;
	
				power.left = u;
				u.parent = power;
	
				power.right = num1half;
				num1half.parent = power;
	
				if (function == head)
					head = power;
				else
				{
					if (function.parent.left == function)
						function.parent.left = power;
					if (function.parent.right == function)
						function.parent.right = power;
					power.parent = function.parent;
				}
	
				function.Dispose();
				Diff(power, var);
	
				return;
			}
			case enumMathOperator.MATH_LN:
			case enumMathOperator.MATH_LOG10:
			{
				TNode divide = NewNode(enumNodeType.NODE_OPERATOR, enumMathOperator.MATH_DIVIDE);
				TNode num1 = NewNode(enumNodeType.NODE_NUMBER);
				num1.value = 1.0;
	
				divide.left = num1;
				num1.parent = divide;
	
				TNode u = function.left;
	
				if (function.eOperator == enumMathOperator.MATH_LN) //ln(x)=1/x
				{
					divide.right = u;
					u.parent = divide;
				}
				else
				{
					//log10(x)=1/(x*ln(10))
					TNode multiply2 = NewNode(enumNodeType.NODE_OPERATOR, enumMathOperator.MATH_MULTIPLY);
	
					divide.right = multiply2;
					multiply2.parent = divide;
	
					multiply2.left = u;
					u.parent = multiply2;
	
					TNode ln = NewNode(enumNodeType.NODE_FUNCTION, enumMathOperator.MATH_LN);
	
					multiply2.right = ln;
					ln.parent = multiply2;
	
					TNode num10 = NewNode(enumNodeType.NODE_NUMBER);
					num10.value = 10.0;
	
					ln.left = num10;
					num10.parent = ln;
				}
	
				TNode top = divide;
				TNode u2 = null;
				if (u.eType != enumNodeType.NODE_VARIABLE)
				{
					TNode multiply1 = NewNode(enumNodeType.NODE_OPERATOR, enumMathOperator.MATH_MULTIPLY);
					u2 = CopyNodeTree(u);
	
					multiply1.left = divide;
					divide.parent = multiply1;
	
					multiply1.right = u2;
					u2.parent = multiply1;
	
					top = multiply1;
				}
	
				if (function == head)
				{
					head = top;
				}
				else
				{
					if (function.parent.left == function)
					{
						function.parent.left = top;
						top.parent = function.parent;
					}
					if (function.parent.right == function)
					{
						function.parent.right = top;
						top.parent = function.parent;
					}
				}
				function.Dispose();
	
				if (u.eType != enumNodeType.NODE_VARIABLE)
					Diff(u2, var);
	
			}
			return;
			case enumMathOperator.MATH_EXP:
			{
				if (function.left.eType == enumNodeType.NODE_VARIABLE) //e^x=e^x
					return;
				TNode multiply = NewNode(enumNodeType.NODE_OPERATOR, enumMathOperator.MATH_MULTIPLY);
				TNode u2 = CopyNodeTree(function.left);
	
				if (function == head)
				{
					head = multiply;
				}
				else
				{
					if (function.parent.left == function)
					{
						function.parent.left = multiply;
						multiply.parent = function.parent;
					}
					if (function.parent.right == function)
					{
						function.parent.right = multiply;
						multiply.parent = function.parent;
					}
				}
	
				multiply.left = function;
				function.parent = multiply;
	
				multiply.right = u2;
				u2.parent = multiply;
	
				Diff(u2, var);
			}
			return;
			case enumMathOperator.MATH_COS:
			{
				TNode negative = new TNode;
				negative.eType = enumNodeType.NODE_OPERATOR;
				negative.eOperator = enumMathOperator.MATH_NEGATIVE;
	
				//连接上一级和负号
				if (function != head)
				{
					if (function.parent.left == function)
					{
						function.parent.left = negative;
						negative.parent = function.parent;
					}
					if (function.parent.right == function)
					{
						function.parent.right = negative;
						negative.parent = function.parent;
					}
				}
				else
				{
					head = negative;
					negative.parent = null;
				}
	
				TNode multiply = new TNode;
				multiply.eType = enumNodeType.NODE_OPERATOR;
				multiply.eOperator = enumMathOperator.MATH_MULTIPLY;
	
				//连接负号和乘号
				negative.left = multiply;
				multiply.parent = negative;
	
				//连接乘号和function
				multiply.left = function;
				function.parent = multiply;
	
				//变更function
				function.eOperator = enumMathOperator.MATH_SIN;
	
				//复制u2并连接乘号
				TNode u2 = CopyNodeTree(function.left);
				multiply.right = u2;
				u2.parent = multiply;
	
				Diff(u2, var);
			}
			return;
			case enumMathOperator.MATH_SIN:
			{
				TNode multiply = new TNode;
				multiply.eType = enumNodeType.NODE_OPERATOR;
				multiply.eOperator = enumMathOperator.MATH_MULTIPLY;
	
				//连接上一级和乘号
				if (function != head)
				{
					if (function.parent.left == function)
					{
						function.parent.left = multiply;
						multiply.parent = function.parent;
					}
					if (function.parent.right == function)
					{
						function.parent.right = multiply;
						multiply.parent = function.parent;
					}
				}
				else
				{
					head = multiply;
					multiply.parent = null;
				}
	
				//连接乘号和function
				multiply.left = function;
				function.parent = multiply;
	
				//变更function
				function.eOperator = enumMathOperator.MATH_COS;
	
				//复制u2并连接乘号
				TNode u2 = CopyNodeTree(function.left);
				multiply.right = u2;
				u2.parent = multiply;
	
				Diff(u2, var);
			}
			//case MATH_ARCTAN:
			//{
			//	TNode *multiply = new TNode()
			//}
			return;
			default:
				throw new TError(enumError.ERROR_WRONG_MATH_OPERATOR, (string)(TEXT("未完成的运算符")) + Function2Str(now.eOperator));
			}
		}
		break;
		}
	}

	//复制节点树，返回新节点树头节点
	private TExpressionTree.TNode CopyNodeTree(TNode oldNode)
	{
		TNode newNode = new TNode;
		newNode.eType = oldNode.eType;
		newNode.eOperator = oldNode.eOperator;
		newNode.value = oldNode.value;
		newNode.varname = oldNode.varname;
	
		if (oldNode.left != null)
		{
			newNode.left = CopyNodeTree(oldNode.left);
			newNode.left.parent = newNode;
		}
		if (oldNode.right != null)
		{
			newNode.right = CopyNodeTree(oldNode.right);
			newNode.right.parent = newNode;
		}
	
		return newNode;
	}
	//String  FindVariableTableFrom(const String varstr, std::vector<String > newVariableTable);//查找变量是否在变量表中，没有则返回NULL
	private void GetVariablePos(TNode now, string var, ref List<TNode *> VarsPos)
	{
		if (now.eType == enumNodeType.NODE_VARIABLE && now.varname == var)
			VarsPos.Add(now);
		if (now.left != null)
			GetVariablePos(now.left, var, ref VarsPos);
		if (now.right != null)
			GetVariablePos(now.right, var, ref VarsPos);
	}

	//string  TExpressionTree::FindVariableTableFrom(const string varstr, std::vector<string > newVariableTable)
	//{
	//	for (auto szNewVar : newVariableTable)
	//		if (_tcscmp(varstr, szNewVar) == 0)
	//			return szNewVar;
	//	return NULL;
	//}
	//
	//void TExpressionTree::ReplaceNodeVariable(TNode *now, std::vector<string > &newVariableTable)
	//{
	//	if (now->left != NULL)
	//		ReplaceNodeVariable(now->left, newVariableTable);
	//	if (now->right != NULL)
	//		ReplaceNodeVariable(now->right, newVariableTable);
	//	TCHAR *temp;
	//	if (now->eType == NODE_VARIABLE && (temp = FindVariableTableFrom(now->varname.c_str(), newVariableTable)))
	//		now->varname = temp;
	//}
	
	private void GetVariablePos(string var, ref List<TNode *> VarsPos)
	{
		GetVariablePos(head, var, ref VarsPos);
	}
	private void CopyVariableTable(ref List<string > Dest, List<string > source)
	{
		Dest.Clear();
		for (auto sz : source)
			Dest.Add(sz);
	}
	//void ReplaceNodeVariable(TNode *now, std::vector<String > &newVariableTable);
	private bool CanCalc(TNode now)
	{
		if (now.left != null)
			if (CanCalc(now.left) == false)
				return false;
		if (now.right != null)
			if (CanCalc(now.right) == false)
				return false;
	
		if (now.eType == enumNodeType.NODE_VARIABLE)
			return false;
		return true;
	}

	//迭代计算，最终剩下1个节点
	private void Calc(TNode now)
	{
		if (now.left != null)
			Calc(ref now.left);
		if (now.right != null)
			Calc(ref now.right);
	
		if (GetOperateNum(now.eOperator) == 1 && now.left.eType == enumNodeType.NODE_NUMBER)
		{
			try
			{
				CalcNode(now, now.left);
			}
			catch (enumError &err)
			{
				throw err;
			}
			now.left = null;
			now.left = null;
		}
	
		if (GetOperateNum(now.eOperator) == 2 && now.left.eType == enumNodeType.NODE_NUMBER && now.right.eType == enumNodeType.NODE_NUMBER)
		{
			try
			{
				CalcNode(now, now.left, now.right);
			}
			catch (enumError &err)
			{
				throw err;
			}
			now.left = null;
			now.right = null;
			now.left = null;
			now.right = null;
		}
	}
	private void LinkParent(TNode child, TNode ignore)
	{
		if (ignore == head)
		{
			head = child;
			child.parent = null;
		}
		else
		{
			if (ignore.parent.left == ignore)
				ignore.parent.left = child;
			if (ignore.parent.right == ignore)
				ignore.parent.right = child;
			child.parent = ignore.parent;
		}
	}
	private TExpressionTree.TNode NewNode(enumNodeType eType)
	{
		return NewNode(eType, enumMathOperator.MATH_NULL);
	}
//C++ TO C# CONVERTER NOTE: Overloaded method(s) are created above to convert the following method having default parameters:
//ORIGINAL LINE: TExpressionTree::TNode *NewNode(enumNodeType eType, enumMathOperator eOperator = MATH_NULL)
	private TExpressionTree.TNode NewNode(enumNodeType eType, enumMathOperator eOperator)
	{
		TNode newNode = new TNode;
		newNode.eType = eType;
		newNode.eOperator = eOperator;
		return newNode;
	}
	private void ReleaseVectorTNode(List<TNode *> vec)
	{
		for (auto pNode : vec)
			pNode = null;
	}
	private void Vpa_inner(TNode now)
	{
		if (now.left != null)
			Vpa_inner(now.left);
		if (now.right != null)
			Vpa_inner(now.right);
	
		if (now.eType == enumNodeType.NODE_VARIABLE)
		{
			now.eType = enumNodeType.NODE_NUMBER;
			now.value = pVariableTable.GetValueFromVarPoint(now.varname);
		}
	}
	private void Solve(TNode now, ref TNode write_pos)
	{
		TNode parent = now.parent;
		if (parent != null)
		{
			TNode brother;
			bool bVarIsLeft;
			if (parent.left == now)
			{
				brother = parent.right;
				bVarIsLeft = true;
			}
			else
			{
				brother = parent.left;
				bVarIsLeft = false;
			}
	
			switch (parent.eOperator)
			{
			case enumMathOperator.MATH_ADD:
				write_pos.eType = enumNodeType.NODE_OPERATOR;
				write_pos.eOperator = enumMathOperator.MATH_SUBSTRACT;
	
				write_pos.right = CopyNodeTree(brother);
				write_pos.right.parent = write_pos;
	
				write_pos.left = new TNode;
	
				write_pos.left.parent = write_pos;
				Solve(ref parent, ref write_pos.left);
				break;
			case enumMathOperator.MATH_MULTIPLY:
				write_pos.eType = enumNodeType.NODE_OPERATOR;
				write_pos.eOperator = enumMathOperator.MATH_DIVIDE;
	
				write_pos.right = CopyNodeTree(brother);
				write_pos.right.parent = write_pos;
	
				write_pos.left = new TNode;
	
				write_pos.left.parent = write_pos;
				Solve(ref parent, ref write_pos.left);
				break;
			case enumMathOperator.MATH_SUBSTRACT: //分左右
				if (bVarIsLeft) //被减数
				{
					write_pos.eType = enumNodeType.NODE_OPERATOR;
					write_pos.eOperator = enumMathOperator.MATH_ADD;
	
					write_pos.right = CopyNodeTree(brother);
					write_pos.right.parent = write_pos;
	
					write_pos.left = new TNode;
	
					write_pos.left.parent = write_pos;
					Solve(ref parent, ref write_pos.left);
				}
				else
				{
					write_pos.eType = enumNodeType.NODE_OPERATOR;
					write_pos.eOperator = enumMathOperator.MATH_SUBSTRACT;
	
					write_pos.left = CopyNodeTree(brother);
					write_pos.left.parent = write_pos;
	
					write_pos.right = new TNode;
	
					write_pos.right.parent = write_pos;
					Solve(ref parent, ref write_pos.right);
				}
				break;
			case enumMathOperator.MATH_DIVIDE: //分左右
				if (bVarIsLeft) //被除数
				{
					write_pos.eType = enumNodeType.NODE_OPERATOR;
					write_pos.eOperator = enumMathOperator.MATH_MULTIPLY;
	
					write_pos.right = CopyNodeTree(brother);
					write_pos.right.parent = write_pos;
	
					write_pos.left = new TNode;
	
					write_pos.left.parent = write_pos;
					Solve(ref parent, ref write_pos.left);
				}
				else
				{
					write_pos.eType = enumNodeType.NODE_OPERATOR;
					write_pos.eOperator = enumMathOperator.MATH_DIVIDE;
	
					write_pos.left = CopyNodeTree(brother);
					write_pos.left.parent = write_pos;
	
					write_pos.right = new TNode;
	
					write_pos.right.parent = write_pos;
					Solve(ref parent, ref write_pos.right);
				}
				break;
			case enumMathOperator.MATH_POSITIVE:
				write_pos.eType = enumNodeType.NODE_OPERATOR;
				write_pos.eOperator = enumMathOperator.MATH_POSITIVE;
	
				write_pos.left = new TNode;
	
				write_pos.left.parent = write_pos;
				Solve(ref parent, ref write_pos.left);
				break;
			case enumMathOperator.MATH_NEGATIVE:
				write_pos.eType = enumNodeType.NODE_OPERATOR;
				write_pos.eOperator = enumMathOperator.MATH_NEGATIVE;
	
				write_pos.left = new TNode;
	
				write_pos.left.parent = write_pos;
				Solve(ref parent, ref write_pos.left);
				break;
			case enumMathOperator.MATH_SIN:
				write_pos.eType = enumNodeType.NODE_FUNCTION;
				write_pos.eOperator = enumMathOperator.MATH_ARCSIN;
	
				write_pos.left = new TNode;
	
				write_pos.left.parent = write_pos;
				Solve(ref parent, ref write_pos.left);
				break;
			case enumMathOperator.MATH_COS:
				write_pos.eType = enumNodeType.NODE_FUNCTION;
				write_pos.eOperator = enumMathOperator.MATH_ARCCOS;
	
				write_pos.left = new TNode;
	
				write_pos.left.parent = write_pos;
				Solve(ref parent, ref write_pos.left);
				break;
			case enumMathOperator.MATH_TAN:
				write_pos.eType = enumNodeType.NODE_FUNCTION;
				write_pos.eOperator = enumMathOperator.MATH_ARCTAN;
	
				write_pos.left = new TNode;
	
				write_pos.left.parent = write_pos;
				Solve(ref parent, ref write_pos.left);
				break;
			default:
				Debug.Assert(0);
				break;
			}
		}
		else
		{
			//have not parent
			write_pos.eType = enumNodeType.NODE_NUMBER;
			write_pos.value = 0.0;
	
		}
	}
	private void CheckOnlyOneVar(TNode now)
	{
		if (now.eType == enumNodeType.NODE_VARIABLE)
		{
			iVarAppearedCount++;
			LastVarNode = now;
		}
		if (now.left != null)
			CheckOnlyOneVar(now.left);
	
		if (now.right != null)
			CheckOnlyOneVar(now.right);
	}
	private void Subs_inner(TNode node, string ptVar, double @value)
	{
		if (node.eType == enumNodeType.NODE_VARIABLE && node.varname == ptVar)
		{
			node.eType = enumNodeType.NODE_NUMBER;
			node.@value = @value;
			return;
		}
	
		if (node.left != null)
			Subs_inner(node.left, ptVar, @value);
	
		if (node.right != null)
			Subs_inner(node.right, ptVar, @value);
	}
	public TNode head;
	public void Reset()
	{
		head = null;
		pVariableTable = null;
	
		iVarAppearedCount = 0;
	}

	//仅将变量表内置数值代入，不进行计算
	public void Vpa(bool bOutput)
	{
		Vpa_inner(head);
	}
	public void LinkVariableTable(TVariableTable p)
	{
		pVariableTable = p;
	}
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	void Read(string expression, bool bOutput);
	public void Read(double num, bool bOutput)
	{
		head = NewNode(enumNodeType.NODE_NUMBER);
		head.value = num;
	}

	//所有操作符未完成

	//求解单变量方程 不验证可求解性，需提前调用HasOnlyOneVar确认
	public string Solve(ref string var, ref double @value)
	{
		TExpressionTree Result = new TExpressionTree();
	
		TNode ResultNow = new TNode;
	
		var = LastVarNode.varname;
	
		Solve(ref LastVarNode, ref ResultNow);
	
		Result.head = ResultNow;
	
		@value = Result.Value(true);
	
		return OutputStr();
	
	}
	public string OutputStr()
	{
		string temp;
	
		if (head != null)
			TraverseInOrder(head, ref temp);
		return temp;
	}
	public void Simplify(bool bOutput)
	{
		Simplify(head);
	}
	public string Diff(string var, int n, bool bOutput)
	{
		if (pVariableTable.FindVariableTable(var) == pVariableTable.VariableTable.end())
			throw TError
			{
				enumError.ERROR_UNDEFINED_VARIABLE, var
			}
	
		for (int i = 0; i < n; i++)
		{
			Diff(head, var);
		}
		return OutputStr();
	}

	//替换  var变量指针 value数字
	public void Subs(string ptVar, double @value, bool output)
	{
		Subs_inner(head, ptVar, @value);
	}
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	void Subs(string vars, string nums, bool output); //vars为被替换变量，nums为替换表达式，以空格分隔

	//替换 VarsVector变量 NumsVector数字
	public void Subs(List<string > VarsVector, List<double> NumsVector, bool output)
	{
		if (VarsVector.Count == NumsVector.Count) //替换与被替换元素数目相等
		{
			for (size_t i = 0; i < VarsVector.Count; i++) //遍历被替换变量
			{
				//查表识别被替换变量
				auto it = pVariableTable.FindVariableTable(VarsVector[i]);
				if (it != pVariableTable.VariableTable.end()) //已识别出
				{
					string var = *it;
					//构建替换节点树
					TExpressionTree Expr = new TExpressionTree();
					Expr.LinkVariableTable(pVariableTable);
					Expr.Read(NumsVector[i], false);
	
					//得到所有被替换变量的位置
					List<TNode > VarsPos = new List<TNode >();
					GetVariablePos(var, ref VarsPos);
					for (size_t j = 0; j < VarsPos.Count; j++)
					{
						TNode newNode = CopyNodeTree(Expr.head);
	
						//连接到新节点
						if (VarsPos[j] != head)
						{
							if (VarsPos[j].parent.left != null && VarsPos[j].parent.left == VarsPos[j])
								VarsPos[j].parent.left = newNode;
							if (VarsPos[j].parent.right != null && VarsPos[j].parent.right == VarsPos[j])
								VarsPos[j].parent.right = newNode;
							newNode.parent = VarsPos[j].parent;
						}
						else
							head = newNode;
	
						//删掉旧节点
						VarsPos[j] = null;
					}
				}
			}
		}
		else
			throw TError
			{
				enumError.ERROR_SUBS_NOT_EQUAL, TEXT("")
			}
	}
	public bool CanCalc()
	{
		return CanCalc(head);
	}
	public bool IsSingleVar()
	{
		//return SelfVariableTable.VariableTable.size() == 1;
		return true;
	}
	public bool HasOnlyOneVar()
	{
		return iVarAppearedCount == 1;
	}
	public bool CheckOnlyOneVar()
	{
		iVarAppearedCount = 0;
		CheckOnlyOneVar(head);
		return HasOnlyOneVar();
	}

	//计算表达式值 operateHeadNode决定是否操作本身的节点
	public double Value(bool operateHeadNode)
	{
		TNode pNode = null;
		if (operateHeadNode)
			pNode = head;
		else
			pNode = CopyNodeTree(head);
	
		try
		{
			Calc(ref pNode);
		}
		catch (enumError &err)
		{
			//删掉节点树并提交给上级
			if (operateHeadNode == false)
				DeleteNode(pNode);
			throw err;
		}
	
		//得到最终结果
		double num = pNode.value;
		//释放复制的树
		if (operateHeadNode == false)
			pNode.Dispose();
		return num;
	}

	//复制出一棵临时树计算值
	public string Calc()
	{
		return Calc(null);
	}
//C++ TO C# CONVERTER NOTE: Overloaded method(s) are created above to convert the following method having default parameters:
//ORIGINAL LINE: string Calc(double *result = null)
	public string Calc(ref double result)
	{
		if (CanCalc())
		{
			TNode Duplicate = CopyNodeTree(head);
			Calc(ref Duplicate);
	
			if (result != null)
				result = Duplicate.value;
	
			string temp = Node2Str(Duplicate);
			Duplicate.Dispose();
	
			return temp;
		}
		else
			return OutputStr();
	}

//C++ TO C# CONVERTER NOTE: This 'CopyFrom' method was converted from the original C++ copy assignment operator:
//ORIGINAL LINE: TExpressionTree &operator =(const TExpressionTree &expr)
	public TExpressionTree CopyFrom(TExpressionTree expr)
	{
		Release();
		Reset();
		head = CopyNodeTree(expr.head);
		LinkVariableTable(expr.pVariableTable);
		return this;
	}
	public static TExpressionTree operator +(TExpressionTree ImpliedObject, TExpressionTree expr)
	{
		if (head == null)
			head = CopyNodeTree(expr.head);
		else
		{
			TNode Add = NewNode(enumNodeType.NODE_OPERATOR, enumMathOperator.MATH_ADD);
			TNode Right = CopyNodeTree(expr.head);
	
	
			Add.left = head;
			Add.right = Right;
	
			head.parent = Add;
			Right.parent = Add;
	
			head = Add;
		}
		return this;
	}
	public static TExpressionTree operator *(TExpressionTree ImpliedObject, double @value)
	{
		if (head == null)
		{
			throw TError
			{
				enumError.ERROR_EMPTY_INPUT, TEXT("")
			}
			return this;
		}
	
		TNode Multiply = NewNode(enumNodeType.NODE_OPERATOR, enumMathOperator.MATH_MULTIPLY);
		TNode Value = NewNode(enumNodeType.NODE_NUMBER);
		Value.@value = @value;
	
		Multiply.left = head;
		Multiply.right = Value;
	
		head.parent = Multiply;
		Value.parent = Multiply;
	
		head = Multiply;
	
		return this;
	}
	public static TExpressionTree operator +(TExpressionTree ImpliedObject, double @value)
	{
		if (head == null)
		{
			throw TError
			{
				enumError.ERROR_EMPTY_INPUT, TEXT("")
			}
			return this;
		}
	
		TNode Add = NewNode(enumNodeType.NODE_OPERATOR, enumMathOperator.MATH_ADD);
		TNode Value = NewNode(enumNodeType.NODE_NUMBER);
		Value.@value = @value;
	
		Add.left = head;
		Add.right = Value;
	
		head.parent = Add;
		Value.parent = Add;
	
		head = Add;
	
		return this;
	}
	public static TExpressionTree operator -(TExpressionTree ImpliedObject, double @value)
	{
		if (head == null)
		{
			throw TError
			{
				enumError.ERROR_EMPTY_INPUT, TEXT("")
			}
			return this;
		}
	
		TNode Substract = NewNode(enumNodeType.NODE_OPERATOR, enumMathOperator.MATH_SUBSTRACT);
		TNode Value = NewNode(enumNodeType.NODE_NUMBER);
		Value.@value = @value;
	
		Substract.left = head;
		Substract.right = Value;
	
		head.parent = Substract;
		Value.parent = Substract;
	
		head = Substract;
	
		return this;
	}

	public TExpressionTree()
	{
		Reset();
	}
	public void Dispose()
	{
		Release();
	}
}