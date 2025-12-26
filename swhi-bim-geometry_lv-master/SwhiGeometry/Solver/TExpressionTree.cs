
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Lv.BIM.Solver
{
    public class TNode //: BinaryTreeNode<TNode>
    {
        //lvwan改写,用来临时存储表达式树，以获取所有parent\left\right

        //private int _id;
        //public int id => _id;
        private enumNodeType _eType;
        public enumNodeType eType
        {
            get { return _eType; }
            set
            {
                _eType = value;
            }
        }
        private enumMathOperator _eOperator;
        public enumMathOperator eOperator
        {
            get { return _eOperator; }
            set
            {
                _eOperator = value;
            }
        }
        private double _value;
        public double value
        {
            get { return _value; }
            set
            {
                _value = value;
            }
        }
        private string _varname;
        public string varname
        {
            get { return _varname; }
            set
            {
                _varname = value;
            }
        }
        public TNode parent;
        public TNode left;
        public TNode right;
#nullable enable
        //所有父子关系均保存在以下三个ID中
        //public int? parentId => parent == null ? null : parent.id;//父ID
        //public int? leftId => left == null ? null : left.id;//左儿子
        //public int? rightId => right == null ? null : right.id;//右儿子
        //public bool IsHead => parentId == null;
        //public bool IsLast => leftId == null && rightId == null;
        //考虑以下情况赋值：ignore.parent.left = child;暂时考虑此种情形可行
        //设置结束后需要将临时变量tempDictionary付给treeDictionary

        //// 单个元素  
        public TNode()
        {
            _eType = enumNodeType.NODE_NULL;
            _eOperator = enumMathOperator.MATH_NULL;
            _value = 0;
            //_id = GetHashCode();
        }
        //数值节点
        public TNode(double aa)
        {
            _eType = enumNodeType.NODE_NUMBER;
            _eOperator = enumMathOperator.MATH_NULL;
            _value = aa;
            //_id = GetHashCode();
        }
        //变量节点
        public TNode(string @var)
        {
            this._eType = enumNodeType.NODE_VARIABLE;
            this._eOperator = enumMathOperator.MATH_NULL;
            this._varname = @var;
            //_id = GetHashCode();
        }
        public string ContentToString()
        {
            string result = "";
            switch (eType)
            {
                case enumNodeType.NODE_NULL:
                    result = "";
                    break;
                case enumNodeType.NODE_NUMBER:
                    result = value.ToString();
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
            if (this.left != null)
            {
                result = "("+result+",";
                result +=  left.ToString();
                if (right == null)
                {
                    result += ")";
                }
            }
            if (right != null)
            {
                result = result + ","; 
                result += right.ToString();
                result += ")";
            }
            return result;
        }

        public bool IsNumber(double number=0)
        {
            if(_eType == enumNodeType.NODE_NUMBER && _value == number)
            {
                return true;
            }
            return false;
        }
        public static TNode operator +(TNode leftin,TNode rightin)
        {
            TNode left = leftin.Copy();
            TNode right = rightin.Copy();
            if(left.IsNumber()) 
            {
                return right;
            }
            if (right.IsNumber())
            {
                return left;
            }
            TNode result = new TNode();
            result.eType = enumNodeType.NODE_OPERATOR;
            result.eOperator = enumMathOperator.MATH_ADD;
            left.parent = result;
            right.parent = result;
            result.left=left;
            result.right = right;
            
            return result;
        }
        public static TNode operator -(TNode leftin)
        {
            TNode left = leftin.Copy();
            TNode result = new TNode();
            result.eType = enumNodeType.NODE_OPERATOR;
            result.eOperator = enumMathOperator.MATH_NEGATIVE;
            left.parent = result;
            result.left = left;            
            
            return result;
        }
        public static TNode operator -(TNode leftin, TNode rightin)
        {
            TNode left = leftin.Copy();
            TNode right = rightin.Copy();
            if (right.IsNumber())
            {
                return left;
            }
            TNode result = new TNode();
            result.eType = enumNodeType.NODE_OPERATOR;
            result.eOperator = enumMathOperator.MATH_SUBSTRACT;
            left.parent = result;
            right.parent = result;
            result.left = left;
            result.right = right;
          
            return result;
        }
        public static TNode operator -(double leftnum, TNode rightin)
        {
            
            TNode right = rightin.Copy();
            TNode result = new TNode();
            result.eType = enumNodeType.NODE_OPERATOR;
            result.eOperator = enumMathOperator.MATH_SUBSTRACT;
            TNode left = new TNode(leftnum);
            if (right.IsNumber())
            {
                return left;
            }
            left.parent = result;
            right.parent = result;
            result.left = left;
            result.right = right;
      
            return result;
        }
        public static TNode operator *(TNode leftin, TNode rightin)
        {
            if (rightin.IsNumber() || leftin.IsNumber())
            {
                return new TNode(0.0);
            }
            TNode left = leftin.Copy();
            TNode right = rightin.Copy();
            TNode result = new TNode();
            result.eType = enumNodeType.NODE_OPERATOR;
            result.eOperator = enumMathOperator.MATH_MULTIPLY;
            left.parent = result;
            right.parent = result;
            result.left = left;
            result.right = right;

            return result;
        }
        public static TNode operator /(TNode leftin, TNode rightin)
        {
            if (leftin.IsNumber())
            {
                return new TNode(0.0);
            }
            TNode left = leftin.Copy();
            TNode right = rightin.Copy();
            TNode result = new TNode();
            result.eType = enumNodeType.NODE_OPERATOR;
            result.eOperator = enumMathOperator.MATH_DIVIDE;
            left.parent = result;
            right.parent = result;
            result.left = left;
            result.right = right;

            return result;
        }
        public static TNode operator ^(TNode leftin, TNode rightin)
        {
            if (leftin.IsNumber())
            {
                return new TNode(0.0);
            }
            if (rightin.IsNumber())
            {
                return leftin.Copy();
            }
            TNode left = leftin.Copy();
            TNode right = rightin.Copy();
            TNode result = new TNode();
            result.eType = enumNodeType.NODE_OPERATOR;
            result.eOperator = enumMathOperator.MATH_POWER;
            left.parent = result;
            right.parent = result;
            result.left = left;
            result.right = right;
            
            return result;
        }
       
        public static TNode operator +(TNode leftin, double rightnum)
        {
            if (leftin.IsNumber())
            {
                return new TNode(rightnum);
            }
            TNode left = leftin.Copy();
            TNode result = new TNode();
            result.eType = enumNodeType.NODE_OPERATOR;
            result.eOperator = enumMathOperator.MATH_ADD;
            
            TNode right = new TNode(rightnum);
            left.parent = result;
            right.parent = result;
            result.left = left;
            result.right = right;
            
            return result;
        }
        public static TNode operator -(TNode leftin, double rightnum)
        {
            if (leftin.IsNumber())
            {
                return new TNode(-rightnum);
            }
            TNode left = leftin.Copy();
            TNode result = new TNode();
            result.eType = enumNodeType.NODE_OPERATOR;
            result.eOperator = enumMathOperator.MATH_SUBSTRACT;
            
            TNode right = new TNode(rightnum);
            left.parent = result;
            right.parent = result;
            result.right = right;
            result.left = left;
            return result;
        }
        public static TNode operator *(TNode leftin, double rightnum)
        {
            if (leftin.IsNumber())
            {
                return new TNode(0.0);
            }
            TNode left = leftin.Copy();
            TNode result = new TNode();
            result.eType = enumNodeType.NODE_OPERATOR;
            result.eOperator = enumMathOperator.MATH_MULTIPLY;
            
            TNode right = new TNode(rightnum);
            left.parent = result;
            right.parent = result;
            result.right = right;
            result.left = left;
            return result;
        }
        public static TNode operator /(TNode leftin, double rightnum)
        {
            if (leftin.IsNumber())
            {
                return new TNode(0.0);
            }
            TNode left = leftin.Copy();
            TNode result = new TNode();
            result.eType = enumNodeType.NODE_OPERATOR;
            result.eOperator = enumMathOperator.MATH_DIVIDE;            
            TNode right=new TNode(rightnum);
            left.parent = result;
            right.parent = result;
            result.right = right;
            result.left = left;
            return result;
        }
        public static TNode operator /(double leftnum, TNode rightin)
        {
            TNode right = rightin.Copy();
            TNode result = new TNode();
            result.eType = enumNodeType.NODE_OPERATOR;
            result.eOperator = enumMathOperator.MATH_DIVIDE;
            TNode left = new TNode(leftnum);
            left.parent = result;
            right.parent = result;
            result.left = left;
            result.right = right;

            return result;
        }
        public static TNode operator ^(TNode leftin, double rightnum)
        {
            if (leftin.IsNumber())
            {
                return new TNode(0.0);
            }
            TNode left = leftin.Copy();
            TNode result = new TNode();
            result.eType = enumNodeType.NODE_FUNCTION;
            result.eOperator = enumMathOperator.MATH_POWER;            
            TNode right = new TNode(rightnum);            
            left.parent = result;
            right.parent = result;
            result.left = left;
            result.right = right;
            return result;
        }
        public static TNode Sin(TNode leftin)
        {
            if (leftin.IsNumber())
            {
                return new TNode(0.0);
            }
            TNode left = leftin.Copy();
            TNode result = new TNode();
            result.eType = enumNodeType.NODE_FUNCTION;
            result.eOperator = enumMathOperator.MATH_SIN;
            left.parent = result;
            result.left = left;

            return result;
        }
        public static TNode Cos(TNode leftin)
        {
            if (leftin.IsNumber())
            {
                return new TNode(1.0);
            }
            TNode left = leftin.Copy();
            TNode result = new TNode();
            result.eType = enumNodeType.NODE_FUNCTION;
            result.eOperator = enumMathOperator.MATH_COS;            
            left.parent = result;
            result.left = left;
            return result;
        }
        public static TNode Ln(TNode leftin)
        {
            TNode left = leftin.Copy();
            TNode result = new TNode();
            result.eType = enumNodeType.NODE_FUNCTION;
            result.eOperator = enumMathOperator.MATH_LN;
            left.parent = result;
            result.left = left;

            return result;
        }
        public static TNode Sqrt(TNode leftin)
        {
            TNode left = leftin.Copy();
            if (left.IsNumber())
            {
                return new TNode(0.0);
            }
            TNode result = new TNode();
            result.eType = enumNodeType.NODE_FUNCTION;
            result.eOperator = enumMathOperator.MATH_SQRT;
            left.parent = result;
            result.left = left;
            
            return result;
        }
        private TNode Copy()
        {
            TNode oldNode = this;
            if (oldNode == null)
            {
                return null;
            }
            TNode newNode = new TNode();
            newNode.eType = oldNode.eType;
            newNode.eOperator = oldNode.eOperator;
            newNode.value = oldNode.value;
            if (!string.IsNullOrEmpty(oldNode.varname)) { newNode.varname = oldNode.varname; }
            if (oldNode.left != null)
            {
                newNode.left = oldNode.left.Copy();

            }
            if (oldNode.right != null)
            {
                newNode.right = oldNode.right.Copy();

            }
            if (oldNode.parent != null)
            {
                newNode.parent = oldNode.parent;

            }

            return newNode;
        }
        public bool IsContainsVar(string var)
        {
            List<string> basefunc = new List<string> { "sin", "cos", "tan", "asin", "acos", "atan", "ln", "log", "exp", "sqrt", "pow" };
            string expstr = ToString().ToLower();
            foreach (string item in basefunc)
            {
                expstr = expstr.Replace(item, "");
            }
            return expstr.Contains(var);
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
    public class TExpressionTree //: BinaryTree<TNode>//,System.IDisposable
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
        //private List<TNode> tree_nodes;
        //private List<TNode> TreeNodes => tree_nodes;
        public TExpressionTree()
        {
            Reset();
        }
        public void Reset()
        {
            str_expression = "";
            head = null;
            pVariableTable = null;

            iVarAppearedCount = 0;
        }

        //运算符性质函数
        #region enumMathOperator
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

        //返回运算符的优先级
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
        #endregion enumMathOperator
        #region 字符串判别
        //是基本运算符()+-* /^&|%  
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
                case enumMathOperator.MATH_POWER:
                    return "pow";
            }
            throw new TError(enumError.ERROR_WRONG_MATH_OPERATOR, "value=" + eOperator.ToString());
        }
        //是整数 且 为偶数
        public bool IsIntAndEven(double n)
        {
            int i = (int)n;
            if (Math.Abs(n - i) < MIN_DOUBLE)
                if (i % 2 == 0)
                    return true;
            return false;
        }
        #endregion 字符串判别
        //TVariableTable SelfVariableTable;
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

                            PostOrderResult[i].right = tempStack.Peek();//更改前：pNodenow.right = tempStack.Peek();
                            tempStack.Peek().parent = pNodeNow;
                            tempStack.Pop();

                            if (tempStack.Count == 0)
                            {
                                //释放所有TNode，报错

                                ReleaseVectorTNode(PostOrder);
                                //Stack只是对PostOrder的重排序，所以不用delete
                                throw new TError(enumError.ERROR_WRONG_EXPRESSION, "");
                                //return;
                            }

                            PostOrderResult[i].left = tempStack.Peek();//更改前：pNodenow.left  = tempStack.Peek();
                            tempStack.Peek().parent = pNodeNow;
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
                            PostOrderResult[i].left = tempStack.Peek();//更改前：pNodenow.left  = tempStack.Peek();
                                                                       //pNodenow.left = tempStack.Peek();
                            tempStack.Peek().parent = pNodeNow;
                            tempStack.Pop();

                            tempStack.Push(pNodeNow);
                        }
                        break;
                }
            }

            //令表达式树等于TNode静态变量中的treeDictionary；lvwan
            //treeDictionary = TNode.treeDictionary;
            //找出根节点
            head = PostOrderResult[0];
            while (head.parent != null)
            {
                head = head.parent as TNode;
            }
            //tree_nodes = PostOrderResult;
            return PostOrderResult;
        }
        public void SetExpression(string expression)
        {
            str_expression = expression;
            Queue<TNode> InOrder = new Queue<TNode>();
            List<TNode> PostOrder = new List<TNode>();

            InOrder = ReadToInOrder(expression);

            PostOrder = InQueue2PostQueue(InOrder);
            
            BuildExpressionTree(PostOrder);
        }
        //读之前不清零，请自行处理
        public void Read(string expression)
        {
            str_expression= expression;
            Queue<TNode> InOrder = new Queue<TNode>();
            List<TNode> PostOrder = new List<TNode>();

            InOrder = ReadToInOrder(expression);

            PostOrder = InQueue2PostQueue(InOrder);
            //tree_nodes =
            BuildExpressionTree(PostOrder);
        }
     
        public void Read(double num, bool bOutput)
        {
            head = NewNode(enumNodeType.NODE_NUMBER);
            head.value = num;
        }
        //化简表达式树
        private void Simplify(TNode now)
        {

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
                    now=CalcNode(now, now.left, now.right);
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

            //str_expression = OutputStr();
        }
        /// <summary>
        /// 化简
        /// </summary>
        /// <param name="bOutput"></param>
        public void Simplify()
        {
            Simplify(head);
        }
        public void StringExpressionSimplify()
        {
            str_expression = str_expression.Replace("-0+", "+");
            str_expression = str_expression.Replace("+0-", "-");
            str_expression = str_expression.Replace("+0+", "+");
            str_expression = str_expression.Replace("-0-", "-");
            str_expression = str_expression.Replace("*1*", "*");
            str_expression = str_expression.Replace("*1/", "/");
            str_expression = str_expression.Replace("*1+", "+");
            str_expression = str_expression.Replace("*1-", "-");
            str_expression = str_expression.Replace("/1*", "*");
            str_expression = str_expression.Replace("/1/", "/");
            str_expression = str_expression.Replace("/1+", "+");
            str_expression = str_expression.Replace("/1-", "-");
            str_expression = str_expression.Replace("^1*", "*");
            str_expression = str_expression.Replace("^1/", "/");
            str_expression = str_expression.Replace("^1+", "+");
            str_expression = str_expression.Replace("^1-", "-");

        }

        //未完成求导：an,
        //以下求导采用引用形式求导
        private TNode Diff(TNode now, string @var)
        {
            TNode result = CopyNodeTree(now);
            enumNodeType ty = now.eType;
            if (now != null)
            {
                //TExpressionTree diffResult = new TExpressionTree();

                switch (ty)
                {
                    case enumNodeType.NODE_VARIABLE:
                        if (now.varname == @var)
                        {
                            result.eType = enumNodeType.NODE_NUMBER;
                            result.value = 1;
                        }
                        else
                        {
                            result.eType = enumNodeType.NODE_NUMBER;
                            result.value = 0;
                        }
                        return result;
                    case enumNodeType.NODE_NUMBER:
                        result.value = 0;
                        return result;
                    case enumNodeType.NODE_OPERATOR: //当前为运算符节点
                        switch (now.eOperator)
                        {
                            case enumMathOperator.MATH_POSITIVE:
                            case enumMathOperator.MATH_NEGATIVE:
                                if (now.left != null)
                                {
                                    //左求导
                                    TNode lefttemp = Diff(now.left, @var);
                                    result.left = lefttemp;
                                }

                                return result;
                            case enumMathOperator.MATH_ADD:
                            case enumMathOperator.MATH_SUBSTRACT:
                                if (now.left != null)
                                {
                                    //Diff(ref now.left, @var);
                                    TNode lefttemp = Diff(now.left, @var);
                                    result.left = lefttemp;
                                }
                                if (now.right != null)
                                {
                                    //Diff(ref now.right, @var);
                                    TNode righttemp = Diff(now.right, @var);
                                    result.right = righttemp;
                                }
                                return result;
                            case enumMathOperator.MATH_MULTIPLY:
                                if (now.left.eType == enumNodeType.NODE_NUMBER || now.right.eType == enumNodeType.NODE_NUMBER) //两个操作数中有一个是数字
                                {
                                    if (now.left.eType == enumNodeType.NODE_NUMBER)
                                    {
                                        TNode temp = Diff(now.right, @var);
                                        result.right = temp;
                                        //Diff(ref now.right, @var);
                                    }
                                    else
                                    {
                                        //Diff(ref now.left, @var);
                                        TNode temp = Diff(now.left, @var);
                                        result.left = temp;
                                    }
                                }
                                else
                                {
                                    TNode plus;
                                    plus = new TNode();
                                    plus.eType = enumNodeType.NODE_OPERATOR;
                                    plus.eOperator = enumMathOperator.MATH_ADD;

                                    TNode plusleft = new TNode();
                                    plusleft.eType = enumNodeType.NODE_OPERATOR;
                                    plusleft.eOperator = enumMathOperator.MATH_MULTIPLY;
                                    TNode plusright = new TNode();
                                    plusright.eType = enumNodeType.NODE_OPERATOR;
                                    plusright.eOperator = enumMathOperator.MATH_MULTIPLY;

                                    TNode leftnow = now.left;
                                    TNode rightnow = now.right;
                                    TNode u = leftnow;
                                    TNode v = rightnow;
                                    TNode leftdiff = Diff(now.left, @var);
                                    TNode rightdiff = Diff(now.right, @var);
                                    //设置左节点
                                    leftdiff.parent = plusleft;
                                    v.parent = plusleft;
                                    plusleft.left = leftdiff;
                                    plusleft.right = v;
                                    //设置右节点
                                    rightdiff.parent = plusright;
                                    u.parent = plusright;
                                    plusright.left = rightdiff;
                                    plusright.right = u;
                                    //设置加左右点
                                    plusleft.parent = plus;
                                    plus.left = plusleft;
                                    plusright.parent = plus;
                                    plus.right = plusright;
                                    //设置父节点
                                    plus.parent = now.parent;
                                    result = plus;
                                }
                                return result;
                            case enumMathOperator.MATH_DIVIDE:
                                if (now.right.eType == enumNodeType.NODE_NUMBER) // f(x)/number = f'(x)/number
                                {
                                    //Diff(ref now.left, @var);
                                    //Diff(ref now.left, @var);
                                    TNode temp = Diff(now.left, @var);
                                    result.left = temp;
                                }
                                else
                                {
                                    TNode divide = now;
                                    TNode u1 = now.left;
                                    TNode v1 = now.right;
                                    TNode u1diff = Diff(now.left, var);
                                    TNode v1diff = Diff(now.right, var);

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

                                    //连接乘号1下面2个节点：u1, v1
                                    multiply1.left = u1diff;
                                    u1diff.parent = multiply1;
                                    multiply1.right = v1;
                                    v1.parent = multiply1;


                                    //连接乘号2下面的u2, v2
                                    multiply2.left = u1;
                                    u1.parent = multiply2;
                                    multiply2.right = v1diff;
                                    v1diff.parent = multiply2;

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
                                    //连接减号下面2个节点
                                    substract.left = multiply1;
                                    multiply1.parent = substract;
                                    substract.right = multiply2;
                                    multiply2.parent = substract;

                                    //连接除号下面2个节点：-, ^
                                    divide.left = substract;
                                    substract.parent = divide;
                                    divide.right = power;
                                    power.parent = divide;
                                    divide.parent = now.parent;
                                    result = divide;
                                }
                                return result;
                            case enumMathOperator.MATH_POWER:
                                {
                                    bool LChildIsNumber = now.left.eType == enumNodeType.NODE_NUMBER;
                                    bool RChildIsNumber = now.right.eType == enumNodeType.NODE_NUMBER;
                                    if (LChildIsNumber && RChildIsNumber)
                                    {
                                        result.left = null;
                                        result.right = null;
                                        result.eType = enumNodeType.NODE_NUMBER;
                                        result.eOperator = enumMathOperator.MATH_NULL;
                                        result.value = 0.0;
                                        return result;
                                    }
                                    if (RChildIsNumber)
                                    {
                                        if (now.right.value == 0)
                                        {
                                            TNode temp = NewNode(enumNodeType.NODE_NUMBER);
                                            temp.value = 0;
                                            return temp;
                                        }
                                        else if (now.right.value == 1)
                                        {
                                            return Diff(now.left, var);
                                        }

                                    }
                                    if (RChildIsNumber)
                                    {
                                        TNode multiply1 = NewNode(enumNodeType.NODE_OPERATOR, enumMathOperator.MATH_MULTIPLY);
                                        TNode multiply2 = NewNode(enumNodeType.NODE_OPERATOR, enumMathOperator.MATH_MULTIPLY);
                                        TNode power = now;
                                        TNode u = now.left;
                                        TNode v = now.right;
                                        TNode u2 = CopyNodeTree(u);
                                        TNode v2 = CopyNodeTree(v);
                                        v.value -= 1.0;
                                        power.right = v;//为lvwan添加20220619                                                        

                                        TNode u2diff = Diff(u2, @var);
                                        u2diff.parent = multiply2;
                                        multiply2.right = u2diff;
                                        v2.parent = multiply2;
                                        multiply2.left = v2;
                                        multiply2.parent = multiply1;
                                        multiply1.right = multiply2;
                                        multiply1.left = power;
                                        power.parent = multiply1;
                                        multiply1.parent = now.parent;
                                        result = multiply1;
                                        return result;
                                    }
                                    //y=f^g,  y'=f^g*(g*f'/f+g'*lnf)=u^v*①(v*③udiff /u+vdiff *②ln u)
                                    else
                                    {

                                        TNode multiply1 = NewNode(enumNodeType.NODE_OPERATOR, enumMathOperator.MATH_MULTIPLY);
                                        TNode multiply2 = NewNode(enumNodeType.NODE_OPERATOR, enumMathOperator.MATH_MULTIPLY);

                                        TNode plus = NewNode(enumNodeType.NODE_OPERATOR, enumMathOperator.MATH_ADD);

                                        TNode multiply3 = NewNode(enumNodeType.NODE_OPERATOR, enumMathOperator.MATH_MULTIPLY);
                                        TNode divide = NewNode(enumNodeType.NODE_OPERATOR, enumMathOperator.MATH_DIVIDE);
                                        TNode power = now;
                                        TNode u = now.left;
                                        TNode v = now.right;
                                        TNode udiff = Diff(now.left, @var);
                                        TNode vdiff = Diff(now.right, @var);
                                        TNode u2 = CopyNodeTree(u);
                                        TNode u3 = CopyNodeTree(u);
                                        TNode v2 = CopyNodeTree(v);
                                        //ln函数
                                        TNode ln = NewNode(enumNodeType.NODE_FUNCTION, enumMathOperator.MATH_LN);
                                        ln.left = u2;
                                        u2.parent = ln;
                                        multiply2.left = vdiff;
                                        vdiff.parent = multiply2;
                                        multiply2.right = ln;
                                        ln.parent = multiply2;

                                        //乘除函数
                                        multiply3.left = v2;
                                        v2.parent = multiply3;
                                        multiply3.right = udiff;
                                        udiff.parent = multiply3;

                                        divide.left = multiply3;
                                        multiply3.parent = divide;
                                        divide.right = u3;
                                        u3.parent = divide;

                                        //加函数
                                        plus.left = divide;
                                        divide.parent = plus;
                                        plus.right = multiply2;
                                        multiply2.parent = plus;

                                        //最后乘函数
                                        multiply1.left = power;
                                        power.parent = multiply1;
                                        multiply1.right = plus;
                                        plus.parent = multiply1;

                                        //
                                        multiply1.parent = now.parent;
                                        result = multiply1;
                                        return result;
                                    }

                                }
                        }
                        break;
                    case enumNodeType.NODE_FUNCTION:
                        {
                            //不考虑定义域
                            //函数内为数字则导为0                            
                            bool LChildIsNumber = now.left.eType == enumNodeType.NODE_NUMBER;
                            bool LChildIsOtherVar = now.left.eType == enumNodeType.NODE_VARIABLE && now.left.varname!=@var;
                            if (LChildIsNumber || LChildIsOtherVar)
                            {
                                result.eType = enumNodeType.NODE_NUMBER;
                                result.value = 0;
                                result.left = null;
                                result.right = null;
                                return result;
                            }
                            TNode function = CopyNodeTree(now);
                            enumMathOperator op = function.eOperator;
                            switch (op)
                            {
                                case enumMathOperator.MATH_SQRT:
                                    {
                                        //转化为幂求导
                                        TNode us = function.left;
                                        TNode power = NewNode(enumNodeType.NODE_OPERATOR, enumMathOperator.MATH_POWER);
                                        TNode num1half = NewNode(enumNodeType.NODE_NUMBER);
                                        num1half.value = 0.5;

                                        power.left = us;
                                        us.parent = power;

                                        power.right = num1half;
                                        num1half.parent = power;

                                        //if (function == head)
                                        //{
                                        //    head = power;
                                        //}
                                        //else
                                        //{
                                        //    if (function.parent.left == function)
                                        //    {
                                        //        function.parent.left = power;
                                        //    }
                                        //    if (function.parent.right == function)
                                        //    {
                                        //        function.parent.right = power;
                                        //    }
                                        //    power.parent = function.parent;
                                        //}

                                        result = Diff(power, @var);
                                        return result;
                                    }
                                case enumMathOperator.MATH_LN:
                                case enumMathOperator.MATH_LOG10:
                                    {
                                        
                                        TNode divideln = NewNode(enumNodeType.NODE_OPERATOR, enumMathOperator.MATH_DIVIDE);
                                        TNode num1ln = NewNode(enumNodeType.NODE_NUMBER);
                                        num1ln.value = 1.0;

                                        divideln.left = num1ln;
                                        num1ln.parent = divideln;

                                        TNode uln = function.left;

                                        if (function.eOperator == enumMathOperator.MATH_LN) //ln(x)=1/x
                                        {
                                            uln.parent = divideln;
                                            divideln.right = uln;                                            
                                        }
                                        else
                                        {
                                            //log10(x)=1/(x*ln(10))
                                            TNode ln = NewNode(enumNodeType.NODE_FUNCTION, enumMathOperator.MATH_LN);
                                            TNode num10 = NewNode(enumNodeType.NODE_NUMBER);
                                            num10.value = 10.0;

                                            num10.parent = ln;
                                            ln.left = num10;                                            

                                            TNode multiply2ln = NewNode(enumNodeType.NODE_OPERATOR, enumMathOperator.MATH_MULTIPLY);

                                            TNode u1ln = CopyNodeTree(uln);
                                            u1ln.parent = multiply2ln;
                                            multiply2ln.left = u1ln;

                                            ln.parent = multiply2ln;
                                            multiply2ln.right = ln;

                                            multiply2ln.parent = divideln;
                                            divideln.right = multiply2ln;
                                            

                                        }
                                        divideln.parent = function.parent;
                                        

                                        TNode topln = divideln;
                                        TNode u2ln = null;
                                        if (function.left.eType != enumNodeType.NODE_VARIABLE)
                                        {
                                            TNode multiply1ln = NewNode(enumNodeType.NODE_OPERATOR, enumMathOperator.MATH_MULTIPLY);
                                            u2ln = Diff(function.left,@var);

                                            u2ln.parent = multiply1ln;
                                            multiply1ln.right = u2ln;

                                            divideln.parent = multiply1ln;
                                            multiply1ln.left = divideln;                                            

                                            topln = multiply1ln;
                                        }
                                        result = topln;
                                        //if (function == head)
                                        //{
                                        //    head = top;
                                        //}
                                        //else
                                        //{
                                        //    if (function.parent.left == function)
                                        //    {
                                        //        function.parent.left = top;
                                        //        top.parent = function.parent;
                                        //    }
                                        //    if (function.parent.right == function)
                                        //    {
                                        //        function.parent.right = top;
                                        //        top.parent = function.parent;
                                        //    }
                                        //}
                                        //function = null;

                                        //if (function.left.eType != enumNodeType.NODE_VARIABLE)
                                        //{
                                        //    result = Diff(u2, @var);
                                        //}

                                    }
                                    return result;
                                case enumMathOperator.MATH_EXP:
                                    {
                                        if (function.left.eType == enumNodeType.NODE_VARIABLE && function.left.varname == @var) //e^x=e^x
                                        {
                                            return result;
                                        }
                                        TNode multiply = NewNode(enumNodeType.NODE_OPERATOR, enumMathOperator.MATH_MULTIPLY);
                                        TNode u2exp = CopyNodeTree(function.left);
                                        TNode u2diff = Diff(u2exp, @var);
                                        //if (function == head)
                                        //{
                                        //    head = multiply;
                                        //}
                                        //else
                                        //{
                                        //    if (function.parent.left == function)
                                        //    {
                                        //        function.parent.left = multiply;
                                        //        multiply.parent = function.parent;
                                        //    }
                                        //    if (function.parent.right == function)
                                        //    {
                                        //        function.parent.right = multiply;
                                        //        multiply.parent = function.parent;
                                        //    }
                                        //}
                                        function.parent = multiply;
                                        multiply.left = function;
                                        
                                        u2diff.parent = multiply;
                                        multiply.right = u2diff;
                                        

                                        multiply.parent = now.parent;
                                        result = multiply;
                                    }
                                    return result;
                                case enumMathOperator.MATH_COS:
                                    {
                                        TNode negativeco = new TNode();
                                        negativeco.eType = enumNodeType.NODE_OPERATOR;
                                        negativeco.eOperator = enumMathOperator.MATH_NEGATIVE;

                                        //连接上一级和负号
                                        //if (function != head)
                                        //{
                                        //    if (function.parent.left == function)
                                        //    {
                                        //        function.parent.left = negative;
                                        //        negative.parent = function.parent;
                                        //    }
                                        //    if (function.parent.right == function)
                                        //    {
                                        //        function.parent.right = negative;
                                        //        negative.parent = function.parent;
                                        //    }
                                        //}
                                        //else
                                        //{
                                        //    head = negative;
                                        //    negative.parent = null;
                                        //}
                                        //判断是否cos(x)
                                        if (function.left.eType == enumNodeType.NODE_VARIABLE && function.left.varname == @var)
                                        {
                                            function.eOperator = enumMathOperator.MATH_SIN;
                                            negativeco.left = function;
                                            function.parent = negativeco;

                                            negativeco.parent = now.parent;
                                            result = negativeco;
                                            return result;
                                        }
                                        TNode multiplyco = new TNode();
                                        multiplyco.eType = enumNodeType.NODE_OPERATOR;
                                        multiplyco.eOperator = enumMathOperator.MATH_MULTIPLY;


                                        //复制u2并连接乘号
                                        TNode u2co = CopyNodeTree(function.left);
                                        TNode u2diff = Diff(u2co, @var);
                                        multiplyco.right = u2diff;
                                        u2diff.parent = multiplyco;
                                        //变更function
                                        function.eOperator = enumMathOperator.MATH_SIN;
                                        //连接乘号和function
                                        multiplyco.left = function;
                                        function.parent = multiplyco;
                                        //连接负号和乘号
                                        negativeco.left = multiplyco;
                                        multiplyco.parent = negativeco;

                                        negativeco.parent = now.parent;
                                        result = negativeco;
                                    }
                                    return result;
                                case enumMathOperator.MATH_SIN:
                                    {
                                        TNode multiplysi = new TNode();
                                        multiplysi.eType = enumNodeType.NODE_OPERATOR;
                                        multiplysi.eOperator = enumMathOperator.MATH_MULTIPLY;

                                        //连接上一级和乘号
                                        //if (function != head)
                                        //{
                                        //    if (function.parent.left == function)
                                        //    {
                                        //        function.parent.left = multiply;
                                        //        multiply.parent = function.parent;
                                        //    }
                                        //    if (function.parent.right == function)
                                        //    {
                                        //        function.parent.right = multiply;
                                        //        multiply.parent = function.parent;
                                        //    }
                                        //}
                                        //else
                                        //{
                                        //    head = multiply;
                                        //    multiply.parent = null;
                                        //}
                                        //判断是否cos(x)
                                        if (function.left.eType == enumNodeType.NODE_VARIABLE && function.left.varname == @var)
                                        {
                                            function.eOperator = enumMathOperator.MATH_COS;
                                            result = function;
                                            return result;
                                        }
                                        //复制u2并连接乘号
                                        TNode u2si = CopyNodeTree(function.left);
                                        TNode u2diff = Diff(u2si, @var);
                                        multiplysi.right = u2diff;
                                        u2diff.parent = multiplysi;

                                        //变更function
                                        function.eOperator = enumMathOperator.MATH_COS;
                                        //连接乘号和function
                                        multiplysi.left = function;
                                        function.parent = multiplysi;

                                        multiplysi.parent = now.parent;
                                        result = multiplysi;
                                    }
                                    return result;
                                case enumMathOperator.MATH_TAN:
                                    {
                                        TNode cofunc = CopyNodeTree(function);
                                        TNode dividetan = new TNode();
                                        dividetan.eType = enumNodeType.NODE_OPERATOR;
                                        dividetan.eOperator = enumMathOperator.MATH_DIVIDE;
                                        dividetan.parent = cofunc.parent;
                                        TNode le = new TNode(1.0);
                                        le.parent = dividetan;
                                        dividetan.left = le;

                                        TNode multiplytan = new TNode();
                                        multiplytan.eType = enumNodeType.NODE_OPERATOR;
                                        multiplytan.eOperator = enumMathOperator.MATH_MULTIPLY;

                                        cofunc.eOperator = enumMathOperator.MATH_COS;
                                        cofunc.parent = multiplytan;
                                        TNode le1 = CopyNodeTree(cofunc);
                                        TNode ri1 = CopyNodeTree(cofunc);
                                        multiplytan.left = le1;
                                        multiplytan.right = ri1;

                                        dividetan.right = multiplytan;
                                        multiplytan.parent = dividetan;

                                        if (function.left.eType == enumNodeType.NODE_VARIABLE && function.left.varname == @var)
                                        {                                       

                                            result = dividetan;
                                            return result;
                                        }
                                        //复制u2并连接乘号
                                        TNode mulitplyre = new TNode();
                                        mulitplyre.eType = enumNodeType.NODE_OPERATOR;
                                        mulitplyre.eOperator = enumMathOperator.MATH_MULTIPLY;
                                        mulitplyre.parent = function.parent;

                                        mulitplyre.left = dividetan;
                                        dividetan.parent= mulitplyre;

                                        TNode u2tan = CopyNodeTree(function.left);
                                        TNode u2diff = Diff(u2tan, @var);
                                        mulitplyre.right = u2diff;
                                        u2diff.parent = mulitplyre;

                                        result = mulitplyre;
                                    }
                                    return result;
                                case enumMathOperator.MATH_ARCSIN:
                                    {
                                        TNode divide = NewNode(enumNodeType.NODE_OPERATOR, enumMathOperator.MATH_DIVIDE);
                                        TNode num1 = NewNode(enumNodeType.NODE_NUMBER);
                                        num1.value = 1.0;

                                        divide.left = num1;
                                        num1.parent = divide;

                                        TNode u = CopyNodeTree(function.left);


                                        //arcsin(x)=1/sqrt(1-x^2)
                                        TNode substract = NewNode(enumNodeType.NODE_OPERATOR, enumMathOperator.MATH_SUBSTRACT);
                                        TNode num1a = NewNode(enumNodeType.NODE_NUMBER);
                                        num1a.value = 1.0;

                                        num1a.parent = substract;
                                        substract.left = num1a;

                                        TNode multiply2 = NewNode(enumNodeType.NODE_OPERATOR, enumMathOperator.MATH_MULTIPLY);

                                        TNode u1 = CopyNodeTree(u);
                                        u1.parent = multiply2;
                                        multiply2.left = u1;
                                        TNode u2 = CopyNodeTree(u);
                                        u2.parent = multiply2;
                                        multiply2.right = u2;

                                        multiply2.parent = substract;
                                        substract.right = multiply2;


                                        TNode sqr = NewNode(enumNodeType.NODE_FUNCTION, enumMathOperator.MATH_SQRT);
                                        substract.parent = sqr;
                                        sqr.left = substract;

                                        sqr.parent = divide;
                                        divide.right = sqr;

                                        TNode top = divide;
                                        TNode u3 = null;
                                        if (function.left.eType != enumNodeType.NODE_VARIABLE)
                                        {
                                            TNode multiply1 = NewNode(enumNodeType.NODE_OPERATOR, enumMathOperator.MATH_MULTIPLY);
                                            u3 = Diff(function.left, @var);

                                            u3.parent = multiply1;
                                            multiply1.right = u3;

                                            divide.parent = multiply1;
                                            multiply1.left = divide;

                                            top = multiply1;
                                        }
                                        result = top;
                                    }
                                    return result;
                                case enumMathOperator.MATH_ARCCOS:
                                    {
                                        TNode divide = NewNode(enumNodeType.NODE_OPERATOR, enumMathOperator.MATH_DIVIDE);
                                        TNode num1 = NewNode(enumNodeType.NODE_NUMBER);
                                        num1.value = -1.0;

                                        divide.left = num1;
                                        num1.parent = divide;

                                        TNode u = CopyNodeTree(function.left);


                                        //arcsin(x)=-1/sqrt(1-x^2)
                                        TNode substract = NewNode(enumNodeType.NODE_OPERATOR, enumMathOperator.MATH_SUBSTRACT);
                                        TNode num1a = NewNode(enumNodeType.NODE_NUMBER);
                                        num1a.value = 1.0;

                                        num1a.parent = substract;
                                        substract.left = num1a;

                                        TNode multiply2 = NewNode(enumNodeType.NODE_OPERATOR, enumMathOperator.MATH_MULTIPLY);

                                        TNode u1 = CopyNodeTree(u);
                                        u1.parent = multiply2;
                                        multiply2.left = u1;
                                        TNode u2 = CopyNodeTree(u);
                                        u2.parent = multiply2;
                                        multiply2.right = u2;

                                        multiply2.parent = substract;
                                        substract.right = multiply2;


                                        TNode sqr = NewNode(enumNodeType.NODE_FUNCTION, enumMathOperator.MATH_SQRT);
                                        substract.parent = sqr;
                                        sqr.left = substract;

                                        sqr.parent = divide;
                                        divide.right = sqr;

                                        TNode top = divide;
                                        TNode u3 = null;
                                        if (function.left.eType != enumNodeType.NODE_VARIABLE)
                                        {
                                            TNode multiply1 = NewNode(enumNodeType.NODE_OPERATOR, enumMathOperator.MATH_MULTIPLY);
                                            u3 = Diff(function.left, @var);

                                            u3.parent = multiply1;
                                            multiply1.right = u3;

                                            divide.parent = multiply1;
                                            multiply1.left = divide;

                                            top = multiply1;
                                        }
                                        result = top;
                                    }
                                    return result;
                                case enumMathOperator.MATH_ARCTAN:
                                    {
                                        TNode divideat = NewNode(enumNodeType.NODE_OPERATOR, enumMathOperator.MATH_DIVIDE);
                                        TNode num1 = NewNode(enumNodeType.NODE_NUMBER);
                                        num1.value = 1.0;

                                        divideat.left = num1;
                                        num1.parent = divideat;

                                        TNode u = CopyNodeTree(function.left);


                                        //arcsin(x)=1/(1+x^2)
                                        TNode addat = NewNode(enumNodeType.NODE_OPERATOR, enumMathOperator.MATH_ADD);
                                        TNode num1a = NewNode(enumNodeType.NODE_NUMBER);
                                        num1a.value = 1.0;

                                        num1a.parent = addat;
                                        addat.left = num1a;

                                        TNode multiply2 = NewNode(enumNodeType.NODE_OPERATOR, enumMathOperator.MATH_MULTIPLY);

                                        TNode u1atan = CopyNodeTree(u);
                                        u1atan.parent = multiply2;
                                        multiply2.left = u1atan;
                                        TNode u2atan = CopyNodeTree(u);
                                        u2atan.parent = multiply2;
                                        multiply2.right = u2atan;

                                        addat.right = multiply2;
                                        multiply2.parent = addat;
                                        
                                                                                
                                        addat.parent = divideat;
                                        divideat.right = addat;

                                        divideat.parent = function.parent;
                                        TNode top = divideat;
                                        TNode u3atan = null;
                                        if (function.left.eType != enumNodeType.NODE_VARIABLE)
                                        {
                                            TNode multiply1 = NewNode(enumNodeType.NODE_OPERATOR, enumMathOperator.MATH_MULTIPLY);
                                            u3atan = Diff(function.left, @var);

                                            u3atan.parent = multiply1;
                                            multiply1.right = u3atan;

                                            divideat.parent = multiply1;
                                            multiply1.left = divideat;

                                            top = multiply1;
                                        }
                                        result = top;
                                    }
                                    return result;
                                default:
                                    throw new TError(enumError.ERROR_WRONG_MATH_OPERATOR, "未完成的运算符" + Function2Str(now.eOperator));
                            }
                        }
                        //break;
                }
            }            
            return result;
        }
        private TNode DiffLv(TNode now, string @var)
        {
            TNode result = CopyNodeTree(now);
            
            if (now != null)
            {
                //TExpressionTree diffResult = new TExpressionTree();
                enumNodeType ty = now.eType;
                switch (ty)
                {
                    case enumNodeType.NODE_VARIABLE:
                        if (now.varname == @var)
                        {
                            result.eType = enumNodeType.NODE_NUMBER;
                            result.eOperator = enumMathOperator.MATH_NULL;
                            result.value = 1;
                        }
                        else
                        {
                            result.eType = enumNodeType.NODE_NUMBER;
                            result.eOperator = enumMathOperator.MATH_NULL;
                            result.value = 0;
                        }
                        return result;
                    case enumNodeType.NODE_NUMBER:
                        result.value = 0;
                        return result;
                    case enumNodeType.NODE_OPERATOR:
                        TNode leftdiffm = DiffLv(now.left, @var);
                        TNode rightdiffm = DiffLv(now.right, @var);
                        TNode leftold = now.left;
                        TNode rightold = now.right;//当前为运算符节点
                        switch (now.eOperator)
                        {
                            case enumMathOperator.MATH_POSITIVE:
                            case enumMathOperator.MATH_NEGATIVE:
                                if (now.left != null)
                                {
                                    //左求导

                                    leftdiffm.parent = result;
                                    result.left = leftdiffm;
                                }

                                return result;
                            case enumMathOperator.MATH_ADD:
                            case enumMathOperator.MATH_SUBSTRACT:
                                result.left = leftdiffm;
                                result.right = rightdiffm;
                                leftdiffm.parent = result;
                                rightdiffm.parent = result;
                                return result;
                            case enumMathOperator.MATH_MULTIPLY:                                
                                result.eOperator = enumMathOperator.MATH_ADD;
                                TNode lefta = leftdiffm * rightold;
                                TNode righta = rightdiffm * leftold;
                                result.left=lefta;
                                result.right=righta;
                                lefta.parent = result;
                                righta.parent = result;
                                return result;
                            case enumMathOperator.MATH_DIVIDE:
                                result.eOperator = enumMathOperator.MATH_SUBSTRACT;
                                TNode leftb = leftdiffm / rightold;
                                TNode rightb = rightdiffm * leftold/(rightold^2);
                                result.left = leftb;
                                result.right = rightb;
                                leftb.parent = result;
                                rightb.parent = result;
                                return result;
                            case enumMathOperator.MATH_POWER:
                                {
                                    if(!leftold.IsContainsVar(@var) && !rightold.IsContainsVar(@var))
                                    {
                                        result.eType = enumNodeType.NODE_NUMBER;
                                        result.eOperator = enumMathOperator.MATH_NULL;
                                        result.left = null;
                                        result.right = null;
                                        result.value = 0;
                                    }else if (leftold.IsContainsVar(var) && !rightold.IsContainsVar(@var))
                                    {
                                        result = leftdiffm * rightold * (leftold ^ (rightold - 1));
                                    }
                                    else
                                    {
                                        result.eOperator = enumMathOperator.MATH_MULTIPLY;
                                        TNode leftc = leftold ^ rightold;
                                        TNode rightc = (leftdiffm * rightold / leftold + rightdiffm * TNode.Ln(leftold));
                                        result.left = leftc;
                                        result.right = rightc;
                                        leftc.parent = result;
                                        rightc.parent = result;
                                    }
                                    
                                    return result;

                                    //y=f^g,  y'=f^g*(g*f'/f+g'*lnf)=u^v*①(v*③udiff /u+vdiff *②ln u)


                                }
                        }
                        break;
                    case enumNodeType.NODE_FUNCTION:
                        {
                            TNode leftdifff = DiffLv(now.left, @var);
                            TNode leftoldf = now.left;
                            //不考虑定义域
                            //函数内为数字则导为0                            
                            bool LChildIsNumber = now.left.eType == enumNodeType.NODE_NUMBER;
                            bool LChildIsOtherVar = now.left.eType == enumNodeType.NODE_VARIABLE && now.left.varname != @var;
                            if (LChildIsNumber || LChildIsOtherVar)
                            {
                                result.eType = enumNodeType.NODE_NUMBER;
                                result.eOperator = enumMathOperator.MATH_NULL;
                                result.value = 0;
                                result.left = null;
                                result.right = null;
                                return result;
                            }
                            TNode function = CopyNodeTree(now);
                            enumMathOperator op = function.eOperator;
                            switch (op)
                            {
                                case enumMathOperator.MATH_SQRT:
                                    {
                                        //转化为幂求导                                       
                                        result = (leftdifff /TNode.Sqrt(leftoldf))/2;
                                        result.parent = now.parent;
                                        return result;
                                    }
                                case enumMathOperator.MATH_LN:
                                case enumMathOperator.MATH_LOG10:
                                    {

                                        if (function.eOperator == enumMathOperator.MATH_LN) //ln(x)=1/x
                                        {
                                            result = leftdifff / leftoldf;
                                            result.parent = now.parent;
                                        }
                                        else
                                        {
                                            //log10(x)=1/(x*ln(10))
                                            result = leftdifff / (leftoldf* Math.Log(10));
                                            result.parent = now.parent;
                                        }

                                    }
                                    return result;
                                case enumMathOperator.MATH_EXP:
                                    {
                                        result=leftdifff* result;
                                        result.parent = now.parent;
                                    }
                                    return result;
                                case enumMathOperator.MATH_COS:
                                    {
                                        result = -TNode.Sin(leftoldf) * leftdifff;
                                        result.parent = now.parent;
                                    }
                                    return result;
                                case enumMathOperator.MATH_SIN:
                                    {
                                        result = TNode.Cos(leftoldf) * leftdifff;
                                        result.parent = now.parent;
                                    }
                                    return result;
                                case enumMathOperator.MATH_TAN:
                                    {
                                        TNode gouzao=TNode.Sin(leftoldf)/TNode.Cos(leftoldf);
                                        result = DiffLv(gouzao, @var);
                                        result.parent = now.parent;
                                    }
                                    return result;
                                case enumMathOperator.MATH_ARCSIN:
                                    {
                                        //arcsin(x)=1/sqrt(1-x^2)
                                        result = leftdifff / TNode.Sqrt(1 - (leftoldf ^ 2));
                                        result.parent = now.parent;
                                        
                                       
                                    }
                                    return result;
                                case enumMathOperator.MATH_ARCCOS:
                                    {
                                        //arcsin(x)=-1/sqrt(1-x^2)
                                        result = -leftdifff / TNode.Sqrt(1 - (leftoldf ^ 2));
                                        result.parent = now.parent;
                                    }
                                    return result;
                                case enumMathOperator.MATH_ARCTAN:
                                    {

                                        //arcsin(x)=1/(1+x^2)
                                        result = leftdifff / TNode.Sqrt((leftoldf ^ 2)+1);
                                        result.parent = now.parent;
                                    }
                                    return result;
                                default:
                                    throw new TError(enumError.ERROR_WRONG_MATH_OPERATOR, "未完成的运算符" + Function2Str(now.eOperator));
                            }
                        }
                        //break;
                }
            }
            if (result == null)
            {
                double aa = 1;
            }
            return result;
        }
        //对变量求导n次
        public TExpressionTree Diff(string @var, int n)
        {
            TExpressionTree diffExpressionTree = Copythis();
            if (pVariableTable.FindVariableTable(@var) == pVariableTable.VariableTable.Count)
            {
                throw new TError(enumError.ERROR_UNDEFINED_VARIABLE, @var);
            }

            for (int i = 0; i < n; i++)
            {
                diffExpressionTree.head = DiffLv(diffExpressionTree.head, @var);
            }
            diffExpressionTree.str_expression = diffExpressionTree.OutputStr();
            diffExpressionTree.StringExpressionSimplify();
            return diffExpressionTree;
        }
        
        //粗切分：利用operator切分
        public struct TStrPiece
        {
            public bool bBaseOperator;
            public string s;
            public TStrPiece(bool bbaseoperator, string ss) { bBaseOperator = bbaseoperator; s = ss; }
            //TStrPiece(bool bIn, String sIn) :bBaseOperator(bIn), s(sIn) { }
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



        private TNode CalcNode(TNode Operator, TNode Node1, TNode Node2 = null)
        {
            TNode result = CopyNodeTree(Operator);
            double value1 = Node1.value;
            double value2 = Node2 != null ? Node2.value : 0.0;
            result.eType = enumNodeType.NODE_NUMBER;
            switch (Operator.eOperator)
            {
                case enumMathOperator.MATH_SQRT:
                    if (value1 < 0.0)
                    {
                        //恢复修改并抛出异常
                        result.eType = enumNodeType.NODE_FUNCTION;
                        throw new TError(enumError.ERROR_I, "sqrt(" + value1.ToString() + ")");
                        //return;
                    }
                    result.value = Math.Sqrt(value1);
                    break;
                case enumMathOperator.MATH_SIN:
                    result.value = Math.Sin(value1);
                    break;
                case enumMathOperator.MATH_COS:
                    result.value = Math.Cos(value1);
                    break;
                case enumMathOperator.MATH_TAN:
                    {
                        //x!=k*pi+pi/2 -> 2*x/pi != 2*k+1(odd)
                        double value = value1 * 2.0 / Math.PI;
                        if (Math.Abs(value - (int)value) < MIN_DOUBLE && (int)value % 2 != 1)
                        {
                            //恢复修改并抛出异常
                            result.eType = enumNodeType.NODE_FUNCTION;
                            throw new TError(enumError.ERROR_OUTOF_DOMAIN, "tan(" + value.ToString() + ")");
                            //return;
                        }
                        result.value = Math.Tan(value1);
                        break;
                    }
                case enumMathOperator.MATH_ARCSIN:
                    if (value1 < -1.0 || value1 > 1.0)
                    {
                        //恢复修改并抛出异常
                        result.eType = enumNodeType.NODE_FUNCTION;
                        throw new TError(enumError.ERROR_OUTOF_DOMAIN, "arcsin(" + value1.ToString() + ")");
                        //return;
                    }
                    result.value = Math.Asin(value1);
                    break;
                case enumMathOperator.MATH_ARCCOS:
                    if (value1 < -1.0 || value1 > 1.0)
                    {
                        //恢复修改并抛出异常
                        result.eType = enumNodeType.NODE_FUNCTION;
                        throw new TError(enumError.ERROR_OUTOF_DOMAIN, "arccos(" + value1.ToString() + ")");
                    }
                    result.value = Math.Acos(value1);
                    break;
                case enumMathOperator.MATH_ARCTAN:
                    result.value = Math.Atan(value1);
                    break;
                case enumMathOperator.MATH_LN:
                    if (value1 < MIN_DOUBLE)
                    {
                        //恢复修改并抛出异常
                        result.eType = enumNodeType.NODE_FUNCTION;
                        throw new TError(enumError.ERROR_OUTOF_DOMAIN, "ln(" + value1.ToString() + ")");
                    }
                    result.value = Math.Log(value1);
                    break;
                case enumMathOperator.MATH_LOG10:
                    if (value1 < MIN_DOUBLE) //log(0)或log(负数)
                    {
                        //恢复修改并抛出异常
                        result.eType = enumNodeType.NODE_FUNCTION;
                        throw new TError(enumError.ERROR_OUTOF_DOMAIN, "log10(" + value1.ToString() + ")");
                    }
                    result.value = Math.Log10(value1);
                    break;
                case enumMathOperator.MATH_EXP:
                    result.value = Math.Exp(value1);
                    break;
                case enumMathOperator.MATH_POSITIVE:
                    break;
                case enumMathOperator.MATH_NEGATIVE:
                    result.value = -value1;
                    break;

                case enumMathOperator.MATH_MOD: //%
                    if ((int)value2 == 0)
                    {
                        throw new TError(enumError.ERROR_DIVIDE_ZERO, value2.ToString());
                    }
                    result.value = (int)value1 % (int)value2;
                    break;
                case enumMathOperator.MATH_AND: //&
                    result.value = (int)value1 & (int)value2;
                    break;
                case enumMathOperator.MATH_OR: //|
                    result.value = (int)value1 | (int)value2;
                    break;

                case enumMathOperator.MATH_POWER: //^
                                                  //0^0
                    if (Math.Abs(value1) < MIN_DOUBLE && Math.Abs(value2) < MIN_DOUBLE)
                    {
                        result.eType = enumNodeType.NODE_OPERATOR;
                        throw new TError(enumError.ERROR_ZERO_POWEROF_ZERO, "");
                    }

                    //(-1)^0.5=i
                    if (value1 < 0 && IsIntAndEven(1L / value2))
                    {
                        result.eType = enumNodeType.NODE_OPERATOR;
                        throw new TError(enumError.ERROR_I, "pow(" + value1.ToString() + "," + value2.ToString() + ")");
                    }
                    result.value = Math.Pow(value1, value2);
                    break;

                case enumMathOperator.MATH_MULTIPLY:
                    result.value = value1 * value2;
                    break;
                case enumMathOperator.MATH_DIVIDE:
                    if (Math.Abs(value2) < MIN_DOUBLE)
                    {
                        result.eType = enumNodeType.NODE_OPERATOR;
                        throw new TError(enumError.ERROR_DIVIDE_ZERO, "");
                    }
                    result.value = value1 / value2;
                    break;

                case enumMathOperator.MATH_ADD:
                    result.value = value1 + value2;
                    break;
                case enumMathOperator.MATH_SUBSTRACT:
                    result.value = value1 - value2;
                    break;
            }
            result.eOperator = enumMathOperator.MATH_NULL;
            result.left = null;
            result.right = null;
            return result;
        }

        /// <summary>
        /// //用子替换父
        /// </summary>
        /// <param name="child">子</param>
        /// <param name="ignore">本节点</param>
        private void LinkParent(TNode child, TNode toRemove)
        {

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

        }

        private TNode NewNode(enumNodeType eType, enumMathOperator eOperator = enumMathOperator.MATH_NULL)
        {
            TNode newNode = new TNode();
            newNode.eType = eType;
            newNode.eOperator = eOperator;
            return newNode;
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

        //复制节点树，返回新节点树头节点
        private TNode CopyNodeTree(TNode oldNode)
        {
            if (oldNode == null)
            {
                return null;
            }
            TNode newNode = new TNode();
            newNode.eType = oldNode.eType;
            newNode.eOperator = oldNode.eOperator;
            newNode.value = oldNode.value;
            if (!string.IsNullOrEmpty(oldNode.varname)) { newNode.varname = oldNode.varname; }           


            if (oldNode.left != null)
            {
                newNode.left = CopyNodeTree(oldNode.left);

            }
            if (oldNode.right != null)
            {
                newNode.right = CopyNodeTree(oldNode.right);

            }
            if (oldNode.parent != null)
            {
                newNode.parent = oldNode.parent;

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

        private void GetVariablePos(string @var, List<TNode> VarsPos)
        {
            GetVariablePos(head, @var, VarsPos);
        }

        private void ReleaseVectorTNode(List<TNode> vec)
        {
            vec = null;
        }

        public void LinkVariableTable(TVariableTable p)
        {
            pVariableTable = p;
        }

        private TNode Solve(TNode now)
        {
            TNode write_pos = new TNode();
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

                        TNode tempright = CopyNodeTree(brother);
                        tempright.parent = write_pos;
                        write_pos.right = tempright;
                        //write_pos.right.parent = write_pos;

                        write_pos.left = new TNode();

                        //write_pos.left.parent = write_pos;
                        //Solve(parent, ref write_pos.left);
                        TNode temp = write_pos.left;
                        temp=Solve(parent);
                        write_pos.left = temp;
                        temp.parent = write_pos;
                        break;
                    case enumMathOperator.MATH_MULTIPLY:
                        write_pos.eType = enumNodeType.NODE_OPERATOR;
                        write_pos.eOperator = enumMathOperator.MATH_DIVIDE;

                        tempright = CopyNodeTree(brother);
                        tempright.parent = write_pos;
                        write_pos.right = tempright;

                        //write_pos.left = new TNode();

                        //write_pos.left.parent = write_pos;
                        //Solve(parent, ref write_pos.left);
                        TNode temp1 = write_pos.left;
                        temp1=Solve(parent);
                        temp1.parent=write_pos;
                        write_pos.left = temp1;
                        break;
                    case enumMathOperator.MATH_SUBSTRACT: //分左右
                        if (bVarIsLeft) //被减数
                        {
                            write_pos.eType = enumNodeType.NODE_OPERATOR;
                            write_pos.eOperator = enumMathOperator.MATH_ADD;

                            tempright = CopyNodeTree(brother);
                            tempright.parent = write_pos;
                            write_pos.right = tempright;

                            write_pos.left = new TNode();

                            //write_pos.left.parent = write_pos;
                            //Solve(parent, ref write_pos.left);
                            TNode temp2a = write_pos.left;
                            temp2a=Solve(parent);
                            temp2a.parent=write_pos;
                            write_pos.left = temp2a;
                        }
                        else
                        {
                            write_pos.eType = enumNodeType.NODE_OPERATOR;
                            write_pos.eOperator = enumMathOperator.MATH_SUBSTRACT;

                            TNode lefttemp = CopyNodeTree(brother);
                            lefttemp.parent = write_pos;
                            write_pos.left = lefttemp;

                            write_pos.right = new TNode();

                            //write_pos.right.parent = write_pos;
                            //Solve(parent, ref write_pos.right);
                            TNode temp2b = write_pos.right;
                            temp2b=Solve(parent);
                            temp2b.parent= write_pos;
                            write_pos.right = temp2b;
                        }
                        break;
                    case enumMathOperator.MATH_DIVIDE: //分左右
                        if (bVarIsLeft) //被除数
                        {
                            write_pos.eType = enumNodeType.NODE_OPERATOR;
                            write_pos.eOperator = enumMathOperator.MATH_MULTIPLY;

                            tempright = CopyNodeTree(brother);
                            tempright.parent = write_pos;
                            write_pos.right = tempright;

                            write_pos.left = new TNode();

                            //write_pos.left.parent = write_pos;
                            //Solve(parent, ref write_pos.left);
                            TNode temp2c = write_pos.left;
                            temp2c=Solve(parent);
                            temp2c.parent=write_pos;
                            write_pos.left = temp2c;

                        }
                        else
                        {
                            write_pos.eType = enumNodeType.NODE_OPERATOR;
                            write_pos.eOperator = enumMathOperator.MATH_DIVIDE;

                            TNode lefttemp = CopyNodeTree(brother);
                            lefttemp.parent = write_pos;
                            write_pos.left = lefttemp;

                            write_pos.right = new TNode();

                            //write_pos.right.parent = write_pos;
                            //Solve(parent, ref write_pos.right);
                            TNode temp2d = write_pos.right;
                            temp2d=Solve(parent);
                            temp2d.parent= write_pos;
                            write_pos.right = temp2d;
                        }
                        break;
                    case enumMathOperator.MATH_POSITIVE:
                        write_pos.eType = enumNodeType.NODE_OPERATOR;
                        write_pos.eOperator = enumMathOperator.MATH_POSITIVE;

                        write_pos.left = new TNode();

                        //write_pos.left.parent = write_pos;
                        //Solve(parent, ref write_pos.left);
                        TNode temp2 = write_pos.left;
                        temp2=Solve(parent);
                        temp2.parent=write_pos;
                        write_pos.left = temp2;
                        break;
                    case enumMathOperator.MATH_NEGATIVE:
                        write_pos.eType = enumNodeType.NODE_OPERATOR;
                        write_pos.eOperator = enumMathOperator.MATH_NEGATIVE;

                        write_pos.left = new TNode();

                        //write_pos.left.parent = write_pos;
                        //Solve(parent, ref write_pos.left);
                        TNode temp3 = write_pos.left;
                        temp3=Solve(parent);
                        temp3.parent= write_pos;
                        write_pos.left = temp3;
                        break;
                    case enumMathOperator.MATH_SIN:
                        write_pos.eType = enumNodeType.NODE_FUNCTION;
                        write_pos.eOperator = enumMathOperator.MATH_ARCSIN;

                        write_pos.left = new TNode();

                        //write_pos.left.parent = write_pos;
                        //Solve(parent, ref write_pos.left);
                        TNode temp2e = write_pos.left;
                        temp2e=Solve(parent);
                        temp2e.parent= write_pos;
                        write_pos.left = temp2e;
                        break;
                    case enumMathOperator.MATH_ARCSIN:
                        write_pos.eType = enumNodeType.NODE_FUNCTION;
                        write_pos.eOperator = enumMathOperator.MATH_SIN;

                        write_pos.left = new TNode();

                        //write_pos.left.parent = write_pos;
                        //Solve(parent, ref write_pos.left);
                        TNode temp2easin = write_pos.left;
                        temp2easin = Solve(parent);
                        temp2easin.parent = write_pos;
                        write_pos.left = temp2easin;
                        break;
                    case enumMathOperator.MATH_COS:
                        write_pos.eType = enumNodeType.NODE_FUNCTION;
                        write_pos.eOperator = enumMathOperator.MATH_ARCCOS;

                        write_pos.left = new TNode();

                        //write_pos.left.parent = write_pos;
                        //Solve(parent, ref write_pos.left);
                        TNode temp2f = write_pos.left;
                        temp2f=Solve(parent);
                        temp2f.parent = write_pos;
                        write_pos.left = temp2f;
                        break;
                    case enumMathOperator.MATH_ARCCOS:
                        write_pos.eType = enumNodeType.NODE_FUNCTION;
                        write_pos.eOperator = enumMathOperator.MATH_COS;

                        write_pos.left = new TNode();

                        //write_pos.left.parent = write_pos;
                        //Solve(parent, ref write_pos.left);
                        TNode temp2facos = write_pos.left;
                        temp2facos = Solve(parent);
                        temp2facos.parent = write_pos;
                        write_pos.left = temp2facos;
                        break;
                    case enumMathOperator.MATH_TAN:
                        write_pos.eType = enumNodeType.NODE_FUNCTION;
                        write_pos.eOperator = enumMathOperator.MATH_ARCTAN;

                        write_pos.left = new TNode();

                        //write_pos.left.parent = write_pos;
                        //Solve(parent, ref write_pos.left);
                        TNode temp2g = write_pos.left;
                        temp2g=Solve(parent);
                        temp2g.parent = write_pos;
                        write_pos.left = temp2g;
                        break;
                    case enumMathOperator.MATH_ARCTAN:
                        write_pos.eType = enumNodeType.NODE_FUNCTION;
                        write_pos.eOperator = enumMathOperator.MATH_TAN;

                        write_pos.left = new TNode();

                        //write_pos.left.parent = write_pos;
                        //Solve(parent, ref write_pos.left);
                        TNode temp2gatan = write_pos.left;
                        temp2gatan = Solve(parent);
                        temp2gatan.parent = write_pos;
                        write_pos.left = temp2gatan;
                        break;
                    case enumMathOperator.MATH_POWER: //分左右
                        if (bVarIsLeft) //x^3
                        {
                            write_pos.eType = enumNodeType.NODE_OPERATOR;
                            write_pos.eOperator = enumMathOperator.MATH_POWER;

                            TNode dividetemp = NewNode(enumNodeType.NODE_OPERATOR,enumMathOperator.MATH_DIVIDE);
                            TNode num1 = new TNode(1.0);
                            num1.parent = dividetemp;
                            dividetemp.left = num1;                            

                            tempright = CopyNodeTree(brother);
                            tempright.parent = dividetemp;
                            dividetemp.right = tempright;

                            dividetemp.parent = write_pos;
                            write_pos.right = dividetemp;

                            write_pos.left = new TNode();

                            //write_pos.left.parent = write_pos;
                            //Solve(parent, ref write_pos.left);
                            TNode temp2a = write_pos.left;
                            temp2a = Solve(parent);
                            temp2a.parent = write_pos;
                            write_pos.left = temp2a;
                        }
                        else//a^x=y,solve后：x=loga(y)=ln(y)/ln(a)
                        {
                            write_pos.eType = enumNodeType.NODE_OPERATOR;
                            write_pos.eOperator = enumMathOperator.MATH_DIVIDE;

                            TNode lnright = NewNode(enumNodeType.NODE_FUNCTION, enumMathOperator.MATH_LN);
                            TNode lefttemp = CopyNodeTree(brother);
                            lefttemp.parent = lnright;
                            lnright.left = lefttemp;

                            lnright.parent = write_pos;
                            write_pos.right= lnright;

                            //write_pos.right = new TNode();

                            //write_pos.right.parent = write_pos;
                            //Solve(parent, ref write_pos.right);
                            TNode temp2b = write_pos.left;
                            temp2b = Solve(parent);
                            TNode lnleft=NewNode(enumNodeType.NODE_FUNCTION,enumMathOperator.MATH_LN);
                            lnleft.left = temp2b;
                            lnleft.parent = write_pos;                            
                            write_pos.left = lnleft;
                        }
                        break;
                    case enumMathOperator.MATH_LN:
                        write_pos.eType = enumNodeType.NODE_FUNCTION;
                        write_pos.eOperator = enumMathOperator.MATH_EXP;

                        write_pos.left = new TNode();

                        //write_pos.left.parent = write_pos;
                        //Solve(parent, ref write_pos.left);
                        TNode temp2ge = write_pos.left;
                        temp2ge = Solve(parent);
                        temp2ge.parent = write_pos;
                        write_pos.left = temp2ge;
                        break;
                    case enumMathOperator.MATH_LOG10:
                        write_pos.eType = enumNodeType.NODE_OPERATOR;
                        write_pos.eOperator = enumMathOperator.MATH_POWER;

                        TNode temp2lt = new TNode(10.0);
                        temp2lt.parent = write_pos;
                        write_pos.left = temp2lt;

                        //write_pos.left.parent = write_pos;
                        //Solve(parent, ref write_pos.left);
                        TNode temp2log = write_pos.right;
                        temp2log = Solve(parent);
                        temp2log.parent = write_pos;
                        write_pos.right = temp2log;
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
            return write_pos;
        }
        //求解单变量方程 不验证可求解性，需提前调用HasOnlyOneVar确认 不改动表达式内容
        public TExpressionTree Solve()
        {
            TExpressionTree Result = new TExpressionTree();

            TNode ResultNow = new TNode();

            //@var = LastVarNode.varname;

            ResultNow=Solve(LastVarNode);

            Result.head = ResultNow;

            //double value = Result.Value();

            return Result;

        }


        //仅将变量表内置数值代入，不进行计算
        public void Vpa()
        {
            head = Vpa_inner(head);
        }
        private TNode Vpa_inner(TNode now)
        {
            TNode result = CopyNodeTree(now);
            if (now.left != null)
            {
                TNode left = now.left;
                TNode temp = Vpa_inner(left);
                result.left = temp;
                //Vpa_inner(ref now.left);
            }
            if (now.right != null)
            {
                //Vpa_inner(ref now.right);
                TNode right = now.right;
                TNode temp = Vpa_inner(right);
                result.right = temp;
            }

            if (now.eType == enumNodeType.NODE_VARIABLE)
            {
                result.eType = enumNodeType.NODE_NUMBER;
                result.value = pVariableTable.GetValueFromVarPoint(now.varname);
            }
            return result;
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
        public TExpressionTree Subs(string ptVar, double value)
        {
            TExpressionTree newtree = Copythis();
            Subs_inner(newtree.head, ptVar, value);
            return newtree;
        }

        ////vars为被替换变量，nums为替换表达式，以空格分隔
        public void Subs(string vars, string nums)
        {
            if (string.IsNullOrEmpty(vars) || string.IsNullOrEmpty(nums))
            {
                throw new TError(enumError.ERROR_EMPTY_INPUT, "");
                //return;
            }

            List<string> VarsVector = vars.Split(" ".ToCharArray()).ToList();
            List<string> NumsVector = nums.Split(" ".ToCharArray()).ToList();

            if (VarsVector.Count == NumsVector.Count) //替换与被替换元素数目相等
            {
                for (int i = 0; i < VarsVector.Count; i++) //遍历被替换变量
                {
                    Subs_inner(head, VarsVector[i], double.Parse(NumsVector[i]));
                }
            }
            else
            {
                throw new TError(enumError.ERROR_SUBS_NOT_EQUAL, "");
            }

        }

        //替换 VarsVector变量 NumsVector数字
        public void Subs(List<string> VarsVector, List<double> NumsVector)
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
        //只有一个变量（实时验证）
        public bool CheckOnlyOneVar()
        {
            iVarAppearedCount = 0;
            CheckOnlyOneVar(head);
            return HasOnlyOneVar();
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
        private TNode Calc(TNode now)
        {
            TNode result = CopyNodeTree(now);
            if (GetOperateNum(now.eOperator) == 1 && now.left.eType == enumNodeType.NODE_NUMBER)
            {
                try
                {
                    result = CalcNode(now, now.left);
                }
                catch (TError err)
                {
                    throw err;
                }
                result.left = null;
                result.right = null;
                return result;
            }
            if (GetOperateNum(now.eOperator) == 2 && now.left.eType == enumNodeType.NODE_NUMBER && now.right.eType == enumNodeType.NODE_NUMBER)
            {
                try
                {
                    result = CalcNode(now, now.left, now.right);
                }
                catch (TError err)
                {
                    throw err;
                }
                result.left = null;
                result.right = null;
                return result;
            }
            if (result.left != null && result.left.eType != enumNodeType.NODE_NUMBER)
            {
                result.left = Calc(result.left);
            }
            if (result.right!=null && result.right.eType != enumNodeType.NODE_NUMBER)
            {
                result.right = Calc(result.right);
            }
            if(result.right == null && result.left == null)
            {
                return result;
            }
            result = Calc(result);
            
            return result;
        }
        //检查是否还有变量存在，可以计算则返回true
        public bool CanCalc()
        {
            return CanCalc(head);
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
        //计算本表达式值 operateHeadNode决定是否操作本身的节点
        public double Value()
        {
            double num=double.NaN;
            bool operateHeadNode = true;
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
                pNode = Calc(pNode);
            }
            catch (TError err)
            {
                //删掉节点树并提交给上级
                if (operateHeadNode == false)
                {
                    DeleteNode(pNode);
                }
                
                //throw err;
            }

            //得到最终结果
            num = pNode.value;
            //释放复制的树
            if (operateHeadNode == false)
            {
                pNode = null;
            }
            return num;
        }
        //新建一个与此一样的表达式
        public TExpressionTree Copythis()
        {
            TExpressionTree result = new TExpressionTree();
            result.Reset();
            result.LinkVariableTable(pVariableTable);
            result.SetExpression(this.ExpressionString);
            
            return result;
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
        public static TExpressionTree operator -(TExpressionTree ImpliedObject, TExpressionTree expr)
        {
            if (ImpliedObject.head == null)
            {
                ImpliedObject.head = ImpliedObject.CopyNodeTree(expr.head);
            }
            else
            {
                TNode sub = ImpliedObject.NewNode(enumNodeType.NODE_OPERATOR, enumMathOperator.MATH_SUBSTRACT);
                TNode Right = ImpliedObject.CopyNodeTree(expr.head);


                sub.left = ImpliedObject.head;
                sub.right = Right;

                ImpliedObject.head.parent = sub;
                Right.parent = sub;

                ImpliedObject.head = sub;
            }
            return ImpliedObject;
        }
        public static TExpressionTree operator *(TExpressionTree ImpliedObject, TExpressionTree expr)
        {
            if (ImpliedObject.head == null)
            {
                ImpliedObject.head = ImpliedObject.CopyNodeTree(expr.head);
            }
            else
            {
                TNode multiply = ImpliedObject.NewNode(enumNodeType.NODE_OPERATOR, enumMathOperator.MATH_MULTIPLY);
                TNode Right = ImpliedObject.CopyNodeTree(expr.head);


                multiply.left = ImpliedObject.head;
                multiply.right = Right;

                ImpliedObject.head.parent = multiply;
                Right.parent = multiply;

                ImpliedObject.head = multiply;
            }
            return ImpliedObject;
        }
        public static TExpressionTree operator /(TExpressionTree ImpliedObject, TExpressionTree expr)
        {
            if (ImpliedObject.head == null)
            {
                ImpliedObject.head = ImpliedObject.CopyNodeTree(expr.head);
            }
            else
            {
                TNode divide = ImpliedObject.NewNode(enumNodeType.NODE_OPERATOR, enumMathOperator.MATH_DIVIDE);
                TNode Right = ImpliedObject.CopyNodeTree(expr.head);


                divide.left = ImpliedObject.head;
                divide.right = Right;

                ImpliedObject.head.parent = divide;
                Right.parent = divide;

                ImpliedObject.head = divide;
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
        public static TExpressionTree operator /(TExpressionTree ImpliedObject, double value)
        {
            if (ImpliedObject.head == null)
            {
                throw new TError(enumError.ERROR_EMPTY_INPUT, "");
                //return ImpliedObject;
            }

            TNode divide = ImpliedObject.NewNode(enumNodeType.NODE_OPERATOR, enumMathOperator.MATH_DIVIDE);
            TNode Value = ImpliedObject.NewNode(enumNodeType.NODE_NUMBER);
            Value.value = value;

            divide.left = ImpliedObject.head;
            divide.right = Value;

            ImpliedObject.head.parent = divide;
            Value.parent = divide;

            ImpliedObject.head = divide;

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
        //按顺序遍历
        private string TraverseInOrder(TNode now)
        {
            string output = "";
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
                    if (now.parent != null)
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
                output += TraverseInOrder(now.left);
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
        public string OutputStr()
        {
            string temp = "";

            if (head != null)
            {
                temp = TraverseInOrder(head);
            }
            return temp;
        }
        public override string ToString()
        {
            string result = "";
                result += head.ToString();
            return result;
        }
        public string ToLispExpression()
        {
            string result = "";

            return result;
        }

    }
}