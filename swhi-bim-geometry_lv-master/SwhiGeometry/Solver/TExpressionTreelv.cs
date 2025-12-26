/**
using System;
using System.Collections.Generic;
using System.Diagnostics;
namespace Lv.BIM.Solver
{
    public class TNode
    {
        //lvwan改写,用来临时存储表达式树，以获取所有parent\left\right
        public static Dictionary<int, TNode> tempDictionary = new Dictionary<int, TNode>();
        private int _id;
        public int id => _id;
        private enumNodeType _eType;
        public enumNodeType eType
        {
            get { return _eType; }
            set
            {
                _eType = value;
                //去静态字段中寻找。如果找到，则同时修改静态字段
                if (tempDictionary.ContainsKey(id))
                {
                    tempDictionary[id] = this;
                }
            }
        }
        private enumMathOperator _eOperator;
        public enumMathOperator eOperator
        {
            get { return _eOperator; }
            set
            {
                _eOperator = value;
                //去静态字段中寻找。如果找到，则同时修改静态字段
                if (tempDictionary.ContainsKey(id))
                {
                    tempDictionary[id] = this;
                }
            }
        }
        private double _value;
        public double value { get { return _value; }
            set 
            { 
                _value = value;
                //去静态字段中寻找。如果找到，则同时修改静态字段
                if (tempDictionary.ContainsKey(id))
                {
                    tempDictionary[id] = this;
                }               
            }
        }
        private string _varname;
        public string varname
        {
            get { return _varname; }
            set
            {
                _varname = value;
                //去静态字段中寻找。如果找到，则同时修改静态字段
                if (tempDictionary.ContainsKey(id))
                {            
                    tempDictionary[id] = this;
                }
            }
        }
#nullable enable
        //所有父子关系均保存在以下三个ID中
        public int? parentId;//父ID
        public int? leftId;//左儿子
        public int? rightId;//右儿子
        public bool IsHead => parentId == null;
        public bool IsLast => leftId == null && rightId == null;
        //考虑以下情况赋值：ignore.parent.left = child;暂时考虑此种情形可行
        //以下取出和设置父子实体，均需要先将treeDictionary付给临时变量tempDictionary
        //设置结束后需要将临时变量tempDictionary付给treeDictionary
        public TNode? parent
        {
            get { return parentId.HasValue ?tempDictionary[parentId.Value]: null; }
            set
            {
                if (value != null)
                {
                    parentId = value.id;
                    //添加父亲
                    if (tempDictionary.ContainsKey(value.id))
                    {
                        tempDictionary[value.id] = value;
                    }
                    else
                    {
                        tempDictionary.Add(value.id, value);
                    }
                }
                else
                {
                    //删除静态字段中parent
                    //if (parentId.HasValue && tempDictionary.ContainsKey(parentId.Value))
                    //{
                    //    tempDictionary.Remove(parentId.Value);
                    //}
                    parentId = null;
                }
                //修改tempDictionary
                if (tempDictionary.ContainsKey(id))
                {                    
                    tempDictionary[id] = this;
                }
            }
        }
        public TNode? left
        {
            get
            {
                return leftId.HasValue ?  tempDictionary[leftId.Value] : null;
            }
            set
            {
                if (value != null)
                {
                    leftId = value.id;
                    if (tempDictionary.ContainsKey(value.id))
                    {
                        tempDictionary[value.id] = value;
                    }
                    else
                    {
                        tempDictionary.Add(value.id, value);
                    }
                }
                else
                {
                    //删除静态字段中left
                    //if(leftId.HasValue && tempDictionary.ContainsKey(leftId.Value))
                    //{
                    //    tempDictionary.Remove(leftId.Value);
                    //}
                    leftId = null;
                }
                //修改tempDictionary
                if (tempDictionary.ContainsKey(id))
                {
                    TNode temp = this;
                    tempDictionary[id] = temp;
                }
            }

        }

        public TNode? right
        {
            get
            {
                return rightId.HasValue ? tempDictionary[rightId.Value]: null;
            }
            set
            {
                if (value != null)
                {
                    rightId = value.id;
                    if (tempDictionary.ContainsKey(value.id))
                    {
                        tempDictionary[value.id] = value;
                    }
                    else
                    {
                        tempDictionary.Add(value.id, value);
                    }
                }
                else
                {
                    //删除静态字段中right
                    //if (rightId.HasValue && tempDictionary.ContainsKey(rightId.Value))
                    //{
                    //    tempDictionary.Remove(rightId.Value);
                    //}
                    rightId = null;
                }
                //修改tempDictionary
                if (tempDictionary.ContainsKey(id))
                {
                    TNode temp = this;
                    tempDictionary[id] = temp;
                }
            }
        }

        /// 单个元素 
        public TNode()
        {
            this._eType = enumNodeType.NODE_NULL;
            this._eOperator = enumMathOperator.MATH_NULL;
            this._value = 0;
            this.parentId = null;
            this.leftId = null;
            this.rightId = null;
            _id = GetHashCode();
        }
       
        public string ContentToString(){
            string result = "";
            switch (eType)
            {
                case enumNodeType.NODE_NULL:
                    result = "";
                    break;
                case enumNodeType.NODE_NUMBER:
                    result=value.ToString();
                    break;
                case enumNodeType.NODE_OPERATOR:
                    result = TExpressionTree.EnumOperatorToTChar(eOperator);
                    break;
                case enumNodeType.NODE_VARIABLE:
                    result = varname;
                    break;
                case enumNodeType.NODE_FUNCTION:
                    result = TExpressionTree.Function2Str(eOperator);
                    break;
                default:
                    result = value.ToString();
                    break;
            }
            return result;
        }
        public override string ToString()
        {
            string result = ContentToString();
            if (left != null)
            {
                result += "\n";
                result += "左：" + left.ContentToString();
            }
            if (right != null)
            {
                result += "\n";
                result += "右：" + right.ContentToString();
            }
            if (parent != null)
            {
                result += "\n";
                result += "父：" + parent.ContentToString();
            }
            return result;
        }

    }
    public enum enumMathOperator
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
    public enum enumNodeType
    {
        NODE_NULL,
        NODE_NUMBER,
        NODE_OPERATOR,
        NODE_VARIABLE,
        NODE_FUNCTION
    }
    public class TExpressionTree : System.IDisposable
    {

        private const int MAX_VAR_NAME = 32; //同时也是浮点数转字符串的最大长度
        private const double MIN_DOUBLE = 1e-6;
        //public TNode head;
        private string str_expression;
        public string ExpressionString => str_expression;
        public TNode head;

        private TVariableTable pVariableTable;
        private int iVarAppearedCount;
        private TNode LastVarNode;
        private List<TNode> tree_nodes=>Read(str_expression);
        private List<TNode> TreeNodes=> tree_nodes;

        //lvwan改写,用来存储表达式树
        Dictionary<int, TNode> treeDictionary = new Dictionary<int, TNode>();
       
        private void Release()
        {
            //DeleteNode(head);
            //head = null;
            treeDictionary.Clear();
        }

        //运算符性质函数
        public int GetOperateNum(enumMathOperator eOperator)
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
        public static string EnumOperatorToTChar(enumMathOperator eOperator)
        {
            switch (eOperator)
            {
                case enumMathOperator.MATH_POSITIVE:
                    return "+";
                case enumMathOperator.MATH_NEGATIVE:
                    return "-";
                case enumMathOperator.MATH_LEFT_PARENTHESIS:
                    return "(";
                case enumMathOperator.MATH_RIGHT_PARENTHESIS:
                    return ")";
                case enumMathOperator.MATH_ADD:
                    return "+";
                case enumMathOperator.MATH_SUBSTRACT:
                    return "-";
                case enumMathOperator.MATH_MULTIPLY:
                    return "*";
                case enumMathOperator.MATH_DIVIDE:
                    return "/";
                case enumMathOperator.MATH_POWER:
                    return "^";
                case enumMathOperator.MATH_AND:
                    return "&";
                case enumMathOperator.MATH_OR:
                    return "|";
                case enumMathOperator.MATH_MOD:
                    return "%";
                default:
                    throw new TError(enumError.ERROR_WRONG_MATH_OPERATOR, "value:" + eOperator.ToString());
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

                case enumMathOperator.MATH_POSITIVE: //������
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
            //Debug.Assert(0);
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
                    throw new TError(enumError.ERROR_WRONG_MATH_OPERATOR, "VALUE:" + eOperator.ToString());
            }
        }


        // 是基本运算符()+-* /^&|%  
        private bool isBaseOperator(char c)
        {

            switch (c)
            {
                case '(':
                case ')':
                case '+':
                case '-':
                case '*':
                case '/':
                case '^':
                case '&':
                case '|':
                case '%':
                    return true;
            }
            return false;
        }

        // 字符是0-9或.
        private bool isDoubleChar(char c)
        {
            if ((c >= '0' && c <= '9') || c == '.')
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //有效性检查（返回0则出现异常字符）
        private bool isLegal(char c)
        {
            if (isDoubleChar(c))
            {
                return true;
            }
            if (isBaseOperator(c))
            {
                return true;
            }
            if (c == '_' || char.IsLetter(c))//IsCharAlpha(c) 是否是字母
            {
                return true;
            }
            return false;
        }


        //
        private static enumMathOperator BaseOperatorCharToEnum(char c)
        {
            switch (c)
            {
                case '(':
                    return enumMathOperator.MATH_LEFT_PARENTHESIS;
                case ')':
                    return enumMathOperator.MATH_RIGHT_PARENTHESIS;
                case '+':
                    return enumMathOperator.MATH_ADD;
                case '-':
                    return enumMathOperator.MATH_SUBSTRACT;
                case '*':
                    return enumMathOperator.MATH_MULTIPLY;
                case '/':
                    return enumMathOperator.MATH_DIVIDE;
                case '^':
                    return enumMathOperator.MATH_POWER;
                case '&':
                    return enumMathOperator.MATH_AND;
                case '|':
                    return enumMathOperator.MATH_OR;
                case '%':
                    return enumMathOperator.MATH_MOD;
                default:
                    return enumMathOperator.MATH_NULL;
            }
        }
        private static enumMathOperator Str2Function(string s)
        {
            if (s == "sin")
            {
                return enumMathOperator.MATH_SIN;
            }
            if (s == "cos")
            {
                return enumMathOperator.MATH_COS;
            }
            if (s == "tan")
            {
                return enumMathOperator.MATH_TAN;
            }
            if (s == "arcsin")
            {
                return enumMathOperator.MATH_ARCSIN;
            }
            if (s == "arccos")
            {
                return enumMathOperator.MATH_ARCCOS;
            }
            if (s == "arctan")
            {
                return enumMathOperator.MATH_ARCTAN;
            }
            if (s == "sqrt")
            {
                return enumMathOperator.MATH_SQRT;
            }
            if (s == "ln")
            {
                return enumMathOperator.MATH_LN;
            }
            if (s == "log10")
            {
                return enumMathOperator.MATH_LOG10;
            }
            if (s == "exp")
            {
                return enumMathOperator.MATH_EXP;
            }
            return enumMathOperator.MATH_NULL;
        }
        public static string Function2Str(enumMathOperator eOperator)
        {
            switch (eOperator)
            {
                case enumMathOperator.MATH_SIN:
                    return "sin";
                case enumMathOperator.MATH_COS:
                    return "cos";
                case enumMathOperator.MATH_TAN:
                    return "tan";
                case enumMathOperator.MATH_ARCSIN:
                    return "arcsin";
                case enumMathOperator.MATH_ARCCOS:
                    return "arccos";
                case enumMathOperator.MATH_ARCTAN:
                    return "arctan";
                case enumMathOperator.MATH_SQRT:
                    return "sqrt";
                case enumMathOperator.MATH_LN:
                    return "ln";
                case enumMathOperator.MATH_LOG10:
                    return "log10";
                case enumMathOperator.MATH_EXP:
                    return "exp";
            }
            throw new TError(enumError.ERROR_WRONG_MATH_OPERATOR, "value=" + eOperator.ToString());
        }

        //
        //////是整数 且 为偶数
        public bool IsIntAndEven(double n)
        {
            int i = (int)n;
            if (Math.Abs(n - i) < MIN_DOUBLE)
                if (i % 2 == 0)
                    return true;
            return false;
        }

        //TVariableTable SelfVariableTable;

        //由in order队列得到post order队列//
        private List<TNode> InQueue2PostQueue(Queue<TNode> InOrder)//修改将C++输入修改为C#输出
        {
            List<TNode> PostOrder = new List<TNode>();
            int parenthesis_num = 0;
            Stack<TNode> temp = new Stack<TNode>();
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
                            ///pop至左括号
                            while (temp.Count > 0)
                            {
                                if (temp.Peek().eOperator == enumMathOperator.MATH_LEFT_PARENTHESIS) //(
                                {
                                    //temp.Peek();//temp.Peek() = null;//C++=>delete temp.top();
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
                                {
                                    break;
                                }
                            }
                            InOrder.Peek(); //此处删除= null，释放peak内存
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
                                {
                                    while (temp.Count > 0 && Rank(InOrder.Peek().eOperator) <= Rank(temp.Peek().eOperator)) //临时栈有内容，且新进符号优先级低，则挤出高优先级及同优先级符号
                                    {
                                        PostOrder.Add(temp.Peek()); //符号进入post队列
                                        temp.Pop();
                                    }
                                }
                                else //右结合
                                {
                                    while (temp.Count > 0 && Rank(InOrder.Peek().eOperator) < Rank(temp.Peek().eOperator)) //临时栈有内容，且新进符号优先级低，则挤出高优先级，但不挤出同优先级符号（因为右结合）
                                    {
                                        PostOrder.Add(temp.Peek()); //符号进入post队列
                                        temp.Pop();
                                    };
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
                throw new TError(enumError.ERROR_PARENTHESIS_NOT_MATCH, "");
            }
            return PostOrder;
        }
        //粗切分：利用operator切分
        public struct TStrPiece
        {
            public bool bBaseOperator;
            public string s;
            public TStrPiece(bool bbaseoperator, string ss) { bBaseOperator = bbaseoperator; s = ss; }
            //TStrPiece(bool bIn, String sIn) :bBaseOperator(bIn), s(sIn) { }
        }
        private Queue<TNode> ReadToInOrder(string expression)//修改前：private void ReadToInOrder(string expression, Queue<TNode> InOrder)
        {
            Queue<TNode> InOrder = new Queue<TNode>();//返回结果
            if (string.IsNullOrEmpty(expression))
            {
                throw new TError(enumError.ERROR_EMPTY_INPUT, expression);
                //return;
            }
            expression.Replace(" ", "");
            expression.Replace("\t", "");
            expression.Replace("\r", "");
            expression.Replace("\n", "");

            //过滤掉所有多余的加减
            expression.Replace("--", "+");
            expression.Replace("+-", "-");
            expression.Replace("-+", "-");

            //字符合法性检查
            foreach (var c in expression)
            {
                if (!isLegal(c))
                {
                    throw new TError(enumError.ERROR_ILLEGALCHAR, "WRONG CHAR:" + c.ToString());
                }
            }


            List<TStrPiece> Data = new List<TStrPiece>();

            string temp = "";
            foreach (var c in expression)
            {
                if (!isBaseOperator(c))
                {
                    temp = temp + c.ToString();
                }
                else
                {
                    if (!string.IsNullOrEmpty(temp))
                    {
                        Data.Add(new TStrPiece(false, temp));
                        //Data.emplace_back(false, temp);
                        temp = "";
                    }
                    Data.Add(new TStrPiece(true, c.ToString()));
                    //Data.emplace_back(true, String({ c}));
                }
            }
            if (!string.IsNullOrEmpty(temp))
            {
                Data.Add(new TStrPiece(false, temp));
                //Data.emplace_back(false, temp);
                temp = "";
            }

            //二次切分：切分出4类元素
            //并送入Pre In order
            List<TNode> PreInOrder = new List<TNode>();
            TNode tempNode;
            //string tempTChar;
            enumMathOperator tempeOperator;
            for (int ii = 0; ii < Data.Count; ii++)
            {
                if (Data[ii].bBaseOperator) //识别出基本运算符（括号也在其中）
                {
                    tempNode = new TNode();
                    tempNode.eType = enumNodeType.NODE_OPERATOR;
                    tempNode.eOperator = BaseOperatorCharToEnum(Data[ii].s[0]);
                    //treeDictionary.Add(tempNode.id, tempNode);//lv添加
                    PreInOrder.Add(tempNode);
                }
                else
                {
                    //逐位检验是否为数字
                    bool isDouble = true;
                    foreach (var c in Data[ii].s)
                    {
                        if (isDoubleChar(c) == false)
                        {
                            isDouble = false;
                            break;
                        }
                    }

                    if (isDouble) //数字
                    {
                        tempNode = new TNode();
                        tempNode.eType = enumNodeType.NODE_NUMBER;
                        tempNode.value = Convert.ToDouble(Data[ii].s);
                        //treeDictionary.Add(tempNode.id, tempNode);//lv添加
                        PreInOrder.Add(tempNode);
                    }
                    else
                    {
                        if ((tempeOperator = Str2Function(Data[ii].s)) != enumMathOperator.MATH_NULL) //识别出函数
                        {
                            tempNode = new TNode();
                            tempNode.eType = enumNodeType.NODE_FUNCTION;
                            tempNode.eOperator = tempeOperator;
                            //treeDictionary.Add(tempNode.id, tempNode);//lv添加
                            PreInOrder.Add(tempNode);
                        }
                        else //变量
                        {
                            //非运算符、数字、函数

                            if (pVariableTable == null)
                            {
                                ReleaseVectorTNode(PreInOrder);
                                //treeDictionary.Clear();//lvwan添加
                                throw new TError(enumError.ERROR_NOT_LINK_VARIABLETABLE, "");
                                //return;
                            }
                            if (!char.IsLetter(Data[ii].s[0]) && Data[ii].s[0] != '_') //变量名首字符需为下划线或字母
                            {
                                ReleaseVectorTNode(PreInOrder);
                                //treeDictionary.Clear();//lvwan添加
                                throw new TError(enumError.ERROR_INVALID_VARNAME, Data[ii].s);
                                //return;
                            }


                            //
                            if (pVariableTable.FindVariableTable(Data[ii].s) == pVariableTable.VariableTable.Count)
                            {
                                ReleaseVectorTNode(PreInOrder);
                                //treeDictionary.Clear();//lvwan添加
                                throw new TError(enumError.ERROR_UNDEFINED_VARIABLE, Data[ii].s);
                                //return;
                            }

                            tempNode = new TNode();
                            tempNode.eType = enumNodeType.NODE_VARIABLE;
                            tempNode.varname = Data[ii].s;
                            //treeDictionary.Add(tempNode.id, tempNode);//lv添加
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
            //lvwan添加，此时4大元素均已存入treeDictionary
            //识别取正运算符与取负运算符
            bool bFirstOrParenFirst = false;
            bool bAferOneOperator = false;
            int i = 0;
            if (PreInOrder[0].eOperator == enumMathOperator.MATH_ADD)
            {
                //TNode tnodtemp = PreInOrder[0];
                //tnodtemp.eOperator = enumMathOperator.MATH_POSITIVE;
                //treeDictionary[PreInOrder[0].id] = tnodtemp;
                PreInOrder[0].eOperator = enumMathOperator.MATH_POSITIVE;
                i++;
            }
            if (PreInOrder[0].eOperator == enumMathOperator.MATH_SUBSTRACT)
            {
                //TNode tnodtemp = PreInOrder[0];
                //tnodtemp.eOperator = enumMathOperator.MATH_NEGATIVE;
                //treeDictionary[PreInOrder[0].id] = tnodtemp;
                PreInOrder[0].eOperator = enumMathOperator.MATH_NEGATIVE;
                i++;
            }
            for (; i < PreInOrder.Count;)
            {
                if (PreInOrder[i].eType == enumNodeType.NODE_OPERATOR && PreInOrder[i].eOperator != enumMathOperator.MATH_RIGHT_PARENTHESIS)
                {
                    if (i + 1 < PreInOrder.Count)
                    {
                        i++;
                    }
                    else
                    {
                        break;
                    }
                    if (PreInOrder[i].eOperator == enumMathOperator.MATH_ADD)
                    {
                        //TNode tnodtemp = PreInOrder[i];
                        //tnodtemp.eOperator = enumMathOperator.MATH_POSITIVE;
                        //treeDictionary[PreInOrder[i].id] = tnodtemp;
                        PreInOrder[i].eOperator = enumMathOperator.MATH_POSITIVE;
                        i++;
                        continue;
                    }
                    if (PreInOrder[i].eOperator == enumMathOperator.MATH_SUBSTRACT)
                    {
                        //TNode tnodtemp = PreInOrder[i];
                        //tnodtemp.eOperator = enumMathOperator.MATH_NEGATIVE;
                        //treeDictionary[PreInOrder[i].id] = tnodtemp;
                        PreInOrder[i].eOperator = enumMathOperator.MATH_NEGATIVE;
                        i++;
                        continue;
                    }
                }
                else
                {
                    i++;
                }
            }

            foreach (var pNode in PreInOrder)
            {
                InOrder.Enqueue(pNode);
            }
            return InOrder;
        }
        private static string Node2Str(TNode node)
        {
            switch (node.eType)
            {
                case enumNodeType.NODE_NUMBER:
                    if (Math.Abs(node.value - (long)node.value) < MIN_DOUBLE)
                    {
                        return node.value.ToString();
                    }
                    else
                    {
                        return node.value.ToString();
                    }
                //break;
                case enumNodeType.NODE_VARIABLE:
                    return node.varname;
                //break;
                case enumNodeType.NODE_FUNCTION:
                    return Function2Str(node.eOperator);
                //break;
                case enumNodeType.NODE_OPERATOR:
                    return EnumOperatorToTChar(node.eOperator);
                    //break;
            }
            //Debug.Assert(0);
            return "Error";
        }

        //将PostOrder建立为树，并进行表达式有效性检验（确保二元及一元运算符、函数均有操作数）
        private List<TNode> BuildExpressionTree(List<TNode> PostOrder)
        {
            List<TNode> PostOrderResult = PostOrder;//用于存储结果lvwan
            Stack<TNode> tempStack = new Stack<TNode>();
            //逐个识别PostOrder序列，构建表达式树
            //修改以下循环，使语义更为清晰
            int numOfpostorder = PostOrder.Count;
            for (int i = 0; i < numOfpostorder; i++)//修改前：foreach (var pNodeNow in PostOrder)
            {
                TNode pNodeNow = PostOrder[i];
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
                                throw new TError(enumError.ERROR_WRONG_EXPRESSION, "");
                                //return;
                            }

                            PostOrderResult[i].rightId = tempStack.Peek().id;//更改前：pNodeNow.right = tempStack.Peek();
                            tempStack.Peek().parentId = pNodeNow.id;
                            tempStack.Pop();

                            if (tempStack.Count == 0)
                            {
                                //释放所有TNode，报错

                                ReleaseVectorTNode(PostOrder);
                                //Stack只是对PostOrder的重排序，所以不用delete
                                throw new TError(enumError.ERROR_WRONG_EXPRESSION, "");
                                //return;
                            }

                            PostOrderResult[i].leftId = tempStack.Peek().id;//更改前：pNodeNow.left  = tempStack.Peek();
                            tempStack.Peek().parentId = pNodeNow.id;
                            tempStack.Pop();

                            tempStack.Push(pNodeNow);
                        }
                        else
                        {
                            if (tempStack.Count == 0)
                            {
                                ////释放所有TNode，报错
                                ReleaseVectorTNode(PostOrder);
                                //Stack只是对PostOrder的重排序，所以不用delete
                                throw new TError(enumError.ERROR_WRONG_EXPRESSION, "");
                                //return;
                            }
                            PostOrderResult[i].leftId = tempStack.Peek().id;//更改前：pNodeNow.left  = tempStack.Peek();
                                                                       //pNodeNow.left = tempStack.Peek();
                            tempStack.Peek().parentId = pNodeNow.id;
                            tempStack.Pop();

                            tempStack.Push(pNodeNow);
                        }
                        break;
                }
            }
           //每次建立表达式树前，清除TNode静态变量中的treeDictionary；lvwan
            //TNode.treeDictionary.Clear();
            foreach (var item in PostOrder)
            {
                if (item != null)
                {
                    treeDictionary.Add(item.id, item);
                }
               
            }
            //令表达式树等于TNode静态变量中的treeDictionary；lvwan
            //treeDictionary = TNode.treeDictionary;
            //找出根节点
            head = PostOrder[0];
            while (head.parentId.HasValue)
            {
                int i = head.parentId.Value;
                head = treeDictionary[i];
            }
            
            return PostOrderResult;
        }
        //按顺序遍历
        private string TraverseInOrder(TNode now)
        {
            string output = "";
            //必须将表达式树提前付给TNode的静态变量，以使得可以求出父子
            TNode.tempDictionary = treeDictionary;
            int has_parenthesis = 0;
            if (GetOperateNum(now.eOperator) == 1) //一元运算符：函数和取负
            {
                if (now.eType == enumNodeType.NODE_FUNCTION)
                {
                    output += Node2Str(now) + "(";
                    has_parenthesis = 1;
                }
                else
                {
                    output += "(" + Node2Str(now);
                    has_parenthesis = 1;
                }
            }

            if (GetOperateNum(now.eOperator) != 1) //非一元运算符才输出，即一元运算符的输出顺序已改变
            {
                if (now.eType == enumNodeType.NODE_OPERATOR) //本级为运算符
                {
                    if (now.parentId.HasValue)
                    {
      
                        if ((GetOperateNum(now.parent.eOperator) == 2 && (Rank(now.parent.eOperator) > Rank(now.eOperator) || (Rank(now.parent.eOperator) == Rank(now.eOperator) && ((inAssociativeLaws(now.parent.eOperator) == false && now == now.parent.right) || (isLeft2Right(now.parent.eOperator) == false && isLeft2Right(now.eOperator) == false))))))
                        //父运算符存在，为二元，
                        ////父级优先级高于本级->加括号
                        //||
                        //两级优先级相等
                        //本级为父级的右子树 且父级不满足结合律->加括号
                        //两级都是右结合
                        //父运算符存在，为除号，且本级为分子，则添加括号
                        //(now->parent->eOperator == MATH_DIVIDE && now == now->parent->right)
                        {
                            output += "(";
                            has_parenthesis = 1;
                        }
                    }
                }
            }

            if (now.left != null) //左遍历
            {
                output+=TraverseInOrder(now.left);
            }

            if (GetOperateNum(now.eOperator) != 1) //非一元运算符才输出，即一元运算符的输出顺序已改变
            {
                output += Node2Str(now);
            }


            if (now.right != null) //右遍历
            {
                output += TraverseInOrder(now.right);
            }

            //回到本级时补齐右括号，包住前面的东西
            if (has_parenthesis != 0)
            {
                output += ")";
            }
            return output;
        }

        private void CalcNode(TNode Operator, TNode Node1, TNode Node2 = null)
        {
            TNode.tempDictionary = treeDictionary;
            double value1 = Node1.value;
            double value2 = Node2 != null ? Node2.value : 0.0;
            Operator.eType = enumNodeType.NODE_NUMBER;
            switch (Operator.eOperator)
            {
                case enumMathOperator.MATH_SQRT:
                    if (value1 < 0.0)
                    {
                        //恢复修改并抛出异常
                        Operator.eType = enumNodeType.NODE_FUNCTION;
                        throw new TError(enumError.ERROR_I, "sqrt(" + value1.ToString() + ")");
                        //return;
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
                        double value = value1 * 2.0 / Math.PI;
                        if (Math.Abs(value - (int)value) < MIN_DOUBLE && (int)value % 2 != 1)
                        {
                            //恢复修改并抛出异常
                            Operator.eType = enumNodeType.NODE_FUNCTION;
                            throw new TError(enumError.ERROR_OUTOF_DOMAIN, "tan(" + value.ToString() + ")");
                            //return;
                        }
                        Operator.value = Math.Tan(value1);
                        break;
                    }
                case enumMathOperator.MATH_ARCSIN:
                    if (value1 < -1.0 || value1 > 1.0)
                    {
                        //恢复修改并抛出异常
                        Operator.eType = enumNodeType.NODE_FUNCTION;
                        throw new TError(enumError.ERROR_OUTOF_DOMAIN, "arcsin(" + value1.ToString() + ")");
                        //return;
                    }
                    Operator.value = Math.Asin(value1);
                    break;
                case enumMathOperator.MATH_ARCCOS:
                    if (value1 < -1.0 || value1 > 1.0)
                    {
                        //恢复修改并抛出异常
                        Operator.eType = enumNodeType.NODE_FUNCTION;
                        throw new TError(enumError.ERROR_OUTOF_DOMAIN, "arccos(" + value1.ToString() + ")");
                    }
                    Operator.value = Math.Acos(value1);
                    break;
                case enumMathOperator.MATH_ARCTAN:
                    Operator.value = Math.Atan(value1);
                    break;
                case enumMathOperator.MATH_LN:
                    if (value1 < MIN_DOUBLE)
                    {
                        //恢复修改并抛出异常
                        Operator.eType = enumNodeType.NODE_FUNCTION;
                        throw new TError(enumError.ERROR_OUTOF_DOMAIN, "ln(" + value1.ToString() + ")");
                    }
                    Operator.value = Math.Log(value1);
                    break;
                case enumMathOperator.MATH_LOG10:
                    if (value1 < MIN_DOUBLE) //log(0)或log(负数)
                    {
                        //恢复修改并抛出异常
                        Operator.eType = enumNodeType.NODE_FUNCTION;
                        throw new TError(enumError.ERROR_OUTOF_DOMAIN, "log10(" + value1.ToString() + ")");
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
                    {
                        throw new TError(enumError.ERROR_DIVIDE_ZERO, value2.ToString());
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
                    if (Math.Abs(value1) < MIN_DOUBLE && Math.Abs(value2) < MIN_DOUBLE)
                    {
                        Operator.eType = enumNodeType.NODE_OPERATOR;
                        throw new TError(enumError.ERROR_ZERO_POWEROF_ZERO, "");
                    }

                    //(-1)^0.5=i
                    if (value1 < 0 && IsIntAndEven(1L / value2))
                    {
                        Operator.eType = enumNodeType.NODE_OPERATOR;
                        throw new TError(enumError.ERROR_I, "pow(" + value1.ToString() + "," + value2.ToString() + ")");
                    }
                    Operator.value = Math.Pow(value1, value2);
                    break;

                case enumMathOperator.MATH_MULTIPLY:
                    Operator.value = value1 * value2;
                    break;
                case enumMathOperator.MATH_DIVIDE:
                    if (Math.Abs(value2) < MIN_DOUBLE)
                    {
                        Operator.eType = enumNodeType.NODE_OPERATOR;
                        throw new TError(enumError.ERROR_DIVIDE_ZERO, "");
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
            treeDictionary = TNode.tempDictionary;
        }

        /// <summary>
        /// //用子替换父
        /// </summary>
        /// <param name="child">子</param>
        /// <param name="ignore">本节点</param>
        private void LinkParent(TNode child, TNode toRemove)
        {
            TNode.tempDictionary = treeDictionary;
            if (toRemove == head)
            {
                head = child;
                child.parent = null;
            }
            else
            {
                if (toRemove.parent.left == toRemove)
                {
                    toRemove.parent.left = child;
                }
                if (toRemove.parent.right == toRemove)
                {
                    toRemove.parent.right = child;
                }
                child.parent = toRemove.parent;
            }
           //删除静态字段中
           TNode.tempDictionary.Remove(toRemove.id);
            treeDictionary = TNode.tempDictionary;
        }

        //化简表达式树，我理解的是从头开始化简，化简后表达式树变为一个新的表达式树
        private void Simplify(TNode now)
        {
            TNode.tempDictionary = treeDictionary;
            //左遍历
            
            if (now.left != null)
            {
                Simplify(now.left);
            }

            //右遍历
            if (now.right != null)
            {
                Simplify(now.right);
            }

            //化简
            //OutputStr();
            if (GetOperateNum(now.eOperator) == 1)
            {
                bool ChildIs0 = (now.left.eType == enumNodeType.NODE_NUMBER && Math.Abs(now.left.value) < MIN_DOUBLE);
                bool ChildIs1 = (now.left.eType == enumNodeType.NODE_NUMBER && Math.Abs(now.left.value - 1) < MIN_DOUBLE);

                //sin(0)=0
                if (now.eOperator == enumMathOperator.MATH_SIN && ChildIs0)
                {
                    LinkParent(now.left, now);
                    now.left.value = 0;
                   
                    // now = null;
                    //TNode.tempDictionary.Remove(now.id);//lv删除

                }

                //cos(0)=1
                if (now.eOperator == enumMathOperator.MATH_COS && ChildIs0)
                {
                    LinkParent(now.left, now);
                    now.left.value = 1;

       
                }

                if (now.eOperator == enumMathOperator.MATH_NEGATIVE && now.left.eType == enumNodeType.NODE_NUMBER)
                {
                    TNode negative = now;
                    TNode num = now.left;
                    LinkParent(num, negative);
                    num.value = -num.value;
                    //negative = null;
                }
            }

            if (GetOperateNum(now.eOperator) == 2)
            {
                //下列每种情况必须互斥，因为每个情况都有返回值，涉及删除操作，若不返回连续执行将导致指针出错
                //不检查左右儿子是否存在，因为此处本身就是2元运算符
                bool LChildIs0 = (now.left.eType == enumNodeType.NODE_NUMBER && Math.Abs(now.left.value) < MIN_DOUBLE);
                bool RChildIs0 = (now.right.eType == enumNodeType.NODE_NUMBER && Math.Abs(now.right.value) < MIN_DOUBLE);
                bool LChildIs1 = (now.left.eType == enumNodeType.NODE_NUMBER && Math.Abs(now.left.value - 1) < MIN_DOUBLE);
                bool RChildIs1 = (now.right.eType == enumNodeType.NODE_NUMBER && Math.Abs(now.right.value - 1) < MIN_DOUBLE);

                //被除数为0
                if (now.eOperator == enumMathOperator.MATH_DIVIDE && RChildIs0)
                {
                    throw new TError(enumError.ERROR_DIVIDE_ZERO, "");
                }

                //0的0次方
                if (now.eOperator == enumMathOperator.MATH_POWER && LChildIs0 && RChildIs0)
                {
                    throw new TError(enumError.ERROR_ZERO_POWEROF_ZERO, "");
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
                    temp = new TNode();
                    temp.eType = enumNodeType.NODE_NUMBER;
                    temp.value = 1;
                    //添加到表达式树
                    TNode.tempDictionary.Add(temp.id, temp);
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
                    {
                        head = temp;
                    }

                    DeleteNode(now);
                }

                //任何数乘或被乘0、被0除、0的除0外的任何次方，等于0
                if ((now.eOperator == enumMathOperator.MATH_MULTIPLY && (LChildIs0 || RChildIs0)) || (now.eOperator == enumMathOperator.MATH_DIVIDE && LChildIs0) || (now.eOperator == enumMathOperator.MATH_POWER && LChildIs0))
                {
                    //替换掉当前运算符，这个0节点将在回溯时处理
                    //新建一个0节点
                    TNode temp;
                    temp = new TNode();
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
                    {
                        head = temp;
                    }

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

                    LChild = null;
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
                    num = null;
                    now = null;
                }


            }

            treeDictionary=TNode.tempDictionary ;
        }

        //以下两个方法没有用到
        private void GetNodeNum(TNode now, ref int n)
        {
            if (now.left != null)
            {
                GetNodeNum(now.left, ref n);
            }
            if (now.right != null)
            {
                GetNodeNum(now.right, ref n);
            }
            n++;
        }
        private int GetNodeNum(TNode head)
        {
            int num = 0;
            if (head.value != 0)
            {
                GetNodeNum(head, ref num);
                return num;
            }
            else
            {
                return 0;
            }
        }

        //删除node指向对象 可删除任意位置节点，如被删节点存在父节点则父节点左右儿子置0
        private void DeleteNode(TNode node)
        {
            //TNode.tempDictionary = treeDictionary;
            if (node != null)
            {
                if (node.parent != null)
                {
                    if (node.parent.left == node)
                    {
                        node.parent.left = null;
                    }
                    if (node.parent.right == node)
                    {
                        node.parent.right = null;
                    }
                }
                //遍历删除节点 未处理父节点，未判断左右儿子是否存在
                if (node.left != null)
                {
                    DeleteNode(node.left);
                }
                if (node.right != null)
                {
                    DeleteNode(node.right);
                }
                //DeleteNodeTraversal(node);
            }
           // treeDictionary = TNode.tempDictionary;
        }

        //遍历删除节点 未处理父节点，未判断左右儿子是否存在
        //以下方法整合入deleteNode
        private void DeleteNodeTraversal(TNode node)
        {
            if (node.left != null)
            {
                DeleteNode(node.left);
            }
            if (node.right != null)
            {
                DeleteNode(node.right);
            }
        }
      	//未完成求导：an,arcsin,arccos,arctan
        //以下求导采用引用形式求导
		private void Diff(ref TNode now, string @var)
		{          
            if (now != null)
            {
                //TExpressionTree diffResult = new TExpressionTree();
                TNode.tempDictionary = treeDictionary;

			    switch (now.eType)
			    {
				    case enumNodeType.NODE_VARIABLE:
					    now.eType = enumNodeType.NODE_NUMBER;
					    if (now.varname == @var)
					    {
						    now.value = 1;
					    }
					    else
					    {
						    now.value = 0;
					    }
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
							    {
                                    TNode temp = now.left;
								    Diff(ref temp, @var);
                                    now.left = temp;
							    }
							    return;
						    case enumMathOperator.MATH_ADD:
						    case enumMathOperator.MATH_SUBSTRACT:
							    if (now.left != null)
							    {
                                    //Diff(ref now.left, @var);
                                    TNode temp = now.left;
                                    Diff(ref temp, @var);
                                    now.left = temp;
                                }
							    if (now.right != null)
							    {
                                    //Diff(ref now.right, @var);
                                    TNode temp = now.right;
                                    Diff(ref temp, @var);
                                    now.right = temp;
                                }
							    return;
						    case enumMathOperator.MATH_MULTIPLY:
							    if (now.left.eType == enumNodeType.NODE_NUMBER || now.right.eType == enumNodeType.NODE_NUMBER) //两个操作数中有一个是数字
							    {
								    if (now.left.eType == enumNodeType.NODE_NUMBER)
								    {
                                        TNode temp = now.right;
                                        Diff(ref temp, @var);
                                        now.right = temp; 
                                        //Diff(ref now.right, @var);
								    }
								    else
								    {
                                        //Diff(ref now.left, @var);
                                        TNode temp = now.left;
                                        Diff(ref temp, @var);
                                        now.left = temp;
                                    }
							    }
							    else
							    {
								    TNode plus;
								    plus = new TNode();
								    plus.eType = enumNodeType.NODE_OPERATOR;
								    plus.eOperator = enumMathOperator.MATH_ADD;
								    if (now != head)
								    {
									    //plus上下行连接
									    if (now.parent.left == now)
									    {
										    now.parent.left = plus;
									    }
									    if (now.parent.right == now)
									    {
										    now.parent.right = plus;
									    }
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
                                    //Diff(ref now.left, @var);
                                    TNode temp = now.left.left;
                                    Diff(ref temp, @var);
                                    now.left.left = temp;
                                    //Diff(ref plus.left.left, @var);
								    //Diff(ref plus.right.right, @var);
                                    TNode temp1 = now.right.right;
                                    Diff(ref temp1, @var);
                                    now.right.right = temp1;
                                }
							    return;
						    case enumMathOperator.MATH_DIVIDE:
							    if (now.right.eType == enumNodeType.NODE_NUMBER) // f(x)/number = f'(x)/number
							    {
                                    //Diff(ref now.left, @var);
                                    //Diff(ref now.left, @var);
                                    TNode temp = now.left;
                                    Diff(ref temp, @var);
                                    now.left = temp;
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

								    Diff(ref u1, @var);
								    Diff(ref v2, @var);

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
									    {
										    power.parent.left = multiply1;
									    }
									    if (power.parent.right == power)
									    {
										    power.parent.right = multiply1;
									    }
									    multiply1.parent = power.parent;
								    }

								    if (RChildIsNumber)
								    {
									    v.value -= 1.0;
								    }

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
									    Diff(ref u2, @var);
									    return;
								    }
								    else
								    {
									    TNode ln = NewNode(enumNodeType.NODE_FUNCTION, enumMathOperator.MATH_LN);

									    multiply2.right = ln;
									    ln.parent = multiply2;

									    ln.left = u2;
									    u2.parent = ln;

									    Diff(ref multiply2, @var);
									    return;
								    }
								    //return;
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
									    {
										    head = power;
									    }
									    else
									    {
										    if (function.parent.left == function)
										    {
											    function.parent.left = power;
										    }
										    if (function.parent.right == function)
										    {
											    function.parent.right = power;
										    }
										    power.parent = function.parent;
									    }

									    function = null;
									    Diff(ref power, @var);

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
									    function = null;

									    if (u.eType != enumNodeType.NODE_VARIABLE)
									    {
										    Diff(ref u2, @var);
									    }

								    }
								    return;
							    case enumMathOperator.MATH_EXP:
								    {
									    if (function.left.eType == enumNodeType.NODE_VARIABLE) //e^x=e^x
									    {
										    return;
									    }
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

									    Diff(ref u2, @var);
								    }
								    return;
							    case enumMathOperator.MATH_COS:
								    {
									    TNode negative = new TNode();
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

									    TNode multiply = new TNode();
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

									    Diff(ref u2, @var);
								    }
								    return;
							    case enumMathOperator.MATH_SIN:
								    {
									    TNode multiply = new TNode();
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

									    Diff(ref u2, @var);
								    }
								    //case MATH_ARCTAN:
								    //{
								    //	TNode *multiply = new TNode()
								    //}
								    return;
							    default:
								    throw new TError(enumError.ERROR_WRONG_MATH_OPERATOR, "未完成的运算符" + Function2Str(now.eOperator));
						    }
					    }
					    //break;
			    }
                treeDictionary= TNode.tempDictionary;
            }
        }

        //复制节点树，返回新节点树头节点
        private TNode CopyNodeTree(TNode oldNode)
        {
            TNode newNode = new TNode();
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
            if (TNode.tempDictionary.ContainsKey(newNode.id))
            {
                TNode.tempDictionary[newNode.id] = newNode;
            }
            else
            {
                TNode.tempDictionary.Add(newNode.id, newNode);
            }
            
            return newNode;
        }
        private void GetVariablePos(TNode now, string @var, List<TNode> VarsPos)
        {
            if (now.eType == enumNodeType.NODE_VARIABLE && now.varname == @var)
            {
                VarsPos.Add(now);
            }
            if (now.left != null)
            {
                GetVariablePos(now.left, @var, VarsPos);
            }
            if (now.right != null)
            {
                GetVariablePos(now.right, @var, VarsPos);
            }
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

        private void GetVariablePos(string @var, List<TNode> VarsPos)
        {
            GetVariablePos(head, @var, VarsPos);
        }
        private void CopyVariableTable(List<string> Dest, List<string> source)
        {
            Dest.Clear();
            foreach (var sz in source)
            {
                Dest.Add(sz);
            }
        }
        //void ReplaceNodeVariable(TNode *now, std::vector<String > &newVariableTable);
        private bool CanCalc(TNode now)
        {
            if (now.left != null)
            {
                if (CanCalc(now.left) == false)
                {
                    return false;
                }
            }
            if (now.right != null)
            {
                if (CanCalc(now.right) == false)
                {
                    return false;
                }
            }

            if (now.eType == enumNodeType.NODE_VARIABLE)
            {
                return false;
            }
            return true;
        }

        //迭代计算，最终剩下1个节点
        private void Calc(TNode now)
        {
            if (now.left != null)
            {
                Calc(now.left);
            }
            if (now.right != null)
            {
                Calc(now.right);
            }

            if (GetOperateNum(now.eOperator) == 1 && now.left.eType == enumNodeType.NODE_NUMBER)
            {
                try
                {
                    CalcNode(now, now.left);
                }
                catch (TError err)
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
                catch (TError err)
                {
                    throw err;
                }
                now.left = null;
                now.right = null;
            }
        }
  
        private TNode NewNode(enumNodeType eType, enumMathOperator eOperator = enumMathOperator.MATH_NULL)
        {
            TNode newNode = new TNode();
            newNode.eType = eType;
            newNode.eOperator = eOperator;
            TNode.tempDictionary.Add(newNode.id, newNode);
            return newNode;
        }
        private void ReleaseVectorTNode(List<TNode> vec)
        {
            vec = null;
        }

        //仅将变量表内置数值代入，不进行计算
        public void Vpa()
        {
            Vpa_inner(ref head);
        }
        private void Vpa_inner(ref TNode now)
        {
            if (now.left != null)
            {
                
                TNode temp = now.left;
                Vpa_inner(ref temp);
                now.left = temp;
                //Vpa_inner(ref now.left);
            }
            if (now.right != null)
            {
                //Vpa_inner(ref now.right);
                TNode temp = now.right;
                Vpa_inner(ref temp);
                now.right = temp;
            }

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

                        write_pos.left = new TNode();

                        write_pos.left.parent = write_pos;
                        //Solve(parent, ref write_pos.left);
                        TNode temp =write_pos.left;
                        Solve(parent, ref temp);
                        write_pos.left = temp;
                        
                        break;
                    case enumMathOperator.MATH_MULTIPLY:
                        write_pos.eType = enumNodeType.NODE_OPERATOR;
                        write_pos.eOperator = enumMathOperator.MATH_DIVIDE;

                        write_pos.right = CopyNodeTree(brother);
                        write_pos.right.parent = write_pos;

                        write_pos.left = new TNode();

                        write_pos.left.parent = write_pos;
                        //Solve(parent, ref write_pos.left);
                        TNode temp1 = write_pos.left;
                        Solve(parent, ref temp1);
                        write_pos.left = temp1;
                        break;
                    case enumMathOperator.MATH_SUBSTRACT: //分左右
                        if (bVarIsLeft) //被减数
                        {
                            write_pos.eType = enumNodeType.NODE_OPERATOR;
                            write_pos.eOperator = enumMathOperator.MATH_ADD;

                            write_pos.right = CopyNodeTree(brother);
                            write_pos.right.parent = write_pos;

                            write_pos.left = new TNode();

                            write_pos.left.parent = write_pos;
                            //Solve(parent, ref write_pos.left);
                            TNode temp2a = write_pos.left;
                            Solve(parent, ref temp2a);
                            write_pos.left = temp2a;
                        }
                        else
                        {
                            write_pos.eType = enumNodeType.NODE_OPERATOR;
                            write_pos.eOperator = enumMathOperator.MATH_SUBSTRACT;

                            write_pos.left = CopyNodeTree(brother);
                            write_pos.left.parent = write_pos;

                            write_pos.right = new TNode();

                            write_pos.right.parent = write_pos;
                            //Solve(parent, ref write_pos.right);
                            TNode temp2b = write_pos.right;
                            Solve(parent, ref temp2b);
                            write_pos.right = temp2b;
                        }
                        break;
                    case enumMathOperator.MATH_DIVIDE: //分左右
                        if (bVarIsLeft) //被除数
                        {
                            write_pos.eType = enumNodeType.NODE_OPERATOR;
                            write_pos.eOperator = enumMathOperator.MATH_MULTIPLY;

                            write_pos.right = CopyNodeTree(brother);
                            write_pos.right.parent = write_pos;

                            write_pos.left = new TNode();

                            write_pos.left.parent = write_pos;
                            //Solve(parent, ref write_pos.left);
                            TNode temp2c = write_pos.left;
                            Solve(parent, ref temp2c);
                            write_pos.left = temp2c;

                        }
                        else
                        {
                            write_pos.eType = enumNodeType.NODE_OPERATOR;
                            write_pos.eOperator = enumMathOperator.MATH_DIVIDE;

                            write_pos.left = CopyNodeTree(brother);
                            write_pos.left.parent = write_pos;

                            write_pos.right = new TNode();

                            write_pos.right.parent = write_pos;
                            //Solve(parent, ref write_pos.right);
                            TNode temp2d = write_pos.right;
                            Solve(parent, ref temp2d);
                            write_pos.right = temp2d;
                        }
                        break;
                    case enumMathOperator.MATH_POSITIVE:
                        write_pos.eType = enumNodeType.NODE_OPERATOR;
                        write_pos.eOperator = enumMathOperator.MATH_POSITIVE;

                        write_pos.left = new TNode();

                        write_pos.left.parent = write_pos;
                        //Solve(parent, ref write_pos.left);
                        TNode temp2 = write_pos.left;
                        Solve(parent, ref temp2);
                        write_pos.left = temp2;
                        break;
                    case enumMathOperator.MATH_NEGATIVE:
                        write_pos.eType = enumNodeType.NODE_OPERATOR;
                        write_pos.eOperator = enumMathOperator.MATH_NEGATIVE;

                        write_pos.left = new TNode();

                        write_pos.left.parent = write_pos;
                        //Solve(parent, ref write_pos.left);
                        TNode temp3 = write_pos.left;
                        Solve(parent, ref temp3);
                        write_pos.left = temp3;
                        break;
                    case enumMathOperator.MATH_SIN:
                        write_pos.eType = enumNodeType.NODE_FUNCTION;
                        write_pos.eOperator = enumMathOperator.MATH_ARCSIN;

                        write_pos.left = new TNode();

                        write_pos.left.parent = write_pos;
                        //Solve(parent, ref write_pos.left);
                        TNode temp2e = write_pos.left;
                        Solve(parent, ref temp2e);
                        write_pos.left = temp2e;
                        break;
                    case enumMathOperator.MATH_COS:
                        write_pos.eType = enumNodeType.NODE_FUNCTION;
                        write_pos.eOperator = enumMathOperator.MATH_ARCCOS;

                        write_pos.left = new TNode();

                        write_pos.left.parent = write_pos;
                        //Solve(parent, ref write_pos.left);
                        TNode temp2f = write_pos.left;
                        Solve(parent, ref temp2f);
                        write_pos.left = temp2f;
                        break;
                    case enumMathOperator.MATH_TAN:
                        write_pos.eType = enumNodeType.NODE_FUNCTION;
                        write_pos.eOperator = enumMathOperator.MATH_ARCTAN;

                        write_pos.left = new TNode();

                        write_pos.left.parent = write_pos;
                        //Solve(parent, ref write_pos.left);
                        TNode temp2g = write_pos.left;
                        Solve(parent, ref temp2g);
                        write_pos.left = temp2g;
                        break;
                    default:
                        //Debug.Assert(0);
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
            {
                CheckOnlyOneVar(now.left);
            }

            if (now.right != null)
            {
                CheckOnlyOneVar(now.right);
            }
        }
        private void Subs_inner(TNode node, string ptVar, double value)
        {
            if (node.eType == enumNodeType.NODE_VARIABLE && node.varname == ptVar)
            {
                node.eType = enumNodeType.NODE_NUMBER;
                node.value = value;
                return;
            }

            if (node.left != null)
            {
                Subs_inner(node.left, ptVar, value);
            }

            if (node.right != null)
            {
                Subs_inner(node.right, ptVar, value);
            }
        }



        public void Reset()
        {
            str_expression = "";
            head = null;
            pVariableTable = null;

            iVarAppearedCount = 0;
        }

        
        public void LinkVariableTable(TVariableTable p)
        {
            pVariableTable = p;
        }

        //读之前不清零，请自行处理
        public List<TNode> Read(string expression)
        {
            Queue<TNode> InOrder = new Queue<TNode>();
            List<TNode> PostOrder = new List<TNode>();

            InOrder = ReadToInOrder(expression);
            
            PostOrder = InQueue2PostQueue(InOrder);
            return BuildExpressionTree(PostOrder);
        }
        public void SetExpression(string expression)
        {
            str_expression = expression;
        }
        public void Read(double num, bool bOutput)
        {
            head = NewNode(enumNodeType.NODE_NUMBER);
            head.value = num;
        }



        ////求解单变量方程 不验证可求解性，需提前调用HasOnlyOneVar确认 不改动表达式内容
        public string Solve(ref string @var, ref double value)
        {
            TExpressionTree Result = new TExpressionTree();

            TNode ResultNow = new TNode();

            @var = LastVarNode.varname;

            Solve(LastVarNode, ref ResultNow);

            Result.head = ResultNow;

            value = Result.Value(true);

            return OutputStr();

        }

        public string OutputStr()
        {
            string temp = "";

            if (head != null)
            {
                temp=TraverseInOrder(head);
            }
            return temp;
        }
        public override string ToString()
        {
            string result = "";
            foreach(TNode t in TreeNodes)
            {
                result += t.ToString();
            }

            return result;
        }
        public TExpressionTree()
        {
            Reset();
        }
        public void Dispose()
        {
            Release();
        }
        /// <summary>
        /// 化简
        /// </summary>
        /// <param name="bOutput"></param>
        public void Simplify(bool bOutput)
        {
            Simplify(head);
        }
        //对变量求导
        public TExpressionTree Diff(string @var, int n)
        {
            TExpressionTree diffExpressionTree = this;
            if (pVariableTable.FindVariableTable(@var) == pVariableTable.VariableTable.Count)
            {
                throw new TError(enumError.ERROR_UNDEFINED_VARIABLE, @var);
            }

            for (int i = 0; i < n; i++)
            {
                Diff(ref diffExpressionTree.head, @var);
            }
            return diffExpressionTree;
        }

        public void Subs(string ptVar, double value, bool output)
        {
            Subs_inner(head, ptVar, value);
        }

        ////vars为被替换变量，nums为替换表达式，以空格分隔
        public void Subs(string vars, string nums, bool output)
        {
            if (string.IsNullOrEmpty(vars) || string.IsNullOrEmpty(nums))
            {
                throw new TError(enumError.ERROR_EMPTY_INPUT, "");
                //return;
            }

            List<string> VarsVector = StrSliceToVector(vars);
            List<string> NumsVector = StrSliceToVector(nums);

            if (VarsVector.Count == NumsVector.Count) //替换与被替换元素数目相等
            {
                for (int i = 0; i < VarsVector.Count; i++) //遍历被替换变量
                {
                    //查表识别被替换变量		
                    var it = pVariableTable.FindVariableTable(VarsVector[i]);
                    if (it != pVariableTable.VariableTable.Count) //已识别出
                    {
                        string @var = VarsVector[it];
                        //构建替换节点树
                        TExpressionTree Expr = new TExpressionTree();
                        Expr.LinkVariableTable(pVariableTable);
                        Expr.Read(NumsVector[i]);

                        //得到所有被替换变量的位置
                        List<TNode> VarsPos = new List<TNode>();
                        GetVariablePos(@var, VarsPos);
                        for (int j = 0; j < VarsPos.Count; j++)
                        {
                            TNode newNode = CopyNodeTree(Expr.head);

                            //连接到新节点
                            if (VarsPos[j] != head)
                            {
                                if (VarsPos[j].parent.left != null && VarsPos[j].parent.left == VarsPos[j])
                                {
                                    VarsPos[j].parent.left = newNode;
                                }
                                if (VarsPos[j].parent.right != null && VarsPos[j].parent.right == VarsPos[j])
                                {
                                    VarsPos[j].parent.right = newNode;
                                }
                                newNode.parent = VarsPos[j].parent;
                            }
                            else
                            {
                                head = newNode;
                            }

                            //删掉旧节点
                            VarsPos[j] = null;
                        }
                    }
                }
            }
            else
            {
                throw new TError(enumError.ERROR_SUBS_NOT_EQUAL, "");
            }

        }
        public List<string> StrSliceToVector(string source)
        {
            List<string> result = new List<string>();
            char[] chars = source.ToCharArray();
            foreach (char a in chars)
            {
                string temp = a.ToString();
                result.Add(temp);
            }
            return result;
        }

        //替换 VarsVector变量 NumsVector数字
        public void Subs(List<string> VarsVector, List<double> NumsVector, bool output)
        {
            if (VarsVector.Count == NumsVector.Count) //替换与被替换元素数目相等
            {
                for (int i = 0; i < VarsVector.Count; i++) //遍历被替换变量
                {
                    //查表识别被替换变量		
                    var it = pVariableTable.FindVariableTable(VarsVector[i]);
                    if (it != pVariableTable.VariableTable.Count) //已识别出
                    {

                        string @var = VarsVector[it];
                        //构建替换节点树
                        TExpressionTree Expr = new TExpressionTree();
                        Expr.LinkVariableTable(pVariableTable);
                        Expr.Read(NumsVector[i], false);

                        //得到所有被替换变量的位置
                        List<TNode> VarsPos = new List<TNode>();
                        GetVariablePos(@var, VarsPos);
                        for (int j = 0; j < VarsPos.Count; j++)
                        {
                            TNode newNode = CopyNodeTree(Expr.head);

                            //连接到新节点
                            if (VarsPos[j] != head)
                            {
                                if (VarsPos[j].parent.left != null && VarsPos[j].parent.left == VarsPos[j])
                                {
                                    VarsPos[j].parent.left = newNode;
                                }
                                if (VarsPos[j].parent.right != null && VarsPos[j].parent.right == VarsPos[j])
                                {
                                    VarsPos[j].parent.right = newNode;
                                }
                                newNode.parent = VarsPos[j].parent;
                            }
                            else
                            {
                                head = newNode;
                            }

                            //删掉旧节点
                            VarsPos[j] = null;
                        }
                    }
                }
            }
            else
            {
                throw new TError(enumError.ERROR_SUBS_NOT_EQUAL, "");
            }
        }
        //检查是否还有变量存在，可以计算则返回true
        public bool CanCalc()
        {
            return CanCalc(head);
        }
        //检查是否为一元(not used)
        public bool IsSingleVar()
        {
            //return SelfVariableTable.VariableTable.size() == 1;
            return true;
        }
        //只有一个变量（只有刚read才有效）
        public bool HasOnlyOneVar()
        {
            return iVarAppearedCount == 1;
        }
        //只有一个变量（实时验证）
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
            {
                pNode = head;
            }
            else
            {
                pNode = CopyNodeTree(head);
            }

            try
            {
                Calc(pNode);
            }
            catch (TError err)
            {
                //删掉节点树并提交给上级
                if (operateHeadNode == false)
                {
                    DeleteNode(pNode);
                }
                throw err;
            }

            //得到最终结果
            double num = pNode.value;
            //释放复制的树
            if (operateHeadNode == false)
            {
                pNode = null;
            }
            return num;
        }

        //计算表达式的值，若传入了result则把结果存入。返回值为结果字符串或表达式串。
        public string Calc()
        {
            System.Nullable<double> tempVar = null;
            return Calc(ref tempVar);
        }
        public string Calc(ref Nullable<double> result)
        {
            if (CanCalc())
            {
                TNode Duplicate = CopyNodeTree(head);
                Calc(Duplicate);

                if (result != null)
                {
                    result = Duplicate.value;
                }

                string temp = Node2Str(Duplicate);
                Duplicate = null;

                return temp;
            }
            else
            {
                return OutputStr();
            }
        }

        //ORIGINAL LINE: TExpressionTree& operator =(const TExpressionTree& expr)
        public TExpressionTree CopyFrom(TExpressionTree expr)
        {
            Release();
            Reset();
            head = CopyNodeTree(expr.head);
            treeDictionary = expr.treeDictionary;
            LinkVariableTable(expr.pVariableTable);
            return this;
        }
        public static TExpressionTree operator +(TExpressionTree ImpliedObject, TExpressionTree expr)
        {
            if (ImpliedObject.head == null)
            {
                ImpliedObject.head = ImpliedObject.CopyNodeTree(expr.head);
            }
            else
            {
                TNode Add = ImpliedObject.NewNode(enumNodeType.NODE_OPERATOR, enumMathOperator.MATH_ADD);
                TNode Right = ImpliedObject.CopyNodeTree(expr.head);


                Add.left = ImpliedObject.head;
                Add.right = Right;

                ImpliedObject.head.parent = Add;
                Right.parent = Add;

                ImpliedObject.head = Add;
            }
            return ImpliedObject;
        }
        public static TExpressionTree operator *(TExpressionTree ImpliedObject, double value)
        {
            if (ImpliedObject.head == null)
            {
                throw new TError(enumError.ERROR_EMPTY_INPUT, "");
                //return ImpliedObject;
            }

            TNode Multiply = ImpliedObject.NewNode(enumNodeType.NODE_OPERATOR, enumMathOperator.MATH_MULTIPLY);
            TNode Value = ImpliedObject.NewNode(enumNodeType.NODE_NUMBER);
            Value.value = value;

            Multiply.left = ImpliedObject.head;
            Multiply.right = Value;

            ImpliedObject.head.parent = Multiply;
            Value.parent = Multiply;

            ImpliedObject.head = Multiply;

            return ImpliedObject;
        }
        public static TExpressionTree operator +(TExpressionTree ImpliedObject, double value)
        {
            if (ImpliedObject.head == null)
            {
                throw new TError(enumError.ERROR_EMPTY_INPUT, "");
                //return ImpliedObject;
            }

            TNode Add = ImpliedObject.NewNode(enumNodeType.NODE_OPERATOR, enumMathOperator.MATH_ADD);
            TNode Value = ImpliedObject.NewNode(enumNodeType.NODE_NUMBER);
            Value.value = value;

            Add.left = ImpliedObject.head;
            Add.right = Value;

            ImpliedObject.head.parent = Add;
            Value.parent = Add;

            ImpliedObject.head = Add;

            return ImpliedObject;
        }
        public static TExpressionTree operator -(TExpressionTree ImpliedObject, double value)
        {
            if (ImpliedObject.head == null)
            {
                throw new TError(enumError.ERROR_EMPTY_INPUT, "");
                //return ImpliedObject;
            }

            TNode Substract = ImpliedObject.NewNode(enumNodeType.NODE_OPERATOR, enumMathOperator.MATH_SUBSTRACT);
            TNode Value = ImpliedObject.NewNode(enumNodeType.NODE_NUMBER);
            Value.value = value;

            Substract.left = ImpliedObject.head;
            Substract.right = Value;

            ImpliedObject.head.parent = Substract;
            Value.parent = Substract;

            ImpliedObject.head = Substract;

            return ImpliedObject;
        }

      public TNode? GetParentNode(TNode now)
        {
            if (!now.parentId.HasValue)
            {
                return null;
            }
            if (!treeDictionary.ContainsKey(now.parentId.Value))
            {
                throw new Exception("表达式树中没有此父对象");
            }
            TNode result = treeDictionary[now.parentId.Value];
            return result;
        }
        public TNode? GetLeftNode(TNode now)
        {
            if (!now.leftId.HasValue)
            {
                return null;
            }
            if (!treeDictionary.ContainsKey(now.leftId.Value))
            {
                throw new Exception("表达式树中没有此父对象");
            }
            TNode result = treeDictionary[now.leftId.Value];
            return result;
        }
        public TNode? GetRightNode(TNode now)
        {
            if (!now.rightId.HasValue)
            {
                return null;
            }
            if (!treeDictionary.ContainsKey(now.rightId.Value))
            {
                throw new Exception("表达式树中没有此父对象");
            }
            TNode result = treeDictionary[now.rightId.Value];
            return result;
        }
        public List<TNode> GetNextLevelNodes(TNode now)
        {
            List<TNode> result = new List<TNode>();
            TNode? left = GetLeftNode(now);
            TNode? right = GetRightNode(now);
            if (left != null)
            {
                result.Add(left);
            }
            if (right != null)
            {
                result.Add(right);
            }
            return result;
        }
        public List<TNode> GetNextLevelNodes(List<TNode> nodes)
        {
            List<TNode> result = new List<TNode>();
            foreach(TNode node in nodes)
            {
                List<TNode> temps = GetNextLevelNodes(node);
                if (temps.Count != 0)
                {
                    result.AddRange(temps);
                }
            }
            return result;
        }
        public void SetParentNode(TNode now,TNode? parentNode=null)
        {
            if (parentNode != null)
            {
                now.parentId = parentNode.id;
                //添加父亲
                if (treeDictionary.ContainsKey(parentNode.id))
                {
                    treeDictionary[parentNode.id] = parentNode;
                }
                else
                {
                    treeDictionary.Add(parentNode.id, parentNode);
                }
            }
            else//父节点为空，即删除父节点，本节点变为根节点
            {
                //删除静态字段中parent
                if (now.parentId.HasValue && treeDictionary.ContainsKey(now.parentId.Value))
                {
                    treeDictionary.Remove(now.parentId.Value);
                }
                now.parentId = null;
            }
            //修改treeDictionary
            if (treeDictionary.ContainsKey(now.id))
            {
                treeDictionary[now.id] = now;
            }
        }
          
        //public void AddChildrenNodesToTree(TNode now)
        //{
        //    List<TNode> childrenNodes = ListAllChildrenNodes(now);
        //    foreach(var item in childrenNodes)
        //    {
        //        AddOneNodeToTree(item);
        //    }
        //}
        public void AddOneNodeToTree(TNode now)
        {
            if (treeDictionary.ContainsKey(now.id))
            {
                treeDictionary.Remove(now.id);
            }
            treeDictionary.Add(now.id, now);
        }
        public List<TNode> ListAllChildrenNodes(TNode now)
        {
            List<TNode> result = new List<TNode>();
            List<TNode> temps = GetNextLevelNodes(now);
            if (temps.Count != 0)
            {
                result.AddRange(temps);
            }
            while (temps.Count != 0)
            {
                temps = GetNextLevelNodes(temps);
                if (temps.Count != 0)
                {
                    result.AddRange(temps);
                }
            }
            return result;
        }


        }
}

**/