using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Swhi.BIM.Solver
{
    /// <summary>
    /// 该类可以用于表达式求值和求导
    /// 效率较低，是TExpressionTree的1/4-1/5
    /// </summary>
    public class LispExpression
    {
        //表达式格式(op,a,b)
        //op为操作符支持，+-*/ ^ sin cos tan asin acos atan atan2 log10 ln e pow  abs 
        //a,b为表达式或数值
        private string str_expression_lisp;
        public string Expression => str_expression_lisp;
        private List<string> _vars;
        public List<string> Vars { get { return _vars; }set { _vars = value; } }
        public int Length=>Expression.Length;
        public LispExpression()
        {

        }
        /// <summary>
        /// 传入Lisp表达式创建
        /// </summary>
        /// <param name="strExpressionLisp"></param>
        public LispExpression(string strExpressionLisp)
        {
            str_expression_lisp = strExpressionLisp;
        }
        public LispExpression(string strExpressionLisp, List<string> varsStrin)
        {
            str_expression_lisp = strExpressionLisp;
            _vars = varsStrin;
        }
        /// <summary>
        /// 根据一般数学表达式创建Lisp表达式
        /// </summary>
        /// <param name="strExpression">为一般数学表达式“cos(x)+ln(10)+y^2”</param>
        /// <param name="vars">变量字符串以空格分割“x y z”</param>
        /// <returns></returns>
        public static LispExpression CreateByExpression(string strExpression, string vars)
        {
            //字符串前处理
            strExpression = strExpression.Replace(" ", "");
            strExpression = strExpression.Replace("\t", "");
            strExpression = strExpression.Replace("\r", "");
            strExpression = strExpression.Replace("\n", "");

            //过滤掉所有多余的加减
            strExpression = strExpression.Replace("--", "+");
            strExpression = strExpression.Replace("+-", "-");
            strExpression = strExpression.Replace("-+", "-");
            strExpression = strExpression.Replace("-0+", "+");
            strExpression = strExpression.Replace("+0-", "-");
            strExpression = strExpression.Replace("+0+", "+");
            strExpression = strExpression.Replace("-0-", "-");
            strExpression = strExpression.Replace("*1*", "*");
            strExpression = strExpression.Replace("*1/", "/");
            strExpression = strExpression.Replace("*1+", "+");
            strExpression = strExpression.Replace("*1-", "-");
            strExpression = strExpression.Replace("/1*", "*");
            strExpression = strExpression.Replace("/1/", "/");
            strExpression = strExpression.Replace("/1+", "+");
            strExpression = strExpression.Replace("/1-", "-");
            strExpression = strExpression.Replace("^1*", "*");
            strExpression = strExpression.Replace("^1/", "/");
            strExpression = strExpression.Replace("^1+", "+");
            strExpression = strExpression.Replace("^1-", "-");
            while(strExpression.EndsWith("+0") || strExpression.EndsWith("-0"))
            {
                strExpression=strExpression.TrimEnd("+0".ToArray());
                strExpression = strExpression.TrimEnd("-0".ToArray());
            }
            while (strExpression.StartsWith("0+") || strExpression.StartsWith("0-"))
            {
                strExpression = strExpression.TrimStart("0+".ToArray());
                strExpression = strExpression.TrimStart("0-".ToArray());
            }
            vars=vars.Replace(",", " ");
            List<string> list = vars.Split(" ".ToCharArray()).ToList();
            string strlisp = ToLispStr(strExpression,list);
            strlisp = strlisp.Replace("<", "(");
            strlisp = strlisp.Replace(">", ")");
            
            return new LispExpression(strlisp, list);
         }
   
        public LispExpression Subs(string var,double value)
        {
            string str = str_expression_lisp;
            str = str.Replace("(" + var + ",", "(" + value + ",");
            str = str.Replace("," + var + ")", "," + value + ")");
            str = str.Replace("," + var + ",", "," + value + ",");
            return new LispExpression(str);
        }
        public LispExpression Subs(string vars, string values)
        {
            string str = str_expression_lisp;
            List<string> varstrs = new List<string>();
            List<string> valuestrs = new List<string>();
            if (vars.Contains(","))
            {
                
                varstrs=vars.Split(",".ToCharArray()).ToList();
                valuestrs = values.Split(",".ToCharArray()).ToList();
                
            }
            else if (vars.Contains(" "))
            {

                varstrs = vars.Split(" ".ToCharArray()).ToList();
                valuestrs = values.Split(" ".ToCharArray()).ToList();

            }
            else
            {
                varstrs.Add(vars);
                valuestrs.Add(values);
            }
            for (int i=0;i< varstrs.Count; i++)
            {
                string var= varstrs[i];
                string value=valuestrs[i];                
                str = str.Replace("(" + var + ",", "(" + value + ",");
                str = str.Replace("," + var + ")", "," + value + ")");
                str = str.Replace("," + var + ",", "," + value + ",");
               
            }
            return new LispExpression(str);
        }
        public LispExpression Value(string format = "0.00000")
        {
            string lispstr = str_expression_lisp;
            if (IsNumeric(lispstr) || IsInt(lispstr))
            {
                return new LispExpression(lispstr);
            }
            if (!HasChild())
            {
                return (LispExpression)ValueOfNoChild() ;
            }
            else
            {
                if (!IsUnaryOperator())
                {
                    string op = GetOperatorStr();
                    LispExpression a = GetParamAStr();
                    LispExpression b = GetParamBStr();
                    if (!a.Contains("(") && !b.Contains("("))
                    {
                        return (LispExpression)ValueOfNoChild();
                    }
                    else if (a.Contains("(") && !b.Contains("("))
                    {
                        LispExpression anew = a.Value();
                        LispExpression lispnew = (LispExpression)("(" + op + "," + anew.Expression + "," + b.Expression + ")");
                        return lispnew.Value();
                    }
                    else if (!a.Contains("(") && b.Contains("("))
                    {
                        LispExpression bnew = b.Value();
                        LispExpression lispnew = (LispExpression)("(" + op + "," + a.Expression + "," + bnew.Expression + ")");
                        return lispnew.Value();
                    }
                    else
                    {
                        LispExpression anew = a.Value();
                        LispExpression bnew = b.Value();
                        LispExpression lispnew = (LispExpression)("(" + op + "," + anew.Expression + "," + bnew.Expression + ")");
                        return lispnew.Value();
                    }
                }
                else
                {
                    string op = GetOperatorStr();
                    LispExpression a = GetParamAStr();
                    if (!a.Contains("("))
                    {
                        return (LispExpression)ValueOfNoChild();
                    }
                    else
                    {
                        LispExpression anew = a.Value();
                        LispExpression lispnew = (LispExpression)("(" + op + "," + anew.Expression + ")");
                        return lispnew.Value();
                    }
                }


            }
        }
        /// <summary>
        /// 一般表达式转Lisp表达式
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="vars"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static string ToLispStr(string expression,List<string> vars)
        {
            //0字符串前处理,删除空格
            string expressionin = expression.Replace(" ", "");
            //替换中文括号
            expressionin = expressionin.Replace("（", "(");
            expressionin = expressionin.Replace("）", ")");
            //判断左括号数和右括号数是否相等
            int coul = Regex.Matches(expressionin, @"[(]").Count;
            int cour = Regex.Matches(expressionin, @"[)]").Count;
            if (coul != cour)
            {
                throw new Exception("输入字符串错误");
            }
            //转换后的元素用<>包围
            if(expressionin.StartsWith("(") && expressionin.EndsWith(")"))
            {
                expressionin = expressionin.Substring(1, expressionin.Length - 2);
                expressionin = "<"+expressionin+">";
            }
            while(expressionin.StartsWith("<<") && expressionin.EndsWith(">>"))
            {
                expressionin = expressionin.Substring(1, expressionin.Length - 2);
            }
            
            List<string> baseoperator=new List<string > { "^","*","/","+","-"};
            List<string> basefunc=new List<string> { "sin","cos","tan","asin","acos", "atan", "ln", "log10","exp","sqrt" };
            List<string> baseall = new List<string>();
            baseall.AddRange(basefunc);
            baseall.AddRange(baseoperator);
            /**20221007前函数处理
            //01函数处理，例：sin(x)替换为<sin,x>,sin(5.0)=>(sin,5.0)
            if (vars != null)
            {
                //替换变量函数
                List<string> varlis = vars.Split(' ').ToList();
                List<string> funcVarStrs = new List<string>();
                List<string> funcVarStrsLisp=new List<string>();
                foreach (string var in varlis)
                {
                    foreach(string f in basefunc)
                    {
                        funcVarStrs.Add(f + "(" + var + ")");
                        funcVarStrsLisp.Add("<" + f + "," + var + ">");
                    }
                }
                for(int i=0;i< funcVarStrs.Count; i++)
                {
                    string a = funcVarStrs[i];
                    string anew = funcVarStrsLisp[i];
                    expressionin=expressionin.Replace(a, anew);
                }
            }
            //替换数值函数sin(5.0)=>(sin,5.0)
            //Regex.IsMatch(value, "^[+-]?\\d*[.]?\\d*$")；
            foreach (string f in basefunc)
            {
                string a = f + "[(]" + "([+-]?\\d+[.]?\\d*)" + "[)]";
                string anew = "<" + f + "," + "$1" + ">";
                //bool isma = Regex.IsMatch(expressionin, a);
                expressionin = Regex.Replace(expressionin, a, anew);
            }
            **/
            //处理function
            //20221006此处尚有问题ln(sqrt(x^2))处理不了
            foreach (string f in basefunc)
            {
                string a = f+"(";
                string anew = "(" + f + ",";
                //bool isma = Regex.IsMatch(expressionin, a);
                expressionin = expressionin.Replace(a, anew);
            }
            foreach (string f in basefunc)
            {                
                string anew = "(" + f + ",";
                string pattern = "\\(" + f;
                MatchCollection mcs= Regex.Matches(expressionin, pattern);
                foreach (Match m in mcs)
                {
                    int st=m.Index;
                    int ed=bracketsMatchedSearch(expressionin,st);
                    if (ed > 0)
                    {
                        expressionin=expressionin.Remove(st, 1);
                        expressionin = expressionin.Insert(st, "<");
                        expressionin=expressionin.Remove(ed,1);
                        expressionin=expressionin.Insert(ed, ">");
                    }
                }
                //bool isma = Regex.IsMatch(expressionin, a);
                
            }

            while (!isFuncAlldone(expressionin))
            {
                foreach (string f in basefunc)
                {
                    string a = f + "[(]" + "([<].*?[>])" + "[)]";
                    string anew = "<" + f + "," + "$1" + ">";
                    //bool isma = Regex.IsMatch(expressionin, a);
                    expressionin = Regex.Replace(expressionin, a, anew);
                }
            }
            //1替换()，即处理括号，处理顺序为最右侧、最内侧
            while (expressionin.Contains("("))
            {
                int left = expressionin.LastIndexOf("(");
                int right = bracketsMatchedSearch(expressionin,left);
                string childexp = expressionin.Substring(left, right-left+1);

                string childexpWithoutkuohao= childexp.Substring(1, childexp.Length-2);
                expressionin =expressionin.Replace(childexp, ToLispStr(childexpWithoutkuohao, vars));
            }

            //3替换操作符
            expressionin = handleOperator(expressionin, "^");
            expressionin = handleOperator(expressionin, "*","/");
            expressionin = handleOperator(expressionin, "+","-");
            //4替换完全标准，每一个左括号下一位是baseoperator或者basefunc

            return expressionin;
            bool isFuncAlldone(string expStr)
            {
                foreach (string item in basefunc)
                {
                    if (expStr.Contains(item))
                    {
                        string[] arys  = Regex.Split(expStr, item, RegexOptions.IgnoreCase);
                        for(int i = 1; i < arys.Length; i++)
                        {
                            if (!arys[i].StartsWith(","))
                            {
                                return false;
                            }
                        }
                    }
                }
                return true;
            }
            
            //返回操作符之前<>中内容
            string kuohaoBeforeoperator(string expstr,int positionindex)
            {
                string qian1 = expstr.Substring(positionindex - 1, 1);
                if (qian1 != ">")
                {
                    throw new Exception("操作符之前不是<");
                }
                List<char> list = new List<char>();
                int flag = 0;
                for(int i = positionindex-1; i >= 0; i--)
                {
                    char c = expstr[i];
                    if (c == ">"[0]) { flag++; }
                    if (c == "<"[0]) { flag = flag - 1; }
                    list.Add(c);
                    if(flag == 0)
                    {
                        break;
                    }
                }
                list.Reverse();
                return new string(list.ToArray());
            }
            string kuohaoAfteroperator(string expstr,int positionindex)
            {
                //char[] strary = expstr.ToCharArray();
                string hou1 = expstr.Substring(positionindex + 1, 1);
                if (hou1 != "<")
                {
                    throw new Exception("操作符之后不是<");
                }
                List<char> list = new List<char>();
                int flag = 0;
                for(int i = positionindex+1; i <expstr.Length; i++)
                {
                    char c = expstr[i];
                    if (c == "<"[0]) { flag++; }
                    if (c == ">"[0]) { flag = flag - 1; }
                    list.Add(c);
                    if(flag == 0)
                    {
                        break;
                    }
                }
                //list.Reverse();
                return new string(list.ToArray());
            }

            string doubleBeforeoperator(string expstr, int positionindex)
            {
                string qian = expstr.Substring(0, positionindex);
                var result = Regex.Match(qian, "\\d+[.]?\\d*$");//"[+-]?\\d+[.]?\\d*$"
                if (result.Success)
                {
                    return result.Groups[result.Groups.Count-1].Value;
                }
                else
                {
                    throw new Exception("字符格式不对");
                }
  
            }            
            string doubleAfteroperator(string expstr, int positionindex)
            {
                string hou = expstr.Substring(positionindex + 1, expstr.Length- positionindex-1);
                var result = Regex.Match(hou, "^[+-]?\\d+[.]?\\d*");
                if (result.Success)
                {
                    return result.Groups[0].Value;
                }
                else
                {
                    throw new Exception("字符格式不对");
                } 
            }

            string varBeforeoperator(string expstr, int positionindex)
            {
                string qian = expstr.Substring(0, positionindex);
                if (vars != null && vars.Count>0)
                {
                    //替换变量函数
                    List<string> varlis = vars;
                    foreach (string var in varlis)
                    {
                        var result = Regex.Match(qian, var + "$");//"[+-]?\\d+[.]?\\d*$"
                        if (result.Success)
                        {
                            return result.Groups[result.Groups.Count - 1].Value;
                        }
                    }
                    return null;
                }
                throw new Exception("没有变量");

            }
            string varAfteroperator(string expstr, int positionindex)
            {
                string hou = expstr.Substring(positionindex + 1, expstr.Length - positionindex - 1);
                if (vars != null && vars.Count > 0)
                {
                    //替换变量函数
                    List<string> varlis = vars;
                    foreach (string var in varlis) 
                    {
                        var result = Regex.Match(hou, "^"+var);
                        if (result.Success)
                        {
                            return result.Groups[0].Value;
                        }
                    }
                    return null;
                }
                else
                {
                    throw new Exception("没有变量");
                }
                
                        
                
            }

            string handleFunction(string stringin,string funstr)
            {                
                string strout = stringin;
                string pattern = funstr + "(";
                string patternnew = "("+funstr + ",";
                stringin.Replace(pattern, patternnew);
                return strout;

            }
            //支持两个同级运算符，例如op1="*",op2="/"
            string handleOperator(string expstr,string op1,string op2=null)
            {
                string pattern = @"(\"+op1 + ")";
                if (op2 != null)
                {
                    pattern = @"(\" + op1 + @"|\" + op2 + ")";
                }
                MatchCollection mcs = Regex.Matches(expstr,pattern);
                //替换后数据长度变化
                MatchCollection mcsnew = Regex.Matches(expstr,pattern);
                int i = 0;
                if(mcs.Count > 0) 
                {
                    foreach(Match mc in mcs)
                    {
                        mcsnew = Regex.Matches(expstr, pattern);
                        string pipei = mcsnew[i].Value;
                        int po = mcsnew[i].Index;
                        
                        string before1 = expstr.Substring(po - 1, 1);
                        string after1 = expstr.Substring(po + 1, 1);
                        //<*,格式是已经处理格式
                        if (before1 != "<" || after1 != ",")
                        {
                            string before = "";
                            string after = "";

                            if (before1 == ">")
                            {
                                before = kuohaoBeforeoperator(expstr, po);
                            }
                            else
                            {
                                if (vars != null)
                                {
                                    before = varBeforeoperator(expstr, po);
                                    if (before == null)
                                    {
                                        before = doubleBeforeoperator(expstr, po);
                                    }
                                }
                                else
                                {
                                    before = doubleBeforeoperator(expstr, po);
                                }

                            }
                            if (after1 == "<")
                            {
                                after = kuohaoAfteroperator(expstr, po);
                            }
                            else
                            {
                                if (vars != null)
                                {
                                    after = varAfteroperator(expstr, po);
                                    if (after == null)
                                    { after = doubleAfteroperator(expstr, po); }
                                }
                                else
                                {
                                    after = doubleAfteroperator(expstr, po);
                                }
                            }

                            expstr = expstr.Replace(before + pipei + after, "<" + pipei + "," + before + "," + after + ">");
                        }
                        i++;
                    }
                }
                return expstr;
            }

        }
        
        public LispExpression Diff(string @var)
        {
            string lispstr = str_expression_lisp;
            string result = "";
            if (lispstr == @var)
            {
                return new LispExpression("1");
            }else if (IsNumeric(lispstr))
            {
                return new LispExpression("0");
            }else if (Vars!=null && Vars.Count>0 && Vars.Contains(lispstr))
            {
                return new LispExpression("0");
            }
            if (!HasChild())
            {
                return DiffOfNoChild(@var);
            }
            else
            {
                if (!IsUnaryOperator())
                {
                    string op = GetOperatorStr();
                    LispExpression a = GetParamAStr();
                    LispExpression b = GetParamBStr();
                    LispExpression adao=new LispExpression();
                    LispExpression bdao=new LispExpression();
                    if (!a.HasChild() && !b.HasChild())
                    {
                        adao = a.DiffOfNoChild(@var);
                        bdao = b.DiffOfNoChild(@var);

                        //return (LispExpression)DiffOfNoChild(@var);
                    }
                    else if (a.HasChild() && !b.HasChild())
                    {
                        adao = a.Diff(@var);
                        bdao = b.DiffOfNoChild(@var);
                    }
                    else if (!a.HasChild() && b.HasChild())
                    {
                        adao = a.DiffOfNoChild(@var);
                        bdao = b.Diff(@var);
                        
                    }
                    else
                    {
                        adao = a.Diff(@var);
                        bdao = b.Diff(@var);
                    }
                    switch (op)
                    {
                        case "+":
                            return adao + bdao;
                        case "-":
                            return adao - bdao;
                        case "*":
                            return (adao * b) + (a * bdao);
                        case "/":
                            return ((adao * b) - (a * bdao)) / (b ^ "2");
                        case "pow"://[f^g]·{[g/f] + g'lnf(x)}
                        case "^":
                        case "**":
                            if (!a.IsContainsVar(@var) && !b.IsContainsVar(@var))
                            {
                                return new LispExpression("0");
                            }else if(a.IsContainsVar(@var) && !b.IsContainsVar(@var))
                            {
                                return adao *b* (a ^ (b - "1"));
                            }else 
                            {
                                LispExpression lna = new LispExpression("(ln," + a.Expression + ")");
                                return this * ((b / a) + (bdao * (lna)));
                            }
                           
                    }
                }
                else
                {
                    string op = GetOperatorStr();
                    LispExpression a = GetParamAStr();
                    LispExpression adao = new LispExpression();
                    if (!a.HasChild())
                    {
                        adao= a.DiffOfNoChild(@var);
                    }
                    else
                    {
                        adao = a.Diff(@var);
                    }
                    switch (op)
                    {
                        case "cos":
                            LispExpression sina = new LispExpression("(sin," + a.Expression + ")");
                            return adao*(-sina);
                        case "sin":
                            LispExpression cosa = new LispExpression("(cos," + a.Expression + ")");
                            return adao*cosa;
                        case "tan":
                            LispExpression cosa1 = new LispExpression("(cos," + a.Expression + ")");
                            return adao/(cosa1^"2");
                        case "acos":
                            LispExpression alef1 = "1" - (a ^ "2");
                            LispExpression aleg1 = new LispExpression("(sqrt," + alef1.Expression + ")");
                            return -(adao / aleg1);
                        case "asin":
                            LispExpression alef = "1" - (a ^ "2");
                            LispExpression aleg = new LispExpression("(sqrt," + alef.Expression + ")");
                            return (LispExpression)adao / aleg;
                        case "atan":
                            LispExpression alef2 = "1" + (a ^ "2");
                            return (LispExpression)adao / alef2;
                        case "ln":
                            return adao/ a;
                        case "log10":
                            return (adao / (a * (Math.Log(10).ToString())));
                        case "e":
                        case "exp":
                            return adao * this;
                        case "sqrt":
                            return adao* "0.5" / this;
                        case "-":
                            return -adao;
                    }
                }

            }

            return (LispExpression)result;
        }
        public bool Contains(string source)
        {
            return Expression.Contains(source);
        }
        public bool IsContainsVar(string var)
        {
            List<string> basefunc = new List<string> { "sin", "cos", "tan", "asin", "acos", "atan", "ln", "log10", "exp", "sqrt","pow" };
            string expstr = Expression;
            foreach (string item in basefunc)
            {
                expstr = expstr.Replace(item, "");
            }
            return expstr.Contains(var);
        }
        /**
	 * 得到匹配括号的位置
	 * 
	 * 返回对应坐标为正常，
	 * -1表示传进来的坐标不是左括号也不是空格，是空格会顺位向后匹配直到遇到左括号
	 * -2表示不存在对应右括号（不应该出现，应该提前校验过括号配对）
	 * 
	 * @date 2018年11月22日 上午10:54:50
	 * @param checkedStr
	 * @return
	 */
        public static int bracketsMatchedSearch(string checkedstring, int leftIndex)
        {
            char[] checkedCharArray= checkedstring.ToCharArray();
            // 校验传进来的数组和索引是否为合法
            if (checkedCharArray.Length < leftIndex)
            {
                return -1;
            }

            char left = checkedCharArray[leftIndex];
            // 非左括号
            if (!('(' == left || '[' == left || '{' == left))
            {
                // 如果是空格则+1
                if (' ' == left)
                {
                    return bracketsMatchedSearch(new string(checkedCharArray), leftIndex + 1);
                }
                else
                {
                    return -1;
                }
            }

            /*
             *  获取传进来的是第几个左括号
             */
            int index = 0;
            string pattern = "\\(|\\[|\\{";
            MatchCollection mcs = Regex.Matches(new string(checkedCharArray), pattern);
            //Matcher matcher = Pattern.compile("\\(|\\[|\\{").matcher(new String(checkedCharArray));
            if (mcs.Count > 0)
            {
                foreach (Match mc in mcs)
                {
                    index++;
                    if (mc.Index == leftIndex)
                    {
                        break;
                    }
                }
            }
            /*
             *  获取另一配对括号位置
             */
            Stack<Char> bracketsStack = new Stack<Char>();
            int count = 0;
            for (int i = 0; i < checkedCharArray.Length; i++)
            {
                char c = checkedCharArray[i];

                // 左括号都压入栈顶，右括号进行比对
                if (c == '(' || c == '[' || c == '{')
                {
                    count++;
                    // 如果是目标，就插入*作为标记
                    if (index == count)
                    {
                        bracketsStack.Push('*');
                    }
                    else
                    {
                        bracketsStack.Push(c);
                    }
                }
                else if (c == ')')
                {
                    // 栈非空校验，防止首先出现的是右括号
                    if (bracketsStack.Count==0)
                    {
                        return i;
                    }
                    Char popChar = bracketsStack.Pop();
                    if ('*' == popChar)
                    {
                        return i;
                    }
                }
                else if (c == ']')
                {
                    if (bracketsStack.Count==0)
                    {
                        return i;
                    }
                    Char popChar = bracketsStack.Pop();
                    if ('*' == popChar)
                    {
                        return i;
                    }
                }
                else if (c == '}')
                {
                    if (bracketsStack.Count==0)
                    {
                        return i;
                    }
                    Char popChar = bracketsStack.Pop();
                    if ('*' == popChar)
                    {
                        return i;
                    }
                }
            }

            return -2;
        }
        private bool HasChild()
        {
            string lispstr = str_expression_lisp;
            int coul = Regex.Matches(lispstr, @"[(]").Count;
            int cour = Regex.Matches(lispstr, @"[)]").Count;
            if(coul==1 && cour == 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private LispExpression ValueOfNoChild(string format="0.00000")
        {
            string lispStrNoChid = str_expression_lisp;
                string[] strary = lispStrNoChid.Split(',');
                string result = "";
                if (strary.Length==3)
                {
                    string op=strary[0].TrimStart('(');
                    string a=strary[1];
                    string b = strary[2].TrimEnd(')');
                    
                    switch (op)
                    {
                        case "+":
                            result = (double.Parse(a) + double.Parse(b)).ToString(format);
                            return (LispExpression)result;
                        case "-":
                            result = (double.Parse(a) - double.Parse(b)).ToString(format);
                            return (LispExpression)result;
                        case "*":
                            result = (double.Parse(a) * double.Parse(b)).ToString(format);
                            return (LispExpression)result;
                        case "/":
                            result = (double.Parse(a) / double.Parse(b)).ToString(format);
                            return (LispExpression)result;
                        case "atan2":
                            result = Math.Atan2(double.Parse(a) , double.Parse(b)).ToString(format);
                            return (LispExpression)result;
                        case "pow":
                        case "^":
                        case "**":
                            result = Math.Pow(double.Parse(a) , double.Parse(b)).ToString(format);
                            return (LispExpression)result;
                       default: throw new ArgumentException("操作符有误");
                    }
                }
                else if(strary.Length==2)
                {
                    string op=strary[0].TrimStart('(');
                    string a=strary[1].TrimEnd(')');
                    switch (op)
                    {
                    case "cos":
                            result = Math.Cos(double.Parse(a)).ToString(format);
                            return (LispExpression)result;
                        case "sin":
                            result = Math.Sin(double.Parse(a)).ToString(format);
                            return (LispExpression)result;
                        case "tan":
                            result = Math.Tan(double.Parse(a)).ToString(format);
                            return (LispExpression)result;
                        case "ln":
                            result = Math.Log(double.Parse(a)).ToString(format);
                            return (LispExpression)result;
                        case "log10":
                            result = Math.Log10(double.Parse(a)).ToString(format);
                            return (LispExpression)result;
                        case "e":
                            result = Math.Exp(double.Parse(a)).ToString(format);
                            return (LispExpression)result;
                        case "exp":
                            result = Math.Exp(double.Parse(a)).ToString(format);
                            return (LispExpression)result;
                        case "asin":
                            result = Math.Asin(double.Parse(a)).ToString(format);
                            return (LispExpression)result;
                        case "acos":
                            result = Math.Acos(double.Parse(a)).ToString(format);
                            return (LispExpression)result;
                        case "atan":
                            result = Math.Atan(double.Parse(a)).ToString(format);
                            return (LispExpression)result;
                        case "abs":
                            result = Math.Abs(double.Parse(a)).ToString(format);
                            return (LispExpression)result;
                        case "sqrt":
                            result = Math.Sqrt(double.Parse(a)).ToString(format);
                            return (LispExpression)result;
                        case "-":
                            result = (0-double.Parse(a)).ToString(format);
                            return (LispExpression)result;
                        default: throw new ArgumentException("操作符有误");
                    }
                }
                else
                {
                    throw new ArgumentException("输入lisp字符串不符合要求");
                }
                
        }
        private LispExpression DiffOfNoChild(string @var)
        {
            string lispStrNoChid = str_expression_lisp;
            string[] strary = lispStrNoChid.Split(',');
            string result = "";
            if (strary.Length == 3)
            {
                string op = strary[0].TrimStart('(');
                string a = strary[1];
                string b = strary[2].TrimEnd(')');
                string newa = a;
                string newb = b;
                if (a == @var)
                {
                    newa = "x";
                }else if (IsNumeric(a) || IsInt(a))
                {
                    newa = "1";
                }
                else
                {
                    newa = "1";
                }
                if (b == @var)
                {
                    newb = "x";
                }else if (IsNumeric(b) || IsInt(b))
                {
                    newb = "1";
                }
                else
                {
                    newb = "1";
                }
                string newex = "(" + op + "," + newa + "," + newb + ")";
                switch (newex)
                {
                    case "(pow,1,1)":
                    case "(^,1,1)":
                    case "(**,1,1)":
                    case "(+,1,1)":
                    case "(-,1,1)":
                    case "(*,1,1)":
                    case "(/,1,1)":
                        return (LispExpression)"0";
                    case "(+,x,1)":
                    case "(-,x,1)":
                    case "(+,1,x)":
                        result = "1";
                        return (LispExpression)result;
                    case "(+,x,x)":
                        result = "2";
                        return (LispExpression)result;
                    case "(-,1,x)":
                        result = "-1";
                        return (LispExpression)result;
                    case "(-,x,x)":
                        result = "0";
                        return (LispExpression)result;
                    case "(*,x,1)":
                        result = b;
                        return (LispExpression)result;
                    case "(*,1,x)":
                        result = a;
                        return (LispExpression)result;
                    case "(*,x,x)":
                        result = "(*,2,"+@var+")";
                        return (LispExpression)result;
                    case "(/,x,1)":
                        result = (1/double.Parse(b)).ToString();
                        return (LispExpression)result;
                    case "(/,1,x)":
                        LispExpression ale = new LispExpression((-double.Parse(a)).ToString());
                        LispExpression ble = new LispExpression(@var)^"2";
                        result = ale/ble;
                        return (LispExpression)result;
                    case "(/,x,x)":
                        result = "1";
                        return (LispExpression)result;
                    
                    //(a^x)'=(lna)(a^x)
                    case "(pow,1,x)":
                    case "(^,1,x)":
                    case "(**,1,x)":
                        LispExpression ale1 = new LispExpression(Math.Log(double.Parse(a)).ToString());
                        LispExpression ble1 = this;
                        result = ale1 * ble1;
                        return (LispExpression)result;
                    case "(pow,x,1)":
                    case "(^,x,1)":
                    case "(**,x,1)":
                        if (double.Parse(b) == 1)
                        {
                            return (LispExpression)"1";
                        }
                        else
                        {
                            LispExpression ale2 = new LispExpression((double.Parse(b)).ToString());
                            LispExpression ble2 = @var^new LispExpression((double.Parse(b) - 1).ToString()); 
                            result = ale2 * ble2;
                            return (LispExpression)result;
                        }
                    case "(pow,x,x)":
                    case "(^,x,x)":
                    case "(**,x,x)":
                        LispExpression ale3= new LispExpression("(ln,"+var+")")+"1";
                        return (LispExpression)this * ale3;
                    default: throw new ArgumentException("操作符有误");
                }
            }
            else if (strary.Length == 2)
            {
                string op = strary[0].TrimStart('(');
                string a = strary[1].TrimEnd(')');
                string newa = a;
                if (a == @var)
                {
                    newa = "x";
                }
                if (IsNumeric(a) || IsInt(a))
                {
                    newa = "1";
                }
                string newex = "(" + op + "," + newa + ")";
                switch (newex)
                {
                    case "(cos,1)":
                    case "(sin,1)":
                    case "(tan,1)":
                    case "(acos,1)":
                    case "(asin,1)":
                    case "(atan,1)":
                    case "(e,1)":
                    case "(exp,1)":
                    case "(log10,1)":
                    case "(ln,1)":
                    case "(-,1)":
                        return (LispExpression)"0";
                    case "(cos,x)":
                        LispExpression alea = new LispExpression("(sin,"+var+")");
                        return -alea;
                    case "(sin,x)":
                        LispExpression aleb = new LispExpression("(cos," + var + ")");
                        return (LispExpression)aleb;
                    case "(tan,x)"://(tanx)'=1/(cosx)^2
                        LispExpression alec = new LispExpression("(cos," + var + ")");
                        return (LispExpression)("1"/(alec^"2"));
                    case "(ln,x)":
                        LispExpression aled = new LispExpression(var);
                        return (LispExpression)("1" / aled);
                    case "(log10,x)"://1/(x*ln(10))
                        LispExpression alee = new LispExpression(var);
                        return (LispExpression)("1" / (alee*(Math.Log(10).ToString())));
                    case "(e,x)":
                    case "(exp,x)":
                        return this;
                    case "(asin,x)"://1/sqrt(1-x^2)
                        LispExpression alef ="1"-( new LispExpression(var)^"2");
                        LispExpression aleg =new LispExpression("(sqrt,"+alef.Expression+")");
                        return (LispExpression)"1"/aleg;
                    case "(acos,x)":
                        //-1 / sqrt(1 - x ^ 2)
                        LispExpression alef1 = "1" - (new LispExpression(var) ^ "2");
                        LispExpression aleg1 = new LispExpression("(sqrt," + alef1.Expression + ")");
                        return (LispExpression)"-1" / aleg1;
                    case "(atan,x)":
                        //1 / (1 + x ^ 2)
                        LispExpression alef2 = "1" + (new LispExpression(var) ^ "2");
                        return (LispExpression)"1" / alef2;
                    //case "abs":
                        
                    //    return (LispExpression)result;
                    case "(sqrt,x)":                        
                        return "0.5"/this;
                    case "(-,x)":                        
                        return (LispExpression)"-1";
                    default: throw new ArgumentException("操作符有误");
                }
            }
            else
            {
                throw new ArgumentException("输入lisp字符串不符合要求");
            }
        }
        public static bool IsNumeric(string value)
        {
            return Regex.IsMatch(value, "^[+-]?\\d+[.]?\\d*$");
        }
        public static bool IsInt(string value)
        {
            return Regex.IsMatch(value, "^[+-]?\\d+$");
        }
        public static LispExpression operator +(LispExpression left, LispExpression right)
        {
            if (null == left)
            {
                throw new Exception("空值错误");
            }
            if (null == right)
            {
                throw new Exception("空值错误");
            }
            if(left.Expression == "0")
            {
                return right;
            }
            if (right.Expression == "0")
            {
                return left;
            }
            return new LispExpression("(+,"+left.Expression+","+right.Expression+")");
        }
        public static LispExpression operator +(LispExpression left, string right)
        {
            if (null == left)
            {
                throw new Exception("空值错误");
            }
            if (null == right)
            {
                throw new Exception("空值错误");
            }
            if (left.Expression == "0")
            {
                return new LispExpression(right);
            }
            if (right == "0")
            {
                return left;
            }
            return new LispExpression("(+," + left.Expression + "," + right + ")");
        }
        public static LispExpression operator +(string  left, LispExpression right)
        {
            if (null == left)
            {
                throw new Exception("空值错误");
            }
            if (null == right)
            {
                throw new Exception("空值错误");
            }
            if (left == "0")
            {
                return right;
            }
            if (right.Expression == "0")
            {
                return new LispExpression(left);
            }
            return new LispExpression("(+," + left + "," + right.Expression + ")");
        }
        public static LispExpression operator -(LispExpression left, LispExpression right)
        {
            if (null == left)
            {
                throw new Exception("空值错误");
            }
            if (null == right)
            {
                throw new Exception("空值错误");
            }
            if (right.Expression == "0")
            {
                return left;
            }
            return new LispExpression("(-," + left.Expression + "," + right.Expression + ")");
        }
        public static LispExpression operator -(string left, LispExpression right)
        {
            if (null == left)
            {
                throw new Exception("空值错误");
            }
            if (null == right)
            {
                throw new Exception("空值错误");
            }
            if (right.Expression == "0")
            {
                return new LispExpression( left);
            }
            return new LispExpression("(-," + left + "," + right.Expression + ")");
        }
        public static LispExpression operator -(LispExpression left, string right)
        {
            if (null == left)
            {
                throw new Exception("空值错误");
            }
            if (null == right)
            {
                throw new Exception("空值错误");
            }
            if (right == "0")
            {
                return left;
            }
            return new LispExpression("(-," + left.Expression + "," + right + ")");
        }
        public static LispExpression operator *(LispExpression left, LispExpression right)
        {
            if (null == left)
            {
                throw new Exception("空值错误");
            }
            if (null == right)
            {
                throw new Exception("空值错误");
            }
            if(right.Expression == "0" || left.Expression == "0")
            {
                return new LispExpression("0");
            }
            return new LispExpression("(*," + left.Expression + "," + right.Expression + ")");
        }
        public static LispExpression operator *(LispExpression left, string right)
        {
            if (null == left)
            {
                throw new Exception("空值错误");
            }
            if (null == right)
            {
                throw new Exception("空值错误");
            }
            if (right== "0" || left.Expression == "0")
            {
                return new LispExpression("0");
            }
            return new LispExpression("(*," + left.Expression + "," + right + ")");
        }
        public static LispExpression operator /(LispExpression left, LispExpression right)
        {
            if (null == left)
            {
                throw new Exception("空值错误");
            }
            if (null == right)
            {
                throw new Exception("空值错误");
            }
            if ( left.Expression == "0")
            {
                return new LispExpression("0");
            }
            return new LispExpression("(/," + left.Expression + "," + right.Expression + ")");
        }
        public static LispExpression operator /(string left, LispExpression right)
        {
            if (null == left)
            {
                throw new Exception("空值错误");
            }
            if (null == right)
            {
                throw new Exception("空值错误");
            }
            if (left== "0")
            {
                return new LispExpression("0");
            }
            return new LispExpression("(/," + left + "," + right.Expression + ")");
        }
        public static LispExpression operator /(LispExpression left, string right)
        {
            if (null == left)
            {
                throw new Exception("空值错误");
            }
            if (null == right)
            {
                throw new Exception("空值错误");
            }
            if (left.Expression == "0")
            {
                return new LispExpression("0");
            }
            return new LispExpression("(/," + left.Expression + "," + right + ")");
        }
        public static LispExpression operator ^(LispExpression left, LispExpression right)
        {
            if (null == left)
            {
                throw new Exception("空值错误");
            }
            if (null == right)
            {
                throw new Exception("空值错误");
            }
            if ( left.Expression == "0")
            {
                return new LispExpression("0");
            }
            if (right.Expression == "0")
            {
                return new LispExpression("1");
            }
            return new LispExpression("(^," + left.Expression + "," + right.Expression + ")");
        }
        public static LispExpression operator ^(LispExpression left, string right)
        {
            if (null == left)
            {
                throw new Exception("空值错误");
            }
            if (null == right)
            {
                throw new Exception("空值错误");
            }
            if (left.Expression == "0")
            {
                return new LispExpression("0");
            }
            if (right == "0")
            {
                return new LispExpression("1");
            }
            return new LispExpression("(^," + left.Expression + "," + right + ")");
        }
        public static LispExpression operator ^(string left, LispExpression right)
        {
            if (null == left)
            {
                throw new Exception("空值错误");
            }
            if (null == right)
            {
                throw new Exception("空值错误");
            }
            if (left== "0")
            {
                return new LispExpression("0");
            }
            if (right.Expression == "0")
            {
                return new LispExpression("1");
            }
            return new LispExpression("(^," + left + "," + right.Expression + ")");
        }
        //public static bool operator ==(LispExpression left, LispExpression right)
        //{
        //    if(left==null || right == null)
        //    {
        //        return false;
        //    }
        //    return left.Expression==right.Expression;
        //}
        //public static bool operator !=(LispExpression left, LispExpression right)
        //{

        //    return left.Expression != right.Expression;
        //}
        public static implicit operator string(LispExpression value) //定义自定义隐式转换为string
        {
            return value.Expression;    //转换为string
        }
        public static explicit operator LispExpression(string value) //LispExpression
        {
            return new LispExpression(value);    //转换为LispExpression
        }
        public static LispExpression operator -(LispExpression source)
        {
            if (null == source)
            {
                throw new Exception("空值错误");
            }
            return new LispExpression("(-," + source.Expression + ")");
        }
        private bool IsUnaryOperator()
        {
            //string lispstr = str_expression;
            string operatorStr=this.GetOperatorStr();
            switch (operatorStr)
            {
                case "cos":
                case "sin":
                case "tan":
                case "ln":
                case "log10":
                case "e":
                case "exp":
                case "asin":
                case "acos":
                case "atan":
                case "abs":
                case "sqrt":
                    return true;
                case "-":
                    {
                        LispExpression a = GetParamAStr();
                        LispExpression b = GetParamBStr();
                        if (a.Expression == b.Expression && Length == (a.Length + operatorStr.Length+ 3))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                        
                    }
                    
                default:
                    return false;
            }
        }
        private string GetOperatorStr()
        {
            string lispstr=str_expression_lisp;
            String[] strArr = lispstr.Split(',');
            string first=strArr[0];
            string last=strArr.Last();
            if(first.StartsWith("(") && last.EndsWith(")"))
            {
                return first.TrimStart('(');
            }
            else
            {
                throw new Exception("字符串格式不对");
            }
        }
        private LispExpression GetParamAStr()
        {
            string lispstr = str_expression_lisp;
            String[] strArr = lispstr.Split(',');
            string first = strArr[0];            
            string last = strArr.Last();
            if (first.StartsWith("(") && last.EndsWith(")"))
            {
                string second = strArr[1];
                if (second.StartsWith("("))
                {
                    List<string> mid = new List<string>();
                    int flag = 0;
                    for(int i = 1; i < strArr.Length; i++)
                    {                       
                        if (strArr[i].Contains("("))
                        {
                            //int coul = Regex.Matches(strArr[i], "(").Count;
                            flag += 1;
                        }
                        if (strArr[i].Contains(")"))
                        {
                            int cour = Regex.Matches(strArr[i], @"[)]").Count;
                            flag -= cour;
                        }
                        mid.Add(strArr[i]);
                        if(flag <= 0)
                        {
                            break;
                        }
                    }
                    string temp = string.Join(",", mid);
                    if (temp.EndsWith(")"))
                    {
                        return new LispExpression(temp.Substring(0, (temp.Length +flag)), Vars);
                    }
                    return new LispExpression(temp, Vars);
                }
                else
                {
                    if (second.EndsWith(")"))
                    {
                        return new LispExpression(second.Substring(0,second.Length-1),Vars);
                    }
                    return new LispExpression(second,Vars);
                }
            }
            else
            {
                throw new Exception("字符串格式不对");
            }
        }
        private LispExpression GetParamBStr()
        {
            string lispstr = str_expression_lisp;
            String[] strArr = lispstr.Split(',');
            string first = strArr[0];
            string last = strArr.Last();
            
            if (first.StartsWith("(") && last.EndsWith(")"))
            {
                string lasttrim = last.Substring(0,last.Length-1);
                if (lasttrim.EndsWith(")"))
                {
                    List<string> mid = new List<string>();
                    int flag = -1;
                    for (int i = strArr.Length-1; i > 0; i--)
                    {
                        if (strArr[i].Contains(")"))
                        {
                            int cour = Regex.Matches(strArr[i], @"[)]").Count;
                            flag += cour;
                        }
                        if (strArr[i].Contains("("))
                        {
                            //int coul = Regex.Matches(strArr[i], "[(]").Count;
                            flag -= 1;
                        }
                        mid.Insert(0,strArr[i]);
                        if (flag == 0)
                        {
                            break;
                        }
                    }
                    string temp = string.Join(",", mid);
                    string temp1=temp.Substring(0,temp.Length-1);
                    return new LispExpression(temp1, Vars);
                }
                else
                {
                    return new LispExpression(lasttrim, Vars);
                }
            }
            else
            {
                throw new Exception("字符串格式不对");
            }
        }
        public string ToString()
        {
            return Expression;
        }
    }
}
